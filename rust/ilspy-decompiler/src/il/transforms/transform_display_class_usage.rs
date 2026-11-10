//! `IL/Transforms/TransformDisplayClassUsage.cs`: the fields of display classes (closures of
//! lambdas, local functions and expression trees) become local variables. A display class is
//! a variable initialized by an empty constructor call (or a display struct), never passed
//! elsewhere; its field accesses become `ldloca` of the new variables, and a field
//! initialized from a parameter, `this` or another display class is replaced by that variable.

use std::collections::HashMap;

use crate::il::ast::{
    BlockKind, FieldOperand, Function, Inst, NodeId, VarId, Variable, VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::metadata::tables::rows;
use crate::typesystem::compilation::{ModuleId, TypeDefId, TypeKind};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin, ResolvedField, ResolvedMethod};
use crate::typesystem::types::{GenericContext, TypeData, TypeId, TypeSystem};

use super::Context;
use super::early_expression_transforms::{ldobj_to_ldloc, stobj_to_stloc};
use super::il_inlining::{inline_one_if_possible, options};
use super::local_function_decompiler::{
    all_functions, declaring_type, function_variables, is_this, match_ld_loc_ref, variable_function,
};
use super::remove_dead_variable_init::reset_uses_initial_value_flag_of;

/// `(IField)field.MemberDefinition`: the field definition (or a fake field by name).
#[derive(Clone, Debug, PartialEq, Eq)]
struct FieldKey(FieldOrigin, String);

fn key(field: &ResolvedField) -> FieldKey {
    match field.origin {
        FieldOrigin::Def { .. } => FieldKey(field.origin, String::new()),
        FieldOrigin::Fake => FieldKey(field.origin, field.name.clone()),
    }
}

/// `VariableToDeclare`.
struct VariableToDeclare {
    /// The display class it belongs to (`container`).
    container: usize,
    field: ResolvedField,
    declared: Option<VarId>,
    can_propagate: bool,
    uses_initial_value: bool,
    initializers: Vec<NodeId>,
}

/// `DisplayClass`.
struct DisplayClass {
    variable: VarId,
    ty: TypeDefId,
    /// `VariablesToDeclare`, in insertion order.
    vars: Vec<(FieldKey, usize)>,
    capture_scope: Option<NodeId>,
    initializer: Option<NodeId>,
}

#[derive(Default)]
struct Tdcu {
    vtds: Vec<VariableToDeclare>,
    classes: Vec<DisplayClass>,
    /// `displayClasses`, in insertion order: the variable and its class.
    display_classes: Vec<(VarId, usize)>,
    /// `displayClassCopyMap`.
    copy_map: HashMap<VarId, VarId>,
    /// `currentFunctions`, innermost last (`None` is the top-level function).
    current: Vec<Option<NodeId>>,
    /// `decompilationContext.CurrentTypeDefinition`.
    decompiled: Option<TypeDefId>,
}

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let mut t = Tdcu {
        decompiled: declaring_type(ctx.ts, r.method),
        ..Tdcu::default()
    };
    t.analyze_function(r, ctx);
    t.transform(r, ctx);
}

impl Tdcu {
    fn display_class(&self, v: VarId) -> Option<usize> {
        self.display_classes
            .iter()
            .find(|&&(w, _)| w == v)
            .map(|&(_, c)| c)
    }

    fn var_of(&self, class: usize, key: &FieldKey) -> Option<usize> {
        self.classes[class]
            .vars
            .iter()
            .find(|(k, _)| k == key)
            .map(|&(_, i)| i)
    }

    fn set_var(&mut self, class: usize, key: FieldKey, vtd: usize) {
        let vars = &mut self.classes[class].vars;
        match vars.iter_mut().find(|(k, _)| *k == key) {
            Some(entry) => entry.1 = vtd,
            None => vars.push((key, vtd)),
        }
    }

    /// `VariableToDeclare.Propagate`.
    fn propagate(&mut self, vtd: usize, v: Option<VarId>) {
        self.vtds[vtd].declared = v;
        self.vtds[vtd].can_propagate = v.is_some();
    }

    /// `VariableToDeclare.GetOrDeclare`: the variable, registered in the display class
    /// variable's function on first use.
    fn get_or_declare(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        vtd: usize,
    ) -> VarId {
        if let Some(v) = self.vtds[vtd].declared {
            return v;
        }
        let class = &self.classes[self.vtds[vtd].container];
        let (variable, capture_scope) = (class.variable, class.capture_scope);
        let owner = variable_function(r, variable);
        let field = &self.vtds[vtd].field;
        let (ty, name) = (field.ty, field.name.clone());
        let v = register_variable(r, ctx, owner, VariableKind::Local, ty, &name);
        let var = r.function.var_mut(v);
        var.initial_value_is_initialized = true;
        var.uses_initial_value = self.vtds[vtd].uses_initial_value;
        var.capture_scope = capture_scope;
        self.vtds[vtd].declared = Some(v);
        v
    }

