//! `IL/ControlFlow/SwitchDetection.cs`: turns trees of conditional branches that test one
//! integer value into switch instructions (which, as in C#, need no contiguous values), and
//! the switch clean-up `ControlFlowSimplification` also runs (`SimplifySwitchInstruction`).

use std::collections::{HashMap, HashSet};

use super::control_flow_graph::ControlFlowGraph;
use super::switch_analysis::SwitchAnalysis;
use crate::il::ast::{BinaryNumericOperator, CallData, Function, Inst, MethodOperand, NodeId};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::names::TopLevelTypeName;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::Nullability;
use crate::util::long_set::LongSet;

/// Only the default section may have more values (C# has no range labels).
const MAX_VALUES_PER_SECTION: u64 = 100;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.sparse_integer_switch {
        return;
    }
    let f = &mut r.function;
    let mut analysis = SwitchAnalysis::default();
    analysis.allow_unreachable_cases = ctx.settings.remove_dead_code;
    let body = f.body.expect("body");
    let containers: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for container in containers {
        let mut d = Detection {
            analysis: &mut analysis,
            container,
            cfg: None,
            needs_cleanup: false,
        };
        for block in f.list(container).to_vec() {
            d.process_block(f, ctx, block);
        }
        if d.needs_cleanup {
            // An unreachable switch-like condition loses its block head to the switch; the
            // rest of it is dead code.
            if ctx.settings.remove_dead_code {
                crate::il::transforms::remove_infeasible_path_transform::sort_blocks(f, container);
            } else {
                f.retain(container, |f, b| !f.list(b).is_empty());
            }
        }
    }
}

struct Detection<'a> {
    analysis: &'a mut SwitchAnalysis,
    container: NodeId,
    /// The graph of the container, until a new switch invalidates it.
    cfg: Option<ControlFlowGraph>,
    needs_cleanup: bool,
}

