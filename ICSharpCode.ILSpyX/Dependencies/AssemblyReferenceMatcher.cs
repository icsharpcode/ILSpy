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

using System;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX.Dependencies
{
	/// <summary>
	/// Decides whether an assembly reference denotes a given assembly definition by its identity:
	/// the simple name (case-insensitive) and the public key token. The version is deliberately not
	/// part of the identity -- binding redirects and unified frameworks routinely satisfy a
	/// reference with a different version -- callers report a version mismatch separately.
	/// </summary>
	public static class AssemblyReferenceMatcher
	{
		/// <summary>The token string used when an assembly has no public key.</summary>
		public const string NullToken = "null";

		/// <summary>
		/// The public key token of an assembly reference as lower-case hex, or <see cref="NullToken"/>.
		/// </summary>
		public static string GetPublicKeyToken(IAssemblyReference reference)
		{
			ArgumentNullException.ThrowIfNull(reference);
			var token = reference.PublicKeyToken;
			if (token == null || token.Length == 0)
				return NullToken;
			return Convert.ToHexString(token).ToLowerInvariant();
		}

		/// <summary>
		/// The public key token of the assembly defined by <paramref name="metadata"/> as lower-case
		/// hex, <see cref="NullToken"/> when it has no public key, or <see langword="null"/> when the
		/// metadata does not define an assembly (a netmodule).
		/// </summary>
		public static string? GetPublicKeyToken(MetadataReader metadata)
		{
			ArgumentNullException.ThrowIfNull(metadata);
			if (!metadata.IsAssembly)
				return null;
			return metadata.GetPublicKeyToken().ToLowerInvariant();
		}

		/// <summary>The simple name of the assembly defined by <paramref name="metadata"/>, or null for a netmodule.</summary>
		public static string? GetAssemblyName(MetadataReader metadata)
		{
			ArgumentNullException.ThrowIfNull(metadata);
			if (!metadata.IsAssembly)
				return null;
			return metadata.GetString(metadata.GetAssemblyDefinition().Name);
		}

		/// <summary>The version of the assembly defined by <paramref name="metadata"/>, or null for a netmodule.</summary>
		public static Version? GetAssemblyVersion(MetadataReader metadata)
		{
			ArgumentNullException.ThrowIfNull(metadata);
			if (!metadata.IsAssembly)
				return null;
			return metadata.GetAssemblyDefinition().Version;
		}

		/// <summary>
		/// True when <paramref name="reference"/> names the assembly defined by <paramref name="target"/>:
		/// same simple name (ignoring case) and same public key token.
		/// </summary>
		public static bool Matches(IAssemblyReference reference, MetadataFile target)
		{
			ArgumentNullException.ThrowIfNull(reference);
			ArgumentNullException.ThrowIfNull(target);
			var metadata = target.Metadata;
			var name = GetAssemblyName(metadata);
			if (name == null || !string.Equals(reference.Name, name, StringComparison.OrdinalIgnoreCase))
				return false;
			return string.Equals(GetPublicKeyToken(reference), GetPublicKeyToken(metadata), StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// True when <paramref name="reference"/> asks for a different version than the one
		/// <paramref name="target"/> defines. Unknown versions never count as a mismatch.
		/// </summary>
		public static bool IsVersionMismatch(IAssemblyReference reference, MetadataFile target)
		{
			ArgumentNullException.ThrowIfNull(reference);
			ArgumentNullException.ThrowIfNull(target);
			var defined = GetAssemblyVersion(target.Metadata);
			return reference.Version != null && defined != null && reference.Version != defined;
		}
	}
}
