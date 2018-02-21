namespace Cef
{
    public class CloseTabCommandFactory
    {
        private readonly BrowserTabSelectorViewModel _browserTabSelector;

        public CloseTabCommandFactory(BrowserTabSelectorViewModel browserTabSelector)
        {
            _browserTabSelector = browserTabSelector;
        }

        public CloseTabCommand Create(BrowserTabViewModel currentTab)
        {
            return new CloseTabCommand(_browserTabSelector, currentTab);
        }
    }
}