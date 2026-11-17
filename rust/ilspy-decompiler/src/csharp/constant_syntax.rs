//! `ExpressionBuilder.ConvertConstantValue(rr, allowImplicitConversion, displayAsHex)` over
//! `TypeSystemAstBuilder.ConvertConstantValue` as the expression builder configures it: a
//! constant as the expression that writes it, with the resolve results the managed builders
//! annotate.

use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{ModuleId, TypeDefId, TypeKind};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// A field the syntax names, as its module and `FieldDef` row.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub struct FieldRef {
    pub module: ModuleId,
    pub row: u32,
}

/// The resolve result annotated on a node.
#[derive(Clone, Debug, PartialEq)]
pub enum ConstRr {
    /// `ConstantResolveResult(type, value)`.
    Constant(TypeId, ConstValue),
    /// `ConstantResolveResult(SpecialType.NullType, null)`.
    NullLiteral,
    /// `TypeOfResolveResult(type, referencedType)`.
    TypeOf(TypeId, TypeId),
}

#[derive(Clone, Debug, PartialEq)]
pub enum ConstSyntaxKind {
    /// `PrimitiveExpression(value)`, in hexadecimal where `hex` is set.
    Literal { value: ConstValue, hex: bool },
    /// `NullReferenceExpression`.
    Null,
    /// `DefaultValueExpression(ConvertType(type))`.
    Default(TypeId),
    /// `CastExpression(ConvertType(type), expression)`.
    Cast(TypeId, Box<ConstSyntax>),
    /// `CastExpression(new PrimitiveType(keyword), expression)`, without a type annotation.
    PrimitiveCast(&'static str, Box<ConstSyntax>),
    /// `Type.Member` of a special constant: the type reference annotated with
    /// `TypeResolveResult(type)`, the member reference with `MemberResolveResult` on it.
    SpecialConstant {
        ty: TypeId,
        field: FieldRef,
        name: String,
    },
    /// `Enum.Member`: the member reference's `MemberResolveResult` has the type reference's
    /// resolve result as target, which is unannotated (`ErrorResolveResult.UnknownError`).
    EnumMember {
        ty: TypeId,
        field: FieldRef,
        name: String,
    },
    /// `Member` of an enum member initializer: an identifier annotated with
    /// `MemberResolveResult(null, field)`.
    EnumMemberIdentifier { field: FieldRef, name: String },
    /// `typeof(T)`.
    TypeOf(TypeId),
    /// `new T[] { elements }` of an attribute's array argument: the array type's syntax with its
    /// array specifiers moved after the element type.
    ArrayCreate(TypeId, Vec<ConstSyntax>),
    /// `left | right`.
    BitwiseOr(Box<ConstSyntax>, Box<ConstSyntax>),
    /// `left / right`.
    Divide(Box<ConstSyntax>, Box<ConstSyntax>),
    /// `left * right`.
    Multiply(Box<ConstSyntax>, Box<ConstSyntax>),
    /// `Math.PI` or `Math.E`: the type reference unannotated (its type syntax carries the
    /// `TypeResolveResult`), the member reference with `MemberResolveResult` where the field
    /// exists.
    MathMember {
        ty: TypeId,
        field: Option<FieldRef>,
        name: &'static str,
    },
    /// `-expression`.
    Minus(Box<ConstSyntax>),
    /// `~expression`.
    BitNot(Box<ConstSyntax>),
}

#[derive(Clone, Debug, PartialEq)]
pub struct ConstSyntax {
    pub kind: ConstSyntaxKind,
    pub rr: Option<ConstRr>,
}

impl ConstSyntax {
    fn new(kind: ConstSyntaxKind) -> Self {
        ConstSyntax { kind, rr: None }
    }

