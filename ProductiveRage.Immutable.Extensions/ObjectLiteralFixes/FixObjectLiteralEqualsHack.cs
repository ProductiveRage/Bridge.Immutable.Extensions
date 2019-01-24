using System.Reflection;
using Bridge;

namespace ProductiveRage.Immutable
{
	internal sealed class FixObjectLiteralEqualsHack
	{
		private static readonly QuickLookup<AreEqual> _cache = new QuickLookup<AreEqual>();
		private delegate bool AreEqual(object x, object y);

		/// <summary>
		/// This is a workaround for [ObjectLiteral] types because they don't currently (as of Jan 2019) always get the same treatment in terms of supporting a custom Equals method that other
		/// types do. This overrides the internal Bridge.equals method and adds additional checks for ObjectLiteral types to it. It will only work if Type metadata is included in the JavaScript.
		/// There is a forum post about some of the discrepancies in support between [ObjectLiteral] and other types at https://forums.bridge.net/forum/community/help/6001. Having this code here
		/// will mean that any projects that pulls in this library should find that [ObjectLiteral] types with a custom Equals(..) method (such as NonBlankTrimmedString) work as expected.
		/// </summary>
		[Init]
		private static void ApplyWorkaround()
		{
			/*@
			if (Bridge.equals.haveManipulatedMethodFlagName) {
				// Just in case this workaround code gets included in multiple places and executed multiple times, only intercept the method once and do nothing on any subsequent calls
				return;
			}
			*/
			Script.Write("Bridge.equals = {0}", GetOverriddenBridgeEquals(Script.Write<AreEqual>("Bridge.equals")));;
			//@ Bridge.equals.haveManipulatedMethodFlagName = true;
		}

		private static AreEqual GetOverriddenBridgeEquals(AreEqual bridgeEquals)
		{
			return (x, y) =>
			{
				// There are some difficult cases to consider here - in the simplest interesting case, both objects are of the same [ObjectLiteral] type and that type has an Equals method and
				// we call it against the first object, passing a reference to the second. The next simplest interesting case is when one of the objects is null and the other is an [ObjectLiteral]
				// with an Equals method. But things start to get more complicated if the first object is a non-null [ObjectLiteral] and the second is a non-null non-[ObjectLiteral] - for example,
				// if x is a NonBlankTrimmedString ([ObjectLiteral]) and y is an Optional<NonBlankTrimmedString> (non-[ObjectLiteral]) then the NonBlankTrimmedString Equals will say that they don't
				// match (because their types are not the same) but the Optional<NonBlankTrimmedString> Equals method would say that they DO - and that should be allowed a chance to do so.
				var customObjectLiteralMethodForFirstArgumentIfAny = TryToCustomObjectLiteralEqualsMethod(x);
				if (customObjectLiteralMethodForFirstArgumentIfAny != null)
				{
					if (Script.Write<bool>("{0}.apply({1}, [{2}])", customObjectLiteralMethodForFirstArgumentIfAny, x, y))
						return true;
				}
				var customObjectLiteralMethodForSecondArgumentIfAny = TryToCustomObjectLiteralEqualsMethod(y);
				if (customObjectLiteralMethodForSecondArgumentIfAny != null)
				{
					if (Script.Write<bool>("{0}.apply({1}, [{2}])", customObjectLiteralMethodForSecondArgumentIfAny, y, x))
						return true;
				}

				// Pass the full arguments array to bridgeEquals in case the Bridge internals include any additional metadata arguments, other than just the x and y values
				return Script.Write<bool>("{0}.apply(this, arguments)", bridgeEquals);
			};
		}

		private static AreEqual TryToCustomObjectLiteralEqualsMethod(object o)
		{
			if (o == null)
				return null;

			if (!ObjectLiteralDetection.IsObjectLiteral(o, out var type, out var uniqueId))
				return null;

			if (_cache.Contains(uniqueId))
				return _cache.Get(uniqueId);

			AreEqual customMethodToCallIfAny;
			var equalsMethodInfo = type.GetMethod(nameof(Equals), BindingFlags.Public | BindingFlags.Instance, new[] { typeof(object) });
			if (equalsMethodInfo == null)
				customMethodToCallIfAny = null;
			else
			{
				var javaScriptEqualsMethodName = Script.Write<string>("{0}.sn", equalsMethodInfo);
				if (!Script.Write<bool>("{0}", javaScriptEqualsMethodName))
					customMethodToCallIfAny = null;
				else
					customMethodToCallIfAny = Script.Write<AreEqual>("{0}.prototype[{1}]", type, javaScriptEqualsMethodName);
			}
			_cache.Set(uniqueId, customMethodToCallIfAny);
			return customMethodToCallIfAny;
		}
	}
}