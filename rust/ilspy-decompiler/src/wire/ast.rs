//! The C# translation of a function body, after the function in the `read_module` format;
//! `IlAstMaterializer.BodyNode` reads it and `RustSeam2.cs` builds the syntax.
//!
//! A trailer byte follows every function: 0 when no translation was asked for, 1 and a
//! string when the body is not translated (the reason), 2 and the `BlockStatement` when it
//! is. A node is its kind, its annotations and then its payload and children in slot order:
//! - annotations: the IL instructions (`WithILInstruction`, as indices of the function's
//!   nodes in completion order), whether they were added before the resolve result, the
//!   resolve result (see `rr`) and the `ImplicitReturnAnnotation` (0 none, else the
//!   `leave`'s index + 1);
//! - a constant is a tag (`ConstValue` order) and its value.

use crate::csharp::Unsupported;
use crate::csharp::ast::{Ast, AstId, AstNode, Rr};
use crate::typesystem::members::ConstValue;

use super::ilast::FunctionWriter;

/// The translation `crate::csharp::convert_body` gives.
pub type Body = Result<(Ast, AstId), Unsupported>;

/// Node kinds.
mod kind {
    pub const BLOCK: u8 = 0;
    pub const RETURN: u8 = 1;
    pub const EMPTY: u8 = 2;
    pub const CONSTANT_VALUE: u8 = 3;
    pub const STRING_LITERAL: u8 = 4;
    pub const NULL: u8 = 5;
    pub const IDENTIFIER: u8 = 6;
    pub const THIS: u8 = 7;
    pub const BASE: u8 = 8;
    pub const MEMBER_REFERENCE: u8 = 9;
    pub const TYPE_REFERENCE: u8 = 10;
    pub const REF_DIRECTION: u8 = 11;
    pub const ASSIGNMENT: u8 = 12;
    pub const EXPRESSION_STATEMENT: u8 = 13;
    pub const INVOCATION: u8 = 14;
    pub const NAMED_ARGUMENT: u8 = 15;
    pub const IF_ELSE: u8 = 16;
    pub const LOGIC_NOT: u8 = 17;
}

pub(super) fn write_body(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    body: Option<&Body>,
) -> Result<(), String> {
    match body {
        None => w.out.u8(0),
        Some(Err(reason)) => {
            w.out.u8(1);
            w.string(reason);
        }
        Some(Ok((ast, root))) => {
            w.out.u8(2);
            node(w, ast, *root)?;
        }
    }
    Ok(())
}

fn il_index(
    w: &FunctionWriter<'_, '_, '_, '_>,
    inst: crate::il::ast::NodeId,
) -> Result<usize, String> {
    w.written
        .get(&inst)
        .copied()
        .ok_or_else(|| "a C# node annotated with an instruction outside the function".to_owned())
}

fn var_index(
    w: &FunctionWriter<'_, '_, '_, '_>,
    var: crate::il::ast::VarId,
) -> Result<usize, String> {
    w.vars
        .get(&var)
        .copied()
        .ok_or_else(|| "a C# node refers to a variable outside the function".to_owned())
}

