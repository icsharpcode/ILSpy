using ICSharpCode.ILSpy.Debugger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Debugger.UI
{
    public partial class AttachToProcessWindow : Window
    {
        public AttachToProcessWindow()
        {
            this.InitializeComponent();
            base.Loaded += this.OnLoaded;
        }

        public Process SelectedProcess
        {
            get
            {
                if (this.RunningProcesses.SelectedItem != null)
                {
                    return ((RunningProcess)this.RunningProcesses.SelectedItem).Process;
                }
                return null;
            }
        }

        private void RefreshProcessList()
        {
            ObservableCollection<RunningProcess> observableCollection = new ObservableCollection<RunningProcess>();
            Process currentProcess = Process.GetCurrentProcess();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (!process.HasExited)
                    {
                        if (currentProcess.Id != process.Id)
                        {
                            bool flag = false;
                            try
                            {
                                IEnumerable<ProcessModule> source = from ProcessModule m in process.Modules
                                                                    where m.ModuleName.StartsWith("mscor", StringComparison.OrdinalIgnoreCase)
                                                                    select m;
                                flag = (source.Count<ProcessModule>() > 0);
                            }
                            catch
                            {
                            }
                            if (flag)
                            {
                                observableCollection.Add(new RunningProcess
                                {
                                    ProcessId = process.Id,
                                    ProcessName = Path.GetFileName(process.MainModule.FileName),
                                    FileName = process.MainModule.FileName,
                                    WindowTitle = process.MainWindowTitle,
                                    Managed = "Managed",
                                    Process = process
                                });
                            }
                        }
                    }
                }
                catch (Win32Exception)
                {
                }
            }
            this.RunningProcesses.ItemsSource = observableCollection;
        }

        private void Attach()
        {
            if (this.RunningProcesses.SelectedItem == null)
            {
                return;
            }
            base.DialogResult = new bool?(true);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshProcessList();
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            this.Attach();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshProcessList();
        }

        private void RunningProcesses_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Attach();
        }
    }
}
