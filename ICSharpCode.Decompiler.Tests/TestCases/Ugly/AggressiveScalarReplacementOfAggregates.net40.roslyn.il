



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmp4B37
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
.module tmp4B37.tmp
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
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NestedDisplayClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program
       extends [mscorlib]System.Object
{
  .method public hidebysig instance int32 
          Rand() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method Program::Rand

  .method public hidebysig instance void 
          Test1() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000e:  dup
    IL_000f:  ldstr      "Hello World!"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0019:  stloc.0
    IL_001a:  ldstr      "{0} {1}"
    IL_001f:  ldloc.0
    IL_0020:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0025:  box        [mscorlib]System.Int32
    IL_002a:  ldloc.0
    IL_002b:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0035:  nop
    IL_0036:  ret
  } // end of method Program::Test1

  .method public hidebysig instance void 
          Test2() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000e:  dup
    IL_000f:  ldstr      "Hello World!"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0019:  stloc.0
    IL_001a:  ldstr      "{0} {1}"
    IL_001f:  ldloc.0
    IL_0020:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0025:  box        [mscorlib]System.Int32
    IL_002a:  ldloc.0
    IL_002b:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_0030:  box        [mscorlib]System.Int32
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_003a:  nop
    IL_003b:  ret
  } // end of method Program::Test2

  .method public hidebysig instance void 
          Test3() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000e:  dup
    IL_000f:  ldstr      "Hello World!"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0019:  stloc.0
    IL_001a:  ldstr      "{0} {1}"
    IL_001f:  ldloc.0
    IL_0020:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0025:  box        [mscorlib]System.Int32
    IL_002a:  ldloc.0
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0030:  nop
    IL_0031:  ret
  } // end of method Program::Test3

  .method public hidebysig instance void 
          Test4() cil managed
  {
    // Code size       114 (0x72)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000d:  dup
    IL_000e:  ldc.i4.s   42
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0015:  dup
    IL_0016:  ldstr      "Hello World!"
    IL_001b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0020:  stloc.0
    IL_0021:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_0026:  dup
    IL_0027:  ldc.i4     0x1267
    IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0031:  dup
    IL_0032:  ldstr      "ILSpy"
    IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_003c:  stloc.1
    IL_003d:  ldloc.0
    IL_003e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0043:  ldc.i4.s   100
    IL_0045:  cgt
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  brfalse.s  IL_0056

    IL_004b:  nop
    IL_004c:  ldloc.1
    IL_004d:  ldloc.0
    IL_004e:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0053:  nop
    IL_0054:  br.s       IL_005f

    IL_0056:  nop
    IL_0057:  ldloc.1
    IL_0058:  ldnull
    IL_0059:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_005e:  nop
    IL_005f:  ldstr      "{0} {1}"
    IL_0064:  ldloc.0
    IL_0065:  ldloc.1
    IL_0066:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_006b:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0070:  nop
    IL_0071:  ret
  } // end of method Program::Test4

  .method public hidebysig instance void 
          Test5() cil managed
  {
    // Code size       135 (0x87)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000d:  dup
    IL_000e:  ldc.i4.s   42
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0015:  dup
    IL_0016:  ldstr      "Hello World!"
    IL_001b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0020:  stloc.0
    IL_0021:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_0026:  dup
    IL_0027:  ldc.i4     0x1267
    IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0031:  dup
    IL_0032:  ldstr      "ILSpy"
    IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_003c:  stloc.1
    IL_003d:  ldloc.0
    IL_003e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0043:  ldc.i4.s   100
    IL_0045:  cgt
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  brfalse.s  IL_0056

    IL_004b:  nop
    IL_004c:  ldloc.1
    IL_004d:  ldloc.0
    IL_004e:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0053:  nop
    IL_0054:  br.s       IL_005f

    IL_0056:  nop
    IL_0057:  ldloc.1
    IL_0058:  ldnull
    IL_0059:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_005e:  nop
    IL_005f:  ldstr      "{0} {1}"
    IL_0064:  ldloc.1
    IL_0065:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_006a:  ldloc.1
    IL_006b:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0070:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0075:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_007a:  ldloc.1
    IL_007b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0085:  nop
    IL_0086:  ret
  } // end of method Program::Test5

  .method public hidebysig instance void 
          Issue1898(int32 i) cil managed
  {
    // Code size       151 (0x97)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::thisField
    IL_000d:  dup
    IL_000e:  ldarg.1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0014:  stloc.0
    IL_0015:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::.ctor()
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_0092

    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_0024:  stloc.3
    IL_0025:  ldloc.3
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  sub
    IL_002a:  switch     ( 
                          IL_003d,
                          IL_004b,
                          IL_0062)
    IL_003b:  br.s       IL_006b

    IL_003d:  ldloc.1
    IL_003e:  ldarg.0
    IL_003f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0049:  br.s       IL_0091

    IL_004b:  ldloc.1
    IL_004c:  ldarg.0
    IL_004d:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program::Rand()
    IL_0052:  stloc.s    V_4
    IL_0054:  ldloca.s   V_4
    IL_0056:  call       instance string [mscorlib]System.Int32::ToString()
    IL_005b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_0060:  br.s       IL_0091

    IL_0062:  ldloc.1
    IL_0063:  ldloc.0
    IL_0064:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0069:  br.s       IL_0091

    IL_006b:  ldloc.1
    IL_006c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field1
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0076:  nop
    IL_0077:  ldloc.1
    IL_0078:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field2
    IL_007d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0082:  nop
    IL_0083:  ldloc.1
    IL_0084:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass ICSharpCode.Decompiler.Tests.TestCases.Ugly.NestedDisplayClass::field3
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_008e:  nop
    IL_008f:  br.s       IL_0091

    IL_0091:  nop
    IL_0092:  ldc.i4.1
    IL_0093:  stloc.s    V_5
    IL_0095:  br.s       IL_001d
  } // end of method Program::Issue1898

  .method public hidebysig instance void 
          Test6(int32 i) cil managed
  {
    // Code size       68 (0x44)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldarg.1
    IL_001a:  ldc.i4.0
    IL_001b:  clt
    IL_001d:  stloc.1
    IL_001e:  ldloc.1
    IL_001f:  brfalse.s  IL_0027

    IL_0021:  nop
    IL_0022:  ldarg.1
    IL_0023:  neg
    IL_0024:  starg.s    i
    IL_0026:  nop
    IL_0027:  ldstr      "{0} {1}"
    IL_002c:  ldloc.0
    IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldloc.0
    IL_0038:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0042:  nop
    IL_0043:  ret
  } // end of method Program::Test6

  .method public hidebysig instance void 
          Test6b(int32 i) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0008:  dup
    IL_0009:  ldloc.0
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000f:  dup
    IL_0010:  ldstr      "Hello World!"
    IL_0015:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_001a:  stloc.1
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  clt
    IL_001f:  stloc.2
    IL_0020:  ldloc.2
    IL_0021:  brfalse.s  IL_0028

    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  neg
    IL_0026:  stloc.0
    IL_0027:  nop
    IL_0028:  ldstr      "{0} {1}"
    IL_002d:  ldloc.1
    IL_002e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldloc.1
    IL_0039:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0043:  nop
    IL_0044:  ret
  } // end of method Program::Test6b

  .method public hidebysig instance void 
          Test7(int32 i) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldstr      "{0} {1} {2}"
    IL_001e:  ldloc.0
    IL_001f:  dup
    IL_0020:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_002e:  ldloc.1
    IL_002f:  box        [mscorlib]System.Int32
    IL_0034:  ldloc.0
    IL_0035:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_003a:  ldarg.1
    IL_003b:  box        [mscorlib]System.Int32
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object,
                                                                  object)
    IL_0045:  nop
    IL_0046:  ret
  } // end of method Program::Test7

  .method public hidebysig instance void 
          Test8(int32 i) cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0006:  dup
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000d:  dup
    IL_000e:  ldstr      "Hello World!"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0018:  stloc.0
    IL_0019:  ldc.i4.s   42
    IL_001b:  starg.s    i
    IL_001d:  ldstr      "{0} {1}"
    IL_0022:  ldloc.0
    IL_0023:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0028:  box        [mscorlib]System.Int32
    IL_002d:  ldloc.0
    IL_002e:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method Program::Test8

  .method public hidebysig instance void 
          Test8b(int32 i) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::.ctor()
    IL_0008:  dup
    IL_0009:  ldloc.0
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_000f:  dup
    IL_0010:  ldstr      "Hello World!"
    IL_0015:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.s   42
    IL_001d:  stloc.0
    IL_001e:  ldstr      "{0} {1}"
    IL_0023:  ldloc.1
    IL_0024:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field1
    IL_0029:  box        [mscorlib]System.Int32
    IL_002e:  ldloc.1
    IL_002f:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Ugly.DisplayClass::field2
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0039:  nop
    IL_003a:  ret
  } // end of method Program::Test8b

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Program::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
