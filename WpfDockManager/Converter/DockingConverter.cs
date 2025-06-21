using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfDockingManager
{
	public class TabPositionDockConverter : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (TabPosition)inputValue;

			var dock = Dock.Top;
			switch (value)
			{
				case TabPosition.Top:
				dock = Dock.Top;
				break;

				case TabPosition.Bottom:
				dock = Dock.Bottom;
				break;

				case TabPosition.Left:
				dock = Dock.Left;
				break;

				case TabPosition.Right:
				dock = Dock.Right;
				break;
			}

			return dock;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (Dock)inputValue;

			var tabPosition = TabPosition.Top;
			switch (value)
			{
				case Dock.Top:
				tabPosition = TabPosition.Top;
				break;

				case Dock.Bottom:
				tabPosition = TabPosition.Bottom;
				break;

				case Dock.Left:
				tabPosition = TabPosition.Left;
				break;

				case Dock.Right:
				tabPosition = TabPosition.Right;
				break;
			}

			return tabPosition;
		}
	}

	public class TabItemRotation : IValueConverter
	{
		public object Convert(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			var value = (TabPosition)inputValue;

			if (value == TabPosition.Left)
				return 270;

			if (value == TabPosition.Right)
				return 90;

			return 0;
		}

		public object ConvertBack(object inputValue, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