    fn with(kind: ConstSyntaxKind, rr: ConstRr) -> Self {
        ConstSyntax { kind, rr: Some(rr) }
    }
}

/// `ReflectionHelper.GetTypeCode`-ordered integer kinds the conversions work with.
pub(crate) fn as_i64(v: &ConstValue) -> Option<i64> {
    Some(match *v {
        ConstValue::Boolean(b) => i64::from(b),
        ConstValue::Char(c) => i64::from(c),
        ConstValue::SByte(x) => i64::from(x),
        ConstValue::Byte(x) => i64::from(x),
        ConstValue::Int16(x) => i64::from(x),
        ConstValue::UInt16(x) => i64::from(x),
        ConstValue::Int32(x) => i64::from(x),
        ConstValue::UInt32(x) => i64::from(x),
        ConstValue::Int64(x) => x,
        ConstValue::UInt64(x) => x as i64,
        _ => return None,
    })
}

/// `CSharpPrimitiveCast.Cast(code, value, checkForOverflow: false)` of an integer.
fn cast_integer(code: KnownTypeCode, v: i64) -> Option<ConstValue> {
    Some(match code {
        KnownTypeCode::Boolean => ConstValue::Boolean(v != 0),
        KnownTypeCode::Char => ConstValue::Char(v as u16),
        KnownTypeCode::SByte => ConstValue::SByte(v as i8),
        KnownTypeCode::Byte => ConstValue::Byte(v as u8),
        KnownTypeCode::Int16 => ConstValue::Int16(v as i16),
        KnownTypeCode::UInt16 => ConstValue::UInt16(v as u16),
        KnownTypeCode::Int32 => ConstValue::Int32(v as i32),
        KnownTypeCode::UInt32 => ConstValue::UInt32(v as u32),
        KnownTypeCode::Int64 => ConstValue::Int64(v),
        KnownTypeCode::UInt64 => ConstValue::UInt64(v as u64),
        _ => return None,
    })
}

/// `specialConstants`: the member of a known type that names the value, and whether it is
/// negated (only for `-Epsilon`, which the floating point conversion handles).
fn special_constant(v: &ConstValue) -> Option<(KnownTypeCode, &'static str)> {
    Some(match *v {
        ConstValue::Byte(u8::MAX) => (KnownTypeCode::Byte, "MaxValue"),
        ConstValue::SByte(i8::MIN) => (KnownTypeCode::SByte, "MinValue"),
        ConstValue::SByte(i8::MAX) => (KnownTypeCode::SByte, "MaxValue"),
        ConstValue::Int16(i16::MIN) => (KnownTypeCode::Int16, "MinValue"),
        ConstValue::Int16(i16::MAX) => (KnownTypeCode::Int16, "MaxValue"),
        ConstValue::UInt16(u16::MAX) => (KnownTypeCode::UInt16, "MaxValue"),
        ConstValue::Int32(i32::MIN) => (KnownTypeCode::Int32, "MinValue"),
        ConstValue::Int32(i32::MAX) => (KnownTypeCode::Int32, "MaxValue"),
        ConstValue::UInt32(u32::MAX) => (KnownTypeCode::UInt32, "MaxValue"),
        ConstValue::Int64(i64::MIN) => (KnownTypeCode::Int64, "MinValue"),
        ConstValue::Int64(i64::MAX) => (KnownTypeCode::Int64, "MaxValue"),
        ConstValue::UInt64(u64::MAX) => (KnownTypeCode::UInt64, "MaxValue"),
        // `decimal.MaxValue` and `MinValue`: all 96 bits set, scale 0.
        ConstValue::Decimal([u32::MAX, u32::MAX, u32::MAX, 0]) => {
            (KnownTypeCode::Decimal, "MaxValue")
        }
        ConstValue::Decimal([u32::MAX, u32::MAX, u32::MAX, 0x8000_0000]) => {
            (KnownTypeCode::Decimal, "MinValue")
        }
        _ => return None,
    })
}

/// The builder of a message's constant syntax.
pub struct ConstantSyntaxBuilder<'t, 'c, 'a> {
    ts: &'t TypeSystem<'c, 'a>,
}

impl<'t, 'c, 'a> ConstantSyntaxBuilder<'t, 'c, 'a> {
    pub fn new(ts: &'t TypeSystem<'c, 'a>) -> Self {
        ConstantSyntaxBuilder { ts }
    }

    fn known_def(&self, code: KnownTypeCode) -> Option<TypeDefId> {
        let name = crate::typesystem::names::FullTypeName::top_level(code.top_level_name()?);
        let comp = self.ts.comp;
        comp.module_ids().find_map(|m| {
            comp.find_type_definition(m, &name, &mut Vec::new())
                .ok()
                .flatten()
        })
    }

    fn known_code(&self, t: TypeId) -> KnownTypeCode {
        match self.ts.data(t) {
            TypeData::Def(d) => self
                .ts
                .comp
                .type_def(*d)
                .map_or(KnownTypeCode::None, |i| i.known),
            _ => KnownTypeCode::None,
        }
    }

    /// The field of `def` with the given name.
    fn field_named(&self, def: TypeDefId, name: &str) -> Option<FieldRef> {
        let md = &self.ts.comp.module(def.module).md;
        let found: Vec<u32> = md
            .type_fields(def.row)
            .ok()?
            .into_iter()
            .filter(|&f| {
                md.get::<rows::Field>(f)
                    .ok()
                    .and_then(|r| md.string(r.name).ok())
                    .is_some_and(|n| n == name)
            })
            .collect();
        // `SingleOrDefault`
        match found[..] {
            [row] => Some(FieldRef {
                module: def.module,
                row,
            }),
            _ => None,
        }
    }

    /// `ExpressionBuilder.ConvertConstantValue(new ConstantResolveResult(ty, value),
    /// allowImplicitConversion, displayAsHex)`. `None` for a floating point value, or a null
    /// of a type parameter's type, which the host converts itself (counted).
    pub fn convert(
        &self,
        ty: TypeId,
        value: &ConstValue,
        allow_implicit_conversion: bool,
        hex: bool,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        let mut expr = self.convert_constant_value(ty, ty, value, hex, special_constants)?;
        if !allow_implicit_conversion {
            let ts = self.ts;
            if matches!(expr.kind, ConstSyntaxKind::Null) && !matches!(ts.data(ty), TypeData::Null)
            {
                expr = ConstSyntax::new(ConstSyntaxKind::Cast(ty, Box::new(expr)));
            } else if let Some(k) = self.small_integer_keyword(ty) {
                expr = ConstSyntax::new(ConstSyntaxKind::PrimitiveCast(k, Box::new(expr)));
            } else if matches!(ts.data(ty), TypeData::NInt) {
                expr = ConstSyntax::new(ConstSyntaxKind::PrimitiveCast("nint", Box::new(expr)));
            } else if matches!(ts.data(ty), TypeData::NUInt) {
                expr = ConstSyntax::new(ConstSyntaxKind::PrimitiveCast("nuint", Box::new(expr)));
            }
        }
        // The expression's own resolve result, or the constant's.
        if expr.rr.is_none() {
            expr.rr = Some(ConstRr::Constant(ty, value.clone()));
        }
        Some(expr)
    }

