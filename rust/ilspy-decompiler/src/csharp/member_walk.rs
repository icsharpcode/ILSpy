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
/// import interface its members in vtable (token) order. A record's member order comes from
/// its decompiled equality members, which the engine does not analyze: an error.
pub fn member_walk(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    flags: u32,
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
    if record {
        // shortcut: the member order of records (`RecordDecompiler.FieldsAndProperties`) stays
        // with the host until RecordDecompiler is ported.
        return Err("the member order of a record".to_owned());
    }
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
        members = fields.iter().map(|&f| field(f)).collect();
        members.extend(properties.iter().map(|&p| property(p)));
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
