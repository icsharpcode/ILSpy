//! `ExpressionBuilder`: the C# expression of an ILAst instruction.

use crate::il::ast::{ComparisonKind, ComparisonLiftingKind, Inst, NodeId, VariableKind};
use crate::il::types::{Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

impl Builder<'_, '_, '_, '_> {
    /// `ExpressionBuilder.Translate(inst, typeHint)`.
    pub(crate) fn translate(
        &mut self,
        inst: NodeId,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let hint = match type_hint {
            Some(t) => t,
            None => self.ts.intern(TypeData::UnknownSpecial),
        };
        match self.f.inst(inst).clone() {
            Inst::LdcI4 { value } => self.ldc_i4(inst, value, hint),
            Inst::LdStr { value } => {
                let string = self.ts.known_type(KnownTypeCode::String);
                let e = self.ast.add(AstNode::StringLiteral {
                    value: value.clone(),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(
                    e,
                    Rr::Constant {
                        ty: string,
                        value: ConstValue::String(value),
                    },
                );
                Ok(TranslatedExpression {
                    expression: e,
                    ty: string,
                })
            }
            Inst::LdNull => {
                // `GetDefaultValueExpression(SpecialType.NullType)`
                let null = self.ts.intern(TypeData::Null);
                let e = self.ast.add(AstNode::Null);
                self.ast.with_rr(
                    e,
                    Rr::Constant {
                        ty: null,
                        value: ConstValue::Null,
                    },
                );
                self.ast.with_il(e, inst);
                Ok(TranslatedExpression {
                    expression: e,
                    ty: null,
                })
            }
            Inst::LdLoc { var } => {
                let e = self.convert_variable(var)?;
                self.ast.with_il(e.expression, inst);
                Ok(e)
            }
            Inst::LdObj {
                target,
                ty,
                unaligned,
                ..
            } => self.ld_obj(inst, target, ty.ty, unaligned, hint),
            // `DelayExceptions` (set for the `ldfld` the reader splits into `ldobj(ldflda)`)
            // does not change the translation.
            Inst::LdFlda { target, field, .. } => self.ld_flda(inst, Some(target), &field),
            Inst::LdsFlda { field } => self.ld_flda(inst, None, &field),
            Inst::StObj {
                target,
                value,
                ty,
                unaligned,
                ..
            } => self.st_obj(inst, target, value, ty.ty, unaligned),
            Inst::StLoc { var, value, .. } => self.st_loc(inst, var, value),
            Inst::Call(c) => self.call(inst, &c, super::call_builder::CallOp::Call),
            Inst::CallVirt(c) => self.call(inst, &c, super::call_builder::CallOp::CallVirt),
            Inst::Comp {
                kind,
                lifting,
                input,
                left,
                right,
                ..
            } => self.comp(inst, kind, lifting, input, left, right),
            other => Err(format!("the instruction {}", super::inst_name(&other))),
        }
    }

    /// `VisitLdcI4`: an int (a uint for an unsigned type hint) adjusted to the type hint.
    fn ldc_i4(
        &mut self,
        inst: NodeId,
        value: i32,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let (ty, value) = if self.ts.type_sign(hint) == Sign::Unsigned {
            (
                self.ts.known_type(KnownTypeCode::UInt32),
                ConstValue::UInt32(value as u32),
            )
        } else {
            (
                self.ts.known_type(KnownTypeCode::Int32),
                ConstValue::Int32(value),
            )
        };
        let (ty, value) = self.adjust_constant_to_type(ty, value, hint)?;
        let e = self.ast.add(AstNode::ConstantValue {
            ty,
            value: value.clone(),
            allow_implicit_conversion: true,
        });
        // `ConvertConstantValue` annotates the expression with the constant when the AST
        // builder did not.
        self.ast.with_rr(e, Rr::Constant { ty, value });
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `AdjustConstantToType`: the constant as the type hint, where that loses nothing.
    fn adjust_constant_to_type(
        &mut self,
        ty: TypeId,
        value: ConstValue,
        hint: TypeId,
    ) -> Result<(TypeId, ConstValue), Unsupported> {
        let hint = self.ts.nullable_underlying(hint);
        if ty == hint {
            return Ok((ty, value));
        }
        if self.ts.is_known(hint, KnownTypeCode::Boolean) {
            return Ok(match value {
                ConstValue::Int32(0) | ConstValue::UInt32(0) => (hint, ConstValue::Boolean(false)),
                ConstValue::Int32(1) | ConstValue::UInt32(1) => (hint, ConstValue::Boolean(true)),
                other => (ty, other),
            });
        }
        let small = matches!(
            self.ts.type_code(hint),
            5..=8 // SByte, Byte, Int16, UInt16
        );
        if self.ts.kind(hint) == crate::semantics::types::TypeKind::Enum
            || self.ts.is_known(hint, KnownTypeCode::Char)
            || small
        {
            return Err("a constant cast to an enum, char or small integer type".to_owned());
        }
        if self.ts.kind(hint).is_any_pointer() {
            return Err("a constant converted to a pointer type".to_owned());
        }
        Ok((ty, value))
    }

    /// `ConvertVariable`: `this`, or the variable's name, with an `ILVariableResolveResult`.
    pub(crate) fn convert_variable(
        &mut self,
        var: crate::il::ast::VarId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let v = self.f.var(var);
        let ty = v.ty;
        if matches!(self.ts.data(ty), TypeData::ByRef(_)) {
            return Err("a by-reference variable".to_owned());
        }
        let e = if v.kind == VariableKind::Parameter && v.index.is_some_and(|i| i < 0) {
            self.ast.add(AstNode::This)
        } else {
            self.ast.add(AstNode::Identifier {
                name: v.name.clone(),
            })
        };
        self.ast.with_rr(e, Rr::Variable { var, ty });
        Ok(TranslatedExpression { expression: e, ty })
    }
    /// `TranslateCondition(condition)`: the condition as a `bool`.
    pub(crate) fn translate_condition(
        &mut self,
        condition: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let e = self.translate(condition, Some(boolean))?;
        // `ConvertToBoolean` returns a bool unchanged.
        if !self.ts.is_known(e.ty, KnownTypeCode::Boolean) {
            return Err("a condition that is no bool".to_owned());
        }
        Ok(e)
    }

    /// `VisitComp` for the equality of a bool with 0 or 1, which `TranslateCeq` writes as the
    /// bool or its negation.
    fn comp(
        &mut self,
        inst: NodeId,
        kind: ComparisonKind,
        lifting: ComparisonLiftingKind,
        input: StackType,
        left: NodeId,
        right: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if lifting == ComparisonLiftingKind::ThreeValuedLogic {
            return Err("a comparison with three-valued logic".to_owned());
        }
        if input == StackType::Ref {
            return Err("a comparison of references".to_owned());
        }
        if !kind.is_equality_or_inequality() {
            return Err("a relational comparison".to_owned());
        }
        let is_inst = |b: &Self, n: NodeId| matches!(b.f.inst(n), Inst::IsInst { .. });
        let is_null = |b: &Self, n: NodeId| matches!(b.f.inst(n), Inst::LdNull);
        if (is_inst(self, left) && is_null(self, right))
            || (is_inst(self, right) && is_null(self, left))
        {
            return Err("a type test".to_owned());
        }
        let l = self.translate(left, None)?;
        let r = self.translate(right, None)?;
        let ldc = |b: &Self, n: NodeId| match *b.f.inst(n) {
            Inst::LdcI4 { value } => Some(value),
            _ => None,
        };
        // Remove redundant bool comparisons.
        let (result, constant) = if self.ts.is_known(l.ty, KnownTypeCode::Boolean) {
            (l, ldc(self, right))
        } else if self.ts.is_known(r.ty, KnownTypeCode::Boolean) {
            (r, ldc(self, left))
        } else {
            return Err("a comparison".to_owned());
        };
        let negate = match constant {
            Some(0) => kind == ComparisonKind::Equality,
            Some(1) => kind == ComparisonKind::Inequality,
            _ => return Err("a comparison".to_owned()),
        };
        if !negate {
            return Ok(result);
        }
        let e = self.logic_not(result);
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }

    /// `LogicNot(expr)`: `!expr`.
    pub(crate) fn logic_not(&mut self, expr: TranslatedExpression) -> TranslatedExpression {
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let e = self.ast.add(AstNode::LogicNot {
            expression: expr.expression,
        });
        self.ast.with_rr(e, Rr::Operator { ty: boolean });
        TranslatedExpression {
            expression: e,
            ty: boolean,
        }
    }
}
