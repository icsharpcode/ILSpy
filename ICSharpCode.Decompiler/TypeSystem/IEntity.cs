// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents a resolved entity.
	/// </summary>
	public interface IEntity : ISymbol, ICompilationProvider, INamedElement
	{
		/// <summary>
		/// Gets the metadata token for this entity.
		/// </summary>
		/// <remarks>
		/// The token is only valid within the context of the assembly defining this entity.
		/// Token may be 0 if this is a generated member.
		/// Note: specialized members will return the token of the member definition.
		/// </remarks>
		System.Reflection.Metadata.EntityHandle MetadataToken { get; }

		/// <summary>
		/// Gets the short name of the entity.
		/// </summary>
		new string Name { get; }

		/// <summary>
		/// Gets the declaring class.
		/// For members, this is the class that contains the member.
		/// For nested classes, this is the outer class. For top-level entities, this property returns null.
		/// </summary>
		ITypeDefinition? DeclaringTypeDefinition { get; }

		/// <summary>
		/// Gets/Sets the declaring type (incl. type arguments, if any).
		/// This property will return null for top-level entities.
		/// If this is not a specialized member, the value returned is equal to <see cref="DeclaringTypeDefinition"/>.
		/// </summary>
		IType? DeclaringType { get; }

		/// <summary>
		/// The module in which this entity is defined.
		/// May return null, if the IEntity was not created from a module.
		/// </summary>
		IModule? ParentModule { get; }

		/// <summary>
		/// Gets the attributes on this entity.
		/// Does not include inherited attributes.
		/// </summary>
		IEnumerable<IAttribute> GetAttributes();

		bool HasAttribute(KnownAttribute attribute);

		IAttribute? GetAttribute(KnownAttribute attribute);

		/// <summary>
		/// Gets the accessibility of this entity.
		/// </summary>
		Accessibility Accessibility { get; }

		/// <summary>
		/// Gets whether this entity is static.
		/// Returns true if either the 'static' or the 'const' modifier is set.
		/// </summary>
		bool IsStatic { get; }

		/// <summary>
		/// Returns whether this entity is abstract.
		/// </summary>
		/// <remarks>Static classes also count as abstract classes.</remarks>
		bool IsAbstract { get; }

		/// <summary>
		/// Returns whether this entity is sealed.
		/// </summary>
		/// <remarks>Static classes also count as sealed classes.</remarks>
		bool IsSealed { get; }
	}
}
