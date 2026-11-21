//! `OverloadResolution`: C# overload resolution (C# spec draft-v11: 12.6.4) and constraint
//! validation.

use std::ops::{BitOr, BitOrAssign};

use crate::typesystem::attrs::ACC_PUBLIC;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::members::RefKind;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::type_inference::TypeInference;
use super::types::TypeKind;
use super::{Conversion, Conversions, Method, MethodList, Parameter, ResolveResult};

/// `OverloadResolutionErrors` (flags).
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct OverloadResolutionErrors(pub u32);

impl OverloadResolutionErrors {
    pub const NONE: Self = Self(0);
    pub const TOO_MANY_POSITIONAL_ARGUMENTS: Self = Self(0x0001);
    pub const NO_PARAMETER_FOUND_FOR_NAMED_ARGUMENT: Self = Self(0x0002);
    pub const TYPE_INFERENCE_FAILED: Self = Self(0x0004);
    pub const WRONG_NUMBER_OF_TYPE_ARGUMENTS: Self = Self(0x0008);
    pub const CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT: Self = Self(0x0010);
    pub const MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER: Self = Self(0x0020);
    pub const MULTIPLE_ARGUMENTS_FOR_SINGLE_PARAMETER: Self = Self(0x0040);
    pub const PARAMETER_PASSING_MODE_MISMATCH: Self = Self(0x0080);
    pub const ARGUMENT_TYPE_MISMATCH: Self = Self(0x0100);
    pub const AMBIGUOUS_MATCH: Self = Self(0x0200);
    pub const INACCESSIBLE: Self = Self(0x0400);
    pub const METHOD_CONSTRAINTS_NOT_SATISFIED: Self = Self(0x0800);
    pub const OUT_VAR_TYPE_MISMATCH: Self = Self(0x1000);

    pub fn contains(self, other: Self) -> bool {
        self.0 & other.0 != 0
    }
}

impl BitOr for OverloadResolutionErrors {
    type Output = Self;
    fn bitor(self, rhs: Self) -> Self {
        Self(self.0 | rhs.0)
    }
}

impl BitOrAssign for OverloadResolutionErrors {
    fn bitor_assign(&mut self, rhs: Self) {
        self.0 |= rhs.0;
    }
}

/// `OverloadResolution.IsApplicable`: ambiguity and unsatisfied method constraints do not
/// affect applicability.
pub fn is_applicable(errors: OverloadResolutionErrors) -> bool {
    errors.0
        & !(OverloadResolutionErrors::AMBIGUOUS_MATCH.0
            | OverloadResolutionErrors::METHOD_CONSTRAINTS_NOT_SATISFIED.0)
        == 0
}

/// The input properties of `OverloadResolution` and the flags of
/// `MethodGroupResolveResult.PerformOverloadResolution`.
#[derive(Clone, Copy, Debug)]
pub struct OverloadOptions {
    /// Whether extension methods are tried when no method is applicable (method groups).
    pub allow_extension_methods: bool,
    /// Whether `params` may expand into individual elements.
    pub allow_expanding_params: bool,
    /// Whether optional parameters may keep their default values.
    pub allow_optional_parameters: bool,
    /// Whether a value argument may be passed to an `in` parameter.
    pub allow_implicit_in: bool,
    /// Whether an extension method receiver may bind through an implicit span conversion
    /// (C# 14 does not consider span conversions for a method group conversion).
    pub allow_span_conversion_on_extension_receiver: bool,
    pub check_for_overflow: bool,
}

impl Default for OverloadOptions {
    fn default() -> Self {
        OverloadOptions {
            allow_extension_methods: true,
            allow_expanding_params: true,
            allow_optional_parameters: true,
            allow_implicit_in: true,
            allow_span_conversion_on_extension_receiver: true,
            check_for_overflow: false,
        }
    }
}

#[derive(Clone, Debug)]
struct Candidate {
    member: Method,
    is_expanded_form: bool,
    /// The parameter types: unsubstituted at first, substituted after type inference.
    parameter_types: Vec<TypeId>,
    /// Argument index to parameter index; -1 for arguments that could not be mapped.
    argument_to_parameter_map: Vec<i32>,
    errors: OverloadResolutionErrors,
    error_count: u32,
    has_unmapped_optional_parameters: bool,
    inferred_types: Option<Vec<TypeId>>,
    /// The definition's parameters, before any substitution.
    parameters: Vec<Parameter>,
    /// The definition's method type parameters, `None` for a non-generic method.
    type_parameters: Option<Vec<TypeId>>,
    argument_conversions: Vec<Conversion>,
}

impl Candidate {
    fn new(member: Method, is_expanded_form: bool) -> Candidate {
        let parameters = member.definition_parameters.clone();
        let type_parameters =
            (!member.type_parameters.is_empty()).then(|| member.type_parameters.clone());
        Candidate {
            parameter_types: parameters.iter().map(|p| p.ty).collect(),
            member,
            is_expanded_form,
            argument_to_parameter_map: Vec::new(),
            errors: OverloadResolutionErrors::NONE,
            error_count: 0,
            has_unmapped_optional_parameters: false,
            inferred_types: None,
            parameters,
            type_parameters,
            argument_conversions: Vec::new(),
        }
    }

