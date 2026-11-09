//! `IL/ControlFlow/DetectPinnedRegions.cs`: turns the stores to pinned locals and the blocks
//! they guard into `PinnedRegion` instructions (C# `fixed` statements), and replaces the
//! pinned variable by a native pointer where possible.
//!
//! ```text
//! stloc(P, ldelema(arr, index))           pinned.region P(ldelema(arr, index), {
//! call use(conv ref->i(ldloc P))     =>       call use(ldloc P)
//! stloc(P, conv i4->u(ldc.i4 0))          })
//! ```

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, BranchTarget, ComparisonKind, ContainerKind, ConversionKind,
    Function, Inst, NodeId, NopKind, VarId, Variable, VariableKind,
};
use crate::il::block_builder::topological_sort;
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::transforms::Context;
use crate::il::transforms::il_inlining::{inline_one_if_possible, options as inlining};
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let root = r.function.body.expect("function body");
    let containers: Vec<NodeId> = r
        .function
        .descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(r.function.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for container in containers {
        if !r.function.is_connected(container) {
            continue;
        }
        detect_null_safe_array_to_pointer_or_custom_ref_pin(r, ctx, container);
        split_blocks_at_writes_to_pinned_locals(r, ctx, container);
        for block in r.function.list(container).to_vec() {
            detect_pinned_region(r, ctx, block);
        }
        // Remove the dummy blocks.
        r.function.retain(container, |f, b| !f.list(b).is_empty());
    }
    // Leftover writes to the original pinned locals.
    let f = &mut r.function;
    let blocks: Vec<NodeId> = f
        .descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect();
    for block in blocks {
        if !f.is_connected(block) {
            continue;
        }
        let mut i = 0;
        while i < f.list(block).len() {
            let inst = f.list(block)[i];
            if let Some((v, value)) = f.match_stloc(inst) {
                let var = f.var(v);
                if var.kind == VariableKind::PinnedLocal
                    && var.load_count() == 0
                    && var.address_count() == 0
                {
                    if is_pure(f.flags(value)) {
                        f.remove_at(block, i);
                        continue;
                    }
                    f.replace(inst, value);
                }
            }
            i += 1;
        }
    }
}

fn new_block(f: &mut Function, instructions: Vec<NodeId>) -> NodeId {
    let final_inst = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Block {
        kind: BlockKind::ControlFlow,
        instructions,
        final_inst,
    })
}

fn new_branch(f: &mut Function, target: NodeId) -> NodeId {
    f.add(Inst::Branch {
        target: BranchTarget::Block(target),
    })
}

/// `new Conv(arg, target, checkForOverflow: false, inputSign: Sign.None)`.
fn new_conv(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    arg: NodeId,
    target: PrimitiveType,
) -> NodeId {
    let input = f.result_type(ctx.ts, arg);
    f.add(Inst::Conv {
        is_lifted: false,
        arg,
        input,
        input_sign: Sign::None,
        target,
        check_overflow: false,
    })
}

fn conv_kind(f: &Function, id: NodeId) -> Option<(ConversionKind, NodeId)> {
    match *f.inst(id) {
        Inst::Conv {
            arg,
            input,
            input_sign,
            target,
            ..
        } => Some((
            crate::il::ast::conversion_kind(target, input, input_sign),
            arg,
        )),
        _ => None,
    }
}

/// `UnwrapConv(kind)`.
fn unwrap_conv(f: &Function, mut id: NodeId, kind: ConversionKind) -> NodeId {
    while let Some((k, arg)) = conv_kind(f, id)
        && k == kind
    {
        id = arg;
    }
    id
}

/// `SplitBlocksAtWritesToPinnedLocals`: every write to a pinned local ends its block (it is
/// followed by a branch), and starts one, so the pinned region consists of whole blocks.
fn split_blocks_at_writes_to_pinned_locals(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
) {
    let mut i = 0;
    while i < r.function.list(container).len() {
        let block = r.function.list(container)[i];
        let mut j = 0;
        while j + 1 < r.function.list(block).len() {
            let inst = r.function.list(block)[j];
            let Some((v, value)) = r.function.match_stloc(inst) else {
                j += 1;
                continue;
            };
            if r.function.var(v).kind != VariableKind::PinnedLocal {
                j += 1;
                continue;
            }
            let f = &mut r.function;
            if !matches!(f.inst(f.list(block)[j + 1]), Inst::Branch { .. }) {
                // Split the block after j.
                let rest = f.list(block)[j + 1..].to_vec();
                let new = new_block(f, Vec::new());
                for &k in &rest {
                    f.push(new, k);
                }
                f.add_il_range(new, f.il_range(rest[0]));
                f.truncate(block, j + 1);
                let br = new_branch(f, new);
                f.push(block, br);
                f.insert(container, i + 1, new);
            }
            // Re-pinning (C++/CLI assignment to a pin_ptr) may read the old value: un-inline
            // those reads into a temporary before the write.
            if r.function.is_used_within(v, value) {
                let ty = r.function.var(v).ty;
                let temp = r.register_variable(ctx, VariableKind::StackSlot, ty);
                let f = &mut r.function;
                let load = f.add(Inst::LdLoc { var: v });
                let store = f.add(Inst::StLoc {
                    var: temp,
                    value: load,
                    il_stack_was_empty: false,
                    is_stack_adjustment: false,
                });
                f.insert(block, j, store);
                j += 1;
                for d in f.descendants_post(value) {
                    if f.match_ldloc(d) == Some(v) {
                        let replacement = f.add(Inst::LdLoc { var: temp });
                        f.set_il_range(replacement, f.il_range(d));
                        f.replace(d, replacement);
                    }
                }
            }
            if j > 0 {
                // Split the block before j.
                let f = &mut r.function;
                let moved = f.list(block)[j..j + 2].to_vec();
                let new = new_block(f, Vec::new());
                for &k in &moved {
                    f.push(new, k);
                }
                f.add_il_range(new, f.il_range(moved[0]));
                f.truncate(block, j);
                let br = new_branch(f, new);
                f.push(block, br);
                f.insert(container, i + 1, new);
            }
            j += 1;
        }
        i += 1;
    }
}

