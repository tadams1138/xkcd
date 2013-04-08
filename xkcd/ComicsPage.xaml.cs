using xkcd.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace xkcd
{
    public sealed partial class ComicsPage : xkcd.Common.LayoutAwarePage
    {
        private ObservableCollection<Comic> comics = new ObservableCollection<Comic>();
        private DateTime month;
        bool handleDataEvent;

        public ComicsPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.month = DateTime.Parse((string)navigationParameter);

            if (!handleDataEvent)
            {
                ComicDataSource.CollectionChanged += this.ComicDataSource_CollectionChanged;
                this.handleDataEvent = true;
            }

            RefreshCollection();

            this.DefaultViewModel["Comics"] = comics;
            this.DefaultViewModel["Month"] = this.month.ToString("MMMM yyyy");
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetComics(this.month.Year, this.month.Month);
            this.comics.Clear();
            foreach (Comic c in newCollection)
            {
                this.comics.Add(c);
            }
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var comic = (Comic)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage), comic.Number);
        }
    }
}
