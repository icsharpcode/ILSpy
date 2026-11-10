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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CliFx;
using CliFx.Activation;
using CliFx.Binding;
using CliFx.Infrastructure;

using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// The verb-less command line, "ilspycmd [options] &lt;assemblies&gt;", as CliFx's default
	/// command. Everything the user can type without a verb is declared here and bound into an
	/// <see cref="ILSpyCmdProgram"/>. Verbs are named commands next to this one, so retiring the
	/// verb-less form means retiring this class and <see cref="LegacyArgumentRewriter"/>.
	/// </summary>
	[Command(Description = """
		dotnet tool for decompiling .NET assemblies and generating portable PDBs

		Remarks:
		  -o is valid with every option and required when using -p or -d.
		  Single-dash spellings longer than one letter are accepted for -il, -lv, -ds, -genpdb and -usepdb[:file].

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
		""")]
	partial class LegacyCommand : ICommand, ICommandWithVersionOption
	{
		public static async Task<int> InvokeAsync(string[] args)
		{
			if (!LegacyArgumentRewriter.TryRewrite(args, Descriptor, out string[] rewritten, out bool jsonOnly, out string error))
			{
				Console.Error.WriteLine(error);
				return ProgramExitCodes.EX_USAGE;
			}

			var command = new LegacyCommand { JsonOnly = jsonOnly };
			// CliFx's own console opens the process's standard handles directly. Routing its
			// output through Console.Out and Console.Error keeps it in order with the engine's
			// output and lets an in-process host capture it with Console.SetOut.
			using var console = new FakeConsole(output: new ConsoleStream(Console.Out), error: new ConsoleStream(Console.Error));
			int exitCode = await new CommandLineApplicationBuilder()
				.AddCommand(Descriptor)
				.SetTitle("ilspycmd")
				.SetExecutableName("ilspycmd")
				.SetVersion(ILSpyCmdProgram.DecompilerVersion)
				.UseConsole(console)
				.UseTypeInstantiator(_ => command)
				.Build()
				.RunAsync(rewritten);
			// CliFx reports every usage mistake it finds itself with exit code 1, which this
			// tool never uses for anything else.
			return exitCode switch {
				0 => command.ExitCode,
				1 => ProgramExitCodes.EX_USAGE,
				_ => exitCode,
			};
		}

		[CommandParameter(0, Name = "Assembly file name(s)", Description = "The list of assemblies that is being decompiled. This argument is mandatory.")]
		public IReadOnlyList<string> InputAssemblyNames { get; set; } = [];

		[CommandOption("outputdir", 'o', Description = "The output directory, if omitted decompiler output is written to standard out.")]
		public string OutputDirectory { get; set; }

		[CommandOption("project", 'p', Description = "Decompile assembly as compilable project. This requires the output directory option.")]
		public bool CreateCompilableProjectFlag { get; set; }

		[CommandOption("type", 't', Description = "The fully qualified name of the type to decompile.")]
		public string TypeName { get; set; }

		[CommandOption("member", 'm', Description = "The single member to decompile: an XML documentation id string (e.g. \"M:System.String.Concat(System.String,System.String)\", as also accepted by the UI's --navigateto option) or a metadata token (e.g. 0x06000005).")]
		public string MemberIdString { get; set; }

		[CommandOption("ilcode", Description = "Show IL code.")]
		public bool ShowILCodeFlag { get; set; }

		[CommandOption("il-sequence-points", Description = "Show IL with sequence points. Implies -il.")]
		public bool ShowILSequencePointsFlag { get; set; }

#if DEBUG
		[CommandOption("ilast", Description = "Show the decompiler's intermediate representation (ILAst) of method bodies, after the full IL transform pipeline. Select what to dump with --type or --member; without either, every method of the assembly is dumped.")]
		public bool ShowILAstFlag { get; set; }

		[CommandOption("after-transform", Description = "Stop the IL transform pipeline after the named transform (or after the transform at the given 1-based pipeline index) and show the ILAst at that point. Implies --ilast. Pass an unknown name to list the pipeline.")]
		public string AfterTransformName { get; set; }
#endif

		[CommandOption("generate-pdb", Description = "Generate PDB.")]
		public bool CreateDebugInfoFlag { get; set; }

		/// <summary>
		/// null when absent, empty for a bare -usepdb (the PDB next to the assembly), one
		/// element for -usepdb:file.
		/// </summary>
		[CommandOption("use-varnames-from-pdb", Description = "Use variable names from PDB.")]
		public IReadOnlyList<string> InputPDBFile { get; set; }

		[CommandOption("list", 'l', Description = "Lists all entities of the specified type(s). Valid types: c(lass), i(nterface), s(truct), d(elegate), e(num)")]
		public IReadOnlyList<string> EntityTypes { get; set; } = [];

		[CommandOption("list-resources", Description = "Lists all embedded resources in the assembly. Entries inside .resources containers are listed individually as '<container>/<entry>'.")]
		public bool ListResourcesFlag { get; set; }

		[CommandOption("resource", Description = "Extract a single resource by name (as printed by --list-resources). Resources whose name ends with '.baml' are decompiled to XAML.")]
		public string ResourceName { get; set; }

		[CommandOption("decompile-baml", Description = "Deprecated: -p decompiles BAML resources to XAML files (Page items) on its own. Accepted so that existing scripts keep working.")]
		public bool DecompileBamlFlag { get; set; }

		[CommandOption("dump-table", Description = "Dump a metadata table: prints RID, token, names, heap offsets and coded indexes of every row. <table> is the ECMA-335 table name (e.g. TypeDef, Property, MethodSemantics; case-insensitive) or table number (decimal or 0x-prefixed hex, e.g. 0x17).")]
		public string DumpTableName { get; set; }

		[CommandOption("dump-pdb", Description = "Dump the debug information of the assembly's PDB: documents, per-method sequence points, the scope tree with its variables, constants and imports, and custom debug information. Reads both Portable and Windows PDBs. Use -usepdb:<file> to point at a PDB that is not next to the assembly.")]
		public bool DumpPdbFlag { get; set; }

		[CommandOption("json", Description = "Output as JSON. Only supported together with --dump-table or --dump-pdb.")]
		public bool JsonOutputFlag { get; set; }

		[CommandOption("languageversion", Converter = typeof(LanguageVersionConverter), Description = "C# Language version: CSharp1, CSharp2, CSharp3, " +
			"CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, " +
			"CSharp10_0, CSharp11_0, CSharp12_0, CSharp13_0, Preview or Latest")]
		public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

		[CommandOption("ilspy-settingsfile", Description = "Path to an ILSpy settings file.")]
		public string ILSpySettingsFile { get; set; }

		[CommandOption("decompiler-setting", Description = "Set a decompiler setting as <name>=<value>. Use multiple times to set multiple settings.")]
		public IReadOnlyList<string> DecompilerSettingOverrides { get; set; } = [];

		[CommandOption("referencepath", 'r', Description = "Path to a directory containing dependencies of the assembly that is being decompiled.")]
		public IReadOnlyList<string> ReferencePaths { get; set; }

		[CommandOption("ignore-decompilation-errors", Description = "Exit with success even when parts of the assembly could not be decompiled. " +
			"The affected code carries the error text in the output and the failures are listed on stderr either way; " +
			"only the exit status changes.")]
		public bool IgnoreDecompilationErrorsFlag { get; set; }

		[CommandOption("no-dead-code", Description = "Remove dead code.")]
		public bool RemoveDeadCode { get; set; }

		[CommandOption("no-dead-stores", Description = "Remove dead stores.")]
		public bool RemoveDeadStores { get; set; }

		[CommandOption("dump-package", 'd', Description = "Dump package assemblies into a folder. This requires the output directory option.")]
		public bool DumpPackageFlag { get; set; }

		[CommandOption("bundle-entry", Description = "The assembly inside a single-file bundle (or other package) to work on, as printed when such a file is passed without this option. Ignored for input files that are not packages.")]
		public string BundleEntryName { get; set; }

		[CommandOption("nested-directories", Description = "Use nested directories for namespaces.")]
		public bool NestedDirectories { get; set; }

		[CommandOption("disable-updatecheck", Description = "If using ilspycmd in a tight loop or fully automated scenario, you might want to disable the automatic update check.")]
		public bool DisableUpdateCheck { get; set; }

		[CommandOption("generate-diagrammer", Description = "Generates an interactive HTML diagrammer app from selected types in the target assembly" +
			" - to the --outputdir or in a 'diagrammer' folder next to to the assembly by default.")]
		public bool GenerateDiagrammer { get; set; }

		[CommandOption("generate-diagrammer-include", Description = "An optional regular expression matching Type.FullName used to whitelist types to include in the generated diagrammer.")]
		public string Include { get; set; }

		[CommandOption("generate-diagrammer-exclude", Description = "An optional regular expression matching Type.FullName used to blacklist types to exclude from the generated diagrammer.")]
		public string Exclude { get; set; }

		[CommandOption("generate-diagrammer-report-excluded", Description = "Outputs a report of types excluded from the generated diagrammer" +
			" - whether by default because compiler-generated, explicitly by '--generate-diagrammer-exclude' or implicitly by '--generate-diagrammer-include'." +
			" You may find this useful to develop and debug your regular expressions.")]
		public bool ReportExcludedTypes { get; set; }

		[CommandOption("generate-diagrammer-docs", Description = "The path or file:// URI of the XML file containing the target assembly's documentation comments." +
			" You only need to set this if a) you want your diagrams annotated with them and b) the file name differs from that of the assembly." +
			" To enable XML documentation output for your assembly, see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output")]
		public string XmlDocs { get; set; }

		[CommandOption("generate-diagrammer-strip-namespaces", Description = "Optional space-separated namespace names that are removed for brevity from XML documentation comments." +
			" Note that the order matters: e.g. replace 'System.Collections' before 'System' to remove both of them completely.")]
		public IReadOnlyList<string> StrippedNamespaces { get; set; }

		[CommandOption("version", 'v', Description = "Show version of ICSharpCode.Decompiler used.")]
		public bool IsVersionRequested { get; set; }

		/// <summary>
		/// --generate-diagrammer-json-only, a developer switch. CliFx has no hidden options, so the
		/// rewriter takes the token out of the command line and the host sets this directly.
		/// </summary>
		public bool JsonOnly { get; set; }

		public int ExitCode { get; private set; }

		public async ValueTask ExecuteAsync(IConsole console)
		{
			var program = new ILSpyCmdProgram {
				InputAssemblyNames = [.. InputAssemblyNames],
				OutputDirectory = OutputDirectory,
				CreateCompilableProjectFlag = CreateCompilableProjectFlag,
				TypeName = TypeName,
				MemberIdString = MemberIdString,
				ShowILCodeFlag = ShowILCodeFlag,
				ShowILSequencePointsFlag = ShowILSequencePointsFlag,
#if DEBUG
				ShowILAstFlag = ShowILAstFlag,
				AfterTransformName = AfterTransformName,
#endif
				CreateDebugInfoFlag = CreateDebugInfoFlag,
				InputPDBFile = (InputPDBFile != null, InputPDBFile is [string pdbFile, ..] ? pdbFile : null),
				EntityTypes = [.. EntityTypes],
				ListResourcesFlag = ListResourcesFlag,
				ResourceName = ResourceName,
				DecompileBamlFlag = DecompileBamlFlag,
				DumpTableName = DumpTableName,
				DumpPdbFlag = DumpPdbFlag,
				JsonOutputFlag = JsonOutputFlag,
				LanguageVersion = LanguageVersion,
				ILSpySettingsFile = ILSpySettingsFile,
				DecompilerSettingOverrides = [.. DecompilerSettingOverrides],
				ReferencePaths = ReferencePaths?.ToArray(),
				IgnoreDecompilationErrorsFlag = IgnoreDecompilationErrorsFlag,
				RemoveDeadCode = RemoveDeadCode,
				RemoveDeadStores = RemoveDeadStores,
				DumpPackageFlag = DumpPackageFlag,
				BundleEntryName = BundleEntryName,
				NestedDirectories = NestedDirectories,
				DisableUpdateCheck = DisableUpdateCheck,
				GenerateDiagrammer = GenerateDiagrammer,
				Include = Include,
				Exclude = Exclude,
				ReportExcludedTypes = ReportExcludedTypes,
				XmlDocs = XmlDocs,
				StrippedNamespaces = StrippedNamespaces?.ToArray(),
				JsonOnly = JsonOnly,
			};
			ExitCode = await program.RunAsync();
		}

		/// <summary>
		/// CliFx's own enum conversion lets a bad value escape as an exception with a stack trace;
		/// a CommandException is reported as a one-line usage error with the given exit code.
		/// </summary>
		sealed class LanguageVersionConverter : ScalarInputConverter<LanguageVersion>
		{
			public override LanguageVersion Convert(string rawValue)
			{
				if (Enum.TryParse(rawValue, ignoreCase: true, out LanguageVersion version))
					return version;
				throw new CommandException($"Unexpected value '{rawValue}' for option '--languageversion'.", ProgramExitCodes.EX_USAGE);
			}
		}

		/// <summary>
		/// A write-only stream handing CliFx's console bytes back to a TextWriter. CliFx encodes
		/// with Console.OutputEncoding, so the same encoding decodes them.
		/// </summary>
		sealed class ConsoleStream(TextWriter writer) : Stream
		{
			readonly Decoder decoder = Console.OutputEncoding.GetDecoder();

			public override void Write(byte[] buffer, int offset, int count)
			{
				char[] chars = new char[Console.OutputEncoding.GetMaxCharCount(count)];
				int charCount = decoder.GetChars(buffer, offset, count, chars, 0);
				writer.Write(chars, 0, charCount);
			}

			public override void Flush() => writer.Flush();

			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => true;
			public override long Length => throw new NotSupportedException();
			public override long Position {
				get => throw new NotSupportedException();
				set => throw new NotSupportedException();
			}
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();
		}
	}
}
