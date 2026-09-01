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
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

using ILSpy.BamlDecompiler.Tests;

// This assembly plays the role of an assembly that maps one CLR namespace to two XML namespaces,
// the way PresentationFramework maps its namespaces to both presentation namespaces.
[assembly: System.Windows.Markup.XmlnsDefinition(XmlNamespaceResolutionTests.Winfx2006Presentation, XmlNamespaceResolutionTests.TestClrNamespace)]
[assembly: System.Windows.Markup.XmlnsDefinition(XmlNamespaceResolutionTests.Netfx2007Presentation, XmlNamespaceResolutionTests.TestClrNamespace)]

namespace System.Windows.Markup
{
	/// <summary>
	/// Stand-in for the WPF attribute of the same name, which is unavailable on platforms without
	/// WPF. The BAML decompiler matches it by full name in metadata, so the declaring assembly does
	/// not matter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class XmlnsDefinitionAttribute : Attribute
	{
		public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
		{
			XmlNamespace = xmlNamespace;
			ClrNamespace = clrNamespace;
		}

		public string XmlNamespace { get; }
		public string ClrNamespace { get; }
	}
}

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// Tests for the fallback used when neither the BAML xmlns records nor the PI mappings name an
	/// XML namespace for a type: which of the assembly's XmlnsDefinition mappings is picked.
	/// </summary>
	[TestFixture]
	public class XmlNamespaceResolutionTests
	{
		public const string Winfx2006Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string Netfx2007Presentation = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";
		public const string TestClrNamespace = "ILSpy.BamlDecompiler.Tests";

		static IModule GetTestAssemblyModule()
		{
			var location = typeof(XmlNamespaceResolutionTests).Assembly.Location;
			using var stream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, stream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			return new BamlDecompilerTypeSystem(file, resolver).MainModule;
		}

		[Test]
		public void PrefersPresentationNamespace_WhenDocumentDeclaresNothing()
		{
			var xmlNs = XamlContext.TryGetXmlNamespace(GetTestAssemblyModule(), TestClrNamespace);

			Assert.That(xmlNs, Is.EqualTo(Winfx2006Presentation));
		}

		[Test]
		public void PrefersNamespaceDeclaredByDocument_OverPresentationNamespace()
		{
			// Issue #1688: the document binds the default prefix to the netfx/2007 presentation
			// namespace. Resolving its elements to the winfx/2006 one made the root start tag both
			// declare and redefine the default prefix, which XmlWriter rejects.
			var root = new XElement(XName.Get("Root", Netfx2007Presentation),
				new XAttribute("xmlns", Netfx2007Presentation));

			var xmlNs = XamlContext.TryGetXmlNamespace(GetTestAssemblyModule(), TestClrNamespace, root);

			Assert.That(xmlNs, Is.EqualTo(Netfx2007Presentation));
		}

		[Test]
		public void PrefersNamespaceDeclaredByAncestor()
		{
			var child = new XElement(XName.Get("Child", Netfx2007Presentation));
			new XElement(XName.Get("Root", Netfx2007Presentation),
				new XAttribute("xmlns", Netfx2007Presentation), child);

			var xmlNs = XamlContext.TryGetXmlNamespace(GetTestAssemblyModule(), TestClrNamespace, child);

			Assert.That(xmlNs, Is.EqualTo(Netfx2007Presentation));
		}
	}
}
