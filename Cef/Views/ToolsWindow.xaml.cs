using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cef.Views
{
    /// <summary>
    /// Interaction logic for ToolsWindow.xaml
    /// </summary>
    public partial class ToolsWindow : Window
    {
        public ToolsWindow()
        {
            InitializeComponent();
            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            var collectionChanged = _listBox.ItemsSource as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged += CollectionChangedOnCollectionChanged;
            }
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            var collectionChanged = _listBox.ItemsSource as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged -= CollectionChangedOnCollectionChanged;
            }
        }

        private void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var collection = _listBox.ItemsSource as ICollection<string>;
            var elementAt = collection.ElementAt(collection.Count - 1);
            _listBox.ScrollIntoView(elementAt);
        }
    }
}
