using System;
#if EXPECTED_OUTPUT
using System.Runtime.CompilerServices;
#endif
namespace Issue3465
{
	internal class Program
	{
		private static Program programNull;

		private static Program GetProgram()
		{
			return null;
		}

		private static bool Test3465()
		{
			Program program = GetProgram();
			return System.Runtime.CompilerServices.Unsafe.As<Program, UIntPtr>(ref program) > System.Runtime.CompilerServices.Unsafe.As<Program, UIntPtr>(ref programNull);
		}
	}
}