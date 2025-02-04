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
    /// <summary>
    /// A service class to handle navigation between ViewModels.
    /// </summary>
    public class NavigationService : INavigationService, INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        // The current view model to display in the window is held in CurrentViewModel
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel { 
            get => _currentViewModel;
            private set {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        /// <summary>
        /// Service constructor. 
        /// </summary>
        /// <param name="serviceProvider">Dependency injection container.</param>
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Handles navigation to a new ViewModel when no parameterss must be sent to the ViewModel constructor.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type to navigate to. Must be a ViewModelBase type.</typeparam>
        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            NavigateTo<TViewModel>(null);
        }

        /// <summary>
        /// Handles navigation to a viewmodel when a parameter must be sent to the ViewModel constructor.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter">Parameter to send to ViewModel constructor.</param>
        public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            if (viewModel is GPSTraceViewModel traceViewModel)
            {
                traceViewModel.Initialise((string)parameter);
            }

            // If the ViewModel is able to receiver a parameter, then retrieve it
            /*
            if (viewModel is IParameterReceiver receiver)
            {
                receiver.ReceiveParameter(parameter);
            }*/

            // Should refactor this to handle passing in parameters, rather than the Initialise() call above.
            if (viewModel is INavigable navigable)
            {
                navigable.OnNavigatedTo();
            }

            CurrentViewModel = viewModel;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
