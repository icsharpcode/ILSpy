// Copyright (c) 2018 Siegfried Pammer
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

using ICSharpCode.ILSpy.Util;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.ReadyToRun
{
	internal partial class ReadyToRunOptions : ObservableObject, ISettingsSection
	{
		private static readonly XNamespace ns = "http://www.ilspy.net/ready-to-run";

		internal static string intel = "Intel";
		internal static string gas = "AT & T";
		internal static string[] disassemblyFormats = [intel, gas];

		public string[] DisassemblyFormats {
			get {
				return disassemblyFormats;
			}
		}

		private bool isShowUnwindInfo;
		public bool IsShowUnwindInfo {
			get => isShowUnwindInfo;
			set => SetProperty(ref isShowUnwindInfo, value);
		}

		private bool isShowDebugInfo;
		public bool IsShowDebugInfo {
			get => isShowDebugInfo;
			set => SetProperty(ref isShowDebugInfo, value);
		}

		private bool isShowGCInfo;
		public bool IsShowGCInfo {
			get => isShowGCInfo;
			set => SetProperty(ref isShowGCInfo, value);
		}

		private string disassemblyFormat;
		public string DisassemblyFormat {
			get => disassemblyFormat;
			set => SetProperty(ref disassemblyFormat, value);
		}

		public XName SectionName { get; } = ns + "ReadyToRunOptions";

		public void LoadFromXml(XElement e)
		{
			XAttribute format = e.Attribute("DisassemblyFormat");
			DisassemblyFormat = format == null ? intel : (string)format;

			XAttribute unwind = e.Attribute("IsShowUnwindInfo");
			IsShowUnwindInfo = unwind != null && (bool)unwind;

			XAttribute debug = e.Attribute("IsShowDebugInfo");
			IsShowDebugInfo = debug == null || (bool)debug;

			XAttribute showGc = e.Attribute("IsShowGCInfo");
			IsShowGCInfo = showGc != null && (bool)showGc;
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			section.SetAttributeValue("DisassemblyFormat", disassemblyFormat);
			section.SetAttributeValue("IsShowUnwindInfo", isShowUnwindInfo);
			section.SetAttributeValue("IsShowDebugInfo", isShowDebugInfo);
			section.SetAttributeValue("IsShowGCInfo", isShowGCInfo);

			return section;
		}
	}
}
