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
    /// For a member of an extension block, the block's marker type (its row): the member is
    /// specialized with the block's type parameters (`ExtensionInfo.GetMembersOfGroup`).
    pub extension_marker: Option<u32>,
    /// `RequiredMembers`: a field with `[RequiredMember]` takes `required` instead.
    pub required_members: bool,
    /// `FixedBuffers`: a field with `[FixedBuffer]` is written as a fixed size buffer.
    pub fixed_buffers: bool,
    /// `IntroduceRefModifiersOnStructs`: a type drops the attributes the compiler adds to a
    /// `ref struct`.
    pub ref_struct_attributes: bool,
}

/// The type parameters of the extension block whose marker type is `marker_type`, created from
/// its own GenericParam rows (`MetadataTypeParameter.Create`), with their names.
fn extension_block_parameters(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    marker_type: u32,
) -> (Vec<TypeId>, Vec<String>) {
    let marker = crate::typesystem::compilation::TypeDefId {
        module,
        row: marker_type,
    };
    let gp_rows: Vec<u32> = ts
        .comp
        .module(module)
        .md
        .generic_params(TableId::TypeDef, marker_type)
        .collect();
    let names = gp_rows
        .iter()
        .map(|&gp| {
            let md = &ts.comp.module(module).md;
            md.get::<rows::GenericParam>(gp)
                .ok()
                .and_then(|r| md.string(r.name).ok())
                .map(|n| n.into_owned())
                .unwrap_or_default()
        })
        .collect();
    let own = (0..gp_rows.len() as u32)
        .map(|index| {
            ts.intern(TypeData::TypeParam {
                owner: crate::typesystem::types::TypeParamOwner::Type(marker),
                index,
            })
        })
        .collect();
    (own, names)
}

/// `IMember.Specialize(new TypeParameterSubstitution(groupTypeParameters, null))` of a
/// declaration's signature types: the class type parameters become the block's.
fn specialize_parameters(
    ts: &mut TypeSystem<'_, '_>,
    parameters: &mut [ParameterSyntax],
    class: &[TypeId],
) {
    for p in parameters {
        p.ty = ts.substitute(p.ty, Some(class), None);
    }
}

fn specialize_constraints(
    ts: &mut TypeSystem<'_, '_>,
    constraints: &mut [ConstraintSyntax],
    class: &[TypeId],
) {
    for c in constraints {
        for b in &mut c.base_types {
            if let ConstraintTypeSyntax::Type(t) = b {
                *t = ts.substitute(*t, Some(class), None);
            }
        }
    }
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
    pub const REF: u32 = 0x20000;
    pub const REQUIRED: u32 = 0x40000;
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
    /// For a fixed size buffer (`DoDecompile(IField)` with `FixedBuffers`), its element type
    /// and length: the declaration becomes a `FixedFieldDeclaration`.
    pub fixed_buffer: Option<(TypeId, i32)>,
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
    let raw_attributes = ts
        .field_attributes(module, row)
        .map_err(|e| e.to_string())?;
    let mut attributes = convert_attributes(ts, &raw_attributes, options)?;
    // `DoDecompile(IField)`: `[RequiredMember]` becomes `required`; a `[FixedBuffer]` field is
    // a fixed size buffer of the attribute's element type and length (`IsFixedField`), and the
    // attribute goes.
    let compiler_services = "System.Runtime.CompilerServices";
    if options.required_members
        && remove_attributes(
            ts,
            &mut attributes,
            compiler_services,
            "RequiredMemberAttribute",
        )
    {
        m |= modifiers::REQUIRED;
    }
    let mut fixed_buffer = None;
    if options.fixed_buffers
        && let Some(a) = raw_attributes.iter().find(|a| {
            ts.name_and_namespace(a.ty)
                .is_some_and(|(n, ns)| n == "FixedBufferAttribute" && ns == compiler_services)
        })
        && let [element, length] = a.fixed.as_slice()
        && let crate::typesystem::attrvalue::ArgValue::Type(element) = element.value
        && let crate::typesystem::attrvalue::ArgValue::Int32(length) = length.value
    {
        fixed_buffer = Some((element, length));
        remove_attributes(
            ts,
            &mut attributes,
            compiler_services,
            "FixedBufferAttribute",
        );
    }
    let ref_readonly = matches!(ts.data(ty), TypeData::ByRef(_)) && is_ref_readonly_attribute;
    let initializer = if is_const {
        let value = ts.field_constant(module, row);
        Some(
            super::constant_syntax::convert_declared(ts, ty, &value, options.special_constants)
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
        fixed_buffer,
    })
}

