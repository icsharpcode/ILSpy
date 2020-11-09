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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents a class, enum, interface, struct, delegate or VB module.
	/// For partial classes, this represents the whole class.
	/// </summary>
	public interface ITypeDefinition : IType, IEntity
	{
		IReadOnlyList<ITypeDefinition> NestedTypes { get; }
		IReadOnlyList<IMember> Members { get; }

		IEnumerable<IField> Fields { get; }
		IEnumerable<IMethod> Methods { get; }
		IEnumerable<IProperty> Properties { get; }
		IEnumerable<IEvent> Events { get; }

		/// <summary>
		/// Gets the known type code for this type definition.
		/// </summary>
		KnownTypeCode KnownTypeCode { get; }

		/// <summary>
		/// For enums: returns the underlying primitive type.
		/// For all other types: returns <see cref="SpecialType.UnknownType"/>.
		/// </summary>
		IType EnumUnderlyingType { get; }

		/// <summary>
		/// For structs: returns whether this is a readonly struct.
		/// For all other types: returns false.
		/// </summary>
		bool IsReadOnly { get; }

		/// <summary>
		/// Gets the full name of this type.
		/// </summary>
		FullTypeName FullTypeName { get; }

		/// <summary>
		/// Gets/Sets the declaring type (incl. type arguments, if any).
		/// This property will return null for top-level types.
		/// </summary>
		new IType DeclaringType { get; } // solves ambiguity between IType.DeclaringType and IEntity.DeclaringType

		/// <summary>
		/// Gets whether this type contains extension methods.
		/// </summary>
		/// <remarks>This property is used to speed up the search for extension methods.</remarks>
		bool HasExtensionMethods { get; }

		/// <summary>
		/// The nullability specified in the [NullableContext] attribute on the type.
		/// This serves as default nullability for members of the type that do not have a [Nullable] attribute.
		/// </summary>
		Nullability NullableContext { get; }

		/// <summary>
		/// Gets whether the type has the necessary members to be considered a C# 9 record type.
		/// </summary>
		bool IsRecord { get; }
	}
}
