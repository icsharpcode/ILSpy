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
								  new Files(Path.Combine(buildOutputDir, "ILSpy.resources.dll")),
								  new Files(Path.Combine(buildOutputDir, "ILSpy.ReadyToRun.Plugin.resources.dll"))));

			project.GUID = new Guid("a12fdab1-731b-4a98-9749-d481ce8692ab");
			project.Version = AppPackage.Version;
			project.SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory);
			project.InstallScope = InstallScope.perUser;
			project.InstallPrivileges = InstallPrivileges.limited;
			project.ControlPanelInfo.ProductIcon = @"..\ILSpy\Images\ILSpy.ico";
			project.ControlPanelInfo.Manufacturer = "ICSharpCode Team";
			project.LocalizationFile = Path.Combine(Environment.CurrentDirectory, "winui.wxl");
			project.Encoding = Encoding.UTF8;

			project.MajorUpgrade = new MajorUpgrade {
				Schedule = UpgradeSchedule.afterInstallInitialize,
				AllowSameVersionUpgrades = true,
				DowngradeErrorMessage = "A newer release of ILSpy is already installed on this system. Please uninstall it first to continue."
			};

			project.UI = WUI.WixUI_InstallDir;
			project.CustomUI =
				new DialogSequence()
					.On(NativeDialogs.WelcomeDlg, Buttons.Next,
						new ShowDialog(NativeDialogs.VerifyReadyDlg))
					.On(NativeDialogs.VerifyReadyDlg, Buttons.Back,
						new ShowDialog(NativeDialogs.WelcomeDlg));

			project.ResolveWildCards().FindFile(f => f.Name.EndsWith("ILSpy.exe")).First()
				.Shortcuts = new[] {
					new FileShortcut("ILSpy", @"%ProgramMenu%")
				};

			Compiler.BuildMsi(project, Path.Combine(Environment.CurrentDirectory, "wix", $"ILSpy-{AppPackage.Version}.msi"));
		}
	}
}
