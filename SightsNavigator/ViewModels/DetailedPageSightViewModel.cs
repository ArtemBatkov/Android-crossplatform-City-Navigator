using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.ViewModels
{

    class DetailedPageSightViewModel
    {

        public DetailedPageSightViewModel()
        {
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
