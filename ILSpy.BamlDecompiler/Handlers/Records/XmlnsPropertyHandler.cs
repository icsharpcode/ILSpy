/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.BamlDecompiler.Baml;
using ILSpy.BamlDecompiler.Xaml;

namespace ILSpy.BamlDecompiler.Handlers
{
	internal class XmlnsPropertyHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.XmlnsProperty;

		IEnumerable<string> ResolveCLRNamespaces(IModule assembly, string ns)
		{
			foreach (var attr in assembly.GetAssemblyAttributes().Where(a => a.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute"))
			{
				Debug.Assert(attr.FixedArguments.Length == 2);

				var xmlNs = attr.FixedArguments[0].Value;
				var clrNs = attr.FixedArguments[1].Value;
				Debug.Assert(xmlNs is string && clrNs is string);

				if ((string)xmlNs == ns)
					yield return (string)clrNs;
			}
		}

		public BamlElement Translate(XamlContext ctx, BamlNode node, BamlElement parent)
		{
			var record = (XmlnsPropertyRecord)((BamlRecordNode)node).Record;
			foreach (var asmId in record.AssemblyIds)
			{
				var assembly = ctx.Baml.ResolveAssembly(asmId);
				ctx.XmlNs.Add(new NamespaceMap(record.Prefix, assembly.FullAssemblyName, record.XmlNamespace));

				if (assembly.Assembly?.IsMainModule == true)
				{
					foreach (var clrNs in ResolveCLRNamespaces(assembly.Assembly, record.XmlNamespace))
						ctx.XmlNs.Add(new NamespaceMap(record.Prefix, assembly.FullAssemblyName, record.XmlNamespace, clrNs));
				}
			}

			XName xmlnsDef;
			if (string.IsNullOrEmpty(record.Prefix))
				xmlnsDef = "xmlns";
			else
				xmlnsDef = XNamespace.Xmlns + XmlConvert.EncodeLocalName(record.Prefix);
			parent.Xaml.Element.Add(new XAttribute(xmlnsDef, ctx.GetXmlNamespace(record.XmlNamespace)));

			return null;
		}
	}
}