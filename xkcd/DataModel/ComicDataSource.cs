using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace xkcd.DataModel
{
    public sealed class ComicDataSource
    {
        private const string CurrentComicUrl = "http://xkcd.com/info.0.json";
        private const string ComicUrlFormat = "http://xkcd.com/{0}/info.0.json";
        private const string ComicsDataFile = "comics.xml";
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        public static event NotifyCollectionChangedEventHandler CollectionChanged;
        
        private static ComicDataSource comicDataSource = new ComicDataSource();

        private Collection<Comic> allItems = new Collection<Comic>();

        public static Collection<Comic> AllItems
        {
            get { return comicDataSource.allItems; }
        }

        public static Comic GetItem(int number)
        {
            // Simple linear search is acceptable for small data sets
            var matches = AllItems.FirstOrDefault(item => item.Number.Equals(number));
            return matches;
        }

        private ComicDataSource()
        {
            InitializeRealData();
        }

        public async void InitializeRealData()
        {
            await LoadComicsDataFile();
            await UpdateComicData();
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private async Task LoadComicsDataFile()
        {
            StorageFile inFile = null;
            bool fileExists = true;
            try
            {
                inFile = await storageFolder.GetFileAsync(ComicsDataFile);
            }
            catch (FileNotFoundException)
            {
                fileExists = false;
            }

            if (!fileExists)
            {
                inFile = await Package.Current.InstalledLocation.GetFileAsync(ComicsDataFile);
            }

            await DeserializeDataFile(inFile);
        }

        private async Task DeserializeDataFile(StorageFile inFile)
        {
            var stream = await inFile.OpenStreamForReadAsync();
            var xmlReader = XmlReader.Create(stream);
            DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<Comic>));
            this.allItems = (ObservableCollection<Comic>)ser.ReadObject(xmlReader, true);
        }

        private async Task UpdateComicData()
        {
            using (HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                int latestComicNumberInDataSource = this.GetLatestComicNumber();
                Comic latestComic = await this.GetComicFromWeb(httpClient, CurrentComicUrl);

                if (latestComic.Number > latestComicNumberInDataSource)
                {
                    await this.AddLatestComicsToDataSource(latestComicNumberInDataSource, latestComic, httpClient);
                    await this.SaveComicData();
                }
            }
        }

        private async Task AddLatestComicsToDataSource(int latestComicNumberInDataSource, Comic latestComic, HttpClient httpClient)
        {
            for (int i = latestComicNumberInDataSource + 1; i < latestComic.Number; i++)
            {
                try
                {
                    string comicUrl = string.Format(ComicUrlFormat, i);
                    Comic comic = await GetComicFromWeb(httpClient, comicUrl);
                    AllItems.Add(comic);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Failed to add comic number {0} to data source: {1}", i, ex.ToString()));
                }
            }

            AllItems.Add(latestComic);
        }

        private async Task<Comic> GetComicFromWeb(HttpClient httpClient, string comicUrl)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(comicUrl))
            {
                string comicJson = await response.Content.ReadAsStringAsync();
                return Comic.FromJson(comicJson);
            }
        }

        private async Task SaveComicData()
        {
            var dataContractSerializer = new DataContractSerializer(AllItems.GetType());
            using (var memoryStream = new MemoryStream())
            {
                dataContractSerializer.WriteObject(memoryStream, AllItems);
                memoryStream.Seek(0, SeekOrigin.Begin);
                string serialized = new StreamReader(memoryStream).ReadToEnd();
                var doc = new XmlDocument();
                doc.LoadXml(serialized);
                var outFile = await this.storageFolder.CreateFileAsync(ComicsDataFile, CreationCollisionOption.ReplaceExisting);
                await doc.SaveToFileAsync(outFile);
            }
        }

        internal static IEnumerable<int> GetYears()
        {
            if (comicDataSource != null)
            {
                return from p in AllItems
                       group p by new { p.Date.Year } into d
                       select d.Key.Year;
            }
            else
            {
                return null;
            }
        }

        internal static IEnumerable<DateTime> GetMonths(int year)
        {
            if (comicDataSource != null)
            {
                return from p in AllItems
                       where p.Date.Year == year
                       group p by new { p.Date.Month } into d
                       select new DateTime(year, d.Key.Month, 1);
            }
            else
            {
                return null;
            }
        }

        internal static IEnumerable<Comic> GetComics(int year, int month)
        {
            if (comicDataSource != null)
            {
                return from p in AllItems
                       where p.Date.Year == year && p.Date.Month == month
                       orderby p.Date
                       select p;
            }
            else
            {
                return null;
            }
        }

        private int GetLatestComicNumber()
        {
            return this.allItems.Max(p => p.Number);
        }        
    }
}
