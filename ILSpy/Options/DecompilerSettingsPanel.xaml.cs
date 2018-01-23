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

using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ICSharpCode.Decompiler;
using WinForms = System.Windows.Forms;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for DecompilerSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = "Decompiler", Order = 0)]
	partial class DecompilerSettingsPanel : UserControl, IOptionPage
	{
#if DEBUG
		public const bool IsDebug = true;
#else
		public const bool IsDebug = false;
#endif

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
			s.AnonymousMethods = (bool?)e.Attribute("anonymousMethods") ?? s.AnonymousMethods;
			s.AnonymousTypes = (bool?)e.Attribute("anonymousTypes") ?? s.AnonymousTypes;
			s.YieldReturn = (bool?)e.Attribute("yieldReturn") ?? s.YieldReturn;
			s.AsyncAwait = (bool?)e.Attribute("asyncAwait") ?? s.AsyncAwait;
			s.AutomaticProperties = (bool?) e.Attribute("automaticProperties") ?? s.AutomaticProperties;
			s.QueryExpressions = (bool?)e.Attribute("queryExpressions") ?? s.QueryExpressions;
			s.ExpressionTrees = (bool?)e.Attribute("expressionTrees") ?? s.ExpressionTrees;
			s.UseDebugSymbols = (bool?)e.Attribute("useDebugSymbols") ?? s.UseDebugSymbols;
			s.ShowDebugInfo = (bool?)e.Attribute("showDebugInfo") ?? s.ShowDebugInfo;
			s.ShowXmlDocumentation = (bool?)e.Attribute("xmlDoc") ?? s.ShowXmlDocumentation;
			s.FoldBraces = (bool?)e.Attribute("foldBraces") ?? s.FoldBraces;
			s.RemoveDeadCode = (bool?)e.Attribute("removeDeadCode") ?? s.RemoveDeadCode;
			s.UsingDeclarations = (bool?)e.Attribute("usingDeclarations") ?? s.UsingDeclarations;
			s.FullyQualifyAmbiguousTypeNames = (bool?)e.Attribute("fullyQualifyAmbiguousTypeNames") ?? s.FullyQualifyAmbiguousTypeNames;
			s.AlwaysUseBraces = (bool?)e.Attribute("alwaysUseBraces") ?? s.AlwaysUseBraces;
			return s;
		}
		
		public void Save(XElement root)
		{
			DecompilerSettings s = (DecompilerSettings)this.DataContext;
			XElement section = new XElement("DecompilerSettings");
			section.SetAttributeValue("anonymousMethods", s.AnonymousMethods);
			section.SetAttributeValue("anonymousTypes", s.AnonymousTypes);
			section.SetAttributeValue("yieldReturn", s.YieldReturn);
			section.SetAttributeValue("asyncAwait", s.AsyncAwait);
			section.SetAttributeValue("automaticProperties", s.AutomaticProperties);
			section.SetAttributeValue("queryExpressions", s.QueryExpressions);
			section.SetAttributeValue("expressionTrees", s.ExpressionTrees);
			section.SetAttributeValue("useDebugSymbols", s.UseDebugSymbols);
			section.SetAttributeValue("showDebugInfo", s.ShowDebugInfo);
			section.SetAttributeValue("xmlDoc", s.ShowXmlDocumentation);
			section.SetAttributeValue("foldBraces", s.FoldBraces);
			section.SetAttributeValue("removeDeadCode", s.RemoveDeadCode);
			section.SetAttributeValue("usingDeclarations", s.UsingDeclarations);
			section.SetAttributeValue("fullyQualifyAmbiguousTypeNames", s.FullyQualifyAmbiguousTypeNames);
			section.SetAttributeValue("alwaysUseBraces", s.AlwaysUseBraces);

			XElement existingElement = root.Element("DecompilerSettings");
			if (existingElement != null)
				existingElement.ReplaceWith(section);
			else
				root.Add(section);
			
			currentDecompilerSettings = s; // update cached settings
		}

		void AdvancedOptions_Click(object sender, RoutedEventArgs e)
		{
#if DEBUG
			// I know this is crazy, but WindowsFormsHost is too buggy in this scenario...
			using (var wnd = new PropertyGridHost(((DecompilerSettings)DataContext).Clone())) {
				if (wnd.ShowDialog() == WinForms.DialogResult.OK) {
					this.DataContext = wnd.Settings;
				}
			}
#endif
		}

#if DEBUG
		class PropertyGridHost : WinForms.Form
		{
			private WinForms.PropertyGrid propertyGrid;
			private WinForms.Button cancelButton;
			private WinForms.Button okButton;

			public DecompilerSettings Settings { get; }

			public PropertyGridHost(DecompilerSettings settings)
			{
				InitializeComponent();
				this.propertyGrid.SelectedObject = Settings = settings;
			}

			private void InitializeComponent()
			{
				this.propertyGrid = new System.Windows.Forms.PropertyGrid();
				this.cancelButton = new System.Windows.Forms.Button();
				this.okButton = new System.Windows.Forms.Button();
				this.SuspendLayout();
				// 
				// propertyGrid
				// 
				this.propertyGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
				| System.Windows.Forms.AnchorStyles.Left
				| System.Windows.Forms.AnchorStyles.Right;
				this.propertyGrid.Location = new System.Drawing.Point(0, 0);
				this.propertyGrid.Name = "propertyGrid";
				this.propertyGrid.Size = new System.Drawing.Size(468, 369);
				this.propertyGrid.TabIndex = 0;
				// 
				// cancelButton
				// 
				this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
				this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.cancelButton.Location = new System.Drawing.Point(365, 375);
				this.cancelButton.Name = "cancelButton";
				this.cancelButton.Size = new System.Drawing.Size(91, 23);
				this.cancelButton.TabIndex = 1;
				this.cancelButton.Text = "Cancel";
				this.cancelButton.UseVisualStyleBackColor = true;
				this.cancelButton.Click += CancelButton_Click;
				// 
				// okButton
				// 
				this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
				this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.okButton.Location = new System.Drawing.Point(267, 375);
				this.okButton.Name = "okButton";
				this.okButton.Size = new System.Drawing.Size(92, 23);
				this.okButton.TabIndex = 2;
				this.okButton.Text = "OK";
				this.okButton.UseVisualStyleBackColor = true;
				this.okButton.Click += OkButton_Click;
				// 
				// Form1
				// 
				this.AcceptButton = this.okButton;
				this.CancelButton = this.cancelButton;
				this.ClientSize = new System.Drawing.Size(468, 410);
				this.Controls.Add(this.okButton);
				this.Controls.Add(this.cancelButton);
				this.Controls.Add(this.propertyGrid);
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "Form1";
				this.Text = "Advanced Decompiler Options";
				this.ShowIcon = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				this.ResumeLayout(false);

			}

			private void OkButton_Click(object sender, System.EventArgs e)
			{
				DialogResult = WinForms.DialogResult.OK;
				Close();
			}

			private void CancelButton_Click(object sender, System.EventArgs e)
			{
				DialogResult = WinForms.DialogResult.Cancel;
				Close();
			}
		}
#endif
	}
}