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
using System.Reflection;
using System.Reflection.PortableExecutable;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.BamlDecompiler.Handlers;
using ICSharpCode.BamlDecompiler.Xaml;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// A type of the assembly being decompiled with a CLR property of its own called "Name".
	/// </summary>
	public class HelperWithItsOwnName
	{
		public string Name { get; set; }
	}

	/// <summary>
	/// x:Name is the directive that registers a name for an element, and it is recorded as the
	/// runtime name property of that element - FrameworkElement.Name for everything WPF, a property
	/// of the framework rather than of the assembly being decompiled. Writing the directive for a
	/// type's own property of the same name registers a name and leaves the property unset, which
	/// still compiles and silently means something else (issue #2253).
	/// </summary>
	[TestFixture]
	public class RuntimeNamePropertyTests
	{
		static ICompilation compilation;

		[OneTimeSetUp]
		public void LoadTestAssembly()
		{
			string location = typeof(RuntimeNamePropertyTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			compilation = new BamlDecompilerTypeSystem(file, resolver);
		}

		static XamlType XamlTypeOf(Type type)
		{
			var definition = compilation.FindType(type).GetDefinition();
			return new XamlType(definition.ParentModule, definition.ParentModule.FullAssemblyName,
				definition.Namespace, definition.Name) {
				ResolvedType = definition
			};
		}

		/// <summary>
		/// A property of <paramref name="declaringType"/>, the way the decompiler resolves one from
		/// the type a BAML attribute record names as its owner.
		/// </summary>
		static XamlProperty PropertyOf(Type declaringType, string propertyName)
		{
			var declaring = XamlTypeOf(declaringType);
			return new XamlProperty(declaring, propertyName) {
				ResolvedMember = declaring.ResolvedType.GetDefinition()
					.GetProperties(p => p.Name == propertyName).FirstOrDefault()
			};
		}

		[Test]
		public void ATypesOwnNamePropertyIsNotTheRuntimeName()
		{
			// <local:Helper Name="theName" />: the property belongs to the assembly being
			// decompiled, and the value has to reach it.
			var property = PropertyOf(typeof(HelperWithItsOwnName), "Name");

			Assert.That(PropertyHandler.IsRuntimeNameOfElement(property, XamlTypeOf(typeof(HelperWithItsOwnName))),
				Is.False);
		}

		[Test]
		public void ANameInheritedFromTheFrameworkIsTheRuntimeName()
		{
			// <local:MyControl x:Name="theName" />: the document names the control as the owner of
			// the attribute, because that is the element it sits on, but the property comes from
			// the framework type the control derives from.
			var property = new XamlProperty(XamlTypeOf(typeof(HelperWithItsOwnName)), "Name") {
				ResolvedMember = compilation.FindType(typeof(MemberInfo)).GetDefinition()
					.GetProperties(p => p.Name == "Name").First()
			};

			Assert.That(PropertyHandler.IsRuntimeNameOfElement(property, XamlTypeOf(typeof(HelperWithItsOwnName))),
				Is.True);
		}

		[Test]
		public void AnElementFromOutsideTheAssemblyKeepsItsProperty()
		{
			var property = PropertyOf(typeof(Uri), "Name");

			Assert.That(PropertyHandler.IsRuntimeNameOfElement(property, XamlTypeOf(typeof(Uri))), Is.False);
		}

		[Test]
		public void APropertyOfAnotherNameIsNeverTheRuntimeName()
		{
			var property = PropertyOf(typeof(Uri), "Title");

			Assert.That(PropertyHandler.IsRuntimeNameOfElement(property, XamlTypeOf(typeof(HelperWithItsOwnName))),
				Is.False);
		}
	}
}
