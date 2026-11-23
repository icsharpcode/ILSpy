//! `CSharpConversions`: whether, and which, conversion exists between two types or from an
//! expression to a type (C# spec draft-v11, section 10).

use std::collections::HashMap;

use crate::typesystem::compilation::ModuleId;
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{ConstValue, RefKind};
use crate::typesystem::options;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

use super::overload_resolution::OverloadOptions;
use super::types::{TypeKind, Variance};
use super::{Conversion, Method, MethodGroup, ResolveResult};

/// `TypeCode` values (`ReflectionHelper.GetTypeCode`).
mod code {
    pub const EMPTY: u8 = 0;
    pub const CHAR: u8 = 4;
    pub const SBYTE: u8 = 5;
    pub const BYTE: u8 = 6;
    pub const INT16: u8 = 7;
    pub const UINT16: u8 = 8;
    pub const INT32: u8 = 9;
    pub const UINT32: u8 = 10;
    pub const INT64: u8 = 11;
    pub const UINT64: u8 = 12;
    pub const SINGLE: u8 = 13;
    pub const DOUBLE: u8 = 14;
    pub const DECIMAL: u8 = 15;
}

/// The implicit numeric conversions between integral types: rows char, sbyte, byte, short,
/// ushort, int, uint, long, ulong; columns short, ushort, int, uint, long, ulong.
const IMPLICIT_NUMERIC: [[bool; 6]; 9] = [
    [false, true, true, true, true, true],
    [true, false, true, false, true, false],
    [true, true, true, true, true, true],
    [true, false, true, false, true, false],
    [false, true, true, true, true, true],
    [false, false, true, false, true, false],
    [false, false, false, true, true, true],
    [false, false, false, false, true, false],
    [false, false, false, false, false, true],
];

/// `OperatorInfo`: an applicable conversion operator.
struct OperatorInfo {
    method: Method,
    source: TypeId,
    target: TypeId,
    lifted: bool,
}

/// `CSharpConversions` over a type system. Results of `implicit_type` are cached.
pub struct Conversions<'t, 'c, 'a> {
    pub ts: &'t mut TypeSystem<'c, 'a>,
    implicit_cache: HashMap<(TypeId, TypeId), Conversion>,
}

impl<'t, 'c, 'a> Conversions<'t, 'c, 'a> {
    pub fn new(ts: &'t mut TypeSystem<'c, 'a>) -> Self {
        Conversions {
            ts,
            implicit_cache: HashMap::new(),
        }
    }

    /// `compilation.TypeSystemOptions.HasFlag(TypeSystemOptions.FirstClassSpanTypes)`.
    pub fn first_class_span_types(&self) -> bool {
        self.ts.options(ModuleId(0)) & options::FIRST_CLASS_SPAN_TYPES != 0
    }

    fn kind(&self, t: TypeId) -> TypeKind {
        self.ts.kind(t)
    }

    fn is_known(&self, t: TypeId, code: KnownTypeCode) -> bool {
        self.ts.is_known(t, code)
    }

    fn first_arg(&self, t: TypeId) -> TypeId {
        self.ts.first_type_argument(t).unwrap_or(t)
    }

    // ---- ImplicitConversion ----

    fn implicit_rr(
        &mut self,
        rr: &ResolveResult,
        to: TypeId,
        allow_user_defined: bool,
        allow_tuple: bool,
    ) -> Conversion {
        let rr_type = rr.ty(self.ts);
        if rr.is_compile_time_constant() {
            let c = self.implicit_enumeration(rr, to);
            if c.is_valid() {
                return c;
            }
            if self.implicit_constant_expression(Some(rr), to) {
                return Conversion::IMPLICIT_CONSTANT_EXPRESSION;
            }
        }
        // C# 9.0 spec: 10.2.5
        if matches!(rr, ResolveResult::InterpolatedString(_))
            && (self.is_known(to, KnownTypeCode::IFormattable)
                || self.is_known(to, KnownTypeCode::FormattableString))
        {
            return Conversion::IMPLICIT_INTERPOLATED_STRING;
        }
        if self.kind(rr_type) == TypeKind::Dynamic {
            return Conversion::IMPLICIT_DYNAMIC;
        }
        let c = self.anonymous_function_conversion(rr, to);
        if c != Conversion::None {
            return c;
        }
        let c = self.method_group_conversion(rr, to);
        if c != Conversion::None {
            return c;
        }
        // C# 9.0 spec: 10.2.16 default literal conversions
        if matches!(rr, ResolveResult::DefaultLiteral(_)) {
            return Conversion::DEFAULT_LITERAL;
        }
        if rr.is_compile_time_constant() {
            let c = self.standard_implicit_with(rr_type, to, allow_tuple);
            if c != Conversion::None {
                return c;
            }
            if allow_user_defined {
                let c = self.user_defined_implicit(Some(rr), rr_type, to);
                if c != Conversion::None {
                    return c;
                }
            }
            Conversion::None
        } else {
            if allow_tuple && let ResolveResult::Tuple { elements, .. } = rr {
                let c = self.tuple_conversion_rr(elements, to, false);
                if c != Conversion::None {
                    return c;
                }
            }
            // C# 9.0 spec: 10.2.17
            if matches!(rr, ResolveResult::Throw) {
                return Conversion::THROW_EXPRESSION;
            }
            if allow_user_defined && allow_tuple {
                self.implicit_type(rr_type, to)
            } else {
                self.implicit_types(rr_type, to, allow_user_defined, allow_tuple)
            }
        }
    }

    fn implicit_types(
        &mut self,
        from: TypeId,
        to: TypeId,
        allow_user_defined: bool,
        allow_tuple: bool,
    ) -> Conversion {
        // C# spec (draft-v11): 10.2 Implicit conversions
        let c = self.standard_implicit_with(from, to, allow_tuple);
        if c == Conversion::None && allow_user_defined {
            return self.user_defined_implicit(None, from, to);
        }
        c
    }

    /// `ImplicitConversion(ResolveResult, IType)`.
    pub fn implicit(&mut self, rr: &ResolveResult, to: TypeId) -> Conversion {
        self.implicit_rr(rr, to, true, true)
    }

    /// `ImplicitConversion(IType, IType)`.
    pub fn implicit_type(&mut self, from: TypeId, to: TypeId) -> Conversion {
        if let Some(c) = self.implicit_cache.get(&(from, to)) {
            return c.clone();
        }
        let c = self.implicit_types(from, to, true, true);
        self.implicit_cache.insert((from, to), c.clone());
        c
    }

    /// `StandardImplicitConversion`.
    pub fn standard_implicit(&mut self, from: TypeId, to: TypeId) -> Conversion {
        self.standard_implicit_with(from, to, true)
    }

    fn standard_implicit_with(
        &mut self,
        from: TypeId,
        to: TypeId,
        allow_tuple: bool,
    ) -> Conversion {
        // C# 9.0 spec: 10.4.2
        if self.identity(from, to) {
            return Conversion::IDENTITY;
        }
        if self.implicit_numeric(from, to) {
            return Conversion::IMPLICIT_NUMERIC;
        }
        let c = self.implicit_nullable(from, to);
        if c != Conversion::None {
            return c;
        }
        if self.null_literal(from, to) {
            return Conversion::NULL_LITERAL;
        }
        if self.implicit_reference(from, to, 0) {
            return Conversion::IMPLICIT_REFERENCE;
        }
        if self.is_boxing(from, to) {
            return Conversion::BOXING;
        }
        if self.implicit_type_parameter(from, to) {
            // Implicit type parameter conversions that aren't also reference conversions are
            // considered to be boxing conversions.
            return Conversion::BOXING;
        }
        if self.implicit_pointer(from, to) {
            return Conversion::IMPLICIT_POINTER;
        }
        if allow_tuple {
            // Roslyn treats tuple conversions as standard implicit conversions (the spec does
            // not list them), so a tuple conversion can make one type encompass another while
            // resolving a user-defined conversion. Callers classifying a cast pass
            // allow_tuple: false, see `explicit`.
            let c = self.tuple_conversion(from, to, false);
            if c != Conversion::None {
                return c;
            }
        }
        if (self.is_known(to, KnownTypeCode::SpanOfT)
            || self.is_known(to, KnownTypeCode::ReadOnlySpanOfT))
            && self.ts.is_inline_array(from)
        {
            let element = self.ts.inline_array_element_type(from);
            let span_element = self.first_arg(to);
            if self.identity(element, span_element) {
                return Conversion::INLINE_ARRAY;
            }
        }
        if self.is_implicit_span(from, to) {
            return Conversion::IMPLICIT_SPAN;
        }
        Conversion::None
    }

