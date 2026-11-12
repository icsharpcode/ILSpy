//! `CallBuilder.HandleDelegateConstruction`: `new D(target.Method)` for the construction of a
//! delegate from a function pointer, with the method group the reference resolves to.

use std::rc::Rc;

use crate::il::ast::{CallData, Inst, MethodOperand, NodeId};
use crate::il::transforms::LookupTarget;
use crate::semantics::{Conversions, Method, MethodGroup, OverloadResolution, ResolveResult};
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::TypeId;

use super::ast::{AstId, AstNode, Rr};
use super::call_builder::CallOp;
use super::{Builder, TranslatedExpression, Unsupported};

impl Builder<'_, '_, '_, '_> {
    /// `HandleDelegateConstruction(CallInstruction)` where `CanUseDelegateConstruction` holds
    /// for an instance method, or a static method of a delegate without a bound first
    /// argument. Extension methods, local functions, accessors and value-type targets are not
    /// translated.
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
        let MethodOrigin::Def(id) = operand.method.origin else {
            return Err("a delegate of an unresolved method".to_owned());
        };
        let method = Method::from_resolved(self.ts, &operand.method)
            .ok_or("a delegate of an unresolved method")?;
        let declaring = method.declaring.ok_or("a method without declaring type")?;
        let flags = self.ts.method_flags(id, None);
        if flags.extension
            || self.ts.accessor_owner(id).is_some()
            || operand.method.name.starts_with('<')
        {
            return Err("a delegate of an extension method, accessor or local function".to_owned());
        }
        // `CanUseDelegateConstruction`
        let invoke = self
            .ts
            .delegate_invoke(delegate_type)
            .ok_or("a delegate type without Invoke")?;
        if invoke.parameters.len() != method.parameters.len()
            || (method.is_static && !matches!(self.f.inst(this_arg), Inst::LdNull))
        {
            return Err("a delegate construction that binds its first argument".to_owned());
        }
        if !method.is_static && self.ts.is_reference_type(declaring) != Some(true) {
            return Err("a delegate of a value type's method".to_owned());
        }
        let target = self.translate_target(
            (!method.is_static).then_some(this_arg),
            op == CallOp::Call,
            method.is_static,
            declaring,
        )?;
        let reference = self.method_reference(&operand, &method, op, target, declaring)?;
        let e = self.ast.add(AstNode::DelegateCreate {
            ty: delegate_type,
            reference,
            method: operand,
            is_virtual: op == CallOp::CallVirt,
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
    fn method_reference(
        &mut self,
        operand: &MethodOperand,
        method: &Method,
        op: CallOp,
        target: TranslatedExpression,
        declaring: TypeId,
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
        // `MethodReferenceSteps`: type arguments, then the target; a cast of the target is not
        // translated.
        let mut type_arguments = false;
        let mut spent_type_arguments = method.type_parameters.is_empty();
        let mut spent_require = require;
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
                break found;
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
            },
        );
        Ok(reference)
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
