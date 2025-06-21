using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockingManager
{
	public class DragTabControlEvent : CancelEventArgs
	{
		public enum EventType
		{
			None,
			Close,
			Move,
			Add,
			Remove
		}

		public EventType Reason { get; set; } = EventType.None;
		public TabItem? TabItem { get; set; } = null;
		public int SourceIndex { get; set; } = -1;
		public int TargetIndex { get; set; } = -1;
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

		public event CancelEventHandler CloseButtonClicked;

		public DragTabControl()
		{
			InitializeComponent();

			CloseButtonClicked += OnCloseButtonEventHandler;
		}

		private void OnCloseButtonEventHandler(object? sender, CancelEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OnCloseButtonEvent(object sender, RoutedEventArgs e)
		{
			var button = sender as DependencyObject;
			if (button == null)
				throw new InvalidOperationException("Unknown sender type");

			TabItem? tabItem = null;
			DragTabControl? tabControl = FindContainers(button, out tabItem);

			if (tabControl == null)
				return;

			DragTabControlEvent eventArg = new DragTabControlEvent()
			{
				Reason = DragTabControlEvent.EventType.Close,
				TabItem = tabItem,
				SourceIndex = tabControl.Items.IndexOf(tabItem)
			};

			CloseButtonClicked(tabControl, eventArg);

			if (tabControl != null)
				tabControl.OnCloseButton(tabItem);
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

		public virtual void OnCloseButton(TabItem? tabItem)
		{
		}
	}
}