    fn add_error(&mut self, error: OverloadResolutionErrors) {
        self.errors |= error;
        if !is_applicable(error) {
            self.error_count += 1;
        }
    }

    /// The type of the `params` collection of an expanded candidate (unsubstituted).
    fn params_collection_type(&self) -> Option<TypeId> {
        match self.parameters.last() {
            Some(last) if self.is_expanded_form && last.is_params => Some(last.ty),
            _ => None,
        }
    }

    fn is_generic_method(&self) -> bool {
        !self.member.type_parameters.is_empty()
    }

    fn arguments_passed_to_params(&self) -> usize {
        if !self.is_expanded_form {
            return 0;
        }
        let params_index = self.parameters.len() as i32 - 1;
        self.argument_to_parameter_map
            .iter()
            .filter(|&&p| p == params_index)
            .count()
    }
}

/// `OverloadResolution`: candidates are added one at a time; the best so far is kept.
pub struct OverloadResolution {
    arguments: Vec<ResolveResult>,
    argument_names: Vec<Option<String>>,
    explicitly_given_type_arguments: Option<Vec<TypeId>>,
    best_candidate: Option<Candidate>,
    best_candidate_ambiguous_with: Option<Candidate>,
    best_candidate_validation: Option<OverloadResolutionErrors>,
    /// Whether the candidates are extension methods called with extension method syntax: the
    /// first argument then allows only identity, reference, boxing (and span) conversions.
    pub is_extension_method_invocation: bool,
    pub options: OverloadOptions,
}

impl OverloadResolution {
    /// `new OverloadResolution(compilation, arguments, argumentNames, typeArguments)`.
    pub fn new(
        arguments: Vec<ResolveResult>,
        argument_names: Option<Vec<Option<String>>>,
        type_arguments: Vec<TypeId>,
    ) -> OverloadResolution {
        let argument_names = argument_names.unwrap_or_else(|| vec![None; arguments.len()]);
        assert_eq!(
            argument_names.len(),
            arguments.len(),
            "argument_names must have one entry per argument"
        );
        OverloadResolution {
            arguments,
            argument_names,
            // stays None without type arguments
            explicitly_given_type_arguments: (!type_arguments.is_empty()).then_some(type_arguments),
            best_candidate: None,
            best_candidate_ambiguous_with: None,
            best_candidate_validation: None,
            is_extension_method_invocation: false,
            options: OverloadOptions::default(),
        }
    }

    pub fn set_options(&mut self, options: OverloadOptions) {
        self.options = options;
    }

    pub fn arguments(&self) -> &[ResolveResult] {
        &self.arguments
    }

    // ---- AddCandidate ----

    /// `AddCandidate(member)`: the errors that keep the member from being applicable.
    pub fn add_candidate(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
        member: Method,
    ) -> OverloadResolutionErrors {
        self.add_candidate_with(conversions, member, OverloadResolutionErrors::NONE)
    }

    /// `AddCandidate(member, additionalErrors)`: with errors from member lookup (for example
    /// `INACCESSIBLE`).
    pub fn add_candidate_with(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
        member: Method,
        additional_errors: OverloadResolutionErrors,
    ) -> OverloadResolutionErrors {
        let mut c = Candidate::new(member.clone(), false);
        c.add_error(additional_errors);
        let c = self.calculate_candidate(conversions, c);
        let errors = c
            .as_ref()
            .map_or(OverloadResolutionErrors::NONE, |c| c.errors);
        let error_count = c.as_ref().map_or(0, |c| c.error_count);
        if self.options.allow_expanding_params
            && member.parameters.last().is_some_and(|p| p.is_params)
        {
            let mut expanded = Candidate::new(member, true);
            expanded.add_error(additional_errors);
            // the expanded form, unless it is obviously wrong
            if let Some(expanded) = self.calculate_candidate(conversions, expanded)
                && expanded.error_count < error_count
            {
                return expanded.errors;
            }
        }
        errors
    }

    /// Applicability and so on for the candidate; `None` when the candidate is dropped
    /// without an error.
    fn calculate_candidate(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
        mut candidate: Candidate,
    ) -> Option<Candidate> {
        if !Self::resolve_parameter_types(conversions, &mut candidate, false) {
            return None;
        }
        self.map_corresponding_parameters(&mut candidate);
        self.run_type_inference(conversions, &mut candidate);
        self.check_applicability(conversions, &mut candidate);
        self.consider_if_new_candidate_is_best(conversions, &candidate);
        Some(candidate)
    }

