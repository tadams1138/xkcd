namespace xkcd
{
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

    public sealed partial class YearsPage : xkcd.Common.LayoutAwarePage
    {
        private ObservableCollection<int> years = new ObservableCollection<int>();
        bool handleDataEvent;

        public YearsPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (!handleDataEvent)
            {
                ComicDataSource.CollectionChanged += this.ComicDataSource_CollectionChanged;
                this.handleDataEvent = true;
            }

            RefreshYearsCollection();

            this.DefaultViewModel["Years"] = years;
        }

        private void RefreshYearsCollection()
        {
            var sampleDataGroups = ComicDataSource.GetYears();
            this.years.Clear();
            foreach (int year in sampleDataGroups)
            {
                this.years.Add(year);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshYearsCollection();
        }       
                
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int year = (int)e.ClickedItem;
            this.Frame.Navigate(typeof(MonthsPage), year);
        }
    }
}
