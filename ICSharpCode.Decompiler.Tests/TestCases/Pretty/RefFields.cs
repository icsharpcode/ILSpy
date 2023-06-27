using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LifetimeTests
	{
		private static int staticField;

		public Span<int> CreateWithoutCapture(scoped ref int value)
		{
			// Okay: value is not captured
			return new Span<int>(ref staticField);
		}

		public Span<int> CreateAndCapture(ref int value)
		{
			// Okay: value Rule 3 specifies that the safe-to-escape be limited to the ref-safe-to-escape
			// of the ref argument. That is the *calling method* for value hence this is not allowed.
			return new Span<int>(ref value);
		}

		public Span<int> ScopedRefSpan(scoped ref Span<int> span)
		{
			return span;
		}

		public Span<int> ScopedSpan(scoped Span<int> span)
		{
			return default(Span<int>);
		}

		public void OutSpan(out Span<int> span)
		{
			span = default(Span<int>);
		}

		public void Calls()
		{
			int value = 0;
			Span<int> span = CreateWithoutCapture(ref value);
			//span = CreateAndCapture(ref value); -- would need scoped local, not yet implemented
			span = ScopedRefSpan(ref span);
			span = ScopedSpan(span);
			OutSpan(out span);
		}
	}

	internal ref struct RefFields
	{
		public ref int Field0;
		public ref readonly int Field1;
		public readonly ref int Field2;
		public readonly ref readonly int Field3;

		public int PropertyAccessingRefFieldByValue {
			get {
				return Field0;
			}
			set {
				Field0 = value;
			}
		}

		public ref int PropertyReturningRefFieldByReference => ref Field0;

		public void Uses(int[] array)
		{
			Field1 = ref array[0];
			Field2 = array[0];
		}

		public void ReadonlyLocal()
		{
			ref readonly int field = ref Field1;
			Console.WriteLine("No inlining");
			field.ToString();
		}

		public RefFields(ref int v)
		{
			Field0 = ref v;
			Field1 = ref v;
			Field2 = ref v;
			Field3 = ref v;
		}
	}
}
