using System.ComponentModel;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Debugger
{
    public class DebuggerSettings : INotifyPropertyChanged
    {
        public static DebuggerSettings Instance
        {
            get
            {
                if (DebuggerSettings.s_instance == null)
                {
                    DebuggerSettings.s_instance = new DebuggerSettings();
                    ILSpySettings settings = ILSpySettings.Load();
                    DebuggerSettings.s_instance.Load(settings);
                }
                return DebuggerSettings.s_instance;
            }
        }

        private DebuggerSettings()
        {
        }

        public void Load(ILSpySettings settings)
        {
            XElement xelement = settings[DebuggerSettings.DEBUGGER_SETTINGS];
            this.ShowWarnings = (((bool?)xelement.Attribute(DebuggerSettings.SHOW_WARNINGS)) ?? this.ShowWarnings);
            this.AskForArguments = (((bool?)xelement.Attribute(DebuggerSettings.ASK_ARGUMENTS)) ?? this.AskForArguments);
            this.ShowAllBookmarks = (((bool?)xelement.Attribute(DebuggerSettings.SHOW_BOOKMARKS)) ?? this.ShowAllBookmarks);
            this.ShowModuleName = (((bool?)xelement.Attribute(DebuggerSettings.SHOW_MODULE)) ?? this.ShowModuleName);
            this.ShowArguments = (((bool?)xelement.Attribute(DebuggerSettings.SHOW_ARGUMENTS)) ?? this.ShowArguments);
            this.ShowArgumentValues = (((bool?)xelement.Attribute(DebuggerSettings.SHOW_ARGUMENTVALUE)) ?? this.ShowArgumentValues);
            this.BreakAtBeginning = (((bool?)xelement.Attribute(DebuggerSettings.BREAK_AT_BEGINNING)) ?? this.BreakAtBeginning);
        }

        public void Save(XElement root)
        {
            XElement xelement = new XElement(DebuggerSettings.DEBUGGER_SETTINGS);
            xelement.SetAttributeValue(DebuggerSettings.SHOW_WARNINGS, this.ShowWarnings);
            xelement.SetAttributeValue(DebuggerSettings.ASK_ARGUMENTS, this.AskForArguments);
            xelement.SetAttributeValue(DebuggerSettings.SHOW_BOOKMARKS, this.ShowAllBookmarks);
            xelement.SetAttributeValue(DebuggerSettings.SHOW_MODULE, this.ShowModuleName);
            xelement.SetAttributeValue(DebuggerSettings.SHOW_ARGUMENTS, this.ShowArguments);
            xelement.SetAttributeValue(DebuggerSettings.SHOW_ARGUMENTVALUE, this.ShowArgumentValues);
            xelement.SetAttributeValue(DebuggerSettings.BREAK_AT_BEGINNING, this.BreakAtBeginning);
            XElement xelement2 = root.Element(DebuggerSettings.DEBUGGER_SETTINGS);
            if (xelement2 != null)
            {
                xelement2.ReplaceWith(xelement);
                return;
            }
            root.Add(xelement);
        }

        [DefaultValue(true)]
        public bool ShowWarnings
        {
            get
            {
                return this.showWarnings;
            }
            set
            {
                if (this.showWarnings != value)
                {
                    this.showWarnings = value;
                    this.OnPropertyChanged("ShowWarnings");
                }
            }
        }

        [DefaultValue(true)]
        public bool AskForArguments
        {
            get
            {
                return this.askArguments;
            }
            set
            {
                if (this.askArguments != value)
                {
                    this.askArguments = value;
                    this.OnPropertyChanged("AskForArguments");
                }
            }
        }

        [DefaultValue(false)]
        public bool DebugWholeTypesOnly
        {
            get
            {
                return this.debugWholeTypesOnly;
            }
            set
            {
                if (this.debugWholeTypesOnly != value)
                {
                    this.debugWholeTypesOnly = value;
                    this.OnPropertyChanged("DebugWholeTypesOnly");
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowAllBookmarks
        {
            get
            {
                return this.showAllBookmarks;
            }
            set
            {
                if (this.showAllBookmarks != value)
                {
                    this.showAllBookmarks = value;
                    this.OnPropertyChanged("ShowAllBookmarks");
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowModuleName
        {
            get
            {
                return this.showModuleName;
            }
            set
            {
                if (this.showModuleName != value)
                {
                    this.showModuleName = value;
                    this.OnPropertyChanged("ShowModuleName");
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowArguments
        {
            get
            {
                return this.showArguments;
            }
            set
            {
                if (this.showArguments != value)
                {
                    this.showArguments = value;
                    this.OnPropertyChanged("ShowArguments");
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowArgumentValues
        {
            get
            {
                return this.showArgumentValues;
            }
            set
            {
                if (this.showArgumentValues != value)
                {
                    this.showArgumentValues = value;
                    this.OnPropertyChanged("ShowArgumentValues");
                }
            }
        }

        [DefaultValue(false)]
        public bool BreakAtBeginning
        {
            get
            {
                return this.breakAtBeginning;
            }
            set
            {
                if (this.breakAtBeginning != value)
                {
                    this.breakAtBeginning = value;
                    this.OnPropertyChanged("BreakAtBeginning");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static readonly string DEBUGGER_SETTINGS = "DebuggerSettings";

        private static readonly string SHOW_WARNINGS = "showWarnings";

        private static readonly string ASK_ARGUMENTS = "askForArguments";

        private static readonly string SHOW_BOOKMARKS = "showAllBookmarks";

        private static readonly string SHOW_MODULE = "showModuleName";

        private static readonly string SHOW_ARGUMENTS = "showArguments";

        private static readonly string SHOW_ARGUMENTVALUE = "showArgumentValues";

        private static readonly string BREAK_AT_BEGINNING = "breakAtBeginning";

        private bool showWarnings = true;

        private bool askArguments = true;

        private bool debugWholeTypesOnly;

        private bool showAllBookmarks;

        private bool showModuleName = true;

        private bool showArguments;

        private bool showArgumentValues;

        private bool breakAtBeginning;

        private static DebuggerSettings s_instance;
    }
}
