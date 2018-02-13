using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CefSharp;
using CefSharp.Internals;
using CefSharp.Wpf;
using CommonsLib;

namespace Cef
{
    public class ChromiumWebBrowserWithScreenshotSupport : ChromiumWebBrowser
    {
        private CancellationTokenSource cancellationTokenSource;
        private volatile bool isTakingScreenshot = false;
        private Size? screenshotSize;
        private int oldFrameRate;
        private TaskCompletionSource<InteropBitmap> screenshotTaskCompletionSource;
        private object screenshotLock = new object();

        public Task<InteropBitmap> TakeScreenshot(Size screenshotSize, CancellationToken token, int? frameRate = 1, int? ignoreFrames = 0, TimeSpan? timeout = null)
        {
            lock (screenshotLock)
            {
                token.ThrowIfCancellationRequested();
                if (screenshotTaskCompletionSource != null &&
                    screenshotTaskCompletionSource.Task.Status == TaskStatus.Running)
                {
                    throw new Exception(
                        "Screenshot already in progress, you must wait for the previous screenshot to complete");
                }

                if (IsBrowserInitialized == false)
                {
                    throw new Exception("Browser has not yet finished initializing or is being disposed");
                }

                if (IsLoading)
                {
                    throw new Exception("Unable to take screenshot while browser is loading");
                }

                var browserHost = this.GetBrowser().GetHost();

                if (browserHost == null)
                {
                    throw new Exception("IBrowserHost is null");
                }
                token.ThrowIfCancellationRequested();
                screenshotTaskCompletionSource = new TaskCompletionSource<InteropBitmap>();

                if (timeout.HasValue)
                {
                    screenshotTaskCompletionSource = screenshotTaskCompletionSource.WithTimeout(timeout.Value);
                }

                if (frameRate.HasValue)
                {
                    oldFrameRate = browserHost.WindowlessFrameRate;
                    browserHost.WindowlessFrameRate = frameRate.Value;
                }

                token.ThrowIfCancellationRequested();
                this.screenshotSize = screenshotSize;
                this.isTakingScreenshot = true;
                //Resize the browser using the desired screenshot dimensions
                //The resulting bitmap will never be rendered to the screen
                browserHost.WasResized();
                
                return screenshotTaskCompletionSource.Task;
                
                   
            }
        }

        protected override ViewRect GetViewRect()
        {
            lock (screenshotLock)
            {
                if (isTakingScreenshot)
                {
                    return new ViewRect((int)Math.Ceiling(screenshotSize.Value.Width), (int)Math.Ceiling(screenshotSize.Value.Height));
                }

                return base.GetViewRect();
            }
            
        }

        protected override void OnPaint(BitmapInfo bitmapInfo)
        {
            lock (screenshotLock)
            {
                if (isTakingScreenshot)
                {
                    //Wait until we have a frame that matches the updated size we requested
                    //if (screenshotSize.HasValue && screenshotSize.Value.Width == bitmapInfo.Width && screenshotSize.Value.Height == bitmapInfo.Height)
                    {
                        //Bitmaps need to be created on the UI thread
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var stride = bitmapInfo.Width * bitmapInfo.BytesPerPixel;

                            lock (bitmapInfo.BitmapLock)
                            {
                                try
                                {
                                    //NOTE: Interopbitmap is not capable of supporting DPI scaling
                                    var bitmap = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(
                                        bitmapInfo.FileMappingHandle,
                                        bitmapInfo.Width, bitmapInfo.Height, PixelFormats.Bgra32, stride, 0);
                                    //Using TaskExtensions.TrySetResultAsync extension method so continuation runs on Threadpool
                                    screenshotTaskCompletionSource.TrySetResultAsync(bitmap);

                                    isTakingScreenshot = false;
                                    var browserHost = GetBrowser().GetHost();
                                    //Return the framerate to the previous value
                                    browserHost.WindowlessFrameRate = oldFrameRate;
                                    //Let the browser know the size changes so normal rendering can continue
                                    browserHost.WasResized();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                            }
                        }));
                    }
                }
                else
                {
                    base.OnPaint(bitmapInfo);
                }
            }
        }

        public void TakeScreenshot(string filename)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            //var loading = true;
            //while (loading)
            //{
            //    Dispatcher.Invoke(() => loading = IsLoading);
            //    Console.WriteLine("loading: " + loading);
            //    Task.Delay(20);
            //}

            cancellationTokenSource = new CancellationTokenSource();

            var uiThreadTaskScheduler = Dispatcher.ToTaskSchedulerAsync();
            
            this.EvaluateScriptAsync(JavascriptQueries.RequestForMaxSize).ContinueWith((scriptTask) =>
            {
                var javascriptResponse = scriptTask.Result;

                if (javascriptResponse.Success)
                {
                    var widthAndHeight = (List<object>)javascriptResponse.Result;

                    var screenshotSize = new Size((int)widthAndHeight[0], (int)widthAndHeight[1]);
                    Console.WriteLine("scroll size: " + screenshotSize) ;
                    TakeScreenshot(screenshotSize, ignoreFrames: 0, frameRate:1, token: cancellationTokenSource.Token).ContinueWith((screenshotTask) =>
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (screenshotTask.Status == TaskStatus.RanToCompletion)
                        {
                            try
                            {
                                var bitmap = screenshotTask.Result;
                                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                using (var stream = new FileStream(filename, FileMode.Create))
                                {
                                    var encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    encoder.Save(stream);
                                }
                                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = filename
                                });
                            }
                            catch (Exception ex)
                            {
                                var msg = ex.ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unable to capture screenshot");
                        }
                    }, uiThreadTaskScheduler); //Make sure continuation runs on UI thread

                }
                else
                {
                    MessageBox.Show("Unable to obtain size of screenshot");
                }
            }, uiThreadTaskScheduler);
        }
    }

    
}