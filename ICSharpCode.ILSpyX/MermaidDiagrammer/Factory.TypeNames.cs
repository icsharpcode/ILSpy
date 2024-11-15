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

using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	public partial class ClassDiagrammerFactory
	{
		/// <summary>Returns a cached display name for <paramref name="type"/>.</summary>
		private string GetName(IType type)
		{
			if (labels!.TryGetValue(type, out string? value))
				return value; // return cached value

			return labels[type] = GenerateName(type); // generate and cache new value
		}

		/// <summary>Generates a display name for <paramref name="type"/>.</summary>
		private string GenerateName(IType type)
		{
			// non-generic types
			if (type.TypeParameterCount < 1)
			{
				if (type is ArrayType array)
					return GetName(array.ElementType) + "[]";

				if (type is ByReferenceType byReference)
					return "&" + GetName(byReference.ElementType);

				ITypeDefinition? typeDefinition = type.GetDefinition();

				if (typeDefinition == null)
					return type.Name;

				if (typeDefinition.KnownTypeCode == KnownTypeCode.None)
				{
					if (type.DeclaringType == null)
						return type.Name.Replace('<', '❰').Replace('>', '❱'); // for module of executable
					else
						return type.DeclaringType.Name + '+' + type.Name; // nested types
				}

				return KnownTypeReference.GetCSharpNameByTypeCode(typeDefinition.KnownTypeCode) ?? type.Name;
			}

			// nullable types
			if (type.TryGetNullableType(out var nullableType))
				return GetName(nullableType) + "?";

			// other generic types
			string typeArguments = type.TypeArguments.Select(GetName).Join(", ");
			return type.Name + $"❰{typeArguments}❱";
		}
	}
}