using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Cef.Views;
using CefSharp.Wpf;

namespace Cef
{
    public class AutoScrollingListBoxBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty IndexToScrollProperty = DependencyProperty.Register(
            "IndexToScroll", typeof(int), typeof(AutoScrollingListBoxBehavior), new PropertyMetadata(default(int)));

        public int IndexToScroll
        {
            get { return (int)GetValue(IndexToScrollProperty); }
            set
            {
                SetValue(IndexToScrollProperty, value);
                AssociatedObject.ScrollIntoView(value);
            }
        }
    }

    public class MoveFocusByEnterKeyBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                base.OnAttached();
                AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
                base.OnDetaching();
            }
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                if (e.Key == Key.Return && e.Key == Key.Enter)
                {
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }

    public class BindingBehavior : Behavior<ChromiumWebBrowser>
    {
        public static readonly DependencyProperty IsLoadingBindableProperty = DependencyProperty.Register(
            "IsLoadingBindable", typeof(bool), typeof(BindingBehavior), new PropertyMetadata(default(bool)));

        public bool IsLoadingBindable
        {
            get { return (bool)GetValue(IsLoadingBindableProperty); }
            set { SetValue(IsLoadingBindableProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingBinderProperty = DependencyProperty.Register(
            "IsLoadingBinder", typeof(bool), typeof(BindingBehavior), new PropertyMetadata(default(bool), PropertyChangedCallback));
        
        public bool IsLoadingBinder
        {
            get { return (bool)GetValue(IsLoadingBinderProperty); }
            set { SetValue(IsLoadingBinderProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            dependencyObject.SetValue(IsLoadingBindableProperty, dependencyPropertyChangedEventArgs.NewValue);
        }
    }
}