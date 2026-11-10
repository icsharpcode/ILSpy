//! `IL/Transforms/IndexRangeTransform.cs`: C# 8 indices and ranges. ExpressionTransforms
//! calls `handle_ld_elema` on array element addresses (`a[a.Length - i]` becomes `a[^i]`);
//! the statement transform turns the `GetOffset` and length arithmetic around an indexer or
//! a `Slice` call into `container[^i]` or `container[a..b]` (`SyntheticRangeIndexAccessor`).

use crate::il::ast::{
    BinaryNumericOperator, CallData, Function, Inst, MethodOperand, NodeId, RangeAccessor,
    TypeOperand, VarId,
};
use crate::il::types::StackType;
use crate::typesystem::compilation::ModuleId;
use crate::typesystem::inheritance::MemberDef;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::members::{MethodDefId, SymbolKind};
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::TypeId;

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
                    range_accessor: None,
                    specialized_by: None,
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

/// `IndexMethods`: the members of `System.Index` and `System.Range` the transform uses.
struct IndexMethods {
    index_ctor: Option<ResolvedMethod>,
    index_implicit_conv: Option<ResolvedMethod>,
    range_ctor: Option<ResolvedMethod>,
    range_start_at: Option<ResolvedMethod>,
    range_end_at: Option<ResolvedMethod>,
    range_get_all: Option<ResolvedMethod>,
}

impl IndexMethods {
    fn new(ctx: &mut Context<'_, '_, '_>) -> Self {
        let index_type = ctx.ts.known_type(KnownTypeCode::Index);
        let range_type = ctx.ts.known_type(KnownTypeCode::Range);
        let mut m = IndexMethods {
            index_ctor: None,
            index_implicit_conv: None,
            range_ctor: None,
            range_start_at: None,
            range_end_at: None,
            range_get_all: None,
        };
        for ctor in ctx
            .ts
            .get_constructors(index_type, MemberOptions::DECLARED)
            .unwrap_or_default()
        {
            if params_are(ctx, &ctor, &[KnownTypeCode::Int32, KnownTypeCode::Boolean]) {
                m.index_ctor = Some(ctor);
            }
        }
        for op in ctx
            .ts
            .get_methods(index_type, None, MemberOptions::default())
            .unwrap_or_default()
        {
            if op.name == "op_Implicit"
                && ctx.ts.method_is_operator(&op)
                && params_are(ctx, &op, &[KnownTypeCode::Int32])
            {
                m.index_implicit_conv = Some(op);
            }
        }
        for ctor in ctx
            .ts
            .get_constructors(range_type, MemberOptions::DECLARED)
            .unwrap_or_default()
        {
            if params_are(ctx, &ctor, &[KnownTypeCode::Index, KnownTypeCode::Index]) {
                m.range_ctor = Some(ctor);
            }
        }
        for method in ctx
            .ts
            .get_methods(range_type, None, MemberOptions::default())
            .unwrap_or_default()
        {
            if params_are(ctx, &method, &[KnownTypeCode::Index]) {
                match method.name.as_str() {
                    "StartAt" => m.range_start_at = Some(method),
                    "EndAt" => m.range_end_at = Some(method),
                    _ => {}
                }
            }
        }
        for p in ctx
            .ts
            .get_properties(range_type, MemberOptions::default())
            .unwrap_or_default()
        {
            let MemberDef::Property(module, row) = p.def else {
                continue;
            };
            let is_static = ctx.ts.member_modifiers(p.def).is_ok_and(|m| m.is_static);
            if !is_static || ctx.ts.member_name(p.def).ok().as_deref() != Some("All") {
                continue;
            }
            let getter = ctx.ts.comp.module(module).md.property_accessors(row).0;
            if getter != 0 {
                m.range_get_all = ctx
                    .ts
                    .method_definition(MethodDefId {
                        module,
                        row: getter,
                    })
                    .ok();
            }
        }
        m
    }

    fn is_valid(&self) -> bool {
        self.index_ctor.is_some() && self.index_implicit_conv.is_some() && self.range_ctor.is_some()
    }

    fn index_type(&self) -> Option<TypeId> {
        self.index_ctor.as_ref().and_then(|c| c.declaring)
    }

    fn range_type(&self) -> Option<TypeId> {
        self.range_ctor.as_ref().and_then(|c| c.declaring)
    }
}

