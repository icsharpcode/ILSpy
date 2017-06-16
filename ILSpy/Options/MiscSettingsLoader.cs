using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Options
{
	public interface IMiscSettingsLoader
	{
		MiscSettings LoadMiscSettings(ILSpySettings settings);
	}

	public class MiscSettingsLoader : IMiscSettingsLoader
	{
		public MiscSettings LoadMiscSettings(ILSpySettings settings)
		{
			XElement e = settings["MiscSettings"];
			MiscSettings s = new MiscSettings();
			s.AllowMultipleInstances = (bool?)e.Attribute("allowMultipleInstance") ?? s.AllowMultipleInstances;
			return s;
		}
	}

	public static class MiscSettingsInstance
	{
		private static MiscSettingsLoader current;

		public static MiscSettingsLoader Current
		{
			get
			{
				current = current ?? new MiscSettingsLoader();
				return current;
			}
		}
	}
}
