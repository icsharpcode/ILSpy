// Property rows without any MethodSemantics accessors (corrupt or hand-written
// metadata). The type system skips such properties, whether parameterized or not;
// decompilation must not crash on them.
.class public auto ansi beforefieldinit BrokenProperties extends [mscorlib]System.Object
{
	.field private int32 _field

	.property instance int32 Parameterized(int32)
	{
	} // end of property BrokenProperties::Parameterized

	.property instance int32 Plain()
	{
	} // end of property BrokenProperties::Plain

	.method public hidebysig instance int32 Touch (int32 i) cil managed
	{
		.maxstack 8
		ldarg.0
		ldfld int32 BrokenProperties::_field
		ldarg.1
		add
		ret
	} // end of method BrokenProperties::Touch

	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8
		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	} // end of method BrokenProperties::.ctor
}
