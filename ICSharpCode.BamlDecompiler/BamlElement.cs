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

using System.Collections.Generic;
using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Baml;

namespace ICSharpCode.BamlDecompiler
{
	internal readonly struct XamlNode
	{
		XamlNode(XElement value)
		{
			Element = value;
			String = null;
		}

		XamlNode(string value)
		{
			Element = null;
			String = value;
		}

		public readonly XElement? Element;
		public readonly string? String;

		public static implicit operator XamlNode(XElement value) => new XamlNode(value);
		public static implicit operator XamlNode(string value) => new XamlNode(value);
		public static implicit operator XElement?(XamlNode node) => node.Element;
		public static implicit operator string?(XamlNode node) => node.String;
	}

	internal class BamlElement
	{
		public BamlNode Node { get; }
		public XamlNode Xaml { get; set; }

		public BamlElement? Parent { get; set; }
		public IList<BamlElement> Children { get; }

		public BamlElement(BamlNode node)
		{
			Node = node;
			Children = new List<BamlElement>();
		}
	}
}
