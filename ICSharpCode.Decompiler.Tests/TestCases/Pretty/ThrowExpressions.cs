using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ThrowExpressions
	{
		private class ArgumentCheckingCtor
		{
			private int initializedFromCtor = CountSheep() ?? throw new Exception("No sheep?!");
			private object cacheObj = TryGetObj() ?? throw new Exception("What?");

			private object simpleObj;
			private int? nullableInt;

			public ArgumentCheckingCtor(object simpleObj, int? nullableInt)
			{
				this.simpleObj = (simpleObj ?? throw new ArgumentNullException("simpleObj"));
				this.nullableInt = (nullableInt ?? throw new ArgumentNullException("nullableInt"));
			}

			public ArgumentCheckingCtor(string input)
				: this(input, GetIntOrNull(input ?? throw new ArgumentNullException("input")))
			{

			}

			public ArgumentCheckingCtor(DataObject obj)
				: this(obj ?? throw new Exception(), GetIntOrNull(obj.NullableDataField?.NullableDataField.ToString() ?? throw new ArgumentNullException("input")))
			{

			}

			private static int? GetIntOrNull(string v)
			{
				if (int.TryParse(v, out var result))
				{
					return result;
				}

				return null;
			}

			private static int? CountSheep()
			{
				throw new NotImplementedException();
			}

			private static object TryGetObj()
			{
				return null;
			}

			public override int GetHashCode()
			{
				return initializedFromCtor;
			}

			public override bool Equals(object obj)
			{
				return true;
			}
		}

		public class DataObject
		{
			public int IntField;
			public int? NullableIntField;
			public Data DataField;
			public Data? NullableDataField;
			public int IntProperty {
				get;
				set;
			}
			public int? NullableIntProperty {
				get;
				set;
			}
			public Data DataProperty {
				get;
			}
			public Data? NullableDataProperty {
				get;
			}
		}

		public struct Data
		{
			public int IntField;
			public int? NullableIntField;
			public MoreData DataField;
			public MoreData? NullableDataField;
			public int IntProperty {
				get;
				set;
			}
			public int? NullableIntProperty {
				get;
				set;
			}
			public MoreData DataProperty {
				get;
			}
			public MoreData? NullableDataProperty {
				get;
			}
		}

		public struct MoreData
		{
			public int IntField;
			public int? NullableIntField;
			public int IntProperty {
				get;
				set;
			}
			public int? NullableIntProperty {
				get;
				set;
			}
		}

		public static int IntField;
		public static int? NullableIntField;
		public static object ObjectField;
		public int InstIntField;
		public int? InstNullableIntField;
		public object InstObjectField;
		public Data DataField;
		public Data? NullableDataField;
		public DataObject DataObjectField;

		public static int IntProperty {
			get;
		}
		public static int? NullableIntProperty {
			get;
		}
		public static object ObjProperty {
			get;
		}
		public int InstIntProperty {
			get;
		}
		public int? InstNullableIntProperty {
			get;
		}
		public object InstObjProperty {
			get;
		}
		public Data DataProperty {
			get;
		}
		public Data? NullableDataProperty {
			get;
		}
		public DataObject DataObjectProperty {
			get;
		}

		public static int ReturnIntField()
		{
			return NullableIntField ?? throw new Exception();
		}
		public static int ReturnIntProperty()
		{
			return NullableIntProperty ?? throw new Exception();
		}
		public static object ReturnObjField()
		{
			return ObjectField ?? throw new Exception();
		}
		public static object ReturnObjProperty()
		{
			return ObjProperty ?? throw new Exception();
		}
		public static int ReturnIntField(ThrowExpressions inst)
		{
			return inst.InstNullableIntField ?? throw new Exception();
		}
		public static int ReturnIntProperty(ThrowExpressions inst)
		{
			return inst.InstNullableIntProperty ?? throw new Exception();
		}
		public static object ReturnObjField(ThrowExpressions inst)
		{
			return inst.InstObjectField ?? throw new Exception();
		}
		public static object ReturnObjProperty(ThrowExpressions inst)
		{
			return inst.InstObjProperty ?? throw new Exception();
		}

		public static void UseComplexNullableStruct(ThrowExpressions inst)
		{
			Use(inst.InstNullableIntField ?? throw new Exception());
			Use((inst.NullableDataField ?? throw new Exception()).IntField);
			Use(inst.NullableDataField?.NullableIntField ?? throw new Exception());
			Use((inst.NullableDataProperty ?? throw new Exception()).IntField);
			Use(inst.NullableDataProperty?.NullableIntField ?? throw new Exception());
			Use((inst.NullableDataField ?? throw new Exception()).DataField.IntField);
			Use(inst.NullableDataField?.DataField.NullableIntField ?? throw new Exception());
			Use((inst.NullableDataProperty ?? throw new Exception()).DataField.IntField);
			Use(inst.NullableDataProperty?.DataField.NullableIntField ?? throw new Exception());
			Use((inst.NullableDataField ?? throw new Exception()).DataProperty.IntField);
			Use(inst.NullableDataField?.DataProperty.NullableIntField ?? throw new Exception());
			Use((inst.NullableDataProperty ?? throw new Exception()).DataProperty.IntField);
			Use(inst.NullableDataProperty?.DataProperty.NullableIntField ?? throw new Exception());
			Use(inst.NullableDataField?.NullableDataField?.IntField ?? throw new Exception());
			Use(inst.NullableDataField?.NullableDataField?.NullableIntField ?? throw new Exception());
			Use(inst.NullableDataProperty?.NullableDataField?.IntField ?? throw new Exception());
			Use(inst.NullableDataProperty?.NullableDataField?.NullableIntField ?? throw new Exception());
			Use(inst.NullableDataField?.NullableDataProperty?.IntField ?? throw new Exception());
			Use(inst.NullableDataField?.NullableDataProperty?.NullableIntField ?? throw new Exception());
			Use(inst.NullableDataProperty?.NullableDataProperty?.IntField ?? throw new Exception());
			Use(inst.NullableDataProperty?.NullableDataProperty?.NullableIntField ?? throw new Exception());
		}

		public static void UseComplexNullableObject(DataObject inst)
		{
			Use(inst?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataField?.IntField ?? throw new Exception());
			Use(inst?.NullableDataField?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.IntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataField?.DataField.IntField ?? throw new Exception());
			Use(inst?.NullableDataField?.DataField.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.DataField.IntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.DataField.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataField?.DataProperty.IntField ?? throw new Exception());
			Use(inst?.NullableDataField?.DataProperty.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.DataProperty.IntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.DataProperty.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataField?.NullableDataField?.IntField ?? throw new Exception());
			Use(inst?.NullableDataField?.NullableDataField?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.NullableDataField?.IntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.NullableDataField?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataField?.NullableDataProperty?.IntField ?? throw new Exception());
			Use(inst?.NullableDataField?.NullableDataProperty?.NullableIntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.NullableDataProperty?.IntField ?? throw new Exception());
			Use(inst?.NullableDataProperty?.NullableDataProperty?.NullableIntField ?? throw new Exception());
		}

		public static void Use<T>(T usage)
		{

		}
	}
}
