//! The ILAst: one arena per function, nodes addressed by `NodeId`, variables by `VarId`.
//!
//! `Inst` holds the instruction kinds the IL reader produces (later steps add the kinds their
//! transforms create). Each kind's result type, flags and inferred type follow
//! `IL/Instructions.tt` and the hand-written partial classes in `IL/Instructions/`.

use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::{ResolvedField, ResolvedMethod};
use crate::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};
use crate::util::interval::Interval;
use crate::util::long_set::LongSet;

use super::types::{PrimitiveType, Sign, StackType};

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct NodeId(pub u32);

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct VarId(pub u32);

/// `InstructionFlags`.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct Flags(pub u32);

impl Flags {
    pub const NONE: Flags = Flags(0);
    pub const MAY_READ_LOCALS: Flags = Flags(0x10);
    pub const MAY_WRITE_LOCALS: Flags = Flags(0x20);
    pub const SIDE_EFFECT: Flags = Flags(0x40);
    pub const MAY_THROW: Flags = Flags(0x100);
    pub const MAY_BRANCH: Flags = Flags(0x200);
    pub const MAY_UNWRAP_NULL: Flags = Flags(0x400);
    pub const END_POINT_UNREACHABLE: Flags = Flags(0x800);
    pub const CONTROL_FLOW: Flags = Flags(0x1000);

    pub fn contains(self, other: Flags) -> bool {
        self.0 & other.0 != 0
    }

    /// `SemanticHelper.CombineBranches`: the end point is unreachable only if it is on both.
    pub fn combine_branches(a: Flags, b: Flags) -> Flags {
        let and = Flags::END_POINT_UNREACHABLE.0;
        Flags((a.0 & b.0) | ((a.0 | b.0) & !and))
    }
}

impl std::ops::BitOr for Flags {
    type Output = Flags;
    fn bitor(self, rhs: Flags) -> Flags {
        Flags(self.0 | rhs.0)
    }
}

impl std::ops::BitOrAssign for Flags {
    fn bitor_assign(&mut self, rhs: Flags) {
        self.0 |= rhs.0;
    }
}

macro_rules! named_enum {
    ($(#[$meta:meta])* $name:ident { $($variant:ident),* $(,)? }) => {
        $(#[$meta])*
        #[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
        pub enum $name { $($variant),* }
        impl $name {
            pub fn name(self) -> &'static str {
                match self { $($name::$variant => stringify!($variant)),* }
            }
        }
    };
}

named_enum!(
    /// `BinaryNumericOperator`.
    BinaryNumericOperator { None, Add, Sub, Mul, Div, Rem, BitAnd, BitOr, BitXor, ShiftLeft, ShiftRight }
);
named_enum!(
    /// `ComparisonKind`.
    ComparisonKind { Equality, Inequality, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual }
);
named_enum!(
    /// `BlockKind`.
    BlockKind {
        ControlFlow, ArrayInitializer, CollectionInitializer, ObjectInitializer,
        StackAllocInitializer, CallInlineAssign, CallWithNamedArgs, DeconstructionConversions,
        DeconstructionAssignments, WithInitializer, InterpolatedString,
    }
);
named_enum!(
    /// `ContainerKind`.
    ContainerKind { Normal, Loop, Switch, While, DoWhile, For }
);
named_enum!(
    /// `VariableKind`.
    VariableKind {
        Local, PinnedLocal, PinnedRegionLocal, UsingLocal, ForeachLocal, InitializerTarget,
        Parameter, ExceptionStackSlot, ExceptionLocal, StackSlot, NamedArgument,
        DisplayClassLocal, PatternLocal, DeconstructionInitTemporary,
    }
);
named_enum!(
    /// `NopKind`.
    NopKind { Normal, Pop }
);
named_enum!(
    /// `ConversionKind`.
    ConversionKind {
        Invalid, Nop, IntToFloat, FloatToInt, FloatPrecisionChange, SignExtend, ZeroExtend,
        Truncate, StopGCTracking, StartGCTracking, ObjectInterior,
    }
);

impl ComparisonKind {
    pub fn is_equality_or_inequality(self) -> bool {
        matches!(self, ComparisonKind::Equality | ComparisonKind::Inequality)
    }

    /// `ComparisonKindExtensions.Negate`.
    pub fn negate(self) -> ComparisonKind {
        use ComparisonKind::*;
        match self {
            Equality => Inequality,
            Inequality => Equality,
            LessThan => GreaterThanOrEqual,
            LessThanOrEqual => GreaterThan,
            GreaterThan => LessThanOrEqual,
            GreaterThanOrEqual => LessThan,
        }
    }
}

/// `Conv.GetConversionKind`.
pub fn conversion_kind(target: PrimitiveType, input: StackType, sign: Sign) -> ConversionKind {
    use ConversionKind::*;
    use PrimitiveType as P;
    use StackType as S;
    let extend = |signed_target: bool| match sign {
        Sign::None if signed_target => SignExtend,
        Sign::None => ZeroExtend,
        Sign::Signed => SignExtend,
        Sign::Unsigned => ZeroExtend,
    };
    match target {
        P::I1 | P::I2 | P::U1 | P::U2 => match input {
            S::I4 | S::I8 | S::I => Truncate,
            S::F4 | S::F8 => FloatToInt,
            _ => Invalid,
        },
        P::I4 | P::U4 => match input {
            S::I4 => Nop,
            S::I | S::I8 => Truncate,
            S::F4 | S::F8 => FloatToInt,
            _ => Invalid,
        },
        P::I8 | P::U8 => match input {
            S::I4 | S::I => extend(target == P::I8),
            S::I8 => Nop,
            S::F4 | S::F8 => FloatToInt,
            S::Ref | S::Obj => StopGCTracking,
            _ => Invalid,
        },
        P::I | P::U => match input {
            S::I4 => extend(target == P::I),
            S::I => Nop,
            S::I8 => Truncate,
            S::F4 | S::F8 => FloatToInt,
            S::Ref | S::Obj => StopGCTracking,
            _ => Invalid,
        },
        P::R4 => match input {
            S::I4 | S::I | S::I8 => IntToFloat,
            S::F4 => Nop,
            S::F8 => FloatPrecisionChange,
            _ => Invalid,
        },
        P::R | P::R8 => match input {
            S::I4 | S::I | S::I8 => IntToFloat,
            S::F4 => FloatPrecisionChange,
            S::F8 => Nop,
            _ => Invalid,
        },
        P::Ref => match input {
            S::I4 | S::I | S::I8 => StartGCTracking,
            S::Obj => ObjectInterior,
            _ => Invalid,
        },
        _ => Invalid,
    }
}

/// `BinaryNumericInstruction.ComputeResultType`.
pub fn binary_result_type(
    op: BinaryNumericOperator,
    left: StackType,
    right: StackType,
) -> StackType {
    if matches!(
        op,
        BinaryNumericOperator::ShiftLeft | BinaryNumericOperator::ShiftRight
    ) {
        return left;
    }
    if left == StackType::Ref || right == StackType::Ref {
        return if left == StackType::Ref && right == StackType::Ref {
            StackType::I
        } else {
            StackType::Ref
        };
    }
    if left == right {
        left
    } else {
        StackType::Unknown
    }
}

/// `ILVariable`.
#[derive(Clone, Debug)]
pub struct Variable {
    pub kind: VariableKind,
    pub ty: TypeId,
    pub stack_type: StackType,
    /// Parameter index (-1 for `this`), local index, or IL offset of an exception handler.
    pub index: Option<i32>,
    pub name: String,
    pub has_generated_name: bool,
    pub initial_value_is_initialized: bool,
    pub uses_initial_value: bool,
    pub is_ref_read_only: bool,
    /// `RemoveIfRedundant`: set by RemoveInfeasiblePathTransform; such a stack slot is split
    /// like a local.
    pub remove_if_redundant: bool,
    /// `StateMachineField`: the field of the compiler-generated class the variable stands for.
    pub state_machine_field: Option<FieldOperand>,
    /// The connected `ldloc` instructions (`LoadInstructions`), maintained by the arena in the
    /// managed order: appended on connect, swap-removed on disconnect.
    pub load_instructions: Vec<NodeId>,
    /// The connected store instructions (`StoreInstructions`), without the initial value.
    pub store_instructions: Vec<NodeId>,
    /// The connected `ldloca` instructions (`AddressInstructions`).
    pub address_instructions: Vec<NodeId>,
}

impl Variable {
    /// `LoadCount`.
    pub fn load_count(&self) -> u32 {
        self.load_instructions.len() as u32
    }

    /// `AddressCount`.
    pub fn address_count(&self) -> u32 {
        self.address_instructions.len() as u32
    }

    /// `StoreCount`: the stores plus the initial value when it is used.
    pub fn store_count(&self) -> u32 {
        self.store_instructions.len() as u32 + u32::from(self.uses_initial_value)
    }

    /// `IsSingleDefinition`.
    pub fn is_single_definition(&self) -> bool {
        self.store_count() == 1 && self.address_count() == 0
    }

    /// `IsDead`.
    pub fn is_dead(&self) -> bool {
        self.store_instructions.is_empty()
            && self.load_instructions.is_empty()
            && self.address_instructions.is_empty()
    }
}

/// A method operand: the metadata token it was read from (resolved again by the
/// materializer, in the function's generic context) and its resolution.
#[derive(Clone, Debug)]
pub struct MethodOperand {
    pub token: u32,
    pub method: ResolvedMethod,
    /// Read in a state machine context (`GenericContext::class_tail`): the host resolves the
    /// token with the function's class and method type parameters as class parameters.
    pub state_machine_context: bool,
}

/// A field operand, as `MethodOperand`.
#[derive(Clone, Debug)]
pub struct FieldOperand {
    pub token: u32,
    pub field: ResolvedField,
    /// As `MethodOperand::state_machine_context`.
    pub state_machine_context: bool,
}

/// A type operand: the metadata token it was read from (0 for a known type the reader picks
/// itself) and the type.
#[derive(Clone, Copy, Debug)]
pub struct TypeOperand {
    pub token: u32,
    pub ty: TypeId,
}

/// A branch target before (`Offset`) and after `BlockBuilder` connects it.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum BranchTarget {
    Offset(i32),
    Block(NodeId),
}

