//! `IL/Transforms/TransformCollectionAndObjectInitializers.cs`: the stores that follow a
//! new object (`Add` calls, property setters, field stores, also nested and indexed) become
//! one collection, object or `with` initializer block.

use crate::il::ast::{BlockKind, Function, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::typesystem::attrs::{
    ACC_INTERNAL, ACC_NONE, ACC_PRIVATE, ACC_PROTECTED, ACC_PROTECTED_AND_INTERNAL,
    ACC_PROTECTED_OR_INTERNAL, ACC_PUBLIC,
};
use crate::typesystem::compilation::{ModuleId, TypeDefId, TypeKind};
use crate::typesystem::inheritance::MemberDef;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{MethodDefId, SymbolKind};
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin, ResolvedField, ResolvedMethod};
use crate::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{inline_one_if_possible, known, options};
use super::statement_transform::StatementTransformContext;

/// The table of a Property token.
const PROPERTY_TABLE: u32 = 0x17;
/// `MethodSemanticsAttributes.Getter`.
const GETTER: u16 = 0x02;

fn new_block(
    f: &mut Function,
    kind: BlockKind,
    instructions: Vec<NodeId>,
    final_inst: NodeId,
) -> NodeId {
    f.add(Inst::Block {
        kind,
        instructions,
        final_inst,
    })
}

fn stloc(f: &mut Function, var: VarId, value: NodeId) -> NodeId {
    f.add(Inst::StLoc {
        var,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    })
}

