//! `IL/Transforms/TransformExpressionTrees.cs`: the `System.Linq.Expressions.Expression`
//! factory calls that build an expression tree become an `ILFunction` lambda with the
//! equivalent ILAst body.
//!
//! As in the managed transform, the conversion runs in two phases: matching builds a tree of
//! builders without changing the function, and only when the whole tree matched do the
//! builders create the new instructions.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, CallData, ComparisonKind, ComparisonLiftingKind,
    ContainerKind, FieldOperand, Function, FunctionKind, Inst, MemberOperand, MethodOperand,
    NodeId, NopKind, NullCoalescingKind, TypeOperand, VarId, Variable, VariableKind, nullable_type,
};
use crate::il::reader::ReadFunction;
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::{Nullability, TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::known;
use super::statement_transform::StatementTransformContext;
use super::transform_assignment::nullable_underlying;

/// A builder of the second phase: creates the converted instruction, or None when a nested
/// conversion declines.
type Build = Box<dyn FnOnce(&mut Env<'_, '_, '_, '_>) -> Option<NodeId>>;
/// A builder that takes the variable of the object being initialized.
type BuildOn = Box<dyn FnOnce(&mut Env<'_, '_, '_, '_>, VarId) -> Option<NodeId>>;

/// The function, the context and the transform's state.
struct Env<'e, 't, 'c, 'a> {
    f: &'e mut Function,
    ctx: &'e mut Context<'t, 'c, 'a>,
    /// The type of the decompiled method (`IsPotentialClosure`).
    decompiled_type: Option<TypeDefId>,
    /// `parameters`: the ParameterExpression variables and their type and name.
    parameters: Vec<(VarId, TypeId, String)>,
    /// `parameterMapping`: ParameterExpression variable to lambda parameter.
    parameter_mapping: Vec<(VarId, VarId)>,
    instructions_to_remove: Vec<NodeId>,
    lambda_stack: Vec<NodeId>,
}

impl Env<'_, '_, '_, '_> {
    fn add(&mut self, inst: Inst) -> NodeId {
        self.f.add(inst)
    }

    fn full_name(&self, t: TypeId) -> String {
        self.ctx.ts.full_name(t)
    }

    /// `IMethod.FullName`.
    fn method_full_name(&self, m: &ResolvedMethod) -> String {
        match m.declaring {
            Some(d) => format!("{}.{}", self.full_name(d), m.name),
            None => m.name.clone(),
        }
    }

    fn infer(&mut self, inst: NodeId) -> TypeId {
        self.f.infer_type(self.ctx.ts, inst)
    }

    fn result_type(&mut self, inst: NodeId) -> StackType {
        self.f.result_type(self.ctx.ts, inst)
    }

    fn is_nullable(&mut self, t: TypeId) -> bool {
        known(self.ctx, t) == Some(KnownTypeCode::NullableOfT)
    }

    /// `NullableType.GetUnderlyingType`.
    fn underlying(&mut self, t: TypeId) -> TypeId {
        if self.is_nullable(t) {
            nullable_underlying(self.ctx, t)
        } else {
            t
        }
    }

    fn is_known(&mut self, t: TypeId, code: KnownTypeCode) -> bool {
        known(self.ctx, t) == Some(code)
    }

    fn is_unknown(&self, t: TypeId) -> bool {
        self.ctx.ts.is_unknown_kind(t)
    }

    fn unknown_type(&mut self) -> TypeId {
        self.ctx.ts.intern(TypeData::UnknownSpecial)
    }

    fn operand(ty: TypeId) -> TypeOperand {
        TypeOperand { token: 0, ty }
    }

    fn method_operand(method: ResolvedMethod) -> MethodOperand {
        MethodOperand {
            range_accessor: None,
            specialized_by: None,
            local_function: None,
            token: 0,
            method,
            context: 0,
            lifted: false,
        }
    }

    fn call(&mut self, virt: bool, method: MethodOperand, args: Vec<NodeId>) -> NodeId {
        let data = CallData {
            method,
            args,
            is_tail: false,
            constrained_to: None,
            il_stack_was_empty: false,
        };
        self.add(if virt {
            Inst::CallVirt(data)
        } else {
            Inst::Call(data)
        })
    }

    fn new_obj(&mut self, method: MethodOperand, args: Vec<NodeId>) -> NodeId {
        self.add(Inst::NewObj(CallData {
            method,
            args,
            is_tail: false,
            constrained_to: None,
            il_stack_was_empty: false,
        }))
    }

    fn ldloc(&mut self, var: VarId) -> NodeId {
        self.add(Inst::LdLoc { var })
    }

    fn ldc_i4(&mut self, value: i32) -> NodeId {
        self.add(Inst::LdcI4 { value })
    }

    fn conv(&mut self, arg: NodeId, target: PrimitiveType, sign: Sign) -> NodeId {
        let input = self.result_type(arg);
        self.add(Inst::Conv {
            arg,
            input,
            input_sign: sign,
            target,
            check_overflow: false,
            is_lifted: false,
        })
    }

    /// `ILFunction.RegisterVariable` on the lambda being built.
    fn register_lambda_variable(&mut self, kind: VariableKind, ty: TypeId) -> VarId {
        let lambda = *self.lambda_stack.last().expect("a lambda");
        let Inst::Function {
            helper_variable_count,
            ..
        } = self.f.inst(lambda)
        else {
            unreachable!("a lambda");
        };
        let name = format!("I_{helper_variable_count}");
        let stack_type = self.ctx.ts.stack_type(ty);
        let v = self
            .f
            .add_var(new_variable(kind, ty, stack_type, None, name, true));
        if let Inst::Function {
            helper_variable_count,
            variables,
            ..
        } = self.f.inst_mut(lambda)
        {
            *helper_variable_count += 1;
            variables.push(v);
        }
        v
    }
}

fn new_variable(
    kind: VariableKind,
    ty: TypeId,
    stack_type: StackType,
    index: Option<i32>,
    name: String,
    has_generated_name: bool,
) -> Variable {
    Variable {
        kind,
        ty,
        stack_type,
        index,
        name,
        has_generated_name,
        initial_value_is_initialized: false,
        uses_initial_value: false,
        is_ref_read_only: false,
        remove_if_redundant: false,
        state_machine_field: None,
        load_instructions: Vec::new(),
        store_instructions: Vec::new(),
        address_instructions: Vec::new(),
        capture_scope: None,
        is_scoped: false,
        is_scoped_without_initializer: false,
    }
}

/// The method and arguments of a `CallInstruction` (call, callvirt or newobj).
fn call_parts(f: &Function, inst: NodeId) -> Option<(MethodOperand, Vec<NodeId>)> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
            Some((c.method.clone(), c.args.clone()))
        }
        _ => None,
    }
}

/// `IMethod.FullNameIs(type, name)` of the method of a call instruction.
fn is_call_to(env: &Env<'_, '_, '_, '_>, inst: NodeId, type_name: &str, name: &str) -> bool {
    call_parts(env.f, inst).is_some_and(|(m, _)| {
        m.method.name == name
            && m.method
                .declaring
                .is_some_and(|d| env.full_name(d) == type_name)
    })
}

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.expression_trees {
        return;
    }
    let decompiled_type = ctx.ts.method_declaring(r.method).ok().flatten();
    let mut env = Env {
        f: &mut r.function,
        ctx,
        decompiled_type,
        parameters: Vec::new(),
        parameter_mapping: Vec::new(),
        instructions_to_remove: Vec::new(),
        lambda_stack: Vec::new(),
    };
    let mut i = pos;
    while i < env.f.list(block).len() {
        let inst = env.f.list(block)[i];
        if let Some((v, ty, name)) = match_parameter_variable_assignment(&mut env, inst) {
            env.parameters.push((v, ty, name));
            i += 1;
            continue;
        }
        if try_convert_expression_tree(&mut env, inst) {
            for remove in std::mem::take(&mut env.instructions_to_remove) {
                if let Some(index) = env.f.list(block).iter().position(|&x| x == remove) {
                    env.f.remove_at(block, index);
                }
            }
        }
        break;
    }
}

