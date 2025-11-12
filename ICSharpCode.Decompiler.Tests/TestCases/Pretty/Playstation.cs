using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Playstation
{
#pragma warning disable CS0414, CS9113, CS9124

	public record struct CopilotContextId
	{
		public Guid Id { get; }

		public CopilotContextId()
		{
			Id = Guid.NewGuid();
		}

		public CopilotContextId(Guid id)
		{
			Id = id;
		}
	}

	public class CopilotContextId_Class(Guid id)
	{
		public Guid guid { get; } = id;

		public CopilotContextId_Class(Guid id, int value)
			: this(Guid.NewGuid())
		{

		}
		public CopilotContextId_Class()
			: this(Guid.NewGuid(), 222)
		{

		}
	}

	public record CopilotContextId_RecordClass(Guid id)
	{
		public Guid guid { get; } = id;

		public CopilotContextId_RecordClass()
			: this(Guid.NewGuid())
		{

		}
	}

	public record struct CopilotContextId_RecordStruct(Guid id)
	{
		public Guid guid { get; } = id;

		public CopilotContextId_RecordStruct()
			: this(Guid.NewGuid())
		{

		}
	}

	public struct CopilotContextId_Struct
	{
		public Guid guid { get; }

		public CopilotContextId_Struct(Guid id)
		{
			guid = id;
		}

		public CopilotContextId_Struct()
			: this(Guid.NewGuid())
		{

		}
	}

	public abstract record CopilotQueriedMention
	{
		public abstract ConsoleKey Type { get; }

		public string DisplayName { get; init; }

		public string FullName { get; init; }

		public object ProviderMoniker { get; init; }

		internal CopilotQueriedMention(object providerMoniker, string fullName, string displayName)
		{
			ProviderMoniker = providerMoniker;
			FullName = fullName;
			DisplayName = displayName;
		}
	}

	public record CopilotQueriedScopeMention : CopilotQueriedMention
	{
		public override ConsoleKey Type { get; } = ConsoleKey.Enter;

		public CopilotQueriedScopeMention(object providerMoniker, string fullName, string displayName)
			: base(providerMoniker, fullName, displayName)
		{
		}
	}

	public class DeserializationException(string response, Exception innerException) : Exception("Error occured while deserializing the response", innerException)
	{
		public string Response { get; } = response;
	}

	internal static class Ensure
	{
		public static T NotNull<T>(T? value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
			return value;
		}

		public static string NotEmptyString(object? value, string name)
		{
#if OPT
			string obj = (value as string) ?? value?.ToString();
			if (obj == null)
			{
				throw new ArgumentNullException(name);
			}

			if (string.IsNullOrWhiteSpace(obj))
			{
				throw new ArgumentException("Parameter cannot be an empty string", name);
			}
			return obj;
#else
			string text = (value as string) ?? value?.ToString();
			if (text == null)
			{
				throw new ArgumentNullException(name);
			}

			if (string.IsNullOrWhiteSpace(text))
			{
				throw new ArgumentException("Parameter cannot be an empty string", name);
			}
			return text;
#endif
		}
	}

	public struct FromBinaryOperator
	{
		public int Leet;

		public FromBinaryOperator(int dummy1, int dummy2)
		{
			Leet = dummy1 + dummy2;
		}
	}

	public struct FromCall
	{
		public int Leet;

		public FromCall(int dummy1, int dummy2)
		{
			Leet = Math.Max(dummy1, dummy2);
		}
	}

	public struct FromConvert
	{
		public int Leet;

		public FromConvert(double dummy1, double dummy2)
		{
			Leet = (int)Math.Min(dummy1, dummy2);
		}
	}

	internal class Issue3452
	{
		private struct Data
		{
			public object Obj;
		}

		private class C1(object obj)
		{
			internal Data d = new Data {
				Obj = obj
			};
		}

		private class C2(object obj)
		{
			public object Obj => obj;
		}

		private class C3(StringComparison comparison)
		{
			private StringComparison _comparison = comparison;

			internal StringComparison Test()
			{
				return comparison;
			}
		}

		private struct S1
		{
			internal Data d;

			public S1(object obj)
			{
				d = new Data {
					Obj = obj
				};
			}
		}

		private struct S2(object obj)
		{
			public object Obj => obj;
		}
	}

	internal class Issue3610
	{
		private struct CtorDoubleAssignmentTest
		{
#if EXPECTED_OUTPUT
			public bool Value = false;

			public CtorDoubleAssignmentTest(string arg1, int arg2)
			{
				Value = true;
			}
#else
			public bool Value;

			public CtorDoubleAssignmentTest(string arg1, int arg2)
			{
				Value = false;
				Value = true;
			}
#endif
		}

		private struct CtorDoubleAssignmentTest2
		{
			public bool Value = true;

			public CtorDoubleAssignmentTest2(string arg1, int arg2)
			{
				Value = false;
			}
		}

		private class FieldInitTest
		{
			public bool Flag = true;
			public Func<int, int> Action = (int a) => a;
			public string Value;

			public FieldInitTest(string value)
			{
				Value = value;
			}
		}

		private abstract class PCFieldInitTest(StringComparison value)
		{
			private StringComparison _value = value;

			public bool Func()
			{
				return value == StringComparison.Ordinal;
			}
		}

		private class RecordTest<T>
		{
			private interface IInterface
			{
				T[] Objects { get; }
			}

			protected record Record(T[] Objects) : IInterface
			{
				public Record(List<T> objects)
					: this(objects.ToArray())
				{
				}
			}
		}

		private abstract record RecordTest2(Guid[] Guids);
	}

	public record NamedParameter(string name, object? value, bool encode = true) : Parameter(Ensure.NotEmptyString(name, "name"), value, encode);

	[DebuggerDisplay("{DebuggerDisplay()}")]
	public abstract record Parameter
	{
		public string? Name { get; }
		public object? Value { get; }
		public bool Encode { get; }
		protected virtual string ValueString => Value?.ToString() ?? "null";

		protected Parameter(string? name, object? value, bool encode)
		{
			Name = name;
			Value = value;
			Encode = encode;
		}

		public sealed override string ToString()
		{
#if OPT
			if (Value != null)
			{
				return Name + "=" + ValueString;
			}
			return Name ?? "";
#else
			return (Value == null) ? (Name ?? "") : (Name + "=" + ValueString);
#endif
		}

		protected string DebuggerDisplay()
		{
			return GetType().Name.Replace("Parameter", "") + " " + ToString();
		}
	}

	public class Person(string name, int age)
	{
		private readonly string _name = name;
		private readonly int _age = age;

		public string Email { get; init; }

		public Person(string name, int age, string email)
			: this(name, age)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentException("Email cannot be empty");
			}

			Email = email;
			Console.WriteLine("Created person: " + name);
		}
	}

	public class PersonPrimary(string name, int age)
	{
		private readonly string _name = name;
	}

	public class PersonPrimary_CaptureParams(string name, int age)
	{
		public string GetDetails()
		{
			return $"{name}, {age}";
		}
	}

	public class PersonRegular1
	{
		private readonly string _name = "name";
		private readonly int _age = 23;

		public PersonRegular1(string name, int age)
		{
			Thread.Sleep(1000);
			_age = name.Length;
		}
	}

	public class PersonRegular2
	{
		private readonly string _name = "name" + Environment.GetEnvironmentVariable("Path");
		private readonly int _age = Environment.GetEnvironmentVariable("Path")?.Length ?? (-1);

		public PersonRegular2(string name, int age)
		{
		}
	}

	public record QueryParameter(string name, object? value, bool encode = true) : NamedParameter(name, value, encode);

	internal ref struct RefFields
	{
		public ref int Field0;

		public RefFields(ref int v)
		{
			Field0 = ref v;
		}
	}

	internal struct StructWithDefaultCtor
	{
		private int X = 42;

		public StructWithDefaultCtor()
		{
		}
	}

	internal struct ValueFields
	{
		public int Field0;

		public ValueFields(int v)
		{
			Field0 = v;
		}
	}

	internal class WebPair1(string name)
	{
		public string Name { get; } = name;
	}

	internal class WebPair1Primary
	{
		public string Name { get; }

		public WebPair1Primary(string name)
		{
			Name = name;
		}
	}

	internal class WebPair2
	{
		public string Name { get; }

		public WebPair2(string name, string? value, ref readonly object encode)
		{
			Name = name;
		}
	}

	internal class WebPair2Primary(string name, string? value, ref readonly object encode)
	{
		public string Name { get; } = name;
	}

	internal class WebPair3
	{
		public string Name { get; }
		public string? Value { get; }
		private string? WebValue { get; }

		public WebPair3(string name, string? value, bool encode = false)
		{
			Name = name;
			Value = value;
			WebValue = (encode ? "111" : value);
		}
	}

	internal class WebPair3Primary(string name, string? value, bool encode = false)
	{
		public string Name { get; } = name;
		public string? Value { get; } = value;
		private string? WebValue { get; } = encode ? "111" : value;
	}

	internal class WebPair4
	{
		public string Name { get; }
		public string? Value { get; }
		private string? WebValue { get; }
		private string? WebValue2 { get; }

		public WebPair4(string name, string? value, ref readonly object encode)
		{
			Name = name;
			Value = value;
			WebValue = ((encode == null) ? "111" : value);
			WebValue2 = encode.ToString();
		}
	}

	internal class WebPair4Primary(string name, string? value, ref readonly object encode)
	{
		public string Name { get; } = name;
		public string? Value { get; } = value;
		private string? WebValue { get; } = (encode == null) ? "111" : value;
		private string? WebValue2 { get; } = encode.ToString();
	}

	internal class WebPair5
	{
		public string Name { get; }

		public WebPair5(string name, string? value)
		{
			Name = name;
		}
	}

	internal class WebPair5Primary(string name, string? value)
	{
		public string Name { get; } = name;
	}

	internal class WebPair6
	{
		public string? Value { get; }
		public string Name { get; }
		private string? WebValue { get; }
		private string? WebValue2 { get; }

		public WebPair6(string name, string? value, ref readonly object encode)
		{
			Value = name;
			Name = value;
			WebValue = ((name != null) ? "111" : value);
			WebValue2 = ((value != null) ? name : "222");
		}
	}

	internal class WebPair6Primary(string name, string? value, ref readonly object encode)
	{
		public string? Value { get; } = name;
		public string Name { get; } = value;
		private string? WebValue { get; } = (name != null) ? "111" : value;
		private string? WebValue2 { get; } = (value != null) ? name : "222";
	}

}

