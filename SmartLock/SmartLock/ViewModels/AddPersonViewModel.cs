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
    public class AddPersonViewModel : ViewModelBase
    {
        #region Fields
        private string id;
        private string name;
        private string message;
        private bool messageIsVisible;
        #endregion

        public AddPersonViewModel()
        {
            SaveCommand = new AsyncCommand(OnSaveButtonClicked);
            Reload = new AsyncCommand(OnReloadButtonClicked);
        }

        #region Properties
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
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
        public ICommand SaveCommand { get; }
        public ICommand Reload { get; }

        #endregion

        #region Methods

        private async Task OnReloadButtonClicked()
        {
            MessageIsVisible = false;
            await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.RegisterFingerPrintCommand);
        }

        private async Task OnSaveButtonClicked()
        {
            Message = "";
            MessageIsVisible = false;

            #region Validate input
            
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Name Cannot be empty";
                MessageIsVisible = true;
                return;
            }

            #endregion

            MessageIsVisible = false;
            Id = FileAccess.GetNewId();

            try
            {
                await DependencyService.Get<IConnectBT>()?.SendData(Id, Name);
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Error while sendind Id");
                return;
            }

            MessageIsVisible = true;
            Message = "Place a finger, please.";

            await Task.Delay(500);
            int data = await DependencyService.Get<IConnectBT>()?.ReceiveInstraction();

            await Task.Delay(1000);
            Message = "Remove and place the same finger again.";
            await Task.Delay(1000);
            MessageIsVisible = false;

            await Task.Delay(2000);

            if (data == (int)Instruction.FingerprintSensorNotFound)
            {
                Message = "Fingerprint sensor could not be found, please try again.";
                MessageIsVisible = true;
                return;
            }
            else if (data == (int)Instruction.CommunicationError)
            {
                Message = "Communication error, please try again.";
                MessageIsVisible = true;
                return;
            }
            else if (data == (int)Instruction.ImagingError)
            {
                Message = "Imaging error, please try again.";
                MessageIsVisible = true;
                return;
            }
            else if (data == (int)Instruction.UnknownError)
            {
                Message = "Unknown error, please try again.";
                MessageIsVisible = true;
                return;
            }
            else if (data == (int)Instruction.ImageTooMessy)
            {
                Message = "Image too messy, please try again.";
                MessageIsVisible = true;
                return;
            }
            else if (data == (int)Instruction.FingerprintsDontMatch)
            {
                Message = "Fingerprints don't match, please try again.";
                MessageIsVisible = true;
                return;
            }
            else// if (data == (int)Instruction.FingerprintRegisteredSuccessfully)
            {
                PeopleCache.AddToCache(new PersonModel { Id = Id, Name = Name});
                FileAccess.AddIdToFile(Id);

                Name = "";
                Message = "Fingerprint registered successfully.";
                MessageIsVisible = true;
                await Task.Delay(2000);
                MessageIsVisible = false;
                return;
            }
        }

        #endregion
    }
}
