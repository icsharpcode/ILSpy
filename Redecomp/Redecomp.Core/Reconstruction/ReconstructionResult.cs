// Copyright (c) 2026 Franck
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

namespace Redecomp.Core.Reconstruction;

/// <summary>Where the C# files of a reconstructed project come from.</summary>
public enum SourceOrigin
{
	/// <summary>The original files, read from the sources embedded in the PDB.</summary>
	EmbeddedSources,
	/// <summary>Decompiled from IL, using PDB symbols when they are available.</summary>
	Decompiled,
}

/// <summary>Outcome of a reconstruction.</summary>
/// <param name="Origin">Where the C# files come from.</param>
/// <param name="DebugInfo">Description of the debug information used, or null when none was found.</param>
/// <param name="ProjectFile">Path of the generated project file.</param>
public sealed record ReconstructionResult(SourceOrigin Origin, string? DebugInfo, string ProjectFile);

/// <summary>Options controlling a reconstruction.</summary>
/// <param name="DeterministicNaming">Whether locals without a name from the debug symbols are named by <see cref="Naming.DeterministicVariableNames"/>.</param>
/// <param name="IncludeContentFiles">Whether files deployed next to the assembly are copied into the project.</param>
public sealed record ReconstructionOptions(bool DeterministicNaming = true, bool IncludeContentFiles = true)
{
	public static ReconstructionOptions Default { get; } = new();
}
