using System.Windows;
using System.Windows.Media;

namespace WPF.Connectors
{ 
    public static class Helper
    {
        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            while (element != null && !(element is T))
                element = VisualTreeHelper.GetParent(element);

            return (T)element;
        }

        public static Point Centre(this Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }
    }

    public static class EnumHelper
    {
        public static IEnumerable<T> SeparateFlags<T>(this T value) where T : Enum
        {
            return from enm in Enum.GetValues(typeof(T)).Cast<T>()
                   where value.IsFlagSet(enm)
                   select enm;
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : Enum
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }
    }
}
