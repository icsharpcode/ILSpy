// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Composition bridge: supplies the live settings instance and the persistence callback
	/// used by the shared AI selection service. Selection changes are written to disk
	/// immediately, independently of the app-exit settings flush.
	/// </summary>
	[Export(typeof(AISelectionHost))]
	[Shared]
	public sealed class AISelectionSettingsHost : AISelectionHost
	{
		readonly SettingsService settingsService;

		[ImportingConstructor]
		public AISelectionSettingsHost(SettingsService settingsService)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
		}

		public override AISettingsModel Settings => settingsService.AISettings;

		public override Func<Task> PersistAsync => () => {
			settingsService.Save();
			return Task.CompletedTask;
		};
	}
}
