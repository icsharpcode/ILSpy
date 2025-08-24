
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmp1ekgox
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmp1ekgox.tmp
// MVID: {67ce32bd-0144-4136-a198-2fc9f941f624}
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
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (!TOnType V_0,
             !TOnType V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    !TOnType
    IL_0008:  ldloc.0
    IL_0009:  box        !TOnType
    IL_000e:  brfalse.s  IL_001a

    IL_0010:  ldloca.s   V_1
    IL_0012:  initobj    !TOnType
    IL_0018:  ldloc.1
    IL_0019:  ret

    IL_001a:  call       !!0 [mscorlib]System.Activator::CreateInstance<!TOnType>()
    IL_001f:  ret
  } // end of method NoNewOfT`1::CreateTOnType

  .method public hidebysig static !!T  CreateUnconstrainedT<.ctor T>() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (!!T V_0,
             !!T V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    !!T
    IL_0008:  ldloc.0
    IL_0009:  box        !!T
    IL_000e:  brfalse.s  IL_001a

    IL_0010:  ldloca.s   V_1
    IL_0012:  initobj    !!T
    IL_0018:  ldloc.1
    IL_0019:  ret

    IL_001a:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_001f:  ret
  } // end of method NoNewOfT`1::CreateUnconstrainedT

  .method public hidebysig static !!T  CreateClassT<class .ctor T>() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0005:  ret
  } // end of method NoNewOfT`1::CreateClassT

  .method public hidebysig static !!T  CollectionInitializer<.ctor (class [mscorlib]System.Collections.Generic.IList`1<int32>) T>() cil managed
  {
    // Code size       95 (0x5f)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1,
             !!T V_2)
    IL_0000:  ldloca.s   V_1
    IL_0002:  initobj    !!T
    IL_0008:  ldloc.1
    IL_0009:  box        !!T
    IL_000e:  brfalse.s  IL_001b

    IL_0010:  ldloca.s   V_2
    IL_0012:  initobj    !!T
    IL_0018:  ldloc.2
    IL_0019:  br.s       IL_0020

    IL_001b:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  box        !!T
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_002d:  ldloc.0
    IL_002e:  box        !!T
    IL_0033:  ldc.i4.2
    IL_0034:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0039:  ldloc.0
    IL_003a:  box        !!T
    IL_003f:  ldc.i4.3
    IL_0040:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0045:  ldloc.0
    IL_0046:  box        !!T
    IL_004b:  ldc.i4.4
    IL_004c:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0051:  ldloc.0
    IL_0052:  box        !!T
    IL_0057:  ldc.i4.5
    IL_0058:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_005d:  ldloc.0
    IL_005e:  ret
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
