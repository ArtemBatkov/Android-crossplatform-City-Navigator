using MvvmHelpers;
using MvvmHelpers.Commands;
using SightsNavigator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;

namespace SightsNavigator.ViewModels
{

    public class TripEditViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, IDisposable
    {
        public event EventHandler<City> CityChanged;

        public INavigation navigation { get; set; }
        public City city { get => _city;
            set
            {                
                    _city = value;
                    CityChanged?.Invoke(this, _city);
                    OnPropertyChanged(nameof(_city));               
            }
        }
        private City _city; 

        public ObservableRangeCollection<String> Backgrounds { get; set; }

        

        //public TripEditViewModel() {
        //    var b = 3;
        //}

        public TripEditViewModel(City city) {
            this._city = city;
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            Backgrounds = new ObservableRangeCollection<string>();
            SelectedItemCommand = new Command(onSelectedItem);
            var backgrounds = city.Backgrounds.ToList();
            backgrounds.ForEach(bg => Backgrounds.Insert(0, bg));
        }
            

        //S-------COMMANDS--------//
        public AsyncCommand PageAppearingCommand { get; set; }
        public ICommand SelectedItemCommand { get; set; }

        //E-------COMMANDS--------//


        public String BackgroundSelected { get => _backgroundSelected; set => _backgroundSelected = value; }
        private string _backgroundSelected;

       
        

        private void onSelectedItem(object obj)
        {
            if(_backgroundSelected == null) return;
            city.CurrentBackground = _backgroundSelected;
            TripListModel.updateCityProporties(city);
            var updatedCity = TripListModel.getCityByName(city.Name);
            if (updatedCity == null) return;
            city = updatedCity;
        }

        
        public void onBackPressedVM()
        {            
            //CityChanged?.Invoke(this, city);
            //OnPropertyChanged();
        }



        private async Task PageAppearing()
        {
            await Refresh();
        }


        public async Task Refresh()
        {
            
        }

        public void Dispose()
        {
            CityChanged = null;
        }
    }
}
