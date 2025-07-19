.assembly _
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
        01 00 08 00 00 00 00 00
    )
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
        01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
        63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
    )
    .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
        01 00 07 01 00 00 00 00
    )
    .custom instance void MyAttribute::.ctor(string, class [System.Runtime]System.Type) = (
        01 00 04 61 73 64 66 0E 41 42 43 44 60 31 3C 73 74 72 69 6E 67 3E 00 00
    )
    .hash algorithm 0x00008004 // SHA1
    .ver 0:0:0:0
}

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit MyAttribute
    extends [System.Runtime]System.Attribute
{
    // Methods
    .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            string x,
            class [System.Runtime]System.Type t
        ) cil managed 
    {
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 01 00 00
        )
        // Method begins at RVA 0x2050
        // Code size 9 (0x9)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Attribute::.ctor()
        IL_0006: nop
        IL_0007: nop
        IL_0008: ret
    } // end of method MyAttribute::.ctor

} // end of class MyAttribute

.class public auto ansi beforefieldinit 'ABCD`1<string>'
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x205a
        // Code size 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method ABCD::.ctor

} // end of class ABCD

