//! `CSharpDecompiler.SetNewModifier`: whether a member or nested type hides an accessible member
//! of a base class and its declaration takes the `new` modifier.

use crate::metadata::tables::TableId;
use crate::typesystem::compilation::{ModuleId, TypeDefId};
use crate::typesystem::inheritance::MemberDef;
use crate::typesystem::members::{MethodDefId, SymbolKind};
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// Why the engine leaves the decision to the host.
pub type Unsupported = String;

/// `HidesMemberOrTypeOfBaseType` for the TypeDef, Field, MethodDef, Property or Event `token` of
/// `module`: a field, property, event or nested type hides every accessible base class member
/// or nested type of the same name; a method hides the non-method members of the same name and
/// the methods with the same signature; an indexer hides the indexers with the same signature.
pub fn hides_base_member(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    token: u32,
) -> Result<bool, Unsupported> {
    let row = token & 0x00FF_FFFF;
    let entity = match token >> 24 {
        0x02 => None,
        0x04 => Some(MemberDef::Field(module, row)),
        0x06 => Some(MemberDef::Method(MethodDefId { module, row })),
        0x17 => Some(MemberDef::Property(module, row)),
        0x14 => Some(MemberDef::Event(module, row)),
        _ => return Err(format!("0x{token:08x} is no member token")),
    };
    let e = |e: crate::metadata::Error| e.to_string();
    let (name, kind, declaring) = match entity {
        None => {
            let def = TypeDefId { module, row };
            let info = ts.comp.type_def(def).map_err(e)?;
            let name = info.full_name.name().to_owned();
            let outer = ts.comp.module(module).md.declaring_type(row).map_err(e)?;
            (name, None, outer)
        }
        Some(m) => {
            let name = ts.member_name(m).map_err(e)?;
            let kind = ts.member_symbol_kind(m).map_err(e)?;
            (name, Some(kind), member_declaring_row(ts, m)?)
        }
    };
    if declaring == 0 {
        return Ok(false);
    }
    let current = TypeDefId {
        module,
        row: declaring,
    };
    let declaring_type = ts.intern_def(current);
    let bases: Vec<TypeId> = ts
        .non_interface_base_types(declaring_type)
        .map_err(e)?
        .into_iter()
        .filter(|&t| t != declaring_type)
        .collect();
    // A constant, field, property, event or type hides all base class members of its name.
    let by_signature = !matches!(
        kind,
        None | Some(SymbolKind::Field | SymbolKind::Property | SymbolKind::Event)
    );
    for &base in &bases {
        let Some(base_def) = ts.definition(base) else {
            continue;
        };
        if !by_signature {
            // `GetNestedTypes`/`GetMembers` with `IgnoreInheritedMembers | ReturnMemberDefinitions`.
            for nested in ts.comp.nested_types(base_def) {
                let info = ts.comp.type_def(nested).map_err(e)?;
                if info.full_name.name() == name {
                    let access = crate::typesystem::attrs::type_accessibility(info.flags);
                    if is_accessible(ts, access, Some(base_def), nested.module, current, module)? {
                        return Ok(true);
                    }
                }
            }
            for m in declared_members(ts, base_def)? {
                if ts.member_symbol_kind(m).map_err(e)? != SymbolKind::Indexer
                    && ts.member_name(m).map_err(e)? == name
                    && member_accessible(ts, m, base_def, current, module)?
                {
                    return Ok(true);
                }
            }
            continue;
        }
        let Some(entity) = entity else {
            continue;
        };
        match kind {
            Some(SymbolKind::Indexer) => {
                // `baseType.GetProperties(p => indexer && accessible)`: specialized and inherited.
                for (m, through) in inherited_members(ts, base)? {
                    if matches!(m, MemberDef::Property(..))
                        && ts.member_symbol_kind(m).map_err(e)? == SymbolKind::Indexer
                        && member_accessible(ts, m, declaring_def(ts, through)?, current, module)?
                        && same_parameters(ts, entity, declaring_type, m, through)?
                    {
                        return Ok(true);
                    }
                }
            }
            Some(SymbolKind::Method) => {
                // A method hides the non-method members of its name and the methods of its
                // signature (type parameter count, parameter types and modifiers).
                for (m, through) in inherited_members(ts, base)? {
                    let candidate_kind = ts.member_symbol_kind(m).map_err(e)?;
                    if matches!(
                        candidate_kind,
                        SymbolKind::Indexer | SymbolKind::Constructor | SymbolKind::Destructor
                    ) || ts.member_name(m).map_err(e)? != name
                        || !member_accessible(ts, m, declaring_def(ts, through)?, current, module)?
                    {
                        continue;
                    }
                    if candidate_kind != SymbolKind::Method
                        || (type_parameter_count(ts, entity) == type_parameter_count(ts, m)
                            && same_parameters(ts, entity, declaring_type, m, through)?)
                    {
                        return Ok(true);
                    }
                }
            }
            _ => {}
        }
    }
    Ok(false)
}

