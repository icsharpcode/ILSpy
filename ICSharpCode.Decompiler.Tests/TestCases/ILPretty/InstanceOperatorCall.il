// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 11:0:0:0
}
.assembly InstanceOperatorCall
{
  .ver 1:0:0:0
}
.module InstanceOperatorCall.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY

// F# emits comparison operators as instance members, so operator metadata names appear on
// methods C# would never bind as operators: the operands are the arguments, and the operator
// lives on the receiver type rather than on either operand type.
.class public auto ansi beforefieldinit Source`1<T>
       extends [System.Runtime]System.Object
{
  .method public hidebysig specialname newslot virtual
          instance bool  op_LessThan(!T a,
                                     !T b) cil managed
  {
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  }

  .method public hidebysig specialname
          instance bool  op_GreaterThan(!T a,
                                        !T b) cil managed
  {
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit CallSite
       extends [System.Runtime]System.Object
{
  .method public hidebysig static bool  CompareVirtual<T>(class Source`1<!!T> s,
                                                          !!T a,
                                                          !!T b) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  callvirt   instance bool class Source`1<!!T>::op_LessThan(!0,
                                                                        !0)
    IL_0008:  ret
  }

  .method public hidebysig static bool  CompareDirect(class Source`1<int32> s,
                                                      int32 a,
                                                      int32 b) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  call       instance bool class Source`1<int32>::op_GreaterThan(!0,
                                                                             !0)
    IL_0008:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}
