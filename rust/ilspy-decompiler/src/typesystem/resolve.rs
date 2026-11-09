//! Member references: `MetadataModule.ResolveMethod` and `ResolveEntity` for MemberRef and
//! MethodSpec tokens, with signature-based lookup, fake members for references that resolve
//! to nothing, specialization by `TypeParameterSubstitution` and vararg expansion.
//!
//! A resolved member is a definition (or fake) plus the substitution applied to it; the
//! substituted signature is computed once and kept with it.

use crate::metadata::bytes::Bytes;
use crate::metadata::signature::{self, MethodSig};
use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Error, Result};

use super::attrs::AttributeSource;
use super::attrvalue::ArgValue;
use super::compilation::{ModuleId, TypeDefId, TypeKind, is_known_type_name};
use super::known::KnownTypeCode;
use super::members::{MethodDefId, SymbolKind};
use super::types::{GenericContext, Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};

/// `TypeAttributes.Import`.
const TYPE_ATTR_IMPORT: u32 = 0x1000;

/// What a resolved method is an instance of.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum MethodOrigin {
    Def(MethodDefId),
    /// The implicit parameterless constructor of a struct or enum without one.
    DummyConstructor(TypeDefId),
    /// A reference that matched no method.
    Fake,
}

#[derive(Clone, Debug)]
pub struct ResolvedMethod {
    pub origin: MethodOrigin,
    pub declaring: Option<TypeId>,
    pub name: String,
    pub is_static: bool,
    /// The substitution, `None` for an unspecialized member.
    pub class_args: Option<Vec<TypeId>>,
    pub method_args: Option<Vec<TypeId>>,
    pub type_args: Vec<TypeId>,
    pub ret: TypeId,
    /// Parameter types of the (specialized) method itself.
    pub base_params: Vec<TypeId>,
    /// For a vararg call site: the types passed after the sentinel. The last base parameter
    /// (`__arglist`) is replaced by them.
    pub varargs: Option<Vec<TypeId>>,
    /// `MethodSemanticsAttributes` of an accessor, 0 otherwise.
    pub accessor_kind: u16,
}

impl ResolvedMethod {
    /// `IMethod.Parameters` types.
    pub fn params(&self) -> Result<Vec<TypeId>> {
        let mut params = self.base_params.clone();
        if let Some(extra) = &self.varargs {
            if params.pop().is_none() {
                return Err(Error::Malformed("vararg method without parameters"));
            }
            params.extend_from_slice(extra);
        }
        Ok(params)
    }
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum FieldOrigin {
    Def { module: ModuleId, row: u32 },
    Fake,
}

#[derive(Clone, Debug)]
pub struct ResolvedField {
    pub origin: FieldOrigin,
    pub declaring: Option<TypeId>,
    pub name: String,
    pub ty: TypeId,
}

const SIG_INSTANCE: u8 = 0x20;

impl TypeSystem<'_, '_> {
    /// `IType.TypeArguments`: a definition's own type parameters, a parameterized type's
    /// arguments, nothing otherwise.
    pub fn type_arguments(&mut self, t: TypeId) -> Vec<TypeId> {
        match self.data(t).clone() {
            TypeData::Inst { args, .. } => args.to_vec(),
            TypeData::Def(def) => {
                let count = self
                    .comp
                    .type_def(def)
                    .map_or(0, |i| i.type_parameter_count);
                (0..count)
                    .map(|i| {
                        let owner = self.class_param_owner(def, i);
                        self.intern(TypeData::TypeParam { owner, index: i })
                    })
                    .collect()
            }
            _ => Vec::new(),
        }
    }

    fn kind_of(&self, t: TypeId) -> Option<TypeKind> {
        match self.data(t) {
            TypeData::Def(d) => self.comp.type_def(*d).ok().map(|i| i.kind),
            TypeData::Inst { generic, .. } => self.kind_of(*generic),
            _ => None,
        }
    }

