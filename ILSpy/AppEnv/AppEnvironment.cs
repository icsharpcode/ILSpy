using System.Runtime.InteropServices;

namespace ICSharpCode.ILSpy.AppEnv
{
	public static class AppEnvironment
	{
		public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	}
}
