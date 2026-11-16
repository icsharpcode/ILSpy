//! `TypeSystemAstBuilder.ConvertEntity` as `CSharpDecompiler.CreateAstBuilder` configures it
//! (`ShowAttributes`, `AlwaysUseShortTypeNames`, `AddResolveResultAnnotations`): the declaration
//! syntax of a member, without its body.

use crate::metadata::tables::{TableId, rows};
use crate::typesystem::attributes::Attribute;
use crate::typesystem::compilation::{ModuleId, TypeKind};
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::Unsupported;
use super::constant_syntax::{ConstSyntax, ConstantSyntaxBuilder};

/// The options of the host's `TypeSystemAstBuilder` the declaration depends on.
#[derive(Clone, Copy, Debug)]
pub struct EntityOptions {
    /// `UseSpecialConstants`, which `DoDecompile(IField)` sets per field.
    pub special_constants: bool,
    /// `SortAttributes` (`SortCustomAttributes`).
    pub sort_attributes: bool,
    /// `UsePrivateProtectedAccessibility` (`IntroducePrivateProtectedAccessibility`).
    pub private_protected: bool,
    /// `UseNullableSpecifierForValueTypes` (`LiftNullables`).
    pub nullable_specifier: bool,
}

/// `Modifiers` of the C# syntax tree.
pub mod modifiers {
    pub const PRIVATE: u32 = 0x0001;
    pub const INTERNAL: u32 = 0x0002;
    pub const PROTECTED: u32 = 0x0004;
    pub const PUBLIC: u32 = 0x0008;
    pub const STATIC: u32 = 0x0080;
    pub const READONLY: u32 = 0x0200;
    pub const CONST: u32 = 0x0400;
    pub const VOLATILE: u32 = 0x4000;
}

/// A named argument of an attribute.
#[derive(Clone, Debug)]
pub struct NamedArgumentSyntax {
    pub name: String,
    pub is_field: bool,
    /// The argument's type, which the host finds the member with.
    pub ty: TypeId,
    pub value: ConstSyntax,
}

/// `ConvertAttribute`: one attribute.
#[derive(Clone, Debug)]
pub struct AttributeSyntax {
    /// The attribute type, whose syntax the type table holds.
    pub ty: TypeId,
    /// The identifier of that syntax, without the `Attribute` suffix (`None` for the null
    /// identifier `ConvertAttributeType` leaves for a name without it).
    pub identifier: Option<String>,
    /// The constructor, as its declaring type and `MethodDef`.
    pub constructor: Option<(TypeId, ModuleId, u32)>,
    pub arguments: Vec<ConstSyntax>,
    pub named: Vec<NamedArgumentSyntax>,
    pub has_decode_errors: bool,
}

/// `ConvertField`.
#[derive(Clone, Debug)]
pub struct FieldSyntax {
    pub modifiers: u32,
    pub attributes: Vec<AttributeSyntax>,
    pub return_type: TypeId,
    /// `HasReadOnlySpecifier` of a `ref readonly` field's type.
    pub ref_readonly: bool,
    pub initializer: Option<ConstSyntax>,
}

/// `ModifierFromAccessibility` of a metadata access mask (field or method: the same values).
fn accessibility_modifier(access: u16, private_protected: bool) -> u32 {
    use modifiers::*;
    match access & 7 {
        6 => PUBLIC,
        3 => INTERNAL,
        4 => PROTECTED,
        5 => PROTECTED | INTERNAL,
        2 if private_protected => PRIVATE | PROTECTED,
        2 => PROTECTED,
        _ => PRIVATE,
    }
}

/// `ConvertAttributes(attributes)`, one section each.
pub fn convert_attributes(
    ts: &mut TypeSystem<'_, '_>,
    attributes: &[Attribute],
    options: EntityOptions,
) -> Result<Vec<AttributeSyntax>, Unsupported> {
    // shortcut: `SortCustomAttributes` compares argument values with `IComparable`, culture
    // dependent for strings; sorted attribute lists stay with the host.
    if options.sort_attributes && attributes.len() > 1 {
        return Err("sorted attributes".to_owned());
    }
    let mut result = Vec::with_capacity(attributes.len());
    for a in attributes {
        result.push(convert_attribute(ts, a)?);
    }
    Ok(result)
}

