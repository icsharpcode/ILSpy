//! `RecordDecompiler.FieldsAndProperties`: the fields and properties of a record in declaration
//! order, read off the members the compiler generated from it (`DetectMemberOrder`). Metadata
//! keeps the order of the fields and of the properties, but not how the two interleave.

use std::collections::HashMap;

use crate::il::ast::{Function, Inst, NodeId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Settings;
use crate::metadata::tables::{TableId, rows, token};
use crate::typesystem::compilation::{ModuleId, TypeDefId};
use crate::typesystem::members::MethodDefId;
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin};
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// How `CSharpDecompiler.DecompileBodyForAnalysis` reads a body: the IL transforms run with the
/// C# 1 settings the host passes.
#[derive(Clone, Copy, Debug)]
pub struct Analysis {
    pub settings: Settings,
    pub use_ref_locals_for_accurate_order_of_evaluation: bool,
}

/// A field or property of the record: the members `FieldsAndProperties` lists.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Member {
    Property(u32),
    Field(u32),
}

impl Member {
    fn token(self) -> u32 {
        match self {
            Member::Property(r) => token(TableId::Property, r),
            Member::Field(r) => token(TableId::Field, r),
        }
    }
}

struct Record<'r, 'c, 'a> {
    ts: &'r mut TypeSystem<'c, 'a>,
    def: TypeDefId,
    /// The record's own type parameters, which `IsRecordType` wants as the type arguments.
    own_arguments: Vec<TypeId>,
    analysis: Analysis,
    /// `backingFieldToAutoProperty`, as rows.
    backing_field_to_auto_property: HashMap<u32, u32>,
}

/// `RecordDecompiler.FieldsAndProperties` of the record `row` of `module`, as tokens. An error
/// where the engine cannot read a body the analysis needs.
pub fn fields_and_properties(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
    analysis: Analysis,
) -> Result<Vec<u32>, String> {
    let def = TypeDefId { module, row };
    let own = ts.intern(TypeData::Def(def));
    let own_arguments = ts.type_arguments(own);
    let mut record = Record {
        ts,
        def,
        own_arguments,
        analysis,
        backing_field_to_auto_property: HashMap::new(),
    };
    let comp = record.ts.comp;
    let properties = comp.visible_properties(def).map_err(|e| e.to_string())?;
    let fields = comp.visible_fields(def).map_err(|e| e.to_string())?;
    // `DetectAutomaticProperties`.
    for &p in &properties {
        if let Some(field) = record.auto_property_field(p)? {
            record.backing_field_to_auto_property.insert(field, p);
        }
    }
    let all_members: Vec<Member> = properties
        .iter()
        .map(|&p| Member::Property(p))
        .chain(
            fields
                .iter()
                .filter(|f| !record.backing_field_to_auto_property.contains_key(f))
                .map(|&f| Member::Field(f)),
        )
        .collect();
    Ok(record
        .detect_member_order(&all_members)?
        .into_iter()
        .map(Member::token)
        .collect())
}

