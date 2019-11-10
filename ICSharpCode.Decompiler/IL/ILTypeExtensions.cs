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

using ICSharpCode.Decompiler.TypeSystem;
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.IL
{
	static class ILTypeExtensions
	{
		public static StackType GetStackType(this PrimitiveType primitiveType)
		{
			switch (primitiveType) {
				case PrimitiveType.I1:
				case PrimitiveType.U1:
				case PrimitiveType.I2:
				case PrimitiveType.U2:
				case PrimitiveType.I4:
				case PrimitiveType.U4:
					return StackType.I4;
				case PrimitiveType.I8:
				case PrimitiveType.U8:
					return StackType.I8;
				case PrimitiveType.I:
				case PrimitiveType.U:
					return StackType.I;
				case PrimitiveType.R4:
					return StackType.F4;
				case PrimitiveType.R8:
				case PrimitiveType.R:
					return StackType.F8;
				case PrimitiveType.Ref: // ByRef
					return StackType.Ref;
				case PrimitiveType.Unknown:
					return StackType.Unknown;
				default:
					return StackType.O;
			}
		}
		
		public static Sign GetSign(this PrimitiveType primitiveType)
		{
			switch (primitiveType) {
				case PrimitiveType.I1:
				case PrimitiveType.I2:
				case PrimitiveType.I4:
				case PrimitiveType.I8:
				case PrimitiveType.R4:
				case PrimitiveType.R8:
				case PrimitiveType.R:
				case PrimitiveType.I:
					return Sign.Signed;
				case PrimitiveType.U1:
				case PrimitiveType.U2:
				case PrimitiveType.U4:
				case PrimitiveType.U8:
				case PrimitiveType.U:
					return Sign.Unsigned;
				default:
					return Sign.None;
			}
		}
		
		/// <summary>
		/// Gets the size in bytes of the primitive type.
		/// 
		/// Returns 0 for non-primitive types.
		/// Returns <c>NativeIntSize</c> for native int/references.
		/// </summary>
		public static int GetSize(this PrimitiveType type)
		{
			switch (type) {
				case PrimitiveType.I1:
				case PrimitiveType.U1:
					return 1;
				case PrimitiveType.I2:
				case PrimitiveType.U2:
					return 2;
				case PrimitiveType.I4:
				case PrimitiveType.U4:
				case PrimitiveType.R4:
					return 4;
				case PrimitiveType.I8:
				case PrimitiveType.R8:
				case PrimitiveType.U8:
				case PrimitiveType.R:
					return 8;
				case PrimitiveType.I:
				case PrimitiveType.U:
				case PrimitiveType.Ref:
					return TypeUtils.NativeIntSize;
				default:
					return 0;
			}
		}
		
		/// <summary>
		/// Gets whether the type is a small integer type.
		/// Small integer types are:
		/// * bool, sbyte, byte, char, short, ushort
		/// * any enums that have a small integer type as underlying type
		/// </summary>
		public static bool IsSmallIntegerType(this PrimitiveType type)
		{
			return GetSize(type) < 4;
		}
		
		public static bool IsIntegerType(this PrimitiveType primitiveType)
		{
			return primitiveType.GetStackType().IsIntegerType();
		}

		public static bool IsFloatType(this PrimitiveType type)
		{
			switch (type) {
				case PrimitiveType.R4:
				case PrimitiveType.R8:
				case PrimitiveType.R:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Infers the C# type for an IL instruction.
		/// 
		/// Returns SpecialType.UnknownType for unsupported instructions.
		/// </summary>
		public static IType InferType(this ILInstruction inst, ICompilation compilation)
		{
			switch (inst) {
				case NewObj newObj:
					return newObj.Method.DeclaringType;
				case NewArr newArr:
					if (compilation != null)
						return new ArrayType(compilation, newArr.Type, newArr.Indices.Count);
					else
						return SpecialType.UnknownType;
				case Call call:
					return call.Method.ReturnType;
				case CallVirt callVirt:
					return callVirt.Method.ReturnType;
				case CallIndirect calli:
					return calli.ReturnType;
				case UserDefinedLogicOperator logicOp:
					return logicOp.Method.ReturnType;
				case LdObj ldobj:
					return ldobj.Type;
				case StObj stobj:
					return stobj.Type;
				case LdLoc ldloc:
					return ldloc.Variable.Type;
				case StLoc stloc:
					return stloc.Variable.Type;
				case LdLoca ldloca:
					return new ByReferenceType(ldloca.Variable.Type);
				case LdFlda ldflda:
					return new ByReferenceType(ldflda.Field.Type);
				case LdsFlda ldsflda:
					return new ByReferenceType(ldsflda.Field.Type);
				case LdElema ldelema:
					if (ldelema.Array.InferType(compilation) is ArrayType arrayType) {
						if (TypeUtils.IsCompatibleTypeForMemoryAccess(arrayType.ElementType, ldelema.Type)) {
							return new ByReferenceType(arrayType.ElementType);
						}
					}
					return new ByReferenceType(ldelema.Type);
				case Comp comp:
					if (compilation == null)
						return SpecialType.UnknownType;
					switch (comp.LiftingKind) {
						case ComparisonLiftingKind.None:
						case ComparisonLiftingKind.CSharp:
							return compilation.FindType(KnownTypeCode.Boolean);
						case ComparisonLiftingKind.ThreeValuedLogic:
							return NullableType.Create(compilation, compilation.FindType(KnownTypeCode.Boolean));
						default:
							return SpecialType.UnknownType;
					}
				case BinaryNumericInstruction bni:
					if (bni.IsLifted)
						return SpecialType.UnknownType;
					switch (bni.Operator) {
						case BinaryNumericOperator.BitAnd:
						case BinaryNumericOperator.BitOr:
						case BinaryNumericOperator.BitXor:
							var left = bni.Left.InferType(compilation);
							var right = bni.Right.InferType(compilation);
							if (left.Equals(right) && (left.IsCSharpPrimitiveIntegerType() || left.IsKnownType(KnownTypeCode.Boolean)))
								return left;
							else
								return SpecialType.UnknownType;
						default:
							return SpecialType.UnknownType;
					}
				default:
					return SpecialType.UnknownType;
			}
		}
	}
}
