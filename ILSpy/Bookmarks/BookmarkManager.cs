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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>What to do when importing into a non-empty list.</summary>
	public enum BookmarkImportMode
	{
		/// <summary>Discard the current list and replace it with the imported one.</summary>
		Replace,

		/// <summary>Keep the current list, adding only imported bookmarks not already present.</summary>
		Merge
	}

	/// <summary>
	/// The single source of truth for the flat bookmark list. Holds the live
	/// <see cref="ObservableCollection{Bookmark}"/>, persists it to <c>ILSpy.Bookmarks.json</c>
	/// next to <c>ILSpy.xml</c>, and raises <see cref="Changed"/> whenever the list or any
	/// bookmark's name/enabled state changes so the gutter margin can redraw. Updates reload the
	/// JSON file under a mutex so multiple ILSpy instances do not overwrite unrelated edits.
	/// </summary>
	[Export]
	[Shared]
	public sealed class BookmarkManager
	{
		const string FileName = "ILSpy.Bookmarks.json";
		const int CurrentVersion = 1;
		const string BookmarksFileMutex = "81FC41D7-A7FA-4386-B8DE-E75BA5355A35";

		static readonly JsonSerializerOptions JsonOptions = new() {
			WriteIndented = true,
			Converters = { new JsonStringEnumConverter() },
		};

		bool suppressPersist;

		public BookmarkManager()
		{
			Bookmarks.CollectionChanged += OnCollectionChanged;
			Load();
		}

		/// <summary>The live list, bound directly by the bookmarks pane.</summary>
		public ObservableCollection<Bookmark> Bookmarks { get; } = new();

		/// <summary>Raised after any structural change or any bookmark name/enabled edit.</summary>
		public event EventHandler? Changed;

		/// <summary>
		/// Adds <paramref name="bookmark"/> when no bookmark with the same anchor exists, or removes
		/// the existing one when it does. Returns true if a bookmark was added, false if removed.
		/// Assigns a default name when the incoming bookmark has none.
		/// </summary>
		public bool Toggle(Bookmark bookmark)
		{
			ArgumentNullException.ThrowIfNull(bookmark);
			bool added = false;
			UpdateSavedBookmarks(bookmarks => {
				var existing = bookmarks.FirstOrDefault(b => b.AnchorKey == bookmark.AnchorKey);
				if (existing != null)
				{
					bookmarks.Remove(existing);
					return;
				}
				if (string.IsNullOrEmpty(bookmark.Name))
					bookmark.Name = NextDefaultName(bookmarks);
				bookmarks.Add(bookmark);
				added = true;
			});
			return added;
		}

		public void Remove(Bookmark bookmark)
		{
			ArgumentNullException.ThrowIfNull(bookmark);
			UpdateSavedBookmarks(bookmarks => bookmarks.RemoveAll(b => b.AnchorKey == bookmark.AnchorKey));
		}

		public void Clear() => UpdateSavedBookmarks(bookmarks => bookmarks.Clear());

		/// <summary>Writes the current list to <paramref name="path"/> in the on-disk JSON format.</summary>
		public void Export(string path)
		{
			ArgumentNullException.ThrowIfNull(path);
			WriteTo(path);
		}

		/// <summary>
		/// Loads bookmarks from <paramref name="path"/>. In <see cref="BookmarkImportMode.Merge"/>
		/// only entries whose anchor is not already present are added; in
		/// <see cref="BookmarkImportMode.Replace"/> the current list is discarded first. Returns
		/// <c>false</c> when the file cannot be read or parsed, leaving the current list untouched --
		/// in particular so a corrupt file picked for a Replace import does not wipe the bookmarks.
		/// A valid but empty file is a success and, in Replace mode, legitimately clears the list.
		/// </summary>
		public bool Import(string path, BookmarkImportMode mode)
		{
			ArgumentNullException.ThrowIfNull(path);
			if (!TryReadFrom(path, out var imported))
				return false;
			if (imported.Count == 0 && mode == BookmarkImportMode.Merge)
				return true;

			UpdateSavedBookmarks(bookmarks => {
				if (mode == BookmarkImportMode.Replace)
					bookmarks.Clear();
				var present = new HashSet<string>(bookmarks.Select(b => b.AnchorKey));
				foreach (var b in imported)
				{
					if (present.Add(b.AnchorKey))
						bookmarks.Add(b);
				}
			});
			return true;
		}

		void Load()
		{
			var loaded = ReadFrom(ConfigurationFiles.GetPath(FileName));
			// Suppress persistence during the initial load: reading the list back is not a change,
			// and we don't want to rewrite (or create) the file just by starting the app.
			suppressPersist = true;
			try
			{
				foreach (var b in loaded)
					Bookmarks.Add(b);
			}
			finally
			{
				suppressPersist = false;
			}
		}

		// Reconciles the observable collection with the freshly saved list, in place and without
		// treating the changes as user edits. Entries already present (matched by AnchorKey) keep their
		// existing instance -- so the bookmarks pane's live selection and the gutter's references stay
		// valid -- and only their editable Name/Enabled are refreshed; entries gone from the saved list
		// are removed and new ones appended. Rebuilding via Clear()+Add() instead would replace every
		// instance and drop the pane's selection on each toggle or edit.
		void ReplaceLiveBookmarks(List<Bookmark> bookmarks)
		{
			suppressPersist = true;
			try
			{
				var live = new Dictionary<string, Bookmark>();
				foreach (var bookmark in Bookmarks)
					live[bookmark.AnchorKey] = bookmark;

				var savedKeys = new HashSet<string>(bookmarks.Select(b => b.AnchorKey));
				for (int i = Bookmarks.Count - 1; i >= 0; i--)
				{
					if (!savedKeys.Contains(Bookmarks[i].AnchorKey))
						Bookmarks.RemoveAt(i);
				}

				foreach (var saved in bookmarks)
				{
					if (live.TryGetValue(saved.AnchorKey, out var existing))
					{
						// Setters are equality-checked, so assigning an unchanged value is a no-op.
						existing.Name = saved.Name;
						existing.Enabled = saved.Enabled;
					}
					else
					{
						Bookmarks.Add(saved);
					}
				}
			}
			finally
			{
				suppressPersist = false;
			}
		}

		string NextDefaultName() => NextDefaultName(Bookmarks);

		static string NextDefaultName(IEnumerable<Bookmark> bookmarks)
		{
			// Pick the lowest unused "Bookmark{n}" so names stay stable and unsurprising.
			var used = new HashSet<string>(bookmarks.Select(b => b.Name));
			for (int i = 0; ; i++)
			{
				var candidate = "Bookmark" + i;
				if (used.Add(candidate))
					return candidate;
			}
		}

		// The collection is only ever mutated under suppressPersist (the initial load and the
		// post-write reconcile in ReplaceLiveBookmarks); persistence and the Changed event are owned
		// by UpdateSavedBookmarks. This handler's sole job is to keep each bookmark's name/enabled
		// edit subscription in step with the live list.
		void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (Bookmark b in e.OldItems)
					b.PropertyChanged -= OnBookmarkPropertyChanged;
			}
			if (e.NewItems != null)
			{
				foreach (Bookmark b in e.NewItems)
					b.PropertyChanged += OnBookmarkPropertyChanged;
			}
		}

		void OnBookmarkPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is Bookmark bookmark
				&& (e.PropertyName == nameof(Bookmark.Name) || e.PropertyName == nameof(Bookmark.Enabled)))
				PersistBookmarkEdit(bookmark);
		}

		void PersistBookmarkEdit(Bookmark bookmark)
		{
			if (suppressPersist)
				return;
			UpdateSavedBookmarks(bookmarks => {
				int index = bookmarks.FindIndex(b => b.AnchorKey == bookmark.AnchorKey);
				if (index >= 0)
					bookmarks[index] = bookmark;
				else
					bookmarks.Add(bookmark);
			});
		}

		void UpdateSavedBookmarks(Action<List<Bookmark>> update)
		{
			ArgumentNullException.ThrowIfNull(update);
			try
			{
				var path = ConfigurationFiles.GetPath(FileName);
				List<Bookmark> bookmarks;
				using (new MutexProtector(BookmarksFileMutex))
				{
					bookmarks = ReadFrom(path);
					update(bookmarks);
					WriteListTo(path, bookmarks);
				}
				ReplaceLiveBookmarks(bookmarks);
				Changed?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BookmarkManager] Update failed: {ex}");
			}
		}

		void WriteTo(string path)
		{
			try
			{
				WriteListTo(path, Bookmarks);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BookmarkManager] Save failed: {ex}");
			}
		}

		static void WriteListTo(string path, IEnumerable<Bookmark> bookmarks)
		{
			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir))
				Directory.CreateDirectory(dir);
			var file = new BookmarkFile(CurrentVersion, bookmarks.Select(BookmarkRecord.From).ToList());
			using var stream = System.IO.File.Create(path);
			JsonSerializer.Serialize(stream, file, JsonOptions);
		}

		// Tolerant read for the sidecar load path: an empty list whether the file is absent, empty, or
		// unreadable. Import goes through TryReadFrom instead, to tell a valid empty file from a failure.
		static List<Bookmark> ReadFrom(string path)
		{
			TryReadFrom(path, out var bookmarks);
			return bookmarks;
		}

		// Reads and parses the bookmark file. Returns false (with an empty list) when the file is
		// missing or cannot be deserialized, so callers that must not destroy data on a bad file --
		// notably a Replace import -- can bail. A valid file (including an empty one) returns true.
		static bool TryReadFrom(string path, out List<Bookmark> bookmarks)
		{
			bookmarks = new List<Bookmark>();
			if (!System.IO.File.Exists(path))
				return false;
			try
			{
				using var stream = System.IO.File.OpenRead(path);
				var file = JsonSerializer.Deserialize<BookmarkFile>(stream, JsonOptions);
				bookmarks = file?.Bookmarks?
					// A bookmark must reference an assembly file. Entries without one are artifacts
					// (e.g. a stray empty row) that could never navigate; drop them defensively.
					.Where(r => !string.IsNullOrEmpty(r.FileName))
					.Select(r => r.ToBookmark()).ToList() ?? new List<Bookmark>();
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BookmarkManager] Load failed: {ex}");
				bookmarks = new List<Bookmark>();
				return false;
			}
		}

		// On-disk shapes. Kept separate from the runtime Bookmark so the file format is explicit
		// and decoupled from the observable object.
		sealed record BookmarkFile(int Version, List<BookmarkRecord> Bookmarks);

		sealed record BookmarkRecord(
			string Name, bool Enabled, string FileName, string AssemblyFullName,
			string ModuleName, uint Token, BookmarkKind Kind, int ILOffset, int LineNumber,
			string MemberName, string? LocationNodeName, BookmarkViewState? ViewState)
		{
			public static BookmarkRecord From(Bookmark b) => new(
				b.Name, b.Enabled, b.FileName, b.AssemblyFullName, b.ModuleName, b.Token, b.Kind,
				b.ILOffset, b.LineNumber, b.MemberName, b.LocationNodeName, b.ViewState);

			public Bookmark ToBookmark() => new() {
				Name = Name,
				Enabled = Enabled,
				FileName = FileName,
				AssemblyFullName = AssemblyFullName,
				ModuleName = ModuleName,
				Token = Token,
				Kind = Kind,
				ILOffset = ILOffset,
				LineNumber = LineNumber,
				MemberName = MemberName,
				LocationNodeName = LocationNodeName,
				ViewState = ViewState,
			};
		}

		sealed class MutexProtector : IDisposable
		{
			readonly Mutex mutex;

			public MutexProtector(string name)
			{
				mutex = new Mutex(true, name, out bool createdNew);
				if (createdNew)
					return;

				try
				{
					mutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
				}
			}

			public void Dispose()
			{
				mutex.ReleaseMutex();
				mutex.Dispose();
			}
		}
	}
}