/// Whether the parameters of `m` are exactly of the known types `codes`.
fn params_are(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod, codes: &[KnownTypeCode]) -> bool {
    let params = m.params().unwrap_or_default();
    params.len() == codes.len()
        && params
            .iter()
            .zip(codes)
            .all(|(&p, &c)| known(ctx, p) == Some(c))
}

/// `IndexMethods.IsRangeCtor`.
fn is_range_ctor(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    m.name == ".ctor"
        && !m.is_static
        && m.declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Range))
        && params_are(ctx, m, &[KnownTypeCode::Index, KnownTypeCode::Index])
}

fn operand(method: ResolvedMethod) -> MethodOperand {
    MethodOperand {
        token: 0,
        method,
        state_machine_context: false,
        lifted: false,
        range_accessor: None,
        specialized_by: None,
    }
}

fn new_call(f: &mut Function, method: ResolvedMethod, args: Vec<NodeId>, newobj: bool) -> NodeId {
    let data = CallData {
        method: operand(method),
        args,
        is_tail: false,
        constrained_to: None,
        il_stack_was_empty: false,
    };
    f.add(if newobj {
        Inst::NewObj(data)
    } else {
        Inst::Call(data)
    })
}

/// `IndexKind`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum IndexKind {
    /// An integer from the start of the container.
    FromStart,
    /// The address of a `System.Index`.
    RefSystemIndex,
    /// An integer from the end of the container.
    FromEnd,
    /// `0`, the start of a slice without a start point (`a[..end]`).
    TheStart,
    /// `^0`, the end of a slice without an end point (`a[start..]`).
    TheEnd,
}

/// The method and arguments of a call instruction.
fn call_parts(f: &Function, inst: NodeId) -> Option<(&MethodOperand, &[NodeId])> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => Some((&c.method, &c.args)),
        _ => None,
    }
}

/// `MatchContainerVar`: `ldloc` or `ldloca` of the container variable (which the first match
/// sets).
fn match_container_var(f: &Function, inst: NodeId, container_var: &mut Option<VarId>) -> bool {
    let v = match *f.inst(inst) {
        Inst::LdLoc { var } | Inst::LdLoca { var } => Some(var),
        _ => None,
    };
    match *container_var {
        Some(c) => v == Some(c),
        None => {
            *container_var = v;
            v.is_some()
        }
    }
}

