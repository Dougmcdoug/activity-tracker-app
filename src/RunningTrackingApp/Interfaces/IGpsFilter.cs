using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace RunningTrackingApp.Interfaces
{
    /// <summary>
    /// Specifies a class that can operate as a filter for a gps trace.
    /// </summary>
    public interface IGpsFilter
    {
        public void Update(double x, double y, double z);
        public (double x, double y, double z) GetState();
    }
}
