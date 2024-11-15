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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	partial class ClassDiagrammerFactory
	{
		/// <summary>Wraps a <see cref="CSharpDecompiler"/> method configurable via <see cref="decompilerSettings"/>
		/// that can be used to determine whether a member should be hidden.</summary>
		private bool IsHidden(IEntity entity) => CSharpDecompiler.MemberIsHidden(entity.ParentModule!.MetadataFile, entity.MetadataToken, decompilerSettings);

		private IField[] GetFields(ITypeDefinition type, IProperty[] properties)
			// only display fields that are not backing properties of the same name and type
			=> type.GetFields(f => !IsHidden(f) // removes compiler-generated backing fields
				/* tries to remove remaining manual backing fields by matching type and name */
				&& !properties.Any(p => f.ReturnType.Equals(p.ReturnType)
					&& Regex.IsMatch(f.Name, "_?" + p.Name, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.NonBacktracking))).ToArray();

		private static IEnumerable<IMethod> GetMethods(ITypeDefinition type) => type.GetMethods(m =>
			!m.IsOperator && !m.IsCompilerGenerated()
			&& (m.DeclaringType == type // include methods if self-declared
				/* but exclude methods declared by object and their overrides, if inherited */
				|| (!m.DeclaringType.IsObject()
					&& (!m.IsOverride || !InheritanceHelper.GetBaseMember(m).DeclaringType.IsObject()))));

		private string FormatMethod(IMethod method)
		{
			string parameters = method.Parameters.Select(p => $"{GetName(p.Type)} {p.Name}").Join(", ");
			string? modifier = method.IsAbstract ? "*" : method.IsStatic ? "$" : default;
			string name = method.Name;

			if (method.IsExplicitInterfaceImplementation)
			{
				IMember member = method.ExplicitlyImplementedInterfaceMembers.Single();
				name = GetName(member.DeclaringType) + '.' + member.Name;
			}

			string? typeArguments = method.TypeArguments.Count == 0 ? null : $"❰{method.TypeArguments.Select(GetName).Join(", ")}❱";
			return $"{GetAccessibility(method.Accessibility)}{name}{typeArguments}({parameters}){modifier} {GetName(method.ReturnType)}";
		}

		private string FormatFlatProperty(IProperty property)
		{
			char? visibility = GetAccessibility(property.Accessibility);
			string? modifier = property.IsAbstract ? "*" : property.IsStatic ? "$" : default;
			return $"{visibility}{GetName(property.ReturnType)} {property.Name}{modifier}";
		}

		private string FormatField(IField field)
		{
			string? modifier = field.IsAbstract ? "*" : field.IsStatic ? "$" : default;
			return $"{GetAccessibility(field.Accessibility)}{GetName(field.ReturnType)} {field.Name}{modifier}";
		}

		// see https://stackoverflow.com/a/16024302 for accessibility modifier flags
		private static char? GetAccessibility(Accessibility access) => access switch {
			Accessibility.Private => '-',
			Accessibility.ProtectedAndInternal or Accessibility.Internal => '~',
			Accessibility.Protected or Accessibility.ProtectedOrInternal => '#',
			Accessibility.Public => '+',
			_ => default,
		};
	}
}