//! The unsafe code of `ExpressionBuilder` and `StatementBuilder`: pointer dereferences and
//! addresses, `sizeof`, the `Unsafe` intrinsics and `fixed`.

use crate::il::ast::{BinaryNumericOperator, Inst, NodeId};
use crate::il::types::StackType;
use crate::metadata::tables::rows;
use crate::semantics::types::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `FieldAttributes.Static`.
const FIELD_STATIC: u16 = 0x0010;

impl Builder<'_, '_, '_, '_> {
    /// `new UnaryOperatorExpression(Dereference, pointer).WithRR(new ResolveResult(ty))`.
    pub(crate) fn dereference(&mut self, pointer: AstId, ty: TypeId) -> TranslatedExpression {
        let e = self.ast.add(AstNode::PointerUnary {
            address_of: false,
            expression: pointer,
        });
        self.ast.with_rr(e, Rr::Plain(ty));
        TranslatedExpression { expression: e, ty }
    }

    /// `new UnaryOperatorExpression(AddressOf, operand)`, without annotations.
    pub(crate) fn address_of_node(&mut self, operand: AstId) -> AstId {
        self.ast.add(AstNode::PointerUnary {
            address_of: true,
            expression: operand,
        })
    }

    /// `settings.IntroduceUnmanagedConstraint`, which the type system options carry.
    fn introduce_unmanaged_constraint(&self) -> bool {
        self.current_type.is_some_and(|d| {
            self.ts.comp.module(d.module).options
                & crate::typesystem::options::UNMANAGED_CONSTRAINTS
                != 0
        })
    }

    /// `type.IsUnmanagedType(settings.IntroduceUnmanagedConstraint)`.
    pub(crate) fn is_unmanaged_type(&mut self, t: TypeId) -> bool {
        let allow_generics = self.introduce_unmanaged_constraint();
        let mut visiting = Vec::new();
        self.is_unmanaged_type_internal(t, allow_generics, &mut visiting)
    }

    fn is_unmanaged_type_internal(
        &mut self,
        t: TypeId,
        allow_generics: bool,
        visiting: &mut Vec<TypeId>,
    ) -> bool {
        let kind = self.ts.kind(t);
        if matches!(
            kind,
            TypeKind::Enum
                | TypeKind::Pointer
                | TypeKind::FunctionPointer
                | TypeKind::NInt
                | TypeKind::NUInt
        ) {
            return true;
        }
        let mut param = t;
        while let TypeData::Annotated { inner, .. } = *self.ts.data(param) {
            param = inner;
        }
        if let TypeData::TypeParam { owner, index } = *self.ts.data(param) {
            return self
                .ts
                .type_parameter_info(owner, index)
                .is_some_and(|i| i.unmanaged);
        }
        let Some(def) = self.ts.definition(t) else {
            return false;
        };
        let Ok(info) = self.ts.comp.type_def(def) else {
            return false;
        };
        use KnownTypeCode as K;
        if matches!(
            info.known,
            K::Void
                | K::Boolean
                | K::Char
                | K::SByte
                | K::Byte
                | K::Int16
                | K::UInt16
                | K::Int32
                | K::UInt32
                | K::Int64
                | K::UInt64
                | K::Decimal
                | K::Single
                | K::Double
                | K::IntPtr
                | K::UIntPtr
                | K::TypedReference
                | K::RuntimeArgumentHandle
        ) {
            return true;
        }
        if kind != TypeKind::Struct {
            return false;
        }
        if !allow_generics && info.type_parameter_count > 0 {
            return false;
        }
        let class_args = match self.ts.data(t) {
            TypeData::Inst { args, .. } => Some(args.to_vec()),
            _ => None,
        };
        let Ok(fields) = self.ts.comp.visible_fields(def) else {
            return false;
        };
        visiting.push(t);
        let mut result = true;
        for row in fields {
            let is_static = self
                .ts
                .comp
                .module(def.module)
                .md
                .get::<rows::Field>(row)
                .is_ok_and(|f| f.flags & FIELD_STATIC != 0);
            if is_static {
                continue;
            }
            let (declared, _) = self.ts.field_type(def.module, row);
            let ty = match &class_args {
                Some(args) => self.ts.substitute(declared, Some(args), None),
                None => declared,
            };
            if visiting.contains(&ty)
                || !self.is_unmanaged_type_internal(ty, allow_generics, visiting)
            {
                result = false;
                break;
            }
        }
        visiting.pop();
        result
    }

    /// `CallUnsafeIntrinsic(name, arguments, returnType, inst, typeArguments)`.
    pub(crate) fn call_unsafe_intrinsic(
        &mut self,
        name: &str,
        arguments: Vec<AstId>,
        return_type: TypeId,
        inst: Option<NodeId>,
        type_arguments: Vec<TypeId>,
    ) -> TranslatedExpression {
        let unsafe_type = self.ts.known_type(KnownTypeCode::Unsafe);
        let e = self.ast.add(AstNode::UnsafeIntrinsic {
            unsafe_type,
            name: name.to_owned(),
            type_arguments,
            arguments,
        });
        if let Some(inst) = inst {
            self.ast.with_il(e, inst);
        }
        match *self.ts.data(return_type) {
            TypeData::ByRef(element) => {
                self.ast.with_rr(e, Rr::Plain(element));
                let invocation = TranslatedExpression {
                    expression: e,
                    ty: element,
                };
                self.wrap_in_ref(invocation, return_type)
            }
            _ => {
                self.ast.with_rr(e, Rr::Plain(return_type));
                TranslatedExpression {
                    expression: e,
                    ty: return_type,
                }
            }
        }
    }

