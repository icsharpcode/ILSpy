// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using AvalonDock.Layout.Serialization;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockLayoutSettings
	{
		/// <remarks>NOTE: do NOT remove any of the (empty) sections, the deserializer is not very resilient and expects this exact order of elements!</remarks>
		private const string DefaultLayout = @"
<LayoutRoot>
	<RootPanel Orientation=""Horizontal"">
		<LayoutAnchorablePaneGroup Orientation=""Horizontal"" DockWidth=""300"">
			<LayoutAnchorablePane>
				<LayoutAnchorable ContentId=""assemblyListPane"" />
			</LayoutAnchorablePane>
		</LayoutAnchorablePaneGroup>
		<LayoutPanel Orientation=""Vertical"">
			<LayoutAnchorablePaneGroup Orientation=""Horizontal"" DockHeight=""225"">
				<LayoutAnchorablePane Id=""3ce12a2b-e6be-47cd-9787-fbf74bb9f157"" />
			</LayoutAnchorablePaneGroup>
			<LayoutDocumentPane />
			<LayoutAnchorablePaneGroup Orientation=""Horizontal"" DockHeight=""225"">
				<LayoutAnchorablePane Id=""8858bc50-019a-480e-b402-30a890e7c68f"" />
			</LayoutAnchorablePaneGroup>
		</LayoutPanel>
	</RootPanel>
	<TopSide />
	<RightSide />
	<LeftSide />
	<BottomSide />
	<FloatingWindows />
	<Hidden>
		<LayoutAnchorable ContentId=""debugStepsPane"" PreviousContainerId=""3ce12a2b-e6be-47cd-9787-fbf74bb9f157"" PreviousContainerIndex=""0"" />
		<LayoutAnchorable ContentId=""searchPane"" PreviousContainerId=""3ce12a2b-e6be-47cd-9787-fbf74bb9f157"" PreviousContainerIndex=""0"" />
		<LayoutAnchorable ContentId=""analyzerPane"" PreviousContainerId=""8858bc50-019a-480e-b402-30a890e7c68f"" PreviousContainerIndex=""0"" />
	</Hidden>
</LayoutRoot>
";

		private string rawSettings;

		public bool Valid => rawSettings != null;

		public void Reset()
		{
			this.rawSettings = DefaultLayout;
		}

		public DockLayoutSettings(XElement element)
		{
			if ((element != null) && element.HasElements)
			{
				rawSettings = element.Elements().FirstOrDefault()?.ToString();
			}
		}

		public XElement SaveAsXml()
		{
			try
			{
				return XElement.Parse(rawSettings);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void Deserialize(XmlLayoutSerializer serializer)
		{
			if (!Valid)
				rawSettings = DefaultLayout;
			try
			{
				Deserialize(rawSettings);
			}
			catch (Exception)
			{
				Deserialize(DefaultLayout);
			}

			void Deserialize(string settings)
			{
				using (StringReader reader = new StringReader(settings))
				{
					serializer.Deserialize(reader);
				}
			}
		}

		public void Serialize(XmlLayoutSerializer serializer)
		{
			using (StringWriter fs = new StringWriter())
			{
				serializer.Serialize(fs);
				rawSettings = fs.ToString();
			}
		}
	}
}
