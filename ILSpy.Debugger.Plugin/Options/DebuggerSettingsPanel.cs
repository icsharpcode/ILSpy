using ICSharpCode.ILSpy.Debugger;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Options
{
    [ExportOptionPage(Title = "Debugger", Order = 2)]
    internal partial class DebuggerSettingsPanel : UserControl, IOptionPage, IComponentConnector
    {
        public DebuggerSettingsPanel()
        {
            this.InitializeComponent();
        }

        public void Load(ILSpySettings settings)
        {
            DebuggerSettings instance = DebuggerSettings.Instance;
            instance.Load(settings);
            base.DataContext = instance;
        }

        public void Save(XElement root)
        {
            DebuggerSettings debuggerSettings = (DebuggerSettings)base.DataContext;
            debuggerSettings.Save(root);
        }

    }
}