impl Detection<'_> {
    fn process_block(&mut self, f: &mut Function, ctx: &mut Context<'_, '_, '_>, block: NodeId) {
        if self.analysis.analyze_block(f, block) && self.use_csharp_switch(f, ctx) {
            // A multi-block switch that combines into one switch instruction.
            let var = self.analysis.switch_variable();
            let mut value = f.add(Inst::LdLoc { var });
            let stack_type = f.var(var).stack_type;
            if !matches!(stack_type, StackType::I4 | StackType::I8) {
                value = f.add(Inst::Conv {
                    arg: value,
                    input: stack_type,
                    input_sign: Sign::Signed,
                    target: PrimitiveType::I8,
                    check_overflow: false,
                });
            }
            let sections = self
                .analysis
                .sections
                .iter()
                .map(|(labels, body)| {
                    f.add(Inst::SwitchSection {
                        labels: labels.clone(),
                        body: *body,
                        has_null_label: false,
                        is_compiler_generated_default_section: false,
                    })
                })
                .collect();
            let sw = f.add(Inst::Switch {
                value,
                sections,
                ty: None,
            });
            let last = *f.list(block).last().expect("analyzed block");
            if !matches!(f.inst(last), Inst::Switch { .. }) {
                // The branch or leave after the if moves into a section.
                let len = f.list(block).len();
                f.remove_at(block, len - 1);
            }
            let last = *f.list(block).last().expect("analyzed block");
            f.add_il_range(sw, f.il_range(last));
            f.replace(last, sw);
            for &inner in &self.analysis.inner_blocks {
                f.truncate(inner, 0);
            }
            self.cfg = None;
            self.needs_cleanup = true;
            sort_switch_sections(f, sw, ctx);
        } else {
            // The second pass, after return blocks were duplicated.
            simplify_switch_instruction(f, block, ctx);
        }
        inline_switch_expression_default_case_throw_helper(f, ctx, block);
    }

    /// `UseCSharpSwitch`: whether a switch reads better than the if statements.
    fn use_csharp_switch(&mut self, f: &Function, ctx: &mut Context<'_, '_, '_>) -> bool {
        let a = &*self.analysis;
        if a.inner_blocks.is_empty() {
            return false;
        }
        let Some((default_key, _)) = a
            .sections
            .iter()
            .find(|(k, _)| k.count() > MAX_VALUES_PER_SECTION)
        else {
            return false;
        };
        if a.sections
            .iter()
            .any(|(k, _)| !k.set_equals(default_key) && k.count() > MAX_VALUES_PER_SECTION)
        {
            return false;
        }
        // The surrounding code forms a switch statement already.
        if a.contains_il_switch || self.match_roslyn_switch_on_string(f, ctx) {
            return true;
        }
        let if_count = a.inner_blocks.len() + 1;
        let interval_count: usize = a
            .sections
            .iter()
            .filter(|(k, _)| !k.set_equals(default_key))
            .map(|(k, _)| k.intervals().len())
            .sum();
        if if_count < interval_count {
            return false;
        }
        let root = a.root();
        let cfg = self
            .cfg
            .get_or_insert_with(|| ControlFlowGraph::new(f, self.container));
        let flow = analyze_control_flow(f, ctx, a, self.container, cfg);
        // One non-default label on a flat condition tree stays a condition (it may be a
        // loop condition such as `while (c == '\n' || c == '\r')`).
        if a.sections.len() == 2 && is_single_condition(f, cfg, &flow, root) {
            return false;
        }
        // A switch that needs goto statements was not one in the source.
        let Some(break_block) = switch_uses_goto(cfg, &flow) else {
            return false;
        };
        let Some(break_block) = break_block else {
            return true;
        };
        // The single break target has the highest IL offset of all targets.
        let max = a
            .sections
            .iter()
            .map(|&(_, body)| {
                f.match_branch(body)
                    .map_or(-1, |b| i64::from(f.il_range(b).start))
            })
            .max()
            .expect("sections");
        i64::from(f.il_range(break_block).start) >= max
    }

    /// `MatchRoslynSwitchOnString`: `stloc switchVar(call ComputeStringHash(ldloc v))` as the
    /// third-to-last instruction of the root.
    fn match_roslyn_switch_on_string(&self, f: &Function, ctx: &Context<'_, '_, '_>) -> bool {
        let list = f.list(self.analysis.root());
        list.len() >= 3
            && match_compute_string_or_read_only_span_hash_call(
                f,
                ctx,
                list[list.len() - 3],
                self.analysis.switch_variable(),
            )
            .is_some()
    }
}

/// `SwitchOnStringTransform.MatchComputeStringOrReadOnlySpanHashCall`: the `ldloc` hashed
/// into `target`.
pub fn match_compute_string_or_read_only_span_hash_call(
    f: &Function,
    ctx: &Context<'_, '_, '_>,
    inst: NodeId,
    target: crate::il::ast::VarId,
) -> Option<NodeId> {
    let (v, value) = f.match_stloc(inst)?;
    if v != target {
        return None;
    }
    let Inst::Call(c) = f.inst(value) else {
        return None;
    };
    let name = c.method.method.name.as_str();
    if let [arg] = c.args[..]
        && matches!(
            name,
            "ComputeStringHash" | "ComputeSpanHash" | "ComputeReadOnlySpanHash"
        )
        && ctx
            .ts
            .method_is_compiler_generated_or_in_compiler_generated_class(&c.method.method)
    {
        return f.match_ldloc(arg).map(|_| arg);
    }
    None
}

/// The flow nodes (the root, the inner blocks and a nullable check) and the case nodes (the
/// blocks the sections branch to) of the candidate switch, and its loop context.
struct Flow {
    flow_nodes: Vec<usize>,
    case_nodes: Vec<usize>,
    loops: LoopContext,
}

