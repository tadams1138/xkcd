using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace xkcd.Test.DataModel
{
    [TestClass]
    public class Comic
    {
        [TestMethod]
        public void FromJson_ParsesJsonString()
        {
            // Arrange
            string sampleJson = Properties.Resources.SampleJson;

            // Act
            var result = xkcd.DataModel.Comic.FromJson(sampleJson);

            // Assert
            result.Title.Should().Be("AirAware");
            result.AltText.Should()
                  .Be(
                      "It ships with a version of Google Now that alerts you when it's too late to leave for your appointments.");
            result.Date.Should().Be(new DateTime(2013, 5, 3));
            result._num.Should().Be(1207);
            result._img.Should().Be(@"http://imgs.xkcd.com/comics/airaware.png");
        }
    }
}
