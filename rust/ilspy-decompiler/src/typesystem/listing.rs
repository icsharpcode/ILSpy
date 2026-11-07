//! A canonical listing of the main module's type definitions and members as the type system
//! sees them.
//!
//! `ICSharpCode.Decompiler.Rust.Tests/TypeSystemListingEqualityTests.cs` produces the same
//! format from the link-compiled `DecompilerTypeSystem` (`TypeSystemOptions.None`);
//! `TypeSystemListingEquality` diffs the two. Any change here must be mirrored there.
//!
//! Types print as `<reflection name>@<defining assembly or ?>`, tokens as 8 hex digits, `-`
//! for none. Lines, in this order:
//! - per TypeDef row: `T <name> kind=<kind> known=<code> decl=<name|-> tparams=<n>
//!   enum=<type|-> base=[a|b]`, followed by `G <name> [<tp>|<tp>]` when it has type parameters;
//! - per Field row: `F <tok> decl=<name|-> name=<n> access=<a> mods=<srvc> type=<type>
//!   value=<v>`;
//! - per MethodDef row: `M <tok> decl=<name|-> name=<n> kind=<k> access=<a> mods=<saevob>
//!   accessor=<tok>:<semantics>|- ret=<type> params=[<p>|<p>] tparams=[<tp>|<tp>]`;
//! - per Property row: `P <tok> decl=<name|-> name=<n> get=<tok|-> set=<tok|-> ret=<type>
//!   params=[...]`;
//! - per Event row: `E <tok> decl=<name|-> name=<n> add=<tok|-> remove=<tok|-> invoke=<tok|->
//!   type=<type>`.
//!
//! `<p>` is `<type>,<name>,<ref kind>,<optional 0|1>,<constant in signature 0|1>,<v>`;
//! `<tp>` is `<name>:<variance>:<constraint flags nrvl>:{<type>;<type>}`; `<v>` is `null` or
//! `<CLR type name>:<value>` with floats as bit patterns, strings and decimals as hex words.
//! A line that cannot be produced prints `<kind letter> <tok> !bad`.

use std::fmt::Write;

use super::compilation::{Compilation, ModuleId, TypeDefId, TypeKind};
use super::members::{
    ConstValue, MethodDefId, ParamInfo, TypeParamInfo, field_accessibility, has_body,
    method_accessibility, method_modifiers,
};
use super::types::{TypeData, TypeId, TypeSystem};
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
                    let tps = ts
                        .type_parameters(main, TableId::TypeDef, row)
                        .map(|tps| type_params(&ts, &tps));
                    line(format!(
                        "G {} {}",
                        info.full_name.reflection_name(),
                        or_bad(tps)
                    ));
                }
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
    out
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
    let module = ts
        .definition(t)
        .map_or("?", |def| ts.comp.module(def.module).assembly_name.as_str());
    format!("{}@{module}", ts.reflection_name(t))
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
    format!(
        "T {} kind={} known={} decl={decl} tparams={} enum={underlying} base={bases}",
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
                "{},{},{},{},{},{}",
                located(ts, p.ty),
                p.name,
                p.ref_kind.name(),
                u8::from(p.optional),
                u8::from(p.has_constant_in_signature),
                value(&p.value)
            )
        })
        .collect();
    format!("[{}]", list.join("|"))
}

fn type_params(ts: &TypeSystem<'_, '_>, tps: &[TypeParamInfo]) -> String {
    let list: Vec<String> = tps
        .iter()
        .map(|tp| {
            let mut flags = String::new();
            for (set, letter) in [
                (tp.default_ctor, 'n'),
                (tp.reference_type, 'r'),
                (tp.value_type, 'v'),
                (tp.allows_ref_like, 'l'),
            ] {
                if set {
                    flags.push(letter);
                }
            }
            let constraints: Vec<String> = tp.constraints.iter().map(|c| located(ts, *c)).collect();
            format!(
                "{}:{}:{flags}:{{{}}}",
                tp.name,
                tp.variance,
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
        (f.flags & 0x0040 != 0, 'c'),
    ] {
        if set {
            mods.push(letter);
        }
    }
    let constant = ts.field_constant(module, row);
    Ok(format!(
        "F {} decl={} name={name} access={} mods={mods} type={} value={}",
        tok(token(TableId::Field, row)),
        decl_name(ts, module, declaring),
        field_accessibility(f.flags),
        located(ts, ty),
        value(&constant)
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
    let accessor = accessor.map_or_else(|| "-".to_owned(), |(t, sem)| format!("{}:{sem}", tok(t)));
    Ok(format!(
        "M {} decl={} name={name} kind={} access={} mods={mods} accessor={accessor} ret={} params={} tparams={}",
        tok(token(TableId::MethodDef, m.row)),
        decl_name(ts, m.module, declaring),
        kind.name(),
        method_accessibility(def.flags),
        located(ts, ret),
        params(ts, &ps),
        type_params(ts, &tps)
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
    Ok(format!(
        "P {} decl={} name={name} get={} set={} ret={} params={}",
        tok(token(TableId::Property, row)),
        decl_name(ts, module, declaring),
        opt_tok(TableId::MethodDef, get),
        opt_tok(TableId::MethodDef, set),
        located(ts, ret),
        params(ts, &ps)
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
    Ok(format!(
        "E {} decl={} name={name} add={} remove={} invoke={} type={}",
        tok(token(TableId::Event, row)),
        decl_name(ts, module, declaring),
        opt_tok(TableId::MethodDef, add),
        opt_tok(TableId::MethodDef, remove),
        opt_tok(TableId::MethodDef, raise),
        located(ts, ty)
    ))
}
