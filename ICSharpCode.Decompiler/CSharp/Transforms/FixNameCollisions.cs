// Copyright (c) 2016 Daniel Grunwald
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

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Rename entities to solve name collisions that make the code uncompilable.
	/// </summary>
	/// <remarks>
	/// Currently, we only rename private fields that collide with property or event names.
	/// This helps especially with compiler-generated events that were not detected as a pattern.
	/// </remarks>
	public class FixNameCollisions : IAstTransform
	{
		public void Run(AstNode rootNode, TransformContext context)
		{
			var renamedSymbols = new Dictionary<ISymbol, string>();
			foreach (var typeDecl in rootNode.DescendantsAndSelf.OfType<TypeDeclaration>()) {
				var memberNames = typeDecl.Members.Select(m => {
				                                          	var type = m.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole);
				                                          	return type.IsNull ? m.Name : type + "." + m.Name;
				                                          }).ToHashSet();
				// memberNames does not include fields or non-custom events because those
				// don't have a single name, but a list of VariableInitializers.
				foreach (var fieldDecl in typeDecl.Members.OfType<FieldDeclaration>()) {
					if (fieldDecl.Variables.Count != 1)
						continue;
					string oldName = fieldDecl.Variables.Single().Name;
					ISymbol symbol = fieldDecl.GetSymbol();
					if (memberNames.Contains(oldName) && ((IField)symbol).Accessibility == Accessibility.Private) {
						string newName = PickNewName(memberNames, oldName);
						if (symbol != null) {
							fieldDecl.Variables.Single().Name = newName;
							renamedSymbols[symbol] = newName;
						}
					}
				}
			}
			
			foreach (var node in rootNode.DescendantsAndSelf) {
				if (node is IdentifierExpression || node is MemberReferenceExpression) {
					ISymbol symbol = node.GetSymbol();
					if (symbol != null && renamedSymbols.TryGetValue(symbol, out string newName)) {
						node.GetChildByRole(Roles.Identifier).Name = newName;
					}
				}
			}
		}

		string PickNewName(ISet<string> memberNames, string name)
		{
			if (!memberNames.Contains("m_" + name))
				return "m_" + name;
			for (int num = 2;; num++) {
				string newName = name + num;
				if (!memberNames.Contains(newName))
					return newName;
			}
		}
	}
}
