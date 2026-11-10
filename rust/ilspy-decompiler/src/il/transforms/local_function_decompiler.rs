//! `IL/Transforms/LocalFunctionDecompiler.cs`: C# 7 local functions. Every use (`call`,
//! `callvirt`, `ldftn` or a delegate construction) of a compiler-generated local function
//! method is found, the method is read into a nested function (in its own arena, with the IL
//! transforms before this one, then imported), its closure parameters and `this` are replaced
//! by the display classes the use sites pass, it moves to the function that declares it, and
//! the use sites become calls of the reduced method (`LocalFunctionMethod`).

use crate::il::ast::{
    CallData, Function, FunctionKind, Inst, LocalFunctionRef, MethodOperand, NestedRead, NodeId,
    ReducedMethod, VarId, VariableKind,
};
use crate::il::reader::{ReadFunction, read_il_in_context};
use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{TypeDefId, TypeKind};
use crate::typesystem::members::{MethodDefId, has_body};
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{GenericContext, TypeData, TypeId, TypeParamOwner, TypeSystem};

use super::delegate_construction::{match_delegate_construction, replace_delegate_target};
use super::{Context, LOCAL_FUNCTION_DECOMPILER_LEAF, run_prefix};

/// `LocalFunctionInfo`.
struct Info {
    id: MethodDefId,
    /// `Method`: the definition.
    method: ResolvedMethod,
    use_sites: Vec<NodeId>,
    /// The nested function, if it could be read.
    definition: Option<NodeId>,
    /// `LocalFunctionArguments`, in insertion order.
    arguments: Vec<(i32, Vec<NodeId>)>,
}

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.local_functions {
        return;
    }
    // Decompiling a display class or local function method on its own leaves it as it is.
    let declaring = declaring_type(ctx.ts, r.method);
    if is_local_function_method(ctx.ts, r.method)
        || declaring.is_some_and(|d| is_local_function_display_class(ctx.ts, d))
    {
        return;
    }
    let mut lfd = Decompiler {
        decompiled_type: declaring,
        infos: Vec::new(),
    };
    if let Some(body) = r.function.body {
        lfd.find_use_sites(r, ctx, body);
    }
    lfd.replace_references_to_display_class_this(r, ctx);
    lfd.determine_capture_and_declaration_scopes(r, ctx);
    lfd.propagate_closure_parameter_arguments(r, ctx);
    lfd.transform_use_sites(r, ctx);
}

struct Decompiler {
    /// `resolveContext.CurrentTypeDefinition`.
    decompiled_type: Option<TypeDefId>,
    infos: Vec<Info>,
}

impl Decompiler {
    fn info(&self, id: MethodDefId) -> Option<usize> {
        self.infos.iter().position(|i| i.id == id)
    }

