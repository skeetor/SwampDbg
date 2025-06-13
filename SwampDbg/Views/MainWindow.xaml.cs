using SwampDbg.Configuration;
using System.Windows;
using System.Windows.Controls;
using WpfDockManager;

namespace SwampDbg
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var generalConfig = App.GetConfig<GeneralConfig>() ?? new GeneralConfig();
			UpdateFromConfig(generalConfig);
		}

		public DockingPanel GetDockingPanel()
		{
			return RootDockPanel;
		}

		private void UpdateFromConfig(GeneralConfig config)
		{
			// If set to null the default is used.
			if (config.MenuPosition != null)
			{
				var menu = mnMainMenu;
				var pos = Dock.Top;
				if (config.MenuPosition == "Bottom")
					pos = Dock.Bottom;

				DockPanel.SetDock(menu, pos);
			}
		}

		private void OnOpenFile(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnOpenFile");
		}

		private void OnOpenProject(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("OnOpenProject");
		}
	}
}
