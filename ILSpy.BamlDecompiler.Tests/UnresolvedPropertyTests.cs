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
using System.Reflection.PortableExecutable;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// A property is written as "Type.Property" only when the element does not have it as one of
	/// its own - the attached-property syntax. Deciding that needs the property resolved, and a
	/// property of an assembly the session cannot load never resolves; every property of such a
	/// type then went out attached, which the XAML parser rejects for a property that has no
	/// attached accessors (issue #3316).
	/// </summary>
	[TestFixture]
	public class UnresolvedPropertyTests
	{
		const string MissingAssemblyFullName =
			"Microsoft.Expression.Interactions, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
		const string BehaviorTypeFullName = "Microsoft.Expression.Interactivity.Core.DataStateBehavior";
		const string BaseTypeFullName = "Microsoft.Expression.Interactivity.Core.TriggerAction";

		static ushort TypeId(KnownTypes type) => unchecked((ushort)-(short)type);

		static MemoryStream CreateBaml(params BamlRecord[] records)
		{
			var version = new BamlDocument.BamlVersion { Major = 0, Minor = 0x60 };
			var document = new BamlDocument {
				Signature = "MSBAML",
				ReaderVersion = version,
				UpdaterVersion = version,
				WriterVersion = version
			};
			document.Add(new DocumentStartRecord());
			document.AddRange(records);
			document.Add(new DocumentEndRecord());

			var stream = new MemoryStream();
			BamlWriter.WriteDocument(document, stream);
			stream.Position = 0;
			return stream;
		}

		static string Decompile(Stream baml)
		{
			var location = typeof(UnresolvedPropertyTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		/// <summary>
		/// Builds a document that puts an element of an assembly nothing can resolve under a
		/// Button, and sets one property on it. <paramref name="ownerTypeId"/> selects the type the
		/// document names as the owner of that property: the element's own type, or another type of
		/// the same unresolvable assembly.
		/// </summary>
		static string DecompileElementWithProperty(ushort ownerTypeId)
		{
			return Decompile(CreateBaml(
				new AssemblyInfoRecord { AssemblyId = 0, AssemblyFullName = MissingAssemblyFullName },
				new TypeInfoRecord { TypeId = 0, AssemblyId = 0, TypeFullName = BehaviorTypeFullName },
				new TypeInfoRecord { TypeId = 1, AssemblyId = 0, TypeFullName = BaseTypeFullName },
				new AttributeInfoRecord { AttributeId = 0, OwnerTypeId = ownerTypeId, Name = "TrueState" },
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new ElementStartRecord { TypeId = 0 },
				new PropertyRecord { AttributeId = 0, Value = "True" },
				new ElementEndRecord(),
				new ElementEndRecord()));
		}

		[Test]
		public void APropertyOfTheElementsOwnTypeIsNotWrittenAttached()
		{
			string xaml = DecompileElementWithProperty(ownerTypeId: 0);

			Assert.That(xaml, Does.Contain("TrueState=\"True\""), xaml);
			Assert.That(xaml, Does.Not.Contain("DataStateBehavior.TrueState"), xaml);
		}

		[Test]
		public void APropertyOfAnotherTypeStaysAttached()
		{
			// The document naming a different owner is what attached-property syntax records, and
			// it stays qualified even though neither type resolves.
			string xaml = DecompileElementWithProperty(ownerTypeId: 1);

			Assert.That(xaml, Does.Contain("TriggerAction.TrueState=\"True\""), xaml);
		}
	}
}
