using Bridge;

namespace ProductiveRage.Immutable
{
	[ObjectLiteral(ObjectCreateMode.Constructor)]
	public sealed class ClassName : NonBlankTrimmedString
	{
		public ClassName(string value) : base(value) { }
	}
}
