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

using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.BamlDecompiler.Xaml {
	internal class XamlProperty {
		public XamlType DeclaringType { get; }
		public string PropertyName { get; }

		public IMember ResolvedMember { get; set; }

		public XamlProperty(XamlType type, string name) {
			DeclaringType = type;
			PropertyName = name;
		}

		public void TryResolve() {
			if (ResolvedMember != null)
				return;

			var typeDef = DeclaringType.ResolvedType.GetDefinition();
			if (typeDef == null)
				return;

			ResolvedMember = typeDef.GetProperties(p => p.Name == PropertyName).FirstOrDefault();
			if (ResolvedMember != null)
				return;

			ResolvedMember = typeDef.GetFields(f => f.Name == PropertyName + "Property").FirstOrDefault();
			if (ResolvedMember != null)
				return;

			ResolvedMember = typeDef.GetEvents(e => e.Name == PropertyName).FirstOrDefault();
			if (ResolvedMember != null)
				return;

			ResolvedMember = typeDef.GetFields(f => f.Name == PropertyName + "Event").FirstOrDefault();
		}

		public bool IsAttachedTo(XamlType type) {
			if (type == null || ResolvedMember == null || type.ResolvedType == null)
				return true;

			var declType = ResolvedMember.DeclaringType;
			var t = type.ResolvedType;

			do {
				if (t.FullName == declType.FullName && t.TypeParameterCount == declType.TypeParameterCount)
					return false;
				t = t.DirectBaseTypes.FirstOrDefault();
			} while (t != null);
			return true;
		}

		public XName ToXName(XamlContext ctx, XElement parent, bool isFullName = true) {
			var typeName = DeclaringType.ToXName(ctx);
			XName name;
			if (!isFullName)
				name = XmlConvert.EncodeLocalName(PropertyName);
			else {
				name = typeName.LocalName + "." + XmlConvert.EncodeLocalName(PropertyName);

				if (parent == null || (parent.GetDefaultNamespace() != typeName.Namespace
					                && parent.Name.Namespace != typeName.Namespace))
					name = typeName.Namespace + name.LocalName;
			}

			return name;
		}

		public override string ToString() => PropertyName;
	}
}