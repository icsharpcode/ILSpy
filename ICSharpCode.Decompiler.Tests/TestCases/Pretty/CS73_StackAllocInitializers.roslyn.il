



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Memory
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 4:0:1:0
}
.assembly CS73_StackAllocInitializers
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
.module CS73_StackAllocInitializers.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit StructWithSize5
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 5
    .field public uint8 a
    .field public uint8 b
    .field public uint8 c
    .field public uint8 d
    .field public uint8 e
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(uint8 a,
                                 uint8 b,
                                 uint8 c,
                                 uint8 d,
                                 uint8 e) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::a
      IL_0008:  ldarg.0
      IL_0009:  ldarg.2
      IL_000a:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::b
      IL_000f:  ldarg.0
      IL_0010:  ldarg.3
      IL_0011:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::c
      IL_0016:  ldarg.0
      IL_0017:  ldarg.s    d
      IL_0019:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::d
      IL_001e:  ldarg.0
      IL_001f:  ldarg.s    e
      IL_0021:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::e
      IL_0026:  ret
    } // end of method StructWithSize5::.ctor

  } // end of class StructWithSize5

  .method public hidebysig instance string 
          SimpleStackAllocStruct1() cil managed
  {
    // Code size       143 (0x8f)
    .maxstack  7
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  conv.u
    IL_0003:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0009:  mul.ovf.un
    IL_000a:  localloc
    IL_000c:  dup
    IL_000d:  ldc.i4.1
    IL_000e:  ldc.i4.2
    IL_000f:  ldc.i4.3
    IL_0010:  ldc.i4.4
    IL_0011:  ldc.i4.5
    IL_0012:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_0017:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_001c:  dup
    IL_001d:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0023:  add
    IL_0024:  ldc.i4.s   11
    IL_0026:  ldc.i4.s   22
    IL_0028:  ldc.i4.s   33
    IL_002a:  ldc.i4.s   44
    IL_002c:  ldc.i4.s   55
    IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_0033:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0038:  dup
    IL_0039:  ldc.i4.2
    IL_003a:  conv.i
    IL_003b:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0041:  mul
    IL_0042:  add
    IL_0043:  ldc.i4.1
    IL_0044:  ldc.i4.4
    IL_0045:  ldc.i4.8
    IL_0046:  ldc.i4.6
    IL_0047:  ldc.i4.2
    IL_0048:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_004d:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0052:  dup
    IL_0053:  ldc.i4.3
    IL_0054:  conv.i
    IL_0055:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_005b:  mul
    IL_005c:  add
    IL_005d:  ldc.i4.s   12
    IL_005f:  ldc.i4.s   23
    IL_0061:  ldc.i4.s   34
    IL_0063:  ldc.i4.s   45
    IL_0065:  ldc.i4.s   56
    IL_0067:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_006c:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0071:  stloc.0
    IL_0072:  ldloc.0
    IL_0073:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0078:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_007d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0082:  nop
    IL_0083:  ldarg.0
    IL_0084:  ldloc.0
    IL_0085:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_008a:  stloc.1
    IL_008b:  br.s       IL_008d

    IL_008d:  ldloc.1
    IL_008e:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocStruct1

  .method public hidebysig instance string 
          SimpleStackAllocBool() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  4
    .locals init (bool* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldsflda    int32 '<PrivateImplementationDetails>'::'063AAB58782881806084E1A944FBCEE5F5815405'
    IL_000b:  ldc.i4.4
    IL_000c:  cpblk
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldind.u1
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  ldloc.0
    IL_0019:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001e:  stloc.1
    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocBool

  .method public hidebysig instance string 
          DoNotInlineTest() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  4
    .locals init (bool* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldsflda    int32 '<PrivateImplementationDetails>'::'063AAB58782881806084E1A944FBCEE5F5815405'
    IL_000b:  ldc.i4.4
    IL_000c:  cpblk
    IL_000e:  stloc.0
    IL_000f:  ldarg.0
    IL_0010:  ldloc.0
    IL_0011:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method CS73_StackAllocInitializers::DoNotInlineTest

  .method public hidebysig instance string 
          SimpleStackAllocByte() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  3
    .locals init (uint8* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.0
    IL_0007:  stind.i1
    IL_0008:  dup
    IL_0009:  ldc.i4.1
    IL_000a:  add
    IL_000b:  ldc.i4.1
    IL_000c:  stind.i1
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldind.u1
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0015:  nop
    IL_0016:  ldarg.0
    IL_0017:  ldloc.0
    IL_0018:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001d:  stloc.1
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.1
    IL_0021:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocByte

  .method public hidebysig instance string 
          SimpleStackAllocPrimesAsBytes() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (uint8* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   55
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldsflda    valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=55' '<PrivateImplementationDetails>'::F623596D706F878F1D12C19353913A8E96904144
    IL_000c:  ldc.i4.s   55
    IL_000e:  cpblk
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  ldind.u1
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  nop
    IL_0019:  ldarg.0
    IL_001a:  ldloc.0
    IL_001b:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocPrimesAsBytes

  .method public hidebysig instance string 
          SimpleStackAllocChar() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  4
    .locals init (char* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.8
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.s   49
    IL_0008:  stind.i2
    IL_0009:  dup
    IL_000a:  ldc.i4.2
    IL_000b:  add
    IL_000c:  ldc.i4.s   50
    IL_000e:  stind.i2
    IL_000f:  dup
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i
    IL_0012:  ldc.i4.2
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldc.i4.s   51
    IL_0017:  stind.i2
    IL_0018:  dup
    IL_0019:  ldc.i4.3
    IL_001a:  conv.i
    IL_001b:  ldc.i4.2
    IL_001c:  mul
    IL_001d:  add
    IL_001e:  ldc.i4.s   52
    IL_0020:  stind.i2
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldind.u2
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(char)
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  ldloc.0
    IL_002c:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0031:  stloc.1
    IL_0032:  br.s       IL_0034

    IL_0034:  ldloc.1
    IL_0035:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocChar

  .method public hidebysig instance string 
          SimpleStackAllocCharAlphabet() cil managed
  {
    // Code size       270 (0x10e)
    .maxstack  4
    .locals init (char* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   52
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.s   65
    IL_0009:  stind.i2
    IL_000a:  dup
    IL_000b:  ldc.i4.2
    IL_000c:  add
    IL_000d:  ldc.i4.s   66
    IL_000f:  stind.i2
    IL_0010:  dup
    IL_0011:  ldc.i4.2
    IL_0012:  conv.i
    IL_0013:  ldc.i4.2
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldc.i4.s   67
    IL_0018:  stind.i2
    IL_0019:  dup
    IL_001a:  ldc.i4.3
    IL_001b:  conv.i
    IL_001c:  ldc.i4.2
    IL_001d:  mul
    IL_001e:  add
    IL_001f:  ldc.i4.s   68
    IL_0021:  stind.i2
    IL_0022:  dup
    IL_0023:  ldc.i4.4
    IL_0024:  conv.i
    IL_0025:  ldc.i4.2
    IL_0026:  mul
    IL_0027:  add
    IL_0028:  ldc.i4.s   69
    IL_002a:  stind.i2
    IL_002b:  dup
    IL_002c:  ldc.i4.5
    IL_002d:  conv.i
    IL_002e:  ldc.i4.2
    IL_002f:  mul
    IL_0030:  add
    IL_0031:  ldc.i4.s   70
    IL_0033:  stind.i2
    IL_0034:  dup
    IL_0035:  ldc.i4.6
    IL_0036:  conv.i
    IL_0037:  ldc.i4.2
    IL_0038:  mul
    IL_0039:  add
    IL_003a:  ldc.i4.s   71
    IL_003c:  stind.i2
    IL_003d:  dup
    IL_003e:  ldc.i4.7
    IL_003f:  conv.i
    IL_0040:  ldc.i4.2
    IL_0041:  mul
    IL_0042:  add
    IL_0043:  ldc.i4.s   72
    IL_0045:  stind.i2
    IL_0046:  dup
    IL_0047:  ldc.i4.8
    IL_0048:  conv.i
    IL_0049:  ldc.i4.2
    IL_004a:  mul
    IL_004b:  add
    IL_004c:  ldc.i4.s   73
    IL_004e:  stind.i2
    IL_004f:  dup
    IL_0050:  ldc.i4.s   9
    IL_0052:  conv.i
    IL_0053:  ldc.i4.2
    IL_0054:  mul
    IL_0055:  add
    IL_0056:  ldc.i4.s   74
    IL_0058:  stind.i2
    IL_0059:  dup
    IL_005a:  ldc.i4.s   10
    IL_005c:  conv.i
    IL_005d:  ldc.i4.2
    IL_005e:  mul
    IL_005f:  add
    IL_0060:  ldc.i4.s   75
    IL_0062:  stind.i2
    IL_0063:  dup
    IL_0064:  ldc.i4.s   11
    IL_0066:  conv.i
    IL_0067:  ldc.i4.2
    IL_0068:  mul
    IL_0069:  add
    IL_006a:  ldc.i4.s   76
    IL_006c:  stind.i2
    IL_006d:  dup
    IL_006e:  ldc.i4.s   12
    IL_0070:  conv.i
    IL_0071:  ldc.i4.2
    IL_0072:  mul
    IL_0073:  add
    IL_0074:  ldc.i4.s   77
    IL_0076:  stind.i2
    IL_0077:  dup
    IL_0078:  ldc.i4.s   13
    IL_007a:  conv.i
    IL_007b:  ldc.i4.2
    IL_007c:  mul
    IL_007d:  add
    IL_007e:  ldc.i4.s   78
    IL_0080:  stind.i2
    IL_0081:  dup
    IL_0082:  ldc.i4.s   14
    IL_0084:  conv.i
    IL_0085:  ldc.i4.2
    IL_0086:  mul
    IL_0087:  add
    IL_0088:  ldc.i4.s   79
    IL_008a:  stind.i2
    IL_008b:  dup
    IL_008c:  ldc.i4.s   15
    IL_008e:  conv.i
    IL_008f:  ldc.i4.2
    IL_0090:  mul
    IL_0091:  add
    IL_0092:  ldc.i4.s   80
    IL_0094:  stind.i2
    IL_0095:  dup
    IL_0096:  ldc.i4.s   16
    IL_0098:  conv.i
    IL_0099:  ldc.i4.2
    IL_009a:  mul
    IL_009b:  add
    IL_009c:  ldc.i4.s   81
    IL_009e:  stind.i2
    IL_009f:  dup
    IL_00a0:  ldc.i4.s   17
    IL_00a2:  conv.i
    IL_00a3:  ldc.i4.2
    IL_00a4:  mul
    IL_00a5:  add
    IL_00a6:  ldc.i4.s   82
    IL_00a8:  stind.i2
    IL_00a9:  dup
    IL_00aa:  ldc.i4.s   18
    IL_00ac:  conv.i
    IL_00ad:  ldc.i4.2
    IL_00ae:  mul
    IL_00af:  add
    IL_00b0:  ldc.i4.s   83
    IL_00b2:  stind.i2
    IL_00b3:  dup
    IL_00b4:  ldc.i4.s   19
    IL_00b6:  conv.i
    IL_00b7:  ldc.i4.2
    IL_00b8:  mul
    IL_00b9:  add
    IL_00ba:  ldc.i4.s   84
    IL_00bc:  stind.i2
    IL_00bd:  dup
    IL_00be:  ldc.i4.s   20
    IL_00c0:  conv.i
    IL_00c1:  ldc.i4.2
    IL_00c2:  mul
    IL_00c3:  add
    IL_00c4:  ldc.i4.s   85
    IL_00c6:  stind.i2
    IL_00c7:  dup
    IL_00c8:  ldc.i4.s   21
    IL_00ca:  conv.i
    IL_00cb:  ldc.i4.2
    IL_00cc:  mul
    IL_00cd:  add
    IL_00ce:  ldc.i4.s   86
    IL_00d0:  stind.i2
    IL_00d1:  dup
    IL_00d2:  ldc.i4.s   22
    IL_00d4:  conv.i
    IL_00d5:  ldc.i4.2
    IL_00d6:  mul
    IL_00d7:  add
    IL_00d8:  ldc.i4.s   87
    IL_00da:  stind.i2
    IL_00db:  dup
    IL_00dc:  ldc.i4.s   23
    IL_00de:  conv.i
    IL_00df:  ldc.i4.2
    IL_00e0:  mul
    IL_00e1:  add
    IL_00e2:  ldc.i4.s   88
    IL_00e4:  stind.i2
    IL_00e5:  dup
    IL_00e6:  ldc.i4.s   24
    IL_00e8:  conv.i
    IL_00e9:  ldc.i4.2
    IL_00ea:  mul
    IL_00eb:  add
    IL_00ec:  ldc.i4.s   89
    IL_00ee:  stind.i2
    IL_00ef:  dup
    IL_00f0:  ldc.i4.s   25
    IL_00f2:  conv.i
    IL_00f3:  ldc.i4.2
    IL_00f4:  mul
    IL_00f5:  add
    IL_00f6:  ldc.i4.s   90
    IL_00f8:  stind.i2
    IL_00f9:  stloc.0
    IL_00fa:  ldloc.0
    IL_00fb:  ldind.u2
    IL_00fc:  call       void [mscorlib]System.Console::WriteLine(char)
    IL_0101:  nop
    IL_0102:  ldarg.0
    IL_0103:  ldloc.0
    IL_0104:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0109:  stloc.1
    IL_010a:  br.s       IL_010c

    IL_010c:  ldloc.1
    IL_010d:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocCharAlphabet

  .method public hidebysig instance string 
          SimpleStackAllocSByte() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  4
    .locals init (int8* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldsflda    valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=3' '<PrivateImplementationDetails>'::'7037807198C22A7D2B0807371D763779A84FDFCF'
    IL_000b:  ldc.i4.3
    IL_000c:  cpblk
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldind.i1
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  ldloc.0
    IL_0019:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001e:  stloc.1
    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocSByte

  .method public hidebysig instance string 
          SimpleStackAllocInt16() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init (int16* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.6
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i2
    IL_0008:  dup
    IL_0009:  ldc.i4.2
    IL_000a:  add
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i2
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.2
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.3
    IL_0014:  stind.i2
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldind.i2
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldloc.0
    IL_0020:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt16

  .method public hidebysig instance string 
          SimpleStackAllocUInt16() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init (uint16* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.6
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i2
    IL_0008:  dup
    IL_0009:  ldc.i4.2
    IL_000a:  add
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i2
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.2
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.3
    IL_0014:  stind.i2
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldind.u2
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldloc.0
    IL_0020:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt16

  .method public hidebysig instance string 
          SimpleStackAllocInt32() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  4
    .locals init (int32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   12
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldc.i4.2
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.3
    IL_0015:  stind.i4
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldind.i4
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  ldloc.0
    IL_0021:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0026:  stloc.1
    IL_0027:  br.s       IL_0029

    IL_0029:  ldloc.1
    IL_002a:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt32(int32 a,
                                int32 b,
                                int32 c) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  4
    .locals init (int32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldarg.1
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.2
    IL_0015:  stind.i4
    IL_0016:  dup
    IL_0017:  ldc.i4.3
    IL_0018:  conv.i
    IL_0019:  ldc.i4.4
    IL_001a:  mul
    IL_001b:  add
    IL_001c:  ldarg.2
    IL_001d:  stind.i4
    IL_001e:  dup
    IL_001f:  ldc.i4.4
    IL_0020:  conv.i
    IL_0021:  ldc.i4.4
    IL_0022:  mul
    IL_0023:  add
    IL_0024:  ldc.i4.3
    IL_0025:  stind.i4
    IL_0026:  dup
    IL_0027:  ldc.i4.5
    IL_0028:  conv.i
    IL_0029:  ldc.i4.4
    IL_002a:  mul
    IL_002b:  add
    IL_002c:  ldarg.3
    IL_002d:  stind.i4
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldind.i4
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  ldloc.0
    IL_0039:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_003e:  stloc.1
    IL_003f:  br.s       IL_0041

    IL_0041:  ldloc.1
    IL_0042:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt32Fibonacci() cil managed
  {
    // Code size       192 (0xc0)
    .maxstack  4
    .locals init (int32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   68
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldc.i4.1
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.2
    IL_0015:  stind.i4
    IL_0016:  dup
    IL_0017:  ldc.i4.3
    IL_0018:  conv.i
    IL_0019:  ldc.i4.4
    IL_001a:  mul
    IL_001b:  add
    IL_001c:  ldc.i4.3
    IL_001d:  stind.i4
    IL_001e:  dup
    IL_001f:  ldc.i4.4
    IL_0020:  conv.i
    IL_0021:  ldc.i4.4
    IL_0022:  mul
    IL_0023:  add
    IL_0024:  ldc.i4.5
    IL_0025:  stind.i4
    IL_0026:  dup
    IL_0027:  ldc.i4.5
    IL_0028:  conv.i
    IL_0029:  ldc.i4.4
    IL_002a:  mul
    IL_002b:  add
    IL_002c:  ldc.i4.8
    IL_002d:  stind.i4
    IL_002e:  dup
    IL_002f:  ldc.i4.6
    IL_0030:  conv.i
    IL_0031:  ldc.i4.4
    IL_0032:  mul
    IL_0033:  add
    IL_0034:  ldc.i4.s   13
    IL_0036:  stind.i4
    IL_0037:  dup
    IL_0038:  ldc.i4.7
    IL_0039:  conv.i
    IL_003a:  ldc.i4.4
    IL_003b:  mul
    IL_003c:  add
    IL_003d:  ldc.i4.s   21
    IL_003f:  stind.i4
    IL_0040:  dup
    IL_0041:  ldc.i4.8
    IL_0042:  conv.i
    IL_0043:  ldc.i4.4
    IL_0044:  mul
    IL_0045:  add
    IL_0046:  ldc.i4.s   34
    IL_0048:  stind.i4
    IL_0049:  dup
    IL_004a:  ldc.i4.s   9
    IL_004c:  conv.i
    IL_004d:  ldc.i4.4
    IL_004e:  mul
    IL_004f:  add
    IL_0050:  ldc.i4.s   55
    IL_0052:  stind.i4
    IL_0053:  dup
    IL_0054:  ldc.i4.s   10
    IL_0056:  conv.i
    IL_0057:  ldc.i4.4
    IL_0058:  mul
    IL_0059:  add
    IL_005a:  ldc.i4.s   89
    IL_005c:  stind.i4
    IL_005d:  dup
    IL_005e:  ldc.i4.s   11
    IL_0060:  conv.i
    IL_0061:  ldc.i4.4
    IL_0062:  mul
    IL_0063:  add
    IL_0064:  ldc.i4     0x90
    IL_0069:  stind.i4
    IL_006a:  dup
    IL_006b:  ldc.i4.s   12
    IL_006d:  conv.i
    IL_006e:  ldc.i4.4
    IL_006f:  mul
    IL_0070:  add
    IL_0071:  ldc.i4     0xe9
    IL_0076:  stind.i4
    IL_0077:  dup
    IL_0078:  ldc.i4.s   13
    IL_007a:  conv.i
    IL_007b:  ldc.i4.4
    IL_007c:  mul
    IL_007d:  add
    IL_007e:  ldc.i4     0x179
    IL_0083:  stind.i4
    IL_0084:  dup
    IL_0085:  ldc.i4.s   14
    IL_0087:  conv.i
    IL_0088:  ldc.i4.4
    IL_0089:  mul
    IL_008a:  add
    IL_008b:  ldc.i4     0x262
    IL_0090:  stind.i4
    IL_0091:  dup
    IL_0092:  ldc.i4.s   15
    IL_0094:  conv.i
    IL_0095:  ldc.i4.4
    IL_0096:  mul
    IL_0097:  add
    IL_0098:  ldc.i4     0x3db
    IL_009d:  stind.i4
    IL_009e:  dup
    IL_009f:  ldc.i4.s   16
    IL_00a1:  conv.i
    IL_00a2:  ldc.i4.4
    IL_00a3:  mul
    IL_00a4:  add
    IL_00a5:  ldc.i4     0x63d
    IL_00aa:  stind.i4
    IL_00ab:  stloc.0
    IL_00ac:  ldloc.0
    IL_00ad:  ldind.i4
    IL_00ae:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00b3:  nop
    IL_00b4:  ldarg.0
    IL_00b5:  ldloc.0
    IL_00b6:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_00bb:  stloc.1
    IL_00bc:  br.s       IL_00be

    IL_00be:  ldloc.1
    IL_00bf:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32Fibonacci

  .method public hidebysig instance string 
          SimpleStackAllocUInt32() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  4
    .locals init (uint32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   12
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldc.i4.2
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.3
    IL_0015:  stind.i4
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldind.u4
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(uint32)
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  ldloc.0
    IL_0021:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0026:  stloc.1
    IL_0027:  br.s       IL_0029

    IL_0029:  ldloc.1
    IL_002a:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt64() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  4
    .locals init (int64* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  conv.i8
    IL_0009:  stind.i8
    IL_000a:  dup
    IL_000b:  ldc.i4.8
    IL_000c:  add
    IL_000d:  ldc.i4.2
    IL_000e:  conv.i8
    IL_000f:  stind.i8
    IL_0010:  dup
    IL_0011:  ldc.i4.2
    IL_0012:  conv.i
    IL_0013:  ldc.i4.8
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i8
    IL_0018:  stind.i8
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldind.i8
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int64)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  ldloc.0
    IL_0024:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0029:  stloc.1
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.1
    IL_002d:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt64

  .method public hidebysig instance string 
          SimpleStackAllocUInt64() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  4
    .locals init (uint64* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  conv.i8
    IL_0009:  stind.i8
    IL_000a:  dup
    IL_000b:  ldc.i4.8
    IL_000c:  add
    IL_000d:  ldc.i4.2
    IL_000e:  conv.i8
    IL_000f:  stind.i8
    IL_0010:  dup
    IL_0011:  ldc.i4.2
    IL_0012:  conv.i
    IL_0013:  ldc.i4.8
    IL_0014:  mul
    IL_0015:  add
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i8
    IL_0018:  stind.i8
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldind.i8
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(uint64)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  ldloc.0
    IL_0024:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0029:  stloc.1
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.1
    IL_002d:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt64

  .method public hidebysig instance string 
          SimpleStackAllocInt32NonConstant(int32 a,
                                           int32 b,
                                           int32 c) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  4
    .locals init (int32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldc.i4.1
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.0
    IL_0015:  stind.i4
    IL_0016:  dup
    IL_0017:  ldc.i4.3
    IL_0018:  conv.i
    IL_0019:  ldc.i4.4
    IL_001a:  mul
    IL_001b:  add
    IL_001c:  ldarg.1
    IL_001d:  stind.i4
    IL_001e:  dup
    IL_001f:  ldc.i4.4
    IL_0020:  conv.i
    IL_0021:  ldc.i4.4
    IL_0022:  mul
    IL_0023:  add
    IL_0024:  ldarg.2
    IL_0025:  stind.i4
    IL_0026:  dup
    IL_0027:  ldc.i4.5
    IL_0028:  conv.i
    IL_0029:  ldc.i4.4
    IL_002a:  mul
    IL_002b:  add
    IL_002c:  ldarg.3
    IL_002d:  stind.i4
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldind.i4
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  ldloc.0
    IL_0039:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_003e:  stloc.1
    IL_003f:  br.s       IL_0041

    IL_0041:  ldloc.1
    IL_0042:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32NonConstant

  .method public hidebysig instance string 
          NotAnInitializer(int32 a,
                           int32 b,
                           int32 c) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (int32* V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.4
    IL_0009:  add
    IL_000a:  ldarg.1
    IL_000b:  stind.i4
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.3
    IL_000e:  conv.i
    IL_000f:  ldc.i4.4
    IL_0010:  mul
    IL_0011:  add
    IL_0012:  ldarg.2
    IL_0013:  stind.i4
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  conv.i
    IL_0017:  ldc.i4.4
    IL_0018:  mul
    IL_0019:  add
    IL_001a:  ldarg.3
    IL_001b:  stind.i4
    IL_001c:  ldloc.0
    IL_001d:  ldind.i4
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  ldloc.0
    IL_0026:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_002b:  stloc.1
    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.1
    IL_002f:  ret
  } // end of method CS73_StackAllocInitializers::NotAnInitializer

  .method public hidebysig instance string 
          NegativeOffsets(int32 a,
                          int32 b,
                          int32 c) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (uint8* V_0,
             int32* V_1,
             string V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   12
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stind.i4
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.4
    IL_000c:  sub
    IL_000d:  ldc.i4.2
    IL_000e:  stind.i4
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.8
    IL_0011:  sub
    IL_0012:  ldc.i4.3
    IL_0013:  stind.i4
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldind.i4
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldloc.1
    IL_0020:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0025:  stloc.2
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.2
    IL_0029:  ret
  } // end of method CS73_StackAllocInitializers::NegativeOffsets

  .method public hidebysig instance string 
          UsePointer(uint8* ptr) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       instance string [mscorlib]System.Byte::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method CS73_StackAllocInitializers::UsePointer

  .method public hidebysig instance string 
          GetSpan() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  2
    .locals init (valuetype [System.Memory]System.Span`1<int32> V_0,
             int32 V_1,
             string V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::GetSize()
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  conv.u
    IL_000a:  ldc.i4.4
    IL_000b:  mul.ovf.un
    IL_000c:  localloc
    IL_000e:  ldloc.1
    IL_000f:  newobj     instance void valuetype [System.Memory]System.Span`1<int32>::.ctor(void*,
                                                                                            int32)
    IL_0014:  stloc.0
    IL_0015:  ldarg.0
    IL_0016:  ldloc.0
    IL_0017:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UseSpan(valuetype [System.Memory]System.Span`1<int32>)
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.2
    IL_0020:  ret
  } // end of method CS73_StackAllocInitializers::GetSpan

  .method public hidebysig instance string 
          GetSpan2() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  4
    .locals init (valuetype [System.Memory]System.Span`1<int32> V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   16
    IL_0003:  conv.u
    IL_0004:  localloc
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stind.i4
    IL_0009:  dup
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ldc.i4.2
    IL_000d:  stind.i4
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.4
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.3
    IL_0015:  stind.i4
    IL_0016:  dup
    IL_0017:  ldc.i4.3
    IL_0018:  conv.i
    IL_0019:  ldc.i4.4
    IL_001a:  mul
    IL_001b:  add
    IL_001c:  ldc.i4.4
    IL_001d:  stind.i4
    IL_001e:  ldc.i4.4
    IL_001f:  newobj     instance void valuetype [System.Memory]System.Span`1<int32>::.ctor(void*,
                                                                                            int32)
    IL_0024:  stloc.0
    IL_0025:  ldarg.0
    IL_0026:  ldloc.0
    IL_0027:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UseSpan(valuetype [System.Memory]System.Span`1<int32>)
    IL_002c:  stloc.1
    IL_002d:  br.s       IL_002f

    IL_002f:  ldloc.1
    IL_0030:  ret
  } // end of method CS73_StackAllocInitializers::GetSpan2

  .method public hidebysig instance string 
          UseSpan(valuetype [System.Memory]System.Span`1<int32> span) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method CS73_StackAllocInitializers::UseSpan

  .method public hidebysig instance int32 
          GetSize() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method CS73_StackAllocInitializers::GetSize

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method CS73_StackAllocInitializers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=3'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 3
  } // end of class '__StaticArrayInitTypeSize=3'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=55'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 55
  } // end of class '__StaticArrayInitTypeSize=55'

  .field static assembly initonly int32 '063AAB58782881806084E1A944FBCEE5F5815405' at I_000032F0
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=3' '7037807198C22A7D2B0807371D763779A84FDFCF' at I_000032F8
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=55' F623596D706F878F1D12C19353913A8E96904144 at I_00003300
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_000032F0 = bytearray (
                 00 01 00 01) 
.data cil I_000032F4 = int8[4]
.data cil I_000032F8 = bytearray (
                 01 02 03) 
.data cil I_000032FB = int8[5]
.data cil I_00003300 = bytearray (
                 01 02 03 05 07 0B 0D 11 13 17 1D 1F 25 29 2B 2F  // ............%)+/
                 35 3B 3D 43 47 49 4F 53 59 61 65 67 6B 6D 71 7F  // 5;=CGIOSYaegkmq.
                 83 89 8B 95 97 9D A3 A7 AD B3 B5 BF C1 C5 C7 D3
                 DF E3 E5 E9 EF F1 FB) 
// *********** DISASSEMBLY COMPLETE ***********************
