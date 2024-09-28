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
	internal class PropertyHandler : IHandler
	{
		public virtual BamlRecordType Type => BamlRecordType.Property;

		public BamlElement? Translate(XamlContext ctx, BamlNode node, BamlElement? parent)
		{
			var record = (PropertyRecord)((BamlRecordNode)node).Record;

			var elemType = parent.Xaml.Element.Annotation<XamlType>();
			var xamlProp = ctx.ResolveProperty(record.AttributeId);
			var value = XamlUtils.Escape(record.Value);
			xamlProp.DeclaringType.ResolveNamespace(parent.Xaml, ctx);

			parent.Xaml.Element.Add(ConstructXAttribute());

			return null;

			XAttribute ConstructXAttribute()
			{
				if (xamlProp.IsAttachedTo(elemType))
					return new XAttribute(xamlProp.ToXName(ctx, parent.Xaml, true), value);

				if (xamlProp.PropertyName == "Name" && elemType.ResolvedType.GetDefinition()?.ParentModule.IsMainModule == true)
					return new XAttribute(ctx.GetKnownNamespace("Name", XamlContext.KnownNamespace_Xaml), value);

				return new XAttribute(xamlProp.ToXName(ctx, parent.Xaml, false), value);
			}
		}
	}
}