using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfDockingManager
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
			if (value is Dock position)
			{
				return position == Dock.Bottom ? 1 : 0;
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack is not implemented for this converter.");
		}
	}

	public class TabPositionToGridRowConverterExtension : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new TabPositionToGridRowConverter();
		}
	}

	public class GridRowAdjustmentConverter : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (Dock)inputValue;

			// The close button on our DragTabControl needs to change the grid rows/columns depending on the placement.
			// In case of left or right the button position has to be adjusted. In case of Top/Bottm, the position is
			// the same.
			if (value is Dock.Top or Dock.Bottom)
				return 0;

			var param = (string)parameter;
			if (value == Dock.Left)
			{
				if (param == "content")
					return 1;

				return 0;
			}

			// Right side, the button is reversed.
			if (param == "content")
				return 0;

			return 1;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class TabItemRotation : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (Dock)inputValue;

			if (value == Dock.Left)
				return 270;

			if (value == Dock.Right)
				return 90;

			return 0;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class DockOrientationConverter : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (Dock)inputValue;

			// The close button on our DragTabControl needs to change the grid rows/columns depending on the placement.
			// In case of left or right the button position has to be adjusted. In case of Top/Bottm, the position is
			// the same.
			if (value is Dock.Top or Dock.Bottom)
				return Orientation.Horizontal;

			return Orientation.Vertical;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class NotEqualConverter : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (Dock)inputValue;
			string param = (string)parameter;

			if (value.ToString() == param)
				return false;

			return true;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
