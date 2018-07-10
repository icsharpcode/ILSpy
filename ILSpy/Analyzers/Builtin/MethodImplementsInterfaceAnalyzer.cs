// Copyright (c) 2018 Siegfried Pammer
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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows methods that implement an interface method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodImplementsInterfaceAnalyzer : ITypeDefinitionAnalyzer<IMethod>
	{
		public string Text => "Implemented By";

		public IEnumerable<IEntity> Analyze(IMethod analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			var token = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentAssembly.PEFile;
			if (!type.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == token && t.ParentAssembly.PEFile == module))
				yield break;

			foreach (var method in type.GetMethods(options: GetMemberOptions.ReturnMemberDefinitions)) {
				if (InheritanceHelper.GetBaseMembers(method, true)
					.Any(m => m.DeclaringTypeDefinition.MetadataToken == token && m.ParentAssembly.PEFile == module))
					yield return method;
			}
		}

		public bool Show(IMethod entity)
		{
			return entity.DeclaringType.Kind == TypeKind.Interface;
		}
	}
}