/// A resolve result: 0 none; 1 `ConstantResolveResult` (type, constant); 2
/// `ILVariableResolveResult` (variable, type); 3 `ThisResolveResult` (variable, type, whether
/// it causes a non-virtual invocation); 4 `TypeResolveResult` (type); 5 the
/// `MemberResolveResult` of a field (the target's resolve result, the field as a member
/// token); 6 `ByReferenceResolveResult` (the element's resolve result); 7 the
/// `OperatorResolveResult` of an assignment or `!` (type; the operands are the children's); 8 the
/// `MemberResolveResult` of an event (the target's resolve result, the event as a member
/// token); 9 the `CSharpInvocationResolveResult` of a call (the target's resolve result and
/// the method as a call operand; the arguments are the invocation's); 10 the
/// `MemberResolveResult` of a property (the target's resolve result and an accessor of the
/// property as a call operand). A field's target is
/// absent (0) for the `field` keyword.
fn rr(w: &mut FunctionWriter<'_, '_, '_, '_>, rr: Option<&Rr>) -> Result<(), String> {
    match rr {
        None => w.out.u8(0),
        Some(Rr::Constant { ty, value }) => {
            w.out.u8(1);
            w.ty(*ty);
            constant(w, value);
        }
        Some(Rr::Variable { var, ty }) => {
            w.out.u8(2);
            let i = var_index(w, *var)?;
            w.out.len(i);
            w.ty(*ty);
        }
        Some(Rr::This {
            var,
            ty,
            causes_non_virtual_invocation,
        }) => {
            w.out.u8(3);
            let i = var_index(w, *var)?;
            w.out.len(i);
            w.ty(*ty);
            w.out.bool(*causes_non_virtual_invocation);
        }
        Some(Rr::Type(ty)) => {
            w.out.u8(4);
            w.ty(*ty);
        }
        Some(Rr::Field { target, field }) => {
            w.out.u8(5);
            self::rr(w, target.as_deref())?;
            w.member(field.token, field.context);
        }
        Some(Rr::Event {
            target, event_row, ..
        }) => {
            w.out.u8(8);
            self::rr(w, Some(target))?;
            w.member(0x1400_0000 | event_row, 0);
        }
        Some(Rr::ByReference(element)) => {
            w.out.u8(6);
            self::rr(w, Some(element))?;
        }
        Some(Rr::Operator { ty }) => {
            w.out.u8(7);
            w.ty(*ty);
        }
        Some(Rr::Invocation { target, method, .. }) => {
            w.out.u8(9);
            self::rr(w, Some(target))?;
            w.call_method(method)?;
        }
        Some(Rr::Property {
            target, accessor, ..
        }) => {
            w.out.u8(10);
            self::rr(w, Some(target))?;
            w.call_method(accessor)?;
        }
    }
    Ok(())
}

fn node(w: &mut FunctionWriter<'_, '_, '_, '_>, ast: &Ast, id: AstId) -> Result<(), String> {
    let n = ast.node(id).clone();
    let a = ast.annotations(id).clone();
    w.out.u8(match n {
        AstNode::Block { .. } => kind::BLOCK,
        AstNode::Return { .. } => kind::RETURN,
        AstNode::Empty => kind::EMPTY,
        AstNode::ConstantValue { .. } => kind::CONSTANT_VALUE,
        AstNode::StringLiteral { .. } => kind::STRING_LITERAL,
        AstNode::Null => kind::NULL,
        AstNode::Identifier { .. } => kind::IDENTIFIER,
        AstNode::This => kind::THIS,
        AstNode::Base => kind::BASE,
        AstNode::MemberReference { .. } => kind::MEMBER_REFERENCE,
        AstNode::TypeReference { .. } => kind::TYPE_REFERENCE,
        AstNode::RefDirection { .. } => kind::REF_DIRECTION,
        AstNode::Assignment { .. } => kind::ASSIGNMENT,
        AstNode::ExpressionStatement { .. } => kind::EXPRESSION_STATEMENT,
        AstNode::Invocation { .. } => kind::INVOCATION,
        AstNode::NamedArgument { .. } => kind::NAMED_ARGUMENT,
        AstNode::IfElse { .. } => kind::IF_ELSE,
        AstNode::LogicNot { .. } => kind::LOGIC_NOT,
    });
    w.out.len(a.il.len());
    for &inst in &a.il {
        let i = il_index(w, inst)?;
        w.out.len(i);
    }
    w.out.bool(a.il_before_rr);
    rr(w, a.rr.as_ref())?;
    match a.implicit_return {
        None => w.out.u8(0),
        Some(leave) => {
            let i = il_index(w, leave)?;
            w.out.len(i + 1);
        }
    }
    match n {
        AstNode::Block { statements } => {
            w.out.len(statements.len());
            for s in statements {
                node(w, ast, s)?;
            }
        }
        AstNode::Return { expression } => {
            w.out.bool(expression.is_some());
            if let Some(e) = expression {
                node(w, ast, e)?;
            }
        }
        AstNode::ConstantValue {
            ty,
            value,
            allow_implicit_conversion,
        } => {
            w.ty(ty);
            constant(w, &value);
            w.out.bool(allow_implicit_conversion);
        }
        AstNode::StringLiteral { value } => utf16(w, &value),
        AstNode::Identifier { name } => w.string(&name),
        AstNode::MemberReference { target, name } => {
            node(w, ast, target)?;
            w.string(&name);
        }
        AstNode::TypeReference { ty } => w.ty(ty),
        AstNode::RefDirection { expression }
        | AstNode::ExpressionStatement { expression }
        | AstNode::LogicNot { expression } => {
            node(w, ast, expression)?;
        }
        AstNode::Assignment { left, right } => {
            node(w, ast, left)?;
            node(w, ast, right)?;
        }
        AstNode::Invocation { target, arguments } => {
            node(w, ast, target)?;
            w.out.len(arguments.len());
            for a in arguments {
                node(w, ast, a)?;
            }
        }
        AstNode::NamedArgument { name, expression } => {
            w.string(&name);
            node(w, ast, expression)?;
        }
        AstNode::IfElse {
            condition,
            true_statement,
            false_statement,
        } => {
            node(w, ast, condition)?;
            node(w, ast, true_statement)?;
            w.out.bool(false_statement.is_some());
            if let Some(s) = false_statement {
                node(w, ast, s)?;
            }
        }
        AstNode::Empty | AstNode::Null | AstNode::This | AstNode::Base => {}
    }
    Ok(())
}

