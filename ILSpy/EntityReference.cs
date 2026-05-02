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

using System.Diagnostics;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

namespace ILSpy
{
	/// <summary>
	/// Unresolved metadata reference — a (module, handle) pair plus a protocol that says how
	/// to interpret the target (e.g. "decompile" jumps to the declaration in C# view).
	/// </summary>
	[DebuggerDisplay("EntityReference Module={Module}, Handle={Handle}, Protocol={Protocol}")]
	public sealed class EntityReference
	{
		readonly MetadataFile? metadataFile;
		public string Module { get; }
		public Handle Handle { get; }
		public string Protocol { get; }

		public EntityReference(string moduleFileName, Handle handle, string protocol = "decompile")
		{
			Module = moduleFileName;
			Handle = handle;
			Protocol = protocol;
		}

		public EntityReference(MetadataFile module, Handle handle, string protocol = "decompile")
		{
			metadataFile = module;
			Module = module.FileName;
			Handle = handle;
			Protocol = protocol;
		}

		public MetadataFile? ResolveAssembly(AssemblyList context)
			=> metadataFile ?? context.FindAssembly(Module)?.GetMetadataFileOrNull();
	}
}