    /// `ConvertTo` from a reference to a pointer: `&*ptr` is `ptr`, else `&element`, then the
    /// remaining pointer cast.
    pub(crate) fn convert_reference_to_pointer(
        &mut self,
        e: TranslatedExpression,
        target: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let AstNode::RefDirection { expression: arg } = *self.ast.node(e.expression) else {
            return Err("a conversion of a reference to a pointer".to_owned());
        };
        let TypeData::ByRef(element) = *self.ts.data(e.ty) else {
            return Err("a conversion of a reference to a pointer".to_owned());
        };
        if let AstNode::PointerUnary {
            address_of: false,
            expression,
        } = *self.ast.node(arg)
        {
            let pointer = self.unwrap_child(arg, expression);
            return self.convert_to(pointer, target, false);
        }
        let pointer_type = self.ts.intern(TypeData::Pointer(element));
        let p = self.address_of_node(arg);
        for inst in self.ast.annotations(e.expression).il.clone() {
            self.ast.with_il(p, inst);
        }
        self.ast.with_rr(p, Rr::Plain(pointer_type));
        let pointer = TranslatedExpression {
            expression: p,
            ty: pointer_type,
        };
        self.convert_to(pointer, target, false)
    }

    /// The ref (re-)assignment of `VisitStLoc` and `VisitStObj`: `ref (target = value)`, the
    /// value converted to the reference type of the `ref target` on the left.
    pub(crate) fn ref_reassignment(
        &mut self,
        left: TranslatedExpression,
        value: TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        let AstNode::RefDirection { expression } = *self.ast.node(left.expression) else {
            unreachable!("a ref expression");
        };
        let left_rr = self.rr_of(left.expression);
        let ref_type = self.rr_type(&left_rr);
        let target = self.unwrap_child(left.expression, expression);
        let value = self.convert_to(value, ref_type, true)?;
        let assign = self.ast.add(AstNode::Assignment {
            left: target.expression,
            right: value.expression,
        });
        self.ast.with_rr(assign, Rr::Operator { ty: target.ty });
        let d = self.ast.add(AstNode::RefDirection { expression: assign });
        self.ast.with_rr(d, left_rr);
        Ok(TranslatedExpression {
            expression: d,
            ty: ref_type,
        })
    }

    /// `StatementBuilder.VisitPinnedRegion`: `fixed (T* v = init) body`.
    pub(crate) fn pinned_region(
        &mut self,
        inst: NodeId,
        variable: crate::il::ast::VarId,
        init: NodeId,
        body: NodeId,
    ) -> Result<AstId, Unsupported> {
        let var_type = self.f.var(variable).ty;
        let initializer =
            if let Inst::GetPinnableReference { arg, method, .. } = self.f.inst(init).clone() {
                match method {
                    Some(m) => {
                        let m = &m.method;
                        let expected = if m.is_static {
                            *m.params()
                                .map_err(|e| e.to_string())?
                                .first()
                                .ok_or("a GetPinnableReference without parameters")?
                        } else {
                            m.declaring.ok_or("a method without declaring type")?
                        };
                        let a = self.translate(arg, Some(expected))?;
                        self.convert_to(a, expected, false)?.expression
                    }
                    None => self.translate(arg, None)?.expression,
                }
            } else {
                let ref_type = match *self.ts.data(var_type) {
                    TypeData::Pointer(element) => self.ts.intern(TypeData::ByRef(element)),
                    _ => var_type,
                };
                let e = self.translate(init, Some(ref_type))?;
                let mut init_expr = self.convert_to(e, ref_type, false)?.expression;
                if let AstNode::RefDirection { expression } = *self.ast.node(init_expr) {
                    init_expr = match *self.ast.node(expression) {
                        AstNode::PointerUnary {
                            address_of: false,
                            expression: pointer,
                        } => pointer,
                        _ => {
                            let a = self.address_of_node(expression);
                            self.ast.with_rr(a, Rr::Plain(var_type));
                            a
                        }
                    };
                }
                let init_rr = self.rr_of(init_expr);
                let init_type = self.rr_type(&init_rr);
                if matches!(self.ts.data(init_type), TypeData::Pointer(_))
                    && let TypeData::ByRef(element) = *self.ts.data(ref_type)
                {
                    // `IsFixedSizeBuffer`
                    let fixed_size_buffer = match &init_rr {
                        Rr::Field { field, .. } | Rr::TupleElement { field, .. } => {
                            let field = field.clone();
                            self.fixed_field_element(&field).is_some()
                        }
                        _ => false,
                    };
                    if !fixed_size_buffer && !self.is_address_of_moveable_var(init_expr) {
                        // C# does not pin an unmanaged pointer: `&Unsafe.AsRef<T>(pointer)`.
                        let as_ref = self
                            .call_unsafe_intrinsic(
                                "AsRef",
                                vec![init_expr],
                                element,
                                None,
                                vec![element],
                            )
                            .expression;
                        init_expr = self.address_of_node(as_ref);
                        self.ast.with_rr(init_expr, Rr::Plain(var_type));
                    }
                }
                init_expr
            };
        let body = self.convert(body)?;
        let s = self.ast.add(AstNode::Fixed {
            variable,
            initializer,
            body,
        });
        Ok(self.ast.with_il(s, inst))
    }