/// `DetectNullSafeArrayToPointerOrCustomRefPin`.
fn detect_null_safe_array_to_pointer_or_custom_ref_pin(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
) {
    let mut modified = false;
    let mut i = 0;
    while i < r.function.list(container).len() {
        let block = r.function.list(container)[i];
        i += 1;
        if let Some((v, p, target)) = is_null_safe_array_to_pointer_pattern(&r.function, ctx, block)
            && let TypeData::Array { elem, .. } = *ctx.ts.data(r.function.var(v).ty)
        {
            let f = &mut r.function;
            let load = f.add(Inst::LdLoc { var: v });
            let mut array_to_pointer = f.add(Inst::GetPinnableReference {
                arg: load,
                ty: elem,
                method: None,
            });
            let p_stack = f.var(p).stack_type;
            if p_stack != StackType::Ref {
                array_to_pointer =
                    new_conv(f, ctx, array_to_pointer, p_stack.to_primitive(Sign::None));
            }
            let n = f.list(block).len();
            let old = f.list(block)[n - 2];
            let store = f.add(Inst::StLoc {
                var: p,
                value: array_to_pointer,
                il_stack_was_empty: false,
                is_stack_adjustment: false,
            });
            f.set_il_range(store, f.il_range(old));
            f.replace(old, store);
            let last = *f.list(block).last().expect("branch");
            f.set_branch_target(last, target);
            modified = true;
        } else if let Some(pattern) = is_custom_ref_pin_pattern(&r.function, ctx, block) {
            apply_custom_ref_pin(r, ctx, block, pattern);
            modified = true;
        }
    }
    if modified {
        // Remove the blocks made unreachable.
        r.function
            .retain(container, |f, b| f.node(b).incoming_edge_count != 0);
    }
}

struct CustomRefPin {
    ldloc_mem: NodeId,
    call_gpr: NodeId,
    v: VarId,
    ptr_assign: Option<NodeId>,
    target: NodeId,
    null_block: Option<NodeId>,
    not_null_block: NodeId,
}

