.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly LateBaseConstructorCall
{
	.hash algorithm 0x00008004
	.ver 0:0:0:0
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LateBaseConstructorCall
	extends [System.Runtime]System.Object
{
	.method private hidebysig static void Initialize() cil managed
	{
		.maxstack 8
		IL_0000: ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor() cil managed
	{
		.maxstack 8
		IL_0000: call void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LateBaseConstructorCall::Initialize()
		IL_0005: ldarg.0
		IL_0006: call instance void [System.Runtime]System.Object::.ctor()
		IL_000b: ret
	}
}

.class public auto ansi sealed ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledRole
	extends [System.Runtime]System.Enum
{
	.field public specialname rtspecialname int32 value__
	.field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledRole Assistant = int32(2)
}

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledPatch
	extends [System.Runtime]System.ValueType
{
	.field public int32 Value
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource
	extends [System.Runtime]System.Object
{
	.field public string Content
	.field public string Refusal
	.field public object Output
	.field public string Function
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledConstructorInitializer
	extends [System.Runtime]System.Object
{
	.method private hidebysig specialname rtspecialname instance void .ctor(
		valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledRole role,
		string content,
		[in] valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledPatch& patch,
		string refusal,
		string participantName,
		object output,
		object tools,
		string function
	) cil managed
	{
		.param [3]
			.custom instance void [System.Runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (01 00 00 00)
		.maxstack 8
		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor(
		class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource source
	) cil managed
	{
		.maxstack 9
		.locals init (
			[0] string function,
			[1] valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledPatch patch
		)

		IL_0000: ldarg.0
		IL_0001: ldc.i4.2
		IL_0002: ldarg.1
		IL_0003: brtrue.s IL_0008
		IL_0005: ldnull
		IL_0006: br.s IL_000e
		IL_0008: ldarg.1
		IL_0009: ldfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource::Content
		IL_000e: ldloca.s patch
		IL_0010: dup
		IL_0011: initobj ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledPatch
		IL_0017: ldarg.1
		IL_0018: brtrue.s IL_001d
		IL_001a: ldnull
		IL_001b: br.s IL_0023
		IL_001d: ldarg.1
		IL_001e: ldfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource::Refusal
		IL_0023: ldnull
		IL_0024: ldarg.1
		IL_0025: brtrue.s IL_002a
		IL_0027: ldnull
		IL_0028: br.s IL_0030
		IL_002a: ldarg.1
		IL_002b: ldfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource::Function
		IL_0030: stloc.0
		IL_0031: ldarg.1
		IL_0032: brtrue.s IL_0037
		IL_0034: ldnull
		IL_0035: br.s IL_003d
		IL_0037: ldarg.1
		IL_0038: ldfld object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledArgumentSource::Output
		IL_003d: brtrue.s IL_0042
		IL_003f: ldnull
		IL_0040: br.s IL_0047
		IL_0042: newobj instance void [System.Runtime]System.Object::.ctor()
		IL_0047: ldnull
		IL_0048: ldloc.0
		IL_0049: call instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledConstructorInitializer::.ctor(
			valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledRole,
			string,
			valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpilledPatch&,
			string,
			string,
			object,
			object,
			string
		)
		IL_004e: ret
	}
}
