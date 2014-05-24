using StingerCheck.Models;
using StingerCheck.Services;
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
        StingerContext db;
        TomatoClient tomato;

        public MovieController(StingerContext db, TomatoClient tomato)
        {
            this.db = db;
            this.tomato = tomato;
        }

        [ResponseType(typeof(Movie[]))]
        public async Task<IHttpActionResult> GetNowPlaying()
        {
            var movies = await tomato.GetNowPlaying();
            return Ok(movies);
        }

        [ResponseType(typeof(Movie))]
        public async Task<IHttpActionResult> GetByTomatoId(string id)
        {
            var movie = await db.Movies.FirstOrDefaultAsync(m => m.TomatoId == id);

            if (movie == null)
            {
                movie = await tomato.GetCachedMovie(id);
            }

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
