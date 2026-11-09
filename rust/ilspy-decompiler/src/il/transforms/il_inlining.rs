//! `IL/Transforms/ILInlining.cs`: moves the value of a single-use `stloc` into the one
//! instruction after it that loads the variable, where evaluation order allows it.

use crate::il::ast::{
    AwaiterMethod, BlockKind, ComparisonKind, ComparisonLiftingKind, CompoundTargetKind, Function,
    Inst, NodeId, VarId, VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::{is_pure, may_reorder};
use crate::il::types::StackType;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId};

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
    let (load, named_argument) = match find_load_in_next(f, ctx, next, v, inlined, options) {
        Find::Found(load) => (load, None),
        Find::NamedArgument { load, call_arg } => (load, Some(call_arg)),
        _ => return false,
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
        if !is_generated_temporary_for_address_of(f, ctx, load, inlined, options) {
            return false;
        }
    } else {
        let aggressive = options & options::AGGRESSIVE != 0;
        if !aggressive
            && f.var(v).kind != VariableKind::StackSlot
            && match named_argument {
                // A named argument inlines what was not on the IL stack top level.
                Some(_) => f
                    .node(inlined)
                    .parent
                    .and_then(|p| match *f.inst(p) {
                        Inst::StLoc {
                            il_stack_was_empty, ..
                        } => Some(il_stack_was_empty),
                        _ => None,
                    })
                    .expect("the inlined value of a store"),
                None => !non_aggressive_inline_into(
                    f,
                    ctx,
                    next.expect("load found in next"),
                    load,
                    inlined,
                    v,
                ),
            }
        {
            return false;
        }
    }
    if let Some(call_arg) = named_argument {
        // Evaluated early now, the argument inlines as usual.
        super::named_argument_transform::introduce_named_argument(f, ctx, call_arg);
    }
    f.add_il_range(inlined, f.il_range(load));
    let result = if is_address {
        // The compiler-generated temporary keeps its semantics as `addressof`.
        let ty = f.var(v).ty;
        f.add(Inst::AddressOf {
            value: inlined,
            ty: crate::il::ast::TypeOperand { token: 0, ty },
        })
    } else {
        inlined
    };
    f.replace(load, result);
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

