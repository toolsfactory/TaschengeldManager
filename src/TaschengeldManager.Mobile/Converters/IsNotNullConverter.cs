using System.Globalization;

namespace TaschengeldManager.Mobile.Converters;

/// <summary>
/// Converts a value to boolean - true if not null, false if null
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
