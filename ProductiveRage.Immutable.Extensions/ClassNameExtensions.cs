using System;

namespace ProductiveRage.Immutable
{
	public static class ClassNameExtensions
	{
		/// <summary>
		/// Combine a ClassName instance with an Optional ClassName instance, if the Optional ClassName has a value - otherwise the ClassName will be
		/// returned. If the Optional ClassName has a value then the two will be joined by a single space.
		/// </summary>
		public static ClassName Add(this ClassName source, Optional<ClassName> other)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Add(" ", other);
		}

		/// <summary>
		/// Combine a ClassName instance with an Optional ClassName instance, if the Optional ClassName has a value - otherwise the ClassName will be
		/// returned. If the Optional ClassName has a value then the two will be joined by a specified delimiter string (which may be blank if desired).
		/// </summary>
		public static ClassName Add(this ClassName source, string delimiter, Optional<ClassName> other)
		{
			if (delimiter == null)
				throw new ArgumentNullException("delimiter");

			if (!other.IsDefined)
				return source;

			return new ClassName(source.Value + delimiter + other.Value.Value);
		}

		/// <summary>
		/// Combine two ClassNames, joining their values with a single space
		/// </summary>
		public static ClassName Add(this ClassName source, ClassName other)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (other == null)
				throw new ArgumentNullException("other");

			return source.Add(" ", other);
		}

		/// <summary>
		/// Combine two ClassNames, joining their values with a specified delimiter string (which may be blank if desired)
		/// </summary>
		public static ClassName Add(this ClassName source, string delimiter, ClassName other)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (delimiter == null)
				throw new ArgumentNullException("delimiter");
			if (other == null)
				throw new ArgumentNullException("other");

			return new ClassName(source.Value + delimiter + other.Value);
		}
	}
}
