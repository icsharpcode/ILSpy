//! `CallBuilder`: method calls, and the call case of `Disambiguator`.
//!
//! The calls translated are those of ordinary non-generic methods with by-value arguments, no
//! `params` or optional parameters, written as `target.Name(arguments)` or `Name(arguments)`.
//! The special forms of `CallBuilder.Build` (delegate construction and invocation, tuples,
//! string concatenation and interpolation, ranges, accessors, operators, extension methods,
//! span helpers) fall back.

use crate::il::ast::{CallData, Inst, MethodOperand, NodeId};
use crate::il::transforms::{AccessorBinding, LookupTarget, MethodGroupList, MethodGroupOperand};
use crate::semantics::overload_resolution::OverloadResolutionErrors;
use crate::semantics::types::TypeKind;
use crate::semantics::{
    Conversions, Method, MethodKey, MethodList, OverloadResolution, ResolveResult, TypeInference,
    TypeInferenceAlgorithm,
};
use crate::typesystem::attrs::ACC_PROTECTED;
use crate::typesystem::compilation::{ModuleId, TypeDefId};
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{ConstValue, RefKind};
use crate::typesystem::options;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Overflow, Rr};
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
pub(crate) struct ArgumentList {
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
    pub(crate) parameter_map: Option<Vec<usize>>,
    /// `ArgumentNames`: the names written from the first argument out of its place on.
    explicit_names: Option<Vec<Option<String>>>,
    /// `IsExpandedForm`: the elements of a `params` array are the last arguments.
    is_expanded_form: bool,
    /// `ExpectedParameters` of the expanded form: the parameters with the `params` one
    /// replaced by one of its element type per element.
    expanded_parameters: Option<Vec<crate::semantics::Parameter>>,
    /// The elements `TransformParamsArgument` would expand the `params` argument into, and
    /// their type; `call` probes the expanded form before it takes it.
    pub(crate) params_candidate: Option<(Vec<TranslatedExpression>, TypeId)>,
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
            && !self.is_expanded_form
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
        if let Some(expanded) = &self.expanded_parameters {
            return expanded.clone();
        }
        match &self.parameter_map {
            Some(map) => map.iter().map(|&p| method.parameters[p].clone()).collect(),
            None => method.parameters.clone(),
        }
    }
}

impl ArgumentList {
    /// The list with the `params` argument replaced by the elements of its candidate, in the
    /// expanded form (`TransformParamsArgument`).
    fn expanded(&self, method: &Method) -> Option<ArgumentList> {
        let (elements, element_type) = self.params_candidate.clone()?;
        let keep = self.arguments.len() - 1;
        let n = elements.len();
        let mut parameters = method.parameters[..keep].to_vec();
        parameters.extend((0..n).map(|_| crate::semantics::Parameter::of_type(element_type)));
        let mut arguments = self.arguments[..keep].to_vec();
        arguments.extend(elements);
        let extend = |v: &[String]| {
            let mut v = v[..keep].to_vec();
            v.extend((0..n).map(|_| String::new()));
            v
        };
        Some(ArgumentList {
            arguments,
            parameter_names: extend(&self.parameter_names),
            is_primitive_value: {
                let mut v = self.is_primitive_value[..keep].to_vec();
                v.extend((0..n).map(|_| false));
                v
            },
            add_names_to_primitive_values: self.add_names_to_primitive_values,
            first_optional: -1,
            ref_kinds: {
                let mut v = self.ref_kinds[..keep].to_vec();
                v.extend((0..n).map(|_| RefKind::None));
                v
            },
            use_implicitly_typed_out: self.use_implicitly_typed_out,
            parameter_map: None,
            explicit_names: None,
            is_expanded_form: true,
            expanded_parameters: Some(parameters),
            params_candidate: None,
        })
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
        if self.settings.string_concat
            && let Some(operands) = self.span_based_string_concat(call)
        {
            return self.build_string_concat(inst, call, &operands);
        }
        let result = self.call_without_tail(inst, call, op)?;
        // The IL `tail.` prefix is written as an inline marker: `/*tail.*/Callee(x)`.
        if call.is_tail {
            self.ast.with_leading_comment(result.expression, "tail.", 1);
        }
        Ok(result)
    }

