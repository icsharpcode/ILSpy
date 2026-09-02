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
		const string WindowsFormsIntegrationName = "WindowsFormsIntegration";
		const string NetCoreAppIdentifier = ".NETCoreApp";
		const string WindowsPlatformName = "Windows";
		const string TrueString = "True";
		const string FalseString = "False";
		const string AnyCpuString = "AnyCPU";

		/// <summary>
		/// References the .NET SDK adds by itself when UseWPF is set, both through the implicit
		/// Microsoft.WindowsDesktop.App framework reference on .NET Core and through the implicit
		/// .NET Framework references. Listing them a second time is a duplicate reference (MSB3243),
		/// or an unresolvable one (MSB3245) once the hint path no longer points anywhere.
		/// Membership must not be decided by probing the machine that runs the export: the
		/// Windows Desktop runtime pack is absent on non-Windows hosts, and the exported project
		/// has to come out the same everywhere.
		/// The SDK gates the version-specific members of this set on the target framework version,
		/// which an assembly satisfies by construction: it can only reference what its own target
		/// framework ships.
		/// </summary>
		static readonly HashSet<string> WpfImplicitReferences = new HashSet<string> {
			"PresentationCore",
			"PresentationFramework",
			"System.Windows.Controls.Ribbon",
			"System.Xaml",
			"UIAutomationClient",
			"UIAutomationClientSideProviders",
			"UIAutomationProvider",
			"UIAutomationTypes",
			"WindowsBase",
		};

		/// <summary>
		/// References the SDK adds for every project, whatever it uses: they are either implicit
		/// for .NET Framework targets or part of the Microsoft.NETCore.App shared framework.
		/// </summary>
		static readonly HashSet<string> ImplicitReferences = new HashSet<string> {
			"mscorlib",
			"netstandard",
			"System",
			"System.Diagnostics.Debug",
			"System.Diagnostics.Tools",
			"System.Drawing",
			"System.Runtime",
			"System.Runtime.Extensions",
		};

		/// <summary>
		/// Item types that must not be listed as project items: source files are covered by the
		/// SDK's default Compile glob, the icon and the manifest are written as properties, and
		/// embedded resources have their own remove/include phase.
		/// </summary>
		static readonly HashSet<string> ItemTypesWrittenElsewhere = new HashSet<string> {
			"ApplicationIcon",
			"ApplicationManifest",
			"Compile",
			"EmbeddedResource",
		};

		/// <summary>
		/// What an assembly uses of the Windows desktop stacks. WPF and Windows Forms are not
		/// alternatives: an assembly can use both, and each brings its own set of implicit
		/// references and its own SDK property.
		/// </summary>
		[Flags]
		enum ProjectType
		{
			Default = 0,
			WinForms = 1,
			Wpf = 2,
			Web = 4,
			Desktop = WinForms | Wpf,
		}

		/// <summary>
		/// Gets the default instance of the <see cref="ProjectFileWriterSdkStyle"/> class.
		/// </summary>
		public static ProjectFileWriterSdkStyle Default { get; } = new();

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
			xml.WriteAttributeString("Sdk", GetSdkString(projectType, TargetServices.DetectTargetFramework(module)));

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
			var customProperties = GetCustomProperties(project, files, module);
			if (customProperties != null)
			{
				using (new Group(xml, "PropertyGroup"))
				{
					foreach (var (name, value) in customProperties)
					{
						xml.WriteElementString(name, value);
					}
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

			string moniker = GetTargetFrameworkMoniker(module, project);
			string? platform = GetTargetPlatform(module, projectType);
			if (platform != null)
			{
				moniker += "-" + platform;
			}
			xml.WriteElementString("TargetFramework", moniker);

			string? minVersion = platform != null ? GetTargetPlatformMinVersion(module, platform) : null;
			if (minVersion != null)
			{
				xml.WriteElementString("TargetPlatformMinVersion", minVersion);
			}

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
		/// Gets the target platform part of the target framework moniker, e.g. "windows7.0" in
		/// "net10.0-windows7.0". Only .NET 5 and later monikers carry one; "net472" or
		/// "netcoreapp3.1" with a platform suffix names no target pack at all.
		/// </summary>
		/// <returns>The lower-case platform, or null when the moniker takes none.</returns>
		static string? GetTargetPlatform(MetadataFile module, ProjectType projectType)
		{
			var targetFramework = TargetServices.DetectTargetFramework(module);
			if (targetFramework.Identifier != NetCoreAppIdentifier || targetFramework.VersionNumber < 500)
			{
				return null;
			}

			string? platform = TargetServices.DetectTargetPlatform(module);
			if (platform == null && (projectType & ProjectType.Desktop) != 0)
			{
				// Assemblies built before platform-suffixed monikers existed carry no
				// TargetPlatformAttribute, but WPF and Windows Forms are Windows-only and the SDK
				// rejects the project outright (NETSDK1136) unless the moniker says so.
				platform = WindowsPlatformName;
			}

			return platform?.ToLowerInvariant();
		}

		/// <summary>
		/// Gets the lowest platform version the assembly supports, when it is lower than the version
		/// it targets. Without it the exported project would raise its own floor to the target version.
		/// </summary>
		static string? GetTargetPlatformMinVersion(MetadataFile module, string platform)
		{
			string? supported = TargetServices.DetectSupportedOSPlatform(module)?.ToLowerInvariant();
			if (supported == null)
			{
				return null;
			}

			var (supportedName, supportedVersion) = SplitPlatform(supported);
			var (platformName, platformVersion) = SplitPlatform(platform);
			if (supportedName != platformName || supportedVersion.Length == 0 || supportedVersion == platformVersion)
			{
				return null;
			}

			return supportedVersion;
		}

		/// <summary>
		/// Splits a platform such as "windows10.0.17763.0" into its name and its version.
		/// </summary>
		static (string Name, string Version) SplitPlatform(string platform)
		{
			int versionStart = 0;
			while (versionStart < platform.Length && !char.IsDigit(platform[versionStart]))
			{
				versionStart++;
			}

			return (platform.Substring(0, versionStart), platform.Substring(versionStart));
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
				if (projectType.HasFlag(ProjectType.Web))
				{
					xml.WriteElementString("OutputType", "Library");
				}
			}
		}

		static void WriteDesktopExtensions(XmlTextWriter xml, ProjectType projectType)
		{
			if (projectType.HasFlag(ProjectType.Wpf))
			{
				xml.WriteElementString("UseWPF", TrueString);
			}
			if (projectType.HasFlag(ProjectType.WinForms))
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

		/// <summary>
		/// Gets custom properties to be added to the project file. Override this method to provide additional properties.
		/// </summary>
		/// <param name="project">The project information provider.</param>
		/// <param name="files">The collection of project item information.</param>
		/// <param name="module">The metadata file representing the module.</param>
		/// <returns>An enumerable of custom properties as name-value pairs. Null if no custom properties are provided.</returns>
		protected virtual IEnumerable<(string, string)>? GetCustomProperties(IProjectInfoProvider project, IEnumerable<ProjectItemInfo> files, MetadataFile module)
		{
			return null;
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

			// Every other item type the export produced, "Page" for XAML recovered from BAML above
			// all. Files the project does not list are files the exported project cannot build.
			foreach (var group in files.Where(t => !ItemTypesWrittenElsewhere.Contains(t.ItemType))
				.GroupBy(t => t.ItemType).OrderBy(g => g.Key, StringComparer.Ordinal))
			{
				var items = group.OrderBy(t => t.FileName, StringComparer.OrdinalIgnoreCase).ToList();

				// remove phase: an SDK glob may already have claimed these files - a WPF project
				// (UseWPF) globs **/*.xaml into Page - and the same file in an item type twice is
				// a build error. Removing first, in the same item group, keeps the explicit item
				// with its metadata and drops the globbed one.
				foreach (var item in items)
				{
					xml.WriteStartElement(group.Key);
					xml.WriteAttributeString("Remove", item.FileName);
					xml.WriteEndElement();
				}

				// include phase
				foreach (var item in items)
				{
					xml.WriteStartElement(group.Key);
					xml.WriteAttributeString("Include", item.FileName);
					if (item.AdditionalProperties != null)
					{
						foreach (var (key, value) in item.AdditionalProperties)
							xml.WriteAttributeString(key, value);
					}
					xml.WriteEndElement();
				}
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
			bool isNetCoreApp = TargetServices.DetectTargetFramework(module).Identifier == NetCoreAppIdentifier;
			var projectType = GetProjectType(module);
			var targetPacks = new HashSet<string>();
			if (isNetCoreApp)
			{
				targetPacks.Add("Microsoft.NETCore.App");
				if ((projectType & ProjectType.Desktop) != 0)
				{
					targetPacks.Add("Microsoft.WindowsDesktop.App");
				}
				if (projectType.HasFlag(ProjectType.Web))
				{
					targetPacks.Add("Microsoft.AspNetCore.App");
					targetPacks.Add("Microsoft.AspNetCore.All");
				}
			}
			foreach (var reference in module.AssemblyReferences)
			{
				if (IsSuppliedBySdk(reference.Name, projectType))
				{
					continue;
				}
				if (isNetCoreApp && project.AssemblyReferenceClassifier.IsSharedAssembly(reference, out string? runtimePack) && targetPacks.Contains(runtimePack))
				{
					continue;
				}
				yield return reference;
			}
		}

		/// <summary>
		/// Determines whether the SDK adds the named reference by itself, given what the project uses.
		/// Each set matches an _SDKImplicitReference group of the SDK and carries the same condition.
		/// </summary>
		static bool IsSuppliedBySdk(string referenceName, ProjectType projectType)
		{
			if (ImplicitReferences.Contains(referenceName))
			{
				return true;
			}
			if (projectType.HasFlag(ProjectType.Wpf) && WpfImplicitReferences.Contains(referenceName))
			{
				return true;
			}
			if (projectType.HasFlag(ProjectType.WinForms) && referenceName == WindowsFormsName)
			{
				return true;
			}
			// The interop assembly is implicit only where both stacks meet.
			return projectType.HasFlag(ProjectType.Desktop) && referenceName == WindowsFormsIntegrationName;
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

		static string GetSdkString(ProjectType projectType, TargetFramework targetFramework)
		{
			if (projectType.HasFlag(ProjectType.Web))
			{
				// A project names a single SDK, and Microsoft.NET.Sdk.Web is the wider one: it
				// imports Microsoft.NET.Sdk, which carries the desktop targets UseWPF and
				// UseWindowsForms need. The desktop SDK carries no web targets, so an assembly
				// that looks like both is exported as a web project.
				return "Microsoft.NET.Sdk.Web";
			}

			// Microsoft.NET.Sdk carries the Windows Desktop targets itself since .NET 5 and
			// warns (NETSDK1137) about projects that still name the separate SDK; only
			// .NET Core 3.x, where the desktop targets are not imported for a plain
			// framework moniker, still needs it.
			if ((projectType & ProjectType.Desktop) != 0
				&& targetFramework.Identifier == NetCoreAppIdentifier
				&& targetFramework.VersionNumber >= 300 && targetFramework.VersionNumber < 500)
			{
				return "Microsoft.NET.Sdk.WindowsDesktop";
			}

			return "Microsoft.NET.Sdk";
		}

		static ProjectType GetProjectType(MetadataFile module)
		{
			var projectType = ProjectType.Default;
			foreach (var referenceName in module.AssemblyReferences.Select(r => r.Name))
			{
				if (referenceName.StartsWith(AspNetCorePrefix, StringComparison.Ordinal))
				{
					projectType |= ProjectType.Web;
				}
				else if (referenceName == PresentationFrameworkName)
				{
					projectType |= ProjectType.Wpf;
				}
				else if (referenceName == WindowsFormsName)
				{
					projectType |= ProjectType.WinForms;
				}
			}

			return projectType;
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
