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
            ImportCommand = new RelayCommand(ImportGpxFile);
            _gpxParserService = gpxParserService;
        }

        private void ImportGpxFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "GPX files (*.gpx)|*.gpx",
                Title = "Select a GPX file"
            };

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                GpxData = _gpxParserService.ParseGpxFile(filePath);
            }
        }

        


        /*private readonly GPXParserService _gpxParserService;

        public string SelectedFile { get; private set; }
        public ICommand ImportGPXCommand { get; }

        public GPXImportViewModel(GPXParserService gpxParserService)
        {
            _gpxParserService = gpxParserService;
            ImportGPXCommand = new RelayCommand(ImportGPX);
        }

        private void ImportGPX()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "GPX files (*.gpx)|*.gpx|All files (*.*)|*.*",
                Title = "Select a GPX File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFile = openFileDialog.FileName;
                //var runData = _gpxParserService.Parse(SelectedFile);

                // Handle parsed data (e.g pass it to another ViewModel)
            }
        }*/

    }
}
