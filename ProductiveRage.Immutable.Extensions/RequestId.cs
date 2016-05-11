using System;

namespace ProductiveRage.Immutable
{
	/// <summary>
	/// It is a common pattern in a Flux-like architecture for API calls to initiate work and for an action to be sent to the Dispatcher when
	/// that work has completed, as all other communications are handled (rather than using the Dispatcher for some events and other async
	/// mechanisms, such as Promises, for other communications). It is often important to be able to identify a particular work-completed
	/// action back to the where the API call was made (for example, if a form's content is to be saved then that form may be displayed in
	/// a disabled state until that particular save operation has completed) or for a particular work-completed action to be compared to
	/// another, to determine which is more recent (if there are multiple ways in which new data may arrive in an application, it is useful
	/// to be able to ignore stale data that arrives after more recent data). A new RequestId instance is guaranteed to be unique, while
	/// still being comparable to other instances to determine which was created more recently. The RequestId is useful as the return
	/// value for API calls, so the caller keeps the RequestId and then waits for an action to arrive that corresponds to it.
	/// </summary>
	public sealed class RequestId
	{
		private static DateTime _timeOfLastId = DateTime.MinValue;
		private static int _offsetOfLastId = 0;

		private readonly DateTime _requestTime;
		private readonly int _requestOffset;
		public RequestId()
		{
			_requestTime = DateTime.Now;
			if (_timeOfLastId < _requestTime)
			{
				_offsetOfLastId = 0;
				_timeOfLastId = _requestTime;
			}
			else
				_offsetOfLastId++;
			_requestOffset = _offsetOfLastId;
		}

		public bool ComesAfter(RequestId other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			if (_requestTime == other._requestTime)
				return _requestOffset > other._requestOffset;
			return (_requestTime > other._requestTime);
		}

		public bool IsEqualToOrComesAfter(Optional<RequestId> other)
		{
			// If the "other" reference is no-RequestId then the "source" may be considered to come after it
			if (!other.IsDefined)
				return true;

			return (other.Value == this) || this.ComesAfter(other.Value);
		}
	}
}