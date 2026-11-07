//! `ApplyAttributeTypeVisitor`: dynamic, native integer, tuple element name and nullability
//! attributes applied to a type, custom modifiers removed. Also the nullable contexts and the
//! per-entity options (`NullablePublicOnly`) that decide which of them apply.

use crate::metadata::custom_attribute::{self, ModuleResolver, Value};
use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Metadata, Result};

use super::compilation::{ModuleId, TypeDefId, TypeKind, is_known_type_name};
use super::known::KnownTypeCode;
use super::names::TopLevelTypeName;
use super::options;
use super::types::{Nullability, TypeData, TypeId, TypeSystem};

const COMPILER_SERVICES: &str = "System.Runtime.CompilerServices";

/// Where the custom attributes that drive the visitor come from.
#[derive(Clone, Copy, Debug)]
pub enum AttributeSource {
    None,
    /// The attributes of one entity (a Param, Field, TypeDef, InterfaceImpl, ...).
    Entity(TableId, u32),
    /// Optional entity attributes, then attributes that override them (a property's own
    /// attributes over its getter's return parameter).
    WithAdditional(Option<(TableId, u32)>, (TableId, u32)),
}

fn attribute_is(md: &Metadata<'_>, ca: u32, name: &str) -> bool {
    md.attribute_type(ca)
        .is_ok_and(|t| is_known_type_name(md, t, COMPILER_SERVICES, name, 0))
}

fn decode(md: &Metadata<'_>, ca: u32) -> Result<custom_attribute::CustomAttributeValue> {
    let attribute = md.get::<rows::CustomAttribute>(ca)?;
    custom_attribute::decode(md, &attribute, &ModuleResolver::new(md))
}

/// The single fixed argument as an array, when every element passes `f`.
fn array_arg<T>(
    value: &custom_attribute::CustomAttributeValue,
    f: impl Fn(&Value) -> Option<T>,
) -> Option<Vec<T>> {
    match value.fixed.as_slice() {
        [Value::Array(Some(items))] => items.iter().map(f).collect(),
        _ => None,
    }
}

/// `GetNullableContext`: the first `NullableContextAttribute` that decodes to 0..=2.
pub fn nullable_context(md: &Metadata<'_>, table: TableId, row: u32) -> Option<Nullability> {
    for ca in md.custom_attributes(table, row) {
        if !attribute_is(md, ca, "NullableContextAttribute") {
            continue;
        }
        let Ok(value) = decode(md, ca) else {
            continue;
        };
        if let [Value::U8(b)] = value.fixed.as_slice()
            && let Some(n) = Nullability::from_byte(*b)
        {
            return Some(n);
        }
    }
    None
}

struct Visitor {
    has_dynamic: bool,
    dynamic_data: Option<Vec<bool>>,
    has_native_integers: bool,
    native_integer_data: Option<Vec<bool>>,
    options: u32,
    tuple_names: Option<Vec<Option<String>>>,
    default_nullability: Nullability,
    nullable_data: Option<Vec<Nullability>>,
    dynamic_index: usize,
    tuple_index: usize,
    nullability_index: usize,
    native_int_index: usize,
}

impl Visitor {
    fn nullability(&mut self) -> Nullability {
        match &self.nullable_data {
            Some(data) if self.nullability_index < data.len() => {
                let n = data[self.nullability_index];
                self.nullability_index += 1;
                n
            }
            _ => self.default_nullability,
        }
    }
}

const TUPLE_REST: usize = 8;

