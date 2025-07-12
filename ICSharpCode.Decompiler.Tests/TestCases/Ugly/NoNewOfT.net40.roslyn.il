
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmpyccvci
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmpyccvci.tmp
// MVID: {ff6a33b6-7f19-447b-b168-f5c528dc4b6f}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                         72 69 74 65 64 00 )                               // rited.
  .field public initonly int32 Version
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  ldarg.1
    IL_0009:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000e:  ret
  } // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 01 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                         72 69 74 65 64 00 )                               // rited.
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string featureName) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method CompilerFeatureRequiredAttribute::.ctor

} // end of class System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.IsExternalInit
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method IsExternalInit::.ctor

} // end of class System.Runtime.CompilerServices.IsExternalInit

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RequiredMemberAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 8C 01 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                         72 69 74 65 64 00 )                               // rited.
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method RequiredMemberAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RequiredMemberAttribute

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1<.ctor TOnType>
       extends [mscorlib]System.Object
{
  .method public hidebysig static !TOnType 
          CreateTOnType() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!TOnType V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!TOnType>()
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
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
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
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method NoNewOfT`1::CreateClassT

  .method public hidebysig static !!T  CollectionInitializer<.ctor (class [mscorlib]System.Collections.Generic.IList`1<int32>) T>() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1)
    IL_0000:  nop
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  ldc.i4.1
    IL_000a:  constrained. !!T
    IL_0010:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloca.s   V_0
    IL_0018:  ldc.i4.2
    IL_0019:  constrained. !!T
    IL_001f:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloca.s   V_0
    IL_0027:  ldc.i4.3
    IL_0028:  constrained. !!T
    IL_002e:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0033:  nop
    IL_0034:  ldloca.s   V_0
    IL_0036:  ldc.i4.4
    IL_0037:  constrained. !!T
    IL_003d:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0042:  nop
    IL_0043:  ldloca.s   V_0
    IL_0045:  ldc.i4.5
    IL_0046:  constrained. !!T
    IL_004c:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
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
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NoNewOfT`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
