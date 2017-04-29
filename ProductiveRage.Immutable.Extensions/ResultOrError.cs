using System;
using Bridge;

namespace ProductiveRage.Immutable
{
	public static class ResultOrError
	{
		public static ResultOrError<T> FromResult<T>(T result)
		{
			return ResultOrError<T>.FromResult(result);
		}
	}

	/// <summary>
	/// This represents EITHER a result or an error message - never both and never neither. It does not directly expose the Result and Error properties since they encourages code based upon conditionals
	/// that are very easy to write such that they only handle one case or the other. Instead, this class has Match methods which must be provided delegates to deal with both the result AND the error cases.
	/// </summary>
	public sealed class ResultOrError<T>
	{
		public static ResultOrError<T> FromResult(T result)
		{
			if (result == null)
				throw new ArgumentNullException(nameof(result));
			return new ResultOrError<T>(result, null);
		}
		public static ResultOrError<T> FromError(NonBlankTrimmedString errorMessage)
		{
			if (errorMessage == null)
				throw new ArgumentNullException(nameof(errorMessage));
			return new ResultOrError<T>(Optional<T>.Missing, errorMessage);
		}

		private readonly Optional<T> _result;
		private readonly Optional<NonBlankTrimmedString> _errorMessage;
		private ResultOrError(Optional<T> result, Optional<NonBlankTrimmedString> errorMessage)
		{
			if ((result.IsDefined && errorMessage.IsDefined) || (!result.IsDefined && !errorMessage.IsDefined))
				throw new ArgumentException($"Precisely one of {nameof(result)} and {nameof(errorMessage)} must have a value");

			_result = result;
			_errorMessage = errorMessage;
		}

		public TResult Match<TResult>(Func<T, TResult> handleResult, Func<NonBlankTrimmedString, TResult> handleError)
		{
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			return _result.IsDefined ? handleResult(_result.Value) : handleError(_errorMessage.Value);
		}

		public void Match(Action<T> handleResult, Action<NonBlankTrimmedString> handleError)
		{
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (_result.IsDefined)
				handleResult(_result.Value);
			else
				handleError(_errorMessage.Value);
		}

		/// <summary>
		/// If this instance represents a Result then that Result will be transformed using the provided delegate, creating a new ResultOrError (potentially wrapping a different type) unless
		/// the new value is the same as the current value, in which case the current ResultOrError instance will be returned (there would be no need to create a new one). If this instance
		/// represents an Error then this will return a ResultOrError instance for the mapper's destination type, maintaining the same error information (unless the destination type is the
		/// same as the current type, in which case the current instance may be directly returned).
		/// </summary>
		public ResultOrError<TDestination> Map<TDestination>(Func<T, TDestination> mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));

			if (!_result.IsDefined)
			{
				if (typeof(T) == typeof(TDestination))
				{
					// If the destination type is the same as the current type then we don't need a new instance (but the C# compiler won't let us do a cast so we have to Script.Write it)
					return Script.Write<ResultOrError<TDestination>>("this");
				}
				return ResultOrError<TDestination>.FromError(_errorMessage.Value); // If there is no result then there must be an error
			}

			var newResult = mapper(_result.Value);
			if (newResult == null)
				throw new ArgumentException($"The provided {nameof(mapper)} returned null, which is not valid");

			if ((typeof(T) == typeof(TDestination)) && newResult.Equals(_result.Value))
			{
				// If the destination type is the same as the current type and the new value is the same as the current one then we don't need a new instance (but the C# compiler won't let us
				// do a cast so we have to Script.Write it)
				return Script.Write<ResultOrError<TDestination>>("this");
			}
			return new ResultOrError<TDestination>(newResult, _errorMessage);
		}
	}
}
