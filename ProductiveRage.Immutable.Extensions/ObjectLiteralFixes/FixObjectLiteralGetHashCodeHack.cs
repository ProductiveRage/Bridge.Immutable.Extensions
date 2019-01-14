using System;
using System.Reflection;
using Bridge;

namespace ProductiveRage.Immutable
{
	internal sealed class FixObjectLiteralGetHashCodeHack
	{
		private static readonly QuickLookup<CalculateHashCode> _cache = new QuickLookup<CalculateHashCode>();
		private delegate int CalculateHashCode(object o);

		/// <summary>
		/// This is a workaround for [ObjectLiteral] types because they don't currently (as of Jan 2019) always get the same treatment in terms of supporting a custom GetHashCode method that other
		/// types do. This overrides the internal Bridge.getHashCode method and adds additional checks for ObjectLiteral types to it. It will only work if Type metadata is included in the JavaScript.
		/// There is a forum post about some of the discrepancies in support between [ObjectLiteral] and other types at https://forums.bridge.net/forum/community/help/6001. Having this code here
		/// will mean that any projects that pulls in this library should find that [ObjectLiteral] types with a custom GetHashCode() method (such as NonBlankTrimmedString) work as expected.
		/// </summary>
		[Init]
		private static void ApplyWorkaround()
		{
			/*@
			if (Bridge.getHashCode.haveManipulatedMethodFlagName) {
				// Just in case this workaround code gets included in multiple places and executed multiple times, only intercept the method once and do nothing on any subsequent calls
				return;
			}
			*/
			Script.Write("Bridge.getHashCode = {0}", GetOverriddenBridgeGetHashCode(Script.Write<CalculateHashCode>("Bridge.getHashCode"))); ;
			//@ Bridge.getHashCode.haveManipulatedMethodFlagName = true;
		}

		private static CalculateHashCode GetOverriddenBridgeGetHashCode(CalculateHashCode bridgeGetHashCode)
		{
			return o =>
			{
				var customObjectLiteralMethodForFirstArgumentIfAny = TryToCustomObjectLiteralGetHashCodeMethod(o);
				if (customObjectLiteralMethodForFirstArgumentIfAny != null)
					return Script.Write<int>("{0}.apply({1}, [])", customObjectLiteralMethodForFirstArgumentIfAny, o);

				// Pass the full arguments array to bridgeGetHashCode in case the Bridge internals include any additional metadata arguments, other than just the o value
				return Script.Write<int>("{0}.apply(this, arguments)", bridgeGetHashCode);
			};
		}

		private static CalculateHashCode TryToCustomObjectLiteralGetHashCodeMethod(object o)
		{
			if (o == null)
				return null;

			if (!ObjectLiteralDetection.IsObjectLiteral(o, out var type, out var uniqueId))
				return null;

			if (_cache.Contains(uniqueId))
				return _cache.Get(uniqueId);

			CalculateHashCode customMethodToCallIfAny;
			var getHashCodeMethodInfo = type.GetMethod(nameof(GetHashCode), BindingFlags.Public | BindingFlags.Instance, new Type[0]);
			if (getHashCodeMethodInfo == null)
				customMethodToCallIfAny = null;
			else
			{
				var javaScriptGetHashCodeMethodName = Script.Write<string>("{0}.sn", getHashCodeMethodInfo);
				if (!Script.Write<bool>("{0}", javaScriptGetHashCodeMethodName))
					customMethodToCallIfAny = null;
				else
					customMethodToCallIfAny = Script.Write<CalculateHashCode>("{0}.prototype[{1}]", type, javaScriptGetHashCodeMethodName);
			}
			_cache.Set(uniqueId, customMethodToCallIfAny);
			return customMethodToCallIfAny;
		}
	}
}