impl TypeSystem<'_, '_> {
    /// `ApplyAttributeTypeVisitor.ApplyAttributesToType`.
    pub fn apply_attributes(
        &mut self,
        ty: TypeId,
        module: ModuleId,
        source: AttributeSource,
        options: u32,
        nullable_context: Nullability,
        children_only: bool,
    ) -> Result<TypeId> {
        let md = &self.comp.module(module).md;
        let mut v = Visitor {
            has_dynamic: false,
            dynamic_data: None,
            has_native_integers: options & options::NATIVE_INTEGERS_WITHOUT_ATTRIBUTE != 0,
            native_integer_data: None,
            options,
            tuple_names: None,
            default_nullability: if options & options::NULLABILITY != 0 {
                nullable_context
            } else {
                Nullability::Oblivious
            },
            nullable_data: None,
            dynamic_index: 0,
            tuple_index: 0,
            nullability_index: 0,
            native_int_index: 0,
        };
        let relevant =
            options::DYNAMIC | options::TUPLE | options::NULLABILITY | options::NATIVE_INTEGERS;
        if options & relevant != 0 {
            let (primary, additional) = match source {
                AttributeSource::None => (None, None),
                AttributeSource::Entity(t, r) => (Some((t, r)), None),
                AttributeSource::WithAdditional(p, a) => (p, Some(a)),
            };
            for (table, row) in primary.into_iter().chain(additional) {
                for ca in md.custom_attributes(table, row) {
                    process_attribute(md, ca, &mut v)?;
                }
            }
        }
        let runs = v.has_dynamic
            || v.has_native_integers
            || v.default_nullability != Nullability::Oblivious
            || v.nullable_data.is_some()
            || options & (options::TUPLE | options::KEEP_MODIFIERS) != options::KEEP_MODIFIERS;
        if !runs {
            return Ok(ty);
        }
        Ok(if children_only {
            self.visit_children(ty, &mut v)
        } else {
            self.visit(ty, &mut v)
        })
    }

    fn visit_children(&mut self, t: TypeId, v: &mut Visitor) -> TypeId {
        let data = self.data(t).clone();
        let new = match data {
            TypeData::Inst { generic, args } => TypeData::Inst {
                generic: self.visit(generic, v),
                args: args.iter().map(|a| self.visit(*a, v)).collect(),
            },
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => TypeData::Array {
                elem: self.visit(elem, v),
                rank,
                nullability,
            },
            TypeData::Pointer(e) => TypeData::Pointer(self.visit(e, v)),
            TypeData::ByRef(e) => TypeData::ByRef(self.visit(e, v)),
            TypeData::Pinned(e) => TypeData::Pinned(self.visit(e, v)),
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => TypeData::Modified {
                modifier: self.visit(modifier, v),
                inner: self.visit(inner, v),
                required,
            },
            TypeData::FnPtr(f) => {
                let mut f = *f;
                f.ret = self.visit(f.ret, v);
                f.params = f.params.iter().map(|p| self.visit(*p, v)).collect();
                TypeData::FnPtr(Box::new(f))
            }
            TypeData::Annotated { inner, nullability } => TypeData::Annotated {
                inner: self.visit(inner, v),
                nullability,
            },
            _ => return t,
        };
        self.intern(new)
    }

    /// `IType.ChangeNullability`.
    pub fn change_nullability(&mut self, t: TypeId, n: Nullability) -> TypeId {
        match self.data(t).clone() {
            TypeData::Def(_) | TypeData::Unknown { .. } => {
                if n == Nullability::Oblivious || self.is_reference_type(t) == Some(false) {
                    t
                } else {
                    self.intern(TypeData::Annotated {
                        inner: t,
                        nullability: n,
                    })
                }
            }
            TypeData::Dynamic | TypeData::TypeParam { .. } => {
                if n == Nullability::Oblivious {
                    t
                } else {
                    self.intern(TypeData::Annotated {
                        inner: t,
                        nullability: n,
                    })
                }
            }
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => {
                if nullability == n {
                    t
                } else {
                    self.intern(TypeData::Array {
                        elem,
                        rank,
                        nullability: n,
                    })
                }
            }
            TypeData::Inst { generic, args } => {
                let g = self.change_nullability(generic, n);
                if g == generic {
                    t
                } else {
                    self.intern(TypeData::Inst { generic: g, args })
                }
            }
            TypeData::Annotated { inner, nullability } => {
                if nullability == n {
                    t
                } else {
                    self.change_nullability(inner, n)
                }
            }
            _ => t,
        }
    }

