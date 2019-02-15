using System;
using System.Collections.Generic;
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
// ReSharper disable InconsistentNaming

namespace ICSharpCode.Decompiler.Console
{
	class Program
	{
		// https://www.freebsd.org/cgi/man.cgi?query=sysexits&apropos=0&sektion=0&manpath=FreeBSD+4.3-RELEASE&format=html
		private const int EX_USAGE = 64;
		private const int EX_DATAERR = 65;
		private const int EX_NOINPUT = 66;
		private const int EX_SOFTWARE = 70;

		static int Main(string[] args)
		{
			// https://github.com/natemcmaster/CommandLineUtils/
			// Older cmd line clients (for options reference): https://github.com/aerror2/ILSpy-For-MacOSX and https://github.com/andreif/ILSpyMono
			var app = new CommandLineApplication();

			app.LongVersionGetter = () => "ilspycmd " + typeof(FullTypeName).Assembly.GetName().Version.ToString();
			app.HelpOption("-h|--help");
			var inputAssemblyFileName = app.Argument("Assembly filename name", "The assembly that is being decompiled. This argument is mandatory.");
			var projectOption = app.Option("-p|--project", "Decompile assembly as compilable project. This requires the output directory option.", CommandOptionType.NoValue);
			var outputOption = app.Option("-o|--outputdir <directory>", "The output directory, if omitted decompiler output is written to standard out.", CommandOptionType.SingleValue);
			var typeOption = app.Option("-t|--type <type-name>", "The fully qualified name of the type to decompile.", CommandOptionType.SingleValue);
			var listOption = app.Option("-l|--list <entity-type(s)>", "Lists all entities of the specified type(s). Valid types: c(lass), i(interface), s(truct), d(elegate), e(num)", CommandOptionType.MultipleValue);
			var ilViewerOption = app.Option("-il|--ilcode", "Show IL code.", CommandOptionType.NoValue);
			var pdbGeneration = app.Option("-d|--debuginfo", "Generate PDB.", CommandOptionType.NoValue);
			app.ExtendedHelpText = Environment.NewLine + "-o is valid with every option and required when using -p.";

			app.ThrowOnUnexpectedArgument = false; // Ignore invalid arguments / options

			app.OnExecute(() => {
				// HACK : the CommandLineUtils package does not allow us to specify an argument as mandatory.
				// Therefore we're implementing it as simple as possible.
				if (inputAssemblyFileName.Value == null) {
					app.ShowVersion();
					app.ShowHint();
					return -1;
				}
				if (!File.Exists(inputAssemblyFileName.Value)) {
					app.Error.WriteLine($"ERROR: Input file not found.");
					return EX_NOINPUT;
				}
				if (!outputOption.HasValue() && projectOption.HasValue()) {
					app.Error.WriteLine($"ERROR: Output directory not speciified.");
					return EX_USAGE;
				}
				if (outputOption.HasValue() && !Directory.Exists(outputOption.Value())) {
					app.Error.WriteLine($"ERROR: Output directory '{outputOption.Value()}' does not exist.");
					return EX_NOINPUT;
				}
				TextWriter output = System.Console.Out;
				try {
					if (projectOption.HasValue()) {
						DecompileAsProject(inputAssemblyFileName.Value, outputOption.Value());
					} else if (listOption.HasValue()) {
						var values = listOption.Values.SelectMany(v => v.Split(',', ';')).ToArray();
						HashSet<TypeKind> kinds = TypesParser.ParseSelection(values);
						if (outputOption.HasValue()) {
							string directory = outputOption.Value();
							string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
							output = File.CreateText(Path.Combine(directory, outputName) + ".list.txt");
						}
						ListContent(inputAssemblyFileName.Value, output, kinds);
					} else if (ilViewerOption.HasValue()) {
						if (outputOption.HasValue()) {
							string directory = outputOption.Value();
							string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
							output = File.CreateText(Path.Combine(directory, outputName) + ".il");
						}
						ShowIL(inputAssemblyFileName.Value, output);
					} else if (pdbGeneration.HasValue()) {
						string pdbFileName = null;
						if (outputOption.HasValue()) {
							string directory = outputOption.Value();
							string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
							pdbFileName = Path.Combine(directory, outputName) + ".pdb";
						} else {
							pdbFileName = Path.ChangeExtension(inputAssemblyFileName.Value, ".pdb");
						}
						return GeneratePdbForAssembly(inputAssemblyFileName.Value, pdbFileName, app);
					} else {
						if (outputOption.HasValue()) {
							string directory = outputOption.Value();
							string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
							output = File.CreateText(Path.Combine(directory, (typeOption.Value() ?? outputName) + ".decompiled.cs"));
						}
						Decompile(inputAssemblyFileName.Value, output, typeOption.Value());
					}
				} finally {
					output.Close();
				}
				// do not use Console here!
				return 0;
			});

			return app.Execute(args);
		}

