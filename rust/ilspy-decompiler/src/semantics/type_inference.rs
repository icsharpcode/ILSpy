//! `TypeInference`: C# type inference (C# spec draft-v11: 12.6.3), the best common type of a
//! set of expressions and the search for a type within bounds.

use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::RefKind;
use crate::typesystem::types::{Nullability, TypeData, TypeId, TypeParamOwner};

use super::overload_resolution::OverloadOptions;
use super::types::{TypeKind, Variance};
use super::{Conversions, ResolveResult};

/// `TypeInferenceAlgorithm`.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub enum TypeInferenceAlgorithm {
    /// C# 4.0 type inference.
    #[default]
    CSharp4,
    /// An improved algorithm (not part of any specification) that fixes with
    /// `find_type_in_bounds`.
    Improved,
    /// `Improved`, reporting every result of an ambiguous search.
    ImprovedReturnAllResults,
}

/// The maximum generic nesting level, against infinite recursion in the improved algorithm.
const MAX_NESTING_LEVEL: u32 = 5;

/// A type variable being inferred (`TP`). Bounds keep their insertion order, as the managed
/// hash sets do.
#[derive(Clone, Debug)]
struct Tp {
    lower: Vec<TypeId>,
    upper: Vec<TypeId>,
    exact: Option<TypeId>,
    multiple_different_exact_bounds: bool,
    type_parameter: TypeId,
    fixed_to: Option<TypeId>,
}

impl Tp {
    fn new(type_parameter: TypeId) -> Tp {
        Tp {
            lower: Vec::new(),
            upper: Vec::new(),
            exact: None,
            multiple_different_exact_bounds: false,
            type_parameter,
            fixed_to: None,
        }
    }

    fn is_fixed(&self) -> bool {
        self.fixed_to.is_some()
    }

    fn has_bounds(&self) -> bool {
        !self.lower.is_empty() || !self.upper.is_empty() || self.exact.is_some()
    }
}

fn add_bound(bounds: &mut Vec<TypeId>, t: TypeId) {
    if !bounds.contains(&t) {
        bounds.push(t);
    }
}

/// `TypeInference` over a conversions instance.
pub struct TypeInference<'x, 't, 'c, 'a> {
    pub conversions: &'x mut Conversions<'t, 'c, 'a>,
    pub algorithm: TypeInferenceAlgorithm,
    nesting_level: u32,
    tps: Vec<Tp>,
    parameter_types: Vec<TypeId>,
    arguments: Vec<ResolveResult>,
    dependency_matrix: Option<Vec<Vec<bool>>>,
    class_type_arguments: Option<Vec<TypeId>>,
}

impl<'x, 't, 'c, 'a> TypeInference<'x, 't, 'c, 'a> {
    pub fn new(conversions: &'x mut Conversions<'t, 'c, 'a>) -> Self {
        TypeInference {
            conversions,
            algorithm: TypeInferenceAlgorithm::CSharp4,
            nesting_level: 0,
            tps: Vec::new(),
            parameter_types: Vec::new(),
            arguments: Vec::new(),
            dependency_matrix: None,
            class_type_arguments: None,
        }
    }

    fn reset(&mut self) {
        self.tps.clear();
        self.parameter_types.clear();
        self.arguments.clear();
        self.dependency_matrix = None;
        self.class_type_arguments = None;
    }

    fn unknown(&mut self) -> TypeId {
        self.conversions.ts.intern(TypeData::UnknownSpecial)
    }

    fn fixed_types(&mut self) -> Vec<TypeId> {
        let unknown = self.unknown();
        self.tps
            .iter()
            .map(|tp| tp.fixed_to.unwrap_or(unknown))
            .collect()
    }

    // ---- InferTypeArguments ----

    /// `InferTypeArguments`: the inferred type arguments for the method type parameters
    /// `type_parameters` (`?` where inference failed) and whether inference succeeded.
    /// `class_type_arguments` replace the class type parameters in the formal parameter types
    /// when a method group or lambda is inferred.
    ///
    /// Panics when a type parameter is not a method type parameter at its index.
    pub fn infer_type_arguments(
        &mut self,
        type_parameters: &[TypeId],
        arguments: &[ResolveResult],
        parameter_types: &[TypeId],
        class_type_arguments: Option<&[TypeId]>,
    ) -> (Vec<TypeId>, bool) {
        self.tps = type_parameters
            .iter()
            .enumerate()
            .map(|(i, &tp)| {
                match *self.conversions.ts.data(tp) {
                    TypeData::TypeParam { owner, index } => {
                        assert_eq!(index as usize, i, "type parameter has wrong index");
                        assert!(
                            owner.is_method(),
                            "type parameter must be owned by a method"
                        );
                    }
                    _ => panic!("not a type parameter"),
                }
                Tp::new(tp)
            })
            .collect();
        let n = arguments.len().min(parameter_types.len());
        self.parameter_types = parameter_types[..n].to_vec();
        self.arguments = arguments[..n].to_vec();
        self.class_type_arguments = class_type_arguments.map(<[_]>::to_vec);
        self.phase_one();
        let success = self.phase_two();
        let result = self.fixed_types();
        self.reset();
        (result, success)
    }

    /// `InferTypeArgumentsFromBounds`: type arguments for the `type_parameters` occurring in
    /// `target_type` so that the substituted type satisfies the bounds.
    pub fn infer_type_arguments_from_bounds(
        &mut self,
        type_parameters: &[TypeId],
        target_type: TypeId,
        lower_bounds: &[TypeId],
        upper_bounds: &[TypeId],
    ) -> (Vec<TypeId>, bool) {
        self.tps = type_parameters.iter().map(|&tp| Tp::new(tp)).collect();
        for &b in lower_bounds {
            self.make_lower_bound_inference(b, target_type);
        }
        for &b in upper_bounds {
            self.make_upper_bound_inference(b, target_type);
        }
        let mut success = true;
        for i in 0..self.tps.len() {
            success &= self.fix(i);
        }
        let result = self.fixed_types();
        self.reset();
        (result, success)
    }

