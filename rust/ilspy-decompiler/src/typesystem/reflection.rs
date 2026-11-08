//! `ReflectionHelper.ParseReflectionName`: a serialized type name resolved in a resolve context.
//! `` `n `` and ``` ``n ``` name the current type's or method's type parameters, and dummy
//! parameters outside them.

use crate::metadata::{Error, Result};

use super::names::{FullTypeName, TopLevelTypeName, split_type_parameter_count};
use super::typename::{self, TypeName};
use super::types::{
    ClassParam, GenericContext, Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem,
};

/// `new TopLevelTypeName(reflectionName)`.
pub fn top_level_from_reflection(name: &str) -> TopLevelTypeName {
    let pos = name.rfind('.');
    let tick = name.rfind('`');
    let start = pos.map_or(0, |p| p + 1);
    let ns = pos.map_or("", |p| &name[..p]);
    if let Some(tick) = tick
        && pos.is_none_or(|p| tick > p)
    {
        let digits = &name[tick + 1..];
        if !digits.is_empty()
            && digits.bytes().all(|b| b.is_ascii_digit())
            && let Ok(n) = digits.parse::<u64>()
            && n <= i32::MAX as u64
        {
            return TopLevelTypeName::new(ns, &name[start..tick], n as u32);
        }
    }
    TopLevelTypeName::new(ns, &name[start..], 0)
}

/// `new FullTypeName(reflectionName)`.
pub fn full_type_name_from_reflection(name: &str) -> FullTypeName {
    let mut parts = name.split('+');
    let mut full = FullTypeName::top_level(top_level_from_reflection(parts.next().unwrap_or("")));
    for part in parts {
        let (n, tpc) = split_type_parameter_count(part);
        full = full.nested_type(n.to_owned(), tpc);
    }
    full
}

/// `int.TryParse` with the default (integer) number style.
fn parse_int(s: &str) -> Option<i64> {
    s.trim()
        .parse::<i64>()
        .ok()
        .filter(|n| i32::try_from(*n).is_ok())
}

impl TypeSystem<'_, '_> {
    /// `ReflectionHelper.ParseReflectionName(name, new SimpleTypeResolveContext(module))`.
    pub fn parse_reflection_name(&mut self, name: &str) -> Result<TypeId> {
        self.parse_reflection_name_in(name, GenericContext::default())
    }

    /// `ReflectionHelper.ParseReflectionName` with the current type definition and method of
    /// `ctx` (`new SimpleTypeResolveContext(entity)`).
    pub fn parse_reflection_name_in(&mut self, name: &str, ctx: GenericContext) -> Result<TypeId> {
        let parsed = typename::parse(name).ok_or(Error::Malformed("invalid type name"))?;
        self.resolve_type_name(&parsed, ctx)
    }

    fn resolve_type_name(&mut self, t: &TypeName, ctx: GenericContext) -> Result<TypeId> {
        if t.is_array() {
            let elem = self.resolve_type_name(t.element_type(), ctx)?;
            return Ok(self.intern(TypeData::Array {
                elem,
                rank: t.array_rank(),
                nullability: Nullability::Oblivious,
            }));
        }
        if t.is_by_ref() {
            let e = self.resolve_type_name(t.element_type(), ctx)?;
            return Ok(self.intern(TypeData::ByRef(e)));
        }
        if t.is_constructed_generic() {
            let generic = self.resolve_type_name(t.generic_definition(), ctx)?;
            let tpc = self.type_parameter_count(generic) as usize;
            if tpc == 0 {
                return Ok(generic);
            }
            if t.generic_args().len() != tpc {
                return Err(Error::Malformed("generic argument count mismatch"));
            }
            let args = t
                .generic_args()
                .iter()
                .map(|a| self.resolve_type_name(a, ctx))
                .collect::<Result<Box<[_]>>>()?;
            return Ok(self.intern(TypeData::Inst { generic, args }));
        }
        if t.is_nested() {
            let declaring = self.resolve_type_name(t.declaring_type(), ctx)?;
            let name = t.name();
            let (plain, tpc) = split_type_parameter_count(&name);
            if let Some(decl) = self.definition(declaring) {
                let decl_tpc = self.comp.type_def(decl)?.type_parameter_count;
                for nested in self.comp.nested_types(decl) {
                    let info = self.comp.type_def(nested)?;
                    if info.full_name.name() == plain && info.type_parameter_count == tpc + decl_tpc
                    {
                        return Ok(self.intern(TypeData::Def(nested)));
                    }
                }
            }
            let full = full_type_name_from_reflection(&t.full_name());
            return Ok(self.intern(TypeData::Unknown {
                name: Box::new(full),
                is_reference_type: None,
            }));
        }
        if t.is_pointer() {
            let e = self.resolve_type_name(t.element_type(), ctx)?;
            return Ok(self.intern(TypeData::Pointer(e)));
        }
        let full = t.full_name();
        if full.len() > 1 && full.starts_with('`') {
            let is_method = full.len() > 2 && full[1..].starts_with('`');
            let digits = if is_method { &full[2..] } else { &full[1..] };
            if let Some(index) = parse_int(digits) {
                let index = u32::try_from(index)
                    .map_err(|_| Error::Malformed("negative type parameter index"))?;
                let (owner, index) = match (is_method, ctx.method) {
                    (true, Some((m, count))) if index < count => (TypeParamOwner::Method(m), index),
                    (true, _) => (TypeParamOwner::DummyMethod, index),
                    (false, _) => match ctx.class_param(index) {
                        Some(ClassParam::Class(def, i)) => (self.class_param_owner(def, i), i),
                        Some(ClassParam::Method(m, i)) => (TypeParamOwner::Method(m), i),
                        None => (TypeParamOwner::DummyType, index),
                    },
                };
                return Ok(self.intern(TypeData::TypeParam { owner, index }));
            }
        }
        let top = top_level_from_reflection(&full);
        let name = FullTypeName::top_level(top.clone());
        let comp = self.comp;
        if let Some(asm) = &t.assembly
            && let Some(m) = comp.module_by_short_name(asm)
        {
            return Ok(
                match comp.find_type_definition(m, &name, &mut Vec::new())? {
                    Some(def) => self.intern(TypeData::Def(def)),
                    None => self.intern(TypeData::Unknown {
                        name: Box::new(name),
                        is_reference_type: None,
                    }),
                },
            );
        }
        for m in comp.module_ids() {
            if let Some(def) = comp.find_type_definition(m, &name, &mut Vec::new())? {
                return Ok(self.intern(TypeData::Def(def)));
            }
        }
        Ok(self.intern(TypeData::Unknown {
            name: Box::new(name),
            is_reference_type: None,
        }))
    }
}