/// `MightBeExpressionTree`: `call Lambda(body, parameters)` with an empty parameter list or
/// an array initializer.
fn might_be_expression_tree(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> bool {
    if !is_call_to(env, inst, "System.Linq.Expressions.Expression", "Lambda") {
        return false;
    }
    let (_, args) = call_parts(env.f, inst).expect("a call");
    if args.len() != 2 {
        return false;
    }
    is_empty_parameter_list(env, args[1])
        || matches!(
            env.f.inst(args[1]),
            Inst::Block {
                kind: BlockKind::ArrayInitializer,
                ..
            }
        )
}

/// `IsEmptyParameterList`: `call Array.Empty()`, or `newarr ParameterExpression` or
/// `newarr Expression` (of any length).
fn is_empty_parameter_list(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> bool {
    if is_call_to(env, inst, "System.Array", "Empty")
        && call_parts(env.f, inst).is_some_and(|(_, a)| a.is_empty())
    {
        return true;
    }
    if let Inst::NewArr { ty, .. } = env.f.inst(inst) {
        let name = env.full_name(ty.ty);
        return name == "System.Linq.Expressions.ParameterExpression"
            || name == "System.Linq.Expressions.Expression";
    }
    false
}

/// `MatchParameterVariableAssignment`:
/// `stloc v(call Parameter(call GetTypeFromHandle(ldtypetoken T), ldstr "name"))` with a
/// single-definition local or stack slot `v` of type ParameterExpression.
fn match_parameter_variable_assignment(
    env: &mut Env<'_, '_, '_, '_>,
    inst: NodeId,
) -> Option<(VarId, TypeId, String)> {
    let (v, init) = env.f.match_stloc(inst)?;
    let var = env.f.var(v);
    if !var.is_single_definition()
        || !matches!(var.kind, VariableKind::Local | VariableKind::StackSlot)
    {
        return None;
    }
    if env.full_name(var.ty) != "System.Linq.Expressions.ParameterExpression" {
        return None;
    }
    let (_, args) = call_parts(env.f, init)?;
    if args.len() != 2 || !is_call_to(env, init, "System.Linq.Expressions.Expression", "Parameter")
    {
        return None;
    }
    let (_, type_args) = call_parts(env.f, args[0])?;
    if type_args.len() != 1 || !is_call_to(env, args[0], "System.Type", "GetTypeFromHandle") {
        return None;
    }
    let Inst::LdTypeToken { ty } = env.f.inst(type_args[0]) else {
        return None;
    };
    let Inst::LdStr { value } = env.f.inst(args[1]) else {
        return None;
    };
    Some((v, ty.ty, String::from_utf16_lossy(value)))
}

/// `TryConvertExpressionTree`: converts the first `call Lambda(...)` in `instruction`, not
/// looking into nested control-flow blocks.
fn try_convert_expression_tree(env: &mut Env<'_, '_, '_, '_>, instruction: NodeId) -> bool {
    if might_be_expression_tree(env, instruction) {
        let Some(lambda) = convert_lambda(env, instruction) else {
            return false;
        };
        // A builder declines with None, and only a function can take the call's place; the
        // tree then stays as the Expression calls that built it.
        let Some(new_lambda) = lambda(env) else {
            return false;
        };
        if !matches!(env.f.inst(new_lambda), Inst::Function { .. }) {
            return false;
        }
        set_expression_tree_flag(env, new_lambda, instruction);
        env.f.replace(instruction, new_lambda);
        return true;
    }
    if matches!(
        env.f.inst(instruction),
        Inst::Block {
            kind: BlockKind::ControlFlow,
            ..
        }
    ) {
        return false;
    }
    for child in env.f.inst(instruction).children() {
        if try_convert_expression_tree(env, child) {
            return true;
        }
    }
    false
}

/// `IsExpressionTree`: `System.Linq.Expressions.Expression<T>`.
fn is_expression_tree(env: &Env<'_, '_, '_, '_>, t: TypeId) -> bool {
    matches!(env.ctx.ts.data(t), TypeData::Inst { args, .. } if args.len() == 1)
        && env.full_name(t) == "System.Linq.Expressions.Expression"
}

/// `UnwrapExpressionTree`: `T` of `Expression<T>`.
fn unwrap_expression_tree(env: &Env<'_, '_, '_, '_>, t: TypeId) -> TypeId {
    if is_expression_tree(env, t)
        && let TypeData::Inst { args, .. } = env.ctx.ts.data(t)
    {
        return args[0];
    }
    t
}

/// `SetExpressionTreeFlag`: delegate type and kind from the return type of `call`.
fn set_expression_tree_flag(env: &mut Env<'_, '_, '_, '_>, lambda: NodeId, call: NodeId) {
    let ret = call_parts(env.f, call).expect("a call").0.method.ret;
    let is_tree = is_expression_tree(env, ret);
    if let Inst::Function {
        kind,
        delegate_type,
        ..
    } = env.f.inst_mut(lambda)
    {
        *kind = if is_tree {
            FunctionKind::ExpressionTree
        } else {
            FunctionKind::Delegate
        };
        *delegate_type = Some(ret);
    }
}

/// `ConvertLambda`: `call Lambda(body, parameters)` becomes an `ILFunction` with
/// `BlockContainer { Block { leave(converted body) } }`.
fn convert_lambda(env: &mut Env<'_, '_, '_, '_>, instruction: NodeId) -> Option<Build> {
    let (m, args) = call_parts(env.f, instruction)?;
    let ret = m.method.ret;
    if m.method.name != "Lambda"
        || args.len() != 2
        || env.full_name(ret) != "System.Linq.Expressions.Expression"
    {
        return None;
    }
    let TypeData::Inst {
        args: type_args, ..
    } = env.ctx.ts.data(ret).clone()
    else {
        return None;
    };
    let &[function_type] = &type_args[..] else {
        return None;
    };
    let (parameter_list, parameter_variables) = read_parameters(env, args[1])?;
    let range = env.f.il_range(instruction);
    let container = env.add(Inst::BlockContainer {
        kind: ContainerKind::Normal,
        expected: None,
        blocks: Vec::new(),
    });
    env.f.add_il_range(container, range);
    let return_type = match env
        .ctx
        .ts
        .get_delegate_invoke_method(function_type)
        .ok()
        .flatten()
    {
        Some(invoke) => invoke.ret,
        None => env.unknown_type(),
    };
    let kind = if is_expression_tree(env, function_type) {
        FunctionKind::ExpressionTree
    } else {
        FunctionKind::Delegate
    };
    let function = env.add(Inst::Function {
        kind,
        delegate_type: Some(function_type),
        return_type,
        parameters: parameter_list,
        variables: parameter_variables,
        helper_variable_count: 0,
        body: container,
        read: None,
        local_functions: Vec::new(),
        declaration_scope: None,
        reduced: None,
        name: None,
    });
    env.f.add_il_range(function, range);
    env.lambda_stack.push(function);
    let body_instruction = convert_instruction(env, args[0], None);
    env.lambda_stack.pop();
    let body_instruction = body_instruction?;
    Some(Box::new(move |env| {
        env.lambda_stack.push(function);
        let converted_body = body_instruction(env);
        env.lambda_stack.pop();
        let converted_body = converted_body?;
        let expected = env.infer(converted_body);
        if let Inst::BlockContainer { expected: e, .. } = env.f.inst_mut(container) {
            *e = Some(expected);
        }
        let leave = env.add(Inst::Leave {
            target: Some(container),
            value: converted_body,
        });
        let nop = env.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let block = env.add(Inst::Block {
            kind: BlockKind::ControlFlow,
            instructions: vec![leave],
            final_inst: nop,
        });
        env.f.push(container, block);
        // Other uses of the ParameterExpression variables load the lambda parameters.
        for (key, value) in env.parameter_mapping.clone() {
            for load in env.f.var(key).load_instructions.clone() {
                if is_descendant_of(env.f, load, instruction) {
                    continue;
                }
                let new_load = env.ldloc(value);
                env.f.replace(load, new_load);
            }
        }
        Some(function)
    }))
}

/// `ILInstruction.IsDescendantOf` (including the node itself).
fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    let mut current = Some(inst);
    while let Some(n) = current {
        if n == ancestor {
            return true;
        }
        current = f.node(n).parent;
    }
    false
}

/// `ConvertQuote`: `call Quote(lambda)` becomes the converted lambda.
fn convert_quote(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    let &[argument] = &args[..] else {
        return None;
    };
    if matches!(env.f.inst(argument), Inst::Function { .. }) {
        return Some(Box::new(move |_| Some(argument)));
    }
    let converted = convert_instruction(env, argument, None)?;
    Some(Box::new(move |env| {
        let f = converted(env)?;
        if matches!(env.f.inst(f), Inst::Function { .. }) && call_parts(env.f, argument).is_some() {
            set_expression_tree_flag(env, f, argument);
        }
        Some(f)
    }))
}

/// The parameters of a lambda (type and name) and their variables.
type LambdaParameters = (Vec<(TypeId, String)>, Vec<VarId>);

/// `ReadParameters`: the lambda parameters (type and name) and their new parameter
/// variables, from the ParameterExpression array initializer.
fn read_parameters(env: &mut Env<'_, '_, '_, '_>, initializer: NodeId) -> Option<LambdaParameters> {
    let mut parameters = Vec::new();
    let mut variables = Vec::new();
    match env.f.inst(initializer) {
        Inst::Block {
            kind, instructions, ..
        } => {
            if *kind != BlockKind::ArrayInitializer {
                return None;
            }
            let instructions = instructions.clone();
            let mut i = 0;
            for inst in instructions {
                let Inst::StObj { value, .. } = *env.f.inst(inst) else {
                    continue;
                };
                if i >= env.parameters.len() {
                    return None;
                }
                let v = env.f.match_ldloc(value)?;
                let (_, ty, name) = env.parameters.iter().find(|p| p.0 == v)?.clone();
                // Each ParameterExpression variable enters the mapping once.
                if !env.parameter_mapping.iter().any(|p| p.0 == v) {
                    let stack_type = env.ctx.ts.stack_type(ty);
                    let param = env.f.add_var(new_variable(
                        VariableKind::Parameter,
                        ty,
                        stack_type,
                        Some(i as i32),
                        name.clone(),
                        false,
                    ));
                    env.parameter_mapping.push((v, param));
                    variables.push(param);
                    parameters.push((ty, name));
                    let store = env.f.var(v).store_instructions[0];
                    env.instructions_to_remove.push(store);
                }
                i += 1;
            }
            Some((parameters, variables))
        }
        _ => is_empty_parameter_list(env, initializer).then_some((parameters, variables)),
    }
}

/// `ConvertInstruction`: the builder for one node of the expression tree; with a type hint,
/// the result converts to its stack type.
fn convert_instruction(
    env: &mut Env<'_, '_, '_, '_>,
    instruction: NodeId,
    type_hint: Option<TypeId>,
) -> Option<Build> {
    let inst = convert(env, instruction, type_hint)?;
    Some(Box::new(move |env| {
        let result = inst(env)?;
        if let Some(hint) = type_hint {
            let hint_stack = env.ctx.ts.stack_type(hint);
            if env.result_type(result) != hint_stack {
                let sign = env.ctx.ts.type_sign(hint);
                return Some(env.conv(result, hint_stack.to_primitive(Sign::None), sign));
            }
        }
        Some(result)
    }))
}

/// The dispatch of `ConvertInstruction`.
fn convert(
    env: &mut Env<'_, '_, '_, '_>,
    instruction: NodeId,
    type_hint: Option<TypeId>,
) -> Option<Build> {
    use BinaryNumericOperator as Op;
    match env.f.inst(instruction) {
        Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_) => {
            let (m, _) = call_parts(env.f, instruction).expect("a call");
            if m.method.declaring.map(|d| env.full_name(d)).as_deref()
                != Some("System.Linq.Expressions.Expression")
            {
                return None;
            }
            let i = instruction;
            match m.method.name.as_str() {
                "Add" => {
                    convert_binary_numeric_operator(env, i, Op::Add, "op_Addition", Some(false))
                }
                "AddChecked" => {
                    convert_binary_numeric_operator(env, i, Op::Add, "op_Addition", Some(true))
                }
                "And" => convert_binary_numeric_operator(env, i, Op::BitAnd, "op_BitwiseAnd", None),
                "AndAlso" => convert_logic_operator(env, i, true),
                "ArrayAccess" | "ArrayIndex" => convert_array_index(env, i),
                "ArrayLength" => convert_array_length(env, i),
                "Call" => convert_call(env, i),
                "Coalesce" => convert_coalesce(env, i),
                "Condition" => convert_condition(env, i),
                "Constant" => convert_constant(env, i),
                "Convert" => convert_cast(env, i, false),
                "ConvertChecked" => convert_cast(env, i, true),
                "Divide" => convert_binary_numeric_operator(env, i, Op::Div, "op_Division", None),
                "Equal" => convert_comparison(env, i, ComparisonKind::Equality),
                "ExclusiveOr" => {
                    convert_binary_numeric_operator(env, i, Op::BitXor, "op_ExclusiveOr", None)
                }
                "Field" => convert_field(env, i, type_hint),
                "GreaterThan" => convert_comparison(env, i, ComparisonKind::GreaterThan),
                "GreaterThanOrEqual" => {
                    convert_comparison(env, i, ComparisonKind::GreaterThanOrEqual)
                }
                "Invoke" => convert_invoke(env, i),
                "Lambda" => convert_lambda(env, i),
                "LeftShift" => {
                    convert_binary_numeric_operator(env, i, Op::ShiftLeft, "op_LeftShift", None)
                }
                "LessThan" => convert_comparison(env, i, ComparisonKind::LessThan),
                "LessThanOrEqual" => convert_comparison(env, i, ComparisonKind::LessThanOrEqual),
                "ListInit" => convert_list_init(env, i),
                "MemberInit" => convert_member_init(env, i),
                "Modulo" => convert_binary_numeric_operator(env, i, Op::Rem, "op_Modulus", None),
                "Multiply" => {
                    convert_binary_numeric_operator(env, i, Op::Mul, "op_Multiply", Some(false))
                }
                "MultiplyChecked" => {
                    convert_binary_numeric_operator(env, i, Op::Mul, "op_Multiply", Some(true))
                }
                "Negate" => convert_unary_numeric_operator(env, i, Op::Sub, Some(false)),
                "NegateChecked" => convert_unary_numeric_operator(env, i, Op::Sub, Some(true)),
                "New" => convert_new_object(env, i),
                "NewArrayBounds" => convert_new_array_bounds(env, i),
                "NewArrayInit" => convert_new_array_init(env, i),
                "Not" | "OnesComplement" => convert_not_operator(env, i),
                "NotEqual" => convert_comparison(env, i, ComparisonKind::Inequality),
                "Or" => convert_binary_numeric_operator(env, i, Op::BitOr, "op_BitwiseOr", None),
                "OrElse" => convert_logic_operator(env, i, false),
                "Property" => convert_property(env, i),
                "Quote" => convert_quote(env, i),
                "RightShift" => {
                    convert_binary_numeric_operator(env, i, Op::ShiftRight, "op_RightShift", None)
                }
                "Subtract" => {
                    convert_binary_numeric_operator(env, i, Op::Sub, "op_Subtraction", Some(false))
                }
                "SubtractChecked" => {
                    convert_binary_numeric_operator(env, i, Op::Sub, "op_Subtraction", Some(true))
                }
                "TypeAs" => convert_type_as(env, i),
                "TypeIs" => convert_type_is(env, i),
                _ => None,
            }
        }
        Inst::Function { .. } => {
            let function = instruction;
            Some(Box::new(move |env| {
                if let Inst::Function {
                    kind: FunctionKind::ExpressionTree,
                    delegate_type,
                    ..
                } = env.f.inst(function).clone()
                {
                    let unwrapped =
                        unwrap_expression_tree(env, delegate_type.expect("a delegate type"));
                    if let Inst::Function {
                        kind,
                        delegate_type,
                        ..
                    } = env.f.inst_mut(function)
                    {
                        *delegate_type = Some(unwrapped);
                        *kind = FunctionKind::Delegate;
                    }
                }
                Some(function)
            }))
        }
        &Inst::LdLoc { var } => {
            if !is_expression_tree_parameter(env, var) {
                return None;
            }
            // An already mapped parameter becomes the lambda parameter variable.
            if let Some(&(_, v)) = env.parameter_mapping.iter().find(|p| p.0 == var) {
                let by_ref_hint = type_hint.is_some_and(|h| is_by_ref(env, h));
                let v_type = env.f.var(v).ty;
                if by_ref_hint && !env.ctx.ts.is_by_ref_like(v_type) {
                    return Some(Box::new(move |env| Some(env.add(Inst::LdLoca { var: v }))));
                }
                return Some(Box::new(move |env| Some(env.ldloc(v))));
            }
            // A parameter of an enclosing lambda, mapped only later (the transform works
            // backwards): the load stays, cast to the parameter's type so that the conversion
            // can go on.
            let ldloc = instruction;
            if env.f.var(var).is_single_definition()
                && let Some(&store) = env.f.var(var).store_instructions.first()
                && let Some((_, t, _)) = match_parameter_variable_assignment(env, store)
            {
                return Some(Box::new(move |env| {
                    Some(env.add(Inst::ExpressionTreeCast {
                        ty: t,
                        arg: ldloc,
                        is_checked: false,
                    }))
                }));
            }
            None
        }
        _ => None,
    }
}

