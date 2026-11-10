//! `IL/Transforms/DeconstructionTransform.cs`: a run of statements that is a lowered
//! deconstruction assignment, rooted in a `Deconstruct` call or in tuple element reads (also
//! nested), becomes one `deconstruct` instruction:
//!
//! ```text
//! stloc tuple(call MakeIntIntTuple(ldloc this))
//! ----
//! stloc myInt(call op_Implicit(ldfld Item2(ldloca tuple)))
//! stloc a(ldfld Item1(ldloca tuple))
//! stloc b(ldloc myInt)
//! ==>
//! deconstruct {
//!     init:
//!     pattern:
//!         match.deconstruct(temp = ldloca tuple) {
//!             match(result0 = deconstruct.result 0(temp)),
//!             match(result1 = deconstruct.result 1(temp))
//!         }
//!     conversions: { stloc conv2(call op_Implicit(ldloc result1)) }
//!     assignments: { stloc a(ldloc result0); stloc b(ldloc conv2) }
//! }
//! ```

use crate::il::ast::{
    BlockKind, ConversionKind, Flags, Function, Inst, MethodOperand, NodeId, NopKind, VarId,
    VariableKind, conversion_kind,
};
use crate::il::reader::ReadFunction;
use crate::il::types::{Sign, StackType};
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::members::RefKind;
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::csharp_semantics::{implicit_conversion, type_erasure_equivalent};
use super::il_inlining::{Find, find_load_in_next, options};
use super::statement_transform::StatementTransformContext;

/// `TupleType.RestPosition`.
const TUPLE_REST: usize = 8;

/// A matched Deconstruct call: one node of the (possibly nested) deconstruction pattern.
#[derive(Clone)]
struct DeconstructionCall {
    method: MethodOperand,
    /// The pattern variable of this node; None for the root, which gets a fresh one.
    receiver: Option<VarId>,
    /// The out-argument variable of each element.
    results: Vec<VarId>,
    /// The nested deconstruction of each element; None for a leaf.
    nested_calls: Vec<Option<DeconstructionCall>>,
}

/// A tuple variable being deconstructed: one node of the (possibly nested) designation.
struct TupleNode {
    variable: VarId,
    /// The tuple type (`TupleType`).
    ty: TypeId,
    element_types: Vec<TypeId>,
    /// The nested designation of each element (an index into `tuple_nodes`); None for a leaf.
    nested_elements: Vec<Option<usize>>,
    /// The flat (depth-first) leaf index of each element.
    element_flat_index: Vec<usize>,
    /// How many element reads of the variable the pattern consumed.
    matched_access_count: u32,
}

/// What the managed transform's `delayedActions` do once the transform is committed.
enum Action {
    /// `assignmentTarget.Type = inputType`.
    SetVariableType(VarId, TypeId),
    /// Add the assignment to the assignments block.
    AddAssignment(NodeId),
    /// `call.Arguments[last] = value`, then add the call to the assignments block.
    AddInlineAssignCall { call: NodeId, value: NodeId },
    /// Add a variable created at match time to the function.
    AddVariable(VarId),
    /// Replace the element read with a load of the result of that flat index.
    ReplaceWithResult { inst: NodeId, flat_index: usize },
    /// A conversion without assignment: its output is assigned from a fresh variable.
    ForwardConversion { stloc: NodeId },
    /// A single-use element used after the deconstruction: its load goes through a fresh
    /// variable assigned inside the deconstruction.
    ForwardResult { v: VarId },
}

/// The kind and value of a matched conversion (`ConversionInfo`).
#[derive(Clone, Copy)]
struct ConversionInfo {
    input_type: Option<TypeId>,
    conv: Option<NodeId>,
}

/// The transform's state.
#[derive(Default)]
struct Deconstruction {
    /// `deconstructionResultsLookup`.
    results_lookup: Vec<(VarId, usize)>,
    /// `tupleNodes` (the arena) with the variable of each.
    tuple_nodes: Vec<TupleNode>,
    /// `deconstructionResults`.
    results: Vec<Option<VarId>>,
    tuple_root: Option<usize>,
    rooted_in_deconstruct_call: bool,
}

impl Deconstruction {
    fn lookup(&self, v: VarId) -> Option<usize> {
        self.results_lookup.iter().find(|e| e.0 == v).map(|e| e.1)
    }

    fn tuple_node_of(&self, v: VarId) -> Option<usize> {
        self.tuple_nodes.iter().position(|n| n.variable == v)
    }
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
    if !ctx.settings.deconstruction {
        return;
    }
    let f = &mut r.function;
    let mut d = Deconstruction::default();
    if transform_deconstruction(f, ctx, &mut d, block, pos) {
        return;
    }
    let mut d = Deconstruction::default();
    inline_deconstruction_initializer(f, ctx, &mut d, block, pos);
}

fn call_parts(f: &Function, inst: NodeId) -> Option<(&MethodOperand, &[NodeId])> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => Some((&c.method, &c.args)),
        _ => None,
    }
}

/// `MatchLdLocOrLdLoca`.
fn match_ldloc_or_ldloca(f: &Function, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::LdLoc { var } | Inst::LdLoca { var } => Some(var),
        _ => None,
    }
}

/// `MatchInstruction.IsDeconstructMethod`.
fn is_deconstruct_method(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    if m.name != "Deconstruct" {
        return false;
    }
    if ctx
        .ts
        .definition(m.ret)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_none_or(|i| i.kind != TypeKind::Void)
    {
        return false;
    }
    let MethodOrigin::Def(id) = m.origin else {
        return false;
    };
    let first_out_param = usize::from(m.is_static);
    if m.is_static {
        let owner = ctx.ts.accessor_owner(id).map(|(o, _)| o);
        if !ctx.ts.method_flags(id, owner).extension {
            return false;
        }
    } else if !m.type_args.is_empty() {
        return false;
    }
    let params = ctx.ts.method_signature(id).1;
    if params.len() < first_out_param {
        return false;
    }
    params[first_out_param..]
        .iter()
        .all(|p| p.ref_kind == RefKind::Out)
}

/// `ILInstruction.IsDescendantOf` (including the node itself).
fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    std::iter::once(inst)
        .chain(f.ancestors(inst))
        .any(|n| n == ancestor)
}

/// `ILInstruction.IsBefore`: whether `a` comes before `b` in a post-order walk.
fn is_before(f: &Function, a: NodeId, b: NodeId) -> bool {
    let level = |n: NodeId| f.ancestors(n).count() + 1;
    let (mut a, mut b) = (a, b);
    let (mut level_a, mut level_b) = (level(a), level(b));
    let (original_a, original_b) = (level_a, level_b);
    while level_a > level_b {
        a = f.node(a).parent.expect("an ancestor");
        level_a -= 1;
    }
    while level_b > level_a {
        b = f.node(b).parent.expect("an ancestor");
        level_b -= 1;
    }
    if a == b {
        // One is a descendant of the other: the deeper one comes first in post-order.
        return original_a > original_b;
    }
    while f.node(a).parent != f.node(b).parent {
        a = f.node(a).parent.expect("an ancestor");
        b = f.node(b).parent.expect("an ancestor");
    }
    f.node(a).child_index < f.node(b).child_index
}

