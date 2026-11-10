//! `IL/Transforms/IntroduceDynamicTypeOnLocals.cs`: an `object` local used as a dynamic operand
//! (or awaited dynamically) gets the type `dynamic`, and so do the other parts of its IL local.

use crate::il::ast::{AwaiterMethod, DynamicKind, Function, Inst, NodeId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeData;

use super::Context;
use super::il_inlining::known;
use super::local_function_decompiler::{all_functions, function_variables};

/// `CSharpArgumentInfoFlags.UseCompileTimeType`.
const USE_COMPILE_TIME_TYPE: i32 = 1;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    // `function.Descendants.OfType<ILFunction>()`: the nested functions, the top-level one last.
    let mut functions: Vec<Option<NodeId>> =
        all_functions(&r.function).into_iter().map(Some).collect();
    functions.push(None);
    let dynamic = ctx.ts.intern(TypeData::Dynamic);
    for function in functions {
        let variables = match function {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        let f = &mut r.function;
        let mut dynamic_indices: Vec<i32> = Vec::new();
        for &v in &variables {
            let var = f.var(v);
            if !matches!(
                var.kind,
                VariableKind::Local
                    | VariableKind::StackSlot
                    | VariableKind::ForeachLocal
                    | VariableKind::UsingLocal
            ) {
                continue;
            }
            if known(ctx, var.ty) != Some(KnownTypeCode::Object) || var.load_instructions.is_empty()
            {
                continue;
            }
            if var
                .load_instructions
                .iter()
                .any(|&load| is_dynamic_use(f, ctx, load))
            {
                let var = f.var_mut(v);
                var.ty = dynamic;
                if let Some(index) = var.index
                    && var.kind == VariableKind::Local
                {
                    dynamic_indices.push(index);
                }
            }
        }
        for v in variables {
            let var = f.var_mut(v);
            if var.kind == VariableKind::Local
                && var.index.is_some_and(|i| dynamic_indices.contains(&i))
            {
                var.ty = dynamic;
            }
        }
    }
}

/// A load as an operand the dynamic binder sees with its runtime type, or awaited dynamically.
fn is_dynamic_use(f: &Function, ctx: &mut Context<'_, '_, '_>, load: NodeId) -> bool {
    let Some(parent) = f.node(load).parent else {
        return false;
    };
    match f.inst(parent) {
        Inst::Dynamic {
            kind,
            ty,
            arg_infos,
            ..
        } => {
            let index = f.node(load).child_index as usize;
            // `GetArgumentInfoOfChild`.
            let flags = match kind {
                DynamicKind::Convert | DynamicKind::IsEvent => 0,
                DynamicKind::InvokeMember => {
                    info_flags(arg_infos, index + usize::from(ty.is_some()))
                }
                // The first info describes the constructed type.
                DynamicKind::InvokeConstructor => info_flags(arg_infos, index + 1),
                _ => info_flags(arg_infos, index),
            };
            flags & USE_COMPILE_TIME_TYPE == 0
        }
        Inst::Await {
            get_result: Some(AwaiterMethod::Dynamic(_)),
            ..
        } => true,
        Inst::Await {
            get_result: Some(AwaiterMethod::Method(m)),
            ..
        } => m
            .method
            .declaring
            .is_some_and(|d| matches!(ctx.ts.data(d), TypeData::Dynamic)),
        _ => false,
    }
}

fn info_flags(infos: &[crate::il::ast::DynamicArgInfo], index: usize) -> i32 {
    infos
        .get(index)
        .expect("GetArgumentInfoOfChild: an argument info for every argument")
        .flags
}
