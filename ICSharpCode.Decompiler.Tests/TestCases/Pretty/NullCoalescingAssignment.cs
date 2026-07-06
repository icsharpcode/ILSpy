using System;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class NullCoalescingAssignment
	{
		private string instanceField;

		private static string staticField;

		private int? instanceNullableField;

		private string[] array;

		public string AutoProp { get; set; }

		public static string StaticAutoProp { get; set; }

		public string ManualProp {
			get {
				return instanceField;
			}
			set {
				instanceField = value;
			}
		}

		public string this[int index] {
			get {
				return array[index];
			}
			set {
				array[index] = value;
			}
		}

		public event EventHandler MyEvent;

		public static void Use<T>(T x)
		{
		}

		public static int SideEffect()
		{
			return 42;
		}

		public static Task<string> GetStringAsync()
		{
			return Task.FromResult("x");
		}

		public void LocalRefType(string a, string b)
		{
			a ??= b;
			Use(a);
		}

		public void LocalNullableValueType(int? a, int b)
		{
			a ??= b;
			Use(a);
		}

		public void LocalNullableValueTypeNullableRhs(int? a, int? b)
		{
			a ??= b;
			Use(a);
		}

		public void LocalUnconstrainedGeneric<T>(T a, T b)
		{
			a ??= b;
			Use(a);
		}

		public void LocalGenericClassConstraint<T>(T a, T b) where T : class
		{
			a ??= b;
			Use(a);
		}

		public void LocalGenericNullableValueType<T>(T? a, T b) where T : struct
		{
			a ??= b;
			Use(a);
		}

		public void InstanceField(string b)
		{
			instanceField ??= b;
		}

		public void StaticField(string b)
		{
			staticField ??= b;
		}

		public void InstanceNullableField(int b)
		{
			instanceNullableField ??= b;
		}

		public void FieldThroughObject(NullCoalescingAssignment c, string b)
		{
			c.instanceField ??= b;
		}

		public void AutoProperty(string b)
		{
			AutoProp ??= b;
		}

		public void StaticAutoProperty(string b)
		{
			StaticAutoProp ??= b;
		}

		public void ManualProperty(string b)
		{
			ManualProp ??= b;
		}

		public void PropertyThroughObject(NullCoalescingAssignment c, string b)
		{
			c.AutoProp ??= b;
		}

		public void Indexer(NullCoalescingAssignment c, string b)
		{
			c[SideEffect()] ??= b;
		}

		public void ArrayElement(string[] arr, string b)
		{
			arr[SideEffect()] ??= b;
		}

		public void EventField(EventHandler h)
		{
			this.MyEvent ??= h;
		}

		public void RefLocal(string b)
		{
			ref string reference = ref instanceField;
			reference ??= b;
		}

		public void RefParameter(ref string a, string b)
		{
			a ??= b;
		}

		public void ExpressionUseLocal(string a, string b)
		{
			Use(a ??= b);
		}

		public string ExpressionReturnLocal(string a, string b)
		{
			return a ??= b;
		}

		public int ExpressionReturnNullableLocal(int? a, int b)
		{
			return a ??= b;
		}

		public void ExpressionUseField(string b)
		{
			Use(instanceField ??= b);
		}

		public void ExpressionUseProperty(string b)
		{
			Use(AutoProp ??= b);
		}

		public void Chained(string a, string b, string c)
		{
			a ??= b ??= c;
			Use(a);
			Use(b);
		}

		public async Task AsyncRhs(string a)
		{
			a ??= await GetStringAsync();
			Use(a);
		}

		public int NullableInterplay(int? a)
		{
			a ??= 5;
			return a.GetValueOrDefault();
		}
	}
}
