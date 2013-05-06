using Microsoft.Phone.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class MonthsPage
    {
        private readonly ObservableCollection<DateTime> _months = new ObservableCollection<DateTime>();
        private int _year;
        bool _handleDataEvent;

        // Constructor
        public MonthsPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = _months;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string year;
            if (NavigationContext.QueryString.TryGetValue("year", out year))
            {
                _year = int.Parse(year);
                RefreshCollection();
                
                if (!_handleDataEvent)
                {
                    ComicDataSource.CollectionChanged += ComicDataSource_CollectionChanged;
                    _handleDataEvent = true;
                }
            }
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetMonths(_year);
            _months.Clear();
            foreach (DateTime month in newCollection)
            {
                _months.Add(month);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshCollection();
        }

        // Handle selection changed on LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DaysPage.xaml?month=" + ((DateTime)MainLongListSelector.SelectedItem).ToString(), UriKind.Relative));

            // Reset selected item to null (no selection)
            MainLongListSelector.SelectedItem = null;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}