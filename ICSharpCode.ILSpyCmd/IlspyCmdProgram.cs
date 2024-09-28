using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.PdbProvider;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.Hosting;

namespace ICSharpCode.ILSpyCmd
{
	[Command(Name = "ilspycmd", Description = "dotnet tool for decompiling .NET assemblies and generating portable PDBs",
		ExtendedHelpText = @"
Remarks:
  -o is valid with every option and required when using -p.

Examples:
    Decompile assembly to console out.
        ilspycmd sample.dll

    Decompile assembly to destination directory (single C# file).
        ilspycmd -o c:\decompiled sample.dll

    Decompile assembly to destination directory, create a project file, one source file per type.
        ilspycmd -p -o c:\decompiled sample.dll

    Decompile assembly to destination directory, create a project file, one source file per type, 
    into nicely nested directories.
        ilspycmd --nested-directories -p -o c:\decompiled sample.dll
")]
	[HelpOption("-h|--help")]
	[ProjectOptionRequiresOutputDirectoryValidation]
	[VersionOptionFromMember("-v|--version", Description = "Show version of ICSharpCode.Decompiler used.",
		MemberName = nameof(DecompilerVersion))]
	class ILSpyCmdProgram
	{
		// https://natemcmaster.github.io/CommandLineUtils/docs/advanced/generic-host.html
		// https://github.com/natemcmaster/CommandLineUtils/blob/main/docs/samples/dependency-injection/generic-host/Program.cs
		public static Task<int> Main(string[] args) => new HostBuilder().RunCommandLineApplicationAsync<ILSpyCmdProgram>(args);

		[FilesExist]
		[Required]
		[Argument(0, "Assembly file name(s)", "The list of assemblies that is being decompiled. This argument is mandatory.")]
		public string[] InputAssemblyNames { get; }

		[Option("-o|--outputdir <directory>", "The output directory, if omitted decompiler output is written to standard out.", CommandOptionType.SingleValue)]
		public string OutputDirectory { get; }

		[Option("-p|--project", "Decompile assembly as compilable project. This requires the output directory option.", CommandOptionType.NoValue)]
		public bool CreateCompilableProjectFlag { get; }

		[Option("-t|--type <type-name>", "The fully qualified name of the type to decompile.", CommandOptionType.SingleValue)]
		public string TypeName { get; }

		[Option("-il|--ilcode", "Show IL code.", CommandOptionType.NoValue)]
		public bool ShowILCodeFlag { get; }

		[Option("--il-sequence-points", "Show IL with sequence points. Implies -il.", CommandOptionType.NoValue)]
		public bool ShowILSequencePointsFlag { get; }

		[Option("-genpdb|--generate-pdb", "Generate PDB.", CommandOptionType.NoValue)]
		public bool CreateDebugInfoFlag { get; }

		[FileExistsOrNull]
		[Option("-usepdb|--use-varnames-from-pdb", "Use variable names from PDB.", CommandOptionType.SingleOrNoValue)]
		public (bool IsSet, string Value) InputPDBFile { get; }

		[Option("-l|--list <entity-type(s)>", "Lists all entities of the specified type(s). Valid types: c(lass), i(nterface), s(truct), d(elegate), e(num)", CommandOptionType.MultipleValue)]
		public string[] EntityTypes { get; } = new string[0];

		public string DecompilerVersion => "ilspycmd: " + typeof(ILSpyCmdProgram).Assembly.GetName().Version.ToString() +
				Environment.NewLine
				+ "ICSharpCode.Decompiler: " +
				typeof(FullTypeName).Assembly.GetName().Version.ToString();

		[Option("-lv|--languageversion <version>", "C# Language version: CSharp1, CSharp2, CSharp3, " +
			"CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, " +
			"CSharp10_0, Preview or Latest", CommandOptionType.SingleValue)]
		public LanguageVersion LanguageVersion { get; } = LanguageVersion.Latest;

		[DirectoryExists]
		[Option("-r|--referencepath <path>", "Path to a directory containing dependencies of the assembly that is being decompiled.", CommandOptionType.MultipleValue)]
		public string[] ReferencePaths { get; }

		[Option("--no-dead-code", "Remove dead code.", CommandOptionType.NoValue)]
		public bool RemoveDeadCode { get; }

		[Option("--no-dead-stores", "Remove dead stores.", CommandOptionType.NoValue)]
		public bool RemoveDeadStores { get; }

		[Option("-d|--dump-package", "Dump package assemblies into a folder. This requires the output directory option.", CommandOptionType.NoValue)]
		public bool DumpPackageFlag { get; }

		[Option("--nested-directories", "Use nested directories for namespaces.", CommandOptionType.NoValue)]
		public bool NestedDirectories { get; }

		[Option("--disable-updatecheck", "If using ilspycmd in a tight loop or fully automated scenario, you might want to disable the automatic update check.", CommandOptionType.NoValue)]
		public bool DisableUpdateCheck { get; }

		private readonly IHostEnvironment _env;
		public ILSpyCmdProgram(IHostEnvironment env)
		{
			_env = env;
		}

		private async Task<int> OnExecuteAsync(CommandLineApplication app)
		{
			Task<PackageCheckResult>? updateCheckTask = null;
			if (!DisableUpdateCheck)
			{
				updateCheckTask = DotNetToolUpdateChecker.CheckForPackageUpdateAsync("ilspycmd");
			}

			TextWriter output = System.Console.Out;
			string outputDirectory = ResolveOutputDirectory(OutputDirectory);

			if (outputDirectory != null)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			try
			{
				if (CreateCompilableProjectFlag)
				{
					if (InputAssemblyNames.Length == 1)
					{
						string projectFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(InputAssemblyNames[0]) + ".csproj");
						DecompileAsProject(InputAssemblyNames[0], projectFileName);
						return 0;
					}
					var projects = new List<ProjectItem>();
					foreach (var file in InputAssemblyNames)
					{
						string projectFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(file), Path.GetFileNameWithoutExtension(file) + ".csproj");
						Directory.CreateDirectory(Path.GetDirectoryName(projectFileName));
						ProjectId projectId = DecompileAsProject(file, projectFileName);
						projects.Add(new ProjectItem(projectFileName, projectId.PlatformName, projectId.Guid, projectId.TypeGuid));
					}
					SolutionCreator.WriteSolutionFile(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(outputDirectory) + ".sln"), projects);
					return 0;
				}
				else
				{
					foreach (var file in InputAssemblyNames)
					{
						int result = PerformPerFileAction(file);
						if (result != 0)
							return result;
					}
					return 0;
				}
			}
			catch (Exception ex)
			{
				app.Error.WriteLine(ex.ToString());
				return ProgramExitCodes.EX_SOFTWARE;
			}
			finally
			{
				output.Close();

				if (null != updateCheckTask)
				{
					var checkResult = await updateCheckTask;
					if (null != checkResult && checkResult.UpdateRecommendation)
					{
						Console.WriteLine("You are not using the latest version of the tool, please update.");
						Console.WriteLine($"Latest version is '{checkResult.LatestVersion}' (yours is '{checkResult.RunningVersion}')");
					}
				}
			}

			int PerformPerFileAction(string fileName)
			{
				if (EntityTypes.Any())
				{
					var values = EntityTypes.SelectMany(v => v.Split(',', ';')).ToArray();
					HashSet<TypeKind> kinds = TypesParser.ParseSelection(values);
					if (outputDirectory != null)
					{
						string outputName = Path.GetFileNameWithoutExtension(fileName);
						output = File.CreateText(Path.Combine(outputDirectory, outputName) + ".list.txt");
					}

					return ListContent(fileName, output, kinds);
				}
				else if (ShowILCodeFlag || ShowILSequencePointsFlag)
				{
					if (outputDirectory != null)
					{
						string outputName = Path.GetFileNameWithoutExtension(fileName);
						output = File.CreateText(Path.Combine(outputDirectory, outputName) + ".il");
					}

					return ShowIL(fileName, output);
				}
				else if (CreateDebugInfoFlag)
				{
					string? pdbFileName = null;
					if (outputDirectory != null)
					{
						string outputName = Path.GetFileNameWithoutExtension(fileName);
						pdbFileName = Path.Combine(outputDirectory, outputName) + ".pdb";
					}
					else
					{
						pdbFileName = Path.ChangeExtension(fileName, ".pdb");
					}

					return GeneratePdbForAssembly(fileName, pdbFileName, app);
				}
				else if (DumpPackageFlag)
				{
					return DumpPackageAssemblies(fileName, outputDirectory, app);
				}
				else
				{
					if (outputDirectory != null)
					{
						string outputName = Path.GetFileNameWithoutExtension(fileName);
						output = File.CreateText(Path.Combine(outputDirectory,
							(string.IsNullOrEmpty(TypeName) ? outputName : TypeName) + ".decompiled.cs"));
					}

					return Decompile(fileName, output, TypeName);
				}
			}
		}

		private static string ResolveOutputDirectory(string outputDirectory)
		{
			// path is not set
			if (string.IsNullOrWhiteSpace(outputDirectory))
				return null;
			// resolve relative path, backreferences ('.' and '..') and other
			// platform-specific path elements, like '~'.
			return Path.GetFullPath(outputDirectory);
		}

		DecompilerSettings GetSettings(PEFile module)
		{
			return new DecompilerSettings(LanguageVersion) {
				ThrowOnAssemblyResolveErrors = false,
				RemoveDeadCode = RemoveDeadCode,
				RemoveDeadStores = RemoveDeadStores,
				UseSdkStyleProjectFormat = WholeProjectDecompiler.CanUseSdkStyleProjectFormat(module),
				UseNestedDirectoriesForNamespaces = NestedDirectories,
			};
		}

		CSharpDecompiler GetDecompiler(string assemblyFileName)
		{
			var module = new PEFile(assemblyFileName);
			var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Metadata.DetectTargetFrameworkId());
			foreach (var path in (ReferencePaths ?? Array.Empty<string>()))
			{
				resolver.AddSearchDirectory(path);
			}
			return new CSharpDecompiler(assemblyFileName, resolver, GetSettings(module)) {
				DebugInfoProvider = TryLoadPDB(module)
			};
		}