/// `AnalyzeControlFlow`.
fn analyze_control_flow(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    a: &SwitchAnalysis,
    container: NodeId,
    cfg: &ControlFlowGraph,
) -> Flow {
    let switch_head = cfg.node_of(a.root());
    let loops = LoopContext::new(f, cfg, switch_head);
    let mut flow_nodes = vec![switch_head];
    flow_nodes.extend(a.inner_blocks.iter().map(|&b| cfg.node_of(b)));
    let mut case_nodes = Vec::new();
    for &(_, body) in &a.sections {
        if let Some(block) = f.match_branch(body)
            && f.node(block).parent == Some(container)
        {
            let node = cfg.node_of(block);
            if !loops.match_continue(node) {
                case_nodes.push(node);
            }
        }
    }
    add_null_case(f, ctx, a, cfg, &mut flow_nodes, &case_nodes);
    Flow {
        flow_nodes,
        case_nodes,
        loops,
    }
}

/// `SwitchUsesGoto`: `None` when the switch needs goto statements, else its single break
/// block, if any.
fn switch_uses_goto(cfg: &ControlFlowGraph, flow: &Flow) -> Option<Option<NodeId>> {
    // Cases entered from outside the switch logic need `goto case`, or are a single break.
    let external: Vec<usize> = flow
        .case_nodes
        .iter()
        .copied()
        .filter(|&c| {
            cfg.nodes[c]
                .predecessors
                .iter()
                .any(|n| !flow.flow_nodes.contains(n))
        })
        .collect();
    if external.len() > 1 {
        return None;
    }
    let break_targets: HashSet<usize> = flow
        .case_nodes
        .iter()
        .filter(|c| !external.contains(c))
        .flat_map(|&n| flow.loops.break_targets(cfg, n))
        .collect();
    if break_targets.len() != 1 {
        return if break_targets.len() > 1 {
            None
        } else {
            Some(None)
        };
    }
    let break_block = cfg.nodes[*break_targets.iter().next().expect("one")].block;
    // The external case must be the break itself.
    if let [e] = external[..]
        && break_block != cfg.nodes[e].block
    {
        return None;
    }
    Some(Some(break_block))
}

/// `AddNullCase`: the `if (!nullable.HasValue) br nullCase; br root` block in front of the
/// root joins the flow nodes when its null case is a case node.
fn add_null_case(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    a: &SwitchAnalysis,
    cfg: &ControlFlowGraph,
    flow_nodes: &mut Vec<usize>,
    case_nodes: &[usize],
) {
    let root = a.root();
    if f.node(root).incoming_edge_count != 1 {
        return;
    }
    let [pred] = cfg.nodes[cfg.node_of(root)].predecessors[..] else {
        return;
    };
    let nullable_block = cfg.nodes[pred].block;
    let list = f.list(nullable_block);
    if list.len() < 2 || f.match_branch(list[list.len() - 1]) != Some(root) {
        return;
    }
    let Some((condition, true_inst)) = f.match_if_instruction(list[list.len() - 2]) else {
        return;
    };
    let Some(get_has_value) = f.match_logic_not(condition) else {
        return;
    };
    if match_has_value_call(f, ctx, get_has_value).is_none() {
        return;
    }
    let Some(null_block) = f.match_branch(true_inst) else {
        return;
    };
    if !case_nodes.iter().any(|&n| cfg.nodes[n].block == null_block) {
        return;
    }
    flow_nodes.push(cfg.node_of(nullable_block));
}

/// `NullableLiftingTransform.MatchHasValueCall`: the argument of `call get_HasValue`.
pub fn match_has_value_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let [arg] = c.args[..] else {
        return None;
    };
    if c.method.method.name != "get_HasValue" {
        return None;
    }
    let declaring = c.method.method.declaring?;
    (crate::il::transforms::il_inlining::known(ctx, declaring) == Some(KnownTypeCode::NullableOfT))
        .then_some(arg)
}

/// `IsFlowNode`: a block that starts with an if instruction.
fn is_flow_node(f: &Function, cfg: &ControlFlowGraph, n: usize) -> bool {
    f.list(cfg.nodes[n].block)
        .first()
        .is_some_and(|&i| matches!(f.inst(i), Inst::If { .. }))
}

