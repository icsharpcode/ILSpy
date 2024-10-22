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
using System.Composition;
using System.Diagnostics;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>
	/// Finds all extension methods defined for a type.
	/// </summary>
	[ExportAnalyzer(Header = "Extension Methods", Order = 50)]
	[Shared]
	class TypeExtensionMethodsAnalyzer : IAnalyzer
	{
		public bool Show(ISymbol symbol) => symbol is ITypeDefinition entity && !entity.IsStatic;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is ITypeDefinition);
			var scope = context.GetScopeOf((ITypeDefinition)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in ScanType((ITypeDefinition)analyzedSymbol, type, context))
					yield return result;
			}
		}

		IEnumerable<IEntity> ScanType(ITypeDefinition analyzedType, ITypeDefinition type, AnalyzerContext context)
		{
			if (!type.HasExtensionMethods)
				yield break;

			if (analyzedType.ParentModule?.MetadataFile == null)
				yield break;

			foreach (IMethod method in type.Methods)
			{
				if (!method.IsExtensionMethod)
					continue;

				var firstParamType = method.Parameters[0].Type.GetDefinition();
				if (firstParamType != null &&
					firstParamType.MetadataToken == analyzedType.MetadataToken &&
					firstParamType.ParentModule?.MetadataFile == analyzedType.ParentModule.MetadataFile)
					yield return method;
			}
		}
	}
}
