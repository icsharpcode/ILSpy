using System;

using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Bookmarks
{
	public class MemberBookmarks : IBookmark
	{
        private IMemberReference member;

        public IMemberReference Member => member;

        public virtual ImageSource Image
        {
            get
            {
                if (member is IField)
                {
                    return FieldTreeNode.GetIcon((IField)member);
                }
                if (member is IProperty)
                {
                    return PropertyTreeNode.GetIcon((IProperty)member);
                }
                if (member is IEvent)
                {
                    return EventTreeNode.GetIcon((IEvent)member);
                }
                if (member is IMethod)
                {
                    return MethodTreeNode.GetIcon((IMethod)member);
                }
                if (member is ITypeDefinition)
                {
                    return TypeTreeNode.GetIcon((ITypeDefinition)member);
                }
                return null;
            }
        }
		
		public int LineNumber { get; private set; }

        int IBookmark.ZOrder => -10;

        bool IBookmark.CanDragDrop => false;

        public MemberBookmarks(IMemberReference member, int line)
        {
            this.member = member;
            LineNumber = line;
        }

        public virtual void MouseDown(MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUp(MouseButtonEventArgs e)
        {
        }

       /* void IBookmark.Drop(int lineNumber)
        {
            throw new NotSupportedException();
        }
	   */
    }

	public interface IBookmark
	{
		int LineNumber { get; }
		int ZOrder { get; }
		bool CanDragDrop { get; }


	

	}
}
