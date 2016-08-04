using System;

namespace ProductiveRage.Immutable
{
	public static class OptionalClassNameExtensions
	{
		/// <summary>
		/// Combine two Optional ClassName instances if both arguments have values, separating them with a single space. If only one argument has a value
		/// then that argument will be returned. If neither argument have a value then a missing value will be returned.
		/// </summary>
		public static Optional<ClassName> Add(this Optional<ClassName> source, Optional<ClassName> other)
		{
			return source.Add(" ", other);
		}

		/// <summary>
		/// Combine two Optional ClassName instances if both arguments have values, separating them with a specified delimiter string (which may be blank
		/// if desired). If only one argument has a value then that argument will be returned. If neither argument have a value then a missing value will
		/// be returned.
		/// </summary>
		public static Optional<ClassName> Add(this Optional<ClassName> source, string delimiter, Optional<ClassName> other)
		{
			if (delimiter == null)
				throw new ArgumentNullException("delimiter");

			if (!source.IsDefined && !other.IsDefined)
				return Optional<ClassName>.Missing;
			else if (!source.IsDefined)
				return other;
			else if (!other.IsDefined)
				return source;

			return new ClassName(source.Value.Value + delimiter + other.Value.Value);
		}

		/// <summary>
		/// Combine an Optional ClassName instance with a non-Optional ClassName - if the first instance has a value then the two will be combined, separated
		/// by a single space. If the source Optional ClassName has no value then the non Optional ClassName will be returned.
		/// </summary>
		public static ClassName Add(this Optional<ClassName> source, ClassName other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			return source.Add(" ", other);
		}

		/// <summary>
		/// Combine an Optional ClassName instance with a non-Optional ClassName - if the first instance has a value then the two will be combined, separated
		/// by a specified delimiter string (which may be blank if desired). If the source Optional ClassName has no value then the non Optional ClassName
		/// will be returned.
		/// </summary>
		public static ClassName Add(this Optional<ClassName> source, string delimiter, ClassName other)
		{
			if (delimiter == null)
				throw new ArgumentNullException("delimiter");
			if (other == null)
				throw new ArgumentNullException("other");

			if (!source.IsDefined)
				return other;

			return new ClassName(source.Value.Value + delimiter + other.Value);
		}
	}
}
