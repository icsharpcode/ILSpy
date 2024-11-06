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

using System.IO;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	public partial class GenerateHtmlDiagrammer
	{
		/// <summary>A helper for loading resources embedded in the nested html folder.</summary>
		private static class EmbeddedResource
		{
			internal static string ReadText(string resourceName)
			{
				Stream stream = GetStream(resourceName);
				using StreamReader reader = new(stream);
				return reader.ReadToEnd();
			}

			internal static void CopyTo(string outputFolder, string resourceName)
			{
				Stream resourceStream = GetStream(resourceName);
				using FileStream output = new(Path.Combine(outputFolder, resourceName), FileMode.Create, FileAccess.Write);
				resourceStream.CopyTo(output);
			}

			private static Stream GetStream(string resourceName)
			{
				var type = typeof(EmbeddedResource);
				var assembly = type.Assembly;
				var fullResourceName = $"{type.Namespace}.html.{resourceName}";
				Stream? stream = assembly.GetManifestResourceStream(fullResourceName);

				if (stream == null)
					throw new FileNotFoundException("Resource not found.", fullResourceName);

				return stream;
			}
		}
	}
}