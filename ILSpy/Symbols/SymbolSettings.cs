// Copyright (c) 2026 Piero Viano
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

using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Symbols
{
	/// <summary>
	/// Symbol-server settings, persisted under the <c>&lt;SymbolSettings/&gt;</c> XML section:
	/// where PDBs are looked up (client side) and whether ILSpy hosts a local symbol server.
	/// </summary>
	public sealed partial class SymbolSettings : ObservableObject, ISettingsSection
	{
		public const int DefaultPort = 33417;

		/// <summary>Symbol path in <c>_NT_SYMBOL_PATH</c> syntax.</summary>
		[ObservableProperty]
		string symbolPath = ILSpyX.Symbols.SymbolPath.DefaultSymbolPath;

		/// <summary>Search the <c>_NT_SYMBOL_PATH</c> environment variable before <see cref="SymbolPath"/>.</summary>
		[ObservableProperty]
		bool useEnvironmentSymbolPath = true;

		/// <summary>Download cache; empty means the default under the local application data folder.</summary>
		[ObservableProperty]
		string cacheDirectory = string.Empty;

		/// <summary>Look up missing PDBs on the symbol path while opened assemblies load.</summary>
		[ObservableProperty]
		bool autoDownload;

		/// <summary>Start the local symbol server when ILSpy starts.</summary>
		[ObservableProperty]
		bool startSymbolServer;

		/// <summary>Loopback port of the local symbol server; 0 picks a free port.</summary>
		[ObservableProperty]
		int symbolServerPort = DefaultPort;

		public XName SectionName => "SymbolSettings";

		public void LoadFromXml(XElement e)
		{
			SymbolPath = (string?)e.Attribute(nameof(SymbolPath)) ?? ILSpyX.Symbols.SymbolPath.DefaultSymbolPath;
			UseEnvironmentSymbolPath = (bool?)e.Attribute(nameof(UseEnvironmentSymbolPath)) ?? true;
			CacheDirectory = (string?)e.Attribute(nameof(CacheDirectory)) ?? string.Empty;
			AutoDownload = (bool?)e.Attribute(nameof(AutoDownload)) ?? false;
			StartSymbolServer = (bool?)e.Attribute(nameof(StartSymbolServer)) ?? false;
			int port = (int?)e.Attribute(nameof(SymbolServerPort)) ?? DefaultPort;
			SymbolServerPort = port is >= 0 and <= 65535 ? port : DefaultPort;
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			section.SetAttributeValue(nameof(SymbolPath), SymbolPath);
			section.SetAttributeValue(nameof(UseEnvironmentSymbolPath), UseEnvironmentSymbolPath);
			section.SetAttributeValue(nameof(CacheDirectory), CacheDirectory);
			section.SetAttributeValue(nameof(AutoDownload), AutoDownload);
			section.SetAttributeValue(nameof(StartSymbolServer), StartSymbolServer);
			section.SetAttributeValue(nameof(SymbolServerPort), SymbolServerPort);
			return section;
		}
	}
}
