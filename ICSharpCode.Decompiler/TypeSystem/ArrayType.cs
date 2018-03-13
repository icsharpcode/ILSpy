// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents an array type.
	/// </summary>
	public sealed class ArrayType : TypeWithElementType, ICompilationProvider
	{
		public ArrayType(ICompilation compilation, IType elementType, int dimensions = 1) : base(elementType)
		{
			if (dimensions <= 0)
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			this.Compilation = compilation ?? throw new ArgumentNullException("compilation");
			this.Dimensions = dimensions;
			
			var p = elementType as ICompilationProvider;
			if (p != null && p.Compilation != compilation)
				throw new InvalidOperationException("Cannot create an array type using a different compilation from the element type.");
		}
		
		public override TypeKind Kind => TypeKind.Array;

		public ICompilation Compilation { get; }

		public int Dimensions { get; }

		public override string NameSuffix => "[" + new string(',', Dimensions-1) + "]";

		public override bool? IsReferenceType => true;

		public override int GetHashCode()
		{
			return unchecked(elementType.GetHashCode() * 71681 + Dimensions);
		}
		
		public override bool Equals(IType other)
		{
			var a = other as ArrayType;
			return a != null && elementType.Equals(a.elementType) && a.Dimensions == Dimensions;
		}
		
		public override ITypeReference ToTypeReference()
		{
			return new ArrayTypeReference(elementType.ToTypeReference(), Dimensions);
		}
		
		public override IEnumerable<IType> DirectBaseTypes {
			get {
				var baseTypes = new List<IType>();
				var t = Compilation.FindType(KnownTypeCode.Array);
				if (t.Kind != TypeKind.Unknown)
					baseTypes.Add(t);
				if (Dimensions == 1 && elementType.Kind != TypeKind.Pointer) {
					// single-dimensional arrays implement IList<T>
					var def = Compilation.FindType(KnownTypeCode.IListOfT) as ITypeDefinition;
					if (def != null)
						baseTypes.Add(new ParameterizedType(def, new[] { elementType }));
					// And in .NET 4.5 they also implement IReadOnlyList<T>
					def = Compilation.FindType(KnownTypeCode.IReadOnlyListOfT) as ITypeDefinition;
					if (def != null)
						baseTypes.Add(new ParameterizedType(def, new[] { elementType }));
				}
				return baseTypes;
			}
		}
		
		public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return Compilation.FindType(KnownTypeCode.Array).GetMethods(filter, options);
		}
		
		public override IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return Compilation.FindType(KnownTypeCode.Array).GetMethods(typeArguments, filter, options);
		}
		
		public override IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return Compilation.FindType(KnownTypeCode.Array).GetAccessors(filter, options);
		}
		
		public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IProperty>.Instance;
			else
				return Compilation.FindType(KnownTypeCode.Array).GetProperties(filter, options);
		}
		
		// NestedTypes, Events, Fields: System.Array doesn't have any; so we can use the AbstractType default implementation
		// that simply returns an empty list
		
		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitArrayType(this);
		}
		
		public override IType VisitChildren(TypeVisitor visitor)
		{
			var e = elementType.AcceptVisitor(visitor);
			if (e == elementType)
				return this;
			else
				return new ArrayType(Compilation, e, Dimensions);
		}
	}
	
	[Serializable]
	public sealed class ArrayTypeReference : ITypeReference, ISupportsInterning
	{
		public ArrayTypeReference(ITypeReference elementType, int dimensions = 1)
		{
			if (dimensions <= 0)
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			this.ElementType = elementType ?? throw new ArgumentNullException("elementType");
			this.Dimensions = dimensions;
		}
		
		public ITypeReference ElementType { get; }

		public int Dimensions { get; }

		public IType Resolve(ITypeResolveContext context)
		{
			return new ArrayType(context.Compilation, ElementType.Resolve(context), Dimensions);
		}
		
		public override string ToString()
		{
			return ElementType.ToString() + "[" + new string(',', Dimensions - 1) + "]";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return ElementType.GetHashCode() ^ Dimensions;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as ArrayTypeReference;
			return o != null && ElementType == o.ElementType && Dimensions == o.Dimensions;
		}
	}
}
