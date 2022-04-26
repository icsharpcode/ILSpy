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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows members from all corresponding interfaces the selected member implements.
	/// </summary>
	[ExportAnalyzer(Header = "Implements", Order = 40)]
	class MemberImplementsInterfaceAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IMember);
			var member = (IMember)analyzedSymbol;

			Debug.Assert(!member.IsStatic);

			var baseMembers = InheritanceHelper.GetBaseMembers(member, includeImplementedInterfaces: true);
			return baseMembers.Where(m => m.DeclaringTypeDefinition.Kind == TypeKind.Interface);
		}

		public bool Show(ISymbol symbol)
		{
			switch (symbol?.SymbolKind)
			{
				case SymbolKind.Event:
				case SymbolKind.Indexer:
				case SymbolKind.Method:
				case SymbolKind.Property:
					var member = (IMember)symbol;
					var type = member.DeclaringTypeDefinition;
					return !member.IsStatic && type is not null && (type.Kind == TypeKind.Class || type.Kind == TypeKind.Struct);

				default:
					return false;
			}
		}
	}
}
