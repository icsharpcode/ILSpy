.assembly extern System.Data
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}

.assembly extern System.Windows.Forms
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}

.assembly Issue2260
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
		01 00 08 00 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
		01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
		63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 07 01 00 00 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.module Issue2260.exe
// MVID: {A7498FA1-F4D7-486E-A872-3DC0CAFD87E2}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = (
	01 00 00 00
)

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2260
    extends [mscorlib]System.Object
{
    
    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        .maxstack 8

        ldarg.0
        call instance void [mscorlib]System.Object::.ctor()
        ret
    }

  .method private hidebysig 
	  instance void dgvItemList_CellValueChanged (
		  object sender,
		  class [System.Windows.Forms]System.Windows.Forms.DataGridViewCellEventArgs e
	  ) cil managed 
  {
	  // Method begins at RVA 0x2088
	  // Code size 885 (0x375)
	  .maxstack 2
	  .locals init (
		  [0] string,
		  [1] class [System.Data]System.Data.DataRow,
		  [2] string,
		  [3] class [System.Data]System.Data.DataTable,
		  [4] string,
		  [5] uint32,
		  [6] object,
		  [7] class [System.Data]System.Data.DataTable,
		  [8] valuetype [mscorlib]System.Decimal,
		  [9] valuetype [mscorlib]System.Decimal,
		  [10] valuetype [mscorlib]System.Decimal,
		  [11] valuetype [mscorlib]System.Decimal,
		  [12] valuetype [mscorlib]System.Decimal,
		  [13] valuetype [mscorlib]System.Decimal,
		  [14] valuetype [mscorlib]System.Decimal,
		  [15] valuetype [mscorlib]System.Decimal,
		  [16] valuetype [mscorlib]System.Decimal,
		  [17] valuetype [mscorlib]System.Decimal,
		  [18] valuetype [mscorlib]System.Decimal,
		  [19] valuetype [mscorlib]System.Decimal,
		  [20] valuetype [mscorlib]System.Decimal,
		  [21] valuetype [mscorlib]System.Decimal,
		  [22] valuetype [mscorlib]System.Decimal,
		  [23] valuetype [mscorlib]System.Decimal,
		  [24] valuetype [mscorlib]System.Decimal,
		  [25] valuetype [mscorlib]System.Decimal,
		  [26] valuetype [mscorlib]System.Decimal
	  )

	  IL_0000: ldloc.0
	  IL_0001: stloc.s 4
	  IL_0003: ldloc.s 4
	  IL_0005: call uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
	  IL_000a: stloc.s 5
	  IL_000c: ldloc.s 5
	  IL_000e: ldc.i4 -1481643850
	  IL_0013: bgt.un.s IL_0031

	  IL_0015: ldloc.s 5
	  IL_0017: ldc.i4 1288728352
	  IL_001c: beq.s IL_0063

	  IL_001e: ldloc.s 5
	  IL_0020: ldc.i4 -1700853173
	  IL_0025: beq.s IL_0092

	  IL_0027: ldloc.s 5
	  IL_0029: ldc.i4 -1481643850
	  IL_002e: beq.s IL_0080

	  IL_0030: ret

	  IL_0031: ldloc.s 5
	  IL_0033: ldc.i4 -497189362
	  IL_0038: bgt.un.s IL_0050

	  IL_003a: ldloc.s 5
	  IL_003c: ldc.i4 -513966981
	  IL_0041: beq IL_00c8

	  IL_0046: ldloc.s 5
	  IL_0048: ldc.i4 -497189362
	  IL_004d: beq.s IL_00b6

	  IL_004f: ret

	  IL_0050: ldloc.s 5
	  IL_0052: ldc.i4 -451434784
	  IL_0057: beq.s IL_0071

	  IL_0059: ldloc.s 5
	  IL_005b: ldc.i4 -413301267
	  IL_0060: beq.s IL_00a4

	  IL_0062: ret

	  IL_0063: ldloc.s 4
	  IL_0065: ldstr "rowno"
	  IL_006a: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_006f: pop
	  IL_0070: ret

	  IL_0071: ldloc.s 4
	  IL_0073: ldstr "item_no"
	  IL_0078: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_007d: brtrue.s IL_00da

	  IL_007f: ret

	  IL_0080: ldloc.s 4
	  IL_0082: ldstr "stock_qty"
	  IL_0087: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_008c: brtrue IL_021f

	  IL_0091: ret

	  IL_0092: ldloc.s 4
	  IL_0094: ldstr "new_price"
	  IL_0099: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_009e: brtrue IL_0285

	  IL_00a3: ret

	  IL_00a4: ldloc.s 4
	  IL_00a6: ldstr "new_price4"
	  IL_00ab: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_00b0: brtrue IL_02d6

	  IL_00b5: ret

	  IL_00b6: ldloc.s 4
	  IL_00b8: ldstr "new_price1"
	  IL_00bd: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_00c2: brtrue IL_0303

	  IL_00c7: ret

	  IL_00c8: ldloc.s 4
	  IL_00ca: ldstr "new_price2"
	  IL_00cf: call bool [mscorlib]System.String::op_Equality(string, string)
	  IL_00d4: brtrue IL_0338

	  IL_00d9: ret

	  IL_00da: nop
	  IL_00db: nop
	  IL_00dc: nop
	  IL_00dd: nop
	  IL_00de: nop
	  IL_00df: nop
	  IL_00e0: nop
	  IL_00e1: nop
	  IL_00e2: nop
	  IL_00e3: nop
	  IL_00e4: nop
	  IL_00e5: nop
	  IL_00e6: nop
	  IL_00e7: nop
	  IL_00e8: nop
	  IL_00e9: nop
	  IL_00ea: nop
	  IL_00eb: nop
	  IL_00ec: nop
	  IL_00ed: nop
	  IL_00ee: nop
	  IL_00ef: nop
	  IL_00f0: nop
	  IL_00f1: nop
	  IL_00f2: nop
	  IL_00f3: nop
	  IL_00f4: nop
	  IL_00f5: nop
	  IL_00f6: nop
	  IL_00f7: nop
	  IL_00f8: nop
	  IL_00f9: nop
	  IL_00fa: nop
	  IL_00fb: nop
	  IL_00fc: nop
	  IL_00fd: nop
	  IL_00fe: nop
	  IL_00ff: nop
	  IL_0100: nop
	  IL_0101: nop
	  IL_0102: nop
	  IL_0103: nop
	  IL_0104: nop
	  IL_0105: nop
	  IL_0106: nop
	  IL_0107: nop
	  IL_0108: nop
	  IL_0109: nop
	  IL_010a: nop
	  IL_010b: nop
	  IL_010c: nop
	  IL_010d: nop
	  IL_010e: nop
	  IL_010f: nop
	  IL_0110: nop
	  IL_0111: nop
	  IL_0112: nop
	  IL_0113: nop
	  IL_0114: nop
	  IL_0115: nop
	  IL_0116: nop
	  IL_0117: nop
	  IL_0118: nop
	  IL_0119: nop
	  IL_011a: nop
	  IL_011b: nop
	  IL_011c: nop
	  IL_011d: nop
	  IL_011e: nop
	  IL_011f: nop
	  IL_0120: nop
	  IL_0121: nop
	  IL_0122: nop
	  IL_0123: nop
	  IL_0124: nop
	  IL_0125: nop
	  IL_0126: nop
	  IL_0127: nop
	  IL_0128: nop
	  IL_0129: nop
	  IL_012a: nop
	  IL_012b: nop
	  IL_012c: nop
	  IL_012d: nop
	  IL_012e: nop
	  IL_012f: nop
	  IL_0130: nop
	  IL_0131: nop
	  IL_0132: nop
	  IL_0133: nop
	  IL_0134: nop
	  IL_0135: nop
	  IL_0136: nop
	  IL_0137: nop
	  IL_0138: nop
	  IL_0139: nop
	  IL_013a: nop
	  IL_013b: nop
	  IL_013c: nop
	  IL_013d: nop
	  IL_013e: nop
	  IL_013f: nop
	  IL_0140: nop
	  IL_0141: nop
	  IL_0142: nop
	  IL_0143: nop
	  IL_0144: nop
	  IL_0145: nop
	  IL_0146: nop
	  IL_0147: nop
	  IL_0148: nop
	  IL_0149: nop
	  IL_014a: nop
	  IL_014b: nop
	  IL_014c: nop
	  IL_014d: nop
	  IL_014e: nop
	  IL_014f: nop
	  IL_0150: nop
	  IL_0151: nop
	  IL_0152: nop
	  IL_0153: nop
	  IL_0154: nop
	  IL_0155: nop
	  IL_0156: nop
	  IL_0157: nop
	  IL_0158: nop
	  IL_0159: nop
	  IL_015a: nop
	  IL_015b: nop
	  IL_015c: nop
	  IL_015d: nop
	  IL_015e: nop
	  IL_015f: nop
	  IL_0160: nop
	  IL_0161: nop
	  IL_0162: nop
	  IL_0163: nop
	  IL_0164: nop
	  IL_0165: nop
	  IL_0166: nop
	  IL_0167: nop
	  IL_0168: nop
	  IL_0169: nop
	  IL_016a: nop
	  IL_016b: nop
	  IL_016c: nop
	  IL_016d: nop
	  IL_016e: nop
	  IL_016f: nop
	  IL_0170: nop
	  IL_0171: nop
	  IL_0172: nop
	  IL_0173: nop
	  IL_0174: nop
	  IL_0175: nop
	  IL_0176: nop
	  IL_0177: nop
	  IL_0178: nop
	  IL_0179: nop
	  IL_017a: nop
	  IL_017b: nop
	  IL_017c: nop
	  IL_017d: nop
	  IL_017e: nop
	  IL_017f: nop
	  IL_0180: nop
	  IL_0181: nop
	  IL_0182: nop
	  IL_0183: nop
	  IL_0184: nop
	  IL_0185: nop
	  IL_0186: nop
	  IL_0187: nop
	  IL_0188: nop
	  IL_0189: nop
	  IL_018a: nop
	  IL_018b: nop
	  IL_018c: nop
	  IL_018d: nop
	  IL_018e: nop
	  IL_018f: nop
	  IL_0190: nop
	  IL_0191: nop
	  IL_0192: nop
	  IL_0193: nop
	  IL_0194: nop
	  IL_0195: nop
	  IL_0196: nop
	  IL_0197: nop
	  IL_0198: nop
	  IL_0199: nop
	  IL_019a: nop
	  IL_019b: nop
	  IL_019c: nop
	  IL_019d: nop
	  IL_019e: nop
	  IL_019f: nop
	  IL_01a0: nop
	  IL_01a1: nop
	  IL_01a2: nop
	  IL_01a3: nop
	  IL_01a4: nop
	  IL_01a5: nop
	  IL_01a6: nop
	  IL_01a7: nop
	  IL_01a8: nop
	  IL_01a9: nop
	  IL_01aa: nop
	  IL_01ab: nop
	  IL_01ac: nop
	  IL_01ad: nop
	  IL_01ae: nop
	  IL_01af: nop
	  IL_01b0: nop
	  IL_01b1: nop
	  IL_01b2: nop
	  IL_01b3: nop
	  IL_01b4: nop
	  IL_01b5: nop
	  IL_01b6: nop
	  IL_01b7: nop
	  IL_01b8: nop
	  IL_01b9: nop
	  IL_01ba: nop
	  IL_01bb: nop
	  IL_01bc: nop
	  IL_01bd: nop
	  IL_01be: nop
	  IL_01bf: nop
	  IL_01c0: nop
	  IL_01c1: nop
	  IL_01c2: nop
	  IL_01c3: nop
	  IL_01c4: nop
	  IL_01c5: nop
	  IL_01c6: nop
	  IL_01c7: nop
	  IL_01c8: nop
	  IL_01c9: nop
	  IL_01ca: nop
	  IL_01cb: nop
	  IL_01cc: nop
	  IL_01cd: nop
	  IL_01ce: nop
	  IL_01cf: nop
	  IL_01d0: nop
	  IL_01d1: nop
	  IL_01d2: nop
	  IL_01d3: nop
	  IL_01d4: nop
	  IL_01d5: nop
	  IL_01d6: nop
	  IL_01d7: nop
	  IL_01d8: nop
	  IL_01d9: nop
	  IL_01da: nop
	  IL_01db: nop
	  IL_01dc: nop
	  IL_01dd: nop
	  IL_01de: nop
	  IL_01df: nop
	  IL_01e0: nop
	  IL_01e1: nop
	  IL_01e2: nop
	  IL_01e3: nop
	  IL_01e4: nop
	  IL_01e5: nop
	  IL_01e6: nop
	  IL_01e7: nop
	  IL_01e8: nop
	  IL_01e9: nop
	  IL_01ea: nop
	  IL_01eb: nop
	  IL_01ec: nop
	  IL_01ed: nop
	  IL_01ee: nop
	  IL_01ef: nop
	  IL_01f0: nop
	  IL_01f1: nop
	  IL_01f2: nop
	  IL_01f3: nop
	  IL_01f4: nop
	  IL_01f5: nop
	  IL_01f6: nop
	  IL_01f7: nop
	  IL_01f8: nop
	  IL_01f9: nop
	  IL_01fa: nop
	  IL_01fb: nop
	  IL_01fc: nop
	  IL_01fd: nop
	  IL_01fe: nop
	  IL_01ff: nop
	  IL_0200: nop
	  IL_0201: nop
	  IL_0202: nop
	  IL_0203: nop
	  IL_0204: nop
	  IL_0205: nop
	  IL_0206: nop
	  IL_0207: nop
	  IL_0208: nop
	  IL_0209: nop
	  IL_020a: nop
	  IL_020b: nop
	  IL_020c: nop
	  IL_020d: nop
	  IL_020e: nop
	  IL_020f: nop
	  IL_0210: nop
	  IL_0211: nop
	  IL_0212: nop
	  IL_0213: nop
	  IL_0214: nop
	  IL_0215: nop
	  IL_0216: nop
	  IL_0217: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_021c: pop
	  IL_021d: nop
	  IL_021e: ret

	  IL_021f: ldloca.s 10
	  IL_0221: initobj [mscorlib]System.Decimal
	  IL_0227: ldloc.2
	  IL_0228: ldloca.s 10
	  IL_022a: call bool [mscorlib]System.Decimal::TryParse(string, valuetype [mscorlib]System.Decimal&)
	  IL_022f: brtrue.s IL_023c

	  IL_0231: nop
	  IL_0232: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_0237: pop
	  IL_0238: nop
	  IL_0239: nop
	  IL_023a: nop
	  IL_023b: ret

	  IL_023c: ldloc.s 10
	  IL_023e: ldsfld valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::One
	  IL_0243: call bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	  IL_0248: brfalse.s IL_025c

	  IL_024a: nop
	  IL_024b: nop
	  IL_024c: nop
	  IL_024d: nop
	  IL_024e: nop
	  IL_024f: nop
	  IL_0250: nop
	  IL_0251: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_0256: pop
	  IL_0257: nop
	  IL_0258: nop
	  IL_0259: nop
	  IL_025a: nop
	  IL_025b: nop

	  IL_025c: nop
	  IL_025d: nop
	  IL_025e: nop
	  IL_025f: nop
	  IL_0260: nop
	  IL_0261: nop
	  IL_0262: nop
	  IL_0263: nop
	  IL_0264: nop
	  IL_0265: nop
	  IL_0266: nop
	  IL_0267: nop
	  IL_0268: nop
	  IL_0269: nop
	  IL_026a: nop
	  IL_026b: nop
	  IL_026c: nop
	  IL_026d: nop
	  IL_026e: nop
	  IL_026f: nop
	  IL_0270: nop
	  IL_0271: nop
	  IL_0272: nop
	  IL_0273: nop
	  IL_0274: nop
	  IL_0275: nop
	  IL_0276: nop
	  IL_0277: nop
	  IL_0278: nop
	  IL_0279: nop
	  IL_027a: nop
	  IL_027b: nop
	  IL_027c: nop
	  IL_027d: nop
	  IL_027e: nop
	  IL_027f: nop
	  IL_0280: nop
	  IL_0281: nop
	  IL_0282: nop
	  IL_0283: nop
	  IL_0284: ret

	  IL_0285: ldloca.s 15
	  IL_0287: initobj [mscorlib]System.Decimal
	  IL_028d: nop
	  IL_028e: nop
	  IL_028f: nop
	  IL_0290: nop
	  IL_0291: nop
	  IL_0292: nop
	  IL_0293: nop
	  IL_0294: nop
	  IL_0295: nop
	  IL_0296: nop
	  IL_0297: nop
	  IL_0298: nop
	  IL_0299: nop
	  IL_029a: nop
	  IL_029b: nop
	  IL_029c: nop
	  IL_029d: nop
	  IL_029e: nop
	  IL_029f: nop
	  IL_02a0: nop
	  IL_02a1: nop
	  IL_02a2: nop
	  IL_02a3: nop
	  IL_02a4: nop
	  IL_02a5: nop
	  IL_02a6: nop
	  IL_02a7: nop
	  IL_02a8: nop
	  IL_02a9: nop
	  IL_02aa: nop
	  IL_02ab: nop
	  IL_02ac: nop
	  IL_02ad: nop
	  IL_02ae: nop
	  IL_02af: nop
	  IL_02b0: nop
	  IL_02b1: nop
	  IL_02b2: nop
	  IL_02b3: nop
	  IL_02b4: nop
	  IL_02b5: nop
	  IL_02b6: nop
	  IL_02b7: nop
	  IL_02b8: nop
	  IL_02b9: nop
	  IL_02ba: nop
	  IL_02bb: nop
	  IL_02bc: nop
	  IL_02bd: nop
	  IL_02be: nop
	  IL_02bf: nop
	  IL_02c0: nop
	  IL_02c1: nop
	  IL_02c2: nop
	  IL_02c3: nop
	  IL_02c4: nop
	  IL_02c5: nop
	  IL_02c6: nop
	  IL_02c7: nop
	  IL_02c8: nop
	  IL_02c9: nop
	  IL_02ca: nop
	  IL_02cb: nop
	  IL_02cc: nop
	  IL_02cd: nop
	  IL_02ce: nop
	  IL_02cf: nop
	  IL_02d0: nop
	  IL_02d1: nop
	  IL_02d2: nop
	  IL_02d3: nop
	  IL_02d4: nop
	  IL_02d5: ret

	  IL_02d6: ldloca.s 20
	  IL_02d8: initobj [mscorlib]System.Decimal
	  IL_02de: ldloc.2
	  IL_02df: ldloca.s 20
	  IL_02e1: call bool [mscorlib]System.Decimal::TryParse(string, valuetype [mscorlib]System.Decimal&)
	  IL_02e6: brtrue.s IL_02ed

	  IL_02e8: nop
	  IL_02e9: nop
	  IL_02ea: nop
	  IL_02eb: nop
	  IL_02ec: ret

	  IL_02ed: ldloc.s 20
	  IL_02ef: ldsfld valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Zero
	  IL_02f4: call bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	  IL_02f9: brfalse IL_0374

	  IL_02fe: nop
	  IL_02ff: nop
	  IL_0300: nop
	  IL_0301: nop
	  IL_0302: ret

	  IL_0303: ldloca.s 21
	  IL_0305: initobj [mscorlib]System.Decimal
	  IL_030b: ldloc.2
	  IL_030c: ldloca.s 21
	  IL_030e: call bool [mscorlib]System.Decimal::TryParse(string, valuetype [mscorlib]System.Decimal&)
	  IL_0313: brtrue.s IL_031c

	  IL_0315: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_031a: pop
	  IL_031b: ret

	  IL_031c: ldloc.s 21
	  IL_031e: ldsfld valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Zero
	  IL_0323: call bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	  IL_0328: brfalse.s IL_0330

	  IL_032a: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_032f: pop

	  IL_0330: nop
	  IL_0331: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_0336: pop
	  IL_0337: ret

	  IL_0338: ldloca.s 26
	  IL_033a: initobj [mscorlib]System.Decimal
	  IL_0340: ldloc.2
	  IL_0341: ldloca.s 26
	  IL_0343: call bool [mscorlib]System.Decimal::TryParse(string, valuetype [mscorlib]System.Decimal&)
	  IL_0348: brtrue.s IL_0354

	  IL_034a: nop
	  IL_034b: nop
	  IL_034c: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_0351: pop
	  IL_0352: nop
	  IL_0353: ret

	  IL_0354: ldloc.s 26
	  IL_0356: ldsfld valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::Zero
	  IL_035b: call bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal, valuetype [mscorlib]System.Decimal)
	  IL_0360: brfalse.s IL_0374

	  IL_0362: nop
	  IL_0363: nop
	  IL_0364: nop
	  IL_0365: nop
	  IL_0366: nop
	  IL_0367: newobj instance void [mscorlib]System.Object::.ctor()
	  IL_036c: pop
	  IL_036d: nop
	  IL_036e: nop
	  IL_036f: nop
	  IL_0370: nop
	  IL_0371: nop
	  IL_0372: nop
	  IL_0373: nop

	  IL_0374: ret
  } // end of method FrmSheetPX::dgvItemList_CellValueChanged


} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue2260


