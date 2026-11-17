//! `CSharpDecompiler.MemberIsHidden`: the types, methods and fields a decompilation leaves out
//! because a transform folds them into the code that uses them (closures, state machines,
//! backing fields, caches) or because they have no C# declaration.

use crate::il::control_flow::yield_return_decompiler::is_compiler_generated as type_is_compiler_generated;
use crate::il::transforms::local_function_decompiler::{
    is_local_function_display_class, is_local_function_method,
};
use crate::il::transforms::transform_collection_and_object_initializers::is_anonymous_type;
use crate::metadata::Metadata;
use crate::metadata::bytes::Bytes;
use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{
    ModuleId, TypeDefId, is_known_type, is_known_type_name, type_def_full_name, type_ref_full_name,
};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::MethodDefId;
use crate::typesystem::types::TypeSystem;

/// The `DecompilerSettings` `MemberIsHidden` reads, as the host passes them.
#[derive(Clone, Copy, Debug, Default)]
pub struct HiddenSettings(pub u32);

impl HiddenSettings {
    pub const LOCAL_FUNCTIONS: u32 = 1 << 0;
    pub const ANONYMOUS_METHODS: u32 = 1 << 1;
    pub const YIELD_RETURN: u32 = 1 << 2;
    pub const ASYNC_AWAIT: u32 = 1 << 3;
    pub const ASYNC_ENUMERATOR: u32 = 1 << 4;
    pub const FIXED_BUFFERS: u32 = 1 << 5;
    pub const INLINE_ARRAYS: u32 = 1 << 6;
    pub const EXTENSION_MEMBERS: u32 = 1 << 7;
    pub const ARRAY_INITIALIZERS: u32 = 1 << 8;
    pub const ANONYMOUS_TYPES: u32 = 1 << 9;
    pub const DYNAMIC: u32 = 1 << 10;
    pub const SWITCH_STATEMENT_ON_STRING: u32 = 1 << 11;
    pub const PRIMARY_CONSTRUCTORS: u32 = 1 << 12;
    pub const AUTOMATIC_PROPERTIES: u32 = 1 << 13;
    pub const FIELD_KEYWORD: u32 = 1 << 14;
    pub const GETTER_ONLY_AUTOMATIC_PROPERTIES: u32 = 1 << 15;
    pub const AUTOMATIC_EVENTS: u32 = 1 << 16;

    fn has(self, flag: u32) -> bool {
        self.0 & flag != 0
    }
}

/// `MemberIsHidden(module, member, settings)` for a TypeDef, MethodDef or Field token of
/// `module`; any other token is never hidden.
pub fn member_is_hidden(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    token: u32,
    settings: HiddenSettings,
) -> bool {
    let row = token & 0x00FF_FFFF;
    if row == 0 {
        return false;
    }
    match token >> 24 {
        0x06 => method_is_hidden(ts, MethodDefId { module, row }, settings),
        0x02 => type_is_hidden(ts, TypeDefId { module, row }, settings),
        0x04 => field_is_hidden(ts, module, row, settings),
        _ => false,
    }
}

/// `IsGeneratedName`: `<` first or `$` anywhere.
fn is_generated_name(name: &str) -> bool {
    name.starts_with('<') || name.contains('$')
}

fn method_is_hidden(ts: &mut TypeSystem<'_, '_>, method: MethodDefId, s: HiddenSettings) -> bool {
    let md = &ts.comp.module(method.module).md;
    let Ok(def) = md.get::<rows::MethodDef>(method.row) else {
        return false;
    };
    // `MethodSemanticsLookup` knows getters, setters, adders and removers only.
    if ts.accessor_owner(method).is_some() {
        return true;
    }
    let md = &ts.comp.module(method.module).md;
    let name = md
        .string(def.name)
        .map(|n| n.into_owned())
        .unwrap_or_default();
    let declaring = md.method_declaring_type(method.row).unwrap_or(0);
    // `TypeAttributes.Import` (0x1000) on the declaring type of a body-less constructor.
    if name == ".ctor"
        && def.rva == 0
        && md
            .get::<rows::TypeDef>(declaring)
            .is_ok_and(|t| t.flags & 0x1000 != 0)
    {
        return true;
    }
    if md.pe.is_some() && is_accessor_interface_implementation_runtime_helper(md, method.row) {
        return true;
    }
    if s.has(HiddenSettings::LOCAL_FUNCTIONS) && is_local_function_method(ts, method) {
        return true;
    }
    if s.has(HiddenSettings::ANONYMOUS_METHODS)
        && is_generated_name(&name)
        && ts.has_compiler_generated(method.module, TableId::MethodDef, method.row)
    {
        return name != "<Extension>$";
    }
    if s.has(HiddenSettings::ASYNC_AWAIT) {
        // `AsyncAwaitDecompiler.IsCompilerGeneratedMainMethod`.
        let md = &ts.comp.module(method.module).md;
        let entry_point = md.pe.as_ref().map_or(0, |pe| pe.cli.entry_point);
        if entry_point == (0x06 << 24 | method.row) && name == "<Main>" {
            return true;
        }
    }
    false
}