    /// `IsConstraintConvertible`: the conversions allowed when satisfying constraints.
    pub fn is_constraint_convertible(&mut self, from: TypeId, to: TypeId) -> bool {
        if self.identity(from, to) || self.implicit_reference(from, to, 0) {
            return true;
        }
        if self.ts.is_nullable(from) {
            // An 'object' constraint still allows nullable value types (object constraints
            // don't exist in C#, but the type system adds them to the direct base types).
            if self.is_known(to, KnownTypeCode::Object) {
                return true;
            }
        } else if self.is_boxing(from, to) {
            return true;
        }
        self.implicit_type_parameter(from, to)
    }

    // ---- ExplicitConversion ----

    /// `ExplicitConversion(ResolveResult, IType)`.
    pub fn explicit(&mut self, rr: &ResolveResult, to: TypeId) -> Conversion {
        let rr_type = rr.ty(self.ts);
        if self.kind(rr_type) == TypeKind::Dynamic {
            return Conversion::EXPLICIT_DYNAMIC;
        }
        // A cast classifies tuple elements as cast conversions, so the explicit tuple
        // conversion below wins over an implicit one (Roslyn's
        // ClassifyConversionFromTypeForCast discards an ImplicitTuple result).
        let c = self.implicit_rr(rr, to, false, false);
        if c != Conversion::None {
            return c;
        }
        if let ResolveResult::Tuple { elements, .. } = rr {
            let c = self.tuple_conversion_rr(elements, to, true);
            if c != Conversion::None {
                return c;
            }
        }
        let c = self.explicit_impl(rr_type, to);
        if c != Conversion::None {
            return c;
        }
        self.user_defined_explicit(Some(rr), rr_type, to)
    }

    /// `ExplicitConversion(IType, IType)`.
    pub fn explicit_type(&mut self, from: TypeId, to: TypeId) -> Conversion {
        let c = self.implicit_types(from, to, false, false);
        if c != Conversion::None {
            return c;
        }
        let c = self.explicit_impl(from, to);
        if c != Conversion::None {
            return c;
        }
        self.user_defined_explicit(None, from, to)
    }

    fn explicit_not_user_defined(&mut self, from: TypeId, to: TypeId) -> Conversion {
        let c = self.implicit_types(from, to, false, false);
        if c != Conversion::None {
            return c;
        }
        self.explicit_impl(from, to)
    }

    fn explicit_impl(&mut self, from: TypeId, to: TypeId) -> Conversion {
        // Called after the implicit conversions, so any remaining conversions are explicit.
        if self.any_numeric(from, to) {
            return Conversion::EXPLICIT_NUMERIC;
        }
        if self.explicit_enumeration(from, to) {
            return Conversion::enumeration(false, false);
        }
        let c = self.explicit_nullable(from, to);
        if c != Conversion::None {
            return c;
        }
        if self.explicit_reference(from, to) {
            return Conversion::EXPLICIT_REFERENCE;
        }
        if self.is_explicit_span(from, to) {
            return Conversion::EXPLICIT_SPAN;
        }
        if self.unboxing(from, to) {
            return Conversion::UNBOXING;
        }
        let c = self.explicit_type_parameter(from, to);
        if c != Conversion::None {
            return c;
        }
        if self.explicit_pointer(from, to) {
            return Conversion::EXPLICIT_POINTER;
        }
        self.tuple_conversion(from, to, true)
    }

    // ---- Identity ----

    /// `IdentityConversion`: equal after type erasure (`object` is `dynamic`, tuples are their
    /// underlying types, nullability and modifiers are gone).
    pub fn identity(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 10.2.2 Identity conversion
        from == to
            || self.ts.normalize_with(from, Normalization::TYPE_ERASURE)
                == self.ts.normalize_with(to, Normalization::TYPE_ERASURE)
    }

    // ---- Numeric ----

    fn implicit_numeric(&self, from: TypeId, to: TypeId) -> bool {
        // C# 9.0 spec: 10.2.3
        let mut f = self.ts.type_code(from);
        if f == code::EMPTY {
            // A native-sized integer converts as 64 bits.
            match self.kind(from) {
                TypeKind::NInt => f = code::INT64,
                TypeKind::NUInt => f = code::UINT64,
                _ => {}
            }
        }
        let mut t = self.ts.type_code(to);
        if t == code::EMPTY {
            // A native-sized integer can only safely hold 32 bits.
            match self.kind(to) {
                TypeKind::NInt => t = code::INT32,
                TypeKind::NUInt => t = code::UINT32,
                _ => {}
            }
        }
        let integral = (code::CHAR..=code::UINT64).contains(&f);
        if (code::SINGLE..=code::DECIMAL).contains(&t) {
            // Every integral type converts to float/double/decimal, and float to double.
            integral || (f == code::SINGLE && t == code::DOUBLE)
        } else {
            integral
                && (code::INT16..=code::UINT64).contains(&t)
                && IMPLICIT_NUMERIC[(f - code::CHAR) as usize][(t - code::INT16) as usize]
        }
    }

    fn is_numeric_type(&self, t: TypeId) -> bool {
        if matches!(self.kind(t), TypeKind::NInt | TypeKind::NUInt) {
            return true;
        }
        (code::CHAR..=code::DECIMAL).contains(&self.ts.type_code(t))
    }

    fn any_numeric(&self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 10.2.3 + 10.3.2 (numeric conversions)
        self.is_numeric_type(from) && self.is_numeric_type(to)
    }

    // ---- Enumeration ----

    fn implicit_enumeration(&mut self, rr: &ResolveResult, to: TypeId) -> Conversion {
        // C# 9.0 spec: 10.2.4 + the enum part of 10.2.6 (nullable conversions)
        let rr_type = rr.ty(self.ts);
        let c = self.ts.type_code(rr_type);
        if (code::SBYTE..=code::DECIMAL).contains(&c)
            && rr.constant_value().is_some_and(is_zero)
            && self.kind(self.ts.nullable_underlying(to)) == TypeKind::Enum
        {
            return Conversion::enumeration(true, self.ts.is_nullable(to));
        }
        Conversion::None
    }

    fn explicit_enumeration(&self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 10.3.3 Explicit enumeration conversions
        if self.kind(from) == TypeKind::Enum {
            self.kind(to) == TypeKind::Enum || self.is_numeric_type(to)
        } else if self.is_numeric_type(from) {
            self.kind(to) == TypeKind::Enum
        } else {
            false
        }
    }

    // ---- Nullable ----

    fn implicit_nullable(&mut self, from: TypeId, to: TypeId) -> Conversion {
        // C# 9.0 spec: 10.2.6
        if self.ts.is_nullable(to) {
            let t = self.ts.nullable_underlying(to);
            let s = self.ts.nullable_underlying(from);
            if self.identity(s, t) {
                return Conversion::IMPLICIT_NULLABLE;
            }
            if self.implicit_numeric(s, t) {
                return Conversion::IMPLICIT_LIFTED_NUMERIC;
            }
        }
        Conversion::None
    }

