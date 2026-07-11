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
using System.Threading.Tasks;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// Verifies that the BAML decompiler survives when the well-known WPF assemblies cannot be
	/// resolved - the situation on any machine without WPF installed (Linux/macOS). Previously
	/// <see cref="KnownThings"/> threw because it could not find PresentationFramework et al.;
	/// now those assemblies are substituted by synthetic stand-ins.
	/// </summary>
	[TestFixture]
	public class MissingReferencesTests
	{
		const string PresentationXmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		/// <summary>
		/// An assembly resolver that hides the WPF assemblies, reproducing the "no WPF available"
		/// environment deterministically on every platform (on Windows the real assemblies would
		/// otherwise be found in the runtime pack).
		/// </summary>
		sealed class WpfHidingResolver : IAssemblyResolver
		{
			static readonly HashSet<string> hidden = new(StringComparer.OrdinalIgnoreCase) {
				"WindowsBase", "PresentationCore", "PresentationFramework", "PresentationUI", "System.Xaml"
			};

			readonly IAssemblyResolver inner;

			public WpfHidingResolver(IAssemblyResolver inner) => this.inner = inner;

			public MetadataFile Resolve(IAssemblyReference reference)
				=> hidden.Contains(reference.Name) ? null : inner.Resolve(reference);

			public MetadataFile ResolveModule(MetadataFile mainModule, string moduleName)
				=> inner.ResolveModule(mainModule, moduleName);

			public Task<MetadataFile> ResolveAsync(IAssemblyReference reference)
				=> hidden.Contains(reference.Name) ? Task.FromResult<MetadataFile>(null) : inner.ResolveAsync(reference);

			public Task<MetadataFile> ResolveModuleAsync(MetadataFile mainModule, string moduleName)
				=> inner.ResolveModuleAsync(mainModule, moduleName);
		}

		static BamlDecompilerTypeSystem CreateTypeSystemWithoutWpf()
		{
			// This test assembly is a managed module that does not itself reference WPF; combined
			// with the WPF-hiding resolver, none of the well-known WPF assemblies resolve.
			var location = typeof(MissingReferencesTests).Assembly.Location;
			// PrefetchEntireImage reads the whole image into memory, so the file stream is only
			// needed while the PEFile is being constructed and can be closed right afterwards.
			using var stream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, stream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new WpfHidingResolver(new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack()));
			return new BamlDecompilerTypeSystem(file, resolver);
		}

		[Test]
		public void KnownThings_DoesNotThrow_WhenWpfAssembliesAreUnavailable()
		{
			var typeSystem = CreateTypeSystemWithoutWpf();
			Assert.DoesNotThrow(() => new KnownThings(typeSystem));
		}

		[Test]
		public void KnownType_ResolvesToSyntheticDefinition_WithCorrectIdentity()
		{
			var knownThings = new KnownThings(CreateTypeSystemWithoutWpf());

			var button = knownThings.Types(KnownTypes.Button);

			Assert.Multiple(() => {
				Assert.That(button.Namespace, Is.EqualTo("System.Windows.Controls"));
				Assert.That(button.Name, Is.EqualTo("Button"));
				Assert.That(button.ParentModule.AssemblyName, Is.EqualTo("PresentationFramework"));
			});
		}

		[Test]
		public void SyntheticModule_IsBounded_ReturnsNullForUnseededType()
		{
			var knownThings = new KnownThings(CreateTypeSystemWithoutWpf());
			var module = knownThings.Types(KnownTypes.Button).ParentModule;

			// A type the decompiler never seeded must stay unresolved, so that references to
			// non-well-known WPF types keep degrading to UnknownType exactly as before.
			var unseeded = module.GetTypeDefinition(
				new TopLevelTypeName("System.Windows.Controls", "ThisTypeDoesNotExist"));

			Assert.That(unseeded, Is.Null);
		}

		[Test]
		public void SyntheticModule_ExposesPresentationXmlnsDefinition()
		{
			var knownThings = new KnownThings(CreateTypeSystemWithoutWpf());
			var module = knownThings.Types(KnownTypes.Button).ParentModule;

			var controlsMapping = module.GetAssemblyAttributes()
				.Where(a => a.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute")
				.FirstOrDefault(a => (string)a.FixedArguments[1].Value == "System.Windows.Controls");

			Assert.That(controlsMapping, Is.Not.Null, "expected an XmlnsDefinition for System.Windows.Controls");
			Assert.That((string)controlsMapping.FixedArguments[0].Value, Is.EqualTo(PresentationXmlns));
		}
	}
}