    fn known_code(&self, t: TypeId) -> KnownTypeCode {
        match self.data(t) {
            TypeData::Def(d) => self
                .comp
                .type_def(*d)
                .map_or(KnownTypeCode::None, |i| i.known),
            _ => KnownTypeCode::None,
        }
    }

    /// `TupleType.IsTupleCompatible` on a (not yet converted) parameterized type.
    fn tuple_cardinality(&self, t: TypeId) -> Option<usize> {
        let TypeData::Inst { generic, args } = self.data(t) else {
            return None;
        };
        let def = self.definition(*generic)?;
        let info = self.comp.type_def(def).ok()?;
        if info.kind != TypeKind::Struct
            || info.full_name.top.ns != "System"
            || info.full_name.name() != "ValueTuple"
        {
            return None;
        }
        let tpc = info.type_parameter_count as usize;
        if tpc > 0 && tpc < TUPLE_REST {
            Some(tpc)
        } else if tpc == TUPLE_REST {
            Some(self.tuple_cardinality(*args.last()?)? + TUPLE_REST - 1)
        } else {
            None
        }
    }

    /// `TupleType.CreateUnderlyingType`, preferring the module the tuple came from.
    pub fn tuple_underlying(&mut self, elements: &[TypeId], assembly: Option<ModuleId>) -> TypeId {
        let find = |ts: &mut Self, tpc: u32| {
            let top = TopLevelTypeName::new("System", "ValueTuple", tpc);
            if let Some(m) = assembly
                && let Ok(Some(def)) = ts.comp.find_type_definition(
                    m,
                    &super::names::FullTypeName::top_level(top.clone()),
                    &mut Vec::new(),
                )
            {
                return ts.intern(TypeData::Def(def));
            }
            ts.find_top_level(top)
        };
        let remainder = (elements.len() - 1) % (TUPLE_REST - 1) + 1;
        let mut pos = elements.len() - remainder;
        let generic = find(self, remainder as u32);
        let mut ty = self.intern(TypeData::Inst {
            generic,
            args: elements[pos..].into(),
        });
        while pos > 0 {
            pos -= TUPLE_REST - 1;
            let generic = find(self, TUPLE_REST as u32);
            let mut args = elements[pos..pos + TUPLE_REST - 1].to_vec();
            args.push(ty);
            ty = self.intern(TypeData::Inst {
                generic,
                args: args.into(),
            });
        }
        ty
    }

