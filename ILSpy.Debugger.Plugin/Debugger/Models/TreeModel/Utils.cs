using Debugger;
using Debugger.MetaData;
using ICSharpCode.ILSpy.Debugger.Services;
using ICSharpCode.ILSpy.Debugger.Services.Debugger;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal static class Utils
    {
        public static void DoEvents(Process process)
        {
            if (process == null)
            {
                return;
            }
            DebuggeeState debuggeeState = process.DebuggeeState;
            Utils.WpfDoEvents();
            DebuggeeState debuggeeState2 = process.DebuggeeState;
            if (debuggeeState != debuggeeState2)
            {
                throw new AbortedBecauseDebuggeeResumedException();
            }
        }

        public static void WpfDoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate ()
            {
                frame.Continue = false;
            }));
            Dispatcher.PushFrame(frame);
        }

        public static IEnumerable<TreeNode> LazyGetChildNodesOfArray(Expression expression, ArrayDimensions dimensions)
        {
            if (dimensions.TotalElementCount == 0)
            {
                return new TreeNode[]
                {
                    new TreeNode(null, "(empty)", null, null, null)
                };
            }
            return new ArrayRangeNode(expression, dimensions, dimensions).ChildNodes;
        }

        public static IEnumerable<TreeNode> LazyGetChildNodesOfObject(Expression targetObject, DebugType shownType)
        {
            MemberInfo[] publicStatic = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            MemberInfo[] publicInstance = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            MemberInfo[] nonPublicStatic = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic);
            MemberInfo[] nonPublicInstance = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            DebugType baseType = (DebugType)shownType.BaseType;
            if (baseType != null)
            {
                yield return new TreeNode(ImageService.GetImage("Icons.16x16.Class"), "BaseClass", baseType.Name, baseType.FullName, (baseType.FullName == "System.Object") ? null : Utils.LazyGetChildNodesOfObject(targetObject, baseType));
            }
            if (nonPublicInstance.Length > 0)
            {
                yield return new TreeNode(null, "NonPublicMembers", string.Empty, string.Empty, Utils.LazyGetMembersOfObject(targetObject, nonPublicInstance));
            }
            if (publicStatic.Length > 0 || nonPublicStatic.Length > 0)
            {
                IEnumerable<TreeNode> childs = Utils.LazyGetMembersOfObject(targetObject, publicStatic);
                if (nonPublicStatic.Length > 0)
                {
                    TreeNode node2 = new TreeNode(null, "NonPublicStaticMembers", string.Empty, string.Empty, Utils.LazyGetMembersOfObject(targetObject, nonPublicStatic));
                    childs = Utils.PrependNode(node2, childs);
                }
                yield return new TreeNode(null, "StaticMembers", string.Empty, string.Empty, childs);
            }
            DebugType iListType = (DebugType)shownType.GetInterface(typeof(IList).FullName);
            DebugType iEnumerableType;
            DebugType itemType;
            if (iListType != null)
            {
                yield return new IListNode(targetObject);
            }
            else if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType))
            {
                yield return new IEnumerableNode(targetObject, itemType);
            }
            foreach (TreeNode node in Utils.LazyGetMembersOfObject(targetObject, publicInstance))
            {
                yield return node;
            }
            yield break;
        }

        public static IEnumerable<TreeNode> LazyGetMembersOfObject(Expression expression, MemberInfo[] members)
        {
            List<TreeNode> list = new List<TreeNode>();
            foreach (MemberInfo memberInfo in members)
            {
                string imageName;
                ImageSource imageForMember = ExpressionNode.GetImageForMember((IDebugMemberInfo)memberInfo, out imageName);
                list.Add(new ExpressionNode(imageForMember, memberInfo.Name, expression.AppendMemberReference((IDebugMemberInfo)memberInfo, new Expression[0]))
                {
                    ImageName = imageName
                });
            }
            return list;
        }

        public static IEnumerable<TreeNode> LazyGetItemsOfIList(Expression targetObject)
        {
            SimpleType type = new SimpleType
            {
                Identifier = typeof(IList).FullName
            };
            type.AddAnnotation(typeof(IList));
            targetObject = new CastExpression
            {
                Expression = targetObject.Clone(),
                Type = type
            };
            int count = 0;
            GetValueException error = null;
            try
            {
                count = Utils.GetIListCount(targetObject);
            }
            catch (GetValueException ex)
            {
                error = ex;
            }
            if (error != null)
            {
                yield return new TreeNode(null, "(error)", error.Message, null, null);
            }
            else if (count == 0)
            {
                yield return new TreeNode(null, "(empty)", null, null, null);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    string imageName;
                    ImageSource image = ExpressionNode.GetImageForArrayIndexer(out imageName);
                    yield return new ExpressionNode(image, "[" + i + "]", targetObject.AppendIndexer(new int[]
                    {
                        i
                    }))
                    {
                        ImageName = imageName
                    };
                }
            }
            yield break;
        }

        public static int GetIListCount(Expression targetObject)
        {
            Value value = targetObject.Evaluate(WindowsDebugger.CurrentProcess);
            Type @interface = value.Type.GetInterface(typeof(ICollection).FullName);
            if (@interface == null)
            {
                throw new GetValueException(targetObject, targetObject.PrettyPrint() + " does not implement System.Collections.ICollection");
            }
            PropertyInfo property = @interface.GetProperty("Count");
            return (int)value.GetPropertyValue(property, new Value[0]).PrimitiveValue;
        }

        public static IEnumerable<TreeNode> PrependNode(TreeNode node, IEnumerable<TreeNode> rest)
        {
            yield return node;
            if (rest != null)
            {
                foreach (TreeNode absNode in rest)
                {
                    yield return absNode;
                }
            }
            yield break;
        }
    }
}