    /// Whether a null of `t` is the null literal rather than `default(T)`.
    // shortcut: a type parameter's reference type question needs its constraints, which the
    // writer's read-only type system does not compute; such a null stays with the host.
    fn null_is_literal(&self, t: TypeId) -> Option<bool> {
        let ts = self.ts;
        Some(match ts.data(t) {
            TypeData::Def(d) => !matches!(
                ts.comp.type_def(*d).ok()?.kind,
                TypeKind::Struct | TypeKind::Enum | TypeKind::Void
            ),
            TypeData::Unknown {
                is_reference_type, ..
            } => *is_reference_type == Some(true),
            TypeData::Inst { generic, .. } => {
                ts.is_known(*generic, KnownTypeCode::NullableOfT)
                    || self.null_is_literal(*generic)?
            }
            TypeData::Annotated { inner, .. }
            | TypeData::Modified { inner, .. }
            | TypeData::Pinned(inner) => self.null_is_literal(*inner)?,
            TypeData::Array { .. } | TypeData::Dynamic | TypeData::Null => true,
            TypeData::Pointer(_) | TypeData::FnPtr(_) => true,
            TypeData::TypeParam { .. } | TypeData::Intersection(_) => return None,
            _ => false,
        })
    }

    /// `ConvertConstantValue(expectedType, type, value)` of an attribute argument: a constant,
    /// `typeof`, or an array of arguments.
    pub fn convert_argument(
        &self,
        expected: TypeId,
        arg: &crate::typesystem::attrvalue::TypedArg,
    ) -> Option<ConstSyntax> {
        use crate::typesystem::attrvalue::ArgValue as A;
        let ty = arg.ty;
        let value = match &arg.value {
            A::Type(t) => {
                return Some(ConstSyntax::with(
                    ConstSyntaxKind::TypeOf(*t),
                    ConstRr::TypeOf(ty, *t),
                ));
            }
            A::Array(elements) => {
                let element_type = match self.ts.data(ty) {
                    TypeData::Array { elem, .. } => Some(*elem),
                    _ => None,
                };
                let mut converted = Vec::with_capacity(elements.len());
                for e in elements {
                    // `(type as ArrayType)?.ElementType ?? SpecialType.UnknownType`
                    let expected =
                        element_type.or_else(|| self.ts.find(&TypeData::UnknownSpecial))?;
                    converted.push(self.convert_argument(expected, e)?);
                }
                return Some(ConstSyntax::new(ConstSyntaxKind::ArrayCreate(
                    ty, converted,
                )));
            }
            A::Null => ConstValue::Null,
            A::Bool(v) => ConstValue::Boolean(*v),
            A::Char(v) => ConstValue::Char(*v),
            A::SByte(v) => ConstValue::SByte(*v),
            A::Byte(v) => ConstValue::Byte(*v),
            A::Int16(v) => ConstValue::Int16(*v),
            A::UInt16(v) => ConstValue::UInt16(*v),
            A::Int32(v) => ConstValue::Int32(*v),
            A::UInt32(v) => ConstValue::UInt32(*v),
            A::Int64(v) => ConstValue::Int64(*v),
            A::UInt64(v) => ConstValue::UInt64(*v),
            A::Single(v) => ConstValue::Single(*v),
            A::Double(v) => ConstValue::Double(*v),
            A::String(v) => ConstValue::String(v.clone()),
            A::Decimal(v) => ConstValue::Decimal(*v),
        };
        self.convert_constant_value(expected, ty, &value, false, true)
    }

    /// `TypeSystemAstBuilder.ConvertConstantValue(type, value)` without the expression
    /// builder's casts: a field's constant initializer.
    pub fn convert_raw(
        &self,
        ty: TypeId,
        value: &ConstValue,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        self.convert_constant_value(ty, ty, value, false, special_constants)
    }

    /// `IsSpecialConstant(underlyingType, value)` of an integer or decimal value: the member of
    /// the value's own type (`int.MaxValue`), which may differ from the expected one (an `int`
    /// in the constant of a `float` parameter), where its field exists. Not for hexadecimal
    /// integer literals.
    fn special_integer_or_decimal(
        &self,
        underlying: TypeId,
        code: KnownTypeCode,
        value: &ConstValue,
        hex: bool,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        let integer = matches!(
            code,
            KnownTypeCode::SByte
                | KnownTypeCode::Byte
                | KnownTypeCode::Int16
                | KnownTypeCode::UInt16
                | KnownTypeCode::Int32
                | KnownTypeCode::UInt32
                | KnownTypeCode::Int64
                | KnownTypeCode::UInt64
        );
        if hex && integer || !special_constants {
            return None;
        }
        let (known, member) = special_constant(value)?;
        let def = self.known_def(known)?;
        let field = self.field_named(def, member)?;
        let ty = if known == code {
            underlying
        } else {
            self.ts.find(&TypeData::Def(def))?
        };
        Some(ConstSyntax::new(ConstSyntaxKind::SpecialConstant {
            ty,
            field,
            name: member.to_owned(),
        }))
    }

