using SmartLock.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartLock.Services
{
    public static class DataAccess
    {
        //Get data from bluetooth
        public static async Task LoadData()
        {
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

            FormatData(data);
        }

        //Edit the data from bluetooth and add them cache
        public static void FormatData(string data)
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
    }
}
