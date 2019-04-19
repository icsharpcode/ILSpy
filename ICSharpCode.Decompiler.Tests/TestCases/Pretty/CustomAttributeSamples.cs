// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples
{
	[Obsolete("reason")]
	public delegate int AppliedToDelegate();

	[Obsolete("reason")]
	public interface AppliedToInterface
	{
	}

	[Obsolete("reason")]
	public struct AppliedToStruct
	{
		public int Field;
	}

	[Flags]
	public enum EnumWithFlagsAttribute
	{
		None = 0x0
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeNamedInitializerFieldEnumAttribute : Attribute
	{
		public AttributeTargets Field;
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeNamedInitializerPropertyEnumAttribute : Attribute
	{
		public AttributeTargets Prop {
			get {
				return AttributeTargets.All;
			}
			set {
			}
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeOnReturnTypeOfDelegateAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeTargetPropertyIndexSetMultiParamAttribute : Attribute
	{
		public int Field;
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeWithCustomPropertyAttribute : Attribute
	{
		public string Prop {
			get {
				return "";
			}
			set {
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MyAttributeWithNamedArgumentAppliedAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyAttributeWithNamedInitializerPropertyTypeAttribute : Attribute
	{
		public Type Prop {
			get {
				return null;
			}
			set {
			}
		}
	}

	[MyAttributeWithCustomProperty(Prop = "value")]
	public class MyClass
	{
	}

	public class MyClass<[MyClassAttributeOnTypeParameter] T>
	{
	}

	[MyAttributeWithNamedInitializerPropertyType(Prop = typeof(Enum))]
	public class MyClass02
	{
	}

	[MyAttributeNamedInitializerPropertyEnum(Prop = (AttributeTargets.Class | AttributeTargets.Method))]
	public class MyClass03
	{
	}

	[MyAttributeNamedInitializerFieldEnum(Field = (AttributeTargets.Class | AttributeTargets.Method))]
	public class MyClass04
	{
	}

	public class MyClass05
	{
		[return: MyAttribute]
		public int MyMethod()
		{
			return 5;
		}
	}


	public class MyClass06
	{
		public int Prop {
			[return: MyAttribute]
			get {
				return 3;
			}
		}
	}

	public class MyClass07
	{
		public int Prop {
			[param: MyAttribute]
			set {
			}
		}
	}


	public class MyClass08
	{
		public int Prop {
			get {
				return 3;
			}
			[return: MyAttribute]
			set {
			}
		}
	}

	public class MyClass09
	{
		public int this[string s] {
			[return: MyAttribute]
			get {
				return 3;
			}
		}
	}

	public class MyClass10
	{
		public int this[[MyAttribute] string s] {
			set {
			}
		}
	}

	public class MyClass11
	{
#if ROSLYN
		public int this[[MyAttribute] string s] => 3;
#else
		public int this[[MyAttribute] string s] {
			get {
				return 3;
			}
		}
#endif
	}

	public class MyClass12
	{
		public string this[int index] {
			get {
				return "";
			}
			[return: MyAttribute]
			set {
			}
		}
	}

	public class MyClass13
	{
		public string this[[MyAttributeTargetPropertyIndexSetMultiParam(Field = 2)] int index1, [MyAttributeTargetPropertyIndexSetMultiParam(Field = 3)] int index2] {
			get {
				return "";
			}
			[param: MyAttribute]
			set {
			}
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyClassAttributeOnTypeParameterAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class MyMethodOrInterfaceAttributeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.All)]
	public class MyTypeAttribute : Attribute
	{
		public MyTypeAttribute(Type t)
		{
		}
	}

	[Obsolete("message")]
	public class ObsoleteClass
	{
	}

	[MyType(typeof(Attribute))]
	public class SomeClass
	{
	}

	[return: MyAttributeOnReturnTypeOfDelegate]
	public delegate void Test();

	public class TestClass
	{
		[MyAttribute]
		public int Field;

		[Obsolete("reason")]
#if ROSLYN
		public int Property => 0;
#else
		public int Property {
			get {
				return 0;
			}
		}
#endif

		public int PropertyAttributeOnGetter {
			[MyAttribute]
			get {
				return 0;
			}
		}

		public int PropertyAttributeOnSetter {
			get {
				return 3;
			}
			[MyAttribute]
			set {
			}
		}

		[Obsolete("reason")]
#if ROSLYN
		public int this[int i] => 0;
#else
		public int this[int i] {
			get {
				return 0;
			}
		}
#endif
		[MyAttribute]
		public event EventHandler MyEvent;

		[method: MyAttribute]
		public event EventHandler MyEvent2;

		[MyAttribute]
		public void Method()
		{
		}

		public void Method([MyAttribute] int val)
		{
		}
	}
}