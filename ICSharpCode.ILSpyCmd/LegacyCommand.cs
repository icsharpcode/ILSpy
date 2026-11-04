// Copyright (c) 2026 Christoph Wille
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
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// The verb-less command line, "ilspycmd [options] &lt;assemblies&gt;": the root command's own
	/// options, argument and action. Everything the user can type without a verb is declared
	/// here and bound into an <see cref="ILSpyCmdProgram"/>. Verbs are subcommands of this root,
	/// so retiring the verb-less form means retiring this class, nothing else.
	/// </summary>
	sealed class LegacyCommand : RootCommand
	{
		const string ExtendedHelpText = @"
Remarks:
  -o is valid with every option and required when using -p or -d.

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

    Generate a HTML diagrammer containing all type info into a folder next to the input assembly
        ilspycmd sample.dll --generate-diagrammer

    Generate a HTML diagrammer containing filtered type info into a custom output folder
    (including types in the LightJson namespace while excluding types in nested LightJson.Serialization namespace)
        ilspycmd sample.dll --generate-diagrammer -o c:\diagrammer --generate-diagrammer-include LightJson\\..+ --generate-diagrammer-exclude LightJson\\.Serialization\\..+

    List all embedded resources in a WPF assembly (including BAML entries inside .g.resources containers).
        ilspycmd sample.dll --list-resources

    Extract a single resource. If the name ends with .baml, the output is decompiled XAML; otherwise raw bytes.
        ilspycmd sample.dll --resource sample.g.resources/mainwindow.baml -o c:\decompiled

    Decompile assembly as a compilable project. BAML resources become XAML Page items.
        ilspycmd sample.dll -p -o c:\decompiled
";

		public static async Task<int> InvokeAsync(string[] args)
		{
			var command = new LegacyCommand();
			ParseResult result = command.Parse(command.RewriteAttachedPdbFile(args), new ParserConfiguration {
				// -il, -lv, -ds, -genpdb and -usepdb are aliases in their own right, and -pd is a
				// mistake, not -p -d.
				EnablePosixBundling = false,
				// @file was never expanded on this command line.
				ResponseFileTokenReplacer = null,
			});
			if (result.Errors.Count > 0)
			{
				foreach (ParseError error in result.Errors)
				{
					Console.Error.WriteLine(error.Message);
				}
				return ProgramExitCodes.EX_USAGE;
			}
			return await result.InvokeAsync();
		}

		readonly Argument<string[]> inputAssemblyNames = new("Assembly file name(s)") {
			Description = "The list of assemblies that is being decompiled. This argument is mandatory.",
			Arity = ArgumentArity.OneOrMore,
		};

		readonly Option<string> outputDirectory = Value("--outputdir", "-o", "directory",
			"The output directory, if omitted decompiler output is written to standard out.");
		readonly Option<bool> createCompilableProject = Flag("--project", "-p",
			"Decompile assembly as compilable project. This requires the output directory option.");
		readonly Option<string> typeName = Value("--type", "-t", "type-name",
			"The fully qualified name of the type to decompile.");
		readonly Option<string> memberIdString = Value("--member", "-m", "doc-id-or-token",
			"The single member to decompile: an XML documentation id string (e.g. \"M:System.String.Concat(System.String,System.String)\", as also accepted by the UI's --navigateto option) or a metadata token (e.g. 0x06000005).");
		readonly Option<bool> showILCode = Flag("--ilcode", "-il", "Show IL code.");
		readonly Option<bool> showILSequencePoints = Flag("--il-sequence-points", null,
			"Show IL with sequence points. Implies -il.");
#if DEBUG
		readonly Option<bool> showILAst = Flag("--ilast", null,
			"Show the decompiler's intermediate representation (ILAst) of method bodies, after the full IL transform pipeline. Select what to dump with --type or --member; without either, every method of the assembly is dumped.");
		readonly Option<string> afterTransformName = Value("--after-transform", null, "name-or-index",
			"Stop the IL transform pipeline after the named transform (or after the transform at the given 1-based pipeline index) and show the ILAst at that point. Implies --ilast. Pass an unknown name to list the pipeline.");
#endif
		readonly Option<bool> createDebugInfo = Flag("--generate-pdb", "-genpdb", "Generate PDB.");
		readonly Option<bool> usePdb = Flag("--use-varnames-from-pdb", "-usepdb",
			"Use variable names from the PDB next to the assembly, or from the file attached as -usepdb:<file> or --use-varnames-from-pdb=<file>. A following bare token is an input file, not the PDB.");
		// The PDB file name is only ever attached with ':' or '='. Modelled as a zero-or-one value
		// on the switch itself, System.CommandLine would also take the next bare token
		// ("-usepdb sample.dll" would read the assembly as the PDB), so the switch is a flag and
		// RewriteAttachedPdbFile routes the attached spellings to this option before parsing.
		readonly Option<string> usePdbFile = new("--use-varnames-from-pdb-file") {
			Hidden = true,
		};
		readonly Option<string[]> entityTypes = Values("--list", "-l", "entity-type(s)",
			"Lists all entities of the specified type(s). Valid types: c(lass), i(nterface), s(truct), d(elegate), e(num)");
		readonly Option<bool> listResources = Flag("--list-resources", null,
			"Lists all embedded resources in the assembly. Entries inside .resources containers are listed individually as '<container>/<entry>'.");
		readonly Option<string> resourceName = Value("--resource", null, "name",
			"Extract a single resource by name (as printed by --list-resources). Resources whose name ends with '.baml' are decompiled to XAML.");
		readonly Option<bool> decompileBaml = Flag("--decompile-baml", null,
			"Deprecated: -p decompiles BAML resources to XAML files (Page items) on its own. Accepted so that existing scripts keep working.");
		readonly Option<string> dumpTableName = Value("--dump-table", null, "table",
			"Dump a metadata table: prints RID, token, names, heap offsets and coded indexes of every row. <table> is the ECMA-335 table name (e.g. TypeDef, Property, MethodSemantics; case-insensitive) or table number (decimal or 0x-prefixed hex, e.g. 0x17).");
		readonly Option<bool> dumpPdb = Flag("--dump-pdb", null,
			"Dump the debug information of the assembly's PDB: documents, per-method sequence points, the scope tree with its variables, constants and imports, and custom debug information. Reads both Portable and Windows PDBs. Use -usepdb:<file> to point at a PDB that is not next to the assembly.");
		readonly Option<bool> jsonOutput = Flag("--json", null,
			"Output as JSON. Only supported together with --dump-table or --dump-pdb.");
		readonly Option<Decompiler.CSharp.LanguageVersion> languageVersion = new("--languageversion", "-lv") {
			Description = "C# Language version: " + string.Join(", ", Enum.GetNames<Decompiler.CSharp.LanguageVersion>()),
			HelpName = "version",
			DefaultValueFactory = _ => Decompiler.CSharp.LanguageVersion.Latest,
		};
		readonly Option<string> ilspySettingsFile = Value("--ilspy-settingsfile", null, "path",
			"Path to an ILSpy settings file.");
		readonly Option<string[]> decompilerSettingOverrides = Values("--decompiler-setting", "-ds", "name=value",
			"Set a decompiler setting. Use multiple times to set multiple settings.");
		readonly Option<string[]> referencePaths = Values("--referencepath", "-r", "path",
			"Path to a directory containing dependencies of the assembly that is being decompiled.");
		readonly Option<bool> ignoreDecompilationErrors = Flag("--ignore-decompilation-errors", null,
			"Exit with success even when parts of the assembly could not be decompiled. " +
			"The affected code carries the error text in the output and the failures are listed on stderr either way; " +
			"only the exit status changes.");
		readonly Option<bool> removeDeadCode = Flag("--no-dead-code", null, "Remove dead code.");
		readonly Option<bool> removeDeadStores = Flag("--no-dead-stores", null, "Remove dead stores.");
		readonly Option<bool> dumpPackage = Flag("--dump-package", "-d",
			"Dump package assemblies into a folder. This requires the output directory option.");
		readonly Option<string> bundleEntryName = Value("--bundle-entry", null, "name",
			"The assembly inside a single-file bundle (or other package) to work on, as printed when such a file is passed without this option. Ignored for input files that are not packages.");
		readonly Option<bool> nestedDirectories = Flag("--nested-directories", null,
			"Use nested directories for namespaces.");
		readonly Option<bool> disableUpdateCheck = Flag("--disable-updatecheck", null,
			"If using ilspycmd in a tight loop or fully automated scenario, you might want to disable the automatic update check.");

		const string generateDiagrammerCmd = "--generate-diagrammer",
			exclude = generateDiagrammerCmd + "-exclude",
			include = generateDiagrammerCmd + "-include";

		readonly Option<bool> generateDiagrammer = Flag(generateDiagrammerCmd, null,
			"Generates an interactive HTML diagrammer app from selected types in the target assembly" +
			" - to the --outputdir or in a 'diagrammer' folder next to to the assembly by default.");
		readonly Option<string> diagrammerInclude = Value(include, null, "regex",
			"An optional regular expression matching Type.FullName used to whitelist types to include in the generated diagrammer.");
		readonly Option<string> diagrammerExclude = Value(exclude, null, "regex",
			"An optional regular expression matching Type.FullName used to blacklist types to exclude from the generated diagrammer.");
		readonly Option<bool> diagrammerReportExcluded = Flag(generateDiagrammerCmd + "-report-excluded", null,
			"Outputs a report of types excluded from the generated diagrammer" +
			$" - whether by default because compiler-generated, explicitly by '{exclude}' or implicitly by '{include}'." +
			" You may find this useful to develop and debug your regular expressions.");
		readonly Option<string> diagrammerXmlDocs = Value(generateDiagrammerCmd + "-docs", null, "path",
			"The path or file:// URI of the XML file containing the target assembly's documentation comments." +
			" You only need to set this if a) you want your diagrams annotated with them and b) the file name differs from that of the assembly." +
			" To enable XML documentation output for your assembly, see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output");
		readonly Option<string[]> diagrammerStrippedNamespaces = Values(generateDiagrammerCmd + "-strip-namespaces", null, "namespace",
			"Optional space-separated namespace names that are removed for brevity from XML documentation comments." +
			" Note that the order matters: e.g. replace 'System.Collections' before 'System' to remove both of them completely.");
		// developer option, output is really only useful in combination with the corresponding task in html/gulpfile.js
		readonly Option<bool> diagrammerJsonOnly = new(generateDiagrammerCmd + "-json-only") {
			Arity = ArgumentArity.Zero,
			Hidden = true,
		};

		public LegacyCommand()
			: base("dotnet tool for decompiling .NET assemblies and generating portable PDBs")
		{
			// RootCommand comes with -h/-?//h//?, --version and the [suggest] directive;
			// this command line knows -h|--help and -v|--version only.
			Options.Clear();
			Directives.Clear();
			Options.Add(new Option<bool>("--version", "-v") {
				Description = "Show version of ICSharpCode.Decompiler used.",
				Arity = ArgumentArity.Zero,
				Action = new VersionAction(),
			});
			Options.Add(new HelpOption("--help", "-h") { Action = new LegacyHelpAction() });

			Option[] options = {
				outputDirectory, createCompilableProject, typeName, memberIdString, showILCode, showILSequencePoints,
#if DEBUG
				showILAst, afterTransformName,
#endif
				createDebugInfo, usePdb, usePdbFile, entityTypes, listResources, resourceName,
				decompileBaml, dumpTableName, dumpPdb, jsonOutput, languageVersion, ilspySettingsFile,
				decompilerSettingOverrides, referencePaths, ignoreDecompilationErrors, removeDeadCode, removeDeadStores,
				dumpPackage, bundleEntryName, nestedDirectories, disableUpdateCheck, generateDiagrammer,
				diagrammerInclude, diagrammerExclude, diagrammerReportExcluded, diagrammerXmlDocs,
				diagrammerStrippedNamespaces, diagrammerJsonOnly,
			};
			foreach (Option option in options)
			{
				Options.Add(option);
			}
			Arguments.Add(inputAssemblyNames);

			// A token the parser does not know as an option ends up in the positional argument;
			// reporting it as an option mistake is what the user meant by the leading dash.
			inputAssemblyNames.Validators.Add(result => {
				foreach (Token token in result.Tokens)
				{
					if (token.Value.StartsWith('-'))
						result.AddError($"Unrecognized option '{token.Value}'");
				}
			});
			// The parser's own message names the command, not the missing argument.
			Validators.Add(result => {
				if (result.GetResult(inputAssemblyNames) is null or { Tokens.Count: 0 })
					result.AddError("The Assembly file name(s) argument is required.");
			});

			SetAction(RunAsync);
		}

		/// <summary>
		/// "-usepdb:file", "-usepdb=file" and the long spellings become the hidden file option, with
		/// the delimiter kept. Every other token stays as it is, including a bare "-usepdb" that
		/// follows an option taking a value: that is the option's value, as it always was.
		/// </summary>
		string[] RewriteAttachedPdbFile(string[] args)
		{
			string[] spellings = [usePdb.Name, .. usePdb.Aliases];
			return args.Select(arg => {
				foreach (string spelling in spellings)
				{
					if (arg.Length > spelling.Length && arg.StartsWith(spelling, StringComparison.Ordinal) && arg[spelling.Length] is ':' or '=')
						return usePdbFile.Name + arg[spelling.Length..];
				}
				return arg;
			}).ToArray();
		}

		Task<int> RunAsync(ParseResult result, CancellationToken cancellationToken)
		{
			var program = new ILSpyCmdProgram {
				InputAssemblyNames = result.GetValue(inputAssemblyNames),
				OutputDirectory = result.GetValue(outputDirectory),
				CreateCompilableProjectFlag = result.GetValue(createCompilableProject),
				TypeName = result.GetValue(typeName),
				MemberIdString = result.GetValue(memberIdString),
				ShowILCodeFlag = result.GetValue(showILCode),
				ShowILSequencePointsFlag = result.GetValue(showILSequencePoints),
#if DEBUG
				ShowILAstFlag = result.GetValue(showILAst),
				AfterTransformName = result.GetValue(afterTransformName),
#endif
				CreateDebugInfoFlag = result.GetValue(createDebugInfo),
				InputPDBFile = (result.GetValue(usePdb) || result.GetResult(usePdbFile) != null, result.GetValue(usePdbFile)),
				EntityTypes = result.GetValue(entityTypes),
				ListResourcesFlag = result.GetValue(listResources),
				ResourceName = result.GetValue(resourceName),
				DecompileBamlFlag = result.GetValue(decompileBaml),
				DumpTableName = result.GetValue(dumpTableName),
				DumpPdbFlag = result.GetValue(dumpPdb),
				JsonOutputFlag = result.GetValue(jsonOutput),
				LanguageVersion = result.GetValue(languageVersion),
				ILSpySettingsFile = result.GetValue(ilspySettingsFile),
				DecompilerSettingOverrides = result.GetValue(decompilerSettingOverrides),
				ReferencePaths = result.GetValue(referencePaths),
				IgnoreDecompilationErrorsFlag = result.GetValue(ignoreDecompilationErrors),
				RemoveDeadCode = result.GetValue(removeDeadCode),
				RemoveDeadStores = result.GetValue(removeDeadStores),
				DumpPackageFlag = result.GetValue(dumpPackage),
				BundleEntryName = result.GetValue(bundleEntryName),
				NestedDirectories = result.GetValue(nestedDirectories),
				DisableUpdateCheck = result.GetValue(disableUpdateCheck),
				GenerateDiagrammer = result.GetValue(generateDiagrammer),
				Include = result.GetValue(diagrammerInclude),
				Exclude = result.GetValue(diagrammerExclude),
				ReportExcludedTypes = result.GetValue(diagrammerReportExcluded),
				XmlDocs = result.GetValue(diagrammerXmlDocs),
				StrippedNamespaces = result.GetValue(diagrammerStrippedNamespaces),
				JsonOnly = result.GetValue(diagrammerJsonOnly),
			};
			return program.RunAsync();
		}

		/// <summary>A switch that takes no value; "-p true" leaves "true" to the positional argument.</summary>
		static Option<bool> Flag(string name, string alias, string description)
		{
			return new Option<bool>(name, alias is null ? [] : [alias]) {
				Description = description,
				Arity = ArgumentArity.Zero,
			};
		}

		static Option<string> Value(string name, string alias, string helpName, string description)
		{
			return new Option<string>(name, alias is null ? [] : [alias]) {
				Description = description,
				HelpName = helpName,
			};
		}

		/// <summary>One value per occurrence, repeat the option for more.</summary>
		static Option<string[]> Values(string name, string alias, string helpName, string description)
		{
			return new Option<string[]>(name, alias is null ? [] : [alias]) {
				Description = description,
				HelpName = helpName,
			};
		}

		sealed class VersionAction : SynchronousCommandLineAction
		{
			// -v on its own must not trip over the missing assembly argument.
			public override bool ClearsParseErrors => true;

			public override int Invoke(ParseResult parseResult)
			{
				parseResult.InvocationConfiguration.Output.WriteLine(ILSpyCmdProgram.DecompilerVersion);
				return 0;
			}
		}

		/// <summary>The standard sections, framed by the version line and the remarks/examples block.</summary>
		sealed class LegacyHelpAction : SynchronousCommandLineAction
		{
			public override bool ClearsParseErrors => true;

			public override int Invoke(ParseResult parseResult)
			{
				TextWriter output = parseResult.InvocationConfiguration.Output;
				output.WriteLine(ILSpyCmdProgram.DecompilerVersion);
				output.WriteLine();
				new HelpAction().Invoke(parseResult);
				output.Write(ExtendedHelpText.TrimStart());
				return 0;
			}
		}
	}
}
