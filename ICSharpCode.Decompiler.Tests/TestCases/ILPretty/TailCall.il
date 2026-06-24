#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly TailCall { }

.class private auto ansi beforefieldinit TailCall
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig static int32 Callee (int32 x) cil managed
    {
        .maxstack 8
        ldarg.0
        ret
    }

    .method public hidebysig static int32 Caller (int32 x) cil managed
    {
        .maxstack 8
        ldarg.0
        tail.
        call int32 TailCall::Callee(int32)
        ret
    }

    .method public hidebysig specialname rtspecialname
        instance void .ctor () cil managed
    {
        .maxstack 8
        ldarg.0
        call instance void [CORE_ASSEMBLY]System.Object::.ctor()
        ret
    }
}
