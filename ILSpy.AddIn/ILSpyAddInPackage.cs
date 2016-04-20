using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Reflection;
using System.IO;
using Mono.Cecil;

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
				// Create the command for the menu item.
				CommandID menuCommandID = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenReferenceInILSpy);
				MenuCommand menuItem = new MenuCommand(OpenReferenceInILSpyCallback, menuCommandID);
				mcs.AddCommand(menuItem);

				// Create the command for the menu item.
				CommandID menuCommandID2 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenProjectOutputInILSpy);
				MenuCommand menuItem2 = new MenuCommand(OpenProjectOutputInILSpyCallback, menuCommandID2);
				mcs.AddCommand(menuItem2);

				// Create the command for the menu item.
				CommandID menuCommandID3 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenCodeItemInILSpy);
				MenuCommand menuItem3 = new MenuCommand(OpenCodeItemInILSpyCallback, menuCommandID3);
				mcs.AddCommand(menuItem3);

				// Create the command for the menu item.
				CommandID menuCommandID4 = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)PkgCmdIDList.cmdidOpenILSpy);
				MenuCommand menuItem4 = new MenuCommand(OpenILSpyCallback, menuCommandID4);
				mcs.AddCommand(menuItem4);
			}
		}
		#endregion

		/// <summary>
		/// This function is the callback used to execute a command when the a menu item is clicked.
		/// See the Initialize method to see how the menu item is associated to this function using
		/// the OleMenuCommandService service and the MenuCommand class.
		/// </summary>
		private void OpenReferenceInILSpyCallback(object sender, EventArgs e)
		{
			var explorer = ((EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE))).ToolWindows.SolutionExplorer;
			var items =(object[]) explorer.SelectedItems;

			foreach (EnvDTE.UIHierarchyItem item in items) {
				dynamic reference = item.Object;
				string path = null;
				if (reference.PublicKeyToken != "") {
					var token = Utils.HexStringToBytes(reference.PublicKeyToken);
					path = GacInterop.FindAssemblyInNetGac(new AssemblyNameReference(reference.Identity, new Version(reference.Version)) { PublicKeyToken = token });
				}
				if (path == null)
					path = reference.Path;
				OpenAssemblyInILSpy(path);
			}
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
			string navigateTo = "/navigateTo:" + GetCodeElementIDString(codeElement);
			OpenProjectInILSpy(codeElement.ProjectItem.ContainingProject, navigateTo);
		}

		// Get ID string for code element, for /navigateTo command line option.
		// See "ID string format" in Appendix A of the C# language specification for details.
		// See ICSharpCode.ILSpy.XmlDoc.XmlDocKeyProvider.GetKey for a similar implementation, based on Mono.Cecil.MemberReference.
		private string GetCodeElementIDString(EnvDTE.CodeElement codeElement)
		{
			switch (codeElement.Kind) {
				case EnvDTE.vsCMElement.vsCMElementEvent:
					return string.Concat("E:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementVariable:
					return string.Concat("F:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementFunction: {
						var codeFunction = (EnvDTE80.CodeFunction2)codeElement;

						var idBuilder = new System.Text.StringBuilder();
						idBuilder.Append("M:");

						// Constructors need to be called "#ctor" for navigation purposes.
						string[] genericClassTypeParameters;
						string classFullName = GetCodeElementContainerString((EnvDTE.CodeElement)codeFunction.Parent, out genericClassTypeParameters);
						string functionName = (codeFunction.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionConstructor ? "#ctor" : codeFunction.Name);
						idBuilder.Append(classFullName);
						idBuilder.Append('.');
						idBuilder.Append(functionName);

						// Get type parameters to generic method, if present.
						string[] genericMethodTypeParameters = new string[0];
						int iGenericParams = codeFunction.FullName.LastIndexOf('<');
						if ((codeFunction.IsGeneric) && (iGenericParams >= 0)) {
							genericMethodTypeParameters = codeFunction.FullName.Substring(iGenericParams).Split(new char[] {'<', ',', ' ', '>'}, StringSplitOptions.RemoveEmptyEntries);
							idBuilder.Append("``");
							idBuilder.Append(genericMethodTypeParameters.Length);
						}

						// Append parameter types, to disambiguate overloaded methods.
						if (codeFunction.Parameters.Count > 0) {
							idBuilder.Append("(");
							bool first = true;
							foreach (EnvDTE.CodeParameter parameter in codeFunction.Parameters) {
								if (!first) {
									idBuilder.Append(",");
								}
								first = false;
								int genericClassTypeParameterIndex = Array.IndexOf(genericClassTypeParameters, parameter.Type.AsFullName);
								if (genericClassTypeParameterIndex >= 0) {
									idBuilder.Append('`');
									idBuilder.Append(genericClassTypeParameterIndex);
								}
								else {
									int genericMethodTypeParameterIndex = Array.IndexOf(genericMethodTypeParameters, parameter.Type.AsFullName);
									if (genericMethodTypeParameterIndex >= 0) {
										idBuilder.Append("``");
										idBuilder.Append(genericMethodTypeParameterIndex);
									}
									else {
										idBuilder.Append(parameter.Type.AsFullName);
									}
								}
							}
							idBuilder.Append(")");
						}
						return idBuilder.ToString();
					}

				case EnvDTE.vsCMElement.vsCMElementNamespace:
					return string.Concat("N:",
						codeElement.FullName);

				case EnvDTE.vsCMElement.vsCMElementProperty:
					return string.Concat("P:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementDelegate:
				case EnvDTE.vsCMElement.vsCMElementEnum:
				case EnvDTE.vsCMElement.vsCMElementInterface:
				case EnvDTE.vsCMElement.vsCMElementStruct:
				case EnvDTE.vsCMElement.vsCMElementClass:
						return string.Concat("T:",
							GetCodeElementContainerString(codeElement));

				default:
					return string.Format("!:Code element {0} is of unsupported type {1}", codeElement.FullName, codeElement.Kind.ToString());
			}
		}

		private string GetCodeElementContainerString(EnvDTE.CodeElement containerElement)
		{
			string[] genericTypeParametersIgnored;
			return GetCodeElementContainerString(containerElement, out genericTypeParametersIgnored);
		}

		private string GetCodeElementContainerString(EnvDTE.CodeElement containerElement, out string[] genericTypeParameters)
		{
			genericTypeParameters = new string[0];

			switch (containerElement.Kind) {
				case EnvDTE.vsCMElement.vsCMElementNamespace:
					return containerElement.FullName;

				case EnvDTE.vsCMElement.vsCMElementInterface:
				case EnvDTE.vsCMElement.vsCMElementStruct:
				case EnvDTE.vsCMElement.vsCMElementClass: {
						var idBuilder = new System.Text.StringBuilder();
						idBuilder.Append(GetCodeElementContainerString((EnvDTE.CodeElement)containerElement.Collection.Parent));
						idBuilder.Append('.');
						idBuilder.Append(containerElement.Name);

						// For "Generic<T1,T2>" we need "Generic`2".
						bool isGeneric =
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementClass) && ((EnvDTE80.CodeClass2)containerElement).IsGeneric) ||
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementStruct) && ((EnvDTE80.CodeStruct2)containerElement).IsGeneric) ||
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementInterface) && ((EnvDTE80.CodeInterface2)containerElement).IsGeneric);
						int iGenericParams = containerElement.FullName.LastIndexOf('<');
						if (isGeneric && (iGenericParams >= 0)) {
							genericTypeParameters = containerElement.FullName.Substring(iGenericParams).Split(new char[] {'<', ',', ' ', '>'}, StringSplitOptions.RemoveEmptyEntries);
							idBuilder.Append('`');
							idBuilder.Append(genericTypeParameters.Length);
						}

						return idBuilder.ToString();
					}

				default:
					return string.Format("!:Code element {0} is of unsupported container type {1}", containerElement.FullName, containerElement.Kind.ToString());
			}
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
			string projectPath = Path.GetDirectoryName(project.FileName);
			string outputPath = config.Properties.Item("OutputPath").Value.ToString();
			string assemblyFileName = project.Properties.Item("OutputFileName").Value.ToString();
			OpenAssemblyInILSpy(Path.Combine(projectPath, outputPath, assemblyFileName), arguments);
		}

		private void OpenAssemblyInILSpy(string assemblyFileName, params string[] arguments)
		{
			if (!File.Exists(assemblyFileName)) {
				ShowMessage("Could not find assembly '{0}', please ensure the project and all references were built correctly!", assemblyFileName);
				return;
			}

			string commandLineArguments = Utils.ArgumentArrayToCommandLine(assemblyFileName);
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