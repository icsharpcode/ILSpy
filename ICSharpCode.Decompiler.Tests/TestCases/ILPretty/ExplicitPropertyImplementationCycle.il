// The MethodImpl row of the getter names the getter itself.
.class public auto ansi beforefieldinit SelfImplementing extends [mscorlib]System.Object
{
	.method public hidebysig specialname newslot virtual instance int32 get_X () cil managed
	{
		.override method instance int32 SelfImplementing::get_X()
		.maxstack 8
		ldc.i4.0
		ret
	} // end of method SelfImplementing::get_X

	.property instance int32 'I.X'()
	{
		.get instance int32 SelfImplementing::get_X()
	} // end of property SelfImplementing::'I.X'
}

// The MethodImpl rows of the two getters name each other.
.class interface public auto ansi abstract beforefieldinit IFirst
{
	.method public hidebysig specialname newslot virtual instance int32 get_X () cil managed
	{
		.override method instance int32 ISecond::get_X()
		.maxstack 8
		ldc.i4.1
		ret
	} // end of method IFirst::get_X

	.property instance int32 'ISecond.X'()
	{
		.get instance int32 IFirst::get_X()
	} // end of property IFirst::'ISecond.X'
}

.class interface public auto ansi abstract beforefieldinit ISecond
{
	.method public hidebysig specialname newslot virtual instance int32 get_X () cil managed
	{
		.override method instance int32 IFirst::get_X()
		.maxstack 8
		ldc.i4.2
		ret
	} // end of method ISecond::get_X

	.property instance int32 'IFirst.X'()
	{
		.get instance int32 ISecond::get_X()
	} // end of property ISecond::'IFirst.X'
}
