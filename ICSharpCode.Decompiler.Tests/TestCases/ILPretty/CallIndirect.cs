using System;
internal class CallIndirect
{
	private unsafe void Test(IntPtr f)
	{
		((delegate* unmanaged[Stdcall]<int, void>)f)(42);
	}
}