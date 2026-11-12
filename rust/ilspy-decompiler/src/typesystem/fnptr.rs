//! Function pointer types (`FunctionPointerType.FromSignature`), the type normalization member
//! lookup compares signatures under, and type parameter substitution.

use crate::metadata::Result;
use crate::metadata::signature::MethodSig;

use super::compilation::ModuleId;
use super::known::KnownTypeCode;
use super::types::{GenericContext, Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};

/// `ReferenceKind` values of function pointer parameters.
pub const REF_NONE: u8 = 0;
pub const REF_OUT: u8 = 1;
pub const REF_REF: u8 = 2;
pub const REF_IN: u8 = 3;
pub const REF_READONLY: u8 = 4;

/// The settings of a `NormalizeTypeVisitor`.
#[derive(Clone, Copy, Debug)]
pub struct Normalization {
    pub class_dummy: bool,
    pub method_dummy: bool,
    pub dynamic_and_object: bool,
    pub intptr_to_nint: bool,
    pub tuple_to_underlying: bool,
    pub remove_tuple_names: bool,
    pub remove_modopt: bool,
    pub remove_modreq: bool,
    pub remove_nullability: bool,
}

impl Normalization {
    /// The default `NormalizeTypeVisitor`.
    pub const MEMBER_LOOKUP: Normalization = Normalization {
        class_dummy: true,
        method_dummy: true,
        dynamic_and_object: true,
        intptr_to_nint: true,
        tuple_to_underlying: true,
        remove_tuple_names: true,
        remove_modopt: true,
        remove_modreq: true,
        remove_nullability: true,
    };

    /// The visitor of `ParameterListComparer`.
    pub const PARAMETER_LIST: Normalization = Normalization {
        class_dummy: false,
        ..Self::MEMBER_LOOKUP
    };

    /// `NormalizeTypeVisitor.TypeErasure`.
    pub const TYPE_ERASURE: Normalization = Normalization {
        class_dummy: false,
        method_dummy: false,
        remove_tuple_names: false,
        ..Self::MEMBER_LOOKUP
    };

    /// `NormalizeTypeVisitor.IgnoreNullabilityAndTuples`.
    pub const IGNORE_NULLABILITY_AND_TUPLES: Normalization = Normalization {
        dynamic_and_object: false,
        intptr_to_nint: false,
        ..Self::TYPE_ERASURE
    };

    /// `NormalizeTypeVisitor.KeyForTypeMerging`.
    pub const KEY_FOR_TYPE_MERGING: Normalization = Normalization {
        class_dummy: false,
        method_dummy: false,
        dynamic_and_object: true,
        intptr_to_nint: false,
        tuple_to_underlying: false,
        remove_tuple_names: true,
        remove_modopt: false,
        remove_modreq: false,
        remove_nullability: true,
    };

    /// `NormalizeTypeVisitor.IgnoreNullability`.
    pub const IGNORE_NULLABILITY: Normalization = Normalization {
        class_dummy: false,
        method_dummy: false,
        dynamic_and_object: false,
        intptr_to_nint: false,
        tuple_to_underlying: false,
        remove_tuple_names: false,
        remove_modopt: true,
        remove_modreq: true,
        remove_nullability: true,
    };
}

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub struct FnPtrType {
    /// `SignatureCallingConvention` (0 default, 1 cdecl, 2 stdcall, 3 thiscall, 4 fastcall,
    /// 5 varargs, 9 unmanaged).
    pub calling_convention: u8,
    pub custom_conventions: Box<[TypeId]>,
    pub ret: TypeId,
    pub ret_ref_readonly: bool,
    pub params: Box<[TypeId]>,
    pub ref_kinds: Box<[u8]>,
}

/// `SignatureHeader.CallingConvention`.
pub fn calling_convention(header: u8) -> u8 {
    match header & 0x0F {
        c @ (0..=5 | 9) => c,
        _ => 0,
    }
}

const COMPILER_SERVICES: &str = "System.Runtime.CompilerServices";
const INTEROP_SERVICES: &str = "System.Runtime.InteropServices";

