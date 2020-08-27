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

using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Resolver
{
	/// <summary>
	/// Represents an anonymous method or lambda expression.
	/// Note: the lambda has no type.
	/// To retrieve the delegate type, look at the anonymous function conversion.
	/// </summary>
	public abstract class LambdaResolveResult : ResolveResult
	{
		protected LambdaResolveResult() : base(SpecialType.NoType)
		{
		}

		/// <summary>
		/// Gets whether there is a parameter list.
		/// This property always returns true for C# 3.0-lambdas, but may return false
		/// for C# 2.0 anonymous methods.
		/// </summary>
		public abstract bool HasParameterList { get; }

		/// <summary>
		/// Gets whether this lambda is using the C# 2.0 anonymous method syntax.
		/// </summary>
		public abstract bool IsAnonymousMethod { get; }

		/// <summary>
		/// Gets whether the lambda parameters are implicitly typed.
		/// </summary>
		/// <remarks>This property returns false for anonymous methods without parameter list.</remarks>
		public abstract bool IsImplicitlyTyped { get; }

		/// <summary>
		/// Gets whether the lambda is async.
		/// </summary>
		public abstract bool IsAsync { get; }

		/// <summary>
		/// Gets the return type inferred when the parameter types are inferred to be <paramref name="parameterTypes"/>
		/// </summary>
		/// <remarks>
		/// This method determines the return type inferred from the lambda body, which is used as part of C# type inference.
		/// Use the <see cref="ReturnType"/> property to retrieve the actual return type as determined by the target delegate type.
		/// </remarks>
		public abstract IType GetInferredReturnType(IType[] parameterTypes);

		/// <summary>
		/// Gets the list of parameters.
		/// </summary>
		public abstract IReadOnlyList<IParameter> Parameters { get; }

		/// <summary>
		/// Gets the return type of the lambda.
		/// 
		/// If the lambda is async, the return type includes <code>Task&lt;T&gt;</code>
		/// </summary>
		public abstract IType ReturnType { get; }

		/// <summary>
		/// Gets whether the lambda body is valid for the given parameter types and return type.
		/// </summary>
		/// <returns>
		/// Produces a conversion with <see cref="Conversion.IsAnonymousFunctionConversion"/>=<c>true</c> if the lambda is valid;
		/// otherwise returns <see cref="Conversion.None"/>.
		/// </returns>
		public abstract Conversion IsValid(IType[] parameterTypes, IType returnType, CSharpConversions conversions);

		/// <summary>
		/// Gets the resolve result for the lambda body.
		/// Returns a resolve result for 'void' for statement lambdas.
		/// </summary>
		public abstract ResolveResult Body { get; }

		public override IEnumerable<ResolveResult> GetChildResults()
		{
			return new[] { this.Body };
		}
	}

	sealed class DecompiledLambdaResolveResult : LambdaResolveResult
	{
		readonly IL.ILFunction function;
		public readonly IType DelegateType;

		/// <summary>
		/// The inferred return type.
		/// Can differ from <c>ReturnType</c> if a return statement
		/// performs an implicit conversion.
		/// </summary>
		public IType InferredReturnType;

		public DecompiledLambdaResolveResult(IL.ILFunction function,
			IType delegateType,
			IType inferredReturnType,
			bool hasParameterList,
			bool isAnonymousMethod,
			bool isImplicitlyTyped)
		{
			this.function = function ?? throw new ArgumentNullException(nameof(function));
			this.DelegateType = delegateType ?? throw new ArgumentNullException(nameof(delegateType));
			this.InferredReturnType = inferredReturnType ?? throw new ArgumentNullException(nameof(inferredReturnType));
			this.HasParameterList = hasParameterList;
			this.IsAnonymousMethod = isAnonymousMethod;
			this.IsImplicitlyTyped = isImplicitlyTyped;
			this.Body = new ResolveResult(SpecialType.UnknownType);
		}

		public override bool HasParameterList { get; }
		public override bool IsAnonymousMethod { get; }
		public override bool IsImplicitlyTyped { get; }
		public override bool IsAsync => function.IsAsync;

		public override IReadOnlyList<IParameter> Parameters => function.Parameters;
		public override IType ReturnType => function.ReturnType;

		public override ResolveResult Body { get; }

		public override IType GetInferredReturnType(IType[] parameterTypes)
		{
			// We don't know how to compute which type would be inferred if
			// given other parameter types.
			// Let's hope this is good enough:
			return InferredReturnType;
		}

		public override Conversion IsValid(IType[] parameterTypes, IType returnType, CSharpConversions conversions)
		{
			// Anonymous method expressions without parameter lists are applicable to any parameter list.
			if (HasParameterList)
			{
				if (this.Parameters.Count != parameterTypes.Length)
					return Conversion.None;
				for (int i = 0; i < parameterTypes.Length; ++i)
				{
					if (!conversions.IdentityConversion(parameterTypes[i], this.Parameters[i].Type))
					{
						if (IsImplicitlyTyped)
						{
							// it's possible that different parameter types also lead to a valid conversion
							return LambdaConversion.Instance;
						}
						else
						{
							return Conversion.None;
						}
					}
				}
			}
			if (conversions.IdentityConversion(this.ReturnType, returnType)
				|| conversions.ImplicitConversion(this.InferredReturnType, returnType).IsValid)
			{
				return LambdaConversion.Instance;
			}
			else
			{
				return Conversion.None;
			}
		}
	}

	class LambdaConversion : Conversion
	{
		public static readonly LambdaConversion Instance = new LambdaConversion();

		public override bool IsAnonymousFunctionConversion => true;
		public override bool IsImplicit => true;
	}
}