/// `typeHint.SkipModifiers() is ByReferenceType`.
fn is_by_ref(env: &Env<'_, '_, '_, '_>, t: TypeId) -> bool {
    match env.ctx.ts.data(t) {
        TypeData::Modified { inner, .. } => is_by_ref(env, *inner),
        TypeData::ByRef(_) => true,
        _ => false,
    }
}

/// `IsExpressionTreeParameter`.
fn is_expression_tree_parameter(env: &Env<'_, '_, '_, '_>, v: VarId) -> bool {
    env.full_name(env.f.var(v).ty) == "System.Linq.Expressions.ParameterExpression"
}

/// Converts each of `instructions`; None when one does not convert.
fn convert_all(env: &mut Env<'_, '_, '_, '_>, instructions: &[NodeId]) -> Option<Vec<Build>> {
    let mut out = Vec::with_capacity(instructions.len());
    for &i in instructions {
        out.push(convert_instruction(env, i, None)?);
    }
    Some(out)
}

/// Runs the builders in order; None when one declines.
fn build_all(env: &mut Env<'_, '_, '_, '_>, builders: Vec<Build>) -> Option<Vec<NodeId>> {
    let mut out = Vec::with_capacity(builders.len());
    for b in builders {
        out.push(b(env)?);
    }
    Some(out)
}

/// `ConvertArrayIndex`: `call ArrayIndex(array, index)` (or an argument list for
/// multi-dimensional arrays) becomes `ldobj T(delayex.ldelema T(array, indices))`.
fn convert_array_index(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let array = convert_instruction(env, args[0], None)?;
    let arguments = match_argument_list(env, args[1]).unwrap_or_else(|| vec![args[1]]);
    Some(Box::new(move |env| {
        let array_inst = array(env)?;
        let array_type = env.infer(array_inst);
        let TypeData::Array { elem, .. } = *env.ctx.ts.data(array_type) else {
            return None;
        };
        let to_be_converted = convert_all(env, &arguments)?;
        let indices = build_all(env, to_be_converted)?;
        let address = env.add(Inst::LdElema {
            ty: Env::operand(elem),
            array: array_inst,
            indices,
            delay_exceptions: true,
            is_read_only: false,
            with_system_index: false,
        });
        Some(env.add(Inst::LdObj {
            target: address,
            ty: Env::operand(elem),
            is_volatile: false,
            unaligned: 0,
        }))
    }))
}

/// `ConvertArrayLength`: `call ArrayLength(array)` becomes `ldlen.i4(array)`.
fn convert_array_length(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    let &[arg] = &args[..] else {
        return None;
    };
    let converted = convert_instruction(env, arg, None)?;
    Some(Box::new(move |env| {
        let array = converted(env)?;
        Some(env.add(Inst::LdLen {
            array,
            result: StackType::I4,
        }))
    }))
}

/// `GetSignForOperator`: the sign is part of the IL opcode only for checked add, sub and mul
/// and for div, rem and shr; elsewhere the reader leaves it at None.
fn sign_for_operator(
    env: &mut Env<'_, '_, '_, '_>,
    op: BinaryNumericOperator,
    is_checked: bool,
    t: TypeId,
) -> Sign {
    match op {
        BinaryNumericOperator::Div
        | BinaryNumericOperator::Rem
        | BinaryNumericOperator::ShiftRight => env.ctx.ts.type_sign(t),
        BinaryNumericOperator::Add | BinaryNumericOperator::Sub | BinaryNumericOperator::Mul => {
            if is_checked {
                env.ctx.ts.type_sign(t)
            } else {
                Sign::None
            }
        }
        _ => Sign::None,
    }
}

/// `CSharpOperators.LiftUserDefinedOperator`.
fn lift(env: &mut Env<'_, '_, '_, '_>, method: &MethodOperand) -> Option<MethodOperand> {
    super::nullable_lifting_transform::lift_user_defined_operator(env.ctx, method)
}

