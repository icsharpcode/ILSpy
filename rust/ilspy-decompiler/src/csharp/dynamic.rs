//! The dynamic operations of `ExpressionBuilder`: `VisitDynamic*Instruction` and
//! `VisitDynamicCompoundAssign`, with `TranslateDynamicTarget`, `TranslateDynamicArgument` and
//! the symbols `CreateDynamic*Symbol` synthesizes, which the host builds from the resolve
//! results' data.

use crate::il::ast::{CompoundTargetKind, DynamicArgInfo, DynamicKind, Inst, NodeId};
use crate::il::transforms::dynamic_call_site::match_get_type_from_handle;
use crate::il::types::StackType;
use crate::semantics::types::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstNode, BinaryOp, DynamicSymbol, Overflow, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `CSharpArgumentInfoFlags`.
const USE_COMPILE_TIME_TYPE: i32 = 0x1;
const CONSTANT: i32 = 0x2;
const NAMED_ARGUMENT: i32 = 0x4;
const IS_REF: i32 = 0x8;
const IS_OUT: i32 = 0x10;
const IS_STATIC_TYPE: i32 = 0x20;

/// `CSharpBinderFlags`.
const CHECKED_CONTEXT: i32 = 0x1;
const INVOKE_SIMPLE_NAME: i32 = 0x2;
const CONVERT_EXPLICIT: i32 = 0x10;

/// `UnaryOperatorType` values.
const UNARY_NOT: u8 = 1;
const UNARY_BIT_NOT: u8 = 2;
const UNARY_MINUS: u8 = 3;
const UNARY_PLUS: u8 = 4;
const UNARY_INCREMENT: u8 = 5;
const UNARY_DECREMENT: u8 = 6;
const UNARY_IS_TRUE: u8 = 14;