/// `RemoveAttribute(entityDecl, attributeType)`: drops every attribute of the type `ns.name`,
/// whether known or unresolved; whether there was one.
fn remove_attributes(
    ts: &TypeSystem<'_, '_>,
    attributes: &mut Vec<AttributeSyntax>,
    ns: &str,
    name: &str,
) -> bool {
    let before = attributes.len();
    attributes.retain(|a| {
        !ts.name_and_namespace(a.ty)
            .is_some_and(|(n, s)| n == name && s == ns)
    });
    attributes.len() != before
}

/// The `EnumMemberDeclaration` `DoDecompile(IField)` writes for a constant of an enum: its
/// attributes, each in a section of its own and in metadata order, and its initializer.
pub struct EnumMemberSyntax {
    pub attributes: Vec<AttributeSyntax>,
    pub initializer: Option<ConstSyntax>,
}

/// The declaration of the enum member `row` of `module` (`DoDecompile(IField)` for a constant
/// of an enum): an underlying type from `sbyte` to `ulong` names the value through the other
/// members (`ConvertEnumValue`), any other one writes the constant (`ConvertConstantValue`).
pub fn convert_enum_member(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
) -> Result<EnumMemberSyntax, Unsupported> {
    let md = &ts.comp.module(module).md;
    let declaring = md.field_declaring_type(row).map_err(|e| e.to_string())?;
    let def = crate::typesystem::compilation::TypeDefId {
        module,
        row: declaring,
    };
    let underlying = ts
        .comp
        .type_def(def)
        .map_err(|e| e.to_string())?
        .enum_underlying
        .ok_or("an enum member of a type that is no enum")?;
    let attributes = ts
        .field_attributes(module, row)
        .map_err(|e| e.to_string())?;
    let attributes = attributes
        .iter()
        .map(|a| convert_attribute(ts, a))
        .collect::<Result<Vec<_>, _>>()?;
    let value = ts.field_constant(module, row);
    let initializer = if value == crate::typesystem::members::ConstValue::Null {
        None
    } else if matches!(
        underlying,
        crate::typesystem::known::KnownTypeCode::SByte
            | crate::typesystem::known::KnownTypeCode::Byte
            | crate::typesystem::known::KnownTypeCode::Int16
            | crate::typesystem::known::KnownTypeCode::UInt16
            | crate::typesystem::known::KnownTypeCode::Int32
            | crate::typesystem::known::KnownTypeCode::UInt32
            | crate::typesystem::known::KnownTypeCode::Int64
            | crate::typesystem::known::KnownTypeCode::UInt64
    ) {
        let enum_type = ts.intern_def(def);
        let val = super::constant_syntax::as_i64(&value)
            .ok_or("an enum member whose constant is no integer")?;
        Some(
            ConstantSyntaxBuilder::new(ts)
                .convert_enum_member_initializer(
                    enum_type,
                    def,
                    val,
                    row,
                    options.special_constants,
                )
                .ok_or("an enum member value the engine does not write")?,
        )
    } else {
        let ty = ts.known_type(underlying);
        Some(
            super::constant_syntax::convert_declared(ts, ty, &value, options.special_constants)
                .ok_or("a constant the engine does not write")?,
        )
    };
    Ok(EnumMemberSyntax {
        attributes,
        initializer,
    })
}

/// `RemoveObsoleteAttribute` (`only` false) and `RemoveCompilerFeatureRequiredAttribute`
/// (`only` true): drops each attribute of the defined type `ns.name` whose first argument (with
/// `only`, whose only argument) is the string `text`.
fn remove_attributes_with_string(
    ts: &TypeSystem<'_, '_>,
    attributes: &mut Vec<AttributeSyntax>,
    (ns, name): (&str, &str),
    text: &str,
    only: bool,
) {
    let text: Vec<u16> = text.encode_utf16().collect();
    attributes.retain(|a| {
        let defined = ts.definition(a.ty).is_some()
            && ts
                .name_and_namespace(a.ty)
                .is_some_and(|(n, s)| n == name && s == ns);
        let count = a.arguments.len() + a.named.len();
        let first_is_text = a.arguments.first().is_some_and(|first| {
            matches!(
                &first.kind,
                super::constant_syntax::ConstSyntaxKind::Literal {
                    value: crate::typesystem::members::ConstValue::String(s),
                    ..
                } if *s == text
            )
        });
        !(defined && first_is_text && if only { count == 1 } else { count >= 1 })
    });
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
    /// `VarianceModifier`, as the managed enum value (an `sbyte`: contravariant is -1).
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
    Ok(Some(ConstraintSyntax {
        type_parameter: simple_type_name(&tp.name),
        base_types: base,
    }))
}