/// `IsShortCircuit`: successor `side` of `parent` is a flow node that also reaches the other
/// successor, so `parent` and it form one `&&` or `||` condition.
fn is_short_circuit(f: &Function, cfg: &ControlFlowGraph, parent: usize, side: usize) -> bool {
    let node = cfg.nodes[parent].successors[side];
    let sibling = cfg.nodes[parent].successors[side ^ 1];
    let n = &cfg.nodes[node];
    if !is_flow_node(f, cfg, node) || n.successors.len() > 2 || n.predecessors.len() != 1 {
        return false;
    }
    n.successors.contains(&sibling)
}

/// `IsSingleCondition`: whether the flow nodes reduce to one short-circuit condition.
fn is_single_condition(f: &Function, cfg: &ControlFlowGraph, flow: &Flow, root: NodeId) -> bool {
    if flow.flow_nodes.len() == 1 {
        return true;
    }
    let root_node = cfg.node_of(root);
    let mut visited = vec![false; cfg.nodes.len()];
    visited[root_node] = true;
    // Down the tree, marking the nodes that continue the condition.
    let mut n = root_node;
    while !cfg.nodes[n].successors.is_empty() && (n == root_node || is_flow_node(f, cfg, n)) {
        let successors = &cfg.nodes[n].successors;
        if successors.len() == 1 {
            // With more than one case, a one-successor flow node is not part of the condition.
            if flow.case_nodes.len() > 1 {
                break;
            }
            n = successors[0];
        } else if is_short_circuit(f, cfg, n, 0) {
            n = successors[0];
        } else if is_short_circuit(f, cfg, n, 1) {
            n = successors[1];
        } else {
            break;
        }
        visited[n] = true;
        if flow.loops.match_continue(n) {
            break;
        }
    }
    flow.flow_nodes.iter().all(|&n| visited[n])
}

/// `LoopContext`: the continue targets of the loops around a node, by depth (1 for the
/// innermost loop). A branch to one of them becomes a `continue`.
struct LoopContext {
    continue_depth: HashMap<usize, usize>,
}

impl LoopContext {
    fn new(f: &Function, cfg: &ControlFlowGraph, context: usize) -> Self {
        // The loop heads: the nodes reachable from `context` that dominate it (`Analyze`, a
        // depth-first search that stops at them).
        let mut loop_heads = Vec::new();
        let mut visited = vec![false; cfg.nodes.len()];
        let mut stack: Vec<(usize, usize)> = Vec::new();
        let mut visit = |n: usize, stack: &mut Vec<(usize, usize)>, loop_heads: &mut Vec<usize>| {
            if visited[n] {
                return;
            }
            visited[n] = true;
            if cfg.dominates(n, context) {
                loop_heads.push(n);
            } else {
                stack.push((n, 0));
            }
        };
        for &s in &cfg.nodes[context].successors {
            visit(s, &mut stack, &mut loop_heads);
            while let Some(&(n, i)) = stack.last() {
                match cfg.nodes[n].successors.get(i) {
                    Some(&next) => {
                        stack.last_mut().expect("top").1 += 1;
                        visit(next, &mut stack, &mut loop_heads);
                    }
                    None => {
                        stack.pop();
                    }
                }
            }
        }
        loop_heads.sort_by_key(|&n| cfg.nodes[n].post_order_number);
        let mut continue_depth = HashMap::new();
        for (l, &head) in loop_heads.iter().enumerate() {
            continue_depth.insert(find_continue(f, cfg, head), l + 1);
        }
        LoopContext { continue_depth }
    }

    fn match_continue(&self, n: usize) -> bool {
        self.continue_depth.contains_key(&n)
    }

