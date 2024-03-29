﻿using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Settings;
using System.Collections.ObjectModel;
using NextPark.Models;
using NextPark.Mobile.UIModels;
using NextPark.Enums.Enums;
using System.Collections.Generic;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Controls;

namespace NextPark.Mobile.ViewModels
{

    public class ParkingDataViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Picture { get; set; }             // Parking image
        public string Address { get; set; }             // Parking address text
        public string Cap { get; set; }                 // Parking CAP text
        public string City { get; set; }                // Parking city text
        public string Revenue { get; set; }             // Parking revenue text
        public string Notes { get; set; }               // Parking notes text
        public bool IsNotesVisible { get; set; }        // Parking notes visibility
        public string ActiveStatusText { get; set; }    // Parking active status text
        public bool ActiveSwitchToggled                 // Parking active status value
        {
            get { return this.activeSwitchToggled; }
            set { activeSwitchToggled = value; OnSwitchToggleMethod(value); }
        }
        public ICommand OnEditParking { get; set; }     // Edit parking button action
        public ICommand OnCalendar { get; set; }        // Calendar click action
        public ICommand OnAddAvailability { get; set; } // Add Availability click action

        public DateTime SelectedDay { get; set; }       // Currently selected day
        private DateTime _pickerDateTime;
        public DateTime DatePickerDate
        {
            get { return _pickerDateTime; }
            set { if (!value.Equals(SelectedDay)) ChangeSelectedDay(value); }
        }
        // DatePicker date
        public ICommand OnPreviousWeek { get; set; }    // Previous week click action
        public ICommand OnNextWeek { get; set; }        // Next week click action
        public ICommand OnDaySelected { get; set; }     // Day number selection

        public string Day1Text { get; set; }            // Monday day numer text
        public Color Day1TextColor { get; set; }        // Monday number text color
        public Color Day1BackgroundColor { get; set; }  // Monday number background color
        public DateTime Day1DateTime { get; set; }      // Monday date/time
        public string Day2Text { get; set; }            // Tuesday day number text
        public Color Day2TextColor { get; set; }        // Tuesday number text color
        public Color Day2BackgroundColor { get; set; }  // Tuesday number background color
        public DateTime Day2DateTime { get; set; }      // Tuesday date/time
        public string Day3Text { get; set; }            // Wednesday day number text 
        public Color Day3TextColor { get; set; }        // Wednesday number text color
        public Color Day3BackgroundColor { get; set; }  // Wednesday number background color
        public DateTime Day3DateTime { get; set; }      // Wednesday date/time
        public string Day4Text { get; set; }            // Thursday day numer text
        public Color Day4TextColor { get; set; }        // Thursday number text color
        public Color Day4BackgroundColor { get; set; }  // Thursday number background color
        public DateTime Day4DateTime { get; set; }      // Thursday date/time
        public string Day5Text { get; set; }            // Friday day numer text
        public Color Day5TextColor { get; set; }        // Friday number text color
        public Color Day5BackgroundColor { get; set; }  // Friday number background color
        public DateTime Day5DateTime { get; set; }      // Friday date/time
        public string Day6Text { get; set; }            // Saturday day numer text
        public Color Day6TextColor { get; set; }        // Saturday number text color
        public Color Day6BackgroundColor { get; set; }  // Saturday number background color
        public DateTime Day6DateTime { get; set; }      // Saturday date/time
        public string Day7Text { get; set; }            // Sunday day numer text
        public Color Day7TextColor { get; set; }        // Sunday number text color
        public Color Day7BackgroundColor { get; set; }  // Sunday number background color
        public DateTime Day7DateTime { get; set; }      // Sunday date/time

        public ICommand OnEventTapAction { get; set; }

        public List<OrderModel> Orders { get; set; }
        public List<EventModel> Events { get; set; }

        public DayView MyDayContent { get; set; }

