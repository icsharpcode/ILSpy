// Copyright (c) 2026 Piero Viano
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
using System.ComponentModel;
using System.Composition;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Symbols;

namespace ICSharpCode.ILSpy.Symbols
{
	/// <summary>
	/// Owns the symbol-server integration of the UI: keeps the assembly list manager's
	/// <see cref="SymbolLocator"/> in sync with <see cref="SymbolSettings"/>, and runs the local
	/// symbol server that serves the open assemblies' symbols to debuggers.
	/// </summary>
	[Export]
	[Shared]
	public sealed partial class SymbolService : ObservableObject, IDisposable
	{
		readonly SettingsService settingsService;
		SymbolLocator locator;
		HttpClient? httpClient;
		SymbolServerHost? host;

		[ObservableProperty]
		bool isServerRunning;

		/// <summary>The URL debuggers use as symbol server; <c>null</c> while stopped.</summary>
		[ObservableProperty]
		string? serverAddress;

		/// <summary>The last start failure (e.g. port in use), or <c>null</c>.</summary>
		[ObservableProperty]
		string? serverError;

		[ImportingConstructor]
		public SymbolService(SettingsService settingsService)
		{
			this.settingsService = settingsService;
			Settings = settingsService.GetSettings<SymbolSettings>();
			locator = CreateLocator(Settings, null);
			settingsService.AssemblyListManager.SymbolLocator = locator;
			Settings.PropertyChanged += Settings_PropertyChanged;
		}

		public SymbolSettings Settings { get; }

		/// <summary>The locator built from the current settings.</summary>
		public SymbolLocator Locator => locator;

		/// <summary>The HTTP client for symbol and source downloads; <c>null</c> uses the shared default.</summary>
		public HttpClient? HttpClient {
			get => httpClient;
			set {
				httpClient = value;
				RebuildLocator();
			}
		}

		public string CacheDirectory => GetCacheDirectory(Settings);

		static string GetCacheDirectory(SymbolSettings settings)
			=> string.IsNullOrWhiteSpace(settings.CacheDirectory) ? SymbolPath.DefaultCacheDirectory : settings.CacheDirectory;

		/// <summary>The symbol path the settings describe, with <c>_NT_SYMBOL_PATH</c> first when enabled.</summary>
		internal static string ComposeSymbolPath(SymbolSettings settings, string? environmentSymbolPath)
		{
			var parts = new List<string>();
			if (settings.UseEnvironmentSymbolPath && !string.IsNullOrWhiteSpace(environmentSymbolPath))
				parts.Add(environmentSymbolPath);
			if (!string.IsNullOrWhiteSpace(settings.SymbolPath))
				parts.Add(settings.SymbolPath);
			return string.Join(";", parts);
		}

		/// <summary>Builds a locator for <paramref name="settings"/>.</summary>
		public static SymbolLocator CreateLocator(SymbolSettings settings, HttpClient? httpClient)
		{
			var path = SymbolPath.Parse(
				ComposeSymbolPath(settings, Environment.GetEnvironmentVariable("_NT_SYMBOL_PATH")),
				GetCacheDirectory(settings));
			return new SymbolLocator(path, httpClient) { AutoDownload = settings.AutoDownload };
		}

		void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(SymbolSettings.SymbolPath) or nameof(SymbolSettings.UseEnvironmentSymbolPath)
				or nameof(SymbolSettings.CacheDirectory) or nameof(SymbolSettings.AutoDownload))
			{
				RebuildLocator();
			}
		}

		void RebuildLocator()
		{
			locator = CreateLocator(Settings, httpClient);
			settingsService.AssemblyListManager.SymbolLocator = locator;
			OnPropertyChanged(nameof(Locator));
		}

		/// <summary>
		/// Creates a provider for the original sources recorded in <paramref name="debugInfo"/>, or
		/// <c>null</c> when it is not a portable PDB.
		/// </summary>
		public OriginalSourceProvider? CreateOriginalSourceProvider(Decompiler.DebugInfo.IDebugInfoProvider? debugInfo)
			=> OriginalSourceProvider.TryCreate(debugInfo, httpClient,
				Path.Combine(CacheDirectory, "sources"));

		/// <summary>Starts the local symbol server when the settings ask for it.</summary>
		public void StartServerIfEnabled()
		{
			if (Settings.StartSymbolServer)
				StartServer();
		}

		/// <summary>
		/// Starts the local symbol server on the configured loopback port. A failure (e.g. port in
		/// use) is reported through <see cref="ServerError"/>.
		/// </summary>
		public void StartServer()
		{
			if (host != null)
				return;
			var assemblyTreeModel = AppComposition.TryGetExport<AssemblyTreeModel>();
			var store = new DecompiledSymbolStore(
				() => assemblyTreeModel?.AssemblyList?.GetAssemblies() ?? Array.Empty<LoadedAssembly>(),
				// The effective settings read UI-bound state, so snapshot them on the UI thread.
				_ => Dispatcher.UIThread.Invoke(settingsService.CreateEffectiveDecompilerSettings),
				Path.Combine(CacheDirectory, "decompiled"));
			var newHost = new SymbolServerHost(store, Settings.SymbolServerPort);
			try
			{
				newHost.Start();
			}
			catch (SocketException ex)
			{
				newHost.Dispose();
				ServerError = ex.Message;
				return;
			}
			host = newHost;
			ServerError = null;
			ServerAddress = newHost.BaseAddress!.ToString();
			IsServerRunning = true;
		}

		public async Task StopServerAsync()
		{
			var current = host;
			if (current == null)
				return;
			host = null;
			await current.StopAsync().ConfigureAwait(true);
			current.Dispose();
			ServerAddress = null;
			IsServerRunning = false;
		}

		[RelayCommand]
		async Task ToggleServerAsync()
		{
			if (IsServerRunning)
				await StopServerAsync();
			else
				StartServer();
		}

		public void Dispose()
		{
			Settings.PropertyChanged -= Settings_PropertyChanged;
			host?.Dispose();
			host = null;
		}
	}
}
