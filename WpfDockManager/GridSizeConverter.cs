using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfDockManager
{
	public class GridSizeConverter : IValueConverter
	{
		public static GridSizeConverter Instance { get; } = new();

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return new GridLength((double)value!, GridUnitType.Pixel);
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return ((GridLength)value!).Value;
		}
	}
}
