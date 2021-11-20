using System;
using System.IO;
using System.Linq;

using WixSharp;

namespace ILSpy.Installer
{
	internal class Builder
	{
		static public void Main()
		{
			Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

			var project = new Project("ILSpy",
							  new InstallDir(@"%LocalAppData%\Programs\ILSpy",
								  new DirFiles(@"ILSpy\bin\Release\net472\*.dll"),
								  new DirFiles(@"ILSpy\bin\Release\net472\*.exe"),
								  new DirFiles(@"ILSpy\bin\Release\net472\*.config"),
								  new Files(@"ILSpy\bin\Release\net472\ILSpy.resources.dll"),
								  new Files(@"ILSpy\bin\Release\net472\ILSpy.ReadyToRun.Plugin.resources.dll")));

			project.GUID = new Guid("a12fdab1-731b-4a98-9749-d481ce8692ab");
			project.Version = AppPackage.Version;
			project.SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory);
			project.InstallScope = InstallScope.perUser;
			project.InstallPrivileges = InstallPrivileges.limited;
			project.ControlPanelInfo.ProductIcon = @"..\ILSpy\Images\ILSpy.ico";

			project.MajorUpgrade = new MajorUpgrade {
				Schedule = UpgradeSchedule.afterInstallInitialize,
				AllowSameVersionUpgrades = true,
				DowngradeErrorMessage = "A newer release of ILSpy is already installed on this system. Please uninstall it first to continue."
			};

			project.UI = WUI.WixUI_ProgressOnly;

			project.ResolveWildCards().FindFile(f => f.Name.EndsWith("ILSpy.exe")).First()
				.Shortcuts = new[] {
					new FileShortcut("ILSpy", @"%ProgramMenu%")
				};

			Compiler.BuildMsi(project, Path.Combine(Environment.CurrentDirectory, "output", "ILSpy.msi"));
		}
	}
}
