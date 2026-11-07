//! A canonical listing of the main module's type definitions and members as the type system
//! sees them.
//!
//! `ICSharpCode.Decompiler.Rust.Tests/TypeSystemListingEqualityTests.cs` produces the same
//! format from the link-compiled `DecompilerTypeSystem` (`TypeSystemOptions.None`);
//! `TypeSystemListingEquality` diffs the two. Any change here must be mirrored there.
//!
//! Types print structurally (`describe` in the C# test): parameterized types as
//! `<generic>[<args>]`, tuples as `(<t>:<name>,...)`, nullability as a trailing `?` or `!`,
//! function pointers as `delegate*[<conv>;<custom>;<ret>[ ro];<p>:<ref kind>,...]@<asm>`, and
//! everything else as `<reflection name>@<defining assembly or ?>`. Tokens print as 8 hex digits, `-`
//! for none. Lines, in this order:
//! - per TypeDef row: `T <name> kind=<kind> known=<code> decl=<name|-> tparams=<n>
//!   enum=<type|-> base=[a|b] tflags=<brxs>`, followed by `G <name> [<tp>|<tp>]` when it has type parameters;
//! - per Field row: `F <tok> decl=<name|-> name=<n> access=<a> mods=<srvc> type=<type>
//!   value=<v> retro=<0|1>`;
//! - per MethodDef row: `M <tok> decl=<name|-> name=<n> kind=<k> access=<a> mods=<saevob>
//!   accessor=<tok>:<semantics>|- ret=<type> params=[<p>|<p>] tparams=[<tp>|<tp>]
//!   ext= thisro= retro= init=<0|1> thisscope=<scope>`;
//! - per Property row: `P <tok> decl=<name|-> name=<n> get=<tok|-> set=<tok|-> ret=<type>
//!   params=[...] retro=<0|1>`;
//! - per Event row: `E <tok> decl=<name|-> name=<n> add=<tok|-> remove=<tok|-> invoke=<tok|->
//!   type=<type>`.
//! - per MemberRef row: `R <tok> <member>`, per MethodSpec row: `S <tok> <member>`, resolved
//!   in an empty generic context as the IL reader would (`ResolveMethod`, varargs expanded;
//!   fields through `ResolveEntity`). A method prints `method=<def tok|fake> decl=<type|->
//!   name=<n> static=<0|1> targs=[...] ret=<type> params=[...] accessor=<semantics>`, a field
//!   `field=<def tok|fake> decl=<type|-> name=<n> type=<type>`.
//! - per CustomAttribute row: `A <tok> fixed=[<arg>|...] named=[<F|P>:<name>:<arg>|...]`, the
//!   value `CustomAttribute.DecodeValue(module.TypeProvider)` returns; `<arg>` is
//!   `<type>=<v>`, `<type>=T:<type>` for a `System.Type` value or `<type>=[<arg>|...]` for an
//!   array.
//!
//! `<p>` is `<type>,<name>,<ref kind>,<optional 0|1>,<constant in signature 0|1>,<v>,
//! <params 0|1>,<declared scope>,<unscoped ref 0|1>,<effective scope>`;
//! `<tp>` is `<name>:<variance>:<constraint flags nrvlu>:<nullability>:{<type>;<type>}`; `<v>` is `null` or
//! `<CLR type name>:<value>` with floats as bit patterns, strings and decimals as hex words.
//! A line that cannot be produced prints `<kind letter> <tok> !bad`.

use std::fmt::Write;

use super::attributes::Attribute;
use super::attrs::accessibility_name;
use super::attrvalue::{ArgValue, AttributeValue, TypedArg};
use super::compilation::{Compilation, ModuleId, TypeDefId, TypeKind};
use super::flags::has_compiler_attribute;
use super::inheritance::{MemberDef, MemberRef, Modifiers};
use super::lookup::MemberOptions;
use super::members::{
    ConstValue, MethodDefId, ParamInfo, TypeParamInfo, field_accessibility, has_body,
    method_accessibility, method_modifiers,
};
use super::resolve::{FieldOrigin, MethodOrigin, ResolvedMethod};
use super::types::{GenericContext, Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};
use crate::metadata::Result;
use crate::metadata::tables::{TableId, rows, token};

