using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class IListNode : TreeNode
    {
        public IListNode(Expression targetObject)
        {
            this.targetObject = targetObject;
            base.Name = "IList";
            this.count = Utils.GetIListCount(this.targetObject);
            this.ChildNodes = Utils.LazyGetItemsOfIList(this.targetObject);
        }

        public override bool HasChildNodes
        {
            get
            {
                return this.count > 0;
            }
        }

        private Expression targetObject;

        private int count;
    }
}
