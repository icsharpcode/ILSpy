using Debugger.MetaData;
using System;

namespace ICSharpCode.ILSpy.Debugger.Services.Debugger
{
    internal static class TypeResolverExtension
    {
        public static DebugType GetGenericInterface(this DebugType type, string fullNamePrefix)
        {
            foreach (DebugType debugType in type.GetInterfaces())
            {
                if (debugType.FullName.StartsWith(fullNamePrefix) && debugType.GetGenericArguments().Length > 0)
                {
                    return debugType;
                }
            }
            if (!(type.BaseType == null))
            {
                return (DebugType)type.BaseType.GetInterface(fullNamePrefix);
            }
            return null;
        }

        public static bool ResolveIListImplementation(this DebugType type, out DebugType iListType, out DebugType itemType)
        {
            return type.ResolveGenericInterfaceImplementation("System.Collections.Generic.IList", out iListType, out itemType);
        }

        public static bool ResolveIEnumerableImplementation(this DebugType type, out DebugType iEnumerableType, out DebugType itemType)
        {
            return type.ResolveGenericInterfaceImplementation("System.Collections.Generic.IEnumerable", out iEnumerableType, out itemType);
        }

        public static bool ResolveGenericInterfaceImplementation(this DebugType type, string fullNamePrefix, out DebugType implementation, out DebugType itemType)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            implementation = null;
            itemType = null;
            implementation = type.GetGenericInterface(fullNamePrefix);
            if (implementation != null && implementation.GetGenericArguments().Length == 1)
            {
                itemType = (DebugType)implementation.GetGenericArguments()[0];
                return true;
            }
            return false;
        }
    }
}
