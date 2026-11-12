//! `StatementBuilder.TransformToForeach`: a `using` of an enumerator whose body is the
//! `while (e.MoveNext())` loop over `e.Current`, as a `foreach` statement. Like the managed
//! builder, the translation changes the ILAst: the loop variable becomes the foreach
//! variable, with the collection's element name and the getter's type.

use std::collections::HashMap;

use crate::il::ast::{ContainerKind, Inst, NodeId, VarId, VariableKind};
use crate::il::transforms::assign_variable_names::{
    add_existing_name, name_from_instruction, split_name,
};
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `MethodSemanticsAttributes.Getter`.
const ACCESSOR_GETTER: u16 = 0x02;
/// `MethodSemanticsAttributes.Setter`.
const ACCESSOR_SETTER: u16 = 0x01;

impl Builder<'_, '_, '_, '_> {
    /// `TransformToForeach(UsingInstruction, Expression)`: the `foreach` statement for the
    /// `using` of `variable`, `None` where the using is no foreach loop.
    pub(super) fn transform_to_foreach(
        &mut self,
        variable: VarId,
        resource_expression: NodeId,
        resource: &TranslatedExpression,
        body: NodeId,
    ) -> Result<Option<AstId>, Unsupported> {
        if !self.settings.foreach_statement {
            return Ok(None);
        }
        let Some((collection, is_async)) = self.match_get_enumerator_pattern(resource)? else {
            return Ok(None);
        };
        // `isAsync != inst.IsAsync`: this using is synchronous.
        if is_async {
            return Ok(None);
        }
        if !matches!(self.f.inst(body), Inst::BlockContainer { .. }) {
            return Ok(None);
        }
        let (loop_container, optional_leave) = self.unwrap_nested_container(body);
        self.transform_to_foreach_loop(
            body,
            loop_container,
            optional_leave,
            variable,
            collection,
            resource_expression,
        )
    }

    /// `MatchGetEnumeratorPattern`: `collection.GetEnumerator()` (or `GetAsyncEnumerator`),
    /// the collection and whether it is asynchronous; the static spelling of an extension
    /// method is not translated.
    fn match_get_enumerator_pattern(
        &self,
        resource: &TranslatedExpression,
    ) -> Result<Option<(AstId, bool)>, Unsupported> {
        let AstNode::Invocation { target, arguments } = self.ast.node(resource.expression) else {
            return Ok(None);
        };
        let AstNode::MemberReference {
            target: collection,
            name,
        } = self.ast.node(*target)
        else {
            return Ok(None);
        };
        let is_async = match name.as_str() {
            "GetEnumerator" => false,
            "GetAsyncEnumerator" => true,
            _ => return Ok(None),
        };
        let extension = matches!(
            self.ast.annotations(resource.expression).rr,
            Some(Rr::Invocation {
                extension: true,
                ..
            })
        );
        if !arguments.is_empty() {
            // `extensionGetEnumeratorPattern`: `Type.GetEnumerator(collection)`.
            if self.settings.extension_methods
                && self.settings.foreach_with_get_enumerator_extension
                && arguments.len() == 1
            {
                return Err("a foreach over a static GetEnumerator extension call".to_owned());
            }
            return Ok(None);
        }
        let extension_pattern_allowed =
            self.settings.extension_methods && self.settings.foreach_with_get_enumerator_extension;
        // An extension GetEnumerator in extension method syntax is spelled like an instance call.
        if extension && !extension_pattern_allowed {
            return Ok(None);
        }
        Ok(Some((*collection, is_async)))
    }

    /// `UnwrapNestedContainerIfPossible`: the container nested in the only block of
    /// `container` that ends in a leave, and that leave where it carries a pure value.
    fn unwrap_nested_container(&self, container: NodeId) -> (NodeId, Option<NodeId>) {
        let Inst::BlockContainer { blocks, .. } = self.f.inst(container) else {
            return (container, None);
        };
        let [block] = blocks[..] else {
            return (container, None);
        };
        let Inst::Block {
            instructions,
            final_inst,
            ..
        } = self.f.inst(block)
        else {
            return (container, None);
        };
        // The block's two instructions: the nested container and the leave.
        let all: Vec<NodeId> = instructions.iter().copied().chain([*final_inst]).collect();
        let all: Vec<NodeId> = all
            .into_iter()
            .filter(|&n| !matches!(self.f.inst(n), Inst::Nop { .. }))
            .collect();
        let [nested, leave] = all[..] else {
            return (container, None);
        };
        if !matches!(self.f.inst(nested), Inst::BlockContainer { .. }) {
            return (container, None);
        }
        let Inst::Leave { target, value } = *self.f.inst(leave) else {
            return (container, None);
        };
        if target == Some(container) && matches!(self.f.inst(value), Inst::Nop { .. }) {
            return (nested, None);
        }
        if crate::il::semantic_helper::is_pure(self.f.flags(value)) {
            return (nested, Some(leave));
        }
        (container, None)
    }