impl TypeSystem<'_, '_> {
    /// `IType.Name` and `IType.Namespace` of a resolved type.
    pub fn name_and_namespace(&self, t: TypeId) -> Option<(String, String)> {
        let name = match self.data(t) {
            TypeData::Def(def) => self.comp.type_def(*def).ok()?.full_name.clone(),
            TypeData::Unknown { name, .. } => (**name).clone(),
            TypeData::Inst { generic, .. } => return self.name_and_namespace(*generic),
            _ => return None,
        };
        Some((name.name().to_owned(), name.top.ns))
    }

    /// `IType.IsKnownType(KnownAttribute)`: the definition is the top-level type `ns.name`.
    fn defines(&self, t: TypeId, ns: &str, name: &str) -> bool {
        self.definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .is_some_and(|i| {
                i.full_name.nested.is_empty()
                    && i.full_name.top.ns == ns
                    && i.full_name.top.name == name
                    && i.full_name.top.tpc == 0
            })
    }

    /// `TypeProvider.GetFunctionPointerType`: the parts are decoded raw; recognized modifiers
    /// become the calling convention, ref-readonly return and parameter ref kinds.
    pub(super) fn function_pointer(
        &mut self,
        module: ModuleId,
        sig: &MethodSig,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        self.function_pointer_with(module, sig, ctx, true)
    }

    /// `FunctionPointerType.FromSignature`, used for `calli` signatures: unlike nested
    /// function pointer types, instance method pointers keep their signature.
    pub fn function_pointer_from_sig(
        &mut self,
        module: ModuleId,
        sig: &MethodSig,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        self.function_pointer_with(module, sig, ctx, false)
    }

    fn function_pointer_with(
        &mut self,
        module: ModuleId,
        sig: &MethodSig,
        ctx: GenericContext,
        instance_as_intptr: bool,
    ) -> Result<TypeId> {
        let mut ret = self.from_sig_raw(module, &sig.ret, ctx)?;
        let raw_params = sig
            .params
            .iter()
            .map(|p| self.from_sig_raw(module, p, ctx))
            .collect::<Result<Vec<_>>>()?;
        if instance_as_intptr && sig.header & 0x20 != 0 {
            // Pointers to instance methods are typed as IntPtr.
            return Ok(self.known_type(KnownTypeCode::IntPtr));
        }
        let mut convention = calling_convention(sig.header);
        let mut custom = Vec::new();
        let mut ret_ref_readonly = false;
        while let TypeData::Modified {
            modifier, inner, ..
        } = self.data(ret).clone()
        {
            if self.defines(modifier, INTEROP_SERVICES, "InAttribute") {
                ret_ref_readonly = true;
            } else if let Some((name, ns)) = self.name_and_namespace(modifier)
                && name.starts_with("CallConv")
                && ns == COMPILER_SERVICES
            {
                match (convention, name.as_str()) {
                    (9, "CallConvCdecl") => convention = 1,
                    (9, "CallConvFastcall") => convention = 4,
                    (9, "CallConvStdcall") => convention = 2,
                    (9, "CallConvThiscall") => convention = 3,
                    _ => custom.push(modifier),
                }
            } else {
                break;
            }
            ret = inner;
        }
        let mut params = Vec::with_capacity(raw_params.len());
        let mut ref_kinds = Vec::with_capacity(raw_params.len());
        for p in raw_params {
            let (mut kind, ty) = match self.data(p).clone() {
                TypeData::Modified {
                    modifier, inner, ..
                } => {
                    if self.defines(modifier, INTEROP_SERVICES, "InAttribute") {
                        (REF_IN, inner)
                    } else if self.defines(modifier, INTEROP_SERVICES, "OutAttribute") {
                        (REF_OUT, inner)
                    } else if self.defines(modifier, COMPILER_SERVICES, "RequiresLocationAttribute")
                    {
                        (REF_READONLY, inner)
                    } else {
                        (REF_NONE, p)
                    }
                }
                _ => (REF_NONE, p),
            };
            if matches!(self.data(ty), TypeData::ByRef(_)) {
                if kind == REF_NONE {
                    kind = REF_REF;
                }
            } else {
                kind = REF_NONE;
            }
            params.push(ty);
            ref_kinds.push(kind);
        }
        self.known_type(KnownTypeCode::UIntPtr);
        Ok(self.intern(TypeData::FnPtr(Box::new(FnPtrType {
            calling_convention: convention,
            custom_conventions: custom.into(),
            ret,
            ret_ref_readonly,
            params: params.into(),
            ref_kinds: ref_kinds.into(),
        }))))
    }