    fn resolve_parameter_types(
        conversions: &mut Conversions<'_, '_, '_>,
        candidate: &mut Candidate,
        use_specialized_parameters: bool,
    ) -> bool {
        let ts = &mut *conversions.ts;
        let count = candidate.parameters.len();
        for i in 0..count {
            let mut ty = if use_specialized_parameters {
                // the parameter type of the specialized non-generic method
                debug_assert!(!candidate.is_generic_method());
                candidate.member.parameters[i].ty
            } else {
                // the type of the original formal parameter
                candidate.parameters[i].ty
            };
            if candidate.is_expanded_form && i == count - 1 {
                ty = match *ts.data(ty) {
                    TypeData::Array { elem, rank: 1, .. } => elem,
                    _ if ts.is_known(ty, KnownTypeCode::ReadOnlySpanOfT)
                        || ts.is_known(ty, KnownTypeCode::SpanOfT)
                        || ts.is_array_interface(ty) =>
                    {
                        match ts.first_type_argument(ty) {
                            Some(e) => e,
                            None => return false,
                        }
                    }
                    // the params collection cannot be unpacked: drop the expanded form
                    _ => return false,
                };
            }
            candidate.parameter_types[i] = ty;
        }
        true
    }

    // ---- AddMethodLists ----

    /// `AddMethodLists`: every candidate of the method lists (base types first), where an
    /// applicable method in a derived type hides all methods of its base types.
    pub fn add_method_lists(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
        method_lists: &[MethodList],
    ) {
        let mut hidden = vec![false; method_lists.len()];
        // derived types first
        for i in (0..method_lists.len()).rev() {
            if hidden[i] {
                continue;
            }
            let list = &method_lists[i];
            let mut found_applicable = false;
            for method in &list.methods {
                let errors = self.add_candidate(conversions, method.clone());
                found_applicable |= is_applicable(errors);
            }
            if found_applicable && i > 0 {
                let bases = conversions
                    .ts
                    .all_base_types(list.declaring)
                    .unwrap_or_default();
                for base in bases {
                    for j in 0..i {
                        if !hidden[j] && base == method_lists[j].declaring {
                            hidden[j] = true;
                        }
                    }
                }
            }
        }
    }

    // ---- MapCorrespondingParameters ----

    fn map_corresponding_parameters(&self, candidate: &mut Candidate) {
        // C# spec (draft-v11): 12.6.2.2 Corresponding parameters (with the non-trailing named
        // arguments of C# 7.2)
        let n = self.arguments.len();
        candidate.argument_to_parameter_map = vec![-1; n];
        let mut has_positional_argument = false;
        // backwards, so that has_positional_argument tells whether there are non-trailing
        // named arguments
        for i in (0..n).rev() {
            let name = self.argument_names[i].as_deref();
            if name.is_none() || has_positional_argument {
                has_positional_argument = true;
                // positional or non-trailing named argument
                if i < candidate.parameter_types.len() {
                    candidate.argument_to_parameter_map[i] = i as i32;
                    if let Some(name) = name
                        && name != candidate.parameters[i].name
                    {
                        // a non-trailing named argument must match the name
                        candidate.add_error(
                            OverloadResolutionErrors::NO_PARAMETER_FOUND_FOR_NAMED_ARGUMENT,
                        );
                    }
                } else if candidate.is_expanded_form {
                    candidate.argument_to_parameter_map[i] =
                        candidate.parameter_types.len() as i32 - 1;
                    if name.is_some() {
                        // no non-trailing named argument here
                        candidate.add_error(
                            OverloadResolutionErrors::NO_PARAMETER_FOUND_FOR_NAMED_ARGUMENT,
                        );
                    }
                } else {
                    candidate.add_error(OverloadResolutionErrors::TOO_MANY_POSITIONAL_ARGUMENTS);
                }
            } else {
                // a trailing named argument
                for (j, p) in candidate.parameters.iter().enumerate() {
                    if name == Some(p.name.as_str()) {
                        candidate.argument_to_parameter_map[i] = j as i32;
                    }
                }
                if candidate.argument_to_parameter_map[i] < 0 {
                    candidate
                        .add_error(OverloadResolutionErrors::NO_PARAMETER_FOUND_FOR_NAMED_ARGUMENT);
                }
            }
        }
    }

    // ---- RunTypeInference ----