    /// `FindUseSites` over the body of a function.
    fn find_use_sites(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        body: NodeId,
    ) {
        for inst in r.function.descendants_post(body) {
            let use_site = match r.function.inst(inst) {
                Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
                    Some((c.method.clone(), inst))
                }
                Inst::LdFtn { method } => {
                    let parent = r.function.node(inst).parent;
                    let site = match parent {
                        Some(p)
                            if matches!(r.function.inst(p), Inst::NewObj(_))
                                && match_delegate_construction(&r.function, ctx, p) =>
                        {
                            p
                        }
                        _ => inst,
                    };
                    Some((method.clone(), site))
                }
                _ => None,
            };
            let Some((method, site)) = use_site else {
                continue;
            };
            if method.local_function.is_some() {
                continue;
            }
            let MethodOrigin::Def(id) = method.method.origin else {
                continue;
            };
            if id.module != r.method.module || !is_local_function_method(ctx.ts, id) {
                continue;
            }
            self.handle_use_site(r, ctx, &method, id, site);
        }
    }

    /// `HandleUseSite`.
    fn handle_use_site(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        target: &MethodOperand,
        id: MethodDefId,
        site: NodeId,
    ) {
        if let Some(i) = self.info(id) {
            self.infos[i].use_sites.push(site);
            return;
        }
        let Ok(method) = ctx.ts.method_definition(id) else {
            return;
        };
        let skip_count = self.skip_count(ctx, r.method, &method);
        let definition = self.read_local_function_definition(r, ctx, target, id, skip_count);
        self.infos.push(Info {
            id,
            method,
            use_sites: vec![site],
            definition,
            arguments: Vec::new(),
        });
        if let Some(definition) = definition
            && let Inst::Function { body, .. } = *r.function.inst(definition)
        {
            self.find_use_sites(r, ctx, body);
        }
    }

    /// `GetSkipCount`: how many leading type parameters of the local function are the
    /// caller's.
    fn skip_count(
        &self,
        ctx: &mut Context<'_, '_, '_>,
        root: MethodDefId,
        target: &ResolvedMethod,
    ) -> u32 {
        let class_count = |ts: &TypeSystem<'_, '_>, d: Option<TypeId>| {
            d.and_then(|d| ts.definition(d))
                .and_then(|d| ts.comp.type_def(d).ok())
                .map_or(0, |i| i.type_parameter_count) as i64
        };
        let root_method = ctx.ts.method_definition(root).ok();
        let root_class = root_method
            .as_ref()
            .map_or(0, |m| class_count(ctx.ts, m.declaring));
        let root_method_count = root_method.as_ref().map_or(0, |m| m.type_args.len() as i64);
        let mut skip =
            (root_class + root_method_count - class_count(ctx.ts, target.declaring)).max(0);
        if !target.type_args.is_empty() {
            let MethodOrigin::Def(id) = target.origin else {
                return skip as u32;
            };
            // The highest index of the local function's type parameters among the type
            // arguments of its closure parameters.
            let mut last: Option<i64> = None;
            for p in target.params().unwrap_or_default() {
                if !self.is_closure_parameter(ctx, p) {
                    continue;
                }
                let element = match *ctx.ts.data(p) {
                    TypeData::ByRef(e) => e,
                    _ => p,
                };
                for a in ctx.ts.type_arguments(element) {
                    let index = match *ctx.ts.data(a) {
                        TypeData::TypeParam {
                            owner: TypeParamOwner::Method(m),
                            index,
                        } if m == id => i64::from(index),
                        _ => -1,
                    };
                    last = Some(last.map_or(index, |l| l.max(index)));
                }
            }
            if let Some(l) = last
                && l + 1 > skip
            {
                skip = l + 1;
            }
        }
        skip as u32
    }

    /// `ReadLocalFunctionDefinition`: the local function, added to the top-level function's.
    fn read_local_function_definition(
        &self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        target: &MethodOperand,
        id: MethodDefId,
        skip_count: u32,
    ) -> Option<NodeId> {
        let context = generic_context_from_type_arguments(ctx.ts, target, id, skip_count)?;
        let md = &ctx.ts.comp.module(id.module).md;
        let def = md.get::<rows::MethodDef>(id.row).ok()?;
        let parameters = target.method.params().unwrap_or_default();
        if !has_body(&def) {
            // An `extern` local function: a `nop` body, and the use site's method.
            let reduced = self.reduce_to_local_function(ctx, id, &parameters, skip_count);
            let body = r.function.add(Inst::Nop {
                kind: crate::il::ast::NopKind::Normal,
            });
            let function = r.function.add(Inst::Function {
                kind: FunctionKind::LocalFunction,
                delegate_type: None,
                return_type: target.method.ret,
                parameters: parameters.into_iter().map(|t| (t, String::new())).collect(),
                variables: Vec::new(),
                helper_variable_count: 0,
                body,
                read: Some(Box::new(NestedRead {
                    method: id,
                    context: context.map_or(0, |c| context_index(&c)),
                    code_size: 0,
                    local_signature_length: 0,
                    async_return_type: None,
                    is_iterator: false,
                    state_machine_compiled_with_mono: false,
                    state_machine_compiled_with_legacy_visual_basic: false,
                    move_next_method: None,
                    async_debug_info: None,
                    warnings: Vec::new(),
                    sequence_point_candidates: Vec::new(),
                    method_operand: Some(target.clone()),
                })),
                local_functions: Vec::new(),
                declaration_scope: None,
                reduced: Some(reduced),
            });
            r.function.local_functions.push(function);
            r.function.connect_root(function);
            return Some(function);
        }
        let body = md.method_body(def.rva).ok()?;
        let mut read = read_il_in_context(ctx.ts, id, body, ctx.reader, context).ok()?;
        read.kind = FunctionKind::LocalFunction;
        {
            let mut nested = Context {
                ts: &mut *ctx.ts,
                settings: ctx.settings,
                reader: ctx.reader,
                rerun_requested: false,
                removed_variables: Vec::new(),
                resolver: ctx.resolver,
            };
            run_prefix(&mut read, &mut nested, LOCAL_FUNCTION_DECOMPILER_LEAF);
            read.flush_pending_variables();
        }
        let reduced =
            self.reduce_to_local_function(ctx, id, &read_method_params(ctx.ts, &read), skip_count);
        let function = import_function(r, read, reduced, target.method.ret, parameters);
        r.function.local_functions.push(function);
        r.function.connect_root(function);
        if let Inst::Function { body, .. } = *r.function.inst(function) {
            r.function.release_root(body);
        }
        Some(function)
    }

    /// `ReduceToLocalFunction`.
    fn reduce_to_local_function(
        &self,
        ctx: &mut Context<'_, '_, '_>,
        id: MethodDefId,
        params: &[TypeId],
        skip_count: u32,
    ) -> ReducedMethod {
        let parameters_to_remove = params
            .iter()
            .rev()
            .take_while(|&&p| self.is_closure_parameter(ctx, p))
            .count() as u32;
        let can_be_static = ctx.settings.static_local_functions && parameters_to_remove == 0 && {
            let md = &ctx.ts.comp.module(id.module).md;
            let is_static = md
                .get::<rows::MethodDef>(id.row)
                .is_ok_and(|d| d.flags & 0x0010 != 0);
            // Used together with a lambda, the local function is in the display class type;
            // without instance fields it can be static.
            is_static
                || declaring_type(ctx.ts, id).is_some_and(|d| {
                    ctx.ts
                        .has_compiler_generated(d.module, TableId::TypeDef, d.row)
                        && !has_instance_fields(ctx.ts, d)
                })
        };
        ReducedMethod {
            is_static_local_function: can_be_static,
            number_of_compiler_generated_parameters: parameters_to_remove,
            number_of_compiler_generated_type_parameters: skip_count,
        }
    }

    /// `ReplaceReferencesToDisplayClassThis`.
    fn replace_references_to_display_class_this(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) {
        for i in 0..self.infos.len() {
            let Some(definition) = self.infos[i].definition else {
                continue;
            };
            if function_method_is_static(ctx.ts, &r.function, definition) {
                continue;
            }
            let Some(this_variable) = function_variables(&r.function, definition)
                .into_iter()
                .find(|&v| is_this(&r.function, v))
            else {
                continue;
            };
            let arguments: Vec<NodeId> = self.infos[i]
                .use_sites
                .iter()
                .filter_map(|&u| match r.function.inst(u) {
                    Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => c.args.first().copied(),
                    _ => None,
                })
                .collect();
            let Some(argument) = self.find_compatible_argument(r, ctx, i, &arguments, true) else {
                continue;
            };
            replace_delegate_target(&mut r.function, definition, argument, Some(this_variable));
            self.determine_argument(r, ctx, i, -1, argument);
        }
    }

    /// `DetermineCaptureAndDeclarationScopes`.
    fn determine_capture_and_declaration_scopes(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) {
        let Some(top_body) = r.function.body else {
            return;
        };
        let is_constructor = ctx
            .ts
            .method_definition(r.method)
            .is_ok_and(|m| m.name == ".ctor" || m.name == ".cctor");
        for i in 0..self.infos.len() {
            let Some(definition) = self.infos[i].definition else {
                // shortcut: the managed warning names the method with `IMethod.ToString()`,
                // which the engine does not format; it only follows a failed read.
                r.warnings
                    .push("Could not decode local function".to_owned());
                continue;
            };
            for u in self.infos[i].use_sites.clone() {
                self.determine_use_site(r, ctx, i, u);
            }
            let f = &mut r.function;
            if is_constructor {
                // Local functions reached from a field initializer capture nothing; the
                // innermost block containing all use sites is a better place for them.
                let mut scope: Option<NodeId> = None;
                for &u in &self.infos[i].use_sites {
                    scope = match scope {
                        None => closest_container(f, u),
                        Some(s) => common_container(f, u, s),
                    };
                    if scope.is_none() {
                        break;
                    }
                }
                let current = declaration_scope(f, definition);
                set_declaration_scope(f, definition, scope.or(current).or(Some(top_body)));
            }
            if declaration_scope(f, definition).is_none() {
                set_declaration_scope(f, definition, Some(top_body));
            }
            let scope = declaration_scope(f, definition).expect("set above");
            if let Some(declaring) = enclosing_function_inclusive(f, scope) {
                // Move the local function from the top-level function to the declaring one.
                f.local_functions.retain(|&l| l != definition);
                f.add_local_function(declaring, definition);
                f.release_root(definition);
            }
        }
    }

    /// `DetermineCaptureAndDeclarationScope(info, useSite)`.
    fn determine_use_site(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        i: usize,
        use_site: NodeId,
    ) {
        let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = r.function.inst(use_site)
        else {
            // An `ldftn` needs a static local function, declared at the top level.
            return;
        };
        // A delegate construction's capture scope came from its target.
        if match_delegate_construction(&r.function, ctx, use_site) {
            return;
        }
        let args = c.args.clone();
        let definition = self.infos[i].definition.expect("read");
        let first = if function_method_is_static(ctx.ts, &r.function, definition) {
            0
        } else {
            1
        };
        for index in (first..args.len()).rev() {
            if !self.determine_argument(r, ctx, i, index as i32 - first as i32, args[index]) {
                break;
            }
        }
        if first > 0 {
            self.determine_argument(r, ctx, i, -1, args[0]);
        }
    }

    /// `DetermineCaptureAndDeclarationScope(info, parameterIndex, arg)`.
    fn determine_argument(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        i: usize,
        parameter_index: i32,
        arg: NodeId,
    ) -> bool {
        let definition = self.infos[i].definition.expect("read");
        if parameter_index >= 0 {
            let params = function_params(ctx.ts, &r.function, definition);
            match params.get(parameter_index as usize) {
                Some(&p) if self.is_closure_parameter(ctx, p) => {}
                _ => return false,
            }
        }
        let mut closure_variable = match *r.function.inst(arg) {
            Inst::LdLoc { var } | Inst::LdLoca { var } => var,
            _ => match self.resolve_ancestor_scope_reference(r, ctx, arg) {
                Some(v) => v,
                None => return false,
            },
        };
        let kind = r.function.var(closure_variable).kind;
        if kind == VariableKind::NamedArgument {
            return false;
        }
        if parameter_index >= 0 && kind == VariableKind::Parameter {
            // The use site is in another local function forwarding the display class it got
            // as a parameter; the scope it was created in comes from the variable holding it.
            let ty = unwrap_by_ref(ctx.ts, r.function.var(closure_variable).ty);
            closure_variable = self
                .find_closure_variable_of_type(r, ctx, ty)
                .unwrap_or(closure_variable);
        }
        let Some(initializer) = closure_initializer(r, ctx, closure_variable) else {
            return false;
        };
        let f = &mut r.function;
        let additional = closest_container(f, initializer);
        let capture_scope = match (f.var(closure_variable).capture_scope, additional) {
            (None, a) => a,
            (Some(s), Some(a)) => common_container(f, s, a),
            (Some(s), None) => Some(s),
        };
        f.var_mut(closure_variable).capture_scope = capture_scope;
        if f.var(closure_variable).kind == VariableKind::Local {
            f.var_mut(closure_variable).kind = VariableKind::DisplayClassLocal;
        }
        let Some(capture_scope) = capture_scope else {
            return true;
        };
        match declaration_scope(f, definition) {
            None => set_declaration_scope(f, definition, Some(capture_scope)),
            Some(scope) if is_descendant_of(f, capture_scope, scope) => {
                // One capture scope inside the other: the declaration belongs in the inner one.
                set_declaration_scope(f, definition, Some(capture_scope));
            }
            Some(scope) => {
                let owner = enclosing_function_inclusive(f, capture_scope);
                if !is_descendant_of(f, scope, capture_scope)
                    && !is_in_nested_local_function(f, scope, owner)
                {
                    let common = common_container(f, scope, capture_scope);
                    set_declaration_scope(f, definition, common);
                }
            }
        }
        true
    }

    /// `PropagateClosureParameterArguments`.
    fn propagate_closure_parameter_arguments(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) {
        for function in all_functions(&r.function) {
            let Inst::Function {
                kind: FunctionKind::LocalFunction,
                read: Some(read),
                ..
            } = r.function.inst(function)
            else {
                continue;
            };
            let Some(i) = self.info(read.method) else {
                continue;
            };
            for u in self.infos[i].use_sites.clone() {
                match r.function.inst(u) {
                    Inst::NewObj(c) => {
                        let target = c.args[0];
                        self.add_as_argument(r, ctx, i, -1, target);
                    }
                    Inst::Call(c) | Inst::CallVirt(c) => {
                        let args = c.args.clone();
                        let first = if self.infos[i].method.is_static {
                            0
                        } else {
                            self.add_as_argument(r, ctx, i, -1, args[0]);
                            1
                        };
                        for index in (first..args.len()).rev() {
                            self.add_as_argument(
                                r,
                                ctx,
                                i,
                                index as i32 - first as i32,
                                args[index],
                            );
                        }
                    }
                    // `&LocalFunction` needs a static local function: no closure parameters.
                    _ => {}
                }
            }
            let definition = self.infos[i]
                .definition
                .expect("a local function in the tree was read");
            for (index, arguments) in self.infos[i].arguments.clone() {
                let Some(target) = function_variables(&r.function, definition)
                    .into_iter()
                    .find(|&v| {
                        let var = r.function.var(v);
                        var.kind == VariableKind::Parameter && var.index == Some(index)
                    })
                else {
                    continue;
                };
                let Some(argument) = self.find_compatible_argument(r, ctx, i, &arguments, false)
                else {
                    continue;
                };
                replace_delegate_target(&mut r.function, definition, argument, Some(target));
            }
        }
    }

    /// `AddAsArgument`.
    fn add_as_argument(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        i: usize,
        index: i32,
        argument: NodeId,
    ) {
        let is_closure = |this: &Self, ctx: &mut Context<'_, '_, '_>| {
            let params = this.infos[i].method.params().unwrap_or_default();
            params
                .get(index as usize)
                .is_some_and(|&p| this.is_closure_parameter(ctx, p))
        };
        match r.function.inst(argument) {
            Inst::LdLoc { .. } | Inst::LdLoca { .. } | Inst::LdFlda { .. } | Inst::LdObj { .. } => {
                if index >= 0 && !is_closure(self, ctx) {
                    return;
                }
            }
            _ => {
                if index >= 0 && is_closure(self, ctx) {
                    let definition = self.infos[i].definition.expect("read");
                    if let Inst::Function {
                        read: Some(read), ..
                    } = r.function.inst_mut(definition)
                    {
                        read.warnings.push(format!(
                            "Could not transform parameter {index}: unsupported argument pattern"
                        ));
                    }
                }
                return;
            }
        }
        let arguments = &mut self.infos[i].arguments;
        match arguments.iter_mut().find(|(k, _)| *k == index) {
            Some((_, list)) => list.push(argument),
            None => arguments.push((index, vec![argument])),
        }
    }

    /// `FindCompatibleArgument`: a node to substitute (possibly a new `ldloc` outside the tree).
    fn find_compatible_argument(
        &self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        i: usize,
        arguments: &[NodeId],
        ignore_structure: bool,
    ) -> Option<NodeId> {
        let definition = self.infos[i].definition.expect("read");
        for &arg in arguments {
            if let Some(v) = variable_operand(&r.function, arg) {
                let owner = variable_function(r, v);
                if ignore_structure
                    || owner.is_none_or(|o| is_descendant_of(&r.function, definition, o))
                {
                    return Some(arg);
                }
            }
            if let Some(v) = self.resolve_ancestor_scope_reference(r, ctx, arg) {
                return Some(r.function.add(Inst::LdLoc { var: v }));
            }
        }
        None
    }

    /// `ResolveAncestorScopeReference`: the display class variable an `ldfld` of an outer
    /// display class field refers to.
    fn resolve_ancestor_scope_reference(
        &self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        inst: NodeId,
    ) -> Option<VarId> {
        let (_, field) = r.function.match_ldfld(ctx.ts, inst)?;
        let ty = field.field.ty;
        let def = ctx.ts.definition(ty)?;
        if ctx.ts.comp.type_def(def).ok()?.kind != TypeKind::Class {
            return None;
        }
        let own_type = ctx
            .ts
            .method_definition(r.method)
            .ok()
            .and_then(|m| m.declaring);
        if !(ctx.ts.is_potential_closure_type(self.decompiled_type, def) || own_type == Some(ty)) {
            return None;
        }
        self.find_closure_variable_of_type(r, ctx, ty)
    }

    /// `FindClosureVariableOfType`: the variable holding a display class of exactly `ty`,
    /// anywhere in the function tree.
    fn find_closure_variable_of_type(
        &self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
        ty: TypeId,
    ) -> Option<VarId> {
        let mut variables = r.variables.clone();
        for function in all_functions(&r.function) {
            variables.extend(function_variables(&r.function, function));
        }
        variables.into_iter().find(|&v| {
            is_closure(r, ctx, self.decompiled_type, v).is_some_and(|d| ctx.ts.intern_def(d) == ty)
        })
    }

    /// `TransformUseSites`.
    fn transform_use_sites(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        for info in &self.infos {
            let Some(definition) = info.definition else {
                continue;
            };
            let Some(reference) = local_function_ref(&r.function, definition) else {
                continue;
            };
            for &u in &info.use_sites {
                match r.function.inst(u).clone() {
                    Inst::NewObj(c) => {
                        transform_to_local_function_reference(r, &reference, c.args[0], c.args[1])
                    }
                    Inst::Call(c) | Inst::CallVirt(c) => {
                        transform_to_local_function_invocation(r, ctx, &reference, u, c)
                    }
                    Inst::LdFtn { method } => {
                        let range = r.function.il_range(u);
                        let replacement = r.function.add(Inst::LdFtn {
                            method: reduce_operand(&method, &reference),
                        });
                        r.function.set_il_range(replacement, range);
                        r.function.replace(u, replacement);
                    }
                    _ => unreachable!("use sites are calls, ldftn or delegate constructions"),
                }
            }
        }
    }

    /// `IsClosureParameter`: a by-reference potential display struct.
    fn is_closure_parameter(&self, ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> bool {
        let TypeData::ByRef(element) = *ctx.ts.data(ty) else {
            return false;
        };
        ctx.ts.definition(element).is_some_and(|def| {
            ctx.ts
                .comp
                .type_def(def)
                .is_ok_and(|i| i.kind == TypeKind::Struct)
                && ctx.ts.is_potential_closure_type(self.decompiled_type, def)
        })
    }
}