    // ---- Phases ----

    fn phase_one(&mut self) {
        // C# 4.0 spec: 7.5.2.1 The first phase
        for i in 0..self.arguments.len() {
            let ei = self.arguments[i].clone();
            let ti = self.parameter_types[i];
            if let ResolveResult::Lambda(lambda) = &ei {
                self.make_explicit_parameter_type_inference(&**lambda, ti);
            }
            if matches!(ei, ResolveResult::Lambda(_) | ResolveResult::MethodGroup(_))
                && self.output_type_contains_unfixed(&ei, ti)
                && !self.input_types_contain_unfixed(&ei, ti)
            {
                // not in the spec
                self.make_output_type_inference(&ei, ti);
            }
            let ei_type = ei.ty(self.conversions.ts);
            if self.is_valid_type(ei_type) {
                if matches!(self.conversions.ts.data(ti), TypeData::ByRef(_)) {
                    self.make_exact_inference(ei_type, ti);
                } else {
                    self.make_lower_bound_inference(ei_type, ti);
                }
            }
        }
    }

    fn is_valid_type(&self, t: TypeId) -> bool {
        !matches!(
            self.conversions.ts.kind(t),
            TypeKind::Unknown | TypeKind::Null | TypeKind::None
        )
    }

    fn phase_two(&mut self) -> bool {
        // C# spec (draft-v11): 12.6.3.3 The second phase
        // All unfixed type variables Xi which do not depend on any Xj are fixed.
        let n = self.tps.len();
        let mut to_fix = Vec::new();
        for i in 0..n {
            if !self.tps[i].is_fixed()
                && !(0..n).any(|j| !self.tps[j].is_fixed() && self.depends_on(i, j))
            {
                to_fix.push(i);
            }
        }
        // Otherwise, all unfixed type variables Xi with non-empty bounds that another type
        // variable depends on.
        if to_fix.is_empty() {
            for i in 0..n {
                if !self.tps[i].is_fixed()
                    && self.tps[i].has_bounds()
                    && (0..n).any(|j| self.depends_on(j, i))
                {
                    to_fix.push(i);
                }
            }
        }
        let mut error_during_fix = false;
        for &i in &to_fix {
            if !self.fix(i) {
                error_during_fix = true;
            }
        }
        if error_during_fix {
            return false;
        }
        let unfixed_exist = self.tps.iter().any(|tp| !tp.is_fixed());
        if to_fix.is_empty() && unfixed_exist {
            // Unfixed type variables remain and none can be fixed: inference fails.
            return false;
        }
        if !unfixed_exist {
            return true;
        }
        // For all arguments whose output types contain unfixed type variables but whose
        // input types do not, an output type inference is made.
        for i in 0..self.arguments.len() {
            let ei = self.arguments[i].clone();
            let ti = self.parameter_types[i];
            if self.output_type_contains_unfixed(&ei, ti)
                && !self.input_types_contain_unfixed(&ei, ti)
            {
                self.make_output_type_inference(&ei, ti);
            }
        }
        // Then the second phase is repeated.
        self.phase_two()
    }

    // ---- Input and output types (12.6.3.4 + 12.6.3.5) ----

    fn input_types(&mut self, e: &ResolveResult, t: TypeId) -> Vec<TypeId> {
        let applies = match e {
            ResolveResult::Lambda(l) => l.is_implicitly_typed(),
            ResolveResult::MethodGroup(_) => true,
            _ => false,
        };
        if applies && let Some(m) = self.delegate_or_expression_tree_signature(t) {
            return m.parameters.iter().map(|p| p.ty).collect();
        }
        Vec::new()
    }

    fn output_types(&mut self, e: &ResolveResult, t: TypeId) -> Vec<TypeId> {
        if matches!(e, ResolveResult::Lambda(_) | ResolveResult::MethodGroup(_))
            && let Some(m) = self.delegate_or_expression_tree_signature(t)
        {
            return vec![m.return_type];
        }
        Vec::new()
    }

    fn delegate_or_expression_tree_signature(&mut self, t: TypeId) -> Option<super::Method> {
        let t = self.conversions.unpack_expression_tree_type(t);
        self.conversions.ts.delegate_invoke(t)
    }

    fn input_types_contain_unfixed(&mut self, e: &ResolveResult, t: TypeId) -> bool {
        let types = self.input_types(e, t);
        self.any_type_contains_unfixed(&types)
    }

    fn output_type_contains_unfixed(&mut self, e: &ResolveResult, t: TypeId) -> bool {
        let types = self.output_types(e, t);
        self.any_type_contains_unfixed(&types)
    }

    fn any_type_contains_unfixed(&self, types: &[TypeId]) -> bool {
        let occurs = self.occurs_in(types);
        self.tps
            .iter()
            .zip(occurs)
            .any(|(tp, occurs)| !tp.is_fixed() && occurs)
    }

    /// `OccursInVisitor`: which of the type variables occur in `types`.
    fn occurs_in(&self, types: &[TypeId]) -> Vec<bool> {
        let mut occurs = vec![false; self.tps.len()];
        let mut stack: Vec<TypeId> = types.to_vec();
        while let Some(t) = stack.pop() {
            match self.conversions.ts.data(t) {
                TypeData::TypeParam { index, .. } => {
                    let i = *index as usize;
                    if i < self.tps.len() && self.tps[i].type_parameter == t {
                        occurs[i] = true;
                    }
                }
                TypeData::Inst { generic, args } => {
                    stack.push(*generic);
                    stack.extend(args.iter());
                }
                TypeData::Array { elem: e, .. }
                | TypeData::Pointer(e)
                | TypeData::ByRef(e)
                | TypeData::Pinned(e)
                | TypeData::Annotated { inner: e, .. } => stack.push(*e),
                TypeData::Modified {
                    modifier, inner, ..
                } => {
                    stack.push(*modifier);
                    stack.push(*inner);
                }
                TypeData::Tuple { elements, .. } => stack.extend(elements.iter()),
                TypeData::FnPtr(f) => {
                    stack.push(f.ret);
                    stack.extend(f.params.iter());
                }
                _ => {}
            }
        }
        occurs
    }

