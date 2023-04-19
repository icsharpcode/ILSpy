using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Services
{

    internal static class ExtensionMethods
    {

        public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }


        public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }


        public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            foreach (T obj in input)
            {
                action(obj);
            }
        }


        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
        {
            foreach (T item in elements)
            {
                list.Add(item);
            }
        }


        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> arr)
        {
            return new ReadOnlyCollection<T>(arr);
        }


        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
        {
            Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(input.GetEnumerator());
                while (stack.Count > 0)
                {
                    while (stack.Peek().MoveNext())
                    {
                        T element = stack.Peek().Current;
                        yield return element;
                        IEnumerable<T> children = recursion(element);
                        if (children != null)
                        {
                            stack.Push(children.GetEnumerator());
                        }
                    }
                    stack.Pop().Dispose();
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
            yield break;
        }

        public static T[] Splice<T>(this T[] array, int startIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            return array.Splice(startIndex, array.Length - startIndex);
        }

        public static T[] Splice<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (startIndex < 0 || startIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + array.Length);
            }
            if (length < 0 || length > array.Length - startIndex)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (array.Length - startIndex));
            }
            T[] array2 = new T[length];
            Array.Copy(array, startIndex, array2, 0, length);
            return array2;
        }


        public static System.Drawing.Point ToSystemDrawing(this System.Windows.Point p)
        {
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }


        public static System.Drawing.Size ToSystemDrawing(this System.Windows.Size s)
        {
            return new System.Drawing.Size((int)s.Width, (int)s.Height);
        }


        public static Rectangle ToSystemDrawing(this Rect r)
        {
            return new Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
        }


        public static System.Drawing.Color ToSystemDrawing(this System.Windows.Media.Color c)
        {
            return System.Drawing.Color.FromArgb((int)c.A, (int)c.R, (int)c.G, (int)c.B);
        }


        public static System.Windows.Point ToWpf(this System.Drawing.Point p)
        {
            return new System.Windows.Point((double)p.X, (double)p.Y);
        }


        public static System.Windows.Size ToWpf(this System.Drawing.Size s)
        {
            return new System.Windows.Size((double)s.Width, (double)s.Height);
        }

        public static Rect ToWpf(this Rectangle rect)
        {
            return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
        }

        public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static Rect TransformToDevice(this Rect rect, Visual visual)
        {
            Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return Rect.Transform(rect, transformToDevice);
        }

        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, transformFromDevice);
        }

        public static System.Windows.Size TransformToDevice(this System.Windows.Size size, Visual visual)
        {
            Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new System.Windows.Size(size.Width * transformToDevice.M11, size.Height * transformToDevice.M22);
        }

        public static System.Windows.Size TransformFromDevice(this System.Windows.Size size, Visual visual)
        {
            Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new System.Windows.Size(size.Width * transformFromDevice.M11, size.Height * transformFromDevice.M22);
        }

        public static System.Windows.Point TransformToDevice(this System.Windows.Point point, Visual visual)
        {
            Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new System.Windows.Point(point.X * transformToDevice.M11, point.Y * transformToDevice.M22);
        }

        public static System.Windows.Point TransformFromDevice(this System.Windows.Point point, Visual visual)
        {
            Matrix transformFromDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new System.Windows.Point(point.X * transformFromDevice.M11, point.Y * transformFromDevice.M22);
        }

        public static string RemoveStart(this string s, string stringToRemove)
        {
            if (s == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(stringToRemove))
            {
                return s;
            }
            if (!s.StartsWith(stringToRemove))
            {
                throw new ArgumentException(string.Format("{0} does not start with {1}", s, stringToRemove));
            }
            return s.Substring(stringToRemove.Length);
        }

        public static string RemoveEnd(this string s, string stringToRemove)
        {
            if (s == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(stringToRemove))
            {
                return s;
            }
            if (!s.EndsWith(stringToRemove))
            {
                throw new ArgumentException(string.Format("{0} does not end with {1}", s, stringToRemove));
            }
            return s.Substring(0, s.Length - stringToRemove.Length);
        }

        public static string TakeStart(this string s, int length)
        {
            if (string.IsNullOrEmpty(s) || length >= s.Length)
            {
                return s;
            }
            return s.Substring(0, length);
        }

        public static string TakeStartEllipsis(this string s, int length)
        {
            if (string.IsNullOrEmpty(s) || length >= s.Length)
            {
                return s;
            }
            return s.Substring(0, length) + "...";
        }

        public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (pattern.Length == 0)
            {
                throw new ArgumentException("String cannot be of zero length.", "pattern");
            }
            if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
            {
                throw new NotSupportedException("Currently only ordinal comparisons are implemented.");
            }
            StringBuilder stringBuilder = new StringBuilder(original.Length);
            int num = 0;
            for (int i = original.IndexOf(pattern, comparisonType); i >= 0; i = original.IndexOf(pattern, num, comparisonType))
            {
                stringBuilder.Append(original, num, i - num);
                num = i + pattern.Length;
                stringBuilder.Append(replacement);
            }
            stringBuilder.Append(original, num, original.Length - num);
            return stringBuilder.ToString();
        }

        public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
        {
            byte[] bytes = encoding.GetBytes(text);
            byte[] preamble = encoding.GetPreamble();
            if (preamble != null && preamble.Length > 0)
            {
                byte[] array = new byte[preamble.Length + bytes.Length];
                preamble.CopyTo(array, 0);
                bytes.CopyTo(array, preamble.Length);
                return array;
            }
            return bytes;
        }

        public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void AddIfNotNull<T>(this IList<T> list, T itemToAdd)
        {
            if (itemToAdd != null)
            {
                list.Add(itemToAdd);
            }
        }

        public static void WriteTo(this Stream sourceStream, Stream targetStream)
        {
            byte[] array = new byte[4096];
            int count;
            while ((count = sourceStream.Read(array, 0, array.Length)) > 0)
            {
                targetStream.Write(array, 0, count);
            }
        }
    }
}