/// `TupleType.GetTupleElementTypes`: the element types of a tuple or `System.ValueTuple`
/// type, flattening `Rest`; None for other types.
fn tuple_element_types(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<Vec<TypeId>> {
    ctx.ts.tuple_element_types(t)
}

/// `TupleType.FromUnderlyingType`: the tuple type (and its element types) of a tuple-compatible
/// type.
fn tuple_from_underlying_type(
    ctx: &mut Context<'_, '_, '_>,
    t: TypeId,
) -> Option<(TypeId, Vec<TypeId>)> {
    let elements = tuple_element_types(ctx, t)?;
    if elements.is_empty() {
        return None;
    }
    let module = ctx.ts.definition(t).map(|d| d.module);
    let underlying = ctx.ts.tuple_underlying(&elements, module);
    let names = vec![None; elements.len()].into();
    let tuple = ctx.ts.intern(TypeData::Tuple {
        underlying,
        elements: elements.clone().into(),
        names,
    });
    Some((tuple, elements))
}

/// `TupleType.IsTupleCompatible`.
fn is_tuple_compatible(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    if matches!(ctx.ts.data(t), TypeData::Tuple { .. }) {
        return true;
    }
    let Some(def) = ctx.ts.definition(t) else {
        return false;
    };
    let Ok(info) = ctx.ts.comp.type_def(def) else {
        return false;
    };
    if info.kind != TypeKind::Struct
        || info.full_name.top.ns != "System"
        || info.full_name.name() != "ValueTuple"
    {
        return false;
    }
    let tpc = info.type_parameter_count as usize;
    if tpc > 0 && tpc < TUPLE_REST {
        return true;
    }
    if tpc == TUPLE_REST {
        let args = ctx.ts.type_arguments(t);
        return args.len() == TUPLE_REST && is_tuple_compatible(ctx, args[TUPLE_REST - 1]);
    }
    false
}

/// `TupleTransform.MatchTupleFieldAccess`: `ldflda ItemN(target)` (through `Rest` fields for
/// long tuples); the tuple type, the target and the one-based position.
fn match_tuple_field_access(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldflda: NodeId,
) -> Option<(TypeId, NodeId, usize)> {
    let Inst::LdFlda { target, field, .. } = f.inst(ldflda) else {
        return None;
    };
    let mut tuple_type = field.field.declaring?;
    let mut target = *target;
    let mut position: usize = field.field.name.strip_prefix("Item")?.parse().ok()?;
    if !is_tuple_compatible(ctx, tuple_type) {
        return None;
    }
    while let Inst::LdFlda {
        target: t, field, ..
    } = f.inst(target)
        && field.field.name == "Rest"
        && let Some(d) = field.field.declaring
        && is_tuple_compatible(ctx, d)
    {
        tuple_type = d;
        target = *t;
        position += TUPLE_REST - 1;
    }
    Some((tuple_type, target, position))
}

/// `MatchTupleElementRead`: `ldobj(ldflda ItemN(ldloc(a) container))`; the container, its
/// type and the zero-based index.
fn match_tuple_element_read(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(VarId, TypeId, usize)> {
    let Inst::LdObj {
        target,
        is_volatile: false,
        unaligned: 0,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    if !matches!(f.inst(target), Inst::LdFlda { .. }) {
        return None;
    }
    let (container_type, target, position) = match_tuple_field_access(f, ctx, target)?;
    let container = match_ldloc_or_ldloca(f, target)?;
    Some((container, container_type, position.checked_sub(1)?))
}

/// `MatchTupleElementStore`: `stloc temp(ldobj(ldflda ItemN(ldloc(a) container)))`.
fn match_tuple_element_store(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: Option<NodeId>,
) -> Option<(VarId, VarId, TypeId, usize)> {
    let (temp, value) = f.match_stloc(inst?)?;
    let (container, container_type, index) = match_tuple_element_read(f, ctx, value)?;
    Some((temp, container, container_type, index))
}

/// `TransformDeconstruction`.
fn transform_deconstruction(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: usize,
) -> bool {
    let start_pos = pos;
    // Blocks are processed back to front: the inner parts of a nested deconstruction come
    // first and must wait for the enclosing attempt.
    if is_consumable_by_enclosing_deconstruction(f, ctx, block, pos) {
        return false;
    }
    // The value-semantics copy before a root Deconstruct call waits for the attempt at the
    // copy, unless that attempt is itself deferred.
    if pos > 0
        && is_root_deconstruction_copy(f, ctx, block, pos - 1).is_some()
        && !is_consumable_by_enclosing_deconstruction(f, ctx, block, pos - 1)
    {
        return false;
    }
    let Some(seq) = match_deconstruction_sequence(f, ctx, d, block, start_pos) else {
        return false;
    };
    let MatchedSequence {
        end_pos,
        root_call,
        root_tested_operand,
        conversion_stlocs,
        actions,
    } = seq;
    let (deconstructed_type, tested_operand) = match &root_call {
        None => {
            let root = &d.tuple_nodes[d.tuple_root.expect("a tuple root")];
            let (ty, var) = (root.ty, root.variable);
            (ty, f.add(Inst::LdLoc { var }))
        }
        Some(call) => {
            let m = &call.method.method;
            let ty = if m.is_static {
                m.params().ok().and_then(|p| p.first().copied())
            } else {
                m.declaring
            };
            (
                ty.unwrap_or_else(|| ctx.ts.intern(TypeData::UnknownSpecial)),
                root_tested_operand.expect("a tested operand"),
            )
        }
    };
    let root_temp = f.register_variable(ctx.ts, VariableKind::PatternLocal, deconstructed_type);
    let pattern = match &root_call {
        Some(call) => build_pattern_match(f, call, root_temp, tested_operand),
        None => {
            let root = d.tuple_root.expect("a tuple root");
            build_tuple_pattern_match(f, ctx, d, root, root_temp, tested_operand)
        }
    };
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let conversions = f.add(Inst::Block {
        kind: BlockKind::DeconstructionConversions,
        instructions: conversion_stlocs,
        final_inst: nop,
    });
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let assignments = f.add(Inst::Block {
        kind: BlockKind::DeconstructionAssignments,
        instructions: Vec::new(),
        final_inst: nop,
    });
    let replacement = f.add(Inst::Deconstruct {
        init: Vec::new(),
        pattern,
        conversions,
        assignments,
    });
    run_actions(f, ctx, d, actions, replacement);
    f.set_child_at(block, start_pos, replacement);
    for _ in start_pos + 1..end_pos {
        f.remove_at(block, start_pos + 1);
    }
    true
}

/// Runs the delayed actions on the committed `deconstruct` instruction.
fn run_actions(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &Deconstruction,
    actions: Vec<Action>,
    deconstruct: NodeId,
) {
    let Inst::Deconstruct { assignments, .. } = *f.inst(deconstruct) else {
        unreachable!("a deconstruction");
    };
    for action in actions {
        match action {
            Action::SetVariableType(v, ty) => f.var_mut(v).ty = ty,
            Action::AddAssignment(inst) => f.push(assignments, inst),
            Action::AddInlineAssignCall { call, value } => {
                let last = f.inst(call).children().len() - 1;
                f.set_child_at(call, last, value);
                f.push(assignments, call);
            }
            Action::AddVariable(v) => f.pending_variables.push(v),
            Action::ReplaceWithResult { inst, flat_index } => {
                let var = d.results[flat_index].expect("a result variable");
                let load = f.add(Inst::LdLoc { var });
                f.replace(inst, load);
            }
            Action::ForwardConversion { stloc } => {
                let (v, _) = f.match_stloc(stloc).expect("a conversion store");
                let ty = f.var(v).ty;
                let fresh = f.register_variable(ctx.ts, VariableKind::StackSlot, ty);
                let load = f.add(Inst::LdLoc { var: fresh });
                let store = f.add(Inst::StLoc {
                    var: v,
                    value: load,
                    il_stack_was_empty: false,
                    is_stack_adjustment: false,
                });
                f.push(assignments, store);
                f.set_variable(stloc, fresh);
            }
            Action::ForwardResult { v } => {
                let load = f.var(v).load_instructions[0];
                if is_descendant_of(f, load, deconstruct) {
                    continue;
                }
                let index = d.lookup(v).expect("a deconstruction result");
                let ty = f.var(v).ty;
                let fresh = f.register_variable(ctx.ts, VariableKind::StackSlot, ty);
                let instructions = f.list(assignments).to_vec();
                let mut insert_pos = 0;
                while insert_pos < instructions.len()
                    && assignment_index(f, ctx, d, instructions[insert_pos]) < index
                {
                    insert_pos += 1;
                }
                let value = f.add(Inst::LdLoc { var: v });
                let store = f.add(Inst::StLoc {
                    var: fresh,
                    value,
                    il_stack_was_empty: false,
                    is_stack_adjustment: false,
                });
                f.insert(assignments, insert_pos, store);
                f.set_variable(load, fresh);
            }
        }
    }
}

/// `GetAssignmentIndex`.
fn assignment_index(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &Deconstruction,
    inst: NodeId,
) -> usize {
    if let Some((_, value)) = is_assignment(f, ctx, inst)
        && let Some(input) = f.match_ldloc(value)
    {
        if let Some(index) = d.lookup(input) {
            return index;
        }
        // A forwarding assignment for a conversion loads a fresh variable; its index is that
        // of the conversion output it stores to.
        if let Some((v, _)) = f.match_stloc(inst)
            && let Some(index) = d.lookup(v)
        {
            return index;
        }
    }
    usize::MAX
}

/// The result of `MatchDeconstructionSequence`.
struct MatchedSequence {
    end_pos: usize,
    root_call: Option<DeconstructionCall>,
    root_tested_operand: Option<NodeId>,
    conversion_stlocs: Vec<NodeId>,
    actions: Vec<Action>,
}

/// `MatchDeconstructionSequence`: the statements of one deconstruction from `start_pos`; the
/// block is not modified.
fn match_deconstruction_sequence(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    start_pos: usize,
) -> Option<MatchedSequence> {
    let mut do_not_nest: Vec<VarId> = Vec::new();
    loop {
        *d = Deconstruction::default();
        let mut pos = start_pos;
        let mut actions = Vec::new();
        let (root_call, root_tested_operand) = match_deconstruction(f, ctx, d, block, &mut pos);
        if root_call.is_none() {
            match_nested_tuple_designations(f, ctx, d, block, &mut pos, &do_not_nest);
        }
        let (conversions, conversion_stlocs) =
            match_conversions(f, ctx, d, block, &mut pos, &mut actions)?;
        let any_assignments = match_assignments(
            f,
            ctx,
            d,
            block,
            &mut pos,
            &conversions,
            &conversion_stlocs,
            &mut actions,
            root_call.is_some(),
        )?;
        // Without an assignment the statement is a plain Deconstruct call, unless a nested
        // deconstruction was consumed.
        let has_nested = root_call
            .as_ref()
            .is_some_and(|c| c.nested_calls.iter().any(Option::is_some));
        if !any_assignments && !has_nested {
            return None;
        }
        // A nested tuple designation holds only if the pattern consumed every read of its
        // temporary; otherwise retry with the variable as a plain leaf.
        let escaped = escaped_tuple_nodes(f, d);
        if !escaped.is_empty() {
            do_not_nest.extend(escaped);
            continue;
        }
        // The first element may not be discarded; that would match a suffix of the pattern.
        d.results.first().copied().flatten()?;
        return Some(MatchedSequence {
            end_pos: pos,
            root_call,
            root_tested_operand,
            conversion_stlocs,
            actions,
        });
    }
}

/// The nested tuple designation variables whose reads the pattern did not all consume.
fn escaped_tuple_nodes(f: &Function, d: &Deconstruction) -> Vec<VarId> {
    let mut escaped = Vec::new();
    for (i, node) in d.tuple_nodes.iter().enumerate() {
        if Some(i) == d.tuple_root {
            continue;
        }
        let v = f.var(node.variable);
        if node.matched_access_count != v.load_count() + v.address_count() {
            escaped.push(node.variable);
        }
    }
    escaped
}

/// `InlineDeconstructionInitializer`: `stloc v(value)` before a deconstruction that uses `v`
/// in its assignments becomes an init store of the deconstruction.
fn inline_deconstruction_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: usize,
) -> bool {
    let stloc = f.list(block)[pos];
    let Some((v, value)) = f.match_stloc(stloc) else {
        return false;
    };
    if !(f.var(v).is_single_definition() && f.var(v).load_count() == 1) {
        return false;
    }
    let load_inst = f.var(v).load_instructions[0];
    let Some(&next) = f.list(block).get(pos + 1) else {
        return false;
    };
    let Find::Deconstruction(deconstruction) =
        find_load_in_next(f, ctx, Some(next), v, value, options::FIND_DECONSTRUCTION)
    else {
        return false;
    };
    let Inst::Deconstruct {
        ref init,
        assignments,
        ..
    } = *f.inst(deconstruction)
    else {
        unreachable!("a deconstruction");
    };
    let first_init = init.first().copied();
    if !is_descendant_of(f, load_inst, assignments) {
        return false;
    }
    let parent = f.node(load_inst).parent.expect("a parent");
    if matches!(f.inst(parent), Inst::StObj { .. })
        && f.node(load_inst).child_index == 0
        && matches!(f.inst(value), Inst::LdFlda { .. } | Inst::LdElema { .. })
    {
        return false;
    }
    if let Some(first) = first_init {
        let (first_var, _) = f.match_stloc(first).expect("an init store");
        let a = f.var(first_var).load_instructions[0];
        if !is_before(f, load_inst, a) {
            return false;
        }
    }
    let _ = d;
    f.insert(deconstruction, 0, stloc);
    f.remove_at(block, pos);
    f.var_mut(v).kind = VariableKind::DeconstructionInitTemporary;
    true
}

/// `IsConsumableByEnclosingDeconstruction`: whether the statement at `pos` belongs to a
/// deconstruction whose matching starts earlier in the block.
fn is_consumable_by_enclosing_deconstruction(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    if let Some(enclosing_pos) = try_find_enclosing_deconstruction_call(f, ctx, block, pos) {
        let list = f.list(block);
        let copy_between = enclosing_pos + 2 == pos
            && f.match_stloc(list[pos - 1])
                .is_some_and(|(_, value)| matches!(f.inst(value), Inst::LdLoc { .. }));
        if enclosing_pos + 1 != pos && !copy_between {
            return false;
        }
        let (_, args) = call_parts(f, list[enclosing_pos]).expect("a call");
        for &arg in &args[1..] {
            let Inst::LdLoca { var } = *f.inst(arg) else {
                return false;
            };
            let v = f.var(var);
            if !(v.store_count() == 0 && v.address_count() == 1 && v.load_count() <= 1) {
                return false;
            }
        }
        return true;
    }
    has_enclosing_tuple_designation(f, ctx, block, pos)
}

/// `TryFindEnclosingDeconstructionCall`: the position of the Deconstruct call one of whose
/// out-arguments the Deconstruct call at `pos` deconstructs further.
fn try_find_enclosing_deconstruction_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> Option<usize> {
    let (m, args) = call_parts(f, f.list(block)[pos])?;
    if !is_deconstruct_method(ctx, &m.method.clone()) || args.is_empty() {
        return None;
    }
    let mut v = match_ldloc_or_ldloca(f, args[0])?;
    // Through the defensive copy of a struct element.
    if let &[copy] = &f.var(v).store_instructions[..]
        && let Some((_, value)) = f.match_stloc(copy)
        && let Some(source) = f.match_ldloc(value)
    {
        v = source;
    }
    // StoreCount also counts the initial value of parameters, on purpose.
    if f.var(v).store_count() != 0 {
        return None;
    }
    let &[address_load] = &f.var(v).address_instructions[..] else {
        return None;
    };
    let enclosing_call = f.node(address_load).parent?;
    let (m, _) = call_parts(f, enclosing_call)?;
    if f.node(address_load).child_index == 0 || !is_deconstruct_method(ctx, &m.method.clone()) {
        return None;
    }
    if f.node(enclosing_call).parent != Some(block) {
        return None;
    }
    let enclosing_pos = f.node(enclosing_call).child_index as usize;
    (enclosing_pos < pos).then_some(enclosing_pos)
}

/// `HasEnclosingTupleDesignation`: the statement at `pos` reads an element of a tuple that an
/// earlier element read stored, with only element reads in between.
fn has_enclosing_tuple_designation(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let Some((_, mut value)) = f.match_stloc(f.list(block)[pos]) else {
        return false;
    };
    if let Inst::Conv { arg, .. } = *f.inst(value) {
        value = arg;
    }
    let Some((container, _, _)) = match_tuple_element_read(f, ctx, value) else {
        return false;
    };
    let &[store] = &f.var(container).store_instructions[..] else {
        return false;
    };
    if f.node(store).parent != Some(block) || !matches!(f.inst(store), Inst::StLoc { .. }) {
        return false;
    }
    if match_tuple_element_store(f, ctx, Some(store)).is_none() {
        return false;
    }
    let store_pos = f.node(store).child_index as usize;
    if store_pos >= pos {
        return false;
    }
    // The temporaries and element reads of one designation are stored back to back.
    (store_pos + 1..pos)
        .all(|between| match_tuple_element_store(f, ctx, Some(f.list(block)[between])).is_some())
}

/// `MatchDeconstruction`: the root Deconstruct call (after its value-semantics copy) and its
/// nested calls; the leaves get flat indices in depth-first order.
fn match_deconstruction(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: &mut usize,
) -> (Option<DeconstructionCall>, Option<NodeId>) {
    let mut root_call = None;
    let mut tested_operand = None;
    // A deconstruction has value semantics: Roslyn copies a local or parameter into a
    // temporary first, which the pattern consumes.
    if let Some((copied_value, call)) = is_root_deconstruction_copy(f, ctx, block, *pos) {
        tested_operand = Some(copied_value);
        root_call = Some(call);
        *pos += 1;
    }
    if root_call.is_none() {
        let Some(&inst) = f.list(block).get(*pos) else {
            return (None, None);
        };
        if let Some((call, operand)) = match_deconstruction_call(f, ctx, inst) {
            root_call = Some(call);
            tested_operand = Some(operand);
        }
    }
    let Some(mut root) = root_call else {
        return (None, None);
    };
    d.rooted_in_deconstruct_call = true;
    *pos += 1;
    match_nested_deconstructions(f, ctx, block, pos, &mut root);
    let mut leaves = Vec::new();
    collect_leaves(&root, &mut leaves);
    d.results = leaves.iter().map(|&v| Some(v)).collect();
    for (i, &v) in leaves.iter().enumerate() {
        d.results_lookup.push((v, i));
    }
    (Some(root), tested_operand)
}

fn collect_leaves(call: &DeconstructionCall, leaves: &mut Vec<VarId>) {
    for (i, &result) in call.results.iter().enumerate() {
        match &call.nested_calls[i] {
            Some(nested) => collect_leaves(nested, leaves),
            None => leaves.push(result),
        }
    }
}

/// `IsRootDeconstructionCopy`: `stloc copy(value)` followed by a root Deconstruct call on the
/// otherwise unused copy; the value and the call.
fn is_root_deconstruction_copy(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> Option<(NodeId, DeconstructionCall)> {
    let list = f.list(block);
    if pos + 1 >= list.len() {
        return None;
    }
    let (copy, value) = f.match_stloc(list[pos])?;
    let c = f.var(copy);
    if !matches!(c.kind, VariableKind::Local | VariableKind::StackSlot) {
        return None;
    }
    // A byref temporary is no copy.
    if c.stack_type == StackType::Ref {
        return None;
    }
    if !(c.store_count() == 1 && c.load_count() + c.address_count() == 1) {
        return None;
    }
    // The defensive copy of a struct element of an enclosing call belongs to that call.
    if try_find_enclosing_deconstruction_call(f, ctx, block, pos + 1).is_some() {
        return None;
    }
    let (call, tested_operand) = match_deconstruction_call(f, ctx, list[pos + 1])?;
    if match_ldloc_or_ldloca(f, tested_operand) != Some(copy) {
        return None;
    }
    Some((value, call))
}

/// `MatchDeconstructionCall`: `call(virt) Deconstruct(target, ldloca out0, ldloca out1, ...)`
/// with single-use out-arguments; the call and the target.
fn match_deconstruction_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(DeconstructionCall, NodeId)> {
    let (m, args) = call_parts(f, inst)?;
    let (method, args) = (m.clone(), args.to_vec());
    if !is_deconstruct_method(ctx, &method.method) {
        return None;
    }
    let declaring_is_value = method
        .method
        .declaring
        .is_some_and(|t| ctx.ts.is_reference_type(t) == Some(false));
    let wants_call = method.method.is_static || declaring_is_value;
    match f.inst(inst) {
        Inst::Call(_) if wants_call => {}
        Inst::CallVirt(_) if !wants_call => {}
        _ => return None,
    }
    if args.len() < 3 {
        return None;
    }
    let mut results = Vec::with_capacity(args.len() - 1);
    for &arg in &args[1..] {
        let Inst::LdLoca { var } = *f.inst(arg) else {
            return None;
        };
        let v = f.var(var);
        if !(v.store_count() == 0 && v.address_count() == 1 && v.load_count() <= 1) {
            return None;
        }
        results.push(var);
    }
    let n = results.len();
    Some((
        DeconstructionCall {
            method,
            receiver: None,
            results,
            nested_calls: vec![None; n],
        },
        args[0],
    ))
}

/// `MatchNestedDeconstructions`: depth-first, the nested Deconstruct calls on elements of the
/// parent call (each after an optional defensive copy).
fn match_nested_deconstructions(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: &mut usize,
    root: &mut DeconstructionCall,
) {
    // The path of element indices from the root to the call being visited, and the element.
    let mut pending: Vec<(Vec<usize>, usize)> = vec![(Vec::new(), 0)];
    while let Some((path, i)) = pending.pop() {
        let parent = call_at(root, &path);
        if i + 1 < parent.results.len() {
            pending.push((path.clone(), i + 1));
        }
        let result = parent.results[i];
        let saved_pos = *pos;
        let mut receiver = result;
        let mut inst = f.list(block).get(*pos).copied();
        if let Some(stloc) = inst
            && let Some((copy, copied_value)) = f.match_stloc(stloc)
            && f.match_ldloc(copied_value) == Some(result)
            && f.var(copy).store_count() == 1
            && f.var(copy).load_count() + f.var(copy).address_count() == 1
        {
            receiver = copy;
            *pos += 1;
            inst = f.list(block).get(*pos).copied();
        }
        let nested = inst.and_then(|i| match_deconstruction_call(f, ctx, i));
        let Some((mut nested, target)) = nested else {
            *pos = saved_pos;
            continue;
        };
        if match_ldloc_or_ldloca(f, target) != Some(receiver) {
            *pos = saved_pos;
            continue;
        }
        // The copy must be the element's only use.
        if receiver != result && f.var(result).load_count() != 1 {
            *pos = saved_pos;
            continue;
        }
        // A nested designation rebinds Deconstruct on the element's static type; a different
        // binding (member hiding) keeps the call explicit.
        let element_type = f.var(result).ty;
        if !binds_on_element_type(ctx, &nested.method.method, element_type) {
            *pos = saved_pos;
            continue;
        }
        *pos += 1;
        nested.receiver = Some(receiver);
        call_at_mut(root, &path).nested_calls[i] = Some(nested);
        // Its elements are evaluated before the parent's remaining ones.
        let mut nested_path = path;
        nested_path.push(i);
        pending.push((nested_path, 0));
    }
}

fn call_at<'a>(root: &'a DeconstructionCall, path: &[usize]) -> &'a DeconstructionCall {
    path.iter().fold(root, |c, &i| {
        c.nested_calls[i].as_ref().expect("a nested call")
    })
}

fn call_at_mut<'a>(root: &'a mut DeconstructionCall, path: &[usize]) -> &'a mut DeconstructionCall {
    path.iter().fold(root, |c, &i| {
        c.nested_calls[i].as_mut().expect("a nested call")
    })
}

