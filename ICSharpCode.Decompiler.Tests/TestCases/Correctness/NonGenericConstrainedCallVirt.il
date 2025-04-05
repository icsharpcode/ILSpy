.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}

.assembly _
{
    .hash algorithm 0x00008004 // SHA1
    .ver 0:0:0:0
}

.class public auto ansi beforefieldinit NonGenericConstrainedCallVirt
    extends [mscorlib]System.Object
{
    // Methods
    .method public hidebysig static 
        void Main () cil managed 
    {
        // Method begins at RVA 0x2050
        // Code size 16 (0x10)
        .entrypoint
        .maxstack 8

        IL_0000: ldstr "A"
        IL_0005: newobj instance void C::.ctor(string)
        IL_000a: call void NonGenericConstrainedCallVirt::Foo(class C)
        IL_000f: ret
    } // end of method NonGenericConstrainedCallVirt::Main

    .method private hidebysig static 
        void Foo (
            class C arg
        ) cil managed 
    {
        // Method begins at RVA 0x2064
        // Code size 25 (0x19)
        .maxstack 8

        IL_0000: ldarga arg
        IL_0004: ldarga arg
        IL_0008: call int32 NonGenericConstrainedCallVirt::Bar(class C&)
        IL_000d: constrained. C
        IL_0013: callvirt instance void C::Baz(int32)
        IL_0018: ret
    } // end of method NonGenericConstrainedCallVirt::Foo

    .method private hidebysig static 
        int32 Bar (
            class C& o
        ) cil managed 
    {
        // Method begins at RVA 0x2080
        // Code size 14 (0xe)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldstr "B"
        IL_0006: newobj instance void C::.ctor(string)
        IL_000b: stind.ref
        IL_000c: ldc.i4.0
        IL_000d: ret
    } // end of method NonGenericConstrainedCallVirt::Bar

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2090
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [mscorlib]System.Object::.ctor()
        IL_0006: ret
    } // end of method NonGenericConstrainedCallVirt::.ctor

} // end of class NonGenericConstrainedCallVirt

.class public auto ansi beforefieldinit C
    extends [mscorlib]System.Object
{
    // Fields
    .field private initonly string '<Name>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    // Methods
    .method public hidebysig specialname rtspecialname 
        instance void .ctor (
            string n
        ) cil managed 
    {
        // Method begins at RVA 0x2098
        // Code size 14 (0xe)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [mscorlib]System.Object::.ctor()
        IL_0006: ldarg.0
        IL_0007: ldarg.1
        IL_0008: stfld string C::'<Name>k__BackingField'
        IL_000d: ret
    } // end of method C::.ctor

    .method public hidebysig 
        instance void Baz (
            int32 arg
        ) cil managed 
    {
        // Method begins at RVA 0x20a8
        // Code size 12 (0xc)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance string C::get_Name()
        IL_0006: call void [mscorlib]System.Console::WriteLine(string)
        IL_000b: ret
    } // end of method C::Baz

    .method public hidebysig specialname 
        instance string get_Name () cil managed 
    {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        // Method begins at RVA 0x20b8
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: ldfld string C::'<Name>k__BackingField'
        IL_0006: ret
    } // end of method C::get_Name

    // Properties
    .property instance string Name()
    {
        .get instance string C::get_Name()
    }

} // end of class C

