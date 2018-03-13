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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Implementation of <see cref="IEntity"/> that resolves an unresolved entity.
	/// </summary>
	public abstract class AbstractResolvedEntity : IEntity
	{
		protected readonly IUnresolvedEntity unresolved;
		protected readonly ITypeResolveContext parentContext;
		
		protected AbstractResolvedEntity(IUnresolvedEntity unresolved, ITypeResolveContext parentContext)
		{
			this.unresolved = unresolved ?? throw new ArgumentNullException("unresolved");
			this.parentContext = parentContext ?? throw new ArgumentNullException("parentContext");
			this.Attributes = unresolved.Attributes.CreateResolvedAttributes(parentContext);
		}
		
		public SymbolKind SymbolKind => unresolved.SymbolKind;

		public ITypeDefinition DeclaringTypeDefinition => parentContext.CurrentTypeDefinition;

		public virtual IType DeclaringType => parentContext.CurrentTypeDefinition;

		public IAssembly ParentAssembly => parentContext.CurrentAssembly;

		public IReadOnlyList<IAttribute> Attributes { get; protected set; }

		public abstract ISymbolReference ToReference();
		
		public bool IsStatic => unresolved.IsStatic;
		public bool IsAbstract => unresolved.IsAbstract;
		public bool IsSealed => unresolved.IsSealed;
		public bool IsShadowing => unresolved.IsShadowing;
		public bool IsSynthetic => unresolved.IsSynthetic;

		public ICompilation Compilation => parentContext.Compilation;

		public string FullName => unresolved.FullName;
		public string Name => unresolved.Name;
		public string ReflectionName => unresolved.ReflectionName;
		public string Namespace => unresolved.Namespace;

		public virtual Accessibility Accessibility => unresolved.Accessibility;
		public bool IsPrivate => Accessibility == Accessibility.Private;
		public bool IsPublic => Accessibility == Accessibility.Public;
		public bool IsProtected => Accessibility == Accessibility.Protected;
		public bool IsInternal => Accessibility == Accessibility.Internal;
		public bool IsProtectedOrInternal => Accessibility == Accessibility.ProtectedOrInternal;
		public bool IsProtectedAndInternal => Accessibility == Accessibility.ProtectedAndInternal;

		public override string ToString()
		{
			return "[" + this.SymbolKind.ToString() + " " + this.ReflectionName + "]";
		}
	}
}
