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
    /// The interface of an explicit implementation.
    pub private_implementation_type: Option<TypeId>,
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
    is_explicit: bool,
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
        _ if is_explicit => false,
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
        if modifiers.is_override && !is_explicit {
            r |= modifiers::OVERRIDE;
        }
        if modifiers.is_sealed && !is_explicit {
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
    let md = &ts.comp.module(module).md;
    let def = md.get::<rows::MethodDef>(row).map_err(|e| e.to_string())?;
    let name = md.string(def.name).map_err(|e| e.to_string())?.into_owned();
    // `IsExplicitInterfaceImplementation`: a dotted name with a MethodImpl.
    let implemented = ts.explicit_implementations(m).map_err(|e| e.to_string())?;
    let is_explicit = name.contains('.') && !implemented.is_empty();
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
        method_modifiers(ts, m, kind, options, is_explicit)?
    };
    // `GetExplicitInterfaceType`: the interface of the first member implemented, as the
    // declaring type implements it (`GetInterfaceAsImplementedBy`).
    let private_implementation_type = match implemented.first().filter(|_| is_explicit) {
        Some(first) => {
            let interface = first
                .declaring
                .ok_or("an implemented member without declaring type")?;
            let this_type = ts.intern_def(declaring);
            let n = crate::typesystem::fnptr::Normalization::IGNORE_NULLABILITY_AND_TUPLES;
            let wanted = ts.normalize_with(interface, n);
            let bases = ts.all_base_types(this_type).map_err(|e| e.to_string())?;
            Some(
                bases
                    .into_iter()
                    .find(|&b| ts.is_interface(b) && ts.normalize_with(b, n) == wanted)
                    .unwrap_or(interface),
            )
        }
        None => None,
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
    if (modifiers_info.is_override || is_explicit) && !type_params.is_empty() {
        // shortcut: an override or explicit implementation restates only the constraints that
        // disambiguate `T?` (`AddNullabilityDisambiguatingConstraints`); generic ones stay with
        // the host.
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
        private_implementation_type,
    })
}

/// `ConvertAccessor`.
#[derive(Clone, Debug)]
pub struct AccessorSyntax {
    /// The accessor's `MethodDef`, whose `IMethod` the host takes from the owner.
    pub row: u32,
    /// `AccessorKind`, as the managed enum value.
    pub kind: u8,
    pub is_init_only: bool,
    pub modifiers: u32,
    pub attributes: Vec<AttributeSyntax>,
    pub return_attributes: Vec<AttributeSyntax>,
    /// The value parameter's attributes (`[param: ...]`) of a setter, adder or remover.
    pub parameter_attributes: Vec<AttributeSyntax>,
}

/// `ConvertProperty`, `ConvertIndexer` or `ConvertEvent`.
#[derive(Clone, Debug)]
pub struct PropertySyntax {
    /// 0 property, 1 indexer, 2 field-like event, 3 custom event.
    pub kind: u8,
    pub modifiers: u32,
    pub attributes: Vec<AttributeSyntax>,
    pub return_type: TypeId,
    pub ref_readonly: bool,
    pub name: String,
    pub parameters: Vec<ParameterSyntax>,
    /// The getter and setter, or the adder and remover.
    pub accessors: [Option<AccessorSyntax>; 2],
    pub private_implementation_type: Option<TypeId>,
}

/// `ModifierFromAccessibility` of a managed `Accessibility` value.
fn modifier_of_accessibility(value: u8, private_protected: bool) -> u32 {
    use modifiers::*;
    match value {
        1 => PRIVATE,
        2 if private_protected => PRIVATE | PROTECTED,
        2 => PROTECTED,
        3 => PROTECTED,
        4 => INTERNAL,
        5 => PROTECTED | INTERNAL,
        6 => PUBLIC,
        _ => 0,
    }
}

