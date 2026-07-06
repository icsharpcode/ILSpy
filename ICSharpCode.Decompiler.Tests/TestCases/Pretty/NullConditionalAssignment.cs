using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class NullConditionalAssignment
	{
		private class MyClass
		{
			public int IntField;
			public string Text;
			public MyClass Field;

			public int IntProp { get; set; }

			public string TextProp { get; set; }

			public MyClass Property { get; set; }

			public ref int RefProperty => ref IntField;

			#pragma warning disable format
			public int this[int index] {
				get {
					return 0;
				}
				set {
				}
			}
			#pragma warning restore format

			public event EventHandler Event;

			public void RaiseEvent()
			{
				this.Event?.Invoke(this, EventArgs.Empty);
			}
		}

		private interface IValue
		{
			int Value { get; set; }
		}

		private MyClass GetMyClass()
		{
			return null;
		}

		private int GetIndex()
		{
			return 1;
		}

		private int GetValue()
		{
			return 42;
		}

		private void SimpleAssignments(MyClass c, int i)
		{
			c?.IntField = i;
			c?.IntProp = i;
			c?.Text = "Hello";
			c?.RefProperty = i;
			c?[i] = i;
			GetMyClass()?.IntProp = i;
		}

		private void CompoundAssignments(MyClass c, int i)
		{
			c?.IntField += i;
			c?.IntProp -= i;
			c?.IntField <<= 1;
			c?.IntProp |= i;
			c?[i] *= i;
			c?.Text += "!";
			c?.TextProp ??= "null";
			c?.Field ??= new MyClass();
		}

		private void ChainedAssignments(MyClass c, int i)
		{
			c?.Field.IntProp = i;
			c?.Field?.IntProp = i;
			c?.Property.IntField = i;
			c?.Property?.Field?.IntProp = i;
			c?.Field.Property?.IntField = i;
			c?.Field?.Text ??= "null";
		}

		private int? ValueUsedAssignment(MyClass c, int i)
		{
			return c?.IntProp = i;
		}

		private int? ValueUsedCompound(MyClass c, int i)
		{
			return c?.IntField += i;
		}

		private int ValueUsedWithCoalesce(MyClass c, int i)
		{
			return (c?.IntProp = i) ?? -1;
		}

		private string ValueUsedReferenceType(MyClass c, string s)
		{
			return c?.Text = s;
		}

		private void ValueUsedAsArgument(MyClass c, int i)
		{
			Console.WriteLine(c?.IntProp = i);
		}

		private void EventSubscription(MyClass c, EventHandler h)
		{
			c?.Event += h;
			c?.Event -= h;
			GetMyClass()?.Event += h;
		}

		public void ArrayElement(int[] a, int i)
		{
			a?[0] = i;
			a?[i] += i;
			a?[GetIndex()] = GetValue();
		}

		private void EvaluationOrder(MyClass[] array)
		{
			array?[GetIndex()].IntProp = GetValue();
			GetMyClass()?[GetIndex()] = GetValue();
		}

		private static void GenericClassConstraint<T>(T t, int i) where T : class, IValue
		{
			t?.Value = i;
		}

		private static void GenericUnconstrained<T>(T t, int i) where T : IValue
		{
			t?.Value = i;
		}

		// Null-conditional assignment is only valid when the receiver is a reference
		// type or an unconstrained/class-constrained type parameter. A Nullable<T>
		// receiver ("s?.X = v"), increment/decrement ("c?.IntProp++") and
		// deconstruction targets ("(c?.IntProp, c?.IntField) = (1, 2)") are compile
		// errors in C# 14 and therefore not covered here.
	}
}