pub fn listing(comp: &Compilation<'_>) -> String {
    let mut ts = TypeSystem::new(comp);
    let main = ModuleId(0);
    let md = &comp.module(main).md;
    let mut out = String::new();
    let mut line = |s: String| {
        out.push_str(&s);
        out.push('\n');
    };
    for row in 1..=md.tables.rows(TableId::TypeDef) {
        let def = TypeDefId { module: main, row };
        match comp.type_def(def) {
            Err(_) => line("T !bad".to_owned()),
            Ok(info) => {
                line(type_line(&mut ts, def, info));
                if info.type_parameter_count > 0 {
                    let tps = ts.type_parameters(main, TableId::TypeDef, row).map(|tps| {
                        let owners: Vec<TypeParamOwner> = (0..tps.len() as u32)
                            .map(|i| ts.class_param_owner(def, i))
                            .collect();
                        type_params(&mut ts, &tps, &owners)
                    });
                    line(format!(
                        "G {} {}",
                        info.full_name.reflection_name(),
                        or_bad(tps)
                    ));
                }
                let bases = base_type_line(&mut ts, def);
                line(format!(
                    "B {} {}",
                    info.full_name.reflection_name(),
                    or_bad(bases)
                ));
                let lookup = lookup_line(&mut ts, def);
                line(format!(
                    "K {} {}",
                    info.full_name.reflection_name(),
                    or_bad(lookup)
                ));
            }
        }
    }
    for row in 1..=md.tables.rows(TableId::Field) {
        line(with_bad(
            "F",
            token(TableId::Field, row),
            field_line(&mut ts, main, row),
        ));
    }
    for row in 1..=md.tables.rows(TableId::MethodDef) {
        let m = MethodDefId { module: main, row };
        line(with_bad(
            "M",
            token(TableId::MethodDef, row),
            method_line(&mut ts, m),
        ));
    }
    for row in 1..=md.tables.rows(TableId::Property) {
        line(with_bad(
            "P",
            token(TableId::Property, row),
            property_line(&mut ts, main, row),
        ));
    }
    for row in 1..=md.tables.rows(TableId::Event) {
        line(with_bad(
            "E",
            token(TableId::Event, row),
            event_line(&mut ts, main, row),
        ));
    }
    for row in 1..=md.tables.rows(TableId::MemberRef) {
        let t = token(TableId::MemberRef, row);
        line(with_bad(
            "R",
            t,
            member_ref_line(&mut ts, main, row).map(|m| format!("R {} {m}", tok(t))),
        ));
    }
    for row in 1..=md.tables.rows(TableId::MethodSpec) {
        let t = token(TableId::MethodSpec, row);
        let resolved = ts
            .resolve_method(main, t, GenericContext::default())
            .and_then(|m| method_ref(&ts, &m));
        line(with_bad(
            "S",
            t,
            resolved.map(|m| format!("S {} {m}", tok(t))),
        ));
    }
    for row in 1..=md.tables.rows(TableId::CustomAttribute) {
        let t = token(TableId::CustomAttribute, row);
        let decoded = ts
            .decode_attribute(main, row)
            .map(|v| attribute_line(&ts, t, &v));
        line(with_bad("A", t, decoded));
    }
    for row in 1..=md.tables.rows(TableId::TypeDef) {
        let def = TypeDefId { module: main, row };
        let name = comp
            .type_def(def)
            .map_or_else(|_| "!bad".to_owned(), |i| i.full_name.reflection_name());
        line(format!(
            "LT {name} {}",
            or_bad(type_attribute_line(&mut ts, def))
        ));
    }
    for row in 1..=md.tables.rows(TableId::MethodDef) {
        let m = MethodDefId { module: main, row };
        let t = token(TableId::MethodDef, row);
        let l = method_attribute_line(&mut ts, m).map(|l| format!("LM {} {l}", tok(t)));
        line(with_bad("LM", t, l));
    }
    for row in 1..=md.tables.rows(TableId::Field) {
        let t = token(TableId::Field, row);
        let l = ts
            .field_attributes(main, row)
            .map(|a| format!("LF {} {}", tok(t), attribute_list(&mut ts, &a)));
        line(with_bad("LF", t, l));
    }
    for row in 1..=md.tables.rows(TableId::Property) {
        let t = token(TableId::Property, row);
        let l = property_attribute_line(&mut ts, main, row).map(|l| format!("LP {} {l}", tok(t)));
        line(with_bad("LP", t, l));
    }
    for row in 1..=md.tables.rows(TableId::Event) {
        let t = token(TableId::Event, row);
        let l = ts
            .event_attributes(main, row)
            .map(|a| format!("LE {} {}", tok(t), attribute_list(&mut ts, &a)));
        line(with_bad("LE", t, l));
    }
    let assembly = ts
        .assembly_attributes(main)
        .map(|a| attribute_list(&mut ts, &a));
    line(format!("LA {}", or_bad(assembly)));
    let module = ts
        .module_attributes(main)
        .map(|a| attribute_list(&mut ts, &a));
    line(format!("LO {}", or_bad(module)));
    out
}

