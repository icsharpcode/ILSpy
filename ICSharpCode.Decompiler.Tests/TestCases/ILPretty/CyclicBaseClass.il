// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CyclicBaseClass
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module CyclicBaseClass.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit CyclicBaseClass.SelfDerived
       extends CyclicBaseClass.SelfDerived
{
} // end of class CyclicBaseClass.SelfDerived

.class public abstract auto ansi beforefieldinit CyclicBaseClass.Expanding`1<T>
       extends class CyclicBaseClass.Expanding`1<class CyclicBaseClass.Expanding`1<!T>>
{
} // end of class CyclicBaseClass.Expanding`1

.class public abstract auto ansi sealed beforefieldinit CyclicBaseClass.Extensions
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
  .method public hidebysig static void  Deconstruct(class CyclicBaseClass.SelfDerived 'value',
                                                    [out] int32& a,
                                                    [out] int32& b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  stind.i4
    IL_0003:  ldarg.2
    IL_0004:  ldc.i4.2
    IL_0005:  stind.i4
    IL_0006:  ret
  } // end of method Extensions::Deconstruct

  .method public hidebysig static void  Deconstruct<T>(class CyclicBaseClass.Expanding`1<!!T> 'value',
                                                       [out] int32& a,
                                                       [out] int32& b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 )
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  stind.i4
    IL_0003:  ldarg.2
    IL_0004:  ldc.i4.2
    IL_0005:  stind.i4
    IL_0006:  ret
  } // end of method Extensions::Deconstruct

} // end of class CyclicBaseClass.Extensions

.class public abstract auto ansi beforefieldinit CyclicBaseClass.Pair`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot abstract virtual
          instance void  Deconstruct([out] int32& first,
                                     [out] !T& second) cil managed
  {
  } // end of method Pair`1::Deconstruct

  .method family hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Pair`1::.ctor

} // end of class CyclicBaseClass.Pair`1

.class public abstract auto ansi sealed beforefieldinit CyclicBaseClass.NestedDeconstruction
       extends [mscorlib]System.Object
{
  .method public hidebysig static int32  Sum(class CyclicBaseClass.Pair`1<class CyclicBaseClass.SelfDerived> pair) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             class CyclicBaseClass.SelfDerived V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_2
    IL_0003:  ldloca.s   V_3
    IL_0005:  callvirt   instance void class CyclicBaseClass.Pair`1<class CyclicBaseClass.SelfDerived>::Deconstruct(int32&,
                                                                                                                    !0&)
    IL_000a:  ldloc.3
    IL_000b:  ldloca.s   V_4
    IL_000d:  ldloca.s   V_5
    IL_000f:  call       void CyclicBaseClass.Extensions::Deconstruct(class CyclicBaseClass.SelfDerived,
                                                                      int32&,
                                                                      int32&)
    IL_0014:  ldloc.2
    IL_0015:  ldloc.s    V_4
    IL_0017:  stloc.0
    IL_0018:  ldloc.s    V_5
    IL_001a:  stloc.1
    IL_001b:  ldloc.0
    IL_001c:  add
    IL_001d:  ldloc.1
    IL_001e:  add
    IL_001f:  ret
  } // end of method NestedDeconstruction::Sum

  .method public hidebysig static int32  Sum(class CyclicBaseClass.Pair`1<class CyclicBaseClass.Expanding`1<int32>> pair) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             class CyclicBaseClass.Expanding`1<int32> V_3,
             int32 V_4,
             int32 V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_2
    IL_0003:  ldloca.s   V_3
    IL_0005:  callvirt   instance void class CyclicBaseClass.Pair`1<class CyclicBaseClass.Expanding`1<int32>>::Deconstruct(int32&,
                                                                                                                           !0&)
    IL_000a:  ldloc.3
    IL_000b:  ldloca.s   V_4
    IL_000d:  ldloca.s   V_5
    IL_000f:  call       void CyclicBaseClass.Extensions::Deconstruct<int32>(class CyclicBaseClass.Expanding`1<!!0>,
                                                                             int32&,
                                                                             int32&)
    IL_0014:  ldloc.2
    IL_0015:  ldloc.s    V_4
    IL_0017:  stloc.0
    IL_0018:  ldloc.s    V_5
    IL_001a:  stloc.1
    IL_001b:  ldloc.0
    IL_001c:  add
    IL_001d:  ldloc.1
    IL_001e:  add
    IL_001f:  ret
  } // end of method NestedDeconstruction::Sum

} // end of class CyclicBaseClass.NestedDeconstruction


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