/// The type definition of `t`.
fn type_def(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<TypeDefId> {
    ctx.ts.definition(t)
}

/// `ITypeDefinition.IsRecord` (`MetadataTypeDefinition.ComputeIsRecord`): a class or struct
/// with the members the compiler generates for a record.
fn is_record(ctx: &mut Context<'_, '_, '_>, def: TypeDefId) -> bool {
    let Ok(info) = ctx.ts.comp.type_def(def) else {
        return false;
    };
    let is_struct = match info.kind {
        TypeKind::Class => false,
        TypeKind::Struct => true,
        _ => return false,
    };
    let md = &ctx.ts.comp.module(def.module).md;
    let Ok(methods) = md.type_methods(def.row) else {
        return false;
    };
    let mut get_equality_contract = is_struct;
    let mut to_string = false;
    let mut print_members = false;
    let mut get_hash_code = false;
    let mut equals = false;
    let mut op_equality = false;
    let mut op_inequality = false;
    let mut clone = is_struct;
    for row in methods {
        let Ok(name) = md
            .get::<crate::metadata::tables::rows::MethodDef>(row)
            .and_then(|m| md.string(m.name).map(|n| n.into_owned()))
        else {
            continue;
        };
        // error CS8859: Members named 'Clone' are disallowed in records.
        if name == "Clone" {
            return false;
        }
        get_equality_contract |= name == "get_EqualityContract";
        to_string |= name == "ToString";
        print_members |= name == "PrintMembers";
        get_hash_code |= name == "GetHashCode";
        equals |= name == "Equals";
        op_equality |= name == "op_Equality";
        op_inequality |= name == "op_Inequality";
        clone |= name == "<Clone>$";
    }
    // A record class may inherit ToString; a record struct needs its own, which calls
    // PrintMembers.
    if is_struct && !to_string {
        return false;
    }
    get_equality_contract
        && print_members
        && get_hash_code
        && equals
        && op_equality
        && op_inequality
        && clone
}

/// `IsRecordCloneMethodCall`: `<Clone>$` of a record with one argument.
fn is_record_clone_method_call(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = f.inst(inst) else {
        return false;
    };
    let (name, args, declaring) = (
        c.method.method.name.clone(),
        c.args.len(),
        c.method.method.declaring,
    );
    let Some(def) = declaring.and_then(|d| type_def(ctx, d)) else {
        return false;
    };
    is_record(ctx, def) && name == "<Clone>$" && args == 1
}

/// `HandleEmptyWith`: `<Clone>$` outside an initializer becomes a `with` initializer block
/// without initializers, which `run` may extend later.
pub(crate) fn handle_empty_with(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
    if !ctx.settings.with_expressions {
        return false;
    }
    let parent = f.node(inst).parent.expect("a parent");
    // Already transformed.
    if matches!(f.inst(parent), Inst::StLoc { .. })
        && f.node(parent).parent.is_some_and(|p| {
            matches!(
                f.inst(p),
                Inst::Block {
                    kind: BlockKind::WithInitializer,
                    ..
                }
            )
        })
    {
        return false;
    }
    if !is_record_clone_method_call(f, ctx, inst) {
        return false;
    }
    let insertion_index = f.node(inst).child_index as usize;
    let ty = f.infer_type(ctx.ts, inst);
    let final_slot = f.register_variable(ctx.ts, VariableKind::InitializerTarget, ty);
    let final_inst = f.add(Inst::LdLoc { var: final_slot });
    let store = stloc(f, final_slot, inst);
    let block = new_block(f, BlockKind::WithInitializer, vec![store], final_inst);
    f.set_child_at(parent, insertion_index, block);
    true
}

/// `TypeContainsInitOnlyProperties`.
fn type_contains_init_only_properties(
    ctx: &mut Context<'_, '_, '_>,
    def: Option<TypeDefId>,
) -> bool {
    let Some(def) = def else {
        return false;
    };
    let md = &ctx.ts.comp.module(def.module).md;
    let Ok(properties) = md.type_properties(def.row) else {
        return false;
    };
    let setters: Vec<(u32, u32)> = properties
        .iter()
        .map(|&p| (p, md.property_accessors(p).1))
        .filter(|&(_, set)| set != 0)
        .collect();
    setters.into_iter().any(|(p, set)| {
        let token = crate::metadata::tables::token(crate::metadata::tables::TableId::Property, p);
        ctx.ts
            .method_flags(
                MethodDefId {
                    module: def.module,
                    row: set,
                },
                Some(token),
            )
            .init_only
    })
}

/// `IsGeneratedName`.
fn is_generated_name(name: &str) -> bool {
    name.starts_with('<') || name.contains('$')
}

/// `IsAnonymousType` of a type definition.
pub(crate) fn is_anonymous_type(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    let Ok(info) = ts.comp.type_def(def) else {
        return false;
    };
    let name = info.full_name.name().to_owned();
    if !(info.full_name.top.ns.is_empty()
        && is_generated_name(&name)
        && (name.contains("AnonType") || name.contains("AnonymousType")))
    {
        return false;
    }
    if !ts.type_is_compiler_generated(def) {
        return false;
    }
    // Only an anonymous type without settable properties (C#, not VB) counts.
    let md = &ts.comp.module(def.module).md;
    md.type_properties(def.row)
        .is_ok_and(|ps| ps.iter().all(|&p| md.property_accessors(p).1 == 0))
}

/// `ContainsAnonymousType`.
pub(crate) fn contains_anonymous_type(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> bool {
    match ts.data(t).clone() {
        TypeData::Def(d) => is_anonymous_type(ts, d),
        TypeData::Inst { generic, args } => {
            contains_anonymous_type(ts, generic)
                || args.iter().any(|&a| contains_anonymous_type(ts, a))
        }
        TypeData::Array { elem, .. }
        | TypeData::Pointer(elem)
        | TypeData::ByRef(elem)
        | TypeData::Pinned(elem) => contains_anonymous_type(ts, elem),
        TypeData::Modified {
            modifier, inner, ..
        } => contains_anonymous_type(ts, modifier) || contains_anonymous_type(ts, inner),
        TypeData::Annotated { inner, .. } => contains_anonymous_type(ts, inner),
        TypeData::Tuple { elements, .. } => {
            elements.iter().any(|&e| contains_anonymous_type(ts, e))
        }
        TypeData::FnPtr(fp) => {
            contains_anonymous_type(ts, fp.ret)
                || fp.params.iter().any(|&p| contains_anonymous_type(ts, p))
        }
        _ => false,
    }
}

/// `TupleType.RestPosition`.
const TUPLE_REST: usize = 8;

/// `TupleType.IsTupleCompatible`: the cardinality of a tuple or `System.ValueTuple` type.
fn tuple_cardinality(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<usize> {
    if let TypeData::Tuple { elements, .. } = ctx.ts.data(t) {
        return Some(elements.len());
    }
    let def = type_def(ctx, t)?;
    let info = ctx.ts.comp.type_def(def).ok()?;
    // C# requires System.ValueTuple to be a struct.
    if info.kind != TypeKind::Struct
        || info.full_name.top.ns != "System"
        || info.full_name.name() != "ValueTuple"
    {
        return None;
    }
    let tpc = info.type_parameter_count as usize;
    if tpc > 0 && tpc < TUPLE_REST {
        Some(tpc)
    } else if tpc == TUPLE_REST
        && let TypeData::Inst { args, .. } = ctx.ts.data(t).clone()
    {
        Some(tuple_cardinality(ctx, args[TUPLE_REST - 1])? + TUPLE_REST - 1)
    } else {
        None
    }
}

/// `TupleTransform.MatchTupleConstruction`.
fn match_tuple_construction(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    mut newobj: NodeId,
) -> bool {
    let Inst::NewObj(c) = f.inst(newobj) else {
        return false;
    };
    let Some(mut element_count) = c
        .method
        .method
        .declaring
        .and_then(|d| tuple_cardinality(ctx, d))
    else {
        return false;
    };
    while element_count >= TUPLE_REST {
        let Inst::NewObj(c) = f.inst(newobj) else {
            return false;
        };
        if c.args.len() != TUPLE_REST {
            return false;
        }
        element_count -= TUPLE_REST - 1;
        newobj = *c.args.last().expect("eight arguments");
        let Inst::NewObj(rest) = f.inst(newobj) else {
            return false;
        };
        match rest
            .method
            .method
            .declaring
            .and_then(|d| tuple_cardinality(ctx, d))
        {
            Some(n) if n == element_count => {}
            _ => return false,
        }
    }
    let Inst::NewObj(c) = f.inst(newobj) else {
        return false;
    };
    c.args.len() == element_count
}

/// The state of one `run` (the transform's fields).
#[derive(Default)]
struct Scan {
    /// `possibleIndexVariables`: index variables and the position of their store (-1 once an
    /// initializer uses them).
    possible_index_variables: Vec<(VarId, i64)>,
    current_path: Vec<AccessPathElement>,
    is_collection: bool,
    path_stack: Vec<Vec<AccessPathElement>>,
}

/// The member of an access path element (`IMember` equality: the same definition with the
/// same substitution).
#[derive(Clone, Debug, PartialEq, Eq)]
enum Member {
    Method {
        origin: MethodKey,
        declaring: Option<TypeId>,
        method_args: Option<Vec<TypeId>>,
    },
    Field {
        origin: FieldKey,
        declaring: Option<TypeId>,
    },
    /// The property or event of an accessor (`AccessorOwner`).
    Owner {
        module: ModuleId,
        token: u32,
        declaring: Option<TypeId>,
    },
    /// The owner a fake accessor gets (`GuessFakeMethodAccessor`).
    FakeOwner {
        name: String,
        declaring: Option<TypeId>,
    },
}

#[derive(Clone, Debug, PartialEq, Eq)]
enum MethodKey {
    Def(MethodDefId),
    Other(String),
}

#[derive(Clone, Debug, PartialEq, Eq)]
enum FieldKey {
    Def(ModuleId, u32),
    Fake(String),
}

/// `AccessPathElement`.
#[derive(Clone, Debug)]
struct AccessPathElement {
    member: Member,
    indices: Option<Vec<NodeId>>,
    /// `Member.ReturnType`.
    return_type: TypeId,
    /// `Member.DeclaringType`.
    declaring: Option<TypeId>,
    /// `Member is IProperty { IsIndexer: true }`.
    is_indexer: bool,
    /// `Member is IProperty`.
    is_property: bool,
    /// `Member is IProperty { Setter.IsInitOnly: true }`.
    setter_init_only: bool,
}

impl AccessPathElement {
    /// `AccessPathElement.Equals`: the same member, and the same indices (pure and
    /// structurally equal, `ILInstructionMatchComparer`).
    fn equals(&self, f: &Function, other: &Self) -> bool {
        if self.member != other.member {
            return false;
        }
        match (&self.indices, &other.indices) {
            (None, None) => true,
            (Some(a), Some(b)) => {
                a.len() == b.len()
                    && a.iter().zip(b).all(|(&x, &y)| {
                        x == y
                            || (is_pure(f.flags(x))
                                && is_pure(f.flags(y))
                                && f.structurally_equal(x, y))
                    })
            }
            _ => false,
        }
    }
}

/// `HashSet<AccessPathElement>.Add`: false when an equal element is in `set`.
fn set_add(f: &Function, set: &mut Vec<AccessPathElement>, element: AccessPathElement) -> bool {
    if set.iter().any(|e| e.equals(f, &element)) {
        return false;
    }
    set.push(element);
    true
}

fn field_element(field: &ResolvedField) -> AccessPathElement {
    AccessPathElement {
        member: Member::Field {
            origin: match field.origin {
                FieldOrigin::Def { module, row } => FieldKey::Def(module, row),
                FieldOrigin::Fake => FieldKey::Fake(field.name.clone()),
            },
            declaring: field.declaring,
        },
        indices: None,
        return_type: field.ty,
        declaring: field.declaring,
        is_indexer: false,
        is_property: false,
        setter_init_only: false,
    }
}

fn method_element(method: &ResolvedMethod) -> AccessPathElement {
    AccessPathElement {
        member: Member::Method {
            origin: match method.origin {
                MethodOrigin::Def(m) => MethodKey::Def(m),
                _ => MethodKey::Other(method.name.clone()),
            },
            declaring: method.declaring,
            method_args: method.method_args.clone(),
        },
        indices: None,
        return_type: method.ret,
        declaring: method.declaring,
        is_indexer: false,
        is_property: false,
        setter_init_only: false,
    }
}

/// What an accessor's owner contributes: the element (without indices), and, for a property,
/// whether it may be set in an initializer (`CanBeUsedInInitializer` with a setter kind).
struct OwnerInfo {
    element: AccessPathElement,
    /// `property.CanSet && (property.Accessibility == property.Setter.Accessibility ||
    /// IsAccessorAccessible(property.Setter))`; None for an event.
    settable: Option<bool>,
}

/// The accessor owner of `method`, if it is an accessor.
fn accessor_owner(ctx: &mut Context<'_, '_, '_>, method: &ResolvedMethod) -> Option<OwnerInfo> {
    if method.accessor_kind == 0 {
        return None;
    }
    let is_event_accessor = matches!(method.accessor_kind, 0x08 | 0x10);
    let MethodOrigin::Def(id) = method.origin else {
        // shortcut: a fake accessor's guessed property is taken as settable and not an
        // indexer; upgrade when such a call shows up in an initializer.
        let name = method.name.get(4..).unwrap_or_default().to_owned();
        let return_type = if method.accessor_kind == GETTER {
            method.ret
        } else {
            *method.base_params.last().unwrap_or(&method.ret)
        };
        return Some(OwnerInfo {
            element: AccessPathElement {
                member: Member::FakeOwner {
                    name,
                    declaring: method.declaring,
                },
                indices: None,
                return_type,
                declaring: method.declaring,
                is_indexer: false,
                is_property: !is_event_accessor,
                setter_init_only: false,
            },
            settable: (!is_event_accessor).then_some(true),
        });
    };
    let (token, _) = ctx.ts.accessor_owner(id)?;
    let row = token & 0x00FF_FFFF;
    let member = Member::Owner {
        module: id.module,
        token,
        declaring: method.declaring,
    };
    if token >> 24 != PROPERTY_TABLE {
        // An event (adder or remover).
        return Some(OwnerInfo {
            element: AccessPathElement {
                member,
                indices: None,
                return_type: method.ret,
                declaring: method.declaring,
                is_indexer: false,
                is_property: false,
                setter_init_only: false,
            },
            settable: None,
        });
    }
    let return_type = ctx
        .ts
        .property_type(id.module, row, method.class_args.as_deref())
        .unwrap_or(method.ret);
    let is_indexer = ctx
        .ts
        .property_symbol_kind(id.module, row)
        .is_ok_and(|k| k == SymbolKind::Indexer);
    let setter = ctx.ts.comp.module(id.module).md.property_accessors(row).1;
    let (settable, setter_init_only) = if setter == 0 {
        (false, false)
    } else {
        let setter = MethodDefId {
            module: id.module,
            row: setter,
        };
        let property_accessibility = ctx
            .ts
            .member_accessibility(MemberDef::Property(id.module, row))
            .unwrap_or(ACC_NONE);
        let setter_accessibility = ctx.ts.method_accessibility_of(setter).unwrap_or(ACC_NONE);
        let init_only = ctx.ts.method_flags(setter, Some(token)).init_only;
        (
            property_accessibility == setter_accessibility
                || is_accessor_accessible(setter_accessibility, setter.module),
            init_only,
        )
    };
    Some(OwnerInfo {
        element: AccessPathElement {
            member,
            indices: None,
            return_type,
            declaring: method.declaring,
            is_indexer,
            is_property: true,
            setter_init_only,
        },
        settable: Some(settable),
    })
}

/// `IsAccessorAccessible` with the IL pipeline's resolver, which has no current type
/// definition: only public members, and internal ones of the main module.
// shortcut: InternalsVisibleTo is not consulted; upgrade with the semantics step (item 67).
fn is_accessor_accessible(accessibility: u8, module: ModuleId) -> bool {
    let internal = module == ModuleId(0);
    match accessibility {
        ACC_PUBLIC => true,
        ACC_INTERNAL | ACC_PROTECTED_OR_INTERNAL => internal,
        ACC_PRIVATE | ACC_PROTECTED | ACC_PROTECTED_AND_INTERNAL => false,
        _ => false,
    }
}

/// `AccessPathKind`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum AccessPathKind {
    Invalid,
    Setter,
    Adder,
}

/// The result of `GetAccessPath`.
struct AccessPath {
    kind: AccessPathKind,
    path: Vec<AccessPathElement>,
    values: Option<Vec<NodeId>>,
    target: Option<VarId>,
    used_index_variables: Vec<VarId>,
}

/// Whether `t` mentions type parameter `index` of a method.
fn mentions_method_type_parameter(ctx: &mut Context<'_, '_, '_>, t: TypeId, index: u32) -> bool {
    match ctx.ts.data(t).clone() {
        TypeData::TypeParam { owner, index: i } => owner.is_method() && i == index,
        TypeData::Inst { generic, args } => {
            mentions_method_type_parameter(ctx, generic, index)
                || args
                    .iter()
                    .any(|&a| mentions_method_type_parameter(ctx, a, index))
        }
        TypeData::Array { elem, .. }
        | TypeData::Pointer(elem)
        | TypeData::ByRef(elem)
        | TypeData::Pinned(elem) => mentions_method_type_parameter(ctx, elem, index),
        TypeData::Modified { inner, .. } | TypeData::Annotated { inner, .. } => {
            mentions_method_type_parameter(ctx, inner, index)
        }
        TypeData::Tuple { underlying, .. } => {
            mentions_method_type_parameter(ctx, underlying, index)
        }
        TypeData::FnPtr(fp) => {
            mentions_method_type_parameter(ctx, fp.ret, index)
                || fp
                    .params
                    .iter()
                    .any(|&p| mentions_method_type_parameter(ctx, p, index))
        }
        _ => false,
    }
}

/// `IsMethodApplicable`.
fn is_method_applicable(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    operand: &crate::il::ast::MethodOperand,
    arguments: &[NodeId],
    root_type: TypeId,
) -> bool {
    let method = &operand.method;
    let is_extension = match method.origin {
        MethodOrigin::Def(d) => {
            let owner = ctx.ts.accessor_owner(d).map(|(o, _)| o);
            ctx.ts.method_flags(d, owner).extension
        }
        _ => false,
    };
    if method.is_static && !is_extension {
        return false;
    }
    if let MethodOrigin::Def(d) = method.origin
        && method.accessor_kind != 0
        && ctx
            .ts
            .accessor_owner(d)
            .is_some_and(|(o, _)| o >> 24 == PROPERTY_TABLE)
    {
        return true;
    }
    if matches!(method.accessor_kind, 0x01 | 0x02) && !matches!(method.origin, MethodOrigin::Def(_))
    {
        // A fake getter's or setter's guessed owner is a property.
        return true;
    }
    if method.name != "Add" || arguments.is_empty() {
        return false;
    }
    if is_extension {
        if !ctx.settings.extension_methods_in_collection_initializers {
            return false;
        }
        // Without a resolver (as in the IL pipeline the tests compare) no extension method is found.
        let Some(resolver) = ctx.resolver else {
            return false;
        };
        if !resolver.can_transform_to_extension_method_call(
            ctx.ts,
            &crate::il::transforms::ExtensionMethodQuery::Method { method: operand },
        ) {
            return false;
        }
    }
    let target_type = get_return_type_from_instruction(f, arguments[0]).unwrap_or(root_type);
    let Ok(bases) = ctx.ts.all_base_types(target_type) else {
        return false;
    };
    if !bases.iter().any(|&b| {
        matches!(
            known(ctx, b),
            Some(KnownTypeCode::IEnumerable | KnownTypeCode::IEnumerableOfT)
        )
    }) {
        return false;
    }
    // `CanInferTypeArgumentsFromParameters`.
    // shortcut: the managed code runs C# type inference with the parameter types as argument
    // types; inferring a type parameter from itself succeeds exactly when some parameter
    // mentions it, which is what the port checks. Port the inference with the semantics step
    // (item 67).
    if method.type_args.is_empty() {
        return true;
    }
    let MethodOrigin::Def(d) = method.origin else {
        return false;
    };
    let Ok(definition) = ctx.ts.method_definition(d) else {
        return false;
    };
    let Ok(params) = definition.params() else {
        return false;
    };
    (0..definition.type_args.len() as u32).all(|i| {
        params
            .iter()
            .any(|&p| mentions_method_type_parameter(ctx, p, i))
    })
}

/// `GetReturnTypeFromInstruction`.
fn get_return_type_from_instruction(f: &Function, inst: NodeId) -> Option<TypeId> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) => Some(c.method.method.ret),
        Inst::LdObj { target, .. } | Inst::StObj { target, .. } => match f.inst(*target) {
            Inst::LdFlda { field, .. } => Some(field.field.ty),
            _ => None,
        },
        _ => None,
    }
}

