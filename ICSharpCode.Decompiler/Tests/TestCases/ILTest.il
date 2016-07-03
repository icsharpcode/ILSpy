// C:\Users\Siegfried\Documents\SharpDevelop Projects\HelloWorld\HelloWorld\bin\Debug\HelloWorld.exe

.assembly extern mscorlib
{
	.publickeytoken = ( b7 7a 5c 56 19 34 e0 89 )
	.ver 4:0:0:0
}
.assembly HelloWorld
{
	.ver 0:0:0:0
}

.module HelloWorld.exe
.corflags 0x00000001 // ILOnly


.class private auto ansi abstract sealed beforefieldinit Program
	extends [mscorlib]System.Object
{
	.method public hidebysig static void Main (string[] args) cil managed 
	{
		.maxstack 8
		.entrypoint

		ldstr "Hello World!"
		call void [mscorlib]System.Console::WriteLine(string)
		ret
	} // end of method Main
} // end of class <Module>