		int ListContent(string assemblyFileName, TextWriter output, ISet<TypeKind> kinds)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);

			foreach (var type in decompiler.TypeSystem.MainModule.TypeDefinitions)
			{
				if (!kinds.Contains(type.Kind))
					continue;
				output.WriteLine($"{type.Kind} {type.FullName}");
			}
			return 0;
		}

		int ShowIL(string assemblyFileName, TextWriter output)
		{
			var module = new PEFile(assemblyFileName);
			output.WriteLine($"// IL code: {module.Name}");
			var disassembler = new ReflectionDisassembler(new PlainTextOutput(output), CancellationToken.None) {
				DebugInfo = TryLoadPDB(module),
				ShowSequencePoints = ShowILSequencePointsFlag,
			};
			disassembler.WriteModuleContents(module);
			return 0;
		}

		ProjectId DecompileAsProject(string assemblyFileName, string projectFileName)
		{
			var module = new PEFile(assemblyFileName);
			var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Metadata.DetectTargetFrameworkId());
			foreach (var path in (ReferencePaths ?? Array.Empty<string>()))
			{
				resolver.AddSearchDirectory(path);
			}
			var decompiler = new WholeProjectDecompiler(GetSettings(module), resolver, null, resolver, TryLoadPDB(module));
			using (var projectFileWriter = new StreamWriter(File.OpenWrite(projectFileName)))
				return decompiler.DecompileProject(module, Path.GetDirectoryName(projectFileName), projectFileWriter);
		}

		int Decompile(string assemblyFileName, TextWriter output, string? typeName = null)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);

			if (typeName == null)
			{
				output.Write(decompiler.DecompileWholeModuleAsString());
			}
			else
			{
				var name = new FullTypeName(typeName);
				output.Write(decompiler.DecompileTypeAsString(name));
			}
			return 0;
		}

		int GeneratePdbForAssembly(string assemblyFileName, string pdbFileName, CommandLineApplication app)
		{
			var module = new PEFile(assemblyFileName,
				new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read),
				PEStreamOptions.PrefetchEntireImage,
				metadataOptions: MetadataReaderOptions.None);

			if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(module))
			{
				app.Error.WriteLine($"Cannot create PDB file for {assemblyFileName}, because it does not contain a PE Debug Directory Entry of type 'CodeView'.");
				return ProgramExitCodes.EX_DATAERR;
			}

			using (FileStream stream = new FileStream(pdbFileName, FileMode.OpenOrCreate, FileAccess.Write))
			{
				var decompiler = GetDecompiler(assemblyFileName);
				PortablePdbWriter.WritePdb(module, decompiler, GetSettings(module), stream);
			}

			return 0;
		}

		int DumpPackageAssemblies(string packageFileName, string outputDirectory, CommandLineApplication app)
		{
			using (var memoryMappedPackage = MemoryMappedFile.CreateFromFile(packageFileName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
			{
				using (var packageView = memoryMappedPackage.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
				{
					if (!SingleFileBundle.IsBundle(packageView, out long bundleHeaderOffset))
					{
						app.Error.WriteLine($"Cannot dump assembiles for {packageFileName}, because it is not a single file bundle.");
						return ProgramExitCodes.EX_DATAERR;
					}

					var manifest = SingleFileBundle.ReadManifest(packageView, bundleHeaderOffset);
					foreach (var entry in manifest.Entries)
					{
						Stream contents;

						if (entry.RelativePath.Replace('\\', '/').Contains("../", StringComparison.Ordinal) || Path.IsPathRooted(entry.RelativePath))
						{
							app.Error.WriteLine($"Skipping single-file entry '{entry.RelativePath}' because it might refer to a location outside of the bundle output directory.");
							continue;
						}

						if (entry.CompressedSize == 0)
						{
							contents = new UnmanagedMemoryStream(packageView.SafeMemoryMappedViewHandle, entry.Offset, entry.Size);
						}
						else
						{
							Stream compressedStream = new UnmanagedMemoryStream(packageView.SafeMemoryMappedViewHandle, entry.Offset, entry.CompressedSize);
							Stream decompressedStream = new MemoryStream((int)entry.Size);
							using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
							{
								deflateStream.CopyTo(decompressedStream);
							}

							if (decompressedStream.Length != entry.Size)
							{
								app.Error.WriteLine($"Corrupted single-file entry '{entry.RelativePath}'. Declared decompressed size '{entry.Size}' is not the same as actual decompressed size '{decompressedStream.Length}'.");
								return ProgramExitCodes.EX_DATAERR;
							}

							decompressedStream.Seek(0, SeekOrigin.Begin);
							contents = decompressedStream;
						}

						string target = Path.Combine(outputDirectory, entry.RelativePath);
						Directory.CreateDirectory(Path.GetDirectoryName(target));
						using (var fileStream = File.Create(target))
						{
							contents.CopyTo(fileStream);
						}
					}
				}
			}

			return 0;
		}

		IDebugInfoProvider TryLoadPDB(PEFile module)
		{
			if (InputPDBFile.IsSet)
			{
				if (InputPDBFile.Value == null)
					return DebugInfoUtils.LoadSymbols(module);
				return DebugInfoUtils.FromFile(module, InputPDBFile.Value);
			}

			return null;
		}
	}
}
