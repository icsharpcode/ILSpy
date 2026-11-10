//! `IL/Transforms/TransformArrayInitializers.cs`: the stores that fill a new array (from
//! `RuntimeHelpers.InitializeArray`, element by element, or nested arrays) become one array
//! initializer block; the same for `stackalloc` buffers, and `ReadOnlySpan<T>` over field
//! data becomes an array initializer or a UTF-8 string literal.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, CallData, Function, Inst, MethodOperand, NodeId, NopKind,
    TypeOperand, VarId, VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::metadata::tables::rows;
use crate::typesystem::compilation::{ModuleId, TypeKind};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::{FieldOrigin, ResolvedField};
use crate::typesystem::types::{Nullability, TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{
    inline_one_if_possible, is_catch_when_block, is_in_constructor_initializer, known, options,
};
use super::statement_transform::StatementTransformContext;
use super::transform_assignment::{PointerOffset, pointer_offset_detect};

/// `FieldAttributes.HasFieldRVA`.
const HAS_FIELD_RVA: u16 = 0x0100;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.array_initializers {
        return;
    }
    let f = &mut r.function;
    if do_transform(f, ctx, block, pos) {
        return;
    }
    if do_transform_multi_dim(f, ctx, block, pos) {
        return;
    }
    if ctx.settings.stack_alloc_initializers
        && do_transform_stack_alloc_initializer(f, ctx, block, pos)
    {
        return;
    }
    do_transform_inline_runtime_helpers_initialize_array(f, ctx, block, pos);
}

fn ldc_i4(f: &mut Function, value: i32) -> NodeId {
    f.add(Inst::LdcI4 { value })
}

fn ldloc(f: &mut Function, var: VarId) -> NodeId {
    f.add(Inst::LdLoc { var })
}

fn stloc(f: &mut Function, var: VarId, value: NodeId) -> NodeId {
    f.add(Inst::StLoc {
        var,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    })
}

fn operand(ty: TypeId) -> TypeOperand {
    TypeOperand { token: 0, ty }
}

/// A new block of `kind` with `instructions` and `final_inst` (`nop` when `None`).
fn new_block(
    f: &mut Function,
    kind: BlockKind,
    instructions: Vec<NodeId>,
    final_inst: Option<NodeId>,
) -> NodeId {
    let final_inst = final_inst.unwrap_or_else(|| {
        f.add(Inst::Nop {
            kind: NopKind::Normal,
        })
    });
    f.add(Inst::Block {
        kind,
        instructions,
        final_inst,
    })
}

fn array_type(ctx: &mut Context<'_, '_, '_>, elem: TypeId, rank: u32) -> TypeId {
    ctx.ts.intern(TypeData::Array {
        elem,
        rank,
        nullability: Nullability::Oblivious,
    })
}

fn is_kind(ctx: &mut Context<'_, '_, '_>, t: TypeId, kind: TypeKind) -> bool {
    ctx.ts
        .definition(t)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == kind)
}

/// `DoTransform`: a one-dimensional `newarr` filled by `InitializeArray`, element stores, or
/// nested arrays.
fn do_transform(f: &mut Function, ctx: &mut Context<'_, '_, '_>, body: NodeId, pos: usize) -> bool {
    if pos as isize >= f.list(body).len() as isize - 2 {
        return false;
    }
    let inst = f.list(body)[pos];
    let Some((v, newarr)) = f.match_stloc(inst) else {
        return false;
    };
    let Some((element_type, array_length)) = match_new_arr(f, newarr) else {
        return false;
    };
    let v_type = f.var(v).ty;
    if let Some((mut values, init_array_pos)) = handle_runtime_helpers_initialize_array(
        f,
        ctx,
        body,
        pos + 1,
        v,
        element_type,
        &array_length,
    ) {
        let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, v_type);
        let mut initializer_instructions_to_remove = 1;
        let mut initializer_start = init_array_pos + 1;
        let mut simple_initializer_variable = v;
        let mut copy_instructions_to_remove = 0;
        // Legacy csc stores the initialized array into another local before writing the
        // non-constant element stores through that copy.
        if let Some(&next) = f.list(body).get(initializer_start)
            && let Some((copy, copied_value)) = f.match_stloc(next)
            && f.match_ldloc(copied_value) == Some(v)
        {
            simple_initializer_variable = copy;
            initializer_start += 1;
            copy_instructions_to_remove = 1;
        }
        let mut target_variable = v;
        if array_length.len() == 1
            && let Some((mixed, simple_to_remove)) = handle_simple_array_initializer(
                f,
                ctx,
                body,
                initializer_start,
                simple_initializer_variable,
                &array_length,
            )
        {
            target_variable = simple_initializer_variable;
            for (i, (_, value)) in mixed.iter().enumerate() {
                if let Some(value) = *value {
                    values[2 * i] = value;
                }
            }
            initializer_instructions_to_remove = 1 + copy_instructions_to_remove + simple_to_remove;
        }
        let block = block_from_initializer(
            f,
            ctx,
            temp_store,
            element_type,
            None,
            &array_length,
            &values,
        );
        let new_store = stloc(f, target_variable, block);
        f.set_child_at(body, pos, new_store);
        for _ in 0..initializer_instructions_to_remove {
            f.remove_at(body, init_array_pos);
        }
        inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
        return true;
    }
    if array_length.len() == 1 {
        if let Some((array_values, instructions_to_remove)) =
            handle_simple_array_initializer(f, ctx, body, pos + 1, v, &array_length)
        {
            let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, v_type);
            let block = build_simple_array_initializer_block(
                f,
                ctx,
                temp_store,
                element_type,
                &array_length,
                array_values,
            );
            let new_store = stloc(f, v, block);
            f.set_child_at(body, pos, new_store);
            for _ in 0..instructions_to_remove {
                f.remove_at(body, pos + 1);
            }
            inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
            return true;
        }
        if let Some((final_store, values, instructions_to_remove)) =
            handle_jagged_array_initializer(f, body, pos + 1, v, array_length[0])
        {
            let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, v_type);
            let values = values
                .into_iter()
                .enumerate()
                .map(|(i, value)| (vec![ldc_i4(f, i as i32)], Some(value)))
                .collect();
            let block = build_simple_array_initializer_block(
                f,
                ctx,
                temp_store,
                element_type,
                &array_length,
                values,
            );
            let new_store = stloc(f, final_store, block);
            f.set_child_at(body, pos, new_store);
            for _ in 0..instructions_to_remove {
                f.remove_at(body, pos + 1);
            }
            inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
            return true;
        }
    }
    false
}

/// `TransformSpanTArrayInitialization`: `newobj ReadOnlySpan<T>..ctor(ldsflda field, size)`
/// over field data.
pub(crate) fn transform_span_t_array_initialization(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    if !ctx.settings.array_initializers {
        return None;
    }
    let (element_type, declaring, field, size) =
        match_span_t_ctor_with_pointer_and_size(f, ctx, inst)?;
    if !has_field_rva(ctx, &field) {
        return None;
    }
    let initial_value = initial_value(ctx, &field)?;
    decode_array_initializer_or_utf8_string_literal(
        f,
        ctx,
        element_type,
        declaring,
        initial_value,
        size,
    )
}

