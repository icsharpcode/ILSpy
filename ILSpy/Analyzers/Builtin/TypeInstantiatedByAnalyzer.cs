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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows methods that instantiate a type.
	/// </summary>
	[Export(typeof(IAnalyzer<ITypeDefinition>))]
	class TypeInstantiatedByAnalyzer : IMethodBodyAnalyzer<ITypeDefinition>
	{
		public string Text => "Instantiated By";

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedEntity, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			bool found = false;
			var blob = methodBody.GetILReader();
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer

			while (!found && blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				if (!CanBeReference(opCode)) {
					blob.SkipOperand(opCode);
					continue;
				}
				EntityHandle methodHandle = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!methodHandle.Kind.IsMemberKind())
					continue;
				var ctor = context.TypeSystem.MainModule.ResolveMethod(methodHandle, genericContext);
				if (ctor == null || !ctor.IsConstructor)
					continue;

				found = ctor.DeclaringTypeDefinition?.MetadataToken == analyzedEntity.MetadataToken
					&& ctor.ParentModule.PEFile == analyzedEntity.ParentModule.PEFile;
			}

			if (found) {
				yield return method;
			}
		}

		bool CanBeReference(ILOpCode opCode)
		{
			return opCode == ILOpCode.Newobj || opCode == ILOpCode.Initobj;
		}

		public bool Show(ITypeDefinition entity) => !entity.IsAbstract && !entity.IsStatic;
	}
}
