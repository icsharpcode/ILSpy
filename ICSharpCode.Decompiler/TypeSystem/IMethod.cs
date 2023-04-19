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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents a method, constructor, destructor or operator.
	/// </summary>
	public interface IMethod : IParameterizedMember
	{
		/// <summary>
		/// Gets the attributes associated with the return type. (e.g. [return: MarshalAs(...)])
		/// </summary>
		/// <remarks>
		/// Does not include inherited attributes.
		/// </remarks>
		IEnumerable<IAttribute> GetReturnTypeAttributes();

		/// <summary>
		/// Gets whether the return type is 'ref readonly'.
		/// </summary>
		bool ReturnTypeIsRefReadOnly { get; }

		/// <summary>
		/// Gets whether this method may only be called on fresh instances.
		/// Used with C# 9 `init;` property setters.
		/// </summary>
		bool IsInitOnly { get; }

		/// <summary>
		/// Gets whether the method accepts the 'this' reference as ref readonly.
		/// This can be either because the method is C# 8.0 'readonly', or because it is within a C# 7.2 'readonly struct'
		/// </summary>
		bool ThisIsRefReadOnly { get; }

		/// <summary>
		/// Gets the type parameters of this method; or an empty list if the method is not generic.
		/// </summary>
		IReadOnlyList<ITypeParameter> TypeParameters { get; }

		/// <summary>
		/// Gets the type arguments passed to this method.
		/// If the method is generic but not parameterized yet, this property returns the type parameters,
		/// as if the method was parameterized with its own type arguments (<c>void M&lt;T&gt;() { M&lt;T&gt;(); }</c>).
		/// </summary>
		IReadOnlyList<IType> TypeArguments { get; }

		bool IsExtensionMethod { get; }
		bool IsLocalFunction { get; }
		bool IsConstructor { get; }
		bool IsDestructor { get; }
		bool IsOperator { get; }

		/// <summary>
		/// Gets whether the method has a body.
		/// This property returns <c>false</c> for <c>abstract</c> or <c>extern</c> methods,
		/// or for <c>partial</c> methods without implementation.
		/// </summary>
		bool HasBody { get; }

		/// <summary>
		/// Gets whether the method is a property/event accessor.
		/// </summary>
		[MemberNotNullWhen(true, nameof(AccessorOwner))]
		bool IsAccessor { get; }

		/// <summary>
		/// If this method is an accessor, returns the corresponding property/event.
		/// Otherwise, returns null.
		/// </summary>
		IMember? AccessorOwner { get; }

		/// <summary>
		/// Gets the kind of accessor this is.
		/// </summary>
		MethodSemanticsAttributes AccessorKind { get; }

		/// <summary>
		/// If this method is reduced from an extension method or a local function returns the original method, <c>null</c> otherwise.
		/// A reduced method doesn't contain the extension method parameter. That means that it has one parameter less than its definition.
		/// A local function doesn't contain compiler-generated method parameters at the end.
		/// </summary>
		IMethod? ReducedFrom { get; }

		/// <summary>
		/// Specializes this method with the given substitution.
		/// If this method is already specialized, the new substitution is composed with the existing substition.
		/// </summary>
		new IMethod Specialize(TypeParameterSubstitution substitution);
	}
}
