//! `IL/Transforms/SplitVariables.cs`: live range splitting. Stores and loads of a local that
//! no reaching definition connects get variables of their own.

use std::collections::HashMap;

use crate::flow_analysis::data_flow::{Analysis, Flow};
use crate::flow_analysis::reaching_definitions::{ReachingDefinitions, State};
use crate::il::ast::{Function, Inst, NodeId, VarId, Variable, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::inheritance::MemberDef;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::SymbolKind;
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};
use crate::util::union_find::UnionFind;

use super::Context;

/// The connected loads, stores and address loads of each variable
/// (`LoadInstructions`, `StoreInstructions`, `AddressInstructions`).
#[derive(Default)]
struct Uses {
    loads: HashMap<VarId, Vec<NodeId>>,
    stores: HashMap<VarId, Vec<NodeId>>,
    addresses: HashMap<VarId, Vec<NodeId>>,
}

impl Uses {
    fn collect(f: &Function) -> Uses {
        let mut uses = Uses::default();
        for n in f.descendants_post(f.body.expect("function body")) {
            let (map, var) = match *f.inst(n) {
                Inst::LdLoc { var } => (&mut uses.loads, var),
                Inst::LdLoca { var } => (&mut uses.addresses, var),
                Inst::StLoc { var, .. } | Inst::TryCatchHandler { variable: var, .. } => {
                    (&mut uses.stores, var)
                }
                _ => continue,
            };
            map.entry(var).or_default().push(n);
        }
        uses
    }

    fn of(map: &HashMap<VarId, Vec<NodeId>>, var: VarId) -> &[NodeId] {
        map.get(&var).map_or(&[], Vec::as_slice)
    }
}

/// The variable of an instruction with a variable operand (`IInstructionWithVariableOperand`).
fn variable_of(f: &Function, id: NodeId) -> Option<VarId> {
    match *f.inst(id) {
        Inst::LdLoc { var } | Inst::LdLoca { var } | Inst::StLoc { var, .. } => Some(var),
        Inst::TryCatchHandler { variable, .. } => Some(variable),
        _ => None,
    }
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &r.function;
    let root = f.body.expect("function body");
    let uses = Uses::collect(f);
    let is_async = r.async_return_type.is_some();
    let analyzed: Vec<bool> = r
        .variables
        .iter()
        .map(|&v| is_candidate_variable(f, ctx.ts, &uses, v, is_async))
        .collect();
    let (rd, mut flow) = ReachingDefinitions::new(f, &r.variables, analyzed);
    let mut groups = GroupStores {
        rd,
        uses,
        union_find: UnionFind::default(),
        uninit_variable_usage: HashMap::new(),
        new_variables: HashMap::new(),
    };
    flow.visit(&mut groups, f, root);
    // Replace the analyzed variables with their split versions.
    let instructions: Vec<NodeId> = f
        .descendants_post(root)
        .into_iter()
        .filter(|&n| variable_of(f, n).is_some())
        .collect();
    for inst in instructions {
        let var = variable_of(&r.function, inst).expect("variable operand");
        if groups.rd.is_analyzed_variable(var) {
            let new = groups.new_variable(r, inst, var);
            r.function.set_variable(inst, new);
        }
    }
    remove_dead_variables(r);
}

/// `ILVariableCollection.RemoveDead`; `RemoveAt` moves the last variable into the gap.
pub fn remove_dead_variables(r: &mut ReadFunction) {
    let mut i = 0;
    while i < r.variables.len() {
        let v = r.function.var(r.variables[i]);
        // Display-class locals stay for the PDB; parameters exist even when unused.
        let remove = v.is_dead()
            && v.kind != VariableKind::DisplayClassLocal
            && v.kind != VariableKind::Parameter;
        if remove {
            r.variables.swap_remove(i);
        } else {
            i += 1;
        }
    }
}

