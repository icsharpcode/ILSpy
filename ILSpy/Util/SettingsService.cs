using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Util
{
	internal class SettingsService
	{
		public static readonly SettingsService Instance = new SettingsService();

		private SettingsService()
		{
			this.SpySettings = ILSpySettings.Load();

			SessionSettings = new(this.SpySettings);
		}

		public ILSpySettings SpySettings { get; }

		public SessionSettings SessionSettings { get; }
	}
}