    /// `AnalyzeFunction`.
    fn analyze_function(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        // The functions in pre-order: the top-level one, then the nested ones by tree order.
        let mut functions: Vec<Option<NodeId>> = vec![None];
        let roots: Vec<NodeId> = r
            .function
            .body
            .into_iter()
            .chain(r.function.local_functions.iter().copied())
            .collect();
        for root in roots {
            pre_order_functions(&r.function, root, &mut functions);
        }
        for owner in functions {
            let variables = match owner {
                None => r.variables.clone(),
                Some(n) => function_variables(&r.function, n),
            };
            for v in variables {
                let Some(class) = self.analyze_variable(r, ctx, owner, v) else {
                    continue;
                };
                let variable = self.classes[class].variable;
                if self.display_class(variable).is_some() {
                    continue;
                }
                self.display_classes.push((variable, class));
            }
        }
        for (v, class) in self.display_classes.clone() {
            if !self.validate_display_class_uses(r, ctx, v, class) {
                self.display_classes.retain(|&(w, _)| w != v);
            }
        }
        for (_, class) in self.display_classes.clone() {
            // Fields never stored to.
            let def = self.classes[class].ty;
            for field in type_fields(ctx.ts, def) {
                let k = key(&field);
                if self.var_of(class, &k).is_some() {
                    continue;
                }
                let vtd = self.add_variable(r, ctx, class, None, field);
                self.vtds[vtd].uses_initial_value = true;
                self.set_var(class, k, vtd);
            }
            for (_, vtd) in self.classes[class].vars.clone() {
                if self.vtds[vtd].can_propagate {
                    let p = self.get_or_declare(r, ctx, vtd);
                    if r.function.var(p).kind != VariableKind::Parameter
                        && self.display_class(p).is_none()
                    {
                        self.propagate(vtd, None);
                    }
                }
            }
        }
    }

    /// `ValidateDisplayClassUses`.
    fn validate_display_class_uses(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        v: VarId,
        class: usize,
    ) -> bool {
        let var = r.function.var(v);
        let uses: Vec<NodeId> = var
            .load_instructions
            .iter()
            .chain(&var.address_instructions)
            .copied()
            .collect();
        uses.into_iter()
            .all(|u| self.validate_use(r, ctx, v, class, u))
    }

    /// `ValidateUse`.
    fn validate_use(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        v: VarId,
        class: usize,
        use_: NodeId,
    ) -> bool {
        let Some(parent) = r.function.node(use_).parent else {
            return false;
        };
        match r.function.inst(parent).clone() {
            Inst::LdFlda { target, field, .. } if target == use_ => {
                let k = key(&field.field);
                let vtd = match self.var_of(class, &k) {
                    Some(i) => i,
                    None => self.add_variable(r, ctx, class, None, field.field.clone()),
                };
                if self.vtds[vtd].can_propagate
                    && !self.is_plain_read_or_initializer_store(r, parent, vtd)
                {
                    // The field is changed after its initialization: the store can only go to the
                    // propagated variable for parameters, not for `this` or a display class.
                    let p = self.get_or_declare(r, ctx, vtd);
                    if is_this(&r.function, p) || self.display_class(p).is_some() {
                        self.propagate(vtd, None);
                    }
                }
                self.set_var(class, k, vtd);
                true
            }
            Inst::StObj { target, value, .. } if value == use_ => {
                if let Some((load, field)) = r.function.match_ldflda(target)
                    && let Some(other) = match_ld_loc_ref(&r.function, ctx, load)
                    && let Some(other_class) = self.display_class(other)
                    && let Some(vtd) = self.var_of(other_class, &key(&field.field))
                {
                    return self.vtds[vtd].can_propagate;
                }
                false
            }
            Inst::StLoc { var, value, .. }
                if r.function.var(var).is_single_definition() && value == use_ =>
            {
                self.copy_map.insert(var, v);
                self.validate_display_class_uses(r, ctx, var, class)
            }
            _ => false,
        }
    }

    /// `IsPlainReadOrInitializerStore`.
    fn is_plain_read_or_initializer_store(
        &self,
        r: &ReadFunction,
        ldflda: NodeId,
        vtd: usize,
    ) -> bool {
        let Some(parent) = r.function.node(ldflda).parent else {
            return false;
        };
        match *r.function.inst(parent) {
            Inst::LdObj { .. } => true,
            Inst::StObj { target, .. } if target == ldflda => {
                self.vtds[vtd].initializers.contains(&parent)
            }
            _ => false,
        }
    }

    /// `AnalyzeVariable`.
    fn analyze_variable(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        owner: Option<NodeId>,
        v: VarId,
    ) -> Option<usize> {
        match r.function.var(v).kind {
            VariableKind::Parameter => {
                if ctx.settings.yield_return
                    && compiled_with_mono(r, owner)
                    && is_this(&r.function, v)
                {
                    return self.handle_mono_state_machine(r, ctx, owner, v);
                }
                None
            }
            VariableKind::StackSlot | VariableKind::Local | VariableKind::DisplayClassLocal => {
                self.detect_display_class(r, ctx, owner, v)
            }
            VariableKind::InitializerTarget => self.detect_display_class_initializer(r, ctx, v),
            _ => None,
        }
    }

