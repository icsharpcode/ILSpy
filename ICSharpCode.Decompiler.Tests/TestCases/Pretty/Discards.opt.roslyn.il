



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Discards
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
.module Discards.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit _
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
    } // end of method _::.ctor

  } // end of class _

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32> '<>9__6_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::'<>9'
      IL_000a:  ret
    } // end of method '<>c'::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance int32 
            '<DiscardedOutVsLambdaParameter>b__6_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_ _) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  ret
    } // end of method '<>c'::'<DiscardedOutVsLambdaParameter>b__6_0'

  } // end of class '<>c'

  .method public hidebysig instance void 
          GetOut([out] int32& 'value') cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.0
    IL_0002:  stind.i4
    IL_0003:  ret
  } // end of method Discards::GetOut

  .method public hidebysig instance void 
          MakeValue(class [mscorlib]System.Func`3<object,string,int32> func) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Discards::MakeValue

  .method public hidebysig instance void 
          MakeValue(class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32> func) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Discards::MakeValue

  .method public hidebysig instance void 
          SimpleParameter(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_ _) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Discards::SimpleParameter

  .method public hidebysig instance void 
          ParameterHiddenByLocal(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_ _) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards::GetOut(int32&)
    IL_0008:  ret
  } // end of method Discards::ParameterHiddenByLocal

  .method public hidebysig instance void 
          DiscardedOutVsLambdaParameter() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldloca.s   V_0
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards::GetOut(int32&)
    IL_0008:  ldarg.0
    IL_0009:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::'<>9__6_0'
    IL_000e:  dup
    IL_000f:  brtrue.s   IL_0028

    IL_0011:  pop
    IL_0012:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::'<>9'
    IL_0017:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::'<DiscardedOutVsLambdaParameter>b__6_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_)
    IL_001d:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32>::.ctor(object,
                                                                                                                                                  native int)
    IL_0022:  dup
    IL_0023:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/'<>c'::'<>9__6_0'
    IL_0028:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards::MakeValue(class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards/_,int32>)
    IL_002d:  ret
  } // end of method Discards::DiscardedOutVsLambdaParameter

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Discards::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Discards


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