fn type_is_hidden(ts: &mut TypeSystem<'_, '_>, def: TypeDefId, s: HiddenSettings) -> bool {
    let md = &ts.comp.module(def.module).md;
    let Ok(row) = md.get::<rows::TypeDef>(def.row) else {
        return false;
    };
    let name = md
        .string(row.name)
        .map(|n| n.into_owned())
        .unwrap_or_default();
    let declaring = md.declaring_type(def.row).unwrap_or(0);
    if declaring != 0 {
        if s.has(HiddenSettings::LOCAL_FUNCTIONS) && is_local_function_display_class(ts, def) {
            return true;
        }
        let md = &ts.comp.module(def.module).md;
        if s.has(HiddenSettings::ANONYMOUS_METHODS) && is_closure_type(md, def.row, &name) {
            return true;
        }
        if s.has(HiddenSettings::YIELD_RETURN)
            && implements_generated(md, def.row, "System.Collections", "IEnumerator", 0)
        {
            return true;
        }
        if s.has(HiddenSettings::ASYNC_AWAIT)
            && implements_generated(
                md,
                def.row,
                "System.Runtime.CompilerServices",
                "IAsyncStateMachine",
                0,
            )
        {
            return true;
        }
        if s.has(HiddenSettings::ASYNC_ENUMERATOR)
            && implements_generated(
                md,
                def.row,
                "System.Collections.Generic",
                "IAsyncEnumerator",
                1,
            )
        {
            return true;
        }
        if s.has(HiddenSettings::FIXED_BUFFERS)
            && name.starts_with('<')
            && name.contains("__FixedBuffer")
        {
            return true;
        }
        if s.has(HiddenSettings::INLINE_ARRAYS)
            && name.starts_with("<>y__InlineArray")
            && name.ends_with("`1")
        {
            return true;
        }
        if s.has(HiddenSettings::EXTENSION_MEMBERS)
            && (name.starts_with("<>E__") || name.starts_with("<G>$"))
        {
            return true;
        }
    } else if type_is_compiler_generated(md, def.row) {
        if s.has(HiddenSettings::ARRAY_INITIALIZERS)
            && name.starts_with("<PrivateImplementationDetails>")
        {
            return true;
        }
        if s.has(HiddenSettings::ANONYMOUS_TYPES) && is_anonymous_type(ts, def) {
            return true;
        }
        let md = &ts.comp.module(def.module).md;
        let delegate = row.extends.token().is_ok_and(|t| {
            t & 0x00FF_FFFF != 0 && is_known_type(md, t, KnownTypeCode::MulticastDelegate)
        });
        if s.has(HiddenSettings::ANONYMOUS_METHODS)
            && delegate
            && name.starts_with("VB$AnonymousDelegate_")
        {
            return true;
        }
        if s.has(HiddenSettings::DYNAMIC)
            && delegate
            && (name.starts_with("<>A") || name.starts_with("<>F"))
        {
            return true;
        }
    }
    s.has(HiddenSettings::ARRAY_INITIALIZERS)
        && s.has(HiddenSettings::SWITCH_STATEMENT_ON_STRING)
        && name.starts_with("<PrivateImplementationDetails>")
}

