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

		public T State { get; private set; }
		public Action<T> OnChange { get; private set; }
		public Optional<ClassName> ClassName { get; private set; }
		public bool Disabled { get; private set; }

		// A Key property may be set by the CommonProps initialisation below but there is no need to expose that information since it is for React's benefit only (so there is no "Key"
		// property here)
	}

	public static class CommonProps
	{
		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, Optional<ClassName>.Missing, false, null);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Any<string, int> key)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, Optional<ClassName>.Missing, false, key);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, null);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, string classNameIfAny, bool disabled)
		{
			var className = string.IsNullOrWhiteSpace(classNameIfAny) ? Optional<ClassName>.Missing : new ClassName(classNameIfAny);
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, null);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled, Any<string, int> key)
		{
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, key);
		}

		[IgnoreGeneric]
		public static CommonProps<T> For<T>(T state, Action<T> onChange, string classNameIfAny, bool disabled, Any<string, int> key)
		{
			var className = string.IsNullOrWhiteSpace(classNameIfAny) ? Optional<ClassName>.Missing : new ClassName(classNameIfAny);
			return BuildCommonPropsObjectLiteral<T>(state, onChange, className, disabled, key);
		}

		[IgnoreGeneric]
		private static CommonProps<T> BuildCommonPropsObjectLiteral<T>(T state, Action<T> onChange, Optional<ClassName> className, bool disabled, Any<string, int> key)
		{
			if (state == null)
				throw new ArgumentNullException("state");
			if (onChange == null)
				throw new ArgumentNullException("onChange");

			// Note: The key argument may be null but this is one of the places that we don't want to model that using Optional<T> since we want to be sure that it
			// is actually stored as null in the CommonProps object, so that React doesn't try to interpret a Missing Optional value as the key to use
			return Script.Write<CommonProps<T>>("{ state: state, onChange: onChange, className: className, disabled: disabled, key: key }");
		}
	}
}