// Copyright (c) 2016 Daniel Grunwald
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Threading;
using System.Text;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using static ICSharpCode.Decompiler.Metadata.DotNetCorePathFinderExtensions;
using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.DebugInfo;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// Decompiles an assembly into a visual studio project file.
	/// </summary>
	public class WholeProjectDecompiler
	{
		#region Settings
		/// <summary>
		/// Gets the setting this instance uses for decompiling.
		/// </summary>
		public DecompilerSettings Settings { get; }

		LanguageVersion? languageVersion;

		public LanguageVersion LanguageVersion {
			get { return languageVersion ?? Settings.GetMinimumRequiredVersion(); }
			set {
				var minVersion = Settings.GetMinimumRequiredVersion();
				if (value < minVersion)
					throw new InvalidOperationException($"The chosen settings require at least {minVersion}." +
						$" Please change the DecompilerSettings accordingly.");
				languageVersion = value;
			}
		}

		public IAssemblyResolver AssemblyResolver { get; }

		public IDebugInfoProvider DebugInfoProvider { get; }

		/// <summary>
		/// The MSBuild ProjectGuid to use for the new project.
		/// </summary>
		public Guid ProjectGuid { get; }

		/// <summary>
		/// Path to the snk file to use for signing.
		/// <c>null</c> to not sign.
		/// </summary>
		public string StrongNameKeyFile { get; set; }

		public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

		public IProgress<DecompilationProgress> ProgressIndicator { get; set; }
		#endregion

		public WholeProjectDecompiler(IAssemblyResolver assemblyResolver)
			: this(new DecompilerSettings(), assemblyResolver, debugInfoProvider: null)
		{
		}

		public WholeProjectDecompiler(
			DecompilerSettings settings,
			IAssemblyResolver assemblyResolver,
			IDebugInfoProvider debugInfoProvider)
			: this(settings, Guid.NewGuid(), assemblyResolver, debugInfoProvider)
		{
		}

		protected WholeProjectDecompiler(
			DecompilerSettings settings,
			Guid projectGuid,
			IAssemblyResolver assemblyResolver,
			IDebugInfoProvider debugInfoProvider)
		{
			Settings = settings ?? throw new ArgumentNullException(nameof(settings));
			ProjectGuid = projectGuid;
			AssemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
			DebugInfoProvider = debugInfoProvider;
		}

		// per-run members
		HashSet<string> directories = new HashSet<string>(Platform.FileNameComparer);

		/// <summary>
		/// The target directory that the decompiled files are written to.
		/// </summary>
		/// <remarks>
		/// This field is set by DecompileProject() and protected so that overridden protected members
		/// can access it.
		/// </remarks>
		protected string targetDirectory;

		public void DecompileProject(PEFile moduleDefinition, string targetDirectory, CancellationToken cancellationToken = default(CancellationToken))
		{
			string projectFileName = Path.Combine(targetDirectory, CleanUpFileName(moduleDefinition.Name) + ".csproj");
			using (var writer = new StreamWriter(projectFileName)) {
				DecompileProject(moduleDefinition, targetDirectory, writer, cancellationToken);
			}
		}

		public ProjectId DecompileProject(PEFile moduleDefinition, string targetDirectory, TextWriter projectFileWriter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(targetDirectory)) {
				throw new InvalidOperationException("Must set TargetDirectory");
			}
			this.targetDirectory = targetDirectory;
			directories.Clear();
			var files = WriteCodeFilesInProject(moduleDefinition, cancellationToken).ToList();
			files.AddRange(WriteResourceFilesInProject(moduleDefinition));
			if (StrongNameKeyFile != null) {
				File.Copy(StrongNameKeyFile, Path.Combine(targetDirectory, Path.GetFileName(StrongNameKeyFile)));
			}
			return WriteProjectFile(projectFileWriter, files, moduleDefinition);
		}

		#region WriteProjectFile
		ProjectId WriteProjectFile(TextWriter writer, IEnumerable<(string itemType, string fileName)> files, Metadata.PEFile module)
		{
			const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			string platformName = GetPlatformName(module);
			Guid guid = this.ProjectGuid;
			var targetFramework = DetectTargetFramework(module);

			List<Guid> typeGuids = new List<Guid>();
			if (targetFramework.IsPortableClassLibrary)
				typeGuids.Add(ProjectTypeGuids.PortableLibrary);
			typeGuids.Add(ProjectTypeGuids.CSharpWindows);
			// TODO: .NET core support

			using (XmlTextWriter w = new XmlTextWriter(writer)) {
				w.Formatting = Formatting.Indented;
				w.WriteStartDocument();
				w.WriteStartElement("Project", ns);
				w.WriteAttributeString("ToolsVersion", "4.0");
				w.WriteAttributeString("DefaultTargets", "Build");

				w.WriteStartElement("PropertyGroup");
				w.WriteElementString("ProjectGuid", guid.ToString("B").ToUpperInvariant());
				w.WriteElementString("ProjectTypeGuids", string.Join(";", typeGuids.Select(g => g.ToString("B").ToUpperInvariant())));

				w.WriteStartElement("Configuration");
				w.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
				w.WriteValue("Debug");
				w.WriteEndElement(); // </Configuration>

				w.WriteStartElement("Platform");
				w.WriteAttributeString("Condition", " '$(Platform)' == '' ");
				w.WriteValue(platformName);
				w.WriteEndElement(); // </Platform>

				if (module.Reader.PEHeaders.IsDll) {
					w.WriteElementString("OutputType", "Library");
				} else {
					switch (module.Reader.PEHeaders.PEHeader.Subsystem) {
						case Subsystem.WindowsGui:
							w.WriteElementString("OutputType", "WinExe");
							break;
						case Subsystem.WindowsCui:
							w.WriteElementString("OutputType", "Exe");
							break;
						default:
							w.WriteElementString("OutputType", "Library");
							break;
					}
				}

				w.WriteElementString("LangVersion", LanguageVersion.ToString().Replace("CSharp", "").Replace('_', '.'));

				w.WriteElementString("AssemblyName", module.Name);
				if (targetFramework.TargetFrameworkIdentifier != null)
					w.WriteElementString("TargetFrameworkIdentifier", targetFramework.TargetFrameworkIdentifier);
				if (targetFramework.TargetFrameworkVersion != null)
					w.WriteElementString("TargetFrameworkVersion", targetFramework.TargetFrameworkVersion);
				if (targetFramework.TargetFrameworkProfile != null)
					w.WriteElementString("TargetFrameworkProfile", targetFramework.TargetFrameworkProfile);
				w.WriteElementString("WarningLevel", "4");
				w.WriteElementString("AllowUnsafeBlocks", "True");

				if (StrongNameKeyFile != null) {
					w.WriteElementString("SignAssembly", "True");
					w.WriteElementString("AssemblyOriginatorKeyFile", Path.GetFileName(StrongNameKeyFile));
				}

				w.WriteEndElement(); // </PropertyGroup>

				w.WriteStartElement("PropertyGroup"); // platform-specific
				w.WriteAttributeString("Condition", " '$(Platform)' == '" + platformName + "' ");
				w.WriteElementString("PlatformTarget", platformName);
				if (targetFramework.VersionNumber > 400 && platformName == "AnyCPU" && (module.Reader.PEHeaders.CorHeader.Flags & CorFlags.Prefers32Bit) == 0) {
					w.WriteElementString("Prefer32Bit", "false");
				}
				w.WriteEndElement(); // </PropertyGroup> (platform-specific)

				w.WriteStartElement("PropertyGroup"); // Debug
				w.WriteAttributeString("Condition", " '$(Configuration)' == 'Debug' ");
				w.WriteElementString("OutputPath", "bin\\Debug\\");
				w.WriteElementString("DebugSymbols", "true");
				w.WriteElementString("DebugType", "full");
				w.WriteElementString("Optimize", "false");
				w.WriteEndElement(); // </PropertyGroup> (Debug)

				w.WriteStartElement("PropertyGroup"); // Release
				w.WriteAttributeString("Condition", " '$(Configuration)' == 'Release' ");
				w.WriteElementString("OutputPath", "bin\\Release\\");
				w.WriteElementString("DebugSymbols", "true");
				w.WriteElementString("DebugType", "pdbonly");
				w.WriteElementString("Optimize", "true");
				w.WriteEndElement(); // </PropertyGroup> (Release)


				w.WriteStartElement("ItemGroup"); // References
				foreach (var r in module.AssemblyReferences) {
					if (r.Name != "mscorlib") {
						w.WriteStartElement("Reference");
						w.WriteAttributeString("Include", r.Name);
						var asm = AssemblyResolver.Resolve(r);
						if (!AssemblyResolver.IsGacAssembly(r)) {
							if (asm != null) {
								w.WriteElementString("HintPath", asm.FileName);
							}
						}
						w.WriteEndElement();
					}
				}
				w.WriteEndElement(); // </ItemGroup> (References)

				foreach (IGrouping<string, string> gr in (from f in files group f.fileName by f.itemType into g orderby g.Key select g)) {
					w.WriteStartElement("ItemGroup");
					foreach (string file in gr.OrderBy(f => f, StringComparer.OrdinalIgnoreCase)) {
						w.WriteStartElement(gr.Key);
						w.WriteAttributeString("Include", file);
						w.WriteEndElement();
					}
					w.WriteEndElement();
				}
				if (targetFramework.IsPortableClassLibrary) {
					w.WriteStartElement("Import");
					w.WriteAttributeString("Project", "$(MSBuildExtensionsPath32)\\Microsoft\\Portable\\$(TargetFrameworkVersion)\\Microsoft.Portable.CSharp.targets");
					w.WriteEndElement();
				} else {
					w.WriteStartElement("Import");
					w.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
					w.WriteEndElement();
				}

				w.WriteEndDocument();
			}

			return new ProjectId(platformName, guid, ProjectTypeGuids.CSharpWindows);
		}

		struct TargetFramework
		{
			public string TargetFrameworkIdentifier;
			public string TargetFrameworkVersion;
			public string TargetFrameworkProfile;
			public int VersionNumber;
			public bool IsPortableClassLibrary => TargetFrameworkIdentifier == ".NETPortable";
		}

		private TargetFramework DetectTargetFramework(PEFile module)
		{
			TargetFramework result = default;

			switch (module.GetRuntime()) {
				case Metadata.TargetRuntime.Net_1_0:
					result.VersionNumber = 100;
					result.TargetFrameworkVersion = "v1.0";
					break;
				case Metadata.TargetRuntime.Net_1_1:
					result.VersionNumber = 110;
					result.TargetFrameworkVersion = "v1.1";
					break;
				case Metadata.TargetRuntime.Net_2_0:
					result.VersionNumber = 200;
					result.TargetFrameworkVersion = "v2.0";
					// TODO: Detect when .NET 3.0/3.5 is required
					break;
				default:
					result.VersionNumber = 400;
					result.TargetFrameworkVersion = "v4.0";
					break;
			}

			string targetFramework = module.DetectTargetFrameworkId();
			if (!string.IsNullOrEmpty(targetFramework)) {
				string[] frameworkParts = targetFramework.Split(',');
				result.TargetFrameworkIdentifier = frameworkParts.FirstOrDefault(a => !a.StartsWith("Version=", StringComparison.OrdinalIgnoreCase) && !a.StartsWith("Profile=", StringComparison.OrdinalIgnoreCase));
				string frameworkVersion = frameworkParts.FirstOrDefault(a => a.StartsWith("Version=", StringComparison.OrdinalIgnoreCase));
				if (frameworkVersion != null) {
					result.TargetFrameworkVersion = frameworkVersion.Substring("Version=".Length);
					result.VersionNumber = int.Parse(frameworkVersion.Substring("Version=v".Length).Replace(".", ""));
					if (result.VersionNumber < 100) result.VersionNumber *= 10;
				}
				string frameworkProfile = frameworkParts.FirstOrDefault(a => a.StartsWith("Profile=", StringComparison.OrdinalIgnoreCase));
				if (frameworkProfile != null)
					result.TargetFrameworkProfile = frameworkProfile.Substring("Profile=".Length);
			}
			return result;
		}

		#endregion

		#region WriteCodeFilesInProject
		protected virtual bool IncludeTypeWhenDecompilingProject(PEFile module, TypeDefinitionHandle type)
		{
			var metadata = module.Metadata;
			var typeDef = metadata.GetTypeDefinition(type);
			if (metadata.GetString(typeDef.Name) == "<Module>" || CSharpDecompiler.MemberIsHidden(module, type, Settings))
				return false;
			if (metadata.GetString(typeDef.Namespace) == "XamlGeneratedNamespace" && metadata.GetString(typeDef.Name) == "GeneratedInternalTypeHelper")
				return false;
			return true;
		}

		CSharpDecompiler CreateDecompiler(DecompilerTypeSystem ts)
		{
			var decompiler = new CSharpDecompiler(ts, Settings);
			decompiler.DebugInfoProvider = DebugInfoProvider;
			decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			decompiler.AstTransforms.Add(new RemoveCLSCompliantAttribute());
			return decompiler;
		}

		IEnumerable<(string itemType, string fileName)> WriteAssemblyInfo(DecompilerTypeSystem ts, CancellationToken cancellationToken)
		{
			var decompiler = CreateDecompiler(ts);
			decompiler.CancellationToken = cancellationToken;
			decompiler.AstTransforms.Add(new RemoveCompilerGeneratedAssemblyAttributes());
			SyntaxTree syntaxTree = decompiler.DecompileModuleAndAssemblyAttributes();

			const string prop = "Properties";
			if (directories.Add(prop))
				Directory.CreateDirectory(Path.Combine(targetDirectory, prop));
			string assemblyInfo = Path.Combine(prop, "AssemblyInfo.cs");
			using (StreamWriter w = new StreamWriter(Path.Combine(targetDirectory, assemblyInfo))) {
				syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, Settings.CSharpFormattingOptions));
			}
			return new[] { ("Compile", assemblyInfo) };
		}

		IEnumerable<(string itemType, string fileName)> WriteCodeFilesInProject(Metadata.PEFile module, CancellationToken cancellationToken)
		{
			var metadata = module.Metadata;
			var files = module.Metadata.GetTopLevelTypeDefinitions().Where(td => IncludeTypeWhenDecompilingProject(module, td)).GroupBy(
				delegate (TypeDefinitionHandle h) {
					var type = metadata.GetTypeDefinition(h);
					string file = CleanUpFileName(metadata.GetString(type.Name)) + ".cs";
					if (string.IsNullOrEmpty(metadata.GetString(type.Namespace))) {
						return file;
					} else {
						string dir = CleanUpFileName(metadata.GetString(type.Namespace));
						if (directories.Add(dir))
							Directory.CreateDirectory(Path.Combine(targetDirectory, dir));
						return Path.Combine(dir, file);
					}
				}, StringComparer.OrdinalIgnoreCase).ToList();
			int total = files.Count;
			var progress = ProgressIndicator;
			DecompilerTypeSystem ts = new DecompilerTypeSystem(module, AssemblyResolver, Settings);
			Parallel.ForEach(
				files,
				new ParallelOptions {
					MaxDegreeOfParallelism = this.MaxDegreeOfParallelism,
					CancellationToken = cancellationToken
				},
				delegate (IGrouping<string, TypeDefinitionHandle> file) {
					using (StreamWriter w = new StreamWriter(Path.Combine(targetDirectory, file.Key))) {
						try {
							CSharpDecompiler decompiler = CreateDecompiler(ts);
							decompiler.CancellationToken = cancellationToken;
							var syntaxTree = decompiler.DecompileTypes(file.ToArray());
							syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, Settings.CSharpFormattingOptions));
						} catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException)) {
							throw new DecompilerException(module, $"Error decompiling for '{file.Key}'", innerException);
						}
					}
					progress?.Report(new DecompilationProgress(total, file.Key));
				});
			return files.Select(f => ("Compile", f.Key)).Concat(WriteAssemblyInfo(ts, cancellationToken));
		}
		#endregion

		#region WriteResourceFilesInProject
		protected virtual IEnumerable<(string itemType, string fileName)> WriteResourceFilesInProject(Metadata.PEFile module)
		{
			foreach (var r in module.Resources.Where(r => r.ResourceType == Metadata.ResourceType.Embedded)) {
				Stream stream = r.TryOpenStream();
				stream.Position = 0;

				if (r.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) {
					bool decodedIntoIndividualFiles;
					var individualResources = new List<(string itemType, string fileName)>();
					try {
						var resourcesFile = new ResourcesFile(stream);
						if (resourcesFile.AllEntriesAreStreams()) {
							foreach (var (name, value) in resourcesFile) {
								string fileName = Path.Combine(name.Split('/').Select(p => CleanUpFileName(p)).ToArray());
								string dirName = Path.GetDirectoryName(fileName);
								if (!string.IsNullOrEmpty(dirName) && directories.Add(dirName)) {
									Directory.CreateDirectory(Path.Combine(targetDirectory, dirName));
								}
								Stream entryStream = (Stream)value;
								entryStream.Position = 0;
								individualResources.AddRange(
									WriteResourceToFile(fileName, (string)name, entryStream));
							}
							decodedIntoIndividualFiles = true;
						} else {
							decodedIntoIndividualFiles = false;
						}
					} catch (BadImageFormatException) {
						decodedIntoIndividualFiles = false;
					} catch (EndOfStreamException) {
						decodedIntoIndividualFiles = false;
					}
					if (decodedIntoIndividualFiles) {
						foreach (var entry in individualResources) {
							yield return entry;
						}
					} else {
						stream.Position = 0;
						string fileName = GetFileNameForResource(r.Name);
						foreach (var entry in WriteResourceToFile(fileName, r.Name, stream)) {
							yield return entry;
						}
					}
				} else {
					string fileName = GetFileNameForResource(r.Name);
					using (FileStream fs = new FileStream(Path.Combine(targetDirectory, fileName), FileMode.Create, FileAccess.Write)) {
						stream.Position = 0;
						stream.CopyTo(fs);
					}
					yield return ("EmbeddedResource", fileName);
				}
			}
		}

		protected virtual IEnumerable<(string itemType, string fileName)> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			if (fileName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) {
				string resx = Path.ChangeExtension(fileName, ".resx");
				try {
					using (FileStream fs = new FileStream(Path.Combine(targetDirectory, resx), FileMode.Create, FileAccess.Write))
					using (ResXResourceWriter writer = new ResXResourceWriter(fs)) {
						foreach (var entry in new ResourcesFile(entryStream)) {
							writer.AddResource(entry.Key, entry.Value);
						}
					}
					return new[] { ("EmbeddedResource", resx) };
				} catch (BadImageFormatException) {
					// if the .resources can't be decoded, just save them as-is
				} catch (EndOfStreamException) {
					// if the .resources can't be decoded, just save them as-is
				}
			}
			using (FileStream fs = new FileStream(Path.Combine(targetDirectory, fileName), FileMode.Create, FileAccess.Write)) {
				entryStream.CopyTo(fs);
			}
			return new[] { ("EmbeddedResource", fileName) };
		}

		string GetFileNameForResource(string fullName)
		{
			string[] splitName = fullName.Split('.');
			string fileName = CleanUpFileName(fullName);
			for (int i = splitName.Length - 1; i > 0; i--) {
				string ns = string.Join(".", splitName, 0, i);
				if (directories.Contains(ns)) {
					string name = string.Join(".", splitName, i, splitName.Length - i);
					fileName = Path.Combine(ns, CleanUpFileName(name));
					break;
				}
			}
			return fileName;
		}
		#endregion

		/// <summary>
		/// Cleans up a node name for use as a file name.
		/// </summary>
		public static string CleanUpFileName(string text)
		{
			int pos = text.IndexOf(':');
			if (pos > 0)
				text = text.Substring(0, pos);
			pos = text.IndexOf('`');
			if (pos > 0)
				text = text.Substring(0, pos);
			text = text.Trim();
			// Whitelist allowed characters, replace everything else:
			StringBuilder b = new StringBuilder(text.Length);
			foreach (var c in text) {
				if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
					b.Append(c);
				else if (c == '.' && b.Length > 0 && b[b.Length - 1] != '.')
					b.Append('.'); // allow dot, but never two in a row
				else
					b.Append('-');
				if (b.Length >= 200)
					break; // limit to 200 chars
			}
			if (b.Length == 0)
				b.Append('-');
			string name = b.ToString();
			if (IsReservedFileSystemName(name))
				return name + "_";
			else if (name == ".")
				return "_";
			else
				return name;
		}

		static bool IsReservedFileSystemName(string name)
		{
			switch (name.ToUpperInvariant()) {
				case "AUX":
				case "COM1":
				case "COM2":
				case "COM3":
				case "COM4":
				case "COM5":
				case "COM6":
				case "COM7":
				case "COM8":
				case "COM9":
				case "CON":
				case "LPT1":
				case "LPT2":
				case "LPT3":
				case "LPT4":
				case "LPT5":
				case "LPT6":
				case "LPT7":
				case "LPT8":
				case "LPT9":
				case "NUL":
				case "PRN":
					return true;
				default:
					return false;
			}
		}

		public static string GetPlatformName(Metadata.PEFile module)
		{
			var headers = module.Reader.PEHeaders;
			var architecture = headers.CoffHeader.Machine;
			var characteristics = headers.CoffHeader.Characteristics;
			var corflags = headers.CorHeader.Flags;
			switch (architecture) {
				case Machine.I386:
					if ((corflags & CorFlags.Prefers32Bit) != 0)
						return "AnyCPU";
					if ((corflags & CorFlags.Requires32Bit) != 0)
						return "x86";
					// According to ECMA-335, II.25.3.3.1 CorFlags.Requires32Bit and Characteristics.Bit32Machine must be in sync
					// for assemblies containing managed code. However, this is not true for C++/CLI assemblies.
					if ((corflags & CorFlags.ILOnly) == 0 && (characteristics & Characteristics.Bit32Machine) != 0)
						return "x86";
					return "AnyCPU";
				case Machine.Amd64:
					return "x64";
				case Machine.IA64:
					return "Itanium";
				default:
					return architecture.ToString();
			}
		}
	}

	public readonly struct DecompilationProgress
	{
		public readonly int TotalNumberOfFiles;
		public readonly string Status;

		public DecompilationProgress(int total, string status = null)
		{
			this.TotalNumberOfFiles = total;
			this.Status = status ?? "";
		}
	}
}
