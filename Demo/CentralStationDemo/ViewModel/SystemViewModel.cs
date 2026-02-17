using CentralStationDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralStationDemo.ViewModel;

public partial class SystemViewModel : ObservableObject
{


    [ObservableProperty]
    private List<SystemTypeViewModel> deviceTypes = new List<SystemTypeViewModel>()
    {
        new SystemTypeViewModel(SystemTypes.CS3),
        new SystemTypeViewModel(SystemTypes.GFP3),
        new SystemTypeViewModel(SystemTypes.MS2),
        new SystemTypeViewModel(SystemTypes.MS1),
        new SystemTypeViewModel(SystemTypes.LinkS88),
        new SystemTypeViewModel(SystemTypes.WebApp),
        new SystemTypeViewModel(SystemTypes.Browser),
    };
}
