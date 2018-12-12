﻿using System;
using System.Threading.Tasks;
using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Services
{
    public interface INavigationService
    {
        Task ClearBackStack();
        Task InitializeAsync();
        Task NavigateBackAsync();
        Task NavigateToAsync(Type viewModelType);
        Task NavigateToAsync(Type viewModelType, object parameter);
        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;
        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;
        Task PopToRootAsync();
        Task RemoveLastFromBackStackAsync();
    }
}