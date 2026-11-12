//! `ExpressionBuilder`: the C# expression of an ILAst instruction.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, ComparisonKind, ComparisonLiftingKind, ConversionKind, Inst,
    NodeId, VariableKind,
};
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::metadata::tables::rows;
use crate::semantics::Conversions;
use crate::semantics::types::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, BinaryOp, InterpolatedContent, Overflow, Rr};
use super::call_builder::ACCESSOR_SETTER;
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
            // `VisitAddressOf`: `ref value`, through a copy (a redundant cast) where the value is
            // a mutable l-value whose address a method call could otherwise modify.
            Inst::AddressOf { value, ty } => {
                let target = ty.ty;
                // `ILInlining.ClassifyExpression`
                let mutable_lvalue = match *self.f.inst(value) {
                    Inst::LdLoc { var } | Inst::StLoc { var, .. } => {
                        let v = self.f.var(var);
                        !(v.is_ref_read_only
                            || matches!(
                                v.kind,
                                VariableKind::ForeachLocal | VariableKind::UsingLocal
                            ))
                    }
                    Inst::LdObj { .. } | Inst::StObj { .. } => {
                        return Err("the address of a loaded or stored value".to_owned());
                    }
                    Inst::Call(ref c) => matches!(
                        c.method.method.declaring.map(|t| self.ts.data(t)),
                        Some(TypeData::Array { .. })
                    ),
                    _ => false,
                };
                // `CanIgnoreCopy`: the address is only read through, under field addresses.
                let mut load_address = inst;
                while let Some(parent) = self.f.node(load_address).parent
                    && matches!(self.f.inst(parent), Inst::LdFlda { .. })
                {
                    load_address = parent;
                }
                let ignore_copy = self
                    .f
                    .node(load_address)
                    .parent
                    .is_some_and(|p| matches!(self.f.inst(p), Inst::LdObj { .. }));
                let v = self.translate(value, Some(target))?;
                let mut v = self.convert_to(v, target, false)?;
                if mutable_lvalue
                    && !ignore_copy
                    && !matches!(self.ast.node(v.expression), AstNode::Cast { .. })
                {
                    // An identity cast, whose resolve result is the host's `ResolveCast`; it
                    // has no checked or unchecked annotation.
                    let c = self.ast.add(AstNode::Cast {
                        ty: target,
                        expression: v.expression,
                        unchecked: false,
                    });
                    self.ast.with_rr(c, Rr::Conversion { ty: target });
                    v = TranslatedExpression {
                        expression: c,
                        ty: target,
                    };
                }
                let element = self.rr_of(v.expression);
                let d = self.ast.add(AstNode::RefDirection {
                    expression: v.expression,
                });
                self.ast.with_il(d, inst);
                self.ast.with_rr(d, Rr::ByReference(Box::new(element)));
                let ty = self.ts.intern(TypeData::ByRef(target));
                Ok(TranslatedExpression { expression: d, ty })
            }
            // `VisitLdElema`: `ref array[indices]`.
            Inst::LdElema {
                ty,
                array,
                indices,
                with_system_index,
                ..
            } => {
                if with_system_index {
                    return Err("an array index of type Index".to_owned());
                }
                let mut a = self.translate(array, None)?;
                let array_element = match *self.ts.data(a.ty) {
                    TypeData::Array { elem, .. } => Some(elem),
                    _ => None,
                };
                let element = match array_element {
                    Some(elem) if self.ts.is_compatible_type_for_memory_access(elem, ty.ty) => elem,
                    _ => {
                        let array_type = self.ts.intern(TypeData::Array {
                            elem: ty.ty,
                            rank: indices.len() as u32,
                            nullability: crate::typesystem::types::Nullability::Oblivious,
                        });
                        a = self.convert_to(a, array_type, false)?;
                        ty.ty
                    }
                };
                let mut written = Vec::with_capacity(indices.len());
                for &i in &indices {
                    written.push(self.translate_array_index(i)?.expression);
                }
                let e = self.ast.add(AstNode::Indexer {
                    target: a.expression,
                    indices: written,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Plain(element));
                let d = self.ast.add(AstNode::RefDirection { expression: e });
                self.ast
                    .with_rr(d, Rr::ByReference(Box::new(Rr::Plain(element))));
                let by_ref = self.ts.intern(TypeData::ByRef(element));
                Ok(TranslatedExpression {
                    expression: d,
                    ty: by_ref,
                })
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
            Inst::Function { .. } => self.translate_function(inst),
            Inst::Match { .. } => self.match_instruction(inst),
            Inst::DefaultValue { ty, .. } => self.default_value(inst, ty.ty),
            // `VisitNullableRewrap`: the value of a `?.` chain, nullable where it is a value type.
            Inst::NullableRewrap { arg, .. } => {
                let a = self.translate(arg, None)?;
                let ty = if self.ts.is_non_nullable_value_type(a.ty) {
                    self.ts.make_nullable(a.ty)
                } else {
                    a.ty
                };
                let e = self.ast.add(AstNode::NullConditional {
                    expression: a.expression,
                    rewrap: true,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Plain(ty));
                Ok(TranslatedExpression { expression: e, ty })
            }
            // `VisitNullableUnwrap`: `arg?`, of the underlying type.
            Inst::NullableUnwrap {
                arg,
                result,
                ref_input,
                ..
            } => {
                let mut a = self.translate(arg, None)?;
                if ref_input
                    && result != StackType::Ref
                    && let AstNode::RefDirection { expression } = *self.ast.node(a.expression)
                {
                    a = self.unwrap_child(a.expression, expression);
                }
                let ty = self.ts.nullable_underlying(a.ty);
                let e = self.ast.add(AstNode::NullConditional {
                    expression: a.expression,
                    rewrap: false,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Plain(ty));
                Ok(TranslatedExpression { expression: e, ty })
            }
            Inst::Block {
                kind: BlockKind::InterpolatedString,
                instructions,
                ..
            } => self.interpolated_string(inst, &instructions),
            Inst::Block {
                kind: BlockKind::ObjectInitializer,
                instructions,
                final_inst,
            } => self.object_initializer(inst, &instructions, final_inst),
            Inst::Block {
                kind: BlockKind::ArrayInitializer,
                instructions,
                final_inst,
            } => self.array_initializer(inst, &instructions, final_inst),
            Inst::Block { kind, .. } => Err(format!("a block of kind {kind:?}")),
            Inst::Conv {
                arg,
                input,
                input_sign,
                target,
                check_overflow,
                is_lifted,
            } => self.conv(
                inst,
                arg,
                input,
                input_sign,
                target,
                check_overflow,
                is_lifted,
                hint,
            ),
            Inst::Await {
                value,
                get_awaiter,
                get_result,
                get_awaiter_callvirt,
            } => self.translate_await(inst, value, get_awaiter, get_result, get_awaiter_callvirt),
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
        // `(e as T) == null` is `!(e is T)`.
        let tested = if is_inst(self, left) && is_null(self, right) {
            Some(left)
        } else if is_inst(self, right) && is_null(self, left) {
            Some(right)
        } else {
            None
        };
        if let Some(is_inst) = tested {
            let result = self.is_type(is_inst)?;
            if kind != ComparisonKind::Equality {
                return Ok(result);
            }
            let e = self.logic_not(result);
            self.ast.with_il(e.expression, inst);
            return Ok(e);
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
        // `TryUniteEqualityOperandType` treats an int zero compared with an enum apart (by the
        // enum's fields), which is not translated; other constants become the enum's.
        for (a, b) in [(&l, &r), (&r, &l)] {
            let zero = matches!(
                self.rr_of(a.expression),
                Rr::Constant {
                    value: ConstValue::Int32(0),
                    ..
                }
            );
            let u = self.ts.nullable_underlying(b.ty);
            if zero && self.ts.kind(u) == TypeKind::Enum {
                return Err("an enum comparison with zero".to_owned());
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
        let converts_operands = self.resolve_builtin_equality(op, &l, &r)?;
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
        // With converted operands the host resolves the operator, for their conversions.
        let rr = if converts_operands {
            Rr::ResolvedOperator { ty: boolean }
        } else {
            Rr::Operator { ty: boolean }
        };
        self.ast.with_rr(e, rr);
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
    ) -> Result<bool, Unsupported> {
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
        // bool operator ==(E x, E y) of an enum type
        if lt == rt && self.ts.kind(lt) == TypeKind::Enum {
            return Ok(false);
        }
        if self.ts.kind(lt) == TypeKind::Enum || self.ts.kind(rt) == TypeKind::Enum {
            return Err("an enum comparison that needs conversions".to_owned());
        }
        let (lc, rc) = (self.ts.type_code(lt), self.ts.type_code(rt));
        if (4..=15).contains(&lc) && (4..=15).contains(&rc) {
            return if lc == rc && (9..=14).contains(&lc) {
                Ok(false)
            } else {
                Err("a comparison that promotes its operands".to_owned())
            };
        }
        // bool operator ==(bool x, bool y)
        if lc == 3 && rc == 3 {
            return Ok(false);
        }
        // A reference comparison converts both operands to object.
        if self.ts.is_known(lt, KnownTypeCode::Object)
            && self.ts.is_known(rt, KnownTypeCode::Object)
        {
            return Ok(false);
        }
        // A type that can be null compared with the null literal.
        if l_null != r_null {
            let other = if l_null { rt } else { lt };
            if self.ts.is_reference_type(other) != Some(false) {
                return Ok(false);
            }
        }
        // Two values of one class or interface type: the reference equality of
        // `bool operator ==(object x, object y)`, both operands converted to object.
        if lt == rt
            && self.ts.is_reference_type(lt) == Some(true)
            && matches!(self.ts.kind(lt), TypeKind::Class | TypeKind::Interface)
        {
            return Ok(true);
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
        // `!expr` converts to bool implicitly, unless that would call a user-defined `operator !`.
        let expr = self.unwrap_implicit_bool_conversion(expr, true);
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
    /// `UnwrapImplicitBoolConversion`: the operand of a cast to `bool` that is a user-defined
    /// implicit conversion, which the context applies itself; with `check_logical_not`, not where
    /// the operand's type declares `operator !`.
    fn unwrap_implicit_bool_conversion(
        &mut self,
        e: TranslatedExpression,
        check_logical_not: bool,
    ) -> TranslatedExpression {
        if !self.ts.is_known(e.ty, KnownTypeCode::Boolean) {
            return e;
        }
        let AstNode::Cast {
            expression: inner, ..
        } = *self.ast.node(e.expression)
        else {
            return e;
        };
        let input = self.rr_of(inner);
        let input_ty = self.rr_type(&input);
        let user_defined_implicit = match self.rr_of(e.expression) {
            Rr::ImplicitConversion { .. } => true,
            Rr::Conversion { .. } => {
                let input_rr = self.semantic_rr(inner, input_ty);
                let c = Conversions::new(self.ts).explicit(&input_rr, e.ty);
                c.is_user_defined() && c.is_implicit()
            }
            _ => false,
        };
        if !user_defined_implicit
            || (check_logical_not && self.declares_method(input_ty, "op_LogicalNot"))
        {
            return e;
        }
        self.unwrap_child(e.expression, inner)
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
        let condition = self.unwrap_implicit_bool_conversion(condition, false);
        let true_branch = self.adjust_constant_expression_to_type(true_branch, false_branch.ty)?;
        let false_branch = self.adjust_constant_expression_to_type(false_branch, true_branch.ty)?;
        let (ty, true_branch, false_branch, rr) =
            match self.conditional_type(&true_branch, &false_branch)? {
                Some(ty) => {
                    let resolve = ty != true_branch.ty || ty != false_branch.ty;
                    (
                        ty,
                        true_branch,
                        false_branch,
                        (resolve, Rr::Operator { ty }),
                    )
                }
                // `rr.IsError`: both branches are converted to a target type.
                None => {
                    let ty =
                        self.conditional_target_type(inst, &true_branch, &false_branch, type_hint)?;
                    let t = self.convert_to(true_branch, ty, false)?;
                    let f = self.convert_to(false_branch, ty, false)?;
                    (ty, t, f, (false, Rr::Plain(ty)))
                }
            };
        let (resolve, rr) = rr;
        if matches!(self.ts.kind(ty), TypeKind::ByReference) {
            return Err("a ref conditional expression".to_owned());
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
            resolve,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, rr);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// The target type `VisitIfInstruction` converts both branches to where
    /// `ResolveConditional` reports an error: their best common type where the instruction's
    /// result has its stack type, else the type hint of that stack type; for a typeless branch
    /// the other branch's type. Inferring the type from the instruction is not translated.
    fn conditional_target_type(
        &mut self,
        inst: NodeId,
        t: &TranslatedExpression,
        f: &TranslatedExpression,
        type_hint: Option<TypeId>,
    ) -> Result<TypeId, Unsupported> {
        let null = |b: &Self, ty: TypeId| b.ts.kind(ty) == TypeKind::Null;
        if null(self, t.ty) || null(self, f.ty) || t.ty == f.ty {
            return Ok(if null(self, t.ty) { f.ty } else { t.ty });
        }
        let result = self.f.result_type(self.ts, inst);
        let t_rr = self.semantic_rr(t.expression, t.ty);
        let f_rr = self.semantic_rr(f.expression, f.ty);
        let mut conversions = Conversions::new(self.ts);
        let (common, success) =
            crate::semantics::TypeInference::new(&mut conversions).best_common_type(&[t_rr, f_rr]);
        if success && self.ts.stack_type(common) == result {
            return Ok(common);
        }
        if let Some(hint) = type_hint
            && self.ts.kind(hint) != TypeKind::Unknown
            && self.ts.stack_type(hint) == result
        {
            return Ok(hint);
        }
        Err("a conditional expression of a type inferred from its instruction".to_owned())
    }

    /// The type `ResolveConditional` gives branches of these types where it resolves without
    /// error (C# 4.0 spec 7.14): the other branch's type where one converts implicitly to it and
    /// not back (a constant expression conversion counting less), their type where they agree,
    /// or the typed branch's where the other is typeless and converts to it. Other cases and
    /// `dynamic` are not translated.
    fn conditional_type(
        &mut self,
        t: &TranslatedExpression,
        f: &TranslatedExpression,
    ) -> Result<Option<TypeId>, Unsupported> {
        if self.ts.kind(t.ty) == TypeKind::Dynamic || self.ts.kind(f.ty) == TypeKind::Dynamic {
            return Err("a dynamic conditional expression".to_owned());
        }
        let has_type =
            |b: &Self, ty: TypeId| !matches!(b.ts.kind(ty), TypeKind::None | TypeKind::Null);
        let t_rr = self.semantic_rr(t.expression, t.ty);
        let f_rr = self.semantic_rr(f.expression, f.ty);
        let typed = (has_type(self, t.ty), has_type(self, f.ty));
        let mut conversions = Conversions::new(self.ts);
        match typed {
            (true, true) => {
                let t2f = conversions.implicit(&t_rr, f.ty);
                let f2t = conversions.implicit(&f_rr, t.ty);
                // `IsBetterConditionalConversion`
                let better = |c1: &crate::semantics::Conversion,
                              c2: &crate::semantics::Conversion| {
                    let constant = crate::semantics::Conversion::IMPLICIT_CONSTANT_EXPRESSION;
                    c1.is_valid() && ((*c1 != constant && *c2 == constant) || !c2.is_valid())
                };
                if better(&t2f, &f2t) {
                    Ok(Some(f.ty))
                } else if better(&f2t, &t2f) || t.ty == f.ty {
                    Ok(Some(t.ty))
                } else {
                    Ok(None)
                }
            }
            (true, false) if conversions.implicit(&f_rr, t.ty).is_valid() => Ok(Some(t.ty)),
            (false, true) if conversions.implicit(&t_rr, f.ty).is_valid() => Ok(Some(f.ty)),
            _ => Ok(None),
        }
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
        if matches!(self.ts.kind(value.ty), TypeKind::Null | TypeKind::None) {
            return Err("a ?? that needs conversions".to_owned());
        }
        // `ResolveNullCoalescingOperator`: the fallback converted to the value's type, else the
        // value to the fallback's; with a conversion the host resolves the operator.
        let ty = if fallback.ty == value.ty {
            value.ty
        } else {
            let value_rr = self.semantic_rr(value.expression, value.ty);
            let fallback_rr = self.semantic_rr(fallback.expression, fallback.ty);
            let mut conversions = Conversions::new(self.ts);
            if conversions.implicit(&fallback_rr, value.ty).is_valid() {
                value.ty
            } else if conversions.implicit(&value_rr, fallback.ty).is_valid() {
                fallback.ty
            } else {
                return Err("a ?? that needs conversions".to_owned());
            }
        };
        let resolved = fallback.ty != value.ty;
        let e = self.ast.add(AstNode::BinaryOperator {
            overflow: Overflow::None,
            op: BinaryOp::NullCoalescing,
            left: value.expression,
            right: fallback.expression,
        });
        self.ast.with_il(e, inst);
        let rr = if resolved {
            Rr::ResolvedOperator { ty }
        } else {
            Rr::Operator { ty }
        };
        self.ast.with_rr(e, rr);
        Ok(TranslatedExpression { expression: e, ty })
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

    /// `TranslateArrayIndex`: the index converted to an integer type an array index takes.
    fn translate_array_index(&mut self, i: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let stack = self.f.result_type(self.ts, i);
        let input = self.translate(i, None)?;
        let sign = self.ts.type_sign(input.ty);
        // A truncated oversized index.
        if self.ts.type_size(input.ty) > stack_size(stack) {
            let target = self.find_integer_type(stack, sign)?;
            return self.convert_to(input, target, false);
        }
        // `IsCSharpPrimitiveIntegerType`: SByte to UInt64 other than Char, used as is.
        let code = self.ts.type_code(input.ty);
        if (5..=12).contains(&code) {
            return Ok(input);
        }
        if matches!(self.ts.kind(input.ty), TypeKind::NInt | TypeKind::NUInt) {
            return Err("a native integer array index".to_owned());
        }
        // `int` is preferred where it is big enough.
        let stack = if stack != StackType::I4 && self.ts.stack_type(input.ty) == StackType::I4 {
            StackType::I4
        } else {
            stack
        };
        let target = self.find_integer_type(stack, sign)?;
        self.convert_to(input, target, false)
    }

    /// `TranslateInterpolatedString`: the handler's `AppendLiteral` and `AppendFormatted` calls
    /// after its creation, as text and interpolations.
    fn interpolated_string(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
    ) -> Result<TranslatedExpression, Unsupported> {
        let mut contents = Vec::new();
        for &i in instructions.iter().skip(1) {
            let Inst::Call(call) = self.f.inst(i).clone() else {
                return Err("an interpolated string part that is no call".to_owned());
            };
            let args = &call.args;
            let literal = |b: &Self, n: NodeId| match b.f.inst(n) {
                Inst::LdStr { value } => Some(value.clone()),
                _ => None,
            };
            let number = |b: &Self, n: NodeId| match *b.f.inst(n) {
                Inst::LdcI4 { value } => Some(value),
                _ => None,
            };
            let name = call.method.method.name.as_str();
            let (alignment, suffix) = match (name, args.len()) {
                ("AppendLiteral", 2) => {
                    let text = literal(self, args[1]).ok_or("an AppendLiteral of no string")?;
                    let mut escaped = Vec::with_capacity(text.len());
                    for u in text {
                        escaped.push(u);
                        if u == u16::from(b'{') || u == u16::from(b'}') {
                            escaped.push(u);
                        }
                    }
                    contents.push(InterpolatedContent::Text(escaped));
                    continue;
                }
                ("AppendFormatted", 2) => (0, None),
                ("AppendFormatted", 3) => match (number(self, args[2]), literal(self, args[2])) {
                    (_, Some(s)) => (0, Some(s)),
                    (Some(a), _) => (a, None),
                    _ => return Err("an AppendFormatted of another shape".to_owned()),
                },
                ("AppendFormatted", 4) => match (number(self, args[2]), literal(self, args[3])) {
                    (Some(a), Some(s)) => (a, Some(s)),
                    _ => return Err("an AppendFormatted of another shape".to_owned()),
                },
                _ => return Err("an interpolated string part of another kind".to_owned()),
            };
            // `call.GetParameter(1)`: the method's first parameter after the handler.
            let params = call.method.method.params().map_err(|e| e.to_string())?;
            let ty = *params
                .first()
                .ok_or("an AppendFormatted without parameters")?;
            let value = self.translate(args[1], None)?;
            let value = self.convert_to(value, ty, true)?;
            contents.push(InterpolatedContent::Interpolation {
                expression: value.expression,
                alignment,
                suffix,
            });
        }
        let string = self.ts.known_type(KnownTypeCode::String);
        let e = self.ast.add(AstNode::InterpolatedString { contents });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(string));
        Ok(TranslatedExpression {
            expression: e,
            ty: string,
        })
    }

    /// `TranslateObjectAndCollectionInitializer` for `new T(...) { Member = value, ... }`: an
    /// object creation whose initializer sets fields and properties of the new object itself.
    /// Nested members, indexers, `Add` calls and other creations are not translated.
    fn object_initializer(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some((&first, rest)) = instructions.split_first() else {
            return Err("an empty initializer".to_owned());
        };
        let (Inst::StLoc { var, value, .. }, Inst::LdLoc { var: final_var }) =
            (self.f.inst(first).clone(), self.f.inst(final_inst).clone())
        else {
            return Err("an initializer of another shape".to_owned());
        };
        if var != final_var || self.f.var(var).kind != VariableKind::InitializerTarget {
            return Err("an initializer of another shape".to_owned());
        }
        if !matches!(self.f.inst(value), Inst::NewObj(_)) {
            return Err("an initializer of another creation".to_owned());
        }
        let creation = self.translate(value, None)?;
        if !matches!(
            self.ast.node(creation.expression),
            AstNode::ObjectCreate { .. }
        ) {
            return Err("an initializer of an object creation with syntax of its own".to_owned());
        }
        let object = Rr::InitializedObject(creation.ty);
        let is_target = |b: &Self, n: NodeId| matches!(*b.f.inst(n), Inst::LdLoc { var: v } | Inst::LdLoca { var: v } if v == var);
        let mut elements = Vec::with_capacity(rest.len());
        for &element in rest {
            // `GetAccessPath` of a setter on the object itself.
            let (name, member_type, value, member) = match self.f.inst(element).clone() {
                Inst::StObj { target, value, .. } => {
                    let Inst::LdFlda {
                        target: object_ref,
                        field,
                        ..
                    } = self.f.inst(target).clone()
                    else {
                        return Err("an initializer element of another kind".to_owned());
                    };
                    if !is_target(self, object_ref) {
                        return Err("a nested initializer".to_owned());
                    }
                    let rr = Rr::Field {
                        target: Some(Box::new(object.clone())),
                        field: field.clone(),
                    };
                    (field.field.name.clone(), field.field.ty, value, rr)
                }
                Inst::Call(call) | Inst::CallVirt(call)
                    if call.method.method.accessor_kind == ACCESSOR_SETTER
                        && call.args.len() == 2 =>
                {
                    if !is_target(self, call.args[0]) {
                        return Err("a nested initializer".to_owned());
                    }
                    let name = call
                        .method
                        .method
                        .name
                        .strip_prefix("set_")
                        .filter(|n| !n.contains('.'))
                        .ok_or("an initializer of an explicitly named setter")?
                        .to_owned();
                    let params = call.method.method.params().map_err(|e| e.to_string())?;
                    let ty = *params.last().ok_or("a setter without parameters")?;
                    let rr = Rr::Property {
                        target: Box::new(object.clone()),
                        accessor: Box::new(call.method.clone()),
                        ty,
                        found_by_lookup: false,
                    };
                    (name, ty, call.args[1], rr)
                }
                _ => return Err("an initializer element of another kind".to_owned()),
            };
            if self
                .f
                .descendants_post(value)
                .into_iter()
                .any(|n| is_target(self, n))
            {
                return Err("an initializer value that uses the object".to_owned());
            }
            let v = self.translate(value, Some(member_type))?;
            let v = self.convert_to(v, member_type, true)?;
            let e = self.ast.add(AstNode::Named {
                name,
                expression: v.expression,
            });
            self.ast.with_il(e, element);
            self.ast.with_rr(e, member);
            elements.push(e);
        }
        if let AstNode::ObjectCreate { initializer, .. } = self.ast.node_mut(creation.expression) {
            *initializer = Some(elements);
        }
        self.ast.with_il(creation.expression, inst);
        Ok(creation)
    }

    /// `TranslateArrayInitializer`: `new T[n, ...] { { ... }, ... }` from the stores of an
    /// array initializer block into the new array, nested by the constant dimensions. A block
    /// converted to a span and arrays of anonymous types are not translated.
    fn array_initializer(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some((&first, rest)) = instructions.split_first() else {
            return Err("an empty array initializer".to_owned());
        };
        let (Inst::StLoc { var, value, .. }, Inst::LdLoc { var: final_var }) =
            (self.f.inst(first).clone(), self.f.inst(final_inst).clone())
        else {
            return Err("an array initializer of another shape".to_owned());
        };
        let Inst::NewArr { ty, indices } = self.f.inst(value).clone() else {
            return Err("an array initializer of another shape".to_owned());
        };
        if var != final_var || self.f.var(var).kind != VariableKind::InitializerTarget {
            return Err("an array initializer of another shape".to_owned());
        }
        let element_type = ty.ty;
        if self.may_contain_anonymous_type(element_type) {
            return Err("an array initializer of anonymous types".to_owned());
        }
        let mut sizes = Vec::with_capacity(indices.len());
        for &i in &indices {
            let d = self.translate(i, None)?;
            let Rr::Constant { value, .. } = self.rr_of(d.expression) else {
                return Err("an array initializer of a dimension that is not constant".to_owned());
            };
            let size = super::translated_expression::integral_value(&value)
                .ok_or("an array initializer of a dimension that is not an integer")?;
            sizes.push(size);
        }
        let dimensions = sizes.len();
        let root = self.ast.add(AstNode::ArrayInitializer {
            elements: Vec::new(),
        });
        // The open initializers, each with the count of its elements.
        let mut container = vec![(root, 0i128)];
        let push = |b: &mut Self, parent: AstId, child: AstId| {
            if let AstNode::ArrayInitializer { elements } = b.ast.node_mut(parent) {
                elements.push(child);
            }
        };
        for &element in rest {
            let Inst::StObj {
                target,
                value,
                ty: store_type,
                ..
            } = self.f.inst(element).clone()
            else {
                return Err("an array initializer element of another kind".to_owned());
            };
            let Inst::LdElema {
                ty: element_ty,
                array,
                ..
            } = self.f.inst(target).clone()
            else {
                return Err("an array initializer element of another kind".to_owned());
            };
            if store_type.ty != element_type
                || element_ty.ty != element_type
                || !matches!(*self.f.inst(array), Inst::LdLoc { var: v } if v == var)
            {
                return Err("an array initializer element of another kind".to_owned());
            }
            while container.len() < dimensions {
                let nested = self.ast.add(AstNode::ArrayInitializer {
                    elements: Vec::new(),
                });
                let parent = container.last_mut().expect("an open initializer");
                parent.1 += 1;
                let parent = parent.0;
                push(self, parent, nested);
                container.push((nested, 0));
            }
            let v = self.translate(value, Some(element_type))?;
            let v = self.convert_to(v, element_type, true)?;
            let current = container
                .last_mut()
                .ok_or("more elements than the dimensions")?;
            current.1 += 1;
            let current = current.0;
            push(self, current, v.expression);
            while container
                .last()
                .is_some_and(|&(_, count)| count == sizes[container.len() - 1])
            {
                container.pop();
            }
        }
        let mut arguments = Vec::with_capacity(indices.len());
        for &i in &indices {
            arguments.push(self.translate(i, None)?.expression);
        }
        // `UseSpecialConstants` is off for the elements of integer and decimal arrays.
        let code = self.ts.type_code(element_type);
        let special_constants =
            !(5..=12).contains(&code) && !self.ts.is_known(element_type, KnownTypeCode::Decimal);
        let array_type = self.ts.intern(TypeData::Array {
            elem: element_type,
            rank: dimensions as u32,
            nullability: crate::typesystem::types::Nullability::Oblivious,
        });
        let e = self.ast.add(AstNode::ArrayCreate {
            element_type,
            arguments,
            initializer: root,
            special_constants,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::ArrayCreate { ty: array_type });
        Ok(TranslatedExpression {
            expression: e,
            ty: array_type,
        })
    }

    /// `VisitMatchInstruction`: `left is pattern` for a type pattern, a declaration pattern or
    /// `{ }`, without sub-patterns; a boxing cast of the tested value is dropped where the
    /// managed builder drops it.
    fn match_instruction(&mut self, inst: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Match {
            variable,
            tested_operand,
            sub_patterns,
            is_deconstruct_call,
            is_deconstruct_tuple,
            check_type,
            check_not_null,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a match");
        };
        if is_deconstruct_call || is_deconstruct_tuple || !sub_patterns.is_empty() {
            return Err("a recursive or positional pattern".to_owned());
        }
        let mut left = self.translate(tested_operand, None)?;
        if let AstNode::Cast {
            expression: inner, ..
        } = *self.ast.node(left.expression)
            && matches!(self.rr_of(left.expression), Rr::Conversion { .. })
        {
            let input = self.rr_of(inner);
            let input_ty = self.rr_type(&input);
            let input_rr = self.semantic_rr(inner, input_ty);
            let boxing = Conversions::new(self.ts)
                .explicit(&input_rr, left.ty)
                .is_boxing();
            let variable_type = self.f.var(variable).ty;
            if boxing
                && (self.ts.is_reference_type(input_ty) != Some(false)
                    || self.ts.is_reference_type(variable_type) == Some(false))
            {
                left = self.unwrap_child(left.expression, inner);
            }
        }
        let v = self.f.var(variable);
        let has_designator = v.load_count() + v.address_count() > 0;
        let variable_type = v.ty;
        let is_var = !check_type && !check_not_null;
        let pattern = if check_not_null && !check_type {
            // `{ } designation`
            self.ast.add(AstNode::RecursivePattern {
                designator: has_designator.then_some(variable),
            })
        } else if has_designator || !check_type {
            debug_assert!(check_type || is_var);
            self.ast.add(AstNode::DeclarationPattern {
                variable,
                var_keyword: !check_type,
            })
        } else {
            self.ast.add(AstNode::TypeReference { ty: variable_type })
        };
        self.ast.with_il(pattern, inst);
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let e = self.ast.add(AstNode::IsPattern {
            left: left.expression,
            pattern,
        });
        self.ast.with_rr(e, Rr::Plain(boolean));
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: boolean,
        })
    }

    /// `VisitDefaultValue`.
    fn default_value(
        &mut self,
        inst: NodeId,
        ty: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let e = self.default_value_expression(ty)?;
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }

    /// `GetDefaultValueExpression`: `null` for a reference type, `(T?)null` for a nullable
    /// type, else `default(T)`, a constant of the type's default value. `0m` is not
    /// translated.
    pub(crate) fn default_value_expression(
        &mut self,
        ty: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let null_type = self.ts.intern(TypeData::Null);
        let null = |b: &mut Self| {
            let n = b.ast.add(AstNode::Null);
            b.ast.with_rr(
                n,
                Rr::Constant {
                    ty: null_type,
                    value: ConstValue::Null,
                },
            );
            n
        };
        if self.ts.is_reference_type(ty) == Some(true) {
            let n = null(self);
            return Ok(TranslatedExpression {
                expression: n,
                ty: null_type,
            });
        }
        if self.ts.is_known(ty, KnownTypeCode::NullableOfT) {
            // `ConversionResolveResult` of the null literal conversion, as `ResolveCast` gives it.
            let n = null(self);
            let c = self.ast.add(AstNode::Cast {
                ty,
                expression: n,
                unchecked: false,
            });
            self.ast.with_rr(c, Rr::Conversion { ty });
            return Ok(TranslatedExpression { expression: c, ty });
        }
        // `CSharpResolver.GetDefaultValue`, of the underlying type of an enum.
        let value = match self.ts.enum_underlying_known(ty) {
            Some(KnownTypeCode::Boolean) => ConstValue::Boolean(false),
            Some(KnownTypeCode::Char) => ConstValue::Char(0),
            Some(KnownTypeCode::SByte) => ConstValue::SByte(0),
            Some(KnownTypeCode::Byte) => ConstValue::Byte(0),
            Some(KnownTypeCode::Int16) => ConstValue::Int16(0),
            Some(KnownTypeCode::UInt16) => ConstValue::UInt16(0),
            Some(KnownTypeCode::Int32) => ConstValue::Int32(0),
            Some(KnownTypeCode::UInt32) => ConstValue::UInt32(0),
            Some(KnownTypeCode::Int64) => ConstValue::Int64(0),
            Some(KnownTypeCode::UInt64) => ConstValue::UInt64(0),
            Some(KnownTypeCode::Single) => ConstValue::Single(0),
            Some(KnownTypeCode::Double) => ConstValue::Double(0),
            Some(KnownTypeCode::Decimal) => return Err("a default decimal".to_owned()),
            _ => ConstValue::Null,
        };
        let e = self.ast.add(AstNode::DefaultValue { ty: Some(ty) });
        self.ast.with_rr(e, Rr::Constant { ty, value });
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `IsType`: `arg is T`, without the boxing cast of a value.
    fn is_type(&mut self, inst: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let Inst::IsInst { arg, ty } = self.f.inst(inst).clone() else {
            unreachable!("an isinst");
        };
        let a = self.translate(arg, None)?;
        let a = self.unwrap_boxing_conversion(a);
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let e = self.ast.add(AstNode::IsType {
            expression: a.expression,
            ty: ty.ty,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::TypeIs { ty: ty.ty });
        Ok(TranslatedExpression {
            expression: e,
            ty: boolean,
        })
    }

    /// `UnwrapBoxingConversion`: the value an `object` cast boxes, which `is` and `as` box
    /// implicitly.
    fn unwrap_boxing_conversion(&mut self, e: TranslatedExpression) -> TranslatedExpression {
        let AstNode::Cast {
            expression: inner, ..
        } = *self.ast.node(e.expression)
        else {
            return e;
        };
        if !self.ts.is_known(e.ty, KnownTypeCode::Object)
            || !matches!(self.rr_of(e.expression), Rr::Conversion { .. })
        {
            return e;
        }
        let input = self.rr_of(inner);
        let input_ty = self.rr_type(&input);
        let input_rr = self.semantic_rr(inner, input_ty);
        if Conversions::new(self.ts)
            .explicit(&input_rr, e.ty)
            .is_boxing()
        {
            self.unwrap_child(e.expression, inner)
        } else {
            e
        }
    }

    /// `VisitConv` without lifting, overflow checks and native integers: the argument as is
    /// where the parent's post-condition handles the conversion, else cast to the target.
    #[allow(clippy::too_many_arguments)]
    fn conv(
        &mut self,
        inst: NodeId,
        arg: NodeId,
        input: StackType,
        input_sign: Sign,
        target: PrimitiveType,
        check_overflow: bool,
        is_lifted: bool,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if is_lifted {
            return Err("a lifted conversion".to_owned());
        }
        if check_overflow {
            return Err("a conversion with an overflow check".to_owned());
        }
        if matches!(input, StackType::I | StackType::Ref | StackType::Unknown)
            || matches!(
                target,
                PrimitiveType::I
                    | PrimitiveType::U
                    | PrimitiveType::Ref
                    | PrimitiveType::None
                    | PrimitiveType::Unknown
            )
        {
            return Err("a conversion from or to a native integer or reference".to_owned());
        }
        let hint_sign = if input_sign == Sign::None {
            self.ts.type_sign(hint)
        } else {
            input_sign
        };
        // `FindArithmeticType(inst.InputType, hintSign)`
        let arg_hint = match input.to_known(hint_sign) {
            KnownTypeCode::None | KnownTypeCode::Void => {
                return Err("a conversion of this stack type".to_owned());
            }
            code => self.ts.known_type(code),
        };
        let mut a = self.translate(arg, Some(arg_hint))?;
        let input_type = self.ts.nullable_underlying(a.ty);
        let kind = crate::il::ast::conversion_kind(target, input, input_sign);
        // The argument converted to the stack type of the given sign first.
        let to_input = |b: &mut Self, a: TranslatedExpression, sign: Sign| {
            let t = b.ts.known_type(input.to_known(sign));
            b.convert_to(a, t, false)
        };
        let oversized = |b: &mut Self| b.ts.type_size(input_type) > stack_size(input);
        match kind {
            ConversionKind::IntToFloat => {
                if oversized(self) || self.ts.type_sign(input_type) != input_sign {
                    a = to_input(self, a, input_sign)?;
                }
            }
            ConversionKind::StartGCTracking => return Ok(a),
            ConversionKind::SignExtend => {
                if self.ts.type_sign(input_type) != Sign::Signed || oversized(self) {
                    a = to_input(self, a, Sign::Signed)?;
                }
                self.ast.with_il(a.expression, inst);
                return Ok(a);
            }
            ConversionKind::ZeroExtend => {
                if self.ts.type_sign(input_type) != Sign::Unsigned || oversized(self) {
                    a = to_input(self, a, Sign::Unsigned)?;
                }
                self.ast.with_il(a.expression, inst);
                return Ok(a);
            }
            ConversionKind::Nop => {
                self.ast.with_il(a.expression, inst);
                return Ok(a);
            }
            ConversionKind::Truncate
                if !target.is_small_integer()
                    || (self.ts.type_size(input_type) <= target.size() as i32
                        && self.ts.type_sign(input_type) == target.sign()) =>
            {
                self.ast.with_il(a.expression, inst);
                return Ok(a);
            }
            ConversionKind::Truncate
            | ConversionKind::FloatToInt
            | ConversionKind::FloatPrecisionChange => {}
            _ => return Err("a conversion of this kind".to_owned()),
        }
        // The type hint where it is the target type, else the target's known type.
        let target_type = if !self.ts.is_nullable(hint) && self.ts.primitive_type(hint) == target {
            hint
        } else {
            self.ts.known_type(target.to_known())
        };
        let c = self.convert_to(a, target_type, false)?;
        self.ast.with_il(c.expression, inst);
        Ok(c)
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
