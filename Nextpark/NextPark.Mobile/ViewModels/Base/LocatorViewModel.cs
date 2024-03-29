﻿using NextPark.Mobile.Services;
using NextPark.Mobile.ViewModels;
using System;
using NextPark.Mobile.Settings;

namespace NextPark.Mobile.Infrastructure
{
    public class LocatorViewModel
    {
        public StartUpViewModel StartUp => IoCSettings.Resolve<StartUpViewModel>();
        public AddEventViewModel AddEvent { get { return (AddEventViewModel)IoCSettings.Resolve<AddEventViewModel>(); } }
        public AddParkingViewModel AddParking { get { return (AddParkingViewModel)IoCSettings.Resolve<AddParkingViewModel>(); } }
        public BookingMapViewModel BookMap { get { return (BookingMapViewModel)IoCSettings.Resolve<BookingMapViewModel>(); } }
        public BookingViewModel BookNow { get { return (BookingViewModel)IoCSettings.Resolve<BookingViewModel>(); } }
        public LaunchScreenViewModel Launch { get { return (LaunchScreenViewModel)IoCSettings.Resolve<LaunchScreenViewModel>(); } }
        public LoginViewModel Login { get { return (LoginViewModel)IoCSettings.Resolve<LoginViewModel>(); } }
        public MoneyViewModel Budget { get { return (MoneyViewModel)IoCSettings.Resolve<MoneyViewModel>(); } }
        public ParkingDataViewModel ParkingData { get { return (ParkingDataViewModel)IoCSettings.Resolve<ParkingDataViewModel>(); } }
        public PasswordViewModel Password { get { return (PasswordViewModel)IoCSettings.Resolve<PasswordViewModel>(); } }
        public PaymentViewModel Payment { get { return (PaymentViewModel)IoCSettings.Resolve<PaymentViewModel>(); } }
        public RegisterViewModel Register { get { return (RegisterViewModel)IoCSettings.Resolve<RegisterViewModel>(); } }
        public ReservationViewModel Reserve { get { return (ReservationViewModel)IoCSettings.Resolve<ReservationViewModel>(); } }
        public UserBookingViewModel UserBooking { get { return (UserBookingViewModel)IoCSettings.Resolve<UserBookingViewModel>(); } }
        public UserDataViewModel UserData { get { return (UserDataViewModel)IoCSettings.Resolve<UserDataViewModel>(); } }
        public UserParkingViewModel UserParking { get { return (UserParkingViewModel)IoCSettings.Resolve<UserParkingViewModel>(); } }
        public UserProfileViewModel Profile { get { return (UserProfileViewModel)IoCSettings.Resolve<UserProfileViewModel>(); } }

        //Only for test
        public TestViewModel Test { get { return (TestViewModel)IoCSettings.Resolve<TestViewModel>(); } }



        public HomeViewModel Home
        {
            get
            {
                HomeViewModel vm = null;

                try
                {
                    vm = IoCSettings.Resolve<HomeViewModel>();
                }
                catch (Exception e)
                {

                }
                return vm;
            }
        }
    }
}