    // ---- Dependence (12.6.3.6) ----

    fn calculate_dependency_matrix(&mut self) -> Vec<Vec<bool>> {
        let n = self.tps.len();
        let mut m = vec![vec![false; n]; n];
        for k in 0..self.arguments.len() {
            let (arg, param) = (self.arguments[k].clone(), self.parameter_types[k]);
            let inputs = self.input_types(&arg, param);
            let outputs = self.output_types(&arg, param);
            let input = self.occurs_in(&inputs);
            let output = self.occurs_in(&outputs);
            for i in 0..n {
                for j in 0..n {
                    m[i][j] |= input[j] && output[i];
                }
            }
        }
        // transitive closure (Warshall's algorithm)
        for i in 0..n {
            for j in 0..n {
                if m[i][j] {
                    let row_j = m[j].clone();
                    for (target, reachable) in m[i].iter_mut().zip(row_j) {
                        *target |= reachable;
                    }
                }
            }
        }
        m
    }

    /// Whether type variable `x` depends on `y`.
    fn depends_on(&mut self, x: usize, y: usize) -> bool {
        if self.dependency_matrix.is_none() {
            self.dependency_matrix = Some(self.calculate_dependency_matrix());
        }
        let x = self.tp_index(x);
        let y = self.tp_index(y);
        self.dependency_matrix.as_ref().expect("a matrix")[x][y]
    }

    /// The index of a type variable's type parameter (`TypeParameter.Index`).
    fn tp_index(&self, i: usize) -> usize {
        match self.conversions.ts.data(self.tps[i].type_parameter) {
            TypeData::TypeParam { index, .. } => *index as usize,
            _ => i,
        }
    }

    // ---- Output type inference (7.5.2.6) ----

    fn make_output_type_inference(&mut self, e: &ResolveResult, t: TypeId) {
        // If E is an anonymous function with inferred return type U and T is a delegate or
        // expression tree type with return type Tb, a lower-bound inference is made from U
        // to Tb.
        if let ResolveResult::Lambda(lambda) = e
            && let Some(m) = self.delegate_or_expression_tree_signature(t)
        {
            let inferred = if lambda.is_implicitly_typed() {
                if m.parameters.len() != lambda.parameters().len() {
                    return; // mismatched parameter lists
                }
                let fixed = self.fixed_types();
                let class = self.class_type_arguments.clone();
                let types: Vec<TypeId> = m
                    .parameters
                    .iter()
                    .map(|p| {
                        self.conversions
                            .ts
                            .substitute(p.ty, class.as_deref(), Some(&fixed))
                    })
                    .collect();
                lambda.inferred_return_type(self.conversions, Some(&types))
            } else {
                lambda.inferred_return_type(self.conversions, None)
            };
            self.make_lower_bound_inference(inferred, m.return_type);
            return;
        }
        // If E is a method group and T a delegate or expression tree type with parameter
        // types T1..Tk and return type Tb, and overload resolution of E with T1..Tk yields a
        // single method with return type U, a lower-bound inference is made from U to Tb.
        if let ResolveResult::MethodGroup(group) = e {
            if let Some(m) = self.delegate_or_expression_tree_signature(t) {
                let fixed = self.fixed_types();
                let class = self.class_type_arguments.clone();
                let args: Vec<ResolveResult> = m
                    .parameters
                    .iter()
                    .map(|param| {
                        let ty = self.conversions.ts.substitute(
                            param.ty,
                            class.as_deref(),
                            Some(&fixed),
                        );
                        if param.ref_kind != RefKind::None
                            && let TypeData::ByRef(elem) = *self.conversions.ts.data(ty)
                        {
                            ResolveResult::by_reference(self.conversions.ts, elem, param.ref_kind)
                        } else {
                            ResolveResult::Expression(ty)
                        }
                    })
                    .collect();
                let or = self.conversions.perform_overload_resolution(
                    group,
                    args,
                    OverloadOptions {
                        allow_expanding_params: false,
                        allow_optional_parameters: false,
                        allow_implicit_in: false,
                        ..OverloadOptions::default()
                    },
                );
                if or.found_applicable_candidate() && or.best_candidate_ambiguous_with().is_none() {
                    let best = or
                        .best_candidate_with_substituted_type_arguments(self.conversions.ts)
                        .expect("a best candidate");
                    self.make_lower_bound_inference(best.return_type, m.return_type);
                }
            }
            return;
        }
        // Otherwise, if E is an expression with type U, a lower-bound inference is made from
        // U to T.
        let e_type = e.ty(self.conversions.ts);
        if self.is_valid_type(e_type) {
            self.make_lower_bound_inference(e_type, t);
        }
    }

    // ---- Explicit parameter type inference (12.6.3.9) ----

    fn make_explicit_parameter_type_inference(&mut self, e: &dyn super::Lambda, t: TypeId) {
        if e.is_implicitly_typed() || !e.has_parameter_list() {
            return;
        }
        let Some(m) = self.delegate_or_expression_tree_signature(t) else {
            return;
        };
        for (pe, pm) in e.parameters().iter().zip(&m.parameters) {
            self.make_exact_inference(pe.ty, pm.ty);
        }
    }

    // ---- Exact inference (7.5.2.8) ----

    /// The nullability-equal case of the bound inferences: both types without it.
    fn strip_equal_nullability(&mut self, u: TypeId, v: TypeId) -> (TypeId, TypeId) {
        let ts = &mut *self.conversions.ts;
        if ts.nullability_of(u) == ts.nullability_of(v) {
            (
                ts.change_nullability(u, Nullability::Oblivious),
                ts.change_nullability(v, Nullability::Oblivious),
            )
        } else {
            (u, v)
        }
    }

    fn element_of(&self, t: TypeId) -> Option<(TypeId, u32)> {
        match *self.conversions.ts.data(t) {
            TypeData::Array { elem, rank, .. } => Some((elem, rank)),
            _ => None,
        }
    }

