using System;
using System.Linq;

namespace ProductiveRage.Immutable
{
	public static class SetExtensions
	{
		// These were not included in Bridge/ProductiveRage.Immutable for the same reason that ToSet wasn't - the more efficient
		// access methods are included there but these operations are common and so it's convenient for them to be available

		/// <summary>
		/// This will remove any items from the set that match the specified filter. If no items were matched then the initial set will be returned unaltered.
		/// </summary>
		public static Set<T> Remove<T>(this Set<T> source, Func<T, bool> filter)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (filter == null)
				throw new ArgumentNullException("filter");

			var newList = Set<T>.Empty;
			var identifiedItemToRemove = false;
			foreach (var existingItem in source.Reverse())
			{
				if (filter(existingItem))
				{
					identifiedItemToRemove = true;
					continue;
				}
				newList = newList.Insert(existingItem);
			}
			return identifiedItemToRemove ? newList : source;
		}

		/// <summary>
		/// This will replace any items from the set that match the specified filter, using an update that takes the matched item and transforms it into a new one
		/// If no items were matched then the initial set will be returned unaltered.
		/// </summary>
		public static Set<T> Update<T>(this Set<T> source, Func<T, bool> filter, Func<T, T> updater)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (filter == null)
				throw new ArgumentNullException("filter");
			if (updater == null)
				throw new ArgumentNullException("updater");

			var newList = Set<T>.Empty;
			var identifiedItemToUpdate = false;
			foreach (var existingItem in source.Reverse())
			{
				if (filter(existingItem))
				{
					identifiedItemToUpdate = true;
					newList = newList.Insert(updater(existingItem));
					continue;
				}
				newList = newList.Insert(existingItem);
			}
			return identifiedItemToUpdate ? newList : source;
		}
	}
}
