
namespace ICSharpCode.ILSpy.Commands
{
	public class SetThemeCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			if (parameter is string theme)
			{
				var snapshot = SettingsService.Instance.CreateSnapshot();
				var sessionSettings = snapshot.GetSettings<SessionSettings>();

				sessionSettings.Theme = theme;

				snapshot.Save();
			}
		}
	}
}
