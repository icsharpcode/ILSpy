//! The member accesses of `ExpressionBuilder`: `VisitLdObj`, `VisitLdFlda`, `VisitLdsFlda`,
//! `VisitStObj`, `VisitStLoc`, `ConvertField`, `TranslateTarget`, `RequiresQualifier` and
//! the field case of `Disambiguator`.

use crate::il::ast::{FieldOperand, Inst, NodeId, VarId, VariableKind};
use crate::il::transforms::LookupTarget;
use crate::il::types::StackType;
use crate::metadata::tables::rows;
use crate::semantics::types::TypeKind;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::resolve::FieldOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `FieldAttributes.Static`.
const FIELD_STATIC: u16 = 0x0010;

impl Builder<'_, '_, '_, '_> {
    /// `ResolveResult.Type` of a resolve result the builders made.
    pub(crate) fn rr_type(&mut self, rr: &Rr) -> TypeId {
        match rr {
            Rr::Constant { ty, .. }
            | Rr::Variable { ty, .. }
            | Rr::This { ty, .. }
            | Rr::Type(ty)
            | Rr::Assign { ty } => *ty,
            Rr::Field { field, .. } => field.field.ty,
            Rr::ByReference(element) => {
                let e = self.rr_type(element);
                self.ts.intern(TypeData::ByRef(e))
            }
        }
    }

    /// `TranslatedExpression.UnwrapChild(child)`: the child with its own resolve result.
    fn unwrap_child(&mut self, child: AstId) -> TranslatedExpression {
        let rr = self
            .ast
            .annotations(child)
            .rr
            .clone()
            .expect("a translated expression has a resolve result");
        TranslatedExpression {
            expression: child,
            ty: self.rr_type(&rr),
        }
    }

    fn rr_of(&self, e: AstId) -> Rr {
        self.ast
            .annotations(e)
            .rr
            .clone()
            .expect("a translated expression has a resolve result")
    }

    /// The pointer or reference a memory access goes through: its element type.
    fn pointer_element(&self, t: TypeId) -> Option<TypeId> {
        match *self.ts.data(t) {
            TypeData::Pointer(e) | TypeData::ByRef(e) => Some(e),
            _ => None,
        }
    }

    /// `TypeUtils.IsCompatiblePointerTypeForMemoryAccess`.
    fn is_compatible_pointer_type_for_memory_access(
        &mut self,
        pointer: TypeId,
        access: TypeId,
    ) -> bool {
        match self.pointer_element(pointer) {
            Some(memory) => self.ts.is_compatible_type_for_memory_access(memory, access),
            None => false,
        }
    }

    /// `VisitLdObj` through a managed reference.
    pub(crate) fn ld_obj(
        &mut self,
        inst: NodeId,
        target: NodeId,
        ty: TypeId,
        unaligned: u8,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if unaligned != 0 {
            return Err("an unaligned load".to_owned());
        }
        let target_is_ref = self.f.result_type(self.ts, target) == StackType::Ref;
        let mut load_type = ty;
        let used_in_generic = target_is_ref;
        if self.ts.kind(hint) != TypeKind::Unknown
            && self
                .ts
                .is_compatible_type_for_memory_access(hint, load_type)
            && !(used_in_generic && self.ts.kind(hint).is_any_pointer())
        {
            load_type = hint;
        }
        if !target_is_ref {
            return Err("a load through a pointer".to_owned());
        }
        let address_hint = self.ts.intern(TypeData::ByRef(load_type));
        let address = self.translate(target, Some(address_hint))?;
        if !self.is_compatible_pointer_type_for_memory_access(address.ty, load_type) {
            return Err("a load that needs a pointer cast".to_owned());
        }
        let AstNode::RefDirection { expression } = *self.ast.node(address.expression) else {
            return Err("a load through a reference that is no ref expression".to_owned());
        };
        let result = self.unwrap_child(expression);
        self.ast.with_il(result.expression, inst);
        Ok(result)
    }

