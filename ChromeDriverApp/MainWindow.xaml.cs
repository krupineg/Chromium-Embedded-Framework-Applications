using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonsLib;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace ChromeDriverApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChromeDriver driver;

        public MainWindow()
        {
            InitializeComponent();
            Content = new Label() { Content = "Please wait" };
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await Task.Run(() =>
            {
                driver = new ChromeDriver(new ChromeOptions());
                driver.Url = "http://www.seleniumhq.org/";
                WaitPage(driver);
                
                var screenshot = driver.GetScreenshot();
                screenshot.SaveAsFile("c:\\temp\\chromedriver.png", ImageFormat.Png);
            }).ContinueWith(task =>
            {
                Content = new Label() { Content = "Screenshot is saved to: c:\\temp\\chromedriver.png" };
            }, CancellationToken.None, TaskContinuationOptions.None, Dispatcher.ToTaskScheduler());
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            driver.Dispose();
        }

        private void WaitPage(ChromeDriver driver)
        {
            bool ready = false;
            while (IsLoading(driver))
            {
            }
        }

        private bool IsLoading(ChromeDriver driver)
        {
            var readyState = driver.ExecuteScript("return document.readyState").ToString();
            return readyState != "complete";
        }
    }
}
