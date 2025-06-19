using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockingManager
{
	public enum TabPosition
	{
		Top,
		Bottom,
		Left,
		Right
	}

	public partial class DockingGroup : UserControl
	{
		public static readonly DependencyProperty TabPositionProperty =
			DependencyProperty.Register("TabPosition", typeof(TabPosition), typeof(DockingGroup),
				new PropertyMetadata(TabPosition.Top));

		public static readonly DependencyProperty TabWidthProperty =
			DependencyProperty.Register("TabWidth", typeof(double), typeof(DockingGroup),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty TabHeightProperty =
			DependencyProperty.Register("TabHeight", typeof(double), typeof(DockingGroup),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty CloseTabCommandProperty =
			DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DockingGroup));

		public TabPosition TabPosition
		{
			get { return (TabPosition)GetValue(TabPositionProperty); }
			set { SetValue(TabPositionProperty, value); }
		}

		public double TabWidth
		{
			get { return (double)GetValue(TabWidthProperty); }
			set { SetValue(TabWidthProperty, value); }
		}

		public double TabHeight
		{
			get { return (double)GetValue(TabHeightProperty); }
			set { SetValue(TabHeightProperty, value); }
		}

		public ICommand CloseTabCommand
		{
			get { return (ICommand)GetValue(CloseTabCommandProperty); }
			set { SetValue(CloseTabCommandProperty, value); }
		}

		public DockingGroup()
		{
			InitializeComponent();
		}
	}
}
