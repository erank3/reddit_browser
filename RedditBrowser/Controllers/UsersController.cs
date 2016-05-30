using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RedditBrowser.Models;

namespace RedditBrowser.Controllers
{
    public class UsersController : ApiController
    {
        private RedditBrowserContext db = new RedditBrowserContext();

        // GET: api/Users
        public IQueryable<UserModel> GetUserModels()
        {
            return db.UserModels;
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserModel))]
        public async Task<IHttpActionResult> GetUserModel(int id)
        {
            UserModel userModel = await db.UserModels.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            return Ok(userModel);
        }


        [Route("api/users/login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentException("invalid arguments");
            }

            var existingUser = await db.UserModels.SingleOrDefaultAsync(u => u.username == userModel.username && u.password == userModel.password);

            if (existingUser == null)
            {
                return BadRequest("invalid grant");
            }

            existingUser.accessToken = Guid.NewGuid().ToString();

            db.Entry(existingUser).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(existingUser.accessToken);
        }
       
        
        [Route("api/users/register")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("invalid arguments");
            }

            userModel.accessToken = Guid.NewGuid().ToString();
            
            db.UserModels.Add(userModel);
            await db.SaveChangesAsync();

            return Ok(userModel.accessToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserModelExists(int id)
        {
            return db.UserModels.Count(e => e.id == id) > 0;
        }
    }
}