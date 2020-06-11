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
			if (module is null) {
				throw new ArgumentNullException(nameof(module));
			}

			int versionNumber;
			switch (module.GetRuntime()) {
				case TargetRuntime.Net_1_0:
					versionNumber = 100;
					break;

				case TargetRuntime.Net_1_1:
					versionNumber = 110;
					break;

				case TargetRuntime.Net_2_0:
					versionNumber = 200;
					// TODO: Detect when .NET 3.0/3.5 is required
					break;

				default:
					versionNumber = 400;
					break;
			}

			string targetFrameworkIdentifier = null;
			string targetFrameworkProfile = null;

			string targetFramework = module.DetectTargetFrameworkId();
			if (!string.IsNullOrEmpty(targetFramework)) {
				string[] frameworkParts = targetFramework.Split(',');
				targetFrameworkIdentifier = frameworkParts.FirstOrDefault(a => !a.StartsWith(VersionToken, StringComparison.OrdinalIgnoreCase) && !a.StartsWith(ProfileToken, StringComparison.OrdinalIgnoreCase));
				string frameworkVersion = frameworkParts.FirstOrDefault(a => a.StartsWith(VersionToken, StringComparison.OrdinalIgnoreCase));

				if (frameworkVersion != null) {
					versionNumber = int.Parse(frameworkVersion.Substring(VersionToken.Length + 1).Replace(".", ""));
					if (versionNumber < 100) versionNumber *= 10;
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
			if (module is null) {
				throw new ArgumentNullException(nameof(module));
			}

			var headers = module.Reader.PEHeaders;
			var architecture = headers.CoffHeader.Machine;
			var characteristics = headers.CoffHeader.Characteristics;
			var corflags = headers.CorHeader.Flags;

			switch (architecture) {
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
	}
}
