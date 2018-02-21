using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CefSharp;
using CommonsLib;

namespace Cef
{
    public class TabHeaderViewModel : ViewModel
    {
        private string _title, _favIcon;
        
        public CloseTabCommand CloseCommand { get; private set; }
        
        public string FavIcon
        {
            get { return _favIcon; }
            set { SetProperty(ref _favIcon, value); }
        }

        public Guid Id { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SetProperty(ref _title, value);
                }
            }
        }

        public TabHeaderViewModel(CloseTabCommand closeCommand, Guid tabGuid)
        {
            Id = tabGuid;
            CloseCommand = closeCommand;
        }

        public void SetBrowser(IWebBrowser browser)
        {
            Title = browser.Address;
        }

        protected override void DisposeInternal()
        {
            CloseCommand.Dispose();
        }
    }
}