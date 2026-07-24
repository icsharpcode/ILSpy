// Copyright (c) 2020 Siegfried Pammer
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
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// A <see cref="IProjectFileWriter"/> implementation that creates the projects in the SDK style format.
	/// </summary>
	public class ProjectFileWriterSdkStyle : IProjectFileWriter
	{
		const string AspNetCorePrefix = "Microsoft.AspNetCore";
		const string PresentationFrameworkName = "PresentationFramework";
		const string WindowsFormsName = "System.Windows.Forms";
		const string TrueString = "True";
		const string FalseString = "False";
		const string AnyCpuString = "AnyCPU";

		static readonly HashSet<string> ImplicitReferences = new HashSet<string> {
			"mscorlib",
			"netstandard",
			"PresentationFramework",
			"System",
			"System.Diagnostics.Debug",
			"System.Diagnostics.Tools",
			"System.Drawing",
			"System.Runtime",
			"System.Runtime.Extensions",
			"System.Windows.Forms",
			"System.Xaml",
		};

		enum ProjectType { Default, WinForms, Wpf, Web }

		/// <summary>
		/// Gets the default instance of the <see cref="ProjectFileWriterSdkStyle"/> class.
		/// </summary>
		public static ProjectFileWriterSdkStyle Default { get; } = new();

		/// <summary>
		/// Occurs when a custom property group can be written to the project file.
		/// </summary>
		protected event Action<XmlTextWriter, IProjectInfoProvider, IEnumerable<ProjectItemInfo>, MetadataFile>? WriteCustomPropertyGroup;
		/// <summary>
		/// Occurs when a custom item group can be written to the project file.
		/// </summary>
		protected event Action<XmlTextWriter, IProjectInfoProvider, IEnumerable<ProjectItemInfo>, MetadataFile>? WriteCustomItemGroup;

		/// <inheritdoc />
		public void Write(
			TextWriter target,
			IProjectInfoProvider project,
			IEnumerable<ProjectItemInfo> files,
			MetadataFile module)
		{
			using (XmlTextWriter xmlWriter = new XmlTextWriter(target))
			{
				xmlWriter.Formatting = Formatting.Indented;
				Write(xmlWriter, project, files, module);
			}
		}

		void Write(XmlTextWriter xml, IProjectInfoProvider project, IEnumerable<ProjectItemInfo> files, MetadataFile module)
		{
			xml.WriteStartElement("Project");

			var projectType = GetProjectType(module);
			xml.WriteAttributeString("Sdk", GetSdkString(projectType));

			using (new Group(xml, "PropertyGroup"))
			{
				WriteAssemblyInfo(xml, module, project, projectType);
			}
			using (new Group(xml, "PropertyGroup"))
			{
				WriteProjectInfo(xml, project);
			}
			using (new Group(xml, "PropertyGroup"))
			{
				WriteMiscellaneousPropertyGroup(xml, files);
			}
			if (WriteCustomPropertyGroup != null)
			{
				using (new Group(xml, "PropertyGroup"))
				{
					WriteCustomPropertyGroup.Invoke(xml, project, files, module);
				}
			}
			using (new Group(xml, "ItemGroup"))
			{
				WriteResources(xml, files);
			}
			using (new Group(xml, "ItemGroup"))
			{
				WriteReferences(xml, module, project);
			}
			if (WriteCustomItemGroup != null)
			{
				using (new Group(xml, "ItemGroup"))
				{
					WriteCustomItemGroup.Invoke(xml, project, files, module);
				}
			}

			xml.WriteEndElement();
		}

		void WriteAssemblyInfo(XmlTextWriter xml, MetadataFile module, IProjectInfoProvider project, ProjectType projectType)
		{
			xml.WriteElementString("AssemblyName", module.Name);

			// Since we create AssemblyInfo.cs manually, we need to disable the auto-generation
			xml.WriteElementString("GenerateAssemblyInfo", FalseString);

			string platformName;
			CorFlags flags;
			if (module is PEFile { Reader.PEHeaders: var headers } peFile)
			{
				WriteOutputType(xml, headers.IsDll, headers.PEHeader!.Subsystem, projectType);
				platformName = TargetServices.GetPlatformName(peFile);
				flags = headers.CorHeader!.Flags;
			}
			else
			{
				WriteOutputType(xml, isDll: true, Subsystem.Unknown, projectType);
				platformName = AnyCpuString;
				flags = 0;
			}

			WriteDesktopExtensions(xml, projectType);

			xml.WriteElementString("TargetFramework", GetTargetFrameworkMoniker(module, project));

			// 'AnyCPU' is default, so only need to specify platform if it differs
			if (platformName != AnyCpuString)
			{
				xml.WriteElementString("PlatformTarget", platformName);
			}

			if (platformName == AnyCpuString && (flags & CorFlags.Prefers32Bit) != 0)
			{
				xml.WriteElementString("Prefer32Bit", TrueString);
			}
		}

		/// <summary>
		/// Gets the target framework moniker for the specified module and project.
		/// </summary>
		/// <param name="module">The module for which to get the target framework moniker.</param>
		/// <param name="project">The project information provider.</param>
		/// <returns>The target framework moniker.</returns>
		/// <exception cref="NotSupportedException">Thrown if the target framework moniker cannot be determined.</exception>
		protected virtual string GetTargetFrameworkMoniker(MetadataFile module, IProjectInfoProvider project)
		{
			var targetFramework = TargetServices.DetectTargetFramework(module);
			if (targetFramework.Identifier == ".NETFramework" && targetFramework.VersionNumber == 200)
				targetFramework = TargetServices.DetectTargetFrameworkNET20(module, project.AssemblyResolver, targetFramework);

			if (targetFramework.Moniker == null)
			{
				throw new NotSupportedException($"Cannot decompile this assembly to a SDK style project. Use default project format instead.");
			}

			return targetFramework.Moniker;
		}

		static void WriteOutputType(XmlTextWriter xml, bool isDll, Subsystem moduleSubsystem, ProjectType projectType)
		{
			if (!isDll)
			{
				switch (moduleSubsystem)
				{
					case Subsystem.WindowsGui:
						xml.WriteElementString("OutputType", "WinExe");
						break;
					case Subsystem.WindowsCui:
						xml.WriteElementString("OutputType", "Exe");
						break;
				}
			}
			else
			{
				// 'Library' is default, so only need to specify output type for executables (excludes ProjectType.Web)
				if (projectType == ProjectType.Web)
				{
					xml.WriteElementString("OutputType", "Library");
				}
			}
		}

		static void WriteDesktopExtensions(XmlTextWriter xml, ProjectType projectType)
		{
			if (projectType == ProjectType.Wpf)
			{
				xml.WriteElementString("UseWPF", TrueString);
			}
			else if (projectType == ProjectType.WinForms)
			{
				xml.WriteElementString("UseWindowsForms", TrueString);
			}
		}

		static void WriteProjectInfo(XmlTextWriter xml, IProjectInfoProvider project)
		{
			xml.WriteElementString("LangVersion", project.LanguageVersion.ToString().Replace("CSharp", "").Replace('_', '.'));
			xml.WriteElementString("AllowUnsafeBlocks", TrueString);
			xml.WriteElementString("CheckForOverflowUnderflow", project.CheckForOverflowUnderflow ? TrueString : FalseString);

			if (project.StrongNameKeyFile != null)
			{
				xml.WriteElementString("SignAssembly", TrueString);
				xml.WriteElementString("AssemblyOriginatorKeyFile", Path.GetFileName(project.StrongNameKeyFile));
			}
		}

		static void WriteMiscellaneousPropertyGroup(XmlTextWriter xml, IEnumerable<ProjectItemInfo> files)
		{
			var (itemType, fileName) = files.FirstOrDefault(t => t.ItemType == "ApplicationIcon");
			if (fileName != null)
				xml.WriteElementString("ApplicationIcon", fileName);

			(itemType, fileName) = files.FirstOrDefault(t => t.ItemType == "ApplicationManifest");
			if (fileName != null)
				xml.WriteElementString("ApplicationManifest", fileName);

			if (files.Any(t => t.ItemType == "EmbeddedResource"))
				xml.WriteElementString("RootNamespace", string.Empty);
			// TODO: We should add CustomToolNamespace for resources, otherwise we should add empty RootNamespace
		}

		static void WriteResources(XmlTextWriter xml, IEnumerable<ProjectItemInfo> files)
		{
			// remove phase
			foreach (var item in files.Where(t => t.ItemType == "EmbeddedResource"))
			{
				string buildAction = Path.GetExtension(item.FileName).ToUpperInvariant() switch {
					".CS" => "Compile",
					".RESX" => "EmbeddedResource",
					_ => "None"
				};
				if (buildAction == "EmbeddedResource")
					continue;

				xml.WriteStartElement(buildAction);
				xml.WriteAttributeString("Remove", item.FileName);
				xml.WriteEndElement();
			}

			// include phase
			foreach (var item in files.Where(t => t.ItemType == "EmbeddedResource"))
			{
				if (Path.GetExtension(item.FileName) == ".resx")
					continue;

				xml.WriteStartElement("EmbeddedResource");
				xml.WriteAttributeString("Include", item.FileName);
				if (item.AdditionalProperties != null)
				{
					foreach (var (key, value) in item.AdditionalProperties)
						xml.WriteAttributeString(key, value);
				}
				xml.WriteEndElement();
			}
		}

		void WriteReferences(XmlTextWriter xml, MetadataFile module, IProjectInfoProvider project)
		{
			foreach (var reference in GetReferences(module, project))
			{
				WriteReference(xml, reference, project);
			}
		}

		/// <summary>
		/// Gets the assembly references for the specified module and project, excluding implicit references and shared assemblies.
		/// </summary>
		/// <param name="module">The module for which to get the assembly references.</param>
		/// <param name="project">The project information provider.</param>
		/// <returns>An enumerable of assembly references.</returns>
		protected virtual IEnumerable<AssemblyReference> GetReferences(MetadataFile module, IProjectInfoProvider project)
		{
			bool isNetCoreApp = TargetServices.DetectTargetFramework(module).Identifier == ".NETCoreApp";
			var targetPacks = new HashSet<string>();
			if (isNetCoreApp)
			{
				targetPacks.Add("Microsoft.NETCore.App");
				switch (GetProjectType(module))
				{
					case ProjectType.WinForms:
					case ProjectType.Wpf:
						targetPacks.Add("Microsoft.WindowsDesktop.App");
						break;
					case ProjectType.Web:
						targetPacks.Add("Microsoft.AspNetCore.App");
						targetPacks.Add("Microsoft.AspNetCore.All");
						break;
				}
			}
			foreach (var reference in module.AssemblyReferences)
			{
				if (ImplicitReferences.Contains(reference.Name))
				{
					continue;
				}
				if (isNetCoreApp && project.AssemblyReferenceClassifier.IsSharedAssembly(reference, out string runtimePack) && targetPacks.Contains(runtimePack))
				{
					continue;
				}
				yield return reference;
			}
		}

		/// <summary>
		/// Writes an assembly reference to the project file.
		/// </summary>
		/// <param name="xml">The XML writer used to write the project file.</param>
		/// <param name="reference">The assembly reference to write.</param>
		/// <param name="project">The project information provider.</param>
		protected virtual void WriteReference(XmlTextWriter xml, AssemblyReference reference, IProjectInfoProvider project)
		{
			xml.WriteStartElement("Reference");
			xml.WriteAttributeString("Include", reference.Name);

			var assembly = project.AssemblyResolver.Resolve(reference);
			if (assembly != null && !project.AssemblyReferenceClassifier.IsGacAssembly(reference))
			{
				xml.WriteElementString("HintPath", FileUtility.GetRelativePath(project.TargetDirectory, assembly.FileName));
			}

			xml.WriteEndElement();
		}

		static string GetSdkString(ProjectType projectType)
		{
			switch (projectType)
			{
				case ProjectType.WinForms:
				case ProjectType.Wpf:
					return "Microsoft.NET.Sdk.WindowsDesktop";
				case ProjectType.Web:
					return "Microsoft.NET.Sdk.Web";
				default:
					return "Microsoft.NET.Sdk";
			}
		}

		static ProjectType GetProjectType(MetadataFile module)
		{
			foreach (var referenceName in module.AssemblyReferences.Select(r => r.Name))
			{
				if (referenceName.StartsWith(AspNetCorePrefix, StringComparison.Ordinal))
				{
					return ProjectType.Web;
				}

				if (referenceName == PresentationFrameworkName)
				{
					return ProjectType.Wpf;
				}

				if (referenceName == WindowsFormsName)
				{
					return ProjectType.WinForms;
				}
			}

			return ProjectType.Default;
		}

		readonly struct Group : IDisposable
		{
			readonly XmlTextWriter xml;

			public Group(XmlTextWriter xml, string name)
			{
				this.xml = xml;
				xml.WriteStartElement(name);
			}

			public void Dispose()
			{
				xml.WriteEndElement();
			}
		}
	}
}