/// A two-operand call of `method` built from `left` and `right`, lifted when the left operand
/// is nullable and `lift_when_nullable` is set.
fn binary_call(
    left: Build,
    right: Build,
    method: MethodOperand,
    lift_when_nullable: bool,
) -> Build {
    Box::new(move |env| {
        let left_inst = left(env)?;
        let right_inst = right(env)?;
        let mut op_method = method;
        if lift_when_nullable {
            let left_type = env.infer(left_inst);
            if env.is_nullable(left_type) {
                op_method = lift(env, &op_method)?;
            }
        }
        Some(env.call(false, op_method, vec![left_inst, right_inst]))
    })
}

/// `ConvertBinaryNumericOperator`.
fn convert_binary_numeric_operator(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    op: BinaryNumericOperator,
    operator_name: &'static str,
    is_checked: Option<bool>,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let left = convert_instruction(env, args[0], None)?;
    let right = convert_instruction(env, args[1], None)?;
    let is_checked = is_checked == Some(true);
    match args.len() {
        // A built-in operator, or the operator method of decimal.
        2 => Some(Box::new(move |env| {
            let left_inst = left(env)?;
            let right_inst = right(env)?;
            let left_type = env.infer(left_inst);
            let right_type = env.infer(right_inst);
            let left_underlying = env.underlying(left_type);
            let right_underlying = env.underlying(right_type);
            let (ls, rs) = (
                env.ctx.ts.stack_type(left_underlying),
                env.ctx.ts.stack_type(right_underlying),
            );
            if matches!(
                op,
                BinaryNumericOperator::ShiftLeft | BinaryNumericOperator::ShiftRight
            ) {
                if !env.is_known(right_underlying, KnownTypeCode::Int32) {
                    return None;
                }
            } else if ls != rs
                && !env.is_unknown(left_underlying)
                && !env.is_unknown(right_underlying)
            {
                // The stack types compare: a small integer converted to Int32 stays as it is.
                return None;
            }
            let ty = if env.is_unknown(left_type) {
                right_type
            } else {
                left_type
            };
            if env.is_known(ty, KnownTypeCode::Decimal) {
                let methods = env
                    .ctx
                    .ts
                    .get_methods(ty, None, MemberOptions::default())
                    .ok()?;
                let mut op_method = None;
                for m in methods {
                    if m.name == operator_name && env.ctx.ts.method_is_operator(&m) {
                        op_method = Some(m);
                        break;
                    }
                }
                let op_method = op_method?;
                return Some(env.call(
                    false,
                    Env::method_operand(op_method),
                    vec![left_inst, right_inst],
                ));
            }
            let is_lifted = env.is_nullable(left_type) || env.is_nullable(right_type);
            let sign = sign_for_operator(env, op, is_checked, left_type);
            let csharp_result_type = if is_lifted {
                let u = if env.is_unknown(left_underlying) {
                    right_underlying
                } else {
                    left_underlying
                };
                Some(nullable_type(env.ctx.ts, u))
            } else {
                None
            };
            Some(env.add(Inst::BinaryNumeric {
                op,
                check_overflow: is_checked,
                sign,
                left_input: ls,
                right_input: rs,
                left: left_inst,
                right: right_inst,
                csharp_result_type,
                is_lifted,
            }))
        })),
        // A user-defined operator.
        3 => {
            let method = match_get_method_from_handle(env, args[2])?;
            Some(binary_call(left, right, method, false))
        }
        // The shape of the comparison factories (liftToNull, method).
        4 => {
            env.f.match_ldc_i4(args[2])?;
            let method = match_get_method_from_handle(env, args[3])?;
            Some(binary_call(left, right, method, true))
        }
        _ => None,
    }
}

/// `ConvertBind`: `call Bind(member, value)` becomes a setter call or a field store on the
/// object being initialized.
fn convert_bind(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<BuildOn> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let value = convert_instruction(env, args[1], None)?;
    if let Some(mut method) = match_get_method_from_handle(env, args[0]) {
        // A getter is valid in Expression.Bind; the assignment uses the setter.
        if let Some(setter) = property_setter_of(env, &method.method)
            && !same_method(&setter, &method.method)
        {
            method = Env::method_operand(setter);
        }
        let is_static = method.method.is_static;
        return Some(Box::new(move |env, target| {
            let value_inst = value(env)?;
            let load = env.ldloc(target);
            Some(env.call(!is_static, method, vec![load, value_inst]))
        }));
    }
    let field = match_get_field_from_handle(env, args[0])?;
    Some(Box::new(move |env, target| {
        let value_inst = value(env)?;
        let load = env.ldloc(target);
        let address = env.add(Inst::LdFlda {
            target: load,
            field: field.clone(),
            delay_exceptions: true,
        });
        Some(env.add(Inst::StObj {
            target: address,
            value: value_inst,
            ty: Env::operand(field.field.ty),
            is_volatile: false,
            unaligned: 0,
        }))
    }))
}

fn same_method(a: &ResolvedMethod, b: &ResolvedMethod) -> bool {
    a.origin == b.origin && a.declaring == b.declaring && a.name == b.name
}

/// The setter of the property `method` is an accessor of (`AccessorOwner is IProperty {
/// CanSet: true }`), specialized like `method`.
fn property_setter_of(
    env: &mut Env<'_, '_, '_, '_>,
    method: &ResolvedMethod,
) -> Option<ResolvedMethod> {
    let crate::typesystem::resolve::MethodOrigin::Def(id) = method.origin else {
        return None;
    };
    let (token, _) = env.ctx.ts.accessor_owner(id)?;
    if token >> 24 != 0x17 {
        return None;
    }
    let setter = env
        .ctx
        .ts
        .comp
        .module(id.module)
        .md
        .property_accessors(token & 0x00FF_FFFF)
        .1;
    if setter == 0 {
        return None;
    }
    let setter = env
        .ctx
        .ts
        .method_definition(crate::typesystem::members::MethodDefId {
            module: id.module,
            row: setter,
        })
        .ok()?;
    let setter = match (&method.class_args, method.declaring) {
        (Some(args), _) => env.ctx.ts.specialize(setter, Some(args.clone()), None),
        _ => setter,
    };
    let mut setter = setter;
    setter.declaring = method.declaring;
    Some(setter)
}

/// `ConvertCall`.
fn convert_call(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let mut target_converter = None;
    let (method, arguments) = if let Some(m) = match_get_method_from_handle(env, args[0]) {
        // A static method.
        let arguments = if args.len() == 2 {
            match_argument_list(env, args[1])
        } else {
            None
        };
        (m, arguments.unwrap_or_else(|| args[1..].to_vec()))
    } else {
        let m = match_get_method_from_handle(env, args[1])?;
        let arguments = if args.len() == 3 {
            match_argument_list(env, args[2])
        } else {
            None
        };
        let arguments = arguments.unwrap_or_else(|| args[2..].to_vec());
        if !matches!(env.f.inst(args[0]), Inst::LdNull) {
            target_converter = Some(convert_instruction(env, args[0], None)?);
        }
        (m, arguments)
    };
    let mut converted_arguments = convert_call_arguments(env, &arguments, &method.method)?;
    if env.method_full_name(&method.method) == "System.Reflection.MethodInfo.CreateDelegate"
        && method.method.params().is_ok_and(|p| p.len() == 2)
    {
        let target_method = match_get_method_from_handle(env, unpack_constant(env, args[0]))?;
        let delegate_type =
            match_get_type_from_handle(env, unpack_constant(env, *arguments.first()?))?;
        let ctors = env
            .ctx
            .ts
            .get_constructors(delegate_type, MemberOptions::DECLARED)
            .ok()?;
        let [ctor] = &ctors[..] else {
            return None;
        };
        let ctor = Env::method_operand(ctor.clone());
        if converted_arguments.len() < 2 {
            return None;
        }
        let target = converted_arguments.swap_remove(1);
        return Some(Box::new(move |env| {
            let target = target(env)?;
            let ftn = env.add(Inst::LdFtn {
                method: target_method,
            });
            Some(env.new_obj(ctor, vec![target, ftn]))
        }));
    }
    Some(Box::new(move |env| {
        let is_static = method.method.is_static;
        let mut call_args = Vec::new();
        if let Some(target_converter) = target_converter {
            let target = target_converter(env)?;
            let target_type = env.infer(target);
            let declaring = method
                .method
                .declaring
                .unwrap_or_else(|| env.unknown_type());
            call_args.push(prepare_call_target(env, declaring, target, target_type));
        }
        call_args.extend(build_all(env, converted_arguments)?);
        Some(env.call(!is_static, method, call_args))
    }))
}

/// `CallInstruction.ExpectedTypeForThisPointer(expectedType, null)`.
fn expected_type_for_this_pointer(env: &mut Env<'_, '_, '_, '_>, t: TypeId) -> StackType {
    if matches!(env.ctx.ts.data(t), TypeData::TypeParam { .. }) {
        return StackType::Ref;
    }
    match env.ctx.ts.is_reference_type(t) {
        Some(true) => StackType::Obj,
        Some(false) => StackType::Ref,
        None => StackType::Unknown,
    }
}