/// `BindsOnElementType`: whether `Deconstruct` called on the element's static type binds to
/// `method`.
fn binds_on_element_type(
    ctx: &mut Context<'_, '_, '_>,
    method: &ResolvedMethod,
    element_type: TypeId,
) -> bool {
    let param_count = method.params().map_or(0, |p| p.len());
    let out_param_count = param_count - usize::from(method.is_static);
    let mut ty = Some(element_type);
    while let Some(t) = ty {
        if !method.is_static
            && let Some(declaring) = method.declaring
            && type_erasure_equivalent(ctx, t, declaring)
        {
            return true;
        }
        // An instance Deconstruct of the same arity on a more derived type hides the method.
        let methods = ctx
            .ts
            .get_methods(t, None, MemberOptions::DECLARED)
            .unwrap_or_default();
        if methods.iter().any(|m| {
            m.name == "Deconstruct"
                && !m.is_static
                && m.params().is_ok_and(|p| p.len() == out_param_count)
        }) {
            return false;
        }
        ty = ctx.ts.definition(t).and_then(|def| {
            let bases = ctx.ts.direct_base_types(def).unwrap_or_default();
            bases.into_iter().find(|&b| {
                ctx.ts
                    .definition(b)
                    .and_then(|bd| ctx.ts.comp.type_def(bd).ok())
                    .is_some_and(|i| i.kind == TypeKind::Class)
            })
        });
    }
    // Without the declaring type in the chain, only an extension on the element type itself
    // binds for certain.
    method.is_static
        && method
            .params()
            .ok()
            .and_then(|p| p.first().copied())
            .is_some_and(|p| type_erasure_equivalent(ctx, element_type, p))
}

