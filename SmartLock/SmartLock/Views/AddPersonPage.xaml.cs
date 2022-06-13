using SmartLock.Services;
using SmartLock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartLock.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPersonPage : ContentPage
    {
        private readonly AddPersonViewModel addPersonViewModel;
        public AddPersonPage()
        {
            InitializeComponent();

            addPersonViewModel = new AddPersonViewModel();
            BindingContext = addPersonViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await DependencyService.Get<IConnectBT>()?.SendData((byte)50);
        }
    }
}