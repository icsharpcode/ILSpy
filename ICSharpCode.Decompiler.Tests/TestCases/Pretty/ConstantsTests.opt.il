



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ConstantsTests.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ConstantsTests.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests
       extends [mscorlib]System.Object
{
  .method public hidebysig instance uint64 
          Issue1308([opt] uint64 u) cil managed
  {
    .param [1] = uint64(0x8)
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.m1
    IL_0003:  conv.u8
    IL_0004:  and
    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  ceq
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_0011:  ldc.i8     0xffffffff00000000
    IL_001a:  ret
  } // end of method ConstantsTests::Issue1308

  .method public hidebysig instance void 
          Byte_BitmaskingInCondition(uint8 v) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   15
    IL_0004:  and
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4     0x123
    IL_0014:  and
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_001d:  ldarg.0
    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.s   15
    IL_0021:  or
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_002a:  ldarg.0
    IL_002b:  ldarg.1
    IL_002c:  ldc.i4     0x123
    IL_0031:  or
    IL_0032:  ldc.i4.0
    IL_0033:  ceq
    IL_0035:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_003a:  ret
  } // end of method ConstantsTests::Byte_BitmaskingInCondition

  .method public hidebysig instance void 
          SByte_BitmaskingInCondition(int8 v) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   15
    IL_0004:  and
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4     0x123
    IL_0014:  and
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_001d:  ldarg.0
    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.s   15
    IL_0021:  or
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_002a:  ldarg.0
    IL_002b:  ldarg.1
    IL_002c:  ldc.i4     0x123
    IL_0031:  or
    IL_0032:  ldc.i4.0
    IL_0033:  ceq
    IL_0035:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests::Test(bool)
    IL_003a:  ret
  } // end of method ConstantsTests::SByte_BitmaskingInCondition

  .method private hidebysig instance void 
          Test(bool expr) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ConstantsTests::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ConstantsTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstantsTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
