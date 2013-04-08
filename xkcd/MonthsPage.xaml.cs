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
    public sealed partial class MonthsPage : xkcd.Common.LayoutAwarePage
    {
        private ObservableCollection<DateTime> months = new ObservableCollection<DateTime>();
        private int year;
        bool handleDataEvent;

        public MonthsPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.year = (int)navigationParameter;

            if (!handleDataEvent)
            {
                ComicDataSource.CollectionChanged += this.ComicDataSource_CollectionChanged;
                this.handleDataEvent = true;
            }

            RefreshCollection();

            this.DefaultViewModel["Months"] = months;
            this.DefaultViewModel["Year"] = this.year;
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetMonths(this.year);
            this.months.Clear();
            foreach (DateTime month in newCollection)
            {
                this.months.Add(month);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var month = (DateTime)e.ClickedItem;
            this.Frame.Navigate(typeof(ComicsPage), month.ToString());
        }
    }
}
