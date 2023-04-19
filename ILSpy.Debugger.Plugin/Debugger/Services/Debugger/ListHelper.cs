using System.Collections.Generic;

namespace ICSharpCode.ILSpy.Debugger.Services.Debugger
{

    internal static class ListHelper
    {

        public static List<T> Sorted<T>(this List<T> list, IComparer<T> comparer)
        {
            list.Sort(comparer);
            return list;
        }


        public static List<T> Sorted<T>(this List<T> list)
        {
            list.Sort();
            return list;
        }


        public static List<T> ToList<T>(this T singleItem)
        {
            List<T> list = new List<T>();
            list.Add(singleItem);
            return list;
        }
    }
}
