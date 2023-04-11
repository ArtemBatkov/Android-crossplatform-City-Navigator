using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services.NavigationService
{
    public interface INavigationService
    {
        Task PushAsync(Page page);
        Task PopAsync();
        Task PopToRootAsync();
    }
}
