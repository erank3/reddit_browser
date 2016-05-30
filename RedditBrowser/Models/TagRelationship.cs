using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RedditBrowser.Models
{
    public class TagRelationship
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        [MaxLength(450)]
        public string postId { get; set; }

        [ForeignKey("user")]
        public int userId { get; set; }
        public virtual UserModel user { get; set; }
    }
}