/// `IsClosureType`.
fn is_closure_type(md: &Metadata<'_>, type_def: u32, name: &str) -> bool {
    let generated = is_generated_name(name);
    if name.contains("DisplayClass")
        || name.contains("AnonStorey")
        || name.contains("Closure$")
        || name.contains("<>c")
    {
        return generated || type_is_compiler_generated(md, type_def);
    }
    if !generated || !type_is_compiler_generated(md, type_def) {
        return false;
    }
    let Ok(row) = md.get::<rows::TypeDef>(type_def) else {
        return false;
    };
    row.extends
        .token()
        .is_ok_and(|t| is_known_type(md, t, KnownTypeCode::Object))
        && md.type_interface_impls(type_def).is_empty()
}

/// A nested type that is, or whose declaring type is,
/// compiler-generated and implements the top-level interface `ns.name` with `tpc` type
/// parameters: `IsCompilerGeneratorEnumerator`, `IsCompilerGeneratorAsyncEnumerator` and
/// `IsCompilerGeneratedStateMachine`.
fn implements_generated(md: &Metadata<'_>, type_def: u32, ns: &str, name: &str, tpc: u32) -> bool {
    let declaring = md.declaring_type(type_def).unwrap_or(0);
    if declaring == 0
        || !(type_is_compiler_generated(md, type_def) || type_is_compiler_generated(md, declaring))
    {
        return false;
    }
    md.type_interface_impls(type_def).any(|ii| {
        md.get::<rows::InterfaceImpl>(ii)
            .and_then(|r| r.interface.token())
            .is_ok_and(|t| is_known_type_name(md, t, ns, name, tpc))
    })
}

fn field_is_hidden(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    field: u32,
    s: HiddenSettings,
) -> bool {
    let md = &ts.comp.module(module).md;
    let Ok(def) = md.get::<rows::Field>(field) else {
        return false;
    };
    let name = md
        .string(def.name)
        .map(|n| n.into_owned())
        .unwrap_or_default();
    let declaring = md.field_declaring_type(field).unwrap_or(0);
    if ts.has_compiler_generated(module, TableId::Field, field) {
        if s.has(HiddenSettings::ANONYMOUS_METHODS)
            && (name.starts_with("CS$<>")
                || name.starts_with("<>f__am")
                || name.starts_with("<>f__mg"))
        {
            return true;
        }
        if s.has(HiddenSettings::PRIMARY_CONSTRUCTORS)
            && name.starts_with('<')
            && name.ends_with(">P")
        {
            return true;
        }
        if (s.has(HiddenSettings::AUTOMATIC_PROPERTIES) || s.has(HiddenSettings::FIELD_KEYWORD))
            && let Some(property) = property_of_backing_field(md, declaring, field)
        {
            if has_accessed_through_property_attribute(md, field) {
                return false;
            }
            // A getter-only property keeps its field where getter-only automatic
            // properties are switched off.
            if !s.has(HiddenSettings::GETTER_ONLY_AUTOMATIC_PROPERTIES) {
                let (getter, setter, _) = md.property_accessors(property);
                if getter != 0 && setter == 0 {
                    return false;
                }
            }
            return true;
        }
        if s.has(HiddenSettings::SWITCH_STATEMENT_ON_STRING) && name.starts_with("<>f__switch") {
            return true;
        }
    }
    let md = &ts.comp.module(module).md;
    if s.has(HiddenSettings::AUTOMATIC_EVENTS) && is_event_backing_field(md, declaring, field) {
        return true;
    }
    if s.has(HiddenSettings::ARRAY_INITIALIZERS)
        && md
            .get::<rows::TypeDef>(declaring)
            .ok()
            .and_then(|t| md.string(t.name).ok())
            .is_some_and(|n| n.starts_with("<PrivateImplementationDetails>"))
    {
        if name.starts_with("__StaticArrayInit") || name.starts_with("$$method") {
            return true;
        }
        if field_type_name_starts_with(md, def.signature, "__StaticArrayInit") {
            return true;
        }
    }
    false
}

