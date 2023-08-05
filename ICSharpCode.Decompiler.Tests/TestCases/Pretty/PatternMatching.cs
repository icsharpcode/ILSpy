using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class PatternMatching
	{
		public class X
		{
			public int? NullableIntField;
			public S? NullableCustomStructField;
			public int I { get; set; }
			public string Text { get; set; }
			public object Obj { get; set; }
			public S CustomStruct { get; set; }
			public int? NullableIntProp { get; set; }
			public S? NullableCustomStructProp { get; set; }
		}

		public struct S
		{
			public int I;
			public string Text { get; set; }
			public object Obj { get; set; }
			public S2 S2 { get; set; }
		}

		public struct S2
		{
			public int I;
			public float F;
			public decimal D;
			public string Text { get; set; }
			public object Obj { get; set; }
		}

		public void SimpleTypePattern(object x)
		{
			if (x is string value)
			{
				Console.WriteLine(value);
			}
		}

		public void TypePatternWithShortcircuit(object x)
		{
			Use(F() && x is string text && text.Contains("a"));
			if (F() && x is string text2 && text2.Contains("a"))
			{
				Console.WriteLine(text2);
			}
		}

		public void TypePatternWithShortcircuitAnd(object x)
		{
			if (x is string text && text.Contains("a"))
			{
				Console.WriteLine(text);
			}
			else
			{
				Console.WriteLine();
			}
		}

		public void TypePatternWithShortcircuitOr(object x)
		{
			if (!(x is string text) || text.Contains("a"))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(text);
			}
		}

		public void TypePatternWithShortcircuitOr2(object x)
		{
			if (F() || !(x is string value))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(value);
			}
		}

		public void TypePatternValueTypesCondition(object x)
		{
			if (x is int num)
			{
				Console.WriteLine("Integer: " + num);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypesCondition2()
		{
			if (GetObject() is int num)
			{
				Console.WriteLine("Integer: " + num);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypesWithShortcircuitAnd(object x)
		{
			if (x is int num && num.GetHashCode() > 0)
			{
				Console.WriteLine("Positive integer: " + num);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypesWithShortcircuitOr(object x)
		{
			if (!(x is int value) || value.GetHashCode() > 0)
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(value);
			}
		}

#if ROSLYN3 || OPT
		// Roslyn 2.x generates a complex infeasible path in debug builds, which RemoveInfeasiblePathTransform
		// currently cannot handle. Because this would increase the complexity of that transform, we ignore
		// this case.
		public void TypePatternValueTypesWithShortcircuitOr2(object x)
		{
			if (F() || !(x is int value))
			{
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(value);
			}
		}
#endif

		public void TypePatternGenerics<T>(object x)
		{
			if (x is T val)
			{
				Console.WriteLine(val.GetType().FullName);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}

		public void TypePatternGenericRefType<T>(object x) where T : class
		{
			if (x is T val)
			{
				Console.WriteLine(val.GetType().FullName);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}

		public void TypePatternGenericValType<T>(object x) where T : struct
		{
			if (x is T val)
			{
				Console.WriteLine(val.GetType().FullName);
			}
			else
			{
				Console.WriteLine("not a " + typeof(T).FullName);
			}
		}

		public void TypePatternValueTypesWithShortcircuitAndMultiUse(object x)
		{
			if (x is int num && num.GetHashCode() > 0 && num % 2 == 0)
			{
				Console.WriteLine("Positive integer: " + num);
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypesWithShortcircuitAndMultiUse2(object x)
		{
			if ((x is int num && num.GetHashCode() > 0 && num % 2 == 0) || F())
			{
				Console.WriteLine("true");
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypesWithShortcircuitAndMultiUse3(object x)
		{
			if (F() || (x is int num && num.GetHashCode() > 0 && num % 2 == 0))
			{
				Console.WriteLine("true");
			}
			else
			{
				Console.WriteLine("else");
			}
		}

		public void TypePatternValueTypes()
		{
			Use(F() && GetObject() is int num && num.GetHashCode() > 0 && num % 2 == 0);
		}

		public static void NotTypePatternVariableUsedOutsideTrueBranch(object x)
		{
			string text = x as string;
			if (text != null && text.Length > 5)
			{
				Console.WriteLine("pattern matches");
			}
			if (text != null && text.Length > 10)
			{
				Console.WriteLine("other use!");
			}
		}

		public static void NotTypePatternBecauseVarIsNotDefAssignedInCaseOfFallthrough(object x)
		{
#if OPT
			string obj = x as string;
			if (obj == null)
			{
				Console.WriteLine("pattern doesn't match");
			}
			Console.WriteLine(obj == null);
#else
			string text = x as string;
			if (text == null)
			{
				Console.WriteLine("pattern doesn't match");
			}
			Console.WriteLine(text == null);
#endif
		}

		public void GenericTypePatternInt<T>(T x)
		{
			if (x is int value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not an int");
			}
		}

		public void GenericValueTypePatternInt<T>(T x) where T : struct
		{
			if (x is int value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not an int");
			}
		}

		public void GenericRefTypePatternInt<T>(T x) where T : class
		{
			if (x is int value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not an int");
			}
		}

		public void GenericTypePatternString<T>(T x)
		{
			if (x is string value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not a string");
			}
		}

		public void GenericRefTypePatternString<T>(T x) where T : class
		{
			if (x is string value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not a string");
			}
		}

		public void GenericValueTypePatternStringRequiresCastToObject<T>(T x) where T : struct
		{
			if ((object)x is string value)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine("not a string");
			}
		}

#if CS80
		public void RecursivePattern_Type(object x)
		{
			if (x is X { Obj: string obj })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_TypeAndConst(object x)
		{
			if (x is X { Obj: string obj, I: 42 })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_Constant(object obj)
		{
			if (obj is X { Obj: null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_StringConstant(object obj)
		{
			if (obj is X { Text: "Hello" } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_MultipleConstants(object obj)
		{
			if (obj is X { I: 42, Text: "Hello" } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_ValueTypeWithField(object obj)
		{
			if (obj is S { I: 42 } s)
			{
				Console.WriteLine("Test " + s);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_MultipleConstantsMixedWithVar(object x)
		{
			if (x is X { I: 42, Obj: var obj, Text: "Hello" })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NonTypePattern(object obj)
		{
			if (obj is X { I: 42, Text: { Length: 0 } } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePatternValueType_NonTypePatternTwoProps(object obj)
		{
			if (obj is X { I: 42, CustomStruct: { I: 0, Text: "Test" } } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NonTypePatternNotNull(object o)
		{
			if (o is X { I: 42, Text: not null, Obj: var obj } x)
			{
				Console.WriteLine("Test " + x.I + " " + obj.GetType());
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_VarLengthPattern(object obj)
		{
			if (obj is X { I: 42, Text: { Length: var length } } x)
			{
				Console.WriteLine("Test " + x.I + ": " + length);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePatternValueType_VarLengthPattern(object obj)
		{
			if (obj is S { I: 42, Text: { Length: var length } } s)
			{
				Console.WriteLine("Test " + s.I + ": " + length);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePatternValueType_VarLengthPattern_SwappedProps(object obj)
		{
			if (obj is S { Text: { Length: var length }, I: 42 } s)
			{
				Console.WriteLine("Test " + s.I + ": " + length);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_VarLengthPattern_SwappedProps(object obj)
		{
			if (obj is X { Text: { Length: var length }, I: 42 } x)
			{
				Console.WriteLine("Test " + x.I + ": " + length);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntField_Const(object obj)
		{
			if (obj is X { NullableIntField: 42 } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntField_Null(object obj)
		{
			if (obj is X { NullableIntField: null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntField_NotNull(object obj)
		{
			if (obj is X { NullableIntField: not null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntField_Var(object obj)
		{
			if (obj is X { NullableIntField: var nullableIntField })
			{
				Console.WriteLine("Test " + nullableIntField.Value);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntProp_Const(object obj)
		{
			if (obj is X { NullableIntProp: 42 } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntProp_Null(object obj)
		{
			if (obj is X { NullableIntProp: null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntProp_NotNull(object obj)
		{
			if (obj is X { NullableIntProp: not null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableIntProp_Var(object obj)
		{
			if (obj is X { NullableIntProp: var nullableIntProp })
			{
				Console.WriteLine("Test " + nullableIntProp.Value);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructField_Const(object obj)
		{
			if (obj is X { NullableCustomStructField: { I: 42, Obj: not null } nullableCustomStructField })
			{
				Console.WriteLine("Test " + nullableCustomStructField.I);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructField_Null(object obj)
		{
			if (obj is X { NullableCustomStructField: null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructField_NotNull(object obj)
		{
			if (obj is X { NullableCustomStructField: not null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructField_Var(object obj)
		{
			if (obj is X { NullableCustomStructField: var nullableCustomStructField, Obj: null })
			{
				Console.WriteLine("Test " + nullableCustomStructField.Value);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructProp_Const(object obj)
		{
			if (obj is X { NullableCustomStructProp: { I: 42, Obj: not null } nullableCustomStructProp })
			{
				Console.WriteLine("Test " + nullableCustomStructProp.Text);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructProp_Null(object obj)
		{
			if (obj is X { NullableCustomStructProp: null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructProp_NotNull(object obj)
		{
			if (obj is X { NullableCustomStructProp: not null } x)
			{
				Console.WriteLine("Test " + x);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_NullableCustomStructProp_Var(object obj)
		{
			if (obj is X { NullableCustomStructProp: var nullableCustomStructProp, Obj: null })
			{
				Console.WriteLine("Test " + nullableCustomStructProp.Value);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_CustomStructNested_Null(object obj)
		{
			if (obj is S { S2: { Obj: null } })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_CustomStructNested_TextLengthZero(object obj)
		{
			if (obj is S { S2: { Text: { Length: 0 } } })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_CustomStructNested_EmptyString(object obj)
		{
			if (obj is S { S2: { Text: "" } })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_CustomStructNested_Float(object obj)
		{
			if (obj is S { S2: { F: 3.141f, Obj: null } })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}

		public void RecursivePattern_CustomStructNested_Decimal(object obj)
		{
			if (obj is S { S2: { D: 3.141m, Obj: null } })
			{
				Console.WriteLine("Test " + obj);
			}
			else
			{
				Console.WriteLine("not Test");
			}
		}
#endif
		private bool F()
		{
			return true;
		}

		private object GetObject()
		{
			throw new NotImplementedException();
		}

		private void Use(bool x)
		{
		}
	}
}
