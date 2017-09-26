using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using McMaster.Extensions.CommandLineUtils;
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

			app.HelpOption("-h|--help");
			var inputAssemblyFileName = app.Argument("Assembly filename name", "The assembly that is being decompiled. This argument is mandatory.");
			var projectOption = app.Option("-p|--project", "Decompile assembly as compilable project. This requires the output directory option.", CommandOptionType.NoValue);
			var outputOption = app.Option("-o|--outputdir <directory>", "The output directory, if omitted decompiler output is written to standard out.", CommandOptionType.SingleValue);
			var typeOption = app.Option("-t|--type <type-name>", "The FQN of the type to decompile.", CommandOptionType.SingleValue);
			var listOption = app.Option("-l|--list <entity-type(s)>", "Lists all entities of the specified type(s). Valid types: c(lass), i(interface), s(truct), d(elegate), e(num)", CommandOptionType.MultipleValue);
			var massOption = app.Option("-m|--mass", "Decompile all assemblies in that directory.", CommandOptionType.NoValue);
			app.ExtendedHelpText = Environment.NewLine + "-o is valid with every option and required when using -p.";

			app.ThrowOnUnexpectedArgument = false; // Ignore invalid arguments / options

			app.OnExecute(() => {
				// HACK : the CommandLineUtils package does not allow us to specify an argument as mandatory.
				// Therefore we're implementing it as simple as possible.
				if (inputAssemblyFileName.Value == null) {
					app.ShowHint();
					return -1;
				}
				if (massOption.HasValue() ? !Directory.Exists(inputAssemblyFileName.Value) : !File.Exists(inputAssemblyFileName.Value)) {
					app.Error.WriteLine($"ERROR: Input file '{inputAssemblyFileName.Value}' not found.");
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
				if (massOption.HasValue()) {
					RecursiveDecompile(inputAssemblyFileName.Value, inputAssemblyFileName.Value, outputOption.Value());
				} else if (projectOption.HasValue()) {
					DecompileAsProject(inputAssemblyFileName.Value, outputOption.Value());
				} else if (listOption.HasValue()) {
					var values = listOption.Values.SelectMany(v => v.Split(',', ';')).ToArray();
					HashSet<TypeKind> kinds = ParseSelection(values);
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
					Decompile(inputAssemblyFileName.Value, output);
				}
				return 0;
			});

			return app.Execute(args);
		}

		static void ListContent(string assemblyFileName, TextWriter output, ISet<TypeKind> kinds)
		{
			DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
			resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyFileName));
			resolver.RemoveSearchDirectory(".");

			var module = ModuleDefinition.ReadModule(assemblyFileName, new ReaderParameters {
				AssemblyResolver = resolver,
				InMemory = true
			});

			var typeSystem = new DecompilerTypeSystem(module);

			foreach (var type in typeSystem.MainAssembly.GetAllTypeDefinitions()) {
				if (!kinds.Contains(type.Kind))
					continue;
				output.WriteLine($"{type.Kind} {type.FullName}");
			}
		}

		static void DecompileAsProject(string assemblyFileName, string outputDirectory)
		{
			ModuleDefinition module = LoadModule(assemblyFileName);
			WholeProjectDecompiler decompiler = new WholeProjectDecompiler();
			decompiler.DecompileProject(module, outputDirectory);
		}

		static void Decompile(string assemblyFileName, TextWriter output, string typeName = null)
		{
			ModuleDefinition module = LoadModule(assemblyFileName);
			var typeSystem = new DecompilerTypeSystem(module);
			CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, new DecompilerSettings());

			decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			SyntaxTree syntaxTree;
			if (typeName == null)
				syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
			else
				syntaxTree = decompiler.DecompileTypes(module.GetTypes().Where(td => string.Equals(td.FullName, typeName, StringComparison.OrdinalIgnoreCase)));

			var visitor = new CSharpOutputVisitor(output, FormattingOptionsFactory.CreateSharpDevelop());
			syntaxTree.AcceptVisitor(visitor);
		}

		static HashSet<TypeKind> ParseSelection(string[] values)
		{
			var possibleValues = new Dictionary<string, TypeKind>(StringComparer.OrdinalIgnoreCase) { ["class"] = TypeKind.Class, ["struct"] = TypeKind.Struct, ["interface"] = TypeKind.Interface, ["enum"] = TypeKind.Enum, ["delegate"] = TypeKind.Delegate };
			HashSet<TypeKind> kinds = new HashSet<TypeKind>();
			if (values.Length == 1 && !possibleValues.Keys.Any(v => values[0].StartsWith(v, StringComparison.OrdinalIgnoreCase))) {
				foreach (char ch in values[0]) {
					switch (ch) {
						case 'c':
							kinds.Add(TypeKind.Class);
							break;
						case 'i':
							kinds.Add(TypeKind.Interface);
							break;
						case 's':
							kinds.Add(TypeKind.Struct);
							break;
						case 'd':
							kinds.Add(TypeKind.Delegate);
							break;
						case 'e':
							kinds.Add(TypeKind.Enum);
							break;
					}
				}
			} else {
				foreach (var value in values) {
					string v = value;
					while (v.Length > 0 && !possibleValues.ContainsKey(v))
						v = v.Remove(v.Length - 1);
					if (possibleValues.TryGetValue(v, out var kind))
						kinds.Add(kind);
				}
			}
			return kinds;
		}

		static ModuleDefinition LoadModule(string assemblyFileName)
		{
			var resolver = new CustomAssemblyResolver(assemblyFileName);

			var module = ModuleDefinition.ReadModule(assemblyFileName, new ReaderParameters {
				AssemblyResolver = resolver,
				InMemory = true
			});

			resolver.TargetFramework = module.Assembly.DetectTargetFrameworkId();

			return module;
		}

		// from https://stackoverflow.com/a/32113484
		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetRelativePath(string fromPath, string toPath)
		{
			if (string.IsNullOrEmpty(fromPath)) {
				throw new ArgumentNullException("fromPath");
			}

			if (string.IsNullOrEmpty(toPath)) {
				throw new ArgumentNullException("toPath");
			}

			Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
			Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

			if (fromUri.Scheme != toUri.Scheme) {
				return toPath;
			}

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)) {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

		private static string AppendDirectorySeparatorChar(string path)
		{
			// Append a slash only if the path is a directory and does not have a slash.
			if (!Path.HasExtension(path) &&
				!path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				return path + Path.DirectorySeparatorChar;
			}

			return path;
		}

		static void RecursiveDecompile(string sDir, string baseInputDir, string baseOutputDir)
		{
			try {
				foreach (string f in Directory.GetFiles(sDir)) {
					if (f.EndsWith(".exe") || f.EndsWith(".dll")) {
						string newDir = GetRelativePath(baseInputDir, f);
						newDir = newDir.Remove(newDir.Length - 4, 4);
						newDir = Path.Combine(baseOutputDir, newDir);
						Directory.CreateDirectory(newDir);
						ConsoleColor oldColor = System.Console.ForegroundColor;
						System.Console.ForegroundColor = ConsoleColor.Blue;
						System.Console.WriteLine("Decompiling " + f + " to " + newDir);
						System.Console.ForegroundColor = oldColor;
						DecompileAsProject(f, newDir);
					}
				}
				foreach (string d in Directory.GetDirectories(sDir)) {
					RecursiveDecompile(d, baseInputDir, baseOutputDir);
				}
			} catch (System.Exception excpt) {
				System.Console.WriteLine(excpt.Message);
			}
		}

	}
}
