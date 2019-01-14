using Bridge;

namespace ProductiveRage.Immutable
{
	[Priority(1)] // Without this, the static field initialisers that use this type don't work (this seems to get defined too late in the compiled output)
	internal class QuickLookup<T>
	{
		private readonly object _data;
		public QuickLookup() => _data = new object();
		public bool Contains(string key) => Script.Write<bool>("{0}.hasOwnProperty({1})", _data, key);
		public T Get(string key) => Script.Write<T>("{0}[{1}]", _data, key);
		public void Set(string key, T value) => Script.Write("{0}[{1}] = {2}", _data, key, value);
	}
}