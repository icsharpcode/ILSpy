//! `IL/Transforms/IntroduceScopedModifierOnLocals.cs`: the C# `scoped` modifier on locals,
//! which IL and PDBs erase. A ref local (or ref struct local) needs it when a later store has
//! a narrower ref-safe context (or safe context) than its declaration initializer. The
//! contexts, narrowest first: the current method, return-only, the calling method.

use std::collections::HashMap;

use crate::il::ast::{BlockKind, Function, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::metadata::tables::rows;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::flags::{ScopedKind, uses_updated_escape_rules};
use crate::typesystem::members::{MethodDefId, RefKind};
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin, ResolvedField, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::Context;
use super::high_level_loop_transform::is_before;
use super::local_function_decompiler::{all_functions, function_variables};

/// `SafeContext`, narrowest first.
#[derive(Clone, Copy, Debug, PartialEq, Eq, PartialOrd, Ord)]
enum SafeContext {
    CurrentMethod,
    ReturnOnly,
    CallingMethod,
}

/// `LocalScopes`.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
struct LocalScopes {
    ref_escape: Option<SafeContext>,
    val_escape: Option<SafeContext>,
    is_scoped: bool,
    is_scoped_without_initializer: bool,
}

/// What the analysis needs of a parameter (`IParameter`).
struct Param {
    ty: TypeId,
    ref_kind: RefKind,
    effective_scope: ScopedKind,
    unscoped_ref: bool,
    /// `parameter.Owner?.ParentModule` uses the updated escape rules.
    updated_rules: bool,
}

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.scoped_ref {
        return;
    }
    // `function.Descendants.OfType<ILFunction>()`: the nested functions, the top-level one last.
    let mut functions: Vec<Option<NodeId>> =
        all_functions(&r.function).into_iter().map(Some).collect();
    functions.push(None);
    for function in functions {
        let method = match function {
            None => Some(r.method),
            Some(n) => match r.function.inst(n) {
                Inst::Function {
                    read: Some(read), ..
                } => Some(read.method),
                _ => None,
            },
        };
        // Only methods of modules with the updated escape rules.
        let Some(method) = method else {
            continue;
        };
        if !uses_updated_escape_rules(&ctx.ts.comp.module(method.module).md) {
            continue;
        }
        let Ok(resolved) = ctx.ts.method_definition(method) else {
            continue;
        };
        let mut a = Analyzer {
            function,
            method,
            resolved,
            stores: Vec::new(),
            field_stores: HashMap::new(),
            call_captures: HashMap::new(),
            scopes: HashMap::new(),
        };
        a.run(r, ctx);
    }
}

struct Analyzer {
    function: Option<NodeId>,
    method: MethodDefId,
    resolved: ResolvedMethod,
    /// `_stores`, in insertion order.
    stores: Vec<(VarId, Vec<NodeId>)>,
    field_stores: HashMap<VarId, Vec<NodeId>>,
    call_captures: HashMap<VarId, Vec<(NodeId, usize)>>,
    scopes: HashMap<VarId, LocalScopes>,
}

impl Analyzer {
    fn has_stores(&self, v: VarId) -> bool {
        self.stores.iter().any(|(w, _)| *w == v)
    }

