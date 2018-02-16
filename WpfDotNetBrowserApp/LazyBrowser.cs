using System;
using System.Windows;
using System.Windows.Controls;
using DotNetBrowser;
using DotNetBrowser.WPF;

namespace WpfDotNetBrowserApp
{
    public class LazyBrowser : UserControl
    {
        public static readonly DependencyProperty BrowserProperty = DependencyProperty.Register(
            "Browser", typeof(Browser), typeof(LazyBrowser), new PropertyMetadata(default(Browser), PropertyChangedCallback));
        
        public Browser Browser
        {
            get { return (Browser)GetValue(BrowserProperty); }
            set { SetValue(BrowserProperty, value); }
        }

        public LazyBrowser()
        {
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Unloaded -= OnUnloaded;
            var disposable = Content as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var browser = dependencyPropertyChangedEventArgs.NewValue as Browser;
            var lazyBrowser = dependencyObject as LazyBrowser;
            if (browser != null)
            {
                lazyBrowser.Content = new WPFBrowserView(browser);
            }
        }
    }
}