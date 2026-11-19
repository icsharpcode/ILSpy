//! `ExpressionBuilder.TranslateFunction`: a delegate's lambda, and the resolve result
//! (`DecompiledLambdaResolveResult`) overload resolution and type inference query it by.

use std::cell::Cell;
use std::rc::Rc;

use crate::semantics::decompiled_lambda::DecompiledLambda;

use crate::il::ast::{FunctionKind, Inst, NodeId, VariableKind};
use crate::semantics::{Conversions, Parameter, ResolveResult, TypeInference};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::RefKind;
use crate::typesystem::types::{TypeData, TypeId};

use crate::il::transforms::transform_collection_and_object_initializers::contains_anonymous_type;

use super::ast::{AstNode, Overflow, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

const COMPILER_GENERATED: &str = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
const ASYNC_STATE_MACHINE: &str = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
const DEBUGGER_STEP_THROUGH: &str = "System.Diagnostics.DebuggerStepThroughAttribute";

impl Builder<'_, '_, '_, '_> {
    /// `VisitILFunction`: `(D)(parameters) => body` or `(D)delegate (parameters) { body }`
    /// cast to its delegate type. The host writes the attribute sections and the parameter
    /// declarations (`MakeParameters`) of the function itself; this decides the syntax.
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
        // A delegate's or an expression tree's lambda (a local function is declared).
        if kind == FunctionKind::LocalFunction {
            return Err("a function that is no delegate".to_owned());
        }
        let delegate_type = delegate_type.ok_or("a function without delegate type")?;
        let is_async = read.as_ref().is_some_and(|r| r.async_return_type.is_some());
        if read.as_ref().is_some_and(|r| r.is_iterator) {
            return Err("an async or iterator lambda".to_owned());
        }
        // The host writes the warnings as comments before the body's statements.
        let has_warnings = read.as_ref().is_some_and(|r| !r.warnings.is_empty());
        // `MakeParameters` over `TypeSystemAstBuilder.ConvertParameter`; the parameters of a
        // function without method (of an expression tree) have no modifiers.
        let mut any_anonymous_type = false;
        let mut all_plain = true;
        let mut any_modifier = false;
        let mut any_attribute = false;
        let mut any_params_or_default = false;
        let mut has_attributes = false;
        let mut param_names: Vec<String> = parameters.iter().map(|(_, n)| n.clone()).collect();
        if let Some(read) = &read {
            // The attribute sections of the method other than `[CompilerGenerated]` (and the
            // state machine's for an async lambda), and of its return type.
            let method = read.method;
            let attributes = self
                .ts
                .method_attributes(method)
                .map_err(|e| e.to_string())?;
            has_attributes = attributes.iter().any(|a| {
                let name = self.ts.reflection_name(a.ty);
                name != COMPILER_GENERATED
                    && !(is_async && (name == ASYNC_STATE_MACHINE || name == DEBUGGER_STEP_THROUGH))
            }) || !self
                .ts
                .return_type_attributes(method)
                .map_err(|e| e.to_string())?
                .is_empty();
            let (_, params) = self.ts.method_signature(method);
            if params.len() != parameters.len() {
                return Err("a lambda whose parameters differ from its method's".to_owned());
            }
            param_names = params.iter().map(|p| p.name.clone()).collect();
            let declaring = self
                .ts
                .method_declaring(method)
                .map_err(|e| e.to_string())?;
            for (i, p) in params.iter().enumerate() {
                let attributes = self
                    .ts
                    .parameter_attributes(method.module, &params, i, declaring)
                    .map_err(|e| e.to_string())?;
                let has_default = self
                    .ts
                    .default_value_assignment_allowed(&params, i, declaring)
                    .map_err(|e| e.to_string())?;
                let modifier = p.ref_kind != RefKind::None;
                any_modifier |= modifier || p.is_params;
                any_attribute |= !attributes.is_empty();
                any_params_or_default |= p.is_params || has_default;
                all_plain &= !modifier
                    && !p.is_params
                    && p.declared_scope == crate::typesystem::flags::ScopedKind::None
                    && attributes.is_empty()
                    && !has_default;
            }
        }
        for &(ty, _) in &parameters {
            if self.settings.anonymous_types && contains_anonymous_type(self.ts, ty) {
                any_anonymous_type = true;
            }
        }
        // An anonymous type cannot be named: the parameters are written without types when
        // the rest of their syntax permits it.
        let implicitly_typed = any_anonymous_type && all_plain;

        // `new StatementBuilder(..., function, ...).ConvertAsBlock(function.Body)`; the names
        // `HidesVariableWithName` checks are those of the function and its ancestors, with
        // their local functions.
        let mut names = self.variable_names.clone();
        names.extend(variables.iter().map(|&v| self.f.var(v).name.clone()));
        for &nested in &local_functions {
            names.push(self.local_function_name(nested)?);
        }
        let saved_local_functions =
            std::mem::replace(&mut self.local_functions, local_functions.clone());
        let saved_declared = std::mem::take(&mut self.declared_local_functions);
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
        // `currentResultType`: an async function returns the result of its task.
        self.result_type = read
            .as_ref()
            .and_then(|r| r.async_return_type)
            .unwrap_or(return_type);
        let first_node = self.ast.nodes.len();
        let saved_function = self.current_function.replace(function);
        let saved_async = std::mem::replace(&mut self.is_async, is_async);
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
        self.local_functions = saved_local_functions;
        let declared = std::mem::replace(&mut self.declared_local_functions, saved_declared);
        let block = block?;
        if declared != local_functions.len() {
            return Err("a local function declared in a loop or switch".to_owned());
        }

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
        let mut is_lambda = if implicitly_typed || has_attributes || any_attribute {
            true
        } else if self.settings.use_lambda_syntax && !any_modifier {
            // `ParameterTypesAreAccessible` and `ParametersAreNameable`: a lambda needs a
            // parameter list its unused parameters can be written in.
            used || {
                let nameable = param_names
                    .iter()
                    .all(|n| crate::il::transforms::assign_variable_names::is_valid_name(n));
                let types: Vec<_> = parameters.iter().map(|&(t, _)| t).collect();
                nameable
                    && self
                        .resolver
                        .is_some_and(|r| r.types_accessible(self.ts, &types))
            }
        } else {
            false
        };
        // `params` and default values are written on the explicitly typed parameter list of a
        // lambda only.
        let keep_params_and_defaults = self.settings.lambda_optional_and_params_parameters
            && (is_lambda || used)
            && !implicitly_typed;
        if keep_params_and_defaults && any_params_or_default {
            is_lambda = true;
        }
        let has_parameter_list = is_lambda || (used && !parameters.is_empty());

        let statements = match self.ast.node(block) {
            AstNode::Block { statements } => statements.clone(),
            _ => unreachable!("a block"),
        };
        // An anonymous method always has a block, as has a body with warnings.
        let expression_body = match statements.as_slice() {
            [s] if is_lambda && !has_warnings => match *self.ast.node(*s) {
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
        let inferred_return_type = if is_async {
            self.task_type(inferred_return_type)
        } else {
            inferred_return_type
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
            anonymous_method: !is_lambda,
            has_parameter_list,
            is_async,
        }));
        let l = self.ast.add(AstNode::Lambda {
            body,
            expression_body: expression_body.is_some(),
            lambda,
            keep_params_and_defaults,
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

    /// `GetTaskType`: `Task` for `void`, else `Task<result>`.
    fn task_type(&mut self, result: TypeId) -> TypeId {
        if matches!(
            self.ts.data(result),
            TypeData::Unknown { .. } | TypeData::UnknownSpecial
        ) {
            return self.ts.intern(TypeData::UnknownSpecial);
        }
        if self.ts.is_known(result, KnownTypeCode::Void) {
            return self.ts.known_type(KnownTypeCode::Task);
        }
        let generic = self.ts.known_type(KnownTypeCode::TaskOfT);
        if self.ts.definition(generic).is_none() {
            return self.ts.intern(TypeData::UnknownSpecial);
        }
        self.ts.intern(TypeData::Inst {
            generic,
            args: Box::new([result]),
        })
    }

    /// The resolve result of a lambda node.
    pub(crate) fn lambda_rr(&self, index: usize) -> ResolveResult {
        ResolveResult::Lambda(self.ast.lambdas[index].clone())
    }
}
