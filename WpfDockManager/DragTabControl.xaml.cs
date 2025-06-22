using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockingManager
{
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

		private TabItem? _draggedTabItem;

		public DragTabControl()
		{
			InitializeComponent();

			ItemStartDraggingEventHandlers += OnItemStartDraggingHandler;
			ItemStopDraggingEventHandlers += OnItemStopDraggingHandler;
			ItemCloseEventHandlers += OnItemCloseHandler;
		}

		private void OnItemStartDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Cancel)
				return;

			_draggedTabItem = e.TabItem;
		}

		private void OnItemStopDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			if (e.Cancel)
				return;

			if (e.SourceIndex == -1 || e.TargetIndex == -1)
				return;

			var tabControl = e.Source as DragTabControl;
			if (tabControl == null)
				return;

			tabControl.Items.RemoveAt(e.SourceIndex);
			tabControl.Items.Insert(e.TargetIndex, e.TabItem);

			// None of these works. The targetpanel will not refresh.
			//tabControl.SelectedItem = e.TabItem;
			//tabControl.SelectedIndex = e.TargetIndex;

			// Setting the SelectedItem/-Index doesn't refresh the TabControl panel and it stays
			// blank for some reason. So we have to set the index accordingly.
			// https://stackoverflow.com/questions/33974939/difficulty-with-tabcontrol-tabitem-refresh
			Dispatcher.BeginInvoke((Action)(() => SelectedIndex = e.TargetIndex));
			_draggedTabItem = null;
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
			var tabControl = (TabControl)sender;
			var draggedTabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);

			if (draggedTabItem != null)
			{
				var ev = new DragTabItemEventArgs(ItemStartDraggingEventEvent)
				{
					TabItem = draggedTabItem,
					SourceIndex = Items.IndexOf(draggedTabItem),
					TargetIndex = -1
				};
				RaiseEvent(ev);
				if (ev.Cancel)
					return;

				DragDrop.DoDragDrop(tabControl, _draggedTabItem, DragDropEffects.Move);
				tabControl.SelectedItem = _draggedTabItem;
			}
		}

		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && _draggedTabItem == null)
			{
				var tabControl = (TabControl)sender;
				TabItem? tabItem = DockingHelper.FindParentClass<TabItem>((DependencyObject)e.OriginalSource);
				if (tabItem != null)
				{
					_draggedTabItem = tabItem;
					DragDrop.DoDragDrop(tabControl, _draggedTabItem, DragDropEffects.Move);
					tabControl.SelectedItem = _draggedTabItem;
				}
			}
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(TabItem)))
			{
				e.Effects = DragDropEffects.Move;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
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
				TabItem = _draggedTabItem,
				SourceIndex = draggedIndex,
				TargetIndex = targetIndex
			};
			_draggedTabItem = null;
			RaiseEvent(ev);
		}
	}
}