    /// `GetBreakTargets`: the successors that leave the dominator tree of `dominator` (with
    /// continue nodes cut off), once per edge; labelled continues count as breaks.
    fn break_targets(&self, cfg: &ControlFlowGraph, dominator: usize) -> Vec<usize> {
        let mut out = Vec::new();
        let mut stack = vec![dominator];
        while let Some(n) = stack.pop() {
            for &s in &cfg.nodes[n].successors {
                if !cfg.dominates(dominator, s) && self.continue_depth.get(&s) != Some(&1) {
                    out.push(s);
                }
            }
            if let Some(children) = &cfg.nodes[n].dominator_tree_children {
                stack.extend(children.iter().rev().filter(|&&c| !self.match_continue(c)));
            }
        }
        out
    }
}

/// `FindContinue`: the target of a `continue` of the loop at `head`: the increment block of a
/// for loop, the condition block of a do-while loop, else the head.
fn find_continue(f: &Function, cfg: &ControlFlowGraph, head: usize) -> usize {
    let mut preds = cfg.nodes[head]
        .predecessors
        .iter()
        .copied()
        .filter(|&p| p != head && cfg.dominates(head, p));
    let (Some(pred), None) = (preds.next(), preds.next()) else {
        return head;
    };
    let head_block = cfg.nodes[head].block;
    let pred_block = cfg.nodes[pred].block;
    let successors = cfg.nodes[pred].successors.len();
    if successors == 1 && match_increment_block(f, pred_block) == Some(head_block) {
        return pred;
    }
    if successors <= 2
        && let Some((t1, t2)) = match_do_while_condition_block(f, pred_block)
        && (t1 == Some(head_block) || t2 == Some(head_block))
    {
        return pred;
    }
    head
}

/// `HighLevelLoopTransform.MatchIncrementBlock`: simple statements and a branch to the loop
/// head.
fn match_increment_block(f: &Function, block: NodeId) -> Option<NodeId> {
    let (&last, rest) = f.list(block).split_last()?;
    let head = f.match_branch(last)?;
    // `IsSimpleStatement` also accepts the compound assignments, which the arena gains with
    // the transforms that create them.
    rest.iter()
        .all(|&i| {
            matches!(
                f.inst(i),
                Inst::Call(_)
                    | Inst::CallVirt(_)
                    | Inst::NewObj(_)
                    | Inst::StLoc { .. }
                    | Inst::StObj { .. }
            )
        })
        .then_some(head)
}

/// `HighLevelLoopTransform.MatchDoWhileConditionBlock`: `if (c) br t1; br t2`, where either
/// branch may be a return (`None`).
fn match_do_while_condition_block(
    f: &Function,
    block: NodeId,
) -> Option<(Option<NodeId>, Option<NodeId>)> {
    let list = f.list(block);
    if list.len() < 2 {
        return None;
    }
    let (_, true_inst) = f.match_if_instruction(list[list.len() - 2])?;
    let last = list[list.len() - 1];
    let target = |i: NodeId| match f.match_branch(i) {
        Some(b) => Some(Some(b)),
        None => f.match_return(i).map(|_| None),
    };
    Some((target(true_inst)?, target(last)?))
}

