using RedditBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Dynamic;

namespace RedditBrowser.Controllers
{
    public class RedditController : ApiController
    {
        readonly string redditEndpointUrl = "https://www.reddit.com";

        private RedditBrowserContext db = new RedditBrowserContext();
        /// <summary>
        /// Gets a list of listings ids and query reddit for the data. 
        /// We need this since we DON'T store any reddit information. 
        /// For tags we just store the post_id and then bulk fetch it when users want to gets his flags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task<IEnumerable<PostFlagModel>> getRedditPostsByName(TagRelationship[] tags)
        {
            var retval = new List<PostFlagModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(redditEndpointUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //I ASSUME ALL POSTS ARE LITINGS HENCE FROM TYPE T3
                var posts_with_affix = tags.Select(x => "t3_" + x.postId);
                var queryString = string.Join(",", posts_with_affix);

                HttpResponseMessage response = await client.GetAsync(string.Format("/by_id/{0}.json", queryString));
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsAsync<RedditResponse>();
                   
                    foreach (var pd in res.data.children)
                    {
                        var flag_post = new PostFlagModel(pd.data);

                        //match flag name to returned data
                        flag_post.tag_name = tags.Where(t => t.postId == pd.data.id).Single().name;
                        retval.Add(flag_post);
                    }
                }
            }

            return retval;
        }

        [Route("api/reddit/tag")]
        [HttpPost]
        public async Task<IHttpActionResult> tag(string access_token, string reddit_id, string tag_name)
        {
            var existingUser = await db.UserModels.SingleOrDefaultAsync(u => u.accessToken == access_token);
            if (existingUser == null)
            {
                return BadRequest("invalid grant");
            }
            //assuming user can only have 1 flag per post
            var existingTag = await db.Favorites.FirstOrDefaultAsync(t => t.userId == existingUser.id && t.postId == reddit_id);

            if (existingTag != null)
            {
                db.Entry(existingTag).State = EntityState.Modified;
                existingTag.name = tag_name;
            } 
            else
            {
                var tag_link = new TagRelationship() { name = tag_name, postId = reddit_id };
                db.Favorites.Add(tag_link);
                existingUser.favorites.Add(tag_link);
            }

            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("api/reddit/tag")]
        [HttpGet]
        public async Task<IHttpActionResult> tag(string access_token)
        {
            var existingUser = await db.UserModels.SingleOrDefaultAsync(u => u.accessToken == access_token);
            if (existingUser == null)
            {
                return BadRequest("invalid grant");
            }
            
            //get current user tags
            var tags = db.Favorites.Where(fav => fav.userId == existingUser.id);

            //fetch current user post by tags
            var posts = await getRedditPostsByName(tags.ToArray());

            return Ok(posts);
        }

        public async Task<IHttpActionResult> GetPosts(string access_token)
        {
            var existingUser = await db.UserModels.SingleOrDefaultAsync(u => u.accessToken == access_token);
            if (existingUser == null)
            {
                return BadRequest("invalid grant");
            }

            var retval = new List<PostModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(redditEndpointUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("hot.json");
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsAsync<RedditResponse>();
                    retval = res.data.children.Select(p => p.data).ToList();
                }
            }


            return Ok(retval);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
