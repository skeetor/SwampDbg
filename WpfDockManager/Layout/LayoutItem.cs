
using System.CodeDom;
using System.Collections;
using System.Configuration;
using System.Windows;

namespace WpfDockManager.Layout
{
	public class LayoutItem
	{
		public DependencyObject Object { get; set; } = null!;
	}

	public class LayoutItemList : IEnumerable<LayoutItem>
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

		public LayoutItem? Add(DependencyObject? element)
		{
			LayoutItem? layoutItem = Find(element);
			if (layoutItem != null)
				return null;

			var li = new LayoutItem()
			{
				Object = element!
			};

			_items.Add(li);
			return li;
		}

		public LayoutItem? this[DependencyObject? element]
		{
			get { return Find(element); }
			set { Add(element); }
		}

		public void Remove(DependencyObject? element)
		{
			LayoutItem? layoutItem = Find(element);
			if (layoutItem == null)
				return;

			_items.Remove(layoutItem!);
		}

		public static LayoutItemList operator +(LayoutItemList list, DependencyObject? element)
		{
			list.Add(element);
			return list;
		}
		public static LayoutItemList operator -(LayoutItemList list, DependencyObject? element)
		{
			list.Remove(element);
			return list;
		}

		public IEnumerator<LayoutItem> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