    /// `IsAddressOfMoveableVar`: `&x` of an expression whose instruction is no fixed variable.
    fn is_address_of_moveable_var(&mut self, e: AstId) -> bool {
        let AstNode::PointerUnary {
            address_of: true,
            expression,
        } = *self.ast.node(e)
        else {
            return false;
        };
        match self.ast.annotations(expression).il.first() {
            Some(&inst) => !self.is_fixed_variable(inst),
            None => true,
        }
    }

    /// `PointerArithmeticOffset.IsFixedVariable`: an uncaptured local's address, a field of
    /// one, or a native integer.
    fn is_fixed_variable(&mut self, inst: NodeId) -> bool {
        match *self.f.inst(inst) {
            Inst::LdLoca { var } => self.f.var(var).capture_scope.is_none(),
            Inst::LdFlda { target, .. } => self.is_fixed_variable(target),
            _ => self.f.result_type(self.ts, inst) == StackType::I,
        }
    }

    /// `ConvertTo` from an integer or pointer to the reference `ref element`: the pointer
    /// cast, then `ref x` of an address `&x`, else `ref *pointer`.
    pub(crate) fn convert_pointer_to_reference(
        &mut self,
        e: TranslatedExpression,
        element: TypeId,
        check: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        let pointer_type = self.ts.intern(TypeData::Pointer(element));
        let arg = self.convert_to_checked(e, pointer_type, check, false)?;
        let expr = match *self.ast.node(arg.expression) {
            AstNode::PointerUnary {
                address_of: true,
                expression,
            } => self.unwrap_child(arg.expression, expression),
            _ => self.dereference(arg.expression, element),
        };
        let element_rr = self.rr_of(expr.expression);
        let d = self.ast.add(AstNode::RefDirection {
            expression: expr.expression,
        });
        self.ast.with_rr(d, Rr::ByReference(Box::new(element_rr)));
        let ty = self.ts.intern(TypeData::ByRef(element));
        Ok(TranslatedExpression { expression: d, ty })
    }

    /// `CSharpDecompiler.IsFixedField`: the element type of a fixed size buffer field.
    pub(crate) fn fixed_field_element(
        &mut self,
        field: &crate::il::ast::FieldOperand,
    ) -> Option<TypeId> {
        use crate::typesystem::attrvalue::ArgValue;
        let crate::typesystem::resolve::FieldOrigin::Def { module, row } = field.field.origin
        else {
            return None;
        };
        let attributes = self.ts.field_attributes(module, row).ok()?;
        attributes.iter().find_map(|a| {
            let (name, ns) = self.ts.name_and_namespace(a.ty)?;
            if name != "FixedBufferAttribute" || ns != "System.Runtime.CompilerServices" {
                return None;
            }
            match a.fixed.as_slice() {
                [element, length] => match (&element.value, &length.value) {
                    (ArgValue::Type(t), ArgValue::Int32(_)) => Some(*t),
                    _ => None,
                },
                _ => None,
            }
        })
    }

    /// `VisitLdFlda` of the `FixedElementField` of a fixed size buffer: `target.buffer` as a
    /// pointer to the element type; as a reference, converted back where the context makes a
    /// pointer of it again (a `fixed` initializer or a conversion to `nuint`), else
    /// `ref target.buffer[0]`.
    pub(crate) fn fixed_buffer_element(
        &mut self,
        inst: NodeId,
        buffer_target: NodeId,
        buffer: &crate::il::ast::FieldOperand,
        element: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let access = self.convert_field(buffer, Some(buffer_target))?;
        let Rr::Field {
            target: Some(target),
            field,
        } = self.rr_of(access.expression)
        else {
            return Err("a fixed size buffer without target".to_owned());
        };
        let pointer_type = self.ts.intern(TypeData::Pointer(element));
        // The `MemberResolveResult` of the field with the pointer type as return type override.
        self.ast.with_rr(
            access.expression,
            Rr::TupleElement {
                target,
                field,
                ty: pointer_type,
            },
        );
        self.ast.with_il(access.expression, inst);
        let result = TranslatedExpression {
            expression: access.expression,
            ty: pointer_type,
        };
        if self.f.result_type(self.ts, inst) != StackType::Ref {
            return Ok(result);
        }
        let parent = self.f.node(inst).parent;
        let in_pin_init = parent.is_some_and(
            |p| matches!(*self.f.inst(p), Inst::PinnedRegion { init, .. } if init == inst),
        );
        let in_conv_to_u = parent.is_some_and(|p| {
            matches!(
                *self.f.inst(p),
                Inst::Conv {
                    target: crate::il::types::PrimitiveType::U,
                    ..
                }
            )
        });
        if in_pin_init || in_conv_to_u {
            let reference = self.ts.intern(TypeData::ByRef(element));
            return self.convert_to(result, reference, false);
        }
        // shortcut: the index 0 is a constant with its resolve result where the managed
        // builder writes a bare `PrimitiveExpression`; the output is the same, upgrade when a
        // transform reads the annotation.
        let int = self.ts.known_type(KnownTypeCode::Int32);
        let zero = self.ast.add(AstNode::ConstantValue {
            ty: int,
            value: crate::typesystem::members::ConstValue::Int32(0),
            allow_implicit_conversion: false,
        });
        self.ast.with_rr(
            zero,
            Rr::Constant {
                ty: int,
                value: crate::typesystem::members::ConstValue::Int32(0),
            },
        );
        let index = self.ast.add(AstNode::Indexer {
            target: result.expression,
            indices: vec![zero],
        });
        self.ast.with_rr(index, Rr::Plain(element));
        let d = self.ast.add(AstNode::RefDirection { expression: index });
        self.ast
            .with_rr(d, Rr::ByReference(Box::new(Rr::Plain(element))));
        let ty = self.ts.intern(TypeData::ByRef(element));
        Ok(TranslatedExpression { expression: d, ty })
    }

