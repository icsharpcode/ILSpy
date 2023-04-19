using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace ICSharpCode.ILSpy.Debugger.UI
{
    public partial class ExecuteProcessWindow : Window
    {
        public ExecuteProcessWindow()
        {
            this.InitializeComponent();
        }

        public string SelectedExecutable
        {
            get
            {
                return this.pathTextBox.Text;
            }
            set
            {
                this.pathTextBox.Text = value;
                this.workingDirectoryTextBox.Text = Path.GetDirectoryName(value);
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return this.workingDirectoryTextBox.Text;
            }
            set
            {
                this.workingDirectoryTextBox.Text = value;
            }
        }

        public string Arguments
        {
            get
            {
                return this.argumentsTextBox.Text;
            }
        }

        private void pathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = ".NET Executable (*.exe) | *.exe",
                InitialDirectory = this.workingDirectoryTextBox.Text,
                RestoreDirectory = true,
                DefaultExt = "exe"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.SelectedExecutable = openFileDialog.FileName;
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.SelectedExecutable))
            {
                return;
            }
            base.DialogResult = new bool?(true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void workingDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = this.workingDirectoryTextBox.Text
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.workingDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
