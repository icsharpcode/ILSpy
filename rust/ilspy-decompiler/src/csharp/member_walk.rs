//! The member walk of `CSharpDecompiler.DoDecompile(ITypeDefinition)`: which entities of a type
//! are decompiled, in which order (`allOrderedEntities`), and which the walk leaves out.

use crate::metadata::tables::{TableId, token};
use crate::typesystem::compilation::{ModuleId, TypeDefId, TypeKind};
use crate::typesystem::types::TypeSystem;

use super::member_hidden::{HiddenSettings, is_event_backing_field_of, member_is_hidden};

/// What the walk does with an entity.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum WalkDecision {
    /// Decompiled.
    Decompile = 0,
    /// Hidden (`MemberIsHidden`).
    Hidden = 1,
    /// A field hidden only as the backing field of an event: decompiled when the event is not
    /// automatic (`IsBackingFieldOfNonAutomaticEvent`), which the host's body analysis decides.
    HiddenUnlessEventIsCustom = 2,
}

/// `RecordClasses` and `RecordStructs`, which the walk reads beside the hidden-member settings.
pub const RECORD_CLASSES: u32 = 1 << 17;
pub const RECORD_STRUCTS: u32 = 1 << 18;

/// `allOrderedEntities` of the type `row` of `module` with the walk's decision for each, as
/// tokens: the nested types, then fields and properties, events and methods, or for a COM
/// import interface its members in vtable (token) order. A record's fields and properties come
/// in the order its generated members read them (`RecordDecompiler.FieldsAndProperties`), whose
/// bodies `analysis` reads.
pub fn member_walk(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    flags: u32,
    analysis: super::record_member_order::Analysis,
) -> Result<Vec<(u32, WalkDecision)>, String> {
    let def = TypeDefId { module, row };
    let comp = ts.comp;
    let info = comp.type_def(def).map_err(|e| e.to_string())?;
    let md = &comp.module(module).md;
    let record = match info.kind {
        TypeKind::Class => flags & RECORD_CLASSES != 0,
        TypeKind::Struct => flags & RECORD_STRUCTS != 0,
        _ => false,
    } && super::entity_syntax::is_record(md, row, info.kind);
    let record_order = if record {
        Some(super::record_member_order::fields_and_properties(
            ts, module, row, analysis,
        )?)
    } else {
        None
    };
    let fields = comp.visible_fields(def).map_err(|e| e.to_string())?;
    let properties = comp.visible_properties(def).map_err(|e| e.to_string())?;
    let events = comp.visible_events(def).map_err(|e| e.to_string())?;
    let methods: Vec<u32> = md
        .type_methods(row)
        .map_err(|e| e.to_string())?
        .into_iter()
        .filter(|&m| {
            comp.is_visible_method(module, m)
                && ts
                    .accessor_owner(crate::typesystem::members::MethodDefId { module, row: m })
                    .is_none()
        })
        .collect();
    let field = |r| token(TableId::Field, r);
    let method = |r| token(TableId::MethodDef, r);
    let property = |r| token(TableId::Property, r);
    let event = |r| token(TableId::Event, r);
    let mut members: Vec<u32>;
    // `ComHelper.IsComImport`: an interface with `TypeAttributes.Import`.
    if info.kind == TypeKind::Interface && info.flags & 0x1000 != 0 {
        // `GetMembersWithNativeOrdering`: a property by its getter (else setter), an event by
        // its adder (else remover, raiser), sorted stably by token.
        let mut keyed: Vec<(u32, u32)> = Vec::new();
        keyed.extend(fields.iter().map(|&f| (field(f), field(f))));
        for &p in &properties {
            let (get, set, _) = md.property_accessors(p);
            let key = [get, set]
                .into_iter()
                .find(|&a| a != 0)
                .map_or(property(p), method);
            keyed.push((key, property(p)));
        }
        keyed.extend(methods.iter().map(|&m| (method(m), method(m))));
        for &e in &events {
            let (add, remove, raise, _) = md.event_accessors(e);
            let key = [add, remove, raise]
                .into_iter()
                .find(|&a| a != 0)
                .map_or(event(e), method);
            keyed.push((key, event(e)));
        }
        keyed.sort_by_key(|&(key, _)| key);
        members = keyed.into_iter().map(|(_, t)| t).collect();
    } else {
        members = match record_order {
            Some(order) => order,
            None => fields
                .iter()
                .map(|&f| field(f))
                .chain(properties.iter().map(|&p| property(p)))
                .collect(),
        };
        members.extend(events.iter().map(|&e| event(e)));
        members.extend(methods.iter().map(|&m| method(m)));
    }
    let mut entities: Vec<u32> = comp
        .nested_types(def)
        .into_iter()
        .map(|n| token(TableId::TypeDef, n.row))
        .collect();
    entities.append(&mut members);
    let settings = HiddenSettings(flags);
    let without_automatic_events = HiddenSettings(flags & !HiddenSettings::AUTOMATIC_EVENTS);
    Ok(entities
        .into_iter()
        .map(|t| {
            let decision = if !member_is_hidden(ts, module, t, settings) {
                WalkDecision::Decompile
            } else if t >> 24 == 0x04
                && flags & HiddenSettings::AUTOMATIC_EVENTS != 0
                && is_event_backing_field_of(ts, module, t & 0x00FF_FFFF)
                && !member_is_hidden(ts, module, t, without_automatic_events)
            {
                WalkDecision::HiddenUnlessEventIsCustom
            } else {
                WalkDecision::Hidden
            };
            (t, decision)
        })
        .collect())
}