    /// `IsCSharpSmallIntegerType`, as its keyword.
    fn small_integer_keyword(&self, t: TypeId) -> Option<&'static str> {
        Some(match self.known_code(t) {
            KnownTypeCode::Byte => "byte",
            KnownTypeCode::SByte => "sbyte",
            KnownTypeCode::Int16 => "short",
            KnownTypeCode::UInt16 => "ushort",
            _ => return None,
        })
    }

    /// `TypeSystemAstBuilder.ConvertConstantValue(type, type, constantValue)`.
    fn convert_constant_value(
        &self,
        expected: TypeId,
        ty: TypeId,
        value: &ConstValue,
        hex: bool,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        let ts = self.ts;
        if matches!(value, ConstValue::Null) {
            // `type.IsReferenceType == true || type.IsKnownType(NullableOfT) || IsAnyPointer`
            return Some(if self.null_is_literal(ty)? {
                ConstSyntax::with(ConstSyntaxKind::Null, ConstRr::NullLiteral)
            } else {
                ConstSyntax::with(
                    ConstSyntaxKind::Default(ty),
                    ConstRr::Constant(ty, ConstValue::Null),
                )
            });
        }
        let underlying = ts.nullable_underlying(ty);
        // `GetDefinition()`: an enum nested in a generic type is an instance of it.
        if let Some(d) = ts.definition(underlying)
            && ts.comp.type_def(d).ok()?.kind == TypeKind::Enum
        {
            return self.convert_enum_value(
                underlying,
                d,
                as_i64(value)?,
                hex,
                special_constants,
                None,
            );
        }
        let code = self.known_code(underlying);
        if let Some(special) =
            self.special_integer_or_decimal(underlying, code, value, hex, special_constants)
        {
            return Some(special);
        }
        if matches!(code, KnownTypeCode::Single | KnownTypeCode::Double) {
            return self.convert_floating_point(
                underlying,
                code == KnownTypeCode::Double,
                value,
                special_constants,
            );
        }
        let native = matches!(ts.data(underlying), TypeData::NInt | TypeData::NUInt);
        let small = self.small_integer_keyword(underlying).is_some();
        let (literal_value, literal_type) = if small || native {
            // C# has no literals of small or native integer types: an `int` (`uint` for `nuint`).
            let unsigned = matches!(ts.data(underlying), TypeData::NUInt);
            let code = if unsigned {
                KnownTypeCode::UInt32
            } else {
                KnownTypeCode::Int32
            };
            let v = cast_integer(code, as_i64(value)?)?;
            let def = self.known_def(code)?;
            (v, ts.find(&TypeData::Def(def))?)
        } else {
            (value.clone(), underlying)
        };
        let literal = ConstSyntax::with(
            ConstSyntaxKind::Literal {
                value: literal_value.clone(),
                hex,
            },
            ConstRr::Constant(literal_type, literal_value),
        );
        // A small or native integer literal of another expected type, or one of an unresolved
        // type, keeps its type as a cast.
        if (small || native) && ty != expected
            || matches!(
                ts.data(underlying),
                TypeData::Unknown { .. } | TypeData::UnknownSpecial
            )
        {
            return Some(ConstSyntax::new(ConstSyntaxKind::Cast(
                ty,
                Box::new(literal),
            )));
        }
        Some(literal)
    }

    /// The initializer of the enum member `field` (a Field row of the enum `def`, whose type is
    /// `ty`) with the value `val`: `ConvertEnumValue(type, val, declaringEnumMember)`.
    pub fn convert_enum_member_initializer(
        &self,
        ty: TypeId,
        def: TypeDefId,
        val: i64,
        field: u32,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        self.convert_enum_value(ty, def, val, false, special_constants, Some(field))
    }

    /// `ConvertEnumValue(type, val, declaringEnumMember)`. Inside the declaration of the member
    /// `declaring` (its Field row), the members are named unqualified, only those declared before
    /// it, and in a `[Flags]` enum only single-bit members directly; a value the members do not
    /// spell stays numeric, without a cast.
    fn convert_enum_value(
        &self,
        ty: TypeId,
        def: TypeDefId,
        val: i64,
        hex: bool,
        special_constants: bool,
        declaring: Option<u32>,
    ) -> Option<ConstSyntax> {
        let ts = self.ts;
        let info = ts.comp.type_def(def).ok()?;
        let base_code = info.enum_underlying?;
        let md = &ts.comp.module(def.module).md;
        let is_flags = crate::typesystem::flags::has_attribute(
            md,
            TableId::TypeDef,
            def.row,
            "System",
            "FlagsAttribute",
        );
        // `PrepareConstant`: the constant fields, with their value and Hamming weight.
        let mut fields = Vec::new();
        for row in md.type_fields(def.row).ok()? {
            if !ts.field_is_const(def.module, row) {
                continue;
            }
            let Some(v) = as_i64(&ts.field_constant(def.module, row)) else {
                continue;
            };
            let name = md
                .string(md.get::<rows::Field>(row).ok()?.name)
                .ok()?
                .into_owned();
            fields.push((
                v,
                FieldRef {
                    module: def.module,
                    row,
                },
                name,
                (v as u64).count_ones(),
            ));
        }
        let member = |f: &(i64, FieldRef, String, u32)| {
            ConstSyntax::new(match declaring {
                None => ConstSyntaxKind::EnumMember {
                    ty,
                    field: f.1,
                    name: f.2.clone(),
                },
                Some(_) => ConstSyntaxKind::EnumMemberIdentifier {
                    field: f.1,
                    name: f.2.clone(),
                },
            })
        };
        let declaring_row = declaring.unwrap_or(u32::MAX);
        let underlying_def = self.known_def(base_code)?;
        let underlying = ts.find(&TypeData::Def(underlying_def))?;
        let numeric = |hex: bool| {
            self.convert_constant_value(
                underlying,
                underlying,
                &cast_integer(base_code, val)?,
                hex,
                special_constants,
            )
        };
        if let Some(f) = fields
            .iter()
            .find(|f| f.0 == val && (declaring.is_none() || !is_flags || f.3 == 1))
        {
            // A member does not name itself or a member declared after it.
            if Some(f.1.row) == declaring || declaring_row < f.1.row {
                return numeric(hex);
            }
            return Some(member(f));
        }
        if is_flags {
            // The complement of a byte- or ushort-based member is computed in int, which an
            // enum member initializer cannot convert back implicitly.
            let complement_compiles = declaring.is_none()
                || !matches!(base_code, KnownTypeCode::Byte | KnownTypeCode::UInt16);
            let mut enum_value = val;
            let mut negated = !val;
            match base_code {
                KnownTypeCode::Byte | KnownTypeCode::SByte => negated &= 0xFF,
                KnownTypeCode::Int16 | KnownTypeCode::UInt16 => negated &= 0xFFFF,
                KnownTypeCode::Int32 | KnownTypeCode::UInt32 => negated &= 0xFFFF_FFFF,
                _ => {}
            }
            let mut expr: Option<ConstSyntax> = None;
            let mut negated_expr: Option<ConstSyntax> = None;
            let (mut terms, mut negated_terms) = (0usize, 0usize);
            // `OrderByDescending(weight)`, which keeps the declaration order of equal weights.
            let mut ordered: Vec<&(i64, FieldRef, String, u32)> = fields.iter().collect();
            ordered.sort_by_key(|f| std::cmp::Reverse(f.3));
            for f in ordered {
                let field_value = f.0;
                if field_value == 0 || Some(f.1.row) == declaring || declaring_row < f.1.row {
                    continue;
                }
                if field_value & enum_value == field_value {
                    expr = Some(match expr {
                        None => member(f),
                        Some(e) => ConstSyntax::new(ConstSyntaxKind::BitwiseOr(
                            Box::new(e),
                            Box::new(member(f)),
                        )),
                    });
                    terms += 1;
                    enum_value &= !field_value;
                }
                if complement_compiles && field_value & negated == field_value {
                    negated_expr = Some(match negated_expr {
                        None => member(f),
                        Some(e) => ConstSyntax::new(ConstSyntaxKind::BitwiseOr(
                            Box::new(e),
                            Box::new(member(f)),
                        )),
                    });
                    negated_terms += 1;
                    negated &= !field_value;
                }
            }
            // A multi-bit value inside a larger, earlier member is a field encoding within that
            // mask, not a union of flags: it stays numeric.
            let encoded_in_earlier_mask = declaring.is_some()
                && fields.iter().any(|f| {
                    Some(f.1.row) != declaring
                        && f.1.row < declaring_row
                        && f.0 & val == val
                        && f.0 != val
                });
            // The complement wins only with fewer nodes, that is with fewer members.
            if enum_value == 0
                && !encoded_in_earlier_mask
                && let Some(e) = expr
                && !(negated == 0 && negated_expr.is_some() && negated_terms < terms)
            {
                return Some(e);
            }
            if complement_compiles
                && negated == 0
                && let Some(n) = negated_expr
            {
                return Some(ConstSyntax::new(ConstSyntaxKind::BitNot(Box::new(n))));
            }
        }
        let numeric = numeric(hex)?;
        if declaring.is_some() {
            return Some(numeric);
        }
        Some(ConstSyntax::new(ConstSyntaxKind::Cast(
            ty,
            Box::new(numeric),
        )))
    }
}

