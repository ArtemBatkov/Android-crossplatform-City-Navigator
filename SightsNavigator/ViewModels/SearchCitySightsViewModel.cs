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
using SightsNavigator.Views.Converters;

namespace SightsNavigator.ViewModels
{
    class SearchCitySightsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public IServiceProvider ServiceProvider { get; set; }
        //public INavigationService Navigation => DependencyService.Get<INavigationService>();
        public ISightRequest service => DependencyService.Get<ISightRequest>();
        public IWebRequest webservice => DependencyService.Get<IWebRequest>();
        public ObservableRangeCollection<City.Sight> Sights { get; set; }

        public INavigation navigation;
        public SearchCitySightsViewModel()
        {
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SearchSightsCommand = new Command(onSearchSights);
            Sights = new ObservableRangeCollection<City.Sight>();
            FooterVisible = false;
            LoadMoreCommand = new AsyncCommand(onLoadMoreCommand);
            SelectedItemCommand = new Command(onSelectedSight);
            onTripPageCommand = new AsyncCommand(gotoTripPage);
        }
        
        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        public ICommand SearchSightsCommand { get; set; }
        public AsyncCommand LoadMoreCommand { get; set; }
        public CollectionView SightsCollectionView { get; set; }
        public ICommand SelectedItemCommand { get; set; }

        public AsyncCommand onTripPageCommand { get; set; }
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

        private bool SearchedPressed = false;

        //FUNCTIONS - start
        private async void onSearchSights()
        {
            SearchedPressed = true;
            IsLoad = true;            
            OnPropertyChanged(nameof(Sights));
            //Step 1 -- find the city
            await FindNewCity();

            //Step 2 -- redefind the sights of this city
            RedefindSights();            
          

            //Step 3 -- LoadMore
            await onLoadMoreCommand();

            //Sights.Clear();
            SearchedPressed = false;
            IsLoad = false;
            HasNothingFound = (Sights.Count == 0)  ? true : false;
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
            OnPropertyChanged(nameof(FooterVisible));
        }



        /// <summary>
        /// Method finds new city according to the search query
        /// </summary>
        private async Task FindNewCity()
        {
            city = await service.GetCityAsync(_query);
        }

        /// <summary>
        /// Method <c>RedefindSights</c> clears the existing list of Sights, and reinitialize according to the city
        /// </summary>
        private void RedefindSights()
        {
            if (city.SightList == null) return;
            //if (city.SightList.Count == 0) return;
            Sights.Clear(); // clear
            _start = 0; // start from zero
            _end = city.ListOfXids.Count(); // end to Count
            //for (int i = 0; i < city.SightList.Count(); i++)
            //{
            //    Sights.Insert(0, city.SightList[i]);
            //}
            OnPropertyChanged(nameof(Sights));
            OnPropertyChanged(nameof(city));
        }

        /// <summary>
        /// Method loads more data to the list
        /// </summary>
        private async Task onLoadMoreCommand()
        {
            TextLM = $"\t\t\t";
            IsLMSpinnerVisible = (SearchedPressed) ? false:true;
            int sec = 2;
            var beginmeasure = DateTime.UtcNow;
            await DelayFor(sec);
            var endmeasure = DateTime.UtcNow;
            Debug.WriteLine($"delay was: {endmeasure - beginmeasure}");

            if (city == null)
            {
                TextLM = "Load More";
                IsLMSpinnerVisible = false;
                return;
            }

            if (city.ListOfXids == null)
            {
                TextLM = "Load More";
                IsLMSpinnerVisible = false;
                return;
            }
            
            if (city.ListOfXids.Count == 0) {
                TextLM = "Load More";
                IsLMSpinnerVisible = false;
                return;
            }

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
            TextLM = "Load More";
            IsLMSpinnerVisible = false;
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
            await navigation.PushAsync(new DetailedPage(city, _sightSelected, ServiceProvider));
            SightSelected = null;
            OnPropertyChanged(nameof(SightSelected));
            //await Shell.Current.GoToAsync(nameof(Views.DetailedPage));
        }


        private async Task gotoTripPage()
        {
            await navigation.PushAsync(new TripPage(ServiceProvider));
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



        public String Query
        {
            get => _query;
            set => _query = value;
        }
        private string _query = "Saint-Petersburg";


        private bool _isload = false;
        public bool IsLoad { get => _isload; set
            {
                _isload = value;
                IsVisibleStack = _isload || _nothingFound;               
                OnPropertyChanged(nameof(IsLoad));
                OnPropertyChanged(nameof(IsVisibleStack));
            }
        }

        private bool _nothingFound = false;
        public bool HasNothingFound { get => _nothingFound; set
            {
                _nothingFound = value;
                IsVisibleStack = _isload || _nothingFound;                
                OnPropertyChanged(nameof(HasNothingFound));
                OnPropertyChanged(nameof(IsVisibleStack));
                OnPropertyChanged(nameof(IsLoad));
            }
        }
        

        public bool IsVisibleStack
        {
            get => _isVisibleStack; set
            {
                _isVisibleStack = value;
                OnPropertyChanged(nameof(IsVisibleStack));
            }
        }
        private bool _isVisibleStack = false;


        public bool IsLMSpinnerVisible { get => _isLMSpinnerVisible;
            set {
                _isLMSpinnerVisible = value;
                OnPropertyChanged(nameof(IsLMSpinnerVisible));
            }
        }
        private bool _isLMSpinnerVisible = false;



        public string TextLM { get => _txtLM;
                set {
                _txtLM = value;
                OnPropertyChanged(nameof(TextLM));
            } 
        }
        private string _txtLM = "Load More";



    }
}
