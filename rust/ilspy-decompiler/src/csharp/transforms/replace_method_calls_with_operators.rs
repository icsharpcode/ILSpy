//! `ReplaceMethodCallsWithOperators`: replaces calls of operator methods, `string.Concat`,
//! `Type.GetTypeFromHandle`, `Activator.CreateInstance<T>` and `RuntimeHelpers.GetSubArray` by
//! the operators and expressions they stand for, and the `methodof` pattern by its ldtoken.
//!
//! The managed transform is a post-order visitor: an invocation or a cast is processed after its
//! children. The facts about methods, types and settings come from the host (`syntax_oracle`);
//! type syntax for new casts and object creations is built by the host and moved into the tree.

use std::collections::HashMap;

use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::{self as q, MethodInfo, ResolveResultFacts, SyntaxOracle};
use crate::csharp::syntax_schema::{
    array_create_expression, array_initializer_expression, binary_operator_expression,
    cast_expression, composed_type, direction_expression, identifier, indexer_expression,
    invocation_expression, kind, member_reference_expression, object_create_expression,
    primitive_expression, type_of_expression, type_reference_expression, unary_operator_expression,
    undocumented_expression,
};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeSystem;

/// A host object of a subtree the host built for a query (`QUERY_CONVERT_TYPE`); the payload is
/// the table number and the handle in that table (LEB128 each).
pub const ANNOTATION_ANSWERED: u8 = 48;
/// `AddCheckedBlocks.CheckedAnnotation`, no payload.
pub const ANNOTATION_CHECKED: u8 = 49;
/// `AddCheckedBlocks.UncheckedAnnotation`, no payload.
pub const ANNOTATION_UNCHECKED: u8 = 50;
/// The literal value `1m` (a boxed decimal), no payload.
pub const ANNOTATION_DECIMAL_ONE: u8 = 51;

/// `BinaryOperatorType` values.
mod bop {
    pub const BITWISE_AND: i64 = 1;
    pub const BITWISE_OR: i64 = 2;
    pub const EXCLUSIVE_OR: i64 = 5;
    pub const GREATER_THAN: i64 = 6;
    pub const GREATER_THAN_OR_EQUAL: i64 = 7;
    pub const EQUALITY: i64 = 8;
    pub const IN_EQUALITY: i64 = 9;
    pub const LESS_THAN: i64 = 10;
    pub const LESS_THAN_OR_EQUAL: i64 = 11;
    pub const ADD: i64 = 12;
    pub const SUBTRACT: i64 = 13;
    pub const MULTIPLY: i64 = 14;
    pub const DIVIDE: i64 = 15;
    pub const MODULUS: i64 = 16;
    pub const SHIFT_LEFT: i64 = 17;
    pub const SHIFT_RIGHT: i64 = 18;
    pub const UNSIGNED_SHIFT_RIGHT: i64 = 19;
}

/// `UnaryOperatorType` values.
mod uop {
    pub const NOT: i64 = 1;
    pub const BIT_NOT: i64 = 2;
    pub const MINUS: i64 = 3;
    pub const PLUS: i64 = 4;
    pub const INCREMENT: i64 = 5;
    pub const DECREMENT: i64 = 6;
    pub const NULL_CONDITIONAL: i64 = 12;
    pub const NULL_CONDITIONAL_REWRAP: i64 = 13;
}

const FIELD_DIRECTION_IN: i64 = 3;
const UNDOCUMENTED_REF_TYPE: i64 = 3;
const IL_FUNCTION_KIND_EXPRESSION_TREE: u8 = 2;

/// Runs the transform on the whole tree.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let mut cx = Cx {
        tree,
        oracle,
        resolve_results: HashMap::new(),
        settings: HashMap::new(),
    };
    // Post-order; the children of a node are those it has when its visit starts, as the
    // managed visitor's enumerator tolerates replacing the current child.
    let mut stack = vec![(root, cx.tree.all_children(root), 0usize)];
    while let Some((node, children, next)) = stack.last_mut() {
        if let Some(&c) = children.get(*next) {
            *next += 1;
            let grandchildren = cx.tree.all_children(c);
            stack.push((c, grandchildren, 0));
            continue;
        }
        let node = *node;
        stack.pop();
        match cx.tree.kind(node) {
            kind::INVOCATION_EXPRESSION => cx.process_invocation(node)?,
            kind::CAST_EXPRESSION => cx.visit_cast(node)?,
            _ => {}
        }
    }
    Ok(())
}