/// `PropertyAndEventBackingFieldLookup.IsPropertyBackingField`: the property of `field`'s type
/// whose backing field it is by name (`<Name>k__BackingField`, or a compiler-generated
/// `_Name`). The lookup visits the properties in order and the last one wins.
fn property_of_backing_field(md: &Metadata<'_>, type_def: u32, field: u32) -> Option<u32> {
    let fields = md.type_fields(type_def).ok()?;
    let field_name = |f: u32| {
        md.get::<rows::Field>(f)
            .ok()
            .and_then(|r| md.string(r.name).ok())
            .map(|n| n.into_owned())
            .unwrap_or_default()
    };
    let own = field_name(field);
    let names: Vec<String> = fields.iter().map(|&f| field_name(f)).collect();
    let mut found = None;
    for p in md.type_properties(type_def).ok()? {
        let Some(name) = md
            .get::<rows::Property>(p)
            .ok()
            .and_then(|r| md.string(r.name).ok())
        else {
            continue;
        };
        let backing = format!("<{name}>k__BackingField");
        if names.contains(&backing) {
            if own == backing {
                found = Some(p);
            }
        } else if own == format!("_{name}") {
            // only reached for a compiler-generated field

            found = Some(p);
        }
    }
    found
}

/// `PropertyAndEventBackingFieldLookup.IsEventBackingField` of a Field row of `module`.
pub fn is_event_backing_field_of(ts: &TypeSystem<'_, '_>, module: ModuleId, field: u32) -> bool {
    let md = &ts.comp.module(module).md;
    let declaring = md.field_declaring_type(field).unwrap_or(0);
    is_event_backing_field(md, declaring, field)
}

/// `PropertyAndEventBackingFieldLookup.IsEventBackingField`: a field named like an event of
/// its type, or like it with an `Event` suffix where no event has that name.
fn is_event_backing_field(md: &Metadata<'_>, type_def: u32, field: u32) -> bool {
    let string = |idx| md.string(idx).map(|n| n.into_owned()).unwrap_or_default();
    let Ok(fields) = md.type_fields(type_def) else {
        return false;
    };
    let names: Vec<String> = fields
        .iter()
        .filter_map(|&f| md.get::<rows::Field>(f).ok())
        .map(|r| string(r.name))
        .collect();
    let Some(own) = md.get::<rows::Field>(field).ok().map(|r| string(r.name)) else {
        return false;
    };
    let events: Vec<String> = md
        .type_events(type_def)
        .unwrap_or_default()
        .into_iter()
        .filter_map(|e| md.get::<rows::Event>(e).ok())
        .map(|r| string(r.name))
        .collect();
    events.iter().any(|name| {
        if names.contains(name) {
            own == *name
        } else {
            let suffixed = format!("{name}Event");
            !events.contains(&suffixed) && names.contains(&suffixed) && own == suffixed
        }
    })
}

/// `HasAccessedThroughPropertyAttribute`.
fn has_accessed_through_property_attribute(md: &Metadata<'_>, field: u32) -> bool {
    md.custom_attributes(TableId::Field, field)
        .into_iter()
        .any(|ca| {
            let Ok(t) = md.attribute_type(ca) else {
                return false;
            };
            let full = match t >> 24 {
                0x01 => type_ref_full_name(md, t & 0x00FF_FFFF),
                0x02 => type_def_full_name(md, t & 0x00FF_FFFF),
                _ => return false,
            };
            full.is_ok_and(|n| {
                let simple = n
                    .nested
                    .last()
                    .map_or(n.top.name.as_str(), |(s, _)| s.as_str());
                simple == "AccessedThroughPropertyAttribute"
            })
        })
}

/// Whether the `FullTypeName` of a field's type (arrays, pointers, references and
/// instantiations unwrapped, as `FullTypeNameSignatureDecoder` does) starts with `prefix`.
fn field_type_name_starts_with(
    md: &Metadata<'_>,
    signature: crate::metadata::heaps::BlobIdx,
    prefix: &str,
) -> bool {
    let Ok(blob) = md.blob(signature) else {
        return false;
    };
    let mut b = Bytes::new(blob);
    if b.u8().ok() != Some(0x06) {
        return false;
    }
    loop {
        let Ok(code) = b.compressed_u32() else {
            return false;
        };
        match code {
            // custom modifiers: skip the modifier type
            0x1F | 0x20 => {
                if b.compressed_u32().is_err() {
                    return false;
                }
            }
            0x0F | 0x10 | 0x1D | 0x45 | 0x14 | 0x15 => {}
            0x11 | 0x12 => {
                let Ok(raw) = b.compressed_u32() else {
                    return false;
                };
                let row = raw >> 2;
                let full = match raw & 3 {
                    0 => type_def_full_name(md, row),
                    1 => type_ref_full_name(md, row),
                    _ => return false,
                };
                return full.is_ok_and(|n| {
                    let mut text = if n.top.ns.is_empty() {
                        n.top.name.clone()
                    } else {
                        format!("{}.{}", n.top.ns, n.top.name)
                    };
                    if n.top.tpc > 0 {
                        text.push_str(&format!("`{}", n.top.tpc));
                    }
                    text.starts_with(prefix)
                });
            }
            // primitive types are named in `System`; type parameters and function pointers
            // have no name.
            _ => return false,
        }
    }
}

