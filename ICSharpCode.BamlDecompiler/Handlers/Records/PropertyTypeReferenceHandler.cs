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

using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.BamlDecompiler.Xaml;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.BamlDecompiler.Handlers
{
	internal class PropertyTypeReferenceHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.PropertyTypeReference;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (PropertyTypeReferenceRecord)((BamlRecordNode)node).Record;
			var attr = ctx.ResolveProperty(record.AttributeId);
			var type = ctx.ResolveType(record.TypeId);
			var typeName = ctx.ToString(parent.Xaml, type);

			var elem = new BamlElement(node);

			var elemAttr = ctx.ResolveProperty(record.AttributeId);
			elem.Xaml = new XElement(elemAttr.ToXName(ctx, null));

			if (attr.ResolvedMember?.FullNameIs("System.Windows.Style", "TargetType") == true)
			{
				parent.Xaml.Element.AddAnnotation(new TargetTypeAnnotation(type));
			}

			elem.Xaml.Element.AddAnnotation(elemAttr);
			parent.Xaml.Element.Add(elem.Xaml.Element);

			var typeElem = new XElement(ctx.GetKnownNamespace("TypeExtension", XamlContext.KnownNamespace_Xaml, parent.Xaml));
			typeElem.AddAnnotation(ctx.ResolveType(0xfd4d)); // Known type - TypeExtension
			typeElem.Add(new XElement(ctx.GetPseudoName("Ctor"), typeName));
			elem.Xaml.Element.Add(typeElem);

			elemAttr.DeclaringType.ResolveNamespace(elem.Xaml, ctx);
			elem.Xaml.Element.Name = elemAttr.ToXName(ctx, null);

			return elem;
		}
	}

	internal class TargetTypeAnnotation
	{
		public XamlType Type { get; }

		public TargetTypeAnnotation(XamlType type)
		{
			this.Type = type;
		}
	}
}