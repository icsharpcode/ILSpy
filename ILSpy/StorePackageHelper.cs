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
#endif
				return false;
			}
		}
		public static string GetPackageFamilyName()
		{
#if NET472
			return WindowsVersionHelper.GetPackageFamilyName();
#endif
			return "";
		}
	}
}
