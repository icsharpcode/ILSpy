// Anonymous methods whose delegate signature contains a type the use site cannot name.
// IL (unlike C#) permits a public delegate with a less-accessible parameter type, so the
// parameter-list-less "delegate {}" form is the only C# syntax the consumer class below
// could legally have used - the decompiler must not expand it to a lambda parameter list.
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly InaccessibleParameterTypes
{
  .ver 1:0:0:0
}
.module InaccessibleParameterTypes.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes
	extends [mscorlib]System.Object
{
	.class nested private auto ansi beforefieldinit Hidden
		extends [mscorlib]System.Object
	{
		.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
		{
			.maxstack 8

			ldarg.0
			call instance void [mscorlib]System.Object::.ctor()
			ret
		}
	}

	.class nested public auto ansi sealed Handler
		extends [mscorlib]System.MulticastDelegate
	{
		.method public hidebysig specialname rtspecialname instance void .ctor (object 'object', native int 'method') runtime managed
		{
		}

		.method public hidebysig newslot virtual instance void Invoke (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden h) runtime managed
		{
		}
	}

	.method public hidebysig static void Register (class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden> callback) cil managed
	{
		.maxstack 8

		ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8

		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer
	extends [mscorlib]System.Object
{
	// Fields
	.field private static class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler '<>f__am$cache0'
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
	.field private static class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden> '<>f__am$cache1'
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

	// Methods
	.method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler Create () cil managed
	{
		.maxstack 8

		ldsfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache0'
		brtrue.s IL_0016

		ldnull
		ldftn void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<Create>m__0'(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden)
		newobj instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler::.ctor(object, native int)
		stsfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache0'

		IL_0016: ldsfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Handler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache0'
		ret
	}

	.method public hidebysig instance void Run () cil managed
	{
		.maxstack 8

		ldsfld class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache1'
		brtrue.s IL_0016

		ldnull
		ldftn void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<Run>m__1'(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden)
		newobj instance void class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden>::.ctor(object, native int)
		stsfld class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache1'

		IL_0016: ldsfld class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypesConsumer::'<>f__am$cache1'
		call void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes::Register(class [System.Core]System.Action`1<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden>)
		ret
	}

	.method private hidebysig static void '<Create>m__0' (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden '') cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
		.maxstack 8

		ret
	}

	.method private hidebysig static void '<Run>m__1' (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.InaccessibleParameterTypes/Hidden '') cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
		.maxstack 8

		ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8

		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}
}
