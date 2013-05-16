namespace xkcd.DataModel
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    [DataContract]
    public class Comic
    {
        [DataMember(Name = "num")]
        public readonly int _num;

        [DataMember(Name = "title")]
        public readonly string Title;

        [DataMember(Name = "alt")]
        public readonly string AltText;

        [DataMember(Name = "img")]
        public readonly string _img;

        [DataMember(Name = "day")]
        public readonly string Day;

        [DataMember(Name = "month")]
        public readonly string Month;

        [DataMember(Name = "year")]
        public readonly string Year;

        [DataMember(Name = "date")]
        public DateTime Date;
        
        public int Number
        {
            get { return _num; }
        }

        public string Subtitle
        {
            get { return string.Format("#{0} {1}", _num, Date.ToString("d")); }
        }

        public string ImageUrl
        {
            get { return _img; }
        }
        
        public Uri Uri
        {
            get { return new Uri(string.Format("http://xkcd.com/{0}/", _num), UriKind.Absolute); }
        }

        public Uri ExplanationUri
        {
            get { return new Uri(string.Format("http://www.explainxkcd.com/wiki/index.php?title={0}", _num), UriKind.Absolute); }
        }

        public override string ToString()
        {
            return Title;
        }

        public static Comic FromJson(string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer =
                    new DataContractJsonSerializer(typeof(Comic));

                var comic = (Comic)serializer.ReadObject(ms);
                comic.SetDate();

                return comic;
            }
        }

        private void SetDate()
        {
            int year = Int32.Parse(Year);
            int month = Int32.Parse(Month);
            int day = Int32.Parse(Day);
            Date = new DateTime(year, month, day);
        }
    }
}
