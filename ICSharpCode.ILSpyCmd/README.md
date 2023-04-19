# ilspycmd .NET 6.0 Tool 

To install:

```
dotnet tool install ilspycmd -g
```

Help output (`ilspycmd ---help`):

```
ilspycmd: 8.0.0.7007
ICSharpCode.Decompiler: 8.0.0.7007

dotnet tool for decompiling .NET assemblies and generating portable PDBs

Usage: ilspycmd [options] <Assembly file name(s)>

Arguments:
  Assembly file name(s)            The list of assemblies that is being decompiled. This argument is mandatory.

Options:
  -v|--version                     Show version information.
  -h|--help                        Show help information.
  -o|--outputdir <directory>       The output directory, if omitted decompiler output is written to standard out.
  -p|--project                     Decompile assembly as compilable project. This requires the output directory option.
  -t|--type <type-name>            The fully qualified name of the type to decompile.
  -il|--ilcode                     Show IL code.
  --il-sequence-points             Show IL with sequence points. Implies -il.
  -genpdb|--generate-pdb           Generate PDB.
  -usepdb|--use-varnames-from-pdb  Use variable names from PDB.
  -l|--list <entity-type(s)>       Lists all entities of the specified type(s). Valid types: c(lass), i(nterface),
                                   s(truct), d(elegate), e(num)
  -lv|--languageversion <version>  C# Language version: CSharp1, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6, CSharp7_0,
                                   CSharp7_1, CSharp7_2, CSharp7_3, CSharp8_0, CSharp9_0, CSharp_10_0 or Latest
  -r|--referencepath <path>        Path to a directory containing dependencies of the assembly that is being decompiled.
  --no-dead-code                   Remove dead code.
  --no-dead-stores                 Remove dead stores.
  -d|--dump-package                Dump package assembiles into a folder. This requires the output directory option.
  --nested-directories             Use nested directories for namespaces.

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
```
