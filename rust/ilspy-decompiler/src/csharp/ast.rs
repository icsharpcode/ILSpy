//! The C# syntax tree the builders produce (`ICSharpCode.Decompiler.CSharp.Syntax`), as an
//! arena. A node holds its children by id, in the order the managed node's slots list them,
//! and the annotations the AST transforms and the output read.

use crate::il::ast::NodeId;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::TypeId;

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub struct AstId(pub u32);

/// A syntax node.
#[derive(Clone, Debug)]
pub enum AstNode {
    /// `BlockStatement`.
    Block { statements: Vec<AstId> },
    /// `ReturnStatement`, `return;` without an expression.
    Return { expression: Option<AstId> },
    /// `EmptyStatement`.
    Empty,
    /// `ExpressionBuilder.ConvertConstantValue(rr, allowImplicitConversion)`: the expression
    /// `TypeSystemAstBuilder.ConvertConstantValue` makes of the constant, which the host
    /// builds (a literal, an enum member reference, a cast, ...).
    ConstantValue {
        ty: TypeId,
        value: ConstValue,
        allow_implicit_conversion: bool,
    },
    /// `PrimitiveExpression` of a string.
    StringLiteral { value: Vec<u16> },
    /// `NullReferenceExpression`.
    Null,
    /// `IdentifierExpression`.
    Identifier { name: String },
    /// `ThisReferenceExpression`.
    This,
    /// `BaseReferenceExpression`.
    Base,
    /// `MemberReferenceExpression`: `target.name`.
    MemberReference { target: AstId, name: String },
    /// `TypeReferenceExpression` of `ExpressionBuilder.ConvertType(ty)`, which the host builds.
    TypeReference { ty: TypeId },
    /// `DirectionExpression` with `FieldDirection.Ref`.
    RefDirection { expression: AstId },
    /// `AssignmentExpression` with `=`.
    Assignment { left: AstId, right: AstId },
    /// `ExpressionStatement`.
    ExpressionStatement { expression: AstId },
    /// `InvocationExpression`: `target(arguments)`.
    Invocation {
        target: AstId,
        arguments: Vec<AstId>,
    },
    /// `NamedArgumentExpression`: `name: expression`.
    NamedArgument { name: String, expression: AstId },
    /// `IfElseStatement`, without `else` when `false_statement` is `None`.
    IfElse {
        condition: AstId,
        true_statement: AstId,
        false_statement: Option<AstId>,
    },
    /// `UnaryOperatorExpression` with `UnaryOperatorType.Not`.
    LogicNot { expression: AstId },
    /// `SwitchStatement`: the value and the `SwitchSection` nodes.
    Switch {
        expression: AstId,
        sections: Vec<AstId>,
    },
    /// `SwitchSection`: its case labels (`None` is `default`) and statements.
    SwitchSection {
        labels: Vec<Option<AstId>>,
        statements: Vec<AstId>,
    },
    /// `AssignmentExpression` of a compound operator (`AssignmentOperatorType` from `Add`),
    /// with the checked or unchecked annotation of `overflow`.
    CompoundAssignment {
        op: u8,
        left: AstId,
        right: AstId,
        overflow: Overflow,
    },
    /// `UnaryOperatorExpression` of `PostIncrement` (or `PostDecrement`).
    PostIncrement {
        increment: bool,
        expression: AstId,
        overflow: Overflow,
    },
    /// `TupleExpression`, elements with a name written as named arguments; the host's
    /// `TupleResolveResult` takes its assembly from `value_tuple`.
    Tuple {
        elements: Vec<AstId>,
        names: Vec<Option<String>>,
        value_tuple: TypeId,
        /// Whether the resolve result carries the names (a converted literal's does not).
        names_in_rr: bool,
    },
    /// `LocalFunctionDeclarationStatement` of the local function its IL instruction is, with
    /// this body; the host writes the declaration of the function's reduced method.
    LocalFunctionDeclaration { body: AstId },
    /// `SwitchExpression`: the value and the (pattern, body) arms.
    SwitchExpression {
        expression: AstId,
        arms: Vec<(AstId, AstId)>,
    },
    /// `ConvertConstantValue(rr, allowImplicitConversion: false, displayAsHex)`: a constant
    /// operand of a bitwise operator.
    HexConstant {
        ty: TypeId,
        value: ConstValue,
        display_as_hex: bool,
    },
    /// `UnaryOperatorExpression` of `BitNot`, whose operator the host resolves.
    BitNot { expression: AstId },
    /// `ThrowExpression`: `throw expression`.
    ThrowExpression { expression: AstId },
    /// `AsExpression`: `expression as ty`.
    As { expression: AstId, ty: TypeId },
    /// `UnaryOperatorExpression` with `UnaryOperatorType.Await`.
    Await { expression: AstId },
    /// `ConditionalExpression`: `condition ? true_expression : false_expression`.
    Conditional {
        condition: AstId,
        true_expression: AstId,
        false_expression: AstId,
        /// Whether the host resolves it with `ResolveConditional` (branches of different
        /// types, one converted to the other's), rather than as an operator over the branches.
        resolve: bool,
    },
    /// `IdentifierExpression` with type arguments, which the host builds with `ConvertType`.
    GenericIdentifier {
        name: String,
        type_arguments: Vec<TypeId>,
    },
    /// `MemberReferenceExpression` with type arguments, which the host builds with
    /// `ConvertType`.
    GenericMemberReference {
        target: AstId,
        name: String,
        type_arguments: Vec<TypeId>,
    },
    /// `CastExpression` to `ExpressionBuilder.ConvertType(ty)`, with
    /// `AddCheckedBlocks.UncheckedAnnotation` when `unchecked`.
    Cast {
        ty: TypeId,
        expression: AstId,
        unchecked: bool,
    },
    /// `DirectionExpression` with `FieldDirection.Out`, with `UseImplicitlyTypedOutAnnotation`
    /// when `implicitly_typed` (`out var`).
    OutDirection {
        expression: AstId,
        implicitly_typed: bool,
    },
    /// `DirectionExpression` of `in`: `in expression`.
    InDirection { expression: AstId },
    /// `TypeOfExpression` of `ExpressionBuilder.ConvertType(ty)`.
    TypeOf { ty: TypeId },
    /// `TryCatchStatement`.
    TryCatch {
        try_block: AstId,
        catch_clauses: Vec<AstId>,
        finally_block: Option<AstId>,
    },
    /// `CatchClause`: `catch (Type name) when (condition) body`, without the parts that are
    /// `None`.
    CatchClause {
        variable_name: Option<String>,
        ty: Option<TypeId>,
        condition: Option<AstId>,
        body: AstId,
    },
    /// `WhileStatement`; a condition of `None` is the literal `true` without resolve result.
    While {
        condition: Option<AstId>,
        body: AstId,
    },
    /// `DoWhileStatement`.
    DoWhile { body: AstId, condition: AstId },
    /// `ForStatement` without initializers.
    For {
        condition: AstId,
        iterators: Vec<AstId>,
        body: AstId,
    },
    /// `BreakStatement`.
    Break,
    /// `ContinueStatement`.
    Continue,
    /// `TupleExpression` of the targets of a deconstruction, without resolve result.
    DeconstructionTuple { elements: Vec<AstId> },
    /// `LabelStatement` of the label `name`.
    Label { name: String },
    /// `GotoStatement` to the label `name`.
    Goto { name: String },
    /// `IndexerExpression` over an array: `target[indices]`.
    Indexer { target: AstId, indices: Vec<AstId> },
    /// `UsingStatement`: `using (resource) body`, with a declaration of `declaration` (of its
    /// type, annotated with its `ILVariableResolveResult`) initialized with the resource.
    Using {
        declaration: Option<crate::il::ast::VarId>,
        resource: AstId,
        body: AstId,
        /// `IsAsync`: `await using`.
        is_async: bool,
    },
    /// `LambdaExpression` with the parameters of the function its IL instruction is, with
    /// an expression body or a block; `lambda` indexes `Ast::lambdas`.
    Lambda {
        body: AstId,
        expression_body: bool,
        lambda: usize,
    },
    /// `InterpolatedStringExpression`.
    InterpolatedString { contents: Vec<InterpolatedContent> },
    /// `NamedExpression` of an object initializer: `name = expression`.
    Named { name: String, expression: AstId },
    /// `ArrayCreateExpression` of the element type with the dimensions as arguments and, for
    /// an array initializer, the initializer, whose elements the host writes with
    /// `UseSpecialConstants` as given.
    ArrayCreate {
        element_type: TypeId,
        arguments: Vec<AstId>,
        initializer: Option<AstId>,
        special_constants: bool,
    },
    /// `ArrayInitializerExpression`.
    ArrayInitializer { elements: Vec<AstId> },
    /// `YieldReturnStatement`.
    YieldReturn { expression: AstId },
    /// `YieldBreakStatement`.
    YieldBreak,
    /// `UnaryOperatorExpression` of `NullConditional` (`expression?`), or of
    /// `NullConditionalRewrap` (the end of a `?.` chain) when `rewrap`.
    NullConditional { expression: AstId, rewrap: bool },
    /// `BinaryOperatorExpression` of `IsPattern`: `left is pattern`.
    IsPattern { left: AstId, pattern: AstId },
    /// `DeclarationExpression` of a pattern: the variable's type (or `var`) and its name.
    DeclarationPattern {
        variable: crate::il::ast::VarId,
        var_keyword: bool,
    },
    /// `RecursivePatternExpression`: `T { Member: pattern, ... } designation`, the type and the
    /// sub-patterns optional.
    RecursivePattern {
        ty: Option<TypeId>,
        sub_patterns: Vec<SubPattern>,
        designator: Option<crate::il::ast::VarId>,
    },
    /// `UnaryOperatorExpression` of a pattern operator (`PatternOperator`) over a pattern.
    PatternUnary {
        operator: PatternOperator,
        pattern: AstId,
    },
    /// `IsExpression`: `expression is T`, the tuple type written as its underlying type.
    IsType { expression: AstId, ty: TypeId },
    /// `DefaultValueExpression`: `default(T)`, or the `default` literal without a type.
    DefaultValue { ty: Option<TypeId> },
    /// `ForeachStatement`: `foreach (T variable in collection) body`, with the variable's type,
    /// and the `ForeachAnnotation` of the instructions it was made of.
    Foreach {
        variable: crate::il::ast::VarId,
        collection: AstId,
        body: AstId,
        get_enumerator: NodeId,
        move_next: NodeId,
        get_current: NodeId,
        /// `IsAsync`: `await foreach`.
        is_async: bool,
    },
    /// `ObjectCreateExpression` of a delegate from a method reference, with the
    /// `ConversionResolveResult` of the method group conversion the host computes.
    DelegateCreate {
        ty: TypeId,
        reference: AstId,
        method: crate::il::ast::MethodOperand,
        is_virtual: bool,
    },
    /// `LockStatement`.
    Lock { expression: AstId, body: AstId },
    /// `ThrowStatement`, a rethrow without an expression.
    Throw { expression: Option<AstId> },
    /// `ObjectCreateExpression` of `ExpressionBuilder.ConvertType(ty)`, which the host builds.
    ObjectCreate {
        ty: TypeId,
        arguments: Vec<AstId>,
        initializer: Option<Vec<AstId>>,
    },
    /// `BinaryOperatorExpression`.
    BinaryOperator {
        op: BinaryOp,
        left: AstId,
        right: AstId,
        overflow: Overflow,
    },
}

