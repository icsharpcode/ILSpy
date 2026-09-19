#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly ParamsPropertySetter { }

.class public auto ansi beforefieldinit ParamsPropertySetter
    extends [CORE_ASSEMBLY]System.Object
{
    .field private int32[] 'values'

    .method public hidebysig specialname instance int32[] get_Values () cil managed
    {
        .maxstack 8
        ldarg.0
        ldfld int32[] ParamsPropertySetter::'values'
        ret
    }

    // C# cannot declare a property whose value is a parameter array, and an assignment has no
    // argument list to expand one into.
    .method public hidebysig specialname instance void set_Values (int32[] 'value') cil managed
    {
        .param [1]
        .custom instance void [CORE_ASSEMBLY]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 )
        .maxstack 8
        ldarg.0
        ldarg.1
        stfld int32[] ParamsPropertySetter::'values'
        ret
    }

    .property instance int32[] Values()
    {
        .get instance int32[] ParamsPropertySetter::get_Values()
        .set instance void ParamsPropertySetter::set_Values(int32[])
    }

    .method public hidebysig instance void Use () cil managed
    {
        .maxstack 4
        ldarg.0
        ldc.i4.2
        newarr [CORE_ASSEMBLY]System.Int32
        dup
        ldc.i4.0
        ldc.i4.1
        stelem.i4
        dup
        ldc.i4.1
        ldc.i4.2
        stelem.i4
        call instance void ParamsPropertySetter::set_Values(int32[])
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
