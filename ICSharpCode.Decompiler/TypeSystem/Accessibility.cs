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

using System.Diagnostics;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Enum that describes the accessibility of an entity.
	/// </summary>
	public enum Accessibility : byte
	{
		// Note: some code depends on the fact that these values are within the range 0-7

		/// <summary>
		/// The entity is completely inaccessible. This is used for C# explicit interface implementations.
		/// </summary>
		None,
		/// <summary>
		/// The entity is only accessible within the same class.
		/// </summary>
		Private,
		/// <summary>
		/// The entity is accessible in derived classes within the same assembly.
		/// This corresponds to C# <c>private protected</c>.
		/// </summary>
		ProtectedAndInternal,
		/// <summary>
		/// The entity is only accessible within the same class and in derived classes.
		/// </summary>
		Protected,
		/// <summary>
		/// The entity is accessible within the same assembly.
		/// </summary>
		Internal,
		/// <summary>
		/// The entity is accessible both everywhere in the assembly, and in all derived classes.
		/// This corresponds to C# <c>protected internal</c>.
		/// </summary>
		ProtectedOrInternal,
		/// <summary>
		/// The entity is accessible everywhere.
		/// </summary>
		Public,
	}

	public static class AccessibilityExtensions
	{
		// This code depends on the fact that the enum values are sorted similar to the partial order
		// where an accessibility is smaller than another if it makes an entity visibible to a subset of the code:
		// digraph Accessibilities {
		//   none -> private -> protected_and_internal -> protected -> protected_or_internal -> public;
		//   none -> private -> protected_and_internal -> internal  -> protected_or_internal -> public;
		// }

		/// <summary>
		/// Gets whether a &lt;= b in the partial order of accessibilities:
		/// return true if b is accessible everywhere where a is accessible.
		/// </summary>
		public static bool LessThanOrEqual(this Accessibility a, Accessibility b)
		{
			// Exploit the enum order being similar to the partial order to dramatically simplify the logic here:
			// protected vs. internal is the only pair for which the enum value order doesn't match the partial order
			return a <= b && !(a == Accessibility.Protected && b == Accessibility.Internal);
		}

		/// <summary>
		/// Computes the intersection of the two accessibilities:
		/// The result is accessible from any given point in the code
		/// iff both a and b are accessible from that point.
		/// </summary>
		public static Accessibility Intersect(this Accessibility a, Accessibility b)
		{
			if (a > b)
			{
				ExtensionMethods.Swap(ref a, ref b);
			}
			if (a == Accessibility.Protected && b == Accessibility.Internal)
			{
				return Accessibility.ProtectedAndInternal;
			}
			else
			{
				Debug.Assert(!(a == Accessibility.Internal && b == Accessibility.Protected));
				return a;
			}
		}

		/// <summary>
		/// Computes the union of the two accessibilities:
		/// The result is accessible from any given point in the code
		/// iff at least one of a or b is accessible from that point.
		/// </summary>
		public static Accessibility Union(this Accessibility a, Accessibility b)
		{
			if (a > b)
			{
				ExtensionMethods.Swap(ref a, ref b);
			}
			if (a == Accessibility.Protected && b == Accessibility.Internal)
			{
				return Accessibility.ProtectedOrInternal;
			}
			else
			{
				Debug.Assert(!(a == Accessibility.Internal && b == Accessibility.Protected));
				return b;
			}
		}

		/// <summary>
		/// Gets the effective accessibility of the entity.
		/// For example, a public method in an internal class returns "internal".
		/// </summary>
		public static Accessibility EffectiveAccessibility(this IEntity entity)
		{
			Accessibility accessibility = entity.Accessibility;
			for (ITypeDefinition typeDef = entity.DeclaringTypeDefinition; typeDef != null; typeDef = typeDef.DeclaringTypeDefinition)
			{
				accessibility = Intersect(accessibility, typeDef.Accessibility);
			}
			return accessibility;
		}
	}
}