fn apply_custom_ref_pin(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    p: CustomRefPin,
) {
    let f = &mut r.function;
    let method = match f.inst(p.call_gpr) {
        Inst::Call(c) | Inst::CallVirt(c) => c.method.clone(),
        other => panic!("{other:?} is not a call"),
    };
    let gpr = match *ctx.ts.data(method.method.ret) {
        TypeData::ByRef(elem) if ctx.settings.pattern_based_fixed_statement => {
            f.add(Inst::GetPinnableReference {
                arg: p.ldloc_mem,
                ty: elem,
                method: Some(method.clone()),
            })
        }
        _ => {
            let null = f.add(Inst::LdNull);
            let condition = f.add(Inst::Comp {
                lifting: crate::il::ast::ComparisonLiftingKind::None,
                kind: ComparisonKind::Inequality,
                sign: Sign::None,
                input: StackType::Obj,
                left: p.ldloc_mem,
                right: null,
            });
            let zero = f.add(Inst::LdcI4 { value: 0 });
            let null_ref = new_conv(f, ctx, zero, PrimitiveType::Ref);
            f.add(Inst::If {
                condition,
                true_inst: p.call_gpr,
                false_inst: null_ref,
                csharp_type: Some(method.method.ret),
            })
        }
    };
    let n = f.list(block).len();
    let old = f.list(block)[n - 2];
    let store = f.add(Inst::StLoc {
        var: p.v,
        value: gpr,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.set_il_range(store, f.il_range(old));
    f.replace(old, store);
    if let Some(ptr) = p.ptr_assign {
        let n = f.list(block).len();
        f.insert(block, n - 1, ptr);
    }
    let last = *f.list(block).last().expect("branch");
    f.set_branch_target(last, p.target);
    // Clear the now unreachable inner blocks, so the target's incoming count is accurate.
    if let Some(null_block) = p.null_block {
        f.truncate(null_block, 0);
    }
    f.truncate(p.not_null_block, 0);
    if f.node(p.target).incoming_edge_count == 1 && f.node(p.target).parent == f.node(block).parent
    {
        let n = f.list(block).len();
        f.remove_at(block, n - 1);
        for inst in f.list(p.target).to_vec() {
            f.push(block, inst);
        }
        f.truncate(p.target, 0);
        if let Some(ptr) = p.ptr_assign {
            let index = f.node(ptr).child_index as usize;
            inline_one_if_possible(f, block, index, inlining::NONE, ctx);
        }
    }
}

/// `IsCustomRefPinPattern`:
/// ```text
///     if (comp.o(ldloc mem != ldnull)) br notNullBlock
///     br nullBlock
/// Block nullBlock (incoming: 1) { stloc ptr(conv i4->u (ldc.i4 0)); br targetBlock }
/// Block notNullBlock (incoming: 1) {
///     stloc V_1(call GetPinnableReference(ldloc mem))
///     stloc ptr(conv ref->u (ldloc V_1))
///     br targetBlock
/// }
/// ```
fn is_custom_ref_pin_pattern(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
) -> Option<CustomRefPin> {
    let (condition, mut true_inst, mut false_inst) = f.match_if_at_end_of_block(block)?;
    let ldloc_mem = match f.match_comp_null(condition, ComparisonKind::Inequality) {
        Some(m) => m,
        None => {
            let m = f.match_comp_null(condition, ComparisonKind::Equality)?;
            std::mem::swap(&mut true_inst, &mut false_inst);
            m
        }
    };
    if !is_pure(f.flags(ldloc_mem)) {
        return None;
    }
    let parent = f.node(block).parent;
    let not_null_block = f
        .match_branch(true_inst)
        .filter(|&b| f.node(b).parent == parent)?;
    let null_block = f
        .match_branch(false_inst)
        .filter(|&b| f.node(b).parent == parent)?;
    if f.node(not_null_block).incoming_edge_count != 1 || f.list(not_null_block).len() < 2 {
        return None;
    }
    let inner = f.list(not_null_block);
    let (v, value) = f.match_stloc(inner[0])?;
    if f.var(v).kind != VariableKind::PinnedLocal {
        return None;
    }
    let args = match f.inst(value) {
        Inst::Call(c) | Inst::CallVirt(c) if c.method.method.name == "GetPinnableReference" => {
            &c.args
        }
        _ => return None,
    };
    if args.len() != 1 || !f.structurally_equal(ldloc_mem, args[0]) {
        return None;
    }
    let ptr_assign = f.match_stloc(inner[1]).map(|_| inner[1]);
    let target;
    let mut null_block = Some(null_block);
    if let Some(ptr) = ptr_assign {
        let (ptr_var, ptr_value) = f.match_stloc(ptr)?;
        let unwrapped = unwrap_conv(f, ptr_value, ConversionKind::StopGCTracking);
        if f.match_ldloc(unwrapped) != Some(v) {
            return None;
        }
        target = f.match_branch(*inner.get(2)?)?;
        let nb = null_block.expect("null block");
        let null_list = f.list(nb);
        if f.node(nb).incoming_edge_count != 1 || null_list.len() != 2 {
            return None;
        }
        let (stored, null_value) = f.match_stloc(null_list[0])?;
        if stored != ptr_var || f.match_ldc_i(null_value) != Some(0) {
            return None;
        }
        if f.match_branch(null_list[1]) != Some(target) {
            return None;
        }
    } else {
        target = f.match_branch(inner[1])?;
        if Some(target) != null_block {
            return None;
        }
        // Not cleared by the caller: it is the target.
        null_block = None;
    }
    let _ = ctx;
    Some(CustomRefPin {
        ldloc_mem,
        call_gpr: value,
        v,
        ptr_assign,
        target,
        null_block,
        not_null_block,
    })
}

/// `IsNullSafeArrayToPointerPattern`:
/// ```text
///   stloc V(ldloc S)
///   if (comp(ldloc S == ldnull)) br B_null_or_empty
///   br B_not_null
/// Block B_not_null { if (conv i->i4 (ldlen(ldloc V))) br B_not_null_and_not_empty; br B_null_or_empty }
/// Block B_not_null_and_not_empty { stloc P(ldelema(ldloc V, ldc.i4 0, ...)); br B_target }
/// Block B_null_or_empty { stloc P(conv i4->u(ldc.i4 0)); br B_target }
/// ```
/// becomes `stloc P(array.to.pointer(V)); br B_target`.
fn is_null_safe_array_to_pointer_pattern(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
) -> Option<(VarId, VarId, NodeId)> {
    let list = f.list(block);
    if list.len() < 2 {
        return None;
    }
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(list[list.len() - 2])
    else {
        return None;
    };
    let Inst::Comp {
        kind: ComparisonKind::Equality,
        left,
        right,
        ..
    } = *f.inst(condition)
    else {
        return None;
    };
    let mut v = f.match_ldloc(left)?;
    if !matches!(f.inst(right), Inst::LdNull) {
        return None;
    }
    let mut using_previous_var = false;
    if f.var(v).kind == VariableKind::StackSlot
        && list.len() >= 3
        && let Some((previous, value)) = f.match_stloc(list[list.len() - 3])
        && f.match_ldloc(value) == Some(v)
    {
        // An inline assignment: `stloc V(ldloc S); if (comp(ldloc S == ldnull)) ...`.
        v = previous;
        using_previous_var = true;
    }
    let null_or_empty = f.match_branch(true_inst)?;
    if !f.match_nop(false_inst) || f.node(null_or_empty).parent != f.node(block).parent {
        return None;
    }
    let (p, target) = is_null_safe_array_to_pointer_null_or_empty_block(f, null_or_empty)?;
    if !(f.var(p).kind == VariableKind::PinnedLocal
        || (using_previous_var && f.var(v).kind == VariableKind::PinnedLocal))
    {
        return None;
    }
    let not_null = f.match_branch(*list.last()?)?;
    if f.node(not_null).parent != f.node(block).parent {
        return None;
    }
    is_null_safe_array_to_pointer_not_null_block(f, ctx, not_null, v, p, null_or_empty, target)
        .then_some((v, p, target))
}

fn is_null_safe_array_to_pointer_not_null_block(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    v: VarId,
    p: VarId,
    null_or_empty: NodeId,
    target: NodeId,
) -> bool {
    let list = f.list(block);
    if list.len() != 2 {
        return false;
    }
    let Some((mut condition, mut true_inst)) = f.match_if_instruction(list[0]) else {
        return false;
    };
    let mut false_inst = list[1];
    if let Inst::Comp {
        kind, left, right, ..
    } = *f.inst(condition)
        && f.match_ldc_i(right) == Some(0)
    {
        match kind {
            // `len == 0` negates the condition.
            ComparisonKind::Equality => {
                condition = left;
                std::mem::swap(&mut true_inst, &mut false_inst);
            }
            // `len != 0` is the implicit non-zero test.
            ComparisonKind::Inequality => condition = left,
            _ => return false,
        }
    }
    let condition = unwrap_conv(f, condition, ConversionKind::Truncate);
    let array = match f.inst(condition) {
        Inst::LdLen {
            array,
            result: StackType::I,
        } => *array,
        // Multi-dimensional arrays use `Array.get_Length`.
        Inst::Call(c) | Inst::CallVirt(c) if c.method.method.name == "get_Length" => {
            if c.args.len() != 1
                || !c
                    .method
                    .method
                    .declaring
                    .is_some_and(|d| known_of(ctx, d) == Some(KnownTypeCode::Array))
            {
                return false;
            }
            c.args[0]
        }
        _ => return false,
    };
    if f.match_ldloc(array) != Some(v) {
        return false;
    }
    let Some(inner) = f.match_branch(true_inst) else {
        return false;
    };
    if f.node(inner).parent != f.node(block).parent
        || !is_null_safe_array_to_pointer_not_null_and_not_empty_block(f, inner, v, p, target)
    {
        return false;
    }
    f.match_branch(false_inst) == Some(null_or_empty)
}

fn is_null_safe_array_to_pointer_not_null_and_not_empty_block(
    f: &Function,
    block: NodeId,
    v: VarId,
    p: VarId,
    target: NodeId,
) -> bool {
    let list = f.list(block);
    if list.len() != 2 {
        return false;
    }
    let Some((p2, mut value)) = f.match_stloc(list[0]) else {
        return false;
    };
    if p != p2 {
        // An unused pointer variable may have been split.
        let (a, b) = (f.var(p), f.var(p2));
        let unused = a.load_count() == 0
            && a.address_count() == 0
            && b.load_count() == 0
            && b.address_count() == 0;
        if !unused || !variables_equal(a, b) {
            return false;
        }
    }
    if f.var(v).kind == VariableKind::PinnedLocal {
        value = unwrap_conv(f, value, ConversionKind::StopGCTracking);
    }
    let Inst::LdElema { array, indices, .. } = f.inst(value) else {
        return false;
    };
    f.match_ldloc(*array) == Some(v)
        && indices.iter().all(|&i| f.match_ldc_i4(i) == Some(0))
        && f.match_branch(list[1]) == Some(target)
}

/// `ILVariableEqualityComparer.Equals` for two variables of one function.
fn variables_equal(x: &Variable, y: &Variable) -> bool {
    if x.kind == VariableKind::StackSlot || y.kind == VariableKind::StackSlot {
        return false;
    }
    if x.kind == VariableKind::PatternLocal || y.kind == VariableKind::PatternLocal {
        return false;
    }
    x.kind == y.kind && x.index.is_some() && x.index == y.index
}

fn is_null_safe_array_to_pointer_null_or_empty_block(
    f: &Function,
    block: NodeId,
) -> Option<(VarId, NodeId)> {
    let list = f.list(block);
    if list.len() != 2 {
        return None;
    }
    let (p, value) = f.match_stloc(list[0])?;
    if !matches!(
        f.var(p).kind,
        VariableKind::PinnedLocal | VariableKind::Local
    ) || !is_null_or_zero(f, value)
    {
        return None;
    }
    Some((p, f.match_branch(list[1])?))
}

fn is_null_or_zero(f: &Function, mut inst: NodeId) -> bool {
    while let Inst::Conv { arg, .. } = *f.inst(inst) {
        inst = arg;
    }
    f.match_ldc_i4(inst) == Some(0) || matches!(f.inst(inst), Inst::LdNull)
}

/// `DetectPinnedRegion`: only the second-to-last instruction of a block can still be a write
/// to a pinned local (after the block splitting); one that pins starts a region.
fn detect_pinned_region(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
) -> bool {
    let f = &r.function;
    let list = f.list(block);
    if list.len() < 2 {
        return false;
    }
    let st_loc = list[list.len() - 2];
    let Some((v, value)) = f.match_stloc(st_loc) else {
        return false;
    };
    if f.var(v).kind != VariableKind::PinnedLocal || is_null_or_zero(f, value) {
        return false;
    }
    // `pinned` has no effect on value types (#2148).
    let ty = f.var(v).ty;
    if ctx.ts.is_reference_type(ty) == Some(false) {
        return false;
    }
    create_pinned_region(r, ctx, block, st_loc)
}

fn create_pinned_region(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    st_loc: NodeId,
) -> bool {
    let f = &mut r.function;
    let (v, init) = f.match_stloc(st_loc).expect("stloc");
    let source = f.node(block).parent.expect("block in a container");
    let blocks = f.list(source).to_vec();
    let mut reached = vec![0u32; blocks.len()];
    let entry = f.branch_target(*f.list(block).last().expect("branch"));
    if f.node(entry).parent != Some(source) {
        return false; // no block to add to the region
    }
    let mut work_list = std::collections::VecDeque::new();
    let starts_with_unpin = |f: &Function, b: NodeId| {
        f.list(b)
            .first()
            .is_some_and(|&i| f.match_stloc(i).is_some_and(|(sv, _)| sv == v))
    };
    if !starts_with_unpin(f, entry) {
        reached[f.node(entry).child_index as usize] += 1;
        work_list.push_back(entry);
    }
    while let Some(item) = work_list.pop_front() {
        for branch in f.descendants_post(item) {
            let Some(target) = f.match_branch(branch) else {
                continue;
            };
            if f.node(target).parent != Some(source) || starts_with_unpin(f, target) {
                continue;
            }
            let i = f.node(target).child_index as usize;
            reached[i] += 1;
            if reached[i] == 1 {
                work_list.push_back(target);
            }
        }
    }
    // A block used both inside and outside the region gets duplicated.
    let clone_blocks = (0..blocks.len())
        .any(|i| reached[i] != 0 && reached[i] != f.node(blocks[i]).incoming_edge_count);
    let body = f.add(Inst::BlockContainer {
        kind: ContainerKind::Normal,
        expected: None,
        blocks: Vec::new(),
    });
    let mut cloned: Vec<Option<NodeId>> = vec![None; blocks.len()];
    for i in 0..blocks.len() {
        if reached[i] == 0 {
            continue;
        }
        let original = f.list(source)[i];
        let inner = if clone_blocks {
            let c = f.clone_subtree(original);
            cloned[i] = Some(c);
            c
        } else {
            original
        };
        if let Some((_, true_inst, false_inst)) = f.match_if_at_end_of_block(inner) {
            handle_branch_leaving_pinned_region(f, true_inst, &reached, source, v);
            handle_branch_leaving_pinned_region(f, false_inst, &reached, source, v);
        } else if let Some(&last) = f.list(inner).last() {
            handle_branch_leaving_pinned_region(f, last, &reached, source, v);
        }
        if !clone_blocks {
            // A dummy block keeps the indices; dummies are deleted later.
            let dummy = new_block(f, Vec::new());
            let mut list = f.list(source).to_vec();
            list[i] = dummy;
            f.set_list(source, list);
        }
        if original == entry {
            f.insert(body, 0, inner);
        } else {
            f.push(body, inner);
        }
    }
    if f.list(body).is_empty() {
        // The entry block itself does not belong into the region.
        let br = new_branch(f, entry);
        let body_block = new_block(f, vec![br]);
        f.set_il_range(body_block, f.il_range(st_loc));
        f.push(body, body_block);
    }
    let region = f.add(Inst::PinnedRegion {
        variable: v,
        init,
        body,
    });
    f.set_il_range(region, f.il_range(st_loc));
    f.replace(st_loc, region);
    let n = f.list(block).len();
    f.remove_at(block, n - 1); // the branch into the body
    if clone_blocks {
        // Branches between cloned blocks go to the clones.
        for branch in f.descendants_post(body) {
            if let Some(target) = f.match_branch(branch)
                && f.node(target).parent == Some(source)
                && let Some(c) = cloned[f.node(target).child_index as usize]
            {
                f.set_branch_target(branch, c);
            }
        }
        // Unreachable blocks of the source container become dummies.
        let mut alive = vec![false; f.list(source).len()];
        let mut duplicated = Vec::new();
        for remaining in topological_sort(f, source) {
            let i = f.node(remaining).child_index as usize;
            alive[i] = true;
            if cloned[i].is_some() {
                duplicated.push(f.il_range(remaining).start);
            }
        }
        for (i, &is_alive) in alive.iter().enumerate() {
            if !is_alive {
                let dummy = new_block(f, Vec::new());
                let mut list = f.list(source).to_vec();
                list[i] = dummy;
                f.set_list(source, list);
            }
        }
        duplicated.sort_unstable();
        let offsets: Vec<String> = duplicated.iter().map(|o| format!("IL_{o:04x}")).collect();
        r.warnings.push(format!(
            "The blocks {} are reachable both inside and outside the pinned region starting at IL_{:04x}. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.",
            offsets.join(", "),
            r.function.il_range(region).start
        ));
    }
    process_pinned_region(r, ctx, region);
    true
}

fn handle_branch_leaving_pinned_region(
    f: &mut Function,
    potential_branch: NodeId,
    reached: &[u32],
    source: NodeId,
    pinned_var: VarId,
) {
    let Some(target) = f.match_branch(potential_branch) else {
        return;
    };
    if f.node(target).incoming_edge_count != 1
        || f.node(target).parent != Some(source)
        || reached[f.node(target).child_index as usize] != 0
    {
        return;
    }
    // The branch leaves the body: drop the unpin at its target.
    if let Some(&first) = f.list(target).first()
        && let Some((var, value)) = f.match_stloc(first)
        && var == pinned_var
        && is_null_or_zero(f, value)
    {
        f.remove_at(target, 0);
    }
}

fn region_parts(f: &Function, region: NodeId) -> (VarId, NodeId, NodeId) {
    match *f.inst(region) {
        Inst::PinnedRegion {
            variable,
            init,
            body,
        } => (variable, init, body),
        ref other => panic!("{other:?} is not a pinned region"),
    }
}

fn new_pinned_region_local(
    r: &mut ReadFunction,
    from: VarId,
    ty: TypeId,
    ctx: &mut Context<'_, '_, '_>,
) -> VarId {
    let o = r.function.var(from).clone();
    let stack_type = ctx.ts.stack_type(ty);
    let v = r.function.add_var(Variable {
        kind: VariableKind::PinnedRegionLocal,
        ty,
        stack_type,
        index: o.index,
        name: o.name,
        has_generated_name: o.has_generated_name,
        initial_value_is_initialized: false,
        uses_initial_value: false,
        is_ref_read_only: false,
        remove_if_redundant: false,
        state_machine_field: None,
        load_instructions: Vec::new(),
        store_instructions: Vec::new(),
        address_instructions: Vec::new(),
    });
    r.variables.push(v);
    v
}

/// `ProcessPinnedRegion`: replaces the pinned variable by a native pointer as far as
/// possible, then detects nested regions.
fn process_pinned_region(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, region: NodeId) {
    let (v, init, _) = region_parts(&r.function, region);
    let ty = r.function.var(v).ty;
    match ctx.ts.data(ty).clone() {
        TypeData::ByRef(mut elem) => {
            // C# has no by-reference variables: use a native pointer.
            if let TypeData::Pointer(pointee) = *ctx.ts.data(elem)
                && let Inst::LdFlda { field, .. } = r.function.inst(init)
                && field.field.ty == pointee
            {
                // Roslyn 2.6 (C# 7.2) pins a fixed field of type `int` as `int*&`.
                elem = pointee;
            }
            let pointer = ctx.ts.intern(TypeData::Pointer(elem));
            let new = new_pinned_region_local(r, v, pointer, ctx);
            replace_pinned_var(r, ctx, v, new, region);
            use_existing_variable_for_pinned_region(&mut r.function, ctx, region);
        }
        TypeData::Array { .. } => {
            move_array_to_pointer_to_pinned_region_init(r, ctx, region);
            use_existing_variable_for_pinned_region(&mut r.function, ctx, region);
        }
        _ if known_of(ctx, ty) == Some(KnownTypeCode::String) => {
            handle_string_to_pointer(r, ctx, region);
        }
        _ => {
            if let Some((ConversionKind::StopGCTracking, arg)) = conv_kind(&r.function, init) {
                // The variable already was a pointer; the conversion is not needed.
                r.function.replace(init, arg);
            }
        }
    }
    // Nested regions.
    let (_, _, body) = region_parts(&r.function, region);
    for block in r.function.list(body).to_vec() {
        detect_pinned_region(r, ctx, block);
    }
    let f = &mut r.function;
    f.retain(body, |f, b| !f.list(b).is_empty());
    let entry = f.list(body)[0];
    f.set_il_range(body, f.il_range(entry));
    let (v, _, _) = region_parts(f, region);
    if f.var(v).kind != VariableKind::PinnedRegionLocal {
        f.var_mut(v).kind = VariableKind::PinnedRegionLocal;
    }
}

fn known_of(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> Option<KnownTypeCode> {
    ctx.ts
        .definition(ty)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .map(|i| i.known)
}

/// `MoveArrayToPointerToPinnedRegionInit`: Roslyn pins the array variable and takes the
/// pointer right inside the region; the pointer becomes the region's variable.
fn move_array_to_pointer_to_pinned_region_init(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    region: NodeId,
) {
    let f = &r.function;
    let (v, init, _) = region_parts(f, region);
    // The single access of the variable inside the region must be an `ldloc`.
    let mut ldloc = None;
    for inst in f.descendants_post(region) {
        if inst == region || f.variable_of(inst) != Some(v) {
            continue;
        }
        if ldloc.is_some() || !matches!(f.inst(inst), Inst::LdLoc { .. }) {
            return;
        }
        ldloc = Some(inst);
    }
    let Some(ldloc) = ldloc else {
        return;
    };
    let Some(array_to_pointer) = f.node(ldloc).parent else {
        return;
    };
    let Inst::GetPinnableReference { ty, method, .. } = f.inst(array_to_pointer).clone() else {
        return;
    };
    let Some(conv) = f.node(array_to_pointer).parent else {
        return;
    };
    if !matches!(
        conv_kind(f, conv),
        Some((ConversionKind::StopGCTracking, _))
    ) {
        return;
    }
    let TypeData::Array { elem, .. } = *ctx.ts.data(f.var(v).ty) else {
        return;
    };
    let pointer = ctx.ts.intern(TypeData::Pointer(elem));
    let new = new_pinned_region_local(r, v, pointer, ctx);
    let f = &mut r.function;
    f.set_variable(region, new);
    let range = f.il_range(array_to_pointer);
    // The old init moves into the new instruction: detach it from the region first.
    let placeholder = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.replace(init, placeholder);
    let new_init = f.add(Inst::GetPinnableReference {
        arg: init,
        ty,
        method,
    });
    f.set_il_range(new_init, range);
    f.replace(placeholder, new_init);
    let load = f.add(Inst::LdLoc { var: new });
    f.set_il_range(load, f.il_range(conv));
    f.replace(conv, load);
}

/// `ReplacePinnedVar`.
fn replace_pinned_var(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    old: VarId,
    new: VarId,
    inst: NodeId,
) {
    let f = &mut r.function;
    let old_stack = f.var(old).stack_type;
    let new_stack = f.var(new).stack_type;
    if let Some((ConversionKind::StopGCTracking, arg)) = conv_kind(f, inst)
        && f.match_ldloc(arg) == Some(old)
        && f.result_type(ctx.ts, inst) == new_stack
    {
        // `conv ref->i (ldloc old)` becomes `ldloc new`.
        f.add_il_range(inst, f.il_range(arg));
        let load = f.add(Inst::LdLoc { var: new });
        f.set_il_range(load, f.il_range(inst));
        f.replace(inst, load);
        return;
    }
    if f.variable_of(inst) == Some(old) {
        f.set_variable(inst, new);
        let old_is_byref = matches!(ctx.ts.data(f.var(old).ty), TypeData::ByRef(_));
        if let Some((_, value)) = f.match_stloc(inst)
            && old_is_byref
        {
            let placeholder = f.add(Inst::Nop {
                kind: NopKind::Normal,
            });
            f.replace(value, placeholder);
            let conv = new_conv(f, ctx, value, PrimitiveType::I);
            f.replace(placeholder, conv);
        }
        if matches!(f.inst(inst), Inst::LdLoc { .. } | Inst::StLoc { .. })
            && !is_slot_accepting_both_managed_and_unmanaged_pointers(f, inst)
            && old_stack != StackType::I
        {
            // Wrap the instruction so the stack types match up.
            let placeholder = f.add(Inst::Nop {
                kind: NopKind::Normal,
            });
            f.replace(inst, placeholder);
            let conv = new_conv(f, ctx, inst, old_stack.to_primitive(Sign::None));
            f.replace(placeholder, conv);
        }
    } else if let Inst::LdStr { value } = f.inst(inst)
        && String::from_utf16_lossy(value) == "Is this ILSpy?"
    {
        let reply = f.add(Inst::LdStr {
            value: "This is ILSpy!".encode_utf16().collect(),
        });
        f.replace(inst, reply);
        return;
    }
    for child in r.function.inst(inst).children() {
        replace_pinned_var(r, ctx, old, new, child);
    }
}

/// `IsSlotAcceptingBothManagedAndUnmanagedPointers`: a block instruction, or the target of
/// `ldobj` / `stobj`.
fn is_slot_accepting_both_managed_and_unmanaged_pointers(f: &Function, inst: NodeId) -> bool {
    let Some(parent) = f.node(inst).parent else {
        return false;
    };
    match f.inst(parent) {
        Inst::Block { instructions, .. } => instructions.contains(&inst),
        Inst::LdObj { target, .. } | Inst::StObj { target, .. } => *target == inst,
        _ => false,
    }
}

/// `IsBranchOnNull`: `if (comp(ldloc nativeVar == 0)) br targetBlock`.
fn is_branch_on_null(f: &Function, cond_branch: NodeId, native: VarId) -> Option<NodeId> {
    let (condition, true_inst) = f.match_if_instruction(cond_branch)?;
    let (left, right) = f.match_comp(condition, ComparisonKind::Equality)?;
    if f.match_ldloc(left) != Some(native) || !is_null_or_zero(f, right) {
        return None;
    }
    f.match_branch(true_inst)
}

/// `HandleStringToPointer`: `fixed (char* p = str)`.
fn handle_string_to_pointer(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, region: NodeId) {
    let (pinned, init, body) = region_parts(&r.function, region);
    let entry = r.function.list(body)[0];
    if r.function.node(entry).incoming_edge_count != 1 {
        return;
    }
    let char_type = ctx.ts.known_type(KnownTypeCode::Char);
    let first = r.function.list(entry)[0];
    let Some((mut native, init_inst)) = r.function.match_stloc(first) else {
        // Legacy csc with an unused pinned variable.
        let p = r.function.var(pinned);
        if p.address_count() == 0 && p.load_count() == 0 {
            let char_ptr = ctx.ts.intern(TypeData::Pointer(char_type));
            let new = new_pinned_region_local(r, pinned, char_ptr, ctx);
            let f = &mut r.function;
            f.set_variable(region, new);
            wrap_init_in_get_pinnable_reference(f, region, init, char_type);
        }
        return;
    };
    if ctx.ts.stack_type(r.function.var(native).ty) != StackType::I {
        return;
    }
    let f = &r.function;
    let entry_len = f.list(entry).len();
    let target;
    let mut adjust: Option<NodeId> = None;
    if entry_len == 2 {
        let Inst::BinaryNumeric {
            op: BinaryNumericOperator::Add,
            left,
            right,
            ..
        } = *f.inst(init_inst)
        else {
            return;
        };
        if f.match_ldloc(unwrap_conv(f, left, ConversionKind::StopGCTracking)) != Some(pinned)
            || !is_offset_to_string_data_call(f, ctx, right)
        {
            return;
        }
        let Some(t) = f.match_branch(f.list(entry)[1]) else {
            return;
        };
        target = t;
    } else if entry_len == 3 {
        if f.match_ldloc(unwrap_conv(f, init_inst, ConversionKind::StopGCTracking)) != Some(pinned)
        {
            return;
        }
        let Some(t) = is_branch_on_null(f, f.list(entry)[1], native) else {
            return;
        };
        let Some(a) = f.match_branch(f.list(entry)[2]) else {
            return;
        };
        if !(f.node(a).parent == Some(body)
            && f.node(a).incoming_edge_count == 1
            && is_offset_to_string_data_block(f, ctx, a, native, t))
        {
            return;
        }
        target = t;
        adjust = Some(a);
    } else {
        return;
    }
    let f = &mut r.function;
    if f.node(target).parent == Some(body) {
        // The target becomes the new entry point.
        f.remove_at(body, 0);
        if let Some(a) = adjust {
            let i = f.node(a).child_index as usize;
            f.remove_at(body, i);
        }
        let i = f.node(target).child_index as usize;
        f.remove_at(body, i);
        f.insert(body, 0, target);
    } else {
        // Empty body: jump straight to the target outside.
        let first_block = f.list(body)[0];
        f.truncate(first_block, 0);
        f.truncate(body, 1);
        let br = new_branch(f, target);
        f.push(first_block, br);
    }
    let (_, init, _) = region_parts(f, region);
    wrap_init_in_get_pinnable_reference(f, region, init, char_type);
    // Optimized builds may keep the native pointer in a stack slot until after the offset
    // adjustment.
    let entry = f.list(body)[0];
    if f.var(native).kind == VariableKind::StackSlot
        && f.var(native).load_count() == 1
        && let Some(&first) = f.list(entry).first()
        && let Some((other, other_init)) = f.match_stloc(first)
        && f.match_ldloc(other_init) == Some(native)
        && f.var(other).is_single_definition()
    {
        f.remove_at(entry, 0);
        native = other;
    }
    let new = if r.function.var(native).kind == VariableKind::Local {
        let ty = r.function.var(native).ty;
        let new = new_pinned_region_local(r, native, ty, ctx);
        replace_pinned_var(r, ctx, native, new, region);
        new
    } else {
        native
    };
    replace_pinned_var(r, ctx, pinned, new, region);
}

/// `pinnedRegion.Init = new GetPinnableReference(pinnedRegion.Init, ty, null)`.
fn wrap_init_in_get_pinnable_reference(f: &mut Function, region: NodeId, init: NodeId, ty: TypeId) {
    let placeholder = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.replace(init, placeholder);
    let new_init = f.add(Inst::GetPinnableReference {
        arg: init,
        ty,
        method: None,
    });
    f.replace(placeholder, new_init);
    let _ = region;
}

/// `IsOffsetToStringDataBlock`:
/// `stloc nativeVar(add(ldloc nativeVar, conv i4->i (call get_OffsetToStringData()))); br target`.
fn is_offset_to_string_data_block(
    f: &Function,
    ctx: &Context<'_, '_, '_>,
    block: NodeId,
    native: VarId,
    target: NodeId,
) -> bool {
    let list = f.list(block);
    if list.len() != 2 {
        return false;
    }
    let nv = f.var(native);
    let value = if nv.is_single_definition() && nv.load_count() == 2 {
        // With no other loads, the variable may have been split.
        let Some((other, value)) = f.match_stloc(list[0]) else {
            return false;
        };
        let o = f.var(other);
        if !(o.is_single_definition() && o.load_count() == 0) {
            return false;
        }
        value
    } else if nv.store_count() == 2 {
        let Some((stored, value)) = f.match_stloc(list[0]) else {
            return false;
        };
        if stored != native {
            return false;
        }
        value
    } else {
        return false;
    };
    let Inst::BinaryNumeric {
        op: BinaryNumericOperator::Add,
        left,
        right,
        ..
    } = *f.inst(value)
    else {
        return false;
    };
    f.match_ldloc(left) == Some(native)
        && is_offset_to_string_data_call(f, ctx, right)
        && f.match_branch(list[1]) == Some(target)
}

fn is_offset_to_string_data_call(f: &Function, ctx: &Context<'_, '_, '_>, inst: NodeId) -> bool {
    let inst = unwrap_conv(f, inst, ConversionKind::SignExtend);
    matches!(f.inst(inst), Inst::Call(c) if c.method.method.name == "get_OffsetToStringData"
        && c.method.method.declaring.is_some_and(|d| ctx.ts.full_name(d) == "System.Runtime.CompilerServices.RuntimeHelpers"))
}

/// `UseExistingVariableForPinnedRegion`: Roslyn copies the pinned variable into a local at
/// the start of the body; that local becomes the region's variable.
fn use_existing_variable_for_pinned_region(
    f: &mut Function,
    _ctx: &mut Context<'_, '_, '_>,
    region: NodeId,
) {
    let (pinned, _, body) = region_parts(f, region);
    if !matches!(f.inst(body), Inst::BlockContainer { .. }) || f.var(pinned).load_count() != 1 {
        return;
    }
    let entry = f.list(body)[0];
    let Some(&first) = f.list(entry).first() else {
        return;
    };
    let Some((v, init)) = f.match_stloc(first) else {
        return;
    };
    if f.match_ldloc(init) != Some(pinned) {
        return;
    }
    let var = f.var(v);
    if !(var.is_single_definition() && var.ty == f.var(pinned).ty)
        || var.kind != VariableKind::Local
    {
        return;
    }
    f.var_mut(v).kind = VariableKind::PinnedRegionLocal;
    f.set_variable(region, v);
    f.remove_at(entry, 0);
}
