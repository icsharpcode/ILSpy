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

using System;
using System.Collections.Generic;
using System.Composition;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;

using ILSpy.Analyzers;
using ILSpy.AssemblyTree;
using ILSpy.Search;
using ILSpy.ViewModels;

namespace ILSpy.Docking
{
	[Export]
	[Shared]
	public class DockWorkspace
	{
		readonly ILSpyDockFactory factory;

		public IFactory Factory => factory;

		public IRootDock Layout { get; }

		public IReadOnlyList<ToolPaneMenuItem> ToolPaneMenuItems { get; }

		[ImportingConstructor]
		public DockWorkspace(
			AssemblyTreeModel assemblyTreeModel,
			SearchPaneModel searchPaneModel,
			AnalyzerTreeViewModel analyzerTreeViewModel)
		{
			factory = new ILSpyDockFactory(assemblyTreeModel, searchPaneModel, analyzerTreeViewModel);
			Layout = factory.CreateLayout();
			// Layout/factory initialization (locators, parent/factory wiring) is done by
			// the DockControl in MainWindow.axaml via InitializeFactory/InitializeLayout.

			ToolPaneMenuItems = new List<ToolPaneMenuItem> {
				new(assemblyTreeModel, factory),
				new(searchPaneModel, factory),
				new(analyzerTreeViewModel, factory),
			};

			factory.DockableAdded += OnDocumentMembershipChanged;
			factory.DockableRemoved += OnDocumentMembershipChanged;
			factory.DockableClosed += OnDocumentMembershipChanged;
			factory.DockableClosing += OnDockableClosing;

			// TODO: layout persistence (load on startup, save on exit). DockSerializer.SystemTextJson
			// trips System.Text.Json's MaxDepth=64 limit on our layout because Dock's JsonConverterList<T>
			// calls JsonSerializer.Serialize per list element, which doesn't preserve the writer's
			// reference-tracking state between calls — so Owner/Factory back-refs aren't deduplicated as
			// $ref. Options when revisiting: try Dock.Serializer.Newtonsoft (better cycle handling),
			// or build a custom JsonSerializerOptions with MaxDepth raised + replicate Dock's internal
			// polymorphic resolver.
		}

		void OnDocumentMembershipChanged(object? sender, EventArgs e) => UpdateLastDocumentCanClose();

		// Convert a tool pane's "close" (X button) into "hide" so the Window menu can restore it.
		// Documents are still closed for real — they get garbage-collected.
		void OnDockableClosing(object? sender, DockableClosingEventArgs e)
		{
			if (e.Dockable is ToolPaneModel toolPane)
			{
				e.Cancel = true;
				factory.HideDockable(toolPane);
			}
		}

		// When only one document is open, make it un-closeable so the user can't end up with an
		// empty document area.
		void UpdateLastDocumentCanClose()
		{
			var docs = factory.Documents?.VisibleDockables;
			if (docs == null)
				return;
			bool canClose = docs.Count > 1;
			foreach (var d in docs)
			{
				d.CanClose = canClose;
			}
		}

		public TabPageModel OpenNewTab(TabPageModel tab)
		{
			if (factory.Documents != null)
			{
				factory.AddDockable(factory.Documents, tab);
				factory.SetActiveDockable(tab);
				factory.SetFocusedDockable(factory.Documents, tab);
			}
			return tab;
		}
	}
}
