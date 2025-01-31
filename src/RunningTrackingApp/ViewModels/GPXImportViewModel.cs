using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Win32;
using RunningTrackingApp.Commands;
using RunningTrackingApp.Models;
using RunningTrackingApp.Services;

namespace RunningTrackingApp.ViewModels
{
    public class GPXImportViewModel : ViewModelBase
    {
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMapCommand { get; }
        public ICommand ImportCommand { get; }

        private GpxData _gpxData;
        public GpxData GpxData
        {
            get { return _gpxData; }
            set { SetProperty(ref _gpxData, value); }
        }

        private GPXParserService _gpxParserService;

        public GPXImportViewModel(NavigationViewModel navigationViewModel, GPXParserService gpxParserService)
        {
            NavigateToHomeCommand = navigationViewModel.NavigateToHomeCommand;
            NavigateToMapCommand = navigationViewModel.NavigateToMapCommand;
            ImportCommand = new RelayCommand(ImportGpxFile);
            _gpxParserService = gpxParserService;
        }

        /// <summary>
        /// Displays a dialog box for the user to select a .gpx file to import. The data is then
        /// imported.
        /// </summary>
        private void ImportGpxFile()
        {
            // Display dialog box for user to select a .gpx file
            var dialog = new OpenFileDialog
            {
                Filter = "GPX files (*.gpx)|*.gpx",
                Title = "Select a GPX file"
            };

            // If 'ok' button is pressed (file must be selected), then proceed to attempt to import
            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                //GpxData = _gpxParserService.ParseGpxFile(filePath);

                NavigateToMapCommand.Execute(filePath);
            }
        }

        


    }
}