    fn run_type_inference(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        candidate: &mut Candidate,
    ) {
        let Some(type_parameters) = candidate.type_parameters.clone() else {
            if self.explicitly_given_type_arguments.is_some() {
                // the method expects no type arguments but was given some
                candidate.add_error(OverloadResolutionErrors::WRONG_NUMBER_OF_TYPE_ARGUMENTS);
            }
            // the specialized parameter types
            Self::resolve_parameter_types(conversions, candidate, true);
            return;
        };
        let class_type_arguments =
            candidate
                .member
                .declaring
                .and_then(|d| match conversions.ts.data(d) {
                    TypeData::Inst { args, .. } => Some(args.to_vec()),
                    _ => None,
                });
        // the method is generic
        let unknown = conversions.ts.intern(TypeData::UnknownSpecial);
        if let Some(given) = &self.explicitly_given_type_arguments {
            if given.len() == type_parameters.len() {
                candidate.inferred_types = Some(given.clone());
            } else {
                candidate.add_error(OverloadResolutionErrors::WRONG_NUMBER_OF_TYPE_ARGUMENTS);
                // truncated, or padded with `?`
                candidate.inferred_types = Some(
                    (0..type_parameters.len())
                        .map(|i| given.get(i).copied().unwrap_or(unknown))
                        .collect(),
                );
            }
        } else {
            let parameter_types: Vec<TypeId> = candidate
                .argument_to_parameter_map
                .iter()
                .map(|&p| {
                    if p >= 0 {
                        candidate.parameter_types[p as usize]
                    } else {
                        unknown
                    }
                })
                .collect();
            let mut ti = TypeInference::new(conversions);
            let (inferred, success) = ti.infer_type_arguments(
                &type_parameters,
                &self.arguments,
                &parameter_types,
                class_type_arguments.as_deref(),
            );
            candidate.inferred_types = Some(inferred);
            if !success {
                candidate.add_error(OverloadResolutionErrors::TYPE_INFERENCE_FAILED);
            }
        }
        // substitute into the formal parameters, validating the constructed types
        let inferred = candidate.inferred_types.clone().expect("inferred types");
        let mut constraints_valid = true;
        for i in 0..candidate.parameter_types.len() {
            let t = candidate.parameter_types[i];
            if constraints_valid {
                constraints_valid = validate_constructed_types(
                    conversions,
                    t,
                    class_type_arguments.as_deref(),
                    &inferred,
                );
            }
            candidate.parameter_types[i] =
                conversions
                    .ts
                    .substitute(t, class_type_arguments.as_deref(), Some(&inferred));
        }
        if !constraints_valid {
            candidate
                .add_error(OverloadResolutionErrors::CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT);
        }
    }

    // ---- Constraints ----

    fn validate_method_constraints(
        conversions: &mut Conversions<'_, '_, '_>,
        candidate: &Candidate,
    ) -> OverloadResolutionErrors {
        // Constraints are not checked when type inference already failed.
        if candidate
            .errors
            .contains(OverloadResolutionErrors::TYPE_INFERENCE_FAILED)
        {
            return OverloadResolutionErrors::NONE;
        }
        let Some(type_parameters) = &candidate.type_parameters else {
            return OverloadResolutionErrors::NONE; // not generic
        };
        let class = candidate.member.class_args.clone();
        let method = candidate.inferred_types.clone().unwrap_or_default();
        for (i, &tp) in type_parameters.iter().enumerate() {
            let Some(&argument) = method.get(i) else {
                continue;
            };
            if !validate_constraints(conversions, tp, argument, class.as_deref(), Some(&method)) {
                return OverloadResolutionErrors::METHOD_CONSTRAINTS_NOT_SATISFIED;
            }
        }
        OverloadResolutionErrors::NONE
    }

    // ---- CheckApplicability ----

    fn check_applicability(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        candidate: &mut Candidate,
    ) {
        // C# 4.0 spec: 7.5.3.1 Applicable function member
        // whether each parameter got the right number of arguments
        let mut argument_count_per_parameter = vec![0u32; candidate.parameter_types.len()];
        for &p in &candidate.argument_to_parameter_map {
            if p >= 0 {
                argument_count_per_parameter[p as usize] += 1;
            }
        }
        let last = argument_count_per_parameter.len().wrapping_sub(1);
        for (i, &count) in argument_count_per_parameter.iter().enumerate() {
            if candidate.is_expanded_form && i == last {
                continue; // any number of arguments for the params collection
            }
            if count == 0 {
                if self.options.allow_optional_parameters && candidate.parameters[i].is_optional {
                    candidate.has_unmapped_optional_parameters = true;
                } else {
                    candidate.add_error(
                        OverloadResolutionErrors::MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER,
                    );
                }
            } else if count > 1 {
                candidate
                    .add_error(OverloadResolutionErrors::MULTIPLE_ARGUMENTS_FOR_SINGLE_PARAMETER);
            }
        }
        candidate.argument_conversions = vec![Conversion::None; self.arguments.len()];
        // whether the argument passing modes match the parameters'
        for (i, argument) in self.arguments.iter().enumerate() {
            let p = candidate.argument_to_parameter_map[i];
            if p < 0 {
                continue;
            }
            let p = p as usize;
            let param_ref_kind = candidate.parameters[p].ref_kind;
            match argument {
                ResolveResult::ByReference { ref_kind, .. } => {
                    if *ref_kind != param_ref_kind {
                        candidate
                            .add_error(OverloadResolutionErrors::PARAMETER_PASSING_MODE_MISMATCH);
                    }
                }
                ResolveResult::OutVar(_) => {
                    if param_ref_kind != RefKind::Out {
                        candidate
                            .add_error(OverloadResolutionErrors::PARAMETER_PASSING_MODE_MISMATCH);
                    }
                    // `out var` declarations fit any out parameter
                    continue;
                }
                _ => {
                    // `in` parameters may be filled without `in` (allow_implicit_in), and the
                    // receiver of an extension method with `this ref` or `this in` without a
                    // keyword.
                    let implicit = (matches!(param_ref_kind, RefKind::In | RefKind::RefReadOnly)
                        && self.options.allow_implicit_in)
                        || (self.is_extension_method_invocation
                            && p == 0
                            && matches!(
                                param_ref_kind,
                                RefKind::In | RefKind::Ref | RefKind::RefReadOnly
                            ));
                    let skipped = conversions.ts.skip_modifiers(candidate.parameter_types[p]);
                    match *conversions.ts.data(skipped) {
                        TypeData::ByRef(element) if implicit => {
                            // as if the parameter were not declared `in`, for applicability
                            // and the better function member
                            candidate.parameter_types[p] = element;
                        }
                        _ if param_ref_kind != RefKind::None => {
                            candidate.add_error(
                                OverloadResolutionErrors::PARAMETER_PASSING_MODE_MISMATCH,
                            );
                        }
                        _ => {}
                    }
                }
            }
            let parameter_type = candidate.parameter_types[p];
            let c = conversions.implicit(argument, parameter_type);
            if self.is_extension_method_invocation && p == 0 {
                // The receiver of an extension method needs an identity, reference, boxing or
                // span conversion.
                let receiver_ok = c == Conversion::IDENTITY
                    || c == Conversion::IMPLICIT_REFERENCE
                    || c == Conversion::BOXING
                    || (c == Conversion::IMPLICIT_SPAN
                        && self.options.allow_span_conversion_on_extension_receiver);
                if !receiver_ok {
                    candidate.add_error(OverloadResolutionErrors::ARGUMENT_TYPE_MISMATCH);
                }
            } else if !c.is_valid()
                && !c.is_user_defined()
                && !c.is_method_group()
                && conversions.ts.kind(parameter_type) != TypeKind::Unknown
            {
                candidate.add_error(OverloadResolutionErrors::ARGUMENT_TYPE_MISMATCH);
            }
            candidate.argument_conversions[i] = c;
        }
    }

