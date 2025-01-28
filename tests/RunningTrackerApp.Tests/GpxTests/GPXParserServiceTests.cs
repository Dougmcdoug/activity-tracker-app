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

        /// <summary>
        /// Tests that ParseGpxFile correctly parses a basic gpx file and correctly returns
        /// the first two data points. Optional data should be null, because it is not
        /// present in the 'basic' file.
        /// </summary>
        [Fact]
        public void ParseGpxFile_ValidBasicFile_CheckCorrectGpxData()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.TempFilePath);
            var point1 = data.Tracks[0].Segments[0].Points[0];
            var point2 = data.Tracks[0].Segments[0].Points[1];

            // Assert
            // Test the two datapoints, checking longitude, latitude and the timestamp
            Assert.Equal(52.5200, point1.Latitude);
            Assert.Equal(13.4050, point1.Longitude);
            Assert.Equal(new DateTime(2025, 1, 21, 12, 0, 0), point1.TimeStamp);
            Assert.Null(point1.Elevation);
            Assert.Null(point1.HeartRate);
            Assert.Null(point1.Power);
            Assert.Null(point1.Cadence);

            Assert.Equal(52.5201, point2.Latitude);
            Assert.Equal(13.4051, point2.Longitude);
            Assert.Equal(new DateTime(2025, 1, 21, 12, 1, 0), point2.TimeStamp);
            Assert.Null(point2.Elevation);
            Assert.Null(point2.HeartRate);
            Assert.Null(point2.Power);
            Assert.Null(point2.Cadence);
        }

        /// <summary>
        /// Tests that ParseGpxFile correctly parses a real .GPX file and correctly returns the
        /// first three data points. All values should be non-null since they are all present in the file.
        /// </summary>
        [Fact]
        public void ParseGpxFile_ValidRealFile_CheckCorrectGpxData()
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.DetailedFilePath);
            var point1 = data.Tracks[0].Segments[0].Points[0];
            var point2 = data.Tracks[0].Segments[0].Points[1];
            var point3 = data.Tracks[0].Segments[0].Points[2];

            // Assert
            // Test the first three datapoints, checking all 
            Assert.Equal(54.4463680, point1.Latitude);
            Assert.Equal(-1.0879010, point1.Longitude);
            Assert.Equal(new DateTime(2025, 1, 19, 10, 37, 45), point1.TimeStamp);
            Assert.Equal(146.4, point1.Elevation);
            Assert.Equal(90, point1.HeartRate);
            Assert.Equal(0, point1.Power);
            Assert.Equal(0, point1.Cadence);

            Assert.Equal(54.4463680, point2.Latitude);
            Assert.Equal(-1.0879010, point2.Longitude);
            Assert.Equal(new DateTime(2025, 1, 19, 10, 37, 46), point2.TimeStamp);
            Assert.Equal(146.4, point2.Elevation);
            Assert.Equal(91, point2.HeartRate);
            Assert.Equal(109, point2.Power);
            Assert.Equal(0, point2.Cadence);

            Assert.Equal(54.4463360, point3.Latitude);
            Assert.Equal(-1.0878880, point3.Longitude);
            Assert.Equal(new DateTime(2025, 1, 19, 10, 37, 47), point3.TimeStamp);
            Assert.Equal(146.4, point3.Elevation);
            Assert.Equal(91, point3.HeartRate);
            Assert.Equal(110, point3.Power);
            Assert.Equal(80, point3.Cadence);
        }
    }
}