    /// The method definition as `MetadataMethod` presents it, unspecialized.
    pub fn method_definition(&mut self, m: MethodDefId) -> Result<ResolvedMethod> {
        let md = &self.comp.module(m.module).md;
        let def = md.get::<rows::MethodDef>(m.row)?;
        let name = md.string(def.name)?.into_owned();
        let declaring_row = md.method_declaring_type(m.row)?;
        let tp_count = md.generic_params(TableId::MethodDef, m.row).len() as u32;
        let declaring = (declaring_row != 0).then(|| {
            self.intern(TypeData::Def(TypeDefId {
                module: m.module,
                row: declaring_row,
            }))
        });
        let type_args = (0..tp_count)
            .map(|index| {
                self.intern(TypeData::TypeParam {
                    owner: TypeParamOwner::Method(m),
                    index,
                })
            })
            .collect();
        let (ret, params) = self.method_signature(m);
        let accessor_kind = self.accessor_owner(m).map_or(0, |(_, kind)| kind);
        Ok(ResolvedMethod {
            origin: MethodOrigin::Def(m),
            declaring,
            name,
            is_static: def.flags & 0x0010 != 0,
            class_args: None,
            method_args: None,
            type_args,
            ret,
            base_params: params.iter().map(|p| p.ty).collect(),
            varargs: None,
            accessor_kind,
        })
    }

    /// `IMember.Specialize(new TypeParameterSubstitution(class, method))`.
    pub fn specialize(
        &mut self,
        mut m: ResolvedMethod,
        class: Option<Vec<TypeId>>,
        method: Option<Vec<TypeId>>,
    ) -> ResolvedMethod {
        // A generic method specialized only by its class gets specialized type parameters of
        // its own (`SpecializedTypeParameter`); substituting them drops `T!` annotations.
        let own_type_params =
            (method.is_none() && !m.type_args.is_empty()).then(|| m.type_args.clone());
        let (c, me) = (
            class.as_deref(),
            method.as_deref().or(own_type_params.as_deref()),
        );
        m.declaring = m.declaring.map(|d| match self.data(d).clone() {
            TypeData::Def(def)
                if self
                    .comp
                    .type_def(def)
                    .is_ok_and(|i| i.type_parameter_count > 0) =>
            {
                let tpc = self
                    .comp
                    .type_def(def)
                    .map_or(0, |i| i.type_parameter_count) as usize;
                match c {
                    Some(args) if args.len() == tpc => self.intern(TypeData::Inst {
                        generic: d,
                        args: args.into(),
                    }),
                    _ => {
                        let own = self.type_arguments(d);
                        let inst = self.intern(TypeData::Inst {
                            generic: d,
                            args: own.into(),
                        });
                        self.substitute(inst, c, me)
                    }
                }
            }
            _ => self.substitute(d, c, me),
        });
        m.ret = self.substitute(m.ret, c, me);
        m.base_params = m
            .base_params
            .iter()
            .map(|p| self.substitute(*p, c, me))
            .collect();
        if let Some(extra) = &m.varargs {
            m.varargs = Some(extra.iter().map(|p| self.substitute(*p, c, me)).collect());
        }
        if let Some(args) = &method {
            m.type_args = args.clone();
        }
        m.class_args = class;
        m.method_args = method;
        m
    }

    /// `TypeDefinition.Methods`: non-accessor methods in row order, plus the implicit
    /// constructor of a struct or enum without a parameterless one.
    pub fn definition_methods(
        &mut self,
        def: TypeDefId,
    ) -> Result<Vec<(ResolvedMethod, SymbolKind)>> {
        let md = &self.comp.module(def.module).md;
        let info = self.comp.type_def(def)?;
        let kind = info.kind;
        let mut result = Vec::new();
        let mut has_default_ctor = false;
        for row in md.type_methods(def.row)? {
            let m = MethodDefId {
                module: def.module,
                row,
            };
            if self.accessor_owner(m).is_some() || !self.comp.is_visible_method(def.module, row) {
                continue;
            }
            let symbol = self.method_symbol_kind(m, None)?;
            let method = self.method_definition(m)?;
            if symbol == SymbolKind::Constructor
                && !method.is_static
                && method.base_params.is_empty()
            {
                has_default_ctor = true;
            }
            result.push((method, symbol));
        }
        if !has_default_ctor && matches!(kind, TypeKind::Struct | TypeKind::Enum) {
            let declaring = self.intern(TypeData::Def(def));
            let void = self.known_type(KnownTypeCode::Void);
            result.push((
                ResolvedMethod {
                    origin: MethodOrigin::DummyConstructor(def),
                    declaring: Some(declaring),
                    name: ".ctor".to_owned(),
                    is_static: false,
                    class_args: None,
                    method_args: None,
                    type_args: Vec::new(),
                    ret: void,
                    base_params: Vec::new(),
                    varargs: None,
                    accessor_kind: 0,
                },
                SymbolKind::Constructor,
            ));
        }
        Ok(result)
    }

