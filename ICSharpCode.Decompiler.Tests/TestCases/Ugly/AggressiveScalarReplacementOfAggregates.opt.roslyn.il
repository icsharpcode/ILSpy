



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly AggressiveScalarReplacementOfAggregates
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
.module AggressiveScalarReplacementOfAggregates.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass
       extends [mscorlib]System.Object
{
  .field public class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program thisField
  .field public int32 field1
  .field public string field2
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method DisplayClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass
       extends [mscorlib]System.Object
{
  .field public class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass field3
  .field public int32 field1
  .field public string field2
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NestedDisplayClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program
       extends [mscorlib]System.Object
{
  .method public hidebysig instance int32 
          Rand() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method Program::Rand

  .method public hidebysig instance void 
          Test1() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.s   42
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldstr      "{0} {1}"
    IL_001e:  ldloc.0
    IL_001f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldloc.0
    IL_002a:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0034:  ret
  } // end of method Program::Test1

  .method public hidebysig instance void 
          Test2() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.s   42
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldstr      "{0} {1}"
    IL_001e:  ldloc.0
    IL_001f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldloc.0
    IL_002a:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_002f:  box        [mscorlib]System.Int32
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0039:  ret
  } // end of method Program::Test2

  .method public hidebysig instance void 
          Test3() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.s   42
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldstr      "{0} {1}"
    IL_001e:  ldloc.0
    IL_001f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldloc.0
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_002f:  ret
  } // end of method Program::Test3

  .method public hidebysig instance void 
          Test4() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000c:  dup
    IL_000d:  ldc.i4.s   42
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0014:  dup
    IL_0015:  ldstr      "Hello World!"
    IL_001a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_001f:  stloc.0
    IL_0020:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_0025:  dup
    IL_0026:  ldc.i4     0x1267
    IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0030:  dup
    IL_0031:  ldstr      "ILSpy"
    IL_0036:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_003b:  stloc.1
    IL_003c:  ldloc.0
    IL_003d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0042:  ldc.i4.s   100
    IL_0044:  ble.s      IL_004f

    IL_0046:  ldloc.1
    IL_0047:  ldloc.0
    IL_0048:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_004d:  br.s       IL_0056

    IL_004f:  ldloc.1
    IL_0050:  ldnull
    IL_0051:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0056:  ldstr      "{0} {1}"
    IL_005b:  ldloc.0
    IL_005c:  ldloc.1
    IL_005d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0067:  ret
  } // end of method Program::Test4

  .method public hidebysig instance void 
          Test5() cil managed
  {
    // Code size       125 (0x7d)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000c:  dup
    IL_000d:  ldc.i4.s   42
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0014:  dup
    IL_0015:  ldstr      "Hello World!"
    IL_001a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_001f:  stloc.0
    IL_0020:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_0025:  dup
    IL_0026:  ldc.i4     0x1267
    IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0030:  dup
    IL_0031:  ldstr      "ILSpy"
    IL_0036:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_003b:  stloc.1
    IL_003c:  ldloc.0
    IL_003d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0042:  ldc.i4.s   100
    IL_0044:  ble.s      IL_004f

    IL_0046:  ldloc.1
    IL_0047:  ldloc.0
    IL_0048:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_004d:  br.s       IL_0056

    IL_004f:  ldloc.1
    IL_0050:  ldnull
    IL_0051:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0056:  ldstr      "{0} {1}"
    IL_005b:  ldloc.1
    IL_005c:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_0061:  ldloc.1
    IL_0062:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0067:  call       instance string [mscorlib]System.Int32::ToString()
    IL_006c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0071:  ldloc.1
    IL_0072:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_007c:  ret
  } // end of method Program::Test5

  .method public hidebysig instance void 
          Issue1898(int32 i) cil managed
  {
    // Code size       135 (0x87)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000c:  dup
    IL_000d:  ldarg.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0013:  stloc.0
    IL_0014:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_0019:  stloc.1
    IL_001a:  ldarg.0
    IL_001b:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_0020:  stloc.2
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.1
    IL_0023:  sub
    IL_0024:  switch     ( 
                          IL_0037,
                          IL_0045,
                          IL_005b)
    IL_0035:  br.s       IL_0064

    IL_0037:  ldloc.1
    IL_0038:  ldarg.0
    IL_0039:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0043:  br.s       IL_001a

    IL_0045:  ldloc.1
    IL_0046:  ldarg.0
    IL_0047:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_004c:  stloc.3
    IL_004d:  ldloca.s   V_3
    IL_004f:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0054:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_0059:  br.s       IL_001a

    IL_005b:  ldloc.1
    IL_005c:  ldloc.0
    IL_005d:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0062:  br.s       IL_001a

    IL_0064:  ldloc.1
    IL_0065:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_006f:  ldloc.1
    IL_0070:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_0075:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007a:  ldloc.1
    IL_007b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0085:  br.s       IL_001a
  } // end of method Program::Issue1898

  .method public hidebysig instance void 
          Test6(int32 i) cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.1
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000c:  dup
    IL_000d:  ldstr      "Hello World!"
    IL_0012:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0017:  stloc.0
    IL_0018:  ldarg.1
    IL_0019:  ldc.i4.0
    IL_001a:  bge.s      IL_0020

    IL_001c:  ldarg.1
    IL_001d:  neg
    IL_001e:  starg.s    i
    IL_0020:  ldstr      "{0} {1}"
    IL_0025:  ldloc.0
    IL_0026:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_002b:  box        [mscorlib]System.Int32
    IL_0030:  ldloc.0
    IL_0031:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_003b:  ret
  } // end of method Program::Test6

  .method public hidebysig instance void 
          Test6b(int32 i) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0007:  dup
    IL_0008:  ldloc.0
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000e:  dup
    IL_000f:  ldstr      "Hello World!"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0019:  stloc.1
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  bge.s      IL_0021

    IL_001e:  ldloc.0
    IL_001f:  neg
    IL_0020:  stloc.0
    IL_0021:  ldstr      "{0} {1}"
    IL_0026:  ldloc.1
    IL_0027:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_002c:  box        [mscorlib]System.Int32
    IL_0031:  ldloc.1
    IL_0032:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_003c:  ret
  } // end of method Program::Test6b

  .method public hidebysig instance void 
          Test7(int32 i) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             int32 V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.1
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000c:  dup
    IL_000d:  ldstr      "Hello World!"
    IL_0012:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0017:  stloc.0
    IL_0018:  ldstr      "{0} {1} {2}"
    IL_001d:  ldloc.0
    IL_001e:  dup
    IL_001f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0024:  stloc.1
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_002d:  ldloc.1
    IL_002e:  box        [mscorlib]System.Int32
    IL_0033:  ldloc.0
    IL_0034:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0039:  ldarg.1
    IL_003a:  box        [mscorlib]System.Int32
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object,
                                                                  object)
    IL_0044:  ret
  } // end of method Program::Test7

  .method public hidebysig instance void 
          Test8(int32 i) cil managed
  {
    // Code size       56 (0x38)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.1
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000c:  dup
    IL_000d:  ldstr      "Hello World!"
    IL_0012:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0017:  stloc.0
    IL_0018:  ldc.i4.s   42
    IL_001a:  starg.s    i
    IL_001c:  ldstr      "{0} {1}"
    IL_0021:  ldloc.0
    IL_0022:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0027:  box        [mscorlib]System.Int32
    IL_002c:  ldloc.0
    IL_002d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0037:  ret
  } // end of method Program::Test8

  .method public hidebysig instance void 
          Test8b(int32 i) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0007:  dup
    IL_0008:  ldloc.0
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000e:  dup
    IL_000f:  ldstr      "Hello World!"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.s   42
    IL_001c:  stloc.0
    IL_001d:  ldstr      "{0} {1}"
    IL_0022:  ldloc.1
    IL_0023:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0028:  box        [mscorlib]System.Int32
    IL_002d:  ldloc.1
    IL_002e:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0038:  ret
  } // end of method Program::Test8b

  .method public hidebysig instance void 
          Test9() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000c:  dup
    IL_000d:  ldc.i4.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0013:  dup
    IL_0014:  ldstr      "Hello World!"
    IL_0019:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::.ctor()
    IL_0025:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_002a:  ldstr      "{0} {1}"
    IL_002f:  ldarg.0
    IL_0030:  ldloc.0
    IL_0031:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_003b:  ret
  } // end of method Program::Test9

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
