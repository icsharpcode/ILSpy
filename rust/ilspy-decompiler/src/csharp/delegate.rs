//! `CallBuilder.HandleDelegateConstruction`: `new D(target.Method)` for the construction of a
//! delegate from a function pointer, with the method group the reference resolves to.

use std::rc::Rc;

use crate::il::ast::{CallData, Inst, MethodOperand, NodeId};
use crate::il::transforms::LookupTarget;
use crate::semantics::{Conversions, Method, MethodGroup, OverloadResolution, ResolveResult};
use crate::typesystem::attrvalue::ArgValue;
use crate::typesystem::fnptr::FnPtrType;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::call_builder::CallOp;
use super::{Builder, TranslatedExpression, Unsupported};

impl Builder<'_, '_, '_, '_> {
    /// `CanUseDelegateConstruction` of a delegate's `newobj D(target, ldftn)`: otherwise the
    /// delegate constructor is called with the function pointer as an argument. A construction
    /// of another shape or of an unresolved method counts as one.
    pub(crate) fn can_use_delegate_construction(&mut self, call: &CallData) -> bool {
        let [this_arg, func] = call.args[..] else {
            return true;
        };
        let (Inst::LdFtn { method: operand }
        | Inst::LdVirtFtn {
            method: operand, ..
        }) = self.f.inst(func).clone()
        else {
            return true;
        };
        let MethodOrigin::Def(id) = operand.method.origin else {
            return true;
        };
        // Accessors cannot be referenced as a method group.
        if self.ts.accessor_owner(id).is_some() {
            return false;
        }
        let Some(declaring) = call.method.method.declaring else {
            return true;
        };
        let Some(invoke) = self.ts.delegate_invoke(declaring) else {
            return true;
        };
        let target_parameters = operand.method.base_params.len();
        if operand.method.is_static {
            if invoke.parameters.len() == target_parameters {
                matches!(self.f.inst(this_arg), Inst::LdNull)
            } else {
                self.ts.method_flags(id, None).extension
                    && invoke.parameters.len() + 1 == target_parameters
            }
        } else {
            invoke.parameters.len() == target_parameters
        }
    }

    /// `HandleDelegateConstruction(CallInstruction)` where `CanUseDelegateConstruction` holds
    /// for an instance method, or a static method of a delegate without a bound first
    /// argument. Extension methods, accessors, compiler-generated methods and value-type targets
    /// boxed other than from an `ldobj` are not translated.
    pub(crate) fn delegate_construction(
        &mut self,
        inst: NodeId,
        call: &CallData,
        delegate_type: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let [this_arg, func] = call.args[..] else {
            return Err("a delegate construction of another shape".to_owned());
        };
        let (operand, op) = match self.f.inst(func).clone() {
            Inst::LdFtn { method } => (method, CallOp::Call),
            Inst::LdVirtFtn { method, .. } => (method, CallOp::CallVirt),
            _ => return Err("a delegate construction of another shape".to_owned()),
        };
        self.delegate_construction_of(inst, this_arg, &operand, op, delegate_type)
    }

