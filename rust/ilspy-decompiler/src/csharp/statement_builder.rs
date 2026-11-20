//! `StatementBuilder`: the C# statements of an ILAst function body.

use crate::il::ast::{
    BlockKind, BranchTarget, ContainerKind, Function, FunctionKind, Inst, NodeId,
};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::ast::{Ast, AstId, AstNode, Overflow, Rr};
use super::{Builder, Unsupported};

/// `new StatementBuilder(...).ConvertAsBlock(function.Body)` for a top-level function: the
/// body as a `BlockStatement` and the function as the translation left it (a copy of
/// `read.function`), or why it is not translated.
pub fn convert_body(
    ts: &mut TypeSystem<'_, '_>,
    read: &ReadFunction,
    settings: crate::il::transforms::Settings,
    use_ref_locals: bool,
    resolver: Option<&dyn crate::il::transforms::Resolver>,
) -> Result<(Ast, AstId, Function), Unsupported> {
    // The unbound type argument the type syntax of a generic definition names.
    ts.intern(TypeData::Unbound);
    let mut function = read.function.clone();
    let f = &mut function;
    let body = f.body.ok_or("a function without body")?;
    if read.kind != FunctionKind::TopLevelFunction {
        return Err("a nested function".to_owned());
    }
    let method = ts
        .method_definition(read.method)
        .map_err(|e| e.to_string())?;
    let current_type = ts
        .method_declaring(read.method)
        .map_err(|e| e.to_string())?;
    let variable_names = read
        .variables
        .iter()
        .map(|&v| f.var(v).name.clone())
        .collect();
    let mut b = Builder {
        f,
        ts,
        ast: Ast::default(),
        return_container: body,
        // `currentResultType`: an async function returns the result of its task.
        result_type: read.async_return_type.unwrap_or(method.ret),
        settings,
        use_ref_locals,
        resolver,
        current_type,
        variable_names,
        continue_target: None,
        continue_count: 0,
        break_target: None,
        return_statements: Vec::new(),
        is_iterator: read.is_iterator,
        is_async: read.async_return_type.is_some(),
        in_lambda: false,
        emit_as_ref_readonly: false,
        root_variables: read.variables.clone(),
        found_override: None,
        loaded_stack_slots: Vec::new(),
        named_argument_map: None,
        consumed_format_strings: Vec::new(),
        needs_boxing_conversion: false,
        local_functions: Vec::new(),
        declared_local_functions: 0,
        in_local_function: false,
        current_function: None,
        labels: Vec::new(),
        end_container_labels: Vec::new(),
        case_label_mapping: Vec::new(),
        duplicate_labels: std::collections::HashMap::new(),
        replaced_loads: Vec::new(),
    };
    b.begin_local_functions()?;
    let stmt = b.convert(body)?;
    if b.declared_local_functions != b.local_functions.len() {
        return Err("a local function declared in a loop or switch".to_owned());
    }
    b.ast.with_il(stmt, body);
    // `stmt as BlockStatement ?? new BlockStatement { stmt }`: a container is a block.
    let ast = b.ast;
    Ok((ast, stmt, function))
}