    /// `TransformToForeach(BlockContainer, ...)` where the loop variable is the existing
    /// variable the first statement of the loop body stores `enumerator.Current` (cast to
    /// its type) in (`RequiredGetCurrentTransformation.UseExistingVariable`).
    fn transform_to_foreach_loop(
        &mut self,
        container: NodeId,
        loop_container: NodeId,
        optional_leave: Option<NodeId>,
        enumerator: VarId,
        collection: AstId,
        resource_expression: NodeId,
    ) -> Result<Option<AstId>, Unsupported> {
        let Inst::BlockContainer {
            kind: ContainerKind::While,
            blocks,
            ..
        } = self.f.inst(loop_container).clone()
        else {
            return Ok(None);
        };
        let Some((condition_inst, body_block)) =
            self.match_condition_block(loop_container, blocks[0])
        else {
            return Ok(None);
        };
        // The loop condition is `enumerator.MoveNext()`.
        let condition = self.translate_condition(condition_inst)?;
        let moves_next = match self.ast.node(condition.expression) {
            AstNode::Invocation { target, arguments } if arguments.is_empty() => {
                match self.ast.node(*target) {
                    AstNode::MemberReference { target, name } if name == "MoveNext" => {
                        matches!(self.ast.node(*target), AstNode::Identifier { .. })
                            && matches!(
                                self.ast.annotations(*target).rr,
                                Some(Rr::Variable { var, .. }) if var == enumerator
                            )
                    }
                    _ => false,
                }
            }
            _ => false,
        };
        if !moves_next {
            return Ok(None);
        }
        // `DetectGetCurrentTransformation`
        let (single_getter, foreach_variable, getter_type) = match self.detect_get_current(
            container,
            body_block,
            loop_container,
            enumerator,
            condition_inst,
        )? {
            Some(found) => found,
            None => return Ok(None),
        };
        // `foreach (var item in this)` is decompiled with `base`, which is not valid here.
        let collection = if matches!(self.ast.node(collection), AstNode::Base) {
            let this = self.ast.add(AstNode::This);
            let annotations = self.ast.annotations(collection).clone();
            *self.ast.annotations_mut(this) = annotations;
            this
        } else {
            collection
        };
        // `IsDynamicCastToIEnumerable`
        if let AstNode::Cast { expression, .. } = *self.ast.node(collection) {
            let input = self.rr_of(expression);
            let input_ty = self.rr_type(&input);
            if self.ts.kind(input_ty) == crate::semantics::types::TypeKind::Dynamic {
                return Err("a foreach over a dynamic collection".to_owned());
            }
        }
        if self.may_contain_anonymous_type(getter_type) {
            return Err("a foreach variable of an anonymous type".to_owned());
        }
        if matches!(self.ts.data(getter_type), TypeData::Tuple { .. })
            || self.ts.tuple_element_types(getter_type).is_some()
        {
            return Err("a foreach variable of a tuple type".to_owned());
        }

        // `UseExistingVariable`: the variable becomes the foreach variable.
        let value_context = self.ast.annotations(collection).il.first().copied();
        let name = self.generate_foreach_variable_name(value_context, foreach_variable)?;
        let old_name = self.f.var(foreach_variable).name.clone();
        {
            let v = self.f.var_mut(foreach_variable);
            v.kind = VariableKind::ForeachLocal;
            v.name = name.clone();
        }
        if self.ts.kind(self.f.var(foreach_variable).ty)
            != crate::semantics::types::TypeKind::Dynamic
        {
            self.f.var_mut(foreach_variable).ty = getter_type;
        }
        if let Some(n) = self.variable_names.iter_mut().find(|n| **n == old_name) {
            *n = name;
        }

        // The modified body as C#: the while loop, without the store to the foreach variable.
        let while_block = self.convert_as_block(container)?;
        let AstNode::Block { statements } = self.ast.node(while_block).clone() else {
            unreachable!("a block");
        };
        let Some(&while_loop) = statements.first() else {
            return Err("a foreach loop that is no while loop".to_owned());
        };
        let AstNode::While { body, .. } = *self.ast.node(while_loop) else {
            return Err("a foreach loop that is no while loop".to_owned());
        };
        match self.ast.node_mut(body) {
            AstNode::Block { statements } if !statements.is_empty() => {
                statements.remove(0);
            }
            _ => return Err("a foreach body without the store to its variable".to_owned()),
        }
        let foreach = self.ast.add(AstNode::Foreach {
            variable: foreach_variable,
            collection,
            body,
            get_enumerator: resource_expression,
            move_next: condition_inst,
            get_current: single_getter,
        });
        for inst in self.ast.annotations(while_loop).il.clone() {
            self.ast.with_il(foreach, inst);
        }
        if optional_leave.is_none() && statements.len() == 1 {
            return Ok(Some(foreach));
        }
        // The leave after the loop, and what else the block held, follow the foreach loop.
        let mut block = vec![foreach];
        if let Some(leave) = optional_leave {
            block.push(self.convert(leave)?);
        }
        for &s in statements.iter().skip(1).skip_while(|&&s| {
            optional_leave.is_some_and(|l| self.ast.annotations(s).il.contains(&l))
        }) {
            block.push(s);
        }
        Ok(Some(self.ast.add(AstNode::Block { statements: block })))
    }

