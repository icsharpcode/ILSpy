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
}

/// The annotations of a node.
#[derive(Clone, Debug, Default)]
pub struct Annotations {
    /// `WithILInstruction`, in the order they were added (an instruction can repeat).
    pub il: Vec<NodeId>,
    pub rr: Option<Rr>,
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
        self.annotations_mut(id).il.push(inst);
        id
    }

    /// `WithRR(rr)`.
    pub fn with_rr(&mut self, id: AstId, rr: Rr) -> AstId {
        self.annotations_mut(id).rr = Some(rr);
        id
    }
}
