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
using System.Text;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// A class describing the target framework of a module.
	/// </summary>
	sealed class TargetFramework
	{
		const string DotNetPortableIdentifier = ".NETPortable";

		/// <summary>
		/// Initializes a new instance of the <see cref="TargetFramework"/> class.
		/// </summary>
		/// <param name="identifier">The framework identifier string. Can be null.</param>
		/// <param name="version">The framework version string. Must be greater than 100 (where 100 corresponds to v1.0).</param>
		/// <param name="profile">The framework profile. Can be null.</param>
		public TargetFramework(string identifier, int version, string profile)
		{
			if (version < 100) {
				throw new ArgumentException("The version number must be greater than or equal to 100", nameof(version));
			}

			Identifier = identifier;
			VersionNumber = version;
			VersionString = "v" + GetVersionString(version, withDots: true);
			Moniker = GetTargetFrameworkMoniker(Identifier, version);
			Profile = profile;
			IsPortableClassLibrary = identifier == DotNetPortableIdentifier;
		}

		/// <summary>
		/// Gets the target framework identifier. Can be null if not defined.
		/// </summary>
		public string Identifier { get; }
		
		/// <summary>
		/// Gets the target framework moniker. Can be null if not supported.
		/// </summary>
		public string Moniker { get; }

		/// <summary>
		/// Gets the target framework version, e.g. "v4.5".
		/// </summary>
		public string VersionString { get; }

		/// <summary>
		/// Gets the target framework version as integer (multiplied by 100), e.g. 450.
		/// </summary>
		public int VersionNumber { get; }

		/// <summary>
		/// Gets the target framework profile. Can be null if not set or not available.
		/// </summary>
		public string Profile { get; }

		/// <summary>
		/// Gets a value indicating whether the target is a portable class library (PCL).
		/// </summary>
		public bool IsPortableClassLibrary { get; }

		static string GetTargetFrameworkMoniker(string frameworkIdentifier, int version)
		{
			// Reference: https://docs.microsoft.com/en-us/dotnet/standard/frameworks
			switch (frameworkIdentifier) {
				case null:
				case ".NETFramework":
					return "net" + GetVersionString(version, withDots: false);

				case ".NETCoreApp":
					return "netcoreapp" + GetVersionString(version, withDots: true);

				case ".NETStandard":
					return "netstandard" + GetVersionString(version, withDots: true);

				case "Silverlight":
					return "sl" + version / 100;

				case ".NETCore":
					return "netcore" + GetVersionString(version, withDots: false);

				case "WindowsPhone":
					return "wp" + GetVersionString(version, withDots: false, omitMinorWhenZero: true);

				case ".NETMicroFramework":
					return "netmf";

				default:
					return null;
			}
		}

		static string GetVersionString(int version, bool withDots, bool omitMinorWhenZero = false)
		{
			int major = version / 100;
			int minor = version % 100 / 10;
			int patch = version % 10;

			if (omitMinorWhenZero && minor == 0 && patch == 0) {
				return major.ToString();
			}

			var versionBuilder = new StringBuilder(8);
			versionBuilder.Append(major);

			if (withDots) {
				versionBuilder.Append('.');
			}

			versionBuilder.Append(minor);

			if (patch != 0) {
				if (withDots) {
					versionBuilder.Append('.');
				}

				versionBuilder.Append(patch);
			}

			return versionBuilder.ToString();
		}
	}
}
