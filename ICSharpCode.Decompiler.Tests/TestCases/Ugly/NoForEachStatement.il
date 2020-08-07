



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly rerff2f0
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module rerff2f0.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoForEachStatement
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  SimpleNonGenericForeach(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init (object V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0019:  nop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_003e

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [mscorlib]System.IDisposable
      IL_002d:  stloc.3
      IL_002e:  ldloc.3
      IL_002f:  ldnull
      IL_0030:  ceq
      IL_0032:  stloc.2
      IL_0033:  ldloc.2
      IL_0034:  brtrue.s   IL_003d

      IL_0036:  ldloc.3
      IL_0037:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003c:  nop
      IL_003d:  endfinally
    }  // end handler
    IL_003e:  nop
    IL_003f:  ret
  } // end of method NoForEachStatement::SimpleNonGenericForeach

  .method public hidebysig static void  SimpleForeachOverInts(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> enumerable) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0019:  nop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  ldnull
      IL_0029:  ceq
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  brtrue.s   IL_0036

      IL_002f:  ldloc.1
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  nop
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  nop
    IL_0038:  ret
  } // end of method NoForEachStatement::SimpleForeachOverInts

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NoForEachStatement::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoForEachStatement


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
