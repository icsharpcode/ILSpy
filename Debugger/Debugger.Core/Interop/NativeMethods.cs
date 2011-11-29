// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Debugger.Interop
{        
	public static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
		public static extern Debugger.Interop.CorDebug.ICorDebug CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetCORVersion([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName, Int32 cchBuffer, out Int32 dwLength);
		
		[DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
		public static extern int GetRequestedRuntimeVersion(string exeFilename, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pVersion, Int32 cchBuffer, out Int32 dwLength);
		
		public static Guid CLSID_CLRMetaHost = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");
		public static Guid IID_ICLRMetaHost = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");
		[DllImport("mscoree.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		public static extern ICLRMetaHost.ICLRMetaHost CLRCreateInstance(ref Guid clsid, ref Guid riid);
	}
}

#pragma warning restore 1591
