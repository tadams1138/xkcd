namespace xkcd.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization;
    using Windows.Data.Json;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    [DataContract]
    [Windows.Foundation.Metadata.WebHostHidden]
    public class Comic : xkcd.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public Comic(int number, string title, DateTime date, string imagePath, string altText)
        {
            this._number = number;
            this._title = title;
            this._date = date;
            this._altText = altText;
            this._imagePath = imagePath;
        }

        private int _number = 0;

        [DataMember]
        public int Number
        {
            get { return this._number; }
            set { this.SetProperty(ref this._number, value); }
        }

        private string _title = string.Empty;

        [DataMember]
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private DateTime _date = DateTime.MinValue;

        [DataMember]
        public DateTime Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        private string _altText = string.Empty;

        [DataMember]
        public string AltText
        {
            get { return this._altText; }
            set { this.SetProperty(ref this._altText, value); }
        }

        private ImageSource _image = null;

        [DataMember]
        private String _imagePath = null;

        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(Comic._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public string ShortDateString
        {
            get
            {
                return this.Date.ToString("d");
            }
        }
        
        public override string ToString()
        {
            return this.Title;
        }

        public static Comic FromJson(string json)
        {
            JsonValue jsonValue = JsonValue.Parse(json);
            DateTime date = GetDate(jsonValue);
            string title = jsonValue.GetObject().GetNamedString("title");
            string altText = jsonValue.GetObject().GetNamedString("alt");
            string imageUrl = jsonValue.GetObject().GetNamedString("img");
            int number = GetNumber(jsonValue);
            return new Comic(number, title, date, imageUrl, altText);
        }

        private static int GetNumber(JsonValue jsonValue)
        {
            double numberDouble = jsonValue.GetObject().GetNamedNumber("num");
            return Convert.ToInt32(numberDouble);
        }

        private static DateTime GetDate(JsonValue jsonValue)
        {
            string yearString = jsonValue.GetObject().GetNamedString("year");
            int year = Int32.Parse(yearString);
            string monthString = jsonValue.GetObject().GetNamedString("month");
            int month = Int32.Parse(monthString);
            string dayString = jsonValue.GetObject().GetNamedString("day");
            int day = Int32.Parse(dayString);
            return new DateTime(year, month, day);
        }
    }
}
