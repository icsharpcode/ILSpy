.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}

.assembly 'ConvTest'
{
	.ver 0:0:0:0
}

.module ConvTest.exe
// MVID: {987E1A15-519A-400C-B879-759CFB7F990B}
.corflags 0x00000001 // ILOnly

.class private auto ansi abstract sealed beforefieldinit Program
	extends [mscorlib]System.Object
{
	.method public hidebysig static void Print (native int) cil managed {
		ldarg.0
		box [mscorlib]System.IntPtr
		call void [mscorlib]System.Console::WriteLine(object)
		ret
	}

	.method public hidebysig static void Main (string[] args) cil managed {
		.entrypoint
		.try {
			ldc.i4.0
			call void Program::Print(native int)
			ldc.i8 7595444518666557374
			conv.i
			call void Program::Print(native int)
			leave.s eof
		} catch [mscorlib]System.Exception {
			callvirt instance string [mscorlib]System.Object::ToString()
			call void [mscorlib]System.Console::WriteLine(object)
			leave.s eof
		}
		eof: ret
	}
}
