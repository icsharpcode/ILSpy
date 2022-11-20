.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly SecurityDeclarations
{
	.custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = (
		01 00 07 31 2e 30 2e 30 2e 30 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module SecurityDeclarations.dll
// MVID: {761F919A-2373-48EB-9282-9DAB26913D43}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly


.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private sequential ansi sealed beforefieldinit SecurityDeclarations.TestStruct
	extends [mscorlib]System.ValueType
{
	.pack 0
	.size 1

} // end of class SecurityDeclarations.TestStruct

.class private auto ansi beforefieldinit SecurityDeclarations.SimpleType
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method SimpleType::.ctor

} // end of class SecurityDeclarations.SimpleType

.class private auto ansi sealed SecurityDeclarations.TestEnum
	extends [mscorlib]System.Enum
{
	// Fields
	.field public specialname rtspecialname int32 value__
	.field public static literal valuetype SecurityDeclarations.TestEnum A = int32(0)
	.field public static literal valuetype SecurityDeclarations.TestEnum B = int32(1)
	.field public static literal valuetype SecurityDeclarations.TestEnum C = int32(2)

} // end of class SecurityDeclarations.TestEnum

.class private auto ansi beforefieldinit SecurityDeclarations.SecurityAttrTest
	extends [mscorlib]System.Security.Permissions.SecurityAttribute
{
	// Fields
	.field private string[] _testStringArray
	.field private int32[] _testInt32Array
	.field private valuetype SecurityDeclarations.TestEnum[] _testEnumArray
	.field private class [mscorlib]System.Type[] _testTypeArray
	.field public int32 TestInt32
	.field public class [mscorlib]System.Type TestType
	.field public valuetype SecurityDeclarations.TestEnum TestEnumType
	.field public object TestBoxed
	.field public object TestBoxed2
	.field public string TestString
	.field public object TestBoxedString
	.field public object TestBoxedArray
	.field public object TestBoxedType

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			valuetype [mscorlib]System.Security.Permissions.SecurityAction action
		) cil managed 
	{
		// Method begins at RVA 0x2059
		// Code size 10 (0xa)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call instance void [mscorlib]System.Security.Permissions.SecurityAttribute::.ctor(valuetype [mscorlib]System.Security.Permissions.SecurityAction)
		IL_0007: nop
		IL_0008: nop
		IL_0009: ret
	} // end of method SecurityAttrTest::.ctor

	.method public hidebysig virtual 
		instance class [mscorlib]System.Security.IPermission CreatePermission () cil managed 
	{
		// Method begins at RVA 0x2064
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
		IL_0006: throw
	} // end of method SecurityAttrTest::CreatePermission

	.method public hidebysig specialname 
		instance string[] get_TestStringArray () cil managed 
	{
		// Method begins at RVA 0x206c
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld string[] SecurityDeclarations.SecurityAttrTest::_testStringArray
		IL_0006: ret
	} // end of method SecurityAttrTest::get_TestStringArray

	.method public hidebysig specialname 
		instance void set_TestStringArray (
			string[] 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2074
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld string[] SecurityDeclarations.SecurityAttrTest::_testStringArray
		IL_0007: ret
	} // end of method SecurityAttrTest::set_TestStringArray

	.method public hidebysig specialname 
		instance int32[] get_TestInt32Array () cil managed 
	{
		// Method begins at RVA 0x207d
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld int32[] SecurityDeclarations.SecurityAttrTest::_testInt32Array
		IL_0006: ret
	} // end of method SecurityAttrTest::get_TestInt32Array

	.method public hidebysig specialname 
		instance void set_TestInt32Array (
			int32[] 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2085
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld int32[] SecurityDeclarations.SecurityAttrTest::_testInt32Array
		IL_0007: ret
	} // end of method SecurityAttrTest::set_TestInt32Array

	.method public hidebysig specialname 
		instance valuetype SecurityDeclarations.TestEnum[] get_TestEnumArray () cil managed 
	{
		// Method begins at RVA 0x208e
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld valuetype SecurityDeclarations.TestEnum[] SecurityDeclarations.SecurityAttrTest::_testEnumArray
		IL_0006: ret
	} // end of method SecurityAttrTest::get_TestEnumArray

	.method public hidebysig specialname 
		instance void set_TestEnumArray (
			valuetype SecurityDeclarations.TestEnum[] 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2096
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld valuetype SecurityDeclarations.TestEnum[] SecurityDeclarations.SecurityAttrTest::_testEnumArray
		IL_0007: ret
	} // end of method SecurityAttrTest::set_TestEnumArray

	.method public hidebysig specialname 
		instance class [mscorlib]System.Type[] get_TestTypeArray () cil managed 
	{
		// Method begins at RVA 0x209f
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Type[] SecurityDeclarations.SecurityAttrTest::_testTypeArray
		IL_0006: ret
	} // end of method SecurityAttrTest::get_TestTypeArray

	.method public hidebysig specialname 
		instance void set_TestTypeArray (
			class [mscorlib]System.Type[] 'value'
		) cil managed 
	{
		// Method begins at RVA 0x20a7
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: stfld class [mscorlib]System.Type[] SecurityDeclarations.SecurityAttrTest::_testTypeArray
		IL_0007: ret
	} // end of method SecurityAttrTest::set_TestTypeArray

	// Properties
	.property instance string[] TestStringArray()
	{
		.get instance string[] SecurityDeclarations.SecurityAttrTest::get_TestStringArray()
		.set instance void SecurityDeclarations.SecurityAttrTest::set_TestStringArray(string[])
	}
	.property instance int32[] TestInt32Array()
	{
		.get instance int32[] SecurityDeclarations.SecurityAttrTest::get_TestInt32Array()
		.set instance void SecurityDeclarations.SecurityAttrTest::set_TestInt32Array(int32[])
	}
	.property instance valuetype SecurityDeclarations.TestEnum[] TestEnumArray()
	{
		.get instance valuetype SecurityDeclarations.TestEnum[] SecurityDeclarations.SecurityAttrTest::get_TestEnumArray()
		.set instance void SecurityDeclarations.SecurityAttrTest::set_TestEnumArray(valuetype SecurityDeclarations.TestEnum[])
	}
	.property instance class [mscorlib]System.Type[] TestTypeArray()
	{
		.get instance class [mscorlib]System.Type[] SecurityDeclarations.SecurityAttrTest::get_TestTypeArray()
		.set instance void SecurityDeclarations.SecurityAttrTest::set_TestTypeArray(class [mscorlib]System.Type[])
	}

} // end of class SecurityDeclarations.SecurityAttrTest

