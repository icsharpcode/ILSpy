// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 9:0:0:0
}
.assembly StackAllocDuplicateStore
{
  .ver 1:0:0:0
}
.module StackAllocDuplicateStore.dll
.corflags 0x00000001    //  ILONLY

// A stackalloc initializer that writes the first element twice. The sequential-store
// initializer transform must not fold this into a 'stackalloc int[] { ... }': the earlier
// store would be dropped, which is observable when its value has side effects.

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.StackAllocDuplicateStore
       extends [System.Runtime]System.Object
{
  .method public hidebysig static int32  Seq(int32 a,
                                             int32 b,
                                             int32 c) cil managed
  {
    .maxstack  4
    .locals init (valuetype [System.Runtime]System.Span`1<int32> V_0)
    IL_0000:  ldc.i4.s   12
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.s   99
    IL_000b:  stind.i4
    IL_000c:  dup
    IL_000d:  ldc.i4.4
    IL_000e:  add
    IL_000f:  ldarg.1
    IL_0010:  stind.i4
    IL_0011:  dup
    IL_0012:  ldc.i4.2
    IL_0013:  conv.i
    IL_0014:  ldc.i4.4
    IL_0015:  mul
    IL_0016:  add
    IL_0017:  ldarg.2
    IL_0018:  stind.i4
    IL_0019:  ldc.i4.3
    IL_001a:  newobj     instance void valuetype [System.Runtime]System.Span`1<int32>::.ctor(void*,
                                                                                             int32)
    IL_001f:  stloc.0
    IL_0020:  ldloca.s   V_0
    IL_0022:  ldc.i4.0
    IL_0023:  call       instance !0& valuetype [System.Runtime]System.Span`1<int32>::get_Item(int32)
    IL_0028:  ldind.i4
    IL_0029:  ldloca.s   V_0
    IL_002b:  ldc.i4.1
    IL_002c:  call       instance !0& valuetype [System.Runtime]System.Span`1<int32>::get_Item(int32)
    IL_0031:  ldind.i4
    IL_0032:  add
    IL_0033:  ldloca.s   V_0
    IL_0035:  ldc.i4.2
    IL_0036:  call       instance !0& valuetype [System.Runtime]System.Span`1<int32>::get_Item(int32)
    IL_003b:  ldind.i4
    IL_003c:  add
    IL_003d:  ret
  } // end of method StackAllocDuplicateStore::Seq

} // end of class StackAllocDuplicateStore
