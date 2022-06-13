using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartLock.Models
{
    public class FeedModel
    {        
        public string FeedId { get; set; }
        public string FeedDate { get; set; }
        public string FeedTime { get; set; }
        public string FeedImage { get; set; }
        public string FeedType { get; set; }        
    }
}
