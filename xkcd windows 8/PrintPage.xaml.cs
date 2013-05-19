using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using xkcd.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace xkcd_windows_8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintPage
    {
        public PrintPage()
        {
            InitializeComponent();
        }

        public PrintPage(Comic comic)
        {
            InitializeComponent();

            Title.Text = comic.ToString();
            Subtitle.Text = comic.Subtitle;
            AltText.Text = comic.AltText;
            Image.Source = new BitmapImage(comic.ImageUri);

            SizeChanged += (sender, args) =>
                {
                    const int combinedHeaderAndFooterHeight = 350;
                    double availableHeight = (sender as Page).Height - combinedHeaderAndFooterHeight;
                    availableHeight = double.IsNaN(availableHeight) ? 1 : availableHeight;
                    double maxHeight = Math.Max(availableHeight, 1);
                    ImageViewbox.MaxHeight = maxHeight;
                };
        }
    }
}
