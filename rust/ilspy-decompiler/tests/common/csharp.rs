//! Fixtures shared by the suites ported from `ICSharpCode.Decompiler.Tests/Semantics`.
//!
//! - `compilation()`: the compiled test assembly with the legacy `mscorlib` and
//!   `System.Core`, all with `TypeSystemOptions.Default` (the managed
//!   `new SimpleCompilation(TestAssembly, Mscorlib, SystemCore)`).
//! - `ref_assembly_compilation()`: the test assembly with the .NET 5 reference
//!   `System.Runtime.dll` (`RefAssemblyCompilation`), from `ILSPY_REF_SYSTEM_RUNTIME`, by
//!   default the copy the managed suite fetched under `ICSharpCode.Decompiler.Tests/bin`.
//! - `ty(ts, "List<string>[]")`: the type `compilation.FindType(typeof(...))` gives for C#
//!   type syntax, with the special types the managed tests map their placeholder classes to
//!   (`null`, `dynamic`, `nint`, `nuint`) and `?` for `SpecialType.UnknownType`.

#![allow(dead_code)]

use std::path::PathBuf;

use ilspy_decompiler::semantics::{Method, MethodGroup, MethodList, Parameter, ResolveResult};
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::lookup::MemberOptions;
use ilspy_decompiler::typesystem::members::{RefKind, TypeParamInfo};
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};

/// The namespaces short type names are looked up in, in order.
const NAMESPACES: &[&str] = &[
    "System",
    "System.Collections",
    "System.Collections.Generic",
    "System.Collections.ObjectModel",
    "System.Linq",
    "System.Linq.Expressions",
    "System.Threading.Tasks",
    "System.Reflection",
    "System.Text",
    "System.IO",
];

/// The test namespace whose fixture classes nest the types the suites use.
pub const TEST_NAMESPACE: &str = "ICSharpCode.Decompiler.Tests.Semantics";

/// `new SimpleCompilation(TestAssembly, Mscorlib, SystemCore)`.
pub fn compilation() -> Compilation<'static> {
    super::compilation(&[
        (super::test_assembly(), options::DEFAULT),
        (super::mscorlib(), options::DEFAULT),
        (super::system_core(), options::DEFAULT),
    ])
}

/// The .NET 5 reference `System.Runtime.dll` (`ILSPY_REF_SYSTEM_RUNTIME`).
pub fn ref_system_runtime() -> PathBuf {
    if let Some(path) = std::env::var_os("ILSPY_REF_SYSTEM_RUNTIME") {
        let path = PathBuf::from(path);
        assert!(
            path.is_file(),
            "ILSPY_REF_SYSTEM_RUNTIME {} not found",
            path.display()
        );
        return path;
    }
    let bin = std::path::Path::new(env!("CARGO_MANIFEST_DIR"))
        .join("../../ICSharpCode.Decompiler.Tests/bin");
    for config in ["Debug", "Release"] {
        for tfm in ["net11.0/win-x64", "net11.0"] {
            let path = bin
                .join(config)
                .join(tfm)
                .join("netfx/5.0.0/ref/net5.0/System.Runtime.dll");
            if path.is_file() {
                return path;
            }
        }
    }
    panic!(
        "the .NET 5 reference System.Runtime.dll was not found under {} (run the managed \
         Semantics tests once, or set ILSPY_REF_SYSTEM_RUNTIME)",
        bin.display()
    );
}

/// `RefAssemblyCompilation.Instance`: the test assembly with the .NET 5 reference
/// `System.Runtime`.
pub fn ref_assembly_compilation() -> Compilation<'static> {
    super::compilation(&[
        (super::test_assembly(), options::DEFAULT),
        (ref_system_runtime(), options::DEFAULT),
    ])
}

/// The type for C# type syntax: keywords, `?`, `null`, `dynamic`, `nint`, `nuint`, dotted
/// and short names (looked up in `NAMESPACES`, then nested in the classes named by `outer`,
/// `+`-separated reflection names relative to `TEST_NAMESPACE`), generic arguments
/// `<...>`, tuples `(a, b)` (as `System.ValueTuple`, like `typeof`), and the suffixes `[]`,
/// `[,]`, `?` (nullable value type) and `*`.
pub fn ty(ts: &mut TypeSystem<'_, '_>, s: &str) -> TypeId {
    ty_in(ts, s, &[])
}

/// `ty`, also looking short names up nested in `outer` (e.g. `ConversionTest`).
pub fn ty_in(ts: &mut TypeSystem<'_, '_>, s: &str, outer: &[&str]) -> TypeId {
    let mut p = Parser {
        s: s.as_bytes(),
        pos: 0,
        outer,
    };
    let t = p.parse_type(ts);
    p.skip_ws();
    assert_eq!(p.pos, p.s.len(), "trailing text in type {s:?}");
    t
}

