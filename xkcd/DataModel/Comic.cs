namespace xkcd.DataModel
{
    using System;
    using System.Runtime.Serialization;
    using Windows.Data.Json;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    [DataContract]
    [Windows.Foundation.Metadata.WebHostHidden]
    public class Comic : Common.BindableBase
    {
        private static readonly Uri BaseUri = new Uri("ms-appx:///");

        public Comic(int number, string title, DateTime date, string imagePath, string altText)
        {
            _number = number;
            _title = title;
            _date = date;
            _altText = altText;
            _imagePath = imagePath;
        }

        private int _number;

        [DataMember]
        public int Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        private string _title = string.Empty;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private DateTime _date = DateTime.MinValue;

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private string _altText = string.Empty;

        [DataMember]
        public string AltText
        {
            get { return _altText; }
            set { SetProperty(ref _altText, value); }
        }

        private ImageSource _image;

        [DataMember]
        private String _imagePath;

        public ImageSource Image
        {
            get
            {
                if (_image == null && _imagePath != null)
                {
                    _image = new BitmapImage(new Uri(BaseUri, _imagePath));
                }
                return _image;
            }

            set
            {
                _imagePath = null;
                SetProperty(ref _image, value);
            }
        }

        public string Subtitle
        {
            get { return string.Format("#{0} {1}", Number, Date.ToString("d")); }
        }
        
        public override string ToString()
        {
            return Title;
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
