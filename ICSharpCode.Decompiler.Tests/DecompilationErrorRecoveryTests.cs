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
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
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

	}
}
