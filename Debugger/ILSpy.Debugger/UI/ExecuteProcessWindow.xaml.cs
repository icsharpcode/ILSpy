// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Debugger.UI
{
  /// <summary>
  /// Interaction logic for ExecuteProcessWindow.xaml
  /// </summary>
  public partial class ExecuteProcessWindow : Window
  {
    public ExecuteProcessWindow()
    {
      InitializeComponent();
      InitFromExecutable();
    }

    public string SelectedExecutable
    {
      get
      {
        return pathTextBox.Text;
      }
      set
      {
        pathTextBox.Text = value;
        // just in case we have no stored working directory
        workingDirectoryTextBox.Text = Path.GetDirectoryName(value);
        InitFromExecutable(value);
      }
    }

    public string WorkingDirectory
    {
      get
      {
        return workingDirectoryTextBox.Text;
      }
      set
      {
        workingDirectoryTextBox.Text = value;
      }
    }

    public string Arguments
    {
      get
      {
        return argumentsTextBox.Text;
      }
      private set
      {
        argumentsTextBox.Text = value;
      }
    }

    void InitFromExecutable(string file = null)
    {
      var settings = ILSpySettings.Load();
      var debuggerSettings = settings["DebuggerSettings"];
      var executables = debuggerSettings.Element("Executables");
      if (null == executables)
        return;
      if (null == file)
        file = (string)executables.Attribute("last");
      var lastAssembly = executables.Elements("Assembly").FirstOrDefault(a => (string)a.Attribute("name") == file);
      if (null == lastAssembly)
        return;
      pathTextBox.Text = (string)lastAssembly.Attribute("name");
      workingDirectoryTextBox.Text = (string)lastAssembly.Attribute("workingDirectory");
      argumentsTextBox.Text = (string)lastAssembly.Attribute("arguments");
    }

    void pathButton_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog()
      {
        Filter = ".NET Executable (*.exe) | *.exe",
        InitialDirectory = workingDirectoryTextBox.Text,
        RestoreDirectory = true,
        DefaultExt = "exe"
      };

      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        SelectedExecutable = dialog.FileName;
      }
    }

    void ExecuteButton_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(SelectedExecutable))
        return;
      // update settings
      ILSpySettings.Update(
        delegate(XElement root)
        {
          XElement debuggerSettings = root.Element("DebuggerSettings");
          if (debuggerSettings == null)
          {
            debuggerSettings = new XElement("DebuggerSettings");
            root.Add(debuggerSettings);
          }
          XElement executables = debuggerSettings.Element("Executables");
          if (executables == null)
          {
            executables = new XElement("Executables");
            debuggerSettings.Add(executables);
          }
          executables.SetAttributeValue("last", SelectedExecutable);
          XElement assembly = new XElement("Assembly");
          assembly.SetAttributeValue("name", SelectedExecutable);
          assembly.SetAttributeValue("workingDirectory", WorkingDirectory);
          assembly.SetAttributeValue("arguments", Arguments);

          XElement listElement = executables.Elements("Assembly").FirstOrDefault(a => (string)a.Attribute("name") == SelectedExecutable);
          if (listElement != null)
            listElement.ReplaceWith(assembly);
          else
            executables.Add(assembly);

        });
      this.DialogResult = true;
    }

    void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    void workingDirectoryButton_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog dialog = new FolderBrowserDialog()
      {
        SelectedPath = workingDirectoryTextBox.Text
      };
      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        workingDirectoryTextBox.Text = dialog.SelectedPath;
      }
    }
  }
}