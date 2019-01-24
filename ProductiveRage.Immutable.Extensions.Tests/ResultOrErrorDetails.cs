using System;
using Bridge;

namespace ProductiveRage.Immutable.Extensions.Tests
{
	/// <summary>
	/// This is a variation on the ResultOrError class that is used in another project I maintain and some problems were encountered since making it [ObjectLiteral]
	/// and so I want to add some tests relating to it
	/// </summary>
	[ObjectLiteral(ObjectCreateMode.Constructor)]
	public sealed class ResultOrErrorDetails<T>
	{
		public static ResultOrErrorDetails<T> FromResult(T result)
		{
			if (result == null)
				throw new ArgumentNullException(nameof(result));
			return new ResultOrErrorDetails<T>(result, null);
		}
		public static ResultOrErrorDetails<T> FromError(IApiErrorDetails error)
		{
			if (error == null)
				throw new ArgumentNullException(nameof(error));
			return new ResultOrErrorDetails<T>(Optional<T>.Missing, Optional.For(error));
		}

		private readonly Optional<T> _result;
		private readonly Optional<IApiErrorDetails> _error;
		private ResultOrErrorDetails(Optional<T> result, Optional<IApiErrorDetails> error)
		{
			if ((result.IsDefined && error.IsDefined) || (!result.IsDefined && !error.IsDefined))
				throw new ArgumentException($"Precisely one of {nameof(result)} and {nameof(error)} must have a value");

			_result = result;
			_error = error;
		}

		public TResult Match<TResult>(Func<T, TResult> handleResult, Func<IApiErrorDetails, TResult> handleError)
		{
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			return _result.IsDefined ? handleResult(_result.Value) : handleError(_error.Value);
		}

		public void Match(Action<T> handleResult, Action<IApiErrorDetails> handleError)
		{
			if (handleResult == null)
				throw new ArgumentNullException(nameof(handleResult));
			if (handleError == null)
				throw new ArgumentNullException(nameof(handleError));

			if (_result.IsDefined)
				handleResult(_result.Value);
			else
				handleError(_error.Value);
		}

		public ResultOrErrorDetails<TDestination> Map<TDestination>(Func<T, TDestination> mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));

			if (!_result.IsDefined)
			{
				if (typeof(T) == typeof(TDestination))
				{
					//@ return this;
				}
				return ResultOrErrorDetails<TDestination>.FromError(_error.Value);
			}

			var newResult = mapper(_result.Value);
			if (newResult == null)
				throw new ArgumentException($"The provided {nameof(mapper)} returned null, which is not valid");

			if ((typeof(T) == typeof(TDestination)) && newResult.Equals(_result.Value))
			{
				//@ return this;
			}
			return new ResultOrErrorDetails<TDestination>(newResult, _error);
		}

		public override bool Equals(object o)
		{
			if ((o == null) || (o.GetType() != this.GetType()))
				return false;

			var otherResultOrErrorDetails = (ResultOrErrorDetails<T>)o;
			return (otherResultOrErrorDetails._result == _result) && (otherResultOrErrorDetails._error == _error);
		}

		public override int GetHashCode()
		{
			// Courtesy of https://stackoverflow.com/a/263416/3813189
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				hash = hash * 23 + _result.GetHashCode();
				hash = hash * 23 + _error.GetHashCode();
				return hash;
			}
		}

		public static bool operator ==(ResultOrErrorDetails<T> x, ResultOrErrorDetails<T> y)
		{
			if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
				return true;
			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			return x.Equals(y);
		}

		public static bool operator !=(ResultOrErrorDetails<T> x, ResultOrErrorDetails<T> y)
		{
			return !(x == y);
		}
	}

	public static class ResultOrErrorDetails
	{
		public static ResultOrErrorDetails<T> FromResult<T>(T result)
		{
			return ResultOrErrorDetails<T>.FromResult(result);
		}
	}

	public interface IApiErrorDetails
	{
		NonBlankTrimmedString Message { get; }
	}
}