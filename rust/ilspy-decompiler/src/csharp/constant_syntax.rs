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
    /// `left | right`.
    BitwiseOr(Box<ConstSyntax>, Box<ConstSyntax>),
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
fn as_i64(v: &ConstValue) -> Option<i64> {
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
        let mut expr = self.convert_constant_value(ty, value, hex, special_constants)?;
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
        if let TypeData::Def(d) = *ts.data(underlying)
            && ts.comp.type_def(d).ok()?.kind == TypeKind::Enum
        {
            return self.convert_enum_value(underlying, d, as_i64(value)?, hex, special_constants);
        }
        let code = self.known_code(underlying);
        if matches!(code, KnownTypeCode::Single | KnownTypeCode::Double) {
            // shortcut: floating point constants (special values, fractions, multiples of PI
            // and E) stay with the host's builder; port ConvertFloatingPointLiteral next.
            return None;
        }
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
        if !(hex && integer)
            && special_constants
            && let Some((known, member)) = special_constant(value)
            && known == code
            && let Some(def) = self.known_def(known)
            && let Some(field) = self.field_named(def, member)
        {
            // `IsSpecialConstant`: the value's own type is the expected type.
            return Some(ConstSyntax::new(ConstSyntaxKind::SpecialConstant {
                ty: underlying,
                field,
                name: member.to_owned(),
            }));
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
        // An unresolved type keeps its name on the literal.
        if matches!(
            ts.data(underlying),
            TypeData::Unknown { .. } | TypeData::UnknownSpecial
        ) {
            return Some(ConstSyntax::new(ConstSyntaxKind::Cast(
                ty,
                Box::new(literal),
            )));
        }
        Some(literal)
    }

    /// `ConvertEnumValue(type, val)` outside an enum declaration.
    fn convert_enum_value(
        &self,
        ty: TypeId,
        def: TypeDefId,
        val: i64,
        hex: bool,
        special_constants: bool,
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
            ConstSyntax::new(ConstSyntaxKind::EnumMember {
                ty,
                field: f.1,
                name: f.2.clone(),
            })
        };
        if let Some(f) = fields.iter().find(|f| f.0 == val) {
            return Some(member(f));
        }
        if is_flags {
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
                if field_value == 0 {
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
                if field_value & negated == field_value {
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
            // The complement wins only with fewer nodes, that is with fewer members.
            if enum_value == 0
                && let Some(e) = expr
                && !(negated == 0 && negated_expr.is_some() && negated_terms < terms)
            {
                return Some(e);
            }
            if negated == 0
                && let Some(n) = negated_expr
            {
                return Some(ConstSyntax::new(ConstSyntaxKind::BitNot(Box::new(n))));
            }
        }
        let underlying_def = self.known_def(base_code)?;
        let underlying = ts.find(&TypeData::Def(underlying_def))?;
        let numeric = self.convert_constant_value(
            underlying,
            &cast_integer(base_code, val)?,
            hex,
            special_constants,
        )?;
        Some(ConstSyntax::new(ConstSyntaxKind::Cast(
            ty,
            Box::new(numeric),
        )))
    }
}
