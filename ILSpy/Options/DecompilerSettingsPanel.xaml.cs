// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for DecompilerSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = "Decompiler", Order = 0)]
	partial class DecompilerSettingsPanel : UserControl, IOptionPage
	{
		public DecompilerSettingsPanel()
		{
			InitializeComponent();
		}
		
		public void Load(ILSpySettings settings)
		{
			this.DataContext = currentDecompilerSettings ?? LoadDecompilerSettings(settings);
		}
		
		static DecompilerSettings currentDecompilerSettings;
		
		public static DecompilerSettings CurrentDecompilerSettings {
			get {
				return currentDecompilerSettings ?? (currentDecompilerSettings = LoadDecompilerSettings(ILSpySettings.Load()));
			}
		}
		
		public static DecompilerSettings LoadDecompilerSettings(ILSpySettings settings)
		{
			XElement e = settings["DecompilerSettings"];
			DecompilerSettings s = new DecompilerSettings();
			s.ShowDebugInfo = (bool?)e.Attribute("showDebugInfo") ?? s.ShowDebugInfo;
			s.ShowXmlDocumentation = (bool?)e.Attribute("xmlDoc") ?? s.ShowXmlDocumentation;
			s.FoldBraces = (bool?)e.Attribute("foldBraces") ?? s.FoldBraces;
			s.ExpandMemberDefinitions = (bool?)e.Attribute("expandMemberDefinitions") ?? s.ExpandMemberDefinitions;
			s.RemoveDeadCode = (bool?)e.Attribute("removeDeadCode") ?? s.RemoveDeadCode;
			s.UsingDeclarations = (bool?)e.Attribute("usingDeclarations") ?? s.UsingDeclarations;
			s.AlwaysUseBraces = (bool?)e.Attribute("alwaysUseBraces") ?? s.AlwaysUseBraces;
			return s;
		}
		
		public void Save(XElement root)
		{
			DecompilerSettings s = (DecompilerSettings)this.DataContext;
			XElement section = new XElement("DecompilerSettings");
			section.SetAttributeValue("useDebugSymbols", s.UseDebugSymbols);
			section.SetAttributeValue("showDebugInfo", s.ShowDebugInfo);
			section.SetAttributeValue("xmlDoc", s.ShowXmlDocumentation);
			section.SetAttributeValue("foldBraces", s.FoldBraces);
			section.SetAttributeValue("expandMemberDefinitions", s.ExpandMemberDefinitions);
			section.SetAttributeValue("removeDeadCode", s.RemoveDeadCode);
			section.SetAttributeValue("usingDeclarations", s.UsingDeclarations);
			section.SetAttributeValue("alwaysUseBraces", s.AlwaysUseBraces);

			XElement existingElement = root.Element("DecompilerSettings");
			if (existingElement != null)
				existingElement.ReplaceWith(section);
			else
				root.Add(section);
			
			currentDecompilerSettings = s; // update cached settings
		}
	}

	public class DecompilerSettings : INotifyPropertyChanged
	{
		bool showXmlDocumentation = true;

		/// <summary>
		/// Gets/Sets whether to include XML documentation comments in the decompiled code.
		/// </summary>
		public bool ShowXmlDocumentation {
			get { return showXmlDocumentation; }
			set {
				if (showXmlDocumentation != value) {
					showXmlDocumentation = value;
					OnPropertyChanged();
				}
			}
		}

		bool foldBraces = false;

		public bool FoldBraces {
			get { return foldBraces; }
			set {
				if (foldBraces != value) {
					foldBraces = value;
					OnPropertyChanged();
				}
			}
		}

		bool expandMemberDefinitions = false;

		public bool ExpandMemberDefinitions {
			get { return expandMemberDefinitions; }
			set {
				if (expandMemberDefinitions != value) {
					expandMemberDefinitions = value;
					OnPropertyChanged();
				}
			}
		}

		bool decompileMemberBodies = true;

		/// <summary>
		/// Gets/Sets whether member bodies should be decompiled.
		/// </summary>
		public bool DecompileMemberBodies {
			get { return decompileMemberBodies; }
			set {
				if (decompileMemberBodies != value) {
					decompileMemberBodies = value;
					OnPropertyChanged();
				}
			}
		}

		bool useDebugSymbols = true;

		/// <summary>
		/// Gets/Sets whether to use variable names from debug symbols, if available.
		/// </summary>
		public bool UseDebugSymbols {
			get { return useDebugSymbols; }
			set {
				if (useDebugSymbols != value) {
					useDebugSymbols = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingDeclarations = true;

		public bool UsingDeclarations {
			get { return usingDeclarations; }
			set {
				if (usingDeclarations != value) {
					usingDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool showDebugInfo;

		public bool ShowDebugInfo {
			get { return showDebugInfo; }
			set {
				if (showDebugInfo != value) {
					showDebugInfo = value;
					OnPropertyChanged();
				}
			}
		}

		bool removeDeadCode = false;

		public bool RemoveDeadCode {
			get { return removeDeadCode; }
			set {
				if (removeDeadCode != value) {
					removeDeadCode = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysUseBraces = true;

		/// <summary>
		/// Gets/Sets whether to use braces for single-statement-blocks. 
		/// </summary>
		public bool AlwaysUseBraces {
			get { return alwaysUseBraces; }
			set {
				if (alwaysUseBraces != value) {
					alwaysUseBraces = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}