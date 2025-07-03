using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfDockingManager
{
	public partial class DockingGroup : Grid
	{
		private static int ItemIdCounter = 0;
		private int ActualItemId = ItemIdCounter++;

		#region Properties
		public static readonly DependencyProperty TabPositionProperty =
			DependencyProperty.Register("TabPosition", typeof(Dock), typeof(DockingGroup),
				new PropertyMetadata(Dock.Top));

		public static readonly DependencyProperty TabHeightProperty =
			DependencyProperty.Register("TabHeight", typeof(double), typeof(DockingGroup),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty CloseTabCommandProperty =
			DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DockingGroup));

		public static readonly DependencyProperty DockAnchorProperty = DockingPanel.DockAnchorProperty;
		public static readonly DependencyProperty DockPositionProperty = DockingPanel.DockPositionProperty;
		public static readonly DependencyProperty DockTargetProperty = DockingPanel.DockTargetProperty;
		public static readonly DependencyProperty DockLengthProperty = DockingPanel.DockLengthProperty;
		public static readonly DependencyProperty DockTitleProperty = DockingPanel.DockTitleProperty;

		public Dock TabPosition
		{
			get { return (Dock)GetValue(TabPositionProperty); }
			set { SetValue(TabPositionProperty, value); }
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

		public string DockAnchor
		{
			get { return DockingPanel.GetDockAnchor(this); }
			set { DockingPanel.SetDockAnchor(this, value); }
		}

		public DockingPosition DockPosition
		{
			get { return DockingPanel.GetDockPosition(this); }
			set { DockingPanel.SetDockPosition(this, value); }
		}

		public string DockTarget
		{
			get { return DockingPanel.GetDockTarget(this); }
			set { DockingPanel.SetDockTarget(this, value); }
		}

		public GridLength DockLength
		{
			get { return DockingPanel.GetDockLength(this); }
			set { DockingPanel.SetDockLength(this, value); }
		}

		public string DockTitle
		{
			get { return DockingPanel.GetDockTitle(this); }
			set { DockingPanel.SetDockTitle(this, value); }
		}
		#endregion Properties

		public DockingGroup()
		{
			InitializeComponent();
		}

		public event DragTabEventHandler ItemCloseEventHandlers
		{
			add { _tabControl.ItemCloseEventHandlers += value; }
			remove { _tabControl.ItemCloseEventHandlers -= value; }
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

		public void InsertItem(UIElement element, int index = -1, bool selected = true)
		{
			var title = DockingPanel.GetDockTitle(element);
			var ti = new TabItem
			{
				Header = title,
				Content = element
			};

			if (index == -1)
				index = _tabControl.Items.Count;
			_tabControl.Items.Insert(index, ti);
		}

		private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
		{
			var button = sender as DependencyObject;
			if (button == null)
				throw new InvalidOperationException("Unknown sender type");

			TabControl? tabControl = null;
			TabItem? tabItem = null;
			DockingGroup? group = FindContainers(button, out tabControl, out tabItem);

			if (group != null)
				OnCloseButton(group, tabControl, tabItem);
		}

		private DockingGroup? FindContainers(DependencyObject element, out TabControl? tabControl, out TabItem? tabItem)
		{
			tabControl = null;
			tabItem = null;

			while (element != null)
			{
				element = VisualTreeHelper.GetParent(element);

				if (element is TabItem ti)
				{
					tabItem ??= ti;
					continue;
				}

				if (element is TabControl tc)
				{
					tabControl ??= tc;
					continue;
				}

				if (element is DockingGroup gr)
					return gr;
			}

			return null;
		}

		public virtual void OnCloseButton(DockingGroup group, TabControl? tabControl, TabItem? tabItem)
		{
		}
	}
}
