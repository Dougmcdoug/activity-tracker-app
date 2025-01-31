using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackingApp.Interfaces
{
    /// <summary>
    /// Interface allowing ViewModels to receive a parameter
    /// </summary>
    public interface IParameterReceiver
    {
        void ReceiveParameter(object parameter);
    }
}
