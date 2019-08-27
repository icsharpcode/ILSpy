using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "Decompiler")]
	[OutputType(typeof(CSharpDecompiler))]
	public class GetDecompilerCmdlet : PSCmdlet
	{
		[Parameter(Position = 0, Mandatory = true, HelpMessage = "Path to the assembly you want to decompile")]
		[Alias("PSPath")]
		[ValidateNotNullOrEmpty]
		public string LiteralPath { get; set; }

		[Parameter(Mandatory = false, HelpMessage = "file with decompiler settings")]
		public string SettingsPath { get; set; }

		protected override void ProcessRecord()
		{
			string path = GetUnresolvedProviderPathFromPSPath(LiteralPath);

			try
			{
				var decompiler = GetCSharpDecompiler(path, GetDecompilerSettings());
				WriteObject(decompiler);

			}
			catch (Exception e)
			{
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.AssemblyLoadFailed, ErrorCategory.OperationStopped, null));
			}
		}

		public DecompilerSettings GetDecompilerSettings()
		{
			string configFile = GetConfigFile();
			if (!File.Exists(configFile))
			{
				return new DecompilerSettings()
				{
					ThrowOnAssemblyResolveErrors = false,
					UseDebugSymbols = true,
					ShowDebugInfo = true,
					RemoveDeadCode = true
				};
			}
			var settings = XDocument
				.Load(configFile, LoadOptions.None)
				.Root
				.Element("DecompilerSettings");
			return LoadDecompilerSettings(settings);
		}

		public Decompiler.DecompilerSettings LoadDecompilerSettings(XElement e)
		{
			var newSettings = new Decompiler.DecompilerSettings();
			var properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);
			foreach (var p in properties)
			{
				var value = (bool?)e.Attribute(p.Name);
				if (value.HasValue)
					p.SetValue(newSettings, value.Value);
			}
			return newSettings;
		}


		public string GetConfigFile()
		{
			string configFilePath = null;
			string localPath = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "ILSpy.xml");
			string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICSharpCode\\ILSpy.xml");
			if (!String.IsNullOrEmpty(SettingsPath) && File.Exists(SettingsPath))
			{
				configFilePath = SettingsPath;
			}
			else if (File.Exists(localPath))
			{
				configFilePath = localPath;
			}
			else if (File.Exists(userPath))
			{
				configFilePath = userPath;
			}
			WriteVerbose($"using decomilersettings from file {configFilePath}");
			return configFilePath;
		}


		/// <summary>
		/// Instianciates a CSharpDecompiler from a given assemblyfilename and applies decompilersettings
		/// </summary>
		/// <param name="assemblyFileName">path for assemblyfile to decompile</param>
		/// <param name="settings">optional decompiler settings</param>
		/// <returns>Instance of CSharpDecompiler</returns>
		public CSharpDecompiler GetCSharpDecompiler(string assemblyFileName, DecompilerSettings settings = null)
		{
			if (settings == null)
				settings = new DecompilerSettings();
			using (var file = new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(assemblyFileName, file, PEStreamOptions.PrefetchEntireImage);
				var resolver = new UniversalAssemblyResolver(assemblyFileName, false,
					module.Reader.DetectTargetFrameworkId(), PEStreamOptions.PrefetchMetadata);
				resolver.AddSearchDirectory(Path.GetDirectoryName(module.FullName));
				var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
				var decompiler = new CSharpDecompiler(typeSystem, settings);
				MetadataReaderProvider provider;
				string pdbFileName;
				decompiler.DebugInfoProvider = module.LoadSymbols();
				return decompiler;
			}
		}
	}
}
