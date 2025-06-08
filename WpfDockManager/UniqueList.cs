using System.Windows;
using WpfDockManager.Layout;

namespace WpfDockManager
{
	public interface ITypeConverter<T, U>
	{
		public T? Convert(U? value);
		public U? Convert(T? value);
	}

	/// <summary>
	/// Maintains a list of unique elements. Null values are rejected.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	//	public class UniqueList<T> where T : new()
	public class UniqueList<T>
	{
		private List<T> _items = new List<T>();

		public int Count { get { return _items.Count; } }

		public T? Find(T? element)
		{
			if (element == null)
				return default(T);

			var i = IndexOf(element);
			if (i >= 0)
				return _items[i];

			return default(T);
		}

		public bool Contains(T item)
		{
			return _items.Count != 0 && IndexOf(item) >= 0;
		}

		private static bool IsCompatibleObject(object? value)
		{
			return (value is T) || (value == null && default(T) == null);
		}

		public int IndexOf(T? item)
		{
			if (item == null)
				return -1;

			if (IsCompatibleObject(item))
				return _items.IndexOf((T)item!);

			return -1;
		}

		public T? this[T? element]
		{
			get { return Find(element); }
			set { Add(element); }
		}

		public T? this[int index]
		{
			get { return _items[index]; }
		}

		public T? Add(T? element)
		{
			if (element == null)
				return default(T);

			var index = IndexOf(element);
			if (index >= 0)
				return _items[index];

			_items.Add(element);
			return element;
		}

		public void Remove(T? element)
		{
			T? layoutItem = Find(element);
			if (layoutItem == null)
				return;

			_items.Remove(layoutItem!);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		public static UniqueList<T>? operator +(UniqueList<T> list, T? element)
		{
			list.Add(element);
			return list;
		}

		public static UniqueList<T>? operator -(UniqueList<T> list, T? element)
		{
			list.Remove(element);
			return list;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}

	public class UniqueList<T, U> : UniqueList<T>
		where T : ITypeConverter<T, U>, new()
	{
		private static T? Converter(U? element) => new T().Convert(element);

		public T? Add(U? element) => Add(Converter(element));

		public void Remove(U? element) => Remove(Converter(element));

		public static UniqueList<T, U> operator +(UniqueList<T, U> list, U? element)
		{
			list.Add(Converter(element));
			return list;
		}

		public static UniqueList<T, U>? operator -(UniqueList<T, U> list, U? element)
		{
			list.Remove(Converter(element));
			return list;
		}
	}
}
