using MvvmHelpers.Commands;
using SmartLock.Models;
using SmartLock.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartLock.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields
        private string pin;
        private string message;
        private bool messageIsVisible;

        private bool _connected = false;
        #endregion

        public LoginViewModel()
        {
            LoginCommand = new AsyncCommand(OnLoginButtonClicked);
            Command3 = new AsyncCommand(OnButton3Clicked);
        }

        #region Properties
        public string Pin
        {
            get => pin;
            set => SetProperty(ref pin, value);
        }
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
        public bool MessageIsVisible
        {
            get => messageIsVisible;
            set => SetProperty(ref messageIsVisible, value);
        }
        #endregion

        #region Commands
        public ICommand LoginCommand { get; }
        public ICommand Command3 { get; }
        #endregion

        #region Methods
        private async Task OnLoginButtonClicked()
        {
            MessageIsVisible = false;

            #region PinValidation
            if (string.IsNullOrWhiteSpace(Pin))
            {
                Message = "Pin cannot be empty.";
                MessageIsVisible = true;
                return;
            }

            if (!int.TryParse(Pin, out _))
            {
                Message = "Pin has to a number.";
                MessageIsVisible = true;
                return;
            }
            #endregion

            if (_connected)
            {
                if (await this.PinIsCorrect())
                {
                    await this.LoadData();
                    await Shell.Current.GoToAsync("//PeoplePage");
                }
            }               
            else
            {
                await this.ConnectionMade();

                if (await this.PinIsCorrect())
                {
                    await this.LoadData();
                    await Shell.Current.GoToAsync("//PeoplePage");
                }
            }
        }

        private async Task<bool> ConnectionMade()
        {
            MessageIsVisible = false;
            Message = "Connecting...";

            int connection = 0;

            try
            {
                MessageIsVisible = true;
                Message = "Connecting...";

                connection = await DependencyService.Get<IConnectBT>()?.MakeConnection(@"HC-05");
            }
            catch (Exception e)
            {
                string a = e.ToString();
                MessageIsVisible = false;
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again.");
                _connected = false;
                return false;
            }

            if (connection == 1)
            {
                MessageIsVisible = false;
                DependencyService.Get<IToast>()?.MakeToast("Adapter could not be found, please try again.");
                _connected = false;
                return false;
            }
            else if (connection == 2)
            {
                MessageIsVisible = false;
                DependencyService.Get<IToast>()?.MakeToast("Adapter is not enabled, please try again.");
                _connected = false;
                return false;
            }
            else if (connection == 3)
            {
                MessageIsVisible = false;
                DependencyService.Get<IToast>()?.MakeToast("Device could not be found, please try again.");
                _connected = false;
                return false;
            }

            MessageIsVisible = false;
            DependencyService.Get<IToast>()?.MakeToast("Connection made successfully");
            _connected = true;
            return true;
        }

        private async Task LoadData()
        {
            MessageIsVisible = true;
            Message = "Loading data, please wait";

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.LoadDataCommand);
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return;
            }

            string data = "";
            try
            {
                data = await DependencyService.Get<IConnectBT>()?.ReceiveData();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return;
            }

            this.FormatData(data);
        }

        private async Task<bool> PinIsCorrect()
        {
            MessageIsVisible = true;
            Message = "Checking pin";

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.CheckPin);
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return false;
            }

            await Task.Delay(500);

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData(Pin);//send the pin
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return false;
            }

            int inst = 0;
            try
            {
                inst = await DependencyService.Get<IConnectBT>()?.ReceiveInstraction();
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again");
                return false;
            }

            if (inst == 0)
            {
                MessageIsVisible = true;
                Message = "Something is not right, please try again.";
                return false;
            }
            else if (inst == (int)Instruction.PinNotRegistered)
            {
                DependencyService.Get<IToast>()?.MakeToast("New pin registered");
            }
            else if (inst == (int)Instruction.PinIncorrect)
            {
                MessageIsVisible = true;
                Message = "Incorrect pin, please try again.";
                return false;
            }
            else if (inst == (int)Instruction.PinCorrect)
            {
                DependencyService.Get<IToast>()?.MakeToast("Login successful");
            }
            else
            {
                DependencyService.Get<IToast>()?.MakeToast("Something is not right, please try again.");
                return false;
            }

            return true;
        }

        private void FormatData(string data)
        {
            string[] rawData = data.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries);

            if (rawData.Length != 2)
                return;

            string[] peopleData = rawData[0].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] feedData = rawData[1].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<PersonModel> people = new List<PersonModel>();
            List<FeedModel> feeds = new List<FeedModel>();

            foreach (var item in peopleData)
            {
                if (item.Length > 1)
                {
                    string[] col = item.Split(new char[] { ' ' });

                    if (col.Length == 2)
                    {
                        PersonModel person = new PersonModel { Id = col[0], Name = col[1] };
                        people.Add(person);
                    }
                }
            }

            foreach (string item in feedData)
            {
                if (item.Length != 13)
                    continue;

                FeedModel feed = new FeedModel
                {
                    FeedId = item.Substring(0, 12),
                    FeedTime = item[0].ToString() + item[1].ToString() + ":" + item[2].ToString() + item[3].ToString(),
                    FeedDate = item[4].ToString() + item[5].ToString() + "-" + item[6].ToString() + item[7].ToString() + "-" + item[8].ToString() + item[9].ToString() + item[10].ToString() + item[11].ToString(),
                    FeedType = item[12].ToString()
                };
                feeds.Add(feed);
            }

            foreach (var item in feeds)
            {
                if (item.FeedType == "1")
                {
                    item.FeedType = "Prox.";
                }
                else if (item.FeedType == "2")
                {
                    item.FeedType = "Fing.";
                }
            }

            if (people == null || people.Count == 0)
            { }
            else
            {
                PeopleCache.AddListToCache(people);
            }

            if (feeds == null || feeds.Count == 0)
            { }
            else
            {
                FeedCache.AddListToCache(feeds);
            }
        }

        //for debug only
        private async Task OnButton3Clicked()
        {
            await Shell.Current.GoToAsync("//PeoplePage");
        }
        #endregion
    }
}