/// A float or double constant, each in its own precision.
#[derive(Clone, Copy)]
enum Fp {
    Single(f32),
    Double(f64),
}

impl Fp {
    fn value(self) -> f64 {
        match self {
            Fp::Single(v) => f64::from(v),
            Fp::Double(v) => v,
        }
    }

    fn constant(self) -> ConstValue {
        match self {
            Fp::Single(v) => ConstValue::Single(v.to_bits()),
            Fp::Double(v) => ConstValue::Double(v.to_bits()),
        }
    }

    /// The same type with another value.
    fn with(self, f: f64) -> Fp {
        match self {
            Fp::Single(_) => Fp::Single(f as f32),
            Fp::Double(_) => Fp::Double(f),
        }
    }

    /// `MakeConstant(type, c)`: the integer as a literal of the floating point type.
    fn of(self, c: i64) -> ConstValue {
        match self {
            Fp::Single(_) => ConstValue::Single((c as f32).to_bits()),
            Fp::Double(_) => ConstValue::Double((c as f64).to_bits()),
        }
    }

    /// `IsEqual(num, den, constantValue, isDouble)`: `num / den` in the type's precision.
    fn equals_fraction(self, num: i64, den: i64) -> bool {
        match self {
            Fp::Single(v) => v == num as f32 / den as f32,
            Fp::Double(v) => v == num as f64 / den as f64,
        }
    }

