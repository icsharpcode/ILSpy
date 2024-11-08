<!-- title: netAmermaid --> <!-- of the printed HTML see https://github.com/yzhang-gh/vscode-markdown#print-markdown-to-html -->
# netAmermaid <!-- omit in toc -->

An automated documentation tool for visually exploring
[.NET assemblies](https://learn.microsoft.com/en-us/dotnet/standard/assembly/) (_*.dll_ files)
along type relations using rapid diagramming.

<img src="./html/netAmermaid.ico" align="right" title="" />

<!--TOC-->
- [What can it do for you and how?](#what-can-it-do-for-you-and-how)
- [How does it work?](#how-does-it-work)
- [Check out the demo](#check-out-the-demo)
- [Optimized for exploration and sharing](#optimized-for-exploration-and-sharing)
- [Generate a HTML diagrammer using the console app](#generate-a-html-diagrammer-using-the-console-app)
  - [Manually before use](#manually-before-use)
  - [Automatically](#automatically)
    - [After building](#after-building)
    - [After publishing](#after-publishing)
  - [Options](#options)
  - [Tips for using the console app](#tips-for-using-the-console-app)
  - [Advanced configuration examples](#advanced-configuration-examples)
    - [Filter extracted types](#filter-extracted-types)
    - [Strip namespaces from XML comments](#strip-namespaces-from-xml-comments)
    - [Adjust for custom XML documentation file names](#adjust-for-custom-xml-documentation-file-names)
- [Tips for using the HTML diagrammer](#tips-for-using-the-html-diagrammer)
- [Thanks to](#thanks-to)
- [Disclaimer](#disclaimer)
<!--/TOC-->

# What can it do for you and how?

> **Class diagrams** and Entity/Relationship diagrams **can be really helpful if done right**.
They let us see how types relate - handing us a **ready-made mental map** for a subdomain.
At the same time **they take a load of our minds**, sparing us from having to remember all those relations correctly across frantic code symbol navigations in our IDE.
And occasionally they can serve as [safe and engaging fodder for big-brained busy-bodies](https://grugbrain.dev/#grug-on-factring-your-code) to stay the heck out of our code base.

> **Drawing them takes way too long though** - even in fancy designers.
And what's the point? Like any other hand-crafted documentation, **they're always outdated** - often from the very beginning.
After a while their usability becomes a function of **how much time you want to spend maintaining** them.
Also, they're next to useless in conversations about the *boundaries* of whatever subdomain or Aggregate you're looking at - because they **lack the interactivity to let you peek beyond the boundary**.

**netAmermaid** helps you create useful on-the-fly class diagrams within seconds in two simple steps:

1. Point the **command line tool** at an assembly to extract its type information
and **build a [HTML5](https://en.wikipedia.org/wiki/HTML5#New_APIs) diagramming app** from it.
To get it hot off your latest build, you can script this step and run it just before using the diagrammer - or
hook it into your build pipeline to automate it for Continuous Integration.
1. Open the **HTML diagrammer** to select types and **render class diagrams** from them
within a couple of keystrokes - after which you can interact with the diagram directly
to unfold the domain along type relations. At any point, familiar key commands will copy the diagram to your clipboard
or export it as either SVG, PNG or in [mermaid class diagram syntax](https://mermaid.js.org/syntax/classDiagram.html). You can also just share the URL with anybody with access to the HTML diagrammer or paste it into your code where helpful.

If [**XML documentation comments** are available for the source assembly](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output), they're **used to annotate types and members on the generated diagrams**. Commented symbols show up highlighted, making the documentation accessible on hover.

> Dealing with .Net assemblies, you've probably come across [**ILSpy**](https://github.com/icsharpcode/ILSpy) and can appreciate how useful it is to explore and understand even the most poorly documented library. Think of netAmermaid as a **visual version** of that - minus geeky details like code decompilation and symbol usage analysis. Instead,

What netAmermaid offers is an **overview** over types, their members and **relations** and the ability to **unfold the domain** along them until you have enough **context** to make an informed decision. Use it as
- a **mental mapping** tool to get your bearings in an **unknown domain**.
- a **communication** tool for **your own domain** - when talking about the bigger picture with your team mates or even non-technical shareholders like product owners and users.

# How does it work?

To **extract the type info from the source assembly**, the netAmermaid CLI side-loads it including all its dependencies.
The current implementation actually uses ILSpy under the hood for that because it's really good at figuring out [which runtime, GAC or private bin path to load referenced assemblies from](https://github.com/icsharpcode/ILSpy/blob/master/ICSharpCode.Decompiler/Metadata/UniversalAssemblyResolver.cs).

The extracted type info is **structured into a model optimized for the HTML diagrammer** and serialized to JSON. The model is a mix between drop-in type definitions in mermaid class diagram syntax and destructured metadata about relations, inheritance and documentation comments.

> The JSON type info is injected into the `template.html` alongside other resources like the `script.js` at corresponding `{{placeholders}}`. It comes baked into the HTML diagrammer to enable
> - accessing the data and
> - importing the mermaid module from a CDN
>
> locally without running a web server [while also avoiding CORS restrictions.](https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy#file_origins)
>
> Note that you can
> - **customize** the `template.html`, `script.js` and `styles.css` used in this process, e.g. for rewording or branding the UI.
> - **build a stand-alone offline diagrammer** with *mermaid* baked in instead of loading it from a CDN. All you have to do is replace the `import` statement for `/dist/mermaid.esm.min.mjs` at the top of `script.js` with the *contents* of [`/dist/mermaid.min.js`](https://unpkg.com/mermaid@latest/dist/mermaid.min.js) from the *mermaid* package of your choice.

In the final step, the **HTML diagrammer app re-assembles the type info** based on the in-app type selection and rendering options **to generate [mermaid class diagrams](https://mermaid.js.org/syntax/classDiagram.html)** with the types, their relations and as much inheritance detail as you need.

# Check out the demo

Have a look at the diagrammer generated for [SubTubular](https://github.com/h0lg/SubTubular):
It's got some [type relations](https://raw.githack.com/h0lg/SubTubular/netAmermaid2/netAmermaid/class-diagrammer.html?d=LR&i=tim&t=Caption-CaptionTrack-PaddedMatch-IncludedMatch-Video-VideoSearchResult-CaptionTrackResult)
and [inheritance](https://raw.githack.com/h0lg/SubTubular/netAmermaid2/netAmermaid/class-diagrammer.html?d=LR&i=tim&t=RemoteValidated-SearchChannel-SearchCommand-Shows-SearchPlaylist-SearchPlaylistCommand-OrderOptions-SearchVideos)
going on that offer a decent playground.

> Wouldn't it be great to show off netAmermaid's capabilities applied to itself?
Sure - but with the console app being as simple as it is, its class diagrams
are pretty boring and don't get the benefit across.
As with any documentation, netAmermaid starts to shine at higher complexity.
So you could say it offers little value to itself - 
but it rather likes to call that selfless and feel good about it.

# Optimized for exploration and sharing

It is not the goal of the HTML diagrammer to create the perfect diagram -
so you'll find few options to customize the layout.
This is - to some degree - due to the nature of generative diagramming itself,
while at other times the [mermaid API](https://mermaid.js.org/syntax/classDiagram.html) poses the limiting factor.
Having said that, you can usually **choose a direction** in which the automated layout works reasonably well.

Instead, think of the diagrammer as
- a browser for **exploring domains**
- a visual design aid for **reasoning about type relations and inheritance**
- a **communication tool** for contributors and users to share aspects of a model
- a **documentation** you don't have to write.

You'll find controls and key bindings to help you get those things done as quickly and efficiently as possible.

# Generate a HTML diagrammer using the console app

Once you have an output folder in mind, you can adopt either of the following strategies
to generate a HTML diagrammer from a .Net assembly using the console app.

## Manually before use

**Create the output folder** in your location of choice and inside it **a new shell script**.

Using the CMD shell in a Windows environment for example, you'd create a `regenerate.cmd` looking somewhat like this:

<pre>
..\..\path\to\netAmermaid.exe --assembly ..\path\to\your\assembly.dll --output-folder .
</pre>

With this script in place, run it to (re-)generate the HTML diagrammer at your leisure. Note that `--output-folder .` directs the output to the current directory.

## Automatically

If you want to deploy an up-to-date HTML diagrammer as part of your live documentation,
you'll want to automate its regeneration to keep it in sync with your code base.

For example, you might like to share the diagrammer on a web server or - in general - with users
who cannot or may not regenerate it; lacking either access to the netAmermaid console app or permission to use it.

In such cases, you can dangle the regeneration off the end of either your build or deployment pipeline.
Note that the macros used here apply to [MSBuild](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild) for [Visual Studio](https://learn.microsoft.com/en-us/visualstudio/ide/reference/pre-build-event-post-build-event-command-line-dialog-box) and your mileage may vary with VS for Mac or VS Code.

### After building

To regenerate the HTML diagrammer from your output assembly after building,
add something like the following to your project file.
Note that the `Condition` here is optional and configures this step to only run after `Release` builds.

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent" Condition="'$(Configuration)' == 'Release'">
  <Exec Command="$(SolutionDir)..\path\to\netAmermaid.exe --assembly $(TargetPath) --output-folder $(ProjectDir)netAmermaid" />
</Target>
```

### After publishing

If you'd rather regenerate the diagram after publishing instead of building, all you have to do is change the `AfterTargets` to `Publish`.
Note that the `Target` `Name` doesn't matter here and that the diagrammer is generated into a folder in the `PublishDir` instead of the `ProjectDir`.

```xml
<Target Name="GenerateHtmlDiagrammer" AfterTargets="Publish">
  <Exec Command="$(SolutionDir)..\path\to\netAmermaid.exe --assembly $(TargetPath) --output-folder $(PublishDir)netAmermaid" />
</Target>
```

## Options

The command line app exposes the following parameters.

| shorthand, name            |                                                                                                                                                                                                                                                                                                                                                                                                          |
| :------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `-a`, `--assembly`         | Required. The path or file:// URI of the .NET assembly to generate a HTML diagrammer for.                                                                                                                                                                                                                                                                                                                |
| `-o`, `--output-folder`    | The path of the folder to generate the HTML diagrammer into. This defaults to a 'netAmermaid' folder in the directory of the `assembly`, which will be created if required.                                                                                                                                                                                                                              |
| `-i`, `--include`          | A regular expression matching Type.FullName used to whitelist types.                                                                                                                                                                                                                                                                                                                                     |
| `-e`, `--exclude`          | A regular expression matching Type.FullName used to blacklist types.                                                                                                                                                                                                                                                                                                                                     |
| `-r`, `--report-excluded`  | Outputs a report of types excluded from the HTML diagrammer - whether by default because compiler-generated, explicitly by `--exclude` or implicitly by `--include`. You may find this useful to develop and debug your regular expressions.                                                                                                                                                             |
| `-n`, `--strip-namespaces` | Space-separated namespace names that are removed for brevity from XML documentation comments. Note that the order matters: e.g. replace 'System.Collections' before 'System' to remove both of them completely.                                                                                                                                                                                          |
| `-d`, `--docs`             | The path or file:// URI of the XML file containing the `assembly`'s documentation comments. You only need to set this if a) you want your diagrams annotated with them and b) the file name differs from that of the `assembly`. To enable XML documentation output for your `assembly` see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output . |

## Tips for using the console app

**Compiler-generated** types and their nested types are **excluded by default**.

Consider sussing out **big source assemblies** using [ILSpy](https://github.com/icsharpcode/ILSpy) first to get an idea about which subdomains to include in your diagrammers. Otherwise you may experience long build times and large file sizes for the diagrammer as well as a looong type selection opening it. At some point, mermaid may refuse to render all types in your selection because their definitions exceed the maximum input size. If that's where you find yourself, you may want to consider
- using `--include` and `--exclude` to **limit the scope of the individual diagrammer to a certain subdomain**
- generating **multiple diagrammers for different subdomains**.

## Advanced configuration examples

Above examples show how the most important options are used. Let's have a quick look at the remaining ones, which allow for customization in your project setup and diagrams.

### Filter extracted types

Sometimes the source assembly contains way more types than are sensible to diagram. Types with metadata for validation or mapping for example. Or auto-generated types.
Especially if you want to tailor a diagrammer for a certain target audience and hide away most of the supporting type system to avoid noise and unnecessary questions.

In these scenarios you can supply Regular Expressions for types to `--include` (white-list) and `--exclude` (black-list).
A third option `--report-excluded` will output a `.txt` containing the list of effectively excluded types next to the HTML diagrammer containing the effectively included types.

<pre>
netAmermaid.exe <b>--include Your\.Models\..+ --exclude .+\+Metadata|.+\.Data\..+Map --report-excluded</b> --assembly ..\path\to\your\assembly.dll --output-folder .
</pre>

This example
- includes all types in the top-level namespace `Your.Models`
- while excluding
  - nested types called `Metadata` and
  - types ending in `Map` in descendant `.Data.` namespaces.

### Strip namespaces from XML comments

You can reduce the noise in the member lists of classes on your diagrams by supplying a space-separated list of namespaces to omit from the output like so:

<pre>
netAmermaid.exe <b>--strip-namespaces System.Collections.Generic System</b> --assembly ..\path\to\your\assembly.dll --output-folder .
</pre>

Note how `System` is replaced **after** other namespaces starting with `System.` to achieve complete removal.
Otherwise `System.Collections.Generic` wouldn't match the `Collections.Generic` left over after removing `System.`, resulting in partial removal only.

### Adjust for custom XML documentation file names

If - for whatever reason - you have customized your XML documentation file output name, you can specify a custom path to pick it up from.

<pre>
netAmermaid.exe <b>--docs ..\path\to\your\docs.xml</b> --assembly ..\path\to\your\assembly.dll --output-folder .
</pre>

# Tips for using the HTML diagrammer

> **On Mac**, use the Command key ⌘ instead of `Ctrl`.

- The type selection is focused by default. That means you can **immediately start typing**
to select the type you want to use as a starting point for your diagram and **hit Enter to render** it.
- Don't forget that you can hold [Shift] to **↕ range-select** and [Ctrl] to **± add to or subtract from** your selection.
- With a **big type selection**, you'll want to use the **pre-filter** often. Focus it with [Ctrl + k]. Use plain text or an EcmaScript flavored RegEx to filter the selection.
- After rendering, you can **explore the domain along type relations** by clicking related types on the diagram to toggle them in the filter and trigger re-rendering.
- Changing the type selection or rendering options updates the URL in the location bar. That means you can
    - 🔖 **bookmark** or 📣 **share the URL** to your diagram with whoever has access to this diagrammer,
    - **access 🕔 earlier diagrams** recorded in your 🧾 browser history and
    - **⇥ restore your type selection** to the picker from the URL using ⟳ Refresh [F5] if you lose it.
- The diagram has a **layout direction**, i.e. **rendering depends on the order of your selection**! Use [Alt] + [Arrow Up|Down] to sort selected types.
- You can **zoom the rendered diagram** using [Ctrl + mouse wheel] and **grab and drag to pan** it. Reset zoom and pan with [Ctrl + 0].
- Need more space? **Adjust the sidebar size** by grabbing and dragging its edge or **hide it completely** with [Ctrl + b] to zen out on the diagram alone.
- You can **copy and save your diagrams** using [Ctrl + c] or [Ctrl + s] respectively. The first time you try to quick-save will open the export options for you to choose the format.
- Showing off the diagrammer remotely? Enter **presentation mode** using [Ctrl + i] to **emphasize your mouse** pointer location, **visualize clicks** and **display pressed keys** for your audience to learn the commands while watching you.
- **Look out for tooltips** to give you **more help** where necessary, like useful **key bindings** to help you get stuff done ASAP. You can also highlight all tool-tipped elements with [Alt + i].

# Thanks to

in the order they're used in netAmermaid's pipeline:

- [Command Line Parser](https://github.com/commandlineparser/commandline) for parsing shell arguments and making validation and help text display easy.
- [ILSpy](https://github.com/icsharpcode/ILSpy) for reliably retrieving type information from anything that runs with half a broken limb and a dozen broken references. You can have an assembly and a dream and it'll still figure out where to load the dependencies from. In .Net Framework version hell, that is nothing short of amazing.
- [mermaid](https://github.com/mermaid-js/mermaid) for abstracting the capabilities of d3 to a degree that allows the rest of us to build useful stuff without hurting our smol brains more than necessary.

# Disclaimer

No mermaids were harmed in the writing of this software and you shouldn't interpret the name as inciting capture of or violence against magical creatures. We would never - [they're doing a great job and we love and respect them for it](https://mermaid.js.org/).