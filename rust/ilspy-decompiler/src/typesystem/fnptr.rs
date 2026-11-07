//! Function pointer types (`FunctionPointerType.FromSignature`) and the type normalization
//! member lookup compares signatures under.

use crate::metadata::Result;
use crate::metadata::signature::{MethodSig, SigType};

use super::compilation::ModuleId;
use super::known::KnownTypeCode;
use super::types::{GenericContext, TypeData, TypeId, TypeParamOwner, TypeSystem};

/// `ReferenceKind` values of function pointer parameters.
pub const REF_NONE: u8 = 0;
pub const REF_OUT: u8 = 1;
pub const REF_REF: u8 = 2;
pub const REF_IN: u8 = 3;
pub const REF_READONLY: u8 = 4;

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

    pub(super) fn function_pointer(
        &mut self,
        module: ModuleId,
        sig: &MethodSig,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        if sig.header & 0x20 != 0 {
            // Pointers to instance methods are typed as IntPtr; the signature is still decoded.
            self.from_sig(module, &sig.ret, ctx)?;
            for p in &sig.params {
                self.from_sig(module, p, ctx)?;
            }
            return Ok(self.known_type(KnownTypeCode::IntPtr));
        }
        let mut convention = calling_convention(sig.header);
        let mut custom = Vec::new();
        let mut ret_ref_readonly = false;
        let mut ret = &sig.ret;
        while let SigType::Modified {
            modifier, inner, ..
        } = ret
        {
            let m = self.from_sig(module, modifier, ctx)?;
            if self.defines(m, INTEROP_SERVICES, "InAttribute") {
                ret_ref_readonly = true;
            } else if let Some((name, ns)) = self.name_and_namespace(m)
                && name.starts_with("CallConv")
                && ns == COMPILER_SERVICES
            {
                match (convention, name.as_str()) {
                    (9, "CallConvCdecl") => convention = 1,
                    (9, "CallConvFastcall") => convention = 4,
                    (9, "CallConvStdcall") => convention = 2,
                    (9, "CallConvThiscall") => convention = 3,
                    _ => custom.push(m),
                }
            } else {
                break;
            }
            ret = inner;
        }
        let ret = self.from_sig(module, ret, ctx)?;
        let mut params = Vec::with_capacity(sig.params.len());
        let mut ref_kinds = Vec::with_capacity(sig.params.len());
        for p in &sig.params {
            let (mut kind, rest, unrecognized) = match p {
                SigType::Modified {
                    modifier, inner, ..
                } => {
                    let m = self.from_sig(module, modifier, ctx)?;
                    if self.defines(m, INTEROP_SERVICES, "InAttribute") {
                        (REF_IN, &**inner, false)
                    } else if self.defines(m, INTEROP_SERVICES, "OutAttribute") {
                        (REF_OUT, &**inner, false)
                    } else if self.defines(m, COMPILER_SERVICES, "RequiresLocationAttribute") {
                        (REF_READONLY, &**inner, false)
                    } else {
                        (REF_NONE, p, true)
                    }
                }
                _ => (REF_NONE, p, false),
            };
            let ty = self.from_sig(module, rest, ctx)?;
            // An unrecognized outer modifier hides the by-ref from the kind check.
            if !unrecognized && matches!(self.data(ty), TypeData::ByRef(_)) {
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

    /// The `NormalizeTypeVisitor` member lookup uses: type parameters become dummies of their
    /// owner kind and unknown types forget whether they are reference types. (Under
    /// `TypeSystemOptions.None` there is no dynamic, nint, tuple or nullability to erase.)
    pub fn normalize(&mut self, t: TypeId) -> TypeId {
        let data = self.data(t).clone();
        let normalized = match data {
            TypeData::TypeParam { owner, index } => TypeData::TypeParam {
                owner: if owner.is_method() {
                    TypeParamOwner::DummyMethod
                } else {
                    TypeParamOwner::DummyType
                },
                index,
            },
            TypeData::Unknown { name, .. } => TypeData::Unknown {
                name,
                is_reference_type: None,
            },
            TypeData::Inst { generic, args } => TypeData::Inst {
                generic: self.normalize(generic),
                args: args.iter().map(|a| self.normalize(*a)).collect(),
            },
            TypeData::Array { elem, rank } => TypeData::Array {
                elem: self.normalize(elem),
                rank,
            },
            TypeData::Pointer(e) => TypeData::Pointer(self.normalize(e)),
            TypeData::ByRef(e) => TypeData::ByRef(self.normalize(e)),
            TypeData::Pinned(e) => TypeData::Pinned(self.normalize(e)),
            TypeData::FnPtr(f) => {
                let mut f = *f;
                f.ret = self.normalize(f.ret);
                f.params = f.params.iter().map(|p| self.normalize(*p)).collect();
                TypeData::FnPtr(Box::new(f))
            }
            other => other,
        };
        self.intern(normalized)
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
            TypeData::Inst { generic, args } => TypeData::Inst {
                generic: self.substitute(generic, class, method),
                args: args
                    .iter()
                    .map(|a| self.substitute(*a, class, method))
                    .collect(),
            },
            TypeData::Array { elem, rank } => TypeData::Array {
                elem: self.substitute(elem, class, method),
                rank,
            },
            TypeData::Pointer(e) => TypeData::Pointer(self.substitute(e, class, method)),
            TypeData::ByRef(e) => TypeData::ByRef(self.substitute(e, class, method)),
            TypeData::Pinned(e) => TypeData::Pinned(self.substitute(e, class, method)),
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
