//! `ExpressionBuilder.TranslateFunction`: a delegate's lambda, and the resolve result
//! (`DecompiledLambdaResolveResult`) overload resolution and type inference query it by.

use std::cell::Cell;
use std::rc::Rc;

use crate::semantics::decompiled_lambda::DecompiledLambda;

use crate::il::ast::{FunctionKind, Inst, NodeId, VariableKind};
use crate::semantics::{Conversions, Parameter, ResolveResult, TypeInference};
use crate::typesystem::members::RefKind;
use crate::typesystem::types::TypeData;

use crate::il::transforms::transform_collection_and_object_initializers::contains_anonymous_type;

use super::ast::{AstNode, Overflow, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

const COMPILER_GENERATED: &str = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";

impl Builder<'_, '_, '_, '_> {
    /// `VisitILFunction`: `(D)(parameters) => body`, a lambda with explicitly typed
    /// parameters that the body uses, cast to its delegate type. Anonymous methods, async
    /// lambdas, attributes and parameters with modifiers or defaults are not translated; a
    /// parameter of an anonymous type makes the parameter list implicitly typed.
    pub(crate) fn translate_function(
        &mut self,
        function: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Function {
            kind,
            delegate_type,
            return_type,
            parameters,
            variables,
            body,
            read,
            local_functions,
            ..
        } = self.f.inst(function).clone()
        else {
            unreachable!("a function");
        };
        if kind != FunctionKind::Delegate {
            return Err("a function that is no delegate".to_owned());
        }
        let delegate_type = delegate_type.ok_or("a function without delegate type")?;
        let read = read.ok_or("a function without method")?;
        if read.async_return_type.is_some() || read.is_iterator {
            return Err("an async or iterator lambda".to_owned());
        }
        if !read.warnings.is_empty() || !local_functions.is_empty() {
            return Err("a lambda with warnings or local functions".to_owned());
        }
        if !self.settings.use_lambda_syntax {
            return Err("an anonymous method".to_owned());
        }
        // Attributes other than `[CompilerGenerated]` make a C# 10 lambda with attributes.
        let method = read.method;
        let attributes = self
            .ts
            .method_attributes(method)
            .map_err(|e| e.to_string())?;
        if attributes
            .iter()
            .any(|a| self.ts.reflection_name(a.ty) != COMPILER_GENERATED)
            || !self
                .ts
                .return_type_attributes(method)
                .map_err(|e| e.to_string())?
                .is_empty()
        {
            return Err("a lambda with attributes".to_owned());
        }
        let (_, params) = self.ts.method_signature(method);
        if params.len() != parameters.len() {
            return Err("a lambda whose parameters differ from its method's".to_owned());
        }
        let declaring = self
            .ts
            .method_declaring(method)
            .map_err(|e| e.to_string())?;
        // `MakeParameters`: a parameter of an anonymous type, which cannot be named, makes the
        // whole parameter list implicitly typed.
        let mut implicitly_typed = false;
        for (i, (p, &(ty, _))) in params.iter().zip(&parameters).enumerate() {
            if p.ref_kind != RefKind::None
                || matches!(self.ts.data(ty), TypeData::ByRef(_))
                || p.is_params
                || p.optional
                || p.has_constant_in_signature
                || p.declared_scope != crate::typesystem::flags::ScopedKind::None
                || p.effective_scope != crate::typesystem::flags::ScopedKind::None
            {
                return Err("a lambda parameter with modifiers".to_owned());
            }
            if self.settings.anonymous_types && contains_anonymous_type(self.ts, ty) {
                implicitly_typed = true;
            }
            let attributes = self
                .ts
                .parameter_attributes(method.module, &params, i, declaring)
                .map_err(|e| e.to_string())?;
            if !attributes.is_empty() {
                return Err("a lambda parameter with attributes".to_owned());
            }
        }

        // `new StatementBuilder(..., function, ...).ConvertAsBlock(function.Body)`; the names
        // `HidesVariableWithName` checks are those of the function and its ancestors.
        let mut names = self.variable_names.clone();
        names.extend(variables.iter().map(|&v| self.f.var(v).name.clone()));
        let saved = (
            self.return_container,
            self.result_type,
            std::mem::replace(&mut self.variable_names, names),
            self.continue_target.take(),
            std::mem::take(&mut self.continue_count),
            self.break_target.take(),
            std::mem::take(&mut self.return_statements),
            std::mem::take(&mut self.labels),
            std::mem::take(&mut self.duplicate_labels),
            std::mem::replace(&mut self.is_iterator, false),
            std::mem::replace(&mut self.in_lambda, true),
        );
        self.return_container = body;
        self.result_type = return_type;
        let first_node = self.ast.nodes.len();
        let saved_function = self.current_function.replace(function);
        let saved_async = std::mem::replace(&mut self.is_async, false);
        let saved_end_labels = std::mem::take(&mut self.end_container_labels);
        let block = self.convert_as_block(body);
        self.end_container_labels = saved_end_labels;
        self.current_function = saved_function;
        self.is_async = saved_async;
        let returns = std::mem::replace(&mut self.return_statements, saved.6);
        self.labels = saved.7;
        self.duplicate_labels = saved.8;
        self.is_iterator = saved.9;
        self.in_lambda = saved.10;
        self.return_container = saved.0;
        self.result_type = saved.1;
        self.variable_names = saved.2;
        self.continue_target = saved.3;
        self.continue_count = saved.4;
        self.break_target = saved.5;
        let block = block?;

        // `parametersAreUsed`: an identifier of one of the function's parameters.
        let parameter_vars: Vec<_> = variables
            .iter()
            .copied()
            .filter(|&v| self.f.var(v).kind == VariableKind::Parameter)
            .collect();
        let used = self.ast.nodes[first_node..].iter().any(|(n, a)| {
            matches!(n, AstNode::Identifier { .. })
                && matches!(a.rr, Some(Rr::Variable { var, .. }) if parameter_vars.contains(&var))
        });
        // A lambda needs a parameter list its unused parameters can be written in:
        // `ParameterTypesAreAccessible` and `ParametersAreNameable`, else the function is an
        // anonymous method without parameter list, `delegate { ... }`.
        let mut anonymous_method = false;
        if !used && !parameters.is_empty() {
            // The names are the method's parameters' (`function.Parameters`).
            let nameable = params
                .iter()
                .all(|p| crate::il::transforms::assign_variable_names::is_valid_name(&p.name));
            let types: Vec<_> = parameters.iter().map(|&(t, _)| t).collect();
            let accessible = self
                .resolver
                .is_some_and(|r| r.types_accessible(self.ts, &types));
            anonymous_method = !nameable || !accessible;
        }

        let statements = match self.ast.node(block) {
            AstNode::Block { statements } => statements.clone(),
            _ => unreachable!("a block"),
        };
        // An anonymous method always has a block.
        let expression_body = match statements.as_slice() {
            _ if anonymous_method => None,
            [s] => match *self.ast.node(*s) {
                AstNode::Return {
                    expression: Some(e),
                } => Some(e),
                _ => None,
            },
            _ => None,
        };
        let (body, inferred_return_type) = match expression_body {
            Some(e) => {
                let rr = self.rr_of(e);
                (e, self.rr_type(&rr))
            }
            None => {
                // `InferReturnType`: the best common type of the returned expressions.
                let mut expressions = Vec::new();
                for s in returns {
                    if let AstNode::Return {
                        expression: Some(e),
                    } = *self.ast.node(s)
                    {
                        let rr = self.rr_of(e);
                        let ty = self.rr_type(&rr);
                        expressions.push(self.semantic_rr(e, ty));
                    }
                }
                let mut conversions = Conversions::new(self.ts);
                let (ty, _) = TypeInference::new(&mut conversions).best_common_type(&expressions);
                (block, ty)
            }
        };

        let lambda = self.ast.lambdas.len();
        self.ast.lambdas.push(Rc::new(DecompiledLambda {
            delegate_type,
            inferred_return_type,
            return_type,
            parameters: parameters
                .iter()
                .map(|&(ty, ref name)| Parameter {
                    name: name.clone(),
                    ..Parameter::of_type(ty)
                })
                .collect(),
            attempted_conversion_with_type_mismatch: Cell::new(false),
            implicitly_typed,
            anonymous_method,
        }));
        let l = self.ast.add(AstNode::Lambda {
            body,
            expression_body: expression_body.is_some(),
            lambda,
        });
        self.ast.with_il(l, function);
        self.ast.with_rr(l, Rr::Lambda(lambda));
        let c = self.ast.add(AstNode::Cast {
            ty: delegate_type,
            expression: l,
            overflow: Overflow::None,
        });
        self.ast
            .with_rr(c, Rr::LambdaConversion { ty: delegate_type });
        self.ast.with_il(c, function);
        Ok(TranslatedExpression {
            expression: c,
            ty: delegate_type,
        })
    }

    /// The resolve result of a lambda node.
    pub(crate) fn lambda_rr(&self, index: usize) -> ResolveResult {
        ResolveResult::Lambda(self.ast.lambdas[index].clone())
    }
}
