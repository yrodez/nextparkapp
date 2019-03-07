﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Domain.Entities;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Settings;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class ReservationViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Info { get; set; }                // Parking info text
        public string SubInfo { get; set; }             // Parking subInfo text
        public string Picture { get; set; }             // Parking picture source text
        public Aspect PictureAspect { get; set; }       // Parking picture aspect
        public string FullPrice { get; set; }           // Parking price full text (2 CHF/h)
        public string FullAvailability { get; set; }    // Parking availability full text (08:00-10:00)
        private DateTime _startDate { get; set; }       // Reservation start date 
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; OnStartDateChanged(); } 
        }         
        public TimeSpan StartTime { get; set; }         // Reservation start time
        public DateTime MinStartDate { get; set; }      // Reservation minimum start date
        public DateTime EndDate { get; set; }           // Reservation end date
        public TimeSpan EndTime { get; set; }           // Reservation end time
        public DateTime MinEndDate { get; set; }        // Reservation minimum end date

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand TimeChanged { get; set; }   // Time Picker property changed
        public ICommand BookAction { get; set; }   // Time Picker property changed

        private UIParkingModel _parking;

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IOrderDataService _orderDataService;
        private readonly IProfileService _profileService;

        public ReservationViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    IOrderDataService orderDataService,
                                    IProfileService profileService)
                                : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _orderDataService = orderDataService;
            _profileService = profileService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            BookAction = new Command<object>(OnBookingMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            // Header
            BackText = "Mappa";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIBookingModel booking)
            {
                _parking = _profileService.GetParkingById(booking.ParkingId);
                Info = _parking.Address;
                SubInfo = _parking.Cap.ToString() + " " + _parking.City;
                if (string.IsNullOrEmpty(_parking.ImageUrl))
                {
                    Picture = "icon_no_photo.png";
                    PictureAspect = Aspect.AspectFit;
                }
                else
                {
                    Picture = ApiSettings.BaseUrl + _parking.ImageUrl;
                    PictureAspect = Aspect.AspectFill;
                }
                FullPrice = _parking.PriceMin.ToString("N2") + " CHF/h";
                FullAvailability = (_parking.isFree()) ? "Disponibile" : "Occupato";
                base.OnPropertyChanged("Info");
                base.OnPropertyChanged("SubInfo");
                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("PictureAspect");
                base.OnPropertyChanged("FullPrice");
                base.OnPropertyChanged("FullAvailability");

                if ((booking.StartDate == null) || (booking.StartDate < DateTime.Now))
                {
                    booking.StartDate = DateTime.Now;
                }
                if ((booking.EndDate == null) || (booking.EndDate < DateTime.Now))
                {
                    booking.EndDate = DateTime.Now;
                }
                StartDate = booking.StartDate.Date;
                StartTime = booking.StartDate.TimeOfDay;
                MinStartDate = DateTime.Now.Date;
                EndDate = booking.EndDate.Date;
                EndTime = booking.EndDate.TimeOfDay;
                MinEndDate = booking.StartDate.Date;

                base.OnPropertyChanged("StartDate");
                base.OnPropertyChanged("StartTime");
                base.OnPropertyChanged("MinStartDate");
                base.OnPropertyChanged("EndDate");
                base.OnPropertyChanged("EndTime");
                base.OnPropertyChanged("MinEndDate");
            }

            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Booking button click action
        public void OnBookingMethod(object sender)
        {
            // Check Data
            if ((StartDate + StartTime) > (EndDate + EndTime))
            {
                _dialogService.ShowAlert("Errore", "Data e ora di fine devono essere sucessive a quelle di inizio");
                return;
            }

            // TODO: fill book data according to add book backend method
            OrderModel order = new OrderModel
            {
                ParkingId = _parking.Id,
                StartDate = StartDate+StartTime,
                EndDate = EndDate + EndTime,
                Price = double.Parse((((EndDate+EndTime)-(StartDate+StartTime)).TotalHours * _parking.PriceMin).ToString("N2")),
                UserId = int.Parse(AuthSettings.UserId),
                PaymentStatus = Enums.Enums.PaymentStatus.Pending
            };

            // Show activity spinner
            this.IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // TODO: send book action to backend
            SendOrder(order);
        }

        public async void SendOrder(OrderModel order)
        {
            try
            {
                var result = await _orderDataService.CreateOrderAsync(order);

                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if (result != null)
                {
                    await NavigationService.NavigateToAsync<UserBookingViewModel>();
                }
                else
                {
                    await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        private void OnStartDateChanged()
        {
            MinEndDate = _startDate;
            if (EndDate < _startDate)
            {
                EndDate = _startDate;
                base.OnPropertyChanged("EndDate");
            }
            base.OnPropertyChanged("MinEndDate");
        }
    }
}