    /// `DetectGetCurrentTransformation` for `UseExistingVariable`: the getter call of
    /// `Current`, the variable the loop body's first statement stores it in, and the type the
    /// value is cast to (the getter's return type without a cast). Other transformations
    /// (a new variable, a local copy, a deconstruction) are not translated.
    fn detect_get_current(
        &mut self,
        using_container: NodeId,
        loop_body: NodeId,
        loop_container: NodeId,
        enumerator: VarId,
        move_next: NodeId,
    ) -> Result<Option<(NodeId, VarId, TypeId)>, Unsupported> {
        let v = self.f.var(enumerator);
        let loads: Vec<NodeId> = v
            .load_instructions
            .iter()
            .chain(&v.address_instructions)
            .copied()
            .filter(|&l| !self.is_descendant_of(l, move_next))
            .collect();
        let [load] = loads[..] else {
            return Ok(None);
        };
        let Some(getter) = self.f.node(load).parent else {
            return Ok(None);
        };
        let getter_type = match self.f.inst(getter) {
            Inst::Call(c) | Inst::CallVirt(c)
                if c.method.method.accessor_kind == ACCESSOR_GETTER =>
            {
                c.method.method.ret
            }
            _ => return Ok(None),
        };
        let Inst::Block { instructions, .. } = self.f.inst(loop_body) else {
            return Ok(None);
        };
        let Some(&first) = instructions.first() else {
            return Ok(None);
        };
        if !self.is_descendant_of(getter, first) {
            return Ok(None);
        }
        // The value of the first statement: the getter, cast where the variable is of another type.
        let mut inst = getter;
        let mut ty = getter_type;
        while let Some(parent) = self.f.node(inst).parent {
            match self.f.inst(parent) {
                Inst::CastClass { ty: t, .. } | Inst::UnboxAny { ty: t, .. } => {
                    ty = t.ty;
                    inst = parent;
                }
                _ => break,
            }
        }
        let Inst::StLoc { var, .. } = *self.f.inst(first) else {
            return Err("a foreach that introduces a variable".to_owned());
        };
        if self.f.node(inst).parent != Some(first)
            || !matches!(
                self.f.var(var).kind,
                VariableKind::Local | VariableKind::StackSlot
            )
        {
            return Err("a foreach that introduces a variable".to_owned());
        }
        if !self.variable_is_only_used_in_block(var, first, using_container, loop_container) {
            return Err("a foreach that introduces a variable".to_owned());
        }
        Ok(Some((getter, var, ty)))
    }