/// `AccessPathElement.GetAccessPath`.
fn get_access_path(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    instruction: NodeId,
    root_type: TypeId,
) -> AccessPath {
    let mut path: Vec<AccessPathElement> = Vec::new();
    let mut target = None;
    let mut kind = AccessPathKind::Invalid;
    let mut values: Option<Vec<NodeId>> = None;
    let mut used_index_variables = Vec::new();
    let mut inst = Some(instruction);
    while let Some(current) = inst {
        match f.inst(current) {
            Inst::Call(call) | Inst::CallVirt(call) => {
                let operand = call.method.clone();
                let method = operand.method.clone();
                let args = call.args.clone();
                if !is_method_applicable(f, ctx, &operand, &args, root_type) {
                    kind = AccessPathKind::Invalid;
                    break;
                }
                let mut next = args[0];
                if let Inst::LdObjIfRef { target: t, .. } = *f.inst(next) {
                    next = t;
                }
                inst = Some(next);
                if let Some(owner) = accessor_owner(ctx, &method) {
                    if owner.settable == Some(false) && kind == AccessPathKind::Setter {
                        kind = AccessPathKind::Invalid;
                        break;
                    }
                    let is_getter = method.accessor_kind == GETTER;
                    let take = args.len() as isize - if is_getter { 1 } else { 2 };
                    let indices: Vec<NodeId> = args
                        .iter()
                        .skip(1)
                        .take(take.max(0) as usize)
                        .copied()
                        .collect();
                    if !indices.is_empty() && !ctx.settings.dictionary_initializers {
                        kind = AccessPathKind::Invalid;
                        break;
                    }
                    // Every index variable counts as used.
                    for &index in &indices {
                        if let Some(v) = f.variable_of(index) {
                            used_index_variables.push(v);
                        }
                    }
                    let mut element = owner.element;
                    element.indices = Some(indices);
                    path.insert(0, element);
                } else {
                    path.insert(0, method_element(&method));
                }
                if values.is_none() {
                    if method.accessor_kind != 0 {
                        kind = AccessPathKind::Setter;
                        values = Some(vec![*args.last().expect("an argument")]);
                    } else {
                        kind = AccessPathKind::Adder;
                        let rest: Vec<NodeId> = args.iter().skip(1).copied().collect();
                        if rest.is_empty() {
                            kind = AccessPathKind::Invalid;
                            break;
                        }
                        values = Some(rest);
                    }
                }
            }
            Inst::LdObj { target: t, .. } => {
                if let Inst::LdFlda {
                    target: ft, field, ..
                } = f.inst(*t)
                    && (kind != AccessPathKind::Setter || !ctx.ts.field_is_read_only(&field.field))
                {
                    let (ft, field) = (*ft, field.field.clone());
                    path.insert(0, field_element(&field));
                    inst = Some(ft);
                } else {
                    kind = AccessPathKind::Invalid;
                    break;
                }
            }
            Inst::LdObjIfRef { target: t, .. } => {
                if let Inst::LdFlda {
                    target: ft, field, ..
                } = f.inst(*t)
                    && (kind != AccessPathKind::Setter || !ctx.ts.field_is_read_only(&field.field))
                {
                    let (ft, field) = (*ft, field.field.clone());
                    path.insert(0, field_element(&field));
                    inst = Some(ft);
                } else if let Inst::LdLoca { var } = *f.inst(*t) {
                    target = Some(var);
                    inst = None;
                } else {
                    kind = AccessPathKind::Invalid;
                    break;
                }
            }
            Inst::StObj {
                target: t, value, ..
            } => {
                if let Inst::LdFlda {
                    target: ft, field, ..
                } = f.inst(*t)
                {
                    let (ft, field, value) = (*ft, field.field.clone(), *value);
                    path.insert(0, field_element(&field));
                    inst = Some(ft);
                    if values.is_none() {
                        values = Some(vec![value]);
                        kind = AccessPathKind::Setter;
                    }
                } else {
                    kind = AccessPathKind::Invalid;
                    break;
                }
            }
            &Inst::LdLoc { var } | &Inst::LdLoca { var } => {
                target = Some(var);
                inst = None;
            }
            Inst::LdFlda {
                target: ft, field, ..
            } => {
                let (ft, field) = (*ft, field.field.clone());
                path.insert(0, field_element(&field));
                inst = Some(ft);
            }
            _ => {
                kind = AccessPathKind::Invalid;
                break;
            }
        }
    }
    if kind != AccessPathKind::Invalid
        && let (Some(values), Some(t)) = (&values, target)
        && values.iter().any(|&v| loads_variable(f, v, t))
    {
        kind = AccessPathKind::Invalid;
    }
    AccessPath {
        kind,
        path,
        values,
        target,
        used_index_variables,
    }
}

