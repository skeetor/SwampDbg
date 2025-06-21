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

	public partial class DockingGroup : Grid
	{
		#region Properties
		public static readonly DependencyProperty TabPositionProperty =
			DependencyProperty.Register("TabPosition", typeof(TabPosition), typeof(DockingGroup),
				new PropertyMetadata(TabPosition.Top));

		public static readonly DependencyProperty TabWidthProperty =
			DependencyProperty.Register("TabWidth", typeof(double), typeof(DockingGroup),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty TabHeightProperty =
			DependencyProperty.Register("TabHeight", typeof(double), typeof(DockingGroup),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty DockAnchorProperty =
			DependencyProperty.Register("DockAnchor", typeof(string), typeof(DockingGroup),
				new PropertyMetadata(""));

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

		public string DockAnchor
		{
			get { return (string)GetValue(DockAnchorProperty); }
			set { SetValue(DockAnchorProperty, value); }
		}

		public ICommand CloseTabCommand
		{
			get { return (ICommand)GetValue(CloseTabCommandProperty); }
			set { SetValue(CloseTabCommandProperty, value); }
		}
		#endregion Properties

		//public ItemCollection Items
		//{
		//	get { return null!; }
		//}

		public DockingGroup()
		{
			InitializeComponent();
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			// If our first tabcontrol is added, we let it through. Everthing else is routed to the tabcontrol.
			if (visualAdded != null)
			{
				if (visualAdded is TabControl firstTab)
				{
					if (_tabControl == null)
					{
						base.OnVisualChildrenChanged(visualAdded, visualRemoved);
						return;
					}
				}

				DockingHelper.RemoveElementFromItsParent(visualAdded as FrameworkElement);
				var title = "Tab title goes here";
				var ti = new TabItem
				{
					Header = title,
					Content = visualAdded
				};
				_tabControl.Items.Add(ti);
			}

			// Our primary tabcontrol may not be removed.
			if (visualRemoved != null)
			{
				if (visualRemoved is TabControl someTab)
				{
					if (someTab == _tabControl)
						throw new InvalidOperationException("Primary TabControl can not be removed.");
				}
				base.OnVisualChildrenChanged(null, visualRemoved);
			}
		}

		public int Count
		{
			get { return _tabControl.Items.Count; }
		}

		public void InsertItem(object item, string title, int index = -1, bool selected = true)
		{
			UIElementCollection c = Children;

		}
	}
}
