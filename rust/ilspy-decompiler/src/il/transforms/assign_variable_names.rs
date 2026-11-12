//! `IL/Transforms/AssignVariableNames.cs`: every variable gets a valid, unique C# name, taken
//! from the debug symbols when they have a usable one, otherwise generated from how the
//! variable is used (a parameter it is passed to, a field or property it is stored from) or
//! from its type; loop counters become `i`, `j`, `k`. Local functions are renamed too.
//!
//! Each function gets a scope below its parent's; the root scope also avoids the lower-case
//! member and type names around the method, which the host lists (`RootNames`).

use std::collections::{HashMap, HashSet, VecDeque};

use crate::il::ast::{ContainerKind, Function, FunctionKind, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::metadata::tables::TableId;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::MethodDefId;
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin, ResolvedField, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId, TypeParamOwner, TypeSystem};

use super::Context;
use super::il_inlining::known;
use super::local_function_decompiler::{all_functions, function_variables, local_function_name};
use super::transform_collection_and_object_initializers::is_anonymous_type;

const MAX_LOOP_VARIABLE_NAME: char = 'n';

/// `MethodSemanticsAttributes`.
const SETTER: u16 = 0x0001;
const ADD_ON: u16 = 0x0008;
const REMOVE_ON: u16 = 0x0010;

/// `VariableScope`.
struct Scope {
    parent: Option<usize>,
    /// The function (`None` for the top-level one).
    function: Option<NodeId>,
    local_functions: Vec<(MethodDefId, String)>,
    /// `variableMapping`, compared with `ILVariableEqualityComparer`.
    mapping: Vec<(VarId, String)>,
    assigned_local_signature_indices: Vec<Option<String>>,
    reserved: HashMap<String, i32>,
    loop_counters: HashSet<VarId>,
    num_display_class_locals: i32,
}

struct Namer {
    scopes: Vec<Scope>,
    /// `currentLowerCaseTypeOrMemberNames`, shared by all scopes.
    lower_case_names: HashSet<String>,
    /// `ILVariable.Function` of every variable.
    owners: HashMap<VarId, Option<NodeId>>,
    work_list: VecDeque<(Option<NodeId>, Option<usize>)>,
}

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let mut owners: HashMap<VarId, Option<NodeId>> =
        r.variables.iter().map(|&v| (v, None)).collect();
    for function in all_functions(&r.function) {
        for v in function_variables(&r.function, function) {
            owners.insert(v, Some(function));
        }
    }
    let mut namer = Namer {
        scopes: Vec::new(),
        lower_case_names: HashSet::new(),
        owners,
        work_list: VecDeque::from([(None, None)]),
    };
    while let Some((function, parent)) = namer.work_list.pop_front() {
        let scope = namer.new_scope(r, ctx, function, parent);
        let local_functions = match function {
            None => r.function.local_functions.clone(),
            Some(n) => match r.function.inst(n) {
                Inst::Function {
                    local_functions, ..
                } => local_functions.clone(),
                _ => Vec::new(),
            },
        };
        for lf in local_functions {
            namer.assign_name_to_local_function(r, ctx, lf, scope);
            namer.work_list.push_back((Some(lf), Some(scope)));
        }
        let body = match function {
            None => r.function.body,
            Some(n) => match r.function.inst(n) {
                Inst::Function { body, .. } => Some(*body),
                _ => None,
            },
        };
        if let Some(body) = body {
            namer.visit(r, ctx, body, scope);
        }
        if function.is_some() {
            let variables = match function {
                Some(n) => function_variables(&r.function, n),
                None => Vec::new(),
            };
            for p in variables {
                let var = r.function.var(p);
                if var.kind == VariableKind::Parameter && var.index.is_some_and(|i| i >= 0) {
                    let name = namer.assign_name_if_unassigned(r, ctx, scope, p);
                    r.function.var_mut(p).name = name;
                }
            }
        }
    }
}

impl Namer {
    fn function_kind(&self, r: &ReadFunction, function: Option<NodeId>) -> FunctionKind {
        match function.map(|n| r.function.inst(n)) {
            Some(Inst::Function { kind, .. }) => *kind,
            _ => FunctionKind::TopLevelFunction,
        }
    }

