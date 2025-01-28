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
        public string DetailedFilePath { get; private set; }

        /// <summary>
        /// Set up files for use in testing GPXParserService.
        /// Basic file contains reduced attributes and only longitude, latitude and timestamp.
        /// Detailed file contains namespace attributes aswell as power, elevation, cadence and heart rate.
        /// </summary>
        public GPXParserServiceFixture()
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, SampleGpxContent);
            TempFilePath = tempFilePath;

            // Get path to the test file
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDir = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));
            var detailedFilePath = Path.Combine(solutionDir, "TestAssets/Morning_Run.gpx");
            DetailedFilePath = detailedFilePath;
        }

        public void Dispose()
        {

        }
    }

    /// <summary>
    /// Verifies that GPXParserService correctly parses a valid GPX file
    /// and updates the GpxData property with the parsed result
    /// </summary>
    public class GPXParserServiceTests : IClassFixture<GPXParserServiceFixture>
    {
        private readonly GPXParserServiceFixture _gpxParserServiceFixture;

        /// <summary>
        /// Test class constuctor, for initialising the service fixture.
        /// </summary>
        /// <param name="gpxParserServiceFixture"></param>
        public GPXParserServiceTests(GPXParserServiceFixture gpxParserServiceFixture)
        {
            _gpxParserServiceFixture = gpxParserServiceFixture;
        }

        /// <summary>
        /// Tests that ParseGpxFile successfully parses a valid .gpx file
        /// </summary>
        [Fact]
        public void ParseGpxFile_ValidBasicFile_ShouldParseCorrectly()
        {
            // Arrange
            var gpxParserService = new GPXParserService();       

            // Act
            GpxData data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.TempFilePath);

            // Assert
            Assert.NotNull(data);
        }

        /// <summary>
        /// Tests that ParseGpxFile correctly throws an ArgumentException when an invalid file path
        /// is provided.
        /// </summary>
        [Fact]
        public void ParseGpxFile_InvalidFilePath_ShouldThrowArgumentException()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            string badFilepath = "badfile.txt";

            // Assert
            Assert.Throws<ArgumentException>(() => gpxParserService.ParseGpxFile(badFilepath));
        }

        [Fact]
        public void ParseGpxFile_ValidRealFile_CheckCorrectActivityNameAndType()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.DetailedFilePath);

            // Assert
            Assert.NotNull(data);
            Assert.Equal("Morning Run", data.Tracks[0].ActivityName);
            Assert.Equal("running", data.Tracks[0].ActivityType);
        }


        [Fact]
        public void ParseGpxFile_ValidBasicFile_CheckCorrectGpxData()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.TempFilePath);

            // Assert
            // Test the two datapoints, checking longitude, latitude and the timestamp
            Assert.Equal(52.5200, data.Tracks[0].Segments[0].Points[0].Latitude);
            Assert.Equal(13.4050, data.Tracks[0].Segments[0].Points[0].Longitude);
            Assert.Equal(new DateTime(2025, 1, 21, 12, 0, 0), data.Tracks[0].Segments[0].Points[0].TimeStamp);

            Assert.Equal(52.5201, data.Tracks[0].Segments[0].Points[1].Latitude);
            Assert.Equal(13.4051, data.Tracks[0].Segments[0].Points[1].Longitude);
            Assert.Equal(new DateTime(2025, 1, 21, 12, 1, 0), data.Tracks[0].Segments[0].Points[1].TimeStamp);
        }
    }
}
