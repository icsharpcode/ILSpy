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
			DecompilerSettings = ISettingsProvider.LoadDecompilerSettings(SpySettings);
			DisplaySettings = DisplaySettingsPanel.LoadDisplaySettings(SpySettings, SessionSettings);
			MiscSettings = MiscSettings.Load(SpySettings);
			AssemblyListManager = new(SpySettings) {
				ApplyWinRTProjections = DecompilerSettings.ApplyWindowsRuntimeProjections,
				UseDebugSymbols = DecompilerSettings.UseDebugSymbols
			};
		}

		public ILSpySettings SpySettings { get; }

		public SessionSettings SessionSettings { get; }

		public DecompilerSettings DecompilerSettings { get; set; }

		public DisplaySettings DisplaySettings { get; }

		public MiscSettings MiscSettings { get; set; }

		public AssemblyListManager AssemblyListManager { get; }

		public DecompilationOptions CreateDecompilationOptions(TabPageModel tabPage)
		{
			return new(SessionSettings.LanguageSettings.LanguageVersion, DecompilerSettings, DisplaySettings) { Progress = tabPage.Content as IProgress<DecompilationProgress> };
		}

		public AssemblyList LoadInitialAssemblyList()
		{
			var loadPreviousAssemblies = MiscSettings.LoadPreviousAssemblies;

			if (loadPreviousAssemblies)
			{
				return AssemblyListManager.LoadList(SessionSettings.ActiveAssemblyList);
			}
			else
			{
				AssemblyListManager.ClearAll();
				return AssemblyListManager.CreateList(AssemblyListManager.DefaultListName);
			}
		}
	}
}
