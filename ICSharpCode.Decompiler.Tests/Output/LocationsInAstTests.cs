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

using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Output
{
	// Sample type decompiled by the tests below. A small method body gives the
	// location-setting output path several statements and tokens to assign positions to.
	internal class LocationSample
	{
		public int Add(int a, int b)
		{
			int sum = a + b;
			return sum;
		}
	}

	// Characterization coverage for the "AST with locations" output path
	// (TokenWriter.CreateWriterThatSetsLocationsInAST -> InsertMissingTokensDecorator). The Pretty
	// suite never drives this path, yet PDB sequence points depend on it (SequencePointBuilder reads
	// node.StartLocation/EndLocation; the GUI mixed-language view consumes those points). These tests
	// pin the observable consequences that must survive later AST changes: the located path emits the
	// same text as the plain path, real nodes receive sensible locations, and sequence points are
	// produced for a method body.
	[TestFixture]
	public class LocationsInAstTests
	{
		static CSharpDecompiler CreateDecompiler(out DecompilerSettings settings)
		{
			string assemblyPath = typeof(LocationsInAstTests).Assembly.Location;
			var module = new PEFile(assemblyPath);
			var resolver = new UniversalAssemblyResolver(assemblyPath, false, module.Metadata.DetectTargetFrameworkId());
			settings = new DecompilerSettings();
			return new CSharpDecompiler(module, resolver, settings);
		}

		static SyntaxTree DecompileSample(out CSharpDecompiler decompiler, out DecompilerSettings settings)
		{
			decompiler = CreateDecompiler(out settings);
			return decompiler.DecompileType(new FullTypeName(typeof(LocationSample).FullName));
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
			Assert.That(names, Does.Contain(nameof(LocationSample.Add)));
			Assert.That(names, Does.Contain("a").And.Contains("b"));
		}

		[Test]
		public void SequencePointsAreGeneratedForTheMethodBody()
		{
			var syntaxTree = DecompileSample(out var decompiler, out var settings);
			Print(syntaxTree, settings, setLocations: true);

			var sequencePoints = decompiler.CreateSequencePoints(syntaxTree);
			Assert.That(sequencePoints, Is.Not.Empty, "no sequence points were produced");

			var points = sequencePoints
				.First(kvp => kvp.Key.Name == nameof(LocationSample.Add))
				.Value;
			Assert.That(points, Is.Not.Empty, "the sample method produced no sequence points");
			Assert.That(points.Any(p => !p.IsHidden && p.StartLine > 0), Is.True,
				"no visible sequence point carries a source line");
		}
	}
}
