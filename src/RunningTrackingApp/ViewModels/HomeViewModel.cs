using RunningTrackingApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningTrackingApp.ViewModels
{
    public class HomeViewModel : ViewModelBase, INavigable
    {
        public ICommand NavigateToImportCommand { get; }

        public HomeViewModel(NavigationViewModel navigationViewModel)
        {
            NavigateToImportCommand = navigationViewModel.NavigateToImportCommand;
        }


        /// <summary>
        /// Initialiation code using input parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public void OnNavigatedTo(object parameter = null)
        {
        }
    }
}
