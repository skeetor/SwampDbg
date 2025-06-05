
using System.Windows;

namespace WpfDockManager.Layout
{
	public class LayoutItem
	{
		public DependencyObject Object { get; set; } = null!;
	}

	public class LayoutItemList
	{
		private List<LayoutItem> _items = new List<LayoutItem>();

		public LayoutItem? Find(DependencyObject? element)
		{
			if (element == null)
				return null;

			foreach (var item in _items)
			{
				if (item.Object == element)
					return item;
			}

			return null;
		}

		public void Add(DependencyObject? element)
		{
			LayoutItem? layoutItem = Find(element);
			if (layoutItem != null)
				return;

			_items.Add(new LayoutItem()
			{
				Object = element!
			});
		}

		public void Remove(DependencyObject? element)
		{
			LayoutItem? layoutItem = Find(element);
			if (layoutItem == null)
				return;

			_items.Remove(layoutItem!);
		}

	}
}