/// The property a getter or setter belongs to.
fn accessor_property(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> Option<(ModuleId, u32)> {
    let MethodOrigin::Def(id) = m.origin else {
        return None;
    };
    let (token, _) = ctx.ts.accessor_owner(id)?;
    (token >> 24 == 0x17).then_some((id.module, token & 0x00FF_FFFF))
}

/// Whether `t` (or a base type) has a property named `name`.
fn has_property_named(ctx: &mut Context<'_, '_, '_>, t: TypeId, name: &str) -> bool {
    let properties = ctx
        .ts
        .get_properties(t, MemberOptions::default())
        .unwrap_or_default();
    properties
        .into_iter()
        .any(|p| ctx.ts.member_name(p.def).ok().as_deref() == Some(name))
}

/// `MatchContainerLength`: `ldloc lengthVar`, or `call get_Length/get_Count(ldloc container)`.
fn match_container_length(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    init: NodeId,
    length_var: Option<VarId>,
    container_var: &mut Option<VarId>,
) -> bool {
    if let Some(length_var) = length_var {
        return f.match_ldloc(init) == Some(length_var);
    }
    let Some((m, args)) = call_parts(f, init) else {
        return false;
    };
    let (m, args) = (m.clone(), args.to_vec());
    if f.result_type(ctx.ts, init) != StackType::I4 || m.method.accessor_kind != 0x02 {
        return false;
    }
    let Some((module, row)) = accessor_property(ctx, &m.method) else {
        return false;
    };
    let name = ctx
        .ts
        .member_name(MemberDef::Property(module, row))
        .unwrap_or_default();
    if name == "Count" {
        // Count works only for a type without Length.
        let declaring = m
            .method
            .declaring
            .expect("an accessor has a declaring type");
        if has_property_named(ctx, declaring, "Length") {
            return false;
        }
    }
    let property_type = ctx
        .ts
        .property_type(module, row, m.method.class_args.as_deref());
    if !property_type.is_some_and(|t| known(ctx, t) == Some(KnownTypeCode::Int32)) {
        return false;
    }
    let is_virtual = ctx
        .ts
        .member_modifiers(MemberDef::Property(module, row))
        .is_ok_and(|m| m.is_virtual);
    if is_virtual && !matches!(f.inst(init), Inst::CallVirt(_)) {
        return false;
    }
    let &[arg] = &args[..] else {
        return false;
    };
    match_container_var(f, arg, container_var)
}

/// `MatchContainerLengthStore`: `stloc lengthVar(call get_Length/get_Count(ldloc container))`.
fn match_container_length_store(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    container_var: &mut Option<VarId>,
) -> Option<VarId> {
    let (length_var, init) = f.match_stloc(inst)?;
    let v = f.var(length_var);
    if !(v.is_single_definition() && v.stack_type == StackType::I4) {
        return None;
    }
    if v.load_count() == 0 || v.load_count() > 2 {
        return None;
    }
    match_container_length(f, ctx, init, None, container_var).then_some(length_var)
}

/// `MatchGetOffset`: `call Index.GetOffset(indexLoad, length)` or `binary.sub.i4(length,
/// indexLoad)`; anything else is an integer from the start. Returns the kind and the index.
fn match_get_offset(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    length_var: Option<VarId>,
    container_var: &mut Option<VarId>,
) -> (IndexKind, NodeId) {
    if match_container_length(f, ctx, inst, length_var, container_var) {
        return (IndexKind::TheEnd, f.add(Inst::LdcI4 { value: 0 }));
    }
    if let Some((m, args)) = call_parts(f, inst) {
        let (m, args) = (m.clone(), args.to_vec());
        let is_get_offset = m.method.name == "GetOffset"
            && m.method
                .declaring
                .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Index));
        if !is_get_offset || args.len() != 2 {
            return (IndexKind::FromStart, inst);
        }
        if !match_container_length(f, ctx, args[1], length_var, container_var) {
            return (IndexKind::FromStart, inst);
        }
        return (IndexKind::RefSystemIndex, args[0]);
    }
    if let Inst::BinaryNumeric {
        op: BinaryNumericOperator::Sub,
        check_overflow,
        is_lifted,
        left,
        right,
        ..
    } = *f.inst(inst)
    {
        if check_overflow || f.result_type(ctx.ts, inst) != StackType::I4 || is_lifted {
            return (IndexKind::FromStart, inst);
        }
        if !match_container_length(f, ctx, left, length_var, container_var) {
            return (IndexKind::FromStart, inst);
        }
        return (IndexKind::FromEnd, right);
    }
    (IndexKind::FromStart, inst)
}

/// `MatchSliceLength`: `binary.sub.i4(<end offset>, ldloc startOffset)` (or without the
/// subtraction of 0 for `a[..end]`); the end kind and index.
fn match_slice_length(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    length_var: Option<VarId>,
    container_var: &mut Option<VarId>,
    start_offset_var: Option<VarId>,
) -> Option<(IndexKind, NodeId)> {
    if let Inst::BinaryNumeric {
        op: BinaryNumericOperator::Sub,
        check_overflow,
        is_lifted,
        left,
        right,
        ..
    } = *f.inst(inst)
    {
        if check_overflow || f.result_type(ctx.ts, inst) != StackType::I4 || is_lifted {
            return None;
        }
        let right_matches = match start_offset_var {
            // `a[..endIndex]`.
            None => f.match_ldc_i4(right) == Some(0),
            Some(s) => f.match_ldloc(right) == Some(s),
        };
        if !right_matches {
            return None;
        }
        return Some(match_get_offset(f, ctx, left, length_var, container_var));
    }
    // Without a start point, the compiler does not always emit the `- 0`.
    if start_offset_var.is_none() {
        return Some(match_get_offset(f, ctx, inst, length_var, container_var));
    }
    None
}

/// `CheckContainerLengthVariableUseCount`.
fn check_container_length_variable_use_count(
    f: &Function,
    length_var: Option<VarId>,
    start: IndexKind,
    end: IndexKind,
) -> bool {
    let uses = |k: IndexKind| u32::from(k != IndexKind::FromStart && k != IndexKind::TheStart);
    let expected = uses(start) + uses(end);
    match length_var {
        Some(v) => f.var(v).load_count() == expected,
        // One inline use.
        None => expected <= 1,
    }
}