/// `TransformToLocalFunctionReference`: `newobj D(target, ldftn M)` becomes
/// `newobj D(ldnull, ldftn LocalFunction)`.
fn transform_to_local_function_reference(
    r: &mut ReadFunction,
    reference: &LocalFunctionRef,
    target: NodeId,
    fnptr: NodeId,
) {
    let f = &mut r.function;
    let null = f.add(Inst::LdNull);
    let range = f.il_range(target);
    f.set_il_range(null, range);
    f.replace(target, null);
    if let Some(v) = variable_operand(f, target)
        && f.var(v).kind == VariableKind::Local
    {
        f.var_mut(v).kind = VariableKind::DisplayClassLocal;
    }
    let Inst::LdFtn { method } = f.inst(fnptr).clone() else {
        return;
    };
    let replacement = f.add(Inst::LdFtn {
        method: reduce_operand(&method, reference),
    });
    let range = f.il_range(fnptr);
    f.set_il_range(replacement, range);
    f.replace(fnptr, replacement);
}

/// `TransformToLocalFunctionInvocation`: the use site becomes a call of the reduced method,
/// without the instance and the closure arguments.
fn transform_to_local_function_invocation(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    reference: &LocalFunctionRef,
    use_site: NodeId,
    call: CallData,
) {
    let f = &mut r.function;
    let was_instance_call = !call.method.method.is_static;
    let first = usize::from(was_instance_call);
    let count = call.args.len();
    let reduced_count = count
        .saturating_sub(reference.reduced.number_of_compiler_generated_parameters as usize + first);
    let args: Vec<NodeId> = call.args[first..first + reduced_count].to_vec();
    let mut ranges = vec![f.il_range(use_site)];
    if was_instance_call {
        let receiver = call.args[0];
        ranges.push(f.il_range(receiver));
        if let Some(v) = match_ld_loc_ref(f, ctx, receiver)
            && f.var(v).kind == VariableKind::NamedArgument
            && let Some(&store) = f.var(v).store_instructions.first()
        {
            // The store of a named argument's simple load goes.
            f.remove(store);
        }
    }
    for i in 0..reference.reduced.number_of_compiler_generated_parameters as usize {
        ranges.push(f.il_range(call.args[count - i - 1]));
    }
    let replacement = f.add(Inst::Call(CallData {
        method: reduce_operand(&call.method, reference),
        args,
        is_tail: call.is_tail,
        constrained_to: call.constrained_to,
        il_stack_was_empty: call.il_stack_was_empty,
    }));
    for range in ranges {
        f.add_il_range(replacement, range);
    }
    f.replace(use_site, replacement);
}

