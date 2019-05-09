# ilspycmd

```
dotnet tool install ilspycmd -g
```

.NET Core 2.1 Tool 

```
ilspycmd -h

dotnet tool for decompiling .NET assemblies and generating portable PDBs

Usage: ilspycmd [arguments] [options]

Arguments:
  Assembly file name          The assembly that is being decompiled. This argument is mandatory.

Options:
  -h|--help                   Show help information
  -o|--outputdir <directory>  The output directory, if omitted decompiler output is written to standard out.
  -p|--project                Decompile assembly as compilable project. This requires the output directory option.
  -t|--type <type-name>       The fully qualified name of the type to decompile.
  -il|--ilcode                Show IL code.
  -d|--debuginfo              Generate PDB.
  -l|--list <entity-type(s)>  Lists all entities of the specified type(s). Valid types: c(lass), i(interface), s(truct), d(elegate), e(num)
  -v|--version                Show version of ICSharpCode.Decompiler used.

Remarks:
  -o is valid with every option and required when using -p.
```
