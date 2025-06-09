using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockManager
{
	public class DockingSplitter : Grid
	{
		public enum Alignment
		{
			Horizontal,
			Vertical
		}

		public const int DefaultHandleWidth = 4;
		public const int DefaultHandleHeigth = 4;
		private Alignment _alignment = Alignment.Vertical;

		public DockingSplitter()
			: base()
		{
			//var row = new RowDefinition();
			//row.Height = new GridLength(1.0, GridUnitType.Star);
			//RowDefinitions.Add(row);

			//var column = new ColumnDefinition();
			//column.Width = new GridLength(1.0, GridUnitType.Star);
			//ColumnDefinitions.Add(column);
		}

		public Alignment Aligned
		{
			get { return _alignment; }

			set
			{
				if (Children.Count > 0 && value != _alignment)
					throw new InvalidOperationException("Alignment change after childs added.");
			}
		}

		public bool IsEmpty() => Children.Count == 0;

		protected GridSplitter GetDefaultGridSplitter()
		{
			return new GridSplitter
			{
				// TODO: These options might have to be customized later
				Width = DefaultHandleWidth,
				Background = Brushes.Black,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
		}

		public void Add(UIElement element) => Insert(element);

		public void Insert(UIElement? element, int index = -1)
		{
			if (Aligned == Alignment.Vertical)
				InsertColumn(element, index);
			else
				InsertRow(element, index);
		}

		private void MoveChildren(int offset)
		{
			if (offset == 0)
				return;

			foreach (UIElement child in Children.Cast<UIElement>())
			{
				if (Aligned == Alignment.Vertical)
					Grid.SetColumn(child, GetColumn(child)+offset);
				else
					Grid.SetRow(child, GetRow(child)+offset);
			}
		}

		protected void InsertColumn(UIElement? element, int index = -1)
		{
			bool append = false;
			bool splitter = true;

			if (index  == -1 || index > Children.Count)
				index = Children.Count;

			if (Children.Count != 0)
			{
				if (index >= Children.Count)
					append = true;
			}
			else
			{
				append = true;
				splitter = false;
			}

			var newColumn = new ColumnDefinition();
			newColumn.Width = new GridLength(1.0, GridUnitType.Star);

			if (append)
			{
				if (splitter)
					InsertColumnSplitter(index, 0);

				ColumnDefinitions.Add(newColumn);
				index = Children.Add(element);
			}
			else
			{
				if (splitter)
					MoveChildren(2);

				if (splitter)
					InsertColumnSplitter(index, 1);

				ColumnDefinitions.Insert(index, newColumn);
				Children.Insert(index, element);
			}

			Grid.SetColumn(element, index);
		}

		protected void InsertColumnSplitter(int index, int columnOffset)
		{
			var newSplitterColumn = new ColumnDefinition() { Width = new GridLength(DefaultHandleWidth + 1) };
			var gridSplitter = GetDefaultGridSplitter();

			ColumnDefinitions.Insert(index, newSplitterColumn);
			Children.Insert(index, gridSplitter);
			Grid.SetColumn(gridSplitter, index+columnOffset);
		}

		protected void InsertRow(UIElement? element, int index = -1)
		{
		}

		protected void InsertRowSplitter(int index = -1)
		{
		}

		public UIElement? GetChild(int index)
		{
			if (Aligned == Alignment.Vertical)
			{
				try
				{
					var c = Children
						.Cast<UIElement>()
						.First(e => Grid.GetColumn(e) == index);

					return c;
				}
				catch
				{
				}
			}
			else
			{
				try
				{
					var c = Children
						.Cast<UIElement>()
						.First(e => Grid.GetRow(e) == index);

					return c;
				}
				catch
				{
				}
			}

			return null;
		}
	}
}
