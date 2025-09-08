.class public auto ansi abstract sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig static 
        valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair MakePair1 (
            int32 x,
            int32 y
        ) cil managed 
    {
        // Method begins at RVA 0x2050
        // Header size: 12
        // Code size: 34 (0x22)
        .maxstack 2
        .locals init (
            [0] valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        )

        IL_0000: ldloca.s 0
        IL_0002: initobj ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        IL_0008: ldloca.s 0
        IL_000a: ldarg.0
        IL_000b: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_0010: ldloca.s 0
        IL_0012: ldarg.1
        IL_0013: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_0015: ldloca.s 0
        IL_001c: call instance valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::ToPair()
        IL_0021: ret
    } // end of method Issue3552::MakePair1

    .method public hidebysig static 
        valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair MakePair2 (
            int32 x,
            int32 y
        ) cil managed 
    {
        // Method begins at RVA 0x2050
        // Header size: 12
        // Code size: 34 (0x22)
        .maxstack 2
        .locals init (
            [0] valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        )

        IL_0000: ldloca.s 0
        IL_0001: dup
        IL_0002: initobj ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        IL_0008: dup
        IL_000a: ldarg.0
        IL_000b: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_0012: ldarg.1
        IL_0013: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_0015: ldloca.s 0
        IL_001c: call instance valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::ToPair()
        IL_0021: ret
    } // end of method Issue3552::MakePair2

    .method public hidebysig static 
        valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair MakePair3 (
            int32 x,
            int32 y
        ) cil managed 
    {
        // Method begins at RVA 0x2050
        // Header size: 12
        // Code size: 34 (0x22)
        .maxstack 2
        .locals init (
            [0] valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        )

        IL_0000: ldloca.s 0
        IL_0001: dup
        IL_0002: initobj ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
        IL_0008: dup
        IL_000a: ldarg.0
        IL_000b: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_0010: dup
        IL_0012: ldarg.1
        IL_0013: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::Add(int32)
        IL_001c: call instance valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::ToPair()
        IL_0021: ret
    } // end of method Issue3552::MakePair3

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair
    extends [System.Runtime]System.ValueType
{
    // Fields
    .field public int32 X
    .field public int32 Y

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
    extends [System.Runtime]System.ValueType
    implements class [System.Runtime]System.Collections.Generic.IEnumerable`1<int32>,
               [System.Runtime]System.Collections.IEnumerable
{
    // Fields
    .field private int32 index
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair pair

    // Methods
    .method public hidebysig 
        instance valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ToPair () cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (
            01 00 00 00
        )
        // Method begins at RVA 0x207e
        // Header size: 1
        // Code size: 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::pair
        IL_0006: ret
    } // end of method Issue3552_IntegerPairBuilder::ToPair

    .method public hidebysig 
        instance void Add (
            int32 'value'
        ) cil managed 
    {
        // Method begins at RVA 0x2088
        // Header size: 12
        // Code size: 65 (0x41)
        .maxstack 3
        .locals init (
            [0] int32
        )

        IL_0000: ldarg.0
        IL_0001: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::index
        IL_0006: stloc.0
        IL_0007: ldloc.0
        IL_0008: brfalse.s IL_0010

        IL_000a: ldloc.0
        IL_000b: ldc.i4.1
        IL_000c: beq.s IL_001e

        IL_000e: br.s IL_002c

        IL_0010: ldarg.0
        IL_0011: ldflda valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::pair
        IL_0016: ldarg.1
        IL_0017: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair::X
        IL_001c: br.s IL_0032

        IL_001e: ldarg.0
        IL_001f: ldflda valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::pair
        IL_0024: ldarg.1
        IL_0025: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPair::Y
        IL_002a: br.s IL_0032

        IL_002c: newobj instance void [System.Runtime]System.IndexOutOfRangeException::.ctor()
        IL_0031: throw

        IL_0032: ldarg.0
        IL_0033: ldarg.0
        IL_0034: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::index
        IL_0039: ldc.i4.1
        IL_003a: add
        IL_003b: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::index
        IL_0040: ret
    } // end of method Issue3552_IntegerPairBuilder::Add

    .method public final hidebysig newslot virtual 
        instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> GetEnumerator () cil managed 
    {
        // Method begins at RVA 0x20d5
        // Header size: 1
        // Code size: 2 (0x2)
        .maxstack 8

        IL_0000: ldnull
        IL_0001: ret
    } // end of method Issue3552_IntegerPairBuilder::GetEnumerator

    .method private final hidebysig newslot virtual 
        instance class [System.Runtime]System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () cil managed 
    {
        .override method instance class [System.Runtime]System.Collections.IEnumerator [System.Runtime]System.Collections.IEnumerable::GetEnumerator()
        // Method begins at RVA 0x20d8
        // Header size: 1
        // Code size: 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance class [System.Runtime]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder::GetEnumerator()
        IL_0006: ret
    } // end of method Issue3552_IntegerPairBuilder::System.Collections.IEnumerable.GetEnumerator

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3552_IntegerPairBuilder
