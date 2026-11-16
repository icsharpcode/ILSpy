//! `TypeSystemAstBuilder.ConvertType` as the expression builder configures it
//! (`AlwaysUseShortTypeNames`, `AddResolveResultAnnotations`, `UseNullableSpecifierForValueTypes`
//! from `LiftNullables`): a type as the syntax that names it. Short type names need no name
//! lookup; the using scopes the builder consults have no aliases (`UsingScope.UsingAliases` is
//! empty), so the syntax is a function of the type alone.

use crate::metadata::tables::{TableId, rows};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};

/// An `AstType` node.
#[derive(Clone, Debug, PartialEq, Eq)]
pub enum TypeSyntaxKind {
    /// `PrimitiveType`: a keyword.
    Primitive(String),
    /// `SimpleType` with its type arguments.
    Simple { name: String, args: Vec<TypeSyntax> },
    /// `ComposedType`: the base type with `?`, `*`s, array specifiers (their dimensions, in
    /// order) and `ref`.
    Composed {
        base: Box<TypeSyntax>,
        nullable: bool,
        reference: bool,
        pointer_rank: u32,
        arrays: Vec<u32>,
    },
    /// `TupleAstType`: the elements with their names.
    Tuple(Vec<(TypeSyntax, Option<String>)>),
}

/// A type syntax node with the `TypeResolveResult` annotations `ConvertType` added, in order.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct TypeSyntax {
    pub kind: TypeSyntaxKind,
    pub annotations: Vec<TypeId>,
}

impl TypeSyntax {
    fn new(kind: TypeSyntaxKind) -> Self {
        TypeSyntax {
            kind,
            annotations: Vec::new(),
        }
    }

    fn simple(name: &str, args: Vec<TypeSyntax>) -> Self {
        // `MakeSimpleType`: `_` is the discard, written as a verbatim identifier.
        let name = if name == "_" { "@_" } else { name };
        Self::new(TypeSyntaxKind::Simple {
            name: name.to_owned(),
            args,
        })
    }

    fn composed(self) -> TypeSyntaxKind {
        TypeSyntaxKind::Composed {
            base: Box::new(self),
            nullable: false,
            reference: false,
            pointer_rank: 0,
            arrays: Vec::new(),
        }
    }

    /// `MakeNullableType`: always a new `ComposedType`.
    fn make_nullable(self) -> Self {
        let mut kind = self.composed();
        if let TypeSyntaxKind::Composed { nullable, .. } = &mut kind {
            *nullable = true;
        }
        Self::new(kind)
    }

    /// `MakePointerType`: one more `*` on a `ComposedType` without array specifiers.
    fn make_pointer(mut self) -> Self {
        if let TypeSyntaxKind::Composed {
            pointer_rank,
            arrays,
            ..
        } = &mut self.kind
            && arrays.is_empty()
        {
            *pointer_rank += 1;
            return self;
        }
        let mut kind = self.composed();
        if let TypeSyntaxKind::Composed { pointer_rank, .. } = &mut kind {
            *pointer_rank = 1;
        }
        Self::new(kind)
    }

    /// `MakeArrayType`: the rank before the existing array specifiers of a `ComposedType`.
    fn make_array(mut self, dimensions: u32) -> Self {
        if let TypeSyntaxKind::Composed { arrays, .. } = &mut self.kind {
            arrays.insert(0, dimensions);
            return self;
        }
        let mut kind = self.composed();
        if let TypeSyntaxKind::Composed { arrays, .. } = &mut kind {
            arrays.push(dimensions);
        }
        Self::new(kind)
    }

    /// `MakeRefType`.
    fn make_ref(mut self) -> Self {
        if let TypeSyntaxKind::Composed { reference, .. } = &mut self.kind {
            *reference = true;
            return self;
        }
        let mut kind = self.composed();
        if let TypeSyntaxKind::Composed { reference, .. } = &mut kind {
            *reference = true;
        }
        Self::new(kind)
    }
}

/// `KnownTypeReference.GetCSharpNameByTypeCode`.
fn keyword(code: KnownTypeCode) -> Option<&'static str> {
    Some(match code {
        KnownTypeCode::Object => "object",
        KnownTypeCode::Boolean => "bool",
        KnownTypeCode::Char => "char",
        KnownTypeCode::SByte => "sbyte",
        KnownTypeCode::Byte => "byte",
        KnownTypeCode::Int16 => "short",
        KnownTypeCode::UInt16 => "ushort",
        KnownTypeCode::Int32 => "int",
        KnownTypeCode::UInt32 => "uint",
        KnownTypeCode::Int64 => "long",
        KnownTypeCode::UInt64 => "ulong",
        KnownTypeCode::Single => "float",
        KnownTypeCode::Double => "double",
        KnownTypeCode::Decimal => "decimal",
        KnownTypeCode::String => "string",
        KnownTypeCode::Void => "void",
        _ => return None,
    })
}

