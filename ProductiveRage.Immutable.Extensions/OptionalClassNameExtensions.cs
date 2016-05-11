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
	}
}