    /// `GetFilteredAccessors`: getters and setters of visible properties, then adders and
    /// removers of events with an adder. When an event's invoker matches, its remover is
    /// yielded (as the managed code does). `name` filters by accessor name.
    pub fn definition_accessors(
        &mut self,
        def: TypeDefId,
        name: Option<&str>,
    ) -> Result<Vec<Option<ResolvedMethod>>> {
        let md = &self.comp.module(def.module).md;
        let mut result = Vec::new();
        let named = |ts: &Self, row: u32| {
            row != 0
                && name.is_none_or(|name| {
                    ts.comp
                        .module(def.module)
                        .md
                        .get::<rows::MethodDef>(row)
                        .and_then(|m| ts.comp.module(def.module).md.string(m.name))
                        .is_ok_and(|n| n == name)
                })
        };
        for p in self.comp.visible_properties(def)? {
            let (get, set, _) = md.property_accessors(p);
            for row in [get, set] {
                if named(self, row) {
                    result.push(Some(self.method_definition(MethodDefId {
                        module: def.module,
                        row,
                    })?));
                }
            }
        }
        for e in self.comp.visible_events(def)? {
            let (add, remove, raise, _) = md.event_accessors(e);
            for row in [add, remove] {
                if named(self, row) {
                    result.push(Some(self.method_definition(MethodDefId {
                        module: def.module,
                        row,
                    })?));
                }
            }
            if named(self, raise) {
                result.push(match remove {
                    0 => None,
                    row => Some(self.method_definition(MethodDefId {
                        module: def.module,
                        row,
                    })?),
                });
            }
        }
        Ok(result)
    }

    fn same_type(&mut self, a: TypeId, b: TypeId) -> bool {
        self.normalize(a) == self.normalize(b)
    }

    /// `ITypeDefinition.GetConstructors()`: instance constructors; for a ComImport interface,
    /// those of its CoClass re-declared by the interface.
    pub fn definition_constructors(&mut self, def: TypeDefId) -> Result<Vec<ResolvedMethod>> {
        self.constructors(def, &mut Vec::new())
    }

    /// `definition_constructors`, with `busy` guarding CoClass cycles.
    fn constructors(
        &mut self,
        def: TypeDefId,
        busy: &mut Vec<TypeDefId>,
    ) -> Result<Vec<ResolvedMethod>> {
        let info = self.comp.type_def(def)?;
        if info.kind == TypeKind::Void {
            return Ok(Vec::new());
        }
        if info.kind != TypeKind::Interface || info.flags & TYPE_ATTR_IMPORT == 0 {
            return Ok(self
                .definition_methods(def)?
                .into_iter()
                .filter(|(m, k)| *k == SymbolKind::Constructor && !m.is_static)
                .map(|(m, _)| m)
                .collect());
        }
        if busy.contains(&def) {
            return Ok(Vec::new());
        }
        busy.push(def);
        let coclass = self.coclass(def);
        let ctors = match coclass.map(|t| self.data(t).clone()) {
            Some(TypeData::Def(c)) => self.constructors(c, busy),
            Some(TypeData::Inst { generic, args }) => match self.definition(generic) {
                Some(c) => self.constructors(c, busy).map(|ctors| {
                    ctors
                        .into_iter()
                        .map(|m| self.specialize(m, Some(args.to_vec()), None))
                        .collect()
                }),
                None => Ok(Vec::new()),
            },
            // Unknown, array and other types have no constructors.
            _ => Ok(Vec::new()),
        };
        busy.pop();
        let declaring = self.intern(TypeData::Def(def));
        Ok(ctors?
            .into_iter()
            .map(|mut m| {
                m.declaring = Some(declaring);
                m
            })
            .collect())
    }

    /// `ComHelper.GetCoClass`: the `Type` argument of the first `[CoClass]` on the type, `None`
    /// when it is missing or cannot be decoded.
    fn coclass(&mut self, def: TypeDefId) -> Option<TypeId> {
        let md = &self.comp.module(def.module).md;
        let row = md
            .custom_attributes(TableId::TypeDef, def.row)
            .into_iter()
            .find(|&ca| {
                md.attribute_type(ca).is_ok_and(|t| {
                    is_known_type_name(
                        md,
                        t,
                        "System.Runtime.InteropServices",
                        "CoClassAttribute",
                        0,
                    )
                })
            })?;
        let value = self.decode_attribute(def.module, row).ok()?;
        match value.fixed.as_slice() {
            [arg] => match arg.value {
                ArgValue::Type(t) => Some(t),
                _ => None,
            },
            _ => None,
        }
    }