    /// `ToString("r")`: the shortest round-trip digits, in fixed notation for decimal exponents
    /// above -5 and below the type's precision (15 for double, 7 for float), otherwise as
    /// `d.dddE+XX`.
    fn round_trip_string(self) -> String {
        let (scientific, precision) = match self {
            Fp::Single(v) => (format!("{v:e}"), 7),
            Fp::Double(v) => (format!("{v:e}"), 15),
        };
        let (mantissa, exponent) = scientific.split_once('e').expect("an exponent");
        let exponent: i32 = exponent.parse().expect("a decimal exponent");
        let digits: String = mantissa.chars().filter(char::is_ascii_digit).collect();
        let mut s = String::new();
        if mantissa.starts_with('-') {
            s.push('-');
        }
        if exponent > -5 && exponent < precision {
            if exponent >= 0 {
                let int_len = exponent as usize + 1;
                if digits.len() <= int_len {
                    s.push_str(&digits);
                    s.extend(std::iter::repeat_n('0', int_len - digits.len()));
                } else {
                    s.push_str(&digits[..int_len]);
                    s.push('.');
                    s.push_str(&digits[int_len..]);
                }
            } else {
                s.push_str("0.");
                s.extend(std::iter::repeat_n('0', (-exponent - 1) as usize));
                s.push_str(&digits);
            }
        } else {
            s.push_str(&digits[..1]);
            if digits.len() > 1 {
                s.push('.');
                s.push_str(&digits[1..]);
            }
            s.push('E');
            s.push(if exponent < 0 { '-' } else { '+' });
            s.push_str(&format!("{:02}", exponent.unsigned_abs()));
        }
        s
    }
}

const MAX_DENOMINATOR_DOUBLE: i64 = 1000;
/// `preferredFractionDenominators`: powers of two and 2^n-1 scales.
const PREFERRED_FRACTION_DENOMINATORS: [i64; 15] = [
    127, 128, 255, 256, 1023, 1024, 4095, 4096, 8192, 16384, 32767, 32768, 65535, 65536, 1048576,
];

fn is_valid_fraction(num: i64, den: i64) -> bool {
    if !(den > 0 && num != 0) {
        return false;
    }
    if den == 1 || num.abs() == 1 {
        return true;
    }
    num.abs() < den && [2, 3, 5].iter().any(|x| den % x == 0)
}

fn integer_literal_length(mut value: i64) -> i64 {
    let mut length = i64::from(value < 0);
    loop {
        length += 1;
        value /= 10;
        if value == 0 {
            return length;
        }
    }
}

fn fraction_display_length(num: i64, den: i64, is_double: bool) -> i64 {
    let suffix = if is_double { 2 } else { 1 };
    integer_literal_length(num) + integer_literal_length(den) + 3 + 2 * suffix
}

fn is_simple_fraction(num: i64, mut den: i64) -> bool {
    if num == 1 || num == -1 {
        return true;
    }
    for p in [2, 3, 5] {
        while den % p == 0 {
            den /= p;
        }
    }
    den == 1
}

fn preferred_fraction_score(num: i64, den: i64, is_double: bool) -> i64 {
    let bonus = if den & (den - 1) == 0 { 1 } else { 2 };
    fraction_display_length(num, den, is_double) - bonus
}

/// `TryGetPreferredFraction`: the numerator, denominator and score.
fn preferred_fraction(v: Fp, is_double: bool) -> Option<(i64, i64, i64)> {
    let value = v.value();
    if value.is_nan() || value.abs() >= 1.0 {
        return None;
    }
    let mut best: Option<(i64, i64, i64)> = None;
    for den in PREFERRED_FRACTION_DENOMINATORS {
        // `Math.Round` rounds half to even.
        let num = (value * den as f64).round_ties_even() as i64;
        if num == 0 || num <= -den || num >= den || !v.equals_fraction(num, den) {
            continue;
        }
        let score = preferred_fraction_score(num, den, is_double);
        if best.is_none_or(|(_, d, s)| score < s || (score == s && den < d)) {
            best = Some((num, den, score));
        }
    }
    best
}

/// `FractionApprox`: the best rational approximation with a bounded denominator, by continued
/// fractions.
fn fraction_approx(value: f64, max_denominator: i64) -> (i64, i64) {
    if value.abs() > f64::from(0x7FFF_FFFF) {
        return (0, 0);
    }
    let start = value;
    let value = value.abs();
    let (mut m00, mut m01, mut m10, mut m11) = (1i64, 0i64, 0i64, 1i64);
    let mut v = value;
    loop {
        let ai = v as i64;
        if m10 * ai + m11 > max_denominator {
            break;
        }
        let t = m00 * ai + m01;
        m01 = m00;
        m00 = t;
        let t = m10 * ai + m11;
        m11 = m10;
        m10 = t;
        if v - ai as f64 == 0.0 {
            break;
        }
        v = 1.0 / (v - ai as f64);
        if v.abs() >= i64::MAX as f64 {
            break;
        }
    }
    if m10 == 0 {
        return (0, 0);
    }
    let (first_n, first_d) = (m00, m10);
    let ai = (max_denominator - m11) / m10;
    let (second_n, second_d) = (m00 * ai + m01, m10 * ai + m11);
    let first_delta = (value - first_n as f64 / first_d as f64).abs();
    let second_delta = (value - second_n as f64 / second_d as f64).abs();
    let sign = |n: i64| if start < 0.0 { -n } else { n };
    if first_delta < second_delta {
        (sign(first_n), first_d)
    } else {
        (sign(second_n), second_d)
    }
}