/// The `ldtoken` operand.
#[derive(Clone, Debug)]
pub enum MemberOperand {
    Method(MethodOperand),
    Field(FieldOperand),
}

#[derive(Clone, Debug)]
pub enum Inst {
    InvalidBranch {
        message: String,
        expected: StackType,
    },
    InvalidExpression {
        message: String,
        expected: StackType,
        severity: String,
    },
    Nop {
        kind: NopKind,
    },
    BlockContainer {
        kind: ContainerKind,
        expected: Option<TypeId>,
        blocks: Vec<NodeId>,
    },
    Block {
        kind: BlockKind,
        instructions: Vec<NodeId>,
        final_inst: NodeId,
    },
    BinaryNumeric {
        op: BinaryNumericOperator,
        check_overflow: bool,
        sign: Sign,
        left_input: StackType,
        right_input: StackType,
        left: NodeId,
        right: NodeId,
    },
    BitNot {
        arg: NodeId,
    },
    Arglist,
    Branch {
        target: BranchTarget,
    },
    /// `target` is `None` for `endfinally` / `endfilter` until `BlockBuilder` assigns it.
    Leave {
        target: Option<NodeId>,
        value: NodeId,
    },
    If {
        condition: NodeId,
        true_inst: NodeId,
        false_inst: NodeId,
        /// `CSharpType`: the type of the value, None for a statement.
        csharp_type: Option<TypeId>,
    },
    Switch {
        value: NodeId,
        sections: Vec<NodeId>,
        /// `SwitchInstruction.Type`: the type of the switch value, when known.
        ty: Option<TypeId>,
        /// `IsLifted`: a switch on a nullable value, with a `case null` section.
        is_lifted: bool,
    },
    SwitchSection {
        labels: LongSet,
        body: NodeId,
        /// `HasNullLabel`: the section serves as `case null` of a lifted switch.
        has_null_label: bool,
        /// `IsCompilerGeneratedDefaultSection`: only the throw helper of a switch expression.
        is_compiler_generated_default_section: bool,
    },
    TryCatch {
        try_block: NodeId,
        handlers: Vec<NodeId>,
    },
    TryCatchHandler {
        filter: NodeId,
        body: NodeId,
        variable: VarId,
    },
    TryFinally {
        try_block: NodeId,
        finally_block: NodeId,
    },
    TryFault {
        try_block: NodeId,
        fault_block: NodeId,
    },
    DebugBreak,
    Comp {
        kind: ComparisonKind,
        sign: Sign,
        input: StackType,
        left: NodeId,
        right: NodeId,
    },
    Call(CallData),
    CallVirt(CallData),
    NewObj(CallData),
    CallIndirect {
        is_instance: bool,
        has_explicit_this: bool,
        /// A `TypeData::FnPtr` (or, without the FunctionPointers option, what the reader decoded).
        signature: TypeId,
        token: u32,
        function_pointer: NodeId,
        args: Vec<NodeId>,
    },
    Ckfinite {
        arg: NodeId,
    },
    Conv {
        arg: NodeId,
        input: StackType,
        input_sign: Sign,
        target: PrimitiveType,
        check_overflow: bool,
    },
    LdLoc {
        var: VarId,
    },
    LdLoca {
        var: VarId,
    },
    StLoc {
        var: VarId,
        value: NodeId,
        il_stack_was_empty: bool,
        is_stack_adjustment: bool,
    },
    AddressOf {
        value: NodeId,
        ty: TypeOperand,
    },
    LdStr {
        value: Vec<u16>,
    },
    LdcI4 {
        value: i32,
    },
    LdcI8 {
        value: i64,
    },
    LdcF4 {
        bits: u32,
    },
    LdcF8 {
        bits: u64,
    },
    /// `ldc.decimal`: the value as `decimal.GetBits`.
    LdcDecimal {
        bits: [u32; 4],
    },
    LdNull,
    LdFtn {
        method: MethodOperand,
    },
    LdVirtFtn {
        arg: NodeId,
        method: MethodOperand,
    },
    LdTypeToken {
        ty: TypeOperand,
    },
    LdMemberToken {
        member: MemberOperand,
    },
    LocAlloc {
        arg: NodeId,
    },
    /// `StringToInt`: the index of the string value in `map`, for a switch on strings.
    StringToInt {
        arg: NodeId,
        /// The values (`None` is null) and the labels they map to.
        map: Vec<(Option<Vec<u16>>, i32)>,
        /// `ExpectedType`: string or a span of char.
        ty: TypeId,
    },
    Cpblk {
        dest: NodeId,
        source: NodeId,
        size: NodeId,
        is_volatile: bool,
        unaligned: u8,
    },
    Initblk {
        address: NodeId,
        value: NodeId,
        size: NodeId,
        is_volatile: bool,
        unaligned: u8,
    },
    LdFlda {
        target: NodeId,
        field: FieldOperand,
        delay_exceptions: bool,
    },
    LdsFlda {
        field: FieldOperand,
    },
    CastClass {
        arg: NodeId,
        ty: TypeOperand,
    },
    IsInst {
        arg: NodeId,
        ty: TypeOperand,
    },
    LdObj {
        target: NodeId,
        ty: TypeOperand,
        is_volatile: bool,
        unaligned: u8,
    },
    LdObjIfRef {
        target: NodeId,
        ty: TypeOperand,
    },
    StObj {
        target: NodeId,
        value: NodeId,
        ty: TypeOperand,
        is_volatile: bool,
        unaligned: u8,
    },
    Box {
        arg: NodeId,
        ty: TypeOperand,
    },
    Unbox {
        arg: NodeId,
        ty: TypeOperand,
    },
    UnboxAny {
        arg: NodeId,
        ty: TypeOperand,
    },
    NewArr {
        ty: TypeOperand,
        indices: Vec<NodeId>,
    },
    DefaultValue {
        ty: TypeOperand,
        il_stack_was_empty: bool,
    },
    Throw {
        arg: NodeId,
    },
    Rethrow,
    SizeOf {
        ty: TypeOperand,
    },
    LdLen {
        array: NodeId,
        result: StackType,
    },
    LdElema {
        ty: TypeOperand,
        array: NodeId,
        indices: Vec<NodeId>,
        delay_exceptions: bool,
        is_read_only: bool,
    },
    MakeRefAny {
        arg: NodeId,
        ty: TypeOperand,
    },
    RefAnyType {
        arg: NodeId,
    },
    RefAnyValue {
        arg: NodeId,
        ty: TypeOperand,
    },
    /// `pinned.region V(init, body)`: a C# `fixed` statement.
    PinnedRegion {
        variable: VarId,
        init: NodeId,
        body: NodeId,
    },
    /// `get.pinnable.reference`: a reference to the first element of an array or string,
    /// or the result of a `GetPinnableReference` method (null for a null input).
    GetPinnableReference {
        arg: NodeId,
        ty: TypeId,
        method: Option<MethodOperand>,
    },
    /// `yield return value`.
    YieldReturn {
        value: NodeId,
    },
    /// `await value`.
    Await {
        value: NodeId,
        get_awaiter: Option<AwaiterMethod>,
        get_result: Option<AwaiterMethod>,
        /// `GetAwaiterCallOpCode`: the `GetAwaiter` call was a `callvirt`.
        get_awaiter_callvirt: bool,
    },
    /// The `DynamicInstruction`s: a C# `dynamic` operation (a call site of the C# runtime
    /// binder). `args` are the children in slot order.
    Dynamic {
        kind: DynamicKind,
        /// `CSharpBinderFlags`.
        binder_flags: i32,
        /// `CallingContext`.
        context: Option<TypeId>,
        /// The member name (`GetMember`, `SetMember`, `InvokeMember`, `IsEvent`).
        name: Option<String>,
        /// `ExpressionType` of an operator.
        operation: i32,
        /// The target type of `Convert`, the type of `InvokeConstructor`, the static target
        /// type of `InvokeMember`.
        ty: Option<TypeId>,
        /// The type arguments of `InvokeMember`.
        type_arguments: Option<Vec<TypeId>>,
        /// `CSharpArgumentInfo`s: one per argument, or the whole binder list for the kinds
        /// that take one (`InvokeConstructor` keeps the type's).
        arg_infos: Vec<DynamicArgInfo>,
        args: Vec<NodeId>,
    },
}

