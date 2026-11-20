//! `StatementBuilder.DeclareLocalFunctions` and the calls of local functions
//! (`CallBuilder.Build` with `ResolveLocalFunction`).

use crate::il::ast::{CallData, FunctionKind, Inst, NodeId};
use crate::semantics::Method;

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

impl Builder<'_, '_, '_, '_> {
    /// The local functions of the top-level function, whose names `HidesVariableWithName`
    /// counts among the function's (`HidesVariableOrNestedFunction`).
    pub(crate) fn begin_local_functions(&mut self) -> Result<(), Unsupported> {
        self.local_functions = self.f.local_functions.clone();
        for lf in self.local_functions.clone() {
            let name = self.local_function_name(lf)?;
            self.variable_names.push(name);
        }
        Ok(())
    }

    /// `ILFunction.Name` of a local function: the name AssignVariableNames gave it, else its
    /// method's.
    pub(super) fn local_function_name(&mut self, lf: NodeId) -> Result<String, Unsupported> {
        let Inst::Function {
            kind: FunctionKind::LocalFunction,
            read: Some(read),
            name,
            ..
        } = self.f.inst(lf)
        else {
            return Err("a local function that was not read from a method".to_owned());
        };
        if let Some(name) = name {
            return Ok(name.clone());
        }
        let method = read.method;
        Ok(self
            .ts
            .method_definition(method)
            .map_err(|e| e.to_string())?
            .name)
    }

    /// `DeclareLocalFunctions(currentFunction, container, blockStatement)`: the declarations of
    /// the local functions declared in `container`, in the ordinal order of their names, at
    /// the end of the block.
    pub(crate) fn declare_local_functions(
        &mut self,
        container: NodeId,
        block_statement: AstId,
    ) -> Result<(), Unsupported> {
        let mut declared = Vec::new();
        for lf in self.local_functions.clone() {
            if let Inst::Function {
                declaration_scope: Some(scope),
                ..
            } = *self.f.inst(lf)
                && scope == container
            {
                let name = self.local_function_name(lf)?;
                declared.push((name, lf));
            }
        }
        declared.sort_by(|a, b| a.0.cmp(&b.0));
        for (_, lf) in declared {
            let stmt = self.translate_local_function(lf)?;
            self.declared_local_functions += 1;
            let AstNode::Block { statements } = self.ast.node_mut(block_statement) else {
                return Err("local functions declared outside a block".to_owned());
            };
            statements.push(stmt);
        }
        Ok(())
    }

