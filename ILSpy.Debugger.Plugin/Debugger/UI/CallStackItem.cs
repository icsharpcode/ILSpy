using Debugger;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.UI
{
    public class CallStackItem
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public StackFrame Frame { get; set; }

        public string Line { get; set; }

        public string ModuleName { get; set; }

        public Brush FontColor
        {
            get
            {
                return Brushes.Black;
            }
        }
    }
}