/// The `GetAwaiter` / `GetResult` method of an await.
#[derive(Clone, Debug)]
pub enum AwaiterMethod {
    Method(MethodOperand),
    /// The method of a dynamic await, on and returning `dynamic` (`CreateDynamicAwaiterMethod`).
    Dynamic(String),
}

/// The `DynamicInstruction` subclasses.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum DynamicKind {
    BinaryOperator,
    UnaryOperator,
    Convert,
    GetMember,
    SetMember,
    GetIndex,
    SetIndex,
    InvokeMember,
    InvokeConstructor,
    Invoke,
    IsEvent,
}

/// `CSharpArgumentInfo`.
#[derive(Clone, Debug)]
pub struct DynamicArgInfo {
    /// `CSharpArgumentInfoFlags`.
    pub flags: i32,
    pub name: Option<String>,
    pub compile_time_type: TypeId,
}

/// The shared state of `call`, `callvirt` and `newobj`.
#[derive(Clone, Debug)]
pub struct CallData {
    pub method: MethodOperand,
    pub args: Vec<NodeId>,
    pub is_tail: bool,
    pub constrained_to: Option<TypeOperand>,
    pub il_stack_was_empty: bool,
}

#[derive(Clone, Debug)]
pub struct Node {
    pub inst: Inst,
    pub parent: Option<NodeId>,
    /// `ChildIndex`: the position among the parent's children.
    pub child_index: u32,
    pub il_range: Interval,
    /// How often the node is reachable from the function root (`refCount`); a node is
    /// connected while this is positive. Moving a subtree briefly counts it twice.
    pub ref_count: u32,
    /// For a block: the connected branches to it, plus one when it is a connected
    /// container's entry point (`IncomingEdgeCount`).
    pub incoming_edge_count: u32,
    /// For a container: the connected `leave` instructions targeting it (`LeaveCount`).
    pub leave_count: u32,
    /// For a load, address load or store: its position in the variable's instruction list
    /// (`IndexInVariableInstructionMapping`).
    pub var_list_index: u32,
}

/// `Interval` merging of `ILInstruction.CombineILRange`.
pub fn combine_il_range(old: Interval, new: Interval) -> Interval {
    if old.is_empty() {
        return new;
    }
    if new.is_empty() {
        return old;
    }
    if new.start <= old.start {
        if new.end < old.start {
            new
        } else {
            Interval::new(new.start, new.end.max(old.end))
        }
    } else if new.start <= old.end {
        Interval::new(old.start, new.end.max(old.end))
    } else {
        old
    }
}

impl Inst {
    /// The children in slot order (`ILInstruction.Children`).
    pub fn children(&self) -> Vec<NodeId> {
        use Inst::*;
        match self {
            InvalidBranch { .. }
            | InvalidExpression { .. }
            | Nop { .. }
            | Arglist
            | Branch { .. }
            | DebugBreak
            | LdLoc { .. }
            | LdLoca { .. }
            | LdStr { .. }
            | LdcI4 { .. }
            | LdcI8 { .. }
            | LdcF4 { .. }
            | LdcF8 { .. }
            | LdcDecimal { .. }
            | LdNull
            | LdFtn { .. }
            | LdTypeToken { .. }
            | LdMemberToken { .. }
            | LdsFlda { .. }
            | DefaultValue { .. }
            | Rethrow
            | SizeOf { .. } => Vec::new(),
            BlockContainer { blocks, .. } => blocks.clone(),
            Block {
                instructions,
                final_inst,
                ..
            } => {
                let mut c = instructions.clone();
                c.push(*final_inst);
                c
            }
            BinaryNumeric { left, right, .. } | Comp { left, right, .. } => vec![*left, *right],
            BitNot { arg }
            | Ckfinite { arg }
            | LocAlloc { arg }
            | StringToInt { arg, .. }
            | CastClass { arg, .. }
            | IsInst { arg, .. }
            | Box { arg, .. }
            | Unbox { arg, .. }
            | UnboxAny { arg, .. }
            | Throw { arg }
            | MakeRefAny { arg, .. }
            | RefAnyType { arg }
            | RefAnyValue { arg, .. }
            | GetPinnableReference { arg, .. }
            | LdVirtFtn { arg, .. }
            | Conv { arg, .. } => vec![*arg],
            YieldReturn { value } | Await { value, .. } => vec![*value],
            PinnedRegion { init, body, .. } => vec![*init, *body],
            Leave { value, .. } => vec![*value],
            If {
                condition,
                true_inst,
                false_inst,
                ..
            } => vec![*condition, *true_inst, *false_inst],
            Switch {
                value, sections, ..
            } => {
                let mut c = vec![*value];
                c.extend(sections);
                c
            }
            SwitchSection { body, .. } => vec![*body],
            TryCatch {
                try_block,
                handlers,
            } => {
                let mut c = vec![*try_block];
                c.extend(handlers);
                c
            }
            TryCatchHandler { filter, body, .. } => vec![*filter, *body],
            TryFinally {
                try_block,
                finally_block,
            } => vec![*try_block, *finally_block],
            TryFault {
                try_block,
                fault_block,
            } => vec![*try_block, *fault_block],
            Call(c) | CallVirt(c) | NewObj(c) => c.args.clone(),
            CallIndirect {
                function_pointer,
                args,
                ..
            } => {
                // The function pointer is child 0, as in the managed `CallIndirect`.
                let mut c = vec![*function_pointer];
                c.extend(args);
                c
            }
            StLoc { value, .. } | AddressOf { value, .. } => vec![*value],
            Cpblk {
                dest, source, size, ..
            } => vec![*dest, *source, *size],
            Initblk {
                address,
                value,
                size,
                ..
            } => vec![*address, *value, *size],
            LdFlda { target, .. } | LdObj { target, .. } | LdObjIfRef { target, .. } => {
                vec![*target]
            }
            StObj { target, value, .. } => vec![*target, *value],
            NewArr { indices, .. } => indices.clone(),
            Dynamic { args, .. } => args.clone(),
            LdLen { array, .. } => vec![*array],
            LdElema { array, indices, .. } => {
                let mut c = vec![*array];
                c.extend(indices);
                c
            }
        }
    }

