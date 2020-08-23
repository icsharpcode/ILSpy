.assembly issue2104
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
		01 00 08 00 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
		01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
		63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 07 01 00 00 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.module issue2104.exe
// MVID: {A7498FA1-F4D7-486E-A872-3DC0CAFD87E2}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = (
	01 00 00 00
)

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104
    extends [mscorlib]System.Object
{
    // Fields
    .field private initonly string 'text'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    // Methods
    .method public hidebysig specialname 
        instance string get_Text () cil managed 
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        // Method begins at RVA 0x2074
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104::text
        IL_0006: ret
    } // end of method ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104::get_Text

    .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            string text
        ) cil managed 
    {
        // Method begins at RVA 0x207c
        // Code size 14 (0xe)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [mscorlib]System.Object::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: stfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104::'text'
        IL_000d: ret
    } // end of method ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104::.ctor

    // Properties
    .property instance string Text()
    {
        .get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104::get_Text()
    }

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2104

