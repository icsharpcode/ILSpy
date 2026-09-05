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

using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.BamlDecompiler.Xaml;

namespace ICSharpCode.BamlDecompiler.Handlers
{
	internal class PropertyHandler : IHandler
	{
		public virtual BamlRecordType Type => BamlRecordType.Property;

		public BamlElement Translate(XamlContext ctx, BamlNode node, BamlElement parent)
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

				if (IsRuntimeNameOfElement(xamlProp, elemType))
					return new XAttribute(ctx.GetKnownNamespace("Name", XamlContext.KnownNamespace_Xaml), value);

				return new XAttribute(xamlProp.ToXName(ctx, parent.Xaml, false), value);
			}
		}

		/// <summary>
		/// Whether <paramref name="property"/> is the name of <paramref name="elementType"/> as
		/// x:Name means it, so that the directive can be written instead of the property.
		/// <para>
		/// x:Name is recorded as the runtime name property of the element, which is
		/// FrameworkElement.Name for everything WPF - a property of the framework, not of the
		/// assembly being decompiled. A type of that assembly declaring a property of its own called
		/// "Name" is an ordinary property: writing the directive for it registers a name and leaves
		/// the property unset, which still compiles and silently means something else (issue #2253).
		/// </para>
		/// </summary>
		internal static bool IsRuntimeNameOfElement(XamlProperty property, XamlType elementType)
		{
			if (property.PropertyName != "Name")
				return false;
			if (elementType?.ResolvedType.GetDefinition()?.ParentModule.IsMainModule != true)
				return false;
			// The type that declares the property, not the one the document names as the owner of
			// the attribute: a control of the assembly being decompiled inherits Name from the
			// framework, and the document names the control. Only a Name the type declares itself
			// is a property of its own rather than the runtime name.
			var declaringType = property.ResolvedMember?.DeclaringTypeDefinition
				?? property.DeclaringType?.ResolvedType?.GetDefinition();
			return declaringType?.ParentModule.IsMainModule != true;
		}
	}
}