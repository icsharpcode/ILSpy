
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly plngalwy
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module plngalwy.dll
// MVID: {0E13F4FD-6735-4D69-893B-CE3CD5A7D290}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00B70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Loops
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
          ForEachOverNonGenericEnumerableWithAutomaticCast(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
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
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCast

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

  .method public hidebysig instance int32 
          MultipleExits() cil managed
  {
    // Code size       97 (0x61)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0051

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.4
    IL_0008:  rem
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  brtrue.s   IL_0018

    IL_0013:  nop
    IL_0014:  ldc.i4.4
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_005f

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.7
    IL_001a:  rem
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  ldc.i4.0
    IL_001f:  ceq
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  brtrue.s   IL_0028

    IL_0025:  nop
    IL_0026:  br.s       IL_0055

    IL_0028:  ldloc.0
    IL_0029:  ldc.i4.s   9
    IL_002b:  rem
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  ldc.i4.0
    IL_0030:  ceq
    IL_0032:  stloc.2
    IL_0033:  ldloc.2
    IL_0034:  brtrue.s   IL_003b

    IL_0036:  nop
    IL_0037:  ldc.i4.5
    IL_0038:  stloc.1
    IL_0039:  br.s       IL_005f

    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.s   11
    IL_003e:  rem
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  ldc.i4.0
    IL_0043:  ceq
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  brtrue.s   IL_004c

    IL_0049:  nop
    IL_004a:  br.s       IL_0055

    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  stloc.0
    IL_0050:  nop
    IL_0051:  ldc.i4.1
    IL_0052:  stloc.2
    IL_0053:  br.s       IL_0005

    IL_0055:  ldc.i4     0x80000000
    IL_005a:  stloc.0
    IL_005b:  ldloc.0
    IL_005c:  stloc.1
    IL_005d:  br.s       IL_005f

    IL_005f:  ldloc.1
    IL_0060:  ret
  } // end of method Loops::MultipleExits

  .method public hidebysig instance int32 
          InterestingLoop() cil managed
  {
    // Code size       117 (0x75)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   11
    IL_0006:  rem
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  stloc.2
    IL_000e:  ldloc.2
    IL_000f:  brtrue.s   IL_006f

    IL_0011:  nop
    IL_0012:  br.s       IL_0064

    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.4
    IL_0017:  rem
    IL_0018:  ldc.i4.0
    IL_0019:  ceq
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  brtrue.s   IL_005f

    IL_0022:  nop
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.7
    IL_0025:  rem
    IL_0026:  ldc.i4.0
    IL_0027:  ceq
    IL_0029:  ldc.i4.0
    IL_002a:  ceq
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  brtrue.s   IL_0050

    IL_0030:  nop
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.s   11
    IL_0034:  rem
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  ldc.i4.0
    IL_0039:  ceq
    IL_003b:  stloc.2
    IL_003c:  ldloc.2
    IL_003d:  brtrue.s   IL_0042

    IL_003f:  nop
    IL_0040:  br.s       IL_0064

    IL_0042:  ldstr      "7"
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004c:  nop
    IL_004d:  nop
    IL_004e:  br.s       IL_005d

    IL_0050:  nop
    IL_0051:  ldstr      "!7"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  nop
    IL_005c:  nop
    IL_005d:  br.s       IL_0068

    IL_005f:  ldloc.0
    IL_0060:  ldc.i4.1
    IL_0061:  add
    IL_0062:  stloc.0
    IL_0063:  nop
    IL_0064:  ldc.i4.1
    IL_0065:  stloc.2
    IL_0066:  br.s       IL_0014

    IL_0068:  ldc.i4     0x80000000
    IL_006d:  stloc.0
    IL_006e:  nop
    IL_006f:  ldloc.0
    IL_0070:  stloc.1
    IL_0071:  br.s       IL_0073

    IL_0073:  ldloc.1
    IL_0074:  ret
  } // end of method Loops::InterestingLoop

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
    // Code size       153 (0x99)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  brtrue.s   IL_008d

    IL_001e:  nop
    IL_001f:  br.s       IL_0072

    IL_0021:  nop
    IL_0022:  ldstr      "Loop Body"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  ldstr      "test"
    IL_0033:  call       instance bool Loops::Condition(string)
    IL_0038:  ldc.i4.0
    IL_0039:  ceq
    IL_003b:  stloc.0
    IL_003c:  ldloc.0
    IL_003d:  brtrue.s   IL_0066

    IL_003f:  nop
    IL_0040:  ldarg.0
    IL_0041:  ldstr      "continue"
    IL_0046:  call       instance bool Loops::Condition(string)
    IL_004b:  ldc.i4.0
    IL_004c:  ceq
    IL_004e:  stloc.0
    IL_004f:  ldloc.0
    IL_0050:  brtrue.s   IL_0054

    IL_0052:  br.s       IL_0072

    IL_0054:  ldarg.0
    IL_0055:  ldstr      "break"
    IL_005a:  call       instance bool Loops::Condition(string)
    IL_005f:  stloc.0
    IL_0060:  ldloc.0
    IL_0061:  brtrue.s   IL_0065

    IL_0063:  br.s       IL_0081

    IL_0065:  nop
    IL_0066:  ldstr      "End of loop body"
    IL_006b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  ldarg.0
    IL_0073:  ldstr      "while"
    IL_0078:  call       instance bool Loops::Condition(string)
    IL_007d:  stloc.0
    IL_007e:  ldloc.0
    IL_007f:  brtrue.s   IL_0021

    IL_0081:  ldstr      "After loop"
    IL_0086:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008b:  nop
    IL_008c:  nop
    IL_008d:  ldstr      "End of method"
    IL_0092:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0097:  nop
    IL_0098:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          DoWhileLoop() cil managed
  {
    // Code size       151 (0x97)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  brtrue.s   IL_008b

    IL_001e:  nop
    IL_001f:  nop
    IL_0020:  ldstr      "Loop Body"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  ldstr      "test"
    IL_0031:  call       instance bool Loops::Condition(string)
    IL_0036:  ldc.i4.0
    IL_0037:  ceq
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  brtrue.s   IL_0064

    IL_003d:  nop
    IL_003e:  ldarg.0
    IL_003f:  ldstr      "continue"
    IL_0044:  call       instance bool Loops::Condition(string)
    IL_0049:  ldc.i4.0
    IL_004a:  ceq
    IL_004c:  stloc.0
    IL_004d:  ldloc.0
    IL_004e:  brtrue.s   IL_0052

    IL_0050:  br.s       IL_0070

    IL_0052:  ldarg.0
    IL_0053:  ldstr      "break"
    IL_0058:  call       instance bool Loops::Condition(string)
    IL_005d:  stloc.0
    IL_005e:  ldloc.0
    IL_005f:  brtrue.s   IL_0063

    IL_0061:  br.s       IL_007f

    IL_0063:  nop
    IL_0064:  ldstr      "End of loop body"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  nop
    IL_006f:  nop
    IL_0070:  ldarg.0
    IL_0071:  ldstr      "while"
    IL_0076:  call       instance bool Loops::Condition(string)
    IL_007b:  stloc.0
    IL_007c:  ldloc.0
    IL_007d:  brtrue.s   IL_001f

    IL_007f:  ldstr      "After loop"
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0089:  nop
    IL_008a:  nop
    IL_008b:  ldstr      "End of method"
    IL_0090:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0095:  nop
    IL_0096:  ret
  } // end of method Loops::DoWhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       159 (0x9f)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0093

    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0078

    IL_0023:  nop
    IL_0024:  ldstr      "Loop Body"
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  ldstr      "test"
    IL_0035:  call       instance bool Loops::Condition(string)
    IL_003a:  ldc.i4.0
    IL_003b:  ceq
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  brtrue.s   IL_0068

    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  ldstr      "continue"
    IL_0048:  call       instance bool Loops::Condition(string)
    IL_004d:  ldc.i4.0
    IL_004e:  ceq
    IL_0050:  stloc.1
    IL_0051:  ldloc.1
    IL_0052:  brtrue.s   IL_0056

    IL_0054:  br.s       IL_0074

    IL_0056:  ldarg.0
    IL_0057:  ldstr      "break"
    IL_005c:  call       instance bool Loops::Condition(string)
    IL_0061:  stloc.1
    IL_0062:  ldloc.1
    IL_0063:  brtrue.s   IL_0067

    IL_0065:  br.s       IL_0087

    IL_0067:  nop
    IL_0068:  ldstr      "End of loop body"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  nop
    IL_0073:  nop
    IL_0074:  ldloc.0
    IL_0075:  ldc.i4.1
    IL_0076:  add
    IL_0077:  stloc.0
    IL_0078:  ldarg.0
    IL_0079:  ldstr      "for"
    IL_007e:  call       instance bool Loops::Condition(string)
    IL_0083:  stloc.1
    IL_0084:  ldloc.1
    IL_0085:  brtrue.s   IL_0023

    IL_0087:  ldstr      "After loop"
    IL_008c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0091:  nop
    IL_0092:  nop
    IL_0093:  ldstr      "End of method"
    IL_0098:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009d:  nop
    IL_009e:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Loops::.ctor

} // end of class Loops


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\Loops.res