impl Builder<'_, '_, '_, '_> {
    /// `Convert(inst)`.
    pub(super) fn convert(&mut self, inst: NodeId) -> Result<AstId, Unsupported> {
        match self.f.inst(inst).clone() {
            Inst::BlockContainer { kind, blocks, .. } => {
                self.visit_block_container(inst, kind, &blocks)
            }
            Inst::Leave { target, value } => self.visit_leave(inst, target, value),
            // `VisitNop`: an empty statement (with the instruction's comment, which no
            // transform of the engine sets).
            Inst::Nop { .. } => {
                let s = self.ast.add(AstNode::Empty);
                Ok(self.ast.with_il(s, inst))
            }
            // `VisitStLoc` and `VisitStObj` strip the top-level `ref` of a ref reassignment.
            Inst::StLoc { .. } | Inst::StObj { .. } => {
                let e = self.translate(inst, None)?;
                let expression = match *self.ast.node(e.expression) {
                    AstNode::RefDirection { expression } => {
                        self.unwrap_child(e.expression, expression).expression
                    }
                    _ => e.expression,
                };
                let s = self.ast.add(AstNode::ExpressionStatement { expression });
                Ok(self.ast.with_il(s, inst))
            }
            Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } => self.visit_if(inst, condition, true_inst, false_inst),
            Inst::Block {
                kind: BlockKind::ControlFlow,
                instructions,
                final_inst,
            } => self.visit_block(inst, &instructions, final_inst),
            // `VisitThrow`: the exception translated with `Exception` as type hint.
            Inst::Throw { arg } => {
                let exception = self.ts.known_type(KnownTypeCode::Exception);
                let e = self.translate(arg, Some(exception))?;
                let s = self.ast.add(AstNode::Throw {
                    expression: Some(e.expression),
                });
                Ok(self.ast.with_il(s, inst))
            }
            // `VisitRethrow`
            Inst::Rethrow => {
                let s = self.ast.add(AstNode::Throw { expression: None });
                Ok(self.ast.with_il(s, inst))
            }
            Inst::TryCatch {
                try_block,
                handlers,
            } => self.visit_try_catch(inst, try_block, &handlers),
            Inst::TryFinally {
                try_block,
                finally_block,
            } => self.visit_try_finally(inst, try_block, finally_block),
            // `VisitLockInstruction`
            Inst::Lock {
                on_expression,
                body,
            } => {
                let expression = self.translate(on_expression, None)?.expression;
                let body = self.convert_as_block(body)?;
                let s = self.ast.add(AstNode::Lock { expression, body });
                Ok(self.ast.with_il(s, inst))
            }
            Inst::Using {
                variable,
                resource_expression,
                body,
                is_async,
                is_ref_struct,
            } => self.visit_using(
                inst,
                variable,
                resource_expression,
                body,
                is_async,
                is_ref_struct,
            ),
            Inst::YieldReturn { value } => self.visit_yield_return(inst, value),
            // `VisitBranch`: `continue` for the loop's continue target, else `goto`.
            Inst::Branch {
                target: BranchTarget::Block(target),
            } if Some(target) == self.continue_target => {
                self.continue_count += 1;
                let s = self.ast.add(AstNode::Continue);
                Ok(self.ast.with_il(s, inst))
            }
            // `goto case` or `goto default` for a block a section of the switch inlines.
            Inst::Branch {
                target: BranchTarget::Block(target),
            } if self.case_label_mapping.iter().any(|(b, ..)| *b == target) => {
                let (_, label, ty) = self
                    .case_label_mapping
                    .iter()
                    .find(|(b, ..)| *b == target)
                    .cloned()
                    .expect("a mapped block");
                let s = match label {
                    Some(value) => {
                        let label = match value {
                            super::switch::CaseValue::Integer(v) => self.case_label(v, ty)?,
                            super::switch::CaseValue::String(c) => self.string_case_label(ty, c)?,
                        };
                        self.ast.add(AstNode::GotoCase { label })
                    }
                    None => self.ast.add(AstNode::GotoDefault),
                };
                Ok(self.ast.with_il(s, inst))
            }
            Inst::Branch {
                target: BranchTarget::Block(target),
            } => {
                let name = self.ensure_unique_label(target);
                let s = self.ast.add(AstNode::Goto { name });
                Ok(self.ast.with_il(s, inst))
            }
            Inst::PinnedRegion {
                variable,
                init,
                body,
            } => self.pinned_region(inst, variable, init, body),
            // `VisitIsInst`: an unused `isinst` is `arg is T`, even of a value type.
            Inst::IsInst { arg, ty } => {
                let a = self.translate(arg, None)?;
                let a = self.unwrap_boxing_conversion(a);
                let e = self.ast.add(AstNode::IsType {
                    expression: a.expression,
                    ty: ty.ty,
                });
                // shortcut: the host writes a tuple type as its underlying type and annotates a
                // `TypeIsResolveResult` where the managed statement keeps the tuple type and a
                // plain `bool`; upgrade when an `isinst` statement of a tuple type is served.
                if matches!(self.ts.data(ty.ty), TypeData::Tuple { .. }) {
                    return Err("the statement IsInst".to_owned());
                }
                self.ast.with_rr(e, Rr::TypeIs { ty: ty.ty });
                self.ast.with_il(e, inst);
                let s = self.ast.add(AstNode::ExpressionStatement { expression: e });
                Ok(self.ast.with_il(s, inst))
            }
            // `VisitInitblk` and `VisitCpblk`: the `Unsafe` block method, commented.
            Inst::Initblk {
                address: a,
                value: b,
                size: c,
                unaligned,
                ..
            }
            | Inst::Cpblk {
                dest: a,
                source: b,
                size: c,
                unaligned,
                ..
            } => {
                let initblk = matches!(self.f.inst(inst), Inst::Initblk { .. });
                let name = match (initblk, unaligned != 0) {
                    (true, false) => "InitBlock",
                    (true, true) => "InitBlockUnaligned",
                    (false, false) => "CopyBlock",
                    (false, true) => "CopyBlockUnaligned",
                };
                let mut arguments = Vec::with_capacity(3);
                for arg in [a, b, c] {
                    arguments.push(self.translate(arg, None)?.expression);
                }
                let void = self.ts.known_type(KnownTypeCode::Void);
                let e = self.call_unsafe_intrinsic(name, arguments, void, Some(inst), Vec::new());
                let s = self.ast.add(AstNode::ExpressionStatement {
                    expression: e.expression,
                });
                let comment = if initblk {
                    " IL initblk instruction"
                } else {
                    " IL cpblk instruction"
                };
                self.ast.with_leading_comment(s, comment, 0);
                Ok(self.ast.with_il(s, inst))
            }
            // `VisitCkfinite`: `if (!float.IsFinite(arg)) throw new ArithmeticException();`,
            // with no resolve results but the exception type's.
            Inst::Ckfinite { arg } => {
                let a = self.translate(arg, None)?.expression;
                let float = self.ts.known_type(KnownTypeCode::Single);
                let t = self.ast.add(AstNode::TypeReference { ty: float });
                let m = self.ast.add(AstNode::MemberReference {
                    target: t,
                    name: "IsFinite".to_owned(),
                });
                let call = self.ast.add(AstNode::Invocation {
                    target: m,
                    arguments: vec![a],
                });
                let condition = self.ast.add(AstNode::LogicNot { expression: call });
                let exception =
                    self.ts
                        .find_top_level(crate::typesystem::names::TopLevelTypeName::new(
                            "System",
                            "ArithmeticException",
                            0,
                        ));
                let creation = self.ast.add(AstNode::ObjectCreate {
                    ty: exception,
                    arguments: Vec::new(),
                    initializer: None,
                });
                let throw = self.ast.add(AstNode::Throw {
                    expression: Some(creation),
                });
                let s = self.ast.add(AstNode::IfElse {
                    condition,
                    true_statement: throw,
                    false_statement: None,
                });
                Ok(self.ast.with_il(s, inst))
            }
            Inst::Switch { .. } | Inst::Branch { .. } | Inst::TryFault { .. } => Err(format!(
                "the statement {}",
                super::inst_name(self.f.inst(inst))
            )),
            // `Default`: an expression statement; without ref locals a top-level `ref` is
            // dropped, which introduces a load.
            _ => {
                let s = self.expression_statement(inst)?;
                let AstNode::ExpressionStatement { expression } = *self.ast.node(s) else {
                    unreachable!("an expression statement");
                };
                if !self.use_ref_locals
                    && let AstNode::RefDirection { expression: inner } = *self.ast.node(expression)
                {
                    *self.ast.node_mut(s) = AstNode::ExpressionStatement { expression: inner };
                }
                Ok(s)
            }
        }
    }

    /// `ConvertAsBlock(inst)`: the statement with the instruction (again), in a block.
    pub(super) fn convert_as_block(&mut self, inst: NodeId) -> Result<AstId, Unsupported> {
        let s = self.convert(inst)?;
        self.ast.with_il(s, inst);
        Ok(self.as_block(s))
    }

    /// `stmt as BlockStatement ?? new BlockStatement { stmt }`.
    fn as_block(&mut self, s: AstId) -> AstId {
        if matches!(self.ast.node(s), AstNode::Block { .. }) {
            s
        } else {
            self.ast.add(AstNode::Block {
                statements: vec![s],
            })
        }
    }

    /// `VisitTryCatch`: a catch clause per handler, with its type and variable where the
    /// handler needs them and its filter as `when` condition.
    fn visit_try_catch(
        &mut self,
        inst: NodeId,
        try_block: NodeId,
        handlers: &[NodeId],
    ) -> Result<AstId, Unsupported> {
        let try_block = self.convert_as_block(try_block)?;
        let mut catch_clauses = Vec::with_capacity(handlers.len());
        for &handler in handlers {
            let Inst::TryCatchHandler {
                filter,
                body,
                variable,
                ..
            } = *self.f.inst(handler)
            else {
                return Err("a catch handler of another kind".to_owned());
            };
            let v = self.f.var(variable);
            let (name, ty) = (v.name.clone(), v.ty);
            let (variable_name, clause_type) =
                if v.store_count() > 1 || v.load_count() > 0 || v.address_count() > 0 {
                    (Some(name), Some(ty))
                } else if !self.ts.is_known(ty, KnownTypeCode::Object) {
                    (None, Some(ty))
                } else {
                    (None, None)
                };
            let condition = if matches!(*self.f.inst(filter), Inst::LdcI4 { value: 1 }) {
                None
            } else {
                Some(self.translate_condition(filter)?.expression)
            };
            let body = self.convert_as_block(body)?;
            let c = self.ast.add(AstNode::CatchClause {
                variable_name,
                ty: clause_type,
                condition,
                body,
            });
            // The handler, then the variable's resolve result.
            self.ast.with_il(c, handler);
            self.ast.with_rr(c, Rr::Variable { var: variable, ty });
            catch_clauses.push(c);
        }
        let s = self.ast.add(AstNode::TryCatch {
            try_block,
            catch_clauses,
            finally_block: None,
        });
        Ok(self.ast.with_il(s, inst))
    }

    /// `VisitTryFinally` with `MakeTryCatch`: a try-catch without finally block gets the finally
    /// block, anything else becomes the try block of a new statement.
    fn visit_try_finally(
        &mut self,
        inst: NodeId,
        try_block: NodeId,
        finally_block: NodeId,
    ) -> Result<AstId, Unsupported> {
        let converted = self.convert(try_block)?;
        let extend = matches!(
            self.ast.node(converted),
            AstNode::TryCatch {
                finally_block: None,
                ..
            }
        );
        let s = if extend {
            converted
        } else {
            let try_block = self.as_block(converted);
            self.ast.add(AstNode::TryCatch {
                try_block,
                catch_clauses: Vec::new(),
                finally_block: None,
            })
        };
        let finally = self.convert_as_block(finally_block)?;
        if let AstNode::TryCatch { finally_block, .. } = self.ast.node_mut(s) {
            *finally_block = Some(finally);
        }
        Ok(self.ast.with_il(s, inst))
    }

    /// `IsPossibleLossOfTypeInformation`, where the managed builder casts a lambda's return
    /// value to the return type.
    fn possible_loss_of_type_information(&mut self, given: TypeId, expected: TypeId) -> bool {
        // An anonymous type cannot be named in a cast.
        if crate::il::transforms::transform_collection_and_object_initializers::contains_anonymous_type(
            self.ts, expected,
        ) {
            return false;
        }
        let n = crate::typesystem::fnptr::Normalization::IGNORE_NULLABILITY;
        if self.ts.normalize_with(given, n) == self.ts.normalize_with(expected, n) {
            return false;
        }
        matches!(
            self.ts.data(expected),
            TypeData::Tuple { .. } | TypeData::Dynamic
        ) || matches!(self.ts.data(given), TypeData::Null)
    }

    /// `VisitYieldReturn`: the value converted to the element type of the iterator.
    fn visit_yield_return(&mut self, inst: NodeId, value: NodeId) -> Result<AstId, Unsupported> {
        // `AsyncReturnType` of an async iterator.
        if self.is_async {
            let element_type = self.result_type;
            let e = self.translate(value, Some(element_type))?;
            let e = self.convert_to(e, element_type, true)?;
            let s = self.ast.add(AstNode::YieldReturn {
                expression: e.expression,
            });
            return Ok(self.ast.with_il(s, inst));
        }
        // `GetElementTypeFromIEnumerable(allowIEnumerator: true)`
        let mut element_type = None;
        let mut non_generic = false;
        let bases = self
            .ts
            .all_base_types(self.result_type)
            .map_err(|e| e.to_string())?;
        for b in bases {
            if (self.ts.is_known(b, KnownTypeCode::IEnumerableOfT)
                || self.ts.is_known(b, KnownTypeCode::IEnumeratorOfT))
                && let TypeData::Inst { args, .. } = self.ts.data(b)
            {
                element_type = Some(args[0]);
                break;
            }
            if self.ts.is_known(b, KnownTypeCode::IEnumerable)
                || self.ts.is_known(b, KnownTypeCode::IEnumerator)
            {
                non_generic = true;
            }
        }
        let element_type = match element_type {
            Some(t) => t,
            None if non_generic => self.ts.known_type(KnownTypeCode::Object),
            None => return Err("an iterator of an unknown element type".to_owned()),
        };
        let e = self.translate(value, Some(element_type))?;
        let e = self.convert_to(e, element_type, true)?;
        let s = self.ast.add(AstNode::YieldReturn {
            expression: e.expression,
        });
        Ok(self.ast.with_il(s, inst))
    }

    /// `VisitUsingInstruction` for a `using` (or `await using`) of a resource whose type
    /// implements `IDisposable` (`IAsyncDisposable`), declared where the body uses the variable.
    fn visit_using(
        &mut self,
        inst: NodeId,
        variable: crate::il::ast::VarId,
        resource_expression: NodeId,
        body: NodeId,
        is_async: bool,
        is_ref_struct: bool,
    ) -> Result<AstId, Unsupported> {
        let resource = self.translate(resource_expression, None)?;
        if let Some(foreach) =
            self.transform_to_foreach(variable, resource_expression, &resource, body, is_async)?
        {
            return Ok(self.ast.with_il(foreach, inst));
        }
        // `IsValidInCSharp`; else the managed builder writes the try/finally out.
        let disposable_code = if is_async {
            KnownTypeCode::IAsyncDisposable
        } else {
            KnownTypeCode::IDisposable
        };
        let v = self.f.var(variable);
        let (ty, used) = (v.ty, v.load_count() > 0 || v.address_count() > 0);
        if !matches!(self.f.inst(resource_expression), Inst::LdNull) && !is_ref_struct {
            let underlying = self.ts.nullable_underlying(ty);
            let disposable = self
                .ts
                .all_base_type_definitions(underlying)
                .into_iter()
                .any(|d| {
                    self.ts
                        .comp
                        .type_def(d)
                        .is_ok_and(|i| i.known == disposable_code)
                });
            // `IsValidInCSharp` fails: the store, then `try`/`finally` disposing through a new
            // variable of the interface type, which the host writes out and registers.
            if !disposable {
                let store = self.ast.add(AstNode::UsingExpanded {
                    variable,
                    part: resource.expression,
                    is_store: true,
                    is_async,
                });
                let body = self.convert_as_block(body)?;
                let try_finally = self.ast.add(AstNode::UsingExpanded {
                    variable,
                    part: body,
                    is_store: false,
                    is_async,
                });
                // A block, which the enclosing block takes the statements of.
                let s = self.ast.add(AstNode::Block {
                    statements: vec![store, try_finally],
                });
                return Ok(self.ast.with_il(s, inst));
            }
        }
        let (declaration, resource) = if used {
            // `var` for anonymous types is not translated.
            if self.may_contain_anonymous_type(ty) {
                return Err("a using declaration of an anonymous type".to_owned());
            }
            (Some(variable), self.convert_to(resource, ty, true)?)
        } else {
            (None, resource)
        };
        let body = self.convert_as_block(body)?;
        let s = self.ast.add(AstNode::Using {
            declaration,
            resource: resource.expression,
            body,
            is_async,
        });
        Ok(self.ast.with_il(s, inst))
    }

    /// `VisitIfInstruction`: `if (condition) true_inst else false_inst`, without `else` for a
    /// `nop`.
    fn visit_if(
        &mut self,
        inst: NodeId,
        condition: NodeId,
        true_inst: NodeId,
        false_inst: NodeId,
    ) -> Result<AstId, Unsupported> {
        let condition = self.translate_condition(condition)?;
        let true_statement = self.convert(true_inst)?;
        let false_statement = if matches!(self.f.inst(false_inst), Inst::Nop { .. }) {
            None
        } else {
            Some(self.convert(false_inst)?)
        };
        let s = self.ast.add(AstNode::IfElse {
            condition: condition.expression,
            true_statement,
            false_statement,
        });
        Ok(self.ast.with_il(s, inst))
    }

    /// `VisitBlock` for a block without container: its instructions as a block statement.
    fn visit_block(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<AstId, Unsupported> {
        let mut statements = Vec::with_capacity(instructions.len() + 1);
        let mut i = 0;
        while i < instructions.len() {
            if let Some(foreach) = self.transform_to_foreach_without_dispose(instructions, i)? {
                statements.push(foreach);
                i += 2;
                continue;
            }
            statements.push(self.convert(instructions[i])?);
            i += 1;
        }
        if !matches!(self.f.inst(final_inst), Inst::Nop { .. }) {
            statements.push(self.convert(final_inst)?);
        }
        let s = self.ast.add(AstNode::Block { statements });
        Ok(self.ast.with_il(s, inst))
    }

    /// `new ExpressionStatement(exprBuilder.Translate(inst)).WithILInstruction(inst)`.
    fn expression_statement(&mut self, inst: NodeId) -> Result<AstId, Unsupported> {
        let e = self.translate(inst, None)?;
        let s = self.ast.add(AstNode::ExpressionStatement {
            expression: e.expression,
        });
        Ok(self.ast.with_il(s, inst))
    }

    fn visit_block_container(
        &mut self,
        container: NodeId,
        kind: ContainerKind,
        blocks: &[NodeId],
    ) -> Result<AstId, Unsupported> {
        let entry = *blocks.first().ok_or("an empty container")?;
        if kind != ContainerKind::Normal && self.f.node(entry).incoming_edge_count > 1 {
            let saved = (self.continue_target, self.continue_count, self.break_target);
            let converted = self.convert_loop(container, kind, blocks);
            (self.continue_target, self.continue_count, self.break_target) = saved;
            let l = converted?;
            // The container as annotation, then as instruction.
            self.ast.with_il(l, container);
            return Ok(self.ast.with_il(l, container));
        }
        if let Inst::Block { instructions, .. } = self.f.inst(entry)
            && instructions.len() == 1
            && matches!(self.f.inst(instructions[0]), Inst::Switch { .. })
        {
            let switch = instructions[0];
            return self.translate_switch(container, blocks, switch);
        }
        let block = self.convert_block_container(container, blocks, false)?;
        self.declare_local_functions(container, block)?;
        if container == self.return_container && self.emit_as_ref_readonly {
            let helper = self.ast.add(AstNode::AsRefReadOnlyHelper);
            if let AstNode::Block { statements } = self.ast.node_mut(block) {
                statements.push(helper);
            }
        }
        Ok(self.ast.with_il(block, container))
    }

    /// `ConvertBlockContainer(new BlockStatement(), container, blocks, isLoop)`.
    fn convert_block_container(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        is_loop: bool,
    ) -> Result<AstId, Unsupported> {
        let block_statement = self.ast.add(AstNode::Block {
            statements: Vec::new(),
        });
        self.convert_block_container_into(block_statement, container, blocks, is_loop)
    }

    /// `ConvertBlockContainer(blockStatement, container, blocks, isLoop)`: the statements of
    /// `blocks` appended to `block_statement`.
    fn convert_block_container_into(
        &mut self,
        block_statement: AstId,
        container: NodeId,
        blocks: &[NodeId],
        is_loop: bool,
    ) -> Result<AstId, Unsupported> {
        let entry = match self.f.inst(container) {
            Inst::BlockContainer { blocks, .. } => blocks.first().copied(),
            _ => None,
        };
        for (index, &block) in blocks.iter().enumerate() {
            // A block with incoming branches, or other than the entry point, gets a label.
            if self.f.node(block).incoming_edge_count > 1 || Some(block) != entry {
                let name = self.ensure_unique_label(block);
                let label = self.ast.add(AstNode::Label { name });
                let AstNode::Block { statements } = self.ast.node_mut(block_statement) else {
                    unreachable!("a block statement");
                };
                statements.push(label);
            }
            let Inst::Block {
                instructions,
                final_inst,
                ..
            } = self.f.inst(block).clone()
            else {
                return Err("a container without blocks".to_owned());
            };
            let mut i = 0;
            while i < instructions.len() {
                let inst = instructions[i];
                i += 1;
                if !is_loop
                    && let Inst::Leave { .. } = self.f.inst(inst)
                    && self.is_final_leave(
                        inst,
                        i - 1,
                        &instructions,
                        final_inst,
                        index,
                        blocks,
                        container,
                    )
                {
                    // the final `leave` falls out of the block
                    self.ast.annotations_mut(block_statement).implicit_return = Some(inst);
                    continue;
                }
                let stmt = match self.transform_to_foreach_without_dispose(&instructions, i - 1)? {
                    Some(foreach) => {
                        i += 1;
                        foreach
                    }
                    None => self.convert(inst)?,
                };
                let nested = match self.ast.node(stmt) {
                    AstNode::Block { statements } => Some(statements.clone()),
                    _ => None,
                };
                let AstNode::Block { statements } = self.ast.node_mut(block_statement) else {
                    unreachable!("a block statement");
                };
                match nested {
                    Some(nested) => statements.extend(nested),
                    None => statements.push(stmt),
                }
            }
            if !matches!(self.f.inst(final_inst), Inst::Nop { .. }) {
                let stmt = self.convert(final_inst)?;
                let AstNode::Block { statements } = self.ast.node_mut(block_statement) else {
                    unreachable!("a block statement");
                };
                statements.push(stmt);
            }
        }
        // The label of the leaves out of the container, after its blocks; a loop goes on with
        // `continue` before it and ends with `break` after it.
        if let Some(label) = self
            .end_container_labels
            .iter()
            .find(|(c, _)| *c == container)
            .map(|(_, l)| l.clone())
        {
            let last_is_continue = matches!(self.ast.node(block_statement), AstNode::Block { statements } if statements.last().is_some_and(|&s| matches!(self.ast.node(s), AstNode::Continue)));
            let mut added = Vec::new();
            if is_loop && !last_is_continue {
                added.push(self.ast.add(AstNode::Continue));
            }
            added.push(self.ast.add(AstNode::Label { name: label }));
            if is_loop {
                added.push(self.ast.add(AstNode::Break));
            }
            let AstNode::Block { statements } = self.ast.node_mut(block_statement) else {
                unreachable!("a block statement");
            };
            statements.extend(added);
        }
        Ok(block_statement)
    }

    /// `VisitLeave` of a leave out of a nested container: `continue` where the container is
    /// the try block of a block that branches to the continue target next, else `goto` the
    /// label at the end of the container, `end_` and its entry point's label (counted with
    /// the block labels where it repeats).
    fn leave_to_label(&mut self, inst: NodeId, target: NodeId) -> AstId {
        if let Some(continue_target) = self.continue_target
            && let Some(try_inst) = self.f.node(target).parent
            && matches!(
                self.f.inst(try_inst),
                Inst::TryCatch { .. } | Inst::TryFinally { .. } | Inst::TryFault { .. }
            )
            && let Some(block) = self.f.node(try_inst).parent
            && let Inst::Block { instructions, .. } = self.f.inst(block)
            && instructions.last().is_some_and(|&l| {
                matches!(*self.f.inst(l), Inst::Branch { target: BranchTarget::Block(t) } if t == continue_target)
            })
        {
            let s = self.ast.add(AstNode::Continue);
            return self.ast.with_il(s, inst);
        }
        let label = match self.end_container_labels.iter().find(|(c, _)| *c == target) {
            Some((_, label)) => label.clone(),
            None => {
                let entry = match self.f.inst(target) {
                    Inst::BlockContainer { blocks, .. } => blocks.first().copied(),
                    _ => None,
                };
                let target_label = entry.map_or(String::new(), |b| {
                    format!("IL_{:04x}", self.f.il_range(b).start)
                });
                let mut label = format!("end_{target_label}");
                match self.duplicate_labels.get_mut(&label) {
                    None => {
                        self.duplicate_labels.insert(label.clone(), 1);
                    }
                    Some(count) => {
                        *count += 1;
                        label = format!("{label}_{count}");
                    }
                }
                self.end_container_labels.push((target, label.clone()));
                label
            }
        };
        let s = self.ast.add(AstNode::Goto { name: label });
        self.ast.with_il(s, inst)
    }

    /// `EnsureUniqueLabel(block)`: `Block.Label`, with a counter where blocks share it.
    pub(super) fn ensure_unique_label(&mut self, block: NodeId) -> String {
        if let Some((_, label)) = self.labels.iter().find(|(b, _)| *b == block) {
            return label.clone();
        }
        let base = format!("IL_{:04x}", self.f.il_range(block).start);
        let label = match self.duplicate_labels.get_mut(&base) {
            None => {
                self.duplicate_labels.insert(base.clone(), 1);
                base
            }
            Some(count) => {
                *count += 1;
                format!("{base}_{count}")
            }
        };
        self.labels.push((block, label.clone()));
        label
    }

    /// `ConvertLoop` for `while (true)` and `while (condition)` loops whose jumps are all
    /// `break` and `continue`, so that no label is written.
    fn convert_loop(
        &mut self,
        container: NodeId,
        kind: ContainerKind,
        blocks: &[NodeId],
    ) -> Result<AstId, Unsupported> {
        self.continue_count = 0;
        self.break_target = Some(container);
        let entry = blocks[0];
        let incoming = self.f.node(entry).incoming_edge_count;
        match kind {
            ContainerKind::Loop => {
                self.continue_target = Some(entry);
                let body = self.convert_block_container(container, blocks, true)?;
                // The entry's label goes when every jump to it became `continue`.
                if incoming == self.continue_count + 1 {
                    self.remove_first_statement(body);
                }
                self.remove_trailing_continue(body);
                self.declare_local_functions(container, body)?;
                Ok(self.ast.add(AstNode::While {
                    condition: None,
                    body,
                }))
            }
            ContainerKind::While => {
                self.continue_target = Some(entry);
                let (condition, body_block) = self
                    .match_condition_block(container, entry)
                    .ok_or("an invalid condition block in a while loop")?;
                let body = self.convert_as_block(body_block)?;
                if !self
                    .f
                    .has_flag(body_block, crate::il::ast::Flags::END_POINT_UNREACHABLE)
                {
                    let b = self.ast.add(AstNode::Break);
                    if let AstNode::Block { statements } = self.ast.node_mut(body) {
                        statements.push(b);
                    }
                }
                let rest: Vec<_> = blocks[1..]
                    .iter()
                    .copied()
                    .filter(|&b| b != body_block)
                    .collect();
                let body = self.convert_block_container_into(body, container, &rest, true)?;
                // An incoming edge of the condition that is no `continue` jumps to a label.
                if self.continue_count + 1 < incoming {
                    self.remove_trailing_continue(body);
                    let name = self.ensure_unique_label(entry);
                    let label = self.ast.add(AstNode::Label { name });
                    if let AstNode::Block { statements } = self.ast.node_mut(body) {
                        statements.push(label);
                    }
                }
                self.remove_trailing_continue(body);
                self.declare_local_functions(container, body)?;
                let condition = self.translate_condition(condition)?.expression;
                Ok(self.ast.add(AstNode::While {
                    condition: Some(condition),
                    body,
                }))
            }
            ContainerKind::DoWhile => {
                let condition_block = *blocks.last().ok_or("an empty do-while loop")?;
                self.continue_target = Some(condition_block);
                let (condition, _) = self
                    .match_condition_block(container, condition_block)
                    .ok_or("an invalid condition block in a do-while loop")?;
                let body =
                    self.convert_block_container(container, &blocks[..blocks.len() - 1], true)?;
                // The entry's label goes when only the loop's own two edges reach it.
                if incoming == 2 {
                    self.remove_first_statement(body);
                }
                self.remove_trailing_continue(body);
                // Branches to the condition block that are no `continue` jump to a label.
                if self.f.node(condition_block).incoming_edge_count > self.continue_count {
                    let name = self.ensure_unique_label(condition_block);
                    let label = self.ast.add(AstNode::Label { name });
                    if let AstNode::Block { statements } = self.ast.node_mut(body) {
                        statements.push(label);
                    }
                }
                self.declare_local_functions(container, body)?;
                let condition = self.translate_condition(condition)?.expression;
                let empty = matches!(self.ast.node(body), AstNode::Block { statements } if statements.is_empty());
                Ok(self.ast.add(if empty {
                    AstNode::While {
                        condition: Some(condition),
                        body,
                    }
                } else {
                    AstNode::DoWhile { body, condition }
                }))
            }
            ContainerKind::For => {
                let increment = *blocks.last().ok_or("an empty for loop")?;
                self.continue_target = Some(increment);
                let (condition, body_block) = self
                    .match_condition_block(container, entry)
                    .ok_or("an invalid condition block in a for loop")?;
                let body = self.convert_as_block(body_block)?;
                if !self
                    .f
                    .has_flag(body_block, crate::il::ast::Flags::END_POINT_UNREACHABLE)
                {
                    let b = self.ast.add(AstNode::Break);
                    if let AstNode::Block { statements } = self.ast.node_mut(body) {
                        statements.push(b);
                    }
                }
                // `MatchIncrementBlock`: the increments end in the branch back to the condition.
                let Inst::Block { instructions, .. } = self.f.inst(increment).clone() else {
                    return Err("an invalid increment block in a for loop".to_owned());
                };
                let ends_in_branch = instructions.last().is_some_and(|&l| {
                    matches!(
                        *self.f.inst(l),
                        Inst::Branch { target: BranchTarget::Block(t) } if t == entry
                    )
                });
                if !ends_in_branch {
                    return Err("an invalid increment block in a for loop".to_owned());
                }
                if blocks.len() != 3 || blocks[1] != body_block {
                    return Err("a for loop with labeled blocks".to_owned());
                }
                let condition = self.translate_condition(condition)?.expression;
                self.remove_trailing_continue(body);
                let mut iterators = Vec::with_capacity(instructions.len() - 1);
                for &i in &instructions[..instructions.len() - 1] {
                    iterators.push(self.convert(i)?);
                }
                if self.f.node(increment).incoming_edge_count > self.continue_count {
                    return Err("a for loop with a jump to its increment".to_owned());
                }
                self.declare_local_functions(container, body)?;
                Ok(self.ast.add(AstNode::For {
                    condition,
                    iterators,
                    body,
                }))
            }
            _ => Err(format!("a {} container", kind.name())),
        }
    }

    /// `blockStatement.Statements.First().Remove()`: the label of a loop's entry.
    fn remove_first_statement(&mut self, block: AstId) {
        if let AstNode::Block { statements } = self.ast.node_mut(block)
            && !statements.is_empty()
        {
            statements.remove(0);
        }
    }

    /// `if (blockStatement.LastOrDefault() is ContinueStatement continueStmt) continueStmt.Remove()`
    fn remove_trailing_continue(&mut self, block: AstId) {
        let last = match self.ast.node(block) {
            AstNode::Block { statements } => statements.last().copied(),
            _ => None,
        };
        if last.is_some_and(|s| matches!(self.ast.node(s), AstNode::Continue))
            && let AstNode::Block { statements } = self.ast.node_mut(block)
        {
            statements.pop();
        }
    }

    /// `BlockContainer.MatchConditionBlock`: a block of one `if (condition) br body; else
    /// leave container`, as the condition and the body's block.
    pub(super) fn match_condition_block(
        &self,
        container: NodeId,
        block: NodeId,
    ) -> Option<(NodeId, NodeId)> {
        let Inst::Block { instructions, .. } = self.f.inst(block) else {
            return None;
        };
        let [only] = instructions[..] else {
            return None;
        };
        let Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } = *self.f.inst(only)
        else {
            return None;
        };
        let Inst::Leave { target, value } = *self.f.inst(false_inst) else {
            return None;
        };
        if target != Some(container) || !matches!(self.f.inst(value), Inst::Nop { .. }) {
            return None;
        }
        let Inst::Branch {
            target: BranchTarget::Block(body),
        } = *self.f.inst(true_inst)
        else {
            return None;
        };
        Some((condition, body))
    }

    /// `IsFinalLeave`: a `leave` without value that ends the last block of its target.
    #[allow(clippy::too_many_arguments)]
    fn is_final_leave(
        &self,
        leave: NodeId,
        index: usize,
        instructions: &[NodeId],
        final_inst: NodeId,
        block_index: usize,
        blocks: &[NodeId],
        container: NodeId,
    ) -> bool {
        let Inst::Leave { target, value } = self.f.inst(leave) else {
            return false;
        };
        matches!(self.f.inst(*value), Inst::Nop { .. })
            && index == instructions.len() - 1
            && matches!(self.f.inst(final_inst), Inst::Nop { .. })
            && block_index == blocks.len() - 1
            && *target == Some(container)
    }

    /// `VisitLeave` for a `leave` out of the function: `return` with the value converted to
    /// the result type.
    fn visit_leave(
        &mut self,
        inst: NodeId,
        target: Option<NodeId>,
        value: NodeId,
    ) -> Result<AstId, Unsupported> {
        if target.is_some() && target == self.break_target {
            let s = self.ast.add(AstNode::Break);
            return Ok(self.ast.with_il(s, inst));
        }
        if target != Some(self.return_container) {
            let target = target.ok_or("a leave without target")?;
            return Ok(self.leave_to_label(inst, target));
        }
        if self.is_iterator {
            let s = self.ast.add(AstNode::YieldBreak);
            return Ok(self.ast.with_il(s, inst));
        }
        let expression = if matches!(self.f.inst(value), Inst::Nop { .. }) {
            None
        } else {
            let result_type = self.result_type;
            let e = self.translate(value, Some(result_type))?;
            let e = self.convert_to(e, result_type, true)?;
            // A lambda's return keeps the result type with an identity cast (`IdentityConversion`,
            // as the host's `ResolveCast` gives it), lest the lambda infer another type.
            if self.in_lambda && self.possible_loss_of_type_information(e.ty, result_type) {
                let c = self.ast.add(AstNode::Cast {
                    ty: result_type,
                    expression: e.expression,
                    overflow: Overflow::None,
                });
                self.ast.with_rr(c, Rr::Conversion { ty: result_type });
                Some(c)
            } else {
                Some(e.expression)
            }
        };
        let s = self.ast.add(AstNode::Return { expression });
        self.return_statements.push(s);
        Ok(self.ast.with_il(s, inst))
    }
}
