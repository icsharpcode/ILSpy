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

namespace ICSharpCode.BamlDecompiler.Handlers
{
	internal class DefAttributeTypeHandler : IHandler, IDeferHandler
	{
		public BamlRecordType Type => BamlRecordType.DefAttributeKeyType;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			XamlResourceKey.Create(node);
			return null;
		}

		public BamlElement TranslateDefer(XamlContext ctx, BamlNode node, BamlElement parent)
		{
			var record = (DefAttributeKeyTypeRecord)((BamlRecordNode)node).Record;
			var type = ctx.ResolveType(record.TypeId);
			var typeName = ctx.ToString(parent.Xaml, type);
			var key = (XamlResourceKey)node.Annotation;

			var bamlElem = new BamlElement(node);
			bamlElem.Xaml = new XElement(ctx.GetKnownNamespace("Key", XamlContext.KnownNamespace_Xaml, parent.Xaml));
			parent.Xaml.Element.Add(bamlElem.Xaml.Element);

			var typeElem = new XElement(ctx.GetKnownNamespace("TypeExtension", XamlContext.KnownNamespace_Xaml, parent.Xaml));
			typeElem.AddAnnotation(ctx.ResolveType(0xfd4d)); // Known type - TypeExtension
			typeElem.Add(new XElement(ctx.GetPseudoName("Ctor"), typeName));
			bamlElem.Xaml.Element.Add(typeElem);

			key.KeyElement = bamlElem;

			return bamlElem;
		}
	}
}