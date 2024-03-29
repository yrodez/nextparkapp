﻿using System;
using System.Collections.Generic;
using NextPark.Mobile.Controls;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class ParkingDataPage : ContentPage
    {
        public ParkingDataPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;

            ParkingDataViewModel bvm = BindingContext as ParkingDataViewModel;
            bvm.MyDayContent = dayView;
        }
    }
}
