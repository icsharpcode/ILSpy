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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public class FunctionPointerType : AbstractType
	{
		public static FunctionPointerType FromSignature(MethodSignature<IType> signature, MetadataModule module)
		{
			IType returnType = signature.ReturnType;
			bool returnIsRefReadOnly = false;
			var customCallConvs = ImmutableArray.CreateBuilder<IType>();
			while (returnType is ModifiedType modReturn)
			{
				if (modReturn.Modifier.IsKnownType(KnownAttribute.In))
				{
					returnType = modReturn.ElementType;
					returnIsRefReadOnly = true;
				}
				else if (modReturn.Modifier.Name.StartsWith("CallConv", StringComparison.Ordinal)
					&& modReturn.Modifier.Namespace == "System.Runtime.CompilerServices")
				{
					returnType = modReturn.ElementType;
					customCallConvs.Add(modReturn.Modifier);
				}
				else
				{
					break;
				}
			}
			var parameterTypes = ImmutableArray.CreateBuilder<IType>(signature.ParameterTypes.Length);
			var parameterReferenceKinds = ImmutableArray.CreateBuilder<ReferenceKind>(signature.ParameterTypes.Length);
			foreach (var p in signature.ParameterTypes)
			{
				IType paramType = p;
				ReferenceKind kind = ReferenceKind.None;
				if (p is ModifiedType modreq)
				{
					if (modreq.Modifier.IsKnownType(KnownAttribute.In))
					{
						kind = ReferenceKind.In;
						paramType = modreq.ElementType;
					}
					else if (modreq.Modifier.IsKnownType(KnownAttribute.Out))
					{
						kind = ReferenceKind.Out;
						paramType = modreq.ElementType;
					}
				}
				if (paramType.Kind == TypeKind.ByReference)
				{
					if (kind == ReferenceKind.None)
						kind = ReferenceKind.Ref;
				}
				else
				{
					kind = ReferenceKind.None;
				}
				parameterTypes.Add(paramType);
				parameterReferenceKinds.Add(kind);
			}
			return new FunctionPointerType(
				module, signature.Header.CallingConvention, customCallConvs.ToImmutable(),
				returnType, returnIsRefReadOnly,
				parameterTypes.MoveToImmutable(), parameterReferenceKinds.MoveToImmutable());
		}

		private readonly MetadataModule module;
		public readonly SignatureCallingConvention CallingConvention;
		public readonly ImmutableArray<IType> CustomCallingConventions;
		public readonly IType ReturnType;
		public readonly bool ReturnIsRefReadOnly;
		public readonly ImmutableArray<IType> ParameterTypes;
		public readonly ImmutableArray<ReferenceKind> ParameterReferenceKinds;

		public FunctionPointerType(MetadataModule module,
			SignatureCallingConvention callingConvention, ImmutableArray<IType> customCallingConventions,
			IType returnType, bool returnIsRefReadOnly,
			ImmutableArray<IType> parameterTypes, ImmutableArray<ReferenceKind> parameterReferenceKinds)
		{
			this.module = module;
			this.CallingConvention = callingConvention;
			this.CustomCallingConventions = customCallingConventions;
			this.ReturnType = returnType;
			this.ReturnIsRefReadOnly = returnIsRefReadOnly;
			this.ParameterTypes = parameterTypes;
			this.ParameterReferenceKinds = parameterReferenceKinds;
			Debug.Assert(parameterTypes.Length == parameterReferenceKinds.Length);
		}

		public override string Name => "delegate*";

		public override bool? IsReferenceType => false;

		public override TypeKind Kind => ((module.TypeSystemOptions & TypeSystemOptions.FunctionPointers) != 0) ? TypeKind.FunctionPointer : TypeKind.Struct;

		public override ITypeDefinition GetDefinition()
		{
			if ((module.TypeSystemOptions & TypeSystemOptions.FunctionPointers) != 0)
			{
				return null;
			}
			else
			{
				// If FunctionPointers are not enabled in the TS, we still use FunctionPointerType instances;
				// but have them act as if they were aliases for UIntPtr.
				return module.Compilation.FindType(KnownTypeCode.UIntPtr).GetDefinition();
			}
		}

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
				return new FunctionPointerType(
					module, CallingConvention, CustomCallingConventions,
					r, ReturnIsRefReadOnly,
					pt != null ? pt.ToImmutableArray() : ParameterTypes,
					ParameterReferenceKinds);
		}

		public override bool Equals(IType other)
		{
			return other is FunctionPointerType fpt
				&& CallingConvention == fpt.CallingConvention
				&& CustomCallingConventions.SequenceEqual(fpt.CustomCallingConventions)
				&& ReturnType.Equals(fpt.ReturnType)
				&& ReturnIsRefReadOnly == fpt.ReturnIsRefReadOnly
				&& ParameterTypes.SequenceEqual(fpt.ParameterTypes)
				&& ParameterReferenceKinds.SequenceEqual(fpt.ParameterReferenceKinds);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = ReturnType.GetHashCode() ^ CallingConvention.GetHashCode();
				foreach (var (p, k) in ParameterTypes.Zip(ParameterReferenceKinds))
				{
					hash ^= p.GetHashCode() ^ k.GetHashCode();
					hash *= 8310859;
				}
				return hash;
			}
		}

		internal IType WithSignature(IType returnType, ImmutableArray<IType> parameterTypes)
		{
			return new FunctionPointerType(this.module,
				this.CallingConvention, this.CustomCallingConventions,
				returnType, this.ReturnIsRefReadOnly,
				parameterTypes, this.ParameterReferenceKinds);
		}
	}
}