    /// `VisitConv` with `StopGCTracking` of a reference: a cast to the pointer type for a
    /// fixed variable, else `Unsafe.AsPointer(ref x)`.
    pub(crate) fn stop_gc_tracking(
        &mut self,
        inst: NodeId,
        arg: NodeId,
        a: TranslatedExpression,
        element: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if self.is_fixed_variable(arg) {
            let pointer_type = self.ts.intern(TypeData::Pointer(element));
            let c = self.convert_to(a, pointer_type, false)?;
            self.ast.with_il(c.expression, inst);
            return Ok(c);
        }
        let void = self.ts.known_type(KnownTypeCode::Void);
        let void_pointer = self.ts.intern(TypeData::Pointer(void));
        Ok(self.call_unsafe_intrinsic(
            "AsPointer",
            vec![a.expression],
            void_pointer,
            Some(inst),
            Vec::new(),
        ))
    }

    /// `EnsureIntegerType`: a pointer arithmetic offset of a C# integer type, else converted
    /// to the arithmetic type of its stack type.
    fn ensure_integer_type(
        &mut self,
        e: TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        use KnownTypeCode as K;
        let primitive = matches!(
            self.ts.known_type_code(e.ty),
            K::Byte | K::SByte | K::Int16 | K::UInt16 | K::Int32 | K::UInt32 | K::Int64 | K::UInt64
        );
        if primitive || matches!(self.ts.kind(e.ty), TypeKind::NInt | TypeKind::NUInt) {
            return Ok(e);
        }
        let stack = self.ts.stack_type(e.ty);
        let sign = self.ts.type_sign(e.ty);
        let target = self.find_arithmetic_type(stack, sign)?;
        self.convert_to(e, target, false)
    }

    /// `GetPointerArithmeticOffset`: the offset in elements of `element` that the byte offset
    /// stands for, `None` where it is no multiple of the element size.
    pub(crate) fn pointer_arithmetic_offset(
        &mut self,
        byte_offset_inst: NodeId,
        byte_offset: TranslatedExpression,
        element: TypeId,
        check_for_overflow: bool,
        unwrap_zero_extension: bool,
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        use crate::il::transforms::transform_assignment::{
            PointerOffset, pointer_offset_detect_in,
        };
        let Some(count) = pointer_offset_detect_in(
            self.f,
            self.ts,
            byte_offset_inst,
            element,
            check_for_overflow,
            unwrap_zero_extension,
        ) else {
            return Ok(None);
        };
        let e = match count {
            PointerOffset::Inst(i) if i == byte_offset_inst => byte_offset,
            PointerOffset::Inst(i) => {
                let e = self.translate(i, None)?;
                // The original instruction stays as annotation.
                let a = self.ast.annotations_mut(e.expression);
                a.il.clear();
                a.il_before_rr = false;
                self.ast.with_il(e.expression, byte_offset_inst);
                e
            }
            PointerOffset::Constant(v) => {
                let unknown = self.ts.intern(TypeData::UnknownSpecial);
                self.ldc_i4(byte_offset_inst, v, unknown)?
            }
        };
        self.ensure_integer_type(e).map(Some)
    }

