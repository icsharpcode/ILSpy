



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
    // Code size       232 (0xe8)
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
    IL_00e7:  ret
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