    fn is_span(&self, t: TypeId, code: KnownTypeCode) -> bool {
        self.conversions.first_class_span_types()
            && matches!(self.conversions.ts.data(t), TypeData::Inst { .. })
            && self.conversions.ts.is_known(t, code)
    }

    fn instance(&self, t: TypeId) -> Option<(TypeId, Vec<TypeId>)> {
        match self.conversions.ts.data(t) {
            TypeData::Inst { generic, args } => Some((*generic, args.to_vec())),
            _ => None,
        }
    }

    fn make_exact_inference(&mut self, u: TypeId, v: TypeId) {
        let (u, v) = self.strip_equal_nullability(u, v);
        // If V is one of the unfixed Xi, U is added to the bounds of Xi.
        if let Some(i) = self.tp_for_type(v)
            && !self.tps[i].is_fixed()
        {
            self.add_exact_bound(i, u);
            return;
        }
        let ts = &*self.conversions.ts;
        if let (TypeData::ByRef(eu), TypeData::ByRef(ev)) = (ts.data(u), ts.data(v)) {
            let (eu, ev) = (*eu, *ev);
            self.make_exact_inference(eu, ev);
            return;
        }
        let u = self.conversions.ts.tuple_underlying_or_self(u);
        let v = self.conversions.ts.tuple_underlying_or_self(v);
        use KnownTypeCode::{ReadOnlySpanOfT as Ros, SpanOfT as Span};
        match (self.element_of(u), self.element_of(v)) {
            (Some((eu, ru)), Some((ev, rv))) if ru == rv => {
                self.make_exact_inference(eu, ev);
                return;
            }
            (Some((eu, _)), None) if self.is_span(v, Span) || self.is_span(v, Ros) => {
                let ev = self.conversions.ts.first_type_argument(v).expect("Span<T>");
                self.make_exact_inference(eu, ev);
                return;
            }
            (None, None)
                if (self.is_span(u, Span) && (self.is_span(v, Span) || self.is_span(v, Ros)))
                    || (self.is_span(u, Ros) && self.is_span(v, Ros)) =>
            {
                let eu = self.conversions.ts.first_type_argument(u).expect("Span<T>");
                let ev = self.conversions.ts.first_type_argument(v).expect("Span<T>");
                self.make_exact_inference(eu, ev);
                return;
            }
            _ => {}
        }
        // parameterized types
        if let (Some((gu, au)), Some((gv, av))) = (self.instance(u), self.instance(v))
            && gu == gv
            && au.len() == av.len()
        {
            for (a, b) in au.into_iter().zip(av) {
                self.make_exact_inference(a, b);
            }
            return;
        }
        let ts = &*self.conversions.ts;
        match (ts.data(u).clone(), ts.data(v).clone()) {
            (TypeData::Pointer(eu), TypeData::Pointer(ev)) => self.make_exact_inference(eu, ev),
            (TypeData::FnPtr(fu), TypeData::FnPtr(fv)) => {
                self.make_exact_inference(fu.ret, fv.ret);
                for (&a, &b) in fu.params.iter().zip(fv.params.iter()) {
                    self.make_exact_inference(a, b);
                }
            }
            _ => {}
        }
    }

    /// `TP.AddExactBound`: exact bounds are kept apart from the lower and upper bounds (see
    /// `GenericArgumentImplicitlyConvertibleToAndFromAnotherTypeList`, #281); two that differ
    /// only in tuple element names merge their names instead of conflicting.
    fn add_exact_bound(&mut self, i: usize, t: TypeId) {
        let Some(existing) = self.tps[i].exact else {
            self.tps[i].exact = Some(t);
            return;
        };
        if existing == t {
            return;
        }
        match self.merge_similar_types(existing, t, Variance::Invariant) {
            Some(merged) => self.tps[i].exact = Some(merged),
            None => self.tps[i].multiple_different_exact_bounds = true,
        }
    }

    /// The type variable `v` is, if any.
    fn tp_for_type(&self, v: TypeId) -> Option<usize> {
        let ts = &*self.conversions.ts;
        let v = match ts.data(v) {
            TypeData::Annotated { inner, .. }
                if matches!(ts.data(*inner), TypeData::TypeParam { .. }) =>
            {
                *inner
            }
            _ => v,
        };
        if let TypeData::TypeParam { index, .. } = ts.data(v) {
            let i = *index as usize;
            if i < self.tps.len() && self.tps[i].type_parameter == v {
                return Some(i);
            }
        }
        None
    }

    // ---- Lower-bound inference (12.6.3.11) ----

    fn type_parameter_variance(&mut self, generic: TypeId, i: usize) -> Variance {
        match self.conversions.ts.definition(generic) {
            Some(def) => self.conversions.ts.type_parameter_variance(def, i),
            None => Variance::Invariant,
        }
    }