/// `reducedMethod.Specialize(useSite.Method.Substitution)`: the use site's method as a use of
/// the local function; its parameters lose the closure parameters.
fn reduce_operand(method: &MethodOperand, reference: &LocalFunctionRef) -> MethodOperand {
    let mut operand = method.clone();
    let keep = operand
        .method
        .base_params
        .len()
        .saturating_sub(reference.reduced.number_of_compiler_generated_parameters as usize);
    operand.method.base_params.truncate(keep);
    operand.local_function = Some(Box::new(reference.clone()));
    operand
}

/// The `LocalFunctionRef` of a read local function.
fn local_function_ref(f: &Function, function: NodeId) -> Option<LocalFunctionRef> {
    match f.inst(function) {
        Inst::Function {
            read: Some(read),
            reduced: Some(reduced),
            ..
        } => Some(LocalFunctionRef {
            method: read.method,
            context: read.context,
            reduced: *reduced,
        }),
        _ => None,
    }
}

/// `GenericContextFromTypeArguments(targetMethod, skipCount)`: the local function's own
/// context with its first `skip_count` type parameters (after the class's) replaced by the
/// use site's type arguments; `Some(None)` for the method's own context.
fn generic_context_from_type_arguments(
    ts: &mut TypeSystem<'_, '_>,
    target: &MethodOperand,
    id: MethodDefId,
    skip_count: u32,
) -> Option<Option<GenericContext>> {
    let definition = ts.method_definition(id).ok()?;
    let own_method: Vec<TypeId> = definition.type_args.clone();
    if skip_count as usize > own_method.len() {
        return None;
    }
    let declaring_def = definition.declaring.and_then(|d| ts.definition(d));
    let own_class: Vec<TypeId> = match declaring_def {
        Some(d) => {
            let own = ts.intern_def(d);
            ts.type_arguments(own)
        }
        None => Vec::new(),
    };
    let total = own_class.len() + skip_count as usize;
    if total == 0 {
        return Some(None);
    }
    let class_args = target
        .method
        .class_args
        .clone()
        .unwrap_or_else(|| own_class.clone());
    let method_args = target
        .method
        .method_args
        .clone()
        .unwrap_or_else(|| own_method.clone());
    let mut class = own_class.clone();
    let mut method = own_method;
    let mut index = 0;
    for skipped in class_args.iter().chain(&method_args).take(total).copied() {
        let is_type_parameter = matches!(ts.data(skipped), TypeData::TypeParam { .. });
        if !is_type_parameter {
            break;
        }
        if index < class.len() {
            class[index] = skipped;
        } else {
            method[index - class.len()] = skipped;
        }
        index += 1;
    }
    if index != total {
        return None;
    }
    // The type parameters not replaced are the use site method's (`targetMethod.TypeParameters`).
    let first = skip_count;
    let of = ((first as usize) < method.len()).then_some((target.token, target.context, first));
    if of.is_some() {
        // The context about to be added; its own type parameters refer to it.
        let context = ts.explicit_contexts().len() as u32;
        for (i, t) in method.iter_mut().enumerate().skip(first as usize) {
            *t = ts.intern(TypeData::TypeParam {
                owner: TypeParamOwner::ContextMethod {
                    method: id,
                    context,
                },
                index: i as u32,
            });
        }
    }
    Some(Some(ts.explicit_context_of(class, method, of)))
}