fn member_declaring_row(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> Result<u32, Unsupported> {
    let comp = ts.comp;
    let result = match m {
        MemberDef::Field(module, row) => comp.module(module).md.field_declaring_type(row),
        MemberDef::Method(id) => comp.module(id.module).md.method_declaring_type(id.row),
        MemberDef::Property(module, row) => match ts.property_any_accessor(module, row) {
            Some(a) => comp.module(module).md.method_declaring_type(a.row),
            None => Ok(0),
        },
        MemberDef::Event(module, row) => match ts.event_any_accessor(module, row) {
            Some(a) => comp.module(module).md.method_declaring_type(a.row),
            None => Ok(0),
        },
    };
    result.map_err(|e| e.to_string())
}

fn declaring_def(ts: &TypeSystem<'_, '_>, t: TypeId) -> Result<TypeDefId, Unsupported> {
    ts.definition(t)
        .ok_or_else(|| "a base type without definition".to_owned())
}

/// The members `ITypeDefinition` lists for the definition `def` (fields, methods that are no
/// accessor or constructor, properties, events): `GetMembers` with `IgnoreInheritedMembers`.
fn declared_members(
    ts: &mut TypeSystem<'_, '_>,
    def: TypeDefId,
) -> Result<Vec<MemberDef>, Unsupported> {
    let comp = ts.comp;
    let e = |e: crate::metadata::Error| e.to_string();
    let md = &comp.module(def.module).md;
    let mut members: Vec<MemberDef> = Vec::new();
    for row in md.type_methods(def.row).map_err(e)? {
        let m = MethodDefId {
            module: def.module,
            row,
        };
        if ts.accessor_owner(m).is_none()
            && comp.is_visible_method(def.module, row)
            && ts.member_symbol_kind(MemberDef::Method(m)).map_err(e)? != SymbolKind::Constructor
        {
            members.push(MemberDef::Method(m));
        }
    }
    members.extend(
        comp.visible_properties(def)
            .map_err(e)?
            .into_iter()
            .map(|r| MemberDef::Property(def.module, r)),
    );
    members.extend(
        comp.visible_fields(def)
            .map_err(e)?
            .into_iter()
            .map(|r| MemberDef::Field(def.module, r)),
    );
    members.extend(
        comp.visible_events(def)
            .map_err(e)?
            .into_iter()
            .map(|r| MemberDef::Event(def.module, r)),
    );
    Ok(members)
}

/// `GetMembers` without options on `base`: the members of it and of its non-interface base
/// types, each with the type that specializes it.
fn inherited_members(
    ts: &mut TypeSystem<'_, '_>,
    base: TypeId,
) -> Result<Vec<(MemberDef, TypeId)>, Unsupported> {
    let chain = ts
        .non_interface_base_types(base)
        .map_err(|e| e.to_string())?;
    let mut result = Vec::new();
    for t in chain {
        let Some(def) = ts.definition(t) else {
            continue;
        };
        for m in declared_members(ts, def)? {
            result.push((m, t));
        }
    }
    Ok(result)
}

fn type_parameter_count(ts: &TypeSystem<'_, '_>, m: MemberDef) -> usize {
    match m {
        MemberDef::Method(id) => ts
            .comp
            .module(id.module)
            .md
            .generic_params(TableId::MethodDef, id.row)
            .len(),
        _ => 0,
    }
}

/// `ParameterListComparer.WithOptions(includeModifiers: true).Equals` of the parameters of
/// `a` (declared by `a_declaring`) and `b` (seen through `b_through`, whose type arguments
/// specialize it): count, reference kinds, `params`, and the normalized types.
fn same_parameters(
    ts: &mut TypeSystem<'_, '_>,
    a: MemberDef,
    a_declaring: TypeId,
    b: MemberDef,
    b_through: TypeId,
) -> Result<bool, Unsupported> {
    let pa = parameters(ts, a, a_declaring)?;
    let pb = parameters(ts, b, b_through)?;
    if pa.len() != pb.len() {
        return Ok(false);
    }
    for ((ta, ka, xa), (tb, kb, xb)) in pa.into_iter().zip(pb) {
        if ka != kb || xa != xb || ts.normalize_parameter(ta) != ts.normalize_parameter(tb) {
            return Ok(false);
        }
    }
    Ok(true)
}

/// The parameters of `m` specialized by `through`: type, reference kind, `params`.
fn parameters(
    ts: &mut TypeSystem<'_, '_>,
    m: MemberDef,
    through: TypeId,
) -> Result<Vec<(TypeId, crate::typesystem::members::RefKind, bool)>, Unsupported> {
    let params = match m {
        MemberDef::Method(id) => ts.method_signature(id).1,
        MemberDef::Property(module, row) => {
            ts.property_signature(module, row)
                .map_err(|e| e.to_string())?
                .1
        }
        MemberDef::Field(..) | MemberDef::Event(..) => Vec::new(),
    };
    let args = match ts.data(through) {
        TypeData::Inst { args, .. } => Some(args.to_vec()),
        _ => None,
    };
    Ok(params
        .iter()
        .map(|p| {
            (
                ts.substitute(p.ty, args.as_deref(), None),
                p.ref_kind,
                p.is_params,
            )
        })
        .collect())
}

fn member_accessible(
    ts: &mut TypeSystem<'_, '_>,
    m: MemberDef,
    declaring: TypeDefId,
    current: TypeDefId,
    current_module: ModuleId,
) -> Result<bool, Unsupported> {
    let access = ts.member_accessibility(m).map_err(|e| e.to_string())?;
    let member_module = match m {
        MemberDef::Field(module, _)
        | MemberDef::Property(module, _)
        | MemberDef::Event(module, _) => module,
        MemberDef::Method(id) => id.module,
    };
    is_accessible(
        ts,
        access,
        Some(declaring),
        member_module,
        current,
        current_module,
    )
}

/// `MemberLookup.IsAccessible(entity, allowProtectedAccess: true)` looked up from the type
/// `current` of `current_module`, for an entity with the accessibility `access` (`Accessibility`
/// values), declared in `declaring` of `module`. Protected access holds from the declaring type
/// and the types derived from it, and from the types nested in those.
pub(crate) fn is_accessible(
    ts: &mut TypeSystem<'_, '_>,
    access: u8,
    declaring: Option<TypeDefId>,
    module: ModuleId,
    current: TypeDefId,
    current_module: ModuleId,
) -> Result<bool, Unsupported> {
    let mut chain = Vec::new();
    let mut t = Some(current);
    while let Some(c) = t {
        chain.push(c);
        let outer = ts
            .comp
            .module(c.module)
            .md
            .declaring_type(c.row)
            .unwrap_or(0);
        t = (outer != 0).then_some(TypeDefId {
            module: c.module,
            row: outer,
        });
    }
    let protected = |ts: &mut TypeSystem<'_, '_>| -> bool {
        let Some(owner) = declaring else {
            return false;
        };
        chain.iter().any(|&c| {
            if c == owner {
                return true;
            }
            let ty = ts.intern_def(c);
            ts.all_base_type_definitions(ty).contains(&owner)
        })
    };
    // shortcut: an internal entity of another module is accessible by `InternalsVisibleTo`,
    // which the engine does not read; such a decision stays with the host.
    let internal = || {
        if module == current_module {
            Ok(true)
        } else {
            Err("an internal member of another module".to_owned())
        }
    };
    Ok(match access {
        1 => declaring.is_some_and(|d| chain.contains(&d)),
        6 => true,
        3 => protected(ts),
        4 => internal()?,
        5 => protected(ts) || internal()?,
        2 => protected(ts) && internal()?,
        _ => false,
    })
}
