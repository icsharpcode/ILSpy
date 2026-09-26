.assembly _
{
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
        01 00 08 00 00 00 00 00
    )
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
        01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
        63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
    )
    .hash algorithm 0x00008004 // SHA1
    .ver 0:0:0:0
}

.class public auto ansi beforefieldinit PropertyBackingFieldWithoutCompilerGeneratedAttribute
    extends [System.Runtime]System.Object
{
    .field private initonly string '<Name>k__BackingField'

    .method public hidebysig specialname
        instance string get_Name () cil managed
    {
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld string PropertyBackingFieldWithoutCompilerGeneratedAttribute::'<Name>k__BackingField'
        IL_0006: ret
    }

    .method public hidebysig specialname rtspecialname
        instance void .ctor (
            string name
        ) cil managed
    {
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: stfld string PropertyBackingFieldWithoutCompilerGeneratedAttribute::'<Name>k__BackingField'
        IL_000d: ret
    }

    .property instance string Name()
    {
        .get instance string PropertyBackingFieldWithoutCompilerGeneratedAttribute::get_Name()
    }
}
