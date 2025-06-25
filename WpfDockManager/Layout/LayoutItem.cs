
using System.Windows;
using WpfDockingManager.Collections;

namespace WpfDockingManager.Layout
{
	public class LayoutItem : ITypeConverter<LayoutItem, DependencyObject>
	{
		public DependencyObject Object { get; set; } = null!;

		public LayoutItem()
		{
		}

		public LayoutItem(DependencyObject element)
		{
			Object = element;
		}

		public static implicit operator DependencyObject(LayoutItem li) => li.Object;
		public static explicit operator LayoutItem(DependencyObject element) => new LayoutItem(element);

		public LayoutItem? Convert(DependencyObject? value)
		{
			if (value == null)
				return null;

			Object = value;
			return this;
		}

		public DependencyObject? Convert(LayoutItem? value)
		{
			if (value == null)
				return null;

			return value.Object;
		}
	}
}
