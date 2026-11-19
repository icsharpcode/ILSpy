//! The member accesses of `ExpressionBuilder`: `VisitLdObj`, `VisitLdFlda`, `VisitLdsFlda`,
//! `VisitStObj`, `VisitStLoc`, `ConvertField`, `TranslateTarget`, `RequiresQualifier` and
//! the field case of `Disambiguator`.

use crate::il::ast::{FieldOperand, Inst, NodeId, VarId, VariableKind};
use crate::il::transforms::{FieldKind, LookupTarget};
use crate::il::types::StackType;
use crate::metadata::tables::rows;
use crate::semantics::types::TypeKind;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::resolve::FieldOrigin;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// `FieldAttributes.Static`.
const FIELD_STATIC: u16 = 0x0010;

/// `MethodAttributes.Static`.
const METHOD_STATIC: u16 = 0x0010;

impl Builder<'_, '_, '_, '_> {
    /// `ResolveResult.Type` of a resolve result the builders made.
    pub(crate) fn rr_type(&mut self, rr: &Rr) -> TypeId {
        match rr {
            Rr::Constant { ty, .. }
            | Rr::Variable { ty, .. }
            | Rr::This { ty, .. }
            | Rr::Type(ty)
            | Rr::Operator { ty }
            | Rr::ExplicitOperator { ty, .. }
            | Rr::OperatorInvocation { ty, .. }
            | Rr::Invocation { ty, .. }
            | Rr::Property { ty, .. }
            | Rr::ObjectCreation { ty, .. }
            | Rr::Plain(ty)
            | Rr::Conversion { ty }
            | Rr::ImplicitConversion { ty }
            | Rr::LambdaConversion { ty }
            | Rr::InitializedObject(ty)
            | Rr::ArrayCreate { ty }
            | Rr::ResolvedOperator { ty }
            | Rr::TypeOf { ty, .. }
            | Rr::ArrayLength { ty, .. }
            | Rr::TupleElement { ty, .. }
            | Rr::TryCast { ty }
            | Rr::FormatString { ty, .. }
            | Rr::MethodMember { ty, .. } => *ty,
            Rr::Field { field, .. } => field.field.ty,
            Rr::Event { ty, .. } => *ty,
            Rr::Lambda(_)
            | Rr::DefaultLiteral { .. }
            | Rr::MethodGroup { .. }
            | Rr::LocalFunctionGroup { .. }
            | Rr::Throw => self.ts.intern(TypeData::NoType),
            Rr::TypeIs { .. } => self
                .ts
                .known_type(crate::typesystem::known::KnownTypeCode::Boolean),
            Rr::ByReference(element)
            | Rr::ByReferenceOut(element)
            | Rr::ByReferenceIn(element, _) => {
                let e = self.rr_type(element);
                self.ts.intern(TypeData::ByRef(e))
            }
        }
    }