    /// The `NormalizeTypeVisitor` member lookup uses (`Normalization::MEMBER_LOOKUP`): type
    /// parameters become dummies of their
    /// owner kind, unknown types forget whether they are reference types, modifiers,
    /// nullability and tuple names go, `object` becomes `dynamic` and `IntPtr`/`UIntPtr`
    /// become `nint`/`nuint`.
    pub fn normalize(&mut self, t: TypeId) -> TypeId {
        self.normalize_with(t, Normalization::MEMBER_LOOKUP)
    }

    /// The `ParameterListComparer` visitor: as `normalize`, but class type parameters stay.
    pub fn normalize_parameter(&mut self, t: TypeId) -> TypeId {
        self.normalize_with(t, Normalization::PARAMETER_LIST)
    }

    /// `type.AcceptVisitor(normalizeTypeVisitor)` for a `NormalizeTypeVisitor` with the
    /// settings `n`.
    pub fn normalize_with(&mut self, t: TypeId, n: Normalization) -> TypeId {
        let data = self.data(t).clone();
        let normalized = match data {
            TypeData::TypeParam { owner, index }
                if (owner.is_method() && n.method_dummy)
                    || (!owner.is_method() && n.class_dummy) =>
            {
                TypeData::TypeParam {
                    owner: if owner.is_method() {
                        TypeParamOwner::DummyMethod
                    } else {
                        TypeParamOwner::DummyType
                    },
                    index,
                }
            }
            TypeData::Unknown { name, .. } => TypeData::Unknown {
                name,
                is_reference_type: None,
            },
            TypeData::Def(def) => match self.comp.type_def(def).map(|i| i.known) {
                Ok(KnownTypeCode::Object) if n.dynamic_and_object => TypeData::Dynamic,
                Ok(KnownTypeCode::IntPtr) if n.intptr_to_nint => TypeData::NInt,
                Ok(KnownTypeCode::UIntPtr) if n.intptr_to_nint => TypeData::NUInt,
                _ => return t,
            },
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => {
                if (required && n.remove_modreq) || (!required && n.remove_modopt) {
                    return self.normalize_with(inner, n);
                }
                TypeData::Modified {
                    modifier,
                    inner: self.normalize_with(inner, n),
                    required,
                }
            }
            TypeData::Annotated { inner, nullability } => {
                if n.remove_nullability {
                    return self.normalize_with(inner, n);
                }
                let inner = self.normalize_with(inner, n);
                return self.change_nullability(inner, nullability);
            }
            TypeData::Tuple {
                underlying,
                elements,
                names,
            } => {
                if n.tuple_to_underlying {
                    return self.normalize_with(underlying, n);
                }
                let elements: Vec<TypeId> = elements
                    .iter()
                    .map(|e| self.normalize_with(*e, n))
                    .collect();
                let names = if n.remove_tuple_names {
                    vec![None; elements.len()].into()
                } else {
                    names
                };
                let assembly = self.definition(t).map(|d| d.module);
                TypeData::Tuple {
                    underlying: self.tuple_underlying(&elements, assembly),
                    elements: elements.into(),
                    names,
                }
            }
            TypeData::Inst { generic, args } => TypeData::Inst {
                generic: self.normalize_with(generic, n),
                args: args.iter().map(|a| self.normalize_with(*a, n)).collect(),
            },
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => TypeData::Array {
                elem: self.normalize_with(elem, n),
                rank,
                nullability: if n.remove_nullability {
                    Nullability::Oblivious
                } else {
                    nullability
                },
            },
            TypeData::Pointer(e) => TypeData::Pointer(self.normalize_with(e, n)),
            TypeData::ByRef(e) => TypeData::ByRef(self.normalize_with(e, n)),
            TypeData::Pinned(e) => TypeData::Pinned(self.normalize_with(e, n)),
            TypeData::FnPtr(f) => {
                let mut f = *f;
                f.ret = self.normalize_with(f.ret, n);
                f.params = f
                    .params
                    .iter()
                    .map(|p| self.normalize_with(*p, n))
                    .collect();
                TypeData::FnPtr(Box::new(f))
            }
            other => other,
        };
        self.intern(normalized)
    }

