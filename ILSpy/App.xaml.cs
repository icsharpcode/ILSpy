// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX.Analyzers;

using Medo.Application;

using TomsToolbox.Wpf.Styles;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Composition;
using TomsToolbox.Wpf.Composition;
using ICSharpCode.ILSpy.Themes;
using System.Globalization;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using TomsToolbox.Composition.MicrosoftExtensions;
using TomsToolbox.Essentials;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		internal static CommandLineArguments CommandLineArguments;
		internal static readonly IList<ExceptionData> StartupExceptions = new List<ExceptionData>();

		public static IExportProvider ExportProvider { get; private set; }

		internal record ExceptionData(Exception Exception)
		{
			public string PluginName { get; init; }
		}

		public App()
		{
			var cmdArgs = Environment.GetCommandLineArgs().Skip(1);
			CommandLineArguments = CommandLineArguments.Create(cmdArgs);

			// This is only a temporary, read only handle to the settings service to access the AllowMultipleInstances setting before DI is initialized.
			// At runtime, you must use the service via DI!
			var settingsService = new SettingsService();

			bool forceSingleInstance = (CommandLineArguments.SingleInstance ?? true)
									   && !settingsService.MiscSettings.AllowMultipleInstances;
			if (forceSingleInstance)
			{
				SingleInstance.Attach();  // will auto-exit for second instance
				SingleInstance.NewInstanceDetected += SingleInstance_NewInstanceDetected;
			}

			InitializeComponent();

			if (!InitializeDependencyInjection(settingsService))
			{
				// There is something completely wrong with DI, probably some service registration is missing => nothing we can do to recover, so stop and shut down.
				Exit += (_, _) => MessageBox.Show(StartupExceptions.FormatExceptions(), "Sorry we crashed!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
				Shutdown(1);
				return;
			}

			if (!Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;
				Dispatcher.CurrentDispatcher.UnhandledException += Dispatcher_UnhandledException;
			}

			TaskScheduler.UnobservedTaskException += DotNet40_UnobservedTaskException;

			SharpTreeNode.SetImagesProvider(new WpfWindowsTreeNodeImagesProvider());

			Resources.RegisterDefaultStyles();

			// Register the export provider so that it can be accessed from WPF/XAML components.
			ExportProviderLocator.Register(ExportProvider);
			// Add data templates registered via MEF.
			Resources.MergedDictionaries.Add(DataTemplateManager.CreateDynamicDataTemplates(ExportProvider));

			var sessionSettings = settingsService.SessionSettings;
			ThemeManager.Current.Theme = sessionSettings.Theme;
			if (!string.IsNullOrEmpty(sessionSettings.CurrentCulture))
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture = new(sessionSettings.CurrentCulture);
			}

			ILSpyTraceListener.Install();

			if (CommandLineArguments.ArgumentsParser.IsShowingInformation)
			{
				MessageBox.Show(CommandLineArguments.ArgumentsParser.GetHelpText(), "ILSpy Command Line Arguments");
			}

			if (CommandLineArguments.ArgumentsParser.RemainingArguments.Any())
			{
				string unknownArguments = string.Join(", ", CommandLineArguments.ArgumentsParser.RemainingArguments);
				MessageBox.Show(unknownArguments, "ILSpy Unknown Command Line Arguments Passed");
			}

			settingsService.AssemblyListManager.CreateDefaultAssemblyLists();
		}

		public new static App Current => (App)Application.Current;

		public new MainWindow MainWindow {
			get => (MainWindow)base.MainWindow;
			private set => base.MainWindow = value;
		}

		private static void SingleInstance_NewInstanceDetected(object sender, NewInstanceEventArgs e) => ExportProvider.GetExportedValue<AssemblyTreeModel>().HandleSingleInstanceCommandLineArguments(e.Args).HandleExceptions();

		static Assembly ResolvePluginDependencies(AssemblyLoadContext context, AssemblyName assemblyName)
		{
			var rootPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
			var assemblyFileName = Path.Combine(rootPath, assemblyName.Name + ".dll");
			if (!File.Exists(assemblyFileName))
				return null;
			return context.LoadFromAssemblyPath(assemblyFileName);
		}

		private bool InitializeDependencyInjection(SettingsService settingsService)
		{
			// Add custom logic for resolution of dependencies.
			// This necessary because the AssemblyLoadContext.LoadFromAssemblyPath and related methods,
			// do not automatically load dependencies.
			AssemblyLoadContext.Default.Resolving += ResolvePluginDependencies;
			try
			{
				var services = new ServiceCollection();


				var pluginDir = Path.GetDirectoryName(typeof(App).Module.FullyQualifiedName);
				if (pluginDir != null)
				{
					foreach (var plugin in Directory.GetFiles(pluginDir, "*.Plugin.dll"))
					{
						var name = Path.GetFileNameWithoutExtension(plugin);
						try
						{
							var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(plugin);
							services.BindExports(assembly);
						}
						catch (Exception ex)
						{
							// Cannot show MessageBox here, because WPF would crash with a XamlParseException
							// Remember and show exceptions in text output, once MainWindow is properly initialized
							StartupExceptions.Add(new(ex) { PluginName = name });
						}
					}
				}

				// Add the built-in parts: First, from ILSpyX
				services.BindExports(typeof(IAnalyzer).Assembly);
				// Then from ILSpy itself
				services.BindExports(Assembly.GetExecutingAssembly());
				// Add the settings service
				services.AddSingleton(settingsService);
				// Add the export provider
				services.AddSingleton(_ => ExportProvider);
				// Add the docking manager
				services.AddSingleton(serviceProvider => serviceProvider.GetService<MainWindow>().DockManager);
				services.AddTransient(serviceProvider => serviceProvider.GetService<AssemblyTreeModel>().AssemblyList);

				var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });

				ExportProvider = new ExportProviderAdapter(serviceProvider);

				Exit += (_, _) => serviceProvider.Dispose();

				return true;
			}
			catch (Exception ex)
			{
				if (ex is AggregateException aggregate)
					StartupExceptions.AddRange(aggregate.InnerExceptions.Select(item => new ExceptionData(ex)));
				else
					StartupExceptions.Add(new(ex));

				return false;
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			MainWindow = ExportProvider.GetExportedValue<MainWindow>();
			MainWindow.Show();
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
	}
}