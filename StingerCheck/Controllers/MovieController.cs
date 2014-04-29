using StingerCheck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StingerCheck.Controllers
{
    public class MovieController : ApiController
    {
        public StingerContext db = new StingerContext();

        [ResponseType(typeof(Movie[]))]
        public async Task<IHttpActionResult> GetNowPlaying()
        {
            var movies = await db.Movies.Take(10) // TODO: Tomato API lookup
                .ToArrayAsync();
            return Ok(movies);
        }

        [ResponseType(typeof(Movie))]
        public async Task<IHttpActionResult> GetByTomatoId(long id)
        {
            var movie = await db.Movies.FirstOrDefaultAsync(m => m.TomatoId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
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
