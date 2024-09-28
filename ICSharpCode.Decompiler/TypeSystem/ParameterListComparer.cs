﻿// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Compares parameter lists by comparing the types of all parameters.
	/// </summary>
	/// <remarks>
	/// 'ref int' and 'out int' are considered to be equal - unless <see cref="includeModifiers" /> is set to true.
	/// 'object' and 'dynamic' are also equal.
	/// For generic methods, "Method{T}(T a)" and "Method{S}(S b)" are considered equal.
	/// However, "Method(T a)" and "Method(S b)" are not considered equal when the type parameters T and S belong to classes.
	/// </remarks>
	public sealed class ParameterListComparer : IEqualityComparer<IReadOnlyList<IParameter>>
	{
		public static readonly ParameterListComparer Instance = new ParameterListComparer();

		static readonly NormalizeTypeVisitor normalizationVisitor = new NormalizeTypeVisitor {
			ReplaceClassTypeParametersWithDummy = false,
			ReplaceMethodTypeParametersWithDummy = true,
			DynamicAndObject = true,
			TupleToUnderlyingType = true,
		};

		bool includeModifiers;

		public static ParameterListComparer WithOptions(bool includeModifiers = false)
		{
			return new ParameterListComparer() {
				includeModifiers = includeModifiers
			};
		}

		public bool Equals(IReadOnlyList<IParameter>? x, IReadOnlyList<IParameter>? y)
		{
			if (x == y)
				return true;
			if (x == null || y == null || x.Count != y.Count)
				return false;
			for (int i = 0; i < x.Count; i++)
			{
				var a = x[i];
				var b = y[i];
				if (a == null && b == null)
					continue;
				if (a == null || b == null)
					return false;

				if (includeModifiers)
				{
					if (a.ReferenceKind != b.ReferenceKind)
						return false;
					if (a.IsParams != b.IsParams)
						return false;
				}

				// We want to consider the parameter lists "Method<T>(T a)" and "Method<S>(S b)" as equal.
				// However, the parameter types are not considered equal, as T is a different type parameter than S.
				// In order to compare the method signatures, we will normalize all method type parameters.
				IType aType = a.Type.AcceptVisitor(normalizationVisitor);
				IType bType = b.Type.AcceptVisitor(normalizationVisitor);

				if (!aType.Equals(bType))
					return false;
			}
			return true;
		}

		public int GetHashCode(IReadOnlyList<IParameter> obj)
		{
			int hashCode = obj.Count;
			unchecked
			{
				foreach (IParameter p in obj)
				{
					hashCode *= 27;
					IType type = p.Type.AcceptVisitor(normalizationVisitor);
					hashCode += type.GetHashCode();
				}
			}
			return hashCode;
		}
	}

	/// <summary>
	/// Compares member signatures.
	/// </summary>
	/// <remarks>
	/// This comparer checks for equal short name, equal type parameter count, and equal parameter types (using ParameterListComparer).
	/// </remarks>
	public sealed class SignatureComparer : IEqualityComparer<IMember>
	{
		StringComparer nameComparer;

		public SignatureComparer(StringComparer nameComparer)
		{
			if (nameComparer == null)
				throw new ArgumentNullException(nameof(nameComparer));
			this.nameComparer = nameComparer;
		}

		/// <summary>
		/// Gets a signature comparer that uses an ordinal comparison for the member name.
		/// </summary>
		public static readonly SignatureComparer Ordinal = new SignatureComparer(StringComparer.Ordinal);

		public bool Equals(IMember? x, IMember? y)
		{
			if (x == y)
				return true;
			if (x == null || y == null || x.SymbolKind != y.SymbolKind || !nameComparer.Equals(x.Name, y.Name))
				return false;
			IParameterizedMember? px = x as IParameterizedMember;
			IParameterizedMember? py = y as IParameterizedMember;
			if (px != null && py != null)
			{
				IMethod? mx = x as IMethod;
				IMethod? my = y as IMethod;
				if (mx != null && my != null && mx.TypeParameters.Count != my.TypeParameters.Count)
					return false;
				return ParameterListComparer.Instance.Equals(px.Parameters, py.Parameters);
			}
			else
			{
				return true;
			}
		}

		public int GetHashCode(IMember obj)
		{
			unchecked
			{
				int hash = (int)obj.SymbolKind * 33 + nameComparer.GetHashCode(obj.Name);
				IParameterizedMember? pm = obj as IParameterizedMember;
				if (pm != null)
				{
					hash *= 27;
					hash += ParameterListComparer.Instance.GetHashCode(pm.Parameters);
					IMethod? m = pm as IMethod;
					if (m != null)
						hash += m.TypeParameters.Count;
				}
				return hash;
			}
		}
	}
}
