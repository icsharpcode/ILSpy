// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#pragma warning disable CS9113

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ConstructorInitializers
	{
		public class JArray
		{
			private readonly List<object> objects = new List<object>();
			private readonly List<string> strings = new List<string>();

			public JArray()
			{
			}

			public JArray(params object[] items)
			{
				foreach (object item in items)
				{
					objects.Add(item);
				}
			}

			public JArray(object content)
			{
				objects.Add(content);
			}

			public JArray(string content)
			{
				strings.Add(content);
			}
		}

		public struct Issue1743
		{
			public int Leet;

			public Issue1743(int dummy)
				: this(dummy, dummy)
			{
				Leet += dummy;
			}

			public Issue1743(int dummy1, int dummy2)
			{
				Leet = dummy1 + dummy2;
			}
		}

#if CS120
		public struct Issue1743WithPrimaryCtor(int dummy1, int dummy2)
		{
			public int Leet = dummy1 + dummy2;

			public Issue1743WithPrimaryCtor(int dummy)
				: this(dummy, dummy)
			{
				Leet += dummy;
			}
		}

		/// <summary>
		/// This is info about the class
		/// </summary>
		private struct StructWithXmlDocCtor
		{
			public int A;

			public int B;

			/// <summary>
			/// This is info about the constructor
			/// </summary>
			public StructWithXmlDocCtor(int a, int b)
			{
				A = a;
				B = b;
			}
		}

		/// <summary>
		/// This is info about the class
		/// </summary>
		private struct StructWithoutXmlDocCtor(int a, int b)
		{
			public int A = a;

			public int B = b;
		}
#endif

		public class ClassWithConstant
		{
			// using decimal constants has the effect that there is a cctor
			// generated containing the explicit initialization of this field.
			// The type is marked beforefieldinit
			private const decimal a = 1.0m;
		}

		public class ClassWithConstantAndStaticCtor
		{
			// The type is not marked beforefieldinit
			private const decimal a = 1.0m;

			static ClassWithConstantAndStaticCtor()
			{

			}
		}

		public class MethodCallInCtorInit
		{
			public MethodCallInCtorInit(ConsoleKey key)
#if MCS5
				: this(((int)key/*cast due to constrained. prefix*/).ToString())
#else
				: this(((int)key).ToString())
#endif
			{
			}

			public MethodCallInCtorInit(string s)
			{
			}
		}

		public struct SimpleStruct
		{
			public int Field1;
			public int Field2;
		}

		public class UnsafeFields
		{
			public unsafe static int StaticSizeOf = sizeof(SimpleStruct);
			public unsafe int SizeOf = sizeof(SimpleStruct);
		}

#if CS120
		public class ClassWithPrimaryCtorUsingGlobalParameter(int a)
		{
			public void Print()
			{
				Console.WriteLine(a);
			}
		}

		public class ClassWithPrimaryCtorUsingGlobalParameterAssignedToField(int a)
		{
#pragma warning disable CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
			private readonly int _a = a;
#pragma warning restore CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.

			public void Print()
			{
				Console.WriteLine(_a);
			}
		}

		public class ClassWithPrimaryCtorUsingGlobalParameterAssignedToFieldAndUsedInMethod(int a)
		{
#pragma warning disable CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
			private readonly int _a = a;
#pragma warning restore CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.

			public void Print()
			{
				Console.WriteLine(a);
			}
		}

		public class ClassWithPrimaryCtorUsingGlobalParameterAssignedToProperty(int a)
		{
			public int A { get; set; } = a;

			public void Print()
			{
				Console.WriteLine(A);
			}
		}

		public class ClassWithPrimaryCtorUsingGlobalParameterInExpressionAssignedToProperty(int a)
		{
			public int A { get; set; } = (int)Math.Abs(Math.PI * (double)a);

			public void Print()
			{
				Console.WriteLine(A);
			}
		}

		public class ClassWithPrimaryCtorUsingGlobalParameterAssignedToEvent(EventHandler a)
		{
			public event EventHandler A = a;

			public void Print()
			{
				Console.WriteLine(this.A);
			}
		}
#endif

		public class NoRecordButCopyConstructorLike
		{
			private NoRecordButCopyConstructorLike parent;

			public NoRecordButCopyConstructorLike(NoRecordButCopyConstructorLike parent)
			{
				this.parent = parent;
			}
		}

#if CS70
		public class NullCheckedArgumentBase
		{
			public NullCheckedArgumentBase(int a, int b, int c, string s)
			{
			}
		}

		public class NullCheckedArgumentChain : NullCheckedArgumentBase
		{
			public string Value;

			// 'value?.Length ?? throw ...' combined with a later reuse of 'value' makes the compiler
			// hoist the null-check in front of the chained constructor call. The decompiler must fold
			// that guard back into the first argument as 'value ?? throw ...' rather than emit an
			// illegal in-body 'base..ctor(...)' call.
			public NullCheckedArgumentChain(string value)
#if EXPECTED_OUTPUT
				: base(0, (value ?? throw new ArgumentNullException("value")).Length, value.Length, "value")
#else
				: base(0, value?.Length ?? throw new ArgumentNullException("value"), value.Length, "value")
#endif
			{
				Value = value;
			}
		}

		public class NullCheckedArgumentThisChain
		{
			public string Value;

			public NullCheckedArgumentThisChain(int a, int b, int c, string s)
			{
			}

			public NullCheckedArgumentThisChain(string value)
#if EXPECTED_OUTPUT
				: this(0, (value ?? throw new ArgumentNullException("value")).Length, value.Length, "value")
#else
				: this(0, value?.Length ?? throw new ArgumentNullException("value"), value.Length, "value")
#endif
			{
				Value = value;
			}
		}

		public struct NullCheckedStructThisChain
		{
			public int Leet;

			// Value types chain via 'this = new TSelf(...)'; ChainedConstructorCallILOffset only
			// reports reference-type chained calls, so the hoisted guard fold must locate the struct
			// this(...) call itself, otherwise the guard survives and defeats the initializer.
			public NullCheckedStructThisChain(string value)
#if EXPECTED_OUTPUT
				: this(0, (value ?? throw new ArgumentNullException("value")).Length, value.Length, "value")
#else
				: this(0, value?.Length ?? throw new ArgumentNullException("value"), value.Length, "value")
#endif
			{
				Leet += value.Length;
			}

			public NullCheckedStructThisChain(int a, int b, int c, string s)
			{
				Leet = a + b + c;
			}
		}

		public struct NullCheckedStructTwoArguments
		{
			public int Leet;

			// Two reused parameters before a value-type this(...) chain produce two stacked hoisted
			// guards; both must be folded.
			public NullCheckedStructTwoArguments(string a, string b)
#if EXPECTED_OUTPUT
				: this((a ?? throw new ArgumentNullException("a")).Length, (b ?? throw new ArgumentNullException("b")).Length, b.Length, a)
#else
				: this(a?.Length ?? throw new ArgumentNullException("a"), b?.Length ?? throw new ArgumentNullException("b"), b.Length, a)
#endif
			{
				Leet += a.Length;
			}

			public NullCheckedStructTwoArguments(int a, int b, int c, string s)
			{
				Leet = a + b + c;
			}
		}

		public class NullCheckedTwoArguments : NullCheckedArgumentBase
		{
			// Two reused parameters produce two stacked hoisted guards before the chained call;
			// each must be folded into the first argument that uses it.
			public NullCheckedTwoArguments(string a, string b)
#if EXPECTED_OUTPUT
				: base((a ?? throw new ArgumentNullException("a")).Length, (b ?? throw new ArgumentNullException("b")).Length, a.Length, b)
#else
				: base(a?.Length ?? throw new ArgumentNullException("a"), b?.Length ?? throw new ArgumentNullException("b"), a.Length, b)
#endif
			{
			}
		}

		public class NullCheckedNullableArgument : NullCheckedArgumentBase
		{
			// A nullable value type argument with 'value ?? throw ...' and a later reuse: here the
			// compiler emits its own Nullable<T> copy plus a HasValue check in the middle of the
			// argument evaluation (instead of hoisting a comparison against null), which the
			// value-type throw-expression fold reassembles.
			public NullCheckedNullableArgument(int? value)
				: base(value ?? throw new ArgumentNullException("value"), value.Value, 0, "value")
			{
			}
		}

		public struct NullCheckedNullableStructThisChain
		{
			public int Leet;

			public NullCheckedNullableStructThisChain(int? value)
				: this(value ?? throw new ArgumentNullException("value"), value.Value, 0, "value")
			{
				Leet += value.Value;
			}

			public NullCheckedNullableStructThisChain(int a, int b, int c, string s)
			{
				Leet = a + b + c;
			}
		}

		public class NullCheckedFirstArgument : NullCheckedArgumentBase
		{
			// The guarded parameter is used by the very first argument, so the fold targets argument
			// index 0.
			public NullCheckedFirstArgument(string value)
#if EXPECTED_OUTPUT
				: base((value ?? throw new ArgumentNullException("value")).Length, value.Length, 0, "value")
#else
				: base(value?.Length ?? throw new ArgumentNullException("value"), value.Length, 0, "value")
#endif
			{
			}
		}

		public class NotHoistedBodyGuard
		{
			public string Value;

			// An ordinary argument-validation guard runs after the (implicit) base call, so it must
			// stay an in-body statement and must not be folded into an initializer.
			public NotHoistedBodyGuard(string value)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Value = value;
			}
		}
#endif

#if CS100
		public class PrimaryCtorClassThisChain(Guid id)
		{
			public Guid guid { get; } = id;

			public PrimaryCtorClassThisChain(Guid id, int value)
				: this(Guid.NewGuid())
			{

			}
			public PrimaryCtorClassThisChain()
				: this(Guid.NewGuid(), 222)
			{

			}
		}
#if EXPECTED_OUTPUT
		public class UnusedPrimaryCtorParameter
		{
			public UnusedPrimaryCtorParameter(int unused)
			{
			}
		}
#else
		public class UnusedPrimaryCtorParameter(int unused)
		{
		}
#endif
#if OPT && EXPECTED_OUTPUT
		public class C8(object obj)
		{
			public int Test()
			{
				object obj2 = obj;
				if (obj2 is int)
				{
					return (int)obj2;
				}
				return 0;
			}
		}
#else
		public class C8(object obj)
		{
			public int Test()
			{
				if (obj is int result)
				{
					return result;
				}
				return 0;
			}
		}
#endif

		public struct My
		{
			private ClassWithConstantAndStaticCtor test;

			internal My(ClassWithConstantAndStaticCtor a)
			{
				test = a;
			}
		}
#endif
	}
}
