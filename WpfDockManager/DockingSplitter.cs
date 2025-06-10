using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockManager
{
	/// <summary>
	/// The DockingSplitter acts similar to a StackPanel, only it inserts drag handles
	/// in between items, so the items can be resized. Because of this it is derived
	/// from Grid instead of StackPanel because Grid supports Splitters natively
	/// while StackPanel does not.
	/// </summary>
	public class DockingSplitter : Grid
	{
		public enum Alignment
		{
			Horizontal,
			Vertical
		}

		public const int DefaultHandleWidth = 4;
		public const int DefaultHandleHeight = 4;
		private Alignment _alignment = Alignment.Vertical;

		public DockingSplitter()
			: base()
		{
		}

		public Alignment Aligned
		{
			get { return _alignment; }

			set
			{
				if (Children.Count > 0 && value != _alignment)
					throw new InvalidOperationException("Alignment change after childs added.");

				_alignment = value;
			}
		}

		public int GetIndex(UIElement? element)
		{
			if (element == null)
				return -1;

			if (Aligned == Alignment.Vertical)
				return GetColumn(element);

			return GetRow(element);
		}

		private void SetIndex(UIElement? element, int index)
		{
			if (element == null)
				return;

			if (Aligned == Alignment.Vertical)
				SetColumn(element, index);

			SetRow(element, index);
		}

		public UIElement? GetChild(int index)
		{
			try
			{
				var c = Children
					.Cast<UIElement>()
					.First(e => GetIndex(e) == index);

				return c;
			}
			catch
			{
			}

			return null;
		}

		public bool IsEmpty() => Children.Count == 0;

		protected GridSplitter GetDefaultGridSplitter()
		{
			var brush = Brushes.Black;

			if (Aligned == Alignment.Vertical)
			{
				return new GridSplitter
				{
					// TODO: These options might have to be customized later
					Width = DefaultHandleWidth,
					Background = brush,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch
				};
			}

			return new GridSplitter
			{
				// TODO: These options might have to be customized later
				Height = DefaultHandleHeight,
				Background = brush,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
		}

		public void Add(UIElement element) => Insert(element);

		public void Insert(UIElement? element, int index = -1)
		{
			if (index > 1)
			{
				// We insert a gridsplitter in between items, so if the user
				// gives us an index and the previous one is a gridsplitter
				// we want to insert before, so that the new item shifts its
				// splitter along. We assume there should never be two
				// consecutive splitters. If this is the case its a bug.
				var item = GetChild(index-1);
				if (item != null && item is GridSplitter)
					index--;
			}

			bool append = false;
			bool splitter = true;

			if (index == -1 || index > Children.Count)
				index = Children.Count;

			if (Children.Count != 0)
			{
				if (index >= Children.Count)
					append = true;

				if (index != 0 && Children.Count > 1 && index != Children.Count)
					index++;
			}
			else
			{
				append = true;
				splitter = false;
			}

			if (append)
			{
				if (splitter)
					InsertSplitter(index, 0);

				InsertDefinition(index, append);
				index = Children.Add(element);
			}
			else
			{
				if (splitter)
				{
					MoveChildren(index, 2);
					InsertSplitter(index, 1);
				}

				InsertDefinition(index, append);
				Children.Insert(index, element);
			}

			SetIndex(element, index);
		}

		private void MoveChildren(int from, int offset)
		{
			if (offset == 0)
				return;

			foreach (UIElement child in Children.Cast<UIElement>())
			{
				int pos = GetIndex(child);

				if (pos < from)
					continue;

				SetIndex(child, GetIndex(child)+offset);
			}
		}

		private void InsertDefinition(int index, bool append)
		{
			if (Aligned == Alignment.Vertical)
			{
				var newColumn = new ColumnDefinition();
				newColumn.Width = new GridLength(1.0, GridUnitType.Star);

				if (append)
					ColumnDefinitions.Add(newColumn);
				else
					ColumnDefinitions.Insert(index, newColumn);
			}
			else
			{
				var newRow = new RowDefinition();
				newRow.Height = new GridLength(1.0, GridUnitType.Star);

				if (append)
					RowDefinitions.Add(newRow);
				else
					RowDefinitions.Insert(index, newRow);
			}
		}

		public void DumpGrid()
		{
			Debug.Write("Dumping:\n");
			foreach (var child in Children)
				Debug.Write(child.ToString()+"\n");
			Debug.Write("\n");
		}

		protected void InsertSplitter(int index, int offset)
		{
			var gridSplitter = GetDefaultGridSplitter();
			if (Aligned == Alignment.Vertical)
			{
				var newSplitterColumn = new ColumnDefinition() { Width = new GridLength(DefaultHandleWidth + 1) };

				ColumnDefinitions.Insert(index, newSplitterColumn);
				Children.Insert(index, gridSplitter);
				SetIndex(gridSplitter, index+offset);
			}
			else
			{
				var newSplitterRow = new RowDefinition() { Height = new GridLength(DefaultHandleHeight + 1) };

				RowDefinitions.Insert(index, newSplitterRow);
				Children.Insert(index, gridSplitter);
				SetIndex(gridSplitter, index + offset);
			}
		}
	}
}
