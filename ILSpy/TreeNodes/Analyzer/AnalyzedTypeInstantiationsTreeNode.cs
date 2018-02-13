// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection;
using System.Threading;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Dom;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	internal sealed class AnalyzedTypeInstantiationsTreeNode : AnalyzerSearchTreeNode
	{
		private readonly TypeDefinition analyzedType;
		private readonly bool isSystemObject;

		public AnalyzedTypeInstantiationsTreeNode(TypeDefinition analyzedType)
		{
			if (analyzedType.IsNil)
				throw new ArgumentNullException(nameof(analyzedType));

			this.analyzedType = analyzedType;

			this.isSystemObject = (analyzedType.FullName.ToString() == "System.Object");
		}

		public override object Text
		{
			get { return "Instantiated By"; }
		}

		protected override IEnumerable<AnalyzerTreeNode> FetchChildren(CancellationToken ct)
		{
			var analyzer = new ScopedWhereUsedAnalyzer<AnalyzerTreeNode>(analyzedType, FindReferencesInType);
			return analyzer.PerformAnalysis(ct).OrderBy(n => n.Text);
		}

		private IEnumerable<AnalyzerTreeNode> FindReferencesInType(TypeDefinition type)
		{
			foreach (MethodDefinition method in type.Methods) {
				bool found = false;
				if (!method.HasBody)
					continue;

				// ignore chained constructors
				// (since object is the root of everything, we can short circuit the test in this case)
				if (method.IsConstructor && (isSystemObject || analyzedType == type || analyzedType.IsBaseTypeOf(type)))
					continue;

				var blob = method.Body.GetILReader();

				while (!found && blob.RemainingBytes > 0) {
					var opCode = ILParser.DecodeOpCode(ref blob);
					switch (opCode.GetOperandType()) {
						case OperandType.Method:
						case OperandType.Sig:
						case OperandType.Tok:
							var member = ILParser.DecodeMemberToken(ref blob, method.Module);
							if (member.Name == ".ctor") {
								if (member.DeclaringType.FullName == analyzedType.FullName) {
									found = true;
								}
							}
							break;
						default:
							ILParser.SkipOperand(ref blob, opCode);
							break;
					}
				}

				if (found) {
					var node = new AnalyzedMethodTreeNode(method);
					node.Language = this.Language;
					yield return node;
				}
			}
		}

		public static bool CanShow(TypeDefinition type)
		{
			return (type.IsClass && !(type.HasFlag(TypeAttributes.Abstract) && type.HasFlag(TypeAttributes.Sealed)) && !type.IsEnum);
		}
	}
}