    /// `MetadataModule.FindMethod(declaringType, name, signature)` for a static method without
    /// type parameters: the method `declaring` (specialized as it is) declares with these
    /// parameter and return types.
    pub fn find_static_method(
        &mut self,
        declaring: TypeId,
        name: &str,
        ret: TypeId,
        params: &[TypeId],
    ) -> Result<Option<ResolvedMethod>> {
        let Some(def) = self.definition(declaring) else {
            return Ok(None);
        };
        let class = self.type_arguments(declaring);
        for (m, kind) in self.definition_methods(def)? {
            if kind == SymbolKind::Constructor
                || m.name != name
                || !m.is_static
                || !m.type_args.is_empty()
            {
                continue;
            }
            let m = if class.is_empty() {
                m
            } else {
                self.specialize(m, Some(class.clone()), None)
            };
            if m.base_params.len() == params.len()
                && m.base_params
                    .clone()
                    .iter()
                    .zip(params)
                    .all(|(a, b)| self.same_type(*b, *a))
                && self.same_type(m.ret, ret)
            {
                return Ok(Some(m));
            }
        }
        Ok(None)
    }

    /// `MetadataModule.FindMethod` on a type definition.
    fn find_method(
        &mut self,
        def: TypeDefId,
        name: &str,
        sig: &MethodSig,
        ret: TypeId,
        params: &[TypeId],
    ) -> Result<Option<ResolvedMethod>> {
        let info = self.comp.type_def(def)?;
        if info.kind == TypeKind::Void {
            return Ok(None);
        }
        let candidates: Vec<Option<ResolvedMethod>> = if name == ".ctor" {
            self.definition_constructors(def)?
                .into_iter()
                .map(Some)
                .collect()
        } else if name == ".cctor" {
            self.definition_methods(def)?
                .into_iter()
                .filter(|(m, k)| *k == SymbolKind::Constructor && m.is_static)
                .map(|(m, _)| Some(m))
                .collect()
        } else {
            let mut c: Vec<Option<ResolvedMethod>> = self
                .definition_methods(def)?
                .into_iter()
                .filter(|(m, k)| *k != SymbolKind::Constructor && m.name == name)
                .map(|(m, _)| Some(m))
                .collect();
            c.extend(self.definition_accessors(def, Some(name))?);
            c
        };
        let mut expected = params.to_vec();
        if sig.header & 0x0F == 0x05 {
            expected.truncate(sig.required_params);
            expected.push(self.intern(TypeData::ArgList));
        }
        for candidate in candidates {
            let Some(m) = candidate else {
                return Err(Error::Unsupported("null accessor candidate"));
            };
            if m.type_args.len() as u32 != sig.generic_params {
                continue;
            }
            if (sig.header & SIG_INSTANCE != 0) == m.is_static {
                continue;
            }
            if m.base_params.len() == expected.len()
                && m.base_params
                    .clone()
                    .iter()
                    .zip(&expected)
                    .all(|(a, b)| self.same_type(*b, *a))
                && self.same_type(m.ret, ret)
            {
                return Ok(Some(m));
            }
        }
        Ok(None)
    }

