using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace xkcd.Test.DataModel
{
    [TestClass]
    public class ComicDataSource
    {
        const string ComicsDataFile = @"C:\Users\Thomas\Source\Repos\xkcd\Data\comics.xml";

        // This is an integration test/tool to update the comics.xml file to the latest json on the web
        //[Ignore]
        [TestMethod]
        public void RefreshComicsDataFile()
        {
            using (var readStream = LoadComicsDataFile())
            {
                xkcd.DataModel.ComicDataSource.LoadDataFromFile(readStream);
            }

            var task = xkcd.DataModel.ComicDataSource.UpdateComicData();
            task.Wait();

            using (var saveStream = GetSaveStream())
            {
                xkcd.DataModel.ComicDataSource.SaveComicData(saveStream);
            }
        }

        private Stream GetSaveStream()
        {
            var saveStream = new FileStream(ComicsDataFile, FileMode.Create);
            return saveStream;
        }

        private Stream LoadComicsDataFile()
        {
            var readStream = new FileStream(ComicsDataFile, FileMode.Open);
            return readStream;
        }
    }
}
