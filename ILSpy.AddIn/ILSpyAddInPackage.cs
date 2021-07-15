using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

using EnvDTE;

using ICSharpCode.ILSpy.AddIn.Commands;

using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Task = System.Threading.Tasks.Task;

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
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	// This attribute is used to register the information needed to show this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(GuidList.guidILSpyAddInPkgString)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
	[ProvideBindingPath]
	public sealed class ILSpyAddInPackage : AsyncPackage
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

		OleMenuCommandService menuService;
		public OleMenuCommandService MenuService => menuService;

		VisualStudioWorkspace workspace;
		public VisualStudioWorkspace Workspace => workspace;

		public EnvDTE80.DTE2 DTE => (EnvDTE80.DTE2)GetGlobalService(typeof(EnvDTE.DTE));


		/////////////////////////////////////////////////////////////////////////////
		// Overridden Package Implementation
		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			Debug.WriteLine($"Entering {nameof(InitializeAsync)}() of: {this}");

			await base.InitializeAsync(cancellationToken, progress);

			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();

			var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));
			Assumes.Present(componentModel);

			// Add our command handlers for menu (commands must exist in the .vsct file)
			this.menuService = (OleMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService));
			Assumes.Present(menuService);

			this.workspace = componentModel.GetService<VisualStudioWorkspace>();
			Assumes.Present(workspace);

			OpenILSpyCommand.Register(this);
			OpenProjectOutputCommand.Register(this);
			OpenReferenceCommand.Register(this, PkgCmdIDList.cmdidOpenReferenceInILSpy);
			OpenReferenceCommand.Register(this, PkgCmdIDList.cmdidOpenPackageReferenceInILSpy);
			OpenReferenceCommand.Register(this, PkgCmdIDList.cmdidOpenProjectReferenceInILSpy);
			OpenCodeItemCommand.Register(this);
		}
		#endregion

		public void ShowMessage(string format, params object[] items)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			ShowMessage(OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_INFO, format, items);
		}

		public void ShowMessage(OLEMSGICON icon, string format, params object[] items)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			ShowMessage(OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, icon, format, items);
		}

		public int ShowMessage(OLEMSGBUTTON buttons, OLEMSGDEFBUTTON defaultButton, OLEMSGICON icon, string format, params object[] items)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
			if (uiShell == null)
			{
				return 0;
			}

			Guid clsid = Guid.Empty;
			int result;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
				uiShell.ShowMessageBox(
					0,
					ref clsid,
					"ILSpy AddIn",
					string.Format(CultureInfo.CurrentCulture, format, items),
					string.Empty,
					0,
					buttons,
					defaultButton,
					icon,
					0,        // false
					out result
				)
			);

			return result;
		}

		public IEnumerable<T> GetSelectedItemsData<T>()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (DTE.ToolWindows.SolutionExplorer.SelectedItems is IEnumerable<UIHierarchyItem> hierarchyItems)
			{
				foreach (var item in hierarchyItems)
				{
					if (item.Object is T typedItem)
					{
						yield return typedItem;
					}
				}
			}
		}
	}
}