    fn run(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        let variables = match self.function {
            None => r.variables.clone(),
            Some(n) => function_variables(&r.function, n),
        };
        for &v in &variables {
            let var = r.function.var_mut(v);
            var.is_scoped = false;
            var.is_scoped_without_initializer = false;
            let var = r.function.var(v);
            if !matches!(var.kind, VariableKind::Local | VariableKind::StackSlot)
                || !ctx.ts.is_ref_like_or_allows(var.ty)
                || var.uses_initial_value
            {
                continue;
            }
            let stores: Vec<NodeId> = var
                .store_instructions
                .iter()
                .copied()
                .filter(|&s| matches!(r.function.inst(s), Inst::StLoc { .. }))
                .collect();
            if stores.is_empty() {
                continue;
            }
            self.stores.push((v, stores));
            self.scopes.insert(v, LocalScopes::default());
        }
        let nodes = self.descendants(&r.function);
        for &store in &nodes {
            let Inst::StObj { target, .. } = *r.function.inst(store) else {
                continue;
            };
            for v in distinct(self.storage_roots(&r.function, ctx.ts, target)) {
                if self.has_stores(v) {
                    self.field_stores.entry(v).or_default().push(store);
                }
            }
        }
        for &call in &nodes {
            let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = r.function.inst(call)
            else {
                continue;
            };
            for index in 0..c.args.len() {
                if !is_writable_ref_struct_argument(&r.function, ctx.ts, call, index) {
                    continue;
                }
                let arg = call_args(&r.function, call)[index];
                for v in distinct(self.storage_roots(&r.function, ctx.ts, arg)) {
                    if self.has_stores(v) {
                        self.call_captures.entry(v).or_default().push((call, index));
                    }
                }
            }
        }
        for _ in 0..=self.stores.len() * 3 {
            let mut changed = false;
            let mut next = self.scopes.clone();
            for (v, stores) in self.stores.clone() {
                let initializer = first_store(&r.function, &stores);
                let scopes =
                    self.calculate_local_scopes(&r.function, ctx.ts, v, initializer, &stores);
                if scopes != self.scopes[&v] {
                    next.insert(v, scopes);
                    changed = true;
                }
            }
            self.scopes = next;
            if !changed {
                break;
            }
        }
        for (&v, scopes) in &self.scopes {
            let var = r.function.var_mut(v);
            var.is_scoped = scopes.is_scoped;
            var.is_scoped_without_initializer = scopes.is_scoped_without_initializer;
        }
    }

    /// `function.Descendants`, in post-order.
    fn descendants(&self, f: &Function) -> Vec<NodeId> {
        match self.function {
            Some(n) => f.descendants_post(n),
            None => f
                .body
                .into_iter()
                .chain(f.local_functions.iter().copied())
                .flat_map(|root| f.descendants_post(root))
                .collect(),
        }
    }

    /// `CalculateLocalScopes`.
    fn calculate_local_scopes(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        v: VarId,
        initializer: NodeId,
        stores: &[NodeId],
    ) -> LocalScopes {
        let ty = f.var(v).ty;
        let is_ref_local = matches!(ts.data(ty), TypeData::ByRef(_));
        let value_of = |s: NodeId| f.match_stloc(s).expect("a store").1;
        let init_value = value_of(initializer);
        let init_ref = if is_ref_local {
            self.ref_escape(f, ts, init_value)
        } else {
            Some(SafeContext::CurrentMethod)
        };
        let value_type = value_type(ts, ty);
        let init_val = if ts.is_ref_like_or_allows(value_type) {
            self.val_escape(f, ts, init_value)
        } else {
            Some(SafeContext::CallingMethod)
        };
        let (Some(init_ref_escape), Some(init_val_escape)) = (init_ref, init_val) else {
            return LocalScopes::default();
        };
        let mut requires_scoped = false;
        let init_escape = if is_ref_local {
            init_ref_escape
        } else {
            init_val_escape
        };
        let mut without_initializer = init_escape < SafeContext::CallingMethod;
        for &store in stores {
            if store == initializer {
                continue;
            }
            let value = value_of(store);
            let escape = if is_ref_local {
                self.ref_escape(f, ts, value)
            } else {
                self.val_escape(f, ts, value)
            };
            let Some(scope) = escape else {
                continue;
            };
            if scope < SafeContext::CallingMethod {
                without_initializer = true;
            }
            if scope < init_escape {
                requires_scoped = true;
            }
        }
        if !requires_scoped && let Some(field_stores) = self.field_stores.get(&v) {
            for &store in field_stores {
                if !is_before(f, initializer, store) {
                    continue;
                }
                let Inst::StObj { value, ty, .. } = *f.inst(store) else {
                    continue;
                };
                let escape = if matches!(ts.data(ty.ty), TypeData::ByRef(_)) {
                    self.ref_escape(f, ts, value)
                } else {
                    self.val_escape(f, ts, value)
                };
                let Some(scope) = escape else {
                    continue;
                };
                if scope < SafeContext::CallingMethod {
                    without_initializer = true;
                }
                if scope < init_val_escape {
                    requires_scoped = true;
                }
            }
        }
        if !requires_scoped && let Some(captures) = self.call_captures.get(&v) {
            for &(call, index) in captures {
                if !is_before(f, initializer, call) {
                    continue;
                }
                let Some(scope) = self.invocation_escape_to_writable_argument(f, ts, call, index)
                else {
                    continue;
                };
                if scope < SafeContext::CallingMethod {
                    without_initializer = true;
                }
                if scope < init_val_escape {
                    requires_scoped = true;
                }
            }
        }
        if !requires_scoped {
            return LocalScopes {
                ref_escape: Some(init_ref_escape),
                val_escape: Some(init_val_escape),
                is_scoped: false,
                is_scoped_without_initializer: without_initializer,
            };
        }
        LocalScopes {
            ref_escape: Some(SafeContext::CurrentMethod),
            val_escape: Some(if is_ref_local {
                SafeContext::CallingMethod
            } else {
                SafeContext::CurrentMethod
            }),
            is_scoped: true,
            is_scoped_without_initializer: true,
        }
    }

