.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit Issue3465.Program
    extends [System.Runtime]System.Object
{
    // Fields
    .field private static class Issue3465.Program programNull

    // Methods
    .method private hidebysig static 
        class Issue3465.Program GetProgram () cil managed 
    {
        // Method begins at RVA 0x2050
        // Header size: 1
        // Code size: 2 (0x2)
        .maxstack 8

        IL_0000: ldnull
        IL_0001: ret
    } // end of method Issue3465.Program::GetProgram

    .method private hidebysig static 
        bool Test3465 () cil managed 
    {
        // Method begins at RVA 0x2054
        // Header size: 12
        // Code size: 7 (0x7)
        .maxstack 1
        .locals init (
            [0] bool
        )

        IL_0000: call class Issue3465.Program Issue3465.Program::GetProgram()
        IL_0001: ldsfld class Issue3465.Program Issue3465.Program::programNull
                 cgt.un
        IL_0002: stloc.0
        IL_0003: br.s IL_0005

        IL_0005: ldloc.0
        IL_0006: ret
    } // end of method Program::Test3465

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2067
        // Header size: 1
        // Code size: 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method Issue3465.Program::.ctor

} // end of class Issue3465.Program

