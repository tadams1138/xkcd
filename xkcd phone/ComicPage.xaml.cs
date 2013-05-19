using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using xkcd.DataModel;

namespace xkcd_phone
{
    public partial class ComicPage
    {
        public ComicPage()
        {
            InitializeComponent();

            DataContext = ComicDataSource.AllItems;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string number;
            if (NavigationContext.QueryString.TryGetValue("number", out number))
            {
                int num = Int32.Parse(number);
                var selected = ComicDataSource.GetComic(num);
                SlideView.SelectedItem = selected;
            }
        }

        private void PanAndZoomImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedComic = (Comic)SlideView.SelectedItem;
            MessageBox.Show(selectedComic.AltText);
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var selected = (Comic)SlideView.SelectedItem;
            var webBrowserTask = new WebBrowserTask { Uri = selected.Uri };
            webBrowserTask.Show();
        }
    }
}