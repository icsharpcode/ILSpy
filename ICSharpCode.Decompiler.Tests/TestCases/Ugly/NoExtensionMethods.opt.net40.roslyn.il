



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmpA29A
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
.module tmpA29A.tmp
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoExtensionMethods
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .method assembly hidebysig static class [mscorlib]System.Func`1<!!T> 
          AsFunc<class T>(!!T 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  ldftn      !!0 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoExtensionMethods::Return<!!0>(!!0)
    IL_000c:  newobj     instance void class [mscorlib]System.Func`1<!!T>::.ctor(object,
                                                                                 native int)
    IL_0011:  ret
  } // end of method NoExtensionMethods::AsFunc

  .method private hidebysig static !!T  Return<T>(!!T 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method NoExtensionMethods::Return

  .method assembly hidebysig static class [mscorlib]System.Func`2<int32,int32> 
          ExtensionMethodAsStaticFunc() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      !!0 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoExtensionMethods::Return<int32>(!!0)
    IL_0007:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_000c:  ret
  } // end of method NoExtensionMethods::ExtensionMethodAsStaticFunc

  .method assembly hidebysig static class [mscorlib]System.Func`1<object> 
          ExtensionMethodBoundToNull() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      !!0 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoExtensionMethods::Return<object>(!!0)
    IL_0007:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_000c:  ret
  } // end of method NoExtensionMethods::ExtensionMethodBoundToNull

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoExtensionMethods


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
