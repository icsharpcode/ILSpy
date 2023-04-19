using Debugger;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class StackFrameNode : TreeNode
    {
        public StackFrame StackFrame
        {
            get
            {
                return this.stackFrame;
            }
        }

        public StackFrameNode(StackFrame stackFrame)
        {
            this.stackFrame = stackFrame;
            base.Name = stackFrame.MethodInfo.Name;
            this.ChildNodes = this.LazyGetChildNodes();
        }

        private IEnumerable<TreeNode> LazyGetChildNodes()
        {
            foreach (DebugParameterInfo par in this.stackFrame.MethodInfo.GetParameters())
            {
                string imageName;
                ImageSource image = ExpressionNode.GetImageForParameter(out imageName);
                yield return new ExpressionNode(image, par.Name, par.GetExpression())
                {
                    ImageName = imageName
                };
            }
            foreach (DebugLocalVariableInfo locVar in this.stackFrame.MethodInfo.GetLocalVariables(this.StackFrame.IP))
            {
                string imageName2;
                ImageSource image2 = ExpressionNode.GetImageForLocalVariable(out imageName2);
                yield return new ExpressionNode(image2, locVar.Name, locVar.GetExpression())
                {
                    ImageName = imageName2
                };
            }
            if (this.stackFrame.Thread.CurrentException != null)
            {
                yield return new ExpressionNode(null, "__exception", new IdentifierExpression("__exception"));
            }
            yield break;
        }

        private StackFrame stackFrame;
    }
}
