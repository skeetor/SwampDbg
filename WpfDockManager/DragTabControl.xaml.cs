using System.Data.Common;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace WpfDockingManager
{
	public class DragStateInfo
	{
		public const double DefaultStartDragDistance = 7.0;

		public bool IsDragging { get; set; } = false;
		public DragTabControl? Owner { get; } = null;
		public TabItem? TabItem { get; } = null;
		public int TabIndex { get; } = -1;
		public Point MousePosition { get; } = default(Point);

		public DragStateInfo()
		{
		}

		public DragStateInfo(DragTabControl? owner, TabItem? tabItem, int tabIndex, Point mousePosition)
		{
			IsDragging = false;
			Owner = owner;
			TabItem = tabItem;
			TabIndex = tabIndex;
			MousePosition = mousePosition;
		}
	}

	public partial class DragTabControl : TabControl
	{
		#region Properties
		public static readonly DependencyProperty TabHeightProperty =
			DependencyProperty.Register("TabHeight", typeof(double), typeof(DragTabControl),
				new PropertyMetadata(double.NaN));

		//public static readonly DependencyProperty TabFontSizeProperty =
		//	DependencyProperty.Register("TabFontSize", typeof(double), typeof(DragTabControl),
		//		new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty CloseTabCommandProperty =
			DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DragTabControl));

		public double TabHeight
		{
			get { return (double)GetValue(TabHeightProperty); }
			set { SetValue(TabHeightProperty, value); }
		}
		//public double TabFontSize
		//{
		//	get { return (double)GetValue(TabFontSizeProperty); }
		//	set { SetValue(TabFontSizeProperty, value); }
		//}

		public ICommand CloseTabCommand
		{
			get { return (ICommand)GetValue(CloseTabCommandProperty); }
			set { SetValue(CloseTabCommandProperty, value); }
		}
		#endregion Properties
		#region Events
		public static readonly RoutedEvent ItemCloseEventEvent = EventManager.RegisterRoutedEvent(
			 "ItemCloseEvent", RoutingStrategy.Bubble, typeof(DragTabEventHandler), typeof(DragTabItemEventArgs));

		public event DragTabEventHandler ItemCloseEventHandlers
		{
			add { AddHandler(ItemCloseEventEvent, value); }
			remove { RemoveHandler(ItemCloseEventEvent, value); }
		}

		public static readonly RoutedEvent ItemStartDraggingEventEvent = EventManager.RegisterRoutedEvent(
			 "ItemStartDraggingEvent", RoutingStrategy.Bubble, typeof(DragTabEventHandler), typeof(DragTabItemEventArgs));

		public event DragTabEventHandler ItemStartDraggingEventHandlers
		{
			add { AddHandler(ItemStartDraggingEventEvent, value); }
			remove { RemoveHandler(ItemStartDraggingEventEvent, value); }
		}

		public static readonly RoutedEvent ItemStopDraggingEventEvent = EventManager.RegisterRoutedEvent(
			 "ItemStopDraggingEvent", RoutingStrategy.Bubble, typeof(DragTabEventHandler), typeof(DragTabItemEventArgs));

		public event DragTabEventHandler ItemStopDraggingEventHandlers
		{
			add { AddHandler(ItemStopDraggingEventEvent, value); }
			remove { RemoveHandler(ItemStopDraggingEventEvent, value); }
		}
		#endregion Events

		public DragStateInfo DragState { get; private set; } = new DragStateInfo();

		public DragTabControl()
		{
			InitializeComponent();

			ItemStartDraggingEventHandlers += OnItemStartDraggingHandler;
			ItemStopDraggingEventHandlers += OnItemStopDraggingHandler;
			ItemCloseEventHandlers += OnItemCloseHandler;
		}

		private void OnItemStartDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Cancel || e.Handled)
				return;

			DragDrop.DoDragDrop(this, e.TabItem, DragDropEffects.Move);
			SelectedItem = e.TabItem;
			DragState.IsDragging = true;
		}

		private void OnItemStopDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Handled)
			{
				DragState = new DragStateInfo();
				return;
			}

			if (e.SourceIndex == -1 || e.TargetIndex == -1)
			{
				DragState = new DragStateInfo();
				return;
			}

			var tabControl = e.Source as DragTabControl;
			if (tabControl == null)
			{
				DragState = new DragStateInfo();
				return;
			}

			tabControl.Items.RemoveAt(e.SourceIndex);
			tabControl.Items.Insert(e.TargetIndex, e.TabItem);

			// None of these works. The targetpanel will not refresh.
			//tabControl.SelectedItem = e.TabItem;
			//tabControl.SelectedIndex = e.TargetIndex;

			// Setting the SelectedItem/-Index doesn't refresh the TabControl panel and it stays
			// blank for some reason. So we have to set the index accordingly.
			// https://stackoverflow.com/questions/33974939/difficulty-with-tabcontrol-tabitem-refresh
			Dispatcher.BeginInvoke((Action)(() => SelectedIndex = e.TargetIndex));
			DragState = new DragStateInfo();
		}

		private void OnItemCloseHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Handled || e.Cancel)
				return;

			var ctrl = e.Source as DragTabControl;
			if (ctrl == null)
				return;

			ctrl.Items.RemoveAt(e.SourceIndex);
			e.Handled = true;
		}

		private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
		{
			var button = sender as DependencyObject;
			if (button == null)
				throw new InvalidOperationException("Unknown sender type");

			TabItem? tabItem = null;
			DragTabControl? tabControl = FindContainers(button, out tabItem);

			if (tabControl == null || tabControl != this)
				return;

			var eventArgs = new DragTabItemEventArgs(ItemCloseEventEvent)
			{
				TabItem = tabItem,
				SourceIndex = tabControl.Items.IndexOf(tabItem)
			};

			RaiseEvent(eventArgs);
		}

		private DragTabControl? FindContainers(DependencyObject element, out TabItem? tabItem)
		{
			tabItem = DockingHelper.FindParentClass<TabItem>(element);
			if (tabItem == null)
				return null;

			var parent = DockingHelper.FindParentClass<DragTabControl>(tabItem);
			if (parent == null)
			{
				tabItem = null;
				return null;
			}

			return parent;
		}

		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			DragState = new DragStateInfo();
			TabItem? tabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);

			if (tabItem == null)
				return;

			DragState = new DragStateInfo(this, tabItem, Items.IndexOf(tabItem), e.GetPosition(this));

			// TODO: Do we want to make the dragged item the selected one?
			// SelectedItem = _dragState.TabItem;
		}

		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed || DragState.TabItem == null)
				return;

			if (DragState.IsDragging)
				return;

			var curPos = e.GetPosition(this);
			Point p = new Point(
							curPos.X - DragState.MousePosition.X,
							curPos.Y - DragState.MousePosition.Y
						);
			var dist = Math.Sqrt(p.X*p.X + p.Y*p.Y);

			// Only start dragging if the user moved the mouse a certain distance.
			if (dist < DragStateInfo.DefaultStartDragDistance)
				return;

			var ev = new DragTabItemEventArgs(ItemStartDraggingEventEvent)
			{
				TabItem = DragState.TabItem,
				SourceIndex = DragState.TabIndex,
				TargetIndex = -1
			};
			RaiseEvent(ev);
			if (ev.Cancel)
			{
				DragState = new DragStateInfo();
				return;
			}
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Handled)
				return;

			if (IsDragTarget((DependencyObject)e.OriginalSource))
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;

			e.Handled = true;
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			if (e.Handled)
				return;

			if (IsDragTarget((DependencyObject)e.OriginalSource))
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;

			e.Handled = true;
		}

		private bool IsDragTarget(DependencyObject element)
		{
			var tabItem = DockingHelper.FindParentClass<TabItem>(element);
			if (tabItem == null)
				return false;

			// TODO: Element needs to be the item the mouse is currently over.
			var tabCtrl = DockingHelper.FindParentClass<DragTabControl>(tabItem);
			if (tabCtrl == null || tabCtrl != this)
				return false;

			return true;
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			var tabControl = (TabControl)sender;
			TabItem? targetTabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);

			int targetIndex = -1;

			if (targetTabItem != null && targetTabItem != DragState.TabItem)
				targetIndex = tabControl.Items.IndexOf(targetTabItem);

			var ev = new DragTabItemEventArgs(ItemStopDraggingEventEvent)
			{
				TabItem = DragState.TabItem,
				SourceIndex = DragState.TabIndex,
				TargetIndex = targetIndex
			};
			RaiseEvent(ev);
		}

		private void OnScrollLeftButton(object sender, RoutedEventArgs e)
		{
			var leftButton = sender as Button;
			if (leftButton == null)
				return;

			var scrollViewer = FindScrollViewer(leftButton)!;
			if (scrollViewer == null)
				return;

			var rightButton = FindButton(DockingHelper.FindParentClass<Grid>(scrollViewer), "Right");
			if (rightButton == null)
				return;

			var tabControl = DockingHelper.FindParentClass<TabControl>(scrollViewer);
			if (tabControl == null)
				return;

			double scrollPos = scrollViewer.HorizontalOffset;
			double newPos = 0;

			rightButton.IsEnabled = true;
			for (int i = 0; i < tabControl.Items.Count - 1; i++)
			{
				var tabItem = tabControl.Items[i] as TabItem;
				if (tabItem == null)
					continue;

				if (newPos + tabItem.ActualWidth >= scrollPos)
				{
					scrollViewer.ScrollToHorizontalOffset(newPos);
					if (newPos <= 0)
						leftButton.IsEnabled = false;

					return;
				}
				newPos += tabItem.ActualWidth;
			}
		}

		private void OnScrollRightButton(object sender, RoutedEventArgs e)
		{
			var rightButton = sender as Button;
			if (rightButton == null)
				return;

			var scrollViewer = FindScrollViewer(rightButton)!;
			if (scrollViewer == null)
				return;

			var leftButton = FindButton(DockingHelper.FindParentClass<Grid>(scrollViewer), "Left");
			if (leftButton == null)
				return;

			var tabControl = DockingHelper.FindParentClass<TabControl>(scrollViewer);
			if (tabControl == null)
				return;

			double scrollPos = scrollViewer.HorizontalOffset;
			double newPos = 0;

			leftButton.IsEnabled = true;
			for (int i = 0; i < tabControl.Items.Count-1; i++)
			{
				var tabItem = tabControl.Items[i] as TabItem;
				if (tabItem == null)
					continue;

				newPos += tabItem.ActualWidth;
				if (newPos > scrollPos)
				{
					scrollViewer.ScrollToHorizontalOffset(newPos);
					if (newPos + scrollViewer.ViewportWidth >= scrollViewer.ExtentWidth)
						rightButton.IsEnabled = false;

					return;
				}
			}
		}

		private void OnScrollUpButton(object sender, RoutedEventArgs e)
		{
			var upButton = sender as Button;
			if (upButton == null)
				return;

			var scrollViewer = FindScrollViewer(upButton)!;
			if (scrollViewer == null)
				return;

			var downButton = FindButton(DockingHelper.FindParentClass<Grid>(scrollViewer), "Down");
			if (downButton == null)
				return;

			var tabControl = DockingHelper.FindParentClass<TabControl>(scrollViewer);
			if (tabControl == null)
				return;

			double scrollPos = scrollViewer.VerticalOffset;
			double newPos = 0;

			downButton.IsEnabled = true;
			for (int i = 0; i < tabControl.Items.Count - 1; i++)
			{
				var tabItem = tabControl.Items[i] as TabItem;
				if (tabItem == null)
					continue;

				if (newPos + tabItem.ActualHeight >= scrollPos)
				{
					scrollViewer.ScrollToVerticalOffset(newPos);
					if (newPos <= 0)
						upButton.IsEnabled = false;

					return;
				}
				newPos += tabItem.ActualHeight;
			}
		}

		private void OnScrollDownButton(object sender, RoutedEventArgs e)
		{
			var downButton = sender as Button;
			if (downButton == null)
				return;

			var scrollViewer = FindScrollViewer(downButton)!;
			if (scrollViewer == null)
				return;

			var upButton = FindButton(DockingHelper.FindParentClass<Grid>(scrollViewer), "Up");
			if (upButton == null)
				return;

			var tabControl = DockingHelper.FindParentClass<TabControl>(scrollViewer);
			if (tabControl == null)
				return;

			double scrollPos = scrollViewer.VerticalOffset;
			double newPos = 0;

			upButton.IsEnabled = true;
			for (int i = 0; i < tabControl.Items.Count - 1; i++)
			{
				var tabItem = tabControl.Items[i] as TabItem;
				if (tabItem == null)
					continue;

				newPos += tabItem.ActualHeight;
				if (newPos > scrollPos)
				{
					scrollViewer.ScrollToVerticalOffset(newPos);
					if (newPos + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight)
						downButton.IsEnabled = false;

					return;
				}
			}
		}

		private ScrollViewer? FindScrollViewer(DependencyObject? depObj)
		{
			if (depObj == null)
				return null;

			var grid = VisualTreeHelper.GetParent(depObj) as Grid;
			if (grid == null)
				return null;

			var scroller = grid.Children
				.Cast<UIElement>()
				.First(e => e is ScrollViewer) as ScrollViewer;
			if (scroller == null)
				return null;

			return scroller;
		}

		private void OnScrollChangedHorizontally(object sender, ScrollChangedEventArgs e)
		{
			var scroller = sender as ScrollViewer;
			if (scroller == null)
				return;

			bool canScrollLeft = scroller.HorizontalOffset > 0;
			bool canScrollRight = scroller.HorizontalOffset < scroller.ExtentWidth - scroller.ViewportWidth;

			var grid = VisualTreeHelper.GetParent(scroller) as Grid;
			if (grid == null)
				return;

			var leftButton = FindButton(grid, "Left");
			var rightButton = FindButton(grid, "Right");

			if (leftButton != null)
			{
				if(canScrollLeft)
					leftButton.IsEnabled = true;
				else
					leftButton.IsEnabled = false;
			}

			if (rightButton != null)
			{
				if (canScrollRight)
					rightButton.IsEnabled = true;
				else
					rightButton.IsEnabled = false;
			}

			UpdateScrollButtons(grid, canScrollLeft || canScrollRight, leftButton, rightButton);
		}

		private void OnScrollChangedVertically(object sender, ScrollChangedEventArgs e)
		{
			var scroller = sender as ScrollViewer;
			if (scroller == null)
				return;

			bool canScrollUp = scroller.VerticalOffset > 0;
			bool canScrollDown = scroller.VerticalOffset < scroller.ExtentHeight - scroller.ViewportHeight;

			var grid = VisualTreeHelper.GetParent(scroller) as Grid;
			if (grid == null)
				return;

			var upButton = FindButton(grid, "Up");
			var downButton = FindButton(grid, "Down");

			if (upButton != null)
			{
				if (canScrollUp)
					upButton.IsEnabled = true;
				else
					upButton.IsEnabled = false;
			}

			if (downButton != null)
			{
				if (canScrollDown)
					downButton.IsEnabled = true;
				else
					downButton.IsEnabled = false;
			}

			UpdateScrollButtons(grid, canScrollUp || canScrollDown, upButton, downButton);
		}

		private Button? FindButton(Grid? grid, string position)
		{
			if (grid == null)
				return null;

			var tagText = "ScrollBtn" + position;
			foreach (var child in grid.Children)
			{
				var btn = child as Button;
				if (btn == null)
					continue;

				if (btn.Tag.ToString() == tagText)
					return btn;
			}

			return null;
		}

		private void UpdateScrollButtons(Grid grid, bool visible, Button? backward, Button? forward)
		{
			if (backward == null || forward == null)
				return;

			var tabControl = DockingHelper.FindParentClass<TabControl>(grid);
			if (tabControl == null)
				return;

			if (tabControl.Items.Count <= 1)
				visible = false;

			if (visible)
			{
				backward.Visibility = Visibility.Visible;
				forward.Visibility = Visibility.Visible;
			}
			else
			{
				backward.Visibility = Visibility.Collapsed;
				forward.Visibility = Visibility.Collapsed;
			}
		}
	}
}
