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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Type reference used to reference nested types.
	/// </summary>
	[Serializable]
	public sealed class NestedTypeReference : ITypeReference, ISymbolReference, ISupportsInterning
	{
		readonly bool? isReferenceType;

		/// <summary>
		/// Creates a new NestedTypeReference.
		/// </summary>
		/// <param name="declaringTypeRef">Reference to the declaring type.</param>
		/// <param name="name">Name of the nested class</param>
		/// <param name="additionalTypeParameterCount">Number of type parameters on the inner class (without type parameters on baseTypeRef)</param>
		/// <remarks>
		/// <paramref name="declaringTypeRef"/> must be exactly the (unbound) declaring type, not a derived type, not a parameterized type.
		/// NestedTypeReference thus always resolves to a type definition, never to (partially) parameterized types.
		/// </remarks>
		public NestedTypeReference(ITypeReference declaringTypeRef, string name, int additionalTypeParameterCount, bool? isReferenceType = null)
		{
			this.DeclaringTypeReference = declaringTypeRef ?? throw new ArgumentNullException("declaringTypeRef");
			this.Name = name ?? throw new ArgumentNullException("name");
			this.AdditionalTypeParameterCount = additionalTypeParameterCount;
			this.isReferenceType = isReferenceType;
		}
		
		public ITypeReference DeclaringTypeReference { get; }

		public string Name { get; }

		public int AdditionalTypeParameterCount { get; }

		public IType Resolve(ITypeResolveContext context)
		{
			var declaringType = DeclaringTypeReference.Resolve(context) as ITypeDefinition;
			if (declaringType != null) {
				var tpc = declaringType.TypeParameterCount;
				foreach (IType type in declaringType.NestedTypes) {
					if (type.Name == Name && type.TypeParameterCount == tpc + AdditionalTypeParameterCount)
						return type;
				}
			}
			return new UnknownType(null, Name, AdditionalTypeParameterCount);
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			var type = Resolve(context);
			if (type is ITypeDefinition)
				return (ISymbol)type;
			return null;
		}
		
		public override string ToString()
		{
			if (AdditionalTypeParameterCount == 0)
				return DeclaringTypeReference + "+" + Name;
			else
				return DeclaringTypeReference + "+" + Name + "`" + AdditionalTypeParameterCount;
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return DeclaringTypeReference.GetHashCode() ^ Name.GetHashCode() ^ AdditionalTypeParameterCount;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as NestedTypeReference;
			return o != null && DeclaringTypeReference == o.DeclaringTypeReference && Name == o.Name 
				&& AdditionalTypeParameterCount == o.AdditionalTypeParameterCount
				&& isReferenceType == o.isReferenceType;
		}
	}
}