    /// `GetStorageRoots`: the locals whose storage an address expression may reach; a ref
    /// returning call may alias any eligible ref argument.
    fn storage_roots(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        target: NodeId,
    ) -> Vec<VarId> {
        match f.inst(target) {
            Inst::LdFlda { target, .. } => self.storage_roots(f, ts, *target),
            Inst::LdElemaInlineArray { array, .. } => self.storage_roots(f, ts, *array),
            Inst::LdLoca { var } => vec![*var],
            Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_) => {
                let args = call_args(f, target).to_vec();
                ref_return_storage_arguments(f, ts, target)
                    .into_iter()
                    .flat_map(|i| self.storage_roots(f, ts, args[i]))
                    .collect()
            }
            _ => Vec::new(),
        }
    }

    /// `GetRefEscape`.
    fn ref_escape(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        value: NodeId,
    ) -> Option<SafeContext> {
        match f.inst(value) {
            Inst::LdLoc { var } => self.variable_scopes(ts, f, *var).ref_escape,
            Inst::LdLoca { .. } | Inst::LocAlloc { .. } | Inst::LocAllocSpan { .. } => {
                Some(SafeContext::CurrentMethod)
            }
            Inst::LdsFlda { .. } | Inst::LdElema { .. } => Some(SafeContext::CallingMethod),
            Inst::LdElemaInlineArray { array, .. } => self.ref_escape(f, ts, *array),
            Inst::LdFlda { target, field, .. } => {
                let (target, field) = (*target, field.field.clone());
                self.field_ref_escape(f, ts, target, &field)
            }
            Inst::LdObjIfRef { target, .. } => self.ref_escape(f, ts, *target),
            Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_) => {
                self.invocation_escape(f, ts, value, true)
            }
            Inst::If {
                true_inst,
                false_inst,
                ..
            } => {
                let (t, e) = (*true_inst, *false_inst);
                intersect(self.ref_escape(f, ts, t), self.ref_escape(f, ts, e))
            }
            Inst::NullCoalescing {
                value, fallback, ..
            } => {
                let (v, fb) = (*value, *fallback);
                intersect(self.ref_escape(f, ts, v), self.ref_escape(f, ts, fb))
            }
            Inst::Block {
                kind, final_inst, ..
            } => {
                if *kind == BlockKind::StackAllocInitializer {
                    Some(SafeContext::CurrentMethod)
                } else {
                    let fi = *final_inst;
                    self.ref_escape(f, ts, fi)
                }
            }
            _ => None,
        }
    }

    /// `GetValEscape`.
    fn val_escape(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        value: NodeId,
    ) -> Option<SafeContext> {
        match f.inst(value) {
            Inst::DefaultValue { .. } | Inst::LdNull | Inst::NewArr { .. } => {
                Some(SafeContext::CallingMethod)
            }
            Inst::LocAlloc { .. } | Inst::LocAllocSpan { .. } => Some(SafeContext::CurrentMethod),
            Inst::LdLoc { var } | Inst::LdLoca { var } => {
                self.variable_scopes(ts, f, *var).val_escape
            }
            Inst::LdsFlda { .. } | Inst::LdElema { .. } => Some(SafeContext::CallingMethod),
            Inst::LdElemaInlineArray { array, .. } => self.val_escape(f, ts, *array),
            Inst::LdFlda { target, field, .. } => {
                let (target, field) = (*target, field.field.clone());
                if field_is_static(ts, &field)
                    || !field.declaring.is_some_and(|d| ts.is_ref_like_or_allows(d))
                {
                    Some(SafeContext::CallingMethod)
                } else {
                    self.val_escape(f, ts, target)
                }
            }
            Inst::LdObj { target, .. } | Inst::LdObjIfRef { target, .. } => {
                self.val_escape(f, ts, *target)
            }
            Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_) => {
                self.invocation_escape(f, ts, value, false)
            }
            Inst::If {
                true_inst,
                false_inst,
                ..
            } => {
                let (t, e) = (*true_inst, *false_inst);
                intersect(self.val_escape(f, ts, t), self.val_escape(f, ts, e))
            }
            Inst::NullCoalescing {
                value, fallback, ..
            } => {
                let (v, fb) = (*value, *fallback);
                intersect(self.val_escape(f, ts, v), self.val_escape(f, ts, fb))
            }
            Inst::Block {
                kind, final_inst, ..
            } => {
                if *kind == BlockKind::StackAllocInitializer {
                    Some(SafeContext::CurrentMethod)
                } else {
                    let fi = *final_inst;
                    self.val_escape(f, ts, fi)
                }
            }
            _ => None,
        }
    }

    /// `GetFieldRefEscape`.
    fn field_ref_escape(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        target: NodeId,
        field: &ResolvedField,
    ) -> Option<SafeContext> {
        if field_is_static(ts, field)
            || field
                .declaring
                .is_some_and(|d| ts.is_reference_type(d) == Some(true))
        {
            return Some(SafeContext::CallingMethod);
        }
        if matches!(ts.data(field.ty), TypeData::ByRef(_)) {
            return self.val_escape(f, ts, target);
        }
        self.ref_escape(f, ts, target)
    }

    /// `GetInvocationEscape`.
    fn invocation_escape(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        call: NodeId,
        is_ref_escape: bool,
    ) -> Option<SafeContext> {
        let method = call_method(f, call).clone();
        let return_type = if matches!(f.inst(call), Inst::NewObj(_)) {
            method.declaring?
        } else {
            method.ret
        };
        let ref_like_return = {
            let v = value_type(ts, return_type);
            ts.is_ref_like_or_allows(v)
        };
        if !is_ref_escape && !ref_like_return {
            return Some(SafeContext::CallingMethod);
        }
        if matches!(
            ts.data(return_type),
            TypeData::Unknown { .. } | TypeData::UnknownSpecial
        ) {
            return None;
        }
        let MethodOrigin::Def(d) = method.origin else {
            return None;
        };
        if uses_updated_escape_rules(&ts.comp.module(d.module).md) {
            self.invocation_escape_with_updated_rules(f, ts, call, is_ref_escape, return_type)
        } else {
            self.invocation_escape_with_old_rules(f, ts, call, is_ref_escape)
        }
    }

    /// `GetInvocationEscapeWithUpdatedRules`.
    fn invocation_escape_with_updated_rules(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        call: NodeId,
        is_ref_escape: bool,
        return_type: TypeId,
    ) -> Option<SafeContext> {
        let method = call_method(f, call).clone();
        let args = call_args(f, call).to_vec();
        let mut result = Some(SafeContext::CallingMethod);
        let returns_ref_to_ref_struct = match *ts.data(return_type) {
            TypeData::ByRef(e) => ts.is_ref_like_or_allows(e),
            _ => false,
        };
        let instance = is_instance_call(f, call);
        for (i, &arg) in args.iter().enumerate() {
            if i == 0 && instance {
                let receiver = method.declaring?;
                if ts.is_ref_like_or_allows(receiver) {
                    let e = self.val_escape(f, ts, arg);
                    result = add_invocation_escape(
                        result,
                        e,
                        false,
                        is_ref_escape,
                        returns_ref_to_ref_struct,
                        true,
                    );
                }
                if ts.is_reference_type(receiver) == Some(false)
                    && this_scope(ts, &method) != ScopedKind::ScopedRef
                {
                    let e = self.ref_escape(f, ts, arg);
                    let is_ref_to_ref_struct = ts.is_ref_like_or_allows(receiver);
                    result = add_invocation_escape(
                        result,
                        e,
                        true,
                        is_ref_escape,
                        returns_ref_to_ref_struct,
                        is_ref_to_ref_struct,
                    );
                }
                continue;
            }
            let p = parameter(f, ts, call, i)?;
            let parameter_type = value_type(ts, p.ty);
            let ref_like = ts.is_ref_like_or_allows(parameter_type);
            let is_ref_to_ref_struct = p.ref_kind != RefKind::None && ref_like;
            if ref_like
                && p.ref_kind != RefKind::Out
                && parameter_val_escape(&p) != SafeContext::CurrentMethod
            {
                let e = self.val_escape(f, ts, arg);
                result = add_invocation_escape(
                    result,
                    e,
                    false,
                    is_ref_escape,
                    returns_ref_to_ref_struct,
                    is_ref_to_ref_struct,
                );
            }
            if p.ref_kind != RefKind::None && parameter_ref_escape(&p) != SafeContext::CurrentMethod
            {
                let e = self.ref_escape(f, ts, arg);
                result = add_invocation_escape(
                    result,
                    e,
                    true,
                    is_ref_escape,
                    returns_ref_to_ref_struct,
                    is_ref_to_ref_struct,
                );
            }
        }
        result
    }

    /// `GetInvocationEscapeWithOldRules`.
    fn invocation_escape_with_old_rules(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        call: NodeId,
        is_ref_escape: bool,
    ) -> Option<SafeContext> {
        let method = call_method(f, call).clone();
        let args = call_args(f, call).to_vec();
        let mut result = Some(SafeContext::CallingMethod);
        let instance = is_instance_call(f, call);
        for (i, &arg) in args.iter().enumerate() {
            if i == 0 && instance {
                if method
                    .declaring
                    .is_some_and(|d| ts.is_ref_like_or_allows(d))
                {
                    let e = self.val_escape(f, ts, arg);
                    result = intersect(result, e);
                }
                continue;
            }
            let p = parameter(f, ts, call, i)?;
            let parameter_type = value_type(ts, p.ty);
            if !is_ref_escape && ts.is_ref_like_or_allows(parameter_type) {
                let e = self.val_escape(f, ts, arg);
                result = intersect(result, e);
            }
            if is_ref_escape && p.ref_kind != RefKind::None {
                let e = self.ref_escape(f, ts, arg);
                result = intersect(result, e);
            }
        }
        result
    }

    /// `GetInvocationEscapeToWritableArgument`: what the callee can capture into the writable
    /// argument.
    fn invocation_escape_to_writable_argument(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        call: NodeId,
        writable: usize,
    ) -> Option<SafeContext> {
        let method = call_method(f, call).clone();
        let MethodOrigin::Def(d) = method.origin else {
            return None;
        };
        if !uses_updated_escape_rules(&ts.comp.module(d.module).md) {
            return None;
        }
        let instance = is_instance_call(f, call);
        let is_constructor = method.name == ".ctor" || method.name == ".cctor";
        let destination = if writable == 0 && instance {
            if is_constructor {
                SafeContext::ReturnOnly
            } else {
                SafeContext::CallingMethod
            }
        } else {
            parameter_val_escape(&parameter(f, ts, call, writable)?)
        };
        let args = call_args(f, call).to_vec();
        let mut result = Some(SafeContext::CallingMethod);
        for (i, &arg) in args.iter().enumerate() {
            if i == writable {
                continue;
            }
            if i == 0 && instance {
                let receiver = method.declaring?;
                let receiver_val_escape = if is_constructor {
                    SafeContext::ReturnOnly
                } else {
                    SafeContext::CallingMethod
                };
                if ts.is_ref_like_or_allows(receiver) && receiver_val_escape >= destination {
                    let e = self.val_escape(f, ts, arg);
                    result = intersect(result, e);
                }
                let receiver_ref_escape = if this_scope(ts, &method) == ScopedKind::ScopedRef {
                    SafeContext::CurrentMethod
                } else {
                    SafeContext::ReturnOnly
                };
                if ts.is_reference_type(receiver) == Some(false)
                    && receiver_ref_escape >= destination
                {
                    let e = self.ref_escape(f, ts, arg);
                    result = intersect(result, e);
                }
                continue;
            }
            let p = parameter(f, ts, call, i)?;
            let parameter_type = value_type(ts, p.ty);
            if ts.is_ref_like_or_allows(parameter_type)
                && p.ref_kind != RefKind::Out
                && parameter_val_escape(&p) >= destination
            {
                let e = self.val_escape(f, ts, arg);
                result = intersect(result, e);
            }
            if p.ref_kind != RefKind::None && parameter_ref_escape(&p) >= destination {
                let e = self.ref_escape(f, ts, arg);
                result = intersect(result, e);
            }
        }
        result
    }

    /// `GetVariableScopes`.
    fn variable_scopes(&self, ts: &mut TypeSystem<'_, '_>, f: &Function, v: VarId) -> LocalScopes {
        if let Some(&scopes) = self.scopes.get(&v) {
            return scopes;
        }
        let var = f.var(v);
        if var.kind != VariableKind::Parameter {
            return LocalScopes::default();
        }
        let Some(index) = var.index else {
            return LocalScopes::default();
        };
        if index < 0 {
            return self.this_parameter_scopes(ts);
        }
        let params = self.resolved.params().unwrap_or_default();
        let Some(&ty) = params.get(index as usize) else {
            return LocalScopes::default();
        };
        let p = method_parameter(ts, &self.resolved, index as usize, ty);
        LocalScopes {
            ref_escape: Some(parameter_ref_escape(&p)),
            val_escape: Some(parameter_val_escape(&p)),
            is_scoped: false,
            is_scoped_without_initializer: false,
        }
    }

    /// `GetThisParameterScopes`.
    fn this_parameter_scopes(&self, ts: &mut TypeSystem<'_, '_>) -> LocalScopes {
        let md = &ts.comp.module(self.method.module).md;
        let declaring = md.method_declaring_type(self.method.row).unwrap_or(0);
        let is_struct = ts
            .comp
            .type_def(crate::typesystem::compilation::TypeDefId {
                module: self.method.module,
                row: declaring,
            })
            .is_ok_and(|i| i.kind == TypeKind::Struct);
        if !is_struct {
            return LocalScopes {
                ref_escape: Some(SafeContext::CallingMethod),
                val_escape: Some(SafeContext::CallingMethod),
                is_scoped: false,
                is_scoped_without_initializer: false,
            };
        }
        let ref_escape = if this_scope(ts, &self.resolved) == ScopedKind::ScopedRef {
            SafeContext::CurrentMethod
        } else {
            SafeContext::ReturnOnly
        };
        let is_constructor = self.resolved.name == ".ctor" || self.resolved.name == ".cctor";
        LocalScopes {
            ref_escape: Some(ref_escape),
            val_escape: Some(if is_constructor {
                SafeContext::ReturnOnly
            } else {
                SafeContext::CallingMethod
            }),
            is_scoped: false,
            is_scoped_without_initializer: false,
        }
    }
}

