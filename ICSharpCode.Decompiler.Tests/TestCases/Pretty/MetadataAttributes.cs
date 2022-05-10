using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class MetadataAttributes
	{
		private class MethodImplAttr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public extern void A();
#if NETCORE
			[MethodImpl(MethodImplOptions.AggressiveOptimization)]
			public extern void B();
#endif
			[MethodImpl(MethodImplOptions.ForwardRef)]
			public extern void D();
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void E();
			[MethodImpl(MethodImplOptions.NoInlining)]
			public extern void F();
			[MethodImpl(MethodImplOptions.NoOptimization)]
			public extern void G();
			[PreserveSig]
			public extern void H();
			[MethodImpl(MethodImplOptions.Synchronized)]
			public extern void I();
			[MethodImpl(MethodImplOptions.Unmanaged)]
			public extern void J();
			[MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.Native)]
			public extern void A1();
#if NETCORE
			[MethodImpl(MethodImplOptions.AggressiveOptimization, MethodCodeType = MethodCodeType.Native)]
			public extern void B1();
#endif
			[MethodImpl(MethodImplOptions.ForwardRef, MethodCodeType = MethodCodeType.Native)]
			public extern void D1();
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
			public extern void E1();
			[MethodImpl(MethodImplOptions.NoInlining, MethodCodeType = MethodCodeType.Native)]
			public extern void F1();
			[MethodImpl(MethodImplOptions.NoOptimization, MethodCodeType = MethodCodeType.Native)]
			public extern void G1();
			[MethodImpl(MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
			public extern void H1();
			[MethodImpl(MethodImplOptions.Synchronized, MethodCodeType = MethodCodeType.Native)]
			public extern void I1();
			[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
			public extern void J1();
			[MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void A2();
#if NETCORE
			[MethodImpl(MethodImplOptions.AggressiveOptimization, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void B2();
#endif
			[MethodImpl(MethodImplOptions.ForwardRef, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void D2();
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void E2();
			[MethodImpl(MethodImplOptions.NoInlining, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void F2();
			[MethodImpl(MethodImplOptions.NoOptimization, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void G2();
			[MethodImpl(MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void H2();
			[MethodImpl(MethodImplOptions.Synchronized, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void I2();
			[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void J2();
			[MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.OPTIL)]
			public extern void A3();
#if NETCORE
			[MethodImpl(MethodImplOptions.AggressiveOptimization, MethodCodeType = MethodCodeType.Runtime)]
			public extern void B3();
#endif
			[MethodImpl(MethodImplOptions.ForwardRef, MethodCodeType = MethodCodeType.Runtime)]
			public extern void D3();
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			public extern void E3();
			[MethodImpl(MethodImplOptions.NoInlining, MethodCodeType = MethodCodeType.Runtime)]
			public extern void F3();
			[MethodImpl(MethodImplOptions.NoOptimization, MethodCodeType = MethodCodeType.Runtime)]
			public extern void G3();
			[MethodImpl(MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Runtime)]
			public extern void H3();
			[MethodImpl(MethodImplOptions.Synchronized, MethodCodeType = MethodCodeType.Runtime)]
			public extern void I3();
			[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Runtime)]
			public extern void J3();
		}
	}
}