    /// `VariableIsOnlyUsedInBlock`: `var` is loaded only inside `using_container`, its
    /// address is only taken there for a call on it or a field load, `store` is its only
    /// store, and it is captured by the loop at most.
    fn variable_is_only_used_in_block(
        &mut self,
        var: VarId,
        store: NodeId,
        using_container: NodeId,
        loop_container: NodeId,
    ) -> bool {
        let variable = self.f.var(var).clone();
        if variable
            .load_instructions
            .iter()
            .any(|&l| !self.is_descendant_of(l, using_container))
        {
            return false;
        }
        for &la in &variable.address_instructions {
            if !self.address_use_allowed(la, using_container) {
                return false;
            }
        }
        if variable.store_instructions.iter().any(|&s| s != store) {
            return false;
        }
        variable.capture_scope.is_none() || variable.capture_scope == Some(loop_container)
    }

    /// `AddressUseAllowed`
    fn address_use_allowed(&mut self, la: NodeId, using_container: NodeId) -> bool {
        if !self.is_descendant_of(la, using_container) {
            return false;
        }
        let Inst::LdLoca { var } = *self.f.inst(la) else {
            return false;
        };
        let ty = self.f.var(var).ty;
        if crate::il::transforms::il_inlining::is_used_as_this_pointer_in_call_of(
            self.f, self.ts, la,
        ) && !self.is_target_of_setter_call(la, ty)
        {
            return true;
        }
        let mut current = self.f.node(la).parent;
        while let Some(c) = current
            && matches!(self.f.inst(c), Inst::LdFlda { .. })
        {
            current = self.f.node(c).parent;
        }
        current.is_some_and(|c| matches!(self.f.inst(c), Inst::LdObj { .. }))
    }

    /// `IsTargetOfSetterCall`: `inst` is the `this` argument of a value type's setter (or of
    /// another accessor of something that is no property).
    fn is_target_of_setter_call(&mut self, inst: NodeId, target_type: TypeId) -> bool {
        if self.f.node(inst).child_index != 0
            || self.ts.is_reference_type(target_type) == Some(true)
        {
            return false;
        }
        let Some(parent) = self.f.node(inst).parent else {
            return false;
        };
        let (Inst::Call(c) | Inst::CallVirt(c)) = self.f.inst(parent) else {
            return false;
        };
        let m = &c.method.method;
        if m.accessor_kind == 0 || m.is_static {
            return false;
        }
        let crate::typesystem::resolve::MethodOrigin::Def(d) = m.origin else {
            return false;
        };
        match self.ts.accessor_owner(d) {
            Some((token, _)) if token >> 24 == 0x17 => m.accessor_kind == ACCESSOR_SETTER,
            _ => true,
        }
    }

    /// `TransformToForeachWithoutDispose(Block, ref int)`: the bare
    /// `enumerator = collection.GetEnumerator(); while (enumerator.MoveNext()) { ... }` the
    /// compiler emits for an enumerator that never needs disposal, as a foreach statement over
    /// instructions `i` and `i + 1`.
    pub(super) fn transform_to_foreach_without_dispose(
        &mut self,
        instructions: &[NodeId],
        i: usize,
    ) -> Result<Option<AstId>, Unsupported> {
        let Inst::StLoc { var, value, .. } = *self.f.inst(instructions[i]) else {
            return Ok(None);
        };
        let Some(&loop_container) = instructions.get(i + 1) else {
            return Ok(None);
        };
        if !matches!(
            self.f.inst(loop_container),
            Inst::BlockContainer {
                kind: ContainerKind::While,
                ..
            }
        ) {
            return Ok(None);
        }
        if !self.settings.foreach_statement
            || !matches!(
                self.f.var(var).kind,
                VariableKind::Local | VariableKind::StackSlot
            )
            || !self.enumerator_type_can_never_be_disposable(self.f.var(var).ty)
            || !self.variable_is_only_used_in_block(
                var,
                instructions[i],
                loop_container,
                loop_container,
            )
        {
            return Ok(None);
        }
        let resource = self.translate(value, None)?;
        let Some((collection, is_async)) = self.match_get_enumerator_pattern(&resource)? else {
            return Ok(None);
        };
        // Async enumerators always implement IAsyncDisposable.
        if is_async {
            return Ok(None);
        }
        let transformed = self.transform_to_foreach_loop(
            loop_container,
            loop_container,
            None,
            var,
            collection,
            value,
        )?;
        Ok(transformed.map(|s| self.ast.with_il(s, loop_container)))
    }

