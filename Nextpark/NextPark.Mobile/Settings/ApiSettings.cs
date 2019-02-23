using System;
namespace NextPark.Mobile.Settings
{
    public static class ApiSettings
    {
        public static string BaseUrl = "https://nextpark-api.azurewebsites.net";
        //"https://nextpark-api.azurewebsites.net";// "http://10.211.55.3/NextPark.Api";
        //public static int BasePort = 80;
        //public static string BaseUrl = string.Format("{0}:{1}", BaseUri, BasePort);  

        public static string AuthEndPoint => string.Format("{0}/api/auth", BaseUrl);
        public static string ProfileEndPoint => string.Format("{0}/api/profile", BaseUrl);
        public static string OrdersEndPoint => string.Format("{0}/api/order", BaseUrl);
        public static string ParkingCategoriesEndPoint => string.Format("{0}/api/parkingcategories", BaseUrl);
        public static string ParkingTypesEndPoint => string.Format("{0}/api/parkingtypes", BaseUrl);
        public static string ParkingsEndPoint => string.Format("{0}/api/parkings", BaseUrl);
        public static string EventsEndPoint => string.Format("{0}/api/events", BaseUrl);
        public static string PurchaseEndPoint => string.Format("{0}/api/purchase", BaseUrl);

    }
}