    fn visit(&mut self, t: TypeId, v: &mut Visitor) -> TypeId {
        let data = self.data(t).clone();
        match data {
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => {
                v.dynamic_index += 1;
                if v.options & options::KEEP_MODIFIERS != 0 {
                    let modifier = self.visit(modifier, v);
                    let inner = self.visit(inner, v);
                    self.intern(TypeData::Modified {
                        modifier,
                        inner,
                        required,
                    })
                } else {
                    self.visit(inner, v)
                }
            }
            TypeData::Pointer(e) => {
                v.dynamic_index += 1;
                let e = self.visit(e, v);
                self.intern(TypeData::Pointer(e))
            }
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => {
                let n = v.nullability();
                v.dynamic_index += 1;
                let elem = self.visit(elem, v);
                let a = self.intern(TypeData::Array {
                    elem,
                    rank,
                    nullability,
                });
                self.change_nullability(a, n)
            }
            TypeData::ByRef(e) => {
                v.dynamic_index += 1;
                let e = self.visit(e, v);
                self.intern(TypeData::ByRef(e))
            }
            TypeData::Inst { generic, args } => self.visit_parameterized(t, generic, &args, v),
            TypeData::FnPtr(f) => {
                v.dynamic_index += 1;
                if f.ret_ref_readonly {
                    v.dynamic_index += 1;
                }
                let mut f = *f;
                f.ret = self.visit(f.ret, v);
                let mut params = Vec::with_capacity(f.params.len());
                for (p, kind) in f.params.iter().zip(f.ref_kinds.iter()) {
                    v.dynamic_index += match *kind {
                        super::fnptr::REF_NONE | super::fnptr::REF_REF => 1,
                        _ => 2,
                    };
                    params.push(self.visit(*p, v));
                }
                f.params = params.into();
                self.intern(TypeData::FnPtr(Box::new(f)))
            }
            TypeData::Def(_) => {
                let code = self.known_code(t);
                let mut new = t;
                if code == KnownTypeCode::Object && v.has_dynamic {
                    let dynamic = match &v.dynamic_data {
                        Some(data) if v.dynamic_index < data.len() => data[v.dynamic_index],
                        _ => true,
                    };
                    if dynamic {
                        new = self.intern(TypeData::Dynamic);
                    }
                } else if matches!(code, KnownTypeCode::IntPtr | KnownTypeCode::UIntPtr)
                    && v.has_native_integers
                {
                    let native = match &v.native_integer_data {
                        Some(data) if v.native_int_index < data.len() => data[v.native_int_index],
                        _ => true,
                    };
                    if native {
                        new = self.intern(if code == KnownTypeCode::IntPtr {
                            TypeData::NInt
                        } else {
                            TypeData::NUInt
                        });
                    }
                    v.native_int_index += 1;
                }
                if self.is_reference_type(t) == Some(true) {
                    let n = v.nullability();
                    self.change_nullability(new, n)
                } else {
                    new
                }
            }
            TypeData::TypeParam { .. } => {
                let n = v.nullability();
                self.change_nullability(t, n)
            }
            TypeData::Pinned(e) => {
                let e = self.visit(e, v);
                self.intern(TypeData::Pinned(e))
            }
            TypeData::Unknown {
                is_reference_type: Some(true),
                ..
            } => {
                let n = v.nullability();
                self.change_nullability(t, n)
            }
            _ => t,
        }
    }

    fn visit_parameterized(
        &mut self,
        t: TypeId,
        generic: TypeId,
        args: &[TypeId],
        v: &mut Visitor,
    ) -> TypeId {
        if v.options & options::TUPLE != 0
            && let Some(cardinality) = self.tuple_cardinality(t)
        {
            if cardinality > 1 {
                let assembly = self.definition(t).map(|d| d.module);
                let names = v.tuple_names.as_ref().and_then(|names| {
                    (v.tuple_index < names.len()).then(|| {
                        (0..cardinality)
                            .map(|i| names.get(v.tuple_index + i).cloned().flatten())
                            .collect::<Vec<_>>()
                    })
                });
                v.tuple_index += cardinality;
                v.nullability();
                let mut elements = Vec::with_capacity(cardinality);
                let mut current = Some(t);
                while let Some(cur) = current {
                    let TypeData::Inst { args, .. } = self.data(cur).clone() else {
                        break;
                    };
                    let normal = args.len().min(TUPLE_REST - 1);
                    for a in &args[..normal] {
                        v.dynamic_index += 1;
                        elements.push(self.visit(*a, v));
                    }
                    if args.len() == TUPLE_REST {
                        let rest = args[TUPLE_REST - 1];
                        v.nullability();
                        v.dynamic_index += 1;
                        match self.tuple_cardinality(rest) {
                            Some(nested) if matches!(self.data(rest), TypeData::Inst { .. }) => {
                                v.tuple_index += nested;
                                current = Some(rest);
                            }
                            _ => current = None,
                        }
                    } else {
                        current = None;
                    }
                }
                let underlying = self.tuple_underlying(&elements, assembly);
                let names: Box<[Option<Box<str>>]> = match names {
                    Some(names) => names
                        .into_iter()
                        .map(|n| n.map(String::into_boxed_str))
                        .collect(),
                    None => vec![None; elements.len()].into(),
                };
                return self.intern(TypeData::Tuple {
                    underlying,
                    elements: elements.into(),
                    names,
                });
            }
            v.tuple_index += cardinality;
        }
        let g = self.visit(generic, v);
        if self.is_reference_type(g) != Some(true) {
            let nullable_of_t = self
                .definition(g)
                .and_then(|d| self.comp.type_def(d).ok())
                .is_some_and(|i| i.known == KnownTypeCode::NullableOfT);
            if !nullable_of_t {
                v.nullability();
            }
        }
        let mut new_args = Vec::with_capacity(args.len());
        for a in args {
            v.dynamic_index += 1;
            new_args.push(self.visit(*a, v));
        }
        self.intern(TypeData::Inst {
            generic: g,
            args: new_args.into(),
        })
    }

