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

using Avalonia.Controls;

using Dock.Controls.DeferredContentControl;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;

using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Base for our docked tool panes (assembly tree, search results, analyzers, …). Opts
	/// out of Dock.Avalonia's deferred-content presentation: by default Dock waits for the
	/// layout to settle before instantiating each tool pane's view, which on startup costs
	/// hundreds of milliseconds between the window painting and the panes appearing. The
	/// trees + search results are cheap to materialise (lazy loading handles deeper levels)
	/// so eager realisation is the right tradeoff — same as <c>ContentTabPage</c>.
	///
	/// The ctor also pre-populates <see cref="Tool.DockCapabilityOverrides"/> with a
	/// default instance because the Dock theme template binds against
	/// <c>ActiveDockable.DockCapabilityOverrides.CanPin</c> / <c>CanClose</c> (and similar)
	/// the moment the pane chrome is realised. Without a non-null instance every pane
	/// startup logs a <c>[Binding]</c> error per property — adds up to ~30 errors per
	/// launch.
	/// </summary>
	public abstract class ToolPaneModel : Tool, IDeferredContentPresentation, IDockableViewOwner
	{
		protected ToolPaneModel()
		{
			DockCapabilityOverrides = new DockCapabilityOverrides();
		}

		bool IDeferredContentPresentation.DeferContentPresentation => false;

		public Control? OwnedView { get; set; }
	}
}
