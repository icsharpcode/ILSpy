// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class EnumTests
	{
		public enum SimpleEnum
		{
			Item1,
			Item2
		}

		public enum NoZero
		{
			Item1 = 1,
			Item2
		}

		public enum OutOfOrderMembers
		{
			Item1 = 1,
			Item0 = 0
		}

		public enum EnumSkippedItemTest
		{
			Item0 = 0,
			Item2 = 2
		}

		public enum EnumDuplicateItemTest
		{
			Item0 = 0,
			Item1 = 1,
			Item2A = 2,
			Item2B = Item2A
		}

		public enum LongBasedEnum : long
		{
			Item1,
			Item2
		}

		public enum LongWithInitializers : long
		{
			Item1 = 0L,
			Item2 = 20L,
			Item3 = 21L
		}

		public enum ShortWithInitializers : short
		{
			Item1 = 0,
			Item2 = 20,
			Item3 = 21
		}

		public enum ByteWithInitializers : byte
		{
			Item1 = 0,
			Item2 = 20,
			Item3 = 21
		}

		[Flags]
		public enum SimpleFlagsEnum
		{
			None = 0,
			Item1 = 1,
			Item2 = 2,
			Item3 = 4,
			All = Item1 | Item2 | Item3
		}

		[Flags]
		public enum SelfReferentialEnum
		{
			None = 0,
			Item1 = 1,
			Item2 = Item1,
			Item3 = 3
		}

		[Flags]
		public enum NegativeValueWithFlags
		{
			Value = -2147483647
		}

		public enum NegativeValueWithoutFlags
		{
			Value = -2147483647
		}

		public enum NonFlagsDuplicateItems
		{
			OK = 200,
			Default = OK,
			NoContent = 204
		}

		public enum ZeroDuplicateItems
		{
			Unknown = 0,
			Default = Unknown
		}

		[Flags]
		public enum ZeroDuplicateFlags
		{
			None = 0,
			Default = 0,
			Item1 = 1
		}

		[Flags]
		public enum MaskFamilyFlags
		{
			VisibilityMask = 7,
			NotPublic = 0,
			Public = 1,
			NestedPublic = 2,
			NestedPrivate = 3,
			LayoutMask = 0x18,
			AutoLayout = 0,
			SequentialLayout = 8,
			ExplicitLayout = 0x10
		}

		[Flags]
		public enum UnsignedFlags : uint
		{
			None = 0u,
			Item1 = 1u,
			Item2 = 2u,
			Item3 = 4u,
			All = uint.MaxValue,
			NotItem1 = ~Item1
		}

		[Flags]
		public enum ByteFlags : byte
		{
			None = 0,
			Item1 = 1,
			Item2 = 2,
			NotItem2 = 0xFD
		}

		[Flags]
		public enum ShortFlags : short
		{
			None = 0,
			Item1 = 1,
			NotItem1 = ~Item1
		}

		[Flags]
		public enum FlagsWithNegation
		{
			None = 0,
			Item1 = 1,
			Item2 = 2,
			NotItem1 = ~Item1
		}

		public AttributeTargets SingleEnumValue()
		{
			return AttributeTargets.Class;
		}

		public AttributeTargets TwoEnumValuesOr()
		{
			return AttributeTargets.Class | AttributeTargets.Method;
		}

		public AttributeTargets ThreeEnumValuesOr()
		{
			return AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter;
		}

		public AttributeTargets UnknownEnumValue()
		{
			return (AttributeTargets)1000000;
		}

		public AttributeTargets EnumAllValue()
		{
			return AttributeTargets.All;
		}

		public AttributeTargets EnumZeroValue()
		{
			return (AttributeTargets)0;
		}

		public object PreservingTypeWhenBoxed()
		{
			return AttributeTargets.Delegate;
		}

		public object PreservingTypeWhenBoxedTwoEnum()
		{
			return AttributeTargets.Class | AttributeTargets.Delegate;
		}

		public SimpleFlagsEnum SignedEnumComplement()
		{
			return ~SimpleFlagsEnum.Item1;
		}

		public UnsignedFlags UnsignedEnumComplement()
		{
			return ~UnsignedFlags.Item2;
		}

		public ByteFlags ByteEnumComplement()
		{
			return ~ByteFlags.Item1;
		}

		public void EnumInNotZeroCheck(SimpleEnum value, NoZero value2)
		{
			if (value != SimpleEnum.Item1)
			{
				Console.WriteLine();
			}

			if (value2 != 0)
			{
				Console.WriteLine();
			}
		}
	}
}