    // ---- BetterFunctionMember ----

    /// 1 when `c1` is better than `c2`, 2 when `c2` is better, 0 when neither is.
    fn better_function_member(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        c1: &Candidate,
        c2: &Candidate,
    ) -> u8 {
        // applicable members first (a heuristic that finds a best candidate even when none is
        // applicable)
        if c1.error_count == 0 && c2.error_count > 0 {
            return 1;
        }
        if c1.error_count > 0 && c2.error_count == 0 {
            return 2;
        }
        // C# 4.0 spec: 7.5.3.2 Better function member
        let mut c1_is_better = false;
        let mut c2_is_better = false;
        let mut parameter_types_equal = true;
        for (i, argument) in self.arguments.iter().enumerate() {
            let p1 = c1.argument_to_parameter_map[i];
            let p2 = c2.argument_to_parameter_map[i];
            if p1 >= 0 && p2 < 0 {
                c1_is_better = true;
            } else if p1 < 0 && p2 >= 0 {
                c2_is_better = true;
            } else if p1 >= 0 && p2 >= 0 {
                let t1 = c1.parameter_types[p1 as usize];
                let t2 = c2.parameter_types[p2 as usize];
                if !conversions.identity(t1, t2) {
                    parameter_types_equal = false;
                }
                match conversions.better_conversion(argument, t1, t2) {
                    1 => c1_is_better = true,
                    2 => c2_is_better = true,
                    _ => {}
                }
            }
        }
        if c1_is_better && !c2_is_better {
            return 1;
        }
        if !c1_is_better && c2_is_better {
            return 2;
        }
        // fewer errors first (part of the heuristic)
        if c1.error_count < c2.error_count {
            return 1;
        }
        if c1.error_count > c2.error_count {
            return 2;
        }
        if !c1_is_better && !c2_is_better && parameter_types_equal {
            // the tie-breaking rules
            // non-generic methods are better
            match (c1.is_generic_method(), c2.is_generic_method()) {
                (false, true) => return 1,
                (true, false) => return 2,
                _ => {}
            }
            // non-expanded members are better
            match (c1.is_expanded_form, c2.is_expanded_form) {
                (false, true) => return 1,
                (true, false) => return 2,
                _ => {}
            }
            // fewer arguments passed to the params collection
            match c1
                .arguments_passed_to_params()
                .cmp(&c2.arguments_passed_to_params())
            {
                std::cmp::Ordering::Less => return 1,
                std::cmp::Ordering::Greater => return 2,
                std::cmp::Ordering::Equal => {}
            }
            // no default values to substitute
            match (
                c1.has_unmapped_optional_parameters,
                c2.has_unmapped_optional_parameters,
            ) {
                (false, true) => return 1,
                (true, false) => return 2,
                _ => {}
            }
            // the formal parameters
            let r = self.more_specific_formal_parameters(conversions, c1, c2);
            if r != 0 {
                return r;
            }
            // non-lifted operators are better
            match (c1.member.lifted, c2.member.lifted) {
                (false, true) => return 1,
                (true, false) => return 2,
                _ => {}
            }
            // by-value parameters before `in` parameters
            let r = better_parameter_passing_choice(c1, c2);
            if r != 0 {
                return r;
            }
            if c1.is_expanded_form {
                debug_assert!(c2.is_expanded_form);
                if let (Some(p1), Some(p2)) =
                    (c1.params_collection_type(), c2.params_collection_type())
                {
                    let r = better_params_collection_type(conversions, p1, p2);
                    if r != 0 {
                        return r;
                    }
                }
            }
        }
        0
    }

