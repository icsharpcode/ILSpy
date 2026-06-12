// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the derived types of a class — every loaded type whose direct base list contains
	/// our target. Lazy: the assembly-list scan only runs when the user expands the node. The
	/// scan is synchronous; small assembly lists complete in milliseconds. A future task can
	/// switch to a background-loaded variant when this proves slow under real workloads.
	/// </summary>
	public sealed class DerivedTypesTreeNode : ILSpyTreeNode
	{
		readonly AssemblyList list;
		readonly ITypeDefinition type;

		public DerivedTypesTreeNode(AssemblyList list, ITypeDefinition type)
		{
			this.list = list;
			this.type = type;
			LazyLoading = true;
		}

		public override object Text => ICSharpCode.ILSpy.Properties.Resources.DerivedTypes;

		public override object? NavigationText => $"{Text} ({Language.TypeToString(type)})";

		public override object Icon => Images.SubTypes;

		protected override void LoadChildren()
		{
			foreach (var entry in FindDerivedTypes(list, LanguageService.CurrentLanguage, type, CancellationToken.None))
				Children.Add(entry);
		}

		internal static IEnumerable<DerivedTypesEntryNode> FindDerivedTypes(AssemblyList list, Language language, ITypeDefinition type, CancellationToken cancellationToken)
		{
			var context = new AnalyzerContext {
				CancellationToken = cancellationToken,
				Language = language,
				AssemblyList = list,
			};
			var scope = context.GetScopeOf(type);

			foreach (var td in scope.GetTypesInScope(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				foreach (var baseType in td.DirectBaseTypes)
				{
					if (baseType.FullName == type.FullName && baseType.TypeParameterCount == type.TypeParameterCount)
					{
						yield return new DerivedTypesEntryNode(list, td);
						break;
					}
				}
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			foreach (var child in Children)
			{
				if (child is ILSpyTreeNode node)
					node.Decompile(language, output, options);
			}
		}
	}
}
