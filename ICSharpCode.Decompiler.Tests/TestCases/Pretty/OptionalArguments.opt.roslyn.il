



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly OptionalArguments
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
.module OptionalArguments.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments
       extends class [mscorlib]System.Collections.Generic.List`1<int32>
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string name,
                               [opt] int32 a) cil managed
  {
    .param [2] = int32(0x00000005)
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method OptionalArguments::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 num,
                               [opt] bool flag) cil managed
  {
    .param [2] = bool(true)
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method OptionalArguments::.ctor

  .method public hidebysig instance void 
          Add(string name,
              [opt] int32 a) cil managed
  {
    .param [2] = int32(0x00000005)
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::Add

  .method private hidebysig instance void 
          SimpleTests() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   10
    IL_0003:  ldstr      "Test"
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.5
    IL_000f:  ldstr      "Test"
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_0019:  ldarg.0
    IL_001a:  ldc.i4.s   10
    IL_001c:  ldstr      "Hello World!"
    IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_0026:  ldarg.0
    IL_0027:  ldc.i4.s   10
    IL_0029:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_002e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Decimal(valuetype [mscorlib]System.Decimal)
    IL_0033:  ldarg.0
    IL_0034:  ldc.i4.5
    IL_0035:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_003a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Decimal(valuetype [mscorlib]System.Decimal)
    IL_003f:  ret
  } // end of method OptionalArguments::SimpleTests

  .method private hidebysig instance void 
          Conflicts() cil managed
  {
    // Code size       113 (0x71)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  ldc.i4.3
    IL_0003:  ldstr      "Hello"
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     string)
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.5
    IL_000f:  ldc.i4.3
    IL_0010:  ldc.r8     3.141
    IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     float64)
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.5
    IL_0020:  ldc.i4.3
    IL_0021:  ldnull
    IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     string)
    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.5
    IL_0029:  ldc.i4.3
    IL_002a:  ldc.r8     (00 00 00 00 00 00 F0 FF)
    IL_0033:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     float64)
    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.s   10
    IL_003b:  ldstr      "World"
    IL_0040:  ldnull
    IL_0041:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary(int32,
                                                                                                                                                  string,
                                                                                                                                                  string)
    IL_0046:  ldarg.0
    IL_0047:  ldc.i4.s   10
    IL_0049:  ldstr      "Hello"
    IL_004e:  ldnull
    IL_004f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary(int32,
                                                                                                                                                  string,
                                                                                                                                                  class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments)
    IL_0054:  ldarg.0
    IL_0055:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount()
    IL_005a:  ldarg.0
    IL_005b:  ldstr      "Hello"
    IL_0060:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount(string)
    IL_0065:  ldarg.0
    IL_0066:  ldstr      "World"
    IL_006b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount(string)
    IL_0070:  ret
  } // end of method OptionalArguments::Conflicts

  .method private hidebysig instance void 
          ParamsTests() cil managed
  {
    // Code size       86 (0x56)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  ldc.i4.3
    IL_0003:  newarr     [mscorlib]System.Int32
    IL_0008:  dup
    IL_0009:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::'0F3DD643C5167ACFC541F72809FFF828A6E41494'
    IL_000e:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_0018:  ldarg.0
    IL_0019:  ldnull
    IL_001a:  call       !!0[] [mscorlib]System.Array::Empty<int32>()
    IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(string,
                                                                                                                     int32[])
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.5
    IL_0026:  call       !!0[] [mscorlib]System.Array::Empty<int32>()
    IL_002b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_0030:  ldarg.0
    IL_0031:  ldc.i4.s   10
    IL_0033:  call       !!0[] [mscorlib]System.Array::Empty<int32>()
    IL_0038:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_003d:  ldarg.0
    IL_003e:  ldnull
    IL_003f:  ldc.i4.3
    IL_0040:  newarr     [mscorlib]System.Int32
    IL_0045:  dup
    IL_0046:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::E429CCA3F703A39CC5954A6572FEC9086135B34E
    IL_004b:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0050:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(string,
                                                                                                                     int32[])
    IL_0055:  ret
  } // end of method OptionalArguments::ParamsTests

  .method private hidebysig instance void 
          CallerInfo() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      "CallerInfo"
    IL_0006:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerMemberName(string)
    IL_000b:  ldarg.0
    IL_000c:  ldnull
    IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerMemberName(string)
    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.s   60
    IL_0015:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerLineNumber(int32)
    IL_001a:  ldarg.0
    IL_001b:  ldc.i4.0
    IL_001c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerLineNumber(int32)
    IL_0021:  ret
  } // end of method OptionalArguments::CallerInfo

  .method private hidebysig instance void 
          Constructor([out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& a,
                      [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& b,
                      [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& c) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "Hallo"
    IL_0006:  ldc.i4.5
    IL_0007:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(string,
                                                                                                              int32)
    IL_000c:  stind.ref
    IL_000d:  ldarg.2
    IL_000e:  ldc.i4.s   10
    IL_0010:  ldc.i4.1
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(int32,
                                                                                                              bool)
    IL_0016:  stind.ref
    IL_0017:  ldarg.3
    IL_0018:  ldc.i4.s   10
    IL_001a:  ldc.i4.1
    IL_001b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(int32,
                                                                                                              bool)
    IL_0020:  dup
    IL_0021:  ldstr      "Test"
    IL_0026:  ldc.i4.s   10
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Add(string,
                                                                                                            int32)
    IL_002d:  dup
    IL_002e:  ldstr      "Test2"
    IL_0033:  ldc.i4.5
    IL_0034:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Add(string,
                                                                                                            int32)
    IL_0039:  stind.ref
    IL_003a:  ret
  } // end of method OptionalArguments::Constructor

  .method private hidebysig instance void 
          CallerMemberName([opt] string memberName) cil managed
  {
    .param [1] = nullref
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerMemberNameAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::CallerMemberName

  .method private hidebysig instance void 
          CallerFilePath([opt] string filePath) cil managed
  {
    .param [1] = nullref
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerFilePathAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::CallerFilePath

  .method private hidebysig instance void 
          CallerLineNumber([opt] int32 lineNumber) cil managed
  {
    .param [1] = int32(0x00000000)
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerLineNumberAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::CallerLineNumber

  .method private hidebysig instance void 
          ParamsMethod([opt] int32 a,
                       int32[] values) cil managed
  {
    .param [1] = int32(0x00000005)
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::ParamsMethod

  .method private hidebysig instance void 
          ParamsMethod([opt] string a,
                       int32[] values) cil managed
  {
    .param [1] = nullref
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::ParamsMethod

  .method private hidebysig instance void 
          DifferenceInArgumentCount() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::DifferenceInArgumentCount

  .method private hidebysig instance void 
          DifferenceInArgumentCount([opt] string a) cil managed
  {
    .param [1] = "Hello"
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::DifferenceInArgumentCount

  .method private hidebysig instance void 
          Test([opt] int32 a,
               [opt] string b) cil managed
  {
    .param [1] = int32(0x0000000A)
    .param [2] = "Test"
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::Test

  .method private hidebysig instance void 
          Decimal([opt] valuetype [mscorlib]System.Decimal d) cil managed
  {
    .param [1]
    .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    uint32,
                                                                                                    uint32,
                                                                                                    uint32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 
                                                                                                                00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::Decimal

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgument(int32 a,
                                       int32 b,
                                       [opt] string c) cil managed
  {
    .param [3] = nullref
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgument

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgument(int32 a,
                                       int32 b,
                                       [opt] float64 d) cil managed
  {
    .param [3] = float64(0xFFF0000000000000) // -1.#INF
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgument

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgumentCastNecessary(int32 a,
                                                    string b,
                                                    [opt] string c) cil managed
  {
    .param [3] = nullref
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgumentCastNecessary(int32 a,
                                                    string b,
                                                    [opt] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments args) cil managed
  {
    .param [3] = nullref
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary

  .method private hidebysig instance string 
          Get([out] int32& a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method OptionalArguments::Get

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '0F3DD643C5167ACFC541F72809FFF828A6E41494' at I_00002DC4
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' E429CCA3F703A39CC5954A6572FEC9086135B34E at I_00002DD4
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_00002DC4 = bytearray (
                 0A 00 00 00 09 00 00 00 08 00 00 00) 
.data cil I_00002DD4 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
