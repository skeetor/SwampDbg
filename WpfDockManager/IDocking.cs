
using System.Windows.Automation;
using System.Windows;

namespace WpfDockingManager
{
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
}
