using RunningTrackingApp.Models;
using RunningTrackingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackerTests.GpxTests
{
    /// <summary>
    /// Collection definition ensures the fixture instance is shared across all required tests
    /// </summary>
    [CollectionDefinition("GPX Service Tests")]
    public class GPXParserCollection : ICollectionFixture<GPXTestFixture>
    {
        // No code required here - xUnit handles the fixture lifecycle automatically
    }


    /// <summary>
    /// Fixture to handle file names and creation of a temporary file containing a mocked 'basic' gpx file.
    /// </summary>
    public class GPXTestFixture : IDisposable
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
        public GPXParserService ParserService { get; private set; }

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
        public GPXTestFixture()
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, SampleGpxContent);
            TempFilePath = tempFilePath;

            // Get path to the test file
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var solutionDir = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));
            var detailedFilePath = Path.Combine(solutionDir, "TestAssets/Morning_Run.gpx");
            DetailedFilePath = detailedFilePath;


            ParserService = new GPXParserService();
        }

        public void Dispose()
        {

        }
    }
}
