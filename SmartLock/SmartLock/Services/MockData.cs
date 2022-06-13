using SmartLock.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartLock.Services
{
    public class MockData
    {
        public List<PersonModel> People = new List<PersonModel>()
        {
            new PersonModel {Id = "1234", Name = "Solomon"},
            new PersonModel {Id= "2345", Name = "Tadese"},
            new PersonModel {Id = "3456", Name = "Abebe"},
            new PersonModel {Id= "4567", Name = "Abel"},
            new PersonModel {Id= "5678", Name = "Nahom"}
        };

        public List<FeedModel> Feeds = new List<FeedModel>()
        {
            new FeedModel {FeedId = "123", FeedImage = "feedImageOne", FeedDate = "19-05-2022", FeedTime = "09:40", FeedType = "Fing."},
            new FeedModel {FeedId = "234", FeedImage = "feedImageTwo", FeedDate = "18-05-2022", FeedTime = "16:00", FeedType = "Prox."},
            new FeedModel {FeedId = "345", FeedImage = "feedImageThree", FeedDate = "2-04-2022", FeedTime = "14:21", FeedType = "Fing."},
            new FeedModel {FeedId = "456", FeedImage = "feedImageFour", FeedDate = "31-03-2022", FeedTime = "11:21", FeedType = "Prox."},
        };
    }
}