    /// `TranslatedExpression.UnwrapChild(child)`: the child of `parent` with its own resolve
    /// result, given the parent's IL instructions.
    pub(crate) fn unwrap_child(&mut self, parent: AstId, child: AstId) -> TranslatedExpression {
        for inst in self.ast.annotations(parent).il.clone() {
            self.ast.with_il(child, inst);
        }
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

    pub(crate) fn rr_of(&self, e: AstId) -> Rr {
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
    pub(crate) fn is_compatible_pointer_type_for_memory_access(
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
        let address_hint = if target_is_ref {
            self.ts.intern(TypeData::ByRef(load_type))
        } else {
            self.ts.intern(TypeData::Pointer(load_type))
        };
        let address = self.translate(target, Some(address_hint))?;
        let result = self.ld_obj_through(address, load_type)?;
        self.ast.with_il(result.expression, inst);
        Ok(result)
    }

    /// `LdObj(address, loadType)` on the translated address: the reference without its `ref`,
    /// the pointer without its `&` or dereferenced, cast to a pointer of the loaded type where
    /// it is of another, or `Unsafe.Read<T>` for a managed type behind a pointer.
    pub(crate) fn ld_obj_through(
        &mut self,
        address: TranslatedExpression,
        load_type: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        if self.is_compatible_pointer_type_for_memory_access(address.ty, load_type) {
            return match *self.ast.node(address.expression) {
                AstNode::RefDirection { expression } => {
                    Ok(self.unwrap_child(address.expression, expression))
                }
                _ => match *self.ts.data(address.ty) {
                    TypeData::Pointer(element) => match *self.ast.node(address.expression) {
                        AstNode::PointerUnary {
                            address_of: true,
                            expression,
                        } => Ok(self.unwrap_child(address.expression, expression)),
                        _ => Ok(self.dereference(address.expression, element)),
                    },
                    _ => Err("a load through a reference that is no ref expression".to_owned()),
                },
            };
        }
        if matches!(
            self.ast.node(address.expression),
            AstNode::RefDirection { .. }
        ) {
            return Err("a load that needs a pointer cast".to_owned());
        }
        if !self.is_unmanaged_type(load_type) {
            // `Unsafe.Read<T>(void*)`
            let void = self
                .ts
                .known_type(crate::typesystem::known::KnownTypeCode::Void);
            let void_pointer = self.ts.intern(TypeData::Pointer(void));
            let pointer = self.convert_to(address, void_pointer, true)?;
            return Ok(self.call_unsafe_intrinsic(
                "Read",
                vec![pointer.expression],
                load_type,
                None,
                vec![load_type],
            ));
        }
        let pointer_type = self.ts.intern(TypeData::Pointer(load_type));
        let address = self.convert_to(address, pointer_type, false)?;
        Ok(match *self.ast.node(address.expression) {
            AstNode::RefDirection { expression } => {
                self.unwrap_child(address.expression, expression)
            }
            _ => self.dereference(address.expression, load_type),
        })
    }

    /// `VisitLdFlda` (with a target) and `VisitLdsFlda` producing a managed reference:
    /// `ref target.field`.
    pub(crate) fn ld_flda(
        &mut self,
        inst: NodeId,
        target: Option<NodeId>,
        field: &FieldOperand,
    ) -> Result<TranslatedExpression, Unsupported> {
        if self.settings.fixed_buffers
            && field.field.name == "FixedElementField"
            && let Some(t) = target
            && let Inst::LdFlda {
                target: buffer_target,
                field: buffer,
                ..
            } = self.f.inst(t).clone()
            && let Some(element) = self.fixed_field_element(&buffer)
        {
            return self.fixed_buffer_element(inst, buffer_target, &buffer, element);
        }
        let e = match target {
            Some(t) if self.is_value_tuple(field.field.declaring) => {
                self.tuple_field_access(field, t)?
            }
            _ => self.convert_field(field, target)?,
        };
        self.ast.with_il(e.expression, inst);
        if target.is_some() && self.f.result_type(self.ts, inst) == StackType::I {
            // `ldflda` producing a native pointer: `&target.field`.
            let a = self.address_of_node(e.expression);
            let ty = self.ts.intern(TypeData::Pointer(e.ty));
            self.ast.with_rr(a, Rr::Plain(ty));
            return Ok(TranslatedExpression { expression: a, ty });
        }
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

    /// The tuple element `MatchTupleFieldAccess` finds for a field of a `ValueTuple`: the
    /// element's name on a target of a matching tuple type, else `ConvertField`. Elements past
    /// the seventh (reached through `Rest`) and targets whose type is no tuple type are not
    /// translated.
    fn tuple_field_access(
        &mut self,
        field: &FieldOperand,
        target: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some(position) = field
            .field
            .name
            .strip_prefix("Item")
            .and_then(|p| p.parse::<usize>().ok())
        else {
            return self.convert_field(field, Some(target));
        };
        if let Inst::LdFlda { field: rest, .. } = self.f.inst(target)
            && rest.field.name == "Rest"
        {
            return Err("a tuple element past the seventh".to_owned());
        }
        let underlying = field
            .field
            .declaring
            .ok_or("a tuple field without declaring type")?;
        let translated = self.translate_target(Some(target), true, false, underlying)?;
        // The managed type system reads every `ValueTuple` as a tuple type, with the element
        // names of `[TupleElementNames]`. The engine keeps the names of a variable's type and of
        // a field of a non-generic type; a member of a generic type is substituted from the IL
        // reference, without the names the target's type gives its type arguments, and is not
        // translated.
        let mut translated = translated;
        let names_known = match self.rr_of(translated.expression) {
            Rr::Variable { .. } => true,
            Rr::Field { field, .. } => field
                .field
                .declaring
                .is_some_and(|d| !matches!(self.ts.data(d), TypeData::Inst { .. })),
            // A non-generic method of a non-generic type returns the type of its signature,
            // with the names of its return's `[TupleElementNames]`.
            Rr::Invocation { method, .. }
            | Rr::Property {
                accessor: method, ..
            } if method
                .method
                .declaring
                .is_some_and(|d| !matches!(self.ts.data(d), TypeData::Inst { .. })) =>
            {
                let generic = crate::semantics::Method::from_resolved(self.ts, &method.method)
                    .is_none_or(|m| m.method_args.is_some_and(|a| !a.is_empty()));
                // The type arguments the overload resolution inferred carry the names of the
                // arguments' types, which the engine may have lost: names it found are the
                // resolver's, a tuple without names may lack them.
                !generic || self.tuple_has_names(translated.ty)
            }
            // A member of a generic type takes the type arguments, with their names, of the
            // target's type, as the resolver substitutes it: exact for a variable, and for
            // other targets where names were found.
            Rr::Invocation { method, target, .. }
            | Rr::Property {
                accessor: method,
                target,
                ..
            } => match self.member_type_of_target(&method.method, &target) {
                Some(ty) if matches!(*target, Rr::Variable { .. }) || self.tuple_has_names(ty) => {
                    translated.ty = ty;
                    true
                }
                _ => false,
            },
            _ => false,
        };
        if !names_known || !matches!(self.ts.data(translated.ty), TypeData::Tuple { .. }) {
            return Err("a tuple element whose names the engine may not know".to_owned());
        }
        let n = Normalization::TYPE_ERASURE;
        let element = match self.ts.data(translated.ty) {
            TypeData::Tuple {
                elements, names, ..
            } if position >= 1 && position <= names.len() => Some((
                elements[position - 1],
                names[position - 1].as_deref().map(str::to_owned),
            )),
            _ => None,
        };
        let Some((ty, name)) = element.filter(|_| {
            self.ts.normalize_with(translated.ty, n) == self.ts.normalize_with(underlying, n)
        }) else {
            return self.convert_field(field, Some(target));
        };
        let name = name.unwrap_or_else(|| format!("Item{position}"));
        let target_rr = self.rr_of(translated.expression);
        let e = self.ast.add(AstNode::MemberReference {
            target: translated.expression,
            name,
        });
        self.ast.with_rr(
            e,
            Rr::TupleElement {
                target: Box::new(target_rr),
                field: field.clone(),
                ty,
            },
        );
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// Whether `ty` is a tuple type with at least one element name.
    fn tuple_has_names(&self, ty: TypeId) -> bool {
        matches!(self.ts.data(ty), TypeData::Tuple { names, .. } if names.iter().any(Option::is_some))
    }

    /// The return type of `method`, a member of a generic type, specialized with the type
    /// arguments of `target`'s type where that is the same type up to tuple names and
    /// nullability; `None` otherwise.
    fn member_type_of_target(
        &mut self,
        method: &crate::typesystem::resolve::ResolvedMethod,
        target: &Rr,
    ) -> Option<TypeId> {
        let declaring = method.declaring?;
        let target_ty = self.rr_type(target);
        let n = Normalization::TYPE_ERASURE;
        if self.ts.normalize_with(target_ty, n) != self.ts.normalize_with(declaring, n) {
            return None;
        }
        let TypeData::Inst { args, .. } = self.ts.data(target_ty).clone() else {
            return None;
        };
        let m = crate::semantics::Method::from_resolved(self.ts, method)?;
        let method_args = m.method_args.clone();
        Some(
            m.specialize_definition(self.ts, Some(args.to_vec()), method_args)
                .return_type,
        )
    }

    /// `ConvertField(field, targetInstruction)` for a field that is no backing field.
    pub(crate) fn convert_field(
        &mut self,
        field: &FieldOperand,
        target_inst: Option<NodeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let name = field.field.name.clone();
        let FieldOrigin::Def { module, row } = field.field.origin else {
            return Err("an unresolved field".to_owned());
        };
        let declaring = field
            .field
            .declaring
            .ok_or("a field without declaring type")?;
        let declaring_def = self.ts.definition(declaring);
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
        let resolver = self
            .resolver
            .ok_or("a member reference without the host's resolver")?;
        let target_is_this = match target_inst {
            None => true,
            Some(t) => self.match_ld_this(t).is_some() || self.is_ld_loca_this(t),
        };
        let kind = resolver.field_kind(self.ts, field, target_is_this);
        if let FieldKind::AutoEventBackingField { event_row } = kind {
            return self.event_reference(module, event_row, target_inst, declaring, declaring_def);
        }
        if kind == FieldKind::FieldKeyword {
            // Unqualified: "this.field" would refer to a real member named "field".
            let e = self.ast.add(AstNode::Identifier {
                name: "field".to_owned(),
            });
            self.ast.with_rr(
                e,
                Rr::Field {
                    target: None,
                    field: field.clone(),
                },
            );
            return Ok(TranslatedExpression {
                expression: e,
                ty: field.field.ty,
            });
        }
        if matches!(self.ts.data(field.field.ty), TypeData::ByRef(_)) {
            return Err("a ref field".to_owned());
        }
        let mut target = self.translate_target(target_inst, true, is_static, declaring)?;
        let require = match kind {
            // The backing field of an automatic property needs a qualifier when its property
            // would, because PatternStatementTransform turns the reference into one to the
            // property.
            FieldKind::AutoPropertyBackingField { property_row } => {
                let (property_name, property_static) = self.property_info(module, property_row)?;
                self.requires_qualifier(
                    &property_name,
                    property_static,
                    declaring_def,
                    target.expression,
                )
            }
            // In a C# 14 property accessor a bare "field" binds to the backing field keyword.
            FieldKind::NamedFieldInAccessor => true,
            _ => self.requires_qualifier(&name, is_static, declaring_def, target.expression),
        };
        let require = self.disambiguate_field(field, &mut target, require, declaring)?;
        let target_rr = self.rr_of(target.expression);
        let mrr = Rr::Field {
            target: Some(Box::new(target_rr)),
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

    /// `ldloca this` (a struct method's `this` address).
    fn is_ld_loca_this(&self, inst: NodeId) -> bool {
        let Inst::LdLoca { var } = *self.f.inst(inst) else {
            return false;
        };
        let v = self.f.var(var);
        v.kind == VariableKind::Parameter && v.index == Some(-1)
    }

    /// The name of a Property row and whether its accessors are static.
    pub(crate) fn property_info(
        &self,
        module: crate::typesystem::compilation::ModuleId,
        row: u32,
    ) -> Result<(String, bool), Unsupported> {
        let md = &self.ts.comp.module(module).md;
        let p = md.get::<rows::Property>(row).map_err(|e| e.to_string())?;
        let name = md.string(p.name).map_err(|e| e.to_string())?.into_owned();
        let (get, set, _) = md.property_accessors(row);
        let accessor = if get != 0 { get } else { set };
        let is_static = md
            .get::<rows::MethodDef>(accessor)
            .is_ok_and(|m| m.flags & METHOD_STATIC != 0);
        Ok((name, is_static))
    }

    /// `ConvertField` for the backing field of an automatic event: the event, written with
    /// its target where `RequiresQualifier` asks for it.
    fn event_reference(
        &mut self,
        module: crate::typesystem::compilation::ModuleId,
        event_row: u32,
        target_inst: Option<NodeId>,
        declaring: TypeId,
        declaring_def: Option<TypeDefId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let md = &self.ts.comp.module(module).md;
        let e = md
            .get::<rows::Event>(event_row)
            .map_err(|e| e.to_string())?;
        let name = md.string(e.name).map_err(|e| e.to_string())?.into_owned();
        let (add, remove, _, _) = md.event_accessors(event_row);
        let accessor = if add != 0 { add } else { remove };
        let is_static = md
            .get::<rows::MethodDef>(accessor)
            .is_ok_and(|m| m.flags & METHOD_STATIC != 0);
        let ty = self
            .ts
            .event_type(module, event_row)
            .map_err(|e| e.to_string())?;
        let target = self.translate_target(target_inst, true, is_static, declaring)?;
        let require = self.requires_qualifier(&name, is_static, declaring_def, target.expression);
        let target_rr = self.rr_of(target.expression);
        let reference = if require {
            self.ast.add(AstNode::MemberReference {
                target: target.expression,
                name,
            })
        } else {
            self.ast.add(AstNode::Identifier { name })
        };
        self.ast.with_rr(
            reference,
            Rr::Event {
                target: Box::new(target_rr),
                event_row,
                ty,
            },
        );
        Ok(TranslatedExpression {
            expression: reference,
            ty,
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
    pub(crate) fn translate_target(
        &mut self,
        target: Option<NodeId>,
        non_virtual_invocation: bool,
        member_static: bool,
        declaring: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        self.translate_constrained_target(
            target,
            non_virtual_invocation,
            member_static,
            declaring,
            None,
        )
    }

    /// `TranslateTarget` with `constrainedTo`: the type of a `constrained.` call's target,
    /// which takes the declaring type's place.
    pub(crate) fn translate_constrained_target(
        &mut self,
        target: Option<NodeId>,
        non_virtual_invocation: bool,
        member_static: bool,
        declaring: TypeId,
        constrained_to: Option<TypeId>,
    ) -> Result<TranslatedExpression, Unsupported> {
        let target_type = constrained_to.unwrap_or(declaring);
        let Some(target) = target.filter(|_| !member_static) else {
            let e = self.ast.add(AstNode::TypeReference { ty: target_type });
            self.ast.with_rr(e, Rr::Type(target_type));
            return Ok(TranslatedExpression {
                expression: e,
                ty: target_type,
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
            _ if constrained_to.is_some() => StackType::Ref,
            _ if matches!(self.ts.data(declaring), TypeData::TypeParam { .. }) => StackType::Ref,
            Some(true) => StackType::Obj,
            Some(false) => StackType::Ref,
            None => StackType::Unknown,
        };
        let target_result = self.f.result_type(self.ts, target);
        let requires_managed_reference = expected == StackType::Ref
            || (expected == StackType::Unknown && target_result == StackType::Ref);
        let hint = if !requires_managed_reference {
            target_type
        } else if target_result == StackType::Ref {
            self.ts.intern(TypeData::ByRef(target_type))
        } else {
            self.ts.intern(TypeData::Pointer(target_type))
        };
        let mut translated = self.translate(target, Some(hint))?;
        if requires_managed_reference {
            // A reference of the declaring type, not of another type (issue #1333).
            let n = Normalization::TYPE_ERASURE;
            let same = match *self.ts.data(translated.ty) {
                TypeData::ByRef(element) => {
                    self.ts.normalize_with(element, n) == self.ts.normalize_with(target_type, n)
                }
                _ => false,
            };
            if !same {
                if matches!(self.ts.data(translated.ty), TypeData::ByRef(_)) {
                    return Err("a member access that needs a reference conversion".to_owned());
                }
                // A pointer (or integer) target: `ref *target`.
                let reference = self.ts.intern(TypeData::ByRef(target_type));
                translated = self.convert_to(translated, reference, false)?;
            }
        }
        // (ref x).member => x.member
        match *self.ast.node(translated.expression) {
            AstNode::RefDirection { expression } => {
                translated = self.unwrap_child(translated.expression, expression);
            }
            // (ref x)?.member => x?.member
            AstNode::NullConditional {
                expression: inner,
                rewrap: false,
            } if matches!(self.ast.node(inner), AstNode::RefDirection { .. }) => {
                let AstNode::RefDirection { expression } = *self.ast.node(inner) else {
                    unreachable!("a ref expression");
                };
                let x = self.unwrap_child(translated.expression, expression);
                let ty = self.ts.nullable_underlying(x.ty);
                let e = self.ast.add(AstNode::NullConditional {
                    expression: x.expression,
                    rewrap: false,
                });
                self.ast.with_rr(e, Rr::Plain(ty));
                translated = TranslatedExpression { expression: e, ty };
            }
            _ if requires_managed_reference => {
                return Err("a member access on a ref expression".to_owned());
            }
            _ => {}
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
        // A field, property or event reference does not dispatch, so `base` is never needed.
        self.requires_qualifier_dispatching(name, is_static, declaring, target, false)
    }

    /// `RequiresQualifier(member, target, nonVirtualDispatch)`; `base_needed` is
    /// `nonVirtualDispatch && member.IsOverridable`.
    pub(crate) fn requires_qualifier_dispatching(
        &self,
        name: &str,
        is_static: bool,
        declaring: Option<TypeDefId>,
        target: AstId,
        base_needed: bool,
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
            AstNode::Base => base_needed,
            AstNode::This => false,
            _ => true,
        }
    }

    /// `IsCurrentOrContainingType`.
    pub(crate) fn is_current_or_containing_type(&self, def: Option<TypeDefId>) -> bool {
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
    /// qualified, then with the target cast to the declaring type).
    fn disambiguate_field(
        &mut self,
        field: &FieldOperand,
        target: &mut TranslatedExpression,
        require: bool,
        declaring: TypeId,
    ) -> Result<bool, Unsupported> {
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
                lookup_target = self.cast_target_to(target, declaring)?;
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
        self.ast.with_rr(e, Rr::Operator { ty: left.ty });
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
        let target_is_ref = self.f.result_type(self.ts, target) == StackType::Ref;
        if unaligned != 0 || (!target_is_ref && !self.is_unmanaged_type(ty)) {
            return Err("a store that is no assignment through a reference".to_owned());
        }
        let hint = if target_is_ref {
            self.ts.intern(TypeData::ByRef(ty))
        } else {
            self.ts.intern(TypeData::Pointer(ty))
        };
        let mut pointer = self.translate(target, Some(hint))?;
        let mut translated_value = None;
        let memory_type = if self.is_compatible_pointer_type_for_memory_access(pointer.ty, ty) {
            self.pointer_element(pointer.ty)
                .expect("a pointer or reference")
        } else {
            // A pointer cast to the type of the value, where that is compatible.
            let v = self.translate(value, Some(ty))?;
            let memory_type = if self.ts.is_compatible_type_for_memory_access(v.ty, ty) {
                v.ty
            } else {
                ty
            };
            translated_value = Some(v);
            if matches!(
                self.ast.node(pointer.expression),
                AstNode::RefDirection { .. }
            ) {
                return Err("a store that needs a pointer cast".to_owned());
            }
            let pointer_type = self.ts.intern(TypeData::Pointer(memory_type));
            pointer = self.convert_to(pointer, pointer_type, false)?;
            memory_type
        };
        let target = match *self.ast.node(pointer.expression) {
            AstNode::RefDirection { expression }
            | AstNode::PointerUnary {
                address_of: true,
                expression,
            } => self.unwrap_child(pointer.expression, expression),
            _ => self.dereference(pointer.expression, memory_type),
        };
        let value = match translated_value {
            Some(v) => v,
            None => self.translate(value, Some(target.ty))?,
        };
        if matches!(
            self.ast.node(target.expression),
            AstNode::RefDirection { .. }
        ) && matches!(self.rr_of(target.expression), Rr::ByReference(_))
        {
            return self.ref_reassignment(target, value);
        }
        let e = self.assignment(target, value)?;
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }

    /// `CanUseTypeForStackSlot`: a single definition, or stores that all infer `ty` (up to
    /// type erasure).
    fn can_use_type_for_stack_slot(&mut self, var: VarId, ty: TypeId) -> bool {
        if self.f.var(var).is_single_definition() {
            return true;
        }
        let n = Normalization::TYPE_ERASURE;
        let expected = self.ts.normalize_with(ty, n);
        let stores = self.f.var(var).store_instructions.clone();
        stores.iter().all(|&s| match *self.f.inst(s) {
            Inst::StLoc { value, .. } => {
                let t = self.f.infer_type(self.ts, value);
                self.ts.normalize_with(t, n) == expected
            }
            _ => false,
        })
    }

    /// `VisitStLoc`.
    pub(crate) fn st_loc(
        &mut self,
        inst: NodeId,
        var: VarId,
        value: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let var_type = self.f.var(var).ty;
        let value_inst = value;
        let value = self.translate(value, Some(var_type))?;
        // Stack slots have inaccurate types (`object` for any reference), which the type of
        // the value replaces where every store agrees on it.
        if self.f.var(var).kind == VariableKind::StackSlot
            && !self.loaded_stack_slots.contains(&var)
        {
            let stack = self.f.var(var).stack_type;
            if self.can_use_type_for_stack_slot(var, value.ty)
                && stack == self.ts.stack_type(value.ty)
                && self.ts.kind(value.ty) != TypeKind::Null
            {
                self.f.var_mut(var).ty = value.ty;
            } else if let Inst::DefaultValue { ty, .. } = *self.f.inst(value_inst)
                && self.ts.stack_type(ty.ty) == StackType::VT
            {
                self.f.var_mut(var).ty = ty.ty;
            }
        }
        let left = self.convert_variable(var)?;
        if matches!(self.ast.node(left.expression), AstNode::RefDirection { .. })
            && matches!(self.rr_of(left.expression), Rr::ByReference(_))
        {
            return self.ref_reassignment(left, value);
        }
        let e = self.assignment(left, value)?;
        self.ast.with_il(e.expression, inst);
        Ok(e)
    }
}
