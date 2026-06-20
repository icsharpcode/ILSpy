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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Output
{
	// Characterization coverage for the "AST with locations" output path
	// (TokenWriter.CreateWriterThatSetsLocationsInAST -> InsertMissingTokensDecorator). The Pretty
	// suite never drives this path, yet PDB sequence points depend on it (SequencePointBuilder reads
	// node.StartLocation/EndLocation; the GUI mixed-language view consumes those points). These tests
	// pin the observable consequences that must survive later AST changes: the located path emits the
	// same text as the plain path, real nodes receive sensible locations, and sequence points are
	// produced for a method body.
	//
	// Like the Pretty tests, the samples live in a TestCases fixture compiled through Tester with a
	// fixed Roslyn version, rather than being read out of this test assembly: its Debug/Release build
	// flavor would otherwise change the sample IL and shift the sequence-point coordinates locked below.
	[TestFixture]
	public class LocationsInAstTests
	{
		// The sample source sits next to this test file under Output/, not in TestCases/.
		static readonly string SamplesPath = Path.Combine(Tester.TestCasePath, "..", "Output");

		const CompilerOptions SampleCompilerOptions =
			CompilerOptions.UseRoslyn4_14_0 | CompilerOptions.UseDebug | CompilerOptions.Library;

		const string LocationSampleName = "ICSharpCode.Decompiler.Tests.TestCases.LocationsInAst.LocationSample";
		const string SequencePointSampleName = "ICSharpCode.Decompiler.Tests.TestCases.LocationsInAst.SequencePointSample";

		static CompilerResults compiledSamples;

		[OneTimeSetUp]
		public void CompileSamples()
		{
			var csFile = Path.Combine(SamplesPath, "LocationsInAstSamples.cs");
			compiledSamples = Tester.CompileCSharp(csFile, SampleCompilerOptions).GetAwaiter().GetResult();
		}

		[OneTimeTearDown]
		public void DeleteCompiledSamples()
		{
			compiledSamples?.DeleteTempFiles();
		}

		static CSharpDecompiler CreateDecompiler(out DecompilerSettings settings)
		{
			string assemblyPath = compiledSamples.PathToAssembly;
			var module = new PEFile(assemblyPath);
			var resolver = new UniversalAssemblyResolver(assemblyPath, false, module.Metadata.DetectTargetFrameworkId());
			settings = new DecompilerSettings();
			return new CSharpDecompiler(module, resolver, settings);
		}

		static SyntaxTree DecompileSample(out CSharpDecompiler decompiler, out DecompilerSettings settings)
		{
			decompiler = CreateDecompiler(out settings);
			return decompiler.DecompileType(new FullTypeName(LocationSampleName));
		}

		static string Print(SyntaxTree syntaxTree, DecompilerSettings settings, bool setLocations)
		{
			var writer = new StringWriter();
			TokenWriter tokenWriter = setLocations
				? TokenWriter.CreateWriterThatSetsLocationsInAST(writer)
				: TokenWriter.Create(writer);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
			return writer.ToString();
		}

		[Test]
		public void LocatedPathEmitsSameTextAsPlainPath()
		{
			// The location-setting decorator must be output-transparent: inserting it changes only the
			// AST side effects (locations + reconstructed tokens), never the emitted characters.
			var plainTree = DecompileSample(out _, out var settings);
			string plain = Print(plainTree, settings, setLocations: false);

			var locatedTree = DecompileSample(out _, out settings);
			string located = Print(locatedTree, settings, setLocations: true);

			Assert.That(located, Is.EqualTo(plain));
		}

		// Returns the substring of <paramref name="length"/> characters that starts at the
		// (1-based line, 1-based column) location, asserting the location is inside the text.
		static string TextAt(string[] lines, TextLocation location, int length)
		{
			Assert.That(location.Line, Is.InRange(1, lines.Length), "location line is out of range");
			string line = lines[location.Line - 1];
			int start = location.Column - 1;
			Assert.That(start, Is.GreaterThanOrEqualTo(0));
			Assert.That(start + length, Is.LessThanOrEqualTo(line.Length), "location column + length exceeds the line");
			return line.Substring(start, length);
		}

		[Test]
		public void StoredIdentifierLocationsIndexTheirNamesInTheOutput()
		{
			// Correctness check for the locations the path assigns: an identifier's StartLocation must
			// point at exactly where its name appears in the emitted text. Re-print without locations to
			// the same text and slice each identifier's name back out of it.
			var syntaxTree = DecompileSample(out _, out var settings);
			string text = Print(syntaxTree, settings, setLocations: true);
			string[] lines = text.Replace("\r\n", "\n").Split('\n');

			var identifiers = syntaxTree.Descendants
				.OfType<Identifier>()
				.Where(id => !string.IsNullOrEmpty(id.Name) && id.StartLocation.Line > 0)
				.ToList();
			Assert.That(identifiers, Is.Not.Empty, "no positioned identifiers in the decompiled output");

			foreach (var id in identifiers)
			{
				// A verbatim identifier is written as "@name"; its StartLocation points at the '@'.
				var nameStart = new TextLocation(id.StartLocation.Line, id.StartLocation.Column + (id.IsVerbatim ? 1 : 0));
				Assert.That(TextAt(lines, nameStart, id.Name.Length), Is.EqualTo(id.Name),
					$"identifier '{id.Name}' at {id.StartLocation} does not index its name in the output");
			}

			// Guard against the loop passing vacuously: the sample's own members must be among them.
			var names = identifiers.Select(id => id.Name).ToList();
			Assert.That(names, Does.Contain("Add"));
			Assert.That(names, Does.Contain("a").And.Contains("b"));
		}

		[Test]
		public void RenamedConstructorNameSurvivesLocationSetting()
		{
			// A constructor whose name differs from its declaring type (e.g. after a type rename) makes
			// CSharpOutputVisitor.VisitConstructorDeclaration print a detached clone of the type's name
			// token. That clone reaches the location-setting decorator parentless, so it has no slot; the
			// decorator must skip it instead of routing SlotKind.None into the throwing child setter.
			var type = new TypeDeclaration { ClassType = ClassType.Class, Name = "Foo" };
			type.Members.Add(new ConstructorDeclaration { Name = "Bar", Body = new BlockStatement() });
			var syntaxTree = new SyntaxTree();
			syntaxTree.Members.Add(type);

			var settings = new DecompilerSettings();
			string located = null;
			Assert.That(() => located = Print(syntaxTree, settings, setLocations: true), Throws.Nothing);

			// The clone path is the one under test: the constructor is named after the type, not "Bar".
			Assert.That(located, Does.Contain("Foo()"));
			Assert.That(located, Is.EqualTo(Print(syntaxTree, settings, setLocations: false)));
		}

		[Test]
		public void SequencePointsAreGeneratedForTheMethodBody()
		{
			var syntaxTree = DecompileSample(out var decompiler, out var settings);
			Print(syntaxTree, settings, setLocations: true);

			var sequencePoints = decompiler.CreateSequencePoints(syntaxTree);
			Assert.That(sequencePoints, Is.Not.Empty, "no sequence points were produced");

			var points = sequencePoints
				.First(kvp => kvp.Key.Name == "Add")
				.Value;
			Assert.That(points, Is.Not.Empty, "the sample method produced no sequence points");
			Assert.That(points.Any(p => !p.IsHidden && p.StartLine > 0), Is.True,
				"no visible sequence point carries a source line");
		}

		// Locks the exact source spans of the visible sequence points for a header-rich method, so a
		// change to how SequencePointBuilder derives brace/paren/keyword locations from the surrounding
		// real nodes cannot silently shift PDB coordinates.
		// The coordinates are in decompiled-output space and are pinned to the fixed compilation above.
		static readonly string[] ExpectedHeaderSequencePoints = {
			"10,50 - 10,57",
			"10,59 - 10,67",
			"10,8 - 10,48",
			"15,4 - 15,10",
			"17,3 - 17,18",
			"18,3 - 21,4",
			"20,4 - 20,16",
			"22,3 - 22,28",
			"23,3 - 23,15",
			"24,3 - 24,4",
			"25,4 - 25,10",
			"26,3 - 26,4",
			"28,3 - 28,4",
			"29,4 - 29,19",
			"31,24 - 31,52",
			"32,3 - 32,4",
			"33,4 - 33,14",
			"36,3 - 36,4",
			"37,4 - 37,14",
			"8,2 - 8,3",
			"9,3 - 9,15",
		};

		[Test]
		public void HeaderSequencePointCoordinatesAreStable()
		{
			var decompiler = CreateDecompiler(out var settings);
			var syntaxTree = decompiler.DecompileType(new FullTypeName(SequencePointSampleName));
			Print(syntaxTree, settings, setLocations: true);

			var points = decompiler.CreateSequencePoints(syntaxTree)
				.First(kvp => kvp.Key.Name == "Headers")
				.Value;

			var actual = points
				.Where(p => !p.IsHidden)
				.Select(p => $"{p.StartLine},{p.StartColumn} - {p.EndLine},{p.EndColumn}")
				.OrderBy(s => s, StringComparer.Ordinal)
				.ToArray();

			Assert.That(actual, Is.EqualTo(ExpectedHeaderSequencePoints));
		}
	}
}
