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

#if ARM64
			var buildPlatform = "arm64";
#else
			var buildPlatform = "x64";
#endif
			var buildOutputDir = $@"ILSpy\bin\{buildConfiguration}\net10.0\win-{buildPlatform}\publish\fwdependent";

			var project = new Project("ILSpy",
							  new InstallDir(@"%LocalAppData%\Programs\ILSpy",
								  new DirFiles(Path.Combine(buildOutputDir, "*.dll")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.exe")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.config")),
								  new DirFiles(Path.Combine(buildOutputDir, "*.json")),
								  new Files(Path.Combine(buildOutputDir, "ILSpy.resources.dll")),
								  new Files(Path.Combine(buildOutputDir, "ILSpy.ReadyToRun.Plugin.resources.dll"))));

			// https://github.com/oleg-shilo/wixsharp/issues/1900 in addition to the pipeline dotnet tool install command
			WixExtension.UI.PreferredVersion = "6.0.2";

#if ARM64
			project.Platform = Platform.arm64;
#else
			project.Platform = Platform.x64;
#endif

			project.GUID = new Guid("a12fdab1-731b-4a98-9749-d481ce8692ab");
			project.Version = AppPackage.Version;
			project.SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory);
			project.Scope = InstallScope.perUser;
			project.ControlPanelInfo.ProductIcon = @"..\ILSpy\Assets\ILSpy.ico";
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

			Compiler.BuildMsi(project, Path.Combine(Environment.CurrentDirectory, "wix", $"ILSpy-{AppPackage.Version}-{buildPlatform}.msi"));
		}
	}
}
