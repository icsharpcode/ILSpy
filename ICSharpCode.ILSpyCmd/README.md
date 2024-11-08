# ilspycmd .NET Tool 

To install:

```
dotnet tool install --global ilspycmd
```

Help output (`ilspycmd --help`):

```
ilspycmd: 9.0.0.7847
ICSharpCode.Decompiler: 9.0.0.7847

dotnet tool for decompiling .NET assemblies and generating portable PDBs

Usage: ilspycmd [options] <Assembly file name(s)>

Arguments:
  Assembly file name(s)                   The list of assemblies that is being decompiled. This argument is mandatory.

Options:
  -v|--version                            Show version of ICSharpCode.Decompiler used.
  -h|--help                               Show help information.
  -o|--outputdir <directory>              The output directory, if omitted decompiler output is written to standard out.
  -p|--project                            Decompile assembly as compilable project. This requires the output directory
                                          option.
  -t|--type <type-name>                   The fully qualified name of the type to decompile.
  -il|--ilcode                            Show IL code.
  --il-sequence-points                    Show IL with sequence points. Implies -il.
  -genpdb|--generate-pdb                  Generate PDB.
  -usepdb|--use-varnames-from-pdb         Use variable names from PDB.
  -l|--list <entity-type(s)>              Lists all entities of the specified type(s). Valid types: c(lass),
                                          i(nterface), s(truct), d(elegate), e(num)
  -lv|--languageversion <version>         C# Language version: CSharp1, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6,
                                          CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, CSharp10_0,
                                          Preview or Latest
                                          Allowed values are: CSharp1, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6,
                                          CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, CSharp10_0,
                                          CSharp11_0, Preview, CSharp12_0, Latest.
                                          Default value is: Latest.
  -r|--referencepath <path>               Path to a directory containing dependencies of the assembly that is being
                                          decompiled.
  --no-dead-code                          Remove dead code.
  --no-dead-stores                        Remove dead stores.
  -d|--dump-package                       Dump package assemblies into a folder. This requires the output directory
                                          option.
  --nested-directories                    Use nested directories for namespaces.
  --disable-updatecheck                   If using ilspycmd in a tight loop or fully automated scenario, you might want
                                          to disable the automatic update check.
  --generate-diagrammer                   Generates an interactive HTML diagrammer app from selected types in the target
                                          assembly - to the --outputdir or in a 'diagrammer' folder next to to the
                                          assembly by default.
  --generate-diagrammer-include           An optional regular expression matching Type.FullName used to whitelist types
                                          to include in the generated diagrammer.
  --generate-diagrammer-exclude           An optional regular expression matching Type.FullName used to blacklist types
                                          to exclude from the generated diagrammer.
  --generate-diagrammer-report-excluded   Outputs a report of types excluded from the generated diagrammer - whether by
                                          default because compiler-generated, explicitly by
                                          '--generate-diagrammer-exclude' or implicitly by
                                          '--generate-diagrammer-include'. You may find this useful to develop and debug
                                          your regular expressions.
  --generate-diagrammer-docs              The path or file:// URI of the XML file containing the target assembly's
                                          documentation comments. You only need to set this if a) you want your diagrams
                                          annotated with them and b) the file name differs from that of the assmbly. To
                                          enable XML documentation output for your assmbly, see
                                          https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output
  --generate-diagrammer-strip-namespaces  Optional space-separated namespace names that are removed for brevity from XML
                                          documentation comments. Note that the order matters: e.g. replace
                                          'System.Collections' before 'System' to remove both of them completely.

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

    Generate a HTML diagrammer containing all type info into a folder next to the input assembly
        ilspycmd sample.dll --generate-diagrammer

    Generate a HTML diagrammer containing filtered type info into a custom output folder
    (including types in the LightJson namespace while excluding types in nested LightJson.Serialization namespace)
        ilspycmd sample.dll --generate-diagrammer -o c:\diagrammer --generate-diagrammer-include LightJson\\..+ --generate-diagrammer-exclude LightJson\\.Serialization\\..+
```
