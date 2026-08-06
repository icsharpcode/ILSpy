// Copyright (c) 2026 Siegfried Pammer
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

using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Gives the anonymous types of a VB assembly that are declared rather than written as C#
	/// anonymous types a name that is legal in C#.
	/// </summary>
	/// <remarks>
	/// An anonymous type with a settable property has no C# anonymous type equivalent
	/// (see <see cref="NRExtensions.IsAnonymousType(IType)"/>), so its declaration is emitted and
	/// referred to by name. The VB compiler separates the parts of that name with '$', which is
	/// not a legal identifier character in C#, and does the same for the backing fields.
	/// Renaming happens per identifier via its symbol, so a reference is renamed the same way
	/// whether or not the declaration is part of the same output.
	/// </remarks>
	public class RenameVisualBasicAnonymousTypes : IAstTransform
	{
		public void Run(AstNode rootNode, TransformContext context)
		{
			foreach (var identifier in rootNode.DescendantsAndSelf.OfType<Identifier>())
			{
				if (!identifier.Name.Contains("$"))
					continue;
				// A field declaration carries its symbol on the FieldDeclaration, not on the
				// VariableInitializer holding the name.
				if (FindEntity(identifier.Parent) is not IEntity entity || entity.Name != identifier.Name)
					continue;
				var declaringType = entity as ITypeDefinition ?? entity.DeclaringTypeDefinition;
				if (declaringType == null || !declaringType.IsAnonymousTypeDeclaredAsNamedType())
					continue;
				identifier.Name = identifier.Name.Replace('$', '_');
			}

			foreach (var typeDeclaration in rootNode.DescendantsAndSelf.OfType<TypeDeclaration>())
			{
				if (typeDeclaration.GetSymbol() is not ITypeDefinition type
					|| !type.IsAnonymousTypeDeclaredAsNamedType())
				{
					continue;
				}
				typeDeclaration.AddLeadingTrivia(new Comment(
					" A VB anonymous type. Its properties are settable and only those declared 'Key'"));
				typeDeclaration.AddLeadingTrivia(new Comment(
					" take part in Equals and GetHashCode, so it cannot be written as a C# anonymous"));
				typeDeclaration.AddLeadingTrivia(new Comment(
					" type and is declared here instead."));
			}

			static IEntity FindEntity(AstNode node)
			{
				return node?.GetSymbol() as IEntity ?? node?.Parent?.GetSymbol() as IEntity;
			}
		}
	}
}
