using Debugger;
using Debugger.MetaData;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.ILSpy.Debugger.Services;
using ICSharpCode.ILSpy.Debugger.Services.Debugger;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class ExpressionNode : TreeNode, ISetText, INotifyPropertyChanged
    {
        public bool Evaluated
        {
            get
            {
                return this.evaluated;
            }
            set
            {
                this.evaluated = value;
            }
        }

        public ICSharpCode.NRefactory.CSharp.Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        public override bool CanSetText
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return this.canSetText;
            }
        }

        public GetValueException Error
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return this.error;
            }
        }

        public string FullText
        {
            get
            {
                return this.fullText;
            }
        }

        public override string Text
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return base.Text;
            }
            set
            {
                if (value != base.Text)
                {
                    base.Text = value;
                    this.NotifyPropertyChanged("Text");
                }
            }
        }

        public override string FullName
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return this.expression.PrettyPrint() ?? base.Name.Trim();
            }
        }

        public override string Type
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return base.Type;
            }
        }

        public override IEnumerable<TreeNode> ChildNodes
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return base.ChildNodes;
            }
        }

        public override bool HasChildNodes
        {
            get
            {
                if (!this.evaluated)
                {
                    this.EvaluateExpression();
                }
                return base.HasChildNodes;
            }
        }

        public override IEnumerable<IVisualizerCommand> VisualizerCommands
        {
            get
            {
                if (this.visualizerCommands == null)
                {
                    this.visualizerCommands = this.getAvailableVisualizerCommands();
                }
                return this.visualizerCommands;
            }
        }

        private IEnumerable<IVisualizerCommand> getAvailableVisualizerCommands()
        {
            if (!this.evaluated)
            {
                this.EvaluateExpression();
            }
            if (!(this.expressionType == null) && !this.valueIsNull && !this.expressionType.IsPrimitive && !this.expressionType.IsSystemDotObject())
            {
                this.expressionType.IsEnum();
            }
            yield break;
        }

        public ExpressionNode(ImageSource image, string name, ICSharpCode.NRefactory.CSharp.Expression expression)
        {
            base.ImageSource = image;
            base.Name = name;
            this.expression = expression;
        }

        private void EvaluateExpression()
        {
            this.evaluated = true;
            Value value;
            try
            {
                StackFrame mostRecentStackFrame = ExpressionNode.WindowsDebugger.DebuggedProcess.SelectedThread.MostRecentStackFrame;
                int metadataToken = mostRecentStackFrame.MethodInfo.MetadataToken;
                int num = base.Name.IndexOf('.');
                string targetName = base.Name;
                if (num != -1)
                {
                    targetName = base.Name.Substring(0, num);
                }
                MemberMapping memberMapping;
                if (DebugInformation.CodeMappings != null && DebugInformation.CodeMappings.TryGetValue(metadataToken, out memberMapping))
                {
                    ILVariable ilvariable = memberMapping.LocalVariables.FirstOrDefault((ILVariable v) => v.Name == targetName);
                    if (ilvariable != null && ilvariable.OriginalVariable != null)
                    {
                        if (this.expression is MemberReferenceExpression)
                        {
                            MemberReferenceExpression memberReferenceExpression = (MemberReferenceExpression)this.expression;
                            memberReferenceExpression.Target.AddAnnotation(new int[]
                            {
                                ilvariable.OriginalVariable.Index
                            });
                        }
                        else
                        {
                            this.expression.AddAnnotation(new int[]
                            {
                                ilvariable.OriginalVariable.Index
                            });
                        }
                    }
                }
                value = this.expression.Evaluate(ExpressionNode.WindowsDebugger.DebuggedProcess);
            }
            catch (GetValueException ex)
            {
                this.error = ex;
                this.Text = ex.Message;
                return;
            }
            this.canSetText = value.Type.IsPrimitive;
            this.expressionType = value.Type;
            this.Type = value.Type.Name;
            this.valueIsNull = value.IsNull;
            if (!value.IsNull && !value.Type.IsPrimitive && !(value.Type.FullName == typeof(string).FullName))
            {
                if (value.Type.IsArray)
                {
                    if (value.ArrayLength > 0)
                    {
                        this.ChildNodes = Utils.LazyGetChildNodesOfArray(this.Expression, value.ArrayDimensions);
                    }
                }
                else if (value.Type.IsClass || value.Type.IsValueType)
                {
                    if (value.Type.FullNameWithoutGenericArguments == typeof(List<>).FullName)
                    {
                        if ((int)value.GetMemberValue("_size").PrimitiveValue > 0)
                        {
                            this.ChildNodes = Utils.LazyGetItemsOfIList(this.expression);
                        }
                    }
                    else
                    {
                        this.ChildNodes = Utils.LazyGetChildNodesOfObject(this.Expression, value.Type);
                    }
                }
                else if (value.Type.IsPointer)
                {
                    Value value2 = value.Dereference();
                    if (value2 != null)
                    {
                        this.ChildNodes = new ExpressionNode[]
                        {
                            new ExpressionNode(base.ImageSource, "*" + base.Name, this.Expression.AppendDereference())
                        };
                    }
                }
            }
            if (value.Type.IsInteger)
            {
                this.fullText = this.FormatInteger(value.PrimitiveValue);
            }
            else if (value.Type.IsPointer)
            {
                this.fullText = string.Format("0x{0:X}", value.PointerAddress);
            }
            else
            {
                if ((value.Type.FullName == typeof(string).FullName || value.Type.FullName == typeof(char).FullName) && !value.IsNull)
                {
                    try
                    {
                        this.fullText = '"' + this.Escape(value.InvokeToString(int.MaxValue)) + '"';
                        goto IL_40E;
                    }
                    catch (GetValueException ex2)
                    {
                        this.error = ex2;
                        this.fullText = ex2.Message;
                        return;
                    }
                }
                if ((value.Type.IsClass || value.Type.IsValueType) && !value.IsNull)
                {
                    try
                    {
                        this.fullText = value.InvokeToString(int.MaxValue);
                        goto IL_40E;
                    }
                    catch (GetValueException ex3)
                    {
                        this.error = ex3;
                        this.fullText = ex3.Message;
                        return;
                    }
                }
                this.fullText = value.AsString(int.MaxValue);
            }
        IL_40E:
            this.Text = ((this.fullText.Length > 256) ? (this.fullText.Substring(0, 256) + "...") : this.fullText);
        }

        private string Escape(string source)
        {
            return source.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\0", "\\0").Replace("\b", "\\b").Replace("\a", "\\a").Replace("\f", "\\f").Replace("\v", "\\v").Replace("\"", "\\\"");
        }

        private string FormatInteger(object i)
        {
            return i.ToString();
        }

        private bool ShowAsHex(object i)
        {
            ulong num;
            if (i is sbyte || i is short || i is int || i is long)
            {
                num = (ulong)Convert.ToInt64(i);
                if (num > 9223372036854775807UL)
                {
                    num = ~num + 1UL;
                }
            }
            else
            {
                num = Convert.ToUInt64(i);
            }
            if (num >= 65536UL)
            {
                return true;
            }
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            while (num != 0UL)
            {
                while ((num & 1UL) == 0UL)
                {
                    num >>= 1;
                    num4++;
                }
                while ((num & 1UL) == 1UL)
                {
                    num >>= 1;
                    num4++;
                    num2++;
                }
                num3++;
            }
            return num4 >= 7 && num3 <= (num4 + 7) / 8;
        }

        public override bool SetText(string newText)
        {
            string fullName = this.FullName;
            Value value = null;
            try
            {
                value = this.Expression.Evaluate(ExpressionNode.WindowsDebugger.DebuggedProcess);
                if (value.Type.IsInteger && newText.StartsWith("0x"))
                {
                    try
                    {
                        value.PrimitiveValue = long.Parse(newText.Substring(2), NumberStyles.HexNumber);
                        goto IL_6C;
                    }
                    catch (FormatException)
                    {
                        throw new NotSupportedException();
                    }
                    catch (OverflowException)
                    {
                        throw new NotSupportedException();
                    }
                }
                value.PrimitiveValue = newText;
            IL_6C:
                this.Text = newText;
                return true;
            }
            catch (NotSupportedException)
            {
                string format = "Can not convert {0} to {1}";
                string messageBoxText = string.Format(format, newText, value.Type.PrimitiveType);
                MessageBox.Show(messageBoxText);
            }
            catch (COMException)
            {
                MessageBox.Show("UnknownError");
            }
            return false;
        }

        public static ImageSource GetImageForThis(out string imageName)
        {
            imageName = "Icons.16x16.Parameter";
            return ImageService.GetImage(imageName);
        }

        public static ImageSource GetImageForParameter(out string imageName)
        {
            imageName = "Icons.16x16.Parameter";
            return ImageService.GetImage(imageName);
        }

        public static ImageSource GetImageForLocalVariable(out string imageName)
        {
            imageName = "Icons.16x16.Local";
            return ImageService.GetImage(imageName);
        }

        public static ImageSource GetImageForArrayIndexer(out string imageName)
        {
            imageName = "Icons.16x16.Field";
            return ImageService.GetImage(imageName);
        }

        public static ImageSource GetImageForMember(IDebugMemberInfo memberInfo, out string imageName)
        {
            string text = string.Empty;
            if (!memberInfo.IsPublic)
            {
                if (memberInfo.IsAssembly)
                {
                    text += "Internal";
                }
                else if (memberInfo.IsFamily)
                {
                    text += "Protected";
                }
                else if (memberInfo.IsPrivate)
                {
                    text += "Private";
                }
            }
            if (memberInfo is FieldInfo)
            {
                text += "Field";
            }
            else if (memberInfo is PropertyInfo)
            {
                text += "Property";
            }
            else
            {
                if (!(memberInfo is MethodInfo))
                {
                    throw new DebuggerException("Unknown member type " + memberInfo.GetType().FullName);
                }
                text += "Method";
            }
            imageName = "Icons.16x16." + text;
            return ImageService.GetImage(imageName);
        }

        public static WindowsDebugger WindowsDebugger
        {
            get
            {
                return (WindowsDebugger)DebuggerService.CurrentDebugger;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private bool evaluated;

        private ICSharpCode.NRefactory.CSharp.Expression expression;

        private bool canSetText;

        private GetValueException error;

        private string fullText;

        private DebugType expressionType;

        private bool valueIsNull = true;

        private IEnumerable<IVisualizerCommand> visualizerCommands;
    }
}