/// Moves a read local function into `r`'s arena (its body still holding the reference of its
/// old root).
fn import_function(
    r: &mut ReadFunction,
    mut read: ReadFunction,
    reduced: ReducedMethod,
    return_type: TypeId,
    parameters: Vec<TypeId>,
) -> NodeId {
    let body = read.function.body.expect("a read function has a body");
    let captured = std::mem::take(&mut read.function.captured_variables);
    let helper_variable_count = read.function.helper_variable_count;
    let by = r.function.import(std::mem::take(&mut read.function));
    let context = context_index(&read.generic_context);
    let info = NestedRead {
        method: read.method,
        context,
        code_size: read.code_size,
        local_signature_length: read.local_signature_length,
        async_return_type: read.async_return_type,
        is_iterator: read.is_iterator,
        state_machine_compiled_with_mono: read.state_machine_compiled_with_mono,
        state_machine_compiled_with_legacy_visual_basic: read
            .state_machine_compiled_with_legacy_visual_basic,
        move_next_method: read.move_next_method,
        async_debug_info: read.async_debug_info,
        warnings: read.warnings,
        sequence_point_candidates: read.sequence_point_candidates,
        method_operand: None,
    };
    let function = r.function.add(Inst::Function {
        kind: FunctionKind::LocalFunction,
        delegate_type: None,
        return_type,
        parameters: parameters.into_iter().map(|t| (t, String::new())).collect(),
        variables: read.variables.iter().map(|&v| by.var(v)).collect(),
        helper_variable_count,
        body: by.node(body),
        read: Some(Box::new(info)),
        local_functions: Vec::new(),
        declaration_scope: None,
        reduced: Some(reduced),
    });
    for (owner, v) in captured {
        let owner = owner.map_or(function, |n| by.node(n));
        r.function.add_captured_variable(Some(owner), by.var(v));
    }
    function
}