/// The builder of one message's type syntax.
pub struct TypeSyntaxBuilder<'t, 'c, 'a> {
    ts: &'t TypeSystem<'c, 'a>,
    /// `UseNullableSpecifierForValueTypes`.
    nullable_specifier: bool,
}

impl<'t, 'c, 'a> TypeSyntaxBuilder<'t, 'c, 'a> {
    pub fn new(ts: &'t TypeSystem<'c, 'a>, nullable_specifier: bool) -> Self {
        TypeSyntaxBuilder {
            ts,
            nullable_specifier,
        }
    }

    /// `ConvertType`: the syntax, annotated with the type; `None` for a function pointer or
    /// intersection type, which the host converts itself (counted).
    pub fn convert_type(&self, t: TypeId) -> Option<TypeSyntax> {
        let mut syntax = self.convert_type_helper(t)?;
        syntax.annotations.push(t);
        Some(syntax)
    }

    fn convert_type_helper(&self, t: TypeId) -> Option<TypeSyntax> {
        let ts = self.ts;
        Some(match ts.data(t).clone() {
            TypeData::Pointer(e) => self.convert_type(e)?.make_pointer(),
            TypeData::Array {
                elem,
                rank,
                nullability,
            } => {
                let array = self.convert_type(elem)?.make_array(rank);
                if nullability == Nullability::Nullable {
                    array.make_nullable()
                } else {
                    array
                }
            }
            TypeData::ByRef(e) => self.convert_type(e)?.make_ref(),
            // Not a type in C#: the element type.
            TypeData::Pinned(e) | TypeData::Modified { inner: e, .. } => self.convert_type(e)?,
            TypeData::Annotated { inner, nullability } => {
                // `ArrayType.ChangeNullability` keeps an array type.
                if let TypeData::Array { elem, rank, .. } = *ts.data(inner) {
                    let array = self.convert_type(elem)?.make_array(rank);
                    return Some(if nullability == Nullability::Nullable {
                        array.make_nullable()
                    } else {
                        array
                    });
                }
                let syntax = self.convert_type(inner)?;
                if nullability == Nullability::Nullable {
                    syntax.make_nullable()
                } else {
                    syntax
                }
            }
            TypeData::Tuple {
                elements, names, ..
            } => {
                let mut items = Vec::with_capacity(elements.len());
                for (&e, n) in elements.iter().zip(names.iter()) {
                    items.push((self.convert_type(e)?, n.as_deref().map(str::to_owned)));
                }
                TypeSyntax::new(TypeSyntaxKind::Tuple(items))
            }
            TypeData::FnPtr(_) | TypeData::Intersection(_) => return None,
            TypeData::Def(_) => {
                let count = ts.type_parameter_count(t) as usize;
                // `IsUnbound`: a generic definition names its type parameters as unbound
                // arguments.
                let args = if count > 0 {
                    vec![ts.find(&TypeData::Unbound)?; count]
                } else {
                    Vec::new()
                };
                self.convert_generic(t, &args)?
            }
            TypeData::Unknown { ref name, .. } => {
                let count = name.type_parameter_count() as usize;
                let args = if count > 0 {
                    vec![ts.find(&TypeData::Unbound)?; count]
                } else {
                    Vec::new()
                };
                self.convert_generic(t, &args)?
            }
            TypeData::Inst { generic, args } => {
                if self.nullable_specifier
                    && ts.is_known(generic, KnownTypeCode::NullableOfT)
                    && let [arg] = *args
                    && !matches!(ts.data(arg), TypeData::UnknownSpecial)
                {
                    return Some(self.convert_type(arg)?.make_nullable());
                }
                // A generic instance carries its nullability on the generic type; a nullable
                // one is the oblivious instance's syntax, annotated with it, made nullable.
                let (definition, nullability) = match *ts.data(generic) {
                    TypeData::Annotated { inner, nullability } => (inner, nullability),
                    _ => (generic, Nullability::Oblivious),
                };
                let syntax = self.convert_generic(definition, &args)?;
                if nullability == Nullability::Nullable {
                    let mut syntax = syntax;
                    syntax.annotations.push(ts.find(&TypeData::Inst {
                        generic: definition,
                        args: args.clone(),
                    })?);
                    return Some(syntax.make_nullable());
                }
                syntax
            }
            TypeData::Dynamic => TypeSyntax::new(TypeSyntaxKind::Primitive("dynamic".to_owned())),
            TypeData::NInt => TypeSyntax::new(TypeSyntaxKind::Primitive("nint".to_owned())),
            TypeData::NUInt => TypeSyntax::new(TypeSyntaxKind::Primitive("nuint".to_owned())),
            TypeData::TypeParam { owner, index } => {
                TypeSyntax::simple(&self.type_parameter_name(owner, index)?, Vec::new())
            }
            TypeData::UnknownSpecial | TypeData::NoType => TypeSyntax::simple("?", Vec::new()),
            TypeData::Unbound => TypeSyntax::simple("", Vec::new()),
            TypeData::ArgList => TypeSyntax::simple("__arglist", Vec::new()),
            TypeData::Null => TypeSyntax::simple("null", Vec::new()),
        })
    }

