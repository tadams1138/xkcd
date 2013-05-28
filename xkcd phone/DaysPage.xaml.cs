using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class DaysPage
    {
        private readonly Collection<Comic> _comics = new Collection<Comic>();
        private DateTime _month;

        public DaysPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = _comics;
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string month;
            if (NavigationContext.QueryString.TryGetValue("month", out month))
            {
                _month = DateTime.Parse(month);
                PageTitle.Text = _month.ToString("MMMM yyyy");

                RefreshCollection();
            }
        }
        
        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetComics(_month.Year, _month.Month);
            _comics.Clear();
            foreach (Comic c in newCollection)
            {
                _comics.Add(c);
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
            NavigationService.Navigate(new Uri("/ComicPage.xaml?number=" + ((Comic)MainLongListSelector.SelectedItem)._num.ToString(), UriKind.Relative));

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