    fn explicit_nullable(&mut self, from: TypeId, to: TypeId) -> Conversion {
        // C# spec (draft-v11): 10.3.4 Explicit nullable conversions
        if self.ts.is_nullable(to) || self.ts.is_nullable(from) {
            let t = self.ts.nullable_underlying(to);
            let s = self.ts.nullable_underlying(from);
            if self.identity(s, t) {
                return Conversion::EXPLICIT_NULLABLE;
            }
            if self.any_numeric(s, t) {
                return Conversion::EXPLICIT_LIFTED_NUMERIC;
            }
            if self.explicit_enumeration(s, t) {
                return Conversion::enumeration(false, true);
            }
        }
        Conversion::None
    }

    // ---- Null literal ----

    fn null_literal(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# 9.0 spec: 10.2.7
        self.kind(from) == TypeKind::Null
            && (self.ts.is_nullable(to) || self.ts.is_reference_type(to) == Some(true))
    }

    // ---- Implicit reference ----

    /// `IsImplicitReferenceConversion`.
    pub fn is_implicit_reference(&mut self, from: TypeId, to: TypeId) -> bool {
        self.implicit_reference(from, to, 0)
    }

    fn implicit_reference(&mut self, from: TypeId, to: TypeId, depth: u32) -> bool {
        // C# 9.0 spec: 10.2.8
        // Reference conversions are possible if both types are known to be reference types,
        // or if both are type parameters and `from` has a class constraint.
        if !(self.ts.is_reference_type(from) == Some(true)
            && self.ts.is_reference_type(to) != Some(false))
        {
            return false;
        }
        if let TypeData::Array {
            elem: from_elem,
            rank: from_rank,
            ..
        } = *self.ts.data(from)
        {
            if let TypeData::Array {
                elem: to_elem,
                rank: to_rank,
                ..
            } = *self.ts.data(to)
            {
                // array covariance (the broken kind)
                return from_rank == to_rank && self.implicit_reference(from_elem, to_elem, depth);
            }
            // A single-dimensional array S[] converts to IList<T>, IReadOnlyList<T> and their
            // base interfaces; array covariance applies (string[] is IList<object>).
            if from_rank == 1
                && let Some(to_arg) = self.unpack_generic_array_interface(to)
            {
                return self.identity(from_elem, to_arg)
                    || self.implicit_reference(from_elem, to_arg, depth);
            }
            // Any array converts to System.Array and the interfaces it implements.
            let system_array = self.ts.known_type(KnownTypeCode::Array);
            return self.implicit_reference(system_array, to, depth);
        }
        // Traverse the inheritance chain, with generics and variance.
        self.is_subtype_of(from, to, depth)
    }

    /// For `IList<T>`, `ICollection<T>`, `IEnumerable<T>`, `IReadOnlyList<T>` and
    /// `IReadOnlyCollection<T>`, T.
    fn unpack_generic_array_interface(&self, t: TypeId) -> Option<TypeId> {
        let TypeData::Inst { args, .. } = self.ts.data(t) else {
            return None;
        };
        match self.ts.known_type_code(t) {
            KnownTypeCode::IListOfT
            | KnownTypeCode::ICollectionOfT
            | KnownTypeCode::IEnumerableOfT
            | KnownTypeCode::IReadOnlyListOfT
            | KnownTypeCode::IReadOnlyCollectionOfT => args.first().copied(),
            _ => None,
        }
    }

    /// Whether `s` is a subtype of `t` (reference, boxing and type parameter conversions).
    fn is_subtype_of(&mut self, s: TypeId, t: TypeId, depth: u32) -> bool {
        // Conversions to dynamic and object always exist.
        if self.kind(t) == TypeKind::Dynamic || self.is_known(t, KnownTypeCode::Object) {
            return true;
        }
        if depth > 10 {
            // Subtyping in C# is undecidable ("On Decidability of Nominal Subtyping with
            // Variance", Kennedy and Pierce), so the nesting depth of variance conversions is
            // limited; real code does not nest generics with variance that deep.
            return false;
        }
        let bases = self.ts.all_base_types(s).unwrap_or_default();
        bases
            .into_iter()
            .any(|base| self.identity_or_variance(base, t, depth + 1))
    }

    fn identity_or_variance(&mut self, s: TypeId, t: TypeId, depth: u32) -> bool {
        let Some(def) = self.ts.definition(s) else {
            // Not type definitions; the types may still be equal (e.g. type parameters).
            return s == t;
        };
        if Some(def) != self.ts.definition(t) {
            return false;
        }
        match (self.ts.data(s).clone(), self.ts.data(t).clone()) {
            (TypeData::Inst { args: ps, .. }, TypeData::Inst { args: pt, .. }) => {
                // C# spec (draft-v11): 19.2.3.3 Variance conversion
                let count = self
                    .ts
                    .comp
                    .type_def(def)
                    .map_or(0, |i| i.type_parameter_count);
                for i in 0..count as usize {
                    let (si, ti) = (ps[i], pt[i]);
                    if self.identity(si, ti) {
                        continue;
                    }
                    let ok = match self.ts.type_parameter_variance(def, i) {
                        Variance::Covariant => self.implicit_reference(si, ti, depth),
                        Variance::Contravariant => self.implicit_reference(ti, si, depth),
                        Variance::Invariant => false,
                    };
                    if !ok {
                        return false;
                    }
                }
                true
            }
            // Only one of them is parameterized.
            (TypeData::Inst { .. }, _) | (_, TypeData::Inst { .. }) => false,
            _ => true,
        }
    }

    // ---- Explicit reference ----

    fn explicit_reference(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 10.3.5 Explicit reference conversions
        if self.ts.is_reference_type(to) != Some(true) {
            return false;
        }
        if self.ts.is_reference_type(from) != Some(true) {
            // From F to T is a reference conversion where T : class, F (F must be a reference
            // type as well, even though C# doesn't treat it as one).
            if self.kind(from) == TypeKind::TypeParameter {
                return self.is_subtype_of(to, from, 0);
            }
            return false;
        }
        let from_array = match *self.ts.data(from) {
            TypeData::Array { elem, rank, .. } => Some((elem, rank)),
            _ => None,
        };
        if let TypeData::Array {
            elem: to_elem,
            rank: to_rank,
            ..
        } = *self.ts.data(to)
        {
            if let Some((from_elem, from_rank)) = from_array {
                // array covariance
                return from_rank == to_rank && self.explicit_reference(from_elem, to_elem);
            }
            if let Some(from_arg) = self.unpack_generic_array_interface(from)
                && to_rank == 1
            {
                return self.explicit_reference(from_arg, to_elem)
                    || self.identity(from_arg, to_elem);
            }
            // Otherwise the array is like a sealed class: the opposite direction must be
            // implicit.
            return self.is_implicit_reference(to, from);
        }
        if let Some((from_elem, from_rank)) = from_array {
            if let Some(to_arg) = self.unpack_generic_array_interface(to)
                && from_rank == 1
            {
                return self.explicit_reference(from_elem, to_arg);
            }
            // Otherwise the array is like a sealed class.
            return self.is_implicit_reference(from, to);
        }
        if self.kind(from) == TypeKind::Delegate && self.kind(to) == TypeKind::Delegate {
            let def = self.ts.definition(from);
            if def.is_none() || def != self.ts.definition(to) {
                return false;
            }
            let def = def.expect("a definition");
            let (ps, pt) = match (self.ts.data(from).clone(), self.ts.data(to).clone()) {
                (TypeData::Inst { args: ps, .. }, TypeData::Inst { args: pt, .. }) => (ps, pt),
                (TypeData::Inst { .. }, _) | (_, TypeData::Inst { .. }) => return false,
                // a non-generic delegate: the identity conversion
                _ => return true,
            };
            let count = self
                .ts
                .comp
                .type_def(def)
                .map_or(0, |i| i.type_parameter_count);
            for i in 0..count as usize {
                let (si, ti) = (ps[i], pt[i]);
                if self.identity(si, ti) {
                    continue;
                }
                let ok = match self.ts.type_parameter_variance(def, i) {
                    Variance::Covariant => self.explicit_reference(si, ti),
                    Variance::Contravariant => {
                        self.ts.is_reference_type(si) == Some(true)
                            && self.ts.is_reference_type(ti) == Some(true)
                    }
                    Variance::Invariant => false,
                };
                if !ok {
                    return false;
                }
            }
            return true;
        }
        if self.is_sealed_reference_type(from) {
            // From a sealed type, explicit conversions can do no more than implicit ones.
            return self.is_implicit_reference(from, to);
        }
        if self.is_sealed_reference_type(to) {
            // To a sealed type, the opposite direction must be implicit.
            return self.is_implicit_reference(to, from);
        }
        if self.kind(from) == TypeKind::Interface || self.kind(to) == TypeKind::Interface {
            return true;
        }
        self.is_implicit_reference(to, from) || self.is_implicit_reference(from, to)
    }

