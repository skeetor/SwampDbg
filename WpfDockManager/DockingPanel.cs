using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
		Center
	}

	public class DockingPanel : Panel
	{
		private Grid _rootChild;

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
					|| dock == DockType.Center
					|| dock == DockType.None
					);
		}
		private static void OnDockChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			UIElement? child = depObj as UIElement;
			if (child == null)
				return;

			DockType dock = (DockType)e.OldValue;
			if ((DockType)e.OldValue == DockType.None && (DockType)e.NewValue != DockType.None)
			{
				DockingPanel? p = VisualTreeHelper.GetParent(child) as DockingPanel;
				if (p == null)
					return;

				p.Refresh(child);
			}
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
		#region TabHeader property
		public static readonly DependencyProperty TabHeaderProperty =
				DependencyProperty.RegisterAttached(
						"TabHeader",
						typeof(string),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							"Default tab title",
							new PropertyChangedCallback(OnTabHeaderChanged))
					);
		private static void OnTabHeaderChanged(DependencyObject child, DependencyPropertyChangedEventArgs e)
		{
			var el = child as FrameworkElement;
			if (el == null)
				return;

			var ti = el.Parent as TabItem;
			if (ti == null)
				return;

			ti.Header = e.NewValue;
		}
		public static string GetTabHeader(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			return (string)element.GetValue(TabHeaderProperty);
		}

		public static void SetTabHeader(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.SetValue(TabHeaderProperty, value);
		}
		#endregion TabHeader property

		public DockingPanel()
			: base()
		{
			Loaded += OnLoadedEvent;

			_rootChild = CreateDefaultGrid();
			Children.Add(_rootChild);
		}

		private void OnLoadedEvent(object? sender, EventArgs e)
		{
			Debug.WriteLine("MyControl Loaded");
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

			if (visualAdded is UIElement elementAdded)
			{
				DockType dock = GetDock(elementAdded);

				switch (dock)
				{
					case DockType.Center:
					case DockType.None:
					{
						RemoveElementFromItsParent(elementAdded as FrameworkElement);

						string? header = GetTabHeader(elementAdded);

						TabControl tc = new TabControl();
						TabItem tcItem = new TabItem();
						tcItem.Header = "DockingTabControlItem";
						tcItem.Content = elementAdded;
						tc.Items.Add(tcItem);

						_rootChild.Children.Add(tc);

						Grid.SetRow(tc, 0);
						Grid.SetColumn(tc, 0);
					}
					break;
				}

				//	switch (dock)
				//	{
				//		case DockType.Top:
				//			if (_topChild != null)
				//			{
				//				ReplaceChild(_rootChild, elementAdded);
				//			}
				//			else
				//			{
				//				_topChild = elementAdded;
				//			}
				//			break;
				//		case DockType.Bottom:
				//			if (_bottomChild != null)
				//			{
				//				ReplaceChild(_bottomChild, elementAdded);
				//			}
				//			else
				//			{
				//				_bottomChild = elementAdded;
				//			}
				//			break;
				//		case DockType.Left:
				//			if (_leftChild != null)
				//			{
				//				ReplaceChild(_leftChild, elementAdded);
				//			}
				//			else
				//			{
				//				_leftChild = elementAdded;
				//			}
				//			break;

				//		case DockType.Right:
				//			if (_rightChild != null)
				//			{
				//				ReplaceChild(_rightChild, elementAdded);
				//			}
				//			else
				//			{
				//				_rightChild = elementAdded;
				//			}
				//			break;

				//		case DockType.None:
				//			return;

				//		default: // DockType.Center or anything else
				//		{
				//			RemoveElementFromItsParent(elementAdded as FrameworkElement);

				//			TabItem ti = new TabItem();
				//			ti.Header = "New Tab";
				//			ti.Content = elementAdded;
				//			_centerTab.Items.Add(ti);
				//		}
				//		break;
				//	}

				InvalidateMeasure();
			}

			if (visualRemoved is UIElement elementRemoved)
			{
				//	if (elementRemoved == _topChild) _topChild = null;
				//	if (elementRemoved == _bottomChild) _bottomChild = null;
				//	if (elementRemoved == _leftChild) _leftChild = null;
				//	if (elementRemoved == _rightChild) _rightChild = null;

				//	InvalidateMeasure();
			}
		}
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
