//! `IL/Transforms/ExpandNestedConditionals.cs`: a statement storing a nested conditional
//! operator to a local is expanded back into if-else past `MAX_NESTING` levels.
//! ExpressionTransforms collapses an else-if ladder of assignments into one expression,
//! however long; the transforms that need a single expression have seen it by now.

use crate::il::ast::{BlockKind, ContainerKind, Function, FunctionKind, Inst, NodeId, NopKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::expression_transforms::{match_logic_and, match_logic_or};
use super::il_extraction::extract;
use super::il_inlining::is_catch_when_block;
use super::local_function_decompiler::all_functions;

/// How many conditional operators one statement may keep.
const MAX_NESTING: u32 = 1;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if ctx.settings.aggressive_inlining {
        return;
    }
    // `function.Descendants.OfType<ILFunction>()`: the nested functions, the top-level one last.
    let mut functions: Vec<Option<NodeId>> =
        all_functions(&r.function).into_iter().map(Some).collect();
    functions.push(None);
    for function in functions {
        if !is_expandable_function(r, ctx, function) {
            continue;
        }
        let f = &r.function;
        let roots: Vec<NodeId> = match function {
            Some(n) => vec![n],
            None => f
                .body
                .into_iter()
                .chain(f.local_functions.iter().copied())
                .collect(),
        };
        let blocks: Vec<NodeId> = roots
            .into_iter()
            .flat_map(|root| f.descendants_post(root))
            .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
            .collect();
        for block in blocks {
            // Only the blocks of this function, not of the functions nested in it.
            if r.function.enclosing_function(block) != function {
                continue;
            }
            if !is_expandable_block(&r.function, block) {
                continue;
            }
            let mut i = 0;
            while i < r.function.list(block).len() {
                extract_into(r, ctx, block, i);
                expand(r, ctx, block, i);
                i += 1;
            }
        }
    }
}

/// `IsExpandableFunction`: a top-level or local function that is no constructor. A lambda may
/// have to stay one expression (query clauses, expression trees); a constructor is matched as
/// a whole for field initializers and primary constructors.
fn is_expandable_function(
    r: &ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    function: Option<NodeId>,
) -> bool {
    let method = match function.map(|n| r.function.inst(n)) {
        None => Some(r.method),
        Some(Inst::Function {
            kind: FunctionKind::LocalFunction,
            read,
            ..
        }) => read.as_ref().map(|read| read.method),
        Some(_) => return false,
    };
    let is_constructor = method
        .and_then(|m| ctx.ts.method_definition(m).ok())
        .is_some_and(|m| m.name == ".ctor" || m.name == ".cctor");
    !is_constructor
}

/// `IsExpandableBlock`: a control-flow block, not a catch-when block nor a loop or switch
/// header.
fn is_expandable_block(f: &Function, block: NodeId) -> bool {
    if !matches!(
        f.inst(block),
        Inst::Block {
            kind: BlockKind::ControlFlow,
            ..
        }
    ) {
        return false;
    }
    if is_catch_when_block(f, block) {
        return false;
    }
    let Some(container) = f.node(block).parent else {
        return true;
    };
    let Inst::BlockContainer { kind, blocks, .. } = f.inst(container) else {
        // A branch body hangs off the if, not off a container.
        return true;
    };
    let (entry, last) = (blocks[0], blocks[blocks.len() - 1]);
    match kind {
        // The entry point holds the loop condition or the switch value.
        ContainerKind::While | ContainerKind::Switch => block != entry,
        // The last block of a for loop holds the increment.
        ContainerKind::For => block != entry && block != last,
        ContainerKind::DoWhile => block != last,
        _ => true,
    }
}

/// `ExtractInto`: a nested conditional that is not stored to a variable moves to a new one,
/// where the position it flows into names a type.
fn extract_into(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, block: NodeId, pos: usize) {
    let f = &r.function;
    let stmt = f.list(block)[pos];
    if let Some((_, value)) = f.match_stloc(stmt)
        && matches!(f.inst(value), Inst::If { .. })
    {
        return;
    }
    let candidates: Vec<NodeId> = f
        .descendants_post(stmt)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::If { .. }))
        .collect();
    for inst in candidates {
        let f = &r.function;
        let parent = f.node(inst).parent;
        if parent.is_some_and(|p| matches!(f.inst(p), Inst::StLoc { .. }))
            || !exceeds_nesting(f, inst, MAX_NESTING)
        {
            continue;
        }
        let Some(parent) = parent else {
            continue;
        };
        let index = f.node(inst).child_index as usize;
        let Some(expected) = infer_expected_type(r, ctx, parent, index) else {
            continue;
        };
        if matches!(
            ctx.ts.data(expected),
            TypeData::Unknown { .. } | TypeData::UnknownSpecial
        ) || ctx.ts.stack_type(expected) != r.function.result_type(ctx.ts, inst)
        {
            continue;
        }
        if let Some(v) = extract(&mut r.function, ctx, inst) {
            r.function.var_mut(v).ty = expected;
            return;
        }
    }
}

