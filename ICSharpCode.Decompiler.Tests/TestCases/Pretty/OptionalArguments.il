



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
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
  } // end of method OptionalArguments::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 num,
                               [opt] bool flag) cil managed
  {
    .param [2] = bool(true)
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
  } // end of method OptionalArguments::.ctor

  .method public hidebysig instance void 
          Add(string name,
              [opt] int32 a) cil managed
  {
    .param [2] = int32(0x00000005)
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::Add

  .method private hidebysig instance void 
          SimpleTests() cil managed
  {
    // Code size       70 (0x46)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  ldstr      "Test"
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.5
    IL_0011:  ldstr      "Test"
    IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_001b:  nop
    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.s   10
    IL_001f:  ldstr      "Hello World!"
    IL_0024:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Test(int32,
                                                                                                             string)
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  ldc.i4.s   10
    IL_002d:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0032:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Decimal(valuetype [mscorlib]System.Decimal)
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.5
    IL_003a:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_003f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Decimal(valuetype [mscorlib]System.Decimal)
    IL_0044:  nop
    IL_0045:  ret
  } // end of method OptionalArguments::SimpleTests

  .method private hidebysig instance void 
          Conflicts() cil managed
  {
    // Code size       123 (0x7b)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  ldc.i4.3
    IL_0004:  ldstr      "Hello"
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     string)
    IL_000e:  nop
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.5
    IL_0011:  ldc.i4.3
    IL_0012:  ldc.r8     3.141
    IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     float64)
    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.5
    IL_0023:  ldc.i4.3
    IL_0024:  ldnull
    IL_0025:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     string)
    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  ldc.i4.5
    IL_002d:  ldc.i4.3
    IL_002e:  ldc.r8     (00 00 00 00 00 00 F0 FF)
    IL_0037:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgument(int32,
                                                                                                                                     int32,
                                                                                                                                     float64)
    IL_003c:  nop
    IL_003d:  ldarg.0
    IL_003e:  ldc.i4.s   10
    IL_0040:  ldstr      "World"
    IL_0045:  ldnull
    IL_0046:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary(int32,
                                                                                                                                                  string,
                                                                                                                                                  string)
    IL_004b:  nop
    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.s   10
    IL_004f:  ldstr      "Hello"
    IL_0054:  ldnull
    IL_0055:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary(int32,
                                                                                                                                                  string,
                                                                                                                                                  class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments)
    IL_005a:  nop
    IL_005b:  ldarg.0
    IL_005c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount()
    IL_0061:  nop
    IL_0062:  ldarg.0
    IL_0063:  ldstr      "Hello"
    IL_0068:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount(string)
    IL_006d:  nop
    IL_006e:  ldarg.0
    IL_006f:  ldstr      "World"
    IL_0074:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::DifferenceInArgumentCount(string)
    IL_0079:  nop
    IL_007a:  ret
  } // end of method OptionalArguments::Conflicts

  .method private hidebysig instance void 
          ParamsTests() cil managed
  {
    // Code size       103 (0x67)
    .maxstack  5
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  ldc.i4.3
    IL_0004:  newarr     [mscorlib]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  stelem.i4
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.1
    IL_0011:  ldc.i4.s   9
    IL_0013:  stelem.i4
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.2
    IL_0016:  ldc.i4.8
    IL_0017:  stelem.i4
    IL_0018:  ldloc.0
    IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  ldnull
    IL_0021:  ldc.i4.0
    IL_0022:  newarr     [mscorlib]System.Int32
    IL_0027:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(string,
                                                                                                                     int32[])
    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  ldc.i4.5
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [mscorlib]System.Int32
    IL_0035:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_003a:  nop
    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.s   10
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [mscorlib]System.Int32
    IL_0044:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(int32,
                                                                                                                     int32[])
    IL_0049:  nop
    IL_004a:  ldarg.0
    IL_004b:  ldnull
    IL_004c:  ldc.i4.3
    IL_004d:  newarr     [mscorlib]System.Int32
    IL_0052:  stloc.0
    IL_0053:  ldloc.0
    IL_0054:  ldc.i4.0
    IL_0055:  ldc.i4.1
    IL_0056:  stelem.i4
    IL_0057:  ldloc.0
    IL_0058:  ldc.i4.1
    IL_0059:  ldc.i4.2
    IL_005a:  stelem.i4
    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.2
    IL_005d:  ldc.i4.3
    IL_005e:  stelem.i4
    IL_005f:  ldloc.0
    IL_0060:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::ParamsMethod(string,
                                                                                                                     int32[])
    IL_0065:  nop
    IL_0066:  ret
  } // end of method OptionalArguments::ParamsTests

  .method private hidebysig instance void 
          CallerInfo() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldstr      "CallerInfo"
    IL_0007:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerMemberName(string)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldnull
    IL_000f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerMemberName(string)
    IL_0014:  nop
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.s   60
    IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerLineNumber(int32)
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.0
    IL_0020:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::CallerLineNumber(int32)
    IL_0025:  nop
    IL_0026:  ret
  } // end of method OptionalArguments::CallerInfo

  .method private hidebysig instance void 
          Constructor([out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& a,
                      [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& b,
                      [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments& c) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "Hallo"
    IL_0007:  ldc.i4.5
    IL_0008:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(string,
                                                                                                              int32)
    IL_000d:  stind.ref
    IL_000e:  ldarg.2
    IL_000f:  ldc.i4.s   10
    IL_0011:  ldc.i4.1
    IL_0012:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(int32,
                                                                                                              bool)
    IL_0017:  stind.ref
    IL_0018:  ldarg.3
    IL_0019:  ldc.i4.s   10
    IL_001b:  ldc.i4.1
    IL_001c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::.ctor(int32,
                                                                                                              bool)
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldstr      "Test"
    IL_0028:  ldc.i4.s   10
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Add(string,
                                                                                                            int32)
    IL_002f:  nop
    IL_0030:  ldloc.0
    IL_0031:  ldstr      "Test2"
    IL_0036:  ldc.i4.5
    IL_0037:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments::Add(string,
                                                                                                            int32)
    IL_003c:  nop
    IL_003d:  ldloc.0
    IL_003e:  stind.ref
    IL_003f:  ret
  } // end of method OptionalArguments::Constructor

  .method private hidebysig instance void 
          CallerMemberName([opt] string memberName) cil managed
  {
    .param [1] = nullref
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerMemberNameAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::CallerMemberName

  .method private hidebysig instance void 
          CallerFilePath([opt] string filePath) cil managed
  {
    .param [1] = nullref
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerFilePathAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::CallerFilePath

  .method private hidebysig instance void 
          CallerLineNumber([opt] int32 lineNumber) cil managed
  {
    .param [1] = int32(0x00000000)
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CallerLineNumberAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::CallerLineNumber

  .method private hidebysig instance void 
          ParamsMethod([opt] int32 a,
                       int32[] values) cil managed
  {
    .param [1] = int32(0x00000005)
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::ParamsMethod

  .method private hidebysig instance void 
          ParamsMethod([opt] string a,
                       int32[] values) cil managed
  {
    .param [1] = nullref
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::ParamsMethod

  .method private hidebysig instance void 
          DifferenceInArgumentCount() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::DifferenceInArgumentCount

  .method private hidebysig instance void 
          DifferenceInArgumentCount([opt] string a) cil managed
  {
    .param [1] = "Hello"
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::DifferenceInArgumentCount

  .method private hidebysig instance void 
          Test([opt] int32 a,
               [opt] string b) cil managed
  {
    .param [1] = int32(0x0000000A)
    .param [2] = "Test"
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
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
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::Decimal

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgument(int32 a,
                                       int32 b,
                                       [opt] string c) cil managed
  {
    .param [3] = nullref
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgument

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgument(int32 a,
                                       int32 b,
                                       [opt] float64 d) cil managed
  {
    .param [3] = float64(0xFFF0000000000000) // -1.#INF
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgument

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgumentCastNecessary(int32 a,
                                                    string b,
                                                    [opt] string c) cil managed
  {
    .param [3] = nullref
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary

  .method private hidebysig instance void 
          OnlyDifferenceIsLastArgumentCastNecessary(int32 a,
                                                    string b,
                                                    [opt] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments args) cil managed
  {
    .param [3] = nullref
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method OptionalArguments::OnlyDifferenceIsLastArgumentCastNecessary

  .method private hidebysig instance string 
          Get([out] int32& a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method OptionalArguments::Get

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OptionalArguments


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