.class private auto ansi beforefieldinit SecurityDeclarations.TestStringTypes
	extends [mscorlib]System.Object
{
	.permissionset assert = {
		class 'SecurityDeclarations.SecurityAttrTest, SecurityDeclarations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' = {
			field string TestString = string('Hello World!')
			field object TestBoxedString = object(string('Boxed String'))
			property string[] TestStringArray = string[2]('a' 'b')
			field object TestBoxedArray = object(string[2]('c' 'd'))
		}
	}
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method TestStringTypes::.ctor

} // end of class SecurityDeclarations.TestStringTypes

.class private auto ansi beforefieldinit SecurityDeclarations.TestTypeTypes
	extends [mscorlib]System.Object
{
	.permissionset demand = {
		class 'SecurityDeclarations.SecurityAttrTest, SecurityDeclarations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' = {
			field type TestType = type(SecurityDeclarations.SimpleType)
			field object TestBoxed = object(type(SecurityDeclarations.TestEnum))
			property type[] TestTypeArray = type[2](SecurityDeclarations.TestStruct SecurityDeclarations.SimpleType)
			field object TestBoxedArray = object(type[2](SecurityDeclarations.TestStringTypes SecurityDeclarations.TestTypeTypes))
		}
	}
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method TestTypeTypes::.ctor

} // end of class SecurityDeclarations.TestTypeTypes

.class private auto ansi beforefieldinit SecurityDeclarations.TestEnumTypes
	extends [mscorlib]System.Object
{
	.permissionset inheritcheck = {
		class 'SecurityDeclarations.SecurityAttrTest, SecurityDeclarations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' = {
			field enum SecurityDeclarations.TestEnum TestEnumType = int32(0)
			field object TestBoxed = object(int32(1))
			property enum SecurityDeclarations.TestEnum[] TestEnumArray = int32[3](0 1 2)
			field object TestBoxed2 = object(object[4](int32(0) int32(1) int32(2) object[1](int32(3))))
			field object TestBoxedArray = object(int32[3](0 1 2))
		}
	}
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method TestEnumTypes::.ctor

} // end of class SecurityDeclarations.TestEnumTypes

.class private auto ansi beforefieldinit SecurityDeclarations.TestInt32Types
	extends [mscorlib]System.Object
{
	.permissionset permitonly = {
		class 'SecurityDeclarations.SecurityAttrTest, SecurityDeclarations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' = {
			field int32 TestInt32 = int32(5)
			field object TestBoxed = object(int32(10))
			property int32[] TestInt32Array = int32[3](1 2 3)
			field object TestBoxedArray = object(int32[3](4 5 6))
			field object TestBoxed2 = object(object[3](int32(7) int32(8) int32(9)))
		}
	}
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method TestInt32Types::.ctor

} // end of class SecurityDeclarations.TestInt32Types

.class private auto ansi beforefieldinit SecurityDeclarations.NestedArrays
	extends [mscorlib]System.Object
{
	.permissionset assert = {
		class 'SecurityDeclarations.SecurityAttrTest, SecurityDeclarations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' = {
			field object TestBoxed2 = object(object[4](int32(1) int32(2) int32(3) object[3](int32(4) int32(5) int32(6))))
		}
	}
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method NestedArrays::.ctor

} // end of class SecurityDeclarations.NestedArrays