    fn more_specific_formal_parameters(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        c1: &Candidate,
        c2: &Candidate,
    ) -> u8 {
        // the member with more formal parameters (when they differ in optional parameters)
        match c1.parameters.len().cmp(&c2.parameters.len()) {
            std::cmp::Ordering::Greater => return 1,
            std::cmp::Ordering::Less => return 2,
            std::cmp::Ordering::Equal => {}
        }
        let t1: Vec<TypeId> = c1.parameters.iter().map(|p| p.ty).collect();
        let t2: Vec<TypeId> = c2.parameters.iter().map(|p| p.ty).collect();
        more_specific_formal_parameter_list(conversions, &t1, &t2)
    }

    // ---- ConsiderIfNewCandidateIsBest ----

    fn consider_if_new_candidate_is_best(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
        candidate: &Candidate,
    ) {
        let Some(best) = self.best_candidate.take() else {
            self.best_candidate = Some(candidate.clone());
            self.best_candidate_validation = None;
            return;
        };
        let r = self.better_function_member(conversions, candidate, &best);
        match r {
            // `best_candidate_ambiguous_with` is overwritten, so that callers can find all
            // ambiguous methods by looking at it after each step.
            0 => {
                self.best_candidate = Some(best);
                self.best_candidate_ambiguous_with = Some(candidate.clone());
            }
            1 => {
                self.best_candidate = Some(candidate.clone());
                self.best_candidate_validation = None;
                self.best_candidate_ambiguous_with = None;
            }
            // the best candidate stays best
            _ => self.best_candidate = Some(best),
        }
    }

    // ---- Output ----

    /// `BestCandidate`: the member as it was added.
    pub fn best_candidate(&self) -> Option<&Method> {
        self.best_candidate.as_ref().map(|c| &c.member)
    }

    /// `BestCandidateErrors`: with the errors that do not affect applicability (ambiguity,
    /// unsatisfied method constraints).
    pub fn best_candidate_errors(
        &mut self,
        conversions: &mut Conversions<'_, '_, '_>,
    ) -> OverloadResolutionErrors {
        let Some(best) = &self.best_candidate else {
            return OverloadResolutionErrors::NONE;
        };
        let validation = match self.best_candidate_validation {
            Some(v) => v,
            None => {
                let v = Self::validate_method_constraints(conversions, best);
                self.best_candidate_validation = Some(v);
                v
            }
        };
        let mut errors = best.errors | validation;
        if self.best_candidate_ambiguous_with.is_some() {
            errors |= OverloadResolutionErrors::AMBIGUOUS_MATCH;
        }
        errors
    }

    pub fn found_applicable_candidate(&self) -> bool {
        self.best_candidate
            .as_ref()
            .is_some_and(|c| is_applicable(c.errors))
    }

    pub fn best_candidate_ambiguous_with(&self) -> Option<&Method> {
        self.best_candidate_ambiguous_with
            .as_ref()
            .map(|c| &c.member)
    }

    pub fn best_candidate_is_expanded_form(&self) -> bool {
        self.best_candidate
            .as_ref()
            .is_some_and(|c| c.is_expanded_form)
    }

    pub fn is_ambiguous(&self) -> bool {
        self.best_candidate_ambiguous_with.is_some()
    }

    /// `InferredTypeArguments`.
    pub fn inferred_type_arguments(&self) -> &[TypeId] {
        self.best_candidate
            .as_ref()
            .and_then(|c| c.inferred_types.as_deref())
            .unwrap_or(&[])
    }

    /// The implicit conversions applied to the arguments.
    pub fn argument_conversions(&self) -> Vec<Conversion> {
        match &self.best_candidate {
            Some(c) if !c.argument_conversions.is_empty() => c.argument_conversions.clone(),
            _ => vec![Conversion::None; self.arguments.len()],
        }
    }

    /// `GetArgumentToParameterMap()`: argument index to parameter index, -1 for arguments
    /// that map to no parameter.
    pub fn argument_to_parameter_map(&self) -> Option<&[i32]> {
        self.best_candidate
            .as_ref()
            .map(|c| c.argument_to_parameter_map.as_slice())
    }

