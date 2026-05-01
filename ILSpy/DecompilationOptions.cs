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

using System.Threading;

using ICSharpCode.Decompiler;

namespace ILSpy
{
	/// <summary>
	/// Options passed to <see cref="Languages.Language"/>'s Decompile* methods.
	/// Stripped-down compared to the WPF host: no project export, no display-settings plumbing,
	/// no view-state restoration — just enough to drive a single decompilation into a text view.
	/// </summary>
	public sealed class DecompilationOptions
	{
		public DecompilerSettings DecompilerSettings { get; }
		public CancellationToken CancellationToken { get; set; }

		/// <summary>Mirrors WPF: full module decompilation rather than just the selected member.</summary>
		public bool FullDecompilation { get; set; }

		/// <summary>Mirrors WPF: target directory for project export. Always null in the
		/// Avalonia host today (no save dialog wired up); kept for signature parity.</summary>
		public string? SaveAsProjectDirectory { get; set; }

		/// <summary>Mirrors WPF: escape invalid C# identifiers so the output compiles.</summary>
		public bool EscapeInvalidIdentifiers { get; set; }

		public DecompilationOptions(DecompilerSettings settings)
		{
			DecompilerSettings = settings;
		}

		public DecompilationOptions() : this(new DecompilerSettings()) { }
	}
}
