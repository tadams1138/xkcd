﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace xkcd.DataModel
{
    public sealed class ComicDataSource
    {
        private const string CurrentComicUrl = "http://xkcd.com/info.0.json";
        private const string ComicUrlFormat = "http://xkcd.com/{0}/info.0.json";

        public static event NotifyCollectionChangedEventHandler CollectionChanged;

        private static readonly ComicDataSource _comicDataSource = new ComicDataSource();

        private readonly Collection<Comic> _allItems = new Collection<Comic>();

        public static Collection<Comic> AllItems
        {
            get { return _comicDataSource._allItems; }
        }

        public static Comic GetItem(int number)
        {
            var matches = AllItems.FirstOrDefault(item => item.Number.Equals(number));
            return matches;
        }

        public static IEnumerable<int> GetYears()
        {
            return from p in AllItems
                   group p by new { p.Date.Year } into d
                   select d.Key.Year;
        }

        public static IEnumerable<DateTime> GetMonths(int year)
        {
            return from p in AllItems
                   where p.Date.Year == year
                   group p by new { p.Date.Month } into d
                   select new DateTime(year, d.Key.Month, 1);
        }

        internal static IEnumerable<Comic> GetComics(int year, int month)
        {
            return from p in AllItems
                   where p.Date.Year == year && p.Date.Month == month
                   orderby p.Date
                   select p;
        }

        public static async Task UpdateComicData(Stream comicsStream)
        {
            LoadDataFromFile(comicsStream);

            int latestComicNumberInDataSource = _comicDataSource.GetLatestComicNumber();
            Comic latestComic = await GetComicFromWeb(CurrentComicUrl);

            if (latestComic.Number > latestComicNumberInDataSource)
            {
                await AddLatestComicsToDataSource(latestComicNumberInDataSource, latestComic);
            }
        }

        private static void LoadDataFromFile(Stream stream)
        {
            ObservableCollection<Comic> comics;
            using (var xmlReader = XmlReader.Create(stream))
            {
                var ser = new DataContractSerializer(typeof (ObservableCollection<Comic>));
                comics = (ObservableCollection<Comic>)ser.ReadObject(xmlReader, true);
            }

            AllItems.Clear();
            foreach (var comic in comics)
            {
                AllItems.Add(comic);
            }
        }

        public static void SaveComicData(Stream stream)
        {
            var ser = new DataContractSerializer(AllItems.GetType());
            using (var xmlWriter = XmlWriter.Create(stream))
            {
                ser.WriteObject(xmlWriter, AllItems);
            }
        }

        private static async Task AddLatestComicsToDataSource(int latestComicNumberInDataSource, Comic latestComic)
        {
            for (int i = latestComicNumberInDataSource + 1; i < latestComic.Number; i++)
            {
                try
                {
                    string comicUrl = string.Format(ComicUrlFormat, i);
                    Comic comic = await GetComicFromWeb(comicUrl);
                    AllItems.Add(comic);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to add comic number {0} to data source: {1}", i, ex);
                }
            }

            AllItems.Add(latestComic);
        }

        private static Task<Comic> GetComicFromWeb(string comicUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(comicUrl);
            Task<WebResponse> requestTask = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                null);
            return requestTask.ContinueWith(t => ConvertWebResponseToComic(t.Result));
        }

        private static Comic ConvertWebResponseToComic(WebResponse result)
        {
            using (var responseStream = result.GetResponseStream())
            {
                var reader = new StreamReader(responseStream);
                string comicJson = reader.ReadToEnd();
                return Comic.FromJson(comicJson);
            }
        }

        private int GetLatestComicNumber()
        {
            return _allItems.Any() ? _allItems.Max(p => p.Number) : 0;
        }
    }
}