/// `MatchNestedTupleDesignations`: the temporaries of nested tuple designations, parents
/// before children.
fn match_nested_tuple_designations(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: &mut usize,
    do_not_nest: &[VarId],
) {
    while let Some((temp, container, container_type, index)) =
        match_tuple_element_store(f, ctx, f.list(block).get(*pos).copied())
    {
        if do_not_nest.contains(&temp) {
            break;
        }
        let t = f.var(temp);
        if !(t.store_count() == 1 && t.load_count() + t.address_count() >= 1) {
            break;
        }
        // Every use must be an element read, so that the pattern can consume them all.
        if !all_uses_are_tuple_element_reads(f, temp) {
            break;
        }
        let Some(container_node) = resolve_tuple_container(f, ctx, d, container, container_type)
        else {
            break;
        };
        let node = &d.tuple_nodes[container_node];
        if index >= node.nested_elements.len() || node.nested_elements[index].is_some() {
            break;
        }
        // The container's element type is authoritative for the temporary's tuple type.
        let element_type = node.element_types[index];
        if tuple_element_types(ctx, element_type).is_none_or(|e| e.is_empty()) {
            break;
        }
        let Some((temp_type, temp_elements)) = tuple_from_underlying_type(ctx, element_type) else {
            break;
        };
        if temp_elements.len() < 2 {
            break;
        }
        let temp_var_type = f.var(temp).ty;
        if tuple_element_types(ctx, temp_var_type).is_some_and(|e| !e.is_empty())
            && !type_erasure_equivalent(ctx, element_type, temp_var_type)
        {
            break;
        }
        let new_node = d.tuple_nodes.len();
        let cardinality = temp_elements.len();
        d.tuple_nodes.push(TupleNode {
            variable: temp,
            ty: temp_type,
            element_types: temp_elements,
            nested_elements: vec![None; cardinality],
            element_flat_index: Vec::new(),
            matched_access_count: 0,
        });
        let container = &mut d.tuple_nodes[container_node];
        container.nested_elements[index] = Some(new_node);
        // The temporary's store reads one element of the container.
        container.matched_access_count += 1;
        initialize_flat_leaf_indices(d);
        *pos += 1;
    }
}

