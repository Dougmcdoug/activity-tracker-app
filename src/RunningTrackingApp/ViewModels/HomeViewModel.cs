using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningTrackingApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ICommand NavigateToImportCommand { get; }

        public HomeViewModel(NavigationViewModel navigationViewModel)
        {
            NavigateToImportCommand = navigationViewModel.NavigateToImportCommand;
        }
    }
}