/// `IsAccessorInterfaceImplementationRuntimeHelper`: an instance method that implements an
/// accessor of an interface explicitly by forwarding all its arguments to a method of the same
/// name and signature (`call` + `ret`).
fn is_accessor_interface_implementation_runtime_helper(md: &Metadata<'_>, method: u32) -> bool {
    let Ok(def) = md.get::<rows::MethodDef>(method) else {
        return false;
    };
    if def.flags & 0x0010 != 0 {
        return false;
    }
    let Ok(raw_name) = md.string(def.name) else {
        return false;
    };
    let Some(dot) = raw_name.rfind('.') else {
        return false;
    };
    let name = &raw_name[dot + 1..];
    let declaring = md.method_declaring_type(method).unwrap_or(0);
    let implements = md.type_method_impls(declaring).into_iter().any(|mi| {
        md.get::<rows::MethodImpl>(mi)
            .and_then(|r| r.body.token())
            .is_ok_and(|t| t == (0x06 << 24 | method))
    });
    if !implements || def.rva == 0 {
        return false;
    }
    if !["get_", "set_", "add_", "remove_", "raise_"]
        .iter()
        .any(|p| name.starts_with(p))
    {
        return false;
    }
    let Ok(signature) = md.blob(def.signature) else {
        return false;
    };
    let Some((generic_count, parameter_count)) = read_parameter_count(signature) else {
        return false;
    };
    let min = (parameter_count + 1) as usize + 5 + 1;
    let max = 4 * (parameter_count + 1) as usize + 5 + 1;
    let Ok(body) = md.method_body(def.rva) else {
        return false;
    };
    let code = body.code;
    if code.len() < min || code.len() > max {
        return false;
    }
    let mut b = Bytes::new(code);
    for i in 0..=parameter_count {
        let index = match b.u8() {
            Ok(0xFE) => match b.u8() {
                Ok(0x09) => b.u16().map(u32::from),
                _ => return false,
            },
            Ok(0x0E) => b.u8().map(u32::from),
            Ok(op @ 0x02..=0x05) => Ok(u32::from(op - 0x02)),
            _ => return false,
        };
        if index.ok() != Some(i) {
            return false;
        }
    }
    if b.remaining() < 6 || b.u8().ok() != Some(0x28) {
        return false;
    }
    let Ok(target) = b.u32() else {
        return false;
    };
    if b.u8().ok() != Some(0x2A) || b.remaining() != 0 {
        return false;
    }
    let target_row = target & 0x00FF_FFFF;
    if target_row == 0 {
        return false;
    }
    let (other_signature, other_name) = match target >> 24 {
        0x06 if generic_count == 0 => match md.get::<rows::MethodDef>(target_row) {
            Ok(r) => (r.signature, r.name),
            Err(_) => return false,
        },
        0x0A if generic_count == 0 => match method_member_ref(md, target_row) {
            Some(x) => x,
            None => return false,
        },
        0x2B if generic_count != 0 => {
            let Ok(spec) = md.get::<rows::MethodSpec>(target_row) else {
                return false;
            };
            if !md
                .blob(spec.instantiation)
                .is_ok_and(|i| is_identity_instantiation(i, generic_count))
            {
                return false;
            }
            match spec.method.decode() {
                Ok((TableId::MethodDef, row)) => match md.get::<rows::MethodDef>(row) {
                    Ok(r) => (r.signature, r.name),
                    Err(_) => return false,
                },
                Ok((TableId::MemberRef, row)) => match method_member_ref(md, row) {
                    Some(x) => x,
                    None => return false,
                },
                _ => return false,
            }
        }
        _ => return false,
    };
    if md.string(other_name).ok().as_deref() != Some(name) {
        return false;
    }
    md.blob(other_signature).is_ok_and(|other| {
        equals_method_signature(&mut Bytes::new(signature), &mut Bytes::new(other), true)
    })
}