    fn is_sealed_reference_type(&self, t: TypeId) -> bool {
        match self.kind(t) {
            TypeKind::Class => self
                .ts
                .definition(t)
                .and_then(|d| self.ts.comp.type_def(d).ok())
                // TypeAttributes.Sealed
                .is_some_and(|i| i.flags & 0x100 != 0),
            TypeKind::Delegate => true,
            _ => false,
        }
    }

    // ---- Boxing ----

    fn is_boxing(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# 9.0 spec: 10.2.9
        let from = self.ts.nullable_underlying(from);
        if self.ts.is_reference_type(from) == Some(false)
            && !self.ts.is_by_ref_like(from)
            && self.ts.is_reference_type(to) == Some(true)
        {
            self.is_subtype_of(from, to, 0)
        } else {
            false
        }
    }

    /// `IsBoxingConversionOrInvolvingTypeParameter`: a boxing conversion, or an implicit
    /// conversion of a type parameter that boxes when it is instantiated with a value type.
    pub fn is_boxing_or_involving_type_parameter(&mut self, from: TypeId, to: TypeId) -> bool {
        self.is_boxing(from, to) || self.implicit_type_parameter(from, to)
    }

    fn unboxing(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 10.3.7 Unboxing conversions
        let to = self.ts.nullable_underlying(to);
        if self.ts.is_reference_type(from) == Some(true)
            && self.ts.is_reference_type(to) == Some(false)
        {
            self.is_subtype_of(to, from, 0)
        } else {
            false
        }
    }

    // ---- Implicit constant expression ----

    fn implicit_constant_expression(&mut self, rr: Option<&ResolveResult>, to: TypeId) -> bool {
        let Some(rr) = rr.filter(|rr| rr.is_compile_time_constant()) else {
            return false;
        };
        // C# 9.0 spec: 10.2.11 + part of 10.2.6 (nullable conversions)
        let rr_type = rr.ty(self.ts);
        let from_code = self.ts.type_code(rr_type);
        let to = self.ts.nullable_underlying(to);
        let mut to_code = self.ts.type_code(to);
        if self.kind(to) == TypeKind::NUInt {
            to_code = code::UINT32;
        }
        match (from_code, rr.constant_value()) {
            (code::INT64, Some(ConstValue::Int64(v))) => *v >= 0 && to_code == code::UINT64,
            (code::INT32, Some(ConstValue::Int32(v))) => {
                let v = i64::from(*v);
                match to_code {
                    code::SBYTE => (i64::from(i8::MIN)..=i64::from(i8::MAX)).contains(&v),
                    code::BYTE => (0..=i64::from(u8::MAX)).contains(&v),
                    code::INT16 => (i64::from(i16::MIN)..=i64::from(i16::MAX)).contains(&v),
                    code::UINT16 => (0..=i64::from(u16::MAX)).contains(&v),
                    code::UINT32 | code::UINT64 => v >= 0,
                    _ => false,
                }
            }
            _ => false,
        }
    }

    // ---- Type parameters ----

    fn implicit_type_parameter(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# 9.0 spec: 10.2.12
        if self.kind(from) != TypeKind::TypeParameter {
            return false;
        }
        if self.ts.is_reference_type(from).is_some() {
            // handled by the implicit reference and boxing conversions
            return false;
        }
        self.is_subtype_of(from, to, 0)
    }

    fn explicit_type_parameter(&mut self, from: TypeId, to: TypeId) -> Conversion {
        if self.kind(to) == TypeKind::TypeParameter {
            // Explicit type parameter conversions that aren't also reference conversions are
            // considered to be unboxing conversions.
            if self.kind(from) == TypeKind::Interface || self.is_subtype_of(to, from, 0) {
                return Conversion::UNBOXING;
            }
        } else if self.kind(from) == TypeKind::TypeParameter && self.kind(to) == TypeKind::Interface
        {
            return Conversion::BOXING;
        }
        Conversion::None
    }

    // ---- Pointers ----

    fn implicit_pointer(&mut self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 24.5 Pointer conversions
        if self.kind(from).is_any_pointer()
            && let TypeData::Pointer(elem) = *self.ts.data(to)
            && self.is_known(elem, KnownTypeCode::Void)
            && matches!(self.ts.data(elem), TypeData::Def(_))
        {
            return true;
        }
        if self.kind(from) == TypeKind::Null && self.kind(to).is_any_pointer() {
            return true;
        }
        if let (TypeData::FnPtr(f), TypeData::FnPtr(t)) =
            (self.ts.data(from).clone(), self.ts.data(to).clone())
            && f.calling_convention == t.calling_convention
            && f.params.len() == t.params.len()
        {
            // Variance applies to function pointer types.
            if !(self.identity(f.ret, t.ret) || self.implicit_reference(f.ret, t.ret, 0)) {
                return false;
            }
            for (&fp, &tp) in f.params.iter().zip(t.params.iter()) {
                if !(self.identity(tp, fp) || self.implicit_reference(tp, fp, 0)) {
                    return false;
                }
            }
            return true;
        }
        false
    }

    fn explicit_pointer(&self, from: TypeId, to: TypeId) -> bool {
        // C# spec (draft-v11): 24.5 Pointer conversions
        if self.kind(from).is_any_pointer() {
            self.kind(to).is_any_pointer() || self.is_integer_type(to)
        } else {
            self.kind(to).is_any_pointer() && self.is_integer_type(from)
        }
    }

    fn is_integer_type(&self, t: TypeId) -> bool {
        if matches!(self.kind(t), TypeKind::NInt | TypeKind::NUInt) {
            return true;
        }
        (code::SBYTE..=code::UINT64).contains(&self.ts.type_code(t))
    }

    // ---- User-defined ----

    /// Whether `a` is encompassed by `b`.
    fn is_encompassed_by(&mut self, a: TypeId, b: TypeId) -> bool {
        self.standard_implicit(a, b).is_valid()
    }

    fn is_encompassing_or_encompassed_by(&mut self, a: TypeId, b: TypeId) -> bool {
        self.standard_implicit(a, b).is_valid() || self.standard_implicit(b, a).is_valid()
    }

    fn find_most_encompassed_type(&mut self, candidates: &[TypeId]) -> Option<TypeId> {
        let mut best: Option<TypeId> = None;
        for &current in candidates {
            match best {
                None => best = Some(current),
                Some(b) if self.is_encompassed_by(current, b) => best = Some(current),
                Some(b) if !self.is_encompassed_by(b, current) => return None, // ambiguous
                _ => {}
            }
        }
        best
    }

