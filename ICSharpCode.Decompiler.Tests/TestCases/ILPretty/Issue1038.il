// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ConsoleApp11
{
  .ver 1:0:0:0
}
.module ConsoleApp11.exe
// MVID: {B973FCD6-A9C4-48A9-8291-26DDC248E208}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x000001C4B6C90000

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<TK, class .ctor TR>
{
	// Fields
	.field private class [System.Core]System.Action`2<!TK, !TR> TestEvent
	.field private static class [System.Core]System.Action`2<!TK, !TR> '<>f__am$cache0'
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

	// Methods
	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8

		ldarg.0
		ldsfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::'<>f__am$cache0'
		brtrue.s IL_0019

		ldnull
		ldftn void class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::'<TestEvent>m__0'(!0, !1)
		newobj instance void class [System.Core]System.Action`2<!TK, !TR>::.ctor(object, native int)
		stsfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::'<>f__am$cache0'

		IL_0019: ldsfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::'<>f__am$cache0'
		stfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::TestEvent
		ret
	}

	.method public hidebysig specialname instance void add_TestEvent (class [System.Core]System.Action`2<!TK, !TR> 'value') cil managed 
	{
		.maxstack 3
		.locals init (
			[0] class [System.Core]System.Action`2<!TK, !TR>,
			[1] class [System.Core]System.Action`2<!TK, !TR>
		)

		ldarg.0
		ldfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::TestEvent
		stloc.0
			IL_0007: ldloc.0
			stloc.1
			ldarg.0
			ldflda class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::TestEvent
			ldloc.1
			ldarg.1
			call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
			castclass class [System.Core]System.Action`2<!TK, !TR>
			ldloc.0
			call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [System.Core]System.Action`2<!TK, !TR>>(!!0&, !!0, !!0)
			stloc.0
			ldloc.0
			ldloc.1
			bne.un IL_0007
		ret
	}

	.method public hidebysig specialname instance void remove_TestEvent (class [System.Core]System.Action`2<!TK, !TR> 'value') cil managed 
	{
		.maxstack 3
		.locals init (
			[0] class [System.Core]System.Action`2<!TK, !TR>,
			[1] class [System.Core]System.Action`2<!TK, !TR>
		)

		ldarg.0
		ldfld class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::TestEvent
		stloc.0
			IL_0007: ldloc.0
			stloc.1
			ldarg.0
			ldflda class [System.Core]System.Action`2<!0, !1> class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2<!TK, !TR>::TestEvent
			ldloc.1
			ldarg.1
			call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
			castclass class [System.Core]System.Action`2<!TK, !TR>
			ldloc.0
			call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [System.Core]System.Action`2<!TK, !TR>>(!!0&, !!0, !!0)
			stloc.0
			ldloc.0
			ldloc.1
			bne.un IL_0007
		ret
	}

	.method private hidebysig static void '<TestEvent>m__0' (!TK '', !TR '') cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
		.maxstack 8
		ret
	}

	// Events
	.event class [System.Core]System.Action`2<!TK, !TR> TestEvent
	{
		.addon instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2::add_TestEvent(class [System.Core]System.Action`2<!0, !1>)
		.removeon instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1038`2::remove_TestEvent(class [System.Core]System.Action`2<!0, !1>)
	}
}