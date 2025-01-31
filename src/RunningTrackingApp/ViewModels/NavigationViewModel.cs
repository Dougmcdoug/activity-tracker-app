using RunningTrackingApp.Commands;
using RunningTrackingApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningTrackingApp.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        public ViewModelBase CurrentViewModel => _navigationService.CurrentViewModel;

        public ICommand NavigateToImportCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMapCommand { get; }

        public NavigationViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            // Example command for navigation
            NavigateToImportCommand = new RelayCommand(() => _navigationService.NavigateTo<GPXImportViewModel>());
            NavigateToHomeCommand = new RelayCommand(() => _navigationService.NavigateTo<HomeViewModel>());

            // Define the NavigateToMapCommand in a way that lets us pass the filepath in as a parameter by using the generic RelayCommand<T>
            NavigateToMapCommand = new RelayCommand<string>((path) => _navigationService.NavigateTo<GPSTraceViewModel>(path)); 


            // Listen for navigation changes
            _navigationService.PropertyChanged += (s, e) =>
            {
                //if (e.PropertyName == nameof(_navigationService.CurrentViewModel))
                if (e.PropertyName == nameof(CurrentViewModel))
                {
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            };
        }
    }
}
