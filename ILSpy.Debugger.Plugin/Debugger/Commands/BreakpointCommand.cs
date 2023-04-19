using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AvalonEdit;
using ICSharpCode.ILSpy.Debugger.Services;
using System.Collections.Generic;
using System.Windows;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportBookmarkActionEntry(Icon = "images/Breakpoint.png", Category = "Debugger")]
    public class BreakpointCommand : IBookmarkActionEntry
    {
        public bool IsEnabled()
        {
            return true;
        }

        public void Execute(int line)
        {
            if (DebugInformation.CodeMappings != null && DebugInformation.CodeMappings.Count > 0)
            {
                Dictionary<int, MemberMapping> codeMappings = DebugInformation.CodeMappings;
                int num = 0;
                foreach (MemberMapping codeMapping in codeMappings.Values)
                {
                    SourceCodeMapping instructionByLineNumber = codeMapping.GetInstructionByLineNumber(line, out num);
                    if (instructionByLineNumber != null)
                    {
                        DebuggerService.ToggleBreakpointAt(instructionByLineNumber.MemberMapping.MemberReference, line, num, instructionByLineNumber.ILInstructionOffset);
                        break;
                    }
                }
                if (num == 0)
                {
                    MessageBox.Show(string.Format("Missing code mappings at line {0}.", line), "Code mappings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }
    }
}
