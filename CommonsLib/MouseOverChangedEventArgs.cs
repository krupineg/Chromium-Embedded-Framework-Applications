using System;
using System.Collections.Generic;

namespace CommonsLib
{
    public class MouseOverChangedEventArgs : EventArgs
    {
        public IDictionary<string, object> ChangedData { get; private set; }

        public Guid PageId { get; private set; }

        public MouseOverChangedEventArgs(Guid pageId, IDictionary<string, object> focusedElementData)
        {
            PageId = pageId;
            ChangedData = focusedElementData;
        }
    }
}