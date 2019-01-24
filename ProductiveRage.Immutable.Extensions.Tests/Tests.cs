using System.Collections.Generic;
using Bridge;
using Bridge.QUnit;

namespace ProductiveRage.Immutable.Extensions.Tests
{
	public static class Tests
	{
		[Ready]
		public static void Go()
		{
			// The NonBlankTrimmedString class was changed in December 2018 to be an [ObjectLiteral] because it improves deserialisation time for API responses that include
			// many string instance because instantiating [ObjectLiteral] types seems to be much faster than "full" types. However, the support in Bridge for [ObjectLiteral]
			// seems to be a little patchy (some of which is discussed in https://forums.bridge.net/forum/community/help/6001) and so I've added some hacks to this library
			// to improve the support. These tests exercise those hacks (and should help highlight any regressions in case the Bridge internals change in the future in a
			// way that invalidates any of the approachs in the hacks).
			ToStringTests();
			EqualsTests();
			DictionaryTests();
		}

		private static void ToStringTests()
		{
			QUnit.Module("ToString()");

			QUnit.Test("NonBlankTrimmedString ToString() call works as expected", assert => assert.Equal(new NonBlankTrimmedString("xyz").ToString(), "xyz"));
			QUnit.Test("NonBlankTrimmedString in string interpolation works as expected", assert => assert.Equal($"{new NonBlankTrimmedString("xyz")}", "xyz"));
			QUnit.Test("NonBlankTrimmedString in string concatenation works as expected", assert => assert.Equal("" + new NonBlankTrimmedString("xyz"), "xyz"));
			QUnit.Test("[ObjectLiteral]-as-generic-type-param's ToString() call works as expected", assert =>
			{
				// This is part of the second reproduce case in https://forums.bridge.net/forum/community/help/6001
				var x = new MultiLanguageTextBoxModel(new LangKey(1));
				assert.Equal(x.TestValue, "1");
			});
		}