    /// The `VariableScope` constructor.
    fn new_scope(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        function: Option<NodeId>,
        parent: Option<usize>,
    ) -> usize {
        let local_signature_length = match function.map(|n| r.function.inst(n)) {
            None => r.local_signature_length,
            Some(Inst::Function {
                read: Some(read), ..
            }) => read.local_signature_length,
            Some(_) => 0,
        };
        let loop_counters = find_loop_counters(&r.function, function);
        self.scopes.push(Scope {
            parent,
            function,
            local_functions: Vec::new(),
            mapping: Vec::new(),
            assigned_local_signature_indices: vec![None; local_signature_length],
            reserved: HashMap::new(),
            loop_counters,
            num_display_class_locals: 0,
        });
        let scope = self.scopes.len() - 1;
        let variables = match function {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        let parameters = function_parameters(r, ctx, function);
        match parent {
            None => self.init_root(r, ctx, scope, &variables, &parameters),
            Some(parent) => {
                let kind = self.function_kind(r, function);
                for (i, (_, name)) in parameters.iter().enumerate() {
                    let variable = variables.iter().copied().find(|&v| {
                        let var = r.function.var(v);
                        var.kind == VariableKind::Parameter && var.index == Some(i as i32)
                    });
                    let name = if name.trim().is_empty() {
                        r.function
                            .var(variable.expect("a parameter variable for an unnamed parameter"))
                            .name
                            .clone()
                    } else {
                        name.clone()
                    };
                    if matches!(kind, FunctionKind::Delegate | FunctionKind::ExpressionTree)
                        && is_transparent_identifier(&name)
                    {
                        add_existing_name(&mut self.scopes[scope].reserved, &name);
                        if let Some(v) = variable {
                            self.scopes[scope].mapping.push((v, name));
                        }
                    } else if !is_valid_name(&name) {
                        // A compiler-generated name (`<p0>`) is left to AssignName.
                        continue;
                    } else {
                        let (without_number, _) = split_name(&name);
                        if !self.is_reserved_variable_name(parent, &without_number).0 {
                            add_existing_name(&mut self.scopes[scope].reserved, &name);
                            if let Some(v) = variable {
                                self.scopes[scope].mapping.push((v, name));
                            }
                        } else if let Some(v) = variable {
                            r.function.var_mut(v).has_generated_name = true;
                        }
                    }
                }
            }
        }
        scope
    }

    /// The root scope's part of the constructor: the names around the method and the
    /// parameter names.
    fn init_root(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        scope: usize,
        variables: &[VarId],
        parameters: &[(TypeId, String)],
    ) {
        if let Some(names) = ctx.root_names {
            for name in &names.member_names {
                self.lower_case_names.insert(name.clone());
            }
            for name in &names.reserved_names {
                self.lower_case_names.insert(name.clone());
                add_existing_name(&mut self.scopes[scope].reserved, name);
            }
        }
        let method = r.method;
        let accessor = ctx.ts.accessor_owner(method);
        // `field` is a keyword in C# 14 property accessors; a local of that name would shadow
        // the backing field.
        if ctx.settings.field_keyword
            && let Some((owner, _)) = accessor
            && owner >> 24 == 0x17
            && property_parameter_count(ctx.ts, method, accessor) == 0
        {
            add_existing_name(&mut self.scopes[scope].reserved, "field");
        }
        let semantics = accessor.map_or(0, |(_, s)| s);
        // Setters, adders and removers have an implicit last parameter.
        let is_set_or_event_accessor = matches!(semantics, SETTER | ADD_ON | REMOVE_ON);
        if is_set_or_event_accessor && !parameters.is_empty() {
            for (_, name) in &parameters[..parameters.len() - 1] {
                add_existing_name(&mut self.scopes[scope].reserved, name);
            }
            let (_, last_name) = parameters.last().expect("checked above").clone();
            let last_index = parameters.len() as i32 - 1;
            let last_variable = variables.iter().copied().find(|&v| {
                let var = r.function.var(v);
                var.kind == VariableKind::Parameter && var.index == Some(last_index)
            });
            let owner = accessor.map_or(0, |(o, _)| o);
            let is_property = owner >> 24 == 0x17;
            let is_event = owner >> 24 == 0x14;
            if is_property && semantics == SETTER {
                if parameters[..parameters.len() - 1]
                    .iter()
                    .any(|(_, n)| n == "value")
                {
                    r.warnings.push(
                        "Parameter named \"value\" already present in property signature!"
                            .to_owned(),
                    );
                } else {
                    self.name_value_parameter(r, scope, last_variable, &last_name);
                }
            } else if is_event {
                // Raisers have no implicit parameter; only adders and removers get here.
                self.name_value_parameter(r, scope, last_variable, &last_name);
            } else if !is_property {
                add_existing_name(&mut self.scopes[scope].reserved, &last_name);
            }
        } else {
            for (i, (ty, name)) in parameters.iter().enumerate() {
                let mut name = name.clone();
                if name.trim().is_empty() && !matches!(ctx.ts.data(*ty), TypeData::ArgList) {
                    // As the reader's parameter variables (`CreateILVariable`).
                    name = format!("P_{i}");
                }
                let variable = variables.iter().copied().find(|&v| {
                    let var = r.function.var(v);
                    var.kind == VariableKind::Parameter && var.index == Some(i as i32)
                });
                if let Some(v) = variable {
                    self.scopes[scope].mapping.push((v, name.clone()));
                }
                add_existing_name(&mut self.scopes[scope].reserved, &name);
            }
        }
    }

    /// The implicit `value` parameter of a setter or event accessor.
    fn name_value_parameter(
        &mut self,
        r: &mut ReadFunction,
        scope: usize,
        variable: Option<VarId>,
        last_name: &str,
    ) {
        match variable {
            None => add_existing_name(&mut self.scopes[scope].reserved, last_name),
            Some(v) => {
                if r.function.var(v).name != "value" {
                    r.function.var_mut(v).name = "value".to_owned();
                }
                let name = r.function.var(v).name.clone();
                add_existing_name(&mut self.scopes[scope].reserved, &name);
            }
        }
    }

    /// `AssignNameToLocalFunction`.
    fn assign_name_to_local_function(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        function: NodeId,
        scope: usize,
    ) {
        let Inst::Function {
            read: Some(read),
            name,
            ..
        } = r.function.inst(function)
        else {
            return;
        };
        let method = read.method;
        let current = match name {
            Some(n) => n.clone(),
            None => ctx
                .ts
                .method_definition(method)
                .map(|m| m.name)
                .unwrap_or_default(),
        };
        let (without_number, number) = match local_function_name(&current) {
            Some(parsed) if is_valid_name(parsed) => split_name(parsed),
            // An obfuscated name, taken verbatim: its digits belong to it.
            _ if is_valid_name(&current) => (current.clone(), 1),
            _ => ("f".to_owned(), 1),
        };
        let count = match self.is_reserved_variable_name(scope, &without_number) {
            (false, _) => 1,
            (true, current_index) if current_index < number => number,
            (true, current_index) => number.max(current_index) + 1,
        };
        self.reserve_variable_name(scope, &without_number, count, None);
        let new_name = if count > 1 {
            format!("{without_number}{count}")
        } else {
            without_number
        };
        if let Inst::Function { name, .. } = r.function.inst_mut(function) {
            *name = Some(new_name.clone());
        }
        self.scopes[scope].local_functions.push((method, new_name));
    }

    /// The visitor: `Default` for each instruction, functions queued, local function uses
    /// renamed.
    fn visit(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        inst: NodeId,
        scope: usize,
    ) {
        match r.function.inst(inst) {
            Inst::Function { .. } => {
                self.work_list.push_back((Some(inst), Some(scope)));
                return;
            }
            Inst::Call(c) => {
                if let Some(l) = &c.method.local_function
                    && let Some(name) = self.local_function_existing_name(scope, l.method)
                    && let Inst::Call(c) = r.function.inst_mut(inst)
                    && let Some(l) = &mut c.method.local_function
                {
                    l.name = Some(name);
                }
            }
            Inst::LdFtn { method } => {
                if let Some(l) = &method.local_function
                    && let Some(name) = self.local_function_existing_name(scope, l.method)
                    && let Inst::LdFtn { method } = r.function.inst_mut(inst)
                    && let Some(l) = &mut method.local_function
                {
                    l.name = Some(name);
                }
            }
            _ => {}
        }
        if let Some(v) = variable_operand(&r.function, inst) {
            self.name_variable(r, ctx, scope, v);
        }
        for child in r.function.inst(inst).children() {
            self.visit(r, ctx, child, scope);
        }
    }

    /// The variable part of `Default`.
    fn name_variable(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        scope: usize,
        v: VarId,
    ) {
        // A valid name for the variable slot is used as it is.
        if let Some(name) = self.existing_name(r, scope, v)
            && !name.is_empty()
        {
            r.function.var_mut(v).name = name;
            return;
        }
        let owner = self.owners.get(&v).copied().flatten();
        let var = r.function.var(v);
        match var.kind {
            // Parameter names of top-level functions come from the reader and FixParameterNames.
            VariableKind::Parameter
                if !var.has_generated_name
                    && self.function_kind(r, owner) == FunctionKind::TopLevelFunction => {}
            VariableKind::InitializerTarget | VariableKind::NamedArgument => {
                let name = var.name.clone();
                self.reserve_variable_name(scope, &name, 1, None);
            }
            // Unread using variables are not declared in source.
            VariableKind::UsingLocal
                if var.address_instructions.is_empty() && var.load_instructions.is_empty() => {}
            VariableKind::DisplayClassLocal => {
                let name = self.next_display_class_local(scope);
                r.function.var_mut(v).name = name;
            }
            VariableKind::Local if var.index.is_some() => {
                let index = var.index.expect("checked") as usize;
                match self.existing_local_signature_name(scope, owner, index) {
                    Some(name) => {
                        // Locals of one signature slot get the same name.
                        r.function.var_mut(v).name = name;
                    }
                    None => {
                        let name = self.assign_name(r, ctx, scope, v);
                        r.function.var_mut(v).name = name.clone();
                        self.assign_name_to_local_signature_index(scope, owner, index, name);
                    }
                }
            }
            _ => {
                let name = self.assign_name(r, ctx, scope, v);
                r.function.var_mut(v).name = name;
            }
        }
    }

    /// `ILVariableEqualityComparer.Equals`.
    fn same_variable(&self, r: &ReadFunction, a: VarId, b: VarId) -> bool {
        if a == b {
            return true;
        }
        let (x, y) = (r.function.var(a), r.function.var(b));
        if matches!(x.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
            || matches!(y.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
        {
            return false;
        }
        if self.owners.get(&a) != self.owners.get(&b) || x.kind != y.kind {
            return false;
        }
        if x.index.is_some() {
            return x.index == y.index;
        }
        match (&x.state_machine_field, &y.state_machine_field) {
            (Some(fx), Some(fy)) => {
                fx.field.origin == fy.field.origin && fx.field.declaring == fy.field.declaring
            }
            _ => false,
        }
    }

    /// `TryGetExistingName(ILVariable)`.
    fn existing_name(&self, r: &ReadFunction, scope: usize, v: VarId) -> Option<String> {
        let s = &self.scopes[scope];
        if let Some((_, name)) = s.mapping.iter().find(|(w, _)| self.same_variable(r, *w, v)) {
            return Some(name.clone());
        }
        self.existing_name(r, s.parent?, v)
    }

    /// `TryGetExistingName(MethodDefinitionHandle)`.
    fn local_function_existing_name(&self, scope: usize, method: MethodDefId) -> Option<String> {
        let s = &self.scopes[scope];
        if let Some((_, name)) = s.local_functions.iter().rev().find(|(m, _)| *m == method) {
            return Some(name.clone());
        }
        self.local_function_existing_name(s.parent?, method)
    }

    /// `TryGetExistingName(ILFunction, int)`.
    fn existing_local_signature_name(
        &self,
        scope: usize,
        function: Option<NodeId>,
        index: usize,
    ) -> Option<String> {
        let s = &self.scopes[scope];
        if s.function == function {
            return s
                .assigned_local_signature_indices
                .get(index)
                .expect("a local index within the signature")
                .clone();
        }
        self.existing_local_signature_name(s.parent?, function, index)
    }

    /// `AssignNameToLocalSignatureIndex`.
    fn assign_name_to_local_signature_index(
        &mut self,
        scope: usize,
        function: Option<NodeId>,
        index: usize,
        name: String,
    ) {
        let mut s = Some(scope);
        while let Some(i) = s {
            if self.scopes[i].function == function {
                self.scopes[i].assigned_local_signature_indices[index] = Some(name);
                return;
            }
            s = self.scopes[i].parent;
        }
    }

    /// `IsReservedVariableName`: whether a name is reserved here or above, and its last index.
    fn is_reserved_variable_name(&self, scope: usize, name: &str) -> (bool, i32) {
        let s = &self.scopes[scope];
        if let Some(&index) = s.reserved.get(name) {
            return (true, index);
        }
        match s.parent {
            Some(p) => self.is_reserved_variable_name(p, name),
            None => (false, 0),
        }
    }

    /// `ReserveVariableName`: in the scope of `target` when it is an enclosing function.
    fn reserve_variable_name(
        &mut self,
        scope: usize,
        name: &str,
        index: i32,
        target: Option<Option<NodeId>>,
    ) {
        if let Some(target) = target
            && target != self.scopes[scope].function
        {
            let mut s = self.scopes[scope].parent;
            while let Some(i) = s {
                if self.scopes[i].function == target {
                    self.reserve_variable_name(i, name, index, None);
                    return;
                }
                s = self.scopes[i].parent;
            }
        }
        self.scopes[scope].reserved.insert(name.to_owned(), index);
    }

    /// `NextDisplayClassLocal`: numbered in the root scope.
    fn next_display_class_local(&mut self, scope: usize) -> String {
        match self.scopes[scope].parent {
            Some(p) => self.next_display_class_local(p),
            None => {
                let n = self.scopes[scope].num_display_class_locals;
                self.scopes[scope].num_display_class_locals += 1;
                format!("CS$<>8__locals{n}")
            }
        }
    }

    fn is_loop_counter(&self, scope: usize, v: VarId) -> bool {
        let s = &self.scopes[scope];
        s.loop_counters.contains(&v) || s.parent.is_some_and(|p| self.is_loop_counter(p, v))
    }

    /// `AssignNameIfUnassigned`.
    fn assign_name_if_unassigned(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        scope: usize,
        v: VarId,
    ) -> String {
        if let Some((_, name)) = self.scopes[scope]
            .mapping
            .iter()
            .find(|(w, _)| self.same_variable(r, *w, v))
        {
            return name.clone();
        }
        self.assign_name(r, ctx, scope, v)
    }

    /// `AssignName`.
    fn assign_name(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        scope: usize,
        v: VarId,
    ) -> String {
        let var = r.function.var(v);
        let mut new_name = var.name.clone();
        if var.has_generated_name || !is_valid_name(&new_name) {
            // A generated name from the symbols is replaced by one from the variable's use.
            new_name = self.generate_name_for_variable(r, ctx, scope, v);
        }
        let (mut without_number, mut new_index) = split_name(&new_name);
        if self.is_reserved_variable_name(scope, &without_number).0 {
            let ty = r.function.var(v).ty;
            if known(ctx, ty) == Some(KnownTypeCode::Int32) && self.is_loop_counter(scope, v) {
                // Loop counters are named i, j, ... rather than i, i2, ...
                new_name = self.generate_name_for_variable(r, ctx, scope, v);
                without_number = new_name.clone();
                new_index = 1;
            }
        }
        let (reserved, last_used) = self.is_reserved_variable_name(scope, &without_number);
        if reserved {
            if new_index <= last_used {
                new_index = last_used + 1;
            }
            new_name = format!("{without_number}{new_index}");
        }
        let owner = self.owners.get(&v).copied().flatten();
        self.reserve_variable_name(scope, &without_number, new_index, Some(owner));
        self.scopes[scope].mapping.push((v, new_name.clone()));
        new_name
    }

    /// `GenerateNameForVariable`.
    fn generate_name_for_variable(
        &self,
        r: &ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        scope: usize,
        v: VarId,
    ) -> String {
        let f = &r.function;
        let var = f.var(v);
        let mut proposed: Option<String> = None;
        if known(ctx, var.ty) == Some(KnownTypeCode::Int32)
            && self.scopes[scope].loop_counters.contains(&v)
        {
            // For loop variables: i, j, k, l, m, n.
            for c in 'i'..=MAX_LOOP_VARIABLE_NAME {
                if !self.is_reserved_variable_name(scope, &c.to_string()).0 {
                    proposed = Some(c.to_string());
                    break;
                }
            }
        }
        // The WinForms designer only loads a form whose resource manager is named `resources`.
        if is_windows_forms_initialize_component_method(ctx, r.method)
            && type_full_name(ctx.ts, var.ty).as_deref()
                == Some("System.ComponentModel.ComponentResourceManager")
        {
            proposed = Some("resources".to_owned());
        }
        if proposed.as_deref().is_none_or(str::is_empty) {
            let mut names: Vec<String> = Vec::new();
            for &a in &var.address_instructions {
                if !matches!(f.inst(a), Inst::LdLoca { .. }) {
                    continue;
                }
                let Some(parent) = f.node(a).parent else {
                    continue;
                };
                let index = f.node(a).child_index as usize;
                if let Some(name) = call_parameter_name(f, ctx.ts, parent, index)
                    && !name.trim().is_empty()
                    && !self.lower_case_names.contains(&name)
                    && !names.contains(&name)
                {
                    names.push(name);
                }
            }
            if let Some(first) = names.into_iter().next() {
                proposed = Some(first);
            }
        }
        if proposed.as_deref().is_none_or(str::is_empty) {
            let mut names: Vec<Option<String>> = Vec::new();
            for &store in &var.store_instructions {
                let name = match f.inst(store) {
                    Inst::StLoc { value, .. } => Some(name_from_instruction(f, ctx.ts, *value)),
                    Inst::Match { tested_operand, .. } if is_sub_pattern(f, store) => {
                        Some(name_from_instruction(f, ctx.ts, *tested_operand))
                    }
                    Inst::PinnedRegion { init, .. } => {
                        Some(name_from_instruction(f, ctx.ts, *init))
                    }
                    _ => None,
                };
                if let Some(name) = name
                    && !name
                        .as_ref()
                        .is_some_and(|n| self.lower_case_names.contains(n))
                    && !names.contains(&name)
                {
                    names.push(name);
                }
            }
            if let [single] = &names[..] {
                proposed = single.clone();
            }
        }
        if proposed.as_deref().is_none_or(str::is_empty) {
            let mut names: Vec<Option<String>> = Vec::new();
            for &load in &var.load_instructions {
                let Some(parent) = f.node(load).parent else {
                    continue;
                };
                let name = name_for_argument(f, ctx, parent, f.node(load).child_index as usize);
                if !name
                    .as_ref()
                    .is_some_and(|n| self.lower_case_names.contains(n))
                    && !names.contains(&name)
                {
                    names.push(name);
                }
            }
            if let [single] = &names[..] {
                proposed = single.clone();
            }
        }
        if proposed.as_deref().is_none_or(str::is_empty) && var.kind == VariableKind::StackSlot {
            let mut names: Vec<String> = Vec::new();
            for &store in &var.store_instructions {
                let Some((_, value)) = f.match_stloc(store) else {
                    continue;
                };
                let ty = guess_type(f, ctx, var.ty, value);
                let name = name_by_type(ctx, ty);
                if !self.lower_case_names.contains(&name) && !names.contains(&name) {
                    names.push(name);
                }
            }
            if let [single] = &names[..] {
                proposed = Some(single.clone());
            }
        }
        let proposed = match proposed {
            Some(p) if !p.is_empty() => p,
            _ => name_by_type(ctx, var.ty),
        };
        // Generated names lose their number suffix.
        split_name(&proposed).0
    }
}

/// The loop counters of a function: the increments of its for loops, in a pre-order walk that
/// stops at the first nested function.
fn find_loop_counters(f: &Function, function: Option<NodeId>) -> HashSet<VarId> {
    let roots: Vec<NodeId> = match function {
        Some(n) => vec![n],
        None => f
            .body
            .into_iter()
            .chain(f.local_functions.iter().copied())
            .collect(),
    };
    let mut counters = HashSet::new();
    let mut stack: Vec<NodeId> = roots.into_iter().rev().collect();
    while let Some(n) = stack.pop() {
        if matches!(f.inst(n), Inst::Function { .. }) && Some(n) != function {
            break;
        }
        if let Inst::BlockContainer {
            kind: ContainerKind::For,
            blocks,
            ..
        } = f.inst(n)
            && let Some(&increment) = blocks.last()
        {
            for &i in f.list(increment) {
                if let Some(v) = match_increment(f, i) {
                    counters.insert(v);
                }
            }
        }
        stack.extend(f.inst(n).children().into_iter().rev());
    }
    counters
}

/// `HighLevelLoopTransform.MatchIncrement`.
fn match_increment(f: &Function, inst: NodeId) -> Option<VarId> {
    use crate::il::ast::{BinaryNumericOperator, CompoundTargetKind};
    match *f.inst(inst) {
        Inst::StLoc { var, value, .. } => match *f.inst(value) {
            Inst::BinaryNumeric {
                op: BinaryNumericOperator::Add,
                left,
                ..
            } if matches!(*f.inst(left), Inst::LdLoc { var: l } if l == var) => Some(var),
            _ => None,
        },
        Inst::NumericCompoundAssign {
            target_kind: CompoundTargetKind::Address,
            target,
            ..
        }
        | Inst::UserDefinedCompoundAssign {
            target_kind: CompoundTargetKind::Address,
            target,
            ..
        }
        | Inst::DynamicCompoundAssign {
            target_kind: CompoundTargetKind::Address,
            target,
            ..
        } => match *f.inst(target) {
            Inst::LdLoca { var } => Some(var),
            _ => None,
        },
        _ => None,
    }
}

/// `ILFunction.Parameters`: types and names.
fn function_parameters(
    r: &ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    function: Option<NodeId>,
) -> Vec<(TypeId, String)> {
    let method = match function.map(|n| r.function.inst(n)) {
        None => Some(r.method),
        Some(Inst::Function {
            read: Some(read), ..
        }) => Some(read.method),
        Some(Inst::Function { parameters, .. }) => return parameters.clone(),
        Some(_) => None,
    };
    let Some(method) = method else {
        return Vec::new();
    };
    let (_, params) = ctx.ts.method_signature(method);
    params.into_iter().map(|p| (p.ty, p.name)).collect()
}

/// The parameter count of the property an accessor belongs to (its indexer parameters).
fn property_parameter_count(
    ts: &mut TypeSystem<'_, '_>,
    method: MethodDefId,
    accessor: Option<(u32, u16)>,
) -> usize {
    let (_, params) = ts.method_signature(method);
    match accessor.map_or(0, |(_, s)| s) {
        // A setter's last parameter is the value.
        SETTER => params.len().saturating_sub(1),
        _ => params.len(),
    }
}

/// `CSharpDecompiler.IsTransparentIdentifier`.
fn is_transparent_identifier(identifier: &str) -> bool {
    if identifier.starts_with("<>") {
        return identifier.contains("TransparentIdentifier") || identifier.contains("TranspIdent");
    }
    // VB names the carriers of its query range variables $VB$It, $VB$It1, ...
    identifier.starts_with("$VB$It")
}

/// `CSharpDecompiler.IsWindowsFormsInitializeComponentMethod`.
fn is_windows_forms_initialize_component_method(
    ctx: &mut Context<'_, '_, '_>,
    method: MethodDefId,
) -> bool {
    let Ok(m) = ctx.ts.method_definition(method) else {
        return false;
    };
    if m.name != "InitializeComponent" || known(ctx, m.ret) != Some(KnownTypeCode::Void) {
        return false;
    }
    let Some(declaring) = m.declaring else {
        return false;
    };
    let Ok(bases) = ctx.ts.non_interface_base_types(declaring) else {
        return false;
    };
    bases
        .into_iter()
        .any(|b| type_full_name(ctx.ts, b).as_deref() == Some("System.Windows.Forms.Control"))
}

/// `IType.FullName` of a type definition or instance.
fn type_full_name(ts: &TypeSystem<'_, '_>, t: TypeId) -> Option<String> {
    let def = ts.definition(t)?;
    let name = &ts.comp.type_def(def).ok()?.full_name;
    let mut s = if name.top.ns.is_empty() {
        name.top.name.clone()
    } else {
        format!("{}.{}", name.top.ns, name.top.name)
    };
    for (nested, _) in &name.nested {
        s.push('.');
        s.push_str(nested);
    }
    Some(s)
}

/// `IType.Name` of a type definition or instance.
fn type_name(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> Option<String> {
    match ts.data(t).clone() {
        TypeData::Unknown { name, .. } => {
            return Some(
                name.nested
                    .last()
                    .map_or_else(|| name.top.name.clone(), |(n, _)| n.clone()),
            );
        }
        TypeData::TypeParam { owner, index } => {
            let (module, table, row) = match owner {
                TypeParamOwner::Type(def) => (def.module, TableId::TypeDef, def.row),
                TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => {
                    (m.module, TableId::MethodDef, m.row)
                }
                TypeParamOwner::DummyType | TypeParamOwner::DummyMethod => return None,
                TypeParamOwner::Synthetic { id, .. } => {
                    return Some(ts.synthetic_type_parameter(id).name.clone());
                }
            };
            let tps = ts.type_parameters(module, table, row).ok()?;
            return tps.get(index as usize).map(|tp| tp.name.clone());
        }
        TypeData::Inst { generic, .. } => return type_name(ts, generic),
        _ => {}
    }
    let def = ts.definition(t)?;
    let name = &ts.comp.type_def(def).ok()?.full_name;
    Some(
        name.nested
            .last()
            .map_or_else(|| name.top.name.clone(), |(n, _)| n.clone()),
    )
}

/// The variable of an `IInstructionWithVariableOperand`.
fn variable_operand(f: &Function, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::LdLoc { var }
        | Inst::LdLoca { var }
        | Inst::StLoc { var, .. }
        | Inst::TryCatchHandler { variable: var, .. }
        | Inst::Match { variable: var, .. }
        | Inst::PinnedRegion { variable: var, .. }
        | Inst::Using { variable: var, .. } => Some(var),
        _ => None,
    }
}

/// `SlotInfo == MatchInstruction.SubPatternsSlot`.
fn is_sub_pattern(f: &Function, inst: NodeId) -> bool {
    f.node(inst)
        .parent
        .is_some_and(|p| matches!(f.inst(p), Inst::Match { .. }) && f.node(inst).child_index >= 1)
}

/// The method of a call instruction.
fn call_method(f: &Function, call: NodeId) -> Option<&ResolvedMethod> {
    match f.inst(call) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => Some(&c.method.method),
        _ => None,
    }
}

/// `call.GetParameter(index).Name`.
fn call_parameter_name(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    call: NodeId,
    index: usize,
) -> Option<String> {
    let method = call_method(f, call)?;
    let first = usize::from(!(method.is_static || matches!(f.inst(call), Inst::NewObj(_))));
    let index = index.checked_sub(first)?;
    let count = method.params().ok()?.len();
    if index >= count {
        return None;
    }
    match method.origin {
        MethodOrigin::Def(d) => ts.method_signature(d).1.get(index).map(|p| p.name.clone()),
        _ => Some(String::new()),
    }
}

/// `ExcludeMethodFromCandidates`.
fn exclude_method_from_candidates(ts: &mut TypeSystem<'_, '_>, m: &ResolvedMethod) -> bool {
    if ts.method_is_operator(m) || m.name == "ToString" || m.name == "GetPinnableReference" {
        return true;
    }
    m.name == "Concat"
        && m.declaring.is_some_and(|d| {
            ts.definition(d)
                .and_then(|def| ts.comp.type_def(def).ok())
                .is_some_and(|i| i.known == KnownTypeCode::String)
        })
}

/// `IsCompilerGeneratedOrIsInCompilerGeneratedClass` of a field.
fn field_is_compiler_generated(ts: &TypeSystem<'_, '_>, field: &ResolvedField) -> bool {
    let FieldOrigin::Def { module, row } = field.origin else {
        return false;
    };
    if ts.has_compiler_generated(module, TableId::Field, row) {
        return true;
    }
    let declaring = ts
        .comp
        .module(module)
        .md
        .field_declaring_type(row)
        .unwrap_or(0);
    declaring != 0
        && ts.is_compiler_generated_or_in_compiler_generated_class(TypeDefId {
            module,
            row: declaring,
        })
}

/// `GetNameFromInstruction`.
pub(crate) fn name_from_instruction(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
) -> Option<String> {
    match f.inst(inst) {
        Inst::GetPinnableReference { arg, .. } => name_from_instruction(f, ts, *arg),
        Inst::LdObj { target, .. } => name_from_instruction(f, ts, *target),
        Inst::LdFlda { target, field, .. } => {
            if field_is_compiler_generated(ts, &field.field) {
                return name_from_instruction(f, ts, *target);
            }
            clean_up_variable_name(&field.field.name)
        }
        Inst::LdsFlda { field } => clean_up_variable_name(&field.field.name),
        Inst::Call(c) | Inst::CallVirt(c) => {
            let m = c.method.method.clone();
            if exclude_method_from_candidates(ts, &m) {
                return None;
            }
            let param_count = m.params().map_or(0, |p| p.len());
            if starts_with_ignore_case(&m.name, "get_") && param_count == 0 {
                // Property names, but not indexers.
                return clean_up_variable_name(&m.name[4..]);
            }
            if starts_with_ignore_case(&m.name, "Get")
                && m.name.len() >= 4
                && char_at(&m.name, 3).is_some_and(is_upper)
            {
                return clean_up_variable_name(&m.name[3..]);
            }
            None
        }
        Inst::Dynamic {
            kind: crate::il::ast::DynamicKind::InvokeMember,
            name: Some(name),
            ..
        } => {
            if starts_with_ignore_case(name, "Get")
                && name.len() >= 4
                && char_at(name, 3).is_some_and(is_upper)
            {
                return clean_up_variable_name(&name[3..]);
            }
            None
        }
        _ => None,
    }
}

/// `GetNameForArgument`.
fn name_for_argument(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    parent: NodeId,
    i: usize,
) -> Option<String> {
    match f.inst(parent) {
        Inst::StObj { target, .. } => match f.inst(*target) {
            Inst::LdFlda { field, .. } | Inst::LdsFlda { field } => {
                clean_up_variable_name(&field.field.name)
            }
            _ => None,
        },
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
            let m = c.method.method.clone();
            if exclude_method_from_candidates(ctx.ts, &m) {
                return None;
            }
            let param_count = m.params().map_or(0, |p| p.len());
            if param_count == 1 && i + 1 == c.args.len() {
                // The value of a setter.
                if starts_with_ignore_case(&m.name, "set_") {
                    return clean_up_variable_name(&m.name[4..]);
                }
                if starts_with_ignore_case(&m.name, "Set")
                    && m.name.len() >= 4
                    && char_at(&m.name, 3).is_some_and(is_upper)
                {
                    return clean_up_variable_name(&m.name[3..]);
                }
            }
            match call_parameter_name(f, ctx.ts, parent, i) {
                Some(name) if !name.is_empty() => clean_up_variable_name(&name),
                _ => None,
            }
        }
        Inst::Leave { .. } => Some("result".to_owned()),
        _ => None,
    }
}

/// `GuessType`: for an `object` variable, the inferred type of the stored value.
fn guess_type(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    variable_type: TypeId,
    value: NodeId,
) -> TypeId {
    if known(ctx, variable_type) != Some(KnownTypeCode::Object) {
        return variable_type;
    }
    let inferred = f.infer_type(ctx.ts, value);
    if matches!(
        ctx.ts.data(inferred),
        TypeData::Unknown { .. } | TypeData::UnknownSpecial
    ) {
        variable_type
    } else {
        inferred
    }
}

/// `NullableType.GetUnderlyingType`.
fn nullable_underlying(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    let t = match *ctx.ts.data(t) {
        TypeData::Annotated { inner, .. } => inner,
        _ => t,
    };
    match ctx.ts.data(t).clone() {
        TypeData::Inst { generic, args }
            if args.len() == 1 && known(ctx, generic) == Some(KnownTypeCode::NullableOfT) =>
        {
            args[0]
        }
        _ => t,
    }
}

/// `GetNameByType`.
fn name_by_type(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> String {
    let mut t = nullable_underlying(ctx, ty);
    while let TypeData::Modified { inner, .. } | TypeData::Pinned(inner) = *ctx.ts.data(t) {
        t = nullable_underlying(ctx, inner);
    }
    let name = type_name(ctx.ts, t).map(|n| strip_arity(&n).to_owned());
    if ctx
        .ts
        .definition(t)
        .is_some_and(|d| is_anonymous_type(ctx, d))
    {
        return "anon".to_owned();
    }
    if let Some(name) = &name {
        if name.ends_with("Exception") {
            return "ex".to_owned();
        }
        if name.ends_with("EventArgs") {
            return "e".to_owned();
        }
    }
    if matches!(ctx.ts.data(t), TypeData::NInt | TypeData::NUInt) {
        return "num".to_owned();
    }
    if let Some(full) = type_full_name(ctx.ts, t)
        && let Some(n) = primitive_variable_name(&full)
        && matches!(ctx.ts.data(t), TypeData::Def(_))
    {
        return n.to_owned();
    }
    // `ParameterizedType.Kind` is the kind of its generic type.
    let kind = match *ctx.ts.data(t) {
        TypeData::Inst { generic, .. } => generic,
        _ => t,
    };
    let name = match ctx.ts.data(kind) {
        TypeData::Array { .. } => "array".to_owned(),
        TypeData::Pointer(_) => "ptr".to_owned(),
        TypeData::TypeParam { .. }
        | TypeData::Unknown { .. }
        | TypeData::UnknownSpecial
        | TypeData::Dynamic => "val".to_owned(),
        TypeData::ByRef(_) => "reference".to_owned(),
        TypeData::Tuple { .. } => "tuple".to_owned(),
        _ => name.unwrap_or_default(),
    };
    // The `I` of interfaces goes.
    let chars: Vec<char> = name.chars().collect();
    let name = if chars.len() >= 3 && chars[0] == 'I' && is_upper(chars[1]) && is_lower(chars[2]) {
        chars[1..].iter().collect()
    } else {
        name
    };
    clean_up_variable_name(&name).unwrap_or_else(|| "obj".to_owned())
}

/// `typeNameToVariableNameDict`.
fn primitive_variable_name(full_name: &str) -> Option<&'static str> {
    Some(match full_name {
        "System.Boolean" => "flag",
        "System.Byte" | "System.SByte" => "b",
        "System.Int16" | "System.Int32" | "System.Int64" | "System.UInt16" | "System.UInt32"
        | "System.UInt64" | "System.Single" | "System.Double" | "System.Decimal" => "num",
        "System.String" => "text",
        "System.Object" => "obj",
        "System.Char" => "c",
        _ => return None,
    })
}

fn strip_arity(name: &str) -> &str {
    name.split('`').next().unwrap_or(name)
}

fn starts_with_ignore_case(s: &str, prefix: &str) -> bool {
    s.len() >= prefix.len() && s.as_bytes()[..prefix.len()].eq_ignore_ascii_case(prefix.as_bytes())
}

fn char_at(s: &str, index: usize) -> Option<char> {
    s.chars().nth(index)
}

// shortcut: `char.IsLetter`, `IsLetterOrDigit`, `IsUpper` and `IsLower` use Unicode general
// categories, which Rust does not expose; ASCII matches exactly, other characters follow the
// close Rust properties. Use a category table if obfuscated names show a difference.
fn is_letter(c: char) -> bool {
    c.is_alphabetic()
}

fn is_letter_or_digit(c: char) -> bool {
    c.is_alphabetic() || c.is_numeric()
}

fn is_upper(c: char) -> bool {
    c.is_uppercase()
}

fn is_lower(c: char) -> bool {
    c.is_lowercase()
}

/// `IsValidName`.
fn is_valid_name(name: &str) -> bool {
    let mut chars = name.chars();
    let Some(first) = chars.next() else {
        return false;
    };
    if name.chars().all(char::is_whitespace) {
        return false;
    }
    (is_letter(first) || first == '_') && chars.all(|c| is_letter_or_digit(c) || c == '_')
}

/// `TextWriterTokenWriter.ContainsNonPrintableIdentifierChar`.
// shortcut: the managed check excludes several Unicode categories (marks, format, private use
// and others); outside ASCII the port only excludes whitespace and control characters. Use a
// category table if obfuscated names show a difference.
fn contains_non_printable_identifier_char(name: &str) -> bool {
    name.chars().any(|c| {
        if c.is_whitespace() || c == '\\' {
            return true;
        }
        if matches!(c, ' ' | '_' | '`' | '^') {
            return false;
        }
        c.is_control()
    })
}

/// `CSharpOutputVisitor.IsKeyword` without a context.
fn is_keyword(identifier: &str) -> bool {
    const KEYWORDS: &[&str] = &[
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while",
        "from",
        "where",
        "join",
        "on",
        "equals",
        "into",
        "let",
        "orderby",
        "ascending",
        "descending",
        "select",
        "group",
        "by",
    ];
    // Only lower-case identifiers of keyword length can be keywords.
    let max = KEYWORDS.iter().map(|k| k.len()).max().unwrap_or(0);
    if identifier.len() > max || identifier.len() < 2 || identifier.as_bytes()[0] < b'a' {
        return false;
    }
    if KEYWORDS.contains(&identifier) {
        return true;
    }
    // Without a context `await` (and the other contextual keywords) count as keywords too.
    identifier == "await"
}

/// `CleanUpVariableName`.
fn clean_up_variable_name(name: &str) -> Option<String> {
    // The backtick of generics goes.
    let mut name = strip_arity(name);
    // Field prefixes go.
    if name.len() > 2 && name.starts_with("m_") {
        name = &name[2..];
    } else if name.len() > 1
        && name.starts_with('_')
        && char_at(name, 1).is_some_and(|c| is_letter(c) || c == '_')
    {
        name = &name[1..];
    }
    if contains_non_printable_identifier_char(name) {
        return None;
    }
    if name.is_empty() {
        return Some("obj".to_owned());
    }
    // A metadata name may be invalid in C# (VB separates its generated names with `$`).
    if !is_valid_name(name) {
        return None;
    }
    let mut chars = name.chars();
    let first = chars.next().expect("not empty");
    let lower: String = first
        .to_lowercase()
        .next()
        .into_iter()
        .chain(chars)
        .collect();
    if is_keyword(&lower) {
        return None;
    }
    Some(lower)
}

/// `AddExistingName`.
pub(crate) fn add_existing_name(reserved: &mut HashMap<String, i32>, name: &str) {
    if name.is_empty() {
        return;
    }
    let (without_digits, number) = split_name(name);
    let entry = reserved.entry(without_digits).or_insert(number);
    *entry = (*entry).max(number);
}

/// `SplitName`: the name without a numeric suffix, and the suffix (1 without one).
pub(crate) fn split_name(name: &str) -> (String, i32) {
    let digits = name.bytes().rev().take_while(u8::is_ascii_digit).count();
    let pos = name.len() - digits;
    if digits > 0
        && let Ok(number) = name[pos..].parse::<i64>()
        && number <= i64::from(i32::MAX)
    {
        return (name[..pos].to_owned(), number as i32);
    }
    (name.to_owned(), 1)
}

#[cfg(test)]
mod tests {
    use super::{clean_up_variable_name, split_name};

    #[test]
    fn names_split_and_clean_up_like_the_managed_code() {
        assert_eq!(split_name("num2"), ("num".to_owned(), 2));
        assert_eq!(split_name("num"), ("num".to_owned(), 1));
        assert_eq!(split_name("x99999999999"), ("x99999999999".to_owned(), 1));
        assert_eq!(clean_up_variable_name("m_Value").as_deref(), Some("value"));
        assert_eq!(clean_up_variable_name("_count").as_deref(), Some("count"));
        assert_eq!(clean_up_variable_name("List`1").as_deref(), Some("list"));
        assert_eq!(clean_up_variable_name("String"), None);
        assert_eq!(clean_up_variable_name("VB$It"), None);
    }
}
