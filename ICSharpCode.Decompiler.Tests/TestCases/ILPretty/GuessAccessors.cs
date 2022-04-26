

// ClassLibrary1.UnknownClassTest
using System;
using System.Collections.Generic;

using UnknownNamespace;
namespace ClassLibrary1
{
	public class UnknownClassTest : EventArgs
	{
		public void MethodUnknownClass()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			UnknownClass val = new UnknownClass();
			int? unknownProperty = val.UnknownProperty;
			int? num = unknownProperty.GetValueOrDefault();
			val.UnknownProperty = num;
			int? num2 = num;
			List<object> list = new List<object> {
			val[unknownProperty.Value] ?? "",
			val.NotProperty,
			val.get_NotPropertyWithGeneric<string>(42),
			val[42],
			val.get_NotPropertyWithParameterAndGeneric<object>(int.MinValue),
			val.get_PropertyCalledGet,
			val.set_HasReturnType(),
			val.set_HasReturnType("")
		};
			val.get_NoReturnType();
			val.set_NoValue();
			val.OnEvent += Instance_OnEvent;
			val.OnEvent -= Instance_OnEvent;
			string text = val[(long?)null];
			val[(long?)long.MaxValue] = text;
			IntPtr intPtr = val[UIntPtr.Zero, "Hello"];
			val[(UIntPtr)32uL, "World"] = intPtr;
		}

		public void MethodUnknownGenericClass()
		{
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected O, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Expected O, but got Unknown
			//IL_00cd: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			//IL_00e1: Expected O, but got Unknown
			UnknownGenericClass<UnknownEventArgs> val = new UnknownGenericClass<UnknownEventArgs>();
			UnknownEventArgs unknownProperty = val.UnknownProperty;
			val.UnknownProperty = unknownProperty;
			List<object> list = new List<object> {
				val[((object)unknownProperty).GetHashCode()] ?? "",
				val.NotProperty,
				val.get_NotPropertyWithGeneric<string>(42),
				val[42],
				val.get_NotPropertyWithParameterAndGeneric<object>(int.MinValue),
				val.get_PropertyCalledGet
			};
			val.OnEvent += Instance_OnEvent;
			val.OnEvent -= Instance_OnEvent;
			UnknownEventArgs val2 = val[(UnknownEventArgs)null];
			val[new UnknownEventArgs()] = val2;
			UnknownEventArgs val3 = val[new UnknownEventArgs(), new UnknownEventArgs()];
			val[new UnknownEventArgs(), new UnknownEventArgs()] = val3;
		}

		public void MethodUnknownStatic()
		{
			int? unknownProperty = UnknownStaticClass.UnknownProperty;
			UnknownStaticClass.UnknownProperty = unknownProperty;
			List<object> list = new List<object> {
				UnknownStaticClass[unknownProperty.Value] ?? "",
				UnknownStaticClass.NotProperty,
				UnknownStaticClass.get_NotPropertyWithGeneric<string>(42),
				UnknownStaticClass[42],
				UnknownStaticClass.get_NotPropertyWithParameterAndGeneric<object>(int.MinValue),
				UnknownStaticClass.get_PropertyCalledGet
			};
			UnknownStaticClass.OnEvent += Instance_OnEvent;
			UnknownStaticClass.OnEvent -= Instance_OnEvent;
		}

		public void MethodUnknownStaticGeneric()
		{
			string unknownProperty = UnknownStaticGenericClass<string>.UnknownProperty;
			UnknownStaticGenericClass<string>.UnknownProperty = unknownProperty;
			List<object> list = new List<object> {
				UnknownStaticGenericClass<string>[unknownProperty.Length] ?? "",
				UnknownStaticGenericClass<string>.NotProperty,
				UnknownStaticGenericClass<string>.get_NotPropertyWithGeneric<string>(42),
				UnknownStaticGenericClass<string>[42],
				UnknownStaticGenericClass<string>.get_NotPropertyWithParameterAndGeneric<object>(int.MinValue),
				UnknownStaticGenericClass<string>.get_PropertyCalledGet
			};
			UnknownStaticGenericClass<string>.OnEvent += Instance_OnEvent;
			UnknownStaticGenericClass<string>.OnEvent -= Instance_OnEvent;
		}

		private void Instance_OnEvent(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Instance_OnEvent(object sender, UnknownEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Instance_OnEvent(object sender, string e)
		{
			throw new NotImplementedException();
		}

		private static void Instance_OnEvent(object sender, object e)
		{
			throw new NotImplementedException();
		}
	}
}