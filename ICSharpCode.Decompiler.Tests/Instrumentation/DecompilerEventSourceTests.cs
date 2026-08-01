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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

using System.IO;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Instrumentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Instrumentation
{
	[TestFixture]
	public class DecompilerEventSourceTests
	{
		/// <summary>
		/// Captures all events the "ICSharpCode.Decompiler" provider emits while the listener
		/// is alive. Events are recorded process-wide, so assertions must filter by payload
		/// (e.g. the decompiled type's full name) to stay robust under parallel test runs.
		/// </summary>
		sealed class RecordingListener : EventListener
		{
			readonly ConcurrentQueue<(string EventName, Dictionary<string, object?> Payload)> events = new();

			public RecordingListener(EventLevel level, EventKeywords keywords)
			{
				EnableEvents(DecompilerEventSource.Log, level, keywords);
			}

			protected override void OnEventWritten(EventWrittenEventArgs eventData)
			{
				var payload = new Dictionary<string, object?>();
				if (eventData.PayloadNames != null && eventData.Payload != null)
				{
					for (int i = 0; i < eventData.PayloadNames.Count; i++)
					{
						payload[eventData.PayloadNames[i]] = eventData.Payload[i];
					}
				}
				events.Enqueue((eventData.EventName ?? "", payload));
			}

			public List<Dictionary<string, object?>> EventsNamed(string eventName)
			{
				return events.Where(e => e.EventName == eventName).Select(e => e.Payload).ToList();
			}

			public List<string> EventNames => events.Select(e => e.EventName).Distinct().ToList();
		}

		/// <summary>Sample decompilation input covering all member kinds.</summary>
		class SampleDecompilationTarget
		{
			public int field;

			public int Property { get; set; }

			public event EventHandler? Event;

			public int Method(int x)
			{
				if (x > 0)
				{
					Event?.Invoke(this, EventArgs.Empty);
					return x + field;
				}
				return -x;
			}
		}

		static CSharpDecompiler CreateDecompilerForTestAssembly(out PEFile module)
		{
			module = new PEFile(typeof(DecompilerEventSourceTests).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			return new CSharpDecompiler(module, resolver, new DecompilerSettings());
		}

		static void DecompileSampleTarget()
		{
			var decompiler = CreateDecompilerForTestAssembly(out var module);
			using (module)
			{
				decompiler.DecompileType(new FullTypeName(typeof(SampleDecompilationTarget).FullName));
			}
		}

		[Test]
		public void ManifestIsValid()
		{
			string manifest = EventSource.GenerateManifest(typeof(DecompilerEventSource), typeof(DecompilerEventSource).Assembly.Location, EventManifestOptions.Strict);
			Assert.That(manifest, Is.Not.Null.And.Not.Empty);
		}

		[Test]
		public void FiringEveryEventProducesNoEventSourceErrors()
		{
			// The provider is process-wide and other fixtures decompile in parallel, so every
			// string payload carries this marker and the count assertions filter on it.
			const string marker = "SchemaSmokeTest.";
			using var listener = new RecordingListener(EventLevel.Verbose, EventKeywords.All);
			var log = DecompilerEventSource.Log;
			log.DecompileTypeStart(marker + "Type");
			log.DecompileTypeStop(marker + "Type");
			log.DecompileMemberStart(marker + "Type.M", 0x06000001, (int)DecompiledMemberKind.Method, 42);
			log.DecompileMemberStop(marker + "Type.M", 0x06000001, (int)DecompiledMemberKind.Method);
			log.TypeSystemInitStart(marker + "module");
			log.TypeSystemInitStop(marker + "module", 3);
			log.AssemblyResolveStart(marker + "Reference");
			log.AssemblyResolveStop(marker + "Reference", marker + "Reference.dll", true);
			log.ProjectDecompilationStart(marker + "module");
			log.ProjectDecompilationStop(marker + "module", 10, 2);
			log.ProjectFileStart(marker + "File.cs", 5);
			log.ProjectFileStop(marker + "File.cs");
			log.ILTransformExecuted(marker + "ILInlining", 0x06000001, 0.5);
			log.AstTransformExecuted(marker + "PatternStatementTransform", 1.5);

			// A mismatch between an [Event] method's signature and its WriteEvent call surfaces
			// as an "EventSourceMessage" error event on the same provider.
			Assert.That(listener.EventsNamed("EventSourceMessage"), Is.Empty);

			string[] expected = {
				"DecompileTypeStart", "DecompileTypeStop",
				"DecompileMemberStart", "DecompileMemberStop",
				"TypeSystemInitStart", "TypeSystemInitStop",
				"AssemblyResolveStart", "AssemblyResolveStop",
				"ProjectDecompilationStart", "ProjectDecompilationStop",
				"ProjectFileStart", "ProjectFileStop",
				"ILTransformExecuted", "AstTransformExecuted",
			};
			foreach (string eventName in expected)
			{
				var markedEvents = listener.EventsNamed(eventName)
					.Where(p => p.Values.OfType<string>().Any(v => v.StartsWith(marker, StringComparison.Ordinal)))
					.ToList();
				Assert.That(markedEvents, Has.Count.EqualTo(1), eventName);
			}
		}

		[Test]
		public void DecompilingTypeEmitsPairedStartStopEvents()
		{
			using var listener = new RecordingListener(EventLevel.Informational, DecompilerEventSource.Keywords.Decompilation);
			DecompileSampleTarget();

			var typeStarts = listener.EventsNamed("DecompileTypeStart")
				.Where(p => ((string?)p["fullName"])?.Contains(nameof(SampleDecompilationTarget)) == true).ToList();
			var typeStops = listener.EventsNamed("DecompileTypeStop")
				.Where(p => ((string?)p["fullName"])?.Contains(nameof(SampleDecompilationTarget)) == true).ToList();
			Assert.That(typeStarts, Has.Count.EqualTo(1));
			Assert.That(typeStops, Has.Count.EqualTo(1));

			var memberStarts = listener.EventsNamed("DecompileMemberStart")
				.Where(p => ((string?)p["fullName"])?.Contains(nameof(SampleDecompilationTarget)) == true).ToList();
			var memberStops = listener.EventsNamed("DecompileMemberStop")
				.Where(p => ((string?)p["fullName"])?.Contains(nameof(SampleDecompilationTarget)) == true).ToList();
			Assert.That(memberStarts, Is.Not.Empty);
			Assert.That(memberStops, Has.Count.EqualTo(memberStarts.Count));

			var kinds = memberStarts.Select(p => (int)p["memberKind"]!).Distinct().ToList();
			Assert.That(kinds, Is.SubsetOf(new[] {
				(int)DecompiledMemberKind.Method,
				(int)DecompiledMemberKind.Field,
				(int)DecompiledMemberKind.Property,
				(int)DecompiledMemberKind.Event,
			}));
			// The sample type has at least one of each member kind.
			Assert.That(kinds, Has.Count.EqualTo(4));
			Assert.That(memberStarts.Select(p => (int)p["metadataToken"]!), Has.All.Not.EqualTo(0));

			var methodStarts = memberStarts.Where(p => (int)p["memberKind"]! == (int)DecompiledMemberKind.Method).ToList();
			Assert.That(methodStarts.Select(p => (int)p["ilBodySize"]!), Has.Some.GreaterThan(0));

			// Per-transform events require the Transforms keyword at Verbose level.
			Assert.That(listener.EventsNamed("ILTransformExecuted"), Is.Empty);
			Assert.That(listener.EventsNamed("AstTransformExecuted"), Is.Empty);
		}

		[Test]
		public void CreatingTypeSystemEmitsTypeSystemAndResolveEvents()
		{
			using var listener = new RecordingListener(EventLevel.Informational,
				DecompilerEventSource.Keywords.TypeSystem | DecompilerEventSource.Keywords.AssemblyResolver);
			using var module = new PEFile(typeof(DecompilerEventSourceTests).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			_ = new DecompilerTypeSystem(module, resolver);

			// Other fixtures may build type systems concurrently (events are process-wide),
			// so assert presence rather than exact global counts.
			var initStarts = listener.EventsNamed("TypeSystemInitStart")
				.Where(p => (string?)p["moduleName"] == module.Name).ToList();
			var initStops = listener.EventsNamed("TypeSystemInitStop")
				.Where(p => (string?)p["moduleName"] == module.Name).ToList();
			Assert.That(initStarts, Is.Not.Empty);
			Assert.That(initStops, Is.Not.Empty);
			Assert.That(initStops.Select(p => (int)p["referencedAssembliesResolved"]!), Has.Some.GreaterThan(0));

			var resolveStarts = listener.EventsNamed("AssemblyResolveStart")
				.Where(p => ((string?)p["referenceName"])?.StartsWith("System.Runtime,", StringComparison.Ordinal) == true).ToList();
			var resolveStops = listener.EventsNamed("AssemblyResolveStop")
				.Where(p => ((string?)p["referenceName"])?.StartsWith("System.Runtime,", StringComparison.Ordinal) == true).ToList();
			Assert.That(resolveStarts, Is.Not.Empty);
			Assert.That(resolveStops, Is.Not.Empty);
			Assert.That(resolveStops.Where(p => (bool)p["success"]! && !string.IsNullOrEmpty((string?)p["resolvedPath"])),
				Is.Not.Empty, "System.Runtime must resolve to a file on disk");
		}

		[Test]
		public void WholeProjectDecompilationEmitsProjectEvents()
		{
			string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempDir);
			try
			{
				string dllPath = Path.Combine(tempDir, "TraceTestAssembly.dll");
				CompileTinyAssembly(dllPath);
				string projectDir = Path.Combine(tempDir, "project");
				Directory.CreateDirectory(projectDir);

				using var listener = new RecordingListener(EventLevel.Informational, DecompilerEventSource.Keywords.ProjectDecompiler);
				using (var module = new PEFile(dllPath))
				{
					var resolver = new UniversalAssemblyResolver(dllPath, false, module.Metadata.DetectTargetFrameworkId());
					new WholeProjectDecompiler(resolver).DecompileProject(module, projectDir);
				}

				var runStarts = listener.EventsNamed("ProjectDecompilationStart")
					.Where(p => ((string?)p["moduleName"])?.Contains("TraceTestAssembly") == true).ToList();
				var runStops = listener.EventsNamed("ProjectDecompilationStop")
					.Where(p => ((string?)p["moduleName"])?.Contains("TraceTestAssembly") == true).ToList();
				Assert.That(runStarts, Has.Count.EqualTo(1));
				Assert.That(runStops, Has.Count.EqualTo(1));
				Assert.That((int)runStops[0]["codeFileCount"]!, Is.GreaterThanOrEqualTo(2));

				string[] expectedFiles = { "TraceTestClassA.cs", "TraceTestClassB.cs" };
				var fileStarts = listener.EventsNamed("ProjectFileStart")
					.Where(p => expectedFiles.Contains((string?)p["fileName"])).ToList();
				var fileStops = listener.EventsNamed("ProjectFileStop")
					.Where(p => expectedFiles.Contains((string?)p["fileName"])).ToList();
				Assert.That(fileStarts, Has.Count.EqualTo(2));
				Assert.That(fileStops, Has.Count.EqualTo(2));
				Assert.That(fileStarts.Select(p => (int)p["typeCount"]!), Has.All.EqualTo(1));
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		static void CompileTinyAssembly(string dllPath)
		{
			const string source = @"
public class TraceTestClassA { public int M(int x) { return x + 1; } }
public class TraceTestClassB { public string N() { return ""b""; } }";
			string runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
			var compilation = CSharpCompilation.Create("TraceTestAssembly",
				new[] { CSharpSyntaxTree.ParseText(source) },
				new[] {
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
					MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll")),
				},
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var result = compilation.Emit(dllPath);
			Assert.That(result.Success, Is.True,
				string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString())));
		}

		[Test]
		public void TransformsKeywordEmitsPerTransformEvents()
		{
			using var listener = new RecordingListener(EventLevel.Verbose, DecompilerEventSource.Keywords.Transforms);
			DecompileSampleTarget();

			var ilTransforms = listener.EventsNamed("ILTransformExecuted");
			Assert.That(ilTransforms, Is.Not.Empty);
			Assert.That(ilTransforms.Select(p => (string?)p["transformName"]), Has.All.Not.Empty);
			Assert.That(ilTransforms.Select(p => (string?)p["transformName"]), Has.Some.EqualTo("ILInlining"));
			Assert.That(ilTransforms.Select(p => (int)p["methodToken"]!), Has.All.Not.EqualTo(0));
			Assert.That(ilTransforms.Select(p => (double)p["elapsedMs"]!), Has.All.GreaterThanOrEqualTo(0.0));

			var astTransforms = listener.EventsNamed("AstTransformExecuted");
			Assert.That(astTransforms, Is.Not.Empty);
			Assert.That(astTransforms.Select(p => (string?)p["transformName"]), Has.Some.EqualTo("PatternStatementTransform"));

			// The Transforms keyword alone must not enable the per-type/per-member events.
			Assert.That(listener.EventsNamed("DecompileTypeStart"), Is.Empty);
			Assert.That(listener.EventsNamed("DecompileMemberStart"), Is.Empty);
		}
	}
}