        // Order detail pop-up
        public bool OrderDetailVisible { get; set; }
        public string OrderStartDateTime { get; set; }
        public string OrderEndDateTime { get; set; }
        public string OrderCarPlate { get; set; }
        public ICommand OnOrderDetailClose { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IProfileService _profileService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IEventDataService _eventDataService;
        private readonly IOrderDataService _orderDataService;

        // PRIVATE VARIABLES
        private ParkingItem _parking;
        private bool eventsReady;
        private bool activeSwitchToggled;
        private ObservableCollection<UICalendarEventModel> calendarEvents;
        public ObservableCollection<UICalendarEventModel> CalendarEvents
        {
            get { return calendarEvents; }
            set { calendarEvents = value; base.OnPropertyChanged("CalendarEvents"); }
        }

        // METHODS
        public ParkingDataViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    IProfileService profileService,
                                    IParkingDataService parkingDataService,
                                    IEventDataService eventDataService,
                                    IOrderDataService orderDataService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _profileService = profileService;
            _parkingDataService = parkingDataService;
            _eventDataService = eventDataService;
            _orderDataService = orderDataService;

            eventsReady = false;
            Events = new List<EventModel>();
            Orders = new List<OrderModel>();

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Buttons action
            OnEditParking = new Command<object>(OnEditParkingMethod);
            OnCalendar = new Command<object>(OnCalendarMethod);
            OnAddAvailability = new Command<object>(OnAddAvailabilityMethod);

            // Calendar actions
            OnPreviousWeek = new Command<object>(OnPreviousWeekMethod);
            OnNextWeek = new Command<object>(OnNextWeekMethod);
            OnDaySelected = new Command<object>(OnDaySelectedMethod);
            OnEventTapAction = new Command<int>(OnEventTapMethod);

            CalendarEvents = new ObservableCollection<UICalendarEventModel>();

            // Order detail pop-up
            OrderDetailVisible = false;
            OnOrderDetailClose = new Command(OnOrderDetailCloseMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            if (data is ParkingItem)
            {
                _parking = (ParkingItem)data;
                _profileService.LastEditingParking = _parking;

                // DEBUG notes
                //_parking.ParkingModel.Notes = "Suonare il campanello";

                // Header
                BackText = "Indietro";
                UserName = AuthSettings.User.Name;
                UserMoney = AuthSettings.UserCoin.ToString("N2");
                base.OnPropertyChanged("BackText");
                base.OnPropertyChanged("UserName");
                base.OnPropertyChanged("UserMoney");

                Picture = _parking.Picture;
                base.OnPropertyChanged("Picture");
                Address = _parking.Address;
                base.OnPropertyChanged("Address");
                Cap = _parking.Cap.ToString();
                base.OnPropertyChanged("Cap");
                City = _parking.City;
                base.OnPropertyChanged("City");
                // Revenue
                double revenue = _parking.ParkingModel.PriceMin * 0.81;
                Revenue = "Guadagno: CHF " + revenue.ToString("N2") + " / h";
                base.OnPropertyChanged("Revenue");
                // Notes
                if (string.IsNullOrEmpty(_parking.ParkingModel.Notes))
                {
                    Notes = "";
                    IsNotesVisible = false;
                } else
                {
                    Notes = _parking.ParkingModel.Notes;
                    IsNotesVisible = true;
                }
                base.OnPropertyChanged("Notes");
                base.OnPropertyChanged("IsNotesVisible");

                // Status
                if (_parking.ParkingModel.Status == ParkingStatus.Enabled)
                {
                    ActiveStatusText = "Attivato";
                    ActiveSwitchToggled = true;
                } else {
                    ActiveStatusText = "Disattivato";
                    ActiveSwitchToggled = false;
                }
                base.OnPropertyChanged("ActiveStatusText");
                base.OnPropertyChanged("ActiveSwitchToggled");

                GetParkingCalendarEvents();
            }

            MyDayContent.ScrollTo((int)DateTime.Now.TimeOfDay.TotalMinutes);

            //ChangeSelectedDay(DateTime.Now.Date);
            ChangeSelectedDay(_profileService.LastEditingEventDate);

            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserParkingViewModel>();
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

        // Edit Parking Click action
        public void OnEditParkingMethod(object sender)
        {
            // TODO: Evaluate Add Parking page with element or EditParking Page to be added
            NavigationService.NavigateToAsync<AddParkingViewModel>(_parking.ParkingModel);
        }

        // Activate/Deactivate Parking toggle switch action
        public void OnSwitchToggleMethod(bool value)
        {
            bool changed = false;

            if (value)
            {
                ActiveStatusText = "Attivato";
                if (_parking.ParkingModel.Status != ParkingStatus.Enabled) {
                    _parking.ParkingModel.Status = ParkingStatus.Enabled;
                    changed = true;
                }
            }
            else
            {
                ActiveStatusText = "Disattivato";
                if (_parking.ParkingModel.Status != ParkingStatus.Disabled)
                {
                    _parking.ParkingModel.Status = ParkingStatus.Disabled;
                    changed = true;
                }
            }
            base.OnPropertyChanged("ActiveStatusText");

            // Update parking on backend
            if (changed)
            {
                UpdateParkingStatus();
            }
        }

        private async Task<bool> UpdateParkingStatus()
        {
            try {
                var result = await _parkingDataService.EditParkingAsync(_parking.ParkingModel);
                if (result != null) {
                    _parking.ParkingModel = result;
                    return true;
                }
            } catch (Exception e) {
                return false;
            }
            return false;
        }

        // Calendar Click action
        public void OnCalendarMethod(object sender)
        {
            // TODO: Open calendar and manage selected date
            _dialogService.ShowAlert("Alert", "TODO: open calendar");
        }

        // Add Availability Click action
        public void OnAddAvailabilityMethod(object sender)
        {
            // TODO: Add availability
            NavigationService.NavigateToAsync<AddEventViewModel>(new EventModel { ParkingId = _parking.UID, StartDate=SelectedDay.Date});
        }

        // Go to previous week
        public void OnPreviousWeekMethod(object data = null)
        {
            ChangeSelectedDay(SelectedDay.AddDays(-7.0));
        }

        // Go to next week
        public void OnNextWeekMethod(object data = null) 
        {
            ChangeSelectedDay(SelectedDay.AddDays(7.0));
        }

        // Day of the week selected
        public void OnDaySelectedMethod(object data)
        {
            if (data is DateTime) {
                ChangeSelectedDay((DateTime)data);
            }
        }

        void OnDateSelectedMethod(object sender, DateChangedEventArgs args)
        {
            ChangeSelectedDay(args.NewDate);
        }

        // Select day
        public void ChangeSelectedDay(DateTime dateTime)
        {
            if ((dateTime == null) || (dateTime.Equals(new DateTime(1900, 1, 1, 0, 0, 0)))) {
                dateTime = DateTime.Now.Date;
            }
            SelectedDay = dateTime;
            _pickerDateTime = dateTime;
            base.OnPropertyChanged("DatePickerDate");

            // Deselect all days
            Day1TextColor = Color.Gray;
            Day1BackgroundColor = Color.Transparent;
            Day2TextColor = Color.Gray;
            Day2BackgroundColor = Color.Transparent;
            Day3TextColor = Color.Gray;
            Day3BackgroundColor = Color.Transparent;
            Day4TextColor = Color.Gray;
            Day4BackgroundColor = Color.Transparent;
            Day5TextColor = Color.Gray;
            Day5BackgroundColor = Color.Transparent;
            Day6TextColor = Color.Gray;
            Day6BackgroundColor = Color.Transparent;
            Day7TextColor = Color.Gray;
            Day7BackgroundColor = Color.Transparent;

            int weekDay = (int)dateTime.DayOfWeek;

            switch(weekDay) {
                default:
                case 1: Day1TextColor = Color.White; Day1BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 2: Day2TextColor = Color.White; Day2BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 3: Day3TextColor = Color.White; Day3BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 4: Day4TextColor = Color.White; Day4BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 5: Day5TextColor = Color.White; Day5BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 6: Day6TextColor = Color.White; Day6BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
                case 0: Day7TextColor = Color.White; Day7BackgroundColor = (Color)Application.Current.Resources["NextParkColor1"]; break;
            }

            if (weekDay == 0) weekDay = 7;
            weekDay--;


            int[] dayText = new int[7];
            DateTime[] dayDateTime = new DateTime[7];

            DateTime setDateTime = dateTime;

            for (int i = 0; i < 7; i++) {
                int diff = i - weekDay;
                TimeSpan time = TimeSpan.FromDays(diff);
                setDateTime = dateTime.AddDays((double)diff);
                dayText[i] = setDateTime.Day;
                dayDateTime[i] = setDateTime;
            }

            Day1Text = dayText[0].ToString();
            Day1DateTime = dayDateTime[0];
            Day2Text = dayText[1].ToString();
            Day2DateTime = dayDateTime[1];
            Day3Text = dayText[2].ToString();
            Day3DateTime = dayDateTime[2];
            Day4Text = dayText[3].ToString();
            Day4DateTime = dayDateTime[3];
            Day5Text = dayText[4].ToString();
            Day5DateTime = dayDateTime[4];
            Day6Text = dayText[5].ToString();
            Day6DateTime = dayDateTime[5];
            Day7Text = dayText[6].ToString();
            Day7DateTime = dayDateTime[6];

            // Update Interface
            base.OnPropertyChanged("Day1Text");
            base.OnPropertyChanged("Day1TextColor");
            base.OnPropertyChanged("Day1BackgroundColor");
            base.OnPropertyChanged("Day1DateTime");
            base.OnPropertyChanged("Day2Text");
            base.OnPropertyChanged("Day2TextColor");
            base.OnPropertyChanged("Day2BackgroundColor");
            base.OnPropertyChanged("Day2DateTime");
            base.OnPropertyChanged("Day3Text");
            base.OnPropertyChanged("Day3TextColor");
            base.OnPropertyChanged("Day3BackgroundColor");
            base.OnPropertyChanged("Day3DateTime");
            base.OnPropertyChanged("Day4Text");
            base.OnPropertyChanged("Day4TextColor");
            base.OnPropertyChanged("Day4BackgroundColor");
            base.OnPropertyChanged("Day4DateTime");
            base.OnPropertyChanged("Day5Text");
            base.OnPropertyChanged("Day5TextColor");
            base.OnPropertyChanged("Day5BackgroundColor");
            base.OnPropertyChanged("Day5DateTime");
            base.OnPropertyChanged("Day6Text");
            base.OnPropertyChanged("Day6TextColor");
            base.OnPropertyChanged("Day6BackgroundColor");
            base.OnPropertyChanged("Day6DateTime");
            base.OnPropertyChanged("Day7Text");
            base.OnPropertyChanged("Day7TextColor");
            base.OnPropertyChanged("Day7BackgroundColor");
            base.OnPropertyChanged("Day7DateTime");

            _profileService.LastEditingEventDate = dateTime;

            if (eventsReady)
            {
                RefreshEvents(dateTime);
            }
        }

        public void RefreshEvents(DateTime dateTime) 
        {
            //var resultDeb = _eventDataService.DeleteSerieEventsAsync(301); 
            /*
            EventModel myEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2.5),
                RepetitionType = RepetitionType.Dayly,
                Id = 2
            };

            OrderModel myOrder = new OrderModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2.5),
                CarPlate = "TI 123456"
            };

            Orders.Add(myOrder);
            */

            // Clear all previous calendar events
            List<UICalendarEventModel> TempCalendarEvents = new List<UICalendarEventModel>();
            /*
            if (CalendarEvents == null) {
                CalendarEvents = new ObservableCollection<UICalendarEventModel>();
            } else {
                CalendarEvents.Clear();
            }

            if (Events.Count == 0)
            {
                Events.Add(new EventModel
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddHours(2),
                    RepetitionType = RepetitionType.None,
                    RepetitionEndDate = DateTime.Now.AddHours(2)
                });
            }
            */
            // Add availabilities
            foreach (EventModel availability in Events) {
                if ((availability.StartDate.Date <= dateTime) && (availability.EndDate.Date >= dateTime)) {
                    // Event for today
                    TimeSpan startPosition = availability.StartDate.TimeOfDay;
                    TimeSpan duration = availability.EndDate.TimeOfDay - availability.StartDate.TimeOfDay;

                    if (availability.RepetitionType == RepetitionType.None) {
                        // Check start position on dateTime
                        if (availability.StartDate.Date < dateTime.Date) {
                            startPosition = TimeSpan.FromMinutes(0); // start from a day before dateTime
                            // Update duration if availability ends on dateTime
                            duration = availability.EndDate.TimeOfDay;
                        }
                        // Check duration for dateTime
                        if (availability.EndDate.Date > dateTime.Date) {
                            // End some day after dateTime, in dateTime ends at 23:59
                            duration = TimeSpan.FromMinutes(1439)-startPosition;
                        }
                    } else {
                        if (availability.StartDate.TimeOfDay > availability.EndDate.TimeOfDay) {
                            continue;
                        }
                    }

                    if (duration.TotalMinutes <= 0) continue;

                    TempCalendarEvents.Add(new UICalendarEventModel
                    {
                        Index = TempCalendarEvents.Count,
                        Text = "Disponibile",
                        StartSeconds = (int)startPosition.TotalMinutes,
                        DurationSeconds = (int)duration.TotalMinutes,
                        EventColor = Color.FromHex("#8CC63F"),
                        TextColor = Color.DarkGreen,
                        yConstPosition = Constraint.RelativeToParent((parent) => { return parent.Y + 7 + (int)startPosition.TotalMinutes; }),
                        xConstPosition = Constraint.RelativeToParent((parent) => { return parent.Width * 0.025; }),
                        OnEventTap = OnEventTapAction,
                        Event = availability
                    });
                }
            }

            // Add orders
            foreach (OrderModel order in Orders)
            {
                if ((order.StartDate.Date <= dateTime) && (order.EndDate.Date >= dateTime))
                {
                    // Compute order start and end positions
                    TimeSpan start = order.StartDate.TimeOfDay;
                    TimeSpan end = order.EndDate.TimeOfDay;

                    if (order.StartDate < dateTime) start = TimeSpan.FromMinutes(0);
                    if (order.EndDate.Date > dateTime.Date) end = TimeSpan.FromMinutes(1439);

                    int startPosition = (int)start.TotalMinutes;

                    TimeSpan duration = end - start;

                    // Temporary put busy if car plate is not present
                    if (string.IsNullOrEmpty(order.CarPlate)) {
                        order.CarPlate = "Occupato";
                    }

                    TempCalendarEvents.Add(new UICalendarEventModel
                    {
                        Index = TempCalendarEvents.Count,
                        Text = order.CarPlate,
                        StartSeconds = 0,
                        DurationSeconds = (int)duration.TotalMinutes,
                        EventColor = (order.OrderStatus == Enums.OrderStatus.Finished) ? Color.LightSkyBlue: Color.Red,
                        TextColor = (order.OrderStatus == Enums.OrderStatus.Finished) ? Color.DarkBlue : Color.DarkRed,
                        yConstPosition = Constraint.RelativeToParent((parent) => { return parent.Y + 7 + startPosition; }),
                        xConstPosition = Constraint.RelativeToParent((parent) => { return parent.Width * 0.525; }),
                        OnEventTap = OnEventTapAction,
                        Order = order 
                    });
                }
            }

            CalendarEvents.Clear();
            CalendarEvents = new ObservableCollection<UICalendarEventModel>(TempCalendarEvents);
        }

        // Calendar event tapped
        public void OnEventTapMethod(int index)
        {
            // Check data
            if (index > CalendarEvents.Count) return;

            // Get calendar event
            UICalendarEventModel myCalendarEvent = CalendarEvents[index];

            if (myCalendarEvent.Event != null)
            {
                // Calendar event is an Availability event
                NavigationService.NavigateToAsync<AddEventViewModel>(myCalendarEvent.Event);
            }
            else if (myCalendarEvent.Order != null)
            {
                // Calendar event is an order

                // Order data
                OrderStartDateTime = myCalendarEvent.Order.StartDate.ToString("ddd, dd MMMMM  hh:mm");
                OrderEndDateTime = myCalendarEvent.Order.EndDate.ToString("ddd, dd MMMMM  hh:mm");
                OrderCarPlate = myCalendarEvent.Order.CarPlate;
                OrderDetailVisible = true;
                base.OnPropertyChanged("OrderStartDateTime");
                base.OnPropertyChanged("OrderEndDateTime");
                base.OnPropertyChanged("OrderCarPlate");
                base.OnPropertyChanged("OrderDetailVisible");

                //_dialogService.ShowAlert("Riservazione", ((myCalendarEvent.Order.CarPlate.Equals("Occupato")) ? "" : myCalendarEvent.Order.CarPlate + "\n") + "Dalle ore: " + myCalendarEvent.Order.StartDate.ToShortTimeString() + "\nAlle ore: " + myCalendarEvent.Order.EndDate.ToShortTimeString());
            }
        }

        private async void GetParkingCalendarEvents() 
        {
            if (_parking == null) return;

            try {
                // Get Events
                var eventsResult = await _eventDataService.GetAllEventsAsync();
                if (eventsResult.Count == 0) return;

                foreach (EventModel availability in eventsResult)
                {               
                    if (_parking.UID == availability.ParkingId) {
                        Events.Add(availability);
                    }
                }

                // Get orders
                var ordersResult = await _orderDataService.GetAllOrdersAsync();
                if ((ordersResult != null) && (ordersResult.Count > 0))
                {
                    foreach (OrderModel order in ordersResult)
                    {
                        if (_parking.UID == order.ParkingId)
                        {
                            Orders.Add(order);
                        }
                    }
                }
                eventsReady = true;
                RefreshEvents(SelectedDay);

            } catch (Exception e) {
                return;
            }
        }

        public void OnOrderDetailCloseMethod()
        {
            OrderDetailVisible = false;
            base.OnPropertyChanged("OrderDetailVisible");
        }
    }
}
