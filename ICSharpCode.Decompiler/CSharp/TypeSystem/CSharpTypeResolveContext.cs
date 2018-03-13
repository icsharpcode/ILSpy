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
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.TypeSystem
{
	public sealed class CSharpTypeResolveContext : ITypeResolveContext
	{
		readonly string[] methodTypeParameterNames;
		
		public CSharpTypeResolveContext(IAssembly assembly, ResolvedUsingScope usingScope = null, ITypeDefinition typeDefinition = null, IMember member = null)
		{
			this.CurrentAssembly = assembly ?? throw new ArgumentNullException("assembly");
			this.CurrentUsingScope = usingScope;
			this.CurrentTypeDefinition = typeDefinition;
			this.CurrentMember = member;
		}
		
		private CSharpTypeResolveContext(IAssembly assembly, ResolvedUsingScope usingScope, ITypeDefinition typeDefinition, IMember member, string[] methodTypeParameterNames)
		{
			this.CurrentAssembly = assembly;
			this.CurrentUsingScope = usingScope;
			this.CurrentTypeDefinition = typeDefinition;
			this.CurrentMember = member;
			this.methodTypeParameterNames = methodTypeParameterNames;
		}
		
		public ResolvedUsingScope CurrentUsingScope { get; }

		public ICompilation Compilation => CurrentAssembly.Compilation;

		public IAssembly CurrentAssembly { get; }

		public ITypeDefinition CurrentTypeDefinition { get; }

		public IMember CurrentMember { get; }

		public CSharpTypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return new CSharpTypeResolveContext(CurrentAssembly, CurrentUsingScope, typeDefinition, CurrentMember, methodTypeParameterNames);
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return WithCurrentTypeDefinition(typeDefinition);
		}
		
		public CSharpTypeResolveContext WithCurrentMember(IMember member)
		{
			return new CSharpTypeResolveContext(CurrentAssembly, CurrentUsingScope, CurrentTypeDefinition, member, methodTypeParameterNames);
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentMember(IMember member)
		{
			return WithCurrentMember(member);
		}
		
		public CSharpTypeResolveContext WithUsingScope(ResolvedUsingScope usingScope)
		{
			return new CSharpTypeResolveContext(CurrentAssembly, usingScope, CurrentTypeDefinition, CurrentMember, methodTypeParameterNames);
		}
	}
}