    /// `IEntity.Accessibility` intersected with every enclosing type's
    /// (`EffectiveAccessibility`), as the managed enum value.
    pub fn effective_accessibility(
        &self,
        entity_accessibility: u8,
        declaring: Option<TypeDefId>,
    ) -> u8 {
        let mut acc = entity_accessibility;
        let mut current = declaring;
        while let Some(def) = current {
            let Ok(info) = self.comp.type_def(def) else {
                break;
            };
            acc = intersect(acc, type_accessibility(info.flags));
            current = (info.declaring != 0).then_some(TypeDefId {
                module: def.module,
                row: info.declaring,
            });
        }
        acc
    }

    /// `MetadataModule.OptionsForEntity`: nullability is dropped for entities less accessible
    /// than the module's `NullablePublicOnly` threshold. `None` stands for a null entity.
    pub fn options_for_entity(&self, module: ModuleId, effective_accessibility: Option<u8>) -> u32 {
        let options = self.options(module);
        if options & options::NULLABILITY == 0 {
            return options;
        }
        let min = min_accessibility_for_nrt(&self.comp.module(module).md);
        match (min, effective_accessibility) {
            (ACC_NONE, _) | (_, None) => options,
            (min, Some(acc)) if less_than_or_equal(min, acc) => options,
            _ => options & !options::NULLABILITY,
        }
    }

    /// `ITypeDefinition.NullableContext`.
    pub fn type_nullable_context(&self, def: TypeDefId) -> Nullability {
        let md = &self.comp.module(def.module).md;
        if let Some(n) = nullable_context(md, TableId::TypeDef, def.row) {
            return n;
        }
        match self.comp.type_def(def) {
            Ok(info) if info.declaring != 0 => self.type_nullable_context(TypeDefId {
                module: def.module,
                row: info.declaring,
            }),
            _ => nullable_context(md, TableId::Module, 1).unwrap_or(Nullability::Oblivious),
        }
    }
}

fn process_attribute(md: &Metadata<'_>, ca: u32, v: &mut Visitor) -> Result<()> {
    let opts = v.options;
    if opts & options::DYNAMIC != 0 && attribute_is(md, ca, "DynamicAttribute") {
        v.has_dynamic = true;
        let value = decode(md, ca)?;
        if let Some(data) = array_arg(&value, |x| match x {
            Value::Bool(b) => Some(*b),
            _ => None,
        }) {
            v.dynamic_data = Some(data);
        }
    } else if opts & options::NATIVE_INTEGERS != 0 && attribute_is(md, ca, "NativeIntegerAttribute")
    {
        v.has_native_integers = true;
        let value = decode(md, ca)?;
        if let Some(data) = array_arg(&value, |x| match x {
            Value::Bool(b) => Some(*b),
            _ => None,
        }) {
            v.native_integer_data = Some(data);
        }
    } else if opts & options::TUPLE != 0 && attribute_is(md, ca, "TupleElementNamesAttribute") {
        let value = decode(md, ca)?;
        if let Some(names) = array_arg(&value, |x| match x {
            Value::String(s) => Some(s.clone()),
            _ => None,
        }) {
            v.tuple_names = Some(names);
        }
    } else if opts & options::NULLABILITY != 0 && attribute_is(md, ca, "NullableAttribute") {
        let value = decode(md, ca)?;
        if let Some(data) = array_arg(&value, |x| match x {
            Value::U8(b) => Nullability::from_byte(*b),
            _ => None,
        }) {
            v.nullable_data = Some(data);
        } else if let [Value::U8(b)] = value.fixed.as_slice()
            && let Some(n) = Nullability::from_byte(*b)
        {
            v.default_nullability = n;
        }
    }
    Ok(())
}

