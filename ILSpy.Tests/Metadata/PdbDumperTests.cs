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

using System.IO;

using ICSharpCode.ILSpyX.Metadata;

using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// Cecil's own symbol discovery (ReadSymbols without a provider) locates the Windows PDB reader
/// by type name through Type.GetType, which fails in a Native AOT build of ilspycmd. The dumper
/// therefore always names its provider; these tests keep it that way.
/// </summary>
[TestFixture]
public class PdbDumperTests
{
	[Test]
	public void ReaderParametersAlwaysCarryAnExplicitSymbolReaderProvider()
	{
		var parameters = PdbDumper.CreateReaderParameters(pdbFileName: null);

		Assert.That(parameters.ReadSymbols, Is.True);
		Assert.That(parameters.SymbolReaderProvider, Is.InstanceOf<PdbReaderProvider>(),
			"without an explicit PDB the Mono.Cecil.Pdb provider must decide between embedded, Portable and Windows PDBs itself");
	}

	[Test]
	public void ExplicitPortablePdbSelectsThePortableProvider()
	{
		// The test assembly ships with a Portable PDB next to it.
		string pdbFileName = Path.ChangeExtension(typeof(PdbDumperTests).Assembly.Location, ".pdb");
		Assert.That(File.Exists(pdbFileName), $"expected a Portable PDB at {pdbFileName}");

		var parameters = PdbDumper.CreateReaderParameters(pdbFileName);

		Assert.That(parameters.SymbolReaderProvider, Is.InstanceOf<PortablePdbReaderProvider>());
		Assert.That(parameters.SymbolStream, Is.Not.Null);
		parameters.SymbolStream.Dispose();
	}
}