    /// Replaces child `index` (in `children()` order) by `new`.
    ///
    /// # Panics
    /// When `index` is out of range.
    pub fn set_child(&mut self, index: usize, new: NodeId) {
        use Inst::*;
        let slot: &mut NodeId = match self {
            BlockContainer { blocks, .. } => &mut blocks[index],
            Block {
                instructions,
                final_inst,
                ..
            } => {
                if index < instructions.len() {
                    &mut instructions[index]
                } else {
                    assert_eq!(index, instructions.len(), "child index out of range");
                    final_inst
                }
            }
            BinaryNumeric { left, right, .. } | Comp { left, right, .. } => {
                [left, right].into_iter().nth(index).expect("child index")
            }
            BitNot { arg }
            | Ckfinite { arg }
            | LocAlloc { arg }
            | StringToInt { arg, .. }
            | CastClass { arg, .. }
            | IsInst { arg, .. }
            | Box { arg, .. }
            | Unbox { arg, .. }
            | UnboxAny { arg, .. }
            | Throw { arg }
            | MakeRefAny { arg, .. }
            | RefAnyType { arg }
            | RefAnyValue { arg, .. }
            | GetPinnableReference { arg, .. }
            | LdVirtFtn { arg, .. }
            | Conv { arg, .. } => {
                assert_eq!(index, 0, "child index out of range");
                arg
            }
            PinnedRegion { init, body, .. } => {
                [init, body].into_iter().nth(index).expect("child index")
            }
            Leave { value, .. }
            | StLoc { value, .. }
            | AddressOf { value, .. }
            | YieldReturn { value }
            | Await { value, .. } => {
                assert_eq!(index, 0, "child index out of range");
                value
            }
            If {
                condition,
                true_inst,
                false_inst,
                ..
            } => [condition, true_inst, false_inst]
                .into_iter()
                .nth(index)
                .expect("child index"),
            Switch {
                value, sections, ..
            } => {
                if index == 0 {
                    value
                } else {
                    &mut sections[index - 1]
                }
            }
            SwitchSection { body, .. } => {
                assert_eq!(index, 0, "child index out of range");
                body
            }
            TryCatch {
                try_block,
                handlers,
            } => {
                if index == 0 {
                    try_block
                } else {
                    &mut handlers[index - 1]
                }
            }
            TryCatchHandler { filter, body, .. } => {
                [filter, body].into_iter().nth(index).expect("child index")
            }
            TryFinally {
                try_block,
                finally_block,
            } => [try_block, finally_block]
                .into_iter()
                .nth(index)
                .expect("child index"),
            TryFault {
                try_block,
                fault_block,
            } => [try_block, fault_block]
                .into_iter()
                .nth(index)
                .expect("child index"),
            Call(c) | CallVirt(c) | NewObj(c) => &mut c.args[index],
            CallIndirect {
                function_pointer,
                args,
                ..
            } => {
                if index == 0 {
                    function_pointer
                } else {
                    &mut args[index - 1]
                }
            }
            Cpblk {
                dest, source, size, ..
            } => [dest, source, size]
                .into_iter()
                .nth(index)
                .expect("child index"),
            Initblk {
                address,
                value,
                size,
                ..
            } => [address, value, size]
                .into_iter()
                .nth(index)
                .expect("child index"),
            LdFlda { target, .. } | LdObj { target, .. } | LdObjIfRef { target, .. } => {
                assert_eq!(index, 0, "child index out of range");
                target
            }
            StObj { target, value, .. } => {
                [target, value].into_iter().nth(index).expect("child index")
            }
            NewArr { indices, .. } => &mut indices[index],
            Dynamic { args, .. } => &mut args[index],
            LdLen { array, .. } => {
                assert_eq!(index, 0, "child index out of range");
                array
            }
            LdElema { array, indices, .. } => {
                if index == 0 {
                    array
                } else {
                    &mut indices[index - 1]
                }
            }
            _ => panic!("instruction has no children"),
        };
        *slot = new;
    }

    /// The flags of the instruction itself, without its children (`DirectFlags`).
    pub fn direct_flags(&self) -> Flags {
        use Inst::*;
        let throws = Flags::MAY_THROW;
        match self {
            InvalidBranch { .. } => throws | Flags::SIDE_EFFECT | Flags::END_POINT_UNREACHABLE,
            InvalidExpression { .. } => throws | Flags::SIDE_EFFECT,
            Branch { .. } => Flags::END_POINT_UNREACHABLE | Flags::MAY_BRANCH,
            Leave { .. } => Flags::MAY_BRANCH | Flags::END_POINT_UNREACHABLE,
            BlockContainer { .. }
            | If { .. }
            | Switch { .. }
            | TryCatch { .. }
            | TryFinally { .. }
            | TryFault { .. } => Flags::CONTROL_FLOW,
            TryCatchHandler { .. } => Flags::CONTROL_FLOW | Flags::MAY_WRITE_LOCALS,
            PinnedRegion { .. } => Flags::MAY_WRITE_LOCALS,
            YieldReturn { .. } => Flags::MAY_BRANCH | Flags::SIDE_EFFECT,
            Await { .. } => Flags::SIDE_EFFECT,
            Dynamic { .. } => throws | Flags::SIDE_EFFECT,
            DebugBreak => Flags::SIDE_EFFECT,
            Call(_) | CallVirt(_) | NewObj(_) | CallIndirect { .. } => throws | Flags::SIDE_EFFECT,
            Ckfinite { .. }
            | LdVirtFtn { .. }
            | LocAlloc { .. }
            | CastClass { .. }
            | Unbox { .. }
            | RefAnyValue { .. }
            | NewArr { .. }
            | LdLen { .. } => throws,
            BinaryNumeric {
                op, check_overflow, ..
            } => {
                if *check_overflow
                    || matches!(op, BinaryNumericOperator::Div | BinaryNumericOperator::Rem)
                {
                    throws
                } else {
                    Flags::NONE
                }
            }
            LdLoc { .. } => Flags::MAY_READ_LOCALS,
            StLoc { .. } => Flags::MAY_WRITE_LOCALS,
            Cpblk { .. } | Initblk { .. } | LdObj { .. } | LdObjIfRef { .. } | StObj { .. } => {
                throws | Flags::SIDE_EFFECT
            }
            UnboxAny { .. } => throws | Flags::SIDE_EFFECT,
            LdFlda {
                delay_exceptions, ..
            }
            | LdElema {
                delay_exceptions, ..
            } => {
                if *delay_exceptions {
                    Flags::NONE
                } else {
                    throws
                }
            }
            Throw { .. } | Rethrow => throws | Flags::END_POINT_UNREACHABLE,
            _ => Flags::NONE,
        }
    }
}

