using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public sealed class Handle
	{
		private readonly Func<int> _func;
		public Handle(Func<int> func)
		{
			_func = func;
		}
	}

	public static class NoLocalFunctions
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass1_0
		{
			public int x;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{
			public int x;

			internal int _003CSimpleCaptureWithRef_003Eg__F_007C0()
			{
					return 42 + x;
			}
		}

		private static void UseLocalFunctionReference()
		{
#if OPT
			new Handle(new Func<int>(_003CUseLocalFunctionReference_003Eg__F_007C0_0));
#else
			Handle handle = new Handle(new Func<int>(_003CUseLocalFunctionReference_003Eg__F_007C0_0));
#endif
		}

		private static void SimpleCapture()
		{
			_003C_003Ec__DisplayClass1_0 A_ = default(_003C_003Ec__DisplayClass1_0);
			A_.x = 1;
			_003CSimpleCapture_003Eg__F_007C1_0(ref A_);
		}

		private static void SimpleCaptureWithRef()
		{
			_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
			_003C_003Ec__DisplayClass2_.x = 1;
#if OPT
			new Handle(new Func<int>(_003C_003Ec__DisplayClass2_._003CSimpleCaptureWithRef_003Eg__F_007C0));
#else
			Handle handle = new Handle(new Func<int>(_003C_003Ec__DisplayClass2_._003CSimpleCaptureWithRef_003Eg__F_007C0));
#endif
		}

		[CompilerGenerated]
		internal static int _003CUseLocalFunctionReference_003Eg__F_007C0_0()
		{
			return 42;
		}

		[CompilerGenerated]
		private static int _003CSimpleCapture_003Eg__F_007C1_0(ref _003C_003Ec__DisplayClass1_0 A_0)
		{
			return 42 + A_0.x;
		}
	}
}
