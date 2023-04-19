using ICSharpCode.ILSpy.Debugger.Models.TreeModel;
using ICSharpCode.ILSpy.Debugger.Services;
using ICSharpCode.ILSpy.XmlDoc;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Debugger.UI
{
    public partial class CallStackPanel : UserControl, IPane
    {
        public static CallStackPanel Instance
        {
            get
            {
                if (CallStackPanel.s_instance == null)
                {
                    Application.Current.VerifyAccess();
                    CallStackPanel.s_instance = new CallStackPanel();
                }
                return CallStackPanel.s_instance;
            }
        }

        private CallStackPanel()
        {
            this.InitializeComponent();
        }

        public void Show()
        {
            if (!base.IsVisible)
            {
                DebuggerSettings.Instance.PropertyChanged += this.OnDebuggerSettingChanged;
                this.SwitchModuleColumn();
                MainWindow.Instance.ShowInBottomPane("Callstack", this);
                DebuggerService.DebugStarted += this.OnDebugStarted;
                DebuggerService.DebugStopped += this.OnDebugStopped;
                if (DebuggerService.IsDebuggerStarted)
                {
                    this.OnDebugStarted(null, EventArgs.Empty);
                }
            }
        }

        public void Closed()
        {
            DebuggerService.DebugStarted -= this.OnDebugStarted;
            DebuggerService.DebugStopped -= this.OnDebugStopped;
            if (this.m_currentDebugger != null)
            {
                this.OnDebugStopped(null, EventArgs.Empty);
            }
            DebuggerSettings.Instance.PropertyChanged -= this.OnDebuggerSettingChanged;
            ILSpySettings.Update(delegate (XElement root)
            {
                DebuggerSettings.Instance.Save(root);
            });
        }

        private void OnDebuggerSettingChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ShowModuleName")
            {
                this.SwitchModuleColumn();
                return;
            }
            if (args.PropertyName == "ShowArguments" || args.PropertyName == "ShowArgumentValues")
            {
                this.RefreshPad();
            }
        }

        private void OnDebugStarted(object sender, EventArgs args)
        {
            this.m_currentDebugger = DebuggerService.CurrentDebugger;
            this.m_currentDebugger.IsProcessRunningChanged += this.OnProcessRunningChanged;
            this.OnProcessRunningChanged(null, EventArgs.Empty);
        }

        private void OnDebugStopped(object sender, EventArgs args)
        {
            this.m_currentDebugger.IsProcessRunningChanged -= this.OnProcessRunningChanged;
            this.m_currentDebugger = null;
            this.view.ItemsSource = null;
        }

        private void OnProcessRunningChanged(object sender, EventArgs args)
        {
            if (this.m_currentDebugger.IsProcessRunning)
            {
                return;
            }
            this.RefreshPad();
        }

        private void SwitchModuleColumn()
        {
            foreach (GridViewColumn gridViewColumn in ((GridView)this.view.View).Columns)
            {
                if ((string)gridViewColumn.Header == "Module")
                {
                    gridViewColumn.Width = (DebuggerSettings.Instance.ShowModuleName ? double.NaN : 0.0);
                }
            }
        }

        private void RefreshPad()
        {
            global::Debugger.Process debuggedProcess = ((WindowsDebugger)this.m_currentDebugger).DebuggedProcess;
            if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null)
            {
                this.view.ItemsSource = null;
                return;
            }
            IList<CallStackItem> list = null;
            global::Debugger.StackFrame activeFrame = null;
            try
            {
                Utils.DoEvents(debuggedProcess);
                list = this.CreateItems(debuggedProcess);
                activeFrame = debuggedProcess.SelectedThread.SelectedStackFrame;
            }
            catch (AbortedBecauseDebuggeeResumedException)
            {
            }
            catch (System.Exception)
            {
                if (debuggedProcess != null && !debuggedProcess.HasExited)
                {
                    throw;
                }
            }
            this.view.ItemsSource = list;
            this.view.SelectedItem = ((list != null) ? list.FirstOrDefault((CallStackItem item) => object.Equals(activeFrame, item.Frame)) : null);
        }

        private IList<CallStackItem> CreateItems(global::Debugger.Process debuggedProcess)
        {
            List<CallStackItem> list = new List<CallStackItem>();
            foreach (global::Debugger.StackFrame stackFrame in debuggedProcess.SelectedThread.GetCallstack(100))
            {
                string moduleName = stackFrame.MethodInfo.DebugModule.ToString();
                CallStackItem callStackItem = new CallStackItem
                {
                    Name = CallStackPanel.GetFullName(stackFrame),
                    ModuleName = moduleName
                };
                callStackItem.Frame = stackFrame;
                list.Add(callStackItem);
                Utils.DoEvents(debuggedProcess);
            }
            return list;
        }

        internal static string GetFullName(global::Debugger.StackFrame frame)
        {
            bool showArguments = DebuggerSettings.Instance.ShowArguments;
            bool showArgumentValues = DebuggerSettings.Instance.ShowArgumentValues;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(frame.MethodInfo.DeclaringType.FullName);
            stringBuilder.Append('.');
            stringBuilder.Append(frame.MethodInfo.Name);
            if (showArguments || showArgumentValues)
            {
                stringBuilder.Append("(");
                for (int i = 0; i < frame.ArgumentCount; i++)
                {
                    string text = null;
                    string text2 = null;
                    if (showArguments)
                    {
                        try
                        {
                            text = frame.MethodInfo.GetParameters()[i].Name;
                        }
                        catch
                        {
                        }
                        if (text == "")
                        {
                            text = null;
                        }
                    }
                    if (showArgumentValues)
                    {
                        try
                        {
                            text2 = frame.GetArgumentValue(i).AsString(100);
                        }
                        catch
                        {
                        }
                    }
                    if (text != null && text2 != null)
                    {
                        stringBuilder.Append(text);
                        stringBuilder.Append("=");
                        stringBuilder.Append(text2);
                    }
                    if (text != null && text2 == null)
                    {
                        stringBuilder.Append(text);
                    }
                    if (text == null && text2 != null)
                    {
                        stringBuilder.Append(text2);
                    }
                    if (text == null && text2 == null)
                    {
                        stringBuilder.Append("Global.NA");
                    }
                    if (i < frame.ArgumentCount - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }
                stringBuilder.Append(")");
            }
            return stringBuilder.ToString();
        }

        private void view_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            CallStackItem callStackItem = this.view.SelectedItem as CallStackItem;
            if (callStackItem == null)
            {
                return;
            }
            LoadedAssembly loadedAssembly = MainWindow.Instance.CurrentAssemblyList.OpenAssembly(callStackItem.Frame.MethodInfo.DebugModule.FullPath);
            if (loadedAssembly == null || loadedAssembly.AssemblyDefinition == null)
            {
                return;
            }
            MemberReference memberReference = XmlDocKeyProvider.FindMemberByKey(loadedAssembly.AssemblyDefinition.MainModule, "M:" + callStackItem.Name);
            if (memberReference == null)
            {
                return;
            }
            MainWindow.Instance.JumpToReference(memberReference);
            e.Handled = true;
        }

        private void SwitchIsChecked(object sender, EventArgs args)
        {
            if (sender is MenuItem)
            {
                MenuItem menuItem = (MenuItem)sender;
                menuItem.IsChecked = !menuItem.IsChecked;
            }
        }

        private static CallStackPanel s_instance;

        private IDebugger m_currentDebugger;
    }
}
