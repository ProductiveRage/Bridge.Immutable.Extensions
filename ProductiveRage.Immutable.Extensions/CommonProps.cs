using System;
using Bridge;

namespace ProductiveRage.Immutable
{
	[ObjectLiteral]
	public sealed class CommonProps<T>
	{
		/// <summary>
		/// Since this is an [ObjectLiteral] class, its constructor would never be executed so the input validation has been moved to the BuildCommonPropsObjectLiteral method in the
		/// non-generic class below which is now the only way to construct an instance of this class (so the constructor is private since BuildCommonPropsObjectLiteral directly writes
		/// out JavaScript to create an object literal with the required data). This class remains as an important part of the type system, despite its weird construction. We shouldn't
		/// mark is as [External] since then it won't be possible to use it as a type parameter (which it may be when using it as the props type for component classes).
		/// </summary>
		private CommonProps() { }

		public T State { get; }
		public Action<T> OnChange { get; }
		public Optional<ClassName> ClassName { get; }
		public bool Disabled { get;  }
		public Optional<Union<string, int>> Key { get; }
	}

	public static class CommonProps
	{
		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, Optional<ClassName>.Missing, false, Optional<Union<string, int>>.Missing);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Union<string, int> key)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, Optional<ClassName>.Missing, false, key);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, Optional<Union<string, int>>.Missing);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, string classNameIfAny, bool disabled)
		{
			var className = string.IsNullOrWhiteSpace(classNameIfAny) ? new ClassName(classNameIfAny) : Optional<ClassName>.Missing;
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, Optional<Union<string, int>>.Missing);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled, Union<string, int> key)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, key);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, string classNameIfAny, bool disabled, Union<string, int> key)
		{
			var className = string.IsNullOrWhiteSpace(classNameIfAny) ? new ClassName(classNameIfAny) : Optional<ClassName>.Missing;
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, key);
		}

		[IgnoreGeneric]
		private static CommonProps<T> BuildCommonPropsObjectLiteral<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled, Optional<Union<string, int>> key)
		{
			if (state == null)
				throw new ArgumentNullException("state");
			if (onChange == null)
				throw new ArgumentNullException("onChange");

			return Script.Write<CommonProps<T>>("{ state: state`, onChange: onChange, className: className, disabled: disabled, key: key }");
		}
	}
}