/// An ILAst function under construction or transformation: the arena, the variables and the
/// root container.
#[derive(Clone, Debug, Default)]
pub struct Function {
    pub nodes: Vec<Node>,
    pub variables: Vec<Variable>,
    /// The `BlockContainer` body.
    pub body: Option<NodeId>,
}

impl Function {
    pub fn node(&self, id: NodeId) -> &Node {
        &self.nodes[id.0 as usize]
    }

    pub fn inst(&self, id: NodeId) -> &Inst {
        &self.node(id).inst
    }

    pub fn inst_mut(&mut self, id: NodeId) -> &mut Inst {
        &mut self.nodes[id.0 as usize].inst
    }

    pub fn var(&self, id: VarId) -> &Variable {
        &self.variables[id.0 as usize]
    }

    pub fn var_mut(&mut self, id: VarId) -> &mut Variable {
        &mut self.variables[id.0 as usize]
    }

    pub fn add_var(&mut self, v: Variable) -> VarId {
        self.variables.push(v);
        VarId(self.variables.len() as u32 - 1)
    }

    /// Adds a node; its children become its own (their parent and child index are set).
    pub fn add(&mut self, inst: Inst) -> NodeId {
        let id = NodeId(self.nodes.len() as u32);
        let children = inst.children();
        self.nodes.push(Node {
            inst,
            parent: None,
            child_index: 0,
            il_range: Interval::default(),
            ref_count: 0,
            incoming_edge_count: 0,
            leave_count: 0,
            var_list_index: 0,
        });
        for (i, child) in children.into_iter().enumerate() {
            self.adopt(id, child, i);
        }
        id
    }

    fn adopt(&mut self, parent: NodeId, child: NodeId, index: usize) {
        let node = &mut self.nodes[child.0 as usize];
        node.parent = Some(parent);
        node.child_index = index as u32;
    }

    /// Re-numbers the parent links of all children of `id` after its child list changed.
    pub fn relink(&mut self, id: NodeId) {
        for (i, child) in self.inst(id).children().into_iter().enumerate() {
            self.adopt(id, child, i);
        }
    }

    pub fn add_il_range(&mut self, id: NodeId, range: Interval) {
        let node = &mut self.nodes[id.0 as usize];
        node.il_range = combine_il_range(node.il_range, range);
    }

    pub fn set_il_range(&mut self, id: NodeId, range: Interval) {
        self.nodes[id.0 as usize].il_range = range;
    }

    pub fn il_range(&self, id: NodeId) -> Interval {
        self.node(id).il_range
    }

    /// `ReplaceWith`: puts `new` in the place `old` has in its parent.
    ///
    /// # Panics
    /// When `old` has no parent.
    pub fn replace(&mut self, old: NodeId, new: NodeId) {
        let node = self.node(old);
        let parent = node.parent.expect("replaced node has a parent");
        let index = node.child_index as usize;
        let entry = self.entry_point(parent);
        self.inst_mut(parent).set_child(index, new);
        self.adopt(parent, new, index);
        // `old` keeps its (now stale) parent, as in the managed tree.
        if self.is_connected(parent) {
            // `new` may be a child of `old`: count it before `old` releases its children.
            self.add_ref(new);
            self.release_ref(old);
        }
        self.entry_point_changed(parent, entry);
    }

    /// The instruction list of a block or the block list of a container.
    fn list_mut(&mut self, id: NodeId) -> &mut Vec<NodeId> {
        match self.inst_mut(id) {
            Inst::Block { instructions, .. } => instructions,
            Inst::BlockContainer { blocks, .. } => blocks,
            Inst::Switch { sections, .. } => sections,
            Inst::TryCatch { handlers, .. } => handlers,
            Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => &mut c.args,
            Inst::Dynamic { args, .. } => args,
            Inst::NewArr { indices, .. } | Inst::LdElema { indices, .. } => indices,
            other => panic!("{other:?} has no instruction list"),
        }
    }

    /// Inserts `item` at `index` of the list of `owner` (`InstructionCollection.Insert`).
    pub fn insert(&mut self, owner: NodeId, index: usize, item: NodeId) {
        let entry = self.entry_point(owner);
        self.list_mut(owner).insert(index, item);
        if self.is_connected(owner) {
            self.add_ref(item);
        }
        // Only the children from `index` on move; relinking all of them would make filling a
        // block quadratic.
        let (offset, trailing) = match self.inst(owner) {
            Inst::Block { final_inst, .. } => (0, Some(*final_inst)),
            Inst::Switch { .. } | Inst::TryCatch { .. } | Inst::LdElema { .. } => (1, None),
            _ => (0, None),
        };
        let len = self.list(owner).len();
        for i in index..len {
            let child = self.list(owner)[i];
            self.adopt(owner, child, offset + i);
        }
        if let Some(t) = trailing {
            self.adopt(owner, t, offset + len);
        }
        self.entry_point_changed(owner, entry);
    }

    /// Appends `item` to the list of `owner` (`InstructionCollection.Add`).
    pub fn push(&mut self, owner: NodeId, item: NodeId) {
        let len = self.list(owner).len();
        self.insert(owner, len, item);
    }

    /// Removes and returns the items from `index` on (`RemoveRange` to the end).
    pub fn truncate(&mut self, owner: NodeId, index: usize) -> Vec<NodeId> {
        let mut items = self.list(owner).to_vec();
        let removed = items.split_off(index);
        let ops = removed.iter().map(|&r| (r, false)).collect();
        self.replace_items(owner, items, ops);
        removed
    }

    /// Replaces the whole list of `owner` (`ReplaceList`). Items may already be in the list;
    /// as in the managed collection, each position adds its new item before it releases the
    /// old one.
    pub fn set_list(&mut self, owner: NodeId, items: Vec<NodeId>) {
        let old = self.list(owner).to_vec();
        let mut ops = Vec::with_capacity(items.len() + old.len());
        for (i, &item) in items.iter().enumerate() {
            ops.push((item, true));
            if let Some(&o) = old.get(i) {
                ops.push((o, false));
            }
        }
        ops.extend(old.iter().skip(items.len()).map(|&o| (o, false)));
        self.replace_items(owner, items, ops);
    }

    /// Removes the item at `index` of the list of `owner` (`RemoveAt`).
    pub fn remove_at(&mut self, owner: NodeId, index: usize) -> NodeId {
        let mut items = self.list(owner).to_vec();
        let removed = items.remove(index);
        self.replace_items(owner, items, vec![(removed, false)]);
        removed
    }

    /// Keeps the items of the list of `owner` that `keep` accepts (`RemoveAll` inverted).
    /// As in the managed loop, each rejected item is released before the next one is tested,
    /// so the predicate sees the counts that earlier removals changed.
    pub fn retain(&mut self, owner: NodeId, mut keep: impl FnMut(&Function, NodeId) -> bool) {
        let connected = self.is_connected(owner);
        let mut items = Vec::new();
        let mut removed = false;
        for item in self.list(owner).to_vec() {
            if keep(self, item) {
                items.push(item);
            } else {
                removed = true;
                if connected {
                    self.release_ref(item);
                }
            }
        }
        if removed {
            self.replace_items(owner, items, Vec::new());
        }
    }

    /// Installs `items` as the list of `owner`, then connects (`true`) and releases (`false`)
    /// the given nodes in order when `owner` is connected, and moves the entry point's edge.
    fn replace_items(&mut self, owner: NodeId, items: Vec<NodeId>, ops: Vec<(NodeId, bool)>) {
        let entry = self.entry_point(owner);
        // Removed items keep their (now stale) parent, as in the managed tree.
        *self.list_mut(owner) = items;
        self.relink(owner);
        if self.is_connected(owner) {
            for (node, connect) in ops {
                if connect {
                    self.add_ref(node);
                } else {
                    self.release_ref(node);
                }
            }
        }
        self.entry_point_changed(owner, entry);
    }

