using ICSharpCode.ILSpy.Debugger.Models.TreeModel;
using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.ILSpy.Debugger.Tooltips
{
    internal class DebuggerPopup : Popup
    {
        public DebuggerPopup(DebuggerTooltipControl parentControl, TextLocation logicalPosition, bool showPins = true)
        {
            this.contentControl = new DebuggerTooltipControl(parentControl, logicalPosition, showPins);
            this.contentControl.containingPopup = this;
            base.Child = this.contentControl;
            this.IsLeaf = false;
        }

        public IEnumerable<ITreeNode> ItemsSource
        {
            get
            {
                return this.contentControl.ItemsSource;
            }
            set
            {
                this.contentControl.SetItemsSource(value);
            }
        }

        public bool IsLeaf
        {
            get
            {
                return this.isLeaf;
            }
            set
            {
                this.isLeaf = value;
                base.StaysOpen = !this.isLeaf;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (this.isLeaf)
            {
                this.contentControl.CloseOnLostFocus();
            }
        }

        public void Open()
        {
            base.IsOpen = true;
        }

        public void CloseSelfAndChildren()
        {
            this.contentControl.CloseChildPopups();
            base.IsOpen = false;
        }

        internal DebuggerTooltipControl contentControl;

        private bool isLeaf;
    }
}
