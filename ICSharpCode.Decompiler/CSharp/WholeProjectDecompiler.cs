// Copyright (c) 2016 Daniel Grunwald
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;
using System.Threading;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Decompiles an assembly into a visual studio project file.
	/// </summary>
	public class WholeProjectDecompiler
	{
		#region Settings
		DecompilerSettings settings = new DecompilerSettings();

		public DecompilerSettings Settings {
			get {
				return settings;
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				settings = value;
			}
		}

		/// <summary>
		/// The MSBuild ProjectGuid to use for the new project.
		/// <c>null</c> to automatically generate a new GUID.
		/// </summary>
		public Guid? ProjectGuid { get; set; }

		public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
		#endregion

		// per-run members
		HashSet<string> directories = new HashSet<string>(Platform.FileNameComparer);

		/// <summary>
		/// The target directory that the decompiled files are written to.
		/// </summary>
		/// <remarks>
		/// This field is set by DecompileProject() and protected so that overridden protected members
		/// can access it.
		/// </remarks>
		protected string targetDirectory;

		public void DecompileProject(ModuleDefinition moduleDefinition, string targetDirectory, CancellationToken cancellationToken = default(CancellationToken))
		{
			string projectFileName = Path.Combine(targetDirectory, CleanUpFileName(moduleDefinition.Assembly.Name.Name) + ".csproj");
			using (var writer = new StreamWriter(projectFileName)) {
				DecompileProject(moduleDefinition, targetDirectory, writer, cancellationToken);
			}
		}

		public void DecompileProject(ModuleDefinition moduleDefinition, string targetDirectory, TextWriter projectFileWriter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(targetDirectory)) {
				throw new InvalidOperationException("Must set TargetDirectory");
			}
			this.targetDirectory = targetDirectory;
			directories.Clear();
			var files = WriteCodeFilesInProject(moduleDefinition, cancellationToken).ToList();
			files.AddRange(WriteResourceFilesInProject(moduleDefinition));
			WriteProjectFile(projectFileWriter, files, moduleDefinition);
		}

		enum LanguageTargets
		{
			None,
			Portable
		}

		#region WriteProjectFile
		void WriteProjectFile(TextWriter writer, IEnumerable<Tuple<string, string>> files, ModuleDefinition module)
		{
			const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			string platformName = GetPlatformName(module);
			Guid guid = this.ProjectGuid ?? Guid.NewGuid();
			using (XmlTextWriter w = new XmlTextWriter(writer)) {
				w.Formatting = Formatting.Indented;
				w.WriteStartDocument();
				w.WriteStartElement("Project", ns);
				w.WriteAttributeString("ToolsVersion", "4.0");
				w.WriteAttributeString("DefaultTargets", "Build");

				w.WriteStartElement("PropertyGroup");
				w.WriteElementString("ProjectGuid", guid.ToString("B").ToUpperInvariant());

				w.WriteStartElement("Configuration");
				w.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
				w.WriteValue("Debug");
				w.WriteEndElement(); // </Configuration>

				w.WriteStartElement("Platform");
				w.WriteAttributeString("Condition", " '$(Platform)' == '' ");
				w.WriteValue(platformName);
				w.WriteEndElement(); // </Platform>

				switch (module.Kind) {
					case ModuleKind.Windows:
						w.WriteElementString("OutputType", "WinExe");
						break;
					case ModuleKind.Console:
						w.WriteElementString("OutputType", "Exe");
						break;
					default:
						w.WriteElementString("OutputType", "Library");
						break;
				}

				w.WriteElementString("AssemblyName", module.Assembly.Name.Name);
				bool useTargetFrameworkAttribute = false;
				LanguageTargets languageTargets = LanguageTargets.None;
				var targetFrameworkAttribute = module.Assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");
				if (targetFrameworkAttribute != null && targetFrameworkAttribute.ConstructorArguments.Any()) {
					string frameworkName = (string)targetFrameworkAttribute.ConstructorArguments[0].Value;
					string[] frameworkParts = frameworkName.Split(',');
					string frameworkIdentifier = frameworkParts.FirstOrDefault(a => !a.StartsWith("Version=", StringComparison.OrdinalIgnoreCase) && !a.StartsWith("Profile=", StringComparison.OrdinalIgnoreCase));
					if (frameworkIdentifier != null) {
						w.WriteElementString("TargetFrameworkIdentifier", frameworkIdentifier);
						switch (frameworkIdentifier) {
							case ".NETPortable":
								languageTargets = LanguageTargets.Portable;
								break;
						}
					}
					string frameworkVersion = frameworkParts.FirstOrDefault(a => a.StartsWith("Version=", StringComparison.OrdinalIgnoreCase));
					if (frameworkVersion != null) {
						w.WriteElementString("TargetFrameworkVersion", frameworkVersion.Substring("Version=".Length));
						useTargetFrameworkAttribute = true;
					}
					string frameworkProfile = frameworkParts.FirstOrDefault(a => a.StartsWith("Profile=", StringComparison.OrdinalIgnoreCase));
					if (frameworkProfile != null)
						w.WriteElementString("TargetFrameworkProfile", frameworkProfile.Substring("Profile=".Length));
				}
				if (!useTargetFrameworkAttribute) {
					switch (module.Runtime) {
						case TargetRuntime.Net_1_0:
							w.WriteElementString("TargetFrameworkVersion", "v1.0");
							break;
						case TargetRuntime.Net_1_1:
							w.WriteElementString("TargetFrameworkVersion", "v1.1");
							break;
						case TargetRuntime.Net_2_0:
							w.WriteElementString("TargetFrameworkVersion", "v2.0");
							// TODO: Detect when .NET 3.0/3.5 is required
							break;
						default:
							w.WriteElementString("TargetFrameworkVersion", "v4.0");
							break;
					}
				}
				w.WriteElementString("WarningLevel", "4");
				w.WriteElementString("AllowUnsafeBlocks", "True");

				w.WriteEndElement(); // </PropertyGroup>

				w.WriteStartElement("PropertyGroup"); // platform-specific
				w.WriteAttributeString("Condition", " '$(Platform)' == '" + platformName + "' ");
				w.WriteElementString("PlatformTarget", platformName);
				w.WriteEndElement(); // </PropertyGroup> (platform-specific)

				w.WriteStartElement("PropertyGroup"); // Debug
				w.WriteAttributeString("Condition", " '$(Configuration)' == 'Debug' ");
				w.WriteElementString("OutputPath", "bin\\Debug\\");
				w.WriteElementString("DebugSymbols", "true");
				w.WriteElementString("DebugType", "full");
				w.WriteElementString("Optimize", "false");
				w.WriteEndElement(); // </PropertyGroup> (Debug)

				w.WriteStartElement("PropertyGroup"); // Release
				w.WriteAttributeString("Condition", " '$(Configuration)' == 'Release' ");
				w.WriteElementString("OutputPath", "bin\\Release\\");
				w.WriteElementString("DebugSymbols", "true");
				w.WriteElementString("DebugType", "pdbonly");
				w.WriteElementString("Optimize", "true");
				w.WriteEndElement(); // </PropertyGroup> (Release)


				w.WriteStartElement("ItemGroup"); // References
				foreach (AssemblyNameReference r in module.AssemblyReferences) {
					if (r.Name != "mscorlib") {
						w.WriteStartElement("Reference");
						w.WriteAttributeString("Include", r.Name);
						var asm = module.AssemblyResolver.Resolve(r);
						if (!IsGacAssembly(r, asm)) {
							if (asm != null) {
								w.WriteElementString("HintPath", asm.MainModule.FileName);
							}
						}
						w.WriteEndElement();
					}
				}
				w.WriteEndElement(); // </ItemGroup> (References)

				foreach (IGrouping<string, string> gr in (from f in files group f.Item2 by f.Item1 into g orderby g.Key select g)) {
					w.WriteStartElement("ItemGroup");
					foreach (string file in gr.OrderBy(f => f, StringComparer.OrdinalIgnoreCase)) {
						w.WriteStartElement(gr.Key);
						w.WriteAttributeString("Include", file);
						w.WriteEndElement();
					}
					w.WriteEndElement();
				}
				switch (languageTargets) {
					case LanguageTargets.Portable:
						w.WriteStartElement("Import");
						w.WriteAttributeString("Project", "$(MSBuildExtensionsPath32)\\Microsoft\\Portable\\$(TargetFrameworkVersion)\\Microsoft.Portable.CSharp.targets");
						w.WriteEndElement();
						break;
					default:
						w.WriteStartElement("Import");
						w.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
						w.WriteEndElement();
						break;
				}

				w.WriteEndDocument();
			}
		}

		protected virtual bool IsGacAssembly(AssemblyNameReference r, AssemblyDefinition asm)
		{
			return false;
		}
		#endregion

		#region WriteCodeFilesInProject
		protected virtual bool IncludeTypeWhenDecompilingProject(TypeDefinition type)
		{
			if (type.Name == "<Module>" || CSharpDecompiler.MemberIsHidden(type, settings))
				return false;
			if (type.Namespace == "XamlGeneratedNamespace" && type.Name == "GeneratedInternalTypeHelper")
				return false;
			return true;
		}

		CSharpDecompiler CreateDecompiler(DecompilerTypeSystem ts)
		{
			var decompiler = new CSharpDecompiler(ts, settings);
			decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			decompiler.AstTransforms.Add(new RemoveCLSCompliantAttribute());
			return decompiler;
		}

		IEnumerable<Tuple<string, string>> WriteAssemblyInfo(DecompilerTypeSystem ts, CancellationToken cancellationToken)
		{
			var decompiler = CreateDecompiler(ts);
			decompiler.CancellationToken = cancellationToken;
			decompiler.AstTransforms.Add(new RemoveCompilerGeneratedAssemblyAttributes());
			SyntaxTree syntaxTree = decompiler.DecompileModuleAndAssemblyAttributes();

			const string prop = "Properties";
			if (directories.Add(prop))
				Directory.CreateDirectory(Path.Combine(targetDirectory, prop));
			string assemblyInfo = Path.Combine(prop, "AssemblyInfo.cs");
			using (StreamWriter w = new StreamWriter(Path.Combine(targetDirectory, assemblyInfo))) {
				syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, settings.CSharpFormattingOptions));
			}
			return new Tuple<string, string>[] { Tuple.Create("Compile", assemblyInfo) };
		}

		IEnumerable<Tuple<string, string>> WriteCodeFilesInProject(ModuleDefinition module, CancellationToken cancellationToken)
		{
			var files = module.Types.Where(IncludeTypeWhenDecompilingProject).GroupBy(
				delegate (TypeDefinition type) {
					string file = CleanUpFileName(type.Name) + ".cs";
					if (string.IsNullOrEmpty(type.Namespace)) {
						return file;
					} else {
						string dir = CleanUpFileName(type.Namespace);
						if (directories.Add(dir))
							Directory.CreateDirectory(Path.Combine(targetDirectory, dir));
						return Path.Combine(dir, file);
					}
				}, StringComparer.OrdinalIgnoreCase).ToList();
			DecompilerTypeSystem ts = new DecompilerTypeSystem(module);
			Parallel.ForEach(
				files,
				new ParallelOptions {
					MaxDegreeOfParallelism = this.MaxDegreeOfParallelism,
					CancellationToken = cancellationToken
				},
				delegate (IGrouping<string, TypeDefinition> file) {
					using (StreamWriter w = new StreamWriter(Path.Combine(targetDirectory, file.Key))) {
						CSharpDecompiler decompiler = CreateDecompiler(ts);
						decompiler.CancellationToken = cancellationToken;
						var syntaxTree = decompiler.DecompileTypes(file.ToArray());
						syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, settings.CSharpFormattingOptions));
					}
				});
			return files.Select(f => Tuple.Create("Compile", f.Key)).Concat(WriteAssemblyInfo(ts, cancellationToken));
		}
		#endregion

		#region WriteResourceFilesInProject
		protected virtual IEnumerable<Tuple<string, string>> WriteResourceFilesInProject(ModuleDefinition module)
		{
			foreach (EmbeddedResource r in module.Resources.OfType<EmbeddedResource>()) {
				Stream stream = r.GetResourceStream();
				stream.Position = 0;

				IEnumerable<DictionaryEntry> entries;
				if (r.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) {
					if (GetEntries(stream, out entries) && entries.All(e => e.Value is Stream)) {
						foreach (var pair in entries) {
							string fileName = Path.Combine(((string)pair.Key).Split('/').Select(p => CleanUpFileName(p)).ToArray());
							string dirName = Path.GetDirectoryName(fileName);
							if (!string.IsNullOrEmpty(dirName) && directories.Add(dirName)) {
								Directory.CreateDirectory(Path.Combine(targetDirectory, dirName));
							}
							Stream entryStream = (Stream)pair.Value;
							entryStream.Position = 0;
							WriteResourceToFile(Path.Combine(targetDirectory, fileName), (string)pair.Key, entryStream);
						}
					} else {
						stream.Position = 0;
						string fileName = GetFileNameForResource(Path.ChangeExtension(r.Name, ".resource"));
						WriteResourceToFile(fileName, r.Name, stream);
					}
				} else {
					string fileName = GetFileNameForResource(r.Name);
					using (FileStream fs = new FileStream(Path.Combine(targetDirectory, fileName), FileMode.Create, FileAccess.Write)) {
						stream.Position = 0;
						stream.CopyTo(fs);
					}
					yield return Tuple.Create("EmbeddedResource", fileName);
				}
			}
		}

		protected virtual IEnumerable<Tuple<string, string>> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				entryStream.CopyTo(fs);
			}
			yield return Tuple.Create("EmbeddedResource", fileName);
		}

		string GetFileNameForResource(string fullName)
		{
			string[] splitName = fullName.Split('.');
			string fileName = CleanUpFileName(fullName);
			for (int i = splitName.Length - 1; i > 0; i--) {
				string ns = string.Join(".", splitName, 0, i);
				if (directories.Contains(ns)) {
					string name = string.Join(".", splitName, i, splitName.Length - i);
					fileName = Path.Combine(ns, CleanUpFileName(name));
					break;
				}
			}
			return fileName;
		}

		bool GetEntries(Stream stream, out IEnumerable<DictionaryEntry> entries)
		{
			try {
				entries = new ResourceSet(stream).Cast<DictionaryEntry>();
				return true;
			} catch (ArgumentException) {
				entries = null;
				return false;
			}
		}
		#endregion

		/// <summary>
		/// Cleans up a node name for use as a file name.
		/// </summary>
		public static string CleanUpFileName(string text)
		{
			int pos = text.IndexOf(':');
			if (pos > 0)
				text = text.Substring(0, pos);
			pos = text.IndexOf('`');
			if (pos > 0)
				text = text.Substring(0, pos);
			text = text.Trim();
			foreach (char c in Path.GetInvalidFileNameChars())
				text = text.Replace(c, '-');
			return text;
		}

		public static string GetPlatformName(ModuleDefinition module)
		{
			switch (module.Architecture) {
				case TargetArchitecture.I386:
					if ((module.Attributes & ModuleAttributes.Preferred32Bit) == ModuleAttributes.Preferred32Bit)
						return "AnyCPU";
					else if ((module.Attributes & ModuleAttributes.Required32Bit) == ModuleAttributes.Required32Bit)
						return "x86";
					else
						return "AnyCPU";
				case TargetArchitecture.AMD64:
					return "x64";
				case TargetArchitecture.IA64:
					return "Itanium";
				default:
					return module.Architecture.ToString();
			}
		}
	}
}
