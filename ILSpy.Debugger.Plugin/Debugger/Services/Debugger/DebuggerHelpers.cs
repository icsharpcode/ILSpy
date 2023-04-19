using Debugger;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ICSharpCode.ILSpy.Debugger.Services.Debugger
{
    internal static class DebuggerHelpers
    {
        public static Expression CreateDebugListExpression(Expression iEnumerableVariable, DebugType itemType, out DebugType listType)
        {
            listType = DebugType.CreateFromType(itemType.AppDomain, typeof(List<>), new DebugType[]
            {
                itemType
            });
            DebugType type = DebugType.CreateFromType(itemType.AppDomain, typeof(IEnumerable<>), new DebugType[]
            {
                itemType
            });
            Expression singleItem = new CastExpression
            {
                Expression = iEnumerableVariable.Clone(),
                Type = type.GetTypeReference()
            };
            ObjectCreateExpression objectCreateExpression = new ObjectCreateExpression
            {
                Type = listType.GetTypeReference()
            };
            objectCreateExpression.Arguments.AddRange(singleItem.ToList<Expression>());
            return objectCreateExpression;
        }

        public static ulong GetObjectAddress(this Value val)
        {
            if (val.IsNull)
            {
                return 0UL;
            }
            ICorDebugReferenceValue corReferenceValue = val.CorReferenceValue;
            return corReferenceValue.GetValue();
        }

        public static bool IsEnum(this DebugType type)
        {
            return type.BaseType != null && type.BaseType.FullName == "System.Enum";
        }

        public static bool IsSystemDotObject(this DebugType type)
        {
            return type.FullName == "System.Object";
        }

        public static ulong GetObjectAddress(this Expression expr)
        {
            return expr.Evaluate(WindowsDebugger.CurrentProcess).GetObjectAddress();
        }

        public static int InvokeDefaultGetHashCode(this Value value)
        {
            if (DebuggerHelpers.hashCodeMethod == null || DebuggerHelpers.hashCodeMethod.Process.HasExited)
            {
                DebugType debugType = DebugType.CreateFromType(value.AppDomain, typeof(RuntimeHelpers), new DebugType[0]);
                DebuggerHelpers.hashCodeMethod = (DebugMethodInfo)debugType.GetMethod("GetHashCode", BindingFlags.Static | BindingFlags.Public);
                if (DebuggerHelpers.hashCodeMethod == null)
                {
                    throw new DebuggerException("Cannot obtain method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode");
                }
            }
            Value value2 = Eval.InvokeMethod(DebuggerHelpers.hashCodeMethod, null, new Value[]
            {
                value
            });
            return (int)value2.PrimitiveValue;
        }

        public static Value EvalPermanentReference(this Expression expr)
        {
            return expr.Evaluate(WindowsDebugger.CurrentProcess).GetPermanentReference();
        }

        public static bool IsNull(this Expression expr)
        {
            return expr.Evaluate(WindowsDebugger.CurrentProcess).IsNull;
        }

        public static DebugType GetType(this Expression expr)
        {
            return expr.Evaluate(WindowsDebugger.CurrentProcess).Type;
        }

        private static DebugMethodInfo hashCodeMethod;
    }
}
