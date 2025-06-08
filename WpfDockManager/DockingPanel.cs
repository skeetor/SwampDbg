using System.Windows;
using System.Windows.Controls;
using LayoutItemList = WpfDockManager.UniqueList<WpfDockManager.Layout.LayoutItem, System.Windows.DependencyObject>;

namespace WpfDockManager
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockManagerNS="clr-namespace:WpfDockManager"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockManagerNS="clr-namespace:WpfDockManager;assembly=WpfDockManager"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <WpfDockManagerNS:DockingPanel/>
	///
	/// </summary>

	public enum DockType
	{
		None,
		Left,
		Top,
		Right,
		Bottom,
		Floating
	}

	public class DockingPanel : Panel
	{
		private Grid _rootGrid;

		// When the class is instantiated we have to remember all items added to it
		// so we can create the layout when all items are fully loaded. Properties
		// are added lazily, so we have to wait until an element has finished getting
		// all properties.
		private LayoutItemList? LayoutItems { get; set; }
		private Dictionary<string, TabControl> DockGroups = new Dictionary<string, TabControl>();

		#region Dock property
		//public static readonly DependencyProperty DockProperty =
		//	DockPanel.DockProperty.AddOwner(typeof(DockingPanel));
		//[CommonDependencyProperty]
		public static readonly DependencyProperty DockProperty =
				DependencyProperty.RegisterAttached(
						"Dock",
						typeof(DockType),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							DockType.None,
							new PropertyChangedCallback(OnDockChanged)
						),
						new ValidateValueCallback(IsValidDock)
				);

		internal static bool IsValidDock(object o)
		{
			DockType dock = (DockType)o;

			return (dock == DockType.None
					|| dock == DockType.Left
					|| dock == DockType.Top
					|| dock == DockType.Right
					|| dock == DockType.Bottom
					|| dock == DockType.Floating
					);
		}

		private static void OnDockChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			//UIElement? child = depObj as UIElement;
			//if (child == null)
			//	return;

			//DockType dock = (DockType)e.OldValue;
			//if ((DockType)e.OldValue == DockType.None && (DockType)e.NewValue != DockType.None)
			//{
			//	DockingPanel? p = VisualTreeHelper.GetParent(child) as DockingPanel;
			//	if (p == null)
			//		return;

			//	p.Refresh(child);
			//}
		}

		public static DockType GetDock(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (DockType)element.GetValue(DockProperty);
		}

		public static void SetDock(UIElement element, DockType value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockProperty, value);
		}
		#endregion Dock property
		#region DockAnchor property
		public static readonly DependencyProperty DockAnchorProperty =
				DependencyProperty.RegisterAttached(
						"DockAnchor",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							"",
							new PropertyChangedCallback(OnDockAnchorChanged)
						)
					);

		private static void OnDockAnchorChanged(DependencyObject child, DependencyPropertyChangedEventArgs e)
		{
			var parent = DockingHelper.FindParentClass<DockingPanel>(child);
			if (parent == null)
				return;

			var newGroup = (e.NewValue as string)!;
			var tabCtrl = parent.FindAnchor(newGroup);

			// If we already have a tab for this group we don't need to do anything.
			if (tabCtrl == null)
			{
				if (newGroup.Length > 0)
				{
					tabCtrl = new TabControl();
					parent.DockGroups[newGroup] = tabCtrl;
					SetDockAnchor(tabCtrl, newGroup+"Anchor");

					var tab = parent.GetRootTabControl();
					if (tab == null)
						parent.AttachToGrid(tabCtrl);
				}
			}

			var oldGroup = (e.OldValue as string)!;
			if (oldGroup.Length == 0)
				return;

			tabCtrl = parent.FindAnchor(oldGroup);
			if (tabCtrl == null)
				return;

			parent.DockGroups.Remove(oldGroup);
		}

		public static string GetDockAnchor(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(DockAnchorProperty);
		}

		public static void SetDockAnchor(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockAnchorProperty, value);
		}
		#endregion DockAnchor property
		#region DockIndex property
		public static readonly DependencyProperty DockIndexProperty =
				DependencyProperty.RegisterAttached(
						"DockIndex",
						typeof(int),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							-1
							)
					);
		public static int GetDockIndex(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (int)element.GetValue(DockIndexProperty);
		}

		public static void SetDockIndex(UIElement element, int value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockIndexProperty, value);
		}
		#endregion DockIndex property
		#region DockTarget property
		public static readonly DependencyProperty DockTargetProperty =
				DependencyProperty.RegisterAttached(
						"DockTarget",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							""
						)
					);
		public static string GetDockTarget(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(DockTargetProperty);
		}

		public static void SetDockTarget(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockTargetProperty, value);
		}
		#endregion DockTarget property
		#region DockTitle property
		public static readonly DependencyProperty DockTitleProperty =
				DependencyProperty.RegisterAttached(
						"DockTitle",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							"Default tab title",
							new PropertyChangedCallback(OnDockTitleChanged))
					);
		private static void OnDockTitleChanged(DependencyObject child, DependencyPropertyChangedEventArgs e)
		{
			var el = child as FrameworkElement;
			if (el == null)
				return;

			var ti = el.Parent as TabItem;
			if (ti == null)
				return;

			ti.Header = e.NewValue;
		}
		public static string GetDockTitle(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(DockTitleProperty);
		}

		public static void SetDockTitle(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockTitleProperty, value);
		}
		#endregion DockTitle property

		public DockingPanel()
			: base()
		{
			LayoutItems = new LayoutItemList();
			Loaded += OnLoadedEvent;

			_rootGrid = CreateDefaultGrid();
			Children.Add(_rootGrid);
		}

		private void OnLoadedEvent(object? sender, EventArgs e)
		{
			if (LayoutItems == null)
				return;

			InitLayout(LayoutItems);

			// These items are only used during initialization. Once this is done
			// We can no longer rely on the references anyway, as they might have been
			// destroyed or moved, so we discard them.
			LayoutItems = null;
			//DockGroups = new ();
		}

		private Grid CreateDefaultGrid()
		{
			// We create a grid that always spans the full window
			var grid = new Grid();
			RowDefinition row = new RowDefinition();
			row.Height = new GridLength(1.0, GridUnitType.Star);
			grid.RowDefinitions.Add(row);

			ColumnDefinition column = new ColumnDefinition();
			column.Width = new GridLength(1.0, GridUnitType.Star);
			grid.ColumnDefinitions.Add(column);

			//grid.HorizontalAlignment = HorizontalAlignment.Left;
			//grid.VerticalAlignment = VerticalAlignment.Top;

			return grid;
		}

		private void Refresh(UIElement child)
		{
			// TODO: This is an ugly hack, because OnVisualChildrenChanged is called before the
			// attached properties are set, so we don't know where the child should be positioned.
			// It seems there is no way to enforce an update, so we remove the child and reinsert it.
			//if (InternalChildren.Count == 0)
			//	return;

			//var child = InternalChildren[0];
			InternalChildren.Remove(child as UIElement);
			InternalChildren.Add(child as UIElement);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			_rootGrid.Measure(availableSize);

			return _rootGrid.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_rootGrid.Arrange(new Rect(new Point(0, 0), finalSize));

			return finalSize;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (LayoutItems == null || visualAdded == _rootGrid)
				return;

			UIElement? child = visualAdded as UIElement;
			if (child != null)
			{
				DockType dock = GetDock(child);

				// If an item has no Docktype yet, then the new item is a new instance and we have to remember it until the
				// class is fully loaded with all properties set. If an element is added or removed during runtime, because
				// the user dragged it in a new position, then the Dock property must be specified to know where it should
				// be docked to.
				if (dock == DockType.None)
				{
					LayoutItems += visualAdded;
					return;
				}
				//DockElement(child);
			}

			child = visualRemoved as UIElement;
			if (child != null)
			{
				LayoutItems -= visualRemoved;
				return;
				//UndockElement(child);
			}
			//InvalidateMeasure();
		}

		private static void ResetProperties(UIElement? element)
		{
			if (element == null)
				return;

			element.ClearValue(DockProperty);
			element.ClearValue(DockAnchorProperty);
			element.ClearValue(DockIndexProperty);
		}

		private TabControl? FindAnchor(string? group)
		{
			if (group == null || group.Length == 0)
				return null;

			try
			{
				return DockGroups[group];
			}
			catch
			{
			}

			return null;
		}

		/// <summary>
		/// Build the layout for the specified objects in the list. The items
		/// are removed from the list, so when this function returns, the list
		/// will be empty.
		/// </summary>
		/// <param name="items"></param>
		/// <exception cref="InvalidOperationException"></exception>
		protected void InitLayout(LayoutItemList items)
		{
			// LayoutItems exists only during initialization and this function should not be called once
			// everything is set up. Use the DockElement instead.
			if (items == null)
				return;

			// When an item is docked, we have to reparent it to our own control. Because of this, we get
			// a VisualChildrenChanged event which causes the item be removed from this layoutlist as well.
			while (items.Count > 0)
			{
				var element = items.Pop(0)!.Object as UIElement;
				if (element == null)
					continue;

				var nm = GetDockAnchor(element);
				TabControl? target = FindAnchor(nm);
				if (target == null)
					target = GetRootTabControl();

				var dock = GetDock(element);
				var index = GetDockIndex(element);

				DockElement(element, dock, target, index);

				// TODO: We don't really need those properties, once the item is docked,so does it make sense to remove them, or should we keep them?
				//ResetProperties(element);
			}
		}

		protected TabControl? GetRootTabControl()
		{
			if (_rootGrid.Children.Count == 0)
				return null;

			return _rootGrid.Children[0] as TabControl;
		}

		public void AttachToGrid(UIElement element, Grid? grid = null, int row = 0, int column = 0)
		{
			if (grid == null)
				grid = _rootGrid;

			grid.Children.Add(element);
			Grid.SetRow(element, row);
			Grid.SetColumn(element, column);
		}

		/// <summary>
		/// Wrap a GUI item in a tabcontrol. If the tabcontrol doesn't exist it will be created.
		/// If index is not specified, it will be appended at the end.
		/// </summary>
		/// <param name="element"></param>
		/// <returns>The specified tabctrl or a new one.</returns>
		protected static TabControl CreateElementTab(FrameworkElement element, TabControl? tabCtrl = null, int index = -1)
		{
			DockingHelper.RemoveElementFromItsParent(element);

			if (tabCtrl == null)
				tabCtrl = new TabControl();

			var title = GetDockTitle(element);
			if (title == null)
				title = "";

			var ti = new TabItem
			{
				Header = title,
				Content = element
			};

			if (index == -1)
				index = tabCtrl.Items.Count;
			tabCtrl.Items.Insert(index, ti);

			return tabCtrl;
		}

		public void DockElement(UIElement element, DockType dock, UIElement? target = null, int index = -1, bool batchDock = false)
		{
			if (element == target)
				throw new ArgumentException("Can not dock an element on itself!");

			var item = element as FrameworkElement;
			if (item == null)
				throw new ArgumentException("Item is not a FrameworkItem");

			//var parent = DockingHelper.FindParentClass<DockingPanel>(element);
			//if (parent == null)
			//	throw new ArgumentException("Item is not connected to a DockingPanel");

			// The first item is always in the center as there are no objects we could split.
			if (_rootGrid.Children.Count == 0)
				dock = DockType.None;

			switch (dock)
			{
				case DockType.None:
				{
					var tabCtrl = CreateElementTab(item, tabCtrl: target as TabControl, index: index);

					// If an element should be added to the center we need a target to add the item to.
					// Only if the panel is empty, we can create a new tab automatically.
					if (target == null)
					{
						if (GetRootTabControl() != null)
							throw new InvalidOperationException("If no dock position is specified the element needs a target");

						AttachToGrid(tabCtrl);
					}
				}
				break;

				case DockType.Left:
				{
				}
				break;

				case DockType.Right:
				{
				}
				break;

				case DockType.Top:
				{
				}
				break;

				case DockType.Bottom:
				{
				}
				break;

				case DockType.Floating:
				{
				}
				break;

				default:
					throw new InvalidOperationException("Undefined docking position: "+dock.ToString());
			}

			if (!batchDock)
				InvalidateMeasure();
		}

		protected void UndockElement(UIElement? element)
		{
			if (element == null)
				return;

			InvalidateMeasure();
		}
	}
}