impl ConstantSyntaxBuilder<'_, '_, '_> {
    fn literal(value: ConstValue) -> ConstSyntax {
        ConstSyntax::new(ConstSyntaxKind::Literal { value, hex: false })
    }

    /// `MakeFraction(type, num, den)`.
    fn fraction(v: Fp, num: i64, den: i64) -> ConstSyntax {
        ConstSyntax::new(ConstSyntaxKind::Divide(
            Box::new(Self::literal(v.of(num))),
            Box::new(Self::literal(v.of(den))),
        ))
    }

    /// The special constants and `ConvertFloatingPointLiteral` of a float or double.
    fn convert_floating_point(
        &self,
        ty: TypeId,
        is_double: bool,
        value: &ConstValue,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        // `CSharpPrimitiveCast.Cast(type.GetTypeCode(), constantValue)`: some compilers embed
        // other numeric types in float and double constants.
        let wide = match *value {
            ConstValue::Single(b) => f64::from(f32::from_bits(b)),
            ConstValue::Double(b) => f64::from_bits(b),
            ref other => as_i64(other)? as f64,
        };
        let v = if is_double {
            Fp::Double(wide)
        } else {
            Fp::Single(wide as f32)
        };
        if let Some(special) = self.special_floating_point(ty, v, special_constants) {
            return Some(special);
        }
        let s = v.round_trip_string();
        let integral = v.value().floor() == v.value();
        let mut expr = integral.then(|| Self::literal(v.constant()));
        let digits = s.len() as i64 - if s.starts_with('-') { 2 } else { 1 };
        let use_fraction = digits > 5;
        if use_fraction && expr.is_none() {
            // A smaller bound for floats avoids coincidences such as 1f/MathF.PI == 113f/355f.
            let (mut num, mut den) = fraction_approx(
                v.value(),
                if is_double {
                    MAX_DENOMINATOR_DOUBLE
                } else {
                    200
                },
            );
            let has_regular =
                is_valid_fraction(num, den) && v.equals_fraction(num, den) && den.abs() != 1;
            if let Some((pn, pd, score)) = preferred_fraction(v, is_double) {
                let baseline = if has_regular {
                    fraction_display_length(num, den, is_double)
                } else {
                    s.len() as i64 + i64::from(!is_double)
                };
                let regular_is_simple = has_regular && is_simple_fraction(num, den);
                if !regular_is_simple && score <= baseline {
                    if has_regular {
                        (num, den) = (pn, pd);
                    } else {
                        expr = Some(Self::fraction(v, pn, pd));
                    }
                }
            }
            if has_regular {
                expr = Some(Self::fraction(v, num, den));
            }
        }
        if use_fraction && expr.is_none() && special_constants {
            // shortcut: a float chooses between MathF and Math by `IsDirectImportOf`, which
            // compares full assembly names; such a float stays with the host until that check
            // is ported.
            if !is_double {
                return None;
            }
            expr = self.math_expression(v, "PI")?;
            if expr.is_none() {
                expr = self.math_expression(v, "E")?;
            }
        }
        let mut expr = expr.unwrap_or_else(|| Self::literal(v.constant()));
        expr.rr = Some(ConstRr::Constant(ty, v.constant()));
        Some(expr)
    }

    /// `IsSpecialConstant` of a float or double: `float.NaN` and the like where the field
    /// exists and `UseSpecialConstants` holds, otherwise the infinities and NaN as divisions.
    fn special_floating_point(
        &self,
        ty: TypeId,
        v: Fp,
        special_constants: bool,
    ) -> Option<ConstSyntax> {
        let x = v.value();
        let (epsilon, min, max) = match v {
            Fp::Single(_) => (
                f64::from(f32::from_bits(1)),
                f64::from(f32::MIN),
                f64::from(f32::MAX),
            ),
            Fp::Double(_) => (f64::from_bits(1), f64::MIN, f64::MAX),
        };
        let (member, negate) = if x.is_nan() {
            ("NaN", false)
        } else if x == f64::NEG_INFINITY {
            ("NegativeInfinity", false)
        } else if x == f64::INFINITY {
            ("PositiveInfinity", false)
        } else if x == min {
            ("MinValue", false)
        } else if x == max {
            ("MaxValue", false)
        } else if x == epsilon {
            ("Epsilon", false)
        } else if x == -epsilon {
            ("Epsilon", true)
        } else {
            return None;
        };
        let code = match v {
            Fp::Single(_) => KnownTypeCode::Single,
            Fp::Double(_) => KnownTypeCode::Double,
        };
        let field = self
            .known_def(code)
            .and_then(|def| self.field_named(def, member))
            .filter(|_| special_constants);
        let Some(field) = field else {
            // +Infinity, -Infinity and NaN cannot be literals; other values are ordinary ones.
            let left = if x.is_nan() {
                0.0
            } else if x == f64::INFINITY {
                1.0
            } else if x == f64::NEG_INFINITY {
                -1.0
            } else {
                return None;
            };
            let operand = |f: f64| {
                ConstSyntax::with(
                    ConstSyntaxKind::Literal {
                        value: v.with(f).constant(),
                        hex: false,
                    },
                    ConstRr::Constant(ty, v.with(f).constant()),
                )
            };
            return Some(ConstSyntax::with(
                ConstSyntaxKind::Divide(Box::new(operand(left)), Box::new(operand(0.0))),
                ConstRr::Constant(ty, v.constant()),
            ));
        };
        let reference = ConstSyntax::new(ConstSyntaxKind::SpecialConstant {
            ty,
            field,
            name: member.to_owned(),
        });
        Some(if negate {
            ConstSyntax::with(
                ConstSyntaxKind::Minus(Box::new(reference)),
                ConstRr::Constant(ty, v.constant()),
            )
        } else {
            reference
        })
    }

    /// `TryExtractExpression(Math, double, value, member, isDouble: true)`: the value as a simple
    /// multiple or divisor of `Math.PI` or `Math.E`. `None` when the syntax cannot be written,
    /// `Some(None)` when the value is no such multiple.
    fn math_expression(&self, v: Fp, member: &'static str) -> Option<Option<ConstSyntax>> {
        let constant = if member == "PI" {
            std::f64::consts::PI
        } else {
            std::f64::consts::E
        };
        let value = v.value();
        let name = crate::typesystem::names::FullTypeName::top_level(
            crate::typesystem::names::TopLevelTypeName::new("System", "Math", 0),
        );
        let comp = self.ts.comp;
        let math_def = comp.module_ids().find_map(|m| {
            comp.find_type_definition(m, &name, &mut Vec::new())
                .ok()
                .flatten()
        })?;
        let math = self.ts.find(&TypeData::Def(math_def))?;
        let field = self.first_field_named(math_def, member);
        let reference = || {
            ConstSyntax::new(ConstSyntaxKind::MathMember {
                ty: math,
                field,
                name: member,
            })
        };
        let extract = |n: i64, d: i64| -> Option<ConstSyntax> {
            // `field * n / d`, `-field / d` or `field / d`
            let mut expr = reference();
            if n != 1 {
                expr = if n == -1 {
                    ConstSyntax::new(ConstSyntaxKind::Minus(Box::new(expr)))
                } else {
                    ConstSyntax::new(ConstSyntaxKind::Multiply(
                        Box::new(expr),
                        Box::new(Self::literal(v.of(n))),
                    ))
                };
            }
            if d != 1 {
                expr = ConstSyntax::new(ConstSyntaxKind::Divide(
                    Box::new(expr),
                    Box::new(Self::literal(v.of(d))),
                ));
            }
            if constant * n as f64 / d as f64 == value {
                return Some(expr);
            }
            // `n / field` or `n / (d * field)`
            let divisor = if d == 1 {
                reference()
            } else {
                ConstSyntax::new(ConstSyntaxKind::Multiply(
                    Box::new(Self::literal(v.of(d))),
                    Box::new(reference()),
                ))
            };
            let expr = ConstSyntax::new(ConstSyntaxKind::Divide(
                Box::new(Self::literal(v.of(n))),
                Box::new(divisor),
            ));
            (n as f64 / (d as f64 * constant) == value).then_some(expr)
        };
        let (num, den) = fraction_approx(value / constant, MAX_DENOMINATOR_DOUBLE);
        if is_valid_fraction(num, den)
            && let Some(e) = extract(num, den)
        {
            return Some(Some(e));
        }
        let (num, den) = fraction_approx(value * constant, MAX_DENOMINATOR_DOUBLE);
        Some(if is_valid_fraction(num, den) {
            extract(num, den)
        } else {
            None
        })
    }

    /// `GetFields(f => f.Name == name).FirstOrDefault()`.
    fn first_field_named(&self, def: TypeDefId, name: &str) -> Option<FieldRef> {
        let md = &self.ts.comp.module(def.module).md;
        md.type_fields(def.row).ok()?.into_iter().find_map(|f| {
            md.get::<rows::Field>(f)
                .ok()
                .and_then(|r| md.string(r.name).ok())
                .is_some_and(|n| n == name)
                .then_some(FieldRef {
                    module: def.module,
                    row: f,
                })
        })
    }
}