/// `AllUsesAreTupleElementReads`.
fn all_uses_are_tuple_element_reads(f: &Function, temp: VarId) -> bool {
    let v = f.var(temp);
    v.address_instructions
        .iter()
        .chain(&v.load_instructions)
        .all(|&use_| {
            let mut node = f.node(use_).parent;
            if !node.is_some_and(|n| matches!(f.inst(n), Inst::LdFlda { .. })) {
                return false;
            }
            while let Some(n) = node
                && matches!(f.inst(n), Inst::LdFlda { .. })
            {
                node = f.node(n).parent;
            }
            node.is_some_and(|n| matches!(f.inst(n), Inst::LdObj { .. }))
        })
}

/// `ResolveTupleContainer`: the tuple node of the container (the first one becomes the root).
fn resolve_tuple_container(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    container: VarId,
    container_type: TypeId,
) -> Option<usize> {
    let _ = f;
    let (normalized, elements) = tuple_from_underlying_type(ctx, container_type)?;
    if elements.len() < 2 {
        return None;
    }
    if d.tuple_root.is_none() {
        let cardinality = elements.len();
        d.tuple_nodes.push(TupleNode {
            variable: container,
            ty: normalized,
            element_types: elements,
            nested_elements: vec![None; cardinality],
            element_flat_index: Vec::new(),
            matched_access_count: 0,
        });
        d.tuple_root = Some(d.tuple_nodes.len() - 1);
        initialize_flat_leaf_indices(d);
    }
    let node = d.tuple_node_of(container)?;
    (d.tuple_nodes[node].ty == normalized).then_some(node)
}