    fn call_without_tail(
        &mut self,
        inst: NodeId,
        call: &CallData,
        op: CallOp,
    ) -> Result<TranslatedExpression, Unsupported> {
        let operand = &call.method;
        if operand.local_function.is_some()
            && operand.specialized_by.is_none()
            && operand.range_accessor.is_none()
            && !operand.lifted
        {
            return self.local_function_call(inst, call);
        }
        // A proxy call `ProxyCallReplacer` replaced calls the specialized method as any other.
        // A slicing range accessor is written by `HandleRangeConstruction`.
        let slicing = operand.range_accessor.as_ref().is_some_and(|r| r.slicing);
        if operand.local_function.is_some() || (operand.range_accessor.is_some() && !slicing) {
            return Err(
                "a call of a local function, proxy, range accessor or lifted operator".to_owned(),
            );
        }
        let resolved = &operand.method;
        let MethodOrigin::Def(id) = resolved.origin else {
            return Err("a call of an unresolved method".to_owned());
        };
        let mut method =
            Method::from_resolved(self.ts, resolved).ok_or("a call of an unresolved method")?;
        method.lifted = operand.lifted;
        if slicing {
            // `SyntheticRangeIndexAccessor.Parameters`: one unnamed parameter of type `Range`.
            method.parameters.truncate(1);
            if let Some(p) = method.parameters.first_mut() {
                *p = crate::semantics::Parameter::of_type(p.ty);
            }
        }
        let explicit_implementation = self
            .ts
            .is_explicit_interface_implementation(id)
            .map_err(|e| e.to_string())?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // A direct non-virtual call of an explicit interface implementation cannot be written
        // in C#; in a sealed class it is equivalent to a virtual call of the interface member.
        if explicit_implementation
            && op == CallOp::Call
            && operand.specialized_by.is_none()
            && self
                .ts
                .definition(declaring)
                .and_then(|d| self.ts.comp.type_def(d).ok())
                .is_some_and(|i| {
                    i.kind == crate::typesystem::compilation::TypeKind::Class
                        && i.flags & 0x100 != 0
                })
        {
            let implemented = self
                .ts
                .explicit_implementations(id)
                .map_err(|e| e.to_string())?;
            if let [interface_member] = &implemented[..] {
                let call = CallData {
                    // Found by lookup: the wire sends the method's signature.
                    method: MethodOperand {
                        token: 0,
                        method: interface_member.clone(),
                        context: operand.context,
                        lifted: false,
                        range_accessor: None,
                        specialized_by: None,
                        local_function: None,
                    },
                    ..call.clone()
                };
                return self.call(inst, &call, CallOp::CallVirt);
            }
        }
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
                // `IsWrittenAsMemberAccess`: a property with parameters is written as a member
                // access only as an indexer (`SymbolKind.Indexer`, named as the type's
                // `[DefaultMember]`); other parameterized properties keep the call syntax.
                let mut written_as_member_access = true;
                if method.parameters.len() != parameters {
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
                        written_as_member_access = false;
                    } else {
                        indexer = Some(semantics == ACCESSOR_SETTER);
                    }
                }
                // `HandleRangeConstruction` comes first and writes `..` for `Range.All`.
                if self.settings.ranges
                    && self.ts.is_known(declaring, KnownTypeCode::Range)
                    && method.name == "get_All"
                    && method.parameters.is_empty()
                    && op == CallOp::Call
                {
                    let e = self.ast.add(AstNode::Range {
                        start: None,
                        end: None,
                    });
                    self.ast.with_rr(
                        e,
                        Rr::PropertyMember {
                            accessor: Box::new(operand.clone()),
                            ty: method.return_type,
                        },
                    );
                    self.ast.with_il(e, inst);
                    return Ok(TranslatedExpression {
                        expression: e,
                        ty: method.return_type,
                    });
                }
                if written_as_member_access {
                    Some(self.property_info(id.module, token & 0x00FF_FFFF)?.0)
                } else {
                    None
                }
            }
        };
        let is_extension = self.ts.method_flags(id, None).extension;
        if explicit_implementation && (property.is_some() || is_extension || method.is_operator) {
            return Err("a call of an explicit interface implementation".to_owned());
        }
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
        // A boxing conversion of the target to `object` is made implicit, with the boxing kept
        // in mind for the overload found (`NeedsBoxingConversion`).
        let mut needs_boxing_conversion = false;
        if constrained_to.is_none() && !method.is_static {
            let unboxed = self.unwrap_boxing_conversion(target);
            if unboxed.expression != target.expression {
                target = unboxed;
                needs_boxing_conversion = true;
            }
        }

        // the arguments
        let is_setter =
            (property.is_some() && method.parameters.len() == 1) || indexer == Some(true);
        let first_argument_node = self.ast.nodes.len();
        let mut list = self.argument_list(call, &method, id, first_param, declaring, is_setter)?;
        if resolved.varargs.is_some() {
            method = self.vararg_argument_list(resolved, &mut list)?;
        }
        // `HandleRangeConstruction`: `a..` and `..b`.
        if self.settings.ranges
            && op == CallOp::Call
            && list.explicit_names.is_none()
            && self.ts.is_known(declaring, KnownTypeCode::Range)
            && matches!(method.name.as_str(), "StartAt" | "EndAt")
            && list.arguments.len() == 1
        {
            let side = Some(list.arguments[0].expression);
            let (start, end) = if method.name == "StartAt" {
                (side, None)
            } else {
                (None, side)
            };
            return Ok(self.range_expression(inst, operand, &method, start, end));
        }
        // `HandleRangeConstruction`: a slicing `Slice()` or `Substring()` is written as an
        // indexer.
        if self.settings.ranges && slicing && list.explicit_names.is_none() {
            let indices = list.arguments.iter().map(|a| a.expression).collect();
            let e = self.ast.add(AstNode::Indexer {
                target: target.expression,
                indices,
            });
            let target_rr = self.rr_of(target.expression);
            self.ast.with_rr(
                e,
                Rr::TargetMethodMember {
                    target: Box::new(target_rr),
                    method: Box::new(operand.clone()),
                    ty: method.return_type,
                },
            );
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: method.return_type,
            });
        }
        if slicing {
            return Err(
                "a call of a local function, proxy, range accessor or lifted operator".to_owned(),
            );
        }
        // `TransformParamsArgument`: the expanded form where the call as written with it resolves
        // to the method in that form.
        if let Some(expanded) = list.expanded(&method) {
            if property.is_some() || indexer.is_some() || method.is_operator {
                return Err("an expanded params argument of this call".to_owned());
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
            let errors = self.probe_call(
                resolver,
                &method,
                op,
                Some(lookup_target),
                &target,
                &expanded,
                &[],
            )?;
            self.found_override = None;
            if errors == OverloadResolutionErrors::NONE {
                list = expanded;
            }
            list.params_candidate = None;
        }
        // Named arguments of an extension method, a property or event accessor or a delegate
        // invocation are not translated; an indexer's are written as they are.
        if list.parameter_map.is_some()
            && (is_extension
                || (property.is_some() && indexer.is_none())
                || (method.name == "Invoke" && self.ts.kind(declaring) == TypeKind::Delegate))
        {
            return Err("named arguments of an extension, accessor or delegate call".to_owned());
        }

        // `IsInterpolatedStringCreation` for `string.Format`: an interpolated string where the
        // format string tokenizes, else an ordinary call. A `params` overload is one in the
        // expanded form; with the array literal itself it is not translated.
        // `FormattableStringFactory.Create` is the interpolated string cast to
        // `FormattableString`; where it does not tokenize, an ordinary call.
        let formattable = method.is_static
            && method.name == "Create"
            && self.ts.definition(declaring).is_some_and(|d| {
                self.ts.comp.type_def(d).is_ok_and(|i| {
                    i.full_name.top.ns == "System.Runtime.CompilerServices"
                        && i.full_name.name() == "FormattableStringFactory"
                })
            });
        // A `params` overload in the unexpanded form is an interpolated string only with the
        // array literal as its argument, which is not translated; else an ordinary call.
        let unexpanded_params =
            method.parameters.last().is_some_and(|p| p.is_params) && !list.is_expanded_form;
        let creation = self.settings.string_interpolation
            && method.is_static
            && (formattable
                || (method.name == "Format" && self.ts.is_known(declaring, KnownTypeCode::String)));
        let array_literal = unexpanded_params
            && list.arguments.len() == 2
            && matches!(
                self.ast.node(list.arguments[1].expression),
                AstNode::ArrayCreate { .. }
            );
        if creation && (!unexpanded_params || array_literal) {
            match self.format_interpolation(inst, &list, first_argument_node, array_literal)? {
                Some(e) if formattable => {
                    let ty = self.ts.known_type(KnownTypeCode::FormattableString);
                    let c = self.ast.add(AstNode::Cast {
                        ty,
                        expression: e.expression,
                        overflow: Overflow::None,
                    });
                    // The cast is the call's expression, the interpolated string inside it is not.
                    self.ast.annotations_mut(e.expression).il.clear();
                    self.ast.with_il(c, inst);
                    // `ImplicitInterpolatedStringConversion`, as `ResolveCast` gives it.
                    self.ast.with_rr(c, Rr::Conversion { ty });
                    return Ok(TranslatedExpression { expression: c, ty });
                }
                Some(e) => return Ok(e),
                None => {}
            }
        }

        // `InlineArrayAsSpan` and `InlineArrayAsReadOnlySpan` of the whole buffer are the inline
        // array conversion of the buffer.
        if self.settings.inline_arrays
            && self.is_inline_array_as_span(&method, declaring)
            && list.arguments.len() == 2
        {
            if list.parameter_map.is_some() || list.actual_count() != 2 {
                return Err("an inline array span with named or optional arguments".to_owned());
            }
            let array_length = method
                .method_args
                .as_ref()
                .and_then(|a| a.first().copied())
                .and_then(|t| self.inline_array_length(t));
            let span_length = match self.rr_of(list.arguments[1].expression) {
                Rr::Constant {
                    value: ConstValue::Int32(v),
                    ..
                } => Some(v),
                _ => None,
            };
            if let (Some(array_length), Some(span_length)) = (array_length, span_length)
                && span_length <= array_length
            {
                let mut argument = list.arguments[0];
                if let AstNode::RefDirection { expression } | AstNode::InDirection { expression } =
                    *self.ast.node(argument.expression)
                {
                    argument = self.unwrap_child(argument.expression, expression);
                }
                let ty = method.return_type;
                // The slice `buffer[..n]`, a `Span<T>` without instructions.
                let slice = span_length < array_length;
                if slice {
                    let array_type = method
                        .method_args
                        .as_ref()
                        .and_then(|a| a.first().copied())
                        .ok_or("an inline array span without type argument")?;
                    let element = self.ts.inline_array_element_type(array_type);
                    let span = self.ts.known_type(KnownTypeCode::SpanOfT);
                    let span = self.ts.intern(TypeData::Inst {
                        generic: span,
                        args: Box::new([element]),
                    });
                    let range = self.ast.add(AstNode::Range {
                        start: None,
                        end: Some(list.arguments[1].expression),
                    });
                    let e = self.ast.add(AstNode::Indexer {
                        target: argument.expression,
                        indices: vec![range],
                    });
                    self.ast.with_rr(e, Rr::Plain(span));
                    argument = TranslatedExpression {
                        expression: e,
                        ty: span,
                    };
                    if self.ts.is_known(ty, KnownTypeCode::SpanOfT) {
                        // `WithILInstruction(inst)` of `Build`'s result.
                        self.ast.with_il(e, inst);
                        return Ok(argument);
                    }
                }
                let c = self.ast.add(AstNode::Cast {
                    ty,
                    expression: argument.expression,
                    overflow: Overflow::None,
                });
                if slice {
                    self.ast.with_rr(c, Rr::InlineArrayConversion { ty });
                } else {
                    self.ast.with_rr(c, Rr::Conversion { ty });
                }
                self.ast.with_il(c, inst);
                return Ok(TranslatedExpression { expression: c, ty });
            }
        }
        // `bool?.GetValueOrDefault()` is `b == true` (with `LiftNullables`), the binary operator
        // carrying the call's resolve result; the literal has none.
        if self.settings.lift_nullables
            && method.name == "GetValueOrDefault"
            && self.ts.is_known(declaring, KnownTypeCode::NullableOfT)
            && self
                .ts
                .first_type_argument(declaring)
                .is_some_and(|a| self.ts.is_known(a, KnownTypeCode::Boolean))
            && list.arguments.is_empty()
            && !self.is_null_conditional(target.expression)
        {
            let boolean = self.ts.known_type(KnownTypeCode::Boolean);
            let literal = self.ast.add(AstNode::ConstantValue {
                ty: boolean,
                value: ConstValue::Boolean(true),
                allow_implicit_conversion: false,
            });
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: super::ast::Overflow::None,
                op: super::ast::BinaryOp::Equality,
                left: target.expression,
                right: literal,
            });
            let target_rr = self.rr_of(target.expression);
            self.ast.with_rr(
                e,
                Rr::Invocation {
                    target: Box::new(target_rr),
                    method: Box::new(operand.clone()),
                    ty: method.return_type,
                    extension: false,
                    delegate: false,
                    found_by_lookup: false,
                    expanded: list.is_expanded_form,
                    method_args: None,
                },
            );
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: method.return_type,
            });
        }
        // `IsDelegateEqualityComparison`: `Delegate.op_Equality` of two operands of one delegate
        // type is the predefined `==` (or `!=`), carrying the call's resolve result.
        if method.is_operator
            && matches!(method.name.as_str(), "op_Equality" | "op_Inequality")
            && self.ts.is_known(declaring, KnownTypeCode::Delegate)
            && list.arguments.len() == 2
            && self.ts.kind(list.arguments[0].ty) == TypeKind::Delegate
            && list.arguments[1].ty == list.arguments[0].ty
        {
            if list.parameter_map.is_some() || list.actual_count() != 2 {
                return Err("a delegate comparison with named or optional arguments".to_owned());
            }
            let e = self.ast.add(AstNode::BinaryOperator {
                overflow: super::ast::Overflow::None,
                op: if method.name == "op_Equality" {
                    super::ast::BinaryOp::Equality
                } else {
                    super::ast::BinaryOp::InEquality
                },
                left: list.arguments[0].expression,
                right: list.arguments[1].expression,
            });
            let target_rr = self.rr_of(target.expression);
            self.ast.with_rr(
                e,
                Rr::Invocation {
                    target: Box::new(target_rr),
                    method: Box::new(operand.clone()),
                    ty: method.return_type,
                    extension: false,
                    delegate: false,
                    found_by_lookup: false,
                    expanded: list.is_expanded_form,
                    method_args: None,
                },
            );
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: method.return_type,
            });
        }
        // `HandleImplicitConversion`: an implicit conversion operator is written as the conversion.
        if method.is_operator && method.name == "op_Implicit" && list.arguments.len() == 1 {
            return self.implicit_conversion_call(inst, &method, &list);
        }
        // The C# 14 compiler emits `AsSpan(string)` and `CastUp` for implicit span conversions,
        // which the call folds back into where the conversion applies to the argument's type.
        if self.settings.first_class_span_types
            && list.arguments.len() == 1
            && (self.is_string_to_read_only_span_char_as_span(&method, declaring)
                || self.is_read_only_span_cast_up(&method, declaring))
            && Conversions::new(self.ts)
                .implicit_type(list.arguments[0].ty, method.return_type)
                .is_implicit_span()
        {
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
                    expanded: false,
                    method_args: None,
                },
            );
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: method.return_type,
            });
        }
        if let Some(is_setter) = indexer {
            self.needs_boxing_conversion = needs_boxing_conversion;
            let access = self.indexer_access(
                inst, operand, id, &method, op, target, list, is_setter, declaring,
            );
            self.needs_boxing_conversion = false;
            return access;
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
        self.needs_boxing_conversion = needs_boxing_conversion;
        let disambiguated =
            self.disambiguate_call(&method, id, op, &mut target, &mut list, require, declaring);
        self.needs_boxing_conversion = false;
        let (mut require, require_type_arguments) = disambiguated?;
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
            // An extension call resolves to the method with other type arguments, which the
            // invocation and its extension method question take.
            if is_extension && found.key != method.key {
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
        let found_method_args = found_override
            .as_ref()
            .filter(|_| is_extension)
            .map(|f| f.method_args.clone().unwrap_or_default());
        let written_method_args = match &found_method_args {
            Some(args) => Some(args),
            None => method.method_args.as_ref(),
        };
        let type_arguments = match written_method_args {
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
            && let Some(e) = self.extension_method_call(
                inst,
                operand,
                found_override.as_ref().unwrap_or(&method),
                &list,
                &type_arguments,
                found_method_args.as_deref(),
            )?
        {
            return Ok(e);
        }
        // `this.Dispose()` of an explicitly implemented `IDisposable.Dispose` is written as
        // `((IDisposable)this).Dispose()`, the cast without a resolve result; other targets of
        // such a call are not translated.
        let mut name = method.name.clone();
        let mut callee_target = target.expression;
        if explicit_implementation {
            // shortcut: the host's lookup finds the implementation by its metadata name (`I.M`)
            // without a target where the managed one needs `this`, so a `this` target is
            // written whatever the probe said; with
            // `AlwaysCastTargetsOfExplicitInterfaceImplementationCalls` (set for
            // `InitializeComponent`) any target is cast, and other targets are not translated.
            if !matches!(self.ast.node(target.expression), AstNode::This) {
                return Err("a call of an explicit interface implementation".to_owned());
            }
            require = true;
            let implemented = self
                .ts
                .explicit_implementations(id)
                .map_err(|e| e.to_string())?;
            let interface_member = implemented
                .first()
                .ok_or("an explicit implementation without interface member")?;
            let interface = interface_member
                .declaring
                .ok_or("an interface member without declaring type")?;
            callee_target = self.ast.add(AstNode::Cast {
                ty: interface,
                expression: target.expression,
                overflow: Overflow::None,
            });
            name.clone_from(&interface_member.name);
        }
        let callee = match (require, type_arguments.is_empty()) {
            (true, true) => self.ast.add(AstNode::MemberReference {
                target: callee_target,
                name,
            }),
            (false, true) => self.ast.add(AstNode::Identifier { name }),
            (true, false) => self.ast.add(AstNode::GenericMemberReference {
                target: callee_target,
                name,
                type_arguments,
            }),
            (false, false) => self.ast.add(AstNode::GenericIdentifier {
                name,
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
                expanded: list.is_expanded_form,
                method_args: None,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression {
            expression: e,
            ty: return_type,
        })
    }

    /// `IsStringToReadOnlySpanCharAsSpan`: `MemoryExtensions.AsSpan(string)`.
    fn is_string_to_read_only_span_char_as_span(&self, method: &Method, declaring: TypeId) -> bool {
        method.is_static
            && method.name == "AsSpan"
            && method.parameters.len() == 1
            && method.method_args.as_ref().is_none_or(|a| a.is_empty())
            && self
                .ts
                .definition(declaring)
                .and_then(|d| self.ts.comp.type_def(d).ok())
                .is_some_and(|i| {
                    i.full_name.top.ns == "System" && i.full_name.name() == "MemoryExtensions"
                })
            && self
                .ts
                .is_known(method.return_type, KnownTypeCode::ReadOnlySpanOfT)
            && self
                .ts
                .first_type_argument(method.return_type)
                .is_some_and(|a| self.ts.is_known(a, KnownTypeCode::Char))
            && self
                .ts
                .is_known(method.parameters[0].ty, KnownTypeCode::String)
    }

    /// `IsReadOnlySpanCastUp`: `ReadOnlySpan<To>.CastUp<From>(ReadOnlySpan<From>)`, the helper
    /// the C# 14 compiler emits for the covariant implicit span conversion.
    fn is_read_only_span_cast_up(&self, method: &Method, declaring: TypeId) -> bool {
        method.is_static
            && method.name == "CastUp"
            && method.parameters.len() == 1
            && method.method_args.as_ref().is_some_and(|a| a.len() == 1)
            && self.ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT)
            && self
                .ts
                .is_known(method.parameters[0].ty, KnownTypeCode::ReadOnlySpanOfT)
    }

    /// `IsEquivalentToSpanConversion`: an implicit conversion operator of a span type,
    /// `AsSpan(string)` or `ReadOnlySpan<T>.CastUp`.
    fn is_equivalent_to_span_conversion(&self, method: &Method) -> bool {
        let Some(declaring) = method.declaring else {
            return false;
        };
        ((self.ts.is_known(declaring, KnownTypeCode::SpanOfT)
            || self.ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT))
            && method.is_operator
            && method.name == "op_Implicit")
            || self.is_string_to_read_only_span_char_as_span(method, declaring)
            || self.is_read_only_span_cast_up(method, declaring)
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
                // The compiler emits an implicit span conversion as a call of one of the span
                // types' own members, so the call is the conversion.
                c => c.is_implicit_span() && self.is_equivalent_to_span_conversion(method),
            };
        if !direct {
            let parameter = method.parameters[0].ty;
            argument = self.convert_to(argument, parameter, false)?;
        }
        // `(T)(in arg)` is invalid syntax.
        if let AstNode::InDirection { expression } = *self.ast.node(argument.expression) {
            argument = self.unwrap_child(argument.expression, expression);
        }
        let c = self.ast.add(AstNode::Cast {
            ty: target,
            expression: argument.expression,
            overflow: Overflow::None,
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
        method_args: Option<&[TypeId]>,
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
        let mut first = list.arguments[0];
        let mut target_rr = self.rr_of(first.expression);
        // A null literal has no type of its own: the target is `(T)null`, of the first
        // parameter's type, asked about as an expression of that type.
        // shortcut: the cast carries the host's `ResolveCast` of the literal where the managed
        // cast has none and the target is its `NullLiteralConversion`; both are of the type.
        let null_target = matches!(
            target_rr,
            Rr::Constant {
                value: ConstValue::Null,
                ..
            }
        );
        if null_target {
            // shortcut: where the call needs its type arguments, the engine writes them where
            // the managed builder may leave them out (`((IEnumerable<int>)null).Any()` in
            // `QualifierTests` compiled with nullable annotations); find where the managed
            // escalation differs before lifting this.
            if !type_arguments.is_empty() {
                return Err("an extension method call on null".to_owned());
            }
            let ty = method.parameters[0].ty;
            target_rr = Rr::Conversion { ty };
            first = TranslatedExpression {
                expression: first.expression,
                ty,
            };
        }
        // A `ref this` extension method takes the referenced expression as its target, which
        // carries the reference implicitly; an `out` argument is no target.
        match *self.ast.node(first.expression) {
            AstNode::RefDirection { expression } | AstNode::InDirection { expression }
                if self.settings.ref_extension_methods =>
            {
                target_rr = self.rr_of(expression);
                first = TranslatedExpression {
                    expression,
                    ty: self.rr_type(&target_rr),
                };
            }
            AstNode::RefDirection { .. } | AstNode::OutDirection { .. } => return Ok(None),
            _ => {}
        }
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let target = if null_target {
            ResolveResult::Expression(first.ty)
        } else {
            self.semantic_rr(first.expression, first.ty)
        };
        // `GetArgumentResolveResults(skipCount: 1)`
        let arguments = self.argument_resolve_results(list).split_off(1);
        let remaining = names
            .as_ref()
            .map(|n| n[1..].to_vec())
            .filter(|n| n.iter().any(Option::is_some));
        // A method group argument goes with the method reference it was written as, which the
        // host looks up again.
        let mut groups = Vec::with_capacity(arguments.len());
        for a in &list.arguments[1..list.actual_count()] {
            groups.push(match self.rr_of(a.expression) {
                // shortcut: the host looks a method group argument up without its extension
                // methods; teach `QueryMethodGroup` `ResolveMemberAccess` when a body needs it.
                Rr::MethodGroup {
                    extension: true, ..
                } => {
                    return Err(
                        "an extension method question with an extension method group".to_owned(),
                    );
                }
                Rr::MethodGroup {
                    target,
                    method,
                    type_arguments,
                    ..
                } => {
                    let target = match target.as_deref() {
                        None => None,
                        Some(&Rr::This {
                            ty,
                            causes_non_virtual_invocation,
                            ..
                        }) => Some(LookupTarget::This {
                            ty,
                            causes_non_virtual_invocation,
                        }),
                        Some(&Rr::Type(ty)) => Some(LookupTarget::Type(ty)),
                        Some(rr) => Some(LookupTarget::Expression(self.rr_type(rr))),
                    };
                    Some((*method, target, type_arguments, None))
                }
                Rr::LocalFunctionGroup { name, method, .. } => {
                    Some((*method, None, Vec::new(), Some(name)))
                }
                _ => None,
            });
        }
        if groups.len() != arguments.len() {
            return Err("an extension method question of another argument count".to_owned());
        }
        let method_groups: Vec<_> = groups
            .iter()
            .map(|g| {
                g.as_ref()
                    .map(
                        |(method, target, type_arguments, local_function)| MethodGroupOperand {
                            method,
                            target: *target,
                            type_arguments,
                            local_function: local_function.as_deref(),
                        },
                    )
            })
            .collect();
        // The question carries expressions, constants, references, `out var`, lambdas, method
        // groups and tuples of expressions and constants.
        fn plain_tuple(elements: &[ResolveResult]) -> bool {
            elements.iter().all(|e| match e {
                ResolveResult::Expression(_) | ResolveResult::Constant { .. } => true,
                ResolveResult::Tuple { elements, .. } => plain_tuple(elements),
                _ => false,
            })
        }
        for (i, rr) in std::iter::once(&target).chain(&arguments).enumerate() {
            let kind = match rr {
                ResolveResult::Expression(_)
                | ResolveResult::Constant { .. }
                | ResolveResult::ByReference { .. }
                | ResolveResult::OutVar(_)
                | ResolveResult::Lambda(_) => continue,
                ResolveResult::Tuple { elements, .. } if plain_tuple(elements) => continue,
                ResolveResult::MethodGroup(_) if i > 0 && method_groups[i - 1].is_some() => {
                    continue;
                }
                ResolveResult::Type(_) => "a type",
                ResolveResult::DefaultLiteral(_) => "a default literal",
                ResolveResult::InterpolatedString(_) => "an interpolated string",
                ResolveResult::Throw => "a throw expression",
                ResolveResult::Tuple { .. } => "a tuple",
                ResolveResult::This { .. } => "this",
                ResolveResult::MethodGroup(_) => "a method group",
            };
            return Err(format!("an extension method question with {kind}"));
        }
        let can = resolver
            .can_use_extension_method_syntax(
                self.ts,
                operand,
                method_args,
                type_arguments,
                &target,
                &arguments,
                &method_groups,
                remaining.as_deref(),
            )
            .ok_or("an extension method question the host cannot answer")?;
        if !can {
            return Ok(None);
        }
        if null_target {
            let c = self.ast.add(AstNode::Cast {
                ty: first.ty,
                expression: first.expression,
                overflow: Overflow::None,
            });
            self.ast.with_rr(c, Rr::Conversion { ty: first.ty });
            first.expression = c;
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
                expanded: list.is_expanded_form,
                method_args: method_args.map(<[TypeId]>::to_vec),
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
    pub(crate) fn argument_list(
        &mut self,
        call: &CallData,
        method: &Method,
        id: crate::typesystem::members::MethodDefId,
        first_param: usize,
        declaring: TypeId,
        is_setter: bool,
    ) -> Result<ArgumentList, Unsupported> {
        let mut infos = self.ts.method_signature(id).1;
        // A local function's reduced method has no closure parameters.
        if call.method.local_function.is_some() {
            infos.truncate(method.parameters.len());
        }
        // `VarArgInstanceMethod.Parameters`: the `__arglist` parameter is replaced by the types
        // passed after the sentinel, which are no optional parameters.
        if let Some(extra) = &call.method.method.varargs
            && let Some(mut arglist) = infos.pop()
        {
            arglist.optional = false;
            infos.extend(std::iter::repeat_n(arglist, extra.len()));
        }
        // A slicing `SyntheticRangeIndexAccessor` has one parameter, no optional one.
        if call
            .method
            .range_accessor
            .as_ref()
            .is_some_and(|r| r.slicing)
        {
            infos.truncate(1);
            if let Some(first) = infos.first_mut() {
                first.optional = false;
                first.name.clear();
            }
        }
        if infos.len() != method.parameters.len() {
            return Err("a call with mismatched parameters".to_owned());
        }
        if call.args.len() != first_param + method.parameters.len() {
            return Err("a call with mismatched arguments".to_owned());
        }
        // The parameter of each argument: the named argument map of `CallWithNamedArgs`.
        let map = self.named_argument_map.take();
        // A setter's assigned value is taken off the end of the argument list, where it stays
        // in an indexer assignment, which evaluates it after the indices.
        if map.as_ref().is_some_and(|m| {
            method.is_operator
                || (is_setter && m.last() != Some(&method.parameters.len().wrapping_sub(1)))
        }) {
            return Err("named arguments of a setter or operator".to_owned());
        }
        let order: Vec<usize> = map
            .clone()
            .unwrap_or_else(|| (0..method.parameters.len()).collect());
        // Names start at the first argument out of its place.
        let mut explicit_names: Option<Vec<Option<String>>> = None;
        let named_parameters = if map.is_some() {
            self.named_parameters(id, method, declaring)?
        } else {
            None
        };
        for (i, &p) in order.iter().enumerate() {
            if explicit_names.is_none() && p != i {
                explicit_names = Some(vec![None; order.len()]);
            }
            let name = named_parameters
                .as_ref()
                .and_then(|n| n.get(p))
                .unwrap_or(&method.parameters[p].name);
            // `GetNamedParameters`: the assigned value is past the end of the indexer's
            // parameters and has no name.
            let is_value = is_setter && p + 1 == method.parameters.len();
            if let Some(names) = &mut explicit_names
                && !is_value
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
            is_expanded_form: false,
            expanded_parameters: None,
            params_candidate: None,
        };
        let declaring_is_nullable = self.ts.is_known(declaring, KnownTypeCode::NullableOfT);
        for (i, &pi) in order.iter().enumerate() {
            let parameter = &method.parameters[pi];
            let arg = self.translate(call.args[first_param + i], Some(parameter.ty))?;
            // `TransformParamsArgument`: an array creation or `Array.Empty<T>()` as the last
            // argument of a `params` parameter is a candidate for the expanded form, which `call`
            // probes; a span constructor or conversion is not translated.
            if parameter.is_params
                && self.settings.expand_params_arguments
                && !is_setter
                && list.parameter_map.is_none()
                && i + 1 == order.len()
            {
                list.params_candidate = self.params_elements(arg, parameter.ty)?;
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
            // A dynamic parameter takes the argument as `object`; a dynamic argument converts
            // explicitly.
            let parameter_type = if self.ts.kind(parameter.ty) == TypeKind::Dynamic {
                self.ts.known_type(KnownTypeCode::Object)
            } else {
                parameter.ty
            };
            let allow_implicit = self.ts.kind(arg.ty) != TypeKind::Dynamic;
            let mut arg = self.convert_to(arg, parameter_type, allow_implicit)?;
            // An operator's operand keeps its type: the default literal converts to every type.
            if method.is_operator {
                arg = self.restore_default_literal_type(arg)?;
            }
            if parameter.ref_kind == RefKind::Out {
                arg = self.change_direction_to_out(arg);
            }
            if matches!(parameter.ref_kind, RefKind::In | RefKind::RefReadOnly) {
                let is_address_of = matches!(
                    self.f.inst(call.args[first_param + i]),
                    Inst::AddressOf { .. }
                );
                arg = self.change_direction_to_in(
                    arg,
                    is_address_of,
                    parameter.ref_kind == RefKind::RefReadOnly,
                );
                // An rvalue bound to an `in` parameter is an ordinary value: a span conversion
                // gets the chance to become implicit that by-value arguments get.
                if let AstNode::Cast { expression, .. } = *self.ast.node(arg.expression)
                    && matches!(
                        self.rr_of(arg.expression),
                        Rr::ImplicitConversion { .. } | Rr::Conversion { .. }
                    )
                    && let TypeData::ByRef(element) =
                        *self.ts.data(self.ts.skip_modifiers(parameter.ty))
                {
                    let inner = self.rr_of(expression);
                    let inner = self.rr_type(&inner);
                    if Conversions::new(self.ts)
                        .implicit_type(inner, arg.ty)
                        .is_implicit_span()
                    {
                        arg = self.convert_to(arg, element, true)?;
                    }
                }
            }
            list.arguments.push(arg);
        }
        Ok(list)
    }

    /// `HandleRangeConstruction`: `start..end` of a `Range` member, with the member resolve
    /// result of the method.
    fn range_expression(
        &mut self,
        inst: NodeId,
        operand: &crate::il::ast::MethodOperand,
        method: &Method,
        start: Option<AstId>,
        end: Option<AstId>,
    ) -> TranslatedExpression {
        // `MemberResolveResult.ComputeType`: a constructor's is its declaring type.
        let ty = match method.declaring {
            Some(declaring) if method.name == ".ctor" => declaring,
            _ => method.return_type,
        };
        let e = self.ast.add(AstNode::Range { start, end });
        self.ast.with_rr(
            e,
            Rr::MethodMember {
                method: Box::new(operand.clone()),
                ty,
            },
        );
        self.ast.with_il(e, inst);
        TranslatedExpression { expression: e, ty }
    }

    /// The `VarArgInstanceMethod` step of `CallBuilder.Build`: the arguments passed after the
    /// sentinel become one `__arglist(...)` argument, each converted to its type at the call
    /// site, and the call is of the base method, which is returned.
    fn vararg_argument_list(
        &mut self,
        resolved: &crate::typesystem::resolve::ResolvedMethod,
        list: &mut ArgumentList,
    ) -> Result<Method, Unsupported> {
        if list.parameter_map.is_some() {
            return Err("named arguments of a vararg call".to_owned());
        }
        let mut base = resolved.clone();
        base.varargs = None;
        let base = Method::from_resolved(self.ts, &base).ok_or("a call of an unresolved method")?;
        let regular = base
            .parameters
            .len()
            .checked_sub(1)
            .ok_or("a vararg method without parameters")?;
        list.first_optional = -1;
        list.add_names_to_primitive_values = false;
        list.use_implicitly_typed_out = false;
        let extra = list.arguments.split_off(regular);
        let types = resolved.varargs.clone().unwrap_or_default();
        let mut arguments = Vec::with_capacity(extra.len());
        for (a, ty) in extra.into_iter().zip(types) {
            arguments.push(self.convert_to(a, ty, false)?.expression);
        }
        /// `UndocumentedExpressionType.ArgList`.
        const ARG_LIST: u8 = 1;
        let e = self.ast.add(AstNode::Undocumented {
            kind: ARG_LIST,
            arguments,
        });
        let ty = self.ts.intern(TypeData::ArgList);
        self.ast.with_rr(e, Rr::Plain(ty));
        list.arguments
            .push(TranslatedExpression { expression: e, ty });
        list.parameter_names.truncate(regular);
        list.is_primitive_value.truncate(regular);
        list.ref_kinds.truncate(regular);
        for p in &base.parameters[regular..] {
            list.parameter_names.push(p.name.clone());
            list.is_primitive_value.push(false);
            list.ref_kinds.push(p.ref_kind);
        }
        list.params_candidate = None;
        Ok(base)
    }

    /// `GetArgumentExpressions(skipCount)`: the written arguments from `skip` on, named where
    /// the list names them, with `out` arguments marked as `out var` while the list writes
    /// them so.
    pub(crate) fn written_arguments(&mut self, list: &ArgumentList, skip: usize) -> Vec<AstId> {
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
    pub(crate) fn change_direction_to_out(
        &mut self,
        arg: TranslatedExpression,
    ) -> TranslatedExpression {
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

    /// `ChangeDirectionExpressionTo(input, In, isAddressOf)`: `in x` of a `ref x` argument, or
    /// the value itself where the reference is a temporary's address or `this`.
    pub(crate) fn change_direction_to_in(
        &mut self,
        arg: TranslatedExpression,
        is_address_of: bool,
        ref_readonly: bool,
    ) -> TranslatedExpression {
        let AstNode::RefDirection { expression } = *self.ast.node(arg.expression) else {
            return arg;
        };
        let Some(Rr::ByReference(element)) = self.ast.annotations(arg.expression).rr.clone() else {
            return arg;
        };
        if is_address_of || matches!(self.ast.node(expression), AstNode::This) {
            return self.unwrap_child(arg.expression, expression);
        }
        let d = self.ast.add(AstNode::InDirection { expression });
        for inst in self.ast.annotations(arg.expression).il.clone() {
            self.ast.with_il(d, inst);
        }
        self.ast
            .with_rr(d, Rr::ByReferenceIn(element, ref_readonly));
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

    /// `GetNamedParameters`: the names of an indexer accessor's parameters are those of the
    /// indexer, which takes them from its getter where it has one; `None` for the method's own.
    fn named_parameters(
        &mut self,
        id: crate::typesystem::members::MethodDefId,
        method: &Method,
        declaring: TypeId,
    ) -> Result<Option<Vec<String>>, Unsupported> {
        let Some((token, _)) = self.ts.accessor_owner(id) else {
            return Ok(None);
        };
        if token >> 24 != 0x17 {
            return Ok(None);
        }
        let row = token & 0x00FF_FFFF;
        let name = self.property_info(id.module, row)?.0;
        let default_member = match self.ts.definition(declaring) {
            Some(def) => self
                .ts
                .default_member_name(def)
                .map_err(|e| e.to_string())?,
            None => None,
        };
        let getter = self.ts.comp.module(id.module).md.property_accessors(row).0;
        if default_member.as_deref() != Some(&name)
            || getter == 0
            || getter == id.row
            || method.parameters.is_empty()
        {
            return Ok(None);
        }
        let getter = crate::typesystem::members::MethodDefId {
            module: id.module,
            row: getter,
        };
        Ok(Some(
            self.ts
                .method_signature(getter)
                .1
                .into_iter()
                .map(|p| p.name)
                .collect(),
        ))
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
            // `DefaultLiteralResolveResult` is a constant with value null.
            Rr::DefaultLiteral { .. } => ConstValue::Null,
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

    /// `IsAppropriateCallTarget` of an indexer found for a virtual call: an override of the
    /// expected one. The accessor found (an override's getter where it declares no setter)
    /// overrides an accessor of the expected property, directly or through further overrides.
    fn overrides_property(
        &mut self,
        op: CallOp,
        found: crate::typesystem::members::MethodDefId,
        property: Option<(ModuleId, u32)>,
        found_method: &Method,
    ) -> Result<bool, Unsupported> {
        let is_override = |s: &Self, m| s.ts.method_modifiers_of(m).is_ok_and(|m| m.is_override);
        if op != CallOp::CallVirt || property.is_none() || !is_override(self, found) {
            return Ok(false);
        }
        // A boxed target does not reach an override in a value type.
        if self.needs_boxing_conversion
            && found_method
                .declaring
                .is_some_and(|d| self.ts.is_reference_type(d) != Some(true))
        {
            return Ok(false);
        }
        let bases = self
            .ts
            .base_members(
                crate::typesystem::inheritance::MemberDef::Method(found),
                false,
            )
            .map_err(|e| e.to_string())?;
        for base in bases {
            let crate::typesystem::inheritance::MemberDef::Method(base_id) = base.def else {
                break;
            };
            let owner = self
                .ts
                .accessor_owner(base_id)
                .map(|(token, _)| (base_id.module, token));
            if owner == property {
                return Ok(true);
            }
            if !is_override(self, base_id) {
                break;
            }
        }
        Ok(false)
    }

    /// `Disambiguator.OmittedArgumentsAreDefaultsOf`: the arguments left out are the default
    /// values of the member found, which for a virtual call can be an override that redeclares
    /// them.
    fn omitted_arguments_are_defaults_of(
        &mut self,
        list: &ArgumentList,
        found: &Method,
    ) -> Result<bool, Unsupported> {
        let omitted_from = list.actual_count();
        if omitted_from >= list.arguments.len() {
            return Ok(true);
        }
        let MethodKey::Def(found_id) = found.key else {
            return Ok(false);
        };
        let infos = self.ts.method_signature(found_id).1;
        for i in omitted_from..list.arguments.len() {
            let p = match &list.parameter_map {
                Some(map) => map[i],
                None => i,
            };
            let Some(info) = infos.get(p) else {
                return Ok(false);
            };
            if !self.is_optional_argument(found_id, info, list.arguments[i])? {
                return Ok(false);
            }
        }
        Ok(true)
    }

    /// `CallBuilder.Build` for `newobj` of an ordinary constructor): `new T(arguments)`.
    pub(crate) fn new_obj(
        &mut self,
        inst: NodeId,
        call: &CallData,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        // `CallWithNamedArgs` builds the constructor call (`Build(OpCode.NewObj, ...)`), which
        // is no tuple literal.
        if self.named_argument_map.is_none()
            && let Some(tuple) = self.tuple_construction(inst, call, hint)?
        {
            return Ok(tuple);
        }
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
        // stackalloc spans, delegates, tuples, ranges and anonymous types.
        let ts = &*self.ts;
        let name = ts
            .definition(declaring)
            .and_then(|d| ts.comp.type_def(d).ok())
            .map(|i| (i.full_name.top.ns.clone(), i.full_name.name().to_owned()));
        if ts.kind(declaring) == TypeKind::Delegate {
            return self.delegate_construction(inst, call, declaring);
        }
        let is_span = ts.is_known(declaring, KnownTypeCode::SpanOfT)
            || ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT);
        // `VisitNewObj`: a span of a stackalloc initializer.
        if is_span
            && call.args.len() == 2
            && matches!(
                self.f.inst(call.args[0]),
                Inst::Block {
                    kind: crate::il::ast::BlockKind::StackAllocInitializer,
                    ..
                }
            )
        {
            let element = match self.ts.data(declaring) {
                TypeData::Inst { args, .. } => args.first().copied(),
                _ => None,
            };
            return self.stack_alloc_initializer(call.args[0], element);
        }
        let special = match name.as_ref() {
            None => Some("an unnamed type"),
            Some((ns, name)) if ns == "System" && name.starts_with("ValueTuple") => Some("a tuple"),
            _ => None,
        };
        if let Some(form) = special {
            return Err(format!(
                "an object creation of {form} with syntax of its own"
            ));
        }
        let MethodOrigin::Def(id) = resolved.origin else {
            return Err("a call of an unresolved method".to_owned());
        };
        let anonymous = self.settings.anonymous_types
            && self.ts.definition(declaring).is_some_and(|d| {
                crate::il::transforms::transform_collection_and_object_initializers::is_anonymous_type(
                    self.ts, d,
                )
            });
        let mut list = self.argument_list(call, &method, id, 0, declaring, false)?;
        // `HandleRangeConstruction`: `new Range(a, b)` is `a..b`.
        if self.settings.ranges
            && self.ts.is_known(declaring, KnownTypeCode::Range)
            && list.explicit_names.is_none()
            && list.arguments.len() == 2
        {
            let (start, end) = (list.arguments[0].expression, list.arguments[1].expression);
            return Ok(self.range_expression(inst, operand, &method, Some(start), Some(end)));
        }
        // `HandleRangeConstruction`: `new Index(n, true)` is `^n`.
        if self.settings.ranges
            && self.ts.is_known(declaring, KnownTypeCode::Index)
            && list.parameter_map.is_none()
            && list.arguments.len() == 2
            && matches!(
                self.ast.node(list.arguments[1].expression),
                AstNode::ConstantValue {
                    value: ConstValue::Boolean(true),
                    ..
                }
            )
        {
            /// `UnaryOperatorType.IndexFromEnd`.
            const INDEX_FROM_END: u8 = 16;
            let e = self.ast.add(AstNode::UnaryOperator {
                op: INDEX_FROM_END,
                expression: list.arguments[0].expression,
                overflow: Overflow::None,
            });
            self.ast.with_rr(e, Rr::Operator { ty: declaring });
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression {
                expression: e,
                ty: declaring,
            });
        }
        if anonymous {
            return self.anonymous_type_creation(inst, operand, &method, &list);
        }
        // `TransformParamsArgument`: the expanded form where the creation as written with it
        // resolves to the constructor in that form.
        if let Some(expanded) = list.expanded(&method) {
            let resolver = self
                .resolver
                .ok_or("a member reference without the host's resolver")?;
            let group = resolver
                .constructors(self.ts, operand)
                .ok_or("constructors the host cannot list")?;
            let errors = self.probe_group(
                &group,
                method.declaring,
                &method,
                CallOp::Call,
                &expanded,
                &[],
            )?;
            if errors == OverloadResolutionErrors::NONE {
                list = expanded;
            }
            list.params_candidate = None;
        }
        self.disambiguate_constructor(operand, &method, &mut list)?;
        let arguments = self.written_arguments(&list, 0);
        let e = self.ast.add(AstNode::ObjectCreate {
            ty: declaring,
            arguments,
            initializer: None,
        });
        // Where native integers need no attribute, a created IntPtr or UIntPtr is nint or
        // nuint (the host overrides the return type alike), so that later conversions are
        // those of native integers.
        let native = self.ts.options(ModuleId(0)) & options::NATIVE_INTEGERS_WITHOUT_ATTRIBUTE != 0;
        let ty = if native && self.ts.is_known(declaring, KnownTypeCode::IntPtr) {
            self.ts.intern(TypeData::NInt)
        } else if native && self.ts.is_known(declaring, KnownTypeCode::UIntPtr) {
            self.ts.intern(TypeData::NUInt)
        } else {
            declaring
        };
        self.ast.with_rr(
            e,
            Rr::ObjectCreation {
                method: Box::new(operand.clone()),
                ty,
                expanded: list.is_expanded_form,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `HandleConstructorCall` of an anonymous type: `new { a, b.c }` where each argument is
    /// a name or member reference of its parameter's name, else `new { A = (T)a, ... }`.
    fn anonymous_type_creation(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
        method: &Method,
        list: &ArgumentList,
    ) -> Result<TranslatedExpression, Unsupported> {
        let parameters = list.expected_parameters(method);
        // `CanInferAnonymousTypePropertyNamesFromArguments`
        let names = &list.parameter_names;
        let inferred =
            list.arguments
                .iter()
                .zip(names)
                .all(|(a, p)| match self.ast.node(a.expression) {
                    AstNode::Identifier { name }
                    | AstNode::GenericIdentifier { name, .. }
                    | AstNode::MemberReference { name, .. }
                    | AstNode::GenericMemberReference { name, .. } => name == p,
                    _ => false,
                });
        let initializers = if inferred {
            self.written_arguments(list, 0)
        } else {
            let mut initializers = Vec::with_capacity(list.arguments.len());
            for ((&a, p), name) in list.arguments.iter().zip(&parameters).zip(names) {
                let converted = self.convert_to(a, p.ty, false)?;
                initializers.push(self.ast.add(AstNode::Named {
                    name: name.clone(),
                    expression: converted.expression,
                }));
            }
            initializers
        };
        let e = self.ast.add(AstNode::AnonymousTypeCreate { initializers });
        let ty = method.declaring.ok_or("a method without declaring type")?;
        self.ast.with_rr(
            e,
            Rr::ObjectCreation {
                method: Box::new(operand.clone()),
                ty,
                expanded: list.is_expanded_form,
            },
        );
        self.ast.with_il(e, inst);
        Ok(TranslatedExpression { expression: e, ty })
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
        mut target: TranslatedExpression,
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
            self.disambiguate_accessor(operand, op, &mut target, require, declaring)?;
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
    /// accessor's (`Disambiguator.ForAccessor`).
    #[allow(clippy::too_many_arguments)]
    fn indexer_access(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
        id: crate::typesystem::members::MethodDefId,
        method: &Method,
        op: CallOp,
        mut target: TranslatedExpression,
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
        let mut group = resolver
            .indexers(self.ts, lookup_target)
            .ok_or("indexers the host cannot name by metadata row")?;
        // `AccessorSteps`: the indexer is written with its target already.
        let mut spent_optional = false;
        let mut spent_cast_arguments = list.arguments.is_empty();
        let mut spent_cast_target = false;
        let mut found_type = None;
        loop {
            let (errors, found) = self.probe_indexers(&group, target.ty, id, op, &list)?;
            if errors == OverloadResolutionErrors::NONE {
                found_type = found;
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
                // `Target.ConvertTo(declaringType)`; the indexers are looked up on the cast.
                let lookup =
                    self.cast_target_to_with(&mut target, declaring, op == CallOp::CallVirt)?;
                group = resolver
                    .indexers(self.ts, lookup)
                    .ok_or("indexers the host cannot name by metadata row")?;
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
            // The indexer found (`disambiguator.FoundMember`) is a member of the target's type,
            // whose type arguments keep their tuple names and nullability.
            None => found_type.unwrap_or(method.return_type),
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
    /// with `CheckBestCandidate`, the best one compared with the accessor by its property; with
    /// the return type of the one found when it is.
    fn probe_indexers(
        &mut self,
        group: &[MethodGroupList],
        lookup_type: TypeId,
        accessor: crate::typesystem::members::MethodDefId,
        op: CallOp,
        list: &ArgumentList,
    ) -> Result<(OverloadResolutionErrors, Option<TypeId>), Unsupported> {
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
        let ambiguous = Ok((OverloadResolutionErrors::AMBIGUOUS_MATCH, None));
        let errors = or.best_candidate_errors(&mut conversions);
        if errors != OverloadResolutionErrors::NONE {
            return Ok((errors, None));
        }
        if or.is_ambiguous() {
            return ambiguous;
        }
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return ambiguous;
        };
        let found_type = found.return_type;
        let found_method = found;
        // `IsAppropriateCallTarget` of the properties: the indexer itself, or for a virtual
        // call an override of it, which overrides the accessor alike.
        let crate::semantics::MethodKey::Def(found) = found_method.key else {
            return ambiguous;
        };
        let found_property = self
            .ts
            .accessor_owner(found)
            .map(|(token, _)| (found.module, token));
        let property = self
            .ts
            .accessor_owner(accessor)
            .map(|(token, _)| (accessor.module, token));
        if (found_property == property
            || self.overrides_property(op, found, property, &found_method)?)
            && self.omitted_arguments_are_defaults_of(list, &found_method)?
        {
            Ok((OverloadResolutionErrors::NONE, Some(found_type)))
        } else {
            ambiguous
        }
    }

    /// `Disambiguator.ForAccessor` without arguments to cast: whether the property is written
    /// with its target, qualified and then cast while its name does not bind back to it.
    fn disambiguate_accessor(
        &mut self,
        accessor: &MethodOperand,
        op: CallOp,
        target: &mut TranslatedExpression,
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
                lookup_target =
                    self.cast_target_to_with(target, declaring, op == CallOp::CallVirt)?;
                continue;
            }
            // No spelling resolves; the most explicit one is emitted anyway.
            return Ok((require, false));
        }
    }

    /// `<PrivateImplementationDetails>.InlineArrayAsSpan` or `InlineArrayAsReadOnlySpan`.
    fn is_inline_array_as_span(&self, method: &Method, declaring: TypeId) -> bool {
        matches!(
            method.name.as_str(),
            "InlineArrayAsSpan" | "InlineArrayAsReadOnlySpan"
        ) && self
            .ts
            .definition(declaring)
            .and_then(|d| self.ts.comp.type_def(d).ok())
            .is_some_and(|i| {
                i.full_name.top.ns.is_empty()
                    && i.full_name.name() == "<PrivateImplementationDetails>"
            })
    }

    /// `IType.GetInlineArrayLength()`: the length of `[InlineArray(n)]` on a struct.
    fn inline_array_length(&mut self, t: TypeId) -> Option<i32> {
        use crate::typesystem::attrvalue::ArgValue;
        if self.ts.kind(t) != TypeKind::Struct {
            return None;
        }
        let def = self.ts.definition(t)?;
        let attributes = self.ts.type_attributes(def).ok()?;
        let attribute = attributes.into_iter().find(|a| {
            self.ts.full_name(a.ty) == "System.Runtime.CompilerServices.InlineArrayAttribute"
        })?;
        match attribute.fixed.first()?.value {
            ArgValue::Int32(n) => Some(n),
            _ => None,
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
        // A span-based `string.Concat` of strings and chars is built before; any other
        // `Concat` is an ordinary call.
        let special = false
            // `FormattableStringFactory.Create` is an interpolated string where its format
            // tokenizes.
            || (in_namespace(
                "System.Runtime.CompilerServices",
                "FormattableStringFactory",
            ) && method.name != "Create");
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
                    // shortcut: a constructor call's `base` or `this` target is not cast; the
                    // engine's probe of `base..ctor()` (F# constructors) escalates where the
                    // managed one resolves. Upgrade once the probes agree.
                    let base_ctor = method.name == ".ctor"
                        && matches!(
                            self.ast.node(target.expression),
                            AstNode::Base | AstNode::This
                        );
                    let uncast = target.expression;
                    // `Target.ConvertTo(declaringType)`: the target cast to the declaring type.
                    lookup_target =
                        self.cast_target_to_with(target, declaring, op == CallOp::CallVirt)?;
                    if base_ctor && target.expression != uncast {
                        return Err("a reference that needs a cast of its target".to_owned());
                    }
                }
                continue;
            }
            if !spent_type_arguments {
                spent_type_arguments = true;
                type_arguments = true;
                continue;
            }
            if !spent_explicit_in {
                // `EnforceExplicitIn`: an `in` argument without direction is written as
                // `in ILSpyHelper_AsRefReadOnly(x)`, which the function body declares.
                let expected = list.expected_parameters(method);
                for (p, a) in expected.iter().zip(list.arguments.iter_mut()) {
                    if p.ref_kind != RefKind::In
                        || matches!(
                            self.ast.node(a.expression),
                            AstNode::InDirection { .. }
                                | AstNode::RefDirection { .. }
                                | AstNode::OutDirection { .. }
                        )
                    {
                        continue;
                    }
                    let helper = self.ast.add(AstNode::Identifier {
                        name: "ILSpyHelper_AsRefReadOnly".to_owned(),
                    });
                    let call = self.ast.add(AstNode::Invocation {
                        target: helper,
                        arguments: vec![a.expression],
                    });
                    let e = self.ast.add(AstNode::InDirection { expression: call });
                    self.ast
                        .with_rr(e, Rr::ByReferenceIn(Box::new(Rr::Plain(a.ty)), false));
                    *a = TranslatedExpression {
                        expression: e,
                        ty: self.ts.intern(TypeData::ByRef(a.ty)),
                    };
                    self.emit_as_ref_readonly = true;
                }
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
            // `TypeVisitor.VisitFunctionPointerType` visits the return and parameter types.
            TypeData::FnPtr(f) => {
                self.contains_anonymous_type_name(f.ret)
                    || f.params
                        .iter()
                        .any(|&p| self.contains_anonymous_type_name(p))
            }
            TypeData::Intersection(_) => true,
            _ => false,
        }
    }

    /// `Target.ConvertTo(declaringType)` of a member reference's target, and the lookup target
    /// of the cast (`base` is cast as `this`). A `this` target cast to a class is not
    /// translated, nor a null-conditional target, which would be cast inside its `?`.
    pub(crate) fn cast_target_to(
        &mut self,
        target: &mut TranslatedExpression,
        declaring: TypeId,
    ) -> Result<LookupTarget, Unsupported> {
        self.cast_target_to_with(target, declaring, false)
    }

    /// `cast_target_to`, where `this_to_class` casts a `this` target to a class as well (the
    /// target of a virtual method reference, which is `this` in the managed builder too).
    pub(crate) fn cast_target_to_with(
        &mut self,
        target: &mut TranslatedExpression,
        declaring: TypeId,
        this_to_class: bool,
    ) -> Result<LookupTarget, Unsupported> {
        // `this` or `base` of the declaring type itself converts without a cast, and stays the
        // lookup target it was (a constructor initializer whose lookup finds no constructor).
        if let Rr::This {
            ty,
            causes_non_virtual_invocation,
            ..
        } = self.rr_of(target.expression)
        {
            let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
            if self.ts.normalize_with(target.ty, n) == self.ts.normalize_with(declaring, n) {
                return Ok(LookupTarget::This {
                    ty,
                    causes_non_virtual_invocation,
                });
            }
        }
        // Any other target of the declaring type itself converts without a cast too, a static
        // member's type reference among them.
        let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
        if self.ts.normalize_with(target.ty, n) == self.ts.normalize_with(declaring, n) {
            return Ok(match self.rr_of(target.expression) {
                Rr::Type(ty) => LookupTarget::Type(ty),
                _ => LookupTarget::Expression(target.ty),
            });
        }
        // shortcut: `this` is cast to a class only as the target of a virtual reference; for a
        // non-virtual one the engine may hold `this` where the managed builder took `base`
        // (`base..ctor()` in F# constructors), which needs no cast. Upgrade once the targets
        // agree.
        let refuse_this = matches!(self.ast.node(target.expression), AstNode::This)
            && !this_to_class
            && !self.ts.is_interface(declaring);
        let value_type = self.ts.is_reference_type(target.ty) == Some(false)
            && !matches!(
                self.ts.data(target.ty),
                TypeData::ByRef(_) | TypeData::Pointer(_)
            );
        if (self.ts.is_reference_type(target.ty) != Some(true) && !value_type)
            || refuse_this
            || matches!(
                self.ast.node(target.expression),
                AstNode::NullConditional { .. }
            )
        {
            return Err("a reference that needs a cast of its target".to_owned());
        }
        *target = self.convert_to(*target, declaring, false)?;
        Ok(LookupTarget::Expression(target.ty))
    }

    /// `IntroduceQueryExpressions.IsNullConditional`: whether `e` is a null-conditional access
    /// chain (`a?.b.c()`).
    pub(crate) fn is_null_conditional(&self, e: AstId) -> bool {
        match *self.ast.node(e) {
            AstNode::NullConditional { rewrap: false, .. } => true,
            AstNode::MemberReference { target, .. }
            | AstNode::GenericMemberReference { target, .. }
            | AstNode::Invocation { target, .. }
            | AstNode::Indexer { target, .. } => self.is_null_conditional(target),
            _ => false,
        }
    }

    /// Whether `e` is a cast other than a lambda's, which a call's target is not written as.
    fn is_cast_target(&self, e: AstId) -> bool {
        matches!(self.ast.node(e), AstNode::Cast { .. })
            && !matches!(self.rr_of(e), Rr::LambdaConversion { .. })
    }

    /// `Disambiguator.CastArguments`: every argument converted to its parameter type with an
    /// explicit cast where it is of another type: a dynamic parameter's as `object`, an `in`
    /// parameter's to its element type, an argument of an anonymous type not at all.
    fn cast_arguments(
        &mut self,
        arguments: &mut [TranslatedExpression],
        parameters: &[crate::semantics::Parameter],
    ) -> Result<(), Unsupported> {
        for (a, p) in arguments.iter_mut().zip(parameters) {
            if self.settings.anonymous_types && self.contains_anonymous_type_name(p.ty) {
                // An argument of an anonymous type cannot be cast; a lambda's return type
                // would be adjusted (`ModifyReturnTypeOfLambda`), which is not translated.
                if matches!(self.ast.node(a.expression), AstNode::Lambda { .. }) {
                    return Err("argument casts to anonymous, dynamic or in parameters".to_owned());
                }
                continue;
            }
            let mut ty = if self.ts.kind(p.ty) == TypeKind::Dynamic {
                self.ts.known_type(KnownTypeCode::Object)
            } else {
                p.ty
            };
            if p.ref_kind == RefKind::In
                && let TypeData::ByRef(element) = *self.ts.data(ty)
                && !matches!(self.ts.data(a.ty), TypeData::ByRef(_))
            {
                ty = element;
            }
            *a = self.convert_to(*a, ty, false)?;
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
        if let (Some(Am::Dynamic(_)), Some(Am::Dynamic(_))) = (&get_awaiter, &get_result) {
            return self.dynamic_await(inst, value);
        }
        // Without a `GetAwaiter` (a runtime async await), the operand is translated as it is,
        // of the type `GetResult` returns.
        if get_awaiter.is_none() {
            let ty = match &get_result {
                Some(Am::Method(m)) => {
                    Method::from_resolved(self.ts, &m.method)
                        .ok_or("an await of an unresolved method")?
                        .return_type
                }
                Some(Am::Dynamic(_)) => self.ts.intern(TypeData::Dynamic),
                None => self.ts.intern(TypeData::UnknownSpecial),
            };
            if matches!(self.f.inst(value), Inst::LdObjIfRef { .. }) {
                return Err("an await of a constrained operand".to_owned());
            }
            let mut operand = self.translate(value, None)?;
            if let AstNode::RefDirection { expression } = *self.ast.node(operand.expression) {
                operand = self.unwrap_child(operand.expression, expression);
            }
            let e = self.ast.add(AstNode::Await {
                expression: operand.expression,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Plain(ty));
            return Ok(TranslatedExpression { expression: e, ty });
        }
        let (Some(Am::Method(awaiter)), Some(Am::Method(result))) = (&get_awaiter, &get_result)
        else {
            return Err("a dynamic await".to_owned());
        };
        // A constrained operand awaits its target.
        let value = match *self.f.inst(value) {
            Inst::LdObjIfRef { target, .. } => target,
            _ => value,
        };
        let awaiter_operand = awaiter;
        let awaiter = Method::from_resolved(self.ts, &awaiter.method)
            .ok_or("an await of an unresolved method")?;
        let result = Method::from_resolved(self.ts, &result.method)
            .ok_or("an await of an unresolved method")?;
        // An extension `GetAwaiter` expects the type of its first parameter.
        let expected = if awaiter.is_static {
            awaiter
                .parameters
                .first()
                .ok_or("an await of an extension GetAwaiter")?
                .ty
        } else {
            awaiter.declaring.ok_or("a method without declaring type")?
        };
        let mut operand = self.translate(value, Some(expected))?;
        if let AstNode::RefDirection { expression } | AstNode::InDirection { expression } =
            *self.ast.node(operand.expression)
        {
            operand = self.unwrap_child(operand.expression, expression);
        }
        // An operand boxed for the `GetAwaiter` call is looked up through the box, which C#
        // writes implicitly where the unboxed operand binds the same `GetAwaiter`.
        let mut boxed = None;
        let mut lookup = operand;
        if let AstNode::Cast {
            expression: inner, ..
        } = *self.ast.node(operand.expression)
        {
            let rr = self.rr_of(inner);
            let inner_ty = self.rr_type(&rr);
            if Conversions::new(self.ts)
                .explicit_type(inner_ty, operand.ty)
                .is_boxing()
            {
                boxed = Some(inner);
                lookup = TranslatedExpression {
                    expression: inner,
                    ty: inner_ty,
                };
            }
        }
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let lookup_target = match self.rr_of(lookup.expression) {
            Rr::This {
                ty,
                causes_non_virtual_invocation,
                ..
            } => LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            },
            Rr::Type(ty) => LookupTarget::Type(ty),
            _ => LookupTarget::Expression(lookup.ty),
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
            is_expanded_form: false,
            expanded_parameters: None,
            params_candidate: None,
        };
        let op = if get_awaiter_callvirt {
            CallOp::CallVirt
        } else {
            CallOp::Call
        };
        let found = if awaiter.is_static {
            // shortcut: `CheckSimpleCall` of an extension `GetAwaiter` is asked as the host's
            // `CanTransformToExtensionMethodCall` without arguments, which binds the same
            // method; upgrade if an awaiter shows where the two answers differ.
            let target = self.semantic_rr(lookup.expression, lookup.ty);
            resolver
                .can_use_extension_method_syntax(
                    self.ts,
                    awaiter_operand,
                    awaiter.method_args.as_deref(),
                    &[],
                    &target,
                    &[],
                    &[],
                    None,
                )
                .ok_or("an extension method question the host cannot answer")?
        } else {
            let errors = self.probe_call(
                resolver,
                &awaiter,
                op,
                Some(lookup_target),
                &lookup,
                &list,
                &[],
            )?;
            self.found_override = None;
            errors == OverloadResolutionErrors::NONE
        };
        if !found {
            operand = self.convert_to(operand, expected, false)?;
        } else if let Some(inner) = boxed {
            operand = self.unwrap_child(operand.expression, inner);
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

    /// `VisitAwait` of a dynamic await: the `GetAwaiter` on `dynamic` binds no method of the
    /// operand, so the operand is converted to `dynamic`; the result is `dynamic`.
    fn dynamic_await(
        &mut self,
        inst: NodeId,
        value: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if matches!(self.f.inst(value), Inst::LdObjIfRef { .. }) {
            return Err("an await of a constrained operand".to_owned());
        }
        let dynamic = self.ts.intern(TypeData::Dynamic);
        let mut operand = self.translate(value, Some(dynamic))?;
        if let AstNode::RefDirection { expression } = *self.ast.node(operand.expression) {
            operand = self.unwrap_child(operand.expression, expression);
        }
        let operand = self.convert_to(operand, dynamic, false)?;
        let e = self.ast.add(AstNode::Await {
            expression: operand.expression,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(dynamic));
        Ok(TranslatedExpression {
            expression: e,
            ty: dynamic,
        })
    }

    /// `IsSpanBasedStringConcat`: a `string.Concat` of `ReadOnlySpan<char>` arguments, each the
    /// implicit conversion of a string or the span of a single char; the strings and chars.
    fn span_based_string_concat(
        &mut self,
        call: &CallData,
    ) -> Option<Vec<(NodeId, KnownTypeCode)>> {
        let m = &call.method.method;
        let ros_char = |b: &Self, t: TypeId| {
            b.ts.is_known(t, KnownTypeCode::ReadOnlySpanOfT)
                && b.ts
                    .first_type_argument(t)
                    .is_some_and(|a| b.ts.is_known(a, KnownTypeCode::Char))
        };
        if m.name != "Concat"
            || !m.is_static
            || !m
                .declaring
                .is_some_and(|d| self.ts.is_known(d, KnownTypeCode::String))
        {
            return None;
        }
        let params = m.params().ok()?;
        if !params.iter().all(|&p| ros_char(self, p)) {
            return None;
        }
        let mut first_string = None;
        let mut operands = Vec::with_capacity(call.args.len());
        for (i, &arg) in call.args.iter().enumerate() {
            match self.f.inst(arg).clone() {
                // `IsStringToReadOnlySpanCharImplicitConversion`
                Inst::Call(c)
                    if c.method.method.name == "op_Implicit"
                        && c.args.len() == 1
                        && ros_char(self, c.method.method.ret) =>
                {
                    first_string.get_or_insert(i);
                    operands.push((c.args[0], KnownTypeCode::String));
                }
                // `IsReadOnlySpanCharCtor`: `new ReadOnlySpan<char>(ref c)`
                Inst::NewObj(c)
                    if c.method.method.name == ".ctor"
                        && c.method.method.declaring.is_some_and(|d| ros_char(self, d))
                        && c.args.len() == 1 =>
                {
                    let Inst::AddressOf { value, .. } = *self.f.inst(c.args[0]) else {
                        return None;
                    };
                    operands.push((value, KnownTypeCode::Char));
                }
                _ => return None,
            }
        }
        (call.args.len() >= 2 && first_string.is_some_and(|i| i <= 1)).then_some(operands)
    }

    /// `BuildStringConcat`: `a + b + ...` of the operands, each operator with the
    /// `MemberResolveResult` of the `Concat` method.
    fn build_string_concat(
        &mut self,
        inst: NodeId,
        call: &CallData,
        operands: &[(NodeId, KnownTypeCode)],
    ) -> Result<TranslatedExpression, Unsupported> {
        let string = self.ts.known_type(KnownTypeCode::String);
        let mut result: Option<TranslatedExpression> = None;
        for &(operand, code) in operands {
            let ty = self.ts.known_type(code);
            let e = self.translate(operand, Some(ty))?;
            let e = self.convert_to(e, ty, false)?;
            result = Some(match result {
                None => e,
                Some(left) => {
                    let b = self.ast.add(AstNode::BinaryOperator {
                        overflow: super::ast::Overflow::None,
                        op: super::ast::BinaryOp::Add,
                        left: left.expression,
                        right: e.expression,
                    });
                    self.ast.with_rr(
                        b,
                        Rr::MethodMember {
                            method: Box::new(call.method.clone()),
                            ty: string,
                        },
                    );
                    TranslatedExpression {
                        expression: b,
                        ty: string,
                    }
                }
            });
        }
        let result = result.ok_or("a string concatenation without operands")?;
        self.ast.with_il(result.expression, inst);
        Ok(result)
    }

    /// `BuildCollectionInitializerExpression`: an element of a collection initializer, the
    /// argument of a one-argument `Add` or `{ a, b }`, its arguments cast where the `Add` group of
    /// the initialized object would resolve to another method (`CastArguments`, after
    /// `NoOptionalArgumentAllowed`). Extension methods and expanded `params` are not translated.
    pub(crate) fn collection_initializer_element(
        &mut self,
        element: NodeId,
        call: &CallData,
        op: CallOp,
        object: TranslatedExpression,
    ) -> Result<AstId, Unsupported> {
        let operand = &call.method;
        let MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a collection initializer of an unresolved method".to_owned());
        };
        let extension = self.ts.method_flags(id, None).extension;
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a collection initializer of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // An extension method's first argument is a `Nop`, translated as an error expression
        // of unknown type that is not written.
        let skip = usize::from(extension);
        let mut list = self.argument_list(call, &method, id, 1 - skip, declaring, false)?;
        if extension {
            let unknown = self.ts.intern(TypeData::UnknownSpecial);
            let e = self.ast.add(AstNode::Error {
                message: "OpCode not supported: Nop".to_owned(),
            });
            self.ast.with_rr(e, Rr::Plain(unknown));
            *list
                .arguments
                .first_mut()
                .ok_or("an extension method without arguments")? = TranslatedExpression {
                expression: e,
                ty: unknown,
            };
        }
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let lookup = Some(LookupTarget::Expression(object.ty));
        // `TransformParamsArgument`: the expanded form where the `Add` call written with it
        // resolves to the method in that form.
        if let Some(expanded) = list.expanded(&method) {
            let errors = self.probe_call(resolver, &method, op, lookup, &object, &expanded, &[])?;
            self.found_override = None;
            if errors == OverloadResolutionErrors::NONE {
                list = expanded;
            }
            list.params_candidate = None;
        }
        list.add_names_to_primitive_values = false;
        list.use_implicitly_typed_out = false;
        let mut spent_optional = false;
        let mut spent_cast = false;
        loop {
            let errors = self.probe_call(resolver, &method, op, lookup, &object, &list, &[])?;
            self.found_override = None;
            if errors == OverloadResolutionErrors::NONE {
                break;
            }
            if !spent_optional && list.first_optional >= 0 {
                spent_optional = true;
                list.first_optional = -1;
                continue;
            }
            if !spent_cast {
                spent_cast = true;
                let expected = list.expected_parameters(&method);
                // The error expression's cast is not written.
                self.cast_arguments(&mut list.arguments[skip..], &expected[skip..])?;
                continue;
            }
            break;
        }
        // One argument needs no braces; the first optional argument is ignored then.
        if let [argument] = list.arguments[skip..] {
            return Ok(self.ast.with_il(argument.expression, element));
        }
        let arguments = self.written_arguments(&list, skip);
        let e = self.ast.add(AstNode::ArrayInitializer {
            elements: arguments,
        });
        self.ast.with_rr(
            e,
            Rr::Invocation {
                target: Box::new(Rr::InitializedObject(object.ty)),
                method: Box::new(operand.clone()),
                ty: method.return_type,
                extension,
                delegate: false,
                found_by_lookup: false,
                expanded: list.is_expanded_form,
                method_args: None,
            },
        );
        Ok(self.ast.with_il(e, element))
    }

    /// `BuildDictionaryInitializerExpression`: `[indices] = value`, the indexer assignment of
    /// the setter call `element` on the initialized object with the indices and value given
    /// (the indices with the index variables of the initializer replaced by their values),
    /// whose target is then removed.
    pub(crate) fn dictionary_initializer_element(
        &mut self,
        element: NodeId,
        indices: &[NodeId],
        value: NodeId,
        object: TranslatedExpression,
    ) -> Result<AstId, Unsupported> {
        let (call, op) = match self.f.inst(element) {
            Inst::Call(c) => (c.clone(), CallOp::Call),
            Inst::CallVirt(c) => (c.clone(), CallOp::CallVirt),
            _ => return Err("a dictionary initializer of another instruction".to_owned()),
        };
        let operand = &call.method;
        let MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a dictionary initializer of an unresolved method".to_owned());
        };
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a dictionary initializer of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // An indexer is the property the type's `[DefaultMember]` names; another parameterized
        // property has no access syntax, which `HandleAccessorCall` would write.
        let (token, _) = self
            .ts
            .accessor_owner(id)
            .ok_or("a dictionary initializer of no accessor")?;
        let name = self.property_info(id.module, token & 0x00FF_FFFF)?.0;
        let default_member = match self.ts.definition(declaring) {
            Some(def) => self
                .ts
                .default_member_name(def)
                .map_err(|e| e.to_string())?,
            None => None,
        };
        if default_member.as_deref() != Some(&name) {
            return Err("a dictionary initializer of a parameterized property".to_owned());
        }
        // `callArguments`: the target (not translated), the indices and the value.
        let mut args = Vec::with_capacity(indices.len() + 2);
        args.push(*call.args.first().ok_or("a setter without target")?);
        args.extend_from_slice(indices);
        args.push(value);
        let call = CallData { args, ..call };
        let list = self.argument_list(&call, &method, id, 1, declaring, true)?;
        let target = self.ast.add(AstNode::Identifier {
            name: "initializedObject".to_owned(),
        });
        self.ast.with_rr(target, Rr::InitializedObject(object.ty));
        let target = TranslatedExpression {
            expression: target,
            ty: object.ty,
        };
        let assignment = self.indexer_access(
            element,
            &call.method,
            id,
            &method,
            op,
            target,
            list,
            true,
            declaring,
        )?;
        let AstNode::Assignment { left, .. } = *self.ast.node(assignment.expression) else {
            return Err("a dictionary initializer without assignment".to_owned());
        };
        let AstNode::Indexer { indices, .. } = self.ast.node(left).clone() else {
            return Err("a dictionary initializer without indexer".to_owned());
        };
        *self.ast.node_mut(left) = AstNode::InitializerIndexer { indices };
        Ok(assignment.expression)
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
            Inst::Call(c) => (c.clone(), Some(CallOp::Call)),
            Inst::CallVirt(c) => (c.clone(), Some(CallOp::CallVirt)),
            // `Build(OpCode.NewObj, ...)`: the constructor call.
            Inst::NewObj(c) => (c.clone(), None),
            _ => return Err("named arguments of another call".to_owned()),
        };
        // An object creation has no target among its arguments.
        let first_param = usize::from(op.is_some() && !call.method.method.is_static);
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
        let result = match op {
            Some(op) => self.call(final_inst, &call, op),
            None => {
                let no_hint = self.ts.intern(TypeData::NoType);
                self.new_obj(final_inst, &call, no_hint)
            }
        };
        self.named_argument_map = None;
        let result = result?;
        self.ast.with_il(result.expression, block);
        Ok(result)
    }

    /// `MatchTupleConstruction` in `CallBuilder.Build`: `new ValueTuple<...>(a, b)` as the tuple
    /// literal `(a, b)`, with the element names of a tuple type hint. Tuples of more than seven
    /// elements (nested through `Rest`) are not translated.
    fn tuple_construction(
        &mut self,
        inst: NodeId,
        call: &CallData,
        hint: TypeId,
    ) -> Result<Option<TranslatedExpression>, Unsupported> {
        let Some(declaring) = call.method.method.declaring else {
            return Ok(None);
        };
        let is_value_tuple = self
            .ts
            .definition(declaring)
            .and_then(|d| self.ts.comp.type_def(d).ok())
            .is_some_and(|i| {
                i.kind == crate::typesystem::compilation::TypeKind::Struct
                    && i.full_name.top.ns == "System"
                    && i.full_name.name() == "ValueTuple"
                    && (2..8).contains(&i.type_parameter_count)
            });
        if !self.settings.tuple_types || !is_value_tuple {
            return Ok(None);
        }
        let TypeData::Inst { args, .. } = self.ts.data(declaring).clone() else {
            return Ok(None);
        };
        if call.args.len() != args.len() {
            return Ok(None);
        }
        let names: Vec<Option<String>> = match self.ts.data(hint) {
            TypeData::Tuple { names, .. } => {
                if names.len() != args.len() {
                    return Err("a tuple literal of a hint of another arity".to_owned());
                }
                names
                    .iter()
                    .map(|n| n.as_deref().map(str::to_owned))
                    .collect()
            }
            _ => vec![None; args.len()],
        };
        let mut elements = Vec::with_capacity(args.len());
        let mut types = Vec::with_capacity(args.len());
        for (&a, &ty) in call.args.iter().zip(args.iter()) {
            let e = self.translate(a, Some(ty))?;
            let e = self.convert_to(e, ty, true)?;
            elements.push(e.expression);
            types.push(e.ty);
        }
        // `TupleResolveResult`: a tuple type of the elements' types, from the module the
        // `ValueTuple` comes from.
        let assembly = self.ts.definition(declaring).map(|d| d.module);
        let underlying = self.ts.tuple_underlying(&types, assembly);
        let ty = self.ts.intern(TypeData::Tuple {
            underlying,
            elements: types.into(),
            names: names
                .iter()
                .map(|n| n.as_deref().map(Box::<str>::from))
                .collect(),
        });
        let e = self.ast.add(AstNode::Tuple {
            elements,
            names,
            value_tuple: declaring,
            names_in_rr: true,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(ty));
        Ok(Some(TranslatedExpression { expression: e, ty }))
    }

    /// `ExtractArguments` of `TransformParamsArgument`: the elements of an array creation of
    /// constant size whose initializer gives every element, or none for `Array.Empty<T>()`,
    /// with the element type of the `params` array. Other arguments stay ordinary ones;
    /// spans and an array creation with default elements are not translated.
    fn params_elements(
        &mut self,
        arg: TranslatedExpression,
        params_type: TypeId,
    ) -> Result<Option<(Vec<TranslatedExpression>, TypeId)>, Unsupported> {
        let rr = self.rr_of(arg.expression);
        // Every expanded argument binds to the element type of the params collection.
        let element_type = match *self.ts.data(params_type) {
            TypeData::Array { elem, rank: 1, .. } => elem,
            _ if self
                .ts
                .is_known(params_type, KnownTypeCode::ReadOnlySpanOfT)
                || self.ts.is_known(params_type, KnownTypeCode::SpanOfT)
                || matches!(
                    self.ts.known_type_code(params_type),
                    KnownTypeCode::IEnumerableOfT
                        | KnownTypeCode::ICollectionOfT
                        | KnownTypeCode::IListOfT
                        | KnownTypeCode::IReadOnlyCollectionOfT
                        | KnownTypeCode::IReadOnlyListOfT
                ) =>
            {
                match self.ts.first_type_argument(params_type) {
                    Some(t) => t,
                    None => return Ok(None),
                }
            }
            _ => return Ok(None),
        };
        // `System.ReadOnlySpan<T>..ctor(ref readonly T)`: the one element.
        if let Rr::ObjectCreation { method, .. } = &rr
            && let AstNode::ObjectCreate {
                ref arguments,
                initializer: None,
                ..
            } = *self.ast.node(arg.expression)
            && let [element] = arguments.as_slice()
        {
            let element = *element;
            let ctor = Method::from_resolved(self.ts, &method.method);
            let single_ref_readonly = ctor.as_ref().and_then(|m| match m.parameters.as_slice() {
                [p] if p.ref_kind == RefKind::RefReadOnly => match *self.ts.data(p.ty) {
                    TypeData::ByRef(e) => Some(e),
                    _ => None,
                },
                _ => None,
            });
            let declaring = method.method.declaring;
            if method.method.name == ".ctor"
                && let Some(param_type) = single_ref_readonly
                && let Some(declaring) = declaring
                && self.ts.is_known(declaring, KnownTypeCode::ReadOnlySpanOfT)
                && self.ts.first_type_argument(declaring) == Some(param_type)
            {
                let rr = self.rr_of(element);
                let ty = self.rr_type(&rr);
                return Ok(Some((
                    vec![TranslatedExpression {
                        expression: element,
                        ty,
                    }],
                    element_type,
                )));
            }
            return Ok(None);
        }
        // An implicit span conversion of an array creation: its elements.
        if let Rr::Conversion { .. } | Rr::ImplicitConversion { .. } = rr {
            let inner = match *self.ast.node(arg.expression) {
                AstNode::Cast { expression, .. } => expression,
                _ => arg.expression,
            };
            if !matches!(self.rr_of(inner), Rr::ArrayCreate { .. }) {
                return Ok(None);
            }
            let inner_rr = self.rr_of(inner);
            let inner_ty = self.rr_type(&inner_rr);
            if !Conversions::new(self.ts)
                .implicit_type(inner_ty, arg.ty)
                .is_implicit_span()
            {
                return Ok(None);
            }
            let array = TranslatedExpression {
                expression: inner,
                ty: inner_ty,
            };
            return self.params_elements(array, params_type);
        }
        match rr {
            Rr::Invocation { method, .. }
                if method.method.name == "Empty"
                    && method
                        .method
                        .declaring
                        .is_some_and(|d| self.ts.is_known(d, KnownTypeCode::Array)) =>
            {
                Ok(Some((Vec::new(), element_type)))
            }
            Rr::ArrayCreate { .. } => {
                let AstNode::ArrayCreate {
                    ref arguments,
                    initializer,
                    ..
                } = *self.ast.node(arg.expression)
                else {
                    return Ok(None);
                };
                let [size] = arguments.as_slice() else {
                    return Ok(None);
                };
                let Rr::Constant {
                    value: ConstValue::Int32(n),
                    ..
                } = self.rr_of(*size)
                else {
                    return Ok(None);
                };
                let elements = match initializer {
                    None => Vec::new(),
                    Some(init) => match self.ast.node(init) {
                        AstNode::ArrayInitializer { elements } => elements.clone(),
                        _ => return Ok(None),
                    },
                };
                if elements.len() > n as usize {
                    return Err("a params array with default elements".to_owned());
                }
                let mut elements: Vec<_> = elements
                    .into_iter()
                    .map(|e| {
                        let rr = self.rr_of(e);
                        TranslatedExpression {
                            expression: e,
                            ty: self.rr_type(&rr),
                        }
                    })
                    .collect();
                // The elements the initializer leaves out are the element type's default.
                while elements.len() < n as usize {
                    elements.push(self.default_value_expression(element_type)?);
                }
                Ok(Some((elements, element_type)))
            }
            _ => Ok(None),
        }
    }

    /// `HandleStringInterpolation` for `string.Format`: the interpolated string the format
    /// string's tokens make with the arguments, or `None` where `TryGetStringInterpolationTokens`
    /// fails (the call is then written as it is).
    fn format_interpolation(
        &mut self,
        inst: NodeId,
        list: &ArgumentList,
        first_argument_node: usize,
        array_literal: bool,
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
        // No other argument may contain a string literal, else the call is an ordinary one. The
        // nodes made for the arguments are scanned, without the format strings nested
        // interpolations consumed.
        // shortcut: the scan also sees nodes a discarded translation left behind; walk the
        // written arguments once the AST has a child iterator.
        let format_node = format_arg.expression.0 as usize;
        let has_string_literal = self.ast.nodes[first_argument_node..]
            .iter()
            .enumerate()
            .any(|(i, (n, _))| {
                let id = first_argument_node + i;
                id != format_node
                    && !self.consumed_format_strings.contains(&id)
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
        // `UnpackSingleElementArray`: the array literal of an unexpanded `params` overload with
        // one element interpolates the element.
        // shortcut: the interpolated string's resolve result takes the element's resolve result
        // where the managed one keeps the array's; send the array's if a consumer reads it.
        let mut unpacked = None;
        if array_literal
            && let AstNode::ArrayCreate {
                initializer: Some(init),
                ..
            } = *self.ast.node(list.arguments[1].expression)
            && let AstNode::ArrayInitializer { elements } = self.ast.node(init)
            && let [element] = elements[..]
        {
            unpacked = Some(element);
        }
        let contents = tokens
            .into_iter()
            .map(|t| match t.index {
                None => super::ast::InterpolatedContent::Text(t.text),
                Some(index) => super::ast::InterpolatedContent::Interpolation {
                    expression: unpacked.unwrap_or(list.arguments[index + 1].expression),
                    alignment: t.alignment,
                    suffix: t.format,
                },
            })
            .collect();
        let string = self.ts.known_type(KnownTypeCode::String);
        self.consumed_format_strings.push(format_node);
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
        // The form meant and the omitted arguments the defaults of the method found, which
        // can be an override that redeclares them (`OmittedArgumentsAreDefaultsOf`); writing
        // the omitted arguments out again corrects either.
        if or.best_candidate_is_expanded_form() != list.is_expanded_form
            || !self.omitted_arguments_are_defaults_of(list, &found)?
        {
            return Ok(if list.first_optional >= 0 {
                OverloadResolutionErrors::MISSING_ARGUMENT_FOR_REQUIRED_PARAMETER
            } else {
                OverloadResolutionErrors::AMBIGUOUS_MATCH
            });
        }
        // The resolve result names the method found, whose inferred type arguments, or the
        // type arguments of the target's type its return type is substituted with, may differ
        // from the call's in tuple names or nullability; the host finds it again.
        if found.method_args != method.method_args || found.return_type != method.return_type {
            self.found_override = Some(found.clone());
        }
        Ok(OverloadResolutionErrors::NONE)
    }

    /// `GetUserDefinedOperatorCandidates` for the operands `IsUnambiguousCall` resolves an
    /// operator call with (and the target type of an explicit conversion): the operators of
    /// that name the underlying types and their base types declare, the primitive types
    /// excepted, each type's operators followed by their lifted forms
    /// (`LiftUserDefinedOperators`).
    fn operator_candidates(
        &mut self,
        method: &Method,
        list: &ArgumentList,
    ) -> Result<Vec<Method>, Unsupported> {
        let written = &list.arguments[..list.actual_count()];
        if !matches!(written.len(), 1 | 2) {
            return Ok(Vec::new());
        }
        let mut types: Vec<TypeId> = written
            .iter()
            .map(|a| self.ts.nullable_underlying(a.ty))
            .collect();
        if written.len() == 1 && method.name == "op_Explicit" {
            types.push(self.ts.nullable_underlying(method.return_type));
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
            let mut operators: Vec<Method> = self
                .method_lists(&groups, Some(t))
                .into_iter()
                .flat_map(|l| l.methods)
                .filter(|m| m.is_operator)
                .collect();
            let lifted: Vec<Method> = operators
                .iter()
                .filter_map(|m| self.lift_user_defined_operator(m))
                .collect();
            operators.extend(lifted);
            for m in operators {
                if !candidates.contains(&m) {
                    candidates.push(m);
                }
            }
        }
        Ok(candidates)
    }

    /// `CSharpOperators.LiftUserDefinedOperator`: the lifted form of a user-defined operator
    /// of non-nullable value types, with nullable parameters and (except for a comparison) a
    /// nullable return type.
    fn lift_user_defined_operator(&mut self, m: &Method) -> Option<Method> {
        let comparison = m.parameters.len() == 2
            && matches!(
                m.name.as_str(),
                "op_Equality"
                    | "op_Inequality"
                    | "op_LessThan"
                    | "op_GreaterThan"
                    | "op_LessThanOrEqual"
                    | "op_GreaterThanOrEqual"
            );
        let non_nullable_value_type = |b: &mut Self, t: TypeId| {
            b.ts.is_reference_type(t) == Some(false)
                && !b.ts.is_known(t, KnownTypeCode::NullableOfT)
        };
        if comparison {
            if !self.ts.is_known(m.return_type, KnownTypeCode::Boolean) {
                return None;
            }
        } else if !non_nullable_value_type(self, m.return_type) {
            return None;
        }
        if !m
            .parameters
            .iter()
            .all(|p| non_nullable_value_type(self, p.ty))
        {
            return None;
        }
        let mut lifted = m.clone();
        for p in &mut lifted.parameters {
            p.ty = crate::il::ast::nullable_type(self.ts, p.ty);
        }
        if !comparison {
            lifted.return_type = crate::il::ast::nullable_type(self.ts, m.return_type);
        }
        lifted.lifted = true;
        Some(lifted)
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
            && expected.lifted == actual.lifted
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
        // A boxed target does not reach an override in a value type.
        if self.needs_boxing_conversion
            && actual
                .declaring
                .is_some_and(|d| self.ts.is_reference_type(d) != Some(true))
        {
            return Ok(false);
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

    /// A token of `TryGetStringInterpolationTokens`: index, text, alignment, format.
    type Token = (Option<usize>, String, i32, Option<String>);

    /// The tokens of `TryGetStringInterpolationTokens`.
    fn tokens(s: &str) -> Option<Vec<Token>> {
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
