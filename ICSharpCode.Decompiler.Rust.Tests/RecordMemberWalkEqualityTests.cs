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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The engine's member walk of every record agrees with the managed one, whose fields and
/// properties come in <c>RecordDecompiler.FieldsAndProperties</c> order.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class RecordMemberWalkEqualityTests
{
	/// <summary>
	/// A private field Equals compares and PrintMembers skips, between members both see.
	/// </summary>
	sealed record InterleavedRecord(int A)
	{
		int b = A;
		public string C { get; init; } = "";
		public int D = A + 1;
		public int B => b;
	}

	public static IEnumerable<TestCaseData> Images()
	{
		yield return new TestCaseData(typeof(InterleavedRecord).Assembly.Location).SetArgDisplayNames(Path.GetFileName(typeof(InterleavedRecord).Assembly.Location));
		foreach (var c in Corpus.ManagedImages())
			yield return c;
	}

	[TestCaseSource(nameof(Images))]
	public void RecordMemberWalkEquality(string path)
	{
		var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
		var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
		var typeSystem = new DecompilerTypeSystem(module, resolver);
		var engine = RustEngine.For(typeSystem, out string? reason);
		Assert.That(engine, Is.Not.Null, reason);
		var settings = new DecompilerSettings();
		var differences = new List<string>();
		foreach (var type in typeSystem.MainModule.TypeDefinitions)
		{
			if (!type.IsRecord || type.Kind is not (TypeKind.Class or TypeKind.Struct))
				continue;
			var record = new RecordDecompiler(typeSystem, type, settings, CancellationToken.None);
			var managed = type.NestedTypes.Cast<IEntity>()
				.Concat(record.FieldsAndProperties.Select(m => m.MemberDefinition))
				.Concat(type.Events).Concat(type.Methods)
				.Where(e => !e.MetadataToken.IsNil)
				.Select(e => MetadataTokens.GetToken(e.MetadataToken)).ToList();
			var walk = engine!.MemberWalk(type, settings, out string? walkReason, out _, out _);
			if (walk == null)
			{
				differences.Add($"{type.FullName}: {walkReason}");
				continue;
			}
			var engineOrder = walk.Select(w => MetadataTokens.GetToken(w.Entity)).ToList();
			if (!managed.SequenceEqual(engineOrder))
				differences.Add($"{type.FullName}: managed {string.Join(" ", managed.Select(t => t.ToString("x8")))}, engine {string.Join(" ", engineOrder.Select(t => t.ToString("x8")))}");
		}
		Assert.That(differences, Is.Empty);
	}
}