/// `AlwaysShowEnumMemberValues`, which `DetectBestEnumValueDisplayMode` reads.
pub const ALWAYS_SHOW_ENUM_MEMBER_VALUES: u32 = 1 << 19;

/// `EnumValueDisplayMode`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum EnumValueDisplayMode {
    None = 0,
    All = 1,
    AllHex = 2,
    FirstOnly = 3,
}

/// `DetectBestEnumValueDisplayMode` of the enum `row` of `module`: which enum member values the
/// declaration shows. `None` where a constant is no integer (`CSharpPrimitiveCast` to `long`
/// would truncate or fail), which the host decides.
pub fn enum_display_mode(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    flags: u32,
) -> Option<EnumValueDisplayMode> {
    let comp = ts.comp;
    let md = &comp.module(module).md;
    let flags_attribute = md
        .custom_attributes(TableId::TypeDef, row)
        .into_iter()
        .any(|ca| {
            md.attribute_type(ca).is_ok_and(|t| {
                crate::typesystem::compilation::is_known_type_name(
                    md,
                    t,
                    "System",
                    "FlagsAttribute",
                    0,
                )
            })
        });
    if flags_attribute {
        return Some(EnumValueDisplayMode::AllHex);
    }
    let fields = comp.visible_fields(TypeDefId { module, row }).ok()?;
    let settings = HiddenSettings(flags);
    let (mut first, mut first_value, mut previous) = (true, 0i64, 0i64);
    let (mut all_powers_of_two, mut all_consecutive) = (true, true);
    for f in fields {
        if member_is_hidden(ts, module, token(TableId::Field, f), settings) {
            continue;
        }
        let value = ts.field_constant(module, f);
        if value == crate::typesystem::members::ConstValue::Null {
            continue;
        }
        let current = super::constant_syntax::as_i64(&value)?;
        all_consecutive = all_consecutive && (first || previous.wrapping_add(1) == current);
        // N & (N - 1) == 0 iff N is a power of two, for all N != 0; 0 counts as one here.
        all_powers_of_two = all_powers_of_two && current & current.wrapping_sub(1) == 0;
        if first {
            first_value = current;
            first = false;
        } else if current <= previous || (!all_consecutive && !all_powers_of_two) {
            return Some(EnumValueDisplayMode::All);
        }
        previous = current;
    }
    if all_powers_of_two {
        if previous > 8 {
            return Some(EnumValueDisplayMode::AllHex);
        } else if !all_consecutive {
            return Some(EnumValueDisplayMode::All);
        }
    }
    if flags & ALWAYS_SHOW_ENUM_MEMBER_VALUES != 0 {
        return Some(EnumValueDisplayMode::All);
    }
    Some(if first_value == 0 {
        EnumValueDisplayMode::None
    } else {
        EnumValueDisplayMode::FirstOnly
    })
}

/// The comment `DoDecompile(IField)` puts after a field with an RVA (`.data`): its bytes as
/// `BitConverter.ToString` writes them, or the message of the `BadImageFormatException` reading
/// them throws.
pub fn field_data_comment(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> String {
    let (ty, _) = ts.field_type(module, row);
    match crate::il::transforms::transform_array_initializers::field_data(ts, module, row, ty) {
        Ok(bytes) => {
            let hex: Vec<String> = bytes.iter().map(|b| format!("{b:02X}")).collect();
            format!(" Not supported: data({}) ", hex.join(" "))
        }
        Err(message) => message,
    }
}

/// Whether `DoDecompile(ITypeDefinition)` removes `[DefaultMember]` from the declaration of a
/// type whose member walk is `walk`: it does when the declaration has an indexer that is no
/// explicit interface implementation, that is, when the walk decompiles such an indexer.
pub fn removes_default_member(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    walk: &[(u32, WalkDecision)],
) -> bool {
    walk.iter().any(|&(t, decision)| {
        if decision != WalkDecision::Decompile || t >> 24 != 0x17 {
            return false;
        }
        let row = t & 0x00FF_FFFF;
        ts.property_symbol_kind(module, row)
            .is_ok_and(|k| k == crate::typesystem::members::SymbolKind::Indexer)
            && !ts
                .property_any_accessor(module, row)
                .is_some_and(|a| ts.is_explicit_interface_implementation(a).unwrap_or(false))
    })
}
