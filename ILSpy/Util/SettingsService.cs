using System;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.ViewModels;
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

		public DisplaySettings DisplaySettings { get; }

		public AssemblyListManager AssemblyListManager { get; }

		public DecompilationOptions CreateDecompilationOptions(TabPageModel tabPage)
		{
			return new(SessionSettings.LanguageSettings.LanguageVersion, DecompilerSettings, DisplaySettings) { Progress = tabPage.Content as IProgress<DecompilationProgress> };
		}
	}
}