/// One attribute: `<type>(<constructor>) fixed=[...] named=[...] err=<0|1>`.
fn attribute_entry(ts: &mut TypeSystem<'_, '_>, a: &Attribute) -> String {
    let ctor = match &a.ctor {
        Some(c) => method_origin(c),
        None => match ts.builtin_constructor(a) {
            Ok(Some(c)) => method_origin(&c),
            Ok(None) => "-".to_owned(),
            Err(_) => "!bad".to_owned(),
        },
    };
    let value = AttributeValue {
        fixed: a.fixed.clone(),
        named: a.named.clone(),
    };
    let args = attribute_line(ts, 0, &value);
    let args = args.strip_prefix("A 00000000 ").unwrap_or(&args);
    format!(
        "{}({ctor}) {args} err={}",
        located(ts, a.ty),
        u8::from(a.has_decode_errors)
    )
}

fn attribute_list(ts: &mut TypeSystem<'_, '_>, list: &[Attribute]) -> String {
    let entries: Vec<String> = list.iter().map(|a| attribute_entry(ts, a)).collect();
    format!("[{}]", entries.join("|"))
}

fn type_parameter_attributes(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    table: TableId,
    row: u32,
) -> Result<String> {
    let md = &ts.comp.module(module).md;
    let rows = md.generic_params(table, row);
    let mut lists = Vec::new();
    for (index, mut gp) in rows.into_iter().enumerate() {
        // A nested type shares the type parameters of the types enclosing it.
        if table == TableId::TypeDef
            && let TypeParamOwner::Type(owner) =
                ts.class_param_owner(TypeDefId { module, row }, index as u32)
            && let Some(outer) = md.generic_params(TableId::TypeDef, owner.row).nth(index)
        {
            gp = outer;
        }
        let attrs = ts.type_parameter_attributes(module, gp)?;
        lists.push(attribute_list(ts, &attrs));
    }
    Ok(format!("[{}]", lists.join(",")))
}

fn parameter_attribute_lists(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    params: &[ParamInfo],
    declaring: Option<TypeDefId>,
) -> Result<String> {
    let mut lists = Vec::new();
    for i in 0..params.len() {
        let attrs = ts.parameter_attributes(module, params, i, declaring)?;
        lists.push(attribute_list(ts, &attrs));
    }
    Ok(format!("[{}]", lists.join(",")))
}