/// `TransformRuntimeHelpersCreateSpanInitialization`: `call RuntimeHelpers.CreateSpan<T>(
/// ldmembertoken field)`.
pub(crate) fn transform_runtime_helpers_create_span_initialization(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    if !ctx.settings.array_initializers {
        return None;
    }
    let (element_type, field) = match_runtime_helpers_create_span(f, ctx, inst)?;
    if !has_field_rva(ctx, &field) {
        return None;
    }
    if is_sub_pattern_of_cpblk_initializer(f, ctx, inst) {
        return None;
    }
    let initial_value = initial_value(ctx, &field)?;
    let element_type_size = ctx.ts.type_size(element_type);
    if element_type_size <= 0 || initial_value.len() as i32 % element_type_size != 0 {
        return None;
    }
    let size = initial_value.len() as i32 / element_type_size;
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let ret = c.method.method.ret;
    decode_array_initializer_or_utf8_string_literal(f, ctx, element_type, ret, initial_value, size)
}

/// `IsSubPatternOfCpblkInitializer`: the `CreateSpan` is the source of a `cpblk` stackalloc
/// initializer (`call get_Item(addressof(call CreateSpan(...)), 0)`).
fn is_sub_pattern_of_cpblk_initializer(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
    let parent = |n: NodeId| f.node(n).parent;
    let Some(address_of) = parent(inst).filter(|&p| matches!(f.inst(p), Inst::AddressOf { .. }))
    else {
        return false;
    };
    let Some(get_item) = parent(address_of).filter(|&p| matches!(f.inst(p), Inst::Call(_))) else {
        return false;
    };
    if !parent(get_item).is_some_and(|p| matches!(f.inst(p), Inst::Cpblk { .. })) {
        return false;
    }
    match_get_static_field_address(f, ctx, get_item).is_some()
}

/// `DecodeArrayInitializerOrUTF8StringLiteral`.
fn decode_array_initializer_or_utf8_string_literal(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    element_type: TypeId,
    target_type: TypeId,
    initial_value: &[u8],
    size: i32,
) -> Option<NodeId> {
    if ctx.settings.utf8_string_literals
        && known(ctx, element_type) == Some(KnownTypeCode::Byte)
        && let Some(text) = decode_utf8_string(initial_value, size)
    {
        return Some(f.add(Inst::LdStrUtf8 { value: text }));
    }
    let values = decode_array_initializer(f, ctx, element_type, initial_value, &[size])?;
    let array = array_type(ctx, element_type, 1);
    let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, array);
    let mut op_implicit = None;
    if matches!(
        known(ctx, target_type),
        Some(KnownTypeCode::SpanOfT | KnownTypeCode::ReadOnlySpanOfT)
    ) {
        // The block builds an array where a Span<T>/ReadOnlySpan<T> is expected, so it ends
        // in the conversion the C# compiler would have applied.
        // shortcut: without a matching op_Implicit the managed code creates a fake method; the
        // span types always declare it, so the transform gives up instead.
        op_implicit = Some(
            ctx.ts
                .find_static_method(target_type, "op_Implicit", target_type, &[array])
                .ok()??,
        );
    }
    Some(block_from_initializer(
        f,
        ctx,
        temp_store,
        element_type,
        op_implicit,
        &[size],
        &values,
    ))
}

/// `DecodeUTF8String`: the text, if the bytes are UTF-8 that round-trips and has no control
/// characters (other than CR, LF, tab and a final null).
fn decode_utf8_string(blob: &[u8], size: i32) -> Option<String> {
    let size = usize::try_from(size).ok()?;
    if size > blob.len() {
        return None;
    }
    let has_implicit_null_terminator = blob.len() == size + 1 && blob[size] == 0;
    for (i, &val) in blob[..size].iter().enumerate() {
        if val == 0 && i == size - 1 && size > 1 {
            // An explicit null terminator.
        } else if !has_implicit_null_terminator
            && val < 0x20
            && !matches!(val, b'\r' | b'\n' | b'\t')
        {
            // Control characters: probably binary data, not a string.
            return None;
        }
    }
    let text = String::from_utf8_lossy(&blob[..size]).into_owned();
    // Only a literal that round-trips the data exactly.
    (text.as_bytes() == &blob[..size]).then_some(text)
}

/// `MatchSpanTCtorWithPointerAndSize`: the element type, the span type, the field and the
/// element count.
fn match_span_t_ctor_with_pointer_and_size(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, TypeId, ResolvedField, i32)> {
    let Inst::NewObj(c) = f.inst(inst) else {
        return None;
    };
    let ty = c.method.method.declaring?;
    if known(ctx, ty) != Some(KnownTypeCode::ReadOnlySpanOfT) {
        return None;
    }
    let &[pointer, size] = &c.args[..] else {
        return None;
    };
    let TypeData::Inst { args, .. } = ctx.ts.data(ty) else {
        return None;
    };
    let &[element_type] = &args[..] else {
        return None;
    };
    let pointer = super::expression_transforms::unwrap_conv(
        f,
        pointer,
        crate::il::ast::ConversionKind::StopGCTracking,
    );
    let Inst::LdsFlda { field } = f.inst(pointer) else {
        return None;
    };
    if field.token == 0 {
        return None;
    }
    let size = f.match_ldc_i4(size)?;
    Some((element_type, ty, field.field.clone(), size))
}

/// `MatchRuntimeHelpersCreateSpan`: the element type and the field.
fn match_runtime_helpers_create_span(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, ResolvedField)> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    if !is_runtime_helpers(ctx, c.method.method.declaring?) {
        return None;
    }
    let &[arg] = &c.args[..] else {
        return None;
    };
    if c.method.method.name != "CreateSpan" {
        return None;
    }
    let &[element_type] = &c.method.method.type_args[..] else {
        return None;
    };
    let arg = super::expression_transforms::unwrap_conv(
        f,
        arg,
        crate::il::ast::ConversionKind::StopGCTracking,
    );
    let field = match_ld_member_token_field(f, arg)?;
    Some((element_type, field))
}

/// `MatchLdMemberToken` of a field with a metadata token.
fn match_ld_member_token_field(f: &Function, inst: NodeId) -> Option<ResolvedField> {
    match f.inst(inst) {
        Inst::LdMemberToken {
            member: crate::il::ast::MemberOperand::Field(field),
        } if field.token != 0 => Some(field.field.clone()),
        _ => None,
    }
}

/// `IsRuntimeHelpers`.
fn is_runtime_helpers(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    ctx.ts.type_parameter_count(t) == 0
        && ctx.ts.full_name(t) == "System.Runtime.CompilerServices.RuntimeHelpers"
}

/// The field row in the main module (`context.PEFile.Metadata.GetFieldDefinition(token)`).
fn field_row(field: &ResolvedField) -> Option<u32> {
    match field.origin {
        FieldOrigin::Def { row, .. } => Some(row),
        FieldOrigin::Fake => None,
    }
}

/// `field.HasFlag(FieldAttributes.HasFieldRVA)`.
fn has_field_rva(ctx: &mut Context<'_, '_, '_>, field: &ResolvedField) -> bool {
    let Some(row) = field_row(field) else {
        return false;
    };
    let md = &ctx.ts.comp.module(ModuleId(0)).md;
    md.get::<rows::Field>(row)
        .is_ok_and(|f| f.flags & HAS_FIELD_RVA != 0)
}