    /// `GetBestCandidateWithSubstitutedTypeArguments()`.
    pub fn best_candidate_with_substituted_type_arguments(
        &self,
        ts: &mut crate::typesystem::types::TypeSystem<'_, '_>,
    ) -> Option<Method> {
        let best = self.best_candidate.as_ref()?;
        if best.member.type_parameters.is_empty() {
            return Some(best.member.clone());
        }
        // The substitutions are merged, not composed (InvocationTests.
        // SubstituteClassAndMethodTypeParametersAtOnce).
        Some(best.member.specialize_definition(
            ts,
            best.member.class_args.clone(),
            best.inferred_types.clone(),
        ))
    }
}

/// `MoreSpecificFormalParameters` over two parameter type lists.
fn more_specific_formal_parameter_list(
    conversions: &mut Conversions<'_, '_, '_>,
    t1: &[TypeId],
    t2: &[TypeId],
) -> u8 {
    let mut c1_is_better = false;
    let mut c2_is_better = false;
    for (&a, &b) in t1.iter().zip(t2) {
        match more_specific_formal_parameter(conversions, a, b) {
            1 => c1_is_better = true,
            2 => c2_is_better = true,
            _ => {}
        }
    }
    if c1_is_better && !c2_is_better {
        1
    } else if !c1_is_better && c2_is_better {
        2
    } else {
        0
    }
}

fn more_specific_formal_parameter(
    conversions: &mut Conversions<'_, '_, '_>,
    t1: TypeId,
    t2: TypeId,
) -> u8 {
    let ts = &*conversions.ts;
    let is_type_parameter = |t: TypeId| match ts.data(t) {
        TypeData::TypeParam { .. } => true,
        TypeData::Annotated { inner, .. } => matches!(ts.data(*inner), TypeData::TypeParam { .. }),
        _ => false,
    };
    match (is_type_parameter(t1), is_type_parameter(t2)) {
        (true, false) => return 2,
        (false, true) => return 1,
        _ => {}
    }
    if let (TypeData::Inst { args: a1, .. }, TypeData::Inst { args: a2, .. }) =
        (ts.data(t1).clone(), ts.data(t2).clone())
        && a1.len() == a2.len()
    {
        let r = more_specific_formal_parameter_list(conversions, &a1, &a2);
        if r > 0 {
            return r;
        }
    }
    let element = |t: TypeId| match *conversions.ts.data(t) {
        TypeData::Array { elem, .. } => Some(elem),
        TypeData::Pointer(e) | TypeData::ByRef(e) | TypeData::Pinned(e) => Some(e),
        _ => None,
    };
    if let (Some(e1), Some(e2)) = (element(t1), element(t2)) {
        return more_specific_formal_parameter(conversions, e1, e2);
    }
    0
}

/// By-value parameters are better than `in` parameters.
fn better_parameter_passing_choice(c1: &Candidate, c2: &Candidate) -> u8 {
    let mut c1_is_better = false;
    let mut c2_is_better = false;
    for (p1, p2) in c1.parameters.iter().zip(&c2.parameters) {
        match (p1.ref_kind, p2.ref_kind) {
            (RefKind::None, RefKind::In) => c1_is_better = true,
            (RefKind::In, RefKind::None) => c2_is_better = true,
            _ => {}
        }
    }
    if c1_is_better && !c2_is_better {
        1
    } else if !c1_is_better && c2_is_better {
        2
    } else {
        0
    }
}

/// The better params collection type (params collections proposal, better function member).
fn better_params_collection_type(
    conversions: &mut Conversions<'_, '_, '_>,
    p1: TypeId,
    p2: TypeId,
) -> u8 {
    use KnownTypeCode::{ReadOnlySpanOfT as Ros, SpanOfT as Span};
    let is_span = |c: &Conversions<'_, '_, '_>, t| c.ts.is_known(t, Span) || c.ts.is_known(t, Ros);
    let span1 = is_span(conversions, p1);
    let span2 = is_span(conversions, p2);
    let arg = |c: &Conversions<'_, '_, '_>, t| c.ts.first_type_argument(t).unwrap_or(t);
    let array_element = |c: &Conversions<'_, '_, '_>, t: TypeId| match *c.ts.data(t) {
        TypeData::Array { elem, .. } => Some(elem),
        _ if c.ts.is_array_interface(t) => c.ts.first_type_argument(t),
        _ => None,
    };
    if !span1 && !span2 {
        let to2 = conversions.implicit_type(p1, p2).is_valid();
        let to1 = conversions.implicit_type(p2, p1).is_valid();
        if to2 && !to1 {
            return 1;
        }
        if !to2 && to1 {
            return 2;
        }
    } else if conversions.ts.is_known(p1, Ros) && conversions.ts.is_known(p2, Span) {
        // ReadOnlySpan<T> beats Span<T> with identical element types
        let (a, b) = (arg(conversions, p1), arg(conversions, p2));
        if conversions.identity(a, b) {
            return 1;
        }
    } else if conversions.ts.is_known(p2, Ros) && conversions.ts.is_known(p1, Span) {
        let (a, b) = (arg(conversions, p2), arg(conversions, p1));
        if conversions.identity(a, b) {
            return 2;
        }
    } else if span1 && let Some(e2) = array_element(conversions, p2) {
        // a span beats an array type with identical element types
        let a = arg(conversions, p1);
        if conversions.identity(a, e2) {
            return 1;
        }
    } else if span2 && let Some(e1) = array_element(conversions, p1) {
        let a = arg(conversions, p2);
        if conversions.identity(a, e1) {
            return 2;
        }
    }
    0
}

