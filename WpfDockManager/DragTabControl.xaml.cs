using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

		public static readonly DependencyProperty CloseTabCommandProperty =
			DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DragTabControl));

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
		private void ScrollLeft_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var scrollViewer = FindScrollViewer(button);
			scrollViewer?.LineLeft();
		}

		private void ScrollRight_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var scrollViewer = FindScrollViewer(button);
			scrollViewer?.LineRight();
		}

		private ScrollViewer? FindScrollViewer(DependencyObject? depObj)
		{
			if (depObj == null)
				return null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
				if (child is ScrollViewer scrollViewer)
					return scrollViewer;
				else
				{
					ScrollViewer? childScrollViewer = FindScrollViewer(child);
					if (childScrollViewer != null)
						return childScrollViewer;
				}
			}

			return null;
		}
	}
}
