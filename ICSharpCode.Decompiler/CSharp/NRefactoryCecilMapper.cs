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

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Maps from cecil types to NRefactory and vice versa.
	/// </summary>
	class NRefactoryCecilMapper
	{
		readonly ITypeResolveContext context;
		readonly CecilLoader cecilLoader = new CecilLoader();
		readonly Func<IUnresolvedEntity, MemberReference> nr2cecilLookup;
		
		/// <param name="compilation">Compilation to use for Cecil-&gt;NRefactory lookups</param>
		/// <param name="module">Compilation to use for Cecil-&gt;NRefactory lookups</param>
		/// <param name = "nr2cecilLookup">NRefactory-&gt;Cecil lookup function</param>
		internal NRefactoryCecilMapper(ICompilation compilation, ModuleDefinition module, Func<IUnresolvedEntity, MemberReference> nr2cecilLookup)
		{
			this.nr2cecilLookup = nr2cecilLookup;
			this.context = new SimpleTypeResolveContext(compilation.MainAssembly);
			this.cecilLoader.SetCurrentModule(module);
		}
		
		public MemberReference GetCecil(IMember member)
		{
			if (member == null)
				return null;
			return nr2cecilLookup(member.UnresolvedMember);
		}

		public MemberReference GetCecil(ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				return null;
			return nr2cecilLookup(typeDefinition.Parts[0]);
		}
		
		/// <summary>
		/// Retrieves a type definition for a type defined in the compilation's main assembly.
		/// </summary>
		public IType GetType(TypeReference typeReference)
		{
			if (typeReference == null)
				return SpecialType.UnknownType;
			var typeRef = cecilLoader.ReadTypeReference(typeReference);
			return typeRef.Resolve(context);
		}
		
		public IMethod GetMethod(MethodReference methodReference)
		{
			var method = GetNonGenericMethod(methodReference.GetElementMethod());
			// TODO: specialize the method
			return method;
		}
		
		IMethod GetNonGenericMethod(MethodReference methodReference)
		{
			ITypeDefinition typeDef = GetType(methodReference.DeclaringType).GetDefinition();
			if (typeDef == null)
				return null;
			IEnumerable<IMethod> methods;
			if (methodReference.Name == ".ctor") {
				methods = typeDef.GetConstructors();
			} else if (methodReference.Name == ".cctor") {
				return typeDef.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
			} else {
				methods = typeDef.GetMethods(m => m.Name == methodReference.Name, GetMemberOptions.IgnoreInheritedMembers)
					.Concat(typeDef.GetAccessors(m => m.Name == methodReference.Name, GetMemberOptions.IgnoreInheritedMembers));
			}
			foreach (var method in methods) {
				if (GetCecil(method) == methodReference)
					return method;
			}
			var parameterTypes = methodReference.Parameters.SelectArray(p => GetType(p.ParameterType));
			foreach (var method in methods) {
				if (parameterTypes.Length == method.Parameters.Count) {
					bool signatureMatches = true;
					for (int i = 0; i < parameterTypes.Length; i++) {
						IType type1 = DummyTypeParameter.NormalizeAllTypeParameters(parameterTypes[i]);
						IType type2 = DummyTypeParameter.NormalizeAllTypeParameters(method.Parameters[i].Type);
						if (!type1.Equals(type2)) {
							signatureMatches = false;
							break;
						}
					}
					if (signatureMatches)
						return method;
				}
			}
			return null;
		}
	}
}
