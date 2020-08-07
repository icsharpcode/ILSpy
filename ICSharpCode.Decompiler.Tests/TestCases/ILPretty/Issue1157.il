.assembly Issue1157
{
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module Issue1157.exe
// MVID: {4C6C7F98-AEB2-4A19-BE6F-43171E6113F1}
.corflags 0x00020003 // ILOnly, Required32Bit, Preferred32Bit

.class private auto ansi abstract beforefieldinit Issue1157.BaseClass
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig specialname newslot abstract virtual 
		instance void add_IDontKnowMeHereOut (
			class [mscorlib]System.EventHandler 'value'
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
	} // end of method BaseClass::add_IDontKnowMeHereOut

	.method public hidebysig specialname newslot abstract virtual 
		instance void remove_IDontKnowMeHereOut (
			class [mscorlib]System.EventHandler 'value'
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
	} // end of method BaseClass::remove_IDontKnowMeHereOut

	.method family hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2063
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method BaseClass::.ctor

	// Events
	.event [mscorlib]System.EventHandler IDontKnowMeHereOut
	{
		.addon instance void Issue1157.BaseClass::add_IDontKnowMeHereOut(class [mscorlib]System.EventHandler)
		.removeon instance void Issue1157.BaseClass::remove_IDontKnowMeHereOut(class [mscorlib]System.EventHandler)
	}


} // end of class Issue1157.BaseClass

.class private auto ansi beforefieldinit Issue1157.OtherClass
	extends Issue1157.BaseClass
{
	// Fields
	.field private class [mscorlib]System.EventHandler IDontKnowMeHereOut
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = (
		01 00 00 00 00 00 00 00
	)

	// Methods
	.method public hidebysig specialname virtual 
		instance void add_IDontKnowMeHereOut (
			class [mscorlib]System.EventHandler 'value'
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x206c
		// Code size 41 (0x29)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.EventHandler,
			[1] class [mscorlib]System.EventHandler,
			[2] class [mscorlib]System.EventHandler
		)

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.EventHandler Issue1157.OtherClass::IDontKnowMeHereOut
		IL_0006: stloc.0
		// loop start (head: IL_0007)
			IL_0007: ldloc.0
			IL_0008: stloc.1
			IL_0009: ldloc.1
			IL_000a: ldarg.1
			IL_000b: call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
			IL_0010: castclass [mscorlib]System.EventHandler
			IL_0015: stloc.2
			IL_0016: ldarg.0
			IL_0017: ldflda class [mscorlib]System.EventHandler Issue1157.OtherClass::IDontKnowMeHereOut
			IL_001c: ldloc.2
			IL_001d: ldloc.1
			IL_001e: call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&, !!0, !!0)
			IL_0023: stloc.0
			IL_0024: ldloc.0
			IL_0025: ldloc.1
			IL_0026: bne.un.s IL_0007
		// end loop
		IL_0028: ret
	} // end of method OtherClass::add_IDontKnowMeHereOut

	.method public hidebysig specialname virtual 
		instance void remove_IDontKnowMeHereOut (
			class [mscorlib]System.EventHandler 'value'
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x20a4
		// Code size 41 (0x29)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.EventHandler,
			[1] class [mscorlib]System.EventHandler,
			[2] class [mscorlib]System.EventHandler
		)

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.EventHandler Issue1157.OtherClass::IDontKnowMeHereOut
		IL_0006: stloc.0
		// loop start (head: IL_0007)
			IL_0007: ldloc.0
			IL_0008: stloc.1
			IL_0009: ldloc.1
			IL_000a: ldarg.1
			IL_000b: call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
			IL_0010: castclass [mscorlib]System.EventHandler
			IL_0015: stloc.2
			IL_0016: ldarg.0
			IL_0017: ldflda class [mscorlib]System.EventHandler Issue1157.OtherClass::IDontKnowMeHereOut
			IL_001c: ldloc.2
			IL_001d: ldloc.1
			IL_001e: call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&, !!0, !!0)
			IL_0023: stloc.0
			IL_0024: ldloc.0
			IL_0025: ldloc.1
			IL_0026: bne.un.s IL_0007
		// end loop
		IL_0028: ret
	} // end of method OtherClass::remove_IDontKnowMeHereOut

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x20d9
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void Issue1157.BaseClass::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method OtherClass::.ctor

	// Events
	.event [mscorlib]System.EventHandler IDontKnowMeHereOut
	{
		.addon instance void Issue1157.OtherClass::add_IDontKnowMeHereOut(class [mscorlib]System.EventHandler)
		.removeon instance void Issue1157.OtherClass::remove_IDontKnowMeHereOut(class [mscorlib]System.EventHandler)
	}

} // end of class Issue1157.OtherClass