
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly kpve0joz
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module kpve0joz.dll
// MVID: {20825129-FBF4-4D0D-B2DB-BB2FE12F3807}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02A50000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          ForEach(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> enumerable) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (string V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0019:  pop
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
  } // end of method Loops::ForEach

  .method public hidebysig instance void 
          ForEachOverList(class [mscorlib]System.Collections.Generic.List`1<string> list) cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  1
    .locals init (string V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001c

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloc.0
      IL_0015:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_001a:  pop
      IL_001b:  nop
      IL_001c:  ldloca.s   V_1
      IL_001e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_0023:  stloc.2
      IL_0024:  ldloc.2
      IL_0025:  brtrue.s   IL_000b

      IL_0027:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_0029:  ldloca.s   V_1
      IL_002b:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_0031:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0036:  nop
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  nop
    IL_0039:  ret
  } // end of method Loops::ForEachOverList

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerable(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init (object V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
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
      IL_0014:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0019:  pop
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
  } // end of method Loops::ForEachOverNonGenericEnumerable

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastValueType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0021

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  unbox.any  [mscorlib]System.Int32
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldloca.s   V_0
      IL_001a:  call       instance string [mscorlib]System.Int32::ToString()
      IL_001f:  pop
      IL_0020:  nop
      IL_0021:  ldloc.1
      IL_0022:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_000b

      IL_002b:  leave.s    IL_0044

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  isinst     [mscorlib]System.IDisposable
      IL_0033:  stloc.3
      IL_0034:  ldloc.3
      IL_0035:  ldnull
      IL_0036:  ceq
      IL_0038:  stloc.2
      IL_0039:  ldloc.2
      IL_003a:  brtrue.s   IL_0043

      IL_003c:  ldloc.3
      IL_003d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0042:  nop
      IL_0043:  endfinally
    }  // end handler
    IL_0044:  nop
    IL_0045:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastValueType

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastRefType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  2
    .locals init (string V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  castclass  [mscorlib]System.String
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldloc.0
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brtrue.s   IL_000b

      IL_002a:  leave.s    IL_0043

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  isinst     [mscorlib]System.IDisposable
      IL_0032:  stloc.3
      IL_0033:  ldloc.3
      IL_0034:  ldnull
      IL_0035:  ceq
      IL_0037:  stloc.2
      IL_0038:  ldloc.2
      IL_0039:  brtrue.s   IL_0042

      IL_003b:  ldloc.3
      IL_003c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0041:  nop
      IL_0042:  endfinally
    }  // end handler
    IL_0043:  nop
    IL_0044:  ret
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
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0046

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.2
    IL_0008:  rem
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  brtrue.s   IL_0034

    IL_0013:  nop
    IL_0014:  ldc.i4.0
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0029

    IL_0018:  nop
    IL_0019:  ldstr      "Y"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.5
    IL_002b:  clt
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  brtrue.s   IL_0018

    IL_0031:  nop
    IL_0032:  br.s       IL_0041

    IL_0034:  nop
    IL_0035:  ldstr      "X"
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  stloc.0
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.s   10
    IL_0049:  clt
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  brtrue.s   IL_0005

    IL_004f:  ret
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
    // Code size       154 (0x9a)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  brtrue.s   IL_008e

    IL_001e:  nop
    IL_001f:  br.s       IL_0073

    IL_0021:  nop
    IL_0022:  ldstr      "Loop Body"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  ldstr      "test"
    IL_0033:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0038:  ldc.i4.0
    IL_0039:  ceq
    IL_003b:  stloc.0
    IL_003c:  ldloc.0
    IL_003d:  brtrue.s   IL_0067

    IL_003f:  nop
    IL_0040:  ldarg.0
    IL_0041:  ldstr      "continue"
    IL_0046:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004b:  ldc.i4.0
    IL_004c:  ceq
    IL_004e:  stloc.0
    IL_004f:  ldloc.0
    IL_0050:  brtrue.s   IL_0055

    IL_0052:  nop
    IL_0053:  br.s       IL_0073

    IL_0055:  ldarg.0
    IL_0056:  ldstr      "break"
    IL_005b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0060:  stloc.0
    IL_0061:  ldloc.0
    IL_0062:  brtrue.s   IL_0066

    IL_0064:  br.s       IL_0082

    IL_0066:  nop
    IL_0067:  ldstr      "End of loop body"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  nop
    IL_0072:  nop
    IL_0073:  ldarg.0
    IL_0074:  ldstr      "while"
    IL_0079:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_007e:  stloc.0
    IL_007f:  ldloc.0
    IL_0080:  brtrue.s   IL_0021

    IL_0082:  ldstr      "After loop"
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008c:  nop
    IL_008d:  nop
    IL_008e:  ldstr      "End of method"
    IL_0093:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0098:  nop
    IL_0099:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       160 (0xa0)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0094

    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0079

    IL_0023:  nop
    IL_0024:  ldstr      "Loop Body"
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  ldstr      "test"
    IL_0035:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003a:  ldc.i4.0
    IL_003b:  ceq
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  brtrue.s   IL_0069

    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  ldstr      "continue"
    IL_0048:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004d:  ldc.i4.0
    IL_004e:  ceq
    IL_0050:  stloc.1
    IL_0051:  ldloc.1
    IL_0052:  brtrue.s   IL_0057

    IL_0054:  nop
    IL_0055:  br.s       IL_0075

    IL_0057:  ldarg.0
    IL_0058:  ldstr      "not-break"
    IL_005d:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0062:  stloc.1
    IL_0063:  ldloc.1
    IL_0064:  brtrue.s   IL_0068

    IL_0066:  br.s       IL_0088

    IL_0068:  nop
    IL_0069:  ldstr      "End of loop body"
    IL_006e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0073:  nop
    IL_0074:  nop
    IL_0075:  ldloc.0
    IL_0076:  ldc.i4.1
    IL_0077:  add
    IL_0078:  stloc.0
    IL_0079:  ldarg.0
    IL_007a:  ldstr      "for"
    IL_007f:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0084:  stloc.1
    IL_0085:  ldloc.1
    IL_0086:  brtrue.s   IL_0023

    IL_0088:  ldstr      "After loop"
    IL_008d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0092:  nop
    IL_0093:  nop
    IL_0094:  ldstr      "End of method"
    IL_0099:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009e:  nop
    IL_009f:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig static void  ForeachExceptForContinuedUse(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> inputs) cil managed
  {
    // Code size       93 (0x5d)
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
      IL_0033:  leave.s    IL_0045

    }  // end .try
    finally
    {
      IL_0035:  ldloc.1
      IL_0036:  ldnull
      IL_0037:  ceq
      IL_0039:  stloc.2
      IL_003a:  ldloc.2
      IL_003b:  brtrue.s   IL_0044

      IL_003d:  ldloc.1
      IL_003e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0043:  nop
      IL_0044:  endfinally
    }  // end handler
    IL_0045:  nop
    IL_0046:  ldstr      "Last: "
    IL_004b:  ldloc.0
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  nop
    IL_005c:  ret
  } // end of method Loops::ForeachExceptForContinuedUse

  .method public hidebysig static void  NonGenericForeachWithReturnFallbackTest(class [mscorlib]System.Collections.IEnumerable e) cil managed
  {
    // Code size       113 (0x71)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             object V_1,
             class [mscorlib]System.IDisposable V_2,
             bool V_3)
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
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  stloc.3
      IL_0029:  ldloc.3
      IL_002a:  brtrue.s   IL_0046

      IL_002c:  nop
      IL_002d:  ldloc.0
      IL_002e:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0033:  stloc.1
      IL_0034:  ldstr      "current: "
      IL_0039:  ldloc.1
      IL_003a:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0044:  nop
      IL_0045:  nop
      IL_0046:  nop
      IL_0047:  leave.s    IL_0064

    }  // end .try
    finally
    {
      IL_0049:  nop
      IL_004a:  ldloc.0
      IL_004b:  isinst     [mscorlib]System.IDisposable
      IL_0050:  stloc.2
      IL_0051:  ldloc.2
      IL_0052:  ldnull
      IL_0053:  ceq
      IL_0055:  stloc.3
      IL_0056:  ldloc.3
      IL_0057:  brtrue.s   IL_0062

      IL_0059:  nop
      IL_005a:  ldloc.2
      IL_005b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0060:  nop
      IL_0061:  nop
      IL_0062:  nop
      IL_0063:  endfinally
    }  // end handler
    IL_0064:  nop
    IL_0065:  ldstr      "After finally!"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Loops.res
