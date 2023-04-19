using Debugger.MetaData;
using ICSharpCode.ILSpy.Debugger.Services.Debugger;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class IEnumerableNode : TreeNode
    {
        public IEnumerableNode(Expression targetObject, DebugType itemType)
        {
            this.targetObject = targetObject;
            base.Name = "IEnumerable";
            this.Text = "Expanding will enumerate the IEnumerable";
            DebugType debugType;
            this.debugListExpression = DebuggerHelpers.CreateDebugListExpression(targetObject, itemType, out debugType);
            this.ChildNodes = Utils.LazyGetItemsOfIList(this.debugListExpression);
        }

        private Expression targetObject;

        private Expression debugListExpression;
    }
}
