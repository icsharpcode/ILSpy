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
    pub const BINARY_OPERATOR: u8 = 18;
    pub const THROW: u8 = 19;
    pub const OBJECT_CREATE: u8 = 20;
    pub const CONDITIONAL: u8 = 21;
    pub const GENERIC_IDENTIFIER: u8 = 22;
    pub const GENERIC_MEMBER_REFERENCE: u8 = 23;
    pub const CAST: u8 = 24;
    pub const TYPE_OF: u8 = 25;
    pub const OUT_DIRECTION: u8 = 26;
    pub const TRY_CATCH: u8 = 27;
    pub const CATCH_CLAUSE: u8 = 28;
    pub const LOCK: u8 = 29;
    pub const WHILE: u8 = 30;
    pub const BREAK: u8 = 31;
    pub const CONTINUE: u8 = 32;
    pub const DO_WHILE: u8 = 33;
    pub const FOR: u8 = 34;
    pub const INDEXER: u8 = 35;
    pub const USING: u8 = 36;
    pub const LAMBDA: u8 = 37;
    pub const INTERPOLATED_STRING: u8 = 38;
    pub const NAMED: u8 = 39;
    pub const ARRAY_CREATE: u8 = 40;
    pub const ARRAY_INITIALIZER: u8 = 41;
    pub const YIELD_RETURN: u8 = 42;
    pub const YIELD_BREAK: u8 = 43;
    pub const NULL_CONDITIONAL: u8 = 44;
    pub const IS_PATTERN: u8 = 45;
    pub const DECLARATION_PATTERN: u8 = 46;
    pub const RECURSIVE_PATTERN: u8 = 47;
    pub const IS_TYPE: u8 = 48;
    pub const DEFAULT_VALUE: u8 = 49;
    pub const FOREACH: u8 = 50;
    pub const DELEGATE_CREATE: u8 = 51;
    pub const AWAIT: u8 = 52;
    pub const SWITCH: u8 = 53;
    pub const SWITCH_SECTION: u8 = 54;
    pub const COMPOUND_ASSIGNMENT: u8 = 55;
    pub const POST_INCREMENT: u8 = 56;
    pub const AS: u8 = 57;
    pub const THROW_EXPRESSION: u8 = 58;
    pub const BIT_NOT: u8 = 59;
    pub const TUPLE: u8 = 60;
    pub const HEX_CONSTANT: u8 = 61;
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
/// `OperatorResolveResult` of an assignment, `!`, a binary or the conditional operator (type; the operands are the children's); 8 the
/// `MemberResolveResult` of an event (the target's resolve result, the event as a member
/// token); 9 the `CSharpInvocationResolveResult` of a call (the target's resolve result and
/// the method as a call operand, then whether it is an extension method invocation; the
/// arguments are the invocation's); 10 the
/// `MemberResolveResult` of a property (the target's resolve result and an accessor of the
/// property as a call operand, then whether the host looks the member up again); 11 the `CSharpInvocationResolveResult` of a constructor
/// call (the constructor as a call operand; no target, the arguments are the object
/// creation's); 12 a plain `ResolveResult` (type); 13 the `ResolveCast` of a cast, which the host
/// computes from the cast's type and operand; 14 `TypeOfResolveResult` (type, referenced type); 15
/// `ByReferenceResolveResult` with `ReferenceKind.Out` (the element's resolve result); 16 the
/// `MemberResolveResult` of `Array.Length` or `LongLength` (the target's resolve result, whether
/// it is `LongLength`); 17 the
/// `ConversionResolveResult` of an implicit conversion operator, which the host computes from the
/// cast's type and operand. A field's target is
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
        Some(Rr::TupleElement { target, field, ty }) => {
            w.out.u8(24);
            self::rr(w, Some(target))?;
            w.member(field.token, field.context);
            w.ty(*ty);
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
        Some(Rr::ArrayLength { target, long, .. }) => {
            w.out.u8(16);
            self::rr(w, Some(target))?;
            w.out.bool(*long);
        }
        Some(Rr::ByReferenceOut(element)) => {
            w.out.u8(15);
            self::rr(w, Some(element))?;
        }
        Some(Rr::Operator { ty }) => {
            w.out.u8(7);
            w.ty(*ty);
        }
        Some(Rr::Invocation {
            target,
            method,
            extension,
            delegate,
            found_by_lookup,
            ..
        }) => {
            w.out.u8(9);
            self::rr(w, Some(target))?;
            w.call_method(method)?;
            w.out.bool(*extension);
            w.out.bool(*delegate);
            w.out.bool(*found_by_lookup);
        }
        Some(Rr::Property {
            target,
            accessor,
            found_by_lookup,
            ..
        }) => {
            w.out.u8(10);
            self::rr(w, Some(target))?;
            w.call_method(accessor)?;
            w.out.bool(*found_by_lookup);
        }
        Some(Rr::Conversion { .. }) => w.out.u8(13),
        Some(Rr::ImplicitConversion { .. }) => w.out.u8(17),
        Some(Rr::LambdaConversion { .. }) => w.out.u8(18),
        // The host builds the resolve result from the node.
        Some(Rr::ArrayCreate { .. }) => w.out.u8(0),
        Some(Rr::TypeIs { .. }) => w.out.u8(0),
        Some(Rr::ResolvedOperator { .. }) => w.out.u8(22),
        Some(Rr::MethodGroup {
            target,
            method,
            type_arguments,
            ..
        }) => {
            w.out.u8(23);
            // The host takes the target's resolve result from the built target expression.
            w.out.bool(target.is_some());
            w.out.len(type_arguments.len());
            for &t in type_arguments {
                w.ty(t);
            }
            w.call_method(method)?;
        }
        Some(Rr::DefaultLiteral { shortened_from }) => {
            w.out.u8(21);
            w.ty(*shortened_from);
        }
        Some(Rr::InitializedObject(ty)) => {
            w.out.u8(19);
            w.ty(*ty);
        }
        Some(Rr::Throw) => w.out.u8(27),
        Some(Rr::TryCast { ty }) => {
            w.out.u8(25);
            w.ty(*ty);
        }
        Some(Rr::FormatString { ty, format }) => {
            w.out.u8(26);
            w.ty(*ty);
            utf16(w, format);
        }
        // The host builds the lambda's resolve result from the node.
        Some(Rr::Lambda(_)) => w.out.u8(0),
        Some(Rr::TypeOf { ty, referenced }) => {
            w.out.u8(14);
            w.ty(*ty);
            w.ty(*referenced);
        }
        Some(Rr::Plain(ty)) => {
            w.out.u8(12);
            w.ty(*ty);
        }
        Some(Rr::ObjectCreation { method, .. }) => {
            w.out.u8(11);
            w.call_method(method)?;
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
        AstNode::BinaryOperator { .. } => kind::BINARY_OPERATOR,
        AstNode::Throw { .. } => kind::THROW,
        AstNode::ObjectCreate { .. } => kind::OBJECT_CREATE,
        AstNode::Conditional { .. } => kind::CONDITIONAL,
        AstNode::GenericIdentifier { .. } => kind::GENERIC_IDENTIFIER,
        AstNode::GenericMemberReference { .. } => kind::GENERIC_MEMBER_REFERENCE,
        AstNode::Cast { .. } => kind::CAST,
        AstNode::TypeOf { .. } => kind::TYPE_OF,
        AstNode::OutDirection { .. } => kind::OUT_DIRECTION,
        AstNode::TryCatch { .. } => kind::TRY_CATCH,
        AstNode::CatchClause { .. } => kind::CATCH_CLAUSE,
        AstNode::Lock { .. } => kind::LOCK,
        AstNode::While { .. } => kind::WHILE,
        AstNode::Break => kind::BREAK,
        AstNode::Continue => kind::CONTINUE,
        AstNode::DoWhile { .. } => kind::DO_WHILE,
        AstNode::For { .. } => kind::FOR,
        AstNode::Indexer { .. } => kind::INDEXER,
        AstNode::Using { .. } => kind::USING,
        AstNode::Lambda { .. } => kind::LAMBDA,
        AstNode::InterpolatedString { .. } => kind::INTERPOLATED_STRING,
        AstNode::Named { .. } => kind::NAMED,
        AstNode::ArrayCreate { .. } => kind::ARRAY_CREATE,
        AstNode::ArrayInitializer { .. } => kind::ARRAY_INITIALIZER,
        AstNode::YieldReturn { .. } => kind::YIELD_RETURN,
        AstNode::YieldBreak => kind::YIELD_BREAK,
        AstNode::NullConditional { .. } => kind::NULL_CONDITIONAL,
        AstNode::IsPattern { .. } => kind::IS_PATTERN,
        AstNode::DeclarationPattern { .. } => kind::DECLARATION_PATTERN,
        AstNode::RecursivePattern { .. } => kind::RECURSIVE_PATTERN,
        AstNode::IsType { .. } => kind::IS_TYPE,
        AstNode::DefaultValue { .. } => kind::DEFAULT_VALUE,
        AstNode::Foreach { .. } => kind::FOREACH,
        AstNode::DelegateCreate { .. } => kind::DELEGATE_CREATE,
        AstNode::Await { .. } => kind::AWAIT,
        AstNode::Switch { .. } => kind::SWITCH,
        AstNode::SwitchSection { .. } => kind::SWITCH_SECTION,
        AstNode::CompoundAssignment { .. } => kind::COMPOUND_ASSIGNMENT,
        AstNode::PostIncrement { .. } => kind::POST_INCREMENT,
        AstNode::As { .. } => kind::AS,
        AstNode::ThrowExpression { .. } => kind::THROW_EXPRESSION,
        AstNode::BitNot { .. } => kind::BIT_NOT,
        AstNode::Tuple { .. } => kind::TUPLE,
        AstNode::HexConstant { .. } => kind::HEX_CONSTANT,
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
    match &a.implicit_conversion {
        None => w.out.bool(false),
        Some((ty, rr)) => {
            w.out.bool(true);
            w.out
                .u8(u8::from(!matches!(rr, Rr::ImplicitConversion { .. })));
            w.ty(*ty);
        }
    }
    match n {
        AstNode::Block { statements } => {
            w.out.len(statements.len());
            for s in statements {
                node(w, ast, s)?;
            }
        }
        AstNode::CompoundAssignment {
            op,
            left,
            right,
            overflow,
        } => {
            w.out.u8(op);
            w.out.u8(overflow as u8);
            node(w, ast, left)?;
            node(w, ast, right)?;
        }
        AstNode::Tuple {
            elements,
            names,
            value_tuple,
            names_in_rr,
        } => {
            w.ty(value_tuple);
            w.out.bool(names_in_rr);
            w.out.len(elements.len());
            for (e, n) in elements.into_iter().zip(names) {
                w.out.bool(n.is_some());
                if let Some(n) = n {
                    w.string(&n);
                }
                node(w, ast, e)?;
            }
        }
        AstNode::As { expression, ty } => {
            w.ty(ty);
            node(w, ast, expression)?;
        }
        AstNode::PostIncrement {
            increment,
            expression,
            overflow,
        } => {
            w.out.bool(increment);
            w.out.u8(overflow as u8);
            node(w, ast, expression)?;
        }
        AstNode::Switch {
            expression,
            sections,
        } => {
            node(w, ast, expression)?;
            w.out.len(sections.len());
            for s in sections {
                node(w, ast, s)?;
            }
        }
        AstNode::SwitchSection { labels, statements } => {
            w.out.len(labels.len());
            for l in labels {
                w.out.bool(l.is_some());
                if let Some(l) = l {
                    node(w, ast, l)?;
                }
            }
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
        AstNode::HexConstant {
            ty,
            value,
            display_as_hex,
        } => {
            w.ty(ty);
            constant(w, &value);
            w.out.bool(display_as_hex);
        }
        AstNode::StringLiteral { value } => utf16(w, &value),
        AstNode::Identifier { name } => w.string(&name),
        AstNode::MemberReference { target, name } => {
            node(w, ast, target)?;
            w.string(&name);
        }
        AstNode::TypeReference { ty } | AstNode::TypeOf { ty } => w.ty(ty),
        AstNode::RefDirection { expression }
        | AstNode::ExpressionStatement { expression }
        | AstNode::LogicNot { expression }
        | AstNode::ThrowExpression { expression }
        | AstNode::BitNot { expression }
        | AstNode::Await { expression } => {
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
        AstNode::TryCatch {
            try_block,
            catch_clauses,
            finally_block,
        } => {
            node(w, ast, try_block)?;
            w.out.len(catch_clauses.len());
            for c in catch_clauses {
                node(w, ast, c)?;
            }
            w.out.bool(finally_block.is_some());
            if let Some(f) = finally_block {
                node(w, ast, f)?;
            }
        }
        AstNode::CatchClause {
            variable_name,
            ty,
            condition,
            body,
        } => {
            w.out.bool(variable_name.is_some());
            if let Some(name) = &variable_name {
                w.string(name);
            }
            w.out.bool(ty.is_some());
            if let Some(t) = ty {
                w.ty(t);
            }
            w.out.bool(condition.is_some());
            if let Some(c) = condition {
                node(w, ast, c)?;
            }
            node(w, ast, body)?;
        }
        AstNode::Indexer { target, indices } => {
            node(w, ast, target)?;
            w.out.len(indices.len());
            for i in indices {
                node(w, ast, i)?;
            }
        }
        AstNode::DoWhile { body, condition } => {
            node(w, ast, body)?;
            node(w, ast, condition)?;
        }
        AstNode::For {
            condition,
            iterators,
            body,
        } => {
            node(w, ast, condition)?;
            w.out.len(iterators.len());
            for i in iterators {
                node(w, ast, i)?;
            }
            node(w, ast, body)?;
        }
        AstNode::While { condition, body } => {
            w.out.bool(condition.is_some());
            if let Some(c) = condition {
                node(w, ast, c)?;
            }
            node(w, ast, body)?;
        }
        AstNode::Lock { expression, body } => {
            node(w, ast, expression)?;
            node(w, ast, body)?;
        }
        AstNode::InterpolatedString { contents } => {
            w.out.len(contents.len());
            for c in contents {
                match c {
                    crate::csharp::ast::InterpolatedContent::Text(text) => {
                        w.out.u8(0);
                        utf16(w, &text);
                    }
                    crate::csharp::ast::InterpolatedContent::Interpolation {
                        expression,
                        alignment,
                        suffix,
                    } => {
                        w.out.u8(1);
                        w.out.ivar(i64::from(alignment));
                        w.out.bool(suffix.is_some());
                        if let Some(s) = suffix {
                            utf16(w, &s);
                        }
                        node(w, ast, expression)?;
                    }
                }
            }
        }
        AstNode::Lambda {
            body,
            expression_body,
            lambda,
        } => {
            let l = &ast.lambdas[lambda];
            w.ty(l.delegate_type);
            w.ty(l.inferred_return_type);
            w.out.bool(l.attempted_conversion_with_type_mismatch.get());
            w.out.bool(l.implicitly_typed);
            w.out.bool(l.anonymous_method);
            w.out.bool(expression_body);
            node(w, ast, body)?;
        }
        AstNode::Using {
            declaration,
            resource,
            body,
        } => {
            w.out.bool(declaration.is_some());
            if let Some(v) = declaration {
                let i = var_index(w, v)?;
                w.out.len(i);
            }
            node(w, ast, resource)?;
            node(w, ast, body)?;
        }
        AstNode::OutDirection {
            expression,
            implicitly_typed,
        } => {
            node(w, ast, expression)?;
            w.out.bool(implicitly_typed);
        }
        AstNode::Cast {
            ty,
            expression,
            unchecked,
        } => {
            w.ty(ty);
            node(w, ast, expression)?;
            w.out.bool(unchecked);
        }
        AstNode::GenericIdentifier {
            name,
            type_arguments,
        } => {
            w.string(&name);
            w.out.len(type_arguments.len());
            for t in type_arguments {
                w.ty(t);
            }
        }
        AstNode::GenericMemberReference {
            target,
            name,
            type_arguments,
        } => {
            node(w, ast, target)?;
            w.string(&name);
            w.out.len(type_arguments.len());
            for t in type_arguments {
                w.ty(t);
            }
        }
        AstNode::Conditional {
            condition,
            true_expression,
            false_expression,
            resolve,
        } => {
            w.out.bool(resolve);
            node(w, ast, condition)?;
            node(w, ast, true_expression)?;
            node(w, ast, false_expression)?;
        }
        AstNode::Throw { expression } => {
            w.out.bool(expression.is_some());
            if let Some(e) = expression {
                node(w, ast, e)?;
            }
        }
        AstNode::ObjectCreate {
            ty,
            arguments,
            initializer,
        } => {
            w.ty(ty);
            w.out.len(arguments.len());
            for a in arguments {
                node(w, ast, a)?;
            }
            w.out.bool(initializer.is_some());
            if let Some(elements) = initializer {
                w.out.len(elements.len());
                for e in elements {
                    node(w, ast, e)?;
                }
            }
        }
        AstNode::ArrayCreate {
            element_type,
            arguments,
            initializer,
            special_constants,
        } => {
            w.ty(element_type);
            w.out.bool(special_constants);
            w.out.len(arguments.len());
            for a in arguments {
                node(w, ast, a)?;
            }
            w.out.bool(initializer.is_some());
            if let Some(i) = initializer {
                node(w, ast, i)?;
            }
        }
        AstNode::YieldReturn { expression } => node(w, ast, expression)?,
        AstNode::DelegateCreate {
            ty,
            reference,
            method,
            is_virtual,
        } => {
            w.ty(ty);
            w.call_method(&method)?;
            w.out.bool(is_virtual);
            node(w, ast, reference)?;
        }
        AstNode::Foreach {
            variable,
            collection,
            body,
            get_enumerator,
            move_next,
            get_current,
        } => {
            let v = var_index(w, variable)?;
            w.out.len(v);
            for inst in [get_enumerator, move_next, get_current] {
                let i = il_index(w, inst)?;
                w.out.len(i);
            }
            node(w, ast, collection)?;
            node(w, ast, body)?;
        }
        AstNode::DefaultValue { ty } => {
            w.out.bool(ty.is_some());
            if let Some(t) = ty {
                w.ty(t);
            }
        }
        AstNode::IsType { expression, ty } => {
            w.ty(ty);
            node(w, ast, expression)?;
        }
        AstNode::IsPattern { left, pattern } => {
            node(w, ast, left)?;
            node(w, ast, pattern)?;
        }
        AstNode::DeclarationPattern {
            variable,
            var_keyword,
        } => {
            let i = var_index(w, variable)?;
            w.out.len(i);
            w.out.bool(var_keyword);
        }
        AstNode::RecursivePattern { designator } => {
            w.out.bool(designator.is_some());
            if let Some(v) = designator {
                let i = var_index(w, v)?;
                w.out.len(i);
            }
        }
        AstNode::NullConditional { expression, rewrap } => {
            w.out.bool(rewrap);
            node(w, ast, expression)?;
        }
        AstNode::ArrayInitializer { elements } => {
            w.out.len(elements.len());
            for e in elements {
                node(w, ast, e)?;
            }
        }
        AstNode::Named { name, expression } => {
            w.string(&name);
            node(w, ast, expression)?;
        }
        AstNode::BinaryOperator {
            op,
            left,
            right,
            overflow,
        } => {
            w.out.u8(op as u8);
            w.out.u8(overflow as u8);
            node(w, ast, left)?;
            node(w, ast, right)?;
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
        AstNode::Empty
        | AstNode::Null
        | AstNode::This
        | AstNode::Base
        | AstNode::Break
        | AstNode::Continue
        | AstNode::YieldBreak => {}
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
pub(super) fn constant(w: &mut FunctionWriter<'_, '_, '_, '_>, value: &ConstValue) {
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
