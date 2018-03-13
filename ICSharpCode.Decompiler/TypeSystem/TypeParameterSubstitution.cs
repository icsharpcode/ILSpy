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
using System.Text;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Substitutes class and method type parameters.
	/// </summary>
	public class TypeParameterSubstitution : TypeVisitor
	{
		/// <summary>
		/// The identity function.
		/// </summary>
		public static readonly TypeParameterSubstitution Identity = new TypeParameterSubstitution(null, null);

		/// <summary>
		/// Creates a new type parameter substitution.
		/// </summary>
		/// <param name="classTypeArguments">
		/// The type arguments to substitute for class type parameters.
		/// Pass <c>null</c> to keep class type parameters unmodified.
		/// </param>
		/// <param name="methodTypeArguments">
		/// The type arguments to substitute for method type parameters.
		/// Pass <c>null</c> to keep method type parameters unmodified.
		/// </param>
		public TypeParameterSubstitution(IReadOnlyList<IType> classTypeArguments, IReadOnlyList<IType> methodTypeArguments)
		{
			this.ClassTypeArguments = classTypeArguments;
			this.MethodTypeArguments = methodTypeArguments;
		}
		
		/// <summary>
		/// Gets the list of class type arguments.
		/// Returns <c>null</c> if this substitution keeps class type parameters unmodified.
		/// </summary>
		public IReadOnlyList<IType> ClassTypeArguments { get; }

		/// <summary>
		/// Gets the list of method type arguments.
		/// Returns <c>null</c> if this substitution keeps method type parameters unmodified.
		/// </summary>
		public IReadOnlyList<IType> MethodTypeArguments { get; }

		#region Compose
		/// <summary>
		/// Computes a single TypeParameterSubstitution so that for all types <c>t</c>:
		/// <c>t.AcceptVisitor(Compose(g, f)) equals t.AcceptVisitor(f).AcceptVisitor(g)</c>
		/// </summary>
		/// <remarks>If you consider type parameter substitution to be a function, this is function composition.</remarks>
		public static TypeParameterSubstitution Compose(TypeParameterSubstitution g, TypeParameterSubstitution f)
		{
			if (g == null)
				return f;
			if (f == null || (f.ClassTypeArguments == null && f.MethodTypeArguments == null))
				return g;
			// The composition is a copy of 'f', with 'g' applied on the array elements.
			// If 'f' has a null list (keeps type parameters unmodified), we have to treat it as
			// the identity function, and thus use the list from 'g'.
			var classTypeArguments = f.ClassTypeArguments != null ? GetComposedTypeArguments(f.ClassTypeArguments, g) : g.ClassTypeArguments;
			var methodTypeArguments = f.MethodTypeArguments != null ? GetComposedTypeArguments(f.MethodTypeArguments, g) : g.MethodTypeArguments;
			return new TypeParameterSubstitution(classTypeArguments, methodTypeArguments);
		}
		
		static IReadOnlyList<IType> GetComposedTypeArguments(IReadOnlyList<IType> input, TypeParameterSubstitution substitution)
		{
			var result = new IType[input.Count];
			for (var i = 0; i < result.Length; i++) {
				result[i] = input[i].AcceptVisitor(substitution);
			}
			return result;
		}
		#endregion
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			var other = obj as TypeParameterSubstitution;
			if (other == null)
				return false;
			return TypeListEquals(ClassTypeArguments, other.ClassTypeArguments)
				&& TypeListEquals(MethodTypeArguments, other.MethodTypeArguments);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return 1124131 * TypeListHashCode(ClassTypeArguments) + 1821779 * TypeListHashCode(MethodTypeArguments);
			}
		}
		
		static bool TypeListEquals(IReadOnlyList<IType> a, IReadOnlyList<IType> b)
		{
			if (a == b)
				return true;
			if (a == null || b == null)
				return false;
			if (a.Count != b.Count)
				return false;
			for (var i = 0; i < a.Count; i++) {
				if (!a[i].Equals(b[i]))
					return false;
			}
			return true;
		}
		
		static int TypeListHashCode(IReadOnlyList<IType> obj)
		{
			if (obj == null)
				return 0;
			unchecked {
				var hashCode = 1;
				foreach (var element in obj) {
					hashCode *= 27;
					hashCode += element.GetHashCode();
				}
				return hashCode;
			}
		}
		#endregion
		
		public override IType VisitTypeParameter(ITypeParameter type)
		{
			var index = type.Index;
			if (ClassTypeArguments != null && type.OwnerType == SymbolKind.TypeDefinition) {
				if (index >= 0 && index < ClassTypeArguments.Count)
					return ClassTypeArguments[index];
				else
					return SpecialType.UnknownType;
			} else if (MethodTypeArguments != null && type.OwnerType == SymbolKind.Method) {
				if (index >= 0 && index < MethodTypeArguments.Count)
					return MethodTypeArguments[index];
				else
					return SpecialType.UnknownType;
			} else {
				return base.VisitTypeParameter(type);
			}
		}
		
		public override string ToString()
		{
			var b = new StringBuilder();
			b.Append('[');
			var first = true;
			if (ClassTypeArguments != null) {
				for (var i = 0; i < ClassTypeArguments.Count; i++) {
					if (first) first = false; else b.Append(", ");
					b.Append('`');
					b.Append(i);
					b.Append(" -> ");
					b.Append(ClassTypeArguments[i].ReflectionName);
				}
			}
			if (MethodTypeArguments != null) {
				for (var i = 0; i < MethodTypeArguments.Count; i++) {
					if (first) first = false; else b.Append(", ");
					b.Append("``");
					b.Append(i);
					b.Append(" -> ");
					b.Append(MethodTypeArguments[i].ReflectionName);
				}
			}
			b.Append(']');
			return b.ToString();
		}
	}
}
