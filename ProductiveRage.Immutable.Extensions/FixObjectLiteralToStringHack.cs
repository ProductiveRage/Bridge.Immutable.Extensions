using Bridge;

namespace ProductiveRage.Immutable
{
	internal sealed class FixObjectLiteralToStringHack
	{
		/// <summary>
		/// This is a workaround for [ObjectLiteral] types that don't get the same ToString() treatment as other types do. Hopefully Bridge will fix this properly, I've raised an issue about it:
		/// https://forums.bridge.net/forum/community/help/6001 - in the meantime, having this code here will mean that any projects that pulls in this library should find that [ObjectLiteral]
		/// types with custom ToString() methods (such as NonBlankTrimmedString) work correctly when referenced in string interpolation calls.
		/// </summary>
		[Init]
		private static void ApplyWorkaround()
		{
			/*@
			(function () {
				var bridgeToString = Bridge.toString;
				if (bridgeToString.haveManipulatedMethodFlagName) {
					// Just in case this workaround code gets included in multiple places and executed multiple times, only intercept the
					// method once and do nothing on any subsequent calls
					return;
				}
				Bridge.toString = function (value) {
					if (value) {
						var type = Bridge.getType(value);
						var identifiedAsObjectLiteral = type.$literal === true;
						if (identifiedAsObjectLiteral) {
							var toStringMethod = type.prototype.toString;
							if (!Bridge.referenceEquals(toStringMethod, Object.prototype.toString)) {
								return toStringMethod.apply(value);
							}
						}
					}
					return bridgeToString(value);
				};
				Bridge.toString.haveManipulatedMethodFlagName = true;
			}());
			*/
		}
	}
}