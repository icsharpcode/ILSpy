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

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// A <see cref="IProjectFileWriter"/> implementation that creates the projects in the SDK style format.
	/// </summary>
	sealed class ProjectFileWriterSdkStyle : IProjectFileWriter
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
		/// Creates a new instance of the <see cref="ProjectFileWriterSdkStyle"/> class.
		/// </summary>
		/// <returns>A new instance of the <see cref="ProjectFileWriterSdkStyle"/> class.</returns>
		public static IProjectFileWriter Create() => new ProjectFileWriterSdkStyle();

		/// <inheritdoc />
		public void Write(
			TextWriter target,
			IProjectInfoProvider project,
			IEnumerable<(string itemType, string fileName)> files,
			PEFile module)
		{
			using (XmlTextWriter xmlWriter = new XmlTextWriter(target))
			{
				xmlWriter.Formatting = Formatting.Indented;
				Write(xmlWriter, project, module);
			}
		}

		static void Write(XmlTextWriter xml, IProjectInfoProvider project, PEFile module)
		{
			xml.WriteStartElement("Project");

			var projectType = GetProjectType(module);
			xml.WriteAttributeString("Sdk", GetSdkString(projectType));

			PlaceIntoTag("PropertyGroup", xml, () => WriteAssemblyInfo(xml, module, projectType));
			PlaceIntoTag("PropertyGroup", xml, () => WriteProjectInfo(xml, project));
			PlaceIntoTag("ItemGroup", xml, () => WriteReferences(xml, module, project));

			xml.WriteEndElement();
		}

		static void PlaceIntoTag(string tagName, XmlTextWriter xml, Action content)
		{
			xml.WriteStartElement(tagName);
			try
			{
				content();
			}
			finally
			{
				xml.WriteEndElement();
			}
		}

		static void WriteAssemblyInfo(XmlTextWriter xml, PEFile module, ProjectType projectType)
		{
			xml.WriteElementString("AssemblyName", module.Name);

			// Since we create AssemblyInfo.cs manually, we need to disable the auto-generation
			xml.WriteElementString("GenerateAssemblyInfo", FalseString);

			// 'Library' is default, so only need to specify output type for executables
			if (!module.Reader.PEHeaders.IsDll)
			{
				WriteOutputType(xml, module.Reader.PEHeaders.PEHeader.Subsystem);
			}

			WriteDesktopExtensions(xml, projectType);

			string platformName = TargetServices.GetPlatformName(module);
			var targetFramework = TargetServices.DetectTargetFramework(module);

			if (targetFramework.Moniker == null)
			{
				throw new NotSupportedException($"Cannot decompile this assembly to a SDK style project. Use default project format instead.");
			}

			xml.WriteElementString("TargetFramework", targetFramework.Moniker);

			// 'AnyCPU' is default, so only need to specify platform if it differs
			if (platformName != AnyCpuString)
			{
				xml.WriteElementString("PlatformTarget", platformName);
			}

			if (platformName == AnyCpuString && (module.Reader.PEHeaders.CorHeader.Flags & CorFlags.Prefers32Bit) != 0)
			{
				xml.WriteElementString("Prefer32Bit", TrueString);
			}
		}

		static void WriteOutputType(XmlTextWriter xml, Subsystem moduleSubsystem)
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

			if (project.StrongNameKeyFile != null)
			{
				xml.WriteElementString("SignAssembly", TrueString);
				xml.WriteElementString("AssemblyOriginatorKeyFile", Path.GetFileName(project.StrongNameKeyFile));
			}
		}

		static void WriteReferences(XmlTextWriter xml, PEFile module, IProjectInfoProvider project)
		{
			foreach (var reference in module.AssemblyReferences.Where(r => !ImplicitReferences.Contains(r.Name)))
			{
				if (project.AssemblyResolver.IsSharedAssembly(reference))
				{
					continue;
				}

				xml.WriteStartElement("Reference");
				xml.WriteAttributeString("Include", reference.Name);

				var asembly = project.AssemblyResolver.Resolve(reference);
				if (asembly != null && !project.AssemblyResolver.IsGacAssembly(reference))
				{
					xml.WriteElementString("HintPath", FileUtility.GetRelativePath(project.TargetDirectory, asembly.FileName));
				}

				xml.WriteEndElement();
			}
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

		static ProjectType GetProjectType(PEFile module)
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
	}
}
