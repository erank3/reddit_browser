using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RedditBrowser.Models
{
    public class UserModel
    {
        [Key]
        public int id { get;  set; }

        [Required]
        [MaxLength(450)]
        public string username { get; set; }

        [Required]

        public string password {get; set; }

        [JsonIgnore]
        public string accessToken { get; set; }

        public virtual ICollection<TagRelationship> favorites { get; set; }
    }
}