    /// `MetadataModule.CreateFakeMethod` plus `GuessFakeMethodAccessor`.
    fn fake_method(
        &mut self,
        module: ModuleId,
        declaring: TypeId,
        name: &str,
        sig: &MethodSig,
        ret: TypeId,
        params: &[TypeId],
    ) -> ResolvedMethod {
        let type_args: Vec<TypeId> = (0..sig.generic_params)
            .map(|index| {
                self.intern(TypeData::TypeParam {
                    owner: TypeParamOwner::DummyMethod,
                    index,
                })
            })
            .collect();
        let declaring_args = self.type_arguments(declaring);
        let (class, method) = if sig.generic_params > 0 {
            (Some(declaring_args), Some(type_args.clone()))
        } else if !declaring_args.is_empty()
            && matches!(self.data(declaring), TypeData::Inst { .. })
        {
            (Some(declaring_args), None)
        } else {
            (None, None)
        };
        // ApplyAttributesAndSubstitutionToType: attribute semantics without attributes, then the
        // substitution.
        let options = self.options(module);
        let apply = |ts: &mut Self, t: TypeId| {
            let t = ts
                .apply_attributes(
                    t,
                    module,
                    AttributeSource::None,
                    options,
                    Nullability::Oblivious,
                    false,
                )
                .unwrap_or(t);
            ts.substitute(t, class.as_deref(), method.as_deref())
        };
        let ret = apply(self, ret);
        let base_params: Vec<TypeId> = params[..sig.required_params]
            .iter()
            .map(|p| apply(self, *p))
            .collect();
        let is_void = |ts: &Self, t: TypeId| ts.kind_of(t) == Some(TypeKind::Void);
        let accessor_kind = if !type_args.is_empty() {
            0
        } else if name.starts_with("get_") {
            if is_void(self, ret) { 0 } else { 0x02 }
        } else if name.starts_with("set_") {
            if base_params.is_empty() || !is_void(self, ret) {
                0
            } else {
                0x01
            }
        } else if name.starts_with("add_") {
            if base_params.len() == 1 { 0x08 } else { 0 }
        } else if name.starts_with("remove_") {
            if base_params.len() == 1 { 0x10 } else { 0 }
        } else if name.starts_with("raise_") {
            0x20
        } else {
            0
        };
        ResolvedMethod {
            origin: MethodOrigin::Fake,
            declaring: Some(declaring),
            name: name.to_owned(),
            is_static: sig.header & SIG_INSTANCE == 0,
            class_args: None,
            method_args: None,
            type_args,
            ret,
            base_params,
            varargs: None,
            accessor_kind,
        }
    }

    /// `ResolveMethodDefinition(expandVarArgs: true)`.
    fn resolve_method_def(&mut self, m: MethodDefId, expand: bool) -> Result<ResolvedMethod> {
        let mut method = self.method_definition(m)?;
        if expand
            && method
                .base_params
                .last()
                .is_some_and(|p| matches!(self.data(*p), TypeData::ArgList))
        {
            method.varargs = Some(Vec::new());
        }
        Ok(method)
    }

    /// `ResolveMethodReference`.
    fn resolve_method_ref(
        &mut self,
        module: ModuleId,
        row: u32,
        ctx: GenericContext,
        method_args: Option<Vec<TypeId>>,
        expand: bool,
    ) -> Result<ResolvedMethod> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let mr = md.get::<rows::MemberRef>(row)?;
        let blob = md.blob(mr.signature)?;
        if signature::header_kind(*blob.first().ok_or(Error::Truncated)?)
            != signature::HEADER_KIND_METHOD
        {
            return Err(Error::Malformed("member reference must be a method"));
        }
        let sig = signature::decode_method(&mut Bytes::new(blob))?;
        let mut class_args = None;
        let (mut method, sig_params) = match mr.class.decode()? {
            (TableId::MethodDef, def_row) => {
                let method = self.resolve_method_def(
                    MethodDefId {
                        module,
                        row: def_row,
                    },
                    false,
                )?;
                let params = sig
                    .params
                    .iter()
                    .map(|p| self.from_sig_raw(module, p, ctx))
                    .collect::<Result<Vec<_>>>()?;
                self.from_sig_raw(module, &sig.ret, ctx)?;
                (method, params)
            }
            _ => {
                let parent = mr.class.token()?;
                let declaring = self.resolve_declaring_type(module, parent, ctx)?;
                let declaring_def = self.definition(declaring);
                let args = self.type_arguments(declaring);
                if !args.is_empty() {
                    class_args = Some(args);
                }
                let name = md.string(mr.name)?.into_owned();
                let sig_ctx = self.type_context(declaring_def);
                let ret = self.from_sig_raw(module, &sig.ret, sig_ctx)?;
                let params = sig
                    .params
                    .iter()
                    .map(|p| self.from_sig_raw(module, p, sig_ctx))
                    .collect::<Result<Vec<_>>>()?;
                let found = match declaring_def {
                    Some(def) => self.find_method(def, &name, &sig, ret, &params)?,
                    None => None,
                };
                let method = match found {
                    Some(m) => m,
                    None => self.fake_method(module, declaring, &name, &sig, ret, &params),
                };
                (method, params)
            }
        };
        if class_args.is_some() || method_args.is_some() {
            method = self.specialize(method, class_args, method_args);
        }
        if expand && sig.header & 0x0F == 0x05 {
            method.varargs = Some(sig_params[sig.required_params..].to_vec());
        }
        Ok(method)
    }

