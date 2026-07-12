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
using ICSharpCode.Decompiler.CSharp.Syntax;
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

	// Exercises local-reference output for local functions: the definition and every use site
	// (specialized generic invocations and method-group references) must be written with equal
	// reference objects, otherwise click-highlighting in the UI cannot match them (issue #2078).
	internal class GenericLocalFunctionHost
	{
		public int Use()
		{
			Func<int, int> f = Identity<int>;
			return Identity(42) + Identity("a").Length + f(0);

			static T2 Identity<T2>(T2 value) => value;
		}

		// Inside a generic method, Roslyn prepends the enclosing method's type parameters to the
		// compiler-generated method backing the local function, so every use site references a
		// specialized method even when the local function's own type argument is a type parameter.
		public T UseInGenericMethod<T>(T t)
		{
			return Echo(Echo(t));

			static T2 Echo<T2>(T2 value) => value;
		}
	}

	[TestFixture]
	public class TextTokenWriterTests
	{
		sealed class ReferenceRecordingOutput : ITextOutput
		{
			public readonly List<(string Text, IMember Member)> MemberReferences = new();
			public readonly List<(string Text, object Reference, bool IsDefinition)> LocalReferences = new();
			public readonly List<bool> FoldStartDefaultCollapsed = new();
			public int FoldEndCount;

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

			public void WriteLocalReference(string text, object reference, bool isDefinition = false)
			{
				LocalReferences.Add((text, reference, isDefinition));
			}

			public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false, bool isDefinition = false)
			{
				FoldStartDefaultCollapsed.Add(defaultCollapsed);
			}

			public void MarkFoldEnd()
			{
				FoldEndCount++;
			}
		}

		// The module must stay alive until the caller is done with the recorded reference
		// objects: they are type-system entities that lazily read from the PE image, e.g.
		// when NUnit formats an assertion message.
		static PEFile OpenTestAssembly()
		{
			return new PEFile(typeof(TextTokenWriterTests).Assembly.Location);
		}

		static ReferenceRecordingOutput DecompileAndCollectReferences(PEFile module, Type type)
		{
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var settings = new DecompilerSettings();
			var decompiler = new CSharpDecompiler(module, resolver, settings);
			var syntaxTree = decompiler.DecompileType(new FullTypeName(type.FullName));

			var output = new ReferenceRecordingOutput();
			var tokenWriter = new TextTokenWriter(output, settings);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
			return output;
		}

		static string FormatMember(IMember member) => $"{member.DeclaringType?.Name}.{member.Name}";

		[Test]
		public void OverrideKeywordReferencesTheOverriddenBaseMember()
		{
			using var module = OpenTestAssembly();
			var references = DecompileAndCollectReferences(module, typeof(OverrideLinkDerived)).MemberReferences;

			var overrideTargets = references.Where(r => r.Text == "override").Select(r => FormatMember(r.Member));
			Assert.That(overrideTargets, Is.EquivalentTo(new[] {
				"OverrideLinkBase.Method",
				"OverrideLinkBase.Property",
			}));
		}

		[Test]
		public void OverrideKeywordReferencesTheNearestOverriddenMember()
		{
			using var module = OpenTestAssembly();
			var references = DecompileAndCollectReferences(module, typeof(OverrideLinkSecondLevel)).MemberReferences;

			var overrideTargets = references.Where(r => r.Text == "override").Select(r => FormatMember(r.Member));
			Assert.That(overrideTargets, Is.EquivalentTo(new[] {
				"OverrideLinkDerived.Method",
			}));
		}

		[Test]
		public void VirtualAndAbstractModifiersAreNotReferences()
		{
			using var module = OpenTestAssembly();
			var references = DecompileAndCollectReferences(module, typeof(OverrideLinkBase)).MemberReferences;

			Assert.That(references.Where(r => r.Text is "virtual" or "abstract" or "override"), Is.Empty);
		}

		static ReferenceRecordingOutput WriteLeadingTrivia(DecompilerSettings settings, params Comment[] comments)
		{
			var output = new ReferenceRecordingOutput();
			var writer = new TextTokenWriter(output, settings);
			var node = new SyntaxTree();
			foreach (var comment in comments)
				node.AddLeadingTrivia(comment);
			foreach (var comment in comments)
			{
				writer.StartNode(comment);
				writer.WriteComment(comment.CommentType, comment.Content);
				writer.EndNode(comment);
			}
			return output;
		}

		[TestCase(false, true)]
		[TestCase(true, false)]
		public void XmlDocumentationFoldDefaultFollowsSetting(bool expandXmlDocumentationComments, bool expectedDefaultCollapsed)
		{
			var settings = new DecompilerSettings {
				ExpandXmlDocumentationComments = expandXmlDocumentationComments
			};

			var output = WriteLeadingTrivia(settings,
				new Comment(" <summary>", CommentType.Documentation),
				new Comment(" </summary>", CommentType.Documentation));

			Assert.That(output.FoldStartDefaultCollapsed, Is.EqualTo(new[] { expectedDefaultCollapsed }));
			Assert.That(output.FoldEndCount, Is.EqualTo(1));
		}

		[Test]
		public void SingleDocumentationLineFollowedByRegularCommentOpensNoFold()
		{
			var output = WriteLeadingTrivia(new DecompilerSettings(),
				new Comment(" <summary>single</summary>", CommentType.Documentation),
				new Comment(" regular"));

			Assert.That(output.FoldStartDefaultCollapsed, Is.Empty);
			Assert.That(output.FoldEndCount, Is.Zero);
		}

		[Test]
		public void DocumentationFoldClosesBeforeFollowingRegularComment()
		{
			var output = WriteLeadingTrivia(new DecompilerSettings(),
				new Comment(" <summary>", CommentType.Documentation),
				new Comment(" </summary>", CommentType.Documentation),
				new Comment(" regular"));

			Assert.That(output.FoldStartDefaultCollapsed, Has.Count.EqualTo(1));
			Assert.That(output.FoldEndCount, Is.EqualTo(1));
		}

		[Test]
		public void GenericLocalFunctionDefinitionAndUsesShareEqualReferenceObjects()
		{
			using var module = OpenTestAssembly();
			var output = DecompileAndCollectReferences(module, typeof(GenericLocalFunctionHost));

			Assert.Multiple(() => {
				// Two invocations (with different inferred type arguments) and one method-group reference.
				AssertLocalFunctionReferenceGroup(output, "Identity", expectedUses: 3);
				// Two nested invocations whose type argument is the enclosing method's type parameter.
				AssertLocalFunctionReferenceGroup(output, "Echo", expectedUses: 2);
			});
		}

		// Runs inside Assert.Multiple: after a recorded assertion failure, bail out of the checks
		// that would dereference the missing data instead of throwing.
		static void AssertLocalFunctionReferenceGroup(ReferenceRecordingOutput output, string name, int expectedUses)
		{
			var references = output.LocalReferences.Where(r => r.Text == name).ToList();

			var definitions = references.Where(r => r.IsDefinition).ToList();
			Assert.That(definitions, Has.Count.EqualTo(1), $"{name}: definition count");
			if (definitions.Count != 1)
				return;
			object definition = definitions[0].Reference;
			Assert.That(definition, Is.InstanceOf<IMethod>(), $"{name}: definition reference type");
			if (definition is not IMethod method)
				return;
			Assert.That(method.IsLocalFunction, Is.True, $"{name}: IsLocalFunction");

			var uses = references.Where(r => !r.IsDefinition).ToList();
			Assert.That(uses, Has.Count.EqualTo(expectedUses), $"{name}: use count");
			foreach (var use in uses)
			{
				Assert.That(use.Reference, Is.EqualTo(definition), $"{name}: use equals definition");
				Assert.That(definition, Is.EqualTo(use.Reference), $"{name}: definition equals use");
				Assert.That(use.Reference.GetHashCode(), Is.EqualTo(definition.GetHashCode()), $"{name}: hash codes");
			}
		}
	}
}
