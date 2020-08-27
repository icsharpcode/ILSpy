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

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public interface IMemberReference
	{
		/// <summary>
		/// Gets the declaring type reference for the member.
		/// </summary>
		ITypeReference DeclaringTypeReference { get; }

		/// <summary>
		/// Resolves the member.
		/// </summary>
		/// <param name="context">
		/// Context to use for resolving this member reference.
		/// Which kind of context is required depends on the which kind of member reference this is;
		/// please consult the documentation of the method that was used to create this member reference,
		/// or that of the class implementing this method.
		/// </param>
		/// <returns>
		/// Returns the resolved member, or <c>null</c> if the member could not be found.
		/// </returns>
		IMember Resolve(ITypeResolveContext context);
	}

	/// <summary>
	/// Method/field/property/event.
	/// </summary>
	public interface IMember : IEntity
	{
		/// <summary>
		/// Gets the original member definition for this member.
		/// Returns <c>this</c> if this is not a specialized member.
		/// Specialized members are the result of overload resolution with type substitution.
		/// </summary>
		IMember MemberDefinition { get; }

		/// <summary>
		/// Gets the return type of this member.
		/// This property never returns <c>null</c>.
		/// </summary>
		IType ReturnType { get; }

		/// <summary>
		/// Gets the interface members explicitly implemented by this member.
		/// </summary>
		/// <remarks>
		/// For methods, equivalent to (
		///		from impl in DeclaringTypeDefinition.GetExplicitInterfaceImplementations()
		///		where impl.Implementation == this
		///		select impl.InterfaceMethod
		/// ),
		/// but may be more efficient than searching the whole list.
		/// 
		/// Note that it is possible for a class to implement an interface using members in a
		/// base class unrelated to that interface:
		///   class BaseClass { public void Dispose() {} }
		///   class C : BaseClass, IDisposable { }
		/// In this case, the interface member will not show up in (BaseClass.Dispose).ImplementedInterfaceMembers,
		/// so use (C).GetInterfaceImplementations() instead to handle this case.
		/// </remarks>
		IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers { get; }

		/// <summary>
		/// Gets whether this member is explicitly implementing an interface.
		/// </summary>
		bool IsExplicitInterfaceImplementation { get; }

		/// <summary>
		/// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
		/// members can be overridden, too; if they are abstract or overriding a method.
		/// </summary>
		bool IsVirtual { get; }

		/// <summary>
		/// Gets whether this member is overriding another member.
		/// </summary>
		bool IsOverride { get; }

		/// <summary>
		/// Gets if the member can be overridden. Returns true when the member is "abstract", "virtual" or "override" but not "sealed".
		/// </summary>
		bool IsOverridable { get; }

		/// <summary>
		/// Gets the substitution belonging to this specialized member.
		/// Returns TypeParameterSubstitution.Identity for not specialized members.
		/// </summary>
		TypeParameterSubstitution Substitution {
			get;
		}

		/// <summary>
		/// Specializes this member with the given substitution.
		/// If this member is already specialized, the new substitution is composed with the existing substition.
		/// </summary>
		IMember Specialize(TypeParameterSubstitution substitution);

		/// <summary>
		/// Gets whether the members are considered equal when applying the specified type normalization.
		/// </summary>
		bool Equals(IMember obj, TypeVisitor typeNormalization);
	}
}
