using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDockManager.Layout;

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
		private Grid _rootChild;

		// When the class is instantiated we have to remember all items added to it
		// so we can create the layout when all items are fully loaded. Properties
		// are added lazily, so we have to wait until an element has finished getting
		// all properties.
		private LayoutItemList? LayoutItems { get; set; }

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
							new PropertyChangedCallback(OnDockChanged)),
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
		#region DockTarget property
		public static readonly DependencyProperty DockTargetProperty =
				DependencyProperty.RegisterAttached(
						"DockTarget",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							""
							)
						// We cant validate the targt name here, so we have to do this after the control is loaded.
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
		#region DockName property
		public static readonly DependencyProperty DockNameProperty =
				DependencyProperty.RegisterAttached(
						"DockName",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							""
							)
					);
		public static string GetDockName(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(DockNameProperty);
		}

		public static void SetDockName(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(DockNameProperty, value);
		}
		#endregion DockName property

		public DockingPanel()
			: base()
		{
			LayoutItems = new LayoutItemList();
			Loaded += OnLoadedEvent;

			_rootChild = CreateDefaultGrid();
			Children.Add(_rootChild);
		}

		private void OnLoadedEvent(object? sender, EventArgs e)
		{
			if (LayoutItems == null)
				return;

			foreach (var item in LayoutItems!)
				InitLayout(item.Object as UIElement);

			LayoutItems = null;
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
			_rootChild.Measure(availableSize);

			return _rootChild.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_rootChild.Arrange(new Rect(new Point(0, 0), finalSize));

			return finalSize;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (visualAdded == _rootChild)
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
					if (LayoutItems != null)
					{
						LayoutItems += visualAdded;
						return;
					}
					else
						throw new InvalidEnumArgumentException("DockType.None is an invalid argument after DockPanel is loaded.");
				}
				//DockElement(child);
			}

			child = visualRemoved as UIElement;
			if (child != null)
			{
				if (LayoutItems != null)
				{
					LayoutItems -= visualRemoved;
					return;
				}
				//UndockElement(child);
			}

			//if (visualAdded is UIElement elementAdded)
			//{
			//	DockType dock = GetDock(elementAdded);

			//	switch (dock)
			//	{
			//		case DockType.Center:
			//		{
			//			RemoveElementFromItsParent(elementAdded as FrameworkElement);

			//			string? header = GetDockTitle(elementAdded);

			//			TabControl tc = new TabControl();
			//			TabItem tcItem = new TabItem();
			//			tcItem.Header = "DockingTabControlItem";
			//			tcItem.Content = elementAdded;
			//			tc.Items.Add(tcItem);

			//			_rootChild.Children.Add(tc);

			//			Grid.SetRow(tc, 0);
			//			Grid.SetColumn(tc, 0);
			//		}
			//		break;
			//	}

			//	InvalidateMeasure();
			//}
		}

		#region Un-/Docking of elements
		protected UIElement? FindTarget(string targetName)
		{
			if (targetName.Length == 0)
				return null;

			foreach (var item in LayoutItems!)
			{
				var element = item.Object as UIElement;
				var nm = GetDockName(element!);
				if (nm != null && nm == targetName)
					return element;
			}

			return null;
		}
		protected void InitLayout(UIElement? element)
		{
			// LayoutItems exists only during initialization and this function should not be called once
			// everything is set up. Use the DockElement instead.
			if (LayoutItems == null)
				throw new InvalidOperationException("LayoutItems is null");

			if (element == null)
				return;

			UIElement? target = null;

			var targetName = GetDockTarget(element);
			if (targetName != null && targetName.Length > 0)
			{
				target = FindTarget(targetName);
				if (target == null)
					throw new InvalidOperationException("Target '" + targetName + "' must be defined.");
			}

			var dock = GetDock(element);

			element.ClearValue(DockProperty);
			element.ClearValue(DockTargetProperty);

			DockElement(element, dock, target);
		}
		protected void DockElement(UIElement element, DockType dock, UIElement? target = null)
		{
			if (element == target)
				throw new InvalidOperationException("Can not dock an element on itself!");

			InvalidateMeasure();
		}

		protected void UndockElement(UIElement? element)
		{
			if (element == null)
				return;

			InvalidateMeasure();
		}
		#endregion Un-/Docking of elements

		public static void RemoveElementFromItsParent(FrameworkElement? el)
		{
			if (el == null)
				return;

			if (el.Parent == null)
				return;

			var panel = el.Parent as Panel;
			if (panel != null)
			{
				panel.Children.Remove(el);
				return;
			}

			var decorator = el.Parent as Decorator;
			if (decorator != null)
			{
				decorator.Child = null;
				return;
			}

			var contentPresenter = el.Parent as ContentPresenter;
			if (contentPresenter != null)
			{
				contentPresenter.Content = null;
				return;
			}

			var contentControl = el.Parent as ContentControl;
			if (contentControl != null)
				contentControl.Content = null;
		}

		private void ReplaceChild(UIElement oldChild, UIElement newChild)
		{
		//	//var dockType = GetDock(oldChild);
		//	//dockType = GetDock(newChild);
		//	//dockType = GetDock(this);

		//	// Disconnect from parent first, before we can add it to the grid.
		//	RemoveElementFromItsParent(oldChild as FrameworkElement);
		//	//RemoveElementFromItsParent(newChild as FrameworkElement);
		//	//parent.RemoveLogicalChild(oldChild);
		//	//var parent = VisualTreeHelper.GetParent(oldChild);

		//	// Create a container (e.g., a Grid) to hold both old and new children
		//	//TabControl tabCtrl = 
		//	//Grid container = new Grid();
		//	//container.Children.Add(oldChild);
		//	//container.Children.Add(newChild);
		//	UIElement container = newChild;

		//	// Replace the old child with the container in the visual tree
		//	int index = InternalChildren.IndexOf(oldChild);
		//	if (index >= 0)
		//	{
		//		InternalChildren.RemoveAt(index);
		//		InternalChildren.Insert(index, container);

		//		// Update the corresponding child reference
		//		if (oldChild == _topChild)
		//			_topChild = container;

		//		if (oldChild == _bottomChild)
		//			_bottomChild = container;

		//		if (oldChild == _leftChild)
		//			_leftChild = container;

		//		if (oldChild == _rightChild)
		//			_rightChild = container;
		//	}
		}
	}
}
