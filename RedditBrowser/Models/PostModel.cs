using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedditBrowser.Models
{
    public class PostModel
    {

        [Key]
        [MaxLength(450)]
        public string id { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
        public string author { get; set; }

    }

    //A helper class to reutrn flag results
    public class PostFlagModel : PostModel
    {
        public string tag_name { get; set; }

        public PostFlagModel(PostModel model)
        {
            base.id = model.id;
            base.title = model.title;
            base.permalink = model.permalink;
            base.url = model.url;
            base.author = model.author;
        }
    }
}