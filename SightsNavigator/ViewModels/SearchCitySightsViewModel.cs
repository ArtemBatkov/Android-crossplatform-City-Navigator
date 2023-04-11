using MvvmHelpers;
using SightsNavigator.Views;
using MvvmHelpers.Commands;
using SightsNavigator.Models;
using SightsNavigator.Services;
using SightsNavigator.Services.SightService;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;
using Debug = System.Diagnostics.Debug;
using SightsNavigator.Services.NavigationService;

namespace SightsNavigator.ViewModels
{
    class SearchCitySightsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public INavigationService Navigation => DependencyService.Get<INavigationService>();
        public ISightRequest service => DependencyService.Get<ISightRequest>();
        public IWebRequest webservice => DependencyService.Get<IWebRequest>();
        public ObservableRangeCollection<City.Sight> Sights { get; set; }
        public SearchCitySightsViewModel()
        {
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SearchSightsCommand = new Command(onSearchSights);
            Sights = new ObservableRangeCollection<City.Sight>();
            FooterVisible = false;
            LoadMoreCommand = new AsyncCommand(onLoadMoreCommand);
            SelectedItemCommand = new Command(onSelectedSight);
        }



        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        public ICommand SearchSightsCommand { get; set; }
        public ICommand LoadMoreCommand { get; set; }
        public CollectionView SightsCollectionView { get; set; }
        public ICommand SelectedItemCommand { get; set; }

        // COMMANDS - end


        //Properties - start
        private int _start = 0; // start chunck
        private int _end = 0; // end chunck
        private int _defaultStep = 7; // default step of chunck

        private City city; //city


        public bool FooterVisible
        {
            get => _footerVisible;
            set
            {
                if (_footerVisible != value)
                    _footerVisible = value;
                OnPropertyChanged(nameof(FooterVisible));
            }
        }

        private bool _footerVisible = false;
        //Properties - end



        //FUNCTIONS - start
        private async void onSearchSights()
        {
            //Step 1 -- find the city
            await FindNewCity();

            //Step 2 -- redefind the sights of this city
            RedefindSights();

            //Step 3 -- LoadMore
            await onLoadMoreCommand();
        }


        public void onSightsScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //page injection
            var collectionView = sender as CollectionView;
            var items = collectionView.ItemsSource as IList<City.Sight>;
            if (items == null || items.Count == 0)
            {
                FooterVisible = false;
                return;
            }
            var lastVisibleItemIndex = e.LastVisibleItemIndex;
            var lastItemIndex = items.Count - 1;
            //var lastItemIndex = BindableLayout.GetItemsSource(collectionView).Cast<object>().Count() - 1;
            //var lastItemIndex = BindableLayout.GetItemsSource(collectionView)?.Cast<object>().Count() - 1 ?? -1;

            //if(lastItemIndex == lastVisibleItemIndex)
            //if (lastItemIndex >= 0 && lastVisibleItemIndex >= 0 && lastItemIndex == lastVisibleItemIndex)
            //{
            //    FooterVisible = true;

            //}
            //else
            //{              
            //    FooterVisible = false;
            //}

            if (lastVisibleItemIndex >= lastItemIndex)
            {
                FooterVisible = true;
            }
            else
            {
                FooterVisible = false;
            }
        }



        /// <summary>
        /// Method finds new city according to the search query
        /// </summary>
        private async Task FindNewCity()
        {
            city = await service.GetCityAsync("Moscow");
        }

        /// <summary>
        /// Method <c>RedefindSights</c> clears the existing list of Sights, and reinitialize according to the city
        /// </summary>
        private void RedefindSights()
        {
            if (city.SightList == null) return;
            if (city.SightList.Count == 0) return;
            Sights.Clear(); // clear
            _start = 0; // start from zero
            _end = city.ListOfXids.Count(); // end to Count
            for (int i = 0; i < city.SightList.Count(); i++)
            {
                Sights.Insert(0, city.SightList[i]);
            }
        }

        /// <summary>
        /// Method loads more data to the list
        /// </summary>
        private async Task onLoadMoreCommand()
        {
            int sec = 2;
            var beginmeasure = DateTime.UtcNow;
            await DelayFor(sec);
            var endmeasure = DateTime.UtcNow;
            Debug.WriteLine($"delay was: {endmeasure - beginmeasure}");

            if (city == null) return;
            if (city.ListOfXids == null) return;
            if (city.ListOfXids.Count == 0) return;
            Debug.WriteLine("Load More...");
            //define the step
            _end = city.ListOfXids.Count();
            int step = _defaultStep;

            if (_end - _start <= _defaultStep)
                step = _end - _start;
            else if (_end - _start > _defaultStep)
                step = _defaultStep;

            int from = _start;
            int to = _start + step;
            Debug.Print($"[from = {from}, to = {to}, overall = {_end} ]");

            var slice = city.ListOfXids.GetRange(from, step);//from = intial point, step = count

            var chunkOfSights = await service.GetChunckOfSights(slice);

            if (chunkOfSights is not null)
            {
                _start = to;
                foreach (var sight in chunkOfSights)
                {
                    //if (!String.Equals(sight.Image, "ERROR_DECODE_IMAGE"))
                    //{
                        city.SightList.Add(sight);
                        Sights.Add(sight);
                   // }
                    
                }


            }
        }

        /// <summary>
        /// Delays your thread for some seconds
        /// </summary>
        /// <param name="sec">seconds for delay</param>
        private async Task DelayFor(int sec)
        {
            int mills = 1000;
            await Task.Delay(mills * sec);
        }

        public City.Sight SightSelected { get => _sightSelected; set => _sightSelected = value; }
        private City.Sight _sightSelected;
        private async void onSelectedSight()
        {
            if (_sightSelected == null) return;
            await Navigation.PushAsync(new DetailedPage(_sightSelected));
        }






        private async Task PageAppearing()
        {
            FooterVisible = false;
            await Refresh();
        }

        public async Task Refresh()
        {
            System.Diagnostics.Debug.WriteLine($"Visible: {_footerVisible}");
        }
        //FUNCTIONS - end



    }
}
