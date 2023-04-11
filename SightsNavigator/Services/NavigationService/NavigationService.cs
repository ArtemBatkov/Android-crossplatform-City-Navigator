using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services.NavigationService
{
    public class NavigationService : INavigationService
    {
        private INavigation _navigation;

        public NavigationService(INavigation navigation)
        {
            _navigation = navigation;
        }

        public async Task PushAsync(Page page)
        {
            await _navigation.PushAsync(page);
        }

        public async Task PopAsync()
        {
            await _navigation.PopAsync();
        }

        public async Task PopToRootAsync()
        {
            await _navigation.PopToRootAsync();
        }
    }
}
