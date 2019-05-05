// Copyright (c) 2019 Siegfried Pammer
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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ILSpy.BamlDecompiler.Baml;
using ILSpy.BamlDecompiler.Xaml;

namespace ILSpy.BamlDecompiler.Handlers
{
	class StaticResourceStartHandler : IHandler, IDeferHandler
	{
		public BamlRecordType Type => BamlRecordType.StaticResourceStart;

		public BamlElement Translate(XamlContext ctx, BamlNode node, BamlElement parent)
		{
			var record = (StaticResourceStartRecord)((BamlBlockNode)node).Record;
			var key = XamlResourceKey.FindKeyInSiblings(node);

			key.StaticResources.Add(node);
			return null;
		}

		public BamlElement TranslateDefer(XamlContext ctx, BamlNode node, BamlElement parent)
		{
			var record = (StaticResourceStartRecord)((BamlBlockNode)node).Record;
			var doc = new BamlElement(node);
			var elemType = ctx.ResolveType(record.TypeId);
			doc.Xaml = new XElement(elemType.ToXName(ctx));
			doc.Xaml.Element.AddAnnotation(elemType);
			parent.Xaml.Element.Add(doc.Xaml.Element);
			HandlerMap.ProcessChildren(ctx, (BamlBlockNode)node, doc);
			return doc;
		}
	}
}
