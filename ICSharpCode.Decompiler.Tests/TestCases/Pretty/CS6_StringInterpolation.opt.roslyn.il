



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

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
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method CS6_StringInterpolation::Main

  .method public hidebysig static void  General(string[] args) cil managed
  {
    // Code size       278 (0x116)
    .maxstack  6
    IL_0000:  ldstr      "{0}"
    IL_0005:  ldarg.0
    IL_0006:  ldlen
    IL_0007:  conv.i4
    IL_0008:  box        [mscorlib]System.Int32
    IL_000d:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  ldstr      "a{{0{0}"
    IL_001c:  ldarg.0
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  ldstr      "{0:x}"
    IL_0033:  ldarg.0
    IL_0034:  ldlen
    IL_0035:  conv.i4
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  ldstr      "\ta{0}b"
    IL_004a:  ldarg.0
    IL_004b:  ldlen
    IL_004c:  conv.i4
    IL_004d:  box        [mscorlib]System.Int32
    IL_0052:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_0061:  ldc.i4.4
    IL_0062:  newarr     [mscorlib]System.Object
    IL_0067:  dup
    IL_0068:  ldc.i4.0
    IL_0069:  ldarg.0
    IL_006a:  ldlen
    IL_006b:  conv.i4
    IL_006c:  box        [mscorlib]System.Int32
    IL_0071:  stelem.ref
    IL_0072:  dup
    IL_0073:  ldc.i4.1
    IL_0074:  ldarg.0
    IL_0075:  ldc.i4.0
    IL_0076:  ldelem.ref
    IL_0077:  stelem.ref
    IL_0078:  dup
    IL_0079:  ldc.i4.2
    IL_007a:  ldarg.0
    IL_007b:  ldarg.0
    IL_007c:  ldlen
    IL_007d:  conv.i4
    IL_007e:  ldelem.ref
    IL_007f:  stelem.ref
    IL_0080:  dup
    IL_0081:  ldc.i4.3
    IL_0082:  ldarg.0
    IL_0083:  ldlen
    IL_0084:  conv.i4
    IL_0085:  box        [mscorlib]System.Int32
    IL_008a:  stelem.ref
    IL_008b:  call       string [mscorlib]System.String::Format(string,
                                                                object[])
    IL_0090:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0095:  ldstr      "\ta{0}"
    IL_009a:  ldarg.0
    IL_009b:  ldlen
    IL_009c:  brtrue.s   IL_00a1

    IL_009e:  ldc.i4.0
    IL_009f:  br.s       IL_00a2

    IL_00a1:  ldc.i4.5
    IL_00a2:  box        [mscorlib]System.Int32
    IL_00a7:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00ac:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00b1:  ldstr      "\ta{0}"
    IL_00b6:  ldarg.0
    IL_00b7:  dup
    IL_00b8:  brtrue.s   IL_00bc

    IL_00ba:  pop
    IL_00bb:  ldarg.0
    IL_00bc:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00c1:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00c6:  ldstr      "\ta{0}"
    IL_00cb:  ldarg.0
    IL_00cc:  ldc.i4.0
    IL_00cd:  ldelem.ref
    IL_00ce:  ldc.i4.0
    IL_00cf:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00d4:  ldc.i4.s   97
    IL_00d6:  ceq
    IL_00d8:  box        [mscorlib]System.Boolean
    IL_00dd:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00e2:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e7:  ldstr      "\ta{0}"
    IL_00ec:  ldstr      "a{0}"
    IL_00f1:  ldarg.0
    IL_00f2:  ldlen
    IL_00f3:  conv.i4
    IL_00f4:  box        [mscorlib]System.Int32
    IL_00f9:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00fe:  ldarg.0
    IL_00ff:  ldc.i4.0
    IL_0100:  ldelem.ref
    IL_0101:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0106:  box        [mscorlib]System.Boolean
    IL_010b:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0110:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0115:  ret
  } // end of method CS6_StringInterpolation::General

  .method public hidebysig static void  InvalidFormatString(string[] args) cil managed
  {
    // Code size       556 (0x22c)
    .maxstack  3
    IL_0000:  ldstr      ""
    IL_0005:  ldarg.0
    IL_0006:  ldlen
    IL_0007:  conv.i4
    IL_0008:  box        [mscorlib]System.Int32
    IL_000d:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  ldstr      "a"
    IL_001c:  ldarg.0
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  ldstr      "}"
    IL_0033:  ldarg.0
    IL_0034:  ldlen
    IL_0035:  conv.i4
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  ldstr      "{"
    IL_004a:  ldarg.0
    IL_004b:  ldlen
    IL_004c:  conv.i4
    IL_004d:  box        [mscorlib]System.Int32
    IL_0052:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  ldstr      ":"
    IL_0061:  ldarg.0
    IL_0062:  ldlen
    IL_0063:  conv.i4
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_006e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0073:  ldstr      "\t"
    IL_0078:  ldarg.0
    IL_0079:  ldlen
    IL_007a:  conv.i4
    IL_007b:  box        [mscorlib]System.Int32
    IL_0080:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0085:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008a:  ldstr      "\\"
    IL_008f:  ldarg.0
    IL_0090:  ldlen
    IL_0091:  conv.i4
    IL_0092:  box        [mscorlib]System.Int32
    IL_0097:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_009c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a1:  ldstr      "\""
    IL_00a6:  ldarg.0
    IL_00a7:  ldlen
    IL_00a8:  conv.i4
    IL_00a9:  box        [mscorlib]System.Int32
    IL_00ae:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00b3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00b8:  ldstr      "aa"
    IL_00bd:  ldarg.0
    IL_00be:  ldlen
    IL_00bf:  conv.i4
    IL_00c0:  box        [mscorlib]System.Int32
    IL_00c5:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00ca:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00cf:  ldstr      "a}"
    IL_00d4:  ldarg.0
    IL_00d5:  ldlen
    IL_00d6:  conv.i4
    IL_00d7:  box        [mscorlib]System.Int32
    IL_00dc:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00e1:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e6:  ldstr      "a{"
    IL_00eb:  ldarg.0
    IL_00ec:  ldlen
    IL_00ed:  conv.i4
    IL_00ee:  box        [mscorlib]System.Int32
    IL_00f3:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_00f8:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00fd:  ldstr      "a:"
    IL_0102:  ldarg.0
    IL_0103:  ldlen
    IL_0104:  conv.i4
    IL_0105:  box        [mscorlib]System.Int32
    IL_010a:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_010f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0114:  ldstr      "a\t"
    IL_0119:  ldarg.0
    IL_011a:  ldlen
    IL_011b:  conv.i4
    IL_011c:  box        [mscorlib]System.Int32
    IL_0121:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0126:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_012b:  ldstr      "a\\"
    IL_0130:  ldarg.0
    IL_0131:  ldlen
    IL_0132:  conv.i4
    IL_0133:  box        [mscorlib]System.Int32
    IL_0138:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_013d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0142:  ldstr      "a\""
    IL_0147:  ldarg.0
    IL_0148:  ldlen
    IL_0149:  conv.i4
    IL_014a:  box        [mscorlib]System.Int32
    IL_014f:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0154:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0159:  ldstr      "a{:"
    IL_015e:  ldarg.0
    IL_015f:  ldlen
    IL_0160:  conv.i4
    IL_0161:  box        [mscorlib]System.Int32
    IL_0166:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_016b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0170:  ldstr      "a{0"
    IL_0175:  ldarg.0
    IL_0176:  ldlen
    IL_0177:  conv.i4
    IL_0178:  box        [mscorlib]System.Int32
    IL_017d:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0182:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0187:  ldstr      "a{{0"
    IL_018c:  ldarg.0
    IL_018d:  ldlen
    IL_018e:  conv.i4
    IL_018f:  box        [mscorlib]System.Int32
    IL_0194:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0199:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_019e:  ldstr      "}a{{0"
    IL_01a3:  ldarg.0
    IL_01a4:  ldlen
    IL_01a5:  conv.i4
    IL_01a6:  box        [mscorlib]System.Int32
    IL_01ab:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01b0:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01b5:  ldstr      "}{"
    IL_01ba:  ldarg.0
    IL_01bb:  ldlen
    IL_01bc:  conv.i4
    IL_01bd:  box        [mscorlib]System.Int32
    IL_01c2:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01c7:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01cc:  ldstr      "{}"
    IL_01d1:  ldarg.0
    IL_01d2:  ldlen
    IL_01d3:  conv.i4
    IL_01d4:  box        [mscorlib]System.Int32
    IL_01d9:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01de:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01e3:  ldstr      "{0:}"
    IL_01e8:  ldarg.0
    IL_01e9:  ldlen
    IL_01ea:  conv.i4
    IL_01eb:  box        [mscorlib]System.Int32
    IL_01f0:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01f5:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01fa:  ldstr      "{0{a}0}"
    IL_01ff:  ldarg.0
    IL_0200:  ldlen
    IL_0201:  conv.i4
    IL_0202:  box        [mscorlib]System.Int32
    IL_0207:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_020c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0211:  ldstr      "test: {0}"
    IL_0216:  ldstr      ","
    IL_021b:  ldarg.0
    IL_021c:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0221:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0226:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_022b:  ret
  } // end of method CS6_StringInterpolation::InvalidFormatString

  .method public hidebysig instance void 
          FormattableStrings(class [mscorlib]System.FormattableString s,
                             string[] args) cil managed
  {
    // Code size       1325 (0x52d)
    .maxstack  8
    IL_0000:  ldstr      "{0}"
    IL_0005:  ldc.i4.1
    IL_0006:  newarr     [mscorlib]System.Object
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldarg.2
    IL_000e:  ldlen
    IL_000f:  conv.i4
    IL_0010:  box        [mscorlib]System.Int32
    IL_0015:  stelem.ref
    IL_0016:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_001b:  starg.s    s
    IL_001d:  ldstr      "a{{0{0}"
    IL_0022:  ldc.i4.1
    IL_0023:  newarr     [mscorlib]System.Object
    IL_0028:  dup
    IL_0029:  ldc.i4.0
    IL_002a:  ldarg.2
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  box        [mscorlib]System.Int32
    IL_0032:  stelem.ref
    IL_0033:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0038:  starg.s    s
    IL_003a:  ldstr      "{0:x}"
    IL_003f:  ldc.i4.1
    IL_0040:  newarr     [mscorlib]System.Object
    IL_0045:  dup
    IL_0046:  ldc.i4.0
    IL_0047:  ldarg.2
    IL_0048:  ldlen
    IL_0049:  conv.i4
    IL_004a:  box        [mscorlib]System.Int32
    IL_004f:  stelem.ref
    IL_0050:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0055:  starg.s    s
    IL_0057:  ldstr      "\ta{0}b"
    IL_005c:  ldc.i4.1
    IL_005d:  newarr     [mscorlib]System.Object
    IL_0062:  dup
    IL_0063:  ldc.i4.0
    IL_0064:  ldarg.2
    IL_0065:  ldlen
    IL_0066:  conv.i4
    IL_0067:  box        [mscorlib]System.Int32
    IL_006c:  stelem.ref
    IL_006d:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0072:  starg.s    s
    IL_0074:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_0079:  ldc.i4.4
    IL_007a:  newarr     [mscorlib]System.Object
    IL_007f:  dup
    IL_0080:  ldc.i4.0
    IL_0081:  ldarg.2
    IL_0082:  ldlen
    IL_0083:  conv.i4
    IL_0084:  box        [mscorlib]System.Int32
    IL_0089:  stelem.ref
    IL_008a:  dup
    IL_008b:  ldc.i4.1
    IL_008c:  ldarg.2
    IL_008d:  ldc.i4.0
    IL_008e:  ldelem.ref
    IL_008f:  stelem.ref
    IL_0090:  dup
    IL_0091:  ldc.i4.2
    IL_0092:  ldarg.2
    IL_0093:  ldarg.2
    IL_0094:  ldlen
    IL_0095:  conv.i4
    IL_0096:  ldelem.ref
    IL_0097:  stelem.ref
    IL_0098:  dup
    IL_0099:  ldc.i4.3
    IL_009a:  ldarg.2
    IL_009b:  ldlen
    IL_009c:  conv.i4
    IL_009d:  box        [mscorlib]System.Int32
    IL_00a2:  stelem.ref
    IL_00a3:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00a8:  starg.s    s
    IL_00aa:  ldstr      "\ta{0}"
    IL_00af:  ldc.i4.1
    IL_00b0:  newarr     [mscorlib]System.Object
    IL_00b5:  dup
    IL_00b6:  ldc.i4.0
    IL_00b7:  ldarg.2
    IL_00b8:  ldlen
    IL_00b9:  brtrue.s   IL_00be

    IL_00bb:  ldc.i4.0
    IL_00bc:  br.s       IL_00bf

    IL_00be:  ldc.i4.5
    IL_00bf:  box        [mscorlib]System.Int32
    IL_00c4:  stelem.ref
    IL_00c5:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00ca:  starg.s    s
    IL_00cc:  ldstr      "\ta{0}"
    IL_00d1:  ldc.i4.1
    IL_00d2:  newarr     [mscorlib]System.Object
    IL_00d7:  dup
    IL_00d8:  ldc.i4.0
    IL_00d9:  ldarg.2
    IL_00da:  dup
    IL_00db:  brtrue.s   IL_00df

    IL_00dd:  pop
    IL_00de:  ldarg.2
    IL_00df:  stelem.ref
    IL_00e0:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_00e5:  starg.s    s
    IL_00e7:  ldstr      "\ta{0}"
    IL_00ec:  ldc.i4.1
    IL_00ed:  newarr     [mscorlib]System.Object
    IL_00f2:  dup
    IL_00f3:  ldc.i4.0
    IL_00f4:  ldarg.2
    IL_00f5:  ldc.i4.0
    IL_00f6:  ldelem.ref
    IL_00f7:  ldc.i4.0
    IL_00f8:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00fd:  ldc.i4.s   97
    IL_00ff:  ceq
    IL_0101:  box        [mscorlib]System.Boolean
    IL_0106:  stelem.ref
    IL_0107:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_010c:  starg.s    s
    IL_010e:  ldstr      "\ta{0}"
    IL_0113:  ldc.i4.1
    IL_0114:  newarr     [mscorlib]System.Object
    IL_0119:  dup
    IL_011a:  ldc.i4.0
    IL_011b:  ldstr      "a{0}"
    IL_0120:  ldarg.2
    IL_0121:  ldlen
    IL_0122:  conv.i4
    IL_0123:  box        [mscorlib]System.Int32
    IL_0128:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_012d:  ldarg.2
    IL_012e:  ldc.i4.0
    IL_012f:  ldelem.ref
    IL_0130:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0135:  box        [mscorlib]System.Boolean
    IL_013a:  stelem.ref
    IL_013b:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0140:  starg.s    s
    IL_0142:  ldarg.0
    IL_0143:  ldstr      "{0}"
    IL_0148:  ldarg.2
    IL_0149:  ldlen
    IL_014a:  conv.i4
    IL_014b:  box        [mscorlib]System.Int32
    IL_0150:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0155:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_015a:  ldarg.0
    IL_015b:  ldstr      "a{{0{0}"
    IL_0160:  ldarg.2
    IL_0161:  ldlen
    IL_0162:  conv.i4
    IL_0163:  box        [mscorlib]System.Int32
    IL_0168:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_016d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0172:  ldarg.0
    IL_0173:  ldstr      "{0:x}"
    IL_0178:  ldarg.2
    IL_0179:  ldlen
    IL_017a:  conv.i4
    IL_017b:  box        [mscorlib]System.Int32
    IL_0180:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0185:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_018a:  ldarg.0
    IL_018b:  ldstr      "\ta{0}b"
    IL_0190:  ldarg.2
    IL_0191:  ldlen
    IL_0192:  conv.i4
    IL_0193:  box        [mscorlib]System.Int32
    IL_0198:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_019d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01a2:  ldarg.0
    IL_01a3:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_01a8:  ldc.i4.4
    IL_01a9:  newarr     [mscorlib]System.Object
    IL_01ae:  dup
    IL_01af:  ldc.i4.0
    IL_01b0:  ldarg.2
    IL_01b1:  ldlen
    IL_01b2:  conv.i4
    IL_01b3:  box        [mscorlib]System.Int32
    IL_01b8:  stelem.ref
    IL_01b9:  dup
    IL_01ba:  ldc.i4.1
    IL_01bb:  ldarg.2
    IL_01bc:  ldc.i4.0
    IL_01bd:  ldelem.ref
    IL_01be:  stelem.ref
    IL_01bf:  dup
    IL_01c0:  ldc.i4.2
    IL_01c1:  ldarg.2
    IL_01c2:  ldarg.2
    IL_01c3:  ldlen
    IL_01c4:  conv.i4
    IL_01c5:  ldelem.ref
    IL_01c6:  stelem.ref
    IL_01c7:  dup
    IL_01c8:  ldc.i4.3
    IL_01c9:  ldarg.2
    IL_01ca:  ldlen
    IL_01cb:  conv.i4
    IL_01cc:  box        [mscorlib]System.Int32
    IL_01d1:  stelem.ref
    IL_01d2:  call       string [mscorlib]System.String::Format(string,
                                                                object[])
    IL_01d7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01dc:  ldarg.0
    IL_01dd:  ldstr      "\ta{0}"
    IL_01e2:  ldarg.2
    IL_01e3:  ldlen
    IL_01e4:  brtrue.s   IL_01e9

    IL_01e6:  ldc.i4.0
    IL_01e7:  br.s       IL_01ea

    IL_01e9:  ldc.i4.5
    IL_01ea:  box        [mscorlib]System.Int32
    IL_01ef:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_01f4:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_01f9:  ldarg.0
    IL_01fa:  ldstr      "\ta{0}"
    IL_01ff:  ldarg.2
    IL_0200:  dup
    IL_0201:  brtrue.s   IL_0205

    IL_0203:  pop
    IL_0204:  ldarg.2
    IL_0205:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_020a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_020f:  ldarg.0
    IL_0210:  ldstr      "\ta{0}"
    IL_0215:  ldarg.2
    IL_0216:  ldc.i4.0
    IL_0217:  ldelem.ref
    IL_0218:  ldc.i4.0
    IL_0219:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_021e:  ldc.i4.s   97
    IL_0220:  ceq
    IL_0222:  box        [mscorlib]System.Boolean
    IL_0227:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_022c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0231:  ldarg.0
    IL_0232:  ldstr      "\ta{0}"
    IL_0237:  ldstr      "a{0}"
    IL_023c:  ldarg.2
    IL_023d:  ldlen
    IL_023e:  conv.i4
    IL_023f:  box        [mscorlib]System.Int32
    IL_0244:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0249:  ldarg.2
    IL_024a:  ldc.i4.0
    IL_024b:  ldelem.ref
    IL_024c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0251:  box        [mscorlib]System.Boolean
    IL_0256:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_025b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(string)
    IL_0260:  ldarg.0
    IL_0261:  ldstr      "{0}"
    IL_0266:  ldc.i4.1
    IL_0267:  newarr     [mscorlib]System.Object
    IL_026c:  dup
    IL_026d:  ldc.i4.0
    IL_026e:  ldarg.2
    IL_026f:  ldlen
    IL_0270:  conv.i4
    IL_0271:  box        [mscorlib]System.Int32
    IL_0276:  stelem.ref
    IL_0277:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_027c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_0281:  ldarg.0
    IL_0282:  ldstr      "a{{0{0}"
    IL_0287:  ldc.i4.1
    IL_0288:  newarr     [mscorlib]System.Object
    IL_028d:  dup
    IL_028e:  ldc.i4.0
    IL_028f:  ldarg.2
    IL_0290:  ldlen
    IL_0291:  conv.i4
    IL_0292:  box        [mscorlib]System.Int32
    IL_0297:  stelem.ref
    IL_0298:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_029d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02a2:  ldarg.0
    IL_02a3:  ldstr      "{0:x}"
    IL_02a8:  ldc.i4.1
    IL_02a9:  newarr     [mscorlib]System.Object
    IL_02ae:  dup
    IL_02af:  ldc.i4.0
    IL_02b0:  ldarg.2
    IL_02b1:  ldlen
    IL_02b2:  conv.i4
    IL_02b3:  box        [mscorlib]System.Int32
    IL_02b8:  stelem.ref
    IL_02b9:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_02be:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02c3:  ldarg.0
    IL_02c4:  ldstr      "\ta{0}b"
    IL_02c9:  ldc.i4.1
    IL_02ca:  newarr     [mscorlib]System.Object
    IL_02cf:  dup
    IL_02d0:  ldc.i4.0
    IL_02d1:  ldarg.2
    IL_02d2:  ldlen
    IL_02d3:  conv.i4
    IL_02d4:  box        [mscorlib]System.Int32
    IL_02d9:  stelem.ref
    IL_02da:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_02df:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_02e4:  ldarg.0
    IL_02e5:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_02ea:  ldc.i4.4
    IL_02eb:  newarr     [mscorlib]System.Object
    IL_02f0:  dup
    IL_02f1:  ldc.i4.0
    IL_02f2:  ldarg.2
    IL_02f3:  ldlen
    IL_02f4:  conv.i4
    IL_02f5:  box        [mscorlib]System.Int32
    IL_02fa:  stelem.ref
    IL_02fb:  dup
    IL_02fc:  ldc.i4.1
    IL_02fd:  ldarg.2
    IL_02fe:  ldc.i4.0
    IL_02ff:  ldelem.ref
    IL_0300:  stelem.ref
    IL_0301:  dup
    IL_0302:  ldc.i4.2
    IL_0303:  ldarg.2
    IL_0304:  ldarg.2
    IL_0305:  ldlen
    IL_0306:  conv.i4
    IL_0307:  ldelem.ref
    IL_0308:  stelem.ref
    IL_0309:  dup
    IL_030a:  ldc.i4.3
    IL_030b:  ldarg.2
    IL_030c:  ldlen
    IL_030d:  conv.i4
    IL_030e:  box        [mscorlib]System.Int32
    IL_0313:  stelem.ref
    IL_0314:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0319:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_031e:  ldarg.0
    IL_031f:  ldstr      "\ta{0}"
    IL_0324:  ldc.i4.1
    IL_0325:  newarr     [mscorlib]System.Object
    IL_032a:  dup
    IL_032b:  ldc.i4.0
    IL_032c:  ldarg.2
    IL_032d:  ldlen
    IL_032e:  brtrue.s   IL_0333

    IL_0330:  ldc.i4.0
    IL_0331:  br.s       IL_0334

    IL_0333:  ldc.i4.5
    IL_0334:  box        [mscorlib]System.Int32
    IL_0339:  stelem.ref
    IL_033a:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_033f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_0344:  ldarg.0
    IL_0345:  ldstr      "\ta{0}"
    IL_034a:  ldc.i4.1
    IL_034b:  newarr     [mscorlib]System.Object
    IL_0350:  dup
    IL_0351:  ldc.i4.0
    IL_0352:  ldarg.2
    IL_0353:  dup
    IL_0354:  brtrue.s   IL_0358

    IL_0356:  pop
    IL_0357:  ldarg.2
    IL_0358:  stelem.ref
    IL_0359:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_035e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_0363:  ldarg.0
    IL_0364:  ldstr      "\ta{0}"
    IL_0369:  ldc.i4.1
    IL_036a:  newarr     [mscorlib]System.Object
    IL_036f:  dup
    IL_0370:  ldc.i4.0
    IL_0371:  ldarg.2
    IL_0372:  ldc.i4.0
    IL_0373:  ldelem.ref
    IL_0374:  ldc.i4.0
    IL_0375:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_037a:  ldc.i4.s   97
    IL_037c:  ceq
    IL_037e:  box        [mscorlib]System.Boolean
    IL_0383:  stelem.ref
    IL_0384:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0389:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_038e:  ldarg.0
    IL_038f:  ldstr      "\ta{0}"
    IL_0394:  ldc.i4.1
    IL_0395:  newarr     [mscorlib]System.Object
    IL_039a:  dup
    IL_039b:  ldc.i4.0
    IL_039c:  ldstr      "a{0}"
    IL_03a1:  ldarg.2
    IL_03a2:  ldlen
    IL_03a3:  conv.i4
    IL_03a4:  box        [mscorlib]System.Int32
    IL_03a9:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_03ae:  ldarg.2
    IL_03af:  ldc.i4.0
    IL_03b0:  ldelem.ref
    IL_03b1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_03b6:  box        [mscorlib]System.Boolean
    IL_03bb:  stelem.ref
    IL_03bc:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_03c1:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.FormattableString)
    IL_03c6:  ldarg.0
    IL_03c7:  ldstr      "{0}"
    IL_03cc:  ldc.i4.1
    IL_03cd:  newarr     [mscorlib]System.Object
    IL_03d2:  dup
    IL_03d3:  ldc.i4.0
    IL_03d4:  ldarg.2
    IL_03d5:  ldlen
    IL_03d6:  conv.i4
    IL_03d7:  box        [mscorlib]System.Int32
    IL_03dc:  stelem.ref
    IL_03dd:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_03e2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_03e7:  ldarg.0
    IL_03e8:  ldstr      "a{{0{0}"
    IL_03ed:  ldc.i4.1
    IL_03ee:  newarr     [mscorlib]System.Object
    IL_03f3:  dup
    IL_03f4:  ldc.i4.0
    IL_03f5:  ldarg.2
    IL_03f6:  ldlen
    IL_03f7:  conv.i4
    IL_03f8:  box        [mscorlib]System.Int32
    IL_03fd:  stelem.ref
    IL_03fe:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0403:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_0408:  ldarg.0
    IL_0409:  ldstr      "{0:x}"
    IL_040e:  ldc.i4.1
    IL_040f:  newarr     [mscorlib]System.Object
    IL_0414:  dup
    IL_0415:  ldc.i4.0
    IL_0416:  ldarg.2
    IL_0417:  ldlen
    IL_0418:  conv.i4
    IL_0419:  box        [mscorlib]System.Int32
    IL_041e:  stelem.ref
    IL_041f:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0424:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_0429:  ldarg.0
    IL_042a:  ldstr      "\ta{0}b"
    IL_042f:  ldc.i4.1
    IL_0430:  newarr     [mscorlib]System.Object
    IL_0435:  dup
    IL_0436:  ldc.i4.0
    IL_0437:  ldarg.2
    IL_0438:  ldlen
    IL_0439:  conv.i4
    IL_043a:  box        [mscorlib]System.Int32
    IL_043f:  stelem.ref
    IL_0440:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0445:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_044a:  ldarg.0
    IL_044b:  ldstr      "\ta{0}ba{1}a{2}a{3}"
    IL_0450:  ldc.i4.4
    IL_0451:  newarr     [mscorlib]System.Object
    IL_0456:  dup
    IL_0457:  ldc.i4.0
    IL_0458:  ldarg.2
    IL_0459:  ldlen
    IL_045a:  conv.i4
    IL_045b:  box        [mscorlib]System.Int32
    IL_0460:  stelem.ref
    IL_0461:  dup
    IL_0462:  ldc.i4.1
    IL_0463:  ldarg.2
    IL_0464:  ldc.i4.0
    IL_0465:  ldelem.ref
    IL_0466:  stelem.ref
    IL_0467:  dup
    IL_0468:  ldc.i4.2
    IL_0469:  ldarg.2
    IL_046a:  ldarg.2
    IL_046b:  ldlen
    IL_046c:  conv.i4
    IL_046d:  ldelem.ref
    IL_046e:  stelem.ref
    IL_046f:  dup
    IL_0470:  ldc.i4.3
    IL_0471:  ldarg.2
    IL_0472:  ldlen
    IL_0473:  conv.i4
    IL_0474:  box        [mscorlib]System.Int32
    IL_0479:  stelem.ref
    IL_047a:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_047f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_0484:  ldarg.0
    IL_0485:  ldstr      "\ta{0}"
    IL_048a:  ldc.i4.1
    IL_048b:  newarr     [mscorlib]System.Object
    IL_0490:  dup
    IL_0491:  ldc.i4.0
    IL_0492:  ldarg.2
    IL_0493:  ldlen
    IL_0494:  brtrue.s   IL_0499

    IL_0496:  ldc.i4.0
    IL_0497:  br.s       IL_049a

    IL_0499:  ldc.i4.5
    IL_049a:  box        [mscorlib]System.Int32
    IL_049f:  stelem.ref
    IL_04a0:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_04a5:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_04aa:  ldarg.0
    IL_04ab:  ldstr      "\ta{0}"
    IL_04b0:  ldc.i4.1
    IL_04b1:  newarr     [mscorlib]System.Object
    IL_04b6:  dup
    IL_04b7:  ldc.i4.0
    IL_04b8:  ldarg.2
    IL_04b9:  dup
    IL_04ba:  brtrue.s   IL_04be

    IL_04bc:  pop
    IL_04bd:  ldarg.2
    IL_04be:  stelem.ref
    IL_04bf:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_04c4:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_04c9:  ldarg.0
    IL_04ca:  ldstr      "\ta{0}"
    IL_04cf:  ldc.i4.1
    IL_04d0:  newarr     [mscorlib]System.Object
    IL_04d5:  dup
    IL_04d6:  ldc.i4.0
    IL_04d7:  ldarg.2
    IL_04d8:  ldc.i4.0
    IL_04d9:  ldelem.ref
    IL_04da:  ldc.i4.0
    IL_04db:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_04e0:  ldc.i4.s   97
    IL_04e2:  ceq
    IL_04e4:  box        [mscorlib]System.Boolean
    IL_04e9:  stelem.ref
    IL_04ea:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_04ef:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_04f4:  ldarg.0
    IL_04f5:  ldstr      "\ta{0}"
    IL_04fa:  ldc.i4.1
    IL_04fb:  newarr     [mscorlib]System.Object
    IL_0500:  dup
    IL_0501:  ldc.i4.0
    IL_0502:  ldstr      "a{0}"
    IL_0507:  ldarg.2
    IL_0508:  ldlen
    IL_0509:  conv.i4
    IL_050a:  box        [mscorlib]System.Int32
    IL_050f:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0514:  ldarg.2
    IL_0515:  ldc.i4.0
    IL_0516:  ldelem.ref
    IL_0517:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_051c:  box        [mscorlib]System.Boolean
    IL_0521:  stelem.ref
    IL_0522:  call       class [mscorlib]System.FormattableString [mscorlib]System.Runtime.CompilerServices.FormattableStringFactory::Create(string,
                                                                                                                                             object[])
    IL_0527:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation::RequiresCast(class [mscorlib]System.IFormattable)
    IL_052c:  ret
  } // end of method CS6_StringInterpolation::FormattableStrings

  .method public hidebysig instance void 
          RequiresCast(string 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

  .method public hidebysig instance void 
          RequiresCast(class [mscorlib]System.FormattableString 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

  .method public hidebysig instance void 
          RequiresCast(class [mscorlib]System.IFormattable 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method CS6_StringInterpolation::RequiresCast

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CS6_StringInterpolation::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CS6_StringInterpolation


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
