using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Event;
using CefSharp.Handler;
using CommonsLib;
using Path = System.Windows.Shapes.Path;

namespace Cef
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new MainViewModel("http://google.com", Dispatcher);
            //DataContext = new MainViewModel("https://github.com/cefsharp/CefSharp/blob/cefsharp/63/CefSharp.Wpf.Example/Controls/ChromiumWebBrowserWithScreenshotSupport.cs");
        }        
       
    }
}
