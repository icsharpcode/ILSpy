// Copyright (c) 2018 Siegfried Pammer
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
using System.Diagnostics;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy
{
	[DebuggerDisplay("EntityReference Module={Module}, Handle={Handle}, Protocol={Protocol}")]
	public class EntityReference
	{
		public string Module { get; }
		public Handle Handle { get; }
		public string Protocol { get; }

		public EntityReference(string moduleFileName, Handle handle)
		{
			this.Module = moduleFileName;
			this.Handle = handle;
			this.Protocol = "decompile";
		}

		public EntityReference(string protocol, string moduleFileName, Handle handle)
			: this(moduleFileName, handle)
		{
			this.Protocol = protocol ?? "decompile";
		}

		public PEFile ResolveAssembly(AssemblyList context)
		{
			return context.FindAssembly(Module)?.GetPEFileOrNull();
		}
	}
}
