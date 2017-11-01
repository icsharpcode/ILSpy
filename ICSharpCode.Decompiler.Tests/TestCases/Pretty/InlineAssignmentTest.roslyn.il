
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly InlineAssignmentTest
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
.module InlineAssignmentTest.dll
// MVID: {370735A1-C3D5-4BD3-8F9B-CC3F81FB734E}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00BE0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest
       extends [mscorlib]System.Object
{
  .field private int32 field1
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest field2
  .field private int32[] field3
  .field private int16 field4
  .field private int32 '<InstanceProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance int32 
          get_InstanceProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::'<InstanceProperty>k__BackingField'
    IL_0006:  ret
  } // end of method InlineAssignmentTest::get_InstanceProperty

  .method public hidebysig specialname instance void 
          set_InstanceProperty(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::'<InstanceProperty>k__BackingField'
    IL_0007:  ret
  } // end of method InlineAssignmentTest::set_InstanceProperty

  .method public hidebysig specialname static 
          int32  get_StaticProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::'<StaticProperty>k__BackingField'
    IL_0005:  ret
  } // end of method InlineAssignmentTest::get_StaticProperty

  .method public hidebysig specialname static 
          void  set_StaticProperty(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::'<StaticProperty>k__BackingField'
    IL_0006:  ret
  } // end of method InlineAssignmentTest::set_StaticProperty

  .method public hidebysig instance void 
          SimpleInlineWithLocals() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetFormat()
    IL_0007:  ldarg.0
    IL_0008:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000d:  dup
    IL_000e:  stloc.0
    IL_000f:  box        [mscorlib]System.Int32
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0019:  nop
    IL_001a:  ldloc.0
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetFormat()
    IL_0027:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_002c:  dup
    IL_002d:  stloc.1
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0033:  nop
    IL_0034:  ldloc.1
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_003a:  nop
    IL_003b:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithLocals

  .method public hidebysig instance void 
          SimpleInlineWithFields() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0016:  dup
    IL_0017:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields

  .method public hidebysig instance void 
          SimpleInlineWithFields2() cil managed
  {
    // Code size       154 (0x9a)
    .maxstack  4
    .locals init (int32 V_0,
             int16 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0022:  dup
    IL_0023:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002d:  nop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  ldarg.0
    IL_003b:  ldc.i4.6
    IL_003c:  dup
    IL_003d:  stloc.1
    IL_003e:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0043:  ldloc.1
    IL_0044:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0049:  pop
    IL_004a:  ldarg.0
    IL_004b:  ldarg.0
    IL_004c:  ldc.i4     0xffffd8f0
    IL_0051:  dup
    IL_0052:  stloc.1
    IL_0053:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0058:  ldloc.1
    IL_0059:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_005e:  pop
    IL_005f:  ldarg.0
    IL_0060:  ldarg.0
    IL_0061:  ldarg.0
    IL_0062:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0067:  conv.i2
    IL_0068:  dup
    IL_0069:  stloc.1
    IL_006a:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_006f:  ldloc.1
    IL_0070:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0075:  pop
    IL_0076:  ldarg.0
    IL_0077:  ldarg.0
    IL_0078:  ldarg.0
    IL_0079:  ldc.i4.0
    IL_007a:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_007f:  dup
    IL_0080:  stloc.1
    IL_0081:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0086:  ldloc.1
    IL_0087:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_008c:  pop
    IL_008d:  ldarg.0
    IL_008e:  ldfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0093:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0098:  nop
    IL_0099:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields2

  .method public hidebysig instance int16 
          UseShort(int16 s) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (int16 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0007:  nop
    IL_0008:  ldarg.1
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method InlineAssignmentTest::UseShort

  .method public hidebysig instance void 
          ReadLoop1(class [mscorlib]System.IO.TextReader r) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (string V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  br.s       IL_000c

    IL_0003:  nop
    IL_0004:  ldloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  nop
    IL_000b:  nop
    IL_000c:  ldarg.1
    IL_000d:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadLine()
    IL_0012:  dup
    IL_0013:  stloc.0
    IL_0014:  ldnull
    IL_0015:  cgt.un
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0003

    IL_001b:  ret
  } // end of method InlineAssignmentTest::ReadLoop1

  .method public hidebysig instance void 
          AccessArray(int32[] a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.i4
    IL_0004:  dup
    IL_0005:  stloc.0
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000b:  nop
    IL_000c:  ldarg.1
    IL_000d:  ldloc.0
    IL_000e:  ldloc.0
    IL_000f:  dup
    IL_0010:  stloc.1
    IL_0011:  stelem.i4
    IL_0012:  ldloc.1
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  nop
    IL_0019:  ret
  } // end of method InlineAssignmentTest::AccessArray

  .method public hidebysig instance int32 
          Return(int32& a) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stind.i4
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.1
    IL_000b:  ret
  } // end of method InlineAssignmentTest::Return

  .method public hidebysig instance int32 
          Array(int32[] a,
                int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.2
    IL_0004:  dup
    IL_0005:  stloc.0
    IL_0006:  stelem.i4
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.1
    IL_000c:  ret
  } // end of method InlineAssignmentTest::Array

  .method public hidebysig instance int32 
          Array2(int32 i) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field3
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.1
    IL_0009:  dup
    IL_000a:  stloc.0
    IL_000b:  stelem.i4
    IL_000c:  ldloc.0
    IL_000d:  stloc.1
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.1
    IL_0011:  ret
  } // end of method InlineAssignmentTest::Array2

  .method public hidebysig instance int32 
          GetIndex() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.Random::.ctor()
    IL_0006:  ldc.i4.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  callvirt   instance int32 [mscorlib]System.Random::Next(int32,
                                                                      int32)
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.0
    IL_0012:  ret
  } // end of method InlineAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method InlineAssignmentTest::GetArray

  .method public hidebysig instance string 
          GetFormat() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "{0}"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method InlineAssignmentTest::GetFormat

  .method public hidebysig instance int32 
          GetValue(int32 'value') cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InlineAssignmentTest::GetValue

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetArray()
    IL_0007:  ldarg.0
    IL_0008:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000d:  ldarg.0
    IL_000e:  ldarg.0
    IL_000f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0014:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetValue(int32)
    IL_0019:  dup
    IL_001a:  stloc.0
    IL_001b:  stelem.i4
    IL_001c:  ldloc.0
    IL_001d:  stloc.1
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.1
    IL_0021:  ret
  } // end of method InlineAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig instance int32 
          StaticPropertyTest() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0007:  dup
    IL_0008:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_StaticProperty(int32)
    IL_000d:  nop
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.0
    IL_0012:  ret
  } // end of method InlineAssignmentTest::StaticPropertyTest

  .method public hidebysig instance int32 
          InstancePropertyTest() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0008:  dup
    IL_0009:  stloc.0
    IL_000a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_InstanceProperty(int32)
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  stloc.1
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.1
    IL_0015:  ret
  } // end of method InlineAssignmentTest::InstancePropertyTest

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method InlineAssignmentTest::.ctor

  .property instance int32 InstanceProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::get_InstanceProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_InstanceProperty(int32)
  } // end of property InlineAssignmentTest::InstanceProperty
  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_StaticProperty(int32)
  } // end of property InlineAssignmentTest::StaticProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