/// `InlineSwitchExpressionDefaultCaseThrowHelper`: the default case of a switch expression,
/// `call ThrowInvalidOperationException(); leave` or `call ThrowSwitchExpressionException(v);
/// leave`, becomes `throw newobj ...Exception(v)`, and a section with such a throw is marked
/// compiler-generated.
fn inline_switch_expression_default_case_throw_helper(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
) {
    // Blocks are basic blocks here, so a switch can only be the last instruction.
    let Some(&sw) = f.list(block).last() else {
        return;
    };
    if !matches!(f.inst(sw), Inst::Switch { .. }) || f.list(sw).is_empty() {
        return;
    }
    let invalid_operation = find_constructor(ctx, "System", "InvalidOperationException", false);
    let switch_expression = find_constructor(
        ctx,
        "System.Runtime.CompilerServices",
        "SwitchExpressionException",
        true,
    );
    if invalid_operation.is_none() && switch_expression.is_none() {
        return;
    }
    let default_section = default_section(f, sw);
    let Some(default_block) = f.match_branch(section_body(f, default_section)) else {
        return;
    };
    match *f.list(default_block) {
        [call, exit] if matches!(f.inst(exit), Inst::Branch { .. } | Inst::Leave { .. }) => {
            let Some(value) = match_switch_expression_throw_helper_call(ctx, f, call) else {
                return;
            };
            let Some(ctor) = (if value.is_none() {
                invalid_operation
            } else {
                switch_expression
            }) else {
                return;
            };
            let new_obj = f.add(Inst::NewObj(CallData {
                method: MethodOperand {
                    // Found by type lookup: the wire sends the constructor's signature.
                    token: 0,
                    method: ctor,
                    state_machine_context: false,
                },
                args: value.into_iter().collect(),
                is_tail: false,
                constrained_to: None,
                il_stack_was_empty: false,
            }));
            let throw = f.add(Inst::Throw { arg: new_obj });
            f.add_il_range(throw, f.il_range(call));
            f.add_il_range(throw, f.il_range(exit));
            f.replace(call, throw);
            f.remove_at(default_block, 1);
        }
        [throw] => {
            let Inst::Throw { arg } = *f.inst(throw) else {
                return;
            };
            let Inst::NewObj(c) = f.inst(arg) else {
                return;
            };
            if c.args.len() != 1
                || !switch_expression
                    .as_ref()
                    .is_some_and(|s| ctx.ts.same_method(&c.method.method, s))
            {
                return;
            }
        }
        _ => return,
    }
    if let Inst::SwitchSection {
        is_compiler_generated_default_section,
        ..
    } = f.inst_mut(default_section)
    {
        *is_compiler_generated_default_section = true;
    }
}

/// `SwitchInstruction.GetDefaultSection`: the first section with the most labels.
fn default_section(f: &Function, sw: NodeId) -> NodeId {
    let mut best = f.list(sw)[0];
    for &s in f.list(sw) {
        if labels(f, s).count() > labels(f, best).count() {
            best = s;
        }
    }
    best
}

/// `MatchSwitchExpressionThrowHelperCall`: a call of a `<PrivateImplementationDetails>` throw
/// helper; the switch value it passes, `None` for the parameterless one.
fn match_switch_expression_throw_helper_call(
    ctx: &Context<'_, '_, '_>,
    f: &Function,
    inst: NodeId,
) -> Option<Option<NodeId>> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let declaring = c.method.method.declaring?;
    if ctx.ts.full_name(declaring) != "<PrivateImplementationDetails>" {
        return None;
    }
    match (&c.args[..], c.method.method.name.as_str()) {
        ([], "ThrowInvalidOperationException") => Some(None),
        (&[value], "ThrowSwitchExpressionException") => Some(Some(value)),
        _ => None,
    }
}

/// `FindConstructor`: the instance constructor of the type without parameters, or with one
/// `object` parameter.
fn find_constructor(
    ctx: &mut Context<'_, '_, '_>,
    ns: &str,
    name: &str,
    object_parameter: bool,
) -> Option<ResolvedMethod> {
    let t = ctx.ts.find_top_level(TopLevelTypeName::new(ns, name, 0));
    let object = ctx.ts.known_type(KnownTypeCode::Object);
    let ctors = ctx
        .ts
        .get_constructors(t, MemberOptions::DECLARED)
        .unwrap_or_default();
    // shortcut: top-level nullability only, enough for `object`; deep IgnoreNullability when other parameter types are looked up
    ctors.into_iter().find(|m| {
        !m.is_static
            && match m.base_params[..] {
                [] => !object_parameter,
                [p] => {
                    object_parameter
                        && ctx.ts.change_nullability(p, Nullability::Oblivious) == object
                }
                _ => false,
            }
    })
}

