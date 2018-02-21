using System;

namespace CommonsLib
{
    public class MutationEventArgs : EventArgs
    {
        public Guid PageId { get; private set; }

        public MutationEventArgs(Guid pageId)
        {
            PageId = pageId;
        }
    }
}