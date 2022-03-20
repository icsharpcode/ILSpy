public static class DecompilerVersionInfo
{
	public const string Major = "8";
	public const string Minor = "0";
	public const string Build = "0";
	public const string Revision = "$INSERTREVISION$";
	public const string VersionName = "alpha1";

	public const string FullVersion = Major + "." + Minor + "." + Build + ".$INSERTREVISION$$INSERTBRANCHPOSTFIX$$INSERTVERSIONNAMEPOSTFIX$";
	public const string FullVersionWithShortCommitHash = FullVersion + "-$INSERTSHORTCOMMITHASH$";
}