/// `ConvertAttribute`.
fn convert_attribute(
    ts: &mut TypeSystem<'_, '_>,
    attribute: &Attribute,
) -> Result<AttributeSyntax, Unsupported> {
    // `ConvertAttributeType` with short type names: the name without the `Attribute` suffix,
    // or none; `ConvertAttribute` then strips a remaining suffix once more.
    let name = match ts.data(attribute.ty) {
        TypeData::Def(d) => ts
            .comp
            .type_def(*d)
            .map_err(|e| e.to_string())?
            .full_name
            .name()
            .to_owned(),
        TypeData::Inst { generic, .. } => match ts.data(*generic) {
            TypeData::Def(d) => ts
                .comp
                .type_def(*d)
                .map_err(|e| e.to_string())?
                .full_name
                .name()
                .to_owned(),
            _ => return Err("an attribute of an unresolved generic type".to_owned()),
        },
        TypeData::Unknown { name, .. } => name.name().to_owned(),
        _ => return Err("an attribute of another type".to_owned()),
    };
    let mut identifier =
        (name.len() > 9 && name.ends_with("Attribute")).then(|| name[..name.len() - 9].to_owned());
    if let Some(id) = &identifier
        && id.ends_with("Attribute")
    {
        identifier = Some(id[..id.len() - 9].to_owned());
    }
    let constructor = match &attribute.ctor {
        Some(c) => match c.origin {
            MethodOrigin::Def(m) => Some((
                c.declaring
                    .ok_or("an attribute constructor without declaring type")?,
                m.module,
                m.row,
            )),
            _ => return Err("an attribute constructor of no definition".to_owned()),
        },
        None => None,
    };
    let parameter_types: Vec<TypeId> = attribute
        .ctor
        .as_ref()
        .map(|c| c.base_params.clone())
        .unwrap_or_default();
    let builder = ConstantSyntaxBuilder::new(ts);
    let mut arguments = Vec::with_capacity(attribute.fixed.len());
    for (i, arg) in attribute.fixed.iter().enumerate() {
        let expected = parameter_types.get(i).copied().unwrap_or(arg.ty);
        arguments.push(
            builder
                .convert_argument(expected, arg)
                .ok_or("an attribute argument the engine does not write")?,
        );
    }
    let mut named = Vec::with_capacity(attribute.named.len());
    for n in &attribute.named {
        named.push(NamedArgumentSyntax {
            name: n
                .name
                .clone()
                .ok_or("a named attribute argument without name")?,
            is_field: n.is_field,
            ty: n.arg.ty,
            value: builder
                .convert_argument(n.arg.ty, &n.arg)
                .ok_or("an attribute argument the engine does not write")?,
        });
    }
    Ok(AttributeSyntax {
        ty: attribute.ty,
        identifier,
        constructor,
        arguments,
        named,
        has_decode_errors: attribute.has_decode_errors,
    })
}

/// `ConvertField(field)`.
pub fn convert_field(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
) -> Result<FieldSyntax, Unsupported> {
    let md = &ts.comp.module(module).md;
    let flags = md.get::<rows::Field>(row).map_err(|e| e.to_string())?.flags;
    let declaring = md.field_declaring_type(row).map_err(|e| e.to_string())?;
    let in_interface = declaring != 0
        && ts
            .comp
            .type_def(crate::typesystem::compilation::TypeDefId {
                module,
                row: declaring,
            })
            .is_ok_and(|i| i.kind == TypeKind::Interface);
    let is_ref_readonly_attribute = crate::typesystem::flags::has_attribute(
        md,
        TableId::Field,
        row,
        "System.Runtime.CompilerServices",
        "IsReadOnlyAttribute",
    );
    let is_const = ts.field_is_const(module, row);
    let (ty, volatile) = ts.field_type(module, row);
    // `GetMemberModifiers`: the accessibility (a public member of an interface has none), then
    // `static`.
    let mut m = 0;
    let public = flags & 7 == 6;
    if !(in_interface && public) {
        m |= accessibility_modifier(flags, options.private_protected);
    }
    if flags & 0x0010 != 0 {
        m |= modifiers::STATIC;
    }
    if is_const {
        m &= !modifiers::STATIC;
        m |= modifiers::CONST;
    } else if flags & 0x0020 != 0 {
        m |= modifiers::READONLY;
    } else if volatile {
        m |= modifiers::VOLATILE;
    }
    let attributes = ts
        .field_attributes(module, row)
        .map_err(|e| e.to_string())?;
    let attributes = convert_attributes(ts, &attributes, options)?;
    let ref_readonly = matches!(ts.data(ty), TypeData::ByRef(_)) && is_ref_readonly_attribute;
    let initializer = if is_const {
        let value = ts.field_constant(module, row);
        Some(
            ConstantSyntaxBuilder::new(ts)
                .convert_raw(ty, &value, options.special_constants)
                .ok_or("a constant the engine does not write")?,
        )
    } else {
        None
    };
    Ok(FieldSyntax {
        modifiers: m,
        attributes,
        return_type: ty,
        ref_readonly,
        initializer,
    })
}