#[cfg(test)]
mod tests {
    use super::{Fp, fraction_approx};

    #[test]
    fn round_trip_strings_match_dotnet() {
        assert_eq!(Fp::Double(0.1).round_trip_string(), "0.1");
        assert_eq!(Fp::Double(0.0001).round_trip_string(), "0.0001");
        assert_eq!(Fp::Double(0.00001).round_trip_string(), "1E-05");
        assert_eq!(Fp::Double(1e14).round_trip_string(), "100000000000000");
        assert_eq!(Fp::Double(1e15).round_trip_string(), "1E+15");
        assert_eq!(Fp::Double(-2.5).round_trip_string(), "-2.5");
        assert_eq!(
            Fp::Double(0.1 + 0.2).round_trip_string(),
            "0.30000000000000004"
        );
        assert_eq!(Fp::Single(1e7).round_trip_string(), "1E+07");
        assert_eq!(Fp::Single(9_999_999.0).round_trip_string(), "9999999");
        assert_eq!(Fp::Single(0.1).round_trip_string(), "0.1");
        assert_eq!(
            Fp::Double(1.2345678901234568e17).round_trip_string(),
            "1.2345678901234568E+17"
        );
    }

    #[test]
    fn fraction_approx_finds_small_fractions() {
        assert_eq!(fraction_approx(1.0 / 3.0, 1000), (1, 3));
        assert_eq!(fraction_approx(-0.75, 1000), (-3, 4));
        assert_eq!(fraction_approx(std::f64::consts::PI, 1000), (355, 113));
    }
}