    /// `DetectDisplayClass`.
    fn detect_display_class(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        owner: Option<NodeId>,
        v: VarId,
    ) -> Option<usize> {
        let var = r.function.var(v);
        let definition = if var.kind != VariableKind::StackSlot {
            ctx.ts.definition(var.ty)
        } else if let Some(&store) = var.store_instructions.first()
            && let Some((_, value)) = r.function.match_stloc(store)
        {
            let ty = r.function.infer_type(ctx.ts, value);
            ctx.ts.definition(ty)
        } else {
            None
        };
        let definition = definition?;
        if !self.validate_display_class_definition(ctx, definition) {
            return None;
        }
        let class = match ctx.ts.comp.type_def(definition).ok()?.kind {
            TypeKind::Class => {
                let var = r.function.var(v);
                if !var.is_single_definition() {
                    return None;
                }
                let &[store] = &var.store_instructions[..] else {
                    return None;
                };
                let (_, value) = r.function.match_stloc(store)?;
                let Inst::NewObj(c) = r.function.inst(value) else {
                    return None;
                };
                let method = c.method.method.clone();
                if !validate_constructor(ctx.ts, r.method.module, &method) {
                    return None;
                }
                let capture_scope = r.function.var(v).capture_scope;
                let class = self.new_class(v, definition, capture_scope, Some(store));
                let block = r.function.node(store).parent;
                let index = r.function.node(store).child_index as usize + 1;
                self.handle_init_block(r, ctx, block, index, class, v);
                class
            }
            TypeKind::Struct => {
                if !r.function.var(v).store_instructions.is_empty() {
                    return None;
                }
                let capture_scope = r.function.var(v).capture_scope;
                let class = self.new_class(v, definition, capture_scope, None);
                let block = find_display_struct_init_block(r, owner, v);
                self.handle_init_block(r, ctx, block, 0, class, v);
                class
            }
            _ => return None,
        };
        if self.is_mono_nested_capture_scope(ctx, definition) {
            self.classes[class].capture_scope = None;
        }
        Some(class)
    }

    fn new_class(
        &mut self,
        variable: VarId,
        ty: TypeDefId,
        capture_scope: Option<NodeId>,
        initializer: Option<NodeId>,
    ) -> usize {
        self.classes.push(DisplayClass {
            variable,
            ty,
            vars: Vec::new(),
            capture_scope,
            initializer,
        });
        self.classes.len() - 1
    }

    /// `HandleInitBlock`.
    fn handle_init_block(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        block: Option<NodeId>,
        start: usize,
        class: usize,
        target: VarId,
    ) {
        let Some(block) = block else {
            return;
        };
        if !matches!(r.function.inst(block), Inst::Block { .. }) {
            return;
        }
        let mut i = start;
        while i < r.function.list(block).len() {
            let init = r.function.list(block)[i];
            let Some((address, field, _)) = r.function.match_stfld(init) else {
                break;
            };
            let field = field.field.clone();
            if match_ld_loc_ref(&r.function, ctx, address) != Some(target) {
                break;
            }
            let k = key(&field);
            if self.var_of(class, &k).is_some() {
                break;
            }
            let vtd = self.add_variable(r, ctx, class, Some(init), field);
            self.set_var(class, k, vtd);
            i += 1;
        }
    }

    /// `DetectDisplayClassInitializer`: `stloc r(block ObjectInitializer { stloc v(newobj); ... })`.
    fn detect_display_class_initializer(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        v: VarId,
    ) -> Option<usize> {
        let f = &r.function;
        let &[store] = &f.var(v).store_instructions[..] else {
            return None;
        };
        let (_, value) = f.match_stloc(store)?;
        let block = f.node(store).parent?;
        if !matches!(
            f.inst(block),
            Inst::Block {
                kind: BlockKind::ObjectInitializer,
                ..
            }
        ) {
            return None;
        }
        let Inst::NewObj(c) = f.inst(value) else {
            return None;
        };
        let method = c.method.method.clone();
        let definition = method.declaring.and_then(|d| ctx.ts.definition(d))?;
        if !self.validate_display_class_definition(ctx, definition)
            || !validate_constructor(ctx.ts, r.method.module, &method)
        {
            return None;
        }
        let parent = r.function.node(block).parent?;
        let (reference, stored) = r.function.match_stloc(parent)?;
        if stored != block {
            return None;
        }
        let reference_var = r.function.var(reference);
        if !reference_var.is_single_definition() {
            return None;
        }
        let &[reference_store] = &reference_var.store_instructions[..] else {
            return None;
        };
        r.function.match_stloc(reference_store)?;
        let capture_scope = reference_var.capture_scope;
        let class = self.new_class(reference, definition, capture_scope, Some(parent));
        self.handle_init_block(r, ctx, Some(block), 1, class, v);
        Some(class)
    }

    /// `ValidateDisplayClassDefinition`.
    fn validate_display_class_definition(
        &self,
        ctx: &mut Context<'_, '_, '_>,
        definition: TypeDefId,
    ) -> bool {
        if definition.module != ModuleId(0) {
            return false;
        }
        // State machines are not to be destroyed.
        let md = &ctx.ts.comp.module(definition.module).md;
        if is_compiler_generated_enumerator(md, definition.row)
            || is_compiler_generated_state_machine(md, definition.row)
        {
            return false;
        }
        if !ctx.settings.aggressive_scalar_replacement_of_aggregates {
            if ctx.ts.comp.type_def(definition).map_or(0, |i| i.declaring) == 0 {
                return false;
            }
            if !ctx
                .ts
                .is_potential_closure_type(self.decompiled, definition)
            {
                return false;
            }
        }
        true
    }

