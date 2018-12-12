﻿using NextPark.Mobile.Core.Resources;
using System.Globalization;
using Xamarin.Forms;

namespace NextPark.Mobile.Core.Services
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }

    public class LocalizationService : ILocalizationService
    {
        public string NotAvailable = "NotAvailable";

        public LocalizationService()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Localize.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }
        public string Accept { get { return Localize.Global_Accept ?? NotAvailable; } }
        public string Cancel { get { return Localize.Global_Cancel ?? NotAvailable; } }
        public string Error { get { return Localize.Global_Error ?? NotAvailable; } }

    }
}
