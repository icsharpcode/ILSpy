using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class TreeNode : ITreeNode, IComparable<ITreeNode>
    {
        public ImageSource ImageSource { get; protected set; }

        public string Name { get; set; }

        public string ImageName { get; set; }

        public virtual string FullName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        public virtual string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public virtual string Type { get; protected set; }

        public virtual IEnumerable<TreeNode> ChildNodes
        {
            get
            {
                return this.childNodes;
            }
            protected set
            {
                this.childNodes = value;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.ChildNodes
        {
            get
            {
                return this.childNodes;
            }
        }

        public virtual bool HasChildNodes
        {
            get
            {
                return this.childNodes != null;
            }
        }

        public virtual bool CanSetText
        {
            get
            {
                return false;
            }
        }

        public virtual IEnumerable<IVisualizerCommand> VisualizerCommands
        {
            get
            {
                return null;
            }
        }

        public virtual bool HasVisualizerCommands
        {
            get
            {
                return this.VisualizerCommands != null && this.VisualizerCommands.Count<IVisualizerCommand>() > 0;
            }
        }

        public bool IsPinned { get; set; }

        public TreeNode()
        {
        }

        public TreeNode(ImageSource iconImage, string name, string text, string type, IEnumerable<TreeNode> childNodes)
        {
            this.ImageSource = iconImage;
            this.Name = name;
            this.text = text;
            this.Type = type;
            this.childNodes = childNodes;
        }

        public int CompareTo(ITreeNode other)
        {
            return this.FullName.CompareTo(other.FullName);
        }

        public virtual bool SetText(string newValue)
        {
            return false;
        }

        private string text = string.Empty;

        private IEnumerable<TreeNode> childNodes;
    }
}
