//! `IL/Transforms/IndexRangeTransform.cs`, the part ExpressionTransforms calls on array
//! element addresses (`HandleLdElema`): `a[a.Length - i]` becomes `a[^i]` and
//! `a[index.GetOffset(a.Length)]` becomes `a[index]`.

use crate::il::ast::{
    BinaryNumericOperator, CallData, Function, Inst, MethodOperand, NodeId, TypeOperand,
};
use crate::il::types::StackType;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::resolve::ResolvedMethod;

use super::Context;
use super::il_inlining::known;

/// `IndexMethods`: the `System.Index` constructor, when the framework has what indices and
/// ranges need (`IsValid`).
fn index_ctor(ctx: &mut Context<'_, '_, '_>) -> Option<ResolvedMethod> {
    let is = |ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod, codes: &[KnownTypeCode]| {
        let params = m.params().unwrap_or_default();
        params.len() == codes.len()
            && params
                .iter()
                .zip(codes)
                .all(|(&p, &c)| known(ctx, p) == Some(c))
    };
    let index_type = ctx.ts.known_type(KnownTypeCode::Index);
    let mut index_ctor = None;
    for ctor in ctx
        .ts
        .get_constructors(index_type, MemberOptions::DECLARED)
        .unwrap_or_default()
    {
        if is(ctx, &ctor, &[KnownTypeCode::Int32, KnownTypeCode::Boolean]) {
            index_ctor = Some(ctor);
        }
    }
    let mut implicit_conv = false;
    for op in ctx
        .ts
        .get_methods(index_type, None, MemberOptions::default())
        .unwrap_or_default()
    {
        if op.name == "op_Implicit"
            && ctx.ts.method_is_operator(&op)
            && is(ctx, &op, &[KnownTypeCode::Int32])
        {
            implicit_conv = true;
        }
    }
    let range_type = ctx.ts.known_type(KnownTypeCode::Range);
    let mut range_ctor = false;
    for ctor in ctx
        .ts
        .get_constructors(range_type, MemberOptions::DECLARED)
        .unwrap_or_default()
    {
        if is(ctx, &ctor, &[KnownTypeCode::Index, KnownTypeCode::Index]) {
            range_ctor = true;
        }
    }
    index_ctor.filter(|_| implicit_conv && range_ctor)
}

/// Adds the IL ranges of `of` and its descendants (`Descendants`) to `to`.
fn absorb_il_ranges(f: &mut Function, to: NodeId, of: NodeId) {
    for node in f.descendants_post(of) {
        f.add_il_range(to, f.il_range(node));
    }
}

fn set_with_system_index(f: &mut Function, ldelema: NodeId) {
    if let Inst::LdElema {
        with_system_index, ..
    } = f.inst_mut(ldelema)
    {
        *with_system_index = true;
    }
}

/// `HandleLdElema`.
pub fn handle_ld_elema(f: &mut Function, ctx: &mut Context<'_, '_, '_>, ldelema: NodeId) -> bool {
    if !ctx.settings.ranges {
        return false;
    }
    let Inst::LdElema {
        array, ref indices, ..
    } = *f.inst(ldelema)
    else {
        return false;
    };
    let Some(array) = f.match_ldloc(array) else {
        return false;
    };
    // Indices and ranges have no multi-dimensional arrays.
    let [index] = indices[..] else {
        return false;
    };
    let is_array_length = |f: &Function, inst: NodeId| matches!(*f.inst(inst), Inst::LdLen { array: a, result: StackType::I4 } if f.match_ldloc(a) == Some(array));
    match f.inst(index) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)
            if c.method.method.name == "GetOffset"
                && c.method
                    .method
                    .declaring
                    .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Index)) =>
        {
            // ldelema T(ldloc array, call GetOffset(..., ldlen.i4(ldloc array)))
            // => withsystemindex.ldelema T(ldloc array, ...)
            let [index_ref, length] = c.args[..] else {
                return false;
            };
            let declaring = c.method.method.declaring.expect("a declaring type");
            if !is_array_length(f, length) {
                return false;
            }
            absorb_il_ranges(f, ldelema, length);
            f.add_il_range(ldelema, f.il_range(index));
            set_with_system_index(f, ldelema);
            // The call took the index by reference for `this`; the index is wanted by value.
            let value = f.add(Inst::LdObj {
                target: index_ref,
                ty: TypeOperand {
                    token: 0,
                    ty: declaring,
                },
                is_volatile: false,
                unaligned: 0,
            });
            f.replace(index, value);
            true
        }
        &Inst::BinaryNumeric {
            op: BinaryNumericOperator::Sub,
            check_overflow: false,
            is_lifted: false,
            left,
            right,
            ..
        } => {
            // ldelema T(ldloc array, binary.sub.i4(ldlen.i4(ldloc array), ...))
            // => withsystemindex.ldelema T(ldloc array, newobj System.Index(..., fromEnd: true))
            if !is_array_length(f, left) {
                return false;
            }
            // Without System.Index in the target framework, no index from the end.
            let Some(ctor) = index_ctor(ctx) else {
                return false;
            };
            absorb_il_ranges(f, ldelema, left);
            f.add_il_range(ldelema, f.il_range(index));
            set_with_system_index(f, ldelema);
            let one = f.add(Inst::LdcI4 { value: 1 });
            let new_index = f.add(Inst::NewObj(CallData {
                method: MethodOperand {
                    lifted: false,
                    // Found by type lookup: the wire sends the constructor's signature.
                    token: 0,
                    method: ctor,
                    state_machine_context: false,
                },
                args: vec![right, one],
                is_tail: false,
                constrained_to: None,
                il_stack_was_empty: false,
            }));
            f.replace(index, new_index);
            true
        }
        _ => false,
    }
}
