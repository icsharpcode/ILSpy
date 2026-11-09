//! `IL/Transforms/NamedArgumentTransform.cs`: an argument that would have to be evaluated
//! before the arguments in front of it becomes a named argument (`Block CallWithNamedArgs`),
//! so that the store of its value can be inlined into the call.

use crate::il::ast::{
    BlockKind, CompoundTargetKind, Function, Inst, NodeId, NopKind, VarId, VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::types::StackType;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::members::SymbolKind;
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{
    Find, find_load_in_next, inline_one_if_possible, options, options_for_block,
};
use super::statement_transform::StatementTransformContext;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.named_arguments {
        return;
    }
    let f = &mut r.function;
    let options = options_for_block(f, ctx, block, pos) | options::INTRODUCE_NAMED_ARGUMENTS;
    inline_one_if_possible(f, block, pos, options, ctx);
}

/// The method and arguments of a call instruction.
fn call_parts(f: &Function, call: NodeId) -> (&ResolvedMethod, &[NodeId]) {
    match f.inst(call) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => (&c.method.method, &c.args),
        _ => unreachable!("a call"),
    }
}

/// `CallInstruction.IsInstanceCall`.
fn is_instance_call(f: &Function, call: NodeId) -> bool {
    let (method, _) = call_parts(f, call);
    !method.is_static && !matches!(f.inst(call), Inst::NewObj(_))
}

/// The indexer an accessor belongs to (module and row).
fn indexer_of(
    ctx: &mut Context<'_, '_, '_>,
    method: &ResolvedMethod,
) -> Option<(crate::typesystem::compilation::ModuleId, u32)> {
    let MethodOrigin::Def(id) = method.origin else {
        return None;
    };
    let (token, _) = ctx.ts.accessor_owner(id)?;
    if token >> 24 != 0x17 {
        return None;
    }
    let row = token & 0x00FF_FFFF;
    ctx.ts
        .property_symbol_kind(id.module, row)
        .is_ok_and(|k| k == SymbolKind::Indexer)
        .then_some((id.module, row))
}

/// `AssignVariableNames.IsValidName`.
fn is_valid_name(name: &str) -> bool {
    let mut chars = name.chars();
    let Some(first) = chars.next() else {
        return false;
    };
    if name.trim().is_empty() || !(first.is_alphabetic() || first == '_') {
        return false;
    }
    chars.all(|c| c.is_alphanumeric() || c == '_')
}

/// `CanWriteArgumentNames`: whether every argument can be named (`CallBuilder` would emit the
/// reordered arguments positionally otherwise).
fn can_write_argument_names(
    ctx: &mut Context<'_, '_, '_>,
    method: &ResolvedMethod,
    is_setter: bool,
) -> bool {
    let MethodOrigin::Def(id) = method.origin else {
        // The parameters of a method decoded from a reference carry no names.
        return false;
    };
    // `CallBuilder.GetNamedParameters`: an indexer's parameters, else the method's.
    let names: Vec<String> = match indexer_of(ctx, method) {
        Some((module, row)) => ctx
            .ts
            .property_signature(module, row)
            .map(|(_, p)| p.into_iter().map(|p| p.name).collect())
            .unwrap_or_default(),
        None => ctx
            .ts
            .method_signature(id)
            .1
            .into_iter()
            .map(|p| p.name)
            .collect(),
    };
    let param_count = method.params().map_or(0, |p| p.len());
    if names.len() + usize::from(is_setter) < param_count {
        return false;
    }
    names.iter().all(|n| is_valid_name(n))
}

/// `CanIntroduceNamedArgument`: a load of `v` in an argument from `child` on, which a named
/// argument for that argument lets `moved` reach.
pub(crate) fn can_introduce_named_argument(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    call: NodeId,
    child: NodeId,
    v: VarId,
    moved: NodeId,
) -> Find {
    let child_index = f.node(child).child_index as usize;
    // A named argument cannot move anything before the this pointer.
    if is_instance_call(f, call) && child_index == 0 {
        return Find::Stop;
    }
    let (method, args) = call_parts(f, call);
    let (method, args) = (method.clone(), args.to_vec());
    // No named arguments for operators.
    if ctx.ts.method_is_operator(&method) {
        return Find::Stop;
    }
    if method.accessor_kind != 0 {
        // Only an indexer access has an argument list that can carry names.
        if indexer_of(ctx, &method).is_none() {
            return Find::Stop;
        }
        // A name replaces the call with a block: a call-inline-assign block is matched by the
        // call it holds, and a compound assignment needs a call as its target.
        let parent = f.node(call).parent;
        if parent.is_some_and(|p| {
            matches!(
                f.inst(p),
                Inst::Block {
                    kind: BlockKind::CallInlineAssign,
                    ..
                }
            )
        }) {
            return Find::Stop;
        }
        if let Some(p) = parent
            && let Inst::NumericCompoundAssign {
                target_kind: CompoundTargetKind::Property,
                target,
                ..
            }
            | Inst::UserDefinedCompoundAssign {
                target_kind: CompoundTargetKind::Property,
                target,
                ..
            }
            | Inst::DynamicCompoundAssign {
                target_kind: CompoundTargetKind::Property,
                target,
                ..
            } = *f.inst(p)
            && target == call
        {
            return Find::Stop;
        }
    }
    // CallBuilder supports no named arguments with varargs.
    if method.varargs.is_some() {
        return Find::Stop;
    }
    if method.name == ".ctor" && !method.is_static {
        let declaring = method.declaring;
        let is_delegate = declaring
            .and_then(|t| ctx.ts.definition(t))
            .and_then(|d| ctx.ts.comp.type_def(d).ok())
            .is_some_and(|i| i.kind == TypeKind::Delegate);
        let is_anonymous = declaring
            .and_then(|t| ctx.ts.definition(t))
            .is_some_and(|d| {
                super::transform_collection_and_object_initializers::is_anonymous_type(ctx, d)
            });
        if is_delegate || is_anonymous {
            return Find::Stop;
        }
    }
    // A setter's last argument is the assigned value, the right-hand side.
    let is_setter = method.accessor_kind == 0x01;
    if !can_write_argument_names(ctx, &method, is_setter) {
        return Find::Stop;
    }
    let nameable = args.len() - usize::from(is_setter);
    for &arg in args.iter().take(nameable).skip(child_index) {
        if let Find::Found(load) = find_load_in_next(f, ctx, Some(arg), v, moved, options::NONE) {
            return Find::NamedArgument {
                load,
                call_arg: arg,
            };
        }
    }
    Find::Stop
}

