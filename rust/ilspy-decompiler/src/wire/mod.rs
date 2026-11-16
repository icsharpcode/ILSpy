//! The wire format the engine hands to the C# materializers.
//!
//! A message is `"ILSW"`, a version varint, the string table, the type table and the payload.
//! Integers are LEB128 varints (signed ones zigzag-encoded); strings and types are indices into
//! their tables. A type entry only refers to entries before it.

pub mod ast;
pub mod ilast;

use std::collections::HashMap;

use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::types::{Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};

pub const MAGIC: &[u8; 4] = b"ILSW";

/// Type entry tags; `Interop/WireReader.cs` mirrors them.
/// The node kinds of a type syntax entry.
mod syntax_tag {
    /// A type the engine does not convert (a function pointer or intersection type).
    pub const NONE: u8 = 0;
    pub const PRIMITIVE: u8 = 1;
    pub const SIMPLE: u8 = 2;
    pub const COMPOSED: u8 = 3;
    pub const TUPLE: u8 = 4;
}

mod tag {
    pub const DEF: u8 = 0;
    pub const UNKNOWN: u8 = 1;
    pub const UNKNOWN_SPECIAL: u8 = 2;
    pub const UNBOUND: u8 = 3;
    pub const TYPE_PARAM: u8 = 4;
    pub const INST: u8 = 5;
    pub const ARRAY: u8 = 6;
    pub const POINTER: u8 = 7;
    pub const BY_REF: u8 = 8;
    pub const PINNED: u8 = 9;
    pub const FN_PTR: u8 = 10;
    pub const ARG_LIST: u8 = 11;
    pub const MODIFIED: u8 = 12;
    pub const DYNAMIC: u8 = 13;
    pub const NINT: u8 = 14;
    pub const NUINT: u8 = 15;
    pub const TUPLE: u8 = 16;
    pub const ANNOTATED: u8 = 17;
    pub const NULL: u8 = 18;
    pub const NO_TYPE: u8 = 19;
    pub const INTERSECTION: u8 = 20;
}

#[derive(Default)]
pub struct Bytes(pub Vec<u8>);

impl Bytes {
    pub fn u8(&mut self, v: u8) {
        self.0.push(v);
    }

    pub fn bool(&mut self, v: bool) {
        self.0.push(u8::from(v));
    }

    pub fn uvar(&mut self, mut v: u64) {
        loop {
            let byte = (v & 0x7F) as u8;
            v >>= 7;
            if v == 0 {
                self.0.push(byte);
                return;
            }
            self.0.push(byte | 0x80);
        }
    }

    pub fn ivar(&mut self, v: i64) {
        self.uvar(((v << 1) ^ (v >> 63)) as u64);
    }

    pub fn len(&mut self, v: usize) {
        self.uvar(v as u64);
    }

    pub fn raw(&mut self, bytes: &[u8]) {
        self.0.extend_from_slice(bytes);
    }
}

/// Collects the string and type tables while the payload is written.
#[derive(Default)]
pub struct Tables {
    strings: Vec<String>,
    string_index: HashMap<String, u32>,
    types: Bytes,
    type_count: u32,
    type_index: HashMap<TypeId, u32>,
    /// The type of each entry, in order.
    type_ids: Vec<TypeId>,
    /// `TypeSystemAstBuilder.ConvertType` of each entry, length-prefixed, when a translated
    /// body asked for it (see `type_syntax`).
    type_syntax: Option<Bytes>,
    /// How many entries `type_syntax` holds.
    type_syntax_written: usize,
}

impl Tables {
    pub fn string(&mut self, s: &str) -> u32 {
        if let Some(&i) = self.string_index.get(s) {
            return i;
        }
        let i = self.strings.len() as u32;
        self.strings.push(s.to_owned());
        self.string_index.insert(s.to_owned(), i);
        i
    }

    fn def(out: &mut Bytes, d: TypeDefId) {
        out.len(d.module.0 as usize);
        out.uvar(u64::from(d.row));
    }

