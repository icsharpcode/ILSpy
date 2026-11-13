//! `StatementBuilder`: the C# statements of an ILAst function body.

use crate::il::ast::{
    BlockKind, BranchTarget, ContainerKind, Function, FunctionKind, Inst, NodeId,
};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::ast::{Ast, AstId, AstNode, Rr};
use super::{Builder, Unsupported};

/// `new StatementBuilder(...).ConvertAsBlock(function.Body)` for a top-level function: the
/// body as a `BlockStatement` and the function as the translation left it (a copy of
/// `read.function`), or why it is not translated.
pub fn convert_body(
    ts: &mut TypeSystem<'_, '_>,
    read: &ReadFunction,
    settings: crate::il::transforms::Settings,
    resolver: Option<&dyn crate::il::transforms::Resolver>,
) -> Result<(Ast, AstId, Function), Unsupported> {
    let mut function = read.function.clone();
    let f = &mut function;
    let body = f.body.ok_or("a function without body")?;
    if read.kind != FunctionKind::TopLevelFunction {
        return Err("a nested function".to_owned());
    }
    if !f.local_functions.is_empty() {
        return Err("a function with local functions".to_owned());
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
        resolver,
        current_type,
        variable_names,
        continue_target: None,
        continue_count: 0,
        break_target: None,
        return_statements: Vec::new(),
        is_iterator: read.is_iterator,
        in_lambda: false,
        root_variables: read.variables.clone(),
        found_override: None,
        loaded_stack_slots: Vec::new(),
        named_argument_map: None,
    };
    let stmt = b.convert(body)?;
    b.ast.with_il(stmt, body);
    // A resolve result inside another one is read without a node of its own, so it cannot
    // be a cast's, which the host computes from the cast's node.
    let nested_cast = b.ast.nodes.iter().any(|(_, a)| match &a.rr {
        Some(
            Rr::Field {
                target: Some(inner),
                ..
            }
            | Rr::Event { target: inner, .. }
            | Rr::TupleElement { target: inner, .. }
            | Rr::ByReference(inner)
            | Rr::ByReferenceOut(inner)
            | Rr::ArrayLength { target: inner, .. }
            | Rr::Property { target: inner, .. },
        ) => matches!(
            **inner,
            Rr::Conversion { .. } | Rr::ImplicitConversion { .. } | Rr::TryCast { .. }
        ),
        _ => false,
    });
    if nested_cast {
        return Err("a cast as the target of a member or reference".to_owned());
    }
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
            // `VisitStLoc` and `VisitStObj` strip a top-level `ref` of a ref reassignment, which
            // the expression builder does not produce.
            Inst::StLoc { .. } | Inst::StObj { .. } => self.expression_statement(inst),
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
            // `VisitBranch`: `continue` for the loop's continue target; a goto is not translated.
            Inst::Branch {
                target: BranchTarget::Block(target),
            } if Some(target) == self.continue_target => {
                self.continue_count += 1;
                let s = self.ast.add(AstNode::Continue);
                Ok(self.ast.with_il(s, inst))
            }
            Inst::Switch { .. }
            | Inst::Branch { .. }
            | Inst::TryFault { .. }
            | Inst::PinnedRegion { .. }
            | Inst::Initblk { .. }
            | Inst::Cpblk { .. }
            | Inst::Ckfinite { .. }
            | Inst::IsInst { .. } => Err(format!(
                "the statement {}",
                super::inst_name(self.f.inst(inst))
            )),
            // `Default`: an expression statement.
            _ => {
                let s = self.expression_statement(inst)?;
                let AstNode::ExpressionStatement { expression } = *self.ast.node(s) else {
                    unreachable!("an expression statement");
                };
                if matches!(self.ast.node(expression), AstNode::RefDirection { .. }) {
                    return Err("a ref expression as statement".to_owned());
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

    /// `VisitUsingInstruction` for a synchronous `using` of a resource whose type implements
    /// `IDisposable`, declared where the body uses the variable.
    fn visit_using(
        &mut self,
        inst: NodeId,
        variable: crate::il::ast::VarId,
        resource_expression: NodeId,
        body: NodeId,
        is_async: bool,
        is_ref_struct: bool,
    ) -> Result<AstId, Unsupported> {
        if is_async {
            return Err("an await using".to_owned());
        }
        let resource = self.translate(resource_expression, None)?;
        if let Some(foreach) =
            self.transform_to_foreach(variable, resource_expression, &resource, body)?
        {
            return Ok(self.ast.with_il(foreach, inst));
        }
        // `IsValidInCSharp`; else the managed builder writes the try/finally out.
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
                        .is_ok_and(|i| i.known == KnownTypeCode::IDisposable)
                });
            if !disposable {
                return Err("a using of a type that does not implement IDisposable".to_owned());
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
        Ok(self.ast.with_il(block, container))
    }

    /// `ConvertBlockContainer(container, isLoop)` for a container whose blocks need no labels;
    /// a loop's entry gets one that `ConvertLoop` checks and removes.
    fn convert_block_container(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        is_loop: bool,
    ) -> Result<AstId, Unsupported> {
        let block_statement = self.ast.add(AstNode::Block {
            statements: Vec::new(),
        });
        for (index, &block) in blocks.iter().enumerate() {
            if index != 0 || (self.f.node(block).incoming_edge_count > 1 && !is_loop) {
                return Err("a labeled block".to_owned());
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
        Ok(block_statement)
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
                if incoming != self.continue_count + 1 {
                    return Err("a loop with a jump to its start".to_owned());
                }
                self.remove_trailing_continue(body);
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
                if blocks.len() != 2 || blocks[1] != body_block {
                    return Err("a while loop with labeled blocks".to_owned());
                }
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
                if self.continue_count + 1 < incoming {
                    return Err("a while loop with a jump to its condition".to_owned());
                }
                self.remove_trailing_continue(body);
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
                if blocks.len() != 2 {
                    return Err("a do-while loop with labeled blocks".to_owned());
                }
                let body = self.convert_block_container(container, &blocks[..1], true)?;
                // The entry's label goes when only the loop's own two edges reach it.
                if incoming != 2 {
                    return Err("a do-while loop with a jump to its start".to_owned());
                }
                self.remove_trailing_continue(body);
                if self.f.node(condition_block).incoming_edge_count > self.continue_count {
                    return Err("a do-while loop with a jump to its condition".to_owned());
                }
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
                Ok(self.ast.add(AstNode::For {
                    condition,
                    iterators,
                    body,
                }))
            }
            _ => Err(format!("a {} container", kind.name())),
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
            return Err("a leave out of a nested container".to_owned());
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
            if self.in_lambda && self.possible_loss_of_type_information(e.ty, result_type) {
                return Err("a lambda return cast to keep its type".to_owned());
            }
            Some(e.expression)
        };
        let s = self.ast.add(AstNode::Return { expression });
        self.return_statements.push(s);
        Ok(self.ast.with_il(s, inst))
    }
}
