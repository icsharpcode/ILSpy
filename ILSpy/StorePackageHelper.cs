using OSVersionHelper;

namespace ICSharpCode.ILSpy
{
	// The Store package is ever only built for net472
	public static class StorePackageHelper
	{
		public static bool HasPackageIdentity {
			get {
#if NET472
				return WindowsVersionHelper.HasPackageIdentity;
#else
				return false;
#endif
			}
		}
		public static string GetPackageFamilyName()
		{
#if NET472
			return WindowsVersionHelper.GetPackageFamilyName();
#else
			return "";
#endif
		}
	}
}
