.assembly extern mscorlib
{
	.publickeytoken = ( b7 7a 5c 56 19 34 e0 89 )
	.ver 4:0:0:0
}

.assembly 'Jmp'
{
	.ver 0:0:0:0
}

.module Jmp.exe
.corflags 0x00000001 // ILOnly

.class private auto ansi abstract sealed beforefieldinit Program
	extends [mscorlib]System.Object
{
	.method public hidebysig static void Main (string[] args) cil managed 
	{
		.maxstack 8
		.entrypoint

		ldstr "Method1(100) = {0}"
		ldc.i4 100
		call int32 Program::Method1(int32)
		box int32
		call void [mscorlib]System.Console::WriteLine(string, object)
		ret
	} // end of method Main
	
  .method public static int32 Method1(int32 val)
	{
    ldarg.0
    ldc.i4.1
    add
    starg.s 0
    jmp int32 Program::Method2(int32)
  }
	
	.method public static int32 Method2(int32 val)
	{
		ldarg.0
		ldc.i4.5
		mul
		ret
	}
}
