using System;
using System.Collections.Generic;

namespace CommonsLib
{
    public class FocusChangedEventArgs : EventArgs
    {
        public Guid PageId { get; private set; }
        public IDictionary<string, object> FocusedElementData { get; private set; }

        public FocusChangedEventArgs(Guid pageId, IDictionary<string, object> focusedElementData)
        {
            PageId = pageId;
            FocusedElementData = focusedElementData;
        }
    }
}