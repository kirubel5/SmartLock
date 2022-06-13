using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartLock.Models;
using SmartLock.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;

namespace SmartLock.ViewModels
{
    public class FeedViewModel : ViewModelBase
    {
        #region Fields
        private bool isRefreshing;
        private ObservableRangeCollection<FeedModel> feeds;
        public ObservableRangeCollection<FeedModel> Feeds { get; set; }
        #endregion

        public FeedViewModel()
        {
            feeds = new ObservableRangeCollection<FeedModel>();
            Feeds = new ObservableRangeCollection<FeedModel>();

            RefreshCommand = new AsyncCommand(OnRefresh);
            DownloadImageCommand = new AsyncCommand<object>(OnDownloadImageButtonClicked);
            ClearCommand = new AsyncCommand(OnClearButtonClicked);
        }

        #region Properties
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        } 
        #endregion

        #region Commands
        public ICommand RefreshCommand { get; }
        public ICommand DownloadImageCommand { get; }
        public ICommand ClearCommand { get; }
        #endregion

        #region Methods
        public void Load()
        {
            MatchImages();
            Feeds?.Clear();
            Feeds.AddRange(feeds);            
        }

        private void MatchImages()
        {
            feeds?.Clear();

            string[] images = DependencyService.Get<IFileService>()?.LoadPicture();

            foreach (var item in images)
            {
                int a = item.LastIndexOf('/');
                string b = item.Substring(a + 1, 12);

                FeedModel model = new FeedModel
                {
                    FeedId = b.Substring(0, 12),
                    FeedTime = b[0].ToString() + b[1].ToString() + ":" + b[2].ToString() + b[3].ToString(),
                    FeedDate = b[4].ToString() + b[5].ToString() + "-" + b[6].ToString() + b[7].ToString() + "-" + 
                               b[8].ToString() + b[9].ToString() + b[10].ToString() + b[11].ToString(),
                    FeedType = this.GetFeedType(),
                     FeedImage = item
                };
                feeds.Add(model);                
            }
        }

        private string GetFeedType()
        {
            Random rand = new Random();
            int a = rand.Next(1, 3);

            if (a == 1)
                return "Prox.";
            else
                return "Fing.";
        }

        private async Task OnRefresh()
        {
            IsRefreshing = true;
            await Task.Delay(200);
            IsRefreshing = false;
        }

        private async Task OnDownloadImageButtonClicked(object obj)
        {
            var cont = obj as FeedModel;

            var memoryStream = new MemoryStream();

            using (var source = File.OpenRead(cont.FeedImage))
            {
                await source.CopyToAsync(memoryStream);
            }

            try
            {
                DependencyService.Get<IFileService>()?.SaveImage(cont.FeedId + ".jpg", memoryStream);
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>()?.MakeToast("Something went wrong, please try again.");
            }

            DependencyService.Get<IToast>()?.MakeToast("Image downloaded successfully.");
        }

        private async Task OnClearButtonClicked()
        {
            string message = $"Are you sure you want to clear feed list?";
            bool result = await Shell.Current.DisplayAlert("Clear", message, "Clear", "Cancel");

            try
            {
                if (result)
                {
                    await DependencyService.Get<IConnectBT>()?.SendData((byte)Instruction.ClearFeedCommand);
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
            FeedCache.ClearFeedCache();
            this.Load();
        }
        #endregion
    }
}
