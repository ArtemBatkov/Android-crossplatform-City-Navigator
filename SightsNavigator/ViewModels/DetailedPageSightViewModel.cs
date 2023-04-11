using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SightsNavigator.Models;

namespace SightsNavigator.ViewModels
{

    class DetailedPageSightViewModel
    {

        public City.Sight SelectedSight;


        public DetailedPageSightViewModel()
        {
            SelectedSight = ViewModelLocator.GetViewModel<City.Sight>(nameof(Views.DetailedPage));
            PageAppearingCommand = new AsyncCommand(PageAppearing);
        }


        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        // COMMANDS - end
        private async Task PageAppearing()
        {             
            await Refresh();
        }

        public async Task Refresh()
        {             
        }
    }
}