    /// `Block.Remove`: takes a block that is not the entry point out of its container by
    /// moving the container's last block into its place (`SwapRemoveAt`).
    pub fn remove_block(&mut self, block: NodeId) {
        let container = self.node(block).parent.expect("block in a container");
        let index = self.node(block).child_index as usize;
        assert!(index > 0, "the entry point cannot be removed");
        self.swap_remove_block_at(container, index);
    }

    /// `Blocks.SwapRemoveAt`: takes the block at `index` (not the entry point) out of the
    /// container by moving the last block into its place.
    pub fn swap_remove_block_at(&mut self, container: NodeId, index: usize) {
        assert!(index > 0, "the entry point cannot be removed");
        let mut blocks = self.list(container).to_vec();
        let block = blocks.swap_remove(index);
        self.replace_items(container, blocks, vec![(block, false)]);
    }

    /// `Remove`: takes `id` out of its parent's list.
    pub fn remove(&mut self, id: NodeId) {
        let node = self.node(id);
        let parent = node.parent.expect("removed node has a parent");
        let offset = match self.inst(parent) {
            Inst::Switch { .. } | Inst::TryCatch { .. } | Inst::LdElema { .. } => 1,
            _ => 0,
        };
        self.remove_at(parent, node.child_index as usize - offset);
    }

    /// The instruction list of a block or the block list of a container.
    pub fn list(&self, id: NodeId) -> &[NodeId] {
        match self.inst(id) {
            Inst::Block { instructions, .. } => instructions,
            Inst::BlockContainer { blocks, .. } => blocks,
            Inst::Switch { sections, .. } => sections,
            Inst::TryCatch { handlers, .. } => handlers,
            Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => &c.args,
            Inst::Dynamic { args, .. } => args,
            Inst::NewArr { indices, .. } | Inst::LdElema { indices, .. } => indices,
            _ => &[],
        }
    }

    /// The nodes under `root` (inclusive) in post-order, as a snapshot.
    pub fn descendants_post(&self, root: NodeId) -> Vec<NodeId> {
        let mut out = Vec::new();
        let mut stack = vec![(root, false)];
        while let Some((id, expanded)) = stack.pop() {
            if expanded {
                out.push(id);
                continue;
            }
            stack.push((id, true));
            for child in self.inst(id).children().into_iter().rev() {
                stack.push((child, false));
            }
        }
        out
    }

    /// `BlockContainer.LeaveCount`: the connected `leave` instructions targeting `container`.
    /// A container outside the function tree has none, as in the managed arena.
    pub fn leave_count(&self, container: NodeId) -> u32 {
        self.node(container).leave_count
    }

    /// The root of the tree `id` belongs to.
    pub fn root_of(&self, mut id: NodeId) -> NodeId {
        while let Some(p) = self.node(id).parent {
            id = p;
        }
        id
    }

    /// `ILInstruction.Flags`.
    pub fn flags(&self, id: NodeId) -> Flags {
        use Inst::*;
        let inst = self.inst(id);
        let union = |ids: &[NodeId]| ids.iter().fold(Flags::NONE, |f, &c| f | self.flags(c));
        match inst {
            If {
                condition,
                true_inst,
                false_inst,
                ..
            } => {
                Flags::CONTROL_FLOW
                    | self.flags(*condition)
                    | Flags::combine_branches(self.flags(*true_inst), self.flags(*false_inst))
            }
            Switch {
                value, sections, ..
            } => {
                let combined = sections.iter().fold(Flags::END_POINT_UNREACHABLE, |f, &s| {
                    Flags::combine_branches(f, self.flags(s))
                });
                self.flags(*value) | Flags::CONTROL_FLOW | combined
            }
            TryCatch {
                try_block,
                handlers,
            } => {
                let combined = handlers.iter().fold(self.flags(*try_block), |f, &h| {
                    Flags::combine_branches(f, self.flags(h))
                });
                combined | Flags::CONTROL_FLOW
            }
            TryFault {
                try_block,
                fault_block,
            } => {
                let fault = Flags(self.flags(*fault_block).0 & !Flags::END_POINT_UNREACHABLE.0);
                self.flags(*try_block) | fault | Flags::CONTROL_FLOW
            }
            BlockContainer { blocks, .. } => {
                let mut flags = Flags::CONTROL_FLOW | union(blocks);
                if self.leave_count(id) == 0 {
                    flags |= Flags::END_POINT_UNREACHABLE;
                } else {
                    flags = Flags(flags.0 & !Flags::END_POINT_UNREACHABLE.0);
                }
                flags
            }
            // A checked conversion may throw, but that is not one of its direct flags.
            Conv {
                arg,
                check_overflow,
                ..
            } => {
                let flags = self.flags(*arg);
                if *check_overflow {
                    flags | Flags::MAY_THROW
                } else {
                    flags
                }
            }
            _ => inst.direct_flags() | union(&inst.children()),
        }
    }

    pub fn has_flag(&self, id: NodeId, flag: Flags) -> bool {
        self.flags(id).contains(flag)
    }

    pub fn has_direct_flag(&self, id: NodeId, flag: Flags) -> bool {
        self.inst(id).direct_flags().contains(flag)
    }

    /// `ILInstruction.ResultType`.
    pub fn result_type(&self, ts: &mut TypeSystem<'_, '_>, id: NodeId) -> StackType {
        use Inst::*;
        match self.inst(id) {
            If { csharp_type, .. } => csharp_type.map_or(StackType::Void, |t| ts.stack_type(t)),
            GetPinnableReference { .. } => StackType::Ref,
            Await { get_result, .. } => match get_result {
                Some(AwaiterMethod::Method(m)) => ts.stack_type(m.method.ret),
                Some(AwaiterMethod::Dynamic(_)) => StackType::Obj,
                None => StackType::Unknown,
            },
            Dynamic {
                kind,
                operation,
                ty,
                ..
            } => match kind {
                DynamicKind::Convert | DynamicKind::InvokeConstructor => {
                    ty.map_or(StackType::Unknown, |t| ts.stack_type(t))
                }
                DynamicKind::IsEvent => StackType::I4,
                // IsFalse and IsTrue (ExpressionType 84, 83) produce a bool.
                DynamicKind::UnaryOperator if matches!(operation, 83 | 84) => StackType::I4,
                _ => StackType::Obj,
            },
            Nop { .. }
            | PinnedRegion { .. }
            | YieldReturn { .. }
            | Branch { .. }
            | Leave { .. }
            | SwitchSection { .. }
            | TryCatchHandler { .. }
            | DebugBreak
            | Ckfinite { .. }
            | Cpblk { .. }
            | Initblk { .. }
            | Throw { .. }
            | Rethrow
            | TryCatch { .. }
            | TryFinally { .. }
            | TryFault { .. }
            | Switch { .. } => StackType::Void,
            InvalidBranch { expected, .. } | InvalidExpression { expected, .. } => *expected,
            BlockContainer { expected, .. } => {
                expected.map_or(StackType::Void, |t| ts.stack_type(t))
            }
            Block { final_inst, .. } => self.result_type(ts, *final_inst),
            BinaryNumeric {
                op,
                left_input,
                right_input,
                ..
            } => binary_result_type(*op, *left_input, *right_input),
            BitNot { arg } => self.result_type(ts, *arg),
            Arglist
            | LdTypeToken { .. }
            | LdMemberToken { .. }
            | MakeRefAny { .. }
            | RefAnyType { .. } => StackType::VT,
            Comp { .. } | SizeOf { .. } | LdcI4 { .. } | StringToInt { .. } => StackType::I4,
            Call(c) | CallVirt(c) => ts.stack_type(c.method.method.ret),
            NewObj(c) => match c.method.method.declaring {
                Some(d) => ts.stack_type(d),
                None => StackType::Unknown,
            },
            CallIndirect { signature, .. } => {
                let ret = match ts.data(*signature) {
                    TypeData::FnPtr(f) => f.ret,
                    _ => *signature,
                };
                ts.stack_type(ret)
            }
            Conv { target, .. } => target.stack_type(),
            LdLoc { var } | StLoc { var, .. } => self.var(*var).stack_type,
            LdLoca { .. }
            | AddressOf { .. }
            | LdsFlda { .. }
            | LdObjIfRef { .. }
            | Unbox { .. }
            | LdElema { .. }
            | RefAnyValue { .. } => StackType::Ref,
            LdStr { .. } | LdNull | IsInst { .. } | Box { .. } | NewArr { .. } => StackType::Obj,
            LdcI8 { .. } => StackType::I8,
            LdcF4 { .. } => StackType::F4,
            LdcF8 { .. } => StackType::F8,
            LdcDecimal { .. } => StackType::VT,
            LdFtn { .. } | LdVirtFtn { .. } | LocAlloc { .. } => StackType::I,
            LdFlda { target, .. } => {
                if self.result_type(ts, *target).is_integer() {
                    StackType::I
                } else {
                    StackType::Ref
                }
            }
            CastClass { ty, .. }
            | LdObj { ty, .. }
            | UnboxAny { ty, .. }
            | DefaultValue { ty, .. } => ts.stack_type(ty.ty),
            StObj { ty, unaligned, .. } => {
                if *unaligned == 0 {
                    ts.stack_type(ty.ty)
                } else {
                    StackType::Void
                }
            }
            LdLen { result, .. } => *result,
        }
    }

