// Copyright (c) 2014 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Decompiler type system for generic types or methods:
	/// used to replace the dummy type parameters by the actual type parameters of the method being decompiled.
	/// </summary>
	public class SpecializingDecompilerTypeSystem : IDecompilerTypeSystem
	{
		readonly IDecompilerTypeSystem context;
		readonly TypeParameterSubstitution substitution;
		
		public SpecializingDecompilerTypeSystem(IDecompilerTypeSystem context, TypeParameterSubstitution substitution)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (substitution == null)
				throw new ArgumentNullException(nameof(substitution));
			this.context = context;
			this.substitution = substitution;
		}

		internal IDecompilerTypeSystem Context {
			get { return context; }
		}
		
		public ICompilation Compilation {
			get { return context.Compilation; }
		}

		public TypeParameterSubstitution Substitution {
			get { return substitution; }
		}

		public IType Resolve(Mono.Cecil.TypeReference typeReference)
		{
			return context.Resolve(typeReference).AcceptVisitor(substitution);
		}

		public IField Resolve(Mono.Cecil.FieldReference fieldReference)
		{
			IField field = context.Resolve(fieldReference);
			if (field != null)
				field = (IField)field.Specialize(substitution);
			return field;
		}

		public IMethod Resolve(Mono.Cecil.MethodReference methodReference)
		{
			IMethod method = context.Resolve(methodReference);
			if (method != null)
				method = (IMethod)method.Specialize(substitution);
			return method;
		}

		public Mono.Cecil.TypeDefinition GetCecil(ITypeDefinition typeDefinition)
		{
			return context.GetCecil(typeDefinition);
		}

		public Mono.Cecil.MemberReference GetCecil(IMember member)
		{
			return context.GetCecil(member);
		}
	}
}
