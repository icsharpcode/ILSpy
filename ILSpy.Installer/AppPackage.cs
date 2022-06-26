using System;

namespace ILSpy.Installer
{
	internal static class AppPackage
	{
		public static Version Version = new(DecompilerVersionInfo.Major + "." + DecompilerVersionInfo.Minor + "." + DecompilerVersionInfo.Build + "." + DecompilerVersionInfo.Revision);
	}
}