    /// `ILInstruction.InferType` for the instructions the reader puts on the stack.
    pub fn infer_type(&self, ts: &mut TypeSystem<'_, '_>, id: NodeId) -> TypeId {
        use Inst::*;
        let known = |ts: &mut TypeSystem<'_, '_>, code| ts.known_type(code);
        let by_ref = |ts: &mut TypeSystem<'_, '_>, t| ts.intern(TypeData::ByRef(t));
        match self.inst(id) {
            Await { get_result, .. } => match get_result {
                Some(AwaiterMethod::Method(m)) => m.method.ret,
                Some(AwaiterMethod::Dynamic(_)) => ts.intern(TypeData::Dynamic),
                None => ts.intern(TypeData::UnknownSpecial),
            },
            Dynamic {
                kind,
                operation,
                ty,
                ..
            } => match kind {
                DynamicKind::Convert | DynamicKind::InvokeConstructor => {
                    ty.unwrap_or_else(|| ts.intern(TypeData::UnknownSpecial))
                }
                DynamicKind::IsEvent => known(ts, KnownTypeCode::Boolean),
                DynamicKind::UnaryOperator if matches!(operation, 83 | 84) => {
                    known(ts, KnownTypeCode::Boolean)
                }
                _ => ts.intern(TypeData::Dynamic),
            },
            InvalidBranch { expected, .. } | InvalidExpression { expected, .. } => {
                find_stack_type(ts, *expected)
            }
            If {
                csharp_type: Some(t),
                ..
            } => *t,
            GetPinnableReference { ty, .. } => by_ref(ts, *ty),
            Nop { .. }
            | Branch { .. }
            | Leave { .. }
            | DebugBreak
            | Ckfinite { .. }
            | SwitchSection { .. }
            | TryCatchHandler { .. }
            | If { .. }
            | PinnedRegion { .. }
            | YieldReturn { .. }
            | Switch { .. }
            | TryCatch { .. }
            | TryFinally { .. }
            | TryFault { .. }
            | Cpblk { .. }
            | Initblk { .. }
            | Throw { .. }
            | Rethrow => known(ts, KnownTypeCode::Void),
            BlockContainer { expected, .. } => match expected {
                Some(t) => *t,
                None => find_stack_type(ts, StackType::Void),
            },
            Block { final_inst, .. } => self.infer_type(ts, *final_inst),
            BinaryNumeric {
                op,
                left_input,
                right_input,
                ..
            } => find_stack_type(ts, binary_result_type(*op, *left_input, *right_input)),
            BitNot { arg } => self.infer_type(ts, *arg),
            Arglist => known(ts, KnownTypeCode::RuntimeArgumentHandle),
            Comp { .. } => known(ts, KnownTypeCode::Boolean),
            Call(c) | CallVirt(c) => c.method.method.ret,
            NewObj(c) => c
                .method
                .method
                .declaring
                .unwrap_or_else(|| ts.intern(TypeData::UnknownSpecial)),
            CallIndirect { signature, .. } => match ts.data(*signature) {
                TypeData::FnPtr(f) => f.ret,
                _ => *signature,
            },
            Conv { target, .. } => {
                let code = target.to_known();
                if code != KnownTypeCode::None {
                    known(ts, code)
                } else {
                    find_stack_type(ts, target.stack_type())
                }
            }
            LdLoc { var } | StLoc { var, .. } => self.var(*var).ty,
            LdLoca { var } => {
                let t = self.var(*var).ty;
                by_ref(ts, t)
            }
            AddressOf { ty, .. }
            | LdObjIfRef { ty, .. }
            | Unbox { ty, .. }
            | LdElema { ty, .. }
            | RefAnyValue { ty, .. } => by_ref(ts, ty.ty),
            LdStr { .. } => known(ts, KnownTypeCode::String),
            LdcI4 { .. } | SizeOf { .. } | StringToInt { .. } => known(ts, KnownTypeCode::Int32),
            LdcI8 { .. } => known(ts, KnownTypeCode::Int64),
            LdcF4 { .. } => known(ts, KnownTypeCode::Single),
            LdcF8 { .. } => known(ts, KnownTypeCode::Double),
            LdcDecimal { .. } => known(ts, KnownTypeCode::Decimal),
            LdNull | IsInst { .. } | Box { .. } => known(ts, KnownTypeCode::Object),
            LdFtn { .. } | LdVirtFtn { .. } => known(ts, KnownTypeCode::IntPtr),
            LdTypeToken { .. } | RefAnyType { .. } => known(ts, KnownTypeCode::RuntimeTypeHandle),
            LdMemberToken { member } => known(
                ts,
                match member {
                    MemberOperand::Field(_) => KnownTypeCode::RuntimeFieldHandle,
                    MemberOperand::Method(_) => KnownTypeCode::RuntimeMethodHandle,
                },
            ),
            LocAlloc { .. } => {
                let void = known(ts, KnownTypeCode::Void);
                ts.intern(TypeData::Pointer(void))
            }
            LdFlda { target, field, .. } => {
                let field_type = field.field.ty;
                if self.result_type(ts, *target).is_integer() {
                    ts.intern(TypeData::Pointer(field_type))
                } else {
                    by_ref(ts, field_type)
                }
            }
            LdsFlda { field } => by_ref(ts, field.field.ty),
            CastClass { ty, .. }
            | LdObj { ty, .. }
            | StObj { ty, .. }
            | UnboxAny { ty, .. }
            | DefaultValue { ty, .. } => ty.ty,
            NewArr { ty, indices } => ts.intern(TypeData::Array {
                elem: ty.ty,
                rank: indices.len() as u32,
                nullability: Nullability::Oblivious,
            }),
            LdLen { result, .. } => find_stack_type(ts, *result),
            MakeRefAny { .. } => known(ts, KnownTypeCode::TypedReference),
        }
    }
}

