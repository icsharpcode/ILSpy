#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly ParameterizedPropertyInitializer { }

// A parameterized property that is not an indexer: the type carries no DefaultMemberAttribute,
// which C# cannot express, but VB, C++/CLI and COM interop all produce it.
.class public auto ansi beforefieldinit ParameterizedPropertyInitializer
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig specialname instance int32 get_Foo (int32 x) cil managed
    {
        .maxstack 8
        ldarg.1
        ret
    }

    .method public hidebysig specialname instance void set_Foo (int32 x, int32 'value') cil managed
    {
        .maxstack 8
        ret
    }

    .property instance int32 Foo(int32)
    {
        .get instance int32 ParameterizedPropertyInitializer::get_Foo(int32)
        .set instance void ParameterizedPropertyInitializer::set_Foo(int32, int32)
    }

    .method public hidebysig static void Consume (class ParameterizedPropertyInitializer p) cil managed
    {
        .maxstack 8
        ret
    }

    .method public hidebysig static void Use () cil managed
    {
        .maxstack 8
        newobj instance void ParameterizedPropertyInitializer::.ctor()
        dup
        ldc.i4.7
        ldc.i4.5
        call instance void ParameterizedPropertyInitializer::set_Foo(int32, int32)
        call void ParameterizedPropertyInitializer::Consume(class ParameterizedPropertyInitializer)
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