/// `MethodOperand::context` of the context a function was read in.
fn context_index(context: &GenericContext) -> u32 {
    match (context.explicit, context.class_tail) {
        (Some(e), _) => e + 2,
        (None, Some(_)) => 1,
        (None, None) => 0,
    }
}

/// `IsClosure`: the display class type of a variable holding a closure.
fn is_closure(
    r: &ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    decompiled: Option<TypeDefId>,
    v: VarId,
) -> Option<TypeDefId> {
    let f = &r.function;
    let var = f.var(v);
    if var.is_single_definition()
        && let [store] = var.store_instructions[..]
        && let Some((_, value)) = f.match_stloc(store)
        && let Inst::NewObj(c) = f.inst(value)
        && let Some(def) = c.method.method.declaring.and_then(|d| ctx.ts.definition(d))
        && ctx.ts.is_potential_closure_type(decompiled, def)
    {
        return Some(def);
    }
    let def = ctx.ts.definition(var.ty)?;
    (ctx.settings.local_functions
        && ctx
            .ts
            .comp
            .type_def(def)
            .is_ok_and(|i| i.kind == TypeKind::Struct)
        && var.uses_initial_value
        && ctx.ts.is_potential_closure_type(decompiled, def))
    .then_some(def)
}

/// `GetClosureInitializer`.
fn closure_initializer(
    r: &ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    v: VarId,
) -> Option<NodeId> {
    let f = &r.function;
    let var = f.var(v);
    let def = ctx.ts.definition(unwrap_by_ref(ctx.ts, var.ty))?;
    if var.kind == VariableKind::Parameter {
        return None;
    }
    if ctx.ts.comp.type_def(def).ok()?.kind == TypeKind::Struct {
        // The first address use by IL offset.
        let first = var
            .address_instructions
            .iter()
            .copied()
            .min_by_key(|&a| f.il_range(a).start)?;
        return containing_statement(f, first);
    }
    var.store_instructions.first().copied()
}

/// `IsLocalFunctionMethod(module, methodHandle)`: an assembly-visible method named like a
/// local function (`<caller>g__name|x_y`) that is, or is declared in, compiler-generated
/// code, or a method taking a compiler-generated display struct by reference.
pub(crate) fn is_local_function_method(ts: &mut TypeSystem<'_, '_>, method: MethodDefId) -> bool {
    let md = &ts.comp.module(method.module).md;
    let Ok(def) = md.get::<rows::MethodDef>(method.row) else {
        return false;
    };
    // `(method.Attributes & MethodAttributes.Assembly) == 0`.
    if def.flags & 0x0003 == 0 {
        return false;
    }
    let declaring = md.method_declaring_type(method.row).unwrap_or(0);
    let compiler_generated =
        ts.has_compiler_generated(method.module, TableId::MethodDef, method.row)
            || ts.has_compiler_generated(method.module, TableId::TypeDef, declaring);
    if compiler_generated
        && md
            .string(def.name)
            .is_ok_and(|n| parse_local_function_name(&n))
    {
        return true;
    }
    // Obfuscators strip the attribute and rename the method, but a compiler-generated struct
    // closure is still passed by reference to the local functions that capture it.
    has_display_struct_parameter(ts, method)
}