fn type_attribute_line(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> Result<String> {
    let attrs = ts.type_attributes(def)?;
    let tps = type_parameter_attributes(ts, def.module, TableId::TypeDef, def.row)?;
    Ok(format!("{} tparams={tps}", attribute_list(ts, &attrs)))
}

fn method_attribute_line(ts: &mut TypeSystem<'_, '_>, m: MethodDefId) -> Result<String> {
    let attrs = ts.method_attributes(m)?;
    let ret = ts.return_type_attributes(m)?;
    let (_, params) = ts.method_signature(m);
    let declaring = ts.method_declaring(m)?;
    let params = parameter_attribute_lists(ts, m.module, &params, declaring)?;
    let tps = type_parameter_attributes(ts, m.module, TableId::MethodDef, m.row)?;
    Ok(format!(
        "{} ret={} params={params} tparams={tps}",
        attribute_list(ts, &attrs),
        attribute_list(ts, &ret)
    ))
}

fn property_attribute_line(
    ts: &mut TypeSystem<'_, '_>,
    module: ModuleId,
    row: u32,
) -> Result<String> {
    let attrs = ts.property_attributes(module, row)?;
    let (_, params) = ts.property_signature(module, row)?;
    let declaring = match ts.property_any_accessor(module, row) {
        Some(a) => ts.method_declaring(a)?,
        None => None,
    };
    let params = parameter_attribute_lists(ts, module, &params, declaring)?;
    Ok(format!("{} params={params}", attribute_list(ts, &attrs)))
}

fn types(ts: &TypeSystem<'_, '_>, list: &[TypeId]) -> String {
    let names: Vec<String> = list.iter().map(|t| located(ts, *t)).collect();
    format!("[{}]", names.join("|"))
}

fn method_origin(m: &ResolvedMethod) -> String {
    match m.origin {
        MethodOrigin::Def(d) => tok(token(TableId::MethodDef, d.row)),
        MethodOrigin::DummyConstructor(_) | MethodOrigin::Fake => "fake".to_owned(),
    }
}

fn method_refs(ts: &TypeSystem<'_, '_>, list: &[ResolvedMethod]) -> String {
    let refs: Vec<String> = list
        .iter()
        .map(|m| {
            let decl = m
                .declaring
                .map_or_else(|| "-".to_owned(), |d| located(ts, d));
            format!("{}:{decl}", method_origin(m))
        })
        .collect();
    format!("[{}]", refs.join("|"))
}

fn member_refs(ts: &TypeSystem<'_, '_>, list: &[MemberRef]) -> String {
    let refs: Vec<String> = list
        .iter()
        .map(|m| {
            let t = match m.def {
                MemberDef::Field(_, row) => token(TableId::Field, row),
                MemberDef::Method(id) => token(TableId::MethodDef, id.row),
                MemberDef::Property(_, row) => token(TableId::Property, row),
                MemberDef::Event(_, row) => token(TableId::Event, row),
            };
            format!("{}:{}", tok(t), located(ts, m.declaring))
        })
        .collect();
    format!("[{}]", refs.join("|"))
}

fn inheritance(overridable: bool, explicit: bool, impls: String) -> String {
    format!(
        "ovr={} eii={} impls={impls}",
        u8::from(overridable),
        u8::from(explicit)
    )
}

fn modifier_letters(m: &Modifiers) -> String {
    letters(&[
        (m.is_static, 's'),
        (m.is_abstract, 'a'),
        (m.is_sealed, 'e'),
        (m.is_virtual, 'v'),
        (m.is_override, 'o'),
    ])
}

fn base_type_line(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> Result<String> {
    let t = ts.intern_def(def);
    let all = ts.all_base_types(t)?;
    let non_interface = ts.non_interface_base_types(t)?;
    let dmn = ts.default_member_name(def)?;
    Ok(format!(
        "all={} nonif={} dmn={}",
        types(ts, &all),
        types(ts, &non_interface),
        dmn.as_deref().unwrap_or("-")
    ))
}

fn lookup_line(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> Result<String> {
    let t = ts.intern_def(def);
    let all = MemberOptions::default();
    let nested = ts.get_nested_types(t, None, all)?;
    let ctors = ts.get_constructors(t, MemberOptions::DECLARED)?;
    let methods = ts.get_methods(t, None, all)?;
    let props = ts.get_properties(t, all)?;
    let fields = ts.get_fields(t, all)?;
    let events = ts.get_events(t, all)?;
    let invoke = ts
        .get_delegate_invoke_method(t)?
        .map_or_else(|| "-".to_owned(), |m| method_refs(ts, &[m]));
    Ok(format!(
        "nested={} ctors={} methods={} props={} fields={} events={} invoke={invoke}",
        types(ts, &nested),
        method_refs(ts, &ctors),
        method_refs(ts, &methods),
        member_refs(ts, &props),
        member_refs(ts, &fields),
        member_refs(ts, &events)
    ))
}

fn method_ref(ts: &TypeSystem<'_, '_>, m: &ResolvedMethod) -> Result<String> {
    let origin = method_origin(m);
    Ok(format!(
        "method={origin} decl={} name={} static={} targs={} ret={} params={} accessor={}",
        m.declaring
            .map_or_else(|| "-".to_owned(), |d| located(ts, d)),
        m.name,
        u8::from(m.is_static),
        types(ts, &m.type_args),
        located(ts, m.ret),
        types(ts, &m.params()?),
        m.accessor_kind
    ))
}

fn member_ref_line(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> Result<String> {
    let md = &ts.comp.module(module).md;
    let mr = md.get::<rows::MemberRef>(row)?;
    let header = *md
        .blob(mr.signature)?
        .first()
        .ok_or(crate::metadata::Error::Truncated)?;
    let ctx = GenericContext::default();
    match crate::metadata::signature::header_kind(header) {
        crate::metadata::signature::HEADER_KIND_FIELD => {
            let f = ts.resolve_field_ref(module, row, ctx)?;
            let origin = match f.origin {
                FieldOrigin::Def { row, .. } => tok(token(TableId::Field, row)),
                FieldOrigin::Fake => "fake".to_owned(),
            };
            Ok(format!(
                "field={origin} decl={} name={} type={}",
                f.declaring
                    .map_or_else(|| "-".to_owned(), |d| located(ts, d)),
                f.name,
                located(ts, f.ty)
            ))
        }
        crate::metadata::signature::HEADER_KIND_METHOD => {
            let m = ts.resolve_method(module, token(TableId::MemberRef, row), ctx)?;
            method_ref(ts, &m)
        }
        _ => Err(crate::metadata::Error::Malformed("member reference kind")),
    }
}

fn attribute_line(ts: &TypeSystem<'_, '_>, t: u32, v: &AttributeValue) -> String {
    let fixed: Vec<String> = v.fixed.iter().map(|a| attribute_argument(ts, a)).collect();
    let named: Vec<String> = v
        .named
        .iter()
        .map(|n| {
            let kind = if n.is_field { 'F' } else { 'P' };
            let name = n.name.as_deref().unwrap_or("");
            format!("{kind}:{name}:{}", attribute_argument(ts, &n.arg))
        })
        .collect();
    format!(
        "A {} fixed=[{}] named=[{}]",
        tok(t),
        fixed.join("|"),
        named.join("|")
    )
}

fn attribute_argument(ts: &TypeSystem<'_, '_>, a: &TypedArg) -> String {
    let v = match &a.value {
        ArgValue::Null => "null".to_owned(),
        ArgValue::Bool(b) => value(&ConstValue::Boolean(*b)),
        ArgValue::Char(c) => value(&ConstValue::Char(*c)),
        ArgValue::SByte(x) => value(&ConstValue::SByte(*x)),
        ArgValue::Byte(x) => value(&ConstValue::Byte(*x)),
        ArgValue::Int16(x) => value(&ConstValue::Int16(*x)),
        ArgValue::UInt16(x) => value(&ConstValue::UInt16(*x)),
        ArgValue::Int32(x) => value(&ConstValue::Int32(*x)),
        ArgValue::UInt32(x) => value(&ConstValue::UInt32(*x)),
        ArgValue::Int64(x) => value(&ConstValue::Int64(*x)),
        ArgValue::UInt64(x) => value(&ConstValue::UInt64(*x)),
        ArgValue::Single(bits) => value(&ConstValue::Single(*bits)),
        ArgValue::Double(bits) => value(&ConstValue::Double(*bits)),
        ArgValue::String(units) => value(&ConstValue::String(units.clone())),
        ArgValue::Decimal(words) => value(&ConstValue::Decimal(*words)),
        ArgValue::Type(t) => format!("T:{}", located(ts, *t)),
        ArgValue::Array(items) => {
            let items: Vec<String> = items.iter().map(|i| attribute_argument(ts, i)).collect();
            format!("[{}]", items.join("|"))
        }
    };
    format!("{}={v}", located(ts, a.ty))
}

fn with_bad(kind: &str, t: u32, r: Result<String>) -> String {
    r.unwrap_or_else(|_| format!("{kind} {} !bad", tok(t)))
}

fn or_bad(r: Result<String>) -> String {
    r.unwrap_or_else(|_| "!bad".to_owned())
}

fn tok(t: u32) -> String {
    format!("{t:08x}")
}

fn opt_tok(table: TableId, row: u32) -> String {
    if row == 0 {
        "-".to_owned()
    } else {
        tok(token(table, row))
    }
}

/// `name@assembly` of the defining module, `name@?` when the type resolved to nothing.
fn located(ts: &TypeSystem<'_, '_>, t: TypeId) -> String {
    match ts.data(t) {
        TypeData::Tuple {
            elements, names, ..
        } => {
            let parts: Vec<String> = elements
                .iter()
                .zip(names.iter())
                .map(|(e, n)| match n {
                    Some(n) => format!("{}:{n}", located(ts, *e)),
                    None => located(ts, *e),
                })
                .collect();
            format!("({})", parts.join(","))
        }
        TypeData::Annotated { inner, nullability } => {
            format!("{}{}", located(ts, *inner), mark(*nullability))
        }
        TypeData::Inst { generic, args } => {
            let args: Vec<String> = args.iter().map(|a| located(ts, *a)).collect();
            format!("{}[{}]", located(ts, *generic), args.join(","))
        }
        TypeData::Array {
            elem,
            rank,
            nullability,
        } => format!(
            "{}[{}]{}",
            located(ts, *elem),
            ",".repeat(rank.saturating_sub(1) as usize),
            mark(*nullability)
        ),
        TypeData::Pointer(e) => format!("{}*", located(ts, *e)),
        TypeData::ByRef(e) => format!("{}&", located(ts, *e)),
        TypeData::Pinned(e) => format!("{} pinned", located(ts, *e)),
        TypeData::FnPtr(f) => {
            let custom: Vec<String> = f
                .custom_conventions
                .iter()
                .map(|c| located(ts, *c))
                .collect();
            let params: Vec<String> = f
                .params
                .iter()
                .zip(f.ref_kinds.iter())
                .map(|(p, k)| format!("{}:{k}", located(ts, *p)))
                .collect();
            format!(
                "delegate*[{};{};{}{};{}]@{}",
                f.calling_convention,
                custom.join(","),
                located(ts, f.ret),
                if f.ret_ref_readonly { " ro" } else { "" },
                params.join(","),
                defining_assembly(ts, t)
            )
        }
        _ => format!("{}@{}", ts.reflection_name(t), defining_assembly(ts, t)),
    }
}

fn defining_assembly<'x>(ts: &'x TypeSystem<'_, '_>, t: TypeId) -> &'x str {
    ts.definition(t)
        .map_or("?", |def| ts.comp.module(def.module).assembly_name.as_str())
}

fn mark(n: Nullability) -> &'static str {
    match n {
        Nullability::Oblivious => "",
        Nullability::NotNullable => "!",
        Nullability::Nullable => "?",
    }
}

fn decl_name(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> String {
    if row == 0 {
        return "-".to_owned();
    }
    let t = ts.intern(TypeData::Def(TypeDefId { module, row }));
    ts.reflection_name(t)
}

fn type_line(
    ts: &mut TypeSystem<'_, '_>,
    def: TypeDefId,
    info: &super::compilation::TypeDefInfo,
) -> String {
    let decl = decl_name(ts, def.module, info.declaring);
    let underlying = match (info.kind, info.enum_underlying) {
        (TypeKind::Enum, Some(code)) => {
            let t = ts.known_type(code);
            located(ts, t)
        }
        _ => "-".to_owned(),
    };
    let bases = or_bad(ts.direct_base_types(def).map(|bases| {
        let names: Vec<String> = bases.iter().map(|b| located(ts, *b)).collect();
        format!("[{}]", names.join("|"))
    }));
    let f = ts.type_flags(def);
    let tflags = letters(&[
        (f.by_ref_like, 'b'),
        (f.read_only, 'r'),
        (f.has_extensions, 'x'),
        (f.is_static, 's'),
    ]);
    format!(
        "T {} kind={} known={} decl={decl} tparams={} enum={underlying} base={bases} tflags={tflags}",
        info.full_name.reflection_name(),
        info.kind.name(),
        info.known.name(),
        info.type_parameter_count
    )
}

fn value(v: &ConstValue) -> String {
    match v {
        ConstValue::Null => "null".to_owned(),
        ConstValue::Boolean(b) => format!("Boolean:{}", if *b { "True" } else { "False" }),
        ConstValue::Char(c) => format!("Char:{c}"),
        ConstValue::SByte(x) => format!("SByte:{x}"),
        ConstValue::Byte(x) => format!("Byte:{x}"),
        ConstValue::Int16(x) => format!("Int16:{x}"),
        ConstValue::UInt16(x) => format!("UInt16:{x}"),
        ConstValue::Int32(x) => format!("Int32:{x}"),
        ConstValue::UInt32(x) => format!("UInt32:{x}"),
        ConstValue::Int64(x) => format!("Int64:{x}"),
        ConstValue::UInt64(x) => format!("UInt64:{x}"),
        ConstValue::Single(bits) => format!("Single:{bits:08x}"),
        ConstValue::Double(bits) => format!("Double:{bits:016x}"),
        ConstValue::String(units) => units.iter().fold("String:".to_owned(), |mut s, u| {
            let _ = write!(s, "{u:04x}");
            s
        }),
        ConstValue::Decimal(words) => {
            let words: Vec<String> = words.iter().map(|w| format!("{w:08x}")).collect();
            format!("Decimal:{}", words.join(","))
        }
    }
}

fn params(ts: &TypeSystem<'_, '_>, params: &[ParamInfo]) -> String {
    let list: Vec<String> = params
        .iter()
        .map(|p| {
            format!(
                "{},{},{},{},{},{},{},{},{},{}",
                located(ts, p.ty),
                p.name,
                p.ref_kind.name(),
                u8::from(p.optional),
                u8::from(p.has_constant_in_signature),
                value(&p.value),
                u8::from(p.is_params),
                p.declared_scope.name(),
                u8::from(p.unscoped_ref),
                p.effective_scope.name()
            )
        })
        .collect();
    format!("[{}]", list.join("|"))
}

fn type_params(
    ts: &mut TypeSystem<'_, '_>,
    tps: &[TypeParamInfo],
    owners: &[TypeParamOwner],
) -> String {
    let list: Vec<String> = tps
        .iter()
        .zip(owners)
        .enumerate()
        .map(|(index, (tp, &owner))| {
            let t = ts.intern(TypeData::TypeParam {
                owner,
                index: index as u32,
            });
            let reference = match ts.is_reference_type(t) {
                Some(true) => "True",
                Some(false) => "False",
                None => "-",
            };
            let mut flags = String::new();
            for (set, letter) in [
                (tp.default_ctor, 'n'),
                (tp.reference_type, 'r'),
                (tp.value_type, 'v'),
                (tp.allows_ref_like, 'l'),
                (tp.unmanaged, 'u'),
            ] {
                if set {
                    flags.push(letter);
                }
            }
            let constraints: Vec<String> = tp.constraints.iter().map(|c| located(ts, *c)).collect();
            format!(
                "{}:{}:{flags}:{}:{reference}:{{{}}}",
                tp.name,
                tp.variance,
                tp.nullability as u8,
                constraints.join(";")
            )
        })
        .collect();
    format!("[{}]", list.join("|"))
}

fn field_line(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> Result<String> {
    let md = &ts.comp.module(module).md;
    let f = md.get::<rows::Field>(row)?;
    let declaring = md.field_declaring_type(row)?;
    let name = md.string(f.name)?.into_owned();
    let (ty, volatile) = ts.field_type(module, row);
    let mut mods = String::new();
    for (set, letter) in [
        (f.flags & 0x0010 != 0, 's'),
        (f.flags & 0x0020 != 0, 'r'),
        (volatile, 'v'),
        (ts.field_is_const(module, row), 'c'),
    ] {
        if set {
            mods.push(letter);
        }
    }
    let constant = ts.field_constant(module, row);
    Ok(format!(
        "F {} decl={} name={name} access={} mods={mods} type={} value={} retro={}",
        tok(token(TableId::Field, row)),
        decl_name(ts, module, declaring),
        field_accessibility(f.flags),
        located(ts, ty),
        value(&constant),
        u8::from(has_compiler_attribute(
            md,
            TableId::Field,
            row,
            "IsReadOnlyAttribute"
        ))
    ))
}

fn method_line(ts: &mut TypeSystem<'_, '_>, m: MethodDefId) -> Result<String> {
    let md = &ts.comp.module(m.module).md;
    let def = md.get::<rows::MethodDef>(m.row)?;
    let declaring = md.method_declaring_type(m.row)?;
    let name = md.string(def.name)?.into_owned();
    let tps = ts.type_parameters(m.module, TableId::MethodDef, m.row)?;
    let accessor = ts.accessor_owner(m);
    let kind = ts.method_symbol_kind(m, accessor)?;
    let (ret, ps) = ts.method_signature(m);
    let mut mods = String::new();
    let [s, a, e, v, o] = method_modifiers(def.flags);
    for (set, letter) in [
        (s, 's'),
        (a, 'a'),
        (e, 'e'),
        (v, 'v'),
        (o, 'o'),
        (has_body(&def), 'b'),
    ] {
        if set {
            mods.push(letter);
        }
    }
    let mf = ts.method_flags(m, accessor.map(|(t, _)| t));
    let accessor = accessor.map_or_else(|| "-".to_owned(), |(t, sem)| format!("{}:{sem}", tok(t)));
    let impls = ts.explicit_implementations(m)?;
    let inheritance = inheritance(
        Modifiers::of_method_flags(def.flags).is_overridable,
        ts.is_explicit_interface_implementation(m)?,
        method_refs(ts, &impls),
    );
    let owners = vec![TypeParamOwner::Method(m); tps.len()];
    let tparams = type_params(ts, &tps, &owners);
    Ok(format!(
        "M {} decl={} name={name} kind={} access={} mods={mods} accessor={accessor} ret={} params={} tparams={} ext={} thisro={} retro={} init={} thisscope={} {inheritance}",
        tok(token(TableId::MethodDef, m.row)),
        decl_name(ts, m.module, declaring),
        kind.name(),
        method_accessibility(def.flags),
        located(ts, ret),
        params(ts, &ps),
        tparams,
        u8::from(mf.extension),
        u8::from(mf.this_read_only),
        u8::from(mf.return_read_only),
        u8::from(mf.init_only),
        mf.this_scope.name()
    ))
}

fn property_line(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> Result<String> {
    let md = &ts.comp.module(module).md;
    let p = md.get::<rows::Property>(row)?;
    let name = md.string(p.name)?.into_owned();
    let (get, set, _) = md.property_accessors(row);
    let any = if get != 0 { get } else { set };
    let declaring = if any == 0 {
        0
    } else {
        md.method_declaring_type(any)?
    };
    let (ret, ps) = ts.property_signature(module, row)?;
    let member = MemberDef::Property(module, row);
    let kind = ts.property_symbol_kind(module, row)?;
    let access = ts.member_accessibility(member)?;
    let mods = ts.member_modifiers(member)?;
    let explicit = match ts.property_any_accessor(module, row) {
        Some(a) => ts.is_explicit_interface_implementation(a)?,
        None => false,
    };
    let impls = ts.property_explicit_implementations(module, row)?;
    let bases = ts.base_members(member, false)?;
    let extra = format!(
        "kind={} access={} mods={} {} bases={}",
        kind.name(),
        accessibility_name(access),
        modifier_letters(&mods),
        inheritance(mods.is_overridable, explicit, member_refs(ts, &impls)),
        member_refs(ts, &bases)
    );
    Ok(format!(
        "P {} decl={} name={name} get={} set={} ret={} params={} retro={} {extra}",
        tok(token(TableId::Property, row)),
        decl_name(ts, module, declaring),
        opt_tok(TableId::MethodDef, get),
        opt_tok(TableId::MethodDef, set),
        located(ts, ret),
        params(ts, &ps),
        u8::from(has_compiler_attribute(
            md,
            TableId::Property,
            row,
            "IsReadOnlyAttribute"
        ))
    ))
}

fn event_line(ts: &mut TypeSystem<'_, '_>, module: ModuleId, row: u32) -> Result<String> {
    let md = &ts.comp.module(module).md;
    let e = md.get::<rows::Event>(row)?;
    let name = md.string(e.name)?.into_owned();
    let (add, remove, raise, _) = md.event_accessors(row);
    let any = [add, remove, raise]
        .into_iter()
        .find(|&r| r != 0)
        .unwrap_or(0);
    let declaring = if any == 0 {
        0
    } else {
        md.method_declaring_type(any)?
    };
    let ty = ts.event_type(module, row)?;
    let member = MemberDef::Event(module, row);
    let access = ts.member_accessibility(member)?;
    let mods = ts.member_modifiers(member)?;
    let explicit = match ts.event_any_accessor(module, row) {
        Some(a) => ts.is_explicit_interface_implementation(a)?,
        None => false,
    };
    let impls = ts.event_explicit_implementations(module, row)?;
    Ok(format!(
        "E {} decl={} name={name} add={} remove={} invoke={} type={} access={} mods={} {}",
        tok(token(TableId::Event, row)),
        decl_name(ts, module, declaring),
        opt_tok(TableId::MethodDef, add),
        opt_tok(TableId::MethodDef, remove),
        opt_tok(TableId::MethodDef, raise),
        located(ts, ty),
        accessibility_name(access),
        modifier_letters(&mods),
        inheritance(mods.is_overridable, explicit, member_refs(ts, &impls))
    ))
}

fn letters(flags: &[(bool, char)]) -> String {
    flags
        .iter()
        .filter(|(set, _)| *set)
        .map(|(_, c)| *c)
        .collect()
}