    /// `ConvertTypeHelper(genericType, typeArguments)` with short type names: the keyword of a
    /// built-in type, otherwise the simple name with the type's own type arguments.
    fn convert_generic(&self, generic: TypeId, args: &[TypeId]) -> Option<TypeSyntax> {
        let ts = self.ts;
        let (name, outer, count) = match ts.data(generic) {
            TypeData::Def(d) => {
                let info = ts.comp.type_def(*d).ok()?;
                if let Some(k) = keyword(info.known) {
                    return Some(TypeSyntax::new(TypeSyntaxKind::Primitive(k.to_owned())));
                }
                let local = info
                    .full_name
                    .nested
                    .last()
                    .map_or(info.full_name.top.tpc, |n| n.1);
                let count = info.full_name.type_parameter_count();
                (info.full_name.name().to_owned(), count - local, count)
            }
            TypeData::Unknown { name, .. } => {
                let local = name.nested.last().map_or(name.top.tpc, |n| n.1);
                let count = name.type_parameter_count();
                (name.name().to_owned(), count - local, count)
            }
            _ => return None,
        };
        let mut converted = Vec::new();
        for &a in args.get(outer as usize..count as usize)? {
            converted.push(self.convert_type(a)?);
        }
        Some(TypeSyntax::simple(&name, converted))
    }

    /// The name of a type parameter: its `GenericParam` row's, or the dummy names.
    fn type_parameter_name(&self, owner: TypeParamOwner, index: u32) -> Option<String> {
        let (module, table, row) = match owner {
            TypeParamOwner::Type(d) => (d.module, TableId::TypeDef, d.row),
            TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => {
                (m.module, TableId::MethodDef, m.row)
            }
            // `DummyTypeParameter`
            TypeParamOwner::DummyType => return Some(format!("`{index}")),
            TypeParamOwner::DummyMethod => return Some(format!("``{index}")),
            TypeParamOwner::Synthetic { id, .. } => {
                return Some(self.ts.synthetic_type_parameter(id).name.clone());
            }
        };
        let md = &self.ts.comp.module(module).md;
        let gp_row = md.generic_params(table, row).nth(index as usize)?;
        let gp = md.get::<rows::GenericParam>(gp_row).ok()?;
        Some(md.string(gp.name).ok()?.into_owned())
    }
}

#[cfg(test)]
mod tests {
    use super::{TypeSyntax, TypeSyntaxKind};

    fn composed(s: &TypeSyntax) -> (bool, bool, u32, Vec<u32>) {
        match &s.kind {
            TypeSyntaxKind::Composed {
                nullable,
                reference,
                pointer_rank,
                arrays,
                ..
            } => (*nullable, *reference, *pointer_rank, arrays.clone()),
            _ => panic!("not a composed type"),
        }
    }

    #[test]
    fn composed_types_merge_like_the_managed_ast() {
        let t = || TypeSyntax::simple("T", Vec::new());
        // `T[,][]`: a further array rank goes before the existing specifiers.
        assert_eq!(
            composed(&t().make_array(1).make_array(2)),
            (false, false, 0, vec![2, 1])
        );
        // `T**`: pointers stack on the same node, but not past an array specifier.
        assert_eq!(
            composed(&t().make_pointer().make_pointer()),
            (false, false, 2, vec![])
        );
        let pointer_to_array = t().make_array(1).make_pointer();
        assert_eq!(composed(&pointer_to_array), (false, false, 1, vec![]));
        // `T?[]` and `ref T?`: a nullable type is a new node, extended by array and ref.
        assert_eq!(
            composed(&t().make_nullable().make_array(1)),
            (true, false, 0, vec![1])
        );
        assert_eq!(
            composed(&t().make_nullable().make_ref()),
            (true, true, 0, vec![])
        );
        // `T[]?`: nullability wraps the array type.
        let nullable_array = t().make_array(1).make_nullable();
        assert_eq!(composed(&nullable_array), (true, false, 0, vec![]));
        // `_` is written as a verbatim identifier.
        assert!(
            matches!(TypeSyntax::simple("_", Vec::new()).kind, TypeSyntaxKind::Simple { ref name, .. } if name == "@_")
        );
    }
}