/// `InferExpectedType`: the type the position `index` of `inst` expects (`None` for unknown).
fn infer_expected_type(
    r: &ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    index: usize,
) -> Option<TypeId> {
    let f = &r.function;
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
            let instance = !(c.method.method.is_static || matches!(f.inst(inst), Inst::NewObj(_)));
            if index == 0 && instance {
                return c.constrained_to.map(|t| t.ty).or(c.method.method.declaring);
            }
            let index = index.checked_sub(usize::from(instance))?;
            c.method.method.params().ok()?.get(index).copied()
        }
        Inst::Leave { target, .. } if index == 0 => {
            // A return value only where the leave leaves its function's body.
            let function = f.enclosing_function(inst);
            let body = match function {
                None => f.body,
                Some(n) => match f.inst(n) {
                    Inst::Function { body, .. } => Some(*body),
                    _ => None,
                },
            };
            if body.is_none() || *target != body {
                return None;
            }
            match function {
                None => ctx.ts.method_definition(r.method).ok().map(|m| m.ret),
                Some(n) => match f.inst(n) {
                    Inst::Function {
                        read: Some(read), ..
                    } => {
                        let method = read.method;
                        ctx.ts.method_definition(method).ok().map(|m| m.ret)
                    }
                    _ => None,
                },
            }
        }
        Inst::StObj { ty, .. } if index == 1 => Some(ty.ty),
        Inst::StLoc { var, .. } if index == 0 => Some(f.var(*var).ty),
        Inst::If { .. } if index == 0 => Some(ctx.ts.known_type(KnownTypeCode::Boolean)),
        Inst::NewArr { .. } => Some(ctx.ts.known_type(KnownTypeCode::Int32)),
        _ => None,
    }
}

/// `Expand`: `stloc v(if (c) a else b)` becomes `if (c) { stloc v(a) } else { stloc v(b) }`,
/// down the nesting branch.
fn expand(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, block: NodeId, pos: usize) {
    let mut worklist = vec![(block, pos)];
    while let Some((current, index)) = worklist.pop() {
        let f = &mut r.function;
        let stloc = f.list(current)[index];
        let Some((v, if_inst)) = f.match_stloc(stloc) else {
            continue;
        };
        let Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } = *f.inst(if_inst)
        else {
            continue;
        };
        let var = f.var(v);
        if !matches!(
            var.kind,
            crate::il::ast::VariableKind::Local | crate::il::ast::VariableKind::StackSlot
        ) || matches!(ctx.ts.data(var.ty), TypeData::ByRef(_))
        {
            continue;
        }
        // `IsInConstructorInitializer` is always false here: expanded functions are no
        // constructors, so their chained constructor call offset is -1.
        if !exceeds_nesting(f, if_inst, MAX_NESTING) {
            continue;
        }
        // Undo the negation and branch swap of HandleConditionalOperator, so the expansion is the
        // exact inverse of the collapse.
        let (mut condition, mut true_value, mut false_value) = (condition, true_inst, false_inst);
        while let Some(inner) = f.match_logic_not(condition) {
            condition = inner;
            std::mem::swap(&mut true_value, &mut false_value);
        }
        let true_block = new_store_block(f, v, true_value);
        let false_block = new_store_block(f, v, false_value);
        let void = ctx.ts.known_type(KnownTypeCode::Void);
        let expanded = f.add(Inst::If {
            condition,
            true_inst: true_block,
            false_inst: false_block,
            csharp_type: Some(void),
        });
        let (if_range, stloc_range) = (f.il_range(if_inst), f.il_range(stloc));
        f.add_il_range(expanded, if_range);
        f.add_il_range(expanded, stloc_range);
        f.replace(stloc, expanded);
        // The branches are statements now, so each may expand in turn.
        worklist.push((true_block, 0));
        worklist.push((false_block, 0));
    }
}

/// A block of the one store `stloc v(value)`; the store and the block take the value's IL
/// range, so the statement keeps a sequence point of its own.
fn new_store_block(f: &mut Function, v: crate::il::ast::VarId, value: NodeId) -> NodeId {
    let range = f.il_range(value);
    let store = f.add(Inst::StLoc {
        var: v,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.add_il_range(store, range);
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let block = f.add(Inst::Block {
        kind: BlockKind::ControlFlow,
        instructions: vec![store],
        final_inst: nop,
    });
    f.add_il_range(block, range);
    block
}

/// `ExceedsNesting`: whether the branches of `inst` nest conditional operators `depth` levels
/// deep; `&&` and `||` share the representation but print without nesting.
fn exceeds_nesting(f: &Function, inst: NodeId, depth: u32) -> bool {
    let Inst::If {
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return false;
    };
    if match_logic_and(f, inst).is_some() || match_logic_or(f, inst).is_some() {
        return false;
    }
    depth == 0
        || exceeds_nesting(f, true_inst, depth - 1)
        || exceeds_nesting(f, false_inst, depth - 1)
}
