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
using System.Threading;
using System.Xml.Linq;

using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.BamlDecompiler.Baml;
using ILSpy.BamlDecompiler.Xaml;

namespace ILSpy.BamlDecompiler
{
	internal class XamlContext
	{
		XamlContext(IDecompilerTypeSystem typeSystem)
		{
			TypeSystem = typeSystem;
			NodeMap = new Dictionary<BamlRecord, BamlBlockNode>();
			XmlNs = new XmlnsDictionary();
		}

		Dictionary<ushort, XamlType> typeMap = new Dictionary<ushort, XamlType>();
		Dictionary<ushort, XamlProperty> propertyMap = new Dictionary<ushort, XamlProperty>();
		Dictionary<string, XNamespace> xmlnsMap = new Dictionary<string, XNamespace>();

		public IDecompilerTypeSystem TypeSystem { get; }
		public CancellationToken CancellationToken { get; private set; }
		public BamlDecompilerSettings Settings { get; private set; }

		public BamlContext Baml { get; private set; }
		public BamlNode RootNode { get; private set; }
		public IDictionary<BamlRecord, BamlBlockNode> NodeMap { get; }

		public XmlnsDictionary XmlNs { get; }

		public static XamlContext Construct(IDecompilerTypeSystem typeSystem, BamlDocument document, CancellationToken token, BamlDecompilerSettings bamlDecompilerOptions)
		{
			var ctx = new XamlContext(typeSystem);
			ctx.CancellationToken = token;
			ctx.Settings = bamlDecompilerOptions ?? new BamlDecompilerSettings();

			ctx.Baml = BamlContext.ConstructContext(typeSystem, document, token);
			ctx.RootNode = BamlNode.Parse(document, token);

			ctx.BuildPIMappings(document);
			ctx.BuildNodeMap(ctx.RootNode as BamlBlockNode);

			return ctx;
		}

		void BuildNodeMap(BamlBlockNode node)
		{
			if (node == null)
				return;

			NodeMap[node.Header] = node;

			foreach (var child in node.Children)
			{
				if (child is BamlBlockNode childBlock)
					BuildNodeMap(childBlock);
			}
		}

		void BuildPIMappings(BamlDocument document)
		{
			foreach (var record in document)
			{
				var piMap = record as PIMappingRecord;
				if (piMap == null)
					continue;

				XmlNs.SetPIMapping(piMap.XmlNamespace, piMap.ClrNamespace, Baml.ResolveAssembly(piMap.AssemblyId).FullAssemblyName);
			}
		}

		public XamlType ResolveType(ushort id)
		{
			if (typeMap.TryGetValue(id, out var xamlType))
				return xamlType;

			IType type;
			IModule assembly;
			string fullAssemblyName;

			if (id > 0x7fff)
			{
				type = Baml.KnownThings.Types((KnownTypes)(short)-unchecked((short)id));
				assembly = type.GetDefinition().ParentModule;
				fullAssemblyName = assembly.FullAssemblyName;
			}
			else
			{
				var typeRec = Baml.TypeIdMap[id];
				(fullAssemblyName, assembly) = Baml.ResolveAssembly(typeRec.AssemblyId);
				type = ReflectionHelper.ParseReflectionName(typeRec.TypeFullName).Resolve(new SimpleTypeResolveContext(TypeSystem));
			}

			var clrNs = type.Namespace;
			var xmlNs = XmlNs.LookupXmlns(fullAssemblyName, clrNs);

			typeMap[id] = xamlType = new XamlType(assembly, fullAssemblyName, clrNs, type.Name, GetXmlNamespace(xmlNs)) {
				ResolvedType = type
			};

			return xamlType;
		}

		public XamlProperty ResolveProperty(ushort id)
		{
			if (propertyMap.TryGetValue(id, out var xamlProp))
				return xamlProp;

			XamlType type;
			string name;
			IMember member;

			if (id > 0x7fff)
			{
				var knownProp = Baml.KnownThings.Members((KnownMembers)unchecked((short)-(short)id));
				type = ResolveType(unchecked((ushort)(short)-(short)knownProp.Parent));
				name = knownProp.Name;
				member = knownProp.Property;
			}
			else
			{
				var attrRec = Baml.AttributeIdMap[id];
				type = ResolveType(attrRec.OwnerTypeId);
				name = attrRec.Name;

				member = null;
			}

			propertyMap[id] = xamlProp = new XamlProperty(type, name) {
				ResolvedMember = member
			};
			xamlProp.TryResolve();

			return xamlProp;
		}

		public string ResolveString(ushort id)
		{
			if (id > 0x7fff)
				return Baml.KnownThings.Strings(unchecked((short)-id));
			else if (Baml.StringIdMap.ContainsKey(id))
				return Baml.StringIdMap[id].Value;

			return null;
		}

		public XNamespace GetXmlNamespace(string xmlns)
		{
			if (xmlns == null)
				return null;

			if (!xmlnsMap.TryGetValue(xmlns, out var ns))
				xmlnsMap[xmlns] = ns = XNamespace.Get(xmlns);
			return ns;
		}

		public const string KnownNamespace_Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string KnownNamespace_Presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string KnownNamespace_PresentationOptions = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";

		public string TryGetXmlNamespace(IModule assembly, string typeNamespace)
		{
			if (assembly == null)
				return null;

			HashSet<string> possibleXmlNs = new HashSet<string>();

			foreach (var attr in assembly.GetAssemblyAttributes().Where(a => a.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute"))
			{
				Debug.Assert(attr.FixedArguments.Length == 2);
				if (attr.FixedArguments.Length != 2)
					continue;
				var xmlNs = attr.FixedArguments[0].Value as string;
				var typeNs = attr.FixedArguments[1].Value as string;
				Debug.Assert((object)xmlNs != null && (object)typeNs != null);
				if ((object)xmlNs == null || (object)typeNs == null)
					continue;

				if (typeNamespace == typeNs)
					possibleXmlNs.Add(xmlNs);
			}

			if (possibleXmlNs.Contains(KnownNamespace_Presentation))
				return KnownNamespace_Presentation;

			return possibleXmlNs.FirstOrDefault();
		}

		public XName GetKnownNamespace(string name, string xmlNamespace, XElement context = null)
		{
			var xNs = GetXmlNamespace(xmlNamespace);
			XName xName;
			if (context != null && xNs == context.GetDefaultNamespace())
				xName = name;
			else
				xName = xNs + name;
			return xName;
		}

		public XName GetPseudoName(string name) => XNamespace.Get("https://github.com/icsharpcode/ILSpy").GetName(name);
	}
}