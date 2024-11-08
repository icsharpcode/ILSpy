# netAmermaid

An automated documentation tool for visually exploring
[.NET assemblies](https://learn.microsoft.com/en-us/dotnet/standard/assembly/) (_*.dll_ files)
along type relations using rapid diagramming.

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

What netAmermaid offers is an **overview** over types, their members and **relations** and the ability to **unfold the domain** along them until you have enough **context** to make an informed decision. Use it as
- a **mental mapping** tool to get your bearings in an **unknown domain**.
- a **communication** tool for **your own domain** - when talking about the bigger picture with your team mates or even non-technical shareholders like product owners and users.

# Check out the demo

Have a look at the diagrammer generated for [SubTubular](https://github.com/h0lg/SubTubular):
It's got some [type relations](https://raw.githack.com/h0lg/SubTubular/netAmermaid2/netAmermaid/class-diagrammer.html?d=LR&i=tim&t=Caption-CaptionTrack-PaddedMatch-IncludedMatch-Video-VideoSearchResult-CaptionTrackResult)
and [inheritance](https://raw.githack.com/h0lg/SubTubular/netAmermaid2/netAmermaid/class-diagrammer.html?d=LR&i=tim&t=RemoteValidated-SearchChannel-SearchCommand-Shows-SearchPlaylist-SearchPlaylistCommand-OrderOptions-SearchVideos)
going on that offer a decent playground.

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
