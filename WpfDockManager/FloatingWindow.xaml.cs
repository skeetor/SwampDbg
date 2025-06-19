using System.Windows;
using System.Windows.Input;

namespace WpfDockingManager
{
	/// <summary>
	/// Interaction logic for FloatingWindow.xaml
	/// </summary>
	public partial class FloatingWindow : Window, IDockingProvider
	{
		public DockingPanel RootDockPanel { get; private set; }

		public FloatingWindow()
		{
			InitializeComponent();

			var panel = new DockingPanel();
			RootDockPanel = panel;

			DockingRootGrid.Children.Add(panel);
		}

		public IDockingPanel GetDockingPanel()
		{
			return RootDockPanel;
		}

		private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
