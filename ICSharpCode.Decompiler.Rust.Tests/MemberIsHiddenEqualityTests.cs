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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The engine's <c>MemberIsHidden</c> agrees with the managed one on every type, method and
/// field of every image, with the default settings (most transforms on) and with the C# 1
/// settings (most of them off).
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class MemberIsHiddenEqualityTests
{
	[TestCaseSource(typeof(Corpus), nameof(Corpus.ManagedImages))]
	public void MemberIsHiddenEquality(string path)
	{
		var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
		var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
		var typeSystem = new DecompilerTypeSystem(module, resolver);
		var engine = RustEngine.For(typeSystem, out string? reason);
		Assert.That(engine, Is.Not.Null, reason);
		var metadata = module.Metadata;
		var members = new List<EntityHandle>();
		members.AddRange(metadata.TypeDefinitions.Select(h => (EntityHandle)h));
		members.AddRange(metadata.MethodDefinitions.Select(h => (EntityHandle)h));
		members.AddRange(metadata.FieldDefinitions.Select(h => (EntityHandle)h));
		var differences = new List<string>();
		foreach (var settings in new[] { new DecompilerSettings(), new DecompilerSettings(LanguageVersion.CSharp1) })
		{
			foreach (var member in members)
			{
				bool managed = CSharpDecompiler.MemberIsHidden(module, member, settings);
				if (engine!.MemberIsHidden(member, settings) != managed)
					differences.Add($"0x{MetadataTokens.GetToken(member):x8} (managed: {managed}, {(settings.AnonymousMethods ? "default" : "C# 1")} settings)");
			}
		}
		Assert.That(differences, Is.Empty);
	}
}