fn is_candidate_variable(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    uses: &Uses,
    v: VarId,
    is_async: bool,
) -> bool {
    match f.var(v).kind {
        VariableKind::Local => {}
        // Stack slots are split by construction, except the locals-turned-stack-slots of
        // async functions and the slots RemoveInfeasiblePathTransform marked.
        VariableKind::StackSlot if is_async || f.var(v).remove_if_redundant => {}
        // Parameters are not split; pinned locals must not be (it would extend the pin).
        _ => return false,
    }
    Uses::of(&uses.addresses, v)
        .iter()
        .all(|&ldloca| determine_address_use(f, ts, uses, ldloca, v) != AddressUse::Unknown)
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum AddressUse {
    Unknown,
    /// The address is used right away, with no `stloc` of the variable in between.
    Immediate,
    /// Stored into a single-assignment ref local whose uses are all immediate.
    WithSupportedRefLocals,
}

fn determine_address_use(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    uses: &Uses,
    address: NodeId,
    target: VarId,
) -> AddressUse {
    let Some(parent) = f.node(address).parent else {
        return AddressUse::Unknown;
    };
    match f.inst(parent) {
        Inst::LdObj { .. } => AddressUse::Immediate,
        Inst::StObj { target: t, .. } if *t == address => AddressUse::Immediate,
        Inst::LdObjIfRef { .. } | Inst::LdFlda { .. } => {
            determine_address_use(f, ts, uses, parent, target)
        }
        Inst::Call(call) | Inst::CallVirt(call) => handle_call(
            f,
            ts,
            uses,
            address,
            target,
            parent,
            &call.method.method,
            false,
        ),
        Inst::NewObj(call) => handle_call(
            f,
            ts,
            uses,
            address,
            target,
            parent,
            &call.method.method,
            true,
        ),
        &Inst::StLoc { var, value, .. } if f.var(var).is_single_definition() => {
            if !matches!(
                f.var(var).kind,
                VariableKind::StackSlot | VariableKind::Local
            ) {
                return AddressUse::Unknown;
            }
            // Only `ldflda*(ldloca)` ref locals are supported (`GetAddressLoadForRefLocalUse`).
            if !matches!(f.inst(strip_ldflda(f, value)), Inst::LdLoca { .. }) {
                return AddressUse::Unknown;
            }
            for &load in Uses::of(&uses.loads, var) {
                if determine_address_use(f, ts, uses, load, target) != AddressUse::Immediate {
                    return AddressUse::Unknown;
                }
            }
            AddressUse::WithSupportedRefLocals
        }
        _ => AddressUse::Unknown,
    }
}

fn strip_ldflda(f: &Function, mut value: NodeId) -> NodeId {
    while let Inst::LdFlda { target, .. } = *f.inst(value) {
        value = target;
    }
    value
}

/// `HandleCall`: a callee may use the address only locally, unless it can return it.
#[allow(clippy::too_many_arguments)]
fn handle_call(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    uses: &Uses,
    address: NodeId,
    target: VarId,
    call: NodeId,
    method: &ResolvedMethod,
    is_newobj: bool,
) -> AddressUse {
    let return_type = if is_newobj {
        method.declaring
    } else {
        Some(method.ret)
    };
    if return_type.is_some_and(|t| ts.is_by_ref_like(t))
        && !is_span_of_t_indexer_accessor(ts, method)
        && determine_address_use(f, ts, uses, call, target) != AddressUse::Immediate
    {
        return AddressUse::Unknown;
    }
    // `out Span<int>` and similar.
    for p in method.params().unwrap_or_default() {
        if let TypeData::ByRef(elem) = *ts.data(skip_modifiers(ts, p))
            && ts.is_by_ref_like(elem)
        {
            return AddressUse::Unknown;
        }
    }
    // No `stloc target` between the `ldloca` and the call consuming the address.
    let args = f.list(call);
    let first = f.node(address).child_index as usize + 1;
    for &arg in args.iter().skip(first) {
        for inst in f.descendants_post(arg) {
            if matches!(*f.inst(inst), Inst::StLoc { var, .. } if var == target) {
                return AddressUse::Unknown;
            }
        }
    }
    AddressUse::Immediate
}

fn skip_modifiers(ts: &TypeSystem<'_, '_>, mut t: TypeId) -> TypeId {
    while let TypeData::Modified { inner, .. } = *ts.data(t) {
        t = inner;
    }
    t
}

/// `IsSpanOfTIndexerAccessor`: `Span<T>.this[int]` and `ReadOnlySpan<T>.this[int]` return a
/// reference into the span, never into their arguments.
fn is_span_of_t_indexer_accessor(ts: &mut TypeSystem<'_, '_>, method: &ResolvedMethod) -> bool {
    let Some(declaring) = method.declaring else {
        return false;
    };
    let known = ts
        .definition(declaring)
        .and_then(|d| ts.comp.type_def(d).ok())
        .map(|i| i.known);
    if !matches!(
        known,
        Some(KnownTypeCode::SpanOfT | KnownTypeCode::ReadOnlySpanOfT)
    ) {
        return false;
    }
    let MethodOrigin::Def(def) = method.origin else {
        return false;
    };
    // The owner comes as a metadata token; only a property (table 0x17) can be an indexer.
    let Some((owner, _)) = ts.accessor_owner(def) else {
        return false;
    };
    if owner >> 24 != 0x17 {
        return false;
    }
    let property = owner & 0x00FF_FFFF;
    let module = def.module;
    if ts.property_symbol_kind(module, property).ok() != Some(SymbolKind::Indexer)
        || ts
            .member_name(MemberDef::Property(module, property))
            .ok()
            .as_deref()
            != Some("Item")
    {
        return false;
    }
    let Ok((ret, params)) = ts.property_signature(module, property) else {
        return false;
    };
    let [param] = params.as_slice() else {
        return false;
    };
    let int32 = ts.known_type(KnownTypeCode::Int32);
    if ts.definition(param.ty) != ts.definition(int32) {
        return false;
    }
    let ret = ts.substitute(
        ret,
        method.class_args.as_deref(),
        method.method_args.as_deref(),
    );
    let TypeData::ByRef(elem) = *ts.data(ret) else {
        return false;
    };
    ts.type_arguments(declaring).first() == Some(&elem)
}

/// `GetAddressLoadForRefLocalUse`: for a use of a supported ref local, the `ldloca` that
/// initialized it.
fn address_load_for_ref_local_use(f: &Function, uses: &Uses, ldloc: NodeId) -> Option<NodeId> {
    let var = f.match_ldloc(ldloc)?;
    if !f.var(var).is_single_definition() {
        return None;
    }
    let [store] = Uses::of(&uses.stores, var) else {
        return None;
    };
    let (_, value) = f.match_stloc(*store)?;
    let value = strip_ldflda(f, value);
    matches!(f.inst(value), Inst::LdLoca { .. }).then_some(value)
}

/// `GroupStores`: merges each load with the stores that reach it; a group of stores and
/// loads that must stay together becomes one variable.
struct GroupStores {
    rd: ReachingDefinitions,
    uses: Uses,
    union_find: UnionFind<NodeId>,
    /// Per variable, one load that may observe its uninitialized value.
    uninit_variable_usage: HashMap<VarId, NodeId>,
    new_variables: HashMap<NodeId, VarId>,
}

impl GroupStores {
    fn handle_variable_load(&mut self, flow: &Flow<State>, f: &Function, inst: NodeId) {
        let var = variable_of(f, inst).expect("load");
        if !self.rd.is_analyzed_variable(var) {
            return;
        }
        if self.rd.is_potentially_uninitialized(&flow.state, var) {
            match self.uninit_variable_usage.get(&var) {
                Some(&first) => self.union_find.merge(inst, first),
                None => {
                    self.uninit_variable_usage.insert(var, inst);
                }
            }
        }
        for store in self.rd.stores(&flow.state, var) {
            self.union_find.merge(inst, store);
        }
    }

    /// `GetNewVariable`.
    fn new_variable(&mut self, r: &mut ReadFunction, inst: NodeId, old: VarId) -> VarId {
        let representative = self.union_find.find(inst);
        let v = match self.new_variables.get(&representative) {
            Some(&v) => v,
            None => {
                let o = r.function.var(old);
                let v = r.function.add_var(Variable {
                    kind: o.kind,
                    ty: o.ty,
                    stack_type: o.stack_type,
                    index: o.index,
                    name: o.name.clone(),
                    has_generated_name: o.has_generated_name,
                    initial_value_is_initialized: o.initial_value_is_initialized,
                    // Set below when an uninitialized load is part of the group.
                    uses_initial_value: false,
                    is_ref_read_only: false,
                    remove_if_redundant: o.remove_if_redundant,
                    load_instructions: Vec::new(),
                    store_instructions: Vec::new(),
                    address_instructions: Vec::new(),
                });
                self.new_variables.insert(representative, v);
                r.variables.push(v);
                v
            }
        };
        if r.function.var(old).uses_initial_value
            && self.uninit_variable_usage.get(&old) == Some(&inst)
        {
            r.function.var_mut(v).uses_initial_value = true;
        }
        v
    }
}

impl Analysis for GroupStores {
    type S = State;

    fn handle_store(&mut self, flow: &mut Flow<State>, _f: &Function, inst: NodeId, var: VarId) {
        self.rd.handle_store(flow, inst, var);
    }

    fn handle_load(&mut self, flow: &mut Flow<State>, f: &Function, inst: NodeId) {
        self.handle_variable_load(flow, f, inst);
        if matches!(f.inst(inst), Inst::LdLoc { .. })
            && let Some(address) = address_load_for_ref_local_use(f, &self.uses, inst)
        {
            // A supported ref local acts as if the `ldloca` were copy-propagated to its use.
            self.handle_variable_load(flow, f, address);
        }
    }
}