    fn make_lower_bound_inference(&mut self, u: TypeId, v: TypeId) {
        let (u, v) = self.strip_equal_nullability(u, v);
        // If V is one of the unfixed Xi, U is added to the lower bounds of Xi.
        if let Some(i) = self.tp_for_type(v)
            && !self.tps[i].is_fixed()
        {
            add_bound(&mut self.tps[i].lower, u);
            return;
        }
        let ts = &*self.conversions.ts;
        // nullable covariance
        if ts.is_nullable(u) && ts.is_nullable(v) {
            let (nu, nv) = (ts.nullable_underlying(u), ts.nullable_underlying(v));
            self.make_lower_bound_inference(nu, nv);
            return;
        }
        if let (TypeData::ByRef(eu), TypeData::ByRef(ev)) = (ts.data(u), ts.data(v)) {
            let (eu, ev) = (*eu, *ev);
            self.make_exact_inference(eu, ev);
            return;
        }
        let v = self.conversions.ts.tuple_underlying_or_self(v);
        use KnownTypeCode::{ReadOnlySpanOfT as Ros, SpanOfT as Span};
        let first = |s: &Self, t: TypeId| {
            s.conversions
                .ts
                .first_type_argument(t)
                .expect("an argument")
        };
        match (self.element_of(u), self.element_of(v)) {
            (Some((eu, ru)), Some((ev, rv))) if ru == rv => {
                self.make_lower_bound_inference(eu, ev);
                return;
            }
            // Span<T> is invariant, so even a lower-bound context gives an exact element
            // inference for a Span<V1> target (C# 14 spec, 12.6.3.10).
            (Some((eu, _)), None) if self.is_span(v, Span) => {
                let ev = first(self, v);
                self.make_exact_inference(eu, ev);
                return;
            }
            (Some((eu, _)), None) if self.is_span(v, Ros) => {
                let ev = first(self, v);
                self.make_lower_bound_inference(eu, ev);
                return;
            }
            (None, None) if self.is_span(u, Span) && self.is_span(v, Span) => {
                let (eu, ev) = (first(self, u), first(self, v));
                self.make_exact_inference(eu, ev);
                return;
            }
            (None, None)
                if (self.is_span(u, Span) || self.is_span(u, Ros)) && self.is_span(v, Ros) =>
            {
                let (eu, ev) = (first(self, u), first(self, v));
                self.make_lower_bound_inference(eu, ev);
                return;
            }
            (Some((eu, 1)), None)
                if matches!(self.conversions.ts.data(v), TypeData::Inst { .. })
                    && self.conversions.ts.is_array_interface(v) =>
            {
                let ev = first(self, v);
                self.make_lower_bound_inference(eu, ev);
                return;
            }
            _ => {}
        }
        // parameterized types
        if let Some((gv, av)) = self.instance(v) {
            let mut unique: Option<Vec<TypeId>> = None;
            let bases = self.conversions.ts.all_base_types(u).unwrap_or_default();
            for base in bases {
                let base = self.conversions.ts.tuple_underlying_or_self(base);
                if let Some((gu, au)) = self.instance(base)
                    && gu == gv
                    && au.len() == av.len()
                {
                    if unique.is_some() {
                        return; // not unique: no inference
                    }
                    unique = Some(au);
                }
            }
            if let Some(au) = unique {
                for (i, (ui, vi)) in au.into_iter().zip(av).enumerate() {
                    if self.conversions.ts.is_reference_type(ui) == Some(true) {
                        match self.type_parameter_variance(gv, i) {
                            Variance::Covariant => self.make_lower_bound_inference(ui, vi),
                            Variance::Contravariant => self.make_upper_bound_inference(ui, vi),
                            Variance::Invariant => self.make_exact_inference(ui, vi),
                        }
                    } else {
                        // not known to be a reference type
                        self.make_exact_inference(ui, vi);
                    }
                }
            }
            return;
        }
        let ts = &*self.conversions.ts;
        match (ts.data(u).clone(), ts.data(v).clone()) {
            (TypeData::Pointer(eu), TypeData::Pointer(ev)) => self.make_exact_inference(eu, ev),
            (TypeData::FnPtr(fu), TypeData::FnPtr(fv)) => {
                self.make_lower_bound_inference(fu.ret, fv.ret);
                for (&a, &b) in fu.params.iter().zip(fv.params.iter()) {
                    self.make_upper_bound_inference(a, b);
                }
            }
            _ => {}
        }
    }

    // ---- Upper-bound inference (7.5.2.10) ----

    fn make_upper_bound_inference(&mut self, u: TypeId, v: TypeId) {
        let (u, v) = self.strip_equal_nullability(u, v);
        // If V is one of the unfixed Xi, U is added to the upper bounds of Xi.
        if let Some(i) = self.tp_for_type(v)
            && !self.tps[i].is_fixed()
        {
            add_bound(&mut self.tps[i].upper, u);
            return;
        }
        let pu_type = self.conversions.ts.tuple_underlying_or_self(u);
        let pu = self.instance(pu_type);
        match (self.element_of(u), self.element_of(v)) {
            (Some((eu, ru)), Some((ev, rv))) if ru == rv => {
                self.make_upper_bound_inference(eu, ev);
                return;
            }
            (_, Some((ev, 1)))
                if pu.is_some() && self.conversions.ts.is_array_interface(pu_type) =>
            {
                let ua = pu.as_ref().expect("a parameterized type").1[0];
                self.make_upper_bound_inference(ua, ev);
                return;
            }
            _ => {}
        }
        // parameterized types
        if let Some((gu, au)) = pu {
            let mut unique: Option<Vec<TypeId>> = None;
            let bases = self.conversions.ts.all_base_types(v).unwrap_or_default();
            for base in bases {
                let base = self.conversions.ts.tuple_underlying_or_self(base);
                if let Some((gv, av)) = self.instance(base)
                    && gu == gv
                    && au.len() == av.len()
                {
                    if unique.is_some() {
                        return; // not unique: no inference
                    }
                    unique = Some(av);
                }
            }
            if let Some(av) = unique {
                for (i, (ui, vi)) in au.into_iter().zip(av).enumerate() {
                    if self.conversions.ts.is_reference_type(ui) == Some(true) {
                        match self.type_parameter_variance(gu, i) {
                            Variance::Covariant => self.make_upper_bound_inference(ui, vi),
                            Variance::Contravariant => self.make_lower_bound_inference(ui, vi),
                            Variance::Invariant => self.make_exact_inference(ui, vi),
                        }
                    } else {
                        // not known to be a reference type
                        self.make_exact_inference(ui, vi);
                    }
                }
            }
            return;
        }
        let ts = &*self.conversions.ts;
        match (ts.data(u).clone(), ts.data(v).clone()) {
            (TypeData::Pointer(eu), TypeData::Pointer(ev)) => self.make_exact_inference(eu, ev),
            (TypeData::FnPtr(fu), TypeData::FnPtr(fv)) => {
                self.make_upper_bound_inference(fu.ret, fv.ret);
                for (&a, &b) in fu.params.iter().zip(fv.params.iter()) {
                    self.make_lower_bound_inference(a, b);
                }
            }
            _ => {}
        }
    }

