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
using System.Threading;
using System.Threading.Tasks;

using ConsoleAppFramework;

using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// The verb-less command line, "ilspycmd [options] &lt;assemblies&gt;", as ConsoleAppFramework's
	/// root command. Everything the user can type without a verb is declared on <see cref="Run"/>
	/// and bound into an <see cref="ILSpyCmdProgram"/>. Verbs are added next to it with their own
	/// names, so retiring the verb-less form means retiring this class and
	/// <see cref="LegacyArguments"/>.
	/// </summary>
	public sealed class LegacyCommand
	{
		public static async Task<int> InvokeAsync(string[] args)
		{
			if (!LegacyArguments.TryRewrite(args, out string[] rewritten, out string error))
			{
				Console.Error.WriteLine(error);
				return ProgramExitCodes.EX_USAGE;
			}

			// ConsoleAppFramework's Run methods return nothing: a command's int result and the
			// code of a usage error both go to Environment.ExitCode, which is sticky across runs
			// in one process and therefore reset here.
			Environment.ExitCode = 0;
			// The default error logger writes to standard output.
			ConsoleApp.LogError = message => Console.Error.WriteLine(message);
			ConsoleApp.Version = ILSpyCmdProgram.DecompilerVersion;

			var app = ConsoleApp.Create();
			app.UseFilter<UsageErrorFilter>();
			app.Add<LegacyCommand>();
			await app.RunAsync(rewritten);
			return Environment.ExitCode;
		}

		/// <summary>
		/// dotnet tool for decompiling .NET assemblies and generating portable PDBs
		/// <para>Remarks: -o is valid with every option and required when using -p or -d.</para>
		/// <para>Examples:<br/>
		/// Decompile assembly to console out: ilspycmd sample.dll<br/>
		/// Decompile assembly to destination directory (single C# file): ilspycmd -o c:\decompiled sample.dll<br/>
		/// Decompile assembly to destination directory, create a project file, one source file per type: ilspycmd -p -o c:\decompiled sample.dll<br/>
		/// Decompile assembly to destination directory, create a project file, one source file per type, into nicely nested directories: ilspycmd --nested-directories -p -o c:\decompiled sample.dll<br/>
		/// Generate a HTML diagrammer containing all type info into a folder next to the input assembly: ilspycmd sample.dll --generate-diagrammer<br/>
		/// Generate a HTML diagrammer containing filtered type info into a custom output folder (including types in the LightJson namespace while excluding types in nested LightJson.Serialization namespace): ilspycmd sample.dll --generate-diagrammer -o c:\diagrammer --generate-diagrammer-include LightJson\\..+ --generate-diagrammer-exclude LightJson\\.Serialization\\..+<br/>
		/// List all embedded resources in a WPF assembly (including BAML entries inside .g.resources containers): ilspycmd sample.dll --list-resources<br/>
		/// Extract a single resource. If the name ends with .baml, the output is decompiled XAML; otherwise raw bytes: ilspycmd sample.dll --resource sample.g.resources/mainwindow.baml -o c:\decompiled<br/>
		/// Decompile assembly as a compilable project. BAML resources become XAML Page items: ilspycmd sample.dll -p -o c:\decompiled</para>
		/// </summary>
		/// <param name="version">-v, Show version of ICSharpCode.Decompiler used.</param>
		/// <param name="outputdir">-o, The output directory, if omitted decompiler output is written to standard out.</param>
		/// <param name="project">-p, Decompile assembly as compilable project. This requires the output directory option.</param>
		/// <param name="type">-t, The fully qualified name of the type to decompile.</param>
		/// <param name="member">-m, The single member to decompile: an XML documentation id string (e.g. "M:System.String.Concat(System.String,System.String)", as also accepted by the UI's --navigateto option) or a metadata token (e.g. 0x06000005).</param>
		/// <param name="ilcode">-il, Show IL code.</param>
		/// <param name="ilSequencePoints">Show IL with sequence points. Implies -il.</param>
		/// <param name="generatePdb">-genpdb, Generate PDB.</param>
		/// <param name="useVarnamesFromPdb">-usepdb, Use variable names from PDB. Name the file as -usepdb:file; -usepdb alone reads the PDB next to the assembly.</param>
		/// <param name="list">-l, Lists all entities of the specified type(s). Valid types: c(lass), i(nterface), s(truct), d(elegate), e(num)</param>
		/// <param name="listResources">Lists all embedded resources in the assembly. Entries inside .resources containers are listed individually as '&lt;container&gt;/&lt;entry&gt;'.</param>
		/// <param name="resource">Extract a single resource by name (as printed by --list-resources). Resources whose name ends with '.baml' are decompiled to XAML.</param>
		/// <param name="decompileBaml">Deprecated: -p decompiles BAML resources to XAML files (Page items) on its own. Accepted so that existing scripts keep working.</param>
		/// <param name="dumpTable">Dump a metadata table: prints RID, token, names, heap offsets and coded indexes of every row. The value is the ECMA-335 table name (e.g. TypeDef, Property, MethodSemantics; case-insensitive) or table number (decimal or 0x-prefixed hex, e.g. 0x17).</param>
		/// <param name="dumpPdb">Dump the debug information of the assembly's PDB: documents, per-method sequence points, the scope tree with its variables, constants and imports, and custom debug information. Reads both Portable and Windows PDBs. Use -usepdb:file to point at a PDB that is not next to the assembly.</param>
		/// <param name="json">Output as JSON. Only supported together with --dump-table or --dump-pdb.</param>
		/// <param name="languageversion">-lv, C# Language version: CSharp1, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, CSharp10_0, CSharp11_0, CSharp12_0, CSharp13_0, Preview or Latest</param>
		/// <param name="ilspySettingsfile">Path to an ILSpy settings file.</param>
		/// <param name="decompilerSetting">-ds, Set a decompiler setting as name=value. Use multiple times to set multiple settings.</param>
		/// <param name="referencepath">-r, Path to a directory containing dependencies of the assembly that is being decompiled.</param>
		/// <param name="ignoreDecompilationErrors">Exit with success even when parts of the assembly could not be decompiled. The affected code carries the error text in the output and the failures are listed on stderr either way; only the exit status changes.</param>
		/// <param name="noDeadCode">Remove dead code.</param>
		/// <param name="noDeadStores">Remove dead stores.</param>
		/// <param name="dumpPackage">-d, Dump package assemblies into a folder. This requires the output directory option.</param>
		/// <param name="bundleEntry">The assembly inside a single-file bundle (or other package) to work on, as printed when such a file is passed without this option. Ignored for input files that are not packages.</param>
		/// <param name="nestedDirectories">Use nested directories for namespaces.</param>
		/// <param name="disableUpdatecheck">If using ilspycmd in a tight loop or fully automated scenario, you might want to disable the automatic update check.</param>
		/// <param name="generateDiagrammer">Generates an interactive HTML diagrammer app from selected types in the target assembly - to the --outputdir or in a 'diagrammer' folder next to to the assembly by default.</param>
		/// <param name="generateDiagrammerInclude">An optional regular expression matching Type.FullName used to whitelist types to include in the generated diagrammer.</param>
		/// <param name="generateDiagrammerExclude">An optional regular expression matching Type.FullName used to blacklist types to exclude from the generated diagrammer.</param>
		/// <param name="generateDiagrammerReportExcluded">Outputs a report of types excluded from the generated diagrammer - whether by default because compiler-generated, explicitly by '--generate-diagrammer-exclude' or implicitly by '--generate-diagrammer-include'. You may find this useful to develop and debug your regular expressions.</param>
		/// <param name="generateDiagrammerDocs">The path or file:// URI of the XML file containing the target assembly's documentation comments. You only need to set this if a) you want your diagrams annotated with them and b) the file name differs from that of the assembly. To enable XML documentation output for your assembly, see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output</param>
		/// <param name="generateDiagrammerStripNamespaces">Optional space-separated namespace names that are removed for brevity from XML documentation comments. Note that the order matters: e.g. replace 'System.Collections' before 'System' to remove both of them completely.</param>
		/// <param name="generateDiagrammerJsonOnly">Whether to generate a model.json file instead of baking it into the HTML template. This is useful for the HTML/JS/CSS development loop.</param>
		/// <param name="inputAssemblyNames">The list of assemblies that is being decompiled. This argument is mandatory.</param>
#if DEBUG
		/// <param name="ilast">Show the decompiler's intermediate representation (ILAst) of method bodies, after the full IL transform pipeline. Select what to dump with --type or --member; without either, every method of the assembly is dumped.</param>
		/// <param name="afterTransform">Stop the IL transform pipeline after the named transform (or after the transform at the given 1-based pipeline index) and show the ILAst at that point. Implies --ilast. Pass an unknown name to list the pipeline.</param>
#endif
		[Command("")]
		public async Task<int> Run(
			bool version = false,
			string outputdir = null,
			bool project = false,
			string type = null,
			string member = null,
			bool ilcode = false,
			bool ilSequencePoints = false,
			bool generatePdb = false,
			string useVarnamesFromPdb = null,
			string[] list = null,
			bool listResources = false,
			string resource = null,
			bool decompileBaml = false,
			string dumpTable = null,
			bool dumpPdb = false,
			bool json = false,
			LanguageVersion languageversion = LanguageVersion.Latest,
			string ilspySettingsfile = null,
			string[] decompilerSetting = null,
			string[] referencepath = null,
			bool ignoreDecompilationErrors = false,
			bool noDeadCode = false,
			bool noDeadStores = false,
			bool dumpPackage = false,
			string bundleEntry = null,
			bool nestedDirectories = false,
			bool disableUpdatecheck = false,
			bool generateDiagrammer = false,
			string generateDiagrammerInclude = null,
			string generateDiagrammerExclude = null,
			bool generateDiagrammerReportExcluded = false,
			string generateDiagrammerDocs = null,
			string[] generateDiagrammerStripNamespaces = null,
			[Hidden] bool generateDiagrammerJsonOnly = false,
#if DEBUG
			bool ilast = false,
			string afterTransform = null,
#endif
			[Argument] params string[] inputAssemblyNames)
		{
			if (version)
			{
				Console.Out.WriteLine(ILSpyCmdProgram.DecompilerVersion);
				return 0;
			}

			var program = new ILSpyCmdProgram {
				InputAssemblyNames = inputAssemblyNames,
				OutputDirectory = outputdir,
				CreateCompilableProjectFlag = project,
				TypeName = type,
				MemberIdString = member,
				ShowILCodeFlag = ilcode,
				ShowILSequencePointsFlag = ilSequencePoints,
#if DEBUG
				ShowILAstFlag = ilast,
				AfterTransformName = afterTransform,
#endif
				CreateDebugInfoFlag = generatePdb,
				// LegacyArguments hands a bare -usepdb over as an empty value.
				InputPDBFile = (useVarnamesFromPdb != null, string.IsNullOrEmpty(useVarnamesFromPdb) ? null : useVarnamesFromPdb),
				EntityTypes = list ?? [],
				ListResourcesFlag = listResources,
				ResourceName = resource,
				DecompileBamlFlag = decompileBaml,
				DumpTableName = dumpTable,
				DumpPdbFlag = dumpPdb,
				JsonOutputFlag = json,
				LanguageVersion = languageversion,
				ILSpySettingsFile = ilspySettingsfile,
				DecompilerSettingOverrides = decompilerSetting ?? [],
				ReferencePaths = referencepath,
				IgnoreDecompilationErrorsFlag = ignoreDecompilationErrors,
				RemoveDeadCode = noDeadCode,
				RemoveDeadStores = noDeadStores,
				DumpPackageFlag = dumpPackage,
				BundleEntryName = bundleEntry,
				NestedDirectories = nestedDirectories,
				DisableUpdateCheck = disableUpdatecheck,
				GenerateDiagrammer = generateDiagrammer,
				Include = generateDiagrammerInclude,
				Exclude = generateDiagrammerExclude,
				ReportExcludedTypes = generateDiagrammerReportExcluded,
				XmlDocs = generateDiagrammerDocs,
				StrippedNamespaces = generateDiagrammerStripNamespaces,
				JsonOnly = generateDiagrammerJsonOnly,
			};
			return await program.RunAsync();
		}
	}

	/// <summary>
	/// ConsoleAppFramework answers a mistake on the command line with exit code 1; this command
	/// line answers it with EX_USAGE. Once a filter is registered the generated code leaves parse
	/// failures to the filter chain, which is what makes the code reachable here.
	/// </summary>
	sealed class UsageErrorFilter(ConsoleAppFilter next) : ConsoleAppFilter(next)
	{
		public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
		{
			try
			{
				await Next.InvokeAsync(context, cancellationToken);
			}
			catch (ArgumentParseFailedException ex)
			{
				ConsoleApp.LogError(ex.Message);
				Environment.ExitCode = ProgramExitCodes.EX_USAGE;
			}
		}
	}
}
