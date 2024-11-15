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

## Generate HTML diagrammers

Once you have an output folder in mind, you can adopt either of the following strategies
to generate a HTML diagrammer from a .Net assembly using the console app.

### Manually before use

**Create the output folder** in your location of choice and inside it **a new shell script**.

Using the CMD shell in a Windows environment for example, you'd create a `regenerate.cmd` looking somewhat like this:

<pre>
..\..\path\to\ilspycmd.exe ..\path\to\your\assembly.dll --generate-diagrammer --outputdir .
</pre>

With this script in place, run it to (re-)generate the HTML diagrammer at your leisure. Note that `--outputdir .` directs the output to the current directory.

### Automatically

If you want to deploy an up-to-date HTML diagrammer as part of your live documentation,
you'll want to automate its regeneration to keep it in sync with your code base.

For example, you might like to share the diagrammer on a web server or - in general - with users
who cannot or may not regenerate it; lacking either access to the ilspycmd console app or permission to use it.

In such cases, you can dangle the regeneration off the end of either your build or deployment pipeline.
Note that the macros used here apply to [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild) for [Visual Studio](https://learn.microsoft.com/en-us/visualstudio/ide/reference/pre-build-event-post-build-event-command-line-dialog-box) and your mileage may vary with VS for Mac or VS Code.

#### After building

To regenerate the HTML diagrammer from your output assembly after building,
add something like the following to your project file.
Note that the `Condition` here is optional and configures this step to only run after `Release` builds.

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent" Condition="'$(Configuration)' == 'Release'">
  <Exec Command="$(SolutionDir)..\path\to\ilspycmd.exe $(TargetPath) --generate-diagrammer --outputdir $(ProjectDir)diagrammer" />
</Target>
```

#### After publishing

If you'd rather regenerate the diagram after publishing instead of building, all you have to do is change the `AfterTargets` to `Publish`.
Note that the `Target` `Name` doesn't matter here and that the diagrammer is generated into a folder in the `PublishDir` instead of the `ProjectDir`.

```xml
<Target Name="GenerateHtmlDiagrammer" AfterTargets="Publish">
  <Exec Command="$(SolutionDir)..\path\to\ilspycmd.exe $(TargetPath) --generate-diagrammer --outputdir $(PublishDir)diagrammer" />
</Target>
```

### Usage tips

**Compiler-generated** types and their nested types are **excluded by default**.

Consider sussing out **big source assemblies** using [ILSpy](https://github.com/icsharpcode/ILSpy) first to get an idea about which subdomains to include in your diagrammers. Otherwise you may experience long build times and large file sizes for the diagrammer as well as a looong type selection opening it. At some point, mermaid may refuse to render all types in your selection because their definitions exceed the maximum input size. If that's where you find yourself, you may want to consider
- using `--generate-diagrammer-include` and `--generate-diagrammer-exclude` to **limit the scope of the individual diagrammer to a certain subdomain**
- generating **multiple diagrammers for different subdomains**.

### Advanced configuration examples

Above examples show how the most important options are used. Let's have a quick look at the remaining ones, which allow for customization in your project setup and diagrams.

#### Filter extracted types

Sometimes the source assembly contains way more types than are sensible to diagram. Types with metadata for validation or mapping for example. Or auto-generated types.
Especially if you want to tailor a diagrammer for a certain target audience and hide away most of the supporting type system to avoid noise and unnecessary questions.

In these scenarios you can supply Regular Expressions for types to `--generate-diagrammer-include` (white-list) and `--generate-diagrammer-exclude` (black-list).
A third option `--generate-diagrammer-report-excluded` will output a `.txt` containing the list of effectively excluded types next to the HTML diagrammer containing the effectively included types.

<pre>
ilspycmd.exe <b>--generate-diagrammer-include Your\.Models\..+ --generate-diagrammer-exclude .+\+Metadata|.+\.Data\..+Map --generate-diagrammer-report-excluded</b> ..\path\to\your\assembly.dll --generate-diagrammer --outputdir .
</pre>

This example
- includes all types in the top-level namespace `Your.Models`
- while excluding
  - nested types called `Metadata` and
  - types ending in `Map` in descendant `.Data.` namespaces.

#### Strip namespaces from XML comments

You can reduce the noise in the XML documentation comments on classes on your diagrams by supplying a space-separated list of namespaces to omit from the output like so:

<pre>
ilspycmd.exe <b>--generate-diagrammer-strip-namespaces System.Collections.Generic System</b> ..\path\to\your\assembly.dll --generate-diagrammer --output-folder .
</pre>

Note how `System` is replaced **after** other namespaces starting with `System.` to achieve complete removal.
Otherwise `System.Collections.Generic` wouldn't match the `Collections.Generic` left over after removing `System.`, resulting in partial removal only.

#### Adjust for custom XML documentation file names

If - for whatever reason - you have customized your XML documentation file output name, you can specify a custom path to pick it up from.

<pre>
ilspycmd.exe <b>--generate-diagrammer-docs ..\path\to\your\docs.xml</b> ..\path\to\your\assembly.dll --generate-diagrammer --output-folder .
</pre>
