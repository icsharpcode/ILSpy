// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Debugger.Interop.ICLRRuntimeInfo
{
	[ComImport, Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ICLRRuntimeInfo
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetVersionString([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzBuffer, [In, Out] ref uint pcchBuffer);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetRuntimeDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzBuffer, [In, Out] ref uint pcchBuffer);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int IsLoaded([In] IntPtr hndProcess);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), LCIDConversion(3)]
		void LoadErrorString([In] uint iResourceID, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzBuffer, [In, Out] ref uint pcchBuffer);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IntPtr LoadLibrary([In, MarshalAs(UnmanagedType.LPWStr)] string pwzDllName);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		IntPtr GetProcAddress([In, MarshalAs(UnmanagedType.LPStr)] string pszProcName);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetInterface([In] ref Guid rclsid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out Object ppObj);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int IsLoadable();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void SetDefaultStartupFlags([In] uint dwStartupFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string pwzHostConfigFile);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void GetDefaultStartupFlags(out uint pdwStartupFlags, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzHostConfigFile, [In, Out] ref uint pcchHostConfigFile);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void BindAsLegacyV2Runtime();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void IsStarted(out int pbStarted, out uint pdwStartupFlags);
	}
}