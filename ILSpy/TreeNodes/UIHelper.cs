using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes
{
    public static class UIHelper
    {
		public static void AddReferenceWarningMessage(ILSpyTreeNode node, ITextOutput output, Language language)
		{
			var assemblyNode = node.AncestorsAndSelf().OfType<AssemblyTreeNode>().First();
			if (!assemblyNode.LoadedAssembly.LoadedAssemblyReferencesInfo.Any(i => i.Value.HasErrors))
				return;
			const string line1 = "Warning: Some assembly references could not be loaded. This might lead to incorrect decompilation of some parts,";
			const string line2 = "for ex. property getter/setter access. To get optimal decompilation results, please manually add the references to the list of loaded assemblies.";
			if (output is ISmartTextOutput fancyOutput) {
				fancyOutput.AddUIElement(() => new StackPanel {
					Margin = new Thickness(5),
					Orientation = Orientation.Horizontal,
					Children = {
						new Image {
							Width = 32,
							Height = 32,
							Source = Images.LoadImage(language, "Images/Warning.png")
						},
						new TextBlock {
							Margin = new Thickness(5, 0, 0, 0),
							Text = line1 + Environment.NewLine + line2
						}
					}
				});
				fancyOutput.WriteLine();
				fancyOutput.AddButton(Images.ViewCode, "Show assembly load log", delegate {
					MainWindow.Instance.SelectNode(assemblyNode.Children.OfType<ReferenceFolderTreeNode>().First());
				});
				fancyOutput.WriteLine();
			} else {
				language.WriteCommentLine(output, line1);
				language.WriteCommentLine(output, line2);
			}
		}
    }
}
