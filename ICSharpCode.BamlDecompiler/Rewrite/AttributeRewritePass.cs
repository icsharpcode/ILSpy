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

using System.Collections.Generic;
using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Xaml;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.BamlDecompiler.Rewrite
{
	internal class AttributeRewritePass : IRewritePass
	{
		XName key;

		public void Run(XamlContext ctx, XDocument document)
		{
			key = ctx.GetKnownNamespace("Key", XamlContext.KnownNamespace_Xaml);

			bool doWork;
			do
			{
				doWork = false;
				foreach (var elem in document.Elements())
				{
					doWork |= ProcessElement(ctx, elem);
				}
			} while (doWork);
		}

		bool ProcessElement(XamlContext ctx, XElement elem)
		{
			bool doWork = false;
			foreach (var child in elem.Elements())
			{
				doWork |= RewriteElement(ctx, elem, child);
				doWork |= ProcessElement(ctx, child);
			}
			return doWork;
		}

		bool RewriteElement(XamlContext ctx, XElement parent, XElement elem)
		{
			if (elem.HasAttributes || elem.HasElements)
				return false;

			var attrName = elem.Name;
			if (attrName != key)
			{
				var property = elem.Annotation<XamlProperty>();
				if (property is null)
					return false;

				if (property.ResolvedMember is IProperty propertyDef && !propertyDef.CanSet)
					return false;

				attrName = property.ToXName(ctx, parent, property.IsAttachedTo(parent.Annotation<XamlType>()));
			}

			ctx.CancellationToken.ThrowIfCancellationRequested();

			var attr = new XAttribute(attrName, elem.Value);
			var list = new List<XAttribute>(parent.Attributes());
			if (attrName == key)
				list.Insert(0, attr);
			else
				list.Add(attr);
			parent.RemoveAttributes();
			parent.ReplaceAttributes(list);
			elem.Remove();

			return true;
		}
	}
}