/// `InitializeFlatLeafIndices`: the depth-first flat leaf indices of the tuple node tree, and
/// an empty results array of the leaf count.
fn initialize_flat_leaf_indices(d: &mut Deconstruction) {
    fn assign(nodes: &mut [TupleNode], node: usize, mut next: usize) -> usize {
        let cardinality = nodes[node].element_types.len();
        nodes[node].element_flat_index = vec![0; cardinality];
        for i in 0..cardinality {
            nodes[node].element_flat_index[i] = next;
            match nodes[node].nested_elements[i] {
                Some(nested) => next = assign(nodes, nested, next),
                None => next += 1,
            }
        }
        next
    }
    let root = d.tuple_root.expect("a tuple root");
    let total = assign(&mut d.tuple_nodes, root, 0);
    d.results = vec![None; total];
}

/// The matched conversions (output variable and kind) and their stores.
type MatchedConversions = (Vec<(VarId, ConversionInfo)>, Vec<NodeId>);

/// `MatchConversions`: the single-use conversion temporaries after the deconstruction, in
/// flat index order.
fn match_conversions(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: &mut usize,
    actions: &mut Vec<Action>,
) -> Option<MatchedConversions> {
    let mut conversions = Vec::new();
    let mut conversion_stlocs = Vec::new();
    let mut previous_index: isize = -1;
    while let Some(&inst) = f.list(block).get(*pos)
        && let Some((output, conv)) = f.match_stloc(inst)
        && let Inst::Conv { arg: input, .. } = *f.inst(conv)
    {
        let info = ConversionInfo {
            input_type: Some(f.infer_type(ctx.ts, input)),
            conv: Some(conv),
        };
        let index = find_index(f, ctx, d, input, actions);
        if index <= previous_index {
            return None;
        }
        let v = f.var(output);
        if !(v.is_single_definition() && v.load_count() == 1) {
            return None;
        }
        d.results_lookup.push((output, index as usize));
        conversions.push((output, info));
        conversion_stlocs.push(inst);
        *pos += 1;
        previous_index = index;
    }
    Some((conversions, conversion_stlocs))
}

/// `MatchAssignments`: the assignments after the conversions, in flat index order; Some(any
/// assignments) on success.
#[allow(clippy::too_many_arguments)]
fn match_assignments(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    block: NodeId,
    pos: &mut usize,
    conversions: &[(VarId, ConversionInfo)],
    conversion_stlocs: &[NodeId],
    actions: &mut Vec<Action>,
    allow_unrelated_assignments: bool,
) -> Option<bool> {
    let mut previous_index: isize = -1;
    let mut conversion_stloc_index = 0;
    let start_pos = *pos;
    while let Some(&inst) = f.list(block).get(*pos)
        && let Some((target_type, value_inst, add_assignment)) = match_assignment(f, ctx, inst)
    {
        let mut adjustments = Vec::new();
        let index = find_index(f, ctx, d, value_inst, &mut adjustments);
        if index < 0 && allow_unrelated_assignments {
            // For a Deconstruct call an unrelated assignment ends the pattern.
            break;
        }
        if index <= previous_index {
            return None;
        }
        add_missing_assignments_for_conversions(
            f,
            d,
            conversion_stlocs,
            &mut conversion_stloc_index,
            &mut previous_index,
            index as usize,
            actions,
        );
        let conversion_info = f
            .match_ldloc(value_inst)
            .and_then(|v| conversions.iter().find(|c| c.0 == v).map(|c| c.1))
            .unwrap_or_else(|| ConversionInfo {
                input_type: Some(f.infer_type(ctx.ts, value_inst)),
                conv: None,
            });
        if let Some((assignment_target, _)) = f.match_stloc(inst)
            && f.var(assignment_target).kind == VariableKind::StackSlot
            && f.var(assignment_target).is_single_definition()
            && conversion_info.conv.is_none()
        {
            actions.push(Action::SetVariableType(
                assignment_target,
                conversion_info.input_type.expect("an input type"),
            ));
        } else if !is_compatible_implicit_conversion(f, ctx, target_type, conversion_info) {
            return None;
        }
        actions.push(add_assignment);
        actions.extend(adjustments);
        *pos += 1;
        previous_index = index;
    }
    add_missing_assignments_for_conversions(
        f,
        d,
        conversion_stlocs,
        &mut conversion_stloc_index,
        &mut previous_index,
        usize::MAX,
        actions,
    );
    // A single-use element used directly after the deconstruction (optimized code) is
    // forwarded through a fresh variable assigned inside the deconstruction. The load count
    // is read now, at match time: for tuples the elements are fresh variables whose loads
    // only appear when the actions run, so forwarding does not apply there.
    for v in d.results.clone().into_iter().flatten() {
        if f.var(v).load_count() != 1 {
            continue;
        }
        actions.push(Action::ForwardResult { v });
    }
    Some(start_pos != *pos)
}