/// `ConstraintValidatingSubstitution`: whether every parameterized type in `t` that the
/// substitution changes satisfies the constraints of its type parameters (C# 4.0 spec: 4.4.4).
fn validate_constructed_types(
    conversions: &mut Conversions<'_, '_, '_>,
    t: TypeId,
    class: Option<&[TypeId]>,
    method: &[TypeId],
) -> bool {
    let children: Vec<TypeId> = match conversions.ts.data(t).clone() {
        TypeData::Inst { generic, args } => {
            let mut all = vec![generic];
            all.extend(args.iter());
            all
        }
        TypeData::Array { elem: e, .. }
        | TypeData::Pointer(e)
        | TypeData::ByRef(e)
        | TypeData::Pinned(e)
        | TypeData::Annotated { inner: e, .. } => vec![e],
        TypeData::Modified {
            modifier, inner, ..
        } => vec![modifier, inner],
        TypeData::Tuple { elements, .. } => elements.to_vec(),
        TypeData::FnPtr(f) => {
            let mut all = vec![f.ret];
            all.extend(f.params.iter());
            all
        }
        _ => Vec::new(),
    };
    for c in children {
        if !validate_constructed_types(conversions, c, class, method) {
            return false;
        }
    }
    let TypeData::Inst { .. } = conversions.ts.data(t) else {
        return true;
    };
    let new_type = conversions.ts.substitute(t, class, Some(method));
    if new_type == t {
        return true;
    }
    let TypeData::Inst { generic, args } = conversions.ts.data(new_type).clone() else {
        return true;
    };
    // `ParameterizedType.TypeParameters`: those of the definition, also where the generic
    // type carries a nullability annotation.
    let Some(definition) = conversions.ts.definition(generic) else {
        return true;
    };
    let generic = conversions.ts.intern_def(definition);
    let type_parameters = conversions.ts.type_arguments(generic);
    for (i, &tp) in type_parameters.iter().enumerate() {
        let Some(&argument) = args.get(i) else {
            continue;
        };
        if !validate_constraints(conversions, tp, argument, Some(&args), None) {
            return false;
        }
    }
    true
}

/// `OverloadResolution.ValidateConstraints`: whether `argument` satisfies the constraints of
/// the type parameter `type_parameter`, with the constraint types under the substitution
/// (`class`, `method`).
pub fn validate_constraints(
    conversions: &mut Conversions<'_, '_, '_>,
    type_parameter: TypeId,
    argument: TypeId,
    class: Option<&[TypeId]>,
    method: Option<&[TypeId]>,
) -> bool {
    // void, null and pointers cannot be type arguments
    if matches!(
        conversions.ts.kind(argument),
        TypeKind::Void | TypeKind::Null | TypeKind::Pointer
    ) {
        return false;
    }
    let TypeData::TypeParam { owner, index } = *conversions.ts.data(type_parameter) else {
        return true;
    };
    let Some(info) = conversions.ts.type_parameter_info(owner, index) else {
        return true;
    };
    if info.reference_type && conversions.ts.is_reference_type(argument) != Some(true) {
        return false;
    }
    if info.value_type && !conversions.ts.is_non_nullable_value_type(argument) {
        return false;
    }
    if info.default_ctor {
        let ts = &mut *conversions.ts;
        // `AbstractTypeParameter.GetConstructors`: a type parameter has the dummy public
        // parameterless constructor exactly when it has a `struct` or `new()` constraint.
        let has_public_parameterless =
            if let TypeData::TypeParam { owner, index } = *ts.data(argument) {
                ts.type_parameter_info(owner, index)
                    .is_some_and(|i| i.default_ctor || i.value_type)
            } else {
                if let Some(def) = ts.definition(argument)
                    // TypeAttributes.Abstract
                    && ts.comp.type_def(def).is_ok_and(|i| i.flags & 0x80 != 0)
                {
                    return false;
                }
                let constructors = ts
                    .get_constructors(
                        argument,
                        MemberOptions {
                            ignore_inherited: true,
                            definitions: true,
                        },
                    )
                    .unwrap_or_default();
                constructors.iter().any(|m| {
                    m.base_params.is_empty()
                        && match m.origin {
                            MethodOrigin::Def(id) => ts
                                .method_accessibility_of(id)
                                .is_ok_and(|a| a == ACC_PUBLIC),
                            MethodOrigin::DummyConstructor(_) => true,
                            MethodOrigin::Fake => false,
                        }
                })
            };
        if !has_public_parameterless {
            return false;
        }
    }
    for constraint in info.constraints {
        let c = conversions.ts.substitute(constraint, class, method);
        if !conversions.is_constraint_convertible(argument, c) {
            return false;
        }
    }
    true
}
