using System;
internal class CallIndirect
{
	private unsafe void Test(IntPtr f)
	{
		((delegate* stdcall<int, void>)f)(42);
	}
}