using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfDockingManager
{
	public class DockingPanel : Grid
	{
		public DockingPanel()
			: base()
		{
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			// Track when objects are added and removed
			if (visualAdded != null)
			{
				// Do stuff with the added object
			}
			if (visualRemoved != null)
			{
				// Do stuff with the removed object
			}

			// Call base function
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
	}
}
