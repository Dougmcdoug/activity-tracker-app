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
    /// <summary>
    /// Verifies that GPXParserService correctly parses a valid GPX file
    /// and updates the GpxData property with the parsed result
    /// </summary>
    [Collection("GPX Service Tests")]
    public class GPXParserServiceTests : IClassFixture<GPXTestFixture>
    {
        private readonly GPXTestFixture _gpxServiceFixture;

        /// <summary>
        /// Test class constuctor, for initialising the service fixture.
        /// </summary>
        /// <param name="gpxParserServiceFixture"></param>
        public GPXParserServiceTests(GPXTestFixture gpxServiceFixture)
        {
            _gpxServiceFixture = gpxServiceFixture;
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
            GpxData data = gpxParserService.ParseGpxFile(_gpxServiceFixture.TempFilePath);

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
            var data = gpxParserService.ParseGpxFile(_gpxServiceFixture.DetailedFilePath);

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
        [MemberData(nameof(_gpxServiceFixture.BasicFileTestDataPoints), MemberType = typeof(GPXTestFixture))]
        public void ParseGpxFile_ValidBasicFile_CheckCorrectGpxData(int index, double lat, double lon, DateTime timeStamp, double? elevation, int? heartRate, int? power, int? cadence)
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            var data = gpxParserService.ParseGpxFile(_gpxServiceFixture.TempFilePath);
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
        [MemberData(nameof(_gpxServiceFixture.DetailedFileTestDataPoints), MemberType =typeof(GPXTestFixture))]
        public void ParseGpxFile_ValidRealFile_CheckCorrectGpxData(int index, double lat, double lon, DateTime timeStamp, double? elevation, int? heartRate, int? power, int? cadence)
        {
            // Arrange
            var gpxParserService = new GPXParserService();

            // Act
            // Note: some of this could be moved into the fixture class, to prevent repeated execution of the next two lines
            var data = gpxParserService.ParseGpxFile(_gpxServiceFixture.DetailedFilePath);
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
