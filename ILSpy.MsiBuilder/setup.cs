using System;
using System.IO;

using WixSharp;

namespace ILSpy.MsiBuilder
{
	internal class MsiBuilder
	{
		static public void Main()
		{
			Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

			var project = new Project("ILSpy",
							  new Dir(@"%LocalAppData%\Programs\ILSpy",
								  new DirFiles(@"ILSpy\bin\Release\net472\*.dll"),
								  new DirFiles(@"ILSpy\bin\Release\net472\*.exe"),
								  new DirFiles(@"ILSpy\bin\Release\net472\*.config"),
								  new Files(@"ILSpy\bin\Release\net472\ILSpy.resources.dll"),
								  new Files(@"ILSpy\bin\Release\net472\ILSpy.ReadyToRun.Plugin.resources.dll")));

			project.GUID = new Guid("a12fdab1-731b-4a98-9749-d481ce8692ab");
			project.Version = new Version("1.0.0.0");
			project.SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory);
			project.InstallScope = InstallScope.perUser;
			project.InstallPrivileges = InstallPrivileges.limited;

			project.UI = WUI.WixUI_Minimal;

			Compiler.BuildMsi(project, Path.Combine(Environment.CurrentDirectory, "output", "ILSpy.msi"));
		}
	}
}
