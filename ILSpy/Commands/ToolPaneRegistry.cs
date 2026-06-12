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
using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Concrete tool-pane entry returned by <see cref="ToolPaneRegistry"/>. Holds the resolved
	/// pane instance together with the metadata that describes where in the layout it should
	/// land. Each pane is materialised once at registry construction so consumers see the
	/// same singleton the rest of the composition host sees (panes are <c>[Shared]</c>).
	/// </summary>
	public sealed record ToolPaneEntry(ToolPaneModel Pane, ToolPaneMetadata Metadata);

	/// <summary>
	/// MEF aggregator for <see cref="ExportToolPaneAttribute"/>-tagged tool panes. The dock
	/// factory iterates this registry to build the default layout, so adding a new pane
	/// reduces to <c>[ExportToolPane]</c> on the model with no central wiring change.
	/// </summary>
	[Export]
	[Shared]
	public sealed class ToolPaneRegistry
	{
		[ImportingConstructor]
		public ToolPaneRegistry(
			[ImportMany("ToolPane")] IEnumerable<ExportFactory<ToolPaneModel, ToolPaneMetadata>> panes)
		{
			using var _ = ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ToolPaneRegistry ctor (materialise tool panes)");
			var ordered = panes.OrderBy(p => p.Metadata.Order).ToArray();
			var entries = new ToolPaneEntry[ordered.Length];
			for (int i = 0; i < ordered.Length; i++)
			{
				var factory = ordered[i];
				var id = factory.Metadata.ContentId ?? $"#{i}";
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase($"ToolPane materialise: {id}"))
					entries[i] = new ToolPaneEntry(factory.CreateExport().Value, factory.Metadata);
			}
			Panes = entries;
			ICSharpCode.ILSpy.AppEnv.AppLog.Mark($"ToolPaneRegistry: {Panes.Count} panes resolved");
		}

		public IReadOnlyList<ToolPaneEntry> Panes { get; }
	}
}