/// The name `MakeSimpleType` writes.
fn simple_type_name(name: &str) -> String {
    if name == "_" {
        "@_".to_owned()
    } else {
        name.to_owned()
    }
}

/// `NullableTypeParameterCollector`: the indexes of `m`'s own type parameters that appear as
/// `T?` anywhere in `t`.
fn collect_nullable_type_parameters(
    ts: &TypeSystem<'_, '_>,
    m: crate::typesystem::members::MethodDefId,
    t: TypeId,
    found: &mut Vec<u32>,
) {
    use crate::typesystem::types::{Nullability, TypeParamOwner};
    match ts.data(t) {
        TypeData::Annotated { inner, nullability } => {
            if *nullability == Nullability::Nullable
                && let TypeData::TypeParam {
                    owner:
                        TypeParamOwner::Method(o) | TypeParamOwner::ContextMethod { method: o, .. },
                    index,
                } = *ts.data(*inner)
                && o == m
                && !found.contains(&index)
            {
                found.push(index);
            }
            collect_nullable_type_parameters(ts, m, *inner, found);
        }
        TypeData::Inst { generic, args } => {
            collect_nullable_type_parameters(ts, m, *generic, found);
            for &a in args.iter() {
                collect_nullable_type_parameters(ts, m, a, found);
            }
        }
        TypeData::Array { elem: e, .. }
        | TypeData::Pointer(e)
        | TypeData::ByRef(e)
        | TypeData::Pinned(e) => collect_nullable_type_parameters(ts, m, *e, found),
        TypeData::Modified {
            modifier, inner, ..
        } => {
            collect_nullable_type_parameters(ts, m, *modifier, found);
            collect_nullable_type_parameters(ts, m, *inner, found);
        }
        TypeData::Tuple {
            underlying,
            elements,
            ..
        } => {
            collect_nullable_type_parameters(ts, m, *underlying, found);
            for &e in elements.iter() {
                collect_nullable_type_parameters(ts, m, e, found);
            }
        }
        TypeData::FnPtr(f) => {
            for &p in std::iter::once(&f.ret).chain(f.params.iter()) {
                collect_nullable_type_parameters(ts, m, p, found);
            }
        }
        TypeData::Intersection(types) => {
            for &t in types.iter() {
                collect_nullable_type_parameters(ts, m, t, found);
            }
        }
        _ => {}
    }
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
    // The members of a C# 14 extension block are specialized with the type parameters of their
    // block (`ExtensionInfo.GetMembersOfGroup`), whose marker type the host names.
    let in_extension_block = ts.is_extension_group_type(declaring);
    let extension_parameters = match (in_extension_block, options.extension_marker) {
        (false, _) => None,
        (true, Some(marker)) => Some(extension_block_parameters(ts, module, marker).0),
        // A block without type parameters specializes nothing.
        (true, None)
            if ts
                .comp
                .type_def(declaring)
                .is_ok_and(|i| i.type_parameter_count == 0) =>
        {
            None
        }
        (true, None) => return Err("a member of an extension block outside its block".to_owned()),
    };
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
    let inherits_constraints = modifiers_info.is_override || is_explicit;
    // `AddNullabilityDisambiguatingConstraints`: the type parameters written as `T?`.
    let mut nullable_type_parameters = Vec::new();
    if inherits_constraints {
        for t in std::iter::once(ret).chain(params.iter().map(|p| p.ty)) {
            collect_nullable_type_parameters(ts, m, t, &mut nullable_type_parameters);
        }
    }
    let mut type_parameters = Vec::with_capacity(type_params.len());
    let mut constraints = Vec::new();
    for (index, (tp, &gp)) in type_params.iter().zip(&gp_rows).enumerate() {
        let a = ts
            .type_parameter_attributes(module, gp)
            .map_err(|e| e.to_string())?;
        type_parameters.push(TypeParameterSyntax {
            variance: match tp.variance {
                "Covariant" => 1,
                "Contravariant" => (-1i8) as u8,
                _ => 0,
            },
            name: tp.name.clone(),
            attributes: convert_attributes(ts, &a, options)?,
        });
        if inherits_constraints {
            // `GetNullabilityDisambiguator`: `class` for a reference type, `default` for neither
            // a reference nor a value type, nothing for a value type.
            if nullable_type_parameters.contains(&(index as u32)) {
                let owner = crate::typesystem::types::TypeParamOwner::Method(m);
                let keyword = match ts.type_parameter_is_reference_type(owner, index as u32) {
                    Some(true) => Some("class"),
                    None => Some("default"),
                    Some(false) => None,
                };
                if let Some(keyword) = keyword {
                    constraints.push(ConstraintSyntax {
                        type_parameter: simple_type_name(&tp.name),
                        base_types: vec![ConstraintTypeSyntax::Keyword(keyword, false)],
                    });
                }
            }
            continue;
        }
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
                super::constant_syntax::convert_declared(
                    ts,
                    ty,
                    &p.value,
                    options.special_constants,
                )
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
    let mut ret = ret;
    if let Some(class) = &extension_parameters {
        ret = ts.substitute(ret, Some(class), None);
        specialize_parameters(ts, &mut parameters, class);
        specialize_constraints(ts, &mut constraints, class);
    }
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
                    super::constant_syntax::convert_declared(
                        ts,
                        pty,
                        &p.value,
                        options.special_constants,
                    )
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
    // A property of an extension block is specialized with the block's type parameters.
    let mut ty = ty;
    if ts.is_extension_group_type(declaring) {
        let generic = ts
            .comp
            .type_def(declaring)
            .is_ok_and(|i| i.type_parameter_count > 0);
        match options.extension_marker {
            Some(marker) => {
                let (class, _) = extension_block_parameters(ts, module, marker);
                ty = ts.substitute(ty, Some(&class), None);
                specialize_parameters(ts, &mut parameters, &class);
            }
            None if generic => {
                return Err("a member of an extension block outside its block".to_owned());
            }
            None => {}
        }
    }
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

/// `ConvertTypeDefinition`: a type declaration's header, or a delegate declaration.
#[derive(Clone, Debug)]
pub struct TypeDeclarationSyntax {
    /// `ClassType`, as the managed enum value; `None` for a delegate.
    pub class_type: Option<u8>,
    pub modifiers: u32,
    pub attributes: Vec<AttributeSyntax>,
    pub name: String,
    pub type_parameters: Vec<TypeParameterSyntax>,
    pub base_types: Vec<TypeId>,
    pub constraints: Vec<ConstraintSyntax>,
    /// A delegate's `Invoke` signature: the return value's attributes, the return type and the
    /// parameters.
    pub return_attributes: Vec<AttributeSyntax>,
    pub return_type: Option<TypeId>,
    pub ref_readonly: bool,
    pub parameters: Vec<ParameterSyntax>,
}

/// `MetadataTypeDefinition.ComputeIsRecord`: a class or struct with the members the compiler
/// generates for records.
pub(crate) fn is_record(md: &crate::metadata::Metadata<'_>, row: u32, kind: TypeKind) -> bool {
    if !matches!(kind, TypeKind::Class | TypeKind::Struct) {
        return false;
    }
    let is_struct = kind == TypeKind::Struct;
    let (mut equality_contract, mut to_string, mut print_members, mut get_hash_code) =
        (is_struct, false, false, false);
    let (mut equals, mut op_equality, mut op_inequality, mut clone) =
        (false, false, false, is_struct);
    for m in md.type_methods(row).unwrap_or_default() {
        let Ok(def) = md.get::<rows::MethodDef>(m) else {
            continue;
        };
        let Ok(name) = md.string(def.name) else {
            continue;
        };
        // Members named `Clone` are disallowed in records (CS8859).
        if name == "Clone" {
            return false;
        }
        equality_contract |= name == "get_EqualityContract";
        to_string |= name == "ToString";
        print_members |= name == "PrintMembers";
        get_hash_code |= name == "GetHashCode";
        equals |= name == "Equals";
        op_equality |= name == "op_Equality";
        op_inequality |= name == "op_Inequality";
        clone |= name == "<Clone>$";
    }
    if is_struct && !to_string {
        return false;
    }
    equality_contract
        && print_members
        && get_hash_code
        && equals
        && op_equality
        && op_inequality
        && clone
}

/// The parameters of `ConvertParameter`, for a method, indexer or delegate.
fn convert_parameters(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    params: &[crate::typesystem::members::ParamInfo],
    declaring: crate::typesystem::compilation::TypeDefId,
    options: EntityOptions,
) -> Result<Vec<ParameterSyntax>, Unsupported> {
    let mut parameters = Vec::with_capacity(params.len());
    for (i, p) in params.iter().enumerate() {
        let a = ts
            .parameter_attributes(module, params, i, Some(declaring))
            .map_err(|e| e.to_string())?;
        let ty = match *ts.data(p.ty) {
            TypeData::ByRef(e) => e,
            _ => p.ty,
        };
        let default_value = if ts
            .default_value_assignment_allowed(params, i, Some(declaring))
            .map_err(|e| e.to_string())?
        {
            Some(
                super::constant_syntax::convert_declared(
                    ts,
                    ty,
                    &p.value,
                    options.special_constants,
                )
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
    Ok(parameters)
}

/// The type definitions `t` names, the generic types of instances included (the
/// `BaseListNameabilityVisitor` visits).
fn named_definitions(
    ts: &TypeSystem<'_, '_>,
    t: TypeId,
    out: &mut Vec<crate::typesystem::compilation::TypeDefId>,
) {
    match ts.data(t) {
        TypeData::Def(d) => out.push(*d),
        TypeData::Inst { generic, args } => {
            named_definitions(ts, *generic, out);
            for &a in args.iter() {
                named_definitions(ts, a, out);
            }
        }
        TypeData::Annotated { inner, .. } | TypeData::Modified { inner, .. } => {
            named_definitions(ts, *inner, out)
        }
        TypeData::Array { elem, .. } | TypeData::Pointer(elem) | TypeData::ByRef(elem) => {
            named_definitions(ts, *elem, out)
        }
        TypeData::Tuple { elements, .. } => {
            for &e in elements.iter() {
                named_definitions(ts, e, out);
            }
        }
        _ => {}
    }
}

/// The enclosing type definition of `def`, if it is nested.
fn enclosing(
    ts: &TypeSystem<'_, '_>,
    def: crate::typesystem::compilation::TypeDefId,
) -> Option<crate::typesystem::compilation::TypeDefId> {
    let declaring = ts.comp.type_def(def).ok()?.declaring;
    (declaring != 0).then_some(crate::typesystem::compilation::TypeDefId {
        module: def.module,
        row: declaring,
    })
}

/// `MemberLookup.IsAccessible(td, allowProtectedAccess: false)` of a type definition, looked up
/// from `current` (the enclosing type of the declared type, or none) in `module`.
fn type_accessible(
    ts: &mut TypeSystem<'_, '_>,
    td: crate::typesystem::compilation::TypeDefId,
    current: Option<crate::typesystem::compilation::TypeDefId>,
    module: ModuleId,
) -> Result<bool, Unsupported> {
    let flags = ts.comp.type_def(td).map_err(|e| e.to_string())?.flags;
    let owner = enclosing(ts, td);
    let chain = |ts: &TypeSystem<'_, '_>| {
        let mut v = Vec::new();
        let mut t = current;
        while let Some(c) = t {
            v.push(c);
            t = enclosing(ts, c);
        }
        v
    };
    // shortcut: an internal type of another module is accessible by `InternalsVisibleTo`, which
    // the engine does not read; such a base list stays with the host.
    let internal = |_: &TypeSystem<'_, '_>| {
        if td.module == module {
            Ok(true)
        } else {
            Err("a base list naming an internal type of another module".to_owned())
        }
    };
    // Protected access for a type definition: from its declaring type or a type derived from it.
    let protected = |ts: &mut TypeSystem<'_, '_>| -> bool {
        let Some(owner) = owner else {
            return false;
        };
        for t in chain(ts) {
            if t == owner {
                return true;
            }
            let ty = ts.intern_def(t);
            if ts.all_base_type_definitions(ty).contains(&owner) {
                return true;
            }
        }
        false
    };
    Ok(match crate::typesystem::attrs::type_accessibility(flags) {
        // Private: from the declaring type or a type nested in it.
        1 => owner.is_some_and(|o| chain(ts).contains(&o)),
        6 => true,
        3 => protected(ts),
        4 => internal(ts)?,
        5 => internal(ts)? || protected(ts),
        2 => internal(ts)? && protected(ts),
        _ => false,
    })
}

/// `BaseTypeAccessibleFrom(baseType, currentType, lookup)`: whether the base list of `current`
/// can name every type definition of `t`.
fn base_type_accessible_from(
    ts: &mut TypeSystem<'_, '_>,
    t: TypeId,
    current: crate::typesystem::compilation::TypeDefId,
) -> Result<bool, Unsupported> {
    let mut defs = Vec::new();
    named_definitions(ts, t, &mut defs);
    let lookup_type = enclosing(ts, current);
    for td in defs {
        // `TypeDefinitionNameableInBaseList`
        let mut td = Some(td);
        while let Some(d) = td {
            let owner = enclosing(ts, d);
            // A type may name the nested types of itself and of its enclosing types.
            let mut c = Some(current);
            let mut nested_in_scope = false;
            while let Some(x) = c {
                if owner == Some(x) {
                    nested_in_scope = true;
                    break;
                }
                c = enclosing(ts, x);
            }
            if nested_in_scope {
                break;
            }
            if !type_accessible(ts, d, lookup_type, current.module)? {
                return Ok(false);
            }
            td = owner;
        }
    }
    Ok(true)
}

/// `ConvertTypeDefinition(typeDefinition)` with the record options.
pub fn convert_type_definition(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    options: EntityOptions,
    record_classes: bool,
    record_structs: bool,
) -> Result<TypeDeclarationSyntax, Unsupported> {
    use crate::typesystem::compilation::TypeDefId;
    use crate::typesystem::known::KnownTypeCode as K;
    let def = TypeDefId { module, row };
    let info = ts.comp.type_def(def).map_err(|e| e.to_string())?;
    let (flags, kind, declaring_row, enum_underlying) =
        (info.flags, info.kind, info.declaring, info.enum_underlying);
    let name = info.full_name.name().to_owned();
    let type_flags = ts.type_flags(def);
    let md = &ts.comp.module(module).md;
    let record = is_record(md, row, kind);
    let mut m = modifier_of_accessibility(
        crate::typesystem::attrs::type_accessibility(flags),
        options.private_protected,
    );
    const ABSTRACT: u32 = 0x80;
    const SEALED: u32 = 0x100;
    if flags & (ABSTRACT | SEALED) == ABSTRACT | SEALED {
        m |= modifiers::STATIC;
    } else if flags & ABSTRACT != 0 {
        m |= modifiers::ABSTRACT;
    } else if flags & SEALED != 0 {
        m |= modifiers::SEALED;
    }
    // `GetDelegateInvokeMethod`: a delegate without `Invoke` is written as a class.
    let invoke = if kind == TypeKind::Delegate {
        md.type_methods(row)
            .map_err(|e| e.to_string())?
            .into_iter()
            .find(|&r| {
                md.get::<rows::MethodDef>(r)
                    .ok()
                    .and_then(|d| md.string(d.name).ok())
                    .is_some_and(|n| n == "Invoke")
            })
    } else {
        None
    };
    let class_type = match kind {
        TypeKind::Struct | TypeKind::Void => {
            m &= !modifiers::SEALED;
            if type_flags.read_only {
                m |= modifiers::READONLY;
            }
            if type_flags.by_ref_like {
                m |= modifiers::REF;
            }
            Some(if record_structs && record { 5 } else { 1 })
        }
        TypeKind::Enum => {
            m &= !modifiers::SEALED;
            Some(3)
        }
        TypeKind::Interface => {
            m &= !modifiers::ABSTRACT;
            Some(2)
        }
        TypeKind::Delegate if invoke.is_some() => None,
        TypeKind::Delegate | TypeKind::Class => Some(if record_classes && record { 4 } else { 0 }),
    };
    let outer = if declaring_row == 0 {
        0
    } else {
        ts.comp
            .type_def(TypeDefId {
                module,
                row: declaring_row,
            })
            .map_err(|e| e.to_string())?
            .type_parameter_count as usize
    };
    let attributes = ts.type_attributes(def).map_err(|e| e.to_string())?;
    let mut attributes = convert_attributes(ts, &attributes, options)?;
    // `DoDecompile(ITypeDefinition)` removes these from a type declaration (not a delegate's).
    if class_type.is_some() {
        if options.ref_struct_attributes {
            remove_attributes_with_string(
                ts,
                &mut attributes,
                ("System", "ObsoleteAttribute"),
                "Types with embedded references are not supported in this version of your compiler.",
                false,
            );
            remove_attributes_with_string(
                ts,
                &mut attributes,
                (
                    "System.Runtime.CompilerServices",
                    "CompilerFeatureRequiredAttribute",
                ),
                "RefStructs",
                true,
            );
        }
        if options.required_members {
            remove_attributes(
                ts,
                &mut attributes,
                "System.Runtime.CompilerServices",
                "RequiredMemberAttribute",
            );
        }
    }
    let type_params = ts
        .type_parameters(module, TableId::TypeDef, row)
        .map_err(|e| e.to_string())?;
    let gp_rows: Vec<u32> = ts
        .comp
        .module(module)
        .md
        .generic_params(TableId::TypeDef, row)
        .collect();
    let mut type_parameters = Vec::new();
    let mut constraints = Vec::new();
    for (tp, &gp) in type_params.iter().zip(&gp_rows).skip(outer) {
        let a = ts
            .type_parameter_attributes(module, gp)
            .map_err(|e| e.to_string())?;
        type_parameters.push(TypeParameterSyntax {
            variance: match tp.variance {
                "Covariant" => 1,
                "Contravariant" => (-1i8) as u8,
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
    let escaped = if name == "_" { "@_".to_owned() } else { name };
    if let Some(invoke) = invoke {
        // `ConvertDelegate`: the signature of `Invoke`.
        let invoke = crate::typesystem::members::MethodDefId {
            module,
            row: invoke,
        };
        let return_attributes = ts
            .return_type_attributes(invoke)
            .map_err(|e| e.to_string())?;
        let return_attributes = convert_attributes(ts, &return_attributes, options)?;
        let (ret, params) = ts.method_signature(invoke);
        let method_flags = ts.method_flags(invoke, None);
        let parameters = convert_parameters(ts, module, &params, def, options)?;
        return Ok(TypeDeclarationSyntax {
            class_type: None,
            modifiers: m & !modifiers::SEALED,
            attributes,
            name: escaped,
            type_parameters,
            base_types: Vec::new(),
            constraints,
            return_attributes,
            return_type: Some(ret),
            ref_readonly: method_flags.return_read_only
                && matches!(ts.data(ret), TypeData::ByRef(_)),
            parameters,
        });
    }
    let mut base_types = Vec::new();
    // `AsParameterizedType()`: the definition with its own type parameters.
    let this_type = ts.intern_def(def);
    let own_args: Vec<TypeId> = (0..type_params.len() as u32)
        .map(|index| {
            ts.intern(TypeData::TypeParam {
                owner: ts.class_param_owner(def, index),
                index,
            })
        })
        .collect();
    let self_type = if own_args.is_empty() {
        this_type
    } else {
        ts.intern(TypeData::Inst {
            generic: this_type,
            args: own_args.into(),
        })
    };
    for b in ts.direct_base_types(def).map_err(|e| e.to_string())? {
        // Interfaces enter the interface-impl metadata transitively, so entries the base list
        // cannot name are dropped; a base class was always written explicitly and stays.
        if ts.is_interface(b) && !base_type_accessible_from(ts, b, def)? {
            continue;
        }
        if kind == TypeKind::Enum && ts.is_known(b, K::Enum) {
            // The underlying type, unless it is `int`.
            let underlying = enum_underlying.ok_or("an enum without primitive underlying type")?;
            if underlying != K::Int32 {
                base_types.push(ts.known_type(underlying));
            }
        } else if !(matches!(kind, TypeKind::Struct | TypeKind::Void) && ts.is_known(b, K::ValueType)
            || ts.is_known(b, K::Object)
            // A record omits `IEquatable<R>`.
            || record_classes && record && is_equatable_of(ts, b, self_type))
        {
            base_types.push(b);
        }
    }
    Ok(TypeDeclarationSyntax {
        class_type,
        modifiers: m,
        attributes,
        name: escaped,
        type_parameters,
        base_types,
        constraints,
        return_attributes: Vec::new(),
        return_type: None,
        ref_readonly: false,
        parameters: Vec::new(),
    })
}

/// `System.IEquatable<R>` of a record `R`, which its declaration omits.
fn is_equatable_of(ts: &TypeSystem<'_, '_>, t: TypeId, self_type: TypeId) -> bool {
    let TypeData::Inst { generic, args } = ts.data(t) else {
        return false;
    };
    let Some(d) = ts.definition(*generic) else {
        return false;
    };
    ts.comp
        .type_def(d)
        .is_ok_and(|i| i.full_name.top.ns == "System" && i.full_name.name() == "IEquatable")
        && args.len() == 1
        && args[0] == self_type
}

/// `ConvertExtension(group)`: an extension block's declaration, without its members.
#[derive(Clone, Debug)]
pub struct ExtensionSyntax {
    pub type_parameters: Vec<TypeParameterSyntax>,
    /// The receiver: the marker method's parameter.
    pub receiver: ParameterSyntax,
    pub constraints: Vec<ConstraintSyntax>,
}

/// `ConvertExtension` of the block whose marker is declared in `marker_type` (the `<M>$` marker
/// type of encoding V2, or the `<>E__` group type of V1).
pub fn convert_extension(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    marker_type: u32,
    options: EntityOptions,
) -> Result<ExtensionSyntax, Unsupported> {
    use crate::typesystem::compilation::TypeDefId;
    let marker_type = TypeDefId {
        module,
        row: marker_type,
    };
    // The container is the enclosing type (V1) or the enclosing type's (V2).
    let mut container = enclosing(ts, marker_type);
    if container.is_some_and(|c| !ts.type_flags(c).has_extensions) {
        container = container.and_then(|c| enclosing(ts, c));
    }
    let container = container.ok_or("an extension block outside a container")?;
    let info = ts
        .extension_info(container)
        .ok_or("an extension block of a type without extensions")?;
    let group = info
        .groups
        .iter()
        .find(|g| {
            ts.comp
                .module(g.marker.module)
                .md
                .method_declaring_type(g.marker.row)
                .is_ok_and(|d| d == marker_type.row)
        })
        .ok_or("an extension block without marker")?
        .clone();
    // The block's type parameters are the marker type's.
    let type_params = ts
        .type_parameters(module, TableId::TypeDef, marker_type.row)
        .map_err(|e| e.to_string())?;
    let gp_rows: Vec<u32> = ts
        .comp
        .module(module)
        .md
        .generic_params(TableId::TypeDef, marker_type.row)
        .collect();
    if type_params.len() != group.type_parameters.len() {
        return Err("an extension block with other type parameters".to_owned());
    }
    // `MetadataTypeParameter.Create(module, markerType, index, handle)`: the block's type
    // parameters are the marker type's own, named by its own GenericParam rows, where the type
    // system gives a nested type's leading parameters to the enclosing grouping type (`$T0`).
    let own_names: Vec<String> = gp_rows
        .iter()
        .map(|&gp| {
            let md = &ts.comp.module(module).md;
            md.get::<rows::GenericParam>(gp)
                .ok()
                .and_then(|r| md.string(r.name).ok())
                .map(|n| n.into_owned())
                .unwrap_or_default()
        })
        .collect();
    let own: Vec<TypeId> = (0..own_names.len() as u32)
        .map(|index| {
            ts.intern(TypeData::TypeParam {
                owner: crate::typesystem::types::TypeParamOwner::Type(marker_type),
                index,
            })
        })
        .collect();
    let mut type_parameters = Vec::new();
    let mut constraints = Vec::new();
    for ((tp, &gp), name) in type_params.iter().zip(&gp_rows).zip(&own_names) {
        let a = ts
            .type_parameter_attributes(module, gp)
            .map_err(|e| e.to_string())?;
        type_parameters.push(TypeParameterSyntax {
            variance: match tp.variance {
                "Covariant" => 1,
                "Contravariant" => (-1i8) as u8,
                _ => 0,
            },
            name: name.clone(),
            attributes: convert_attributes(ts, &a, options)?,
        });
        let constraint_attributes = has_constraint_attributes(&ts.comp.module(module).md, gp);
        if let Some(mut c) = convert_constraint(ts, tp, constraint_attributes)? {
            c.type_parameter = if name == "_" {
                "".to_owned()
            } else {
                name.clone()
            };
            for b in &mut c.base_types {
                if let ConstraintTypeSyntax::Type(t) = b {
                    *t = ts.substitute(*t, Some(&own), None);
                }
            }
            constraints.push(c);
        }
    }
    // `group.MarkerMethod.Specialize(subst).Parameters.Single()`, the block's own parameters
    // substituted for the marker type's.
    let (_, params) = ts.method_signature(group.marker);
    let mut receiver = convert_parameters(ts, module, &params, marker_type, options)?;
    let mut receiver = receiver
        .pop()
        .ok_or("an extension marker without parameter")?;
    receiver.ty = ts.substitute(receiver.ty, Some(&own), None);
    Ok(ExtensionSyntax {
        type_parameters,
        receiver,
        constraints,
    })
}