    /// `HandlePointerArithmetic`: `ptr + n`, `n + ptr` or `ptr - n` with an offset `n` in
    /// elements; `None` where the operation is no pointer arithmetic.
    #[allow(clippy::too_many_arguments)]
    pub(crate) fn pointer_arithmetic(
        &mut self,
        inst: NodeId,
        op: BinaryNumericOperator,
        check_overflow: bool,
        is_lifted: bool,
        inputs: (StackType, StackType),
        operands: (NodeId, NodeId),
        left: TranslatedExpression,
        right: TranslatedExpression,
        type_hint: Option<TypeId>,
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        let add = op == BinaryNumericOperator::Add;
        if !(add || op == BinaryNumericOperator::Sub) || check_overflow || is_lifted {
            return Ok(None);
        }
        if inputs != (StackType::I, StackType::I) {
            return Ok(None);
        }
        let left_pointer = matches!(self.ts.data(left.ty), TypeData::Pointer(_));
        let (byte_offset_inst, byte_offset, mut pointer_type) = if left_pointer {
            (operands.1, right, left.ty)
        } else if matches!(self.ts.data(right.ty), TypeData::Pointer(_)) {
            if !add {
                return Ok(None);
            }
            (operands.0, left, right.ty)
        } else {
            return Ok(None);
        };
        let element = match *self.ts.data(pointer_type) {
            TypeData::Pointer(e) => e,
            _ => unreachable!("a pointer type"),
        };
        let mut offset = None;
        if let Some(hint) = type_hint
            && let TypeData::Pointer(hint_element) = *self.ts.data(hint)
        {
            // The type hint's element type where the pointer's is a struct of unknown size or
            // of another size than the hint's (not `char*` as `short*`).
            let size = self.ts.type_size(element);
            if size == 0 || self.ts.type_size(hint_element) != size {
                offset = self.pointer_arithmetic_offset(
                    byte_offset_inst,
                    byte_offset,
                    hint_element,
                    check_overflow,
                    false,
                )?;
                if offset.is_some() {
                    pointer_type = hint;
                }
            }
        }
        if offset.is_none() {
            let element = match *self.ts.data(pointer_type) {
                TypeData::Pointer(e) => e,
                _ => unreachable!("a pointer type"),
            };
            offset = self.pointer_arithmetic_offset(
                byte_offset_inst,
                byte_offset,
                element,
                check_overflow,
                false,
            )?;
        }
        let offset = match offset {
            Some(o) => o,
            None => {
                // `FallBackToBytePointer`
                let byte = self.ts.known_type(KnownTypeCode::Byte);
                pointer_type = self.ts.intern(TypeData::Pointer(byte));
                self.ensure_integer_type(byte_offset)?
            }
        };
        let (l, r) = if left_pointer {
            (self.convert_to(left, pointer_type, false)?, offset)
        } else {
            (offset, self.convert_to(right, pointer_type, false)?)
        };
        let e = self.ast.add(AstNode::BinaryOperator {
            op: if add {
                super::ast::BinaryOp::Add
            } else {
                super::ast::BinaryOp::Subtract
            },
            left: l.expression,
            right: r.expression,
            overflow: super::ast::Overflow::None,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Operator { ty: pointer_type });
        Ok(Some(TranslatedExpression {
            expression: e,
            ty: pointer_type,
        }))
    }

    /// `HandlePointerSubtraction`: `div(sub(a, b), sizeof(T))` of `T*` operands, which the C#
    /// compiler generates for `a - b`; `None` where the division is no pointer subtraction.
    pub(crate) fn pointer_subtraction(
        &mut self,
        inst: NodeId,
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        let Inst::BinaryNumeric {
            check_overflow: false,
            left_input: StackType::I,
            left: sub,
            right,
            ..
        } = *self.f.inst(inst)
        else {
            return Ok(None);
        };
        let Inst::BinaryNumeric {
            op: BinaryNumericOperator::Sub,
            check_overflow: false,
            left: sub_left,
            right: sub_right,
            ..
        } = *self.f.inst(sub)
        else {
            return Ok(None);
        };
        let (element_size, element_type) = match self.f.match_ldc_i(right) {
            Some(size) => (size, None),
            None => {
                let unwrapped = crate::il::transforms::expression_transforms::unwrap_conv(
                    self.f,
                    right,
                    crate::il::ast::ConversionKind::SignExtend,
                );
                match *self.f.inst(unwrapped) {
                    Inst::SizeOf { ty } => (0, Some(ty.ty)),
                    _ => return Ok(None),
                }
            }
        };
        let left = self.translate(sub_left, None)?;
        let right = self.translate(sub_right, None)?;
        let is_matching = |b: &mut Self, t: TypeId| match *b.ts.data(t) {
            TypeData::Pointer(element) => match element_type {
                Some(e) => e == element,
                None => {
                    element_size > 0
                        && crate::il::transforms::transform_assignment::compute_size_of(
                            b.ts, element,
                        ) == Some(element_size)
                }
            },
            _ => false,
        };
        let pointer_type = if is_matching(self, left.ty) {
            left.ty
        } else if is_matching(self, right.ty) {
            right.ty
        } else if element_size == 1
            && self.ts.kind(left.ty) == TypeKind::Pointer
            && self.ts.kind(right.ty) == TypeKind::Pointer
        {
            // Two pointers of other types divided by 1 (debug builds): byte pointers.
            let byte = self.ts.known_type(KnownTypeCode::Byte);
            self.ts.intern(TypeData::Pointer(byte))
        } else {
            return Ok(None);
        };
        let left = self.convert_to(left, pointer_type, false)?;
        let right = self.convert_to(right, pointer_type, false)?;
        let long = self.ts.known_type(KnownTypeCode::Int64);
        let e = self.ast.add(AstNode::BinaryOperator {
            op: super::ast::BinaryOp::Subtract,
            left: left.expression,
            right: right.expression,
            overflow: super::ast::Overflow::None,
        });
        self.ast.with_il(e, inst);
        self.ast.with_il(e, sub);
        self.ast.with_rr(e, Rr::Operator { ty: long });
        Ok(Some(TranslatedExpression {
            expression: e,
            ty: long,
        }))
    }

    /// `CreateBuiltinBinaryOperator`: the comparison with the `OperatorResolveResult` of `bool`
    /// over the operands.
    pub(crate) fn builtin_comparison(
        &mut self,
        left: TranslatedExpression,
        op: super::ast::BinaryOp,
        right: TranslatedExpression,
    ) -> TranslatedExpression {
        let boolean = self.ts.known_type(KnownTypeCode::Boolean);
        let e = self.ast.add(AstNode::BinaryOperator {
            overflow: super::ast::Overflow::None,
            op,
            left: left.expression,
            right: right.expression,
        });
        self.ast.with_rr(e, Rr::Operator { ty: boolean });
        TranslatedExpression {
            expression: e,
            ty: boolean,
        }
    }

    /// `new NullReferenceExpression()` with the null constant's resolve result, for the
    /// instruction `inst`.
    pub(crate) fn null_literal(&mut self, inst: NodeId) -> TranslatedExpression {
        let null = self.ts.intern(TypeData::Null);
        let e = self.ast.add(AstNode::Null);
        self.ast.with_rr(
            e,
            Rr::Constant {
                ty: null,
                value: crate::typesystem::members::ConstValue::Null,
            },
        );
        self.ast.with_il(e, inst);
        TranslatedExpression {
            expression: e,
            ty: null,
        }
    }

    /// `TranslateLocAlloc`: `stackalloc T[count]`, the element type from a `count * sizeof(T)`
    /// size, else from the pointer type the context expects, else `byte`.
    fn translate_loc_alloc(
        &mut self,
        arg: NodeId,
        type_hint: Option<TypeId>,
    ) -> Result<(AstNode, TypeId), Unsupported> {
        use crate::il::ast::ConversionKind;
        use crate::il::transforms::expression_transforms::unwrap_conv;
        let int = self.ts.known_type(KnownTypeCode::Int32);
        let sized = match *self.f.inst(arg) {
            Inst::BinaryNumeric {
                op: BinaryNumericOperator::Mul,
                left,
                right,
                ..
            } => {
                let r = unwrap_conv(self.f, right, ConversionKind::SignExtend);
                let r = unwrap_conv(self.f, r, ConversionKind::ZeroExtend);
                match *self.f.inst(r) {
                    Inst::SizeOf { ty } => Some((left, ty.ty)),
                    _ => None,
                }
            }
            _ => None,
        };
        let (count, element) = if let Some((left, element)) = sized {
            let left = unwrap_conv(self.f, left, ConversionKind::ZeroExtend);
            (self.translate(left, None)?, element)
        } else {
            let pointer_element = match type_hint.map(|t| self.ts.data(t).clone()) {
                Some(TypeData::Pointer(e)) => Some(e),
                _ => None,
            };
            let mut found = None;
            if let Some(e) = pointer_element {
                let byte_offset = self.translate(arg, None)?;
                if let Some(offset) =
                    self.pointer_arithmetic_offset(arg, byte_offset, e, true, true)?
                {
                    found = Some((offset, e));
                }
            }
            match found {
                Some(f) => f,
                None => {
                    let byte = self.ts.known_type(KnownTypeCode::Byte);
                    (self.translate(arg, None)?, byte)
                }
            }
        };
        let count = self.convert_to(count, int, false)?;
        Ok((
            AstNode::StackAlloc {
                ty: element,
                count: Some(count.expression),
                initializer: None,
            },
            element,
        ))
    }

    /// `TranslateLocAllocSpan`: `stackalloc T[count]` of the span's element type.
    fn translate_loc_alloc_span(
        &mut self,
        arg: NodeId,
        span: TypeId,
    ) -> Result<(AstNode, TypeId), Unsupported> {
        let TypeData::Inst { args, .. } = self.ts.data(span).clone() else {
            return Err("a stackalloc span of no generic type".to_owned());
        };
        let element = *args.first().ok_or("a span without type argument")?;
        let int = self.ts.known_type(KnownTypeCode::Int32);
        let count = self.translate(arg, None)?;
        let count = self.convert_to(count, int, false)?;
        Ok((
            AstNode::StackAlloc {
                ty: element,
                count: Some(count.expression),
                initializer: None,
            },
            element,
        ))
    }

    /// `VisitLocAlloc` (`span` `None`) and `VisitLocAllocSpan`.
    pub(crate) fn loc_alloc(
        &mut self,
        inst: NodeId,
        arg: NodeId,
        span: Option<TypeId>,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let (node, ty) = match span {
            None => {
                let (node, element) = self.translate_loc_alloc(arg, type_hint)?;
                (node, self.ts.intern(TypeData::Pointer(element)))
            }
            Some(span) => (self.translate_loc_alloc_span(arg, span)?.0, span),
        };
        let e = self.ast.add(node);
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(ty));
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `TranslateStackAllocInitializer`: `stackalloc T[n] { elements }` of a
    /// `StackAllocInitializer` block, as a pointer or (ending in the span constructor) a span.
    pub(crate) fn stack_alloc_initializer(
        &mut self,
        block: NodeId,
        type_hint: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        use crate::il::ast::ConversionKind;
        use crate::il::transforms::expression_transforms::unwrap_conv;
        use crate::il::transforms::transform_assignment::{
            PointerOffset, pointer_offset_detect_in,
        };
        let invalid = || "a stackalloc initializer of another shape".to_owned();
        let Inst::Block {
            instructions,
            final_inst,
            ..
        } = self.f.inst(block).clone()
        else {
            return Err(invalid());
        };
        let stloc = instructions.first().copied().ok_or_else(invalid)?;
        let Inst::StLoc {
            var: target_var,
            value: allocation,
            ..
        } = *self.f.inst(stloc)
        else {
            return Err(invalid());
        };
        let mut type_hint = type_hint;
        let mut result_type = None;
        let final_var = match self.f.inst(final_inst).clone() {
            Inst::LdLoc { var } => var,
            Inst::NewObj(c)
                if c.args.len() == 2
                    && c.method.method.declaring.is_some_and(|d| {
                        self.ts.is_known(d, KnownTypeCode::SpanOfT)
                            || self.ts.is_known(d, KnownTypeCode::ReadOnlySpanOfT)
                    }) =>
            {
                let Inst::LdLoc { var } = *self.f.inst(c.args[0]) else {
                    return Err(invalid());
                };
                let span = c.method.method.declaring.expect("a span type");
                result_type = Some(span);
                // The pointer type of the span's element type.
                if let TypeData::Inst { args, .. } = self.ts.data(span).clone()
                    && let Some(&element) = args.first()
                {
                    type_hint = Some(self.ts.intern(TypeData::Pointer(element)));
                }
                var
            }
            _ => return Err(invalid()),
        };
        if target_var != final_var
            || self.f.var(target_var).kind != crate::il::ast::VariableKind::InitializerTarget
        {
            return Err(invalid());
        }
        let Some(&Inst::StObj { ty: first_type, .. }) =
            instructions.get(1).map(|&i| self.f.inst(i))
        else {
            return Err(invalid());
        };
        // The element type of the stores where the hint is no compatible pointer type.
        let compatible_hint = match type_hint.map(|t| self.ts.data(t).clone()) {
            Some(TypeData::Pointer(e)) => self
                .ts
                .is_compatible_type_for_memory_access(first_type.ty, e),
            _ => false,
        };
        if !compatible_hint {
            type_hint = Some(self.ts.intern(TypeData::Pointer(first_type.ty)));
        }
        let (node, element) = match self.f.inst(allocation).clone() {
            Inst::LocAlloc { arg } => self.translate_loc_alloc(arg, type_hint)?,
            Inst::LocAllocSpan { arg, ty } => self.translate_loc_alloc_span(arg, ty)?,
            _ => return Err(invalid()),
        };
        let mut elements = Vec::new();
        for (expected_offset, &store) in (0i64..).zip(&instructions[1..]) {
            let Inst::StObj {
                target, value, ty, ..
            } = *self.f.inst(store)
            else {
                return Err(invalid());
            };
            if !self.ts.is_compatible_type_for_memory_access(element, ty.ty) {
                return Err(invalid());
            }
            let target = unwrap_conv(self.f, target, ConversionKind::StopGCTracking);
            let mut offset = 0;
            if !matches!(*self.f.inst(target), Inst::LdLoc { var } if var == target_var) {
                let Inst::BinaryNumeric {
                    op: BinaryNumericOperator::Add,
                    check_overflow,
                    left,
                    right,
                    ..
                } = *self.f.inst(target)
                else {
                    return Err(invalid());
                };
                let left = unwrap_conv(self.f, left, ConversionKind::StopGCTracking);
                if !matches!(*self.f.inst(left), Inst::LdLoc { var } if var == final_var) {
                    return Err(invalid());
                }
                offset = match pointer_offset_detect_in(
                    self.f,
                    self.ts,
                    right,
                    element,
                    check_overflow,
                    false,
                ) {
                    Some(PointerOffset::Constant(v)) => i64::from(v),
                    Some(PointerOffset::Inst(i)) => self.f.match_ldc_i(i).ok_or_else(invalid)?,
                    None => return Err(invalid()),
                };
            }
            if expected_offset < offset {
                // shortcut: the default elements of a gap are not written (the managed builder
                // translates `GetNullExpression` instructions outside the function); upgrade
                // when a stackalloc initializer with gaps reaches the engine.
                return Err("a stackalloc initializer with gaps".to_owned());
            }
            let v = self.translate(value, Some(element))?;
            let v = self.convert_to(v, element, true)?;
            elements.push(v.expression);
        }
        let AstNode::StackAlloc { ty, count, .. } = node else {
            unreachable!("a stackalloc expression");
        };
        let e = self.ast.add(AstNode::StackAlloc {
            ty,
            count,
            initializer: Some(elements),
        });
        let ty = result_type.unwrap_or(self.f.var(target_var).ty);
        self.ast.with_il(e, block);
        self.ast.with_rr(e, Rr::Plain(ty));
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `VisitArglist`, `VisitMakeRefAny`, `VisitRefAnyType` and `VisitRefAnyValue`: the
    /// undocumented `__arglist`, `__makeref(x)`, `__reftype(r).TypeHandle` and
    /// `ref __refvalue(r, T)`.
    pub(crate) fn undocumented(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        /// `UndocumentedExpressionType`.
        const ARG_LIST_ACCESS: u8 = 0;
        const REF_VALUE: u8 = 2;
        const REF_TYPE: u8 = 3;
        const MAKE_REF: u8 = 4;
        let (kind, arguments, code) = match self.f.inst(inst).clone() {
            Inst::Arglist => (
                ARG_LIST_ACCESS,
                Vec::new(),
                KnownTypeCode::RuntimeArgumentHandle,
            ),
            Inst::MakeRefAny { arg, .. } => {
                let a = self.translate(arg, None)?.expression;
                let a = match *self.ast.node(a) {
                    AstNode::RefDirection { expression } => expression,
                    _ => a,
                };
                (MAKE_REF, vec![a], KnownTypeCode::TypedReference)
            }
            Inst::RefAnyType { arg } => {
                let a = self.translate(arg, None)?.expression;
                let u = self.ast.add(AstNode::Undocumented {
                    kind: REF_TYPE,
                    arguments: vec![a],
                });
                let handle = self.ts.known_type(KnownTypeCode::RuntimeTypeHandle);
                let e = self.ast.add(AstNode::MemberReference {
                    target: u,
                    name: "TypeHandle".to_owned(),
                });
                self.ast.with_il(e, inst);
                self.ast.with_rr(e, Rr::Type(handle));
                return Ok(TranslatedExpression {
                    expression: e,
                    ty: handle,
                });
            }
            Inst::RefAnyValue { arg, ty } => {
                let a = self.translate(arg, None)?.expression;
                let t = self.ast.add(AstNode::TypeReference { ty: ty.ty });
                let u = self.ast.add(AstNode::Undocumented {
                    kind: REF_VALUE,
                    arguments: vec![a, t],
                });
                self.ast.with_rr(u, Rr::Plain(ty.ty));
                self.ast.with_il(u, inst);
                let d = self.ast.add(AstNode::RefDirection { expression: u });
                self.ast
                    .with_rr(d, Rr::ByReference(Box::new(Rr::Plain(ty.ty))));
                let by_ref = self.ts.intern(TypeData::ByRef(ty.ty));
                return Ok(TranslatedExpression {
                    expression: d,
                    ty: by_ref,
                });
            }
            _ => unreachable!("an undocumented instruction"),
        };
        let ty = self.ts.known_type(code);
        let e = self.ast.add(AstNode::Undocumented { kind, arguments });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Type(ty));
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `VisitCallIndirect`: `fp(arguments)` of a static function pointer signature, with a
    /// plain resolve result of the return type (`ref` around a by-reference return).
    pub(crate) fn call_indirect(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        use crate::typesystem::fnptr::{REF_IN, REF_NONE, REF_OUT, REF_READONLY};
        let Inst::CallIndirect {
            is_instance,
            signature,
            function_pointer,
            args,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a calli");
        };
        if is_instance {
            return Err("a calli with an instance method signature".to_owned());
        }
        let mut fp = self.translate(function_pointer, Some(signature))?;
        let n = crate::typesystem::fnptr::Normalization::TYPE_ERASURE;
        if self.ts.normalize_with(fp.ty, n) != self.ts.normalize_with(signature, n) {
            fp = self.convert_to(fp, signature, false)?;
        }
        let pointer = self.ts.skip_modifiers(fp.ty);
        let TypeData::FnPtr(fpt) = self.ts.data(pointer).clone() else {
            return Err("a calli of no function pointer type".to_owned());
        };
        let mut arguments = Vec::with_capacity(args.len());
        for (i, &arg) in args.iter().enumerate() {
            let (Some(&param), Some(&ref_kind)) = (fpt.params.get(i), fpt.ref_kinds.get(i)) else {
                break;
            };
            let a = self.translate(arg, Some(param))?;
            let mut a = self.convert_to(a, param, true)?;
            let is_address_of = matches!(self.f.inst(arg), Inst::AddressOf { .. });
            a = match ref_kind {
                REF_NONE => a,
                REF_OUT => self.change_direction_to_out(a),
                REF_IN | REF_READONLY => {
                    self.change_direction_to_in(a, is_address_of, ref_kind == REF_READONLY)
                }
                _ => a,
            };
            arguments.push(a.expression);
        }
        let e = self.ast.add(AstNode::Invocation {
            target: fp.expression,
            arguments,
        });
        let ret = self.ts.skip_modifiers(fpt.ret);
        if let TypeData::ByRef(element) = *self.ts.data(ret) {
            self.ast.with_rr(e, Rr::Plain(element));
            self.ast.with_il(e, inst);
            let d = self.ast.add(AstNode::RefDirection { expression: e });
            self.ast
                .with_rr(d, Rr::ByReference(Box::new(Rr::Plain(element))));
            let ty = self.ts.intern(TypeData::ByRef(element));
            return Ok(TranslatedExpression { expression: d, ty });
        }
        self.ast.with_rr(e, Rr::Plain(fpt.ret));
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: fpt.ret,
        })
    }

    /// `VisitSizeOf`: `sizeof(T)` of an unmanaged type, else `Unsafe.SizeOf<T>()`.
    pub(crate) fn size_of(
        &mut self,
        inst: NodeId,
        ty: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let int = self.ts.known_type(KnownTypeCode::Int32);
        if self.is_unmanaged_type(ty) {
            let e = self.ast.add(AstNode::SizeOf { ty });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Plain(int));
            return Ok(TranslatedExpression {
                expression: e,
                ty: int,
            });
        }
        Ok(self.call_unsafe_intrinsic("SizeOf", Vec::new(), int, Some(inst), vec![ty]))
    }
}