/// `FieldValueSizeDecoder`: the size of the data a field of type `t` has.
fn field_value_size(ctx: &mut Context<'_, '_, '_>, t: TypeId, pointer_size: i32) -> i32 {
    match ctx.ts.data(t).clone() {
        TypeData::Modified { inner, .. } | TypeData::Pinned(inner) => {
            field_value_size(ctx, inner, pointer_size)
        }
        TypeData::Pointer(_) | TypeData::ByRef(_) | TypeData::FnPtr(_) => pointer_size,
        TypeData::NInt | TypeData::NUInt => pointer_size,
        TypeData::Array { .. } | TypeData::TypeParam { .. } | TypeData::Dynamic => 0,
        TypeData::Inst { generic, .. } => field_value_size(ctx, generic, pointer_size),
        _ => match known(ctx, t) {
            Some(KnownTypeCode::Boolean | KnownTypeCode::Byte | KnownTypeCode::SByte) => 1,
            Some(KnownTypeCode::Char | KnownTypeCode::Int16 | KnownTypeCode::UInt16) => 2,
            Some(KnownTypeCode::Int32 | KnownTypeCode::UInt32 | KnownTypeCode::Single) => 4,
            Some(KnownTypeCode::Int64 | KnownTypeCode::UInt64 | KnownTypeCode::Double) => 8,
            Some(KnownTypeCode::IntPtr | KnownTypeCode::UIntPtr) => pointer_size,
            Some(
                KnownTypeCode::Object
                | KnownTypeCode::String
                | KnownTypeCode::TypedReference
                | KnownTypeCode::Void,
            ) => 0,
            _ => match ctx.ts.definition(t) {
                Some(def) => ctx
                    .ts
                    .comp
                    .module(def.module)
                    .md
                    .type_layout(def.row)
                    .map_or(0, |(_, size)| size as i32),
                None => 0,
            },
        },
    }
}

/// `FieldDefinition.GetInitialValue`: the field's data.
// shortcut: data that the image does not hold makes the managed code throw
// BadImageFormatException (reported as a warning only by InitializeArray); the port gives up
// on the transform.
fn initial_value<'m>(ctx: &mut Context<'_, '_, 'm>, field: &ResolvedField) -> Option<&'m [u8]> {
    let row = field_row(field)?;
    let comp = ctx.ts.comp;
    let md = &comp.module(ModuleId(0)).md;
    if md.get::<rows::Field>(row).ok()?.flags & HAS_FIELD_RVA == 0 {
        return Some(&[]);
    }
    let rva = md.field_rva(row).ok()?;
    if rva == 0 {
        return Some(&[]);
    }
    let pe = md.pe.as_ref();
    let pointer_size = match pe {
        Some(pe) if !pe.is_pe32_plus => 4,
        _ => 8,
    };
    let size = field_value_size(ctx, field.ty, pointer_size);
    let section = pe?.section_data(rva).ok()?;
    if section.is_empty() && size != 0 {
        return None;
    }
    let size = usize::try_from(size).ok()?;
    section.get(..size)
}

/// `DoTransformMultiDim`.
fn do_transform_multi_dim(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    body: NodeId,
    pos: usize,
) -> bool {
    if pos as isize >= f.list(body).len() as isize - 2 {
        return false;
    }
    let inst = f.list(body)[pos];
    let Some((v, newarr)) = f.match_stloc(inst) else {
        return false;
    };
    let Some((element_type, length)) = match_new_arr(f, newarr) else {
        return false;
    };
    if let Some((values, init_array_pos)) =
        handle_runtime_helpers_initialize_array(f, ctx, body, pos + 1, v, element_type, &length)
    {
        let block = block_from_initializer(f, ctx, v, element_type, None, &length, &values);
        let new_store = stloc(f, v, block);
        f.replace(inst, new_store);
        f.remove_at(body, init_array_pos);
        inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
        return true;
    }
    if let Some((array_values, instructions_to_remove)) =
        handle_simple_array_initializer(f, ctx, body, pos + 1, v, &length)
    {
        let v_type = f.var(v).ty;
        let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, v_type);
        let block = build_simple_array_initializer_block(
            f,
            ctx,
            temp_store,
            element_type,
            &length,
            array_values,
        );
        let new_store = stloc(f, v, block);
        f.set_child_at(body, pos, new_store);
        for _ in 0..instructions_to_remove {
            f.remove_at(body, pos + 1);
        }
        inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
        return true;
    }
    false
}

/// `PointerType.ElementType` of a pointer type.
fn pointer_element(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<TypeId> {
    match *ctx.ts.data(t) {
        TypeData::Pointer(elem) => Some(elem),
        _ => None,
    }
}

/// `DoTransformStackAllocInitializer`.
fn do_transform_stack_alloc_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    body: NodeId,
    pos: usize,
) -> bool {
    if pos as isize >= f.list(body).len() as isize - 2 {
        return false;
    }
    let inst = f.list(body)[pos];
    let Some((v, localloc)) = f.match_stloc(inst) else {
        return false;
    };
    let Inst::LocAlloc { arg: length_inst } = *f.inst(localloc) else {
        return false;
    };
    if let Some(length_in_bytes) = f.match_ldc_i(length_inst)
        && let Some((blob, element_type)) =
            handle_cpblk_initializer(f, ctx, body, pos + 1, v, length_in_bytes)
    {
        let pointer = ctx.ts.intern(TypeData::Pointer(element_type));
        let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, pointer);
        let mut instructions = vec![stloc(f, temp_store, localloc)];
        let mut offset = 0;
        while offset < blob.len() {
            let Some((value, read)) = read_element(f, ctx, &blob[offset..], element_type) else {
                return false;
            };
            instructions.push(st_elem_ptr(
                f,
                ctx,
                temp_store,
                offset as i32,
                value,
                element_type,
            ));
            offset += read;
        }
        let final_inst = ldloc(f, temp_store);
        let block = new_block(
            f,
            BlockKind::StackAllocInitializer,
            instructions,
            Some(final_inst),
        );
        let new_store = stloc(f, v, block);
        f.set_child_at(body, pos, new_store);
        f.remove_at(body, pos + 1);
        inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
        let statement = f.list(body)[pos];
        super::expression_transforms::run_on_single_statement(f, ctx, statement);
        return true;
    }
    if let Some((element_type, values, instructions_to_remove)) =
        handle_sequential_loc_alloc_initializer(f, ctx, body, pos + 1, v, localloc)
    {
        let pointer = ctx.ts.intern(TypeData::Pointer(element_type));
        let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, pointer);
        let mut instructions = vec![stloc(f, temp_store, localloc)];
        for value in values {
            instructions.push(rewrap_store(f, ctx, temp_store, value));
        }
        let final_inst = ldloc(f, temp_store);
        let block = new_block(
            f,
            BlockKind::StackAllocInitializer,
            instructions,
            Some(final_inst),
        );
        let new_store = stloc(f, v, block);
        f.set_child_at(body, pos, new_store);
        for _ in 0..instructions_to_remove {
            f.remove_at(body, pos + 1);
        }
        inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
        let statement = f.list(body)[pos];
        super::expression_transforms::run_on_single_statement(f, ctx, statement);
        return true;
    }
    false
}

