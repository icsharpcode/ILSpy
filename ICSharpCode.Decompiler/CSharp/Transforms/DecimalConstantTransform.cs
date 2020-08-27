// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Transforms decimal constant fields.
	/// </summary>
	public class DecimalConstantTransform : DepthFirstAstVisitor, IAstTransform
	{
		static readonly PrimitiveType decimalType = new PrimitiveType("decimal");

		public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			const Modifiers staticReadOnly = Modifiers.Static | Modifiers.Readonly;
			if ((fieldDeclaration.Modifiers & staticReadOnly) == staticReadOnly && decimalType.IsMatch(fieldDeclaration.ReturnType))
			{
				foreach (var attributeSection in fieldDeclaration.Attributes)
				{
					foreach (var attribute in attributeSection.Attributes)
					{
						var t = attribute.Type.GetSymbol() as IType;
						if (t != null && t.Name == "DecimalConstantAttribute" && t.Namespace == "System.Runtime.CompilerServices")
						{
							attribute.Remove();
							if (attributeSection.Attributes.Count == 0)
								attributeSection.Remove();
							fieldDeclaration.Modifiers = (fieldDeclaration.Modifiers & ~staticReadOnly) | Modifiers.Const;
							return;
						}
					}
				}
			}
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			if (!context.Settings.DecimalConstants)
				return;
			rootNode.AcceptVisitor(this);
		}
	}
}
