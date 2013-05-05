using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xkcd.Test.DataModel
{
    [TestClass]
    public class ComicDataSource
    {
        const string ComicsDataFile = @"C:\Users\Thomas\Source\Repos\xkcd\Data\comics.xml";

        [Ignore]
        [TestMethod]
        public void RefreshComicsDataFile()
        {
            using (var readStream = LoadComicsDataFile())
            {
                var task = xkcd.DataModel.ComicDataSource.UpdateComicData(readStream);
                task.Wait();
            }

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
            var saveStream = new FileStream(ComicsDataFile, FileMode.Open);
            return saveStream;
        }
    }
}
