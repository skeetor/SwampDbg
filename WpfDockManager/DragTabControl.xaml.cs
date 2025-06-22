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

		public DragTabControl()
		{
			InitializeComponent();

			ItemStartDraggingEventHandlers += OnItemStartDraggingHandler;
			ItemStopDraggingEventHandlers += OnItemStopDraggingHandler;
			ItemCloseEventHandlers += OnItemCloseHandler;
		}

		private void OnItemStartDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OnItemStopDraggingHandler(object? sender, DragTabItemEventArgs e)
		{
			throw new NotImplementedException();
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
	}
}
