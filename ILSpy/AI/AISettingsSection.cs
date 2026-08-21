// Copyright (c) 2026 Dr. Masroor Ehsan
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

using System.ComponentModel;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Desktop persistence adapter: registers the portable <see cref="AISettingsModel"/> with the
	/// ILSpyX settings service under the same <c>AISettings</c> XML section name and schema the
	/// pre-extraction <c>ICSharpCode.ILSpyX.Settings.AISettings</c> section used. All state,
	/// defaults, validation, and XML translation live in the model; this wrapper only forwards
	/// load/save and property-change notification. API key material is never persisted.
	/// </summary>
	public sealed class AISettingsSection : ISettingsSection
	{
		public AISettingsSection()
		{
			Model = new AISettingsModel();
			Model.PropertyChanged += Model_PropertyChanged;
		}

		/// <summary>The live portable settings state wrapped by this section.</summary>
		public AISettingsModel Model { get; }

		public XName SectionName => AISettingsModel.SectionElementName;

		public event PropertyChangedEventHandler? PropertyChanged;

		public void LoadFromXml(XElement section) => Model.LoadFromXml(section);

		public XElement SaveToXml() => Model.SaveToXml();

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Re-raise with the section as sender: SettingsServiceBase subscribers (and the
			// desktop MessageBus fan-out) identify sections, not the portable model they wrap.
			PropertyChanged?.Invoke(this, e);
		}
	}
}
