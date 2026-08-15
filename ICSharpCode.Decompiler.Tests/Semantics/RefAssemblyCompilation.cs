// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	/// <summary>
	/// A compilation over a .NET reference assembly plus the test assembly, shared by all tests
	/// whose subject types postdate the legacy mscorlib the main test compilation resolves
	/// against - System.ValueTuple and Span&lt;T&gt;/ReadOnlySpan&lt;T&gt;. The reference assembly
	/// is read once for the whole test run.
	/// </summary>
	static class RefAssemblyCompilation
	{
		public static ICompilation Instance => instance.Value;

		static readonly Lazy<ICompilation> instance = new Lazy<ICompilation>(
			delegate {
				string path = Path.Combine(
					Tester.RefAssembliesToolset.GetPath(".NETCoreApp,Version=v5.0"), "System.Runtime.dll");
				return new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
					new PEFile(path, new FileStream(path, FileMode.Open, FileAccess.Read)));
			});
	}
}
