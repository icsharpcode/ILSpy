using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.ILSpy.AvalonEdit;
using ICSharpCode.ILSpy.Debugger.Services;
using ICSharpCode.NRefactory;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Debugger.Tooltips
{
    [Export(typeof(ITextEditorListener))]
    public class TextEditorListener : ITextEditorListener, IWeakEventListener
    {
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(ICSharpCode.ILSpy.AvalonEdit.TextEditorWeakEventManager.MouseHover))
            {
                this.editor = (TextEditor)sender;
                this.OnMouseHover((MouseEventArgs)e);
            }
            if (managerType == typeof(ICSharpCode.ILSpy.AvalonEdit.TextEditorWeakEventManager.MouseHoverStopped))
            {
                this.OnMouseHoverStopped((MouseEventArgs)e);
            }
            if (managerType == typeof(ICSharpCode.ILSpy.AvalonEdit.TextEditorWeakEventManager.MouseDown))
            {
                this.OnMouseDown((MouseEventArgs)e);
            }
            return true;
        }

        private void OnMouseDown(MouseEventArgs e)
        {
            this.ClosePopup();
        }

        private void OnMouseHoverStopped(MouseEventArgs e)
        {
        }

        private void OnMouseHover(MouseEventArgs e)
        {
            ToolTipRequestEventArgs toolTipRequestEventArgs = new ToolTipRequestEventArgs(this.editor);
            TextViewPosition? positionFromPoint = this.editor.GetPositionFromPoint(e.GetPosition(this.editor));
            toolTipRequestEventArgs.InDocument = (positionFromPoint != null);
            if (positionFromPoint != null)
            {
                toolTipRequestEventArgs.LogicalPosition = new TextLocation(positionFromPoint.Value.Line, positionFromPoint.Value.Column);
                DebuggerService.HandleToolTipRequest(toolTipRequestEventArgs);
                if (toolTipRequestEventArgs.ContentToShow != null)
                {
                    ITooltip tooltip = toolTipRequestEventArgs.ContentToShow as ITooltip;
                    if (tooltip != null && tooltip.ShowAsPopup)
                    {
                        if (!(toolTipRequestEventArgs.ContentToShow is UIElement))
                        {
                            throw new NotSupportedException("Content to show in Popup must be UIElement: " + toolTipRequestEventArgs.ContentToShow);
                        }
                        if (this.popup == null)
                        {
                            this.popup = this.CreatePopup();
                        }
                        if (this.TryCloseExistingPopup(false))
                        {
                            tooltip.Closed += delegate (object A_1, RoutedEventArgs A_2)
                            {
                                this.popup.IsOpen = false;
                            };
                            this.popup.Child = (UIElement)toolTipRequestEventArgs.ContentToShow;
                            this.SetPopupPosition(this.popup, e);
                            this.popup.IsOpen = true;
                        }
                        e.Handled = true;
                        return;
                    }
                }
                else
                {
                    if (this.popup != null)
                    {
                        e.Handled = true;
                    }
                    this.TryCloseExistingPopup(false);
                }
                return;
            }
        }

        private bool TryCloseExistingPopup(bool mouseClick)
        {
            bool flag = true;
            if (this.popup != null)
            {
                ITooltip tooltip = this.popup.Child as ITooltip;
                if (tooltip != null)
                {
                    flag = tooltip.Close(mouseClick);
                }
                if (flag)
                {
                    this.popup.IsOpen = false;
                }
            }
            return flag;
        }

        private void SetPopupPosition(Popup popup, MouseEventArgs mouseArgs)
        {
            Point popupPosition = this.GetPopupPosition(mouseArgs);
            popup.HorizontalOffset = popupPosition.X;
            popup.VerticalOffset = popupPosition.Y;
        }

        private Point GetPopupPosition(MouseEventArgs mouseArgs)
        {
            Point position = mouseArgs.GetPosition(this.editor);
            TextViewPosition? positionFromPoint = this.editor.GetPositionFromPoint(position);
            Point point;
            if (positionFromPoint != null)
            {
                ICSharpCode.AvalonEdit.Rendering.TextView textView = this.editor.TextArea.TextView;
                point = textView.PointToScreen(textView.GetVisualPosition(positionFromPoint.Value, VisualYPosition.LineBottom) - textView.ScrollOffset);
                point.X -= 4.0;
            }
            else
            {
                point = this.editor.PointToScreen(position + new Vector(-4.0, 6.0));
            }
            return point.TransformFromDevice(this.editor);
        }

        private Popup CreatePopup()
        {
            this.popup = new Popup();
            this.popup.Closed += delegate (object A_1, EventArgs A_2)
            {
                this.popup = null;
            };
            this.popup.AllowsTransparency = true;
            this.popup.PlacementTarget = this.editor;
            this.popup.Placement = PlacementMode.Absolute;
            this.popup.StaysOpen = true;
            return this.popup;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return this.ReceiveWeakEvent(managerType, sender, e);
        }

        public void ClosePopup()
        {
            this.TryCloseExistingPopup(true);
            if (this.popup != null)
            {
                this.popup.IsOpen = false;
            }
        }

        private Popup popup;

        private TextEditor editor;
    }
}