impl Record<'_, '_, '_> {
    fn module(&self) -> ModuleId {
        self.def.module
    }

    /// `IsRecordType`: the record itself, with its own type parameters as the arguments.
    fn is_record_type(&mut self, t: TypeId) -> bool {
        let t = match self.ts.data(t) {
            TypeData::Annotated { inner, .. } => *inner,
            _ => t,
        };
        self.ts.definition(t) == Some(self.def) && self.ts.type_arguments(t) == self.own_arguments
    }

    /// `DecompileBody`: the function as `DecompileBodyForAnalysis` leaves it, `None` for a method
    /// without a body.
    fn body(&mut self, method: u32) -> Result<Option<ReadFunction>, String> {
        if method == 0 {
            return Ok(None);
        }
        let m = MethodDefId {
            module: self.module(),
            row: method,
        };
        let comp = self.ts.comp;
        let md = &comp.module(m.module).md;
        let def = md
            .get::<rows::MethodDef>(method)
            .map_err(|e| e.to_string())?;
        // `HasBody`: an RVA and neither abstract nor an internal call, native or runtime method.
        if def.rva == 0 || def.flags & 0x2400 != 0 || def.impl_flags & 0x1007 != 0 {
            return Ok(None);
        }
        if !md.generic_params(TableId::MethodDef, method).is_empty() {
            // As the host's analysis, which reads such a method with the managed pipeline.
            return Err("the analysis of a generic method".to_owned());
        }
        let body = md.method_body(def.rva).map_err(|e| e.to_string())?;
        let context =
            crate::wire::ilast::analysis_context(self.ts, m).map_err(|e| e.to_string())?;
        let options = crate::il::reader::ReaderOptions {
            use_ref_locals_for_accurate_order_of_evaluation: self
                .analysis
                .use_ref_locals_for_accurate_order_of_evaluation,
            debug_info: None,
        };
        let mut f = crate::il::reader::read_il_in_context(self.ts, m, body, options, context)
            .map_err(|e| e.to_string())?;
        let mut ctx = crate::il::transforms::Context {
            ts: self.ts,
            settings: self.analysis.settings,
            reader: options,
            rerun_requested: false,
            removed_variables: Vec::new(),
            resolver: None,
            root_names: None,
        };
        crate::il::transforms::run_prefix(
            &mut f,
            &mut ctx,
            crate::il::transforms::analysis_prefix(),
        );
        crate::il::transforms::combine_exits_transform::run(&mut f, &mut ctx);
        Ok(Some(f))
    }

    /// `IsAutoProperty`: the backing field of the auto property `p`.
    fn auto_property_field(&mut self, p: u32) -> Result<Option<u32>, String> {
        let module = self.module();
        let comp = self.ts.comp;
        let md = &comp.module(module).md;
        let (get, set, _) = md.property_accessors(p);
        let any = if get != 0 { get } else { set };
        if any == 0 {
            return Ok(None);
        }
        let is_static = md
            .get::<rows::MethodDef>(any)
            .is_ok_and(|m| m.flags & 0x0010 != 0);
        if is_static {
            return Ok(None);
        }
        let property_name = md
            .string(md.get::<rows::Property>(p).map_err(|e| e.to_string())?.name)
            .map_err(|e| e.to_string())?
            .into_owned();
        let (_, parameters) = self
            .ts
            .property_signature(module, p)
            .map_err(|e| e.to_string())?;
        if !parameters.is_empty() {
            return Ok(None);
        }
        let mut field = None;
        if get != 0 {
            let Some(f) = self.auto_getter_field(get)? else {
                return Ok(None);
            };
            field = Some(f);
        }
        if set != 0 {
            let Some(f) = self.auto_setter_field(set)? else {
                return Ok(None);
            };
            if field.as_ref().is_some_and(|g| g.0 != f.0) {
                return Ok(None);
            }
            field.get_or_insert(f);
        }
        let Some((row, declaring, name)) = field else {
            return Ok(None);
        };
        if !declaring.is_some_and(|d| self.is_record_type(d)) {
            return Ok(None);
        }
        Ok((name == format!("<{property_name}>k__BackingField")).then_some(row))
    }

    /// `IsAutoGetter` of an instance getter: `return this.field;`.
    fn auto_getter_field(
        &mut self,
        getter: u32,
    ) -> Result<Option<(u32, Option<TypeId>, String)>, String> {
        let Some(r) = self.body(getter)? else {
            return Ok(None);
        };
        let f = &r.function;
        let Some(&first) = entry_instructions(f).first() else {
            return Ok(None);
        };
        let Some(value) = f.match_return(first) else {
            return Ok(None);
        };
        let Some((target, field)) = f.match_ldfld(self.ts, value) else {
            return Ok(None);
        };
        if !f.match_ld_this(target) {
            return Ok(None);
        }
        Ok(self.own_field(&field.field))
    }

    /// `IsAutoSetter` of an instance setter: `this.field = value; return;`.
    fn auto_setter_field(
        &mut self,
        setter: u32,
    ) -> Result<Option<(u32, Option<TypeId>, String)>, String> {
        let Some(r) = self.body(setter)? else {
            return Ok(None);
        };
        let f = &r.function;
        let instructions = entry_instructions(f);
        let Some(&first) = instructions.first() else {
            return Ok(None);
        };
        let Some((target, field, value)) = f.match_stfld(first) else {
            return Ok(None);
        };
        if !f.match_ld_this(target) {
            return Ok(None);
        }
        let Some(v) = f.match_ldloc(value) else {
            return Ok(None);
        };
        let var = f.var(v);
        if !(var.kind == VariableKind::Parameter && var.index == Some(0)) {
            return Ok(None);
        }
        let returns = instructions
            .get(1)
            .and_then(|&i| f.match_return(i))
            .is_some_and(|v| f.match_nop(v));
        Ok(if returns {
            self.own_field(&field.field)
        } else {
            None
        })
    }

    /// A field of this module, as row, declaring type and name.
    fn own_field(
        &self,
        field: &crate::typesystem::resolve::ResolvedField,
    ) -> Option<(u32, Option<TypeId>, String)> {
        match field.origin {
            FieldOrigin::Def { module, row } if module == self.module() => {
                Some((row, field.declaring, field.name.clone()))
            }
            _ => None,
        }
    }

    /// `DetectMemberOrder`.
    fn detect_member_order(&mut self, all_members: &[Member]) -> Result<Vec<Member>, String> {
        let equality_order = self.detect_order_from_equals(all_members)?;
        let print_order = self.detect_order_from_print_members(all_members)?;
        let declaration_order = match (equality_order, print_order) {
            (Some(e), Some(p)) => merge_along_common_members(&e, &p),
            (Some(o), None) | (None, Some(o)) => o,
            (None, None) => return Ok(all_members.to_vec()),
        };
        // The members whose order was read off fill the positions they had, in that order.
        let mut next = declaration_order.iter();
        Ok(all_members
            .iter()
            .map(|m| {
                if declaration_order.contains(m) {
                    *next.next().expect("one position per ordered member")
                } else {
                    *m
                }
            })
            .collect())
    }

    /// The first visible instance method of the record named `name` with one parameter, and
    /// that parameter's type.
    fn single_parameter_method(&mut self, name: &str) -> Result<Vec<(u32, TypeId)>, String> {
        let module = self.module();
        let comp = self.ts.comp;
        let md = &comp.module(module).md;
        let mut result = Vec::new();
        for m in md.type_methods(self.def.row).map_err(|e| e.to_string())? {
            if !comp.is_visible_method(module, m) {
                continue;
            }
            let def = md.get::<rows::MethodDef>(m).map_err(|e| e.to_string())?;
            if def.flags & 0x0010 != 0 || md.string(def.name).map_err(|e| e.to_string())? != name {
                continue;
            }
            let (_, parameters) = self.ts.method_signature(MethodDefId { module, row: m });
            if let [p] = parameters.as_slice() {
                result.push((m, p.ty));
            }
        }
        Ok(result)
    }

    /// `DetectOrderFromEquals`: the members the generated `Equals(R other)` compares, in order.
    fn detect_order_from_equals(
        &mut self,
        all_members: &[Member],
    ) -> Result<Option<Vec<Member>>, String> {
        let mut equals = None;
        for (m, ty) in self.single_parameter_method("Equals")? {
            if self.is_record_type(ty) {
                equals = Some(m);
                break;
            }
        }
        let Some(equals) = equals else {
            return Ok(None);
        };
        let Some(r) = self.body(equals)? else {
            return Ok(None);
        };
        let f = &r.function;
        let Some(&first) = entry_instructions(f).first() else {
            return Ok(None);
        };
        let Some(mut value) = f.match_return(first) else {
            return Ok(None);
        };
        if let Some((_, rhs)) =
            crate::il::transforms::expression_transforms::match_logic_or(f, value)
        {
            // this == other || ...
            value = rhs;
        }
        let mut conditions = Vec::new();
        unpack_logic_and_chain(f, value, &mut conditions);
        let mut order = Vec::new();
        for condition in conditions {
            // callvirt Equals(call get_Default(), ldfld <A>k__BackingField(ldloc this), ...)
            let Inst::CallVirt(call) = f.inst(condition) else {
                continue;
            };
            if call.method.method.name != "Equals" || call.args.len() != 3 {
                continue;
            }
            let Some(member) = self.member_access_on_this(f, call.args[1], all_members) else {
                continue;
            };
            if !order.contains(&member) {
                order.push(member);
            }
        }
        Ok((!order.is_empty()).then_some(order))
    }

    /// `DetectOrderFromPrintMembers`: the members the generated `PrintMembers` prints, in
    /// order, named by the constants it appends ("Name = ", ", Name = ").
    fn detect_order_from_print_members(
        &mut self,
        all_members: &[Member],
    ) -> Result<Option<Vec<Member>>, String> {
        let Some(&(print_members, _)) = self.single_parameter_method("PrintMembers")?.first()
        else {
            return Ok(None);
        };
        let Some(r) = self.body(print_members)? else {
            return Ok(None);
        };
        let f = &r.function;
        let Some(builder) = r.variables.iter().copied().find(|&v| {
            let var = f.var(v);
            var.kind == VariableKind::Parameter && var.index == Some(0)
        }) else {
            return Ok(None);
        };
        let mut order = Vec::new();
        // The name and the " = " after it are one constant for a current compiler and separate
        // appends for an older one, so consecutive constants are joined before being read.
        let mut pending: Option<Vec<u16>> = None;
        for &instruction in entry_instructions(f) {
            if let Some(text) = self.string_builder_append_constant(f, instruction, builder) {
                pending.get_or_insert_with(Vec::new).extend_from_slice(text);
                continue;
            }
            if let Some(text) = pending.take()
                && !self.add_member_named_by(&text, all_members, &mut order)
            {
                return Ok(None);
            }
        }
        if let Some(text) = pending
            && !self.add_member_named_by(&text, all_members, &mut order)
        {
            return Ok(None);
        }
        Ok((!order.is_empty()).then_some(order))
    }

    /// `MatchStringBuilderAppend` with an `ldstr` value: `callvirt StringBuilder.Append(ldloc
    /// builder, ldstr text)`.
    fn string_builder_append_constant<'f>(
        &self,
        f: &'f Function,
        inst: NodeId,
        builder: crate::il::ast::VarId,
    ) -> Option<&'f [u16]> {
        let Inst::CallVirt(call) = f.inst(inst) else {
            return None;
        };
        let method = &call.method.method;
        if method.name != "Append"
            || !method.declaring.is_some_and(|d| {
                self.ts
                    .name_and_namespace(d)
                    .is_some_and(|(n, ns)| n == "StringBuilder" && ns == "System.Text")
            })
            || call.args.len() != 2
            || f.match_ldloc(call.args[0]) != Some(builder)
        {
            return None;
        }
        match f.inst(call.args[1]) {
            Inst::LdStr { value } => Some(value),
            _ => None,
        }
    }

    /// `AddMemberNamedBy`: false where the text names a member the record does not have.
    fn add_member_named_by(
        &self,
        text: &[u16],
        all_members: &[Member],
        order: &mut Vec<Member>,
    ) -> bool {
        let text = String::from_utf16_lossy(text);
        let Some(name) = text.strip_suffix(" = ") else {
            return true; // the separator alone, or a constant that names nothing
        };
        let name = name.strip_prefix(", ").unwrap_or(name);
        let Some(&member) = all_members.iter().find(|&&m| self.member_name(m) == name) else {
            return false;
        };
        if !order.contains(&member) {
            order.push(member);
        }
        true
    }

    fn member_name(&self, member: Member) -> String {
        let md = &self.ts.comp.module(self.module()).md;
        let name = match member {
            Member::Property(r) => md.get::<rows::Property>(r).map(|p| p.name),
            Member::Field(r) => md.get::<rows::Field>(r).map(|f| f.name),
        };
        name.ok()
            .and_then(|n| md.string(n).ok())
            .map(|n| n.into_owned())
            .unwrap_or_default()
    }

    /// `MatchMemberAccessOnThis`: the member a generated body reads off `this`, a field
    /// directly or the property a backing field belongs to.
    fn member_access_on_this(
        &mut self,
        f: &Function,
        inst: NodeId,
        all_members: &[Member],
    ) -> Option<Member> {
        let field = f
            .match_ldfld(self.ts, inst)
            .or_else(|| f.match_ldflda(inst));
        if let Some((target, field)) = field {
            if !f.match_ld_this(target) {
                return None;
            }
            let (row, _, _) = self.own_field(&field.field)?;
            if let Some(&property) = self.backing_field_to_auto_property.get(&row) {
                return Some(Member::Property(property));
            }
            return all_members
                .iter()
                .copied()
                .find(|&m| m == Member::Field(row));
        }
        let (Inst::Call(call) | Inst::CallVirt(call)) = f.inst(inst) else {
            return None;
        };
        if call.args.len() != 1 || !f.match_ld_this(call.args[0]) {
            return None;
        }
        let MethodOrigin::Def(method) = call.method.method.origin else {
            return None;
        };
        if method.module != self.module() {
            return None;
        }
        let comp = self.ts.comp;
        let md = &comp.module(self.module()).md;
        all_members.iter().copied().find(|&m| match m {
            Member::Property(p) => {
                let (get, _, _) = md.property_accessors(p);
                get != 0 && get == method.row
            }
            Member::Field(_) => false,
        })
    }
}

