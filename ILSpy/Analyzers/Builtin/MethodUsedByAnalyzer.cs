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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that are used by a method.
	/// </summary>
	[Export(typeof(IAnalyzer<IMethod>))]
	class MethodUsedByAnalyzer : IMethodBodyAnalyzer<IMethod>
	{
		public string Text => "Used By";

		public bool Show(IMethod entity) => !entity.IsVirtual;

		public IEnumerable<IEntity> Analyze(IMethod analyzedMethod, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			var blob = methodBody.GetILReader();

			var baseMethod = InheritanceHelper.GetBaseMember(analyzedMethod);

			while (blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				if (opCode != ILOpCode.Call && opCode != ILOpCode.Callvirt) {
					ILParser.SkipOperand(ref blob, opCode);
					continue;
				}
				var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (member.IsNil || !member.Kind.IsMemberKind()) continue;

				var m = context.TypeSystem.ResolveAsMember(member)?.MemberDefinition;
				if (m == null) continue;

				if (opCode == ILOpCode.Call) {
					if (IsSameMember(analyzedMethod, m)) {
						yield return method;
						yield break;
					}
				}

				if (opCode == ILOpCode.Callvirt && baseMethod != null) {
					if (IsSameMember(baseMethod, m)) {
						yield return method;
						yield break;
					}
				}
			}
		}

		static bool IsSameMember(IMember analyzedMethod, IMember m)
		{
			return m.MetadataToken == analyzedMethod.MetadataToken
				&& m.ParentAssembly.PEFile == analyzedMethod.ParentAssembly.PEFile;
		}
	}
}
