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
using System.IO;
using System.Threading.Tasks;

using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Platform.Storage;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Helpers around <see cref="IStorageProvider"/> for save dialogs. Mirrors the WPF
	/// SaveFileDialog Filter API ("Display|*.ext;*.ext|...") so callers can keep the existing
	/// strings.
	/// </summary>
	public static class FilePickers
	{
		/// <summary>
		/// Shows a save-file picker. <paramref name="filter"/> uses WPF SaveFileDialog syntax —
		/// pairs of <c>display|patterns</c> joined by <c>|</c>. <paramref name="defaultFileName"/>
		/// pre-fills the file name (no path). Returns the selected absolute path, or <c>null</c>
		/// if the user cancelled.
		/// </summary>
		public static async Task<string?> SaveAsync(
			string filter,
			string? defaultFileName = null,
			string? title = null)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return null;

			var fileTypes = ParseFilter(filter);
			var suggested = defaultFileName != null
				? Path.GetFileName(defaultFileName)
				: null;

			var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
				Title = title,
				FileTypeChoices = fileTypes,
				SuggestedFileName = suggested,
				DefaultExtension = fileTypes.Count > 0 ? GuessExtension(fileTypes[0]) : null,
			});

			return file?.TryGetLocalPath();
		}

		/// <summary>
		/// Shows a folder-picker dialog. <paramref name="title"/> appears in the dialog
		/// chrome. Returns the selected folder's absolute path, or <c>null</c> if the user
		/// cancelled or the storage provider refused to give a local path (e.g. cloud
		/// folder).
		/// </summary>
		public static async Task<string?> PickFolderAsync(string? title = null)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return null;

			var folders = await owner.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {
				Title = title,
				AllowMultiple = false,
			});
			if (folders.Count == 0)
				return null;
			return folders[0].TryGetLocalPath();
		}

		/// <summary>"PNG (*.png)|*.png|All files|*.*" → two file types.</summary>
		internal static IReadOnlyList<FilePickerFileType> ParseFilter(string filter)
		{
			var result = new List<FilePickerFileType>();
			var parts = filter.Split('|');
			for (int i = 0; i + 1 < parts.Length; i += 2)
			{
				var display = parts[i];
				var patterns = parts[i + 1].Split(';');
				result.Add(new FilePickerFileType(display) { Patterns = patterns });
			}
			return result;
		}

		static string? GuessExtension(FilePickerFileType type)
		{
			if (type.Patterns is null)
				return null;
			foreach (var p in type.Patterns)
			{
				// "*.ext" → "ext"; bare "*" or "*.*" yields no useful extension.
				if (p.StartsWith("*.", System.StringComparison.Ordinal) && p.Length > 2 && p[2] != '*')
					return p[2..];
			}
			return null;
		}
	}
}