impl Builder<'_, '_, '_, '_> {
    /// `VisitDynamic*Instruction` of `inst`.
    pub(crate) fn dynamic(&mut self, inst: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Dynamic {
            kind,
            binder_flags,
            name,
            operation,
            ty,
            type_arguments,
            arg_infos,
            args,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a dynamic instruction");
        };
        let dynamic = self.ts.intern(TypeData::Dynamic);
        let checked = binder_flags & CHECKED_CONTEXT != 0;
        let arg = |i: usize| -> Result<NodeId, Unsupported> {
            args.get(i)
                .copied()
                .ok_or_else(|| "a dynamic instruction without its arguments".to_owned())
        };
        let info = |i: usize| -> Result<DynamicArgInfo, Unsupported> {
            arg_infos
                .get(i)
                .cloned()
                .ok_or_else(|| "a dynamic instruction without its argument infos".to_owned())
        };
        match kind {
            // `VisitDynamicConvertInstruction`
            DynamicKind::Convert => {
                let ty = ty.ok_or("a dynamic conversion without its type")?;
                let operand = self.translate(arg(0)?, None)?;
                let operand = self.convert_to(operand, dynamic, false)?;
                let c = self.ast.add(AstNode::Cast {
                    ty,
                    expression: operand.expression,
                    overflow: if checked {
                        Overflow::Checked
                    } else {
                        Overflow::Unchecked
                    },
                });
                self.ast.with_il(c, inst);
                self.ast.with_rr(
                    c,
                    Rr::DynamicConversion {
                        ty,
                        explicit: binder_flags & CONVERT_EXPLICIT != 0,
                    },
                );
                Ok(TranslatedExpression { expression: c, ty })
            }
            // `VisitDynamicGetIndexInstruction`
            DynamicKind::GetIndex => {
                let target = self.translate_dynamic_target(arg(0)?, &info(0)?)?;
                let arguments = self
                    .translate_dynamic_arguments(&args[1..], arg_infos.get(1..).unwrap_or(&[]))?;
                let symbol = DynamicSymbol::Indexer {
                    declaring: self.dynamic_argument_type(&info(0)?),
                    parameters: self.dynamic_parameters(arg_infos.get(1..).unwrap_or(&[])),
                };
                let target_rr = self.rr_of(target.expression);
                let e = self.ast.add(AstNode::Indexer {
                    target: target.expression,
                    indices: arguments.iter().map(|a| a.expression).collect(),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(
                    e,
                    Rr::DynamicInvocation {
                        target: Box::new(target_rr),
                        indexing: true,
                        symbol: Some(Box::new(symbol)),
                    },
                );
                Ok(TranslatedExpression {
                    expression: e,
                    ty: dynamic,
                })
            }
            // `VisitDynamicGetMemberInstruction`
            DynamicKind::GetMember => {
                let name = name.ok_or("a dynamic member access without its name")?;
                let target_info = info(0)?;
                let target = self.translate_dynamic_target(arg(0)?, &target_info)?;
                let e = self.dynamic_member(target, name, &target_info);
                // `WithILInstruction` before `WithRR`.
                self.ast.annotations_mut(e.expression).il_before_rr = true;
                self.ast.with_il(e.expression, inst);
                Ok(e)
            }
            // `VisitDynamicInvokeConstructorInstruction`
            DynamicKind::InvokeConstructor => {
                let ty = ty.ok_or("a dynamic object creation without its type")?;
                let infos = arg_infos.get(1..).unwrap_or(&[]);
                let arguments = self.translate_dynamic_arguments(&args, infos)?;
                let parameters = self.dynamic_parameters(infos);
                let e = self.ast.add(AstNode::ObjectCreate {
                    ty,
                    arguments: arguments.iter().map(|a| a.expression).collect(),
                    initializer: None,
                });
                self.ast.with_il(e, inst);
                self.ast
                    .with_rr(e, Rr::DynamicConstructor { ty, parameters });
                Ok(TranslatedExpression { expression: e, ty })
            }
            // `VisitDynamicInvokeMemberInstruction`
            DynamicKind::InvokeMember => {
                let name = name.ok_or("a dynamic invocation without its name")?;
                let type_arguments = type_arguments.unwrap_or_default();
                let target = match ty {
                    Some(static_target) => {
                        let t = self.ast.add(AstNode::TypeReference { ty: static_target });
                        self.ast.with_rr(t, Rr::Type(static_target));
                        TranslatedExpression {
                            expression: t,
                            ty: static_target,
                        }
                    }
                    None => self.translate_dynamic_target(arg(0)?, &info(0)?)?,
                };
                let target_expression = if binder_flags & INVOKE_SIMPLE_NAME != 0
                    && matches!(self.ast.node(target.expression), AstNode::This)
                {
                    if type_arguments.is_empty() {
                        self.ast.add(AstNode::Identifier { name: name.clone() })
                    } else {
                        self.ast.add(AstNode::GenericIdentifier {
                            name: name.clone(),
                            type_arguments: type_arguments.clone(),
                        })
                    }
                } else if type_arguments.is_empty() {
                    self.ast.add(AstNode::MemberReference {
                        target: target.expression,
                        name: name.clone(),
                    })
                } else {
                    self.ast.add(AstNode::GenericMemberReference {
                        target: target.expression,
                        name: name.clone(),
                        type_arguments: type_arguments.clone(),
                    })
                };
                let values = if ty.is_some() {
                    &args[..]
                } else {
                    args.get(1..).unwrap_or(&[])
                };
                let infos = arg_infos.get(1..).unwrap_or(&[]);
                let arguments = self.translate_dynamic_arguments(values, infos)?;
                let declaring = match ty {
                    Some(t) => t,
                    None => self.dynamic_argument_type(&info(0)?),
                };
                let symbol = DynamicSymbol::Method {
                    name,
                    declaring,
                    parameters: self.dynamic_parameters(infos),
                    type_arguments,
                };
                let target_rr = self.rr_of(target.expression);
                let e = self.ast.add(AstNode::Invocation {
                    target: target_expression,
                    arguments: arguments.iter().map(|a| a.expression).collect(),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(
                    e,
                    Rr::DynamicInvocation {
                        target: Box::new(target_rr),
                        indexing: false,
                        symbol: Some(Box::new(symbol)),
                    },
                );
                Ok(TranslatedExpression {
                    expression: e,
                    ty: dynamic,
                })
            }
            // `VisitDynamicInvokeInstruction`
            DynamicKind::Invoke => {
                let target = self.translate_dynamic_target(arg(0)?, &info(0)?)?;
                let arguments = self
                    .translate_dynamic_arguments(&args[1..], arg_infos.get(1..).unwrap_or(&[]))?;
                let target_rr = self.rr_of(target.expression);
                let e = self.ast.add(AstNode::Invocation {
                    target: target.expression,
                    arguments: arguments.iter().map(|a| a.expression).collect(),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(
                    e,
                    Rr::DynamicInvocation {
                        target: Box::new(target_rr),
                        indexing: false,
                        symbol: None,
                    },
                );
                Ok(TranslatedExpression {
                    expression: e,
                    ty: dynamic,
                })
            }
            // `VisitDynamicSetIndexInstruction`
            DynamicKind::SetIndex => {
                if args.len() < 3 || arg_infos.len() < 3 {
                    return Err("a dynamic index assignment of fewer arguments".to_owned());
                }
                let target = self.translate_dynamic_target(args[0], &arg_infos[0])?;
                let mut arguments =
                    self.translate_dynamic_arguments(&args[1..], &arg_infos[1..])?;
                let value = arguments
                    .pop()
                    .ok_or("a dynamic index assignment without its value")?;
                self.require_unnamed(value)?;
                let symbol = DynamicSymbol::Indexer {
                    declaring: self.dynamic_argument_type(&arg_infos[0]),
                    parameters: self.dynamic_parameters(&arg_infos[1..arg_infos.len() - 1]),
                };
                let target_rr = self.rr_of(target.expression);
                let indexer = self.ast.add(AstNode::Indexer {
                    target: target.expression,
                    indices: arguments.iter().map(|a| a.expression).collect(),
                });
                self.ast.with_rr(
                    indexer,
                    Rr::DynamicInvocation {
                        target: Box::new(target_rr),
                        indexing: true,
                        symbol: Some(Box::new(symbol)),
                    },
                );
                let left = TranslatedExpression {
                    expression: indexer,
                    ty: dynamic,
                };
                let e = self.assignment(left, value)?;
                self.ast.with_il(e.expression, inst);
                Ok(e)
            }
            // `VisitDynamicSetMemberInstruction`
            DynamicKind::SetMember => {
                let name = name.ok_or("a dynamic member assignment without its name")?;
                let target_info = info(0)?;
                let target = self.translate_dynamic_target(arg(0)?, &target_info)?;
                let value = self.translate_dynamic_argument(arg(1)?, &info(1)?)?;
                self.require_unnamed(value)?;
                let member = self.dynamic_member(target, name, &target_info);
                let e = self.assignment(member, value)?;
                self.ast.with_il(e.expression, inst);
                Ok(e)
            }
            // `VisitDynamicBinaryOperatorInstruction`
            DynamicKind::BinaryOperator => {
                let (op, overflow) = match operation {
                    // Add, AddAssign
                    0 | 63 => (BinaryOp::Add, Some(checked)),
                    // AddChecked, AddAssignChecked
                    1 | 74 => (BinaryOp::Add, Some(true)),
                    // Subtract, SubtractAssign
                    42 | 73 => (BinaryOp::Subtract, Some(checked)),
                    // SubtractChecked, SubtractAssignChecked
                    43 | 76 => (BinaryOp::Subtract, Some(true)),
                    // Multiply, MultiplyAssign
                    26 | 69 => (BinaryOp::Multiply, Some(checked)),
                    // MultiplyChecked, MultiplyAssignChecked
                    27 | 75 => (BinaryOp::Multiply, Some(true)),
                    // Divide, DivideAssign
                    12 | 65 => (BinaryOp::Divide, None),
                    // Modulo, ModuloAssign
                    25 | 68 => (BinaryOp::Modulus, None),
                    13 => (BinaryOp::Equality, None),
                    35 => (BinaryOp::InEquality, None),
                    20 => (BinaryOp::LessThan, None),
                    21 => (BinaryOp::LessThanOrEqual, None),
                    15 => (BinaryOp::GreaterThan, None),
                    16 => (BinaryOp::GreaterThanOrEqual, None),
                    // And, AndAssign
                    2 | 64 => (BinaryOp::BitwiseAnd, None),
                    // Or, OrAssign
                    36 | 70 => (BinaryOp::BitwiseOr, None),
                    // ExclusiveOr, ExclusiveOrAssign
                    14 | 66 => (BinaryOp::ExclusiveOr, None),
                    // LeftShift, LeftShiftAssign
                    19 | 67 => (BinaryOp::ShiftLeft, None),
                    // RightShift, RightShiftAssign
                    41 | 72 => (BinaryOp::ShiftRight, None),
                    _ => return Err("a dynamic binary operator of another operation".to_owned()),
                };
                self.dynamic_binary(inst, op, overflow, &args, &arg_infos)
            }
            // `VisitDynamicLogicOperatorInstruction`
            DynamicKind::LogicOperator => {
                let op = match operation {
                    // AndAlso
                    3 => BinaryOp::ConditionalAnd,
                    // OrElse
                    37 => BinaryOp::ConditionalOr,
                    _ => return Err("a dynamic logic operator of another operation".to_owned()),
                };
                self.dynamic_binary(inst, op, None, &args, &arg_infos)
            }
            // `VisitDynamicUnaryOperatorInstruction`
            DynamicKind::UnaryOperator => {
                let (op, overflow) = match operation {
                    // Not
                    34 => (UNARY_NOT, None),
                    // Decrement
                    49 => (UNARY_DECREMENT, Some(checked)),
                    // Increment
                    54 => (UNARY_INCREMENT, Some(checked)),
                    // Negate
                    28 => (UNARY_MINUS, Some(checked)),
                    // NegateChecked
                    30 => (UNARY_MINUS, Some(true)),
                    // UnaryPlus
                    29 => (UNARY_PLUS, Some(checked)),
                    // OnesComplement
                    82 => (UNARY_BIT_NOT, None),
                    // IsTrue, IsFalse
                    83 | 84 => {
                        return self.dynamic_is_true(inst, operation == 83, arg(0)?, &info(0)?);
                    }
                    _ => return Err("a dynamic unary operator of another operation".to_owned()),
                };
                let operand = self.translate_dynamic_argument(arg(0)?, &info(0)?)?;
                self.require_unnamed(operand)?;
                let e = self.ast.add(AstNode::UnaryOperator {
                    op,
                    expression: operand.expression,
                    overflow: overflow_of(overflow),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Plain(dynamic));
                Ok(TranslatedExpression {
                    expression: e,
                    ty: dynamic,
                })
            }
            DynamicKind::IsEvent => Err("a dynamic IsEvent".to_owned()),
        }
    }

    /// `VisitDynamicCompoundAssign`: `target op= value`, checked or unchecked by the binder's
    /// context.
    pub(crate) fn dynamic_compound_assign(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Inst::DynamicCompoundAssign {
            operation,
            binder_flags,
            target_info,
            value_info,
            target_kind,
            target,
            value,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a dynamic compound assignment");
        };
        let dynamic = self.ts.intern(TypeData::Dynamic);
        // `AssignmentExpression.GetAssignmentOperatorTypeFromExpressionType`
        let op: u8 = match operation {
            // AddAssign, AddAssignChecked
            63 | 74 => 1,
            // SubtractAssign, SubtractAssignChecked
            73 | 76 => 2,
            // MultiplyAssign, MultiplyAssignChecked
            69 | 75 => 3,
            65 => 4,
            68 => 5,
            67 => 6,
            72 => 7,
            64 => 9,
            70 => 10,
            66 => 11,
            _ => return Err("a dynamic compound assignment of another operation".to_owned()),
        };
        let target = if target_kind == CompoundTargetKind::Address {
            // `LdObj(inst.Target, SpecialType.Dynamic)`
            let hint = if self.f.result_type(self.ts, target) == StackType::Ref {
                self.ts.intern(TypeData::ByRef(dynamic))
            } else {
                self.ts.intern(TypeData::Pointer(dynamic))
            };
            let address = self.translate(target, Some(hint))?;
            self.ld_obj_through(address, dynamic)?
        } else {
            self.translate_dynamic_argument(target, &target_info)?
        };
        self.require_unnamed(target)?;
        let value = self.translate_dynamic_argument(value, &value_info)?;
        self.require_unnamed(value)?;
        let e = self.ast.add(AstNode::CompoundAssignment {
            op,
            left: target.expression,
            right: value.expression,
            overflow: if binder_flags & CHECKED_CONTEXT != 0 {
                Overflow::Checked
            } else {
                Overflow::Unchecked
            },
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::DynamicOperator { operation });
        Ok(TranslatedExpression {
            expression: e,
            ty: dynamic,
        })
    }

    /// `CreateBinaryOperator` of the dynamic binary and logic operators: the operator over the
    /// translated operands, of type `dynamic`.
    fn dynamic_binary(
        &mut self,
        inst: NodeId,
        op: BinaryOp,
        overflow: Option<bool>,
        args: &[NodeId],
        arg_infos: &[DynamicArgInfo],
    ) -> Result<TranslatedExpression, Unsupported> {
        if args.len() < 2 || arg_infos.len() < 2 {
            return Err("a dynamic binary operator without its operands".to_owned());
        }
        let left = self.translate_dynamic_argument(args[0], &arg_infos[0])?;
        let right = self.translate_dynamic_argument(args[1], &arg_infos[1])?;
        self.require_unnamed(left)?;
        self.require_unnamed(right)?;
        let dynamic = self.ts.intern(TypeData::Dynamic);
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: left.expression,
            right: right.expression,
            overflow: overflow_of(overflow),
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(dynamic));
        Ok(TranslatedExpression {
            expression: e,
            ty: dynamic,
        })
    }

    /// The `IsTrue` and `IsFalse` cases of `VisitDynamicUnaryOperatorInstruction`: an implicit
    /// `operator true` in a condition, else a conditional that invokes the operator.
    fn dynamic_is_true(
        &mut self,
        inst: NodeId,
        is_true: bool,
        operand: NodeId,
        info: &DynamicArgInfo,
    ) -> Result<TranslatedExpression, Unsupported> {
        let operand = self.translate_dynamic_argument(operand, info)?;
        self.require_unnamed(operand)?;
        // `inst.SlotInfo == IfInstruction.ConditionSlot`
        let in_condition = is_true
            && self.f.node(inst).child_index == 0
            && self
                .f
                .node(inst)
                .parent
                .is_some_and(|p| matches!(self.f.inst(p), Inst::If { .. }));
        let e = if in_condition {
            self.ast.add(AstNode::UnaryOperator {
                op: UNARY_IS_TRUE,
                expression: operand.expression,
                overflow: Overflow::None,
            })
        } else {
            let t = self.ast.add(AstNode::BoolLiteral { value: is_true });
            let f = self.ast.add(AstNode::BoolLiteral { value: !is_true });
            self.ast.add(AstNode::Conditional {
                condition: operand.expression,
                true_expression: t,
                false_expression: f,
                resolve: false,
            })
        };
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(boolean));
        Ok(TranslatedExpression {
            expression: e,
            ty: boolean,
        })
    }

    /// The member reference of `VisitDynamicGetMemberInstruction` and
    /// `VisitDynamicSetMemberInstruction`, without instructions.
    fn dynamic_member(
        &mut self,
        target: TranslatedExpression,
        name: String,
        target_info: &DynamicArgInfo,
    ) -> TranslatedExpression {
        let declaring = self.dynamic_argument_type(target_info);
        let target_rr = self.rr_of(target.expression);
        let e = self.ast.add(AstNode::MemberReference {
            target: target.expression,
            name: name.clone(),
        });
        self.ast.with_rr(
            e,
            Rr::DynamicMember {
                target: Box::new(target_rr),
                name,
                declaring,
            },
        );
        TranslatedExpression {
            expression: e,
            ty: self.ts.intern(TypeData::Dynamic),
        }
    }

    /// `TranslateDynamicTarget`: a static type as a type reference, else the target converted
    /// to its compile-time type or `dynamic`, without the `ref` of a by-reference target.
    fn translate_dynamic_target(
        &mut self,
        inst: NodeId,
        info: &DynamicArgInfo,
    ) -> Result<TranslatedExpression, Unsupported> {
        if info.flags & IS_STATIC_TYPE != 0
            && let Some(t) = match_get_type_from_handle(self.f, self.ts, inst)
        {
            let e = self.ast.add(AstNode::TypeReference { ty: t });
            self.ast.with_rr(e, Rr::Type(t));
            return Ok(TranslatedExpression {
                expression: e,
                ty: t,
            });
        }
        let target_type = if info.flags & USE_COMPILE_TIME_TYPE != 0 {
            info.compile_time_type
        } else {
            self.ts.intern(TypeData::Dynamic)
        };
        let t = self.translate(inst, Some(target_type))?;
        let mut t = self.convert_to(t, target_type, false)?;
        if info.flags & IS_REF != 0 {
            // (ref x).member => x.member
            if let AstNode::RefDirection { expression }
            | AstNode::OutDirection { expression, .. }
            | AstNode::InDirection { expression } = *self.ast.node(t.expression)
            {
                t = self.unwrap_child(t.expression, expression);
            }
        }
        Ok(t)
    }

    /// `TranslateDynamicArguments`: the arguments paired with their infos.
    fn translate_dynamic_arguments(
        &mut self,
        arguments: &[NodeId],
        infos: &[DynamicArgInfo],
    ) -> Result<Vec<TranslatedExpression>, Unsupported> {
        let mut translated = Vec::with_capacity(arguments.len().min(infos.len()));
        for (&argument, info) in arguments.iter().zip(infos) {
            translated.push(self.translate_dynamic_argument(argument, info)?);
        }
        Ok(translated)
    }

    /// `TranslateDynamicArgument`: the argument converted to its compile-time type or
    /// `dynamic` (a null literal stays one for `dynamic`), `out` for an out argument, and
    /// named where the binder names it; a named argument has no resolve result.
    fn translate_dynamic_argument(
        &mut self,
        argument: NodeId,
        info: &DynamicArgInfo,
    ) -> Result<TranslatedExpression, Unsupported> {
        let dynamic = self.ts.intern(TypeData::Dynamic);
        let hint = if info.flags & USE_COMPILE_TIME_TYPE != 0 {
            info.compile_time_type
        } else {
            dynamic
        };
        let mut e = self.translate(argument, Some(hint))?;
        if !(self.ts.kind(hint) == TypeKind::Dynamic
            && matches!(self.ts.data(e.ty), TypeData::Null))
        {
            e = self.convert_to(e, hint, false)?;
        }
        if info.flags & IS_OUT != 0 {
            e = self.change_direction_to_out(e);
        }
        if info.flags & NAMED_ARGUMENT != 0
            && let Some(name) = info.name.as_ref().filter(|n| !n.trim().is_empty())
        {
            let n = self.ast.add(AstNode::NamedArgument {
                name: name.clone(),
                expression: e.expression,
            });
            e = TranslatedExpression {
                expression: n,
                ty: self.ts.intern(TypeData::UnknownSpecial),
            };
        }
        Ok(e)
    }

    /// A named argument outside an argument list has no resolve result to convert or take.
    fn require_unnamed(&self, e: TranslatedExpression) -> Result<(), Unsupported> {
        if self.ast.annotations(e.expression).rr.is_none() {
            return Err("a named dynamic argument outside an argument list".to_owned());
        }
        Ok(())
    }

    /// `DynamicArgumentType`: the compile-time type the binder uses, else `dynamic`.
    fn dynamic_argument_type(&mut self, info: &DynamicArgInfo) -> TypeId {
        if info.flags & (USE_COMPILE_TIME_TYPE | CONSTANT) != 0 {
            info.compile_time_type
        } else {
            self.ts.intern(TypeData::Dynamic)
        }
    }

    /// The parameters of a synthesized dynamic symbol: one per argument, of its
    /// `DynamicArgumentType`, named as the binder names it.
    fn dynamic_parameters(&mut self, infos: &[DynamicArgInfo]) -> Vec<(TypeId, String)> {
        infos
            .iter()
            .map(|i| {
                (
                    self.dynamic_argument_type(i),
                    i.name.clone().unwrap_or_default(),
                )
            })
            .collect()
    }
}

/// The `AddCheckedBlocks` annotation of `isChecked`: none, checked or unchecked.
fn overflow_of(checked: Option<bool>) -> Overflow {
    match checked {
        None => Overflow::None,
        Some(true) => Overflow::Checked,
        Some(false) => Overflow::Unchecked,
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn overflow_annotation_follows_is_checked() {
        assert_eq!(overflow_of(None), Overflow::None);
        assert_eq!(overflow_of(Some(true)), Overflow::Checked);
        assert_eq!(overflow_of(Some(false)), Overflow::Unchecked);
    }
}