/// `IsSlicingMethod`: `Slice(int, int)`, or `string.Substring(int, int)`.
fn is_slicing_method(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    let is_extension = match m.origin {
        MethodOrigin::Def(d) => {
            let owner = ctx.ts.accessor_owner(d).map(|(o, _)| o);
            ctx.ts.method_flags(d, owner).extension
        }
        _ => false,
    };
    if is_extension || !params_are(ctx, m, &[KnownTypeCode::Int32, KnownTypeCode::Int32]) {
        return false;
    }
    m.name == "Slice"
        || (m.name == "Substring"
            && m.declaring
                .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::String)))
}

/// `CSharpWillGenerateIndexer`: whether C# calls `container[int]` (or `Slice`) for
/// `container[Index]` (or `container[Range]`).
fn csharp_will_generate_indexer(
    ctx: &mut Context<'_, '_, '_>,
    declaring: TypeId,
    slicing: bool,
) -> bool {
    let (mut int32, mut index, mut range, mut count) = (false, false, false, false);
    for p in ctx
        .ts
        .get_properties(declaring, MemberOptions::default())
        .unwrap_or_default()
    {
        let MemberDef::Property(module, row) = p.def else {
            continue;
        };
        let is_indexer = ctx
            .ts
            .property_symbol_kind(module, row)
            .is_ok_and(|k| k == SymbolKind::Indexer);
        let name = ctx.ts.member_name(p.def).unwrap_or_default();
        let params = if is_indexer {
            ctx.ts
                .property_signature(module, row)
                .map(|(_, p)| p)
                .unwrap_or_default()
        } else {
            Vec::new()
        };
        if is_indexer && params.len() == 1 {
            match known(ctx, params[0].ty) {
                Some(KnownTypeCode::Int32) => int32 = true,
                Some(KnownTypeCode::Index) => index = true,
                Some(KnownTypeCode::Range) => range = true,
                _ => {}
            }
        } else if name == "Length" || name == "Count" {
            count = true;
        }
    }
    if slicing {
        count && !range
    } else {
        int32 && count && !index
    }
}

/// `MatchIndexImplicitConv`: the argument of `call Index.op_Implicit(int)`.
fn match_index_implicit_conv(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let (m, args) = call_parts(f, inst)?;
    let method = m.method.clone();
    let args = args.to_vec();
    if method.name != "op_Implicit" || !ctx.ts.method_is_operator(&method) {
        return None;
    }
    if !params_are(ctx, &method, &[KnownTypeCode::Int32]) {
        return None;
    }
    if !method
        .declaring
        .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Index))
    {
        return None;
    }
    let &[offset] = &args[..] else {
        return None;
    };
    Some(offset)
}

/// `MatchIndexFromRange`: `addressof System.Index(call get_Start/get_End(ldloca rangeVar))`.
fn match_index_from_range(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    kind: IndexKind,
    index_load: NodeId,
    range_var: VarId,
    accessor_name: &str,
) -> bool {
    if kind != IndexKind::RefSystemIndex {
        return false;
    }
    let Inst::AddressOf { value, ty } = *f.inst(index_load) else {
        return false;
    };
    if known(ctx, ty.ty) != Some(KnownTypeCode::Index) {
        return false;
    }
    let Inst::Call(c) = f.inst(value) else {
        return false;
    };
    let (name, declaring, args) = (
        c.method.method.name.clone(),
        c.method.method.declaring,
        c.args.clone(),
    );
    name == accessor_name
        && declaring.is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Range))
        && args.len() == 1
        && matches!(*f.inst(args[0]), Inst::LdLoca { var } if var == range_var)
}

/// `MakeIndex`.
fn make_index(f: &mut Function, kind: IndexKind, index_load: NodeId, m: &IndexMethods) -> NodeId {
    match kind {
        // complex_expr(call get_Item(ldloc container, ldobj startIndexLoad))
        IndexKind::RefSystemIndex => {
            let ty = m.index_type().expect("valid index methods");
            f.add(Inst::LdObj {
                target: index_load,
                ty: TypeOperand { token: 0, ty },
                is_volatile: false,
                unaligned: 0,
            })
        }
        // newobj System.Index(startIndexLoad, fromEnd: true)
        IndexKind::FromEnd | IndexKind::TheEnd => {
            let one = f.add(Inst::LdcI4 { value: 1 });
            let ctor = m.index_ctor.clone().expect("valid index methods");
            new_call(f, ctor, vec![index_load, one], true)
        }
        IndexKind::FromStart | IndexKind::TheStart => {
            let conv = m.index_implicit_conv.clone().expect("valid index methods");
            new_call(f, conv, vec![index_load], false)
        }
    }
}