		private static void EqualsTests()
		{
			QUnit.Module("Equals(..)");

			QUnit.Test("An instance of NonBlankTrimmedString is found to be equal to itself", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				AssertEqualsViaNonBlankTrimmedStringEqualsCall(assert, x, x, shouldBeEqual: true);
			});

			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are found to be equal when compared as NonBlankTrimmedString", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new NonBlankTrimmedString("xyz");
				AssertEqualsViaNonBlankTrimmedStringEqualsCall(assert, x, y, shouldBeEqual: true);
			});
			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are found to be equal when compared as generic type param of NonBlankTrimmedString", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new NonBlankTrimmedString("xyz");
				AssertEqualsViaSharedGenericTypeEqualsCall(assert, x, y, shouldBeEqual: true);
			});
			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are found to be equal when compared as Object", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new NonBlankTrimmedString("xyz");
				AssertEqualsViaObjectEqualsCall(assert, x, y, shouldBeEqual: true);
			});

			// There was a hack in ProductiveRage.Immutable to help Optional<T>'s Equals implementation work when T is an [ObjectLiteral] but that isn't required with the hacks in this library
			// (so, once I'm happy with this all, I'll remove them from ProductiveRage.Immutable, so 3.2.0 will have the hacks removed that were introduced in 3.1.0 - for now, the tests here
			// are referencing the 3.0.0 NuGet package, which doesn't have the hacks in it)
			QUnit.Test("Two instances of Optional<NonBlankTrimmedString> with the same value are found to be equal when compared as NonBlankTrimmedString", assert =>
			{
				var x = Optional.For(new NonBlankTrimmedString("xyz"));
				var y = Optional.For(new NonBlankTrimmedString("xyz"));
				AssertEqualsViaOptionalNonBlankTrimmedStringEqualsCall(assert, x, y, shouldBeEqual: true);
			});
			QUnit.Test("Two instances of Optional<NonBlankTrimmedString> with the same value are found to be equal when compared as generic type param of NonBlankTrimmedString", assert =>
			{
				var x = Optional.For(new NonBlankTrimmedString("xyz"));
				var y = Optional.For(new NonBlankTrimmedString("xyz"));
				AssertEqualsViaSharedGenericTypeEqualsCall(assert, x, y, shouldBeEqual: true);
			});
			QUnit.Test("Two instances of Optional<NonBlankTrimmedString> with the same value are found to be equal when compared as Object", assert =>
			{
				var x = Optional.For(new NonBlankTrimmedString("xyz"));
				var y = Optional.For(new NonBlankTrimmedString("xyz"));
				AssertEqualsViaObjectEqualsCall(assert, x, y, shouldBeEqual: true);
			});

			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are NOT equal if they are of different types when compared as NonBlankTrimmedString", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new ClassName("xyz");
				AssertEqualsViaNonBlankTrimmedStringEqualsCall(assert, x, y, shouldBeEqual: false);
			});
			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are NOT equal if they are of different types when compared as generic type param of NonBlankTrimmedString", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new ClassName("xyz");
				AssertEqualsViaSharedGenericTypeEqualsCall(assert, x, y, shouldBeEqual: false);
			});
			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are NOT equal if they are of different types when compared as Object", assert =>
			{
				var x = new NonBlankTrimmedString("xyz");
				var y = new ClassName("xyz");
				AssertEqualsViaObjectEqualsCall(assert, x, y, shouldBeEqual: false);
			});

			// This tests a few issues that have been encountered after using [ObjectLiteral] in more places - the FixObjectLiteralEqualsHack approach means that the implicit operator will
			// not reliably be called when comparing an Optional<T> to T (the fix for that is in 4.1.0 of ProductiveRage.Immutable) and FixObjectLiteralEqualsHack now supports "falling
			// through" equality checks if false is returned (so is comparing a NonBlankTrimmedString, which is an [ObjectLiteral], to an Optional<NonBlankTrimmedString> then the Equals
			// check made against the NonBlankTrimmedString's Equals method will return false as it is unaware of Optional's implicit casting support and so other Equals methods need to
			// be give opportunity to run) and it tests a bug with FixObjectLiteralEqualsHack relating to different generic types where the cached Equals lookup for an [ObjectLiteral]
			// generic class would result in type checks against the generic type parameters being incorrect.
			QUnit.Test("Integration tests around NonBlankTrimmedString / Optional / another [ObjectLiteral]", assert =>
			{
				var stringValue = new NonBlankTrimmedString("test");
				var optionalOfString = Optional.For(stringValue);
				var resultOrErrorOfString = ResultOrErrorDetails.FromResult(stringValue);

				assert.NotOk(Equals(optionalOfString, resultOrErrorOfString));
				assert.Ok(Equals(optionalOfString, stringValue));
				assert.Ok(Equals(stringValue, optionalOfString));
				assert.Ok(Equals(ResultOrErrorDetails.FromResult("abc"), ResultOrErrorDetails.FromResult("abc")));
				assert.Ok(Equals(ResultOrErrorDetails.FromResult(new NonBlankTrimmedString("abc")), ResultOrErrorDetails.FromResult(new NonBlankTrimmedString("abc"))));
				assert.NotOk(Equals(ResultOrErrorDetails.FromResult(new NonBlankTrimmedString("abc")), ResultOrErrorDetails.FromResult("abc")));
			});
		}

		private static void DictionaryTests()
		{
			QUnit.Module("Dictionary lookups");

			QUnit.Test("Two instances of NonBlankTrimmedString with the same value are found to be equal when used as a Dictionary key", assert =>
			{
				// This will test both the bridge.getHashCode AND the bridge.equals hacks
				var d = new Dictionary<NonBlankTrimmedString, int>
				{
					{ new NonBlankTrimmedString("xyz"), 123 }
				};
				assert.Ok(d.ContainsKey(new NonBlankTrimmedString("xyz")));
			});
		}

		private static void AssertEqualsViaObjectEqualsCall(Assert assert, object x, object y, bool shouldBeEqual)
		{
			if ((x == null) && (y == null))
				return;

			assert.Ok((x != null) && (y != null), "Unless both x and y are null, they must both NOT be null in order to be equal");
			var result = x.Equals(y);
			if (shouldBeEqual)
				assert.Ok(x.Equals(y));
			else
				assert.NotOk(x.Equals(y));
		}

		private static void AssertEqualsViaNonBlankTrimmedStringEqualsCall(Assert assert, NonBlankTrimmedString x, NonBlankTrimmedString y, bool shouldBeEqual)
		{
			if ((x == null) && (y == null))
				return;

			assert.Ok((x != null) && (y != null), "Unless both x and y are null, they must both NOT be null in order to be equal");
			var result = x.Equals(y);
			if (shouldBeEqual)
				assert.Ok(x.Equals(y));
			else
				assert.NotOk(x.Equals(y));
		}
		private static void AssertEqualsViaOptionalNonBlankTrimmedStringEqualsCall(Assert assert, Optional<NonBlankTrimmedString> x, Optional<NonBlankTrimmedString> y, bool shouldBeEqual)
		{
			if ((x == null) && (y == null))
				return;

			assert.Ok((x != null) && (y != null), "Unless both x and y are null, they must both NOT be null in order to be equal");
			var result = x.Equals(y);
			if (shouldBeEqual)
				assert.Ok(x.Equals(y));
			else
				assert.NotOk(x.Equals(y));
		}

		private static void AssertEqualsViaSharedGenericTypeEqualsCall<T>(Assert assert, T x, T y, bool shouldBeEqual)
		{
			if ((x == null) && (y == null))
				return;

			assert.Ok((x != null) && (y != null), "Unless both x and y are null, they must both NOT be null in order to be equal");
			var result = x.Equals(y);
			if (shouldBeEqual)
				assert.Ok(x.Equals(y));
			else
				assert.NotOk(x.Equals(y));
		}

		// This class is part of the second reproduce case in https://forums.bridge.net/forum/community/help/6001
		[ObjectLiteral(ObjectCreateMode.Constructor)]
		public struct LangKey
		{
			public LangKey(int value) { Value = value; }
			public int Value { get; }
			public override string ToString() => Value.ToString();
		}
		
		// This class is part of the second reproduce case in https://forums.bridge.net/forum/community/help/6001
		public sealed class MultiLanguageTextBoxModel : AbstractMultiLanguageTextBoxModel<LangKey>
		{
			public MultiLanguageTextBoxModel(LangKey selected) : base(selected) { }
		}

		// This class is part of the second reproduce case in https://forums.bridge.net/forum/community/help/6001
		public abstract class AbstractMultiLanguageTextBoxModel<TKey>
		{
			public AbstractMultiLanguageTextBoxModel(TKey selected)
			{
				Selected = selected;
				TestValue = Selected.ToString();
			}
			public TKey Selected { get; }
			public string TestValue { get; }
		}
	}
}