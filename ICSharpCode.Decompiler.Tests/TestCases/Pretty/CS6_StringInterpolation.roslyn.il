



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
.module CS6_StringInterpolation.exe
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CS6_StringInterpolation::Main

  .method public hidebysig static void  General(string[] args) cil managed
  {
    // Code size       288 (0x120)
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
    IL_00f0:  ldstr      "\ta{0}"
    IL_00f5:  ldstr      "a{0}"
    IL_00fa:  ldarg.0
    IL_00fb:  ldlen
    IL_00fc:  conv.i4
    IL_00fd:  box        [mscorlib]System.Int32
    IL_0102:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0107:  ldarg.0
    IL_0108:  ldc.i4.0
    IL_0109:  ldelem.ref
    IL_010a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_010f:  box        [mscorlib]System.Boolean
    IL_0114:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0119:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_011e:  nop
    IL_011f:  ret
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

  .method public hidebysig instance void 
          FormattableStrings(class [mscorlib]System.FormattableString s,
                             string[] args) cil managed
  {
    // Code size       1353 (0x549)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "{0}"
    IL_0006:  ldc.i4.1
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.2
    IL_000f:  ldlen
    IL_0010:  conv.i4
    IL_0011:  box        [mscorlib]System.Int32
    IL_0016:  stelem.ref
    IL_0017:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_001c:  starg.s    s
    IL_001e:  ldstr      "a{{0{0}"
    IL_0023:  ldc.i4.1
    IL_0024:  newarr     [mscorlib]System.Object
    IL_0029:  dup
    IL_002a:  ldc.i4.0
    IL_002b:  ldarg.2
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  box        [mscorlib]System.Int32
    IL_0033:  stelem.ref
    IL_0034:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0039:  starg.s    s
    IL_003b:  ldstr      "{0:x}"
    IL_0040:  ldc.i4.1
    IL_0041:  newarr     [mscorlib]System.Object
    IL_0046:  dup
    IL_0047:  ldc.i4.0
    IL_0048:  ldarg.2
    IL_0049:  ldlen
    IL_004a:  conv.i4
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  stelem.ref
    IL_0051:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0056:  starg.s    s
    IL_0058:  ldstr      "\ta{0}b"
    IL_005d:  ldc.i4.1
    IL_005e:  newarr     [mscorlib]System.Object
    IL_0063:  dup
    IL_0064:  ldc.i4.0
    IL_0065:  ldarg.2
    IL_0066:  ldlen
    IL_0067:  conv.i4
    IL_0068:  box        [mscorlib]System.Int32
    IL_006d:  stelem.ref
    IL_006e:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0073:  starg.s    s
    IL_0075:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_007a:  ldc.i4.4
    IL_007b:  newarr     [mscorlib]System.Object
    IL_0080:  dup
    IL_0081:  ldc.i4.0
    IL_0082:  ldarg.2
    IL_0083:  ldlen
    IL_0084:  conv.i4
    IL_0085:  box        [mscorlib]System.Int32
    IL_008a:  stelem.ref
    IL_008b:  dup
    IL_008c:  ldc.i4.1
    IL_008d:  ldarg.2
    IL_008e:  ldc.i4.0
    IL_008f:  ldelem.ref
    IL_0090:  stelem.ref
    IL_0091:  dup
    IL_0092:  ldc.i4.2
    IL_0093:  ldarg.2
    IL_0094:  ldarg.2
    IL_0095:  ldlen
    IL_0096:  conv.i4
    IL_0097:  ldelem.ref
    IL_0098:  stelem.ref
    IL_0099:  dup
    IL_009a:  ldc.i4.3
    IL_009b:  ldarg.2
    IL_009c:  ldlen
    IL_009d:  conv.i4
    IL_009e:  box        [mscorlib]System.Int32
    IL_00a3:  stelem.ref
    IL_00a4:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00a9:  starg.s    s
    IL_00ab:  ldstr      "\ta{0}"
    IL_00b0:  ldc.i4.1
    IL_00b1:  newarr     [mscorlib]System.Object
    IL_00b6:  dup
    IL_00b7:  ldc.i4.0
    IL_00b8:  ldarg.2
    IL_00b9:  ldlen
    IL_00ba:  brtrue.s   IL_00bf

    IL_00bc:  ldc.i4.0
    IL_00bd:  br.s       IL_00c0

    IL_00bf:  ldc.i4.5
    IL_00c0:  box        [mscorlib]System.Int32
    IL_00c5:  stelem.ref
    IL_00c6:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00cb:  starg.s    s
    IL_00cd:  ldstr      "\ta{0}"
    IL_00d2:  ldc.i4.1
    IL_00d3:  newarr     [mscorlib]System.Object
    IL_00d8:  dup
    IL_00d9:  ldc.i4.0
    IL_00da:  ldarg.2
    IL_00db:  dup
    IL_00dc:  brtrue.s   IL_00e0

    IL_00de:  pop
    IL_00df:  ldarg.2
    IL_00e0:  stelem.ref
    IL_00e1:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00e6:  starg.s    s
    IL_00e8:  ldstr      "\ta{0}"
    IL_00ed:  ldc.i4.1
    IL_00ee:  newarr     [mscorlib]System.Object
    IL_00f3:  dup
    IL_00f4:  ldc.i4.0
    IL_00f5:  ldarg.2
    IL_00f6:  ldc.i4.0
    IL_00f7:  ldelem.ref
    IL_00f8:  ldc.i4.0
    IL_00f9:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00fe:  ldc.i4.s   97
    IL_0100:  ceq
    IL_0102:  box        [mscorlib]System.Boolean
    IL_0107:  stelem.ref
    IL_0108:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_010d:  starg.s    s
    IL_010f:  ldstr      "\ta{0}"
    IL_0114:  ldc.i4.1
    IL_0115:  newarr     [mscorlib]System.Object
    IL_011a:  dup
    IL_011b:  ldc.i4.0
    IL_011c:  ldstr      "a{0}"
    IL_0121:  ldarg.2
    IL_0122:  ldlen
    IL_0123:  conv.i4
    IL_0124:  box        [mscorlib]System.Int32
    IL_0129:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_012e:  ldarg.2
    IL_012f:  ldc.i4.0
    IL_0130:  ldelem.ref
    IL_0131:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0136:  box        [mscorlib]System.Boolean
    IL_013b:  stelem.ref
    IL_013c:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0141:  starg.s    s
    IL_0143:  ldarg.0
    IL_0144:  ldstr      "{0}"
    IL_0149:  ldarg.2
    IL_014a:  ldlen
    IL_014b:  conv.i4
    IL_014c:  box        [mscorlib]System.Int32
    IL_0151:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0156:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_015b:  nop
    IL_015c:  ldarg.0
    IL_015d:  ldstr      "a{{0{0}"
    IL_0162:  ldarg.2
    IL_0163:  ldlen
    IL_0164:  conv.i4
    IL_0165:  box        [mscorlib]System.Int32
    IL_016a:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_016f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0174:  nop
    IL_0175:  ldarg.0
    IL_0176:  ldstr      "{0:x}"
    IL_017b:  ldarg.2
    IL_017c:  ldlen
    IL_017d:  conv.i4
    IL_017e:  box        [mscorlib]System.Int32
    IL_0183:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0188:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_018d:  nop
    IL_018e:  ldarg.0
    IL_018f:  ldstr      "\ta{0}b"
    IL_0194:  ldarg.2
    IL_0195:  ldlen
    IL_0196:  conv.i4
    IL_0197:  box        [mscorlib]System.Int32
    IL_019c:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01a1:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01a6:  nop
    IL_01a7:  ldarg.0
    IL_01a8:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_01ad:  ldc.i4.4
    IL_01ae:  newarr     [mscorlib]System.Object
    IL_01b3:  dup
    IL_01b4:  ldc.i4.0
    IL_01b5:  ldarg.2
    IL_01b6:  ldlen
    IL_01b7:  conv.i4
    IL_01b8:  box        [mscorlib]System.Int32
    IL_01bd:  stelem.ref
    IL_01be:  dup
    IL_01bf:  ldc.i4.1
    IL_01c0:  ldarg.2
    IL_01c1:  ldc.i4.0
    IL_01c2:  ldelem.ref
    IL_01c3:  stelem.ref
    IL_01c4:  dup
    IL_01c5:  ldc.i4.2
    IL_01c6:  ldarg.2
    IL_01c7:  ldarg.2
    IL_01c8:  ldlen
    IL_01c9:  conv.i4
    IL_01ca:  ldelem.ref
    IL_01cb:  stelem.ref
    IL_01cc:  dup
    IL_01cd:  ldc.i4.3
    IL_01ce:  ldarg.2
    IL_01cf:  ldlen
    IL_01d0:  conv.i4
    IL_01d1:  box        [mscorlib]System.Int32
    IL_01d6:  stelem.ref
    IL_01d7:  call       string [mscorlib]System.String::Format(string,
                                                                object[])
    IL_01dc:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01e1:  nop
    IL_01e2:  ldarg.0
    IL_01e3:  ldstr      "\ta{0}"
    IL_01e8:  ldarg.2
    IL_01e9:  ldlen
    IL_01ea:  brtrue.s   IL_01ef

    IL_01ec:  ldc.i4.0
    IL_01ed:  br.s       IL_01f0

    IL_01ef:  ldc.i4.5
    IL_01f0:  box        [mscorlib]System.Int32
    IL_01f5:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01fa:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01ff:  nop
    IL_0200:  ldarg.0
    IL_0201:  ldstr      "\ta{0}"
    IL_0206:  ldarg.2
    IL_0207:  dup
    IL_0208:  brtrue.s   IL_020c

    IL_020a:  pop
    IL_020b:  ldarg.2
    IL_020c:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0211:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0216:  nop
    IL_0217:  ldarg.0
    IL_0218:  ldstr      "\ta{0}"
    IL_021d:  ldarg.2
    IL_021e:  ldc.i4.0
    IL_021f:  ldelem.ref
    IL_0220:  ldc.i4.0
    IL_0221:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0226:  ldc.i4.s   97
    IL_0228:  ceq
    IL_022a:  box        [mscorlib]System.Boolean
    IL_022f:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0234:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0239:  nop
    IL_023a:  ldarg.0
    IL_023b:  ldstr      "\ta{0}"
    IL_0240:  ldstr      "a{0}"
    IL_0245:  ldarg.2
    IL_0246:  ldlen
    IL_0247:  conv.i4
    IL_0248:  box        [mscorlib]System.Int32
    IL_024d:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0252:  ldarg.2
    IL_0253:  ldc.i4.0
    IL_0254:  ldelem.ref
    IL_0255:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_025a:  box        [mscorlib]System.Boolean
    IL_025f:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0264:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0269:  nop
    IL_026a:  ldarg.0
    IL_026b:  ldstr      "{0}"
    IL_0270:  ldc.i4.1
    IL_0271:  newarr     [mscorlib]System.Object
    IL_0276:  dup
    IL_0277:  ldc.i4.0
    IL_0278:  ldarg.2
    IL_0279:  ldlen
    IL_027a:  conv.i4
    IL_027b:  box        [mscorlib]System.Int32
    IL_0280:  stelem.ref
    IL_0281:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0286:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_028b:  nop
    IL_028c:  ldarg.0
    IL_028d:  ldstr      "a{{0{0}"
    IL_0292:  ldc.i4.1
    IL_0293:  newarr     [mscorlib]System.Object
    IL_0298:  dup
    IL_0299:  ldc.i4.0
    IL_029a:  ldarg.2
    IL_029b:  ldlen
    IL_029c:  conv.i4
    IL_029d:  box        [mscorlib]System.Int32
    IL_02a2:  stelem.ref
    IL_02a3:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_02a8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02ad:  nop
    IL_02ae:  ldarg.0
    IL_02af:  ldstr      "{0:x}"
    IL_02b4:  ldc.i4.1
    IL_02b5:  newarr     [mscorlib]System.Object
    IL_02ba:  dup
    IL_02bb:  ldc.i4.0
    IL_02bc:  ldarg.2
    IL_02bd:  ldlen
    IL_02be:  conv.i4
    IL_02bf:  box        [mscorlib]System.Int32
    IL_02c4:  stelem.ref
    IL_02c5:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_02ca:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02cf:  nop
    IL_02d0:  ldarg.0
    IL_02d1:  ldstr      "\ta{0}b"
    IL_02d6:  ldc.i4.1
    IL_02d7:  newarr     [mscorlib]System.Object
    IL_02dc:  dup
    IL_02dd:  ldc.i4.0
    IL_02de:  ldarg.2
    IL_02df:  ldlen
    IL_02e0:  conv.i4
    IL_02e1:  box        [mscorlib]System.Int32
    IL_02e6:  stelem.ref
    IL_02e7:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_02ec:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02f1:  nop
    IL_02f2:  ldarg.0
    IL_02f3:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_02f8:  ldc.i4.4
    IL_02f9:  newarr     [mscorlib]System.Object
    IL_02fe:  dup
    IL_02ff:  ldc.i4.0
    IL_0300:  ldarg.2
    IL_0301:  ldlen
    IL_0302:  conv.i4
    IL_0303:  box        [mscorlib]System.Int32
    IL_0308:  stelem.ref
    IL_0309:  dup
    IL_030a:  ldc.i4.1
    IL_030b:  ldarg.2
    IL_030c:  ldc.i4.0
    IL_030d:  ldelem.ref
    IL_030e:  stelem.ref
    IL_030f:  dup
    IL_0310:  ldc.i4.2
    IL_0311:  ldarg.2
    IL_0312:  ldarg.2
    IL_0313:  ldlen
    IL_0314:  conv.i4
    IL_0315:  ldelem.ref
    IL_0316:  stelem.ref
    IL_0317:  dup
    IL_0318:  ldc.i4.3
    IL_0319:  ldarg.2
    IL_031a:  ldlen
    IL_031b:  conv.i4
    IL_031c:  box        [mscorlib]System.Int32
    IL_0321:  stelem.ref
    IL_0322:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0327:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_032c:  nop
    IL_032d:  ldarg.0
    IL_032e:  ldstr      "\ta{0}"
    IL_0333:  ldc.i4.1
    IL_0334:  newarr     [mscorlib]System.Object
    IL_0339:  dup
    IL_033a:  ldc.i4.0
    IL_033b:  ldarg.2
    IL_033c:  ldlen
    IL_033d:  brtrue.s   IL_0342

    IL_033f:  ldc.i4.0
    IL_0340:  br.s       IL_0343

    IL_0342:  ldc.i4.5
    IL_0343:  box        [mscorlib]System.Int32
    IL_0348:  stelem.ref
    IL_0349:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_034e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_0353:  nop
    IL_0354:  ldarg.0
    IL_0355:  ldstr      "\ta{0}"
    IL_035a:  ldc.i4.1
    IL_035b:  newarr     [mscorlib]System.Object
    IL_0360:  dup
    IL_0361:  ldc.i4.0
    IL_0362:  ldarg.2
    IL_0363:  dup
    IL_0364:  brtrue.s   IL_0368

    IL_0366:  pop
    IL_0367:  ldarg.2
    IL_0368:  stelem.ref
    IL_0369:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_036e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_0373:  nop
    IL_0374:  ldarg.0
    IL_0375:  ldstr      "\ta{0}"
    IL_037a:  ldc.i4.1
    IL_037b:  newarr     [mscorlib]System.Object
    IL_0380:  dup
    IL_0381:  ldc.i4.0
    IL_0382:  ldarg.2
    IL_0383:  ldc.i4.0
    IL_0384:  ldelem.ref
    IL_0385:  ldc.i4.0
    IL_0386:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_038b:  ldc.i4.s   97
    IL_038d:  ceq
    IL_038f:  box        [mscorlib]System.Boolean
    IL_0394:  stelem.ref
    IL_0395:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_039a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_039f:  nop
    IL_03a0:  ldarg.0
    IL_03a1:  ldstr      "\ta{0}"
    IL_03a6:  ldc.i4.1
    IL_03a7:  newarr     [mscorlib]System.Object
    IL_03ac:  dup
    IL_03ad:  ldc.i4.0
    IL_03ae:  ldstr      "a{0}"
    IL_03b3:  ldarg.2
    IL_03b4:  ldlen
    IL_03b5:  conv.i4
    IL_03b6:  box        [mscorlib]System.Int32
    IL_03bb:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_03c0:  ldarg.2
    IL_03c1:  ldc.i4.0
    IL_03c2:  ldelem.ref
    IL_03c3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_03c8:  box        [mscorlib]System.Boolean
    IL_03cd:  stelem.ref
    IL_03ce:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_03d3:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_03d8:  nop
    IL_03d9:  ldarg.0
    IL_03da:  ldstr      "{0}"
    IL_03df:  ldc.i4.1
    IL_03e0:  newarr     [mscorlib]System.Object
    IL_03e5:  dup
    IL_03e6:  ldc.i4.0
    IL_03e7:  ldarg.2
    IL_03e8:  ldlen
    IL_03e9:  conv.i4
    IL_03ea:  box        [mscorlib]System.Int32
    IL_03ef:  stelem.ref
    IL_03f0:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_03f5:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_03fa:  nop
    IL_03fb:  ldarg.0
    IL_03fc:  ldstr      "a{{0{0}"
    IL_0401:  ldc.i4.1
    IL_0402:  newarr     [mscorlib]System.Object
    IL_0407:  dup
    IL_0408:  ldc.i4.0
    IL_0409:  ldarg.2
    IL_040a:  ldlen
    IL_040b:  conv.i4
    IL_040c:  box        [mscorlib]System.Int32
    IL_0411:  stelem.ref
    IL_0412:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0417:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_041c:  nop
    IL_041d:  ldarg.0
    IL_041e:  ldstr      "{0:x}"
    IL_0423:  ldc.i4.1
    IL_0424:  newarr     [mscorlib]System.Object
    IL_0429:  dup
    IL_042a:  ldc.i4.0
    IL_042b:  ldarg.2
    IL_042c:  ldlen
    IL_042d:  conv.i4
    IL_042e:  box        [mscorlib]System.Int32
    IL_0433:  stelem.ref
    IL_0434:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0439:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_043e:  nop
    IL_043f:  ldarg.0
    IL_0440:  ldstr      "\ta{0}b"
    IL_0445:  ldc.i4.1
    IL_0446:  newarr     [mscorlib]System.Object
    IL_044b:  dup
    IL_044c:  ldc.i4.0
    IL_044d:  ldarg.2
    IL_044e:  ldlen
    IL_044f:  conv.i4
    IL_0450:  box        [mscorlib]System.Int32
    IL_0455:  stelem.ref
    IL_0456:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_045b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_0460:  nop
    IL_0461:  ldarg.0
    IL_0462:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_0467:  ldc.i4.4
    IL_0468:  newarr     [mscorlib]System.Object
    IL_046d:  dup
    IL_046e:  ldc.i4.0
    IL_046f:  ldarg.2
    IL_0470:  ldlen
    IL_0471:  conv.i4
    IL_0472:  box        [mscorlib]System.Int32
    IL_0477:  stelem.ref
    IL_0478:  dup
    IL_0479:  ldc.i4.1
    IL_047a:  ldarg.2
    IL_047b:  ldc.i4.0
    IL_047c:  ldelem.ref
    IL_047d:  stelem.ref
    IL_047e:  dup
    IL_047f:  ldc.i4.2
    IL_0480:  ldarg.2
    IL_0481:  ldarg.2
    IL_0482:  ldlen
    IL_0483:  conv.i4
    IL_0484:  ldelem.ref
    IL_0485:  stelem.ref
    IL_0486:  dup
    IL_0487:  ldc.i4.3
    IL_0488:  ldarg.2
    IL_0489:  ldlen
    IL_048a:  conv.i4
    IL_048b:  box        [mscorlib]System.Int32
    IL_0490:  stelem.ref
    IL_0491:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0496:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_049b:  nop
    IL_049c:  ldarg.0
    IL_049d:  ldstr      "\ta{0}"
    IL_04a2:  ldc.i4.1
    IL_04a3:  newarr     [mscorlib]System.Object
    IL_04a8:  dup
    IL_04a9:  ldc.i4.0
    IL_04aa:  ldarg.2
    IL_04ab:  ldlen
    IL_04ac:  brtrue.s   IL_04b1

    IL_04ae:  ldc.i4.0
    IL_04af:  br.s       IL_04b2

    IL_04b1:  ldc.i4.5
    IL_04b2:  box        [mscorlib]System.Int32
    IL_04b7:  stelem.ref
    IL_04b8:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_04bd:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_04c2:  nop
    IL_04c3:  ldarg.0
    IL_04c4:  ldstr      "\ta{0}"
    IL_04c9:  ldc.i4.1
    IL_04ca:  newarr     [mscorlib]System.Object
    IL_04cf:  dup
    IL_04d0:  ldc.i4.0
    IL_04d1:  ldarg.2
    IL_04d2:  dup
    IL_04d3:  brtrue.s   IL_04d7

    IL_04d5:  pop
    IL_04d6:  ldarg.2
    IL_04d7:  stelem.ref
    IL_04d8:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_04dd:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_04e2:  nop
    IL_04e3:  ldarg.0
    IL_04e4:  ldstr      "\ta{0}"
    IL_04e9:  ldc.i4.1
    IL_04ea:  newarr     [mscorlib]System.Object
    IL_04ef:  dup
    IL_04f0:  ldc.i4.0
    IL_04f1:  ldarg.2
    IL_04f2:  ldc.i4.0
    IL_04f3:  ldelem.ref
    IL_04f4:  ldc.i4.0
    IL_04f5:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_04fa:  ldc.i4.s   97
    IL_04fc:  ceq
    IL_04fe:  box        [mscorlib]System.Boolean
    IL_0503:  stelem.ref
    IL_0504:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0509:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_050e:  nop
    IL_050f:  ldarg.0
    IL_0510:  ldstr      "\ta{0}"
    IL_0515:  ldc.i4.1
    IL_0516:  newarr     [mscorlib]System.Object
    IL_051b:  dup
    IL_051c:  ldc.i4.0
    IL_051d:  ldstr      "a{0}"
    IL_0522:  ldarg.2
    IL_0523:  ldlen
    IL_0524:  conv.i4
    IL_0525:  box        [mscorlib]System.Int32
    IL_052a:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_052f:  ldarg.2
    IL_0530:  ldc.i4.0
    IL_0531:  ldelem.ref
    IL_0532:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0537:  box        [mscorlib]System.Boolean
    IL_053c:  stelem.ref
    IL_053d:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0542:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_0547:  nop
    IL_0548:  ret
  } // end of method CS6_StringInterpolation::FormattableStrings

  .method public hidebysig instance void 
          RequiresCast(string 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

  .method public hidebysig instance void 
          RequiresCast(class [mscorlib]System.FormattableString 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

  .method public hidebysig instance void 
          RequiresCast(class [mscorlib]System.IFormattable 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

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
