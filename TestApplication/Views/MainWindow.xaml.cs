using TestApplication.Configuration;
using System.Windows;
using System.Windows.Controls;
using TestApplication.Controls;
using System.Windows.Automation;
using WpfDockingManager;

namespace TestApplication
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDockingProvider
	{
		public MainWindow()
		{
			InitializeComponent();
			var generalConfig = App.GetConfig<GeneralConfig>() ?? new GeneralConfig();
			UpdateFromConfig(generalConfig);
		}

		public IDockingPanel GetDockingPanel()
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

		private void OnAddRootDocking(object sender, RoutedEventArgs e)
		{
			var targetName = _cmbRootDirection.SelectedValue.ToString()!;
			var dockPanel = GetDockingPanel();

			if (targetName == "Floating")
			{
				dockPanel.DockingFloat(TestControl.CreateInstance(), DockPosition.None);
				return;
			}
		
			var dock = TestControl.TypeNames[targetName];

			var element = TestControl.CreateInstance();
			dockPanel.DockElement(element, dock, target: null);
		}
	}
}
