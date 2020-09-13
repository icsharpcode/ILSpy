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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataExtensions;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// Decompiles an assembly into a visual studio project file.
	/// </summary>
	public class WholeProjectDecompiler : IProjectInfoProvider
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
		/// The target directory that the decompiled files are written to.
		/// </summary>
		/// <remarks>
		/// This property is set by DecompileProject() and protected so that overridden protected members
		/// can access it.
		/// </remarks>
		public string TargetDirectory { get; protected set; }

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
			projectWriter = Settings.UseSdkStyleProjectFormat ? ProjectFileWriterSdkStyle.Create() : ProjectFileWriterDefault.Create();
		}

		// per-run members
		HashSet<string> directories = new HashSet<string>(Platform.FileNameComparer);

		readonly IProjectFileWriter projectWriter;

		public void DecompileProject(PEFile moduleDefinition, string targetDirectory, CancellationToken cancellationToken = default(CancellationToken))
		{
			string projectFileName = Path.Combine(targetDirectory, CleanUpFileName(moduleDefinition.Name) + ".csproj");
			using (var writer = new StreamWriter(projectFileName))
			{
				DecompileProject(moduleDefinition, targetDirectory, writer, cancellationToken);
			}
		}

		public ProjectId DecompileProject(PEFile moduleDefinition, string targetDirectory, TextWriter projectFileWriter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(targetDirectory))
			{
				throw new InvalidOperationException("Must set TargetDirectory");
			}
			TargetDirectory = targetDirectory;
			directories.Clear();
			var files = WriteCodeFilesInProject(moduleDefinition, cancellationToken).ToList();
			files.AddRange(WriteResourceFilesInProject(moduleDefinition));
			files.AddRange(WriteMiscellaneousFilesInProject(moduleDefinition));
			if (StrongNameKeyFile != null)
			{
				File.Copy(StrongNameKeyFile, Path.Combine(targetDirectory, Path.GetFileName(StrongNameKeyFile)));
			}

			projectWriter.Write(projectFileWriter, this, files, moduleDefinition);

			string platformName = TargetServices.GetPlatformName(moduleDefinition);
			return new ProjectId(platformName, ProjectGuid, ProjectTypeGuids.CSharpWindows);
		}

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
				Directory.CreateDirectory(Path.Combine(TargetDirectory, prop));
			string assemblyInfo = Path.Combine(prop, "AssemblyInfo.cs");
			using (StreamWriter w = new StreamWriter(Path.Combine(TargetDirectory, assemblyInfo)))
			{
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
					if (string.IsNullOrEmpty(metadata.GetString(type.Namespace)))
					{
						return file;
					}
					else
					{
						string dir = CleanUpDirectoryName(metadata.GetString(type.Namespace));
						if (directories.Add(dir))
							Directory.CreateDirectory(Path.Combine(TargetDirectory, dir));
						return Path.Combine(dir, file);
					}
				}, StringComparer.OrdinalIgnoreCase).ToList();
			int total = files.Count;
			var progress = ProgressIndicator;
			DecompilerTypeSystem ts = new DecompilerTypeSystem(module, AssemblyResolver, Settings);
			Parallel.ForEach(
				Partitioner.Create(files, loadBalance: true),
				new ParallelOptions {
					MaxDegreeOfParallelism = this.MaxDegreeOfParallelism,
					CancellationToken = cancellationToken
				},
				delegate (IGrouping<string, TypeDefinitionHandle> file) {
					using (StreamWriter w = new StreamWriter(Path.Combine(TargetDirectory, file.Key)))
					{
						try
						{
							CSharpDecompiler decompiler = CreateDecompiler(ts);
							decompiler.CancellationToken = cancellationToken;
							var syntaxTree = decompiler.DecompileTypes(file.ToArray());
							syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, Settings.CSharpFormattingOptions));
						}
						catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
						{
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
			foreach (var r in module.Resources.Where(r => r.ResourceType == Metadata.ResourceType.Embedded))
			{
				Stream stream = r.TryOpenStream();
				stream.Position = 0;

				if (r.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
				{
					bool decodedIntoIndividualFiles;
					var individualResources = new List<(string itemType, string fileName)>();
					try
					{
						var resourcesFile = new ResourcesFile(stream);
						if (resourcesFile.AllEntriesAreStreams())
						{
							foreach (var (name, value) in resourcesFile)
							{
								string fileName = Path.Combine(name.Split('/').Select(p => CleanUpFileName(p)).ToArray());
								string dirName = Path.GetDirectoryName(fileName);
								if (!string.IsNullOrEmpty(dirName) && directories.Add(dirName))
								{
									Directory.CreateDirectory(Path.Combine(TargetDirectory, dirName));
								}
								Stream entryStream = (Stream)value;
								entryStream.Position = 0;
								individualResources.AddRange(
									WriteResourceToFile(fileName, (string)name, entryStream));
							}
							decodedIntoIndividualFiles = true;
						}
						else
						{
							decodedIntoIndividualFiles = false;
						}
					}
					catch (BadImageFormatException)
					{
						decodedIntoIndividualFiles = false;
					}
					catch (EndOfStreamException)
					{
						decodedIntoIndividualFiles = false;
					}
					if (decodedIntoIndividualFiles)
					{
						foreach (var entry in individualResources)
						{
							yield return entry;
						}
					}
					else
					{
						stream.Position = 0;
						string fileName = GetFileNameForResource(r.Name);
						foreach (var entry in WriteResourceToFile(fileName, r.Name, stream))
						{
							yield return entry;
						}
					}
				}
				else
				{
					string fileName = GetFileNameForResource(r.Name);
					using (FileStream fs = new FileStream(Path.Combine(TargetDirectory, fileName), FileMode.Create, FileAccess.Write))
					{
						stream.Position = 0;
						stream.CopyTo(fs);
					}
					yield return ("EmbeddedResource", fileName);
				}
			}
		}

		protected virtual IEnumerable<(string itemType, string fileName)> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			if (fileName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
			{
				string resx = Path.ChangeExtension(fileName, ".resx");
				try
				{
					using (FileStream fs = new FileStream(Path.Combine(TargetDirectory, resx), FileMode.Create, FileAccess.Write))
					using (ResXResourceWriter writer = new ResXResourceWriter(fs))
					{
						foreach (var entry in new ResourcesFile(entryStream))
						{
							writer.AddResource(entry.Key, entry.Value);
						}
					}
					return new[] { ("EmbeddedResource", resx) };
				}
				catch (BadImageFormatException)
				{
					// if the .resources can't be decoded, just save them as-is
				}
				catch (EndOfStreamException)
				{
					// if the .resources can't be decoded, just save them as-is
				}
			}
			using (FileStream fs = new FileStream(Path.Combine(TargetDirectory, fileName), FileMode.Create, FileAccess.Write))
			{
				entryStream.CopyTo(fs);
			}
			return new[] { ("EmbeddedResource", fileName) };
		}

		string GetFileNameForResource(string fullName)
		{
			string[] splitName = fullName.Split('.');
			string fileName = CleanUpFileName(fullName);
			for (int i = splitName.Length - 1; i > 0; i--)
			{
				string ns = string.Join(".", splitName, 0, i);
				if (directories.Contains(ns))
				{
					string name = string.Join(".", splitName, i, splitName.Length - i);
					fileName = Path.Combine(ns, CleanUpFileName(name));
					break;
				}
			}
			return fileName;
		}
		#endregion

		#region WriteMiscellaneousFilesInProject
		protected virtual IEnumerable<(string itemType, string fileName)> WriteMiscellaneousFilesInProject(PEFile module)
		{
			var resources = module.Reader.ReadWin32Resources();
			if (resources == null)
				yield break;

			byte[] appIcon = CreateApplicationIcon(resources);
			if (appIcon != null)
			{
				File.WriteAllBytes(Path.Combine(TargetDirectory, "app.ico"), appIcon);
				yield return ("ApplicationIcon", "app.ico");
			}

			byte[] appManifest = CreateApplicationManifest(resources);
			if (appManifest != null)
			{
				File.WriteAllBytes(Path.Combine(TargetDirectory, "app.manifest"), appManifest);
				yield return ("ApplicationManifest", "app.manifest");
			}

			var appConfig = module.FileName + ".config";
			if (File.Exists(appConfig))
			{
				File.Copy(appConfig, Path.Combine(TargetDirectory, "app.config"));
				yield return ("ApplicationConfig", Path.GetFileName(appConfig));
			}
		}

		const int RT_ICON = 3;
		const int RT_GROUP_ICON = 14;

		unsafe static byte[] CreateApplicationIcon(Win32ResourceDirectory resources)
		{
			var iconGroup = resources.Find(new Win32ResourceName(RT_GROUP_ICON))?.Directories.FirstOrDefault()?.Datas.FirstOrDefault()?.Data;
			if (iconGroup == null)
				return null;

			var iconDir = resources.Find(new Win32ResourceName(RT_ICON));
			if (iconDir == null)
				return null;

			using var outStream = new MemoryStream();
			using var writer = new BinaryWriter(outStream);
			fixed (byte* pIconGroupData = iconGroup)
			{
				var pIconGroup = (GRPICONDIR*)pIconGroupData;
				writer.Write(pIconGroup->idReserved);
				writer.Write(pIconGroup->idType);
				writer.Write(pIconGroup->idCount);

				int iconCount = pIconGroup->idCount;
				uint offset = (2 * 3) + ((uint)iconCount * 0x10);
				for (int i = 0; i < iconCount; i++)
				{
					var pIconEntry = pIconGroup->idEntries + i;
					writer.Write(pIconEntry->bWidth);
					writer.Write(pIconEntry->bHeight);
					writer.Write(pIconEntry->bColorCount);
					writer.Write(pIconEntry->bReserved);
					writer.Write(pIconEntry->wPlanes);
					writer.Write(pIconEntry->wBitCount);
					writer.Write(pIconEntry->dwBytesInRes);
					writer.Write(offset);
					offset += pIconEntry->dwBytesInRes;
				}

				for (int i = 0; i < iconCount; i++)
				{
					var icon = iconDir.FindDirectory(new Win32ResourceName(pIconGroup->idEntries[i].nID))?.Datas.FirstOrDefault()?.Data;
					if (icon == null)
						return null;
					writer.Write(icon);
				}
			}

			return outStream.ToArray();
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		unsafe struct GRPICONDIR
		{
			public ushort idReserved;
			public ushort idType;
			public ushort idCount;
			private fixed byte _idEntries[1];
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
			public GRPICONDIRENTRY* idEntries {
				get {
					fixed (byte* p = _idEntries)
						return (GRPICONDIRENTRY*)p;
				}
			}
		};

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		struct GRPICONDIRENTRY
		{
			public byte bWidth;
			public byte bHeight;
			public byte bColorCount;
			public byte bReserved;
			public ushort wPlanes;
			public ushort wBitCount;
			public uint dwBytesInRes;
			public short nID;
		};

		const int RT_MANIFEST = 24;

		unsafe static byte[] CreateApplicationManifest(Win32ResourceDirectory resources)
		{
			return resources.Find(new Win32ResourceName(RT_MANIFEST))?.Directories.FirstOrDefault()?.Datas.FirstOrDefault()?.Data;
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
			foreach (var c in text)
			{
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

		/// <summary>
		/// Cleans up a node name for use as a directory name.
		/// </summary>
		public static string CleanUpDirectoryName(string text)
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
			foreach (var c in text)
			{
				if (char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '\\')
					b.Append(c);
				else if (c == '.' && b.Length > 0 && b[b.Length - 1] != '.')
					b.Append('\\'); // allow dot, but never two in a row
				else
					b.Append('-');
				if (b.Length >= 200)
					break; // limit to 200 chars
			}
			if (b.Length == 0)
				b.Append('-');
			string name = b.ToString();
			if (name == ".")
				return "_";
			else
				return name;
		}

		static bool IsReservedFileSystemName(string name)
		{
			switch (name.ToUpperInvariant())
			{
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