struct Cx<'a> {
    tree: &'a mut SyntaxTree,
    oracle: &'a dyn SyntaxOracle,
    /// The resolve result facts of each handle asked, `None` for an object that is none.
    resolve_results: HashMap<u32, Option<ResolveResultFacts>>,
    settings: HashMap<&'static str, bool>,
}

impl Cx<'_> {
    fn setting(&mut self, name: &'static str) -> Result<bool, String> {
        if let Some(&v) = self.settings.get(name) {
            return Ok(v);
        }
        let v = q::setting(self.oracle, name)
            .ok_or_else(|| format!("the host has no transform context for the setting {name}"))?;
        self.settings.insert(name, v);
        Ok(v)
    }

    fn facts_of_handle(&mut self, h: u32) -> Option<ResolveResultFacts> {
        let oracle = self.oracle;
        *self
            .resolve_results
            .entry(h)
            .or_insert_with(|| q::resolve_result_facts(oracle, h))
    }

    /// The handle of `Annotation<ResolveResult>()`.
    fn resolve_result(&mut self, node: SyntaxId) -> Option<u32> {
        let annotations = self.tree.node(node).annotations.clone();
        annotations
            .into_iter()
            .find(|&h| self.facts_of_handle(h).is_some())
    }

    /// The facts of `GetResolveResult()`: those of `ErrorResolveResult.UnknownError` (an unknown
    /// type) for a node without resolve result.
    fn facts(&mut self, node: SyntaxId) -> ResolveResultFacts {
        match self.resolve_result(node) {
            Some(h) => self.facts_of_handle(h).unwrap_or_default(),
            None => ResolveResultFacts::default(),
        }
    }

    /// `GetSymbol() as IMethod`, with the handle of the resolve result.
    fn method_of(&mut self, node: SyntaxId) -> Option<(u32, MethodInfo)> {
        let rr = self.resolve_result(node)?;
        q::method(self.oracle, rr).map(|m| (rr, m))
    }

    fn process_invocation(&mut self, inv: SyntaxId) -> Result<(), String> {
        let Some((rr, method)) = self.method_of(inv) else {
            return Ok(());
        };
        let mut arguments = self
            .tree
            .children(inv, invocation_expression::slot::ARGUMENTS)
            .to_vec();

        // Reduce "String.Concat(a, b)" to "a + b"
        if is_string_concat(&method) && self.setting("StringConcat")? {
            if let [single] = arguments[..]
                && self.tree.kind(single) == kind::ARRAY_CREATE_EXPRESSION
                && let Some(initializer) = self
                    .tree
                    .child(single, array_create_expression::slot::INITIALIZER)
                && matches!(
                    method.parameters[..],
                    [q::ParameterInfo {
                        array_element: Some(_),
                        ..
                    }]
                )
            {
                arguments = self
                    .tree
                    .children(initializer, array_initializer_expression::slot::ELEMENTS)
                    .to_vec();
            }
            if !self.check_arguments_for_string_concat(&arguments) {
                return Ok(());
            }
            let is_in_expression_tree = self.is_in_expression_tree(inv);
            let mut arg0 = arguments[0];
            self.tree.detach(arg0);
            let mut arg1 = arguments[1];
            self.tree.detach(arg1);
            if !is_in_expression_tree {
                arg1 =
                    self.remove_redundant_to_string_in_concat(arg1, &method, arguments.len() == 2);
                self.tree.detach(arg1);
                if self.facts(arg1).known_type == KnownTypeCode::String as u8 {
                    arg0 = self.remove_redundant_to_string_in_concat(arg0, &method, false);
                    self.tree.detach(arg0);
                }
            }
            let mut expr = self.binary(arg0, bop::ADD, arg1);
            for i in 2..arguments.len() {
                let mut arg = arguments[i];
                self.tree.detach(arg);
                if !is_in_expression_tree {
                    arg = self.remove_redundant_to_string_in_concat(
                        arg,
                        &method,
                        i == arguments.len() - 1,
                    );
                    self.tree.detach(arg);
                }
                expr = self.binary(expr, bop::ADD, arg);
            }
            self.copy_annotations(expr, inv);
            self.tree.replace(inv, expr);
            return Ok(());
        }

        match method.full_name.as_str() {
            "System.Type.GetTypeFromHandle" => {
                if arguments.len() == 1 && self.matches_type_handle_on_type_of(arguments[0]) {
                    let target = self
                        .tree
                        .child(arguments[0], member_reference_expression::slot::TARGET)
                        .expect("matched");
                    self.copy_instructions(target, inv);
                    self.tree.replace(inv, target);
                    return Ok(());
                }
            }
            "System.Activator.CreateInstance" => {
                if self.setting("UseObjectCreationOfGenericTypeParameter")?
                    && arguments.is_empty()
                    && method.type_argument_count == 1
                    && method.first_type_argument_is_instantiable
                {
                    let ty = self.convert_type(rr, 0)?;
                    let object_create = self.tree.new_node(kind::OBJECT_CREATE_EXPRESSION);
                    self.tree.set_child(
                        object_create,
                        object_create_expression::slot::TYPE,
                        Some(ty),
                    );
                    self.tree.replace(inv, object_create);
                }
            }
            "System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray"
                if arguments.len() == 2 && self.setting("Ranges")? =>
            {
                let slicing = self.tree.new_node(kind::INDEXER_EXPRESSION);
                self.tree.set_child(
                    slicing,
                    indexer_expression::slot::TARGET,
                    Some(arguments[0]),
                );
                self.tree.insert_child(
                    slicing,
                    indexer_expression::slot::ARGUMENTS,
                    0,
                    arguments[1],
                );
                self.copy_annotations(slicing, inv);
                self.tree.replace(inv, slicing);
            }
            _ => {}
        }

        if let Some((op, is_checked)) = self.binary_operator_from_metadata_name(&method.name)?
            && arguments.len() == 2
        {
            for &a in &arguments {
                self.tree.detach(a);
            }
            self.add_checked_annotation(inv, rr, &method, is_checked);
            let left = self.unwrap_in_direction_expression(arguments[0]);
            let right = self.unwrap_in_direction_expression(arguments[1]);
            let binary = self.binary(left, op, right);
            self.copy_annotations(binary, inv);
            self.tree.replace(inv, binary);
            return Ok(());
        }
        if let Some((op, is_checked)) = self.unary_operator_from_metadata_name(&method.name)?
            && arguments.len() == 1
        {
            self.add_checked_annotation(inv, rr, &method, is_checked);
            if op == uop::INCREMENT || op == uop::DECREMENT {
                // `op_Increment(a)` is not `++a`, which also assigns; legacy csc turns "d + 1m"
                // into "op_Increment(d)", which is reversed here.
                if method.declaring_known_type == KnownTypeCode::Decimal as u8 {
                    let operand = self.unwrap_in_direction_expression(arguments[0]);
                    self.tree.detach(operand);
                    let one = self.tree.new_node(kind::PRIMITIVE_EXPRESSION);
                    let value = self.tree.new_annotation(ANNOTATION_DECIMAL_ONE, Vec::new());
                    self.tree.node_mut(one).scalars[primitive_expression::scalar::VALUE] =
                        Scalar::Object(value);
                    let arithmetic = self.binary(
                        operand,
                        if op == uop::INCREMENT {
                            bop::ADD
                        } else {
                            bop::SUBTRACT
                        },
                        one,
                    );
                    self.copy_annotations(arithmetic, inv);
                    self.tree.replace(inv, arithmetic);
                }
            } else {
                self.tree.detach(arguments[0]);
                let operand = self.unwrap_in_direction_expression(arguments[0]);
                let unary = self.unary(op, operand);
                self.copy_annotations(unary, inv);
                self.tree.replace(inv, unary);
            }
            return Ok(());
        }
        if (method.name == "op_Explicit" || method.name == "op_CheckedExplicit")
            && arguments.len() == 1
        {
            self.tree.detach(arguments[0]);
            let is_checked = method.name == "op_CheckedExplicit";
            self.add_checked_annotation(inv, rr, &method, is_checked);
            let ty = self.convert_type(rr, 1)?;
            let operand = self.unwrap_in_direction_expression(arguments[0]);
            let cast = self.tree.new_node(kind::CAST_EXPRESSION);
            self.tree
                .set_child(cast, cast_expression::slot::TYPE, Some(ty));
            self.tree
                .set_child(cast, cast_expression::slot::EXPRESSION, Some(operand));
            self.copy_annotations(cast, inv);
            self.tree.replace(inv, cast);
            return Ok(());
        }
        if method.name == "op_True" && arguments.len() == 1 && self.is_in_condition_slot(inv) {
            let condition = self.unwrap_in_direction_expression(arguments[0]);
            self.tree.replace(inv, condition);
        }
        Ok(())
    }

    /// The checked or unchecked annotation the managed transform adds to the invocation before
    /// it copies the invocation's annotations to the operator.
    fn add_checked_annotation(
        &mut self,
        inv: SyntaxId,
        rr: u32,
        method: &MethodInfo,
        is_checked: bool,
    ) {
        let kind = if is_checked {
            ANNOTATION_CHECKED
        } else if self.has_checked_equivalent(rr, method) {
            ANNOTATION_UNCHECKED
        } else {
            return;
        };
        let h = self.tree.new_annotation(kind, Vec::new());
        self.tree.node_mut(inv).annotations.push(h);
    }

    fn has_checked_equivalent(&self, rr: u32, method: &MethodInfo) -> bool {
        let name = match method.name.strip_prefix("op_") {
            Some(rest) => format!("op_Checked{rest}"),
            None => method.name.clone(),
        };
        q::has_operator(self.oracle, rr, &name)
    }

    fn is_in_expression_tree(&mut self, node: SyntaxId) -> bool {
        let mut ancestor = self.tree.node(node).parent;
        while let Some(a) = ancestor {
            if self.tree.kind(a) == kind::LAMBDA_EXPRESSION {
                let function_kind = self
                    .tree
                    .node(a)
                    .annotations
                    .iter()
                    .find_map(|&h| q::il_instruction(self.oracle, h).flatten());
                if function_kind == Some(IL_FUNCTION_KIND_EXPRESSION_TREE) {
                    return true;
                }
            }
            ancestor = self.tree.node(a).parent;
        }
        false
    }

    fn is_in_condition_slot(&self, node: SyntaxId) -> bool {
        let Some(parent) = self.tree.node(node).parent else {
            return false;
        };
        self.tree
            .parent_slot(node)
            .is_some_and(|slot| self.tree.node(parent).schema().slots[slot].kind == "Condition")
    }

    fn check_arguments_for_string_concat(&mut self, arguments: &[SyntaxId]) -> bool {
        if arguments.len() < 2 {
            return false;
        }
        if arguments
            .iter()
            .any(|&a| self.tree.kind(a) == kind::NAMED_ARGUMENT_EXPRESSION)
        {
            return false;
        }
        // All implicit ToString() calls but the last must be free of side effects, since the
        // compilers disagree on when they happen (https://github.com/dotnet/roslyn/issues/38641).
        for &arg in &arguments[..arguments.len() - 1] {
            if !to_string_is_known_effect_free(&self.facts(arg)) {
                return false;
            }
        }
        for &arg in arguments {
            let facts = self.facts(arg);
            // A nested string.Concat() stays a call so that the evaluation order does not change.
            if facts.is_string_concat_invocation {
                return false;
            }
            // ref structs cannot be converted to object for use with +
            if facts.is_by_ref_like {
                return false;
            }
        }
        // One of the first two arguments must be string for + to be a string concatenation.
        self.facts(arguments[0]).known_type == KnownTypeCode::String as u8
            || self.facts(arguments[1]).known_type == KnownTypeCode::String as u8
    }

    /// `target` and whether the null-conditional form matched, where `expr` is `target.ToString()`
    /// or `target?.ToString()`; with the `ToString()` invocation.
    fn match_to_string_call(&self, expr: SyntaxId) -> Option<(SyntaxId, SyntaxId, bool)> {
        let t = &*self.tree;
        let to_string_call = |call: SyntaxId| -> Option<SyntaxId> {
            if t.kind(call) != kind::INVOCATION_EXPRESSION
                || !t
                    .children(call, invocation_expression::slot::ARGUMENTS)
                    .is_empty()
            {
                return None;
            }
            let mre = t.child(call, invocation_expression::slot::TARGET)?;
            if !self.is_member_reference(mre, "ToString") {
                return None;
            }
            t.child(mre, member_reference_expression::slot::TARGET)
        };
        if let Some(target) = to_string_call(expr) {
            return Some((target, expr, false));
        }
        if t.kind(expr) != kind::UNARY_OPERATOR_EXPRESSION
            || operator(t, expr, unary_operator_expression::scalar::OPERATOR)
                != uop::NULL_CONDITIONAL_REWRAP
        {
            return None;
        }
        let call = t.child(expr, unary_operator_expression::slot::EXPRESSION)?;
        let conditional = to_string_call(call)?;
        if t.kind(conditional) != kind::UNARY_OPERATOR_EXPRESSION
            || operator(t, conditional, unary_operator_expression::scalar::OPERATOR)
                != uop::NULL_CONDITIONAL
        {
            return None;
        }
        let target = t.child(conditional, unary_operator_expression::slot::EXPRESSION)?;
        Some((target, call, true))
    }

    fn remove_redundant_to_string_in_concat(
        &mut self,
        expr: SyntaxId,
        concat_method: &MethodInfo,
        is_last_argument: bool,
    ) -> SyntaxId {
        let Some((target, call, null_conditional)) = self.match_to_string_call(expr) else {
            return expr;
        };
        // With object parameters string.Concat() calls ToString() itself, so the compiler
        // generates no ToString() calls of its own.
        if !concat_method.parameters.iter().all(is_string_parameter) {
            return expr;
        }
        let to_string_method = self.method_of(call).map(|(_, m)| m);
        let facts = self.facts(target);
        // ref structs cannot be converted to object for use with +
        if facts.is_by_ref_like {
            return expr;
        }
        let effect_free = to_string_is_known_effect_free(&facts);
        if !(is_last_argument || effect_free) {
            return expr;
        }
        // ToString() might throw NullReferenceException, the builtin operator+ does not.
        if facts.is_reference_type != Some(false) && !null_conditional {
            return expr;
        }
        // operator+ calls ToString() on a copy of a struct, an explicit call on the original.
        if !effect_free && to_string_method.is_some_and(|m| m.requires_copy_for_readonly_lvalue) {
            return expr;
        }
        target
    }

    /// `typeof(T).TypeHandle` or `__reftype(x).TypeHandle`.
    fn matches_type_handle_on_type_of(&self, node: SyntaxId) -> bool {
        let t = &*self.tree;
        if !self.is_member_reference(node, "TypeHandle") {
            return false;
        }
        let Some(target) = t.child(node, member_reference_expression::slot::TARGET) else {
            return false;
        };
        match t.kind(target) {
            kind::TYPE_OF_EXPRESSION => t.child(target, type_of_expression::slot::TYPE).is_some(),
            kind::UNDOCUMENTED_EXPRESSION => {
                operator(
                    t,
                    target,
                    undocumented_expression::scalar::UNDOCUMENTED_EXPRESSION_TYPE,
                ) == UNDOCUMENTED_REF_TYPE
                    && t.children(target, undocumented_expression::slot::ARGUMENTS)
                        .len()
                        == 1
            }
            _ => false,
        }
    }

    /// A member reference named `name` without type arguments.
    fn is_member_reference(&self, node: SyntaxId, name: &str) -> bool {
        let t = &*self.tree;
        t.kind(node) == kind::MEMBER_REFERENCE_EXPRESSION
            && t.children(node, member_reference_expression::slot::TYPE_ARGUMENTS)
                .is_empty()
            && t.child(node, member_reference_expression::slot::MEMBER_NAME_TOKEN)
                .is_some_and(|id| {
                    t.node(id).scalars[identifier::scalar::NAME]
                        .as_str()
                        .is_some_and(|n| n == name)
                })
    }

    /// `TypePattern`: a type (or a composed type without specifiers around it) whose
    /// `TypeResolveResult` names `System.Reflection.<name>` for one of `names`.
    fn matches_reflection_type(&mut self, node: Option<SyntaxId>, names: &[&str]) -> bool {
        let Some(mut node) = node else {
            return false;
        };
        if self.tree.kind(node) == kind::COMPOSED_TYPE {
            let n = self.tree.node(node);
            if n.scalars[composed_type::scalar::HAS_REF_SPECIFIER] == Scalar::Bool(false)
                && n.scalars[composed_type::scalar::HAS_NULLABLE_SPECIFIER] == Scalar::Bool(false)
                && n.scalars[composed_type::scalar::POINTER_RANK] == Scalar::Int(0)
                && self
                    .tree
                    .children(node, composed_type::slot::ARRAY_SPECIFIERS)
                    .is_empty()
            {
                match self.tree.child(node, composed_type::slot::BASE_TYPE) {
                    Some(base) => node = base,
                    None => return false,
                }
            }
        }
        let Some(rr) = self.resolve_result(node) else {
            return false;
        };
        q::type_resolve_result_name(self.oracle, rr)
            .is_some_and(|(ns, name)| ns == "System.Reflection" && names.contains(&name.as_str()))
    }

    /// The `methodof` pattern: `(MethodInfo)MethodBase.GetMethodFromHandle(ldtoken(m).MethodHandle
    /// [, typeof(T).TypeHandle])` becomes the ldtoken, its operand written as a call on `T`.
    fn visit_cast(&mut self, cast: SyntaxId) -> Result<(), String> {
        let ty = self.tree.child(cast, cast_expression::slot::TYPE);
        if !self.matches_reflection_type(ty, &["MethodInfo", "ConstructorInfo"]) {
            return Ok(());
        }
        let Some(inv) = self.tree.child(cast, cast_expression::slot::EXPRESSION) else {
            return Ok(());
        };
        if self.tree.kind(inv) != kind::INVOCATION_EXPRESSION {
            return Ok(());
        }
        let Some(callee) = self.tree.child(inv, invocation_expression::slot::TARGET) else {
            return Ok(());
        };
        if !self.is_member_reference(callee, "GetMethodFromHandle") {
            return Ok(());
        }
        let Some(type_reference) = self
            .tree
            .child(callee, member_reference_expression::slot::TARGET)
            .filter(|&n| self.tree.kind(n) == kind::TYPE_REFERENCE_EXPRESSION)
        else {
            return Ok(());
        };
        let base = self
            .tree
            .child(type_reference, type_reference_expression::slot::TYPE);
        if !self.matches_reflection_type(base, &["MethodBase"]) {
            return Ok(());
        }
        let arguments = self
            .tree
            .children(inv, invocation_expression::slot::ARGUMENTS)
            .to_vec();
        if arguments.is_empty() || arguments.len() > 2 {
            return Ok(());
        }
        let ldtoken_node = arguments[0];
        let Some(method_node) = self.match_ldtoken_method_handle(ldtoken_node) else {
            return Ok(());
        };
        let declaring_type = match arguments.get(1) {
            None => None,
            Some(&a) => {
                if !self.is_member_reference(a, "TypeHandle") {
                    return Ok(());
                }
                let Some(type_of) = self
                    .tree
                    .child(a, member_reference_expression::slot::TARGET)
                    .filter(|&n| self.tree.kind(n) == kind::TYPE_OF_EXPRESSION)
                else {
                    return Ok(());
                };
                let Some(t) = self.tree.child(type_of, type_of_expression::slot::TYPE) else {
                    return Ok(());
                };
                Some(t)
            }
        };
        if let Some(declaring_type) = declaring_type
            && let Some((rr, method)) = self.method_of(method_node)
        {
            let type_reference = self.tree.new_node(kind::TYPE_REFERENCE_EXPRESSION);
            self.tree.set_child(
                type_reference,
                type_reference_expression::slot::TYPE,
                Some(declaring_type),
            );
            let member = self.member_reference(type_reference, &method.name);
            let call = self.tree.new_node(kind::INVOCATION_EXPRESSION);
            self.tree
                .set_child(call, invocation_expression::slot::TARGET, Some(member));
            for i in 0..method.parameters.len() {
                let ty = self.convert_type(rr, 2 + i)?;
                let argument = self.tree.new_node(kind::TYPE_REFERENCE_EXPRESSION);
                self.tree
                    .set_child(argument, type_reference_expression::slot::TYPE, Some(ty));
                self.tree
                    .insert_child(call, invocation_expression::slot::ARGUMENTS, i, argument);
            }
            self.tree.replace(method_node, call);
        }
        self.copy_annotations(ldtoken_node, cast);
        self.tree.replace(cast, ldtoken_node);
        Ok(())
    }

    /// `ldtoken(m).MethodHandle` (an invocation with an `LdTokenAnnotation` and one argument):
    /// the argument `m`.
    fn match_ldtoken_method_handle(&self, node: SyntaxId) -> Option<SyntaxId> {
        let t = &*self.tree;
        if !self.is_member_reference(node, "MethodHandle") {
            return None;
        }
        let ldtoken = t.child(node, member_reference_expression::slot::TARGET)?;
        if t.kind(ldtoken) != kind::INVOCATION_EXPRESSION {
            return None;
        }
        let has_annotation = t
            .node(ldtoken)
            .annotations
            .iter()
            .any(|&h| q::class_name(self.oracle, h).is_some_and(|c| c == "LdTokenAnnotation"));
        match t.children(ldtoken, invocation_expression::slot::ARGUMENTS) {
            [single] if has_annotation => Some(*single),
            _ => None,
        }
    }

    /// The type syntax the host builds for a type of the method of `rr`, moved into the tree.
    fn convert_type(&mut self, rr: u32, which: usize) -> Result<SyntaxId, String> {
        let (table, count, bytes) = q::convert_type(self.oracle, rr, which)
            .ok_or("the host built no type syntax for the method")?;
        let sub = SyntaxTree::decode(&bytes, count)?;
        self.tree
            .import(sub, |t, h| {
                let mut payload = Vec::new();
                uvar(&mut payload, table as u64);
                uvar(&mut payload, u64::from(h));
                t.new_annotation(ANNOTATION_ANSWERED, payload)
            })
            .ok_or_else(|| "an empty type syntax from the host".to_owned())
    }

    fn binary(&mut self, left: SyntaxId, op: i64, right: SyntaxId) -> SyntaxId {
        let b = self.tree.new_node(kind::BINARY_OPERATOR_EXPRESSION);
        self.tree.node_mut(b).scalars[binary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(op);
        self.tree
            .set_child(b, binary_operator_expression::slot::LEFT, Some(left));
        self.tree
            .set_child(b, binary_operator_expression::slot::RIGHT, Some(right));
        b
    }

    fn unary(&mut self, op: i64, operand: SyntaxId) -> SyntaxId {
        let u = self.tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
        self.tree.node_mut(u).scalars[unary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(op);
        self.tree.set_child(
            u,
            unary_operator_expression::slot::EXPRESSION,
            Some(operand),
        );
        u
    }

    /// `new MemberReferenceExpression(target, name)`, its identifier as `Identifier.Create`
    /// makes it.
    fn member_reference(&mut self, target: SyntaxId, name: &str) -> SyntaxId {
        let id = self.tree.new_node(kind::IDENTIFIER);
        let (text, verbatim) = match name.strip_prefix('@') {
            Some(rest) => (rest, true),
            None => (name, false),
        };
        let n = self.tree.node_mut(id);
        n.scalars[identifier::scalar::NAME] = Scalar::Str(text.encode_utf16().collect());
        n.scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
        let mre = self.tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
        self.tree
            .set_child(mre, member_reference_expression::slot::TARGET, Some(target));
        self.tree.set_child(
            mre,
            member_reference_expression::slot::MEMBER_NAME_TOKEN,
            Some(id),
        );
        mre
    }

    /// `UnwrapInDirectionExpression`: the detached operand of an `in` argument, else `expr`.
    fn unwrap_in_direction_expression(&mut self, expr: SyntaxId) -> SyntaxId {
        if self.tree.kind(expr) != kind::DIRECTION_EXPRESSION
            || operator(
                self.tree,
                expr,
                direction_expression::scalar::FIELD_DIRECTION,
            ) != FIELD_DIRECTION_IN
        {
            return expr;
        }
        match self
            .tree
            .child(expr, direction_expression::slot::EXPRESSION)
        {
            Some(inner) => {
                self.tree.detach(inner);
                inner
            }
            None => expr,
        }
    }

    /// `CopyAnnotationsFrom`: appends the annotations of `source` to `target`, and copies of its
    /// trivia.
    fn copy_annotations(&mut self, target: SyntaxId, source: SyntaxId) {
        let s = self.tree.node(source);
        let annotations = s.annotations.clone();
        let leading = s.leading_trivia.clone();
        let trailing = s.trailing_trivia.clone();
        self.tree.node_mut(target).annotations.extend(annotations);
        for (list, leading) in [(leading, true), (trailing, false)] {
            for t in list {
                let copy = self.tree.clone_subtree(t);
                self.tree.node_mut(copy).parent = Some(target);
                let n = self.tree.node_mut(target);
                if leading {
                    n.leading_trivia.push(copy);
                } else {
                    n.trailing_trivia.push(copy);
                }
            }
        }
    }

    /// `CopyInstructionsFrom`: appends the `ILInstruction` annotations of `source` to `target`.
    fn copy_instructions(&mut self, target: SyntaxId, source: SyntaxId) {
        let instructions: Vec<u32> = self
            .tree
            .node(source)
            .annotations
            .iter()
            .copied()
            .filter(|&h| q::il_instruction(self.oracle, h).is_some())
            .collect();
        self.tree.node_mut(target).annotations.extend(instructions);
    }

    fn binary_operator_from_metadata_name(
        &mut self,
        name: &str,
    ) -> Result<Option<(i64, bool)>, String> {
        Ok(match name {
            "op_Addition" => Some((bop::ADD, false)),
            "op_Subtraction" => Some((bop::SUBTRACT, false)),
            "op_Multiply" => Some((bop::MULTIPLY, false)),
            "op_Division" => Some((bop::DIVIDE, false)),
            "op_CheckedAddition" if self.setting("CheckedOperators")? => Some((bop::ADD, true)),
            "op_CheckedSubtraction" if self.setting("CheckedOperators")? => {
                Some((bop::SUBTRACT, true))
            }
            "op_CheckedMultiply" if self.setting("CheckedOperators")? => {
                Some((bop::MULTIPLY, true))
            }
            "op_CheckedDivision" if self.setting("CheckedOperators")? => Some((bop::DIVIDE, true)),
            "op_Modulus" => Some((bop::MODULUS, false)),
            "op_BitwiseAnd" => Some((bop::BITWISE_AND, false)),
            "op_BitwiseOr" => Some((bop::BITWISE_OR, false)),
            "op_ExclusiveOr" => Some((bop::EXCLUSIVE_OR, false)),
            "op_LeftShift" => Some((bop::SHIFT_LEFT, false)),
            "op_RightShift" => Some((bop::SHIFT_RIGHT, false)),
            "op_UnsignedRightShift" if self.setting("UnsignedRightShift")? => {
                Some((bop::UNSIGNED_SHIFT_RIGHT, false))
            }
            "op_Equality" => Some((bop::EQUALITY, false)),
            "op_Inequality" => Some((bop::IN_EQUALITY, false)),
            "op_LessThan" => Some((bop::LESS_THAN, false)),
            "op_LessThanOrEqual" => Some((bop::LESS_THAN_OR_EQUAL, false)),
            "op_GreaterThan" => Some((bop::GREATER_THAN, false)),
            "op_GreaterThanOrEqual" => Some((bop::GREATER_THAN_OR_EQUAL, false)),
            _ => None,
        })
    }

    fn unary_operator_from_metadata_name(
        &mut self,
        name: &str,
    ) -> Result<Option<(i64, bool)>, String> {
        Ok(match name {
            "op_LogicalNot" => Some((uop::NOT, false)),
            "op_OnesComplement" => Some((uop::BIT_NOT, false)),
            "op_UnaryNegation" => Some((uop::MINUS, false)),
            "op_CheckedUnaryNegation" if self.setting("CheckedOperators")? => {
                Some((uop::MINUS, true))
            }
            "op_UnaryPlus" => Some((uop::PLUS, false)),
            "op_Increment" => Some((uop::INCREMENT, false)),
            "op_Decrement" => Some((uop::DECREMENT, false)),
            "op_CheckedIncrement" if self.setting("CheckedOperators")? => {
                Some((uop::INCREMENT, true))
            }
            "op_CheckedDecrement" if self.setting("CheckedOperators")? => {
                Some((uop::DECREMENT, true))
            }
            _ => None,
        })
    }
}

fn operator(tree: &SyntaxTree, node: SyntaxId, scalar: usize) -> i64 {
    tree.node(node).scalars[scalar].as_int().unwrap_or(-1)
}

fn is_string_concat(method: &MethodInfo) -> bool {
    method.name == "Concat" && method.declaring_known_type == KnownTypeCode::String as u8
}

fn is_string_parameter(p: &q::ParameterInfo) -> bool {
    let code = match p.array_element {
        Some(element) if p.is_params => element,
        _ => p.known_type,
    };
    code == KnownTypeCode::String as u8
}

/// Whether `ToString()` on the type of `facts` (or its nullable underlying type) has no side
/// effects.
fn to_string_is_known_effect_free(facts: &ResolveResultFacts) -> bool {
    use KnownTypeCode as K;
    [
        K::Boolean,
        K::Char,
        K::SByte,
        K::Byte,
        K::Int16,
        K::UInt16,
        K::Int32,
        K::UInt32,
        K::Int64,
        K::UInt64,
        K::Single,
        K::Double,
        K::Decimal,
        K::IntPtr,
        K::UIntPtr,
        K::String,
    ]
    .iter()
    .any(|&k| k as u8 == facts.underlying_known_type)
}

fn uvar(out: &mut Vec<u8>, mut v: u64) {
    loop {
        let b = (v & 0x7F) as u8;
        v >>= 7;
        if v == 0 {
            out.push(b);
            return;
        }
        out.push(b | 0x80);
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn string_parameters_include_params_string_arrays() {
        let s = KnownTypeCode::String as u8;
        let p = |known_type, is_params, array_element| q::ParameterInfo {
            known_type,
            is_params,
            array_element,
        };
        assert!(is_string_parameter(&p(s, false, None)));
        assert!(is_string_parameter(&p(
            KnownTypeCode::None as u8,
            true,
            Some(s)
        )));
        assert!(!is_string_parameter(&p(
            KnownTypeCode::None as u8,
            false,
            Some(s)
        )));
        assert!(!is_string_parameter(&p(
            KnownTypeCode::Object as u8,
            false,
            None
        )));
    }

    #[test]
    fn to_string_of_primitive_types_is_effect_free() {
        let facts = |code: KnownTypeCode| ResolveResultFacts {
            underlying_known_type: code as u8,
            ..ResolveResultFacts::default()
        };
        assert!(to_string_is_known_effect_free(&facts(KnownTypeCode::Int32)));
        assert!(to_string_is_known_effect_free(&facts(
            KnownTypeCode::String
        )));
        assert!(!to_string_is_known_effect_free(&facts(
            KnownTypeCode::Object
        )));
        assert!(!to_string_is_known_effect_free(&facts(KnownTypeCode::None)));
    }
}