/// `ReadElement`: the constant at the start of `blob` and how many bytes it takes.
fn read_element(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    blob: &[u8],
    element_type: TypeId,
) -> Option<(NodeId, usize)> {
    let bytes = |n: usize| blob.get(..n);
    // Floating-point elements are read as their own type: their bytes are not the integer of
    // the same width, and the constant has to have the element type's stack type.
    match known(ctx, element_type) {
        Some(KnownTypeCode::Single) => {
            let bits = u32::from_le_bytes(bytes(4)?.try_into().ok()?);
            return Some((f.add(Inst::LdcF4 { bits }), 4));
        }
        Some(KnownTypeCode::Double) => {
            let bits = u64::from_le_bytes(bytes(8)?.try_into().ok()?);
            return Some((f.add(Inst::LdcF8 { bits }), 8));
        }
        _ => {}
    }
    // shortcut: an element size other than 1, 2, 4 or 8 makes the managed code throw
    // NotSupportedException; the port gives up on the transform.
    match ctx.ts.type_size(element_type) {
        1 => Some((ldc_i4(f, i32::from(bytes(1)?[0])), 1)),
        2 => {
            let value = i16::from_le_bytes(bytes(2)?.try_into().ok()?);
            Some((ldc_i4(f, i32::from(value)), 2))
        }
        4 => {
            let value = i32::from_le_bytes(bytes(4)?.try_into().ok()?);
            Some((ldc_i4(f, value), 4))
        }
        8 => {
            let value = i64::from_le_bytes(bytes(8)?.try_into().ok()?);
            Some((f.add(Inst::LdcI8 { value }), 8))
        }
        _ => None,
    }
}

/// `HandleCpblkInitializer`: `cpblk(ldloc v, field address, length)` and the store of the
/// buffer; the field data and the element type.
fn handle_cpblk_initializer<'m>(
    f: &mut Function,
    ctx: &mut Context<'_, '_, 'm>,
    block: NodeId,
    pos: usize,
    v: VarId,
    length: i64,
) -> Option<(&'m [u8], TypeId)> {
    let Inst::Cpblk {
        dest, source, size, ..
    } = *f.inst(f.list(block)[pos])
    else {
        return None;
    };
    if f.match_ldloc(dest) != Some(v) || f.match_ldc_i4(size) != Some(length as i32) {
        return None;
    }
    let field = match_get_static_field_address(f, ctx, source)?;
    let var = f.var(v);
    if !(var.is_single_definition() && var.load_count() == 2) {
        return None;
    }
    if field_token(f, ctx, source) == 0 {
        return None;
    }
    let next = *f.list(block).get(pos + 1)?;
    let (final_store, value) = match f.match_stloc(next) {
        Some(store) => store,
        None => {
            // The other use of the buffer moves out into a store of its own.
            let other_load = f.var(v).load_instructions.iter().copied().find(|&l| {
                !f.node(l)
                    .parent
                    .is_some_and(|p| matches!(f.inst(p), Inst::Cpblk { .. }))
            })?;
            let parent = f.node(other_load).parent?;
            let final_store = super::il_extraction::extract(f, ctx, parent)?;
            let store = f.var(final_store).store_instructions[0];
            (final_store, f.match_stloc(store)?.1)
        }
    };
    if !has_field_rva(ctx, &field) {
        return None;
    }
    let element_type = if f.match_ldloc(value) == Some(v) {
        let ty = f.var(final_store).ty;
        pointer_element(ctx, ty)?
    } else if let Inst::NewObj(c) = f.inst(value)
        && let &[first, count] = &c.args[..]
        && let Some(declaring) = c.method.method.declaring
        && known(ctx, declaring) == Some(KnownTypeCode::SpanOfT)
        && f.match_ldloc(first) == Some(v)
        && let Some(element_count) = f.match_ldc_i4(count)
    {
        let TypeData::Inst { args, .. } = ctx.ts.data(declaring) else {
            return None;
        };
        let element_type = *args.first()?;
        let element_size = ctx.ts.type_size(element_type);
        if element_size == 0 || i64::from(element_count) != length / i64::from(element_size) {
            return None;
        }
        element_type
    } else {
        return None;
    };
    Some((initial_value(ctx, &field)?, element_type))
}

