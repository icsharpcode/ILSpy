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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows properties that implement an interface property.
	/// </summary>
	[ExportAnalyzer(Header = "Implemented By", Order = 10)]
	class PropertyImplementedByAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IProperty);
			var scope = context.GetScopeOf((IProperty)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken)) {
				foreach (var result in AnalyzeType((IProperty)analyzedSymbol, type))
					yield return result;
			}
		}

		IEnumerable<IEntity> AnalyzeType(IProperty analyzedEntity, ITypeDefinition type)
		{
			var token = analyzedEntity.MetadataToken;
			var declaringTypeToken = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentModule.PEFile;
			var allTypes = type.GetAllBaseTypeDefinitions();
			if (!allTypes.Any(t => t.MetadataToken == declaringTypeToken && t.ParentModule.PEFile == module))
				yield break;

			foreach (var property in type.Properties) {
				var baseMembers = InheritanceHelper.GetBaseMembers(property, true);
				if (baseMembers.Any(m => m.MetadataToken == token && m.ParentModule.PEFile == module))
					yield return property;
			}
		}

		public bool Show(ISymbol symbol)
		{
			return symbol is IProperty entity && entity.DeclaringType.Kind == TypeKind.Interface;
		}
	}
}
