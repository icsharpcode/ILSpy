



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly prqfqkbt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module prqfqkbt.dll
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
    // Code size       50 (0x32)
    .maxstack  1
    .locals init (object V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_0016

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0016:  ldloc.1
      IL_0017:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001c:  brtrue.s   IL_0009

      IL_001e:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0020:  ldloc.1
      IL_0021:  isinst     [mscorlib]System.IDisposable
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brfalse.s  IL_0030

      IL_002a:  ldloc.2
      IL_002b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method NoForEachStatement::SimpleNonGenericForeach

  .method public hidebysig static void  SimpleForeachOverInts(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> enumerable) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  1
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_0016

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0016:  ldloc.1
      IL_0017:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001c:  brtrue.s   IL_0009

      IL_001e:  leave.s    IL_002a

    }  // end .try
    finally
    {
      IL_0020:  ldloc.1
      IL_0021:  brfalse.s  IL_0029

      IL_0023:  ldloc.1
      IL_0024:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0029:  endfinally
    }  // end handler
    IL_002a:  ret
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
