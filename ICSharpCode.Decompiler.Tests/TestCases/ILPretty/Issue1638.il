// Regression fixture for issue #1638: yield-return state machine with obfuscated names.
//
// This is Roslyn /optimize codegen for
//     public IEnumerator Test(object arg1, object arg2)
//     {
//         try { yield return arg1; yield return state; }
//         finally { state = 0; }
//         yield return arg2;
//     }
// with an obfuscator-style rename pass applied: the state machine class, its fields,
// 'System.IDisposable.Dispose' and 'MoveNext' all carry meaningless names, and
// [IteratorStateMachine] is stripped. The MethodImpl (.override) rows are kept intact,
// so the decompiler must identify Dispose and MoveNext through them instead of by name.

.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly Issue1638
{
  .ver 0:0:0:0
}
.module Issue1638.dll

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638
	extends [System.Runtime]System.Object
{
	// Nested Types
	.class nested private auto ansi sealed beforefieldinit ObfuscatedIterator
		extends [System.Runtime]System.Object
		implements class [System.Runtime]System.Collections.Generic.IEnumerator`1<object>,
		           [System.Runtime]System.Collections.IEnumerator,
		           [System.Runtime]System.IDisposable
	{
		.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field private int32 m_state
		.field private object m_current
		.field public object m_1
		.field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638 m_owner
		.field public object m_2

		// Methods
		.method public hidebysig specialname rtspecialname
			instance void .ctor (
				int32 initialState
			) cil managed
		{
			.custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [System.Runtime]System.Object::.ctor()
			IL_0006: ldarg.0
			IL_0007: ldarg.1
			IL_0008: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
			IL_000d: ret
		} // end of method ObfuscatedIterator::.ctor

		.method private final hidebysig newslot virtual
			instance void ObfuscatedDispose () cil managed
		{
			.custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [System.Runtime]System.IDisposable::Dispose()
			.maxstack 2
			.locals init (
				[0] int32
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
			IL_0006: stloc.0
			IL_0007: ldloc.0
			IL_0008: ldc.i4.s -3
			IL_000a: beq.s IL_0012

			IL_000c: ldloc.0
			IL_000d: ldc.i4.1
			IL_000e: sub
			IL_000f: ldc.i4.1
			IL_0010: bgt.un.s IL_001c

			IL_0012: nop
			.try
			{
				IL_0013: leave.s IL_001c
			} // end .try
			finally
			{
				IL_0015: ldarg.0
				IL_0016: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::ObfuscatedFinally()
				IL_001b: endfinally
			} // end handler

			IL_001c: ldarg.0
			IL_001d: ldc.i4.s -2
			IL_001f: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
			IL_0024: ret
		} // end of method ObfuscatedIterator::ObfuscatedDispose

		.method private final hidebysig newslot virtual
			instance bool ObfuscatedMoveNext () cil managed
		{
			.override method instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
			.maxstack 2
			.locals init (
				[0] bool,
				[1] int32,
				[2] class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638
			)

			.try
			{
				IL_0000: ldarg.0
				IL_0001: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_0006: stloc.1
				IL_0007: ldarg.0
				IL_0008: ldfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_owner
				IL_000d: stloc.2
				IL_000e: ldloc.1
				IL_000f: switch (IL_002b, IL_0051, IL_0075, IL_009a)

				IL_0024: ldc.i4.0
				IL_0025: stloc.0
				IL_0026: leave IL_00ac

				IL_002b: ldarg.0
				IL_002c: ldc.i4.m1
				IL_002d: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_0032: ldarg.0
				IL_0033: ldc.i4.s -3
				IL_0035: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_003a: ldarg.0
				IL_003b: ldarg.0
				IL_003c: ldfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_1
				IL_0041: stfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_current
				IL_0046: ldarg.0
				IL_0047: ldc.i4.1
				IL_0048: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_004d: ldc.i4.1
				IL_004e: stloc.0
				IL_004f: leave.s IL_00ac

				IL_0051: ldarg.0
				IL_0052: ldc.i4.s -3
				IL_0054: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_0059: ldarg.0
				IL_005a: ldloc.2
				IL_005b: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638::state
				IL_0060: box [System.Runtime]System.Int32
				IL_0065: stfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_current
				IL_006a: ldarg.0
				IL_006b: ldc.i4.2
				IL_006c: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_0071: ldc.i4.1
				IL_0072: stloc.0
				IL_0073: leave.s IL_00ac

				IL_0075: ldarg.0
				IL_0076: ldc.i4.s -3
				IL_0078: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_007d: ldarg.0
				IL_007e: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::ObfuscatedFinally()
				IL_0083: ldarg.0
				IL_0084: ldarg.0
				IL_0085: ldfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_2
				IL_008a: stfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_current
				IL_008f: ldarg.0
				IL_0090: ldc.i4.3
				IL_0091: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_0096: ldc.i4.1
				IL_0097: stloc.0
				IL_0098: leave.s IL_00ac

				IL_009a: ldarg.0
				IL_009b: ldc.i4.m1
				IL_009c: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
				IL_00a1: ldc.i4.0
				IL_00a2: stloc.0
				IL_00a3: leave.s IL_00ac
			} // end .try
			fault
			{
				IL_00a5: ldarg.0
				IL_00a6: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::ObfuscatedDispose()
				IL_00ab: endfinally
			} // end handler

			IL_00ac: ldloc.0
			IL_00ad: ret
		} // end of method ObfuscatedIterator::ObfuscatedMoveNext

		.method private hidebysig
			instance void ObfuscatedFinally () cil managed
		{
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldc.i4.m1
			IL_0002: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_state
			IL_0007: ldarg.0
			IL_0008: ldfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_owner
			IL_000d: ldc.i4.0
			IL_000e: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638::state
			IL_0013: ret
		} // end of method ObfuscatedIterator::ObfuscatedFinally

		.method private final hidebysig specialname newslot virtual
			instance object 'System.Collections.Generic.IEnumerator<System.Object>.get_Current' () cil managed
		{
			.custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<object>::get_Current()
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_current
			IL_0006: ret
		} // end of method ObfuscatedIterator::'System.Collections.Generic.IEnumerator<System.Object>.get_Current'

		.method private final hidebysig newslot virtual
			instance void System.Collections.IEnumerator.Reset () cil managed
		{
			.custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [System.Runtime]System.Collections.IEnumerator::Reset()
			.maxstack 8

			IL_0000: newobj instance void [System.Runtime]System.NotSupportedException::.ctor()
			IL_0005: throw
		} // end of method ObfuscatedIterator::System.Collections.IEnumerator.Reset

		.method private final hidebysig specialname newslot virtual
			instance object System.Collections.IEnumerator.get_Current () cil managed
		{
			.custom instance void [System.Runtime]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance object [System.Runtime]System.Collections.IEnumerator::get_Current()
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_current
			IL_0006: ret
		} // end of method ObfuscatedIterator::System.Collections.IEnumerator.get_Current

		// Properties
		.property instance object 'System.Collections.Generic.IEnumerator<System.Object>.Current'()
		{
			.get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::'System.Collections.Generic.IEnumerator<System.Object>.get_Current'()
		}
		.property instance object System.Collections.IEnumerator.Current()
		{
			.get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::System.Collections.IEnumerator.get_Current()
		}

	} // end of class ObfuscatedIterator


	// Fields
	.field private int32 state

	// Methods
	.method public hidebysig
		instance class [System.Runtime]System.Collections.IEnumerator Test (
			object arg1,
			object arg2
		) cil managed
	{
		.maxstack 8

		IL_0000: ldc.i4.0
		IL_0001: newobj instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::.ctor(int32)
		IL_0006: dup
		IL_0007: ldarg.0
		IL_0008: stfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_owner
		IL_000d: dup
		IL_000e: ldarg.1
		IL_000f: stfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_1
		IL_0014: dup
		IL_0015: ldarg.2
		IL_0016: stfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638/ObfuscatedIterator::m_2
		IL_001b: ret
	} // end of method Issue1638::Test

	.method public hidebysig specialname rtspecialname
		instance void .ctor () cil managed
	{
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: ret
	} // end of method Issue1638::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1638
