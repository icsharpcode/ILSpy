using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace EleCho.WpfSuite
{
    internal static class MathHelper
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016;

        public static bool AreClose(double value1, double value2) =>
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            value1 == value2 || IsVerySmall(value1 - value2);

        public static double Lerp(double x, double y, double alpha) => x * (1.0 - alpha) + y * alpha;

        public static bool IsVerySmall(double value) => Math.Abs(value) < 1E-06;

        public static bool IsZero(double value) => Math.Abs(value) < 10.0 * DBL_EPSILON;

        public static bool IsFiniteDouble(double x) => !double.IsInfinity(x) && !double.IsNaN(x);

        public static double DoubleFromMantissaAndExponent(double x, int exp) => x * Math.Pow(2.0, exp);

        public static bool GreaterThan(double value1, double value2) => value1 > value2 && !AreClose(value1, value2);

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static double Hypotenuse(double x, double y) => Math.Sqrt(x * x + y * y);

        public static bool LessThan(double value1, double value2) => value1 < value2 && !AreClose(value1, value2);

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static double EnsureRange(double value, double? min, double? max)
        {
            if (min.HasValue && value < min.Value)
            {
                return min.Value;
            }
            if (max.HasValue && value > max.Value)
            {
                return max.Value;
            }
            return value;
        }

        public static double SafeDivide(double lhs, double rhs, double fallback)
        {
            if (!IsVerySmall(rhs))
            {
                return lhs / rhs;
            }
            return fallback;
        }
    }
}
