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
use crate::semantics::{
    Conversions, Method, MethodKey, MethodList, OverloadResolution, ResolveResult, TypeInference,
    TypeInferenceAlgorithm,
};
use crate::typesystem::attrs::ACC_PROTECTED;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{ConstValue, RefKind};
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `MethodSemanticsAttributes.Setter` and `Getter`.
pub(crate) const ACCESSOR_SETTER: u16 = 0x01;
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
    /// `FirstOptionalArgumentIndex`: -2 for no optional arguments, -1 where they are not
    /// allowed, else the first of the trailing arguments that are the parameters' defaults.
    first_optional: i32,
    /// The parameters' reference kinds, in argument order.
    ref_kinds: Vec<RefKind>,
    /// `UseImplicitlyTypedOut`: `out` arguments are written and resolved as `out var`.
    use_implicitly_typed_out: bool,
    /// `ArgumentToParameterMap` of a call with named arguments: the parameter of each argument.
    parameter_map: Option<Vec<usize>>,
    /// `ArgumentNames`: the names written from the first argument out of its place on.
    explicit_names: Option<Vec<Option<String>>>,
}

impl ArgumentList {
    /// `GetActualArgumentCount()`: the arguments written.
    fn actual_count(&self) -> usize {
        match usize::try_from(self.first_optional) {
            Ok(first) => first,
            Err(_) => self.arguments.len(),
        }
    }

    /// `GetArgumentNames()`: the explicit names, and names for the bool literals where every
    /// parameter has a name, up to the arguments written.
    fn argument_names(&self) -> Option<Vec<Option<String>>> {
        let mut names = self.explicit_names.clone();
        if self.add_names_to_primitive_values
            && self.is_primitive_value.iter().any(|&p| p)
            && !self.parameter_names.iter().any(String::is_empty)
        {
            let names = names.get_or_insert_with(|| vec![None; self.arguments.len()]);
            for (i, &primitive) in self.is_primitive_value.iter().enumerate() {
                if primitive && names[i].is_none() {
                    names[i] = Some(self.parameter_names[i].clone());
                }
            }
        }
        let mut names = names?;
        names.truncate(self.actual_count());
        Some(names)
    }

