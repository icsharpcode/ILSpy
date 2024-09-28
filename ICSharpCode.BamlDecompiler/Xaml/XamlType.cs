﻿/*
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

using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.BamlDecompiler.Xaml
{
	internal class XamlType
	{
		/// <summary>
		/// Assembly that contains the type defintion. Can be null.
		/// </summary>
		public IModule Assembly { get; }
		public string FullAssemblyName { get; }
		public string TypeNamespace { get; }
		public string TypeName { get; }

		public XNamespace? Namespace { get; private set; }
		public IType ResolvedType { get; set; }

		public XamlType(IModule assembly, string fullAssemblyName, string ns, string name)
			: this(assembly, fullAssemblyName, ns, name, null)
		{
		}

		public XamlType(IModule assembly, string fullAssemblyName, string ns, string name, XNamespace? xmlns)
		{
			Assembly = assembly;
			FullAssemblyName = fullAssemblyName;
			TypeNamespace = ns;
			TypeName = name;
			Namespace = xmlns;
		}

		public void ResolveNamespace(XElement elem, XamlContext ctx)
		{
			if (Namespace != null)
				return;

			// Since XmlnsProperty records are inside the element,
			// the namespace is resolved after processing the element body.

			string? xmlNs = null;
			if (elem.Annotation<XmlnsScope>() != null)
				xmlNs = elem.Annotation<XmlnsScope>().LookupXmlns(FullAssemblyName, TypeNamespace);
			if (xmlNs == null)
				xmlNs = ctx.XmlNs.LookupXmlns(FullAssemblyName, TypeNamespace);
			// Sometimes there's no reference to System.Xaml even if x:Type is used
			if (xmlNs == null)
				xmlNs = ctx.TryGetXmlNamespace(Assembly, TypeNamespace);

			if (xmlNs == null)
			{
				if (FullAssemblyName == ctx.TypeSystem.MainModule.FullAssemblyName)
					xmlNs = $"clr-namespace:{TypeNamespace}";
				else
				{
					var name = ICSharpCode.Decompiler.Metadata.AssemblyNameReference.Parse(FullAssemblyName);
					xmlNs = $"clr-namespace:{TypeNamespace};assembly={name.Name}";
				}

				var nsSeg = TypeNamespace.Split('.');
				var prefix = nsSeg[nsSeg.Length - 1].ToLowerInvariant();
				if (string.IsNullOrEmpty(prefix))
				{
					if (string.IsNullOrEmpty(TypeNamespace))
						prefix = "global";
					else
						prefix = "empty";
				}
				int count = 0;
				var truePrefix = prefix;
				XNamespace prefixNs, ns = ctx.GetXmlNamespace(xmlNs);
				while ((prefixNs = elem.GetNamespaceOfPrefix(truePrefix)) != null && prefixNs != ns)
				{
					count++;
					truePrefix = prefix + count;
				}

				if (prefixNs == null)
				{
					elem.Add(new XAttribute(XNamespace.Xmlns + XmlConvert.EncodeLocalName(truePrefix), ns));
					if (string.IsNullOrEmpty(TypeNamespace))
						elem.AddBeforeSelf(new XComment(string.Format("'{0}' is prefix for the global namespace", truePrefix)));
				}
			}
			Namespace = ctx.GetXmlNamespace(xmlNs);
		}

		public XName ToXName(XamlContext ctx)
		{
			if (Namespace == null)
				return XmlConvert.EncodeLocalName(TypeName);
			return Namespace + XmlConvert.EncodeLocalName(TypeName);
		}

		public override string ToString() => TypeName;
	}
}