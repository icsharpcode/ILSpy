using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Console
{
	class Program
	{
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
			var typeOption = app.Option("-t|--type <type-name>", "The FQN of the type to decompile.", CommandOptionType.SingleValue);
			var listOption = app.Option("-l|--list <entity-type(s)>", "Lists all entities of the specified type(s). Valid types: c(lass), i(interface), s(truct), d(elegate), e(num)", CommandOptionType.MultipleValue);
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
					return -1;
				}
				if (!outputOption.HasValue() && projectOption.HasValue()) {
					app.Error.WriteLine($"ERROR: Output directory not speciified.");
					return -1;
				}
				if (outputOption.HasValue() && !Directory.Exists(outputOption.Value())) {
					app.Error.WriteLine($"ERROR: Output directory '{outputOption.Value()}' does not exist.");
					return -1;
				}
				if (projectOption.HasValue()) {
					DecompileAsProject(inputAssemblyFileName.Value, outputOption.Value());
				} else if (listOption.HasValue()) {
					var values = listOption.Values.SelectMany(v => v.Split(',', ';')).ToArray();
					HashSet<TypeKind> kinds = TypesParser.ParseSelection(values);
					TextWriter output = System.Console.Out;
					if (outputOption.HasValue()) {
						string directory = outputOption.Value();
						string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
						output = File.CreateText(Path.Combine(directory, outputName) + ".list.txt");
					}
					ListContent(inputAssemblyFileName.Value, output, kinds);
				} else {
					TextWriter output = System.Console.Out;
					if (outputOption.HasValue()) {
						string directory = outputOption.Value();
						string outputName = Path.GetFileNameWithoutExtension(inputAssemblyFileName.Value);
						output = File.CreateText(Path.Combine(directory, (typeOption.Value() ?? outputName) + ".decompiled.cs"));
					}
					Decompile(inputAssemblyFileName.Value, output, typeOption.Value());
				}
				return 0;
			});

			return app.Execute(args);
		}

		static CSharpDecompiler GetDecompiler(string assemblyFileName)
		{
			return new CSharpDecompiler(assemblyFileName, new DecompilerSettings() {  ThrowOnAssemblyResolveErrors = false });
		}

		static void ListContent(string assemblyFileName, TextWriter output, ISet<TypeKind> kinds)
		{
			CSharpDecompiler decompiler = GetDecompiler(assemblyFileName);

			foreach (var type in decompiler.TypeSystem.Compilation.MainAssembly.GetAllTypeDefinitions()) {
				if (!kinds.Contains(type.Kind))
					continue;
				output.WriteLine($"{type.Kind} {type.FullName}");
			}
		}

		static void DecompileAsProject(string assemblyFileName, string outputDirectory)
		{
			ModuleDefinition module = UniversalAssemblyResolver.LoadMainModule(assemblyFileName);
			WholeProjectDecompiler decompiler = new WholeProjectDecompiler();
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
	}
}
