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
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.BamlDecompiler.Baml;

namespace ICSharpCode.BamlDecompiler
{
	internal interface IHandler
	{
		BamlRecordType Type { get; }
		BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent);
	}

	internal interface IDeferHandler
	{
		BamlElement TranslateDefer(XamlContext ctx, BamlNode node, BamlElement parent);
	}

	internal static class HandlerMap
	{
		static readonly Dictionary<BamlRecordType, IHandler> handlers;

		static HandlerMap()
		{
			handlers = new Dictionary<BamlRecordType, IHandler>();

			foreach (var type in typeof(IHandler).Assembly.GetTypes())
			{
				if (typeof(IHandler).IsAssignableFrom(type) &&
					!type.IsInterface && !type.IsAbstract)
				{
					var handler = (IHandler)Activator.CreateInstance(type);
					handlers.Add(handler.Type, handler);
				}
			}
		}

		public static IHandler LookupHandler(BamlRecordType type)
		{
#if DEBUG
			switch (type)
			{
				case BamlRecordType.AssemblyInfo:
				case BamlRecordType.TypeInfo:
				case BamlRecordType.AttributeInfo:
				case BamlRecordType.StringInfo:
					break;
				default:
					if (!handlers.ContainsKey(type))
						throw new NotSupportedException(type.ToString());
					break;
			}
#endif
			return handlers.ContainsKey(type) ? handlers[type] : null;
		}

		public static void ProcessChildren(XamlContext ctx, BamlBlockNode node, BamlElement nodeElem)
		{
			ctx.XmlNs.PushScope(nodeElem);
			if (nodeElem.Xaml.Element != null)
				nodeElem.Xaml.Element.AddAnnotation(ctx.XmlNs.CurrentScope);
			foreach (var child in node.Children)
			{
				var handler = LookupHandler(child.Type);
				if (handler == null)
				{
					Debug.WriteLine("BAML Handler {0} not implemented.", child.Type);
					continue;
				}
				var elem = handler.Translate(ctx, (BamlNode)child, nodeElem);
				if (elem != null)
				{
					nodeElem.Children.Add(elem);
					elem.Parent = nodeElem;
				}

				ctx.CancellationToken.ThrowIfCancellationRequested();
			}
			ctx.XmlNs.PopScope();
		}
	}
}