// Copyright (c) 2024 Holger Schmidt
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	partial class GenerateHtmlDiagrammer
	{
		public void Run()
		{
			var assemblyPath = Assembly;
			XmlDocumentationFormatter? xmlDocs = CreateXmlDocsFormatter(assemblyPath);
			ClassDiagrammer model = BuildModel(assemblyPath, xmlDocs);
			GenerateOutput(assemblyPath, model);
		}

		protected virtual XmlDocumentationFormatter? CreateXmlDocsFormatter(string assemblyPath)
		{
			var xmlDocsPath = XmlDocs == null ? Path.ChangeExtension(assemblyPath, ".xml") : XmlDocs;
			XmlDocumentationFormatter? xmlDocs = null;

			if (File.Exists(xmlDocsPath))
				xmlDocs = new XmlDocumentationFormatter(new XmlDocumentationProvider(xmlDocsPath), StrippedNamespaces?.ToArray());
			else
				Debug.WriteLine("No XML documentation file found. Continuing without.");

			return xmlDocs;
		}

		protected virtual ClassDiagrammer BuildModel(string assemblyPath, XmlDocumentationFormatter? xmlDocs)
			=> new ClassDiagrammerFactory(xmlDocs).BuildModel(assemblyPath, Include, Exclude);

		private string SerializeModel(ClassDiagrammer diagrammer)
		{
			/* convert collections to dictionaries for easier access in ES using
			 * for (let [key, value] of Object.entries(dictionary)) */
			var model = new DiagrammerModel(
				diagrammer.OutsideReferences,
				diagrammer.TypesByNamespace.ToDictionary(ns => ns.Key, ns => ns.Value.ToDictionary(t => t.Id, t => t)));
			string modelJson = JsonSerializer.Serialize(model, DiagrammerJsonContext.Default.DiagrammerModel);

			// wrap model including the data required for doing the template replacement in a JS build task
			if (JsonOnly)
			{
				var bundle = new DiagrammerBundle(
					diagrammer.SourceAssemblyName,
					diagrammer.SourceAssemblyVersion,
					DecompilerVersionInfo.FullVersionWithCommitHash,
					RepoUrl,
					// pre-serialized so that the JS build task does not have to re-serialize it
					modelJson);
				return JsonSerializer.Serialize(bundle, DiagrammerJsonContext.Default.DiagrammerBundle);
			}

			return modelJson;
		}

		/// <summary>The JSON model the diagrammer's script works with.</summary>
		internal sealed record DiagrammerModel(
			Dictionary<string, string> OutsideReferences,
			Dictionary<string, Dictionary<string, ClassDiagrammer.Type>> TypesByNamespace);

		/// <summary>The JSON-only output: the pre-serialized model plus the values a JS build task
		/// substitutes into the HTML template.</summary>
		internal sealed record DiagrammerBundle(
			string SourceAssemblyName,
			string SourceAssemblyVersion,
			string BuilderVersion,
			string RepoUrl,
			string Model);

		private void GenerateOutput(string assemblyPath, ClassDiagrammer model)
		{
			string modelJson = SerializeModel(model);

			// If no out folder is specified, default to a "<Input.Assembly.Name> diagrammer" folder next to the input assembly.
			var outputFolder = OutputFolder
				?? Path.Combine(
					Path.GetDirectoryName(assemblyPath) ?? string.Empty,
					Path.GetFileNameWithoutExtension(assemblyPath) + " diagrammer");

			if (!Directory.Exists(outputFolder))
				Directory.CreateDirectory(outputFolder);

			if (JsonOnly)
			{
				File.WriteAllText(Path.Combine(outputFolder, "model.json"), modelJson);
				Debug.WriteLine("Successfully generated model.json for HTML diagrammer.");
			}
			else
			{
				var htmlTemplate = EmbeddedResource.ReadText("template.html");

				var html = htmlTemplate
					.Replace("{{SourceAssemblyName}}", model.SourceAssemblyName)
					.Replace("{{SourceAssemblyVersion}}", model.SourceAssemblyVersion)
					.Replace("{{BuilderVersion}}", DecompilerVersionInfo.FullVersionWithCommitHash)
					.Replace("{{RepoUrl}}", RepoUrl)
					.Replace("{{Model}}", modelJson);

				File.WriteAllText(Path.Combine(outputFolder, "index.html"), html);

				// copy required resources to output folder while flattening paths if required
				foreach (var resource in new[] { "styles.css", "ILSpy.ico", "script.js" })
					EmbeddedResource.CopyTo(outputFolder, resource);

				Debug.WriteLine("Successfully generated HTML diagrammer.");
			}

			if (ReportExcludedTypes)
			{
				string excludedTypes = model.Excluded.Join(Environment.NewLine);
				File.WriteAllText(Path.Combine(outputFolder, "excluded types.txt"), excludedTypes);
			}
		}
	}

	// Source-generated so the serializer needs no runtime reflection: Native AOT turns reflection-based
	// System.Text.Json off, and trimming would strip the model's property getters.
	[JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonSerializable(typeof(GenerateHtmlDiagrammer.DiagrammerModel))]
	[JsonSerializable(typeof(GenerateHtmlDiagrammer.DiagrammerBundle))]
	internal partial class DiagrammerJsonContext : JsonSerializerContext
	{
	}
}