/// Whether `inst` or a descendant is an `ldloc` or `ldloca` of `v`.
fn loads_variable(f: &Function, inst: NodeId, v: VarId) -> bool {
    f.descendants_post(inst)
        .into_iter()
        .any(|n| matches!(*f.inst(n), Inst::LdLoc { var } | Inst::LdLoca { var } if var == v))
}

/// `NormalizeTypeVisitor.IgnoreNullabilityAndTuples`: tuples become their underlying type,
/// modifiers and nullability go.
fn ignore_nullability_and_tuples(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    let data = ctx.ts.data(t).clone();
    let normalized = match data {
        TypeData::Modified { inner, .. } | TypeData::Annotated { inner, .. } => {
            return ignore_nullability_and_tuples(ctx, inner);
        }
        TypeData::Tuple { underlying, .. } => {
            return ignore_nullability_and_tuples(ctx, underlying);
        }
        TypeData::Inst { generic, args } => TypeData::Inst {
            generic: ignore_nullability_and_tuples(ctx, generic),
            args: args
                .iter()
                .map(|&a| ignore_nullability_and_tuples(ctx, a))
                .collect(),
        },
        TypeData::Array { elem, rank, .. } => TypeData::Array {
            elem: ignore_nullability_and_tuples(ctx, elem),
            rank,
            nullability: Nullability::Oblivious,
        },
        TypeData::Pointer(e) => TypeData::Pointer(ignore_nullability_and_tuples(ctx, e)),
        TypeData::ByRef(e) => TypeData::ByRef(ignore_nullability_and_tuples(ctx, e)),
        _ => return t,
    };
    ctx.ts.intern(normalized)
}

