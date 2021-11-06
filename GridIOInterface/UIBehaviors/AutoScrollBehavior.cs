using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GridIOInterface {
    class AutoScrollBehavior : DependencyObject {
        public static readonly DependencyProperty autoScrollProperty = DependencyProperty.RegisterAttached(
                "AutoScroll",
                typeof(bool),
                typeof(ScrollViewer),
                new PropertyMetadata(false)
            );

        public static void AutoScroller_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.ExtentHeightChange != 0) {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }

        public static void SetAutoScroll(UIElement element, bool value) {
            ScrollViewer scrollViewer = (ScrollViewer)element;
            if (value) {
                scrollViewer.ScrollChanged += AutoScroller_ScrollChanged;
            }
            else {
                scrollViewer.ScrollChanged -= AutoScroller_ScrollChanged;
            }
            element.SetValue(autoScrollProperty, value);
        }

        public static bool GetAutoScroll(UIElement element) {
            return (bool)element.GetValue(autoScrollProperty);
        }
    }
}
