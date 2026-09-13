#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly ParameterizedPropertySetterCall { }

// A parameterized property whose setter takes an index and the assigned value. It is not the
// type's default member, so there is no access syntax for it and the accessor is written as a
// call - which means the assigned value is an ordinary argument, and the optional index before
// it is not trailing.
.class public auto ansi beforefieldinit ParameterizedPropertySetterCall
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig specialname instance int32 get_P (int32 i) cil managed
    {
        .maxstack 8
        ldarg.1
        ret
    }

    .method public hidebysig specialname instance void set_P ([opt] int32 i, int32 'value') cil managed
    {
        .param [1] = int32(0x00000000)
        .maxstack 8
        ret
    }

    .property instance int32 P(int32)
    {
        .get instance int32 ParameterizedPropertySetterCall::get_P(int32)
        .set instance void ParameterizedPropertySetterCall::set_P(int32, int32)
    }

    .method public hidebysig instance void Use () cil managed
    {
        .maxstack 8
        ldarg.0
        ldc.i4.0
        ldc.i4.5
        call instance void ParameterizedPropertySetterCall::set_P(int32, int32)
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
