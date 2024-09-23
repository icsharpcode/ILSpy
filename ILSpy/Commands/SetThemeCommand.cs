
namespace ICSharpCode.ILSpy.Commands
{
	public class SetThemeCommand : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			if (parameter is string theme)
			{
				SettingsService.Instance.SessionSettings.Theme = theme;
			}
		}
	}
}
