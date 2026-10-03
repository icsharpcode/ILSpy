.class public auto ansi beforefieldinit MyClass extends [mscorlib]System.Object
{
	.field private object _MyValue

	.method public hidebysig specialname rtspecialname instance void .ctor () cil managed
	{
		.maxstack 8
		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	} // end of method MyClass::.ctor

	.method private specialname newslot strict virtual instance object get_MyValue () cil managed
	{
		.maxstack 8
		ldarg.0
		ldfld object MyClass::_MyValue
		ret
	} // end of method MyClass::get_MyValue

	.method private specialname newslot strict virtual instance void set_MyValue (object 'value') cil managed synchronized
	{
		.maxstack 8
		ldarg.0
		ldarg.1
		stfld object MyClass::_MyValue
		ret
	} // end of method MyClass::set_MyValue

	.property instance object MyValue()
	{
		.get instance object MyClass::get_MyValue()
		.set instance void MyClass::set_MyValue(object)
	} // end of property MyClass::MyValue
}
