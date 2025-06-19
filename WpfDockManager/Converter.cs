using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

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

	public class TabPositionToGridRowConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TabPosition position)
			{
				return position == TabPosition.Bottom ? 1 : 0;
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack is not implemented for this converter.");
		}
	}

	// DockingGroup XAML
	public class TabPositionToGridRowConverterExtension : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new TabPositionToGridRowConverter();
		}
	}

}