    /// `AddVariable`.
    fn add_variable(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        class: usize,
        statement: Option<NodeId>,
        field: ResolvedField,
    ) -> usize {
        self.vtds.push(VariableToDeclare {
            container: class,
            field: field.clone(),
            declared: None,
            can_propagate: false,
            uses_initial_value: false,
            initializers: Vec::new(),
        });
        let vtd = self.vtds.len() - 1;
        if let Some(statement) = statement
            && let Inst::StObj { value, .. } = *r.function.inst(statement)
        {
            let v = self.resolve_variable_to_propagate(r, ctx, value, Some(field.ty));
            self.propagate(vtd, v);
            self.vtds[vtd].initializers.push(statement);
        }
        let ty = ctx.ts.intern_def(self.classes[class].ty);
        let variable = self.classes[class].variable;
        self.vtds[vtd].uses_initial_value = ctx.ts.is_reference_type(ty) != Some(false)
            || r.function.var(variable).uses_initial_value;
        vtd
    }

    /// `ResolveVariableToPropagate`: the variable a field value can be replaced by, if any.
    fn resolve_variable_to_propagate(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        value: NodeId,
        expected: Option<TypeId>,
    ) -> Option<VarId> {
        match *r.function.inst(value) {
            Inst::LdLoc { var: v } => {
                let var = r.function.var(v);
                if var.kind == VariableKind::Parameter {
                    // A parameter used elsewhere cannot replace the field.
                    if var.load_count() != 1 && !is_this(&r.function, v) {
                        return None;
                    }
                } else if ctx.ts.is_reference_type(var.ty) != Some(true) {
                    // Display structs (as local functions use them) are not propagated.
                    return None;
                }
                let var = r.function.var(v);
                if !var.is_single_definition() {
                    return None;
                }
                let ok = match expected {
                    None => true,
                    Some(_) if var.kind == VariableKind::StackSlot => true,
                    Some(e) => equivalent_ignoring_nullability(ctx.ts, var.ty, e),
                };
                ok.then_some(v)
            }
            Inst::LdObj { target, .. } => {
                let mut current: Option<usize> = None;
                for item in r.function.descendants_post(target) {
                    if let Some(v) = self.is_display_class_load(&r.function, item) {
                        current = Some(self.display_class(v)?);
                    }
                    let class = current?;
                    if let Inst::LdFlda { field, .. } = r.function.inst(item)
                        && let Some(vtd) = self.var_of(class, &key(&field.field))
                    {
                        if !self.vtds[vtd].can_propagate {
                            return None;
                        }
                        let declared = self.get_or_declare(r, ctx, vtd);
                        current = Some(self.display_class(declared)?);
                    }
                }
                current.map(|c| self.classes[c].variable)
            }
            _ => None,
        }
    }

    /// `IsMonoNestedCaptureScope`.
    fn is_mono_nested_capture_scope(
        &self,
        ctx: &mut Context<'_, '_, '_>,
        definition: TypeDefId,
    ) -> bool {
        let md = &ctx.ts.comp.module(definition.module).md;
        let named = md
            .get::<rows::TypeDef>(definition.row)
            .ok()
            .and_then(|t| md.string(t.name).ok())
            .is_some_and(|n| n.contains("AnonStorey"));
        named
            && type_fields(ctx.ts, definition).into_iter().any(|f| {
                ctx.ts
                    .definition(f.ty)
                    .is_some_and(|d| ctx.ts.is_potential_closure_type(self.decompiled, d))
            })
    }