/// `PrepareCallTarget`: the address of a value-type target, or a boxed value where the method
/// is declared on a reference type; a conv bridges a known and an unknown type.
fn prepare_call_target(
    env: &mut Env<'_, '_, '_, '_>,
    expected_type: TypeId,
    target: NodeId,
    target_type: TypeId,
) -> NodeId {
    let mut result = match expected_type_for_this_pointer(env, expected_type) {
        StackType::Ref => {
            if env.result_type(target) == StackType::Ref {
                target
            } else if let Inst::LdLoc { var } = *env.f.inst(target) {
                let range = env.f.il_range(target);
                let address = env.add(Inst::LdLoca { var });
                env.f.add_il_range(address, range);
                address
            } else {
                env.add(Inst::AddressOf {
                    value: target,
                    ty: Env::operand(expected_type),
                })
            }
        }
        // The boxing of a value-type receiver is implicit in an expression tree; an
        // unconstrained type parameter may be a value type at runtime.
        StackType::Obj => {
            if env.ctx.ts.is_reference_type(target_type) != Some(true) {
                box_value(env, target, target_type)
            } else {
                target
            }
        }
        _ => target,
    };
    let expected_unknown = env.is_unknown(expected_type);
    let result_type = env.result_type(result);
    if expected_unknown && result_type != StackType::Unknown {
        result = env.conv(target, PrimitiveType::Unknown, Sign::None);
    } else if !expected_unknown && result_type == StackType::Unknown {
        // With missing references the unknown type is coerced to the expected one.
        let primitive = env.ctx.ts.primitive_type(expected_type);
        result = env.conv(result, primitive, Sign::None);
    }
    result
}

