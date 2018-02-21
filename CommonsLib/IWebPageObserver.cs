using System;

namespace CommonsLib
{
    public interface IWebPageObserver
    {
        event EventHandler<MouseOverChangedEventArgs> MouseOverChanged;
        event EventHandler<FocusChangedEventArgs> FocusChanged;
        event EventHandler<MutationEventArgs> Mutated;
    }
}