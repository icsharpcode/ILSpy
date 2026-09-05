#nullable enable
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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	static class ILTypeExtensions
	{
		public static StackType GetStackType(this PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
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
					return StackType.Obj;
			}
		}

		public static Sign GetSign(this PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
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

		public static bool HasOppositeSign(this PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
				case PrimitiveType.I1:
				case PrimitiveType.I2:
				case PrimitiveType.I4:
				case PrimitiveType.I8:
				case PrimitiveType.U1:
				case PrimitiveType.U2:
				case PrimitiveType.U4:
				case PrimitiveType.U8:
				case PrimitiveType.I:
				case PrimitiveType.U:
					return true;
				default:
					return false;
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
			switch (type)
			{
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
			switch (type)
			{
				case PrimitiveType.R4:
				case PrimitiveType.R8:
				case PrimitiveType.R:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Infers the C# type an instruction expects of the child in <paramref name="childIndex"/>,
		/// i.e. the counterpart to <see cref="ILInstruction.InferType"/>: that one asks what a value is, this one
		/// asks what the position it flows into says it should be.
		///
		/// Returns SpecialType.UnknownType where the position names nothing.
		/// </summary>
		/// <remarks>
		/// Where a value's own type is only its stack type - `I4` being `int`, `bool`, `char` and
		/// every enum at once - the consumer often still knows, because a parameter, a return type
		/// or a field carries its type in metadata.
		/// </remarks>
		public static IType InferExpectedType(this ILInstruction inst, int childIndex, ICompilation? compilation)
		{
			switch (inst)
			{
				case CallInstruction call:
					if (childIndex == 0 && call.IsInstanceCall)
						return call.ConstrainedTo ?? call.Method.DeclaringType;
					return call.GetParameter(childIndex)?.Type ?? SpecialType.UnknownType;
				case Leave leave when childIndex == 0:
					// the value of a leave is a return value only where it leaves the function body
					var function = leave.Ancestors.OfType<ILFunction>().FirstOrDefault();
					if (function == null || leave.TargetContainer != function.Body)
						return SpecialType.UnknownType;
					return function.Method?.ReturnType ?? SpecialType.UnknownType;
				case StObj stobj when childIndex == 1:
					return stobj.Type;
				case StLoc stloc when childIndex == 0:
					return stloc.Variable.Type;
				case IfInstruction ifInst when childIndex == 0:
					return compilation?.FindType(KnownTypeCode.Boolean) ?? SpecialType.UnknownType;
				case NewArr newArr:
					return compilation?.FindType(KnownTypeCode.Int32) ?? SpecialType.UnknownType;
				default:
					return SpecialType.UnknownType;
			}
		}
	}
}
