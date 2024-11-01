
using System.Composition;

namespace ICSharpCode.ILSpy.Commands
{
	[Export]
	[Shared]
	public class SetThemeCommand(SettingsService settingsService) : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			if (parameter is string theme)
			{
				settingsService.SessionSettings.Theme = theme;
			}
		}
	}
}
