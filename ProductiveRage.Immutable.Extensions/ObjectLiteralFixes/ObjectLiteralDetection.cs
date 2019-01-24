using System;
using Bridge;

namespace ProductiveRage.Immutable
{
	internal sealed class ObjectLiteralDetection
	{
		public static bool IsObjectLiteral(object o, out Type typeIfObjectLiteral, out string uniqueIdentifierIfObjectLiteral)
		{
			if (Script.Write<bool>("{0}", o))
			{
				typeIfObjectLiteral = o.GetType();
				if (Script.Write<bool>("{0}.$literal === true", typeIfObjectLiteral))
				{
					// 2018-01-24: Need this to include generic type params if considering a generic type so that the ID is unique across different generic variations
					// because the Equals implementation may capture different variables from one generic variation to another and so FixObjectLiteralEqualsHack could
					// get things wrong if we cached a single Equals lookup for a single generic type (without differentiating generic type param values). The same
					// also applies to FixObjectLiteralGetHashCodeHack and FixObjectLiteralToStringHack. Using Bridge.getTypeName and Bridge.getType does what is
					// required here.
					uniqueIdentifierIfObjectLiteral = Script.Write<string>("Bridge.getTypeName(Bridge.getType({0}))", o); // BRIDGE-INTERNAL
					return true;
				}
			}

			uniqueIdentifierIfObjectLiteral = null;
			typeIfObjectLiteral = null;
			return false;
		}
	}
}