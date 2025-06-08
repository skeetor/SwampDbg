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
		Bottom
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

			return (dock == DockType.Left
					|| dock == DockType.Top
					|| dock == DockType.Right
					|| dock == DockType.Bottom
					|| dock == DockType.None
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
		#region DockGroup property
		public static readonly DependencyProperty DockGroupProperty =
				DependencyProperty.RegisterAttached(
						"DockGroup",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							"",
							new PropertyChangedCallback(OnDockGroupChanged)
						)
					);

		private static void OnDockGroupChanged(DependencyObject child, DependencyPropertyChangedEventArgs e)
		{
			var parent = DockingHelper.FindParentClass<DockingPanel>(child);
			if (parent == null)
				return;

			var newGroup = (e.NewValue as string)!;
			var tabCtrl = parent.FindGroup(newGroup);

			// If we already have a tab for this group we don't need to do anything.
			if (tabCtrl == null)
			{
				if (newGroup.Length > 0)
				{
					tabCtrl = new TabControl();
					parent.DockGroups[newGroup] = tabCtrl;
					SetDockGroup(tabCtrl, newGroup);
				}
			}

			var oldGroup = (e.OldValue as string)!;
			if (oldGroup.Length == 0)
				return;

			tabCtrl = parent.FindGroup(oldGroup);
			if (tabCtrl == null)
				return;

			parent.DockGroups.Remove(oldGroup);
		}

		public static string GetDockGroup(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(DockGroupProperty);
		}

		public static void SetDockGroup(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockGroupProperty, value);
		}
		#endregion DockGroup property
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
			DockGroups = new ();
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

		private void ResetProperties(UIElement? element)
		{
			if (element == null)
				return;

			element.ClearValue(DockProperty);
			element.ClearValue(DockGroupProperty);
			element.ClearValue(DockIndexProperty);
		}

		#region Un-/Docking of elements
		private TabControl? FindGroup(string? group)
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

				TabControl? target = FindGroup(GetDockGroup(element));
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

		/// <summary>
		/// Wrap a GUI item in a tabcontrol. If the tabcontrol doesn't exist it will be created.
		/// If index is not specified, it will be appended at the end.
		/// </summary>
		/// <param name="element"></param>
		/// <returns>The specified tabctrl or a new one.</returns>
		public TabControl CreateElementTab(FrameworkElement element, TabControl? tabCtrl = null, int index = -1, string? title = "")
		{
			DockingHelper.RemoveElementFromItsParent(element);

			if (tabCtrl == null)
				tabCtrl = new TabControl();

			if (title == null)
				title = "";

			TabItem ti = new TabItem();
			ti.Header = title;
			ti.Content = element;
			if (index == -1)
				index = tabCtrl.Items.Count;

			tabCtrl.Items.Insert(index, ti);

			return tabCtrl;
		}

		public void DockElement(UIElement element, DockType dock, UIElement? target = null, int index = -1)
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
					var title = GetDockTitle(element);

					// If we have no target for a child which should be attached to the center
					// we add it to the root.
					var tabCtrl = CreateElementTab(item, tabCtrl: target as TabControl, title: title, index: index);
					if (target == null)
					{
						_rootGrid.Children.Add(tabCtrl);
						Grid.SetRow(tabCtrl, 0);
						Grid.SetColumn(tabCtrl, 0);
						target = tabCtrl;

						//throw new InvalidOperationException("Item can not be added without a target");
					}
				}
				break;
			}

			InvalidateMeasure();
		}
		protected void UndockElement(UIElement? element)
		{
			if (element == null)
				return;

			InvalidateMeasure();
		}
		#endregion Un-/Docking of elements
	}
}
