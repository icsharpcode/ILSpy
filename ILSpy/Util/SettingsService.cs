using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Util
{
	public class SettingsService
	{
		public static readonly SettingsService Instance = new();

		private SettingsService()
		{
			SpySettings = ILSpySettings.Load();
			SessionSettings = new(SpySettings);
			DecompilerSettings = DecompilerSettingsPanel.LoadDecompilerSettings(SpySettings);
			DisplaySettings = DisplaySettingsPanel.LoadDisplaySettings(SpySettings, SessionSettings);
			AssemblyListManager = new(SpySettings) {
				ApplyWinRTProjections = DecompilerSettings.ApplyWindowsRuntimeProjections,
				UseDebugSymbols = DecompilerSettings.UseDebugSymbols
			};
		}

		public ILSpySettings SpySettings { get; }

		public SessionSettings SessionSettings { get; }

		public DecompilerSettings DecompilerSettings { get; set; }

		public DisplaySettingsViewModel DisplaySettings { get; }

		public AssemblyListManager AssemblyListManager { get; }
	}
}