    /// The index of `t`, writing its entry (and those of its parts) on first use.
    pub fn ty(&mut self, ts: &TypeSystem<'_, '_>, t: TypeId) -> u32 {
        if let Some(&i) = self.type_index.get(&t) {
            return i;
        }
        let mut e = Bytes::default();
        match ts.data(t).clone() {
            TypeData::Def(d) => {
                e.u8(tag::DEF);
                Self::def(&mut e, d);
            }
            TypeData::Unknown {
                name,
                is_reference_type,
            } => {
                e.u8(tag::UNKNOWN);
                let ns = self.string(&name.top.ns);
                let n = self.string(&name.top.name);
                e.uvar(u64::from(ns));
                e.uvar(u64::from(n));
                e.uvar(u64::from(name.top.tpc));
                e.len(name.nested.len());
                for (nested, tpc) in &name.nested {
                    let s = self.string(nested);
                    e.uvar(u64::from(s));
                    e.uvar(u64::from(*tpc));
                }
                e.u8(match is_reference_type {
                    None => 0,
                    Some(false) => 1,
                    Some(true) => 2,
                });
            }
            TypeData::UnknownSpecial => e.u8(tag::UNKNOWN_SPECIAL),
            TypeData::Unbound => e.u8(tag::UNBOUND),
            TypeData::TypeParam { owner, index } => {
                e.u8(tag::TYPE_PARAM);
                match owner {
                    TypeParamOwner::Type(d) => {
                        e.u8(0);
                        Self::def(&mut e, d);
                    }
                    TypeParamOwner::Method(m) => {
                        e.u8(1);
                        e.len(m.module.0 as usize);
                        e.uvar(u64::from(m.row));
                    }
                    TypeParamOwner::ContextMethod { context, .. } => {
                        e.u8(4);
                        e.uvar(u64::from(context));
                    }
                    TypeParamOwner::DummyType => e.u8(2),
                    TypeParamOwner::DummyMethod => e.u8(3),
                    // shortcut: a synthetic type parameter goes over the wire as the dummy
                    // parameter at its index; upgrade when engine-created type parameters
                    // reach the host.
                    TypeParamOwner::Synthetic { method, .. } => e.u8(if method { 3 } else { 2 }),
                }
                e.uvar(u64::from(index));
            }
            TypeData::Inst { generic, args } => {
                let g = self.ty(ts, generic);
                let a: Vec<u32> = args.iter().map(|&x| self.ty(ts, x)).collect();
                e.u8(tag::INST);
                e.uvar(u64::from(g));
                self.list(&mut e, &a);
            }
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => {
                let el = self.ty(ts, elem);
                e.u8(tag::ARRAY);
                e.uvar(u64::from(el));
                e.uvar(u64::from(rank));
                e.u8(nullability as u8);
            }
            TypeData::Pointer(x) | TypeData::ByRef(x) | TypeData::Pinned(x) => {
                let el = self.ty(ts, x);
                e.u8(match ts.data(t) {
                    TypeData::Pointer(_) => tag::POINTER,
                    TypeData::ByRef(_) => tag::BY_REF,
                    _ => tag::PINNED,
                });
                e.uvar(u64::from(el));
            }
            TypeData::FnPtr(f) => {
                let custom: Vec<u32> = f
                    .custom_conventions
                    .iter()
                    .map(|&x| self.ty(ts, x))
                    .collect();
                let ret = self.ty(ts, f.ret);
                let params: Vec<u32> = f.params.iter().map(|&x| self.ty(ts, x)).collect();
                e.u8(tag::FN_PTR);
                e.u8(f.calling_convention);
                self.list(&mut e, &custom);
                e.uvar(u64::from(ret));
                e.bool(f.ret_ref_readonly);
                self.list(&mut e, &params);
                e.raw(&f.ref_kinds);
            }
            TypeData::ArgList => e.u8(tag::ARG_LIST),
            TypeData::Null => e.u8(tag::NULL),
            TypeData::NoType => e.u8(tag::NO_TYPE),
            TypeData::Intersection(types) => {
                let list: Vec<u32> = types.iter().map(|&x| self.ty(ts, x)).collect();
                e.u8(tag::INTERSECTION);
                self.list(&mut e, &list);
            }
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => {
                let m = self.ty(ts, modifier);
                let i = self.ty(ts, inner);
                e.u8(tag::MODIFIED);
                e.uvar(u64::from(m));
                e.uvar(u64::from(i));
                e.bool(required);
            }
            TypeData::Dynamic => e.u8(tag::DYNAMIC),
            TypeData::NInt => e.u8(tag::NINT),
            TypeData::NUInt => e.u8(tag::NUINT),
            TypeData::Tuple {
                underlying,
                elements,
                names,
            } => {
                let el: Vec<u32> = elements.iter().map(|&x| self.ty(ts, x)).collect();
                let n: Vec<u32> = names
                    .iter()
                    .map(|n| n.as_deref().map_or(0, |s| self.string(s) + 1))
                    .collect();
                e.u8(tag::TUPLE);
                // The module of `System.ValueTuple`, +1 (0 when it is unresolved).
                e.len(
                    ts.definition(underlying)
                        .map_or(0, |d| d.module.0 as usize + 1),
                );
                self.list(&mut e, &el);
                self.list(&mut e, &n);
            }
            TypeData::Annotated { inner, nullability } => {
                let i = self.ty(ts, inner);
                e.u8(tag::ANNOTATED);
                e.uvar(u64::from(i));
                e.u8(match nullability {
                    Nullability::Oblivious => 0,
                    Nullability::NotNullable => 1,
                    Nullability::Nullable => 2,
                });
            }
        }
        let i = self.type_count;
        self.type_count += 1;
        // Each entry is length-prefixed, so the host can decode it on first use.
        self.types.len(e.0.len());
        self.types.raw(&e.0);
        self.type_index.insert(t, i);
        self.type_ids.push(t);
        i
    }

