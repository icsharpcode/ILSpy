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

using System.Text;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// One row in the GAC browser. Wraps an <see cref="AssemblyNameReference"/> + the
	/// resolved on-disk path, exposing each WPF column's bound property (ShortName,
	/// Version, Culture, PublicKeyToken, FileName) so the DataGrid can sort by any of
	/// them independently. <see cref="FullName"/> + <see cref="FormattedVersion"/> back
	/// the filter, mirroring the WPF dialog.
	/// </summary>
	public sealed class GacEntry
	{
		readonly AssemblyNameReference reference;
		string? formattedVersion;
		string? publicKeyToken;

		public GacEntry(AssemblyNameReference reference, string fileName)
		{
			this.reference = reference;
			FileName = fileName;
		}

		public string FileName { get; }

		public string FullName => reference.FullName;

		public string ShortName => reference.Name;

		public System.Version? Version => reference.Version;

		public string FormattedVersion => formattedVersion ??= (Version?.ToString() ?? string.Empty);

		public string Culture
			=> string.IsNullOrEmpty(reference.Culture) ? "neutral" : reference.Culture!;

		public string PublicKeyToken => publicKeyToken ??= FormatPublicKeyToken(reference.PublicKeyToken);

		static string FormatPublicKeyToken(byte[]? token)
		{
			if (token == null || token.Length == 0)
				return "null";
			var sb = new StringBuilder(token.Length * 2);
			foreach (var b in token)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		public override string ToString() => FullName;
	}
}