    // ---- Fixing (12.6.3.13) ----

    fn nested(&mut self) -> TypeInference<'_, 't, 'c, 'a> {
        TypeInference {
            conversions: &mut *self.conversions,
            algorithm: self.algorithm,
            nesting_level: self.nesting_level + 1,
            tps: Vec::new(),
            parameter_types: Vec::new(),
            arguments: Vec::new(),
            dependency_matrix: None,
            class_type_arguments: None,
        }
    }

    fn fix(&mut self, i: usize) -> bool {
        debug_assert!(!self.tps[i].is_fixed());
        let lower = self.tps[i].lower.clone();
        let upper = self.tps[i].upper.clone();
        if let Some(exact) = self.tps[i].exact {
            // Roslyn (not the C# standard): a lower or upper bound of the exact bound's shape
            // but other tuple element names merges its names in (kept where both agree,
            // dropped where they conflict); see MergeTupleNames in Roslyn's
            // MethodTypeInference.cs.
            let mut fixed_to = exact;
            for &b in &lower {
                fixed_to = self
                    .merge_similar_types(fixed_to, b, Variance::Covariant)
                    .unwrap_or(fixed_to);
            }
            for &b in &upper {
                fixed_to = self
                    .merge_similar_types(fixed_to, b, Variance::Contravariant)
                    .unwrap_or(fixed_to);
            }
            // the exact bound determines the result, up to the merged element names
            self.tps[i].fixed_to = Some(fixed_to);
            if self.tps[i].multiple_different_exact_bounds {
                return false;
            }
            return lower
                .iter()
                .all(|&b| self.conversions.implicit_type(b, fixed_to).is_valid())
                && upper
                    .iter()
                    .all(|&b| self.conversions.implicit_type(fixed_to, b).is_valid());
        }
        let types = self.nested().find_types_in_bounds(&lower, &upper);
        if self.algorithm == TypeInferenceAlgorithm::ImprovedReturnAllResults {
            self.tps[i].fixed_to = Some(self.conversions.ts.intersection(&types));
            !types.is_empty()
        } else {
            self.tps[i].fixed_to = Some(self.first_type_prefer_non_interfaces(&types));
            types.len() == 1
        }
    }

    /// `MergeSimilarTypes`: the merge of types that differ only in tuple element names (kept
    /// where both agree), `object`/`dynamic` (`dynamic` wins) and nullability (by the
    /// variance of the position); `None` when they differ otherwise.
    fn merge_similar_types(&mut self, a: TypeId, b: TypeId, variance: Variance) -> Option<TypeId> {
        if a == b {
            return Some(a);
        }
        let ts = &mut *self.conversions.ts;
        let (da, db) = (ts.data(a).clone(), ts.data(b).clone());
        if matches!(da, TypeData::Annotated { .. }) || matches!(db, TypeData::Annotated { .. }) {
            let nullability =
                merge_nullability(ts.nullability_of(a), ts.nullability_of(b), variance);
            let wa = ts.change_nullability(a, Nullability::Oblivious);
            let wb = ts.change_nullability(b, Nullability::Oblivious);
            let merged = self.merge_similar_types(wa, wb, variance)?;
            return Some(self.conversions.ts.change_nullability(merged, nullability));
        }
        if matches!(da, TypeData::Dynamic) && ts.is_known(b, KnownTypeCode::Object) {
            return Some(a);
        }
        if matches!(db, TypeData::Dynamic) && ts.is_known(a, KnownTypeCode::Object) {
            return Some(b);
        }
        match (da, db) {
            (
                TypeData::Tuple {
                    elements: ea,
                    names: na,
                    ..
                },
                TypeData::Tuple {
                    elements: eb,
                    names: nb,
                    ..
                },
            ) if ea.len() == eb.len() => {
                let mut elements = Vec::with_capacity(ea.len());
                for (&x, &y) in ea.iter().zip(eb.iter()) {
                    // ValueTuple's type parameters are invariant, but Roslyn merges tuple
                    // element types covariantly.
                    elements.push(self.merge_similar_types(
                        x,
                        y,
                        variance.combine(Variance::Covariant),
                    )?);
                }
                let names: Box<[Option<Box<str>>]> = na
                    .iter()
                    .zip(nb.iter())
                    .map(|(x, y)| if x == y { x.clone() } else { None })
                    .collect();
                let ts = &mut *self.conversions.ts;
                let module = ts.definition(a).map(|d| d.module);
                let underlying = ts.tuple_underlying(&elements, module);
                Some(ts.intern(TypeData::Tuple {
                    underlying,
                    elements: elements.into(),
                    names,
                }))
            }
            (
                TypeData::Inst {
                    generic: ga,
                    args: aa,
                },
                TypeData::Inst {
                    generic: gb,
                    args: ab,
                },
            ) if aa.len() == ab.len() => {
                let generic = self.merge_similar_types(ga, gb, variance)?;
                let mut args = Vec::with_capacity(aa.len());
                for (i, (&x, &y)) in aa.iter().zip(ab.iter()).enumerate() {
                    let v = self.type_parameter_variance(ga, i);
                    args.push(self.merge_similar_types(x, y, variance.combine(v))?);
                }
                Some(self.conversions.ts.intern(TypeData::Inst {
                    generic,
                    args: args.into(),
                }))
            }
            (
                TypeData::Array {
                    elem: ea,
                    rank: ra,
                    nullability: na,
                },
                TypeData::Array {
                    elem: eb,
                    rank: rb,
                    nullability: nb,
                },
            ) if ra == rb => {
                // Roslyn merges array element types covariantly.
                let elem =
                    self.merge_similar_types(ea, eb, variance.combine(Variance::Covariant))?;
                Some(self.conversions.ts.intern(TypeData::Array {
                    elem,
                    rank: ra,
                    nullability: merge_nullability(na, nb, variance),
                }))
            }
            (TypeData::ByRef(ea), TypeData::ByRef(eb)) => {
                let elem = self.merge_similar_types(ea, eb, Variance::Invariant)?;
                Some(self.conversions.ts.intern(TypeData::ByRef(elem)))
            }
            (TypeData::Pointer(ea), TypeData::Pointer(eb)) => {
                let elem = self.merge_similar_types(ea, eb, Variance::Invariant)?;
                Some(self.conversions.ts.intern(TypeData::Pointer(elem)))
            }
            (TypeData::FnPtr(fa), TypeData::FnPtr(fb))
                if fa.calling_convention == fb.calling_convention
                    && fa.custom_conventions == fb.custom_conventions
                    && fa.ret_ref_readonly == fb.ret_ref_readonly
                    && fa.params.len() == fb.params.len()
                    && fa.ref_kinds == fb.ref_kinds =>
            {
                let ret = self.merge_similar_types(
                    fa.ret,
                    fb.ret,
                    variance.combine(Variance::Covariant),
                )?;
                let mut params = Vec::with_capacity(fa.params.len());
                for (&x, &y) in fa.params.iter().zip(fb.params.iter()) {
                    params.push(self.merge_similar_types(
                        x,
                        y,
                        variance.combine(Variance::Contravariant),
                    )?);
                }
                let mut f = *fa;
                f.ret = ret;
                f.params = params.into();
                Some(self.conversions.ts.intern(TypeData::FnPtr(Box::new(f))))
            }
            (
                TypeData::Modified {
                    modifier: ma,
                    inner: ia,
                    required: ra,
                },
                TypeData::Modified {
                    modifier: mb,
                    inner: ib,
                    required: rb,
                },
            ) if ra == rb && ma == mb => {
                let inner = self.merge_similar_types(ia, ib, variance)?;
                Some(self.conversions.ts.intern(TypeData::Modified {
                    modifier: ma,
                    inner,
                    required: ra,
                }))
            }
            (
                TypeData::Unknown {
                    name: na,
                    is_reference_type: ra,
                },
                TypeData::Unknown {
                    name: nb,
                    is_reference_type: rb,
                },
            ) if na == nb => {
                if ra == rb {
                    Some(a)
                } else {
                    Some(self.conversions.ts.intern(TypeData::Unknown {
                        name: na,
                        is_reference_type: None,
                    }))
                }
            }
            _ => None,
        }
    }

