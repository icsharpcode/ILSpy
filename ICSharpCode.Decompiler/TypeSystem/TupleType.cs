// Copyright (c) 2018 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public sealed class TupleType : AbstractType, ICompilationProvider
	{
		public const int RestPosition = 8;
		const int RestIndex = RestPosition - 1;

		public ICompilation Compilation { get; }

		/// <summary>
		/// Gets the underlying <c>System.ValueType</c> type.
		/// </summary>
		public ParameterizedType UnderlyingType { get; }

		/// <summary>
		/// Gets the tuple elements.
		/// </summary>
		public ImmutableArray<IType> TupleElementTypes { get; }

		/// <summary>
		/// Gets the names of the tuple elements.
		/// </summary>
		public ImmutableArray<string> TupleElementNames { get; }

		public bool HasCustomElementNames { get; }

		public TupleType(ICompilation compilation, ImmutableArray<IType> elementTypes)
		{
			this.Compilation = compilation;
			this.UnderlyingType = CreateUnderlyingType(compilation, elementTypes);
			this.TupleElementTypes = elementTypes;
			this.TupleElementNames = Enumerable.Range(1, elementTypes.Length).Select(p => "Item" + p).ToImmutableArray();
			this.HasCustomElementNames = false;
		}

		public TupleType(ICompilation compilation, ImmutableArray<IType> elementTypes, ImmutableArray<string> elementNames)
		{
			this.Compilation = compilation;
			this.UnderlyingType = CreateUnderlyingType(compilation, elementTypes);
			this.TupleElementTypes = elementTypes;
			this.TupleElementNames = elementNames;
			this.HasCustomElementNames = true;
		}

		static ParameterizedType CreateUnderlyingType(ICompilation compilation, ImmutableArray<IType> elementTypes)
		{
			int remainder = (elementTypes.Length - 1) % (RestPosition - 1) + 1;
			Debug.Assert(remainder >= 1 && remainder < RestPosition);
			int pos = elementTypes.Length - remainder;
			var type = new ParameterizedType(
				compilation.FindType(new TopLevelTypeName("System", "ValueTuple", remainder)),
				elementTypes.Slice(pos));
			while (pos > 0) {
				pos -= (RestPosition - 1);
				type = new ParameterizedType(
					compilation.FindType(new TopLevelTypeName("System", "ValueTuple", RestPosition)),
					elementTypes.Slice(pos, RestPosition - 1).Concat(new[] { type }));
			}
			Debug.Assert(pos == 0);
			return type;
		}

		/// <summary>
		/// Gets whether the specified type is a valid underlying type for a tuple.
		/// Also returns type for tuple types themselves.
		/// </summary>
		public static bool IsTupleCompatible(IType type, out int tupleCardinality)
		{
			switch (type.Kind) {
				case TypeKind.Tuple:
					tupleCardinality = ((TupleType)type).TupleElementNames.Length;
					return true;
				case TypeKind.Class:
				case TypeKind.Struct:
					if (type.Namespace == "System" && type.Name == "ValueType") {
						int tpc = type.TypeParameterCount;
						if (tpc > 0 && tpc < RestPosition) {
							tupleCardinality = tpc;
							return true;
						} else if (tpc == RestPosition && type is ParameterizedType pt) {
							if (IsTupleCompatible(pt.TypeArguments[RestIndex], out tupleCardinality)) {
								tupleCardinality += RestPosition - 1;
								return true;
							}
						}
					}
					break;
			}
			tupleCardinality = 0;
			return false;
		}

		static bool CollectTupleElementTypes(IType type, List<IType> output)
		{
			switch (type.Kind) {
				case TypeKind.Tuple:
					output.AddRange(((TupleType)type).TupleElementTypes);
					return true;
				case TypeKind.Class:
				case TypeKind.Struct:
					if (type.Namespace == "System" && type.Name == "ValueType") {
						int tpc = type.TypeParameterCount;
						if (tpc > 0 && tpc < RestPosition) {
							output.AddRange(type.TypeArguments);
							return true;
						} else if (tpc == RestPosition) {
							output.AddRange(type.TypeArguments.Take(RestPosition - 1));
							return CollectTupleElementTypes(type.TypeArguments[RestIndex], output);
						}
					}
					break;
			}
			return false;
		}

		public override TypeKind Kind => TypeKind.Tuple;
		public override bool? IsReferenceType => UnderlyingType.IsReferenceType;
		public override int TypeParameterCount => 0;
		public override IReadOnlyList<ITypeParameter> TypeParameters => EmptyList<ITypeParameter>.Instance;
		public override IReadOnlyList<IType> TypeArguments => EmptyList<IType>.Instance;
		public override IEnumerable<IType> DirectBaseTypes => UnderlyingType.DirectBaseTypes;
		public override string FullName => UnderlyingType.FullName;
		public override string Name => UnderlyingType.Name;
		public override string ReflectionName => UnderlyingType.ReflectionName;
		public override string Namespace => UnderlyingType.Namespace;

		public override bool Equals(IType other)
		{
			var o = other as TupleType;
			if (o == null)
				return false;
			if (!UnderlyingType.Equals(o.UnderlyingType))
				return false;
			return UnderlyingType.Equals(o.UnderlyingType)
				&& TupleElementNames.SequenceEqual(o.TupleElementNames);
		}

		public override int GetHashCode()
		{
			unchecked {
				int hash = UnderlyingType.GetHashCode();
				foreach (string name in TupleElementNames) {
					hash *= 31;
					hash += name.GetHashCode();
				}
				return hash;
			}
		}

		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTupleType(this);
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType[] newElementTypes = null;
			for (int i = 0; i < TupleElementTypes.Length; i++) {
				IType type = TupleElementTypes[i];
				var newType = type.AcceptVisitor(visitor);
				if (newType != type) {
					if (newElementTypes == null) {
						newElementTypes = TupleElementTypes.ToArray();
					}
					newElementTypes[i] = newType;
				}
			}
			if (newElementTypes != null) {
				return new TupleType(this.Compilation, newElementTypes.ToImmutableArray(), this.TupleElementNames);
			} else {
				return this;
			}
		}

		public override IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetAccessors(filter, options);
		}

		public override IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			// CS8181 'new' cannot be used with tuple type. Use a tuple literal expression instead.
			return EmptyList<IMethod>.Instance;
		}

		public override ITypeDefinition GetDefinition()
		{
			return UnderlyingType.GetDefinition();
		}

		public override IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetEvents(filter, options);
		}

		public override IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			// The fields from the underlying type (Item1..Item7 and Rest)
			foreach (var field in UnderlyingType.GetFields(filter, options)) {
				yield return field;
			}
			for (int i = 0; i <= TupleElementTypes.Length; i++) {
				var type = TupleElementTypes[i];
				var name = TupleElementNames[i];
				int pos = i + 1;
				string itemName = "Item" + pos;
				if (name != itemName)
					yield return MakeField(type, name);
				if (pos >= RestPosition)
					yield return MakeField(type, itemName);
			}
		}

		private IField MakeField(IType type, string name)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetMethods(filter, options);
		}

		public override IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetMethods(typeArguments, filter, options);
		}

		public override IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetNestedTypes(filter, options);
		}

		public override IEnumerable<IType> GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetNestedTypes(typeArguments, filter, options);
		}

		public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetProperties(filter, options);
		}
	}

	public static class TupleTypeExtensions
	{
		public static IType TupleUnderlyingTypeOrSelf(this IType type)
		{
			return (type as TupleType)?.UnderlyingType ?? type;
		}
	}
}