    /// The type syntax of every entry, which the host builds the C# types of a translated body
    /// from; the entries its annotations add get theirs as well.
    pub fn type_syntax(&mut self, ts: &TypeSystem<'_, '_>, nullable_specifier: bool) {
        let builder = crate::csharp::type_syntax::TypeSyntaxBuilder::new(ts, nullable_specifier);
        let mut out = self.type_syntax.take().unwrap_or_default();
        let mut done = self.type_syntax_written;
        while done < self.type_ids.len() {
            let t = self.type_ids[done];
            let mut e = Bytes::default();
            match builder.convert_type(t) {
                Some(syntax) => self.syntax_node(ts, &mut e, &syntax),
                None => e.u8(syntax_tag::NONE),
            }
            out.len(e.0.len());
            out.raw(&e.0);
            done += 1;
        }
        self.type_syntax = Some(out);
        self.type_syntax_written = done;
    }

    fn syntax_node(
        &mut self,
        ts: &TypeSystem<'_, '_>,
        e: &mut Bytes,
        s: &crate::csharp::type_syntax::TypeSyntax,
    ) {
        use crate::csharp::type_syntax::TypeSyntaxKind as K;
        match &s.kind {
            K::Primitive(keyword) => {
                e.u8(syntax_tag::PRIMITIVE);
                let i = self.string(keyword);
                e.uvar(u64::from(i));
            }
            K::Simple { name, args } => {
                e.u8(syntax_tag::SIMPLE);
                let i = self.string(name);
                e.uvar(u64::from(i));
                e.len(args.len());
                for a in args {
                    self.syntax_node(ts, e, a);
                }
            }
            K::Composed {
                base,
                nullable,
                reference,
                pointer_rank,
                arrays,
            } => {
                e.u8(syntax_tag::COMPOSED);
                self.syntax_node(ts, e, base);
                e.bool(*nullable);
                e.bool(*reference);
                e.uvar(u64::from(*pointer_rank));
                e.len(arrays.len());
                for &d in arrays {
                    e.uvar(u64::from(d));
                }
            }
            K::Tuple(elements) => {
                e.u8(syntax_tag::TUPLE);
                e.len(elements.len());
                for (element, name) in elements {
                    self.syntax_node(ts, e, element);
                    match name {
                        Some(n) => {
                            let i = self.string(n);
                            e.uvar(u64::from(i) + 1);
                        }
                        None => e.uvar(0),
                    }
                }
            }
        }
        e.len(s.annotations.len());
        for &t in &s.annotations {
            let i = self.ty(ts, t);
            e.uvar(u64::from(i));
        }
    }

    fn list(&self, e: &mut Bytes, items: &[u32]) {
        e.len(items.len());
        for &x in items {
            e.uvar(u64::from(x));
        }
    }

    /// The whole message around `payload`.
    pub fn finish(self, version: u32, payload: Bytes) -> Vec<u8> {
        let mut out = Bytes::default();
        out.raw(MAGIC);
        out.uvar(u64::from(version));
        out.len(self.strings.len());
        for s in &self.strings {
            out.len(s.len());
            out.raw(s.as_bytes());
        }
        out.uvar(u64::from(self.type_count));
        out.raw(&self.types.0);
        // The type syntax, one entry per type, when a body asked for it.
        match &self.type_syntax {
            Some(syntax) => {
                debug_assert_eq!(self.type_syntax_written, self.type_ids.len());
                out.u8(1);
                out.raw(&syntax.0);
            }
            None => out.u8(0),
        }
        out.raw(&payload.0);
        out.0
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn varints_match_leb128_and_zigzag() {
        let mut b = Bytes::default();
        b.uvar(0);
        b.uvar(127);
        b.uvar(128);
        b.ivar(-1);
        b.ivar(1);
        b.ivar(i64::MIN);
        assert_eq!(
            b.0,
            [
                0, 0x7F, 0x80, 0x01, 1, 2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0x01
            ]
        );
    }
}
