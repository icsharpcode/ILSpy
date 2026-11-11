//! `StatementBuilder`: the C# statements of an ILAst function body.

use crate::il::ast::{BlockKind, ContainerKind, FunctionKind, Inst, NodeId};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeSystem;

use super::ast::{Ast, AstId, AstNode, Rr};
use super::{Builder, Unsupported};

/// `new StatementBuilder(...).ConvertAsBlock(function.Body)` for a top-level function: the
/// body as a `BlockStatement`, or why it is not translated.
pub fn convert_body(
    ts: &mut TypeSystem<'_, '_>,
    read: &ReadFunction,
    settings: crate::il::transforms::Settings,
    resolver: Option<&dyn crate::il::transforms::Resolver>,
) -> Result<(Ast, AstId), Unsupported> {
    let f = &read.function;
    let body = f.body.ok_or("a function without body")?;
    if read.kind != FunctionKind::TopLevelFunction {
        return Err("a nested function".to_owned());
    }
    if read.is_iterator {
        return Err("an iterator".to_owned());
    }
    if read.async_return_type.is_some() {
        return Err("an async function".to_owned());
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
    let mut b = Builder {
        f,
        ts,
        ast: Ast::default(),
        return_container: body,
        result_type: method.ret,
        settings,
        resolver,
        current_type,
        variable_names: read
            .variables
            .iter()
            .map(|&v| f.var(v).name.clone())
            .collect(),
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
            | Rr::ByReference(inner)
            | Rr::ByReferenceOut(inner)
            | Rr::ArrayLength { target: inner, .. }
            | Rr::Invocation { target: inner, .. }
            | Rr::Property { target: inner, .. },
        ) => matches!(**inner, Rr::Conversion { .. }),
        _ => false,
    });
    if nested_cast {
        return Err("a cast as the target of a member or reference".to_owned());
    }
    // `stmt as BlockStatement ?? new BlockStatement { stmt }`: a container is a block.
    Ok((b.ast, stmt))
}

impl Builder<'_, '_, '_, '_> {
    /// `Convert(inst)`.
    fn convert(&mut self, inst: NodeId) -> Result<AstId, Unsupported> {
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
            Inst::Switch { .. }
            | Inst::Branch { .. }
            | Inst::YieldReturn { .. }
            | Inst::TryFault { .. }
            | Inst::Using { .. }
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
    fn convert_as_block(&mut self, inst: NodeId) -> Result<AstId, Unsupported> {
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
        for (i, &s) in instructions.iter().enumerate() {
            // `TransformToForeachWithoutDispose` looks for a store followed by a while loop.
            if matches!(self.f.inst(s), Inst::StLoc { .. })
                && instructions.get(i + 1).is_some_and(|&n| {
                    matches!(
                        self.f.inst(n),
                        Inst::BlockContainer {
                            kind: ContainerKind::While,
                            ..
                        }
                    )
                })
            {
                return Err("a foreach without Dispose".to_owned());
            }
            statements.push(self.convert(s)?);
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
        if kind != ContainerKind::Normal {
            return Err(format!("a {} container", kind.name()));
        }
        if let Inst::Block { instructions, .. } = self.f.inst(entry)
            && instructions.len() == 1
            && matches!(self.f.inst(instructions[0]), Inst::Switch { .. })
        {
            return Err("a switch".to_owned());
        }
        let block = self.convert_block_container(container, blocks)?;
        Ok(self.ast.with_il(block, container))
    }

    /// `ConvertBlockContainer(container, isLoop: false)`.
    fn convert_block_container(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
    ) -> Result<AstId, Unsupported> {
        let block_statement = self.ast.add(AstNode::Block {
            statements: Vec::new(),
        });
        for (index, &block) in blocks.iter().enumerate() {
            if self.f.node(block).incoming_edge_count > 1 || index != 0 {
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
            for (i, &inst) in instructions.iter().enumerate() {
                if let Inst::Leave { .. } = self.f.inst(inst)
                    && self.is_final_leave(
                        inst,
                        i,
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
                let stmt = self.convert(inst)?;
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
        if target != Some(self.return_container) {
            return Err("a leave out of a nested container".to_owned());
        }
        let expression = if matches!(self.f.inst(value), Inst::Nop { .. }) {
            None
        } else {
            let result_type = self.result_type;
            let e = self.translate(value, Some(result_type))?;
            Some(self.convert_to(e, result_type, true)?.expression)
        };
        let s = self.ast.add(AstNode::Return { expression });
        Ok(self.ast.with_il(s, inst))
    }
}
