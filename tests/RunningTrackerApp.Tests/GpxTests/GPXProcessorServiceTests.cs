using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningTrackingApp.Services;

namespace RunningTrackerTests.GpxTests
{
    [Collection("GPX Service Tests")]
    public class GPXProcessorServiceTests : IClassFixture<GPXTestFixture>
    {
        private readonly GPXTestFixture _gpxServiceFixture;

        public GPXProcessorServiceTests(GPXTestFixture gpxServiceFixture)
        {
            _gpxServiceFixture = gpxServiceFixture;
        }

        /// <summary>
        /// Calculate the distance of the activity in the detailed gpx file.
        /// True distance is 8.62 km (as told by Strava!) so check distance is between 8 and 9 km.
        /// </summary>
        [Fact]
        public void CalculateTotalDistance_ValidDetailedFile_DistanceWithinExpectedRange()
        {
            var data = _gpxServiceFixture.ParserService.ParseGpxFile(_gpxServiceFixture.DetailedFilePath);

            // NOTE: I should be mocking this! This is a quick test for now, will return to this.
            var mapService = new MapService();
            var processor = new GPXProcessorService(mapService);

            var points = _gpxServiceFixture.ParserService.ExtractTrackPoints(data);

            var distance = processor.CalculateTotalDistance(points);

            Assert.True(distance > 8000);
            Assert.True(distance < 9500);
        }


        [Fact]
        public void CalculateTotalElevation_ValidDetailedFile_ElevationWithinExpectedRange()
        {
            var data = _gpxServiceFixture.ParserService.ParseGpxFile(_gpxServiceFixture.DetailedFilePath);

            // NOTE: I should be mocking this! This is a quick test for now, will return to this.
            var mapService = new MapService();
            var processor = new GPXProcessorService(mapService);

            var points = _gpxServiceFixture.ParserService.ExtractTrackPoints(data);

            var elevation = processor.CalculateTotalElevation(points);

            Assert.True(elevation > 250);
            Assert.True(elevation < 350);
        }
    }
}