/// `AddMissingAssignmentsForConversions`: the conversions before `index` without an
/// assignment are assigned from a fresh variable.
fn add_missing_assignments_for_conversions(
    f: &Function,
    d: &Deconstruction,
    conversion_stlocs: &[NodeId],
    conversion_stloc_index: &mut usize,
    previous_index: &mut isize,
    index: usize,
    actions: &mut Vec<Action>,
) {
    while let Some(&stloc) = conversion_stlocs.get(*conversion_stloc_index) {
        let (v, _) = f.match_stloc(stloc).expect("a conversion store");
        let conversion_result_index = d.lookup(v).expect("a conversion output");
        if conversion_result_index >= index {
            break;
        }
        if conversion_result_index as isize > *previous_index {
            actions.push(Action::ForwardConversion { stloc });
        }
        *previous_index = conversion_result_index as isize;
        *conversion_stloc_index += 1;
    }
}

/// `MatchAssignment`: an assignment, or the inline-assign block of a setter whose result is
/// used; the target type, the value and how to add it to the assignments.
fn match_assignment(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId, Action)> {
    if let Some((v, value)) = f.match_stloc(inst)
        && let Some((call, value_inst)) = match_inline_assign_block(f, value)
    {
        let (target_type, _) = is_assignment(f, ctx, call)?;
        let var = f.var(v);
        if !(var.is_single_definition() && var.load_count() == 0) {
            return None;
        }
        return Some((
            target_type,
            value_inst,
            Action::AddInlineAssignCall {
                call,
                value: value_inst,
            },
        ));
    }
    let (target_type, value) = is_assignment(f, ctx, inst)?;
    Some((target_type, value, Action::AddAssignment(inst)))
}

/// `Block.MatchInlineAssignBlock`: `Block CallInlineAssign { call set_P(..., stloc tmp(value));
/// final: ldloc tmp }`; the call and the value.
fn match_inline_assign_block(f: &Function, block: NodeId) -> Option<(NodeId, NodeId)> {
    let Inst::Block {
        kind: BlockKind::CallInlineAssign,
        ref instructions,
        final_inst,
    } = *f.inst(block)
    else {
        return None;
    };
    let &[call] = &instructions[..] else {
        return None;
    };
    let (_, args) = call_parts(f, call)?;
    let &last = args.last()?;
    let (tmp, value) = f.match_stloc(last)?;
    let t = f.var(tmp);
    if !(t.is_single_definition() && t.load_count() == 1) {
        return None;
    }
    (f.match_ldloc(final_inst) == Some(tmp)).then_some((call, value))
}

/// `DeconstructInstruction.IsAssignment`: a setter call, a store to a variable, or a store
/// through a simple target; the target type and the value.
fn is_assignment(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId)> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
            if c.method.method.accessor_kind != 0x01 {
                return None;
            }
            for &arg in &c.args[..c.args.len().checked_sub(1)?] {
                // Integer literals and the like, or a variable.
                if f.flags(arg) != Flags::NONE && f.match_ldloc(arg).is_none() {
                    return None;
                }
            }
            let expected = *c.method.method.params().ok()?.last()?;
            Some((expected, *c.args.last()?))
        }
        &Inst::StLoc { var, value, .. } => Some((f.var(var).ty, value)),
        &Inst::StObj {
            target, value, ty, ..
        } => {
            let mut t = target;
            while let Inst::LdFlda { target: nested, .. } = *f.inst(t) {
                t = nested;
            }
            if f.flags(t) != Flags::NONE && f.match_ldloc(t).is_none() {
                return None;
            }
            // stobj's type comes from the store opcode and is sign-agnostic, so for pointer
            // targets the element type of the target's inferred type counts.
            let inferred = f.infer_type(ctx.ts, target);
            let expected = match *ctx.ts.data(inferred) {
                TypeData::ByRef(e) | TypeData::Pointer(e) => e,
                _ => ty.ty,
            };
            Some((expected, value))
        }
        _ => None,
    }
}

/// `FindIndex`: the flat leaf index of a conversion's or an assignment's value (`ldloc` of a
/// result, or an element read of the tuple node tree); -1 when there is none.
fn find_index(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &mut Deconstruction,
    inst: NodeId,
    actions: &mut Vec<Action>,
) -> isize {
    if let Some(v) = f.match_ldloc(inst) {
        return d.lookup(v).map_or(-1, |i| i as isize);
    }
    let Some((container, container_type, element_index)) = match_tuple_element_read(f, ctx, inst)
    else {
        return -1;
    };
    // A pattern rooted in a Deconstruct call absorbs no tuple element accesses.
    if d.rooted_in_deconstruct_call {
        return -1;
    }
    let Some(node) = resolve_tuple_container(f, ctx, d, container, container_type) else {
        return -1;
    };
    let n = &mut d.tuple_nodes[node];
    // An element bound to a nested designation cannot be read directly as well.
    if element_index >= n.nested_elements.len() || n.nested_elements[element_index].is_some() {
        return -1;
    }
    let flat_index = n.element_flat_index[element_index];
    n.matched_access_count += 1;
    let element_type = n.element_types[element_index];
    if d.results[flat_index].is_none() {
        let stack_type = ctx.ts.stack_type(element_type);
        let fresh = f.add_var(fresh_variable(
            VariableKind::StackSlot,
            element_type,
            stack_type,
            flat_index,
        ));
        actions.push(Action::AddVariable(fresh));
        d.results[flat_index] = Some(fresh);
    }
    actions.push(Action::ReplaceWithResult { inst, flat_index });
    flat_index as isize
}