    /// `ExpectedParameters`: the method's parameters in argument order.
    fn expected_parameters(&self, method: &Method) -> Vec<crate::semantics::Parameter> {
        match &self.parameter_map {
            Some(map) => map.iter().map(|&p| method.parameters[p].clone()).collect(),
            None => method.parameters.clone(),
        }
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
        if self
            .ts
            .is_explicit_interface_implementation(id)
            .unwrap_or(true)
        {
            return Err("a call of an explicit interface implementation".to_owned());
        }
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // `IsWrittenAsMemberAccess`: the name of the property the accessor belongs to; for an
        // indexer whether the accessor is its setter.
        let mut indexer: Option<bool> = None;
        // For an event's add or remove accessor: the compound assignment operator, `+=` (1) or
        // `-=` (2) as `AssignmentOperatorType`.
        let mut event_op: Option<u8> = None;
        let property = match self.ts.accessor_owner(id) {
            None => None,
            Some((token, semantics)) if token >> 24 == 0x14 => {
                event_op = Some(match semantics {
                    0x08 => 1,
                    0x10 => 2,
                    _ => return Err("an event raise call".to_owned()),
                });
                let md = &self.ts.comp.module(id.module).md;
                let e = md
                    .get::<crate::metadata::tables::rows::Event>(token & 0x00FF_FFFF)
                    .map_err(|e| e.to_string())?;
                Some(md.string(e.name).map_err(|e| e.to_string())?.into_owned())
            }
            Some((token, semantics)) => {
                if token >> 24 != 0x17 {
                    return Err("an accessor of another kind".to_owned());
                }
                let parameters = usize::from(semantics == ACCESSOR_SETTER);
                if !matches!(semantics, ACCESSOR_GETTER | ACCESSOR_SETTER) {
                    return Err("an indexer or parameterized property access".to_owned());
                }
                if method.parameters.len() != parameters {
                    // `SymbolKind.Indexer`: a property with parameters named as the type's
                    // `[DefaultMember]`; other parameterized properties keep the call syntax.
                    let name = self.property_info(id.module, token & 0x00FF_FFFF)?.0;
                    let default_member = match self.ts.definition(declaring) {
                        Some(def) => self
                            .ts
                            .default_member_name(def)
                            .map_err(|e| e.to_string())?,
                        None => None,
                    };
                    if method.parameters.len() < parameters
                        || default_member.as_deref() != Some(&name)
                    {
                        return Err("a parameterized property access".to_owned());
                    }
                    indexer = Some(semantics == ACCESSOR_SETTER);
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
        let is_extension = self.ts.method_flags(id, None).extension;
        if property.is_none() {
            self.check_special_calls(&method, declaring)?;
        }

        // the target
        let first_param = usize::from(!method.is_static);
        let constrained_to = call.constrained_to.as_ref().map(|t| t.ty);
        let mut this_arg = call.args.first().copied().filter(|_| !method.is_static);
        if let Some(t) = this_arg
            && let Inst::LdObjIfRef { target, .. } = *self.f.inst(t)
        {
            if constrained_to.is_none() {
                return Err("a call on a constrained target".to_owned());
            }
            this_arg = Some(target);
        }
        let mut target = self.translate_constrained_target(
            this_arg,
            op == CallOp::Call,
            method.is_static,
            declaring,
            constrained_to,
        )?;

        // the arguments
        let is_setter =
            (property.is_some() && method.parameters.len() == 1) || indexer == Some(true);
        let first_argument_node = self.ast.nodes.len();
        let mut list = self.argument_list(call, &method, id, first_param, declaring, is_setter)?;
        // Named arguments of an extension method, an accessor or a delegate invocation are not
        // translated.
        if list.parameter_map.is_some()
            && (is_extension
                || property.is_some()
                || indexer.is_some()
                || (method.name == "Invoke" && self.ts.kind(declaring) == TypeKind::Delegate))
        {
            return Err("named arguments of an extension, accessor or delegate call".to_owned());
        }

        // `IsInterpolatedStringCreation` for `string.Format`: an interpolated string where the
        // format string tokenizes, else an ordinary call. A `params` overload, written as an
        // interpolated string only for an array literal or the expanded form, is not translated.
        if self.settings.string_interpolation
            && method.is_static
            && method.name == "Format"
            && self.ts.is_known(declaring, KnownTypeCode::String)
        {
            if method.parameters.last().is_some_and(|p| p.is_params) {
                return Err("a string.Format call of a params overload".to_owned());
            }
            if let Some(e) = self.format_interpolation(inst, &list, first_argument_node)? {
                return Ok(e);
            }
        }

        // `HandleImplicitConversion`: an implicit conversion operator is written as the conversion.
        if method.is_operator && method.name == "op_Implicit" && list.arguments.len() == 1 {
            return self.implicit_conversion_call(inst, &method, &list);
        }

        // A delegate's `Invoke` is the invocation of the delegate itself, unless the target is
        // null-conditional: `d?.Invoke(...)` is an ordinary call.
        if method.name == "Invoke"
            && self.ts.kind(declaring) == TypeKind::Delegate
            && !matches!(
                self.ast.node(target.expression),
                AstNode::NullConditional { rewrap: false, .. }
            )
        {
            if self.is_cast_target(target.expression) {
                return Err("a cast as the target of a member or reference".to_owned());
            }
            let arguments = self.written_arguments(&list, 0);
            let e = self.ast.add(AstNode::Invocation {
                target: target.expression,
                arguments,
            });
            let target_rr = self.rr_of(target.expression);
            self.ast.with_rr(
                e,
                Rr::Invocation {
                    target: Box::new(target_rr),
                    method: Box::new(operand.clone()),
                    ty: method.return_type,
                    extension: false,
                    delegate: true,
                    found_by_lookup: false,
                },
            );
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: method.return_type,
            });
        }
        if let Some(is_setter) = indexer {
            return self.indexer_access(
                inst, operand, id, &method, target, list, is_setter, declaring,
            );
        }
        if let Some(name) = property {
            return self.accessor_call(
                inst, operand, &method, op, target, list, name, declaring, event_op,
            );
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
        self.found_override = None;
        // A cast the target already is (the boxing `NeedsBoxingConversion` unwraps, or a cast of
        // another kind) is not translated; only the cast `CastTarget` writes is.
        if self.is_cast_target(target.expression) {
            return Err("a cast as the target of a member or reference".to_owned());
        }
        let (require, require_type_arguments) =
            self.disambiguate_call(&method, id, op, &mut target, &mut list, require, declaring)?;
        // The managed builder comments a cast target of a `constrained.` call, which is not
        // translated.
        if constrained_to.is_some() && self.is_cast_target(target.expression) {
            return Err("a cast target of a constrained call".to_owned());
        }
        // The overload the call resolves to may be an override of the method, or the method
        // with other inferred type arguments; the written argument names take its parameter
        // names, and the call its return type.
        let found_override = self.found_override.take();
        if let Some(found) = &found_override {
            if list.parameter_map.is_some() {
                return Err("named arguments of a call that resolves to another method".to_owned());
            }
            if is_extension {
                return Err("an extension call that resolves to another method".to_owned());
            }
            if list.parameter_names.len() >= found.parameters.len() {
                for (n, p) in list.parameter_names.iter_mut().zip(&found.parameters) {
                    n.clone_from(&p.name);
                }
            }
        }

        // the invocation
        let return_type = found_override
            .as_ref()
            .map_or(method.return_type, |f| f.return_type);
        let type_arguments = match &method.method_args {
            Some(args) if require_type_arguments => args.clone(),
            _ => Vec::new(),
        };
        // The managed builder leaves out type arguments with anonymous types.
        if type_arguments
            .iter()
            .any(|&t| self.may_contain_anonymous_type(t))
        {
            return Err("type arguments that may be anonymous types".to_owned());
        }
        if is_extension
            && let Some(e) =
                self.extension_method_call(inst, operand, &method, &list, &type_arguments)?
        {
            return Ok(e);
        }
        let callee = match (require, type_arguments.is_empty()) {
            (true, true) => self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name: method.name.clone(),
            }),
            (false, true) => self.ast.add(AstNode::Identifier {
                name: method.name.clone(),
            }),
            (true, false) => self.ast.add(AstNode::GenericMemberReference {
                target: target.expression,
                name: method.name.clone(),
                type_arguments,
            }),
            (false, false) => self.ast.add(AstNode::GenericIdentifier {
                name: method.name.clone(),
                type_arguments,
            }),
        };
        let arguments = self.written_arguments(&list, 0);
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
                ty: return_type,
                extension: false,
                delegate: false,
                found_by_lookup: found_override.is_some(),
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: return_type,
        })
    }

    /// `HandleImplicitConversion`: `(T)argument`, with the argument first cast to the
    /// parameter type where the implicit conversion from its own type is not the operator.
    fn implicit_conversion_call(
        &mut self,
        inst: NodeId,
        method: &Method,
        list: &ArgumentList,
    ) -> Result<TranslatedExpression, Unsupported> {
        let mut argument = list.arguments[0];
        let target = method.return_type;
        let conversion = Conversions::new(self.ts).implicit_type(argument.ty, target);
        let direct = conversion.is_valid()
            && match &conversion {
                crate::semantics::Conversion::UserDefined { method: found, .. } => {
                    self.same_method_after_type_erasure(method, found)
                }
                _ => false,
            };
        if !direct {
            let parameter = method.parameters[0].ty;
            argument = self.convert_to(argument, parameter, false)?;
        }
        let c = self.ast.add(AstNode::Cast {
            ty: target,
            expression: argument.expression,
            unchecked: false,
        });
        self.ast.with_rr(c, Rr::ImplicitConversion { ty: target });
        self.ast.with_il(c, inst);
        Ok(TranslatedExpression {
            expression: c,
            ty: target,
        })
    }

    /// `TryUseExtensionMethodSyntax`: `first.Name(rest)` where the resolver finds the method
    /// with the first argument as its target; `None` keeps the static call.
    fn extension_method_call(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
        method: &Method,
        list: &ArgumentList,
        type_arguments: &[TypeId],
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        // Without using declarations an extension method's namespace is not imported.
        if list.arguments.is_empty()
            || list.first_optional == 0
            || !self.settings.using_declarations
        {
            return Ok(None);
        }
        // The target is the first argument, written first and positionally.
        let names = list.argument_names();
        if names.as_ref().is_some_and(|n| n[0].is_some()) {
            return Ok(None);
        }
        let first = list.arguments[0];
        let target_rr = self.rr_of(first.expression);
        if matches!(
            target_rr,
            Rr::Constant {
                value: ConstValue::Null,
                ..
            }
        ) {
            return Err("an extension method call on null".to_owned());
        }
        if matches!(
            self.ast.node(first.expression),
            AstNode::RefDirection { .. } | AstNode::OutDirection { .. }
        ) {
            return Err("an extension method call on a reference".to_owned());
        }
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let target = self.semantic_rr(first.expression, first.ty);
        // `GetArgumentResolveResults(skipCount: 1)`
        let arguments = self.argument_resolve_results(list).split_off(1);
        let remaining = names
            .as_ref()
            .map(|n| n[1..].to_vec())
            .filter(|n| n.iter().any(Option::is_some));
        let can = resolver
            .can_use_extension_method_syntax(
                self.ts,
                operand,
                type_arguments,
                &target,
                &arguments,
                remaining.as_deref(),
            )
            .ok_or("an extension method question the host cannot answer")?;
        if !can {
            return Ok(None);
        }
        let callee = if type_arguments.is_empty() {
            self.ast.add(AstNode::MemberReference {
                target: first.expression,
                name: method.name.clone(),
            })
        } else {
            self.ast.add(AstNode::GenericMemberReference {
                target: first.expression,
                name: method.name.clone(),
                type_arguments: type_arguments.to_vec(),
            })
        };
        let rest = self.written_arguments(list, 1);
        let e = self.ast.add(AstNode::Invocation {
            target: callee,
            arguments: rest,
        });
        self.ast.with_rr(
            e,
            Rr::Invocation {
                target: Box::new(target_rr),
                method: Box::new(operand.clone()),
                ty: method.return_type,
                extension: true,
                delegate: false,
                found_by_lookup: false,
            },
        );
        self.ast.with_il(e, inst);
        Ok(Some(TranslatedExpression {
            expression: e,
            ty: method.return_type,
        }))
    }

    /// `BuildArgumentList` for by-value arguments without expanded `params`, optional or
    /// dynamic parameters, each translated and converted to its parameter type.
    fn argument_list(
        &mut self,
        call: &CallData,
        method: &Method,
        id: crate::typesystem::members::MethodDefId,
        first_param: usize,
        declaring: TypeId,
        is_setter: bool,
    ) -> Result<ArgumentList, Unsupported> {
        let infos = self.ts.method_signature(id).1;
        if infos.len() != method.parameters.len() {
            return Err("a call with mismatched parameters".to_owned());
        }
        if call.args.len() != first_param + method.parameters.len() {
            return Err("a call with mismatched arguments".to_owned());
        }
        // The parameter of each argument: the named argument map of `CallWithNamedArgs`.
        let map = self.named_argument_map.take();
        if map.is_some() && (is_setter || method.is_operator) {
            return Err("named arguments of a setter or operator".to_owned());
        }
        let order: Vec<usize> = map
            .clone()
            .unwrap_or_else(|| (0..method.parameters.len()).collect());
        // Names start at the first argument out of its place.
        let mut explicit_names: Option<Vec<Option<String>>> = None;
        for (i, &p) in order.iter().enumerate() {
            if explicit_names.is_none() && p != i {
                explicit_names = Some(vec![None; order.len()]);
            }
            let name = &method.parameters[p].name;
            if let Some(names) = &mut explicit_names
                && crate::il::transforms::assign_variable_names::is_valid_name(name)
            {
                names[i] = Some(name.clone());
            }
        }
        let mut list = ArgumentList {
            arguments: Vec::with_capacity(method.parameters.len()),
            parameter_names: order
                .iter()
                .map(|&p| method.parameters[p].name.clone())
                .collect(),
            is_primitive_value: Vec::with_capacity(method.parameters.len()),
            add_names_to_primitive_values: self.settings.named_arguments
                && self.settings.non_trailing_named_arguments,
            first_optional: if self.settings.optional_arguments {
                -2
            } else {
                -1
            },
            ref_kinds: order
                .iter()
                .map(|&p| method.parameters[p].ref_kind)
                .collect(),
            use_implicitly_typed_out: true,
            parameter_map: map,
            explicit_names,
        };
        let declaring_is_nullable = self.ts.is_known(declaring, KnownTypeCode::NullableOfT);
        for (i, &pi) in order.iter().enumerate() {
            let parameter = &method.parameters[pi];
            // `out` arguments resolve as `OutVarResolveResult`s and `in` arguments may be made
            // explicit, which is not translated.
            if !matches!(
                parameter.ref_kind,
                RefKind::None | RefKind::Ref | RefKind::Out
            ) {
                return Err("an in argument".to_owned());
            }
            if self.ts.kind(parameter.ty) == TypeKind::Dynamic {
                return Err("an argument of a dynamic parameter".to_owned());
            }
            let arg = self.translate(call.args[first_param + i], Some(parameter.ty))?;
            if self.ts.kind(arg.ty) == TypeKind::Dynamic {
                return Err("a dynamic argument".to_owned());
            }
            // `TransformParamsArgument` expands an array creation, `Array.Empty<T>()`, a span
            // constructor or a span conversion of an array creation into the call's arguments,
            // which is not translated; any other argument of a `params` parameter is an
            // ordinary one.
            if parameter.is_params
                && matches!(
                    self.rr_of(arg.expression),
                    Rr::ArrayCreate { .. }
                        | Rr::Invocation { .. }
                        | Rr::ObjectCreation { .. }
                        | Rr::Conversion { .. }
                        | Rr::ImplicitConversion { .. }
                )
            {
                return Err("a params argument".to_owned());
            }
            let primitive = matches!(self.rr_of(arg.expression), Rr::Constant { .. })
                && !declaring_is_nullable
                && self.ts.is_known(parameter.ty, KnownTypeCode::Boolean);
            list.is_primitive_value.push(primitive);
            // The assigned value of a setter does not end the run of optional arguments.
            if !(is_setter && i + 1 == method.parameters.len()) {
                if self.is_optional_argument(id, &infos[pi], arg)? {
                    if list.first_optional == -2 {
                        list.first_optional = i as i32;
                    }
                } else if list.first_optional != -1 {
                    list.first_optional = -2;
                }
            }
            let mut arg = self.convert_to(arg, parameter.ty, true)?;
            // An operator's operand keeps its type: the default literal converts to every type.
            if method.is_operator {
                arg = self.restore_default_literal_type(arg)?;
            }
            if parameter.ref_kind == RefKind::Out {
                arg = self.change_direction_to_out(arg);
            }
            list.arguments.push(arg);
        }
        // Omitted optional arguments among named ones are found through the map, which the
        // engine does not follow.
        if list.parameter_map.is_some() && list.first_optional >= 0 {
            return Err("omitted optional arguments with named arguments".to_owned());
        }
        Ok(list)
    }

    /// `GetArgumentExpressions(skipCount)`: the written arguments from `skip` on, named where
    /// the list names them, with `out` arguments marked as `out var` while the list writes
    /// them so.
    fn written_arguments(&mut self, list: &ArgumentList, skip: usize) -> Vec<AstId> {
        let names = list.argument_names();
        let mut written = Vec::with_capacity(list.actual_count());
        for (i, a) in list.arguments[..list.actual_count()]
            .iter()
            .enumerate()
            .skip(skip)
        {
            if list.use_implicitly_typed_out
                && let AstNode::OutDirection {
                    implicitly_typed, ..
                } = self.ast.node_mut(a.expression)
            {
                *implicitly_typed = true;
            }
            match names.as_ref().and_then(|n| n[i].clone()) {
                Some(name) => written.push(self.ast.add(AstNode::NamedArgument {
                    name,
                    expression: a.expression,
                })),
                None => written.push(a.expression),
            }
        }
        written
    }

    /// `ChangeDirectionExpressionTo(arg, ReferenceKind.Out)`: `ref x` becomes `out x`, with the
    /// `ref`'s instructions and an `out` resolve result.
    fn change_direction_to_out(&mut self, arg: TranslatedExpression) -> TranslatedExpression {
        let AstNode::RefDirection { expression } = *self.ast.node(arg.expression) else {
            return arg;
        };
        let Some(Rr::ByReference(element)) = self.ast.annotations(arg.expression).rr.clone() else {
            return arg;
        };
        let d = self.ast.add(AstNode::OutDirection {
            expression,
            implicitly_typed: false,
        });
        for inst in self.ast.annotations(arg.expression).il.clone() {
            self.ast.with_il(d, inst);
        }
        self.ast.with_rr(d, Rr::ByReferenceOut(element));
        TranslatedExpression {
            expression: d,
            ty: arg.ty,
        }
    }

    /// `GetArgumentResolveResults()`: the written arguments' resolve results, an `out` argument
    /// as `OutVarResolveResult` of its element type while the list writes `out var`.
    fn argument_resolve_results(&self, list: &ArgumentList) -> Vec<ResolveResult> {
        list.arguments[..list.actual_count()]
            .iter()
            .zip(&list.ref_kinds)
            .map(|(a, &kind)| match *self.ts.data(a.ty) {
                TypeData::ByRef(element)
                    if list.use_implicitly_typed_out && kind == RefKind::Out =>
                {
                    ResolveResult::OutVar(element)
                }
                _ => self.semantic_rr(a.expression, a.ty),
            })
            .collect()
    }

    /// `IsOptionalArgument`: a constant argument of an optional parameter that is its default
    /// value, unless the parameter is filled in by the compiler (caller information).
    fn is_optional_argument(
        &mut self,
        method: crate::typesystem::members::MethodDefId,
        info: &crate::typesystem::members::ParamInfo,
        arg: TranslatedExpression,
    ) -> Result<bool, Unsupported> {
        if !info.optional {
            return Ok(false);
        }
        // A constant, or a null literal conversion (whose constant value is null).
        let value = match self.rr_of(arg.expression) {
            Rr::Constant { value, .. } => value,
            Rr::Conversion { ty } => {
                let AstNode::Cast {
                    expression: inner, ..
                } = *self.ast.node(arg.expression)
                else {
                    return Ok(false);
                };
                let input = self.rr_of(inner);
                let input_ty = self.rr_type(&input);
                let input_rr = self.semantic_rr(inner, input_ty);
                if !Conversions::new(self.ts)
                    .explicit(&input_rr, ty)
                    .is_null_literal()
                {
                    return Ok(false);
                }
                ConstValue::Null
            }
            _ => return Ok(false),
        };
        if info.row != 0 {
            let md = &self.ts.comp.module(method.module).md;
            let ns = "System.Runtime.CompilerServices";
            let caller_info = [
                "CallerMemberNameAttribute",
                "CallerFilePathAttribute",
                "CallerLineNumberAttribute",
            ]
            .iter()
            .any(|name| {
                crate::typesystem::flags::has_attribute(
                    md,
                    crate::metadata::tables::TableId::Param,
                    info.row,
                    ns,
                    name,
                )
            });
            if caller_info {
                return Ok(false);
            }
        }
        Ok(info.value == value)
    }

    /// `CallBuilder.Build` for `newobj` of an ordinary constructor): `new T(arguments)`.
    pub(crate) fn new_obj(
        &mut self,
        inst: NodeId,
        call: &CallData,
    ) -> Result<TranslatedExpression, Unsupported> {
        let operand = &call.method;
        if operand.local_function.is_some()
            || operand.specialized_by.is_some()
            || operand.range_accessor.is_some()
            || operand.lifted
        {
            return Err(
                "a newobj of a local function, proxy, range accessor or lifted operator".to_owned(),
            );
        }
        let resolved = &operand.method;
        if !matches!(resolved.origin, MethodOrigin::Def(_)) {
            return Err("a call of an unresolved method".to_owned());
        }
        if resolved.varargs.is_some() {
            return Err("a vararg call".to_owned());
        }
        let method =
            Method::from_resolved(self.ts, resolved).ok_or("a call of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // The forms `VisitNewObj` and `CallBuilder.Build` write with syntax of their own:
        // stackalloc spans, delegates, tuples, ranges and anonymous types; and the native
        // integer the constructor of IntPtr or UIntPtr may create.
        let ts = &*self.ts;
        let name = ts
            .definition(declaring)
            .and_then(|d| ts.comp.type_def(d).ok())
            .map(|i| (i.full_name.top.ns.clone(), i.full_name.name().to_owned()));
        if ts.kind(declaring) == TypeKind::Delegate {
            return self.delegate_construction(inst, call, declaring);
        }
        let special = if ts.is_known(declaring, KnownTypeCode::SpanOfT)
            || ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT)
        {
            Some("a span")
        } else if ts.is_known(declaring, KnownTypeCode::Range)
            || ts.is_known(declaring, KnownTypeCode::Index)
        {
            Some("a range or index")
        } else if ts.is_known(declaring, KnownTypeCode::IntPtr)
            || ts.is_known(declaring, KnownTypeCode::UIntPtr)
        {
            Some("a native integer")
        } else if ts.is_known(declaring, KnownTypeCode::NullableOfT) {
            // `new T?(x)` is written as `x` where it converts implicitly.
            Some("a nullable value")
        } else {
            match name.as_ref() {
                None => Some("an unnamed type"),
                Some((ns, name)) if ns == "System" && name.starts_with("ValueTuple") => {
                    Some("a tuple")
                }
                // `IsAnonymousType`, without its checks for generated names and attributes.
                Some((ns, name))
                    if ns.is_empty()
                        && (name.contains("AnonType") || name.contains("AnonymousType")) =>
                {
                    Some("an anonymous type")
                }
                _ => None,
            }
        };
        if let Some(form) = special {
            return Err(format!(
                "an object creation of {form} with syntax of its own"
            ));
        }
        let MethodOrigin::Def(id) = resolved.origin else {
            return Err("a call of an unresolved method".to_owned());
        };
        let mut list = self.argument_list(call, &method, id, 0, declaring, false)?;
        self.disambiguate_constructor(operand, &method, &mut list)?;
        let arguments = self.written_arguments(&list, 0);
        let e = self.ast.add(AstNode::ObjectCreate {
            ty: declaring,
            arguments,
            initializer: None,
        });
        self.ast.with_rr(
            e,
            Rr::ObjectCreation {
                method: Box::new(operand.clone()),
                ty: declaring,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: declaring,
        })
    }

    /// `Disambiguator.ForConstructorCall`: drop the names of bool literals, then cast the
    /// arguments, while overload resolution over the accessible constructors does not pick
    /// the one called.
    fn disambiguate_constructor(
        &mut self,
        operand: &MethodOperand,
        method: &Method,
        list: &mut ArgumentList,
    ) -> Result<(), Unsupported> {
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let group = resolver
            .constructors(self.ts, operand)
            .ok_or("constructors the host cannot list")?;
        let mut spent_names = false;
        let mut spent_optional = false;
        let mut spent_cast_arguments = false;
        loop {
            let errors =
                self.probe_group(&group, method.declaring, method, CallOp::Call, list, &[])?;
            if errors == OverloadResolutionErrors::NONE {
                return Ok(());
            }
            // `TryRepair` for a missing argument, then the `ConstructorCallSteps`.
            if errors == OverloadResolutionErrors::MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER
                && !spent_optional
                && list.first_optional >= 0
            {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            if !spent_names && list.add_names_to_primitive_values {
                spent_names = true;
                list.add_names_to_primitive_values = false;
                continue;
            }
            if !spent_optional && list.first_optional >= 0 {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            if !spent_cast_arguments {
                spent_cast_arguments = true;
                {
                    let expected = list.expected_parameters(method);
                    self.cast_arguments(&mut list.arguments, &expected)?;
                }
                continue;
            }
            // No spelling resolves; the most explicit one is written anyway.
            return Ok(());
        }
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
        event_op: Option<u8>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let declaring_def = self.ts.definition(declaring);
        let require = self.requires_qualifier_dispatching(
            &name,
            method.is_static,
            declaring_def,
            target.expression,
            op != CallOp::CallVirt && method.is_overridable,
        );
        let (require, found_by_lookup) =
            self.disambiguate_accessor(operand, op, &target, require, declaring)?;
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
                found_by_lookup,
            },
        );
        let Some(value) = value else {
            self.ast.with_il(reference, inst);
            return Ok(TranslatedExpression {
                expression: reference,
                ty,
            });
        };
        // An event's accessor is `+=` or `-=`.
        let e = match event_op {
            Some(op) => self.ast.add(AstNode::CompoundAssignment {
                op,
                left: reference,
                right: value.expression,
                overflow: super::ast::Overflow::None,
            }),
            None => self.ast.add(AstNode::Assignment {
                left: reference,
                right: value.expression,
            }),
        };
        self.ast.with_rr(e, Rr::Type(ty));
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `HandleAccessorCall` for an indexer: `target[indices]`, or `target[indices] = value` for
    /// its setter, written once the indexer the indices resolve to from the target is the
    /// accessor's (`Disambiguator.ForAccessor`). Arguments or a target that would need casts are
    /// not translated.
    #[allow(clippy::too_many_arguments)]
    fn indexer_access(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
        id: crate::typesystem::members::MethodDefId,
        method: &Method,
        target: TranslatedExpression,
        mut list: ArgumentList,
        is_setter: bool,
        declaring: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        // Readability names and `out var` are not written for an access.
        list.add_names_to_primitive_values = false;
        list.use_implicitly_typed_out = false;
        // The assigned value is not part of the member reference.
        let value = if is_setter {
            list.parameter_names.pop();
            list.is_primitive_value.pop();
            list.ref_kinds.pop();
            Some(
                list.arguments
                    .pop()
                    .ok_or("an indexer setter without value")?,
            )
        } else {
            None
        };
        // An indexer access spells out at least one index.
        if list.first_optional == 0 && !list.arguments.is_empty() {
            list.first_optional = 1;
        }
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
        let group = resolver
            .indexers(self.ts, lookup_target)
            .ok_or("indexers the host cannot name by metadata row")?;
        // `AccessorSteps`: the indexer is written with its target already.
        let mut spent_optional = false;
        let mut spent_cast_arguments = list.arguments.is_empty();
        let mut spent_cast_target = false;
        loop {
            let errors = self.probe_indexers(&group, target.ty, id, &list)?;
            if errors == OverloadResolutionErrors::NONE {
                break;
            }
            // `TryRepair`, then `Escalate`
            if !spent_optional && list.first_optional >= 0 {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            if !spent_cast_arguments {
                spent_cast_arguments = true;
                {
                    let expected = list.expected_parameters(method);
                    self.cast_arguments(&mut list.arguments, &expected)?;
                }
                continue;
            }
            if !spent_cast_target {
                spent_cast_target = true;
                if self.convert_to(target, declaring, false)?.expression != target.expression {
                    return Err("a reference that needs a cast of its target".to_owned());
                }
                continue;
            }
            // No spelling resolves; the most explicit one is written anyway.
            break;
        }
        let indices = self.written_arguments(&list, 0);
        let ty = match value {
            Some(_) => method
                .parameters
                .last()
                .map(|p| p.ty)
                .ok_or("a setter without parameters")?,
            None => method.return_type,
        };
        let e = self.ast.add(AstNode::Indexer {
            target: target.expression,
            indices,
        });
        let target_rr = self.rr_of(target.expression);
        self.ast.with_rr(
            e,
            Rr::Property {
                target: Box::new(target_rr),
                accessor: Box::new(operand.clone()),
                ty,
                found_by_lookup: false,
            },
        );
        let Some(value) = value else {
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression { expression: e, ty });
        };
        let a = self.ast.add(AstNode::Assignment {
            left: e,
            right: value.expression,
        });
        self.ast.with_rr(a, Rr::Type(ty));
        self.ast.with_il(a, inst);
        Ok(TranslatedExpression { expression: a, ty })
    }

    /// `IsUnambiguousAccess` for an indexer: overload resolution of the indices over the
    /// target's indexers (stand-ins of their getters, or of their setters without the value),
    /// with `CheckBestCandidate`, the best one compared with the accessor by its property.
    fn probe_indexers(
        &mut self,
        group: &[MethodGroupList],
        lookup_type: TypeId,
        accessor: crate::typesystem::members::MethodDefId,
        list: &ArgumentList,
    ) -> Result<OverloadResolutionErrors, Unsupported> {
        let mut lists = self.method_lists(group, Some(lookup_type));
        for l in &mut lists {
            for m in &mut l.methods {
                if let crate::semantics::MethodKey::Def(d) = m.key
                    && self
                        .ts
                        .accessor_owner(d)
                        .is_some_and(|(_, s)| s == ACCESSOR_SETTER)
                {
                    m.parameters.pop();
                    m.definition_parameters.pop();
                }
            }
        }
        let arguments = self.argument_resolve_results(list);
        let names = list.argument_names();
        let mut conversions = Conversions::new(self.ts);
        let mut or = OverloadResolution::new(arguments, names, Vec::new());
        or.add_method_lists(&mut conversions, &lists);
        let errors = or.best_candidate_errors(&mut conversions);
        if errors != OverloadResolutionErrors::NONE {
            return Ok(errors);
        }
        if or.is_ambiguous() {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        }
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        };
        // `IsAppropriateCallTarget` of the properties; an override of it is not translated.
        let crate::semantics::MethodKey::Def(found) = found.key else {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        };
        let found_property = self
            .ts
            .accessor_owner(found)
            .map(|(token, _)| (found.module, token));
        let property = self
            .ts
            .accessor_owner(accessor)
            .map(|(token, _)| (accessor.module, token));
        Ok(if found_property == property {
            OverloadResolutionErrors::NONE
        } else {
            OverloadResolutionErrors::AMBIGUOUS_MATCH
        })
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
    ) -> Result<(bool, bool), Unsupported> {
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
                AccessorBinding::Owner => return Ok((require, false)),
                // The resolve result names the member found, which the host looks up again.
                AccessorBinding::Other => return Ok((require, true)),
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
                // A cast of the target is not translated.
                if self.convert_to(*target, declaring, false)?.expression != target.expression {
                    return Err("a reference that needs a cast of its target".to_owned());
                }
                continue;
            }
            // No spelling resolves; the most explicit one is emitted anyway.
            return Ok((require, false));
        }
    }

    /// The forms of `CallBuilder.Build` that are no plain invocation.
    fn check_special_calls(
        &mut self,
        method: &Method,
        declaring: TypeId,
    ) -> Result<(), Unsupported> {
        let ts = &*self.ts;
        let in_namespace = |ns: &str, name: &str| {
            ts.definition(declaring)
                .and_then(|d| ts.comp.type_def(d).ok())
                .is_some_and(|i| i.full_name.top.ns == ns && i.full_name.name() == name)
        };
        // `IsSpanBasedStringConcat`: a `string.Concat` of `ReadOnlySpan<char>` parameters only;
        // any other `Concat` is an ordinary call.
        let span_concat = method.name == "Concat"
            && method.is_static
            && method.parameters.iter().all(|p| {
                ts.is_known(p.ty, KnownTypeCode::ReadOnlySpanOfT)
                    && ts
                        .first_type_argument(p.ty)
                        .is_some_and(|a| ts.is_known(a, KnownTypeCode::Char))
            });
        let special = (ts.is_known(declaring, KnownTypeCode::String) && span_concat)
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
        target: &mut TranslatedExpression,
        list: &mut ArgumentList,
        require: bool,
        declaring: TypeId,
    ) -> Result<(bool, bool), Unsupported> {
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let mut lookup_target = match self.rr_of(target.expression) {
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
        let mut spent_optional = false;
        let mut spent_cast_target = false;
        let mut spent_type_arguments = false;
        let mut spent_explicit_in = false;
        // `RequireTypeArguments`, which a non-generic method takes too, with none to write.
        let mut type_arguments = false;
        // `WriteTypeArgumentsInferenceCannotReach`: the type arguments are written up front
        // where inference from the arguments cannot succeed; casting the arguments takes the
        // guess back.
        let mut type_arguments_up_front = false;
        let extension_call_on_null = self.ts.method_flags(id, None).extension
            && list
                .arguments
                .first()
                .is_some_and(|a| matches!(self.ast.node(a.expression), AstNode::Null));
        if !method.type_parameters.is_empty()
            && !extension_call_on_null
            && !self.can_infer_type_arguments(method, list)
        {
            if method
                .method_args
                .iter()
                .flatten()
                .any(|&a| self.may_contain_anonymous_type(a))
            {
                return Err("a generic call of anonymous types inference does not reach".to_owned());
            }
            spent_type_arguments = true;
            type_arguments = true;
            type_arguments_up_front = true;
        }
        loop {
            let probe_target = require.then_some(lookup_target);
            let written = if type_arguments {
                method.method_args.clone().unwrap_or_default()
            } else {
                Vec::new()
            };
            let errors =
                self.probe_call(resolver, method, op, probe_target, target, list, &written)?;
            if errors == OverloadResolutionErrors::NONE {
                return Ok((require, type_arguments));
            }
            if errors == OverloadResolutionErrors::OUT_VAR_TYPE_MISMATCH
                && list.use_implicitly_typed_out
            {
                list.use_implicitly_typed_out = false;
                continue;
            }
            // `TryRepair`
            if (errors == OverloadResolutionErrors::TYPE_INFERENCE_FAILED
                || errors == OverloadResolutionErrors::WRONG_NUMBER_OF_TYPE_ARGUMENTS)
                && !spent_type_arguments
            {
                spent_type_arguments = true;
                type_arguments = true;
                continue;
            }
            if errors == OverloadResolutionErrors::MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER
                && !spent_optional
                && list.first_optional >= 0
            {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            // `Escalate`
            if !spent_names && list.add_names_to_primitive_values {
                spent_names = true;
                list.add_names_to_primitive_values = false;
                continue;
            }
            // `NoOptionalArgumentAllowed`, also the repair for a missing argument.
            if !spent_optional && list.first_optional >= 0 {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            if !spent_cast_arguments {
                spent_cast_arguments = true;
                if type_arguments_up_front {
                    // The guess did not help: a later step may still reach for it.
                    type_arguments_up_front = false;
                    type_arguments = false;
                    spent_type_arguments = false;
                }
                // Cast arguments are written with their types, not as `out var`.
                list.use_implicitly_typed_out = false;
                {
                    let expected = list.expected_parameters(method);
                    self.cast_arguments(&mut list.arguments, &expected)?;
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
                    // `Target.ConvertTo(declaringType)`: the target cast to the declaring type. A
                    // value-type target, which the managed probe resolves without the cast where
                    // the engine's does not, is not translated.
                    // A `this` or `base` target, which the managed probe resolves without the
                    // cast (a constructor initializer), is not translated either; nor is a
                    // null-conditional target, which would be cast inside its `?`.
                    if self.ts.is_reference_type(target.ty) != Some(true)
                        || matches!(
                            self.ast.node(target.expression),
                            AstNode::This | AstNode::Base | AstNode::NullConditional { .. }
                        )
                    {
                        return Err("a reference that needs a cast of its target".to_owned());
                    }
                    *target = self.convert_to(*target, declaring, false)?;
                    lookup_target = LookupTarget::Expression(target.ty);
                }
                continue;
            }
            if !spent_type_arguments {
                spent_type_arguments = true;
                type_arguments = true;
                continue;
            }
            if !spent_explicit_in {
                // No `in` parameter to make explicit.
                spent_explicit_in = true;
                continue;
            }
            // No spelling resolves; the most explicit one is written anyway.
            return Ok((require, type_arguments));
        }
    }

    /// `ContainsAnonymousType` by the name check of `IsAnonymousType` alone (a type named like
    /// an anonymous type counts even where it is no compiler-generated one), through the
    /// annotations, modifiers and element types the managed visitor visits.
    fn contains_anonymous_type_name(&self, t: TypeId) -> bool {
        match self.ts.data(t) {
            TypeData::Def(def) => self.ts.comp.type_def(*def).is_ok_and(|i| {
                i.full_name.top.ns.is_empty()
                    && (i.full_name.name().contains("AnonType")
                        || i.full_name.name().contains("AnonymousType"))
            }),
            TypeData::Inst { generic, args } => {
                self.contains_anonymous_type_name(*generic)
                    || args.iter().any(|&a| self.contains_anonymous_type_name(a))
            }
            TypeData::Array { elem, .. } => self.contains_anonymous_type_name(*elem),
            TypeData::Pointer(e) | TypeData::ByRef(e) | TypeData::Pinned(e) => {
                self.contains_anonymous_type_name(*e)
            }
            TypeData::Annotated { inner, .. } | TypeData::Modified { inner, .. } => {
                self.contains_anonymous_type_name(*inner)
            }
            TypeData::Tuple { underlying, .. } => self.contains_anonymous_type_name(*underlying),
            TypeData::FnPtr(_) | TypeData::Intersection(_) => true,
            _ => false,
        }
    }

    /// Whether `e` is a cast other than a lambda's, which a call's target is not written as.
    fn is_cast_target(&self, e: AstId) -> bool {
        matches!(self.ast.node(e), AstNode::Cast { .. })
            && !matches!(self.rr_of(e), Rr::LambdaConversion { .. })
    }

    /// `Disambiguator.CastArguments`: every argument converted to its parameter type with an
    /// explicit cast where it is of another type. Parameters of anonymous or dynamic types
    /// and `in` parameters, which the managed code treats apart, are not translated.
    fn cast_arguments(
        &mut self,
        arguments: &mut [TranslatedExpression],
        parameters: &[crate::semantics::Parameter],
    ) -> Result<(), Unsupported> {
        for (a, p) in arguments.iter_mut().zip(parameters) {
            if self.contains_anonymous_type_name(p.ty)
                || self.ts.kind(p.ty) == TypeKind::Dynamic
                || p.ref_kind == RefKind::In
            {
                return Err("argument casts to anonymous, dynamic or in parameters".to_owned());
            }
            *a = self.convert_to(*a, p.ty, false)?;
        }
        Ok(())
    }

    /// `CanInferTypeArgumentsFromArguments`: whether type inference from the arguments
    /// succeeds for the method definition, with the expression builder's improved algorithm.
    fn can_infer_type_arguments(&mut self, method: &Method, list: &ArgumentList) -> bool {
        let arguments: Vec<_> = list
            .arguments
            .iter()
            .map(|a| self.semantic_rr(a.expression, a.ty))
            .collect();
        let parameters: Vec<TypeId> = match &list.parameter_map {
            Some(map) => map
                .iter()
                .map(|&p| method.definition_parameters[p].ty)
                .collect(),
            None => method.definition_parameters.iter().map(|p| p.ty).collect(),
        };
        let mut conversions = Conversions::new(self.ts);
        let mut ti = TypeInference::new(&mut conversions);
        ti.algorithm = TypeInferenceAlgorithm::Improved;
        ti.infer_type_arguments(&method.type_parameters, &arguments, &parameters, None)
            .1
    }

    /// Whether `t` may contain an anonymous type, which `WriteTypeArgumentsInferenceCannotReach`
    /// treats apart: a type definition named like one (as `IsAnonymousType` checks the name),
    /// or any type the check does not look into.
    pub(crate) fn may_contain_anonymous_type(&self, t: TypeId) -> bool {
        match self.ts.data(t) {
            TypeData::Def(def) => self.ts.comp.type_def(*def).map_or(true, |i| {
                i.full_name.top.ns.is_empty()
                    && (i.full_name.name().contains("AnonType")
                        || i.full_name.name().contains("AnonymousType"))
            }),
            TypeData::Inst { generic, args } => {
                self.may_contain_anonymous_type(*generic)
                    || args.iter().any(|&a| self.may_contain_anonymous_type(a))
            }
            TypeData::Array { elem, .. } => self.may_contain_anonymous_type(*elem),
            TypeData::Pointer(e) | TypeData::ByRef(e) => self.may_contain_anonymous_type(*e),
            TypeData::TypeParam { .. } => false,
            _ => true,
        }
    }

    /// `VisitAwait`: `await value`, of the type `GetResult` returns. The operand is cast to the
    /// type declaring `GetAwaiter` where `Disambiguator.CheckSimpleCall` does not find it from
    /// the operand. A dynamic await, an extension `GetAwaiter`, a constrained operand and an
    /// operand that already is a cast (a box the managed builder looks through) are not
    /// translated.
    pub(crate) fn translate_await(
        &mut self,
        inst: NodeId,
        value: NodeId,
        get_awaiter: Option<crate::il::ast::AwaiterMethod>,
        get_result: Option<crate::il::ast::AwaiterMethod>,
        get_awaiter_callvirt: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        use crate::il::ast::AwaiterMethod as Am;
        let (Some(Am::Method(awaiter)), Some(Am::Method(result))) = (&get_awaiter, &get_result)
        else {
            return Err("a dynamic await".to_owned());
        };
        if matches!(self.f.inst(value), Inst::LdObjIfRef { .. }) {
            return Err("an await of a constrained operand".to_owned());
        }
        let awaiter = Method::from_resolved(self.ts, &awaiter.method)
            .ok_or("an await of an unresolved method")?;
        let result = Method::from_resolved(self.ts, &result.method)
            .ok_or("an await of an unresolved method")?;
        if awaiter.is_static {
            return Err("an await of an extension GetAwaiter".to_owned());
        }
        let expected = awaiter.declaring.ok_or("a method without declaring type")?;
        let mut operand = self.translate(value, Some(expected))?;
        if let AstNode::RefDirection { expression } = *self.ast.node(operand.expression) {
            operand = self.unwrap_child(operand.expression, expression);
        }
        if matches!(self.ast.node(operand.expression), AstNode::Cast { .. }) {
            return Err("an await of a cast operand".to_owned());
        }
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let lookup_target = match self.rr_of(operand.expression) {
            Rr::This {
                ty,
                causes_non_virtual_invocation,
                ..
            } => LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            },
            Rr::Type(ty) => LookupTarget::Type(ty),
            _ => LookupTarget::Expression(operand.ty),
        };
        let list = ArgumentList {
            arguments: Vec::new(),
            parameter_names: Vec::new(),
            is_primitive_value: Vec::new(),
            add_names_to_primitive_values: false,
            first_optional: -2,
            ref_kinds: Vec::new(),
            use_implicitly_typed_out: false,
            parameter_map: None,
            explicit_names: None,
        };
        let op = if get_awaiter_callvirt {
            CallOp::CallVirt
        } else {
            CallOp::Call
        };
        let errors = self.probe_call(
            resolver,
            &awaiter,
            op,
            Some(lookup_target),
            &operand,
            &list,
            &[],
        )?;
        self.found_override = None;
        if errors != OverloadResolutionErrors::NONE {
            operand = self.convert_to(operand, expected, false)?;
        }
        let e = self.ast.add(AstNode::Await {
            expression: operand.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(result.return_type));
        Ok(TranslatedExpression {
            expression: e,
            ty: result.return_type,
        })
    }

    /// `CallWithNamedArgs`: the call with the arguments the block stores in temporaries first,
    /// in their order, then the others, each with the index of its parameter.
    pub(crate) fn call_with_named_args(
        &mut self,
        block: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let (call, op) = match self.f.inst(final_inst) {
            Inst::Call(c) => (c.clone(), CallOp::Call),
            Inst::CallVirt(c) => (c.clone(), CallOp::CallVirt),
            _ => return Err("named arguments of an object creation".to_owned()),
        };
        let first_param = usize::from(!call.method.method.is_static);
        let is_named = |b: &Self, n: NodeId| match *b.f.inst(n) {
            Inst::LdLoc { var } => b.f.var(var).kind == crate::il::ast::VariableKind::NamedArgument,
            _ => false,
        };
        let mut args: Vec<NodeId> = call.args[..first_param].to_vec();
        let mut map = Vec::with_capacity(call.args.len() - first_param);
        for (k, &store) in instructions.iter().enumerate() {
            let Inst::StLoc { var, value, .. } = *self.f.inst(store) else {
                return Err("a named argument block of another shape".to_owned());
            };
            // The target stored first (its parameter index -1 in the managed map).
            if first_param == 1
                && matches!(*self.f.inst(call.args[0]), Inst::LdLoc { var: v } if v == var)
            {
                if k != 0 {
                    return Err("a named argument target stored after an argument".to_owned());
                }
                args[0] = value;
                continue;
            }
            let position = call.args[first_param..]
                .iter()
                .position(|&a| matches!(*self.f.inst(a), Inst::LdLoc { var: v } if v == var))
                .ok_or("a named argument the call does not load")?;
            args.push(value);
            map.push(position);
        }
        for (position, &a) in call.args[first_param..].iter().enumerate() {
            if !is_named(self, a) {
                args.push(a);
                map.push(position);
            }
        }
        let call = crate::il::ast::CallData { args, ..call };
        self.named_argument_map = Some(map);
        let result = self.call(final_inst, &call, op);
        self.named_argument_map = None;
        let result = result?;
        self.ast.with_il(result.expression, block);
        Ok(result)
    }

    /// `HandleStringInterpolation` for `string.Format`: the interpolated string the format
    /// string's tokens make with the arguments, or `None` where `TryGetStringInterpolationTokens`
    /// fails (the call is then written as it is).
    fn format_interpolation(
        &mut self,
        inst: NodeId,
        list: &ArgumentList,
        first_argument_node: usize,
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        let Some(format_arg) = list.arguments.first() else {
            return Ok(None);
        };
        let format = match self.rr_of(format_arg.expression) {
            Rr::Constant {
                ty,
                value: ConstValue::String(s),
            } if self.ts.is_known(ty, KnownTypeCode::String) => s,
            _ => return Ok(None),
        };
        // No other argument may contain a string literal.
        let format_node = format_arg.expression.0 as usize;
        let has_string_literal = self.ast.nodes[first_argument_node..]
            .iter()
            .enumerate()
            .any(|(i, (n, _))| {
                first_argument_node + i != format_node
                    && matches!(
                        n,
                        AstNode::StringLiteral { .. }
                            | AstNode::ConstantValue {
                                value: ConstValue::String(_),
                                ..
                            }
                    )
            });
        if has_string_literal {
            return Ok(None);
        }
        let Some(tokens) = format_tokens(&format) else {
            return Ok(None);
        };
        if tokens.is_empty()
            || tokens.iter().filter(|t| t.index.is_some()).count() + 1 != list.arguments.len()
        {
            return Ok(None);
        }
        let contents = tokens
            .into_iter()
            .map(|t| match t.index {
                None => super::ast::InterpolatedContent::Text(t.text),
                Some(index) => super::ast::InterpolatedContent::Interpolation {
                    expression: list.arguments[index + 1].expression,
                    alignment: t.alignment,
                    suffix: t.format,
                },
            })
            .collect();
        let string = self.ts.known_type(KnownTypeCode::String);
        let e = self.ast.add(AstNode::InterpolatedString { contents });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::FormatString { ty: string, format });
        Ok(Some(TranslatedExpression {
            expression: e,
            ty: string,
        }))
    }

    /// `Disambiguator.ProbeCall`: the overload resolution of the current spelling, over the
    /// method group the host looks the name up to.
    #[allow(clippy::too_many_arguments)]
    fn probe_call(
        &mut self,
        resolver: &dyn crate::il::transforms::Resolver,
        method: &Method,
        op: CallOp,
        lookup_target: Option<LookupTarget>,
        target: &TranslatedExpression,
        list: &ArgumentList,
        type_arguments: &[TypeId],
    ) -> Result<OverloadResolutionErrors, Unsupported> {
        if method.is_operator {
            return self.probe_group(&[], None, method, op, list, type_arguments);
        }
        let Some(group) =
            resolver.method_group(self.ts, &method.name, lookup_target, type_arguments)
        else {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        };
        let lookup_type = lookup_target.map(|_| target.ty);
        self.probe_group(&group, lookup_type, method, op, list, type_arguments)
    }

    /// The overload resolution of `IsUnambiguousCall` over a method group, with
    /// `CheckBestCandidate`.
    fn probe_group(
        &mut self,
        group: &[MethodGroupList],
        lookup_type: Option<TypeId>,
        method: &Method,
        op: CallOp,
        list: &ArgumentList,
        type_arguments: &[TypeId],
    ) -> Result<OverloadResolutionErrors, Unsupported> {
        // An operator's candidates are the operators of its operands' types.
        let operators = if method.is_operator {
            Some(self.operator_candidates(method, list)?)
        } else {
            None
        };
        let lists = if operators.is_some() {
            Vec::new()
        } else {
            self.method_lists(group, lookup_type)
        };
        let arguments = self.argument_resolve_results(list);
        let out_vars: Vec<Option<TypeId>> = arguments
            .iter()
            .map(|a| match a {
                ResolveResult::OutVar(t) => Some(*t),
                _ => None,
            })
            .collect();
        let names = list.argument_names();
        let mut conversions = Conversions::new(self.ts);
        let mut or = OverloadResolution::new(arguments, names, type_arguments.to_vec());
        match operators {
            Some(candidates) => {
                for m in candidates {
                    or.add_candidate(&mut conversions, m);
                }
            }
            None => or.add_method_lists(&mut conversions, &lists),
        }
        // `CheckBestCandidate`
        let errors = or.best_candidate_errors(&mut conversions);
        if errors != OverloadResolutionErrors::NONE {
            return Ok(errors);
        }
        if or.is_ambiguous() {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        }
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        };
        if !self.is_appropriate_call_target(op, method, &found)? {
            return Ok(OverloadResolutionErrors::AMBIGUOUS_MATCH);
        }
        if !self.same_method_after_type_erasure(method, &found) {
            self.found_override = Some(found.clone());
        }
        // An `out var` must declare the type of the parameter found.
        if list.parameter_map.is_some() && out_vars.iter().any(Option::is_some) {
            return Err("an out var among named arguments".to_owned());
        }
        for (i, out_var) in out_vars.iter().enumerate() {
            if let Some(element) = *out_var
                && let Some(p) = found.parameters.get(i)
            {
                let parameter = match *self.ts.data(p.ty) {
                    TypeData::ByRef(e) => e,
                    _ => p.ty,
                };
                if parameter != element {
                    return Ok(OverloadResolutionErrors::OUT_VAR_TYPE_MISMATCH);
                }
            }
        }
        // The normal form was meant. The omitted arguments are the defaults of the method
        // found, which is the one called (`OmittedArgumentsAreDefaultsOf`); writing them out
        // again corrects a resolution that reached the expanded form.
        if or.best_candidate_is_expanded_form() {
            return Ok(if list.first_optional >= 0 {
                OverloadResolutionErrors::MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER
            } else {
                OverloadResolutionErrors::AMBIGUOUS_MATCH
            });
        }
        // The resolve result names the method found, whose inferred type arguments may differ
        // from the call's in tuple names or nullability; the host finds it again.
        if found.method_args != method.method_args {
            self.found_override = Some(found.clone());
        }
        Ok(OverloadResolutionErrors::NONE)
    }

    /// `GetUserDefinedOperatorCandidates` for the operands `IsUnambiguousCall` resolves an
    /// operator call with (and the target type of an explicit conversion): the operators of
    /// that name the types and their base types declare, the primitive types excepted. Lifted
    /// operators are only applicable to nullable operands, which are not translated.
    fn operator_candidates(
        &mut self,
        method: &Method,
        list: &ArgumentList,
    ) -> Result<Vec<Method>, Unsupported> {
        let written = &list.arguments[..list.actual_count()];
        if !matches!(written.len(), 1 | 2) {
            return Ok(Vec::new());
        }
        let mut types: Vec<TypeId> = written.iter().map(|a| a.ty).collect();
        if written.len() == 1 && method.name == "op_Explicit" {
            types.push(method.return_type);
        }
        if types
            .iter()
            .any(|&t| self.ts.is_known(t, KnownTypeCode::NullableOfT))
        {
            return Err("a lifted operator call".to_owned());
        }
        let mut candidates: Vec<Method> = Vec::new();
        for t in types {
            if (3..=15).contains(&self.ts.type_code(t)) {
                continue;
            }
            let mut groups = Vec::new();
            for d in self.ts.all_base_type_definitions(t) {
                let md = &self.ts.comp.module(d.module).md;
                let rows = md.type_methods(d.row).map_err(|e| e.to_string())?;
                let mut methods = Vec::new();
                for row in rows {
                    let m = md
                        .get::<crate::metadata::tables::rows::MethodDef>(row)
                        .map_err(|e| e.to_string())?;
                    if md.string(m.name).is_ok_and(|n| n == method.name) {
                        methods.push(crate::typesystem::members::MethodDefId {
                            module: d.module,
                            row,
                        });
                    }
                }
                if !methods.is_empty() {
                    groups.push(MethodGroupList {
                        declaring: d,
                        methods,
                    });
                }
            }
            for l in self.method_lists(&groups, Some(t)) {
                for m in l.methods {
                    if m.is_operator && !candidates.contains(&m) {
                        candidates.push(m);
                    }
                }
            }
        }
        Ok(candidates)
    }

    /// The host's method group as the overload resolution's method lists: each definition
    /// specialized by the base type of the lookup type (or of the current type and the types
    /// containing it, for a simple name) that declares it.
    pub(super) fn method_lists(
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
    pub(super) fn is_appropriate_call_target(
        &mut self,
        op: CallOp,
        expected: &Method,
        actual: &Method,
    ) -> Result<bool, Unsupported> {
        if self.same_method_after_type_erasure(expected, actual) {
            return Ok(true);
        }
        let MethodKey::Def(actual_id) = actual.key else {
            return Ok(false);
        };
        if op != CallOp::CallVirt
            || !self
                .ts
                .method_modifiers_of(actual_id)
                .is_ok_and(|m| m.is_override)
        {
            return Ok(false);
        }
        // `NeedsBoxingConversion` is not tracked: an override in a value type is not translated.
        if let Some(d) = actual.declaring
            && self.ts.is_reference_type(d) != Some(true)
        {
            return Err("a call whose name binds to a value type's override".to_owned());
        }
        // `InheritanceHelper.GetBaseMembers`: the method the override overrides, directly or
        // through further overrides.
        let MethodKey::Def(expected_id) = expected.key else {
            return Ok(false);
        };
        let bases = self
            .ts
            .base_members(
                crate::typesystem::inheritance::MemberDef::Method(actual_id),
                false,
            )
            .map_err(|e| e.to_string())?;
        for base in bases {
            let crate::typesystem::inheritance::MemberDef::Method(base_id) = base.def else {
                break;
            };
            if base_id == expected_id {
                return Ok(true);
            }
            if !self
                .ts
                .method_modifiers_of(base_id)
                .is_ok_and(|m| m.is_override)
            {
                break;
            }
        }
        Ok(false)
    }
}

/// A token of `TryGetStringInterpolationTokens`: text (`index` none, with its `{{` and `}}`
/// kept), or an argument with its alignment and format.
struct FormatToken {
    index: Option<usize>,
    text: Vec<u16>,
    alignment: i32,
    format: Option<Vec<u16>>,
}

/// `TokenizeFormatString` and `TryGetStringInterpolationTokens`: the tokens of a composite
/// format string whose arguments are numbered in order, `None` for any other string.
fn format_tokens(value: &[u16]) -> Option<Vec<FormatToken>> {
    #[derive(Clone, Copy, PartialEq)]
    enum Kind {
        String,
        Argument,
        WithFormat,
        WithAlignment,
        WithAlignmentAndFormat,
    }
    let (open, close, colon, comma) = (
        u16::from(b'{'),
        u16::from(b'}'),
        u16::from(b':'),
        u16::from(b','),
    );
    let mut raw: Vec<(Kind, Vec<u16>)> = Vec::new();
    let mut kind = Kind::String;
    let mut sb: Vec<u16> = Vec::new();
    let mut pos = 0;
    while pos < value.len() {
        let c = value[pos];
        pos += 1;
        let peek = value.get(pos).copied();
        if c == open {
            if peek == Some(open) {
                kind = Kind::String;
                sb.extend([open, open]);
                pos += 1;
            } else {
                if !sb.is_empty() {
                    raw.push((kind, std::mem::take(&mut sb)));
                }
                kind = Kind::Argument;
                sb.clear();
            }
        } else if c == close {
            if kind != Kind::String {
                raw.push((kind, std::mem::take(&mut sb)));
                kind = Kind::String;
            } else if peek == Some(close) {
                sb.extend([close, close]);
                pos += 1;
            } else {
                return None;
            }
        } else if c == colon {
            if kind == Kind::Argument {
                kind = Kind::WithFormat;
            } else if kind == Kind::WithAlignment {
                kind = Kind::WithAlignmentAndFormat;
            }
            sb.push(colon);
        } else if c == comma {
            if kind == Kind::Argument {
                kind = Kind::WithAlignment;
            }
            sb.push(comma);
        } else {
            sb.push(c);
        }
    }
    if !sb.is_empty() {
        if kind != Kind::String {
            return None;
        }
        raw.push((kind, sb));
    }
    // `int.TryParse`: an optional sign, digits, surrounding white space.
    let parse =
        |s: &[u16]| -> Option<i32> { String::from_utf16(s).ok()?.trim().parse::<i32>().ok() };
    let split = |s: &[u16], seps: &[u16], parts: usize| -> Vec<Vec<u16>> {
        let mut out = vec![Vec::new()];
        for &c in s {
            if out.len() < parts && seps.contains(&c) {
                out.push(Vec::new());
            } else {
                out.last_mut().expect("a part").push(c);
            }
        }
        out
    };
    let mut next = 0;
    let mut tokens = Vec::new();
    for (kind, data) in raw {
        let (index, alignment, format) = match kind {
            Kind::String => {
                tokens.push(FormatToken {
                    index: None,
                    text: data,
                    alignment: 0,
                    format: None,
                });
                continue;
            }
            Kind::Argument => (parse(&data)?, 0, None),
            Kind::WithFormat => {
                let parts = split(&data, &[colon], 2);
                if parts.len() != 2 || parts[1].is_empty() {
                    return None;
                }
                (parse(&parts[0])?, 0, Some(parts[1].clone()))
            }
            Kind::WithAlignment => {
                let parts = split(&data, &[comma], 2);
                if parts.len() != 2 || parts[1].is_empty() {
                    return None;
                }
                (parse(&parts[0])?, parse(&parts[1])?, None)
            }
            Kind::WithAlignmentAndFormat => {
                let parts = split(&data, &[comma, colon], 3);
                if parts.len() != 3 || parts[1].is_empty() || parts[2].is_empty() {
                    return None;
                }
                (parse(&parts[0])?, parse(&parts[1])?, Some(parts[2].clone()))
            }
        };
        if usize::try_from(index).ok()? != next {
            return None;
        }
        next += 1;
        tokens.push(FormatToken {
            index: Some(next - 1),
            text: Vec::new(),
            alignment,
            format,
        });
    }
    Some(tokens)
}

#[cfg(test)]
mod tests {
    use super::format_tokens;

    fn utf16(s: &str) -> Vec<u16> {
        s.encode_utf16().collect()
    }

    /// The tokens of `TryGetStringInterpolationTokens`, as (index, text, alignment, format).
    fn tokens(s: &str) -> Option<Vec<(Option<usize>, String, i32, Option<String>)>> {
        Some(
            format_tokens(&utf16(s))?
                .into_iter()
                .map(|t| {
                    (
                        t.index,
                        String::from_utf16(&t.text).unwrap(),
                        t.alignment,
                        t.format.map(|f| String::from_utf16(&f).unwrap()),
                    )
                })
                .collect(),
        )
    }

    #[test]
    fn format_strings_tokenize_as_the_managed_builder_does() {
        assert_eq!(
            tokens("a {0} b {1,-5:x2}{{c}}"),
            Some(vec![
                (None, "a ".to_owned(), 0, None),
                (Some(0), String::new(), 0, None),
                (None, " b ".to_owned(), 0, None),
                (Some(1), String::new(), -5, Some("x2".to_owned())),
                (None, "{{c}}".to_owned(), 0, None),
            ])
        );
        assert_eq!(
            tokens("{0:N}"),
            Some(vec![(Some(0), String::new(), 0, Some("N".to_owned()))])
        );
        // Arguments out of order, a lone brace, an unterminated argument, an empty format.
        assert_eq!(tokens("{1}{0}"), None);
        assert_eq!(tokens("a } b"), None);
        assert_eq!(tokens("{0"), None);
        assert_eq!(tokens("{0:}"), None);
        assert_eq!(tokens(""), Some(Vec::new()));
    }
}
