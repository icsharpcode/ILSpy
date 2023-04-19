using ICSharpCode.ILSpy.Debugger.UI;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using Mono.Cecil;
using System.Linq;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportContextMenuEntry(Header = "_Debug Assembly", Icon = "Images/application-x-executable.png")]
    internal sealed class DebugExecutableNodeCommand : DebuggerCommand, IContextMenuEntry
    {
        public bool IsVisible(TextViewContext context)
        {
            if (context.SelectedTreeNodes != null)
            {
                return context.SelectedTreeNodes.All(delegate (SharpTreeNode n)
                {
                    AssemblyTreeNode assemblyTreeNode = n as AssemblyTreeNode;
                    if (assemblyTreeNode == null)
                    {
                        return false;
                    }
                    AssemblyDefinition assemblyDefinition = assemblyTreeNode.LoadedAssembly.AssemblyDefinition;
                    return assemblyDefinition != null && assemblyDefinition.EntryPoint != null;
                });
            }
            return false;
        }

        public bool IsEnabled(TextViewContext context)
        {
            return context.SelectedTreeNodes != null && context.SelectedTreeNodes.Length == 1;
        }

        public void Execute(TextViewContext context)
        {
            if (context.SelectedTreeNodes == null)
            {
                return;
            }
            if (!DebuggerCommand.CurrentDebugger.IsDebugging)
            {
                AssemblyTreeNode assemblyTreeNode = context.SelectedTreeNodes[0] as AssemblyTreeNode;
                if (DebuggerSettings.Instance.AskForArguments)
                {
                    ExecuteProcessWindow executeProcessWindow = new ExecuteProcessWindow
                    {
                        Owner = MainWindow.Instance,
                        SelectedExecutable = assemblyTreeNode.LoadedAssembly.FileName
                    };
                    if (executeProcessWindow.ShowDialog() == true)
                    {
                        string selectedExecutable = executeProcessWindow.SelectedExecutable;
                        base.StartExecutable(selectedExecutable, executeProcessWindow.WorkingDirectory, executeProcessWindow.Arguments);
                        return;
                    }
                }
                else
                {
                    base.StartExecutable(assemblyTreeNode.LoadedAssembly.FileName, null, null);
                }
            }
        }
    }
}
