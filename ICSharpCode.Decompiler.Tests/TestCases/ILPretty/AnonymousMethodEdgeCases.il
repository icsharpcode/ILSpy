// Hand-written input for two anonymous-method shapes that C# cannot express directly:
// an expression-bodied lambda whose body is the assignment declaring its own local, and
// a parameter whose metadata name is not a valid C# identifier but IS used in the body.
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly AnonymousMethodEdgeCases
{
  .ver 1:0:0:0
}
.module AnonymousMethodEdgeCases.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003
.corflags 0x00000001

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AnonymousMethodEdgeCases
	extends [mscorlib]System.Object
{
	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8
		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}

	.method public hidebysig instance class [System.Core]System.Func`2<int32, int32> AssignmentIsTheLambdaBody () cil managed
	{
		.maxstack 8
		ldarg.0
		ldftn instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AnonymousMethodEdgeCases::'<AssignmentIsTheLambdaBody>b__1_0'(int32)
		newobj instance void class [System.Core]System.Func`2<int32, int32>::.ctor(object, native int)
		ret
	}

	// return num = x; -- the store's value is the return value, so the local has a store and
	// no load, and the decompiled lambda body is the assignment itself.
	.method private hidebysig instance int32 '<AssignmentIsTheLambdaBody>b__1_0' (int32 x) cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
		.maxstack 2
		.locals init (
			[0] int32 num
		)
		ldarg.1
		dup
		stloc.0
		ret
	}

	.method public hidebysig instance class [System.Core]System.Action`1<object> UsedParameterWithInvalidName () cil managed
	{
		.maxstack 8
		ldarg.0
		ldftn instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AnonymousMethodEdgeCases::'<UsedParameterWithInvalidName>b__2_0'(object)
		newobj instance void class [System.Core]System.Action`1<object>::.ctor(object, native int)
		ret
	}

	// The parameter name '<p0>' is what C# emits for an anonymous method declared without a
	// parameter list; used here, so the lambda's parameter list has to name it somehow.
	.method private hidebysig instance void '<UsedParameterWithInvalidName>b__2_0' (object '<p0>') cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
		.maxstack 8
		ldarg.1
		call void [mscorlib]System.Console::WriteLine(object)
		ret
	}
}
