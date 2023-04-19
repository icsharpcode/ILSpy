using ICSharpCode.ILSpy.Debugger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    public abstract class DebuggerCommand : SimpleCommand
    {
        public DebuggerCommand()
        {
            MainWindow.Instance.KeyUp += this.OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (key != Key.F5)
            {
                if (key != Key.F11)
                {
                    if (key != Key.System)
                    {
                        return;
                    }
                    if (this is StepOverCommand)
                    {
                        ((StepOverCommand)this).Execute(null);
                        e.Handled = true;
                        return;
                    }
                }
                else if (this is StepIntoCommand)
                {
                    ((StepIntoCommand)this).Execute(null);
                    e.Handled = true;
                }
            }
            else if (this is ContinueDebuggingCommand)
            {
                ((ContinueDebuggingCommand)this).Execute(null);
                e.Handled = true;
                return;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static void SendWpfWindowPos(Window window, IntPtr place)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            DebuggerCommand.SetWindowPos(handle, place, 0, 0, 0, 0, 3U);
        }

        public override void Execute(object parameter)
        {
        }

        protected static IDebugger CurrentDebugger
        {
            get
            {
                return DebuggerService.CurrentDebugger;
            }
        }

        protected void StartExecutable(string fileName, string workingDirectory, string arguments)
        {
            DebuggerCommand.CurrentDebugger.BreakAtBeginning = DebuggerSettings.Instance.BreakAtBeginning;
            DebuggerCommand.CurrentDebugger.Start(new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = (workingDirectory ?? Path.GetDirectoryName(fileName)),
                Arguments = arguments
            });
            this.Finish();
        }

        protected void StartAttaching(Process process)
        {
            DebuggerCommand.CurrentDebugger.BreakAtBeginning = DebuggerSettings.Instance.BreakAtBeginning;
            DebuggerCommand.CurrentDebugger.Attach(process);
            this.Finish();
        }

        protected void Finish()
        {
            this.EnableDebuggerUI(false);
            DebuggerCommand.CurrentDebugger.DebugStopped += this.OnDebugStopped;
            DebuggerCommand.CurrentDebugger.IsProcessRunningChanged += this.CurrentDebugger_IsProcessRunningChanged;
            DebugInformation.IsDebuggerLoaded = true;
            MainWindow.Instance.SetStatus("Running...", Brushes.Black);
        }

        protected void OnDebugStopped(object sender, EventArgs e)
        {
            this.EnableDebuggerUI(true);
            DebuggerCommand.CurrentDebugger.DebugStopped -= this.OnDebugStopped;
            DebuggerCommand.CurrentDebugger.IsProcessRunningChanged -= this.CurrentDebugger_IsProcessRunningChanged;
            DebugInformation.IsDebuggerLoaded = false;
            MainWindow.Instance.SetStatus("Stand by...", Brushes.Black);
        }

        protected void EnableDebuggerUI(bool enable)
        {
            ItemCollection mainMenuItems = MainWindow.Instance.GetMainMenuItems();
            ItemCollection toolBarItems = MainWindow.Instance.GetToolBarItems();
            IEnumerable<MenuItem> source = from m in mainMenuItems.OfType<MenuItem>()
                                           where m.Header as string == "_Debugger"
                                           select m;
            foreach (MenuItem menuItem in source.First<MenuItem>().Items.OfType<MenuItem>())
            {
                string text = (string)menuItem.Header;
                if (!text.StartsWith("Remove") && !text.StartsWith("Show"))
                {
                    if (text.StartsWith("Attach") || text.StartsWith("Debug"))
                    {
                        menuItem.IsEnabled = enable;
                    }
                    else
                    {
                        menuItem.IsEnabled = !enable;
                    }
                }
            }
            IEnumerable<Button> enumerable = from b in toolBarItems.OfType<Button>()
                                             where b.Tag as string == "Debugger"
                                             select b;
            foreach (Button button in enumerable)
            {
                button.IsEnabled = enable;
            }
            MainWindow.Instance.SessionSettings.FilterSettings.ShowInternalApi = true;
        }

        private void CurrentDebugger_IsProcessRunningChanged(object sender, EventArgs e)
        {
            if (DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                MainWindow.Instance.SetStatus("Running...", Brushes.Black);
                return;
            }
            MainWindow instance = MainWindow.Instance;
            DebuggerCommand.SendWpfWindowPos(instance, DebuggerCommand.HWND_TOP);
            instance.Activate();
            if (DebugInformation.DebugStepInformation != null)
            {
                instance.JumpToReference(DebugInformation.DebugStepInformation.Item3);
            }
            instance.SetStatus("Debugging...", Brushes.Red);
        }

        private const uint SWP_NOSIZE = 1U;

        private const uint SWP_NOMOVE = 2U;

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private static readonly IntPtr HWND_TOP = new IntPtr(0);
    }
}
