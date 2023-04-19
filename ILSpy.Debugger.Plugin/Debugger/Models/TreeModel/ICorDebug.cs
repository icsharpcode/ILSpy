using Debugger;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using System;
using System.Collections.Generic;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class ICorDebug
    {
        public static ICorDebug.InfoNode GetDebugInfoRoot(global::Debugger.AppDomain appDomain, ICorDebugValue corValue)
        {
            return new ICorDebug.InfoNode("ICorDebug", "", ICorDebug.GetDebugInfo(appDomain, corValue));
        }

        public static List<TreeNode> GetDebugInfo(global::Debugger.AppDomain appDomain, ICorDebugValue corValue)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (corValue != null)
            {
                ICorDebug.InfoNode infoNode = new ICorDebug.InfoNode("ICorDebugValue", "");
                infoNode.AddChild("Address", corValue.GetAddress().ToString("X8"));
                infoNode.AddChild("Type", ((CorElementType)corValue.GetTheType()).ToString());
                infoNode.AddChild("Size", corValue.GetSize().ToString());
                list.Add(infoNode);
            }
            if (corValue is ICorDebugValue2)
            {
                ICorDebug.InfoNode infoNode2 = new ICorDebug.InfoNode("ICorDebugValue2", "");
                ICorDebugValue2 instance = (ICorDebugValue2)corValue;
                string text;
                try
                {
                    text = DebugType.CreateFromCorType(appDomain, instance.GetExactType()).FullName;
                }
                catch (DebuggerException ex)
                {
                    text = ex.Message;
                }
                infoNode2.AddChild("ExactType", text);
                list.Add(infoNode2);
            }
            if (corValue is ICorDebugGenericValue)
            {
                ICorDebug.InfoNode infoNode3 = new ICorDebug.InfoNode("ICorDebugGenericValue", "");
                try
                {
                    byte[] rawValue = ((ICorDebugGenericValue)corValue).GetRawValue();
                    for (int i = 0; i < rawValue.Length; i += 8)
                    {
                        string text2 = "";
                        int num = i;
                        while (num < rawValue.Length && num < i + 8)
                        {
                            text2 = text2 + rawValue[num].ToString("X2") + " ";
                            num++;
                        }
                        infoNode3.AddChild("Value" + i.ToString("X2"), text2);
                    }
                }
                catch (ArgumentException)
                {
                    infoNode3.AddChild("Value", "N/A");
                }
                list.Add(infoNode3);
            }
            if (corValue is ICorDebugReferenceValue)
            {
                ICorDebug.InfoNode infoNode4 = new ICorDebug.InfoNode("ICorDebugReferenceValue", "");
                ICorDebugReferenceValue instance2 = (ICorDebugReferenceValue)corValue;
                infoNode4.AddChild("IsNull", (instance2.IsNull() != 0).ToString());
                if (instance2.IsNull() == 0)
                {
                    infoNode4.AddChild("Value", instance2.GetValue().ToString("X8"));
                    if (instance2.Dereference() != null)
                    {
                        infoNode4.AddChild("Dereference", "", ICorDebug.GetDebugInfo(appDomain, instance2.Dereference()));
                    }
                    else
                    {
                        infoNode4.AddChild("Dereference", "N/A");
                    }
                }
                list.Add(infoNode4);
            }
            if (corValue is ICorDebugHeapValue)
            {
                ICorDebug.InfoNode item = new ICorDebug.InfoNode("ICorDebugHeapValue", "");
                list.Add(item);
            }
            if (corValue is ICorDebugHeapValue2)
            {
                ICorDebug.InfoNode item2 = new ICorDebug.InfoNode("ICorDebugHeapValue2", "");
                list.Add(item2);
            }
            if (corValue is ICorDebugObjectValue)
            {
                ICorDebug.InfoNode infoNode5 = new ICorDebug.InfoNode("ICorDebugObjectValue", "");
                ICorDebugObjectValue instance3 = (ICorDebugObjectValue)corValue;
                infoNode5.AddChild("Class", instance3.GetClass().GetToken().ToString("X8"));
                infoNode5.AddChild("IsValueClass", (instance3.IsValueClass() != 0).ToString());
                list.Add(infoNode5);
            }
            if (corValue is ICorDebugObjectValue2)
            {
                ICorDebug.InfoNode item3 = new ICorDebug.InfoNode("ICorDebugObjectValue2", "");
                list.Add(item3);
            }
            if (corValue is ICorDebugBoxValue)
            {
                ICorDebug.InfoNode infoNode6 = new ICorDebug.InfoNode("ICorDebugBoxValue", "");
                ICorDebugBoxValue instance4 = (ICorDebugBoxValue)corValue;
                infoNode6.AddChild("Object", "", ICorDebug.GetDebugInfo(appDomain, instance4.GetObject()));
                list.Add(infoNode6);
            }
            if (corValue is ICorDebugStringValue)
            {
                ICorDebug.InfoNode infoNode7 = new ICorDebug.InfoNode("ICorDebugStringValue", "");
                ICorDebugStringValue corDebugStringValue = (ICorDebugStringValue)corValue;
                infoNode7.AddChild("Length", corDebugStringValue.GetLength().ToString());
                infoNode7.AddChild("String", corDebugStringValue.GetString());
                list.Add(infoNode7);
            }
            if (corValue is ICorDebugArrayValue)
            {
                ICorDebug.InfoNode infoNode8 = new ICorDebug.InfoNode("ICorDebugArrayValue", "");
                infoNode8.AddChild("...", "...");
                list.Add(infoNode8);
            }
            if (corValue is ICorDebugHandleValue)
            {
                ICorDebug.InfoNode infoNode9 = new ICorDebug.InfoNode("ICorDebugHandleValue", "");
                ICorDebugHandleValue instance5 = (ICorDebugHandleValue)corValue;
                infoNode9.AddChild("HandleType", instance5.GetHandleType().ToString());
                list.Add(infoNode9);
            }
            return list;
        }

        public class InfoNode : TreeNode
        {
            public InfoNode(string name, string text) : this(name, text, null)
            {
            }

            public InfoNode(string name, string text, List<TreeNode> children)
            {
                base.Name = name;
                this.Text = text;
                this.ChildNodes = children;
                this.children = children;
            }

            public void AddChild(string name, string text)
            {
                if (this.children == null)
                {
                    this.children = new List<TreeNode>();
                    this.ChildNodes = this.children;
                }
                this.children.Add(new ICorDebug.InfoNode(name, text));
            }

            public void AddChild(string name, string text, List<TreeNode> subChildren)
            {
                if (this.children == null)
                {
                    this.children = new List<TreeNode>();
                    this.ChildNodes = this.children;
                }
                this.children.Add(new ICorDebug.InfoNode(name, text, subChildren));
            }

            private List<TreeNode> children;
        }
    }
}
