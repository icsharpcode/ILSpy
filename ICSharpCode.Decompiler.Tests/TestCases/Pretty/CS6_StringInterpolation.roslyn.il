
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CS6_StringInterpolation
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
.module CS6_StringInterpolation.dll
// MVID: {13226DB1-4F60-403F-8874-DA41699044FA}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00DD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  General(string[] args) cil managed
  {
    // Code size       241 (0xf1)
    .maxstack  6
    IL_0000:  nop
    IL_0001:  ldstr      "{0}"
    IL_0006:  ldarg.0
    IL_0007:  ldlen
    IL_0008:  conv.i4
    IL_0009:  box        [mscorlib]System.Int32
    IL_000e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0018:  nop
    IL_0019:  ldstr      "a{{0{0}"
    IL_001e:  ldarg.0
    IL_001f:  ldlen
    IL_0020:  conv.i4
    IL_0021:  box        [mscorlib]System.Int32
    IL_0026:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  ldstr      "{0:x}"
    IL_0036:  ldarg.0
    IL_0037:  ldlen
    IL_0038:  conv.i4
    IL_0039:  box        [mscorlib]System.Int32
    IL_003e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0048:  nop
    IL_0049:  ldstr      "\ta{0}b"
    IL_004e:  ldarg.0
    IL_004f:  ldlen
    IL_0050:  conv.i4
    IL_0051:  box        [mscorlib]System.Int32
    IL_0056:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_0066:  ldc.i4.4
    IL_0067:  newarr     [mscorlib]System.Object
    IL_006c:  dup
    IL_006d:  ldc.i4.0
    IL_006e:  ldarg.0
    IL_006f:  ldlen
    IL_0070:  conv.i4
    IL_0071:  box        [mscorlib]System.Int32
    IL_0076:  stelem.ref
    IL_0077:  dup
    IL_0078:  ldc.i4.1
    IL_0079:  ldarg.0
    IL_007a:  ldc.i4.0
    IL_007b:  ldelem.ref
    IL_007c:  stelem.ref
    IL_007d:  dup
    IL_007e:  ldc.i4.2
    IL_007f:  ldarg.0
    IL_0080:  ldarg.0
    IL_0081:  ldlen
    IL_0082:  conv.i4
    IL_0083:  ldelem.ref
    IL_0084:  stelem.ref
    IL_0085:  dup
    IL_0086:  ldc.i4.3
    IL_0087:  ldarg.0
    IL_0088:  ldlen
    IL_0089:  conv.i4
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  stelem.ref
    IL_0090:  call       string [mscorlib]System.String::Format(string,
                                                                object[])
    IL_0095:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009a:  nop
    IL_009b:  ldstr      "\ta{0}"
    IL_00a0:  ldarg.0
    IL_00a1:  ldlen
    IL_00a2:  brtrue.s   IL_00a7

    IL_00a4:  ldc.i4.0
    IL_00a5:  br.s       IL_00a8

    IL_00a7:  ldc.i4.5
    IL_00a8:  box        [mscorlib]System.Int32
    IL_00ad:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00b2:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00b7:  nop
    IL_00b8:  ldstr      "\ta{0}"
    IL_00bd:  ldarg.0
    IL_00be:  dup
    IL_00bf:  brtrue.s   IL_00c3

    IL_00c1:  pop
    IL_00c2:  ldarg.0
    IL_00c3:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00c8:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00cd:  nop
    IL_00ce:  ldstr      "\ta{0}"
    IL_00d3:  ldarg.0
    IL_00d4:  ldc.i4.0
    IL_00d5:  ldelem.ref
    IL_00d6:  ldc.i4.0
    IL_00d7:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00dc:  ldc.i4.s   97
    IL_00de:  ceq
    IL_00e0:  box        [mscorlib]System.Boolean
    IL_00e5:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00ea:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00ef:  nop
    IL_00f0:  ret
  } // end of method CS6_StringInterpolation::General

  .method public hidebysig static void  InvalidFormatString(string[] args) cil managed
  {
    // Code size       581 (0x245)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  ldstr      ""
    IL_0006:  ldarg.0
    IL_0007:  ldlen
    IL_0008:  conv.i4
    IL_0009:  box        [mscorlib]System.Int32
    IL_000e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0018:  nop
    IL_0019:  ldstr      "a"
    IL_001e:  ldarg.0
    IL_001f:  ldlen
    IL_0020:  conv.i4
    IL_0021:  box        [mscorlib]System.Int32
    IL_0026:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  ldstr      "}"
    IL_0036:  ldarg.0
    IL_0037:  ldlen
    IL_0038:  conv.i4
    IL_0039:  box        [mscorlib]System.Int32
    IL_003e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0048:  nop
    IL_0049:  ldstr      "{"
    IL_004e:  ldarg.0
    IL_004f:  ldlen
    IL_0050:  conv.i4
    IL_0051:  box        [mscorlib]System.Int32
    IL_0056:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  ldstr      ":"
    IL_0066:  ldarg.0
    IL_0067:  ldlen
    IL_0068:  conv.i4
    IL_0069:  box        [mscorlib]System.Int32
    IL_006e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0073:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0078:  nop
    IL_0079:  ldstr      "\t"
    IL_007e:  ldarg.0
    IL_007f:  ldlen
    IL_0080:  conv.i4
    IL_0081:  box        [mscorlib]System.Int32
    IL_0086:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_008b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0090:  nop
    IL_0091:  ldstr      "\\"
    IL_0096:  ldarg.0
    IL_0097:  ldlen
    IL_0098:  conv.i4
    IL_0099:  box        [mscorlib]System.Int32
    IL_009e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a8:  nop
    IL_00a9:  ldstr      "\""
    IL_00ae:  ldarg.0
    IL_00af:  ldlen
    IL_00b0:  conv.i4
    IL_00b1:  box        [mscorlib]System.Int32
    IL_00b6:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00bb:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00c0:  nop
    IL_00c1:  ldstr      "aa"
    IL_00c6:  ldarg.0
    IL_00c7:  ldlen
    IL_00c8:  conv.i4
    IL_00c9:  box        [mscorlib]System.Int32
    IL_00ce:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00d3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00d8:  nop
    IL_00d9:  ldstr      "a}"
    IL_00de:  ldarg.0
    IL_00df:  ldlen
    IL_00e0:  conv.i4
    IL_00e1:  box        [mscorlib]System.Int32
    IL_00e6:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00eb:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f0:  nop
    IL_00f1:  ldstr      "a{"
    IL_00f6:  ldarg.0
    IL_00f7:  ldlen
    IL_00f8:  conv.i4
    IL_00f9:  box        [mscorlib]System.Int32
    IL_00fe:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0103:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0108:  nop
    IL_0109:  ldstr      "a:"
    IL_010e:  ldarg.0
    IL_010f:  ldlen
    IL_0110:  conv.i4
    IL_0111:  box        [mscorlib]System.Int32
    IL_0116:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_011b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0120:  nop
    IL_0121:  ldstr      "a\t"
    IL_0126:  ldarg.0
    IL_0127:  ldlen
    IL_0128:  conv.i4
    IL_0129:  box        [mscorlib]System.Int32
    IL_012e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0133:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0138:  nop
    IL_0139:  ldstr      "a\\"
    IL_013e:  ldarg.0
    IL_013f:  ldlen
    IL_0140:  conv.i4
    IL_0141:  box        [mscorlib]System.Int32
    IL_0146:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_014b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0150:  nop
    IL_0151:  ldstr      "a\""
    IL_0156:  ldarg.0
    IL_0157:  ldlen
    IL_0158:  conv.i4
    IL_0159:  box        [mscorlib]System.Int32
    IL_015e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0163:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0168:  nop
    IL_0169:  ldstr      "a{:"
    IL_016e:  ldarg.0
    IL_016f:  ldlen
    IL_0170:  conv.i4
    IL_0171:  box        [mscorlib]System.Int32
    IL_0176:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_017b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0180:  nop
    IL_0181:  ldstr      "a{0"
    IL_0186:  ldarg.0
    IL_0187:  ldlen
    IL_0188:  conv.i4
    IL_0189:  box        [mscorlib]System.Int32
    IL_018e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0193:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0198:  nop
    IL_0199:  ldstr      "a{{0"
    IL_019e:  ldarg.0
    IL_019f:  ldlen
    IL_01a0:  conv.i4
    IL_01a1:  box        [mscorlib]System.Int32
    IL_01a6:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01ab:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01b0:  nop
    IL_01b1:  ldstr      "}a{{0"
    IL_01b6:  ldarg.0
    IL_01b7:  ldlen
    IL_01b8:  conv.i4
    IL_01b9:  box        [mscorlib]System.Int32
    IL_01be:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01c3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01c8:  nop
    IL_01c9:  ldstr      "}{"
    IL_01ce:  ldarg.0
    IL_01cf:  ldlen
    IL_01d0:  conv.i4
    IL_01d1:  box        [mscorlib]System.Int32
    IL_01d6:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01db:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01e0:  nop
    IL_01e1:  ldstr      "{}"
    IL_01e6:  ldarg.0
    IL_01e7:  ldlen
    IL_01e8:  conv.i4
    IL_01e9:  box        [mscorlib]System.Int32
    IL_01ee:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01f3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01f8:  nop
    IL_01f9:  ldstr      "{0:}"
    IL_01fe:  ldarg.0
    IL_01ff:  ldlen
    IL_0200:  conv.i4
    IL_0201:  box        [mscorlib]System.Int32
    IL_0206:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_020b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0210:  nop
    IL_0211:  ldstr      "{0{a}0}"
    IL_0216:  ldarg.0
    IL_0217:  ldlen
    IL_0218:  conv.i4
    IL_0219:  box        [mscorlib]System.Int32
    IL_021e:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0223:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0228:  nop
    IL_0229:  ldstr      "test: {0}"
    IL_022e:  ldstr      ","
    IL_0233:  ldarg.0
    IL_0234:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0239:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_023e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0243:  nop
    IL_0244:  ret
  } // end of method CS6_StringInterpolation::InvalidFormatString

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method CS6_StringInterpolation::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
