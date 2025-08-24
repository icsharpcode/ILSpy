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
    .hash algorithm 0x00008004 // SHA1
    .ver 0:0:0:0
}

.class public auto ansi beforefieldinit C
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig specialname static 
        int32 get_X () cil managed 
    {
        // Method begins at RVA 0x2050
        // Code size 6 (0x6)
        .maxstack 8

        IL_0000: ldc.i4 32
        IL_0005: ret
    } // end of method C::get_X

    .method public hidebysig specialname static 
        void set_X (
            int32 'value'
        ) cil managed 
    {
        // Method begins at RVA 0x2058
        // Code size 1 (0x1)
        .maxstack 8

        IL_0000: ret
    } // end of method C::set_X

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2068
        // Code size 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method C::.ctor

    .method private hidebysig specialname rtspecialname static 
        void .cctor () cil managed 
    {
        // Method begins at RVA 0x2074
        // Code size 11 (0xb)
        .maxstack 8

        IL_0000: ldc.i4 1
        IL_0005: call void C::set_X(int32)
        IL_000a: ret
    } // end of method C::.cctor

    // Properties
    .property int32 X()
    {
        .get int32 C::get_X()
        .set void C::set_X(int32)
    }

} // end of class C

