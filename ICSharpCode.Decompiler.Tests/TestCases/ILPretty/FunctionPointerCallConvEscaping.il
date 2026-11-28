#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}

.assembly FunctionPointerCallConvEscaping { }

.class public auto ansi beforefieldinit FunctionPointerCallConvEscaping
    extends [CORE_ASSEMBLY]System.Object
{
    .field public static method unmanaged cdecl void modopt([CORE_ASSEMBLY]System.Runtime.CompilerServices.'CallConvSuppress\r\033[2K\nGCTransition') *() Callback
}
