//! `IL/Transforms/LocalFunctionDecompiler.cs`: the local function method tests other
//! transforms use.

use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{TypeDefId, TypeKind};
use crate::typesystem::members::MethodDefId;
use crate::typesystem::types::{TypeData, TypeSystem};

/// `IsLocalFunctionMethod(module, methodHandle)`: an assembly-visible method named like a
/// local function (`<caller>g__name|x_y`) that is, or is declared in, compiler-generated
/// code, or a method taking a compiler-generated display struct by reference.
pub(crate) fn is_local_function_method(ts: &mut TypeSystem<'_, '_>, method: MethodDefId) -> bool {
    let md = &ts.comp.module(method.module).md;
    let Ok(def) = md.get::<rows::MethodDef>(method.row) else {
        return false;
    };
    // `(method.Attributes & MethodAttributes.Assembly) == 0`.
    if def.flags & 0x0003 == 0 {
        return false;
    }
    let declaring = md.method_declaring_type(method.row).unwrap_or(0);
    let compiler_generated =
        ts.has_compiler_generated(method.module, TableId::MethodDef, method.row)
            || ts.has_compiler_generated(method.module, TableId::TypeDef, declaring);
    if compiler_generated
        && md
            .string(def.name)
            .is_ok_and(|n| parse_local_function_name(&n))
    {
        return true;
    }
    // Obfuscators strip the attribute and rename the method, but a compiler-generated struct
    // closure is still passed by reference to the local functions that capture it.
    has_display_struct_parameter(ts, method)
}

/// `HasDisplayStructParameter`: a by-reference parameter of a compiler-generated value type
/// with a generated name, defined in the method's module.
fn has_display_struct_parameter(ts: &mut TypeSystem<'_, '_>, method: MethodDefId) -> bool {
    let Ok(definition) = ts.method_definition(method) else {
        return false;
    };
    definition.base_params.iter().any(|&p| {
        let TypeData::ByRef(element) = *ts.data(p) else {
            return false;
        };
        let Some(def) = ts.definition(element) else {
            return false;
        };
        def.module == method.module && is_display_struct(ts, def)
    })
}

fn is_display_struct(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    let md = &ts.comp.module(def.module).md;
    ts.has_compiler_generated(def.module, TableId::TypeDef, def.row)
        && ts
            .comp
            .type_def(def)
            .is_ok_and(|i| i.kind == TypeKind::Struct)
        && md
            .get::<rows::TypeDef>(def.row)
            .ok()
            .and_then(|t| md.string(t.name).ok())
            .is_some_and(|n| n.starts_with('<') || n.contains('$'))
}

/// `ParseLocalFunctionName`: whether `name` matches `^<(.*)>g__([^|]*)\|?\d+(_\d+)?$`, the
/// newer Roslyn format `<caller>g__name|x_y` or the older `<caller>g__namex_y`.
pub(crate) fn parse_local_function_name(name: &str) -> bool {
    let Some(rest) = name.strip_prefix('<') else {
        return false;
    };
    rest.match_indices(">g__").any(|(i, _)| {
        let tail = &rest[i + 4..];
        match tail.split_once('|') {
            // The name cannot contain `|`, so the separator is the optional one.
            Some((_, numbers)) => is_digits_and_suffix(numbers),
            // Without it the name gives back trailing characters until `\d+(_\d+)?` matches,
            // which it does exactly when the tail ends in a digit.
            None => tail.ends_with(|c: char| c.is_ascii_digit()),
        }
    })
}

/// `\d+(_\d+)?` matched against the whole of `s`.
fn is_digits_and_suffix(s: &str) -> bool {
    let digits = |s: &str| !s.is_empty() && s.bytes().all(|b| b.is_ascii_digit());
    match s.split_once('_') {
        Some((a, b)) => digits(a) && digits(b),
        None => digits(s),
    }
}

#[cfg(test)]
mod tests {
    use super::parse_local_function_name;

    #[test]
    fn local_function_names_match_the_managed_regex() {
        assert!(parse_local_function_name("<Main>g__Local|0_0"));
        assert!(parse_local_function_name("<Main>g__Local0_1"));
        assert!(parse_local_function_name("<Main>g__Local|12"));
        assert!(parse_local_function_name("<<Main>b__0>g__Inner|0_1"));
        assert!(parse_local_function_name("<M>g__|3"));
        assert!(!parse_local_function_name("<Main>g__Local|0_"));
        assert!(!parse_local_function_name("<Main>g__Local|a"));
        assert!(!parse_local_function_name("<Main>b__0_0"));
        assert!(!parse_local_function_name("Main>g__Local|0_0"));
        assert!(!parse_local_function_name("<Main>g__Local"));
    }
}
