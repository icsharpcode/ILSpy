
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmpvpdwr1
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmpvpdwr1.tmp
// MVID: {8447adbb-757d-4626-b7b7-846d745d90d0}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1<.ctor TOnType>
       extends [mscorlib]System.Object
{
  .method public hidebysig static !TOnType 
          CreateTOnType() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  1
    .locals init (!TOnType V_0,
             !TOnType V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_1
    IL_0003:  initobj    !TOnType
    IL_0009:  ldloc.1
    IL_000a:  box        !TOnType
    IL_000f:  brfalse.s  IL_001c

    IL_0011:  ldloca.s   V_1
    IL_0013:  initobj    !TOnType
    IL_0019:  ldloc.1
    IL_001a:  br.s       IL_0021

    IL_001c:  call       !!0 [mscorlib]System.Activator::CreateInstance<!TOnType>()
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method NoNewOfT`1::CreateTOnType

  .method public hidebysig static !!T  CreateUnconstrainedT<.ctor T>() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  1
    .locals init (!!T V_0,
             !!T V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_1
    IL_0003:  initobj    !!T
    IL_0009:  ldloc.1
    IL_000a:  box        !!T
    IL_000f:  brfalse.s  IL_001c

    IL_0011:  ldloca.s   V_1
    IL_0013:  initobj    !!T
    IL_0019:  ldloc.1
    IL_001a:  br.s       IL_0021

    IL_001c:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method NoNewOfT`1::CreateUnconstrainedT

  .method public hidebysig static !!T  CreateClassT<class .ctor T>() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method NoNewOfT`1::CreateClassT

  .method public hidebysig static !!T  CollectionInitializer<.ctor (class [mscorlib]System.Collections.Generic.IList`1<int32>) T>() cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1,
             !!T V_2)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_2
    IL_0003:  initobj    !!T
    IL_0009:  ldloc.2
    IL_000a:  box        !!T
    IL_000f:  brfalse.s  IL_001c

    IL_0011:  ldloca.s   V_2
    IL_0013:  initobj    !!T
    IL_0019:  ldloc.2
    IL_001a:  br.s       IL_0021

    IL_001c:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  box        !!T
    IL_0029:  ldc.i4.1
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_002f:  nop
    IL_0030:  ldloc.0
    IL_0031:  box        !!T
    IL_0036:  ldc.i4.2
    IL_0037:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_003c:  nop
    IL_003d:  ldloc.0
    IL_003e:  box        !!T
    IL_0043:  ldc.i4.3
    IL_0044:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0049:  nop
    IL_004a:  ldloc.0
    IL_004b:  box        !!T
    IL_0050:  ldc.i4.4
    IL_0051:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0056:  nop
    IL_0057:  ldloc.0
    IL_0058:  box        !!T
    IL_005d:  ldc.i4.5
    IL_005e:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0063:  nop
    IL_0064:  ldloc.0
    IL_0065:  stloc.1
    IL_0066:  br.s       IL_0068

    IL_0068:  ldloc.1
    IL_0069:  ret
  } // end of method NoNewOfT`1::CollectionInitializer

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NoNewOfT`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