/// `IsValidObjectInitializerTarget`: an indexer is only valid inside a member of its own
/// declaring type.
fn is_valid_object_initializer_target(
    ctx: &mut Context<'_, '_, '_>,
    path: &[AccessPathElement],
) -> bool {
    let Some(element) = path.last() else {
        return true;
    };
    if !element.is_property || !element.is_indexer {
        return true;
    }
    if path.len() >= 2 {
        let previous = &path[path.len() - 2];
        let Some(declaring) = element.declaring else {
            return false;
        };
        let a = ignore_nullability_and_tuples(ctx, previous.return_type);
        let b = ignore_nullability_and_tuples(ctx, declaring);
        return a == b;
    }
    false
}

impl Scan {
    /// `IsPartOfInitializer`.
    #[allow(clippy::too_many_arguments)]
    fn is_part_of_initializer(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        pos: usize,
        target: VarId,
        root_type: TypeId,
        block_kind: &mut BlockKind,
        initializer_contains_init_only_items: &mut bool,
    ) -> bool {
        let inst = f.list(block)[pos];
        // A store to a single-assigned local that does not use the initializer variable may
        // be an index variable of a dictionary initializer.
        if let Some((v, value)) = f.match_stloc(inst)
            && f.var(v).kind == VariableKind::Local
            && f.var(v).is_single_definition()
        {
            if !ctx.settings.dictionary_initializers {
                return false;
            }
            if loads_variable(f, value, target) {
                return false;
            }
            self.possible_index_variables
                .push((v, i64::from(f.node(inst).child_index)));
            return true;
        }
        let AccessPath {
            kind,
            path: mut new_path,
            values,
            target: target_variable,
            used_index_variables,
        } = get_access_path(f, ctx, inst, root_type);
        if kind == AccessPathKind::Invalid || Some(target) != target_variable {
            return false;
        }
        // The last element is the Add call or the setter.
        let last_element = new_path.pop().expect("a valid path has an element");
        let min_len = self.current_path.len().min(new_path.len());
        let mut first_difference_index = 0;
        while first_difference_index < min_len
            && new_path[first_difference_index]
                .equals(f, &self.current_path[first_difference_index])
        {
            first_difference_index += 1;
        }
        while self.current_path.len() > first_difference_index {
            self.is_collection = false;
            self.current_path.pop();
            self.path_stack.pop();
        }
        while self.current_path.len() < new_path.len() {
            let new_element = new_path[self.current_path.len()].clone();
            self.current_path.push(new_element.clone());
            if self.is_collection
                || !set_add(
                    f,
                    self.path_stack.last_mut().expect("a path set"),
                    new_element,
                )
            {
                return false;
            }
            self.path_stack.push(Vec::new());
        }
        match kind {
            AccessPathKind::Adder => {
                self.is_collection = true;
                if !self.path_stack.last().expect("a path set").is_empty() {
                    return false;
                }
                self.mark_used_indices(&used_index_variables);
                true
            }
            AccessPathKind::Setter => {
                let init_only = last_element.is_property && last_element.setter_init_only;
                if self.is_collection
                    || !set_add(
                        f,
                        self.path_stack.last_mut().expect("a path set"),
                        last_element,
                    )
                {
                    return false;
                }
                if values.as_ref().is_none_or(|v| v.len() != 1)
                    || !is_valid_object_initializer_target(ctx, &self.current_path)
                {
                    return false;
                }
                if *block_kind != BlockKind::ObjectInitializer
                    && *block_kind != BlockKind::WithInitializer
                {
                    *block_kind = BlockKind::ObjectInitializer;
                }
                *initializer_contains_init_only_items |= init_only;
                self.mark_used_indices(&used_index_variables);
                true
            }
            AccessPathKind::Invalid => false,
        }
    }