/// A variable named `E_<index>`.
fn fresh_variable(
    kind: VariableKind,
    ty: TypeId,
    stack_type: StackType,
    index: usize,
) -> crate::il::ast::Variable {
    crate::il::ast::Variable {
        kind,
        ty,
        stack_type,
        index: None,
        name: format!("E_{index}"),
        has_generated_name: false,
        initial_value_is_initialized: false,
        uses_initial_value: false,
        is_ref_read_only: false,
        remove_if_redundant: false,
        state_machine_field: None,
        load_instructions: Vec::new(),
        store_instructions: Vec::new(),
        address_instructions: Vec::new(),
        capture_scope: None,
        is_scoped: false,
        is_scoped_without_initializer: false,
    }
}

/// `IsCompatibleImplicitConversion`: whether the matched conv (or none) lowers the implicit
/// conversion from the input type to the target type.
fn is_compatible_implicit_conversion(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    target_type: TypeId,
    info: ConversionInfo,
) -> bool {
    let input_type = info.input_type.expect("an input type");
    let c = implicit_conversion(ctx, input_type, target_type);
    if !c.is_valid() {
        return false;
    }
    let conv = info.conv.map(|n| match *f.inst(n) {
        Inst::Conv {
            input,
            input_sign,
            target,
            ..
        } => (conversion_kind(target, input, input_sign), input_sign),
        _ => unreachable!("a conv"),
    });
    if c.is_identity() || c.is_reference() {
        conv.is_none_or(|(kind, _)| kind == ConversionKind::Nop)
    } else if c.is_numeric() {
        let Some((kind, input_sign)) = conv else {
            return false;
        };
        let sign = ctx.ts.type_sign(input_type);
        match kind {
            ConversionKind::IntToFloat => sign == input_sign,
            ConversionKind::FloatPrecisionChange => true,
            ConversionKind::SignExtend => sign == Sign::Signed,
            ConversionKind::ZeroExtend => sign == Sign::Unsigned,
            _ => false,
        }
    } else {
        false
    }
}

/// `BuildPatternMatch`: `match.deconstruct[Method](matchVariable = testedOperand) { ... }`.
fn build_pattern_match(
    f: &mut Function,
    call: &DeconstructionCall,
    match_variable: VarId,
    tested_operand: NodeId,
) -> NodeId {
    f.var_mut(match_variable).kind = VariableKind::PatternLocal;
    let mut sub_patterns = Vec::with_capacity(call.results.len());
    for (i, &result) in call.results.iter().enumerate() {
        let load = f.add(Inst::LdLoc {
            var: match_variable,
        });
        match &call.nested_calls[i] {
            Some(nested) => {
                let receiver = nested.receiver.expect("a nested call has a receiver");
                let (ty, stack_type) = (f.var(receiver).ty, f.var(receiver).stack_type);
                let result_inst = deconstruct_result(f, i, ty, stack_type, load);
                sub_patterns.push(build_pattern_match(f, nested, receiver, result_inst));
            }
            None => {
                f.var_mut(result).kind = VariableKind::PatternLocal;
                let (ty, stack_type) = (f.var(result).ty, f.var(result).stack_type);
                let result_inst = deconstruct_result(f, i, ty, stack_type, load);
                sub_patterns.push(new_match(
                    f,
                    result,
                    None,
                    result_inst,
                    Vec::new(),
                    false,
                    false,
                ));
            }
        }
    }
    new_match(
        f,
        match_variable,
        Some(call.method.clone()),
        tested_operand,
        sub_patterns,
        true,
        false,
    )
}

/// `BuildTuplePatternMatch`: `match.tuple(matchVariable = testedOperand) { ... }`; leaf
/// elements without an assignment get a fresh, load-free pattern variable (a discard).
fn build_tuple_pattern_match(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    d: &Deconstruction,
    node: usize,
    match_variable: VarId,
    tested_operand: NodeId,
) -> NodeId {
    f.var_mut(match_variable).kind = VariableKind::PatternLocal;
    let cardinality = d.tuple_nodes[node].element_types.len();
    let mut sub_patterns = Vec::with_capacity(cardinality);
    for i in 0..cardinality {
        let element_type = d.tuple_nodes[node].element_types[i];
        let load = f.add(Inst::LdLoc {
            var: match_variable,
        });
        match d.tuple_nodes[node].nested_elements[i] {
            Some(nested) => {
                // A stack slot temporary's type can be imprecise; a tuple pattern's match
                // variable has the tuple type.
                let nested_var = d.tuple_nodes[nested].variable;
                let nested_var_type = f.var(nested_var).ty;
                if tuple_element_types(ctx, nested_var_type).is_none_or(|e| e.is_empty()) {
                    f.var_mut(nested_var).ty = d.tuple_nodes[nested].ty;
                }
                let stack_type = f.var(nested_var).stack_type;
                let result_inst = deconstruct_result(f, i, element_type, stack_type, load);
                sub_patterns.push(build_tuple_pattern_match(
                    f,
                    ctx,
                    d,
                    nested,
                    nested_var,
                    result_inst,
                ));
            }
            None => {
                let flat_index = d.tuple_nodes[node].element_flat_index[i];
                let result = match d.results[flat_index] {
                    Some(result) => {
                        f.var_mut(result).kind = VariableKind::PatternLocal;
                        result
                    }
                    None => {
                        let stack_type = ctx.ts.stack_type(element_type);
                        let fresh = f.add_var(fresh_variable(
                            VariableKind::PatternLocal,
                            element_type,
                            stack_type,
                            flat_index,
                        ));
                        f.pending_variables.push(fresh);
                        fresh
                    }
                };
                let stack_type = f.var(result).stack_type;
                let result_inst = deconstruct_result(f, i, element_type, stack_type, load);
                sub_patterns.push(new_match(
                    f,
                    result,
                    None,
                    result_inst,
                    Vec::new(),
                    false,
                    false,
                ));
            }
        }
    }
    new_match(
        f,
        match_variable,
        None,
        tested_operand,
        sub_patterns,
        false,
        true,
    )
}

fn deconstruct_result(
    f: &mut Function,
    index: usize,
    ty: TypeId,
    result: StackType,
    arg: NodeId,
) -> NodeId {
    f.add(Inst::DeconstructResult {
        index: index as i32,
        ty,
        result,
        arg,
    })
}

fn new_match(
    f: &mut Function,
    variable: VarId,
    method: Option<MethodOperand>,
    tested_operand: NodeId,
    sub_patterns: Vec<NodeId>,
    is_deconstruct_call: bool,
    is_deconstruct_tuple: bool,
) -> NodeId {
    f.add(Inst::Match {
        variable,
        method,
        tested_operand,
        sub_patterns,
        is_deconstruct_call,
        is_deconstruct_tuple,
        check_type: false,
        check_not_null: false,
    })
}
