// 
// AttributeSection.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>attribute_section ::= '[' ( identifier ':' )? attribute* ']'</c> (C# grammar §23.3)
	/// </summary>
	[DecompilerAstNode(hasPatternPlaceholder: true)]
	public partial class AttributeSection : AstNode
	{
		public string AttributeTarget {
			get {
				return GetChild(Slots.Identifier)?.Name ?? string.Empty;
			}
			set {
				SetChild(Slots.Identifier, Identifier.Create(value));
			}
		}

		// DoMatch compares the name string; exclude the token slot to avoid matching it twice.
		// Optional: most attribute sections have no target (e.g. [Foo]); only [return:]/[assembly:]/...
		// carry one, so the backing token slot may be empty.
		[ExcludeFromMatch]
		[Slot("Identifier")]
		public partial Identifier? AttributeTargetToken { get; set; }

		[Slot("Attribute")]
		public partial AstNodeCollection<Attribute> Attributes { get; }

		public AttributeSection(Attribute attr)
		{
			this.Attributes.Add(attr);
		}

		//		public static string GetAttributeTargetName(AttributeTarget attributeTarget)
		//		{
		//			switch (attributeTarget) {
		//				case AttributeTarget.None:
		//					return null;
		//				case AttributeTarget.Assembly:
		//					return "assembly";
		//				case AttributeTarget.Module:
		//					return "module";
		//				case AttributeTarget.Type:
		//					return "type";
		//				case AttributeTarget.Param:
		//					return "param";
		//				case AttributeTarget.Field:
		//					return "field";
		//				case AttributeTarget.Return:
		//					return "return";
		//				case AttributeTarget.Method:
		//					return "method";
		//				default:
		//					throw new NotSupportedException("Invalid value for AttributeTarget");
		//			}
		//		}
	}
}