fn distinct(mut v: Vec<VarId>) -> Vec<VarId> {
    let mut seen = Vec::new();
    v.retain(|x| {
        if seen.contains(x) {
            false
        } else {
            seen.push(*x);
            true
        }
    });
    v
}

/// `GetFirstStore`: the earliest store in the tree.
fn first_store(f: &Function, stores: &[NodeId]) -> NodeId {
    let mut first = stores[0];
    for &s in &stores[1..] {
        if is_before(f, s, first) {
            first = s;
        }
    }
    first
}

fn call_method(f: &Function, call: NodeId) -> &ResolvedMethod {
    match f.inst(call) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => &c.method.method,
        _ => unreachable!("a call instruction"),
    }
}

fn call_args(f: &Function, call: NodeId) -> &[NodeId] {
    match f.inst(call) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => &c.args,
        _ => unreachable!("a call instruction"),
    }
}

/// `CallInstruction.IsInstanceCall`.
fn is_instance_call(f: &Function, call: NodeId) -> bool {
    !(call_method(f, call).is_static || matches!(f.inst(call), Inst::NewObj(_)))
}

/// `GetValueType`: the element type of a by-reference type.
fn value_type(ts: &TypeSystem<'_, '_>, t: TypeId) -> TypeId {
    match *ts.data(t) {
        TypeData::ByRef(e) => e,
        _ => t,
    }
}

