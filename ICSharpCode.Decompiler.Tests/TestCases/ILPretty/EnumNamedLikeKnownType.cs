#if EXPECTED_OUTPUT
using System.Runtime.InteropServices;

namespace System
{
	[StructLayout(LayoutKind.Auto)]
	public struct Int32 : Enum
	{
		public int value__;
	}

	[StructLayout(LayoutKind.Auto)]
	public struct String : Enum
	{
		public string value__;
	}
}
#endif