fn utf16(w: &mut FunctionWriter<'_, '_, '_, '_>, value: &[u16]) {
    w.out.len(value.len());
    for u in value {
        w.out.raw(&u.to_le_bytes());
    }
}

/// A constant: the `ConstValue` variant's position, then its value (integers as signed or
/// unsigned variable-length numbers, floating point and decimal as their bits).
fn constant(w: &mut FunctionWriter<'_, '_, '_, '_>, value: &ConstValue) {
    match value {
        ConstValue::Null => w.out.u8(0),
        ConstValue::Boolean(b) => {
            w.out.u8(1);
            w.out.bool(*b);
        }
        ConstValue::Char(c) => {
            w.out.u8(2);
            w.out.uvar(u64::from(*c));
        }
        ConstValue::SByte(v) => {
            w.out.u8(3);
            w.out.ivar(i64::from(*v));
        }
        ConstValue::Byte(v) => {
            w.out.u8(4);
            w.out.uvar(u64::from(*v));
        }
        ConstValue::Int16(v) => {
            w.out.u8(5);
            w.out.ivar(i64::from(*v));
        }
        ConstValue::UInt16(v) => {
            w.out.u8(6);
            w.out.uvar(u64::from(*v));
        }
        ConstValue::Int32(v) => {
            w.out.u8(7);
            w.out.ivar(i64::from(*v));
        }
        ConstValue::UInt32(v) => {
            w.out.u8(8);
            w.out.uvar(u64::from(*v));
        }
        ConstValue::Int64(v) => {
            w.out.u8(9);
            w.out.ivar(*v);
        }
        ConstValue::UInt64(v) => {
            w.out.u8(10);
            w.out.uvar(*v);
        }
        ConstValue::Single(bits) => {
            w.out.u8(11);
            w.out.raw(&bits.to_le_bytes());
        }
        ConstValue::Double(bits) => {
            w.out.u8(12);
            w.out.raw(&bits.to_le_bytes());
        }
        ConstValue::String(s) => {
            w.out.u8(13);
            utf16(w, s);
        }
        ConstValue::Decimal(parts) => {
            w.out.u8(14);
            for p in parts {
                w.out.raw(&p.to_le_bytes());
            }
        }
    }
}
