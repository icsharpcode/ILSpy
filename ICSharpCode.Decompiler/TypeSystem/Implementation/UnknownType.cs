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
using System.Diagnostics;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// An unknown type where (part) of the name is known.
	/// </summary>
	[Serializable]
	public class UnknownType : AbstractType, ITypeDefinitionOrUnknown, ITypeReference
	{
		readonly bool namespaceKnown;
		readonly FullTypeName fullTypeName;
		readonly bool? isReferenceType = null;

		/// <summary>
		/// Creates a new unknown type.
		/// </summary>
		/// <param name="namespaceName">Namespace name, if known. Can be null if unknown.</param>
		/// <param name="name">Name of the type, must not be null.</param>
		/// <param name="typeParameterCount">Type parameter count, zero if unknown.</param>
		public UnknownType(string namespaceName, string name, int typeParameterCount = 0, bool? isReferenceType = null)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			this.namespaceKnown = namespaceName != null;
			this.fullTypeName = new TopLevelTypeName(namespaceName ?? string.Empty, name, typeParameterCount);
			this.isReferenceType = isReferenceType;
		}

		/// <summary>
		/// Creates a new unknown type.
		/// </summary>
		/// <param name="fullTypeName">Full name of the unknown type.</param>
		public UnknownType(FullTypeName fullTypeName, bool? isReferenceType = null)
		{
			this.isReferenceType = isReferenceType;
			if (fullTypeName.Name == null)
			{
				Debug.Assert(fullTypeName == default(FullTypeName));
				this.namespaceKnown = false;
				this.fullTypeName = new TopLevelTypeName(string.Empty, "?", 0);
			}
			else
			{
				this.namespaceKnown = true;
				this.fullTypeName = fullTypeName;
			}
		}

		public override TypeKind Kind {
			get { return TypeKind.Unknown; }
		}

		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			return this;
		}

		public override ITypeDefinitionOrUnknown GetDefinitionOrUnknown()
		{
			return this;
		}

		public override string Name {
			get { return fullTypeName.Name; }
		}

		public override string Namespace {
			get { return fullTypeName.TopLevelTypeName.Namespace; }
		}

		public override string ReflectionName {
			get { return namespaceKnown ? fullTypeName.ReflectionName : "?"; }
		}

		public FullTypeName FullTypeName => fullTypeName;

		public override int TypeParameterCount => fullTypeName.TypeParameterCount;
		public override IReadOnlyList<ITypeParameter> TypeParameters => DummyTypeParameter.GetClassTypeParameterList(TypeParameterCount);
		public override IReadOnlyList<IType> TypeArguments => TypeParameters;

		public override bool? IsReferenceType {
			get { return isReferenceType; }
		}

		public override IType ChangeNullability(Nullability nullability)
		{
			if (nullability == Nullability.Oblivious)
				return this;
			else
				return new NullabilityAnnotatedType(this, nullability);
		}

		public override int GetHashCode()
		{
			return (namespaceKnown ? 812571 : 12651) ^ fullTypeName.GetHashCode();
		}

		public override bool Equals(IType other)
		{
			UnknownType o = other as UnknownType;
			if (o == null)
				return false;
			return this.namespaceKnown == o.namespaceKnown && this.fullTypeName == o.fullTypeName && this.isReferenceType == o.isReferenceType;
		}

		public override string ToString()
		{
			return "[UnknownType " + fullTypeName.ReflectionName + "]";
		}
	}
}