fn intersect(left: Option<SafeContext>, right: Option<SafeContext>) -> Option<SafeContext> {
    Some(left?.min(right?))
}

/// `AddInvocationEscape`.
fn add_invocation_escape(
    current: Option<SafeContext>,
    argument: Option<SafeContext>,
    argument_is_ref_escape: bool,
    invocation_is_ref_escape: bool,
    returns_ref_to_ref_struct: bool,
    is_ref_to_ref_struct: bool,
) -> Option<SafeContext> {
    if returns_ref_to_ref_struct
        && (!is_ref_to_ref_struct || argument_is_ref_escape != invocation_is_ref_escape)
    {
        return current;
    }
    intersect(current, argument)
}

/// `GetThisParameterScope`.
fn this_scope(ts: &mut TypeSystem<'_, '_>, method: &ResolvedMethod) -> ScopedKind {
    match method.origin {
        MethodOrigin::Def(d) => {
            let owner = ts.accessor_owner(d).map(|(o, _)| o);
            ts.method_flags(d, owner).this_scope
        }
        _ => ScopedKind::None,
    }
}

/// `ThisIsRefReadOnly`.
fn this_is_ref_read_only(ts: &mut TypeSystem<'_, '_>, method: &ResolvedMethod) -> bool {
    match method.origin {
        MethodOrigin::Def(d) => {
            let owner = ts.accessor_owner(d).map(|(o, _)| o);
            ts.method_flags(d, owner).this_read_only
        }
        _ => false,
    }
}