/// The instructions of the function's entry block (`Block.Instructions`, without the final
/// instruction).
fn entry_instructions(f: &Function) -> &[NodeId] {
    let Some(body) = f.body else {
        return &[];
    };
    let Inst::BlockContainer { blocks, .. } = f.inst(body) else {
        return &[];
    };
    match blocks.first().map(|&b| f.inst(b)) {
        Some(Inst::Block { instructions, .. }) => instructions,
        _ => &[],
    }
}

/// `UnpackLogicAndChain`.
fn unpack_logic_and_chain(f: &Function, inst: NodeId, result: &mut Vec<NodeId>) {
    if let Some((lhs, rhs)) = crate::il::transforms::expression_transforms::match_logic_and(f, inst)
    {
        unpack_logic_and_chain(f, lhs, result);
        unpack_logic_and_chain(f, rhs, result);
    } else {
        result.push(inst);
    }
}

/// `MergeAlongCommonMembers`: merges two orderings of overlapping subsets; where they disagree,
/// `first` (the order the equality members follow) wins.
fn merge_along_common_members(first: &[Member], second: &[Member]) -> Vec<Member> {
    let mut result = Vec::new();
    let (mut i, mut j) = (0, 0);
    while i < first.len() && j < second.len() {
        if first[i] == second[j] {
            result.push(first[i]);
            i += 1;
            j += 1;
        } else if !second[j..].contains(&first[i]) {
            result.push(first[i]);
            i += 1;
        } else if !first[i..].contains(&second[j]) {
            result.push(second[j]);
            j += 1;
        } else {
            result.push(first[i]);
            i += 1;
        }
    }
    for &member in first[i..].iter().chain(&second[j..]) {
        if !result.contains(&member) {
            result.push(member);
        }
    }
    result
}

#[cfg(test)]
mod tests {
    use super::{Member, merge_along_common_members};

    #[test]
    fn merge_interleaves_members_only_one_order_has() {
        let (a, b, c, d) = (
            Member::Field(1),
            Member::Property(1),
            Member::Field(2),
            Member::Property(2),
        );
        // Equals sees a private field PrintMembers skips; PrintMembers a property Equals skips.
        assert_eq!(
            merge_along_common_members(&[a, b, d], &[b, c, d]),
            vec![a, b, c, d]
        );
        // Opposite orders: the equality order wins.
        assert_eq!(merge_along_common_members(&[a, b], &[b, a]), vec![a, b]);
        assert_eq!(merge_along_common_members(&[], &[c]), vec![c]);
    }
}
