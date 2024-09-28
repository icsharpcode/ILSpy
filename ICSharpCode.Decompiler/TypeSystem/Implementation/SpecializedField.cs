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

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IField (field after type substitution).
	/// </summary>
	public class SpecializedField : SpecializedMember, IField
	{
		internal static IField Create(IField fieldDefinition, TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution) || fieldDefinition.DeclaringType.TypeParameterCount == 0)
			{
				return fieldDefinition;
			}
			if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
				substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			return new SpecializedField(fieldDefinition, substitution);
		}

		readonly IField fieldDefinition;

		public SpecializedField(IField fieldDefinition, TypeParameterSubstitution substitution)
			: base(fieldDefinition)
		{
			this.fieldDefinition = fieldDefinition;
			AddSubstitution(substitution);
		}

		public bool IsReadOnly => fieldDefinition.IsReadOnly;
		public bool ReturnTypeIsRefReadOnly => fieldDefinition.ReturnTypeIsRefReadOnly;
		public bool IsVolatile => fieldDefinition.IsVolatile;

		IType IVariable.Type {
			get { return this.ReturnType; }
		}

		public bool IsConst {
			get { return fieldDefinition.IsConst; }
		}

		public object? GetConstantValue(bool throwOnInvalidMetadata)
		{
			return fieldDefinition.GetConstantValue(throwOnInvalidMetadata);
		}
	}
}
