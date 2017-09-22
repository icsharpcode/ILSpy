using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.ILSpy.AddIn
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
	// a package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	// This attribute is used to register the information needed to show this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(GuidList.guidILSpyAddInPkgString)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
	public sealed class ILSpyAddInPackage : Package
	{
		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public ILSpyAddInPackage()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
		}



		/////////////////////////////////////////////////////////////////////////////
		// Overridden Package Implementation
		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs) {
				// Create the command for the References context menu.
				CommandID menuCommandID = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenReferenceInILSpy);
				OleMenuCommand menuItem = new OleMenuCommand(OpenReferenceInILSpyCallback, menuCommandID);
				menuItem.BeforeQueryStatus += OpenReferenceInILSpyCallback_BeforeQueryStatus;
				mcs.AddCommand(menuItem);

				// Create the command for the Project context menu, to open the output assembly.
				CommandID menuCommandID2 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenProjectOutputInILSpy);
				MenuCommand menuItem2 = new MenuCommand(OpenProjectOutputInILSpyCallback, menuCommandID2);
				mcs.AddCommand(menuItem2);

				// Create the command for the code window context menu.
				CommandID menuCommandID3 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenCodeItemInILSpy);
				OleMenuCommand menuItem3 = new OleMenuCommand(OpenCodeItemInILSpyCallback, menuCommandID3);
				menuItem3.BeforeQueryStatus += OpenCodeItemInILSpyCallback_BeforeQueryStatus;
				mcs.AddCommand(menuItem3);

				// Create the command for the Tools menu item.
				CommandID menuCommandID4 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenILSpy);
				MenuCommand menuItem4 = new MenuCommand(OpenILSpyCallback, menuCommandID4);
				mcs.AddCommand(menuItem4);
			}
		}
		#endregion

		string[] SupportedItems = new[] {
			"Microsoft.VisualStudio.ProjectSystem.VS.Implementation.Package.Automation.OAProjectItem",
			"Microsoft.VisualStudio.ProjectSystem.VS.Implementation.Package.Automation.OAReferenceItem"
		};

		private void OpenReferenceInILSpyCallback_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand command = (OleMenuCommand)sender;
			var explorer = ((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ToolWindows.SolutionExplorer;
			var items = (object[])explorer.SelectedItems;
			if (!items.OfType<EnvDTE.UIHierarchyItem>().Any()) {
				command.Visible = false;
			} else {
				command.Visible = items.OfType<EnvDTE.UIHierarchyItem>().All(i => SupportedItems.Contains(i.Object.GetType().FullName) && HasProperties(((dynamic)i.Object).Properties, "Type", "Version", "ResolvedPath"));
			}
		}

		private void OpenReferenceInILSpyCallback(object sender, EventArgs e)
		{
			var explorer = ((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ToolWindows.SolutionExplorer;
			var items = (object[])explorer.SelectedItems;

			foreach (EnvDTE.UIHierarchyItem item in items) {
				var reference = GetReference(item.Object);
				if (reference == null) {
					ShowMessage("No reference information was found in the selection!");
					continue;
				}
				string path = null;
				if (!string.IsNullOrEmpty(reference.PublicKeyToken)) {
					var token = Utils.HexStringToBytes(reference.PublicKeyToken);
					path = GacInterop.FindAssemblyInNetGac(new AssemblyNameReference(reference.Name, reference.Version) { PublicKeyToken = token });
				}
				if (path == null)
					path = reference.Path;
				OpenAssembliesInILSpy(new[] { path });
			}
		}

		class ReferenceInfo
		{
			public string Name { get; set; }
			public string PublicKeyToken { get; set; }
			public string Path { get; set; }
			public Version Version { get; set; }

			internal static ReferenceInfo FromFullName(string fullName)
			{
				string[] parts = fullName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				return new ReferenceInfo {
					Name = parts[0],
					Version = new Version(parts[1].Substring("Version=".Length)),
					PublicKeyToken = parts[3].Substring("PublicKeyToken=".Length)
				};
			}
		}

		private ReferenceInfo GetReference(object o)
		{
			var projectItem = (EnvDTE.ProjectItem)o;
			string referenceType = o.GetType().FullName;
			string[] values;

			switch (referenceType) {
				case "Microsoft.VisualStudio.ProjectSystem.VS.Implementation.Package.Automation.OAReferenceItem":
				case "Microsoft.VisualStudio.ProjectSystem.VS.Implementation.Package.Automation.OAProjectItem":
					values = GetProperties(projectItem.Properties, "Type", "FusionName", "ResolvedPath");
					if (values[0] == "Package") {
						values = GetProperties(projectItem.Properties, "Name", "Version", "Path");
						if (values[0] != null && values[1] != null && values[2] != null) {
							return new ReferenceInfo {
								Name = values[0],
								Path = $"{values[2]}\\{values[0]}.{values[1]}.nupkg"
							};
						}
					} else if (values[2] != null) {
						return new ReferenceInfo { Path = values[2] };
					} else if (!string.IsNullOrWhiteSpace(values[1])) {
						return ReferenceInfo.FromFullName(values[1]);
					}
					return null;
				default:
					dynamic obj = o;

					// C++
					if (referenceType.StartsWith("Microsoft.VisualStudio.PlatformUI", StringComparison.Ordinal)) {
						return new ReferenceInfo { Path = projectItem.Name };
					}

					// F#
					if (referenceType.StartsWith("Microsoft.VisualStudio.FSharp", StringComparison.Ordinal)) {
						o = projectItem.Object;
					}

					// C# and VB
					return new ReferenceInfo {
						Name = obj.Identity,
						PublicKeyToken = obj.PublicKeyToken,
						Path = obj.Path,
						Version = new Version(obj.Version)
					};
			}
		}

		private string[] GetProperties(EnvDTE.Properties properties, params string[] names)
		{
			string[] values = new string[names.Length];
			foreach (dynamic p in properties) {
				try {
					ShowMessage("Name: " + p.Name + ", Value: " + p.Value);
					for (int i = 0; i < names.Length; i++) {
						if (names[i] == p.Name) {
							values[i] = p.Value;
							break;
						}
					}
				} catch {
					continue;
				}
			}
			return values;
		}

		private object GetPropertyObject(EnvDTE.Properties properties, string name)
		{
			foreach (dynamic p in properties) {
				try {
					if (name == p.Name) {
						return p.Object;
					}
				} catch {
					continue;
				}
			}
			return null;
		}

		private bool HasProperties(EnvDTE.Properties properties, params string[] names)
		{
			return properties.Count > 0 && names.Any(n => HasProperty(properties, n));
		}

		private bool HasProperty(EnvDTE.Properties properties, string name)
		{
			foreach (dynamic p in properties) {
				try {
					if (name == p.Name) {
						return true;
					}
				} catch {
					continue;
				}
			}
			return false;
		}

		private void OpenProjectOutputInILSpyCallback(object sender, EventArgs e)
		{
			var explorer = ((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ToolWindows.SolutionExplorer;
			var items = (object[])explorer.SelectedItems;

			foreach (EnvDTE.UIHierarchyItem item in items) {
				EnvDTE.Project project = (EnvDTE.Project)item.Object;
				OpenProjectInILSpy(project);
			}
		}

		// Called when the menu is popped, determines whether "Open code in ILSpy" option is available.
		private void OpenCodeItemInILSpyCallback_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuItem = sender as OleMenuCommand;
			if (menuItem != null) {
				var document = (EnvDTE.Document)(((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ActiveDocument);
				menuItem.Enabled =
					(document != null) &&
					(document.ProjectItem != null) &&
					(document.ProjectItem.ContainingProject != null) &&
					(document.ProjectItem.ContainingProject.ConfigurationManager != null) &&
					!string.IsNullOrEmpty(document.ProjectItem.ContainingProject.FileName);
			}
		}

		private void OpenCodeItemInILSpyCallback(object sender, EventArgs e)
		{
			var document = (EnvDTE.Document)(((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ActiveDocument);
			var selection = (EnvDTE.TextPoint)((EnvDTE.TextSelection)document.Selection).ActivePoint;

			// Search code elements in desired order, working from innermost to outermost.
			// Should eventually find something, and if not we'll just open the assembly itself.
			var codeElement = GetSelectedCodeElement(selection,
				EnvDTE.vsCMElement.vsCMElementFunction,
				EnvDTE.vsCMElement.vsCMElementEvent,
				EnvDTE.vsCMElement.vsCMElementVariable,		// There is no vsCMElementField, fields are just variables outside of function scope.
				EnvDTE.vsCMElement.vsCMElementProperty,
				EnvDTE.vsCMElement.vsCMElementDelegate,
				EnvDTE.vsCMElement.vsCMElementEnum,
				EnvDTE.vsCMElement.vsCMElementInterface,
				EnvDTE.vsCMElement.vsCMElementStruct,
				EnvDTE.vsCMElement.vsCMElementClass,
				EnvDTE.vsCMElement.vsCMElementNamespace);

			if (codeElement != null) {
				OpenCodeItemInILSpy(codeElement);
			}
			else {
				OpenProjectInILSpy(document.ProjectItem.ContainingProject);
			}
		}

		private EnvDTE.CodeElement GetSelectedCodeElement(EnvDTE.TextPoint selection, params EnvDTE.vsCMElement[] elementTypes)
		{
			foreach (var elementType in elementTypes) {
				var codeElement = selection.CodeElement[elementType];
				if (codeElement != null) {
					return codeElement;
				}
			}

			return null;
		}

		private void OpenCodeItemInILSpy(EnvDTE.CodeElement codeElement)
		{
			string codeElementKey = CodeElementXmlDocKeyProvider.GetKey(codeElement);
			OpenProjectInILSpy(codeElement.ProjectItem.ContainingProject, "/navigateTo:" + codeElementKey);
		}

		private void OpenILSpyCallback(object sender, EventArgs e)
		{
			Process.Start(GetILSpyPath());
		}

		private string GetILSpyPath()
		{
			var basePath = Path.GetDirectoryName(typeof(ILSpyAddInPackage).Assembly.Location);
			return Path.Combine(basePath, "ILSpy.exe");
		}

		private void OpenProjectInILSpy(EnvDTE.Project project, params string[] arguments)
		{
			EnvDTE.Configuration config = project.ConfigurationManager.ActiveConfiguration;
			var outputFiles = config.OutputGroups.OfType<EnvDTE.OutputGroup>()
				.Where(g => g.FileCount > 0 && g.CanonicalName == "Built")
				.SelectMany(g => (object[])g.FileURLs).Select(f => f?.ToString())
				.Where(CheckExtension).Select(f => f.Substring("file:///".Length)).ToArray();
			OpenAssembliesInILSpy(outputFiles, arguments);
		}

		private bool CheckExtension(string fileName)
		{
			switch (Path.GetExtension(fileName).ToLowerInvariant()) {
				case ".exe":
				case ".dll":
					return true;
				default:
					return false;
			}
		}

		private void OpenAssembliesInILSpy(IEnumerable<string> assemblyFileNames, params string[] arguments)
		{
			foreach (string assemblyFileName in assemblyFileNames) {
				if (!File.Exists(assemblyFileName)) {
					ShowMessage("Could not find assembly '{0}', please ensure the project and all references were built correctly!", assemblyFileName);
				}
			}

			string commandLineArguments = Utils.ArgumentArrayToCommandLine(assemblyFileNames.ToArray());
			if (arguments != null) {
				commandLineArguments = string.Concat(commandLineArguments, " ", Utils.ArgumentArrayToCommandLine(arguments));
			}

			Process.Start(GetILSpyPath(), commandLineArguments);
		}

		private void ShowMessage(string format, params object[] items)
		{
			IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			Guid clsid = Guid.Empty;
			int result;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
				uiShell.ShowMessageBox(
					0,
					ref clsid,
					"ILSpy.AddIn",
					string.Format(CultureInfo.CurrentCulture, format, items),
					string.Empty,
					0,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
					OLEMSGICON.OLEMSGICON_INFO,
					0,        // false
					out result
				)
			);
		}
	}
}