//! `ExpressionBuilder`: the C# expression of an ILAst instruction.

use crate::il::ast::{
    BinaryNumericOperator, ComparisonKind, ComparisonLiftingKind, Inst, NodeId, VariableKind,
};
use crate::il::types::{Sign, StackType};
use crate::metadata::tables::rows;
use crate::semantics::types::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, BinaryOp, Overflow, Rr};
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
            // `VisitLdLoca`: `ref name`, with the instruction on the name, which a dereference
            // keeps.
            Inst::LdLoca { var } => {
                if matches!(self.ts.data(self.f.var(var).ty), TypeData::ByRef(_)) {
                    return Err("the address of a by-reference variable".to_owned());
                }
                let e = self.convert_variable(var)?;
                self.ast.with_il(e.expression, inst);
                let element = self.rr_of(e.expression);
                let d = self.ast.add(AstNode::RefDirection {
                    expression: e.expression,
                });
                self.ast.with_rr(d, Rr::ByReference(Box::new(element)));
                let ty = self.ts.intern(TypeData::ByRef(e.ty));
                Ok(TranslatedExpression { expression: d, ty })
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
            Inst::NewObj(c) => self.new_obj(inst, &c),
            Inst::BinaryNumeric {
                op,
                check_overflow,
                sign,
                left_input,
                right_input,
                left,
                right,
                is_lifted,
                ..
            } => self.binary_numeric(
                inst,
                op,
                check_overflow,
                sign,
                left_input,
                right_input,
                left,
                right,
                is_lifted,
                type_hint,
            ),
            // `VisitLdTypeToken`: `typeof(T).TypeHandle`.
            Inst::LdTypeToken { ty } => {
                let type_type = self.ts.known_type(KnownTypeCode::Type);
                let handle = self.ts.known_type(KnownTypeCode::RuntimeTypeHandle);
                let t = self.ast.add(AstNode::TypeOf { ty: ty.ty });
                self.ast.with_rr(
                    t,
                    Rr::TypeOf {
                        ty: type_type,
                        referenced: ty.ty,
                    },
                );
                let m = self.ast.add(AstNode::MemberReference {
                    target: t,
                    name: "TypeHandle".to_owned(),
                });
                self.ast.with_il(m, inst);
                self.ast.with_rr(
                    m,
                    Rr::TypeOf {
                        ty: handle,
                        referenced: ty.ty,
                    },
                );
                Ok(TranslatedExpression {
                    expression: m,
                    ty: handle,
                })
            }
            // `VisitBox`: `(object)arg`, its boxing conversion resolved by the host's `ResolveCast`.
            Inst::Box { arg, ty } => {
                let target = ty.ty;
                if self.ts.is_known(target, KnownTypeCode::IntPtr)
                    || self.ts.is_known(target, KnownTypeCode::UIntPtr)
                {
                    return Err("a box of a native integer".to_owned());
                }
                let a = self.translate(arg, Some(target))?;
                let a = self.convert_to(a, target, false)?;
                let object = self.ts.known_type(KnownTypeCode::Object);
                let c = self.ast.add(AstNode::Cast {
                    ty: object,
                    expression: a.expression,
                    unchecked: false,
                });
                self.ast.with_il(c, inst);
                self.ast.with_rr(c, Rr::Conversion { ty: object });
                Ok(TranslatedExpression {
                    expression: c,
                    ty: object,
                })
            }
            // `VisitLdLen`: `array.Length` or `array.LongLength`.
            Inst::LdLen { array, result } => {
                let array_type = self.ts.known_type(KnownTypeCode::Array);
                let mut a = self.translate(array, Some(array_type))?;
                if self.ts.kind(a.ty) != TypeKind::Array {
                    a = self.convert_to(a, array_type, false)?;
                }
                let long = result != StackType::I4;
                let (name, code) = if long {
                    ("LongLength", KnownTypeCode::Int64)
                } else {
                    ("Length", KnownTypeCode::Int32)
                };
                let ty = self.ts.known_type(code);
                let e = self.ast.add(AstNode::MemberReference {
                    target: a.expression,
                    name: name.to_owned(),
                });
                let target = self.rr_of(a.expression);
                self.ast.with_il(e, inst);
                self.ast.with_rr(
                    e,
                    Rr::ArrayLength {
                        target: Box::new(target),
                        long,
                        ty,
                    },
                );
                Ok(TranslatedExpression { expression: e, ty })
            }
            // `VisitCastClass`
            Inst::CastClass { arg, ty } => {
                let e = self.translate(arg, None)?;
                self.convert_to(e, ty.ty, false)
            }
            Inst::NullCoalescing {
                value, fallback, ..
            } => self.null_coalescing(inst, value, fallback),
            Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } => self.if_expression(inst, condition, true_inst, false_inst, type_hint),
            Inst::Comp {
                kind,
                lifting,
                sign,
                input,
                left,
                right,
            } => self.comp(inst, kind, lifting, sign, input, left, right),
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
        let (ty, value) = self.constant_value_rr(ty, value, true)?;
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
            // An IL constant is a bit pattern: converting it to an integer type at least as
            // wide only reinterprets it, so only a narrowing cast checks for overflow.
            let may_be_lossy = !self.ts.stack_type(ty).is_integer()
                || self.ts.type_size(ty) > self.ts.type_size(hint);
            return Ok(
                match self.resolve_constant_cast(hint, ty, &value, may_be_lossy)? {
                    Some(cast) => (hint, cast),
                    None => (ty, value),
                },
            );
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
        let e = if v.kind == VariableKind::Parameter && v.index.is_some_and(|i| i < 0) {
            self.ast.add(AstNode::This)
        } else {
            self.ast.add(AstNode::Identifier {
                name: v.name.clone(),
            })
        };
        // A by-reference variable is `ref name`, the name with the element type's resolve result.
        if let TypeData::ByRef(element) = *self.ts.data(ty) {
            let element_rr = Rr::Variable { var, ty: element };
            self.ast.with_rr(e, element_rr.clone());
            let d = self.ast.add(AstNode::RefDirection { expression: e });
            self.ast.with_rr(d, Rr::ByReference(Box::new(element_rr)));
            return Ok(TranslatedExpression { expression: d, ty });
        }
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
    #[allow(clippy::too_many_arguments)]
    fn comp(
        &mut self,
        inst: NodeId,
        kind: ComparisonKind,
        lifting: ComparisonLiftingKind,
        sign: Sign,
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
            return self.translate_comp(inst, kind, lifting, sign, input, left, right);
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
        // Remove redundant bool comparisons: `b == 0` is `!b`, `b != 0` is `b`, and the
        // other way round for 1.
        let shortcut = |b: &Self, operand: TranslatedExpression, other: NodeId| match ldc(b, other)
        {
            Some(0) => Some((operand, ComparisonKind::Equality)),
            Some(1) => Some((operand, ComparisonKind::Inequality)),
            _ => None,
        };
        let redundant = if self.ts.is_known(l.ty, KnownTypeCode::Boolean) {
            shortcut(self, l, right)
        } else if self.ts.is_known(r.ty, KnownTypeCode::Boolean) {
            shortcut(self, r, left)
        } else {
            None
        };
        if let Some((result, negated)) = redundant {
            if kind != negated {
                return Ok(result);
            }
            let e = self.logic_not(result);
            self.ast.with_il(e.expression, inst);
            return Ok(e);
        }
        self.ceq(inst, kind, input, l, r, left, right)
    }

    /// The rest of `TranslateCeq`: `left == right` or `left != right` where the operator
    /// resolves to a built-in one without converting the operands.
    #[allow(clippy::too_many_arguments)]
    fn ceq(
        &mut self,
        inst: NodeId,
        kind: ComparisonKind,
        input: StackType,
        l: TranslatedExpression,
        r: TranslatedExpression,
        left: NodeId,
        right: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let is_zero = |b: &Self, n: NodeId| {
            matches!(
                *b.f.inst(n),
                Inst::LdcI4 { value: 0 } | Inst::LdcI8 { value: 0 }
            )
        };
        if (self.ts.kind(l.ty) == TypeKind::Pointer && is_zero(self, right))
            || (self.ts.kind(r.ty) == TypeKind::Pointer && is_zero(self, left))
        {
            return Err("a comparison of a pointer with null".to_owned());
        }
        // `TryUniteEqualityOperandType` treats a zero compared with an enum apart.
        for t in [l.ty, r.ty] {
            let u = self.ts.nullable_underlying(t);
            if self.ts.kind(u) == TypeKind::Enum {
                return Err("an enum comparison".to_owned());
            }
        }
        let l = self.adjust_constant_expression_to_type(l, r.ty)?;
        let r = self.adjust_constant_expression_to_type(r, l.ty)?;
        let op = if kind == ComparisonKind::Equality {
            BinaryOp::Equality
        } else {
            BinaryOp::InEquality
        };
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        if self.is_special_cased_reference_comparison_with_null(l.ty, r.ty) {
            // `CreateBuiltinBinaryOperator(...).WithILInstruction(inst)`
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: Overflow::None,
                op,
                left: l.expression,
                right: r.expression,
            });
            self.ast.with_rr(e, Rr::Operator { ty: boolean });
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: boolean,
            });
        }
        self.resolve_builtin_equality(op, &l, &r)?;
        // The check `TranslateCeq` makes before it takes the resolve result.
        let operand = self.ts.nullable_underlying(l.ty);
        if self.ts.stack_type(operand) != input {
            return Err("a comparison that needs casts".to_owned());
        }
        let e = self.ast.add(AstNode::BinaryOperator {
            overflow: Overflow::None,
            op,
            left: l.expression,
            right: r.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: boolean });
        Ok(TranslatedExpression {
            expression: e,
            ty: boolean,
        })
    }

    /// `AdjustConstantExpressionToType`: a constant converted to the type hint where that loses
    /// nothing, carrying the original's instructions.
    fn adjust_constant_expression_to_type(
        &mut self,
        e: TranslatedExpression,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Rr::Constant { ty, value } = self.rr_of(e.expression) else {
            return Ok(e);
        };
        let (new_ty, new_value) = self.adjust_constant_to_type(ty, value.clone(), hint)?;
        if new_ty == ty && new_value == value {
            return Ok(e);
        }
        let n = self.ast.add(AstNode::ConstantValue {
            ty: new_ty,
            value: new_value.clone(),
            allow_implicit_conversion: true,
        });
        let (ty, value) = self.constant_value_rr(new_ty, new_value, true)?;
        self.ast.with_rr(n, Rr::Constant { ty, value });
        for inst in self.ast.annotations(e.expression).il.clone() {
            self.ast.with_il(n, inst);
        }
        Ok(TranslatedExpression { expression: n, ty })
    }

    /// `IsSpecialCasedReferenceComparisonWithNull`: a string or delegate compared with the
    /// null literal, which the compiler emits as a reference comparison.
    fn is_special_cased_reference_comparison_with_null(&self, lhs: TypeId, rhs: TypeId) -> bool {
        let (lhs, rhs) = if self.ts.kind(lhs) == TypeKind::Null {
            (rhs, lhs)
        } else {
            (lhs, rhs)
        };
        self.ts.kind(rhs) == TypeKind::Null
            && (self.ts.kind(lhs) == TypeKind::Delegate
                || self.ts.is_known(lhs, KnownTypeCode::String))
            && self.ts.definition(lhs) != self.current_type
    }

    /// The part of `CSharpResolver.ResolveBinaryOperator` for `==` and `!=` that ends in a
    /// built-in operator whose parameters are the operands' types, so that its
    /// `OperatorResolveResult` has the operands' resolve results unchanged; every other
    /// outcome is unsupported.
    fn resolve_builtin_equality(
        &mut self,
        op: BinaryOp,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
    ) -> Result<(), Unsupported> {
        let (lt, rt) = (l.ty, r.ty);
        for t in [lt, rt] {
            match self.ts.kind(t) {
                TypeKind::Dynamic => return Err("a dynamic comparison".to_owned()),
                TypeKind::TypeParameter | TypeKind::NInt | TypeKind::NUInt => {
                    return Err("a comparison of a type parameter or native integer".to_owned());
                }
                TypeKind::Pointer | TypeKind::FunctionPointer => {
                    return Err("a pointer comparison".to_owned());
                }
                _ => {}
            }
            if self.ts.is_known(t, KnownTypeCode::NullableOfT) {
                return Err("a lifted comparison".to_owned());
            }
        }
        // `GetUserDefinedOperatorCandidates`, which skips the primitive types (Boolean to
        // Decimal).
        let name = if op == BinaryOp::Equality {
            "op_Equality"
        } else {
            "op_Inequality"
        };
        for t in [lt, rt] {
            if !(3..=15).contains(&self.ts.type_code(t)) && self.declares_method(t, name) {
                return Err("a comparison with user-defined operators".to_owned());
            }
        }
        let l_null = self.ts.kind(lt) == TypeKind::Null;
        let r_null = self.ts.kind(rt) == TypeKind::Null;
        if (l_null && self.ts.is_reference_type(rt) == Some(false))
            || (r_null && self.ts.is_reference_type(lt) == Some(false))
        {
            return Err("a lifted comparison".to_owned());
        }
        if matches!(self.rr_of(l.expression), Rr::Constant { .. })
            && matches!(self.rr_of(r.expression), Rr::Constant { .. })
        {
            return Err("a comparison of constants".to_owned());
        }
        // `BinaryNumericPromotion` converts unless both are the same type of at least `int`:
        // Int32, UInt32, Int64, UInt64, Single or Double.
        let (lc, rc) = (self.ts.type_code(lt), self.ts.type_code(rt));
        if (4..=15).contains(&lc) && (4..=15).contains(&rc) {
            return if lc == rc && (9..=14).contains(&lc) {
                Ok(())
            } else {
                Err("a comparison that promotes its operands".to_owned())
            };
        }
        // bool operator ==(bool x, bool y)
        if lc == 3 && rc == 3 {
            return Ok(());
        }
        // A reference comparison converts both operands to object.
        if self.ts.is_known(lt, KnownTypeCode::Object)
            && self.ts.is_known(rt, KnownTypeCode::Object)
        {
            return Ok(());
        }
        // A type that can be null compared with the null literal.
        if l_null != r_null {
            let other = if l_null { rt } else { lt };
            if self.ts.is_reference_type(other) != Some(false) {
                return Ok(());
            }
        }
        Err("a comparison that needs casts".to_owned())
    }

    /// Whether `t` or one of its base types declares a method named `name`.
    fn declares_method(&mut self, t: TypeId, name: &str) -> bool {
        for d in self.ts.all_base_type_definitions(t) {
            let md = &self.ts.comp.module(d.module).md;
            let Ok(methods) = md.type_methods(d.row) else {
                return true;
            };
            for m in methods {
                let Ok(row) = md.get::<rows::MethodDef>(m) else {
                    return true;
                };
                if md.string(row.name).map_or(true, |n| n == name) {
                    return true;
                }
            }
        }
        false
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
    /// `VisitIfInstruction` in an expression: `&&` and `||` with a `bool` on the right, and
    /// the conditional operator where both branches have the same type.
    fn if_expression(
        &mut self,
        inst: NodeId,
        condition: NodeId,
        true_inst: NodeId,
        false_inst: NodeId,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let condition = self.translate_condition(condition)?;
        let true_branch = self.translate(true_inst, type_hint)?;
        let false_branch = self.translate(false_inst, type_hint)?;
        let ldc = |b: &Self, n: NodeId| match *b.f.inst(n) {
            Inst::LdcI4 { value } => Some(value),
            _ => None,
        };
        // `MatchLogicAnd` is `if (a) b else 0`, `MatchLogicOr` is `if (a) 1 else b`.
        let logic = if ldc(self, false_inst) == Some(0) && ldc(self, true_inst) != Some(1) {
            Some((BinaryOp::ConditionalAnd, true_branch))
        } else if ldc(self, true_inst) == Some(1) && ldc(self, false_inst) != Some(0) {
            Some((BinaryOp::ConditionalOr, false_branch))
        } else {
            None
        };
        if let Some((op, rhs)) = logic {
            let is_bool = self.ts.is_known(rhs.ty, KnownTypeCode::Boolean);
            if is_bool || self.is_in_condition_slot(inst) {
                // `ConvertToBoolean` returns a bool unchanged.
                if !is_bool {
                    return Err("a logic operator with an operand that is no bool".to_owned());
                }
                let boolean = self.ts.known_type(KnownTypeCode::Boolean);
                let e = self.ast.add(AstNode::BinaryOperator {
                    overflow: Overflow::None,
                    op,
                    left: condition.expression,
                    right: rhs.expression,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Plain(boolean));
                return Ok(TranslatedExpression {
                    expression: e,
                    ty: boolean,
                });
            }
        }
        let true_branch = self.adjust_constant_expression_to_type(true_branch, false_branch.ty)?;
        let false_branch = self.adjust_constant_expression_to_type(false_branch, true_branch.ty)?;
        // `ResolveConditional` with branches of the same type: no conversion, the type is
        // theirs.
        let ty = true_branch.ty;
        let has_type =
            |b: &Self, t: TypeId| !matches!(b.ts.kind(t), TypeKind::None | TypeKind::Null);
        if ty != false_branch.ty
            || !has_type(self, ty)
            || matches!(self.ts.kind(ty), TypeKind::Dynamic | TypeKind::ByReference)
        {
            return Err("a conditional expression that needs conversions".to_owned());
        }
        let constant = |b: &Self, e: AstId| matches!(b.rr_of(e), Rr::Constant { .. });
        if constant(self, condition.expression)
            && constant(self, true_branch.expression)
            && constant(self, false_branch.expression)
        {
            return Err("a conditional expression of constants".to_owned());
        }
        let e = self.ast.add(AstNode::Conditional {
            condition: condition.expression,
            true_expression: true_branch.expression,
            false_expression: false_branch.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty });
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `IfInstruction.IsInConditionSlot`: whether the value is only used as a condition.
    fn is_in_condition_slot(&self, inst: NodeId) -> bool {
        let node = self.f.node(inst);
        let Some(parent) = node.parent else {
            return false;
        };
        let index = node.child_index;
        match self.f.inst(parent) {
            Inst::If { .. } => index == 0 || self.is_in_condition_slot(parent),
            Inst::NullCoalescing { .. } if index == 1 => self.is_in_condition_slot(parent),
            Inst::Comp { left, right, .. } => {
                let zero = |n: NodeId| matches!(*self.f.inst(n), Inst::LdcI4 { value: 0 });
                (*left == inst && zero(*right)) || (*right == inst && zero(*left))
            }
            _ => false,
        }
    }
    /// `VisitNullCoalescingInstruction` where `ResolveNullCoalescingOperator` needs no
    /// conversion: a value that is no nullable value type and a fallback of its type.
    fn null_coalescing(
        &mut self,
        inst: NodeId,
        value: NodeId,
        fallback: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let value = self.translate(value, None)?;
        let fallback = self.translate(fallback, None)?;
        let fallback = self.adjust_constant_expression_to_type(fallback, value.ty)?;
        for t in [value.ty, fallback.ty] {
            if self.ts.kind(t) == TypeKind::Dynamic {
                return Err("a dynamic ??".to_owned());
            }
        }
        if self.ts.is_known(value.ty, KnownTypeCode::NullableOfT) {
            return Err("a ?? of a nullable value type".to_owned());
        }
        // `TryConvert(ref rhs, lhs.Type)` leaves the fallback alone only for the same type.
        if fallback.ty != value.ty
            || matches!(self.ts.kind(value.ty), TypeKind::Null | TypeKind::None)
        {
            return Err("a ?? that needs conversions".to_owned());
        }
        let e = self.ast.add(AstNode::BinaryOperator {
            overflow: Overflow::None,
            op: BinaryOp::NullCoalescing,
            left: value.expression,
            right: fallback.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: value.ty });
        Ok(TranslatedExpression {
            expression: e,
            ty: value.ty,
        })
    }
    /// `TranslateComp`: a relational comparison, where the operands are of the same type
    /// from `int` to `double` (which binary numeric promotion leaves alone) and resolve to a
    /// built-in operator, or are cast to the comparison's integer type.
    #[allow(clippy::too_many_arguments)]
    fn translate_comp(
        &mut self,
        inst: NodeId,
        kind: ComparisonKind,
        lifting: ComparisonLiftingKind,
        sign: Sign,
        input: StackType,
        left: NodeId,
        right: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if lifting != ComparisonLiftingKind::None {
            return Err("a lifted relational comparison".to_owned());
        }
        let op = match kind {
            ComparisonKind::LessThan => BinaryOp::LessThan,
            ComparisonKind::LessThanOrEqual => BinaryOp::LessThanOrEqual,
            ComparisonKind::GreaterThan => BinaryOp::GreaterThan,
            ComparisonKind::GreaterThanOrEqual => BinaryOp::GreaterThanOrEqual,
            _ => unreachable!("an equality comparison"),
        };
        let l = self.translate(left, None)?;
        let r = self.translate(right, None)?;
        let l = self.prepare_arithmetic_argument(l, input, sign)?;
        let r = self.prepare_arithmetic_argument(r, input, sign)?;
        // Special case comparisons with enum and char literals
        let l = self.adjust_constant_expression_to_type(l, r.ty)?;
        let r = self.adjust_constant_expression_to_type(r, l.ty)?;
        for t in [l.ty, r.ty] {
            let k = self.ts.kind(self.ts.nullable_underlying(t));
            if matches!(
                k,
                TypeKind::Enum
                    | TypeKind::Pointer
                    | TypeKind::Dynamic
                    | TypeKind::NInt
                    | TypeKind::NUInt
                    | TypeKind::TypeParameter
            ) || self.ts.is_known(t, KnownTypeCode::NullableOfT)
            {
                return Err("a relational comparison of these types".to_owned());
            }
        }
        if matches!(self.rr_of(l.expression), Rr::Constant { .. })
            && matches!(self.rr_of(r.expression), Rr::Constant { .. })
        {
            return Err("a comparison of constants".to_owned());
        }
        // `ResolveBinaryOperator` leaves operands of the same type from Int32 to Double alone.
        let (lc, rc) = (self.ts.type_code(l.ty), self.ts.type_code(r.ty));
        if lc != rc || !(9..=14).contains(&lc) {
            return Err("a relational comparison that promotes its operands".to_owned());
        }
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let (l, r) = if self.ts.type_sign(l.ty) == sign && self.ts.stack_type(l.ty) == input {
            (l, r)
        } else if input.is_integer() {
            // Ensure the inputs have the correct sign.
            let input_type = self.find_integer_type(input, sign)?;
            let l = self.convert_to(l, input_type, false)?;
            let r = self.convert_to(r, input_type, false)?;
            (l, r)
        } else if matches!(input, StackType::F4 | StackType::F8) {
            (l, r)
        } else {
            return Err("a relational comparison of references".to_owned());
        };
        let e = self.ast.add(AstNode::BinaryOperator {
            overflow: Overflow::None,
            op,
            left: l.expression,
            right: r.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: boolean });
        Ok(TranslatedExpression {
            expression: e,
            ty: boolean,
        })
    }

    /// `PrepareArithmeticArgument` without lifting: an operand wider than the stack type is
    /// truncated to it.
    fn prepare_arithmetic_argument(
        &mut self,
        arg: TranslatedExpression,
        stack: StackType,
        sign: Sign,
    ) -> Result<TranslatedExpression, Unsupported> {
        let mut arg = arg;
        if stack.is_integer() && stack_size(stack) < self.ts.type_size(arg.ty) {
            let target = self.find_integer_type(stack, sign)?;
            arg = self.convert_to(arg, target, false)?;
        }
        if self.ts.is_known(arg.ty, KnownTypeCode::IntPtr)
            || self.ts.is_known(arg.ty, KnownTypeCode::UIntPtr)
        {
            return Err("arithmetic on IntPtr".to_owned());
        }
        Ok(arg)
    }

    /// `FindType(stackType, sign)` for the 32- and 64-bit integer stack types.
    fn find_integer_type(&mut self, stack: StackType, sign: Sign) -> Result<TypeId, Unsupported> {
        let unsigned = sign == Sign::Unsigned;
        let code = match stack {
            StackType::I4 if unsigned => KnownTypeCode::UInt32,
            StackType::I4 => KnownTypeCode::Int32,
            StackType::I8 if unsigned => KnownTypeCode::UInt64,
            StackType::I8 => KnownTypeCode::Int64,
            _ => return Err("arithmetic on a native integer".to_owned()),
        };
        Ok(self.ts.known_type(code))
    }

    /// `HandleBinaryNumeric` for `+ - * / %` and `& | ^` on operands of the same type from
    /// `int` to `double` (where the operator resolves to the built-in one of that type), or
    /// cast to the result's arithmetic type.
    #[allow(clippy::too_many_arguments)]
    fn binary_numeric(
        &mut self,
        inst: NodeId,
        operator: BinaryNumericOperator,
        check_overflow: bool,
        sign: Sign,
        left_input: StackType,
        right_input: StackType,
        left: NodeId,
        right: NodeId,
        is_lifted: bool,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let op = match operator {
            BinaryNumericOperator::Add => BinaryOp::Add,
            BinaryNumericOperator::Sub => BinaryOp::Subtract,
            BinaryNumericOperator::Mul => BinaryOp::Multiply,
            BinaryNumericOperator::Div => BinaryOp::Divide,
            BinaryNumericOperator::Rem => BinaryOp::Modulus,
            BinaryNumericOperator::BitAnd => BinaryOp::BitwiseAnd,
            BinaryNumericOperator::BitOr => BinaryOp::BitwiseOr,
            BinaryNumericOperator::BitXor => BinaryOp::ExclusiveOr,
            _ => return Err("a shift".to_owned()),
        };
        if is_lifted {
            return Err("a lifted arithmetic operation".to_owned());
        }
        let bitwise = matches!(
            op,
            BinaryOp::BitwiseAnd | BinaryOp::BitwiseOr | BinaryOp::ExclusiveOr
        );
        let hint = if bitwise && left_input != right_input {
            type_hint
        } else {
            None
        };
        let l = self.translate(left, hint)?;
        let r = self.translate(right, hint)?;
        if left_input == StackType::Ref || right_input == StackType::Ref {
            return Err("managed pointer arithmetic".to_owned());
        }
        for t in [l.ty, r.ty] {
            let k = self.ts.kind(self.ts.nullable_underlying(t));
            if matches!(
                k,
                TypeKind::Pointer
                    | TypeKind::Enum
                    | TypeKind::Dynamic
                    | TypeKind::NInt
                    | TypeKind::NUInt
                    | TypeKind::TypeParameter
            ) || self.ts.is_known(t, KnownTypeCode::NullableOfT)
            {
                return Err("arithmetic on these types".to_owned());
            }
        }
        let l = self.prepare_arithmetic_argument(l, left_input, sign)?;
        let r = self.prepare_arithmetic_argument(r, right_input, sign)?;
        if op == BinaryOp::Subtract
            && matches!(
                *self.f.inst(left),
                Inst::LdcI4 { value: 0 } | Inst::LdcI8 { value: 0 }
            )
        {
            return Err("a negation".to_owned());
        }
        if matches!(self.rr_of(l.expression), Rr::Constant { .. })
            && matches!(self.rr_of(r.expression), Rr::Constant { .. })
        {
            return Err("an arithmetic operation on constants".to_owned());
        }
        // `ResolveBinaryOperator` leaves operands of the same type from Int32 to Double alone.
        let (lc, rc) = (self.ts.type_code(l.ty), self.ts.type_code(r.ty));
        if lc != rc || !(9..=14).contains(&lc) {
            return Err("an arithmetic operation that promotes its operands".to_owned());
        }
        let result_stack = crate::il::ast::binary_result_type(operator, left_input, right_input);
        let compatible = |b: &mut Self, t: TypeId| sign == Sign::None || b.ts.type_sign(t) == sign;
        let (l, r) = if self.ts.stack_type(l.ty) == result_stack && compatible(self, l.ty) {
            (l, r)
        } else {
            // The operands are converted to a common type.
            let mut s = sign;
            if s == Sign::None {
                s = type_hint.map_or(Sign::None, |t| self.ts.type_sign(t));
                if s == Sign::None {
                    s = if bitwise {
                        Sign::Unsigned
                    } else {
                        Sign::Signed
                    };
                }
            }
            let target = match result_stack {
                StackType::F4 => self.ts.known_type(KnownTypeCode::Single),
                StackType::F8 => self.ts.known_type(KnownTypeCode::Double),
                _ => self.find_integer_type(result_stack, s)?,
            };
            let l = self.convert_to(l, target, false)?;
            let r = self.convert_to(r, target, false)?;
            (l, r)
        };
        // Constant operands of bitwise operators are written anew (in hexadecimal where that
        // reads better), which is not translated.
        if bitwise
            && (matches!(self.rr_of(l.expression), Rr::Constant { .. })
                || matches!(self.rr_of(r.expression), Rr::Constant { .. }))
        {
            return Err("a bitwise operation with a constant".to_owned());
        }
        let overflow = if bitwise || matches!(result_stack, StackType::F4 | StackType::F8) {
            Overflow::None
        } else if check_overflow {
            Overflow::Checked
        } else {
            Overflow::Unchecked
        };
        let ty = l.ty;
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: l.expression,
            right: r.expression,
            overflow,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty });
        Ok(TranslatedExpression { expression: e, ty })
    }
}

/// `StackType.GetSize` for integer stack types (`NativeIntSize` for `I`).
fn stack_size(stack: StackType) -> i32 {
    match stack {
        StackType::I4 => 4,
        StackType::I8 => 8,
        _ => crate::il::types::NATIVE_INT_SIZE,
    }
}
