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

using ICSharpCode.BamlDecompiler.Baml;

namespace ICSharpCode.BamlDecompiler.Handlers
{
	internal class TextHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.Text;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (TextRecord)((BamlRecordNode)node).Record;

			parent.Xaml.Element.Add(record.Value);

			return null;
		}
	}

	internal class TextWithIdHandler : IHandler
	{
		public BamlRecordType Type => BamlRecordType.TextWithId;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (TextWithIdRecord)((BamlRecordNode)node).Record;

			parent.Xaml.Element.Add(ctx.ResolveString(record.ValueId));

			return null;
		}
	}
}