    fn find_most_encompassing_type(&mut self, candidates: &[TypeId]) -> Option<TypeId> {
        let mut best: Option<TypeId> = None;
        for &current in candidates {
            match best {
                None => best = Some(current),
                Some(b) if self.is_encompassed_by(b, current) => best = Some(current),
                Some(b) if !self.is_encompassed_by(current, b) => return None, // ambiguous
                _ => {}
            }
        }
        best
    }

    #[allow(clippy::too_many_arguments)]
    fn select_operator(
        &mut self,
        most_specific_source: TypeId,
        most_specific_target: TypeId,
        operators: &[OperatorInfo],
        implicit: bool,
        source: TypeId,
        target: TypeId,
    ) -> Conversion {
        let selected: Vec<&OperatorInfo> = operators
            .iter()
            .filter(|op| op.source == most_specific_source && op.target == most_specific_target)
            .collect();
        if selected.is_empty() {
            return Conversion::None;
        }
        let non_lifted: Vec<&&OperatorInfo> = selected.iter().filter(|s| !s.lifted).collect();
        let (op, ambiguous) = if selected.len() == 1 {
            (selected[0], false)
        } else if non_lifted.len() == 1 {
            (*non_lifted[0], false)
        } else {
            (selected[0], true)
        };
        let before = self.explicit_not_user_defined(source, most_specific_source);
        let after = self.explicit_not_user_defined(most_specific_target, target);
        Conversion::user_defined(
            op.method.clone(),
            implicit,
            before,
            after,
            op.lifted,
            ambiguous,
        )
    }

    fn ambiguous_user_defined(op: &OperatorInfo, implicit: bool) -> Conversion {
        Conversion::user_defined(
            op.method.clone(),
            implicit,
            Conversion::None,
            Conversion::None,
            op.lifted,
            true,
        )
    }

    fn user_defined_implicit(
        &mut self,
        from_rr: Option<&ResolveResult>,
        from: TypeId,
        to: TypeId,
    ) -> Conversion {
        // C# spec (draft-v11): 10.5.4 User-defined implicit conversions
        // User-defined conversions are not supported with interfaces.
        if self.kind(from) == TypeKind::Interface || self.kind(to) == TypeKind::Interface {
            return Conversion::None;
        }
        // C# 14: no user-defined conversions between types with an implicit or explicit span
        // conversion; string[] must not reach Span<object> through op_Implicit(object[]) and
        // array covariance.
        if self.is_implicit_span(from, to) || self.is_explicit_span(from, to) {
            return Conversion::None;
        }
        let operators = self.applicable_conversion_operators(from_rr, from, to, false);
        if operators.is_empty() {
            return Conversion::None;
        }
        let most_specific_source = if operators.iter().any(|op| op.source == from) {
            Some(from)
        } else {
            let sources: Vec<TypeId> = operators.iter().map(|op| op.source).collect();
            self.find_most_encompassed_type(&sources)
        };
        let Some(most_specific_source) = most_specific_source else {
            return Self::ambiguous_user_defined(&operators[0], true);
        };
        let most_specific_target = if operators.iter().any(|op| op.target == to) {
            Some(to)
        } else {
            let targets: Vec<TypeId> = operators.iter().map(|op| op.target).collect();
            self.find_most_encompassing_type(&targets)
        };
        let Some(most_specific_target) = most_specific_target else {
            if self.ts.is_nullable(to) {
                let to = self.ts.nullable_underlying(to);
                return self.user_defined_implicit(from_rr, from, to);
            }
            return Self::ambiguous_user_defined(&operators[0], true);
        };
        let selected = self.select_operator(
            most_specific_source,
            most_specific_target,
            &operators,
            true,
            from,
            to,
        );
        if selected != Conversion::None {
            if selected.is_lifted() && self.ts.is_nullable(to) {
                // Prefer A -> B -> B? over A -> A? -> B?
                let underlying = self.ts.nullable_underlying(to);
                let other = self.user_defined_implicit(from_rr, from, underlying);
                if other != Conversion::None {
                    return other;
                }
            }
            selected
        } else if self.ts.is_nullable(to) {
            let to = self.ts.nullable_underlying(to);
            self.user_defined_implicit(from_rr, from, to)
        } else {
            Conversion::None
        }
    }

    fn user_defined_explicit(
        &mut self,
        from_rr: Option<&ResolveResult>,
        from: TypeId,
        to: TypeId,
    ) -> Conversion {
        // C# spec (draft-v11): 10.5.5 User-defined explicit conversions
        // User-defined conversions are not supported with interfaces.
        if self.kind(from) == TypeKind::Interface || self.kind(to) == TypeKind::Interface {
            return Conversion::None;
        }
        // C# 14: no user-defined conversions between types with a span conversion.
        if self.is_implicit_span(from, to) || self.is_explicit_span(from, to) {
            return Conversion::None;
        }
        let operators = self.applicable_conversion_operators(from_rr, from, to, true);
        if operators.is_empty() {
            return Conversion::None;
        }
        let most_specific_source = if operators.iter().any(|op| op.source == from) {
            Some(from)
        } else {
            let mut encompassing = Vec::new();
            for op in &operators {
                let underlying = self.ts.nullable_underlying(op.source);
                if self.is_encompassed_by(from, op.source)
                    || self.implicit_constant_expression(from_rr, underlying)
                {
                    encompassing.push(op.source);
                }
            }
            if !encompassing.is_empty() {
                self.find_most_encompassed_type(&encompassing)
            } else {
                let sources: Vec<TypeId> = operators.iter().map(|op| op.source).collect();
                self.find_most_encompassing_type(&sources)
            }
        };
        let Some(most_specific_source) = most_specific_source else {
            return Self::ambiguous_user_defined(&operators[0], false);
        };
        let most_specific_target = if operators.iter().any(|op| op.target == to) {
            Some(to)
        } else {
            let mut encompassed = Vec::new();
            for op in &operators {
                if self.is_encompassed_by(op.target, to) {
                    encompassed.push(op.target);
                }
            }
            if !encompassed.is_empty() {
                self.find_most_encompassing_type(&encompassed)
            } else {
                let targets: Vec<TypeId> = operators.iter().map(|op| op.target).collect();
                self.find_most_encompassed_type(&targets)
            }
        };
        let Some(most_specific_target) = most_specific_target else {
            if self.ts.is_nullable(to) {
                let to = self.ts.nullable_underlying(to);
                return self.user_defined_explicit(from_rr, from, to);
            }
            return Self::ambiguous_user_defined(&operators[0], false);
        };
        let selected = self.select_operator(
            most_specific_source,
            most_specific_target,
            &operators,
            false,
            from,
            to,
        );
        if selected != Conversion::None {
            if selected.is_lifted() && self.ts.is_nullable(to) {
                // Prefer A -> B -> B? over A -> A? -> B?
                let underlying = self.ts.nullable_underlying(to);
                let other = self.user_defined_implicit(from_rr, from, underlying);
                if other != Conversion::None {
                    return other;
                }
            }
            selected
        } else if self.ts.is_nullable(to) {
            let to = self.ts.nullable_underlying(to);
            self.user_defined_explicit(from_rr, from, to)
        } else if self.ts.is_nullable(from) {
            // A? -> A -> B
            let from = self.ts.nullable_underlying(from);
            self.user_defined_explicit(None, from, to)
        } else {
            Conversion::None
        }
    }

    /// `UnderlyingTypeForConversion`: the element of a by-reference type, then the nullable's
    /// underlying type.
    fn underlying_type_for_conversion(&self, t: TypeId) -> TypeId {
        let t = match self.ts.data(t) {
            TypeData::ByRef(e) => *e,
            _ => t,
        };
        self.ts.nullable_underlying(t)
    }

