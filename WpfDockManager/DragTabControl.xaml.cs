using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfDockingManager
{
	public class DragState
	{
		public const double DefaultStartDragDistance = 7.0;

		public bool IsDragging { get; set; } = false;
		public TabItem? TabItem { get; set; } = null;
		public int TabIndex { get; set; } = -1;
		public Point MousePosition { get; set; } = default(Point);
		public double DragDistance {  get; set; } = 0;
	}

	public partial class DragTabControl : TabControl
	{
		#region Properties
		public static readonly DependencyProperty TabWidthProperty =
			DependencyProperty.Register("TabWidth", typeof(double), typeof(DragTabControl),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty TabHeightProperty =
			DependencyProperty.Register("TabHeight", typeof(double), typeof(DragTabControl),
				new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty CloseTabCommandProperty =
			DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(DragTabControl));

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

		private DragState _dragState = new DragState();

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
			_dragState.IsDragging = true;
		}

		private void OnItemStopDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Handled)
			{
				_dragState = new DragState();
				return;
			}

			if (e.SourceIndex == -1 || e.TargetIndex == -1)
			{
				_dragState = new DragState();
				return;
			}

			var tabControl = e.Source as DragTabControl;
			if (tabControl == null)
			{
				_dragState = new DragState();
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
			_dragState = new DragState();
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
			_dragState = new DragState();
			_dragState.TabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);

			if (_dragState.TabItem == null)
				return;

			_dragState.MousePosition = e.GetPosition(this);
			_dragState.DragDistance = 0;

			// TODO: Do we want to make the dragged item the selected one?
			// SelectedItem = _dragState.TabItem;
		}

		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed || _dragState.TabItem == null)
				return;

			if (_dragState.IsDragging)
				return;

			var curPos = e.GetPosition(this);
			Point p = new Point(
							curPos.X - _dragState.MousePosition.X,
							curPos.Y - _dragState.MousePosition.Y
						);
			var dist = Math.Sqrt(p.X*p.X + p.Y*p.Y);

			// Only start dragging if the user moved the mouse a certain distance.
			if (dist < DragState.DefaultStartDragDistance)
				return;

			var ev = new DragTabItemEventArgs(ItemStartDraggingEventEvent)
			{
				TabItem = _dragState.TabItem,
				SourceIndex = _dragState.TabIndex,
				TargetIndex = -1
			};
			RaiseEvent(ev);
			if (ev.Cancel)
			{
				_dragState = new DragState();
				return;
			}
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Handled)
				return;

			var tabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);
			if (tabItem != null)
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;

			e.Handled = true;
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			if (e.Handled)
				return;

			var tabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);
			if (tabItem != null)
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;

			e.Handled = true;
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			var tabControl = (TabControl)sender;
			TabItem? targetTabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);
			TabItem draggedItem = (TabItem)e.Data.GetData(typeof(TabItem));

			int targetIndex = -1;
			int draggedIndex = -1;

			if (targetTabItem != null && draggedItem != null && targetTabItem != draggedItem)
			{
				targetIndex = tabControl.Items.IndexOf(targetTabItem);
				draggedIndex = tabControl.Items.IndexOf(draggedItem);
			}

			var ev = new DragTabItemEventArgs(ItemStopDraggingEventEvent)
			{
				TabItem = _dragState.TabItem,
				SourceIndex = draggedIndex,
				TargetIndex = targetIndex
			};
			RaiseEvent(ev);
		}
	}
}
