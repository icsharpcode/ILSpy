
// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	sealed class AssemblyTreeNode : ILSpyTreeNode
	{
		readonly LoadedAssembly assembly;
		string? loadError;
		MetadataFile? cachedModule;

		public LoadedAssembly LoadedAssembly => assembly;

		public AssemblyTreeNode(LoadedAssembly assembly)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			this.assembly = assembly;
			LazyLoading = true;
			_ = InitAsync();
		}

		async Task InitAsync()
		{
			try
			{
				var loadResult = await assembly.GetLoadResultAsync();
				cachedModule = loadResult.MetadataFile;
				if (cachedModule == null && loadResult.Package == null)
				{
					loadError = File.Exists(assembly.FileName)
						? $"Failed to load '{assembly.FileName}'."
						: $"File not found:\n{assembly.FileName}";
				}
			}
			catch (Exception ex)
			{
				loadError = $"Failed to load '{assembly.FileName}':\n{ex.GetBaseException().Message}";
			}
			RaisePropertyChanged(nameof(Text));
			RaisePropertyChanged(nameof(Icon));
			RaisePropertyChanged(nameof(ToolTip));
			RaisePropertyChanged(nameof(ShowExpander));
		}

		public override object Text => assembly.Text;

		// ToString is the stable identity used by SessionSettings.ActiveTreeViewPath — must not
		// depend on the active language. The full file path uniquely identifies the assembly.
		public override string ToString() => assembly.FileName;

		public override object Icon {
			get {
				if (assembly.HasLoadError || loadError != null)
					return Images.Images.AssemblyWarning;
				if (!assembly.IsLoaded)
					return Images.Images.AssemblyLoading;

				var loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
				if (loadResult.Package != null)
				{
					return loadResult.Package.Kind switch {
						LoadedPackage.PackageKind.Zip => Images.Images.NuGet,
						_ => Images.Images.Library,
					};
				}
				if (loadResult.MetadataFile != null)
				{
					return loadResult.MetadataFile.Kind switch {
						MetadataFile.MetadataFileKind.PortableExecutable => Images.Images.Assembly,
						MetadataFile.MetadataFileKind.ProgramDebugDatabase => Images.Images.ProgramDebugDatabase,
						MetadataFile.MetadataFileKind.WebCIL => Images.Images.WebAssemblyFile,
						_ => Images.Images.MetadataFile,
					};
				}
				return Images.Images.Assembly;
			}
		}

		public override object? ToolTip {
			get {
				if (assembly.HasLoadError || loadError != null)
					return loadError ?? "Assembly could not be loaded.";
				if (!assembly.IsLoaded || cachedModule == null)
					return assembly.FileName;

				var tb = new TextBlock { TextWrapping = TextWrapping.Wrap };

				var metadata = cachedModule.Metadata;
				if (metadata?.IsAssembly == true && metadata.TryGetFullAssemblyName(out var assemblyName))
				{
					tb.Inlines!.Add(BoldRun("Name: "));
					tb.Inlines.Add(new Run(assemblyName));
					tb.Inlines.Add(new LineBreak());
				}

				tb.Inlines!.Add(BoldRun("Location: "));
				tb.Inlines.Add(new Run(assembly.FileName));

				if (cachedModule is PEFile peFile)
				{
					tb.Inlines.Add(new LineBreak());
					tb.Inlines.Add(BoldRun("Architecture: "));
					tb.Inlines.Add(new Run(GetPlatformDisplayName(peFile)));
				}

				string? runtime = GetRuntimeDisplayName(cachedModule);
				if (runtime != null)
				{
					tb.Inlines.Add(new LineBreak());
					tb.Inlines.Add(BoldRun("Runtime: "));
					tb.Inlines.Add(new Run(runtime));
				}

				return tb;
			}
		}

		public override bool ShowExpander => !assembly.HasLoadError && loadError == null && base.ShowExpander;

		public override bool IsAutoLoaded => assembly.IsAutoLoaded;

		protected override void LoadChildren()
		{
			var module = cachedModule;
			if (module == null)
			{
				try
				{
					module = assembly.GetLoadResultAsync().GetAwaiter().GetResult().MetadataFile;
				}
				catch
				{
					return;
				}
			}
			if (module == null)
				return;

			if (module.Kind != MetadataFile.MetadataFileKind.PortableExecutable
				&& module.Kind != MetadataFile.MetadataFileKind.WebCIL)
			{
				return;
			}

			Children.Add(new ReferenceFolderTreeNode(module, this));

			var metadata = module.Metadata;
			var namespaces = metadata.TypeDefinitions
				.Where(t => metadata.GetTypeDefinition(t).GetDeclaringType().IsNil)
				.Select(t => metadata.GetString(metadata.GetTypeDefinition(t).Namespace))
				.Where(ns => !string.IsNullOrEmpty(ns))
				.Distinct()
				.OrderBy(ns => ns, NaturalStringComparer.Instance);

			foreach (var ns in namespaces)
				Children.Add(new NamespaceTreeNode(ns, module));

			var globalTypes = metadata.TypeDefinitions
				.Where(t => {
					var td = metadata.GetTypeDefinition(t);
					return td.GetDeclaringType().IsNil
						&& string.IsNullOrEmpty(metadata.GetString(td.Namespace));
				});

			foreach (var t in globalTypes)
				Children.Add(new TypeTreeNode(t, module));
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			void HandleException(Exception ex, string message)
			{
				language.WriteCommentLine(output, message);
				output.WriteLine();
				output.MarkFoldStart("Exception details", true);
				output.Write(ex.ToString());
				output.MarkFoldEnd();
			}

			try
			{
				var loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
				if (loadResult.MetadataFile != null)
				{
					switch (loadResult.MetadataFile.Kind)
					{
						case MetadataFile.MetadataFileKind.ProgramDebugDatabase:
						case MetadataFile.MetadataFileKind.Metadata:
							output.WriteLine("// " + assembly.FileName);
							break;
						default:
							language.DecompileAssembly(assembly, output, options);
							break;
					}
				}
				else if (loadResult.Package != null)
				{
					output.WriteLine("// " + assembly.FileName);
					DecompilePackage(loadResult.Package, output);
				}
				else if (loadResult.FileLoadException != null)
				{
					HandleException(loadResult.FileLoadException, loadResult.FileLoadException.Message);
				}
			}
			catch (BadImageFormatException badImage)
			{
				HandleException(badImage, "This file does not contain a managed assembly.");
			}
			catch (FileNotFoundException fileNotFound) when (options.SaveAsProjectDirectory == null)
			{
				HandleException(fileNotFound, "The file was not found.");
			}
			catch (DirectoryNotFoundException dirNotFound) when (options.SaveAsProjectDirectory == null)
			{
				HandleException(dirNotFound, "The directory was not found.");
			}
			catch (MetadataFileNotSupportedException notSupported)
			{
				HandleException(notSupported, notSupported.Message);
			}
		}

		static void DecompilePackage(LoadedPackage package, ITextOutput output)
		{
			switch (package.Kind)
			{
				case LoadedPackage.PackageKind.Zip:
					output.WriteLine("// File format: .zip file");
					break;
				case LoadedPackage.PackageKind.Bundle:
					var header = package.BundleHeader;
					output.WriteLine($"// File format: .NET bundle {header.MajorVersion}.{header.MinorVersion}");
					break;
			}
			output.WriteLine();
			output.WriteLine("Entries:");
			foreach (var entry in package.Entries)
			{
				output.WriteLine($" {entry.Name} ({entry.TryGetLength()} bytes)");
			}
		}

		static Run BoldRun(string text) => new(text) { FontWeight = FontWeight.Bold };

		static string GetPlatformDisplayName(PEFile file)
		{
			return file.Reader.PEHeaders.CoffHeader.Machine switch {
				System.Reflection.PortableExecutable.Machine.I386 => "x86",
				System.Reflection.PortableExecutable.Machine.Amd64 => "x64",
				System.Reflection.PortableExecutable.Machine.IA64 => "Itanium",
				System.Reflection.PortableExecutable.Machine.Arm => "ARM",
				System.Reflection.PortableExecutable.Machine.Arm64 => "ARM64",
				_ => file.Reader.PEHeaders.CoffHeader.Machine.ToString(),
			};
		}

		static string? GetRuntimeDisplayName(MetadataFile module)
		{
			return module.Metadata.MetadataKind switch {
				System.Reflection.Metadata.MetadataKind.Ecma335 => module.Metadata.MetadataVersion,
				System.Reflection.Metadata.MetadataKind.WindowsMetadata => "WinRT",
				System.Reflection.Metadata.MetadataKind.ManagedWindowsMetadata => "Managed WinRT",
				_ => null,
			};
		}
	}
}