    /// `ResolveMethodSpecification`.
    fn resolve_method_spec(
        &mut self,
        module: ModuleId,
        row: u32,
        ctx: GenericContext,
        expand: bool,
    ) -> Result<ResolvedMethod> {
        let md = &self.comp.module(module).md;
        let spec = md.get::<rows::MethodSpec>(row)?;
        let args = signature::decode_method_spec(&mut Bytes::new(md.blob(spec.instantiation)?))?
            .iter()
            .map(|a| self.from_sig(module, a, ctx))
            .collect::<Result<Vec<_>>>()?;
        match spec.method.decode()? {
            (TableId::MethodDef, def_row) => {
                let method = self.resolve_method_def(
                    MethodDefId {
                        module,
                        row: def_row,
                    },
                    expand,
                )?;
                Ok(self.specialize(method, None, Some(args)))
            }
            (TableId::MemberRef, ref_row) => {
                self.resolve_method_ref(module, ref_row, ctx, Some(args), expand)
            }
            _ => Err(Error::Malformed("method specification parent")),
        }
    }

    /// `MetadataModule.ResolveMethod`: a MethodDef, MemberRef or MethodSpec token, with
    /// vararg call sites expanded.
    pub fn resolve_method(
        &mut self,
        module: ModuleId,
        token: u32,
        ctx: GenericContext,
    ) -> Result<ResolvedMethod> {
        let row = token & 0x00FF_FFFF;
        match token >> 24 {
            0x06 => self.resolve_method_def(MethodDefId { module, row }, true),
            0x0A => self.resolve_method_ref(module, row, ctx, None, true),
            0x2B => self.resolve_method_spec(module, row, ctx, true),
            _ => Err(Error::Malformed("not a method token")),
        }
    }

    /// `ResolveFieldReference`.
    pub fn resolve_field_ref(
        &mut self,
        module: ModuleId,
        row: u32,
        ctx: GenericContext,
    ) -> Result<ResolvedField> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let mr = md.get::<rows::MemberRef>(row)?;
        let declaring = self.resolve_declaring_type(module, mr.class.token()?, ctx)?;
        let declaring_def = self.definition(declaring);
        let name = md.string(mr.name)?.into_owned();
        let sig = signature::decode_field(&mut Bytes::new(md.blob(mr.signature)?))?;
        let field_type = self.from_sig_raw(module, &sig, self.type_context(declaring_def))?;
        let class = match self.data(declaring) {
            TypeData::Inst { args, .. } => Some(args.to_vec()),
            _ => None,
        };
        if let Some(def) = declaring_def
            && self.comp.type_def(def)?.kind != TypeKind::Void
        {
            let fmd = &comp.module(def.module).md;
            for f in comp.visible_fields(def)? {
                let frow = fmd.get::<rows::Field>(f)?;
                if fmd.string(frow.name)? != name {
                    continue;
                }
                let (ty, _) = self.field_type(def.module, f);
                if self.same_type(ty, field_type) {
                    let ty = self.substitute(ty, class.as_deref(), None);
                    let generic = self.intern_def(def);
                    let declaring = match &class {
                        Some(args) => self.intern(TypeData::Inst {
                            generic,
                            args: args.clone().into(),
                        }),
                        None => generic,
                    };
                    return Ok(ResolvedField {
                        origin: FieldOrigin::Def {
                            module: def.module,
                            row: f,
                        },
                        declaring: Some(declaring),
                        name,
                        ty,
                    });
                }
            }
        }
        let options = self.options(module);
        let ty = self.apply_attributes(
            field_type,
            module,
            AttributeSource::None,
            options,
            Nullability::Oblivious,
            false,
        )?;
        let ty = self.substitute(ty, class.as_deref(), None);
        Ok(ResolvedField {
            origin: FieldOrigin::Fake,
            declaring: Some(declaring),
            name,
            ty,
        })
    }

    pub fn intern_def(&mut self, def: TypeDefId) -> TypeId {
        self.intern(TypeData::Def(def))
    }
}
