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
		readonly IModule module;
		readonly ResolvedUsingScope currentUsingScope;
		readonly ITypeDefinition currentTypeDefinition;
		readonly IMember currentMember;
		readonly string[] methodTypeParameterNames;

		public CSharpTypeResolveContext(IModule module, ResolvedUsingScope usingScope = null, ITypeDefinition typeDefinition = null, IMember member = null)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			this.module = module;
			this.currentUsingScope = usingScope;
			this.currentTypeDefinition = typeDefinition;
			this.currentMember = member;
		}

		private CSharpTypeResolveContext(IModule module, ResolvedUsingScope usingScope, ITypeDefinition typeDefinition, IMember member, string[] methodTypeParameterNames)
		{
			this.module = module;
			this.currentUsingScope = usingScope;
			this.currentTypeDefinition = typeDefinition;
			this.currentMember = member;
			this.methodTypeParameterNames = methodTypeParameterNames;
		}

		public ResolvedUsingScope CurrentUsingScope {
			get { return currentUsingScope; }
		}

		public ICompilation Compilation {
			get { return module.Compilation; }
		}

		public IModule CurrentModule {
			get { return module; }
		}

		public ITypeDefinition CurrentTypeDefinition {
			get { return currentTypeDefinition; }
		}

		public IMember CurrentMember {
			get { return currentMember; }
		}

		public CSharpTypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return new CSharpTypeResolveContext(module, currentUsingScope, typeDefinition, currentMember, methodTypeParameterNames);
		}

		ITypeResolveContext ITypeResolveContext.WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return WithCurrentTypeDefinition(typeDefinition);
		}

		public CSharpTypeResolveContext WithCurrentMember(IMember member)
		{
			return new CSharpTypeResolveContext(module, currentUsingScope, currentTypeDefinition, member, methodTypeParameterNames);
		}

		ITypeResolveContext ITypeResolveContext.WithCurrentMember(IMember member)
		{
			return WithCurrentMember(member);
		}

		public CSharpTypeResolveContext WithUsingScope(ResolvedUsingScope usingScope)
		{
			return new CSharpTypeResolveContext(module, usingScope, currentTypeDefinition, currentMember, methodTypeParameterNames);
		}
	}
}
