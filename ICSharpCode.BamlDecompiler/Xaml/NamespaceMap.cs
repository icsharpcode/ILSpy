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

using System;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.BamlDecompiler.Xaml
{
	internal class NamespaceMap
	{
		public string XmlnsPrefix { get; set; }
		public string FullAssemblyName { get; set; }

		/// <summary>
		/// The assembly <see cref="FullAssemblyName"/> resolves to, where it could be resolved.
		/// The name is the one the document was written against, which is not always the name of
		/// the assembly the types actually come from.
		/// </summary>
		public IModule Assembly { get; set; }
		public string XMLNamespace { get; set; }
		public string CLRNamespace { get; set; }

		public NamespaceMap(string prefix, string fullAssemblyName, string xmlNs)
			: this(prefix, fullAssemblyName, xmlNs, null)
		{
		}

		public NamespaceMap(string prefix, string fullAssemblyName, string xmlNs, string clrNs)
		{
			XmlnsPrefix = prefix;
			FullAssemblyName = fullAssemblyName;
			XMLNamespace = xmlNs;
			CLRNamespace = clrNs;
		}

		/// <summary>
		/// Whether <paramref name="map"/> is the declaration to use for a type named
		/// <paramref name="typeName"/> in <paramref name="clrNs"/> of
		/// <paramref name="fullAssemblyName"/>.
		/// </summary>
		public static bool Matches(NamespaceMap map, string fullAssemblyName, string clrNs, string typeName)
		{
			if (map.CLRNamespace != clrNs)
				return false;
			if (map.FullAssemblyName == fullAssemblyName)
				return true;
			// The document records the assembly it was written against, while a well-known type
			// carries the assembly it resolves to now - "mscorlib" against "System.Private.CoreLib"
			// on .NET, say. The two name the same type when the recorded assembly forwards it, and
			// then the declaration the document made is the one to use.
			return typeName != null && ForwardsOrDeclares(map.Assembly, clrNs, typeName);
		}

		static bool ForwardsOrDeclares(IModule assembly, string clrNs, string typeName)
		{
			if (assembly == null)
				return false;
			var name = new TopLevelTypeName(clrNs, typeName);
			if (assembly.GetTypeDefinition(name) != null)
				return true;
			return assembly.MetadataFile?.GetTypeForwarder(new FullTypeName(name)).IsNil == false;
		}

		public override string ToString() => $"{XmlnsPrefix}:[{FullAssemblyName}|{CLRNamespace ?? XMLNamespace}]";
	}
}