/// The `AddCheckedBlocks` annotation of an arithmetic operator.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Overflow {
    /// None: a comparison, a logic or bitwise operator, or floating point arithmetic.
    None = 0,
    /// `CheckedAnnotation`.
    Checked = 1,
    /// `UncheckedAnnotation`.
    Unchecked = 2,
}

/// `BinaryOperatorType`, with its values.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum BinaryOp {
    BitwiseAnd = 1,
    BitwiseOr = 2,
    ConditionalAnd = 3,
    ConditionalOr = 4,
    ExclusiveOr = 5,
    GreaterThan = 6,
    GreaterThanOrEqual = 7,
    Equality = 8,
    InEquality = 9,
    LessThan = 10,
    LessThanOrEqual = 11,
    Add = 12,
    Subtract = 13,
    Multiply = 14,
    Divide = 15,
    Modulus = 16,
    ShiftLeft = 17,
    ShiftRight = 18,
    UnsignedShiftRight = 19,
    NullCoalescing = 20,
}

/// A sub-pattern of a recursive pattern: `Member: pattern`, as a `NamedArgumentExpression`
/// with the `MemberResolveResult` of the member without target.
#[derive(Clone, Debug)]
pub struct SubPattern {
    pub member: PatternMember,
    pub pattern: AstId,
}

