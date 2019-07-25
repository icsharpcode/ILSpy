.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Issue684
{
  .ver 1:0:0:0
}
.module Issue684.exe

.class public auto ansi abstract sealed Issue684
	extends [mscorlib]System.Object
{
	// Methods
	
  .method static privatescope 
	  int32 Main$PST06000001 (
		  string[] ''
	  ) cil managed 
  {
	  // Method begins at RVA 0x2050
	  // Code size 196 (0xc4)
	  .maxstack 11
	  .entrypoint
	  .locals init (
		  [0] int32,
		  [1] int32,
		  [2] int32,
		  [3] int32[],
		  [4] int32
	  )

	  IL_0000: ldc.i4 1000
	  IL_0005: newarr [mscorlib]System.Int32
	  IL_000a: stloc.3
	  IL_000b: call string [mscorlib]System.Console::ReadLine()
	  IL_0010: call int32 [mscorlib]System.Int32::Parse(string)
	  IL_0015: stloc.2
	  IL_0016: ldloc.2
	  IL_0017: ldc.i4 1000
	  IL_001c: clt
	  IL_001e: ldc.i4.0
	  IL_001f: ceq
	  IL_0021: dup
	  IL_0022: brtrue IL_0030

	  IL_0027: pop
	  IL_0028: ldloc.2
	  IL_0029: ldc.i4 2
	  IL_002e: clt

	  IL_0030: brfalse IL_0045

	  IL_0035: ldc.i4 1
	  IL_003a: neg
	  IL_003b: call void [mscorlib]System.Console::WriteLine(int32)
	  IL_0040: br IL_00c2

	  IL_0045: ldc.i4 2
	  IL_004a: stloc.0
	  IL_004b: ldc.i4 2
	  IL_0050: stloc.1
	  // loop start (head: IL_0051)
		  IL_0051: ldloc.1
		  IL_0052: ldloc.2
		  IL_0053: cgt
		  IL_0055: ldc.i4.0
		  IL_0056: ceq
		  IL_0058: brfalse IL_00c2

		  IL_005d: ldloc.1
		  IL_005e: call void [mscorlib]System.Console::WriteLine(int32)
		  // loop start (head: IL_0063)
			  IL_0063: ldloc.0
			  IL_0064: ldloc.2
			  IL_0065: cgt
			  IL_0067: ldc.i4.0
			  IL_0068: ceq
			  IL_006a: brfalse IL_0088

			  IL_006f: ldc.i4 1
			  IL_0074: stloc.s 4
			  IL_0076: ldloc.3
			  IL_0077: ldloc.0
			  IL_0078: ldloc.s 4
			  IL_007a: stelem.any [mscorlib]System.Int32
			  IL_007f: ldloc.0
			  IL_0080: ldloc.1
			  IL_0081: add
			  IL_0082: stloc.0
			  IL_0083: br IL_0063
		  // end loop

		  IL_0088: ldloc.1
		  IL_0089: stloc.0
		  // loop start (head: IL_008a)
			  IL_008a: ldloc.0
			  IL_008b: ldloc.2
			  IL_008c: cgt
			  IL_008e: ldc.i4.0
			  IL_008f: ceq
			  IL_0091: dup
			  IL_0092: brfalse IL_00a9

			  IL_0097: pop
			  IL_0098: ldloc.3
			  IL_0099: ldloc.0
			  IL_009a: ldelem.any [mscorlib]System.Int32
			  IL_009f: ldc.i4 0
			  IL_00a4: ceq
			  IL_00a6: ldc.i4.0
			  IL_00a7: ceq

			  IL_00a9: brfalse IL_00bb

			  IL_00ae: ldloc.0
			  IL_00af: ldc.i4 1
			  IL_00b4: add
			  IL_00b5: stloc.0
			  IL_00b6: br IL_008a
		  // end loop

		  IL_00bb: ldloc.0
		  IL_00bc: stloc.1
		  IL_00bd: br IL_0051
	  // end loop

	  IL_00c2: ldc.i4.0
	  IL_00c3: ret
  } // end of method Program::Main

} // end of class Issue684
