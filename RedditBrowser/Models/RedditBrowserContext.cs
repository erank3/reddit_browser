using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;

namespace RedditBrowser.Models
{
    public class RedditBrowserContext : DbContext
    {

        public RedditBrowserContext()
            : base("name=RedditBrowserContext")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .Property(t => t.username)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_username") { IsUnique = true }));

            modelBuilder.Entity<TagRelationship>()
                .Property(e => e.userId)
                .HasColumnAnnotation(
                 IndexAnnotation.AnnotationName,
                 new IndexAnnotation(new IndexAttribute("TagIndex", 1)));

            modelBuilder.Entity<TagRelationship>()
                .Property(e => e.postId)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("TagIndex", 2)));
        }

        public System.Data.Entity.DbSet<RedditBrowser.Models.UserModel> UserModels { get; set; }
        public System.Data.Entity.DbSet<RedditBrowser.Models.TagRelationship> Favorites { get; set; }

    }
}
