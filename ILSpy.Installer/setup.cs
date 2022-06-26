using System;
using System.IO;
using System.Linq;
using System.Text;

using WixSharp;
using WixSharp.Controls;

namespace ILSpy.Installer
{
	internal class Builder
	{
		static public void Main()
		{
			Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

#if DEBUG
			var buildConfiguration = "Debug";
#else
			var buildConfiguration = "Release";
#endif
			var buildOutputDir = $@"ILSpy\bin\{buildConfiguration}\net6.0-windows";

			var project = new Project("ILSpy",
							  new InstallDir(@"%LocalAppData%\Programs\ILSpy",
								  new DirFiles(Path.Combine(buildOutputDir, "*.dll")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.exe")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.config")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.json")),
								  new Files(Path.Combine(buildOutputDir, "ILSpy.resources.dll")),
								  new Files(Path.Combine(buildOutputDir, "ILSpy.ReadyToRun.Plugin.resources.dll"))))
				{
					GUID = new Guid("a12fdab1-731b-4a98-9749-d481ce8692ab"),
					Version = AppPackage.Version,
					SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory),
					InstallScope = InstallScope.perUser,
					InstallPrivileges = InstallPrivileges.limited,
					ControlPanelInfo = {
						ProductIcon = @"..\ILSpy\Images\ILSpy.ico",
						Manufacturer = "ICSharpCode Team"
					},
					LocalizationFile = Path.Combine(Environment.CurrentDirectory, "winui.wxl"),
					Encoding = Encoding.UTF8,
					MajorUpgrade = new() {
						Schedule = UpgradeSchedule.afterInstallInitialize,
						AllowSameVersionUpgrades = true,
						DowngradeErrorMessage = "A newer release of ILSpy is already installed on this system. Please uninstall it first to continue."
					},
					UI = WUI.WixUI_InstallDir,
					CustomUI = new DialogSequence()
						.On(NativeDialogs.WelcomeDlg, Buttons.Next,
							new ShowDialog(NativeDialogs.VerifyReadyDlg))
						.On(NativeDialogs.VerifyReadyDlg, Buttons.Back,
							new ShowDialog(NativeDialogs.WelcomeDlg))
				};

			project.ResolveWildCards().FindFile(f => f.Name.EndsWith("ILSpy.exe")).First()
				.Shortcuts = new[] {
					new FileShortcut("ILSpy", @"%ProgramMenu%")
				};

			Compiler.BuildMsi(project, Path.Combine(Environment.CurrentDirectory, "wix", $"ILSpy-{AppPackage.Version}.msi"));
		}
	}
}
