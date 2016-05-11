namespace ProductiveRage.Immutable
{
	public static class OptionalNonBlankTrimmedStringExtensions
	{
		/// <summary>
		/// Translate an Optional NonBlankTrimmedString instance into a string - returning null if there is no value. Nulls are usually not desirable since it's
		/// difficult for the type system to describe where a null is and isn't acceptable (which is what the Optional struct is intended to help with) but, if
		/// an Optional NonBlankTrimmedString is to be used as a class name of an element then it will need to be reduced to a string instance again at some
		/// point since React elements have a string ClassName property (which may be null, meaning set no class attribute).
		/// </summary>
		public static string ToNullableString<T>(this Optional<T> source) where T : NonBlankTrimmedString
		{
			return source.IsDefined ? source.Value.Value : null;
		}
	}
}
