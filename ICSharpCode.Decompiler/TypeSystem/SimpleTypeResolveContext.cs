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

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Default ITypeResolveContext implementation.
	/// </summary>
	public class SimpleTypeResolveContext : ITypeResolveContext
	{
		readonly ICompilation compilation;
		readonly IModule currentModule;
		readonly ITypeDefinition currentTypeDefinition;
		readonly IMember currentMember;
		
		public SimpleTypeResolveContext(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException(nameof(compilation));
			this.compilation = compilation;
		}
		
		public SimpleTypeResolveContext(IModule module)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			this.compilation = module.Compilation;
			this.currentModule = module;
		}
		
		public SimpleTypeResolveContext(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			this.compilation = entity.Compilation;
			this.currentModule = entity.ParentModule;
			this.currentTypeDefinition = (entity as ITypeDefinition) ?? entity.DeclaringTypeDefinition;
			this.currentMember = entity as IMember;
		}
		
		private SimpleTypeResolveContext(ICompilation compilation, IModule currentModule, ITypeDefinition currentTypeDefinition, IMember currentMember)
		{
			this.compilation = compilation;
			this.currentModule = currentModule;
			this.currentTypeDefinition = currentTypeDefinition;
			this.currentMember = currentMember;
		}
		
		public ICompilation Compilation {
			get { return compilation; }
		}
		
		public IModule CurrentModule {
			get { return currentModule; }
		}
		
		public ITypeDefinition CurrentTypeDefinition {
			get { return currentTypeDefinition; }
		}
		
		public IMember CurrentMember {
			get { return currentMember; }
		}
		
		public ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return new SimpleTypeResolveContext(compilation, currentModule, typeDefinition, currentMember);
		}
		
		public ITypeResolveContext WithCurrentMember(IMember member)
		{
			return new SimpleTypeResolveContext(compilation, currentModule, currentTypeDefinition, member);
		}
	}
}
