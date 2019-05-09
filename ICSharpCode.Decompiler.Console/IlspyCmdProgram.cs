using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Disassembler;
using System.Threading;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.DebugInfo;
// ReSharper disable All

namespace ICSharpCode.Decompiler.Console
{
	[Command(Name = "ilspycmd", Description = "dotnet tool for decompiling .NET assemblies and generating portable PDBs",
		ExtendedHelpText = @"
Remarks:
  -o is valid with every option and required when using -p.
")]
	[HelpOption("-h|--help")]
	[ProjectOptionRequiresOutputDirectoryValidationAttribute]
	class ILSpyCmdProgram
	{
		public static int Main(string[] args) => CommandLineApplication.Execute<ILSpyCmdProgram>(args);

		[FileExists]
		[Required]
		[Argument(0, "Assembly file name", "The assembly that is being decompiled. This argument is mandatory.")]
		public string InputAssemblyName { get; }

		[DirectoryExists]
		[Option("-o|--outputdir <directory>", "The output directory, if omitted decompiler output is written to standard out.", CommandOptionType.SingleValue)]
		public string OutputDirectory { get; }

		[Option("-p|--project", "Decompile assembly as compilable project. This requires the output directory option.", CommandOptionType.NoValue)]
		public bool CreateCompilableProjectFlag { get; }

		[Option("-t|--type <type-name>", "The fully qualified name of the type to decompile.", CommandOptionType.SingleValue)]
		public string TypeName { get; }

		[Option("-il|--ilcode", "Show IL code.", CommandOptionType.NoValue)]
		public bool ShowILCodeFlag { get; }

		[Option("-d|--debuginfo", "Generate PDB.", CommandOptionType.NoValue)]
		public bool CreateDebugInfoFlag { get; }

		[Option("-l|--list <entity-type(s)>", "Lists all entities of the specified type(s). Valid types: c(lass), i(interface), s(truct), d(elegate), e(num)", CommandOptionType.MultipleValue)]
		public string[] EntityTypes { get; } = new string[0];

		[Option("-v|--version", "Show version of ICSharpCode.Decompiler used.", CommandOptionType.NoValue)]
		public bool ShowVersion { get; }

		[DirectoryExists]
		[Option("-r|--referencepath <path>", "Path to a directory containing dependencies of the assembly that is being decompiled.", CommandOptionType.MultipleValue)]
		public string[] ReferencePaths { get; } = new string[0];

		private int OnExecute(CommandLineApplication app)
		{
			TextWriter output = System.Console.Out;
			bool outputDirectorySpecified = !String.IsNullOrEmpty(OutputDirectory);

			try {
				if (CreateCompilableProjectFlag) {
					DecompileAsProject(InputAssemblyName, OutputDirectory, ReferencePaths);
				} else if (EntityTypes.Any()) {
					var values = EntityTypes.SelectMany(v => v.Split(',', ';')).ToArray();
					HashSet<TypeKind> kinds = TypesParser.ParseSelection(values);
					if (outputDirectorySpecified) {
						string outputName = Path.GetFileNameWithoutExtension(InputAssemblyName);
						output = File.CreateText(Path.Combine(OutputDirectory, outputName) + ".list.txt");
					}

					ListContent(InputAssemblyName, output, kinds, ReferencePaths);
				} else if (ShowILCodeFlag) {
					if (outputDirectorySpecified) {
						string outputName = Path.GetFileNameWithoutExtension(InputAssemblyName);
						output = File.CreateText(Path.Combine(OutputDirectory, outputName) + ".il");
					}

					ShowIL(InputAssemblyName, output, ReferencePaths);
				} else if (CreateDebugInfoFlag) {
					string pdbFileName = null;
					if (outputDirectorySpecified) {
						string outputName = Path.GetFileNameWithoutExtension(InputAssemblyName);
						pdbFileName = Path.Combine(OutputDirectory, outputName) + ".pdb";
					} else {
						pdbFileName = Path.ChangeExtension(InputAssemblyName, ".pdb");
					}

					return GeneratePdbForAssembly(InputAssemblyName, pdbFileName, ReferencePaths, app);
				} else if (ShowVersion) {
					string vInfo = "ilspycmd: " + typeof(ILSpyCmdProgram).Assembly.GetName().Version.ToString() +
					               Environment.NewLine
					               + "ICSharpCode.Decompiler: " +
					               typeof(FullTypeName).Assembly.GetName().Version.ToString();
					output.WriteLine(vInfo);
				} else {
					if (outputDirectorySpecified) {
						string outputName = Path.GetFileNameWithoutExtension(InputAssemblyName);
						output = File.CreateText(Path.Combine(OutputDirectory,
							(String.IsNullOrEmpty(TypeName) ? outputName : TypeName) + ".decompiled.cs"));
					}

					Decompile(InputAssemblyName, output, ReferencePaths, TypeName);
				}
			} catch (Exception ex) {
				app.Error.WriteLine(ex.ToString());
				return ProgramExitCodes.EX_SOFTWARE;
			} finally {
				output.Close();
			}

			return 0;
		}

		static CSharpDecompiler GetDecompiler(string assemblyFileName, string[] referencePaths)
		{
			var module = new PEFile(assemblyFileName);
			var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Reader.DetectTargetFrameworkId());
			foreach (var path in referencePaths) {
				resolver.AddSearchDirectory(path);
			}
			return new CSharpDecompiler(assemblyFileName, resolver, new DecompilerSettings());
		}

		static void ListContent(string assemblyFileName, TextWriter output, ISet<TypeKind> kinds, string[] referencePaths)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName, referencePaths);

			foreach (var type in decompiler.TypeSystem.MainModule.TypeDefinitions) {
				if (!kinds.Contains(type.Kind))
					continue;
				output.WriteLine($"{type.Kind} {type.FullName}");
			}
		}

