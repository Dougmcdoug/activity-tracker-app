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
        public GpxData BasicData { get; private set; }
        public GpxData DetailedData { get; private set; }

        /// <summary>
        /// Public static property holding the expected test results for the two points in the 'basic' file.
        /// </summary>
        public static List<object?[]> BasicFileTestDataPoints { get; } = new List<object?[]>
        {
            new object?[] { 0, 52.5200, 13.4050, new DateTime(2025, 1, 21, 12, 0, 0), null, null, null, null },
            new object?[] {1, 52.5201, 13.4051, new DateTime(2025, 1, 21, 12, 1, 0), null, null, null, null }
        };


        /// <summary>
        /// Public static property holding the expected test results for the two points in the 'basic' file.
        /// </summary>
        public static List<object?[]> DetailedFileTestDataPoints { get; } = new List<object?[]>
        {
            new object?[] { 0, 54.4463680, -1.0879010, new DateTime(2025, 1, 19, 10, 37, 45), 146.4, 90, 0, 0 },
            new object?[] {1, 54.4463680, -1.0879010, new DateTime(2025, 1, 19, 10, 37, 46), 146.4, 91, 109, 0 },
            new object?[] {2, 54.4463360, -1.0878880, new DateTime(2025, 1, 19, 10, 37, 47), 146.4, 91, 110, 80 }     
        };


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
    /// Collection definition ensures the fixture instance is shared across all required tests
    /// </summary>
    [CollectionDefinition("GPX Parser Service Tests")]
    public class GPXParserCollection : ICollectionFixture<GPXParserServiceFixture>
    {
        // No code required here - xUnit handles the fixture lifecycle automatically
    }


    /// <summary>
    /// Verifies that GPXParserService correctly parses a valid GPX file
    /// and updates the GpxData property with the parsed result
    /// </summary>
    [Collection("GPX Parser Service Tests")]
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
        /// Tests that ParseGpxFile parses the gpx file and correctly returns the metadata for the (index)'th point. 
        /// Optional data should be null, because it is not present in the 'basic' file.
        /// </summary>
        [Theory]
        [MemberData(nameof(_gpxParserServiceFixture.BasicFileTestDataPoints), MemberType = typeof(GPXParserServiceFixture))]
        public void ParseGpxFile_ValidBasicFile_CheckCorrectGpxData(int index, double lat, double lon, DateTime timeStamp, double? elevation, int? heartRate, int? power, int? cadence)
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.TempFilePath);
            var points = gpxParserService.ExtractTrackPoints(data); // obtain all points as a List<TrackPoint>
            var point = points[index];

            // Assert
            Assert.Equal(lat, point.Latitude);
            Assert.Equal(lon, point.Longitude);
            Assert.Equal(timeStamp, point.TimeStamp);
            Assert.Equal(elevation, point.Elevation);
            Assert.Equal(heartRate, point.HeartRate);
            Assert.Equal(power, point.Power);
            Assert.Equal(cadence, point.Cadence);
        }

        /// <summary>
        /// Tests that ParseGpxFile correctly parses a real .GPX file and correctly returns the
        /// datapoint specified by index. All values should be non-null since they are all present in the file.
        /// </summary>
        [Theory]
        [MemberData(nameof(GPXParserServiceFixture.DetailedFileTestDataPoints), MemberType =typeof(GPXParserServiceFixture))]
        public void ParseGpxFile_ValidRealFile_CheckCorrectGpxData(int index, double lat, double lon, DateTime timeStamp, double? elevation, int? heartRate, int? power, int? cadence)
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            // Note: some of this could be moved into the fixture class, to prevent repeated execution of the next two lines
            var data = gpxParserService.ParseGpxFile(_gpxParserServiceFixture.DetailedFilePath);
            var points = gpxParserService.ExtractTrackPoints(data); // obtain all points as a List<TrackPoint>
            var point = points[index];

            // Assert
            Assert.Equal(lat, point.Latitude);
            Assert.Equal(lon, point.Longitude);
            Assert.Equal(timeStamp, point.TimeStamp);
            Assert.Equal(elevation, point.Elevation);
            Assert.Equal(heartRate, point.HeartRate);
            Assert.Equal(power, point.Power);
            Assert.Equal(cadence, point.Cadence);
        }
    }
}
