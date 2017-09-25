
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Loops
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
.module Loops.dll
// MVID: {C3B63F66-C940-45B6-9786-076A79DB1EC5}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          ForEach(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> enumerable) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0011:  stloc.1
      IL_0012:  nop
      IL_0013:  ldloc.1
      IL_0014:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0019:  pop
      IL_001a:  nop
      IL_001b:  ldloc.0
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brtrue.s   IL_000b

      IL_0023:  leave.s    IL_0030

    }  // end .try
    finally
    {
      IL_0025:  ldloc.0
      IL_0026:  brfalse.s  IL_002f

      IL_0028:  ldloc.0
      IL_0029:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002e:  nop
      IL_002f:  endfinally
    }  // end handler
    IL_0030:  ret
  } // end of method Loops::ForEach

  .method public hidebysig instance void 
          ForEachOverList(class [mscorlib]System.Collections.Generic.List`1<string> list) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_001c

      IL_000b:  ldloca.s   V_0
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_0012:  stloc.1
      IL_0013:  nop
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_001a:  pop
      IL_001b:  nop
      IL_001c:  ldloca.s   V_0
      IL_001e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_0036

    }  // end .try
    finally
    {
      IL_0027:  ldloca.s   V_0
      IL_0029:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_002f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0034:  nop
      IL_0035:  endfinally
    }  // end handler
    IL_0036:  ret
  } // end of method Loops::ForEachOverList

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerable(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       56 (0x38)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             object V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  stloc.1
      IL_0012:  nop
      IL_0013:  ldloc.1
      IL_0014:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0019:  pop
      IL_001a:  nop
      IL_001b:  ldloc.0
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brtrue.s   IL_000b

      IL_0023:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0025:  ldloc.0
      IL_0026:  isinst     [mscorlib]System.IDisposable
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  brfalse.s  IL_0036

      IL_002f:  ldloc.2
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  nop
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerable

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastValueType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             int32 V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_0021

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  unbox.any  [mscorlib]System.Int32
      IL_0016:  stloc.1
      IL_0017:  nop
      IL_0018:  ldloca.s   V_1
      IL_001a:  call       instance string [mscorlib]System.Int32::ToString()
      IL_001f:  pop
      IL_0020:  nop
      IL_0021:  ldloc.0
      IL_0022:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0027:  brtrue.s   IL_000b

      IL_0029:  leave.s    IL_003d

    }  // end .try
    finally
    {
      IL_002b:  ldloc.0
      IL_002c:  isinst     [mscorlib]System.IDisposable
      IL_0031:  stloc.2
      IL_0032:  ldloc.2
      IL_0033:  brfalse.s  IL_003c

      IL_0035:  ldloc.2
      IL_0036:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003b:  nop
      IL_003c:  endfinally
    }  // end handler
    IL_003d:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastValueType

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastRefType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             string V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  castclass  [mscorlib]System.String
      IL_0016:  stloc.1
      IL_0017:  nop
      IL_0018:  ldloc.1
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloc.0
      IL_0021:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0026:  brtrue.s   IL_000b

      IL_0028:  leave.s    IL_003c

    }  // end .try
    finally
    {
      IL_002a:  ldloc.0
      IL_002b:  isinst     [mscorlib]System.IDisposable
      IL_0030:  stloc.2
      IL_0031:  ldloc.2
      IL_0032:  brfalse.s  IL_003b

      IL_0034:  ldloc.2
      IL_0035:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003a:  nop
      IL_003b:  endfinally
    }  // end handler
    IL_003c:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastRefType

  .method public hidebysig instance void 
          ForOverArray(string[] 'array') cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0014

    IL_0005:  nop
    IL_0006:  ldarg.1
    IL_0007:  ldloc.0
    IL_0008:  ldelem.ref
    IL_0009:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_000e:  pop
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldarg.1
    IL_0016:  ldlen
    IL_0017:  conv.i4
    IL_0018:  clt
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0005

    IL_001e:  ret
  } // end of method Loops::ForOverArray

  .method public hidebysig instance void 
          NestedLoops() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             int32 V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0043

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.2
    IL_0008:  rem
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brfalse.s  IL_0031

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.2
    IL_0013:  br.s       IL_0026

    IL_0015:  nop
    IL_0016:  ldstr      "Y"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  nop
    IL_0021:  nop
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.2
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.5
    IL_0028:  clt
    IL_002a:  stloc.3
    IL_002b:  ldloc.3
    IL_002c:  brtrue.s   IL_0015

    IL_002e:  nop
    IL_002f:  br.s       IL_003e

    IL_0031:  nop
    IL_0032:  ldstr      "X"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  nop
    IL_003d:  nop
    IL_003e:  nop
    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stloc.0
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.s   10
    IL_0046:  clt
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloc.s    V_4
    IL_004c:  brtrue.s   IL_0005

    IL_004e:  ret
  } // end of method Loops::NestedLoops

  .method private hidebysig instance bool 
          Condition(string arg) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Condition: "
    IL_0006:  ldarg.1
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0016

    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method Loops::Condition

  .method public hidebysig instance void 
          WhileLoop() cil managed
  {
    // Code size       150 (0x96)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  brfalse.s  IL_008a

    IL_001b:  nop
    IL_001c:  br.s       IL_006d

    IL_001e:  nop
    IL_001f:  ldstr      "Loop Body"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  ldstr      "test"
    IL_0030:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  brfalse.s  IL_0061

    IL_0039:  nop
    IL_003a:  ldarg.0
    IL_003b:  ldstr      "continue"
    IL_0040:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  brfalse.s  IL_004c

    IL_0049:  nop
    IL_004a:  br.s       IL_006d

    IL_004c:  ldarg.0
    IL_004d:  ldstr      "break"
    IL_0052:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0057:  ldc.i4.0
    IL_0058:  ceq
    IL_005a:  stloc.3
    IL_005b:  ldloc.3
    IL_005c:  brfalse.s  IL_0060

    IL_005e:  br.s       IL_007e

    IL_0060:  nop
    IL_0061:  ldstr      "End of loop body"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  nop
    IL_006c:  nop
    IL_006d:  ldarg.0
    IL_006e:  ldstr      "while"
    IL_0073:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloc.s    V_4
    IL_007c:  brtrue.s   IL_001e

    IL_007e:  ldstr      "After loop"
    IL_0083:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0088:  nop
    IL_0089:  nop
    IL_008a:  ldstr      "End of method"
    IL_008f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0094:  nop
    IL_0095:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       158 (0x9e)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  brfalse.s  IL_0092

    IL_001b:  nop
    IL_001c:  ldc.i4.0
    IL_001d:  stloc.1
    IL_001e:  br.s       IL_0075

    IL_0020:  nop
    IL_0021:  ldstr      "Loop Body"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  ldstr      "test"
    IL_0032:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0037:  stloc.2
    IL_0038:  ldloc.2
    IL_0039:  brfalse.s  IL_0065

    IL_003b:  nop
    IL_003c:  ldarg.0
    IL_003d:  ldstr      "continue"
    IL_0042:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0047:  stloc.3
    IL_0048:  ldloc.3
    IL_0049:  brfalse.s  IL_004e

    IL_004b:  nop
    IL_004c:  br.s       IL_0071

    IL_004e:  ldarg.0
    IL_004f:  ldstr      "not-break"
    IL_0054:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0059:  ldc.i4.0
    IL_005a:  ceq
    IL_005c:  stloc.s    V_4
    IL_005e:  ldloc.s    V_4
    IL_0060:  brfalse.s  IL_0064

    IL_0062:  br.s       IL_0086

    IL_0064:  nop
    IL_0065:  ldstr      "End of loop body"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  nop
    IL_0071:  ldloc.1
    IL_0072:  ldc.i4.1
    IL_0073:  add
    IL_0074:  stloc.1
    IL_0075:  ldarg.0
    IL_0076:  ldstr      "for"
    IL_007b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0080:  stloc.s    V_5
    IL_0082:  ldloc.s    V_5
    IL_0084:  brtrue.s   IL_0020

    IL_0086:  ldstr      "After loop"
    IL_008b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0090:  nop
    IL_0091:  nop
    IL_0092:  ldstr      "End of method"
    IL_0097:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009c:  nop
    IL_009d:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig static void  ForeachExceptForContinuedUse(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> inputs) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "ForeachExceptForContinuedUse"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.0
    IL_000e:  ldarg.0
    IL_000f:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0014:  stloc.1
    .try
    {
      IL_0015:  nop
      IL_0016:  br.s       IL_0028

      IL_0018:  nop
      IL_0019:  ldloc.1
      IL_001a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001f:  stloc.0
      IL_0020:  ldloc.0
      IL_0021:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0026:  nop
      IL_0027:  nop
      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_002e:  stloc.2
      IL_002f:  ldloc.2
      IL_0030:  brtrue.s   IL_0018

      IL_0032:  nop
      IL_0033:  leave.s    IL_0040

    }  // end .try
    finally
    {
      IL_0035:  ldloc.1
      IL_0036:  brfalse.s  IL_003f

      IL_0038:  ldloc.1
      IL_0039:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003e:  nop
      IL_003f:  endfinally
    }  // end handler
    IL_0040:  ldstr      "Last: "
    IL_0045:  ldloc.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  nop
    IL_0056:  ret
  } // end of method Loops::ForeachExceptForContinuedUse

  .method public hidebysig static void  NonGenericForeachWithReturnFallbackTest(class [mscorlib]System.Collections.IEnumerable e) cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             bool V_1,
             object V_2,
             class [mscorlib]System.IDisposable V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldstr      "NonGenericForeachWithReturnFallback:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0012:  stloc.0
    .try
    {
      IL_0013:  nop
      IL_0014:  ldstr      "MoveNext"
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  ldloc.0
      IL_0020:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0025:  stloc.1
      IL_0026:  ldloc.1
      IL_0027:  brfalse.s  IL_0043

      IL_0029:  nop
      IL_002a:  ldloc.0
      IL_002b:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0030:  stloc.2
      IL_0031:  ldstr      "current: "
      IL_0036:  ldloc.2
      IL_0037:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_003c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0041:  nop
      IL_0042:  nop
      IL_0043:  nop
      IL_0044:  leave.s    IL_0063

    }  // end .try
    finally
    {
      IL_0046:  nop
      IL_0047:  ldloc.0
      IL_0048:  isinst     [mscorlib]System.IDisposable
      IL_004d:  stloc.3
      IL_004e:  ldloc.3
      IL_004f:  ldnull
      IL_0050:  cgt.un
      IL_0052:  stloc.s    V_4
      IL_0054:  ldloc.s    V_4
      IL_0056:  brfalse.s  IL_0061

      IL_0058:  nop
      IL_0059:  ldloc.3
      IL_005a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_005f:  nop
      IL_0060:  nop
      IL_0061:  nop
      IL_0062:  endfinally
    }  // end handler
    IL_0063:  ldstr      "After finally!"
    IL_0068:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006d:  nop
    IL_006e:  ret
  } // end of method Loops::NonGenericForeachWithReturnFallbackTest

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Loops::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
