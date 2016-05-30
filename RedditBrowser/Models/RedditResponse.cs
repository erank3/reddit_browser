using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditBrowser.Models
{
    public class RedditPost
    {
        public string kind { get; set; }
        public PostModel data { get; set; }
    }
    public class RedditData
    {
        public RedditPost[] children { get; set; }
    }
    public class RedditResponse
    {

        public string kind { get; set; }

        public RedditData data { get; set; }
    }

}