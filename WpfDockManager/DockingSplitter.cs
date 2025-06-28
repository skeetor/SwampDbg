using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDockingManager
{
	/// <summary>
	/// The DockingSplitter acts similar to a StackPanel, only it inserts drag handles
	/// in between items, so the items can be resized. Because of this it is derived
	/// from Grid instead of StackPanel because Grid supports Splitters natively
	/// while StackPanel does not.
	/// </summary>
	public class DockingSplitter : Grid
	{
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
			else
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

		protected GridSplitter CreateDefaultGridSplitter()
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
					MoveChildGridPosition(index, 2);
					InsertSplitter(index, 1);
				}

				InsertDefinition(index, append);
				Children.Insert(index, element);
			}

			SetIndex(element, index);
		}

		/// <summary>
		/// When a child has been inserted or removed all childs after this have to
		/// be adjusted accordingly so they are in the correct row/column again.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="offset"></param>
		private void MoveChildGridPosition(int from, int offset)
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

		protected void InsertSplitter(int index, int offset)
		{
			var gridSplitter = CreateDefaultGridSplitter();
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

		//private void SaveSplitterPosition()
		//{
		//	Properties.Settings.Default.Column1Width = myGrid.ColumnDefinitions[0].Width.Value;
		//	Properties.Settings.Default.Save();
		//}

		//private void RestoreSplitterPosition()
		//{
		//	if (Properties.Settings.Default.Column1Width > 0)
		//	{
		//		myGrid.ColumnDefinitions[0].Width = new GridLength(Properties.Settings.Default.Column1Width);
		//	}
		//}

		public int Replace(UIElement oldChild, UIElement newChild)
		{
			int index = GetIndex(oldChild);
			if (index == -1)
				throw new InvalidOperationException("'oldChild' is not an element of this DockingSplitter");

			Children.Insert(index, newChild);
			if (Aligned == Alignment.Vertical)
				SetColumn(newChild, index);
			else
				SetRow(newChild, index);

			Children.Remove(oldChild);

			return index;
		}

		public void SetLength(int index, GridLength length)
		{
			if (index == -1)
				index = Children.Count - 1;

			if (index < 0 || index >= Children.Count)
				throw new IndexOutOfRangeException("Index " + index.ToString() + "/" + Children.Count.ToString());

			if (Aligned == Alignment.Vertical)
				ColumnDefinitions[index].Width = length;
			else
				RowDefinitions[index].Height = length;
		}

		public GridLength GetLength(int index)
		{
			if (index == -1)
				index = Children.Count - 1;

			if (index < 0 || index >= Children.Count)
				throw new IndexOutOfRangeException("Index "+index.ToString()+"/"+ Children.Count.ToString());

			if (Aligned == Alignment.Vertical)
				return ColumnDefinitions[index].Width;

			return RowDefinitions[index].Height;
		}

		public int Remove(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);

			int index = GetIndex(element);
			if (index == -1)
				throw new InvalidOperationException("Element is not a member of this DockingSplitter");

			RemoveItem(index);

			return index;
		}

		public UIElement? RemoveAt(int index)
		{
			var child = GetChild(index);
			if (child is GridSplitter)
				return null;

			RemoveItem(index);

			return child;
		}

		/// <summary>
		/// Removes the item with the specified index and the splitter before it.
		/// Returns true if a splitter was also removed.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected bool RemoveItem(int index)
		{
			Children.RemoveAt(index);

			if (Aligned == Alignment.Vertical)
				ColumnDefinitions.RemoveAt(index);
			else
				RowDefinitions.RemoveAt(index);

			MoveChildGridPosition(index, -1);

			if (Children.Count == 0)
				return false;

			if (index >= Children.Count)
				index = Children.Count-1;

			if (GetChild(index) is not GridSplitter)
				return false;

			Children.RemoveAt(index);
			if (Aligned == Alignment.Vertical)
				ColumnDefinitions.RemoveAt(index);
			else
				RowDefinitions.RemoveAt(index);

			MoveChildGridPosition(index, -1);

			return true;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			// After SetLength was called, the grid is updated, so we check if there
			// are any columns with a Star in between and update them to their calculated
			// length. The last item will always get the Star setting, so it fills the whole
			// client area. If this is not done, we will end up with splitters moving the
			// left and right columns, which we dont want.
			// I found no better way to achieve this, because there is no way to reposition
			// a splitter by code. :(
			var rc = base.ArrangeOverride(arrangeSize);
			bool update = false;

			for (int i = 0; i < ColumnDefinitions.Count; i++)
			{
				// Here the items are already set to their required length as
				// determined by the splitters, so can replace any Star in between
				// with this length to prevent the splitter from dragging both
				// sides.
				if (i >= Children.Count)
					break;

				var w = (Children[i] as FrameworkElement)!.ActualWidth;
				var col = ColumnDefinitions[i].Width;

				if (col.IsStar && i != Children.Count-1)
				{
					col = new GridLength(w);
					ColumnDefinitions[i].Width = col;
					update = true;
				}
			}

			// The last item in the grid should always take up the remaining area.
			if (update)
			{
				var index = ColumnDefinitions.Count - 1;
				ColumnDefinitions[index].Width = new GridLength(1, GridUnitType.Star);
			}

			return rc;
		}

		public void DumpGrid()
		{
			Debug.WriteLine("GridDump");
			if (Aligned == Alignment.Vertical)
			{
				foreach (var d in ColumnDefinitions)
					Debug.WriteLine("Length (C): " + d.Width.Value.ToString());
			}
			else
			{
				foreach (var d in RowDefinitions)
					Debug.WriteLine("Length (R): " + d.Height.Value.ToString());
			}
		}
	}
}
