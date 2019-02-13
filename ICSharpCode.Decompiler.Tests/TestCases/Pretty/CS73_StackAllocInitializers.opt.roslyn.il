



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

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
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::a
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::b
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::c
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    d
      IL_0018:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::d
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    e
      IL_0020:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::e
      IL_0025:  ret
    } // end of method StructWithSize5::.ctor

  } // end of class StructWithSize5

  .method public hidebysig instance string 
          SimpleStackAllocStruct1() cil managed
  {
    // Code size       137 (0x89)
    .maxstack  7
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5* V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  conv.u
    IL_0002:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0008:  mul.ovf.un
    IL_0009:  localloc
    IL_000b:  dup
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.2
    IL_000e:  ldc.i4.3
    IL_000f:  ldc.i4.4
    IL_0010:  ldc.i4.5
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_0016:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_001b:  dup
    IL_001c:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0022:  add
    IL_0023:  ldc.i4.s   11
    IL_0025:  ldc.i4.s   22
    IL_0027:  ldc.i4.s   33
    IL_0029:  ldc.i4.s   44
    IL_002b:  ldc.i4.s   55
    IL_002d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_0032:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0037:  dup
    IL_0038:  ldc.i4.2
    IL_0039:  conv.i
    IL_003a:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0040:  mul
    IL_0041:  add
    IL_0042:  ldc.i4.1
    IL_0043:  ldc.i4.4
    IL_0044:  ldc.i4.8
    IL_0045:  ldc.i4.6
    IL_0046:  ldc.i4.2
    IL_0047:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_004c:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0051:  dup
    IL_0052:  ldc.i4.3
    IL_0053:  conv.i
    IL_0054:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_005a:  mul
    IL_005b:  add
    IL_005c:  ldc.i4.s   12
    IL_005e:  ldc.i4.s   23
    IL_0060:  ldc.i4.s   34
    IL_0062:  ldc.i4.s   45
    IL_0064:  ldc.i4.s   56
    IL_0066:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5::.ctor(uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8,
                                                                                                                                        uint8)
    IL_006b:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0070:  stloc.0
    IL_0071:  ldloc.0
    IL_0072:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_0077:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers/StructWithSize5
    IL_007c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0081:  ldarg.0
    IL_0082:  ldloc.0
    IL_0083:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0088:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocStruct1

  .method public hidebysig instance string 
          SimpleStackAllocBool() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (bool* V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldsflda    int32 '<PrivateImplementationDetails>'::'063AAB58782881806084E1A944FBCEE5F5815405'
    IL_000a:  ldc.i4.4
    IL_000b:  cpblk
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldind.u1
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0015:  ldarg.0
    IL_0016:  ldloc.0
    IL_0017:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001c:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocBool

  .method public hidebysig instance string 
          DoNotInlineTest() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  4
    .locals init (bool* V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldsflda    int32 '<PrivateImplementationDetails>'::'063AAB58782881806084E1A944FBCEE5F5815405'
    IL_000a:  ldc.i4.4
    IL_000b:  cpblk
    IL_000d:  stloc.0
    IL_000e:  ldarg.0
    IL_000f:  ldloc.0
    IL_0010:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0015:  ret
  } // end of method CS73_StackAllocInitializers::DoNotInlineTest

  .method public hidebysig instance string 
          SimpleStackAllocByte() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  3
    .locals init (uint8* V_0)
    IL_0000:  ldc.i4.2
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldc.i4.0
    IL_0006:  stind.i1
    IL_0007:  dup
    IL_0008:  ldc.i4.1
    IL_0009:  add
    IL_000a:  ldc.i4.1
    IL_000b:  stind.i1
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldind.u1
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0014:  ldarg.0
    IL_0015:  ldloc.0
    IL_0016:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001b:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocByte

  .method public hidebysig instance string 
          SimpleStackAllocPrimesAsBytes() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  4
    .locals init (uint8* V_0)
    IL_0000:  ldc.i4.s   55
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldsflda    valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=55' '<PrivateImplementationDetails>'::F623596D706F878F1D12C19353913A8E96904144
    IL_000b:  ldc.i4.s   55
    IL_000d:  cpblk
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldind.u1
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0017:  ldarg.0
    IL_0018:  ldloc.0
    IL_0019:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001e:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocPrimesAsBytes

  .method public hidebysig instance string 
          SimpleStackAllocChar() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  4
    .locals init (char* V_0)
    IL_0000:  ldc.i4.8
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldc.i4.s   49
    IL_0007:  stind.i2
    IL_0008:  dup
    IL_0009:  ldc.i4.2
    IL_000a:  add
    IL_000b:  ldc.i4.s   50
    IL_000d:  stind.i2
    IL_000e:  dup
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i
    IL_0011:  ldc.i4.2
    IL_0012:  mul
    IL_0013:  add
    IL_0014:  ldc.i4.s   51
    IL_0016:  stind.i2
    IL_0017:  dup
    IL_0018:  ldc.i4.3
    IL_0019:  conv.i
    IL_001a:  ldc.i4.2
    IL_001b:  mul
    IL_001c:  add
    IL_001d:  ldc.i4.s   52
    IL_001f:  stind.i2
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldind.u2
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(char)
    IL_0028:  ldarg.0
    IL_0029:  ldloc.0
    IL_002a:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_002f:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocChar

  .method public hidebysig instance string 
          SimpleStackAllocCharAlphabet() cil managed
  {
    // Code size       264 (0x108)
    .maxstack  4
    .locals init (char* V_0)
    IL_0000:  ldc.i4.s   52
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.s   65
    IL_0008:  stind.i2
    IL_0009:  dup
    IL_000a:  ldc.i4.2
    IL_000b:  add
    IL_000c:  ldc.i4.s   66
    IL_000e:  stind.i2
    IL_000f:  dup
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i
    IL_0012:  ldc.i4.2
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldc.i4.s   67
    IL_0017:  stind.i2
    IL_0018:  dup
    IL_0019:  ldc.i4.3
    IL_001a:  conv.i
    IL_001b:  ldc.i4.2
    IL_001c:  mul
    IL_001d:  add
    IL_001e:  ldc.i4.s   68
    IL_0020:  stind.i2
    IL_0021:  dup
    IL_0022:  ldc.i4.4
    IL_0023:  conv.i
    IL_0024:  ldc.i4.2
    IL_0025:  mul
    IL_0026:  add
    IL_0027:  ldc.i4.s   69
    IL_0029:  stind.i2
    IL_002a:  dup
    IL_002b:  ldc.i4.5
    IL_002c:  conv.i
    IL_002d:  ldc.i4.2
    IL_002e:  mul
    IL_002f:  add
    IL_0030:  ldc.i4.s   70
    IL_0032:  stind.i2
    IL_0033:  dup
    IL_0034:  ldc.i4.6
    IL_0035:  conv.i
    IL_0036:  ldc.i4.2
    IL_0037:  mul
    IL_0038:  add
    IL_0039:  ldc.i4.s   71
    IL_003b:  stind.i2
    IL_003c:  dup
    IL_003d:  ldc.i4.7
    IL_003e:  conv.i
    IL_003f:  ldc.i4.2
    IL_0040:  mul
    IL_0041:  add
    IL_0042:  ldc.i4.s   72
    IL_0044:  stind.i2
    IL_0045:  dup
    IL_0046:  ldc.i4.8
    IL_0047:  conv.i
    IL_0048:  ldc.i4.2
    IL_0049:  mul
    IL_004a:  add
    IL_004b:  ldc.i4.s   73
    IL_004d:  stind.i2
    IL_004e:  dup
    IL_004f:  ldc.i4.s   9
    IL_0051:  conv.i
    IL_0052:  ldc.i4.2
    IL_0053:  mul
    IL_0054:  add
    IL_0055:  ldc.i4.s   74
    IL_0057:  stind.i2
    IL_0058:  dup
    IL_0059:  ldc.i4.s   10
    IL_005b:  conv.i
    IL_005c:  ldc.i4.2
    IL_005d:  mul
    IL_005e:  add
    IL_005f:  ldc.i4.s   75
    IL_0061:  stind.i2
    IL_0062:  dup
    IL_0063:  ldc.i4.s   11
    IL_0065:  conv.i
    IL_0066:  ldc.i4.2
    IL_0067:  mul
    IL_0068:  add
    IL_0069:  ldc.i4.s   76
    IL_006b:  stind.i2
    IL_006c:  dup
    IL_006d:  ldc.i4.s   12
    IL_006f:  conv.i
    IL_0070:  ldc.i4.2
    IL_0071:  mul
    IL_0072:  add
    IL_0073:  ldc.i4.s   77
    IL_0075:  stind.i2
    IL_0076:  dup
    IL_0077:  ldc.i4.s   13
    IL_0079:  conv.i
    IL_007a:  ldc.i4.2
    IL_007b:  mul
    IL_007c:  add
    IL_007d:  ldc.i4.s   78
    IL_007f:  stind.i2
    IL_0080:  dup
    IL_0081:  ldc.i4.s   14
    IL_0083:  conv.i
    IL_0084:  ldc.i4.2
    IL_0085:  mul
    IL_0086:  add
    IL_0087:  ldc.i4.s   79
    IL_0089:  stind.i2
    IL_008a:  dup
    IL_008b:  ldc.i4.s   15
    IL_008d:  conv.i
    IL_008e:  ldc.i4.2
    IL_008f:  mul
    IL_0090:  add
    IL_0091:  ldc.i4.s   80
    IL_0093:  stind.i2
    IL_0094:  dup
    IL_0095:  ldc.i4.s   16
    IL_0097:  conv.i
    IL_0098:  ldc.i4.2
    IL_0099:  mul
    IL_009a:  add
    IL_009b:  ldc.i4.s   81
    IL_009d:  stind.i2
    IL_009e:  dup
    IL_009f:  ldc.i4.s   17
    IL_00a1:  conv.i
    IL_00a2:  ldc.i4.2
    IL_00a3:  mul
    IL_00a4:  add
    IL_00a5:  ldc.i4.s   82
    IL_00a7:  stind.i2
    IL_00a8:  dup
    IL_00a9:  ldc.i4.s   18
    IL_00ab:  conv.i
    IL_00ac:  ldc.i4.2
    IL_00ad:  mul
    IL_00ae:  add
    IL_00af:  ldc.i4.s   83
    IL_00b1:  stind.i2
    IL_00b2:  dup
    IL_00b3:  ldc.i4.s   19
    IL_00b5:  conv.i
    IL_00b6:  ldc.i4.2
    IL_00b7:  mul
    IL_00b8:  add
    IL_00b9:  ldc.i4.s   84
    IL_00bb:  stind.i2
    IL_00bc:  dup
    IL_00bd:  ldc.i4.s   20
    IL_00bf:  conv.i
    IL_00c0:  ldc.i4.2
    IL_00c1:  mul
    IL_00c2:  add
    IL_00c3:  ldc.i4.s   85
    IL_00c5:  stind.i2
    IL_00c6:  dup
    IL_00c7:  ldc.i4.s   21
    IL_00c9:  conv.i
    IL_00ca:  ldc.i4.2
    IL_00cb:  mul
    IL_00cc:  add
    IL_00cd:  ldc.i4.s   86
    IL_00cf:  stind.i2
    IL_00d0:  dup
    IL_00d1:  ldc.i4.s   22
    IL_00d3:  conv.i
    IL_00d4:  ldc.i4.2
    IL_00d5:  mul
    IL_00d6:  add
    IL_00d7:  ldc.i4.s   87
    IL_00d9:  stind.i2
    IL_00da:  dup
    IL_00db:  ldc.i4.s   23
    IL_00dd:  conv.i
    IL_00de:  ldc.i4.2
    IL_00df:  mul
    IL_00e0:  add
    IL_00e1:  ldc.i4.s   88
    IL_00e3:  stind.i2
    IL_00e4:  dup
    IL_00e5:  ldc.i4.s   24
    IL_00e7:  conv.i
    IL_00e8:  ldc.i4.2
    IL_00e9:  mul
    IL_00ea:  add
    IL_00eb:  ldc.i4.s   89
    IL_00ed:  stind.i2
    IL_00ee:  dup
    IL_00ef:  ldc.i4.s   25
    IL_00f1:  conv.i
    IL_00f2:  ldc.i4.2
    IL_00f3:  mul
    IL_00f4:  add
    IL_00f5:  ldc.i4.s   90
    IL_00f7:  stind.i2
    IL_00f8:  stloc.0
    IL_00f9:  ldloc.0
    IL_00fa:  ldind.u2
    IL_00fb:  call       void [mscorlib]System.Console::WriteLine(char)
    IL_0100:  ldarg.0
    IL_0101:  ldloc.0
    IL_0102:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0107:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocCharAlphabet

  .method public hidebysig instance string 
          SimpleStackAllocSByte() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int8* V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldsflda    valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=3' '<PrivateImplementationDetails>'::'7037807198C22A7D2B0807371D763779A84FDFCF'
    IL_000a:  ldc.i4.3
    IL_000b:  cpblk
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldind.i1
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0015:  ldarg.0
    IL_0016:  ldloc.0
    IL_0017:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_001c:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocSByte

  .method public hidebysig instance string 
          SimpleStackAllocInt16() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  4
    .locals init (int16* V_0)
    IL_0000:  ldc.i4.6
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldc.i4.1
    IL_0006:  stind.i2
    IL_0007:  dup
    IL_0008:  ldc.i4.2
    IL_0009:  add
    IL_000a:  ldc.i4.2
    IL_000b:  stind.i2
    IL_000c:  dup
    IL_000d:  ldc.i4.2
    IL_000e:  conv.i
    IL_000f:  ldc.i4.2
    IL_0010:  mul
    IL_0011:  add
    IL_0012:  ldc.i4.3
    IL_0013:  stind.i2
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldind.i2
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  ldarg.0
    IL_001d:  ldloc.0
    IL_001e:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0023:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt16

  .method public hidebysig instance string 
          SimpleStackAllocUInt16() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  4
    .locals init (uint16* V_0)
    IL_0000:  ldc.i4.6
    IL_0001:  conv.u
    IL_0002:  localloc
    IL_0004:  dup
    IL_0005:  ldc.i4.1
    IL_0006:  stind.i2
    IL_0007:  dup
    IL_0008:  ldc.i4.2
    IL_0009:  add
    IL_000a:  ldc.i4.2
    IL_000b:  stind.i2
    IL_000c:  dup
    IL_000d:  ldc.i4.2
    IL_000e:  conv.i
    IL_000f:  ldc.i4.2
    IL_0010:  mul
    IL_0011:  add
    IL_0012:  ldc.i4.3
    IL_0013:  stind.i2
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldind.u2
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  ldarg.0
    IL_001d:  ldloc.0
    IL_001e:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0023:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt16

  .method public hidebysig instance string 
          SimpleStackAllocInt32() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   12
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.3
    IL_0014:  stind.i4
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldind.i4
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  ldarg.0
    IL_001e:  ldloc.0
    IL_001f:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0024:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt32(int32 a,
                                int32 b,
                                int32 c) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  4
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldarg.1
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.2
    IL_0014:  stind.i4
    IL_0015:  dup
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i
    IL_0018:  ldc.i4.4
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldarg.2
    IL_001c:  stind.i4
    IL_001d:  dup
    IL_001e:  ldc.i4.4
    IL_001f:  conv.i
    IL_0020:  ldc.i4.4
    IL_0021:  mul
    IL_0022:  add
    IL_0023:  ldc.i4.3
    IL_0024:  stind.i4
    IL_0025:  dup
    IL_0026:  ldc.i4.5
    IL_0027:  conv.i
    IL_0028:  ldc.i4.4
    IL_0029:  mul
    IL_002a:  add
    IL_002b:  ldarg.3
    IL_002c:  stind.i4
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  ldind.i4
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0035:  ldarg.0
    IL_0036:  ldloc.0
    IL_0037:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_003c:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt32Fibonacci() cil managed
  {
    // Code size       186 (0xba)
    .maxstack  4
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   68
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldc.i4.1
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.2
    IL_0014:  stind.i4
    IL_0015:  dup
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i
    IL_0018:  ldc.i4.4
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldc.i4.3
    IL_001c:  stind.i4
    IL_001d:  dup
    IL_001e:  ldc.i4.4
    IL_001f:  conv.i
    IL_0020:  ldc.i4.4
    IL_0021:  mul
    IL_0022:  add
    IL_0023:  ldc.i4.5
    IL_0024:  stind.i4
    IL_0025:  dup
    IL_0026:  ldc.i4.5
    IL_0027:  conv.i
    IL_0028:  ldc.i4.4
    IL_0029:  mul
    IL_002a:  add
    IL_002b:  ldc.i4.8
    IL_002c:  stind.i4
    IL_002d:  dup
    IL_002e:  ldc.i4.6
    IL_002f:  conv.i
    IL_0030:  ldc.i4.4
    IL_0031:  mul
    IL_0032:  add
    IL_0033:  ldc.i4.s   13
    IL_0035:  stind.i4
    IL_0036:  dup
    IL_0037:  ldc.i4.7
    IL_0038:  conv.i
    IL_0039:  ldc.i4.4
    IL_003a:  mul
    IL_003b:  add
    IL_003c:  ldc.i4.s   21
    IL_003e:  stind.i4
    IL_003f:  dup
    IL_0040:  ldc.i4.8
    IL_0041:  conv.i
    IL_0042:  ldc.i4.4
    IL_0043:  mul
    IL_0044:  add
    IL_0045:  ldc.i4.s   34
    IL_0047:  stind.i4
    IL_0048:  dup
    IL_0049:  ldc.i4.s   9
    IL_004b:  conv.i
    IL_004c:  ldc.i4.4
    IL_004d:  mul
    IL_004e:  add
    IL_004f:  ldc.i4.s   55
    IL_0051:  stind.i4
    IL_0052:  dup
    IL_0053:  ldc.i4.s   10
    IL_0055:  conv.i
    IL_0056:  ldc.i4.4
    IL_0057:  mul
    IL_0058:  add
    IL_0059:  ldc.i4.s   89
    IL_005b:  stind.i4
    IL_005c:  dup
    IL_005d:  ldc.i4.s   11
    IL_005f:  conv.i
    IL_0060:  ldc.i4.4
    IL_0061:  mul
    IL_0062:  add
    IL_0063:  ldc.i4     0x90
    IL_0068:  stind.i4
    IL_0069:  dup
    IL_006a:  ldc.i4.s   12
    IL_006c:  conv.i
    IL_006d:  ldc.i4.4
    IL_006e:  mul
    IL_006f:  add
    IL_0070:  ldc.i4     0xe9
    IL_0075:  stind.i4
    IL_0076:  dup
    IL_0077:  ldc.i4.s   13
    IL_0079:  conv.i
    IL_007a:  ldc.i4.4
    IL_007b:  mul
    IL_007c:  add
    IL_007d:  ldc.i4     0x179
    IL_0082:  stind.i4
    IL_0083:  dup
    IL_0084:  ldc.i4.s   14
    IL_0086:  conv.i
    IL_0087:  ldc.i4.4
    IL_0088:  mul
    IL_0089:  add
    IL_008a:  ldc.i4     0x262
    IL_008f:  stind.i4
    IL_0090:  dup
    IL_0091:  ldc.i4.s   15
    IL_0093:  conv.i
    IL_0094:  ldc.i4.4
    IL_0095:  mul
    IL_0096:  add
    IL_0097:  ldc.i4     0x3db
    IL_009c:  stind.i4
    IL_009d:  dup
    IL_009e:  ldc.i4.s   16
    IL_00a0:  conv.i
    IL_00a1:  ldc.i4.4
    IL_00a2:  mul
    IL_00a3:  add
    IL_00a4:  ldc.i4     0x63d
    IL_00a9:  stind.i4
    IL_00aa:  stloc.0
    IL_00ab:  ldloc.0
    IL_00ac:  ldind.i4
    IL_00ad:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00b2:  ldarg.0
    IL_00b3:  ldloc.0
    IL_00b4:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_00b9:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32Fibonacci

  .method public hidebysig instance string 
          SimpleStackAllocUInt32() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (uint32* V_0)
    IL_0000:  ldc.i4.s   12
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.3
    IL_0014:  stind.i4
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldind.u4
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(uint32)
    IL_001d:  ldarg.0
    IL_001e:  ldloc.0
    IL_001f:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0024:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt32

  .method public hidebysig instance string 
          SimpleStackAllocInt64() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  4
    .locals init (int64* V_0)
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  conv.i8
    IL_0008:  stind.i8
    IL_0009:  dup
    IL_000a:  ldc.i4.8
    IL_000b:  add
    IL_000c:  ldc.i4.2
    IL_000d:  conv.i8
    IL_000e:  stind.i8
    IL_000f:  dup
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i
    IL_0012:  ldc.i4.8
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldc.i4.3
    IL_0016:  conv.i8
    IL_0017:  stind.i8
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldind.i8
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(int64)
    IL_0020:  ldarg.0
    IL_0021:  ldloc.0
    IL_0022:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0027:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt64

  .method public hidebysig instance string 
          SimpleStackAllocUInt64() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  4
    .locals init (uint64* V_0)
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  conv.i8
    IL_0008:  stind.i8
    IL_0009:  dup
    IL_000a:  ldc.i4.8
    IL_000b:  add
    IL_000c:  ldc.i4.2
    IL_000d:  conv.i8
    IL_000e:  stind.i8
    IL_000f:  dup
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i
    IL_0012:  ldc.i4.8
    IL_0013:  mul
    IL_0014:  add
    IL_0015:  ldc.i4.3
    IL_0016:  conv.i8
    IL_0017:  stind.i8
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldind.i8
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(uint64)
    IL_0020:  ldarg.0
    IL_0021:  ldloc.0
    IL_0022:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0027:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocUInt64

  .method public hidebysig instance string 
          SimpleStackAllocInt32NonConstant(int32 a,
                                           int32 b,
                                           int32 c) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  4
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.0
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldc.i4.1
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.0
    IL_0014:  stind.i4
    IL_0015:  dup
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i
    IL_0018:  ldc.i4.4
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldarg.1
    IL_001c:  stind.i4
    IL_001d:  dup
    IL_001e:  ldc.i4.4
    IL_001f:  conv.i
    IL_0020:  ldc.i4.4
    IL_0021:  mul
    IL_0022:  add
    IL_0023:  ldarg.2
    IL_0024:  stind.i4
    IL_0025:  dup
    IL_0026:  ldc.i4.5
    IL_0027:  conv.i
    IL_0028:  ldc.i4.4
    IL_0029:  mul
    IL_002a:  add
    IL_002b:  ldarg.3
    IL_002c:  stind.i4
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  ldind.i4
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0035:  ldarg.0
    IL_0036:  ldloc.0
    IL_0037:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_003c:  ret
  } // end of method CS73_StackAllocInitializers::SimpleStackAllocInt32NonConstant

  .method public hidebysig instance string 
          NotAnInitializer(int32 a,
                           int32 b,
                           int32 c) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  3
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.4
    IL_0008:  add
    IL_0009:  ldarg.1
    IL_000a:  stind.i4
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.3
    IL_000d:  conv.i
    IL_000e:  ldc.i4.4
    IL_000f:  mul
    IL_0010:  add
    IL_0011:  ldarg.2
    IL_0012:  stind.i4
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.5
    IL_0015:  conv.i
    IL_0016:  ldc.i4.4
    IL_0017:  mul
    IL_0018:  add
    IL_0019:  ldarg.3
    IL_001a:  stind.i4
    IL_001b:  ldloc.0
    IL_001c:  ldind.i4
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  ldarg.0
    IL_0023:  ldloc.0
    IL_0024:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0029:  ret
  } // end of method CS73_StackAllocInitializers::NotAnInitializer

  .method public hidebysig instance string 
          NegativeOffsets(int32 a,
                          int32 b,
                          int32 c) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  3
    .locals init (int32* V_0)
    IL_0000:  ldc.i4.s   12
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  sub
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.8
    IL_000f:  sub
    IL_0010:  ldc.i4.3
    IL_0011:  stind.i4
    IL_0012:  stloc.0
    IL_0013:  ldloc.0
    IL_0014:  ldind.i4
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  ldarg.0
    IL_001b:  ldloc.0
    IL_001c:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UsePointer(uint8*)
    IL_0021:  ret
  } // end of method CS73_StackAllocInitializers::NegativeOffsets

  .method public hidebysig instance string 
          UsePointer(uint8* ptr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       instance string [mscorlib]System.Byte::ToString()
    IL_0006:  ret
  } // end of method CS73_StackAllocInitializers::UsePointer

  .method public hidebysig instance string 
          GetSpan() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (valuetype [System.Memory]System.Span`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::GetSize()
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  conv.u
    IL_0009:  ldc.i4.4
    IL_000a:  mul.ovf.un
    IL_000b:  localloc
    IL_000d:  ldloc.1
    IL_000e:  newobj     instance void valuetype [System.Memory]System.Span`1<int32>::.ctor(void*,
                                                                                            int32)
    IL_0013:  stloc.0
    IL_0014:  ldarg.0
    IL_0015:  ldloc.0
    IL_0016:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UseSpan(valuetype [System.Memory]System.Span`1<int32>)
    IL_001b:  ret
  } // end of method CS73_StackAllocInitializers::GetSpan

  .method public hidebysig instance string 
          GetSpan2() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  4
    .locals init (valuetype [System.Memory]System.Span`1<int32> V_0)
    IL_0000:  ldc.i4.s   16
    IL_0002:  conv.u
    IL_0003:  localloc
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stind.i4
    IL_0008:  dup
    IL_0009:  ldc.i4.4
    IL_000a:  add
    IL_000b:  ldc.i4.2
    IL_000c:  stind.i4
    IL_000d:  dup
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i
    IL_0010:  ldc.i4.4
    IL_0011:  mul
    IL_0012:  add
    IL_0013:  ldc.i4.3
    IL_0014:  stind.i4
    IL_0015:  dup
    IL_0016:  ldc.i4.3
    IL_0017:  conv.i
    IL_0018:  ldc.i4.4
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldc.i4.4
    IL_001c:  stind.i4
    IL_001d:  ldc.i4.4
    IL_001e:  newobj     instance void valuetype [System.Memory]System.Span`1<int32>::.ctor(void*,
                                                                                            int32)
    IL_0023:  stloc.0
    IL_0024:  ldarg.0
    IL_0025:  ldloc.0
    IL_0026:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS73_StackAllocInitializers::UseSpan(valuetype [System.Memory]System.Span`1<int32>)
    IL_002b:  ret
  } // end of method CS73_StackAllocInitializers::GetSpan2

  .method public hidebysig instance string 
          UseSpan(valuetype [System.Memory]System.Span`1<int32> span) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method CS73_StackAllocInitializers::UseSpan

  .method public hidebysig instance int32 
          GetSize() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method CS73_StackAllocInitializers::GetSize

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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

  .field static assembly initonly int32 '063AAB58782881806084E1A944FBCEE5F5815405' at I_00003240
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=3' '7037807198C22A7D2B0807371D763779A84FDFCF' at I_00003248
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=55' F623596D706F878F1D12C19353913A8E96904144 at I_00003250
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_00003240 = bytearray (
                 00 01 00 01) 
.data cil I_00003244 = int8[4]
.data cil I_00003248 = bytearray (
                 01 02 03) 
.data cil I_0000324B = int8[5]
.data cil I_00003250 = bytearray (
                 01 02 03 05 07 0B 0D 11 13 17 1D 1F 25 29 2B 2F  // ............%)+/
                 35 3B 3D 43 47 49 4F 53 59 61 65 67 6B 6D 71 7F  // 5;=CGIOSYaegkmq.
                 83 89 8B 95 97 9D A3 A7 AD B3 B5 BF C1 C5 C7 D3
                 DF E3 E5 E9 EF F1 FB) 
// *********** DISASSEMBLY COMPLETE ***********************
