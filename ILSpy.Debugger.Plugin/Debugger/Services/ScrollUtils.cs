using Mono.Cecil;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Services
{
    public static class ScrollUtils
    {
        public static ScrollViewer GetScrollViewer(this DependencyObject o)
        {
            ScrollViewer scrollViewer = o as ScrollViewer;
            if (scrollViewer != null)
            {
                return scrollViewer;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(o, i);
                ScrollViewer scrollViewer2 = child.GetScrollViewer();
                if (scrollViewer2 != null)
                {
                    return scrollViewer2;
                }
            }
            return null;
        }

        public static void ScrollUp(this ScrollViewer scrollViewer, double offset)
        {
            scrollViewer.ScrollByVerticalOffset(-offset);
        }

        public static void ScrollDown(this ScrollViewer scrollViewer, double offset)
        {
            scrollViewer.ScrollByVerticalOffset(offset);
        }

        public static void ScrollByVerticalOffset(this ScrollViewer scrollViewer, double offset)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
        }

        public static MemberReference GetMemberByToken(this TypeDefinition type, int memberToken)
        {
            if (type.HasProperties)
            {
                foreach (PropertyDefinition propertyDefinition in type.Properties)
                {
                    if (propertyDefinition.MetadataToken.ToInt32() == memberToken)
                    {
                        return propertyDefinition;
                    }
                    if (propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.MetadataToken.ToInt32() == memberToken)
                    {
                        return propertyDefinition;
                    }
                    if (propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.MetadataToken.ToInt32() == memberToken)
                    {
                        return propertyDefinition;
                    }
                }
            }
            if (type.HasEvents)
            {
                foreach (EventDefinition eventDefinition in type.Events)
                {
                    if (eventDefinition.MetadataToken.ToInt32() == memberToken)
                    {
                        return eventDefinition;
                    }
                    if (eventDefinition.AddMethod != null && eventDefinition.AddMethod.MetadataToken.ToInt32() == memberToken)
                    {
                        return eventDefinition;
                    }
                    if (eventDefinition.RemoveMethod != null && eventDefinition.RemoveMethod.MetadataToken.ToInt32() == memberToken)
                    {
                        return eventDefinition;
                    }
                    if (eventDefinition.InvokeMethod != null && eventDefinition.InvokeMethod.MetadataToken.ToInt32() == memberToken)
                    {
                        return eventDefinition;
                    }
                }
            }
            if (type.HasMethods)
            {
                foreach (MethodDefinition methodDefinition in type.Methods)
                {
                    if (methodDefinition.MetadataToken.ToInt32() == memberToken)
                    {
                        return methodDefinition;
                    }
                }
            }
            if (type.HasFields)
            {
                foreach (FieldDefinition fieldDefinition in type.Fields)
                {
                    if (fieldDefinition.MetadataToken.ToInt32() == memberToken)
                    {
                        return fieldDefinition;
                    }
                }
            }
            return null;
        }
    }
}