fn labels(f: &Function, section: NodeId) -> &LongSet {
    match f.inst(section) {
        Inst::SwitchSection { labels, .. } => labels,
        other => panic!("{other:?} is not a switch section"),
    }
}

fn section_body(f: &Function, section: NodeId) -> NodeId {
    match f.inst(section) {
        Inst::SwitchSection { body, .. } => *body,
        other => panic!("{other:?} is not a switch section"),
    }
}

fn set_labels(f: &mut Function, section: NodeId, new: LongSet) {
    if let Inst::SwitchSection { labels, .. } = f.inst_mut(section) {
        *labels = new;
    }
}

/// `SimplifySwitchInstruction`: merges the sections that branch to the same block, moves a
/// constant offset of the switch value into the labels and sorts the sections.
pub fn simplify_switch_instruction(f: &mut Function, block: NodeId, ctx: &Context<'_, '_, '_>) {
    // Blocks are basic blocks here, so a switch can only be the last instruction.
    let Some(&sw) = f.list(block).last() else {
        return;
    };
    if !matches!(f.inst(sw), Inst::Switch { .. }) {
        return;
    }
    let mut primary: HashMap<NodeId, NodeId> = HashMap::new();
    let mut kept = Vec::new();
    for &section in &f.list(sw).to_vec() {
        if let Some(target) = f.match_branch(section_body(f, section)) {
            if let Some(&first) = primary.get(&target) {
                let merged = labels(f, first).union_with(labels(f, section));
                set_labels(f, first, merged);
                let null = matches!(
                    f.inst(section),
                    Inst::SwitchSection {
                        has_null_label: true,
                        ..
                    }
                );
                if let Inst::SwitchSection { has_null_label, .. } = f.inst_mut(first) {
                    *has_null_label |= null;
                }
                continue;
            }
            primary.insert(target, section);
        }
        kept.push(section);
    }
    // `RemoveAll`.
    let kept: std::collections::HashSet<NodeId> = kept.into_iter().collect();
    f.retain(sw, |_, s| kept.contains(&s));
    adjust_labels(f, sw);
    sort_switch_sections(f, sw, ctx);
}

fn switch_value(f: &Function, sw: NodeId) -> NodeId {
    match f.inst(sw) {
        Inst::Switch { value, .. } => *value,
        other => panic!("{other:?} is not a switch"),
    }
}

/// `AdjustLabels`.
fn adjust_labels(f: &mut Function, sw: NodeId) {
    let value = switch_value(f, sw);
    let Inst::BinaryNumeric {
        op,
        check_overflow: false,
        left,
        right,
        ..
    } = *f.inst(value)
    else {
        return;
    };
    let Some(val) = f.match_ldc_i(right) else {
        return;
    };
    let offset = match op {
        BinaryNumericOperator::Add => val.wrapping_neg(),
        BinaryNumericOperator::Sub => val,
        _ => return,
    };
    f.replace(value, left);
    for section in f.list(sw).to_vec() {
        let moved = labels(f, section).add_offset(offset);
        set_labels(f, section, moved);
    }
}

/// `SortSwitchSections`: a stable sort, as `OrderBy`.
fn sort_switch_sections(f: &mut Function, sw: NodeId, ctx: &Context<'_, '_, '_>) {
    let first_label = |f: &Function, s: NodeId| labels(f, s).values().next().unwrap_or(0);
    let mut sections = f.list(sw).to_vec();
    if ctx.settings.sort_switch_sections {
        sections.sort_by_key(|&s| first_label(f, s));
    } else {
        // `int?` keys: sections that neither branch nor leave sort first.
        let start = |f: &Function, s: NodeId| {
            let body = section_body(f, s);
            match f.inst(body) {
                Inst::Branch { .. } => f.match_branch(body).map(|b| f.il_range(b).start),
                Inst::Leave { .. } => Some(f.il_range(body).start),
                _ => None,
            }
        };
        sections.sort_by_key(|&s| (start(f, s), first_label(f, s)));
    }
    f.set_list(sw, sections);
}
