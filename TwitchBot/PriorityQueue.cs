using System;
using System.Collections.Generic;

namespace TwitchBot
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		public PriorityQueue()
		{
			_items = new List<T>();
		}

		private IList<T> _items;

		public int Count
		{
			get { return _items.Count; }
		}

		/// <summary>
		/// Insert the item into the priority queue.
		/// </summary>
		public void Add(T item)
		{
			int left = 0;
			int right = _items.Count - 1;
			while (left <= right)
			{
				// Average left and right in a way that doesn't allow overflow.
				int middle = left + (right - left) / 2;
				if (_items[middle].CompareTo(item) < 0)
				{
					left = middle + 1;
				}
				else
				{
					right = middle - 1;
				}
			}
			_items.Insert(right + 1, item);
		}

		/// <summary>
		/// Retrieve and remove the item with the highest priority.
		/// </summary>
		public T Remove()
		{
			T retval = _items[_items.Count - 1];
			_items.RemoveAt(_items.Count - 1);
			return retval;
		}
	}
}
