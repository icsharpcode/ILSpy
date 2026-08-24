using System;
using System.ComponentModel;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class FieldKeyword
	{
		public class BaseVirtual
		{
			public virtual int P {
				get {
					return field;
				}
				set {
					field = value + 1;
				}
			}
		}

		public class DerivedVirtual : BaseVirtual
		{
			public override int P {
				get {
					return field * 2;
				}
				set {
					field = value - 1;
				}
			}
		}

		public class Generic<T> where T : class
		{
			public T? Item {
				get {
					return field;
				}
				set {
					if (value != null)
					{
						field = value;
					}
				}
			}

			public T Lazy => field ?? (field = Activator.CreateInstance<T>());
		}

		public class Notify : INotifyPropertyChanged
		{
			public string Name {
				get {
					return field;
				}
				set {
					if (field != value)
					{
						field = value;
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
					}
				}
			} = "";

			public event PropertyChangedEventHandler? PropertyChanged;
		}

		public struct StructProperties
		{
			public int Value {
				get {
					return field;
				}
				set {
					field = value & 0xFF;
				}
			}

			public readonly int Doubled => field * 2;

			public StructProperties(int doubled)
			{
				Doubled = doubled;
			}
		}

		public class RealFieldName
		{
			private int field;

			public int PlusOne => this.field + 1;

			public void Reset()
			{
				field = 0;
			}
		}

		public class Field
		{
			public int Value;
		}

		// A generated local name comes from its type, so a local of type "Field" would be
		// called "field" - which C# 14 rejects inside an accessor (CS9273), because the
		// identifier is the backing-field keyword there.
		public class FieldTypedLocal
		{
			public int Count {
				get {
					Field field2 = Create();
					Console.WriteLine(field2.Value);
					return field2.Value + field;
				}
				set {
					field = value;
				}
			}

			// Same collision from the other direction: a local named after the method it is
			// assigned from, with the "Get" prefix stripped.
			public int FromGetter {
				get {
					int field2 = GetField();
					Console.WriteLine(field2);
					return field2 + field;
				}
				set {
					field = value;
				}
			}

			private static Field Create()
			{
				return new Field();
			}

			private static int GetField()
			{
				return 42;
			}
		}

		// A static member named "field" cannot be disambiguated with "this.", so the accessor
		// has to name the declaring type instead; a bare "field" there would bind to the
		// property's own backing field.
		public class RealStaticFieldName
		{
			private static int field;

			public static int PlusOne => RealStaticFieldName.field + 1;

			public static void Reset()
			{
				field = 0;
			}
		}

		public record RecordWithAutoProperties
		{
			public int Value { get; init; }

			public string Text { get; init; } = "";

			public RecordWithAutoProperties(int value)
			{
				Value = value;
			}
		}

		// 0.00m and -0.0 compare equal to their defaults but are observably different, so
		// neither initializer may be dropped as a redundant default.
		public struct PreciseDefaults
		{
			public decimal Scale {
				get {
					return field;
				}
				set {
					field = value;
				}
			} = 0.00m;

			public double Sign {
				get {
					return field;
				}
				set {
					field = value;
				}
			} = -0.0;

			public PreciseDefaults()
			{
			}
		}

		public Func<int> Capture {
			get {
				return () => (field != null) ? 1 : 0;
			}
			set;
		}

		public int GetOnly {
			get {
				if (field == 0)
				{
					field = 42;
				}
				return field;
			}
		}

		public string InitChecked {
			get;
			init {
				field = value.Trim();
			}
		} = "";

		public int LazyGet {
			get {
				if (field == 0)
				{
					field = ComputeDefault();
				}
				return field;
			}
			set;
		}

		public string NullResilient => field ?? (field = CreateDefault());

		public string? OptionalText {
			get {
				return field;
			}
			set {
				field = value ?? string.Empty;
			}
		}

		public int SetOnly {
			set {
				field = value * 2;
			}
		}

		public int SetterValidated {
			get;
			set {
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				field = value;
			}
		}

		public static int StaticCounter {
			get {
				return field;
			}
			set {
				field = Math.Max(field, value);
			}
		}

		public static Func<int> StaticFuncProperty {
			get {
				return () => (field == null) ? 1 : 2;
			}
			set;
		}

		public int TrivialGet => field;

		public int ViaLocalFunction {
			get {
				return Twice();
				int Twice()
				{
					return field * 2;
				}
			}
			set;
		}

		[field: NonSerialized]
		public int WithFieldAttribute {
			get {
				return field;
			}
			set {
				field = value & 0xF;
			}
		}

		public int WithInit {
			get {
				return field;
			}
			set {
				field = value + 1;
			}
		} = 5;

		private static int ComputeDefault()
		{
			return 7;
		}

		private static string CreateDefault()
		{
			return "x";
		}
	}
}
