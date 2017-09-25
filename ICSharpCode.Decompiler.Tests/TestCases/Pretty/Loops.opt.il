
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ukcebu5a
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ukcebu5a.dll
// MVID: {D6A0A420-9828-48A7-BAA6-F0BAFB50052B}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02F50000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          ForEach(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> enumerable) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  1
    .locals init (string V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_0017

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0016:  pop
      IL_0017:  ldloc.1
      IL_0018:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001d:  brtrue.s   IL_0009

      IL_001f:  leave.s    IL_002b

    }  // end .try
    finally
    {
      IL_0021:  ldloc.1
      IL_0022:  brfalse.s  IL_002a

      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002a:  endfinally
    }  // end handler
    IL_002b:  ret
  } // end of method Loops::ForEach

  .method public hidebysig instance void 
          ForEachOverList(class [mscorlib]System.Collections.Generic.List`1<string> list) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  1
    .locals init (string V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_0018

      IL_0009:  ldloca.s   V_1
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0017:  pop
      IL_0018:  ldloca.s   V_1
      IL_001a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0023:  ldloca.s   V_1
      IL_0025:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_002b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method Loops::ForEachOverList

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerable(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  1
    .locals init (object V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_0017

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0016:  pop
      IL_0017:  ldloc.1
      IL_0018:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001d:  brtrue.s   IL_0009

      IL_001f:  leave.s    IL_0032

    }  // end .try
    finally
    {
      IL_0021:  ldloc.1
      IL_0022:  isinst     [mscorlib]System.IDisposable
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brfalse.s  IL_0031

      IL_002b:  ldloc.2
      IL_002c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0031:  endfinally
    }  // end handler
    IL_0032:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerable

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastValueType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  1
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_001d

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  unbox.any  [mscorlib]System.Int32
      IL_0014:  stloc.0
      IL_0015:  ldloca.s   V_0
      IL_0017:  call       instance string [mscorlib]System.Int32::ToString()
      IL_001c:  pop
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0023:  brtrue.s   IL_0009

      IL_0025:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [mscorlib]System.IDisposable
      IL_002d:  stloc.2
      IL_002e:  ldloc.2
      IL_002f:  brfalse.s  IL_0037

      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastValueType

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastRefType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  1
    .locals init (string V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.1
    .try
    {
      IL_0007:  br.s       IL_001b

      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  castclass  [mscorlib]System.String
      IL_0014:  stloc.0
      IL_0015:  ldloc.0
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brtrue.s   IL_0009

      IL_0023:  leave.s    IL_0036

    }  // end .try
    finally
    {
      IL_0025:  ldloc.1
      IL_0026:  isinst     [mscorlib]System.IDisposable
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  brfalse.s  IL_0035

      IL_002f:  ldloc.2
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  endfinally
    }  // end handler
    IL_0036:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastRefType

  .method public hidebysig instance void 
          ForOverArray(string[] 'array') cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0011

    IL_0004:  ldarg.1
    IL_0005:  ldloc.0
    IL_0006:  ldelem.ref
    IL_0007:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_000c:  pop
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  ldarg.1
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  blt.s      IL_0004

    IL_0017:  ret
  } // end of method Loops::ForOverArray

  .method public hidebysig instance void 
          NestedLoops() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_002f

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.2
    IL_0006:  rem
    IL_0007:  brtrue.s   IL_0021

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_001b

    IL_000d:  ldstr      "Y"
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  ldloc.1
    IL_0018:  ldc.i4.1
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.5
    IL_001d:  blt.s      IL_000d

    IL_001f:  br.s       IL_002b

    IL_0021:  ldstr      "X"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.s   10
    IL_0032:  blt.s      IL_0004

    IL_0034:  ret
  } // end of method Loops::NestedLoops

  .method private hidebysig instance bool 
          Condition(string arg) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldstr      "Condition: "
    IL_0005:  ldarg.1
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldc.i4.0
    IL_0011:  ret
  } // end of method Loops::Condition

  .method public hidebysig instance void 
          WhileLoop() cil managed
  {
    // Code size       118 (0x76)
    .maxstack  2
    IL_0000:  ldstr      "Initial"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "if"
    IL_0010:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0015:  brfalse.s  IL_006b

    IL_0017:  br.s       IL_0054

    IL_0019:  ldstr      "Loop Body"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldarg.0
    IL_0024:  ldstr      "test"
    IL_0029:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_002e:  brfalse.s  IL_004a

    IL_0030:  ldarg.0
    IL_0031:  ldstr      "continue"
    IL_0036:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003b:  brtrue.s   IL_0054

    IL_003d:  ldarg.0
    IL_003e:  ldstr      "break"
    IL_0043:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0048:  brfalse.s  IL_0061

    IL_004a:  ldstr      "End of loop body"
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0054:  ldarg.0
    IL_0055:  ldstr      "while"
    IL_005a:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_005f:  brtrue.s   IL_0019

    IL_0061:  ldstr      "After loop"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  ldstr      "End of method"
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0075:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       124 (0x7c)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "Initial"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "if"
    IL_0010:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0015:  brfalse.s  IL_0071

    IL_0017:  ldc.i4.0
    IL_0018:  stloc.0
    IL_0019:  br.s       IL_005a

    IL_001b:  ldstr      "Loop Body"
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0025:  ldarg.0
    IL_0026:  ldstr      "test"
    IL_002b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0030:  brfalse.s  IL_004c

    IL_0032:  ldarg.0
    IL_0033:  ldstr      "continue"
    IL_0038:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003d:  brtrue.s   IL_0056

    IL_003f:  ldarg.0
    IL_0040:  ldstr      "not-break"
    IL_0045:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004a:  brfalse.s  IL_0067

    IL_004c:  ldstr      "End of loop body"
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.1
    IL_0058:  add
    IL_0059:  stloc.0
    IL_005a:  ldarg.0
    IL_005b:  ldstr      "for"
    IL_0060:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0065:  brtrue.s   IL_001b

    IL_0067:  ldstr      "After loop"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  ldstr      "End of method"
    IL_0076:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007b:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig static void  ForeachExceptForContinuedUse(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> inputs) cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1)
    IL_0000:  ldstr      "ForeachExceptForContinuedUse"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0012:  stloc.1
    .try
    {
      IL_0013:  br.s       IL_0022

      IL_0015:  ldloc.1
      IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001b:  stloc.0
      IL_001c:  ldloc.0
      IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0022:  ldloc.1
      IL_0023:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0028:  brtrue.s   IL_0015

      IL_002a:  leave.s    IL_0036

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  brfalse.s  IL_0035

      IL_002f:  ldloc.1
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  endfinally
    }  // end handler
    IL_0036:  ldstr      "Last: "
    IL_003b:  ldloc.0
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  ret
  } // end of method Loops::ForeachExceptForContinuedUse

  .method public hidebysig static void  NonGenericForeachWithReturnFallbackTest(class [mscorlib]System.Collections.IEnumerable e) cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             object V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldstr      "NonGenericForeachWithReturnFallback:"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0010:  stloc.0
    .try
    {
      IL_0011:  ldstr      "MoveNext"
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001b:  ldloc.0
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brfalse.s  IL_003a

      IL_0023:  ldloc.0
      IL_0024:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0029:  stloc.1
      IL_002a:  ldstr      "current: "
      IL_002f:  ldloc.1
      IL_0030:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_003a:  leave.s    IL_004d

    }  // end .try
    finally
    {
      IL_003c:  ldloc.0
      IL_003d:  isinst     [mscorlib]System.IDisposable
      IL_0042:  stloc.2
      IL_0043:  ldloc.2
      IL_0044:  brfalse.s  IL_004c

      IL_0046:  ldloc.2
      IL_0047:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_004c:  endfinally
    }  // end handler
    IL_004d:  ldstr      "After finally!"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  ret
  } // end of method Loops::NonGenericForeachWithReturnFallbackTest

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Loops::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Loops.opt.res
