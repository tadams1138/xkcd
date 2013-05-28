using Microsoft.Phone.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class YearsPage : PhoneApplicationPage
    {
        private readonly Collection<int> _years = new Collection<int>();

        // Constructor
        public YearsPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = _years;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshYearsCollection();
        }

        private void RefreshYearsCollection()
        {
            var sampleDataGroups = ComicDataSource.GetYears();
            _years.Clear();
            foreach (int year in sampleDataGroups)
            {
                _years.Add(year);
            }
        }

        private void ComicDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshYearsCollection();
        }

        // Handle selection changed on LongListSelector
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MainLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/MonthsPage.xaml?year=" + (int)MainLongListSelector.SelectedItem, UriKind.Relative));

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