/// `IsLocalFunctionDisplayClass`: a private nested type with a generated name whose declaring
/// type has a local function method with parameters. (The managed code decodes the signature
/// with a `FindTypeDecoder` for the type but then only asks `ParameterTypes.Any()`, so any
/// parameter counts.)
fn is_local_function_display_class(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    let Ok(info) = ts.comp.type_def(def) else {
        return false;
    };
    // `TypeAttributes.NestedPrivate`.
    if info.flags & 0x7 != 0x3 || !has_generated_type_name(ts, def) || info.declaring == 0 {
        return false;
    }
    let declaring = TypeDefId {
        module: def.module,
        row: info.declaring,
    };
    let md = &ts.comp.module(def.module).md;
    let Ok(methods) = md.type_methods(declaring.row) else {
        return false;
    };
    methods.into_iter().any(|row| {
        let id = MethodDefId {
            module: def.module,
            row,
        };
        is_local_function_method(ts, id)
            && ts
                .method_definition(id)
                .is_ok_and(|m| !m.base_params.is_empty())
    })
}

/// `HasDisplayStructParameter`: a by-reference parameter of a compiler-generated value type
/// with a generated name, defined in the method's module.
fn has_display_struct_parameter(ts: &mut TypeSystem<'_, '_>, method: MethodDefId) -> bool {
    let Ok(definition) = ts.method_definition(method) else {
        return false;
    };
    definition.base_params.iter().any(|&p| {
        let TypeData::ByRef(element) = *ts.data(p) else {
            return false;
        };
        let Some(def) = ts.definition(element) else {
            return false;
        };
        def.module == method.module
            && ts.has_compiler_generated(def.module, TableId::TypeDef, def.row)
            && ts
                .comp
                .type_def(def)
                .is_ok_and(|i| i.kind == TypeKind::Struct)
            && has_generated_type_name(ts, def)
    })
}

/// `TypeDefinitionHandle.HasGeneratedName`: `<` first or `$` anywhere in the metadata name.
fn has_generated_type_name(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    let md = &ts.comp.module(def.module).md;
    md.get::<rows::TypeDef>(def.row)
        .ok()
        .and_then(|t| md.string(t.name).ok())
        .is_some_and(|n| n.starts_with('<') || n.contains('$'))
}

fn has_instance_fields(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    let md = &ts.comp.module(def.module).md;
    md.type_fields(def.row).is_ok_and(|fields| {
        fields.into_iter().any(|row| {
            md.get::<rows::Field>(row)
                .is_ok_and(|f| f.flags & 0x0010 == 0)
        })
    })
}

/// `ParseLocalFunctionName`: whether `name` matches `^<(.*)>g__([^|]*)\|?\d+(_\d+)?$`, the
/// newer Roslyn format `<caller>g__name|x_y` or the older `<caller>g__namex_y`.
pub(crate) fn parse_local_function_name(name: &str) -> bool {
    let Some(rest) = name.strip_prefix('<') else {
        return false;
    };
    rest.match_indices(">g__").any(|(i, _)| {
        let tail = &rest[i + 4..];
        match tail.split_once('|') {
            // The name cannot contain `|`, so the separator is the optional one.
            Some((_, numbers)) => is_digits_and_suffix(numbers),
            // Without it the name gives back trailing characters until `\d+(_\d+)?` matches,
            // which it does exactly when the tail ends in a digit.
            None => tail.ends_with(|c: char| c.is_ascii_digit()),
        }
    })
}

/// `\d+(_\d+)?` matched against the whole of `s`.
fn is_digits_and_suffix(s: &str) -> bool {
    let digits = |s: &str| !s.is_empty() && s.bytes().all(|b| b.is_ascii_digit());
    match s.split_once('_') {
        Some((a, b)) => digits(a) && digits(b),
        None => digits(s),
    }
}

/// `MatchLdLocRef`: `ldloc` of a reference type variable, or `ldloca` of another.
fn match_ld_loc_ref(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::LdLoc { var } => {
            (ctx.ts.is_reference_type(f.var(var).ty) == Some(true)).then_some(var)
        }
        Inst::LdLoca { var } => {
            let ty = f.var(var).ty;
            let is_type_parameter = matches!(ctx.ts.data(ty), TypeData::TypeParam { .. });
            (ctx.ts.is_reference_type(ty) != Some(true) || is_type_parameter).then_some(var)
        }
        _ => None,
    }
}

/// The variable of an `IInstructionWithVariableOperand`.
fn variable_operand(f: &Function, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::LdLoc { var } | Inst::LdLoca { var } | Inst::StLoc { var, .. } => Some(var),
        _ => None,
    }
}

/// `ILVariable.Function`: the nested function listing `v`, `None` for the top-level one.
fn variable_function(r: &ReadFunction, v: VarId) -> Option<NodeId> {
    if r.variables.contains(&v) {
        return None;
    }
    all_functions(&r.function)
        .into_iter()
        .find(|&n| function_variables(&r.function, n).contains(&v))
}

/// The nested functions of the arena's function tree in `Descendants` order: the body's, then
/// the top-level local functions with theirs.
fn all_functions(f: &Function) -> Vec<NodeId> {
    f.body
        .into_iter()
        .chain(f.local_functions.iter().copied())
        .flat_map(|root| f.descendants_post(root))
        .filter(|&n| matches!(f.inst(n), Inst::Function { .. }))
        .collect()
}

fn function_variables(f: &Function, function: NodeId) -> Vec<VarId> {
    match f.inst(function) {
        Inst::Function { variables, .. } => variables.clone(),
        _ => Vec::new(),
    }
}

fn is_this(f: &Function, v: VarId) -> bool {
    let var = f.var(v);
    var.kind == VariableKind::Parameter && var.index == Some(-1)
}