/// `call.GetParameter(index)`.
fn parameter(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    call: NodeId,
    index: usize,
) -> Option<Param> {
    let method = call_method(f, call).clone();
    let first = usize::from(is_instance_call(f, call));
    let index = index.checked_sub(first)?;
    let params = method.params().ok()?;
    let &ty = params.get(index)?;
    Some(method_parameter(ts, &method, index, ty))
}

/// Parameter `index` (of type `ty`) of a method.
fn method_parameter(
    ts: &mut TypeSystem<'_, '_>,
    method: &ResolvedMethod,
    index: usize,
    ty: TypeId,
) -> Param {
    if let MethodOrigin::Def(d) = method.origin {
        let (_, infos) = ts.method_signature(d);
        let updated_rules = uses_updated_escape_rules(&ts.comp.module(d.module).md);
        if let Some(info) = infos.get(index) {
            return Param {
                ty,
                ref_kind: info.ref_kind,
                effective_scope: info.effective_scope,
                unscoped_ref: info.unscoped_ref,
                updated_rules,
            };
        }
    }
    let ref_kind = if matches!(ts.data(ty), TypeData::ByRef(_)) {
        RefKind::Ref
    } else {
        RefKind::None
    };
    Param {
        ty,
        ref_kind,
        effective_scope: ScopedKind::None,
        unscoped_ref: false,
        updated_rules: false,
    }
}

