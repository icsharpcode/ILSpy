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

using System.IO;
using System.Reflection.Metadata.Ecma335;

using AvaloniaEdit.Document;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// Turns a clicked line in the decompiled C# view into a <see cref="Bookmark"/> anchor: a line
	/// that starts a statement becomes an IL-offset body anchor; a definition line becomes a token
	/// anchor; other displayed lines fall back to their current visible line number.
	/// </summary>
	public static class BookmarkAnchoring
	{
		/// <summary>
		/// Builds an unnamed candidate bookmark for <paramref name="line"/>, or null when the line
		/// is outside the current document or no containing entity can be identified.
		/// </summary>
		public static Bookmark? CreateForLine(DecompiledDebugInfo? debugInfo,
			TextSegmentCollection<ReferenceSegment>? references, TextDocument document, int line,
			IEntity? fallbackOwner = null, string? locationNodeName = null)
		{
			if (document == null || line < 1 || line > document.LineCount)
				return null;

			if (debugInfo != null && debugInfo.TryGetBodyAnchor(line, out var method, out var ilOffset))
			{
				return new Bookmark {
					Kind = BookmarkKind.Body,
					Token = method.Token,
					ILOffset = ilOffset,
					LineNumber = line,
					FileName = method.FileName,
					AssemblyFullName = method.AssemblyFullName,
					ModuleName = method.ModuleName,
					MemberName = method.MemberName,
					LocationNodeName = locationNodeName,
				};
			}

			if (references != null)
			{
				var docLine = document.GetLineByNumber(line);
				foreach (var segment in references.FindOverlappingSegments(docLine.Offset, docLine.Length))
				{
					if (segment.IsDefinition && segment.Reference is IEntity entity && CreateForEntity(entity, line, locationNodeName) is { } bookmark)
						return bookmark;
				}
			}

			return CreateForFallbackLine(fallbackOwner ?? FindFirstEntity(references), line, locationNodeName);
		}

		static IEntity? FindFirstEntity(TextSegmentCollection<ReferenceSegment>? references)
		{
			if (references == null)
				return null;
			foreach (var segment in references)
			{
				if (segment.Reference is IEntity entity)
					return entity;
			}
			return null;
		}

		static Bookmark? CreateForFallbackLine(IEntity? entity, int line, string? locationNodeName)
		{
			if (entity == null || CreateForEntity(entity, line, locationNodeName) is not { } bookmark)
				return null;
			return new Bookmark {
				Kind = BookmarkKind.Line,
				Token = bookmark.Token,
				LineNumber = line,
				FileName = bookmark.FileName,
				AssemblyFullName = bookmark.AssemblyFullName,
				ModuleName = bookmark.ModuleName,
				MemberName = bookmark.MemberName,
				LocationNodeName = bookmark.LocationNodeName,
			};
		}

		static Bookmark? CreateForEntity(IEntity entity, int line, string? locationNodeName)
		{
			var file = entity.ParentModule?.MetadataFile;
			if (file == null)
				return null;
			string moduleName = string.IsNullOrEmpty(file.FileName) ? file.Name : Path.GetFileName(file.FileName);
			return new Bookmark {
				Kind = BookmarkKind.Token,
				Token = (uint)MetadataTokens.GetToken(entity.MetadataToken),
				LineNumber = line,
				FileName = file.FileName,
				AssemblyFullName = file.FullName,
				ModuleName = moduleName,
				MemberName = entity.FullName,
				LocationNodeName = locationNodeName,
			};
		}
	}
}
