// Copyright (c) 2023 Siegfried Pammer
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.XPath;

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ICSharpCode.Decompiler.Tests.Util
{
	[TestFixture]
	public class ResourceReaderWriterTests
	{
		const string WinFormsAssemblyName = ", System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		const string MSCorLibAssemblyName = ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

		[Serializable]
		public class SerializableClass
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}

		static readonly object[][] TestWriteCases = {
			new object[] { "Decimal", 1.0m, "1.0", "System.Decimal" + MSCorLibAssemblyName },
			new object[] { "TimeSpan", TimeSpan.FromSeconds(42), "00:00:42", "System.TimeSpan" + MSCorLibAssemblyName },
			new object[] { "DateTime", DateTime.Parse("06/18/2023 21:36:30", CultureInfo.InvariantCulture), "06/18/2023 21:36:30", "System.DateTime" + MSCorLibAssemblyName },
		};

		static readonly object[][] TestReadCases = {
			new object[] { "Decimal", 1.0m },
			new object[] { "TimeSpan", TimeSpan.FromSeconds(42) },
			new object[] { "DateTime", DateTime.Parse("06/18/2023 21:36:30", CultureInfo.InvariantCulture) },
		};

		static MemoryStream ProduceResourcesTestFile<T>(string name, T value)
		{
			var ms = new MemoryStream();
			var writer = new ResourceWriter(ms);
			writer.AddResource(name, value);
			writer.Generate();
			ms.Position = 0;
			return ms;
		}

		static XElement ProduceResXTest<T>(string name, T value)
		{
			using var ms = new MemoryStream();
			var writer = new ResXResourceWriter(ms);
			writer.AddResource(name, value);
			writer.Generate();
			ms.Position = 0;
			var doc = XDocument.Load(ms);
			return doc.XPathSelectElement(".//data");
		}

		[TestCase("Null", null)]
		[TestCase("String", "Hello World!")]
		[TestCase("Char", 'A')]
		[TestCase("Bool", true)]
		[TestCase("Bool", false)]
		[TestCase("Byte", (byte)1)]
		[TestCase("SByte", (sbyte)-1)]
		[TestCase("Int16", (short)1)]
		[TestCase("UInt16", (ushort)1)]
		[TestCase("Int32", 1)]
		[TestCase("UInt32", (uint)1)]
		[TestCase("Int64", (long)1)]
		[TestCase("UInt64", (ulong)1)]
		[TestCase("Single", 1.0f)]
		[TestCase("Double", 1.0d)]
		[TestCase("Bytes", new byte[] { 42, 43, 44 })]
		[TestCaseSource(nameof(TestReadCases))]
		public void Read(string name, object value)
		{
			using var testFile = ProduceResourcesTestFile(name, value);
			using var reader = new ResourcesFile(testFile);
			var items = reader.ToArray();
			Assert.That(items.Length, Is.EqualTo(1));
			Assert.That(items[0].Key, Is.EqualTo(name));
			Assert.That(items[0].Value, Is.EqualTo(value));
		}

		[TestCase("Null", null, null, "System.Resources.ResXNullRef" + WinFormsAssemblyName)]
		[TestCase("String", "Hello World!", "Hello World!", null)]
		[TestCase("Bool", true, "True", "System.Boolean" + MSCorLibAssemblyName)]
		[TestCase("Bool", false, "False", "System.Boolean" + MSCorLibAssemblyName)]
		[TestCase("Char", 'A', "A", "System.Char" + MSCorLibAssemblyName)]
		[TestCase("Byte", (byte)1, "1", "System.Byte" + MSCorLibAssemblyName)]
		[TestCase("SByte", (sbyte)-1, "-1", "System.SByte" + MSCorLibAssemblyName)]
		[TestCase("Int16", (short)1, "1", "System.Int16" + MSCorLibAssemblyName)]
		[TestCase("UInt16", (ushort)1, "1", "System.UInt16" + MSCorLibAssemblyName)]
		[TestCase("Int32", 1, "1", "System.Int32" + MSCorLibAssemblyName)]
		[TestCase("UInt32", (uint)1, "1", "System.UInt32" + MSCorLibAssemblyName)]
		[TestCase("Int64", (long)1, "1", "System.Int64" + MSCorLibAssemblyName)]
		[TestCase("UInt64", (ulong)1, "1", "System.UInt64" + MSCorLibAssemblyName)]
		[TestCase("Single", 1.0f, "1", "System.Single" + MSCorLibAssemblyName)]
		[TestCase("Double", 1.0d, "1", "System.Double" + MSCorLibAssemblyName)]
		[TestCaseSource(nameof(TestWriteCases))]
		public void Write(string name, object value, string serializedValue, string typeName)
		{
			var element = ProduceResXTest(name, value);
			Assert.That(element.Attribute("name")?.Value, Is.EqualTo(name));
			if (typeName != null)
			{
				Assert.That(element.Attribute("type")?.Value, Is.EqualTo(typeName));
			}
			var v = element.Element("value");
			Assert.That(v, Is.Not.Null);
			Assert.That(v.IsEmpty ? serializedValue == null : v.Value == serializedValue);
		}

		[Test]
		public void ResXSerializableClassIsRejected()
		{
			Assert.Throws<NotSupportedException>(
				() => ProduceResXTest("Serial", new SerializableClass { Name = "Hugo", Age = 42 })
			);
		}

		[Test]
		public void BitmapIsResourceSerializedObject()
		{
			Stream stream = typeof(ResourceReaderWriterTests).Assembly
				.GetManifestResourceStream(typeof(ResourceReaderWriterTests).Namespace + ".Test.resources");
			using var reader = new ResourcesFile(stream);
			var items = reader.ToArray();
			Assert.That(items.Length, Is.EqualTo(3));
			var item = items.FirstOrDefault(i => i.Key == "Bitmap");
			Assert.That(item.Key, Is.Not.Null);
			Assert.That(item.Value, Is.InstanceOf<ResourceSerializedObject>());
		}

		[Test]
		public void ByteArrayIsSupported()
		{
			Stream stream = typeof(ResourceReaderWriterTests).Assembly
				.GetManifestResourceStream(typeof(ResourceReaderWriterTests).Namespace + ".Test.resources");
			using var reader = new ResourcesFile(stream);
			var items = reader.ToArray();
			Assert.That(items.Length, Is.EqualTo(3));
			var item = items.FirstOrDefault(i => i.Key == "Byte[]");
			Assert.That(item.Key, Is.Not.Null);
			Assert.That(item.Value, Is.InstanceOf<byte[]>());
			byte[] array = (byte[])item.Value;
			Assert.That(array.Length, Is.EqualTo(3));
			Assert.That(array[0], Is.EqualTo(42));
			Assert.That(array[1], Is.EqualTo(43));
			Assert.That(array[2], Is.EqualTo(44));
		}

		[Test]
		public void MemoryStreamIsSupported()
		{
			Stream stream = typeof(ResourceReaderWriterTests).Assembly
				.GetManifestResourceStream(typeof(ResourceReaderWriterTests).Namespace + ".Test.resources");
			using var reader = new ResourcesFile(stream);
			var items = reader.ToArray();
			Assert.That(items.Length, Is.EqualTo(3));
			var item = items.FirstOrDefault(i => i.Key == "MemoryStream");
			Assert.That(item.Key, Is.Not.Null);
			Assert.That(item.Value, Is.InstanceOf<MemoryStream>());
		}
	}
}