/// `GetParameterRefEscape`.
fn parameter_ref_escape(p: &Param) -> SafeContext {
    if p.ref_kind == RefKind::None || p.effective_scope == ScopedKind::ScopedRef {
        return SafeContext::CurrentMethod;
    }
    if p.unscoped_ref && p.updated_rules && p.ref_kind == RefKind::Out {
        return SafeContext::ReturnOnly;
    }
    if p.unscoped_ref && p.updated_rules {
        return SafeContext::CallingMethod;
    }
    SafeContext::ReturnOnly
}

/// `GetParameterValEscape`.
fn parameter_val_escape(p: &Param) -> SafeContext {
    if p.effective_scope == ScopedKind::ScopedValue {
        return SafeContext::CurrentMethod;
    }
    if p.ref_kind == RefKind::Out && p.updated_rules {
        return SafeContext::ReturnOnly;
    }
    SafeContext::CallingMethod
}

/// `IsWritableRefStructArgument`.
fn is_writable_ref_struct_argument(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    call: NodeId,
    index: usize,
) -> bool {
    if index == 0 && is_instance_call(f, call) {
        let method = call_method(f, call).clone();
        return method
            .declaring
            .is_some_and(|d| ts.is_ref_like_or_allows(d))
            && !this_is_ref_read_only(ts, &method);
    }
    let Some(p) = parameter(f, ts, call, index) else {
        return false;
    };
    let v = value_type(ts, p.ty);
    matches!(p.ref_kind, RefKind::Ref | RefKind::Out) && ts.is_ref_like_or_allows(v)
}

