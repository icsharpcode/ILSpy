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

		// This constant is (1 / (double)long.MaxValue). Note that the (double) cast involves
		// loss of precision: long.MaxValue is rounded up to (long.MaxValue+1), which is a power of two.
		// The division then is exact, resulting in the double 0x3c00000000000000.
		// When trying to represent this as a fraction, we get (long.MaxValue+1) as divisor, which
		// does not fit type long, but compares equals to long.MaxValue due to the long->double conversion.
		public const double Double_One_Div_LongMaxValue = 1.0842021724855044E-19;

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

		public const float Float_One = 1f;
		public const double Double_One = 1.0;
		public const float Float_Two = 2f;
		public const double Double_Two = 2.0;
		public const float Float_Sixth = 1f / 6f;
		public const double Double_Sixth = 1.0 / 6.0;
		public const float Float_Tenth = 0.1f;
		public const double Double_Tenth = 0.1;

#if ROSLYN && !NET40
		public const float Float_PI = MathF.PI;
		public const float Float_HalfOfPI = MathF.PI / 2f;
		public const float Float_QuarterOfPI = MathF.PI / 4f;
		public const float Float_PITimes2 = MathF.PI * 2f;
		public const float Float_3QuartersOfPI = MathF.PI * 3f / 4f;
		public const float Float_PIDiv360 = MathF.PI / 360f;
		public const float Float_PIDiv16 = MathF.PI / 16f;
		public const float Float_PIDiv32 = MathF.PI / 32f;
		public const float Float_PIInverseFraction = 1f / MathF.PI;
		public const float Float_PIInverseFraction2 = 2f / MathF.PI;
		public const float Float_PIInverseFraction5 = 5f / MathF.PI;
		public const float Float_PITimes90 = MathF.PI * 90f;
		public const float Float_PITimes180 = MathF.PI * 180f;
		public const float Float_LooksLikePI = 3.1415925f;
		public const float Float_LooksLikePI2 = 3.14159f;
		public const float Float_LooksLikePI3 = 3.141f;
		public const float Float_BeforePI = 3.1415925f;
		public const float Float_AfterPI = 3.141593f;
		public const float Float_Negated_PI = -MathF.PI;
		public const float Float_Negated_HalfOfPI = -MathF.PI / 2f;
		public const float Float_Negated_QuarterOfPI = -MathF.PI / 4f;
		public const float Float_Negated_PITimes2 = MathF.PI * -2f;
		public const float Float_Negated_3QuartersOfPI = MathF.PI * -3f / 4f;
		public const float Float_Negated_PIDiv360 = -MathF.PI / 360f;
		public const float Float_Negated_PIDiv16 = -MathF.PI / 16f;
		public const float Float_Negated_PIDiv32 = -MathF.PI / 32f;
		public const float Float_Negated_PIInverseFraction = -1f / MathF.PI;
		public const float Float_Negated_PIInverseFraction2 = -2f / MathF.PI;
		public const float Float_Negated_PIInverseFraction5 = -5f / MathF.PI;
		public const float Float_Negated_PITimes90 = MathF.PI * -90f;
		public const float Float_Negated_PITimes180 = MathF.PI * -180f;
		public const float Float_Negated_LooksLikePI = -3.141f;
		public const float Float_Negated_BeforePI = -3.1415925f;
		public const float Float_Negated_AfterPI = -3.141593f;

		public const float Float_E = MathF.E;
		public const float Float_Negated_E = -MathF.E;
