﻿using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class LaunchScreen : ContentPage
    {
        public LaunchScreen()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                bvm.InitializeAsync();
            }
        }
    }
}
