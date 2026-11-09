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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.PdbProvider;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// For every method body of the main module, the engine's IL reader output, materialized as a
/// link-compiled <see cref="ILFunction"/>, prints the same as the managed <see cref="ILReader"/>
/// output (without debug symbols). A method that fails on one side must fail on the other.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class IlAstReadEqualityTests
{
	static readonly bool UseRefLocals = new DecompilerSettings().UseRefLocalsForAccurateOrderOfEvaluation;

	/// <summary>
	/// <see cref="ILFunction.WriteTo"/> with IL ranges, plus what the printout leaves out: the
	/// full variable types (with tuple element names and <c>dynamic</c>) and the internal
	/// <c>RemoveIfRedundant</c> flag, the local signature length and the sequence point
	/// candidates.
	/// </summary>
	internal static string Print(ILFunction function)
	{
		var output = new PlainTextOutput();
		function.WriteTo(output, new ILAstWritingOptions { ShowILRanges = true });
		output.WriteLine();
		foreach (var v in function.Variables)
			output.WriteLine($"variable {v.Name} : {v.Type}" + (v.RemoveIfRedundant ? " remove-if-redundant" : ""));
		output.WriteLine("locals: " + function.LocalVariableSignatureLength);
		output.WriteLine("sequence points: " + string.Join(",", function.SequencePointCandidates ?? []));
		return output.ToString().Replace("\r\n", "\n");
	}

	static string? Managed(MetadataModule module, MethodDefinitionHandle handle, IDebugInfoProvider? debugInfo, out string? error)
	{
		error = null;
		try
		{
			var file = (PEFile)module.MetadataFile;
			var body = file.GetMethodBody(file.Metadata.GetMethodDefinition(handle).RelativeVirtualAddress);
			var reader = new ILReader(module) {
				UseDebugSymbols = debugInfo != null,
				DebugInfo = debugInfo,
				UseRefLocalsForAccurateOrderOfEvaluation = UseRefLocals,
			};
			return Print(reader.ReadIL(handle, body));
		}
		catch (Exception ex)
		{
			error = ex.GetType().Name + ": " + ex.Message;
			return null;
		}
	}

	internal static void AssertEqual(string path, TypeSystemOptions options, bool withSymbols = false)
	{
		var ts = TypeSystemListingEqualityTests.Load(path, options);
		var module = (MetadataModule)ts.MainModule;
		IDebugInfoProvider? debugInfo = null;
		if (withSymbols)
		{
			debugInfo = DebugInfoUtils.LoadSymbols((PEFile)module.MetadataFile);
			Assert.That(debugInfo, Is.Not.Null, "no debug symbols found");
		}
		using var disposeDebugInfo = debugInfo as IDisposable;
		using var table = new RustModuleTable(ts);
		var engine = IlAstMaterializer.ReadModule(table.DebugReadIL(UseRefLocals, debugInfo), ts, table.Modules, new DecompilerSettings());
		var metadata = module.MetadataFile.Metadata;
		int compared = 0;
		foreach (var handle in metadata.MethodDefinitions)
		{
			if (metadata.GetMethodDefinition(handle).RelativeVirtualAddress == 0)
				continue;
			int row = MetadataTokens.GetRowNumber(handle);
			string name = $"method 0x{MetadataTokens.GetToken(handle):x8}";
			Assert.That(engine.TryGetValue(row, out var rust), Is.True, name + " is missing from the engine output");
			string? expected = Managed(module, handle, debugInfo, out string? managedError);
			if (expected == null || rust.Function == null)
			{
				Assert.That(rust.Error != null, Is.EqualTo(managedError != null),
					$"{name}: managed: {managedError ?? "read"}, engine: {rust.Error ?? "read"}");
				continue;
			}
			string? difference = Corpus.FirstDifference(expected, Print(rust.Function));
			Assert.That(difference, Is.Null, name + ": " + difference);
			compared++;
		}
		Assert.That(engine.Count, Is.EqualTo(metadata.MethodDefinitions.Count(h => metadata.GetMethodDefinition(h).RelativeVirtualAddress != 0)));
		TestContext.Out.WriteLine($"{compared} functions compared");
	}

	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void IlAstReadEquality(string path) => AssertEqual(path, TypeSystemOptions.Default);

	[TestCaseSource(typeof(Corpus), nameof(Corpus.ImagesWithSymbols))]
	public void IlAstReadEqualityWithDebugSymbols(string path) => AssertEqual(path, TypeSystemOptions.Default, withSymbols: true);

	/// <summary>
	/// Local names from this assembly's PDB reach the engine's variables and the printout.
	/// </summary>
	[Test]
	public void DebugSymbolsNameLocals()
	{
		var ts = TypeSystemListingEqualityTests.Load(typeof(Corpus).Assembly.Location, TypeSystemOptions.Default);
		var debugInfo = DebugInfoUtils.LoadSymbols((PEFile)((MetadataModule)ts.MainModule).MetadataFile);
		Assert.That(debugInfo, Is.Not.Null);
		using var table = new RustModuleTable(ts);
		var engine = IlAstMaterializer.ReadModule(table.DebugReadIL(UseRefLocals, debugInfo), ts, table.Modules, new DecompilerSettings());
		var functions = engine.Values.Where(f => f.Function != null).Select(f => f.Function!).ToList();
		Assert.That(functions.SelectMany(f => f.Variables)
			.Any(v => v.Kind == VariableKind.Local && v.Name == "disposeDebugInfo" && !v.HasGeneratedName), Is.True);
		Assert.That(string.Join("\n", functions.Select(Print)), Does.Contain("variable disposeDebugInfo : System.IDisposable"));
	}

	/// <summary>The printout carries the details the equality relies on.</summary>
	[Test]
	public void PrintoutCoversReaderOutput()
	{
		string path = Path.Combine(Corpus.TestCasesDirectory(), "Pretty", "ExceptionHandling.roslyn4.dll");
		var ts = TypeSystemListingEqualityTests.Load(path, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		var engine = IlAstMaterializer.ReadModule(table.DebugReadIL(UseRefLocals), ts, table.Modules, new DecompilerSettings());
		string all = string.Join("\n", engine.Values.Where(f => f.Function != null).Select(f => Print(f.Function!)));
		Assert.That(all, Does.Contain("stack "));
		Assert.That(all, Does.Contain("exception stack "));
		Assert.That(all, Does.Contain(".try "));
		Assert.That(all, Does.Contain("catch "));
		Assert.That(all, Does.Contain("(incoming: "));
		Assert.That(all, Does.Contain("leave IL_"));
		Assert.That(all, Does.Match(@"sequence points: \d"));
	}
}
