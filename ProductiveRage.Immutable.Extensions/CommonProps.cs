using System;
using Bridge;
using Bridge.React;

namespace ProductiveRage.Immutable
{
	// 2018-02-20 DWR: This used to be an [ObjectLiteral] for eking out a small performance improvement but Bridge (16, I think) has meant that this won't work properly any more when used
	// as Component props class and so [ObjectLiteral] is being removed (I'll leave the constructor private since we don't need to change the interface)
	public sealed class CommonProps<T> : PropsWithKey
	{
		public CommonProps(Optional<ClassName> className, bool disabled, T model, Action<T> onChange, Union<int, string> key) : base(key)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));
			if (onChange == null)
				throw new ArgumentNullException(nameof(onChange));

			ClassName = className;
			Disabled = disabled;
			Model = model;
			OnChange = onChange;
		}

		public Optional<ClassName> ClassName { get; }
		public bool Disabled { get; }
		public T Model { get; }
		public Action<T> OnChange { get; }
		
		// 2017-04-28 DWR: I'd like to rename all of the "state" properties in the construction methods below but that could break existing code too (maybe one day in the future)
		[Obsolete("The 'State'property was confusingly-named since it did not relate to the React concept of State (where a Component has a Props reference and a State instance) and so it has been renamed to Model (as in View Model) - this property is an alias on to Model and exists for backwards compatibility but will be removed in a future version of the library")]
		public T State { get { return Model; } }
	}

	public static class CommonProps
	{
		// 2018-02-20 DWR: These used to use [IgnoreGeneric] for small performance improvements (which were less relevant after Bridge made changes to how they dealt with generic) but it's safer to remove them

		public static CommonProps<T> For<T>(T model, Action<T> onChange) { return new CommonProps<T>(null, false, model, onChange, null); }
		public static CommonProps<T> For<T>(T model, Action<T> onChange, Union<int, string> key) { return new CommonProps<T>(null, false, model, onChange, key); }
		public static CommonProps<T> For<T>(T model, Action<T> onChange, Optional<ClassName> className, bool disabled) { return new CommonProps<T>(className, disabled, model, onChange, null); }
		public static CommonProps<T> For<T>(T model, Action<T> onChange, string classNameIfAny, bool disabled) { return new CommonProps<T>(ToNonBlankTrimmedString(classNameIfAny), disabled, model, onChange, null); }
		public static CommonProps<T> For<T>(T model, Action<T> onChange, Optional<ClassName> className, bool disabled, Union<int, string> key) { return new CommonProps<T>(className, disabled, model, onChange, key); }
		public static CommonProps<T> For<T>(T model, Action<T> onChange, string classNameIfAny, bool disabled, Union<int, string> key) { return new CommonProps<T>(ToNonBlankTrimmedString(classNameIfAny), disabled, model, onChange, key); }

		private static Optional<ClassName> ToNonBlankTrimmedString(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : new ClassName(value);
		}
	}
}