    /// `HandleMonoStateMachine`: mcs moves the captured variables of yield state machines into
    /// the state machine type; a display class spanning the whole body stands for it.
    fn handle_mono_state_machine(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        owner: Option<NodeId>,
        this_variable: VarId,
    ) -> Option<usize> {
        let closure = ctx.ts.definition(r.function.var(this_variable).ty)?;
        if Some(closure) == self.decompiled
            || !is_potential_closure_allowing_interfaces(ctx.ts, self.decompiled, closure)
        {
            return None;
        }
        let body = match owner {
            None => r.function.body,
            Some(n) => match r.function.inst(n) {
                Inst::Function { body, .. } => Some(*body),
                _ => None,
            },
        };
        let class = self.new_class(this_variable, closure, body, None);
        let variables = match owner {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        for v in variables {
            let Some(field) = r.function.var(v).state_machine_field.clone() else {
                continue;
            };
            let k = key(&field.field);
            if self.var_of(class, &k).is_some() {
                continue;
            }
            self.vtds.push(VariableToDeclare {
                container: class,
                field: field.field,
                declared: Some(v),
                can_propagate: false,
                uses_initial_value: false,
                initializers: Vec::new(),
            });
            let vtd = self.vtds.len() - 1;
            self.classes[class].vars.push((k, vtd));
        }
        let method_is_static = ctx
            .ts
            .method_definition(function_method(r, owner))
            .is_ok_and(|m| m.is_static);
        if method_is_static {
            return Some(class);
        }
        // `FindThisField`: an instance field of the decompiled type.
        let decompiled = self.decompiled?;
        let this_field = type_fields(ctx.ts, closure).into_iter().find(|f| {
            !is_static_field(ctx.ts, f)
                && self.var_of(class, &key(f)).is_none()
                && ctx.ts.definition(f.ty) == Some(decompiled)
        });
        let Some(this_field) = this_field else {
            return Some(class);
        };
        let variables = match owner {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        let existing = variables.into_iter().find(|&v| {
            is_this(&r.function, v) && ctx.ts.definition(r.function.var(v).ty) == Some(decompiled)
        });
        let this_var = match existing {
            Some(v) => {
                if let Some(field) = &r.function.var(v).state_machine_field
                    && self.var_of(class, &key(&field.field)).is_some()
                {
                    // `this` was added before.
                    return Some(class);
                }
                v
            }
            None => {
                let ty = ctx.ts.intern_def(decompiled);
                let stack_type = ctx.ts.stack_type(ty);
                let token = match this_field.origin {
                    FieldOrigin::Def { row, .. } => 0x0400_0000 | row,
                    FieldOrigin::Fake => 0,
                };
                let v = r.function.add_var(Variable {
                    kind: VariableKind::Parameter,
                    ty,
                    stack_type,
                    index: Some(-1),
                    name: "this".to_owned(),
                    has_generated_name: false,
                    initial_value_is_initialized: false,
                    uses_initial_value: true,
                    is_ref_read_only: false,
                    remove_if_redundant: false,
                    state_machine_field: Some(FieldOperand {
                        token,
                        field: this_field.clone(),
                        context: 0,
                    }),
                    load_instructions: Vec::new(),
                    store_instructions: Vec::new(),
                    address_instructions: Vec::new(),
                    capture_scope: None,
                    is_scoped: false,
                    is_scoped_without_initializer: false,
                });
                match owner {
                    None => r.variables.push(v),
                    Some(n) => {
                        if let Inst::Function { variables, .. } = r.function.inst_mut(n) {
                            variables.push(v);
                        }
                    }
                }
                v
            }
        };
        self.vtds.push(VariableToDeclare {
            container: class,
            field: this_field.clone(),
            declared: Some(this_var),
            can_propagate: false,
            uses_initial_value: false,
            initializers: Vec::new(),
        });
        let vtd = self.vtds.len() - 1;
        self.classes[class].vars.push((key(&this_field), vtd));
        Some(class)
    }

    /// `IsDisplayClassLoad`: `ldloc v` or `ldloca v`, through the copy map (local functions take
    /// display structs by reference, so `MatchLdLocRef` would not do).
    fn is_display_class_load(&self, f: &Function, target: NodeId) -> Option<VarId> {
        let (Inst::LdLoc { var } | Inst::LdLoca { var }) = *f.inst(target) else {
            return None;
        };
        Some(self.copy_map.get(&var).copied().unwrap_or(var))
    }

    /// `IsDisplayClassFieldAccess`: the display class and field of `ldflda field(load)`.
    fn display_class_field_access(
        &self,
        f: &Function,
        inst: NodeId,
    ) -> Option<(VarId, usize, ResolvedField)> {
        let Inst::LdFlda { target, field, .. } = f.inst(inst) else {
            return None;
        };
        let v = self.is_display_class_load(f, *target)?;
        Some((v, self.display_class(v)?, field.field.clone()))
    }

    /// `Transform`.
    fn transform(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        self.current.push(None);
        let roots: Vec<NodeId> = r
            .function
            .body
            .into_iter()
            .chain(r.function.local_functions.iter().copied())
            .collect();
        self.visit_sequence(r, ctx, roots);
        self.current.pop();
        // The top-level function's new variables join its list (`RegisterVariable` adds
        // them right away) before the flags are reset.
        r.flush_pending_variables();
        // `ResetHasInitialValueFlag`, for every function, the top-level one last.
        let mut functions: Vec<Option<NodeId>> =
            all_functions(&r.function).into_iter().map(Some).collect();
        functions.push(None);
        for owner in functions {
            let variables = match owner {
                None => r.variables.clone(),
                Some(n) => function_variables(&r.function, n),
            };
            reset_uses_initial_value_flag_of(&mut r.function, ctx, &variables, owner);
            let f = &mut r.function;
            let dead: Vec<VarId> = f
                .captured_variables
                .iter()
                .filter(|&&(o, v)| o == owner && f.var(v).is_dead())
                .map(|&(_, v)| v)
                .collect();
            f.captured_variables
                .retain(|&(o, v)| !(o == owner && dead.contains(&v)));
        }
    }

    /// The managed `Default` over a fixed list (the top-level function's children).
    fn visit_sequence(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        nodes: Vec<NodeId>,
    ) {
        for n in nodes {
            self.visit(r, ctx, n);
        }
    }

    /// `Default`: the children in turn, each next sibling taken before visiting the child.
    fn visit_children(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        inst: NodeId,
    ) {
        let mut child = r.function.inst(inst).children().first().copied();
        while let Some(c) = child {
            let next = next_sibling(&r.function, c);
            self.visit(r, ctx, c);
            child = next;
        }
    }

    fn visit(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
        match r.function.inst(inst) {
            Inst::Function { .. } => {
                self.current.push(Some(inst));
                self.visit_children(r, ctx, inst);
                self.current.pop();
            }
            Inst::StLoc { .. } => self.visit_stloc(r, ctx, inst),
            Inst::StObj { .. } => self.visit_stobj(r, ctx, inst),
            Inst::LdObj { .. } => {
                self.visit_children(r, ctx, inst);
                ldobj_to_ldloc(&mut r.function, ctx.ts, inst);
            }
            Inst::LdFlda { .. } => self.visit_ldflda(r, ctx, inst),
            _ => self.visit_children(r, ctx, inst),
        }
    }

    /// `VisitStLoc`.
    fn visit_stloc(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
        let (var, value) = r.function.match_stloc(inst).expect("stloc");
        let parent_block = r
            .function
            .node(inst)
            .parent
            .filter(|&p| matches!(r.function.inst(p), Inst::Block { .. }));
        if let Some(block) = parent_block
            && r.function.var(var).is_single_definition()
        {
            let v = r.function.var(var);
            if matches!(v.kind, VariableKind::Local | VariableKind::StackSlot)
                && v.load_count() == 0
            {
                // Pre-order, so later cases need not deal with it.
                self.visit_children(r, ctx, inst);
                let value = r.function.match_stloc(inst).map(|(_, value)| value);
                if let Some(value) = value
                    && matches!(
                        r.function.inst(value),
                        Inst::StLoc { .. }
                            | Inst::NumericCompoundAssign { .. }
                            | Inst::UserDefinedCompoundAssign { .. }
                            | Inst::DynamicCompoundAssign { .. }
                    )
                {
                    r.function.replace(inst, value);
                }
                return;
            }
            if let Some(class) = self.display_class(var)
                && self.classes[class].initializer == Some(inst)
            {
                // The contents of an object initializer block move after it.
                if let Inst::Block {
                    kind: BlockKind::ObjectInitializer,
                    ..
                } = r.function.inst(value)
                {
                    let mut insertion = r.function.node(inst).child_index as usize;
                    let stores: Vec<NodeId> = r.function.list(value)[1..].to_vec();
                    for stobj in stores {
                        let Inst::StObj {
                            target,
                            value: stored,
                            ..
                        } = *r.function.inst(stobj)
                        else {
                            continue;
                        };
                        let Inst::LdFlda { field, .. } = r.function.inst(target) else {
                            continue;
                        };
                        let Some(vtd) = self.var_of(class, &key(&field.field)) else {
                            continue;
                        };
                        // A propagated field would be assigned to itself.
                        if self.vtds[vtd].can_propagate
                            && self.vtds[vtd].initializers.contains(&stobj)
                        {
                            continue;
                        }
                        let declared = self.get_or_declare(r, ctx, vtd);
                        let range = r.function.il_range(stobj);
                        let store = r.function.add(Inst::StLoc {
                            var: declared,
                            value: stored,
                            il_stack_was_empty: false,
                            is_stack_adjustment: false,
                        });
                        r.function.set_il_range(store, range);
                        insertion += 1;
                        r.function.insert(block, insertion, store);
                    }
                }
                r.function.remove(inst);
                return;
            }
            if matches!(
                r.function.inst(value),
                Inst::LdLoc { .. } | Inst::LdObj { .. }
            ) {
                // A display class reference in a stack slot (when inlining failed): the loads of
                // the slot become loads of the display class.
                let referenced = match self.copy_map.get(&var) {
                    Some(&d) => Some(d),
                    None => self.resolve_variable_to_propagate(r, ctx, value, None),
                };
                if let Some(referenced) = referenced
                    && self.display_class(referenced).is_some()
                {
                    for load in r.function.var(var).load_instructions.clone() {
                        let range = r.function.il_range(load);
                        let new_load = r.function.add(Inst::LdLoc { var: referenced });
                        r.function.set_il_range(new_load, range);
                        r.function.replace(load, new_load);
                    }
                    r.function.remove(inst);
                    return;
                }
            }
        }
        self.visit_children(r, ctx, inst);
    }

    /// `VisitStObj`.
    fn visit_stobj(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
        let Inst::StObj { target, value, .. } = *r.function.inst(inst) else {
            unreachable!("stobj");
        };
        if let Some((_, class, field)) = self.display_class_field_access(&r.function, target) {
            let vtd = self
                .var_of(class, &key(&field))
                .expect("every field of a display class has a variable");
            if self.vtds[vtd].can_propagate
                && self.vtds[vtd].initializers.contains(&inst)
                && r.function
                    .node(inst)
                    .parent
                    .is_some_and(|p| matches!(r.function.inst(p), Inst::Block { .. }))
            {
                r.function.remove(inst);
                return;
            }
            if let Inst::LdLoc { var } = *r.function.inst(value)
                && r.function.var(var).is_single_definition()
                && r.function.var(var).capture_scope.is_none()
                && let Some(&store) = r.function.var(var).store_instructions.first()
                && r.function.match_stloc(store).is_some()
                && let Some(block) = r.function.node(store).parent
                && matches!(r.function.inst(block), Inst::Block { .. })
            {
                let index = r.function.node(store).child_index as usize;
                inline_one_if_possible(&mut r.function, block, index, options::NONE, ctx);
            }
        }
        self.visit_children(r, ctx, inst);
        stobj_to_stloc(&mut r.function, ctx.ts, inst);
    }

    /// `VisitLdFlda`: a display class field access becomes `ldloca` of its variable, which is
    /// captured by every function from the use to the one declaring it.
    fn visit_ldflda(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
        self.visit_children(r, ctx, inst);
        let Some((_, class, field)) = self.display_class_field_access(&r.function, inst) else {
            return;
        };
        let vtd = self
            .var_of(class, &key(&field))
            .expect("every field of a display class has a variable");
        let variable = self.get_or_declare(r, ctx, vtd);
        let range = r.function.il_range(inst);
        let replacement = r.function.add(Inst::LdLoca { var: variable });
        r.function.set_il_range(replacement, range);
        r.function.replace(inst, replacement);
        let owner = variable_function(r, variable);
        for &f in self.current.iter().rev() {
            if f == owner {
                break;
            }
            r.function.add_captured_variable(f, variable);
        }
    }
}

/// The `Function` instructions under `root` in pre-order (the analysis order).
fn pre_order_functions(f: &Function, root: NodeId, out: &mut Vec<Option<NodeId>>) {
    let mut stack = vec![root];
    while let Some(n) = stack.pop() {
        if matches!(f.inst(n), Inst::Function { .. }) {
            out.push(Some(n));
        }
        stack.extend(f.inst(n).children().into_iter().rev());
    }
}

/// `GetNextSibling`.
fn next_sibling(f: &Function, inst: NodeId) -> Option<NodeId> {
    let parent = f.node(inst).parent?;
    let index = f.node(inst).child_index as usize;
    f.inst(parent).children().get(index + 1).copied()
}

/// `FindDisplayStructInitBlock`: the innermost block holding all uses of `v` in its function.
fn find_display_struct_init_block(
    r: &ReadFunction,
    owner: Option<NodeId>,
    v: VarId,
) -> Option<NodeId> {
    let f = &r.function;
    let root = match owner {
        None => f.body?,
        Some(n) => match f.inst(n) {
            Inst::Function { body, .. } => *body,
            _ => return None,
        },
    };
    fn visit(f: &Function, v: VarId, inst: NodeId) -> Option<NodeId> {
        match *f.inst(inst) {
            Inst::LdLoc { var } | Inst::StLoc { var, .. } | Inst::LdLoca { var } if var == v => {
                Some(inst)
            }
            _ => {
                let mut result = None;
                for child in f.inst(inst).children() {
                    let found = visit(f, v, child);
                    match (result, found) {
                        (None, found) => result = found,
                        // A second use in this subtree: the subtree itself.
                        (Some(_), Some(_)) => return Some(inst),
                        (Some(_), None) => {}
                    }
                }
                result
            }
        }
    }
    let common = visit(f, v, root)?;
    std::iter::once(common)
        .chain(f.ancestors(common))
        .find(|&n| matches!(f.inst(n), Inst::Block { .. }))
}

/// `function.StateMachineCompiledWithMono`.
fn compiled_with_mono(r: &ReadFunction, owner: Option<NodeId>) -> bool {
    match owner.map(|n| r.function.inst(n)) {
        None => r.state_machine_compiled_with_mono,
        Some(Inst::Function {
            read: Some(read), ..
        }) => read.state_machine_compiled_with_mono,
        Some(_) => false,
    }
}

fn function_method(
    r: &ReadFunction,
    owner: Option<NodeId>,
) -> crate::typesystem::members::MethodDefId {
    match owner.map(|n| r.function.inst(n)) {
        Some(Inst::Function {
            read: Some(read), ..
        }) => read.method,
        _ => r.method,
    }
}

/// `function.RegisterVariable(kind, type, name)` in the function `owner` (`None` for the
/// top-level one).
fn register_variable(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    owner: Option<NodeId>,
    kind: VariableKind,
    ty: TypeId,
    name: &str,
) -> VarId {
    let name = (!name.trim().is_empty()).then_some(name);
    r.function
        .register_variable_in(ctx.ts, owner, kind, ty, name)
}

/// `ITypeDefinition.Fields`, as fields of the definition.
fn type_fields(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> Vec<ResolvedField> {
    let md = &ts.comp.module(def.module).md;
    let Ok(rows) = md.type_fields(def.row) else {
        return Vec::new();
    };
    let declaring = ts.intern_def(def);
    rows.into_iter()
        .filter_map(|row| {
            let md = &ts.comp.module(def.module).md;
            let name = md
                .string(md.get::<rows::Field>(row).ok()?.name)
                .ok()?
                .into_owned();
            let (ty, _) = ts.field_type(def.module, row);
            Some(ResolvedField {
                origin: FieldOrigin::Def {
                    module: def.module,
                    row,
                },
                declaring: Some(declaring),
                name,
                ty,
            })
        })
        .collect()
}

fn is_static_field(ts: &TypeSystem<'_, '_>, field: &ResolvedField) -> bool {
    let FieldOrigin::Def { module, row } = field.origin else {
        return false;
    };
    let md = &ts.comp.module(module).md;
    md.get::<rows::Field>(row)
        .is_ok_and(|f| f.flags & 0x0010 != 0)
}

/// `IsPotentialClosure(..., allowTypeImplementingInterfaces: true)`.
fn is_potential_closure_allowing_interfaces(
    ts: &mut TypeSystem<'_, '_>,
    decompiled: Option<TypeDefId>,
    display: TypeDefId,
) -> bool {
    if !matches!(
        ts.comp.type_def(display).map(|i| i.kind),
        Ok(TypeKind::Struct | TypeKind::Class)
    ) {
        return false;
    }
    ts.is_potential_closure(decompiled, display)
        .unwrap_or(false)
}

/// `NormalizeTypeVisitor.IgnoreNullability.EquivalentTypes`.
fn equivalent_ignoring_nullability(ts: &mut TypeSystem<'_, '_>, a: TypeId, b: TypeId) -> bool {
    strip_nullability(ts, a) == strip_nullability(ts, b)
}

/// The type with every nullability annotation removed.
fn strip_nullability(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> TypeId {
    let data = ts.data(t).clone();
    let stripped = match data {
        TypeData::Annotated { inner, .. } => return strip_nullability(ts, inner),
        TypeData::Inst { generic, args } => TypeData::Inst {
            generic: strip_nullability(ts, generic),
            args: args.iter().map(|&a| strip_nullability(ts, a)).collect(),
        },
        TypeData::Array { elem, rank, .. } => TypeData::Array {
            elem: strip_nullability(ts, elem),
            rank,
            nullability: crate::typesystem::types::Nullability::Oblivious,
        },
        TypeData::Pointer(e) => TypeData::Pointer(strip_nullability(ts, e)),
        TypeData::ByRef(e) => TypeData::ByRef(strip_nullability(ts, e)),
        TypeData::Tuple { .. } => return t,
        other => other,
    };
    ts.intern(stripped)
}

/// `YieldReturnDecompiler.IsCompilerGeneratorEnumerator`.
fn is_compiler_generated_enumerator(md: &crate::metadata::Metadata<'_>, type_def: u32) -> bool {
    implements_compiler_generated(md, type_def, "System.Collections", "IEnumerator")
}

/// `AsyncAwaitDecompiler.IsCompilerGeneratedStateMachine`.
fn is_compiler_generated_state_machine(md: &crate::metadata::Metadata<'_>, type_def: u32) -> bool {
    implements_compiler_generated(
        md,
        type_def,
        "System.Runtime.CompilerServices",
        "IAsyncStateMachine",
    )
}

/// A nested type that is, or whose declaring type is, compiler-generated and that implements
/// the non-generic interface `ns.name`.
fn implements_compiler_generated(
    md: &crate::metadata::Metadata<'_>,
    type_def: u32,
    ns: &str,
    name: &str,
) -> bool {
    use crate::il::control_flow::yield_return_decompiler::{
        interface_name_is, is_compiler_generated,
    };
    if type_def == 0 {
        return false;
    }
    let declaring = md.declaring_type(type_def).unwrap_or(0);
    if declaring == 0
        || !(is_compiler_generated(md, type_def) || is_compiler_generated(md, declaring))
    {
        return false;
    }
    md.type_interface_impls(type_def)
        .any(|ii| interface_name_is(md, ii, ns, name))
}

/// `ValidateConstructor`: `ldarg.0; call object::.ctor(); ret`, or a one-parameter copy
/// constructor of the display class (`if (arg1 != null) { this.f = arg1.f; ... }`).
pub(crate) fn validate_constructor(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    method: &ResolvedMethod,
) -> bool {
    use crate::il::control_flow::yield_return_decompiler::IlCursor;
    const OP_CALL: u16 = 0x28;
    const OP_RET: u16 = 0x2A;
    const OP_LDFLD: u16 = 0x7B;
    const OP_STFLD: u16 = 0x7D;
    const OP_BRFALSE: u16 = 0x39;
    const OP_BRFALSE_S: u16 = 0x2C;
    let Ok(params) = method.params() else {
        return false;
    };
    if params.len() > 1 {
        return false;
    }
    let MethodOrigin::Def(d) = method.origin else {
        return false;
    };
    if d.module != module {
        return false;
    }
    let md = &ts.comp.module(module).md;
    let Ok(def) = md.get::<rows::MethodDef>(d.row) else {
        return false;
    };
    if def.rva == 0 {
        return false;
    }
    let Ok(body) = md.method_body(def.rva) else {
        return false;
    };
    if !body.clauses.is_empty() || body.code.len() < 7 {
        return false;
    }
    let mut c = IlCursor::new(body.code);
    let mut check = || -> Option<bool> {
        if !c.ldarg(0)? || c.op_skip_nop()? != OP_CALL {
            return Some(false);
        }
        let token = c.i32()? as u32;
        if token & 0x00FF_FFFF == 0 {
            return Some(false);
        }
        let Ok(base) = ts.resolve_method(module, token, GenericContext::default()) else {
            return Some(false);
        };
        let declaring_is_object = base
            .declaring
            .and_then(|t| ts.definition(t))
            .and_then(|d| ts.comp.type_def(d).ok())
            .is_some_and(|i| i.known == KnownTypeCode::Object);
        if !declaring_is_object {
            return Some(false);
        }
        if !matches!(base.name.as_str(), ".ctor" | ".cctor")
            || !base.params().is_ok_and(|p| p.is_empty())
        {
            return Some(false);
        }
        if params.is_empty() {
            return Some(c.op_skip_nop()? == OP_RET);
        }
        let declaring_def = method.declaring.and_then(|t| ts.definition(t));
        if ts.definition(params[0]) != declaring_def || declaring_def.is_none() {
            return Some(false);
        }
        if !c.ldarg(1)? {
            return Some(false);
        }
        match c.op_skip_nop()? {
            OP_BRFALSE => {
                c.i32()?;
            }
            OP_BRFALSE_S => {
                c.u8()?;
            }
            _ => return Some(false),
        }
        let mut copied_field = false;
        loop {
            let op = c.op_skip_nop()?;
            if op == OP_RET {
                return Some(copied_field);
            }
            if !c.ldarg_op(op, 0)? || !c.ldarg(1)? || c.op_skip_nop()? != OP_LDFLD {
                return Some(false);
            }
            let source = c.i32()? as u32;
            if c.op_skip_nop()? != OP_STFLD {
                return Some(false);
            }
            let target = c.i32()? as u32;
            if source & 0x00FF_FFFF == 0 || source != target {
                return Some(false);
            }
            copied_field = true;
        }
    };
    check().unwrap_or(false)
}
