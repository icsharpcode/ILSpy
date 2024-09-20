namespace ICSharpCode.ILSpy.Util
{
	internal static class ResourceHelper
	{
		internal static string GetString(string key)
		{
			if (string.IsNullOrEmpty(key))
				return null;

			string value = Properties.Resources.ResourceManager.GetString(key);

			return !string.IsNullOrEmpty(value) ? value : key;
		}
	}
}
