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
        private readonly int _number;

        [DataMember(Name = "title")]
        private readonly string _title;

        [DataMember(Name = "alt")]
        private readonly string _altText;

        [DataMember(Name = "img")]
        private readonly string _imgUrl;

        [DataMember(Name = "day")]
        private readonly string _day;

        [DataMember(Name = "month")]
        private readonly string _month;

        [DataMember(Name = "year")]
        private readonly string _year;

        [DataMember(Name = "date")]
        private DateTime _date;

        public int Number
        {
            get { return _number; }
        }

        public string Title
        {
            get { return _title; }
        }

        public DateTime Date
        {
            get { return _date; }
        }

        public string AltText
        {
            get { return _altText; }
        }

        public string ImageUrl
        {
            get { return _imgUrl; }
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
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer =
                    new DataContractJsonSerializer(typeof (Comic));

                var comic = (Comic)serializer.ReadObject(ms);
                comic.SetDate();
                
                return comic;
            }
        }

        private void SetDate()
        {
            int year = Int32.Parse(_year);
            int month = Int32.Parse(_month);
            int day = Int32.Parse(_day);
            _date = new DateTime(year, month, day);
        }
    }
}
