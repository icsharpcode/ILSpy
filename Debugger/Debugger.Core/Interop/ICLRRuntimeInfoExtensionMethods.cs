// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Interop.ICLRRuntimeInfo
{
	public static partial class ICLRRuntimeInfoExtensionMethods
	{
		public static string GetVersion(this ICLRRuntimeInfo clrRuntimeInfo)
		{
			var stringBuilder = new System.Text.StringBuilder(20);
			var capacity = (uint)stringBuilder.Capacity;
			clrRuntimeInfo.GetVersionString(stringBuilder, capacity);
			return stringBuilder.ToString();
		}
	}
}
