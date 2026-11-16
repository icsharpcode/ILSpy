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
    pub const ABSTRACT: u32 = 0x0010;
    pub const VIRTUAL: u32 = 0x0020;
    pub const SEALED: u32 = 0x0040;
    pub const STATIC: u32 = 0x0080;
    pub const OVERRIDE: u32 = 0x0100;
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

/// `ConvertParameter`.
#[derive(Clone, Debug)]
pub struct ParameterSyntax {
    /// `ReferenceKind`, as the managed enum value.
    pub ref_kind: u8,
    pub is_params: bool,
    /// `Lifetime.ScopedRef`: a declared `scoped`.
    pub is_scoped: bool,
    pub attributes: Vec<AttributeSyntax>,
    /// The type without the `ByRef` the reference kind writes.
    pub ty: TypeId,
    pub name: String,
    pub default_value: Option<ConstSyntax>,
}

/// `ConvertTypeParameter`.
#[derive(Clone, Debug)]
pub struct TypeParameterSyntax {
    /// `VarianceModifier`, as the managed enum value.
    pub variance: u8,
    pub name: String,
    pub attributes: Vec<AttributeSyntax>,
}

/// One entry of a constraint's base types.
#[derive(Clone, Debug)]
pub enum ConstraintTypeSyntax {
    /// `PrimitiveType(keyword)`, made nullable for `class?`.
    Keyword(&'static str, bool),
    Type(TypeId),
}

/// `ConvertTypeParameterConstraint`.
#[derive(Clone, Debug)]
pub struct ConstraintSyntax {
    pub type_parameter: String,
    pub base_types: Vec<ConstraintTypeSyntax>,
}

/// The declaration a method is written as.
#[derive(Clone, Debug)]
pub enum MethodKindSyntax {
    Method,
    /// An operator, by its metadata name (`op_Addition`), which `OperatorDeclaration` maps.
    Operator(String),
    /// A constructor or destructor, named after the declaring type.
    Constructor(String),
    Destructor(String),
}

/// `ConvertMethod`, `ConvertOperator`, `ConvertConstructor` or `ConvertDestructor`.
#[derive(Clone, Debug)]
pub struct MethodSyntax {
    pub kind: MethodKindSyntax,
    pub modifiers: u32,
    pub attributes: Vec<AttributeSyntax>,
    /// The attributes of the return value (`[return: ...]`).
    pub return_attributes: Vec<AttributeSyntax>,
    /// The return type, none for a constructor or destructor.
    pub return_type: Option<TypeId>,
    pub ref_readonly: bool,
    pub name: String,
    pub type_parameters: Vec<TypeParameterSyntax>,
    pub parameters: Vec<ParameterSyntax>,
    /// `HasThisModifier` of the first parameter of an extension method.
    pub this_modifier: bool,
    pub constraints: Vec<ConstraintSyntax>,
}

/// `IsObjectOrValueType`.
fn is_object_or_value_type(ts: &TypeSystem<'_, '_>, t: TypeId) -> bool {
    use crate::typesystem::known::KnownTypeCode as K;
    ts.is_known(t, K::Object) || ts.is_known(t, K::ValueType)
}

/// `ConvertTypeParameterConstraint(tp)`: `None` without constraints to write.
fn convert_constraint(
    ts: &TypeSystem<'_, '_>,
    tp: &crate::typesystem::members::TypeParamInfo,
    constraint_attributes: bool,
) -> Result<Option<ConstraintSyntax>, Unsupported> {
    use crate::typesystem::types::Nullability;
    if !tp.default_ctor
        && !tp.reference_type
        && !tp.value_type
        && !tp.allows_ref_like
        && tp.nullability != Nullability::NotNullable
        && tp
            .constraints
            .iter()
            .all(|&t| is_object_or_value_type(ts, t))
    {
        return Ok(None);
    }
    // shortcut: attributes on constraint types (`where T : [A] I`) are written as an attributed
    // ComposedType, which the engine does not build; such constraints stay with the host.
    if constraint_attributes {
        return Err("attributes on a type constraint".to_owned());
    }
    let mut base = Vec::new();
    if tp.reference_type {
        base.push(ConstraintTypeSyntax::Keyword(
            "class",
            tp.nullability == Nullability::Nullable,
        ));
    } else if tp.value_type {
        base.push(ConstraintTypeSyntax::Keyword(
            if tp.unmanaged { "unmanaged" } else { "struct" },
            false,
        ));
    } else if tp.nullability == Nullability::NotNullable {
        base.push(ConstraintTypeSyntax::Keyword("notnull", false));
    }
    for &t in &tp.constraints {
        if !is_object_or_value_type(ts, t) {
            base.push(ConstraintTypeSyntax::Type(t));
        }
    }
    if tp.default_ctor && !tp.value_type {
        base.push(ConstraintTypeSyntax::Keyword("new", false));
    }
    if tp.allows_ref_like {
        base.push(ConstraintTypeSyntax::Keyword("allows ref struct", false));
    }
    let name = if tp.name == "_" {
        "@_".to_owned()
    } else {
        tp.name.clone()
    };
    Ok(Some(ConstraintSyntax {
        type_parameter: name,
        base_types: base,
    }))
}

/// Whether a `GenericParam`'s constraint rows carry attributes the type system keeps
/// (everything but the nullability ones it reads).
fn has_constraint_attributes(md: &crate::metadata::Metadata<'_>, generic_param: u32) -> bool {
    md.generic_param_constraints(generic_param).any(|c| {
        md.custom_attributes(TableId::GenericParamConstraint, c)
            .into_iter()
            .any(|ca| {
                md.attribute_type(ca).is_ok_and(|t| {
                    !crate::typesystem::compilation::is_known_type_name(
                        md,
                        t,
                        "System.Runtime.CompilerServices",
                        "NullableAttribute",
                        0,
                    )
                })
            })
    })
}

/// `GetMemberModifiers(method)` of a method that is no explicit interface implementation.
fn method_modifiers(
    ts: &mut TypeSystem<'_, '_>,
    m: crate::typesystem::members::MethodDefId,
    kind: crate::typesystem::members::SymbolKind,
    options: EntityOptions,
) -> Result<u32, Unsupported> {
    use crate::typesystem::members::SymbolKind as S;
    let md = &ts.comp.module(m.module).md;
    let def = md
        .get::<rows::MethodDef>(m.row)
        .map_err(|e| e.to_string())?;
    let declaring = md.method_declaring_type(m.row).map_err(|e| e.to_string())?;
    let declaring = crate::typesystem::compilation::TypeDefId {
        module: m.module,
        row: declaring,
    };
    let info_kind = ts.comp.type_def(declaring).map_err(|e| e.to_string())?.kind;
    let declaring_read_only = ts.type_flags(declaring).read_only;
    let modifiers = ts.method_modifiers_of(m).map_err(|e| e.to_string())?;
    let flags = ts.method_flags(m, None);
    let access = def.flags & 7;
    let public = access == 6;
    let needs_accessibility = match kind {
        S::Constructor => !modifiers.is_static,
        S::Destructor => false,
        _ if info_kind == TypeKind::Interface => !public,
        _ => true,
    };
    let mut r = 0;
    // `MetadataMethod.GetAccessibility`: a compiler-controlled method has no accessibility.
    if needs_accessibility && access != 0 {
        r |= accessibility_modifier(access, options.private_protected);
    }
    if modifiers.is_static {
        r |= modifiers::STATIC;
    }
    if flags.this_read_only && !declaring_read_only {
        r |= modifiers::READONLY;
    }
    if info_kind == TypeKind::Interface {
        let has_body = crate::typesystem::members::has_body(&def);
        if !modifiers.is_static
            && !modifiers.is_virtual
            && !modifiers.is_abstract
            && !modifiers.is_override
            && access != 1
            && has_body
        {
            r |= modifiers::SEALED;
        }
        if modifiers.is_static {
            if modifiers.is_abstract {
                r |= modifiers::ABSTRACT;
            } else if modifiers.is_virtual && !modifiers.is_override {
                r |= modifiers::VIRTUAL;
            }
        }
    } else {
        if modifiers.is_abstract {
            r |= modifiers::ABSTRACT;
        } else if modifiers.is_virtual && !modifiers.is_override {
            r |= modifiers::VIRTUAL;
        }
        if modifiers.is_override {
            r |= modifiers::OVERRIDE;
        }
        if modifiers.is_sealed {
            r |= modifiers::SEALED;
        }
    }
    Ok(r)
}

/// `ConvertEntity(method)` of a method, operator, constructor or destructor.
pub fn convert_method(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
) -> Result<MethodSyntax, Unsupported> {
    use crate::typesystem::members::{MethodDefId, SymbolKind as S};
    let m = MethodDefId { module, row };
    let kind = ts.method_symbol_kind(m, None).map_err(|e| e.to_string())?;
    if !ts
        .explicit_implementations(m)
        .map_err(|e| e.to_string())?
        .is_empty()
    {
        return Err("an explicit interface implementation".to_owned());
    }
    let md = &ts.comp.module(module).md;
    let def = md.get::<rows::MethodDef>(row).map_err(|e| e.to_string())?;
    let name = md.string(def.name).map_err(|e| e.to_string())?.into_owned();
    let declaring_row = md.method_declaring_type(row).map_err(|e| e.to_string())?;
    let declaring = crate::typesystem::compilation::TypeDefId {
        module,
        row: declaring_row,
    };
    let declaring_name = ts
        .comp
        .type_def(declaring)
        .map_err(|e| e.to_string())?
        .full_name
        .name()
        .to_owned();
    // shortcut: the members of a C# 14 extension block take their type parameters' names from
    // `ExtensionInfo`, which the engine does not build yet; they stay with the host.
    let enclosing = ts
        .comp
        .type_def(declaring)
        .map_err(|e| e.to_string())?
        .declaring;
    let in_extension_block = enclosing != 0
        && ts
            .type_flags(crate::typesystem::compilation::TypeDefId {
                module,
                row: enclosing,
            })
            .has_extensions;
    if ts.type_flags(declaring).has_extensions || in_extension_block {
        return Err("a member of a type with extension blocks".to_owned());
    }
    let method_kind = match kind {
        S::Method => MethodKindSyntax::Method,
        S::Operator => {
            // `ConvertOperator` writes a method for a name that is no operator, and for the
            // unsigned right shift and checked operators the options do not support.
            let short = name.rsplit('.').next().unwrap_or(&name).to_owned();
            MethodKindSyntax::Operator(short)
        }
        S::Constructor => MethodKindSyntax::Constructor(declaring_name),
        S::Destructor => MethodKindSyntax::Destructor(declaring_name),
        _ => return Err("an accessor".to_owned()),
    };
    let modifiers = if matches!(kind, S::Destructor) {
        0
    } else {
        method_modifiers(ts, m, kind, options)?
    };
    let modifiers_info = ts.method_modifiers_of(m).map_err(|e| e.to_string())?;
    let flags = ts.method_flags(m, None);
    let attributes = ts.method_attributes(m).map_err(|e| e.to_string())?;
    let attributes = convert_attributes(ts, &attributes, options)?;
    let return_attributes = if matches!(kind, S::Constructor | S::Destructor) {
        Vec::new()
    } else {
        let a = ts.return_type_attributes(m).map_err(|e| e.to_string())?;
        convert_attributes(ts, &a, options)?
    };
    let (ret, params) = ts.method_signature(m);
    // Type parameters with their attributes and constraints.
    let type_params = ts
        .type_parameters(module, TableId::MethodDef, row)
        .map_err(|e| e.to_string())?;
    let gp_rows: Vec<u32> = ts
        .comp
        .module(module)
        .md
        .generic_params(TableId::MethodDef, row)
        .collect();
    if (modifiers_info.is_override) && !type_params.is_empty() {
        // shortcut: an override restates only the constraints that disambiguate `T?`
        // (`AddNullabilityDisambiguatingConstraints`); generic overrides stay with the host.
        return Err("constraints of a generic override".to_owned());
    }
    let mut type_parameters = Vec::with_capacity(type_params.len());
    let mut constraints = Vec::new();
    for (tp, &gp) in type_params.iter().zip(&gp_rows) {
        let a = ts
            .type_parameter_attributes(module, gp)
            .map_err(|e| e.to_string())?;
        type_parameters.push(TypeParameterSyntax {
            variance: match tp.variance {
                "Covariant" => 1,
                "Contravariant" => 2,
                _ => 0,
            },
            name: tp.name.clone(),
            attributes: convert_attributes(ts, &a, options)?,
        });
        let constraint_attributes = has_constraint_attributes(&ts.comp.module(module).md, gp);
        if let Some(c) = convert_constraint(ts, tp, constraint_attributes)? {
            constraints.push(c);
        }
    }
    let mut parameters = Vec::with_capacity(params.len());
    for (i, p) in params.iter().enumerate() {
        let a = ts
            .parameter_attributes(module, &params, i, Some(declaring))
            .map_err(|e| e.to_string())?;
        let ty = match *ts.data(p.ty) {
            TypeData::ByRef(e) => e,
            _ => p.ty,
        };
        let default_value = if ts
            .default_value_assignment_allowed(&params, i, Some(declaring))
            .map_err(|e| e.to_string())?
        {
            Some(
                ConstantSyntaxBuilder::new(ts)
                    .convert_raw(ty, &p.value, options.special_constants)
                    .ok_or("a default value the engine does not write")?,
            )
        } else {
            None
        };
        parameters.push(ParameterSyntax {
            ref_kind: p.ref_kind as u8,
            is_params: p.is_params,
            is_scoped: p.declared_scope != crate::typesystem::flags::ScopedKind::None,
            attributes: convert_attributes(ts, &a, options)?,
            ty,
            name: p.name.clone(),
            default_value,
        });
    }
    let has_return = !matches!(kind, S::Constructor | S::Destructor);
    Ok(MethodSyntax {
        kind: method_kind,
        modifiers,
        attributes,
        return_attributes,
        return_type: has_return.then_some(ret),
        ref_readonly: has_return
            && flags.return_read_only
            && matches!(ts.data(ret), TypeData::ByRef(_)),
        name,
        type_parameters,
        this_modifier: flags.extension && !parameters.is_empty(),
        parameters,
        constraints,
    })
}
