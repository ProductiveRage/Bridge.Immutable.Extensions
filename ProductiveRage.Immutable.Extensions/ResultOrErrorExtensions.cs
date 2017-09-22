using System;

namespace ProductiveRage.Immutable
{
	public static class ResultOrErrorExtensions
	{
		/// <summary>
		/// Given a ResultOrError of Optional T, return a ResultOrError of T by replacing a Missing T value with an Error ResultOrError of T
		/// </summary>
		public static ResultOrError<T> ReplaceMissingResultWithError<T>(this ResultOrError<Optional<T>> source, NonBlankTrimmedString errorMessageForMissingValue)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (errorMessageForMissingValue == null)
				throw new ArgumentNullException(nameof(errorMessageForMissingValue));

			return source.Match(
				handleResult: resultIfAvailable => resultIfAvailable
					.Map(result => ResultOrError.FromResult(result))
					.GetValueOrDefault(ResultOrError<T>.FromError(errorMessageForMissingValue)),
				handleError: errorMessage => ResultOrError<T>.FromError(errorMessage)
			);
		}
	}
}