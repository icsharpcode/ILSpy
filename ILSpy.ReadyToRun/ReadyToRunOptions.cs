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

using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.ReadyToRun
{
	internal class ReadyToRunOptions
	{
		private static readonly XNamespace ns = "http://www.ilspy.net/ready-to-run";

		internal static string intel = "Intel";
		internal static string gas = "AT & T";
		internal static string[] disassemblyFormats = new string[] { intel, gas };

		public static string GetDisassemblyFormat(ILSpySettings settings)
		{
			if (settings == null)
			{
				settings = ILSpySettings.Load();
			}
			XElement e = settings[ns + "ReadyToRunOptions"];
			XAttribute a = e.Attribute("DisassemblyFormat");
			if (a == null)
			{
				return ReadyToRunOptions.intel;
			}
			else
			{
				return (string)a;
			}
		}

		public static bool GetIsShowUnwindInfo(ILSpySettings settings)
		{
			if (settings == null)
			{
				settings = ILSpySettings.Load();
			}
			XElement e = settings[ns + "ReadyToRunOptions"];
			XAttribute a = e.Attribute("IsShowUnwindInfo");

			if (a == null)
			{
				return false;
			}
			else
			{
				return (bool)a;
			}
		}

		public static bool GetIsShowDebugInfo(ILSpySettings settings)
		{
			if (settings == null)
			{
				settings = ILSpySettings.Load();
			}
			XElement e = settings[ns + "ReadyToRunOptions"];
			XAttribute a = e.Attribute("IsShowDebugInfo");

			if (a == null)
			{
				return true;
			}
			else
			{
				return (bool)a;
			}
		}

		public static void SetDisassemblyOptions(XElement root, string disassemblyFormat, bool isShowUnwindInfo, bool isShowDebugInfo)
		{
			XElement section = new XElement(ns + "ReadyToRunOptions");
			section.SetAttributeValue("DisassemblyFormat", disassemblyFormat);
			section.SetAttributeValue("IsShowUnwindInfo", isShowUnwindInfo);
			section.SetAttributeValue("IsShowDebugInfo", isShowDebugInfo);
			XElement existingElement = root.Element(ns + "ReadyToRunOptions");
			if (existingElement != null)
			{
				existingElement.ReplaceWith(section);
			}
			else
			{
				root.Add(section);
			}
		}
	}
}