/// The metadata token of the field `MatchGetStaticFieldAddress` finds in `inst`.
fn field_token(f: &Function, _ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> u32 {
    match f.inst(inst) {
        Inst::LdsFlda { field } => field.token,
        Inst::Call(c) => {
            let Some(&Inst::AddressOf { value, .. }) = c.args.first().map(|&a| f.inst(a)) else {
                return 0;
            };
            match f.inst(value) {
                Inst::Call(create_span) => match create_span.args.first().map(|&a| f.inst(a)) {
                    Some(Inst::LdMemberToken {
                        member: crate::il::ast::MemberOperand::Field(field),
                    }) => field.token,
                    _ => 0,
                },
                _ => 0,
            }
        }
        _ => 0,
    }
}

/// `MatchGetStaticFieldAddress`: `ldsflda field`, or
/// `call get_Item(addressof ReadOnlySpan<T>(call CreateSpan(ldmembertoken field)), ldc.i4 0)`.
fn match_get_static_field_address(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    input: NodeId,
) -> Option<ResolvedField> {
    if let Inst::LdsFlda { field } = f.inst(input) {
        return Some(field.field.clone());
    }
    let Inst::Call(call) = f.inst(input) else {
        return None;
    };
    if call.method.method.name != "get_Item" || call.args.len() != 2 {
        return None;
    }
    if known(ctx, call.method.method.declaring?) != Some(KnownTypeCode::ReadOnlySpanOfT) {
        return None;
    }
    if f.match_ldc_i4(call.args[1]) != Some(0) {
        return None;
    }
    let Inst::AddressOf { value, .. } = *f.inst(call.args[0]) else {
        return None;
    };
    let Inst::Call(create_span) = f.inst(value) else {
        return None;
    };
    if create_span.method.method.name != "CreateSpan" || create_span.args.len() != 1 {
        return None;
    }
    if !is_runtime_helpers(ctx, create_span.method.method.declaring?) {
        return None;
    }
    match f.inst(create_span.args[0]) {
        Inst::LdMemberToken {
            member: crate::il::ast::MemberOperand::Field(field),
        } => Some(field.field.clone()),
        _ => None,
    }
}

/// `HandleSequentialLocAllocInitializer`: the element type, the element stores and how many
/// instructions they take.
fn handle_sequential_loc_alloc_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    mut pos: usize,
    store: VarId,
    loc_alloc: NodeId,
) -> Option<(TypeId, Vec<NodeId>, usize)> {
    let mut element_count = 0;
    let mut min_expected_offset: i64 = 0;
    let mut values: Option<Vec<Option<NodeId>>> = None;
    let mut element_type: Option<TypeId> = None;
    let mut instructions_to_remove = 0;
    let Inst::LocAlloc {
        arg: length_instruction,
    } = *f.inst(loc_alloc)
    else {
        return None;
    };
    let mut blob: Option<&[u8]> = None;
    if let Some(byte_count) = f.match_ldc_i(length_instruction) {
        // An initblk or cpblk of another shape stays in place; the element scan then fails to
        // match it and rejects the transform.
        handle_loc_alloc_initializer_prefix(
            f,
            ctx,
            block,
            &mut pos,
            store,
            byte_count,
            &mut blob,
            &mut instructions_to_remove,
        );
    }
    let count_of = |f: &Function, ctx: &mut Context<'_, '_, '_>, elem: TypeId| -> Option<i64> {
        match pointer_offset_detect(f, ctx, length_instruction, elem, true, false)? {
            PointerOffset::Constant(c) => Some(i64::from(c)),
            PointerOffset::Inst(n) => f.match_ldc_i(n),
        }
    };
    let mut i = pos;
    while i < f.list(block).len() {
        let inst = f.list(block)[i];
        let Inst::StObj {
            target,
            value,
            ty: current_type,
            ..
        } = *f.inst(inst)
        else {
            break;
        };
        let current_type = current_type.ty;
        if f.descendants_post(value)
            .into_iter()
            .any(|n| f.variable_of(n) == Some(store))
        {
            break;
        }
        match element_type {
            None => {
                element_type = Some(current_type);
                if let Some(blob) = blob {
                    let values_length = count_of(f, ctx, current_type)?;
                    if values_length < 1 {
                        return None;
                    }
                    let mut list = vec![None; values_length as usize];
                    let mut offset = 0;
                    let mut value_index = 0;
                    while offset < blob.len() && value_index < list.len() {
                        let (constant, read) = read_element(f, ctx, &blob[offset..], current_type)?;
                        list[value_index] = Some(st_elem_ptr(
                            f,
                            ctx,
                            store,
                            offset as i32,
                            constant,
                            current_type,
                        ));
                        offset += read;
                        value_index += 1;
                    }
                    values = Some(list);
                }
            }
            Some(e) if e != current_type => break,
            Some(_) => {}
        }
        let element = element_type.expect("set above");
        match get_sequential_store_offset(f, ctx, target, store, element, &mut min_expected_offset)
        {
            SequentialStoreMatch::Abort => return None,
            SequentialStoreMatch::SequenceEnd => break,
            SequentialStoreMatch::Matched => {}
        }
        if values.is_none() {
            let values_length = count_of(f, ctx, element)?;
            if values_length < 1 {
                return None;
            }
            values = Some(vec![None; values_length as usize]);
        }
        let list = values.as_mut().expect("set above");
        if min_expected_offset >= list.len() as i64 {
            break;
        }
        list[min_expected_offset as usize] = Some(inst);
        element_count += 1;
        i += 1;
    }
    // Every element is written, from the data blob or by a store; a gap means a partially
    // initialized buffer, which an initializer cannot express.
    let values = values?;
    let var = f.var(store);
    if values.iter().any(Option::is_none)
        || var.kind != VariableKind::StackSlot
        || var.store_count() != 1
        || var.address_count() != 0
        || var.load_count() as usize > values.len() + 1
    {
        return None;
    }
    let mut element_type = element_type?;
    if let Some(&last_load) = var.load_instructions.last()
        && let Some(parent) = f.node(last_load).parent
        && let Some((final_store, _)) = f.match_stloc(parent)
    {
        let ty = f.var(final_store).ty;
        element_type = pointer_element(ctx, ty)?;
    }
    instructions_to_remove += element_count;
    let values: Vec<NodeId> = values.into_iter().flatten().collect();
    (element_count <= values.len()).then_some((element_type, values, instructions_to_remove))
}

/// `HandleLocAllocInitializerPrefix`: an `initblk` or a `cpblk` of field data that fills the
/// whole buffer before the element stores.
#[allow(clippy::too_many_arguments)]
fn handle_loc_alloc_initializer_prefix<'m>(
    f: &Function,
    ctx: &mut Context<'_, '_, 'm>,
    block: NodeId,
    pos: &mut usize,
    store: VarId,
    byte_count: i64,
    blob: &mut Option<&'m [u8]>,
    instructions_to_remove: &mut usize,
) {
    let Some(&inst) = f.list(block).get(*pos) else {
        return;
    };
    match *f.inst(inst) {
        // initblk(ldloc store, value, byteCount)
        Inst::Initblk { address, size, .. } => {
            if f.match_ldloc(address) != Some(store) || f.match_ldc_i(size) != Some(byte_count) {
                return;
            }
            *instructions_to_remove += 1;
            *pos += 1;
        }
        // cpblk(ldloc store, ldsflda/call get_Item(CreateSpan(field)) data, byteCount)
        Inst::Cpblk {
            dest, source, size, ..
        } => {
            if f.match_ldloc(dest) != Some(store) || f.match_ldc_i(size) != Some(byte_count) {
                return;
            }
            let Some(field) = match_get_static_field_address(f, ctx, source) else {
                return;
            };
            if field_token(f, ctx, source) == 0 || !has_field_rva(ctx, &field) {
                return;
            }
            let Some(data) = initial_value(ctx, &field) else {
                return;
            };
            *blob = Some(data);
            *instructions_to_remove += 1;
            *pos += 1;
        }
        _ => {}
    }
}

/// `SequentialStoreMatch`.
enum SequentialStoreMatch {
    /// The store belongs to the initializer sequence.
    Matched,
    /// The store is not part of the sequence; it ends the scan, keeping what was matched.
    SequenceEnd,
    /// The store's shape is unexpected; the whole transform is rejected.
    Abort,
}

/// `GetSequentialStoreOffset`.
fn get_sequential_store_offset(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    target: NodeId,
    store: VarId,
    element_type: TypeId,
    min_expected_offset: &mut i64,
) -> SequentialStoreMatch {
    // stobj T(ldloc store, value) writes at the current expected offset, initially 0.
    if f.match_ldloc(target) == Some(store) {
        return SequentialStoreMatch::Matched;
    }
    // stobj T(binary.add(ldloc store, offset), value): the offset is sizeof(T) or an element
    // index times sizeof(T).
    let Inst::BinaryNumeric {
        op: BinaryNumericOperator::Add,
        left,
        right,
        check_overflow,
        ..
    } = *f.inst(target)
    else {
        return SequentialStoreMatch::Abort;
    };
    if f.match_ldloc(left) != Some(store) {
        return SequentialStoreMatch::SequenceEnd;
    }
    let offset = match pointer_offset_detect(f, ctx, right, element_type, check_overflow, false) {
        None => return SequentialStoreMatch::Abort,
        Some(PointerOffset::Constant(c)) => Some(i64::from(c)),
        Some(PointerOffset::Inst(n)) => f.match_ldc_i(n),
    };
    match offset {
        Some(offset) if offset >= 0 && offset >= *min_expected_offset => {
            *min_expected_offset = offset;
            SequentialStoreMatch::Matched
        }
        _ => SequentialStoreMatch::SequenceEnd,
    }
}