		static CSharpDecompiler GetDecompiler(string assemblyFileName)
		{
			return new CSharpDecompiler(assemblyFileName, new DecompilerSettings() { ThrowOnAssemblyResolveErrors = false });
		}

		static void ListContent(string assemblyFileName, TextWriter output, ISet<TypeKind> kinds)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);

			foreach (var type in decompiler.TypeSystem.MainModule.TypeDefinitions) {
				if (!kinds.Contains(type.Kind))
					continue;
				output.WriteLine($"{type.Kind} {type.FullName}");
			}
		}

		static void ShowIL(string assemblyFileName, TextWriter output)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);
			ITextOutput textOutput = new PlainTextOutput();
			ReflectionDisassembler disassembler = new ReflectionDisassembler(textOutput, CancellationToken.None);

			disassembler.DisassembleNamespace(decompiler.TypeSystem.MainModule.RootNamespace.Name,
				decompiler.TypeSystem.MainModule.PEFile,
				decompiler.TypeSystem.MainModule.TypeDefinitions.Select(x => (TypeDefinitionHandle)x.MetadataToken));

			output.WriteLine($"// IL code: {decompiler.TypeSystem.MainModule.AssemblyName}");
			output.WriteLine(textOutput.ToString());
		}

		static void DecompileAsProject(string assemblyFileName, string outputDirectory)
		{
			WholeProjectDecompiler decompiler = new WholeProjectDecompiler();
			var module = new PEFile(assemblyFileName);
			decompiler.AssemblyResolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Reader.DetectTargetFrameworkId());
			decompiler.DecompileProject(module, outputDirectory);
		}

		static void Decompile(string assemblyFileName, TextWriter output, string typeName = null)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);

			if (typeName == null) {
				output.Write(decompiler.DecompileWholeModuleAsString());
			} else {
				var name = new FullTypeName(typeName);
				output.Write(decompiler.DecompileTypeAsString(name));
			}
		}

		static int GeneratePdbForAssembly(string assemblyFileName, string pdbFileName, CommandLineApplication app)
		{
			var module = new PEFile(assemblyFileName,
				new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read),
				PEStreamOptions.PrefetchEntireImage,
				metadataOptions: MetadataReaderOptions.None);

			if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(module)) {
				app.Error.WriteLine($"Cannot create PDB file for {assemblyFileName}, because it does not contain a PE Debug Directory Entry of type 'CodeView'.");
				return EX_DATAERR;
			}

			using (FileStream stream = new FileStream(pdbFileName, FileMode.OpenOrCreate, FileAccess.Write)) {
				try {
					var decompiler = GetDecompiler(assemblyFileName);
					PortablePdbWriter.WritePdb(module, decompiler, new DecompilerSettings() { ThrowOnAssemblyResolveErrors = false }, stream);
				} catch (Exception ex) {
					app.Error.WriteLine($"Cannot create PDB file for {assemblyFileName}");
					app.Error.WriteLine(ex.ToString());
					return EX_SOFTWARE;
				}
			}

			return 0;
		}
	}
}
