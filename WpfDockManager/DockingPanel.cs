using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using LayoutItemList = WpfDockingManager.UniqueList<WpfDockingManager.Layout.LayoutItem, System.Windows.DependencyObject>;

namespace WpfDockingManager
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockingManagerNS="clr-namespace:WpfDockingManager"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockingManagerNS="clr-namespace:WpfDockingManager;assembly=WpfDockingManager"
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
	///     <WpfDockingManagerNS:DockingPanel/>
	///
	/// </summary>

	public class DockingPanel : Panel, IDockingPanel
	{
		// When the class is instantiated we have to remember all items added to it
		// so we can create the layout when all items are fully loaded. Properties
		// are added lazily, so we have to wait until an element has finished getting
		// all properties.
		private LayoutItemList? LayoutItems { get; set; }

		public static Dictionary<string, UIElement> DockingAnchors { get; } = new();
		private DockingSplitter RootSplitter = new();

		#region DockPosition property
		//public static readonly DependencyProperty DockProperty =
		//	DockPanel.DockProperty.AddOwner(typeof(DockingPanel));
		//[CommonDependencyProperty]
		public static readonly DependencyProperty DockPositionProperty =
				DependencyProperty.RegisterAttached(
						"DockPosition",
						typeof(DockingPosition),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							DockingPosition.None,
							new PropertyChangedCallback(OnDockChanged)
						),
						new ValidateValueCallback(IsValidDock)
				);
		internal static bool IsValidDock(object o)
		{
			DockingPosition dock = (DockingPosition)o;

			return dock == DockingPosition.None
					|| dock == DockingPosition.Left
					|| dock == DockingPosition.Top
					|| dock == DockingPosition.Right
					|| dock == DockingPosition.Bottom
					|| dock == DockingPosition.Floating
					;
		}
		private static void OnDockChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			//UIElement? child = depObj as UIElement;
			//if (child == null)
			//	return;

			//DockingPosition dock = (DockingPosition)e.OldValue;
			//if ((DockingPosition)e.OldValue == DockingPosition.None && (DockingPosition)e.NewValue != DockingPosition.None)
			//{
			//	DockingPanel? p = VisualTreeHelper.GetParent(child) as DockingPanel;
			//	if (p == null)
			//		return;

			//	p.Refresh(child);
			//}
		}

		public static DockingPosition GetDockPosition(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (DockingPosition)element.GetValue(DockPositionProperty);
		}

		public static void SetDockPosition(UIElement element, DockingPosition value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockPositionProperty, value);
		}
		#endregion DockPosition property
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
		private static void OnDockAnchorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			UpdateDockAnchor((depObj as UIElement)!, (string)e.NewValue);
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
							"",
							new PropertyChangedCallback(OnDockTargetChanged)
						)
					);
		private static void OnDockTargetChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			string value = (string)e.NewValue;
			if (value.Length > 0 && !DockingAnchors.ContainsKey(value))
				throw new InvalidOperationException("DockTarget '" + value + "' not defined");
		}
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
						typeof(GridLength),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							new GridLength(-1, GridUnitType.Pixel)
							)
					);

		public static GridLength GetDockLength(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (GridLength)element.GetValue(DockLengthProperty);
		}

		public static void SetDockLength(UIElement element, GridLength value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockLengthProperty, value);
		}
		#endregion DockLength property
		#region DockFloating property
		public static readonly DependencyProperty DockFloatingProperty =
				DependencyProperty.RegisterAttached(
						"DockFloating",
						typeof(Alignment),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							(Alignment)0		// There is no null value so we have to use this instead.
							),
						new ValidateValueCallback(IsValidDockFloating)
					);

		internal static bool IsValidDockFloating(object o)
		{
			Alignment alignment = (Alignment)o;
			return alignment is 0 or Alignment.Vertical or Alignment.Horizontal;
		}

		public static Alignment GetDockFloating(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (Alignment)element.GetValue(DockFloatingProperty);
		}

		public static void SetDockFloating(UIElement element, Alignment value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockFloatingProperty, value);
		}
		#endregion DockFloating property
		#region FloatingRectangle property
		public static readonly DependencyProperty FloatingRectangleProperty =
				DependencyProperty.RegisterAttached(
						"FloatingRectangle",
						typeof(Rect),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							default(Rect)
							)
					);

		public static Rect GetFloatingRectangle(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (Rect)element.GetValue(FloatingRectangleProperty);
		}

		public static void SetFloatingRectangle(UIElement element, Rect value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(FloatingRectangleProperty, value);
		}
		#endregion FloatingRectangle property

		public DockingPanel()
			: base()
		{
			RootSplitter.Aligned = Alignment.Vertical;
			Children.Add(RootSplitter);
		}

		public bool IsEmpty() => RootSplitter.IsEmpty();

		public static void UpdateDockAnchor(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);

			if (DockingAnchors.ContainsKey(value))
			{
				var el = DockingAnchors[value];
				if (el != element)
					throw new InvalidOperationException("DockingAnchor '" + value + "' already used.");
			}

			UIElement? existingElement = null;
			string? existingKey = null;

			foreach (KeyValuePair<string, UIElement> entry in DockingAnchors)
			{
				if (entry.Value == element)
				{
					existingElement = entry.Value;
					existingKey = entry.Key;
					break;
				}
			}

			if (existingElement == null)
			{
				if (!value.Equals(""))
					DockingAnchors.Add(value, element);

				return;
			}

			// If the new value is empty, we remove the entry from the grouplist, otherwise
			// its updated and the previous entry removed.
			DockingAnchors.Remove(existingKey!);
			if (!value.Equals(""))
				DockingAnchors.Add(value, element);
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			bool valid = false;

			if (visualAdded is DockingSplitter)
			{
				valid = true;
			}

			if (visualAdded is DockingGroup)
			{
				valid = true;
				AddItem((visualAdded as UIElement)!);
			}

			if (!valid && visualAdded != null)
				throw new InvalidOperationException("DockingPanel only accepts DockingGroup or DockingSplitter as a child object.");

			if (visualRemoved is DockingSplitter)
			{
			}
			else if (visualRemoved is DockingGroup)
				UndockElement(visualRemoved as UIElement);

			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		protected void AddItem(UIElement element)
		{
			if (element == null)
				ArgumentNullException.ThrowIfNull(element);

			var nm = GetDockTarget(element);
			var target = FindAnchor(nm);
			if (target == null && nm.Length > 0)
				throw new InvalidOperationException("Target '" + nm + "' not defined");

			var dock = GetDockPosition(element);
			var index = GetDockIndex(element);

			var floating = GetDockFloating(element);
			if (floating is Alignment.Vertical or Alignment.Horizontal)
			{
				var rect = GetFloatingRectangle(element);
				DockingFloat(element, dock, target, index, true, rect);
			}
			else
				DockElement(element, dock, target, index);

			// TODO: We don't really need those properties, once the item is docked,so does it make sense to remove them, or should we keep them?
			//ResetProperties(element);
		}

		protected void RemoveItem(UIElement element) => UndockElement(element);

		protected override Size MeasureOverride(Size availableSize)
		{
			RootSplitter.Measure(availableSize);

			return RootSplitter.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			RootSplitter.Arrange(new Rect(new Point(0, 0), finalSize));

			return finalSize;
		}

		private static void ResetProperties(UIElement? element)
		{
			if (element == null)
				return;

			element.ClearValue(DockPositionProperty);
			element.ClearValue(DockAnchorProperty);
			element.ClearValue(DockIndexProperty);
		}

		private UIElement? FindAnchor(string? group)
		{
			if (group == null || group.Length == 0)
				return null;

			try
			{
				return DockingAnchors[group];
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
				var target = FindAnchor(nm);

				var dock = GetDockPosition(element);
				var index = GetDockIndex(element);

				var floating = GetDockFloating(element);
				if (floating is Alignment.Vertical or Alignment.Horizontal)
				{
					var rect = GetFloatingRectangle(element);
					DockingFloat(element, dock, target, index, true, rect);
				}
				else
					DockElement(element, dock, target, index);

				// TODO: We don't really need those properties, once the item is docked,so does it make sense to remove them, or should we keep them?
				//ResetProperties(element);
			}

			// We have to set the lengths of the items, after the layout has been estalished.
			// This can not be done directly, because the splitters can not be moved appropriately
			// and when new elements are added to the splitter, it would get disrupted.
			//int[] cols = { 100, 500, 20 };
			//for (int i = 0; i < cols.Length; i++)
			//	_root.SetLength(i * 2, cols[i]);
			//RootSplitter.DumpGrid();

			InvalidateMeasure();
		}

		/// <summary>
		/// Wrap a GUI item in a tabcontrol. If the tabcontrol doesn't exist it will be created.
		/// If index is not specified, it will be appended at the end.
		/// </summary>
		/// <param name="element"></param>
		/// <returns>The specified tabctrl or a new one.</returns>
		protected DockingGroup CreateTabElement(FrameworkElement element, DockingGroup? tabControl = null, int index = -1)
		{
			DockingHelper.RemoveElementFromItsParent(element);

			if (tabControl == null)
			{
				tabControl = new DockingGroup();
				var nm = GetDockAnchor(element);
				if (nm.Length > 0)
					DockingAnchors[nm] = tabControl;
			}

			var title = GetDockTitle(element);
			if (title == null)
				title = "";

			var ti = new TabItem
			{
				Header = title,
				Content = element
			};

			//if (index == -1)
			//	index = tabControl.Items.Count;
			//tabControl.Items.Insert(index, ti);

			tabControl.InsertItem(element, title, index);

			return tabControl;
		}

		public void DockElement(UIElement element, DockingPosition dock, UIElement? target = null, int index = -1)
		{
			if (element == target)
				throw new ArgumentException("Can not dock an element on itself!");

			var item = element as FrameworkElement;
			if (item == null)
				throw new ArgumentException("Item is not a FrameworkItem");

			if (IsEmpty())
			{
				if (dock is DockingPosition.Top or DockingPosition.Bottom)
					RootSplitter.Aligned = Alignment.Horizontal;
				else if (dock is DockingPosition.Left or DockingPosition.Right)
					RootSplitter.Aligned = Alignment.Vertical;

				dock = DockingPosition.None;
			}

			switch (dock)
			{
				case DockingPosition.None:
				{
					// If an element should be added to the center we need a target to add the item to.
					// If the panel is empty, we can create a new tab automatically.
					if (target == null)
					{
						if (!IsEmpty())
							target = RootSplitter.GetChild(0);
					}

					//var tabControl = CreateTabElement(item, tabControl: target as DockingGroup, index: index);
					var tabControl = (item as DockingGroup)!;
					DockingHelper.RemoveElementFromItsParent(tabControl);
					if (IsEmpty())
						RootSplitter.Add(tabControl);

					UpdateLength(item, VisualTreeHelper.GetParent(tabControl) as DockingSplitter, tabControl);
				}
				break;

				// Dock to left of target
				case DockingPosition.Left:
				{
					var tabCtrl = target as DockingGroup;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a DockingGroup");

					VerticalSplit(item, true, target as DockingGroup);
				}
				break;

				// Dock to right of target
				case DockingPosition.Right:
				{
					var tabCtrl = target as DockingGroup;

					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a DockingGroup");

					VerticalSplit(item, false, target as DockingGroup);
				}
				break;

				case DockingPosition.Top:
				{
					var tabCtrl = target as DockingGroup;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a DockingGroup");

					HorizontalSplit(item, true, target as DockingGroup);
				}
				break;

				case DockingPosition.Bottom:
				{
					var tabCtrl = target as DockingGroup;
					if (tabCtrl == null && target != null)
						throw new InvalidOperationException("Target is not a DockingGroup");

					HorizontalSplit(item, false, target as DockingGroup);
				}
				break;

				default:
					throw new InvalidOperationException("Undefined docking position: "+dock.ToString());
			}
		}

		protected void UpdateLength(UIElement element, DockingSplitter? parent, DockingGroup tabControl)
		{
			if (parent == null)
				return;

			var length = GetDockLength(element);

			if (length.Value != -1)
			{
				var pos = parent.GetIndex(tabControl);
				parent.SetLength(pos, length);
			}
		}

		protected void HorizontalSplit(FrameworkElement element, bool before, DockingGroup? target = null) => Split(element, before, Alignment.Horizontal, target);

		protected void VerticalSplit(FrameworkElement element, bool before, DockingGroup? target = null) => Split(element, before, Alignment.Vertical, target);

		protected void Split(FrameworkElement element, bool before, Alignment axis, DockingGroup? target = null)
		{
			DockingSplitter? parent;
			int index;

			if (target == null)
				parent = ReplaceRootSplitter(before, axis, out index);
			else
			{
				parent = VisualTreeHelper.GetParent(target) as DockingSplitter;
				if (parent == null)
					throw new InvalidOperationException("DockingGrid is not a parent for target '"+GetDockTarget(target)+"'");

				index = parent.GetIndex(target);
				if (index == -1 && before)
					index = 0;
				else if (!before && target != null)
					index++;

				if (parent.Aligned != axis)
					parent = ReplaceWithSplitter(axis, parent, target!, before, out index);
			}

			// When we are splitting, the item will always need a new DockingGroup
			var tabControl = CreateTabElement(element, tabControl: null);
			parent.Insert(tabControl, index);

			UpdateLength(element, parent, tabControl);
		}

		protected DockingSplitter ReplaceRootSplitter(bool before, Alignment axis, out int index)
		{
			index = 0;
			var parent = RootSplitter;

			if (parent.Aligned != axis)
			{
				parent = new DockingSplitter();
				parent.Aligned = axis;
				DockingHelper.RemoveElementFromItsParent(RootSplitter);
				parent.Add(RootSplitter);
				RootSplitter = parent;
				Children.Add(parent);
			}

			if (before)
				return parent;

			index = -1;
			return parent;
		}

		protected DockingSplitter ReplaceWithSplitter(Alignment axis, DockingSplitter parentSplitter, DockingGroup target, bool before, out int index)
		{
			var targetIndex = parentSplitter.GetIndex(target);
			if (targetIndex == -1)
				throw new InvalidOperationException("Target is not an element of the provided DockingSplitter");

			var newSplitter = new DockingSplitter();
			newSplitter.Aligned = axis;

			parentSplitter.Replace(target, newSplitter);
			newSplitter.Add(target);

			if (before)
				index = 0;
			else
				index = 1;

			return newSplitter;
		}

		public void UndockElement(UIElement? element)
		{
			if (element == RootSplitter)
				return;

			FrameworkElement removeElement = (element as FrameworkElement)!;
			if (removeElement == null)
				return;

			FrameworkElement? dockingChild = element as DockingGroup;
			DockingSplitter? splitter = null;

			if (dockingChild != null)
			{
				// If the element is not connected to any parent we are done.
				var p = VisualTreeHelper.GetParent(dockingChild);
				if (p == null)
					return;

				splitter = p as DockingSplitter;
			}

			if (splitter == null)
				splitter = FindAssociatedContainers(removeElement, out dockingChild);

			if (splitter == null || dockingChild == null)
				throw new InvalidOperationException("No docking parent found");

			// If the whole element should be removed we are done
			if (dockingChild != element)
			{
				bool remove = false;
				if (dockingChild is DockingGroup tab && tab.Count <= 1)
					remove = true;
				// If the last item is remove, we also remove the DockingGroup
				else if (dockingChild is DockingSplitter s&& s.IsEmpty())
					remove = true;

				// TODO: Client code should be able to veto this as it might choose to keep the FrameworkElement in place.
				if (remove)
					removeElement = dockingChild;
			}

			if (removeElement != null)
				splitter.Remove(removeElement);

			if (splitter.IsEmpty())
			{
				// TODO: Client code should be able to veto this as it might choose to keep the DockingGroup in place.
				var parent = VisualTreeHelper.GetParent(splitter);
				UndockElement(splitter);
			}
		}

		public IDockingProvider DockingFloat(UIElement element, DockingPosition dock, UIElement? target = null, int index = -1, bool show = true, Rect position = default)
		{
			var floating = new FloatingWindow();
			var dockingPanel = floating.RootDockPanel;

			dockingPanel.DockElement(element, dock, target, index);

			if (!position.Equals(default))
			{
				floating.Left = position.Left;
				floating.Top = position.Top;
				floating.Width = position.Width;
				floating.Height = position.Height;
			}

			if (show)
				floating.Show();

			return floating;
		}

		/// <summary>
		/// Find the parent DockingSplitter and the FrameworkElement which is directly associated to the splitter.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="dockingChild"></param>
		/// <returns></returns>
		public static DockingSplitter? FindAssociatedContainers(UIElement? element, out FrameworkElement? dockingChild)
		{
			dockingChild = null;
			if (element == null)
				return null;

			UIElement? parent = VisualTreeHelper.GetParent(element) as UIElement;
			if (parent == null)
				return null;

			DockingSplitter? splitter = parent as DockingSplitter;
			dockingChild = element as FrameworkElement;

			if (splitter != null)
				return splitter;

			return FindAssociatedContainers(parent, out dockingChild);
		}

		private static DockingGroup? FindParentDockingGroup(DependencyObject element)
		{
			if (element == null)
				return null;

			var parent = VisualTreeHelper.GetParent(element);
			var tabControl = parent as DockingGroup;

			if (tabControl != null)
				return tabControl;

			if (parent != null)
				return FindParentDockingGroup(parent);

			return null;
		}
	}
}
