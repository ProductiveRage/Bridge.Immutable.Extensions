using System;
using Bridge;
using Bridge.React;

namespace ProductiveRage.Immutable
{
	// While I think that 99% of classes should be either abstract or sealed, this has value in not being sealed it is likely to be a common base for other
	// types that may want to effectively be a NonBlankTrimmedString-with-some-extra-validation (or just a specialisation of NonBlankTrimmedString to aid
	// API documentation). This makes some things more complicated, like the Equals implementation (if the type was sealed then the type comparisons
	// would not be required).
	public class NonBlankTrimmedString
	{
		public NonBlankTrimmedString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Null, blank or whitespace-only value specified");

			Value = value.Trim();
		}

		/// <summary>
		/// This will never be null, blank or have any leading or trailing whitespace
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument
		/// that requires a string
		/// </summary>
		public static implicit operator string(NonBlankTrimmedString value)
		{
			// A null NonBlankTrimmedString should be mapped onto a null string. As an example, if props.ClassName is Optional<NonBlankTrimmedString> then
			// the below would result in a call to this implicit operator for a null value, which would error here unless we support null -
			//   string x = props.ClassName.IsDefined ? props.ClassName.Value : null;
			if (value == null)
				return null;
			return value.Value;
		}

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument
		/// that requires a ReactElement-or-string, such as for the children array of the React
		/// DOM component factories
		/// </summary>
		public static implicit operator Union<ReactElement, string>(NonBlankTrimmedString value)
		{
			// Support null input for the same sort of reason as in the to-string operator above
			if (value == null)
				return null;

			var text = value.Value;
			return Script.Write<Union<ReactElement, string>>("text");
		}

		public static bool operator ==(NonBlankTrimmedString x, NonBlankTrimmedString y)
		{
			var xIsNull = Object.ReferenceEquals(x, null);
			var yIsNull = Object.ReferenceEquals(y, null);
			if (xIsNull && yIsNull)
				return true;
			else if (xIsNull || yIsNull)
				return false;
			else
				return x.Equals(y);
		}

		public static bool operator !=(NonBlankTrimmedString x, NonBlankTrimmedString y)
		{
			return !(x == y);
		}

		public override bool Equals(object obj)
		{
			if ((obj == null) || (obj.GetType() != this.GetType()))
				return false;
			return ((NonBlankTrimmedString)obj).Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
