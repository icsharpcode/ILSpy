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
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// A method body that cannot be decompiled must not take the surrounding type - or, when
	/// exporting a project, the surrounding assembly - down with it. The failure is turned into
	/// output the user can copy into a bug report, and decompilation continues.
	/// </summary>
	[TestFixture]
	public class DecompilationErrorRecoveryTests
	{

		[Test]
		public void FailingMethodBodyKeepsTheRestOfTheType()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));

			string code = decompiler.DecompileTypeAsString(
				new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(code, Does.Contain(StepperTesting.SimulatedFailure), "the exception text must show up in the output");
				Assert.That(code, Does.Contain(CSharpDecompiler.DecompilationErrorReportUrl), "users need to be told where to report this");
				Assert.That(code, Does.Contain("public static string CleanUpFileName"), "the failing member keeps its signature");
				Assert.That(code, Does.Contain("DecompileProject"), "the other members of the type are unaffected");
			}
		}

		[Test]
		public void FailingMethodBodyIsRecordedAsError()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			decompiler.ILTransforms.Add(new StepperTesting.ThrowingILTransform("CleanUpFileName"));

			decompiler.DecompileTypeAsString(
				new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"));

			var error = decompiler.Errors.Single();
			Assert.That(error.Message, Does.Contain("CleanUpFileName"));
		}

		/// <summary>
		/// <see cref="CSharpDecompiler.Errors"/> describes the decompilation that just ran, so a
		/// reused instance must not report the previous one's failures against it.
		/// </summary>
		[Test]
		public void ErrorsCoverOnlyTheLastDecompilation()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			var failing = new StepperTesting.ThrowingILTransform("CleanUpFileName");
			decompiler.ILTransforms.Add(failing);
			decompiler.DecompileTypeAsString(
				new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"));

			decompiler.ILTransforms.Remove(failing);
			decompiler.DecompileTypeAsString(
				new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"));

			Assert.That(decompiler.Errors, Is.Empty);
		}

		/// <summary>
		/// A member whose output throws is replaced by the error text, and writing carries on with
		/// the rest of the type - a file cut off mid-member would leave the braces around it open
		/// and every later type unreadable.
		/// </summary>
		[Test]
		public void FailingOutputKeepsTheFileWellFormed()
		{
			var decompiler = StepperTesting.CreateDecompiler();
			var syntaxTree = decompiler.DecompileType(
				new FullTypeName("ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler"));

			// A member that cannot be written: an expression node with no children to write.
			var victim = syntaxTree.Descendants.OfType<MethodDeclaration>().First(m => m.Name == "CleanUpFileName");
			victim.Body.Statements.Clear();
			victim.Body.Statements.Add(new ExpressionStatement(new BinaryOperatorExpression()));

			var writer = new StringWriter();
			var outputVisitor = new ErrorTolerantOutputVisitor(writer, new DecompilerSettings().CSharpFormattingOptions);
			syntaxTree.AcceptVisitor(outputVisitor);
			string code = writer.ToString();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(outputVisitor.Errors, Has.Count.EqualTo(1), "the failure is reported to the caller");
				Assert.That(code, Does.Contain(CSharpDecompiler.DecompilationErrorReportUrl), "and shows up in the file");
				Assert.That(code, Does.Contain("DecompileProject"), "the members after the failing one are still written");
				Assert.That(code.Count(c => c == '{'), Is.EqualTo(code.Count(c => c == '}')),
					"every brace the failed member opened is closed again");
			}
		}

		/// <summary>
		/// A .resources container holds every BAML stream of an assembly. One entry the decompiler
		/// cannot write - obfuscated BAML that produces characters XML cannot carry, say - must not
		/// take the entries next to it down: they are unrelated pages of an unrelated type.
		/// </summary>
		[Test]
		public void FailingResourceEntryKeepsTheOtherEntriesOfTheContainer()
		{
			string location = typeof(DecompilationErrorRecoveryTests).Assembly.Location;
			using var stream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var module = new PEFile(location, stream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var decompiler = new EntryFailingProjectDecompiler(
				new UniversalAssemblyResolver(location, throwOnError: false, module.DetectTargetFrameworkId()));

			var items = decompiler.WriteResources(module).ToList();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(items.Select(i => i.FileName), Does.Contain("good.baml"),
					"the entry after the failing one is still written");
				Assert.That(decompiler.Errors, Has.Count.EqualTo(1), "the failure is reported to the caller");
				Assert.That(decompiler.Errors[0].ToString(), Does.Contain("bad.baml"),
					"and names the entry that failed");
			}
		}

		/// <summary>
		/// Writes every resource entry as a project item, except the one named "bad.baml", which
		/// throws the way a resource handler does when it cannot produce a file.
		/// </summary>
		sealed class EntryFailingProjectDecompiler : WholeProjectDecompiler
		{
			public EntryFailingProjectDecompiler(IAssemblyResolver assemblyResolver)
				: base(assemblyResolver)
			{
				// Entries this fixture does not override still get written to disk.
				TargetDirectory = Directory.CreateTempSubdirectory("ILSpyResourceRecovery").FullName;
			}

			public IEnumerable<ProjectItemInfo> WriteResources(MetadataFile module)
				=> WriteResourceFilesInProject(module);

			protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
			{
				if (resourceName == "bad.baml")
					throw new NotSupportedException("cannot write bad.baml");
				return new[] { new ProjectItemInfo("Page", fileName) };
			}
		}

	}
}
