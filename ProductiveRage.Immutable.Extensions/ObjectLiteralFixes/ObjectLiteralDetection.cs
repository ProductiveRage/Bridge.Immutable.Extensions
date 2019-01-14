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
					uniqueIdentifierIfObjectLiteral = Script.Write<string>("{0}.$getType.toString()", o);
					return true;
				}
			}

			uniqueIdentifierIfObjectLiteral = null;
			typeIfObjectLiteral = null;
			return false;
		}
	}
}