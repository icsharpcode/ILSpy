using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class PdbGenerationTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/PdbGen";

		/// <summary>
		/// How strictly a reconstructed breakpoint must match the original compiler's.
		/// </summary>
		enum Tolerance
		{
			/// <summary>Visible breakpoints must match on source line and column.</summary>
			LinesAndColumns,
			/// <summary>Only the source line span must match; column placement may differ.</summary>
			Lines
		}

		[Test]
		public void HelloWorld()
		{
			TestSequencePoints();
		}

		[Test]
		public void ForLoopTests()
		{
			// The compiler hides the unconditional branch from the loop setup to the condition test;
			// the decompiler folds that IL into the loop body's point instead. The residual pins it.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void LambdaCapturing()
		{
			// The decompiler emits one extra visible breakpoint that the C# compiler keeps hidden;
			// the recorded residual pins that known, deliberate difference.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void Members()
		{
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void TryCatchFinally()
		{
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void SwitchStatement()
		{
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void AsyncAwait()
		{
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void YieldReturn()
		{
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void ForeachUsing()
		{
			// The using statement and foreach are lowered to try/finally; the decompiler places the
			// disposal and the closing braces differently from the compiler.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void WhileLoops()
		{
			// The compiler keeps the while-condition back-branch hidden, while the decompiler
			// projects the loop condition and body points onto the decompiled source layout.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void LockStatement()
		{
			// The compiler breakpoints the lock's closing brace and the static-field initializer in
			// the .cctor; the decompiler reconstructs the lock exit and the cctor differently.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void LinqQuery()
		{
			// The query is lowered to lambdas; the decompiler emits an extra hidden point inside each
			// lambda and places the enclosing foreach's point differently from the compiler.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void SwitchExpression()
		{
			// The compiler breakpoints each switch-expression arm; the decompiler breakpoints the
			// method's opening brace and keeps the arm bodies hidden.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void LocalFunctions()
		{
			TestSequencePoints();
		}

		[Test]
		public void GotoLabels()
		{
			TestSequencePoints();
		}

		[Test]
		public void CheckedUnchecked()
		{
			TestSequencePoints();
		}

		[Test]
		public void NestedLoops()
		{
			// Same residual as ForLoopTests, once per loop: the compiler keeps the back-branch from
			// the increment to the condition test hidden, while the decompiler folds that IL into the
			// loop body's point.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void ConditionalOperators()
		{
			// The decompiler emits an extra hidden point after each statement that the compiler
			// does not, where the ?? / ?: result is consumed.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void PatternMatching()
		{
			TestSequencePoints();
		}

		[Test]
		public void UsingDeclaration()
		{
			// The using declaration is lowered to a try/finally whose disposal point the decompiler
			// reconstructs at a different location than the compiler.
			TestSequencePoints(knownResidual: true);
		}

		[Test]
		public void CustomPdbId()
		{
			// Generate a PDB for an assembly using a randomly-generated ID, then validate that the PDB uses the specified ID
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(CustomPdbId));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			var expectedPdbId = new BlobContentId(Guid.NewGuid(), (uint)Random.Shared.Next());

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(CustomPdbId) + ".pdb"), FileMode.Create, FileAccess.ReadWrite))
			{
				new PortablePdbWriter { NoLogo = true, PdbId = expectedPdbId }
					.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream);

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);

				Assert.That(generatedPdbId.Guid, Is.EqualTo(expectedPdbId.Guid));
				Assert.That(generatedPdbId.Stamp, Is.EqualTo(expectedPdbId.Stamp));
			}
		}

		[Test]
		public void EmbedSourceFiles_False_Omits_Embedded_Source()
		{
			// Generating a PDB alongside a project export (where the .cs are already on disk) should be
			// able to skip embedding the source again. EmbedSourceFiles = false must drop the
			// EmbeddedSource custom debug info (smaller PDB, no embeddedSourceLength) while keeping the
			// document checksum/hash intact.

			// Compile HelloWorld's source into a uniquely-named assembly so this test never collides
			// with the HelloWorld fixture's .expected files under the fixture's parallel scope.
			string sourceFile = Path.Combine(TestCasePath, nameof(HelloWorld) + ".cs");
			string outputBase = Path.Combine(TestCasePath, nameof(EmbedSourceFiles_False_Omits_Embedded_Source) + ".expected");
			CompileCSharpWithPdb(outputBase, sourceFile);
			string peFileName = outputBase + ".dll";

			var module = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false,
				module.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);

			byte[] WritePdbBytes(bool embedSources)
			{
				var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
				using var ms = new MemoryStream();
				new PortablePdbWriter { NoLogo = true, EmbedSourceFiles = embedSources }
					.WritePdb(module, decompiler, new DecompilerSettings(), ms);
				return ms.ToArray();
			}

			// Inspect the portable PDB metadata directly (environment-independent): count the
			// EmbeddedSource custom-debug-information rows and the source documents.
			(int EmbeddedSources, int Documents) Inspect(byte[] pdb)
			{
				using var ms = new MemoryStream(pdb);
				var reader = MetadataReaderProvider.FromPortablePdbStream(ms).GetMetadataReader();
				int embedded = 0;
				foreach (var handle in reader.CustomDebugInformation)
				{
					var cdi = reader.GetCustomDebugInformation(handle);
					if (reader.GetGuid(cdi.Kind) == KnownGuids.EmbeddedSource)
						embedded++;
				}
				return (embedded, reader.Documents.Count);
			}

			var withEmbed = WritePdbBytes(embedSources: true);
			var withoutEmbed = WritePdbBytes(embedSources: false);

			var embedInfo = Inspect(withEmbed);
			var noEmbedInfo = Inspect(withoutEmbed);

			Assert.That(embedInfo.EmbeddedSources, Is.GreaterThan(0),
				"the default (embed = true) keeps the embedded-source blobs");
			Assert.That(noEmbedInfo.EmbeddedSources, Is.EqualTo(0),
				"embed = false must omit every embedded-source blob");
			Assert.That(noEmbedInfo.Documents, Is.EqualTo(embedInfo.Documents).And.GreaterThan(0),
				"the source documents (with their checksum/hash) must remain even without embedded source");
			Assert.That(withoutEmbed.Length, Is.LessThan(withEmbed.Length),
				"omitting embedded source must produce a strictly smaller PDB");
		}

		[Test]
		public void ProgressReporting()
		{
			// Generate a PDB for an assembly and validate that the progress reporter is called with reasonable values
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(ProgressReporting));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());

			var lastFilesWritten = 0;
			var totalFiles = -1;

			Action<DecompilationProgress> reportFunc = progress => {
				if (totalFiles == -1)
				{
					// Initialize value on first call
					totalFiles = progress.TotalUnits;
				}

				Assert.That(totalFiles, Is.EqualTo(progress.TotalUnits));
				Assert.That(lastFilesWritten + 1, Is.EqualTo(progress.UnitsCompleted));

				lastFilesWritten = progress.UnitsCompleted;
			};

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(ProgressReporting) + ".pdb"), FileMode.Create, FileAccess.ReadWrite))
			{
				new PortablePdbWriter { NoLogo = true, Progress = new TestProgressReporter(reportFunc) }
					.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream);

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);
			}

			Assert.That(lastFilesWritten, Is.EqualTo(totalFiles));
		}

		private class TestProgressReporter : IProgress<DecompilationProgress>
		{
			private Action<DecompilationProgress> reportFunc;

			public TestProgressReporter(Action<DecompilationProgress> reportFunc)
			{
				this.reportFunc = reportFunc;
			}

			public void Report(DecompilationProgress value)
			{
				reportFunc(value);
			}
		}

		/// <summary>
		/// Compiles the fixture with the C# compiler (producing a real PDB used as the oracle),
		/// decompiles it and reconstructs a PDB with <see cref="PortablePdbWriter"/>, then compares
		/// the two PDBs' breakpoint maps. The decompiler reconstructs IL ranges and local scopes
		/// differently from the compiler, so the comparison asserts on source locations and hidden
		/// point placement without depending on exact IL offsets.
		/// </summary>
		/// <param name="tolerance">
		/// Whether visible breakpoints must match on column as well as line.
		/// </param>
		/// <param name="knownResidual">
		/// When false, the breakpoint map must match the compiler exactly (after the projection
		/// above). When true, the residual difference must match the committed
		/// <c>&lt;testName&gt;.residual.txt</c> snapshot - this pins the known imperfections the
		/// decompiler cannot yet reproduce, so an improvement or a regression both flip the test and
		/// prompt a deliberate snapshot update (the same accept-the-diff workflow as the pretty tests).
		/// </param>
		private void TestSequencePoints(Tolerance tolerance = Tolerance.LinesAndColumns, bool knownResidual = false,
			[CallerMemberName] string testName = null)
		{
			(string peFileName, string pdbFileName) = CompileTestCase(testName);

			var module = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, module.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());

			using var generatedPdb = new MemoryStream();
			new PortablePdbWriter { NoLogo = true }
				.WritePdb(module, decompiler, new DecompilerSettings(), generatedPdb);

			var methodNames = PdbSequencePoints.ReadMethodNames(peFileName);
			var actual = PdbSequencePoints.Read(generatedPdb);
			Dictionary<int, List<PdbSequencePoints.RawSequencePoint>> expected;
			using (var compilerPdb = File.OpenRead(pdbFileName))
				expected = PdbSequencePoints.Read(compilerPdb);

			// Oracle-free structural check on the decompiler's own PDB.
			string wellFormed = PdbSequencePoints.CheckWellFormed(actual, methodNames);
			Assert.That(wellFormed, Is.Empty, "the reconstructed PDB is not well-formed:\n" + wellFormed);

			string residual = PdbSequencePoints.CompareBreakpointMaps(
				expected, actual, methodNames,
				includeColumns: tolerance == Tolerance.LinesAndColumns, includeHidden: true);

			if (knownResidual)
			{
				string snapshotFile = Path.Combine(TestCasePath, testName + ".residual.txt");
				string expectedResidual = File.Exists(snapshotFile)
					? File.ReadAllText(snapshotFile).Replace("\r\n", "\n")
					: "";
				if (residual != expectedResidual)
				{
					// Setting ILSPY_ACCEPT_PDB_RESIDUAL=1 accepts the new residual in place, so a
					// deliberate change is committed by re-running the test with the variable set
					// rather than by hand-copying files.
					if (Environment.GetEnvironmentVariable("ILSPY_ACCEPT_PDB_RESIDUAL") == "1")
					{
						File.WriteAllText(snapshotFile, residual);
					}
					else
					{
						File.WriteAllText(snapshotFile + ".generated", residual);
						Assert.Fail($"the breakpoint-map residual changed; review the difference and, if intended, "
							+ $"accept it by re-running this test with ILSPY_ACCEPT_PDB_RESIDUAL=1 set (or copy "
							+ $"'{testName}.residual.txt.generated' over '{testName}.residual.txt')."
							+ $"\n\nExpected:\n{expectedResidual}\nActual:\n{residual}");
					}
				}
			}
			else
			{
				Assert.That(residual, Is.Empty, "the reconstructed breakpoint map differs from the compiler's:\n" + residual);
			}
		}

		private static void CompileCSharpWithPdb(string outputBase, string sourceFile)
		{
			Tester.CompileCSharpWithPdb(outputBase, new Dictionary<string, string> {
				{ Path.GetFileName(sourceFile), File.ReadAllText(sourceFile) }
			});
		}

		private (string peFileName, string pdbFileName) CompileTestCase(string testName)
		{
			string sourceFile = Path.Combine(TestCasePath, testName + ".cs");
			CompileCSharpWithPdb(Path.Combine(TestCasePath, testName + ".expected"), sourceFile);

			string peFileName = Path.Combine(TestCasePath, testName + ".expected.dll");
			string pdbFileName = Path.Combine(TestCasePath, testName + ".expected.pdb");

			return (peFileName, pdbFileName);
		}
	}
}
