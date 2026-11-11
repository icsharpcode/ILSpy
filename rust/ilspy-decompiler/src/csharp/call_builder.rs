//! `CallBuilder`: method calls, and the call case of `Disambiguator`.
//!
//! The calls translated are those of ordinary non-generic methods with by-value arguments, no
//! `params` or optional parameters, written as `target.Name(arguments)` or `Name(arguments)`.
//! The special forms of `CallBuilder.Build` (delegate construction and invocation, tuples,
//! string concatenation and interpolation, ranges, accessors, operators, extension methods,
//! span helpers) fall back.

use crate::il::ast::{CallData, Inst, MethodOperand, NodeId};
use crate::il::transforms::{AccessorBinding, LookupTarget, MethodGroupList};
use crate::semantics::overload_resolution::OverloadResolutionErrors;
use crate::semantics::types::TypeKind;
use crate::semantics::{Conversions, Method, MethodKey, MethodList, OverloadResolution};
use crate::typesystem::attrs::ACC_PROTECTED;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::RefKind;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `MethodSemanticsAttributes.Setter` and `Getter`.
const ACCESSOR_SETTER: u16 = 0x01;
const ACCESSOR_GETTER: u16 = 0x02;

/// The opcode of the call (`ExpectedTargetDetails.CallOpCode`).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub(crate) enum CallOp {
    Call,
    CallVirt,
}

/// `ArgumentList` for calls without `params`, optional arguments or names of their own.
struct ArgumentList {
    arguments: Vec<TranslatedExpression>,
    parameter_names: Vec<String>,
    is_primitive_value: Vec<bool>,
    add_names_to_primitive_values: bool,
}

impl ArgumentList {
    /// `GetArgumentNames()`: names for the bool literals, where every parameter has a name.
    fn argument_names(&self) -> Option<Vec<Option<String>>> {
        if !self.add_names_to_primitive_values
            || !self.is_primitive_value.iter().any(|&p| p)
            || self.parameter_names.iter().any(String::is_empty)
        {
            return None;
        }
        Some(
            self.is_primitive_value
                .iter()
                .zip(&self.parameter_names)
                .map(|(&primitive, name)| primitive.then(|| name.clone()))
                .collect(),
        )
    }
}