/// A MemberRef that references a method (its signature is no field signature).
fn method_member_ref(
    md: &Metadata<'_>,
    row: u32,
) -> Option<(
    crate::metadata::heaps::BlobIdx,
    crate::metadata::heaps::StrIdx,
)> {
    let r = md.get::<rows::MemberRef>(row).ok()?;
    let first = md.blob(r.signature).ok()?.first().copied();
    // `MemberReference.GetKind`: the header kind of a field signature is 6.
    (first.map(|h| h & 0x0F) != Some(0x06)).then_some((r.signature, r.name))
}

/// `IsIdentityInstantiation`: `GENERICINST count MVAR 0 .. MVAR count-1`.
fn is_identity_instantiation(blob: &[u8], expected: u32) -> bool {
    let mut b = Bytes::new(blob);
    if b.u8().ok() != Some(0x0A) || b.compressed_u32().ok() != Some(expected) {
        return false;
    }
    (0..expected).all(|i| b.u8().ok() == Some(0x1E) && b.compressed_u32().ok() == Some(i))
}

/// `SignatureBlobComparer.ReadParameterCount`; `None` where either count is unreadable.
fn read_parameter_count(signature: &[u8]) -> Option<(u32, u32)> {
    let mut b = Bytes::new(signature);
    let header = b.u8().ok()?;
    let generic = if header & 0x10 != 0 {
        b.compressed_u32().ok()?
    } else {
        0
    };
    Some((generic, b.compressed_u32().ok()?))
}

/// `SignatureBlobComparer.EqualsMethodSignature` of two signatures of one module.
fn equals_method_signature(a: &mut Bytes<'_>, b: &mut Bytes<'_>, skip_modifiers: bool) -> bool {
    if a.is_empty() || b.is_empty() {
        return false;
    }
    let (Ok(header), Ok(other)) = (a.u8(), b.u8()) else {
        return false;
    };
    if header != other {
        return false;
    }
    if header & 0x10 != 0 && same_compressed(a, b).is_none() {
        return false;
    }
    let Some(count) = same_compressed(a, b) else {
        return false;
    };
    if !type_slot_equals(a, b, skip_modifiers) {
        return false;
    }
    let mut i = 0;
    while i < count {
        let (Ok(mut code_a), Ok(mut code_b)) = (a.compressed_u32(), b.compressed_u32()) else {
            return false;
        };
        // varargs sentinel
        if code_a == 0x41 || code_b == 0x41 {
            if code_a != code_b {
                return false;
            }
            break;
        }
        if skip_modifiers {
            let (Some(x), Some(y)) = (
                skip_custom_modifiers(a, code_a),
                skip_custom_modifiers(b, code_b),
            ) else {
                return false;
            };
            code_a = x;
            code_b = y;
        }
        if code_a != code_b || !types_are_equal(a, b, code_a, skip_modifiers) {
            return false;
        }
        i += 1;
    }
    while i < count {
        if !type_slot_equals(a, b, skip_modifiers) {
            return false;
        }
        i += 1;
    }
    true
}

fn skip_custom_modifiers(r: &mut Bytes<'_>, mut code: u32) -> Option<u32> {
    while code == 0x1F || code == 0x20 {
        r.compressed_u32().ok()?;
        code = r.compressed_u32().ok()?;
    }
    Some(code)
}

fn type_slot_equals(a: &mut Bytes<'_>, b: &mut Bytes<'_>, skip_modifiers: bool) -> bool {
    if !skip_modifiers {
        return same_compressed(a, b).is_some_and(|code| types_are_equal(a, b, code, false));
    }
    let (Ok(code_a), Ok(code_b)) = (a.compressed_u32(), b.compressed_u32()) else {
        return false;
    };
    match (
        skip_custom_modifiers(a, code_a),
        skip_custom_modifiers(b, code_b),
    ) {
        (Some(x), Some(y)) => x == y && types_are_equal(a, b, x, true),
        _ => false,
    }
}