/// The member a sub-pattern tests.
#[derive(Clone, Debug)]
pub enum PatternMember {
    Field(crate::il::ast::FieldOperand),
    /// The property of this getter.
    Property(crate::il::ast::MethodOperand),
}

/// `UnaryOperatorType` of a pattern: `not`, `<`, `<=`, `>`, `>=`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum PatternOperator {
    Not,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}

/// `ResolveResult` annotations.
#[derive(Clone, Debug)]
pub enum Rr {
    /// `ConstantResolveResult`.
    Constant { ty: TypeId, value: ConstValue },
    /// `ILVariableResolveResult`.
    Variable {
        var: crate::il::ast::VarId,
        ty: TypeId,
    },
    /// `ThisResolveResult` of the `this` parameter.
    This {
        var: crate::il::ast::VarId,
        ty: TypeId,
        causes_non_virtual_invocation: bool,
    },
    /// `TypeResolveResult`.
    Type(TypeId),
    /// `MemberResolveResult` of a field with the target's resolve result (none for the
    /// `field` keyword).
    Field {
        target: Option<Box<Rr>>,
        field: crate::il::ast::FieldOperand,
    },
    /// `InterpolatedStringResolveResult` of a `string.Format` call written as an interpolated
    /// string, whose arguments the host takes from the interpolations.
    FormatString { ty: TypeId, format: Vec<u16> },
    /// `ToMethodGroup`: the `MethodGroupResolveResult` of a local function, named `name`, whose
    /// only method is the reduced method `method` (`Ast::method_groups[index]` for the engine).
    LocalFunctionGroup {
        index: usize,
        name: String,
        method: Box<crate::il::ast::MethodOperand>,
    },
    /// `ThrowResolveResult`, of no type.
    Throw,
    /// `ConversionResolveResult` of `Conversion.TryCast` to `ty`, which the host computes from
    /// the `as` node's operand.
    TryCast { ty: TypeId },
    /// `MemberResolveResult` of a tuple element's field with the element type of the target's
    /// tuple type (`returnTypeOverride`).
    TupleElement {
        target: Box<Rr>,
        field: crate::il::ast::FieldOperand,
        ty: TypeId,
    },
    /// `MemberResolveResult` of an event of the main module (its Event row and type) with
    /// the target's resolve result.
    Event {
        target: Box<Rr>,
        event_row: u32,
        ty: TypeId,
    },
    /// `ByReferenceResolveResult` with `ReferenceKind.Ref` of the element's resolve result.
    ByReference(Box<Rr>),
    /// `ByReferenceResolveResult` with `ReferenceKind.Out` of the element's resolve result.
    ByReferenceOut(Box<Rr>),
    /// `ByReferenceResolveResult` of an `in` argument, with the element's resolve result, of
    /// the parameter's reference kind (`In`, or `RefReadOnly` where `true`).
    ByReferenceIn(Box<Rr>, bool),
    /// `OperatorResolveResult` of the node's operator (an assignment, `!`, a binary or the conditional
    /// operator), of
    /// the given
    /// type, whose operands are the resolve results of the node's children.
    Operator { ty: TypeId },
    /// `CSharpInvocationResolveResult` of a call of `method` (of the given return type) on the
    /// target's resolve result, whose arguments are the resolve results of the invocation's
    /// arguments; `extension` for an extension method call written on its first argument
    /// (`isExtensionMethodInvocation`).
    Invocation {
        target: Box<Rr>,
        method: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
        extension: bool,
        /// `isDelegateInvocation`: `Invoke` of a delegate, written as `target(arguments)`.
        delegate: bool,
        /// Whether the name resolves to an override of `method`, whose invocation the host
        /// finds again with the overload resolution of the written call.
        found_by_lookup: bool,
        /// `isExpandedForm`: the `params` array's elements are the last arguments.
        expanded: bool,
        /// The type arguments of the method the call resolves to where they are not `method`'s
        /// (the found method), which the host specializes the method's definition with.
        method_args: Option<Vec<TypeId>>,
    },
    /// A plain `ResolveResult` of the given type.
    Plain(TypeId),
    /// The `MemberResolveResult` of `System.Array.Length` (`LongLength` when `long`) with the
    /// target's resolve result, which the host looks the property up for; `ty` is `int` or
    /// `long`.
    ArrayLength {
        target: Box<Rr>,
        long: bool,
        ty: TypeId,
    },
    /// `TypeOfResolveResult` of the type `ty` for the referenced type.
    TypeOf { ty: TypeId, referenced: TypeId },
    /// `ConversionResolveResult` of a cast for an implicit conversion operator: the implicit
    /// conversion from the operand's type to `ty`, which the host computes.
    ImplicitConversion { ty: TypeId },
    /// `resolver.ResolveCast(ty, operand)` of a cast, over the resolve result of the cast's
    /// operand, which the host computes.
    Conversion { ty: TypeId },
    /// `ConversionResolveResult` of a cast of a lambda to the delegate type `ty`, with the
    /// lambda's resolve result and `LambdaConversion`.
    LambdaConversion { ty: TypeId },
    /// `DecompiledLambdaResolveResult`, at this index of `Ast::lambdas`, which the host builds
    /// for the lambda's node.
    Lambda(usize),
    /// `InitializedObjectResolveResult` of the object an initializer initializes.
    InitializedObject(TypeId),
    /// `ArrayCreateResolveResult` of the array type `ty`, over the resolve results of the
    /// dimensions and the elements, which the host collects from the node.
    ArrayCreate { ty: TypeId },
    /// `TypeIsResolveResult` of a test for the type `ty`, over the tested expression's
    /// resolve result.
    TypeIs { ty: TypeId },
    /// `DefaultLiteralResolveResult` of a `default` literal shortened from `default(T)`.
    DefaultLiteral { shortened_from: TypeId },
    /// `resolver.ResolveBinaryOperator` of a binary operator over its operands' resolve
    /// results, of the type `ty`, which the host computes (with the operands' conversions).
    ResolvedOperator { ty: TypeId },
    /// `MethodGroupResolveResult` of a method reference (`Ast::method_groups[index]` for the
    /// engine), which the host looks up again from the target (or the simple name) with the
    /// type arguments and narrows to `method` (`WithChosenMethod`).
    MethodGroup {
        index: usize,
        target: Option<Box<Rr>>,
        method: Box<crate::il::ast::MethodOperand>,
        type_arguments: Vec<TypeId>,
    },
    /// `CSharpInvocationResolveResult` without target of the constructor `method` of the type
    /// `ty`, whose arguments are the resolve results of the object creation's arguments.
    ObjectCreation {
        method: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
        /// `isExpandedForm`: the `params` array's elements are the last arguments.
        expanded: bool,
    },
    /// `MemberResolveResult` of the property (of the given type) `accessor` belongs to, with
    /// the target's resolve result.
    /// `found_by_lookup` when the name binds to another member the call may write (the
    /// declaration an override overrides), which the host looks up again for the result.
    Property {
        target: Box<Rr>,
        accessor: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
        found_by_lookup: bool,
    },
}

