// Copyright (c) 2026 Christoph Wille
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
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// A type system that resolves no corlib (ILSpy with "auto-load references" off) takes its known
/// types from MinimalCorlib, which the engine is not given: the engine would read void, object
/// and int as unknown types and hand the managed transforms ILAst that breaks their invariants.
/// </summary>
[TestFixture]
public class NoCorlibTests
{
	[Test]
	public void A_Type_System_Without_A_Corlib_Stays_Managed()
	{
		string path = typeof(NoCorlibTests).Assembly.Location;
		var file = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)), PEStreamOptions.PrefetchEntireImage);
		var typeSystem = new DecompilerTypeSystem(file, new NoReferences(), TypeSystemOptions.Default);

		Assert.That(RustEngine.For(typeSystem, out string? reason), Is.Null);
		Assert.That(reason, Does.Contain("MinimalCorlib"));
	}

	sealed class NoReferences : IAssemblyResolver
	{
		public MetadataFile? Resolve(IAssemblyReference reference) => null;
		public MetadataFile? ResolveModule(MetadataFile mainModule, string moduleName) => null;
		public Task<MetadataFile?> ResolveAsync(IAssemblyReference reference) => Task.FromResult<MetadataFile?>(null);
		public Task<MetadataFile?> ResolveModuleAsync(MetadataFile mainModule, string moduleName) => Task.FromResult<MetadataFile?>(null);
		public IDisposable? BeginSnapshot() => null;
	}
}
