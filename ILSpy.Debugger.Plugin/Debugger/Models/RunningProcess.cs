using System.Diagnostics;

namespace ICSharpCode.ILSpy.Debugger.Models
{
    internal sealed class RunningProcess
    {
        public int ProcessId { get; set; }

        public string WindowTitle { get; set; }

        public string ProcessName { get; set; }

        public string FileName { get; set; }

        public string Managed { get; set; }

        public Process Process { get; set; }
    }
}
