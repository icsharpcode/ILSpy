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
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// A <see cref="IProjectFileWriter"/> implementation that creates the projects in the default format.
	/// </summary>
	sealed class ProjectFileWriterDefault : IProjectFileWriter
	{
		/// <summary>
		/// Creates a new instance of the <see cref="ProjectFileWriterDefault"/> class.
		/// </summary>
		/// <returns>A new instance of the <see cref="ProjectFileWriterDefault"/> class.</returns>
		public static IProjectFileWriter Create() => new ProjectFileWriterDefault();

		/// <inheritdoc />
		public void Write(
			TextWriter target,
			IProjectInfoProvider project,
			IEnumerable<ProjectItemInfo> files,
			PEFile module)
		{
			const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			string platformName = TargetServices.GetPlatformName(module);
			var targetFramework = TargetServices.DetectTargetFramework(module);
			if (targetFramework.Identifier == ".NETFramework" && targetFramework.VersionNumber == 200)
				targetFramework = TargetServices.DetectTargetFrameworkNET20(module, project.AssemblyResolver, targetFramework);

			List<Guid> typeGuids = new List<Guid>();
			if (targetFramework.IsPortableClassLibrary)
				typeGuids.Add(ProjectTypeGuids.PortableLibrary);
			typeGuids.Add(ProjectTypeGuids.CSharpWindows);

			using (XmlTextWriter w = new XmlTextWriter(target))
			{
				w.Formatting = Formatting.Indented;
				w.WriteStartDocument();
				w.WriteStartElement("Project", ns);
				w.WriteAttributeString("ToolsVersion", "4.0");
				w.WriteAttributeString("DefaultTargets", "Build");

				w.WriteStartElement("PropertyGroup");
				w.WriteElementString("ProjectGuid", project.ProjectGuid.ToString("B").ToUpperInvariant());
				w.WriteElementString("ProjectTypeGuids", string.Join(";", typeGuids.Select(g => g.ToString("B").ToUpperInvariant())));

				w.WriteStartElement("Configuration");
				w.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
				w.WriteValue("Debug");
				w.WriteEndElement(); // </Configuration>

				w.WriteStartElement("Platform");
				w.WriteAttributeString("Condition", " '$(Platform)' == '' ");
				w.WriteValue(platformName);
				w.WriteEndElement(); // </Platform>

				if (module.Reader.PEHeaders.IsDll)
				{
					w.WriteElementString("OutputType", "Library");
				}
				else
				{
					switch (module.Reader.PEHeaders.PEHeader.Subsystem)
					{
						case Subsystem.WindowsGui:
							w.WriteElementString("OutputType", "WinExe");
							break;
						case Subsystem.WindowsCui:
							w.WriteElementString("OutputType", "Exe");
							break;
						default:
							w.WriteElementString("OutputType", "Library");
							break;
					}
				}

				w.WriteElementString("LangVersion", project.LanguageVersion.ToString().Replace("CSharp", "").Replace('_', '.'));

				w.WriteElementString("AssemblyName", module.Name);
				if (targetFramework.Identifier != null)
					w.WriteElementString("TargetFrameworkIdentifier", targetFramework.Identifier);
				if (targetFramework.VersionString != null)
					w.WriteElementString("TargetFrameworkVersion", targetFramework.VersionString);
				if (targetFramework.Profile != null)
					w.WriteElementString("TargetFrameworkProfile", targetFramework.Profile);
				w.WriteElementString("WarningLevel", "4");
				w.WriteElementString("AllowUnsafeBlocks", "True");

				if (project.StrongNameKeyFile != null)
				{
					w.WriteElementString("SignAssembly", "True");
					w.WriteElementString("AssemblyOriginatorKeyFile", Path.GetFileName(project.StrongNameKeyFile));
				}

				w.WriteEndElement(); // </PropertyGroup>

				w.WriteStartElement("PropertyGroup"); // platform-specific
				w.WriteAttributeString("Condition", " '$(Platform)' == '" + platformName + "' ");
				w.WriteElementString("PlatformTarget", platformName);
				if (targetFramework.VersionNumber > 400 && platformName == "AnyCPU" && (module.Reader.PEHeaders.CorHeader.Flags & CorFlags.Prefers32Bit) == 0)
				{
					w.WriteElementString("Prefer32Bit", "false");
				}
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
				foreach (var r in module.AssemblyReferences)
				{
					if (r.Name != "mscorlib")
					{
						w.WriteStartElement("Reference");
						w.WriteAttributeString("Include", r.Name);
						var asm = project.AssemblyResolver.Resolve(r);
						if (asm != null && !project.AssemblyReferenceClassifier.IsGacAssembly(r))
						{
							w.WriteElementString("HintPath", asm.FileName);
						}
						w.WriteEndElement();
					}
				}
				w.WriteEndElement(); // </ItemGroup> (References)

				foreach (IGrouping<string, ProjectItemInfo> gr in files.GroupBy(f => f.ItemType).OrderBy(g => g.Key))
				{
					w.WriteStartElement("ItemGroup");
					foreach (var item in gr.OrderBy(f => f.FileName, StringComparer.OrdinalIgnoreCase))
					{
						w.WriteStartElement(gr.Key);
						w.WriteAttributeString("Include", item.FileName);
						if (item.AdditionalProperties != null)
						{
							foreach (var (key, value) in item.AdditionalProperties)
								w.WriteAttributeString(key, value);
						}
						w.WriteEndElement();
					}
					w.WriteEndElement();
				}
				if (targetFramework.IsPortableClassLibrary)
				{
					w.WriteStartElement("Import");
					w.WriteAttributeString("Project", "$(MSBuildExtensionsPath32)\\Microsoft\\Portable\\$(TargetFrameworkVersion)\\Microsoft.Portable.CSharp.targets");
					w.WriteEndElement();
				}
				else
				{
					w.WriteStartElement("Import");
					w.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
					w.WriteEndElement();
				}

				w.WriteEndDocument();
			}
		}
	}
}
