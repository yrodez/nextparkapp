﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Settings;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        //fields
        private readonly IEventDataService _eventDataService;
        private readonly IParkingDataService _parkingDataService;
        private readonly IOrderDataService _orderDataService;
        private readonly IPurchaseDataService _purchaseDataService;

        private bool _startButtonEnabled;
        private string _resultConsole;
        private StringBuilder _consoleBuffer;

        public UserModel LoggedUser { get; set; }

        public TestViewModel(IApiService apiService, IAuthService authService, INavigationService navService,
            IEventDataService eventDataService, IParkingDataService parkingDataService, IOrderDataService orderDataService, IPurchaseDataService purchaseDataService) : base(apiService, authService,
            navService)
        {
            _eventDataService = eventDataService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;
            _purchaseDataService = purchaseDataService;

            CleanConsoleAsync();

        }


        //Properties
        public bool StartButtonEnabled
        {
            get => _startButtonEnabled;
            set => SetValue(ref _startButtonEnabled, value);
        }


        public string ResultConsole
        {
            get => _resultConsole;
            set => SetValue(ref _resultConsole, value);
        }

        //Commands
        public ICommand StartTesting => new Command(StartTestingAsync);
        public ICommand CleanConsole => new Command(CleanConsoleAsync);

        private async void StartTestingAsync()
        {
            StartButtonEnabled = false;

            await AuthServiceTest();

            await ParkingServiceTest();
            await OrderServiceTest().ConfigureAwait(false);
            await EventServiceTest();
            PurchaseServiceTest();

            await LogoutTest();

            StartButtonEnabled = true;
        }

        private void CleanConsoleAsync()
        {
            _consoleBuffer = new StringBuilder();
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            _consoleBuffer.Append("STARTING SERVICES TESTING...");
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            ResultConsole = _consoleBuffer.ToString();
        }

        private void AddLineToConsole(string data)
        {
            _consoleBuffer.AppendLine(data);
            ResultConsole = string.Empty;
            ResultConsole = _consoleBuffer.ToString();
        }

        #region Auth Testing
        private async Task AuthServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING LOGIN...");

            //Demo Login OK
            var loginResponse = await AuthService.Login("info@nextpark.ch", "NextPark.1");

            if (loginResponse.IsSuccess)
            {
                AddLineToConsole("Login Ok");
                AddLineToConsole($"User id: {loginResponse.UserId}");
                AddLineToConsole($"User name: {loginResponse.UserName}");

                AddLineToConsole("Getting full user information...");

                var userData = await AuthService.GetUserByUserName(loginResponse.UserName);

                if (userData.IsSuccess)
                {
                    AddLineToConsole("Full user information OK");
                    this.LoggedUser = userData.Result as UserModel;
                }
                else
                {
                    AddLineToConsole("Full user information FAILED");
                }
            }
            else
            {
                AddLineToConsole("Login FAILED");
            }

            AddLineToConsole("TESTING REGISTER...");
            //Demo Register OK
            var demoUser = new RegisterModel
            {
                Address = "Via Demo User",
                CarPlate = "TI 00DEMO00",
                Email = "demo@nextpark.ch",
                Lastname = "Demo",
                Name = "User",
                Password = "Wisegar.1",
                State = "DemoState",
                UserName = "demo@nextpark.ch",
                Phone = "00410000000",
                Cap = 11111,
                City = "Vezia"
            };

            var registerResponse = await AuthService.Register(demoUser);

            if (registerResponse.IsSuccess)
            {
                AddLineToConsole("Register OK");
                AddLineToConsole($"User token: {registerResponse.AuthToken}");
                AddLineToConsole($"User id: {registerResponse.UserId}");
            }
            else
            {
                AddLineToConsole("Register FAILED");
            }
        }
        #endregion

        #region Parking Test




        private async Task ParkingServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING PARKING SERVICE...");

            AddLineToConsole("Getting all parkings");
            var parkings = await _parkingDataService.GetAllParkingsAsync();

            AddLineToConsole($"Found: {parkings.Count} parkings");


            var parking1 = new ParkingModel
            {
                Address = "Via Strada",
                Cap = 7777,
                City = "Lugano",
                CarPlate = "TI 000000",
                Latitude = 40,
                Longitude = 40,
                PriceMax = 4,
                PriceMin = 4,
                State = "Ticino",
                Status = Enums.Enums.ParkingStatus.Enabled,
                UserId = 1,
                ImageUrl = $"{ApiSettings.BaseUrl}/images/image_parking1.png"
            };

            AddLineToConsole("Creating one parking");
            var postedParking = await _parkingDataService.CreateParkingAsync(parking1);

            if (postedParking != null)
            {
                AddLineToConsole("Creating the parking OK");
            }
            else
            {
                AddLineToConsole("Creating the parking FAILED");
            }

            var eventParking = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(5),
                ParkingId = postedParking.Id,
                RepetitionEndDate = DateTime.Now,
                RepetitionType = Enums.Enums.RepetitionType.Dayly
            };

            // postedParking.Status = Enums.Enums.ParkingStatus.Disabled;

            var result = await _eventDataService.CreateEventAsync(eventParking);

            AddLineToConsole("Editing the parking");
            var parkingResult = await _parkingDataService.EditParkingAsync(postedParking);

            if (parkingResult != null)
            {
                AddLineToConsole("Editing the parking OK");
            }
            else
            {
                AddLineToConsole("Editing the parking FAILED");
            }

            AddLineToConsole("Deleting the parking");
            var deletedParking = await _parkingDataService.DeleteParkingsAsync(parkingResult.Id);
            if (deletedParking != null)
            {
                AddLineToConsole("Deleting the parking OK");
            }
            else
            {
                AddLineToConsole("Deleting the parking FAILED");
            }

        }

        #endregion

        #region Event Testing
        private async Task EventServiceTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING EVENTS...");

            AddLineToConsole("Selecting one parking");
            var parkings = await _parkingDataService.GetAllParkingsAsync();

            AddLineToConsole($"Found: {parkings.Count} parkings");

            var selectedPArking = parkings.FirstOrDefault();
            if (selectedPArking != null)
            {
                AddLineToConsole("Selecting one parking OK");
            }
            else
            {
                AddLineToConsole("Selecting one parking FAILED");
                AddLineToConsole("Events testing FAILED");
                return;
            }

            AddLineToConsole("Creating monthly event for the selected parking");

            var newMonthlyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(3),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
                RepetitionType = RepetitionType.Monthly,
                MonthlyRepeatDay = new List<int>
                {
                    10,
                    15,
                    23
                }
            };

            var createdMonthlyEvents = await _eventDataService.CreateEventAsync(newMonthlyEvent);

            if (createdMonthlyEvents != null)
            {
                AddLineToConsole($"Created: {createdMonthlyEvents.Count} monthly events for the selected parking");
                AddLineToConsole("Creating monthly events OK");
            }
            else
            {
                AddLineToConsole("Creating monthly events FAILED");
            }

            AddLineToConsole("Creating weekly event for the selected parking");

            var newWeklyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(4),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = (DateTime.Now).AddDays(4),
                RepetitionType = RepetitionType.Weekly,
                WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Friday
                }
            };

            var createdWeeklyEvents = await _eventDataService.CreateEventAsync(newWeklyEvent);

            if (createdWeeklyEvents != null)
            {
                AddLineToConsole($"Created: {createdWeeklyEvents.Count} weekly events for the selected parking");
                AddLineToConsole("Creating weekly events OK");
            }
            else
            {
                AddLineToConsole("Creating newWeklyEvent events FAILED");
            }

            AddLineToConsole("Creating dayly event for the selected parking");

            var newDaylyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(4),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = (DateTime.Now).AddDays(4),
                RepetitionType = RepetitionType.Dayly,
            };

            var createdDaylyEvents = await _eventDataService.CreateEventAsync(newDaylyEvent);

            if (createdDaylyEvents != null)
            {
                AddLineToConsole($"Created: {createdWeeklyEvents.Count} dayly events for the selected parking");
                AddLineToConsole("Creating dayly events OK");
            }
            else
            {
                AddLineToConsole("Creating dayly events FAILED");
            }

            AddLineToConsole("Getting all events of the selected parking");
            var parkingEvents = await _parkingDataService.GetParkingEventsAsync(selectedPArking.Id);

            if (parkingEvents != null)
            {
                AddLineToConsole($"Fount: {parkingEvents.Count} events for the selected parking");
                AddLineToConsole("Getting parking events OK");
            }
            else
            {
                AddLineToConsole("Getting parking events FAILED");
            }

            var firstRepitedEvent = parkingEvents.FirstOrDefault(e => e.RepetitionId != Guid.Empty);
            if (firstRepitedEvent != null)
            {
                AddLineToConsole("Editing all events of the selected parking by series");
                AddLineToConsole("Editing all events repetition end date to more 5 days");
                firstRepitedEvent.RepetitionEndDate = firstRepitedEvent.RepetitionEndDate.AddDays(5);

                var editedEvents = await _eventDataService.EditSerieEventsAsync(firstRepitedEvent);

                if (editedEvents != null)
                {

                    AddLineToConsole("Editing parking events by series OK");
                }
                else
                {
                    AddLineToConsole("Editing parking events by series FAILED");
                }

                AddLineToConsole("Deleting all events of the selected parking  by series ");
                var deletedSeriesParkingEvents = await _eventDataService.DeleteSerieEventsAsync(firstRepitedEvent.Id);

                if (deletedSeriesParkingEvents != null)
                {
                    AddLineToConsole($"Deleted: {deletedSeriesParkingEvents.Count} events for the selected parking  by series");

                    AddLineToConsole("Getting parking events  by series OK");
                }
                else
                {
                    AddLineToConsole("Getting parking events by series FAILED");
                }

            }

            AddLineToConsole("Deleting all events of the selected parking");
            var deletedParkingEvents = await _parkingDataService.DeleteParkingsEventsAsync(selectedPArking.Id);

            if (deletedParkingEvents != null)
            {
                AddLineToConsole($"Deleted: {deletedParkingEvents.Count} events for the selected parking");

                AddLineToConsole("Getting parking events OK");
            }
            else
            {
                AddLineToConsole("Getting parking events FAILED");
            }

        }
        #endregion

        #region Order Service
        private async Task OrderServiceTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING ORDER SERVICE...");

            var order = new OrderModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ParkingId = 1,
                UserId = 1,
                Price = 20,
                PaymentCode = "drtrtrt",
                OrderStatus = Enums.OrderStatus.Actived,
                PaymentStatus = PaymentStatus.Pending
            };
            AddLineToConsole("Creating one order...");
            var postedOrder = await _orderDataService.CreateOrderAsync(order).ConfigureAwait(false);
            AddLineToConsole(postedOrder != null ? "Creating the order OK" : "Creating the order FAILED");
            if (postedOrder != null)
            {
                AddLineToConsole("Editing the order...");
                postedOrder.PaymentStatus = PaymentStatus.Cancel;
                var editedOrder = _orderDataService.EditOrderAsync(postedOrder.Id, postedOrder);
                AddLineToConsole(editedOrder != null ? "Editing the order OK" : "Editing the order FAILED");
                AddLineToConsole("Terminate the order...");
                var finishedOrder = _orderDataService.TerminateOrderAsync(postedOrder.Id);
                AddLineToConsole(finishedOrder != null ? "Terminate the order OK" : "Terminate the order FAILED");
            }
        }
        #endregion

        #region Purchase Service
        private void PurchaseServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING PURCHASE SERVICE...");

            AddLineToConsole("Adding money...");
            var purchaseModel = _purchaseDataService.BuyAmount(new PurchaseModel { CashToAdd = 10, UserId = this.LoggedUser.Id });

            if (purchaseModel != null)
            {
                AddLineToConsole("Adding money... OK");

            }
            else
            {
                AddLineToConsole("Adding money... FAILED");
            }

        }
        #endregion

        #region Logout testing
        private async Task LogoutTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING LOGOUT...");

            //Demo Login OK
            var loginResponse = await AuthService.Logout();
            if (loginResponse.IsSuccess)
            {
                AddLineToConsole("Logout OK");
            }
            else
            {
                AddLineToConsole("Logout FAILED");
            }

        }

        #endregion

    }
}