/// `RewrapStore`: the element store with `target` in place of the buffer variable.
fn rewrap_store(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    target: VarId,
    store: NodeId,
) -> NodeId {
    let Inst::StObj {
        target: old_target,
        value,
        ty,
        ..
    } = *f.inst(store)
    else {
        unreachable!("an element store");
    };
    let target_inst = match *f.inst(old_target) {
        Inst::LdLoc { .. } => ldloc(f, target),
        Inst::BinaryNumeric {
            op: BinaryNumericOperator::Add,
            right,
            check_overflow,
            sign,
            ..
        } => {
            let left = ldloc(f, target);
            new_binary_add(f, ctx, left, right, check_overflow, sign)
        }
        _ => panic!("This should never happen: Bug in HandleSequentialLocAllocInitializer!"),
    };
    f.add(Inst::StObj {
        target: target_inst,
        value,
        ty,
        is_volatile: false,
        unaligned: 0,
    })
}

/// `new BinaryNumericInstruction(Add, left, right, checkForOverflow, sign)`.
fn new_binary_add(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    left: NodeId,
    right: NodeId,
    check_overflow: bool,
    sign: Sign,
) -> NodeId {
    let left_input = f.result_type(ctx.ts, left);
    let right_input = f.result_type(ctx.ts, right);
    f.add(Inst::BinaryNumeric {
        op: BinaryNumericOperator::Add,
        check_overflow,
        sign,
        left_input,
        right_input,
        left,
        right,
        csharp_result_type: None,
        is_lifted: false,
    })
}

/// `StElemPtr`: `stobj T(ldloc target + offset, value)`.
fn st_elem_ptr(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    target: VarId,
    offset: i32,
    value: NodeId,
    ty: TypeId,
) -> NodeId {
    let target_inst = if offset == 0 {
        ldloc(f, target)
    } else {
        let left = ldloc(f, target);
        let constant = ldc_i4(f, offset);
        let right = f.add(Inst::Conv {
            arg: constant,
            input: StackType::I4,
            input_sign: Sign::Signed,
            target: PrimitiveType::I,
            check_overflow: false,
            is_lifted: false,
        });
        new_binary_add(f, ctx, left, right, false, Sign::None)
    };
    f.add(Inst::StObj {
        target: target_inst,
        value,
        ty: operand(ty),
        is_volatile: false,
        unaligned: 0,
    })
}

/// The indices and values (`None` for a default element) of an array initializer.
type ElementValues = Vec<(Vec<NodeId>, Option<NodeId>)>;

/// `HandleSimpleArrayInitializer`: the element stores after `pos` that fill `store` (an
/// array of `array_length`), as indices and values (`None` for a default element), and how
/// many instructions they take.
pub(crate) fn handle_simple_array_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
    store: VarId,
    array_length: &[i32],
) -> Option<(ElementValues, usize)> {
    let mut instructions_to_remove = 0;
    let mut element_count = 0;
    // The dimensions come from the IL and need not multiply within the int range; this is
    // only a capacity hint, so it saturates.
    let length = array_length.iter().fold(1i32, |t, &l| {
        (i64::from(t) * i64::from(l)).min(i64::from(i32::MAX)) as i32
    });
    let mut values: Vec<(Vec<NodeId>, Option<NodeId>)> = Vec::new();
    let mut next_minimum_index = vec![0i32; array_length.len()];
    // `CalculateNextIndices`: the indices of the next element and whether `indices` (when
    // given) are exactly those; None when `indices` are out of range or go back.
    let calculate_next_indices = |f: &mut Function,
                                  next_minimum_index: &mut Vec<i32>,
                                  indices: Option<&[NodeId]>|
     -> Option<(Vec<NodeId>, bool)> {
        let mut next_indices = Vec::with_capacity(array_length.len());
        let mut exact_match = true;
        match indices {
            None => {
                for &index in next_minimum_index.iter() {
                    next_indices.push(ldc_i4(f, index));
                }
            }
            Some(indices) => {
                let mut previous_component_was_greater = false;
                for (k, &i) in indices.iter().enumerate() {
                    let index = f.match_ldc_i4(i)?;
                    // In range, and not before the next minimum index (so that nothing is
                    // reordered or overwritten); skipped slots get default values.
                    if index < 0
                        || index >= array_length[k]
                        || (!previous_component_was_greater && index < next_minimum_index[k])
                    {
                        return None;
                    }
                    next_indices.push(ldc_i4(f, next_minimum_index[k]));
                    if index != next_minimum_index[k] {
                        exact_match = false;
                        // [3, 3] is before [4, 0] even though 3 > 0.
                        if index > next_minimum_index[k] {
                            previous_component_was_greater = true;
                        }
                    }
                }
            }
        }
        for k in (0..next_minimum_index.len()).rev() {
            next_minimum_index[k] += 1;
            if next_minimum_index[k] < array_length[k] || k == 0 {
                break;
            }
            next_minimum_index[k] = 0;
        }
        Some((next_indices, exact_match))
    };
    let mut i = pos;
    while i < f.list(block).len() {
        let inst = f.list(block)[i];
        // stobj elementType(ldelema elementType(ldloc store, indices), value)
        let (indices, value, step) = if let Inst::StObj { target, value, .. } = *f.inst(inst) {
            match f.inst(target) {
                Inst::LdElema { array, indices, .. } if f.match_ldloc(*array) == Some(store) => {
                    (indices.clone(), value, 1)
                }
                _ => break,
            }
        } else if let Some((address_temporary, address_value)) = f.match_stloc(inst) {
            // stloc s(ldelema elementType(ldloc store, indices)); stobj elementType(ldloc s, value)
            let t = f.var(address_temporary);
            if !(t.is_single_definition() && t.load_count() == 1) {
                break;
            }
            let Inst::LdElema { array, indices, .. } = f.inst(address_value) else {
                break;
            };
            if f.match_ldloc(*array) != Some(store) {
                break;
            }
            let indices = indices.clone();
            let Some(&next) = f.list(block).get(i + 1) else {
                break;
            };
            let Inst::StObj { target, value, .. } = *f.inst(next) else {
                break;
            };
            if f.match_ldloc(target) != Some(address_temporary) {
                break;
            }
            (indices, value, 2)
        } else {
            break;
        };
        if f.descendants_post(value)
            .into_iter()
            .any(|n| f.variable_of(n) == Some(store))
        {
            break;
        }
        if indices.len() != array_length.len() {
            break;
        }
        if length <= 0 {
            break;
        }
        loop {
            let (next_indices, exact) =
                calculate_next_indices(f, &mut next_minimum_index, Some(&indices))?;
            if exact {
                values.push((next_indices, Some(value)));
                element_count += 1;
                instructions_to_remove += step;
            } else {
                values.push((next_indices, None));
            }
            if !(values.len() < length as usize && !exact) {
                break;
            }
        }
        i += step;
    }
    if let Some(&next) = f.list(block).get(i)
        && let Inst::StObj { target, .. } = *f.inst(next)
        && let Inst::LdElema { array, .. } = *f.inst(target)
        && f.match_ldloc(array) == Some(store)
    {
        // An element is modified right after the initializer: no partial initializers.
        return None;
    }
    if pos + instructions_to_remove >= f.list(block).len() {
        return None;
    }
    if element_count == 0 {
        return None;
    }
    let first = f.list(block)[pos];
    let must_transform =
        is_catch_when_block(f, block) || is_in_constructor_initializer(f, ctx, first);
    // Too few user-defined elements: no initializer (which also avoids huge allocations).
    if element_count < length / 3 - 5 {
        if !must_transform {
            return None;
        }
        // .NET does not allow arrays of 2 GB or more.
        if length == i32::MAX {
            return None;
        }
    }
    for _ in values.len()..length.max(0) as usize {
        let (next_indices, _) = calculate_next_indices(f, &mut next_minimum_index, None)?;
        values.push((next_indices, None));
    }
    Some((values, instructions_to_remove))
}

