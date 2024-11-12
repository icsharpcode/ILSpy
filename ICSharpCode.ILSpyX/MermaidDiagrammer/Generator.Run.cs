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
			var assemblyPath = GetPath(Assembly);
			XmlDocumentationFormatter? xmlDocs = CreateXmlDocsFormatter(assemblyPath);
			ClassDiagrammer model = BuildModel(assemblyPath, xmlDocs);
			GenerateOutput(assemblyPath, model);
		}

		protected virtual XmlDocumentationFormatter? CreateXmlDocsFormatter(string assemblyPath)
		{
			var xmlDocsPath = XmlDocs == null ? Path.ChangeExtension(assemblyPath, ".xml") : GetPath(XmlDocs);
			XmlDocumentationFormatter? xmlDocs = null;

			if (File.Exists(xmlDocsPath))
				xmlDocs = new XmlDocumentationFormatter(new XmlDocumentationProvider(xmlDocsPath), StrippedNamespaces?.ToArray());
			else
				Console.WriteLine("No XML documentation file found. Continuing without.");

			return xmlDocs;
		}

		protected virtual ClassDiagrammer BuildModel(string assemblyPath, XmlDocumentationFormatter? xmlDocs)
			=> new ClassDiagrammerFactory(xmlDocs).BuildModel(assemblyPath, Include, Exclude);

		private string SerializeModel(ClassDiagrammer diagrammer)
		{
			object jsonModel = new {
				diagrammer.OutsideReferences,

				/* convert collections to dictionaries for easier access in ES using
				 * for (let [key, value] of Object.entries(dictionary)) */
				TypesByNamespace = diagrammer.TypesByNamespace.ToDictionary(ns => ns.Key,
					ns => ns.Value.ToDictionary(t => t.Id, t => t))
			};

			// wrap model including the data required for doing the template replacement in a JS build task
			if (JsonOnly)
			{
				jsonModel = new {
					diagrammer.SourceAssemblyName,
					diagrammer.SourceAssemblyVersion,
					BuilderVersion = DecompilerVersionInfo.FullVersionWithCommitHash,
					RepoUrl,
					// pre-serialize to a string so that we don't have to re-serialize it in the JS build task
					Model = Serialize(jsonModel)
				};
			}

			return Serialize(jsonModel);
		}

		private static JsonSerializerOptions serializerOptions = new() {
			WriteIndented = true,
			// avoid outputting null properties unnecessarily
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		private static string Serialize(object json) => JsonSerializer.Serialize(json, serializerOptions);

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
				Console.WriteLine("Successfully generated model.json for HTML diagrammer.");
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

				Console.WriteLine("Successfully generated HTML diagrammer.");
			}

			if (ReportExludedTypes)
			{
				string excludedTypes = model.Excluded.Join(Environment.NewLine);
				File.WriteAllText(Path.Combine(outputFolder, "excluded types.txt"), excludedTypes);
			}
		}

		private protected virtual string GetPath(string pathOrUri)
		{
			// convert file:// style argument, see https://stackoverflow.com/a/38245329
			if (!Uri.TryCreate(pathOrUri, UriKind.RelativeOrAbsolute, out Uri? uri))
				throw new ArgumentException("'{0}' is not a valid URI", pathOrUri);

			// support absolute paths as well as file:// URIs and interpret relative path as relative to the current directory
			return uri.IsAbsoluteUri ? uri.AbsolutePath : pathOrUri;
		}
	}
}