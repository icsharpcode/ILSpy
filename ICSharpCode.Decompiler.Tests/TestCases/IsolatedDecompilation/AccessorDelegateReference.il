#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}

.assembly AccessorDelegateReference { }

.class private auto ansi beforefieldinit C
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig specialname rtspecialname instance void .ctor () cil managed
    {
        .maxstack 1
        ldarg.0
        call instance void [CORE_ASSEMBLY]System.Object::.ctor()
        ret
    }

    .method private hidebysig specialname instance int32 get_P () cil managed
    {
        .maxstack 1
        ldc.i4.0
        ret
    }

    .property instance int32 P ()
    {
        .get instance int32 C::get_P()
    }

    .method private hidebysig static class [CORE_ASSEMBLY]System.Func`1<int32> GetGetter () cil managed
    {
        .maxstack 2
        newobj instance void C::.ctor()
        ldftn instance int32 C::get_P()
        newobj instance void class [CORE_ASSEMBLY]System.Func`1<int32>::.ctor(object, native int)
        ret
    }
}