    /// `EnumeratorTypeCanNeverBeDisposable`: a struct that is no ref struct, or a sealed
    /// class, without `IDisposable` among its base types.
    fn enumerator_type_can_never_be_disposable(&mut self, ty: TypeId) -> bool {
        let Some(def) = self.ts.definition(ty) else {
            return false;
        };
        let Ok(info) = self.ts.comp.type_def(def) else {
            return false;
        };
        match info.kind {
            crate::typesystem::compilation::TypeKind::Struct => {
                if self.ts.is_by_ref_like(ty) {
                    return false;
                }
            }
            crate::typesystem::compilation::TypeKind::Class => {
                // `TypeAttributes.Sealed`
                if info.flags & 0x0100 == 0 {
                    return false;
                }
            }
            _ => return false,
        }
        let Ok(bases) = self.ts.all_base_types(ty) else {
            return false;
        };
        !bases.iter().any(|&b| {
            self.ts
                .is_known(b, crate::typesystem::known::KnownTypeCode::IDisposable)
        })
    }

    fn is_descendant_of(&self, inst: NodeId, ancestor: NodeId) -> bool {
        crate::il::transforms::local_function_decompiler::is_descendant_of(self.f, inst, ancestor)
    }

    /// `AssignVariableNames.GenerateForeachVariableName`: the name of an existing variable
    /// that has one of its own, else the singular of the collection's name (`item` without
    /// one), made unique among the function's variables.
    fn generate_foreach_variable_name(
        &mut self,
        value_context: Option<NodeId>,
        existing: VarId,
    ) -> Result<String, Unsupported> {
        let v = self.f.var(existing);
        if !v.has_generated_name {
            return Ok(v.name.clone());
        }
        // `CollectReservedVariableNames`: the parameters of the top-level function and the
        // other variables of every function.
        let mut reserved: HashMap<String, i32> = HashMap::new();
        let mut functions = vec![self.root_variables.clone()];
        if let Some(body) = self.f.body {
            for n in self.f.descendants_post(body) {
                if let Inst::Function { variables, .. } = self.f.inst(n) {
                    functions.push(variables.clone());
                }
            }
        }
        for &p in &self.root_variables {
            let p = self.f.var(p);
            if p.kind == VariableKind::Parameter && p.index.is_some_and(|i| i >= 0) {
                add_existing_name(&mut reserved, &p.name);
            }
        }
        for variables in &functions {
            for &v in variables {
                let var = self.f.var(v);
                if var.kind != VariableKind::Parameter && v != existing {
                    add_existing_name(&mut reserved, &var.name);
                }
            }
        }
        let mut base_name = match value_context {
            Some(inst) => name_from_instruction(self.f, self.ts, inst),
            None => None,
        };
        if base_name.as_deref().is_none_or(str::is_empty)
            && let Some(inst) = value_context
            && let Inst::LdLoc { var } = *self.f.inst(inst)
            && self.f.var(var).kind == VariableKind::Parameter
        {
            base_name = Some(self.f.var(var).name.clone());
        }
        let mut proposed = "item".to_owned();
        if let Some(base) = base_name.filter(|b| !b.is_empty()) {
            let resolver = self
                .resolver
                .ok_or("a foreach variable name without the host's resolver")?;
            let singular = resolver
                .singularize(self.ts, &base)
                .ok_or("a foreach variable name the host cannot singularize")?;
            if !singular.trim().is_empty() && singular != base {
                proposed = singular;
            } else if base.len() > 4 && base.ends_with("List") {
                proposed = base[..base.len() - 4].to_owned();
            } else if base.eq_ignore_ascii_case("list") {
                proposed = "item".to_owned();
            } else if base.to_ascii_lowercase().ends_with("children") {
                proposed = base[..base.len() - 3].to_owned();
            }
        }
        let (proposed, _) = split_name(&proposed);
        let count = reserved.entry(proposed.clone()).or_insert(0);
        *count += 1;
        Ok(if *count > 1 {
            format!("{proposed}{count}")
        } else {
            proposed
        })
    }
}
