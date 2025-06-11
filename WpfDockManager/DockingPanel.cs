using System;
using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
		// When the class is instantiated we have to remember all items added to it
		// so we can create the layout when all items are fully loaded. Properties
		// are added lazily, so we have to wait until an element has finished getting
		// all properties.
		private LayoutItemList? LayoutItems { get; set; }

		private Dictionary<string, TabControl> DockGroups = new Dictionary<string, TabControl>();
		private DockingSplitter _root = new();

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
			//var parent = DockingHelper.FindParentClass<DockingPanel>(child);
			//if (parent == null)
			//	return;

			//var newGroup = (e.NewValue as string)!;
			//var tabCtrl = parent.FindAnchor(newGroup);

			//// If we already have a tab for this group we don't need to do anything.
			//if (tabCtrl == null)
			//{
			//	if (newGroup.Length > 0)
			//	{
			//		tabCtrl = new TabControl();
			//		var tabGroup = newGroup + "Anchor";

			//		parent.DockGroups[tabGroup] = tabCtrl;
			//		SetDockAnchor(tabCtrl, tabGroup);
			//	}
			//}

			//var oldGroup = (e.OldValue as string)!;
			//if (oldGroup.Length == 0)
			//	return;

			//tabCtrl = parent.FindAnchor(oldGroup);
			//if (tabCtrl == null)
			//	return;

			//parent.DockGroups.Remove(oldGroup);
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
		#region DockLength property
		public static readonly DependencyProperty DockLengthProperty =
				DependencyProperty.RegisterAttached(
						"DockLength",
						typeof(int),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							-1
							),
						new ValidateValueCallback(IsValidDockLength)
					);

		internal static bool IsValidDockLength(object o)
		{
			// Length may be -1 for the default size of the control
			// or the value may not be 0 or less.
			int length = (int)o;
			return (length == -1 || length > 0);
		}

		public static int GetDockLength(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (int)element.GetValue(DockLengthProperty);
		}

		public static void SetDockLength(UIElement element, int value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockTitleProperty, value);
		}
		#endregion DockLength property

		public DockingPanel()
			: base()
		{
			LayoutItems = new LayoutItemList();
			Loaded += OnLoadedEvent;

			_root.Aligned = DockingSplitter.Alignment.Vertical;
			Children.Add(_root);
		}

		private void OnLoadedEvent(object? sender, EventArgs e)
		{
			if (LayoutItems == null)
				return;

			var items = LayoutItems;
			// These items are only used during initialization. Once this is done
			// We can no longer rely on the references anyway, as they might have been
			// destroyed or moved, so we discard them.
			LayoutItems = null;
			InitLayout(items);
		}

		public bool IsEmpty() => _root.IsEmpty();

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
			_root.Measure(availableSize);

			return _root.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_root.Arrange(new Rect(new Point(0, 0), finalSize));

			return finalSize;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (LayoutItems == null || visualAdded == _root)
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

				var nm = GetDockTarget(element);
				TabControl? target = FindAnchor(nm);

				var dock = GetDock(element);
				var index = GetDockIndex(element);

				DockElement(element, dock, target, index, batchDock: true);

				// TODO: We don't really need those properties, once the item is docked,so does it make sense to remove them, or should we keep them?
				//ResetProperties(element);
			}

			// We have to set the lengths of the items, after the layout has been estalished.
			// This can not be done directly, because the splitters can not be moved appropriately
			// and when new elements are added to the splitter, it would get disrupted.
			//int[] cols = { 100, 500, 20 };
			//for (int i = 0; i < cols.Length; i++)
			//	_root.SetLength(i * 2, cols[i]);
			_root.DumpGrid();

			InvalidateMeasure();
		}

		/// <summary>
		/// Wrap a GUI item in a tabcontrol. If the tabcontrol doesn't exist it will be created.
		/// If index is not specified, it will be appended at the end.
		/// </summary>
		/// <param name="element"></param>
		/// <returns>The specified tabctrl or a new one.</returns>
		protected TabControl CreateTabElement(FrameworkElement element, TabControl? tabControl = null, int index = -1)
		{
			DockingHelper.RemoveElementFromItsParent(element);

			if (tabControl == null)
			{
				tabControl = new TabControl();
				var nm = GetDockAnchor(element);
				if (nm.Length > 0)
				{
					DockGroups[nm] = tabControl;
					SetDockAnchor(element, nm+"Anchor");
				}
			}

			var title = GetDockTitle(element);
			if (title == null)
				title = "";

			var ti = new TabItem
			{
				Header = title,
				Content = element
			};

			if (index == -1)
				index = tabControl.Items.Count;
			tabControl.Items.Insert(index, ti);

			return tabControl;
		}

		public void DockElement(UIElement element, DockType dock, UIElement? target = null, int index = -1, bool batchDock = false)
		{
			if (element == target)
				throw new ArgumentException("Can not dock an element on itself!");

			var item = element as FrameworkElement;
			if (item == null)
				throw new ArgumentException("Item is not a FrameworkItem");

			if (IsEmpty())
				dock = DockType.None;

			switch (dock)
			{
				case DockType.None:
				{
					// If an element should be added to the center we need a target to add the item to.
					// If the panel is empty, we can create a new tab automatically.
					if (target == null)
					{
						if (!IsEmpty())
							target = _root.GetChild(0);

						//target = _rootGrid.GetChild(0) as TabControl;
						//if (target == null && !IsEmpty())
						//	throw new InvalidOperationException("Unable to find a default TabControl as target");
					}

					var tabControl = CreateTabElement(item, tabControl: target as TabControl, index: index);
					if (IsEmpty())
						_root.Add(tabControl);

					UpdateLength(item, VisualTreeHelper.GetParent(tabControl) as DockingSplitter, tabControl);
				}
				break;

				// Dock to left of target
				case DockType.Left:
				{
					var tabCtrl = target as TabControl;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a TabControl");

					VerticalSplit(item, true, target as TabControl);
				}
				break;

				// Dock to right of target
				case DockType.Right:
				{
					var tabCtrl = target as TabControl;

					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a TabControl");

					VerticalSplit(item, false, target as TabControl);
				}
				break;

				case DockType.Top:
				{
					var tabCtrl = target as TabControl;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a TabControl");

					HorizontalSplit(item, true, target as TabControl);
				}
				break;

				case DockType.Bottom:
				{
					var tabCtrl = target as TabControl;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a TabControl");

					HorizontalSplit(item, false, target as TabControl);
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

		protected void UpdateLength(UIElement element, DockingSplitter? parent, TabControl tabControl)
		{
			if (parent == null)
				return;

			var length = GetDockLength(element);
			if (length == -1)
				return;

			var pos = parent.GetIndex(tabControl);
			parent.SetLength(pos, length);
		}

		protected void HorizontalSplit(FrameworkElement element, bool before, TabControl? target = null) => Split(element, before, DockingSplitter.Alignment.Horizontal, target);

		protected void VerticalSplit(FrameworkElement element, bool before, TabControl? target = null) => Split(element, before, DockingSplitter.Alignment.Vertical, target);

		protected void Split(FrameworkElement element, bool before, DockingSplitter.Alignment axis, TabControl? target = null)
		{
			DockingSplitter? parent;

			if (target == null)
				parent = _root;
			else
				parent = VisualTreeHelper.GetParent(target) as DockingSplitter;

			if (parent == null)
				throw new InvalidOperationException("DockingGrid for target '" + GetDockTarget(target!) + "' not found!");

			var index = parent.GetIndex(target);
			if (index == -1 && before)
				index = 0;
			else if (!before && target != null)
				index++;

			// TODO: We have to create a new splitter in this case.
			if (parent.Aligned != axis)
				throw new InvalidOperationException("DockingGrid is not of the same alignment.");

			var tabControl = CreateTabElement(element, tabControl: null);
			parent.Insert(tabControl, index);

			UpdateLength(element, parent, tabControl);
		}

		protected void UndockElement(UIElement? element)
		{
			if (element == null)
				return;

			InvalidateMeasure();
		}
	}
}