    /// The delegate construction of `operand` on `this_arg`, of `newobj D(target, ldftn)` or of
    /// `ldvirtdelegate` (`Build(LdVirtDelegate)`, a `callvirt`).
    pub(crate) fn delegate_construction_of(
        &mut self,
        inst: NodeId,
        this_arg: NodeId,
        operand: &MethodOperand,
        op: CallOp,
        delegate_type: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a delegate of an unresolved method".to_owned());
        };
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a delegate of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        if operand.local_function.is_some() {
            return self.local_function_delegate(inst, this_arg, operand, &method, delegate_type);
        }
        let flags = self.ts.method_flags(id, None);
        // `CanUseDelegateConstruction`
        let invoke = self
            .ts
            .delegate_invoke(delegate_type)
            .ok_or("a delegate type without Invoke")?;
        if self.ts.accessor_owner(id).is_some() {
            return Err("a delegate of an extension method, accessor or local function".to_owned());
        }
        // An extension method bound to its first argument is referenced as an extension
        // method (`target.Method`); one with all its parameters unbound as any static method.
        if flags.extension && invoke.parameters.len() + 1 == method.parameters.len() {
            let reference = self.extension_method_reference(this_arg, operand, &method, op)?;
            let e = self.ast.add(AstNode::DelegateCreate {
                ty: delegate_type,
                reference,
                method: operand.clone(),
                is_virtual: op == CallOp::CallVirt,
            });
            self.ast.with_il(e, inst);
            self.ast.with_rr(e, Rr::Plain(delegate_type));
            return Ok(TranslatedExpression {
                expression: e,
                ty: delegate_type,
            });
        }
        if invoke.parameters.len() != method.parameters.len()
            || (method.is_static && !matches!(self.f.inst(this_arg), Inst::LdNull))
        {
            return Err("a delegate construction that binds its first argument".to_owned());
        }
        // A struct method's target is boxed for the delegate, where `TranslateTarget` expects
        // a reference: that the boxed `ldobj` loads.
        let mut this_arg = this_arg;
        if self.ts.is_reference_type(declaring) == Some(false)
            && let Inst::Box { arg, .. } = *self.f.inst(this_arg)
        {
            // shortcut: a boxed value other than an `ldobj` takes the address of a new
            // `AddressOf` instruction, which the engine does not create; upgrade when a
            // fixture needs it.
            let Inst::LdObj { target, .. } = *self.f.inst(arg) else {
                return Err("a delegate of a value type's method".to_owned());
            };
            this_arg = target;
        }
        let target = self.translate_target(
            (!method.is_static).then_some(this_arg),
            op == CallOp::Call,
            method.is_static,
            declaring,
        )?;
        let reference = self.method_reference(operand, &method, op, target, declaring, false)?;
        let e = self.ast.add(AstNode::DelegateCreate {
            ty: delegate_type,
            reference,
            method: operand.clone(),
            is_virtual: op == CallOp::CallVirt,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(delegate_type));
        Ok(TranslatedExpression {
            expression: e,
            ty: delegate_type,
        })
    }