/// `function.Method.IsStatic` of a read nested function.
fn function_method_is_static(ts: &TypeSystem<'_, '_>, f: &Function, function: NodeId) -> bool {
    let Inst::Function {
        read: Some(read), ..
    } = f.inst(function)
    else {
        return false;
    };
    let md = &ts.comp.module(read.method.module).md;
    md.get::<rows::MethodDef>(read.method.row)
        .is_ok_and(|d| d.flags & 0x0010 != 0)
}

/// `function.Method.Parameters` types of a read nested function: its parameter variables.
fn function_params(ts: &TypeSystem<'_, '_>, f: &Function, function: NodeId) -> Vec<TypeId> {
    let _ = ts;
    let mut params: Vec<(i32, TypeId)> = function_variables(f, function)
        .into_iter()
        .filter_map(|v| {
            let var = f.var(v);
            match (var.kind, var.index) {
                (VariableKind::Parameter, Some(i)) if i >= 0 => Some((i, var.ty)),
                _ => None,
            }
        })
        .collect();
    params.sort_by_key(|&(i, _)| i);
    params.into_iter().map(|(_, t)| t).collect()
}

/// The parameter types of a read function's (specialized) method.
fn read_method_params(ts: &TypeSystem<'_, '_>, read: &ReadFunction) -> Vec<TypeId> {
    let _ = ts;
    let mut params: Vec<(i32, TypeId)> = read
        .variables
        .iter()
        .filter_map(|&v| {
            let var = read.function.var(v);
            match (var.kind, var.index) {
                (VariableKind::Parameter, Some(i)) if i >= 0 => Some((i, var.ty)),
                _ => None,
            }
        })
        .collect();
    params.sort_by_key(|&(i, _)| i);
    params.into_iter().map(|(_, t)| t).collect()
}

fn unwrap_by_ref(ts: &TypeSystem<'_, '_>, t: TypeId) -> TypeId {
    match *ts.data(t) {
        TypeData::ByRef(e) => e,
        _ => t,
    }
}

fn declaring_type(ts: &TypeSystem<'_, '_>, method: MethodDefId) -> Option<TypeDefId> {
    let md = &ts.comp.module(method.module).md;
    let row = md.method_declaring_type(method.row).ok()?;
    (row != 0).then_some(TypeDefId {
        module: method.module,
        row,
    })
}

fn declaration_scope(f: &Function, function: NodeId) -> Option<NodeId> {
    match f.inst(function) {
        Inst::Function {
            declaration_scope, ..
        } => *declaration_scope,
        _ => None,
    }
}

fn set_declaration_scope(f: &mut Function, function: NodeId, scope: Option<NodeId>) {
    if let Inst::Function {
        declaration_scope, ..
    } = f.inst_mut(function)
    {
        *declaration_scope = scope;
    }
}

/// `GetDeclaringFunction`: the nearest function at or above `node`, `None` for the top level.
fn enclosing_function_inclusive(f: &Function, node: NodeId) -> Option<NodeId> {
    std::iter::once(node)
        .chain(f.ancestors(node))
        .find(|&n| matches!(f.inst(n), Inst::Function { .. }))
}

/// `IsDescendantOf`: `node` is `ancestor` or below it.
fn is_descendant_of(f: &Function, node: NodeId, ancestor: NodeId) -> bool {
    node == ancestor || f.ancestors(node).any(|a| a == ancestor)
}

/// `IsInNestedLocalFunction`: whether `scope` is in the body of `function` (`None` for the
/// top-level one) or of one of its local functions, at any depth.
fn is_in_nested_local_function(f: &Function, scope: NodeId, function: Option<NodeId>) -> bool {
    let (body, local_functions) = match function.map(|n| f.inst(n)) {
        Some(Inst::Function {
            body,
            local_functions,
            ..
        }) => (Some(*body), local_functions.clone()),
        _ => (f.body, f.local_functions.clone()),
    };
    body.is_some_and(|b| is_descendant_of(f, scope, b))
        || local_functions
            .into_iter()
            .any(|l| is_in_nested_local_function(f, scope, Some(l)))
}

/// `BlockContainer.FindClosestContainer`.
fn closest_container(f: &Function, inst: NodeId) -> Option<NodeId> {
    std::iter::once(inst)
        .chain(f.ancestors(inst))
        .find(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
}

/// `FindCommonAncestorInstruction<BlockContainer>(a, b)`.
fn common_container(f: &Function, a: NodeId, b: NodeId) -> Option<NodeId> {
    let of_b: Vec<NodeId> = std::iter::once(b)
        .chain(f.ancestors(b))
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .collect();
    std::iter::once(a)
        .chain(f.ancestors(a))
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .find(|n| of_b.contains(n))
}

/// `Block.GetContainingStatement`: the ancestor (or `inst`) directly in a control flow block.
fn containing_statement(f: &Function, inst: NodeId) -> Option<NodeId> {
    let mut current = inst;
    loop {
        let parent = f.node(current).parent?;
        if matches!(
            f.inst(parent),
            Inst::Block {
                kind: crate::il::ast::BlockKind::ControlFlow,
                ..
            }
        ) {
            return Some(current);
        }
        current = parent;
    }
}

#[cfg(test)]
mod tests {
    use super::parse_local_function_name;

    #[test]
    fn local_function_names_match_the_managed_regex() {
        assert!(parse_local_function_name("<Main>g__Local|0_0"));
        assert!(parse_local_function_name("<Main>g__Local0_1"));
        assert!(parse_local_function_name("<Main>g__Local|12"));
        assert!(parse_local_function_name("<<Main>b__0>g__Inner|0_1"));
        assert!(parse_local_function_name("<M>g__|3"));
        assert!(!parse_local_function_name("<Main>g__Local|0_"));
        assert!(!parse_local_function_name("<Main>g__Local|a"));
        assert!(!parse_local_function_name("<Main>b__0_0"));
        assert!(!parse_local_function_name("Main>g__Local|0_0"));
        assert!(!parse_local_function_name("<Main>g__Local"));
    }
}