pub(crate) fn known(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> Option<KnownTypeCode> {
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
        Inst::DefaultValue { .. }
            | Inst::StObj { .. }
            | Inst::NumericCompoundAssign { .. }
            | Inst::UserDefinedCompoundAssign { .. }
            | Inst::Switch { .. }
            | Inst::Await { .. }
    ) {
        return true;
    }
    if let Inst::LdLoc { var } = *f.inst(inlined)
        && f.var(v).state_machine_field.is_none()
        && f.var(var).state_machine_field.is_some()
    {
        // Roslyn puts a fetched state machine field into a temporary.
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
    // `ILiftableInstruction.IsLifted`.
    let lifted = match f.inst(parent) {
        Inst::Comp { lifting, .. } => *lifting != ComparisonLiftingKind::None,
        Inst::BinaryNumeric { is_lifted, .. } | Inst::Conv { is_lifted, .. } => *is_lifted,
        Inst::NumericCompoundAssign { is_lifted, .. } => *is_lifted,
        Inst::BitNot {
            lifted_underlying, ..
        } => lifted_underlying.is_some(),
        Inst::ThreeValuedBoolAnd { .. } | Inst::ThreeValuedBoolOr { .. } => true,
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => c.method.lifted,
        _ => false,
    };
    if lifted {
        // Into lifted operators.
        return true;
    }
    match f.inst(parent) {
        Inst::Leave { .. } | Inst::YieldReturn { .. } => return true,
        // Size expressions into `localloc.span`, and `fixed` initializers.
        Inst::GetPinnableReference { .. } | Inst::LocAllocSpan { .. } => return true,
        // Into dynamic compound assignments.
        Inst::Dynamic {
            kind: crate::il::ast::DynamicKind::GetMember | crate::il::ast::DynamicKind::GetIndex,
            ..
        } if f
            .node(parent)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::DynamicCompoundAssign { .. })) =>
        {
            return true;
        }
        Inst::DynamicCompoundAssign { .. } => return true,
        // Into the `?.` operator.
        Inst::NullableUnwrap { .. } => return true,
        // Nullables into `??`.
        Inst::NullCoalescing { .. }
            if known(ctx, f.var(v).ty) == Some(KnownTypeCode::NullableOfT) =>
        {
            return true;
        }
        Inst::LdElema {
            with_system_index: true,
            ..
        } => return true,
        // The string switch value.
        Inst::StringToInt { .. }
            if f.node(parent).child_index == 0
                && f.node(parent)
                    .parent
                    .is_some_and(|p| matches!(f.inst(p), Inst::Switch { .. })) =>
        {
            return true;
        }
        &Inst::Match {
            is_deconstruct_tuple,
            check_type,
            variable,
            ..
        } => {
            if is_deconstruct_tuple
                || (check_type && ctx.ts.is_reference_type(f.var(variable).ty) != Some(true))
            {
                return true;
            }
        }
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
        // Into getter calls that are the target of a compound assignment (the compiler uses
        // locals to evaluate the arguments once for getter and setter).
        Inst::Call(_) | Inst::CallVirt(_) if is_compound_target(f, parent) => return true,
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
/// `FindResultType`.
pub(crate) enum Find {
    Found(NodeId),
    /// Not found, and the moved expression cannot be reordered past this point.
    Stop,
    /// Not found, but the search may go on.
    Continue,
    /// A deconstruction (with `FIND_DECONSTRUCTION`), into whose init the store may go.
    Deconstruction(NodeId),
    /// A load in a call argument that a named argument can evaluate early (with
    /// `INTRODUCE_NAMED_ARGUMENTS`): the load and the argument.
    NamedArgument {
        load: NodeId,
        call_arg: NodeId,
    },
}

/// `FindLoadInNext`.
pub(crate) fn find_load_in_next(
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
    if matches!(
        f.inst(expr),
        Inst::Block {
            kind: BlockKind::CallWithNamedArgs,
            ..
        }
    ) {
        return super::named_argument_transform::can_extend_named_argument(f, ctx, expr, v, moved);
    }
    if options & options::FIND_DECONSTRUCTION != 0
        && matches!(f.inst(expr), Inst::Deconstruct { .. })
    {
        return Find::Deconstruction(expr);
    }
    for (index, child) in f.inst(expr).children().into_iter().enumerate() {
        if !can_inline_into_slot(f, ctx, expr, index, moved) {
            return Find::Stop;
        }
        let r = find_load_in_next(f, ctx, Some(child), v, moved, options);
        if r != Find::Continue {
            if r == Find::Stop
                && options & options::INTRODUCE_NAMED_ARGUMENTS != 0
                && matches!(
                    f.inst(expr),
                    Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_)
                )
            {
                return super::named_argument_transform::can_introduce_named_argument(
                    f, ctx, expr, child, v, moved,
                );
            }
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
        Inst::If { .. }
        | Inst::Switch { .. }
        | Inst::PinnedRegion { .. }
        | Inst::Lock { .. }
        | Inst::Using { .. }
        | Inst::NullCoalescing { .. } => index == 0,
        // The init stores and the pattern, not the conversions and assignments.
        Inst::Deconstruct { init, .. } => index <= init.len(),
        // Only completely pure constants go into an expression tree, whose evaluation the
        // lambda delays.
        Inst::Function { kind, .. } => {
            *kind == crate::il::ast::FunctionKind::ExpressionTree
                && f.flags(moved) == crate::il::ast::Flags::NONE
        }
        // Not an expression containing a `nullable.unwrap` that it does not rewrap itself.
        Inst::NullableRewrap { .. } => !f.has_flag(moved, crate::il::ast::Flags::MAY_UNWRAP_NULL),
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
            ..
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

/// `InlineInto`: inlines the instructions before `pos` into the one at `pos`, as long as
/// that succeeds; returns how many were inlined.
pub fn inline_into(
    f: &mut Function,
    block: NodeId,
    pos: usize,
    options: u32,
    ctx: &mut Context<'_, '_, '_>,
) -> usize {
    if pos >= f.list(block).len() {
        return 0;
    }
    let mut count = 0;
    let mut pos = pos;
    while pos > 0 {
        pos -= 1;
        if inline_one_if_possible(f, block, pos, options, ctx) {
            count += 1;
        } else {
            break;
        }
    }
    count
}

/// `IsReadonlyReference`: whether C# can only take `addr` as a readonly reference.
pub(crate) fn is_readonly_reference(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    addr: NodeId,
) -> bool {
    match f.inst(addr) {
        Inst::LdFlda { target, field, .. } => {
            ctx.ts.field_is_read_only(&field.field)
                || (field
                    .field
                    .declaring
                    .and_then(|d| ctx.ts.definition(d))
                    .and_then(|d| ctx.ts.comp.type_def(d).ok())
                    .is_some_and(|i| i.kind == crate::typesystem::compilation::TypeKind::Struct)
                    && is_readonly_reference(f, ctx, *target))
        }
        Inst::LdsFlda { field } => ctx.ts.field_is_read_only(&field.field),
        Inst::LdLoc { var } => f.var(*var).is_ref_read_only,
        Inst::Call(c) | Inst::CallVirt(c) => {
            ctx.ts.method_return_type_is_ref_read_only(&c.method.method)
        }
        Inst::CallIndirect { signature, .. } => matches!(
            ctx.ts.data(*signature),
            crate::typesystem::types::TypeData::FnPtr(p) if p.ret_ref_readonly
        ),
        // C# does not allow mutation of value-type temporaries.
        Inst::AddressOf { .. } => true,
        _ => f
            .match_ldfld(ctx.ts, addr)
            .is_some_and(|(_, field)| ctx.ts.field_return_type_is_ref_read_only(&field.field)),
    }
}

/// The method `IsUsedAsThisPointerInCall` reports, with the constrained type of the call.
enum ThisCallMethod {
    None,
    Method(ResolvedMethod, Option<TypeId>),
    /// The `GetAwaiter` of a dynamic await, declared on `dynamic`.
    DynamicAwaiter,
}

/// `ILInlining.IsUsedAsThisPointerInCall`: `ldloca` of a value-type variable used as the
/// `this` argument of a call (through `ldobj.if.ref` and field addresses), an `await` or a
/// `match`.
pub(crate) fn is_used_as_this_pointer_in_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> bool {
    used_as_this_pointer_in_call(f, ctx, ldloca).is_some()
}

/// `IsUsedAsThisPointerInCall(ldloca, out method, out constrainedType)`.
fn used_as_this_pointer_in_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> Option<ThisCallMethod> {
    let Inst::LdLoca { var } = *f.inst(ldloca) else {
        return None;
    };
    if ctx.ts.is_reference_type(f.var(var).ty) == Some(true) {
        return None;
    }
    let parent_of = |inst: NodeId| f.node(inst).parent;
    let mut inst = ldloca;
    if let Some(p) = parent_of(inst)
        && matches!(f.inst(p), Inst::LdObjIfRef { .. })
    {
        inst = p;
    }
    while let Some(p) = parent_of(inst)
        && matches!(f.inst(p), Inst::LdFlda { .. })
    {
        inst = p;
    }
    if f.node(inst).child_index != 0 {
        return None;
    }
    let parent = parent_of(inst)?;
    match f.inst(parent) {
        Inst::Call(c) | Inst::CallVirt(c) => {
            let m = &c.method.method;
            let used = if m.accessor_kind != 0 {
                // A getter, unless it is the target of a property compound assignment (C#
                // allows none on temporary structs); C# calls no setters on temporaries.
                m.accessor_kind == 0x0002
                    && !parent_of(parent).is_some_and(|cai| {
                        matches!(*f.inst(cai),
                            Inst::NumericCompoundAssign { target_kind: CompoundTargetKind::Property, target, .. }
                            | Inst::UserDefinedCompoundAssign { target_kind: CompoundTargetKind::Property, target, .. }
                            | Inst::DynamicCompoundAssign { target_kind: CompoundTargetKind::Property, target, .. }
                            if target == parent)
                    })
            } else {
                !m.is_static
            };
            used.then(|| ThisCallMethod::Method(m.clone(), c.constrained_to.map(|t| t.ty)))
        }
        Inst::Await { get_awaiter, .. } => Some(match get_awaiter {
            Some(AwaiterMethod::Method(m)) => ThisCallMethod::Method(m.method.clone(), None),
            Some(AwaiterMethod::Dynamic(_)) => ThisCallMethod::DynamicAwaiter,
            None => ThisCallMethod::None,
        }),
        Inst::Match { method, .. } => Some(match method {
            Some(m) => ThisCallMethod::Method(m.method.clone(), None),
            None => ThisCallMethod::None,
        }),
        Inst::NullableUnwrap { ref_input, .. } => ref_input.then_some(ThisCallMethod::None),
        _ => None,
    }
}

/// `IsUsedAsThisPointerInFieldRead`: mcs reads fields of rvalues through temporaries.
fn is_used_as_this_pointer_in_field_read(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> bool {
    let Inst::LdLoca { var } = *f.inst(ldloca) else {
        return false;
    };
    if ctx.ts.is_reference_type(f.var(var).ty) == Some(true) {
        return false;
    }
    let mut inst = ldloca;
    while let Some(p) = f.node(inst).parent
        && matches!(f.inst(p), Inst::LdFlda { .. })
    {
        inst = p;
    }
    inst != ldloca
        && f.node(inst)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::LdObj { .. }))
}

/// `MethodRequiresCopyForReadonlyLValue`.
fn method_requires_copy_for_readonly_lvalue(
    ctx: &mut Context<'_, '_, '_>,
    method: &ThisCallMethod,
) -> bool {
    let (m, constrained_to) = match method {
        ThisCallMethod::None => return true,
        // `dynamic` is a reference type, which is never copied.
        ThisCallMethod::DynamicAwaiter => return false,
        ThisCallMethod::Method(m, constrained_to) => (m, *constrained_to),
    };
    if let Some(t) = constrained_to.or(m.declaring)
        && ctx.ts.is_reference_type(t) == Some(true)
    {
        return false;
    }
    // No copies for calls on readonly structs and readonly members (`ThisIsRefReadOnly`).
    if let MethodOrigin::Def(id) = m.origin {
        let owner = ctx.ts.accessor_owner(id).map(|(o, _)| o);
        if ctx.ts.method_flags(id, owner).this_read_only {
            return false;
        }
    }
    true
}

/// `ExpressionClassification`.
#[allow(clippy::enum_variant_names)]
#[derive(Clone, Copy, PartialEq, Eq)]
enum ExpressionClassification {
    RValue,
    MutableLValue,
    ReadonlyLValue,
}

/// `ClassifyExpression`: whether the C# of `inst` is an l-value that can mutate a value type.
fn classify_expression(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> ExpressionClassification {
    use ExpressionClassification::*;
    let lvalue = |readonly: bool| {
        if readonly {
            ReadonlyLValue
        } else {
            MutableLValue
        }
    };
    match f.inst(inst) {
        &Inst::LdLoc { var } | &Inst::StLoc { var, .. } => {
            let v = f.var(var);
            lvalue(
                v.is_ref_read_only
                    || matches!(
                        v.kind,
                        VariableKind::ForeachLocal | VariableKind::UsingLocal
                    ),
            )
        }
        &Inst::LdObj { target, .. } | &Inst::StObj { target, .. } => {
            lvalue(is_readonly_reference(f, ctx, target))
        }
        // Multi-dimensional array getters are l-values.
        Inst::Call(c) => match c.method.method.declaring.map(|t| ctx.ts.data(t)) {
            Some(TypeData::Array { .. }) => MutableLValue,
            _ => RValue,
        },
        _ => RValue,
    }
}

/// `IsPassedToReadOnlySpanCtor`: `ldloca` as the argument of `new ReadOnlySpan<T>(ref x)`.
fn is_passed_to_read_only_span_ctor(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> bool {
    let Some(parent) = f.node(ldloca).parent else {
        return false;
    };
    let Inst::NewObj(c) = f.inst(parent) else {
        return false;
    };
    let m = &c.method.method;
    m.name == ".ctor"
        && m.params().is_ok_and(|p| p.len() == 1)
        && m.declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::ReadOnlySpanOfT))
}

/// `IsPassedToInlineArrayAsSpan`: `ldloca` passed to the compiler's
/// `<PrivateImplementationDetails>.InlineArrayAs(ReadOnly)Span(ref buffer, length)`.
fn is_passed_to_inline_array_as_span(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> bool {
    let Some(parent) = f.node(ldloca).parent else {
        return false;
    };
    let Inst::Call(c) = f.inst(parent) else {
        return false;
    };
    let m = &c.method.method;
    let span_return = matches!(
        known(ctx, m.ret),
        Some(KnownTypeCode::SpanOfT | KnownTypeCode::ReadOnlySpanOfT)
    );
    let params_match = match m.params().as_deref() {
        Ok(&[arg, length]) => {
            matches!(ctx.ts.data(arg), TypeData::ByRef(_))
                && known(ctx, length) == Some(KnownTypeCode::Int32)
        }
        _ => false,
    };
    m.declaring
        .is_some_and(|d| ctx.ts.reflection_name(d) == "<PrivateImplementationDetails>")
        && matches!(
            m.name.as_str(),
            "InlineArrayAsReadOnlySpan" | "InlineArrayAsSpan"
        )
        && params_match
        && span_return
}

/// `IsGeneratedTemporaryForAddressOf`: whether `ldloca v` may become `addressof(inlined)`.
fn is_generated_temporary_for_address_of(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    load: NodeId,
    inlined: NodeId,
    options: u32,
) -> bool {
    use ExpressionClassification::*;
    // No ldloca inlining in the early inlining stage.
    if options & options::ALLOW_INLINING_OF_LDLOCA == 0 {
        return false;
    }
    let aggressive = options & options::AGGRESSIVE != 0;
    if let Some(method) = used_as_this_pointer_in_call(f, ctx, load) {
        if aggressive {
            return true;
        }
        return match classify_expression(f, ctx, inlined) {
            RValue => true,
            MutableLValue => false,
            ReadonlyLValue => method_requires_copy_for_readonly_lvalue(ctx, &method),
        };
    }
    if f.node(load)
        .parent
        .is_some_and(|p| matches!(f.inst(p), Inst::LdElemaInlineArray { .. }))
    {
        return true;
    }
    if is_passed_to_read_only_span_ctor(f, ctx, load) {
        // An `in` or `ref readonly` parameter takes an rvalue; the expression builder copies.
        return true;
    }
    if is_passed_to_inline_array_as_span(f, ctx, load) {
        // `InlineArrayAsReadOnlySpan(GetInlineArray())` is no valid C#.
        return false;
    }
    if is_passed_to_in_parameter(f, ctx, load) {
        return aggressive || classify_expression(f, ctx, inlined) == RValue;
    }
    if is_used_as_this_pointer_in_field_read(f, ctx, load) {
        return classify_expression(f, ctx, inlined) == RValue;
    }
    false
}

/// `ILInlining.IsPassedToInParameter`: `ldloca` passed to an `in` parameter of a call.
pub(crate) fn is_passed_to_in_parameter(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    ldloca: NodeId,
) -> bool {
    let Some(parent) = f.node(ldloca).parent else {
        return false;
    };
    let (c, is_newobj) = match f.inst(parent) {
        Inst::Call(c) | Inst::CallVirt(c) => (c, false),
        Inst::NewObj(c) => (c, true),
        _ => return false,
    };
    let first_param = usize::from(!(c.method.method.is_static || is_newobj));
    let index = f.node(ldloca).child_index as usize;
    if index < first_param {
        return false;
    }
    let crate::typesystem::resolve::MethodOrigin::Def(def) = c.method.method.origin else {
        return false;
    };
    let (_, params) = ctx.ts.method_signature(def);
    params
        .get(index - first_param)
        .is_some_and(|p| p.ref_kind == crate::typesystem::members::RefKind::In)
}

/// `ILInlining.IsCatchWhenBlock`: the block is in the filter of a catch handler.
pub(crate) fn is_catch_when_block(f: &Function, block: NodeId) -> bool {
    let Some(container) = std::iter::once(block)
        .chain(f.ancestors(block))
        .find(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
    else {
        return false;
    };
    f.node(container).parent.is_some_and(
        |p| matches!(*f.inst(p), Inst::TryCatchHandler { filter, .. } if filter == container),
    )
}

/// `IStatementTransform.Run` of `ILInlining`: inlines into the statement at `pos` until
/// nothing changes.
pub fn run_statement(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
    options: u32,
) {
    let f = &mut r.function;
    let options = options | options_for_block(f, ctx, block, pos);
    while inline_one_if_possible(f, block, pos, options, ctx) {}
}

/// `OptionsForBlock`.
pub(crate) fn options_for_block(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> u32 {
    let mut result = options::NONE;
    if ctx.settings.aggressive_inlining || is_catch_when_block(f, block) {
        result |= options::AGGRESSIVE;
    } else {
        // The engine's functions are top-level methods, never delegates or expression trees,
        // so `PreferExpressionsOverStatements` is false.
        let inst = f.list(block)[pos];
        if is_in_constructor_initializer(f, ctx, inst) {
            result |= options::AGGRESSIVE;
        }
    }
    if !ctx.reader.use_ref_locals_for_accurate_order_of_evaluation {
        result |= options::ALLOW_CHANGING_ORDER_OF_EVALUATION_FOR_EXCEPTIONS;
    }
    result
}

/// `IsInConstructorInitializer`: `inst` and its statement end before the chained
/// constructor call.
pub(crate) fn is_in_constructor_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
    let ctor_call_start = chained_constructor_call_il_offset(f, ctx);
    let f = &*f;
    if f.il_range(inst).end > ctor_call_start {
        return false;
    }
    let in_block = |n: NodeId| {
        f.node(n)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::Block { .. }))
    };
    let Some(top_level) = std::iter::once(inst)
        .chain(f.ancestors(inst))
        .filter(|&n| in_block(n))
        .last()
    else {
        return false;
    };
    f.il_range(top_level).end <= ctor_call_start
}

/// `ILFunction.ChainedConstructorCallILOffset`: the start of the first `base(...)` or
/// `this(...)` call statement of an instance constructor, -1 otherwise. Computed once, on
/// the tree of the first request.
fn chained_constructor_call_il_offset(f: &mut Function, ctx: &mut Context<'_, '_, '_>) -> i32 {
    if let Some(offset) = f.chained_constructor_call_il_offset {
        return offset;
    }
    let offset = if f.is_instance_constructor {
        f.descendants_post(f.body.expect("function body"))
            .into_iter()
            .find(|&d| {
                let (Inst::Call(c) | Inst::CallVirt(c)) = f.inst(d) else {
                    return false;
                };
                let m = &c.method.method;
                let is_constructor = m.name == ".ctor" || m.name == ".cctor";
                is_constructor
                    && m.declaring
                        .is_some_and(|t| ctx.ts.is_reference_type(t) == Some(true))
                    && f.node(d)
                        .parent
                        .is_some_and(|p| matches!(f.inst(p), Inst::Block { .. }))
            })
            .map_or(-1, |d| f.il_range(d).start)
    } else {
        -1
    };
    f.chained_constructor_call_il_offset = Some(offset);
    offset
}

/// Whether `inst` is in the target slot of a compound assignment
/// (`CompoundAssignmentInstruction.TargetSlot`).
fn is_compound_target(f: &Function, inst: NodeId) -> bool {
    f.node(inst).parent.is_some_and(|p| {
        matches!(*f.inst(p),
            Inst::NumericCompoundAssign { target, .. }
            | Inst::UserDefinedCompoundAssign { target, .. }
            | Inst::DynamicCompoundAssign { target, .. }
            if target == inst)
    })
}

/// `ILInlining.CanInlineInto`: whether `moved` (the value of `v`) may be inlined into its
/// load in `expr`.
pub(crate) fn can_inline_into(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    expr: NodeId,
    v: VarId,
    moved: NodeId,
) -> bool {
    matches!(
        find_load_in_next(f, ctx, Some(expr), v, moved, options::NONE),
        Find::Found(_)
    )
}

/// `ILInlining.CanMoveInto`: whether `moved`, from before `stmt`, may become the replacement
/// of `target_load`, a descendant of `stmt`.
pub(crate) fn can_move_into(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    moved: NodeId,
    stmt: NodeId,
    target_load: NodeId,
) -> bool {
    can_move_into_skipping(f, ctx, moved, stmt, target_load, None)
}

/// `ILInlining.CanMoveIntoCallVirt`: as `can_move_into`, but `moved` may be reordered with the
/// `this` argument of `nested_call_virt`.
pub(crate) fn can_move_into_call_virt(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    moved: NodeId,
    stmt: NodeId,
    nested_call_virt: NodeId,
    target_load: NodeId,
) -> bool {
    let this_arg = f.inst(nested_call_virt).children()[0];
    can_move_into_skipping(f, ctx, moved, stmt, target_load, Some(this_arg))
}

fn can_move_into_skipping(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    moved: NodeId,
    stmt: NodeId,
    target_load: NodeId,
    skipped: Option<NodeId>,
) -> bool {
    let mut inst = target_load;
    while inst != stmt {
        let parent = f.node(inst).parent.expect("a descendant of the statement");
        let index = f.node(inst).child_index as usize;
        if !can_inline_into_slot(f, ctx, parent, index, moved) {
            return false;
        }
        // The reordering with the predecessors.
        for predecessor in f.inst(parent).children().into_iter().take(index) {
            if Some(predecessor) == skipped {
                continue;
            }
            if !may_reorder(f, moved, skip_named_argument_store(f, predecessor)) {
                return false;
            }
        }
        inst = parent;
    }
    true
}

/// `SkipNamedArgumentStore`: the value of a named-argument store, which is only read in the
/// final instruction of its block.
fn skip_named_argument_store(f: &Function, predecessor: NodeId) -> NodeId {
    match f.match_stloc(predecessor) {
        Some((v, value)) if f.var(v).kind == VariableKind::NamedArgument => value,
        _ => predecessor,
    }
}