/// `HandleJaggedArrayInitializer`: the variable the array ends in, the nested initializers
/// and how many instructions they take.
fn handle_jagged_array_initializer(
    f: &Function,
    block: NodeId,
    mut pos: usize,
    store: VarId,
    length: i32,
) -> Option<(VarId, Vec<NodeId>, usize)> {
    let mut instructions_to_remove = 0;
    let mut values = Vec::new();
    for i in 0..length {
        // An optional temporary copy of the store.
        // shortcut: running out of instructions makes the managed code throw; the port gives
        // up on the transform.
        let inst = *f.list(block).get(pos)?;
        let copy = f
            .match_stloc(inst)
            .filter(|&(_, load)| f.match_ldloc(load) == Some(store))
            .map(|(temp, _)| temp);
        let initializer = match copy {
            Some(temp) => match_jagged_array_store(f, block, pos + 1, temp, i)?,
            None => match_jagged_array_store(f, block, pos, store, i)?,
        };
        values.push(initializer);
        let inc = if copy.is_some() { 3 } else { 2 };
        pos += inc;
        instructions_to_remove += inc;
    }
    // An extra copy of the store variable is removed and its value used instead.
    let inst = *f.list(block).get(pos)?;
    let final_store = if let Some((final_store, array)) = f.match_stloc(inst) {
        if f.match_ldloc(array) != Some(store) {
            return None;
        }
        instructions_to_remove += 1;
        final_store
    } else {
        store
    };
    Some((final_store, values, instructions_to_remove))
}

/// `MatchJaggedArrayStore`: `stloc initializerStore(block); stobj(ldelema(ldloc store,
/// ldc.i4 index), ldloc initializerStore)`; the nested initializer block.
fn match_jagged_array_store(
    f: &Function,
    block: NodeId,
    pos: usize,
    store: VarId,
    index: i32,
) -> Option<NodeId> {
    let final_instruction = *f.list(block).get(pos + 1)?;
    let Inst::StObj {
        target: temp_access,
        value: temp_array_load,
        ..
    } = *f.inst(final_instruction)
    else {
        return None;
    };
    let initializer_store = f.match_ldloc(temp_array_load)?;
    let Inst::LdElema { array, indices, .. } = f.inst(temp_access) else {
        return None;
    };
    if f.match_ldloc(*array) != Some(store)
        || indices.len() != 1
        || f.match_ldc_i4(indices[0]) != Some(index)
    {
        return None;
    }
    let next_instruction = *f.list(block).get(pos)?;
    let (v, initializer) = f.match_stloc(next_instruction)?;
    (v == initializer_store && matches!(f.inst(initializer), Inst::Block { .. }))
        .then_some(initializer)
}

/// `BlockFromInitializer`: the array initializer block from `values`, which holds for each
/// element its value and then its indices from the last dimension to the first.
fn block_from_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    v: VarId,
    element_type: TypeId,
    array_to_span: Option<crate::typesystem::resolve::ResolvedMethod>,
    array_length: &[i32],
    values: &[NodeId],
) -> NodeId {
    let lengths: Vec<NodeId> = array_length.iter().map(|&l| ldc_i4(f, l)).collect();
    let newarr = f.add(Inst::NewArr {
        ty: operand(element_type),
        indices: lengths,
    });
    let mut instructions = vec![stloc(f, v, newarr)];
    let step = array_length.len() + 1;
    for i in 0..values.len() / step {
        let value = values[step * i];
        let indices: Vec<NodeId> = (1..step).rev().map(|j| values[step * i + j]).collect();
        let array = ldloc(f, v);
        instructions.push(st_elem(f, ctx, array, indices, value, element_type));
    }
    let mut final_inst = ldloc(f, v);
    if let Some(method) = array_to_span {
        final_inst = f.add(Inst::Call(CallData {
            method: MethodOperand {
                range_accessor: None,
                specialized_by: None,
                token: 0,
                method,
                context: 0,
                lifted: false,
            },
            args: vec![final_inst],
            is_tail: false,
            constrained_to: None,
            il_stack_was_empty: false,
        }));
    }
    new_block(
        f,
        BlockKind::ArrayInitializer,
        instructions,
        Some(final_inst),
    )
}

/// `BuildSimpleArrayInitializerBlock`.
fn build_simple_array_initializer_block(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    v: VarId,
    element_type: TypeId,
    array_length: &[i32],
    values: Vec<(Vec<NodeId>, Option<NodeId>)>,
) -> NodeId {
    let lengths: Vec<NodeId> = array_length.iter().map(|&l| ldc_i4(f, l)).collect();
    let newarr = f.add(Inst::NewArr {
        ty: operand(element_type),
        indices: lengths,
    });
    let mut instructions = vec![stloc(f, v, newarr)];
    for (indices, value) in values {
        let value = match value {
            Some(value) => value,
            None => get_null_expression(f, ctx, element_type),
        };
        let array = ldloc(f, v);
        instructions.push(st_elem(f, ctx, array, indices, value, element_type));
    }
    let final_inst = ldloc(f, v);
    new_block(
        f,
        BlockKind::ArrayInitializer,
        instructions,
        Some(final_inst),
    )
}

/// `MatchNewArr`: the element type and the (positive constant) lengths.
fn match_new_arr(f: &Function, inst: NodeId) -> Option<(TypeId, Vec<i32>)> {
    let Inst::NewArr { ty, indices } = f.inst(inst) else {
        return None;
    };
    let mut length = Vec::with_capacity(indices.len());
    for &arg in indices {
        let value = f.match_ldc_i4(arg)?;
        if value <= 0 {
            return None;
        }
        length.push(value);
    }
    Some((ty.ty, length))
}

/// `MatchInitializeArrayCall`: `call RuntimeHelpers.InitializeArray(array, ldmembertoken
/// field)`.
fn match_initialize_array_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(NodeId, ResolvedField)> {
    let Inst::Call(call) = f.inst(inst) else {
        return None;
    };
    let &[array, token] = &call.args[..] else {
        return None;
    };
    let method = &call.method.method;
    if !method.is_static || method.name != "InitializeArray" {
        return None;
    }
    let declaring = method.declaring?;
    ctx.ts.definition(declaring)?;
    if !is_runtime_helpers(ctx, declaring) {
        return None;
    }
    let field = match_ld_member_token_field(f, token)?;
    Some((array, field))
}

/// `HandleRuntimeHelpersInitializeArray`: the decoded values and the position of the call.
fn handle_runtime_helpers_initialize_array(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    body: NodeId,
    pos: usize,
    array: VarId,
    array_type: TypeId,
    array_length: &[i32],
) -> Option<(Vec<NodeId>, usize)> {
    let inst = f.list(body)[pos];
    let (array_inst, field) = match_initialize_array_call(f, ctx, inst)?;
    if f.match_ldloc(array_inst) != Some(array) || !has_field_rva(ctx, &field) {
        return None;
    }
    // shortcut: unreadable data is reported as a warning in the managed code; the port only
    // gives up on the transform.
    let initial_value = initial_value(ctx, &field)?;
    let values = decode_array_initializer(f, ctx, array_type, initial_value, array_length)?;
    Some((values, pos))
}

