
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace WpfDockingManager
{
	public enum DockingEventType
	{
		None,
		Close,
		CanMove,
		Move,
		CanAdd,
		Add,
		CanRemove,
		Remove,
	}

	public enum Alignment
	{
		Horizontal = 1,
		Vertical = 2
	}

	public interface IDockingProvider
	{
		public IDockingPanel GetDockingPanel();
	}

	public interface IDockingPanel
	{
		public void DockElement(UIElement element, DockPosition dock, UIElement? target = null, int index = -1);
		public void UndockElement(UIElement? element);
		public IDockingProvider DockingFloat(UIElement element, DockPosition dock, UIElement? target = null, int index = -1, bool show = true, Rect position = default);
	}

	public delegate void DragTabEventHandler(object? sender, DragTabItemEventArgs e);
	public class DragTabItemEventArgs : RoutedEventArgs //CancelEventArgs
	{
		public DragTabItemEventArgs()
			:base()
		{
		}

		public DragTabItemEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public DragTabItemEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}

		public bool Cancel { get; set; } = false;

		public TabItem? TabItem { get; set; } = null;
		public int SourceIndex { get; set; } = -1;
		public int TargetIndex { get; set; } = -1;
	}
}
