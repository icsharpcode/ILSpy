
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmp4xstme
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmp4xstme.tmp
// MVID: {a031faef-fcb2-420c-9cd3-6e9b01e7f81e}
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
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
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000d:  ret
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method CompilerFeatureRequiredAttribute::.ctor

} // end of class System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute

.class private auto ansi beforefieldinit System.Runtime.CompilerServices.IsExternalInit
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method RequiredMemberAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RequiredMemberAttribute

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoNewOfT`1<.ctor TOnType>
       extends [mscorlib]System.Object
{
  .method public hidebysig static !TOnType 
          CreateTOnType() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!TOnType>()
    IL_0005:  ret
  } // end of method NoNewOfT`1::CreateTOnType

  .method public hidebysig static !!T  CreateUnconstrainedT<.ctor T>() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0005:  ret
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
    // Code size       78 (0x4e)
    .maxstack  2
    .locals init (!!T V_0)
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0005:  stloc.0
    IL_0006:  ldloca.s   V_0
    IL_0008:  ldc.i4.1
    IL_0009:  constrained. !!T
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.2
    IL_0017:  constrained. !!T
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0022:  ldloca.s   V_0
    IL_0024:  ldc.i4.3
    IL_0025:  constrained. !!T
    IL_002b:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_0030:  ldloca.s   V_0
    IL_0032:  ldc.i4.4
    IL_0033:  constrained. !!T
    IL_0039:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_003e:  ldloca.s   V_0
    IL_0040:  ldc.i4.5
    IL_0041:  constrained. !!T
    IL_0047:  callvirt   instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
    IL_004c:  ldloc.0
    IL_004d:  ret
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
