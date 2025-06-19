using DisplayAnOfflineMapOnDemand.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayAnOfflineMapOnDemand.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; }

        public MainViewModel()
        {
            CurrentViewModel = new NACViewModel(); // Initialize the CurrentViewModel property
 
        }
    }
}
