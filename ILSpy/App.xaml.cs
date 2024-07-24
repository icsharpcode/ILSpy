﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Settings;

using Medo.Application;

using Microsoft.VisualStudio.Composition;

using TomsToolbox.Wpf.Styles;
using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		internal static CommandLineArguments CommandLineArguments;
		internal static readonly IList<ExceptionData> StartupExceptions = new List<ExceptionData>();

		public static ExportProvider ExportProvider { get; private set; }
		public static IExportProviderFactory ExportProviderFactory { get; private set; }

		internal class ExceptionData
		{
			public Exception Exception;
			public string PluginName;
		}

		public App()
		{
			ILSpySettings.SettingsFilePathProvider = new ILSpySettingsFilePathProvider();

			var cmdArgs = Environment.GetCommandLineArgs().Skip(1);
			App.CommandLineArguments = CommandLineArguments.Create(cmdArgs);

			bool forceSingleInstance = (App.CommandLineArguments.SingleInstance ?? true)
				&& !MiscSettingsPanel.CurrentMiscSettings.AllowMultipleInstances;
			if (forceSingleInstance)
			{
				SingleInstance.Attach();  // will auto-exit for second instance
				SingleInstance.NewInstanceDetected += SingleInstance_NewInstanceDetected;
			}

			SharpTreeNode.SetImagesProvider(new WpfWindowsTreeNodeImagesProvider());

			InitializeComponent();

			Resources.RegisterDefaultStyles();

			if (!System.Diagnostics.Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;
				Dispatcher.CurrentDispatcher.UnhandledException += Dispatcher_UnhandledException;
			}
			TaskScheduler.UnobservedTaskException += DotNet40_UnobservedTaskException;
			InitializeMef().GetAwaiter().GetResult();
			Languages.Initialize(ExportProvider);
			EventManager.RegisterClassHandler(typeof(Window),
											  Hyperlink.RequestNavigateEvent,
											  new RequestNavigateEventHandler(Window_RequestNavigate));
			ILSpyTraceListener.Install();

			if (App.CommandLineArguments.ArgumentsParser.IsShowingInformation)
			{
				MessageBox.Show(App.CommandLineArguments.ArgumentsParser.GetHelpText(), "ILSpy Command Line Arguments");
			}

			if (App.CommandLineArguments.ArgumentsParser.RemainingArguments.Any())
			{
				string unknownArguments = string.Join(", ", App.CommandLineArguments.ArgumentsParser.RemainingArguments);
				MessageBox.Show(unknownArguments, "ILSpy Unknown Command Line Arguments Passed");
			}
		}

		private static void SingleInstance_NewInstanceDetected(object sender, NewInstanceEventArgs e)
		{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			ICSharpCode.ILSpy.MainWindow.Instance.HandleSingleInstanceCommandLineArguments(e.Args);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		static Assembly ResolvePluginDependencies(AssemblyLoadContext context, AssemblyName assemblyName)
		{
			var rootPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
			var assemblyFileName = Path.Combine(rootPath, assemblyName.Name + ".dll");
			if (!File.Exists(assemblyFileName))
				return null;
			return context.LoadFromAssemblyPath(assemblyFileName);
		}

		private static async Task InitializeMef()
		{
			// Add custom logic for resolution of dependencies.
			// This necessary because the AssemblyLoadContext.LoadFromAssemblyPath and related methods,
			// do not automatically load dependencies.
			AssemblyLoadContext.Default.Resolving += ResolvePluginDependencies;
			// Cannot show MessageBox here, because WPF would crash with a XamlParseException
			// Remember and show exceptions in text output, once MainWindow is properly initialized
			try
			{
				// Set up VS MEF. For now, only do MEF1 part discovery, since that was in use before.
				// To support both MEF1 and MEF2 parts, just change this to:
				// var discovery = PartDiscovery.Combine(new AttributedPartDiscoveryV1(Resolver.DefaultInstance),
				//                                       new AttributedPartDiscovery(Resolver.DefaultInstance));
				var discovery = new AttributedPartDiscoveryV1(Resolver.DefaultInstance);
				var catalog = ComposableCatalog.Create(Resolver.DefaultInstance);
				var pluginDir = Path.GetDirectoryName(typeof(App).Module.FullyQualifiedName);
				if (pluginDir != null)
				{
					foreach (var plugin in Directory.GetFiles(pluginDir, "*.Plugin.dll"))
					{
						var name = Path.GetFileNameWithoutExtension(plugin);
						try
						{
							var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(plugin);
							var parts = await discovery.CreatePartsAsync(asm);
							catalog = catalog.AddParts(parts);
						}
						catch (Exception ex)
						{
							StartupExceptions.Add(new ExceptionData { Exception = ex, PluginName = name });
						}
					}
				}
				// Add the built-in parts: First, from ILSpyX
				var xParts = await discovery.CreatePartsAsync(typeof(IAnalyzer).Assembly);
				catalog = catalog.AddParts(xParts);
				// Then from ILSpy itself
				var createdParts = await discovery.CreatePartsAsync(Assembly.GetExecutingAssembly());
				catalog = catalog.AddParts(createdParts);

				// If/When the project switches to .NET Standard/Core, this will be needed to allow metadata interfaces (as opposed
				// to metadata classes). When running on .NET Framework, it's automatic.
				//   catalog.WithDesktopSupport();
				// If/When any part needs to import ICompositionService, this will be needed:
				//   catalog.WithCompositionService();
				var config = CompositionConfiguration.Create(catalog);
				ExportProviderFactory = config.CreateExportProviderFactory();
				ExportProvider = ExportProviderFactory.CreateExportProvider();
				// This throws exceptions for composition failures. Alternatively, the configuration's CompositionErrors property
				// could be used to log the errors directly. Used at the end so that it does not prevent the export provider setup.
				config.ThrowOnErrors();
			}
			catch (CompositionFailedException ex) when (ex.InnerException is AggregateException agex)
			{
				foreach (var inner in agex.InnerExceptions)
				{
					StartupExceptions.Add(new ExceptionData { Exception = inner });
				}
			}
			catch (Exception ex)
			{
				StartupExceptions.Add(new ExceptionData { Exception = ex });
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var output = new StringBuilder();
			if (ILSpy.MainWindow.FormatExceptions(StartupExceptions.ToArray(), output))
			{
				MessageBox.Show(output.ToString(), "Sorry we crashed!");
				Environment.Exit(1);
			}
			base.OnStartup(e);
		}

		void DotNet40_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			// On .NET 4.0, an unobserved exception in a task terminates the process unless we mark it as observed
			e.SetObserved();
		}

		#region Exception Handling
		static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			UnhandledException(e.Exception);
			e.Handled = true;
		}

		static void ShowErrorBox(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				UnhandledException(ex);
			}
		}

		[ThreadStatic]
		static bool showingError;

		internal static void UnhandledException(Exception exception)
		{
			Debug.WriteLine(exception.ToString());
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				ReflectionTypeLoadException rtle = ex as ReflectionTypeLoadException;
				if (rtle != null && rtle.LoaderExceptions.Length > 0)
				{
					exception = rtle.LoaderExceptions[0];
					Debug.WriteLine(exception.ToString());
					break;
				}
			}
			if (showingError)
			{
				// Ignore re-entrant calls
				// We run the risk of opening an infinite number of exception dialogs.
				return;
			}
			showingError = true;
			try
			{
				MessageBox.Show(exception.ToString(), "Sorry, we crashed");
			}
			finally
			{
				showingError = false;
			}
		}
		#endregion

		void Window_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			ILSpy.MainWindow.Instance.NavigateTo(e);
		}
	}
}