// Copyright (c) 2024 Holger Schmidt
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions
{
	internal static class TypeExtensions
	{
		internal static bool IsObject(this IType t) => t.IsKnownType(KnownTypeCode.Object);
		internal static bool IsInterface(this IType t) => t.Kind == TypeKind.Interface;

		internal static bool TryGetNullableType(this IType type, [MaybeNullWhen(false)] out IType typeArg)
		{
			bool isNullable = type.IsKnownType(KnownTypeCode.NullableOfT);
			typeArg = isNullable ? type.TypeArguments.Single() : null;
			return isNullable;
		}
	}

	internal static class MemberInfoExtensions
	{
		/// <summary>Groups the <paramref name="members"/> into a dictionary
		/// with <see cref="IMember.DeclaringType"/> keys.</summary>
		internal static Dictionary<IType, T[]> GroupByDeclaringType<T>(this IEnumerable<T> members) where T : IMember
			=> members.GroupByDeclaringType(m => m);

		/// <summary>Groups the <paramref name="objectsWithMembers"/> into a dictionary
		/// with <see cref="IMember.DeclaringType"/> keys using <paramref name="getMember"/>.</summary>
		internal static Dictionary<IType, T[]> GroupByDeclaringType<T>(this IEnumerable<T> objectsWithMembers, Func<T, IMember> getMember)
			=> objectsWithMembers.GroupBy(m => getMember(m).DeclaringType).ToDictionary(g => g.Key, g => g.ToArray());
	}

	internal static class DictionaryExtensions
	{
		/// <summary>Returns the <paramref name="dictionary"/>s value for the specified <paramref name="key"/>
		/// if available and otherwise the default for <typeparamref name="Tout"/>.</summary>
		internal static Tout? GetValue<T, Tout>(this IDictionary<T, Tout> dictionary, T key)
			=> dictionary.TryGetValue(key, out Tout? value) ? value : default;
	}
}