.class private auto ansi sealed '<PrivateImplementationDetails>'
	extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	// Methods
	.method assembly hidebysig static 
		uint32 ComputeStringHash (
			string s
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 44 (0x2c)
		.maxstack 2
		.locals init (
			[0] uint32,
			[1] int32
		)

		IL_0000: ldarg.0
		IL_0001: brfalse.s IL_002a

		IL_0003: ldc.i4 -2128831035
		IL_0008: stloc.0
		IL_0009: ldc.i4.0
		IL_000a: stloc.1
		IL_000b: br.s IL_0021
		// loop start (head: IL_0021)
			IL_000d: ldarg.0
			IL_000e: ldloc.1
			IL_000f: callvirt instance char [mscorlib]System.String::get_Chars(int32)
			IL_0014: ldloc.0
			IL_0015: xor
			IL_0016: ldc.i4 16777619
			IL_001b: mul
			IL_001c: stloc.0
			IL_001d: ldloc.1
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: stloc.1

			IL_0021: ldloc.1
			IL_0022: ldarg.0
			IL_0023: callvirt instance int32 [mscorlib]System.String::get_Length()
			IL_0028: blt.s IL_000d
		// end loop

		IL_002a: ldloc.0
		IL_002b: ret
	} // end of method '<PrivateImplementationDetails>'::ComputeStringHash

} // end of class <PrivateImplementationDetails>

