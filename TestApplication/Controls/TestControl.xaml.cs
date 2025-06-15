using System.Windows;
using System.Windows.Controls;
using WpfDockManager;

namespace TestApplication.Controls
{
	/// <summary>
	/// Interaction logic for TestControl.xaml
	/// </summary>
	public partial class TestControl : UserControl
	{
		private static int InstanceCounter = 0;

		public static Dictionary<string, WpfDockManager.DockType> TypeNames =
			new Dictionary<string, WpfDockManager.DockType>()
			{
				[nameof(WpfDockManager.DockType.Top)] = WpfDockManager.DockType.Top,
				[nameof(WpfDockManager.DockType.Bottom)] = WpfDockManager.DockType.Bottom,
				[nameof(WpfDockManager.DockType.Left)] = WpfDockManager.DockType.Left,
				[nameof(WpfDockManager.DockType.Right)] = WpfDockManager.DockType.Right,
				["Center"] = WpfDockManager.DockType.None
			};

		public TestControl()
		{
			InitializeComponent();

			InstanceCounter++;
		}

		private DockingPanel GetDockingPanel()
		{
			var mainWindow = (App.Current.MainWindow as MainWindow)!;
			return mainWindow.GetDockingPanel();
		}

		public void SetText(string text)
		{
			_TextBoxCtrl.Text = text;
		}

		private void OnAddItem(object sender, RoutedEventArgs e)
		{
			HandleDocking(true);
		}

		private void OnRemoveItem(object sender, RoutedEventArgs e)
		{
			HandleDocking(false);
		}

		public static TestControl CreateInstance()
		{
			var element = new TestControl();
			element.SetText("Instance: " + InstanceCounter.ToString());
			DockingPanel.SetDockTitle(element, "Instance: "+InstanceCounter.ToString());

			return element;
		}

		private void HandleDocking(bool add)
		{
			var targetName = _ComboBoxCtrl.SelectedValue.ToString()!;
			var dockPanel = GetDockingPanel();

			if (targetName == "Floating")
			{
				var element = TestControl.CreateInstance();
				dockPanel.DockingFloat(element, DockType.None);
				return;
			}

			var dock = TypeNames[targetName];

			if (add)
			{
				FrameworkElement? target = null;
				DockingPanel.FindAssociatedContainers(this, out target);

				var element = TestControl.CreateInstance();
				dockPanel.DockElement(element, dock, target);
			}
			else
				dockPanel.UndockElement(this);
		}
	}
}
