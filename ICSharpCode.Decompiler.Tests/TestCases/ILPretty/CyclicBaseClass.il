.assembly extern mscorlib
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly CyclicBaseClass
{
	.hash algorithm 0x00008004
	.ver 1:0:0:0
}

.module CyclicBaseClass.dll

.class public auto ansi abstract beforefieldinit CyclicBaseClass.SelfDerived
	extends CyclicBaseClass.SelfDerived
{
}

.class public auto ansi abstract beforefieldinit CyclicBaseClass.Expanding`1<T>
	extends class CyclicBaseClass.Expanding`1<class CyclicBaseClass.Expanding`1<!T>>
{
}

.class public auto ansi abstract sealed beforefieldinit CyclicBaseClass.Extensions
	extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = (
		01 00 00 00
	)
	.method public hidebysig static
		void Deconstruct (
			class CyclicBaseClass.SelfDerived 'value',
			[out] int32& a,
			[out] int32& b
		) cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 8
		ldarg.1
		ldc.i4.1
		stind.i4
		ldarg.2
		ldc.i4.2
		stind.i4
		ret
	}

	.method public hidebysig static
		void Deconstruct<T> (
			class CyclicBaseClass.Expanding`1<!!T> 'value',
			[out] int32& a,
			[out] int32& b
		) cil managed
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = (
			01 00 00 00
		)
		.maxstack 8
		ldarg.1
		ldc.i4.1
		stind.i4
		ldarg.2
		ldc.i4.2
		stind.i4
		ret
	}
}

.class public auto ansi abstract beforefieldinit CyclicBaseClass.Pair`1<T>
	extends [mscorlib]System.Object
{
	.method public hidebysig newslot abstract virtual
		instance void Deconstruct (
			[out] int32& first,
			[out] !T& second
		) cil managed
	{
	}

	.method family hidebysig specialname rtspecialname
		instance void .ctor () cil managed
	{
		.maxstack 8
		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}
}

.class public auto ansi abstract sealed beforefieldinit CyclicBaseClass.NestedDeconstruction
	extends [mscorlib]System.Object
{
	.method public hidebysig static
		int32 Sum (
			class CyclicBaseClass.Pair`1<class CyclicBaseClass.SelfDerived> pair
		) cil managed
	{
		.maxstack 3
		.locals init (
			[0] int32,
			[1] int32,
			[2] int32,
			[3] class CyclicBaseClass.SelfDerived,
			[4] int32,
			[5] int32
		)
		ldarg.0
		ldloca.s 2
		ldloca.s 3
		callvirt instance void class CyclicBaseClass.Pair`1<class CyclicBaseClass.SelfDerived>::Deconstruct(int32&, !0&)
		ldloc.3
		ldloca.s 4
		ldloca.s 5
		call void CyclicBaseClass.Extensions::Deconstruct(class CyclicBaseClass.SelfDerived, int32&, int32&)
		ldloc.2
		ldloc.s 4
		stloc.0
		ldloc.s 5
		stloc.1
		ldloc.0
		add
		ldloc.1
		add
		ret
	}

	.method public hidebysig static
		int32 Sum (
			class CyclicBaseClass.Pair`1<class CyclicBaseClass.Expanding`1<int32>> pair
		) cil managed
	{
		.maxstack 3
		.locals init (
			[0] int32,
			[1] int32,
			[2] int32,
			[3] class CyclicBaseClass.Expanding`1<int32>,
			[4] int32,
			[5] int32
		)
		ldarg.0
		ldloca.s 2
		ldloca.s 3
		callvirt instance void class CyclicBaseClass.Pair`1<class CyclicBaseClass.Expanding`1<int32>>::Deconstruct(int32&, !0&)
		ldloc.3
		ldloca.s 4
		ldloca.s 5
		call void CyclicBaseClass.Extensions::Deconstruct<int32>(class CyclicBaseClass.Expanding`1<!!0>, int32&, int32&)
		ldloc.2
		ldloc.s 4
		stloc.0
		ldloc.s 5
		stloc.1
		ldloc.0
		add
		ldloc.1
		add
		ret
	}
}
