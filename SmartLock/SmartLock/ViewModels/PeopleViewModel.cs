using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartLock.Models;
using SmartLock.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartLock.ViewModels
{
    public class PeopleViewModel : ViewModelBase
    {
        #region Fields
        private bool isRefreshing;
        private IEnumerable<PersonModel> people;
        public ObservableRangeCollection<PersonModel> People { get; set; }
        public PersonModel SelectedPerson { get; set; }
        #endregion

        public PeopleViewModel()
        {
            people = new ObservableRangeCollection<PersonModel>();
            People = new ObservableRangeCollection<PersonModel>();
            SelectedPerson = new PersonModel();

            AddPersonCommand = new AsyncCommand(OnAddButtonClicked);
            SelectCommand = new AsyncCommand(OnSelect);
            ClearCommand = new AsyncCommand(OnClearButtonClicked);
            RefreshCommand = new AsyncCommand(OnRefresh);

            SelectedPerson = null;
        }

        #region Properties
        
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }
        #endregion

        #region Commands
        public ICommand AddPersonCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RefreshCommand { get; }
        #endregion

        #region Methods
        public async Task Load()
        {    
            SelectedPerson = null;
            People.Clear();

            if (PeopleCache.People == null || PeopleCache.People.Count == 0)
                return;
            else
                People.AddRange(PeopleCache.People);
        }

        public async Task OnRefresh()
        {
            IsRefreshing = true;

            try
            {
                await DataAccess.LoadData();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return;
            }

            SelectedPerson = null;
            People.Clear();

            if (PeopleCache.People == null || PeopleCache.People.Count == 0)
                return;
            else
                People.AddRange(PeopleCache.People);

            IsRefreshing = false;
        }

        private async Task OnSelect()
        {
            if (SelectedPerson == null)
                return;

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.DeleteFingerprintCommand);
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return;
            }

            string message = $"Are you sure you want to delete {SelectedPerson.Name}";
            bool result = await Shell.Current.DisplayAlert("Delete", message, "Delete", "Cancel");

            if (result)
            {
                try
                {
                    await DependencyService.Get<IConnectBT>()?.SendData((byte)int.Parse(SelectedPerson.Id));//send id to be deleted
                }
                catch (Exception)
                {
                    DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                    return;
                }
            }
            else
            {
                try
                {
                    await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.CancelDeleteOrder);//cancel delete fingerprint command
                    return;
                }
                catch (Exception)
                {
                    DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                    return;
                }
            }

            PeopleCache.DeleteFromCache(SelectedPerson.Id);//delete user from cache
            FileAccess.DeleteIdFromFile(SelectedPerson.Id);//delete id from id list
            await Task.Delay(1000);//sleep for a second before sending new list of people

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData(PeopleCache.GetListInSingleStringForm());
            }
            catch (Exception)
            {
                return;
            }

            DependencyService.Get<IToast>()?.MakeToast("Deleted successfully");
            await this.Load();
        }

        private async Task OnAddButtonClicked()
        {
            await Shell.Current.GoToAsync("AddPersonPage");
        }

        private async Task OnClearButtonClicked()
        {
            string message = $"Are you sure you want to clear people list?";
            bool result = await Shell.Current.DisplayAlert("Clear", message, "Clear", "Cancel");

            try
            {
                if (result)
                {
                    await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.ClearFingerprintDatabaseCommand);
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return;
            }

            DependencyService.Get<IToast>()?.MakeToast("Cleared successfully");
            PeopleCache.ClearPeopleCache();
            await this.Load();
        }

        #endregion
    }
}
