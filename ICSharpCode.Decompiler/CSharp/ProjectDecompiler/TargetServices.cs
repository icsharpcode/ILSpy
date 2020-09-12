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
using System.Linq;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// Helper services for determining the target framework and platform of a module.
	/// </summary>
	static class TargetServices
	{
		const string VersionToken = "Version=";
		const string ProfileToken = "Profile=";

		/// <summary>
		/// Gets the <see cref="TargetFramework"/> for the specified <paramref name="module"/>.
		/// </summary>
		/// <param name="module">The module to get the target framework description for. Cannot be null.</param>
		/// <returns>A new instance of the <see cref="TargetFramework"/> class that describes the specified <paramref name="module"/>.
		/// </returns>
		public static TargetFramework DetectTargetFramework(PEFile module)
		{
			if (module is null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			int versionNumber;
			switch (module.GetRuntime())
			{
				case TargetRuntime.Net_1_0:
					versionNumber = 100;
					break;

				case TargetRuntime.Net_1_1:
					versionNumber = 110;
					break;

				case TargetRuntime.Net_2_0:
					versionNumber = 200;
					break;

				default:
					versionNumber = 400;
					break;
			}

			string targetFrameworkIdentifier = null;
			string targetFrameworkProfile = null;

			string targetFramework = module.DetectTargetFrameworkId();
			if (!string.IsNullOrEmpty(targetFramework))
			{
				string[] frameworkParts = targetFramework.Split(',');
				targetFrameworkIdentifier = frameworkParts.FirstOrDefault(a => !a.StartsWith(VersionToken, StringComparison.OrdinalIgnoreCase) && !a.StartsWith(ProfileToken, StringComparison.OrdinalIgnoreCase));
				string frameworkVersion = frameworkParts.FirstOrDefault(a => a.StartsWith(VersionToken, StringComparison.OrdinalIgnoreCase));

				if (frameworkVersion != null)
				{
					versionNumber = int.Parse(frameworkVersion.Substring(VersionToken.Length + 1).Replace(".", ""));
					if (versionNumber < 100)
						versionNumber *= 10;
				}

				string frameworkProfile = frameworkParts.FirstOrDefault(a => a.StartsWith(ProfileToken, StringComparison.OrdinalIgnoreCase));
				if (frameworkProfile != null)
					targetFrameworkProfile = frameworkProfile.Substring(ProfileToken.Length);
			}

			return new TargetFramework(targetFrameworkIdentifier, versionNumber, targetFrameworkProfile);
		}

		/// <summary>
		/// Gets the string representation (name) of the target platform of the specified <paramref name="module"/>.
		/// </summary>
		/// <param name="module">The module to get the target framework description for. Cannot be null.</param>
		/// <returns>The platform name, e.g. "AnyCPU" or "x86".</returns>
		public static string GetPlatformName(PEFile module)
		{
			if (module is null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			var headers = module.Reader.PEHeaders;
			var architecture = headers.CoffHeader.Machine;
			var characteristics = headers.CoffHeader.Characteristics;
			var corflags = headers.CorHeader.Flags;

			switch (architecture)
			{
				case Machine.I386:
					if ((corflags & CorFlags.Prefers32Bit) != 0)
						return "AnyCPU";

					if ((corflags & CorFlags.Requires32Bit) != 0)
						return "x86";

					// According to ECMA-335, II.25.3.3.1 CorFlags.Requires32Bit and Characteristics.Bit32Machine must be in sync
					// for assemblies containing managed code. However, this is not true for C++/CLI assemblies.
					if ((corflags & CorFlags.ILOnly) == 0 && (characteristics & Characteristics.Bit32Machine) != 0)
						return "x86";
					return "AnyCPU";

				case Machine.Amd64:
					return "x64";

				case Machine.IA64:
					return "Itanium";

				default:
					return architecture.ToString();
			}
		}

		static HashSet<string> dotNet30Assemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
			"ComSvcConfig, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"infocard, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"Microsoft.Transactions.Bridge, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Transactions.Bridge.Dtc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"PresentationBuildTasks, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationCFFRasterizer, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationFramework.Aero, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationFramework.Classic, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationFramework.Luna, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationFramework.Royale, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"PresentationUI, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"ReachFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"ServiceModelReg, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"SMSvcHost, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"System.IdentityModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.IdentityModel.Selectors, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.IO.Log, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"System.Printing, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.ServiceModel.Install, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Speech, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Workflow.Activities, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Workflow.ComponentModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Workflow.Runtime, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"UIAutomationClient, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"UIAutomationClientsideProviders, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"UIAutomationProvider, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"UIAutomationTypes, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"WindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"WsatConfig, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
		};

		static HashSet<string> dotNet35Assemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
			"AddInProcess, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"AddInProcess32, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"AddInUtil, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"DataSvcUtil, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"EdmGen, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"Microsoft.Build.Conversion.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Build.Engine, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Build.Framework, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Build.Tasks.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Build.Utilities.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.Data.Entity.Build.Tasks, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Microsoft.VisualC.STLCLR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"MSBuild, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"System.AddIn, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.AddIn.Contract, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"System.ComponentModel.DataAnnotations, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.DataSetExtensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Entity.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Services, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Services.Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.Services.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.DirectoryServices.AccountManagement, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Management.Instrumentation, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Net, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			"System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.DynamicData.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Web.Entity.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Windows.Presentation, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.WorkflowServices, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"System.Xml.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
		};

		/// <summary>
		/// Gets exact <see cref="TargetFramework"/> if <see cref="PEFile.GetRuntime"/> is <see cref="TargetRuntime.Net_2_0"/>
		/// </summary>
		public static TargetFramework DetectTargetFrameworkNET20(PEFile module, IAssemblyResolver assemblyResolver, TargetFramework targetFramework)
		{
			var resolvedAssemblies = new HashSet<string>();
			int version = 200;
			GetFrameworkVersionNET20(module, assemblyResolver, resolvedAssemblies, ref version);
			return new TargetFramework(targetFramework.Identifier, version, targetFramework.Profile);
		}

		static void GetFrameworkVersionNET20(PEFile module, IAssemblyResolver assemblyResolver, HashSet<string> resolvedAssemblies, ref int version)
		{
			foreach (var r in module.Metadata.AssemblyReferences)
			{
				var reference = new AssemblyReference(module, r);
				if (!resolvedAssemblies.Add(reference.FullName))
					continue;

				if (dotNet30Assemblies.Contains(reference.FullName))
				{
					version = 300;
					continue;
				}
				else if (dotNet35Assemblies.Contains(reference.FullName))
				{
					version = 350;
					break;
				}

				PEFile resolvedReference;
				try
				{
					resolvedReference = assemblyResolver.Resolve(reference);
				}
				catch (AssemblyResolutionException)
				{
					resolvedReference = null;
				}
				if (resolvedReference == null)
					continue;
				resolvedAssemblies.Add(resolvedReference.FullName);
				GetFrameworkVersionNET20(resolvedReference, assemblyResolver, resolvedAssemblies, ref version);
				if (version == 350)
					return;
			}
		}
	}
}
