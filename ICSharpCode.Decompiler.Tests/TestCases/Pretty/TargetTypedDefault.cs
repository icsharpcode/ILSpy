// Copyright (c) 2026 Siegfried Pammer
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

using System;
using System.Linq.Expressions;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class TargetTypedDefault
	{
		public struct ValueHolder
		{
			public int Value;
		}

		public enum Flavor
		{
			None,
			Sweet
		}

#if !OPT
		public Guid guidField = default;
#else
		public Guid guidField;
#endif

		public void OptGuid(Guid guid = default(Guid))
		{
		}

		public void OptCancellationToken(CancellationToken cancellationToken = default(CancellationToken))
		{
		}

		public void OptTimeSpan(TimeSpan timeSpan = default(TimeSpan))
		{
		}

		public void OptDateTime(DateTime dateTime = default(DateTime))
		{
		}

		public void OptCustomStruct(ValueHolder holder = default(ValueHolder))
		{
		}

		public void OptDecimal(decimal d = 0m)
		{
		}

		public void OptNullable(int? x = null)
		{
		}

		public void OptEnum(Flavor flavor = Flavor.None)
		{
		}

		public void OptString(string s = null)
		{
		}

		public void OptInt(int i = 0)
		{
		}

		public void OptGeneric<T>(T value = default(T))
		{
		}

		public void OptGenericStruct<T>(T value = default(T)) where T : struct
		{
		}

		public void OptTuple((int, string) pair = default((int, string)))
		{
		}

		public void CallsWithExplicitDefaults()
		{
			OptGuid();
			OptCancellationToken();
			OptTimeSpan();
			OptCustomStruct();
			OptNullable();
			OptString();
			OptGeneric<Guid>();
			OptTuple();
		}

		public void TakeGuid(Guid guid)
		{
		}

		public void TakeInGuid(in Guid guid)
		{
		}

		public void Over(int x)
		{
		}

		public void Over(string s)
		{
		}

		public void OverStruct(Guid guid)
		{
		}

		public void OverStruct(TimeSpan timeSpan)
		{
		}

		public void CallOverloads()
		{
			Over(0);
			Over(null);
			OverStruct(default(Guid));
			OverStruct(default(TimeSpan));
		}

		public void ArgumentPositions(bool b, Guid guid)
		{
			TakeGuid(default);
			TakeInGuid(default(Guid));
			TakeGuid(b ? guid : default(Guid));
		}

		public T GenericReturn<T>()
		{
			return default;
		}

		public T GenericClassReturn<T>() where T : class
		{
			return null;
		}

		public T GenericNewReturn<T>() where T : new()
		{
			return default;
		}

		public ValueHolder StructReturn()
		{
			return default;
		}

		public (int, string) TupleReturn()
		{
			return default;
		}

		public int? NullableReturn()
		{
			return null;
		}

		public void OutDefault(out Guid guid)
		{
			guid = default;
		}

		public bool GuidIsDefault(Guid guid)
		{
			return guid == default(Guid);
		}

		public bool TimeSpanIsDefault(TimeSpan timeSpan)
		{
			return timeSpan == default(TimeSpan);
		}

		public bool IntIsDefault(int i)
		{
			return i == 0;
		}

		public bool NullableIsDefault(int? x)
		{
			return !x.HasValue;
		}

		public string CoalesceDefault(string s)
		{
			return s ?? null;
		}

		public T[] DefaultInitializedArray<T>()
		{
			return new T[3];
		}

		public unsafe int* PointerDefault()
		{
			return null;
		}

		public Expression<Func<Guid>> GuidExpressionTree()
		{
			return () => default;
		}

		public Expression<Func<int>> ZeroExpressionTree()
		{
			return () => 0;
		}
	}
}
