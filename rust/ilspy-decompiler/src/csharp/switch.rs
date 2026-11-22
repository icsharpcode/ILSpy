//! `StatementBuilder.TranslateSwitch`: a switch container (a block container whose entry is a
//! single `SwitchInstruction`) as a `SwitchStatement`.

use crate::il::ast::{BranchTarget, Flags, Inst, NodeId};
use crate::il::types::{Sign, StackType};
use crate::semantics::types::TypeKind;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// The most case labels one section is written with; a larger range is not translated.
const MAX_LABELS: u64 = 4096;

/// `StringToInt.Map`: each string (`None` for null) with the value it maps to.
type StringMap = [(Option<Vec<u16>>, i32)];

/// The first value of a switch section's labels, which `goto case` writes.
#[derive(Clone, Debug)]
pub(crate) enum CaseValue {
    Integer(i64),
    /// A string (or `null`) of a switch on strings.
    String(ConstValue),
}

impl Builder<'_, '_, '_, '_> {
    /// `TranslateSwitch(switchContainer, inst)`.
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
        // 'break' within a switch leaves the switch.
        let saved = self.break_target.replace(container);
        let result = self.switch_statement(container, blocks, value, &sections, ty, is_lifted);
        self.break_target = saved;
        // `TranslateSwitch` declares no local functions: those declared in the switch
        // container are left out of the output.
        let left_out = self
            .local_functions
            .iter()
            .filter(|&&lf| {
                matches!(
                    *self.f.inst(lf),
                    Inst::Function {
                        declaration_scope: Some(scope),
                        ..
                    } if scope == container
                )
            })
            .count();
        self.declared_local_functions += left_out;
        result
    }

    fn switch_statement(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        value: NodeId,
        sections: &[NodeId],
        ty: Option<TypeId>,
        is_lifted: bool,
    ) -> Result<AstId, Unsupported> {
        let section_parts: Vec<_> = sections
            .iter()
            .map(|&s| match self.f.inst(s) {
                Inst::SwitchSection { labels, body, .. } => (labels.clone(), *body),
                _ => unreachable!("a switch section"),
            })
            .collect();
        let null_labels: Vec<bool> = sections
            .iter()
            .map(|&s| {
                matches!(
                    self.f.inst(s),
                    Inst::SwitchSection {
                        has_null_label: true,
                        ..
                    }
                )
            })
            .collect();
        // A switch on strings: `StringToInt`'s map gives the string labels of each value.
        let string_map = match self.f.inst(value) {
            Inst::StringToInt { map, .. } => Some(map.clone()),
            _ => None,
        };
        let (value, case_type) =
            self.translate_switch_value(value, ty, &section_parts, false, is_lifted)?;

        let default = default_section(&section_parts);

        // `caseLabelMapping`: the block a section's branch inlines, where every branch to it is
        // in this switch, with the section's first value.
        let mut mapping = Vec::new();
        for (i, (labels, body)) in section_parts.iter().enumerate() {
            if let Inst::Branch {
                target: BranchTarget::Block(b),
            } = *self.f.inst(*body)
                && blocks[1..].contains(&b)
                && self.branches_stay_in_switch(container, b)
            {
                let mut label_type = case_type;
                let label = if i == default {
                    None
                } else if null_labels[i] {
                    // `case null`: the null of the null type.
                    label_type = self.ts.intern(TypeData::Null);
                    Some(CaseValue::String(ConstValue::Null))
                } else {
                    let v = labels
                        .values()
                        .next()
                        .ok_or("a switch section without labels")?;
                    Some(match &string_map {
                        // The first string the section's first value maps.
                        Some(map) => {
                            let (key, _) = map
                                .iter()
                                .find(|(_, mapped)| i64::from(*mapped) == v)
                                .ok_or("a string switch value without key")?;
                            CaseValue::String(match key {
                                Some(s) => ConstValue::String(s.clone()),
                                None => ConstValue::Null,
                            })
                        }
                        None => CaseValue::Integer(v),
                    })
                };
                mapping.push((b, label, label_type));
            }
        }
        let saved_mapping = std::mem::replace(&mut self.case_label_mapping, mapping);
        let result = self.switch_sections(
            container,
            blocks,
            &section_parts,
            default,
            case_type,
            string_map.as_deref(),
            &null_labels,
        );
        self.case_label_mapping = saved_mapping;
        let ast_sections = result?;
        let s = self.ast.add(AstNode::Switch {
            expression: value.expression,
            sections: ast_sections,
        });
        Ok(self.ast.with_il(s, container))
    }

    /// Whether every branch to `block` has `container` as its closest switch container
    /// (`BlockContainer.FindClosestSwitchContainer`).
    fn branches_stay_in_switch(&self, container: NodeId, block: NodeId) -> bool {
        self.f.descendants_post(container).into_iter().all(|n| {
            !matches!(*self.f.inst(n), Inst::Branch { target: BranchTarget::Block(t) } if t == block)
                || self.closest_switch_container(n) == Some(container)
        })
    }

    /// `FindClosestSwitchContainer`
    fn closest_switch_container(&self, inst: NodeId) -> Option<NodeId> {
        let mut current = self.f.node(inst).parent;
        while let Some(c) = current {
            if matches!(
                self.f.inst(c),
                Inst::BlockContainer {
                    kind: crate::il::ast::ContainerKind::Switch,
                    ..
                }
            ) {
                return Some(c);
            }
            current = self.f.node(c).parent;
        }
        None
    }

    /// The sections of the switch, each with its case labels and its body (the inlined block of
    /// a section that branches to one).
    #[allow(clippy::too_many_arguments)]
    fn switch_sections(
        &mut self,
        container: NodeId,
        blocks: &[NodeId],
        section_parts: &[(crate::util::long_set::LongSet, NodeId)],
        default: usize,
        case_type: TypeId,
        string_map: Option<&StringMap>,
        null_labels: &[bool],
    ) -> Result<Vec<AstId>, Unsupported> {
        let mut ast_sections = Vec::with_capacity(section_parts.len());
        for (i, (labels, body)) in section_parts.iter().enumerate() {
            let case_labels = if i == default {
                vec![None]
            } else {
                if labels.count() > MAX_LABELS {
                    return Err("a switch section with too many labels".to_owned());
                }
                let mut case_labels = Vec::new();
                if null_labels[i] {
                    case_labels.push(Some(self.ast.add(AstNode::Null)));
                }
                for v in labels.values() {
                    match &string_map {
                        // `CreateTypedCaseLabel` of a string: every key mapped to the value.
                        Some(map) => {
                            for (key, _) in map.iter().filter(|(_, mapped)| i64::from(*mapped) == v)
                            {
                                let constant = match key {
                                    Some(s) => ConstValue::String(s.clone()),
                                    None => ConstValue::Null,
                                };
                                case_labels
                                    .push(Some(self.string_case_label(case_type, constant)?));
                            }
                        }
                        None => case_labels.push(Some(self.case_label(v, case_type)?)),
                    }
                }
                case_labels
            };
            let statements = match *self.f.inst(*body) {
                Inst::Branch {
                    target: BranchTarget::Block(b),
                } if blocks[1..].contains(&b) => {
                    if !self.case_label_mapping.iter().any(|(m, ..)| *m == b) {
                        return Err("a switch section branching out of its switch".to_owned());
                    }
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
        // The blocks no section label maps follow the last section, each with its label.
        let Some(&last) = ast_sections.last() else {
            return Ok(ast_sections);
        };
        let mut added = Vec::new();
        for &block in &blocks[1..] {
            if self.case_label_mapping.iter().any(|(m, ..)| *m == block) {
                continue;
            }
            let name = self.ensure_unique_label(block);
            added.push(self.ast.add(AstNode::Label { name }));
            let Inst::Block { instructions, .. } = self.f.inst(block).clone() else {
                return Err("a switch container without blocks".to_owned());
            };
            for nested in instructions {
                let s = self.convert(nested)?;
                match self.ast.node(s) {
                    AstNode::Block { statements } => added.extend(statements.clone()),
                    _ => added.push(s),
                }
            }
        }
        if let Some(label) = self
            .end_container_labels
            .iter()
            .find(|(c, _)| *c == container)
            .map(|(_, l)| l.clone())
        {
            added.push(self.ast.add(AstNode::Label { name: label }));
            added.push(self.ast.add(AstNode::Break));
        }
        if let AstNode::SwitchSection { statements, .. } = self.ast.node_mut(last) {
            statements.extend(added);
        }
        Ok(ast_sections)
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
        is_expression_context: bool,
        is_lifted: bool,
    ) -> Result<(TranslatedExpression, TypeId), Unsupported> {
        // `StringToInt`: the string (or span of chars) itself, of its expected type.
        if let Inst::StringToInt {
            arg, ty: expected, ..
        } = *self.f.inst(value)
        {
            let translated = self.translate(arg, None)?;
            let string = self.ts.known_type(KnownTypeCode::String);
            // A switch expression does not convert implicitly.
            if is_expression_context {
                let value = self.convert_to(translated, expected, false)?;
                return Ok((value, string));
            }
            let mut value = self.convert_to(translated, expected, true)?;
            if self.csharp_switch_governing_type(value.ty)? != expected {
                value = self.convert_to(value, expected, false)?;
            }
            return Ok((value, string));
        }
        let translated = self.translate(value, None)?;
        let mut governing = ty.unwrap_or(translated.ty);
        let value_stack = self.f.result_type(self.ts, value);
        // `GetStackType()` and `GetSign()` of the type (a `bool` or `char` is of stack type I4).
        let (governing_stack, governing_sign) =
            (self.ts.stack_type(governing), self.ts.type_sign(governing));
        match value_stack {
            StackType::I8 => {
                if governing_stack != StackType::I8 {
                    governing = self.find_type(StackType::I8, governing_sign);
                }
            }
            StackType::I4 => {
                if governing_stack != StackType::I4 {
                    governing = self.find_type(StackType::I4, governing_sign);
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
            // A lifted switch is governed by its nullable type.
            StackType::VT if is_lifted => {}
            _ => return Err("a switch on a value type".to_owned()),
        }
        // A switch expression does not convert implicitly.
        if is_expression_context {
            let value = self.convert_to(translated, governing, false)?;
            return Ok((value, governing));
        }
        let mut value = self.convert_to(translated, governing, true)?;
        if self.csharp_switch_governing_type(value.ty)? != governing {
            value = self.convert_to(value, governing, false)?;
        }
        Ok((value, governing))
    }

    /// `GetCSharpSwitchGoverningType`: a type that is no switch type is governed through its
    /// only implicit conversion operator to a switch type.
    fn csharp_switch_governing_type(&mut self, t: TypeId) -> Result<TypeId, Unsupported> {
        if self.is_compatible_with_switch(t) {
            return Ok(t);
        }
        let candidates = self
            .ts
            .get_methods(t, None, crate::typesystem::lookup::MemberOptions::default())
            .map_err(|e| e.to_string())?;
        let mut applicable = Vec::new();
        for c in &candidates {
            if c.name != "op_Implicit" {
                continue;
            }
            let Some(m) = crate::semantics::Method::from_resolved(self.ts, c) else {
                continue;
            };
            if m.is_operator
                && m.parameters.len() == 1
                && self.is_compatible_with_switch(m.return_type)
            {
                applicable.push(m.return_type);
            }
        }
        Ok(match applicable[..] {
            [only] => only,
            _ => t,
        })
    }

    /// `VisitSwitchInstruction` as an expression: `value switch { label => body, ..., _ => body }`,
    /// the bodies converted to the result type and cast where their best common type is
    /// another.
    pub(super) fn switch_expression(
        &mut self,
        inst: NodeId,
        hint: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Switch {
            value,
            sections,
            ty,
            is_lifted,
            ..
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a switch");
        };
        let string_map = match self.f.inst(value) {
            Inst::StringToInt { map, .. } => Some(map.clone()),
            _ => None,
        };
        let parts: Vec<_> = sections
            .iter()
            .map(|&s| match self.f.inst(s) {
                Inst::SwitchSection {
                    labels,
                    body,
                    is_compiler_generated_default_section,
                    has_null_label,
                } => (
                    labels.clone(),
                    *body,
                    *is_compiler_generated_default_section,
                    *has_null_label,
                ),
                _ => unreachable!("a switch section"),
            })
            .collect();
        let label_sets: Vec<_> = parts.iter().map(|(l, b, ..)| (l.clone(), *b)).collect();
        let (value, case_type) =
            self.translate_switch_value(value, ty, &label_sets, true, is_lifted)?;
        let default = default_section(&label_sets);
        let result_stack = self.f.result_type(self.ts, inst);
        let result_type = if self.ts.kind(hint) != TypeKind::Unknown
            && self.ts.stack_type(hint) == result_stack
        {
            hint
        } else {
            self.f.infer_type(self.ts, inst)
        };
        let mut arms = Vec::with_capacity(parts.len());
        // The bodies type inference sees (not the throw expressions), as indices into `arms`.
        let mut inferred = Vec::new();
        for (i, (labels, body, _, has_null_label)) in parts.iter().enumerate() {
            if i == default {
                continue;
            }
            let pattern = if *has_null_label {
                self.ast.add(AstNode::Null)
            } else {
                let mut values = labels.values();
                let (Some(v), None) = (values.next(), values.next()) else {
                    return Err("a switch expression section of several labels".to_owned());
                };
                match &string_map {
                    // `CreateTypedCaseLabel(...).Single()`: the one key mapped to the value.
                    Some(map) => {
                        let mut keys = map.iter().filter(|(_, mapped)| i64::from(*mapped) == v);
                        let (Some((key, _)), None) = (keys.next(), keys.next()) else {
                            return Err("a switch expression section of several strings".to_owned());
                        };
                        let constant = match key {
                            Some(s) => ConstValue::String(s.clone()),
                            None => ConstValue::Null,
                        };
                        self.string_case_label(case_type, constant)?
                    }
                    None => self.case_label(v, case_type)?,
                }
            };
            let body = self.switch_expression_body(*body, result_type)?;
            if !matches!(
                self.ast.node(body.expression),
                AstNode::ThrowExpression { .. }
            ) {
                inferred.push(arms.len());
            }
            arms.push((pattern, body));
        }
        if !parts[default].2 {
            let pattern = self.ast.add(AstNode::Identifier {
                name: "_".to_owned(),
            });
            let body = self.switch_expression_body(parts[default].1, result_type)?;
            if !matches!(
                self.ast.node(body.expression),
                AstNode::ThrowExpression { .. }
            ) {
                inferred.push(arms.len());
            }
            arms.push((pattern, body));
        }
        if inferred.is_empty() {
            return Err("a switch expression without a typed body".to_owned());
        }
        // `GetBestCommonType` of the bodies (with the C# algorithm); where it is not the result
        // type, the first body is cast to it, then all of them.
        let n = Normalization::TYPE_ERASURE;
        let mut ty = None;
        for attempt in 0..2 {
            if attempt == 1 {
                let i = inferred[0];
                arms[i].1 = self.convert_to(arms[i].1, result_type, false)?;
            }
            let rrs: Vec<_> = inferred
                .iter()
                .map(|&i| self.semantic_rr(arms[i].1.expression, arms[i].1.ty))
                .collect();
            let mut conversions = crate::semantics::Conversions::new(self.ts);
            let (common, success) =
                crate::semantics::TypeInference::new(&mut conversions).best_common_type(&rrs);
            if success
                && self.ts.normalize_with(common, n) == self.ts.normalize_with(result_type, n)
            {
                ty = Some(common);
                break;
            }
        }
        let ty = match ty {
            Some(t) => t,
            None => {
                for &i in &inferred[1..] {
                    arms[i].1 = self.convert_to(arms[i].1, result_type, false)?;
                }
                result_type
            }
        };
        let e = self.ast.add(AstNode::SwitchExpression {
            expression: value.expression,
            arms: arms.into_iter().map(|(p, b)| (p, b.expression)).collect(),
        });
        self.ast.with_il(e, inst);
        self.ast.with_rr(e, Rr::Plain(ty));
        Ok(TranslatedExpression { expression: e, ty })
    }

    /// `TranslateSectionBody`: the body converted to the result type, implicitly where it can.
    fn switch_expression_body(
        &mut self,
        body: NodeId,
        result_type: TypeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let b = self.translate(body, Some(result_type))?;
        self.convert_to(b, result_type, true)
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

    /// `FindType(stackType, sign)`: `nint` or `nuint` for `I` with native integers.
    pub(super) fn find_type(&mut self, stack: StackType, sign: Sign) -> TypeId {
        if stack == StackType::I && self.settings.native_integers {
            return self.ts.intern(if sign == Sign::Unsigned {
                TypeData::NUInt
            } else {
                TypeData::NInt
            });
        }
        let code = stack.to_primitive(sign).to_known();
        self.ts.known_type(code)
    }

    /// `CreateTypedCaseLabel(i, type)` without a string map, written with
    /// `ConvertConstantValue(label, allowImplicitConversion: true)`.
    /// The case label of a string constant (or `null`) of a switch on strings.
    pub(crate) fn string_case_label(
        &mut self,
        ty: TypeId,
        value: ConstValue,
    ) -> Result<AstId, Unsupported> {
        let e = self.ast.add(AstNode::ConstantValue {
            ty,
            value: value.clone(),
            allow_implicit_conversion: true,
        });
        let (ty, value) = self.constant_value_rr(ty, value, true)?;
        self.ast.with_rr(e, Rr::Constant { ty, value });
        Ok(e)
    }

    pub(crate) fn case_label(&mut self, i: i64, ty: TypeId) -> Result<AstId, Unsupported> {
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