#else
		public const float Float_PI = (float)Math.PI;
		public const float Float_HalfOfPI = (float)Math.PI / 2f;
		public const float Float_QuarterOfPI = (float)Math.PI / 4f;
		public const float Float_PITimes2 = (float)Math.PI * 2f;
		public const float Float_3QuartersOfPI = (float)Math.PI * 3f / 4f;
		public const float Float_PIDiv360 = (float)Math.PI / 360f;
		public const float Float_PIDiv16 = (float)Math.PI / 16f;
		public const float Float_PIDiv32 = (float)Math.PI / 32f;
		public const float Float_PIInverseFraction = 1f / (float)Math.PI;
		public const float Float_PIInverseFraction2 = 2f / (float)Math.PI;
		public const float Float_PIInverseFraction5 = 5f / (float)Math.PI;
		public const float Float_PITimes90 = (float)Math.PI * 90f;
		public const float Float_PITimes180 = (float)Math.PI * 180f;
		public const float Float_LooksLikePI = 3.1415925f;
		public const float Float_LooksLikePI2 = 3.14159f;
		public const float Float_LooksLikePI3 = 3.141f;
		public const float Float_BeforePI = 3.1415925f;
		public const float Float_AfterPI = 3.141593f;
		public const float Float_Negated_PI = -(float)Math.PI;
		public const float Float_Negated_HalfOfPI = -(float)Math.PI / 2f;
		public const float Float_Negated_QuarterOfPI = -(float)Math.PI / 4f;
		public const float Float_Negated_PITimes2 = (float)Math.PI * -2f;
		public const float Float_Negated_3QuartersOfPI = (float)Math.PI * -3f / 4f;
		public const float Float_Negated_PIDiv360 = -(float)Math.PI / 360f;
		public const float Float_Negated_PIDiv16 = -(float)Math.PI / 16f;
		public const float Float_Negated_PIDiv32 = -(float)Math.PI / 32f;
		public const float Float_Negated_PIInverseFraction = -1f / (float)Math.PI;
		public const float Float_Negated_PIInverseFraction2 = -2f / (float)Math.PI;
		public const float Float_Negated_PIInverseFraction5 = -5f / (float)Math.PI;
		public const float Float_Negated_PITimes90 = (float)Math.PI * -90f;
		public const float Float_Negated_PITimes180 = (float)Math.PI * -180f;
		public const float Float_Negated_LooksLikePI = -3.141f;
		public const float Float_Negated_BeforePI = -3.1415925f;
		public const float Float_Negated_AfterPI = -3.141593f;

		public const float Float_E = (float)Math.E;
		public const float Float_Negated_E = -(float)Math.E;
#endif

		public const double Double_PI = Math.PI;
		public const double Double_HalfOfPI = Math.PI / 2.0;
		public const double Double_QuarterOfPI = Math.PI / 4.0;
		public const double Double_PITimes2 = Math.PI * 2.0;
		public const double Double_3QuartersOfPI = Math.PI * 3.0 / 4.0;
		public const double Double_PIDiv360 = Math.PI / 360.0;
		public const double Double_PIDiv16 = Math.PI / 16.0;
		public const double Double_PIDiv32 = Math.PI / 32.0;
		public const double Double_PIInverseFraction = 1.0 / Math.PI;
		public const double Double_PIInverseFraction2 = 2.0 / Math.PI;
		public const double Double_PIInverseFraction5 = 5.0 / Math.PI;
		public const double Double_PITimes90 = Math.PI * 90.0;
		public const double Double_PITimes180 = Math.PI * 180.0;
		public const double Double_LooksLikePI = 3.1415926;
		public const double Double_LooksLikePI2 = 3.14159;
		public const double Double_LooksLikePI3 = 3.141;
		public const double Double_BeforePI = 3.1415926535897927;
		public const double Double_AfterPI = 3.1415926535897936;
		public const double Double_Negated_PI = -Math.PI;
		public const double Double_Negated_HalfOfPI = -Math.PI / 2.0;
		public const double Double_Negated_QuarterOfPI = -Math.PI / 4.0;
		public const double Double_Negated_PITimes2 = Math.PI * -2.0;
		public const double Double_Negated_3QuartersOfPI = Math.PI * -3.0 / 4.0;
		public const double Double_Negated_PIDiv360 = -Math.PI / 360.0;
		public const double Double_Negated_PIDiv16 = -Math.PI / 16.0;
		public const double Double_Negated_PIDiv32 = -Math.PI / 32.0;
		public const double Double_Negated_PIInverseFraction = -1.0 / Math.PI;
		public const double Double_Negated_PIInverseFraction2 = -2.0 / Math.PI;
		public const double Double_Negated_PIInverseFraction5 = -5.0 / Math.PI;
		public const double Double_Negated_PITimes90 = Math.PI * -90.0;
		public const double Double_Negated_PITimes180 = Math.PI * -180.0;
		public const double Double_Negated_LooksLikePI = -3.141;
		public const double Double_Negated_BeforePI = -3.1415926535897927;
		public const double Double_Negated_AfterPI = -3.1415926535897936;

		public const double Double_E = Math.E;
		public const double Double_BeforeE = 2.7182818284590446;
		public const double Double_AfterE = 2.7182818284590455;
		public const double Double_Negated_E = -Math.E;
		public const double Double_Negated_BeforeE = -2.7182818284590446;
		public const double Double_Negated_AfterE = -2.7182818284590455;
	}
}