/// `SyntheticRangeIndexAccessor`: `method` taking an index or a range in place of its integer
/// parameters.
fn synthetic_range_index_accessor(
    method: &MethodOperand,
    index_or_range_type: TypeId,
    slicing: bool,
) -> MethodOperand {
    let underlying_params = method.method.params().unwrap_or_default();
    let mut synthetic = method.method.clone();
    let mut params = vec![index_or_range_type];
    if !slicing {
        params.extend(underlying_params.iter().skip(1));
    }
    synthetic.base_params = params;
    synthetic.varargs = None;
    MethodOperand {
        method: synthetic,
        range_accessor: Some(Box::new(RangeAccessor {
            index_or_range_type,
            slicing,
            underlying_params,
        })),
        ..method.clone()
    }
}

/// The new call to the synthetic accessor in the place of `call`, with the IL ranges of the
/// call and of the statements it absorbs.
fn replace_with_synthetic_call(
    f: &mut Function,
    call: NodeId,
    method: MethodOperand,
    args: Vec<NodeId>,
    statements: &[NodeId],
) {
    let (kind, constrained_to, il_stack_was_empty) = match f.inst(call) {
        Inst::Call(c) => (0, c.constrained_to, c.il_stack_was_empty),
        Inst::CallVirt(c) => (1, c.constrained_to, c.il_stack_was_empty),
        Inst::NewObj(c) => (2, c.constrained_to, c.il_stack_was_empty),
        _ => unreachable!("a call"),
    };
    let data = CallData {
        method,
        args,
        is_tail: false,
        constrained_to,
        il_stack_was_empty,
    };
    let new_call = f.add(match kind {
        0 => Inst::Call(data),
        1 => Inst::CallVirt(data),
        _ => Inst::NewObj(data),
    });
    f.add_il_range(new_call, f.il_range(call));
    for &s in statements {
        f.add_il_range(new_call, f.il_range(s));
    }
    f.replace(call, new_call);
}

/// The state of one `Run`.
struct IndexRange {
    block: NodeId,
    start_pos: usize,
    pos: usize,
    container_var: Option<VarId>,
    container_length_var: Option<VarId>,
    range_var: Option<VarId>,
    range_var_init: Option<NodeId>,
}

impl IndexRange {
    /// `block.Instructions.RemoveRange(startPos, pos - startPos)`.
    fn remove_absorbed(&self, f: &mut Function) {
        for _ in self.start_pos..self.pos {
            f.remove_at(self.block, self.start_pos);
        }
    }

    fn absorbed(&self, f: &Function) -> Vec<NodeId> {
        f.list(self.block)[self.start_pos..self.pos].to_vec()
    }
}

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut crate::il::reader::ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut super::block_transform::BlockTransformContext,
    _sc: &mut super::statement_transform::StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.ranges {
        return;
    }
    let f = &mut r.function;
    let mut s = IndexRange {
        block,
        start_pos: pos,
        pos,
        container_var: None,
        container_length_var: None,
        range_var: None,
        range_var_init: None,
    };
    // The container length may be a separate instruction, or inline with its use.
    let first = f.list(block)[s.pos];
    match match_container_length_store(f, ctx, first, &mut s.container_var) {
        // stloc containerLengthVar(call get_Length/get_Count(ldloc container))
        Some(v) => {
            s.container_length_var = Some(v);
            s.pos += 1;
        }
        // A partial match: MatchGetOffset sets the container variable.
        None => s.container_var = None,
    }
    let Some(&inst) = f.list(block).get(s.pos) else {
        return;
    };
    if let Some((v, init)) = f.match_stloc(inst)
        && known(ctx, f.var(v).ty) == Some(KnownTypeCode::Range)
    {
        // stloc rangeVar(rangeVarInit)
        s.range_var = Some(v);
        s.range_var_init = Some(init);
        s.pos += 1;
    }
    let Some(&inst) = f.list(block).get(s.pos) else {
        return;
    };
    // stloc startOffsetVar(call GetOffset(startIndexLoad, ldloc length))
    let start = f
        .match_stloc(inst)
        .filter(|&(v, _)| f.var(v).is_single_definition() && f.var(v).stack_type == StackType::I4);
    let Some((start_offset_var, start_offset_var_init)) = start else {
        // Not the primary pattern, but maybe a partially transformed one to extend.
        extend_slicing(f, ctx, &mut s);
        return;
    };
    let (start_index_kind, start_index_load) = match_get_offset(
        f,
        ctx,
        start_offset_var_init,
        s.container_length_var,
        &mut s.container_var,
    );
    s.pos += 1;
    let start = Start {
        offset_var: Some(start_offset_var),
        offset_var_init: start_offset_var_init,
        kind: start_index_kind,
        load: start_index_load,
    };
    match f.var(start_offset_var).load_count() {
        1 => transform_indexing(f, ctx, &mut s, start),
        // Maybe slicing: once for the slice length, once for the Slice call.
        2 => transform_slicing(f, ctx, &mut s, start, false),
        _ => {}
    }
}

