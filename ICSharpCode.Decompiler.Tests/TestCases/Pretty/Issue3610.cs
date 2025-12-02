#pragma warning disable CS9124

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3610
	{
		private struct CtorDoubleAssignmentTest
		{
			public bool Value;

			public CtorDoubleAssignmentTest(string arg1, int arg2)
			{
				Value = false;
				Value = true;
			}
		}

		private struct CtorDoubleAssignmentTest2
		{
			public bool Value;

			public CtorDoubleAssignmentTest2(string arg1, int arg2)
			{
				Value = true;
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

}