/// `CanExtendNamedArgument`: a load of `v` in a `CallWithNamedArgs` block that `moved` may
/// reach.
pub(crate) fn can_extend_named_argument(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    v: VarId,
    moved: NodeId,
) -> Find {
    let Inst::Block {
        ref instructions,
        final_inst: call,
        ..
    } = *f.inst(block)
    else {
        unreachable!("a block");
    };
    let instructions = instructions.clone();
    let first_arg = f
        .match_stloc(instructions[0])
        .expect("a named argument store")
        .1;
    let mut r = find_load_in_next(
        f,
        ctx,
        Some(first_arg),
        v,
        moved,
        options::INTRODUCE_NAMED_ARGUMENTS,
    );
    if matches!(r, Find::Found(_) | Find::NamedArgument { .. }) {
        // Into the first instruction of the block.
        return r;
    }
    if is_instance_call(f, call) {
        // The first instruction is the this argument; only position 1 can take a store.
        if r == Find::Stop {
            return Find::Stop;
        }
        // The load may be inside the second instruction.
        if let Some(&second) = instructions.get(1) {
            r = find_load_in_next(
                f,
                ctx,
                Some(second),
                v,
                moved,
                options::INTRODUCE_NAMED_ARGUMENTS,
            );
            if matches!(r, Find::Found(_) | Find::NamedArgument { .. }) {
                return r;
            }
        }
    }
    let (method, args) = call_parts(f, call);
    // A setter's last argument is the assigned value, the right-hand side.
    let nameable = args.len() - usize::from(method.accessor_kind == 0x01);
    for &arg in &args[..nameable] {
        if f.match_ldloc(arg) == Some(v) {
            return Find::NamedArgument {
                load: arg,
                call_arg: arg,
            };
        }
    }
    Find::Stop
}

/// `CallInstruction.ExpectedTypeForThisPointer`.
fn expected_type_for_this_pointer(
    ctx: &mut Context<'_, '_, '_>,
    declaring: TypeId,
    constrained: Option<TypeId>,
) -> StackType {
    if constrained.is_some() || matches!(ctx.ts.data(declaring), TypeData::TypeParam { .. }) {
        return StackType::Ref;
    }
    match ctx.ts.is_reference_type(declaring) {
        Some(true) => StackType::Obj,
        Some(false) => StackType::Ref,
        None => StackType::Unknown,
    }
}

/// `IntroduceNamedArgument`: `arg` becomes a named argument, evaluated before the other
/// arguments (but after the this pointer).
pub(crate) fn introduce_named_argument(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    arg: NodeId,
) {
    let call = f.node(arg).parent.expect("an argument of a call");
    let ty = f.infer_type(ctx.ts, arg);
    let v = f.register_variable(ctx.ts, VariableKind::NamedArgument, ty);
    let parent = f.node(call).parent.expect("a call with a parent");
    let named_arg_block = if matches!(
        f.inst(parent),
        Inst::Block {
            kind: BlockKind::CallWithNamedArgs,
            ..
        }
    ) {
        parent
    } else {
        let nop = f.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let block = f.add(Inst::Block {
            kind: BlockKind::CallWithNamedArgs,
            instructions: Vec::new(),
            final_inst: nop,
        });
        f.replace(call, block);
        f.set_child_at(block, 0, call);
        if is_instance_call(f, call) {
            let (constrained, declaring) = match f.inst(call) {
                Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
                    (c.constrained_to.map(|t| t.ty), c.method.method.declaring)
                }
                _ => unreachable!("a call"),
            };
            let declaring = declaring.unwrap_or_else(|| ctx.ts.intern(TypeData::UnknownSpecial));
            let mut this_var_type = constrained.unwrap_or(declaring);
            if expected_type_for_this_pointer(ctx, declaring, constrained) == StackType::Ref {
                this_var_type = ctx.ts.intern(TypeData::ByRef(this_var_type));
            }
            let this_arg_var = f.register_variable_named(
                ctx.ts,
                VariableKind::NamedArgument,
                this_var_type,
                Some("this_arg"),
            );
            let this_arg = call_parts(f, call).1[0];
            let store = f.add(Inst::StLoc {
                var: this_arg_var,
                value: this_arg,
                il_stack_was_empty: false,
                is_stack_adjustment: false,
            });
            f.push(block, store);
            let load = f.add(Inst::LdLoc { var: this_arg_var });
            f.set_child_at(call, 0, load);
        }
        block
    };
    let arg_index = f.node(arg).child_index as usize;
    let store = f.add(Inst::StLoc {
        var: v,
        value: arg,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    let insert_at = usize::from(is_instance_call(f, call));
    f.insert(named_arg_block, insert_at, store);
    let load = f.add(Inst::LdLoc { var: v });
    f.set_child_at(call, arg_index, load);
}
