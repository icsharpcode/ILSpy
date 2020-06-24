#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.class private auto ansi beforefieldinit CallIndirect
    extends [System.Private.CoreLib]System.Object
{
    // Methods
    .method private hidebysig 
        instance void Test (
            native int f
        ) cil managed 
    {
        .maxstack 8

        ldc.i4.s 42
        ldarg.1
        calli unmanaged stdcall void(int32)
        ret
    } // end of method Example::Test

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x206e
        // Code size 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Private.CoreLib]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method Example::.ctor

} // end of class Example
