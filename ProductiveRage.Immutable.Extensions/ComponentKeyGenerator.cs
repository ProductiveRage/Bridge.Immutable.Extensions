using System;
using Bridge;

namespace ProductiveRage.Immutable
{
	public static class ComponentKeyGenerator
	{
		private static DateTime _timeOfLastId = DateTime.MinValue;
		private static int _offsetOfLastId = 0;

		/// <summary>
		/// When declaring dynamic child components, each should one should have a consistent unique key. For some components, this is easy - eg. if rendering a list of
		/// messages that have been persisted on the server, each message is likely to have a unique id that may be used as the key. However, there are also times when
		/// a dynamic list of items may be created and managed where items may be added and removed (which prevents a simple incrementing numeric value to be used)
		/// where there is no "persistence id" to rely upon. This function will generate a new key that is guaranteed to be unique from any other keys that it
		/// has returned (so it is not safe to have a set of child components where some use keys from here and some use keys from elsewhere).
		/// </summary>
		public static Union<string, int> GetNew()
		{
			// This uses the same logic as the RequestId, but we can't use just re-use the RequestId's code since it doesn't expose its internal requestTime and
			// requestOffset values. Also, we may want to change this logic in the future.
			var requestTime = DateTime.Now;
			if (_timeOfLastId < requestTime)
			{
				_offsetOfLastId = 0;
				_timeOfLastId = requestTime;
			}
			else
				_offsetOfLastId++;

			return string.Format(
				"{0}-{1}",
				requestTime.GetTime(),
				_offsetOfLastId
			);
		}
	}
}