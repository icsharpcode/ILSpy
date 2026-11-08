//! `IL/Transforms/ILInlining.cs`: moves the value of a single-use `stloc` into the one
//! instruction after it that loads the variable, where evaluation order allows it.

use crate::il::ast::{BlockKind, ComparisonKind, Function, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::{is_pure, may_reorder};
use crate::il::types::StackType;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeId;

use super::Context;
use super::split_variables::remove_dead_variables;

/// `InliningOptions`.
pub mod options {
    pub const NONE: u32 = 0;
    pub const AGGRESSIVE: u32 = 1;
    pub const INTRODUCE_NAMED_ARGUMENTS: u32 = 2;
    pub const FIND_DECONSTRUCTION: u32 = 4;
    pub const ALLOW_CHANGING_ORDER_OF_EVALUATION_FOR_EXCEPTIONS: u32 = 8;
    pub const ALLOW_INLINING_OF_LDLOCA: u32 = 0x10;
}

#[derive(Clone, Copy, Debug, Default)]
pub struct ILInlining {
    pub options: u32,
}

impl ILInlining {
    /// The top-level transform: every block of the function, then `RemoveDead`.
    pub fn run(&self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        let f = &mut r.function;
        let root = f.body.expect("function body");
        let blocks: Vec<NodeId> = f
            .descendants_post(root)
            .into_iter()
            .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
            .collect();
        for block in blocks {
            if f.is_connected(block) {
                inline_all_in_block(f, block, self.options, ctx);
            }
        }
        remove_dead_variables(r);
    }
}

/// `InlineAllInBlock`: from the last instruction backwards, inlines every `stloc` it can.
pub fn inline_all_in_block(
    f: &mut Function,
    block: NodeId,
    options: u32,
    ctx: &mut Context<'_, '_, '_>,
) -> bool {
    let mut modified = false;
    let mut i = f.list(block).len();
    while i > 0 {
        i -= 1;
        if matches!(f.inst(f.list(block)[i]), Inst::StLoc { .. })
            && inline_one_if_possible(f, block, i, options, ctx)
        {
            modified = true;
        }
    }
    modified
}

/// `InlineOneIfPossible`.
pub fn inline_one_if_possible(
    f: &mut Function,
    block: NodeId,
    pos: usize,
    options: u32,
    ctx: &mut Context<'_, '_, '_>,
) -> bool {
    let stloc = f.list(block)[pos];
    let Some((v, _)) = f.match_stloc(stloc) else {
        return false;
    };
    if !variable_can_be_used_for_inlining(f, v) {
        return false;
    }
    inline_one(f, stloc, options, ctx)
}

/// `VariableCanBeUsedForInlining`: stored once, used once, not pinned.
pub fn variable_can_be_used_for_inlining(f: &Function, v: VarId) -> bool {
    let var = f.var(v);
    var.kind != VariableKind::PinnedLocal
        && var.store_count() == 1
        && var.load_count() + var.address_count() == 1
}

/// `InlineOne`: inlines the `stloc` into the next instruction, or drops it when the variable
/// is never used. The caller ensures `v` has no other uses.
pub fn inline_one(
    f: &mut Function,
    stloc: NodeId,
    options: u32,
    ctx: &mut Context<'_, '_, '_>,
) -> bool {
    let (v, value) = f.match_stloc(stloc).expect("stloc");
    let block = f.node(stloc).parent.expect("stloc in a block");
    let pos = f.node(stloc).child_index as usize;
    let next = f.list(block).get(pos + 1).copied();
    if do_inline(f, v, value, next, options, ctx) {
        f.add_il_range(value, f.il_range(stloc));
        f.remove_at(block, pos);
        return true;
    }
    let var = f.var(v);
    if var.load_count() == 0 && var.address_count() == 0 {
        if is_pure(f.flags(value)) {
            // No effects except reading locals: drop the whole store.
            f.remove_at(block, pos);
            return true;
        }
        if var.kind == VariableKind::StackSlot {
            // Drop the store, keep the expression.
            f.add_il_range(value, f.il_range(stloc));
            f.replace(stloc, value);
            return true;
        }
    }
    false
}

/// `DoInline`: replaces the load of `v` in `next` by `inlined`, if possible.
fn do_inline(
    f: &mut Function,
    v: VarId,
    inlined: NodeId,
    next: Option<NodeId>,
    options: u32,
    ctx: &mut Context<'_, '_, '_>,
) -> bool {
    let Find::Found(load) = find_load_in_next(f, ctx, next, v, inlined, options) else {
        return false;
    };
    // A pointer-typed stackalloc is valid C# only as a local initializer or as the pointer
    // argument of the Span<T> / ReadOnlySpan<T> constructor.
    let is_stackalloc = match f.inst(inlined) {
        Inst::LocAlloc { .. } => true,
        Inst::Block { kind, .. } => *kind == BlockKind::StackAllocInitializer,
        _ => false,
    };
    if is_stackalloc
        && f.result_type(ctx.ts, inlined) == StackType::I
        && !matches!(
            f.node(load).parent.map(|p| f.inst(p)),
            Some(Inst::StLoc { .. })
        )
        && !is_stack_alloc_span_constructor_argument(f, ctx, load)
    {
        return false;
    }
    let is_address = matches!(f.inst(load), Inst::LdLoca { .. });
    if is_address {
        // shortcut: `IsGeneratedTemporaryForAddressOf` is not ported, so `ldloca` is never
        // inlined; port it with the first pipeline step that sets AllowInliningOfLdloca.
        if options & options::ALLOW_INLINING_OF_LDLOCA == 0 {
            return false;
        }
        unimplemented!("inlining of ldloca");
    }
    let aggressive = options & options::AGGRESSIVE != 0;
    if !aggressive
        && f.var(v).kind != VariableKind::StackSlot
        && !non_aggressive_inline_into(f, ctx, next.expect("load found in next"), load, inlined, v)
    {
        return false;
    }
    f.add_il_range(inlined, f.il_range(load));
    f.replace(load, inlined);
    true
}

/// `IsStackAllocSpanConstructorArgument`.
fn is_stack_alloc_span_constructor_argument(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    load: NodeId,
) -> bool {
    let Some(parent) = f.node(load).parent else {
        return false;
    };
    let Inst::NewObj(call) = f.inst(parent) else {
        return false;
    };
    call.args.len() == 2
        && call.args[0] == load
        && call.method.method.declaring.is_some_and(|d| {
            known(ctx, d).is_some_and(|k| {
                matches!(k, KnownTypeCode::SpanOfT | KnownTypeCode::ReadOnlySpanOfT)
            })
        })
}

fn known(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> Option<KnownTypeCode> {
    ctx.ts
        .definition(ty)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .map(|i| i.known)
}

/// `NonAggressiveInlineInto`: whether a non-stack-slot variable is inlined although
/// inlining is not aggressive.
fn non_aggressive_inline_into(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    next: NodeId,
    load: NodeId,
    inlined: NodeId,
    v: VarId,
) -> bool {
    // By the expression being inlined.
    if matches!(
        f.inst(inlined),
        Inst::DefaultValue { .. } | Inst::StObj { .. } | Inst::Switch { .. }
    ) {
        return true;
    }
    if f.result_type(ctx.ts, inlined) == StackType::Ref {
        // VB uses ref locals for compound assignments; avoid them.
        return true;
    }
    // By the instruction it is inlined into.
    let mut parent = f.node(load).parent.expect("load has a parent");
    if is_nullable_ctor(f, ctx, parent) {
        parent = f.node(parent).parent.expect("nullable ctor has a parent");
    }
    match f.inst(parent) {
        Inst::Leave { .. } => return true,
        Inst::Switch { .. } => {
            // Keep the variable's type on the switch when inlining into its value slot.
            if f.var(v).kind != VariableKind::StackSlot && f.node(load).child_index == 0 {
                let ty = f.var(v).ty;
                if let Inst::Switch { ty: switch_ty, .. } = f.inst_mut(parent)
                    && switch_ty.is_none()
                {
                    *switch_ty = Some(ty);
                }
            }
            return true;
        }
        Inst::CallIndirect { .. } if f.node(load).child_index == 0 => {
            return true;
        }
        _ => {}
    }
    // By the top-level instruction.
    if matches!(f.inst(next), Inst::If { .. }) {
        while f.match_logic_not(parent).is_some() {
            parent = f.node(parent).parent.expect("logic not has a parent");
        }
        return parent == next;
    }
    false
}

/// `NullableLiftingTransform.MatchNullableCtor`: `newobj Nullable<T>..ctor(arg)`.
fn is_nullable_ctor(f: &Function, ctx: &mut Context<'_, '_, '_>, id: NodeId) -> bool {
    let Inst::NewObj(call) = f.inst(id) else {
        return false;
    };
    call.args.len() == 1
        && call.method.method.name == ".ctor"
        && call
            .method
            .method
            .declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::NullableOfT))
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Find {
    Found(NodeId),
    /// Not found, and the moved expression cannot be reordered past this point.
    Stop,
    /// Not found, but the search may go on.
    Continue,
}

/// `FindLoadInNext`.
fn find_load_in_next(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    expr: Option<NodeId>,
    v: VarId,
    moved: NodeId,
    options: u32,
) -> Find {
    let Some(expr) = expr else {
        return Find::Stop;
    };
    let is_load = match *f.inst(expr) {
        Inst::LdLoc { var } | Inst::LdLoca { var } => var == v,
        _ => false,
    };
    if is_load {
        let parent = f.node(expr).parent.expect("load has a parent");
        let index = f.node(expr).child_index as usize;
        if satisfies_slot_restriction_for_inlining(f, parent, index, moved) {
            return Find::Found(expr);
        }
        if options & options::ALLOW_CHANGING_ORDER_OF_EVALUATION_FOR_EXCEPTIONS != 0
            && matches!(f.inst(parent), Inst::StObj { .. })
            && index == 0
        {
            // Changes when the exception is thrown, but avoids a ref local.
            match f.inst_mut(moved) {
                Inst::LdFlda {
                    delay_exceptions, ..
                }
                | Inst::LdElema {
                    delay_exceptions, ..
                } => *delay_exceptions = true,
                _ => {}
            }
            return Find::Found(expr);
        }
        return Find::Stop;
    }
    for (index, child) in f.inst(expr).children().into_iter().enumerate() {
        if !can_inline_into_slot(f, ctx, expr, index, moved) {
            return Find::Stop;
        }
        let r = find_load_in_next(f, ctx, Some(child), v, moved, options);
        if r != Find::Continue {
            return r;
        }
    }
    if may_reorder(f, moved, expr) {
        Find::Continue
    } else {
        Find::Stop
    }
}

/// `ILInstruction.CanInlineIntoSlot` and its overrides: whether `moved`, which runs before
/// `parent`, may move into the child slot `index`.
fn can_inline_into_slot(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    parent: NodeId,
    index: usize,
    moved: NodeId,
) -> bool {
    match f.inst(parent) {
        Inst::If { .. } | Inst::Switch { .. } => index == 0,
        Inst::SwitchSection { .. }
        | Inst::TryCatch { .. }
        | Inst::TryFinally { .. }
        | Inst::TryFault { .. }
        | Inst::TryCatchHandler { .. } => false,
        Inst::Block {
            kind, instructions, ..
        } => match kind {
            BlockKind::ControlFlow => {
                let in_container = f
                    .node(parent)
                    .parent
                    .is_some_and(|p| matches!(f.inst(p), Inst::BlockContainer { .. }));
                in_container && index == 0
            }
            BlockKind::ArrayInitializer
            | BlockKind::CollectionInitializer
            | BlockKind::ObjectInitializer
            | BlockKind::CallInlineAssign => index == 0,
            BlockKind::CallWithNamedArgs => index < instructions.len(),
            _ => false,
        },
        // Into the entry point, unless that moves code into a loop.
        Inst::BlockContainer { .. } => {
            index == 0
                && f.entry_point(parent)
                    .is_some_and(|e| f.node(e).incoming_edge_count == 1)
        }
        &Inst::Comp {
            kind,
            sign,
            input,
            left,
            right,
        } => {
            // `comp.o` other than (in)equality becomes `Unsafe.As(ref a) op Unsafe.As(ref b)`,
            // which needs variables.
            if kind.is_equality_or_inequality() || input != StackType::Obj {
                return true;
            }
            if matches!(f.inst(moved), Inst::LdLoc { .. }) || is_ldsfld(f, moved) {
                return true;
            }
            sign != crate::il::types::Sign::Signed
                && [moved, left, right]
                    .iter()
                    .any(|&n| matches!(f.inst(n), Inst::LdNull))
        }
        Inst::IsInst { ty, arg } => {
            let ty = ty.ty;
            let arg = *arg;
            if ctx.ts.is_reference_type(ty) == Some(true) || is_pure(f.flags(moved)) {
                return true;
            }
            // `expr is T ? (T)expr : null` duplicates a pure box harmlessly, but only for a
            // box that becomes the whole argument.
            if matches!(*f.inst(moved), Inst::Box { arg: boxed, .. } if is_pure(f.flags(boxed)))
                && f.inst(arg).children().is_empty()
            {
                return true;
            }
            let Some(grand) = f.node(parent).parent else {
                return false;
            };
            match *f.inst(grand) {
                // `expr as T?` (`ExpressionBuilder.IsUnboxAnyWithIsInst`).
                Inst::UnboxAny { ty: unbox, .. } => {
                    unbox.ty == ty
                        && (known(ctx, unbox.ty) == Some(KnownTypeCode::NullableOfT)
                            || ctx.ts.is_reference_type(ty) == Some(true))
                }
                // `expr is T`, and the statement form `StatementBuilder.VisitIsInst` supports.
                Inst::Block {
                    kind: BlockKind::ControlFlow,
                    ..
                } => true,
                _ => {
                    f.match_comp_null(grand, ComparisonKind::Equality).is_some()
                        || f.match_comp_null(grand, ComparisonKind::Inequality)
                            .is_some()
                }
            }
        }
        _ => true,
    }
}

/// `MatchLdsFld`: `ldobj(ldsflda F)`, neither volatile nor unaligned.
fn is_ldsfld(f: &Function, id: NodeId) -> bool {
    matches!(*f.inst(id), Inst::LdObj { target, is_volatile: false, unaligned: 0, .. }
        if matches!(f.inst(target), Inst::LdsFlda { .. }))
}

/// `SatisfiesSlotRestrictionForInlining`: a `stobj` target that may throw early (an
/// `ldflda` or `ldelema`) only takes the slot when the stored value is pure.
fn satisfies_slot_restriction_for_inlining(
    f: &Function,
    parent: NodeId,
    index: usize,
    new_child: NodeId,
) -> bool {
    if let Inst::StObj { value, .. } = *f.inst(parent)
        && index == 0
        && matches!(
            f.inst(new_child),
            Inst::LdElema { .. } | Inst::LdFlda { .. }
        )
    {
        return is_pure(f.flags(value));
    }
    true
}
