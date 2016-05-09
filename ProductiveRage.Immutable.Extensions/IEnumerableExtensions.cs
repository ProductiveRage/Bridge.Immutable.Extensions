using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductiveRage.Immutable.Extensions
{
	public static class IEnumerableExtensions
	{
		// I didn't include this in Bridge/ProductiveRage.Immutable because it's most efficient to start with an empty Set
		// and Insert items into the start of it - this is a very cheap operation. But this is very convenient and so it
		// makes sense to declare it somewhere.
		/// <summary>
		/// This will throw an exception for any null references (either the values set or any reference within that set)
		/// since the Set type will not accept any nulls (if it must potentially contain missing values then type T should
		/// be an Optional of some type)
		/// </summary>
		public static Set<T> ToSet<T>(this IEnumerable<T> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			return Set.Of(values.ToArray());
		}
	}
}
