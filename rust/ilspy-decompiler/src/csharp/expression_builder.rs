//! `ExpressionBuilder`: the C# expression of an ILAst instruction.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, ComparisonKind, ComparisonLiftingKind, ConversionKind, Inst,
    NodeId, VariableKind,
};
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::metadata::tables::rows;
use crate::semantics::types::TypeKind;
use crate::semantics::{Conversions, Method};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{
    AstId, AstNode, BinaryOp, InterpolatedContent, Overflow, PatternMember, PatternOperator, Rr,
    SubPattern,
};
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
            Inst::LdcI8 { value } => self.ldc_i8(inst, value, hint),
            // `VisitLdcF4` and `VisitLdcF8`: the literal of the floating point type.
            Inst::LdcF4 { bits } => {
                let ty = self.ts.known_type(KnownTypeCode::Single);
                self.float_constant(inst, ty, ConstValue::Single(bits))
            }
            Inst::LdcF8 { bits } => {
                let ty = self.ts.known_type(KnownTypeCode::Double);
                self.float_constant(inst, ty, ConstValue::Double(bits))
            }
            // `VisitLdcDecimal`: the literal of `decimal`.
            Inst::LdcDecimal { bits } => {
                let ty = self.ts.known_type(KnownTypeCode::Decimal);
                self.float_constant(inst, ty, ConstValue::Decimal(bits))
            }
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
                let v = self.f.var(var);
                if v.kind == VariableKind::StackSlot
                    && v.is_single_definition()
                    && !self.loaded_stack_slots.contains(&var)
                {
                    self.loaded_stack_slots.push(var);
                }
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
            Inst::Call(c) => {
                let e = self.call(inst, &c, super::call_builder::CallOp::Call)?;
                Ok(self.wrap_in_ref(e, c.method.method.ret))
            }
            Inst::CallVirt(c) => {
                let e = self.call(inst, &c, super::call_builder::CallOp::CallVirt)?;
                Ok(self.wrap_in_ref(e, c.method.method.ret))
            }
            Inst::NewObj(c) => self.new_obj(inst, &c, hint),
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
                let mut target = ty.ty;
                let a = self.translate(arg, Some(target))?;
                if a.ty != target {
                    target = self.prefer_native_integer_type(target);
                }
                let a = self.convert_to(a, target, false)?;
                let object = self.ts.known_type(KnownTypeCode::Object);
                let c = self.ast.add(AstNode::Cast {
                    ty: object,
                    expression: a.expression,
                    overflow: Overflow::None,
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
                    Inst::LdObj { target, .. } | Inst::StObj { target, .. } => {
                        !crate::il::transforms::il_inlining::is_readonly_reference_in(
                            self.f, self.ts, target,
                        )
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
                        overflow: Overflow::None,
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
            // `VisitLdElemaInlineArray`: `ref array[indices]` of an inline array, the target
            // translated as that of a member of the inline array type.
            Inst::LdElemaInlineArray {
                ty, array, indices, ..
            } => {
                let a = self.translate_target(Some(array), true, false, ty)?;
                let element = self.ts.inline_array_element_type(ty);
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
                kind: BlockKind::ObjectInitializer | BlockKind::CollectionInitializer,
                instructions,
                final_inst,
            } => self.object_initializer(inst, &instructions, final_inst),
            Inst::Block {
                kind: BlockKind::WithInitializer,
                instructions,
                final_inst,
            } => self.with_initializer(inst, &instructions, final_inst),
            Inst::Block {
                kind: BlockKind::ArrayInitializer,
                instructions,
                final_inst,
            } => self.array_initializer(inst, &instructions, final_inst),
            Inst::Block {
                kind: BlockKind::CallWithNamedArgs,
                instructions,
                final_inst,
            } => self.call_with_named_args(inst, &instructions, final_inst),
            // `TranslateSetterCallAssignment`: the setter call with the stored value as its value.
            Inst::Block {
                kind: BlockKind::CallInlineAssign,
                ..
            } => {
                let (call, value) =
                    crate::il::transforms::deconstruction_transform::match_inline_assign_block(
                        self.f, inst,
                    )
                    .ok_or("an inline assign block of another shape")?;
                let (mut data, op) = match self.f.inst(call).clone() {
                    Inst::Call(c) => (c, super::call_builder::CallOp::Call),
                    Inst::CallVirt(c) => (c, super::call_builder::CallOp::CallVirt),
                    _ => return Err("an inline assign block of another call".to_owned()),
                };
                *data.args.last_mut().expect("a setter value") = value;
                self.call(call, &data, op)
            }
            Inst::Block {
                kind: BlockKind::StackAllocInitializer,
                ..
            } => self.stack_alloc_initializer(inst, type_hint),
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
            Inst::NewArr { ty, ref indices } => self.new_arr(inst, ty.ty, indices),
            Inst::NumericCompoundAssign { .. } => self.numeric_compound_assign(inst),
            Inst::UserDefinedCompoundAssign { .. } => self.user_defined_compound_assign(inst),
            Inst::Switch {
                result_type: Some(_),
                ..
            } => self.switch_expression(inst, hint),
            // `VisitIsInst`: `arg as T` for a reference type, a boxing conversion of the argument
            // dropped; a value type, emulated with `is` and a conditional, is not translated.
            Inst::UnboxAny { arg, ty } => self.unbox_any(inst, arg, ty.ty),
            Inst::Deconstruct { .. } => self.deconstruct(inst),
            // `Build(LdVirtDelegate)`: the delegate construction of a `callvirt`.
            Inst::LdVirtDelegate {
                arg,
                ty,
                ref method,
            } => self.delegate_construction_of(
                inst,
                arg,
                method,
                super::call_builder::CallOp::CallVirt,
                ty,
            ),
            Inst::BitNot {
                arg,
                lifted_underlying,
            } => self.bit_not(inst, arg, lifted_underlying, hint),
            Inst::ThreeValuedBoolAnd { left, right } => {
                self.three_valued_logic(inst, left, right, BinaryOp::BitwiseAnd)
            }
            Inst::ThreeValuedBoolOr { left, right } => {
                self.three_valued_logic(inst, left, right, BinaryOp::BitwiseOr)
            }
            Inst::UserDefinedLogic {
                ref method,
                left,
                right,
            } => self.user_defined_logic(inst, method, left, right),
            // `VisitThrow`: `throw ex`, the exception translated with `Exception` as type hint.
            Inst::Throw { arg } => {
                let exception = self.ts.known_type(KnownTypeCode::Exception);
                let ex = self.translate(arg, Some(exception))?;
                let e = self.ast.add(AstNode::ThrowExpression {
                    expression: ex.expression,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Throw);
                Ok(TranslatedExpression {
                    expression: e,
                    ty: self.ts.intern(TypeData::NoType),
                })
            }
            Inst::IsInst { arg, ty } => {
                if self.ts.is_reference_type(ty.ty) != Some(true) {
                    return Err("an isinst of a value type".to_owned());
                }
                let a = self.translate(arg, None)?;
                let a = self.unwrap_boxing_conversion(a);
                let e = self.ast.add(AstNode::As {
                    expression: a.expression,
                    ty: ty.ty,
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::TryCast { ty: ty.ty });
                Ok(TranslatedExpression {
                    expression: e,
                    ty: ty.ty,
                })
            }
            Inst::Await {
                value,
                get_awaiter,
                get_result,
                get_awaiter_callvirt,
            } => self.translate_await(inst, value, get_awaiter, get_result, get_awaiter_callvirt),
            Inst::SizeOf { ty } => self.size_of(inst, ty.ty),
            Inst::CallIndirect { .. } => self.call_indirect(inst),
            Inst::Arglist
            | Inst::MakeRefAny { .. }
            | Inst::RefAnyType { .. }
            | Inst::RefAnyValue { .. } => self.undocumented(inst),
            Inst::LocAlloc { arg } => self.loc_alloc(inst, arg, None, type_hint),
            Inst::LocAllocSpan { arg, ty } => self.loc_alloc(inst, arg, Some(ty), type_hint),
            other => Err(format!("the instruction {}", super::inst_name(&other))),
        }
    }

    /// `VisitLdcI4`: an int (a uint for an unsigned type hint) adjusted to the type hint.
    pub(crate) fn ldc_i4(
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

    /// `VisitBitNot` without lifting: `~arg`, an undersized argument, a small integer enum, a
    /// `bool` or a `char` first converted to the result's integer type.
    fn bit_not(
        &mut self,
        inst: NodeId,
        arg: NodeId,
        lifted_underlying: Option<StackType>,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let result_stack = match lifted_underlying {
            Some(stack) => stack,
            None => self.f.result_type(self.ts, inst),
        };
        let mut a = self.translate(arg, None)?;
        if lifted_underlying.is_none() && self.ts.is_known(a.ty, KnownTypeCode::NullableOfT) {
            return Err("a ~ of a nullable value".to_owned());
        }
        let u = self.ts.nullable_underlying(a.ty);
        let stack = self.ts.stack_type(u);
        let small = self.ts.type_size(u) > 0 && self.ts.type_size(u) < 4;
        let enum_type = self.ts.kind(u) == TypeKind::Enum;
        // IntPtr, bool and char have no `~`.
        if stack_size(stack) < stack_size(result_stack)
            || (enum_type && small)
            || (stack == StackType::I && !self.is_native_integer(u))
            || self.ts.is_known(u, KnownTypeCode::Boolean)
            || self.ts.is_known(u, KnownTypeCode::Char)
        {
            let mut sign = self.ts.type_sign(hint);
            if sign == Sign::None {
                sign = self.ts.type_sign(u);
            }
            let mut target = self.find_arithmetic_type(result_stack, sign)?;
            if lifted_underlying.is_some() {
                target = self.ts.make_nullable(target);
            }
            a = self.convert_to(a, target, false)?;
        }
        // The type of `~`: an enum's or native integer's own, `int` for the small integers;
        // lifted, its nullable.
        let u = self.ts.nullable_underlying(a.ty);
        let mut ty = if self.ts.kind(u) == TypeKind::Enum || self.is_native_integer(u) {
            u
        } else {
            match self.ts.type_code(u) {
                5..=9 => self.ts.known_type(KnownTypeCode::Int32),
                10..=12 => u,
                _ => return Err("a ~ of these types".to_owned()),
            }
        };
        if self.ts.is_known(a.ty, KnownTypeCode::NullableOfT) {
            ty = self.ts.make_nullable(ty);
        }
        let e = self.ast.add(AstNode::BitNot {
            expression: a.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::ResolvedOperator { ty });
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `HandleThreeValuedLogic`: `left & right` or `left | right` on `bool?`, the operands
    /// converted to `bool?` (a non-nullable right operand of a nullable left one to `bool`),
    /// with the lifted operator's resolve result.
    fn three_valued_logic(
        &mut self,
        inst: NodeId,
        left: NodeId,
        right: NodeId,
        op: BinaryOp,
    ) -> Result<TranslatedExpression, Unsupported> {
        let l = self.translate(left, None)?;
        let r = self.translate(right, None)?;
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let nullable_bool = self.ts.make_nullable(boolean);
        let (l, r) = if self.ts.is_nullable(l.ty) {
            let l = self.convert_to(l, nullable_bool, false)?;
            let right_type = if self.ts.is_nullable(r.ty) {
                nullable_bool
            } else {
                boolean
            };
            (l, self.convert_to(r, right_type, false)?)
        } else {
            let l = self.convert_to(l, boolean, false)?;
            (l, self.convert_to(r, nullable_bool, false)?)
        };
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: l.expression,
            right: r.expression,
            overflow: Overflow::None,
        });
        self.ast.with_rr(
            e,
            Rr::ExplicitOperator {
                ty: nullable_bool,
                method: None,
                lifted: true,
                operand_overflow: Overflow::None,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: nullable_bool,
        })
    }

    /// `VisitUserDefinedLogicOperator`: `left && right` or `left || right` of a user-defined
    /// `&` or `|` (with `true` and `false` operators), the operands converted to the operator's
    /// parameter types.
    fn user_defined_logic(
        &mut self,
        inst: NodeId,
        operand: &crate::il::ast::MethodOperand,
        left: NodeId,
        right: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if operand.lifted
            || operand.local_function.is_some()
            || operand.specialized_by.is_some()
            || operand.range_accessor.is_some()
        {
            return Err("a lifted user-defined logic operator".to_owned());
        }
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a logic operator of an unresolved method")?;
        let [ref p0, ref p1] = method.parameters[..] else {
            return Err("a logic operator of another arity".to_owned());
        };
        let op = match method.name.as_str() {
            "op_BitwiseAnd" => BinaryOp::ConditionalAnd,
            "op_BitwiseOr" => BinaryOp::ConditionalOr,
            _ => return Err("a logic operator of another name".to_owned()),
        };
        let l = self.translate(left, Some(p0.ty))?;
        let l = self.convert_to(l, p0.ty, false)?;
        let r = self.translate(right, Some(p1.ty))?;
        let r = self.convert_to(r, p1.ty, false)?;
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: l.expression,
            right: r.expression,
            overflow: Overflow::None,
        });
        self.ast.with_rr(
            e,
            Rr::OperatorInvocation {
                ty: method.return_type,
                method: Box::new(operand.clone()),
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: method.return_type,
        })
    }

    /// A constant operand of a bitwise operator: `ConvertConstantValue(rr, allowImplicitConversion:
    /// false, ShouldDisplayAsHex(value, type))`, with the operand's instructions.
    fn bitwise_constant(
        &mut self,
        e: TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Rr::Constant { ty, value } = self.rr_of(e.expression) else {
            return Ok(e);
        };
        let v = super::translated_expression::integral_value(&value)
            .ok_or("a bitwise operation with a constant of another kind")?;
        // `CSharpPrimitiveCast.Cast(TypeCode.Int64, value)`, unchecked.
        let v = v as i64;
        let display_as_hex =
            !((0..=9).contains(&v) || (v < 0 && self.ts.type_sign(ty) == Sign::Signed));
        let n = self.ast.add(AstNode::HexConstant {
            ty,
            value: value.clone(),
            display_as_hex,
        });
        let (rr_ty, rr_value) = self.constant_value_rr(ty, value, false)?;
        self.ast.with_rr(
            n,
            Rr::Constant {
                ty: rr_ty,
                value: rr_value,
            },
        );
        for inst in self.ast.annotations(e.expression).il.clone() {
            self.ast.with_il(n, inst);
        }
        Ok(TranslatedExpression {
            expression: n,
            ty: e.ty,
        })
    }

    /// `VisitLdcI8`: the `long` (or, for an unsigned hint, `ulong`) constant, adjusted to the hint.
    fn ldc_i8(
        &mut self,
        inst: NodeId,
        value: i64,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let (ty, value) = if self.ts.type_sign(hint) == Sign::Unsigned {
            (
                self.ts.known_type(KnownTypeCode::UInt64),
                ConstValue::UInt64(value as u64),
            )
        } else {
            (
                self.ts.known_type(KnownTypeCode::Int64),
                ConstValue::Int64(value),
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

    /// `TypeSystemAstBuilder.ConvertConstantValue(type, value)` of a floating point literal.
    fn float_constant(
        &mut self,
        inst: NodeId,
        ty: TypeId,
        value: ConstValue,
    ) -> Result<TranslatedExpression, Unsupported> {
        let e = self.ast.add(AstNode::ConstantValue {
            ty,
            value: value.clone(),
            allow_implicit_conversion: false,
        });
        let (ty, value) = self.constant_value_rr(ty, value, false)?;
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
            // Zero is the null pointer; another constant stays an integer.
            if matches!(value, ConstValue::Int32(0) | ConstValue::UInt32(0)) {
                return Ok((hint, ConstValue::Null));
            }
            return Ok((ty, value));
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
        self.convert_to_boolean(e)
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
        let is_zero = |b: &Self, n: NodeId| b.f.match_ldc_i(n) == Some(0);
        let op = if kind == ComparisonKind::Equality {
            BinaryOp::Equality
        } else {
            BinaryOp::InEquality
        };
        // A pointer compared with null.
        if self.ts.kind(l.ty) == TypeKind::Pointer && is_zero(self, right) {
            let n = self.null_literal(right);
            let e = self.builtin_comparison(l, op, n);
            self.ast.with_il(e.expression, inst);
            return Ok(e);
        }
        if self.ts.kind(r.ty) == TypeKind::Pointer && is_zero(self, left) {
            let n = self.null_literal(left);
            let e = self.builtin_comparison(n, op, r);
            self.ast.with_il(e.expression, inst);
            return Ok(e);
        }
        let l = self.unite_equality_operand_type(l, &r)?;
        let r = self.unite_equality_operand_type(r, &l)?;
        // Two pointers: the built-in pointer comparison of `ResolveBinaryOperator`.
        if self.ts.kind(l.ty) == TypeKind::Pointer
            && self.ts.kind(r.ty) == TypeKind::Pointer
            && input == StackType::I
        {
            let boolean = self.ts.known_type(KnownTypeCode::Boolean);
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: Overflow::None,
                op,
                left: l.expression,
                right: r.expression,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Operator { ty: boolean });
            return Ok(TranslatedExpression {
                expression: e,
                ty: boolean,
            });
        }
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
        let resolved = self.resolve_builtin_equality(op, &l, &r)?;
        let (mut l, mut r) = (l, r);
        // The check `TranslateCeq` makes before it takes the resolve result: where no operator
        // applies, or one over another stack type, an operand is cast to the comparison's type,
        // and both where one is not enough.
        let mut converts_operands = resolved.unwrap_or(false);
        let mut operator_built = false;
        if !self.fits_comparison(resolved, &l, &r, input) {
            // Lifted, the cast is to the nullable of the comparison's type.
            let mut target = self.comparison_target_type(input, l.ty, r.ty);
            if matches!(
                self.f.inst(inst),
                Inst::Comp {
                    lifting: ComparisonLiftingKind::CSharp,
                    ..
                }
            ) {
                target = self.ts.make_nullable(target);
            }
            if target == l.ty {
                r = self.convert_to(r, target, false)?;
            } else {
                l = self.convert_to(l, target, false)?;
            }
            let resolved = self.resolve_builtin_equality(op, &l, &r)?;
            converts_operands = resolved.unwrap_or(false);
            if !self.fits_comparison(resolved, &l, &r, input) {
                l = self.convert_to(l, target, false)?;
                r = self.convert_to(r, target, false)?;
                converts_operands = false;
                operator_built = true;
            }
        }
        // The resolver folds a comparison of two constants into a constant, which the host
        // computes by resolving the operator; only the operator built over both converted
        // operands stays an operator.
        if !operator_built && self.both_constant(&l, &r) {
            converts_operands = true;
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

    /// Whether the operator resolved over the comparison's stack type.
    fn fits_comparison(
        &mut self,
        resolved: Option<bool>,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
        input: StackType,
    ) -> bool {
        let operand = self.equality_operand_type(l, r);
        resolved.is_some() && self.ts.stack_type(operand) == input
    }

    /// The type of the operator's first operand (`rr.Operands[0].Type`): the promoted type of
    /// two numeric operands, else the left operand's type, without nullability.
    fn equality_operand_type(
        &mut self,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
    ) -> TypeId {
        let lu = self.ts.nullable_underlying(l.ty);
        let ru = self.ts.nullable_underlying(r.ty);
        if (self.is_native_integer(l.ty) || self.is_native_integer(r.ty))
            && let Ok(Some(p)) = self.native_integer_promotion(l, r)
        {
            return p;
        }
        let (lc, rc) = (self.ts.type_code(lu), self.ts.type_code(ru));
        if self.ts.kind(lu) != TypeKind::Enum
            && self.ts.kind(ru) != TypeKind::Enum
            && (4..=15).contains(&lc)
            && (4..=15).contains(&rc)
            && let Ok(Some(p)) = self.operator_promotion(l, r, lc, rc)
        {
            return self.ts.known_type(known_of_type_code(p));
        }
        lu
    }

    /// The type of the predefined enum operator `+` or `-` the operands resolve to: `E + U`
    /// and `U + E` and `E - U` are of type `E`, `E - E` of type `U`, where `U` is the enum's
    /// underlying type and the other operand converts implicitly to it.
    fn enum_operator(
        &mut self,
        op: BinaryOp,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
    ) -> Option<TypeId> {
        let is_enum = |b: &Self, t: TypeId| b.ts.kind(t) == TypeKind::Enum;
        let (le, re) = (is_enum(self, l.ty), is_enum(self, r.ty));
        let converts = |b: &mut Self, e: &TranslatedExpression, to: TypeId| {
            let rr = b.semantic_rr(e.expression, e.ty);
            Conversions::new(b.ts).implicit(&rr, to).is_valid()
        };
        match (op, le, re) {
            (BinaryOp::Subtract, true, true) if l.ty == r.ty => Some(self.enum_underlying(l.ty)),
            (BinaryOp::Add | BinaryOp::Subtract, true, false) => {
                let u = self.enum_underlying(l.ty);
                converts(self, r, u).then_some(l.ty)
            }
            (BinaryOp::Add, false, true) => {
                let u = self.enum_underlying(r.ty);
                converts(self, l, u).then_some(r.ty)
            }
            _ => None,
        }
    }

    /// Whether an operand is `nint` or `nuint`.
    fn is_native_integer(&self, t: TypeId) -> bool {
        matches!(self.ts.kind(t), TypeKind::NInt | TypeKind::NUInt)
    }

    /// `BinaryNumericPromotion` and the native integer operators of `ResolveBinaryOperator`
    /// for operands of which one is `nint` or `nuint` (which promote between `int` and
    /// `long`): the operands' type, `None` where no operator applies (mixed signs with an
    /// unsigned operand, or a native integer next to a type it does not promote with).
    fn native_integer_promotion(
        &mut self,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
    ) -> Result<Option<TypeId>, Unsupported> {
        let operand = |b: &Self, e: &TranslatedExpression| {
            let kind = b.ts.kind(e.ty);
            let nonnegative_constant = matches!(b.rr_of(e.expression), Rr::Constant { value, .. }
                if super::translated_expression::integral_value(&value).is_some_and(|v| v >= 0));
            PromotionOperand {
                code: b.ts.type_code(e.ty),
                native: matches!(kind, TypeKind::NInt | TypeKind::NUInt).then_some(kind),
                nonnegative_constant,
            }
        };
        let (lo, ro) = (operand(self, l), operand(self, r));
        Ok(match native_promotion(lo, ro, l.ty == r.ty)? {
            None => None,
            Some(NativePromotion::Operands) => Some(l.ty),
            Some(NativePromotion::Known(code)) => Some(self.ts.known_type(code)),
            Some(NativePromotion::NInt) => Some(self.ts.intern(TypeData::NInt)),
            Some(NativePromotion::NUInt) => Some(self.ts.intern(TypeData::NUInt)),
        })
    }

    /// The type code of a binary operator's operands, `BinaryNumericPromotion` with the implicit
    /// constant conversions overload resolution sees: a non-negative `int` or `long` constant
    /// next to a `uint` or `ulong` operand converts to that type, whose operator is then the
    /// better one.
    fn operator_promotion(
        &self,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
        lc: u8,
        rc: u8,
    ) -> Result<Option<u8>, Unsupported> {
        const UINT32: u8 = 10;
        const UINT64: u8 = 12;
        if lc != rc {
            for (a, other) in [(l, rc), (r, lc)] {
                // An `int` constant converts to `uint` and `ulong`, a `long` constant to `ulong`.
                let (value, is_long) = match self.rr_of(a.expression) {
                    Rr::Constant {
                        value: ConstValue::Int32(v),
                        ..
                    } => (i128::from(v), false),
                    Rr::Constant {
                        value: ConstValue::Int64(v),
                        ..
                    } => (i128::from(v), true),
                    _ => continue,
                };
                let max = match other {
                    UINT32 if !is_long => i128::from(u32::MAX),
                    UINT64 => i128::from(u64::MAX),
                    _ => continue,
                };
                if (0..=max).contains(&value) {
                    return Ok(Some(other));
                }
            }
        }
        numeric_promotion(lc, rc)
    }

    /// The type `TranslateCeq` casts an operand to: `object` for a comparison of references,
    /// else the type of the operand that has the comparison's stack type and is no small
    /// integer, else the stack type's type with the left operand's sign.
    fn comparison_target_type(&mut self, input: StackType, left: TypeId, right: TypeId) -> TypeId {
        if input == StackType::Obj {
            return self.ts.known_type(KnownTypeCode::Object);
        }
        let lu = self.ts.nullable_underlying(left);
        let ru = self.ts.nullable_underlying(right);
        for u in [lu, ru] {
            let size = self.ts.type_size(u);
            if self.ts.stack_type(u) == input && !(size > 0 && size < 4) {
                return u;
            }
        }
        let sign = self.ts.primitive_type(lu).sign();
        self.find_type(input, sign)
    }

    /// `TryUniteEqualityOperandType`: a constant operand as the other operand's type, except an
    /// int zero compared with an enum where `AvoidConvertingZeroToEnum`, which stays `0`.
    fn unite_equality_operand_type(
        &mut self,
        operand: TranslatedExpression,
        other: &TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        let zero = matches!(
            self.rr_of(operand.expression),
            Rr::Constant {
                value: ConstValue::Int32(0),
                ..
            }
        );
        if zero && self.avoid_converting_zero_to_enum(other) {
            let int = self.ts.known_type(KnownTypeCode::Int32);
            return self.adjust_constant_expression_to_type(operand, int);
        }
        self.adjust_constant_expression_to_type(operand, other.ty)
    }

    /// `AvoidConvertingZeroToEnum`: `0` compared with an enum stays an integer for a flag check
    /// `(e & E.X) == 0`, and where the enum has no member of value zero to write instead.
    fn avoid_converting_zero_to_enum(&mut self, other: &TranslatedExpression) -> bool {
        let enum_type = self.ts.nullable_underlying(other.ty);
        if self.ts.kind(enum_type) != TypeKind::Enum {
            return false;
        }
        if matches!(
            self.ast.node(other.expression),
            AstNode::BinaryOperator {
                op: BinaryOp::BitwiseAnd,
                ..
            }
        ) {
            return true;
        }
        let Some(def) = self.ts.definition(enum_type) else {
            return true;
        };
        let fields = self
            .ts
            .comp
            .module(def.module)
            .md
            .type_fields(def.row)
            .unwrap_or_default();
        let has_zero = fields.iter().any(|&f| {
            super::translated_expression::integral_value(&self.ts.field_constant(def.module, f))
                .is_some_and(|v| v as u64 == 0)
        });
        !has_zero
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
    /// outcome is unsupported. `None` is a comparison no operator applies to.
    fn resolve_builtin_equality(
        &mut self,
        op: BinaryOp,
        l: &TranslatedExpression,
        r: &TranslatedExpression,
    ) -> Result<Option<bool>, Unsupported> {
        let (lt, rt) = (l.ty, r.ty);
        for t in [lt, rt] {
            match self.ts.kind(t) {
                TypeKind::Dynamic => return Err("a dynamic comparison".to_owned()),
                TypeKind::Pointer | TypeKind::FunctionPointer => {
                    return Err("a pointer comparison".to_owned());
                }
                _ => {}
            }
        }
        // A lifted operator: a nullable value compared with the null literal is the plain
        // operator; otherwise the predefined operator over the underlying types (the same enum,
        // `bool`, or the promoted numeric type), which the host resolves, since its resolve
        // result is a lifted operator.
        if self.ts.is_known(lt, KnownTypeCode::NullableOfT)
            || self.ts.is_known(rt, KnownTypeCode::NullableOfT)
        {
            if self.ts.kind(lt) == TypeKind::Null || self.ts.kind(rt) == TypeKind::Null {
                return Ok(Some(false));
            }
            let lu = self.ts.nullable_underlying(lt);
            let ru = self.ts.nullable_underlying(rt);
            if self.ts.kind(lu) == TypeKind::Enum || self.ts.kind(ru) == TypeKind::Enum {
                return if lu == ru {
                    Ok(Some(true))
                } else {
                    Err("a lifted enum comparison that needs conversions".to_owned())
                };
            }
            let (lc, rc) = (self.ts.type_code(lu), self.ts.type_code(ru));
            if lc == 3 && rc == 3 {
                return Ok(Some(true));
            }
            if (4..=14).contains(&lc) && (4..=14).contains(&rc) {
                return Ok(self.operator_promotion(l, r, lc, rc)?.map(|_| true));
            }
            return Err("a lifted comparison".to_owned());
        }
        // `GetUserDefinedOperatorCandidates`, which skips the primitive types (Boolean to
        // Decimal).
        let name = if op == BinaryOp::Equality {
            "op_Equality"
        } else {
            "op_Inequality"
        };
        // A user-defined operator `TranslateCeq` does not take: where both operands convert
        // implicitly to the declaring type, no built-in operator fits, and an operand is cast
        // (to `object` for references), after which the operator no longer applies.
        for t in [lt, rt] {
            if !(3..=15).contains(&self.ts.type_code(t)) && self.declares_method(t, name) {
                if self.ts.is_reference_type(t) != Some(true) {
                    return Err("a comparison with user-defined operators".to_owned());
                }
                // A cast null stays the null constant, which converts to every reference type.
                let applies = [l, r].into_iter().all(|e| {
                    let mut node = e.expression;
                    while let AstNode::Cast { expression, .. } = *self.ast.node(node) {
                        node = expression;
                    }
                    matches!(self.ast.node(node), AstNode::Null) || {
                        let rr = self.semantic_rr(e.expression, e.ty);
                        Conversions::new(self.ts).implicit(&rr, t).is_valid()
                    }
                });
                if applies {
                    return Ok(None);
                }
            }
        }
        let l_null = self.ts.kind(lt) == TypeKind::Null;
        let r_null = self.ts.kind(rt) == TypeKind::Null;
        if (l_null && self.ts.is_reference_type(rt) == Some(false))
            || (r_null && self.ts.is_reference_type(lt) == Some(false))
        {
            return Err("a lifted comparison".to_owned());
        }
        // `BinaryNumericPromotion` converts unless both are the same type of at least `int`:
        // Int32, UInt32, Int64, UInt64, Single or Double.
        // bool operator ==(E x, E y) of an enum type
        if lt == rt && self.ts.kind(lt) == TypeKind::Enum {
            return Ok(Some(false));
        }
        // An enum and another operand: the enum's operator where the other is a constant zero
        // (which converts implicitly to every enum), else no operator applies.
        if self.ts.kind(lt) == TypeKind::Enum || self.ts.kind(rt) == TypeKind::Enum {
            let other = if self.ts.kind(lt) == TypeKind::Enum {
                r
            } else {
                l
            };
            let zero = matches!(self.rr_of(other.expression), Rr::Constant { value, .. }
                if super::translated_expression::integral_value(&value) == Some(0));
            return Ok(zero.then_some(true));
        }
        // The native integer operator of the promoted type, which both operands convert to.
        if self.is_native_integer(lt) || self.is_native_integer(rt) {
            return Ok(self
                .native_integer_promotion(l, r)?
                .map(|p| !(lt == p && rt == p)));
        }
        let (lc, rc) = (self.ts.type_code(lt), self.ts.type_code(rt));
        if (4..=15).contains(&lc) && (4..=15).contains(&rc) {
            // The operator over the promoted type, converting the operands not of that type.
            return Ok(self
                .operator_promotion(l, r, lc, rc)?
                .map(|p| !(lc == p && rc == p)));
        }
        // bool operator ==(bool x, bool y)
        if lc == 3 && rc == 3 {
            return Ok(Some(false));
        }
        // A reference comparison converts both operands to object.
        if self.ts.is_known(lt, KnownTypeCode::Object)
            && self.ts.is_known(rt, KnownTypeCode::Object)
        {
            return Ok(Some(false));
        }
        // A type that can be null compared with the null literal.
        if l_null != r_null {
            let other = if l_null { rt } else { lt };
            if self.ts.is_reference_type(other) != Some(false) {
                return Ok(Some(false));
            }
        }
        // Two values of one class or interface type: the reference equality of
        // `bool operator ==(object x, object y)`, both operands converted to object.
        if lt == rt
            && self.ts.is_reference_type(lt) == Some(true)
            && matches!(self.ts.kind(lt), TypeKind::Class | TypeKind::Interface)
        {
            return Ok(Some(true));
        }
        // Two reference types: the reference equality where one converts to the other by a
        // reference conversion, else no operator applies (an error the caller casts for).
        if self.ts.is_reference_type(lt) == Some(true)
            && self.ts.is_reference_type(rt) == Some(true)
            && !l_null
            && !r_null
        {
            let mut conversions = Conversions::new(self.ts);
            let related = conversions.identity(lt, rt)
                || conversions.explicit_type(lt, rt).is_reference()
                || conversions.explicit_type(rt, lt).is_reference();
            return Ok(related.then_some(true));
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
        // A conditional ref: `ref (condition ? ref a : ref b)`, the conditional of the element
        // type.
        if let TypeData::ByRef(element) = *self.ts.data(ty) {
            let e = self.ast.add(AstNode::Conditional {
                condition: condition.expression,
                true_expression: true_branch.expression,
                false_expression: false_branch.expression,
                resolve: false,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Plain(element));
            let r = self.ast.add(AstNode::RefDirection { expression: e });
            self.ast
                .with_rr(r, Rr::ByReference(Box::new(Rr::Plain(element))));
            return Ok(TranslatedExpression { expression: r, ty });
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
        // The expression builder's `typeInference`, with the improved algorithm.
        let mut conversions = Conversions::new(self.ts);
        let mut ti = crate::semantics::TypeInference::new(&mut conversions);
        ti.algorithm = crate::semantics::TypeInferenceAlgorithm::Improved;
        let (common, success) = ti.best_common_type(&[t_rr, f_rr]);
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
        // A nullable value `a ?? b`: of the underlying type where `b` converts to it, else of
        // the nullable type; the host resolves the operator.
        if self.ts.is_known(value.ty, KnownTypeCode::NullableOfT) {
            let underlying = self.ts.nullable_underlying(value.ty);
            let fallback_rr = self.semantic_rr(fallback.expression, fallback.ty);
            let mut conversions = Conversions::new(self.ts);
            let ty = if conversions.implicit(&fallback_rr, underlying).is_valid() {
                underlying
            } else if conversions.implicit(&fallback_rr, value.ty).is_valid() {
                value.ty
            } else {
                return Err("a ?? of a nullable value type that needs conversions".to_owned());
            };
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: Overflow::None,
                op: BinaryOp::NullCoalescing,
                left: value.expression,
                right: fallback.expression,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::ResolvedOperator { ty });
            return Ok(TranslatedExpression { expression: e, ty });
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
        if lifting == ComparisonLiftingKind::ThreeValuedLogic {
            return Err("a lifted relational comparison".to_owned());
        }
        let lifted = lifting == ComparisonLiftingKind::CSharp;
        let op = match kind {
            ComparisonKind::LessThan => BinaryOp::LessThan,
            ComparisonKind::LessThanOrEqual => BinaryOp::LessThanOrEqual,
            ComparisonKind::GreaterThan => BinaryOp::GreaterThan,
            ComparisonKind::GreaterThanOrEqual => BinaryOp::GreaterThanOrEqual,
            _ => unreachable!("an equality comparison"),
        };
        let l = self.translate(left, None)?;
        let r = self.translate(right, None)?;
        if self.ts.kind(l.ty) == TypeKind::Pointer && self.ts.kind(r.ty) == TypeKind::Pointer {
            let e = self.builtin_comparison(l, op, r);
            self.ast.with_il(e.expression, inst);
            return Ok(e);
        }
        let l = self.prepare_lifted_arithmetic_argument(l, input, sign, lifted)?;
        let r = self.prepare_lifted_arithmetic_argument(r, input, sign, lifted)?;
        // Special case comparisons with enum and char literals
        let l = self.adjust_constant_expression_to_type(l, r.ty)?;
        let r = self.adjust_constant_expression_to_type(r, l.ty)?;
        // The enum comparison operators of two operands of one enum type.
        let same_enum = self.ts.kind(l.ty) == TypeKind::Enum && l.ty == r.ty;
        for t in [l.ty, r.ty] {
            let k = self.ts.kind(self.ts.nullable_underlying(t));
            if (k == TypeKind::Enum && !same_enum)
                || matches!(
                    k,
                    TypeKind::Pointer | TypeKind::Dynamic | TypeKind::TypeParameter
                )
                || (self.ts.is_known(t, KnownTypeCode::NullableOfT) && !lifted)
            {
                return Err("a relational comparison of these types".to_owned());
            }
        }
        // `ResolveBinaryOperator`: the operator over the promoted type of two numeric operands
        // (lifted, the promoted type of the underlying types); a constant of another type
        // converts implicitly in ways the promotion does not model.
        let promoted = if same_enum {
            Some(l.ty)
        } else if self.is_native_integer(l.ty) || self.is_native_integer(r.ty) {
            self.native_integer_promotion(&l, &r)?
        } else {
            let (lc, rc) = (
                self.ts.type_code(self.ts.nullable_underlying(l.ty)),
                self.ts.type_code(self.ts.nullable_underlying(r.ty)),
            );
            if !(4..=14).contains(&lc) || !(4..=14).contains(&rc) {
                return Err("a relational comparison of these types".to_owned());
            }
            self.operator_promotion(&l, &r, lc, rc)?
                .map(|p| self.ts.known_type(known_of_type_code(p)))
        };
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        // The resolver's operator where its operands have the comparison's sign and stack type;
        // the operator built over operands cast to the input type otherwise.
        let fits = promoted
            .is_some_and(|p| self.ts.type_sign(p) == sign && self.ts.stack_type(p) == input);
        let (l, r) = if fits {
            let p = promoted.expect("a promoted type");
            // A lifted operator is the resolver's.
            let rr = if lifted || p != l.ty || p != r.ty || self.both_constant(&l, &r) {
                Rr::ResolvedOperator { ty: boolean }
            } else {
                Rr::Operator { ty: boolean }
            };
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: Overflow::None,
                op,
                left: l.expression,
                right: r.expression,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, rr);
            return Ok(TranslatedExpression {
                expression: e,
                ty: boolean,
            });
        } else if input.is_integer() {
            // Ensure the inputs have the correct sign.
            let mut input_type = self.find_arithmetic_type(input, sign)?;
            if lifted {
                input_type = self.ts.make_nullable(input_type);
            }
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

    /// Whether both operands are constants.
    fn both_constant(&self, l: &TranslatedExpression, r: &TranslatedExpression) -> bool {
        matches!(self.rr_of(l.expression), Rr::Constant { .. })
            && matches!(self.rr_of(r.expression), Rr::Constant { .. })
    }

    /// `PrepareArithmeticArgument` of a lifted instruction: a nullable operand is prepared as
    /// its underlying type, converted to the nullable of the target; others as without lifting.
    fn prepare_lifted_arithmetic_argument(
        &mut self,
        arg: TranslatedExpression,
        stack: StackType,
        sign: Sign,
        is_lifted: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        if !is_lifted || !self.ts.is_known(arg.ty, KnownTypeCode::NullableOfT) {
            return self.prepare_arithmetic_argument(arg, stack, sign);
        }
        let mut arg = arg;
        let mut underlying = self.ts.nullable_underlying(arg.ty);
        if stack.is_integer() && stack_size(stack) < self.ts.type_size(underlying) {
            underlying = self.find_type(stack, sign);
            let target = self.ts.make_nullable(underlying);
            arg = self.convert_to(arg, target, false)?;
        }
        if self.ts.is_known(underlying, KnownTypeCode::IntPtr)
            || self.ts.is_known(underlying, KnownTypeCode::UIntPtr)
        {
            let target = self.find_arithmetic_type(StackType::I, sign)?;
            let target = self.ts.make_nullable(target);
            arg = self.convert_to(arg, target, false)?;
        }
        Ok(arg)
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
            let target = self.find_type(stack, sign);
            arg = self.convert_to(arg, target, false)?;
        }
        // No operator applies to IntPtr, and pointer arithmetic counts elements, not bytes:
        // native-sized operands are converted to nint or long.
        if self.ts.is_known(arg.ty, KnownTypeCode::IntPtr)
            || self.ts.is_known(arg.ty, KnownTypeCode::UIntPtr)
        {
            let target = self.find_arithmetic_type(StackType::I, sign)?;
            arg = self.convert_to(arg, target, false)?;
        }
        Ok(arg)
    }

    /// `PreferNativeIntegerType`: `nint` or `nuint` for `IntPtr` or `UIntPtr` with native
    /// integers.
    fn prefer_native_integer_type(&mut self, t: TypeId) -> TypeId {
        if self.settings.native_integers {
            if self.ts.is_known(t, KnownTypeCode::IntPtr) {
                return self.ts.intern(TypeData::NInt);
            }
            if self.ts.is_known(t, KnownTypeCode::UIntPtr) {
                return self.ts.intern(TypeData::NUInt);
            }
        }
        t
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
        if operator == BinaryNumericOperator::Div
            && let Some(e) = self.pointer_subtraction(inst)?
        {
            return Ok(e);
        }
        let op = match operator {
            BinaryNumericOperator::Add => BinaryOp::Add,
            BinaryNumericOperator::Sub => BinaryOp::Subtract,
            BinaryNumericOperator::Mul => BinaryOp::Multiply,
            BinaryNumericOperator::Div => BinaryOp::Divide,
            BinaryNumericOperator::Rem => BinaryOp::Modulus,
            BinaryNumericOperator::BitAnd => BinaryOp::BitwiseAnd,
            BinaryNumericOperator::BitOr => BinaryOp::BitwiseOr,
            BinaryNumericOperator::BitXor => BinaryOp::ExclusiveOr,
            BinaryNumericOperator::ShiftLeft | BinaryNumericOperator::ShiftRight => {
                return self.shift(
                    inst,
                    operator == BinaryNumericOperator::ShiftLeft,
                    sign,
                    left_input,
                    left,
                    right,
                    is_lifted,
                    type_hint,
                );
            }
            BinaryNumericOperator::None => return Err("a binary operator of no kind".to_owned()),
        };
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
        if (self.ts.kind(l.ty) == TypeKind::Pointer || self.ts.kind(r.ty) == TypeKind::Pointer)
            && let Some(e) = self.pointer_arithmetic(
                inst,
                operator,
                check_overflow,
                is_lifted,
                (left_input, right_input),
                (left, right),
                l,
                r,
                type_hint,
            )?
        {
            return Ok(e);
        }
        for t in [l.ty, r.ty] {
            let k = self.ts.kind(self.ts.nullable_underlying(t));
            if matches!(
                k,
                TypeKind::Pointer | TypeKind::Dynamic | TypeKind::TypeParameter
            ) || (self.ts.is_known(t, KnownTypeCode::NullableOfT) && !is_lifted)
            {
                let nullable = if self.ts.is_known(t, KnownTypeCode::NullableOfT) {
                    "nullable "
                } else {
                    ""
                };
                return Err(format!("arithmetic on {nullable}{k:?} operands"));
            }
        }
        // A lifted operator over a nullable operand is of the nullable of its type, which the
        // resolver gives.
        let nullable_result = is_lifted
            && (self.ts.is_known(l.ty, KnownTypeCode::NullableOfT)
                || self.ts.is_known(r.ty, KnownTypeCode::NullableOfT));
        if nullable_result
            && [l.ty, r.ty]
                .iter()
                .any(|&t| self.ts.kind(self.ts.nullable_underlying(t)) == TypeKind::Enum)
        {
            return Err("a lifted arithmetic operation".to_owned());
        }
        let l = self.prepare_lifted_arithmetic_argument(l, left_input, sign, is_lifted)?;
        let r = self.prepare_lifted_arithmetic_argument(r, right_input, sign, is_lifted)?;
        // `MatchLdcI(0)`, which sees through the extension of a native integer constant.
        // Unary minus is supported on int, long, nint and the small integers (which promote
        // to int).
        if op == BinaryOp::Subtract && self.f.match_ldc_i(left) == Some(0) {
            let right_underlying = self.ts.nullable_underlying(r.ty);
            if self.ts.is_known(right_underlying, KnownTypeCode::Int32)
                || self.ts.is_known(right_underlying, KnownTypeCode::Int64)
                || (5..=8).contains(&self.ts.type_code(right_underlying))
                || self.ts.kind(right_underlying) == TypeKind::NInt
            {
                let mut ty = self.find_arithmetic_type(right_input, Sign::Signed)?;
                if is_lifted {
                    ty = self.ts.make_nullable(ty);
                }
                let e = self.ast.add(AstNode::UnaryOperator {
                    op: 3,
                    expression: r.expression,
                    overflow: if check_overflow {
                        Overflow::Checked
                    } else {
                        Overflow::Unchecked
                    },
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Operator { ty });
                return Ok(TranslatedExpression { expression: e, ty });
            }
        }
        // `ResolveBinaryOperator`: the operator over the promoted type of two numeric operands
        // (`None` where no predefined operator applies); a constant of another type converts
        // implicitly in ways the promotion does not model.
        // A bitwise operator with an enum operand: the constants adjusted to the other operand's
        // type, the enum's operator where both are of the enum type.
        let enum_operands = bitwise
            && (self.ts.kind(l.ty) == TypeKind::Enum || self.ts.kind(r.ty) == TypeKind::Enum);
        let (mut l, mut r) = if enum_operands {
            let l = self.adjust_constant_expression_to_type(l, r.ty)?;
            let r = self.adjust_constant_expression_to_type(r, l.ty)?;
            (l, r)
        } else {
            (l, r)
        };
        let enum_arithmetic = matches!(op, BinaryOp::Add | BinaryOp::Subtract)
            && (self.ts.kind(l.ty) == TypeKind::Enum || self.ts.kind(r.ty) == TypeKind::Enum);
        let promoted = if enum_operands {
            (l.ty == r.ty).then_some(l.ty)
        } else if enum_arithmetic {
            // The enum operators `E + U`, `U + E`, `E - U` (of type `E`) and `E - E` (of type
            // `U`); where they do not resolve, the constants as the enum's bit patterns.
            match self.enum_operator(op, &l, &r) {
                Some(p) => Some(p),
                None => {
                    let al = self.adjust_constant_expression_to_type(l, r.ty)?;
                    let ar = self.adjust_constant_expression_to_type(r, l.ty)?;
                    let p = self.enum_operator(op, &al, &ar);
                    if p.is_some() {
                        l = al;
                        r = ar;
                    }
                    p
                }
            }
        } else {
            let (lc, rc) = (
                self.ts.type_code(self.ts.nullable_underlying(l.ty)),
                self.ts.type_code(self.ts.nullable_underlying(r.ty)),
            );
            // The predefined logical operators of `bool`.
            if bitwise && lc == 3 && rc == 3 {
                Some(self.ts.nullable_underlying(l.ty))
            } else if [l.ty, r.ty]
                .iter()
                .any(|&t| self.ts.kind(self.ts.nullable_underlying(t)) == TypeKind::Enum)
            {
                // No predefined operator of this kind takes an enum.
                None
            } else if self.is_native_integer(l.ty) || self.is_native_integer(r.ty) {
                self.native_integer_promotion(&l, &r)?
            } else {
                if !(4..=14).contains(&lc) || !(4..=14).contains(&rc) {
                    return Err(format!("arithmetic on the type codes {lc} and {rc}"));
                }
                self.operator_promotion(&l, &r, lc, rc)?
                    .map(|p| self.ts.known_type(known_of_type_code(p)))
            }
        };
        let result_stack = crate::il::ast::binary_result_type(operator, left_input, right_input);
        let compatible = |b: &mut Self, t: TypeId| sign == Sign::None || b.ts.type_sign(t) == sign;
        let fits =
            promoted.is_some_and(|p| self.ts.stack_type(p) == result_stack && compatible(self, p));
        // With operands converted by the promotion, the host resolves the operator.
        let mut resolved_type = None;
        let (l, r) = if let Some(p) = promoted.filter(|_| fits) {
            if nullable_result {
                resolved_type = Some(self.ts.make_nullable(p));
            } else if p != l.ty || p != r.ty {
                resolved_type = Some(p);
            }
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
                _ => self.find_arithmetic_type(result_stack, s)?,
            };
            // The nullable of the type for a nullable operand.
            let target_of = |b: &mut Self, t: TypeId| {
                if b.ts.is_known(t, KnownTypeCode::NullableOfT) {
                    b.ts.make_nullable(target)
                } else {
                    target
                }
            };
            let (lt, rt) = (target_of(self, l.ty), target_of(self, r.ty));
            let l = self.convert_to(l, lt, false)?;
            let r = self.convert_to(r, rt, false)?;
            if nullable_result {
                resolved_type = Some(self.ts.make_nullable(target));
            }
            (l, r)
        };
        // Constant operands of bitwise operators are written anew, without implicit conversion
        // and in hexadecimal where `ShouldDisplayAsHex` says so.
        let (l, r) = if bitwise {
            (self.bitwise_constant(l)?, self.bitwise_constant(r)?)
        } else {
            (l, r)
        };
        let overflow = if bitwise || matches!(result_stack, StackType::F4 | StackType::F8) {
            Overflow::None
        } else if check_overflow {
            Overflow::Checked
        } else {
            Overflow::Unchecked
        };
        let ty = resolved_type.unwrap_or(l.ty);
        // Two constant operands: the resolver folds the operator into a constant, and an
        // operation that overflows in a checked context is written `unchecked(...)`.
        let folded = match (self.rr_of(l.expression), self.rr_of(r.expression)) {
            (Rr::Constant { value: lv, .. }, Rr::Constant { value: rv, .. }) => {
                Some(self.fold_constant_arithmetic(op, &lv, &rv, ty, check_overflow)?)
            }
            _ => None,
        };
        let overflow = match folded {
            Some((_, true)) => Overflow::ExplicitUnchecked,
            _ => overflow,
        };
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: l.expression,
            right: r.expression,
            overflow,
        });
        self.ast.with_il(e, inst);
        let rr = if let Some((value, _)) = folded {
            Rr::Constant { ty, value }
        } else if resolved_type.is_some() {
            Rr::ResolvedOperator { ty }
        } else {
            Rr::Operator { ty }
        };
        self.ast.with_rr(e, rr);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// The constant `ResolveBinaryOperator` folds an unchecked integer `+`, `-`, `*`, `/` or `%`
    /// of two constants into (of the operator's type `ty`), and whether the operation overflows
    /// in a checked context (`ConstantBinaryOperatorOverflows`).
    fn fold_constant_arithmetic(
        &mut self,
        op: BinaryOp,
        left: &ConstValue,
        right: &ConstValue,
        ty: TypeId,
        check_overflow: bool,
    ) -> Result<(ConstValue, bool), Unsupported> {
        // shortcut: checked, floating point, bitwise and enum folding are not ported; port
        // them when such a constant operation is to be served.
        let unsupported = || "an arithmetic operation on constants".to_owned();
        let code = self.ts.type_code(ty);
        if check_overflow || self.ts.kind(ty) == TypeKind::Enum || !(9..=12).contains(&code) {
            return Err(unsupported());
        }
        let l = super::translated_expression::integral_value(left).ok_or_else(unsupported)?;
        let r = super::translated_expression::integral_value(right).ok_or_else(unsupported)?;
        let unsigned = matches!(code, 10 | 12);
        if unsigned && (l < 0 || r < 0) {
            return Err(unsupported());
        }
        let exact = match op {
            BinaryOp::Add => l + r,
            BinaryOp::Subtract => l - r,
            BinaryOp::Multiply => l.checked_mul(r).ok_or_else(unsupported)?,
            BinaryOp::Divide | BinaryOp::Modulus if r == 0 => return Err(unsupported()),
            BinaryOp::Divide => l / r,
            BinaryOp::Modulus => l % r,
            _ => return Err(unsupported()),
        };
        let (value, wrapped) = match code {
            9 => (ConstValue::Int32(exact as i32), i128::from(exact as i32)),
            10 => (ConstValue::UInt32(exact as u32), i128::from(exact as u32)),
            11 => (ConstValue::Int64(exact as i64), i128::from(exact as i64)),
            _ => (ConstValue::UInt64(exact as u64), i128::from(exact as u64)),
        };
        // `MinValue / -1` and `MinValue % -1` throw even unchecked.
        if matches!(op, BinaryOp::Divide | BinaryOp::Modulus) && r == -1 {
            return Err(unsupported());
        }
        Ok((value, wrapped != exact))
    }

    /// `HandleCompoundAssignment`: `target op= value`, or `target++` / `target--` for the old
    /// value. Shifts, lifted operators, pointer and native integer targets, dynamic targets and
    /// a checked conversion of the value are not translated.
    fn numeric_compound_assign(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        use crate::il::ast::{CompoundEvalMode, CompoundTargetKind};
        let Inst::NumericCompoundAssign {
            op,
            check_overflow,
            sign,
            right_input,
            underlying_result,
            is_lifted,
            ty,
            eval_mode,
            target_kind,
            target,
            value,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a compound assignment");
        };
        // `AssignmentOperatorType`: Add 1, Subtract 2, Multiply 3, Divide 4, Modulus 5,
        // BitwiseAnd 9, BitwiseOr 10, ExclusiveOr 11.
        let assignment_op: u8 = match op {
            BinaryNumericOperator::Add => 1,
            BinaryNumericOperator::Sub => 2,
            BinaryNumericOperator::Mul => 3,
            BinaryNumericOperator::Div => 4,
            BinaryNumericOperator::Rem => 5,
            BinaryNumericOperator::BitAnd => 9,
            BinaryNumericOperator::BitOr => 10,
            BinaryNumericOperator::BitXor => 11,
            BinaryNumericOperator::ShiftLeft => 6,
            // `UnsignedShiftRight` for an unsigned shift of a signed type, or of a small integer
            // type where the setting allows `>>>`.
            BinaryNumericOperator::ShiftRight
                if sign == Sign::Unsigned
                    && (self.ts.type_sign(ty) == Sign::Signed
                        || ((5..=8).contains(&self.ts.type_code(ty))
                            && self.settings.unsigned_right_shift)) =>
            {
                8
            }
            BinaryNumericOperator::ShiftRight => 7,
            BinaryNumericOperator::None => return Err("a binary operator of no kind".to_owned()),
        };
        let target = match target_kind {
            // `LdObj(inst.Target, inst.Type)`: the target address dereferenced.
            CompoundTargetKind::Address => {
                if self.f.result_type(self.ts, target) != StackType::Ref {
                    let hint = self.ts.intern(TypeData::Pointer(ty));
                    let address = self.translate(target, Some(hint))?;
                    self.ld_obj_through(address, ty)?
                } else {
                    let hint = self.ts.intern(TypeData::ByRef(ty));
                    let address = self.translate(target, Some(hint))?;
                    if !self.is_compatible_pointer_type_for_memory_access(address.ty, ty) {
                        return Err("a compound assignment that needs a pointer cast".to_owned());
                    }
                    let AstNode::RefDirection { expression } = *self.ast.node(address.expression)
                    else {
                        return Err(
                            "a compound assignment through a reference that is no ref expression"
                                .to_owned(),
                        );
                    };
                    self.unwrap_child(address.expression, expression)
                }
            }
            CompoundTargetKind::Property => self.translate(target, Some(ty))?,
            CompoundTargetKind::Dynamic => {
                return Err("a dynamic compound assignment target".to_owned());
            }
        };
        let target_underlying = self.ts.nullable_underlying(target.ty);
        let pointer_element = match *self.ts.data(target.ty) {
            TypeData::Pointer(e) if matches!(assignment_op, 1 | 2) => Some(e),
            _ => None,
        };
        if (pointer_element.is_none()
            && matches!(
                self.ts.kind(target_underlying),
                TypeKind::Pointer | TypeKind::FunctionPointer | TypeKind::Dynamic
            ))
            || (self.ts.is_known(target.ty, KnownTypeCode::NullableOfT) && !is_lifted)
        {
            return Err("a compound assignment of these types".to_owned());
        }
        if matches!(assignment_op, 6..=8) {
            return self.compound_shift(inst, assignment_op, target, value);
        }
        let bitwise = matches!(assignment_op, 9..=11);
        let overflow = if bitwise || matches!(underlying_result, StackType::F4 | StackType::F8) {
            Overflow::None
        } else if check_overflow {
            Overflow::Checked
        } else {
            Overflow::Unchecked
        };
        let target_rr_ty = target.ty;
        if eval_mode == CompoundEvalMode::EvaluatesToOldValue {
            let e = self.ast.add(AstNode::PostIncrement {
                increment: assignment_op == 1,
                expression: target.expression,
                overflow,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Operator { ty: target_rr_ty });
            return Ok(TranslatedExpression {
                expression: e,
                ty: target_rr_ty,
            });
        }
        let v = self.translate(value, None)?;
        let v = self.prepare_lifted_arithmetic_argument(v, right_input, sign, is_lifted)?;
        if let Some(element) = pointer_element {
            // `ptr += n` and `ptr -= n` with the offset in elements.
            let offset = self
                .pointer_arithmetic_offset(value, v, element, check_overflow, false)?
                .ok_or("a pointer compound assignment of no element offset")?;
            let e = self.ast.add(AstNode::CompoundAssignment {
                op: assignment_op,
                left: target.expression,
                right: offset.expression,
                overflow,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Operator { ty: target_rr_ty });
            return Ok(TranslatedExpression {
                expression: e,
                ty: target_rr_ty,
            });
        }
        // `ConvertValue`: to the target's type (an enum's underlying type for `+=` and `-=`),
        // its nullable for a nullable value.
        let mut value_type = target_underlying;
        if matches!(assignment_op, 1 | 2) && self.ts.kind(value_type) == TypeKind::Enum {
            value_type = self.enum_underlying(value_type);
        }
        // IntPtr and UIntPtr take a value cast to nint or nuint; nint and nuint convert it
        // implicitly.
        let mut allow_implicit_conversion = true;
        if self.ts.stack_type(value_type) == StackType::I {
            allow_implicit_conversion = self.is_native_integer(value_type);
            let unsigned = self.ts.type_sign(value_type) == Sign::Unsigned;
            value_type = self.ts.intern(if unsigned {
                TypeData::NUInt
            } else {
                TypeData::NInt
            });
        }
        if self.ts.is_nullable(v.ty) {
            value_type = self.ts.make_nullable(value_type);
        }
        let converted =
            self.convert_to_checked(v, value_type, check_overflow, allow_implicit_conversion)?;
        let e = self.ast.add(AstNode::CompoundAssignment {
            op: assignment_op,
            left: target.expression,
            right: converted.expression,
            overflow,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: target_rr_ty });
        Ok(TranslatedExpression {
            expression: e,
            ty: target_rr_ty,
        })
    }

    /// `VisitUserDefinedCompoundAssign`: `target op= value` of a user-defined binary operator
    /// (or `string.Concat` as `+=`), or `++` / `--` of a user-defined increment, of the
    /// operator's return type. A lifted operator, a span-based `string.Concat` and a string
    /// `+=` of a `ToString()` call are not translated.
    fn user_defined_compound_assign(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        use crate::il::ast::{CompoundEvalMode, CompoundTargetKind};
        let Inst::UserDefinedCompoundAssign {
            method: operand,
            eval_mode,
            target_kind,
            target,
            value,
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a user-defined compound assignment");
        };
        if operand.lifted
            || operand.local_function.is_some()
            || operand.specialized_by.is_some()
            || operand.range_accessor.is_some()
        {
            return Err("a lifted user-defined compound assignment".to_owned());
        }
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a compound assignment of an unresolved method")?;
        let declaring = method
            .declaring
            .ok_or("a compound assignment of a method without declaring type")?;
        let is_string_concat = method.name == "Concat"
            && method.is_static
            && self.ts.is_known(declaring, KnownTypeCode::String);
        if is_string_concat
            && method
                .parameters
                .iter()
                .any(|p| self.ts.is_known(p.ty, KnownTypeCode::ReadOnlySpanOfT))
        {
            return Err("a span-based string concatenation as +=".to_owned());
        }
        let load_type = method
            .parameters
            .first()
            .ok_or("a compound assignment of a method without parameters")?
            .ty;
        let target = match target_kind {
            // `LdObj(inst.Target, loadType)`: the target address dereferenced.
            CompoundTargetKind::Address => {
                if self.f.result_type(self.ts, target) != StackType::Ref {
                    return Err("a compound assignment through a pointer".to_owned());
                }
                let hint = self.ts.intern(TypeData::ByRef(load_type));
                let address = self.translate(target, Some(hint))?;
                if !self.is_compatible_pointer_type_for_memory_access(address.ty, load_type) {
                    return Err("a compound assignment that needs a pointer cast".to_owned());
                }
                let AstNode::RefDirection { expression } = *self.ast.node(address.expression)
                else {
                    return Err(
                        "a compound assignment through a reference that is no ref expression"
                            .to_owned(),
                    );
                };
                self.unwrap_child(address.expression, expression)
            }
            CompoundTargetKind::Property => self.translate(target, Some(load_type))?,
            CompoundTargetKind::Dynamic => {
                return Err("a dynamic compound assignment target".to_owned());
            }
        };
        // `OperatorDeclaration.IsChecked`, else `ReplaceMethodCallsWithOperators
        // .HasCheckedEquivalent`: the target carries the operator's checked context.
        let operand_overflow = if matches!(
            method.name.as_str(),
            "op_CheckedAddition"
                | "op_CheckedSubtraction"
                | "op_CheckedMultiply"
                | "op_CheckedDivision"
                | "op_CheckedUnaryNegation"
                | "op_CheckedIncrement"
                | "op_CheckedDecrement"
                | "op_CheckedExplicit"
        ) {
            Overflow::Checked
        } else if self.has_checked_equivalent(&method.name, declaring)? {
            Overflow::Unchecked
        } else {
            Overflow::None
        };
        let rr = Rr::ExplicitOperator {
            ty: method.return_type,
            method: Some(Box::new(operand.clone())),
            lifted: false,
            operand_overflow,
        };
        let e = if is_string_concat || method.parameters.len() == 2 {
            let value_type = method
                .parameters
                .get(1)
                .ok_or("a string concatenation of another arity as +=")?
                .ty;
            let v = self.translate(value, None)?;
            let v = self.convert_to(v, value_type, is_string_concat)?;
            if is_string_concat
                && let Rr::Invocation { method: called, .. } = self.rr_of(v.expression)
                && called.method.name == "ToString"
            {
                // shortcut: `RemoveRedundantToStringInConcat` is not ported; port it when a
                // string `+=` of a `ToString()` call is to be served.
                return Err("a string += of a ToString call".to_owned());
            }
            // `GetAssignmentOperatorTypeFromMetadataName`
            let op = match method.name.as_str() {
                _ if is_string_concat => 1,
                "op_Addition" => 1,
                "op_Subtraction" => 2,
                "op_Multiply" => 3,
                "op_Division" => 4,
                "op_Modulus" => 5,
                "op_LeftShift" => 6,
                "op_RightShift" => 7,
                "op_UnsignedRightShift" if self.settings.unsigned_right_shift => 8,
                "op_BitwiseAnd" => 9,
                "op_BitwiseOr" => 10,
                "op_ExclusiveOr" => 11,
                _ => return Err("a compound assignment of another operator".to_owned()),
            };
            self.ast.add(AstNode::CompoundAssignment {
                op,
                left: target.expression,
                right: v.expression,
                overflow: Overflow::None,
            })
        } else {
            // `GetUnaryOperatorTypeFromMetadataName`
            let postfix = eval_mode == CompoundEvalMode::EvaluatesToOldValue;
            let op = match method.name.as_str() {
                "op_Increment" | "op_CheckedIncrement" => 5,
                "op_Decrement" | "op_CheckedDecrement" => 6,
                _ => return Err("a compound assignment of another unary operator".to_owned()),
            } + if postfix { 2 } else { 0 };
            self.ast.add(AstNode::UnaryOperator {
                op,
                expression: target.expression,
                overflow: Overflow::None,
            })
        };
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, rr);
        Ok(TranslatedExpression {
            expression: e,
            ty: method.return_type,
        })
    }

    /// `ReplaceMethodCallsWithOperators.HasCheckedEquivalent`: whether the declaring type has
    /// an operator `op_Checked...` of the operator's name.
    fn has_checked_equivalent(
        &mut self,
        name: &str,
        declaring: TypeId,
    ) -> Result<bool, Unsupported> {
        let checked = match name.strip_prefix("op_") {
            Some(rest) => format!("op_Checked{rest}"),
            None => name.to_owned(),
        };
        let candidates = self
            .ts
            .get_methods(
                declaring,
                None,
                crate::typesystem::lookup::MemberOptions::default(),
            )
            .map_err(|e| e.to_string())?;
        Ok(candidates.iter().any(|c| {
            c.name == checked && Method::from_resolved(self.ts, c).is_some_and(|m| m.is_operator)
        }))
    }

    /// `HandleCompoundShift`: `target op= value` with the value converted to `int`, of the
    /// target's type (`ResolveAssignment` of a predefined shift).
    fn compound_shift(
        &mut self,
        inst: NodeId,
        assignment_op: u8,
        target: TranslatedExpression,
        value: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let target_underlying = self.ts.nullable_underlying(target.ty);
        let code = self.ts.type_code(target_underlying);
        if !((4..=12).contains(&code) || self.is_native_integer(target_underlying)) {
            return Err("a compound shift of these types".to_owned());
        }
        // Shifts take an `int` (an `int?` for a nullable value).
        let v = self.translate(value, None)?;
        let mut int = self.ts.known_type(KnownTypeCode::Int32);
        if self.ts.is_nullable(v.ty) {
            int = self.ts.make_nullable(int);
        }
        let v = self.convert_to(v, int, false)?;
        let e = self.ast.add(AstNode::CompoundAssignment {
            op: assignment_op,
            left: target.expression,
            right: v.expression,
            overflow: Overflow::None,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: target.ty });
        Ok(TranslatedExpression {
            expression: e,
            ty: target.ty,
        })
    }

    /// `VisitUnboxAny`: `expr as T` for `unbox.any T(isinst T(expr))` of a nullable or
    /// reference type, else `(T)arg` with the argument converted to `object`. A type parameter
    /// target, cast through its effective base class where needed, is not translated.
    fn unbox_any(
        &mut self,
        inst: NodeId,
        arg: NodeId,
        ty: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if let Inst::IsInst {
            arg: inner,
            ty: isinst_type,
        } = *self.f.inst(arg)
            && isinst_type.ty == ty
            && (self.ts.is_known(ty, KnownTypeCode::NullableOfT)
                || self.ts.is_reference_type(ty) == Some(true))
        {
            let a = self.translate(inner, None)?;
            let a = self.unwrap_boxing_conversion(a);
            let e = self.ast.add(AstNode::As {
                expression: a.expression,
                ty,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::TryCast { ty });
            return Ok(TranslatedExpression { expression: e, ty });
        }
        let a = self.translate(arg, None)?;
        let object = self.ts.known_type(KnownTypeCode::Object);
        // A type parameter target is cast from the argument as it is, through the effective base
        // class where that cast is an error (C# 6.2.7).
        let a = if self.ts.kind(ty) == TypeKind::TypeParameter {
            let rr = self.semantic_rr(a.expression, a.ty);
            if Conversions::new(self.ts).explicit(&rr, ty).is_valid() {
                a
            } else {
                let base = self
                    .ts
                    .type_parameter_effective_base_class(ty)
                    .ok_or("an unboxing to a type parameter without base class")?;
                self.convert_to(a, base, false)?
            }
        } else {
            self.convert_to(a, object, false)?
        };
        let e = self.ast.add(AstNode::Cast {
            ty,
            expression: a.expression,
            overflow: Overflow::None,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Conversion { ty });
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `HandleShift` without lifting: `left << right` or `left >> right` (`>>>` for an unsigned
    /// shift where C# 11 has it), the left operand cast to the arithmetic type unless it is a
    /// small integer C# promotes, the right one converted to `int`.
    #[allow(clippy::too_many_arguments)]
    fn shift(
        &mut self,
        inst: NodeId,
        is_left: bool,
        sign: Sign,
        left_input: StackType,
        left: NodeId,
        right: NodeId,
        is_lifted: bool,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let l = self.translate(left, None)?;
        let r = self.translate(right, None)?;
        let mut l = self.prepare_lifted_arithmetic_argument(l, left_input, sign, is_lifted)?;
        // The underlying result type: a shift is of its left input's stack type.
        let result_stack = if is_lifted {
            left_input
        } else {
            self.f.result_type(self.ts, inst)
        };
        let nullable_left = self.ts.is_known(l.ty, KnownTypeCode::NullableOfT);
        if nullable_left && !is_lifted {
            return Err("a shift of a nullable value".to_owned());
        }
        let lu = self.ts.nullable_underlying(l.ty);
        let code = self.ts.type_code(lu);
        let primitive_integer = (5..=12).contains(&code) || self.is_native_integer(lu);
        let small = (5..=8).contains(&code);
        let hint_sign = type_hint.map_or(Sign::None, |t| self.ts.type_sign(t));
        let could_use_unsigned_right_shift = sign == Sign::Unsigned
            && !is_left
            && self.settings.unsigned_right_shift
            && primitive_integer
            && hint_sign != Sign::Unsigned;
        let mut op = if is_left {
            BinaryOp::ShiftLeft
        } else {
            BinaryOp::ShiftRight
        };
        if small
            && result_stack == StackType::I4
            && (sign != Sign::Unsigned || could_use_unsigned_right_shift)
        {
            // C# promotes a small integer to `int`; `>>>` shifts it unsigned.
            if sign == Sign::Unsigned {
                op = BinaryOp::UnsignedShiftRight;
            }
        } else if could_use_unsigned_right_shift
            && self.ts.type_size(lu) == stack_size(result_stack)
            && self.ts.type_sign(lu) == Sign::Signed
        {
            op = BinaryOp::UnsignedShiftRight;
        } else {
            let mut s = sign;
            if s == Sign::None {
                s = self.ts.type_sign(lu);
            }
            let mut target = self.find_arithmetic_type(result_stack, s)?;
            if nullable_left {
                target = self.ts.make_nullable(target);
            }
            l = self.convert_to(l, target, false)?;
        }
        // The right operand is an `int` (an `int?` where it is nullable).
        let int = self.ts.known_type(KnownTypeCode::Int32);
        let right_type = if self.ts.is_known(r.ty, KnownTypeCode::NullableOfT) {
            self.ts.make_nullable(int)
        } else {
            int
        };
        let nullable_result = nullable_left || right_type != int;
        let r = self.convert_to(r, right_type, false)?;
        // The type of the predefined shift: the left operand's, `int` for a small integer;
        // a native integer stays (with an error resolve result, as the right operand differs).
        let lu = self.ts.nullable_underlying(l.ty);
        let mut ty = match self.ts.type_code(lu) {
            _ if self.is_native_integer(lu) => lu,
            5..=9 => int,
            10..=12 => lu,
            _ => return Err("a shift of these types".to_owned()),
        };
        if nullable_result {
            if self.is_native_integer(lu) {
                return Err("a lifted shift of a native integer".to_owned());
            }
            ty = self.ts.make_nullable(ty);
        }
        let e = self.ast.add(AstNode::BinaryOperator {
            op,
            left: l.expression,
            right: r.expression,
            overflow: Overflow::None,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::ResolvedOperator { ty });
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `VisitNewArr`: `new T[n]`, with each dimension translated as an array index.
    fn new_arr(
        &mut self,
        inst: NodeId,
        element_type: TypeId,
        indices: &[NodeId],
    ) -> Result<TranslatedExpression, Unsupported> {
        let mut arguments = Vec::with_capacity(indices.len());
        for &i in indices {
            arguments.push(self.translate_array_index(i)?.expression);
        }
        let array_type = self.ts.intern(TypeData::Array {
            elem: element_type,
            rank: indices.len() as u32,
            nullability: crate::typesystem::types::Nullability::Oblivious,
        });
        let e = self.ast.add(AstNode::ArrayCreate {
            element_type,
            arguments,
            initializer: None,
            special_constants: true,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::ArrayCreate { ty: array_type });
        Ok(TranslatedExpression {
            expression: e,
            ty: array_type,
        })
    }

    /// `TranslateArrayIndex`: the index converted to an integer type an array index takes.
    fn translate_array_index(&mut self, i: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let stack = self.f.result_type(self.ts, i);
        let input = self.translate(i, None)?;
        let sign = self.ts.type_sign(input.ty);
        // A truncated oversized index.
        if self.ts.type_size(input.ty) > stack_size(stack) {
            let target = self.find_type(stack, sign);
            return self.convert_to(input, target, false);
        }
        // `IsCSharpPrimitiveIntegerType` (SByte to UInt64 other than Char) and native integers,
        // used as is.
        let code = self.ts.type_code(input.ty);
        if (5..=12).contains(&code) || self.is_native_integer(input.ty) {
            return Ok(input);
        }
        // `int` is preferred where it is big enough.
        let stack = if stack != StackType::I4 && self.ts.stack_type(input.ty) == StackType::I4 {
            StackType::I4
        } else {
            stack
        };
        let target = self.find_arithmetic_type(stack, sign)?;
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
    /// object creation with the initializer `BuildArrayInitializerExpression` makes of the
    /// block.
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
        // `new T()` of a `default(T)` or `Activator.CreateInstance<T>()`, with the type's
        // resolve result.
        let created_type = match self.f.inst(value).clone() {
            Inst::DefaultValue { ty, .. } => Some(ty.ty),
            Inst::Call(c)
                if c.method.method.name == "CreateInstance"
                    && c.args.is_empty()
                    && c.method.method.declaring.is_some_and(|d| {
                        self.ts.definition(d).is_some_and(|def| {
                            self.ts.comp.type_def(def).is_ok_and(|i| {
                                i.full_name.top.ns == "System" && i.full_name.name() == "Activator"
                            })
                        })
                    }) =>
            {
                Method::from_resolved(self.ts, &c.method.method)
                    .and_then(|m| m.method_args)
                    .and_then(|a| (a.len() == 1).then(|| a[0]))
            }
            _ => None,
        };
        let creation = if let Some(ty) = created_type {
            let e = self.ast.add(AstNode::ObjectCreate {
                ty,
                arguments: Vec::new(),
                initializer: None,
            });
            self.ast.with_il(e, value);
            self.ast.with_rr(e, Rr::Type(ty));
            TranslatedExpression { expression: e, ty }
        } else if matches!(self.f.inst(value), Inst::NewObj(_))
            || matches!(
                self.f.inst(value),
                Inst::Block {
                    kind: BlockKind::CallWithNamedArgs,
                    ..
                }
            )
        {
            self.translate(value, None)?
        } else {
            return Err("an initializer of another creation".to_owned());
        };
        if !matches!(
            self.ast.node(creation.expression),
            AstNode::ObjectCreate { .. }
        ) {
            return Err("an initializer of an object creation with syntax of its own".to_owned());
        }
        let elements = self.initializer_elements(rest, creation)?;
        if let AstNode::ObjectCreate { initializer, .. } = self.ast.node_mut(creation.expression) {
            *initializer = Some(elements);
        }
        self.ast.with_il(creation.expression, inst);
        Ok(creation)
    }

    /// `TranslateArrayInitializer`: `new T[n, ...] { { ... }, ... }` from the stores of an
    /// array initializer block into the new array, nested by the constant dimensions. A block
    /// converted to a span is not translated; the host writes an array of anonymous types as
    /// `new[] { ... }`.
    fn array_initializer(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some((&first, rest)) = instructions.split_first() else {
            return Err("an empty array initializer".to_owned());
        };
        // `MatchArrayInitializerFinal`: the array, or its implicit conversion to a span.
        let mut final_inst = final_inst;
        let mut array_to_span = None;
        if let Inst::Call(c) | Inst::CallVirt(c) = self.f.inst(final_inst)
            && let [array] = c.args[..]
            && c.method.method.name == "op_Implicit"
            && Method::from_resolved(self.ts, &c.method.method).is_some_and(|m| m.is_operator)
            && (self
                .ts
                .is_known(c.method.method.ret, KnownTypeCode::SpanOfT)
                || self
                    .ts
                    .is_known(c.method.method.ret, KnownTypeCode::ReadOnlySpanOfT))
        {
            array_to_span = Some(c.method.method.ret);
            final_inst = array;
        }
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
            initializer: Some(root),
            special_constants,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::ArrayCreate { ty: array_type });
        if let Some(span) = array_to_span {
            // shortcut: the cast carries the host's implicit conversion from the array to the
            // span where the managed one is `Conversion.ImplicitSpanConversion` itself; they
            // differ where the host's conversions do not know first-class spans.
            let c = self.ast.add(AstNode::Cast {
                ty: span,
                expression: e,
                overflow: super::ast::Overflow::None,
            });
            self.ast.with_rr(c, Rr::ImplicitConversion { ty: span });
            return Ok(TranslatedExpression {
                expression: c,
                ty: span,
            });
        }
        Ok(TranslatedExpression {
            expression: e,
            ty: array_type,
        })
    }

    /// `WrapInRef`: a call returning a reference is `ref call(...)`, with the
    /// `ByReferenceResolveResult` of the call's.
    pub(crate) fn wrap_in_ref(
        &mut self,
        e: TranslatedExpression,
        ty: TypeId,
    ) -> TranslatedExpression {
        let TypeData::ByRef(element) = *self.ts.data(ty) else {
            return e;
        };
        // The call itself is of the referenced type.
        if let Some(Rr::Invocation { ty, .. } | Rr::Property { ty, .. }) =
            &mut self.ast.annotations_mut(e.expression).rr
        {
            *ty = element;
        }
        let rr = self.rr_of(e.expression);
        let d = self.ast.add(AstNode::RefDirection {
            expression: e.expression,
        });
        self.ast.with_rr(d, Rr::ByReference(Box::new(rr)));
        TranslatedExpression { expression: d, ty }
    }

    /// `VisitMatchInstruction`: `left is pattern`; a boxing cast of the tested value is dropped
    /// where the managed builder drops it.
    fn match_instruction(&mut self, inst: NodeId) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Match {
            variable,
            tested_operand,
            is_deconstruct_call,
            is_deconstruct_tuple,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a match");
        };
        if is_deconstruct_call || is_deconstruct_tuple {
            return Err("a positional pattern".to_owned());
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
        let pattern = self.translate_pattern(inst, left.ty)?;
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

    /// `TranslatePattern(pattern, leftHandType)`: a type, declaration or recursive pattern of a
    /// match, `not`, a relational or a constant pattern of a comparison, or the constant of a
    /// string or decimal equality. Positional patterns are not translated.
    fn translate_pattern(
        &mut self,
        pattern: NodeId,
        left_type: TypeId,
    ) -> Result<AstId, Unsupported> {
        match self.f.inst(pattern).clone() {
            Inst::Match {
                variable,
                sub_patterns,
                is_deconstruct_call,
                is_deconstruct_tuple,
                check_type,
                check_not_null,
                ..
            } => {
                if is_deconstruct_call || is_deconstruct_tuple {
                    return Err("a positional pattern".to_owned());
                }
                let v = self.f.var(variable);
                // `HasDesignator`: the sub-patterns load the variable once each.
                let has_designator = v.load_count() + v.address_count() > sub_patterns.len() as u32;
                let variable_type = v.ty;
                let p = if !sub_patterns.is_empty() || (check_not_null && !check_type) {
                    let mut subs = Vec::with_capacity(sub_patterns.len());
                    for sub in sub_patterns {
                        let tested = self
                            .pattern_tested_operand(sub)
                            .ok_or("an invalid sub-pattern")?;
                        let (member, member_type) = match self.f.inst(tested).clone() {
                            Inst::Call(c) | Inst::CallVirt(c) => {
                                let MethodOrigin::Def(id) = c.method.method.origin else {
                                    return Err("a sub-pattern of an unresolved method".to_owned());
                                };
                                if self.ts.accessor_owner(id).is_none() {
                                    return Err("an invalid sub-pattern".to_owned());
                                }
                                let getter = Method::from_resolved(self.ts, &c.method.method)
                                    .ok_or("a sub-pattern of an unresolved method")?;
                                (PatternMember::Property(c.method), getter.return_type)
                            }
                            _ => {
                                let (_, field) = self
                                    .f
                                    .match_ldfld(self.ts, tested)
                                    .ok_or("an invalid sub-pattern")?;
                                let field = field.clone();
                                let ty = field.field.ty;
                                (PatternMember::Field(field), ty)
                            }
                        };
                        let pattern = self.translate_pattern(sub, member_type)?;
                        subs.push(SubPattern { member, pattern });
                    }
                    self.ast.add(AstNode::RecursivePattern {
                        ty: check_type.then_some(variable_type),
                        sub_patterns: subs,
                        designator: has_designator.then_some(variable),
                    })
                } else if has_designator || !check_type {
                    self.ast.add(AstNode::DeclarationPattern {
                        variable,
                        var_keyword: !check_type,
                    })
                } else {
                    self.ast.add(AstNode::TypeReference { ty: variable_type })
                };
                Ok(self.ast.with_il(p, pattern))
            }
            Inst::Comp { kind, right, .. } => {
                if let Some(operand) = self.f.match_logic_not(pattern)
                    && self.pattern_tested_operand(operand).is_some()
                {
                    // `logic.not` as a pattern
                    let sub = self.translate_pattern(operand, left_type)?;
                    if let AstNode::PatternUnary {
                        operator: PatternOperator::Not,
                        pattern: inner,
                    } = *self.ast.node(sub)
                    {
                        return Ok(self.ast.with_il(inner, pattern));
                    }
                    let p = self.ast.add(AstNode::PatternUnary {
                        operator: PatternOperator::Not,
                        pattern: sub,
                    });
                    return Ok(self.ast.with_il(p, pattern));
                }
                let constant = if let Inst::DefaultValue { .. } = self.f.inst(right) {
                    // Not `(int?)null`: the null literal itself.
                    let null_type = self.ts.intern(TypeData::Null);
                    let n = self.ast.add(AstNode::Null);
                    self.ast.with_rr(
                        n,
                        Rr::Constant {
                            ty: null_type,
                            value: ConstValue::Null,
                        },
                    );
                    n
                } else {
                    self.translate(right, Some(left_type))?.expression
                };
                let operator = match kind {
                    ComparisonKind::Equality => return Ok(self.ast.with_il(constant, pattern)),
                    ComparisonKind::Inequality => PatternOperator::Not,
                    ComparisonKind::LessThan => PatternOperator::LessThan,
                    ComparisonKind::LessThanOrEqual => PatternOperator::LessThanOrEqual,
                    ComparisonKind::GreaterThan => PatternOperator::GreaterThan,
                    ComparisonKind::GreaterThanOrEqual => PatternOperator::GreaterThanOrEqual,
                };
                let p = self.ast.add(AstNode::PatternUnary {
                    operator,
                    pattern: constant,
                });
                Ok(self.ast.with_il(p, pattern))
            }
            // `IsCallToOpEquality` of string or decimal: the constant.
            Inst::Call(c) if c.args.len() == 2 => Ok(self.translate(c.args[1], None)?.expression),
            _ => Err("a pattern of another kind".to_owned()),
        }
    }

    /// The tested operand of a sub-pattern (`MatchInstruction.IsPatternMatch`, whose checks the
    /// pattern matching transform made).
    fn pattern_tested_operand(&self, pattern: NodeId) -> Option<NodeId> {
        match *self.f.inst(pattern) {
            Inst::Match { tested_operand, .. } => Some(tested_operand),
            Inst::Comp { left, .. } => match self.f.match_logic_not(pattern) {
                Some(operand) => self.pattern_tested_operand(operand).or(Some(left)),
                None => Some(left),
            },
            Inst::Call(ref c) if c.args.len() == 2 && c.method.method.name == "op_Equality" => {
                Some(c.args[0])
            }
            _ => None,
        }
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
                overflow: Overflow::None,
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
            // `new PrimitiveExpression(0m)`.
            Some(KnownTypeCode::Decimal) => {
                let value = ConstValue::Decimal([0; 4]);
                let e = self.ast.add(AstNode::ConstantValue {
                    ty,
                    value: value.clone(),
                    allow_implicit_conversion: true,
                });
                self.ast.with_rr(e, Rr::Constant { ty, value });
                return Ok(TranslatedExpression { expression: e, ty });
            }
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
    pub(crate) fn unwrap_boxing_conversion(
        &mut self,
        e: TranslatedExpression,
    ) -> TranslatedExpression {
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

    /// `VisitConv`: the argument as is
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
        if target == PrimitiveType::Unknown {
            return Err("a conversion to an unknown type".to_owned());
        }
        let hint_sign = if input_sign == Sign::None {
            self.ts.type_sign(hint)
        } else {
            input_sign
        };
        let arg_hint = self.find_arithmetic_type(input, hint_sign)?;
        let mut a = self.translate(arg, Some(arg_hint))?;
        let input_type = self.ts.nullable_underlying(a.ty);
        let kind = crate::il::ast::conversion_kind(target, input, input_sign);
        // `GetType`: the known type, `nint` or `nuint` rather than `IntPtr` or `UIntPtr` with
        // native integers unless the type hint is that type.
        // A lifted conversion is to the nullable of the type.
        let get_type = |b: &mut Self, code: KnownTypeCode| {
            let mut t = b.ts.known_type(code);
            if b.settings.native_integers && t != hint {
                match code {
                    KnownTypeCode::IntPtr => t = b.ts.intern(TypeData::NInt),
                    KnownTypeCode::UIntPtr => t = b.ts.intern(TypeData::NUInt),
                    _ => {}
                }
            }
            if is_lifted {
                t = b.ts.make_nullable(t);
            }
            t
        };
        // The argument converted to the stack type of the given sign first.
        let to_input = |b: &mut Self, a: TranslatedExpression, sign: Sign| {
            let t = get_type(b, input.to_known(sign));
            b.convert_to(a, t, false)
        };
        let oversized = |b: &mut Self| b.ts.type_size(input_type) > stack_size(input);
        if check_overflow || kind == ConversionKind::IntToFloat {
            // The argument of the expected sign, narrowed to the input stack type, so that the
            // narrowing does not mix with the overflow check; a checked cast then has the C#
            // semantics of the input type.
            if oversized(self) || self.ts.type_sign(input_type) != input_sign {
                a = to_input(self, a, input_sign)?;
            }
            let t = get_type(self, target.to_known());
            let c = self.convert_to_checked(a, t, check_overflow, false)?;
            self.ast.with_il(c.expression, inst);
            return Ok(c);
        }
        match kind {
            ConversionKind::StartGCTracking => return Ok(a),
            ConversionKind::StopGCTracking => {
                if let TypeData::ByRef(element) = *self.ts.data(input_type) {
                    return self.stop_gc_tracking(inst, arg, a, element);
                }
                // Tracking stops for an integer it has just started for.
                if self.ts.stack_type(a.ty).is_integer() {
                    return Ok(a);
                }
            }
            ConversionKind::SignExtend => {
                if self.ts.type_sign(input_type) != Sign::Signed
                    || self.value_might_be_oversized(a.expression, input_type, input)
                {
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
            // An unknown value where an object is expected stays as it is: the post-condition
            // allows that, and a cast to object would likely be cast back.
            ConversionKind::Invalid
                if input == StackType::Unknown
                    && target == PrimitiveType::None
                    && self.ts.kind(a.ty) == TypeKind::Unknown =>
            {
                self.ast.with_il(a.expression, inst);
                return Ok(a);
            }
            _ => {}
        }
        // The type hint where it is the target type, else the target's type: a reference to
        // byte for an unknown reference, object for an object type.
        let hint_underlying = self.ts.nullable_underlying(hint);
        let target_type = if self.ts.is_nullable(hint) == is_lifted
            && self.ts.primitive_type(hint_underlying) == target
        {
            hint
        } else if target == PrimitiveType::Ref {
            let byte = self.ts.known_type(KnownTypeCode::Byte);
            self.ts.intern(TypeData::ByRef(byte))
        } else if target == PrimitiveType::None {
            self.ts.known_type(KnownTypeCode::Object)
        } else {
            get_type(self, target.to_known())
        };
        let c = self.convert_to(a, target_type, false)?;
        self.ast.with_il(c.expression, inst);
        Ok(c)
    }

    /// `FindArithmeticType`: the type of the stack type and sign, `nint` or `nuint` for native
    /// integers (64-bit integers without them).
    pub(crate) fn find_arithmetic_type(
        &mut self,
        stack: StackType,
        sign: Sign,
    ) -> Result<TypeId, Unsupported> {
        let unsigned = sign == Sign::Unsigned;
        Ok(match stack {
            StackType::I if self.settings.native_integers => self.ts.intern(if unsigned {
                TypeData::NUInt
            } else {
                TypeData::NInt
            }),
            StackType::I => self.ts.known_type(StackType::I8.to_known(sign)),
            StackType::Unknown => self.ts.intern(TypeData::UnknownSpecial),
            StackType::Ref => {
                let byte = self.ts.known_type(KnownTypeCode::Byte);
                self.ts.intern(TypeData::ByRef(byte))
            }
            // `KnownTypeCode.None` finds the unknown type.
            _ => match stack.to_known(sign) {
                KnownTypeCode::None => self.ts.intern(TypeData::UnknownSpecial),
                code => self.ts.known_type(code),
            },
        })
    }

    /// `ValueMightBeOversized`: whether a value of `ty` may not fit the stack type; a pointer
    /// difference (of type long) always fits a native integer.
    fn value_might_be_oversized(&mut self, e: AstId, ty: TypeId, stack: StackType) -> bool {
        if self.ts.type_size(ty) <= stack_size(stack) {
            return false;
        }
        if stack == StackType::I
            && let AstNode::BinaryOperator {
                op: BinaryOp::Subtract,
                left,
                right,
                ..
            } = *self.ast.node(e)
        {
            let pointer = |b: &mut Self, operand: AstId| {
                let rr = b.rr_of(operand);
                let t = b.rr_type(&rr);
                b.ts.kind(t) == TypeKind::Pointer
            };
            if pointer(self, left) && pointer(self, right) {
                return false;
            }
        }
        true
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

/// An operand of `BinaryNumericPromotion` next to a native integer: its type code (0 for
/// `nint` and `nuint`), which native integer it is, and whether it is a non-negative `int` or
/// `long` constant (which converts to the unsigned types).
#[derive(Clone, Copy)]
struct PromotionOperand {
    code: u8,
    native: Option<TypeKind>,
    nonnegative_constant: bool,
}

/// The type `native_promotion` promotes both operands to.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum NativePromotion {
    /// The operands' own type, which they share.
    Operands,
    Known(KnownTypeCode),
    NInt,
    NUInt,
}

/// `BinaryNumericPromotion` and the native integer operators of `ResolveBinaryOperator` where
/// an operand is `nint` or `nuint`, which promote between `int` and `long` (as `int` and
/// `uint`): `None` where no operator applies (an unsigned 64-bit or native operand with a
/// signed one, or a native integer next to another type it does not promote with).
fn native_promotion(
    l: PromotionOperand,
    r: PromotionOperand,
    same_type: bool,
) -> Result<Option<NativePromotion>, Unsupported> {
    const INT32: u8 = 9;
    const UINT32: u8 = 10;
    const INT64: u8 = 11;
    const UINT64: u8 = 12;
    let code = |o: PromotionOperand| match o.native {
        Some(TypeKind::NInt) => INT32,
        Some(_) => UINT32,
        None => o.code,
    };
    let (lc, rc) = (code(l), code(r));
    // `IsSigned`: a non-negative int or long constant counts as unsigned, an nint never.
    let signed = |o: PromotionOperand, c: u8| match c {
        5 | 7 => true,
        INT32 | INT64 => o.native.is_some() || !o.nonnegative_constant,
        _ => false,
    };
    let any_signed = signed(l, lc) || signed(r, rc);
    let either = |c: u8| lc == c || rc == c;
    let nuint = l.native == Some(TypeKind::NUInt) || r.native == Some(TypeKind::NUInt);
    Ok(if !(4..=15).contains(&lc) || !(4..=15).contains(&rc) {
        // No promotion: the native operator applies only to two operands of one type.
        same_type.then_some(NativePromotion::Operands)
    } else if either(15) {
        return Err("a decimal operation with a native integer".to_owned());
    } else if either(14) {
        Some(NativePromotion::Known(KnownTypeCode::Double))
    } else if either(13) {
        Some(NativePromotion::Known(KnownTypeCode::Single))
    } else if either(UINT64) {
        (!any_signed).then_some(NativePromotion::Known(KnownTypeCode::UInt64))
    } else if nuint {
        (!any_signed).then_some(NativePromotion::NUInt)
    } else if either(UINT32) {
        Some(NativePromotion::Known(if any_signed {
            KnownTypeCode::Int64
        } else {
            KnownTypeCode::UInt32
        }))
    } else if either(INT64) {
        Some(NativePromotion::Known(KnownTypeCode::Int64))
    } else {
        Some(NativePromotion::NInt)
    })
}

/// `BinaryNumericPromotion` for `==` and `!=` over two numeric type codes (`Char` to `Decimal`):
/// the type code of the operator's operands, `None` where no predefined operator applies (an
/// `ulong` with a signed operand). Decimal comparisons are not translated.
fn numeric_promotion(lc: u8, rc: u8) -> Result<Option<u8>, Unsupported> {
    const DECIMAL: u8 = 15;
    const DOUBLE: u8 = 14;
    const SINGLE: u8 = 13;
    const UINT64: u8 = 12;
    const INT64: u8 = 11;
    const UINT32: u8 = 10;
    const INT32: u8 = 9;
    // SByte, Int16, Int32, Int64
    let signed = |c: u8| matches!(c, 5 | 7 | 9 | 11);
    let either = |c: u8| lc == c || rc == c;
    let other = |c: u8| if lc == c { rc } else { lc };
    if either(DECIMAL) {
        return Err("a decimal comparison".to_owned());
    }
    Ok(Some(if either(DOUBLE) {
        DOUBLE
    } else if either(SINGLE) {
        SINGLE
    } else if either(UINT64) {
        if signed(other(UINT64)) {
            return Ok(None);
        }
        UINT64
    } else if either(INT64) {
        INT64
    } else if either(UINT32) {
        if signed(other(UINT32)) { INT64 } else { UINT32 }
    } else {
        INT32
    }))
}

/// The `KnownTypeCode` of a primitive `TypeCode` from `Char` to `Double`.
fn known_of_type_code(code: u8) -> KnownTypeCode {
    match code {
        4 => KnownTypeCode::Char,
        5 => KnownTypeCode::SByte,
        6 => KnownTypeCode::Byte,
        7 => KnownTypeCode::Int16,
        8 => KnownTypeCode::UInt16,
        9 => KnownTypeCode::Int32,
        10 => KnownTypeCode::UInt32,
        11 => KnownTypeCode::Int64,
        12 => KnownTypeCode::UInt64,
        13 => KnownTypeCode::Single,
        _ => KnownTypeCode::Double,
    }
}

#[cfg(test)]
mod tests {
    use super::{NativePromotion, PromotionOperand, native_promotion};
    use crate::semantics::types::TypeKind;
    use crate::typesystem::known::KnownTypeCode;

    const NINT: PromotionOperand = PromotionOperand {
        code: 0,
        native: Some(TypeKind::NInt),
        nonnegative_constant: false,
    };
    const NUINT: PromotionOperand = PromotionOperand {
        code: 0,
        native: Some(TypeKind::NUInt),
        nonnegative_constant: false,
    };

    fn of(code: u8) -> PromotionOperand {
        PromotionOperand {
            code,
            native: None,
            nonnegative_constant: false,
        }
    }

    fn promote(l: PromotionOperand, r: PromotionOperand) -> Option<NativePromotion> {
        native_promotion(l, r, false).unwrap()
    }

    #[test]
    fn native_integers_promote_between_int_and_long() {
        use NativePromotion::*;
        // int and smaller go to nint; long and floating point win; uint with nint is long.
        assert_eq!(promote(NINT, of(9)), Some(NInt));
        assert_eq!(promote(of(5), NINT), Some(NInt));
        assert_eq!(promote(NINT, of(11)), Some(Known(KnownTypeCode::Int64)));
        assert_eq!(promote(NINT, of(10)), Some(Known(KnownTypeCode::Int64)));
        assert_eq!(promote(NINT, of(14)), Some(Known(KnownTypeCode::Double)));
        assert_eq!(promote(NUINT, of(13)), Some(Known(KnownTypeCode::Single)));
        // nuint takes unsigned operands and non-negative constants only.
        assert_eq!(promote(NUINT, of(10)), Some(NUInt));
        assert_eq!(promote(NUINT, of(9)), None);
        let constant = PromotionOperand {
            nonnegative_constant: true,
            ..of(9)
        };
        assert_eq!(promote(NUINT, constant), Some(NUInt));
        assert_eq!(promote(NUINT, NINT), None);
        assert_eq!(promote(NINT, of(12)), None);
        // No promotion with bool: only an operand of the same type has an operator.
        assert_eq!(promote(NINT, of(3)), None);
        assert_eq!(
            native_promotion(NINT, NINT, true).unwrap(),
            Some(NativePromotion::NInt)
        );
        assert!(native_promotion(NINT, of(15), false).is_err());
    }
}