/// The start offset of a pattern: its variable (none for `a[..end]`), the variable's value,
/// and the kind and instruction of the start index.
#[derive(Clone, Copy)]
struct Start {
    offset_var: Option<VarId>,
    offset_var_init: NodeId,
    kind: IndexKind,
    load: NodeId,
}

/// `TransformIndexing`: `complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))`.
fn transform_indexing(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    s: &mut IndexRange,
    start: Start,
) {
    let start_offset_var = start.offset_var.expect("indexing has a start offset");
    if s.range_var.is_some() {
        return;
    }
    let load = f.var(start_offset_var).load_instructions[0];
    let Some(call) = f.node(load).parent.filter(|&p| call_parts(f, p).is_some()) else {
        return;
    };
    let (m, args) = call_parts(f, call).expect("a call");
    let (method, args) = (m.clone(), args.to_vec());
    let param_count = method.method.params().map_or(usize::MAX, |p| p.len());
    let accessor_kind = method.method.accessor_kind;
    if (accessor_kind == 0x02 && args.len() == 2) || (accessor_kind == 0x01 && args.len() == 3) {
        let is_indexer = accessor_property(ctx, &method.method).is_some_and(|(module, row)| {
            ctx.ts
                .property_symbol_kind(module, row)
                .is_ok_and(|k| k == SymbolKind::Indexer)
        });
        let expected_params = if accessor_kind == 0x02 { 1 } else { 2 };
        if !is_indexer || param_count != expected_params {
            return;
        }
    } else if is_slicing_method(ctx, &method.method) {
        // Without a start point, the slice length looks like the start offset.
        let zero = f.add(Inst::LdcI4 { value: 0 });
        let start = Start {
            offset_var: None,
            offset_var_init: start.offset_var_init,
            kind: IndexKind::TheStart,
            load: zero,
        };
        transform_slicing(f, ctx, s, start, true);
        return;
    } else {
        return;
    }
    // FromStart only matters for slicing; indexing from the start needs no System.Index.
    if start.kind == IndexKind::FromStart {
        return;
    }
    if !check_container_length_variable_use_count(
        f,
        s.container_length_var,
        start.kind,
        IndexKind::FromStart,
    ) {
        return;
    }
    let Some(&stmt) = f.list(s.block).get(s.pos) else {
        return;
    };
    if !is_descendant_of(f, call, stmt) {
        return;
    }
    // startOffsetVar may be used deep inside the statement; all of it must inline there.
    for i in s.start_pos..s.pos {
        let moved = f.list(s.block)[i];
        if !super::il_inlining::can_inline_into(f, ctx, stmt, start_offset_var, moved) {
            return;
        }
    }
    let params = method.method.params().unwrap_or_default();
    if !params
        .first()
        .is_some_and(|&p| known(ctx, p) == Some(KnownTypeCode::Int32))
    {
        return;
    }
    if !match_container_var(f, args[0], &mut s.container_var) {
        return;
    }
    if f.match_ldloc(args[1]) != Some(start_offset_var) {
        return;
    }
    let special = IndexMethods::new(ctx);
    if !special.is_valid() {
        return;
    }
    let Some(declaring) = method.method.declaring else {
        return;
    };
    if !csharp_will_generate_indexer(ctx, declaring, false) {
        return;
    }
    let index_type = special.index_type().expect("valid");
    let new_method = synthetic_range_index_accessor(&method, index_type, false);
    let index = make_index(f, start.kind, start.load, &special);
    let mut new_args = vec![args[0], index];
    new_args.extend(&args[2..]);
    let statements = s.absorbed(f);
    replace_with_synthetic_call(f, call, new_method, new_args, &statements);
    s.remove_absorbed(f);
}

