using Microsoft.Extensions.DependencyInjection;
using RunningTrackingApp.Interfaces;
using RunningTrackingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackingApp.Services
{
    public class NavigationService : INavigationService, INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        //public ViewModelBase CurrentViewModel { get; private set; }
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel { 
            get => _currentViewModel;
            private set {
                if (_currentViewModel != value)
                {
                    //Debug.WriteLine($"CurrentViewModel changed from {CurrentViewModel?.GetType().Name} to {value?.GetType().Name}");
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            if (viewModel is INavigable navigable)
            {
                navigable.OnNavigatedTo();
            }
            CurrentViewModel = viewModel;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
