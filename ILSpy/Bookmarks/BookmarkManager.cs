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
	/// bookmark's name/enabled state changes so the gutter margin can redraw. Mirrors the
	/// best-effort load/save of the dock layout sidecar.
	/// </summary>
	[Export]
	[Shared]
	public sealed class BookmarkManager
	{
		const string FileName = "ILSpy.Bookmarks.json";
		const int CurrentVersion = 1;

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
			var existing = Bookmarks.FirstOrDefault(b => b.AnchorKey == bookmark.AnchorKey);
			if (existing != null)
			{
				Bookmarks.Remove(existing);
				return false;
			}
			if (string.IsNullOrEmpty(bookmark.Name))
				bookmark.Name = NextDefaultName();
			Bookmarks.Add(bookmark);
			return true;
		}

		public void Remove(Bookmark bookmark) => Bookmarks.Remove(bookmark);

		public void Clear() => Bookmarks.Clear();

		/// <summary>Writes the current list to <paramref name="path"/> in the on-disk JSON format.</summary>
		public void Export(string path)
		{
			ArgumentNullException.ThrowIfNull(path);
			WriteTo(path);
		}

		/// <summary>
		/// Loads bookmarks from <paramref name="path"/>. In <see cref="BookmarkImportMode.Merge"/>
		/// only entries whose anchor is not already present are added; in
		/// <see cref="BookmarkImportMode.Replace"/> the current list is discarded first.
		/// </summary>
		public void Import(string path, BookmarkImportMode mode)
		{
			ArgumentNullException.ThrowIfNull(path);
			var imported = ReadFrom(path);
			if (imported.Count == 0 && mode == BookmarkImportMode.Merge)
				return;

			RunBatch(() => {
				if (mode == BookmarkImportMode.Replace)
					Bookmarks.Clear();
				var present = new HashSet<string>(Bookmarks.Select(b => b.AnchorKey));
				foreach (var b in imported)
				{
					if (present.Add(b.AnchorKey))
						Bookmarks.Add(b);
				}
			});
		}

		/// <summary>Persists the current list to the standard sidecar location.</summary>
		public void Save() => WriteTo(ConfigurationFiles.GetPath(FileName));

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

		// Applies a multi-step mutation without persisting/raising per step, then persists once.
		void RunBatch(Action action)
		{
			suppressPersist = true;
			try
			{
				action();
			}
			finally
			{
				suppressPersist = false;
			}
			PersistAndNotify();
		}

		string NextDefaultName()
		{
			// Pick the lowest unused "Bookmark{n}" so names stay stable and unsurprising.
			var used = new HashSet<string>(Bookmarks.Select(b => b.Name));
			for (int i = 0; ; i++)
			{
				var candidate = "Bookmark" + i;
				if (used.Add(candidate))
					return candidate;
			}
		}

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
			PersistAndNotify();
		}

		void OnBookmarkPropertyChanged(object? sender, PropertyChangedEventArgs e) => PersistAndNotify();

		void PersistAndNotify()
		{
			if (suppressPersist)
				return;
			Save();
			Changed?.Invoke(this, EventArgs.Empty);
		}

		void WriteTo(string path)
		{
			try
			{
				var dir = Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(dir))
					Directory.CreateDirectory(dir);
				var file = new BookmarkFile(CurrentVersion, Bookmarks.Select(BookmarkRecord.From).ToList());
				using var stream = System.IO.File.Create(path);
				JsonSerializer.Serialize(stream, file, JsonOptions);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BookmarkManager] Save failed: {ex}");
			}
		}

		static List<Bookmark> ReadFrom(string path)
		{
			if (!System.IO.File.Exists(path))
				return new List<Bookmark>();
			try
			{
				using var stream = System.IO.File.OpenRead(path);
				var file = JsonSerializer.Deserialize<BookmarkFile>(stream, JsonOptions);
				return file?.Bookmarks?
					// A bookmark must reference an assembly file. Entries without one are artifacts
					// (e.g. a stray empty row) that could never navigate; drop them defensively.
					.Where(r => !string.IsNullOrEmpty(r.FileName))
					.Select(r => r.ToBookmark()).ToList() ?? new List<Bookmark>();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BookmarkManager] Load failed: {ex}");
				return new List<Bookmark>();
			}
		}

		// On-disk shapes. Kept separate from the runtime Bookmark so the file format is explicit
		// and decoupled from the observable object.
		sealed record BookmarkFile(int Version, List<BookmarkRecord> Bookmarks);

		sealed record BookmarkRecord(
			string Name, bool Enabled, string FileName, string AssemblyFullName,
			string ModuleName, uint Token, BookmarkKind Kind, int ILOffset, string MemberName)
		{
			public static BookmarkRecord From(Bookmark b) => new(
				b.Name, b.Enabled, b.FileName, b.AssemblyFullName, b.ModuleName, b.Token, b.Kind, b.ILOffset, b.MemberName);

			public Bookmark ToBookmark() => new() {
				Name = Name,
				Enabled = Enabled,
				FileName = FileName,
				AssemblyFullName = AssemblyFullName,
				ModuleName = ModuleName,
				Token = Token,
				Kind = Kind,
				ILOffset = ILOffset,
				MemberName = MemberName,
			};
		}
	}
}
