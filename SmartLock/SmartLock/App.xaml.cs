//using SmartLock.Services;
using SmartLock.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartLock
{
    public partial class App : Application
    {

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjE2MjM3QDMyMzAyZTMxMmUzMFZWVEI2TFhmemxZSWVJVVphTExLQ1B5a1VHdngvY0ljcmtaMmZxa2ZPQjQ9");
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            await Shell.Current.GoToAsync("//LoginPage");
            //    await Shell.Current.GoToAsync("//FeedPage");
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