/// `DoTransformInlineRuntimeHelpersInitializeArray`: `call InitializeArray(newarr T(size),
/// ldmembertoken field)` becomes an array initializer block.
fn do_transform_inline_runtime_helpers_initialize_array(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    body: NodeId,
    pos: usize,
) -> bool {
    let inst = f.list(body)[pos];
    let Some((array_inst, field)) = match_initialize_array_call(f, ctx, inst) else {
        return false;
    };
    let Some((element_type, array_length)) = match_new_arr(f, array_inst) else {
        return false;
    };
    if !has_field_rva(ctx, &field) {
        return false;
    }
    let Some(initial_value) = initial_value(ctx, &field) else {
        return false;
    };
    let Some(values) = decode_array_initializer(f, ctx, element_type, initial_value, &array_length)
    else {
        return false;
    };
    let ty = array_type(ctx, element_type, array_length.len() as u32);
    let temp_store = f.register_variable(ctx.ts, VariableKind::InitializerTarget, ty);
    let block = block_from_initializer(
        f,
        ctx,
        temp_store,
        element_type,
        None,
        &array_length,
        &values,
    );
    f.set_child_at(body, pos, block);
    inline_one_if_possible(f, body, pos, options::AGGRESSIVE, ctx);
    true
}

/// `DecodeArrayInitializer`: for each element its constant, then its indices from the last
/// dimension to the first.
fn decode_array_initializer(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    ty: TypeId,
    initial_value: &[u8],
    array_length: &[i32],
) -> Option<Vec<NodeId>> {
    let code = known(ctx, ty);
    let code = match code {
        Some(
            KnownTypeCode::Boolean
            | KnownTypeCode::Byte
            | KnownTypeCode::SByte
            | KnownTypeCode::Int16
            | KnownTypeCode::Char
            | KnownTypeCode::UInt16
            | KnownTypeCode::Int32
            | KnownTypeCode::UInt32
            | KnownTypeCode::Int64
            | KnownTypeCode::UInt64
            | KnownTypeCode::Single
            | KnownTypeCode::Double,
        ) => code?,
        _ if is_kind(ctx, ty, TypeKind::Enum) => ctx.ts.enum_underlying_known(ty)?,
        _ => return None,
    };
    let element_size = match code {
        KnownTypeCode::Boolean | KnownTypeCode::Byte | KnownTypeCode::SByte => 1,
        KnownTypeCode::Char | KnownTypeCode::Int16 | KnownTypeCode::UInt16 => 2,
        KnownTypeCode::Int32 | KnownTypeCode::UInt32 | KnownTypeCode::Single => 4,
        KnownTypeCode::Int64 | KnownTypeCode::UInt64 | KnownTypeCode::Double => 8,
        // An enum with another underlying type.
        _ => return None,
    };
    // The managed product and size wrap on overflow.
    let total_length = array_length.iter().fold(1i32, |t, &l| t.wrapping_mul(l));
    if (initial_value.len() as i64) < i64::from(total_length.wrapping_mul(element_size)) {
        return None;
    }
    let mut output = Vec::new();
    let mut offset = 0usize;
    for i in 0..total_length.max(0) {
        let bytes = initial_value.get(offset..offset + element_size as usize)?;
        offset += element_size as usize;
        let value = match code {
            KnownTypeCode::Boolean | KnownTypeCode::Byte => ldc_i4(f, i32::from(bytes[0])),
            KnownTypeCode::SByte => ldc_i4(f, i32::from(bytes[0] as i8)),
            KnownTypeCode::Int16 => ldc_i4(f, i32::from(i16::from_le_bytes([bytes[0], bytes[1]]))),
            KnownTypeCode::Char | KnownTypeCode::UInt16 => {
                ldc_i4(f, i32::from(u16::from_le_bytes([bytes[0], bytes[1]])))
            }
            KnownTypeCode::Int32 | KnownTypeCode::UInt32 => {
                ldc_i4(f, i32::from_le_bytes(bytes.try_into().ok()?))
            }
            KnownTypeCode::Int64 | KnownTypeCode::UInt64 => f.add(Inst::LdcI8 {
                value: i64::from_le_bytes(bytes.try_into().ok()?),
            }),
            KnownTypeCode::Single => f.add(Inst::LdcF4 {
                bits: u32::from_le_bytes(bytes.try_into().ok()?),
            }),
            _ => f.add(Inst::LdcF8 {
                bits: u64::from_le_bytes(bytes.try_into().ok()?),
            }),
        };
        output.push(value);
        let mut next = i;
        for &length in array_length.iter().rev() {
            output.push(ldc_i4(f, next % length));
            next /= length;
        }
    }
    Some(output)
}

/// `StElem`: `stobj T(ldelema T(array, indices), value)`, converting the value to the
/// element's stack type.
fn st_elem(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    array: NodeId,
    indices: Vec<NodeId>,
    mut value: NodeId,
    ty: TypeId,
) -> NodeId {
    if ctx.ts.stack_type(ty) != f.result_type(ctx.ts, value) {
        let input = f.result_type(ctx.ts, value);
        let target = ctx.ts.primitive_type(ty);
        value = f.add(Inst::Conv {
            arg: value,
            input,
            input_sign: Sign::None,
            target,
            check_overflow: false,
            is_lifted: false,
        });
    }
    let address = f.add(Inst::LdElema {
        ty: operand(ty),
        array,
        indices,
        delay_exceptions: true,
        is_read_only: false,
        with_system_index: false,
    });
    f.add(Inst::StObj {
        target: address,
        value,
        ty: operand(ty),
        is_volatile: false,
        unaligned: 0,
    })
}

/// `GetNullExpression`: the default value of an element type as a constant where there is
/// one.
pub(crate) fn get_null_expression(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    element_type: TypeId,
) -> NodeId {
    match ctx.ts.enum_underlying_known(element_type) {
        Some(
            KnownTypeCode::Boolean
            | KnownTypeCode::Char
            | KnownTypeCode::SByte
            | KnownTypeCode::Byte
            | KnownTypeCode::Int16
            | KnownTypeCode::UInt16
            | KnownTypeCode::Int32
            | KnownTypeCode::UInt32,
        ) => ldc_i4(f, 0),
        Some(KnownTypeCode::Int64 | KnownTypeCode::UInt64) => f.add(Inst::LdcI8 { value: 0 }),
        Some(KnownTypeCode::Single) => f.add(Inst::LdcF4 { bits: 0 }),
        Some(KnownTypeCode::Double) => f.add(Inst::LdcF8 { bits: 0 }),
        Some(KnownTypeCode::Decimal) => f.add(Inst::LdcDecimal { bits: [0; 4] }),
        Some(KnownTypeCode::Void) => panic!("void is not a valid element type!"),
        _ => f.add(Inst::DefaultValue {
            ty: operand(element_type),
            il_stack_was_empty: false,
        }),
    }
}
