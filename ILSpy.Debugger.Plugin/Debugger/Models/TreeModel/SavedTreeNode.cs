using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class SavedTreeNode : TreeNode
    {
        public override bool CanSetText
        {
            get
            {
                return true;
            }
        }

        public SavedTreeNode(ImageSource image, string fullname, string text)
        {
            base.ImageSource = image;
            this.FullName = fullname;
            this.Text = text;
        }

        public override bool SetText(string newValue)
        {
            this.Text = newValue;
            return false;
        }
    }
}