/// The interface of an explicit implementation through `accessor`, as `declaring` implements it.
fn explicit_interface_of(
    ts: &mut TypeSystem<'_, '_>,
    accessor: crate::typesystem::members::MethodDefId,
    declaring: crate::typesystem::compilation::TypeDefId,
) -> Result<Option<TypeId>, Unsupported> {
    let md = &ts.comp.module(accessor.module).md;
    let def = md
        .get::<rows::MethodDef>(accessor.row)
        .map_err(|e| e.to_string())?;
    let name = md.string(def.name).map_err(|e| e.to_string())?.into_owned();
    let implemented = ts
        .explicit_implementations(accessor)
        .map_err(|e| e.to_string())?;
    let Some(first) = implemented.first().filter(|_| name.contains('.')) else {
        return Ok(None);
    };
    let interface = first
        .declaring
        .ok_or("an implemented member without declaring type")?;
    let this_type = ts.intern_def(declaring);
    let n = crate::typesystem::fnptr::Normalization::IGNORE_NULLABILITY_AND_TUPLES;
    let wanted = ts.normalize_with(interface, n);
    let bases = ts.all_base_types(this_type).map_err(|e| e.to_string())?;
    Ok(Some(
        bases
            .into_iter()
            .find(|&b| ts.is_interface(b) && ts.normalize_with(b, n) == wanted)
            .unwrap_or(interface),
    ))
}

/// `ConvertAccessor(accessor, kind, ownerAccessibility, addParameterAttribute)`.
#[allow(clippy::too_many_arguments)]
fn convert_accessor(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    kind: u8,
    owner_accessibility: u8,
    parameter_attribute: bool,
    declaring: crate::typesystem::compilation::TypeDefId,
    options: EntityOptions,
    init_accessors: bool,
) -> Result<AccessorSyntax, Unsupported> {
    use crate::typesystem::members::MethodDefId;
    let m = MethodDefId { module, row };
    let attributes = ts.method_attributes(m).map_err(|e| e.to_string())?;
    let attributes = convert_attributes(ts, &attributes, options)?;
    let return_attributes = ts.return_type_attributes(m).map_err(|e| e.to_string())?;
    let return_attributes = convert_attributes(ts, &return_attributes, options)?;
    let (_, params) = ts.method_signature(m);
    let parameter_attributes = if parameter_attribute && !params.is_empty() {
        let a = ts
            .parameter_attributes(module, &params, params.len() - 1, Some(declaring))
            .map_err(|e| e.to_string())?;
        convert_attributes(ts, &a, options)?
    } else {
        Vec::new()
    };
    let accessibility = ts.method_accessibility_of(m).map_err(|e| e.to_string())?;
    let mut modifiers = 0;
    if accessibility != owner_accessibility {
        modifiers = modifier_of_accessibility(accessibility, options.private_protected);
    }
    let flags = ts.method_flags(m, None);
    // `HasReadonlyModifier`: a readonly accessor of a struct that is not itself readonly.
    if flags.this_read_only && !ts.type_flags(declaring).read_only {
        modifiers |= modifiers::READONLY;
    }
    // A setter of an init-only property is an `init` accessor where the option supports it.
    let kind = if kind == 2 && init_accessors && flags.init_only {
        3
    } else {
        kind
    };
    Ok(AccessorSyntax {
        row,
        kind,
        is_init_only: flags.init_only,
        modifiers,
        attributes,
        return_attributes,
        parameter_attributes,
    })
}

/// `MergeReadOnlyModifiers`: `readonly` moves to the declaration where every accessor has it.
fn merge_read_only(modifiers: &mut u32, accessors: &mut [Option<AccessorSyntax>; 2]) {
    let readonly = |a: &Option<AccessorSyntax>| {
        a.as_ref()
            .is_some_and(|a| a.modifiers & modifiers::READONLY != 0)
    };
    if accessors[0].is_none() || !readonly(&accessors[0]) {
        return;
    }
    if accessors[1].is_none() || readonly(&accessors[1]) {
        for a in accessors.iter_mut().flatten() {
            a.modifiers &= !modifiers::READONLY;
        }
        *modifiers |= modifiers::READONLY;
    }
}

