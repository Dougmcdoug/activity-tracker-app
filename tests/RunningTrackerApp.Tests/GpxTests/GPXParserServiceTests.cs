using RunningTrackingApp.Models;
using RunningTrackingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RunningTrackerTests.GpxTests
{
    public class GPXParserServiceFixture : IDisposable
    {
        private const string SampleGpxContent = @"<gpx>
    <trk>
        <name>Test Track</name>
        <trkseg>
            <trkpt lat=""52.5200"" lon=""13.4050"">
                <time>2025-01-21T12:00:00Z</time>
            </trkpt>
            <trkpt lat=""52.5201"" lon=""13.4051"">
                <time>2025-01-21T12:01:00Z</time>
            </trkpt>
        </trkseg>
    </trk>
</gpx>";

        public string TempFilePath { get; private set; }

        public GPXParserServiceFixture()
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, SampleGpxContent);
            TempFilePath = tempFilePath;
        }

        public void Dispose()
        {

        }
    }

    public class GPXParserServiceTests : IClassFixture<GPXParserServiceFixture>
    {
        private readonly GPXParserServiceFixture _gpxParserServiceFixture;

        public GPXParserServiceTests(GPXParserServiceFixture gpxParserServiceFixture)
        {
            _gpxParserServiceFixture = gpxParserServiceFixture;
        }

        [Fact]
        public void ParseGpxFile_ValidFile_ShouldParseCorrectly()
        {
            // Arrange
            var gpxParserService = new GPXParserService();       

            // Act
            GpxData data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.TempFilePath);

            // Assert
            Assert.NotNull(data);
            Assert.Single(data.Tracks);
        }

        [Fact]
        public void ParseGpxFile_InvalidFilePath_ShouldThrowInvalidArgumentException()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            string badFilepath = "badfile.txt";

            // Assert
            Assert.Throws<ArgumentException>(() => gpxParserService.ParseGpxFile(badFilepath));
        }
    }
}
