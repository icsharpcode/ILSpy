//! `StatementBuilder.TranslateSwitch`: a switch container (a block container whose entry is a
//! single `SwitchInstruction`) as a `SwitchStatement`.

use crate::il::ast::{BranchTarget, Flags, Inst, NodeId};
use crate::il::types::{Sign, StackType};
use crate::semantics::types::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::TypeId;

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// The most case labels one section is written with; a larger range is not translated.
const MAX_LABELS: u64 = 4096;

impl Builder<'_, '_, '_, '_> {
    /// `TranslateSwitch(switchContainer, inst)`. A lifted switch, a switch on strings, labels a
    /// section body does not inline (`goto case`, labeled blocks) and a governing type reached
    /// through a conversion operator are not translated.
    pub(super) fn translate_switch(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        switch: NodeId,
    ) -> Result<AstId, Unsupported> {
        let Inst::Switch {
            value,
            sections,
            ty,
            is_lifted,
            ..
        } = self.f.inst(switch).clone()
        else {
            unreachable!("a switch");
        };
        if is_lifted {
            return Err("a lifted switch".to_owned());
        }
        if matches!(self.f.inst(value), Inst::StringToInt { .. }) {
            return Err("a switch on strings".to_owned());
        }
        // 'break' within a switch leaves the switch.
        let saved = self.break_target.replace(container);
        let result = self.switch_statement(container, blocks, value, &sections, ty);
        self.break_target = saved;
        result
    }

    fn switch_statement(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        value: NodeId,
        sections: &[NodeId],
        ty: Option<TypeId>,
    ) -> Result<AstId, Unsupported> {
        let section_parts: Vec<_> = sections
            .iter()
            .map(|&s| match self.f.inst(s) {
                Inst::SwitchSection { labels, body, .. } => (labels.clone(), *body),
                _ => unreachable!("a switch section"),
            })
            .collect();
        let (value, case_type) = self.translate_switch_value(value, ty, &section_parts)?;

        let default = default_section(&section_parts);

        // The blocks of the container a section's branch inlines (`caseLabelMapping`).
        let mut inlined = Vec::new();
        let mut ast_sections = Vec::with_capacity(section_parts.len());
        for (i, (labels, body)) in section_parts.iter().enumerate() {
            let case_labels = if i == default {
                vec![None]
            } else {
                if labels.count() > MAX_LABELS {
                    return Err("a switch section with too many labels".to_owned());
                }
                let mut case_labels = Vec::new();
                for v in labels.values() {
                    case_labels.push(Some(self.case_label(v, case_type)?));
                }
                case_labels
            };
            let statements = match *self.f.inst(*body) {
                Inst::Branch {
                    target: BranchTarget::Block(b),
                } if blocks[1..].contains(&b) => {
                    // A block other branches reach would need `goto case`.
                    if self.f.node(b).incoming_edge_count != 1 {
                        return Err("a switch section reached by goto case".to_owned());
                    }
                    inlined.push(b);
                    self.switch_section_body(b)?
                }
                Inst::Leave { target, .. }
                    if i == default && case_labels.len() == 1 && target == Some(container) =>
                {
                    // A default section that only leaves the switch is dropped.
                    continue;
                }
                _ => self.switch_section_body(*body)?,
            };
            let s = self.ast.add(AstNode::SwitchSection {
                labels: case_labels,
                statements,
            });
            ast_sections.push(s);
        }
        // Blocks no section inlines would be written with labels.
        if blocks[1..].iter().any(|b| !inlined.contains(b)) {
            return Err("a switch with labeled blocks".to_owned());
        }
        let s = self.ast.add(AstNode::Switch {
            expression: value.expression,
            sections: ast_sections,
        });
        Ok(self.ast.with_il(s, container))
    }

    /// `ConvertSwitchSectionBody`: the body, with a `break` where its end point is reachable.
    fn switch_section_body(&mut self, body: NodeId) -> Result<Vec<AstId>, Unsupported> {
        let s = self.convert(body)?;
        let mut statements = vec![s];
        if !self.f.has_flag(body, Flags::END_POINT_UNREACHABLE) {
            let b = self.ast.add(AstNode::Break);
            if let AstNode::Block { statements: inner } = self.ast.node_mut(s) {
                inner.push(b);
            } else {
                statements.push(b);
            }
        }
        Ok(statements)
    }

