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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Output
{
	// Sample hierarchy decompiled by the tests below. The members exercise the
	// "override links to the overridden member" feature for methods and properties,
	// including a second override level to pin down "nearest base member" semantics.
	internal abstract class OverrideLinkBase
	{
		public abstract void Method();
		public virtual int Property { get; set; }
	}

	internal class OverrideLinkDerived : OverrideLinkBase
	{
		public override void Method() { }
		public override int Property { get; set; }
	}

	internal class OverrideLinkSecondLevel : OverrideLinkDerived
	{
		public override void Method() { }
	}

	[TestFixture]
	public class TextTokenWriterTests
	{
		sealed class ReferenceRecordingOutput : ITextOutput
		{
			public readonly List<(string Text, IMember Member)> MemberReferences = new();

			public string IndentationString { get; set; } = "\t";
			public void Indent() { }
			public void Unindent() { }
			public void Write(char ch) { }
			public void Write(string text) { }
			public void WriteLine() { }
			public void WriteReference(OpCodeInfo opCode, bool omitSuffix = false) { }
			public void WriteReference(MetadataFile metadata, Handle handle, string text, string protocol = "decompile", bool isDefinition = false) { }
			public void WriteReference(IType type, string text, bool isDefinition = false) { }

			public void WriteReference(IMember member, string text, bool isDefinition = false)
			{
				if (!isDefinition)
					MemberReferences.Add((text, member));
			}

			public void WriteLocalReference(string text, object reference, bool isDefinition = false) { }
			public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false, bool isDefinition = false) { }
			public void MarkFoldEnd() { }
		}

		static List<(string Text, IMember Member)> DecompileAndCollectMemberReferences(Type type)
		{
			string assemblyPath = typeof(TextTokenWriterTests).Assembly.Location;
			using var module = new PEFile(assemblyPath);
			var resolver = new UniversalAssemblyResolver(assemblyPath, false, module.Metadata.DetectTargetFrameworkId());
			var settings = new DecompilerSettings();
			var decompiler = new CSharpDecompiler(module, resolver, settings);
			var syntaxTree = decompiler.DecompileType(new FullTypeName(type.FullName));

			var output = new ReferenceRecordingOutput();
			var tokenWriter = new TextTokenWriter(output, settings, decompiler.TypeSystem);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
			return output.MemberReferences;
		}

		static string FormatMember(IMember member) => $"{member.DeclaringType?.Name}.{member.Name}";

		[Test]
		public void OverrideKeywordReferencesTheOverriddenBaseMember()
		{
			var references = DecompileAndCollectMemberReferences(typeof(OverrideLinkDerived));

			var overrideTargets = references.Where(r => r.Text == "override").Select(r => FormatMember(r.Member));
			Assert.That(overrideTargets, Is.EquivalentTo(new[] {
				"OverrideLinkBase.Method",
				"OverrideLinkBase.Property",
			}));
		}

		[Test]
		public void OverrideKeywordReferencesTheNearestOverriddenMember()
		{
			var references = DecompileAndCollectMemberReferences(typeof(OverrideLinkSecondLevel));

			var overrideTargets = references.Where(r => r.Text == "override").Select(r => FormatMember(r.Member));
			Assert.That(overrideTargets, Is.EquivalentTo(new[] {
				"OverrideLinkDerived.Method",
			}));
		}

		[Test]
		public void VirtualAndAbstractModifiersAreNotReferences()
		{
			var references = DecompileAndCollectMemberReferences(typeof(OverrideLinkBase));

			Assert.That(references.Where(r => r.Text is "virtual" or "abstract" or "override"), Is.Empty);
		}
	}
}
