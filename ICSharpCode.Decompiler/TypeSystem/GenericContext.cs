// Copyright (c) 2018 Daniel Grunwald
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
	public readonly struct GenericContext
	{
		public readonly IReadOnlyList<ITypeParameter> ClassTypeParameters;
		public readonly IReadOnlyList<ITypeParameter> MethodTypeParameters;

		public GenericContext(IReadOnlyList<ITypeParameter> classTypeParameters)
		{
			this.ClassTypeParameters = classTypeParameters;
			this.MethodTypeParameters = null;
		}

		public GenericContext(IReadOnlyList<ITypeParameter> classTypeParameters, IReadOnlyList<ITypeParameter> methodTypeParameters)
		{
			this.ClassTypeParameters = classTypeParameters;
			this.MethodTypeParameters = methodTypeParameters;
		}

		internal GenericContext(ITypeResolveContext context)
		{
			this.ClassTypeParameters = context.CurrentTypeDefinition?.TypeParameters;
			this.MethodTypeParameters = (context.CurrentMember as IMethod)?.TypeParameters;
		}

		internal GenericContext(IEntity context)
		{
			if (context is ITypeDefinition td)
			{
				this.ClassTypeParameters = td.TypeParameters;
				this.MethodTypeParameters = null;
			}
			else
			{
				this.ClassTypeParameters = context.DeclaringTypeDefinition?.TypeParameters;
				this.MethodTypeParameters = (context as IMethod)?.TypeParameters;
			}
		}

		public ITypeParameter GetClassTypeParameter(int index)
		{
			if (index < ClassTypeParameters?.Count)
				return ClassTypeParameters[index];
			else
				return DummyTypeParameter.GetClassTypeParameter(index);
		}

		public ITypeParameter GetMethodTypeParameter(int index)
		{
			if (index < MethodTypeParameters?.Count)
				return MethodTypeParameters[index];
			else
				return DummyTypeParameter.GetMethodTypeParameter(index);
		}

		internal TypeParameterSubstitution ToSubstitution()
		{
			// The TS prefers 'null' over empty lists in substitutions, and we need our substitution
			// to compare equal to the ones created by the TS.
			return new TypeParameterSubstitution(
				classTypeArguments: ClassTypeParameters?.Count > 0 ? ClassTypeParameters : null,
				methodTypeArguments: MethodTypeParameters?.Count > 0 ? MethodTypeParameters : null
			);
		}
	}
}