    /// The static one-parameter `op_Implicit` (and for explicit conversions `op_Explicit`)
    /// operators of both types, without duplicates.
    fn conversion_operators(&mut self, from: TypeId, to: TypeId, explicit: bool) -> Vec<Method> {
        let mut result: Vec<Method> = Vec::new();
        for t in [
            self.underlying_type_for_conversion(from),
            self.underlying_type_for_conversion(to),
        ] {
            let methods = self.ts.conversion_operator_methods(t, explicit);
            for m in methods {
                if !(m.name == "op_Implicit" || (explicit && m.name == "op_Explicit"))
                    || !m.is_static
                {
                    continue;
                }
                let Some(m) = Method::from_resolved(self.ts, &m) else {
                    continue;
                };
                if m.is_operator && m.parameters.len() == 1 && !result.contains(&m) {
                    result.push(m);
                }
            }
        }
        result
    }

    fn applicable_conversion_operators(
        &mut self,
        from_rr: Option<&ResolveResult>,
        from: TypeId,
        to: TypeId,
        explicit: bool,
    ) -> Vec<OperatorInfo> {
        let operators = self.conversion_operators(from, to, explicit);
        let mut result = Vec::new();
        for op in operators {
            let param = &op.parameters[0];
            let mut source = param.ty;
            if let TypeData::ByRef(e) = *self.ts.data(source)
                && param.ref_kind == RefKind::In
                && self.kind(from) != TypeKind::ByReference
            {
                source = e;
            }
            let target = op.return_type;
            // whether the operator is applicable
            let applicable = if explicit {
                (self.is_encompassing_or_encompassed_by(from, source)
                    || self.implicit_constant_expression(from_rr, source))
                    && self.is_encompassing_or_encompassed_by(target, to)
            } else {
                (self.is_encompassed_by(from, source)
                    || self.implicit_constant_expression(from_rr, source))
                    && self.is_encompassed_by(target, to)
            };
            if applicable {
                result.push(OperatorInfo {
                    method: op.clone(),
                    source,
                    target,
                    lifted: false,
                });
            }
            // whether it is applicable in lifted form; for explicit conversions an operator
            // can be applicable in both forms
            if self.ts.is_non_nullable_value_type(source) {
                let lifted_source = self.ts.make_nullable(source);
                let lifted_target = if self.ts.is_non_nullable_value_type(target) {
                    self.ts.make_nullable(target)
                } else {
                    target
                };
                let applicable = if explicit {
                    self.is_encompassing_or_encompassed_by(from, lifted_source)
                        && self.is_encompassing_or_encompassed_by(lifted_target, to)
                } else {
                    self.is_encompassed_by(from, lifted_source)
                        && self.is_encompassed_by(lifted_target, to)
                };
                if applicable {
                    result.push(OperatorInfo {
                        method: op,
                        source: lifted_source,
                        target: lifted_target,
                        lifted: true,
                    });
                }
            }
        }
        result
    }

    // ---- Span conversions ----

    /// The C# 14 implicit span conversions between arrays, `Span<T>`, `ReadOnlySpan<T>` and
    /// `string` (first-class span types proposal).
    fn is_implicit_span(&mut self, from: TypeId, to: TypeId) -> bool {
        if !self.first_class_span_types() {
            return false;
        }
        match *self.ts.data(from) {
            TypeData::Array { elem, rank: 1, .. } => {
                if self.is_known(to, KnownTypeCode::SpanOfT) {
                    let to_elem = self.first_arg(to);
                    return self.identity(elem, to_elem);
                }
                if self.is_known(to, KnownTypeCode::ReadOnlySpanOfT) {
                    let to_elem = self.first_arg(to);
                    return self.identity(elem, to_elem)
                        || self.is_implicit_reference(elem, to_elem);
                }
                false
            }
            TypeData::Inst { ref args, .. }
                if self.is_known(from, KnownTypeCode::SpanOfT)
                    || self.is_known(from, KnownTypeCode::ReadOnlySpanOfT) =>
            {
                let from_elem = args[0];
                if self.is_known(to, KnownTypeCode::ReadOnlySpanOfT) {
                    let to_elem = self.first_arg(to);
                    return self.identity(from_elem, to_elem)
                        || self.is_implicit_reference(from_elem, to_elem);
                }
                false
            }
            _ if self.is_known(from, KnownTypeCode::String) => {
                self.is_known(to, KnownTypeCode::ReadOnlySpanOfT)
                    && self.is_known(self.first_arg(to), KnownTypeCode::Char)
            }
            _ => false,
        }
    }

    /// The C# 14 explicit span conversion: a single-dimensional array to `Span<U>` or
    /// `ReadOnlySpan<U>` whose element types are related by an explicit reference conversion
    /// (the explicit conversions include the implicit ones, so `string[]` to `Span<object>`
    /// lands here too).
    fn is_explicit_span(&mut self, from: TypeId, to: TypeId) -> bool {
        if !self.first_class_span_types() {
            return false;
        }
        if let TypeData::Array { elem, rank: 1, .. } = *self.ts.data(from)
            && (self.is_known(to, KnownTypeCode::SpanOfT)
                || self.is_known(to, KnownTypeCode::ReadOnlySpanOfT))
        {
            let span_elem = self.first_arg(to);
            return self.identity(elem, span_elem)
                || self.is_implicit_reference(elem, span_elem)
                || self.explicit_reference(elem, span_elem);
        }
        false
    }

    // ---- Anonymous functions ----

    fn anonymous_function_conversion(&mut self, rr: &ResolveResult, to: TypeId) -> Conversion {
        // C# 9.0 spec 10.7 Anonymous function conversions
        let ResolveResult::Lambda(f) = rr else {
            return Conversion::None;
        };
        // A lambda (unlike an anonymous method) converts to expression trees, even when that
        // conversion is a compile-time error (statement lambdas).
        let to = if f.is_anonymous_method() {
            to
        } else {
            self.unpack_expression_tree_type(to)
        };
        let Some(d) = self.ts.delegate_invoke(to) else {
            return Conversion::None;
        };
        let d_param_types: Vec<TypeId> = d.parameters.iter().map(|p| p.ty).collect();
        if f.has_parameter_list() {
            // With an anonymous-function-signature, D and F have the same number of
            // parameters.
            if d.parameters.len() != f.parameters().len() {
                return Conversion::None;
            }
            if f.is_implicitly_typed() {
                // With an implicitly typed parameter list, D has no ref or out parameters.
                if d.parameters.iter().any(|p| p.ref_kind != RefKind::None) {
                    return Conversion::None;
                }
            } else {
                // With an explicitly typed parameter list, each parameter of D has the type
                // and modifiers of the corresponding parameter of F.
                for (i, pf) in f.parameters().iter().enumerate() {
                    if d.parameters[i].ref_kind != pf.ref_kind
                        || !self.identity(d_param_types[i], pf.ty)
                    {
                        return Conversion::None;
                    }
                }
            }
        } else if d.parameters.iter().any(|p| p.ref_kind == RefKind::Out) {
            // Without an anonymous-function-signature, D may have any parameters but no out
            // parameters.
            return Conversion::None;
        }
        let f = f.clone();
        f.is_valid(self, &d_param_types, d.return_type)
    }

    /// `Expression<T>` (System.Linq.Expressions) gives T, other types themselves.
    pub fn unpack_expression_tree_type(&self, t: TypeId) -> TypeId {
        if let TypeData::Inst { generic, args } = self.ts.data(t)
            && args.len() == 1
            && let Some(def) = self.ts.definition(*generic)
            && let Ok(info) = self.ts.comp.type_def(def)
            && info.full_name.top.ns == "System.Linq.Expressions"
            && info.full_name.name() == "Expression"
        {
            return args[0];
        }
        t
    }

