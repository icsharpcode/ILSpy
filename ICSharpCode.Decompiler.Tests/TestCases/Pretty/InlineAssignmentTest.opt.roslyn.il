
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module InlineAssignmentTest.dll
// MVID: {2D549318-D869-4294-BA40-CB6EEA5D9CE2}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02CB0000


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
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
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
    // Code size       55 (0x37)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetFormat()
    IL_0006:  ldarg.0
    IL_0007:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000c:  dup
    IL_000d:  stloc.0
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.0
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001e:  ldarg.0
    IL_001f:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetFormat()
    IL_0024:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0029:  dup
    IL_002a:  stloc.1
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0030:  ldloc.1
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0036:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithLocals

  .method public hidebysig instance void 
          SimpleInlineWithFields() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0009:  ldloc.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0014:  dup
    IL_0015:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001f:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields

  .method public hidebysig instance void 
          SimpleInlineWithFields2() cil managed
  {
    // Code size       148 (0x94)
    .maxstack  4
    .locals init (int32 V_0,
             int16 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0009:  ldloc.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ldarg.0
    IL_0010:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_001f:  dup
    IL_0020:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0034:  ldarg.0
    IL_0035:  ldarg.0
    IL_0036:  ldc.i4.6
    IL_0037:  dup
    IL_0038:  stloc.1
    IL_0039:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_003e:  ldloc.1
    IL_003f:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0044:  pop
    IL_0045:  ldarg.0
    IL_0046:  ldarg.0
    IL_0047:  ldc.i4     0xffffd8f0
    IL_004c:  dup
    IL_004d:  stloc.1
    IL_004e:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0053:  ldloc.1
    IL_0054:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0059:  pop
    IL_005a:  ldarg.0
    IL_005b:  ldarg.0
    IL_005c:  ldarg.0
    IL_005d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0062:  conv.i2
    IL_0063:  dup
    IL_0064:  stloc.1
    IL_0065:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_006a:  ldloc.1
    IL_006b:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0070:  pop
    IL_0071:  ldarg.0
    IL_0072:  ldarg.0
    IL_0073:  ldarg.0
    IL_0074:  ldc.i4.0
    IL_0075:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_007a:  dup
    IL_007b:  stloc.1
    IL_007c:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_0081:  ldloc.1
    IL_0082:  call       instance int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::UseShort(int16)
    IL_0087:  pop
    IL_0088:  ldarg.0
    IL_0089:  ldfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field4
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0093:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields2

  .method public hidebysig instance int16 
          UseShort(int16 s) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0006:  ldarg.1
    IL_0007:  ret
  } // end of method InlineAssignmentTest::UseShort

  .method public hidebysig instance void 
          ReadLoop1(class [mscorlib]System.IO.TextReader r) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  br.s       IL_0008

    IL_0002:  ldloc.0
    IL_0003:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0008:  ldarg.1
    IL_0009:  callvirt   instance string [mscorlib]System.IO.TextReader::ReadLine()
    IL_000e:  dup
    IL_000f:  stloc.0
    IL_0010:  brtrue.s   IL_0002

    IL_0012:  ret
  } // end of method InlineAssignmentTest::ReadLoop1

  .method public hidebysig instance void 
          AccessArray(int32[] a) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.i4
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldarg.1
    IL_000b:  ldloc.0
    IL_000c:  ldloc.0
    IL_000d:  dup
    IL_000e:  stloc.1
    IL_000f:  stelem.i4
    IL_0010:  ldloc.1
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  ret
  } // end of method InlineAssignmentTest::AccessArray

  .method public hidebysig instance int32 
          Return(int32& a) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stind.i4
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InlineAssignmentTest::Return

  .method public hidebysig instance int32 
          Array(int32[] a,
                int32 i) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.2
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stelem.i4
    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method InlineAssignmentTest::Array

  .method public hidebysig instance int32 
          Array2(int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field3
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.1
    IL_0008:  dup
    IL_0009:  stloc.0
    IL_000a:  stelem.i4
    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method InlineAssignmentTest::Array2

  .method public hidebysig instance int32 
          GetIndex() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.Random::.ctor()
    IL_0005:  ldc.i4.0
    IL_0006:  ldc.i4.s   100
    IL_0008:  callvirt   instance int32 [mscorlib]System.Random::Next(int32,
                                                                      int32)
    IL_000d:  ret
  } // end of method InlineAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method InlineAssignmentTest::GetArray

  .method public hidebysig instance string 
          GetFormat() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "{0}"
    IL_0005:  ret
  } // end of method InlineAssignmentTest::GetFormat

  .method public hidebysig instance int32 
          GetValue(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method InlineAssignmentTest::GetValue

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetArray()
    IL_0006:  ldarg.0
    IL_0007:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000c:  ldarg.0
    IL_000d:  ldarg.0
    IL_000e:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0013:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetValue(int32)
    IL_0018:  dup
    IL_0019:  stloc.0
    IL_001a:  stelem.i4
    IL_001b:  ldloc.0
    IL_001c:  ret
  } // end of method InlineAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig instance int32 
          StaticPropertyTest() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0006:  dup
    IL_0007:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_StaticProperty(int32)
    IL_000c:  ret
  } // end of method InlineAssignmentTest::StaticPropertyTest

  .method public hidebysig instance int32 
          InstancePropertyTest() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0007:  dup
    IL_0008:  stloc.0
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::set_InstanceProperty(int32)
    IL_000e:  ldloc.0
    IL_000f:  ret
  } // end of method InlineAssignmentTest::InstancePropertyTest

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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
