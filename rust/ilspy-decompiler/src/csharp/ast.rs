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
    /// `ConditionalExpression`: `condition ? true_expression : false_expression`.
    Conditional {
        condition: AstId,
        true_expression: AstId,
        false_expression: AstId,
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
    /// `ThrowStatement`, a rethrow without an expression.
    Throw { expression: Option<AstId> },
    /// `ObjectCreateExpression` of `ExpressionBuilder.ConvertType(ty)`, which the host builds.
    ObjectCreate { ty: TypeId, arguments: Vec<AstId> },
    /// `BinaryOperatorExpression`.
    BinaryOperator {
        op: BinaryOp,
        left: AstId,
        right: AstId,
    },
}

/// `BinaryOperatorType`, with its values.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum BinaryOp {
    ConditionalAnd = 3,
    ConditionalOr = 4,
    Equality = 8,
    InEquality = 9,
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
    /// `MemberResolveResult` of an event of the main module (its Event row and type) with
    /// the target's resolve result.
    Event {
        target: Box<Rr>,
        event_row: u32,
        ty: TypeId,
    },
    /// `ByReferenceResolveResult` with `ReferenceKind.Ref` of the element's resolve result.
    ByReference(Box<Rr>),
    /// `OperatorResolveResult` of the node's operator (an assignment, `!`, a binary or the conditional
    /// operator), of
    /// the given
    /// type, whose operands are the resolve results of the node's children.
    Operator { ty: TypeId },
    /// `CSharpInvocationResolveResult` of a call of `method` (of the given return type) on the
    /// target's resolve result, whose arguments are the resolve results of the invocation's
    /// arguments.
    Invocation {
        target: Box<Rr>,
        method: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
    },
    /// A plain `ResolveResult` of the given type.
    Plain(TypeId),
    /// `CSharpInvocationResolveResult` without target of the constructor `method` of the type
    /// `ty`, whose arguments are the resolve results of the object creation's arguments.
    ObjectCreation {
        method: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
    },
    /// `MemberResolveResult` of the property (of the given type) `accessor` belongs to, with
    /// the target's resolve result.
    Property {
        target: Box<Rr>,
        accessor: Box<crate::il::ast::MethodOperand>,
        ty: TypeId,
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
}

#[derive(Clone, Debug, Default)]
pub struct Ast {
    pub nodes: Vec<(AstNode, Annotations)>,
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