/// `GetMemberModifiers(member)` of a property or event, whose modifiers are those of its first
/// accessor and whose accessibility is `accessibility`.
fn accessor_owner_modifiers(
    ts: &mut TypeSystem<'_, '_>,
    any: crate::typesystem::members::MethodDefId,
    accessibility: u8,
    in_interface: bool,
    is_explicit: bool,
    options: EntityOptions,
) -> Result<u32, Unsupported> {
    let modifiers_info = ts.method_modifiers_of(any).map_err(|e| e.to_string())?;
    let mut r = 0;
    let needs_accessibility = !is_explicit && (!in_interface || accessibility != 6);
    if needs_accessibility {
        r |= modifier_of_accessibility(accessibility, options.private_protected);
    }
    if modifiers_info.is_static {
        r |= modifiers::STATIC;
    }
    if in_interface {
        if modifiers_info.is_static {
            if modifiers_info.is_abstract {
                r |= modifiers::ABSTRACT;
            } else if modifiers_info.is_virtual && !modifiers_info.is_override {
                r |= modifiers::VIRTUAL;
            }
        }
    } else {
        if modifiers_info.is_abstract {
            r |= modifiers::ABSTRACT;
        } else if modifiers_info.is_virtual && !modifiers_info.is_override {
            r |= modifiers::VIRTUAL;
        }
        if modifiers_info.is_override && !is_explicit {
            r |= modifiers::OVERRIDE;
        }
        if modifiers_info.is_sealed && !is_explicit {
            r |= modifiers::SEALED;
        }
    }
    Ok(r)
}

/// `ConvertProperty(property)` or `ConvertIndexer(indexer)`.
pub fn convert_property(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
    init_accessors: bool,
) -> Result<PropertySyntax, Unsupported> {
    use crate::typesystem::members::{MethodDefId, SymbolKind as S};
    let md = &ts.comp.module(module).md;
    let (getter, setter, _) = md.property_accessors(row);
    let any = if getter != 0 { getter } else { setter };
    if any == 0 {
        return Err("a property without accessors".to_owned());
    }
    let p = md.get::<rows::Property>(row).map_err(|e| e.to_string())?;
    let name = md.string(p.name).map_err(|e| e.to_string())?.into_owned();
    let ref_readonly_attribute = crate::typesystem::flags::has_attribute(
        md,
        TableId::Property,
        row,
        "System.Runtime.CompilerServices",
        "IsReadOnlyAttribute",
    );
    let declaring = crate::typesystem::compilation::TypeDefId {
        module,
        row: md.method_declaring_type(any).map_err(|e| e.to_string())?,
    };
    let kind = ts
        .property_symbol_kind(module, row)
        .map_err(|e| e.to_string())?;
    let any_method = MethodDefId { module, row: any };
    // `MetadataProperty.ComputeAccessibility`, with an override of one accessor copying the
    // base member's.
    let accessibility = ts
        .member_accessibility(crate::typesystem::inheritance::MemberDef::Property(
            module, row,
        ))
        .map_err(|e| e.to_string())?;
    let in_interface = ts
        .comp
        .type_def(declaring)
        .is_ok_and(|i| i.kind == TypeKind::Interface);
    let private_implementation_type = explicit_interface_of(ts, any_method, declaring)?;
    let is_explicit = private_implementation_type.is_some();
    let mut modifiers = accessor_owner_modifiers(
        ts,
        any_method,
        accessibility,
        in_interface,
        is_explicit,
        options,
    )?;
    let attributes = ts
        .property_attributes(module, row)
        .map_err(|e| e.to_string())?;
    let attributes = convert_attributes(ts, &attributes, options)?;
    let (ty, params) = ts
        .property_signature(module, row)
        .map_err(|e| e.to_string())?;
    let indexer = matches!(kind, S::Indexer);
    let mut parameters = Vec::new();
    if indexer {
        for (i, p) in params.iter().enumerate() {
            let a = ts
                .parameter_attributes(module, &params, i, Some(declaring))
                .map_err(|e| e.to_string())?;
            let pty = match *ts.data(p.ty) {
                TypeData::ByRef(e) => e,
                _ => p.ty,
            };
            let default_value = if ts
                .default_value_assignment_allowed(&params, i, Some(declaring))
                .map_err(|e| e.to_string())?
            {
                Some(
                    ConstantSyntaxBuilder::new(ts)
                        .convert_raw(pty, &p.value, options.special_constants)
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
                ty: pty,
                name: p.name.clone(),
                default_value,
            });
        }
    } else if !params.is_empty() {
        return Err("a property with parameters".to_owned());
    }
    let get = if getter != 0 {
        Some(convert_accessor(
            ts,
            module,
            getter,
            1,
            accessibility,
            false,
            declaring,
            options,
            init_accessors,
        )?)
    } else {
        None
    };
    let set = if setter != 0 {
        Some(convert_accessor(
            ts,
            module,
            setter,
            2,
            accessibility,
            true,
            declaring,
            options,
            init_accessors,
        )?)
    } else {
        None
    };
    let mut accessors = [get, set];
    merge_read_only(&mut modifiers, &mut accessors);
    Ok(PropertySyntax {
        kind: if indexer { 1 } else { 0 },
        modifiers,
        attributes,
        return_type: ty,
        ref_readonly: matches!(ts.data(ty), TypeData::ByRef(_)) && ref_readonly_attribute,
        name,
        parameters,
        accessors,
        private_implementation_type,
    })
}

