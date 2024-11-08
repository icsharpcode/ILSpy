# How does it work?

To **extract the type info from the source assembly**, ILSpy side-loads it including all its dependencies.

The extracted type info is **structured into a model optimized for the HTML diagrammer** and serialized to JSON. The model is a mix between drop-in type definitions in mermaid class diagram syntax and destructured metadata about relations, inheritance and documentation comments.

> The JSON type info is injected into the `template.html` alongside other resources like the `script.js` at corresponding `{{placeholders}}`. It comes baked into the HTML diagrammer to enable
> - accessing the data and
> - importing the mermaid module from a CDN
>
> locally without running a web server [while also avoiding CORS restrictions.](https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy#file_origins)

In the final step, the **HTML diagrammer app re-assembles the type info** based on the in-app type selection and rendering options **to generate [mermaid class diagrams](https://mermaid.js.org/syntax/classDiagram.html)** with the types, their relations and as much inheritance detail as you need.
