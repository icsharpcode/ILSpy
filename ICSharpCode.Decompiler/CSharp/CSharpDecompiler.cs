// Copyright (c) 2014 Daniel Grunwald
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

using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.CSharp
{
	public class CSharpDecompiler
	{
		CecilLoader cecilLoader = new CecilLoader { IncludeInternalMembers = true, LazyLoad = true };
		Dictionary<IUnresolvedEntity, MemberReference> entityDict = new Dictionary<IUnresolvedEntity, MemberReference>();
		ICompilation compilation;
		ITypeResolveContext mainAssemblyTypeResolveContext;
		TypeSystemAstBuilder typeSystemAstBuilder;
		StatementBuilder statementBuilder;

		public CancellationToken CancellationToken { get; set; }

		public CSharpDecompiler(ModuleDefinition module)
		{
			cecilLoader.OnEntityLoaded = (entity, mr) => {
				// entityDict needs locking because the type system is multi-threaded and may be accessed externally
				lock (entityDict)
					entityDict[entity] = mr;
			};

			IUnresolvedAssembly mainAssembly = cecilLoader.LoadModule(module);
			var referencedAssemblies = new List<IUnresolvedAssembly>();
			foreach (var asmRef in module.AssemblyReferences) {
				var asm = module.AssemblyResolver.Resolve(asmRef);
				if (asm != null)
					referencedAssemblies.Add(cecilLoader.LoadAssembly(asm));
			}
			compilation = new SimpleCompilation(mainAssembly, referencedAssemblies);
			mainAssemblyTypeResolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);

			typeSystemAstBuilder = new TypeSystemAstBuilder();
			typeSystemAstBuilder.AlwaysUseShortTypeNames = true;
			typeSystemAstBuilder.AddAnnotations = true;

			statementBuilder = new StatementBuilder(compilation);
		}

		MemberReference GetMemberReference(IMember member)
		{
			var unresolved = member.UnresolvedMember;
			lock (entityDict) {
				MemberReference mr;
				if (unresolved != null && entityDict.TryGetValue(unresolved, out mr))
					return mr;
			}
			return null;
		}

		ITypeDefinition GetTypeDefinition(TypeDefinition typeDef)
		{
			return compilation.MainAssembly.GetTypeDefinition(typeDef.GetFullTypeName());
		}

		IMethod GetMethod(MethodDefinition methodDef)
		{
			ITypeDefinition typeDef = GetTypeDefinition(methodDef.DeclaringType);
			if (typeDef == null)
				return null;
			return typeDef.Methods.FirstOrDefault(m => GetMemberReference(m) == methodDef);
		}

		public EntityDeclaration Decompile(MethodDefinition methodDefinition)
		{
			if (methodDefinition == null)
				throw new ArgumentNullException("methodDefinition");
			var method = GetMethod(methodDefinition);
			if (method == null)
				throw new InvalidOperationException("Could not find method in NR type system");
			var entityDecl = typeSystemAstBuilder.ConvertEntity(method);
			if (methodDefinition.HasBody) {
				var ilReader = new ILReader(methodDefinition.Body, CancellationToken);
				var inst = ilReader.CreateBlocks(true);
				var body = statementBuilder.ConvertBlockContainer(inst);
				body.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
				entityDecl.AddChild(body, Roles.Body);
			}
			return entityDecl;
		}
	}
}