/// `GetRefReturnStorageArguments`.
fn ref_return_storage_arguments(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    call: NodeId,
) -> Vec<usize> {
    let method = call_method(f, call).clone();
    let TypeData::ByRef(element) = *ts.data(method.ret) else {
        return Vec::new();
    };
    if !ts.is_ref_like_or_allows(element) {
        return Vec::new();
    }
    let count = call_args(f, call).len();
    let instance = is_instance_call(f, call);
    let mut out = Vec::new();
    for i in 0..count {
        if i == 0 && instance {
            if method
                .declaring
                .is_some_and(|d| ts.is_ref_like_or_allows(d))
                && !this_is_ref_read_only(ts, &method)
                && this_scope(ts, &method) != ScopedKind::ScopedRef
            {
                out.push(i);
            }
            continue;
        }
        if let Some(p) = parameter(f, ts, call, i) {
            let v = value_type(ts, p.ty);
            if matches!(p.ref_kind, RefKind::Ref | RefKind::Out)
                && ts.is_ref_like_or_allows(v)
                && parameter_ref_escape(&p) != SafeContext::CurrentMethod
            {
                out.push(i);
            }
        }
    }
    out
}

fn field_is_static(ts: &TypeSystem<'_, '_>, field: &ResolvedField) -> bool {
    let FieldOrigin::Def { module, row } = field.origin else {
        return false;
    };
    let md = &ts.comp.module(module).md;
    md.get::<rows::Field>(row)
        .is_ok_and(|f| f.flags & 0x0010 != 0)
}
