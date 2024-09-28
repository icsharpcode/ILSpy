﻿// Copyright (c) 2019 Siegfried Pammer
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

using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.BamlDecompiler.Xaml;

namespace ICSharpCode.BamlDecompiler.Handlers
{
	class StaticResourceIdHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.StaticResourceId;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (StaticResourceIdRecord)((BamlRecordNode)node).Record;

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
			var resElem = handler.TranslateDefer(ctx, resNode, parent);

			parent.Children.Add(resElem);
			resElem.Parent = parent;

			return resElem;
		}
	}
}
