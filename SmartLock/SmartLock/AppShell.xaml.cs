using SmartLock.ViewModels;
using SmartLock.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SmartLock
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddPersonPage), typeof(AddPersonPage));
        }

    }
}
