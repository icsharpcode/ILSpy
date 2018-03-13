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
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public sealed class PointerType : TypeWithElementType
	{
		public PointerType(IType elementType) : base(elementType)
		{
		}
		
		public override TypeKind Kind => TypeKind.Pointer;

		public override string NameSuffix => "*";

		public override bool? IsReferenceType => null;

		public override int GetHashCode()
		{
			return elementType.GetHashCode() ^ 91725811;
		}
		
		public override bool Equals(IType other)
		{
			var a = other as PointerType;
			return a != null && elementType.Equals(a.elementType);
		}
		
		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitPointerType(this);
		}
		
		public override IType VisitChildren(TypeVisitor visitor)
		{
			var e = elementType.AcceptVisitor(visitor);
			if (e == elementType)
				return this;
			else
				return new PointerType(e);
		}
		
		public override ITypeReference ToTypeReference()
		{
			return new PointerTypeReference(elementType.ToTypeReference());
		}
	}
	
	[Serializable]
	public sealed class PointerTypeReference : ITypeReference, ISupportsInterning
	{
		public PointerTypeReference(ITypeReference elementType)
		{
			this.ElementType = elementType ?? throw new ArgumentNullException("elementType");
		}
		
		public ITypeReference ElementType { get; }

		public IType Resolve(ITypeResolveContext context)
		{
			return new PointerType(ElementType.Resolve(context));
		}
		
		public override string ToString()
		{
			return ElementType.ToString() + "*";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return ElementType.GetHashCode() ^ 91725812;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as PointerTypeReference;
			return o != null && this.ElementType == o.ElementType;
		}
	}
}
