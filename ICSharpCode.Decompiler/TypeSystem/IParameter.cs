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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <remarks>
	/// Should match order in <see cref="CSharp.Syntax.FieldDirection"/>.
	/// </remarks>
	public enum ReferenceKind : byte
	{
		None,
		Out,
		Ref,
		In,
		RefReadOnly,
	}

	public struct LifetimeAnnotation
	{
		/// <summary>
		/// C# 11 scoped annotation: "scoped ref" (ScopedRefAttribute)
		/// </summary>
		public bool ScopedRef {
#pragma warning disable 618
			get { return RefScoped; }
			set { RefScoped = value; }
#pragma warning restore 618
		}

		[Obsolete("Use ScopedRef property instead of directly accessing this field")]
		public bool RefScoped;

		[Obsolete("C# 11 preview: \"ref scoped\" no longer supported")]
		public bool ValueScoped;
	}

	public interface IParameter : IVariable
	{
		/// <summary>
		/// Gets the attributes on this parameter.
		/// </summary>
		IEnumerable<IAttribute> GetAttributes();

		/// <summary>
		/// Gets the reference kind of this parameter.
		/// </summary>
		ReferenceKind ReferenceKind { get; }

		/// <summary>
		/// C# 11 scoped annotation.
		/// </summary>
		LifetimeAnnotation Lifetime { get; }

		/// <summary>
		/// Gets whether this parameter is a C# 'params' parameter.
		/// </summary>
		bool IsParams { get; }

		/// <summary>
		/// Gets whether this parameter is optional.
		/// The default value is given by the <see cref="IVariable.GetConstantValue"/> function.
		/// </summary>
		bool IsOptional { get; }

		/// <summary>
		/// Gets whether this parameter has a constant value when presented in method signature.
		/// </summary>
		/// <remarks>
		/// This can only be <c>true</c> if the parameter is optional, and it's true for most 
		/// optional parameters. However it is possible to compile a parameter without a default value,
		/// and some parameters handle their default values in an special way.
		/// 
		/// For example, <see cref="DecimalConstantAttribute"/> does not use normal constants,
		/// so when <see cref="DecompilerSettings.DecimalConstants" /> is <c>false</c>
		/// we expose <c>DecimalConstantAttribute</c> directly instead of a constant value.
		/// 
		/// On the call sites, though, we can still use the value inferred from the attribute.
		/// </remarks>
		bool HasConstantValueInSignature { get; }

		/// <summary>
		/// Gets the owner of this parameter.
		/// May return null; for example when parameters belong to lambdas or anonymous methods.
		/// </summary>
		IParameterizedMember? Owner { get; }
	}

	internal static class IParameterExtensions
	{
		/// <summary>
		/// Checks if the parameter is allowed to be assigned a default value.
		/// </summary>
		/// <remarks>
		/// This checks <see cref="IParameter.IsOptional"/>, <see cref="IParameter.HasConstantValueInSignature"/>, <see cref="IParameter.ReferenceKind"/>,
		/// and <see cref="IParameter.IsParams"/> on this parameter and all subsequent parameters.
		/// If the parameter has no <see cref="IParameter.Owner"/>, it does not check subsequent parameters.
		/// </remarks>
		/// <param name="parameter">The parameter</param>
		/// <returns>True if the <paramref name="parameter"/> has a default value and is allowed to be assigned a default value.</returns>
		public static bool IsDefaultValueAssignmentAllowed(this IParameter parameter)
		{
			if (!DefaultValueAssignmentAllowedIndividual(parameter))
				return false;

			if (parameter.Owner == null)
				return true;

			for (int i = parameter.Owner.Parameters.Count - 1; i >= 0; i--)
			{
				IParameter otherParameter = parameter.Owner.Parameters[i];
				if (otherParameter == parameter)
					break;

				if (DefaultValueAssignmentAllowedIndividual(otherParameter) || otherParameter.IsParams)
					continue;

				return false;
			}
			return true;

			static bool DefaultValueAssignmentAllowedIndividual(IParameter parameter)
			{
				return parameter.IsOptional && parameter.HasConstantValueInSignature && parameter.ReferenceKind is ReferenceKind.None or ReferenceKind.In or ReferenceKind.RefReadOnly;
			}
		}
	}
}
