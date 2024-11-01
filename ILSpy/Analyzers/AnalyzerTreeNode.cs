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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Composition;

namespace ICSharpCode.ILSpy.Analyzers
{
	public abstract class AnalyzerTreeNode : SharpTreeNode
	{
		protected static Language Language => App.ExportProvider.GetExportedValue<LanguageService>().Language;

		protected static AssemblyList AssemblyList => App.ExportProvider.GetExportedValue<AssemblyList>();

		public override bool CanDelete()
		{
			return Parent is { IsRoot: true };
		}

		public override void DeleteCore()
		{
			Parent.Children.Remove(this);
		}

		public override void Delete()
		{
			DeleteCore();
		}

		public static ICollection<IExport<IAnalyzer, IAnalyzerMetadata>> Analyzers => App.ExportProvider
			.GetExports<IAnalyzer, IAnalyzerMetadata>("Analyzer")
			.OrderBy(item => item.Metadata?.Order)
			.ToArray();

		/// <summary>
		/// Handles changes to the assembly list.
		/// </summary>
		public abstract bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies);
	}
}