		static void ShowIL(string assemblyFileName, TextWriter output, string[] referencePaths)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName, referencePaths);
			ITextOutput textOutput = new PlainTextOutput();
			ReflectionDisassembler disassembler = new ReflectionDisassembler(textOutput, CancellationToken.None);

			disassembler.DisassembleNamespace(decompiler.TypeSystem.MainModule.RootNamespace.Name,
				decompiler.TypeSystem.MainModule.PEFile,
				decompiler.TypeSystem.MainModule.TypeDefinitions.Select(x => (TypeDefinitionHandle)x.MetadataToken));

			output.WriteLine($"// IL code: {decompiler.TypeSystem.MainModule.AssemblyName}");
			output.WriteLine(textOutput.ToString());
		}

		static void DecompileAsProject(string assemblyFileName, string outputDirectory, string[] referencePaths)
		{
			var decompiler = new WholeProjectDecompiler();
			var module = new PEFile(assemblyFileName);
			var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Reader.DetectTargetFrameworkId());
			foreach (var path in referencePaths) {
				resolver.AddSearchDirectory(path);
			}
			decompiler.AssemblyResolver = resolver;
			decompiler.DecompileProject(module, outputDirectory);
		}

		static void Decompile(string assemblyFileName, TextWriter output, string[] referencePaths, string typeName = null)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName, referencePaths);

			if (typeName == null) {
				output.Write(decompiler.DecompileWholeModuleAsString());
			} else {
				var name = new FullTypeName(typeName);
				output.Write(decompiler.DecompileTypeAsString(name));
			}
		}

		static int GeneratePdbForAssembly(string assemblyFileName, string pdbFileName, string[] referencePaths, CommandLineApplication app)
		{
			var module = new PEFile(assemblyFileName,
				new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read),
				PEStreamOptions.PrefetchEntireImage,
				metadataOptions: MetadataReaderOptions.None);

			if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(module)) {
				app.Error.WriteLine($"Cannot create PDB file for {assemblyFileName}, because it does not contain a PE Debug Directory Entry of type 'CodeView'.");
				return ProgramExitCodes.EX_DATAERR;
			}

			using (FileStream stream = new FileStream(pdbFileName, FileMode.OpenOrCreate, FileAccess.Write)) {
				var decompiler = GetDecompiler(assemblyFileName, referencePaths);
				PortablePdbWriter.WritePdb(module, decompiler, new DecompilerSettings() { ThrowOnAssemblyResolveErrors = false }, stream);
			}

			return 0;
		}
	}
}