    /// `VisitLdFlda` (with a target) and `VisitLdsFlda` producing a managed reference:
    /// `ref target.field`.
    pub(crate) fn ld_flda(
        &mut self,
        inst: NodeId,
        target: Option<NodeId>,
        field: &FieldOperand,
    ) -> Result<TranslatedExpression, Unsupported> {
        if field.field.name == "FixedElementField" {
            return Err("a fixed buffer element".to_owned());
        }
        if target.is_some() && self.is_value_tuple(field.field.declaring) {
            return Err("a tuple element access".to_owned());
        }
        if self.f.result_type(self.ts, inst) == StackType::I {
            return Err("a field address as a native pointer".to_owned());
        }
        let e = self.convert_field(field, target)?;
        self.ast.with_il(e.expression, inst);
        let element = self.rr_of(e.expression);
        let d = self.ast.add(AstNode::RefDirection {
            expression: e.expression,
        });
        self.ast.with_rr(d, Rr::ByReference(Box::new(element)));
        let ty = self.ts.intern(TypeData::ByRef(e.ty));
        Ok(TranslatedExpression { expression: d, ty })
    }

    fn is_value_tuple(&self, t: Option<TypeId>) -> bool {
        t.and_then(|t| self.ts.definition(t))
            .and_then(|d| self.ts.comp.type_def(d).ok())
            .is_some_and(|i| i.full_name.top.ns == "System" && i.full_name.name() == "ValueTuple")
    }