    /// `IType.Nullability`.
    pub fn nullability_of(&self, t: TypeId) -> Nullability {
        match self.data(t) {
            TypeData::Annotated { nullability, .. } | TypeData::Array { nullability, .. } => {
                *nullability
            }
            TypeData::Inst { generic, .. } => self.nullability_of(*generic),
            _ => Nullability::Oblivious,
        }
    }

    /// `TypeParameterSubstitution` applied to a type: class and method type parameters are
    /// replaced by index (out of range gives `?`); `None` lists leave that kind alone.
    pub fn substitute(
        &mut self,
        t: TypeId,
        class: Option<&[TypeId]>,
        method: Option<&[TypeId]>,
    ) -> TypeId {
        let data = self.data(t).clone();
        let replaced = match data {
            TypeData::TypeParam { owner, index } => {
                let args = if owner.is_method() { method } else { class };
                if let Some(args) = args {
                    return args
                        .get(index as usize)
                        .copied()
                        .unwrap_or_else(|| self.intern(TypeData::UnknownSpecial));
                }
                return t;
            }
            TypeData::Annotated { inner, nullability } => {
                if let TypeData::TypeParam { owner, .. } = *self.data(inner) {
                    // NullabilityAnnotatedTypeParameter: a substitution for the parameter's
                    // owner kind replaces it (even by an identical parameter) and keeps only `?`.
                    let applies = if owner.is_method() {
                        method.is_some()
                    } else {
                        class.is_some()
                    };
                    if !applies {
                        return t;
                    }
                    let s = self.substitute(inner, class, method);
                    return if nullability == Nullability::Nullable {
                        self.change_nullability(s, Nullability::Nullable)
                    } else {
                        s
                    };
                }
                let s = self.substitute(inner, class, method);
                if s == inner {
                    return t;
                }
                if self.nullability_of(s) == Nullability::Nullable {
                    return s;
                }
                if matches!(self.data(s), TypeData::TypeParam { .. })
                    || self.is_reference_type(s) == Some(true)
                {
                    return self.change_nullability(s, nullability);
                }
                return s;
            }
            TypeData::Inst { generic, args } => TypeData::Inst {
                generic: self.substitute(generic, class, method),
                args: args
                    .iter()
                    .map(|a| self.substitute(*a, class, method))
                    .collect(),
            },
            TypeData::Tuple {
                elements, names, ..
            } => {
                let elements: Vec<TypeId> = elements
                    .iter()
                    .map(|e| self.substitute(*e, class, method))
                    .collect();
                let assembly = self.definition(t).map(|d| d.module);
                let underlying = self.tuple_underlying(&elements, assembly);
                TypeData::Tuple {
                    underlying,
                    elements: elements.into(),
                    names,
                }
            }
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => TypeData::Array {
                elem: self.substitute(elem, class, method),
                rank,
                nullability,
            },
            TypeData::Pointer(e) => TypeData::Pointer(self.substitute(e, class, method)),
            TypeData::ByRef(e) => TypeData::ByRef(self.substitute(e, class, method)),
            TypeData::Pinned(e) => TypeData::Pinned(self.substitute(e, class, method)),
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => TypeData::Modified {
                modifier: self.substitute(modifier, class, method),
                inner: self.substitute(inner, class, method),
                required,
            },
            TypeData::FnPtr(f) => {
                let mut f = *f;
                f.ret = self.substitute(f.ret, class, method);
                f.params = f
                    .params
                    .iter()
                    .map(|p| self.substitute(*p, class, method))
                    .collect();
                TypeData::FnPtr(Box::new(f))
            }
            _ => return t,
        };
        self.intern(replaced)
    }
}