struct Parser<'s> {
    s: &'s [u8],
    pos: usize,
    outer: &'s [&'s str],
}

impl Parser<'_> {
    fn skip_ws(&mut self) {
        while self.pos < self.s.len() && self.s[self.pos] == b' ' {
            self.pos += 1;
        }
    }

    fn peek(&mut self) -> Option<u8> {
        self.skip_ws();
        self.s.get(self.pos).copied()
    }

    fn expect(&mut self, c: u8) {
        assert_eq!(
            self.peek(),
            Some(c),
            "expected {:?} at {}",
            c as char,
            self.pos
        );
        self.pos += 1;
    }

    fn ident(&mut self) -> String {
        self.skip_ws();
        let start = self.pos;
        while self.pos < self.s.len()
            && (self.s[self.pos].is_ascii_alphanumeric()
                || matches!(self.s[self.pos], b'_' | b'.' | b'+'))
        {
            self.pos += 1;
        }
        assert!(self.pos > start, "expected a name at {}", self.pos);
        String::from_utf8(self.s[start..self.pos].to_vec()).unwrap()
    }

    fn parse_type(&mut self, ts: &mut TypeSystem<'_, '_>) -> TypeId {
        let mut t = self.parse_base(ts);
        loop {
            match self.peek() {
                Some(b'[') => {
                    self.pos += 1;
                    let mut rank = 1;
                    while self.peek() == Some(b',') {
                        self.pos += 1;
                        rank += 1;
                    }
                    self.expect(b']');
                    t = ts.intern(TypeData::Array {
                        elem: t,
                        rank,
                        nullability: Nullability::Oblivious,
                    });
                }
                Some(b'?') => {
                    self.pos += 1;
                    t = ts.make_nullable(t);
                }
                Some(b'*') => {
                    self.pos += 1;
                    t = ts.intern(TypeData::Pointer(t));
                }
                _ => return t,
            }
        }
    }

    fn type_list(&mut self, ts: &mut TypeSystem<'_, '_>, close: u8) -> Vec<TypeId> {
        let mut list = vec![self.parse_type(ts)];
        while self.peek() == Some(b',') {
            self.pos += 1;
            list.push(self.parse_type(ts));
        }
        self.expect(close);
        list
    }

    fn parse_base(&mut self, ts: &mut TypeSystem<'_, '_>) -> TypeId {
        if self.peek() == Some(b'(') {
            self.pos += 1;
            let elements = self.type_list(ts, b')');
            return ts.tuple_underlying(&elements, None);
        }
        if self.peek() == Some(b'?') {
            self.pos += 1;
            return ts.intern(TypeData::UnknownSpecial);
        }
        let name = self.ident();
        let args = if self.peek() == Some(b'<') {
            self.pos += 1;
            self.type_list(ts, b'>')
        } else {
            Vec::new()
        };
        if args.is_empty()
            && let Some(t) = keyword(ts, &name)
        {
            return t;
        }
        let generic = self.resolve(ts, &name, args.len());
        if args.is_empty() {
            generic
        } else {
            ts.intern(TypeData::Inst {
                generic,
                args: args.into(),
            })
        }
    }

    fn resolve(&self, ts: &mut TypeSystem<'_, '_>, name: &str, arity: usize) -> TypeId {
        let suffix = if arity > 0 {
            format!("`{arity}")
        } else {
            String::new()
        };
        let mut candidates = vec![name.to_owned()];
        candidates.extend(NAMESPACES.iter().map(|ns| format!("{ns}.{name}")));
        for outer in self.outer {
            candidates.push(format!(
                "{TEST_NAMESPACE}.{outer}+{}",
                name.replace('.', "+")
            ));
        }
        candidates.push(format!("{TEST_NAMESPACE}.{name}"));
        for c in candidates {
            if let Ok(t) = ts.parse_reflection_name(&format!("{c}{suffix}"))
                && ts.definition(t).is_some()
            {
                return t;
            }
        }
        panic!("type {name} (arity {arity}) not found");
    }
}

