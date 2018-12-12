﻿using System.Threading.Tasks;
using Acr.UserDialogs;

namespace NextPark.Mobile.Services
{
    public class DialogService : IDialogService
    {
        private readonly ILocalizationService _localizationService;
        public DialogService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public Task ShowAlert(string title, string message)
        {
            return UserDialogs.Instance.AlertAsync(message, title, _localizationService.Accept);
        }
        public async Task<bool> ShowConfirmAlert(string title, string message)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(message, title, _localizationService.Accept, _localizationService.Cancel);
            return result;
        }
        public Task ShowErrorAlert(string message)
        {
            return UserDialogs.Instance.AlertAsync(message, _localizationService.Error, _localizationService.Accept);
        }

        public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }
    }
}
