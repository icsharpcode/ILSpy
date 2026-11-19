//! `ExpressionBuilder.BuildArrayInitializerExpression`: the elements of an object, collection
//! or `with` initializer from the access paths of the block's instructions, nested where
//! consecutive paths share a prefix (`new T { A = { B = 1 }, C = { 2, 3 } }`).

use crate::il::ast::{Inst, NodeId, VarId};
use crate::il::semantic_helper::is_pure;
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin};
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::call_builder::{ACCESSOR_SETTER, CallOp};
use super::{Builder, TranslatedExpression, Unsupported};

/// `MethodSemanticsAttributes.Getter`.
const ACCESSOR_GETTER: u16 = 0x02;

/// The member of an access path element, for `AccessPathElement.Equals`.
#[derive(Clone, Debug, PartialEq, Eq)]
enum MemberKey {
    Field {
        origin: (u32, u32),
        declaring: Option<TypeId>,
    },
    Property {
        module: u32,
        token: u32,
        declaring: Option<TypeId>,
    },
    Method {
        module: u32,
        row: u32,
        declaring: Option<TypeId>,
        method_args: Option<Vec<TypeId>>,
    },
}

/// `AccessPathElement`, with what the syntax needs of its member.
#[derive(Clone, Debug)]
struct PathElement {
    key: MemberKey,
    indices: Option<Vec<NodeId>>,
    /// `Member.Name`.
    name: String,
    /// The resolve result `new MemberResolveResult(initObjRR, Member)` of the member.
    member: Rr,
    /// `memberRR.Type`.
    ty: TypeId,
    /// `Member is IMethod { Name: "Add" }`.
    is_add: bool,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum AccessPathKind {
    Invalid,
    Setter,
    Adder,
}

/// `AccessPathElement.GetAccessPath` without a resolver.
struct AccessPath {
    kind: AccessPathKind,
    path: Vec<PathElement>,
    values: Vec<NodeId>,
    /// The `Call` or `CallVirt` of an adder.
    call: Option<(crate::il::ast::CallData, CallOp)>,
}

impl Builder<'_, '_, '_, '_> {
    /// `TranslateWithInitializer`: `value with { ... }`, the value of a record the clone of
    /// a reference type's `<Clone>$()` call.
    pub(crate) fn with_initializer(
        &mut self,
        inst: NodeId,
        instructions: &[NodeId],
        final_inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Some((&first, rest)) = instructions.split_first() else {
            return Err("an empty with initializer".to_owned());
        };
        let (Inst::StLoc { var, value, .. }, Inst::LdLoc { var: final_var }) =
            (self.f.inst(first).clone(), self.f.inst(final_inst).clone())
        else {
            return Err("a with initializer of another shape".to_owned());
        };
        if var != final_var
            || self.f.var(var).kind != crate::il::ast::VariableKind::InitializerTarget
        {
            return Err("a with initializer of another shape".to_owned());
        }
        let ty = self.f.var(var).ty;
        let mut old_value = value;
        if self.ts.is_reference_type(ty) == Some(true) {
            // Reference types go through a `<Clone>$()` call.
            let (Inst::Call(c) | Inst::CallVirt(c)) = self.f.inst(value) else {
                return Err("a with initializer without clone call".to_owned());
            };
            let [this] = c.args[..] else {
                return Err("a with initializer without clone call".to_owned());
            };
            old_value = this;
        }
        let expression = self.translate(old_value, Some(ty))?;
        let object = TranslatedExpression {
            expression: expression.expression,
            ty,
        };
        let initializer = self.initializer_elements(rest, object)?;
        let e = self.ast.add(AstNode::WithInitializer {
            expression: expression.expression,
            initializer,
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(ty));
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `BuildArrayInitializerExpression`: the elements of the initializer of the object
    /// `object` the block's `instructions` (after the store of the object) build.
    pub(crate) fn initializer_elements(
        &mut self,
        instructions: &[NodeId],
        object: TranslatedExpression,
    ) -> Result<Vec<AstId>, Unsupported> {
        let init = Rr::InitializedObject(object.ty);
        let mut stack: Vec<Vec<AstId>> = vec![Vec::new()];
        let mut current: Option<Vec<PathElement>> = None;
        for &inst in instructions {
            // The index variables of C# 6 dictionary initializers.
            if matches!(self.f.inst(inst), Inst::StLoc { .. }) {
                return Err("a dictionary initializer".to_owned());
            }
            let info = self.access_path(inst, &init)?;
            // The IL transform makes no invalid access paths; the managed builder skips one.
            if info.kind == AccessPathKind::Invalid {
                continue;
            }
            match current.take() {
                None => current = Some(info.path.clone()),
                Some(previous) => {
                    let min = previous.len().min(info.path.len());
                    let mut first_difference = 0;
                    while first_difference < min
                        && self
                            .same_element(&info.path[first_difference], &previous[first_difference])
                    {
                        first_difference += 1;
                    }
                    while stack.len() - 1 > first_difference {
                        let member_path = previous[stack.len() - 1].clone();
                        let value_path = previous[stack.len() - 2].clone();
                        let values = stack.pop().expect("an open initializer");
                        let a = self.initializer_assignment(&member_path, &value_path, values)?;
                        stack.last_mut().expect("the root initializer").push(a);
                    }
                    current = Some(info.path.clone());
                }
            }
            let path = current.as_ref().expect("a current path");
            while stack.len() < path.len() {
                stack.push(Vec::new());
            }
            let last = path.last().expect("a member").clone();
            match info.kind {
                AccessPathKind::Adder => {
                    let (call, op) = info.call.ok_or("an adder without its call")?;
                    let e = self.collection_initializer_element(inst, &call, op, object)?;
                    stack.last_mut().expect("an open initializer").push(e);
                }
                AccessPathKind::Setter => {
                    if last.indices.as_ref().is_some_and(|i| !i.is_empty()) {
                        return Err("a dictionary initializer".to_owned());
                    }
                    let [value] = info.values[..] else {
                        return Err("a setter with other values".to_owned());
                    };
                    let v = self.translate(value, Some(last.ty))?;
                    let v = self.convert_to(v, last.ty, true)?;
                    let e = self.ast.add(AstNode::Named {
                        name: last.name.clone(),
                        expression: v.expression,
                    });
                    self.ast.with_il(e, inst);
                    self.ast.with_rr(e, last.member.clone());
                    stack.last_mut().expect("an open initializer").push(e);
                }
                AccessPathKind::Invalid => unreachable!("skipped above"),
            }
        }
        while stack.len() > 1 {
            let path = current.as_ref().expect("a current path");
            let member_path = path[stack.len() - 1].clone();
            let value_path = path[stack.len() - 2].clone();
            let values = stack.pop().expect("an open initializer");
            let a = self.initializer_assignment(&member_path, &value_path, values)?;
            stack.last_mut().expect("the root initializer").push(a);
        }
        Ok(stack.pop().expect("the root initializer"))
    }

    /// `MakeInitializerAssignment`: `Member = value` of the nested initializer of a path
    /// prefix; a value is braced unless it is the one value that is no assignment.
    fn initializer_assignment(
        &mut self,
        member_path: &PathElement,
        value_path: &PathElement,
        values: Vec<AstId>,
    ) -> Result<AstId, Unsupported> {
        let single = match values[..] {
            [v] if !member_path.is_add
                && !matches!(
                    self.ast.node(v),
                    AstNode::Assignment { .. }
                        | AstNode::CompoundAssignment { .. }
                        | AstNode::Named { .. }
                ) =>
            {
                Some(v)
            }
            _ => None,
        };
        let value = match single {
            Some(v) => v,
            None => {
                let e = self.ast.add(AstNode::ArrayInitializer { elements: values });
                let unknown = self.ts.intern(TypeData::UnknownSpecial);
                self.ast.with_rr(e, Rr::Plain(unknown));
                e
            }
        };
        if value_path.indices.as_ref().is_some_and(|i| !i.is_empty()) {
            return Err("a dictionary initializer".to_owned());
        }
        let e = self.ast.add(AstNode::Named {
            name: value_path.name.clone(),
            expression: value,
        });
        self.ast.with_rr(e, value_path.member.clone());
        Ok(e)
    }

    /// `AccessPathElement.Equals`: the same member, and the same indices (pure and
    /// structurally equal).
    fn same_element(&self, a: &PathElement, b: &PathElement) -> bool {
        if a.key != b.key {
            return false;
        }
        match (&a.indices, &b.indices) {
            (None, None) => true,
            (Some(x), Some(y)) => {
                x.len() == y.len()
                    && x.iter().zip(y).all(|(&p, &q)| {
                        p == q
                            || (is_pure(self.f.flags(p))
                                && is_pure(self.f.flags(q))
                                && self.f.structurally_equal(p, q))
                    })
            }
            _ => false,
        }
    }

    /// `GetAccessPath(inst, rootType, settings)`: the members from the initialized object to
    /// the setter or adder `inst` is, with its values.
    fn access_path(&mut self, instruction: NodeId, init: &Rr) -> Result<AccessPath, Unsupported> {
        let mut path: Vec<PathElement> = Vec::new();
        let mut target: Option<VarId> = None;
        let mut kind = AccessPathKind::Invalid;
        let mut values: Option<Vec<NodeId>> = None;
        let mut adder = None;
        let mut inst = Some(instruction);
        while let Some(current) = inst {
            match self.f.inst(current).clone() {
                Inst::Call(call) | Inst::CallVirt(call) => {
                    let op = if matches!(self.f.inst(current), Inst::CallVirt(_)) {
                        CallOp::CallVirt
                    } else {
                        CallOp::Call
                    };
                    let method = &call.method.method;
                    let Some(&first) = call.args.first() else {
                        kind = AccessPathKind::Invalid;
                        break;
                    };
                    let mut next = first;
                    if let Inst::LdObjIfRef { target: t, .. } = *self.f.inst(next) {
                        next = t;
                    }
                    inst = Some(next);
                    let is_accessor = method.accessor_kind != 0;
                    if is_accessor {
                        let MethodOrigin::Def(id) = method.origin else {
                            return Err("an initializer of an unresolved accessor".to_owned());
                        };
                        let (token, _) = self
                            .ts
                            .accessor_owner(id)
                            .ok_or("an accessor without owner")?;
                        if token >> 24 != 0x17 {
                            return Err("an initializer of an event".to_owned());
                        }
                        let row = token & 0x00FF_FFFF;
                        // `CanBeUsedInInitializer` without a resolve context.
                        let can_set =
                            self.ts.comp.module(id.module).md.property_accessors(row).1 != 0;
                        if !can_set && kind == AccessPathKind::Setter {
                            kind = AccessPathKind::Invalid;
                            break;
                        }
                        let is_getter = method.accessor_kind == ACCESSOR_GETTER;
                        let take = call
                            .args
                            .len()
                            .saturating_sub(if is_getter { 1 } else { 2 });
                        let indices: Vec<NodeId> =
                            call.args.iter().skip(1).take(take).copied().collect();
                        if !indices.is_empty() && !self.settings.dictionary_initializers {
                            kind = AccessPathKind::Invalid;
                            break;
                        }
                        let name = self.property_info(id.module, row)?.0;
                        let ty = if method.accessor_kind == ACCESSOR_SETTER {
                            *method
                                .params()
                                .map_err(|e| e.to_string())?
                                .last()
                                .ok_or("a setter without parameters")?
                        } else {
                            method.ret
                        };
                        path.insert(
                            0,
                            PathElement {
                                key: MemberKey::Property {
                                    module: id.module.0,
                                    token,
                                    declaring: method.declaring,
                                },
                                indices: Some(indices),
                                name,
                                member: Rr::Property {
                                    target: Box::new(init.clone()),
                                    accessor: Box::new(call.method.clone()),
                                    ty,
                                    found_by_lookup: false,
                                },
                                ty,
                                is_add: false,
                            },
                        );
                    } else {
                        let MethodOrigin::Def(id) = method.origin else {
                            return Err("an initializer of an unresolved method".to_owned());
                        };
                        path.insert(
                            0,
                            PathElement {
                                key: MemberKey::Method {
                                    module: id.module.0,
                                    row: id.row,
                                    declaring: method.declaring,
                                    method_args: method.method_args.clone(),
                                },
                                indices: None,
                                name: method.name.clone(),
                                member: Rr::MethodMember {
                                    method: Box::new(call.method.clone()),
                                    ty: method.ret,
                                },
                                ty: method.ret,
                                is_add: method.name == "Add",
                            },
                        );
                    }
                    if values.is_none() {
                        if is_accessor {
                            kind = AccessPathKind::Setter;
                            values = Some(vec![*call.args.last().expect("an argument")]);
                        } else {
                            kind = AccessPathKind::Adder;
                            let rest: Vec<NodeId> = call.args.iter().skip(1).copied().collect();
                            if rest.is_empty() {
                                kind = AccessPathKind::Invalid;
                                break;
                            }
                            values = Some(rest);
                            adder = Some((call.clone(), op));
                        }
                    }
                }
                Inst::LdObj { target: t, .. } | Inst::LdObjIfRef { target: t, .. } => {
                    if let Inst::LdFlda {
                        target: ft, field, ..
                    } = self.f.inst(t).clone()
                        && (kind != AccessPathKind::Setter
                            || !self.ts.field_is_read_only(&field.field))
                    {
                        path.insert(0, self.field_element(&field, init));
                        inst = Some(ft);
                    } else if matches!(self.f.inst(current), Inst::LdObjIfRef { .. })
                        && let Inst::LdLoca { var } = *self.f.inst(t)
                    {
                        target = Some(var);
                        inst = None;
                    } else {
                        kind = AccessPathKind::Invalid;
                        break;
                    }
                }
                Inst::StObj {
                    target: t, value, ..
                } => {
                    let Inst::LdFlda {
                        target: ft, field, ..
                    } = self.f.inst(t).clone()
                    else {
                        kind = AccessPathKind::Invalid;
                        break;
                    };
                    path.insert(0, self.field_element(&field, init));
                    inst = Some(ft);
                    if values.is_none() {
                        values = Some(vec![value]);
                        kind = AccessPathKind::Setter;
                    }
                }
                Inst::LdLoc { var } | Inst::LdLoca { var } => {
                    target = Some(var);
                    inst = None;
                }
                Inst::LdFlda {
                    target: ft, field, ..
                } => {
                    path.insert(0, self.field_element(&field, init));
                    inst = Some(ft);
                }
                _ => {
                    kind = AccessPathKind::Invalid;
                    break;
                }
            }
        }
        if kind != AccessPathKind::Invalid
            && let (Some(values), Some(t)) = (&values, target)
            && values.iter().any(|&v| {
                self.f.descendants_post(v).into_iter().any(
                    |n| matches!(*self.f.inst(n), Inst::LdLoc { var } | Inst::LdLoca { var } if var == t),
                )
            })
        {
            kind = AccessPathKind::Invalid;
        }
        Ok(AccessPath {
            kind,
            path,
            values: values.unwrap_or_default(),
            call: adder,
        })
    }

    /// The access path element of a field.
    fn field_element(&self, field: &crate::il::ast::FieldOperand, init: &Rr) -> PathElement {
        let origin = match field.field.origin {
            FieldOrigin::Def { module, row } => (module.0, row),
            FieldOrigin::Fake => (u32::MAX, field.token),
        };
        PathElement {
            key: MemberKey::Field {
                origin,
                declaring: field.field.declaring,
            },
            indices: None,
            name: field.field.name.clone(),
            member: Rr::Field {
                target: Some(Box::new(init.clone())),
                field: field.clone(),
            },
            ty: field.field.ty,
            is_add: false,
        }
    }
}