    /// `TranslateFunction(function)`: the body of the local function, by a statement builder
    /// of its own; the host writes the declaration of the reduced method around it. Functions
    /// without a body are declared `extern`.
    fn translate_local_function(&mut self, lf: NodeId) -> Result<AstId, Unsupported> {
        let Inst::Function {
            return_type,
            variables,
            body,
            read: Some(read),
            local_functions,
            reduced: Some(_),
            ..
        } = self.f.inst(lf).clone()
        else {
            return Err("a local function without reduced method".to_owned());
        };
        // A method without body declares an `extern` local function.
        if read.method_operand.is_some() {
            let stmt = self
                .ast
                .add(AstNode::LocalFunctionDeclaration { body: None });
            return Ok(self.ast.with_il(stmt, lf));
        }
        // `new StatementBuilder(..., function, ...)`: the names `HidesVariableWithName` checks
        // are those of the function and its ancestors, with their local functions.
        let mut names = self.variable_names.clone();
        names.extend(variables.iter().map(|&v| self.f.var(v).name.clone()));
        for &nested in &local_functions {
            names.push(self.local_function_name(nested)?);
        }
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
            std::mem::replace(&mut self.is_iterator, read.is_iterator),
            std::mem::replace(&mut self.in_lambda, false),
            std::mem::replace(&mut self.in_local_function, true),
            std::mem::replace(&mut self.local_functions, local_functions.clone()),
        );
        self.return_container = body;
        // `currentResultType`: an async function returns the result of its task.
        self.result_type = read.async_return_type.unwrap_or(return_type);
        let saved_function = self.current_function.replace(lf);
        let saved_async = std::mem::replace(&mut self.is_async, read.async_return_type.is_some());
        let saved_end_labels = std::mem::take(&mut self.end_container_labels);
        let saved_emit = std::mem::take(&mut self.emit_as_ref_readonly);
        let block = self.convert_as_block(body);
        self.end_container_labels = saved_end_labels;
        self.emit_as_ref_readonly = saved_emit;
        self.current_function = saved_function;
        self.is_async = saved_async;
        self.return_statements = saved.6;
        self.labels = saved.7;
        self.duplicate_labels = saved.8;
        self.is_iterator = saved.9;
        self.in_lambda = saved.10;
        self.in_local_function = saved.11;
        self.local_functions = saved.12;
        self.return_container = saved.0;
        self.result_type = saved.1;
        self.variable_names = saved.2;
        self.continue_target = saved.3;
        self.continue_count = saved.4;
        self.break_target = saved.5;
        let declared = std::mem::replace(&mut self.declared_local_functions, saved_declared);
        let block = block?;
        if declared != local_functions.len() {
            return Err("a local function declared in a loop or switch".to_owned());
        }
        let stmt = self
            .ast
            .add(AstNode::LocalFunctionDeclaration { body: Some(block) });
        Ok(self.ast.with_il(stmt, lf))
    }

    /// `ResolveLocalFunction(method)`: the local function read from `method`, among those of
    /// the current function and its ancestors.
    fn resolve_local_function(
        &self,
        method: crate::typesystem::members::MethodDefId,
    ) -> Option<NodeId> {
        let find = |list: &[NodeId]| {
            list.iter().copied().find(|&lf| {
                matches!(self.f.inst(lf), Inst::Function { read: Some(read), .. } if read.method == method)
            })
        };
        let mut current = self.current_function;
        while let Some(function) = current {
            if let Inst::Function {
                local_functions, ..
            } = self.f.inst(function)
                && let Some(lf) = find(local_functions)
            {
                return Some(lf);
            }
            current = self.f.node(function).parent;
            while let Some(p) = current {
                if matches!(self.f.inst(p), Inst::Function { .. }) {
                    break;
                }
                current = self.f.node(p).parent;
            }
        }
        find(&self.f.local_functions)
    }

    /// `CallBuilder.Build` of a call of a local function: `Name(arguments)`, without
    /// disambiguation, the target the local function's method group.
    pub(crate) fn local_function_call(
        &mut self,
        inst: NodeId,
        call: &CallData,
    ) -> Result<TranslatedExpression, Unsupported> {
        let operand = &call.method;
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a call of an unresolved method")?;
        let crate::typesystem::resolve::MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a call of an unresolved method".to_owned());
        };
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // The identifier is annotated with the function (`ide.AddAnnotation(localFunction)`).
        let target = self.local_function_reference(operand, &method, true)?;
        // shortcut: the arguments of a call of a generic local function are converted to
        // parameter types whose type parameters the engine does not relate to the caller's
        // (a cast `LocalFunctions.TestGenericArgs` does without); find the difference before
        // lifting this.
        if matches!(self.ast.node(target), AstNode::GenericIdentifier { .. }) {
            return Err("a reference of a generic local function".to_owned());
        }
        let mut list = self.argument_list(call, &method, id, 0, declaring, false)?;
        // `TransformParamsArgument` never takes the expanded form: `IsUnambiguousCall` looks
        // the name up on the local function's method group, whose type has no members.
        list.params_candidate = None;
        let arguments = self.written_arguments(&list, 0);
        let e = self.ast.add(AstNode::Invocation { target, arguments });
        let target_rr = self.rr_of(target);
        self.ast.with_rr(
            e,
            Rr::Invocation {
                target: Box::new(target_rr),
                method: Box::new(operand.clone()),
                ty: method.return_type,
                extension: false,
                delegate: false,
                found_by_lookup: false,
                expanded: false,
                method_args: None,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: method.return_type,
        })
    }

    /// The local function's `IdentifierExpression` with the resolve result of `ToMethodGroup`,
    /// with the type arguments of the reduced method, annotated with the function where
    /// `annotate`.
    pub(crate) fn local_function_reference(
        &mut self,
        operand: &crate::il::ast::MethodOperand,
        method: &Method,
        annotate: bool,
    ) -> Result<AstId, Unsupported> {
        let reference = operand
            .local_function
            .as_ref()
            .ok_or("a reference of no local function")?;
        let lf = self
            .resolve_local_function(reference.method)
            .ok_or("a reference of a local function of another function")?;
        let name = self.local_function_name(lf)?;
        let generated = reference
            .reduced
            .number_of_compiler_generated_type_parameters as usize;
        // `LocalFunctionMethod.TypeArguments`: those past the compiler-generated ones.
        let type_arguments: Vec<_> = method
            .method_args
            .as_deref()
            .unwrap_or_default()
            .iter()
            .skip(generated)
            .copied()
            .collect();
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        let index = self.ast.method_groups.len();
        let unknown = self
            .ts
            .intern(crate::typesystem::types::TypeData::UnknownSpecial);
        self.ast
            .method_groups
            .push(std::rc::Rc::new(crate::semantics::MethodGroup {
                target: crate::semantics::ResolveResult::Expression(unknown),
                name: name.clone(),
                method_lists: vec![crate::semantics::MethodList {
                    declaring,
                    methods: vec![method.clone()],
                }],
                type_arguments: type_arguments.clone(),
                extension_methods: Vec::new(),
            }));
        let target = if type_arguments.is_empty() {
            self.ast.add(AstNode::Identifier { name: name.clone() })
        } else {
            self.ast.add(AstNode::GenericIdentifier {
                name: name.clone(),
                type_arguments,
            })
        };
        if annotate {
            self.ast.with_il(target, lf);
        }
        self.ast.with_rr(
            target,
            Rr::LocalFunctionGroup {
                index,
                name,
                method: Box::new(operand.clone()),
            },
        );
        Ok(target)
    }
}
