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

using System.Linq;
using System.Xml.Linq;

using ILSpy.BamlDecompiler.Xaml;

namespace ILSpy.BamlDecompiler.Rewrite
{
	internal class XClassRewritePass : IRewritePass
	{
		public void Run(XamlContext ctx, XDocument document)
		{
			foreach (var elem in document.Elements(ctx.GetPseudoName("Document")).Elements())
				RewriteClass(ctx, elem);
		}

		void RewriteClass(XamlContext ctx, XElement elem)
		{
			var type = elem.Annotation<XamlType>();
			if (type == null || type.ResolvedType == null)
				return;

			var typeDef = type.ResolvedType.GetDefinition();
			if (typeDef == null || !typeDef.ParentModule.IsMainModule)
				return;

			var newType = typeDef.DirectBaseTypes.First().GetDefinition();
			if (newType == null)
				return;
			var xamlType = new XamlType(newType.ParentModule, newType.ParentModule.FullAssemblyName, newType.Namespace, newType.Name);
			xamlType.ResolveNamespace(elem, ctx);

			elem.Name = xamlType.ToXName(ctx);

			var attrName = ctx.GetKnownNamespace("Class", XamlContext.KnownNamespace_Xaml, elem);

			var attrs = elem.Attributes().ToList();
			if (typeDef.Accessibility != ICSharpCode.Decompiler.TypeSystem.Accessibility.Public)
			{
				var classModifierName = ctx.GetKnownNamespace("ClassModifier", XamlContext.KnownNamespace_Xaml, elem);
				attrs.Insert(0, new XAttribute(classModifierName, "internal"));
			}
			attrs.Insert(0, new XAttribute(attrName, type.ResolvedType.FullName));
			ctx.XClassNames.Add(type.ResolvedType.FullName);
			elem.ReplaceAttributes(attrs);
		}
	}
}