    // ---- Method groups ----

    fn method_group_conversion(&mut self, rr: &ResolveResult, to: TypeId) -> Conversion {
        // C# 9.0 spec 10.8 Method group conversions
        let ResolveResult::MethodGroup(group) = rr else {
            return Conversion::None;
        };
        let Some(invoke) = self.ts.delegate_invoke(to) else {
            return Conversion::None;
        };
        let object = self.ts.known_type(KnownTypeCode::Object);
        let args: Vec<ResolveResult> = invoke
            .parameters
            .iter()
            .map(|param| {
                if param.ref_kind != RefKind::None
                    && let TypeData::ByRef(e) = *self.ts.data(param.ty)
                {
                    ResolveResult::by_reference(self.ts, e, param.ref_kind)
                } else if self.kind(param.ty) == TypeKind::Dynamic {
                    ResolveResult::Expression(object)
                } else {
                    ResolveResult::Expression(param.ty)
                }
            })
            .collect();
        let or = self.perform_overload_resolution(
            group,
            args,
            OverloadOptions {
                allow_expanding_params: false,
                allow_optional_parameters: false,
                allow_implicit_in: false,
                // C# 14 first-class spans: no span conversion in overload resolution for a
                // method group conversion.
                allow_span_conversion_on_extension_receiver: false,
                ..OverloadOptions::default()
            },
        );
        if !or.found_applicable_candidate() {
            return Conversion::None;
        }
        let method = or
            .best_candidate_with_substituted_type_arguments(self.ts)
            .expect("a best candidate");
        let non_virtual = matches!(
            group.target,
            ResolveResult::This {
                causes_non_virtual_invocation: true,
                ..
            }
        );
        let is_virtual = method.is_overridable && !non_virtual;
        let valid = !or.is_ambiguous()
            && self.is_delegate_compatible_with(
                &method,
                &invoke,
                or.is_extension_method_invocation,
            );
        let captures_first = or.is_extension_method_invocation || !method.is_static;
        Conversion::method_group(method, is_virtual, captures_first, valid)
    }

    /// `MethodGroupResolveResult.PerformOverloadResolution`: overload resolution over the
    /// group's methods, then, when none is applicable, over its extension methods with the
    /// target as first argument.
    pub fn perform_overload_resolution(
        &mut self,
        group: &MethodGroup,
        arguments: Vec<ResolveResult>,
        options: OverloadOptions,
    ) -> super::OverloadResolution {
        let type_arguments = group.type_arguments.clone();
        let mut or =
            super::OverloadResolution::new(arguments.clone(), None, type_arguments.clone());
        or.set_options(options);
        or.add_method_lists(self, &group.method_lists);
        if options.allow_extension_methods
            && !or.found_applicable_candidate()
            && !group.extension_methods.is_empty()
        {
            // No applicable match, so try the extension methods.
            let target_type = group.target.ty(self.ts);
            let mut ext_arguments = vec![ResolveResult::Expression(target_type)];
            ext_arguments.extend(arguments);
            let mut ext = super::OverloadResolution::new(ext_arguments, None, type_arguments);
            ext.set_options(options);
            ext.is_extension_method_invocation = true;
            for g in &group.extension_methods {
                for m in g {
                    ext.add_candidate(self, m.clone());
                }
                if ext.found_applicable_candidate() {
                    break;
                }
            }
            // Without a better comparison (the one in overload resolution depends on the
            // argument set), an extension method result wins only when it is applicable or
            // there is no normal result.
            if ext.found_applicable_candidate() || or.best_candidate().is_none() {
                or = ext;
            }
        }
        or
    }

    /// `IsDelegateCompatible(method, delegateType)` (C# spec draft-v11: 21.4).
    pub fn is_delegate_compatible(&mut self, method: &Method, delegate_type: TypeId) -> bool {
        match self.ts.delegate_invoke(delegate_type) {
            Some(invoke) => self.is_delegate_compatible_with(method, &invoke, false),
            None => false,
        }
    }

    /// Whether `m` is compatible with the delegate whose invoke method is `d`; with
    /// extension method syntax the first parameter of `m` is the receiver.
    fn is_delegate_compatible_with(&mut self, m: &Method, d: &Method, extension: bool) -> bool {
        // C# spec (draft-v11): 21.4 Delegate compatibility
        let first = usize::from(extension);
        if m.parameters.len() < first || m.parameters.len() - first != d.parameters.len() {
            return false;
        }
        for (pm, pd) in m.parameters[first..].iter().zip(&d.parameters) {
            // ref/out/in must match
            if pm.ref_kind != pd.ref_kind {
                return false;
            }
            if pm.ref_kind != RefKind::None {
                // The spec demands the same types for ref/out/in parameters; Roslyn allows
                // identity conversions.
                if !self.identity(pd.ty, pm.ty) {
                    return false;
                }
            } else if !self.identity(pd.ty, pm.ty) && !self.is_implicit_reference(pd.ty, pm.ty) {
                // By-value parameters need an identity or reference conversion from pd to pm.
                return false;
            }
        }
        if m.return_type_is_ref_readonly != d.return_type_is_ref_readonly {
            return false;
        }
        self.identity(m.return_type, d.return_type)
            || self.is_implicit_reference(m.return_type, d.return_type)
    }

    // ---- Tuples ----

    fn tuple_conversion_rr(
        &mut self,
        from_elements: &[ResolveResult],
        to: TypeId,
        explicit: bool,
    ) -> Conversion {
        // C# 9.0 spec: 10.2.13 (implicit tuple conversions) + 10.3.6 (explicit tuple
        // conversions)
        let Some(to_elements) = self.ts.tuple_element_types(to) else {
            return Conversion::None;
        };
        if from_elements.len() != to_elements.len() {
            return Conversion::None;
        }
        let mut conversions = Vec::with_capacity(from_elements.len());
        for (from, &to) in from_elements.iter().zip(&to_elements) {
            let c = if explicit {
                self.explicit(from, to)
            } else {
                self.implicit(from, to)
            };
            if !c.is_valid() {
                return Conversion::None;
            }
            conversions.push(c);
        }
        Conversion::tuple(conversions)
    }

    fn tuple_conversion(&mut self, from: TypeId, to: TypeId, explicit: bool) -> Conversion {
        // C# 9.0 spec: 10.2.13 (implicit tuple conversions) + 10.3.6 (explicit tuple
        // conversions)
        let Some(from_elements) = self.ts.tuple_element_types(from).filter(|e| !e.is_empty())
        else {
            return Conversion::None;
        };
        let Some(to_elements) = self.ts.tuple_element_types(to) else {
            return Conversion::None;
        };
        if from_elements.len() != to_elements.len() {
            return Conversion::None;
        }
        let mut conversions = Vec::with_capacity(from_elements.len());
        for (&from, &to) in from_elements.iter().zip(&to_elements) {
            let c = if explicit {
                self.explicit_type(from, to)
            } else {
                self.implicit_type(from, to)
            };
            if !c.is_valid() {
                return Conversion::None;
            }
            conversions.push(c);
        }
        Conversion::tuple(conversions)
    }

    // ---- Better conversion ----

