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
    public partial class PeoplePage : ContentPage
    {
        private readonly PeopleViewModel peopleViewModel;

        public PeoplePage()
        {
            InitializeComponent();

            peopleViewModel = new PeopleViewModel();
            BindingContext = peopleViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await peopleViewModel.Load();
        }
    }
}