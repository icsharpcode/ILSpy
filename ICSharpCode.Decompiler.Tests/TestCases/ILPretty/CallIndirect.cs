using System;
internal class CallIndirect
{
	private unsafe void Test(IntPtr f)
	{
		((delegate* unmanaged[Stdcall]<int, void>)f)(42);
	}

	private unsafe void UnmanagedDefaultCall(IntPtr f)
	{
		((delegate* unmanaged<int, void>)f)(42);
	}

	private unsafe void CustomCall(IntPtr f)
	{
		((delegate* unmanaged[Custom]<int, void>)f)(42);
	}

	private unsafe void MultipleCustomCall(IntPtr f)
	{
		((delegate* unmanaged[SuppressGCTransition, Custom]<int, void>)f)(42);
	}
}
