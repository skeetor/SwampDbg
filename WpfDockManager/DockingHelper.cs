using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockManager
{
	internal class DockingHelper
	{
		/// <summary>
		/// Recursively finds the specified parent in a control hierarchy
		/// </summary>
		/// <typeparam name="T">The type of the targeted Find</typeparam>
		/// <param name="child">The child control to start with</param>
		/// <returns></returns>
		public static T? FindParentClass<T>(DependencyObject child) where T : DependencyObject
		{
			if (child == null)
				return null;

			T? foundParent = null;
			var currentParent = VisualTreeHelper.GetParent(child);
			if (currentParent == null)
				return null;

			do
			{
				var frameworkElement = currentParent as FrameworkElement;
				if (frameworkElement is T)
				{
					foundParent = (T)currentParent;
					break;
				}

				currentParent = VisualTreeHelper.GetParent(currentParent);

			} while (currentParent != null);

			return foundParent;
		}
		public static void RemoveElementFromItsParent(FrameworkElement? el)
		{
			if (el == null)
				return;

			if (el.Parent == null)
				return;

			var panel = el.Parent as Panel;
			if (panel != null)
			{
				panel.Children.Remove(el);
				return;
			}

			var decorator = el.Parent as Decorator;
			if (decorator != null)
			{
				decorator.Child = null;
				return;
			}

			var contentPresenter = el.Parent as ContentPresenter;
			if (contentPresenter != null)
			{
				contentPresenter.Content = null;
				return;
			}

			var contentControl = el.Parent as ContentControl;
			if (contentControl != null)
				contentControl.Content = null;
		}
	}
}