fn keyword(ts: &mut TypeSystem<'_, '_>, name: &str) -> Option<TypeId> {
    let code = match name {
        "object" => KnownTypeCode::Object,
        "string" => KnownTypeCode::String,
        "bool" => KnownTypeCode::Boolean,
        "char" => KnownTypeCode::Char,
        "sbyte" => KnownTypeCode::SByte,
        "byte" => KnownTypeCode::Byte,
        "short" => KnownTypeCode::Int16,
        "ushort" => KnownTypeCode::UInt16,
        "int" => KnownTypeCode::Int32,
        "uint" => KnownTypeCode::UInt32,
        "long" => KnownTypeCode::Int64,
        "ulong" => KnownTypeCode::UInt64,
        "float" => KnownTypeCode::Single,
        "double" => KnownTypeCode::Double,
        "decimal" => KnownTypeCode::Decimal,
        "void" => KnownTypeCode::Void,
        "null" => return Some(ts.intern(TypeData::Null)),
        "dynamic" => return Some(ts.intern(TypeData::Dynamic)),
        "nint" => return Some(ts.intern(TypeData::NInt)),
        "nuint" => return Some(ts.intern(TypeData::NUInt)),
        _ => return None,
    };
    Some(ts.known_type(code))
}

/// A tuple type with element names (`new TupleType(compilation, elements, names)`).
pub fn tuple(ts: &mut TypeSystem<'_, '_>, elements: &[TypeId], names: &[Option<&str>]) -> TypeId {
    let underlying = ts.tuple_underlying(elements, None);
    ts.intern(TypeData::Tuple {
        underlying,
        elements: elements.into(),
        names: names.iter().map(|n| n.map(Into::into)).collect(),
    })
}

/// `new DefaultTypeParameter(compilation, ownerType, index, name, ...)`: a method (`method`)
/// or class type parameter; `constraints` are the declared constraint types.
#[allow(clippy::too_many_arguments)]
pub fn type_parameter(
    ts: &mut TypeSystem<'_, '_>,
    method: bool,
    index: u32,
    name: &str,
    reference_type: bool,
    value_type: bool,
    default_ctor: bool,
    constraints: Vec<TypeId>,
) -> TypeId {
    ts.new_type_parameter(
        method,
        index,
        TypeParamInfo {
            name: name.to_owned(),
            variance: "Invariant",
            default_ctor,
            reference_type,
            value_type,
            allows_ref_like: false,
            constraints,
            unmanaged: false,
            nullability: Nullability::Oblivious,
        },
    )
}

/// An unconstrained method type parameter.
pub fn method_type_parameter(ts: &mut TypeSystem<'_, '_>, index: u32, name: &str) -> TypeId {
    type_parameter(ts, true, index, name, false, false, false, Vec::new())
}

/// The synthetic type parameter id of a `type_parameter` result (for constraints that
/// refer to type parameters created later).
pub fn synthetic_id(ts: &TypeSystem<'_, '_>, t: TypeId) -> u32 {
    match ts.data(t) {
        TypeData::TypeParam {
            owner: ilspy_decompiler::typesystem::types::TypeParamOwner::Synthetic { id, .. },
            ..
        } => *id,
        _ => panic!("not a synthetic type parameter"),
    }
}

/// `type.GetMethods(m => m.Name == name)` (with inherited members).
pub fn methods(ts: &mut TypeSystem<'_, '_>, t: TypeId, name: &str) -> Vec<Method> {
    let found = ts
        .get_methods(t, None, MemberOptions::default())
        .expect("methods");
    found
        .iter()
        .filter(|m| m.name == name)
        .map(|m| Method::from_resolved(ts, m).expect("a method"))
        .collect()
}

/// `type.GetDefinition().GetMethods(m => m.Name == name)`: the declared and inherited methods
/// of the type definition.
pub fn definition_methods(ts: &mut TypeSystem<'_, '_>, t: TypeId, name: &str) -> Vec<Method> {
    let def = ts.definition(t).expect("a type definition");
    let t = ts.intern_def(def);
    methods(ts, t, name)
}

/// `new MethodGroupResolveResult(new ResolveResult(declaring) or target, name,
/// new[] { new MethodListWithDeclaringType(declaring, methods named name) }, typeArguments)`.
pub fn method_group(
    ts: &mut TypeSystem<'_, '_>,
    declaring: TypeId,
    name: &str,
    target: Option<ResolveResult>,
    type_arguments: Vec<TypeId>,
) -> ResolveResult {
    let methods = methods(ts, declaring, name);
    ResolveResult::MethodGroup(std::rc::Rc::new(MethodGroup {
        target: target.unwrap_or(ResolveResult::Expression(declaring)),
        name: name.to_owned(),
        method_lists: vec![MethodList { declaring, methods }],
        type_arguments,
        extension_methods: Vec::new(),
    }))
}

/// A parameter of a fake method (`new DefaultParameter(type, name, ...)`).
pub fn parameter(ty: TypeId, ref_kind: RefKind, is_params: bool, is_optional: bool) -> Parameter {
    Parameter {
        name: String::new(),
        ty,
        ref_kind,
        is_params,
        is_optional,
    }
}
