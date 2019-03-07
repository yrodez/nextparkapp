﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using NextPark.Mobile.UIModels;

namespace NextPark.Mobile.ViewModels
{
    public class UserBookingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public bool NoElementFound { get; set; }
        public double BookingListHeight { get; set; }   // Booking List View Height Request

        public ICommand OnBookingTapped { get; set; }   // Booking list tap action
        public ICommand OnBookingDelete { get; set; }   // Booking delete action
        //public ICommand OnBookingSwiped { get; set; }   // Booking swipe action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IProfileService _profileService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IOrderDataService _orderDataService;

        // PRIVATE VARIABLES
        private ObservableCollection<UIBookingModel> bookingList;

        public ObservableCollection<UIBookingModel> BookingList
        {
            get { return bookingList; }
            set { bookingList = value; base.OnPropertyChanged("BookingList"); }
        }

        // METHODS
        public UserBookingViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    IProfileService profileService,
                                    IParkingDataService parkingDataService,
                                    IOrderDataService orderDataService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _profileService = profileService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Item actions
            OnBookingTapped = new Command<object>(OnBookingTappedMethod);
            OnBookingDelete = new Command<object>(OnBookingDeleteMethod);
            //OnBookingSwiped = new Command<object>(OnBookingSwipedMethod);

            // Example starting values
            BookingList = new ObservableCollection<UIBookingModel>();

            NoElementFound = false;
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {

            // Header
            BackText = "Profilo";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // TODO: add booking list
            UpdateUserBookings();

            return Task.FromResult(false);
        }

        public async void UpdateUserBookings()
        {
            await GetUserBookings();
        }

        private async Task GetUserBookings()
        {
            var ordersResponse = await _orderDataService.GetAllOrdersAsync();

            /*
            {
                new BookingItem { UID = 0, Address = "Via Strada 1", City = "Lugano, Ticino", Time = "01h45", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete },
                new BookingItem { UID = 1, Address = "Via Strada 1.5", City = "Lugano, Ticino", Time = "02h00", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete },
                new BookingItem { UID = 2, Address = "Via Strada 2", City = "Lugano, Ticino", Time = "02h30", OnBookingTap = OnBookingTapped, OnBookingDel = OnBookingDelete }
            };
            */

            int count = 0;
            BookingList.Clear();

            ordersResponse = new List<OrderModel>();
            ordersResponse.Add(new OrderModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                OrderStatus = Enums.OrderStatus.Actived,
                ParkingId = 10,
                Price = 2.0,
                UserId = 5
            });

            foreach (OrderModel order in ordersResponse)
            {

                if (order.UserId == int.Parse(AuthSettings.UserId))
                {
                    //var parking = await _parkingDataService.GetParkingAsync(order.ParkingId);
                    var parking = _profileService.GetParkingById(order.ParkingId);
                    if (parking != null) {

                        TimeSpan remainingTime = order.EndDate - DateTime.Now;

                        BookingList.Add(new UIBookingModel
                        {
                            UID = order.Id,
                            Index = count++,
                            Address = parking.Address,
                            Cap = parking.Cap.ToString(),
                            City = parking.City,
                            Parking = (ParkingModel)parking,
                            Time = string.Format("{0}:{1}", remainingTime.Hours.ToString(), remainingTime.Minutes.ToString()),
                            OnBookingDel = OnBookingDelete,
                            OnBookingTap = OnBookingTapped
                        });
                    }

                }
            }
            base.OnPropertyChanged("BookingList");

            if (BookingList.Count == 0)
            {
                NoElementFound = true;

            }
            else
            {
                NoElementFound = false;
            }
            base.OnPropertyChanged("NoElementFound");

            BookingListHeight = BookingList.Count * 50.0;
            base.OnPropertyChanged("BookingListHeight");
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Booking list item click action
        public void OnBookingTappedMethod(object sender)
        {
            // TODO: Evaluate using UID or index of booking list (currently using UID as index)
            if (sender is int)
            {
                UIBookingModel item = BookingList[(int)sender];
                // TODO: pass booking item to booking map page
                NavigationService.NavigateToAsync<BookingMapViewModel>(item);
            }
        }

        // Booking list item delete click action
        public void OnBookingDeleteMethod(object sender)
        {
            // TODO: Evaluate using UID or index of booking list (currently using UID as index)
            if (sender is int)
            {
                UIBookingModel item = BookingList[(int)sender];

                if (DateTime.Now < item.StartDate)
                {
                    // Delete
                    // TODO: manage booking delete action
                    _dialogService.ShowAlert("Alert", "TODO: manage booking delete action");
                }
                else
                {
                    // Terminate
                    // TODO: manage booking delete action
                    _dialogService.ShowAlert("Alert", "TODO: manage terminate oreder");
                }
            }
        }

        public void OnBookingSwipedMethod(object args)
        {
            _dialogService.ShowAlert("Alert", "Swipe booking: " + args);
        }
    }
}
