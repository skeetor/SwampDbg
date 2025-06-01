using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfDockManager
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockManagerNS="clr-namespace:WpfDockManager"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:WpfDockManagerNS="clr-namespace:WpfDockManager;assembly=WpfDockManager"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <WpfDockManagerNS:DockingPanel/>
	///
	/// </summary>

	public enum DockType
	{
		Left,
		Top,
		Right,
		Bottom,
		Center
	}

	[ContentProperty(nameof(Children))]
	public class DockingPanel : ContentControl
	{
		private List<UIElement> _children;
		public static DockingPanel? Root;

		#region static

		static DockingPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingPanel), new FrameworkPropertyMetadata(typeof(DockingPanel)));
		}

		//[CommonDependencyProperty]
		public static readonly DependencyProperty DockProperty =
				DependencyProperty.RegisterAttached(
						"Dock",
						typeof(DockType),
						typeof(DockingPanel),
						new FrameworkPropertyMetadata(
							DockType.Center,
							new PropertyChangedCallback(OnDockChanged)),
						new ValidateValueCallback(IsValidDock)
				);

		internal static bool IsValidDock(object o)
		{
			DockType dock = (DockType)o;

			return (dock == DockType.Left
					|| dock == DockType.Top
					|| dock == DockType.Right
					|| dock == DockType.Bottom
					|| dock == DockType.Center
					);
		}

		private static void OnDockChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			UIElement? child = depObj as UIElement;
			if (child == null)
				return;

			DockingPanel? p = VisualTreeHelper.GetParent(child) as DockingPanel;
			if (p == null)
				return;

			p.InvalidateMeasure();
		}

		#endregion static

		public DockingPanel()
			: base()
		{
			_children = new List<UIElement>();
			Root = this;
		}
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			// We create a grid that always spans the full window
/*			var grid = new Grid();
			RowDefinition row = new RowDefinition();
			row.Height = new GridLength(1.0, GridUnitType.Star);
			grid.RowDefinitions.Add(row);

			ColumnDefinition column = new ColumnDefinition();
			column.Width = new GridLength(1.0, GridUnitType.Star);
			grid.ColumnDefinitions.Add(column);

			var btn = new Button();
			btn.Content = "Testbutton";

			Grid.SetRow(btn, 0);
			Grid.SetColumn(btn, 0);
			grid.Children.Add(btn);

			AddChild(grid);
*/
			// Additional initialization logic can be added here
		}

		[AttachedPropertyBrowsableForChildren]
		public static ObservableCollection<UIElement> GetDock(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("'element' argument is null");

			return (ObservableCollection<UIElement>)element.GetValue(DockProperty);
		}
		public static void SetDock(UIElement element, DockType dock)
		{
			element.SetValue(DockProperty, dock);
		}

		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<UIElement> Children
		{
			get
			{
				return _children;
			}
		}
		protected override Size MeasureOverride(Size availableSize)
		{
			//Size desiredSize = new Size();
			foreach (UIElement child in Children)
			{
				var parent = VisualTreeHelper.GetParent(child);
				if (parent == null)
					System.Console.WriteLine("test");
				//child.Measure(availableSize);
				//desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width);
				//desiredSize.Height += child.DesiredSize.Height;
			}
			//return desiredSize;
			return new Size(500, 200);
		}

		//protected override Size ArrangeOverride(Size finalSize)
		//{
		//	return new Size(100, 200);
		//	//double currentY = 0;
		//	//foreach (UIElement child in Children)
		//	//{
		//	//	child.Arrange(new Rect(0, currentY, finalSize.Width, child.DesiredSize.Height));
		//	//	currentY += child.DesiredSize.Height;
		//	//}
		//	//return finalSize;
		//}
	}
}
