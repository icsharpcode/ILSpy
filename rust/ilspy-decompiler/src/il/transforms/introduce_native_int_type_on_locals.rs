//! `IL/Transforms/IntroduceNativeIntTypeOnLocals.cs`: an `IntPtr` or `UIntPtr` local used or
//! assigned as a native integer gets the type `nint` or `nuint`, and so do the other parts of
//! its IL local.

use crate::il::ast::{Function, Inst, NodeId, VariableKind, binary_result_type};
use crate::il::reader::ReadFunction;
use crate::il::types::StackType;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::Context;
use super::il_inlining::known;
use super::local_function_decompiler::{all_functions, function_variables};

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.native_integers {
        return;
    }
    // `function.Descendants.OfType<ILFunction>()`: the nested functions, the top-level one last.
    let mut functions: Vec<Option<NodeId>> =
        all_functions(&r.function).into_iter().map(Some).collect();
    functions.push(None);
    let nint = ctx.ts.intern(TypeData::NInt);
    let nuint = ctx.ts.intern(TypeData::NUInt);
    for function in functions {
        let variables = match function {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        let f = &mut r.function;
        let mut mapping: Vec<(i32, TypeId)> = Vec::new();
        for &v in &variables {
            let var = f.var(v);
            if !matches!(
                var.kind,
                VariableKind::Local
                    | VariableKind::StackSlot
                    | VariableKind::PatternLocal
                    | VariableKind::ForeachLocal
                    | VariableKind::UsingLocal
            ) {
                continue;
            }
            let unsigned = match known(ctx, var.ty) {
                Some(KnownTypeCode::IntPtr) => false,
                Some(KnownTypeCode::UIntPtr) => true,
                _ => continue,
            };
            let used = var
                .load_instructions
                .iter()
                .any(|&load| is_used_as_native_int(f, ctx.ts, load));
            let assigned = var
                .store_instructions
                .iter()
                .any(|&store| is_native_int_store(f, ctx.ts, store));
            if used || assigned {
                let ty = if unsigned { nuint } else { nint };
                let var = f.var_mut(v);
                var.ty = ty;
                if var.kind == VariableKind::Local
                    && let Some(index) = var.index
                {
                    match mapping.iter_mut().find(|(i, _)| *i == index) {
                        Some(entry) => entry.1 = ty,
                        None => mapping.push((index, ty)),
                    }
                }
            }
        }
        for v in variables {
            let var = f.var_mut(v);
            if var.kind == VariableKind::Local
                && let Some(index) = var.index
                && let Some(&(_, ty)) = mapping.iter().find(|(i, _)| *i == index)
            {
                var.ty = ty;
            }
        }
    }
}

/// `IsCSharpNativeIntegerType`.
fn is_native_integer_type(ts: &TypeSystem<'_, '_>, t: TypeId) -> bool {
    matches!(ts.data(t), TypeData::NInt | TypeData::NUInt)
}

/// `IsUsedAsNativeInt`.
fn is_used_as_native_int(f: &Function, ts: &mut TypeSystem<'_, '_>, load: NodeId) -> bool {
    let Some(parent) = f.node(load).parent else {
        return false;
    };
    match f.inst(parent) {
        // `UnderlyingResultType`.
        Inst::BinaryNumeric {
            op,
            left_input,
            right_input,
            ..
        } => binary_result_type(*op, *left_input, *right_input) == StackType::I,
        Inst::BitNot {
            arg,
            lifted_underlying,
            ..
        } => lifted_underlying.unwrap_or_else(|| f.result_type(ts, *arg)) == StackType::I,
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
            // `GetParameter(load.ChildIndex)`.
            let first = usize::from(
                !c.method.method.is_static && !matches!(f.inst(parent), Inst::NewObj(_)),
            );
            let index = f.node(load).child_index as usize;
            let Some(index) = index.checked_sub(first) else {
                return false;
            };
            c.method
                .method
                .params()
                .ok()
                .and_then(|p| p.get(index).copied())
                .is_some_and(|t| is_native_integer_type(ts, t))
        }
        _ => false,
    }
}

/// `IsNativeIntStore`.
fn is_native_int_store(f: &Function, ts: &mut TypeSystem<'_, '_>, store: NodeId) -> bool {
    let Some((_, value)) = f.match_stloc(store) else {
        return false;
    };
    match f.inst(value) {
        Inst::BinaryNumeric {
            op,
            left_input,
            right_input,
            ..
        } if binary_result_type(*op, *left_input, *right_input) == StackType::I => true,
        Inst::Conv { .. } if f.result_type(ts, value) == StackType::I => true,
        _ => {
            let ty = f.infer_type(ts, value);
            is_native_integer_type(ts, ty)
        }
    }
}
