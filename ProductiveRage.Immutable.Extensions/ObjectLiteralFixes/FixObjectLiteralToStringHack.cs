using System;
using System.Reflection;
using Bridge;

namespace ProductiveRage.Immutable
{
	internal sealed class FixObjectLiteralToStringHack
	{
		private static readonly QuickLookup<GetString> _cache = new QuickLookup<GetString>();
		private delegate string GetString(object o);

		/// <summary>
		/// This is a workaround for [ObjectLiteral] types because they don't currently (as of Jan 2019) always get the same treatment in terms of supporting a custom ToString method that other
		/// types do. This overrides the internal Bridge.toString method and adds additional checks for ObjectLiteral types to it. It will only work if Type metadata is included in the JavaScript.
		/// There is a forum post about some of the discrepancies in support between [ObjectLiteral] and other types at https://forums.bridge.net/forum/community/help/6001. Having this code here
		/// will mean that any projects that pulls in this library should find that [ObjectLiteral] types with a custom ToString() method (such as NonBlankTrimmedString) work as expected.
		/// </summary>
		[Init]
		private static void ApplyWorkaround()
		{
			/*@
			if (Bridge.toString.haveManipulatedMethodFlagName) {
				// Just in case this workaround code gets included in multiple places and executed multiple times, only intercept the method once and do nothing on any subsequent calls
				return;
			}
			*/
			Script.Write("Bridge.toString = {0}", GetOverriddenBridgeToString(Script.Write<GetString>("Bridge.toString"))); ;
			//@ Bridge.toString.haveManipulatedMethodFlagName = true;
		}

		private static GetString GetOverriddenBridgeToString(GetString bridgeToString)
		{
			return o =>
			{
				var customObjectLiteralMethodForFirstArgumentIfAny = TryToCustomObjectLiteralToStringMethod(o);
				if (customObjectLiteralMethodForFirstArgumentIfAny != null)
					return Script.Write<string>("{0}.apply({1}, [])", customObjectLiteralMethodForFirstArgumentIfAny, o);

				// Pass the full arguments array to bridgeToString in case the Bridge internals include any additional metadata arguments, other than just the o value
				return Script.Write<string>("{0}.apply(this, arguments)", bridgeToString);
			};
		}

		private static GetString TryToCustomObjectLiteralToStringMethod(object o)
		{
			if (o == null)
				return null;

			if (!ObjectLiteralDetection.IsObjectLiteral(o, out var type, out var uniqueId))
				return null;

			if (_cache.Contains(uniqueId))
				return _cache.Get(uniqueId);

			GetString customMethodToCallIfAny;
			var toStringMethodInfo = type.GetMethod(nameof(ToString), BindingFlags.Public | BindingFlags.Instance, new Type[0]);
			if (toStringMethodInfo == null)
				customMethodToCallIfAny = null;
			else
			{
				var javaScriptToStringMethodName = Script.Write<string>("{0}.sn", toStringMethodInfo);
				if (!Script.Write<bool>("{0}", javaScriptToStringMethodName))
					customMethodToCallIfAny = null;
				else
					customMethodToCallIfAny = Script.Write<GetString>("{0}.prototype[{1}]", type, javaScriptToStringMethodName);
			}
			_cache.Set(uniqueId, customMethodToCallIfAny);
			return customMethodToCallIfAny;
		}
	}
}