/// `UnpackConstant`: the value of `call Constant(value, type)`.
fn unpack_constant(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> NodeId {
    if is_call_to(env, inst, "System.Linq.Expressions.Expression", "Constant")
        && let Some((_, args)) = call_parts(env.f, inst)
        && args.len() == 2
    {
        return args[0];
    }
    inst
}

/// `ConvertCallArguments`: each argument with the parameter type as type hint.
fn convert_call_arguments(
    env: &mut Env<'_, '_, '_, '_>,
    arguments: &[NodeId],
    method: &ResolvedMethod,
) -> Option<Vec<Build>> {
    let params = method.params().ok()?;
    let mut converted = Vec::with_capacity(arguments.len());
    for (i, &argument) in arguments.iter().enumerate() {
        let expected = *params.get(i)?;
        converted.push(convert_instruction(env, argument, Some(expected))?);
    }
    Some(converted)
}

/// `ConvertCast`: `call Convert(expr, type)` becomes `expression.tree.cast T(expr)` (or the
/// operand, or a box); with a method, the (lifted) conversion operator call.
fn convert_cast(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    is_checked: bool,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let target_type = match_get_type_from_handle(env, args[1])?;
    let expr = convert_instruction(env, args[0], None)?;
    if args.len() == 3
        && let Some(unlifted) = match_get_method_from_handle(env, args[2])
    {
        return Some(Box::new(move |env| {
            let expr_inst = expr(env)?;
            let mut op_method = unlifted.clone();
            let ty = env.infer(expr_inst);
            if env.is_nullable(ty) {
                match lift(env, &unlifted) {
                    Some(lifted) => op_method = lifted,
                    None => {
                        return Some(env.add(Inst::ExpressionTreeCast {
                            ty: target_type,
                            arg: expr_inst,
                            is_checked,
                        }));
                    }
                }
            }
            Some(env.call(false, op_method, vec![expr_inst]))
        }));
    }
    Some(Box::new(move |env| {
        let expr_inst = expr(env)?;
        let operand_type = env.infer(expr_inst);
        let size = env.ctx.ts.type_size(operand_type);
        if size > 0 && size < 4 && env.is_known(target_type, KnownTypeCode::Int32) {
            return Some(expr_inst);
        }
        if env.ctx.ts.is_reference_type(operand_type) == Some(false)
            && env.ctx.ts.is_reference_type(target_type) == Some(true)
        {
            return Some(box_value(env, expr_inst, operand_type));
        }
        Some(env.add(Inst::ExpressionTreeCast {
            ty: target_type,
            arg: expr_inst,
            is_checked,
        }))
    }))
}

/// `ConvertCoalesce`: `call Coalesce(left, right)` becomes `if.notnull(left, right)`.
fn convert_coalesce(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let true_inst = convert_instruction(env, args[0], None)?;
    let fallback_inst = convert_instruction(env, args[1], None)?;
    Some(Box::new(move |env| {
        let true_value = true_inst(env)?;
        let fallback_value = fallback_inst(env)?;
        let true_type = env.infer(true_value);
        let fallback_type = env.infer(fallback_value);
        let mut kind = NullCoalescingKind::Ref;
        let true_non_nullable = env.underlying(true_type);
        let target_type = if env.is_nullable(true_type)
            && implicit_conversion(env, fallback_type, true_non_nullable)
        {
            kind = if env.is_nullable(fallback_type) {
                NullCoalescingKind::Nullable
            } else {
                NullCoalescingKind::NullableWithValueFallback
            };
            true_non_nullable
        } else if implicit_conversion(env, fallback_type, true_type) {
            true_type
        } else {
            fallback_type
        };
        Some(env.add(Inst::NullCoalescing {
            kind,
            ty: target_type,
            value: true_value,
            fallback: fallback_value,
        }))
    }))
}

/// `ConvertComparison`.
fn convert_comparison(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    kind: ComparisonKind,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let left = convert_instruction(env, args[0], None)?;
    let right = convert_instruction(env, args[1], None)?;
    if args.len() == 4
        && env.f.match_ldc_i4(args[2]).is_some()
        && let Some(method) = match_get_method_from_handle(env, args[3])
    {
        return Some(binary_call(left, right, method, true));
    }
    Some(Box::new(move |env| {
        let left_inst = left(env)?;
        let right_inst = right(env)?;
        let left_type = env.infer(left_inst);
        let right_type = env.infer(right_inst);
        if let Some(op) = resolve_user_defined_binary_operator(env, kind, left_type, right_type) {
            return Some(env.call(false, op, vec![left_inst, right_inst]));
        }
        if env.is_known(left_type, KnownTypeCode::String)
            && env.is_known(right_type, KnownTypeCode::String)
        {
            let name = match kind {
                ComparisonKind::Equality => "op_Equality",
                ComparisonKind::Inequality => "op_Inequality",
                _ => return None,
            };
            let methods = env
                .ctx
                .ts
                .get_methods(left_type, None, MemberOptions::default())
                .ok()?;
            let mut operator_method = None;
            for m in methods {
                if m.name != name || !env.ctx.ts.method_is_operator(&m) {
                    continue;
                }
                let Ok(params) = m.params() else {
                    continue;
                };
                if params.len() == 2
                    && env.is_known(params[0], KnownTypeCode::String)
                    && env.is_known(params[1], KnownTypeCode::String)
                {
                    operator_method = Some(m);
                    break;
                }
            }
            let operator_method = operator_method?;
            return Some(env.call(
                false,
                Env::method_operand(operator_method),
                vec![left_inst, right_inst],
            ));
        }
        // A comparison of type parameters not known to be reference types has no C# spelling.
        let is_unconstrained_type_param = |env: &mut Env<'_, '_, '_, '_>, t: TypeId| {
            matches!(env.ctx.ts.data(t), TypeData::TypeParam { .. })
                && env.ctx.ts.is_reference_type(t) != Some(true)
        };
        if is_unconstrained_type_param(env, left_type)
            || is_unconstrained_type_param(env, right_type)
        {
            return None;
        }
        let lifting = if env.is_nullable(left_type) {
            ComparisonLiftingKind::CSharp
        } else {
            ComparisonLiftingKind::None
        };
        let utype = env.underlying(left_type);
        let input = env.ctx.ts.stack_type(utype);
        let sign = env.ctx.ts.type_sign(utype);
        Some(env.add(Inst::Comp {
            kind,
            lifting,
            sign,
            input,
            left: left_inst,
            right: right_inst,
        }))
    }))
}

/// `ConvertCondition`: `call Condition(c, a, b)` becomes `if (c) a else b`.
fn convert_condition(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 3 {
        return None;
    }
    let condition = convert_instruction(env, args[0], None)?;
    let true_inst = convert_instruction(env, args[1], None)?;
    let false_inst = convert_instruction(env, args[2], None)?;
    Some(Box::new(move |env| {
        let condition_value = condition(env)?;
        let true_value = true_inst(env)?;
        let false_value = false_inst(env)?;
        let condition_type = env.infer(condition_value);
        if !env.is_known(condition_type, KnownTypeCode::Boolean) {
            return None;
        }
        let true_type = env.infer(true_value);
        let false_type = env.infer(false_value);
        let result_type = if type_erasure_equivalent(env, true_type, false_type)
            || is_implicit_reference_conversion(env, false_type, true_type)
        {
            true_type
        } else if is_implicit_reference_conversion(env, true_type, false_type) {
            false_type
        } else {
            return None;
        };
        Some(env.add(Inst::If {
            condition: condition_value,
            true_inst: true_value,
            false_inst: false_value,
            csharp_type: Some(result_type),
        }))
    }))
}

/// `NormalizeTypeVisitor.TypeErasure.EquivalentTypes`.
fn type_erasure_equivalent(env: &mut Env<'_, '_, '_, '_>, a: TypeId, b: TypeId) -> bool {
    env.ctx.ts.normalize(a) == env.ctx.ts.normalize(b)
}

/// `ConvertConstant`: the value of `call Constant(value[, type])`, cast to the declared type
/// of a boxed constant that infers differently.
fn convert_constant(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (m, args) = call_parts(env.f, invocation)?;
    if env.method_full_name(&m.method) != "System.Linq.Expressions.Expression.Constant" {
        return None;
    }
    let value = *args.first()?;
    // The two-argument overload passes typeof(T); legacy csc uses the one-argument overload
    // for display-class instances.
    if args.len() == 2 && match_get_type_from_handle(env, args[1]).is_none() {
        return None;
    }
    if let Inst::Box { arg, ty } = *env.f.inst(value) {
        let box_type = ty.ty;
        return Some(Box::new(move |env| {
            // A constant narrower than its stack type infers as int; the cast keeps the
            // declared type.
            let constant_value = convert_value(env, arg, invocation)?;
            let inferred = env.infer(constant_value);
            if !type_erasure_equivalent(env, inferred, box_type) {
                return Some(env.add(Inst::ExpressionTreeCast {
                    ty: box_type,
                    arg: constant_value,
                    is_checked: false,
                }));
            }
            Some(constant_value)
        }));
    }
    Some(Box::new(move |env| convert_value(env, value, invocation)))
}

/// `ConvertElementInit`: `call ElementInit(method, arguments)` becomes the `Add` call without
/// its target, which `convert_list_init` inserts.
fn convert_element_init(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let member = match_get_method_from_handle(env, args[0])?;
    let arguments = match_argument_list(env, args[1])?;
    let converted = convert_all(env, &arguments)?;
    Some(Box::new(move |env| {
        let is_static = member.method.is_static;
        let call_args = build_all(env, converted)?;
        Some(env.call(!is_static, member, call_args))
    }))
}

/// `ConvertField`: `call Field(target, field)` becomes `ldobj T(delayex.ldflda F(target))` or
/// `ldobj T(ldsflda F)`; a by-ref hint keeps the address.
fn convert_field(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    type_hint: Option<TypeId>,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let target_converter = if matches!(env.f.inst(args[0]), Inst::LdNull) {
        None
    } else {
        Some(convert_instruction(env, args[0], None)?)
    };
    let member = match_get_field_from_handle(env, args[1])?;
    Some(Box::new(move |env| {
        let field_type = member.field.ty;
        let mut inst = match target_converter {
            None => env.add(Inst::LdsFlda {
                field: member.clone(),
            }),
            Some(target_converter) => {
                let target = target_converter(env)?;
                let declaring = member.field.declaring.unwrap_or_else(|| env.unknown_type());
                let target = if env.ctx.ts.is_reference_type(declaring) == Some(true) {
                    target
                } else {
                    env.add(Inst::AddressOf {
                        value: target,
                        ty: Env::operand(declaring),
                    })
                };
                env.add(Inst::LdFlda {
                    target,
                    field: member.clone(),
                    delay_exceptions: true,
                })
            }
        };
        let keeps_address =
            type_hint.is_some_and(|h| is_by_ref(env, h)) && !env.ctx.ts.is_by_ref_like(field_type);
        if !keeps_address {
            inst = env.add(Inst::LdObj {
                target: inst,
                ty: Env::operand(field_type),
                is_volatile: false,
                unaligned: 0,
            });
        }
        Some(inst)
    }))
}

/// `ConvertInvoke`: `call Invoke(target, arguments)` becomes `callvirt Invoke(target, ...)`
/// of the target's delegate type.
fn convert_invoke(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let target_converter = convert_instruction(env, args[0], None)?;
    let arguments = match_argument_list(env, args[1])?;
    Some(Box::new(move |env| {
        let target = target_converter(env)?;
        let target_type = env.infer(target);
        let invoke = env
            .ctx
            .ts
            .get_delegate_invoke_method(target_type)
            .ok()
            .flatten()?;
        let converted = convert_call_arguments(env, &arguments, &invoke)?;
        let mut call_args = vec![target];
        call_args.extend(build_all(env, converted)?);
        Some(env.call(true, Env::method_operand(invoke), call_args))
    }))
}

/// `ConvertListInit`: `call ListInit(call New(...), elements)` becomes a collection
/// initializer block.
fn convert_list_init(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let new_obj = convert_instruction(env, args[0], None)?;
    let ctor = match_new(env, args[0])?;
    let arguments = if match_get_method_from_handle(env, args[1]).is_none() {
        match_argument_list(env, args[1])?
    } else {
        if args.len() != 3 {
            return None;
        }
        match_argument_list(env, args[2])?
    };
    if arguments.is_empty() {
        return None;
    }
    let mut converted: Vec<BuildOn> = Vec::with_capacity(arguments.len());
    for &argument in &arguments {
        if is_call_to(
            env,
            argument,
            "System.Linq.Expressions.Expression",
            "ElementInit",
        ) {
            let arg = convert_element_init(env, argument)?;
            converted.push(Box::new(move |env, v| {
                let a = arg(env)?;
                if !matches!(env.f.inst(a), Inst::Call(_) | Inst::CallVirt(_)) {
                    return None;
                }
                let load = env.ldloc(v);
                env.f.insert(a, 0, load);
                Some(a)
            }));
        } else {
            let arg = convert_instruction(env, argument, None)?;
            converted.push(Box::new(move |env, _| arg(env)));
        }
    }
    let declaring = ctor.method.declaring;
    Some(Box::new(move |env| {
        let ty = declaring.unwrap_or_else(|| env.unknown_type());
        let initializer = env.register_lambda_variable(VariableKind::InitializerTarget, ty);
        let final_inst = env.ldloc(initializer);
        let new_obj_inst = new_obj(env)?;
        let store = stloc(env, initializer, new_obj_inst);
        let mut instructions = vec![store];
        for f in converted {
            instructions.push(f(env, initializer)?);
        }
        Some(env.add(Inst::Block {
            kind: BlockKind::CollectionInitializer,
            instructions,
            final_inst,
        }))
    }))
}

fn stloc(env: &mut Env<'_, '_, '_, '_>, var: VarId, value: NodeId) -> NodeId {
    env.add(Inst::StLoc {
        var,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    })
}

/// `ConvertLogicOperator`: `call AndAlso(a, b)` becomes `if (a) b else ldc.i4 0` and
/// `call OrElse(a, b)` becomes `if (a) ldc.i4 1 else b`; with a method, its (lifted) call.
fn convert_logic_operator(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    and: bool,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let left = convert_instruction(env, args[0], None)?;
    let right = convert_instruction(env, args[1], None)?;
    match args.len() {
        2 => Some(Box::new(move |env| {
            let left_inst = left(env)?;
            let right_inst = right(env)?;
            let csharp_type = env.infer(right_inst);
            let (true_inst, false_inst) = if and {
                (right_inst, env.ldc_i4(0))
            } else {
                (env.ldc_i4(1), right_inst)
            };
            Some(env.add(Inst::If {
                condition: left_inst,
                true_inst,
                false_inst,
                csharp_type: Some(csharp_type),
            }))
        })),
        3 => {
            let method = match_get_method_from_handle(env, args[2])?;
            Some(binary_call(left, right, method, false))
        }
        4 => {
            env.f.match_ldc_i4(args[2])?;
            let method = match_get_method_from_handle(env, args[3])?;
            Some(binary_call(left, right, method, true))
        }
        _ => None,
    }
}

/// `ConvertMemberInit`: `call MemberInit(call New(...), bindings)` becomes an object
/// initializer block; only Expression.Bind bindings convert.
fn convert_member_init(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let new_obj = convert_instruction(env, args[0], None)?;
    let ctor = match_new(env, args[0])?;
    let arguments = match_argument_list(env, args[1])?;
    if arguments.is_empty() {
        return None;
    }
    let mut converted = Vec::with_capacity(arguments.len());
    for &argument in &arguments {
        if !is_call_to(env, argument, "System.Linq.Expressions.Expression", "Bind") {
            return None;
        }
        converted.push(convert_bind(env, argument)?);
    }
    let declaring = ctor.method.declaring;
    Some(Box::new(move |env| {
        let ty = declaring.unwrap_or_else(|| env.unknown_type());
        let initializer = env.register_lambda_variable(VariableKind::InitializerTarget, ty);
        let final_inst = env.ldloc(initializer);
        let new_obj_inst = new_obj(env)?;
        let store = stloc(env, initializer, new_obj_inst);
        let mut instructions = vec![store];
        for f in converted {
            instructions.push(f(env, initializer)?);
        }
        Some(env.add(Inst::Block {
            kind: BlockKind::ObjectInitializer,
            instructions,
            final_inst,
        }))
    }))
}

/// `ConvertNewArrayBounds`: `call NewArrayBounds(type, bounds)` becomes `newarr T(bounds)`.
fn convert_new_array_bounds(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let ty = match_get_type_from_handle(env, args[0])?;
    let arguments = match_argument_list(env, args[1])?;
    if arguments.is_empty() {
        return None;
    }
    let indices = convert_all(env, &arguments)?;
    Some(Box::new(move |env| {
        let indices = build_all(env, indices)?;
        Some(env.add(Inst::NewArr {
            ty: Env::operand(ty),
            indices,
        }))
    }))
}

/// `ConvertNewArrayInit`: `call NewArrayInit(type, values)` becomes an array initializer
/// block (`newarr T(ldc.i4 0)` without values).
fn convert_new_array_init(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let ty = match_get_type_from_handle(env, args[0])?;
    let arguments = match_argument_list(env, args[1])?;
    let array_type = env.ctx.ts.intern(TypeData::Array {
        elem: ty,
        rank: 1,
        nullability: Nullability::Oblivious,
    });
    if arguments.is_empty() {
        return Some(Box::new(move |env| {
            let zero = env.ldc_i4(0);
            Some(env.add(Inst::NewArr {
                ty: Env::operand(ty),
                indices: vec![zero],
            }))
        }));
    }
    let converted = convert_all(env, &arguments)?;
    Some(Box::new(move |env| {
        let variable = env.register_lambda_variable(VariableKind::InitializerTarget, array_type);
        let length = env.ldc_i4(converted.len() as i32);
        let newarr = env.add(Inst::NewArr {
            ty: Env::operand(ty),
            indices: vec![length],
        });
        let mut instructions = vec![stloc(env, variable, newarr)];
        for (i, f) in converted.into_iter().enumerate() {
            let value = f(env)?;
            let array = env.ldloc(variable);
            let index = env.ldc_i4(i as i32);
            let address = env.add(Inst::LdElema {
                ty: Env::operand(ty),
                array,
                indices: vec![index],
                delay_exceptions: true,
                is_read_only: false,
                with_system_index: false,
            });
            instructions.push(env.add(Inst::StObj {
                target: address,
                value,
                ty: Env::operand(ty),
                is_volatile: false,
                unaligned: 0,
            }));
        }
        let final_inst = env.ldloc(variable);
        Some(env.add(Inst::Block {
            kind: BlockKind::ArrayInitializer,
            instructions,
            final_inst,
        }))
    }))
}

/// `MatchNew`: the constructor `call New(...)` names.
fn match_new(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<MethodOperand> {
    let (m, args) = call_parts(env.f, invocation)?;
    if m.method.name != "New" {
        return None;
    }
    match args.len() {
        1 => {
            if let Some(ty) = match_get_type_from_handle(env, args[0]) {
                return parameterless_constructor(env, ty).map(Env::method_operand);
            }
            match_get_constructor_from_handle(env, args[0])
        }
        2 | 3 => match_get_constructor_from_handle(env, args[0]),
        _ => None,
    }
}

/// `type.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault()`.
fn parameterless_constructor(env: &mut Env<'_, '_, '_, '_>, ty: TypeId) -> Option<ResolvedMethod> {
    let ctors = env
        .ctx
        .ts
        .get_constructors(ty, MemberOptions::DECLARED)
        .ok()?;
    ctors
        .into_iter()
        .find(|c| c.params().is_ok_and(|p| p.is_empty()))
}

/// `ConvertNewObject`: `call New(...)` becomes `newobj ctor(arguments)`; the member list of
/// an anonymous type has no ILAst equivalent.
fn convert_new_object(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    match args.len() {
        1 => {
            if let Some(ty) = match_get_type_from_handle(env, args[0]) {
                let ctor = Env::method_operand(parameterless_constructor(env, ty)?);
                return Some(Box::new(move |env| Some(env.new_obj(ctor, Vec::new()))));
            }
            let ctor = match_get_constructor_from_handle(env, args[0])?;
            Some(Box::new(move |env| Some(env.new_obj(ctor, Vec::new()))))
        }
        2 | 3 => {
            let ctor = match_get_constructor_from_handle(env, args[0])?;
            let arguments = match_argument_list(env, args[1])?;
            let converted = convert_call_arguments(env, &arguments, &ctor.method)?;
            Some(Box::new(move |env| {
                let call_args = build_all(env, converted)?;
                Some(env.new_obj(ctor, call_args))
            }))
        }
        _ => None,
    }
}

/// `ConvertNotOperator`: `logic.not` of a bool, `bit.not` otherwise (both lifted on a
/// nullable); with a method, its call.
fn convert_not_operator(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    let &first = args.first()?;
    let argument = convert_instruction(env, first, None)?;
    match args.len() {
        1 => Some(Box::new(move |env| {
            let argument_inst = argument(env)?;
            let argument_type = env.infer(argument_inst);
            let underlying = env.underlying(argument_type);
            let is_lifted = env.is_nullable(argument_type);
            if env.is_known(underlying, KnownTypeCode::Boolean) {
                let zero = env.ldc_i4(0);
                return Some(env.add(Inst::Comp {
                    kind: ComparisonKind::Equality,
                    lifting: if is_lifted {
                        ComparisonLiftingKind::ThreeValuedLogic
                    } else {
                        ComparisonLiftingKind::None
                    },
                    sign: Sign::None,
                    input: StackType::I4,
                    left: argument_inst,
                    right: zero,
                }));
            }
            let underlying_stack = env.ctx.ts.stack_type(underlying);
            Some(env.add(Inst::BitNot {
                arg: argument_inst,
                lifted_underlying: is_lifted.then_some(underlying_stack),
            }))
        })),
        2 => {
            let method = match_get_method_from_handle(env, args[1])?;
            Some(Box::new(move |env| {
                let argument_inst = argument(env)?;
                Some(env.call(false, method, vec![argument_inst]))
            }))
        }
        _ => None,
    }
}

/// `ConvertProperty`: `call Property(target, accessor[, indices])` becomes the accessor call.
fn convert_property(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() < 2 {
        return None;
    }
    let target_converter = if matches!(env.f.inst(args[0]), Inst::LdNull) {
        None
    } else {
        Some(convert_instruction(env, args[0], None)?)
    };
    let member = match_get_method_from_handle(env, args[1])?;
    let arguments = if args.len() == 3 {
        match_argument_list(env, args[2])
    } else {
        None
    }
    .unwrap_or_default();
    let converted = convert_call_arguments(env, &arguments, &member.method)?;
    Some(Box::new(move |env| {
        let is_static = member.method.is_static;
        let mut call_args = Vec::new();
        if let Some(target_converter) = target_converter {
            let target = target_converter(env)?;
            let target_type = env.infer(target);
            let declaring = member
                .method
                .declaring
                .unwrap_or_else(|| env.unknown_type());
            call_args.push(prepare_call_target(env, declaring, target, target_type));
        }
        call_args.extend(build_all(env, converted)?);
        Some(env.call(!is_static, member, call_args))
    }))
}

/// `ConvertTypeAs`: `isinst T(value)`, unboxed for a nullable `T` (ECMA-335, III.4.6).
fn convert_type_as(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let converted = convert_instruction(env, args[0], None);
    let ty = match_get_type_from_handle(env, args[1])?;
    let converted = converted?;
    Some(Box::new(move |env| {
        let value = converted(env)?;
        let mut inst = env.add(Inst::IsInst {
            arg: value,
            ty: Env::operand(ty),
        });
        if env.is_nullable(ty) {
            inst = env.add(Inst::UnboxAny {
                arg: inst,
                ty: Env::operand(ty),
            });
        }
        Some(inst)
    }))
}

/// `ConvertTypeIs`: `comp.obj(isinst T(value) != ldnull)`.
fn convert_type_is(env: &mut Env<'_, '_, '_, '_>, invocation: NodeId) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    if args.len() != 2 {
        return None;
    }
    let converted = convert_instruction(env, args[0], None);
    let ty = match_get_type_from_handle(env, args[1])?;
    let converted = converted?;
    Some(Box::new(move |env| {
        let value = converted(env)?;
        let isinst = env.add(Inst::IsInst {
            arg: value,
            ty: Env::operand(ty),
        });
        let null = env.add(Inst::LdNull);
        let input = env.result_type(isinst);
        Some(env.add(Inst::Comp {
            kind: ComparisonKind::Inequality,
            lifting: ComparisonLiftingKind::None,
            sign: Sign::None,
            input,
            left: isinst,
            right: null,
        }))
    }))
}

/// `ConvertUnaryNumericOperator`: `call Negate(a)` becomes `binary.sub(0, a)`; with a method,
/// its call.
fn convert_unary_numeric_operator(
    env: &mut Env<'_, '_, '_, '_>,
    invocation: NodeId,
    op: BinaryNumericOperator,
    is_checked: Option<bool>,
) -> Option<Build> {
    let (_, args) = call_parts(env.f, invocation)?;
    let &first = args.first()?;
    let argument = convert_instruction(env, first, None)?;
    let is_checked = is_checked == Some(true);
    match args.len() {
        1 => Some(Box::new(move |env| {
            let argument_inst = argument(env)?;
            let argument_type = env.infer(argument_inst);
            let underlying = env.underlying(argument_type);
            let stack = env.ctx.ts.stack_type(underlying);
            let left = match stack {
                StackType::I4 => env.ldc_i4(0),
                StackType::I8 => env.add(Inst::LdcI8 { value: 0 }),
                StackType::I => {
                    let zero = env.ldc_i4(0);
                    env.conv(zero, PrimitiveType::I, Sign::None)
                }
                StackType::F4 => env.add(Inst::LdcF4 { bits: 0 }),
                StackType::F8 => env.add(Inst::LdcF8 { bits: 0 }),
                StackType::VT if env.is_known(underlying, KnownTypeCode::Decimal) => {
                    env.add(Inst::LdcDecimal { bits: [0; 4] })
                }
                _ => return None,
            };
            let sign = sign_for_operator(env, op, is_checked, argument_type);
            let is_lifted = env.is_nullable(argument_type);
            Some(env.add(Inst::BinaryNumeric {
                op,
                check_overflow: is_checked,
                sign,
                left_input: stack,
                right_input: stack,
                left,
                right: argument_inst,
                csharp_result_type: None,
                is_lifted,
            }))
        })),
        2 => {
            let method = match_get_method_from_handle(env, args[1])?;
            Some(Box::new(move |env| {
                let argument_inst = argument(env)?;
                Some(env.call(false, method, vec![argument_inst]))
            }))
        }
        _ => None,
    }
}

/// `BoxValue`: `box T(value)`, without an unchecked cast to `T` in front, which says nothing
/// the box does not.
fn box_value(env: &mut Env<'_, '_, '_, '_>, mut value: NodeId, ty: TypeId) -> NodeId {
    if let Inst::ExpressionTreeCast {
        ty: cast_type,
        arg,
        is_checked: false,
    } = *env.f.inst(value)
        && cast_type == ty
    {
        value = arg;
    }
    env.add(Inst::Box {
        arg: value,
        ty: Env::operand(ty),
    })
}

/// `ConvertValue`: the value operand of a converted `Expression.Constant` call; `context` is
/// that call.
fn convert_value(env: &mut Env<'_, '_, '_, '_>, value: NodeId, context: NodeId) -> Option<NodeId> {
    let Inst::LdLoc { var } = *env.f.inst(value) else {
        return Some(env.f.clone_subtree(value));
    };
    if is_expression_tree_parameter(env, var) {
        let Some(&(_, v)) = env.parameter_mapping.iter().find(|p| p.0 == var) else {
            return Some(env.f.clone_subtree(value));
        };
        let parent = env.f.node(context).parent;
        let parent_is_call = parent
            .is_some_and(|p| is_call_to(env, p, "System.Linq.Expressions.Expression", "Call"));
        if parent_is_call && env.f.var(v).stack_type.is_integer() {
            let range = env.f.il_range(value);
            let address = env.add(Inst::LdLoca { var: v });
            env.f.add_il_range(address, range);
            return Some(address);
        }
        return None;
    }
    if is_closure_reference(env, var) {
        if env.f.var(var).kind == VariableKind::Local {
            env.f.var_mut(var).kind = VariableKind::DisplayClassLocal;
        }
        if env.f.var(var).capture_scope.is_none() {
            let scope = std::iter::once(context)
                .chain(env.f.ancestors(context))
                .find(|&n| matches!(env.f.inst(n), Inst::BlockContainer { .. }));
            env.f.var_mut(var).capture_scope = scope;
            if let Some(scope) = scope {
                let function = env.f.enclosing_function(scope);
                env.f.add_captured_variable(function, var);
            }
        }
    }
    Some(value)
}

/// `IsClosureReference`: a single `stloc v(newobj DisplayClass..ctor())` of a potential
/// closure.
fn is_closure_reference(env: &mut Env<'_, '_, '_, '_>, v: VarId) -> bool {
    let var = env.f.var(v);
    if !var.is_single_definition() {
        return false;
    }
    let &[store] = &var.store_instructions[..] else {
        return false;
    };
    let Some((_, value)) = env.f.match_stloc(store) else {
        return false;
    };
    let Inst::NewObj(c) = env.f.inst(value) else {
        return false;
    };
    let Some(display) = c
        .method
        .method
        .declaring
        .and_then(|d| env.ctx.ts.definition(d))
    else {
        return false;
    };
    let decompiled = env.decompiled_type;
    env.ctx.ts.is_potential_closure_type(decompiled, display)
}

/// `MatchGetTypeFromHandle`: `call GetTypeFromHandle(ldtypetoken T)`.
fn match_get_type_from_handle(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> Option<TypeId> {
    if !is_call_to(env, inst, "System.Type", "GetTypeFromHandle") {
        return None;
    }
    let (_, args) = call_parts(env.f, inst)?;
    let &[arg] = &args[..] else {
        return None;
    };
    match env.f.inst(arg) {
        Inst::LdTypeToken { ty } => Some(ty.ty),
        _ => None,
    }
}

/// `castclass <class_name>(call GetMethodFromHandle(...))`.
fn match_cast_from_handle(
    env: &Env<'_, '_, '_, '_>,
    inst: NodeId,
    class_name: &str,
) -> Option<MemberOperand> {
    let Inst::CastClass { arg, ty } = *env.f.inst(inst) else {
        return None;
    };
    if env.full_name(ty.ty) != class_name {
        return None;
    }
    if !is_call_to(
        env,
        arg,
        "System.Reflection.MethodBase",
        "GetMethodFromHandle",
    ) {
        return None;
    }
    match_from_handle_parameter_list(env, arg)
}

/// `MatchGetMethodFromHandle`: the method of
/// `castclass MethodInfo(call GetMethodFromHandle(ldmembertoken M[, ldtypetoken T]))`.
fn match_get_method_from_handle(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> Option<MethodOperand> {
    match match_cast_from_handle(env, inst, "System.Reflection.MethodInfo")? {
        MemberOperand::Method(m) => Some(m),
        MemberOperand::Field(_) => None,
    }
}

/// `MatchGetConstructorFromHandle`: as `match_get_method_from_handle` with ConstructorInfo.
fn match_get_constructor_from_handle(
    env: &Env<'_, '_, '_, '_>,
    inst: NodeId,
) -> Option<MethodOperand> {
    match match_cast_from_handle(env, inst, "System.Reflection.ConstructorInfo")? {
        MemberOperand::Method(m) => Some(m),
        MemberOperand::Field(_) => None,
    }
}

/// `MatchGetFieldFromHandle`: the field of `call GetFieldFromHandle(ldmembertoken F)`.
fn match_get_field_from_handle(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> Option<FieldOperand> {
    if !is_call_to(
        env,
        inst,
        "System.Reflection.FieldInfo",
        "GetFieldFromHandle",
    ) {
        return None;
    }
    match match_from_handle_parameter_list(env, inst)? {
        MemberOperand::Field(f) => Some(f),
        MemberOperand::Method(_) => None,
    }
}

/// `MatchFromHandleParameterList`: `(ldmembertoken M)` or `(ldmembertoken M, ldtypetoken T)`.
fn match_from_handle_parameter_list(
    env: &Env<'_, '_, '_, '_>,
    call: NodeId,
) -> Option<MemberOperand> {
    let (_, args) = call_parts(env.f, call)?;
    let member = match (args.len(), args.first().map(|&a| env.f.inst(a))) {
        (1 | 2, Some(Inst::LdMemberToken { member })) => member.clone(),
        _ => return None,
    };
    if args.len() == 2 && !matches!(env.f.inst(args[1]), Inst::LdTypeToken { .. }) {
        return None;
    }
    Some(member)
}

/// `MatchArgumentList`: the values of an array initializer with dense indices, or none for
/// an empty parameter list.
fn match_argument_list(env: &Env<'_, '_, '_, '_>, inst: NodeId) -> Option<Vec<NodeId>> {
    let Inst::Block {
        kind: BlockKind::ArrayInitializer,
        instructions,
        ..
    } = env.f.inst(inst)
    else {
        return is_empty_parameter_list(env, inst).then(Vec::new);
    };
    let mut arguments = Vec::new();
    for &item in instructions {
        let Inst::StObj { target, value, .. } = *env.f.inst(item) else {
            continue;
        };
        let Inst::LdElema { indices, .. } = env.f.inst(target) else {
            return None;
        };
        // `Indices.Single()` throws on another count; the port declines.
        let &[index] = &indices[..] else {
            return None;
        };
        if env.f.match_ldc_i4(index) != Some(arguments.len() as i32) {
            return None;
        }
        arguments.push(value);
    }
    Some(arguments)
}

// shortcut: operator overload resolution is approximated for the operand types expression
// trees produce; port it with the semantics step (item 67).

/// `CSharpResolver.ResolveBinaryOperator(...).UserDefinedOperatorMethod` for a comparison:
/// the single user-defined operator of the operand types (or their base types) whose
/// parameters take the operands, lifted for nullable operands.
fn resolve_user_defined_binary_operator(
    env: &mut Env<'_, '_, '_, '_>,
    kind: ComparisonKind,
    left: TypeId,
    right: TypeId,
) -> Option<MethodOperand> {
    let name = match kind {
        ComparisonKind::Equality => "op_Equality",
        ComparisonKind::Inequality => "op_Inequality",
        ComparisonKind::LessThan => "op_LessThan",
        ComparisonKind::LessThanOrEqual => "op_LessThanOrEqual",
        ComparisonKind::GreaterThan => "op_GreaterThan",
        ComparisonKind::GreaterThanOrEqual => "op_GreaterThanOrEqual",
    };
    let lifted = env.is_nullable(left) || env.is_nullable(right);
    let (left_u, right_u) = (env.underlying(left), env.underlying(right));
    let mut candidates: Vec<ResolvedMethod> = Vec::new();
    for t in [left_u, right_u] {
        // The built-in operators of bool, char, the numeric types and decimal are no
        // user-defined operators, even where the framework declares them as such.
        if is_built_in_operator_type(env, t) {
            continue;
        }
        let Ok(methods) = env.ctx.ts.get_methods(t, None, MemberOptions::default()) else {
            continue;
        };
        for m in methods {
            if m.name == name
                && m.is_static
                && env.ctx.ts.method_is_operator(&m)
                && !candidates.iter().any(|c| same_method(c, &m))
            {
                candidates.push(m);
            }
        }
    }
    let mut applicable = Vec::new();
    for m in candidates {
        let Ok(params) = m.params() else {
            continue;
        };
        if params.len() != 2 {
            continue;
        }
        if implicit_conversion(env, left_u, params[0])
            && implicit_conversion(env, right_u, params[1])
        {
            applicable.push(m);
        }
    }
    let best = match applicable.len() {
        0 => return None,
        1 => applicable.pop()?,
        _ => {
            // The one taking the operand types themselves; otherwise ambiguous.
            let mut exact = Vec::new();
            for m in applicable {
                let params = m.params().ok()?;
                if type_erasure_equivalent(env, params[0], left_u)
                    && type_erasure_equivalent(env, params[1], right_u)
                {
                    exact.push(m);
                }
            }
            if exact.len() != 1 {
                return None;
            }
            exact.pop()?
        }
    };
    let operand = Env::method_operand(best);
    if lifted {
        return lift(env, &operand);
    }
    Some(operand)
}

/// `TypeCode.Boolean <= GetTypeCode(type) <= TypeCode.Decimal`.
fn is_built_in_operator_type(env: &mut Env<'_, '_, '_, '_>, t: TypeId) -> bool {
    use KnownTypeCode as K;
    matches!(
        known(env.ctx, t),
        Some(
            K::Boolean
                | K::Char
                | K::SByte
                | K::Byte
                | K::Int16
                | K::UInt16
                | K::Int32
                | K::UInt32
                | K::Int64
                | K::UInt64
                | K::Single
                | K::Double
                | K::Decimal
        )
    )
}

/// `CSharpConversions.ImplicitConversion(from, to).IsValid`.
fn implicit_conversion(env: &mut Env<'_, '_, '_, '_>, from: TypeId, to: TypeId) -> bool {
    super::csharp_semantics::implicit_conversion(env.ctx, from, to).is_valid()
}

/// `CSharpConversions.IsImplicitReferenceConversion`.
fn is_implicit_reference_conversion(
    env: &mut Env<'_, '_, '_, '_>,
    from: TypeId,
    to: TypeId,
) -> bool {
    super::csharp_semantics::is_implicit_reference_conversion(env.ctx, from, to)
}