    /// `ConvertField(field, targetInstruction)` for a field that is no backing field.
    fn convert_field(
        &mut self,
        field: &FieldOperand,
        target_inst: Option<NodeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let name = field.field.name.clone();
        // Backing fields of automatic properties and events, and the C# 14 `field` keyword,
        // depend on how the declarations around the body are decompiled.
        if name.starts_with('<') || name == "field" {
            return Err("a backing field".to_owned());
        }
        let FieldOrigin::Def { module, row } = field.field.origin else {
            return Err("an unresolved field".to_owned());
        };
        let declaring = field
            .field
            .declaring
            .ok_or("a field without declaring type")?;
        let declaring_def = self.ts.definition(declaring);
        if let Some(def) = declaring_def {
            if self.has_event_named(def, &name) {
                return Err("the backing field of an event".to_owned());
            }
            // The VB backing field `_Name` of a property `Name`.
            if let Some(property) = name.strip_prefix('_')
                && self.has_property_named(def, property)
            {
                return Err("a backing field".to_owned());
            }
        }
        let is_static = self
            .ts
            .comp
            .module(module)
            .md
            .get::<rows::Field>(row)
            .map_err(|e| e.to_string())?
            .flags
            & FIELD_STATIC
            != 0;
        if matches!(self.ts.data(field.field.ty), TypeData::ByRef(_)) {
            return Err("a ref field".to_owned());
        }
        let target = self.translate_target(target_inst, true, is_static, declaring)?;
        let require = self.requires_qualifier(&name, is_static, declaring_def, target.expression);
        let require = self.disambiguate_field(field, &target, require, declaring)?;
        let target_rr = self.rr_of(target.expression);
        let mrr = Rr::Field {
            target: Box::new(target_rr),
            field: field.clone(),
        };
        let e = if require {
            self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name,
            })
        } else {
            self.ast.add(AstNode::Identifier { name })
        };
        self.ast.with_rr(e, mrr);
        Ok(TranslatedExpression {
            expression: e,
            ty: field.field.ty,
        })
    }

    fn has_event_named(&self, def: TypeDefId, name: &str) -> bool {
        let md = &self.ts.comp.module(def.module).md;
        self.ts
            .comp
            .visible_events(def)
            .unwrap_or_default()
            .into_iter()
            .any(|row| {
                md.get::<rows::Event>(row)
                    .ok()
                    .and_then(|e| md.string(e.name).ok())
                    .is_some_and(|n| n == name)
            })
    }

    fn has_property_named(&self, def: TypeDefId, name: &str) -> bool {
        let md = &self.ts.comp.module(def.module).md;
        self.ts
            .comp
            .visible_properties(def)
            .unwrap_or_default()
            .into_iter()
            .any(|row| {
                md.get::<rows::Property>(row)
                    .ok()
                    .and_then(|p| md.string(p.name).ok())
                    .is_some_and(|n| n == name)
            })
    }

    /// `inst.MatchLdThis()`: the `this` parameter it loads.
    fn match_ld_this(&self, inst: NodeId) -> Option<VarId> {
        let Inst::LdLoc { var } = *self.f.inst(inst) else {
            return None;
        };
        let v = self.f.var(var);
        (v.kind == VariableKind::Parameter && v.index == Some(-1)).then_some(var)
    }

    /// `TranslateTarget(target, nonVirtualInvocation, memberStatic, memberDeclaringType)` for
    /// a reference-type target, `base`, or a static member's type.
    fn translate_target(
        &mut self,
        target: Option<NodeId>,
        non_virtual_invocation: bool,
        member_static: bool,
        declaring: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some(target) = target.filter(|_| !member_static) else {
            let e = self.ast.add(AstNode::TypeReference { ty: declaring });
            self.ast.with_rr(e, Rr::Type(declaring));
            return Ok(TranslatedExpression {
                expression: e,
                ty: declaring,
            });
        };
        let current_is_struct = self
            .current_type
            .and_then(|d| self.ts.comp.type_def(d).ok())
            .is_some_and(|i| i.kind == crate::typesystem::compilation::TypeKind::Struct);
        if current_is_struct && matches!(self.f.inst(target), Inst::Box { .. }) {
            return Err("a member access on a boxed this".to_owned());
        }
        if non_virtual_invocation
            && let Some(this_var) = self.match_ld_this(target)
            && self.ts.definition(declaring) != self.current_type
        {
            // `base`: the current type's first non-interface base type.
            let base_type = match self.current_type {
                Some(def) => self
                    .ts
                    .direct_base_types(def)
                    .unwrap_or_default()
                    .into_iter()
                    .find(|&t| !self.ts.is_interface(t)),
                None => None,
            }
            .unwrap_or(declaring);
            let e = self.ast.add(AstNode::Base);
            self.ast.with_il(e, target);
            self.ast.with_rr(
                e,
                Rr::This {
                    var: this_var,
                    ty: base_type,
                    causes_non_virtual_invocation: non_virtual_invocation,
                },
            );
            return Ok(TranslatedExpression {
                expression: e,
                ty: base_type,
            });
        }
        if matches!(self.f.inst(target), Inst::Conv { .. }) {
            return Err("a converted member target".to_owned());
        }
        // `CallInstruction.ExpectedTypeForThisPointer`
        let expected = match self.ts.is_reference_type(declaring) {
            _ if matches!(self.ts.data(declaring), TypeData::TypeParam { .. }) => StackType::Ref,
            Some(true) => StackType::Obj,
            Some(false) => StackType::Ref,
            None => StackType::Unknown,
        };
        let target_result = self.f.result_type(self.ts, target);
        if expected == StackType::Ref
            || (expected == StackType::Unknown && target_result == StackType::Ref)
        {
            return Err("a member access on a value type".to_owned());
        }
        let translated = self.translate(target, Some(declaring))?;
        if matches!(
            self.ast.node(translated.expression),
            AstNode::RefDirection { .. }
        ) {
            return Err("a member access on a ref expression".to_owned());
        }
        if matches!(self.ast.node(translated.expression), AstNode::This)
            && let Some(Rr::Variable { var, .. }) = self.ast.annotations(translated.expression).rr
        {
            // An explicit `this` gets the resolve result `base` gets.
            let e = self.ast.add(AstNode::This);
            self.ast.with_il(e, target);
            self.ast.with_rr(
                e,
                Rr::This {
                    var,
                    ty: translated.ty,
                    causes_non_virtual_invocation: non_virtual_invocation,
                },
            );
            return Ok(TranslatedExpression {
                expression: e,
                ty: translated.ty,
            });
        }
        Ok(translated)
    }

    /// `RequiresQualifier(member, target)` for a field.
    fn requires_qualifier(
        &self,
        name: &str,
        is_static: bool,
        declaring: Option<TypeDefId>,
        target: AstId,
    ) -> bool {
        if self.settings.always_qualify_member_references
            || self.variable_names.iter().any(|n| n == name)
        {
            return true;
        }
        if is_static {
            return !self.is_current_or_containing_type(declaring);
        }
        match self.ast.node(target) {
            // A field is not overridable.
            AstNode::Base => false,
            AstNode::This => false,
            _ => true,
        }
    }

    /// `IsCurrentOrContainingType`.
    fn is_current_or_containing_type(&self, def: Option<TypeDefId>) -> bool {
        let mut current = self.current_type;
        while let Some(c) = current {
            if Some(c) == def {
                return true;
            }
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
        false
    }

    /// `Disambiguator.ForField`: whether the field is written with its target, after asking
    /// the host's resolver whether each spelling binds back to the field (unqualified, then
    /// qualified, then with the target cast to the declaring type, which is only translated
    /// when the cast is an identity).
    fn disambiguate_field(
        &mut self,
        field: &FieldOperand,
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
            if resolver.field_reference_binds(self.ts, field, probe_target) {
                return Ok(require);
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

    /// `ExpressionBuilder.Assignment(left, right)`.
    fn assignment(
        &mut self,
        left: TranslatedExpression,
        right: TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        let right = self.convert_to(right, left.ty, true)?;
        let e = self.ast.add(AstNode::Assignment {
            left: left.expression,
            right: right.expression,
        });
        self.ast.with_rr(e, Rr::Assign { ty: left.ty });
        Ok(TranslatedExpression {
            expression: e,
            ty: left.ty,
        })
    }

    /// `VisitStObj` through a managed reference.
    pub(crate) fn st_obj(
        &mut self,
        inst: NodeId,
        target: NodeId,
        value: NodeId,
        ty: TypeId,
        unaligned: u8,
    ) -> Result<TranslatedExpression, Unsupported> {
        if unaligned != 0 || self.f.result_type(self.ts, target) != StackType::Ref {
            return Err("a store that is no assignment through a reference".to_owned());
        }
        let hint = self.ts.intern(TypeData::ByRef(ty));
        let pointer = self.translate(target, Some(hint))?;
        if !self.is_compatible_pointer_type_for_memory_access(pointer.ty, ty) {
            return Err("a store that needs a pointer cast".to_owned());
        }
        let AstNode::RefDirection { expression } = *self.ast.node(pointer.expression) else {
            return Err("a store through a reference that is no ref expression".to_owned());
        };
        let target = self.unwrap_child(expression);
        let value = self.translate(value, Some(target.ty))?;
        if matches!(
            self.ast.node(target.expression),
            AstNode::RefDirection { .. }
        ) {
            return Err("a ref reassignment".to_owned());
        }
        let e = self.assignment(target, value)?;
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }

    /// `VisitStLoc` for a variable that is no stack slot.
    pub(crate) fn st_loc(
        &mut self,
        inst: NodeId,
        var: VarId,
        value: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let var_type = self.f.var(var).ty;
        let value = self.translate(value, Some(var_type))?;
        if self.f.var(var).kind == VariableKind::StackSlot {
            return Err("a store to a stack slot".to_owned());
        }
        let left = self.convert_variable(var)?;
        let e = self.assignment(left, value)?;
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }
}
