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

using System;
using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.BamlDecompiler.Xaml;

namespace ICSharpCode.BamlDecompiler.Handlers
{
	internal class PropertyWithStaticResourceIdHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.PropertyWithStaticResourceId;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (PropertyWithStaticResourceIdRecord)((BamlRecordNode)node).Record;
			var doc = new BamlElement(node);

			var elemAttr = ctx.ResolveProperty(record.AttributeId);
			doc.Xaml = new XElement(elemAttr.ToXName(ctx, null));

			doc.Xaml.Element.AddAnnotation(elemAttr);
			parent.Xaml.Element.Add(doc.Xaml.Element);

			BamlNode? found = node;
			XamlResourceKey? key;
			do
			{
				key = XamlResourceKey.FindKeyInAncestors(found.Parent, out found);
			} while (key != null && record.StaticResourceId >= key.StaticResources.Count);

			if (key == null)
				throw new Exception("Cannot find StaticResource @" + node.Record.Position);

			var resNode = key.StaticResources[record.StaticResourceId];

			var handler = (IDeferHandler)HandlerMap.LookupHandler(resNode.Type);
			var resElem = handler.TranslateDefer(ctx, resNode, doc);

			doc.Children.Add(resElem);
			resElem.Parent = doc;

			elemAttr.DeclaringType.ResolveNamespace(doc.Xaml, ctx);
			doc.Xaml.Element.Name = elemAttr.ToXName(ctx, null);

			return doc;
		}
	}
}