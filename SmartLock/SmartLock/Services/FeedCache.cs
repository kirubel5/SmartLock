using SmartLock.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartLock.Services
{
    public static class FeedCache
    {
        public static List<FeedModel> Feeds { get; set; } = new List<FeedModel>();
        
        //This method clears feed from cache
        public static void ClearFeedCache()
        {
            Feeds.Clear();
        }

        //This method caches the list received from the arduino
        public static void AddListToCache(List<FeedModel> models)
        {
            Feeds.AddRange(models);
        }
    }
}