/// `ILInstruction.IsDescendantOf` (including the node itself).
fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    std::iter::once(inst)
        .chain(f.ancestors(inst))
        .any(|n| n == ancestor)
}

/// `TransformSlicing`:
///
/// ```text
/// stloc containerLengthVar(call get_Length(ldloc containerVar))
/// stloc startOffset(call GetOffset(startIndexLoad, ldloc length))
/// stloc sliceLengthVar(binary.sub.i4(call GetOffset(endIndexLoad, ldloc length), ldloc startOffset))
/// complex_expr(call Slice(ldloc containerVar, ldloc startOffset, ldloc sliceLength))
/// ```
fn transform_slicing(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    s: &mut IndexRange,
    start: Start,
    slice_length_was_misdetected_as_start_offset: bool,
) {
    let (slice_length_var, slice_length_var_init) = if slice_length_was_misdetected_as_start_offset
    {
        // The store taken for the start offset stores the slice length (`a[..end]`).
        let store = f.list(s.block)[s.pos - 1];
        f.match_stloc(store).expect("a store")
    } else {
        let Some(&inst) = f.list(s.block).get(s.pos) else {
            return;
        };
        let Some(store) = f.match_stloc(inst) else {
            return;
        };
        s.pos += 1;
        store
    };
    let v = f.var(slice_length_var);
    if !(v.is_single_definition() && v.load_count() == 1) {
        return;
    }
    let Some((end_kind, end_load)) = match_slice_length(
        f,
        ctx,
        slice_length_var_init,
        s.container_length_var,
        &mut s.container_var,
        start.offset_var,
    ) else {
        return;
    };
    if !check_container_length_variable_use_count(f, s.container_length_var, start.kind, end_kind) {
        return;
    }
    // A range variable only appears in the second step (ExtendSlicing).
    if s.range_var.is_some() {
        return;
    }
    let load = f.var(slice_length_var).load_instructions[0];
    let Some(call) = f.node(load).parent.filter(|&p| call_parts(f, p).is_some()) else {
        return;
    };
    let Some(&stmt) = f.list(s.block).get(s.pos) else {
        return;
    };
    if !is_descendant_of(f, call, stmt) {
        return;
    }
    let (m, args) = call_parts(f, call).expect("a call");
    let (method, args) = (m.clone(), args.to_vec());
    if !is_slicing_method(ctx, &method.method) || args.len() != 3 {
        return;
    }
    if !match_container_var(f, args[0], &mut s.container_var) {
        return;
    }
    match start.offset_var {
        None => {
            if f.match_ldc_i4(args[1]) != Some(0) {
                return;
            }
        }
        Some(start_var) => {
            if f.match_ldloc(args[1]) != Some(start_var) {
                return;
            }
            if !super::il_inlining::can_move_into(f, ctx, start.offset_var_init, stmt, args[1]) {
                return;
            }
        }
    }
    if f.match_ldloc(args[2]) != Some(slice_length_var) {
        return;
    }
    if !super::il_inlining::can_move_into(f, ctx, slice_length_var_init, stmt, args[2]) {
        return;
    }
    let Some(declaring) = method.method.declaring else {
        return;
    };
    if !csharp_will_generate_indexer(ctx, declaring, true) {
        return;
    }
    let special = IndexMethods::new(ctx);
    if !special.is_valid() {
        return;
    }
    let range_type = special.range_type().expect("valid");
    let new_method = synthetic_range_index_accessor(&method, range_type, true);
    let range = make_range(f, s, start.kind, start.load, end_kind, end_load, &special);
    let statements = s.absorbed(f);
    replace_with_synthetic_call(f, call, new_method, vec![args[0], range], &statements);
    s.remove_absorbed(f);
}

