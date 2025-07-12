
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 10:0:0:0
}
.assembly tmpybdlca
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 19 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                              65 72 73 69 6F 6E 3D 76 31 30 2E 30 00 00 )       // ersion=v10.0..
  .permissionset reqmin
             = {[System.Runtime]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmpybdlca.tmp
// MVID: {b57f13a4-d291-42ea-b595-3766c1f1470c}
.custom instance void [System.Runtime]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void [System.Runtime]System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1<.ctor TOnType>
       extends [System.Runtime]System.Object
{
  .method public hidebysig static !TOnType 
          CreateTOnType() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!TOnType V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [System.Runtime]System.Activator::CreateInstance<!TOnType>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method NoNewOfT`1::CreateTOnType

  .method public hidebysig static !!T  CreateUnconstrainedT<.ctor T>() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [System.Runtime]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method NoNewOfT`1::CreateUnconstrainedT

  .method public hidebysig static !!T  CreateClassT<class .ctor T>() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [System.Runtime]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method NoNewOfT`1::CreateClassT

  .method public hidebysig static !!T  CollectionInitializer<.ctor (class [System.Runtime]System.Collections.Generic.IList`1<int32>) T>() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1)
    IL_0000:  nop
    IL_0001:  call       !!0 [System.Runtime]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldc.i4.1
    IL_000a:  constrained. !!T
    IL_0010:  callvirt   instance void class [System.Runtime]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloca.s   V_0
    IL_0018:  ldc.i4.2
    IL_0019:  constrained. !!T
    IL_001f:  callvirt   instance void class [System.Runtime]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloca.s   V_0
    IL_0027:  ldc.i4.3
    IL_0028:  constrained. !!T
    IL_002e:  callvirt   instance void class [System.Runtime]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0033:  nop
    IL_0034:  ldloca.s   V_0
    IL_0036:  ldc.i4.4
    IL_0037:  constrained. !!T
    IL_003d:  callvirt   instance void class [System.Runtime]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0042:  nop
    IL_0043:  ldloca.s   V_0
    IL_0045:  ldc.i4.5
    IL_0046:  constrained. !!T
    IL_004c:  callvirt   instance void class [System.Runtime]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0051:  nop
    IL_0052:  ldloc.0
    IL_0053:  stloc.1
    IL_0054:  br.s       IL_0056

    IL_0056:  ldloc.1
    IL_0057:  ret
  } // end of method NoNewOfT`1::CollectionInitializer

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NoNewOfT`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
