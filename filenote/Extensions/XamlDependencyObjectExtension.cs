using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Sbs20.Filenote.Extensions
{
    public static class XamlDependencyObjectExtension
    {
        // Inspired by: http://blog.jerrynixon.com/2012/09/how-to-access-named-control-inside-xaml.html
        public static IEnumerable<FrameworkElement> AllChildren(this DependencyObject parent)
        {
            var list = new List<FrameworkElement>();
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is FrameworkElement)
                {
                    list.Add(child as FrameworkElement);
                }

                list.AddRange(child.AllChildren());
            }

            return list;
        }
    }
}
