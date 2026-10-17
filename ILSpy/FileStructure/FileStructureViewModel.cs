// Copyright (c) 2026 Piero Viano
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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Composition;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// The File Structure tool pane: the types and members of the active decompiler document in
	/// document order. Follows the active document tab and refreshes whenever that document's text
	/// is replaced (a new decompile, a language switch, ...). Activating a row moves the editor's
	/// caret to the member's definition.
	/// </summary>
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Right, Order = 0, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class FileStructureViewModel : ToolPaneModel
	{
		public const string PaneContentId = "FileStructure";

		// The workspace is resolved lazily: it is built from the tool-pane registry, which builds
		// this pane, so it cannot be a constructor dependency.
		DockWorkspace? workspace;
		ContentTabPage? trackedTab;

		[ObservableProperty]
		private DecompilerTabPageModel? document;

		[ObservableProperty]
		private FileMemberEntry? selectedEntry;

		public FileStructureViewModel()
		{
			Id = PaneContentId;
			Title = Resources.FileStructure;
		}

		/// <summary>The outline of <see cref="Document"/>.</summary>
		public ObservableCollection<FileMemberEntry> Entries { get; } = new();

		/// <summary>
		/// Starts following the active document (idempotent) and refreshes the outline. Called when
		/// the pane is shown or its view attaches.
		/// </summary>
		public void Attach()
		{
			if (workspace == null)
			{
				workspace = AppComposition.TryGetExport<DockWorkspace>();
				if (workspace != null)
					workspace.PropertyChanged += OnWorkspacePropertyChanged;
			}
			TrackActiveDocument();
		}

		/// <summary>Moves the editor caret to <paramref name="entry"/>'s definition.</summary>
		public bool Activate(FileMemberEntry? entry)
		{
			if (entry == null || Document?.NavigateToOffset is not { } navigate)
				return false;
			SelectedEntry = entry;
			navigate(entry.Offset);
			return true;
		}

		void OnWorkspacePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DockWorkspace.ActiveContentTabPage))
				TrackActiveDocument();
		}

		void OnTrackedTabPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// The reusable preview tab swaps its content between decompiler output and other pages.
			if (e.PropertyName == nameof(ContentTabPage.Content))
				TrackActiveDocument();
		}

		void TrackActiveDocument()
		{
			var tab = workspace?.ActiveContentTabPage;
			if (!ReferenceEquals(tab, trackedTab))
			{
				if (trackedTab != null)
					trackedTab.PropertyChanged -= OnTrackedTabPropertyChanged;
				trackedTab = tab;
				if (trackedTab != null)
					trackedTab.PropertyChanged += OnTrackedTabPropertyChanged;
			}
			Document = ActiveDocument.Get();
			Rebuild();
		}

		partial void OnDocumentChanged(DecompilerTabPageModel? oldValue, DecompilerTabPageModel? newValue)
		{
			if (oldValue != null)
				oldValue.PropertyChanged -= OnDocumentPropertyChanged;
			if (newValue != null)
				newValue.PropertyChanged += OnDocumentPropertyChanged;
		}

		void OnDocumentPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Text is assigned after the references it is described by, so it marks a complete update.
			if (e.PropertyName == nameof(DecompilerTabPageModel.Text))
				Rebuild();
		}

		void Rebuild()
		{
			Entries.Clear();
			foreach (var entry in FileMemberCollector.Collect(Document))
				Entries.Add(entry);
			SelectedEntry = null;
			OnPropertyChanged(nameof(HasNoEntries));
		}

		/// <summary>True when the outline has no rows.</summary>
		public bool HasNoEntries => Entries.Count == 0;
	}
}
