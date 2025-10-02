using System;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue3571_B
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct fsResult
	{
		public static fsResult Success => default(fsResult);
		public static fsResult Failure => default(fsResult);
		public bool Succeeded => true;
		public bool Failed => false;
		public static fsResult operator +(fsResult a, fsResult b)
		{
			return default(fsResult);
		}
	}

	internal static class Issue3571_B
	{
		public static fsResult M()
		{
			fsResult success = fsResult.Success;
			fsResult fsResult2 = success + fsResult.Success;
			if (fsResult2.Succeeded)
			{
				return success;
			}
			Console.WriteLine("Failed");
			return fsResult2 + fsResult.Failure;
		}
	}
}
