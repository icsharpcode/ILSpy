#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly IndexerAccessorParameterNames { }

.class public auto ansi beforefieldinit IndexerAccessorParameterNames
    extends [CORE_ASSEMBLY]System.Object
{
    .custom instance void [CORE_ASSEMBLY]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )

    // The indexer's parameter names are the getter's: x and y.
    .method public hidebysig specialname instance int32 get_Item (int32 x, int32 y) cil managed
    {
        .maxstack 8
        ldarg.1
        ret
    }

    // The setter is free to name the same parameters differently, which C# cannot express.
    .method public hidebysig specialname instance void set_Item (int32 a, int32 b, int32 'value') cil managed
    {
        .maxstack 8
        ret
    }

    .property instance int32 Item(int32, int32)
    {
        .get instance int32 IndexerAccessorParameterNames::get_Item(int32, int32)
        .set instance void IndexerAccessorParameterNames::set_Item(int32, int32, int32)
    }

    .method private hidebysig instance int32 Get (int32 i) cil managed
    {
        .maxstack 8
        ldarg.1
        ret
    }

    // this[y: Get(1), x: Get(2)] = 3;
    .method public hidebysig instance void Use () cil managed
    {
        .maxstack 4
        .locals init (int32 V_0)
        ldarg.0
        ldarg.0
        ldc.i4.1
        call instance int32 IndexerAccessorParameterNames::Get(int32)
        stloc.0
        ldarg.0
        ldc.i4.2
        call instance int32 IndexerAccessorParameterNames::Get(int32)
        ldloc.0
        ldc.i4.3
        call instance void IndexerAccessorParameterNames::set_Item(int32, int32, int32)
        ret
    }

    .method public hidebysig specialname rtspecialname instance void .ctor () cil managed
    {
        .maxstack 8
        ldarg.0
        call instance void [CORE_ASSEMBLY]System.Object::.ctor()
        ret
    }
}