/// `ConvertEvent(ev)`, with `UseCustomEvents` as `DoDecompile(IEvent)` sets it.
pub fn convert_event(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
    custom_events: bool,
) -> Result<PropertySyntax, Unsupported> {
    use crate::typesystem::members::MethodDefId;
    let md = &ts.comp.module(module).md;
    let (adder, remover, raiser, _) = md.event_accessors(row);
    // `GetAny`: the adder, else the remover, else the raiser.
    let any = [adder, remover, raiser].into_iter().find(|&r| r != 0);
    let Some(any) = any else {
        return Err("an event without accessors".to_owned());
    };
    let e = md.get::<rows::Event>(row).map_err(|e| e.to_string())?;
    let name = md.string(e.name).map_err(|e| e.to_string())?.into_owned();
    let declaring = crate::typesystem::compilation::TypeDefId {
        module,
        row: md.method_declaring_type(any).map_err(|e| e.to_string())?,
    };
    let any_method = MethodDefId { module, row: any };
    // `MetadataEvent.Accessibility`: the first accessor's.
    let accessibility = ts
        .method_accessibility_of(any_method)
        .map_err(|e| e.to_string())?;
    let in_interface = ts
        .comp
        .type_def(declaring)
        .is_ok_and(|i| i.kind == TypeKind::Interface);
    let private_implementation_type = explicit_interface_of(ts, any_method, declaring)?;
    let is_explicit = private_implementation_type.is_some();
    let mut modifiers = accessor_owner_modifiers(
        ts,
        any_method,
        accessibility,
        in_interface,
        is_explicit,
        options,
    )?;
    let attributes = ts
        .event_attributes(module, row)
        .map_err(|e| e.to_string())?;
    let attributes = convert_attributes(ts, &attributes, options)?;
    let ty = ts.event_type(module, row).map_err(|e| e.to_string())?;
    let mut accessors = [None, None];
    if custom_events {
        for (slot, (r, kind)) in [(adder, 4), (remover, 5)].into_iter().enumerate() {
            if r != 0 {
                accessors[slot] = Some(convert_accessor(
                    ts,
                    module,
                    r,
                    kind,
                    accessibility,
                    true,
                    declaring,
                    options,
                    false,
                )?);
            }
        }
        merge_read_only(&mut modifiers, &mut accessors);
    }
    Ok(PropertySyntax {
        kind: if custom_events { 3 } else { 2 },
        modifiers,
        attributes,
        return_type: ty,
        ref_readonly: false,
        name,
        parameters: Vec::new(),
        accessors,
        private_implementation_type: if custom_events {
            private_implementation_type
        } else {
            None
        },
    })
}
