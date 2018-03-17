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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class WellKnownConstants
	{
		public const byte ByteMaxValue = byte.MaxValue;
		public const byte ByteMinValue = 0;

		public const sbyte SByteMaxValue = sbyte.MaxValue;
		public const sbyte SByteMinValue = sbyte.MinValue;

		public const ushort UShortMaxValue = ushort.MaxValue;
		public const ushort UShortMinValue = 0;

		public const short ShortMaxValue = short.MinValue;
		public const short ShortMinValue = short.MaxValue;

		public const uint UIntMaxValue = uint.MaxValue;
		public const uint UIntMinValue = 0u;

		public const int IntMaxValue = int.MaxValue;
		public const int IntMinValue = int.MinValue;

		public const ulong ULongMaxValue = ulong.MaxValue;
		public const ulong ULongMinValue = 0uL;

		public const long LongMaxValue = long.MaxValue;
		public const long LongMinValue = long.MinValue;

		public const float FloatZero = 0f;
		public const float FloatMinusZero = -0f;
		public const float FloatNaN = float.NaN;
		public const float FloatPositiveInfinity = float.PositiveInfinity;
		public const float FloatNegativeInfinity = float.NegativeInfinity;
		public const float FloatMaxValue = float.MaxValue;
		public const float FloatMinValue = float.MinValue;
		public const float FloatEpsilon = float.Epsilon;

		public const double DoubleZero = 0.0;
		public const double DoubleMinusZero = -0.0;
		public const double DoubleNaN = double.NaN;
		public const double DoublePositiveInfinity = double.PositiveInfinity;
		public const double DoubleNegativeInfinity = double.NegativeInfinity;
		public const double DoubleMaxValue = double.MaxValue;
		public const double DoubleMinValue = double.MinValue;
		public const double DoubleEpsilon = double.Epsilon;

		public const decimal DecimalMaxValue = decimal.MaxValue;
		public const decimal DecimalMinValue = decimal.MinValue;
	}
}