    /// `VisitLdFtn`: `(delegate*<...>)(&Method)` of a static method, the reference written as
    /// `BuildMethodReference` writes it; an instance method is the pseudo-call `__ldftn(M)`.
    pub(crate) fn ldftn(
        &mut self,
        inst: NodeId,
        operand: &MethodOperand,
    ) -> Result<TranslatedExpression, Unsupported> {
        let MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a function pointer of an unresolved method".to_owned());
        };
        let mut method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a function pointer of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        // `BuildMethodReference(method, isVirtual: false)`, with a `MemberResolveResult` without
        // target in place of the method group.
        let reference = if let Some(local) = &operand.local_function {
            // `LocalFunctionMethod`: static, without the compiler-generated parameters.
            let reference = self.local_function_reference(operand, &method, false)?;
            let generated = local.reduced.number_of_compiler_generated_parameters as usize;
            let keep = method.parameters.len().saturating_sub(generated);
            method.parameters.truncate(keep);
            method.is_static = true;
            reference
        } else {
            let target = self.translate_target(None, true, method.is_static, declaring)?;
            self.method_reference(operand, &method, CallOp::Call, target, declaring, true)?
        };
        // `MemberResolveResult.ComputeType`: a by-reference return is of its element type.
        let member_type = match *self.ts.data(method.return_type) {
            TypeData::ByRef(element) => element,
            _ => method.return_type,
        };
        self.ast.with_rr(
            reference,
            Rr::MethodMember {
                method: Box::new(operand.clone()),
                ty: member_type,
            },
        );
        if !method.is_static {
            // C# function pointers do not support instance methods.
            let name = self.ast.add(AstNode::Identifier {
                name: "__ldftn".to_owned(),
            });
            let e = self.ast.add(AstNode::Invocation {
                target: name,
                arguments: vec![reference],
            });
            let void = self.ts.known_type(KnownTypeCode::Void);
            let ty = self.ts.intern(TypeData::Pointer(void));
            self.ast.with_rr(e, Rr::Plain(ty));
            self.ast.with_il(e, inst);
            return Ok(TranslatedExpression { expression: e, ty });
        }
        // The calling convention and the custom ones of `[UnmanagedCallersOnly]`.
        let mut calling_convention = 0u8;
        let mut custom_conventions = Vec::new();
        let attributes = self.ts.method_attributes(id).map_err(|e| e.to_string())?;
        let unmanaged_callers_only = attributes.iter().find(|a| {
            self.ts.name_and_namespace(a.ty).is_some_and(|(name, ns)| {
                ns == "System.Runtime.InteropServices" && name == "UnmanagedCallersOnlyAttribute"
            })
        });
        if let Some(attribute) = unmanaged_callers_only {
            /// `SignatureCallingConvention.Unmanaged`.
            const UNMANAGED: u8 = 9;
            calling_convention = UNMANAGED;
            let conventions = attribute
                .named
                .iter()
                .find(|n| n.name.as_deref() == Some("CallConvs"));
            if let Some(n) = conventions
                && let ArgValue::Array(elements) = &n.arg.value
            {
                for element in elements {
                    let ArgValue::Type(t) = element.value else {
                        continue;
                    };
                    let found = match self.ts.name_and_namespace(t) {
                        Some((name, ns))
                            if ns == "System.Runtime.CompilerServices"
                                && calling_convention == UNMANAGED =>
                        {
                            match name.as_str() {
                                "CallConvCdecl" => Some(1),
                                "CallConvFastcall" => Some(4),
                                "CallConvStdcall" => Some(2),
                                "CallConvThiscall" => Some(3),
                                _ => None,
                            }
                        }
                        _ => None,
                    };
                    match found {
                        Some(c) => calling_convention = c,
                        None => custom_conventions.push(t),
                    }
                }
            }
        }
        let ret_ref_readonly = self.ts.method_return_type_is_ref_read_only(&operand.method);
        let ftp = self.ts.intern(TypeData::FnPtr(Box::new(FnPtrType {
            calling_convention,
            custom_conventions: custom_conventions.into(),
            ret: method.return_type,
            ret_ref_readonly,
            params: method.parameters.iter().map(|p| p.ty).collect(),
            ref_kinds: method.parameters.iter().map(|p| p.ref_kind as u8).collect(),
        })));
        let address_of = self.ast.add(AstNode::PointerUnary {
            address_of: true,
            expression: reference,
        });
        let no_type = self.ts.intern(TypeData::NoType);
        self.ast.with_rr(address_of, Rr::Plain(no_type));
        self.ast.with_il(address_of, inst);
        let e = self.ast.add(AstNode::Cast {
            ty: ftp,
            expression: address_of,
            overflow: super::ast::Overflow::None,
        });
        self.ast.with_rr(
            e,
            Rr::FunctionPointerConversion {
                ty: ftp,
                method: Box::new(operand.clone()),
            },
        );
        Ok(TranslatedExpression {
            expression: e,
            ty: ftp,
        })
    }

    /// `HandleDelegateConstruction` of a local function, whose reduced method is static:
    /// `new D(Name)` without target or disambiguation (`DisambiguateDelegateReference`), the
    /// name resolved to `ToMethodGroup`.
    fn local_function_delegate(
        &mut self,
        inst: NodeId,
        this_arg: NodeId,
        operand: &MethodOperand,
        method: &Method,
        delegate_type: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let reference = self.local_function_reference(operand, method, false)?;
        // `CanUseDelegateConstruction` of a static method.
        let invoke = self
            .ts
            .delegate_invoke(delegate_type)
            .ok_or("a delegate type without Invoke")?;
        if invoke.parameters.len() != method.parameters.len()
            || !matches!(self.f.inst(this_arg), Inst::LdNull)
        {
            return Err("a delegate construction that binds its first argument".to_owned());
        }
        let e = self.ast.add(AstNode::DelegateCreate {
            ty: delegate_type,
            reference,
            method: operand.clone(),
            is_virtual: false,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(delegate_type));
        Ok(TranslatedExpression {
            expression: e,
            ty: delegate_type,
        })
    }

    /// `BuildDelegateReference` with `Disambiguator.ForMethodReference`: `target.Name` or
    /// `Name`, with type arguments where they are needed, written once the method group of the
    /// name, resolved with the parameter types as arguments, resolves back to the method.
    /// `rr_replaced`: the caller replaces the reference's resolve result.
    fn method_reference(
        &mut self,
        operand: &MethodOperand,
        method: &Method,
        op: CallOp,
        mut target: TranslatedExpression,
        declaring: TypeId,
        rr_replaced: bool,
    ) -> Result<AstId, Unsupported> {
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let declaring_def = self.ts.definition(declaring);
        let mut require = self.requires_qualifier_dispatching(
            &method.name,
            method.is_static,
            declaring_def,
            target.expression,
            op != CallOp::CallVirt && method.is_overridable,
        );
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
        // `MethodReferenceSteps`: type arguments, then the target, then its cast.
        let mut type_arguments = false;
        let mut spent_type_arguments = method.type_parameters.is_empty();
        let mut spent_require = require;
        let mut spent_cast = false;
        let found = loop {
            let written = if type_arguments {
                method.method_args.clone().unwrap_or_default()
            } else {
                Vec::new()
            };
            let probe_target = require.then_some(lookup_target);
            let group =
                resolver.method_group_reference(self.ts, &method.name, probe_target, &written);
            if let Some(group) = group
                && let Some(found) = self.probe_method_reference(
                    &group,
                    probe_target.map(|_| target.ty),
                    method,
                    op,
                    &written,
                )?
            {
                break Some(found);
            }
            if !spent_type_arguments {
                spent_type_arguments = true;
                type_arguments = true;
                continue;
            }
            if !spent_require {
                spent_require = true;
                require = true;
                continue;
            }
            if !spent_cast {
                spent_cast = true;
                // `Target.ConvertTo(declaringType)`
                lookup_target =
                    self.cast_target_to_with(&mut target, declaring, op == CallOp::CallVirt)?;
                continue;
            }
            // No spelling resolves; the most explicit one is written where the caller replaces
            // its resolve result (`BuildMethodReference`); with the resolve result of its last
            // probe it is not translated.
            if rr_replaced {
                break None;
            }
            return Err("a method reference that needs a cast of its target".to_owned());
        };
        let written = if type_arguments {
            method.method_args.clone().unwrap_or_default()
        } else {
            Vec::new()
        };
        let name = method.name.clone();
        let reference = match (require, written.is_empty()) {
            (true, true) => self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name,
            }),
            (true, false) => self.ast.add(AstNode::GenericMemberReference {
                target: target.expression,
                name,
                type_arguments: written.clone(),
            }),
            (false, true) => self.ast.add(AstNode::Identifier { name }),
            (false, false) => self.ast.add(AstNode::GenericIdentifier {
                name,
                type_arguments: written.clone(),
            }),
        };
        let Some(found) = found else {
            return Ok(reference);
        };
        let target_rr = require.then(|| Box::new(self.rr_of(target.expression)));
        let index = self.ast.method_groups.len();
        self.ast.method_groups.push(found);
        self.ast.with_rr(
            reference,
            Rr::MethodGroup {
                index,
                target: target_rr,
                method: Box::new(operand.clone()),
                type_arguments: written,
                extension: false,
            },
        );
        Ok(reference)
    }

    /// `DisambiguateDelegateReference` of an extension method bound to its first argument:
    /// `target.Name`, with `Disambiguator.ForMethodReference` over `ExtensionMethodReferenceSteps`
    /// (the target cast to the receiver's type, then the type arguments); a null target is cast
    /// up front.
    fn extension_method_reference(
        &mut self,
        this_arg: NodeId,
        operand: &MethodOperand,
        method: &Method,
        op: CallOp,
    ) -> Result<AstId, Unsupported> {
        let mut target_type = method.parameters[0].ty;
        let mut this_arg = this_arg;
        if let crate::typesystem::types::TypeData::ByRef(element) = *self.ts.data(target_type)
            && let Inst::Box { arg, .. } = *self.f.inst(this_arg)
        {
            target_type = element;
            this_arg = arg;
        }
        let mut target = self.translate(this_arg, Some(target_type))?;
        let mut spent_cast = false;
        if matches!(self.f.inst(this_arg), Inst::LdNull) {
            spent_cast = true;
            target = self.convert_to(target, target_type, false)?;
        }
        let mut type_arguments = false;
        let mut spent_type_arguments = false;
        let found = loop {
            let written = if type_arguments {
                method.method_args.clone().unwrap_or_default()
            } else {
                Vec::new()
            };
            if let Some(group) =
                self.probe_extension_method_reference(&target, method, op, &written)?
            {
                break group;
            }
            if !spent_cast {
                spent_cast = true;
                target = self.convert_to(target, target_type, false)?;
                continue;
            }
            if !spent_type_arguments {
                spent_type_arguments = true;
                type_arguments = true;
                continue;
            }
            // shortcut: no spelling resolves; the managed reference then carries the resolve
            // result of its last probe, which may be none. Upgrade when a fixture needs it.
            return Err("an extension method reference that does not resolve".to_owned());
        };
        let written = found.type_arguments.clone();
        let name = method.name.clone();
        let reference = if written.is_empty() {
            self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name,
            })
        } else {
            self.ast.add(AstNode::GenericMemberReference {
                target: target.expression,
                name,
                type_arguments: written.clone(),
            })
        };
        let target_rr = Box::new(self.rr_of(target.expression));
        let index = self.ast.method_groups.len();
        self.ast.method_groups.push(found);
        self.ast.with_rr(
            reference,
            Rr::MethodGroup {
                index,
                target: Some(target_rr),
                method: Box::new(operand.clone()),
                type_arguments: written,
                extension: true,
            },
        );
        Ok(reference)
    }

    /// `IsUnambiguousMethodReference` with `isExtensionMethodReference`: the member access as
    /// an invocation target, its overload resolution with the parameter types past the
    /// receiver standing in for arguments (extension methods allowed), not ambiguous and with
    /// the method as its best candidate; the method group where it resolves back.
    fn probe_extension_method_reference(
        &mut self,
        target: &TranslatedExpression,
        method: &Method,
        op: CallOp,
        type_arguments: &[TypeId],
    ) -> Result<Option<Rc<MethodGroup>>, Unsupported> {
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
        let Some(answer) =
            resolver.extension_method_group(self.ts, &method.name, lookup_target, type_arguments)
        else {
            return Ok(None);
        };
        let method_lists = self.method_lists(&answer.methods, Some(target.ty));
        let mut extension_methods = Vec::with_capacity(answer.extension_methods.len());
        for group in &answer.extension_methods {
            let mut methods = Vec::with_capacity(group.len());
            for &id in group {
                let definition = self.ts.method_definition(id).map_err(|e| e.to_string())?;
                // Explicit type arguments specialize the candidates (`GetExtensionMethods`).
                let resolved = if type_arguments.is_empty() {
                    definition
                } else {
                    self.ts
                        .specialize(definition, None, Some(type_arguments.to_vec()))
                };
                methods.push(
                    Method::from_resolved(self.ts, &resolved)
                        .ok_or("an extension method without definition")?,
                );
            }
            extension_methods.push(methods);
        }
        let group = MethodGroup {
            target: self.semantic_rr(target.expression, target.ty),
            name: method.name.clone(),
            method_lists,
            type_arguments: type_arguments.to_vec(),
            extension_methods,
        };
        let arguments: Vec<ResolveResult> = method.parameters[1..]
            .iter()
            .map(|p| ResolveResult::Type(p.ty))
            .collect();
        let mut conversions = Conversions::new(self.ts);
        let or = conversions.perform_overload_resolution(
            &group,
            arguments,
            crate::semantics::overload_resolution::OverloadOptions::default(),
        );
        if or.is_ambiguous() {
            return Ok(None);
        }
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return Ok(None);
        };
        if !self.is_appropriate_call_target(op, method, &found)? {
            return Ok(None);
        }
        Ok(Some(Rc::new(group)))
    }

    /// `IsUnambiguousMethodReference`: overload resolution over the method group with the
    /// parameter types standing in for arguments, the best candidate compared with the method
    /// (no `CheckBestCandidate`); the method group as the engine's resolve result where it
    /// resolves back.
    fn probe_method_reference(
        &mut self,
        group: &[crate::il::transforms::MethodGroupList],
        lookup_type: Option<TypeId>,
        method: &Method,
        op: CallOp,
        type_arguments: &[TypeId],
    ) -> Result<Option<Rc<MethodGroup>>, Unsupported> {
        let lists = self.method_lists(group, lookup_type);
        let arguments: Vec<ResolveResult> = method
            .parameters
            .iter()
            .map(|p| ResolveResult::Type(p.ty))
            .collect();
        let mut conversions = Conversions::new(self.ts);
        let mut or = OverloadResolution::new(arguments, None, type_arguments.to_vec());
        or.add_method_lists(&mut conversions, &lists);
        let Some(found) = or.best_candidate_with_substituted_type_arguments(self.ts) else {
            return Ok(None);
        };
        if !self.is_appropriate_call_target(op, method, &found)? {
            return Ok(None);
        }
        let target = match lookup_type {
            Some(t) => ResolveResult::Expression(t),
            None => ResolveResult::Expression(
                self.ts
                    .intern(crate::typesystem::types::TypeData::UnknownSpecial),
            ),
        };
        Ok(Some(Rc::new(MethodGroup {
            target,
            name: method.name.clone(),
            method_lists: lists,
            type_arguments: type_arguments.to_vec(),
            extension_methods: Vec::new(),
        })))
    }
}
