using System;
using System.Runtime.InteropServices;
internal class EvalOrder
{
	private SimpleStruct field;

	public static void Test(EvalOrder p)
	{
		// ldflda (and potential NRE) before MyStruct ctor call
		ref SimpleStruct reference = ref p.field;
		reference = new SimpleStruct(1);
	}
}
[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct SimpleStruct
{
	public SimpleStruct(int val)
	{
		Console.WriteLine(val);
	}
}