/// `ICompilation.FindType(StackType)`.
pub fn find_stack_type(ts: &mut TypeSystem<'_, '_>, st: StackType) -> TypeId {
    match st {
        StackType::Unknown => ts.intern(TypeData::UnknownSpecial),
        StackType::Ref => {
            let byte = ts.known_type(KnownTypeCode::Byte);
            ts.intern(TypeData::ByRef(byte))
        }
        _ => ts.known_type(st.to_known(Sign::None)),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn nop(f: &mut Function) -> NodeId {
        f.add(Inst::Nop {
            kind: NopKind::Normal,
        })
    }

    #[test]
    fn add_sets_parent_and_child_index() {
        let mut f = Function::default();
        let a = f.add(Inst::LdcI4 { value: 1 });
        let b = f.add(Inst::LdcI4 { value: 2 });
        let add = f.add(Inst::BinaryNumeric {
            op: BinaryNumericOperator::Add,
            check_overflow: false,
            sign: Sign::None,
            left_input: StackType::I4,
            right_input: StackType::I4,
            left: a,
            right: b,
        });
        assert_eq!(f.node(a).parent, Some(add));
        assert_eq!(f.node(b).parent, Some(add));
        assert_eq!((f.node(a).child_index, f.node(b).child_index), (0, 1));
        assert_eq!(f.inst(add).children(), [a, b]);
    }

    #[test]
    fn replace_rewires_the_parent() {
        let mut f = Function::default();
        let a = f.add(Inst::LdcI4 { value: 1 });
        let throw = f.add(Inst::Throw { arg: a });
        let b = f.add(Inst::LdNull);
        f.replace(a, b);
        assert_eq!(f.inst(throw).children(), [b]);
        assert_eq!(f.node(b).parent, Some(throw));
        // The replaced node keeps its stale parent, as in the managed tree.
        assert_eq!(f.node(a).parent, Some(throw));
    }

    #[test]
    fn branches_end_the_block_and_if_combines_both_arms() {
        let mut f = Function::default();
        let br = f.add(Inst::Branch {
            target: BranchTarget::Offset(4),
        });
        assert!(f.has_direct_flag(br, Flags::END_POINT_UNREACHABLE));
        assert!(f.has_flag(br, Flags::MAY_BRANCH));
        let cond = f.add(Inst::LdcI4 { value: 1 });
        let other = nop(&mut f);
        let iff = f.add(Inst::If {
            condition: cond,
            true_inst: br,
            false_inst: other,
            csharp_type: None,
        });
        assert!(!f.has_flag(iff, Flags::END_POINT_UNREACHABLE));
        assert!(f.has_flag(iff, Flags::MAY_BRANCH));
    }

    #[test]
    fn container_end_point_depends_on_leaves() {
        let mut f = Function::default();
        let final_inst = nop(&mut f);
        let throw_arg = f.add(Inst::LdNull);
        let throw = f.add(Inst::Throw { arg: throw_arg });
        let block = f.add(Inst::Block {
            kind: BlockKind::ControlFlow,
            instructions: vec![throw],
            final_inst,
        });
        let container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Normal,
            expected: None,
            blocks: vec![block],
        });
        f.connect_root(container);
        assert!(f.has_flag(container, Flags::END_POINT_UNREACHABLE));

        let value = nop(&mut f);
        let leave = f.add(Inst::Leave {
            target: Some(container),
            value,
        });
        f.replace(throw, leave);
        assert_eq!(f.leave_count(container), 1);
        assert!(!f.has_flag(container, Flags::END_POINT_UNREACHABLE));
    }

    #[test]
    fn checked_conversion_throws_without_a_direct_flag() {
        let mut f = Function::default();
        let arg = f.add(Inst::LdcI4 { value: 1 });
        let conv = f.add(Inst::Conv {
            arg,
            input: StackType::I4,
            input_sign: Sign::Signed,
            target: PrimitiveType::I1,
            check_overflow: true,
        });
        assert!(f.has_flag(conv, Flags::MAY_THROW));
        assert!(!f.has_direct_flag(conv, Flags::MAY_THROW));
    }

    #[test]
    fn il_ranges_combine_like_the_managed_code() {
        let r = |a, b| Interval::new(a, b);
        assert_eq!(combine_il_range(Interval::default(), r(2, 4)), r(2, 4));
        assert_eq!(combine_il_range(r(2, 4), r(4, 8)), r(2, 8));
        assert_eq!(combine_il_range(r(4, 8), r(0, 2)), r(0, 2));
        assert_eq!(combine_il_range(r(4, 8), r(10, 12)), r(4, 8));
    }

    #[test]
    fn conversion_kinds() {
        use ConversionKind::*;
        assert_eq!(
            conversion_kind(PrimitiveType::I8, StackType::I4, Sign::None),
            SignExtend
        );
        assert_eq!(
            conversion_kind(PrimitiveType::U8, StackType::I4, Sign::None),
            ZeroExtend
        );
        assert_eq!(
            conversion_kind(PrimitiveType::U8, StackType::I4, Sign::Signed),
            SignExtend
        );
        assert_eq!(
            conversion_kind(PrimitiveType::I, StackType::Ref, Sign::None),
            StopGCTracking
        );
        assert_eq!(
            conversion_kind(PrimitiveType::Ref, StackType::Obj, Sign::None),
            ObjectInterior
        );
        assert_eq!(
            conversion_kind(PrimitiveType::R8, StackType::F4, Sign::None),
            FloatPrecisionChange
        );
        assert_eq!(
            conversion_kind(PrimitiveType::I4, StackType::Obj, Sign::None),
            Invalid
        );
    }

    #[test]
    fn binary_result_types() {
        use BinaryNumericOperator::*;
        assert_eq!(
            binary_result_type(ShiftLeft, StackType::I8, StackType::I4),
            StackType::I8
        );
        assert_eq!(
            binary_result_type(Sub, StackType::Ref, StackType::Ref),
            StackType::I
        );
        assert_eq!(
            binary_result_type(Add, StackType::Ref, StackType::I),
            StackType::Ref
        );
        assert_eq!(
            binary_result_type(Add, StackType::I4, StackType::I8),
            StackType::Unknown
        );
    }
}

#[cfg(test)]
mod list_tests {
    use super::*;

    fn ldc(f: &mut Function, value: i32) -> NodeId {
        f.add(Inst::LdcI4 { value })
    }

    fn block(f: &mut Function, instructions: Vec<NodeId>) -> NodeId {
        let final_inst = f.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        f.add(Inst::Block {
            kind: BlockKind::ControlFlow,
            instructions,
            final_inst,
        })
    }

    #[test]
    fn insert_push_and_truncate_keep_child_indices() {
        let mut f = Function::default();
        let a = ldc(&mut f, 1);
        let b = block(&mut f, vec![a]);
        let c = ldc(&mut f, 2);
        f.insert(b, 0, c);
        assert_eq!(f.list(b), [c, a]);
        assert_eq!((f.node(c).child_index, f.node(a).child_index), (0, 1));
        let d = ldc(&mut f, 3);
        f.push(b, d);
        assert_eq!(f.list(b), [c, a, d]);
        let removed = f.truncate(b, 1);
        assert_eq!(removed, [a, d]);
        assert_eq!(f.list(b), [c]);
        // Removed items keep their stale parent, as in the managed tree.
        assert_eq!(f.node(a).parent, Some(b));
        // The final instruction follows the list.
        let Inst::Block { final_inst, .. } = *f.inst(b) else {
            unreachable!()
        };
        assert_eq!(f.node(final_inst).child_index, 1);
    }

    #[test]
    fn descendants_are_post_order() {
        let mut f = Function::default();
        let a = ldc(&mut f, 1);
        let b = ldc(&mut f, 2);
        let add = f.add(Inst::BinaryNumeric {
            op: BinaryNumericOperator::Add,
            check_overflow: false,
            sign: Sign::None,
            left_input: StackType::I4,
            right_input: StackType::I4,
            left: a,
            right: b,
        });
        let throw = f.add(Inst::Throw { arg: add });
        assert_eq!(f.descendants_post(throw), [a, b, add, throw]);
    }
}
