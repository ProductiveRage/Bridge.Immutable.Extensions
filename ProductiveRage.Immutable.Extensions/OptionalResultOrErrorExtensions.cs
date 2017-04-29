using System;
using Bridge;
using Bridge.React;

namespace ProductiveRage.Immutable
{
	public static class OptionalResultOrErrorExtensions
	{
		/// <summary>
		/// It is common to want to match an Optional Result-or-Errort to one of three ReactElements (a loading state, an error state and a result state) but if all three delegates are specified as anonymous
		/// lambdas and they all return custom components (that are derived from Bridge.React's Component, PureComponent or StatelessComponent) then the compiler will not be able to infer a common base type
		/// for them and so type inference on the two-type-argument Match method will fail. Having this method signature gives the compiler a big enough clue that it can work out that common case.
		/// </summary>
		public static ReactElement Match<T>(this Optional<ResultOrError<T>> source, ReactElement handleNoValue, Func<T, ReactElement> handleResult, Func<NonBlankTrimmedString, ReactElement> handleError)
		{
			if (handleNoValue == null)
				throw new ArgumentNullException(nameof(handleNoValue));
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (!source.IsDefined)
				return handleNoValue;

			return source.Value.Match(handleResult, handleError);
		}

		public static TResult Match<T, TResult>(this Optional<ResultOrError<T>> source, TResult handleNoValue, Func<T, TResult> handleResult, Func<NonBlankTrimmedString, TResult> handleError)
		{
			if (handleNoValue == null)
				throw new ArgumentNullException(nameof(handleNoValue));
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (!source.IsDefined)
				return handleNoValue;

			return source.Value.Match(handleResult, handleError);
		}

		public static TResult Match<T, TResult>(this Optional<ResultOrError<T>> source, Func<TResult> handleNoValue, Func<T, TResult> handleResult, Func<NonBlankTrimmedString, TResult> handleError)
		{
			if (handleNoValue == null)
				throw new ArgumentNullException(nameof(handleNoValue));
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (!source.IsDefined)
				return handleNoValue();

			return source.Value.Match(handleResult, handleError);
		}

		public static void Match<T>(this Optional<ResultOrError<T>> source, Action handleNoValue, Action<T> handleResult, Action<NonBlankTrimmedString> handleError)
		{
			if (handleNoValue == null)
				throw new ArgumentNullException(nameof(handleNoValue));
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (!source.IsDefined)
				handleNoValue();
			else
				source.Value.Match(handleResult, handleError);
		}

		/// <summary>
		/// If this instance represents a Result then that Result will be transformed using the provided delegate, creating a new ResultOrError (potentially wrapping a different type) unless
		/// the new value is the same as the current value, in which case the current ResultOrError instance will be returned (there would be no need to create a new one). If this instance
		/// represents an Error then this will return a ResultOrError instance for the mapper's destination type, maintaining the same error information (unless the destination type is the
		/// same as the current type, in which case the current instance may be directly returned). If this is a missing Optional value then a missing Optional for the destination type will
		/// be returned.
		/// </summary>
		public static Optional<ResultOrError<TDestination>> Map<T, TDestination>(this Optional<ResultOrError<T>> source, Func<T, TDestination> mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));

			if (!source.IsDefined)
			{
				// If the destination type is the same as the current type then we don't need a new instance (but the C# compiler won't let us do a cast so we have to Script.Write it)
				if (typeof(T) == typeof(TDestination))
					return Script.Write<Optional<ResultOrError<TDestination>>>("source");
				return Optional<ResultOrError<TDestination>>.Missing;
			}

			return source.Value.Map(mapper);
		}

		/// <summary>
		/// To deal correctly with all of the no-value, error, result states it is strongly recommended that the Match methods be used but there are some times where you just want to be
		/// able to say give me the if you have it (and Missing if not). On those occasions, this method may be used.
		/// </summary>
		public static Optional<T> TryToGetResult<T>(this Optional<ResultOrError<T>> value)
		{
			return value.Match(
				handleNoValue: Optional<T>.Missing,
				handleError: error => Optional<T>.Missing,
				handleResult: result => Optional.For(result) // Can't rely upon implicit cast due to https://forums.bridge.net/forum/bridge-net-pro/bugs/4049)
			);
		}
	}
}