impl Builder<'_, '_, '_, '_> {
    /// `CallBuilder.Build(inst)` for `call` and `callvirt`.
    pub(crate) fn call(
        &mut self,
        inst: NodeId,
        call: &CallData,
        op: CallOp,
    ) -> Result<TranslatedExpression, Unsupported> {
        if call.is_tail {
            return Err("a tail call".to_owned());
        }
        if call.constrained_to.is_some() {
            return Err("a constrained call".to_owned());
        }
        let operand = &call.method;
        if operand.local_function.is_some()
            || operand.specialized_by.is_some()
            || operand.range_accessor.is_some()
            || operand.lifted
        {
            return Err(
                "a call of a local function, proxy, range accessor or lifted operator".to_owned(),
            );
        }
        let resolved = &operand.method;
        let MethodOrigin::Def(id) = resolved.origin else {
            return Err("a call of an unresolved method".to_owned());
        };
        if resolved.varargs.is_some() {
            return Err("a vararg call".to_owned());
        }
        let method =
            Method::from_resolved(self.ts, resolved).ok_or("a call of an unresolved method")?;
        if !method.type_parameters.is_empty() {
            return Err("a call of a generic method".to_owned());
        }
        if self
            .ts
            .is_explicit_interface_implementation(id)
            .unwrap_or(true)
        {
            return Err("a call of an explicit interface implementation".to_owned());
        }
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // `IsWrittenAsMemberAccess`: the name of the property the accessor belongs to.
        let property = match self.ts.accessor_owner(id) {
            None => None,
            Some((token, semantics)) => {
                if token >> 24 != 0x17 {
                    return Err("an event accessor call".to_owned());
                }
                let parameters = usize::from(semantics == ACCESSOR_SETTER);
                if !matches!(semantics, ACCESSOR_GETTER | ACCESSOR_SETTER)
                    || method.parameters.len() != parameters
                {
                    return Err("an indexer or parameterized property access".to_owned());
                }
                // `HandleRangeConstruction` comes first and writes `..` for `Range.All`.
                if self.ts.is_known(declaring, KnownTypeCode::Range)
                    || self.ts.is_known(declaring, KnownTypeCode::Index)
                {
                    return Err("a property of Range or Index".to_owned());
                }
                Some(self.property_info(id.module, token & 0x00FF_FFFF)?.0)
            }
        };
        if property.is_none() {
            if self.ts.method_flags(id, None).extension {
                return Err("an extension method call".to_owned());
            }
            if method.is_operator {
                return Err("an operator call".to_owned());
            }
            self.check_special_calls(&method, declaring)?;
        }

        // the target
        let first_param = usize::from(!method.is_static);
        let this_arg = call.args.first().copied().filter(|_| !method.is_static);
        if let Some(t) = this_arg
            && matches!(self.f.inst(t), Inst::LdObjIfRef { .. })
        {
            return Err("a call on a constrained target".to_owned());
        }
        let target =
            self.translate_target(this_arg, op == CallOp::Call, method.is_static, declaring)?;

        // the arguments
        if call.args.len() != first_param + method.parameters.len() {
            return Err("a call with mismatched arguments".to_owned());
        }
        let mut list = ArgumentList {
            arguments: Vec::with_capacity(method.parameters.len()),
            parameter_names: method.parameters.iter().map(|p| p.name.clone()).collect(),
            is_primitive_value: Vec::with_capacity(method.parameters.len()),
            add_names_to_primitive_values: self.settings.named_arguments
                && self.settings.non_trailing_named_arguments,
        };
        let declaring_is_nullable = self.ts.is_known(declaring, KnownTypeCode::NullableOfT);
        for (i, parameter) in method.parameters.iter().enumerate() {
            if parameter.ref_kind != RefKind::None {
                return Err("a by-reference argument".to_owned());
            }
            if parameter.is_params {
                return Err("a params argument".to_owned());
            }
            if parameter.is_optional {
                return Err("an argument of an optional parameter".to_owned());
            }
            if self.ts.kind(parameter.ty) == TypeKind::Dynamic {
                return Err("an argument of a dynamic parameter".to_owned());
            }
            let arg = self.translate(call.args[first_param + i], Some(parameter.ty))?;
            if self.ts.kind(arg.ty) == TypeKind::Dynamic {
                return Err("a dynamic argument".to_owned());
            }
            let primitive = matches!(self.rr_of(arg.expression), Rr::Constant { .. })
                && !declaring_is_nullable
                && self.ts.is_known(parameter.ty, KnownTypeCode::Boolean);
            list.is_primitive_value.push(primitive);
            let arg = self.convert_to(arg, parameter.ty, true)?;
            list.arguments.push(arg);
        }

        if let Some(name) = property {
            return self.accessor_call(inst, operand, &method, op, target, list, name, declaring);
        }

        // `GetRequiredTransformationsForCall`
        let declaring_def = self.ts.definition(declaring);
        let base_needed = op != CallOp::CallVirt && method.is_overridable;
        let require = method.name == ".ctor"
            || method.name == ".cctor"
            || self.requires_qualifier_dispatching(
                &method.name,
                method.is_static,
                declaring_def,
                target.expression,
                base_needed,
            );
        let require =
            self.disambiguate_call(&method, id, op, &target, &mut list, require, declaring)?;

        // the invocation
        let callee = if require {
            self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name: method.name.clone(),
            })
        } else {
            self.ast.add(AstNode::Identifier {
                name: method.name.clone(),
            })
        };
        let names = list.argument_names();
        let mut arguments = Vec::with_capacity(list.arguments.len());
        for (i, a) in list.arguments.iter().enumerate() {
            match names.as_ref().and_then(|n| n[i].clone()) {
                Some(name) => arguments.push(self.ast.add(AstNode::NamedArgument {
                    name,
                    expression: a.expression,
                })),
                None => arguments.push(a.expression),
            }
        }
        let e = self.ast.add(AstNode::Invocation {
            target: callee,
            arguments,
        });
        let target_rr = self.rr_of(target.expression);
        self.ast.with_rr(
            e,
            Rr::Invocation {
                target: Box::new(target_rr),
                method: Box::new(operand.clone()),
                ty: method.return_type,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: method.return_type,
        })
    }

    /// `HandleAccessorCall` for a property without parameters: `target.Name` or `Name`, and
    /// for a setter the assignment of its value to that.
    #[allow(clippy::too_many_arguments)]
    fn accessor_call(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
        method: &Method,
        op: CallOp,
        target: TranslatedExpression,
        list: ArgumentList,
        name: String,
        declaring: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let declaring_def = self.ts.definition(declaring);
        let require = self.requires_qualifier_dispatching(
            &name,
            method.is_static,
            declaring_def,
            target.expression,
            op != CallOp::CallVirt && method.is_overridable,
        );
        let require = self.disambiguate_accessor(operand, op, &target, require, declaring)?;
        // A setter's only argument is the value, which is not part of the member reference.
        let value = list.arguments.first().copied();
        let ty = match value {
            Some(_) => method.parameters[0].ty,
            None => method.return_type,
        };
        let reference = if require {
            self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name,
            })
        } else {
            self.ast.add(AstNode::Identifier { name })
        };
        let target_rr = self.rr_of(target.expression);
        self.ast.with_rr(
            reference,
            Rr::Property {
                target: Box::new(target_rr),
                accessor: Box::new(operand.clone()),
                ty,
            },
        );
        let Some(value) = value else {
            self.ast.with_il(reference, inst);
            return Ok(TranslatedExpression {
                expression: reference,
                ty,
            });
        };
        let e = self.ast.add(AstNode::Assignment {
            left: reference,
            right: value.expression,
        });
        self.ast.with_rr(e, Rr::Type(ty));
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `Disambiguator.ForAccessor` without arguments to cast: whether the property is written
    /// with its target, qualified and then cast while its name does not bind back to it.
    fn disambiguate_accessor(
        &mut self,
        accessor: &MethodOperand,
        op: CallOp,
        target: &TranslatedExpression,
        require: bool,
        declaring: TypeId,
    ) -> Result<bool, Unsupported> {
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let lookup_target = match self.rr_of(target.expression) {
            Rr::This {
                ty,
                causes_non_virtual_invocation,
                ..
            } => LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            },
            Rr::Type(ty) => LookupTarget::Type(ty),
            _ => LookupTarget::Expression(target.ty),
        };
        let mut require = require;
        let mut spent_require = require;
        let mut spent_cast = false;
        loop {
            let probe_target = require.then_some(lookup_target);
            match resolver.accessor_reference(
                self.ts,
                accessor,
                probe_target,
                op == CallOp::CallVirt,
            ) {
                AccessorBinding::Owner => return Ok(require),
                // The resolve result would name the member found.
                AccessorBinding::Other => {
                    return Err("a property name that binds to another declaration".to_owned());
                }
                AccessorBinding::None => {}
            }
            if !spent_require {
                require = true;
                spent_require = true;
                continue;
            }
            if !spent_cast {
                spent_cast = true;
                // `Target.ConvertTo(declaringType)`
                self.convert_to(*target, declaring, false)?;
                continue;
            }
            // No spelling resolves; the most explicit one is emitted anyway.
            return Ok(require);
        }
    }

    /// The forms of `CallBuilder.Build` that are no plain invocation.
    fn check_special_calls(
        &mut self,
        method: &Method,
        declaring: TypeId,
    ) -> Result<(), Unsupported> {
        let ts = &*self.ts;
        let kind = ts.kind(declaring);
        let in_namespace = |ns: &str, name: &str| {
            ts.definition(declaring)
                .and_then(|d| ts.comp.type_def(d).ok())
                .is_some_and(|i| i.full_name.top.ns == ns && i.full_name.name() == name)
        };
        let special = (kind == TypeKind::Delegate && method.name == "Invoke")
            || (ts.is_known(declaring, KnownTypeCode::String)
                && matches!(method.name.as_str(), "Format" | "Concat"))
            || in_namespace(
                "System.Runtime.CompilerServices",
                "FormattableStringFactory",
            )
            || ts.is_known(declaring, KnownTypeCode::Range)
            || ts.is_known(declaring, KnownTypeCode::Index)
            || in_namespace("System", "MemoryExtensions")
            || ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT)
            || ts.is_known(declaring, KnownTypeCode::SpanOfT)
            || in_namespace("", "<PrivateImplementationDetails>")
            || ts.is_known(declaring, KnownTypeCode::NullableOfT)
            || ts.is_known(declaring, KnownTypeCode::Delegate);
        if special {
            return Err(format!("a call of {} with its own syntax", method.name));
        }
        Ok(())
    }

    /// `Disambiguator.ForCall`: whether the call is written with its target, escalating as
    /// the managed steps do while the name does not resolve back to the method.
    #[allow(clippy::too_many_arguments)]
    fn disambiguate_call(
        &mut self,
        method: &Method,
        id: crate::typesystem::members::MethodDefId,
        op: CallOp,
        target: &TranslatedExpression,
        list: &mut ArgumentList,
        require: bool,
        declaring: TypeId,
    ) -> Result<bool, Unsupported> {
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let lookup_target = match self.rr_of(target.expression) {
            Rr::This {
                ty,
                causes_non_virtual_invocation,
                ..
            } => LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            },
            Rr::Type(ty) => LookupTarget::Type(ty),
            _ => LookupTarget::Expression(target.ty),
        };
        // `skipTargetCast`: a protected member of a base type cannot be reached through a cast.
        let declaring_def = self.ts.definition(declaring);
        let skip_target_cast = self
            .ts
            .method_accessibility_of(id)
            .is_ok_and(|a| a <= ACC_PROTECTED)
            && match (self.current_type, declaring_def) {
                (Some(current), Some(d)) => {
                    let t = self.ts.intern_def(current);
                    self.ts.all_base_type_definitions(t).contains(&d)
                }
                _ => false,
            };
        let initially_required = require;
        let mut require = require;
        // The steps of `Disambiguator.CallSteps`, spent once each.
        let mut spent_names = false;
        let mut spent_cast_arguments = false;
        let mut spent_require = require;
        let mut spent_cast_target = false;
        let mut spent_type_arguments = false;
        let mut spent_explicit_in = false;
        loop {
            let probe_target = require.then_some(lookup_target);
            let errors = self.probe_call(resolver, method, op, probe_target, target, list);
            if errors == OverloadResolutionErrors::NONE {
                return Ok(require);
            }
            // `TryRepair`
            if (errors == OverloadResolutionErrors::TYPE_INFERENCE_FAILED
                || errors == OverloadResolutionErrors::WRONG_NUMBER_OF_TYPE_ARGUMENTS)
                && !spent_type_arguments
            {
                // A non-generic method has no type arguments to write.
                spent_type_arguments = true;
                continue;
            }
            // `Escalate`
            if !spent_names && list.add_names_to_primitive_values {
                spent_names = true;
                list.add_names_to_primitive_values = false;
                continue;
            }
            if !spent_cast_arguments {
                spent_cast_arguments = true;
                // `CastArguments`: a cast to the parameter type, translated only where it
                // is none.
                let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
                for (a, p) in list.arguments.clone().iter().zip(&method.parameters) {
                    if self.ts.normalize_with(a.ty, n) != self.ts.normalize_with(p.ty, n) {
                        return Err("a call that needs argument casts".to_owned());
                    }
                }
                continue;
            }
            if !spent_require {
                spent_require = true;
                require = true;
                continue;
            }
            if !spent_cast_target {
                spent_cast_target = true;
                if skip_target_cast && require != initially_required {
                    require = false;
                } else {
                    self.convert_to(*target, declaring, false)?;
                }
                continue;
            }
            if !spent_type_arguments {
                spent_type_arguments = true;
                continue;
            }
            if !spent_explicit_in {
                // No `in` parameter to make explicit.
                spent_explicit_in = true;
                continue;
            }
            // No spelling resolves; the most explicit one is written anyway.
            return Ok(require);
        }
    }

    /// `Disambiguator.ProbeCall`: the overload resolution of the current spelling, over the
    /// method group the host looks the name up to.
    fn probe_call(
        &mut self,
        resolver: &dyn crate::il::transforms::Resolver,
        method: &Method,
        op: CallOp,
        lookup_target: Option<LookupTarget>,
        target: &TranslatedExpression,
        list: &ArgumentList,
    ) -> OverloadResolutionErrors {
        let Some(group) = resolver.method_group(self.ts, &method.name, lookup_target) else {
            return OverloadResolutionErrors::AMBIGUOUS_MATCH;
        };
        let lookup_type = lookup_target.map(|_| target.ty);
        let lists = self.method_lists(&group, lookup_type);
        let arguments = list
            .arguments
            .iter()
            .map(|a| self.semantic_rr(a.expression, a.ty))
            .collect();
        let names = list.argument_names();
        let mut conversions = Conversions::new(self.ts);
        let mut or = OverloadResolution::new(arguments, names, Vec::new());
        or.add_method_lists(&mut conversions, &lists);
        // `CheckBestCandidate`
        let errors = or.best_candidate_errors(&mut conversions);
        if errors != OverloadResolutionErrors::NONE {
            return errors;
        }
        if or.is_ambiguous() {
            return OverloadResolutionErrors::AMBIGUOUS_MATCH;
        }
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return OverloadResolutionErrors::AMBIGUOUS_MATCH;
        };
        if !self.is_appropriate_call_target(op, method, &found) {
            return OverloadResolutionErrors::AMBIGUOUS_MATCH;
        }
        // The normal form was meant; no argument was left out.
        if or.best_candidate_is_expanded_form() {
            return OverloadResolutionErrors::AMBIGUOUS_MATCH;
        }
        OverloadResolutionErrors::NONE
    }

    /// The host's method group as the overload resolution's method lists: each definition
    /// specialized by the base type of the lookup type (or of the current type and the types
    /// containing it, for a simple name) that declares it.
    fn method_lists(
        &mut self,
        group: &[MethodGroupList],
        lookup_type: Option<TypeId>,
    ) -> Vec<MethodList> {
        let mut scopes = Vec::new();
        match lookup_type {
            Some(t) => scopes.push(t),
            None => {
                let mut current = self.current_type;
                while let Some(c) = current {
                    scopes.push(self.ts.intern_def(c));
                    current = self
                        .ts
                        .comp
                        .type_def(c)
                        .ok()
                        .filter(|i| i.declaring != 0)
                        .map(|i| TypeDefId {
                            module: c.module,
                            row: i.declaring,
                        });
                }
            }
        }
        let mut result = Vec::with_capacity(group.len());
        for list in group {
            let mut declaring = self.ts.intern_def(list.declaring);
            'scopes: for &scope in &scopes {
                for base in self.ts.all_base_types(scope).unwrap_or_default() {
                    if self.ts.definition(base) == Some(list.declaring) {
                        declaring = base;
                        break 'scopes;
                    }
                }
            }
            let class_args = match self.ts.data(declaring) {
                TypeData::Inst { args, .. } => Some(args.to_vec()),
                _ => None,
            };
            let mut methods = Vec::with_capacity(list.methods.len());
            for &m in &list.methods {
                let Ok(definition) = self.ts.method_definition(m) else {
                    continue;
                };
                let specialized = if class_args.is_some() {
                    self.ts.specialize(definition, class_args.clone(), None)
                } else {
                    definition
                };
                if let Some(method) = Method::from_resolved(self.ts, &specialized) {
                    methods.push(method);
                }
            }
            result.push(MethodList { declaring, methods });
        }
        result
    }

    /// The type arguments a method is specialized with, the definition's own where it has
    /// none.
    fn effective_type_arguments(&mut self, m: &Method) -> (Vec<TypeId>, Vec<TypeId>) {
        let class = match &m.class_args {
            Some(a) => a.clone(),
            None => m
                .declaring
                .and_then(|d| self.ts.definition(d))
                .map(|d| {
                    let t = self.ts.intern_def(d);
                    self.ts.type_arguments(t)
                })
                .unwrap_or_default(),
        };
        let method = m
            .method_args
            .clone()
            .unwrap_or_else(|| m.type_parameters.clone());
        let n = Normalization::TYPE_ERASURE;
        (
            class
                .into_iter()
                .map(|t| self.ts.normalize_with(t, n))
                .collect(),
            method
                .into_iter()
                .map(|t| self.ts.normalize_with(t, n))
                .collect(),
        )
    }

    /// `expected.Equals(actual, NormalizeTypeVisitor.TypeErasure)`.
    fn same_method_after_type_erasure(&mut self, expected: &Method, actual: &Method) -> bool {
        expected.key == actual.key
            && self.effective_type_arguments(expected) == self.effective_type_arguments(actual)
    }

    /// `Disambiguator.IsAppropriateCallTarget`: the method found is the one called, or for a
    /// virtual call an override of it.
    fn is_appropriate_call_target(
        &mut self,
        op: CallOp,
        expected: &Method,
        actual: &Method,
    ) -> bool {
        if self.same_method_after_type_erasure(expected, actual) {
            return true;
        }
        let MethodKey::Def(actual_id) = actual.key else {
            return false;
        };
        if op != CallOp::CallVirt
            || !self
                .ts
                .method_modifiers_of(actual_id)
                .is_ok_and(|m| m.is_override)
        {
            return false;
        }
        // shortcut: an override found in place of the base method would be written as a call of
        // the override, with its resolve result; such calls are not translated yet. Upgrade with
        // the base member chain (`InheritanceHelper.GetBaseMembers`) when they are.
        false
    }
}
