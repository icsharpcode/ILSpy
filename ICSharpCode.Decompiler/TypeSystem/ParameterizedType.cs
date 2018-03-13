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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// ParameterizedType represents an instance of a generic type.
	/// Example: List&lt;string&gt;
	/// </summary>
	/// <remarks>
	/// When getting the members, this type modifies the lists so that
	/// type parameters in the signatures of the members are replaced with
	/// the type arguments.
	/// </remarks>
	[Serializable]
	public sealed class ParameterizedType : IType
	{
		readonly IType[] typeArguments;
		
		public ParameterizedType(IType genericType, IEnumerable<IType> typeArguments)
		{
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.GenericType = genericType ?? throw new ArgumentNullException("genericType");
			this.typeArguments = typeArguments.ToArray(); // copy input array to ensure it isn't modified
			if (this.typeArguments.Length == 0)
				throw new ArgumentException("Cannot use ParameterizedType with 0 type arguments.");
			if (genericType.TypeParameterCount != this.typeArguments.Length)
				throw new ArgumentException("Number of type arguments must match the type definition's number of type parameters");
			var gp = genericType as ICompilationProvider;
			for (var i = 0; i < this.typeArguments.Length; i++) {
				if (this.typeArguments[i] == null)
					throw new ArgumentNullException("typeArguments[" + i + "]");
				var p = this.typeArguments[i] as ICompilationProvider;
				if (p != null && gp != null && p.Compilation != gp.Compilation)
					throw new InvalidOperationException("Cannot parameterize a type with type arguments from a different compilation.");
			}
		}
		
		/// <summary>
		/// Fast internal version of the constructor. (no safety checks)
		/// Keeps the array that was passed and assumes it won't be modified.
		/// </summary>
		internal ParameterizedType(IType genericType, IType[] typeArguments)
		{
			Debug.Assert(genericType.TypeParameterCount == typeArguments.Length);
			this.GenericType = genericType;
			this.typeArguments = typeArguments;
		}
		
		public TypeKind Kind => GenericType.Kind;

		public IType GenericType { get; }

		public bool? IsReferenceType => GenericType.IsReferenceType;

		public IType DeclaringType {
			get {
				var declaringType = GenericType.DeclaringType;
				if (declaringType != null && declaringType.TypeParameterCount > 0
				    && declaringType.TypeParameterCount <= GenericType.TypeParameterCount)
				{
					var newTypeArgs = new IType[declaringType.TypeParameterCount];
					Array.Copy(this.typeArguments, 0, newTypeArgs, 0, newTypeArgs.Length);
					return new ParameterizedType(declaringType, newTypeArgs);
				}
				return declaringType;
			}
		}

		public int TypeParameterCount => typeArguments.Length;

		public string FullName => GenericType.FullName;

		public string Name => GenericType.Name;

		public string Namespace => GenericType.Namespace;

		public string ReflectionName {
			get {
				var b = new StringBuilder(GenericType.ReflectionName);
				b.Append('[');
				for (var i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						b.Append(',');
					b.Append('[');
					b.Append(typeArguments[i].ReflectionName);
					b.Append(']');
				}
				b.Append(']');
				return b.ToString();
			}
		}
		
		public override string ToString()
		{
			return ReflectionName;
		}

		public IReadOnlyList<IType> TypeArguments => typeArguments;

		/// <summary>
		/// Same as 'parameterizedType.TypeArguments[index]'.
		/// </summary>
		public IType GetTypeArgument(int index)
		{
			return typeArguments[index];
		}

		public IReadOnlyList<ITypeParameter> TypeParameters => GenericType.TypeParameters;

		/// <summary>
		/// Gets the definition of the generic type.
		/// For <c>ParameterizedType</c>, this method never returns null.
		/// </summary>
		public ITypeDefinition GetDefinition()
		{
			return GenericType as ITypeDefinition;
		}
		
		public ITypeReference ToTypeReference()
		{
			return new ParameterizedTypeReference(GenericType.ToTypeReference(), typeArguments.Select(t => t.ToTypeReference()));
		}
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type.
		/// </summary>
		public TypeParameterSubstitution GetSubstitution()
		{
			return new TypeParameterSubstitution(typeArguments, null);
		}
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type,
		/// and also substitutes method type parameters with the specified method type arguments.
		/// </summary>
		public TypeParameterSubstitution GetSubstitution(IReadOnlyList<IType> methodTypeArguments)
		{
			return new TypeParameterSubstitution(typeArguments, methodTypeArguments);
		}
		
		public IEnumerable<IType> DirectBaseTypes {
			get {
				var substitution = GetSubstitution();
				return GenericType.DirectBaseTypes.Select(t => t.AcceptVisitor(substitution));
			}
		}
		
		public IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetNestedTypes(filter, options);
			else
				return GetMembersHelper.GetNestedTypes(this, filter, options);
		}
		
		public IEnumerable<IType> GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetNestedTypes(typeArguments, filter, options);
			else
				return GetMembersHelper.GetNestedTypes(this, typeArguments, filter, options);
		}
		
		public IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetConstructors(filter, options);
			else
				return GetMembersHelper.GetConstructors(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetMethods(filter, options);
			else
				return GetMembersHelper.GetMethods(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetMethods(typeArguments, filter, options);
			else
				return GetMembersHelper.GetMethods(this, typeArguments, filter, options);
		}
		
		public IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetProperties(filter, options);
			else
				return GetMembersHelper.GetProperties(this, filter, options);
		}
		
		public IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetFields(filter, options);
			else
				return GetMembersHelper.GetFields(this, filter, options);
		}
		
		public IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetEvents(filter, options);
			else
				return GetMembersHelper.GetEvents(this, filter, options);
		}
		
		public IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetMembers(filter, options);
			else
				return GetMembersHelper.GetMembers(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return GenericType.GetAccessors(filter, options);
			else
				return GetMembersHelper.GetAccessors(this, filter, options);
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as IType);
		}
		
		public bool Equals(IType other)
		{
			var c = other as ParameterizedType;
			if (c == null || !GenericType.Equals(c.GenericType) || typeArguments.Length != c.typeArguments.Length)
				return false;
			for (var i = 0; i < typeArguments.Length; i++) {
				if (!typeArguments[i].Equals(c.typeArguments[i]))
					return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			var hashCode = GenericType.GetHashCode();
			unchecked {
				foreach (var ta in typeArguments) {
					hashCode *= 1000000007;
					hashCode += 1000000009 * ta.GetHashCode();
				}
			}
			return hashCode;
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitParameterizedType(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			var g = GenericType.AcceptVisitor(visitor);
			// Keep ta == null as long as no elements changed, allocate the array only if necessary.
			var ta = (g != GenericType) ? new IType[typeArguments.Length] : null;
			for (var i = 0; i < typeArguments.Length; i++) {
				var r = typeArguments[i].AcceptVisitor(visitor);
				if (r == null)
					throw new NullReferenceException("TypeVisitor.Visit-method returned null");
				if (ta == null && r != typeArguments[i]) {
					// we found a difference, so we need to allocate the array
					ta = new IType[typeArguments.Length];
					for (var j = 0; j < i; j++) {
						ta[j] = typeArguments[j];
					}
				}
				if (ta != null)
					ta[i] = r;
			}
			if (ta == null)
				return this;
			else
				return new ParameterizedType(g, ta ?? typeArguments);
		}
	}
	
	/// <summary>
	/// ParameterizedTypeReference is a reference to generic class that specifies the type parameters.
	/// Example: List&lt;string&gt;
	/// </summary>
	[Serializable]
	public sealed class ParameterizedTypeReference : ITypeReference, ISupportsInterning
	{
		readonly ITypeReference[] typeArguments;
		
		public ParameterizedTypeReference(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
		{
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.GenericType = genericType ?? throw new ArgumentNullException("genericType");
			this.typeArguments = typeArguments.ToArray();
			for (var i = 0; i < this.typeArguments.Length; i++) {
				if (this.typeArguments[i] == null)
					throw new ArgumentNullException("typeArguments[" + i + "]");
			}
		}
		
		public ITypeReference GenericType { get; }

		public IReadOnlyList<ITypeReference> TypeArguments => typeArguments;

		public IType Resolve(ITypeResolveContext context)
		{
			var baseType = GenericType.Resolve(context);
			var tpc = baseType.TypeParameterCount;
			if (tpc == 0)
				return baseType;
			var resolvedTypes = new IType[tpc];
			for (var i = 0; i < resolvedTypes.Length; i++) {
				if (i < typeArguments.Length)
					resolvedTypes[i] = typeArguments[i].Resolve(context);
				else
					resolvedTypes[i] = SpecialType.UnknownType;
			}
			return new ParameterizedType(baseType, resolvedTypes);
		}
		
		public override string ToString()
		{
			var b = new StringBuilder(GenericType.ToString());
			b.Append('[');
			for (var i = 0; i < typeArguments.Length; i++) {
				if (i > 0)
					b.Append(',');
				b.Append('[');
				b.Append(typeArguments[i].ToString());
				b.Append(']');
			}
			b.Append(']');
			return b.ToString();
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			var hashCode = GenericType.GetHashCode();
			unchecked {
				foreach (var t in typeArguments) {
					hashCode *= 27;
					hashCode += t.GetHashCode();
				}
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			var o = other as ParameterizedTypeReference;
			if (o != null && GenericType == o.GenericType && typeArguments.Length == o.typeArguments.Length) {
				for (var i = 0; i < typeArguments.Length; i++) {
					if (typeArguments[i] != o.typeArguments[i])
						return false;
				}
				return true;
			}
			return false;
		}
	}
}
