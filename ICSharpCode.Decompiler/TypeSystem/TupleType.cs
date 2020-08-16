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
using System.Text;
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
		public ImmutableArray<IType> ElementTypes { get; }

		/// <summary>
		/// Gets the cardinality of the tuple.
		/// </summary>
		public int Cardinality => ElementTypes.Length;

		/// <summary>
		/// Gets the names of the tuple elements.
		/// </summary>
		public ImmutableArray<string> ElementNames { get; }
		
		public TupleType(ICompilation compilation, ImmutableArray<IType> elementTypes,
			ImmutableArray<string> elementNames = default(ImmutableArray<string>),
			IModule valueTupleAssembly = null)
		{
			this.Compilation = compilation;
			this.UnderlyingType = CreateUnderlyingType(compilation, elementTypes, valueTupleAssembly);
			this.ElementTypes = elementTypes;
			if (elementNames.IsDefault) {
				this.ElementNames = Enumerable.Repeat<string>(null, elementTypes.Length).ToImmutableArray();
			} else {
				Debug.Assert(elementNames.Length == elementTypes.Length);
				this.ElementNames = elementNames;
			}
		}

		static ParameterizedType CreateUnderlyingType(ICompilation compilation, ImmutableArray<IType> elementTypes, IModule valueTupleAssembly)
		{
			int remainder = (elementTypes.Length - 1) % (RestPosition - 1) + 1;
			Debug.Assert(remainder >= 1 && remainder < RestPosition);
			int pos = elementTypes.Length - remainder;
			var type = new ParameterizedType(
				FindValueTupleType(compilation, valueTupleAssembly, remainder),
				elementTypes.Slice(pos));
			while (pos > 0) {
				pos -= (RestPosition - 1);
				type = new ParameterizedType(
					FindValueTupleType(compilation, valueTupleAssembly, RestPosition),
					elementTypes.Slice(pos, RestPosition - 1).Concat(new[] { type }));
			}
			Debug.Assert(pos == 0);
			return type;
		}

		private static IType FindValueTupleType(ICompilation compilation, IModule valueTupleAssembly, int tpc)
		{
			var typeName = new TopLevelTypeName("System", "ValueTuple", tpc);
			if (valueTupleAssembly != null) {
				var typeDef = valueTupleAssembly.GetTypeDefinition(typeName);
				if (typeDef != null)
					return typeDef;
			}
			return compilation.FindType(typeName);
		}

		/// <summary>
		/// Gets whether the specified type is a valid underlying type for a tuple.
		/// Also returns true for tuple types themselves.
		/// </summary>
		public static bool IsTupleCompatible(IType type, out int tupleCardinality)
		{
			switch (type.Kind) {
				case TypeKind.Tuple:
					tupleCardinality = ((TupleType)type).ElementTypes.Length;
					return true;
				case TypeKind.Class:
				case TypeKind.Struct:
					if (type.Namespace == "System" && type.Name == "ValueTuple") {
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

		/// <summary>
		/// Construct a tuple type (without element names) from the given underlying type.
		/// Returns null if the input is not a valid underlying type.
		/// </summary>
		public static TupleType FromUnderlyingType(ICompilation compilation, IType type)
		{
			var elementTypes = GetTupleElementTypes(type);
			if (elementTypes.Length > 0) {
				return new TupleType(
					compilation,
					elementTypes,
					valueTupleAssembly: type.GetDefinition()?.ParentModule
				);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets the tuple element types from a tuple type or tuple underlying type.
		/// </summary>
		public static ImmutableArray<IType> GetTupleElementTypes(IType tupleType)
		{
			List<IType> output = null;
			if (Collect(tupleType)) {
				return output.ToImmutableArray();
			} else {
				return default(ImmutableArray<IType>);
			}

			bool Collect(IType type)
			{
				switch (type.Kind) {
					case TypeKind.Tuple:
						if (output == null)
							output = new List<IType>();
						output.AddRange(((TupleType)type).ElementTypes);
						return true;
					case TypeKind.Class:
					case TypeKind.Struct:
						if (type.Namespace == "System" && type.Name == "ValueTuple") {
							if (output == null)
								output = new List<IType>();
							int tpc = type.TypeParameterCount;
							if (tpc > 0 && tpc < RestPosition) {
								output.AddRange(type.TypeArguments);
								return true;
							} else if (tpc == RestPosition) {
								output.AddRange(type.TypeArguments.Take(RestPosition - 1));
								return Collect(type.TypeArguments[RestIndex]);
							}
						}
						break;
				}
				return false;
			}
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
				&& ElementNames.SequenceEqual(o.ElementNames);
		}

		public override int GetHashCode()
		{
			unchecked {
				int hash = UnderlyingType.GetHashCode();
				foreach (string name in ElementNames) {
					hash *= 31;
					hash += name != null ? name.GetHashCode() : 0;
				}
				return hash;
			}
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append('(');
			for (int i = 0; i < ElementTypes.Length; i++) {
				if (i > 0)
					b.Append(", ");
				b.Append(ElementTypes[i]);
				if (ElementNames[i] != null) {
					b.Append(' ');
					b.Append(ElementNames[i]);
				}
			}
			b.Append(')');
			return b.ToString();
		}

		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTupleType(this);
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType[] newElementTypes = null;
			for (int i = 0; i < ElementTypes.Length; i++) {
				IType type = ElementTypes[i];
				var newType = type.AcceptVisitor(visitor);
				if (newType != type) {
					if (newElementTypes == null) {
						newElementTypes = ElementTypes.ToArray();
					}
					newElementTypes[i] = newType;
				}
			}
			if (newElementTypes != null) {
				return new TupleType(this.Compilation, newElementTypes.ToImmutableArray(), this.ElementNames,
					this.GetDefinition()?.ParentModule);
			} else {
				return this;
			}
		}

		public override IEnumerable<IMethod> GetAccessors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetAccessors(filter, options);
		}

		public override IEnumerable<IMethod> GetConstructors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			// CS8181 'new' cannot be used with tuple type. Use a tuple literal expression instead.
			return EmptyList<IMethod>.Instance;
		}

		public override ITypeDefinition GetDefinition()
		{
			return UnderlyingType.GetDefinition();
		}

		public override IEnumerable<IEvent> GetEvents(Predicate<IEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetEvents(filter, options);
		}

		public override IEnumerable<IField> GetFields(Predicate<IField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			// The fields from the underlying type (Item1..Item7 and Rest)
			foreach (var field in UnderlyingType.GetFields(filter, options)) {
				yield return field;
			}
			/*for (int i = 0; i < ElementTypes.Length; i++) {
				var type = ElementTypes[i];
				var name = ElementNames[i];
				int pos = i + 1;
				string itemName = "Item" + pos;
				if (name != itemName && name != null)
					yield return MakeField(type, name);
				if (pos >= RestPosition)
					yield return MakeField(type, itemName);
			}*/
		}

		/*private IField MakeField(IType type, string name)
		{
			var f = new DefaultUnresolvedField();
			f.ReturnType = SpecialType.UnknownType;
			f.Name = name;
			return new TupleElementField(f, Compilation.TypeResolveContext);
		}*/

		public override IEnumerable<IMethod> GetMethods(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetMethods(filter, options);
		}

		public override IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
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

		public override IEnumerable<IProperty> GetProperties(Predicate<IProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return UnderlyingType.GetProperties(filter, options);
		}
	}

	public class TupleTypeReference : ITypeReference
	{
		/// <summary>
		/// Gets the types of the tuple elements.
		/// </summary>
		public ImmutableArray<ITypeReference> ElementTypes { get; }

		/// <summary>
		/// Gets the names of the tuple elements.
		/// </summary>
		public ImmutableArray<string> ElementNames { get; }

		public IModuleReference ValueTupleAssembly { get; }

		public TupleTypeReference(ImmutableArray<ITypeReference> elementTypes)
		{
			this.ElementTypes = elementTypes;
		}

		public TupleTypeReference(ImmutableArray<ITypeReference> elementTypes,
			ImmutableArray<string> elementNames = default(ImmutableArray<string>),
			IModuleReference valueTupleAssembly = null)
		{
			this.ValueTupleAssembly = valueTupleAssembly;
			this.ElementTypes = elementTypes;
			this.ElementNames = elementNames;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return new TupleType(context.Compilation,
				ElementTypes.Select(t => t.Resolve(context)).ToImmutableArray(),
				ElementNames,
				ValueTupleAssembly?.Resolve(context)
			);
		}
	}

	public static class TupleTypeExtensions
	{
		public static IType TupleUnderlyingTypeOrSelf(this IType type)
		{
			var t = (type as TupleType)?.UnderlyingType ?? type;
			return t.WithoutNullability();
		}
	}
}