    /// `TranslateSwitchValue(inst, isExpressionContext: false)` without `StringToInt`: the
    /// switch value converted to its governing type, and the type of the case labels.
    fn translate_switch_value(
        &mut self,
        value: NodeId,
        ty: Option<TypeId>,
        sections: &[(crate::util::long_set::LongSet, NodeId)],
    ) -> Result<(TranslatedExpression, TypeId), Unsupported> {
        let translated = self.translate(value, None)?;
        let mut governing = ty.unwrap_or(translated.ty);
        let value_stack = self.f.result_type(self.ts, value);
        let primitive = self.ts.primitive_type(governing);
        match value_stack {
            StackType::I8 => {
                if primitive.stack_type() != StackType::I8 {
                    governing = self.find_type(StackType::I8, primitive.sign());
                }
            }
            StackType::I4 => {
                if primitive.stack_type() != StackType::I4 {
                    governing = self.find_type(StackType::I4, primitive.sign());
                }
                let p = self.ts.primitive_type(governing);
                let size = self.ts.type_size(governing);
                if size > 0 && size < 4 {
                    // The governing type is widened when it cannot hold every case value.
                    let bits = 8 * size;
                    let (min, max) = if p.sign() == Sign::Unsigned {
                        (0i64, (1i64 << bits) - 1)
                    } else {
                        (-(1i64 << (bits - 1)), (1i64 << (bits - 1)) - 1)
                    };
                    let default = default_section(sections);
                    for (i, (labels, _)) in sections.iter().enumerate() {
                        if i == default {
                            continue;
                        }
                        let interval = labels.containing_interval();
                        if interval.start < min || interval.inclusive_end() > max {
                            governing = self.find_type(StackType::I4, Sign::Signed);
                            break;
                        }
                    }
                }
            }
            _ => return Err("a switch on a value type".to_owned()),
        }
        let mut value = self.convert_to(translated, governing, true)?;
        // `GetCSharpSwitchGoverningType`: a type that is no switch type is governed through its
        // implicit conversion operators, which are not translated (an enum declares none).
        if !self.is_compatible_with_switch(value.ty) && self.ts.kind(value.ty) != TypeKind::Enum {
            return Err("a switch governed through a conversion operator".to_owned());
        }
        if value.ty != governing {
            value = self.convert_to(value, governing, false)?;
        }
        Ok((value, governing))
    }

    /// `IsCompatibleWithSwitch`.
    fn is_compatible_with_switch(&mut self, t: TypeId) -> bool {
        let t = self.ts.nullable_underlying(t);
        [
            KnownTypeCode::Boolean,
            KnownTypeCode::SByte,
            KnownTypeCode::Byte,
            KnownTypeCode::Int16,
            KnownTypeCode::UInt16,
            KnownTypeCode::Int32,
            KnownTypeCode::UInt32,
            KnownTypeCode::Int64,
            KnownTypeCode::UInt64,
            KnownTypeCode::Char,
            KnownTypeCode::String,
        ]
        .iter()
        .any(|&c| self.ts.is_known(t, c))
    }

    /// `FindType(stackType, sign)`.
    pub(super) fn find_type(&mut self, stack: StackType, sign: Sign) -> TypeId {
        let code = stack.to_primitive(sign).to_known();
        self.ts.known_type(code)
    }

    /// `CreateTypedCaseLabel(i, type)` without a string map, written with
    /// `ConvertConstantValue(label, allowImplicitConversion: true)`.
    fn case_label(&mut self, i: i64, ty: TypeId) -> Result<AstId, Unsupported> {
        let ty = self.ts.nullable_underlying(ty);
        let value = if self.ts.is_known(ty, KnownTypeCode::Boolean) {
            ConstValue::Boolean(i != 0)
        } else {
            let code = if self.ts.kind(ty) == TypeKind::Enum {
                let underlying = self.enum_underlying(ty);
                self.ts.type_code(underlying)
            } else {
                self.ts.type_code(ty)
            };
            if code == 0 {
                return Err("a case label of a type without type code".to_owned());
            }
            super::translated_expression::primitive_cast(code, &ConstValue::Int64(i))?
                .ok_or("a case label the managed cast rejects")?
        };
        let e = self.ast.add(AstNode::ConstantValue {
            ty,
            value: value.clone(),
            allow_implicit_conversion: true,
        });
        let (ty, value) = self.constant_value_rr(ty, value, true)?;
        self.ast.with_rr(e, Rr::Constant { ty, value });
        Ok(e)
    }
}

/// `GetDefaultSection` over the sections' labels.
fn default_section(sections: &[(crate::util::long_set::LongSet, NodeId)]) -> usize {
    let mut default = 0;
    for (i, (labels, _)) in sections.iter().enumerate() {
        if labels.count() > sections[default].0.count() {
            default = i;
        }
    }
    default
}