/// `MakeRange`.
fn make_range(
    f: &mut Function,
    s: &IndexRange,
    start_kind: IndexKind,
    start_load: NodeId,
    end_kind: IndexKind,
    end_load: NodeId,
    m: &IndexMethods,
) -> NodeId {
    if s.range_var.is_some() {
        return s
            .range_var_init
            .expect("a range variable has an initializer");
    }
    if start_kind == IndexKind::TheStart
        && end_kind == IndexKind::TheEnd
        && let Some(get_all) = &m.range_get_all
    {
        return new_call(f, get_all.clone(), Vec::new(), false);
    }
    if start_kind == IndexKind::TheStart
        && let Some(end_at) = &m.range_end_at
    {
        let index = make_index(f, end_kind, end_load, m);
        return new_call(f, end_at.clone(), vec![index], false);
    }
    if end_kind == IndexKind::TheEnd
        && let Some(start_at) = &m.range_start_at
    {
        let index = make_index(f, start_kind, start_load, m);
        return new_call(f, start_at.clone(), vec![index], false);
    }
    let start = make_index(f, start_kind, start_load, m);
    let end = make_index(f, end_kind, end_load, m);
    let ctor = m.range_ctor.clone().expect("valid index methods");
    new_call(f, ctor, vec![start, end], true)
}

/// `ExtendSlicing`: a slicing that an earlier run transformed from a partial view of the
/// pattern (slicing from the end read as from the start) absorbs the container length store.
fn extend_slicing(f: &mut Function, ctx: &mut Context<'_, '_, '_>, s: &mut IndexRange) {
    let Some(length_var) = s.container_length_var else {
        return;
    };
    let first_load = f.var(length_var).load_instructions[0];
    let mut range_ctor_call = None;
    let ancestors: Vec<NodeId> = std::iter::once(first_load)
        .chain(f.ancestors(first_load))
        .collect();
    for inst in ancestors {
        if let Inst::NewObj(c) = f.inst(inst) {
            let method = c.method.method.clone();
            if is_range_ctor(ctx, &method) {
                range_ctor_call = Some(inst);
                break;
            }
        }
        if inst == s.block {
            break;
        }
    }
    let Some(range_ctor_call) = range_ctor_call else {
        return;
    };
    // The pattern TransformSlicing produced in the FromStart case.
    let Some(slicing_call) = f.node(range_ctor_call).parent else {
        return;
    };
    let Some((m, args)) = call_parts(f, slicing_call) else {
        return;
    };
    if m.range_accessor.is_none() {
        return;
    }
    let first_arg = args[0];
    if !match_container_var(f, first_arg, &mut s.container_var) {
        return;
    }
    let Some(&stmt) = f.list(s.block).get(s.pos) else {
        return;
    };
    if !is_descendant_of(f, slicing_call, stmt) {
        return;
    }
    let Inst::NewObj(c) = f.inst(range_ctor_call) else {
        unreachable!("a range constructor");
    };
    let (start_arg, end_arg) = (c.args[0], c.args[1]);
    let Some(start_offset_inst) = match_index_implicit_conv(f, ctx, start_arg) else {
        return;
    };
    let Some(end_offset_inst) = match_index_implicit_conv(f, ctx, end_arg) else {
        return;
    };
    let (start_kind, start_load) = match_get_offset(
        f,
        ctx,
        start_offset_inst,
        Some(length_var),
        &mut s.container_var,
    );
    let (end_kind, end_load) = match_get_offset(
        f,
        ctx,
        end_offset_inst,
        Some(length_var),
        &mut s.container_var,
    );
    if !check_container_length_variable_use_count(f, Some(length_var), start_kind, end_kind) {
        return;
    }
    if let Some(range_var) = s.range_var {
        let init = s
            .range_var_init
            .expect("a range variable has an initializer");
        if !super::il_inlining::can_move_into(f, ctx, init, stmt, start_load) {
            return;
        }
        if !match_index_from_range(f, ctx, start_kind, start_load, range_var, "get_Start") {
            return;
        }
        if !match_index_from_range(f, ctx, end_kind, end_load, range_var, "get_End") {
            return;
        }
    }
    let special = IndexMethods::new(ctx);
    if !special.is_valid() {
        return;
    }
    let range = make_range(f, s, start_kind, start_load, end_kind, end_load, &special);
    f.replace(range_ctor_call, range);
    for inst in s.absorbed(f) {
        f.add_il_range(slicing_call, f.il_range(inst));
    }
    s.remove_absorbed(f);
}
