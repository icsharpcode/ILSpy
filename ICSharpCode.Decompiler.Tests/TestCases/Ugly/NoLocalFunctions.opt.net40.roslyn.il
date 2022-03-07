



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmpAE03
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
.module tmpAE03.tmp
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.Handle
       extends [mscorlib]System.Object
{
  .field private initonly class [mscorlib]System.Func`1<int32> _func
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Func`1<int32> func) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Ugly.Handle::_func
    IL_000d:  ret
  } // end of method Handle::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Handle

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1_0'
         extends [mscorlib]System.ValueType
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 x
  } // end of class '<>c__DisplayClass1_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2_0'::.ctor

    .method assembly hidebysig instance int32 
            '<SimpleCaptureWithRef>g__F|0'() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldc.i4.s   42
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass2_0'::x
      IL_0008:  add
      IL_0009:  ret
    } // end of method '<>c__DisplayClass2_0'::'<SimpleCaptureWithRef>g__F|0'

  } // end of class '<>c__DisplayClass2_0'

  .method private hidebysig static void  UseLocalFunctionReference() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions::'<UseLocalFunctionReference>g__F|0_0'()
    IL_0007:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_000c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.Handle::.ctor(class [mscorlib]System.Func`1<int32>)
    IL_0011:  pop
    IL_0012:  ret
  } // end of method NoLocalFunctions::UseLocalFunctionReference

  .method private hidebysig static void  SimpleCapture() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass1_0' V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldc.i4.1
    IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass1_0'::x
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions::'<SimpleCapture>g__F|1_0'(valuetype ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass1_0'&)
    IL_000f:  pop
    IL_0010:  ret
  } // end of method NoLocalFunctions::SimpleCapture

  .method private hidebysig static void  SimpleCaptureWithRef() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass2_0'::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass2_0'::x
    IL_000c:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass2_0'::'<SimpleCaptureWithRef>g__F|0'()
    IL_0012:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0017:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.Handle::.ctor(class [mscorlib]System.Func`1<int32>)
    IL_001c:  pop
    IL_001d:  ret
  } // end of method NoLocalFunctions::SimpleCaptureWithRef

  .method assembly hidebysig static int32 
          '<UseLocalFunctionReference>g__F|0_0'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.s   42
    IL_0002:  ret
  } // end of method NoLocalFunctions::'<UseLocalFunctionReference>g__F|0_0'

  .method assembly hidebysig static int32 
          '<SimpleCapture>g__F|1_0'(valuetype ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass1_0'& A_0) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldc.i4.s   42
    IL_0002:  ldarg.0
    IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions/'<>c__DisplayClass1_0'::x
    IL_0008:  add
    IL_0009:  ret
  } // end of method NoLocalFunctions::'<SimpleCapture>g__F|1_0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoLocalFunctions


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