    // ---- Best common type ----

    /// `GetBestCommonType`: the best common type of a set of expressions (C# spec draft-v11:
    /// 12.6.3.17) and whether there is one.
    pub fn best_common_type(&mut self, expressions: &[ResolveResult]) -> (TypeId, bool) {
        if let [single] = expressions {
            let t = single.ty(self.conversions.ts);
            return (t, self.is_valid_type(t));
        }
        let tp = self.conversions.ts.intern(TypeData::TypeParam {
            owner: TypeParamOwner::DummyMethod,
            index: 0,
        });
        self.tps = vec![Tp::new(tp)];
        for r in expressions {
            self.make_output_type_inference(r, tp);
        }
        let success = self.fix(0);
        let unknown = self.unknown();
        let result = self.tps[0].fixed_to.unwrap_or(unknown);
        self.reset();
        (result, success)
    }

    // ---- FindTypeInBounds ----

    /// `FindTypeInBounds`: a type satisfying the bounds (with `ImprovedReturnAllResults` the
    /// intersection of all candidates, otherwise preferring non-interfaces when there
    /// are several; `?` when there is none).
    pub fn find_type_in_bounds(&mut self, lower: &[TypeId], upper: &[TypeId]) -> TypeId {
        let result = self.find_types_in_bounds(lower, upper);
        if self.algorithm == TypeInferenceAlgorithm::ImprovedReturnAllResults {
            self.conversions.ts.intersection(&result)
        } else {
            self.first_type_prefer_non_interfaces(&result)
        }
    }

    fn first_type_prefer_non_interfaces(&mut self, types: &[TypeId]) -> TypeId {
        types
            .iter()
            .copied()
            .find(|&c| self.conversions.ts.kind(c) != TypeKind::Interface)
            .or_else(|| types.first().copied())
            .unwrap_or_else(|| self.unknown())
    }

