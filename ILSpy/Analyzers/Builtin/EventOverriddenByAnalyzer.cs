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
	/// Shows events that override an event.
	/// </summary>
	[ExportAnalyzer(Header = "Overridden By", Order = 20)]
	class EventOverriddenByAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IEvent);
			var scope = context.GetScopeOf((IEvent)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken)) {
				foreach (var result in AnalyzeType((IEvent)analyzedSymbol, type))
					yield return result;
			}
		}

		IEnumerable<IEntity> AnalyzeType(IEvent analyzedEntity, ITypeDefinition type)
		{
			var token = analyzedEntity.MetadataToken;
			var declaringTypeToken = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentModule.PEFile;
			var allTypes = type.GetAllBaseTypeDefinitions();
			if (!allTypes.Any(t => t.MetadataToken == declaringTypeToken && t.ParentModule.PEFile == module))
				yield break;

			foreach (var @event in type.Events) {
				if (!@event.IsOverride) continue;
				var baseMembers = InheritanceHelper.GetBaseMembers(@event, false);
				if (baseMembers.Any(p => p.MetadataToken == token && p.ParentModule.PEFile == module)) {
					yield return @event;
				}
			}
		}

		public bool Show(ISymbol symbol)
		{
			return symbol is IEvent entity && entity.IsOverridable && entity.DeclaringType.Kind != TypeKind.Interface;
		}
	}
}