/// `InterpolatedStringContent`: text (with its braces doubled) or an interpolation.
#[derive(Clone, Debug)]
pub enum InterpolatedContent {
    Text(Vec<u16>),
    Interpolation {
        expression: AstId,
        alignment: i32,
        suffix: Option<Vec<u16>>,
    },
}

/// The annotations of a node.
#[derive(Clone, Debug, Default)]
pub struct Annotations {
    /// `WithILInstruction`, in the order they were added (an instruction can repeat).
    pub il: Vec<NodeId>,
    pub rr: Option<Rr>,
    /// Whether the IL instructions were annotated before the resolve result (the managed
    /// builders add them in either order).
    pub il_before_rr: bool,
    /// `ImplicitReturnAnnotation`: the final `leave` a block falls out of.
    pub implicit_return: Option<NodeId>,
    /// `ImplicitConversionAnnotation`: the type and resolve result (`ImplicitConversion` or
    /// `Conversion`) of the user-defined cast `ConvertTo` dropped around the expression.
    pub implicit_conversion: Option<(TypeId, Rr)>,
}

#[derive(Clone, Debug, Default)]
pub struct Ast {
    pub nodes: Vec<(AstNode, Annotations)>,
    /// The resolve results of the lambdas, which overload resolution can update.
    pub lambdas: Vec<std::rc::Rc<crate::semantics::decompiled_lambda::DecompiledLambda>>,
    /// The method groups of method references, as overload resolution sees them.
    pub method_groups: Vec<std::rc::Rc<crate::semantics::MethodGroup>>,
}

impl Ast {
    pub fn add(&mut self, node: AstNode) -> AstId {
        self.nodes.push((node, Annotations::default()));
        AstId(self.nodes.len() as u32 - 1)
    }

    pub fn node(&self, id: AstId) -> &AstNode {
        &self.nodes[id.0 as usize].0
    }

    pub fn node_mut(&mut self, id: AstId) -> &mut AstNode {
        &mut self.nodes[id.0 as usize].0
    }

    pub fn annotations(&self, id: AstId) -> &Annotations {
        &self.nodes[id.0 as usize].1
    }

    pub fn annotations_mut(&mut self, id: AstId) -> &mut Annotations {
        &mut self.nodes[id.0 as usize].1
    }

    /// `WithILInstruction(inst)`.
    pub fn with_il(&mut self, id: AstId, inst: NodeId) -> AstId {
        let a = self.annotations_mut(id);
        if a.il.is_empty() && a.rr.is_none() {
            a.il_before_rr = true;
        }
        a.il.push(inst);
        id
    }

    /// `WithRR(rr)`.
    pub fn with_rr(&mut self, id: AstId, rr: Rr) -> AstId {
        self.annotations_mut(id).rr = Some(rr);
        id
    }
}
