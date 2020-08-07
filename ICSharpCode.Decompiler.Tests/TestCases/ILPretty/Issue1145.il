.class public auto ansi sealed EvType extends [mscorlib]System.MulticastDelegate
{
	// Methods not included. Just the run of the mill ctor + {Begin|End}?Invoke 
}

.class public auto ansi serializable beforefieldinit OwningClass 
{

.field class EvType EvName

.event EvType EvName
{
	.addon instance void OwningClass::add_EvName(class EvType)
	.removeon instance void OwningClass::remove_EvName(class EvType)
}

.method public hidebysig specialname 
	instance void add_EvName (
		class EvType 'value'
	) cil managed 
{
	.maxstack 3
	.locals init (
		[0] class EvType,
		[1] class EvType
	)

	IL_0000: ldarg.0
	IL_0001: ldfld class EvType OwningClass::EvName
	IL_0006: stloc.0
	// loop start (head: IL_0007)
		IL_0007: ldloc.0
		IL_0008: stloc.1
		IL_0009: ldarg.0
		IL_000a: ldflda class EvType OwningClass::EvName
		IL_000f: ldloc.1
		IL_0010: ldarg.1
		IL_0011: call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
		IL_0016: castclass EvType
		IL_001b: ldloc.0
		IL_001c: call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class EvType>(!!0&, !!0, !!0)
		IL_0021: stloc.0
		IL_0022: ldloc.0
		IL_0023: ldloc.1
		IL_0024: bne.un IL_0007
	// end loop
	IL_0029: ret
} // end of method OwningClass::add_EvName

.method public hidebysig specialname 
	instance void remove_EvName (
		class EvType 'value'
	) cil managed 
{
	.maxstack 3
	.locals init (
		[0] class EvType,
		[1] class EvType
	)

	IL_0000: ldarg.0
	IL_0001: ldfld class EvType OwningClass::EvName
	IL_0006: stloc.0
	// loop start (head: IL_0007)
		IL_0007: ldloc.0
		IL_0008: stloc.1
		IL_0009: ldarg.0
		IL_000a: ldflda class EvType OwningClass::EvName
		IL_000f: ldloc.1
		IL_0010: ldarg.1
		IL_0011: call class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate, class [mscorlib]System.Delegate)
		IL_0016: castclass EvType
		IL_001b: ldloc.0
		IL_001c: call !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class EvType>(!!0&, !!0, !!0)
		IL_0021: stloc.0
		IL_0022: ldloc.0
		IL_0023: ldloc.1
		IL_0024: bne.un IL_0007
	// end loop
	IL_0029: ret
} // end of method OwningClass::remove_EvName

} // end of class OwningClass