fn same_compressed(a: &mut Bytes<'_>, b: &mut Bytes<'_>) -> Option<u32> {
    let x = a.compressed_u32().ok()?;
    let y = b.compressed_u32().ok()?;
    (x == y).then_some(x)
}

fn same_compressed_signed(a: &mut Bytes<'_>, b: &mut Bytes<'_>) -> bool {
    matches!((a.compressed_i32(), b.compressed_i32()), (Ok(x), Ok(y)) if x == y)
}

/// `TypesAreEqual`: nested type codes are compared without skipping modifiers, so a modifier
/// below the top of a slot makes the signatures unequal.
fn types_are_equal(a: &mut Bytes<'_>, b: &mut Bytes<'_>, code: u32, skip_modifiers: bool) -> bool {
    match code {
        0x01..=0x0E | 0x16 | 0x18 | 0x19 | 0x1C => true,
        0x0F | 0x10 | 0x45 | 0x1D => {
            same_compressed(a, b).is_some_and(|c| types_are_equal(a, b, c, skip_modifiers))
        }
        0x1B => equals_method_signature(a, b, skip_modifiers),
        0x14 => {
            if !same_compressed(a, b).is_some_and(|c| types_are_equal(a, b, c, skip_modifiers)) {
                return false;
            }
            if same_compressed(a, b).is_none() {
                return false;
            }
            let Some(sizes) = same_compressed(a, b) else {
                return false;
            };
            if !(0..sizes).all(|_| same_compressed(a, b).is_some()) {
                return false;
            }
            let Some(bounds) = same_compressed(a, b) else {
                return false;
            };
            (0..bounds).all(|_| same_compressed_signed(a, b))
        }
        0x15 => {
            if !same_compressed(a, b).is_some_and(|c| types_are_equal(a, b, c, skip_modifiers)) {
                return false;
            }
            let Some(count) = same_compressed(a, b) else {
                return false;
            };
            (0..count).all(|_| {
                same_compressed(a, b).is_some_and(|c| types_are_equal(a, b, c, skip_modifiers))
            })
        }
        0x13 | 0x1E => same_compressed(a, b).is_some(),
        // `TypeHandleEquals` within one module: the same non-nil TypeDefOrRefOrSpec.
        0x11 | 0x12 => match (a.compressed_u32(), b.compressed_u32()) {
            (Ok(x), Ok(y)) => x >> 2 != 0 && y >> 2 != 0 && x == y,
            _ => false,
        },
        _ => false,
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn signatures_compare_like_the_blob_comparer() {
        // instance void (int32, class [TypeRef 1])
        let a = [0x20, 0x02, 0x01, 0x08, 0x12, 0x05];
        assert!(equals_method_signature(
            &mut Bytes::new(&a),
            &mut Bytes::new(&a),
            true
        ));
        // a top-level modifier is skipped, a nested one is not
        let b = [0x20, 0x02, 0x01, 0x20, 0x05, 0x08, 0x12, 0x05];
        assert!(equals_method_signature(
            &mut Bytes::new(&a),
            &mut Bytes::new(&b),
            true
        ));
        assert!(!equals_method_signature(
            &mut Bytes::new(&a),
            &mut Bytes::new(&b),
            false
        ));
        let c = [0x20, 0x01, 0x01, 0x1D, 0x20, 0x05, 0x08];
        let d = [0x20, 0x01, 0x01, 0x1D, 0x08];
        assert!(!equals_method_signature(
            &mut Bytes::new(&c),
            &mut Bytes::new(&d),
            true
        ));
        // a different type reference
        let e = [0x20, 0x02, 0x01, 0x08, 0x12, 0x09];
        assert!(!equals_method_signature(
            &mut Bytes::new(&a),
            &mut Bytes::new(&e),
            true
        ));
        assert_eq!(read_parameter_count(&[0x30, 0x02, 0x01]), Some((2, 1)));
        assert!(is_identity_instantiation(
            &[0x0A, 0x02, 0x1E, 0x00, 0x1E, 0x01],
            2
        ));
        assert!(!is_identity_instantiation(
            &[0x0A, 0x02, 0x1E, 0x01, 0x1E, 0x00],
            2
        ));
    }
}
