using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private readonly ObservableCollection<Comic> _comics = new ObservableCollection<Comic>();
        private string _queryText;

        public SearchPage()
        {
            InitializeComponent();

            // Set the data context of the LongListSelector control to the sample data
            DataContext = _comics;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshCollection();
        }

        private void RefreshCollection()
        {
            var newCollection = ComicDataSource.GetComics(_queryText);
            _comics.Clear();
            foreach (Comic c in newCollection)
            {
                _comics.Add(c);
            }
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

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            var brush1 = new SolidColorBrush {Color = Colors.Black};
            SearchTextBox.Foreground = brush1;
        }

        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _queryText = SearchTextBox.Text;
            RefreshCollection();
        }
    }
}