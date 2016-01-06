using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Sbs20.Filenotes.Extensions
{
    public static class XamlDependencyObjectExtension
    {
        // Inspired by: http://blog.jerrynixon.com/2012/09/how-to-access-named-control-inside-xaml.html
        public static IEnumerable<DependencyObject> AllChildren(this DependencyObject parent)
        {
            var list = new List<DependencyObject>();
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                list.Add(child);

                list.AddRange(child.AllChildren());
            }

            return list;
        }

        public static IEnumerable<DependencyObject> AllAncestry(this DependencyObject child)
        {
            var list = new List<DependencyObject>();
            for (var parent = VisualTreeHelper.GetParent(child); parent != null; parent = VisualTreeHelper.GetParent(parent))
            {
                //VisualTreeHelper.GetParent(
                list.Add(parent);
            }

            return list;
        }

        public static bool IsVisible(this DependencyObject child)
        {
            var ancestors = child.AllAncestry().OfType<FrameworkElement>();
            foreach (var ancestor in ancestors)
            {
                if (ancestor.Visibility == Visibility.Collapsed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
