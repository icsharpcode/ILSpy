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
using System.Runtime.CompilerServices;
using System.Text.Json;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// G-ilast(N): for every method body of every corpus image, the engine's function after IL
/// prefix N (the engine's <c>IL_PREFIX</c>, served through the session export seam 1 uses)
/// prints the same as the managed function after the same prefix, except the entries of
/// rust/ilast-divergence-allowlist.json.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class IlAstPrefixEqualityTests
{
	sealed record Divergence(string Transform, string Fixture, string Method, string Reason);

	static string RepositoryRoot([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, ".."));

	static readonly Lazy<List<Divergence>> allowlist = new(() => JsonSerializer.Deserialize<List<Divergence>>(
		File.ReadAllText(Path.Combine(RepositoryRoot(), "rust", "ilast-divergence-allowlist.json")),
		new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower })!);

	/// <summary>
	/// The transforms that run exactly the first <paramref name="prefix"/> leaves of
	/// <paramref name="pipeline"/> (see <see cref="IlTransformPrefix"/>): a group the prefix
	/// ends in is replaced by a truncated copy, so nested transforms get their own index.
	/// </summary>
	internal static List<IILTransform> Truncate(IEnumerable<IILTransform> pipeline, int prefix)
	{
		var result = new List<IILTransform>();
		foreach (var transform in pipeline)
		{
			if (prefix == 0)
				break;
			int leaves = IlTransformPrefix.Leaves(transform).Count();
			if (leaves <= prefix)
			{
				result.Add(transform);
				prefix -= leaves;
				continue;
			}
			var group = (BlockILTransform)transform;
			var copy = new BlockILTransform();
			prefix = Fill(group.PreOrderTransforms, copy.PreOrderTransforms, prefix);
			Fill(group.PostOrderTransforms, copy.PostOrderTransforms, prefix);
			result.Add(copy);
			break;
		}
		return result;
	}

	static int Fill(IList<IBlockTransform> source, IList<IBlockTransform> target, int prefix)
	{
		foreach (var transform in source)
		{
			if (prefix == 0)
				break;
			int leaves = IlTransformPrefix.Leaves(transform).Count();
			if (leaves <= prefix)
			{
				target.Add(transform);
				prefix -= leaves;
				continue;
			}
			var statement = (StatementTransform)transform;
			target.Add(new StatementTransform(IlTransformPrefix.Children(statement).Take(prefix).ToArray()));
			return 0;
		}
		return prefix;
	}

	/// <summary>
	/// Every prefix has exactly as many leaves as its index, and the full prefix is the whole
	/// pipeline: the 59 transforms of RustPort_AgentInstructions.md items 7 to 64 (item 32 is
	/// a FORK, item 56 covers three transforms).
	/// </summary>
	[Test]
	public void TruncationCountsLeaves()
	{
		var names = CSharpDecompiler.GetILTransforms().SelectMany(IlTransformPrefix.Leaves).Select(t => t.GetType().Name).ToList();
		Assert.That(names, Has.Count.EqualTo(59));
		for (int n = 0; n <= names.Count; n++)
		{
			var truncated = Truncate(CSharpDecompiler.GetILTransforms(), n).SelectMany(IlTransformPrefix.Leaves).Select(t => t.GetType().Name);
			Assert.That(truncated, Is.EqualTo(names.Take(n)), $"prefix {n}");
		}
	}

	static string? Managed(DecompilerTypeSystem ts, MethodDefinitionHandle handle, DecompilerSettings settings, int prefix, out string? error)
	{
		error = null;
		try
		{
			var module = (MetadataModule)ts.MainModule;
			var file = (PEFile)module.MetadataFile;
			var body = file.GetMethodBody(file.Metadata.GetMethodDefinition(handle).RelativeVirtualAddress);
			var reader = new ILReader(module) {
				UseDebugSymbols = settings.UseDebugSymbols,
				UseRefLocalsForAccurateOrderOfEvaluation = settings.UseRefLocalsForAccurateOrderOfEvaluation,
			};
			var function = reader.ReadIL(handle, body);
			// RunTransforms without its Debug-only invariant checks: the reference is what a Release
			// pipeline produces, and the managed async/await decompiler leaves trees on some runtime
			// methods that the checks reject (an instruction shared by two blocks).
			var context = new ILTransformContext(function, ts, null, settings);
			foreach (var transform in Truncate(CSharpDecompiler.GetILTransforms(), prefix))
			{
				if (transform is BlockILTransform blockTransform)
					RunBlockTransforms(blockTransform, function, context);
				else
					transform.Run(function, context);
			}
			return IlAstReadEqualityTests.Print(function);
		}
		catch (Exception ex)
		{
			error = ex.GetType().Name + ": " + ex.Message;
			return null;
		}
	}

	/// <summary>
	/// <c>BlockILTransform.Run</c> without the Debug-only invariant checks of
	/// <c>Block.RunTransforms</c>, for the same reason as the loop above.
	/// </summary>
	static void RunBlockTransforms(BlockILTransform blockTransform, ILFunction function, ILTransformContext context)
	{
		var blockContext = new BlockTransformContext(context);
		foreach (var container in function.Descendants.OfType<BlockContainer>().ToList())
		{
			var cfg = new ControlFlowGraph(container, context.CancellationToken);
			blockContext.ControlFlowGraph = cfg;
			IEnumerable<ControlFlowNode> Preorder(ControlFlowNode node)
			{
				Run(node, blockTransform.PreOrderTransforms);
				return node.DominatorTreeChildren;
			}
			foreach (var node in TreeTraversal.PostOrder(cfg.GetNode(container.EntryPoint), Preorder))
				Run(node, blockTransform.PostOrderTransforms);
		}

		void Run(ControlFlowNode node, IList<IBlockTransform> transforms)
		{
			var block = (Block)node.UserData;
			blockContext.ControlFlowNode = node;
			blockContext.Block = block;
			blockContext.IndexOfFirstAlreadyTransformedInstruction = block.Instructions.Count;
			foreach (var transform in transforms)
				transform.Run(block, blockContext);
		}
	}

	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void IlAstPrefixEquality(string path) => AssertPrefixEqual(path, new DecompilerSettings());

	/// <summary>
	/// Every setting the engine reads (<see cref="RustSettings"/>) flipped from its default,
	/// over the fixtures whose names suggest they exercise them (Switch*, UnsafeCode*) and the
	/// decompiler itself.
	/// </summary>
	[TestCaseSource(nameof(SettingsImages))]
	public void IlAstPrefixEqualityWithFlippedSettings(string path)
	{
		var settings = new DecompilerSettings();
		foreach (string setting in RustSettings.Names)
		{
			var property = typeof(DecompilerSettings).GetProperty(setting)!;
			property.SetValue(settings, !(bool)property.GetValue(settings)!);
		}
		AssertPrefixEqual(path, settings);
	}

	static IEnumerable<TestCaseData> SettingsImages() => Corpus.ManagedImages().Where(c => Path.GetFileName((string)c.Arguments[0]!) is var name
		&& (name.StartsWith("Switch", StringComparison.Ordinal) || name.StartsWith("UnsafeCode", StringComparison.Ordinal)
			|| name.StartsWith("ICSharpCode.Decompiler", StringComparison.Ordinal)));

	[Test]
	public void EngineSettingsAreDecompilerSettings()
	{
		Assert.That(RustSettings.Names, Is.EqualTo(new[] { "RemoveDeadStores", "SortSwitchSections", "PatternBasedFixedStatement", "YieldReturn", "AsyncAwait", "AsyncEnumerator", "AwaitInCatchFinally", "Dynamic", "SparseIntegerSwitch", "RemoveDeadCode", "SwitchStatementOnString", "SwitchOnReadOnlySpanChar", "LiftNullables", "PatternMatching", "RecursivePatternMatching", "PatternCombinators", "RelationalPatterns", "AssumeArrayLengthFitsIntoInt32", "SwitchExpressions", "Ranges", "LockStatement", "UsingStatement", "AsyncUsingAndForEachStatement", "IntroduceRefModifiersOnStructs" }));
		var settings = new DecompilerSettings { RemoveDeadStores = false, SortSwitchSections = true, PatternBasedFixedStatement = false, YieldReturn = true, AsyncAwait = false, AsyncEnumerator = true, AwaitInCatchFinally = false, Dynamic = true, SparseIntegerSwitch = false, RemoveDeadCode = true, SwitchStatementOnString = false, SwitchOnReadOnlySpanChar = true, LiftNullables = false, PatternMatching = true, RecursivePatternMatching = false, PatternCombinators = true, RelationalPatterns = false, AssumeArrayLengthFitsIntoInt32 = true, SwitchExpressions = false, Ranges = true, LockStatement = false, UsingStatement = true, AsyncUsingAndForEachStatement = false, IntroduceRefModifiersOnStructs = true };
		Assert.That(RustSettings.Bits(settings), Is.EqualTo(0xAAAAAAUL));
	}

	static void AssertPrefixEqual(string path, DecompilerSettings settings)
	{
		int prefix = RustEngine.ILPrefix;
		var ts = TypeSystemListingEqualityTests.Load(path, DecompilerTypeSystem.GetOptions(settings));
		var engine = RustEngine.For(ts, out string? unsupported);
		Assert.That(engine, Is.Not.Null, unsupported);
		string fixture = Path.GetFileName(path);
		var metadata = ts.MainModule.MetadataFile.Metadata;
		int compared = 0;
		foreach (var handle in metadata.MethodDefinitions)
		{
			if (metadata.GetMethodDefinition(handle).RelativeVirtualAddress == 0)
				continue;
			string name = $"method 0x{MetadataTokens.GetToken(handle):x8}";
			if (allowlist.Value.Any(d => d.Fixture == fixture && d.Method == name))
				continue;
			string? expected = Managed(ts, handle, settings, prefix, out string? managedError);
			string? engineError = null;
			ILFunction? function = null;
			try
			{
				var status = engine!.ReadFunction(handle, prefix, settings, null, out function, out string? reason);
				Assert.That(status, Is.EqualTo(RustStatus.Ok), $"{name}: {reason}");
			}
			catch (RustEngineException ex)
			{
				engineError = ex.Message;
			}
			if (expected == null || function == null)
			{
				Assert.That(engineError != null, Is.EqualTo(managedError != null),
					$"{name}: managed: {managedError ?? "read"}, engine: {engineError ?? "read"}");
				continue;
			}
			string? difference = Corpus.FirstDifference(expected, IlAstReadEqualityTests.Print(function));
			Assert.That(difference, Is.Null, $"{name} after IL prefix {prefix}: {difference}");
			compared++;
		}
		TestContext.Out.WriteLine($"{compared} functions compared at IL prefix {prefix}");
	}
}
