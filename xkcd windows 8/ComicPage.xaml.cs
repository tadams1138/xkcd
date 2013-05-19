using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Search;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Printing;
using xkcd.DataModel;
using xkcd_windows_8.Common;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace xkcd_windows_8
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ComicPage
    {
        private Comic _currentComic;

        public ComicPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var comic = ComicDataSource.GetItem((int)navigationParameter);
            DefaultViewModel["Items"] = ComicDataSource.AllItems;
            FlipView.SelectedItem = comic;
            RegisterForPrinting();
            SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (Comic)FlipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.Number;
            UnregisterForPrinting();
            SearchPane.GetForCurrentView().ShowOnKeyboardInput = false;
        }

        private void WebsiteOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (Comic)FlipView.SelectedItem;
                Windows.System.Launcher.LaunchUriAsync(selectedItem.Uri);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.ToString());
                dialog.ShowAsync();
            }
        }

        private void ExplainationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (Comic)FlipView.SelectedItem;
                Windows.System.Launcher.LaunchUriAsync(selectedItem.ExplanationUri);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.ToString());
                dialog.ShowAsync();
            }
        }

        private async void ImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentComic = (Comic)FlipView.SelectedItem;
            var dialog = new MessageDialog(currentComic.AltText);
            await dialog.ShowAsync();
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = FlipView.SelectedItem as Comic;
            _currentComic = selectedItem;
            PreparetPrintContent();
        }

        #region Printing methods
        /// <summary>
        /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
        /// </summary>
        private const double ApplicationContentMarginLeft = 0.075;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        private const double ApplicationContentMarginTop = 0.03;

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing. 
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        private PrintDocument _printDocument;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        private IPrintDocumentSource _printDocumentSource;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<UIElement> PrintPreviewPages = new List<UIElement>();

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        private PrintPage _printPage;

        /// <summary>
        /// Factory method for every scenario that will create/generate print content specific to each scenario
        /// For scenarios 1-5: it will create the first page from which content will flow
        /// Scenario 6 uses a different approach
        /// </summary>
        private void PreparetPrintContent()
        {
            _printPage = new PrintPage(_currentComic);

            // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Clear();
            PrintingRoot.Children.Add(_printPage);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            string title = "xkcd";
            if (_currentComic != null)
            {
                title += " " + _currentComic.Number + " - " + _currentComic;
            }

            printTask = e.Request.CreatePrintTask(title, sourceRequested =>
                {
                    // Print Task event handler is invoked when the print job is completed.
                    printTask.Completed += async (s, args) =>
                        {
                            // Notify the user when the print operation fails.
                            if (args.Completion == PrintTaskCompletion.Failed)
                            {
                                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                    {
                                        var dialog = new MessageDialog("Failed to print.");
                                        await dialog.ShowAsync();
                                    });
                            }
                        };

                    sourceRequested.SetSource(_printDocumentSource);
                });
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        private void RegisterForPrinting()
        {
            // Create the PrintDocument.
            _printDocument = new PrintDocument();

            // Save the DocumentSource.
            _printDocumentSource = _printDocument.DocumentSource;

            // Add an event handler which creates preview pages.
            _printDocument.Paginate += CreatePrintPreviewPages;

            // Add an event handler which provides a specified preview page.
            _printDocument.GetPreviewPage += GetPrintPreviewPage;

            // Add an event handler which provides all final print pages.
            _printDocument.AddPages += AddPrintPages;

            // Create a PrintManager and add a handler for printing initialization.
            var printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        private void UnregisterForPrinting()
        {
            if (_printDocument == null)
                return;

            _printDocument.Paginate -= CreatePrintPreviewPages;
            _printDocument.GetPreviewPage -= GetPrintPreviewPage;
            _printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;

            PrintingRoot.Children.Clear();
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            // Clear the cache of preview pages 
            PrintPreviewPages.Clear();

            // Clear the printing root of preview pages
            PrintingRoot.Children.Clear();

            // Get the PrintTaskOptions
            var printingOptions = e.PrintTaskOptions;

            // Get the page description to deterimine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            // We know there is at least one page to be printed. passing null as the first parameter to
            // AddOnePrintPreviewPage tells the function to add the first page.
            AddOnePrintPreviewPage(pageDescription);

            var printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            printDoc.SetPreviewPageCount(PrintPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPage(e.PageNumber, PrintPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < PrintPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                _printDocument.AddPage(PrintPreviewPages[i]);
            }

            var printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="printPageDescription">Printer's page description</param>
        private void AddOnePrintPreviewPage(PrintPageDescription printPageDescription)
        {
            // Set "paper" width
            _printPage.Width = printPageDescription.PageSize.Width;
            _printPage.Height = printPageDescription.PageSize.Height;

            var printableArea = (Grid)_printPage.Content;

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(
                printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            double marginHeight =
                Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                         printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = _printPage.Width - marginWidth;
            printableArea.Height = _printPage.Height - marginHeight;

            // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.            
            PrintingRoot.Children.Add(_printPage);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();

            // Add the page to the page preview collection
            PrintPreviewPages.Add(_printPage);
        }
        #endregion
    }
}
