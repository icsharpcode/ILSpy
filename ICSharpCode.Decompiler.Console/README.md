# ilspycmd

```
dotnet tool install ilspycmd -g
```

.NET Core 3.1 and .NET 6.0 Tool 

```
ilspycmd -h
ilspycmd: 7.2.0.0
ICSharpCode.Decompiler: 7.2.0.6768

dotnet tool for decompiling .NET assemblies and generating portable PDBs

Usage: ilspycmd [options] <Assembly file name>

Arguments:
  Assembly file name               The assembly that is being decompiled. This argument is mandatory.

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

Remarks:
  -o is valid with every option and required when using -p.
```
