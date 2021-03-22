using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class Ext
	{
		public static void ExtOnRef(this ref RefLocalsAndReturns.NormalStruct s)
		{
		}
		public static void ExtOnIn(this in RefLocalsAndReturns.NormalStruct s)
		{
		}
		public static void ExtOnRef(this ref RefLocalsAndReturns.ReadOnlyStruct s)
		{
		}
		public static void ExtOnIn(this in RefLocalsAndReturns.ReadOnlyStruct s)
		{
		}
		public static void ExtOnRef(this ref RefLocalsAndReturns.ReadOnlyRefStruct s)
		{
		}
		public static void ExtOnIn(this in RefLocalsAndReturns.ReadOnlyRefStruct s)
		{
		}
	}

	internal class RefLocalsAndReturns
	{
		public struct Issue1630
		{
			private object data;

			private int next;

			public static void Test()
			{
				Issue1630[] array = new Issue1630[1];
				int num = 0;
				while (num >= 0)
				{
					ref Issue1630 reference = ref array[num];
					Console.WriteLine(reference.data);
					num = reference.next;
				}
			}
		}


		public delegate ref T RefFunc<T>();
		public delegate ref readonly T ReadOnlyRefFunc<T>();
		public delegate ref TReturn RefFunc<T1, TReturn>(T1 param1);

		public ref struct RefStruct
		{
			private int dummy;
		}

		public readonly ref struct ReadOnlyRefStruct
		{
			private readonly int dummy;
		}

		public struct NormalStruct
		{
			private readonly int dummy;
			private int[] arr;

			public int Property {
				get {
					return 1;
				}
				set {
				}
			}

#if CS80
			public readonly int ReadOnlyProperty {
				get {
					return 1;
				}
				set {
				}
			}

			public int PropertyWithReadOnlyGetter {
				readonly get {
					return 1;
				}
				set {
				}
			}

			public int PropertyWithReadOnlySetter {
				get {
					return 1;
				}
				readonly set {
				}
			}

			public ref int RefProperty => ref arr[0];
			public ref readonly int RefReadonlyProperty => ref arr[0];
			public readonly ref int ReadonlyRefProperty => ref arr[0];
			public readonly ref readonly int ReadonlyRefReadonlyProperty => ref arr[0];
#endif

			public ref readonly int this[in int index] => ref arr[index];

			public event EventHandler NormalEvent;

#if CS80
			public readonly event EventHandler ReadOnlyEvent {
				add {
				}
				remove {
				}
			}
#endif
			public void Method()
			{
			}

#if CS80
			public readonly void ReadOnlyMethod()
			{
			}
#endif
		}

		public readonly struct ReadOnlyStruct
		{
			private readonly int Field;

			public void Method()
			{
				ref readonly int field = ref Field;
				Console.WriteLine("No inlining");
				Console.WriteLine(field.GetHashCode());
			}

			public void RefReadonlyCallVirt(RefLocalsAndReturns provider)
			{
				ref readonly NormalStruct readonlyRefInstance = ref provider.GetReadonlyRefInstance<NormalStruct>();
				Console.WriteLine("No inlining");
				readonlyRefInstance.Method();
			}
		}

		private static int[] numbers = new int[10] { 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023 };
		private static string[] strings = new string[2] { "Hello", "World" };

		private static string NullString = "";

		private static int DefaultInt = 0;

		public static ref T GetRef<T>()
		{
			throw new NotImplementedException();
		}

		public static ref readonly T GetReadonlyRef<T>()
		{
			throw new NotImplementedException();
		}

		public ref readonly T GetReadonlyRefInstance<T>()
		{
			throw new NotImplementedException();
		}

		public void CallOnRefReturn()
		{
			// Both direct calls:
			GetRef<NormalStruct>().Method();
			GetRef<ReadOnlyStruct>().Method();

			// call on a copy, not the original ref:
			NormalStruct @ref = GetRef<NormalStruct>();
			@ref.Method();

			ReadOnlyStruct ref2 = GetRef<ReadOnlyStruct>();
			ref2.Method();
		}

		public void CallOnReadOnlyRefReturn()
		{
			// uses implicit temporary:
			GetReadonlyRef<NormalStruct>().Method();
			// direct call:
			GetReadonlyRef<ReadOnlyStruct>().Method();
			// call on a copy, not the original ref:
			ReadOnlyStruct readonlyRef = GetReadonlyRef<ReadOnlyStruct>();
			readonlyRef.Method();
		}

		public void CallOnInParam(in NormalStruct ns, in ReadOnlyStruct rs)
		{
			// uses implicit temporary:
			ns.Method();
			// direct call:
			rs.Method();
			// call on a copy, not the original ref:
			ReadOnlyStruct readOnlyStruct = rs;
			readOnlyStruct.Method();
		}

		public void M(in DateTime a = default(DateTime))
		{
		}

		public void M2<T>(in T a = default(T))
		{
		}

		public void M3<T>(in T? a = null) where T : struct
		{
		}

		public static TReturn Invoker<T1, TReturn>(RefFunc<T1, TReturn> action, T1 value)
		{
			return action(value);
		}

		public static ref int FindNumber(int target)
		{
			for (int i = 0; i < numbers.Length; i++)
			{
				if (numbers[i] >= target)
				{
					return ref numbers[i];
				}
			}
			return ref numbers[0];
		}

		public static ref int LastNumber()
		{
			return ref numbers[numbers.Length - 1];
		}

		public static ref int ElementAtOrDefault(int index)
		{
			if (index >= 0 && index < numbers.Length)
			{
				return ref numbers[index];
			}
			return ref DefaultInt;
		}

		public static ref int LastOrDefault()
		{
			if (numbers.Length != 0)
			{
				return ref numbers[numbers.Length - 1];
			}
			return ref DefaultInt;
		}

		public static void DoubleNumber(ref int num)
		{
			Console.WriteLine("old: " + num);
			num *= 2;
			Console.WriteLine("new: " + num);
		}

		public static ref string GetOrSetString(int index)
		{
			if (index < 0 || index >= strings.Length)
			{
				return ref NullString;
			}

			return ref strings[index];
		}

		public void CallSiteTests(NormalStruct s, ReadOnlyStruct r, ReadOnlyRefStruct rr)
		{
			s.ExtOnIn();
			s.ExtOnRef();
			r.ExtOnIn();
			r.ExtOnRef();
			rr.ExtOnIn();
			rr.ExtOnRef();
			CallOnInParam(in s, in r);
		}

		public void RefReassignment(ref NormalStruct s)
		{
			ref NormalStruct @ref = ref GetRef<NormalStruct>();
			RefReassignment(ref @ref);
			if (s.GetHashCode() == 0)
			{
				@ref = ref GetRef<NormalStruct>();
			}
			RefReassignment(ref @ref.GetHashCode() == 4 ? ref @ref : ref s);
		}

		public static void Main(string[] args)
		{
			DoubleNumber(ref args.Length == 1 ? ref numbers[0] : ref DefaultInt);
			DoubleNumber(ref FindNumber(32));
			Console.WriteLine(string.Join(", ", numbers));
			DoubleNumber(ref LastNumber());
			Console.WriteLine(string.Join(", ", numbers));
			Console.WriteLine(GetOrSetString(0));
			GetOrSetString(0) = "Goodbye";
			Console.WriteLine(string.Join(" ", strings));
			GetOrSetString(5) = "Here I mutated the null value!?";
			Console.WriteLine(GetOrSetString(-5));

			Console.WriteLine(Invoker((int x) => ref numbers[x], 0));
			Console.WriteLine(LastOrDefault());
			LastOrDefault() = 10000;
			Console.WriteLine(ElementAtOrDefault(-5));
		}
	}
}