    /// The candidate types X with `lower <: X <: upper` (`FindTypesInBounds`); with
    /// `ImprovedReturnAllResults` every candidate of an ambiguous search (the managed
    /// `FindTypeInBounds` returns their `IntersectionType`).
    pub fn find_types_in_bounds(&mut self, lower: &[TypeId], upper: &[TypeId]) -> Vec<TypeId> {
        // A single type, or none for empty inputs.
        if lower.is_empty() && upper.len() <= 1 {
            return upper.to_vec();
        }
        if upper.is_empty() && lower.len() <= 1 {
            return lower.to_vec();
        }
        if self.nesting_level > MAX_NESTING_LEVEL {
            return Vec::new();
        }
        // Deduplicate, merging types that differ only in tuple element names, object/dynamic
        // and nullability (not in the specification, but Roslyn's AddAllCandidates does it,
        // and tuple element names and nested nullabilities depend on it).
        let mut top_level_nullability: Option<Nullability> = None;
        let mut keys: Vec<TypeId> = Vec::new();
        let mut merged: Vec<TypeId> = Vec::new();
        for (bounds, variance) in [
            (lower, Variance::Covariant),
            (upper, Variance::Contravariant),
        ] {
            for &bound in bounds {
                let n = self.conversions.ts.nullability_of(bound);
                top_level_nullability = Some(match top_level_nullability {
                    Some(t) => merge_nullability(t, n, variance),
                    None => n,
                });
                let key = self
                    .conversions
                    .ts
                    .normalize_with(bound, Normalization::KEY_FOR_TYPE_MERGING);
                match keys.iter().position(|&k| k == key) {
                    Some(i) => {
                        let existing = merged[i];
                        let m = self.merge_similar_types(existing, bound, variance);
                        debug_assert!(
                            m.is_some(),
                            "merge_similar_types merges every type of a merge key"
                        );
                        if let Some(m) = m {
                            merged[i] = m;
                        }
                    }
                    None => {
                        keys.push(key);
                        merged.push(bound);
                    }
                }
            }
        }
        // the fixing algorithm of the C# spec (12.6.3.13)
        let mut candidates = Vec::new();
        for &c in &merged {
            if lower
                .iter()
                .all(|&b| self.conversions.implicit_type(b, c).is_valid())
                && upper
                    .iter()
                    .all(|&b| self.conversions.implicit_type(c, b).is_valid())
            {
                candidates.push(c);
            }
        }
        // The result is the unique candidate to which every other candidate converts.
        let all = candidates.clone();
        candidates.retain(|&c| {
            all.iter()
                .all(|&o| self.conversions.implicit_type(o, c).is_valid())
        });
        // the merged top-level nullability
        let top = top_level_nullability.expect("a bound");
        for c in &mut candidates {
            *c = self.conversions.ts.change_nullability(*c, top);
        }
        if candidates.len() == 1
            || !matches!(
                self.algorithm,
                TypeInferenceAlgorithm::Improved | TypeInferenceAlgorithm::ImprovedReturnAllResults
            )
        {
            return candidates;
        }
        candidates.clear();
        // the improved algorithm
        let mut definitions: Vec<TypeDefId> = if let Some((&first, rest)) = lower.split_first() {
            // candidates from the lower bounds
            let mut set = self.conversions.ts.all_base_type_definitions(first);
            for &b in rest {
                let other = self.conversions.ts.all_base_type_definitions(b);
                set.retain(|d| other.contains(d));
            }
            set
        } else {
            // candidates from all classes of the compilation
            all_type_definitions(self.conversions)
        };
        // without the candidates that violate the upper bounds
        for &ub in upper {
            if let Some(ub_def) = self.conversions.ts.definition(ub) {
                definitions.retain(|&c| self.conversions.ts.is_derived_from_definition(c, ub_def));
            }
        }
        for candidate_def in definitions {
            let tpc = self
                .conversions
                .ts
                .comp
                .type_def(candidate_def)
                .map_or(0, |i| i.type_parameter_count);
            let candidate = if tpc == 0 {
                self.conversions.ts.intern_def(candidate_def)
            } else {
                let definition = self.conversions.ts.intern_def(candidate_def);
                let type_parameters = self.conversions.ts.type_arguments(definition);
                let target = self.conversions.ts.intern(TypeData::Inst {
                    generic: definition,
                    args: type_parameters.clone().into(),
                });
                let (result, success) =
                    self.infer_type_arguments_from_bounds(&type_parameters, target, lower, upper);
                if !success {
                    continue;
                }
                self.conversions.ts.intern(TypeData::Inst {
                    generic: definition,
                    args: result.into(),
                })
            };
            let derived = |s: &mut Self, a: TypeId, b: TypeDefId| {
                s.conversions
                    .ts
                    .definition(a)
                    .is_some_and(|d| s.conversions.ts.is_derived_from_definition(d, b))
            };
            let derived_def = |s: &mut Self, a: TypeDefId, b: TypeId| {
                s.conversions
                    .ts
                    .definition(b)
                    .is_some_and(|d| s.conversions.ts.is_derived_from_definition(a, d))
            };
            if upper.is_empty() {
                // Only lower bounds: aim for the most specific candidate, unless a more
                // specific one is already there.
                if !candidates
                    .clone()
                    .into_iter()
                    .any(|c| derived(self, c, candidate_def))
                {
                    let kept: Vec<TypeId> = candidates
                        .clone()
                        .into_iter()
                        .filter(|&c| !derived_def(self, candidate_def, c))
                        .collect();
                    candidates = kept;
                    candidates.push(candidate);
                }
            } else {
                // Upper bounds: aim for the least specific candidate, unless a less specific
                // one is already there.
                if !candidates
                    .clone()
                    .into_iter()
                    .any(|c| derived_def(self, candidate_def, c))
                {
                    let kept: Vec<TypeId> = candidates
                        .clone()
                        .into_iter()
                        .filter(|&c| !derived(self, c, candidate_def))
                        .collect();
                    candidates = kept;
                    candidates.push(candidate);
                }
            }
        }
        candidates
    }
}

/// `compilation.GetAllTypeDefinitions()`: every type definition of every module.
fn all_type_definitions(conversions: &Conversions<'_, '_, '_>) -> Vec<TypeDefId> {
    let comp = conversions.ts.comp;
    let mut result = Vec::new();
    for module in comp.module_ids() {
        let count = comp
            .module(module)
            .md
            .tables
            .rows(crate::metadata::tables::TableId::TypeDef);
        // Row 1 is `<Module>`.
        for row in 2..=count {
            result.push(TypeDefId { module, row });
        }
    }
    result
}

/// `MergeNullability`, like Roslyn's `MergeNullableAnnotation`.
fn merge_nullability(a: Nullability, b: Nullability, variance: Variance) -> Nullability {
    use Nullability::{NotNullable, Nullable, Oblivious};
    match variance {
        // Nullable wins over Oblivious, which wins over NotNullable.
        Variance::Covariant => match (a, b) {
            (Nullable, _) | (_, Nullable) => Nullable,
            (Oblivious, _) | (_, Oblivious) => Oblivious,
            _ => NotNullable,
        },
        // NotNullable wins over Oblivious, which wins over Nullable.
        Variance::Contravariant => match (a, b) {
            (NotNullable, _) | (_, NotNullable) => NotNullable,
            (Oblivious, _) | (_, Oblivious) => Oblivious,
            _ => Nullable,
        },
        // NotNullable wins over Nullable, which wins over Oblivious (as Roslyn does:
        // `static T M<T>(ref T x, ref T y)` called with a nullable and a non-nullable array
        // infers the non-nullable one and warns at the first argument).
        Variance::Invariant => match (a, b) {
            (NotNullable, _) | (_, NotNullable) => NotNullable,
            (Nullable, _) | (_, Nullable) => Nullable,
            _ => Oblivious,
        },
    }
}