pub const ACC_NONE: u8 = 0;
pub const ACC_PRIVATE: u8 = 1;
pub const ACC_PROTECTED_AND_INTERNAL: u8 = 2;
pub const ACC_PROTECTED: u8 = 3;
pub const ACC_INTERNAL: u8 = 4;
pub const ACC_PROTECTED_OR_INTERNAL: u8 = 5;
pub const ACC_PUBLIC: u8 = 6;

/// `MetadataTypeDefinition.Accessibility`.
pub fn type_accessibility(flags: u32) -> u8 {
    match flags & 7 {
        0 | 5 => ACC_INTERNAL,
        1 | 2 => ACC_PUBLIC,
        3 => ACC_PRIVATE,
        4 => ACC_PROTECTED,
        6 => ACC_PROTECTED_AND_INTERNAL,
        7 => ACC_PROTECTED_OR_INTERNAL,
        _ => ACC_NONE,
    }
}

/// The managed value of a member `Accessibility` name.
pub fn accessibility_value(name: &str) -> u8 {
    match name {
        "Private" => ACC_PRIVATE,
        "ProtectedAndInternal" => ACC_PROTECTED_AND_INTERNAL,
        "Protected" => ACC_PROTECTED,
        "Internal" => ACC_INTERNAL,
        "ProtectedOrInternal" => ACC_PROTECTED_OR_INTERNAL,
        "Public" => ACC_PUBLIC,
        _ => ACC_NONE,
    }
}

fn less_than_or_equal(a: u8, b: u8) -> bool {
    a <= b && !(a == ACC_PROTECTED && b == ACC_INTERNAL)
}

fn intersect(a: u8, b: u8) -> u8 {
    let (a, b) = if a > b { (b, a) } else { (a, b) };
    if a == ACC_PROTECTED && b == ACC_INTERNAL {
        ACC_PROTECTED_AND_INTERNAL
    } else {
        a
    }
}

/// `FindMinimumAccessibilityForNRT`.
fn min_accessibility_for_nrt(md: &Metadata<'_>) -> u8 {
    for ca in md.custom_attributes(TableId::Module, 1) {
        if !attribute_is(md, ca, "NullablePublicOnlyAttribute") {
            continue;
        }
        let Ok(value) = decode(md, ca) else {
            continue;
        };
        if let [Value::Bool(includes_internals)] = value.fixed.as_slice() {
            return if *includes_internals {
                ACC_PROTECTED_AND_INTERNAL
            } else {
                ACC_PROTECTED
            };
        }
    }
    ACC_NONE
}

/// The managed `Accessibility` name of a value.
pub fn accessibility_name(value: u8) -> &'static str {
    match value {
        ACC_PRIVATE => "Private",
        ACC_PROTECTED_AND_INTERNAL => "ProtectedAndInternal",
        ACC_PROTECTED => "Protected",
        ACC_INTERNAL => "Internal",
        ACC_PROTECTED_OR_INTERNAL => "ProtectedOrInternal",
        ACC_PUBLIC => "Public",
        _ => "None",
    }
}
