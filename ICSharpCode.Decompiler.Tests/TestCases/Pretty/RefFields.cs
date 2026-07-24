using System;
using System.Diagnostics.CodeAnalysis;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal struct Buffer
	{
		private int value;

		[UnscopedRef]
		public ref int GetRef()
		{
			return ref value;
		}
	}

	internal ref struct Holder
	{
		public Span<int> Inner;

		public void Set(Span<int> inner)
		{
			Inner = inner;
		}

		public readonly void ReadonlyUse(Span<int> inner)
		{
			_ = inner.Length;
		}
	}

	internal static class HolderExtensions
	{
		public static void SetExtension(this ref Holder holder, Span<int> inner)
		{
			holder.Inner = inner;
		}

		public static void ReadonlyExtension(this in Holder holder, Span<int> inner)
		{
			_ = inner.Length;
		}
	}

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

		public Span<int> CaptureOut([UnscopedRef] out int value)
		{
			value = 0;
			return new Span<int>(ref value);
		}

		public Span<int> NoCaptureOut(out int value)
		{
			value = 0;
			return default(Span<int>);
		}

		public Span<int> Identity(Span<int> span)
		{
			return span;
		}

		public void Calls()
		{
			int value = 0;
			scoped Span<int> span = CreateWithoutCapture(ref value);
			span = CreateAndCapture(ref value);
			span = ScopedRefSpan(ref span);
			span = ScopedSpan(span);
			OutSpan(out span);
		}

		public int ReassignScopedRefToLocal(bool b, ref int x)
		{
			int num = 42;
			scoped ref int reference = ref x;
			if (b)
			{
				reference = ref num;
			}
			return reference;
		}

		public int ReassignScopedSpanFromOut(bool b)
		{
			int value = 0;
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = CaptureOut(out value);
			}
			return span[0] + value;
		}

		public int ImplicitScopedOutDoesNotCapture()
		{
			Span<int> span = default(Span<int>);
			span = NoCaptureOut(out var value);
			Console.WriteLine(span.Length);
			return span.Length + value;
		}

		public int ReassignScopedSpanFromReceiver(bool b)
		{
			UnscopedRefStruct unscopedRefStruct = default(UnscopedRefStruct);
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = unscopedRefStruct.AsSpan();
			}
			return span[0];
		}

		public int ReassignScopedSpanFromRefParameter(bool b, ref int value)
		{
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = CreateAndCapture(ref value);
			}
			return span[0];
		}

		public int ReassignScopedSpanFromStackAlloc(bool b)
		{
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = stackalloc int[1];
			}
			return span[0];
		}

		public int ReassignScopedSpanFromScopedValue(bool b, scoped Span<int> value)
		{
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = value;
			}
			return span[0];
		}

		public int ReassignScopedSpanFromNestedCall(bool b)
		{
			int value = 0;
			scoped Span<int> span = default(Span<int>);
			if (b)
			{
				span = Identity(CreateAndCapture(ref value));
			}
			return span[0];
		}

		public int ReassignScopedSpanFromLocalCopy(bool b)
		{
			Span<int> span = stackalloc int[1];
			scoped Span<int> span2 = default(Span<int>);
			if (b)
			{
				span2 = span;
			}
			return span2[0];
		}

		public int ReassignScopedSpanFromScopedRefValue(bool b)
		{
			Span<int> span = stackalloc int[1];
			scoped Span<int> span2 = default(Span<int>);
			if (b)
			{
				span2 = ScopedRefSpan(ref span);
			}
			return span2[0];
		}

		public int NarrowInitializerDoesNotNeedScoped(bool b)
		{
			int num = 1;
			int num2 = 2;
			ref int reference = ref num;
			if (b)
			{
				reference = ref num2;
			}
			return reference;
		}

		public int NarrowThenWideDoesNotNeedScoped(bool b, ref int value)
		{
			int num = 1;
			ref int reference = ref num;
			if (b)
			{
				reference = ref value;
			}
			return reference;
		}

		public int ClassParameterReceiverDoesNotNarrow(SpanProvider provider, bool b)
		{
			Span<int> span = default(Span<int>);
			if (b)
			{
				span = provider.GetBuffer();
			}
			return span.Length;
		}

		public int ClassLocalReceiverDoesNotNarrow(bool b)
		{
			SpanProvider spanProvider = new SpanProvider();
			Span<int> span = default(Span<int>);
			if (b)
			{
				span = spanProvider.GetBuffer();
			}
			return span.Length;
		}

		public ref int PlainStructReceiverDoesNotForceScoped(bool b, ref Buffer buffer, ref int other)
		{
			ref int result = ref buffer.GetRef();
			if (b)
			{
				result = ref other;
			}
			return ref result;
		}

		public int FieldStoreRequiresScoped()
		{
			scoped Holder holder = default(Holder);
			Span<int> inner = stackalloc int[4];
			holder.Inner = inner;
			return holder.Inner.Length;
		}

		public int ReceiverCallRequiresScoped()
		{
			scoped Holder holder = default(Holder);
			Span<int> inner = stackalloc int[4];
			holder.Set(inner);
			return holder.Inner.Length;
		}

		public int ExtensionReceiverCallRequiresScoped()
		{
			scoped Holder holder = default(Holder);
			Span<int> inner = stackalloc int[4];
			holder.SetExtension(inner);
			return holder.Inner.Length;
		}

		public Holder ReadonlyReceiverDoesNotRequireScoped(bool b)
		{
			Holder result = default(Holder);
			if (b)
			{
				Span<int> inner = stackalloc int[4];
				result.ReadonlyUse(inner);
			}
			return result;
		}

		public Holder ReadonlyExtensionReceiverDoesNotRequireScoped(bool b)
		{
			Holder holder = default(Holder);
			if (b)
			{
				Span<int> inner = stackalloc int[4];
				holder.ReadonlyExtension(inner);
			}
			return holder;
		}

		public ReadOnlyHolder ReadonlyRefStructReceiverDoesNotRequireScoped(bool b, Span<int> wide)
		{
			ReadOnlyHolder result = new ReadOnlyHolder(wide);
			if (b)
			{
				Span<int> other = stackalloc int[1];
				result.Compare(other);
			}
			return result;
		}

		public int TryInitThenReassign(bool condition, Span<int> wide)
		{
			scoped Span<int> span;
			try
			{
				span = stackalloc int[4];
			}
			finally
			{
				Console.WriteLine("cleanup");
			}
			if (condition)
			{
				span = wide;
			}
			return span[0];
		}

	}

	internal readonly ref struct ReadOnlyHolder(Span<int> inner)
	{
		private readonly Span<int> inner = inner;

		public void Compare(Span<int> other)
		{
			_ = inner.Length;
			_ = other.Length;
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

	internal sealed class SpanProvider
	{
		public Span<int> GetBuffer()
		{
			return default(Span<int>);
		}
	}

	internal ref struct UnscopedRefStruct
	{
		private int val;

		[UnscopedRef]
		public ref int ByRefProperty => ref val;

		[UnscopedRef]
		public ref int ByRefAccess()
		{
			return ref val;
		}

		[UnscopedRef]
		public Span<int> AsSpan()
		{
			return new Span<int>(ref val);
		}
	}
}
