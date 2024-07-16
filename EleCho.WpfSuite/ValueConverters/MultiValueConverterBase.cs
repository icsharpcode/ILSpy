using System;
using System.Globalization;
using System.Windows.Data;

namespace EleCho.WpfSuite
{
    /// <summary>
    /// Base class of multi value converter
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class MultiValueConverterBase<TSelf> : IMultiValueConverter
    {
        /// <inheritdoc/>
        public abstract object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture);

        /// <inheritdoc/>
        public virtual object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