    /// `MarkUsedIndices`.
    fn mark_used_indices(&mut self, used: &[VarId]) {
        for &index in used {
            if let Some(entry) = self
                .possible_index_variables
                .iter_mut()
                .find(|(v, _)| *v == index)
            {
                entry.1 = -1;
            }
        }
    }
}

/// `IsMethodCallOnVariable`.
fn is_method_call_on_variable(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    v: VarId,
) -> bool {
    if f.match_ld_loc_ref(ctx.ts, inst) == Some(v) {
        return true;
    }
    if let Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) = f.inst(inst)
        && !c.args.is_empty()
        && !c.method.method.is_static
    {
        return is_method_call_on_variable(f, ctx, c.args[0], v);
    }
    if let Some((target, _)) = f.match_ldfld(ctx.ts, inst) {
        return is_method_call_on_variable(f, ctx, target, v);
    }
    if let Some((target, _, _)) = f.match_stfld(inst) {
        return is_method_call_on_variable(f, ctx, target, v);
    }
    if let Some((target, _)) = f.match_ldflda(inst) {
        return is_method_call_on_variable(f, ctx, target, v);
    }
    false
}

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.object_or_collection_initializers {
        return;
    }
    let inst = r.function.list(block)[pos];
    // stloc v(newobj ...)
    let Some((v, mut init_inst)) = r.function.match_stloc(inst) else {
        return;
    };
    let v_kind = r.function.var(v).kind;
    if v_kind != VariableKind::Local && v_kind != VariableKind::StackSlot {
        return;
    }
    let current_method = r.method;
    let mut block_kind = BlockKind::CollectionInitializer;
    let insertion_pos = r.function.node(init_inst).child_index as usize;
    let siblings = r.function.node(init_inst).parent.expect("a parent");
    // A castclass may wrap the init instruction: inherited record types on runtimes without
    // covariant return types.
    let mut target_type = None;
    if let Inst::CastClass { arg, ty } = *r.function.inst(init_inst) {
        init_inst = arg;
        target_type = Some(ty.ty);
    }
    let f = &mut r.function;
    let mut inst_type = match f.inst(init_inst) {
        Inst::NewObj(c) => {
            let declaring = c.method.method.declaring;
            let il_stack_was_empty = c.il_stack_was_empty;
            let current_is_constructor = ctx
                .ts
                .method_definition(current_method)
                .is_ok_and(|m| m.name == ".ctor" || m.name == ".cctor");
            let current_is_compiler_generated =
                ctx.ts.method_definition(current_method).is_ok_and(|m| {
                    ctx.ts
                        .method_is_compiler_generated_or_in_compiler_generated_class(&m)
                });
            let declaring_def = declaring.and_then(|d| type_def(ctx, d));
            // At statement level (nothing else on the IL stack), local variables (not stack
            // slots) stay, except in constructors (where an initializer may matter for the
            // base constructor call) and compiler-generated methods (query expressions).
            if il_stack_was_empty
                && v_kind == VariableKind::Local
                && !type_contains_init_only_properties(ctx, declaring_def)
                && !current_is_constructor
                && !current_is_compiler_generated
            {
                return;
            }
            // DelegateConstruction cannot handle an initializer on a delegate construction.
            if super::delegate_construction::match_delegate_construction(f, ctx, init_inst) {
                return;
            }
            let decompiled = ctx.ts.method_declaring(current_method).ok().flatten();
            if let Some(display) = declaring_def
                && ctx.ts.is_potential_closure_type(decompiled, display)
            {
                return;
            }
            let Some(declaring) = declaring else {
                return;
            };
            // `new { A = 5 } { 3, 4, 5 }` is invalid syntax.
            if contains_anonymous_type(ctx.ts, declaring) {
                return;
            }
            // Tuples cannot have initializers.
            if match_tuple_construction(f, ctx, init_inst) {
                return;
            }
            declaring
        }
        Inst::DefaultValue { ty, .. } => ty.ty,
        Inst::Call(c)
            if c.method.method.name == "CreateInstance"
                && c.method.method.type_args.len() == 1
                && c.method
                    .method
                    .declaring
                    .is_some_and(|d| ctx.ts.full_name(d) == "System.Activator") =>
        {
            if !ctx.settings.use_object_creation_of_generic_type_parameter {
                return;
            }
            block_kind = BlockKind::ObjectInitializer;
            c.method.method.type_args[0]
        }
        Inst::Block {
            kind: BlockKind::WithInitializer,
            instructions,
            ..
        } if instructions.len() == 1 && f.match_stloc(instructions[0]).is_some() => {
            // An existing with-initializer block from HandleEmptyWith grows.
            let existing = f.match_stloc(instructions[0]).expect("checked").1;
            block_kind = BlockKind::WithInitializer;
            init_inst = existing;
            f.infer_type(ctx.ts, existing)
        }
        _ => {
            let v_type = f.var(v).ty;
            let def = type_def(ctx, v_type);
            // `IsReferenceType == false`: records are classes or structs.
            let is_value_type_record = def.is_some_and(|d| {
                ctx.ts
                    .comp
                    .type_def(d)
                    .is_ok_and(|i| i.kind == TypeKind::Struct)
                    && is_record(ctx, d)
            });
            if ctx.settings.with_expressions && is_value_type_record {
                block_kind = BlockKind::WithInitializer;
                v_type
            } else {
                return;
            }
        }
    };
    if let Some(t) = target_type {
        inst_type = t;
    }
    // Copy-propagate a stack slot holding an `ldloca` of the variable.
    if let Some(&next) = f.list(block).get(pos + 1)
        && let Some((slot, value)) = f.match_stloc(next)
        && f.var(slot).kind == VariableKind::StackSlot
        && f.var(slot).is_single_definition()
        && matches!(*f.inst(value), Inst::LdLoca { var } if var == v)
    {
        super::copy_propagation::propagate(&mut r.function, &mut r.variables, ctx, next);
    }
    let f = &mut r.function;
    let mut initializer_items_count = 0;
    let mut initializer_contains_init_only_items = false;
    let mut scan = Scan {
        path_stack: vec![Vec::new()],
        ..Scan::default()
    };
    // Each following statement must be part of the initializer: a setter makes an object
    // initializer, an Add with arguments a collection or dictionary initializer.
    while pos + initializer_items_count + 1 < f.list(block).len()
        && scan.is_part_of_initializer(
            f,
            ctx,
            block,
            pos + initializer_items_count + 1,
            v,
            inst_type,
            &mut block_kind,
            &mut initializer_contains_init_only_items,
        )
    {
        initializer_items_count += 1;
    }
    // No initializer when the variable is used incompatibly right after it.
    // shortcut: when the initializer reaches the end of the block, the managed indexer
    // throws; the port treats the missing statement as no use.
    if !initializer_contains_init_only_items
        && let Some(&after) = f.list(block).get(pos + initializer_items_count + 1)
        && is_method_call_on_variable(f, ctx, after, v)
    {
        return;
    }
    // Index variables an initializer used are marked -1; the first unused one ends the
    // initializer at its store.
    if let Some(index) = scan
        .possible_index_variables
        .iter()
        .map(|&(_, i)| i)
        .filter(|&i| i > -1)
        .min()
    {
        let count = index - pos as i64 - 1;
        if count <= 0 {
            return;
        }
        initializer_items_count = count as usize;
    }
    if initializer_items_count == 0 {
        return;
    }
    let final_slot = f.register_variable(ctx.ts, VariableKind::InitializerTarget, inst_type);
    let final_inst = f.add(Inst::LdLoc { var: final_slot });
    let mut instructions = vec![stloc(f, final_slot, init_inst)];
    for i in 1..=initializer_items_count {
        let item = f.list(block)[i + pos];
        let retarget = match f.inst(item) {
            Inst::Call(c) | Inst::CallVirt(c) => Some(c.args[0]),
            Inst::NewObj(_) => continue,
            Inst::StObj { target, .. } => Some(*target),
            Inst::StLoc { .. } => None,
            _ => continue,
        };
        if let Some(root) = retarget {
            for load in f.descendants_post(root) {
                if matches!(*f.inst(load), Inst::LdLoc { var } | Inst::LdLoca { var } if var == v) {
                    f.set_variable(load, final_slot);
                }
            }
        }
        instructions.push(item);
    }
    let initializer_block = new_block(f, block_kind, instructions, final_inst);
    for _ in 0..initializer_items_count {
        f.remove_at(block, pos + 1);
    }
    f.set_child_at(siblings, insertion_pos, initializer_block);
    inline_one_if_possible(f, block, pos, options::AGGRESSIVE, ctx);
}
