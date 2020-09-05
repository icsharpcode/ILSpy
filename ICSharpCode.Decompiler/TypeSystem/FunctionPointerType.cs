// Copyright (c) 2020 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public class FunctionPointerType : AbstractType
	{
		public readonly SignatureCallingConvention CallingConvention;
		public readonly IType ReturnType;
		public readonly bool ReturnIsRefReadOnly;
		public readonly ImmutableArray<IType> ParameterTypes;
		public readonly ImmutableArray<ReferenceKind> ParameterReferenceKinds;

		public FunctionPointerType(SignatureCallingConvention callingConvention,
			IType returnType, bool returnIsRefReadOnly,
			ImmutableArray<IType> parameterTypes, ImmutableArray<ReferenceKind> parameterReferenceKinds)
		{
			this.CallingConvention = callingConvention;
			this.ReturnType = returnType;
			this.ReturnIsRefReadOnly = returnIsRefReadOnly;
			this.ParameterTypes = parameterTypes;
			this.ParameterReferenceKinds = parameterReferenceKinds;
			Debug.Assert(parameterTypes.Length == parameterReferenceKinds.Length);
		}

		public override string Name => "delegate*";

		public override bool? IsReferenceType => false;

		public override TypeKind Kind => TypeKind.FunctionPointer;

		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitFunctionPointerType(this);
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType r = ReturnType.AcceptVisitor(visitor);
			// Keep ta == null as long as no elements changed, allocate the array only if necessary.
			IType[] pt = (r != ReturnType) ? new IType[ParameterTypes.Length] : null;
			for (int i = 0; i < ParameterTypes.Length; i++)
			{
				IType p = ParameterTypes[i].AcceptVisitor(visitor);
				if (p == null)
					throw new NullReferenceException("TypeVisitor.Visit-method returned null");
				if (pt == null && p != ParameterTypes[i])
				{
					// we found a difference, so we need to allocate the array
					pt = new IType[ParameterTypes.Length];
					for (int j = 0; j < i; j++)
					{
						pt[j] = ParameterTypes[j];
					}
				}
				if (pt != null)
					pt[i] = p;
			}
			if (pt == null)
				return this;
			else
				return new FunctionPointerType(CallingConvention,
					r, ReturnIsRefReadOnly,
					pt != null ? pt.ToImmutableArray() : ParameterTypes,
					ParameterReferenceKinds);
		}

		public override bool Equals(IType other)
		{
			return other is FunctionPointerType fpt && ReturnType.Equals(fpt.ReturnType)
				&& ReturnIsRefReadOnly == fpt.ReturnIsRefReadOnly
				&& ParameterTypes.SequenceEqual(fpt.ParameterTypes)
				&& ParameterReferenceKinds.SequenceEqual(fpt.ParameterReferenceKinds);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = ReturnType.GetHashCode();
				foreach (var p in ParameterTypes)
				{
					hash ^= p.GetHashCode();
					hash *= 8310859;
				}
				return hash;
			}
		}
	}
}
