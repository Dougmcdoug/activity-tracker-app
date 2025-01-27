using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RunningTrackingApp.ViewModels;
using Moq;
using RunningTrackingApp.Services;

namespace RunningTrackerTests.GpxTests
{
    public class GpxImportViewModelTests
    {
        private const string SampleGpxContent = @"
            <gpx>
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

        /*
        [Fact]
        public void ParseGpxFile_ValidFile_ShouldParseCorrectly()
        {
            // Arrange
            var mockNavigationViewModel = new Mock<NavigationViewModel>();
            var mockGPXParserService = new Mock<GPXParserService>();

            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, SampleGpxContent);

            var viewModel = new GPXImportViewModel(mockNavigationViewModel.Object, mockGPXParserService.Object);

            // Act
            var gpxData = viewModel.ImportGpxFile(tempFilePath);

            // Assert

        }*/
    }
}