    /// `BetterConversion(ResolveResult, IType, IType)`: the better conversion from an
    /// expression (C# 8.0 spec, 12.6.4.5): 0 neither, 1 `t1`, 2 `t2`.
    pub fn better_conversion(&mut self, rr: &ResolveResult, t1: TypeId, t2: TypeId) -> u8 {
        let t1_exact = self.is_exactly_matching(rr, t1);
        let t2_exact = self.is_exactly_matching(rr, t2);
        if t1_exact && !t2_exact {
            return 1;
        }
        if t2_exact && !t1_exact {
            return 2;
        }
        let rr_type = rr.ty(self.ts);
        if !t1_exact && !t2_exact {
            let c1 = self.is_implicit_span(rr_type, t1);
            let c2 = self.is_implicit_span(rr_type, t2);
            if c1 && !c2 {
                return 1;
            }
            if c2 && !c1 {
                return 2;
            }
        }
        if t1_exact == t2_exact {
            let r = self.better_conversion_target(t1, t2);
            if r != 0 {
                return r;
            }
        }
        let ResolveResult::Lambda(lambda) = rr else {
            return self.better_conversion_from_type(rr_type, t1, t2);
        };
        let (t1, t2) = if lambda.is_anonymous_method() {
            (t1, t2)
        } else {
            (
                self.unpack_expression_tree_type(t1),
                self.unpack_expression_tree_type(t2),
            )
        };
        let (Some(m1), Some(m2)) = (self.ts.delegate_invoke(t1), self.ts.delegate_invoke(t2))
        else {
            return 0;
        };
        if m1.parameters.len() != m2.parameters.len() {
            return 0;
        }
        let mut parameter_types = Vec::with_capacity(m1.parameters.len());
        for (p1, p2) in m1.parameters.iter().zip(&m2.parameters) {
            if p1.ty != p2.ty {
                return 0;
            }
            parameter_types.push(p1.ty);
        }
        if lambda.has_parameter_list() && parameter_types.len() != lambda.parameters().len() {
            return 0;
        }
        let (mut ret1, mut ret2) = (m1.return_type, m2.return_type);
        let void1 = self.kind(ret1) == TypeKind::Void;
        let void2 = self.kind(ret2) == TypeKind::Void;
        if void1 && !void2 {
            return 2;
        }
        if !void1 && void2 {
            return 1;
        }
        let lambda = lambda.clone();
        let mut inferred = lambda.inferred_return_type(self, Some(&parameter_types));
        let mut r = self.better_conversion_from_type(inferred, ret1, ret2);
        if r == 0
            && lambda.is_async()
            && let (Some(a), Some(b), Some(c)) = (
                self.unpack_task(ret1),
                self.unpack_task(ret2),
                self.unpack_task(inferred),
            )
        {
            (ret1, ret2, inferred) = (a, b, c);
            r = self.better_conversion_from_type(inferred, ret1, ret2);
        }
        r
    }

    /// Whether an expression exactly matches a type (C# 8.0 spec, 12.6.4.6).
    fn is_exactly_matching(&mut self, e: &ResolveResult, t: TypeId) -> bool {
        let s = e.ty(self.ts);
        if self.identity(s, t) {
            return true;
        }
        let ResolveResult::Lambda(lambda) = e else {
            return false;
        };
        let t = if lambda.is_anonymous_method() {
            t
        } else {
            self.unpack_expression_tree_type(t)
        };
        let Some(m) = self.ts.delegate_invoke(t) else {
            return false;
        };
        let parameter_types: Vec<TypeId> = m.parameters.iter().map(|p| p.ty).collect();
        let lambda = lambda.clone();
        let x = lambda.inferred_return_type(self, Some(&parameter_types));
        let y = m.return_type;
        if self.identity(x, y) {
            return true;
        }
        let (x, y) = if lambda.is_async() {
            (self.unpack_task(x), self.unpack_task(y))
        } else {
            (Some(x), Some(y))
        };
        match (x, y) {
            (Some(x), Some(y)) => self.is_exactly_matching(&ResolveResult::Expression(x), y),
            _ => false,
        }
    }

    /// The `T` of a generic task type `TaskType[T]`, `None` for other types.
    fn unpack_task(&mut self, t: TypeId) -> Option<TypeId> {
        if (self.ts.is_task(t) || self.ts.is_custom_task(t))
            && self.ts.type_parameter_count_of(t) == 1
        {
            self.ts.first_type_argument(t)
        } else {
            None
        }
    }

    /// `BetterConversion(IType, IType, IType)`: the better conversion from a type (C# 4.0
    /// spec, 7.5.3.4): 0 neither, 1 `t1`, 2 `t2`.
    pub fn better_conversion_from_type(&mut self, s: TypeId, t1: TypeId, t2: TypeId) -> u8 {
        let ident1 = self.identity(s, t1);
        let ident2 = self.identity(s, t2);
        if ident1 && !ident2 {
            return 1;
        }
        if ident2 && !ident1 {
            return 2;
        }
        self.better_conversion_target(t1, t2)
    }

    /// The better conversion target (C# 9.0 spec, 12.6.4.7): 0 neither, 1 `t1`, 2 `t2`.
    fn better_conversion_target(&mut self, t1: TypeId, t2: TypeId) -> u8 {
        // ReadOnlySpan<E> beats Span<E>, before the mutual-convertibility rule (which would
        // conclude the opposite from the Span-to-ReadOnlySpan conversion). ReadOnlySpan<E1>
        // against ReadOnlySpan<E2> is decided by convertibility between the span types, which
        // the rule below tests.
        if self.is_known(t1, KnownTypeCode::ReadOnlySpanOfT)
            && self.is_known(t2, KnownTypeCode::SpanOfT)
        {
            let (a, b) = (self.first_arg(t1), self.first_arg(t2));
            if self.identity(a, b) {
                return 1;
            }
        }
        if self.is_known(t2, KnownTypeCode::ReadOnlySpanOfT)
            && self.is_known(t1, KnownTypeCode::SpanOfT)
        {
            let (a, b) = (self.first_arg(t2), self.first_arg(t1));
            if self.identity(a, b) {
                return 2;
            }
        }
        let t1_to_2 = self.implicit_type(t1, t2).is_valid();
        let t2_to_1 = self.implicit_type(t2, t1).is_valid();
        if t1_to_2 && !t2_to_1 {
            return 1;
        }
        if t2_to_1 && !t1_to_2 {
            return 2;
        }
        if let (Some(s1), Some(s2)) = (self.unpack_task(t1), self.unpack_task(t2)) {
            return self.better_conversion_target(s1, s2);
        }
        let c1 = self.ts.type_code(t1);
        let c2 = self.ts.type_code(t2);
        if is_better_integral_type(c1, c2) {
            return 1;
        }
        if is_better_integral_type(c2, c1) {
            return 2;
        }
        0
    }
}

/// Signed integral types are better than unsigned ones.
fn is_better_integral_type(t1: u8, t2: u8) -> bool {
    match t1 {
        code::SBYTE => matches!(t2, code::BYTE | code::UINT16 | code::UINT32 | code::UINT64),
        code::INT16 => matches!(t2, code::UINT16 | code::UINT32 | code::UINT64),
        code::INT32 => matches!(t2, code::UINT32 | code::UINT64),
        code::INT64 => t2 == code::UINT64,
        _ => false,
    }
}

/// `Convert.ToDouble(value) == 0` for the numeric constant values.
fn is_zero(value: &ConstValue) -> bool {
    match *value {
        ConstValue::SByte(v) => v == 0,
        ConstValue::Byte(v) => v == 0,
        ConstValue::Int16(v) => v == 0,
        ConstValue::UInt16(v) => v == 0,
        ConstValue::Int32(v) => v == 0,
        ConstValue::UInt32(v) => v == 0,
        ConstValue::Int64(v) => v == 0,
        ConstValue::UInt64(v) => v == 0,
        ConstValue::Single(bits) => f32::from_bits(bits) == 0.0,
        ConstValue::Double(bits) => f64::from_bits(bits) == 0.0,
        // lo, mid and hi of `decimal.GetBits` are all zero
        ConstValue::Decimal([lo, mid, hi, _]) => lo == 0 && mid == 0 && hi == 0,
        _ => false,
    }
}
