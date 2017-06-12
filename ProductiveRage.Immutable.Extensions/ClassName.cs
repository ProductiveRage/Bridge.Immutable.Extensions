namespace ProductiveRage.Immutable
{
	public sealed class ClassName : NonBlankTrimmedString
	{
		public ClassName(string value) : base(value) { }

		// This constructor only exists as a workaround until https://github.com/bridgedotnet/Bridge.Newtonsoft.Json/issues/4 is resolved
		private ClassName() : this("TO-BE-DESERIALISED") { }
	}
}
